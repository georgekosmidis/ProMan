using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using ProManClient.App_Start;

namespace ProManClient
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            ProManClient.App_Start.FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            ProManClient.App_Start.BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}
