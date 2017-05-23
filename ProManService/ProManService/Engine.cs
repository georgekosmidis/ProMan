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
            //WindowsIdentity currentIdentity = WindowsIdentity.GetCurrent();
            //var username = currentIdentity.Name;
            
        }
        
    }
}
