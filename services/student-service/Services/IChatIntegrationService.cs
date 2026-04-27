using LearningPlatform.StudentService.DTOs;

namespace LearningPlatform.StudentService.Services
{
    public interface IChatIntegrationService
    {
        Task<List<ConversationDto>> GetConversationsAsync(string studentId);
        Task<List<ChatMessageDto>> GetMessagesAsync(int chatSessionId);
        Task SendMessageAsync(int chatSessionId, string content);
        Task<ConversationDto> StartConversationAsync(string studentId, int teacherId);
    }
}
