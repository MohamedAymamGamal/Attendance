using Job.Data;
using Job.DTO;
using Job.Helper;
using Job.Models;
using Microsoft.EntityFrameworkCore;

namespace Job.Reposities
{
    public class AttendanceRepo
    {
        private readonly ApplicationDbContext _context;
        public AttendanceRepo(ApplicationDbContext context)
        {
            _context = context;
        }

        //////getTodayAttendance
        public async Task<Attendance?> GetTodayAttendanceAync(string userId)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);


            return await _context.Attendances.Include(a => a.User)
                .FirstOrDefaultAsync(a => a.UserId == userId && a.Date == today);


        }
        //get AbsentUser
        public async Task<List<AppUsers>> GetAbsentUsersAsync(List<string> presentUserIds)
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            return await _context.Users
                .Where(u => u.IsActive
                         && !presentUserIds.Contains(u.Id))
                .ToListAsync();
        }

        //get Setting
        public async Task<WorkSettings?> GetSettingsAsync(string adminId)
          => await _context.workSettings
              .FirstOrDefaultAsync(s => s.AdminId == adminId);


        //Getall Today
        public async Task<List<Attendance>> GetTodayAllAsync()
        {
            //.OrderBy(a => a.User!.UserName)
            var today = DateOnly.FromDateTime(DateTime.Today);
            return await _context.Attendances
                .Include(u => u.User)
                .Where(a => a.Date == today)
                .OrderBy(a => a.UserId).ToListAsync();
        }

        //getStill Active 
        public async Task<List<Attendance>> GetStillActiveAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);
            return await _context.Attendances
                .Include(a => a.User)
                .Where(a => a.Date == today
                         && a.CheckInTime != null
                         && a.CheckOutTime == null)
                .ToListAsync();
        }

        //get by id
        public async Task<Attendance?> GetById(int id)
            => await _context.Attendances.Include(a => a.User)
            .FirstOrDefaultAsync(a => a.Id == id);

        //get filtered 
        public async Task<List<Attendance>> GetFilteredAsync(Params param)
        {
            var query = _context.Attendances
                .Include(a => a.User)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(param.userId))
                query = query.Where(a => a.UserId == param.userId);

            if (param.from.HasValue)
                query = query.Where(a => a.Date >= param.from.Value);

            if (param.to.HasValue)
                query = query.Where(a => a.Date <= param.to.Value);

            if (!string.IsNullOrWhiteSpace(param.department))
                query = query.Where(a => a.User!.Department == param.department);

            return await query
                .OrderByDescending(a => a.Date)
                .ThenBy(a => a.User!.UserName)
                .ToListAsync();
        }

        //create
        public async Task<Attendance> CreateAync(Attendance attendance)
        {
            await _context.Attendances.AddAsync(attendance);
            await _context.SaveChangesAsync();
            return attendance;


        }
        //updateAsync
        public async Task<Attendance> UpdateAsync(Attendance attendance)
        {
            _context.Attendances.Update(attendance);
            await _context.SaveChangesAsync();
            return attendance;
        }
        //DeletedAsync
        public async Task<bool>  DeletedAsync(Attendance attendance)
        {
            _context.Attendances.Remove(attendance);
            await _context.SaveChangesAsync();
            return true;

        }
}}
