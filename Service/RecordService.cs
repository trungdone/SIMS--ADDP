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
                    .Select(s => new Record { StudentId = s.UserId, StudentName = s.Name, IsPresent = false, ClassId = classId })
                    .ToList();
            }
            return new List<Record>();
            return selectedClass.Students
    .Select(s => new Record { StudentId = s.UserId, StudentName = s.Name, IsPresent = false })
    .ToList();

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
