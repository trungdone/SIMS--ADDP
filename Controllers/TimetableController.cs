using Microsoft.AspNetCore.Mvc;
using SIMS_App.Data;
using SIMS_App.Models;
using System.Collections.Generic;
using System.Text.Json;

namespace SIMS_App.Controllers
{
    [Route("Timetable")]
    public class TimetableController : Controller
    {
        private readonly TimetableService _timetableService;

        public TimetableController()
        {
            _timetableService = new TimetableService();
        }

        [Route("ManageTimetable")]
        public IActionResult ManageTimetable()
        {
            var timetables = _timetableService.GetTimetables();
            return View("~/Views/Admin/ManageTimetable.cshtml", timetables);
        }

        [Route("ViewTimetable")]
        public IActionResult ViewTimetable()
        {
            var timetables = _timetableService.GetTimetables();
            return View("~/Views/Student/ViewTimetable.cshtml", timetables);
        }


        [HttpPost]
        [Route("AddTimetable")]
        public IActionResult AddTimetable([FromBody] Timetable timetable)
        {
            if (timetable == null)
            {
                return Json(new { success = false, message = "Invalid data!" });
            }

            _timetableService.AddTimetable(timetable);
            return Json(new { success = true, message = "Timetable added successfully!" });
        }

        [HttpPost]
        [Route("UpdateTimetable")]
        public IActionResult UpdateTimetable([FromBody] Timetable timetable)
        {
            if (timetable == null)
            {
                return Json(new { success = false, message = "Invalid data!" });
            }

            _timetableService.UpdateTimetable(timetable);
            return Json(new { success = true, message = "Timetable updated successfully!" });
        }

        [HttpPost]
        [Route("DeleteTimetable")]
        public IActionResult DeleteTimetable([FromBody] JsonElement data)
        {
            try
            {
                Console.WriteLine($"Raw data received: {data}"); // Debug

                if (!data.TryGetProperty("id", out JsonElement idElement))
                {
                    return Json(new { success = false, message = "Invalid request: ID missing!" });
                }

                int id = idElement.GetInt32();
                Console.WriteLine($"Extracted ID: {id}");

                bool deleted = _timetableService.DeleteTimetable(id);

                if (deleted)
                    return Json(new { success = true, message = "Timetable deleted successfully!" });
                else
                    return Json(new { success = false, message = $"Timetable not found! ID: {id}" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Error: {ex.Message}" });
            }
        }

    }
}
