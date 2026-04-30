using LearningPlatformAuth.Data;
using LearningPlatformAuth.Models;
using Microsoft.AspNetCore.Identity;

namespace LearningPlatformAuth.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthRepository(UserManager<ApplicationUser> userManager,
                              RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<ApplicationUser?> CreateUserAsync(RegisterRequest model)
        {
            if (!await _roleManager.RoleExistsAsync(model.Role))
                await _roleManager.CreateAsync(new IdentityRole(model.Role));

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                DisplayName = model.DisplayName,
                IsApproved = model.Role == "Student"
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded) return null;

            await _userManager.AddToRoleAsync(user, model.Role);

            return user;
        }

        public async Task<ApplicationUser?> ValidateUserAsync(LoginRequest model)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null) return null;

            var valid = await _userManager.CheckPasswordAsync(user, model.Password);

            return valid ? user : null;
        }

        public async Task<IList<string>> GetRolesAsync(ApplicationUser user)
        {
            return await _userManager.GetRolesAsync(user);
        }

        public async Task<List<object>> GetPendingTeachersAsync()
        {
            var users = await _userManager.GetUsersInRoleAsync("Teacher");

            return users
                .Where(u => !u.IsApproved && !u.ApprovalDate.HasValue)
                .Select(u => new
                {
                    u.Id,
                    u.Email,
                    u.DisplayName
                }).Cast<object>().ToList();
        }

        public async Task<object> UpdateTeacherStatus(string userId, bool isApproved, string? notes)
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
                return new { error = "User not found" };

            user.IsApproved = isApproved;
            user.ApprovalDate = DateTime.UtcNow;
            user.ApprovalNotes = notes;

            await _userManager.UpdateAsync(user);

            return new
            {
                success = true,
                message = isApproved ? "Approved" : "Rejected"
            };
        }

        public async Task<ApplicationUser?> GetUserByIdAsync(string userId)
        {
            return await _userManager.FindByIdAsync(userId);
        }

        public async Task<ApplicationUser?> GetUserByEmailAsync(string email)
        {
            return await _userManager.FindByEmailAsync(email);
        }

        public async Task<List<ApplicationUser>> GetAllUsersAsync()
        {
            return _userManager.Users.ToList();
        }

        public async Task<List<ApplicationUser>> GetUsersByRoleAsync(string role)
        {
            var users = await _userManager.GetUsersInRoleAsync(role);
            return users.ToList();
        }

        public async Task<List<ApplicationUser>> GetApprovedUsersAsync()
        {
            return _userManager.Users
                .Where(u => u.IsApproved)
                .ToList();
        }
    }
}
