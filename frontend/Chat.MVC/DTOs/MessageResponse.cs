namespace Chat_MVC.DTOs
{
    public class MessageResponse
    {
        public int Id { get; set; }
        public int ChatSessionId { get; set; }
        public string SenderRole { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
    }
}
