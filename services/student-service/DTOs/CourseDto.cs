namespace LearningPlatform.StudentService.DTOs
{
    public class CourseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? Level { get; set; }
        public int TotalLessons { get; set; }
        public string? ThumbnailUrl { get; set; }  // course cover image
        public string? InstructorName { get; set; } // teacher name
        public string? VideoUrl { get; set; }        // direct video URL from Sudhish
    }
}
