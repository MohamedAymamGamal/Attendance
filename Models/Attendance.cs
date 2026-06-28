using Job.Models.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Job.Models
{
    [Table("Attendance")]
    public class Attendance
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public AppUsers User { get; set; }

        public DateOnly Date { get; set; }
        public DateTime? CheckInTime { get; set; }
        public DateTime? CheckOutTime { get; set; }
        public AttendanceStatus Status { get; set; }
        public int WorkedMinutes { get; set; }
    }
}
