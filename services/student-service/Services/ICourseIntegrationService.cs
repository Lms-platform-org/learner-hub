using LearningPlatform.StudentService.DTOs;

namespace LearningPlatform.StudentService.Services
{
    public interface ICourseIntegrationService
    {
        Task<int> GetTotalLessonsForCourseAsync(int courseId);
        Task<PagedResponseDto<CourseDto>> GetAllCoursesAsync(int page, int pageSize);
        Task<PagedResponseDto<CourseDto>> SearchCoursesAsync(string query, int page, int pageSize);
        Task<CourseDto?> GetCourseByIdAsync(int courseId);
        Task NotifyEnrollmentAsync(string studentId, int courseId);
        Task NotifyProgressAsync(string studentId, int courseId, int percentage);
    }
}
