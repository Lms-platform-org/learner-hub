using LearningPlatformAuth.Models;

namespace LearningPlatformAuth.Services
{
    public interface IAuthService
    {
        Task<object> RegisterAsync(RegisterRequest model);
        Task<AuthResponse?> LoginAsync(LoginRequest model);
        Task<List<object>> GetPendingTeachersAsync();
        Task<object> ApproveTeacherAsync(string userId, ApprovalRequest request);
        Task<object> ApproveTeacherAsync(string userId, ApprovalRequest request, string adminId);
        Task<object> RejectTeacherAsync(string userId, ApprovalRequest request);
        Task<object> RejectTeacherAsync(string userId, ApprovalRequest request, string adminId);

        /// <summary>
        /// Get user details by user ID
        /// </summary>
        Task<object?> GetUserByIdAsync(string userId);

        /// <summary>
        /// Get all users with a specific role
        /// </summary>
        Task<List<object>> GetUsersByRoleAsync(string role);

        /// <summary>
        /// Get user details by email
        /// </summary>
        Task<object?> GetUserByEmailAsync(string email);

        /// <summary>
        /// Get all approved users
        /// </summary>
        Task<List<object>> GetApprovedUsersAsync();
    }
}

