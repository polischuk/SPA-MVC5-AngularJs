using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity.EntityFramework;
using MyApp.data.Entities;

namespace MyApp.data.Repositories
{
    public interface IApplicationUserRepository
    {
        IQueryable<ApplicationUser> GetAllUsers();

        ApplicationUser GetUser(Guid userId);
        ApplicationUser GetUser(string userName);

        int GetNumberOfUsersActiveAfter(DateTime afterWhen);

        int CountUsersWithName(string nameToMatch);
        IQueryable<ApplicationUser> UsersWithNamePattern(string nameToMatch);

        int TotalUsersCount();

        IQueryable<ApplicationUser> GetUsersAsQueryable();

        ICollection<IdentityUserRole> GetUsersInRole(string roleName);
        ICollection<IdentityUserRole> GetUsersInRole(Guid roleId);
        ICollection<IdentityUserRole> GetUsersInRole(IdentityRole role);
        IQueryable<IdentityRole> GetAllRoles();

        IdentityRole GetRole(Guid id);

        IdentityRole GetRole(string name);

        ICollection<IdentityUserRole> GetRolesForUser(string userName);
        ICollection<IdentityUserRole> GetRolesForUser(Guid userId);
        ICollection<IdentityUserRole> GetRolesForUser(ApplicationUser user);

        ApplicationUser CreateUser(string username, string password, string email, IdentityRole role);
        ApplicationUser CreateUser(string username, string password, string email, string displayName, IdentityRole role);

        void DeleteUser(ApplicationUser user);
        void DeleteUser(string userName);


        void AddRole(IdentityRole role);
        void AddRole(string roleName);

        void AddRoleToUser(Guid userId, string roleName);
        void AddRoleToUser(string userName, string roleName);
        void AddRoleToUser(ApplicationUser user, IdentityRole role);

        void DeleteRole(IdentityRole role);
        void DeleteRole(string roleName);

        void SaveChanges();

        bool UserExists(ApplicationUser user);
        bool RoleExists(IdentityRole role);
        bool UserNameTaken(string userName);
        void ClearUserRoles(string userName);
    }
}
