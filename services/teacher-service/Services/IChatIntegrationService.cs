using System.Collections.Generic;
using System.Threading.Tasks;
using TeacherDashboardApi.DTOs;

namespace TeacherDashboardApi.Services
{
    public interface IChatIntegrationService
    {
        Task<IEnumerable<ConversationDto>> GetConversationsAsync(string teacherId);
        Task<IEnumerable<ChatMessageDto>> GetMessagesAsync(int chatSessionId);
        Task SendMessageAsync(int chatSessionId, string content);
        Task<ConversationDto> StartConversationAsync(string teacherId, string studentId);
    }
}
