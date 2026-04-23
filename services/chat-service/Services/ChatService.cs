using WebApplication1.DTOs;
using WebApplication1.Models;
using WebApplication1.Repositories;

namespace WebApplication1.Services
{

    public class ChatService : IChatService
    {
        private readonly IChatRepository _repo;

        public ChatService(IChatRepository repo)
        {
            _repo = repo;
        }

        public async Task<MessageResponse> ProcessMessageAsync(SendMessageRequest dto)
        {
            var wordCount = dto.Content?
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Length ?? 0;

            if (wordCount < 3)
                throw new InvalidOperationException("Message must contain at least 3 words.");

            var session = await _repo.GetSessionAsync(dto.ChatSessionId)
                ?? throw new InvalidOperationException("Chat session not found.");

            var message = new Message
            {
                ChatSessionId = dto.ChatSessionId,
                SenderRole = dto.SenderRole,
                Content = dto.Content,
                SentAt = DateTime.UtcNow
            };

            await _repo.SaveMessageAsync(message);

            return new MessageResponse
            {
                Id = message.Id,
                ChatSessionId = message.ChatSessionId,
                SenderRole = message.SenderRole,
                Content = message.Content,
                SentAt = message.SentAt
            };
        }

        public async Task<ChatSessionResponse> RequestChatAsync(CreateSessionRequest dto)
        {
            var existing = await _repo.GetSessionByUsersAsync(dto.StudentId, dto.TeacherId);
            if (existing != null)
            {
                return new ChatSessionResponse
                {
                    Id = existing.Id,
                    TeacherId = existing.TeacherId,
                    StudentId = existing.StudentId,
                    CreatedAt = existing.CreatedAt
                };
            }

            var session = new ChatSession
            {
                TeacherId = dto.TeacherId,
                StudentId = dto.StudentId,
                CreatedAt = DateTime.UtcNow // Ensure UTC
            };

            await _repo.CreateSessionAsync(session);

            return new ChatSessionResponse
            {
                Id = session.Id,
                TeacherId = session.TeacherId,
                StudentId = session.StudentId,
                CreatedAt = session.CreatedAt
            };
        }

        public async Task<IEnumerable<MessageResponse>> GetHistoryAsync(int sessionId)
        {
            var messages = await _repo.GetChatHistoryAsync(sessionId);
            return messages.Select(m => new MessageResponse
            {
                Id = m.Id,
                ChatSessionId = m.ChatSessionId,
                SenderRole = m.SenderRole,
                Content = m.Content,
                SentAt = m.SentAt
            });
        }
    }
}