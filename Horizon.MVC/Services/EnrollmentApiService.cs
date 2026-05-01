using Horizon.MVC.DTOs;

namespace Horizon.MVC.Services
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

        public async Task<List<int>> GetEnrolledCourseIdsAsync()
        {
            try
            {
                AttachToken();
                var response = await _client
                    .GetFromJsonAsync<ApiResponseDto<List<EnrollmentDto>>>("api/enrollment");
                return response?.Data?.Select(e => e.CourseId).ToList() ?? new List<int>();
            }
            catch { return new List<int>(); }
        }
    }

    public class EnrollmentDto
    {
        public int CourseId { get; set; }
        public DateTime EnrolledAt { get; set; }
    }
}


