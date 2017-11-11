-- Drop any existing temp tables

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

GO

-- Create new temp tables

CREATE TABLE [LinkReports-Dynamo] (
    [NewId] varchar(256),
    Id varchar(256),
    ContentSize bigint NULL,
    Mean bigint,
    StandardDeviation float,
    SdMaximum int,
	Link_LinkId varchar(256),
	LinkStatId varchar(256) NULL,
	PRIMARY KEY (Id)
);
GO

CREATE TABLE [Sources-Dynamo] (
	[NewId] varchar(256),
    SourceId varchar(256),
    [Name] varchar(256),
    [Description] varchar(1024) NULL,
    AllowLinkChecking bit,
    AllowLinkExtractions bit,
	S3BucketId varchar(256) NULL,
	DateCreated smalldatetime,
	PRIMARY KEY (SourceId)
);
GO

CREATE TABLE [Settings-Dynamo] (
	[NewId] varchar(256),
	SettingId varchar(256),
	[Name] varchar(256),
	[Value] varchar(1024) NULL,
	[Description] varchar(1024) NULL,
	DateCreated datetime,
	DateModified datetime NULL,
	ModifiedUser varchar(256),
	PRIMARY KEY (SettingId)
);
GO

CREATE TABLE [S3Objects_Links-Dynamo] (
	[NewId] varchar(256),
	S3ObjectLinkId varchar(256),
	S3ObjectId varchar(256),
	LinkId varchar(256),
	DateFirstFound smalldatetime,
	DateLastFound smalldatetime,
	DateRemoved smalldatetime NULL,
	PRIMARY KEY (S3ObjectLinkId)
);
GO

CREATE TABLE [S3Objects-Dynamo] (
	[NewId] varchar(256),
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
GO

CREATE TABLE [S3Buckets-Dynamo] (
	[NewId] varchar(256),
	S3BucketId varchar(256),
	[Name] varchar(256),
	AccessKey varchar(256),
	SecretKey varchar(256),
	Region varchar(256) NULL,
	SearchPrefix varchar(256) NULL,
	DateCreated smalldatetime,
	PRIMARY KEY (S3BucketId)
);
GO

CREATE TABLE [PackageUploads-Dynamo] (
	[NewId] varchar(256),
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
GO

CREATE TABLE [PackageUploadLink-Dynamo] (
	[NewId] varchar(256),
	PackageUploads_Id varchar(256),
	Links_LinkId varchar(256),
	PRIMARY KEY (PackageUploads_Id)
);
GO

CREATE TABLE [PackageUploadFiles-Dynamo] (
	[NewId] varchar(256),
	Id varchar(256),
	CourseLocation nvarchar(MAX),
	Link_LinkId varchar(256),
	Protocol varchar(MAX) NULL,
	LinkName varchar(MAX) NULL,
	ParentFolder varchar(MAX) NULL,
	PackageUploadId varchar(256) NULL,
	PRIMARY KEY (Id)
);
GO

CREATE TABLE [LinkStats-Dynamo] (
	[NewId] varchar(256),
	LinkStatId varchar(256),
	LinkId varchar(256),
	ContentSize bigint NULL,
	DownloadTime int NULL,
	DateChecked datetime,
	ErrorMessage varchar(512) NULL,
	StatusCode varchar(256) NULL,
	StatusDesc varchar(2048) NULL,
	ContentType varchar(256) NULL,
	PRIMARY KEY (LinkStatId)
);
GO

CREATE TABLE [Links-Dynamo] (
	[NewId] varchar(256),
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
GO

-- Copy over the data

INSERT INTO   [LinkReports-Dynamo] (Id, ContentSize, Mean, StandardDeviation, SdMaximum, Link_LinkId, LinkStatId)
SELECT Id, ContentSize, Mean, StandardDeviation, SdMaximum, Link_LinkId, LinkStatId FROM LinkReports

INSERT INTO   [Sources-Dynamo] (SourceId, [Name], [Description], AllowLinkChecking, AllowLinkExtractions, S3BucketId, DateCreated)
SELECT SourceId, [Name], [Description], AllowLinkChecking, AllowLinkExtractions, S3BucketId, DateCreated FROM Sources

INSERT INTO   [Settings-Dynamo] (SettingId, [Name], [Value], [Description], DateCreated, DateModified, ModifiedUser)
SELECT SettingId, [Name], [Value], [Description], DateCreated, DateModified, ModifiedUser FROM Settings

INSERT INTO   [S3Objects_Links-Dynamo] (S3ObjectLinkId, S3ObjectId, LinkId, DateFirstFound, DateLastFound, DateRemoved)
SELECT S3ObjectLinkId, S3ObjectId, LinkId, DateFirstFound, DateLastFound, DateRemoved FROM S3Objects_Links

INSERT INTO   [S3Objects-Dynamo] (S3ObjectId, S3BucketId, [Key], ItemName, ETag, IsFolder, ContentLastModified, DateFirstFound, DateLastFound, DateLinksLastExtracted, LinkCheckDisabledDate, LinkCheckDisabledUser)
SELECT S3ObjectId, S3BucketId, [Key], ItemName, ETag, IsFolder, ContentLastModified, DateFirstFound, DateLastFound, DateLinksLastExtracted, LinkCheckDisabledDate, LinkCheckDisabledUser FROM S3Objects

INSERT INTO   [S3Buckets-Dynamo] (S3BucketId, [Name], AccessKey, SecretKey, Region, SearchPrefix, DateCreated)
SELECT S3BucketId, [Name], AccessKey, SecretKey, Region, SearchPrefix, DateCreated FROM S3Buckets

INSERT INTO   [PackageUploads-Dynamo] (Id, [Name], [Description], UploadedBy, DateUploaded, [Key], PackageProcessed, [FileName], ImsSchema, ImsSchemaVersion, ImsTitle, ImsDescription)
SELECT Id, [Name], [Description], UploadedBy, DateUploaded, [Key], PackageProcessed, [FileName], ImsSchema, ImsSchemaVersion, ImsTitle, ImsDescription FROM PackageUploads

INSERT INTO   [PackageUploadLink-Dynamo] (PackageUploads_Id, Links_LinkId)
SELECT PackageUploads_Id, Links_LinkId FROM PackageUploadLink

INSERT INTO   [PackageUploadFiles-Dynamo] (Id, CourseLocation, Link_LinkId, Protocol, LinkName, ParentFolder, PackageUploadId)
SELECT Id, CourseLocation, Link_LinkId, Protocol, LinkName, ParentFolder, PackageUploadId FROM PackageUploadFiles

INSERT INTO   [LinkStats-Dynamo] (LinkStatId, LinkId, ContentSize, DownloadTime, DateChecked, ErrorMessage, StatusCode, StatusDesc, ContentType)
SELECT LinkStatId, LinkId, ContentSize, DownloadTime, DateChecked, ErrorMessage, StatusCode, StatusDesc, ContentType FROM LinkStats

INSERT INTO   [Links-Dynamo] (LinkId, SourceId, LinkUrl, DateFirstFound, DateLastFound, LinkCheckDisabledDate, LinkCheckDisabledUser, AttemptCount, IsValid, DateLastChecked, AllTimeMinDownloadTime, AllTimeMaxDownloadTime, AllTimeStdDevDownloadTime, PastWeekMinDownloadTime, PastWeekMaxDownloadTime, PastWeekStdDevDownloadTime, DateStatsUpdated, ReportNotBeforeDate)
SELECT LinkId, SourceId, LinkUrl, DateFirstFound, DateLastFound, LinkCheckDisabledDate, LinkCheckDisabledUser, AttemptCount, IsValid, DateLastChecked, AllTimeMinDownloadTime, AllTimeMaxDownloadTime, AllTimeStdDevDownloadTime, PastWeekMinDownloadTime, PastWeekMaxDownloadTime, PastWeekStdDevDownloadTime, DateStatsUpdated, ReportNotBeforeDate FROM Links

GO

-- Set the default for NewId

UPDATE [S3Buckets-Dynamo]
SET [NewId] = newid()
GO

UPDATE [Sources-Dynamo]
SET [NewId] = newid()
GO

UPDATE [Settings-Dynamo]
SET [NewId] = newid()
GO

UPDATE [LinkReports-Dynamo]
SET [NewId] = newid()
GO

UPDATE [S3Objects_Links-Dynamo]
SET [NewId] = newid()
GO

UPDATE [S3Objects-Dynamo]
SET [NewId] = newid()
GO

UPDATE [PackageUploads-Dynamo]
SET [NewId] = newid()
GO

UPDATE [PackageUploadLink-Dynamo]
SET [NewId] = newid()
GO

UPDATE [PackageUploadFiles-Dynamo]
SET [NewId] = newid()
GO

UPDATE [LinkStats-Dynamo] 
SET [NewId] = newid()
GO

UPDATE [Links-Dynamo]
SET [NewId] = newid()
GO

-- Update any references

UPDATE [Sources-Dynamo]
SET S3BucketId = (SELECT [NewId] 
				  FROM [S3Buckets-Dynamo] 
				  WHERE [S3Buckets-Dynamo].S3BucketId = [Sources-Dynamo].S3BucketId)
GO

UPDATE [Links-Dynamo]
SET SourceId = (SELECT [NewId] 
				FROM [Sources-Dynamo] 
				WHERE [Links-Dynamo].SourceId = [Sources-Dynamo].SourceId)
GO

UPDATE [LinkStats-Dynamo]
SET LinkId = (SELECT [NewId] 
		      FROM [Links-Dynamo] 
			  WHERE [Links-Dynamo].LinkId = [LinkStats-Dynamo].LinkId)
GO

UPDATE [S3Objects-Dynamo]
SET S3BucketId = (SELECT [NewId] 
				  FROM [S3Buckets-Dynamo] 
				  WHERE [S3Buckets-Dynamo].S3BucketId = [S3Objects-Dynamo].S3BucketId)
GO

UPDATE [LinkReports-Dynamo]
SET Link_LinkId = (SELECT [NewId] 
		           FROM [Links-Dynamo] 
			       WHERE [Links-Dynamo].LinkId = [LinkReports-Dynamo].Link_LinkId)
GO

UPDATE [LinkReports-Dynamo]
SET LinkStatId = (SELECT [NewId] 
		          FROM [LinkStats-Dynamo] 
			      WHERE [LinkStats-Dynamo].LinkStatId = [LinkReports-Dynamo].LinkStatId)
GO

UPDATE [PackageUploadLink-Dynamo]
SET Links_LinkId = (SELECT [NewId] 
		            FROM [Links-Dynamo] 
			        WHERE [Links-Dynamo].LinkId = [PackageUploadLink-Dynamo].Links_LinkId)
GO

UPDATE [S3Objects_Links-Dynamo]
SET LinkId = (SELECT [NewId] 
		      FROM [Links-Dynamo] 
			  WHERE [Links-Dynamo].LinkId = [S3Objects_Links-Dynamo].LinkId)
GO

UPDATE [S3Objects_Links-Dynamo]
SET S3ObjectId = (SELECT [NewId] 
		          FROM [S3Objects-Dynamo] 
			      WHERE [S3Objects-Dynamo].S3ObjectId = [S3Objects_Links-Dynamo].S3ObjectId)
GO

UPDATE [PackageUploadFiles-Dynamo]
SET Link_LinkId = (SELECT [NewId] 
		           FROM [Links-Dynamo] 
			       WHERE [Links-Dynamo].LinkId = [PackageUploadFiles-Dynamo].Link_LinkId)
GO

UPDATE [PackageUploadFiles-Dynamo]
SET PackageUploadId = (SELECT [NewId] 
		               FROM [PackageUploads-Dynamo] 
			           WHERE [PackageUploads-Dynamo].Id = [PackageUploadFiles-Dynamo].PackageUploadId)
GO

-- Move over the NewId

UPDATE [S3Buckets-Dynamo]
SET S3BucketId = [NewId]
GO

UPDATE [Sources-Dynamo]
SET SourceId = [NewId]
GO

UPDATE [Settings-Dynamo]
SET SettingId = [NewId]
GO

UPDATE [LinkReports-Dynamo]
SET Id = [NewId]
GO

UPDATE [S3Objects_Links-Dynamo]
SET S3ObjectLinkId = [NewId]
GO

UPDATE [S3Objects-Dynamo]
SET S3ObjectId = [NewId]
GO

UPDATE [PackageUploads-Dynamo]
SET Id = [NewId]
GO

UPDATE [PackageUploadLink-Dynamo]
SET PackageUploads_Id = [NewId]
GO

UPDATE [PackageUploadFiles-Dynamo]
SET Id = [NewId]
GO

UPDATE [LinkStats-Dynamo] 
SET LinkStatId = [NewId]
GO

UPDATE [Links-Dynamo]
SET LinkId = [NewId]
GO

-- Drop the temp columns

ALTER TABLE [S3Buckets-Dynamo]
DROP COLUMN [NewId]
GO

ALTER TABLE [Sources-Dynamo]
DROP COLUMN [NewId]
GO

ALTER TABLE [Settings-Dynamo]
DROP COLUMN [NewId]
GO

ALTER TABLE [LinkReports-Dynamo]
DROP COLUMN [NewId]
GO

ALTER TABLE [S3Objects_Links-Dynamo]
DROP COLUMN [NewId]
GO

ALTER TABLE [S3Objects-Dynamo]
DROP COLUMN [NewId]
GO

ALTER TABLE [PackageUploads-Dynamo]
DROP COLUMN [NewId]
GO

ALTER TABLE [PackageUploadLink-Dynamo]
DROP COLUMN [NewId]
GO

ALTER TABLE [PackageUploadFiles-Dynamo]
DROP COLUMN [NewId]
GO

ALTER TABLE [LinkStats-Dynamo]
DROP COLUMN [NewId]
GO

ALTER TABLE [Links-Dynamo]
DROP COLUMN [NewId]
GO