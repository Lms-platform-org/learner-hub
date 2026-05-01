using Horizon.MVC.DTOs;
using Horizon.MVC.ViewModels;
using Horizon.MVC.Models;
using Horizon.MVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace Horizon.MVC.Controllers
{
    public class DashboardController : Controller
    {
        private readonly DashboardApiService _service;

        public DashboardController(DashboardApiService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index(string? token = null, string? name = null, string? email = null)
        {
            if (!string.IsNullOrWhiteSpace(token))
                HttpContext.Session.SetString("jwt", token);

            if (!string.IsNullOrWhiteSpace(name))
                HttpContext.Session.SetString("name", name);

            if (!string.IsNullOrWhiteSpace(email))
                HttpContext.Session.SetString("email", email);

            if (!string.IsNullOrWhiteSpace(token) || !string.IsNullOrWhiteSpace(name) || !string.IsNullOrWhiteSpace(email))
                return RedirectToAction(nameof(Index));

            var data = await _service.GetAsync();

            var allMilestones = data?.ContinueWatching?
                .SelectMany(p => p.EarnedMilestones ?? new())
                .Concat(data?.Progress?.SelectMany(p => p.EarnedMilestones ?? new()) ?? new List<string>())
                .Distinct()
                .ToList() ?? new List<string>();

            var vm = new DashboardViewModel
            {
                EnrolledCount    = data?.EnrolledCourseIds?.Count ?? 0,
                CompletedCount   = data?.Progress?.Count(p => p.Percentage == 100) ?? 0,
                InProgressCount  = data?.ContinueWatching?.Count ?? 0,
                TotalBookmarks   = data?.TotalBookmarks ?? 0,
                TotalXP          = data?.TotalXP ?? 0,
                RecommendedCourseIds = data?.RecommendedCourses ?? new(),
                Milestones       = allMilestones,
                ContinueWatching = data?.ContinueWatching?.Take(5).Select(p => new ProgressViewModel
                {
                    Percentage               = p.Percentage,
                    LastLessonId             = p.LastLessonId,
                    LastVideoTimestampSeconds = p.LastVideoTimestampSeconds,
                    XP                       = p.XP,
                    EarnedMilestones         = p.EarnedMilestones ?? new()
                }).ToList() ?? new()
            };

            return View(vm);
        }
    }
}

