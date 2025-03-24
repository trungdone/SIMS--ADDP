using System;

namespace SIMS_App.Models
{
    public class Record
    {
        public int StudentId { get; set; }
        public int ClassId { get; set; }

        public string StudentName { get; set; }
        public bool IsPresent { get; set; } // true: Present, false: Absent
    }
}

