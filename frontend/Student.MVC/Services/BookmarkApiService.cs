using LearningPlatform.StudentService.WebApp.Models.DTOs;

namespace LearningPlatform.StudentService.WebApp.Services
{
    public class BookmarkApiService : BaseApiService
    {
        public BookmarkApiService(IHttpClientFactory factory, IHttpContextAccessor httpContextAccessor)
            : base(factory, httpContextAccessor) { }

        public async Task<PagedResponseDto<BookmarkDto>> GetAllAsync(int page, int pageSize)
        {
            try
            {
                AttachToken();
                var response = await _client
                    .GetFromJsonAsync<ApiResponseDto<PagedResponseDto<BookmarkDto>>>(
                        $"api/bookmarks?page={page}&pageSize={pageSize}");
                return response?.Data ?? new PagedResponseDto<BookmarkDto>();
            }
            catch { return new PagedResponseDto<BookmarkDto>(); }
        }

        public async Task<BookmarkDto> GetByIdAsync(int id)
        {
            try
            {
                AttachToken();
                var response = await _client
                    .GetFromJsonAsync<ApiResponseDto<BookmarkDto>>($"api/bookmarks/{id}");
                return response?.Data ?? new BookmarkDto();
            }
            catch { return new BookmarkDto(); }
        }

        public async Task AddAsync(BookmarkDto dto)
        {
            try
            {
                AttachToken();
                var response = await _client.PostAsJsonAsync("api/bookmarks", dto);
                response.EnsureSuccessStatusCode();
            }
            catch { /* silently fail — user sees no crash */ }
        }

        public async Task UpdateAsync(int id, BookmarkDto dto)
        {
            try
            {
                AttachToken();
                var response = await _client.PutAsJsonAsync($"api/bookmarks/{id}", dto);
                response.EnsureSuccessStatusCode();
            }
            catch { }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                AttachToken();
                var response = await _client.DeleteAsync($"api/bookmarks/{id}");
                response.EnsureSuccessStatusCode();
            }
            catch { }
        }
    }
}
