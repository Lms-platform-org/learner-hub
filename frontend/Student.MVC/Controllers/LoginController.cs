using Microsoft.AspNetCore.Mvc;

namespace LearningPlatform.StudentService.WebApp.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Submit(string token, string name, string email)
        {
            HttpContext.Session.SetString("jwt", token?.Trim() ?? string.Empty);
            HttpContext.Session.SetString("name", name ?? string.Empty);
            HttpContext.Session.SetString("email", email ?? string.Empty);

            return RedirectToAction("Index", "Dashboard");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
