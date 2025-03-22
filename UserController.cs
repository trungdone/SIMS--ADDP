using Microsoft.AspNetCore.Mvc;

namespace SIMS_App.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
