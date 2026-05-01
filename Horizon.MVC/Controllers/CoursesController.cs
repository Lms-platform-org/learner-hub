using Horizon.MVC.DTOs;
using Horizon.MVC.ViewModels;
using Horizon.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace Horizon.MVC.Controllers
{
    public class CoursesController : Controller
    {
        private readonly HttpClient _httpClient;

        public CoursesController(IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient("LearningApi");
        }

        // GET: Courses
        public async Task<IActionResult> Index()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/CoursesApi");
                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<CourseReadDTO>>>();
                    return View(apiResponse?.Data ?? new List<CourseReadDTO>());
                }
                return View(new List<CourseReadDTO>());
            }
            catch (Exception)
            {
                return View(new List<CourseReadDTO>());
            }
        }

        // GET: Courses/TeacherDashboard
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> TeacherDashboard()
        {
            try
            {
                var response = await _httpClient.GetAsync("api/CoursesApi/TeacherIndex");
                
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    return RedirectToAction("Login", "Account");
                }

                if (response.IsSuccessStatusCode)
                {
                    var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<List<CourseReadDTO>>>();
                    return View(apiResponse?.Data ?? new List<CourseReadDTO>());
                }

                ModelState.AddModelError("", "Failed to retrieve courses from the API.");
                return View(new List<CourseReadDTO>());
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return View(new List<CourseReadDTO>());
            }
        }

        // GET: Courses/Details/5
        public async Task<IActionResult> Details(int id)
        {
            CourseReadDTO? course = null;
            try
            {
                var apiResponse = await _httpClient.GetFromJsonAsync<ApiResponse<CourseReadDTO>>($"api/CoursesApi/{id}");
                course = apiResponse?.Data;
            }
            catch (Exception)
            {
                return NotFound();
            }

            if (course == null)
            {
                return NotFound();
            }

            // Check if student is enrolled
            var currentUserId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            try
            {
                var apiResponse = await _httpClient.GetFromJsonAsync<ApiResponse<List<Enrollment>>>($"api/EnrollmentsApi/Course/{id}");
                var enrollments = apiResponse?.Data;
                ViewBag.IsEnrolled = enrollments?.Any(e => e.StudentId == currentUserId || e.StudentId == "jayraj") ?? false;
            }
            catch (Exception)
            {
                ViewBag.IsEnrolled = false;
            }

            // Get comments via API
            try
            {
                var apiResponse = await _httpClient.GetFromJsonAsync<ApiResponse<List<CommentResponseDto>>>($"api/CommentsApi/Course/{id}");
                var comments = apiResponse?.Data;
                ViewBag.Comments = comments ?? new List<CommentResponseDto>();
            }
            catch (Exception)
            {
                ViewBag.Comments = new List<CommentResponseDto>();
            }

            return View(course);
        }

        // GET: Courses/Create
        [Authorize(Roles = "Teacher")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Courses/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Create(CourseWriteDTO courseDto)
        {
            if (ModelState.IsValid)
            {
                var response = await _httpClient.PostAsJsonAsync("api/CoursesApi", courseDto);
                
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(TeacherDashboard));
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.Conflict)
                {
                    ModelState.AddModelError("", "A course with this title already exists for you.");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", "Failed to create course. Please check your inputs.");
                    // Log errorContent for debugging if needed
                }
            }
            return View(courseDto);
        }

        // GET: Courses/Edit/5
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var response = await _httpClient.GetAsync($"api/CoursesApi/{id}");
                
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return NotFound();
                }

                response.EnsureSuccessStatusCode();
                var course = await response.Content.ReadFromJsonAsync<CourseReadDTO>();
                
                if (course == null) return NotFound();
            
                // Map ReadDTO to WriteDTO for the form
                var editModel = new CourseWriteDTO
                {
                    Title = course.Title,
                    Description = course.Description,
                    Price = course.Price,
                    Category = course.Category,
                    Duration = course.Duration,
                    ThumbnailUrl = course.ThumbnailUrl,
                    StartDate = course.StartDate,
                    IsPublished = course.IsPublished
                };
                
                ViewBag.Id = course.Id;
                ViewBag.Status = course.Status;
                ViewBag.CreatedAt = course.CreatedAt;
                return View(editModel);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"An error occurred while retrieving the course: {ex.Message}");
                return RedirectToAction(nameof(TeacherDashboard));
            }
        }

        // POST: Courses/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Edit(int id, CourseWriteDTO courseInput)
        {
            if (ModelState.IsValid)
            {
                var response = await _httpClient.PutAsJsonAsync($"api/CoursesApi/{id}", courseInput);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(TeacherDashboard));
                }
                
                ModelState.AddModelError("", "Failed to update course. Please check your inputs.");
            }
            return View(courseInput);
        }

        // POST: Courses/Publish/5
        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> Publish(int id)
        {
            var response = await _httpClient.PostAsync($"api/CoursesApi/{id}/publish", null);
            return RedirectToAction(nameof(TeacherDashboard));
        }

        // POST: Courses/AddVideo
        [HttpPost]
        [Authorize(Roles = "Teacher")]
        public async Task<IActionResult> AddVideo(int courseId, string videoUrl)
        {
            if (!string.IsNullOrEmpty(videoUrl))
            {
                await _httpClient.PostAsJsonAsync($"api/CoursesApi/{courseId}/videos", videoUrl);
            }
            return RedirectToAction(nameof(Details), new { id = courseId });
        }

        // POST: Courses/AddComment
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddComment(int courseId, string content)
        {
            if (!string.IsNullOrEmpty(content))
            {
                var dto = new CreateCommentDto
                {
                    CourseId = courseId,
                    Content = content
                };
                await _httpClient.PostAsJsonAsync("api/CommentsApi", dto);
            }
            return RedirectToAction(nameof(Details), new { id = courseId });
        }

        // POST: Courses/Enroll
        [HttpPost]
        [Authorize(Roles = "Student")]
        public async Task<IActionResult> Enroll(int courseId)
        {
            var request = new { CourseId = courseId };
            var response = await _httpClient.PostAsJsonAsync("api/EnrollmentsApi", request);

            if (response.IsSuccessStatusCode)
            {
                TempData["Success"] = "Successfully enrolled in the course!";
            }
            else
            {
                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<object>>();
                TempData["Error"] = apiResponse?.Message ?? "Enrollment failed. Please try again.";
            }

            return RedirectToAction(nameof(Details), new { id = courseId });
        }
    }
}



