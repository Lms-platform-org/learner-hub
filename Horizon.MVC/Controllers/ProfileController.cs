using Horizon.MVC.DTOs;
using Horizon.MVC.ViewModels;
using Horizon.MVC.Models;
using Horizon.MVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace Horizon.MVC.Controllers
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
            try
            {
                var data = await _service.GetAsync();

                // Fall back to the name entered at login when the API has no FullName yet
                if (string.IsNullOrWhiteSpace(data.FullName))
                    data.FullName = HttpContext.Session.GetString("name") ?? string.Empty;

                return View(data);
            }
            catch (UnauthorizedAccessException)
            {
                TempData["Error"] = "Your session has expired. Please log in again.";
                return RedirectToAction("Index", "Dashboard");
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to load profile: " + ex.Message;
                return View(new ProfileDto { FullName = HttpContext.Session.GetString("name") ?? "" });
            }
        }

        [HttpPost]
        public async Task<IActionResult> Save(
            string FullName,
            string? Bio,
            string? SkillsInput,
            string PreferredLevel)
        {
            try
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
            }
            catch (Exception ex)
            {
                TempData["Error"] = "Failed to save profile: " + ex.Message;
            }

            return RedirectToAction("Index");
        }
    }
}



