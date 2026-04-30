using System;
using System.Collections.Generic;

namespace Courses.Models.DTOs
{
    public class ChatSessionDTO
    {
        public int Id { get; set; }
        // without TId, SId & CreatedAt as requested
    }

    public class MessageDTO
    {
        public string Sender { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
    }

    public class SendMessageDTO
    {
        public int ChatSessionId { get; set; }
        public string Content { get; set; } = string.Empty;
    }
}
