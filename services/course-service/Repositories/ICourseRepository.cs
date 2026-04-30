using Courses.Models;

namespace Courses.Api.Repositories
{
    public interface ICourseRepository
    {
        Task<IEnumerable<Course>> GetAllPublishedAsync();
        Task<IEnumerable<Course>> GetByInstructorIdAsync(string instructorId);
        Task<Course?> GetByIdAsync(int id);
        Task AddAsync(Course course);
        Task UpdateAsync(Course course);
        Task AddVideoAsync(CourseVideo video);
        Task<bool> ExistsAsync(int id);
        Task SaveChangesAsync();
        Task<IEnumerable<Course>> GetAllAsync();
        Task<bool> ExistsByTitleAndInstructorAsync(string title, string instructorId);
    }
}
