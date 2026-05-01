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

        // GET: api/coursesapi
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

        // GET: api/coursesapi/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<CourseReadDTO>> GetCourse(int id)
        {
            var course = await _courseService.GetCourseByIdAsync(id);
            if (course == null) return NotFound();
            return Ok(_mapper.Map<CourseReadDTO>(course));
        }

        // POST: api/coursesapi
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

        // PUT: api/coursesapi/5
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

        // POST: api/coursesapi/5/publish
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

        // POST: api/coursesapi/5/videos
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

        // NEW: Student Discovery Endpoints
        // GET: api/coursesapi/search?query=math&page=1&pageSize=10
        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<IActionResult> SearchCourses([FromQuery] string query, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest(new { Message = "Search query is required" });

            var courses = await _courseService.GetPublishedCoursesAsync();

            var filtered = courses.Where(c =>
                c.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                c.Description.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                c.Category.Contains(query, StringComparison.OrdinalIgnoreCase));

            var totalCount = filtered.Count();

            var data = filtered
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(c => new
                {
                    c.Id,
                    c.Title,
                    c.Description,
                    c.Category,
                    Level = string.Empty,
                    TotalLessons = c.Videos?.Count ?? 0,
                    ThumbnailUrl = c.ThumbnailUrl,
                    InstructorName = c.InstructorId,
                    VideoUrl = c.Videos?.FirstOrDefault()?.VideoUrl
                })
                .ToList();

            return Ok(new
            {
                Data = data,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount
            });
        }

        // POST: api/coursesapi/enroll
        [HttpPost("enroll")]
        [AllowAnonymous]
        public IActionResult NotifyEnrollment([FromBody] object payload)
        {
            return Ok(new { Message = "Enrollment received" });
        }

        // POST: api/coursesapi/progress
        [HttpPost("progress")]
        [AllowAnonymous]
        public IActionResult NotifyProgress([FromBody] object payload)
        {
            return Ok(new { Message = "Progress received" });
        }
    }
}
