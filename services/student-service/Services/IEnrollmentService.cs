using LearningPlatform.StudentService.Models;

namespace LearningPlatform.StudentService.Services
{
    public interface IEnrollmentService
    {
        Task EnrollAsync(string studentId, int courseId);
        Task<List<Enrollment>> GetEnrolledCoursesAsync(string studentId);
        Task<bool> IsEnrolledAsync(string studentId, int courseId);
    }
}
