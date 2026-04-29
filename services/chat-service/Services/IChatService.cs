using WebApplication1.DTOs;

namespace WebApplication1.Services
{
    public interface IChatService
    {
        Task<ChatSessionResponse> RequestChatAsync(CreateSessionRequest dto);
        Task<MessageResponse> ProcessMessageAsync(SendMessageRequest dto);
        Task<IEnumerable<MessageResponse>> GetHistoryAsync(int sessionId);
    }
}
