namespace Chat_MVC.Models.ViewModels
{
    public class SessionViewModel
    {
        public int Id { get; set; }
        public string TeacherId { get; set; } = string.Empty;
        public string StudentId { get; set; } = string.Empty;
            public string TeacherName { get; set; } = string.Empty;
            public string StudentName { get; set; } = string.Empty;
            public string LastMessage { get; set; } = "No messages yet";
            public DateTime? LastMessageAt { get; set; }
            public int UnreadCount { get; set; }
        
    }
}
