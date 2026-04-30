using LearningPlatform.StudentService.WebApp.Services;
using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.StudentService.WebApp.Controllers
{
    public class RecommendationController : Controller
    {
        private readonly RecommendationApiService _service;

        public RecommendationController(RecommendationApiService service)
        {
            _service = service;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _service.GetPersonalizedAsync();
            return View(data);
        }
    }
}