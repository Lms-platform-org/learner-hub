using Microsoft.AspNetCore.Mvc;
using Courses.Models;
using Courses.Api.Services;
using Microsoft.AspNetCore.Authorization;
using AutoMapper;
using Courses.Models.DTOs;
using Microsoft.Extensions.Logging;

namespace Courses.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoursesApiController : ControllerBase
    {
        private readonly ICourseService _courseService;
        private readonly IMapper _mapper;
        private readonly IUserContext _userContext;
        private readonly ILogger<CoursesApiController> _logger;

        public CoursesApiController(ICourseService courseService, IMapper mapper, IUserContext userContext, ILogger<CoursesApiController> logger)
        {
            _courseService = courseService;
            _mapper = mapper;
            _userContext = userContext;
            _logger = logger;
        }

        // GET: api/coursesapi (with pagination for student discovery)
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult> GetCourses([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                if (page < 1) page = 1;
                if (pageSize < 1 || pageSize > 50) pageSize = 10;

                var courses = await _courseService.GetPublishedCoursesAsync();
                var totalCount = courses.Count();

                var data = courses
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .Select(c => new
                    {
                        c.Id,
                        c.Title,
                        c.Description,
                        c.Category,
                        c.Price,
                        c.ThumbnailUrl,
                        InstructorName = c.InstructorId,
                        TotalLessons = c.Videos?.Count ?? 0,
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching all courses");
                return StatusCode(500, new { Message = "Error fetching courses" });
            }
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

        // NEW: Search courses endpoint
        [HttpGet("search")]
        [AllowAnonymous]
        public async Task<ActionResult> SearchCourses([FromQuery] string query, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest(new { Message = "Search query is required" });

            try
            {
                var courses = await _courseService.GetPublishedCoursesAsync();

                var filtered = courses.Where(c =>
                    c.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    c.Description.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                    c.Category.Contains(query, StringComparison.OrdinalIgnoreCase))
                    .ToList();

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error searching courses");
                return StatusCode(500, new { Message = "Error searching courses" });
            }
        }

        // GET: api/coursesapi/5
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<CourseReadDTO>> GetCourse(int id)
        {
            if (id <= 0)
                return BadRequest(new { Message = "Invalid course id" });

            try
            {
                var course = await _courseService.GetCourseByIdAsync(id);
                if (course == null)
                    return NotFound(new { Message = "Course not found" });

                var result = _mapper.Map<CourseReadDTO>(course);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching course {CourseId}", id);
                return StatusCode(500, new { Message = "Error fetching course" });
            }
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
            if (!Uri.TryCreate(videoUrl, UriKind.Absolute, out var uriResult) || 
                (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps))
            {
                return BadRequest("Invalid URL format. Please provide a valid http/https link.");
            }

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

        // POST: api/coursesapi/enroll (for notifications from student-service)
        [HttpPost("enroll")]
        [AllowAnonymous]
        public IActionResult NotifyEnrollment([FromBody] object payload)
        {
            _logger.LogInformation("Enrollment notification received: {Payload}", payload);
            return Ok(new { Message = "Enrollment received" });
        }

        // POST: api/coursesapi/progress (for notifications from student-service)
        [HttpPost("progress")]
        [AllowAnonymous]
        public IActionResult NotifyProgress([FromBody] object payload)
        {
            _logger.LogInformation("Progress notification received: {Payload}", payload);
            return Ok(new { Message = "Progress received" });
        }
    }
}
