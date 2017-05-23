using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using ProManService.Engines.TfsLibs;
using ProManService.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProManService.Engines
{
    public class TFS : EngineBase
    {
        IData _data;
        public TFS(IData data)
        {
            _data = data;
        }

        public void Update()
        {

            //Get SVN respository credentials
            List<Repository> tfsRepositories = _data.GetRepositories("tfs");
            foreach (var tfsRepository in tfsRepositories)
            {

                //invokeEvent: Libs.WriteLine("Initializing: " + tfsRepository.Name);

                //credentials
                CredentialsProvider connect = new CredentialsProvider();
                ICredentials iCred = new NetworkCredential(tfsRepository.RootUsername, tfsRepository.RootPassword, "*");
                connect.GetCredentials(new Uri(tfsRepository.BaseUrl), iCred);

                //var tfsCreds = new TfsClientCredentials(new WindowsCredential(), true);
                //var configurationServer = TfsConfigurationServerFactory.GetConfigurationServer(new Uri(tfsRepository.BaseUrl), connect);
                //configurationServer.EnsureAuthenticated();
                //var projects = TfsTeamProjectCollectionFactory.GetTeamProjectCollection(new Uri(tfsRepository.BaseUrl), tfsCreds);

                var projects = new TfsTeamProjectCollection(new Uri(tfsRepository.BaseUrl), iCred);
                var vcs = projects.GetService<VersionControlServer>();

                var tmpFile = System.IO.Path.GetTempPath() + Path.DirectorySeparatorChar + "ProMan.tmp";

                long maxRevision = tfsRepository.LastRevision;
                var lastRevision = vcs.GetLatestChangesetId();

                //invokeEvent: Libs.WriteLine("Scanning: " + tfsRepository.Name);
                //get projects in this repository

                for (var iRevision = maxRevision; iRevision < lastRevision; iRevision++)
                {

                    //invokeEvent: Libs.WriteLine("        r" + iRevision);

                    long totalBoc = 0;
                    var newestDate = DateTime.MinValue;

                    if (maxRevision >= lastRevision)
                        continue;

                    var changeset = vcs.GetChangeset((int)iRevision, true, true);

                    //check and find dev in this project
                    var developer = _data.GetDevelopers().Where(o => o.TfsUser == changeset.Committer).FirstOrDefault();
                    if (developer == null)
                        developer = _data.InsertDeveloper(changeset.Committer);

                    foreach (var w in changeset.Changes)
                    {
                        //get file url
                        var fileUrl = tfsRepository.BaseUrl + w.Item.ServerItem;
                        ProjectRepository projectRepository = null;
                        foreach (var pr in _data.GetProjectRepositories())
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

                        if (w.Item.ItemType != ItemType.File)
                            continue;

                        if (rgxExclude.IsMatch(w.Item.ServerItem))
                            continue;

                        //get filetype
                        var ext = Path.GetExtension(w.Item.ServerItem);
                        var fileType = _data.GetFileTypes().Where(o => o.Type == ext).FirstOrDefault();
                        if (fileType == null)
                            continue;

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
                            continue;

                        //find file in db or create one
                        var file = _data.GetFile(w.Item.ServerItem);

                        if (file == null)
                            file = _data.InsertFile(w.Item.ServerItem);


                        _data.InsertBOC(projectRepository == null ? null : (int?)projectRepository.ProjectID,
                                            projectRepository == null ? null : (int?)projectRepository.ID,
                                            developer.ID,
                                            file.ID,
                                            fileType == null ? null : (int?)fileType.ID,
                                            w.Item.ServerItem,
                                            bocs,
                                            iRevision,
                                            changeTypeString,
                                            w.Item.CheckinDate);

                    }

                    _data.UpdateRepositoryRevision(iRevision, tfsRepository.ID);
                }
            }

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
    }
}
