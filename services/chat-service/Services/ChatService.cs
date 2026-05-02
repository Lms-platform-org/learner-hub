using AutoMapper;
using WebApplication1.DTOs;
using WebApplication1.Exceptions;
using WebApplication1.Models;
using WebApplication1.Repositories;

namespace WebApplication1.Services
{

    public class ChatService : IChatService
    {
        private readonly IChatRepository _repo;

        private readonly IMapper _mapper;

        public ChatService(IChatRepository repo, IMapper mapper)
        {
            _repo = repo;
            _mapper = mapper;
        }
        
        public async Task<MessageResponse> ProcessMessageAsync(SendMessageRequest dto)
        {
            var wordCount = dto.Content?
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Length ?? 0;

            if (wordCount < 3)
                throw new BadRequestException("Message must contain at least 3 words.");

            var session = await _repo.GetSessionAsync(dto.ChatSessionId)
                ?? throw new NotFoundException("Chat session not found.");

            var message = new Message
            {
                ChatSessionId = dto.ChatSessionId,
                SenderRole = dto.SenderRole,
                Content = dto.Content,
                SentAt = DateTime.UtcNow
            };

            await _repo.SaveMessageAsync(message);

            return _mapper.Map<MessageResponse>(message);
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
                CreatedAt = DateTime.UtcNow
            };

            await _repo.CreateSessionAsync(session);

            return _mapper.Map<ChatSessionResponse>(session);
        }

        public async Task<IEnumerable<MessageResponse>> GetHistoryAsync(int sessionId)
        {
            var messages = await _repo.GetChatHistoryAsync(sessionId);
            return _mapper.Map<IEnumerable<MessageResponse>>(messages);
        }
    }
}