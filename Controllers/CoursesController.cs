using Microsoft.AspNetCore.Mvc;
using SIMS_App.Models;
using SIMS_App.Services;
using System.Collections.Generic;

namespace SIMS_App.Controllers
{
    [Route("Courses")]
    public class CourseController : Controller
    {
        private readonly CourseService _courseService;

        public CourseController()
        {
            _courseService = new CourseService();
        }


  
            [Route("ManageCourses")]
            public IActionResult Index()
            {
                List<Course> courses = _courseService.GetCourses();
                return View("~/Views/Admin/ManageCourses.cshtml", courses);
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

        [HttpDelete]
        [Route("DeleteCourse")]
        public IActionResult DeleteCourse(int id)
        {
            _courseService.DeleteCourse(id);
            return Json(new { success = true, message = "Course deleted successfully!" });
        }

        [HttpGet]
        [Route("ViewStudents")]
        public IActionResult ViewStudents(int id)
        {
            var students = _courseService.GetStudentsByCourseId(id);

            if (students.Any())
            {
                return Json(new { success = true, students });
            }
            else
            {
                return Json(new { success = false, message = "No students found in this course." });
            }
        }

    }
}
