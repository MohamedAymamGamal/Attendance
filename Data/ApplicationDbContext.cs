
using Job.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Job.Data
{
    public class ApplicationDbContext : IdentityDbContext<AppUsers>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
      : base(options)
        {
        }

        public DbSet<Attendance> Attendances { get; set; }

        public DbSet<WorkSettings> workSettings { get; set; }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<AppUsers>()
            .HasOne(u => u.CreatedByAdmin)
            .WithMany(a => a.ManagedUsers)
            .HasForeignKey(u => u.CreatedByAdminId)
            .OnDelete(DeleteBehavior.SetNull); // if admin deleted, don't delete users

            builder.Entity<WorkSettings>()
                .HasOne(w => w.Admin)
                .WithOne()
                .HasForeignKey<WorkSettings>(w => w.AdminId)
                .OnDelete(DeleteBehavior.Cascade);

            // One user → many attendance records
            builder.Entity<Attendance>()
                .HasOne(a => a.User)
                .WithMany(u => u.Attendances)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<Attendance>()
                .HasIndex(a => new { a.UserId, a.Date })
                .IsUnique();

            //builder.Entity<WorkSettings>()
            //    .HasOne(a => a.User)
            //    .WithMany(i => i.Wp);

            builder.Entity<IdentityRole>().HasData(
                  new IdentityRole
                  {
                      Id = "1",
                      Name = "Admin",
                      NormalizedName = "ADMIN",
                      ConcurrencyStamp = "admin-role"
                  },
                  new IdentityRole
                  {
                      Id = "2",
                      Name = "User",
                      NormalizedName = "USER",
                      ConcurrencyStamp = "user-role"
                  }
              );
        }



    }
}
