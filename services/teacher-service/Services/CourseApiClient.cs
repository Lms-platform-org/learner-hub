using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using LearningPlatform.TeacherService.Common;
using LearningPlatform.TeacherService.DTOs;
using LearningPlatform.TeacherService.Exceptions;

namespace LearningPlatform.TeacherService.Services
{
    /// <summary>
    /// Concrete HTTP client that talks to the Course API through the gateway.
    ///
    /// Response unwrapping strategy:
    ///   The Course API's ResponseWrapperFilter wraps every successful response as:
    ///   { "success": true, "message": "Success", "data": <actual payload> }
    ///
    ///   Because the filter serialises the inner object as System.Text.Json
    ///   anonymous type → object, the generic parameter on the wire is always
    ///   object/JsonElement.  We therefore deserialise in two steps:
    ///   1. Read ApiResponseDto&lt;JsonElement&gt;
    ///   2. Re-deserialise data element into the target type.
    ///
    ///   This is the same approach used in the MVC layer's CoursesController.
    /// </summary>
    public class CourseApiClient : ICourseApiClient
    {
        private readonly IHttpClientFactory _factory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILogger<CourseApiClient> _logger;

        private static readonly JsonSerializerOptions _jsonOpts = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public CourseApiClient(
            IHttpClientFactory factory,
            IHttpContextAccessor httpContextAccessor,
            ILogger<CourseApiClient> logger)
        {
            _factory = factory;
            _httpContextAccessor = httpContextAccessor;
            _logger = logger;
        }

        // ─────────────────────────────────────────────────────────
        //  Private helpers
        // ─────────────────────────────────────────────────────────

        private HttpClient CreateClient()
        {
            var client = _factory.CreateClient("CourseApi");

            // Forward the bearer token from the incoming request so the Course API
            // can verify the teacher's role via its MockAuthMiddleware / real JWT.
            var token = _httpContextAccessor.HttpContext?.Request.Headers["Authorization"]
                            .FirstOrDefault()?.Replace("Bearer ", "", StringComparison.OrdinalIgnoreCase);

            if (!string.IsNullOrEmpty(token))
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            return client;
        }

        /// <summary>
        /// Unwrap a 200-level response that may be either:
        ///   a) Already the target type (raw list/object)
        ///   b) Wrapped in ApiResponse { data: … }
        /// </summary>
        private static async Task<T> UnwrapAsync<T>(HttpResponseMessage response) where T : new()
        {
            var json = await response.Content.ReadAsStringAsync();

            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            // ResponseWrapperFilter produces { success, message, data }
            if (root.TryGetProperty("data", out var dataEl) ||
                root.TryGetProperty("Data", out dataEl))
            {
                return JsonSerializer.Deserialize<T>(dataEl.GetRawText(), _jsonOpts) ?? new T();
            }

            // No wrapper – deserialise the root directly
            return JsonSerializer.Deserialize<T>(json, _jsonOpts) ?? new T();
        }

        private static void EnsureSuccess(HttpResponseMessage response, string operation)
        {
            if (response.IsSuccessStatusCode) return;

            throw response.StatusCode switch
            {
                HttpStatusCode.NotFound => new NotFoundException($"Course API: resource not found during '{operation}'."),
                HttpStatusCode.Unauthorized => new UnauthorizedException($"Course API: unauthorized during '{operation}'."),
                HttpStatusCode.Forbidden => new UnauthorizedException($"Course API: forbidden during '{operation}'."),
                HttpStatusCode.Conflict => new ConflictException($"Course API: conflict during '{operation}'."),
                _ => new CourseApiException(
                        $"Course API returned {(int)response.StatusCode} during '{operation}'.",
                        (int)response.StatusCode)
            };
        }

        // ─────────────────────────────────────────────────────────
        //  ICourseApiClient implementation
        // ─────────────────────────────────────────────────────────

        public async Task<List<CourseReadDto>> GetTeacherCoursesAsync(string instructorId)
        {
            // Course API route: GET api/CoursesApi/TeacherIndex   (requires Teacher role)
            try
            {
                var client = CreateClient();
                var response = await client.GetAsync("api/CoursesApi/TeacherIndex");
                EnsureSuccess(response, nameof(GetTeacherCoursesAsync));
                return await UnwrapAsync<List<CourseReadDto>>(response);
            }
            catch (Exception ex) when (ex is not CourseApiException
                                           and not NotFoundException
                                           and not UnauthorizedException
                                           and not ConflictException)
            {
                _logger.LogError(ex, "Network error fetching teacher courses for {InstructorId}", instructorId);
                throw new CourseApiException("Unable to reach Course API. Please try again later.");
            }
        }

        public async Task<CourseReadDto?> GetCourseByIdAsync(int courseId)
        {
            // Course API route: GET api/CoursesApi/{id}   (AllowAnonymous)
            try
            {
                var client = CreateClient();
                var response = await client.GetAsync($"api/CoursesApi/{courseId}");

                if (response.StatusCode == HttpStatusCode.NotFound) return null;

                EnsureSuccess(response, nameof(GetCourseByIdAsync));
                return await UnwrapAsync<CourseReadDto>(response);
            }
            catch (Exception ex) when (ex is not CourseApiException
                                           and not NotFoundException
                                           and not UnauthorizedException)
            {
                _logger.LogError(ex, "Network error fetching course {CourseId}", courseId);
                throw new CourseApiException("Unable to reach Course API. Please try again later.");
            }
        }

        public async Task<CourseReadDto> CreateCourseAsync(CourseWriteDto dto, string instructorId)
        {
            // Course API route: POST api/CoursesApi   (requires Teacher role)
            try
            {
                var client = CreateClient();
                var response = await client.PostAsJsonAsync("api/CoursesApi", dto);

                if (response.StatusCode == HttpStatusCode.Conflict)
                    throw new ConflictException($"A course titled '{dto.Title}' already exists for you.");

                EnsureSuccess(response, nameof(CreateCourseAsync));
                return await UnwrapAsync<CourseReadDto>(response);
            }
            catch (Exception ex) when (ex is not CourseApiException
                                           and not ConflictException
                                           and not UnauthorizedException)
            {
                _logger.LogError(ex, "Network error creating course for {InstructorId}", instructorId);
                throw new CourseApiException("Unable to reach Course API. Please try again later.");
            }
        }

        public async Task UpdateCourseAsync(int courseId, CourseWriteDto dto, string instructorId)
        {
            // Course API route: PUT api/CoursesApi/{id}   (requires Teacher role)
            try
            {
                var client = CreateClient();
                var response = await client.PutAsJsonAsync($"api/CoursesApi/{courseId}", dto);

                if (response.StatusCode == HttpStatusCode.NotFound)
                    throw new NotFoundException($"Course {courseId} not found.");

                if (response.StatusCode == HttpStatusCode.Forbidden)
                    throw new UnauthorizedException("You do not own this course.");

                EnsureSuccess(response, nameof(UpdateCourseAsync));
            }
            catch (Exception ex) when (ex is not CourseApiException
                                           and not NotFoundException
                                           and not UnauthorizedException)
            {
                _logger.LogError(ex, "Network error updating course {CourseId}", courseId);
                throw new CourseApiException("Unable to reach Course API. Please try again later.");
            }
        }

        public async Task<CourseReadDto> PublishCourseAsync(int courseId, string instructorId)
        {
            // Course API route: POST api/CoursesApi/{id}/publish   (requires Teacher role)
            try
            {
                var client = CreateClient();
                var response = await client.PostAsync($"api/CoursesApi/{courseId}/publish", null);

                if (response.StatusCode == HttpStatusCode.NotFound)
                    throw new NotFoundException($"Course {courseId} not found or not owned by you.");

                EnsureSuccess(response, nameof(PublishCourseAsync));
                return await UnwrapAsync<CourseReadDto>(response);
            }
            catch (Exception ex) when (ex is not CourseApiException
                                           and not NotFoundException
                                           and not UnauthorizedException)
            {
                _logger.LogError(ex, "Network error publishing course {CourseId}", courseId);
                throw new CourseApiException("Unable to reach Course API. Please try again later.");
            }
        }

        public async Task AddVideoAsync(int courseId, string videoUrl, string instructorId)
        {
            // Course API route: POST api/CoursesApi/{id}/videos   (requires Teacher role)
            try
            {
                var client = CreateClient();
                // Course API expects a raw JSON string, not an object
                var response = await client.PostAsJsonAsync($"api/CoursesApi/{courseId}/videos", videoUrl);

                if (response.StatusCode == HttpStatusCode.NotFound)
                    throw new NotFoundException($"Course {courseId} not found or not owned by you.");

                EnsureSuccess(response, nameof(AddVideoAsync));
            }
            catch (Exception ex) when (ex is not CourseApiException
                                           and not NotFoundException
                                           and not UnauthorizedException)
            {
                _logger.LogError(ex, "Network error adding video to course {CourseId}", courseId);
                throw new CourseApiException("Unable to reach Course API. Please try again later.");
            }
        }

        public async Task<List<EnrollmentReadDto>> GetCourseEnrollmentsAsync(int courseId)
        {
            // Course API route: GET api/EnrollmentsApi/Course/{courseId}   (AllowAnonymous)
            try
            {
                var client = CreateClient();
                var response = await client.GetAsync($"api/EnrollmentsApi/Course/{courseId}");

                if (response.StatusCode == HttpStatusCode.NotFound)
                    return new List<EnrollmentReadDto>();

                EnsureSuccess(response, nameof(GetCourseEnrollmentsAsync));
                return await UnwrapAsync<List<EnrollmentReadDto>>(response);
            }
            catch (Exception ex) when (ex is not CourseApiException
                                           and not NotFoundException
                                           and not UnauthorizedException)
            {
                _logger.LogError(ex, "Network error fetching enrollments for course {CourseId}", courseId);
                throw new CourseApiException("Unable to reach Course API. Please try again later.");
            }
        }

        public async Task<int> GetEnrollmentCountAsync(int courseId)
        {
            // Course API route: GET api/EnrollmentsApi/Course/{courseId}/count
            try
            {
                var client = CreateClient();
                var response = await client.GetAsync($"api/EnrollmentsApi/Course/{courseId}/count");

                if (!response.IsSuccessStatusCode)
                {
                    // Graceful fallback: fetch full list and count locally
                    var enrollments = await GetCourseEnrollmentsAsync(courseId);
                    return enrollments.Count;
                }

                // The count endpoint returns a plain int wrapped by ResponseWrapperFilter:
                // { "success": true, "data": 42 }
                var json = await response.Content.ReadAsStringAsync();
                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;

                if (root.TryGetProperty("data", out var dataEl) ||
                    root.TryGetProperty("Data", out dataEl))
                {
                    return dataEl.GetInt32();
                }

                return root.GetInt32(); // bare int fallback
            }
            catch (Exception ex) when (ex is not CourseApiException)
            {
                _logger.LogWarning(ex, "Could not get enrollment count for course {CourseId}, returning 0", courseId);
                return 0;
            }
        }
    }
}