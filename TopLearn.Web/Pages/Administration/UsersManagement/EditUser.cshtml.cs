using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using TopLearn.Core.DTOs.User;
using TopLearn.Core.Security;
using TopLearn.Core.Services.Interfaces;

namespace TopLearn.Web.Pages.Administration.UsersManagement
{
    [PermissionChecker(4)]
    public class EditUserModel : PageModel
    {
        [BindProperty] public EditUserFromAdminViewModel EditModel { get; set; }

        #region Injections

        private readonly IUserService _userService;
        private readonly IPermissionService _permissionService;

        public EditUserModel(IUserService userService, IPermissionService permissionService)
        {
            _userService = userService;
            _permissionService = permissionService;
        }

        #endregion

        public void OnGet(int id)
        {
            EditModel = _userService.GetUserDetailsForEditFromAdmin(id);
            ViewData["Roles"] = _permissionService.GetRoles();
        }

        public IActionResult OnPost(string userName, List<int> selectedRoles)
        {
            if (!ModelState.IsValid)
            {
                ViewData["Roles"] = _permissionService.GetRoles();
                return Page();
            }

            _userService.EditUserFromAdmin(EditModel);

            _permissionService.EditUserRoles(selectedRoles, EditModel.UserId);

            if (User.Identity.Name == userName && EditModel.UserName != userName)
            {
                HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                return Redirect("/Login?editProfile=true");
            }

            return RedirectToPage("./Index");
        }
    }
}
