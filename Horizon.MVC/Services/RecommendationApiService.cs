using Horizon.MVC.DTOs;

namespace Horizon.MVC.Services
{
    public class RecommendationApiService : BaseApiService
    {
        public RecommendationApiService(IHttpClientFactory factory, IHttpContextAccessor httpContextAccessor)
            : base(factory, httpContextAccessor) { }

        public async Task<List<int>> GetPersonalizedAsync()
        {
            try
            {
                AttachToken();
                var response = await _client
                    .GetFromJsonAsync<ApiResponseDto<List<int>>>(
                        "api/recommendation/personalized");
                return response?.Data ?? new List<int>();
            }
            catch { return new List<int>(); }
        }
    }
}


