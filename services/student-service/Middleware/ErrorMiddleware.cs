using LearningPlatform.StudentService.DTOs;
using LearningPlatform.StudentService.Exceptions;
using LearningPlatform.StudentService.Common;
namespace LearningPlatform.StudentService.Middleware
{
    public class ErrorMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorMiddleware> _logger;

        public ErrorMiddleware(RequestDelegate next, ILogger<ErrorMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (BusinessException ex)
            {
                _logger.LogWarning(ex, "Business rule violation on {Method} {Path}", context.Request.Method, context.Request.Path);
                context.Response.StatusCode = ex.StatusCode;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(ApiResponseDto<object>.Fail(ex.Message));
            }
            catch (ResourceNotFoundException ex)
            {
                _logger.LogWarning(ex, "Resource not found on {Method} {Path}", context.Request.Method, context.Request.Path);
                context.Response.StatusCode = 404;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(ApiResponseDto<object>.Fail(ex.Message));
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning(ex, "Unauthorized access on {Method} {Path}", context.Request.Method, context.Request.Path);
                context.Response.StatusCode = 401;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(ApiResponseDto<object>.Fail("Unauthorized"));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception on {Method} {Path}", context.Request.Method, context.Request.Path);
                context.Response.StatusCode = 500;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(ApiResponseDto<object>.Fail("Something went wrong"));
            }
        }
    }
}
