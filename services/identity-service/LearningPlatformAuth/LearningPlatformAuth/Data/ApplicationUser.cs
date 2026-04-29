using Microsoft.AspNetCore.Identity;

namespace LearningPlatformAuth.Data
{
    public class ApplicationUser : IdentityUser
    {
        public string? DisplayName { get; set; }

        
        public bool IsApproved { get; set; } = false;

       
        public DateTime? ApprovalDate { get; set; }

        
        public string? ApprovalNotes { get; set; }
    }
}
