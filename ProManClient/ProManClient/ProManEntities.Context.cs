﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class ProManEntities : DbContext
    {
        public ProManEntities()
            : base("name=ProManEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<BOC> BOC { get; set; }
        public virtual DbSet<Developers> Developers { get; set; }
        public virtual DbSet<ErrorLog> ErrorLog { get; set; }
        public virtual DbSet<Files> Files { get; set; }
        public virtual DbSet<FileTypes> FileTypes { get; set; }
        public virtual DbSet<ProjectHistory_OFF> ProjectHistory_OFF { get; set; }
        public virtual DbSet<ProjectRepositories> ProjectRepositories { get; set; }
        public virtual DbSet<Projects> Projects { get; set; }
        public virtual DbSet<Repositories> Repositories { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
        public virtual DbSet<Users> Users { get; set; }
    }
}
