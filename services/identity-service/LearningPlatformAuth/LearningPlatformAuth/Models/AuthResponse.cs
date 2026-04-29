namespace LearningPlatformAuth.Models
{
    public class AuthResponse
    {
        public string Token { get; set; } = "";
        public string Email { get; set; } = "";
        public string UserId { get; set; } = "";
        public string Name { get; set; } = "";
        public string[] Roles { get; set; } = System.Array.Empty<string>();
        public string? Message { get; set; } = "Login Successful...!!!!";
    }
}
 