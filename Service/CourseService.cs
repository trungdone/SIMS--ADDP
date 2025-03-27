using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using SIMS_App.Models;

namespace SIMS_App.Services
{
    public class CourseService
    {
        private const string FilePath = "Resources/Course.json";
        private List<Course> courses;

        public CourseService()
        {
            LoadCourses();
        }

        private void LoadCourses()
        {
            try
            {
                if (File.Exists(FilePath))
                {
                    string json = File.ReadAllText(FilePath);

                    // Kiểm tra nếu JSON rỗng
                    if (string.IsNullOrWhiteSpace(json))
                    {
                        courses = new List<Course>();
                        return;
                    }

                    // Deserialize với kiểm soát lỗi
                    courses = JsonSerializer.Deserialize<List<Course>>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    }) ?? new List<Course>();
                }
                else
                {
                    courses = new List<Course>();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi đọc file JSON: {ex.Message}");
                courses = new List<Course>(); // Khởi tạo danh sách rỗng nếu có lỗi
            }
        }


        private void SaveCourses()
        {
            try
            {
                string json = JsonSerializer.Serialize(courses, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                File.WriteAllText(FilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi ghi file JSON: {ex.Message}");
            }
        }


        public List<Course> GetCourses() => courses;

        public Course GetCourseById(int courseId) => courses.FirstOrDefault(c => c.CourseId == courseId);

        public void AddCourse(Course course)
        {
            course.CourseId = courses.Any() ? courses.Max(c => c.CourseId) + 1 : 1;
            courses.Add(course);
            SaveCourses();
        }

        public void UpdateCourse(Course updatedCourse)
        {
            var existingCourse = GetCourseById(updatedCourse.CourseId);
            if (existingCourse != null)
            {
                existingCourse.Name = updatedCourse.Name;
                existingCourse.TotalStudents = updatedCourse.TotalStudents;
                existingCourse.StartDate = updatedCourse.StartDate;
                existingCourse.EndDate = updatedCourse.EndDate;
                existingCourse.NumberOfSessions = updatedCourse.NumberOfSessions;
                SaveCourses();
            }
        }

        public void DeleteCourse(int courseId)
        {
            var course = GetCourseById(courseId);
            if (course != null)
            {
                courses.Remove(course);
                SaveCourses();
            }
        }

        public List<Student> GetStudentsByCourseId(int courseId)
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
                if (data.Length >= 5 && int.TryParse(data[4], out int classId) && classId == courseId)
                {
                    students.Add(new Student
                    {
                        StudentId = int.Parse(data[0]),
                        Name = data[1],
                        Age = int.Parse(data[2]),
                        Email = data[3],
                        ClassId = classId
                    });
                }
            }

            Console.WriteLine($"Total students found: {students.Count}"); // Debug số lượng sinh viên
            return students;
        }



    }
}

