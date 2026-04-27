using LearningPlatform.StudentService.Models;
namespace LearningPlatform.StudentService.Repositories
{
    public interface IProfileRepository
    {
        Task<StudentProfile?> GetByStudentIdAsync(string sid);
        Task UpsertAsync(StudentProfile p);
    }
}
