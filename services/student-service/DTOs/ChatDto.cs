namespace LearningPlatform.StudentService.DTOs
{
    public class ChatMessageDto
    {
        public int Id { get; set; }
        public int ChatSessionId { get; set; }
        public string SenderRole { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
    }

    public class SendMessageDto
    {
        public int ChatSessionId { get; set; }
        public string SenderRole { get; set; } = "Student";
        public string Content { get; set; } = string.Empty;
    }

    public class ConversationDto
    {
        public int ChatSessionId { get; set; }
        public int TeacherId { get; set; }
        public string TeacherName { get; set; } = string.Empty;
        public string LastMessage { get; set; } = string.Empty;
        public DateTime LastMessageAt { get; set; }
    }
}
