using Microsoft.AspNetCore.Mvc;
using Serilog;
using WebApplication1.DTOs;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;
        private readonly ILogger<ChatController> _logger;

        public ChatController(IChatService chatService, ILogger<ChatController> logger)
        {
            _chatService = chatService;
            _logger = logger;
        }

        // POST: api/chat/request-session
        [HttpPost("request-session")]
        public async Task<ActionResult<ChatSessionResponse>> RequestSession([FromBody] CreateSessionRequest request)
        {
            _logger.LogInformation("Chat session requested. StudentId: {StudentId}, TeacherId: {TeacherId}",
                request.StudentId, request.TeacherId);

            var result = await _chatService.RequestChatAsync(request);

            _logger.LogInformation("Chat session created successfully. ChatSessionId: {ChatSessionId}", result.Id);

            return Ok(result);
        }

        // POST: api/chat/send-message
        [HttpPost("send-message")]
        public async Task<ActionResult<MessageResponse>> SendMessage([FromBody] SendMessageRequest request)
        {
            _logger.LogInformation("Message send request received. ChatSessionId: {ChatSessionId}, SenderRole: {SenderRole}",
                request.ChatSessionId, request.SenderRole);

            var result = await _chatService.ProcessMessageAsync(request);

            _logger.LogInformation("Message stored successfully. MessageId: {MessageId}, ChatSessionId: {ChatSessionId}",
                result.Id, result.ChatSessionId);

            return Ok(result);
        }

        // GET: api/chat/history/5
        [HttpGet("history/{sessionId}")]
        public async Task<ActionResult<IEnumerable<MessageResponse>>> GetHistory(int sessionId)
        {
            _logger.LogInformation("Fetching chat history for ChatSessionId: {ChatSessionId}", sessionId);

            var result = await _chatService.GetHistoryAsync(sessionId);

            _logger.LogInformation("Fetched {Count} messages for ChatSessionId: {ChatSessionId}",
                result.Count(), sessionId);

            return Ok(result);
        }
    }

}