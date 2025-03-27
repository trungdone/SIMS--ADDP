using System;

namespace SIMS_App.Models
{
    public interface IAttendanceObserver
    {
        void UpdateAttendance(Record record);
    }

    public class AttendanceObserver : IAttendanceObserver
    {
        public void UpdateAttendance(Record record)
        {
            Console.WriteLine($"Attendance status of {record.StudentName}: {(record.IsPresent ? "✅ Present" : "❌ Absent")}");
        }
    }
}

