
namespace SIMS_App.Models
{
    public class Class
    {
        private static int _nextId = 1; // ID tự động tăng
        public int Id { get; set; }
        public int ClassId { get; set; }
        public string Name { get; set; }
        public int CourseId { get; set; } // Mỗi lớp học thuộc một khóa học

        public List<User> Students { get; set; } = new List<User>(); // Danh sách sinh viên trong lớp học
        public int StudentId { get; set; }
    }
}

