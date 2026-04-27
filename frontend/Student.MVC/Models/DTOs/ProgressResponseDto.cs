namespace LearningPlatform.StudentService.WebApp.Models.DTOs
{
    public class ProgressResponseDto
    {
        public int Percentage { get; set; }
        public int XP { get; set; }
        public List<string> EarnedMilestones { get; set; } = new();
        public int LastLessonId { get; set; }
        public int LastVideoTimestampSeconds { get; set; }
        public DateTime LastAccessed { get; set; }
    }
}
