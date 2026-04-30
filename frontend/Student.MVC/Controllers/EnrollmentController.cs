using LearningPlatform.StudentService.WebApp.Models.DTOs;
using LearningPlatform.StudentService.WebApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.StudentService.WebApp.Controllers
{
    public class EnrollmentController : Controller
    {
        private readonly EnrollmentApiService _enrollmentService;
        private readonly DiscoveryApiService _discoveryService;
        private readonly ProgressApiService _progressService;
        private readonly IConfiguration _config;

        public EnrollmentController(
            EnrollmentApiService enrollmentService,
            DiscoveryApiService discoveryService,
            ProgressApiService progressService,
            IConfiguration config)
        {
            _enrollmentService = enrollmentService;
            _discoveryService = discoveryService;
            _progressService = progressService;
            _config = config;
        }

        public async Task<IActionResult> MyCourses()
        {
            var courseIds = await _enrollmentService.GetEnrolledCourseIdsAsync();
            var viewModels = new List<EnrolledCourseViewModel>();

            foreach (var courseId in courseIds)
            {
                var course = await _discoveryService.GetCourseByIdAsync(courseId);
                var progress = await _progressService.GetAsync(courseId);

                viewModels.Add(new EnrolledCourseViewModel
                {
                    CourseId = courseId,
                    Title = course?.Title ?? $"Course #{courseId}",
                    Description = course?.Description,
                    Category = course?.Category,
                    Level = course?.Level,
                    ThumbnailUrl = course?.ThumbnailUrl,
                    InstructorName = course?.InstructorName,
                    VideoUrl = course?.VideoUrl,
                    TotalLessons = course?.TotalLessons ?? 0,
                    Percentage = progress?.Percentage ?? 0,
                    XP = progress?.XP ?? 0,
                    LastLessonId = progress?.LastLessonId ?? 1,
                    LastVideoTimestampSeconds = progress?.LastVideoTimestampSeconds ?? 0,
                    LastAccessed = progress?.LastAccessed,
                    EarnedMilestones = progress?.EarnedMilestones ?? new()
                });
            }

            ViewBag.CourseFrontendUrl = _config["ExternalUrls:CourseFrontend"] ?? "https://localhost:7081";

            return View(viewModels);
        }

        [HttpPost]
        public async Task<IActionResult> Enroll(int courseId)
        {
            await _enrollmentService.EnrollAsync(courseId);
            TempData["Success"] = $"Successfully enrolled in Course #{courseId}!";
            return RedirectToAction("Index", "Discovery");
        }

        [HttpPost]
        public async Task<IActionResult> EnrollAjax(int courseId)
        {
            try
            {
                await _enrollmentService.EnrollAsync(courseId);
                return Json(new { success = true, message = "Enrolled successfully" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
