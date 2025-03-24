using Microsoft.AspNetCore.Mvc;
using SIMS_App.Data;
using SIMS_App.Models;
using System.Linq;

namespace SIMS_App.Controllers
{
    public class ClassController : Controller
    {
        private readonly IDataService _dataService;

        public ClassController(IDataService dataService)
        {
            _dataService = dataService;
        }

        public IActionResult ManageClass()
        {
            var classes = _dataService.GetClasses();
            return View("~/Views/Admin/ManageClass.cshtml", classes); 
        }


        [HttpPost]
        public IActionResult AddClass(string name, int courseId)
        {
            var newClass = new Class { ClassId = _dataService.GetClasses().Count + 1, Name = name, CourseId = courseId };
            _dataService.AddClass(newClass);
            return RedirectToAction("Index");
        }

        public IActionResult Edit(int id)
        {
            var classItem = _dataService.GetClassById(id);
            if (classItem == null) return NotFound();
            return View(classItem);
        }

        public IActionResult DeleteClass(int id)
        {
            _dataService.DeleteClass(id);
            return RedirectToAction("ManageClasses");
        }

        public IActionResult ViewStudents(int id)
        {
            var classItem = _dataService.GetClassById(id);
            if (classItem == null) return NotFound();
            return View(classItem.Students);
        }
    }
}
