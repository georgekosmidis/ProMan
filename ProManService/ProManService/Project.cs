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
    
    public partial class Project
    {
        public Project()
        {
            this.BOCs = new HashSet<BOC>();
            this.ProjectRepositories = new HashSet<ProjectRepository>();
            this.Developers = new HashSet<Developer>();
            this.FileTypes = new HashSet<FileType>();
        }
    
        public int ID { get; set; }
        public int RepositoryID_OFF { get; set; }
        public string Name { get; set; }
        public string Descr { get; set; }
        public bool IsActive { get; set; }
        public string ExcludeRegExp { get; set; }
    
        public virtual ICollection<BOC> BOCs { get; set; }
        public virtual ICollection<ProjectRepository> ProjectRepositories { get; set; }
        public virtual ICollection<Developer> Developers { get; set; }
        public virtual ICollection<FileType> FileTypes { get; set; }
    }
}