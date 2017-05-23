using ProManService.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProManService.Data
{
    public class EfData : IData
    {
        public EfData()
        {

        }

        public List<Developer> GetDevelopers()
        {
            using (var proManEntities = new ProManEntities())
            {
                return proManEntities.Developers.ToList();
            }
        }

        public List<FileType> GetFileTypes()
        {
            using (var proManEntities = new ProManEntities())
            {
                return proManEntities.FileTypes.ToList();
            }
        }

        public List<ProjectRepository> GetProjectRepositories()
        {
            using (var proManEntities = new ProManEntities())
            {
                return proManEntities.ProjectRepositories.ToList();
            }
        }

        public File GetFile(string fileUrl)
        {
            using (var proManEntities = new ProManEntities())
            {
                return proManEntities.Files.Where(fl => fl.Filename == fileUrl).FirstOrDefault();
            }
        }

        public List<Repository> GetRepositories(string repositoryType)
        {
            using (var proManEntities = new ProManEntities())
            {
                return proManEntities.Repositories.Where(repository => repository.Type == repositoryType).ToList<Repository>();
            }
        }

        public Developer InsertDeveloper(string devName)
        {
            using (var ctx = new ProManEntities())
            {
                var ctx_d = new Developer();
                ctx_d.SvnUser = devName;
                ctx.Developers.Add(ctx_d);
                ctx.SaveChanges();
                return ctx.Developers.Where(x => x.SvnUser == devName).FirstOrDefault();
            }
        }

        public File InsertFile(string fileUrl)
        {
            using (var ctxFile = new ProManEntities())
            {
                var file = new ProManService.File();
                file.Filename = fileUrl;
                ctxFile.Files.Add(file);
                ctxFile.SaveChanges();
                return ctxFile.Files.Where(fl => fl.Filename == fileUrl).FirstOrDefault();
            }
        }

        public void UpdateRepositoryRevision(long revision, int repositoryID)
        {
            using (var ctx = new ProManEntities())
            {
                var repo = ctx.Repositories.Where(o => o.ID == repositoryID).First();
                repo.LastRevision = revision;
                ctx.SaveChanges();
            }
        }

        public void InsertBOC(int? projectID, int? projectRepositoryID, int developerID, int fileID, int? filetypeID, string fileURL, long bocs, long revision, string action, DateTime lTime)
        {
            using (var ctx = new ProManEntities())
            {
                var oldboc = ctx.BOCs.Where(o =>

                                       o.DeveloperID == developerID &&
                                       o.FileID == fileID &&
                                       o.RevisionNumber == revision
                    ).FirstOrDefault();

                if (oldboc != null)
                {
                    ctx.BOCs.Remove(oldboc);
                    ctx.SaveChanges();
                }

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


                ctx.BOCs.Add(boc);

                ctx.SaveChanges();

            }
        }
    }
}
