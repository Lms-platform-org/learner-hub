using System.ComponentModel.DataAnnotations;

namespace LearningPlatform.StudentService.WebApp.Models
{
    public class BookmarkViewModel
    {
        public int Id { get; set; }
        [Required]
        public int CourseId { get; set; }
        [Required]
        public string Category { get; set; }
        [MaxLength(200)]
        public string PersonalNote { get; set; }
    }
}
