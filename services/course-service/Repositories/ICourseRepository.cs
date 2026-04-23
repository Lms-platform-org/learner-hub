using Courses.Models;

namespace Courses.Api.Repositories
{
    public interface ICourseRepository
    {
        Task<IEnumerable<Course>> GetAllPublishedAsync();
        Task<IEnumerable<Course>> GetByTeacherIdAsync(string teacherId);
        Task<Course?> GetByIdAsync(int id);
        Task AddAsync(Course course);
        Task UpdateAsync(Course course);
        Task AddVideoAsync(CourseVideo video);
        Task<bool> ExistsAsync(int id);
        Task SaveChangesAsync();
    }
}
