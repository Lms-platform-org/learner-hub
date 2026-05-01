using System;

namespace Horizon.MVC.DTOs
{
    public class CreateCommentDto
    {
        public int CourseId { get; set; }
        public string? UserId { get; set; }
        public required string Content { get; set; }
    }

    public class CommentResponseDto
    {
        public int Id { get; set; }
        public int CourseId { get; set; }
        public required string UserId { get; set; }
        public required string UserName { get; set; }
        public required string UserRole { get; set; }
        public required string Content { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class CommentReportDTO
    {
        public string SName { get; set; } = string.Empty; // Student Name
        public string CourseTitle { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}

