using LearningPlatformAuth.Models;
using LearningPlatformAuth.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace LearningPlatformAuth.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _service;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IAuthService service, ILogger<AuthController> logger)
        {
            _service = service;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest model)
        {
            try
            {
                var result = await _service.RegisterAsync(model);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception in Register endpoint");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginRequest model)
        {
            try
            {
                var result = await _service.LoginAsync(model);

                if (result == null)
                    return Unauthorized(new { error = "Invalid credentials" });

                if (result.Token == "PENDING_APPROVAL")
                    return Unauthorized(new { error = "Pending approval" });

                if (result.Token == "TEACHER_REJECTED")
                    return Unauthorized(new { error = result.Message });

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception in Login endpoint");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }

        [HttpGet("pending-teachers")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetPendingTeachers()
        {
            try
            {
                var result = await _service.GetPendingTeachersAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception in GetPendingTeachers endpoint");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }

        [HttpPost("approve-teacher/{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Approve(string id, ApprovalRequest req)
        {
            try
            {
                var adminId = User.FindFirst("sub")?.Value ?? User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "Unknown";
                var result = await _service.ApproveTeacherAsync(id, req, adminId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception in Approve endpoint");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }

        [HttpPost("reject-teacher/{id}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> Reject(string id, ApprovalRequest req)
        {
            try
            {
                var adminId = User.FindFirst("sub")?.Value ?? "Unknown";
                var result = await _service.RejectTeacherAsync(id, req, adminId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception in Reject endpoint");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }

       
        [HttpGet("user/{id}")]
        [Authorize]
        public async Task<IActionResult> GetUserById(string id)
        {
            try
            {
                var user = await _service.GetUserByIdAsync(id);
                if (user == null)
                    return NotFound(new { error = "User not found" });

                return Ok(new { success = true, data = user });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception in GetUserById endpoint");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }

      
        [HttpGet("user-by-email")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetUserByEmail([FromQuery] string email)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(email))
                    return BadRequest(new { error = "Email is required" });

                var user = await _service.GetUserByEmailAsync(email);
                if (user == null)
                    return NotFound(new { error = "User not found" });

                return Ok(new { success = true, data = user });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception in GetUserByEmail endpoint");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }

        
        [HttpGet("users-by-role/{role}")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetUsersByRole(string role)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(role))
                    return BadRequest(new { error = "Role is required" });

                var users = await _service.GetUsersByRoleAsync(role);
                return Ok(new { success = true, count = users.Count, data = users });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception in GetUsersByRole endpoint");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }

      
        [HttpGet("users/approved")]
        [Authorize(Policy = "AdminOnly")]
        public async Task<IActionResult> GetApprovedUsers()
        {
            try
            {
                var users = await _service.GetApprovedUsersAsync();
                return Ok(new { success = true, count = users.Count, data = users });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception in GetApprovedUsers endpoint");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }

       
        [HttpGet("verify-token")]
        [Authorize]
        public IActionResult VerifyToken()
        {
            try
            {
                var userId = User.FindFirst("sub")?.Value ?? User.FindFirst("nameid")?.Value;
                if (string.IsNullOrEmpty(userId))
                    return Unauthorized(new { error = "Invalid token" });

                return Ok(new { success = true, message = "Token is valid", userId = userId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception in VerifyToken endpoint");
                return StatusCode(StatusCodes.Status500InternalServerError, new { error = ex.Message });
            }
        }
    }
}