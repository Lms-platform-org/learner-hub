namespace LearningPlatformAuth.Models
{
    public class UserDto
    {
        public string Id { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? DisplayName { get; set; }
        public bool IsApproved { get; set; }
        public DateTime? ApprovalDate { get; set; }
        public string? ApprovalNotes { get; set; }
        public string? Role { get; set; }
        public DateTime? CreatedAt { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
    }
}
