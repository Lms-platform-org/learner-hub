using LearningPlatform.StudentService.Data;
using LearningPlatform.StudentService.Models;
using Microsoft.EntityFrameworkCore;

namespace LearningPlatform.StudentService
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<StudentDbContext>();
                
                // Apply migrations
                await context.Database.MigrateAsync();

                // Check if data already exists
                if (await context.Enrollments.AnyAsync())
                    return;

                var studentId = "e5b04ec4-a881-4d41-9d05-fa8ccc892de1";

                // Seed Enrollments
                var enrollments = new List<Enrollment>
                {
                    new Enrollment { StudentId = studentId, CourseId = 1, EnrolledAt = DateTime.UtcNow.AddDays(-60) },
                    new Enrollment { StudentId = studentId, CourseId = 2, EnrolledAt = DateTime.UtcNow.AddDays(-45) },
                    new Enrollment { StudentId = studentId, CourseId = 3, EnrolledAt = DateTime.UtcNow.AddDays(-30) },
                    new Enrollment { StudentId = studentId, CourseId = 4, EnrolledAt = DateTime.UtcNow.AddDays(-15) },
                    new Enrollment { StudentId = studentId, CourseId = 5, EnrolledAt = DateTime.UtcNow.AddDays(-7) }
                };
                await context.Enrollments.AddRangeAsync(enrollments);

                // Seed Progress
                var progressRecords = new List<StudentProgress>
                {
                    new StudentProgress 
                    { 
                        StudentId = studentId, 
                        CourseId = 1, 
                        Percentage = 100, 
                        XPScore = 500,
                        LastLessonId = 10,
                        LastVideoTimestampSeconds = 0,
                        LastAccessed = DateTime.UtcNow.AddDays(-5),
                        EarnedMilestones = new List<string> { "Completed", "Expert" }
                    },
                    new StudentProgress 
                    { 
                        StudentId = studentId, 
                        CourseId = 2, 
                        Percentage = 100, 
                        XPScore = 450,
                        LastLessonId = 8,
                        LastVideoTimestampSeconds = 0,
                        LastAccessed = DateTime.UtcNow.AddDays(-3),
                        EarnedMilestones = new List<string> { "Completed" }
                    },
                    new StudentProgress 
                    { 
                        StudentId = studentId, 
                        CourseId = 3, 
                        Percentage = 75, 
                        XPScore = 350,
                        LastLessonId = 6,
                        LastVideoTimestampSeconds = 1200,
                        LastAccessed = DateTime.UtcNow.AddHours(-2),
                        EarnedMilestones = new List<string> { "Halfway" }
                    },
                    new StudentProgress 
                    { 
                        StudentId = studentId, 
                        CourseId = 4, 
                        Percentage = 45, 
                        XPScore = 200,
                        LastLessonId = 4,
                        LastVideoTimestampSeconds = 800,
                        LastAccessed = DateTime.UtcNow.AddHours(-6),
                        EarnedMilestones = new List<string>()
                    },
                    new StudentProgress 
                    { 
                        StudentId = studentId, 
                        CourseId = 5, 
                        Percentage = 20, 
                        XPScore = 100,
                        LastLessonId = 2,
                        LastVideoTimestampSeconds = 300,
                        LastAccessed = DateTime.UtcNow.AddDays(-1),
                        EarnedMilestones = new List<string>()
                    }
                };
                await context.AddRangeAsync(progressRecords);

                // Seed Bookmarks
                var bookmarks = new List<Bookmark>
                {
                    new Bookmark 
                    { 
                        StudentId = studentId, 
                        CourseId = 6, 
                        Type = "course",
                        Category = "python",
                        PersonalNote = "Interesting course on advanced Python"
                    },
                    new Bookmark 
                    { 
                        StudentId = studentId, 
                        CourseId = 7, 
                        Type = "course",
                        Category = "web",
                        PersonalNote = "React patterns to learn"
                    },
                    new Bookmark 
                    { 
                        StudentId = studentId, 
                        CourseId = 8, 
                        Type = "course",
                        Category = "ml",
                        PersonalNote = null
                    },
                    new Bookmark 
                    { 
                        StudentId = studentId, 
                        BookKey = "OL123456M", 
                        BookTitle = "Clean Code",
                        BookAuthor = "Robert C. Martin",
                        Type = "book",
                        Category = "general",
                        PersonalNote = "Must read for developers"
                    },
                    new Bookmark 
                    { 
                        StudentId = studentId, 
                        BookKey = "OL234567M", 
                        BookTitle = "Design Patterns",
                        BookAuthor = "Gang of Four",
                        Type = "book",
                        Category = "general",
                        PersonalNote = null
                    }
                };
                await context.Bookmarks.AddRangeAsync(bookmarks);

                // Seed Student Profile
                var profile = new StudentProfile
                {
                    StudentId = studentId,
                    FullName = "John Developer",
                    Bio = "Passionate about learning new technologies and building amazing applications.",
                    Skills = new List<string> { "C#", "JavaScript", "Python", "SQL", "React" },
                    PreferredLevel = "intermediate",
                    JoinedDate = DateTime.UtcNow.AddDays(-90)
                };
                await context.StudentProfiles.AddAsync(profile);

                await context.SaveChangesAsync();
            }
        }
    }
}
