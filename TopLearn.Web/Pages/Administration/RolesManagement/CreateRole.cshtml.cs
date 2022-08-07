using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using TopLearn.Core.Security;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Entities.User;

namespace TopLearn.Web.Pages.Administration.RolesManagement
{
    [PermissionChecker(7)]
    public class CreateRoleModel : PageModel
    {
        [BindProperty] public Role Role { get; set; }

        #region Injections

        private readonly IPermissionService _permissionService;

        public CreateRoleModel(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        #endregion

        public void OnGet()
        {
            ViewData["Permissions"] = _permissionService.GetPermissions();
        }

        public IActionResult OnPost(List<int> selectedPermissions)
        {
            if (!ModelState.IsValid) return Page();

            var roleId = _permissionService.AddRole(Role);

            // Add Permission

            _permissionService.AddPermissionToRole(Role.RoleId, selectedPermissions);

            return RedirectToPage("./Index");
        }
    }
}
