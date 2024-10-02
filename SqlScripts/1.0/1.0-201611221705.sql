IF EXISTS (SELECT * FROM   sys.objects
           WHERE  object_id = OBJECT_ID(N'[dbo].[GetReportByStatusTotalWorkHour]')
                  AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
  DROP FUNCTION [dbo].[GetReportByStatusTotalWorkHour]

/****** Object:  UserDefinedFunction [dbo].[GetReportByStatusTotalWorkHour]    Script Date: 11/22/2016 5:05:11 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[GetReportByStatusTotalWorkHour]
(
	@paramLocationCode	VARCHAR(10),
	@paramUnitCode		VARCHAR(10),
	@paramShift			INT,
	@paramBrandGroup	VARCHAR(20),
	@paramBrandCode		VARCHAR(20),
	@paramProdDateFrom	DATETIME,
	@paramProdDateTo	DATETIME
)
RETURNS 
@result TABLE 
(
	LocationCode  VARCHAR(10),
	TotalWorkHour DECIMAL(10, 2)
)
AS
BEGIN
	-- Declare the return variable here
	DECLARE @ResultVar DECIMAL(10, 2);

	DECLARE @countChild INT;
	SELECT @countChild = COUNT(*) FROM [dbo].[GetLastChildLocation](@paramLocationCode);

	IF (@paramBrandCode = 'All')
		SET @paramBrandCode = NULL;

	IF (@countChild > 1) -- Parent Location SKT, TPO, REG1-4
	BEGIN
		INSERT INTO @result
		SELECT TOP 1 @paramLocationCode, SUM(MaxWorkHourPerDay) FROM
		(
		select ProductionDate, ROUND(MAX(WorkHour), 2) as MaxWorkHourPerDay 
		from ExeReportByGroups where LocationCode in (select LocationCode from GetLastChildLocation(@paramLocationCode)) and Shift = @paramShift
		and BrandGroupCode = @paramBrandGroup and ProductionDate between @paramProdDateFrom and @paramProdDateTo AND BrandCode = COALESCE(@paramBrandCode, BrandCode)
		group by ProductionDate
		) d
	END
	ELSE
	BEGIN
		INSERT INTO @result
		SELECT TOP 1 @paramLocationCode, SUM(MaxWorkHourPerDay) FROM
		(
		select ProductionDate, ROUND(MAX(WorkHour), 2) as MaxWorkHourPerDay 
		from ExeReportByGroups where LocationCode = @paramLocationCode and UnitCode = @paramUnitCode and Shift = @paramShift
		and BrandGroupCode = @paramBrandGroup and ProductionDate between @paramProdDateFrom and @paramProdDateTo AND BrandCode = COALESCE(@paramBrandCode, BrandCode)
		group by ProductionDate
		) d
	END
	
	RETURN 
END