using Horizon.MVC.DTOs;

namespace Horizon.MVC.Services
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

        // returns true = bookmarked, false = removed
        public async Task<bool> ToggleAsync(BookmarkDto dto)
        {
            try
            {
                AttachToken();
                var response = await _client.PostAsJsonAsync("api/bookmarks/toggle", dto);
                var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<ToggleResult>>();
                return result?.Data?.IsBookmarked ?? false;
            }
            catch { return false; }
        }

        public async Task AddAsync(BookmarkDto dto)
        {
            try
            {
                AttachToken();
                await _client.PostAsJsonAsync("api/bookmarks/toggle", dto);
            }
            catch { }
        }

        public async Task UpdateNoteAsync(int id, string? note)
        {
            try
            {
                AttachToken();
                await _client.PatchAsJsonAsync($"api/bookmarks/{id}/note", new { note });
            }
            catch { }
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
                await _client.DeleteAsync($"api/bookmarks/{id}");
            }
            catch { }
        }

        public async Task<bool> IsBookmarkedAsync(BookmarkDto dto)
        {
            try
            {
                AttachToken();
                var response = await _client.PostAsJsonAsync("api/bookmarks/check", dto);
                var result = await response.Content.ReadFromJsonAsync<ApiResponseDto<CheckResult>>();
                return result?.Data?.IsBookmarked ?? false;
            }
            catch { return false; }
        }
    }

    public class ToggleResult
    {
        public bool IsBookmarked { get; set; }
    }

    public class CheckResult
    {
        public bool IsBookmarked { get; set; }
    }
}


