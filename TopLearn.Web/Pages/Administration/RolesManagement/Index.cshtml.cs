using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Collections.Generic;
using TopLearn.Core.Security;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Entities.User;

namespace TopLearn.Web.Pages.Administration.RolesManagement
{
    [PermissionChecker(6)]
    public class IndexModel : PageModel
    {
        #region Injections

        private readonly IPermissionService _permissionService;

        public IndexModel(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }

        #endregion

        public List<Role> Roles { get; set; }

        public void OnGet(bool IsRemoved)
        {
            Roles = _permissionService.GetRoles(IsRemoved);
        }
    }
}
