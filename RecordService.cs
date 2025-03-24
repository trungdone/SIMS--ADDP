using SIMS_App.Models;
using SIMS_App.Data;
using System.Collections.Generic;
using System.Linq;

namespace SIMS_App.Services
{
    public class RecordService
    {
        private readonly IDataService _dataService;
        private List<IAttendanceObserver> _observers = new List<IAttendanceObserver>();

        // Thêm constructor nhận IDataService
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

        public void SetAttendance(int studentId, string studentName, bool isPresent) // Cập nhật tham số để tránh lỗi
        {
            var record = new Record { StudentId = studentId, StudentName = studentName, IsPresent = isPresent };

            foreach (var observer in _observers)
            {
                observer.UpdateAttendance(record);
            }
        }
    }

}
