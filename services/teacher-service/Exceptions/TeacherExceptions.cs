namespace LearningPlatform.TeacherService.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException(string message) : base(message) { }
    }

    public class UnauthorizedException : Exception
    {
        public UnauthorizedException(string message) : base(message) { }
    }

    public class ConflictException : Exception
    {
        public ConflictException(string message) : base(message) { }
    }

    public class CourseApiException : Exception
    {
        public int StatusCode { get; }

        public CourseApiException(string message, int statusCode = 502)
            : base(message)
        {
            StatusCode = statusCode;
        }
    }

    public class ChatApiException : Exception
    {
        public ChatApiException(string message) : base(message) { }
    }
}
