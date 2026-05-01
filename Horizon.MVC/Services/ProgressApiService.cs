using Horizon.MVC.DTOs;

namespace Horizon.MVC.Services
{
    public class ProgressApiService : BaseApiService
    {
        public ProgressApiService(IHttpClientFactory factory, IHttpContextAccessor httpContextAccessor)
            : base(factory, httpContextAccessor) { }

        public async Task<StudentProgress?> GetAsync(int courseId)
        {
            try
            {
                AttachToken();
                var response = await _client
                    .GetFromJsonAsync<ApiResponseDto<StudentProgress>>(
                        $"api/progress/{courseId}");
                return response?.Data;
            }
            catch { return null; }
        }

        public async Task CompleteAsync(int courseId, int lessonId)
        {
            try
            {
                AttachToken();
                var response = await _client.PostAsync(
                    $"api/progress/complete/{courseId}/{lessonId}", null);
                response.EnsureSuccessStatusCode();
            }
            catch { }
        }
    }
}


