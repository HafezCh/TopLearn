using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using TopLearn.Core.Services.Interfaces;

namespace TopLearn.Web.Controllers
{
    public class CourseController : Controller
    {
        #region Injections

        private readonly ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        #endregion

        public IActionResult Index(int pageId = 1, string filter = ""
            , string getType = "all", string orderByType = "date",
            int startPrice = 0, int endPrice = 0, List<int> selectedGroups = null)
        {
            ViewBag.Groups = _courseService.GetCourseGroups();
            ViewBag.selectedGroups = selectedGroups;
            ViewBag.pageId = pageId;
            var courses = _courseService.GetCourses(pageId, filter, getType, orderByType, startPrice, endPrice, selectedGroups, 9);
            return View(courses);
        }

        [HttpGet("ShowCourse/{id}")]
        public IActionResult ShowCourse(int id)
        {
            var course = _courseService.GetCourseForShow(id);
            if (course == null) return NotFound();
            return View(course);
        }
    }
}
