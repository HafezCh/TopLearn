using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using TopLearn.Core.Convertors;
using TopLearn.Core.DTOs.User;
using TopLearn.Core.Generator;
using TopLearn.Core.Security;
using TopLearn.Core.Senders;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Entities.User;

namespace TopLearn.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly IViewRenderService _viewRenderService;

        public AccountController(IUserService userService, IViewRenderService viewRenderService)
        {
            _userService = userService;
            _viewRenderService = viewRenderService;
        }

        #region Register

        [HttpGet("Register")]
        public IActionResult Register() => View();

        [HttpPost("Register")]
        [ValidateAntiForgeryToken]
        public IActionResult Register(RegisterViewModel command)
        {
            if (!ModelState.IsValid)
            {
                return View(command);
            }

            if (_userService.IsExistUserName(command.UserName))
            {
                ModelState.AddModelError("UserName", "نام کاربری معتبر نمی باشد");
                return View(command);
            }

            if (_userService.IsExistsEmail(command.Email))
            {
                ModelState.AddModelError("Email", "ایمیل معتبر نمی باشد");
                return View(command);
            }

            // Register User

            DataLayer.Entities.User.User user = new User
            {
                UserName = command.UserName,
                Email = FixedText.FixEmail(command.Email),
                Password = PasswordHelper.EncodePasswordMd5(command.Password),
                ActiveCode = Generator.GenerateUniqCode(),
                RegisterDate = DateTime.Now,
                UserAvatar = "Default.gif",
                IsActive = false
            };

            // Send EmailActiveCode

            var emailBody = _viewRenderService.RenderToStringAsync("_ActiveEmail", user);
            SendEmail.Send(user.Email, "فعال سازی حساب کاربری", emailBody);

            _userService.AddUser(user);

            return View("SuccessRegister", user);
        }
        #endregion

        #region Login

        [HttpGet("Login")]
        public IActionResult Login(bool editProfile = false, string redirectedPath = "")
        {
            ViewBag.EditProfile = editProfile;
            ViewBag.RedirectedPath = redirectedPath;
            return View();
        }

        [HttpPost("Login")]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel command, string redirectedPath = "")
        {
            if (!ModelState.IsValid) return View(command);

            var user = _userService.LoginUser(command);

            if (user != null)
            {
                if (user.IsActive)
                {
                    var claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.NameIdentifier,user.UserId.ToString()),
                        new Claim(ClaimTypes.Name,user.UserName)
                    };
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);

                    var properties = new AuthenticationProperties
                    {
                        IsPersistent = command.RememberMe
                    };
                    HttpContext.SignInAsync(principal, properties);

                    ViewBag.IsSuccess = true;
                    ViewBag.RedirectPath = redirectedPath;
                    return View();
                }
                else
                {
                    ModelState.AddModelError("Email", "حساب کاربری شما فعال نمی باشد");
                }
            }
            ModelState.AddModelError("Email", "کاربری با مشخصات وارد شده یافت نشد");
            return View(command);
        }

        #endregion

        #region Logout
        [Route("Logout")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/Login");
        }

        #endregion

        #region Active Account

        public IActionResult ActiveAccount(string id)
        {
            ViewBag.IsActive = _userService.ActiveAccount(id);
            return View();
        }

        #endregion

        #region Forget Password

        [HttpGet("ForgotPassword")]
        public IActionResult ForgotPassword()
        {
            return View();
        }

        [HttpPost("ForgotPassword")]
        [AutoValidateAntiforgeryToken]
        public IActionResult ForgotPassword(ForgotPasswordViewModel command)
        {
            if (!ModelState.IsValid) return View(command);

            var fixedEmail = FixedText.FixEmail(command.Email);
            var user = _userService.GetByEmail(command.Email);

            if (user == null)
            {
                ModelState.AddModelError("Email", "کاربری یافت نشد");
                return View(command);
            }

            string bodyEmail = _viewRenderService.RenderToStringAsync("_ForgotPassword", user);
            SendEmail.Send(user.Email, "بازیابی حساب کاربری", bodyEmail);
            ViewBag.IsSuccess = true;

            return View();
        }

        #endregion

        #region Reset Password

        [HttpGet]
        public IActionResult ResetPassword(string id)
        {
            return View(new ResetPasswordViewModel
            {
                ActiveCode = id
            });
        }

        [HttpPost]
        [AutoValidateAntiforgeryToken]
        public IActionResult ResetPassword(ResetPasswordViewModel command)
        {
            if (!ModelState.IsValid) return View(command);

            var user = _userService.GetByActiveCode(command.ActiveCode);

            if (user == null) return NotFound();

            string hashPassword = PasswordHelper.EncodePasswordMd5(command.Password);
            user.Password = hashPassword;
            _userService.UpdateUser(user);

            return Redirect("/Login");
        }

        #endregion
    }
}
