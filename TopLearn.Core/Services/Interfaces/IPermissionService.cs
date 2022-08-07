using System.Collections.Generic;
using TopLearn.DataLayer.Entities.Permission;
using TopLearn.DataLayer.Entities.User;

namespace TopLearn.Core.Services.Interfaces
{
    public interface IPermissionService
    {
        #region Roles

        Role GetById(int roleId);
        List<Role> GetRoles(bool IsRemoved);
        List<Role> GetRoles();
        void RemoveRole(Role role);
        void UpdateRole(Role role);
        int AddRole(Role role);
        void AddRolesToUser(List<int> roleIds, int userId);
        void EditUserRoles(List<int> roleIds, int userId);

        #endregion

        #region Permissions

        List<Permission> GetPermissions();
        void AddPermissionToRole(int roleId, List<int> permissions);
        void UpdatePermissionRole(int roleId, List<int> permissions);
        List<int> GetPermissionsRole(int roleId);
        bool CheckPermission(int permissionId, string userName);

        #endregion
    }
}