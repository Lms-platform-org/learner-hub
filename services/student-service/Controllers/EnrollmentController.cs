using LearningPlatform.StudentService.Common;
using LearningPlatform.StudentService.DTOs;
using LearningPlatform.StudentService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.StudentService.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class EnrollmentController : BaseController
    {
        private readonly IEnrollmentService _service;
        private readonly ILogger<EnrollmentController> _logger;

        public EnrollmentController(IEnrollmentService service, ILogger<EnrollmentController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // GET /api/enrollment — list all courses student is enrolled in
        [HttpGet]
        public async Task<IActionResult> GetEnrolled()
        {
            _logger.LogInformation("Fetching enrolled courses");
            var result = await _service.GetEnrolledCoursesAsync(GetUserId());
            return Ok(ApiResponseDto<object>.Ok(result));
        }

        // GET /api/enrollment/{courseId}/status — check if enrolled
        [HttpGet("{courseId:int}/status")]
        public async Task<IActionResult> CheckEnrollment(int courseId)
        {
            var isEnrolled = await _service.IsEnrolledAsync(GetUserId(), courseId);
            return Ok(ApiResponseDto<object>.Ok(new { isEnrolled }));
        }

        // POST /api/enrollment/{courseId} — enroll in a course
        [HttpPost("{courseId:int}")]
        public async Task<IActionResult> Enroll(int courseId)
        {
            _logger.LogInformation("Enrolling in course {CourseId}", courseId);
            await _service.EnrollAsync(GetUserId(), courseId);
            return Created("", ApiResponseDto<string>.Ok("Enrolled successfully"));
        }
    }
}
