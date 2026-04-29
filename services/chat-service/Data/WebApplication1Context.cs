using Microsoft.EntityFrameworkCore;
using WebApplication1.Models;

namespace WebApplication1.Data
{
    public class WebApplication1Context : DbContext
    {
        public WebApplication1Context(DbContextOptions<WebApplication1Context> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<ChatSession> ChatSessions { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Foreign Keys to prevent cascading delete conflicts
            modelBuilder.Entity<ChatSession>()
                .HasOne(c => c.Student)
                .WithMany()
                .HasForeignKey(c => c.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ChatSession>()
                .HasOne(c => c.Teacher)
                .WithMany()
                .HasForeignKey(c => c.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);

            // NEW: Message column constraints
            modelBuilder.Entity<Message>(entity =>
            {
                entity.Property(m => m.Content)
                    .IsRequired()
                    .HasMaxLength(2000);

                entity.Property(m => m.SenderRole)
                    .IsRequired()
                    .HasMaxLength(50);
            });
            modelBuilder.Entity<User>().HasData(
                new User { Id = "1", Name = "Teacher User", Role = "Teacher" },
                new User { Id = "2", Name = "Student User", Role = "Student" }
            );
        }
    }
}