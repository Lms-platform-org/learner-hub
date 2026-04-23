using Courses.Models;

namespace Courses.Api.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByIdAsync(string id);
        Task<bool> ExistsAsync(string id);
    }
}
