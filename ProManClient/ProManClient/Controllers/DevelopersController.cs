using ProManClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Globalization;
using ProManClient.ActionFilters;

namespace ProManClient.Controllers {
    [Authorize]
    public class DevelopersController : BaseController {
        readonly ProManEntities proMan = new ProManEntities();

        // GET: Developers
        [MenuItemActionFilterAttribute( Active = "dev_" )]
        public ActionResult Index( int id ) {

            if ( !AllowUserForDev( id ) || id == null )
                return RedirectToAction( "Index", "Home" );

            PoseidonWeb.Helpers.Classic.Sessions.Set<int>( "DeveloperID", id );

            DevelopersViewModel vm  = (from p in proMan.Developers
                                       where p.ID == id
                                       select new DevelopersViewModel() {
                                           ID = p.ID,
                                           Name = p.Name
                                       }).FirstOrDefault();

            return View( vm );
        }

        [HttpPost]
        public ActionResult GetBOCPerTime() {
            var id = PoseidonWeb.Helpers.Classic.Sessions.Get<int>( "DeveloperID" );

            var start = this.StartDate;
            var end = this.EndDate;
            if ( start.ToString( "d MMM yyyy" ) == end.ToString( "d MMM yyyy" ) ) {
                start = start.AddDays( -1 );
                end = end.AddDays( 1 );
            }

            try {

                IEnumerable<ProjectsViewModel> result = (from p in proMan.BOC
                                                         where p.DT >= start && p.DT <= end && p.DeveloperID == id
                                                         select new ProjectsViewModel {
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

                //IEnumerable<ProjectsViewModel> result2 = (from p in proMan.BOC
                //                                          where p.DT >= this.StartDate && p.DT <= this.EndDate && p.DeveloperID == id
                //                                          group p by p.ProjectID into pg
                //                                          select new ProjectsViewModel {
                //                                              ID = pg.FirstOrDefault().ProjectID,
                //                                              Descr = pg.FirstOrDefault().Project.Descr,
                //                                              Name = pg.FirstOrDefault().Project.Name,
                //                                              Allowed = allowedProjects.Contains( pg.FirstOrDefault().ProjectID ),
                //                                              BPL = pg.FirstOrDefault().FileType.BPL,
                //                                              CocomoMode = pg.FirstOrDefault().ProjectRepository.CocomoMode,
                //                                              TotalBytes = (float)pg.Sum( x => x.Bytes )
                //                                          }).ToList();

                //double bytes1=0; double bytes2 = 0;
                //double time1=0; double time2 = 0;
                //foreach ( var r in result ) {
                //    bytes1 += r.totalbytes;
                //    time1 += r.totaldevtime;
                //}

                //foreach ( var r in result2 ) {
                //    bytes2 += r.TotalBytes;
                //    time2 += r.TotalDevTime;
                //}

                return Json( BOC );
            }
            catch ( Exception ex ) {
                return Json( new { error = ex.Message } );
            }
        }

        [HttpPost]
        public ActionResult GetProjectsList( [DataSourceRequest] DataSourceRequest request ) {
            var id = PoseidonWeb.Helpers.Classic.Sessions.Get<int>( "DeveloperID" );

            try {
                IEnumerable<ProjectsViewModel> result = (from p in proMan.BOC
                                                         where p.DT >= this.StartDate && p.DT <= this.EndDate && p.DeveloperID == id && p.ProjectID != null && p.ProjectRepositoryID != null
                                                         select new ProjectsViewModel {
                                                             ID = p.ProjectID.Value,
                                                             Descr = p.Projects.Descr,
                                                             Name = p.Projects.Name,
                                                             Allowed = allowedProjects.Contains( p.ProjectID.Value ),
                                                             BPL = p.FileTypes.BPL,
                                                             CommitDate = DbFunctions.TruncateTime( p.DT ),
                                                             CocomoMode = p.ProjectRepositories.CocomoMode,
                                                             TotalBytes = p.Bytes
                                                         }).ToList();

                IEnumerable<ListViewModel> projects = (from p in result
                                                       group p by p.ID into pg
                                                       select new ListViewModel {
                                                           ID = pg.FirstOrDefault().ID,
                                                           Name = pg.FirstOrDefault().Name,
                                                           Allowed = pg.FirstOrDefault().Allowed,
                                                           TotalBytes = pg.Sum( x => x.TotalBytes ),
                                                           TotalDevTime = pg.Sum( x => x.TotalDevTime )
                                                       }).ToList().OrderByDescending( o => o.TotalDevTime );
                int counter = 1;
                foreach ( var p in projects )
                    p.OrderNumber = counter++;

                return Json( projects.ToDataSourceResult( request ), JsonRequestBehavior.AllowGet );
            }
            catch ( Exception ex ) {
                return Json( new { error = ex.Message } );
            }

        }


        [HttpPost]
        public ActionResult GetProjectsListPie() {
            var id = PoseidonWeb.Helpers.Classic.Sessions.Get<int>( "DeveloperID" );
            try {

                IEnumerable<ProjectsViewModel> result = (from p in proMan.BOC
                                                         where p.DT >= this.StartDate && p.DT <= this.EndDate && p.DeveloperID == id && p.ProjectID != null && p.ProjectRepositoryID != null
                                                         select new ProjectsViewModel {
                                                             ID = p.ProjectID.Value,
                                                             Descr = p.Projects.Descr,
                                                             Name = p.Projects.Name,
                                                             Allowed = allowedProjects.Contains( p.ProjectID.Value ),
                                                             BPL = p.FileTypes.BPL,
                                                             CommitDate = DbFunctions.TruncateTime( p.DT ),
                                                             CocomoMode = p.ProjectRepositories.CocomoMode,
                                                             TotalBytes = p.Bytes
                                                         }).ToList();

                IEnumerable<PieChartViewModel> projects = (from p in result
                                                           group p by p.ID into pg
                                                           select new PieChartViewModel {
                                                               Name = pg.FirstOrDefault().Name,
                                                               TotalDevTime = pg.Sum( x => x.TotalDevTime )
                                                           }).ToList();
                return Json( projects );
            }
            catch ( Exception ex ) {
                return Json( new { error = ex.Message } );
            }

        }

        public ActionResult GetSearchResults() {
            var id = PoseidonWeb.Helpers.Classic.Sessions.Get<int>( "DeveloperID" );
            try {
                IEnumerable<SearchByViewModel> result = (from p in proMan.BOC
                                                         where p.DT >= this.StartDate && p.DT <= this.EndDate && p.DeveloperID == id && p.ProjectID != null && p.ProjectRepositoryID != null
                                                         select new SearchByViewModel {
                                                             Title = p.Projects.Name,
                                                             ID = p.ProjectID.Value,
                                                             Controller = "Project",
                                                             Allowed = allowedProjects.Contains( p.ProjectID.Value )
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

        [HttpPost]
        public ActionResult GetCommitsList( [DataSourceRequest] DataSourceRequest request, int searchedID = -1 ) {
            var id = PoseidonWeb.Helpers.Classic.Sessions.Get<int>( "DeveloperID" );

            try {
                IEnumerable<CommitsViewModel> result;
                if ( searchedID != -1 ) {
                    result = (from p in proMan.BOC
                              where p.DT >= this.StartDate && p.DT <= this.EndDate && p.DeveloperID == id && p.ProjectID == searchedID
                              group p by DbFunctions.TruncateTime( p.DT ) into g
                              select new CommitsViewModel {
                                  Commits = g.GroupBy( r => r.RevisionNumber ).Count(),
                                  Files = g.Select( x => x.RevisionNumber ).Count(),
                                  DT = g.FirstOrDefault().DT
                              }).ToList().OrderByDescending( o => o.DT );
                }
                else {
                    result = (from p in proMan.BOC
                              where p.DT >= this.StartDate && p.DT <= this.EndDate && p.DeveloperID == id
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


        [HttpPost]
        public ActionResult GetModificationList( [DataSourceRequest] DataSourceRequest request ) {
            var id = PoseidonWeb.Helpers.Classic.Sessions.Get<int>( "DeveloperID" );
            try {
                IEnumerable<ModificationViewModel> result = (from r in proMan.BOC
                                                             where r.DT >= this.StartDate && r.DT <= this.EndDate && r.DeveloperID == id
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
            var id = PoseidonWeb.Helpers.Classic.Sessions.Get<int>( "DeveloperID" );

            try {
                IEnumerable<ModificationViewModel> result = (from r in proMan.BOC
                                                             where r.DT >= this.StartDate && r.DT <= this.EndDate && r.DeveloperID == id
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
            var id = PoseidonWeb.Helpers.Classic.Sessions.Get<int>( "DeveloperID" );
            var period = " <small> for period " + this.StartDate.ToString( "dd.MM.yy" ) + " - " + this.EndDate.ToString( "dd.MM.yy" ) + "</small>";
            List<TotalNumbersViewModel> ivm = new List<TotalNumbersViewModel>();

            //projects
            var vm = new TotalNumbersViewModel();
            vm.Name = "Projects" + period;
            vm.Value = proMan.BOC.Where( p => p.DT >= this.StartDate && p.DT <= this.EndDate && p.DeveloperID == id )
                                           .Select( p => new { id = p.ProjectID } )
                                           .Distinct()
                                           .Count().ToString( "#,##0" );
            vm.Icon = "ion-star";
            vm.BgColor = "bg-blue";
            ivm.Add( vm );

            //Bytes
            vm = new TotalNumbersViewModel();
            vm.Name = "Size" + period;
            try {
                vm.Value = (proMan.BOC.Where( p => p.DT >= this.StartDate && p.DT <= this.EndDate && p.DeveloperID == id )
                                           .Sum( p => p.Bytes ) / 1024).ToString( "#,##0" ) + " KB";
            }
            catch {
                vm.Value = 0.ToString( "#.##" ) + " KB";
            }
            vm.Icon = "ion-beer";
            vm.BgColor = "bg-navy";
            ivm.Add( vm );

            //Commits
            vm = new TotalNumbersViewModel();
            vm.Name = "Commits" + period;
            vm.Value = (proMan.BOC.Where( p => p.DT >= this.StartDate && p.DT <= this.EndDate && p.DeveloperID == id )
                                           .Select( p => new { id = p.RevisionNumber } )
                                           .Distinct()
                                           .Count()).ToString( "#,##0" );
            vm.Icon = "ion-pull-request";
            vm.BgColor = "bg-red";
            ivm.Add( vm );

            ////Files
            //vm = new TotalNumbersViewModel();
            //vm.Name = "Files" + period;
            //vm.Value = (proMan.BOC.Where( p => p.DT >= this.StartDate && p.DT <= this.EndDate && p.DeveloperID == id )
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
                                                       where p.DT >= this.StartDate && p.DT <= this.EndDate && p.DeveloperID == id
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
        public ActionResult Files_Read( [DataSourceRequest] DataSourceRequest request, string date ) {
            DateTime dt;
            DateTime.TryParseExact( date, "M/d/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dt );
            var dtend = dt.AddDays( 1 ).AddSeconds( -1 );

            var id = PoseidonWeb.Helpers.Classic.Sessions.Get<int>( "DeveloperID" );
            //group r by r.FileID 
            try {
                IEnumerable<FilesViewModel> result = (from r in proMan.BOC
                                                      join p in proMan.Projects on r.ProjectID equals p.ID
                                                      join o in proMan.Repositories on p.Developers.FirstOrDefault().ID equals o.ID
                                                      where r.DT >= dt && r.DT <= dtend && r.DeveloperID == id
                                                      select new FilesViewModel() {
                                                          Type = r.Type,
                                                          DeveloperName = r.Developers.Name,
                                                          ProjectName = allowedProjects.Contains( r.Projects.ID ) ? r.Projects.Name : "Hidden Name",
                                                          Url = o.SourceUrl + r.Files.Filename,
                                                          Bytes = r.Bytes,
                                                          ProjectID = r.ProjectID.Value,
                                                          Allowed = allowedProjects.Contains( r.Projects.ID )
                                                      }).ToList();

                return Json( result.ToDataSourceResult( request ), JsonRequestBehavior.AllowGet );
            }
            catch ( Exception ex ) {
                return Json( new { error = ex.Message } );
            }
        }

    }
}