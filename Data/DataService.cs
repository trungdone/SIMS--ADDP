using SIMS_App.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.Json;

namespace SIMS_App.Data
{
    public class DataService : IDataService
    {
        private static List<Course> courses = new List<Course>
        {
        };

        private List<Class> classes;
        private const string FilePath = "Resources/Classes.json";

        public DataService()
        {
            LoadClasses();
        }

        private void LoadClasses()
        {
            if (File.Exists(FilePath))
            {
                string json = File.ReadAllText(FilePath);
                classes = JsonSerializer.Deserialize<List<Class>>(json) ?? new List<Class>();
            }
            else
            {
                classes = new List<Class>();
            }
        }


        public List<Student> GetStudentsByClassId(int classId)
        {
            var students = new List<Student>();
            string filePath = "Resources/Student.CSV";

            if (!File.Exists(filePath))
            {
                Console.WriteLine("Student file not found.");
                return students;
            }

            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines.Skip(1))
            {
                var data = line.Split(',');
                if (data.Length >= 5 && int.TryParse(data[4], out int studentClassId) && studentClassId == classId)
                {
                    students.Add(new Student
                    {
                        StudentId = int.Parse(data[0]),
                        Name = data[1],
                        Age = int.Parse(data[2]),
                        Email = data[3],
                        ClassId = studentClassId
                    });
                }
            }

            Console.WriteLine($"Total students found in class {classId}: {students.Count}");
            return students;
        }



        private void SaveClasses()
        {
            string json = JsonSerializer.Serialize(classes, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }

        public string GetStudentName(int studentId)
        {
            var student = classes.SelectMany(c => c.Students).FirstOrDefault(s => s.Id == studentId);
            return student?.Name ?? string.Empty;
        }

        public List<Course> GetCourses() => courses;
        public Course GetCourseById(int courseId)
        {
            Console.WriteLine($"\n🔍 Đang tìm khóa học với CourseId: {courseId}");
            var result = courses.FirstOrDefault(c => c.CourseId == courseId);
            Console.WriteLine($"Kết quả: {(result != null ? $"Tìm thấy: {result.Name}" : "Không tìm thấy")}");
            return result;
        }

        public void AddCourse(Course course)
        {
            course.CourseId = courses.Max(c => c.CourseId) + 1;
            courses.Add(course);
        }

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

        public void DeleteCourse(int courseId)
        {
            var course = GetCourseById(courseId);
            if (course != null)
            {
                courses.Remove(course);
            }
        }

        public List<Class> GetClasses() => classes;
        public Class GetClassById(int classId)
        {
            Console.WriteLine($"\n🔍 Đang tìm lớp với ClassId: {classId}");
            var result = classes.FirstOrDefault(c => c.Id == classId);
            Console.WriteLine($"Kết quả: {(result != null ? $"Tìm thấy: {result.Name}" : "Không tìm thấy")}");
            return result;
        }


        public void AddClass(Class newClass)
        {
            newClass.Id = classes.Any() ? classes.Max(c => c.Id) + 1 : 1;

            // Lấy danh sách tất cả học sinh để tìm ID lớn nhất
            int maxStudentId = classes.SelectMany(c => c.Students).Any()
                ? classes.SelectMany(c => c.Students).Max(s => s.Id)
                : 0;

            // Cập nhật StudentId mới dựa trên ID cao nhất hiện có
            newClass.StudentId = maxStudentId + 1;

            classes.Add(newClass);
            SaveClasses();
        }




        public void UpdateClass(Class cls)
        {
            var existingClass = GetClassById(cls.ClassId);
            if (existingClass != null)
            {
                existingClass.Name = cls.Name;
                existingClass.CourseId = cls.CourseId;
                SaveClasses();
            }
        }

        public void DeleteClass(int classId)
        {
            var cls = GetClassById(classId);
            if (cls != null)
            {
                classes.Remove(cls);
                SaveClasses();
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

        public string GetStudentName(int studentId)
        {
            // Đọc từ file Student.CSV thay vì từ classes
            string filePath = "Resources/Student.CSV";
            if (!File.Exists(filePath)) return string.Empty;

            var lines = File.ReadAllLines(filePath);
            foreach (var line in lines.Skip(1))
            {
                var data = line.Split(',');
                if (data.Length >= 5 && int.TryParse(data[0], out int csvStudentId) && csvStudentId == studentId)
                {
                    return data[1]; // Trả về tên student
                }
            }
            return string.Empty;
        }
    }
}