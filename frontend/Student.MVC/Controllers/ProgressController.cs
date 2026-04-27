using LearningPlatform.StudentService.WebApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.StudentService.WebApp.Controllers
{
    public class ProgressController : Controller
    {
        private readonly ProgressApiService _service;

        public ProgressController(ProgressApiService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index(int courseId)
        {
            var data = await _service.GetAsync(courseId);
            ViewBag.CourseId = courseId;

            if (data == null)
            {
                TempData["Info"] = "No progress found for this course yet. Start watching to track progress!";
                return View(new LearningPlatform.StudentService.WebApp.Models.DTOs.StudentProgress());
            }

            return View(data);
        }

        public async Task<IActionResult> Complete(int courseId, int lessonId)
        {
            await _service.CompleteAsync(courseId, lessonId);
            return RedirectToAction("Index", new { courseId });
        }
    }
}