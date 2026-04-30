using LearningPlatform.StudentService.Models;

namespace LearningPlatform.StudentService.Repositories
{
    public interface IEnrollmentRepository
    {
        Task<Enrollment?> GetAsync(string studentId, int courseId);
        Task<List<Enrollment>> GetByStudentIdAsync(string studentId);
        Task AddAsync(Enrollment enrollment);
        Task<bool> IsEnrolledAsync(string studentId, int courseId);
    }
}
