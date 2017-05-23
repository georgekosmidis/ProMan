using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ProManClient.ActionFilters;
using ProManClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace ProManClient.Controllers {
    [Authorize]
    public class ProjectController : BaseController {
        readonly ProManEntities proMan = new ProManEntities();

        // GET: Project
        [MenuItemActionFilterAttribute( Active = "project_" )]
        public ActionResult Index( int id ) {

            if ( !AllowUserForProject( id ) || id == null )
                return RedirectToAction( "Index", "Home" );

            var oProj = proMan.Projects.Where( o => o.ID == id ).FirstOrDefault();
            if ( oProj == null )
                return RedirectToAction( "Index", "Home" );

            var oDev = proMan.Developers.Where( o => o.Username == User.Identity.Name ).FirstOrDefault();
            if ( oDev == null )
                return RedirectToAction( "Index", "Home" );

            PoseidonWeb.Helpers.Classic.Sessions.Set<int>( "ProjectID", id );

            ProjectsViewModel vm  = (from p in proMan.Projects
                                     where p.ID == id
                                     select new ProjectsViewModel() {
                                         ID = p.ID,
                                         Name = p.Name,
                                         Descr = p.Descr
                                     }).FirstOrDefault();

            return View( vm );
        }

        [HttpPost]
        public ActionResult GetBOCPerTime() {

            var id = PoseidonWeb.Helpers.Classic.Sessions.Get<int>( "ProjectID" );

            var start = this.StartDate;
            var end = this.EndDate;
            if ( start.ToString( "d MMM yyyy" ) == end.ToString( "d MMM yyyy" ) ) {
                start = start.AddDays( -1 );
                end = end.AddDays( 1 );
            }

            try {

                IEnumerable<DevelopersViewModel> result = (from p in proMan.BOC
                                                           where p.DT >= start && p.DT <= end && p.ProjectID == id && p.Developers.Username != null
                                                           select new DevelopersViewModel {
                                                               Name = p.Projects.Name,
                                                               BPL = p.FileTypes.BPL,
                                                               CommitDate = DbFunctions.TruncateTime( p.DT ),
                                                               CocomoMode = p.ProjectRepositories.CocomoMode,
                                                               TotalBytes = p.Bytes
                                                           }).ToList();


                IEnumerable<BOCPerTimeViewModel> BOC   = (from p in result
                                                            group p by p.CommitDate into g
                                                            select new BOCPerTimeViewModel() {
                                                                CommitDate = g.Key,
                                                                TotalDevTime = g.Sum( x => x.TotalDevTime )
                                                            }).ToList();

                return Json( BOC );
            }
            catch ( Exception ex ) {
                return Json( new { error = ex.Message } );
            }
        }

        [HttpPost]
        public ActionResult GetDevelopersList( [DataSourceRequest] DataSourceRequest request ) {
            var id = PoseidonWeb.Helpers.Classic.Sessions.Get<int>( "ProjectID" );

            try {
                IEnumerable<DevelopersViewModel> result = (from p in proMan.BOC
                                                           where p.DT >= this.StartDate && p.DT <= this.EndDate && p.ProjectID == id && p.Developers.Username != null
                                                           select new DevelopersViewModel {
                                                               ID = p.DeveloperID,
                                                               Name = p.Developers.Name,
                                                               Allowed = allowedDevelopers.Contains( p.DeveloperID ),
                                                               BPL = p.FileTypes.BPL,
                                                               CommitDate = DbFunctions.TruncateTime( p.DT ),
                                                               CocomoMode = p.ProjectRepositories.CocomoMode,
                                                               TotalBytes = p.Bytes
                                                           }).ToList();

                IEnumerable<ListViewModel> developers = (from p in result
                                                         group p by p.ID into pg
                                                         select new ListViewModel {
                                                             ID = pg.FirstOrDefault().ID,
                                                             Name = pg.FirstOrDefault().Name ?? "Dev Not Monitored",
                                                             Allowed = pg.FirstOrDefault().Allowed,
                                                             TotalBytes = pg.Sum( x => x.TotalBytes ),
                                                             TotalDevTime = pg.Sum( x => x.TotalDevTime ),
                                                         }).ToList().OrderByDescending( o => o.TotalDevTime );

                int counter = 1;
                foreach ( var d in developers )
                    d.OrderNumber = counter++;

                return Json( developers.ToDataSourceResult( request ), JsonRequestBehavior.AllowGet );
            }
            catch ( Exception ex ) {
                return Json( new { error = ex.Message } );
            }

        }

        [HttpPost]
        public ActionResult GetDevelopersListPie() {
            var id = PoseidonWeb.Helpers.Classic.Sessions.Get<int>( "ProjectID" );
            try {

                IEnumerable<DevelopersViewModel> result = (from p in proMan.BOC
                                                           where p.DT >= this.StartDate && p.DT <= this.EndDate && p.ProjectID == id && p.Developers.Username != null
                                                           select new DevelopersViewModel {
                                                               ID = p.DeveloperID,
                                                               Name = p.Developers.Name,
                                                               Allowed = allowedDevelopers.Contains( p.DeveloperID ),
                                                               BPL = p.FileTypes.BPL,
                                                               CommitDate = DbFunctions.TruncateTime( p.DT ),
                                                               CocomoMode = p.ProjectRepositories.CocomoMode,
                                                               TotalBytes = p.Bytes
                                                           }).ToList();

                IEnumerable<ListViewModel> developers = (from p in result
                                                         group p by p.ID into pg
                                                         select new ListViewModel {
                                                             Name = pg.FirstOrDefault().Name,
                                                             Allowed = pg.FirstOrDefault().Allowed,
                                                             TotalDevTime = pg.Sum( x => x.TotalDevTime )
                                                         }).ToList().OrderByDescending( o => o.TotalDevTime );

                List<PieChartViewModel> developersPieData = new List<PieChartViewModel>();
                foreach ( var dev in developers.Where( x => x.Allowed == true ) ) {
                    developersPieData.Add( new PieChartViewModel() { Name = dev.Name, TotalDevTime = dev.TotalDevTime } );
                }
                if ( developersPieData.Count() < developers.Count() ) {
                    developersPieData.Add( new PieChartViewModel() { Name = "Rest", TotalDevTime = 0 } );
                    foreach ( var dev in developers.Where( x => x.Allowed == false ) ) {
                        developersPieData.Where( x => x.Name == "Rest" ).FirstOrDefault().TotalDevTime += dev.TotalDevTime;
                    }
                }
                return Json( developersPieData );
            }
            catch ( Exception ex ) {
                return Json( new { error = ex.Message } );
            }
        }

        [HttpPost]
        public ActionResult GetModificationList( [DataSourceRequest] DataSourceRequest request ) {
            var id = PoseidonWeb.Helpers.Classic.Sessions.Get<int>( "ProjectID" );
            try {
                IEnumerable<ModificationViewModel> result = (from r in proMan.BOC
                                                             where r.DT >= this.StartDate && r.DT <= this.EndDate && r.ProjectID == id && r.Developers.Username != null
                                                             group r by r.Type into g
                                                             select new ModificationViewModel() {
                                                                 Type = g.FirstOrDefault().Type,
                                                                 TotalBytes = Math.Abs( (float)g.Sum( x => x.Bytes ) )
                                                             }).ToList();

                return Json( result.ToDataSourceResult( request ), JsonRequestBehavior.AllowGet );
            }
            catch ( Exception ex ) {
                return Json( new { error = ex.Message } );
            }
        }

        [HttpPost]
        public ActionResult GetModificationListPie() {
            var id = PoseidonWeb.Helpers.Classic.Sessions.Get<int>( "ProjectID" );
            try {
                IEnumerable<ModificationViewModel> result = (from r in proMan.BOC
                                                             where r.DT >= this.StartDate && r.DT <= this.EndDate && r.ProjectID == id && r.Developers.Username != null
                                                             group r by r.Type into g
                                                             select new ModificationViewModel() {
                                                                 Type = g.FirstOrDefault().Type,
                                                                 TotalBytes = Math.Abs( (float)g.Sum( x => x.Bytes ) )
                                                             }).ToList();

                return Json( result );
            }
            catch ( Exception ex ) {
                return Json( new { error = ex.Message } );
            }
        }

        [HttpPost]
        public ActionResult GetTotalNumbers( [DataSourceRequest] DataSourceRequest request ) {
            var id = PoseidonWeb.Helpers.Classic.Sessions.Get<int>( "ProjectID" );
            List<TotalNumbersViewModel> ivm = new List<TotalNumbersViewModel>();
            var period = " <small> for period " + this.StartDate.ToString( "dd.MM.yy" ) + " - " + this.EndDate.ToString( "dd.MM.yy" ) + "</small>";

            //Developers
            var vm = new TotalNumbersViewModel();
            vm.Name = "Developers" + period;
            vm.Value = proMan.BOC.Where( p => p.DT >= this.StartDate && p.DT <= this.EndDate && p.ProjectID == id && p.Developers.Username != null)
                                           .Select( p => new { id = p.DeveloperID } )
                                           .Distinct()
                                           .Count().ToString( "#,##0" );
            vm.Icon = "ion-man";
            vm.BgColor = "bg-yellow";
            ivm.Add( vm );

            //Bytes
            vm = new TotalNumbersViewModel();
            vm.Name = "Size" + period;
            try {
                vm.Value = (proMan.BOC.Where( p => p.DT >= this.StartDate && p.DT <= this.EndDate && p.ProjectID == id && p.Developers.Username != null)
                                           .Sum( p => p.Bytes ) / 1024).ToString( "#.##" ) + " KB";
            }
            catch {
                vm.Value = 0.ToString( "#,##0" ) + " KB";
            }
            vm.Icon = "ion-beer";
            vm.BgColor = "bg-navy";
            ivm.Add( vm );

            //Commits
            vm = new TotalNumbersViewModel();
            vm.Name = "Commits" + period;
            vm.Value = (proMan.BOC.Where( p => p.DT >= this.StartDate && p.DT <= this.EndDate && p.ProjectID == id && p.Developers.Username != null)
                                           .Select( p => new { id = p.RevisionNumber } )
                                           .Distinct()
                                           .Count()).ToString( "#,##0" );
            vm.Icon = "ion-pull-request";
            vm.BgColor = "bg-red";
            ivm.Add( vm );

            ////Files
            //vm = new TotalNumbersViewModel();
            //vm.Name = "Files" + period;
            //vm.Value = (proMan.BOC.Where( p => p.DT >= this.StartDate && p.DT <= this.EndDate && p.ProjectID == id )
            //                               .Select( p => new { id = p.FileID } )
            //                               .Distinct()
            //                               .Count()).ToString( "#,##0" );
            //vm.Icon = "ion-document-text";
            //vm.BgColor = "bg-purple";
            //ivm.Add( vm );

            //Development Time
            vm = new TotalNumbersViewModel();
            vm.Name = "Development Months" + period;

            IEnumerable<ProjectsViewModel> projects = (from p in proMan.BOC
                                                       where p.DT >= this.StartDate && p.DT <= this.EndDate && p.ProjectID == id && p.Developers.Username != null
                                                       select new ProjectsViewModel {
                                                           BPL = p.FileTypes.BPL,
                                                           CommitDate = DbFunctions.TruncateTime( p.DT ),
                                                           CocomoMode = p.ProjectRepositories.CocomoMode,
                                                           TotalBytes = p.Bytes
                                                       }).ToList();
            double sum = 0;
            foreach ( var item in projects )
                sum += item.TotalDevTime;
            vm.Value = sum == 0 ? "0" : sum.ToString( "#.##" );

            vm.Icon = "ion-clock";
            vm.BgColor = "bg-purple";
            ivm.Add( vm );

            return Json( ivm.ToDataSourceResult( request ), JsonRequestBehavior.AllowGet ); //PartialView( "../Shared/Widgets/TotalNumbers", vm );

        }

        [HttpPost]
        public ActionResult GetCommitsList( [DataSourceRequest] DataSourceRequest request, int searchedID = -1 ) {
            var id = PoseidonWeb.Helpers.Classic.Sessions.Get<int>( "ProjectID" );

            try {
                IEnumerable<CommitsViewModel> result;
                if ( searchedID != -1 ) {
                    result = (from p in proMan.BOC
                              where p.DT >= this.StartDate && p.DT <= this.EndDate && p.ProjectID == id && p.DeveloperID == searchedID && p.Developers.Username != null
                              group p by DbFunctions.TruncateTime( p.DT ) into g
                              select new CommitsViewModel {
                                  Commits = g.GroupBy( r => r.RevisionNumber ).Count(),
                                  Files = g.Select( x => x.RevisionNumber ).Count(),
                                  DT = g.FirstOrDefault().DT
                              }).ToList().OrderByDescending( o => o.DT );
                }
                else {
                    result = (from p in proMan.BOC
                              where p.DT >= this.StartDate && p.DT <= this.EndDate && p.ProjectID == id
                              group p by DbFunctions.TruncateTime( p.DT ) into g
                              select new CommitsViewModel {
                                  Commits = g.GroupBy( r => r.RevisionNumber ).Count(),
                                  Files = g.Select( x => x.RevisionNumber ).Count(),
                                  DT = g.FirstOrDefault().DT
                              }).ToList().OrderByDescending( o => o.DT );
                }

                return Json( result.ToDataSourceResult( request ), JsonRequestBehavior.AllowGet );
            }
            catch ( Exception ex ) {
                return Json( new { error = ex.Message } );
            }
        }

        public ActionResult Files_Read( [DataSourceRequest] DataSourceRequest request, string date ) {
            DateTime dt;
            DateTime.TryParseExact( date, "M/d/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt );
            var dtend = dt.AddDays( 1 ).AddSeconds( -1 );

            var id = PoseidonWeb.Helpers.Classic.Sessions.Get<int>( "ProjectID" );

            try {
                IEnumerable<FilesViewModel> result = (from r in proMan.BOC
                                                      join p in proMan.Projects on r.ProjectID equals p.ID
                                                      join o in proMan.Repositories on p.ProjectRepositories.FirstOrDefault().ID equals o.ID
                                                      where r.DT >= dt && r.DT <= dtend && r.ProjectID == id 
                                                      select new FilesViewModel() {
                                                          Type = r.Type,
                                                          DeveloperName = allowedDevelopers.Contains( r.Developers.ID ) ? r.Developers.Name : "Hidden Name",
                                                          ProjectName = r.Projects.Name,
                                                          Url = o.SourceUrl + r.Files.Filename,
                                                          Bytes = r.Bytes,
                                                          Allowed = allowedDevelopers.Contains( r.Developers.ID ) 
                                                      }).ToList();


                return Json( result.ToDataSourceResult( request ), JsonRequestBehavior.AllowGet );
            }
            catch ( Exception ex ) {
                return Json( new { error = ex.Message } );
            }
        }

        public ActionResult GetSearchResults() {
            var id = PoseidonWeb.Helpers.Classic.Sessions.Get<int>( "ProjectID" );
            try {
                IEnumerable<SearchByViewModel> result = (from p in proMan.BOC
                                                         where p.DT >= this.StartDate && p.DT <= this.EndDate && p.ProjectID == id && p.Developers.Username != null
                                                         select new SearchByViewModel {
                                                             Title = p.Developers.Name,
                                                             ID = p.DeveloperID,
                                                             Controller = "Developers",
                                                             Allowed = allowedDevelopers.Contains( p.DeveloperID )
                                                         }).Distinct();

                List<SearchByViewModel> reducedList = result.Where( element => element.Allowed == true ).ToList();
                reducedList.Add( new SearchByViewModel() { Allowed = true, ID = -1, Title = "All" } );
                reducedList = reducedList.OrderBy( o => o.ID ).ToList();
                return Json( reducedList, JsonRequestBehavior.AllowGet );
            }
            catch ( Exception ex ) {
                return Json( new { error = ex.Message } );
            }
        }

    }
}