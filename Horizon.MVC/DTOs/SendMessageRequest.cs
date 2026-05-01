namespace Horizon.MVC.DTOs
{
    public class SendMessageRequest
    {
        public int ChatSessionId { get; set; }
        public string SenderRole { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
    }
}


