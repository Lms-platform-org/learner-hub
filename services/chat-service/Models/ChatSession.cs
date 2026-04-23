namespace WebApplication1.Models
{
    public class ChatSession
    {
        public int Id { get; set; }
        public int TeacherId { get; set; }
        public int StudentId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; // Changed to UtcNow

        public ICollection<Message> Messages { get; set; } = new List<Message>();
        public User? Teacher { get; set; }
        public User? Student { get; set; }
    }
}
