using Microsoft.AspNetCore.Mvc;
using Courses.Models;
using Courses.Api.Services;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using Courses.Models.DTOs;

namespace Courses.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EnrollmentsApiController : ControllerBase
    {
        private readonly IEnrollmentService _enrollmentService;
        private readonly IMapper _mapper;
        private readonly IUserContext _userContext;

        public EnrollmentsApiController(IEnrollmentService enrollmentService, IMapper mapper, IUserContext userContext)
        {
            _enrollmentService = enrollmentService;
            _mapper = mapper;
            _userContext = userContext;
        }

        // GET: api/Enrollments
        [HttpGet]
        [Authorize(Roles = "Admin")] // Assuming only admin should see all enrollments
        public async Task<ActionResult<IEnumerable<EnrollmentReadDTO>>> GetEnrollments()
        {
            var enrollments = await _enrollmentService.GetAllEnrollmentsAsync();
            return Ok(_mapper.Map<IEnumerable<EnrollmentReadDTO>>(enrollments));
        }

        // POST: api/Enrollments
        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<EnrollmentReadDTO>> Enroll([FromBody] EnrollmentCreateDTO request)
        {
            string studentId = _userContext.UserId;
            if (string.IsNullOrEmpty(studentId)) return Unauthorized();

            // 1. Check if already enrolled to provide a better error message
            var courses = await _enrollmentService.GetStudentCoursesAsync(studentId);
            if (courses.Any(c => c.Id == request.CourseId))
            {
                return Conflict(new { Message = "You are already enrolled in this course." });
            }
            
            var enrollment = await _enrollmentService.EnrollStudentAsync(request.CourseId, studentId);
            if (enrollment == null) return BadRequest("Could not enroll student.");

            return Ok(_mapper.Map<EnrollmentReadDTO>(enrollment));
        }

        // GET: api/Enrollments/Course/5
        [HttpGet("Course/{courseId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<EnrollmentReadDTO>>> GetCourseEnrollments(int courseId)
        {
            var enrollments = await _enrollmentService.GetCourseEnrollmentsAsync(courseId);
            return Ok(_mapper.Map<IEnumerable<EnrollmentReadDTO>>(enrollments));
        }

        // GET: api/Enrollments/Course/5/count
        [HttpGet("Course/{courseId}/count")]
        [AllowAnonymous]
        public async Task<ActionResult<int>> GetEnrollmentCount(int courseId)
        {
            var count = await _enrollmentService.GetEnrollmentCountByCourseIdAsync(courseId);
            return Ok(count);
        }

        // GET: api/Enrollments/Student
        [HttpGet("Student")]
        [Authorize(Roles = "Student")]
        public async Task<ActionResult<IEnumerable<CourseReadDTO>>> GetMyCourses()
        {
            string studentId = _userContext.UserId;
            if (string.IsNullOrEmpty(studentId)) return Unauthorized();

            var courses = await _enrollmentService.GetStudentCoursesAsync(studentId);
            return Ok(_mapper.Map<IEnumerable<CourseReadDTO>>(courses));
        }
    }
}
