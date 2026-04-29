namespace WebApplication1.Models
{
    public class Message
    {
        public int Id { get; set; }
        public int ChatSessionId { get; set; }
        public string SenderRole { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; } = DateTime.UtcNow; // Changed to UtcNow

        public ChatSession? ChatSession { get; set; }
    }
}