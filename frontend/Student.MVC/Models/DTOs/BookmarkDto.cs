namespace LearningPlatform.StudentService.WebApp.Models.DTOs
{
    public class BookmarkDto
    {
        public int Id { get; set; }
        public int? CourseId { get; set; }
        public string? BookKey { get; set; }
        public string? BookTitle { get; set; }
        public string? BookAuthor { get; set; }
        public string Type { get; set; } = "course";
        public string Category { get; set; } = "General";
        public string? PersonalNote { get; set; }
    }
}
