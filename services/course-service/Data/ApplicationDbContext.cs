using Microsoft.EntityFrameworkCore;
using Courses.Models;

namespace Courses.Api.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CourseVideo> CourseVideos { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Course configuration
            modelBuilder.Entity<Course>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Title).IsRequired().HasMaxLength(100);
                entity.Property(c => c.Description).IsRequired();
                entity.Property(c => c.Price).HasPrecision(18, 2);
                
                entity.HasMany(c => c.Videos)
                      .WithOne(v => v.Course)
                      .HasForeignKey(v => v.CourseId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasMany(c => c.Enrollments)
                      .WithOne(e => e.Course)
                      .HasForeignKey(e => e.CourseId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // Enrollment configuration
            modelBuilder.Entity<Enrollment>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.StudentId).IsRequired();
            });

            // Comment configuration
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Content).IsRequired();
                entity.Property(c => c.UserId).IsRequired();
                
                entity.HasOne(c => c.Course)
                      .WithMany()
                      .HasForeignKey(c => c.CourseId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            // CourseVideo configuration
            modelBuilder.Entity<CourseVideo>(entity =>
            {
                entity.HasKey(v => v.Id);
                entity.Property(v => v.VideoUrl).IsRequired();
            });
        }
    }
}
