using System;
using System.Linq;
using System.Web.Mvc;

namespace ProManClient.ActionFilters {
    public class MenuItemActionFilterAttribute : ActionFilterAttribute {

        public string Active { get; set; }

        public override void OnActionExecuting( ActionExecutingContext filterContext ) {
            try {
                string active = filterContext.ActionParameters.Count() > 0 && filterContext.ActionParameters.ContainsKey( "id" ) ? Active + filterContext.ActionParameters["id"] : Active;
                PoseidonWeb.Helpers.Classic.Sessions.Set<string>( "isActive", active );
            }
            catch ( Exception ) {

            }
        }
    }
}