using LearningPlatform.TeacherService.Common;
using LearningPlatform.TeacherService.DTOs;
using LearningPlatform.TeacherService.Exceptions;
using LearningPlatform.TeacherService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.TeacherService.Controllers
{
    /// <summary>
    /// Provides the aggregated teacher dashboard.
    /// Calls <see cref="ITeacherDashboardService"/> which fans out concurrently
    /// to the Course API to collect course + enrollment data.
    /// </summary>
    [ApiController]
    [Route("api/teacher/dashboard")]
    [Authorize(Roles = "Teacher")]
    public class TeacherDashboardController : ControllerBase
    {
        private readonly ITeacherDashboardService _dashboardService;
        private readonly IUserContext _userContext;
        private readonly ILogger<TeacherDashboardController> _logger;

        public TeacherDashboardController(
            ITeacherDashboardService dashboardService,
            IUserContext userContext,
            ILogger<TeacherDashboardController> logger)
        {
            _dashboardService = dashboardService;
            _userContext = userContext;
            _logger = logger;
        }

        /// <summary>
        /// GET api/teacher/dashboard
        /// Returns all dashboard KPIs for the authenticated teacher.
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(ApiResponseDto<TeacherDashboardDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiResponseDto<object>), StatusCodes.Status502BadGateway)]
        public async Task<IActionResult> GetDashboard()
        {
            var teacherId = _userContext.UserId;
            if (string.IsNullOrEmpty(teacherId))
                return Unauthorized(ApiResponseDto<object>.Fail("Teacher identity could not be resolved."));

            _logger.LogInformation("Dashboard requested for teacher {TeacherId}", teacherId);

            var dashboard = await _dashboardService.GetDashboardAsync(teacherId);
            return Ok(ApiResponseDto<TeacherDashboardDto>.Ok(dashboard));
        }
    }
}