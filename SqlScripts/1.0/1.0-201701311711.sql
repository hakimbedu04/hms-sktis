IF EXISTS (SELECT * FROM sys.objects WHERE  object_id = OBJECT_ID(N'[dbo].[ExeReportByProcessFunc]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
  DROP FUNCTION [dbo].[ExeReportByProcessFunc]

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE FUNCTION [dbo].[ExeReportByProcessFunc]
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
	-- author : hakim
	-- ticket : http://tp.voxteneo.co.id/entity/11279
	-- desc : create function exereport by process (query pa luki)
	-- date : 2016-11-22 1528

	-- author : hakim
	-- ticket : http://tp.voxteneo.co.id/entity/13108
	-- desc : remove sktis in from t-sql
	-- date : 2017-01-31 17.00

	INSERT INTO @ExeReportByProcess
	select
		LocationCode
      ,UnitCode
      ,BrandCode
      --,KPSYear
      --,KPSWeek
      ,ProcessGroup
      ,ProcessOrder
      ,Shift
      ,Description
      ,UOM
      ,UOMOrder
      ,sum(Production) as Production 
      ,sum(KeluarBersih)as KeluarBersih
      ,sum(RejectSample) as RejectSample
      ,sum(BeginningStock)as BeginningStock
      ,sum(EndingStock)as EndingStock
	from (
	  SELECT LocationCode
		  ,UnitCode
		  ,BrandCode
		  --,KPSYear
		  --,KPSWeek
		  ,ProcessGroup
		  ,ProcessOrder
		  ,Shift
		  ,Description
		  ,UOM
		  ,UOMOrder
		  ,Production
		  ,KeluarBersih
		  ,RejectSample
		  ,0 as BeginningStock
		  ,0 as EndingStock
	  FROM ExeReportByProcess
	  where ProductionDate between @StartDate AND @EndDate
  
	  UNION ALL
	  SELECT LocationCode
		  ,UnitCode
		  ,BrandCode
		  --,KPSYear
		  --,KPSWeek
		  ,ProcessGroup
		  ,ProcessOrder
		  ,Shift
		  ,Description
		  ,UOM
		  ,UOMOrder
		  ,0 as Production
		  ,0 KeluarBersih
		  ,0 as RejectSample
		  ,BeginningStock
		  ,0 as EndingStock
	  FROM ExeReportByProcess
	  where ProductionDate = @StartDate
	  UNION ALL
	  SELECT LocationCode
		  ,UnitCode
		  ,BrandCode
		  --,KPSYear
		  --,KPSWeek
		  ,ProcessGroup
		  ,ProcessOrder
		  ,Shift
		  ,Description
		  ,UOM
		  ,UOMOrder
		  ,0 as Production
		  ,0 as KeluarBersih
		  ,0 as RejectSample
		  ,0 as BeginningStock
		  ,EndingStock
	  FROM ExeReportByProcess
	  where ProductionDate = @EndDate
	)  x
	where LocationCode = @LocationCode
	  and UnitCode = @UnitCode
	  and shift = @Shift
	  and BrandCode = @BrandCode
	group by LocationCode,UnitCode,BrandCode,ProcessGroup,ProcessOrder,Shift,Description,UOM,UOMOrder
	ORDER BY UOMOrder ASC;
	RETURN;
END;

