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
            _recordService = new SIMS_App.Services.RecordService(dataService);
            _recordService.AddObserver(new AttendanceObserver());
        }

        public IActionResult Index()
        {
            return RedirectToAction("ManageRecord");
        }

        public IActionResult ManageRecord()
        {
            ViewBag.Classes = _dataService.GetClasses();
            return View("~/Views/Admin/ManageRecord.cshtml");
        }

        [HttpGet]
        public JsonResult GetStudentsByClass(int classId)
        {
            var records = _recordService.GetStudentsByClass(classId);
            return Json(records);
        }




        [HttpPost]
        public JsonResult SaveAttendance([FromBody] List<Record> attendanceList)
        {
            foreach (var record in attendanceList)
            {
                _recordService.SetAttendance(record.StudentId, record.StudentName, record.IsPresent, record.ClassId);
            }

            return Json(new { success = true, message = "Attendance saved successfully!" });
        }

        [HttpGet]
        public IActionResult MyAttendance()
        {
            var studentIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(studentIdStr) || !int.TryParse(studentIdStr, out int studentId))
            {
                return RedirectToAction("Login", "Auth");
            }

            var records = _recordService.GetRecordsByStudentId(studentId);

            // Enrich with class name
            foreach (var record in records)
            {
                var cls = _dataService.GetClassById(record.ClassId);
                record.ClassName = cls?.Name ?? "N/A";
            }

            return View("~/Views/Student/MyAttendance.cshtml", records);
        }






[HttpGet]
public IActionResult ViewRecords()
{
    var studentIdStr = HttpContext.Session.GetString("UserId");

    if (string.IsNullOrEmpty(studentIdStr) || !int.TryParse(studentIdStr, out int studentId))
    {
        return RedirectToAction("Login", "Auth");
    }

    var records = _recordService.GetRecordsByStudentId(studentId);

    // Optional: Add class and course info
    foreach (var record in records)
    {
        var cls = _dataService.GetClassById(record.ClassId);
        record.ClassName = cls?.Name ?? "N/A";
        record.CourseName = cls != null ? _dataService.GetCourseById(cls.CourseId)?.Name ?? "N/A" : "N/A";
    }

    // Optional: Calculate absence rate per student (if not per day)
    double absenceRate = CalculateAbsenceRate(records);
    records.ForEach(r => r.AbsenceRate = absenceRate);

    return View("~/Views/Student/ViewRecords.cshtml", records);
}

        private double CalculateAbsenceRate(List<Record> records)
        {
            if (records == null || records.Count == 0) return 0;

            int total = records.Count;
            int absent = records.Count(r => !r.IsPresent);
            return (double)absent / total * 100;
        }

    }
}
