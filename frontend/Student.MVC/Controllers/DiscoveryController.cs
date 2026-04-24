using LearningPlatform.StudentService.WebApp.Models.DTOs;
using LearningPlatform.StudentService.WebApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.StudentService.WebApp.Controllers
{
    public class DiscoveryController : Controller
    {
        private readonly DiscoveryApiService _discoveryService;
        private readonly BookmarkApiService _bookmarkService;

        public DiscoveryController(DiscoveryApiService discoveryService, BookmarkApiService bookmarkService)
        {
            _discoveryService = discoveryService;
            _bookmarkService = bookmarkService;
        }

        // GET /Discovery — browse all courses
        public async Task<IActionResult> Index(int page = 1)
        {
            var courses = await _discoveryService.GetAllCoursesAsync(page, 9);
            return View(courses);
        }

        // GET /Discovery/Search?query=python
        public async Task<IActionResult> Search(string query, int page = 1)
        {
            if (string.IsNullOrWhiteSpace(query))
                return RedirectToAction("Index");

            var courses = await _discoveryService.SearchCoursesAsync(query, page, 9);
            ViewBag.Query = query;
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

        // POST /Discovery/Bookmark — bookmark a course directly from discovery
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
    }
}
