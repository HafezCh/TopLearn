using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using TopLearn.Core.Security;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Entities.Course;

namespace TopLearn.Web.Pages.Administration.CourseGroupsManagement
{
    [PermissionChecker(13)]
    public class IndexModel : PageModel
    {
        #region Injections

        private readonly ICourseService _courseService;

        public IndexModel(ICourseService courseService)
        {
            _courseService = courseService;
        }

        #endregion

        public List<CourseGroup> CourseGroups { get; set; }

        public void OnGet()
        {
            CourseGroups = _courseService.GetCourseGroups();
        }
    }
}
