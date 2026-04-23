using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Courses.Models
{
    public class Enrollment
    {
        public int Id { get; set; }

        public int CourseId { get; set; }
        public Course? Course { get; set; }

        public string StudentId { get; set; } = string.Empty;
        
        public User? Student { get; set; }

        public DateTime EnrolledAt { get; set; } = DateTime.UtcNow;
    }
}
