namespace WebApplication1.Models
{
    public class User
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; //either teacher or student is talking
    }
}
