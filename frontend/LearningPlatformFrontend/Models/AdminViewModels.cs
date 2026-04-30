using System.ComponentModel.DataAnnotations;

namespace LearningPlatformFrontend.Models
{
    public class UserDto
    {
        [System.Text.Json.Serialization.JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("userId")]
        public string UserId { get => Id; set => Id = value; }

        public string? Email { get; set; }
        public string? DisplayName { get; set; }
        public bool IsApproved { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string? ApprovalNotes { get; set; }
        public string? Role { get; set; }
        public DateTime? CreatedAt { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
    }

    public class ApprovalRequestViewModel
    {
        public string? Notes { get; set; }
    }

    public class AdminDashboardViewModel
    {
        public List<UserDto> PendingTeachers { get; set; } = new();
        public List<UserDto> ApprovedUsers { get; set; } = new();
    }
}
