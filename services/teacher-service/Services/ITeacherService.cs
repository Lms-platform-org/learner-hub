using System.Collections.Generic;
using System.Threading.Tasks;
using TeacherDashboardApi.DTOs;

namespace TeacherDashboardApi.Services
{
    public interface ITeacherService
    {
        Task<InstructorStatsDTO> GetInstructorStatsAsync(string teacherId);
        Task<EnrollmentGrowthDTO> GetEnrollmentGrowthAsync(string teacherId, string period);
        Task<IEnumerable<TopEnrolledCourseDTO>> GetTopEnrolledCoursesAsync(string teacherId);
        Task<CourseRevenueDataDTO> GetCourseRevenueStatsAsync(string teacherId);
        Task<IEnumerable<TeacherCourseListItemDTO>> GetTeacherCoursesForDashboardAsync(string teacherId);
    }
}
