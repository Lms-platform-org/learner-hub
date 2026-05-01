using Horizon.MVC.DTOs;

namespace Horizon.MVC.Services
{
    public class ProfileApiService : BaseApiService
    {
        public ProfileApiService(IHttpClientFactory factory, IHttpContextAccessor httpContextAccessor)
            : base(factory, httpContextAccessor) { }

        public async Task<ProfileDto> GetAsync()
        {
            AttachToken();
            var response = await _client
                .GetFromJsonAsync<ApiResponseDto<ProfileDto>>("api/profile");
            return response?.Data ?? new ProfileDto();
        }

        public async Task SaveAsync(ProfileDto dto)
        {
            AttachToken();
            var response = await _client.PostAsJsonAsync("api/profile", dto);
            response.EnsureSuccessStatusCode();
        }
    }
}


