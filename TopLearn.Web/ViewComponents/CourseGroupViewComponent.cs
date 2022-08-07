using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using TopLearn.Core.Services.Interfaces;

namespace TopLearn.Web.ViewComponents
{
    public class CourseGroupViewComponent : ViewComponent
    {
        private readonly ICourseService _courseService;

        public CourseGroupViewComponent(ICourseService courseService)
        {
            _courseService = courseService;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var groups = _courseService.GetCourseGroups();

            return await Task.FromResult((IViewComponentResult)View(groups));
        }
    }
}