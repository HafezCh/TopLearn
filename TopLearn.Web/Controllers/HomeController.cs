using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;
using TopLearn.Core.Services.Interfaces;

namespace TopLearn.Web.Controllers
{
    public class HomeController : Controller
    {
        #region Injections

        private readonly IUserService _userService;
        private readonly ICourseService _courseService;

        public HomeController(IUserService userService, ICourseService courseService)
        {
            _userService = userService;
            _courseService = courseService;
        }

        #endregion

        public IActionResult Index()
        {
            var courses = _courseService.GetCourses();
            return View(courses);
        }

        [HttpGet("OnlinePayment/{id}")]
        public IActionResult OnlinePayment(int id)
        {
            if (HttpContext.Request.Query["Status"] != "" &&
                HttpContext.Request.Query["Status"].ToString().ToLower() == "ok"
                && HttpContext.Request.Query["Authority"] != "")
            {
                string authority = HttpContext.Request.Query["Authority"];

                var wallet = _userService.GetWalletByWalletId(id);

                var payment = new ZarinpalSandbox.Payment(wallet.Amount);
                var res = payment.Verification(authority).Result;
                if (res.Status == 100)
                {
                    ViewBag.code = res.RefId;
                    ViewBag.IsSuccess = true;
                    wallet.IsPay = true;
                    _userService.UpdateWallet(wallet);
                }
            }

            return View();
        }

        public JsonResult GetSubGroups(int id)
        {
            var groups = new List<SelectListItem>
            {
                new SelectListItem{Text = "انتخاب کنید",Value = ""}
            };
            groups.AddRange(_courseService.GetSubGroupForManageCourse(id));
            return Json(new SelectList(groups, "Value", "Text"));
        }
    }
}
