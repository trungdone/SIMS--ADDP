using SIMS_App.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SIMS_App.Data
{
    public class DataService : IDataService
    {
        private static List<Course> courses = new List<Course>
        {
            new Course { CourseId = 1, Name = "Introduction to Programming", TotalStudents = 6, StartDate = new DateTime(2025, 4, 10), EndDate = new DateTime(2025, 6, 30), NumberOfSessions = 10 },
            new Course { CourseId = 2, Name = "PBI", TotalStudents = 1, StartDate = new DateTime(2024, 1, 15), EndDate = new DateTime(2024, 6, 15), NumberOfSessions = 24 },
            new Course { CourseId = 3, Name = "APDP", TotalStudents = 1, StartDate = new DateTime(2023, 6, 4), EndDate = new DateTime(2024, 6, 4), NumberOfSessions = 24 }
        };

        private static List<Class> classes = new List<Class>
    {
        new Class { ClassId = 1, Name = "Class A", CourseId = 1, Students = new List<User> {
            new User { UserId = 1, Name = "Alice" },
            new User { UserId = 2, Name = "Bob" }
        }},
        new Class { ClassId = 2, Name = "Class B", CourseId = 2, Students = new List<User> {
            new User { UserId = 3, Name = "Charlie" }
        }},
        new Class { ClassId = 3, Name = "Class C", CourseId = 3, Students = new List<User> {
            new User { UserId = 4, Name = "David" },
            new User { UserId = 5, Name = "Eve" }
        }}
    };

        public string GetStudentName(int studentId)
        {
            var student = classes.SelectMany(c => c.Students).FirstOrDefault(s => s.UserId == studentId);
            return student?.Name ?? string.Empty; // Trả về tên nếu tìm thấy, nếu không trả về chuỗi rỗng
        }


        // Lấy danh sách khóa học
        public List<Course> GetCourses() => courses;

        // Lấy khóa học theo ID
        public Course GetCourseById(int courseId) => courses.FirstOrDefault(c => c.CourseId == courseId);

        // Thêm khóa học mới
        public void AddCourse(Course course)
        {
            course.CourseId = courses.Max(c => c.CourseId) + 1;
            courses.Add(course);
        }

        // Cập nhật khóa học
        public void UpdateCourse(Course course)
        {
            var existingCourse = GetCourseById(course.CourseId);
            if (existingCourse != null)
            {
                existingCourse.Name = course.Name;
                existingCourse.TotalStudents = course.TotalStudents;
                existingCourse.StartDate = course.StartDate;
                existingCourse.EndDate = course.EndDate;
                existingCourse.NumberOfSessions = course.NumberOfSessions;
            }
        }

        // Xóa khóa học
        public void DeleteCourse(int courseId)
        {
            var course = GetCourseById(courseId);
            if (course != null)
            {
                courses.Remove(course);
            }
        }

        // Lấy danh sách lớp
        public List<Class> GetClasses() => classes;

        // Lấy lớp theo ID
        public Class GetClassById(int classId) => classes.FirstOrDefault(c => c.ClassId == classId);

        // Thêm lớp mới
        public void AddClass(Class cls)
        {
            cls.ClassId = classes.Count + 1;
            classes.Add(cls);
        }

        // Cập nhật lớp học
        public void UpdateClass(Class cls)
        {
            var existingClass = GetClassById(cls.ClassId);
            if (existingClass != null)
            {
                existingClass.Name = cls.Name;
                existingClass.CourseId = cls.CourseId;
            }
        }

        // Xóa lớp học
        public void DeleteClass(int classId)
        {
            var cls = GetClassById(classId);
            if (cls != null)
            {
                classes.Remove(cls);
            }
        }
    }

    public class RecordService
    {
        private readonly IDataService _dataService;
        private List<IAttendanceObserver> _observers = new List<IAttendanceObserver>();

        public RecordService(IDataService dataService)
        {
            _dataService = dataService;
        }

        public void AddObserver(IAttendanceObserver observer)
        {
            _observers.Add(observer);
        }

        public void RemoveObserver(IAttendanceObserver observer)
        {
            _observers.Remove(observer);
        }

        // Lấy danh sách sinh viên theo lớp
        public List<Record> GetStudentsByClass(int classId)
        {
            var selectedClass = _dataService.GetClassById(classId);
            if (selectedClass != null)
            {
                return selectedClass.Students
                    .Select(s => new Record { StudentId = s.UserId, StudentName = s.Name, IsPresent = false })
                    .ToList();
            }
            return new List<Record>();
        }

        // Cập nhật điểm danh
        public void SetAttendance(int studentId, bool isPresent)
        {
            var student = _dataService.GetClasses()
                .SelectMany(c => c.Students)
                .FirstOrDefault(s => s.UserId == studentId);

            if (student != null)
            {
                var record = new Record { StudentId = student.UserId, StudentName = student.Name, IsPresent = isPresent };

                foreach (var observer in _observers)
                {
                    observer.UpdateAttendance(record);
                }
            }
        }


    }
}
