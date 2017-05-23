using ProManClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace ProManClient.Controllers {
    public class HelperController : BaseController {
        readonly ProManEntities proMan = new ProManEntities();

        public HelperController() {

        }

        [HttpPost]
        public void SetDates( DateTime startDate, DateTime endDate ) {
            this.StartDate = startDate;
            this.EndDate = endDate.AddDays( 1 ).AddSeconds( -1 );
        }

        public ActionResult GetLeftMenu() {
            IEnumerable<MenuViewModel> projects = (from p in proMan.BOC
                                                   group p by p.ProjectID into pg
                                                   join proj in proMan.Projects on pg.FirstOrDefault().ProjectID equals proj.ID
                                                   select new MenuViewModel {
                                                       ID = pg.FirstOrDefault().ProjectID.Value,
                                                       Title = pg.FirstOrDefault().Projects.Name,
                                                       HasChildren = false,
                                                       Controller = "Project",
                                                       Allowed = allowedProjects.Contains( pg.FirstOrDefault().ProjectID.Value )
                                                   }).ToList().OrderByDescending( o => o.Allowed );


            IEnumerable<MenuViewModel> developers = (from r in proMan.Developers
                                                     where r.Username != null
                                                     select new MenuViewModel() {
                                                         ID = r.ID,
                                                         Title = r.Name,
                                                         HasChildren = false,
                                                         Controller = "Developers",
                                                         Allowed = allowedDevelopers.Contains( r.ID )
                                                     }).ToList().OrderByDescending( o => o.Allowed );



            List<MenuViewModel> menu = new List<MenuViewModel>();
            menu.Add( new MenuViewModel() { Title = "Dashboard", Allowed = true, Key = "home", Icon = "fa fa-dashboard", HasChildren = false, Action = "Index", Controller = "Home", Active = GetisActive( "home" ) } );

            //Add projects
            var active = "default";
            foreach ( var p in projects ) {
                p.Active = GetisActive( "project_" + p.ID );
                if ( p.Active == "active" ) {
                    active = p.Active;
                }
            }
            menu.Add( new MenuViewModel() {
                Title = "Projects",
                Icon = "fa fa-star",
                HasChildren = true,
                Active = active,
                Children = projects
            } );

            //Add developers
            active = "default";
            foreach ( var p in developers ) {
                p.Active = GetisActive( "dev_" + p.ID );
                if ( p.Active == "active" ) {
                    active = p.Active;
                }
            }
            menu.Add( new MenuViewModel() {
                Title = "Developers",
                Icon = "glyphicon glyphicon-user",
                HasChildren = true,
                Active = active,
                Children = developers
            } );


            return PartialView( "../Shared/LayoutPartials/LeftMenu", menu );
        }

        public string GetisActive( string key ) {
            string activeKey = PoseidonWeb.Helpers.Classic.Sessions.Get<string>( "isActive" );
            return key == activeKey ? "active" : "default";
        }

        public ActionResult GetSearchResults( string text ) {
            try {
                if ( !String.IsNullOrEmpty( text ) ) {
                    IEnumerable<SearchByViewModel> developers = (from r in proMan.BOC
                                                                 where r.Developers.Name.Contains( text )
                                                                 select new SearchByViewModel() {
                                                                     Title = r.Developers.Name,
                                                                     ID = r.DeveloperID,
                                                                     Controller = "Developers",
                                                                     Allowed = allowedDevelopers.Contains( r.DeveloperID )
                                                                 }).Distinct().ToList();

                    IEnumerable<SearchByViewModel> projects = (from r in proMan.BOC
                                                               where r.Projects.Name.Contains( text )
                                                               select new SearchByViewModel() {
                                                                   Title = r.Projects.Name,
                                                                   ID = r.ProjectID.Value,
                                                                   Controller = "Project",
                                                                   Allowed = allowedProjects.Contains( r.ProjectID.Value )
                                                               }).Distinct().ToList();

                    IEnumerable<SearchByViewModel> result = developers.Concat( projects ).Where( element => element.Allowed == true );
                    return Json( result, JsonRequestBehavior.AllowGet );
                }
                return null;
            }
            catch ( Exception ex ) {
                return Json( new { error = ex.Message } );
            }
            
        }

    }
}