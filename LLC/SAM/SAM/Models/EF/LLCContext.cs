using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DbCore.Models
{
    public partial class LLCContext : DbContext
    {
        public virtual DbSet<Buckets> Buckets { get; set; }
        public virtual DbSet<Links> Links { get; set; }
        public virtual DbSet<ObjectLinks> ObjectLinks { get; set; }
        public virtual DbSet<Objects> Objects { get; set; }
        public virtual DbSet<Packages> Packages { get; set; }
        public virtual DbSet<Reports> Reports { get; set; }
        public virtual DbSet<Settings> Settings { get; set; }
        public virtual DbSet<Sources> Sources { get; set; }
        public virtual DbSet<Stats> Stats { get; set; }

        public LLCContext()
        {
            Database.SetCommandTimeout(5 * 60);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer(@"Server=drgk6yf55vn8y4.cbl0vtr35mzo.us-west-2.rds.amazonaws.com;Database=LLC;User Id=llcadmin;Password=77zWm!~-twaWuQUS;MultipleActiveResultSets=true");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Buckets>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasMaxLength(256)
                    .ValueGeneratedNever();

                entity.Property(e => e.AccessKey)
                    .HasMaxLength(256);

                entity.Property(e => e.DateCreated).HasColumnType("smalldatetime");

                entity.Property(e => e.Name)
                    .HasMaxLength(256);

                entity.Property(e => e.Region)
                    .HasMaxLength(256);

                entity.Property(e => e.SearchPrefix)
                    .HasMaxLength(256);

                entity.Property(e => e.SecretKey)
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<Links>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasMaxLength(256)
                    .ValueGeneratedNever();

                entity.Property(e => e.AllTimeStdDevDownloadTime).HasColumnType("decimal(14, 4)");

                entity.Property(e => e.DateFirstFound).HasColumnType("smalldatetime");

                entity.Property(e => e.DateLastChecked).HasColumnType("smalldatetime");

                entity.Property(e => e.DateLastFound).HasColumnType("smalldatetime");

                entity.Property(e => e.DateUpdated).HasColumnType("smalldatetime");

                entity.Property(e => e.DisabledDate).HasColumnType("smalldatetime");

                entity.Property(e => e.DisabledUser)
                    .HasMaxLength(256);

                entity.Property(e => e.PastWeekStdDevDownloadTime).HasColumnType("decimal(14, 4)");

                entity.Property(e => e.ReportNotBeforeDate).HasColumnType("date");

                entity.Property(e => e.Source)
                    .HasMaxLength(256);

                entity.Property(e => e.Url)
                    .HasMaxLength(1024);
            });

            modelBuilder.Entity<ObjectLinks>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasMaxLength(256)
                    .ValueGeneratedNever();

                entity.Property(e => e.DateFirstFound).HasColumnType("smalldatetime");

                entity.Property(e => e.DateLastFound).HasColumnType("smalldatetime");

                entity.Property(e => e.DateRemoved).HasColumnType("smalldatetime");

                entity.Property(e => e.Link)
                    .HasMaxLength(256);

                entity.Property(e => e.Object)
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<Objects>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasMaxLength(256)
                    .ValueGeneratedNever();

                entity.Property(e => e.Bucket)
                    .HasMaxLength(256);

                entity.Property(e => e.ContentLastModified).HasColumnType("datetime");

                entity.Property(e => e.DateFirstFound).HasColumnType("smalldatetime");

                entity.Property(e => e.DateLastFound).HasColumnType("smalldatetime");

                entity.Property(e => e.DateLinksLastExtracted).HasColumnType("smalldatetime");

                entity.Property(e => e.DisabledDate).HasColumnType("smalldatetime");

                entity.Property(e => e.DisabledUser)
                    .HasMaxLength(256);

                entity.Property(e => e.Etag)
                    .HasColumnName("ETag")
                    .HasMaxLength(256);

                entity.Property(e => e.ItemName)
                    .HasMaxLength(256);

                entity.Property(e => e.Key)
                    .HasMaxLength(1024);
            });

            modelBuilder.Entity<Packages>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasMaxLength(256)
                    .ValueGeneratedNever();

                entity.Property(e => e.DateUploaded).HasColumnType("datetime");

                entity.Property(e => e.FileName);

                entity.Property(e => e.ImsDescription);

                entity.Property(e => e.ImsSchema);

                entity.Property(e => e.ImsSchemaVersion);

                entity.Property(e => e.ImsTitle);

                entity.Property(e => e.Key);
            });

            modelBuilder.Entity<Reports>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasMaxLength(256)
                    .ValueGeneratedNever();

                entity.Property(e => e.Link)
                    .HasMaxLength(256);

                entity.Property(e => e.Stat)
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<Settings>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasMaxLength(256)
                    .ValueGeneratedNever();

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateModified).HasColumnType("datetime");

                entity.Property(e => e.Description)
                    .HasMaxLength(1024);

                entity.Property(e => e.ModifiedUser)
                    .HasMaxLength(256);

                entity.Property(e => e.Name)
                    .HasMaxLength(256);

                entity.Property(e => e.Value)
                    .HasMaxLength(1024);
            });

            modelBuilder.Entity<Sources>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasMaxLength(256)
                    .ValueGeneratedNever();

                entity.Property(e => e.DateCreated).HasColumnType("smalldatetime");

                entity.Property(e => e.Description)
                    .HasMaxLength(1024);

                entity.Property(e => e.Name)
                    .HasMaxLength(256);

                entity.Property(e => e.S3bucketId)
                    .HasColumnName("S3BucketId")
                    .HasMaxLength(256);
            });

            modelBuilder.Entity<Stats>(entity =>
            {
                entity.Property(e => e.Id)
                    .HasMaxLength(256)
                    .ValueGeneratedNever();

                entity.Property(e => e.ContentType)
                    .HasMaxLength(256);

                entity.Property(e => e.DateChecked).HasColumnType("datetime");

                entity.Property(e => e.Error)
                    .HasMaxLength(512);

                entity.Property(e => e.Link)
                    .HasMaxLength(256);

                entity.Property(e => e.StatusCode)
                    .HasMaxLength(256);

                entity.Property(e => e.StatusDescription)
                    .HasMaxLength(2048);
            });
        }
    }
}
