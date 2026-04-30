namespace LearningPlatform.StudentService.Exceptions
{
    public class ResourceNotFoundException : Exception
    {
        public int StatusCode { get; set; } = 404;

        public ResourceNotFoundException(string message) : base(message) { }

        public ResourceNotFoundException(string resourceName, int id) 
            : base($"{resourceName} with ID {id} not found") { }
    }
}
