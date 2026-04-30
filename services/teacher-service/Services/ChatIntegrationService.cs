using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeacherDashboardApi.DTOs;

namespace TeacherDashboardApi.Services
{
    public class ChatIntegrationService : IChatIntegrationService
    {
        public async Task<IEnumerable<ConversationDto>> GetConversationsAsync(string teacherId)
        {
            // Simulate some conversations
            var conversations = new List<ConversationDto>
            {
                new ConversationDto
                {
                    ChatSessionId = 101,
                    StudentId = "Student1",
                    StudentName = "John Doe",
                    LastMessage = "Hi professor, could you help with module 3?",
                    LastMessageAt = DateTime.UtcNow.AddHours(-2)
                },
                new ConversationDto
                {
                    ChatSessionId = 102,
                    StudentId = "Student2",
                    StudentName = "Jane Smith",
                    LastMessage = "Thank you for the explanation!",
                    LastMessageAt = DateTime.UtcNow.AddDays(-1)
                }
            };

            return await Task.FromResult(conversations);
        }

        public async Task<IEnumerable<ChatMessageDto>> GetMessagesAsync(int chatSessionId)
        {
            var messages = new List<ChatMessageDto>
            {
                new ChatMessageDto
                {
                    Id = 1,
                    ChatSessionId = chatSessionId,
                    SenderRole = "Student",
                    Content = "Hi professor, could you help with module 3?",
                    SentAt = DateTime.UtcNow.AddHours(-2)
                },
                new ChatMessageDto
                {
                    Id = 2,
                    ChatSessionId = chatSessionId,
                    SenderRole = "Teacher",
                    Content = "Sure, I'd be happy to help. What's the issue?",
                    SentAt = DateTime.UtcNow.AddHours(-1)
                }
            };

            return await Task.FromResult(messages);
        }

        public async Task SendMessageAsync(int chatSessionId, string content)
        {
            // Simulate sending message
            await Task.CompletedTask;
        }

        public async Task<ConversationDto> StartConversationAsync(string teacherId, string studentId)
        {
            var conversation = new ConversationDto
            {
                ChatSessionId = 103,
                StudentId = studentId,
                StudentName = "New Student",
                LastMessage = "Conversation started by teacher.",
                LastMessageAt = DateTime.UtcNow
            };

            return await Task.FromResult(conversation);
        }
    }
}
