namespace LearningPlatform.TeacherService.DTOs
{
    // ──────────────────────────────────────────────────────────────
    //  Mirrored from Course API  (Courses.Models.DTOs.CourseReadDTO)
    //  Field names & types must stay identical – the Teacher API
    //  receives these over HTTP and forwards them unchanged.
    // ──────────────────────────────────────────────────────────────

    public class CourseReadDto
    {
        public int    Id           { get; set; }
        public string Title        { get; set; } = string.Empty;
        public string Description  { get; set; } = string.Empty;
        public string Status       { get; set; } = string.Empty;   // "Draft" | "Published"
        public DateTime  CreatedAt    { get; set; }
        public DateTime? PublishedAt  { get; set; }
        public decimal   Price        { get; set; }
        public string Category     { get; set; } = string.Empty;
        public string Duration     { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public string InstructorId { get; set; } = string.Empty;
        public bool   IsPublished  { get; set; }
        public DateTime StartDate  { get; set; }
        public List<CourseVideoDto> Videos { get; set; } = new();
    }

    public class CourseVideoDto
    {
        public int    Id       { get; set; }
        public string VideoUrl { get; set; } = string.Empty;
    }

    // ──────────────────────────────────────────────────────────────
    //  Write DTO for course creation / update  (forwarded to Course API)
    //  Mirrors Courses.Models.DTOs.CourseWriteDTO exactly.
    // ──────────────────────────────────────────────────────────────

    public class CourseWriteDto
    {
        public string  Title        { get; set; } = string.Empty;
        public string  Description  { get; set; } = string.Empty;
        public decimal Price        { get; set; }
        public string  Category     { get; set; } = string.Empty;
        public string  Duration     { get; set; } = string.Empty;
        public string? ThumbnailUrl { get; set; }
        public DateTime StartDate   { get; set; }
        public bool    IsPublished  { get; set; }
    }

    // ──────────────────────────────────────────────────────────────
    //  Enrollment DTO  (mirrored from Courses.Models.DTOs.EnrollmentReadDTO)
    // ──────────────────────────────────────────────────────────────

    public class EnrollmentReadDto
    {
        public int      Id          { get; set; }
        public int      CourseId    { get; set; }
        public string   StudentId   { get; set; } = string.Empty;
        public DateTime EnrolledAt  { get; set; }
        public string?  CourseTitle { get; set; }
    }

    // ──────────────────────────────────────────────────────────────
    //  Teacher Dashboard aggregate DTO
    //  Built by TeacherDashboardService from Course API data.
    // ──────────────────────────────────────────────────────────────

    public class TeacherDashboardDto
    {
        public string   TeacherId          { get; set; } = string.Empty;
        public int      TotalCourses       { get; set; }
        public int      PublishedCourses   { get; set; }
        public int      DraftCourses       { get; set; }
        public int      TotalStudents      { get; set; }       // sum of enrollments
        public decimal  TotalEarnings      { get; set; }       // sum of Price × StudentCount
        public CourseReadDto?   BestSellingCourse { get; set; } // highest enrollment count
        public int      BestSellingCourseStudentCount { get; set; }
        public List<CourseMetricsDto> CourseMetrics { get; set; } = new();
    }

    /// <summary>Per-course statistics row shown in the teacher dashboard table.</summary>
    public class CourseMetricsDto
    {
        public int      CourseId       { get; set; }
        public string   Title          { get; set; } = string.Empty;
        public string   Status         { get; set; } = string.Empty;
        public int      StudentCount   { get; set; }
        public decimal  Price          { get; set; }
        public decimal  Earnings       { get; set; }   // Price × StudentCount
        public DateTime CreatedAt      { get; set; }
        public DateTime? PublishedAt   { get; set; }

        // Chat integration placeholder – populated when Chat API is wired up
        public int UnreadMessages { get; set; }
    }

    // ──────────────────────────────────────────────────────────────
    //  Chat integration placeholders
    //  These models are the contract surface the Chat API will fill.
    //  They sit here so the Teacher API compiles and the MVC layer
    //  can reference them without any Chat API being live yet.
    // ──────────────────────────────────────────────────────────────

    /// <summary>
    /// Represents a chat conversation visible to a teacher.
    /// Populated by ChatIntegrationService once the Chat API is connected.
    /// </summary>
    public class TeacherConversationDto
    {
        public int      ChatSessionId  { get; set; }
        public string   StudentId      { get; set; } = string.Empty;
        public string   StudentName    { get; set; } = string.Empty;
        public string   LastMessage    { get; set; } = string.Empty;
        public DateTime LastMessageAt  { get; set; }
        public int      UnreadCount    { get; set; }
    }

    /// <summary>A single chat message in a teacher–student session.</summary>
    public class ChatMessageDto
    {
        public int      Id             { get; set; }
        public int      ChatSessionId  { get; set; }
        public string   SenderRole     { get; set; } = string.Empty;  // "Teacher" | "Student"
        public string   Content        { get; set; } = string.Empty;
        public DateTime SentAt         { get; set; }
    }

    /// <summary>Payload to send a message in an existing chat session.</summary>
    public class SendChatMessageDto
    {
        public string Content { get; set; } = string.Empty;
    }

    // ──────────────────────────────────────────────────────────────
    //  Paged response – matches StudentService shape so MVC layer
    //  can use the same deserialization helper for both.
    // ──────────────────────────────────────────────────────────────

    public class PagedResponseDto<T>
    {
        public List<T> Data       { get; set; } = new();
        public int     Page       { get; set; }
        public int     PageSize   { get; set; }
        public int     TotalCount { get; set; }
        public int     TotalPages => PageSize > 0
            ? (int)Math.Ceiling((double)TotalCount / PageSize)
            : 0;
    }
}
