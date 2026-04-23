using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
namespace Courses.Models
{
    public class User
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string Role { get; set; } // either teacher or student is talking
        public string Password { get; set; } = string.Empty;
    }
}
