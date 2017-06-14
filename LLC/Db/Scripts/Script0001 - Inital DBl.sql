
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 05/03/2017 18:54:04
-- Generated from EDMX file: D:\IDLA\LLC\LLCCommon\Models\LLCDataModel.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [$DBName$];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_dbo_AspNetUserClaims_dbo_AspNetUsers_UserId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AspNetUserClaims] DROP CONSTRAINT [FK_dbo_AspNetUserClaims_dbo_AspNetUsers_UserId];
GO
IF OBJECT_ID(N'[dbo].[FK_dbo_AspNetUserLogins_dbo_AspNetUsers_UserId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AspNetUserLogins] DROP CONSTRAINT [FK_dbo_AspNetUserLogins_dbo_AspNetUsers_UserId];
GO
IF OBJECT_ID(N'[dbo].[FK_dbo_AspNetUserRoles_dbo_AspNetRoles_RoleId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AspNetUserRoles] DROP CONSTRAINT [FK_dbo_AspNetUserRoles_dbo_AspNetRoles_RoleId];
GO
IF OBJECT_ID(N'[dbo].[FK_dbo_AspNetUserRoles_dbo_AspNetUsers_UserId]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[AspNetUserRoles] DROP CONSTRAINT [FK_dbo_AspNetUserRoles_dbo_AspNetUsers_UserId];
GO
IF OBJECT_ID(N'[dbo].[FK_LinkReportLink]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[LinkReports] DROP CONSTRAINT [FK_LinkReportLink];
GO
IF OBJECT_ID(N'[dbo].[FK_Links_Sources]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Links] DROP CONSTRAINT [FK_Links_Sources];
GO
IF OBJECT_ID(N'[dbo].[FK_LinkStats_Links]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[LinkStats] DROP CONSTRAINT [FK_LinkStats_Links];
GO
IF OBJECT_ID(N'[dbo].[FK_ObjectLinks_Links]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[S3Objects_Links] DROP CONSTRAINT [FK_ObjectLinks_Links];
GO
IF OBJECT_ID(N'[dbo].[FK_ObjectLinks_LorObjects]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[S3Objects_Links] DROP CONSTRAINT [FK_ObjectLinks_LorObjects];
GO
IF OBJECT_ID(N'[dbo].[FK_S3Objects_S3Buckets]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[S3Objects] DROP CONSTRAINT [FK_S3Objects_S3Buckets];
GO
IF OBJECT_ID(N'[dbo].[FK_Sources_S3Buckets]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Sources] DROP CONSTRAINT [FK_Sources_S3Buckets];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[AspNetRoles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AspNetRoles];
GO
IF OBJECT_ID(N'[dbo].[AspNetUserClaims]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AspNetUserClaims];
GO
IF OBJECT_ID(N'[dbo].[AspNetUserLogins]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AspNetUserLogins];
GO
IF OBJECT_ID(N'[dbo].[AspNetUserRoles]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AspNetUserRoles];
GO
IF OBJECT_ID(N'[dbo].[AspNetUsers]', 'U') IS NOT NULL
    DROP TABLE [dbo].[AspNetUsers];
GO
IF OBJECT_ID(N'[dbo].[LinkReports]', 'U') IS NOT NULL
    DROP TABLE [dbo].[LinkReports];
GO
IF OBJECT_ID(N'[dbo].[Links]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Links];
GO
IF OBJECT_ID(N'[dbo].[LinkStats]', 'U') IS NOT NULL
    DROP TABLE [dbo].[LinkStats];
GO
IF OBJECT_ID(N'[dbo].[Logs]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Logs];
GO
IF OBJECT_ID(N'[dbo].[S3Buckets]', 'U') IS NOT NULL
    DROP TABLE [dbo].[S3Buckets];
GO
IF OBJECT_ID(N'[dbo].[S3Objects]', 'U') IS NOT NULL
    DROP TABLE [dbo].[S3Objects];
GO
IF OBJECT_ID(N'[dbo].[S3Objects_Links]', 'U') IS NOT NULL
    DROP TABLE [dbo].[S3Objects_Links];
GO
IF OBJECT_ID(N'[dbo].[Settings]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Settings];
GO
IF OBJECT_ID(N'[dbo].[Sources]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Sources];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Links'
CREATE TABLE [dbo].[Links] (
    [LinkId] int IDENTITY(1,1) NOT NULL,
    [SourceId] int  NULL,
    [LinkUrl] varchar(2048)  NOT NULL,
    [DateFirstFound] datetime  NOT NULL,
    [DateLastFound] datetime  NOT NULL,
    [LinkCheckDisabledDate] datetime  NULL,
    [LinkCheckDisabledUser] varchar(256)  NULL,
    [AttemptCount] int  NOT NULL,
    [IsValid] bit  NULL,
    [DateLastChecked] datetime  NULL,
    [AllTimeMinDownloadTime] int  NULL,
    [AllTimeMaxDownloadTime] int  NULL,
    [AllTimeStdDevDownloadTime] decimal(14,4)  NULL,
    [PastWeekMinDownloadTime] int  NULL,
    [PastWeekMaxDownloadTime] int  NULL,
    [PastWeekStdDevDownloadTime] decimal(14,4)  NULL,
    [DateStatsUpdated] datetime  NULL,
    [ReportNotBeforeDate] datetime  NULL
);
GO

-- Creating table 'LinkStats'
CREATE TABLE [dbo].[LinkStats] (
    [LinkStatId] int IDENTITY(1,1) NOT NULL,
    [LinkId] int  NOT NULL,
    [ContentSize] bigint  NULL,
    [DownloadTime] int  NULL,
    [DateChecked] datetime  NOT NULL,
    [ErrorMessage] varchar(MAX)  NULL,
    [StatusCode] varchar(256)  NULL,
    [StatusDesc] varchar(MAX)  NULL,
    [ContentType] varchar(256)  NULL
);
GO

-- Creating table 'Logs'
CREATE TABLE [dbo].[Logs] (
    [LogId] int IDENTITY(1,1) NOT NULL,
    [DateCreated] datetime  NOT NULL,
    [Source] varchar(50)  NULL,
    [Title] varchar(256)  NOT NULL,
    [Notes] varchar(MAX)  NULL,
    [IsError] bit  NOT NULL,
    [ExceptionDetails] varchar(MAX)  NULL
);
GO

-- Creating table 'S3Buckets'
CREATE TABLE [dbo].[S3Buckets] (
    [S3BucketId] int  NOT NULL,
    [Name] varchar(256)  NOT NULL,
    [AccessKey] varchar(256)  NOT NULL,
    [SecretKey] varchar(256)  NOT NULL,
    [Region] varchar(256)  NULL,
    [SearchPrefix] varchar(256)  NULL,
    [DateCreated] datetime  NOT NULL
);
GO

-- Creating table 'S3Objects'
CREATE TABLE [dbo].[S3Objects] (
    [S3ObjectId] int IDENTITY(1,1) NOT NULL,
    [S3BucketId] int  NOT NULL,
    [Key] varchar(1024)  NOT NULL,
    [ItemName] varchar(256)  NOT NULL,
    [ETag] varchar(256)  NULL,
    [IsFolder] bit  NOT NULL,
    [ContentLastModified] datetime  NULL,
    [DateFirstFound] datetime  NOT NULL,
    [DateLastFound] datetime  NOT NULL,
    [DateLinksLastExtracted] datetime  NULL,
    [LinkCheckDisabledDate] datetime  NULL,
    [LinkCheckDisabledUser] varchar(256)  NULL
);
GO

-- Creating table 'S3Objects_Links'
CREATE TABLE [dbo].[S3Objects_Links] (
    [S3ObjectLinkId] int IDENTITY(1,1) NOT NULL,
    [S3ObjectId] int  NOT NULL,
    [LinkId] int  NOT NULL,
    [DateFirstFound] datetime  NOT NULL,
    [DateLastFound] datetime  NOT NULL,
    [DateRemoved] datetime  NULL
);
GO

-- Creating table 'Settings'
CREATE TABLE [dbo].[Settings] (
    [SettingId] int IDENTITY(1,1) NOT NULL,
    [Name] varchar(256)  NOT NULL,
    [Value] varchar(1024)  NULL,
    [Description] varchar(1024)  NULL,
    [DateCreated] datetime  NOT NULL,
    [DateModified] datetime  NULL,
    [ModifiedUser] varchar(256)  NULL
);
GO

-- Creating table 'Sources'
CREATE TABLE [dbo].[Sources] (
    [SourceId] int  NOT NULL,
    [Name] varchar(256)  NOT NULL,
    [Description] varchar(1024)  NULL,
    [AllowLinkChecking] bit  NOT NULL,
    [AllowLinkExtractions] bit  NOT NULL,
    [S3BucketId] int  NULL,
    [DateCreated] datetime  NOT NULL
);
GO

-- Creating table 'AspNetRoles'
CREATE TABLE [dbo].[AspNetRoles] (
    [Id] nvarchar(128)  NOT NULL,
    [Name] nvarchar(256)  NOT NULL
);
GO

-- Creating table 'AspNetUserClaims'
CREATE TABLE [dbo].[AspNetUserClaims] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [UserId] nvarchar(128)  NOT NULL,
    [ClaimType] nvarchar(max)  NULL,
    [ClaimValue] nvarchar(max)  NULL
);
GO

-- Creating table 'AspNetUserLogins'
CREATE TABLE [dbo].[AspNetUserLogins] (
    [LoginProvider] nvarchar(128)  NOT NULL,
    [ProviderKey] nvarchar(128)  NOT NULL,
    [UserId] nvarchar(128)  NOT NULL
);
GO

-- Creating table 'AspNetUsers'
CREATE TABLE [dbo].[AspNetUsers] (
    [Id] nvarchar(128)  NOT NULL,
    [Email] nvarchar(256)  NULL,
    [EmailConfirmed] bit  NOT NULL,
    [PasswordHash] nvarchar(max)  NULL,
    [SecurityStamp] nvarchar(max)  NULL,
    [PhoneNumber] nvarchar(max)  NULL,
    [PhoneNumberConfirmed] bit  NOT NULL,
    [TwoFactorEnabled] bit  NOT NULL,
    [LockoutEndDateUtc] datetime  NULL,
    [LockoutEnabled] bit  NOT NULL,
    [AccessFailedCount] int  NOT NULL,
    [UserName] nvarchar(256)  NOT NULL
);
GO

-- Creating table 'LinkReports'
CREATE TABLE [dbo].[LinkReports] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [ContentSize] bigint  NULL,
    [Mean] bigint  NOT NULL,
    [StandardDeviation] float  NOT NULL,
    [SdMaximum] int  NOT NULL,
    [Link_LinkId] int  NOT NULL,
    [LinkStatId] int  NULL
);
GO

-- Creating table 'PackageUploads'
CREATE TABLE [dbo].[PackageUploads] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Name] nvarchar(max)  NOT NULL,
    [Description] nvarchar(max)  NOT NULL,
    [UploadedBy] nvarchar(max)  NOT NULL,
    [DateUploaded] datetime  NOT NULL
);
GO

-- Creating table 'AspNetUserRoles'
CREATE TABLE [dbo].[AspNetUserRoles] (
    [AspNetRoles_Id] nvarchar(128)  NOT NULL,
    [AspNetUsers_Id] nvarchar(128)  NOT NULL
);
GO

-- Creating table 'PackageUploadLink'
CREATE TABLE [dbo].[PackageUploadLink] (
    [PackageUploads_Id] int  NOT NULL,
    [Links_LinkId] int  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [LinkId] in table 'Links'
ALTER TABLE [dbo].[Links]
ADD CONSTRAINT [PK_Links]
    PRIMARY KEY CLUSTERED ([LinkId] ASC);
GO

-- Creating primary key on [LinkStatId] in table 'LinkStats'
ALTER TABLE [dbo].[LinkStats]
ADD CONSTRAINT [PK_LinkStats]
    PRIMARY KEY CLUSTERED ([LinkStatId] ASC);
GO

-- Creating primary key on [LogId] in table 'Logs'
ALTER TABLE [dbo].[Logs]
ADD CONSTRAINT [PK_Logs]
    PRIMARY KEY CLUSTERED ([LogId] ASC);
GO

-- Creating primary key on [S3BucketId] in table 'S3Buckets'
ALTER TABLE [dbo].[S3Buckets]
ADD CONSTRAINT [PK_S3Buckets]
    PRIMARY KEY CLUSTERED ([S3BucketId] ASC);
GO

-- Creating primary key on [S3ObjectId] in table 'S3Objects'
ALTER TABLE [dbo].[S3Objects]
ADD CONSTRAINT [PK_S3Objects]
    PRIMARY KEY CLUSTERED ([S3ObjectId] ASC);
GO

-- Creating primary key on [S3ObjectLinkId] in table 'S3Objects_Links'
ALTER TABLE [dbo].[S3Objects_Links]
ADD CONSTRAINT [PK_S3Objects_Links]
    PRIMARY KEY CLUSTERED ([S3ObjectLinkId] ASC);
GO

-- Creating primary key on [SettingId] in table 'Settings'
ALTER TABLE [dbo].[Settings]
ADD CONSTRAINT [PK_Settings]
    PRIMARY KEY CLUSTERED ([SettingId] ASC);
GO

-- Creating primary key on [SourceId] in table 'Sources'
ALTER TABLE [dbo].[Sources]
ADD CONSTRAINT [PK_Sources]
    PRIMARY KEY CLUSTERED ([SourceId] ASC);
GO

-- Creating primary key on [Id] in table 'AspNetRoles'
ALTER TABLE [dbo].[AspNetRoles]
ADD CONSTRAINT [PK_AspNetRoles]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'AspNetUserClaims'
ALTER TABLE [dbo].[AspNetUserClaims]
ADD CONSTRAINT [PK_AspNetUserClaims]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [LoginProvider], [ProviderKey], [UserId] in table 'AspNetUserLogins'
ALTER TABLE [dbo].[AspNetUserLogins]
ADD CONSTRAINT [PK_AspNetUserLogins]
    PRIMARY KEY CLUSTERED ([LoginProvider], [ProviderKey], [UserId] ASC);
GO

-- Creating primary key on [Id] in table 'AspNetUsers'
ALTER TABLE [dbo].[AspNetUsers]
ADD CONSTRAINT [PK_AspNetUsers]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'LinkReports'
ALTER TABLE [dbo].[LinkReports]
ADD CONSTRAINT [PK_LinkReports]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'PackageUploads'
ALTER TABLE [dbo].[PackageUploads]
ADD CONSTRAINT [PK_PackageUploads]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [AspNetRoles_Id], [AspNetUsers_Id] in table 'AspNetUserRoles'
ALTER TABLE [dbo].[AspNetUserRoles]
ADD CONSTRAINT [PK_AspNetUserRoles]
    PRIMARY KEY CLUSTERED ([AspNetRoles_Id], [AspNetUsers_Id] ASC);
GO

-- Creating primary key on [PackageUploads_Id], [Links_LinkId] in table 'PackageUploadLink'
ALTER TABLE [dbo].[PackageUploadLink]
ADD CONSTRAINT [PK_PackageUploadLink]
    PRIMARY KEY CLUSTERED ([PackageUploads_Id], [Links_LinkId] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [SourceId] in table 'Links'
ALTER TABLE [dbo].[Links]
ADD CONSTRAINT [FK_Links_Sources]
    FOREIGN KEY ([SourceId])
    REFERENCES [dbo].[Sources]
        ([SourceId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Links_Sources'
CREATE INDEX [IX_FK_Links_Sources]
ON [dbo].[Links]
    ([SourceId]);
GO

-- Creating foreign key on [LinkId] in table 'LinkStats'
ALTER TABLE [dbo].[LinkStats]
ADD CONSTRAINT [FK_LinkStats_Links]
    FOREIGN KEY ([LinkId])
    REFERENCES [dbo].[Links]
        ([LinkId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_LinkStats_Links'
CREATE INDEX [IX_FK_LinkStats_Links]
ON [dbo].[LinkStats]
    ([LinkId]);
GO

-- Creating foreign key on [LinkId] in table 'S3Objects_Links'
ALTER TABLE [dbo].[S3Objects_Links]
ADD CONSTRAINT [FK_ObjectLinks_Links]
    FOREIGN KEY ([LinkId])
    REFERENCES [dbo].[Links]
        ([LinkId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ObjectLinks_Links'
CREATE INDEX [IX_FK_ObjectLinks_Links]
ON [dbo].[S3Objects_Links]
    ([LinkId]);
GO

-- Creating foreign key on [S3BucketId] in table 'S3Objects'
ALTER TABLE [dbo].[S3Objects]
ADD CONSTRAINT [FK_S3Objects_S3Buckets]
    FOREIGN KEY ([S3BucketId])
    REFERENCES [dbo].[S3Buckets]
        ([S3BucketId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_S3Objects_S3Buckets'
CREATE INDEX [IX_FK_S3Objects_S3Buckets]
ON [dbo].[S3Objects]
    ([S3BucketId]);
GO

-- Creating foreign key on [S3BucketId] in table 'Sources'
ALTER TABLE [dbo].[Sources]
ADD CONSTRAINT [FK_Sources_S3Buckets]
    FOREIGN KEY ([S3BucketId])
    REFERENCES [dbo].[S3Buckets]
        ([S3BucketId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_Sources_S3Buckets'
CREATE INDEX [IX_FK_Sources_S3Buckets]
ON [dbo].[Sources]
    ([S3BucketId]);
GO

-- Creating foreign key on [S3ObjectId] in table 'S3Objects_Links'
ALTER TABLE [dbo].[S3Objects_Links]
ADD CONSTRAINT [FK_ObjectLinks_LorObjects]
    FOREIGN KEY ([S3ObjectId])
    REFERENCES [dbo].[S3Objects]
        ([S3ObjectId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ObjectLinks_LorObjects'
CREATE INDEX [IX_FK_ObjectLinks_LorObjects]
ON [dbo].[S3Objects_Links]
    ([S3ObjectId]);
GO

-- Creating foreign key on [UserId] in table 'AspNetUserClaims'
ALTER TABLE [dbo].[AspNetUserClaims]
ADD CONSTRAINT [FK_dbo_AspNetUserClaims_dbo_AspNetUsers_UserId]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[AspNetUsers]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_dbo_AspNetUserClaims_dbo_AspNetUsers_UserId'
CREATE INDEX [IX_FK_dbo_AspNetUserClaims_dbo_AspNetUsers_UserId]
ON [dbo].[AspNetUserClaims]
    ([UserId]);
GO

-- Creating foreign key on [UserId] in table 'AspNetUserLogins'
ALTER TABLE [dbo].[AspNetUserLogins]
ADD CONSTRAINT [FK_dbo_AspNetUserLogins_dbo_AspNetUsers_UserId]
    FOREIGN KEY ([UserId])
    REFERENCES [dbo].[AspNetUsers]
        ([Id])
    ON DELETE CASCADE ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_dbo_AspNetUserLogins_dbo_AspNetUsers_UserId'
CREATE INDEX [IX_FK_dbo_AspNetUserLogins_dbo_AspNetUsers_UserId]
ON [dbo].[AspNetUserLogins]
    ([UserId]);
GO

-- Creating foreign key on [AspNetRoles_Id] in table 'AspNetUserRoles'
ALTER TABLE [dbo].[AspNetUserRoles]
ADD CONSTRAINT [FK_AspNetUserRoles_AspNetRole]
    FOREIGN KEY ([AspNetRoles_Id])
    REFERENCES [dbo].[AspNetRoles]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [AspNetUsers_Id] in table 'AspNetUserRoles'
ALTER TABLE [dbo].[AspNetUserRoles]
ADD CONSTRAINT [FK_AspNetUserRoles_AspNetUser]
    FOREIGN KEY ([AspNetUsers_Id])
    REFERENCES [dbo].[AspNetUsers]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AspNetUserRoles_AspNetUser'
CREATE INDEX [IX_FK_AspNetUserRoles_AspNetUser]
ON [dbo].[AspNetUserRoles]
    ([AspNetUsers_Id]);
GO

-- Creating foreign key on [Link_LinkId] in table 'LinkReports'
ALTER TABLE [dbo].[LinkReports]
ADD CONSTRAINT [FK_LinkReportLink]
    FOREIGN KEY ([Link_LinkId])
    REFERENCES [dbo].[Links]
        ([LinkId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_LinkReportLink'
CREATE INDEX [IX_FK_LinkReportLink]
ON [dbo].[LinkReports]
    ([Link_LinkId]);
GO

-- Creating foreign key on [PackageUploads_Id] in table 'PackageUploadLink'
ALTER TABLE [dbo].[PackageUploadLink]
ADD CONSTRAINT [FK_PackageUploadLink_PackageUpload]
    FOREIGN KEY ([PackageUploads_Id])
    REFERENCES [dbo].[PackageUploads]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating foreign key on [Links_LinkId] in table 'PackageUploadLink'
ALTER TABLE [dbo].[PackageUploadLink]
ADD CONSTRAINT [FK_PackageUploadLink_Link]
    FOREIGN KEY ([Links_LinkId])
    REFERENCES [dbo].[Links]
        ([LinkId])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_PackageUploadLink_Link'
CREATE INDEX [IX_FK_PackageUploadLink_Link]
ON [dbo].[PackageUploadLink]
    ([Links_LinkId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------