namespace Horizon.MVC.DTOs
{
    public class DashboardData
    {
        public List<int>? EnrolledCourseIds { get; set; }
        public List<ProgressData>? Progress { get; set; }
        public List<ProgressData>? ContinueWatching { get; set; }
        public int TotalBookmarks { get; set; }
        public int TotalXP { get; set; }
        public List<int>? RecommendedCourses { get; set; }
    }

    public class ProgressData
    {
        public int Percentage { get; set; }
        public int LastLessonId { get; set; }
        public int LastVideoTimestampSeconds { get; set; }
        public int XP { get; set; }
        public List<string>? EarnedMilestones { get; set; }
    }

    public class BookmarkDTO
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public int LessonId { get; set; }
        public string? Note { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class ProfileDTO
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public int TotalXP { get; set; }
        public List<string>? Achievements { get; set; }
    }

    public class DiscoveryDTO
    {
        public List<CourseReadDTO>? Courses { get; set; }
        public List<string>? Categories { get; set; }
    }
}


