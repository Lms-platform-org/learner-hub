using System;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;


namespace Horizon.MVC.Models
{
    public class Comment
    {
        public int Id { get; set; }
        
        public int CourseId { get; set; }
        public required string UserId { get; set; }
        
        public required string Content { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation properties
        [JsonIgnore]
        [ValidateNever]
        public Course? Course { get; set; }
    }
}

