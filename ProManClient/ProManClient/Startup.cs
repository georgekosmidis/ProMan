using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ProManClient.App_Start.Startup))]
namespace ProManClient.App_Start
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
