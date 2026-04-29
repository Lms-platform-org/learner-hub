namespace LearningPlatformAuth.Models
{
    public class RegisterRequest
    {
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
        public string Role { get; set; } = ""; // "Student" or "Teacher"
        public string? DisplayName { get; set; }
    }
}
