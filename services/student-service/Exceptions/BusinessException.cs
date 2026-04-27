namespace LearningPlatform.StudentService.Exceptions
{
    public class BusinessException : Exception
    {
        public int StatusCode { get; set; } = 400;

        public BusinessException(string message) : base(message) { }

        public BusinessException(string message, int statusCode) : base(message)
        {
            StatusCode = statusCode;
        }
    }
}
