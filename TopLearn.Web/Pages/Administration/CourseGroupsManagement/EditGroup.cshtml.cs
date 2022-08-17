using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopLearn.Core.Security;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Entities.Course;

namespace TopLearn.Web.Pages.Administration.CourseGroupsManagement
{
    [PermissionChecker(15)]
    public class EditGroupModel : PageModel
    {
        #region Injections

        private readonly ICourseService _courseService;

        public EditGroupModel(ICourseService courseService)
        {
            _courseService = courseService;
        }

        #endregion

        [BindProperty] public CourseGroup CourseGroup { get; set; }

        public void OnGet(int id)
        {
            CourseGroup = _courseService.GetGroupById(id);
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid) return Page();

            _courseService.UpdateGroup(CourseGroup);

            return RedirectToPage("./Index");
        }
    }
}
