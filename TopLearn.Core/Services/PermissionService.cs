using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using TopLearn.Core.Services.Interfaces;
using TopLearn.DataLayer.Context;
using TopLearn.DataLayer.Entities.Permission;
using TopLearn.DataLayer.Entities.User;

namespace TopLearn.Core.Services
{
    public class PermissionService : IPermissionService
    {
        private readonly TopLearnDbContext _context;

        public PermissionService(TopLearnDbContext context)
        {
            _context = context;
        }

        public void AddPermissionToRole(int roleId, List<int> permissions)
        {
            foreach (var permission in permissions)
            {
                _context.RolePermissions.Add(new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = permission
                });
            }

            _context.SaveChanges();
        }

        public int AddRole(Role role)
        {
            _context.Roles.Add(role);
            _context.SaveChanges();
            return role.RoleId;
        }

        public void AddRolesToUser(List<int> roleIds, int userId)
        {
            foreach (var roleId in roleIds)
            {
                _context.UserRoles.Add(new UserRole
                {
                    RoleId = roleId,
                    UserId = userId
                });
            }

            _context.SaveChanges();
        }

        public bool CheckPermission(int permissionId, string userName)
        {
            var userId = _context.Users.Single(x => x.UserName == userName).UserId;

            var userRoles = _context.UserRoles.Where(x => x.UserId == userId)
                .Select(x => x.RoleId).ToList();

            if (!userRoles.Any())
                return false;

            var rolesPermission = _context.RolePermissions.Where(x => x.PermissionId == permissionId)
                .Select(x => x.RoleId).ToList();

            return rolesPermission.Any(p => userRoles.Contains(p));
        }

        public void EditUserRoles(List<int> roleIds, int userId)
        {
            // Delete All User Roles
            _context.UserRoles
                .Where(x => x.UserId == userId).ToList()
                .ForEach(x => _context.UserRoles.Remove(x));

            //Add New UserRoles
            AddRolesToUser(roleIds, userId);
        }

        public Role GetById(int roleId)
        {
            return _context.Roles.Find(roleId);
        }

        public List<Permission> GetPermissions()
        {
            return _context.Permissions.AsNoTracking().ToList();
        }

        public List<int> GetPermissionsRole(int roleId)
        {
            return _context.RolePermissions
                .Where(x => x.RoleId == roleId)
                .Select(x => x.PermissionId).ToList();
        }

        public List<Role> GetRoles(bool IsRemoved)
        {
            if (IsRemoved)
                return _context.Roles.IgnoreQueryFilters().Where(r => r.IsRemoved).AsNoTracking().ToList();

            return _context.Roles.AsNoTracking().ToList();
        }

        public List<Role> GetRoles()
        {
            return _context.Roles.AsNoTracking().ToList();
        }

        public void RemoveRole(Role role)
        {
            role.IsRemoved = true;
            UpdateRole(role);
        }

        public void UpdatePermissionRole(int roleId, List<int> permissions)
        {
            _context.RolePermissions.Where(p => p.RoleId == roleId)
                .ToList().ForEach(p => _context.Remove(p));

            AddPermissionToRole(roleId, permissions);
        }

        public void UpdateRole(Role role)
        {
            _context.Roles.Update(role);
            _context.SaveChanges();
        }
    }
}