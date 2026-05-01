using System;
using System.Collections.Generic;

namespace Horizon.MVC.DTOs
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
        public string Category { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public string InstructorId { get; set; } = string.Empty;
        public bool IsPublished { get; set; }
        public DateTime StartDate { get; set; }
        public List<CourseVideoDTO> Videos { get; set; } = new();
    }

    public class CourseVideoDTO
    {
        public int Id { get; set; }
        public string VideoUrl { get; set; } = string.Empty;
    }

    public class CourseWriteDTO
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Duration { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public DateTime StartDate { get; set; }
        public bool IsPublished { get; set; }
    }
}

