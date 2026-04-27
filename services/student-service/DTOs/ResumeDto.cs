using System.ComponentModel.DataAnnotations;

namespace LearningPlatform.StudentService.DTOs
{
    public class ResumeDto
    {
        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "CourseId must be positive")]
        public int CourseId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "LessonId must be positive")]
        public int LessonId { get; set; }

        [Range(0, int.MaxValue, ErrorMessage = "Seconds cannot be negative")]
        public int Seconds { get; set; }
    }
}