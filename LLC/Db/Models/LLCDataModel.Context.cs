﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Db.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class LORLinkCheckerEntities : DbContext
    {
        public LORLinkCheckerEntities()
            : base("name=LORLinkCheckerEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<C__MigrationHistory> C__MigrationHistory { get; set; }
        public virtual DbSet<AspNetRole> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUser> AspNetUsers { get; set; }
        public virtual DbSet<LinkReport> LinkReports { get; set; }
        public virtual DbSet<Link> Links { get; set; }
        public virtual DbSet<LinkStat> LinkStats { get; set; }
        public virtual DbSet<Log> Logs { get; set; }
        public virtual DbSet<PackageUploadFile> PackageUploadFiles { get; set; }
        public virtual DbSet<PackageUpload> PackageUploads { get; set; }
        public virtual DbSet<S3Buckets> S3Buckets { get; set; }
        public virtual DbSet<S3Objects> S3Objects { get; set; }
        public virtual DbSet<S3Objects_Links> S3Objects_Links { get; set; }
        public virtual DbSet<SchemaVersion> SchemaVersions { get; set; }
        public virtual DbSet<Setting> Settings { get; set; }
        public virtual DbSet<Source> Sources { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
    }
}
