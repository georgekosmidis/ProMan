using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace ProManClient.Controllers {
    public class BaseController : Controller {

        protected enum UserLevel { FullView = 11, TeamView = 7, DevView = 1, Denied = 0 };
        protected UserLevel currentUserLevel = UserLevel.Denied;
        protected List<int> allowedDevelopers = new List<int>();
        protected List<int> allowedProjects = new List<int>();

        readonly ProManEntities proMan = new ProManEntities();

        protected double CalculateEffort( int cocomoMode, float totalBytes, float bpl ) {
            switch ( cocomoMode ) {
                case 0:
                    return 2.4 * Math.Pow( (totalBytes / bpl / 1000), 1.05 );
                case 1:
                    return 3.0 * Math.Pow( (totalBytes / bpl / 1000), 1.12 );
                case 2:
                    return 3.6 * Math.Pow( (totalBytes / bpl / 1000), 1.20 );
            }
            return 0;
        }

        protected double CalculateDevTime( int cocomoMode, float totalBytes, float bpl ) {
            var e = CalculateEffort( cocomoMode, totalBytes, bpl );
            switch ( cocomoMode ) {
                case 0:
                    return 2.5 * Math.Pow( e, 0.38 );
                case 1:
                    return 2.5 * Math.Pow( e, 0.35 );
                case 2:
                    return 2.5 * Math.Pow( e, 0.32 );
            }
            return 0;
        }

        protected override void OnActionExecuting( ActionExecutingContext ctx ) {
            //ctx.ActionDescriptor.ActionName
            //

            if ( ctx.ActionDescriptor.GetCustomAttributes( typeof( AllowAnonymousAttribute ), true ).Any()
                    || ctx.ActionDescriptor.ControllerDescriptor.GetCustomAttributes( typeof( AllowAnonymousAttribute ), true ).Any() )
                return;

            if ( !ctx.HttpContext.User.Identity.IsAuthenticated ) {
                //base.HandleUnauthorizedRequest( ctx );
                ctx.Result = new RedirectToRouteResult(
                        new RouteValueDictionary( new { controller = "Account", action = "Login" } )
                );
                return;
            }
            else {
                var oUser = proMan.Users.Where( o => o.Username == User.Identity.Name ).FirstOrDefault();
                if ( oUser == null ) {
                    var oDev = proMan.Developers.Where( o => o.Username == User.Identity.Name ).FirstOrDefault();
                    if ( oDev == null ) {
                        FormsAuthentication.SignOut();
                        ctx.Result = new RedirectToRouteResult(
                                            new RouteValueDictionary( new { controller = "Account", action = "Login" } )
                                    );
                        return;
                    }
                    else {
                        currentUserLevel = UserLevel.DevView;
                    }

                }
                else {
                    currentUserLevel = (UserLevel)oUser.UserLevel;
                }
            }

            foreach ( var dev in proMan.Developers )
                if ( AllowUserForDev( dev.ID ) )
                    allowedDevelopers.Add( dev.ID );

            foreach ( var proj in proMan.Projects )
                if ( AllowUserForProject( proj.ID ) )
                    allowedProjects.Add( proj.ID );

        }

        protected bool AllowUserForDev( int id ) {
            if ( currentUserLevel == UserLevel.Denied )
                return false;

            if ( currentUserLevel == UserLevel.FullView )
                return true;
            else {
                if ( currentUserLevel == UserLevel.DevView )
                    return proMan.Developers.Where( o => o.Username == User.Identity.Name && o.ID == id ).FirstOrDefault() != null;
                if ( currentUserLevel == UserLevel.TeamView ) {

                    var projects = proMan.BOC.Where( o => o.DeveloperID == id )
                                              .Select( x => x.ProjectID )
                                              .ToList().Distinct();

                    var devs = proMan.BOC.Where( o => projects.Contains( o.ProjectID ) )
                                            .Select( p => p.DeveloperID )
                                            .ToList().Distinct();

                    return devs.Contains( id );
                }
            }
            return false;
        }

        protected bool AllowUserForProject( int id ) {
            if ( currentUserLevel == UserLevel.Denied )
                return false;

            if ( currentUserLevel == UserLevel.FullView )
                return true;
            else {
                var oDev = proMan.Developers.Where( o => o.Username == User.Identity.Name ).First();

                if ( currentUserLevel == UserLevel.DevView )
                    return proMan.BOC.Where( o => o.DeveloperID == oDev.ID && o.ProjectID == id ).FirstOrDefault() != null;
                if ( currentUserLevel == UserLevel.TeamView ) {

                    //var projects = (from r in proMan.BOC
                    //                where r.DeveloperID == id
                    //                select new {
                    //                    ProjectID = (int)r.ProjectID
                    //                }).ToList();

                    //find all projects user is in
                    var projects = proMan.BOC.Where( o => o.DeveloperID == oDev.ID )
                                              .Select( x => x.ProjectID )
                                              .ToList().Distinct();

                    //find all devs that are in this project (teams)
                    var devs = proMan.BOC.Where( o => projects.Contains( o.ProjectID ) )
                                            .Select( p => p.DeveloperID )
                                            .ToList().Distinct();

                    //find all projects that the teams have
                    var projects2 = proMan.BOC.Where( o => devs.Contains( o.DeveloperID ) )
                          .Select( x => x.ProjectID )
                          .ToList().Distinct();

                    return projects2.Contains( id );
                }
            }
            return false;
        }

        public BaseController() {
            if ( PoseidonWeb.Helpers.Classic.Sessions.Get<DateTime>( "StartDate" ) == null || PoseidonWeb.Helpers.Classic.Sessions.Get<DateTime>( "StartDate" ) == DateTime.MinValue ) {
                var daysBack = ProManClient.Helpers.GlobalVariables.InitialDaysBefore;
                PoseidonWeb.Helpers.Classic.Sessions.Set<DateTime>( "StartDate", DateTime.Now.AddDays( -1 * daysBack ) );
                PoseidonWeb.Helpers.Classic.Sessions.Set<DateTime>( "EndDate", DateTime.Now );
            }

        }

        public DateTime StartDate {
            get {
                return PoseidonWeb.Helpers.Classic.Sessions.Get<DateTime>( "StartDate" );
            }
            set {
                PoseidonWeb.Helpers.Classic.Sessions.Set<DateTime>( "StartDate", value );
            }
        }
        public DateTime EndDate {
            get {
                return PoseidonWeb.Helpers.Classic.Sessions.Get<DateTime>( "EndDate" );
            }
            set {
                PoseidonWeb.Helpers.Classic.Sessions.Set<DateTime>( "EndDate", value );
            }
        }


        public string Encrypt( string inString ) {
            byte[] rgbIV = new byte[8]
              {
                (byte) 51,
                (byte) 37,
                (byte) 87,
                (byte) 121,
                (byte) 156,
                (byte) 172,
                (byte) 206,
                (byte) 240
              };
            try {
                byte[] bytes1 = Encoding.UTF8.GetBytes( "pr0man:)" );
                DESCryptoServiceProvider cryptoServiceProvider = new DESCryptoServiceProvider();
                byte[] bytes2 = Encoding.UTF8.GetBytes( inString );
                MemoryStream memoryStream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream( (Stream)memoryStream, cryptoServiceProvider.CreateEncryptor( bytes1, rgbIV ), CryptoStreamMode.Write );
                cryptoStream.Write( bytes2, 0, bytes2.Length );
                cryptoStream.FlushFinalBlock();
                return Convert.ToBase64String( memoryStream.ToArray() );
            }
            catch ( Exception ex ) {
                //Debug.WriteLine( ex.Message );
                return "";
            }
        }

        public string Decrypt( string inString ) {
            byte[] rgbIV = new byte[8]
              {
                (byte) 51,
                (byte) 37,
                (byte) 87,
                (byte) 121,
                (byte) 156,
                (byte) 172,
                (byte) 206,
                (byte) 240
              };
            try {
                byte[] bytes = Encoding.UTF8.GetBytes( "pr0man:)" );
                DESCryptoServiceProvider cryptoServiceProvider = new DESCryptoServiceProvider();
                byte[] buffer = Convert.FromBase64String( inString );
                MemoryStream memoryStream = new MemoryStream();
                cryptoServiceProvider.Key = bytes;
                cryptoServiceProvider.IV = rgbIV;
                CryptoStream cryptoStream = new CryptoStream( (Stream)memoryStream, cryptoServiceProvider.CreateDecryptor( bytes, rgbIV ), CryptoStreamMode.Write );
                cryptoStream.Write( buffer, 0, buffer.Length );
                cryptoStream.FlushFinalBlock();
                return Encoding.UTF8.GetString( memoryStream.ToArray() );
            }
            catch {
                return "";
            }
        }
    }
}