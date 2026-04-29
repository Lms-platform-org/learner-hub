using LearningPlatformAuth.Models;
using LearningPlatformAuth.Repositories;
using LearningPlatformAuth.Responses;
using LearningPlatformAuth.Exceptions;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace LearningPlatformAuth.Services
{
    public class AuthService : IAuthService
    {
        private readonly IAuthRepository _repo;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly ILogger<AuthService> _logger;

        public AuthService(IAuthRepository repo, ITokenService tokenService, IMapper mapper, ILogger<AuthService> logger)
        {
            _repo = repo;
            _tokenService = tokenService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<object> RegisterAsync(RegisterRequest model)
        {
            ArgumentNullException.ThrowIfNull(model);

            if (model.Role != "Student" && model.Role != "Teacher")
            {
                _logger.LogError("Invalid role provided during registration: {Role} for email: {Email}", model.Role, model.Email);
                throw new BadRequestException("Invalid role");
            }

            _logger.LogInformation("Creating new user - Email: {Email}, Role: {Role}", model.Email, model.Role);
            var user = await _repo.CreateUserAsync(model);

            if (user == null)
            {
                _logger.LogError("User creation failed in repository for email: {Email}", model.Email);
                throw new BadRequestException("User creation failed");
            }

            var message = model.Role == "Teacher"
                ? "Wait for admin approval"
                : "You can login now";

            var data = new
            {
                email = user.Email,
                userId = user.Id,
                name = user.DisplayName ?? "",
                role = model.Role
            };

            _logger.LogInformation("User created successfully - UserId: {UserId}, Email: {Email}, Role: {Role}", user.Id, user.Email, model.Role);
            return new ApiResponse<object>(true, message, data);
        }

        public async Task<AuthResponse?> LoginAsync(LoginRequest model)
        {
            ArgumentNullException.ThrowIfNull(model);

            _logger.LogInformation("Validating user credentials for email: {Email}", model.Email);
            var user = await _repo.ValidateUserAsync(model);

            if (user == null)
            {
                _logger.LogWarning("User validation failed - Email: {Email} not found or invalid password", model.Email);
                return null;
            }

            var roles = await _repo.GetRolesAsync(user);
            _logger.LogInformation("User retrieved with roles: {Roles} for email: {Email}", string.Join(", ", roles), model.Email);

            if (roles.Contains("Teacher") && !user.IsApproved)
            {
                if (user.ApprovalDate.HasValue)
                {
                    _logger.LogWarning("Teacher login rejected - ApprovalDate: {ApprovalDate}, Notes: {Notes} for email: {Email}", user.ApprovalDate, user.ApprovalNotes, model.Email);
                    var rejectedResponse = new AuthResponse
                    {
                        Token = "TEACHER_REJECTED",
                        Message = user.ApprovalNotes ?? "Rejected"
                    };
                    return rejectedResponse;
                }

                _logger.LogWarning("Teacher login pending approval for email: {Email}", model.Email);
                var pendingResponse = new AuthResponse
                {
                    Token = "PENDING_APPROVAL",
                    Message = "Pending approval"
                };
                return pendingResponse;
            }

            var token = await _tokenService.GenerateToken(user);
            var response = _mapper.Map<AuthResponse>(user);
            response.Token = token;
            response.Roles = roles.ToArray();

            _logger.LogInformation("Token generated successfully for user: {UserId}, Email: {Email}", user.Id, user.Email);
            return response;
        }

        public async Task<List<object>> GetPendingTeachersAsync()
        {
            _logger.LogInformation("Fetching pending teachers from repository");
            var result = await _repo.GetPendingTeachersAsync();
            _logger.LogInformation("Retrieved {Count} pending teachers", result.Count);
            return result;
        }

        public async Task<object> ApproveTeacherAsync(string userId, ApprovalRequest request)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                _logger.LogError("ApproveTeacherAsync called with empty userId");
                throw new ArgumentException("User ID cannot be empty", nameof(userId));
            }

            _logger.LogInformation("Approving teacher - UserId: {UserId}, Notes: {Notes}", userId, request?.Notes);
            var result = await _repo.UpdateTeacherStatus(userId, true, request?.Notes);
            _logger.LogInformation("Teacher approved successfully - UserId: {UserId}", userId);
            return result;
        }

        public async Task<object> ApproveTeacherAsync(string userId, ApprovalRequest request, string adminId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                _logger.LogError("ApproveTeacherAsync called with empty userId");
                throw new ArgumentException("User ID cannot be empty", nameof(userId));
            }

            _logger.LogInformation("Teacher approval initiated - TeacherId: {TeacherId}, AdminId: {AdminId}, Notes: {Notes}", userId, adminId, request?.Notes);
            var result = await _repo.UpdateTeacherStatus(userId, true, request?.Notes);
            _logger.LogInformation("Teacher approved successfully - TeacherId: {TeacherId}, ApprovedBy: {AdminId}", userId, adminId);
            return result;
        }

        public async Task<object> RejectTeacherAsync(string userId, ApprovalRequest request)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                _logger.LogError("RejectTeacherAsync called with empty userId");
                throw new ArgumentException("User ID cannot be empty", nameof(userId));
            }

            _logger.LogInformation("Rejecting teacher - UserId: {UserId}, Reason: {Reason}", userId, request?.Notes);
            var result = await _repo.UpdateTeacherStatus(userId, false, request?.Notes);
            _logger.LogInformation("Teacher rejected successfully - UserId: {UserId}", userId);
            return result;
        }

        public async Task<object> RejectTeacherAsync(string userId, ApprovalRequest request, string adminId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                _logger.LogError("RejectTeacherAsync called with empty userId");
                throw new ArgumentException("User ID cannot be empty", nameof(userId));
            }

            _logger.LogInformation("Teacher rejection initiated - TeacherId: {TeacherId}, AdminId: {AdminId}, Reason: {Reason}", userId, adminId, request?.Notes);
            var result = await _repo.UpdateTeacherStatus(userId, false, request?.Notes);
            _logger.LogInformation("Teacher rejected successfully - TeacherId: {TeacherId}, RejectedBy: {AdminId}", userId, adminId);
            return result;
        }

        public async Task<object?> GetUserByIdAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                _logger.LogWarning("GetUserByIdAsync called with empty userId");
                return null;
            }

            _logger.LogInformation("Fetching user by ID: {UserId}", userId);
            var user = await _repo.GetUserByIdAsync(userId);

            if (user == null)
            {
                _logger.LogWarning("User not found - UserId: {UserId}", userId);
                return null;
            }

            var roles = await _repo.GetRolesAsync(user);
            var userData = new
            {
                userId = user.Id,
                email = user.Email,
                displayName = user.DisplayName ?? "",
                roles = roles,
                isApproved = user.IsApproved,
                approvalDate = user.ApprovalDate
            };

            _logger.LogInformation("User retrieved successfully - UserId: {UserId}", userId);
            return userData;
        }

        public async Task<List<object>> GetUsersByRoleAsync(string role)
        {
            if (string.IsNullOrWhiteSpace(role))
            {
                _logger.LogWarning("GetUsersByRoleAsync called with empty role");
                return new List<object>();
            }

            _logger.LogInformation("Fetching users by role: {Role}", role);
            var users = await _repo.GetUsersByRoleAsync(role);

            var userDetails = new List<object>();
            foreach (var user in users)
            {
                var userRoles = await _repo.GetRolesAsync(user);
                userDetails.Add(new
                {
                    userId = user.Id,
                    email = user.Email,
                    displayName = user.DisplayName ?? "",
                    roles = userRoles,
                    isApproved = user.IsApproved,
                    approvalDate = user.ApprovalDate
                });
            }

            _logger.LogInformation("Retrieved {Count} users with role: {Role}", users.Count, role);
            return userDetails;
        }

        public async Task<object?> GetUserByEmailAsync(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                _logger.LogWarning("GetUserByEmailAsync called with empty email");
                return null;
            }

            _logger.LogInformation("Fetching user by email: {Email}", email);
            var user = await _repo.GetUserByEmailAsync(email);

            if (user == null)
            {
                _logger.LogWarning("User not found - Email: {Email}", email);
                return null;
            }

            var roles = await _repo.GetRolesAsync(user);
            var userData = new
            {
                userId = user.Id,
                email = user.Email,
                displayName = user.DisplayName ?? "",
                roles = roles,
                isApproved = user.IsApproved,
                approvalDate = user.ApprovalDate
            };

            _logger.LogInformation("User retrieved successfully - Email: {Email}", email);
            return userData;
        }

        public async Task<List<object>> GetApprovedUsersAsync()
        {
            _logger.LogInformation("Fetching all approved users");
            var users = await _repo.GetApprovedUsersAsync();

            var userDetails = new List<object>();
            foreach (var user in users)
            {
                var userRoles = await _repo.GetRolesAsync(user);
                userDetails.Add(new
                {
                    userId = user.Id,
                    email = user.Email,
                    displayName = user.DisplayName ?? "",
                    roles = userRoles,
                    role = userRoles.FirstOrDefault() ?? "Student",
                    isApproved = user.IsApproved,
                    approvalDate = user.ApprovalDate
                });
            }

            _logger.LogInformation("Retrieved {Count} approved users", users.Count);
            return userDetails;
        }
    }
}
