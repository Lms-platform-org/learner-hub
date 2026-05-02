using LearningPlatform.TeacherService.DTOs;

namespace LearningPlatform.TeacherService.Services
{
    /// <summary>
    /// Abstracts every HTTP call the Teacher Service makes to the Course API.
    /// Keeping this behind an interface lets us mock it in unit tests and swap
    /// the transport without touching business logic.
    /// </summary>
    public interface ICourseApiClient
    {
        /// <summary>Fetch all courses that belong to <paramref name="instructorId"/>.</summary>
        Task<List<CourseReadDto>> GetTeacherCoursesAsync(string instructorId);

        /// <summary>Fetch a single course by id. Returns null when not found.</summary>
        Task<CourseReadDto?> GetCourseByIdAsync(int courseId);

        /// <summary>Create a new course on behalf of the authenticated teacher.</summary>
        Task<CourseReadDto> CreateCourseAsync(CourseWriteDto dto, string instructorId);

        /// <summary>Update an existing course. Throws <see cref="Exceptions.NotFoundException"/> when the course does not exist.</summary>
        Task UpdateCourseAsync(int courseId, CourseWriteDto dto, string instructorId);

        /// <summary>Publish a draft course.</summary>
        Task<CourseReadDto> PublishCourseAsync(int courseId, string instructorId);

        /// <summary>Add a video URL to an existing course.</summary>
        Task AddVideoAsync(int courseId, string videoUrl, string instructorId);

        /// <summary>
        /// Get all enrollment records for a specific course.
        /// Used for per-course student-count aggregation.
        /// </summary>
        Task<List<EnrollmentReadDto>> GetCourseEnrollmentsAsync(int courseId);

        /// <summary>
        /// Get only the enrollment count for a course (lightweight endpoint).
        /// Falls back to GetCourseEnrollmentsAsync count if the endpoint is absent.
        /// </summary>
        Task<int> GetEnrollmentCountAsync(int courseId);
    }
}