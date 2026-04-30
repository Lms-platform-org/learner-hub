using LearningPlatform.StudentService.WebApp.Models.DTOs;
using LearningPlatform.StudentService.WebApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.StudentService.WebApp.Controllers
{
    public class ProfileController : Controller
    {
        private readonly ProfileApiService _service;

        public ProfileController(ProfileApiService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _service.GetAsync();

            // Fall back to the name entered at login when the API has no FullName yet
            if (string.IsNullOrWhiteSpace(data.FullName))
                data.FullName = HttpContext.Session.GetString("name") ?? string.Empty;

            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> Save(
            string FullName,
            string? Bio,
            string? SkillsInput,
            string PreferredLevel)
        {
            var skills = string.IsNullOrWhiteSpace(SkillsInput)
                ? new List<string>()
                : SkillsInput.Split(',')
                    .Select(s => s.Trim())
                    .Where(s => !string.IsNullOrEmpty(s))
                    .ToList();

            await _service.SaveAsync(new ProfileDto
            {
                FullName = FullName,
                Bio = Bio,
                Skills = skills,
                PreferredLevel = PreferredLevel ?? "Beginner"
            });

            TempData["Success"] = "Profile saved successfully!";
            return RedirectToAction("Index");
        }
    }
}
