using Microsoft.AspNetCore.Http;

namespace LearningPlatform.StudentService.WebApp.Services
{
    public abstract class BaseApiService
    {
        protected readonly HttpClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;

        protected BaseApiService(IHttpClientFactory factory, IHttpContextAccessor httpContextAccessor)
        {
            _client = factory.CreateClient("StudentAPI");
            _httpContextAccessor = httpContextAccessor;
        }

        protected void AttachToken()
        {
            var token = _httpContextAccessor.HttpContext?.Session.GetString("jwt");
            if (!string.IsNullOrEmpty(token))
            {
                _client.DefaultRequestHeaders.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
            }
        }

        protected bool IsTokenPresent() =>
            !string.IsNullOrEmpty(_httpContextAccessor.HttpContext?.Session.GetString("jwt"));
    }
}
