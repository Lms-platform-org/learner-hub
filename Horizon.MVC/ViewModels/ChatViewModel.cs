namespace Horizon.MVC.ViewModels
{
    public class ChatViewModel
    {
        public int SessionId { get; set; }
        public string CurrentUserId { get; set; } = string.Empty;
        public string CurrentUserRole { get; set; } = "";
        public List<MessageViewModel> Messages { get; set; } = new();
    }

    public class MessageViewModel
    {
        public int Id { get; set; }
        public string SenderRole { get; set; } = "";
        public string Content { get; set; } = "";
        public DateTime SentAt { get; set; }
        public bool IsOwn { get; set; }
    }
}

