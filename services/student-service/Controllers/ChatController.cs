using LearningPlatform.StudentService.Common;
using LearningPlatform.StudentService.DTOs;
using LearningPlatform.StudentService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.StudentService.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : BaseController
    {
        private readonly IChatIntegrationService _chatService;
        private readonly ILogger<ChatController> _logger;

        public ChatController(IChatIntegrationService chatService, ILogger<ChatController> logger)
        {
            _chatService = chatService;
            _logger = logger;
        }

        // GET /api/chat/conversations
        // Returns all chat sessions for the logged-in student
        [HttpGet("conversations")]
        public async Task<IActionResult> GetConversations()
        {
            _logger.LogInformation("Fetching conversations");
            var result = await _chatService.GetConversationsAsync(GetUserId());
            return Ok(ApiResponseDto<object>.Ok(result));
        }

        // GET /api/chat/sessions/{chatSessionId}/messages
        // Returns all messages in a specific chat session
        [HttpGet("sessions/{chatSessionId:int}/messages")]
        public async Task<IActionResult> GetMessages(int chatSessionId)
        {
            if (chatSessionId <= 0)
                return BadRequest(ApiResponseDto<string>.Fail("Invalid session id"));

            _logger.LogInformation("Fetching messages for session {SessionId}", chatSessionId);
            var result = await _chatService.GetMessagesAsync(chatSessionId);
            return Ok(ApiResponseDto<object>.Ok(result));
        }

        // POST /api/chat/sessions/{chatSessionId}/messages
        // Send a message in an existing chat session
        [HttpPost("sessions/{chatSessionId:int}/messages")]
        public async Task<IActionResult> SendMessage(int chatSessionId, [FromBody] SendMessageDto dto)
        {
            if (chatSessionId <= 0)
                return BadRequest(ApiResponseDto<string>.Fail("Invalid session id"));

            if (string.IsNullOrWhiteSpace(dto.Content))
                return BadRequest(ApiResponseDto<string>.Fail("Message cannot be empty"));

            _logger.LogInformation("Sending message in session {SessionId}", chatSessionId);
            await _chatService.SendMessageAsync(chatSessionId, dto.Content);
            return Ok(ApiResponseDto<string>.Ok("Message sent"));
        }

        // POST /api/chat/sessions/start/{teacherId}
        // Start a new conversation with a teacher
        [HttpPost("sessions/start/{teacherId:int}")]
        public async Task<IActionResult> StartConversation(int teacherId)
        {
            if (teacherId <= 0)
                return BadRequest(ApiResponseDto<string>.Fail("Invalid teacher id"));

            _logger.LogInformation("Starting conversation with teacher {TeacherId}", teacherId);
            var result = await _chatService.StartConversationAsync(GetUserId(), teacherId);
            return Created("", ApiResponseDto<object>.Ok(result));
        }
    }
}
