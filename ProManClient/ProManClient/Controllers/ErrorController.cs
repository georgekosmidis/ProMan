using ProManClient.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PylonPriceList.Controllers {
    [AllowAnonymous]
    public class ErrorController : BaseController {

        public ActionResult Index( int code ) {
            switch ( code ) {
                case 500:
                    return E5xx();
                default:
                    return E4xx();
            }
        }

        public ActionResult E5xx() {
            return View( "E5xx" );
        }

        public ActionResult E4xx() {
            return View( "E4xx" );
        }

    }
}