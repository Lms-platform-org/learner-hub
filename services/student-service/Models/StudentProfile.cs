using System.ComponentModel.DataAnnotations;

namespace LearningPlatform.StudentService.Models
{
    public class StudentProfile
    {
        
        //    public int Id { get; set; }
        [Key]
        public string StudentId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Bio { get; set; }
        public List<string> Skills { get; set; } = new();
        public string PreferredLevel { get; set; } = "Beginner";

        //public List<Courses> CourseDto { get; set; } 

        //public int TotalXPScore { get; set; }

        public DateTime JoinedDate { get; set; } = DateTime.UtcNow;
    }
}
