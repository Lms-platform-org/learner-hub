using Microsoft.AspNetCore.Mvc;
using WebApplication1.DTOs;
using WebApplication1.Services;

namespace WebApplication1.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChatController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatController(IChatService chatService)
        {
            _chatService = chatService;
        }

        // POST: api/chat/request-session
        [HttpPost("request-session")]
        public async Task<ActionResult<ChatSessionResponse>> RequestSession([FromBody] CreateSessionRequest request)
        {
            try
            {
                var result = await _chatService.RequestChatAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // POST: api/chat/send-message
        [HttpPost("send-message")]
        public async Task<ActionResult<MessageResponse>> SendMessage([FromBody] SendMessageRequest request)
        {
            try
            {
                var result = await _chatService.ProcessMessageAsync(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        // GET: api/chat/history/5
        [HttpGet("history/{sessionId}")]
        public async Task<ActionResult<IEnumerable<MessageResponse>>> GetHistory(int sessionId)
        {
            try
            {
                var result = await _chatService.GetHistoryAsync(sessionId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }
    }
}