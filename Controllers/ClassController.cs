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
        public JsonResult AddClass(string name, int courseId)
        {
            var newClass = new Class { Name = name, CourseId = courseId };
            _dataService.AddClass(newClass);
            return Json(new { success = true, message = "Class added successfully!" });
        }

        [HttpPost]
        public JsonResult Edit(int id, string name, int courseId)
        {
            var classItem = _dataService.GetClassById(id);
            if (classItem != null)
            {
                classItem.Name = name;
                classItem.CourseId = courseId;
                _dataService.UpdateClass(classItem);
                return Json(new { success = true, message = "Class updated successfully!" });
            }
            return Json(new { success = false, message = "Class not found!" });
        }

        [HttpPost]
        public JsonResult DeleteClass(int id)
        {
            _dataService.DeleteClass(id);
            return Json(new { success = true, message = "Class deleted successfully!" });
        }


        [HttpGet]

        public JsonResult ViewStudents(int id)
        {
            Console.WriteLine($"Received Class ID: {id}"); // Debug ID nhận vào

            if (id <= 0)
                return Json(new { success = false, message = "Invalid Class ID!" });

            var classItem = _dataService.GetClassById(id);
            if (classItem == null)
                return Json(new { success = false, message = "Class not found!" });

            var students = _dataService.GetStudentsByClassId(id)
                .Select(s => new { s.StudentId, s.Name, s.Age, s.Email })
                .ToList();

            if (students == null || !students.Any())
                return Json(new { success = false, message = "No students found in this class!" });

            return Json(new { success = true, students });
        }


    }
}
