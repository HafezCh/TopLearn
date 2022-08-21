using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Entities.Course;

namespace TopLearn.Web.Controllers
{
    public class CourseController : Controller
    {
        #region Injections

        private readonly IUserService _userService;
        private readonly IOrderService _orderService;
        private readonly ICourseService _courseService;

        public CourseController(ICourseService courseService, IOrderService orderService, IUserService userService)
        {
            _courseService = courseService;
            _orderService = orderService;
            _userService = userService;
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

        [Authorize]
        public IActionResult BuyCourse(int id)
        {
            var orderId = _orderService.AddOrder(User.Identity.Name, id);

            return Redirect("/UserPanel/MyOrder/ShowOrder/" + orderId);
        }

        [Route("DownloadFile/{episodeId}")]
        public IActionResult DownloadFile(int episodeId)
        {
            var episode = _courseService.GetEpisodeById(episodeId);

            var fileName = episode.EpisodeFileName;

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/CourseFiles", fileName);

            if (episode.IsFree)
            {
                byte[] file = System.IO.File.ReadAllBytes(filePath);
                return File(file, "application/force-download", fileName);
            }

            if (User.Identity.IsAuthenticated && _orderService.IsUserInCourse(User.Identity.Name, episode.CourseId))
            {
                byte[] file = System.IO.File.ReadAllBytes(filePath);
                return File(file, "application/force-download", fileName);
            }

            return Forbid();
        }

        [HttpPost]
        public IActionResult CreateComment(CourseComment command)
        {
            if (command.Comment == null) return null;

            command.IsRemoved = false;
            command.IsAdminRead = false;
            command.CreationDate = DateTime.Now;
            command.UserId = _userService.GetUserIdByUserName(User.Identity.Name);

            _courseService.AddComment(command);

            var comments = _courseService.GetCourseComments(command.CourseId);

            return View("ShowComment", comments);
        }

        public IActionResult ShowComment(int id, int pageId = 1)
        {
            var comments = _courseService.GetCourseComments(id, pageId);

            return View(comments);
        }

        public IActionResult CourseVote(int id)
        {
            return PartialView(_courseService.GetCourseVotes(id));
        }

        [Authorize]
        public IActionResult AddVote(int id, bool vote)
        {
            var userId = _userService.GetUserIdByUserName(User.Identity.Name);

            _courseService.AddVote(userId, id, vote);

            return PartialView("CourseVote", _courseService.GetCourseVotes(id));
        }
    }
}
