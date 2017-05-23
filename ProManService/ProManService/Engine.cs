using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using ProManService.tfs;
using SharpSvn;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;


namespace ProManService
{
    public static class Engine
    {

        // Correct entries in BOC when new project/projectrepository is added
        //UPDATE b 
        //SET b.ProjectID = t.ProjectID, b.ProjectRepositoryID = t.ProjectRepositoryID
        //FROM BOC b
        //INNER JOIN (
        //    SELECT b.ID AS BocID, pr.ID AS ProjectRepositoryID, pr.ProjectID FROM BOC b 
        //        INNER JOIN Files f ON b.FileID = f.ID
        //        INNER JOIN ProjectRepositories pr ON REPLACE(REPLACE(SUBSTRING(f.Filename, 1, LEN(pr.Path)+1), '//', '/'), ':/', '://') = pr.Path
        //) t ON b.ID = t.BocID


        public static void Start()
        {
            WindowsIdentity currentIdentity = WindowsIdentity.GetCurrent();
            var username = currentIdentity.Name;

            //if username has not domain EPSILONET
            if (username.Substring(0, "EPSILONET".Length).ToUpper() != "EPSILONET")
            {
                return;
            }

            using (var proManEntities = new ProManEntities())
            {

                //enumerate svn svnLogEventArgsList
                DbSet<Developer> developers = proManEntities.Developers;
                DbSet<FileType> fileTypes = proManEntities.FileTypes;
                DbSet<ProjectRepository> projectRepositories = proManEntities.ProjectRepositories;

                #region SVN

                using (SvnClient svnClient = new SvnClient())
                {

                    //Get SVN respository credentials
                    List<Repository> svnRepositories = proManEntities.Repositories.Where(repository => repository.Type == "svn").ToList<Repository>();
                    foreach (var svnRepository in svnRepositories)
                    {
                        /******************************************************************************************/
                        //Convert password to ecrypted password
                        using (var ctx = new ProManEntities())
                        {
                            // Repository oo = proManEntities.Repositories.Where( repository => repository.Name == "SVN" ).FirstOrDefault();
                            //if ( oo != null )
                            var ctx_r = ctx.Repositories.Where(x => x.ID == svnRepository.ID).FirstOrDefault();
                            if (ctx_r.RootPassword.StartsWith("#!#"))
                            {
                                ctx_r.RootPassword = Libs.Encrypt(ctx_r.RootPassword.Substring(3, ctx_r.RootPassword.Length - 3).ToString());
                                svnRepository.RootPassword = Libs.Encrypt(ctx_r.RootPassword.Substring(3, ctx_r.RootPassword.Length - 3).ToString());
                            }
                            ctx.SaveChanges();
                        }
                        /******************************************************************************************/

                        //Authenticate SVN
                        svnClient.Authentication.DefaultCredentials = new NetworkCredential(svnRepository.RootUsername, Libs.Decrypt(svnRepository.RootPassword));

                        //Get projects repositories and iterate them
                        //List<ProjectRepository> projectsRepositories = proManEntities.ProjectRepositories.Where( projectRepository => projectRepository.Project.RepositoryID == svnRepositoryCredentials.ID && projectRepository.IsActive ).ToList();

                        Libs.WriteLine("Initiating: " + svnRepository.Name);

                        //iterate projects repositories
                        // foreach ( var projectRepository in projectsRepositories ) {
                        Collection<SvnLogEventArgs> svnLogEventArgsCollection;
                        SvnLogArgs args = new SvnLogArgs(new SvnRevisionRange(svnRepository.LastRevision, Int64.MaxValue));
                        svnClient.GetLog(new Uri(svnRepository.BaseUrl), args, out svnLogEventArgsCollection);

                        Libs.WriteLine("Scanning: " + svnRepository.Name);
                        //Get svn event args of each repository with larger revision than last revision
                        List<SvnLogEventArgs> svnLogEventArgsList = svnLogEventArgsCollection.Where(eventArgs => eventArgs.Revision > svnRepository.LastRevision).OrderBy(eventArgs => eventArgs.Revision).ToList();

                        long maxRevision = 0;
                        long totalBytesOfCode = 0;


                        foreach (SvnLogEventArgs svnLogEventArg in svnLogEventArgsList)
                        {

                            Libs.WriteLine("        r" + svnLogEventArg.Revision);

                            //check for developer
                            var developer = developers.Where(dev => dev.SvnUser == svnLogEventArg.Author || dev.SvnUser2 == svnLogEventArg.Author).FirstOrDefault();

                            if (developer == null)
                            {
                                /******************************************************************************************/
                                using (var ctx = new ProManEntities())
                                {
                                    var ctx_d = new Developer();
                                    ctx_d.SvnUser = svnLogEventArg.Author;
                                    ctx.Developers.Add(ctx_d);
                                    ctx.SaveChanges();
                                    developer = ctx.Developers.Where(x => x.SvnUser == svnLogEventArg.Author).FirstOrDefault();
                                }
                                /******************************************************************************************/
                            }

                            //hold current revision for insert and loop through list of changes
                            long currentRevision = 0;
                            foreach (var changedPath in svnLogEventArg.ChangedPaths)
                            {

                                if (changedPath.NodeKind != SvnNodeKind.File)
                                {
                                    continue;
                                }

                                //get filetype
                                var ext = Path.GetExtension(changedPath.Path);
                                var filetype = fileTypes.Where(o => o.Type == ext).FirstOrDefault();

                                if (filetype == null)
                                {
                                    continue;//gifs, jpegs, etc
                                    //filetype = fileTypes.Where( o => o.Type == "*" ).FirstOrDefault();
                                }

                                //get file url
                                var fileUrl = svnRepository.BaseUrl + changedPath.Path;
                                ProjectRepository projectRepository = null;
                                foreach (var pr in projectRepositories)
                                {
                                    if (fileUrl.Replace("//", "/").ToLower().Contains(pr.Path.Replace("//", "/").ToLower()))
                                    {
                                        projectRepository = pr;
                                        break;
                                    }
                                }
                                if (projectRepository == null)
                                    continue;
                                var rgxExclude = new Regex(projectRepository.Project.ExcludeRegExp);
                                if (rgxExclude.IsMatch(fileUrl))
                                    continue;


                                //Get file bytes of code
                                var bytesOfCode = 0;

                                using (var ms = new MemoryStream())
                                {
                                    try
                                    {
                                        var svnUri = new SvnUriTarget(fileUrl, svnLogEventArg.Revision);
                                        svnClient.Write(svnUri, ms);
                                    }
                                    catch (Exception ex)
                                    {
                                        Helper.InsertErrorLog(fileUrl, ex.Message);
                                        continue;
                                    }

                                    var str = Encoding.Default.GetString(ms.ToArray());

                                    bytesOfCode = GetCodeBytes(str, filetype.RemovesRegExp);

                                    //action deleted
                                    if (changedPath.Action == SvnChangeAction.Delete)
                                    {
                                        bytesOfCode *= -1;
                                    }

                                    //action modified | replaced
                                    if (changedPath.Action == SvnChangeAction.Modify || changedPath.Action == SvnChangeAction.Replace)
                                    {

                                        using (var ms2 = new MemoryStream())
                                        {
                                            try
                                            {
                                                var svnUri2 = new SvnUriTarget(fileUrl, svnLogEventArg.Revision - 1);
                                                svnClient.Write(svnUri2, ms2);
                                            }
                                            catch (Exception ex)
                                            {
                                                Helper.InsertErrorLog(fileUrl, ex.Message);
                                                continue;
                                            }

                                            var str2 = Encoding.Default.GetString(ms2.ToArray());
                                            var pbocs = GetCodeBytes(str2, filetype.RemovesRegExp);
                                            bytesOfCode -= pbocs;
                                        }
                                    }

                                    totalBytesOfCode += bytesOfCode;
                                }

                                if (bytesOfCode == 0)
                                {
                                    continue;
                                }

                                //find file in db or create one
                                var file = proManEntities.Files.Where(fl => fl.Filename == fileUrl).FirstOrDefault();

                                if (file == null)
                                {
                                    using (var ctxFile = new ProManEntities())
                                    {
                                        file = new ProManService.File();
                                        file.Filename = fileUrl;
                                        ctxFile.Files.Add(file);
                                        ctxFile.SaveChanges();
                                        file = ctxFile.Files.Where(fl => fl.Filename == fileUrl).FirstOrDefault();
                                    }
                                }

                                //try insert bytes of code
                                Helper.InsertBOC(projectRepository == null ? null : (int?)projectRepository.ProjectID,
                                                    projectRepository == null ? null : (int?)projectRepository.ID,
                                                    developer.ID,
                                                    file.ID,
                                                    filetype == null ? null : (int?)filetype.ID,
                                                    fileUrl,
                                                    bytesOfCode,
                                                    svnLogEventArg.Revision,
                                                    SvnChangeTypeString(changedPath.Action),
                                                    svnLogEventArg.Time);
                            }
                            maxRevision = maxRevision < svnLogEventArg.Revision ? svnLogEventArg.Revision : maxRevision;

                            if (maxRevision > 0)
                            {
                                Helper.UpdateRepositoryRevision(maxRevision, svnRepository.ID);
                            }

                            //try insert project history
                            //if ( currentRevision != svnLogEventArg.Revision ) {
                            //    currentRevision = svnLogEventArg.Revision;
                            //    Helper.InsertProjectHistory( totalBytesOfCode, currentRevision, projectRepository.ID, developer.ID, svnLogEventArg.Time );
                            //    totalBytesOfCode = 0;
                            //}
                        }

                        if (maxRevision > 0)
                        {
                            Helper.UpdateRepositoryRevision(maxRevision, svnRepository.ID);
                        }
                    }
                }

                #endregion

                #region TFS

                //TFS
                try
                {

                    //Get SVN respository credentials
                    List<Repository> tfsRepositories = proManEntities.Repositories.Where(repository => repository.Type == "tfs").ToList<Repository>();
                    foreach (var tfsRepository in tfsRepositories)
                    {
                        /******************************************************************************************/
                        //Convert password to ecrypted password
                        using (var ctx = new ProManEntities())
                        {
                            // Repository oo = proManEntities.Repositories.Where( repository => repository.Name == "SVN" ).FirstOrDefault();
                            //if ( oo != null )
                            var ctx_r = ctx.Repositories.Where(x => x.ID == tfsRepository.ID).FirstOrDefault();
                            if (ctx_r.RootPassword.StartsWith("#!#"))
                            {
                                ctx_r.RootPassword = Libs.Encrypt(ctx_r.RootPassword.Substring(3, ctx_r.RootPassword.Length - 3).ToString());
                                tfsRepository.RootPassword = Libs.Encrypt(ctx_r.RootPassword.Substring(3, ctx_r.RootPassword.Length - 3).ToString());
                            }
                            ctx.SaveChanges();
                        }
                        /*****************************************************************************************/

                        Libs.WriteLine("Initializing: " + tfsRepository.Name);

                        //credentials
                        CredentialsProvider connect = new CredentialsProvider();
                        ICredentials iCred = new NetworkCredential(tfsRepository.RootUsername, Libs.Decrypt(tfsRepository.RootPassword), "*");
                        connect.GetCredentials(new Uri(tfsRepository.BaseUrl), iCred);

                        var configurationServer = TfsConfigurationServerFactory.GetConfigurationServer(new Uri(tfsRepository.BaseUrl), connect);
                        configurationServer.EnsureAuthenticated();

                        var projects = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(new Uri(tfsRepository.BaseUrl));
                        var vcs = projects.GetService<VersionControlServer>();

                        var tmpFile = System.IO.Path.GetTempPath() + Path.DirectorySeparatorChar + "ProMan.tmp";

                        long maxRevision = tfsRepository.LastRevision;
                        var lastRevision = vcs.GetLatestChangesetId();

                        Libs.WriteLine("Scanning: " + tfsRepository.Name);
                        //get projects in this repository
                        //foreach ( var projectRepositories in proManEntities.ProjectRepositories.Where( o => o.Project.RepositoryID == ttfs.ID && o.IsActive ) ) {
                        for (var iRevision = maxRevision; iRevision < lastRevision; iRevision++)
                        {

                            Libs.WriteLine("        r" + iRevision);

                            long totalBoc = 0;
                            var newestDate = DateTime.MinValue;

                            if (maxRevision >= lastRevision)
                            {
                                continue;
                            }

                            try
                            {
                                var changeset = vcs.GetChangeset((int)iRevision, true, true);

                                //check and find dev in this project
                                var developer = developers.Where(o => o.TfsUser == changeset.Committer).FirstOrDefault();

                                if (developer == null)
                                {
                                    /******************************************************************************************/
                                    using (var ctx = new ProManEntities())
                                    {
                                        var ctx_d = new Developer();
                                        ctx_d.TfsUser = changeset.Committer;
                                        ctx.Developers.Add(ctx_d);
                                        ctx.SaveChanges();
                                        developer = ctx.Developers.Where(x => x.TfsUser == changeset.Committer).FirstOrDefault();
                                    }
                                    /******************************************************************************************/
                                }

                                foreach (var w in changeset.Changes)
                                {
                                    //get file url
                                    var fileUrl = tfsRepository.BaseUrl + w.Item.ServerItem;
                                    ProjectRepository projectRepository = null;
                                    foreach (var pr in projectRepositories)
                                    {
                                        if (fileUrl.Replace("//", "/").ToLower().Contains(pr.Path.Replace("//", "/").ToLower()))
                                        {
                                            projectRepository = pr;
                                            break;
                                        }
                                    }
                                    if (projectRepository == null)
                                        continue;
                                    var rgxExclude = new Regex(projectRepository.Project.ExcludeRegExp);
                                    if (rgxExclude.IsMatch(fileUrl))
                                        continue;

                                    if (!w.Item.ServerItem.StartsWith(projectRepository.Path, true, System.Globalization.CultureInfo.InvariantCulture))
                                        continue;

                                    var changeType = TfsChangeType(w.ChangeType);
                                    var changeTypeString = "add";

                                    //foreach ( var o in w.Item.Attributes ) {
                                    //    Debug.Print( o.PropertyName + " " + o.Value.ToString() );
                                    //}

                                    if (w.Item.ItemType != ItemType.File)
                                    {
                                        continue;
                                    }

                                    if (rgxExclude.IsMatch(w.Item.ServerItem))
                                    {
                                        continue;
                                    }

                                    //try insert project history
                                    //if ( currentRevision != iRevision ) {
                                    //    currentRevision = iRevision;
                                    //    Helper.InsertProjectHistory( totalBoc, currentRevision, projectRepositories.ID, developer.ID, w.Item.CheckinDate );
                                    //    totalBoc = 0;
                                    //}

                                    //get filetype
                                    var ext = Path.GetExtension(w.Item.ServerItem);
                                    var fileType = fileTypes.Where(o => o.Type == ext).FirstOrDefault();
                                    if (fileType == null)
                                    {
                                        continue;
                                    }

                                    //Get file BOC
                                    long bocs = 0;
                                    var revision = w.ChangeType == ChangeType.Delete ? iRevision - 1 : iRevision;

                                    w.Item.DownloadFile(tmpFile);
                                    var str = System.IO.File.ReadAllText(tmpFile);
                                    bocs = GetCodeBytes(str, fileType.RemovesRegExp);

                                    if (changeType == ChangeType.Delete)
                                    {
                                        changeTypeString = "del";
                                        bocs *= -1;
                                    }

                                    if (changeType == ChangeType.Edit || changeType == ChangeType.Merge || changeType == ChangeType.Rollback)
                                    {
                                        try
                                        {
                                            Item previousItem = vcs.GetItem(w.Item.ItemId, Convert.ToInt32(iRevision - 1), true);

                                            previousItem.DownloadFile(tmpFile);
                                            var str2 = System.IO.File.ReadAllText(tmpFile);
                                            bocs = bocs - GetCodeBytes(str2, fileType.RemovesRegExp);
                                            changeTypeString = "mod";
                                        }
                                        catch
                                        {
                                            bocs = w.Item.ContentLength;
                                        }
                                    }

                                    totalBoc += bocs;

                                    if (bocs == 0)
                                    {
                                        continue;
                                    }

                                    //find file in db or create one
                                    var file = proManEntities.Files.Where(o => o.Filename == w.Item.ServerItem).FirstOrDefault();

                                    if (file == null)
                                    {
                                        using (var ctxFile = new ProManEntities())
                                        {
                                            file = new ProManService.File();
                                            file.Filename = w.Item.ServerItem;
                                            ctxFile.Files.Add(file);
                                            ctxFile.SaveChanges();
                                            file = ctxFile.Files.Where(o => o.Filename == w.Item.ServerItem).FirstOrDefault();
                                        }
                                    }


                                    Helper.InsertBOC(projectRepository == null ? null : (int?)projectRepository.ProjectID,
                                                        projectRepository == null ? null : (int?)projectRepository.ID,
                                                        developer.ID,
                                                        file.ID,
                                                        fileType == null ? null : (int?)fileType.ID,
                                                        w.Item.ServerItem,
                                                        bocs,
                                                        iRevision,
                                                        changeTypeString,
                                                        w.Item.CheckinDate);

                                    //try insert boc
                                    //Helper.InsertBOC( projectRepositories.ProjectID, projectRepositories.ID, developer.ID, file.ID, fileType.ID, w.Item.ServerItem, bocs, iRevision, changeTypeString, w.Item.CheckinDate );

                                }

                                //maxRevision = maxRevision < iRevision ? iRevision : maxRevision;
                                //if ( maxRevision > 0 ) {
                                //Helper.UpdateProjectRepositoryRevision( maxRevision, projectRepositories.ID );
                                //}

                                Helper.UpdateRepositoryRevision(iRevision, tfsRepository.ID);
                            }
                            catch (Exception ex)
                            {
                                var message = "";
                                while (ex != null)
                                {
                                    message = ex.Message + Environment.NewLine;
                                    ex = ex.InnerException;
                                }
                                Helper.InsertErrorLog("", message);
                            }
                            finally
                            {

                                //Helper.UpdateProjectRepositoryRevision( iRevision, projectRepositories.ID );
                            }
                            //}

                            //if ( maxRevision > 0 ) {
                            //    Helper.UpdateProjectRepositoryRevision( maxRevision, projectRepositories.ID );
                            //}


                        }
                    }
                }
                catch (Exception ex)
                {
                    var message = "";
                    while (ex != null)
                    {
                        message = ex.Message + Environment.NewLine;
                        ex = ex.InnerException;
                    }
                    Helper.InsertErrorLog("", message);
                }


                #endregion

                proManEntities.SaveChanges();
            }

        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //SVN methods
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////


        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //<<<<<<< .mine
        //TFS methods
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public static long GetByteDiffence(Microsoft.TeamFoundation.VersionControl.Client.TeamProject teamProject, VersionControlServer versionControlServer, String serverItem)
        {
            long difference = 0;
            string path = string.Format("$/{0}", teamProject.Name);

            IEnumerable history = versionControlServer.QueryHistory(
                path,                       //path
                LatestVersionSpec.Instance, //version spec
                0,                          //deletion id
                RecursionType.Full,         //recursion type
                null,                       //user
                new DateVersionSpec(DateTime.Now - TimeSpan.FromDays(90000)), //version from
                LatestVersionSpec.Instance,  //version to
                Int32.MaxValue,             //number of results
                true,                      //include changes?
                false);                     // slot mode?

            List<Changeset> lastTwoChangeSet = new List<Changeset>();

            if (history.Cast<Changeset>().ToList().Count > 1)
            {
                List<Changeset> changeSet = history.Cast<Changeset>().ToList();

                int count = 0;
                long value1 = 0;
                long value2 = 0;

                foreach (var changes in changeSet)
                {
                    foreach (var change in changes.Changes.Where(x => x.Item.ServerItem.Contains(serverItem) == true))
                    {
                        if (count == 0)
                        {
                            value1 = change.Item.ContentLength;
                        }
                        else if (count == 1)
                        {
                            value2 = change.Item.ContentLength;
                            break;
                        }

                        count++;
                    }
                }

                difference = value1 - value2;
            }
            else if (history.Cast<Changeset>().ToList().Count == 1)
            {
                difference = (from change in lastTwoChangeSet[0].Changes
                              where change.Item.ServerItem.Contains(serverItem) == true
                              select change.Item.ContentLength).Take(1).ToList()[0];

                var changeType = (from change in lastTwoChangeSet[0].Changes
                                  where change.Item.ServerItem.Contains(serverItem) == true
                                  select change.ChangeType).Take(1).ToList()[0];

                if (ChangeType.Delete == TfsChangeType(changeType))
                {
                    difference = -difference;
                }
            }

            return difference;

        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //=======
        //>>>>>>> .r6403
        // utility methods
        ////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////

        private static string SvnChangeTypeString(SvnChangeAction act)
        {
            switch (act)
            {
                case SvnChangeAction.Add:
                    return "add";
                case SvnChangeAction.Delete:
                    return "del";
                case SvnChangeAction.Modify:
                    return "mod";
                case SvnChangeAction.Replace:
                    return "rep";
            }
            return "non";
        }

        private static ChangeType TfsChangeType(ChangeType act)
        {

            if (act.HasFlag(ChangeType.Undelete) || act.HasFlag(ChangeType.Add))
                return ChangeType.Add;

            if (act.HasFlag(ChangeType.Delete))
                return ChangeType.Delete;

            if (act.HasFlag(ChangeType.Merge) || act.HasFlag(ChangeType.Edit) || act.HasFlag(ChangeType.Rollback))
                return ChangeType.Edit;

            return ChangeType.None;
        }

        //private string TfsChageTypeString( ChangeType act ) {
        //    switch ( act ) {
        //        case ChangeType.Undelete:
        //        case ChangeType.Add:
        //            return "add";
        //        case ChangeType.Delete:
        //            return "del";
        //        case ChangeType.Merge:
        //        case ChangeType.Edit:
        //            return "mod";
        //        //case ChangeType.:
        //        //   return "rep";
        //    }
        //    return "non";
        //}

        private static int GetCodeBytes(string s, string regexp)
        {

            Regex rgx = new Regex("( |\t)+");
            while (rgx.IsMatch(s))
                s = rgx.Replace(s, "");

            rgx = new Regex("([\n|\r|\n\r|\r\n].{1,2}[\n|\r|\n\r|\r\n])");
            while (rgx.IsMatch(s))
                s = rgx.Replace(s, "");

            var i = 0;
            rgx = new Regex("([\n|\r|\n\r|\r\n]{2,})");
            while (++i < 10)
                s = rgx.Replace(s, Environment.NewLine);//

            rgx = new Regex(regexp);
            s = rgx.Replace(s, "");

            rgx = new Regex("([\n|\r|\n\r|\r\n])");
            while (rgx.IsMatch(s))
                s = rgx.Replace(s, "");//Environment.NewLine

            return s.Trim().Length;//.Split( new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries ).Count();
        }
    }
}
