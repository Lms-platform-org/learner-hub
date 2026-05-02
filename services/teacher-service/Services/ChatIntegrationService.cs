using System.Net.Http.Json;
using LearningPlatform.TeacherService.DTOs;
using LearningPlatform.TeacherService.Exceptions;

namespace LearningPlatform.TeacherService.Services
{
    /// <summary>
    /// Concrete Chat API client for the Teacher Service.
    ///
    /// Current behaviour:
    ///   - All methods call the Chat API (at "ChatApi" named client base address).
    ///   - If the Chat API is unreachable the methods log a warning and return
    ///     safe empty results instead of propagating the exception, so the
    ///     teacher dashboard still loads.
    ///   - SendMessageAsync is the one exception: failure is surfaced to the
    ///     caller because a silent drop of a sent message would be confusing.
    ///
    /// Chat API endpoints expected (from chat-service ChatController):
    ///   POST api/chat/request-session           → ChatSessionResponse
    ///   POST api/chat/send-message              → MessageResponse
    ///   GET  api/chat/history/{sessionId}       → IEnumerable&lt;MessageResponse&gt;
    ///
    /// TODO: replace stub conversation/unread endpoints once Chat API exposes them.
    /// </summary>
    public class ChatIntegrationService : IChatIntegrationService
    {
        private readonly IHttpClientFactory _factory;
        private readonly ILogger<ChatIntegrationService> _logger;

        // Named client key – registered in Program.cs
        private const string ClientName = "ChatApi";

        public ChatIntegrationService(
            IHttpClientFactory factory,
            ILogger<ChatIntegrationService> logger)
        {
            _factory = factory;
            _logger  = logger;
        }

        /// <inheritdoc/>
        /// <remarks>
        /// The Chat API does not yet expose a teacher-scoped conversations endpoint.
        /// Returns an empty list so the dashboard renders without errors.
        /// Replace the stub body with the real HTTP call once the endpoint exists:
        ///   GET api/chat/conversations?teacherId={teacherId}
        /// </remarks>
        public Task<List<TeacherConversationDto>> GetConversationsAsync(string teacherId)
        {
            _logger.LogInformation(
                "[CHAT STUB] GetConversationsAsync called for teacher {TeacherId}. " +
                "Chat API conversation list endpoint not yet available.", teacherId);

            // TODO: replace with real call:
            // var client = _factory.CreateClient(ClientName);
            // var result = await client.GetFromJsonAsync<List<TeacherConversationDto>>(
            //     $"api/chat/conversations?teacherId={teacherId}");
            // return result ?? new();

            return Task.FromResult(new List<TeacherConversationDto>());
        }

        /// <inheritdoc/>
        public async Task<List<ChatMessageDto>> GetMessagesAsync(int chatSessionId)
        {
            try
            {
                var client = _factory.CreateClient(ClientName);
                // Chat API route: GET api/chat/history/{sessionId}
                var result = await client.GetFromJsonAsync<List<ChatMessageDto>>(
                    $"api/chat/history/{chatSessionId}");

                return result ?? new List<ChatMessageDto>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex,
                    "Could not reach Chat API for message history of session {SessionId}", chatSessionId);
                return new List<ChatMessageDto>();
            }
        }

        /// <inheritdoc/>
        public async Task SendMessageAsync(int chatSessionId, string content, string teacherId)
        {
            try
            {
                var client  = _factory.CreateClient(ClientName);
                var payload = new
                {
                    ChatSessionId = chatSessionId,
                    SenderRole    = "Teacher",
                    Content       = content
                };
                // Chat API route: POST api/chat/send-message
                var response = await client.PostAsJsonAsync("api/chat/send-message", payload);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,
                    "Failed to send message in session {SessionId} for teacher {TeacherId}",
                    chatSessionId, teacherId);
                throw new ChatApiException("Chat service is currently unavailable. Please try again later.");
            }
        }

        /// <inheritdoc/>
        /// <remarks>
        /// Chat API does not yet expose a per-teacher unread count.
        /// Returns 0 so dashboard badges render without errors.
        /// </remarks>
        public Task<int> GetUnreadCountAsync(string teacherId)
        {
            _logger.LogInformation(
                "[CHAT STUB] GetUnreadCountAsync for teacher {TeacherId}. " +
                "Unread count endpoint not yet available.", teacherId);

            // TODO: replace with real call once endpoint exists:
            // GET api/chat/unread?teacherId={teacherId}

            return Task.FromResult(0);
        }
    }
}
