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
                SaveCourses(); // if you persist to file
            }
        }


        public List<Student> GetStudentsByClassId(int classId)
        {
            var students = new List<Student>();
            string filePath = "Resources/Student.CSV";

            if (!File.Exists(filePath))
            {
                Console.WriteLine("⚠ Student file not found.");
                return students;
            }

            var lines = File.ReadAllLines(filePath);
            Console.WriteLine($"📌 Total lines in CSV: {lines.Length}");

            // Bỏ qua dòng header nếu có
            var startLine = lines[0].StartsWith("StudentId,Name,Age,Email,ClassId") ? 1 : 0;

            for (int i = startLine; i < lines.Length; i++)
            {
                var line = lines[i];
                if (string.IsNullOrWhiteSpace(line)) continue;

                var data = line.Split(',');
                if (data.Length < 5)
                {
                    Console.WriteLine($"⚠ Invalid data format in line {i + 1}: {line}");
                    continue;
                }

                try
                {
                    if (!int.TryParse(data[4], out int studentClassId) || studentClassId != classId)
                        continue;

                    var student = new Student
                    {
                        StudentId = int.Parse(data[0]),
                        Name = data[1].Trim(),
                        Age = int.Parse(data[2]),
                        Email = data[3].Trim(),
                        ClassId = studentClassId
                    };
                    students.Add(student);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠ Error processing line {i + 1}: {ex.Message}");
                }
            }

            Console.WriteLine($"📌 Found {students.Count} students for class {classId}");
            return students;
        }

    }
}

