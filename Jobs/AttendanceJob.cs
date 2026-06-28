using DocumentFormat.OpenXml.Office2013.Drawing.ChartStyle;
using DocumentFormat.OpenXml.Spreadsheet;
using Job.Models;
using Job.Reposities;
using Job.Services;
using Microsoft.AspNetCore.Identity;

namespace Job.Jobs
{
    public class AttendanceJob
    {
        private readonly AttendanceRepo _repo;
        private readonly UserManager<AppUsers> _userManager;
        private readonly ILogger<AttendanceJob> _logger;
        private readonly AttendanceExcelService _execl;

        public AttendanceJob(AttendanceRepo repo, UserManager<AppUsers> userManager, AttendanceExcelService execl)
        {
            _repo = repo;
            _userManager = userManager;
            _execl = execl;
            
        }

        public async Task Execute()
        {
            _logger.LogInformation("[AttendanceJob] Starting at {Time}", DateTime.Now);

            try
            {
                await AutoCheckOutActiveUsersAsync();
                await MarkAbsentUsersAsync();
                await ExportToExcelAsync();

                _logger.LogInformation("[AttendanceJob] Completed at {Time}", DateTime.Now);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[AttendanceJob] Failed at {Time}", DateTime.Now);
                throw;
            }
        }

        private async Task MarkAbsentUsersAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            var todayRecords = await _repo.GetTodayAllAsync();

            var presentIds = todayRecords.Select(r => r.UserId).ToList();


            // Get all active users with no record
            var absentUsers = await _repo.GetAbsentUsersAsync(presentIds);

            if (!absentUsers.Any())
            {
                _logger.LogInformation("[AttendanceJob] No absent users.");
                return;
            }

            var absentRecords = new List<Attendance>();

            foreach(var user in absentRecords)
            {
                
            }
        }
    }
}
