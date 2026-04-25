using LearningPlatform.StudentService.DTOs;
using LearningPlatform.StudentService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.StudentService.Controllers
{
    //[Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ProfileController : BaseController
    {
        private readonly IProfileService _service;
        private readonly ILogger<ProfileController> _logger;

        public ProfileController(
            IProfileService service,
            ILogger<ProfileController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            _logger.LogInformation("Fetching profile");

            var profile = await _service.GetAsync(GetUserId());

            if (profile == null)
            {
                // return empty profile on first visit — student fills it in
                return Ok(ApiResponseDto<object>.Ok(new ProfileDto
                {
                    FullName = string.Empty,
                    Bio = null,
                    Skills = new List<string>(),
                    PreferredLevel = "Beginner",
                    Email = string.Empty,
                    JoinedDate = DateTime.UtcNow
                }));
            }

            return Ok(ApiResponseDto<object>.Ok(new ProfileDto
            {
                FullName = profile.FullName,
                Bio = profile.Bio,
                Skills = profile.Skills,
                PreferredLevel = profile.PreferredLevel,
                Email = string.Empty,
                JoinedDate = profile.JoinedDate
            }));
        }

        [HttpPost]
        public async Task<IActionResult> Save([FromBody] ProfileDto dto)
        {
            await _service.SaveAsync(GetUserId(), dto);

            return Ok(ApiResponseDto<string>.Ok("Profile updated"));
        }
    }
}
