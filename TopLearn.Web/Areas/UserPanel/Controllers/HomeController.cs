using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TopLearn.Core.Convertors;
using TopLearn.Core.DTOs;
using TopLearn.Core.Services.Interfaces;

namespace TopLearn.Web.Areas.UserPanel.Controllers
{
    [Authorize]
    [Area("UserPanel")]
    public class HomeController : Controller
    {
        private readonly IUserService _userService;

        public HomeController(IUserService userService)
        {
            _userService = userService;
        }

        public IActionResult Index()
        {
            var userInfo = _userService.GetUserInformation(User.Identity.Name);
            return View(userInfo);
        }

        #region Edit Profile

        [HttpGet("UserPanel/EditProfile")]
        public IActionResult EditProfile()
        {
            var user = _userService.GetDataForEditUserProfile(User.Identity.Name);
            return View(user);
        }

        [HttpPost("UserPanel/EditProfile")]
        [ValidateAntiForgeryToken]
        public IActionResult EditProfile(EditUserProfileViewModel command)
        {
            if (!ModelState.IsValid) return View(command);

            if (_userService.IsExistUserNameForEditProfile(command.UserName, command.UserId))
            {
                ModelState.AddModelError("UserName", "نام کاربری نامعتبر است");
                return View(command);
            }

            var email = FixedText.FixEmail(command.Email);

            if (_userService.IsExistEmailForEditProfile(email, command.UserId))
            {
                ModelState.AddModelError("Email", "ایمیل نامعتبر است");
                return View(command);
            }

            _userService.EditProfile(User.Identity.Name, command);

            //Log Out User
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            return Redirect("/Login?editProfile=true");
        }

        #endregion

        #region Change Password

        [HttpGet("UserPanel/ChangePassword")]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost("UserPanel/ChangePassword")]
        [ValidateAntiForgeryToken]
        public IActionResult ChangePassword(ChangePasswordViewModel command)
        {
            if (!ModelState.IsValid) return View(command);

            var userName = User.Identity.Name;

            if (!_userService.CompareOldPassword(userName, command.OldPassword))
            {
                ModelState.AddModelError("OldPassword", "کلمه عبور فعلی نادرست می باشد");
                return View(command);
            }

            _userService.ChangeUserPassword(userName, command.Password);
            ViewBag.IsSuccess = true;

            return View();
        }

        #endregion
    }
}
