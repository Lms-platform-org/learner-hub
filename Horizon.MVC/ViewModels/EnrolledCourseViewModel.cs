namespace Horizon.MVC.ViewModels
{
    public class EnrolledCourseViewModel
    {
        public int CourseId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? Level { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? InstructorName { get; set; }
        public string? VideoUrl { get; set; }
        public int TotalLessons { get; set; }

        // from progress
        public int Percentage { get; set; }
        public int XP { get; set; }
        public int LastLessonId { get; set; }
        public int LastVideoTimestampSeconds { get; set; }
        public DateTime? LastAccessed { get; set; }
        public List<string> EarnedMilestones { get; set; } = new();

        // computed
        public bool IsCompleted => Percentage == 100;
        public bool IsStarted => Percentage > 0;
        public string ResumeTime => TimeSpan.FromSeconds(LastVideoTimestampSeconds).ToString(@"mm\:ss");
    }
}


