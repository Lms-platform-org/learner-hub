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
    public class ProgressController : BaseController
    {
        private readonly IStudentExperienceService _service;
        private readonly IEnrollmentService _enrollmentService;
        private readonly ILogger<ProgressController> _logger;

        public ProgressController(
            IStudentExperienceService service,
            IEnrollmentService enrollmentService,
            ILogger<ProgressController> logger)
        {
            _service = service;
            _enrollmentService = enrollmentService;
            _logger = logger;
        }

        [HttpPost("complete/{courseId:int}/{lessonId:int}")]
        public async Task<IActionResult> Complete(int courseId, int lessonId)
        {
            if (!await _enrollmentService.IsEnrolledAsync(GetUserId(), courseId))
                return Forbid();

            _logger.LogInformation("Marking lesson complete");

            var result = await _service.MarkLessonCompleteAsync(GetUserId(), courseId, lessonId);

            var response = new ProgressResponseDto
            {
                Percentage = result.Percentage,
                XP = result.XPScore,
                EarnedMilestones = result.EarnedMilestones,
                LastLessonId = result.LastLessonId,
                LastVideoTimestampSeconds = result.LastVideoTimestampSeconds,
                LastAccessed = result.LastAccessed
            };

            return Ok(ApiResponseDto<object>.Ok(response));
        }

        [HttpGet("{courseId:int}")]
        public async Task<IActionResult> GetProgress(int courseId)
        {
            _logger.LogInformation("Fetching progress");

            var progress = await _service.GetProgressAsync(GetUserId(), courseId);

            if (progress == null)
                return NotFound(ApiResponseDto<string>.Fail("Progress not found"));

            var response = new ProgressResponseDto
            {
                Percentage = progress.Percentage,
                XP = progress.XPScore,
                EarnedMilestones = progress.EarnedMilestones ?? new(),
                LastLessonId = progress.LastLessonId,
                LastVideoTimestampSeconds = progress.LastVideoTimestampSeconds,
                LastAccessed = progress.LastAccessed
            };

            return Ok(ApiResponseDto<object>.Ok(response));
        }


        [HttpPost("resume")]
        public async Task<IActionResult> Resume([FromBody] ResumeDto dto)
        {
            if (!await _enrollmentService.IsEnrolledAsync(GetUserId(), dto.CourseId))
                return Forbid();

            _logger.LogInformation("Updating resume point");

            await _service.SetResumePointAsync(
                GetUserId(),
                dto.CourseId,
                dto.LessonId,
                dto.Seconds
            );

            return Ok(ApiResponseDto<string>.Ok("Resume updated"));
        }
    }
}