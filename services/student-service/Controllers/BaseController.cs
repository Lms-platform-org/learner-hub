using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LearningPlatform.StudentService.Controllers
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        protected string GetUserId()
        {
            var userId = User.FindFirst("sub")?.Value
                      ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userId))
                throw new UnauthorizedAccessException("User not authenticated");

            return userId;
        }

        protected string GetEmail() =>
            User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
            ?? User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress")?.Value
            ?? string.Empty;

        protected string GetRole() =>
            User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value
            ?? User.FindFirst("http://schemas.microsoft.com/ws/2008/06/identity/claims/role")?.Value
            ?? string.Empty;
    }
}