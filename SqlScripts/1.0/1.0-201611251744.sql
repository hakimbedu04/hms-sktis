IF EXISTS (SELECT * FROM sys.objects WHERE  object_id = OBJECT_ID(N'[dbo].[ExeReportByProcessFuncParent]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
	DROP FUNCTION [dbo].[ExeReportByProcessFuncParent]

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[ExeReportByProcessFuncParent]
(	
	@StartDate as DATETIME,
	@EndDate as DATETIME,
	@LocationCode VARCHAR(20),
	@UnitCode VARCHAR(20),
	@Shift INT,
	@BrandCode VARCHAR(11)
)
RETURNS @ExeReportByProcess TABLE (LocationCode VARCHAR(10), UnitCode VARCHAR(10), BrandCode VARCHAR(11), ProcessGroup VARCHAR(20),ProcessOrder INT NOT NULL, Shift INT NOT NULL
, Description VARCHAR(255), UOM VARCHAR(100), UOMOrder INT NOT NULL, Production FLOAT NOT NULL,KeluarBersih FLOAT NOT NULL, RejectSample FLOAT NOT NULL, BeginningStock FLOAT NOT NULL, EndingStock FLOAT NOT NULL)
AS
BEGIN
	IF (@UnitCode = 'All')
		SET @UnitCode = NULL;

	IF (@BrandCode = 'All')
		SET @BrandCode = NULL;
	
	INSERT INTO @ExeReportByProcess
	select
	   @LocationCode as LocationCode
      ,CASE WHEN (ISNULL(@UnitCode, '') = '') THEN 'All' ELSE @UnitCode END as UnitCode
      ,CASE WHEN (ISNULL(@BrandCode, '') = '') THEN 'All' ELSE @BrandCode END as BrandCode
      --,KPSYear
      --,KPSWeek
      ,' ' as ProcessGroup
      ,0 as ProcessOrder
      ,@Shift as Shift
	  ,[Description]
      ,UOM
      ,UOMOrder
      ,sum(Production) as Production 
      ,sum(KeluarBersih)as KeluarBersih
      ,sum(RejectSample) as RejectSample
      ,sum(BeginningStock)as BeginningStock
      ,sum(EndingStock)as EndingStock
	from (
	  SELECT Description
		  ,UOM
		  ,UOMOrder
		  ,Production
		  ,KeluarBersih
		  ,RejectSample
		  ,0 as BeginningStock
		  ,0 as EndingStock
	  FROM [SKTIS].[dbo].[ExeReportByProcess]
	  where ProductionDate between @StartDate AND @EndDate
  and
	  LocationCode IN (SELECT LocationCode FROM [dbo].GetLastChildLocation(@LocationCode))
	  and UnitCode = COALESCE(@UnitCode, UnitCode)
	  and shift = @Shift
	  and BrandCode = COALESCE(@BrandCode, BrandCode)
	  UNION ALL
	  SELECT [Description]
		  ,UOM
		  ,UOMOrder
		  ,0 as Production
		  ,0 KeluarBersih
		  ,0 as RejectSample
		  ,BeginningStock
		  ,0 as EndingStock
	  FROM [SKTIS].[dbo].[ExeReportByProcess]
	  where ProductionDate = @StartDate
	  and
	  LocationCode IN (SELECT LocationCode FROM [dbo].GetLastChildLocation(@LocationCode))
	  and UnitCode = COALESCE(@UnitCode, UnitCode)
	  and shift = @Shift
	  and BrandCode = COALESCE(@BrandCode, BrandCode)
	  UNION ALL
	  SELECT [Description]
		  ,UOM
		  ,UOMOrder
		  ,0 as Production
		  ,0 as KeluarBersih
		  ,0 as RejectSample
		  ,0 as BeginningStock
		  ,EndingStock
	  FROM [SKTIS].[dbo].[ExeReportByProcess]
	  where ProductionDate = @EndDate and
	  LocationCode IN (SELECT LocationCode FROM [dbo].GetLastChildLocation(@LocationCode))
	  and UnitCode = COALESCE(@UnitCode, UnitCode)
	  and shift = @Shift
	  and BrandCode = COALESCE(@BrandCode, BrandCode)
	)  x
	group by Description,UOM,UOMOrder
	ORDER BY UOMOrder ASC;
	RETURN;
END;