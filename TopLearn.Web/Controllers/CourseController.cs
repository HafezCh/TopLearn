using Microsoft.AspNetCore.Mvc;

namespace TopLearn.Web.Controllers
{
    public class CourseController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
