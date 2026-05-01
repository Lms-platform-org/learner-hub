namespace Horizon.MVC.DTOs
{
    public class ChatSessionResponse
    {
        public int Id { get; set; }
        public string TeacherId { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}


