using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using MyApp.core.Models;
using MyApp.data;
using MyApp.data.Entities;
using MyApp.data.Users;

namespace MyApp.web.Controllers
{
    public class AccountController : Controller
    {
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }
        readonly ApplicationDbContext _context = DependencyResolver.Current.GetService<ApplicationDbContext>();

        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        [Authorize]
        public async Task<JsonResult> GetUserInfo()
        {
            try
            {
                var name = User.Identity.Name;
                var findUser = await UserManager.FindByNameAsync(name);
                return Json(new {data = findUser,status ="OK"});
            }
            catch
            {
                return Json(new { status = "ERROR" });

            }
            

        }

        [HttpPost]
        public ActionResult LogOff()
        {
            AuthenticationManager.SignOut();
            return Json(new { status = "OK" });
        }
        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> Login(string userName, string password)
        {
            if (ModelState.IsValid)
            {
                var findUser = await UserManager.FindByNameAsync(userName);
                var user = await UserManager.FindAsync(findUser.UserName, password);
                if (user != null)
                {
                    await SignInAsync(user, false);
                    return Json("OK");
                }
                else
                {
                    return Json("ERROR");
                }
            }
            return Json("ERROR");
        }
        private async Task SignInAsync(data.Entities.ApplicationUser user, bool isPersistent)
        {
            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
            AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, await user.GenerateUserIdentityAsync(UserManager));
        }

    }
}