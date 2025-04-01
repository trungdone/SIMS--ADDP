using Microsoft.AspNetCore.Mvc;
using SIMS_App.Models;
using SIMS_App.Services;

namespace SIMS_App.Controllers
{
    [Route("User")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        }

        [Route("AdminDashboard")]
        public IActionResult AdminDashboard()
        {
            return View("~/Views/Admin/DashboardAdmin.cshtml");
        }

        [Route("DashboardTeacher")]
        public IActionResult DashboardTeacher()
        {
            return View("~/Views/Teacher/DashboardTeacher.cshtml");
        }

        [Route("DashboardStudent")]
        public IActionResult DashboardStudent()
        {
            return View("~/Views/Student/DashboardStudent.cshtml");
        }


        [HttpGet]
        [Route("")]
        [Route("Index")]
        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("Role");

            if (role == "Teacher")
            {
                return RedirectToAction("TeacherDashboard");
            }
            else if (role == "Admin")
            {
                return RedirectToAction("AdminDashboard");
            }

            return RedirectToAction("Login", "Auth"); // Nếu không có quyền, quay lại trang đăng nhập
        }

        [HttpGet]
        [Route("GetUsers")]
        public JsonResult GetUsers()
        {
            var users = _userService.GetAllUsers();
            return Json(new { success = true, users });
        }


        [HttpPost]
        [Route("AddUser")]
        public JsonResult AddUser([FromBody] User user)
        {
            if (user == null)
                return Json(new { success = false, message = "Invalid data" });

            _userService.AddUser(user);
            return Json(new { success = true, message = "User added successfully" });
        }

        [HttpPost]
        [Route("UpdateUser")]
        public JsonResult UpdateUser([FromBody] User user)
        {
            if (user == null)
                return Json(new { success = false, message = "Invalid data" });

            _userService.UpdateUser(user);
            return Json(new { success = true, message = "User updated successfully" });
        }

        [HttpPost]
        [Route("DeleteUser")]
        public JsonResult DeleteUser([FromBody] int id)
        {
            _userService.DeleteUser(id);
            return Json(new { success = true, message = "User deleted successfully" });
        }

    }
}
