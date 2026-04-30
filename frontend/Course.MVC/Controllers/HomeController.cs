using Microsoft.AspNetCore.Mvc;

namespace Courses.Mvc.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
