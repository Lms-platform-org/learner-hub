namespace LearningPlatform.StudentService.WebApp.Models.DTOs
{
    public class BookmarkAjaxRequest
    {
        public int? CourseId { get; set; }
        public string? BookKey { get; set; }
        public string? BookTitle { get; set; }
        public string? BookAuthor { get; set; }
        public string? Category { get; set; }
        public string? Note { get; set; }
        public string Type { get; set; } = "course";
    }
}
