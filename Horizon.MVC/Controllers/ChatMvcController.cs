using Horizon.MVC.DTOs;
using Horizon.MVC.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace Horizon.MVC.Controllers
{
    [Route("chat")]
    public class ChatMvcController : Controller
    {
        private readonly HttpClient _httpClient;

        public ChatMvcController(IHttpClientFactory factory)
        {
            _httpClient = factory.CreateClient("ChatApi");
        }

        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("start-session")]
        public async Task<IActionResult> StartSession(string studentId, string teacherId)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(studentId) || string.IsNullOrWhiteSpace(teacherId))
                    return BadRequest("StudentId or TeacherId missing from form.");
                var response = await _httpClient.PostAsJsonAsync("api/chat/request-session",
                    new CreateSessionRequest
                    {
                        StudentId = studentId,
                        TeacherId = teacherId
                    });

                if (!response.IsSuccessStatusCode)
                {
                    var errorBody = await response.Content.ReadAsStringAsync();
                    return BadRequest($"API ERROR: {errorBody}");
                }

                var result = await response.Content.ReadFromJsonAsync<ChatSessionResponse>();

                if (result == null)
                    return BadRequest("Session response null from API");

                return RedirectToAction("Session", new { sessionId = result.Id, role = "Student" });
            }
            catch (Exception ex)
            {
                return BadRequest("MVC EXCEPTION: " + ex.Message + " | INNER: " + ex.InnerException?.Message);
            }
        }

        [HttpGet("session/{sessionId}")]
        public async Task<IActionResult> Session(int sessionId, string role)
        {
            var messages = await _httpClient.GetFromJsonAsync<List<MessageResponse>>(
                $"api/chat/history/{sessionId}") ?? new List<MessageResponse>();

            var vm = new ChatViewModel
            {
                SessionId = sessionId,
                CurrentUserId = "",
                CurrentUserRole = role,
                Messages = messages.Select(m => new MessageViewModel
                {
                    Id = m.Id,
                    Content = m.Content,
                    SenderRole = m.SenderRole,
                    SentAt = m.SentAt,
                    IsOwn = m.SenderRole == role
                }).ToList()
            };

            return View(vm);
        }

        [HttpPost("send")]
        public async Task<IActionResult> Send([FromBody] SendMessageRequest request)
        {
            var response = await _httpClient.PostAsJsonAsync("api/chat/send-message", request);

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                return BadRequest(errorBody);
            }

            var result = await response.Content.ReadFromJsonAsync<MessageResponse>();

            if (result == null)
                return BadRequest("Message response null from API");

            return Ok(result);
        }

        [HttpGet("session-messages/{sessionId}")]
        public async Task<IActionResult> GetSessionMessages(int sessionId)
        {
            var messages = await _httpClient.GetFromJsonAsync<List<MessageResponse>>(
                $"api/chat/history/{sessionId}") ?? new List<MessageResponse>();

            return Json(messages);
        }
    }
}

