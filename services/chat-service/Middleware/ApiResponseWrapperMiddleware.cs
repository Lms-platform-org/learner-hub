using System.Text.Json;
using WebApplication1.Common;

namespace WebApplication1.Middleware
{
    public class ApiResponseWrapperMiddleware
    {
        private readonly RequestDelegate _next;

        public ApiResponseWrapperMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            var originalBodyStream = context.Response.Body;

            using var responseBody = new MemoryStream();
            context.Response.Body = responseBody;

            await _next(context);

            responseBody.Seek(0, SeekOrigin.Begin);
            var responseText = await new StreamReader(responseBody).ReadToEndAsync();

            // ONLY wrap controller api responses
            if (!context.Request.Path.StartsWithSegments("/api"))
            {
                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
                context.Response.Body = originalBodyStream;
                return;
            }

            // skip empty responses
            if (string.IsNullOrWhiteSpace(responseText))
            {
                responseBody.Seek(0, SeekOrigin.Begin);
                await responseBody.CopyToAsync(originalBodyStream);
                context.Response.Body = originalBodyStream;
                return;
            }

            // prevent double wrapping
            try
            {
                using var document = JsonDocument.Parse(responseText);
                var root = document.RootElement;

                if (root.TryGetProperty("success", out _) &&
                    root.TryGetProperty("message", out _) &&
                    root.TryGetProperty("data", out _) &&
                    root.TryGetProperty("errors", out _))
                {
                    context.Response.Body = originalBodyStream;
                    context.Response.ContentType = "application/json";
                    await context.Response.WriteAsync(responseText);
                    return;
                }
            }
            catch
            {
                // if parsing fails continue wrapping
            }

            object responseData;

            try
            {
                responseData = JsonSerializer.Deserialize<object>(responseText,
                    new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
            }
            catch
            {
                responseData = responseText;
            }

            bool success = context.Response.StatusCode >= 200 && context.Response.StatusCode < 300;
            string message = success ? "Request successful" : "Request failed";

            var wrappedResponse = new ApiResponse<object>(
                success,
                message,
                success ? responseData : null,
                success ? null : responseData
            );

            var jsonResponse = JsonSerializer.Serialize(wrappedResponse);

            context.Response.Body = originalBodyStream;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(jsonResponse);
        }
    }
}