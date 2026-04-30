using System.Security.Claims;

namespace Courses.Api.Services
{
    public interface IUserContext
    {
        string? UserId { get; }
        string? UserRole { get; }
        bool IsAuthenticated { get; }
    }
}
