using System.Net;
using System.Text.Json;
using WebApplication1.Exceptions;
using Serilog;

namespace WebApplication1.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;

        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Unhandled exception occurred at {Path}", context.Request.Path);
                await HandleException(context, ex);
            }
        }

        private static Task HandleException(HttpContext context, Exception ex)
        {
            HttpStatusCode status = HttpStatusCode.InternalServerError;

            if (ex is NotFoundException)
                status = HttpStatusCode.NotFound;

            else if (ex is BadRequestException)
                status = HttpStatusCode.BadRequest;

            var response = new
            {
                error = ex.Message
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)status;

            return context.Response.WriteAsync(JsonSerializer.Serialize(response));
        }
    }
}