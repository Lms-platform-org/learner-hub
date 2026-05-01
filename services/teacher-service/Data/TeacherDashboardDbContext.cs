using Microsoft.EntityFrameworkCore;
using TeacherDashboardApi.Models;

namespace TeacherDashboardApi.Data
{
    public class TeacherDashboardDbContext : DbContext
    {
        public TeacherDashboardDbContext(DbContextOptions<TeacherDashboardDbContext> options)
            : base(options)
        {
        }

        public DbSet<TeacherProfile> TeacherProfiles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<TeacherProfile>(entity =>
            {
                entity.HasKey(p => p.Id);
                entity.Property(p => p.FullName).IsRequired();
                entity.Property(p => p.Email).IsRequired();
                
                // Map List<string> to JSON for Skills (EF Core 8+)
                entity.Property(p => p.Skills);
            });
        }
    }
}
