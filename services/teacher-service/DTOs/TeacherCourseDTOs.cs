using System;
using System.ComponentModel.DataAnnotations;
using Courses.Models;

namespace TeacherDashboardApi.DTOs
{
    public class CourseCreateDTO
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string Category { get; set; } = string.Empty;

        [Required]
        public string Duration { get; set; } = string.Empty;

        public string? ThumbnailUrl { get; set; }
        
        public DateTime StartDate { get; set; } = DateTime.UtcNow;
    }

    public class CourseUpdateDTO
    {
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;

        [Required]
        public string Description { get; set; } = string.Empty;

        [Required]
        public decimal Price { get; set; }

        [Required]
        public string Category { get; set; } = string.Empty;

        [Required]
        public string Duration { get; set; } = string.Empty;

        public string? ThumbnailUrl { get; set; }
        
        public CourseStatus Status { get; set; }
    }

    public class StudentEnrollmentDTO
    {
        public string StudentId { get; set; } = string.Empty;
        public string StudentName { get; set; } = string.Empty;
        public DateTime EnrolledAt { get; set; }
        public int CourseId { get; set; }
        public string CourseTitle { get; set; } = string.Empty;
    }

    public class InstructorStatsDTO
    {
        public int TotalStudents { get; set; }
        public double CourseRating { get; set; }
        public decimal TotalRevenue { get; set; }
        public int ActiveCourses { get; set; }
        public double StudentGrowthPercentage { get; set; }
        public int PendingReviews { get; set; }
    }

    public class CourseRevenuePointDTO
    {
        public string CourseTitle { get; set; } = string.Empty;
        public decimal Revenue { get; set; }
    }

    public class CourseRevenueDataDTO
    {
        public List<CourseRevenuePointDTO> Data { get; set; } = new();
    }

    public class EnrollmentGrowthPointDTO
    {
        public string Label { get; set; } = string.Empty;
        public int Value { get; set; }
    }

    public class EnrollmentGrowthDTO
    {
        public string Period { get; set; } = "Weekly";
        public List<EnrollmentGrowthPointDTO> Data { get; set; } = new();
    }

    public class SubjectDistributionDTO
    {
        public string SubjectName { get; set; } = string.Empty;
        public int CourseCount { get; set; }
        public double Percentage { get; set; }
    }

    public class TopEnrolledCourseDTO
    {
        public string CourseTitle { get; set; } = string.Empty;
        public int EnrollmentCount { get; set; }
        public double EnrollmentPercentage { get; set; }
    }

    public class TeacherCourseListItemDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int StudentCount { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class TeacherProfileDTO
    {
        [Required]
        public string FullName { get; set; } = string.Empty;

        public string Bio { get; set; } = string.Empty;

        public List<string> Skills { get; set; } = new();

        public string PreferredLevel { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        public DateTime JoinedDate { get; set; } = DateTime.UtcNow;
    }
}
