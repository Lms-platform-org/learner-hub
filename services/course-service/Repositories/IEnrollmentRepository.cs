using Courses.Models;

namespace Courses.Api.Repositories
{
    public interface IEnrollmentRepository
    {
        Task<Enrollment?> GetByStudentAndCourseAsync(string studentId, int courseId);
        Task AddAsync(Enrollment enrollment);
        Task SaveChangesAsync();
        Task<IEnumerable<Enrollment>> GetByCourseIdAsync(int courseId);
        Task<int> GetCountByCourseIdAsync(int courseId);
        Task<IEnumerable<Enrollment>> GetByStudentIdAsync(string studentId);
        Task<IEnumerable<Enrollment>> GetAllAsync();
    }
}
