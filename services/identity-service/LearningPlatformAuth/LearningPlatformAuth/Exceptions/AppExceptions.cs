namespace LearningPlatformAuth.Exceptions
{
    
    public abstract class AppException : Exception
    {
        public int StatusCode { get; }

        protected AppException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }

    public class BadRequestException : AppException
    {
        public BadRequestException(string message) : base(message, 400) { }
    }

    public class NotFoundException : AppException
    {
        public NotFoundException(string message) : base(message, 404) { }
    }

    public class ConflictException : AppException
    {
        public ConflictException(string message) : base(message, 409) { }
    }

    public class ValidationException : AppException
    {
        public IEnumerable<string> ValidationErrors { get; }

        public ValidationException(IEnumerable<string> errors)
            : base("Validation failed", 400)
        {
            ValidationErrors = errors;
        }
    }
}
