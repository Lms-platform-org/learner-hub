using LearningPlatform.StudentService.WebApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.StudentService.WebApp.Controllers
{
    public class DashboardController : Controller
    {
        private readonly DashboardApiService _service;

        public DashboardController(DashboardApiService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _service.GetAsync();
            return View(data);
        }
    }
}