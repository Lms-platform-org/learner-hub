using Horizon.MVC.DTOs;
using Horizon.MVC.ViewModels;
using Horizon.MVC.Models;
using System.Security.Claims;
using Horizon.MVC.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace Horizon.MVC.Controllers
{
    public class AuthController : Controller
    {
        private readonly ApiService _apiService;
        private readonly IConfiguration _configuration;

        public AuthController(ApiService apiService, IConfiguration configuration)
        {
            _apiService = apiService;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["HideSidebar"] = true;
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            ViewData["HideSidebar"] = true;
            if (!ModelState.IsValid) return View(model);

            var result = await _apiService.LoginAsync(model);

            if (result.Success)
            {
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, result.Name ?? model.Email),
                    new Claim(ClaimTypes.Email, model.Email),
                    new Claim("Token", result.Token ?? "")
                };

                foreach (var role in result.Roles)
                {
                    claims.Add(new Claim(ClaimTypes.Role, role));
                }

                if (result.Roles.Length == 0)
                {
                    claims.Add(new Claim(ClaimTypes.Role, "Student"));
                }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new AuthenticationProperties { IsPersistent = true };

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity), authProperties);

                if (result.Roles.Contains("Admin"))
                {
                    return RedirectToAction("Dashboard", "Admin");
                }
                return Redirect($"{_configuration["AppUrls:StudentMvcBaseUrl"] ?? "https://localhost:7009/"}?name={Uri.EscapeDataString(result.Name ?? model.Email)}&email={Uri.EscapeDataString(model.Email)}&token={Uri.EscapeDataString(result.Token ?? "")}");
            }

            ModelState.AddModelError("", result.Message ?? "Invalid login attempt");
            return View(model);
        }

        [HttpGet]
        public IActionResult Register()
        {
            ViewData["HideSidebar"] = true;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            ViewData["HideSidebar"] = true;
            if (!ModelState.IsValid) return View(model);

            var result = await _apiService.RegisterAsync(model);

            if (result.Success)
            {
                ViewData["Message"] = "Your account has been created successfully!";
                return View("LoginSuccess");
            }

            ModelState.AddModelError("", result.Message ?? "Registration failed");
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
    }
}


