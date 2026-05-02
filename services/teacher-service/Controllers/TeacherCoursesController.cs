using LearningPlatform.TeacherService.Common;
using LearningPlatform.TeacherService.DTOs;
using LearningPlatform.TeacherService.Exceptions;
using LearningPlatform.TeacherService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.TeacherService.Controllers
{
    /// <summary>
    /// Course management endpoints for teachers.
    ///
    /// All write operations are forwarded to the Course API via
    /// <see cref="ITeacherCourseService"/> → <see cref="ICourseApiClient"/>.
    /// The Teacher API never owns course data directly; it is always the
    /// Course API that persists and authorises the operation.
    /// </summary>
    [ApiController]
    [Route("api/teacher/courses")]
    [Authorize(Roles = "Teacher")]
    public class TeacherCoursesController : ControllerBase
    {
        private readonly ITeacherCourseService _courseService;
        private readonly IUserContext _userContext;
        private readonly ILogger<TeacherCoursesController> _logger;

        public TeacherCoursesController(
            ITeacherCourseService courseService,
            IUserContext userContext,
            ILogger<TeacherCoursesController> logger)
        {
            _courseService = courseService;
            _userContext = userContext;
            _logger = logger;
        }

        // ── Helpers ─────────────────────────────────────────────────────────

        private IActionResult TeacherRequired()
            => Unauthorized(ApiResponseDto<object>.Fail("Teacher identity could not be resolved."));

        // ── Endpoints ───────────────────────────────────────────────────────

        /// <summary>
        /// GET api/teacher/courses
        /// List all courses created by the authenticated teacher.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponseDto<List<CourseReadDto>>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetMyCourses()
        {
            var teacherId = _userContext.UserId;
            if (string.IsNullOrEmpty(teacherId)) return TeacherRequired();

            var courses = await _courseService.GetMyCoursesAsync(teacherId);
            return Ok(ApiResponseDto<List<CourseReadDto>>.Ok(courses,
                $"{courses.Count} course(s) found."));
        }

        /// <summary>
        /// GET api/teacher/courses/{id}
        /// Get a single course by id. Returns 404 if not found.
        /// </summary>
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(ApiResponseDto<CourseReadDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetCourse(int id)
        {
            if (id <= 0)
                return BadRequest(ApiResponseDto<object>.Fail("Invalid course id."));

            var course = await _courseService.GetCourseAsync(id);
            if (course is null)
                return NotFound(ApiResponseDto<object>.Fail($"Course {id} not found."));

            return Ok(ApiResponseDto<CourseReadDto>.Ok(course));
        }

        /// <summary>
        /// POST api/teacher/courses
        /// Create a new course. The authenticated teacher's id is attached automatically.
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponseDto<CourseReadDto>), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateCourse([FromBody] CourseWriteDto dto)
        {
            // FluentValidation runs automatically via AddFluentValidationAutoValidation
            var teacherId = _userContext.UserId;
            if (string.IsNullOrEmpty(teacherId)) return TeacherRequired();

            var created = await _courseService.CreateCourseAsync(dto, teacherId);
            return CreatedAtAction(nameof(GetCourse), new { id = created.Id },
                ApiResponseDto<CourseReadDto>.Ok(created, "Course created successfully."));
        }

        /// <summary>
        /// PUT api/teacher/courses/{id}
        /// Update an existing course owned by the authenticated teacher.
        /// </summary>
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status204NoContent)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> UpdateCourse(int id, [FromBody] CourseWriteDto dto)
        {
            if (id <= 0)
                return BadRequest(ApiResponseDto<object>.Fail("Invalid course id."));

            var teacherId = _userContext.UserId;
            if (string.IsNullOrEmpty(teacherId)) return TeacherRequired();

            await _courseService.UpdateCourseAsync(id, dto, teacherId);
            return NoContent();
        }

        /// <summary>
        /// POST api/teacher/courses/{id}/publish
        /// Transition a draft course to Published status.
        /// </summary>
        [HttpPost("{id:int}/publish")]
        [ProducesResponseType(typeof(ApiResponseDto<CourseReadDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> PublishCourse(int id)
        {
            if (id <= 0)
                return BadRequest(ApiResponseDto<object>.Fail("Invalid course id."));

            var teacherId = _userContext.UserId;
            if (string.IsNullOrEmpty(teacherId)) return TeacherRequired();

            var published = await _courseService.PublishCourseAsync(id, teacherId);
            return Ok(ApiResponseDto<CourseReadDto>.Ok(published, "Course published successfully."));
        }

        /// <summary>
        /// POST api/teacher/courses/{id}/videos
        /// Add a video URL to an existing course.
        /// </summary>
        [HttpPost("{id:int}/videos")]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> AddVideo(int id, [FromBody] string videoUrl)
        {
            if (id <= 0)
                return BadRequest(ApiResponseDto<object>.Fail("Invalid course id."));

            if (string.IsNullOrWhiteSpace(videoUrl))
                return BadRequest(ApiResponseDto<object>.Fail("Video URL is required."));

            var teacherId = _userContext.UserId;
            if (string.IsNullOrEmpty(teacherId)) return TeacherRequired();

            await _courseService.AddVideoAsync(id, videoUrl, teacherId);
            return Ok(ApiResponseDto<object>.Ok(new { courseId = id, videoUrl },
                "Video added successfully."));
        }

        /// <summary>
        /// GET api/teacher/courses/{id}/enrollments
        /// List all enrollment records for a specific course.
        /// Useful for the teacher to see which students are enrolled.
        /// </summary>
        [HttpGet("{id:int}/enrollments")]
        [ProducesResponseType(typeof(ApiResponseDto<List<EnrollmentReadDto>>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetEnrollments(int id)
        {
            if (id <= 0)
                return BadRequest(ApiResponseDto<object>.Fail("Invalid course id."));

            var teacherId = _userContext.UserId;
            if (string.IsNullOrEmpty(teacherId)) return TeacherRequired();

            // Verify the course exists before returning enrollment data
            var course = await _courseService.GetCourseAsync(id);
            if (course is null)
                return NotFound(ApiResponseDto<object>.Fail($"Course {id} not found."));

            // Guard: only the owner should see enrollment data
            if (!string.Equals(course.InstructorId, teacherId, StringComparison.OrdinalIgnoreCase))
                return Forbid();

            _logger.LogInformation("Teacher {TeacherId} fetching enrollments for course {CourseId}",
                teacherId, id);

            // Use the client directly for this read-only query
            // (no business logic needed beyond the ownership check above)
            var client = HttpContext.RequestServices.GetRequiredService<ICourseApiClient>();
            var enrollments = await client.GetCourseEnrollmentsAsync(id);

            return Ok(ApiResponseDto<List<EnrollmentReadDto>>.Ok(enrollments,
                $"{enrollments.Count} enrollment(s) found."));
        }
    }
}