using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Courses.Api.Models.Common;

namespace Courses.Api.Filters
{
    public class ResponseWrapperFilter : IResultFilter
    {
        public void OnResultExecuting(ResultExecutingContext context)
        {
            if (context.Result is ObjectResult objectResult)
            {
                // Check if already wrapped
                var value = objectResult.Value;
                if (value == null) return;

                var type = value.GetType();
                
                // Don't wrap if it's already an ApiResponse
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ApiResponse<>))
                {
                    return;
                }

                var statusCode = objectResult.StatusCode ?? 200;
                var success = statusCode >= 200 && statusCode < 300;

                // Try to extract a more descriptive message if available
                string message = success ? "Success" : "Failed";
                if (value != null)
                {
                    // Check if value is an anonymous object with a Message property
                    var messageProp = value.GetType().GetProperty("Message");
                    if (messageProp != null)
                    {
                        message = messageProp.GetValue(value)?.ToString() ?? message;
                    }
                }

                // Create the wrapped object
                var wrappedResponse = new ApiResponse<object>
                {
                    Success = success,
                    Message = message,
                    Data = success ? value : null // Don't hide the message object inside data if it's an error
                };

                objectResult.Value = wrappedResponse;
            }
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            // Nothing to do here
        }
    }
}
