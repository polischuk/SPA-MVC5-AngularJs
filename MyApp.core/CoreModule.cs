using Ninject.Modules;

namespace MyApp.core
{
   public class CoreModule : NinjectModule
    {
       public override void Load()
       {
           Bind<IApplicationSettings>().To<ProductionApplicationSettings>().InSingletonScope();
       }
    }

}
