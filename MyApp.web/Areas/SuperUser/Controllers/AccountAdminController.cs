using System;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity.Owin;
using MyApp.data;
using MyApp.data.Entities;
using MyApp.data.Repositories;
using MyApp.data.Users;

namespace MyApp.web.Areas.SuperUser.Controllers
{
    [Authorize(Roles = "Administrator")]
    public class AccountAdminController : Controller
    {
        private ApplicationUserManager _userManager;
        private ApplicationUserRepository _repository;
        public AccountAdminController()
        {
        }
        readonly ApplicationDbContext _context = DependencyResolver.Current.GetService<ApplicationDbContext>();
        public AccountAdminController(ApplicationUserManager userManager)
        {
            UserManager = userManager;
        }

        public ApplicationUserRepository Repository
        {
            get
            {
                return _repository ?? DependencyResolver.Current.GetService<ApplicationUserRepository>();
            }
            private set
            {
                _repository = value;
            }
        }
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
        [HttpPost]
        public async Task<ActionResult> RegisterUserTask(string firstName, string secondName, string patronymic, string tin, string address, string externalId, string password, string email, string imageId, string phoneNumber)
        {
            var findUser = _context.Users.FirstOrDefault(u => u.ExternalId == externalId || u.UserName == tin);
            if (findUser != null)
            {
                return Json(new
                {
                    status = "ERROR",
                    exception = "Такой пользователь уще существует"

                });
            }
            try
            {
                var user = new ApplicationUser
                           {
                               UserName = tin,
                               FirstName = firstName,
                               SecondName = secondName,
                               Patronymic = patronymic,
                               Tin = tin,
                               Address = address,
                               ExternalId = externalId,
                               Email = email,
                               ImageId = imageId,
                               PhoneNumber = phoneNumber
                           };
                var result = await UserManager.CreateAsync(user, password);
                if (result.Succeeded)
                {
                    Repository.AddRoleToUser(user.UserName, "User");
                    return Json(new
                                {
                                    status = "OK"
                                });
                }
                return Json(new
                            {
                                status = "ERROR",
                                exception = result.Errors.First(),

                            });
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = "ERROR",
                    exception = ex.Message,
                    innerExeption = ex.InnerException

                });
            }
        }
        [HttpPost]
        public async Task<ActionResult> ChangeRoles(string id, string roleId)
        {

            try
            {

                var roles = await UserManager.GetRolesAsync(id);
                foreach (var role in roles)
                {
                    await UserManager.RemoveFromRoleAsync(id, role);
                }
                var findUser = _context.Users.FirstOrDefault(m => m.Id == id);
                var findRole = _context.Roles.FirstOrDefault(m => m.Id == roleId);
                Repository.AddRoleToUser(findUser, findRole);
                return Json(new
                {
                    status = "OK"
                });


            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = "ERROR",
                    exception = ex.Message,
                    innerExeption = ex.InnerException

                });
            }
        }

        [HttpPost]
        public ActionResult UpdateUser(string id, string firstName, string secondName, string patronymic, string address, string externalId, string email, string imageId, string phoneNumber)
        {

            try
            {
                var findUser = _context.Users.FirstOrDefault(u => u.Id == id);
                if (findUser != null)
                {
                    findUser.FirstName = firstName;
                    findUser.SecondName = secondName;
                    findUser.Patronymic = patronymic;
                    findUser.Address = address;
                    findUser.ExternalId = externalId;
                    findUser.Email = email;
                    findUser.ImageId = imageId;
                    findUser.PhoneNumber = phoneNumber;
                    _context.SaveChanges();
                }
                return Json(new
                            {
                                status = "OK"
                            });

            }
            catch (Exception ex)
            {
                return Json(new
               {
                   status = "ERROR",
                   exception = ex.Message,
                   innerExeption = ex.InnerException

               });
            }

        }

        [HttpPost]
        public ActionResult RemoveUser(string id)
        {
            try
            {

                var findUser = _context.Users.FirstOrDefault(u => u.Id == id);
                _context.Users.Remove(findUser);
                _context.SaveChanges();
                return Json(new
                {
                    status = "OK"
                });

            }
            catch (Exception ex)
            {
                return Json(new
                {
                    status = "ERROR",
                    exception = ex.Message,
                    innerExeption = ex.InnerException

                });
            }

        }
    }
}