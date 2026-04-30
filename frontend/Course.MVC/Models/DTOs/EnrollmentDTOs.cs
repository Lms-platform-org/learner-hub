using System;

namespace Courses.Models.DTOs
{
    public class EnrollmentCreateDTO
    {
        public int CourseId { get; set; }
        public string? StudentId { get; set; }
    }

    public class EnrollmentReadDTO
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public string StudentId { get; set; } = string.Empty;
        public DateTime EnrolledAt { get; set; }
        public string? CourseTitle { get; set; }
    }
}
