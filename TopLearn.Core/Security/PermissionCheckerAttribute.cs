using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using TopLearn.Core.Services.Interfaces;

namespace TopLearn.Core.Security
{
    public class PermissionCheckerAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private int _permissionId = 0;
        private IPermissionService _permissionService;

        public PermissionCheckerAttribute(int permissionId)
        {
            _permissionId = permissionId;
        }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            if (context.HttpContext.User.Identity.IsAuthenticated)
            {
                _permissionService = (IPermissionService)context.HttpContext.RequestServices.GetService(typeof(IPermissionService));
                var userName = context.HttpContext.User.Identity.Name;
                if (!_permissionService.CheckPermission(_permissionId, userName))
                {
                    context.Result = new RedirectResult($"/Login?redirectedPath={context.HttpContext.Request.Path}");
                }
            }
            else
            {
                context.Result = new RedirectResult("/Login");
            }
        }
    }
}
