using Microsoft.AspNetCore.Mvc;

namespace SIMS_App.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            var role = HttpContext.Session.GetString("Role") ?? "Guest";

            if (role == "Teacher")
            {
                return RedirectToAction("DashboardTeacher", "User"); 
            }
            else if (role == "Admin")
            {
                return RedirectToAction("AdminDashboard", "User");
            }
            else if (role == "Student") 
            {
                return RedirectToAction("DashboardStudent", "User");
            }

            return RedirectToAction("Login", "Auth");
        }
    }
}
