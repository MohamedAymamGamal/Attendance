using Job.Models;
using Job.Models.Enums;
using Job.Reposities;
using Job.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace Job.Jobs
{
    public class AttendanceJob
    {
        private readonly AttendanceRepo _repo;
        private readonly UserManager<AppUsers> _userManager;
        private readonly ILogger<AttendanceJob> _logger;
        private readonly AttendanceExcelService _excel;

        public AttendanceJob(
            AttendanceRepo repo,
            UserManager<AppUsers> userManager,
            ILogger<AttendanceJob> logger,
            AttendanceExcelService excel)
        {
            _repo = repo;
            _userManager = userManager;
            _logger = logger;
            _excel = excel;
        }

        public async Task Execute()
        {
            _logger.LogInformation("[AttendanceJob] Starting at {Time}", DateTime.Now);

            try
            {
                var today = DateOnly.FromDateTime(DateTime.Now);

                // Fetch settings ONCE per admin instead of per-user inside loops
                var settingsByAdmin = await _repo.GetAllSettingsAsync();

                await AutoCheckOutActiveUsersAsync(settingsByAdmin);
                await MarkAbsentUsersAsync(today, settingsByAdmin);
                await ExportToExcelAsync(today);

                _logger.LogInformation("[AttendanceJob] Completed at {Time}", DateTime.Now);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AttendanceJob] Failed at {Time}", DateTime.Now);
                throw;
            }
        }

        private async Task AutoCheckOutActiveUsersAsync(Dictionary<string, WorkSettings> settingsByAdmin)
        {
            var stillActive = await _repo.GetStillActiveAsync();
            if (stillActive.Count == 0)
            {
                _logger.LogInformation("[AttendanceJob] No active users to check out.");
                return;
            }

            var fallbackCheckout = DateTime.Today.AddHours(17);

            foreach (var record in stillActive)
            {
                var adminId = record.User?.CreatedByAdminId;
                var checkoutTime = adminId != null && settingsByAdmin.TryGetValue(adminId, out var settings)
                    ? DateTime.Today.Add(settings.CheckOutTime.ToTimeSpan())
                    : fallbackCheckout;

                record.CheckOutTime = checkoutTime;
                record.WorkedMinutes = (int)(checkoutTime - record.CheckInTime!.Value).TotalMinutes;

                if (record.WorkedMinutes < 240 && record.Status == AttendanceStatus.Present)
                    record.Status = AttendanceStatus.Late;
            }

            // Single batched update instead of N round trips
            await _repo.BulkUpdateAsync(stillActive);

            _logger.LogInformation("[AttendanceJob] Auto checked out {Count} users.", stillActive.Count);
        }

        private async Task MarkAbsentUsersAsync(DateOnly today, Dictionary<string, WorkSettings> settingsByAdmin)
        {
            var todayRecords = await _repo.GetTodayAllAsync();
            var presentIds = todayRecords.Select(r => r.UserId).ToHashSet();

            var absentUsers = await _repo.GetAbsentUsersAsync(presentIds.ToList());
            if (absentUsers.Count == 0)
            {
                _logger.LogInformation("[AttendanceJob] No absent users.");
                return;
            }

            var todayName = today.DayOfWeek.ToString();

            var absentRecords = absentUsers
                .Select(user =>
                {
                    var isWeekend = user.CreatedByAdminId != null
                        && settingsByAdmin.TryGetValue(user.CreatedByAdminId, out var settings)
                        && settings.Weekends.Split(',', StringSplitOptions.TrimEntries).Contains(todayName);

                    return new Attendance
                    {
                        UserId = user.Id,
                        Date = today,
                        CheckInTime = null,
                        CheckOutTime = null,
                        Status = isWeekend ? AttendanceStatus.Weekend : AttendanceStatus.Absent,
                        WorkedMinutes = 0
                    };
                })
                .ToList();

            await _repo.BulkUpdateAsync(absentRecords);

            _logger.LogInformation("[AttendanceJob] Marked {Count} users as Absent/Weekend.", absentRecords.Count);
        }

        private async Task ExportToExcelAsync(DateOnly today)
        {
            var records = await _repo.GetTodayAllAsync();
            if (records.Count == 0)
            {
                _logger.LogInformation("[AttendanceJob] No records to export.");
                return;
            }

            var filePath = await _excel.GenerateDailyReport();
            _logger.LogInformation("[AttendanceJob] Excel exported -> {Path}", filePath);
        }
    }
}