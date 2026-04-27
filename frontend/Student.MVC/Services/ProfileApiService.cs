using LearningPlatform.StudentService.WebApp.Models.DTOs;

namespace LearningPlatform.StudentService.WebApp.Services
{
    public class ProfileApiService : BaseApiService
    {
        public ProfileApiService(IHttpClientFactory factory, IHttpContextAccessor httpContextAccessor)
            : base(factory, httpContextAccessor) { }

        public async Task<ProfileDto> GetAsync()
        {
            try
            {
                AttachToken();
                var response = await _client
                    .GetFromJsonAsync<ApiResponseDto<ProfileDto>>("api/profile");
                return response?.Data ?? new ProfileDto();
            }
            catch { return new ProfileDto(); }
        }

        public async Task SaveAsync(ProfileDto dto)
        {
            try
            {
                AttachToken();
                var response = await _client.PostAsJsonAsync("api/profile", dto);
                response.EnsureSuccessStatusCode();
            }
            catch { }
        }
    }
}
