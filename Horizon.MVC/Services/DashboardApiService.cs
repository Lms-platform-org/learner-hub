using Horizon.MVC.DTOs;

namespace Horizon.MVC.Services
{
    public class DashboardApiService : BaseApiService
    {
        public DashboardApiService(IHttpClientFactory factory, IHttpContextAccessor httpContextAccessor)
            : base(factory, httpContextAccessor) { }

        public async Task<DashboardDto> GetAsync()
        {
            try
            {
                AttachToken();
                var response = await _client
                    .GetFromJsonAsync<ApiResponseDto<DashboardDto>>("api/dashboard");
                return response?.Data ?? new DashboardDto();
            }
            catch { return new DashboardDto(); }
        }
    }
}


