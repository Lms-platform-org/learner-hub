using LearningPlatform.StudentService.WebApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.StudentService.WebApp.Controllers
{
    public class EnrollmentController : Controller
    {
        private readonly EnrollmentApiService _service;

        public EnrollmentController(EnrollmentApiService service)
        {
            _service = service;
        }

        [HttpPost]
        public async Task<IActionResult> Enroll(int courseId)
        {
            await _service.EnrollAsync(courseId);
            TempData["Success"] = $"Successfully enrolled in Course #{courseId}!";
            return RedirectToAction("Index", "Discovery");
        }
    }
}
