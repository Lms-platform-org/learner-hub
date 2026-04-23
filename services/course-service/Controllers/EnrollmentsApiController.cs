using Microsoft.AspNetCore.Mvc;
using Courses.Models;
using Courses.Api.Services;
using Microsoft.AspNetCore.Authorization;

namespace Courses.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EnrollmentsApiController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;

        public EnrollmentsApiController(IEnrollmentService enrollmentService)
        {
            _enrollmentService = enrollmentService;
        }

        // POST: api/Enrollments
        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<Enrollment>> Enroll([FromBody] EnrollmentRequest request)
        {
            string studentId = User.Identity?.Name ?? "Student1";
            
            var enrollment = await _enrollmentService.EnrollStudentAsync(request.CourseId, studentId);
            if (enrollment == null) return BadRequest("Could not enroll student.");

            return Ok(enrollment);
        }

        // GET: api/Enrollments/Course/5
        [HttpGet("Course/{courseId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Enrollment>>> GetCourseEnrollments(int courseId)
        {
            var enrollments = await _enrollmentService.GetCourseEnrollmentsAsync(courseId);
            return Ok(enrollments);
        }

        // GET: api/Enrollments/Course/5/count
        [HttpGet("Course/{courseId}/count")]
        [AllowAnonymous]
        public async Task<ActionResult<int>> GetEnrollmentCount(int courseId)
        {
            var count = await _enrollmentService.GetEnrollmentCountByCourseIdAsync(courseId);
            return Ok(count);
        }
    }

    public class EnrollmentRequest
    {
        public int CourseId { get; set; }
        public string? StudentId { get; set; }
    }
}
