using System.Web.Optimization;

namespace ProManClient.App_Start
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles( BundleCollection bundles ) {
            bundles.Add( new ScriptBundle( "~/bundles/jquery" ).Include(
                "~/Scripts/Libraries/jquery-{version}.js" ) );

         
            bundles.Add( new ScriptBundle( "~/bundles/SiteScripts" ).Include(
                //Bootstrap
                "~/Scripts/Libraries/bootstrap.min.js",
                //Tree menu
                "~/Scripts/Plugins/TreeMenu/TreeMenu.js",
                //icheck
               // "~/Scripts/Plugins/icheck/icheck.min.js",
                //Slim scroll
                "~/Scripts/Plugins/slimScroll/jquery.slimscroll.min.js",
                //daterangepicker
                "~/Scripts/Plugins/daterangepicker/daterangepicker.js",
                //datepicker
                // "~/Scripts/Plugins/datepicker/bootstrap-datepicker.js",
                //ProMan App
                "~/Scripts/ProMan/Layout.js" ) );

            bundles.Add( new ScriptBundle( "~/bundles/jqueryui" ).Include(
            "~/Scripts/Libraries/jquery-ui-{version}.js" ) );

            bundles.Add( new ScriptBundle( "~/bundles/jqueryval" ).Include(
                "~/Scripts/Libraries/jquery.unobtrusive*",
                "~/Scripts/Libraries/jquery.validate*" ) );

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add( new ScriptBundle( "~/bundles/modernizr" ).Include(
                "~/Scripts/Libraries/modernizr-*" ) );

            bundles.Add( new ScriptBundle( "~/bundles/kendo" ).Include(
            "~/Scripts/kendo/2014.3.1119/kendo.all.min.js",
                // "~/Scripts/kendo/kendo.timezones.min.js", // uncomment if using the Scheduler
            "~/Scripts/kendo/2014.3.1119/kendo.aspnetmvc.min.js" ) );

            bundles.Add( new StyleBundle( "~/Content/css" ).Include(
                "~/Content/bootstrap.css",
                "~/Content/Site.css",
                //icheck 
                // "~/Content/icheck/all.css",
                //Date Picker
                // "~/Content/datepicker/datepicker3.css",
                //Daterange picker
                 "~/Content/daterangepicker/daterangepicker-bs3.css",
                //Theme style
                 "~/Content/Site.css" ) );

            bundles.Add( new StyleBundle( "~/Content/kendo/css" ).Include(
                "~/Content/kendo/2014.3.1119/kendo.common.min.css",
                "~/Content/kendo/2014.3.1119/kendo.metro.min.css" ) );

            bundles.Add( new StyleBundle( "~/Content/Login/css" ).Include(
                "~/Content/Login.css" ) );


            bundles.Add( new StyleBundle( "~/Content/themes/base/css" ).Include(
                        "~/Content/themes/base/jquery.ui.core.css",
                        "~/Content/themes/base/jquery.ui.resizable.css",
                        "~/Content/themes/base/jquery.ui.selectable.css",
                        "~/Content/themes/base/jquery.ui.accordion.css",
                        "~/Content/themes/base/jquery.ui.autocomplete.css",
                        "~/Content/themes/base/jquery.ui.button.css",
                        "~/Content/themes/base/jquery.ui.dialog.css",
                        "~/Content/themes/base/jquery.ui.slider.css",
                        "~/Content/themes/base/jquery.ui.tabs.css",
                        "~/Content/themes/base/jquery.ui.datepicker.css",
                        "~/Content/themes/base/jquery.ui.progressbar.css",
                        "~/Content/themes/base/jquery.ui.theme.css" ) );

            System.Web.Optimization.BundleTable.EnableOptimizations = false;
        }
    }
}