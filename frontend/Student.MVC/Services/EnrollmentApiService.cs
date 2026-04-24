using LearningPlatform.StudentService.WebApp.Models.DTOs;

namespace LearningPlatform.StudentService.WebApp.Services
{
    public class EnrollmentApiService : BaseApiService
    {
        public EnrollmentApiService(IHttpClientFactory factory, IHttpContextAccessor httpContextAccessor)
            : base(factory, httpContextAccessor) { }

        public async Task EnrollAsync(int courseId)
        {
            try
            {
                AttachToken();
                var response = await _client.PostAsync($"api/enrollment/{courseId}", null);
                response.EnsureSuccessStatusCode();
            }
            catch { }
        }

        public async Task<bool> IsEnrolledAsync(int courseId)
        {
            try
            {
                AttachToken();
                var response = await _client
                    .GetFromJsonAsync<ApiResponseDto<object>>($"api/enrollment/{courseId}/status");
                return response?.Data?.ToString()?.Contains("true") ?? false;
            }
            catch { return false; }
        }
    }
}
