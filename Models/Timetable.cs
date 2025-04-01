using System.ComponentModel.DataAnnotations;

namespace SIMS_App.Models
{
    public class Timetable
    {
        public int Id { get; set; }

        [Required]
        public string StudentName { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string ClassName { get; set; }

        [Required]
        public string TeacherName { get; set; }

        [Required]
        public string StartTime { get; set; }  // Đổi thành string để nhận giá trị từ form

        [Required]
        public string EndTime { get; set; }
    }
}