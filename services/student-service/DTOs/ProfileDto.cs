using System.ComponentModel.DataAnnotations;

namespace LearningPlatform.StudentService.DTOs
{
    public class ProfileDto
    {
        [Required]
        [MaxLength(100)]
        public string FullName { get; set; } = string.Empty;
        [MaxLength(300)]
        public string? Bio { get; set; }
        public List<string> Skills { get; set; } = new();
        public string PreferredLevel { get; set; } = "Beginner";
        public string? Email { get; set; }
        public DateTime JoinedDate { get; set; } = DateTime.UtcNow;
    }
}
