using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using SIMS_App.Models;

namespace SIMS_App.Data
{
    public class GradeService
    {
        private const string FilePath = "Resources/Grade.json";
        private List<Grade> _grades;

        public GradeService()
        {
            LoadGrades();
        }

        private void LoadGrades()
        {
            if (File.Exists(FilePath))
            {
                string json = File.ReadAllText(FilePath);
                _grades = JsonSerializer.Deserialize<List<Grade>>(json) ?? new List<Grade>();
            }
            else
            {
                _grades = new List<Grade>();
            }
        }

        public List<Grade> GetGrades() => _grades;

        public void AddGrade(Grade grade)
        {
            if (_grades.Count > 0)
            {
                int newId = _grades.Max(g => g.StudentId) + 1; // Lấy ID lớn nhất + 1
                grade.StudentId = newId;
            }
            else
            {
                grade.StudentId = 1; // Nếu danh sách rỗng, bắt đầu từ 1
            }

            _grades.Add(grade);
            SaveGrades();
        }

        public void UpdateGrade(Grade updatedGrade)
        {
            var existingGrade = _grades.FirstOrDefault(g => g.StudentId == updatedGrade.StudentId && g.Subject == updatedGrade.Subject);
            if (existingGrade != null)
            {
                // Cập nhật toàn bộ dữ liệu của bản ghi hiện có
                existingGrade.StudentName = updatedGrade.StudentName;
                existingGrade.ClassName = updatedGrade.ClassName;
                existingGrade.Score = updatedGrade.Score;
                existingGrade.Type = updatedGrade.Type;
                SaveGrades();
            }
        }


        public void DeleteGrade(int studentId, string subject)
        {
            var grade = _grades.Find(g => g.StudentId == studentId && g.Subject == subject);
            if (grade != null)
            {
                _grades.Remove(grade);
                SaveGrades();
            }
        }

        private void SaveGrades()
        {
            string json = JsonSerializer.Serialize(_grades, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }

    }
}

