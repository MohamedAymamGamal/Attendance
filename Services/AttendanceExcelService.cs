using ClosedXML.Excel;
using Job.Data;
using Job.Models;
using Microsoft.EntityFrameworkCore;
namespace Job.Services
{
    public class AttendanceExcelService
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AttendanceExcelService(ApplicationDbContext context, IWebHostEnvironment env )
        {
            _context = context;
            _env = env;
        }

        public async Task<string> GenerateDailyReport()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            var data = await _context.Attendances
                .Include(a => a.User)
                .Where(a => a.Date == today)
                .OrderBy(x => x.User)
                .ToListAsync();

            using var workbook = new XLWorkbook();
            var sheet = workbook.Worksheets.Add("Attendance");

            sheet.Cell(1, 1).Value = "Employee";
            sheet.Cell(1, 2).Value = "Department";
            sheet.Cell(1, 3).Value = "Check In";
            sheet.Cell(1, 4).Value = "Check Out";
            sheet.Cell(1, 5).Value = "Worked";
            sheet.Cell(1, 6).Value = "Status";

            int row = 2;

            foreach (var item in data)
            {
                sheet.Cell(row, 1).Value = item.User.UserName;
                sheet.Cell(row, 2).Value = item.User.Department;
                sheet.Cell(row, 3).Value = item.CheckInTime?.ToString("hh:mm tt");
                sheet.Cell(row, 4).Value = item.CheckOutTime?.ToString("hh:mm tt");
                sheet.Cell(row, 5).Value = $"{item.WorkedMinutes / 60}h {item.WorkedMinutes % 60}m";
                sheet.Cell(row, 6).Value = item.Status.ToString();

                row++;
            }


            sheet.Columns().AdjustToContents();
            
            var folder = Path.Combine(_env.WebRootPath,"Repots");


            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            var file = Path.Combine(folder,
                $"Attendance-{DateTime.Today:yyyy-MM-dd}.xlsx");

            workbook.SaveAs(file);

            return file;
        }



    }
}
