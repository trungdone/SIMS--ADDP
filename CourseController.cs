using Microsoft.AspNetCore.Mvc;

namespace SIMS_App.Controllers
{
    public class CourseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
