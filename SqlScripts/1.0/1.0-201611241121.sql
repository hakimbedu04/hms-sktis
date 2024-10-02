/****** Object:  UserDefinedFunction [dbo].[GetReportByStatusTotalWorkHour]    Script Date: 11/24/2016 11:03:44 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER FUNCTION [dbo].[GetReportByStatusTotalWorkHour]
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
		IF (@paramUnitCode = 'All')
			SET @paramUnitCode = NULL;

		INSERT INTO @result
		SELECT TOP 1 @paramLocationCode, SUM(MaxWorkHourPerDay) FROM
		(
		select ProductionDate, ROUND(MAX(WorkHour), 2) as MaxWorkHourPerDay 
		from ExeReportByGroups where LocationCode = @paramLocationCode and UnitCode = COALESCE(@paramUnitCode, UnitCode) and Shift = @paramShift
		and BrandGroupCode = @paramBrandGroup and ProductionDate between @paramProdDateFrom and @paramProdDateTo AND BrandCode = COALESCE(@paramBrandCode, BrandCode)
		group by ProductionDate
		) d
	END
	
	RETURN 
END