using Microsoft.Owin;
using MyApp.web;
using Owin;

[assembly: OwinStartup(typeof(Startup))]
namespace MyApp.web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
