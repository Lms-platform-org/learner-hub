using System;
using System.Collections.Generic;

namespace Courses.Models.DTOs
{
    public class CourseReadDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? PublishedAt { get; set; }
        public decimal Price { get; set; }
        // No TeacherId (TId) as requested
    }
}
