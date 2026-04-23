using LearningPlatform.StudentService.DTOs;

namespace LearningPlatform.StudentService.Services
{
    public class ChatIntegrationService : IChatIntegrationService
    {
        private readonly IHttpClientFactory _factory;
        private readonly ILogger<ChatIntegrationService> _logger;

        public ChatIntegrationService(IHttpClientFactory factory, ILogger<ChatIntegrationService> logger)
        {
            _factory = factory;
            _logger = logger;
        }

        public async Task<List<ConversationDto>> GetConversationsAsync(string studentId)
        {
            try
            {
                var client = _factory.CreateClient("ChatService");
                var result = await client.GetFromJsonAsync<List<ConversationDto>>(
                    $"api/chat/conversations?studentId={studentId}");
                return result ?? new List<ConversationDto>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not reach ChatService for conversations");
                return new List<ConversationDto>();
            }
        }

        public async Task<List<ChatMessageDto>> GetMessagesAsync(int chatSessionId)
        {
            try
            {
                var client = _factory.CreateClient("ChatService");
                var result = await client.GetFromJsonAsync<List<ChatMessageDto>>(
                    $"api/chat/sessions/{chatSessionId}/messages");
                return result ?? new List<ChatMessageDto>();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not reach ChatService for messages in session {SessionId}", chatSessionId);
                return new List<ChatMessageDto>();
            }
        }

        public async Task SendMessageAsync(int chatSessionId, string content)
        {
            try
            {
                var client = _factory.CreateClient("ChatService");
                var payload = new { chatSessionId, senderRole = "Student", content };
                var response = await client.PostAsJsonAsync("api/chat/messages", payload);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not reach ChatService to send message");
                throw new Exception("Chat service is currently unavailable");
            }
        }

        public async Task<ConversationDto> StartConversationAsync(string studentId, int teacherId)
        {
            try
            {
                var client = _factory.CreateClient("ChatService");
                var payload = new { studentId, teacherId };
                var response = await client.PostAsJsonAsync("api/chat/sessions", payload);
                response.EnsureSuccessStatusCode();
                var result = await response.Content.ReadFromJsonAsync<ConversationDto>();
                return result ?? new ConversationDto();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not reach ChatService to start conversation");
                throw new Exception("Chat service is currently unavailable");
            }
        }
    }
}
