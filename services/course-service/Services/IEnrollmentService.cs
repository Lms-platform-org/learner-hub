using Courses.Models;

namespace Courses.Api.Services
{
    public interface IEnrollmentService
    {
        Task<Enrollment?> EnrollStudentAsync(int courseId, string studentId);
        Task<IEnumerable<Enrollment>> GetCourseEnrollmentsAsync(int courseId);
        Task<int> GetEnrollmentCountByCourseIdAsync(int courseId);
        Task<IEnumerable<Course>> GetStudentCoursesAsync(string studentId);
        Task<IEnumerable<Enrollment>> GetAllEnrollmentsAsync();
    }
}
