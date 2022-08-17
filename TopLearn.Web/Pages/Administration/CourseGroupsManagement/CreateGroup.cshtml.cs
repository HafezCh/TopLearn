using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopLearn.Core.Security;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Entities.Course;

namespace TopLearn.Web.Pages.Administration.CourseGroupsManagement
{
    [PermissionChecker(14)]
    public class CreateGroupModel : PageModel
    {
        #region Injections

        private readonly ICourseService _courseService;

        public CreateGroupModel(ICourseService courseService)
        {
            _courseService = courseService;
        }

        #endregion

        [BindProperty] public CourseGroup CourseGroup { get; set; }

        public void OnGet(int? id)
        {
            CourseGroup = new CourseGroup
            {
                ParentId = id
            };
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid) return Page();

            _courseService.AddGroup(CourseGroup);

            return RedirectToPage("./Index");
        }
    }
}
