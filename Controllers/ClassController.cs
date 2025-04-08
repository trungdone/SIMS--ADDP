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

        [HttpGet]
        public IActionResult ViewClass()
        {
            var classes = _dataService.GetClasses();
            return View("~/Views/Student/ViewClass.cshtml", classes);
        }

        [HttpGet]
        public JsonResult ViewStudents(int id)
        {
            var students = _dataService.GetStudentsByClassId(id);

            if (students == null || !students.Any())
                return Json(new { success = false, message = "No students found in this class!" });

            return Json(new
            {
                success = true,
                students = students.Select(s => new {
                    id = s.StudentId,
                    name = s.Name,
                    email = s.Email
                }).ToList()
            });
        }




        [HttpPost]
        public JsonResult AddClass(string name, int courseId)
        {
            try
            {
                var newClass = new Class
                {
                    Name = name,
                    CourseId = courseId
                    // Không cần gán Id vì đã được xử lý trong DataService
                };

                _dataService.AddClass(newClass);

                return Json(new
                {
                    success = true,
                    message = "Class added successfully!",
                    newClassId = newClass.Id // Trả về ID mới tạo
                });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = $"Error adding class: {ex.Message}"
                });
            }
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

        [HttpPost]
        public JsonResult AddStudentToClass([FromBody] Student student)
        {
            _dataService.AddStudentToClass(student);
            return Json(new { success = true, message = "Student added successfully!" });
        }

        [HttpGet]
        public JsonResult GetAllStudents(int classId)
        {
            var students = _dataService.GetAllStudentsNotInClass(classId);
            return Json(students);
        }


        [HttpPost]
        public JsonResult AssignStudentToClass([FromBody] AssignStudentModel model)
        {
            _dataService.AssignStudentToClass(model.StudentId, model.ClassId);
            return Json(new { success = true, message = "Student assigned successfully!" });
        }





    }
}
