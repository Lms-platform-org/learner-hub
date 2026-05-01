using Horizon.MVC.DTOs;
using Horizon.MVC.ViewModels;
using Horizon.MVC.Models;
using Microsoft.AspNetCore.Mvc;
using Horizon.MVC.Services;

namespace Horizon.MVC.Controllers
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

        [HttpGet]
        public async Task<IActionResult> GetAllAjax()
        {
            try
            {
                var data = await _service.GetAllAsync(1, 200);
                return Json(data);
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAjax(int id)
        {
            try
            {
                await _service.DeleteAsync(id);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateNote(int id, [FromBody] UpdateNoteRequest req)
        {
            try
            {
                await _service.UpdateNoteAsync(id, req.Note);
                return Ok();
            }
            catch
            {
                return BadRequest();
            }
        }
    }

    public class UpdateNoteRequest
    {
        public string? Note { get; set; }
    }
}



