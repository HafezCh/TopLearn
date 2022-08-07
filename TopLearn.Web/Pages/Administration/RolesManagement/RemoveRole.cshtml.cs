using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopLearn.Core.Security;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Entities.User;

namespace TopLearn.Web.Pages.Administration.RolesManagement
{
    [PermissionChecker(9)]
    public class RemoveRoleModel : PageModel
    {
        [BindProperty] public Role Role { get; set; }

        #region Injections

        private readonly IPermissionService _permissionService;

        public RemoveRoleModel(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        #endregion

        public void OnGet(int id)
        {
            Role = _permissionService.GetById(id);
        }

        public IActionResult OnPost()
        {
            _permissionService.RemoveRole(Role);

            return RedirectToPage("./Index");
        }
    }
}
