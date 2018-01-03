IF db_id('LLC') IS NULL 
	CREATE DATABASE LLC
GO

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_Objects' AND object_id = OBJECT_ID('Objects'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_Objects ON [LLC].[dbo].[Objects] ([Key])
END

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_LinksUrl' AND object_id = OBJECT_ID('Links'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_LinksUrl ON [LLC].[dbo].[Links] ([Url])
END

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_ReportsLink' AND object_id = OBJECT_ID('Reports'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_ReportsLink ON [LLC].[dbo].[Reports] ([Link])
END

IF NOT EXISTS(SELECT * FROM sys.indexes WHERE name = 'IX_StatsLink' AND object_id = OBJECT_ID('Stats'))
BEGIN
    CREATE NONCLUSTERED INDEX IX_StatsLink ON [LLC].[dbo].[Stats] ([Link])
END