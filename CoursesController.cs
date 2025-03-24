using Microsoft.AspNetCore.Mvc;
using SIMS_App.Data;
using SIMS_App.Models;
using System.Collections.Generic;

namespace SIMS_App.Controllers
{
    public class CoursesController : Controller
    {
        private readonly IDataService _dataService;

        public CoursesController(IDataService dataService)
        {
            _dataService = dataService;
        }

        public IActionResult ManageCourses()
        {
            var courses = _dataService.GetCourses();
            return View("~/Views/Admin/ManageCourses.cshtml", courses);
        }

        // Hiển thị danh sách khóa học
        public IActionResult Index()
        {
            List<Course> courses = _dataService.GetCourses();
            return View(courses);
        }

        // Hiển thị form thêm khóa học
        public IActionResult Create()
        {
            return View();
        }

        // Xử lý thêm khóa học
        [HttpPost]
        public IActionResult Create(Course course)
        {
            if (ModelState.IsValid)
            {
                _dataService.AddCourse(course);
                return RedirectToAction("Index");
            }
            return View(course);
        }

        // Hiển thị form chỉnh sửa khóa học
        public IActionResult Edit(int id)
        {
            Course course = _dataService.GetCourseById(id);
            if (course == null) return NotFound();
            return View(course);
        }

        // Xử lý cập nhật khóa học
        [HttpPost]
        public IActionResult Edit(Course course)
        {
            if (ModelState.IsValid)
            {
                _dataService.UpdateCourse(course);
                return RedirectToAction("Index");
            }
            return View(course);
        }

        // Xóa khóa học
        public IActionResult Delete(int id)
        {
            _dataService.DeleteCourse(id);
            return RedirectToAction("Index");
        }
    }
}
