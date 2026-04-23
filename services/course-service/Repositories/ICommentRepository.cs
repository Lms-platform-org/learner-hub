using Courses.Models;

namespace Courses.Api.Repositories
{
    public interface ICommentRepository
    {
        Task<IEnumerable<Comment>> GetByCourseIdAsync(int courseId);
        Task AddAsync(Comment comment);
        Task SaveChangesAsync();
    }
}
