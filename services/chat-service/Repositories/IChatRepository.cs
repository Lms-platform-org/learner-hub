using WebApplication1.Models;

namespace WebApplication1.Repositories
{
    public interface IChatRepository
    {
        Task<ChatSession> CreateSessionAsync(ChatSession session);
        Task<ChatSession> GetSessionAsync(int sessionId);
        Task<ChatSession> GetSessionByUsersAsync(string studentId, string teacherId);
        Task<Message> SaveMessageAsync(Message message); 
        Task<IEnumerable<Message>> GetChatHistoryAsync(int sessionId);
    }
}
