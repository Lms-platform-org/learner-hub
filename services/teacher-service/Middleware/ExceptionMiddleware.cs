using System.Net;
using System.Text.Json;
using LearningPlatform.TeacherService.Common;
using LearningPlatform.TeacherService.Exceptions;

namespace LearningPlatform.TeacherService.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next   = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception in Teacher API");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var (statusCode, message) = exception switch
            {
                NotFoundException        => (HttpStatusCode.NotFound,          exception.Message),
                UnauthorizedException    => (HttpStatusCode.Unauthorized,      exception.Message),
                UnauthorizedAccessException => (HttpStatusCode.Unauthorized,   exception.Message),
                ConflictException        => (HttpStatusCode.Conflict,          exception.Message),
                ArgumentException        => (HttpStatusCode.BadRequest,        exception.Message),
                CourseApiException cae   => ((HttpStatusCode)cae.StatusCode,   exception.Message),
                _                        => (HttpStatusCode.InternalServerError,
                                             "An unexpected error occurred. Please try again later.")
            };

            context.Response.StatusCode = (int)statusCode;

            var response = ApiResponseDto<object>.Fail(message);

            return context.Response.WriteAsync(
                JsonSerializer.Serialize(response,
                    new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
        }
    }
}
