using LearningPlatform.StudentService.Common;
using LearningPlatform.StudentService.DTOs;
using Microsoft.Extensions.Caching.Memory;

namespace LearningPlatform.StudentService.Services
{
    public class CourseIntegrationService : ICourseIntegrationService
    {
        private readonly IHttpClientFactory _factory;
        private readonly IMemoryCache _cache;
        private readonly ILogger<CourseIntegrationService> _logger;

        public CourseIntegrationService(IHttpClientFactory factory, IMemoryCache cache, ILogger<CourseIntegrationService> logger)
        {
            _factory = factory;
            _cache = cache;
            _logger = logger;
        }

        public async Task<int> GetTotalLessonsForCourseAsync(int courseId)
        {
            var course = await GetCourseByIdAsync(courseId);
            return course?.TotalLessons > 0 ? course.TotalLessons : 50;
        }

        public async Task<CourseDto?> GetCourseByIdAsync(int courseId)
        {
            var cacheKey = $"course_{courseId}";

            if (_cache.TryGetValue(cacheKey, out CourseDto? cached))
                return cached;

            try
            {
                var client = _factory.CreateClient("CourseService");
                // Use correct route: api/coursesapi instead of api/courses
                var response = await client.GetFromJsonAsync<ApiResponseDto<CourseDto>>($"api/coursesapi/{courseId}");

                var course = response?.Data;
                if (course != null)
                    _cache.Set(cacheKey, course, TimeSpan.FromMinutes(30));

                return course;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not reach CourseService for courseId {CourseId}", courseId);
                return null;
            }
        }

        public async Task<PagedResponseDto<CourseDto>> GetAllCoursesAsync(int page, int pageSize)
        {
            try
            {
                var client = _factory.CreateClient("CourseService");
                // Use correct route: api/coursesapi instead of api/courses
                var response = await client.GetFromJsonAsync<ApiResponseDto<PagedResponseDto<CourseDto>>>(
                    $"api/coursesapi?page={page}&pageSize={pageSize}");

                return response?.Data ?? EmptyPaged<CourseDto>(page, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not reach CourseService for course listing");
                return EmptyPaged<CourseDto>(page, pageSize);
            }
        }

        public async Task<PagedResponseDto<CourseDto>> SearchCoursesAsync(string query, int page, int pageSize)
        {
            try
            {
                var client = _factory.CreateClient("CourseService");
                // Use correct route: api/coursesapi/search instead of api/courses/search
                var response = await client.GetFromJsonAsync<ApiResponseDto<PagedResponseDto<CourseDto>>>(
                    $"api/coursesapi/search?query={Uri.EscapeDataString(query)}&page={page}&pageSize={pageSize}");

                return response?.Data ?? EmptyPaged<CourseDto>(page, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not reach CourseService for course search");
                return EmptyPaged<CourseDto>(page, pageSize);
            }
        }

        private static PagedResponseDto<T> EmptyPaged<T>(int page, int pageSize) =>
            new() { Data = new List<T>(), Page = page, PageSize = pageSize, TotalCount = 0 };

        public async Task NotifyEnrollmentAsync(string studentId, int courseId)
        {
            try
            {
                var client = _factory.CreateClient("CourseService");
                var payload = new { studentId, courseId };
                // Use correct route: api/coursesapi/enroll instead of api/courses/enroll
                await client.PostAsJsonAsync("api/coursesapi/enroll", payload);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not notify CourseService of enrollment for course {CourseId}", courseId);
            }
        }

        public async Task NotifyProgressAsync(string studentId, int courseId, int percentage)
        {
            try
            {
                var client = _factory.CreateClient("CourseService");
                var payload = new { studentId, courseId, percentage };
                // Use correct route: api/coursesapi/progress instead of api/courses/progress
                await client.PostAsJsonAsync("api/coursesapi/progress", payload);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Could not notify CourseService of progress for course {CourseId}", courseId);
            }
        }
    }
}