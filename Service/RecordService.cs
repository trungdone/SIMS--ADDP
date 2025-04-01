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

        public List<Record> GetStudentsByClass(int classId)
        {
            var records = new List<Record>();

            // Đọc từ file CSV
            if (File.Exists(_filePath))
            {
                var lines = File.ReadAllLines(_filePath).Skip(1); // Bỏ dòng header
                foreach (var line in lines)
                {
                    var parts = line.Split(',');
                    if (parts.Length >= 4 && int.TryParse(parts[0], out int studentId) && int.TryParse(parts[2], out int csvClassId))
                    {
                        if (csvClassId == classId) // Lọc đúng class
                        {
                            records.Add(new Record
                            {
                                StudentId = studentId,
                                StudentName = parts[1],
                                ClassId = csvClassId,
                                IsPresent = bool.Parse(parts[3])
                            });
                        }
                    }
                }
            }

            return records;
        }

        public List<Record> GetRecordsByStudentId(int studentId)
        {
            Console.WriteLine("\n---- BẮT ĐẦU GetRecordsByStudentId ----");
            Console.WriteLine($"🔎 StudentId cần tìm: {studentId}");

            var fullPath = Path.GetFullPath(_filePath);
            Console.WriteLine($"📁 Đường dẫn file CSV: {fullPath}");
            Console.WriteLine($"🔍 File tồn tại: {File.Exists(fullPath)}");

            if (!File.Exists(fullPath))
            {
                Console.WriteLine("❌ File CSV không tồn tại!");
                return new List<Record>();
            }

            Console.WriteLine("\nNội dung file CSV:");
            Console.WriteLine(File.ReadAllText(fullPath));

            var records = new List<Record>();
            var lines = File.ReadAllLines(fullPath);

            Console.WriteLine($"\nTổng số dòng: {lines.Length}");
            Console.WriteLine("Header: " + lines[0]);

            for (int i = 1; i < lines.Length; i++)
            {
                var line = lines[i];
                Console.WriteLine($"\nĐang xử lý dòng {i}: {line}");

                var parts = line.Split(',');
                Console.WriteLine($"Số phần tử sau split: {parts.Length}");

                if (parts.Length >= 4)
                {
                    Console.WriteLine($"Phần tử 0 (StudentId): {parts[0]}");
                    Console.WriteLine($"Phần tử 2 (ClassId): {parts[2]}");

                    if (int.TryParse(parts[0], out int csvStudentId) && csvStudentId == studentId)
                    {
                        Console.WriteLine("✅ Tìm thấy bản ghi phù hợp!");
                        records.Add(new Record
                        {
                            StudentId = csvStudentId,
                            StudentName = parts[1],
                            ClassId = int.Parse(parts[2]),
                            IsPresent = bool.Parse(parts[3]),
                            Date = DateTime.Now
                        });
                    }
                }
            }

            Console.WriteLine($"\nTổng số bản ghi tìm thấy: {records.Count}");
            Console.WriteLine("---- KẾT THÚC GetRecordsByStudentId ----\n");
            return records;
        }



        public void SetAttendance(int studentId, string studentName, bool isPresent, int classId)
        {
            var record = new Record { StudentId = studentId, StudentName = studentName, IsPresent = isPresent, ClassId = classId };

            foreach (var observer in _observers)
            {
                observer.UpdateAttendance(record);
            }

            // Lưu điểm danh vào file CSV
            SaveAttendanceToCsv(record);
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
