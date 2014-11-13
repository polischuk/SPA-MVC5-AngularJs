using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MyApp.data.Entities;
using MyApp.data.Users;
using System.Web.Helpers;

namespace MyApp.data.Repositories
{
    public class ApplicationUserRepository : IApplicationUserRepository
    {
        private readonly ApplicationDbContext _db;
        public readonly ApplicationRoleManager RoleManager;
        public readonly ApplicationUserManager UserManager;
        public ApplicationUserRepository()
        {
            _db = new ApplicationDbContext();
            UserManager = new ApplicationUserManager(new UserStore<ApplicationUser>(_db));
            RoleManager = new ApplicationRoleManager(new RoleStore<IdentityRole>(_db));
        }

        public IQueryable<ApplicationUser> GetAllUsers()
        {
            return UserManager.Users;
        }

        public ApplicationUser GetUser(Guid userId)
        {
            return UserManager.Users.SingleOrDefault(usr => usr.Id == userId.ToString());
        }

        public ApplicationUser GetUser(string userName)
        {
            return UserManager.Users.SingleOrDefault(usr => usr.UserName == userName);
        }

        public int GetNumberOfUsersActiveAfter(DateTime afterWhen)
        {
            return 1;//_db.Users.Where(usr => usr.LastActivityDate > afterWhen).Count();
        }

        public int CountUsersWithName(string nameToMatch)
        {
            return UserManager.Users.Count(usr => usr.UserName.Contains(nameToMatch));
        }

        public IQueryable<ApplicationUser> UsersWithNamePattern(string nameToMatch)
        {
            return UserManager.Users.Where(usr => usr.UserName.Contains(nameToMatch)).OrderBy(usr => usr.UserName);
        }

        public int TotalUsersCount()
        {
            return UserManager.Users.Count();
        }

        public IQueryable<ApplicationUser> GetUsersAsQueryable()
        {
            return UserManager.Users.OrderBy(usr => usr.UserName);
        }

        public ICollection<IdentityUserRole> GetUsersInRole(string roleName)
        {
            return GetUsersInRole(GetRole(roleName));
        }

        public ICollection<IdentityUserRole> GetUsersInRole(Guid roleId)
        {
            return GetUsersInRole(GetRole(roleId));
        }

        public ICollection<IdentityUserRole> GetUsersInRole(IdentityRole role)
        {
            if (!RoleExists(role))
                throw new ArgumentException("MissingRole");

            return RoleManager.Roles.SingleOrDefault(rl => rl.Id == role.Id).Users;
        }

        public IQueryable<IdentityRole> GetAllRoles()
        {
            return RoleManager.Roles;
        }

        public IdentityRole GetRole(Guid id)
        {
            return RoleManager.Roles.SingleOrDefault(rl => rl.Id == id.ToString());
        }

        public IdentityRole GetRole(string name)
        {
            return RoleManager.Roles.SingleOrDefault(rl => rl.Name == name);
        }

        public ICollection<IdentityUserRole> GetRolesForUser(string userName)
        {
            return UserManager.Users.SingleOrDefault(usr => usr.UserName == userName).Roles;
        }

        public ICollection<IdentityUserRole> GetRolesForUser(Guid userId)
        {
            return UserManager.Users.SingleOrDefault(usr => usr.Id == userId.ToString()).Roles;
        }

        public ICollection<IdentityUserRole> GetRolesForUser(ApplicationUser user)
        {
            return GetRolesForUser(user.Id);
        }

        private void AddUser(ApplicationUser user)
        {
            if (UserExists(user))
                throw new ArgumentException("UserAlreadyExists");

            UserManager.CreateAsync(user);
        }

        public ApplicationUser CreateUser(string userName, string password, string email, string displayName,IdentityRole role)
        {
            var user = CreateUser(userName, password, email,role);
            UserManager.CreateAsync(user);
            return user;
        }

        public ApplicationUser CreateUser(string userName, string password, string email,IdentityRole role)
        {
            if (string.IsNullOrEmpty(userName.Trim()))
                throw new ArgumentException("The user name provided is invalid. Please check the value and try again.");
            if (string.IsNullOrEmpty(password.Trim()))
                throw new ArgumentException("The password provided is invalid. Please enter a valid password value.");
            if (_db.Users.Any(user => user.UserName == userName))
                throw new ArgumentException("Username already exists. Please enter a different user name.");

            var cryptoPass =Crypto.HashPassword(password);
            
            var newUser = new ApplicationUser()
                              {
                                  UserName = userName,
                                  PasswordHash = cryptoPass,
                                  Email = email,

                              };
            UserManager.CreateAsync(newUser);
            var rolesForUser = UserManager.GetRoles(newUser.Id);
            if (!rolesForUser.Contains(role.Name))
            {
                 UserManager.AddToRole(newUser.Id, role.Name);
            }
            try
            {
                AddUser(newUser);
            }
            catch (ArgumentException ae)
            {
                throw ae;
            }
            catch (Exception e)
            {
                throw new ArgumentException("The authentication provider returned an error. Please verify your entry and try again. " +
                    "If the problem persists, please contact your system administrator.");
            }

          

            return newUser;
        }

        public void DeleteUser(ApplicationUser user)
        {
            if (!UserExists(user))
                throw new ArgumentException("MissingUser");

            UserManager.DeleteAsync(user);
        }

        public void DeleteUser(string userName)
        {
            DeleteUser(GetUser(userName));
        }

        public void AddRole(IdentityRole role)
        {
            if (RoleExists(role))
                throw new ArgumentException("RoleAlreadyExists");

            RoleManager.CreateAsync(role);
        }

        public void AddRole(string roleName)
        {
            var role = new IdentityRole()
            {
                Name = roleName
            };

            AddRole(role);
        }

        public void AddRoleToUser(Guid userId, string roleName)
        {
            var usr = UserManager.Users.SingleOrDefault(u => u.Id == userId.ToString());
            var rl = RoleManager.Roles.SingleOrDefault(r => r.Name == roleName);
            AddRoleToUser(usr, rl);
        }

        public void AddRoleToUser(string userName, string roleName)
        {
            var usr = _db.Users.SingleOrDefault(u => u.UserName == userName);
            var rl = _db.Roles.SingleOrDefault(r => r.Name == roleName);
            AddRoleToUser(usr, rl);
        }

        public void AddRoleToUser(ApplicationUser user, IdentityRole role)
        {
             UserManager.AddToRolesAsync(user.Id,role.Name);
        }

        public void DeleteRole(IdentityRole role)
        {
            if (!RoleExists(role))
                throw new ArgumentException("Role doesn't exist");

            RoleManager.DeleteAsync(role);
        }

        public void DeleteRole(string roleName)
        {
            DeleteRole(GetRole(roleName));

        }

        public void SaveChanges()
        {
           _db.SaveChanges();
        }

        public bool UserExists(ApplicationUser user)
        {
            if (user == null)
                return false;
 
            return (_db.Users.SingleOrDefault(u => u.Id == user.Id || u.UserName == user.UserName) != null);
        }

        public bool UserNameTaken(string userName)
        {
            if (userName == String.Empty) return true;

            return (_db.Users.SingleOrDefault(u => u.UserName == userName) != null);
        }

        public void ClearUserRoles(string userName)
        {
            var user = _db.Users.Single(vis => vis.UserName == userName);
            user.Roles.Clear();
        }

        public bool RoleExists(IdentityRole role)
        {
            if (role == null)
                return false;

            return (_db.Roles.ToList().SingleOrDefault(r => r.Id == role.Id || r.Name == role.Name) != null);
        }
    }
}