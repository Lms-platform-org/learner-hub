using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Courses.Models;
using TeacherDashboardApi.DTOs;
using TeacherDashboardApi.Repositories;

namespace TeacherDashboardApi.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly ITeacherRepository _repository;

        public TeacherService(ITeacherRepository repository)
        {
            _repository = repository;
        }

        public async Task<InstructorStatsDTO> GetInstructorStatsAsync(string teacherId)
        {
            var courses = await _repository.GetCoursesByTeacherAsync(teacherId);
            var enrollments = await _repository.GetEnrollmentsByTeacherAsync(teacherId);

            var totalStudents = enrollments.Select(e => e.StudentId).Distinct().Count();
            var activeCourses = courses.Count(c => c.Status == CourseStatus.Published);
            
            // Calculate Revenue
            decimal totalRevenue = 0;
            foreach (var course in courses)
            {
                var courseEnrollments = enrollments.Count(e => e.CourseId == course.Id);
                totalRevenue += course.Price * courseEnrollments;
            }

            // Simulated Data for things not in DB
            double courseRating = 4.92; // Simulated as per image
            double studentGrowthPercentage = 12.0; // Simulated +12%
            int pendingReviews = 2; // Simulated

            return new InstructorStatsDTO
            {
                TotalStudents = totalStudents,
                CourseRating = courseRating,
                TotalRevenue = totalRevenue,
                ActiveCourses = activeCourses,
                StudentGrowthPercentage = studentGrowthPercentage,
                PendingReviews = pendingReviews
            };
        }

        public async Task<CourseRevenueDataDTO> GetCourseRevenueStatsAsync(string teacherId)
        {
            var courses = await _repository.GetCoursesByTeacherAsync(teacherId);
            var enrollments = await _repository.GetEnrollmentsByTeacherAsync(teacherId);

            var data = courses.Select(c => new CourseRevenuePointDTO
            {
                CourseTitle = c.Title,
                Revenue = c.Price * enrollments.Count(e => e.CourseId == c.Id)
            }).OrderByDescending(x => x.Revenue).Take(7).ToList();

            return new CourseRevenueDataDTO { Data = data };
        }

        public async Task<EnrollmentGrowthDTO> GetEnrollmentGrowthAsync(string teacherId, string period)
        {
            var enrollments = await _repository.GetEnrollmentsByTeacherAsync(teacherId);
            
            var growthData = new List<EnrollmentGrowthPointDTO>();

            if (period.ToLower() == "weekly")
            {
                // Group by Day of Week for the last week or just simulate Mon-Sun
                var days = new[] { "Mon", "Tue", "Wed", "Thu", "Fri", "Sat", "Sun" };
                var random = new Random();
                
                foreach (var day in days)
                {
                    // Simulate growth data as per image
                    growthData.Add(new EnrollmentGrowthPointDTO
                    {
                        Label = day,
                        Value = random.Next(100, 500)
                    });
                }
            }
            else
            {
                // Daily or Monthly simulation
                growthData.Add(new EnrollmentGrowthPointDTO { Label = "Jan", Value = 1200 });
                growthData.Add(new EnrollmentGrowthPointDTO { Label = "Feb", Value = 1500 });
                growthData.Add(new EnrollmentGrowthPointDTO { Label = "Mar", Value = 1800 });
            }

            return new EnrollmentGrowthDTO
            {
                Period = period,
                Data = growthData
            };
        }

        public async Task<IEnumerable<TopEnrolledCourseDTO>> GetTopEnrolledCoursesAsync(string teacherId)
        {
            var courses = await _repository.GetCoursesByTeacherAsync(teacherId);
            var enrollments = await _repository.GetEnrollmentsByTeacherAsync(teacherId);
            
            if (!courses.Any())
            {
                return Enumerable.Empty<TopEnrolledCourseDTO>();
            }

            var courseEnrollmentCounts = courses.Select(c => new
            {
                Course = c,
                Count = enrollments.Count(e => e.CourseId == c.Id)
            }).ToList();

            int totalEnrollments = enrollments.Count();

            return courseEnrollmentCounts
                .OrderByDescending(x => x.Count)
                .Take(5)
                .Select(x => new TopEnrolledCourseDTO
                {
                    CourseTitle = x.Course.Title,
                    EnrollmentCount = x.Count,
                    EnrollmentPercentage = totalEnrollments > 0 
                        ? Math.Round((double)x.Count / totalEnrollments * 100, 2) 
                        : 0
                });
        }

        public async Task<IEnumerable<TeacherCourseListItemDTO>> GetTeacherCoursesForDashboardAsync(string teacherId)
        {
            var courses = await _repository.GetCoursesByTeacherAsync(teacherId);
            var enrollments = await _repository.GetEnrollmentsByTeacherAsync(teacherId);

            var listItems = new List<TeacherCourseListItemDTO>();

            foreach (var course in courses)
            {
                var studentCount = enrollments.Count(e => e.CourseId == course.Id);
                listItems.Add(new TeacherCourseListItemDTO
                {
                    Id = course.Id,
                    Title = course.Title,
                    Category = string.IsNullOrEmpty(course.Category) ? "Uncategorized" : course.Category,
                    StudentCount = studentCount,
                    Status = course.Status.ToString()
                });
            }

            return listItems;
        }
    }
}
