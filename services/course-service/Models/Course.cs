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
      
        public string InstructorId { get; set; } = string.Empty; 
 
        public CourseStatus Status { get; set; } = CourseStatus.Draft;
 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
 
        public DateTime? PublishedAt { get; set; }
 
        public decimal Price { get; set; }

        public string Category { get; set; } = string.Empty; 

        public string Duration { get; set; } = string.Empty;

        public string? ThumbnailUrl { get; set; }

        public bool IsPublished { get; set; }

        public DateTime StartDate { get; set; } = DateTime.UtcNow;

        public ICollection<CourseVideo> Videos { get; set; } = new List<CourseVideo>();
        [JsonIgnore]
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
