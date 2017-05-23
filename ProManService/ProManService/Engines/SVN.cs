using ProManService.Interfaces;
using SharpSvn;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ProManService.Engines
{
    public class SVN : EngineBase
    {
        IData _data;
        public SVN(IData data)
        {
            _data = data;
        }

        public void Update()
        {
            using (SvnClient svnClient = new SvnClient())
            {
                //Get SVN respository credentials
                List<Repository> svnRepositories = _data.GetRepositories("svn");
                foreach (var svnRepository in svnRepositories)
                {
                    //Authenticate SVN
                    svnClient.Authentication.DefaultCredentials = new NetworkCredential(svnRepository.RootUsername, svnRepository.RootPassword);

                    //invokeEvent: Libs.WriteLine("Initiating: " + svnRepository.Name);

                    //iterate projects repositories
                    Collection<SvnLogEventArgs> svnLogEventArgsCollection;
                    SvnLogArgs args = new SvnLogArgs(new SvnRevisionRange(svnRepository.LastRevision, Int64.MaxValue));
                    svnClient.GetLog(new Uri(svnRepository.BaseUrl), args, out svnLogEventArgsCollection);

                    //invokeEvent: Libs.WriteLine("Scanning: " + svnRepository.Name);

                    //Get svn event args of each repository with larger revision than last revision
                    List<SvnLogEventArgs> svnLogEventArgsList = svnLogEventArgsCollection.Where(eventArgs => eventArgs.Revision > svnRepository.LastRevision).OrderBy(eventArgs => eventArgs.Revision).ToList();

                    long maxRevision = 0;
                    long totalBytesOfCode = 0;

                    foreach (SvnLogEventArgs svnLogEventArg in svnLogEventArgsList)
                    {
                        //invokeEvent: Libs.WriteLine("        r" + svnLogEventArg.Revision);

                        //check for developer
                        var developer = _data.GetDevelopers().Where(dev => dev.SvnUser == svnLogEventArg.Author || dev.SvnUser2 == svnLogEventArg.Author).FirstOrDefault();

                        if (developer == null)
                            developer = _data.InsertDeveloper(svnLogEventArg.Author);

                        //hold current revision for insert and loop through list of changes
                        foreach (var changedPath in svnLogEventArg.ChangedPaths)
                        {
                            if (changedPath.NodeKind != SvnNodeKind.File)
                                continue;

                            //get filetype
                            var ext = Path.GetExtension(changedPath.Path);
                            var filetype = _data.GetFileTypes().Where(o => o.Type == ext).FirstOrDefault();

                            if (filetype == null)
                                continue;//gifs, jpegs, etc

                            //get file url
                            var fileUrl = svnRepository.BaseUrl + changedPath.Path;
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


                            //Get file bytes of code
                            var bytesOfCode = 0;

                            using (var ms = new MemoryStream())
                            {
                                var svnUri = new SvnUriTarget(fileUrl, svnLogEventArg.Revision);
                                svnClient.Write(svnUri, ms);

                                var str = Encoding.Default.GetString(ms.ToArray());
                                bytesOfCode = GetCodeBytes(str, filetype.RemovesRegExp);

                                //action deleted
                                if (changedPath.Action == SvnChangeAction.Delete)
                                    bytesOfCode *= -1;

                                //action modified | replaced
                                if (changedPath.Action == SvnChangeAction.Modify || changedPath.Action == SvnChangeAction.Replace)
                                {

                                    using (var ms2 = new MemoryStream())
                                    {
                                        var svnUri2 = new SvnUriTarget(fileUrl, svnLogEventArg.Revision - 1);
                                        svnClient.Write(svnUri2, ms2);

                                        var str2 = Encoding.Default.GetString(ms2.ToArray());
                                        var pbocs = GetCodeBytes(str2, filetype.RemovesRegExp);
                                        bytesOfCode -= pbocs;
                                    }
                                }

                                totalBytesOfCode += bytesOfCode;
                            }

                            if (bytesOfCode == 0)
                                continue;

                            //find file in db or create one
                            var file = _data.GetFile(fileUrl);
                            if (file == null)
                                file = _data.InsertFile(fileUrl);

                            //try insert bytes of code
                            _data.InsertBOC(projectRepository == null ? null : (int?)projectRepository.ProjectID,
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
                            _data.UpdateRepositoryRevision(maxRevision, svnRepository.ID);
                    }

                    if (maxRevision > 0)
                        _data.UpdateRepositoryRevision(maxRevision, svnRepository.ID);
                }
            }


        }

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


    }
}
