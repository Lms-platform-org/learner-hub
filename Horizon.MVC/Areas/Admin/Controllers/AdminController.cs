using Horizon.MVC.DTOs;
using Horizon.MVC.ViewModels;
using Horizon.MVC.Models;
using Horizon.MVC.Models;
using Horizon.MVC.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Horizon.MVC.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class AdminController : Controller
    {
        private readonly ApiService _apiService;

        public AdminController(ApiService apiService)
        {
            _apiService = apiService;
        }

        private string GetToken()
        {
            return User.FindFirst("Token")?.Value ?? "";
        }

        public async Task<IActionResult> Dashboard()
        {
            var token = GetToken();
            var model = new AdminDashboardViewModel
            {
                PendingTeachers = await _apiService.GetPendingTeachersAsync(token),
                ApprovedUsers = await _apiService.GetApprovedUsersAsync(token)
            };
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ApproveTeacher([FromForm] string id, [FromForm] string notes)
        {
            var token = GetToken();
            var success = await _apiService.ApproveTeacherAsync(token, id, notes);
            if (success)
            {
                TempData["Message"] = "Teacher approved successfully.";
            }
            else
            {
                TempData["Error"] = "Failed to approve teacher.";
            }
            return RedirectToAction(nameof(Dashboard));
        }

        [HttpPost]
        public async Task<IActionResult> RejectTeacher([FromForm] string id, [FromForm] string notes)
        {
            var token = GetToken();
            var success = await _apiService.RejectTeacherAsync(token, id, notes);
            if (success)
            {
                TempData["Message"] = "Teacher rejected successfully.";
            }
            else
            {
                TempData["Error"] = "Failed to reject teacher.";
            }
            return RedirectToAction(nameof(Dashboard));
        }
    }
}



