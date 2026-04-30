using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TeacherDashboardApi.Data;
using TeacherDashboardApi.DTOs;
using TeacherDashboardApi.Models;

namespace TeacherDashboardApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Teacher")]
    public class TeacherProfileController : ControllerBase
    {
        private readonly TeacherDashboardDbContext _context;

        public TeacherProfileController(TeacherDashboardDbContext context)
        {
            _context = context;
        }

        // GET: api/TeacherProfile
        [HttpGet]
        public async Task<ActionResult<TeacherProfileDTO>> GetProfile()
        {
            var teacherId = User.Identity?.Name;
            if (string.IsNullOrEmpty(teacherId)) return Unauthorized();

            var profile = await _context.TeacherProfiles.FindAsync(teacherId);
            
            if (profile == null)
            {
                return NotFound("Profile not found for this teacher.");
            }

            var dto = new TeacherProfileDTO
            {
                FullName = profile.FullName,
                Bio = profile.Bio,
                Skills = profile.Skills,
                PreferredLevel = profile.PreferredLevel,
                Email = profile.Email,
                JoinedDate = profile.JoinedDate
            };

            return Ok(dto);
        }

        // POST: api/TeacherProfile
        [HttpPost]
        public async Task<ActionResult<TeacherProfileDTO>> SaveProfile([FromBody] TeacherProfileDTO dto)
        {
            var teacherId = User.Identity?.Name;
            if (string.IsNullOrEmpty(teacherId)) return Unauthorized();

            var profile = await _context.TeacherProfiles.FindAsync(teacherId);

            if (profile == null)
            {
                profile = new TeacherProfile
                {
                    Id = teacherId,
                    FullName = dto.FullName,
                    Bio = dto.Bio,
                    Skills = dto.Skills,
                    PreferredLevel = dto.PreferredLevel,
                    Email = dto.Email,
                    JoinedDate = dto.JoinedDate
                };
                _context.TeacherProfiles.Add(profile);
            }
            else
            {
                profile.FullName = dto.FullName;
                profile.Bio = dto.Bio;
                profile.Skills = dto.Skills;
                profile.PreferredLevel = dto.PreferredLevel;
                profile.Email = dto.Email;
                profile.JoinedDate = dto.JoinedDate;
                _context.TeacherProfiles.Update(profile);
            }

            await _context.SaveChangesAsync();

            return Ok(dto);
        }
    }
}
