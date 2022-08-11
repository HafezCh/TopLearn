using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using TopLearn.Core.DTOs.Course;
using TopLearn.Core.Security;
using TopLearn.Core.Services.Interfaces;

namespace TopLearn.Web.Pages.Administration.CoursesManagement
{
    [PermissionChecker(10)]
    public class IndexModel : PageModel
    {
        #region Injections

        private readonly ICourseService _courseService;

        public IndexModel(ICourseService courseService)
        {
            _courseService = courseService;
        }

        #endregion

        public List<ShowCourseForAdminViewModel> Courses { get; set; }

        public void OnGet()
        {
            Courses = _courseService.GetCoursesForAdmin();
        }
    }
}
