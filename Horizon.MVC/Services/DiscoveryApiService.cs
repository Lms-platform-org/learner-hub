using Horizon.MVC.DTOs;

namespace Horizon.MVC.Services
{
    public class DiscoveryApiService : BaseApiService
    {
        public DiscoveryApiService(IHttpClientFactory factory, IHttpContextAccessor httpContextAccessor)
            : base(factory, httpContextAccessor) { }

        public async Task<PagedResponseDto<CourseDto>> GetAllCoursesAsync(int page, int pageSize)
        {
            try
            {
                AttachToken();
                var response = await _client
                    .GetFromJsonAsync<ApiResponseDto<PagedResponseDto<CourseDto>>>(
                        $"api/discovery/courses?page={page}&pageSize={pageSize}");
                return response?.Data ?? Empty<CourseDto>(page, pageSize);
            }
            catch { return Empty<CourseDto>(page, pageSize); }
        }

        public async Task<PagedResponseDto<CourseDto>> SearchCoursesAsync(string query, int page, int pageSize)
        {
            try
            {
                AttachToken();
                var response = await _client
                    .GetFromJsonAsync<ApiResponseDto<PagedResponseDto<CourseDto>>>(
                        $"api/discovery/courses/search?query={Uri.EscapeDataString(query)}&page={page}&pageSize={pageSize}");
                return response?.Data ?? Empty<CourseDto>(page, pageSize);
            }
            catch { return Empty<CourseDto>(page, pageSize); }
        }

        public async Task<PagedResponseDto<BookDto>> SearchBooksAsync(string topic, int page, int pageSize)
        {
            try
            {
                AttachToken();
                var response = await _client
                    .GetFromJsonAsync<ApiResponseDto<PagedResponseDto<BookDto>>>(
                        $"api/discovery/books?topic={Uri.EscapeDataString(topic)}&page={page}&pageSize={pageSize}");
                return response?.Data ?? Empty<BookDto>(page, pageSize);
            }
            catch { return Empty<BookDto>(page, pageSize); }
        }

        public async Task<CourseDto?> GetCourseByIdAsync(int courseId)
        {
            try
            {
                AttachToken();
                var response = await _client
                    .GetFromJsonAsync<ApiResponseDto<CourseDto>>($"api/discovery/courses/{courseId}");
                return response?.Data;
            }
            catch { return null; }
        }

        private static PagedResponseDto<T> Empty<T>(int page, int pageSize) =>
            new() { Data = new List<T>(), Page = page, PageSize = pageSize, TotalCount = 0 };
    }
}


