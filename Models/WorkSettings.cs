using System.ComponentModel.DataAnnotations.Schema;

namespace Job.Models
{
    [Table("WorkSettings")]
    public class WorkSettings
    {
        public int Id { get; set; }

        public TimeOnly CheckInStart { get; set; }    // e.g. 08:00 — earliest allowed
        public TimeOnly CheckInDeadline { get; set; } // e.g. 09:00 — after this = Late
        public TimeOnly CheckOutTime { get; set; }    // e.g. 17:00 — auto-checkout time

        public string Weekends { get; set; } = "Friday,Saturday";

        public string AdminId { get; set; }

        [ForeignKey(nameof(AdminId))]
        public AppUsers Admin { get; set; }

    }
}
