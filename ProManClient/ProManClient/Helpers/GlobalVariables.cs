
using System.Web;
namespace ProManClient.Helpers {
    public class GlobalVariables {
        //The number of days back to show initial charts results
        public static int InitialDaysBefore = 6;

        //The top listed developers or projects according to kbytes
        public static int TopSelectd = 3;

        //The Current Route Url
        public static string CurrentRouteUrl = Helper.Setting<string>( "BaseURL" );
    }
}