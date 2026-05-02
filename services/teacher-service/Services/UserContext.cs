using System.Security.Claims;

namespace LearningPlatform.TeacherService.Services
{
    public interface IUserContext
    {
        string? UserId        { get; }
        string? UserRole      { get; }
        bool    IsAuthenticated { get; }
    }

    public class UserContext : IUserContext
    {
        private readonly IHttpContextAccessor _accessor;

        public UserContext(IHttpContextAccessor accessor)
        {
            _accessor = accessor;
        }

        // Checks both "sub" (JWT standard) and NameIdentifier (ASP.NET Core convention)
        public string? UserId =>
            _accessor.HttpContext?.User?.FindFirst("sub")?.Value
            ?? _accessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        public string? UserRole =>
            _accessor.HttpContext?.User?.FindFirst(ClaimTypes.Role)?.Value;

        public bool IsAuthenticated =>
            _accessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
    }
}
