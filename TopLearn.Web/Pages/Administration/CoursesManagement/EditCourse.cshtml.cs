using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TopLearn.Core.Security;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Entities.Course;

namespace TopLearn.Web.Pages.Administration.CoursesManagement
{
    [PermissionChecker(12)]
    public class EditCourseModel : PageModel
    {
        #region Injections

        private readonly ICourseService _courseService;

        public EditCourseModel(ICourseService courseService)
        {
            _courseService = courseService;
        }

        #endregion

        [BindProperty] public Course Course { get; set; }

        public void OnGet(int id)
        {
            Course = _courseService.GetCourseDetails(id);

            var groups = _courseService.GetGroupForManageCourse();
            ViewData["Groups"] = new SelectList(groups, "Value", "Text", Course.GroupId);

            var subGroups = _courseService.GetSubGroupForManageCourse(Course.GroupId);
            ViewData["SubGroups"] = new SelectList(subGroups, "Value", "Text", Course.SubGroup ?? 0);

            var teachers = _courseService.GetTeachers();
            ViewData["Teachers"] = new SelectList(teachers, "Value", "Text", Course.TeacherId);

            var levels = _courseService.GetLevels();
            ViewData["Levels"] = new SelectList(levels, "Value", "Text", Course.LevelId);

            var statuses = _courseService.GetStatues();
            ViewData["Statuses"] = new SelectList(statuses, "Value", "Text", Course.StatusId);
        }

        public IActionResult OnPost(IFormFile imgCourseUp, IFormFile demoUp)
        {
            if (!ModelState.IsValid) return Page();

            _courseService.UpdateCourse(Course, imgCourseUp, demoUp);

            return RedirectToPage("./Index");
        }
    }
}