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

            using (var reader = new StreamReader("Resources/Student.CSV"))
            {
                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');

                    // Bỏ qua dòng tiêu đề
                    if (!int.TryParse(values[0], out int studentId))
                    {
                        continue;
                    }

                    var student = new Student
                    {
                        StudentId = int.Parse(values[0]),
                        Name = values[1],
                        Age = int.Parse(values[2]),
                        Email = values[3],
                        ClassId = string.IsNullOrEmpty(values[4]) || values[4] == "0" ? null : int.Parse(values[4])
                    };


                    if (student.ClassId == classId)
                    {
                        students.Add(student);
                    }
                }
            }


            return students.Count > 0 ? students : new List<Student>();
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
        public Course GetCourseById(int courseId) => courses.FirstOrDefault(c => c.CourseId == courseId);

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
            return classes.FirstOrDefault(c => c.Id == classId); // Đổi ClassId thành Id
        }


        public void AddClass(Class cls)
        {
            // Nếu danh sách rỗng, ID bắt đầu từ 1
            int newId = classes.Any() ? classes.Max(c => c.Id) + 1 : 1;
            cls.Id = newId;
            classes.Add(cls);
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
    }
}