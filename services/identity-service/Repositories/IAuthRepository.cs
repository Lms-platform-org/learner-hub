using LearningPlatformAuth.Data;
using LearningPlatformAuth.Models;

namespace LearningPlatformAuth.Repositories
{
    public interface IAuthRepository
    {
        Task<ApplicationUser?> CreateUserAsync(RegisterRequest model);
        Task<ApplicationUser?> ValidateUserAsync(LoginRequest model);
        Task<IList<string>> GetRolesAsync(ApplicationUser user);
        Task<List<object>> GetPendingTeachersAsync();
        Task<object> UpdateTeacherStatus(string userId, bool isApproved, string? notes);
        Task<ApplicationUser?> GetUserByIdAsync(string userId);
        Task<ApplicationUser?> GetUserByEmailAsync(string email);
        Task<List<ApplicationUser>> GetAllUsersAsync();
        Task<List<ApplicationUser>> GetUsersByRoleAsync(string role);
        Task<List<ApplicationUser>> GetApprovedUsersAsync();
    }
}
