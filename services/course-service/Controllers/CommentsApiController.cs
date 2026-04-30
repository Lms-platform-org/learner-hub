using Microsoft.AspNetCore.Mvc;
using Courses.Api.Services;
using Courses.Models.DTOs;
using Microsoft.AspNetCore.Authorization;

namespace Courses.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsApiController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly IUserContext _userContext;

        public CommentsApiController(ICommentService commentService, IUserContext userContext)
        {
            _commentService = commentService;
            _userContext = userContext;
        }

        // GET: api/Comments
        [HttpGet]
        public async Task<IActionResult> GetComments()
        {
            var comments = await _commentService.GetAllCommentsAsync();
            return Ok(comments);
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
        [Authorize]
        public async Task<IActionResult> PostComment([FromBody] CreateCommentDto dto)
        {
            var userId = _userContext.UserId;
            if (string.IsNullOrEmpty(userId)) return Unauthorized();

            dto.UserId = userId;
            var response = await _commentService.AddCommentAsync(dto);
            if (response == null)
            {
                return NotFound("Course not found.");
            }

            return Ok(response);
        }
    }
}
