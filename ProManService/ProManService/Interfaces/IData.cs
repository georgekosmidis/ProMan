using System;
using System.Collections.Generic;

namespace ProManService.Interfaces
{
    public interface IData
    {
        List<Developer> GetDevelopers();
        List<FileType> GetFileTypes();
        List<ProjectRepository> GetProjectRepositories();
        File GetFile(string fileUrl);
        List<Repository> GetRepositories(string repositoryType);
        Developer InsertDeveloper(string devName);
        File InsertFile(string fileUrl);

        void InsertBOC(int? projectID, int? projectRepositoryID, int developerID, int fileID, int? filetypeID, string fileURL, long bocs, long revision, string action, DateTime lTime);
        void UpdateRepositoryRevision(long revision, int repositoryID);
    }
}