using LearningPlatform.StudentService.DTOs;
using LearningPlatform.StudentService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.StudentService.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : BaseController
    {
        private readonly IDashboardService _service;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(
            IDashboardService service,
            ILogger<DashboardController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation("Fetching dashboard for user");

            var data = await _service.GetDashboardAsync(GetUserId());

            if (data == null)
                return NotFound(ApiResponseDto<string>.Fail("Dashboard data not found"));

            return Ok(ApiResponseDto<object>.Ok(data));
        }
    }
}