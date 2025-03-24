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
            _userService = userService;
        }

        [Route("")]
        [Route("Index")]
        public IActionResult Index()
        {
            var users = _userService.GetAllUsers();
            return View("~/Views/Admin/DashboardAdmin.cshtml", users);
        }

        [HttpGet]
        [Route("ManageUser/{id?}")]
        public IActionResult ManageUser(int? id)
        {
            User user;
            if (id.HasValue)
            {
                user = _userService.GetUserById(id.Value);
                if (user == null)
                    return NotFound();
            }
            else
            {
                user = new User();
            }

            return View("~/Views/Admin/ManageUsers.cshtml", user);
        }

        [HttpPost]
        [Route("ManageUser/{id?}")]
        public IActionResult ManageUser(User user)
        {
            if (ModelState.IsValid)
            {
                if (user.Id == 0)
                {
                    _userService.AddUser(user);
                }
                else
                {
                    _userService.UpdateUser(user);
                }
                return RedirectToAction("Index");
            }
            return View("~/Views/Admin/ManageUsers.cshtml", user);
        }

        [Route("Delete/{id}")]
        public IActionResult Delete(int id)
        {
            _userService.DeleteUser(id);
            return RedirectToAction("Index");
        }
    }
}