using System.ComponentModel.DataAnnotations;

namespace LearningPlatformFrontend.Models
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

        public string? ReturnUrl { get; set; }
    }

    public class RegisterViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = "";

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string Password { get; set; } = "";

        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = "";

        [Required]
        public string Role { get; set; } = "Student";

        [Display(Name = "Display Name")]
        public string? DisplayName { get; set; }
    }

    public class AuthResponse
    {
        public bool Success { get; set; } // We'll set this manually based on HTTP status
        public string Token { get; set; } = "";
        public string Email { get; set; } = "";
        public string UserId { get; set; } = "";
        public string Name { get; set; } = "";
        public string[] Roles { get; set; } = System.Array.Empty<string>();
        public string? Message { get; set; }
    }
}
