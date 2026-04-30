using System.Security.Claims;

namespace Courses.Api.Middlewares
{
    public class MockAuthMiddleware
    {
        private readonly RequestDelegate _next;

        public MockAuthMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Only apply mock auth if no real token is provided
            if (!context.User.Identity?.IsAuthenticated ?? true)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, "jayraj"),
                    new Claim(ClaimTypes.Email, "jayraj@gmail.com"),
                    new Claim(ClaimTypes.NameIdentifier, "jayraj"),
                    new Claim(ClaimTypes.Role, "Teacher"),
                    new Claim(ClaimTypes.Role, "Student"),
                    new Claim(ClaimTypes.Role, "Admin"),
                    new Claim("InstructorId", "jayraj"),
                    new Claim("StudentId", "jayraj")
                };

                var identity = new ClaimsIdentity(claims, "MockAuth");
                context.User = new ClaimsPrincipal(identity);
            }

            await _next(context);
        }
    }
}
