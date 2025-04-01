using Microsoft.AspNetCore.Mvc;
using SIMS_App.Data;
using SIMS_App.Models;
using System.Collections.Generic;

namespace SIMS_App.Controllers
{
    [Route("Grade")]
    public class GradeController : Controller
    {
        private readonly GradeService _gradeService;

        public GradeController()
        {
            _gradeService = new GradeService();
        }

        [Route("ManageGrade")]
        public IActionResult ManageGrade()
        {
            var grades = _gradeService.GetGrades();
            return View("~/Views/Teacher/ManageGrade.cshtml", grades);
        }

        [Route("ViewGrade")]
        public IActionResult ViewGrade()
        {
            var grades = _gradeService.GetGrades();
            return View("~/Views/Student/ViewGrade.cshtml", grades);
        }

        [HttpPost]
        [Route("AddGrade")]
        public IActionResult AddGrade([FromBody] Grade grade)
        {
            if (grade == null)
            {
                return Json(new { success = false, message = "Invalid data!" });
            }

            _gradeService.AddGrade(grade);
            return Json(new { success = true, message = "Grade added successfully!" });
        }

        [HttpPost]
        [Route("UpdateGrade")]
        public IActionResult UpdateGrade([FromBody] Grade grade)
        {
            if (grade == null)
            {
                return Json(new { success = false, message = "Invalid data!" });
            }

            var existingGrade = _gradeService.GetGrades().FirstOrDefault(g => g.StudentId == grade.StudentId && g.Subject == grade.Subject);
            if (existingGrade != null)
            {
                _gradeService.UpdateGrade(grade);
                return Json(new { success = true, message = "Grade updated successfully!" });
            }
            else
            {
                return Json(new { success = false, message = "Grade not found!" });
            }
        }


        [HttpPost]
        [Route("DeleteGrade")]
        public IActionResult DeleteGrade([FromBody] Grade grade)
        {
            if (grade == null)
            {
                return Json(new { success = false, message = "Invalid data!" });
            }

            _gradeService.DeleteGrade(grade.StudentId, grade.Subject);
            return Json(new { success = true, message = "Grade deleted successfully!" });
        }
    }
}
