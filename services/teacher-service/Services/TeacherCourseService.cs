using LearningPlatform.TeacherService.DTOs;
using LearningPlatform.TeacherService.Exceptions;

namespace LearningPlatform.TeacherService.Services
{
    /// <summary>
    /// Orchestrates course management operations for teachers.
    /// All Course API calls go through <see cref="ICourseApiClient"/>; this class
    /// owns any business rules that live at the Teacher API boundary (e.g. input
    /// coercion, logging) without duplicating Course API validation logic.
    /// </summary>
    public class TeacherCourseService : ITeacherCourseService
    {
        private readonly ICourseApiClient _courseApiClient;
        private readonly ILogger<TeacherCourseService> _logger;

        public TeacherCourseService(
            ICourseApiClient courseApiClient,
            ILogger<TeacherCourseService> logger)
        {
            _courseApiClient = courseApiClient;
            _logger          = logger;
        }

        public async Task<List<CourseReadDto>> GetMyCoursesAsync(string teacherId)
        {
            _logger.LogInformation("Fetching courses for teacher {TeacherId}", teacherId);
            return await _courseApiClient.GetTeacherCoursesAsync(teacherId);
        }

        public async Task<CourseReadDto?> GetCourseAsync(int courseId)
        {
            _logger.LogInformation("Fetching course {CourseId}", courseId);
            return await _courseApiClient.GetCourseByIdAsync(courseId);
        }

        public async Task<CourseReadDto> CreateCourseAsync(CourseWriteDto dto, string teacherId)
        {
            _logger.LogInformation("Teacher {TeacherId} creating course '{Title}'", teacherId, dto.Title);
            var created = await _courseApiClient.CreateCourseAsync(dto, teacherId);
            _logger.LogInformation("Course {CourseId} created for teacher {TeacherId}", created.Id, teacherId);
            return created;
        }

        public async Task UpdateCourseAsync(int courseId, CourseWriteDto dto, string teacherId)
        {
            _logger.LogInformation("Teacher {TeacherId} updating course {CourseId}", teacherId, courseId);
            await _courseApiClient.UpdateCourseAsync(courseId, dto, teacherId);
            _logger.LogInformation("Course {CourseId} updated", courseId);
        }

        public async Task<CourseReadDto> PublishCourseAsync(int courseId, string teacherId)
        {
            _logger.LogInformation("Teacher {TeacherId} publishing course {CourseId}", teacherId, courseId);
            var published = await _courseApiClient.PublishCourseAsync(courseId, teacherId);
            _logger.LogInformation("Course {CourseId} published", courseId);
            return published;
        }

        public async Task AddVideoAsync(int courseId, string videoUrl, string teacherId)
        {
            // Validate URL shape before hitting the Course API to return a clear 400
            if (!Uri.TryCreate(videoUrl, UriKind.Absolute, out var uri)
                || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            {
                throw new ArgumentException("Video URL must be a valid http/https URL.");
            }

            _logger.LogInformation("Teacher {TeacherId} adding video to course {CourseId}", teacherId, courseId);
            await _courseApiClient.AddVideoAsync(courseId, videoUrl, teacherId);
        }
    }
}
