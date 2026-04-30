using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TeacherDashboardApi.DTOs;
using TeacherDashboardApi.Services;

namespace TeacherDashboardApi.Controllers
{
    [Authorize(Roles = "Teacher")]
    [ApiController]
    [Route("api/[controller]")]
    public class ChatController : ControllerBase
    {
        private readonly IChatIntegrationService _chatService;
        private readonly ILogger<ChatController> _logger;

        public ChatController(IChatIntegrationService chatService, ILogger<ChatController> logger)
        {
            _chatService = chatService;
            _logger = logger;
        }

        // GET /api/chat/conversations
        // Returns all chat sessions for the logged-in teacher
        [HttpGet("conversations")]
        public async Task<IActionResult> GetConversations()
        {
            var teacherId = User.Identity?.Name;
            if (string.IsNullOrEmpty(teacherId)) return Unauthorized();

            _logger.LogInformation("Fetching conversations for teacher {TeacherId}", teacherId);
            var result = await _chatService.GetConversationsAsync(teacherId);
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

        // POST /api/chat/sessions/start/{studentId}
        // Start a new conversation with a student
        [HttpPost("sessions/start/{studentId}")]
        public async Task<IActionResult> StartConversation(string studentId)
        {
            var teacherId = User.Identity?.Name;
            if (string.IsNullOrEmpty(teacherId)) return Unauthorized();

            if (string.IsNullOrWhiteSpace(studentId))
                return BadRequest(ApiResponseDto<string>.Fail("Invalid student id"));

            _logger.LogInformation("Starting conversation with student {StudentId} by teacher {TeacherId}", studentId, teacherId);
            var result = await _chatService.StartConversationAsync(teacherId, studentId);
            return Created("", ApiResponseDto<object>.Ok(result));
        }
    }
}
