using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopLearn.Core.DTOs.User;
using TopLearn.Core.Security;
using TopLearn.Core.Services.Interfaces;

namespace TopLearn.Web.Pages.Administration.UsersManagement
{
    [PermissionChecker(5)]
    public class RemovedUsersModel : PageModel
    {
        #region Injections

        private readonly IUserService _userService;

        public RemovedUsersModel(IUserService userService)
        {
            _userService = userService;
        }

        #endregion

        public UserForAdminViewModel ViewModel { get; set; }

        public void OnGet(int pageId = 1, string userNameFilter = "", string emailFilter = "")
        {
            ViewModel = _userService.GetRemovedUsers(pageId, emailFilter, userNameFilter);
        }

        public IActionResult OnGetRestore(int id)
        {
            _userService.RestoreUser(id);

            return RedirectToPage("./RemovedUsers");
        }
    }
}
