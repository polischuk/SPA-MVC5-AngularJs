using MyApp.data.Repositories;
using Ninject.Modules;
using Ninject.Web.Common;

namespace MyApp.data
{
    public class DataModule : NinjectModule
    {
        public override void Load()
        {
            Bind<ApplicationDbContext>().ToSelf().InRequestScope();
            Bind<IApplicationUserRepository>().To<ApplicationUserRepository>().InRequestScope();
        }
    }

}
