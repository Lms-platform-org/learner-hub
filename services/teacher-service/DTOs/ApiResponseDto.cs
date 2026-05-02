namespace LearningPlatform.TeacherService.Common
{
    /// <summary>
    /// Unified API envelope used by every endpoint in this service.
    /// Shape is intentionally identical to StudentService.Common.ApiResponseDto
    /// so the MVC layer can deserialise responses from both services the same way.
    /// </summary>
    public class ApiResponseDto<T>
    {
        public bool   Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T?     Data    { get; set; }

        // Factory helpers keep construction off the controllers
        public static ApiResponseDto<T> Ok(T data, string message = "Success")
            => new() { Success = true, Data = data, Message = message };

        public static ApiResponseDto<T> Fail(string message)
            => new() { Success = false, Message = message };
    }
}
