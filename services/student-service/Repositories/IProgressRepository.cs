using LearningPlatform.StudentService.Models;
namespace LearningPlatform.StudentService.Repositories
{
    public interface IProgressRepository
    {
        Task<StudentProgress?> GetProgressAsync(string sid, int cid);
        Task AddAsync(StudentProgress p);
        Task UpdateAsync(StudentProgress p);
        Task<List<int>> GetTrendingCourseIdsAsync(int count);
        Task<List<StudentProgress>> GetProgressForCoursesAsync(string sid, List<int> courseIds);
        Task<List<StudentProgress>> GetRecentProgressAsync(string sid, int count);
    }
}
