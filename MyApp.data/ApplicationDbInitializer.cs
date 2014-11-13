using System.Collections.Generic;
using System.Data.Entity.Migrations;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using MyApp.data.Entities;
using MyApp.data.Users;

namespace MyApp.data
{
    public class ApplicationDbInitializer
        : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public ApplicationDbInitializer()
        {
            AutomaticMigrationsEnabled = true;
            AutomaticMigrationDataLossAllowed = false;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            InitializeIdentityForEF(context);
            base.Seed(context);
        }


        public static void InitializeIdentityForEF(ApplicationDbContext db)
        {


            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));
            var roleManager = new ApplicationRoleManager(new RoleStore<IdentityRole>(db));

            const string adminRoleName = "Administrator";

            var roles = new List<IdentityRole>
                {
                    new IdentityRole
                        {
                            Name = adminRoleName
                        },
                        new IdentityRole
                        {
                            Name = "User"
                        }
                };


            foreach (var identityRole in roles)
            {
                var existingRole = roleManager.FindByName(identityRole.Name);

                if (existingRole == null)
                {
                    roleManager.Create(identityRole);
                }

            }

            var adminUsers = new List<ApplicationUser>
            {
                new ApplicationUser
                    {
                        UserName = "admin",
                        Email = "admin@radacode.com",
                        EmailConfirmed = true
                    }
            };


            foreach (var applicationUser in adminUsers)
            {
                var existingUser = userManager.FindByName(applicationUser.UserName);
                if (existingUser == null)
                {
                    userManager.Create(applicationUser, "q1w2e3");
                    userManager.SetLockoutEnabled(applicationUser.Id, false);

                    // Add user admin to Role Admin if not already added
                    var rolesForUser = userManager.GetRoles(applicationUser.Id);
                    if (!rolesForUser.Contains(adminRoleName))
                    {
                       userManager.AddToRole(applicationUser.Id, adminRoleName);
                    }
                }
            }
        }
    }
}
