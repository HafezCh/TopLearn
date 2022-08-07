using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TopLearn.Core.DTOs.User;
using TopLearn.Core.Security;
using TopLearn.Core.Services.Interfaces;

namespace TopLearn.Web.Pages.Administration.UsersManagement
{
    [PermissionChecker(2)]
    public class IndexModel : PageModel
    {
        #region Injections

        private readonly IUserService _userService;

        public IndexModel(IUserService userService)
        {
            _userService = userService;
        }

        #endregion

        public UserForAdminViewModel ViewModel { get; set; }

        public void OnGet(int pageId = 1, string userNameFilter = "", string emailFilter = "")
        {
            ViewModel = _userService.GetUsers(pageId, emailFilter, userNameFilter);
        }

        public IActionResult OnGetRemove(int id)
        {
            _userService.RemoveUser(id);

            return RedirectToPage("./Index");
        }
    }
}
