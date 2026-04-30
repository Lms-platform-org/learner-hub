using LearningPlatform.StudentService.Common;
using LearningPlatform.StudentService.DTOs;
using LearningPlatform.StudentService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.StudentService.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class RecommendationController : BaseController
    {
        private readonly IRecommendationService _service;
        private readonly ILogger<RecommendationController> _logger;

        public RecommendationController(
            IRecommendationService service,
            ILogger<RecommendationController> logger)
        {
            _service = service;
            _logger = logger;
        }


        [HttpGet("trending")]
        public async Task<IActionResult> GetTrending(int count = 5)
        {
            _logger.LogInformation("Fetching trending courses");

            var result = await _service.GetTrendingCourseIdsAsync(count);

            return Ok(ApiResponseDto<object>.Ok(result));
        }


        [HttpGet("personalized")]
        public async Task<IActionResult> GetPersonalized(int count = 5)
        {
            _logger.LogInformation("Fetching personalized recommendations");

            var result = await _service.GetPersonalizedCourseIdsAsync(GetUserId(), count);

            return Ok(ApiResponseDto<object>.Ok(result));
        }
    }
}