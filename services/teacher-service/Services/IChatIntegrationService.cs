using LearningPlatform.TeacherService.DTOs;

namespace LearningPlatform.TeacherService.Services
{
    /// <summary>
    /// Chat integration contract for the Teacher Service.
    ///
    /// This interface mirrors the shape used by Student API's IChatIntegrationService
    /// so both services have consistent Chat API interaction patterns.
    ///
    /// The stub implementation (<see cref="ChatIntegrationService"/>) returns empty
    /// collections so all Teacher API endpoints compile and run today.
    /// When the Chat API is ready, swap in a real HTTP implementation without
    /// touching the controller or dashboard service.
    /// </summary>
    public interface IChatIntegrationService
    {
        /// <summary>
        /// Get all chat conversations for this teacher.
        /// Returns the teacher's open/active sessions with students.
        /// </summary>
        Task<List<TeacherConversationDto>> GetConversationsAsync(string teacherId);

        /// <summary>
        /// Fetch the message history of a specific chat session.
        /// </summary>
        Task<List<ChatMessageDto>> GetMessagesAsync(int chatSessionId);

        /// <summary>
        /// Send a message in an existing chat session as the teacher.
        /// </summary>
        Task SendMessageAsync(int chatSessionId, string content, string teacherId);

        /// <summary>
        /// Count unread messages for a teacher across all their sessions.
        /// Used to populate the dashboard badge.
        /// </summary>
        Task<int> GetUnreadCountAsync(string teacherId);
    }
}
