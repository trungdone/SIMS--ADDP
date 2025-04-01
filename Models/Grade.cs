using System.ComponentModel.DataAnnotations;

namespace SIMS_App.Models
{
    // Lớp đại diện cho điểm số của sinh viên
    public class Grade
    {
        public int StudentId { get; set; }

        [Required]
        public string StudentName { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public string ClassName { get; set; }

        [Range(0, 100, ErrorMessage = "Score must be between 0 and 100.")]
        public double Score { get; set; }

        [Required]
        public string Type { get; set; }
    }

    // Interface cho Decorator Pattern để định dạng điểm số
    public interface IGradeFormatter
    {
        string FormatGrade(Grade grade);
    }

    // Lớp định dạng mặc định cho điểm số
    public class BaseGradeFormatter : IGradeFormatter
    {
        public virtual string FormatGrade(Grade grade)
        {
            return $"{grade.Score}/10";
        }
    }

    // Lớp Decorator cho bài thi (Exam), định dạng điểm theo thang 100
    public class ExamGradeFormatter : BaseGradeFormatter
    {
        public override string FormatGrade(Grade grade)
        {
            return $"{grade.Score * 10}/100";
        }
    }
}
