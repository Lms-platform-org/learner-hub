using System;
using System.Collections.Generic;

namespace Courses.Models.DTOs
{
    public class DashboardStatsDTO
    {
        public int TotalCourses { get; set; }
        public int PublishedCourses { get; set; }
        public int TotalUsers { get; set; }
        public int TotalComments { get; set; }
        public int TotalVideos { get; set; }
        public int TotalEnrollments { get; set; }
        public List<RecentCourseDTO> RecentCourses { get; set; } = new();
    }

    public class RecentCourseDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }

    public class CourseDistributionDTO
    {
        public string Status { get; set; } = string.Empty;
        public int Count { get; set; }
    }

    public class TeacherDashboardDTO
    {
        public int TotalCourses { get; set; }
        public int TotalStudents { get; set; }
        public int TotalViews { get; set; }
    }
}
