using Horizon.MVC.DTOs;
using Horizon.MVC.ViewModels;
using Horizon.MVC.Models;
using Horizon.MVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace Horizon.MVC.Controllers
{
    public class DiscoveryController : Controller
    {
        private readonly DiscoveryApiService _discoveryService;
        private readonly BookmarkApiService _bookmarkService;
        private readonly IConfiguration _config;

        public DiscoveryController(
            DiscoveryApiService discoveryService,
            BookmarkApiService bookmarkService,
            IConfiguration config)
        {
            _discoveryService = discoveryService;
            _bookmarkService = bookmarkService;
            _config = config;
        }

        public async Task<IActionResult> Index(int page = 1)
        {
            ViewBag.CourseFrontendUrl = _config["ExternalUrls:CourseFrontend"] ?? "https://localhost:7081";
            var courses = await _discoveryService.GetAllCoursesAsync(page, 9);
            return View(courses);
        }

        public async Task<IActionResult> Search(string query, int page = 1)
        {
            if (string.IsNullOrWhiteSpace(query))
                return RedirectToAction("Index");

            ViewBag.CourseFrontendUrl = _config["ExternalUrls:CourseFrontend"] ?? "https://localhost:7081";
            ViewBag.Query = query;
            var courses = await _discoveryService.SearchCoursesAsync(query, page, 9);
            return View("Index", courses);
        }

        // GET /Discovery/Books?topic=python
        public async Task<IActionResult> Books(string topic, int page = 1)
        {
            ViewBag.Topic = topic;

            if (string.IsNullOrWhiteSpace(topic))
                return View(new PagedResponseDto<BookDto>());

            var books = await _discoveryService.SearchBooksAsync(topic, page, 10);
            return View(books);
        }

        // POST /Discovery/Bookmark — bookmark a course (form post)
        [HttpPost]
        public async Task<IActionResult> Bookmark(int courseId, string category, string? note)
        {
            await _bookmarkService.AddAsync(new BookmarkDto
            {
                CourseId = courseId,
                Type = "course",
                Category = string.IsNullOrWhiteSpace(category) ? "general" : category,
                PersonalNote = note
            });
            TempData["Success"] = "Course bookmarked!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> BookmarkAjax([FromBody] BookmarkAjaxRequest req)
        {
            try
            {
                var isBookmarked = await _bookmarkService.ToggleAsync(new BookmarkDto
                {
                    CourseId = req.CourseId,
                    BookKey = req.BookKey,
                    BookTitle = req.BookTitle,
                    BookAuthor = req.BookAuthor,
                    Type = req.Type ?? "course"
                });
                return Json(new { isBookmarked });
            }
            catch (Exception ex)
            {
                return Json(new { isBookmarked = false, message = ex.Message });
            }
        }

        // POST /Discovery/BookmarkBook — bookmark a book from books page
        [HttpPost]
        public async Task<IActionResult> BookmarkBook(string bookKey, string bookTitle, string? bookAuthor, string category, string? note)
        {
            await _bookmarkService.AddAsync(new BookmarkDto
            {
                BookKey = bookKey,
                BookTitle = bookTitle,
                BookAuthor = bookAuthor,
                Type = "book",
                Category = string.IsNullOrWhiteSpace(category) ? "general" : category,
                PersonalNote = note
            });

            TempData["Success"] = $"'{bookTitle}' bookmarked!";
            return RedirectToAction("Books");
        }

        [HttpGet]
        public async Task<IActionResult> CheckBookmark(int? courseId, string? bookKey, string type)
        {
            try
            {
                var dto = new BookmarkDto { Type = type, CourseId = courseId, BookKey = bookKey };
                var isBookmarked = await _bookmarkService.IsBookmarkedAsync(dto);
                return Json(new { isBookmarked });
            }
            catch
            {
                return Json(new { isBookmarked = false });
            }
        }
    }
}


