using Microsoft.AspNetCore.Mvc;

namespace SIMS_App.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
