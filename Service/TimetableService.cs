using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using SIMS_App.Models;

namespace SIMS_App.Data
{
    public class TimetableService
    {
        private const string FilePath = "Resources/Timetable.json";
        private List<Timetable> _timetables;

        public TimetableService()
        {
            LoadTimetables();
        }

        private void LoadTimetables()
        {
            if (File.Exists(FilePath))
            {
                string json = File.ReadAllText(FilePath);
                Console.WriteLine($"Loaded JSON: {json}"); // Debug JSON
                _timetables = JsonSerializer.Deserialize<List<Timetable>>(json) ?? new List<Timetable>();

                // Đảm bảo ID là số nguyên
                foreach (var timetable in _timetables)
                {
                    Console.WriteLine($"Loaded Timetable ID: {timetable.Id}"); // Debug ID
                    timetable.Id = Convert.ToInt32(timetable.Id);
                }
            }
            else
            {
                _timetables = new List<Timetable>();
            }
        }



        public List<Timetable> GetTimetables() => _timetables;

        public void AddTimetable(Timetable timetable)
        {
            if (_timetables.Count > 0)
            {
                timetable.Id = _timetables.Max(t => t.Id) + 1; // Lấy ID lớn nhất + 1
            }
            else
            {
                timetable.Id = 1; // Nếu danh sách rỗng, bắt đầu từ 1
            }

            _timetables.Add(timetable);
            SaveTimetables();
        }

        public void UpdateTimetable(Timetable updatedTimetable)
        {
            var existingTimetable = _timetables.FirstOrDefault(t => t.Id == updatedTimetable.Id);
            if (existingTimetable != null)
            {
                existingTimetable.StudentName = updatedTimetable.StudentName;
                existingTimetable.Subject = updatedTimetable.Subject;
                existingTimetable.ClassName = updatedTimetable.ClassName;
                existingTimetable.TeacherName = updatedTimetable.TeacherName;
                existingTimetable.StartTime = updatedTimetable.StartTime;
                existingTimetable.EndTime = updatedTimetable.EndTime;
                SaveTimetables();
            }
        }

        public bool DeleteTimetable(int id)
        {
            Console.WriteLine($"Searching for ID: {id} in Timetable list...");
            Console.WriteLine($"Total Timetables: {_timetables.Count}");

            foreach (var t in _timetables)
            {
                Console.WriteLine($"Existing ID: {t.Id}");
            }

            var timetable = _timetables.FirstOrDefault(t => t.Id == id);
            if (timetable != null)
            {
                _timetables.Remove(timetable);
                SaveTimetables();
                Console.WriteLine($"Deleted Timetable ID: {id}");
                return true;
            }
            Console.WriteLine($"Timetable ID: {id} not found!");
            return false;
        }

        private void SaveTimetables()
        {
            string json = JsonSerializer.Serialize(_timetables, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(FilePath, json);
        }
    }
}
