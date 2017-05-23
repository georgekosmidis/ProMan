using ProManClient.Helpers;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using ProManClient.ViewModels;

namespace ProManClient.Controllers {
    public class AccountController : BaseController {

        private readonly ProManEntities proMan = new ProManEntities();

        [AllowAnonymous]
        public ActionResult Login() {
            return this.View();
        }

        [HttpPost]
        [AllowAnonymous]
        public ActionResult Login( LoginModel model, string returnUrl ) {
            if ( !this.ModelState.IsValid ) {
                return this.View( model );
            }

            /******************************************************************************************/
            //Convert password to ecrypted password
            using ( var ctx = new ProManEntities() ) {
                var user = ctx.Users.Where( x => x.Username == model.UserName ).FirstOrDefault();
                if ( user != null )
                    if ( user.Password.StartsWith( "#!#" ) ) {
                        user.Password = base.Encrypt( user.Password.Substring( 3, user.Password.Length - 3 ).ToString() );
                    }
                ctx.SaveChanges();
            }
            /******************************************************************************************/

            var modelPassword = base.Encrypt( model.Password );
            bool validateSqlUser = proMan.Users.Any( x => x.Username == model.UserName && x.Password == modelPassword );

            if ( Membership.ValidateUser( model.UserName, model.Password ) || validateSqlUser ) {
                FormsAuthentication.SetAuthCookie( model.UserName, model.RememberMe );
                if ( this.Url.IsLocalUrl( returnUrl ) && returnUrl.Length > 1 && returnUrl.StartsWith( "/" )
                    && !returnUrl.StartsWith( "//" ) && !returnUrl.StartsWith( "/\\" ) ) {
                    return this.Redirect( returnUrl );
                }

                return this.RedirectToAction( "Index", "Home" );
            }

            this.ModelState.AddModelError( string.Empty, Helper.GetResStr( "lblLoginError", "Site" ) );

            return this.View( model );
        }

        [AllowAnonymous]
        public ActionResult LogOff() {
            FormsAuthentication.SignOut();

            return this.RedirectToAction( "Index", "Home" );
        }


    }
}