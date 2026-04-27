namespace LearningPlatform.StudentService.Models
{
    public class Enrollment
    {
        public int Id { get; set; }
        public string StudentId { get; set; } = string.Empty;
        public int CourseId { get; set; }
        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
    }
}
