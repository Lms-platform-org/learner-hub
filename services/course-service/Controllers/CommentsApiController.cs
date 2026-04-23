using Microsoft.AspNetCore.Mvc;
using Courses.Api.Services;
using Courses.Models.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace Courses.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CommentsApiController : ControllerBase
    {
        private readonly ICommentService _commentService;

        public CommentsApiController(ICommentService commentService)
        {
            _commentService = commentService;
        }

        // GET: api/Comments/Course/5
        [HttpGet("Course/{courseId}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCommentsForCourse(int courseId)
        {
            var comments = await _commentService.GetCommentsByCourseIdAsync(courseId);
            return Ok(comments);
        }

        // POST: api/Comments
        [HttpPost]
        public async Task<IActionResult> PostComment([FromBody] CreateCommentDto dto)
        {
            dto.UserId = User.Identity?.Name ?? "User1";
            var response = await _commentService.AddCommentAsync(dto);
            if (response == null)
            {
                return NotFound("Course or User not found.");
            }

            return Ok(response);
        }
    }
}
