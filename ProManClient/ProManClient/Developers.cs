//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ProManClient
{
    using System;
    using System.Collections.Generic;
    
    public partial class Developers
    {
        public Developers()
        {
            this.BOC = new HashSet<BOC>();
            this.ProjectHistory_OFF = new HashSet<ProjectHistory_OFF>();
            this.Projects = new HashSet<Projects>();
        }
    
        public int ID { get; set; }
        public string Username { get; set; }
        public string Name { get; set; }
        public string SvnUser { get; set; }
        public string SvnUser2 { get; set; }
        public string TfsUser { get; set; }
    
        public virtual ICollection<BOC> BOC { get; set; }
        public virtual ICollection<ProjectHistory_OFF> ProjectHistory_OFF { get; set; }
        public virtual ICollection<Projects> Projects { get; set; }
    }
}
