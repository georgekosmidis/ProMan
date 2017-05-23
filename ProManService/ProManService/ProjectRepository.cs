//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ProManService
{
    using System;
    using System.Collections.Generic;
    
    public partial class ProjectRepository
    {
        public ProjectRepository()
        {
            this.BOCs = new HashSet<BOC>();
            this.ProjectHistory_OFF = new HashSet<ProjectHistory_OFF>();
        }
    
        public int ID { get; set; }
        public int ProjectID { get; set; }
        public Nullable<int> RepositoryID { get; set; }
        public string Name { get; set; }
        public string BasePath_OFF { get; set; }
        public string Path { get; set; }
        public long LastRevision_OFF { get; set; }
        public byte CocomoMode { get; set; }
        public bool IsActive { get; set; }
        public string ExcludeRegExp_OFF { get; set; }
    
        public virtual ICollection<BOC> BOCs { get; set; }
        public virtual ICollection<ProjectHistory_OFF> ProjectHistory_OFF { get; set; }
        public virtual Project Project { get; set; }
        public virtual Repository Repository { get; set; }
    }
}
