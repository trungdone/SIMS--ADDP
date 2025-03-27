namespace SIMS_App.Models
{
    public class Course
    {
        public int CourseId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int TotalStudents { get; set; } 
        public DateTime StartDate { get; set; } 
        public DateTime EndDate { get; set; } // Ngày kết thúc
        public int NumberOfSessions { get; set; } // Tổng số buổi học
        public List<Class> Classes { get; set; } = new List<Class>();
    }
}
