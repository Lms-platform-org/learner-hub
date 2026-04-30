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
    public class CoursesApiController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly IMapper _mapper;
        private readonly IUserContext _userContext;

        public CoursesApiController(ICourseService courseService, IMapper mapper, IUserContext userContext)
        {
            _courseService = courseService;
            _mapper = mapper;
            _userContext = userContext;
        }

        // GET: api/Courses
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<CourseReadDTO>>> GetCourses()
        {
            var courses = await _courseService.GetAllCoursesAsync();
            return Ok(_mapper.Map<IEnumerable<CourseReadDTO>>(courses));
        }

        [HttpGet("TeacherIndex")]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult<IEnumerable<CourseReadDTO>>> GetTeacherCourses()
        {
            var instructorId = _userContext.UserId;
            if (string.IsNullOrEmpty(instructorId)) return Unauthorized();
            
            var courses = await _courseService.GetTeacherCoursesAsync(instructorId);
            return Ok(_mapper.Map<IEnumerable<CourseReadDTO>>(courses));
        }

        // GET: api/Courses/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<CourseReadDTO>> GetCourse(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null) return NotFound();
            return Ok(_mapper.Map<CourseReadDTO>(course));
        }

        // POST: api/Courses
        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult<CourseReadDTO>> PostCourse([FromBody] CourseWriteDTO courseDto)
        {
            var instructorId = _userContext.UserId;
            if (string.IsNullOrEmpty(instructorId)) return Unauthorized();
            
            // Check for duplicate title without throwing exception
            if (await _courseService.CheckDuplicateCourseAsync(courseDto.Title, instructorId))
            {
                return Conflict(new { 
                    StatusCode = 409, 
                    Message = $"A course with title '{courseDto.Title}' already exists for you." 
                });
            }

            var course = _mapper.Map<Course>(courseDto);
            course.InstructorId = instructorId;
            
            var createdCourse = await _courseService.CreateCourseAsync(course);
            var resultDto = _mapper.Map<CourseReadDTO>(createdCourse);
            
            return CreatedAtAction(nameof(GetCourse), new { id = createdCourse.Id }, resultDto);
        }

        // PUT: api/Courses/5
        [HttpPut("{id}")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> PutCourse(int id, [FromBody] CourseWriteDTO courseDto)
        {
            var instructorId = _userContext.UserId;
            if (string.IsNullOrEmpty(instructorId)) return Unauthorized();

            var existingCourse = await _courseService.GetCourseByIdAsync(id);
            
            if (existingCourse == null)
            {
                return NotFound();
            }

            if (existingCourse.InstructorId != instructorId)
            {
                return Forbid();
            }

            var courseInput = _mapper.Map<Course>(courseDto);
            var result = await _courseService.UpdateCourseAsync(id, courseInput);
            
            if (!result) return NotFound();
            return NoContent();
        }

        // POST: api/Courses/5/publish
        [HttpPost("{id}/publish")]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> PublishCourse(int id)
        {
            var instructorId = _userContext.UserId;
            if (string.IsNullOrEmpty(instructorId)) return Unauthorized();

            var course = await _courseService.PublishCourseAsync(id, instructorId);
            if (course == null) return NotFound();
            return Ok(course);
        }

        // POST: api/Courses/5/videos
        [HttpPost("{id}/videos")]
        [Authorize(Roles = "Teacher")]
        public async Task<ActionResult<CourseVideo>> PostCourseVideo(int id, [FromBody] string videoUrl)
        {
            // 1. Basic URL Validation
            if (!Uri.TryCreate(videoUrl, UriKind.Absolute, out var uriResult) || 
                (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
            {
                return BadRequest("Invalid URL format. Please provide a valid http/https link.");
            }

            // 2. Video Platform Check
            var url = videoUrl.ToLower();
            if (!(url.Contains("youtube.com") || url.Contains("youtu.be") || 
                  url.Contains("vimeo.com") || url.Contains("drive.google.com") || 
                  url.Contains(".mp4") || url.Contains(".webm")))
            {
                return BadRequest("Only video links (YouTube, Vimeo, Google Drive, MP4, etc.) are accepted.");
            }

            var instructorId = _userContext.UserId;
            if (string.IsNullOrEmpty(instructorId)) return Unauthorized();

            var video = await _courseService.AddVideoToCourseAsync(id, instructorId, videoUrl);
            if (video == null) return NotFound();

            return CreatedAtAction(nameof(GetCourse), new { id = id }, video);
        }
    }
}
