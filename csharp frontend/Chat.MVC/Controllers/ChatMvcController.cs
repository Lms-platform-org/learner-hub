[HttpGet("session/{sessionId}")]
public async Task<IActionResult> Session(int sessionId, string role)
{
    var apiResult = await _httpClient.GetFromJsonAsync<ApiResponse<List<MessageResponse>>>(
        $"api/chat/history/{sessionId}");

    var messages = apiResult?.Data ?? new List<MessageResponse>();

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