using LearningPlatform.StudentService.DTOs;
using LearningPlatform.StudentService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.StudentService.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DiscoveryController : BaseController
    {
        private readonly IBookDiscoveryService _bookService;
        private readonly ICourseIntegrationService _courseService;
        private readonly ILogger<DiscoveryController> _logger;

        public DiscoveryController(
            IBookDiscoveryService bookService,
            ICourseIntegrationService courseService,
            ILogger<DiscoveryController> logger)
        {
            _bookService = bookService;
            _courseService = courseService;
            _logger = logger;
        }

        [HttpGet("books")]
        public async Task<IActionResult> SearchBooks(
            [FromQuery] string topic,
            int page = 1,
            int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(topic))
                return BadRequest(ApiResponseDto<string>.Fail("Topic is required"));

            _logger.LogInformation("Searching books for topic");

            var result = await _bookService.SearchBooksByTopicAsync(topic, page, pageSize);

            if (result?.Data == null || !result.Data.Any())
                return NotFound(ApiResponseDto<string>.Fail($"No books found for '{topic}'"));

            return Ok(ApiResponseDto<object>.Ok(result));
        }

        [HttpGet("courses")]
        public async Task<IActionResult> GetAllCourses(int page = 1, int pageSize = 10)
        {
            _logger.LogInformation("Fetching all courses, page {Page}", page);

            var result = await _courseService.GetAllCoursesAsync(page, pageSize);

            return Ok(ApiResponseDto<object>.Ok(result));
        }

        [HttpGet("courses/search")]
        public async Task<IActionResult> SearchCourses(
            [FromQuery] string query,
            int page = 1,
            int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest(ApiResponseDto<string>.Fail("Search query is required"));

            _logger.LogInformation("Searching courses");

            var result = await _courseService.SearchCoursesAsync(query, page, pageSize);

            if (result?.Data == null || !result.Data.Any())
                return NotFound(ApiResponseDto<string>.Fail($"No courses found for '{query}'"));

            return Ok(ApiResponseDto<object>.Ok(result));
        }

        [HttpGet("courses/{courseId:int}")]
        public async Task<IActionResult> GetCourse(int courseId)
        {
            var result = await _courseService.GetCourseByIdAsync(courseId);

            if (result == null)
                return NotFound(ApiResponseDto<string>.Fail("Course not found"));

            return Ok(ApiResponseDto<object>.Ok(result));
        }
    }
}