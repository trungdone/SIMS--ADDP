using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
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



        [HttpGet("Login")]  // ✅ Định nghĩa đường dẫn chính xác
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost("Login")]
        public IActionResult Login(LoginModel model)
        {

            if (ModelState.IsValid)
            {
                var user = _authService.ValidateUser(model.Username, model.Password);
                

                if (user == null)
                {
                    Console.WriteLine($"Login failed for user: {model.Username}");
                    ModelState.AddModelError("", "Invalid username or password.");
                    return View(model);
                }

                // ✅ Lưu thông tin user vào Session
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("Role", user.Role);

                Console.WriteLine($"Login successful for user: {user.Username}");

                // ✅ Chuyển hướng sau khi đăng nhập thành công
                return RedirectToAction("Index", "Home");

                if (user != null)
                {
                    HttpContext.Session.SetString("Role", user.Role);
                    return RedirectToAction("Index", "Home");
                }

                ModelState.AddModelError("", "Invalid credentials");
            }

            return View(model);
        }



        [HttpGet("Register")]
        public IActionResult Register()
        {
            return View();
        }

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
            
            // Đăng nhập thành công - chuyển hướng về trang chủ
            return RedirectToAction("Index", "Home");
        }


        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }


    }
}




