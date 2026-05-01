namespace Horizon.MVC.DTOs
{
    public class CourseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Category { get; set; }
        public string? Level { get; set; }
        public int TotalLessons { get; set; }
        public string? ThumbnailUrl { get; set; }
        public string? InstructorName { get; set; }
        public string? VideoUrl { get; set; }
    }
}


