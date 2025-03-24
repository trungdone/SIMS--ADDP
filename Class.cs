namespace SIMS_App.Models
{
    public class Class
    {
        public int ClassId { get; set; }
        public string Name { get; set; }
        public int CourseId { get; set; } // Mỗi lớp học thuộc một khóa học
        public List<User> Students { get; set; } = new List<User>(); // Danh sách sinh viên trong lớp học
    }
}

