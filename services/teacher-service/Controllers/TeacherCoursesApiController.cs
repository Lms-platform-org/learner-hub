using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeacherDashboardApi.DTOs;
using TeacherDashboardApi.Services;

namespace TeacherDashboardApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Teacher")]
    public class TeacherCoursesApiController : ControllerBase
    {
        private readonly ITeacherService _teacherService;

        public TeacherCoursesApiController(ITeacherService teacherService)
        {
            _teacherService = teacherService;
        }

        // GET: api/TeacherCoursesApi/stats
        [HttpGet("stats")]
        public async Task<ActionResult<InstructorStatsDTO>> GetStats()
        {
            var teacherId = User.Identity?.Name;
            if (string.IsNullOrEmpty(teacherId)) return Unauthorized();

            var stats = await _teacherService.GetInstructorStatsAsync(teacherId);
            return Ok(stats);
        }

        // GET: api/TeacherCoursesApi/enrollment-growth
        [HttpGet("enrollment-growth")]
        public async Task<ActionResult<EnrollmentGrowthDTO>> GetEnrollmentGrowth([FromQuery] string period = "Weekly")
        {
            var teacherId = User.Identity?.Name;
            if (string.IsNullOrEmpty(teacherId)) return Unauthorized();

            var growth = await _teacherService.GetEnrollmentGrowthAsync(teacherId, period);
            return Ok(growth);
        }

        // GET: api/TeacherCoursesApi/course-revenue
        [HttpGet("course-revenue")]
        public async Task<ActionResult<CourseRevenueDataDTO>> GetCourseRevenue()
        {
            var teacherId = User.Identity?.Name;
            if (string.IsNullOrEmpty(teacherId)) return Unauthorized();

            var revenueData = await _teacherService.GetCourseRevenueStatsAsync(teacherId);
            return Ok(revenueData);
        }

        // GET: api/TeacherCoursesApi/top-enrolled-courses
        [HttpGet("top-enrolled-courses")]
        public async Task<ActionResult<IEnumerable<TopEnrolledCourseDTO>>> GetTopEnrolledCourses()
        {
            var teacherId = User.Identity?.Name;
            if (string.IsNullOrEmpty(teacherId)) return Unauthorized();

            var courses = await _teacherService.GetTopEnrolledCoursesAsync(teacherId);
            return Ok(courses);
        }

        // GET: api/TeacherCoursesApi/courses
        [HttpGet("courses")]
        public async Task<ActionResult<IEnumerable<TeacherCourseListItemDTO>>> GetDashboardCourses()
        {
            var teacherId = User.Identity?.Name;
            if (string.IsNullOrEmpty(teacherId)) return Unauthorized();

            var courses = await _teacherService.GetTeacherCoursesForDashboardAsync(teacherId);
            return Ok(courses);
        }
    }
}
