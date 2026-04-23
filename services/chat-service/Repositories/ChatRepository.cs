using Microsoft.EntityFrameworkCore;
using WebApplication1.Data;
using WebApplication1.Models;

namespace WebApplication1.Repositories
{

    public class ChatRepository : IChatRepository
    {
        private readonly WebApplication1Context _context;

        public ChatRepository(WebApplication1Context context)
        {
            _context = context;
        }

        public async Task<ChatSession> CreateSessionAsync(ChatSession session)
        {
            _context.ChatSessions.Add(session);
            await _context.SaveChangesAsync();
            return session;
        }

        public async Task<ChatSession?> GetSessionAsync(int sessionId)
        {
            return await _context.ChatSessions.FindAsync(sessionId);
        }

        public async Task<ChatSession?> GetSessionByUsersAsync(int studentId, int teacherId)
        {
            return await _context.ChatSessions
                .FirstOrDefaultAsync(s => s.StudentId == studentId && s.TeacherId == teacherId);
        }

        public async Task<Message> SaveMessageAsync(Message message)
        {
            _context.Messages.Add(message);
            await _context.SaveChangesAsync();
            return message;
        }

        private static readonly TimeSpan MessageRetentionWindow = TimeSpan.FromDays(15);

        public async Task<IEnumerable<Message>> GetChatHistoryAsync(int sessionId)
        {
            var cutoff = DateTime.UtcNow.Subtract(MessageRetentionWindow);

            return await _context.Messages
                .AsNoTracking()
                .Where(m => m.ChatSessionId == sessionId && m.SentAt >= cutoff)
                .OrderBy(m => m.SentAt)
                .ToListAsync();
        }
    }
}