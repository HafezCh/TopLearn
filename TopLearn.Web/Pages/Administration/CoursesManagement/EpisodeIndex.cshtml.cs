using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Entities.Course;

namespace TopLearn.Web.Pages.Administration.CoursesManagement
{
    public class EpisodeIndexModel : PageModel
    {
        #region Injections

        private readonly ICourseService _courseService;

        public EpisodeIndexModel(ICourseService courseService)
        {
            _courseService = courseService;
        }

        #endregion

        public List<CourseEpisode> CourseEpisodes { get; set; }

        public void OnGet(int id)
        {
            ViewData["CourseId"] = id;
            CourseEpisodes = _courseService.GetCourseEpisodes(id);
        }

        public IActionResult OnGetDeleteEpisode(int id)
        {
            var courseId = _courseService.DeleteEpisode(id);

            return Redirect("/Administration/CoursesManagement/EpisodeIndex/" + courseId);
        }
    }
}
