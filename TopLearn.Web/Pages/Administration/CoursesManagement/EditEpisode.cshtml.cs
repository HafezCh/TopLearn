using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Entities.Course;

namespace TopLearn.Web.Pages.Administration.CoursesManagement
{
    public class EditEpisodeModel : PageModel
    {
        #region Injections

        private readonly ICourseService _courseService;

        public EditEpisodeModel(ICourseService courseService)
        {
            _courseService = courseService;
        }

        #endregion

        [BindProperty] public CourseEpisode CourseEpisode { get; set; }

        public void OnGet(int id)
        {
            CourseEpisode = _courseService.GetEpisodeById(id);
        }

        public IActionResult OnPost(IFormFile fileEpisode)
        {
            if (!ModelState.IsValid) return Page();

            if (fileEpisode != null)
            {
                if (_courseService.CheckExistFile(fileEpisode.FileName))
                {
                    ViewData["IsExistFile"] = true;
                    return Page();
                }
            }

            _courseService.UpdateEpisode(CourseEpisode, fileEpisode);

            return Redirect("/Administration/CoursesManagement/EpisodeIndex/" + CourseEpisode.CourseId);
        }
    }
}
