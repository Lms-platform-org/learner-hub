using Microsoft.AspNetCore.Mvc;
using Courses.Models;
using Courses.Api.Services;
using Microsoft.AspNetCore.Authorization;

namespace Courses.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CoursesApiController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CoursesApiController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        // GET: api/Courses
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Course>>> GetCourses()
        {
            var courses = await _courseService.GetPublishedCoursesAsync();
            return Ok(courses);
        }

        // GET: api/Courses/TeacherIndex
        [HttpGet("TeacherIndex")]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult<IEnumerable<Course>>> GetTeacherCourses()
        {
            var teacherId = User.Identity?.Name ?? "Teacher1";
            var courses = await _courseService.GetTeacherCoursesAsync(teacherId);
            return Ok(courses);
        }

        // GET: api/Courses/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Course>> GetCourse(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null) return NotFound();
            return Ok(course);
        }

        // POST: api/Courses
        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult<Course>> PostCourse([FromBody] Course course)
        {
            course.TeacherId = User.Identity?.Name ?? "Teacher1";
            var createdCourse = await _courseService.CreateCourseAsync(course);
            return CreatedAtAction(nameof(GetCourse), new { id = createdCourse.Id }, createdCourse);
        }

        // PUT: api/Courses/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutCourse(int id, [FromBody] Course courseInput)
        {
            var result = await _courseService.UpdateCourseAsync(id, courseInput);
            if (!result) return NotFound();
            return NoContent();
        }

        // POST: api/Courses/5/publish
        [HttpPost("{id}/publish")]
        public async Task<IActionResult> PublishCourse(int id)
        {
            var teacherId = User.Identity?.Name ?? "Teacher1";
            var course = await _courseService.PublishCourseAsync(id, teacherId);
            if (course == null) return NotFound();
            return Ok(course);
        }

        // POST: api/Courses/5/videos
        [HttpPost("{id}/videos")]
        public async Task<ActionResult<CourseVideo>> PostCourseVideo(int id, [FromBody] string videoUrl)
        {
            var teacherId = User.Identity?.Name ?? "Teacher1";
            var video = await _courseService.AddVideoToCourseAsync(id, teacherId, videoUrl);
            if (video == null) return NotFound();
            return CreatedAtAction(nameof(GetCourse), new { id = id }, video);
        }
    }
}
