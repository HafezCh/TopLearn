using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using TopLearn.Core.Security;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Entities.User;

namespace TopLearn.Web.Pages.Administration.RolesManagement
{
    [PermissionChecker(8)]
    public class EditRoleModel : PageModel
    {
        [BindProperty] public Role Role { get; set; }

        #region Injections

        private readonly IPermissionService _permissionService;

        public EditRoleModel(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        #endregion

        public void OnGet(int id)
        {
            Role = _permissionService.GetById(id);
            ViewData["Permissions"] = _permissionService.GetPermissions();
            ViewData["SelectedPermissions"] = _permissionService.GetPermissionsRole(id);
        }

        public IActionResult OnPost(List<int> selectedPermissions)
        {
            if (!ModelState.IsValid) return Page();

            _permissionService.UpdateRole(Role);

            // Update Permissions
            _permissionService.UpdatePermissionRole(Role.RoleId, selectedPermissions);

            return RedirectToPage("./Index");
        }
    }
}
