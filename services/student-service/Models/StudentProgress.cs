using Microsoft.EntityFrameworkCore;

using System.ComponentModel.DataAnnotations;
namespace LearningPlatform.StudentService.Models
{
    public class StudentProgress
    {
        public string StudentId { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public int Percentage { get; set; }
        public int XPScore { get; set; }
        public List<int> CompletedLessonIds { get; set; } = new();
        public List<string> EarnedMilestones { get; set; } = new();
        public int LastLessonId { get; set; }
        public int LastVideoTimestampSeconds { get; set; }
        public DateTime LastAccessed { get; set; } = DateTime.UtcNow;
    }
}
