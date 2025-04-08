using Microsoft.AspNetCore.Mvc;
using SIMS_App.Models;
using SIMS_App.Services;

namespace SIMS_App.Controllers
{
    [Route("Auth")]
    public class AuthController : Controller
    {
        private readonly AuthService _authService;

        public AuthController()
        {
            _authService = new AuthService();
        }

        // 🔐 GET: Login Page
        [HttpGet("Login")]
        public IActionResult Login()
        {
            return View();
        }

        // 🔐 POST: Login
        [HttpPost("Login")]
        public IActionResult Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _authService.ValidateUser(model.Username, model.Password);

                if (user == null)
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                    return View(model);
                }

                // ✅ Save session data
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("Role", user.Role);
                HttpContext.Session.SetString("UserId", user.Id.ToString());

                Console.WriteLine($"✅ Login success: {user.Username}, Role: {user.Role}, ID: {user.Id}");

                return RedirectToAction("Index", "Home");
            }

            return View(model);
        }

        // 🧾 GET: Register Page
        [HttpGet("Register")]
        public IActionResult Register()
        {
            return View();
        }

        // 🧾 POST: Register
        [HttpPost("Register")]
        public IActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                var result = _authService.RegisterUser(model);

                if (result)
                {
                    return RedirectToAction("Login");
                }

                ModelState.AddModelError("", "Registration failed. Username may already exist.");
            }

            return View(model);
        }

        // 🚪 Logout
        [HttpGet("Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}
