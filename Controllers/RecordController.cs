using Microsoft.AspNetCore.Mvc;
using SIMS_App.Models;
using SIMS_App.Services;
using SIMS_App.Data;
using System.Collections.Generic;

namespace SIMS_App.Controllers
{
    public class RecordController : Controller
    {
        private readonly SIMS_App.Services.RecordService _recordService;
        private readonly IDataService _dataService;

        public RecordController(IDataService dataService)
        {
            _dataService = dataService;
            _recordService = new SIMS_App.Services.RecordService(dataService); // Tránh lỗi mơ hồ
            _recordService.AddObserver(new AttendanceObserver());
        }

        public IActionResult Index()
        {
            return RedirectToAction("ManageRecord");
        }

        // Trang điểm danh với danh sách khóa học và lớp học
        public IActionResult ManageRecord(int? classId)
        {
            var courses = _dataService.GetCourses();
            var classes = _dataService.GetClasses();
            var records = classId.HasValue ? _recordService.GetStudentsByClass(classId.Value) : new List<Record>();

            ViewBag.Courses = courses;
            ViewBag.Classes = classes;
            ViewBag.SelectedClassId = classId;

            return View("~/Views/Admin/ManageRecord.cshtml", records);
        }
        [HttpPost]
        public IActionResult UpdateAttendance(int studentId, bool isPresent, int classId)
        {
            string studentName = _dataService.GetStudentName(studentId);
            if (string.IsNullOrEmpty(studentName))
            {
                return BadRequest("Student not found");
            }

            // Cập nhật điểm danh và lưu vào file CSV
            _recordService.SetAttendance(studentId, studentName, isPresent, classId);

            return RedirectToAction("ManageRecord", new { classId });
        }
    }
}
