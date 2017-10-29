IF OBJECT_ID('dbo.LinkReports-Dynamo', 'U') IS NOT NULL 
	DROP TABLE [LinkReports-Dynamo]

IF OBJECT_ID('dbo.Sources-Dynamo', 'U') IS NOT NULL 	
	DROP TABLE [Sources-Dynamo]
	
IF OBJECT_ID('dbo.Settings-Dynamo', 'U') IS NOT NULL 
	DROP TABLE [Settings-Dynamo]
	
IF OBJECT_ID('dbo.S3Objects_Links-Dynamo', 'U') IS NOT NULL 
	DROP TABLE [S3Objects_Links-Dynamo]
	
IF OBJECT_ID('dbo.S3Objects-Dynamo', 'U') IS NOT NULL 	
	DROP TABLE [S3Objects-Dynamo]
	
IF OBJECT_ID('dbo.S3Buckets-Dynamo', 'U') IS NOT NULL 	
	DROP TABLE [S3Buckets-Dynamo]
	
IF OBJECT_ID('dbo.PackageUploads-Dynamo', 'U') IS NOT NULL 	
	DROP TABLE [PackageUploads-Dynamo]
	
IF OBJECT_ID('dbo.PackageUploadLink-Dynamo', 'U') IS NOT NULL 	
	DROP TABLE [PackageUploadLink-Dynamo]
	
IF OBJECT_ID('dbo.PackageUploadFiles-Dynamo', 'U') IS NOT NULL 	
	DROP TABLE [PackageUploadFiles-Dynamo]
	
IF OBJECT_ID('dbo.LinkStats-Dynamo', 'U') IS NOT NULL 	
	DROP TABLE [LinkStats-Dynamo]
	
IF OBJECT_ID('dbo.Links-Dynamo', 'U') IS NOT NULL 	
	DROP TABLE [Links-Dynamo]

CREATE TABLE [LinkReports-Dynamo] (
    Id varchar(256),
    ContentSize bigint NULL,
    Mean bigint,
    StandardDeviation float,
    SdMaximum int,
	Link_LinkId varchar(256),
	LinkStatId varchar(256) NULL,
	PRIMARY KEY (Id)
);

CREATE TABLE [Sources-Dynamo] (
    SourceId varchar(256),
    [Name] varchar(256),
    [Description] varchar(1024) NULL,
    AllowLinkChecking bit,
    AllowLinkExtractions bit,
	S3BucketId varchar(256) NULL,
	DateCreated smalldatetime,
	PRIMARY KEY (SourceId)
);

CREATE TABLE [Settings-Dynamo] (
	SettingId varchar(256),
	[Name] varchar(256),
	[Value] varchar(1024) NULL,
	[Description] varchar(1024) NULL,
	DateCreated datetime,
	DateModified datetime NULL,
	ModifiedUser varchar(256),
	PRIMARY KEY (SettingId)
);

CREATE TABLE [S3Objects_Links-Dynamo] (
	S3ObjectLinkId varchar(256),
	S3ObjectId varchar(256),
	LinkId varchar(256),
	DateFirstFound smalldatetime,
	DateLastFound smalldatetime,
	DateRemoved smalldatetime NULL,
	PRIMARY KEY (S3ObjectLinkId)
);

CREATE TABLE [S3Objects-Dynamo] (
	S3ObjectId varchar(256),
	S3BucketId varchar(256),
	[Key] varchar(1024),
	ItemName varchar(256),
	ETag varchar(256) NULL,
	IsFolder bit,
	ContentLastModified datetime NULL,
	DateFirstFound smalldatetime,
	DateLastFound smalldatetime,
	DateLinksLastExtracted smalldatetime NULL,
	LinkCheckDisabledDate smalldatetime NULL,
	LinkCheckDisabledUser varchar(256) NULL,
	PRIMARY KEY (S3ObjectId)
);

CREATE TABLE [S3Buckets-Dynamo] (
	S3BucketId varchar(256),
	[Name] varchar(256),
	AccessKey varchar(256),
	SecretKey varchar(256),
	Region varchar(256) NULL,
	SearchPrefix varchar(256) NULL,
	DateCreated smalldatetime,
	PRIMARY KEY (S3BucketId)
);

CREATE TABLE [PackageUploads-Dynamo] (
	Id varchar(256),
	[Name] nvarchar(MAX),
	[Description] nvarchar(MAX),
	UploadedBy nvarchar(MAX),
	DateUploaded datetime,
	[Key] varchar(MAX) NULL,
	PackageProcessed bit NULL,
	[FileName] varchar(MAX) NULL,
	ImsSchema varchar(MAX) NULL,
	ImsSchemaVersion varchar(MAX) NULL,
	ImsTitle varchar(MAX) NULL,
	ImsDescription varchar(MAX) NULL,
	PRIMARY KEY (Id)
);

CREATE TABLE [PackageUploadLink-Dynamo] (
	PackageUploads_Id varchar(256),
	Links_LinkId varchar(256),
	PRIMARY KEY (PackageUploads_Id)
);

CREATE TABLE [PackageUploadFiles-Dynamo] (
	Id varchar(256),
	CourseLocation nvarchar(MAX),
	Link_LinkId varchar(256),
	Protocol varchar(MAX) NULL,
	LinkName varchar(MAX) NULL,
	ParentFolder varchar(MAX) NULL,
	PackageUploadId varchar(256) NULL,
	PRIMARY KEY (Id)
);

CREATE TABLE [LinkStats-Dynamo] (
	LinkStatId varchar(256),
	LinkId varchar(256),
	ContentSize bigint NULL,
	DownloadTime int NULL,
	DateChecked datetime,
	ErrorMessage varchar(512) NULL,
	StatusCode varchar(256) NULL,
	StatusSesc varchar(2048) NULL,
	ContentType varchar(256) NULL,
	PRIMARY KEY (LinkStatId)
);

CREATE TABLE [Links-Dynamo] (
	LinkId varchar(256),
	SourceId varchar(256) NULL,
	LinkUrl varchar(2048),
	DateFirstFound smalldatetime,
	DateLastFound smalldatetime,
	LinkCheckDisabledDate smalldatetime NULL,
	LinkCheckDisabledUser varchar(256) NULL,
	AttemptCount int,
	IsValid bit NULL,
	DateLastChecked smalldatetime NULL,
	AllTimeMinDownloadTime int NULL,
	AllTimeMaxDownloadTime int NULL,
	AllTimeStdDevDownloadTime decimal(14, 4) NULL,
	PastWeekMinDownloadTime int NULL,
	PastWeekMaxDownloadTime int NULL,
	PastWeekStdDevDownloadTime decimal(14, 4) NULL,
	DateStatsUpdated smalldatetime NULL,
	ReportNotBeforeDate date NULL,
	PRIMARY KEY (LinkId)
);

INSERT INTO   [LinkReports-Dynamo]
SELECT * FROM LinkReports

INSERT INTO   [Sources-Dynamo]
SELECT * FROM Sources

INSERT INTO   [Settings-Dynamo]
SELECT * FROM Settings

INSERT INTO   [S3Objects_Links-Dynamo]
SELECT * FROM S3Objects_Links

INSERT INTO   [S3Objects-Dynamo]
SELECT * FROM S3Objects

INSERT INTO   [S3Buckets-Dynamo]
SELECT * FROM S3Buckets

INSERT INTO   [PackageUploads-Dynamo]
SELECT * FROM PackageUploads

INSERT INTO   [PackageUploadLink-Dynamo]
SELECT * FROM PackageUploadLink

INSERT INTO   [PackageUploadFiles-Dynamo]
SELECT * FROM PackageUploadFiles

INSERT INTO   [LinkStats-Dynamo]
SELECT * FROM LinkStats

INSERT INTO   [Links-Dynamo]
SELECT * FROM Links
