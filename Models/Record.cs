using System;

namespace SIMS_App.Models
{
    public class Record
    {
        public int StudentId { get; set; }
        public int ClassId { get; set; }
        public string StudentName { get; set; }
        public bool IsPresent { get; set; } // true: Present, false: Absent
        public string ClassName { get; set; }  
        public string CourseName { get; set; } 
        public double AbsenceRate { get; set; } // Tỷ lệ vắng mặt (%)
        public DateTime Date { get; set; }
    }
}

