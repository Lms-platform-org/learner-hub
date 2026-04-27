namespace LearningPlatform.StudentService.DTOs
{
    public class ReviewDto
    {
        public int CourseId { get; set; }
        public string StudentId { get; set; }
        public string StudentName { get; set; }
        public string Comment { get; set; }
        public int Rating { get; set; } // 1-5
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
