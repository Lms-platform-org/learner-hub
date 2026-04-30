using System;
using System.Collections.Generic;

namespace TeacherDashboardApi.DTOs
{
    public class ChatMessageDto
    {
        public int Id { get; set; }
        public int ChatSessionId { get; set; }
        public string SenderRole { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime SentAt { get; set; }
    }

    public class SendMessageDto
    {
        public int ChatSessionId { get; set; }
        public string SenderRole { get; set; } = "Teacher";
        public string Content { get; set; } = string.Empty;
    }

    public class ConversationDto
    {
        public int ChatSessionId { get; set; }
        public string StudentId { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public string LastMessage { get; set; } = string.Empty;
        public DateTime LastMessageAt { get; set; }
    }

    public class ApiResponseDto<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }

        public static ApiResponseDto<T> Ok(T? data, string message = "Success")
        {
            return new ApiResponseDto<T> { Success = true, Data = data, Message = message };
        }

        public static ApiResponseDto<T> Fail(string message)
        {
            return new ApiResponseDto<T> { Success = false, Message = message };
        }
    }
}
