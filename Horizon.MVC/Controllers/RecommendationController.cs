using Horizon.MVC.DTOs;
using Horizon.MVC.ViewModels;
using Horizon.MVC.Models;
using Horizon.MVC.Services;
using Microsoft.AspNetCore.Mvc;

namespace Horizon.MVC.Controllers
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

