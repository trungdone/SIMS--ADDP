using Microsoft.AspNetCore.Mvc;
using SIMS_App.Data;
using SIMS_App.Models;
using SIMS_App.Services;
using System.Collections.Generic;

namespace SIMS_App.Controllers
{
    [Route("Courses")]
    public class CourseController : Controller
    {
        private readonly CourseService _courseService;
        private readonly IDataService _dataService;
        public CourseController(IDataService dataService)
        {
            _courseService = new CourseService();
            _dataService = dataService;
        }




        [Route("ManageCourses")]
            public IActionResult Index()
            {
                List<Course> courses = _courseService.GetCourses();
                return View("~/Views/Admin/ManageCourses.cshtml", courses);
            }


        [Route("ViewCourses")]
        public IActionResult ViewCourses()
        {
            var courses = _courseService.GetCourses();
            return View("~/Views/Student/ViewCourses.cshtml", courses);
        }

        [HttpPost]
        [Route("AddCourse")]
        public IActionResult AddCourse([FromBody] Course course)
        {
            _courseService.AddCourse(course);
            return Json(new { success = true, message = "Course added successfully!" });
        }

       




        [HttpPut]
        [Route("UpdateCourse")]
        public IActionResult UpdateCourse([FromBody] Course course)
        {
            _courseService.UpdateCourse(course);
            return Json(new { success = true, message = "Course updated successfully!" });
        }

        [HttpPost]
        [Route("DeleteCourse")]
        public IActionResult DeleteCourse([FromBody] int courseId)
        {
            Console.WriteLine($"🔧 Attempting to delete course with ID: {courseId}");

            try
            {
                _courseService.DeleteCourse(courseId);
                return Json(new { success = true, message = "✅ Course deleted successfully!" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ ERROR during deletion: {ex.Message}");
                return Json(new { success = false, message = "❌ Server error: " + ex.Message });
            }
        }



        [HttpPost]
        [Route("AssignStudentToCourse")]
        public JsonResult AssignStudentToCourse([FromBody] AssignStudentModel model)
        {
            _dataService.AssignStudentToCourse(model.StudentId, model.CourseId);
            return Json(new { success = true, message = "✅ Student assigned successfully!" });
        }



        [HttpGet]
        [Route("ViewStudents")]
        public IActionResult ViewStudents(int id)  // id = courseId
        {
            Console.WriteLine($"📥 ViewStudents called with CourseId = {id}");

            var classObj = _dataService.GetClasses().FirstOrDefault(c => c.CourseId == id);
            if (classObj == null)
            {
                Console.WriteLine("❌ No class found for the provided course.");
                return Json(new { success = false, message = "No class found for this course." });
            }

            var students = _dataService.GetStudentsByClassId(classObj.Id);
            if (students == null || !students.Any())
            {
                Console.WriteLine("⚠ No students found in the associated class.");
                return Json(new { success = false, message = "No students found in this course." });
            }

            Console.WriteLine($"✅ Found {students.Count} student(s) for CourseId = {id}");

            return Json(new
            {
                success = true,
                students = students.Select(s => new
                {
                    id = s.StudentId,
                    name = s.Name,
                    email = s.Email
                }).ToList()
            });
        }

        [HttpPost]
        [Route("RemoveStudentFromCourse")]
        public JsonResult RemoveStudentFromCourse([FromBody] AssignStudentModel model)
        {
            try
            {
                _dataService.RemoveStudentFromCourse(model.StudentId, model.CourseId);
                return Json(new { success = true, message = "✅ Student removed from course!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "❌ Error: " + ex.Message });
            }
        }




    }
}
