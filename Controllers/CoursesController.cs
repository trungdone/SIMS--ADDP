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
            Console.WriteLine($"Received class ID: {id}");
            var students = _courseService.GetStudentsByClassId(id);

            if (students != null && students.Any())
            {
                Console.WriteLine($"Total students found: {students.Count}");
                return Json(new
                {
                    success = true,
                    students = students.Select(s => new {
                        id = s.StudentId,  // Đổi thành lowercase để phù hợp với JavaScript
                        name = s.Name,
                        email = s.Email
                    }).ToList()
                });
            }
            else
            {
                Console.WriteLine("No students found for this class.");
                return Json(new
                {
                    success = false,
                    message = "No students found in this course."
                });
            }
        }

    }
}
