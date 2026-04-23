using LearningPlatform.StudentService.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Text.Json;

namespace LearningPlatform.StudentService.Data
{
    public class StudentDbContext : DbContext
    {
        public StudentDbContext(DbContextOptions<StudentDbContext> options)
            : base(options) { }

        public DbSet<Bookmark> Bookmarks { get; set; }
        public DbSet<StudentProgress> ProgressRecords { get; set; }
        public DbSet<StudentProfile> StudentProfiles { get; set; }
        public DbSet<Enrollment> Enrollments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var jsonOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            modelBuilder.Entity<StudentProgress>()
                .HasKey(p => new { p.StudentId, p.CourseId });

            modelBuilder.Entity<Enrollment>()
                .HasIndex(e => e.StudentId);

            modelBuilder.Entity<Enrollment>()
                .HasIndex(e => new { e.StudentId, e.CourseId })
                .IsUnique(); // one enrollment per student per course

            modelBuilder.Entity<Bookmark>()
                .HasIndex(b => b.StudentId);

            modelBuilder.Entity<Bookmark>()
                .HasIndex(b => b.Category);

            modelBuilder.Entity<StudentProgress>()
                .HasIndex(p => new { p.StudentId, p.CourseId });

            modelBuilder.Entity<Bookmark>()
                .Property(b => b.Category)
                .HasMaxLength(20)
                .IsRequired();

            modelBuilder.Entity<Bookmark>()
                .Property(b => b.Type)
                .HasMaxLength(10)
                .IsRequired();

            modelBuilder.Entity<Bookmark>()
                .Property(b => b.BookKey)
                .HasMaxLength(100);

            modelBuilder.Entity<Bookmark>()
                .Property(b => b.BookTitle)
                .HasMaxLength(300);

            modelBuilder.Entity<Bookmark>()
                .Property(b => b.BookAuthor)
                .HasMaxLength(200);

            modelBuilder.Entity<Bookmark>()
                .Property(b => b.PersonalNote)
                .HasMaxLength(200);

            modelBuilder.Entity<StudentProfile>()
                .Property(p => p.FullName)
                .HasMaxLength(100)
                .IsRequired();

            modelBuilder.Entity<StudentProfile>()
                .Property(p => p.Bio)
                .HasMaxLength(300)
                .IsRequired(false);

            var stringListComparer = new ValueComparer<List<string>>(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()
            );

            var intListComparer = new ValueComparer<List<int>>(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()
            );


            modelBuilder.Entity<StudentProgress>()
                .Property(p => p.EarnedMilestones)
                .HasConversion(
                    v => JsonSerializer.Serialize(v ?? new List<string>(), jsonOptions),
                    v => JsonSerializer.Deserialize<List<string>>(v, jsonOptions) ?? new List<string>()
                )
                .Metadata.SetValueComparer(stringListComparer);

            modelBuilder.Entity<StudentProfile>()
                .Property(p => p.Skills)
                .HasConversion(
                    v => JsonSerializer.Serialize(v ?? new List<string>(), jsonOptions),
                    v => JsonSerializer.Deserialize<List<string>>(v, jsonOptions) ?? new List<string>()
                )
                .Metadata.SetValueComparer(stringListComparer);

            modelBuilder.Entity<StudentProgress>()
                .Property(p => p.CompletedLessonIds)
                .HasConversion(
                    v => JsonSerializer.Serialize(v ?? new List<int>(), jsonOptions),
                    v => JsonSerializer.Deserialize<List<int>>(v, jsonOptions) ?? new List<int>()
                )
                .Metadata.SetValueComparer(intListComparer);

            base.OnModelCreating(modelBuilder);
        }
    }
}