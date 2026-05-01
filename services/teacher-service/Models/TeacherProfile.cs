using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TeacherDashboardApi.Models
{
    public class TeacherProfile
    {
        [Key]
        public string Id { get; set; } = string.Empty; // Matches TeacherId

        [Required]
        public string FullName { get; set; } = string.Empty;

        public string Bio { get; set; } = string.Empty;

        public List<string> Skills { get; set; } = new();

        public string PreferredLevel { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public DateTime JoinedDate { get; set; } = DateTime.UtcNow;
    }
}
