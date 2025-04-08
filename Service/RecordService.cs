using SIMS_App.Data;
using SIMS_App.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SIMS_App.Services
{
    public class RecordService
    {
        private readonly IDataService _dataService;
        private List<IAttendanceObserver> _observers = new List<IAttendanceObserver>();
        private readonly string _filePath = "Resources/Record.csv"; // Đường dẫn file CSV

        // Thêm vào RecordService.cs
        public RecordService(IDataService dataService)
        {
            _dataService = dataService;
            var fullPath = Path.GetFullPath(_filePath);
            Console.WriteLine($"📌 Full CSV path: {fullPath}");
            Console.WriteLine($"🔍 File exists: {File.Exists(fullPath)}");
            Console.WriteLine($"🔐 Read access: {HasReadAccess(fullPath)}");
        }

        private bool HasReadAccess(string filePath)
        {
            try
            {
                File.ReadAllText(filePath);
                return true;
            }
            catch { return false; }
        }

        public void AddObserver(IAttendanceObserver observer)
        {
            _observers.Add(observer);
        }

        public void RemoveObserver(IAttendanceObserver observer)
        {
            _observers.Remove(observer);
        }



        public List<Record> GetRecordsByStudentId(int studentId)
        {
            Console.WriteLine("\n---- BẮT ĐẦU GetRecordsByStudentId ----");
            var records = new List<Record>();
            string filePath = "Resources/Record.csv";

            if (!File.Exists(filePath))
            {
                Console.WriteLine("❌ File CSV không tồn tại!");
                return records;
            }

            var lines = File.ReadAllLines(filePath);

            for (int i = 1; i < lines.Length; i++)
            {
                var parts = lines[i].Split(',');
                if (parts.Length >= 5 &&
                    int.TryParse(parts[0], out int csvStudentId) &&
                    csvStudentId == studentId)
                {
                    records.Add(new Record
                    {
                        StudentId = csvStudentId,
                        StudentName = parts[1],
                        ClassId = int.Parse(parts[2]),
                        Date = DateTime.Parse(parts[3]), // ✅ Parse date
                        IsPresent = bool.Parse(parts[4])
                    });
                }
            }

            Console.WriteLine($"✅ Tổng số bản ghi tìm thấy: {records.Count}");
            Console.WriteLine("---- KẾT THÚC GetRecordsByStudentId ----\n");
            return records;
        }



        public void SetAttendance(int studentId, string studentName, bool isPresent, int classId)
        {
            string filePath = "Resources/Record.csv";
            string date = DateTime.Now.ToString("yyyy-MM-dd");

            var lines = File.Exists(filePath) ? File.ReadAllLines(filePath).ToList() : new List<string>();

            // Header
            if (lines.Count == 0 || !lines[0].StartsWith("StudentId"))
                lines.Insert(0, "StudentId,StudentName,ClassId,Date,IsPresent");

            // Remove old record for same student/class/date
            lines = lines.Where(line =>
            {
                var parts = line.Split(',');
                return !(parts.Length >= 5 &&
                         parts[0] == studentId.ToString() &&
                         parts[2] == classId.ToString() &&
                         parts[3] == date);
            }).ToList();

            // Add new record
            lines.Add($"{studentId},{studentName},{classId},{date},{isPresent}");

            File.WriteAllLines(filePath, lines);
        }
        public List<Record> GetStudentsByClass(int classId)
        {
            var selectedClass = _dataService.GetClassById(classId);
            var records = new List<Record>();
            if (selectedClass == null || selectedClass.Students == null)
                return records;

            string today = DateTime.Now.ToString("yyyy-MM-dd");
            string filePath = "Resources/Record.csv";
            var attendanceMap = new Dictionary<int, bool>();

            if (File.Exists(filePath))
            {
                foreach (var line in File.ReadLines(filePath).Skip(1))
                {
                    var parts = line.Split(',');
                    if (parts.Length >= 5 &&
                        int.TryParse(parts[0], out int sid) &&
                        int.TryParse(parts[2], out int cid) &&
                        parts[3] == today &&
                        cid == classId)
                    {
                        attendanceMap[sid] = bool.Parse(parts[4]);
                    }
                }
            }

            foreach (var student in selectedClass.Students)
            {
                records.Add(new Record
                {
                    StudentId = student.Id,
                    StudentName = student.Name,
                    ClassId = classId,
                    IsPresent = attendanceMap.ContainsKey(student.Id) ? attendanceMap[student.Id] : false
                });
            }

            return records;
        }

        private void SaveAttendanceToCsv(Record record)
        {
            try
            {
                bool fileExists = File.Exists(_filePath);
                using (var writer = new StreamWriter(_filePath, true, Encoding.UTF8))
                {
                    if (!fileExists)
                    {
                        writer.WriteLine("StudentId,StudentName,ClassId,IsPresent"); // Ghi header nếu file chưa tồn tại
                    }
                    writer.WriteLine($"{record.StudentId},{record.StudentName},{record.ClassId},{record.IsPresent}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error writing to CSV: {ex.Message}");
            }
        }
    }
}
