using Microsoft.AspNetCore.Mvc;

namespace SIMS_App.Controllers
{
    public class RecordController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
