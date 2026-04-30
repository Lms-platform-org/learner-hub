using System.ComponentModel.DataAnnotations;

namespace LearningPlatform.StudentService.DTOs
{
    public class BookmarkDto
    {
        // Course bookmark fields
        public int? CourseId { get; set; }

        // Book bookmark fields
        public string? BookKey { get; set; }
        public string? BookTitle { get; set; }
        public string? BookAuthor { get; set; }

        [Required]
        public string Type { get; set; } = "course"; // "course" or "book"

        [Required]
        [MaxLength(20)]
        public string Category { get; set; } = "General";

        [MaxLength(200)]
        public string? PersonalNote { get; set; }
    }
}