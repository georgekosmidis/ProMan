using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace ProManClient.Attributes {

    [AttributeUsageAttribute( AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = true )]
    public class ProManAuthorizeAttribute : AuthorizeAttribute {

        public int DevID { get; set; }
        public int ProjectID { get; set; }

        private readonly bool authorizingCurrentPath = true;

        protected override void HandleUnauthorizedRequest( AuthorizationContext ctx ) {
         //var o = ctx.ActionDescriptor.GetCustomAttributes( typeof(AllowAnonymousAttribute), true ).Any();
            if ( ctx.ActionDescriptor.GetCustomAttributes( typeof( AllowAnonymousAttribute ), true ).Any()
                   || ctx.ActionDescriptor.ControllerDescriptor.GetCustomAttributes( typeof( AllowAnonymousAttribute ), true ).Any() )
                return;

            if ( !ctx.HttpContext.User.Identity.IsAuthenticated ) {
                //base.HandleUnauthorizedRequest( ctx );
                ctx.Result = new RedirectToRouteResult(
                        new RouteValueDictionary( new { controller = "Account", action = "Login" } )
                );
            }
            else {
                if ( authorizingCurrentPath ) {
                    // handle controller access
                    ctx.Result = new ViewResult { ViewName = "NotAuthorized" };
                    ctx.HttpContext.Response.StatusCode = 403;
                }
                else {
                    // handle menu links
                    ctx.Result = new HttpUnauthorizedResult();
                    ctx.HttpContext.Response.StatusCode = 403;
                }
            }

            //if ( !String.IsNullOrWhiteSpace( filterContext.HttpContext.Current.User.Identity.Name ) ) {
            //    if ( DevID )
            //        filterContext.Result = new RedirectToRouteResult(
            //                new RouteValueDictionary( new { controller = "Account", action = "Login" } )
            //        );
            //}
            //else {
            //    filterContext.Result = new RedirectToRouteResult(
            //            new RouteValueDictionary( new { controller = "Account", action = "NotAuthorized" } )
            //    );
            //}
        }

        public override void OnAuthorization( AuthorizationContext ctx ) {

        //    base.OnAuthorization( ctx );

        //    // this is overriden for kendo menus to hide 
        //    var ctrl = ctx.RequestContext.RouteData.GetRequiredString( "controller" );
        //    var action = ctx.ActionDescriptor.ActionName;

        //    //if ( ctrl == "DevelopersController" ) {
        //    //    var DevID = PoseidonWeb.Helpers.Classic.Sessions.Get<int>( "DeveloperID" );
        //    //    if(action.ToLower() == "Index" 
        //    //        && ctx.ActionDescriptor.GetParameters().Where( o => o.ParameterName == "id").Count() > 0)
        //    //        DevID = ctx.ActionDescriptor.GetParameters().Where( o => o.ParameterName == "id").First().BindingInfo.

        //    //}
        //    //else if ( ctrl == "ProjectController" ) {
        //    //     var ProjID = PoseidonWeb.Helpers.Classic.Sessions.Get<int>( "ProjectID" );
        //    //}
        //    //else {

            //}



        //    //// useful to determine if it's authorizing current controller path or menu links
        //    var path = ctx.HttpContext.Request.PhysicalPath;
        //    _authorizingCurrentPath = path.Contains( ctrl ) || path.EndsWith( "WebUI" );


        //    //if ( userAuth < requiredAuth )
        //    //    HandleUnauthorizedRequest( filterContext );

        }


        protected override bool AuthorizeCore( HttpContextBase httpContext ) {
            return httpContext.User.Identity.IsAuthenticated;
        }
    }
}