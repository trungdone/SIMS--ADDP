using SIMS_App.Models;
using System.Collections.Generic;

namespace SIMS_App.Data
{
    public interface IDataService
    {
        List<Course> GetCourses();
        Course GetCourseById(int courseId);
        void AddCourse(Course course);
        void UpdateCourse(Course course);
        void DeleteCourse(int courseId);

        List<Class> GetClasses();
        Class GetClassById(int classId);
        void AddClass(Class cls);
        void UpdateClass(Class cls);
        void DeleteClass(int classId);
        string GetStudentName(int studentId);

        List<Student> GetStudentsByClassId(int classId);


    }
}
