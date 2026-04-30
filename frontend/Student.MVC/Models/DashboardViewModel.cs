using LearningPlatform.StudentService.WebApp.Models.DTOs;

namespace LearningPlatform.StudentService.WebApp.Models
{
    public class DashboardViewModel
    {
        public int EnrolledCount { get; set; }
        public int CompletedCount { get; set; }
        public int InProgressCount { get; set; }
        public int TotalBookmarks { get; set; }
        public int TotalXP { get; set; }
        public List<ProgressViewModel> ContinueWatching { get; set; } = new();
        public List<int> RecommendedCourseIds { get; set; } = new();
        public List<string> Milestones { get; set; } = new();
    }

    public class ProgressViewModel
    {
        public int CourseId { get; set; }
        public int Percentage { get; set; }
        public int LastLessonId { get; set; }
        public int LastVideoTimestampSeconds { get; set; }
        public int XP { get; set; }
        public List<string> EarnedMilestones { get; set; } = new();
    }
}
