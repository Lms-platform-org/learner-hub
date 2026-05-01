using Horizon.MVC.DTOs;
using Horizon.MVC.ViewModels;
using Horizon.MVC.Models;
using Microsoft.AspNetCore.Http;

namespace Horizon.MVC.Services
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
            if (string.IsNullOrEmpty(token))
            {
                throw new UnauthorizedAccessException("User session has expired or token is missing. Please log in again.");
            }

            _client.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        protected bool IsTokenPresent() =>
            !string.IsNullOrEmpty(_httpContextAccessor.HttpContext?.Session.GetString("jwt"));
    }
}


