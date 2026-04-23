using System;
using System.Text.Json.Serialization;

namespace Courses.Models
{
    public enum CourseStatus
    {
        Draft,
        Published
    }
    public class Course
    {
        public int Id { get; set; }

        public string Title { get; set; } = string.Empty;
 
        public string Description { get; set; } = string.Empty;
      
        public string TeacherId { get; set; } = "Teacher1"; 
 
        public CourseStatus Status { get; set; } = CourseStatus.Draft;
 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
 
        public DateTime? PublishedAt { get; set; }
 
        public decimal Price { get; set; }

        public ICollection<CourseVideo> Videos { get; set; } = new List<CourseVideo>();
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
