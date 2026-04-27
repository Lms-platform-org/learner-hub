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
            if (string.IsNullOrWhiteSpace(name) || name.Trim().Length < 2)
            {
                TempData["Error"] = "Please enter your name";
                return View("Index");
            }

            if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
            {
                TempData["Error"] = "Please enter a valid email";
                return View("Index");
            }

            HttpContext.Session.SetString("jwt", token?.Trim() ?? string.Empty);
            HttpContext.Session.SetString("name", name.Trim());
            HttpContext.Session.SetString("email", email.Trim());

            return RedirectToAction("Index", "Dashboard");
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
