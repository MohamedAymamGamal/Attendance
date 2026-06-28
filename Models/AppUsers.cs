using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Job.Models
{
    [Table("AppUser")]  
    public class AppUsers : IdentityUser
    {

        public string? Department { get; set; }
        public bool IsActive { get; set; } = true;

        public string? CreatedByAdminId { get; set; }

        [ForeignKey(nameof(CreatedByAdminId))]
        public AppUsers? CreatedByAdmin { get; set; }

        public ICollection<AppUsers>? ManagedUsers { get; set; }

        public ICollection<Attendance>? Attendances { get; set; }
    }
}
