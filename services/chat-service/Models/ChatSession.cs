namespace WebApplication1.Models
{
    public class ChatSession
    {
        public int Id { get; set; }
        public string TeacherId { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public ICollection<Message> Messages { get; set; } = new List<Message>();
        public User? Teacher { get; set; }
        public User? Student { get; set; }
    }
}
