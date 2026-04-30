namespace LearningPlatform.StudentService.WebApp.Models.DTOs
{
    public class ProfileDto
    {
        public string FullName { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public List<string> Skills { get; set; } = new();
        public string PreferredLevel { get; set; } = "Beginner";
        public string? Email { get; set; }
        public DateTime JoinedDate { get; set; } = DateTime.UtcNow;
    }
}
