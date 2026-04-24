using Microsoft.AspNetCore.Mvc;
using LearningPlatform.StudentService.WebApp.Models;
using LearningPlatform.StudentService.WebApp.Services;
using Microsoft.AspNetCore.Mvc;
using LearningPlatform.StudentService.WebApp.Models.DTOs;

namespace LearningPlatform.StudentService.WebApp.Controllers
{
    public class BookmarksController : Controller
    {
        private readonly BookmarkApiService _service;

        public BookmarksController(BookmarkApiService service)
        {
            _service = service;
        }

        // LIST
        public async Task<IActionResult> Index(int page = 1)
        {
            var data = await _service.GetAllAsync(page, 10);
            return View(data);
        }

        // CREATE (GET)
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // CREATE (POST)
        [HttpPost]
        public async Task<IActionResult> Create(BookmarkViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _service.AddAsync(new BookmarkDto
            {
                CourseId = model.CourseId,
                Category = model.Category,
                PersonalNote = model.PersonalNote
            });

            return RedirectToAction("Index");
        }

        // EDIT (GET)
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var data = await _service.GetByIdAsync(id);

            var vm = new BookmarkViewModel
            {
                Id = data.Id,
                CourseId = data.CourseId ?? 0,
                Category = data.Category,
                PersonalNote = data.PersonalNote
            };

            return View(vm);
        }

        // EDIT (POST)
        [HttpPost]
        public async Task<IActionResult> Edit(int id, BookmarkViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            await _service.UpdateAsync(id, new BookmarkDto
            {
                CourseId = model.CourseId,
                Category = model.Category,
                PersonalNote = model.PersonalNote
            });

            return RedirectToAction("Index");
        }

        // DELETE
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}
