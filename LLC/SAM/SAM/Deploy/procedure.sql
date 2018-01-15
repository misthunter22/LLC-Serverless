SET QUOTED_IDENTIFIER ON
GO
SET ANSI_NULLS ON
GO

-- =============================================
-- Author:		Jerry E. Latimer
-- Created:		April 8, 2016
-- Description:	Show various stats by source
-- =============================================
-- Change History:
-- 04/10/2016	JEL		Initial development.
-- 04/20/2016	JEL		Added order by source id

CREATE PROCEDURE [dbo].[p_Reports_SourceStats]
AS
BEGIN

	-- Source Related Statistics
	SELECT 
		  src.Id
		, [Source] = src.Name
		, s3o.ObjectCount
		, s3o.HTMLCount
		, s3o.ContentLastModified
		, s3o.FirstObjectFound
		, s3o.LastObjectFound
		, s3o.LinksLastExtracted
		, lnk.LinkCount
		, lnk.ValidLinkCount
		, lnk.InvalidLinkCount
		, lnk.LinksLastChecked
		, lnk.LinkStatsLastAggregated
	FROM dbo.Sources AS src
	LEFT OUTER JOIN 
	(
		-- S3 Objects
		SELECT 
			  s3o.Bucket
			, [ObjectCount] = COUNT(s3o.Id)
			, [HTMLCount] = SUM(CASE WHEN s3o.ItemName LIKE '%.htm%' THEN 1 ELSE 0 END)
			, [ContentLastModified] = MAX(s3o.ContentLastModified)
			, [FirstObjectFound] = MIN(s3o.DateFirstFound)
			, [LastObjectFound] = MAX(s3o.DateLastFound)
			, [LinksLastExtracted] = MAX(s3o.DateLinksLastExtracted)
		FROM dbo.Objects AS s3o
		GROUP BY s3o.Bucket
	) AS s3o
		ON s3o.Bucket = src.S3BucketId
	LEFT OUTER JOIN 
	(
		-- Links
		SELECT
			lnk.Source
			, [LinkCount] = COUNT(lnk.Id)
			, [ValidLinkCount] = SUM(CASE WHEN lnk.Valid = 1 THEN 1 ELSE 0 END)
			, [InvalidLinkCount] = SUM(CASE WHEN lnk.Valid = 0 THEN 1 ELSE 0 END)
			, [LinksLastChecked] = MAX(lnk.DateLastChecked)
			, [LinkStatsLastAggregated] = MAX(lnk.DateUpdated)
		FROM dbo.Links AS lnk
		GROUP BY lnk.Source
	) AS lnk
		ON lnk.Source = src.Id
	WHERE src.S3BucketId IS NOT NULL
	ORDER BY
		src.Id

END
GO