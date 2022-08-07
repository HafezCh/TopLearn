using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using TopLearn.Core.DTOs.User;
using TopLearn.Core.Security;
using TopLearn.Core.Services.Interfaces;

namespace TopLearn.Web.Pages.Administration.UsersManagement
{
    [PermissionChecker(3)]
    public class CreateUserModel : PageModel
    {
        [BindProperty] public CreateUserFromAdminViewModel UserModel { get; set; }

        #region Injections

        private readonly IUserService _userService;
        private readonly IPermissionService _permissionService;

        public CreateUserModel(IUserService userService, IPermissionService permissionService)
        {
            _userService = userService;
            _permissionService = permissionService;
        }

        #endregion

        public void OnGet()
        {
            ViewData["Roles"] = _permissionService.GetRoles();
        }

        public IActionResult OnPost(List<int> selectedRoles)
        {
            if (!ModelState.IsValid) return Page();

            // Add User
            var userId = _userService.AddUserFromAdmin(UserModel);

            // Add Roles
            _permissionService.AddRolesToUser(selectedRoles, userId);

            return RedirectToPage("./Index");
        }
    }
}
