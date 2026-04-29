namespace WebApplication1.DTOs
{
    public class CreateSessionRequest
    {
        public string StudentId { get; set; } = string.Empty;
        public string TeacherId { get; set; } = string.Empty;
    }

    public class SendMessageRequest
    {
        public int ChatSessionId { get; set; }
        public string SenderRole { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }

    public class MessageResponse
    {
        public int Id { get; set; }
        public int ChatSessionId { get; set; }
        public string SenderRole { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
    }

    public class ChatSessionResponse
    {
        public int Id { get; set; }
        public string TeacherId { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}