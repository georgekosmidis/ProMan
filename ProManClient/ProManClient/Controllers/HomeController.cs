using ProManClient.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using ProManClient.Helpers;
using ProManClient.ActionFilters;

namespace ProManClient.Controllers {
    [Authorize]
    public class HomeController : BaseController {

        readonly ProManEntities proMan = new ProManEntities();

        [MenuItemActionFilterAttribute( Active = "home" )]
        public ActionResult Index() {
            return View();
        }

        [HttpPost]
        public ActionResult GetBOCPerTime() {

            try {
                var start = this.StartDate;
                var end = this.EndDate;
                if ( start.ToString( "d MMM yyyy" ) == end.ToString( "d MMM yyyy" ) ) {
                    start = start.AddDays( -1 );
                    end = end.AddDays( 1 );
                }

                IEnumerable<ProjectsViewModel> result = (from p in proMan.BOC
                                                         where p.DT >= start && p.DT <= end && p.Developers.Username != null
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

                return Json( BOC );
            }
            catch ( Exception ex ) {
                return Json( new { error = ex.Message } );
            }
        }

        [HttpPost]
        public ActionResult GetProjectsList( [DataSourceRequest] DataSourceRequest request ) {
            try {
                IEnumerable<ProjectsViewModel> result = (from p in proMan.BOC
                                                         where p.DT >= this.StartDate && p.DT <= this.EndDate && p.ProjectID != null && p.ProjectRepositoryID != null && p.Developers.Username != null
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
            try {
                IEnumerable<ProjectsViewModel> result = (from p in proMan.BOC
                                                         where p.DT >= this.StartDate && p.DT <= this.EndDate && p.Developers.Username != null
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

        [HttpPost]
        public ActionResult GetDevelopersList( [DataSourceRequest] DataSourceRequest request ) {
            try {
                IEnumerable<DevelopersViewModel> result = (from p in proMan.BOC
                                                           where p.DT >= this.StartDate && p.DT <= this.EndDate && p.Developers.Username != null
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
                                                             Name = pg.FirstOrDefault().Name,
                                                             Allowed = pg.FirstOrDefault().Allowed,
                                                             TotalBytes = pg.Sum( x => x.TotalBytes ),
                                                             TotalDevTime = pg.Sum( x => x.TotalDevTime )
                                                         }).ToList().OrderByDescending( o => o.TotalDevTime );

                int counter = 0;
                foreach ( var d in developers ) {
                    d.Allowed = counter < GlobalVariables.TopSelectd ? true : d.Allowed;
                    d.OrderNumber = ++counter;
                }

                return Json( developers.ToDataSourceResult( request ), JsonRequestBehavior.AllowGet );
            }
            catch ( Exception ex ) {
                return Json( new { error = ex.Message } );
            }

        }

        [HttpPost]
        public ActionResult GetDevelopersListPie() {
            try {
                IEnumerable<DevelopersViewModel> result = (from p in proMan.BOC
                                                           where p.DT >= this.StartDate && p.DT <= this.EndDate && p.Developers.Username != null
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
                foreach ( var dev in developers.Take( GlobalVariables.TopSelectd ) ) {
                    developersPieData.Add( new PieChartViewModel() { Name = dev.Name, TotalDevTime = dev.TotalDevTime } );
                }
                if ( developersPieData.Count() < developers.Count() ) {
                    developersPieData.Add( new PieChartViewModel() { Name = "Rest", TotalDevTime = 0 } );
                    foreach ( var dev in developers.Skip( GlobalVariables.TopSelectd ) ) {
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
            try {
                IEnumerable<ModificationViewModel> result = (from r in proMan.BOC
                                                             where r.DT >= this.StartDate && r.DT <= this.EndDate && r.Developers.Username != null
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
            try {
                IEnumerable<ModificationViewModel> result = (from r in proMan.BOC
                                                             where r.DT >= this.StartDate && r.DT <= this.EndDate && r.Developers.Username != null
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
            List<TotalNumbersViewModel> ivm = new List<TotalNumbersViewModel>();

            var period = " <small> for period " + this.StartDate.ToString( "dd.MM.yy" ) + " - " + this.EndDate.ToString( "dd.MM.yy" ) + "</small>";
            //projects
            var vm = new TotalNumbersViewModel();
            vm.Name = "Projects" + period;
            vm.Value = proMan.BOC.Where( p => p.DT >= this.StartDate && p.DT <= this.EndDate && p.Developers.Username != null)
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
                vm.Value = (proMan.BOC.Where( p => p.DT >= this.StartDate && p.DT <= this.EndDate && p.Developers.Username != null)
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
            vm.Value = (proMan.BOC.Where( p => p.DT >= this.StartDate && p.DT <= this.EndDate && p.Developers.Username != null)
                                           .Select( p => new { id = p.RevisionNumber } )
                                           .Distinct()
                                           .Count()).ToString( "#,##0" );
            vm.Icon = "ion-pull-request";
            vm.BgColor = "bg-red";
            ivm.Add( vm );

            ////Files
            //vm = new TotalNumbersViewModel();
            //vm.Name = "Files" + period;
            //vm.Value = (proMan.BOC.Where( p => p.DT >= this.StartDate && p.DT <= this.EndDate )
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
                                                       where p.DT >= this.StartDate && p.DT <= this.EndDate && p.ProjectID != null && p.ProjectRepositoryID != null && p.Developers.Username != null
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
    }
}
