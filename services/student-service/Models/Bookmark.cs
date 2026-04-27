namespace LearningPlatform.StudentService.Models
{
    public class Bookmark
    {
        public int Id { get; set; }
        public string StudentId { get; set; } = string.Empty;

        // Course bookmark
        public int? CourseId { get; set; }

        // Book bookmark
        public string? BookKey { get; set; }   // e.g. /works/OL45804W
        public string? BookTitle { get; set; } // stored so we don't need to re-fetch
        public string? BookAuthor { get; set; }

        public string Type { get; set; } = "course"; // "course" or "book"
        public string Category { get; set; } = "General";
        public string? PersonalNote { get; set; }
    }
}
