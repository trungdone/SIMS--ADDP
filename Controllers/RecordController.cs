using Microsoft.AspNetCore.Mvc;
using SIMS_App.Models;
using SIMS_App.Services;
using SIMS_App.Data;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

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

        public IActionResult ViewRecords()
        {
            Console.WriteLine("\n===== BẮT ĐẦU XỬ LÝ ViewRecords =====");

            // 1. Lấy danh sách điểm danh của tất cả sinh viên có ID từ 1 đến 10
            List<Record> allStudentRecords = new List<Record>();

            for (int studentId = 1; studentId <= 10; studentId++)
            {
                Console.WriteLine($"⚡ Đang xử lý StudentId={studentId}");

                var studentRecords = _recordService.GetRecordsByStudentId(studentId);
                if (studentRecords != null && studentRecords.Any())
                {
                    foreach (var record in studentRecords)
                    {
                        var classInfo = _dataService.GetClassById(record.ClassId);
                        record.ClassName = classInfo?.Name ?? "N/A";

                        if (classInfo != null)
                        {
                            var courseInfo = _dataService.GetCourseById(classInfo.CourseId);
                            record.CourseName = courseInfo?.Name ?? "N/A";
                        }
                        else
                        {
                            record.CourseName = "N/A";
                        }
                    }

                    allStudentRecords.AddRange(studentRecords);
                }
                else
                {
                    Console.WriteLine($"❌ Không tìm thấy bản ghi nào cho StudentId={studentId}");
                }
            }

            // 2. Tính toán tỷ lệ vắng mặt cho tất cả sinh viên
            Console.WriteLine("\n[2] Đang tính toán tỷ lệ vắng mặt...");
            double absenceRate = CalculateAbsenceRate(allStudentRecords);
            allStudentRecords.ForEach(r => r.AbsenceRate = absenceRate);

            Console.WriteLine("\n===== KẾT THÚC XỬ LÝ =====\n");
            return View("~/Views/Student/ViewRecords.cshtml", allStudentRecords);
        }



        // Thêm phương thức tính toán tỷ lệ vắng mặt
        private double CalculateAbsenceRate(List<Record> records)
        {
            if (records == null || records.Count == 0) return 0;

            int total = records.Count;
            int absentCount = records.Count(r => !r.IsPresent);

            return (double)absentCount / total * 100;
        }


        // Trang điểm danh với danh sách khóa học và lớp học
        public IActionResult ManageRecord(int? classId)
        {
            var courses = _dataService.GetCourses();
            var classes = _dataService.GetClasses();

            // Đọc danh sách điểm danh từ CSV nếu có classId
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
