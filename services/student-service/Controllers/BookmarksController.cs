using LearningPlatform.StudentService.DTOs;
using LearningPlatform.StudentService.Models;
using LearningPlatform.StudentService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.StudentService.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BookmarksController : BaseController
    {
        private readonly IBookmarkService _service;
        private readonly ILogger<BookmarksController> _logger;

        public BookmarksController(
            IBookmarkService service,
            ILogger<BookmarksController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll(int page = 1, int pageSize = 10)
        {
            _logger.LogInformation("Fetching bookmarks for user");

            var result = await _service.GetPagedAsync(GetUserId(), page, pageSize);

            return Ok(ApiResponseDto<object>.Ok(result));
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var data = await _service.GetByIdAsync(GetUserId(), id);

            if (data == null)
                return NotFound();

            return Ok(ApiResponseDto<Bookmark>.Ok(data));
        }


        [HttpGet("category/{category}")]
        public async Task<IActionResult> GetByCategory(string category, int page = 1, int pageSize = 10)
        {
            var result = await _service.GetByCategoryPagedAsync(GetUserId(), category, page, pageSize);

            return Ok(ApiResponseDto<object>.Ok(result));
        }


        [HttpPost]
        public async Task<IActionResult> Create([FromBody] BookmarkDto dto)
        {
            await _service.AddAsync(GetUserId(), dto);
            return Created("", ApiResponseDto<string>.Ok("Bookmark created"));
        }

        [HttpPost("toggle")]
        public async Task<IActionResult> Toggle([FromBody] BookmarkDto dto)
        {
            var isBookmarked = await _service.ToggleAsync(GetUserId(), dto);
            return Ok(ApiResponseDto<object>.Ok(new { isBookmarked }));
        }

        [HttpPost("check")]
        public async Task<IActionResult> Check([FromBody] BookmarkDto dto)
        {
            var isBookmarked = await _service.IsBookmarkedAsync(GetUserId(), dto);
            return Ok(ApiResponseDto<object>.Ok(new { isBookmarked }));
        }

        [HttpPatch("{id}/note")]
        public async Task<IActionResult> UpdateNote(int id, [FromBody] UpdateNoteDto dto)
        {
            await _service.UpdateNoteAsync(GetUserId(), id, dto.Note);
            return Ok(ApiResponseDto<string>.Ok("Note updated"));
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _service.DeleteAsync(GetUserId(), id);

            if (!deleted)
                return NotFound(ApiResponseDto<string>.Fail("Bookmark not found"));

            return NoContent();
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] BookmarkDto dto)
        {
            var updated = await _service.UpdateAsync(GetUserId(), id, dto);

            if (!updated)
                return NotFound(ApiResponseDto<string>.Fail("Bookmark not found"));

            return Ok(ApiResponseDto<string>.Ok("Bookmark updated"));
        }
    }
}