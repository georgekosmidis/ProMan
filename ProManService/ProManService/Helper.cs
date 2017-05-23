using SharpSvn;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProManService {
    public static class Helper {


        //public static void InsertProjectHistory( long totalBoc, long revision, int projectRepositoryID, int developerID, DateTime lTime ) {
        //    if ( totalBoc == 0 )
        //        return;
        //    using ( var ctx = new ProManEntities() ) {
        //        var history = new ProManService.ProjectHistory();
        //        history.Bytes = totalBoc;
        //        history.LastDT = lTime;
        //        history.LastRevision = revision;
        //        history.ProjectRepositoryID = projectRepositoryID;
        //        history.DeveloperID = developerID;
        //        ctx.ProjectHistories.Add( history );
        //        try {
        //            ctx.SaveChanges();
        //        }
        //        catch {
        //            try {
        //                var history2 = ctx.ProjectHistories.Where( o =>
        //                        o.ProjectRepositoryID == projectRepositoryID &&
        //                        o.DeveloperID == developerID &&
        //                        o.LastRevision == revision
        //                    ).FirstOrDefault();
        //                if ( history2 != null ) {
        //                    history2.Bytes = totalBoc;
        //                    history2.LastDT = lTime;
        //                    ctx.SaveChanges();
        //                }
        //            }
        //            catch { }
        //        }
        //    }
        //}

        public static void InsertErrorLog( string file, string descr ) {
            using ( var ctx = new ProManEntities() ) {
                var error = new ProManService.ErrorLog();
                error.FileURL = file;
                error.ErrorMessage = descr;
                error.dt = DateTime.Now;
                ctx.ErrorLogs.Add( error );

                try {
                    ctx.SaveChanges();
                }
                catch { }
            }
        }


        public static void UpdateRepositoryRevision( long revision, int repositoryID ) {
            using ( var ctx = new ProManEntities() ) {
                var repo = ctx.Repositories.Where( o => o.ID == repositoryID ).First();
                repo.LastRevision = revision;
                ctx.SaveChanges();
            }
        }


        public static void InsertBOC( int? projectID, int? projectRepositoryID, int developerID, int fileID, int? filetypeID, string fileURL, long bocs, long revision, string action, DateTime lTime ) {
            using ( var ctx = new ProManEntities() ) {
                var oldboc = ctx.BOCs.Where( o =>
                                       
                                        o.DeveloperID == developerID &&
                                        o.FileID == fileID &&
                                        o.RevisionNumber == revision
                    ).FirstOrDefault();

                if ( oldboc != null ) {
                    ctx.BOCs.Remove( oldboc );
                    ctx.SaveChanges();
                }
                
                // o.ProjectID == projectID && o.ProjectRepositoryID == projectRepositoryID &&


                var boc = new ProManService.BOC();
                boc.ProjectID = projectID;
                boc.ProjectRepositoryID = projectRepositoryID;
                boc.DeveloperID = developerID;
                boc.FileTypeID = filetypeID;
                boc.DT = lTime;
                boc.RevisionNumber = revision;
                boc.Bytes = bocs;
                boc.Type = action;
                boc.FileID = fileID;

                //if ( action == "add" )
                //    boc.Change = 1;
                //else if ( action == "del" )
                //    boc.Change = -1;
                //else {
                //    using ( var ctxBoc2 = new ProManEntities() ) {
                //        var opbocs = ctxBoc2.BOCs.Where( o =>
                //                        o.ProjectID == projectID &&
                //                        o.DeveloperID == developerID &&
                //                        o.File.Filename == fileURL
                //                    ).FirstOrDefault();

                //        var pbocs = opbocs == null ? 0 : opbocs.Bytes;
                //        boc.Change = 1 - (decimal)pbocs / bocs;
                //    }
                //}

                ctx.BOCs.Add( boc );
                try {
                    ctx.SaveChanges();
                }
                catch ( Exception ex ) {
                    var message = "";
                    while ( ex != null ) {
                        message = ex.Message + Environment.NewLine;
                        ex = ex.InnerException;
                    }
                    Helper.InsertErrorLog( "InsertBOC(...)", message );
                }
            }
        }


    }
}
