/****** Object:  UserDefinedFunction [dbo].[GetExeReportMaterialUsagePeriod]    Script Date: 18/05/2016 11:58:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER FUNCTION [dbo].[GetExeReportMaterialUsagePeriod]
(	
	@LocationCode VARCHAR(10),
	@UnitCode VARCHAR(10),
	@Year int,
	@Month int,
	@MaterialCode VARCHAR(10)
)
RETURNS TABLE 
AS
RETURN 
(
	WITH ReportMaterialUsagePeriod AS(		
		select a.ProductionDate,b.Week,a.LocationCode,a.UnitCode,a.BrandGroupCode,a.MaterialCode,a.Ambil1 Ambil1, a.Ambil2 Ambil2,a.Ambil3 Ambil3,a.pakai pakai,a.TobFM TobFM,a.TobStem TobStem,a.TobSapon TobSapon,a.UncountableWaste UncountableWaste,a.Sisa Sisa,a.CountableWaste CountableWaste
		From [dbo].[ExeMaterialUsage] a inner join [dbo].[MstGenWeek] b on ProductionDate >= StartDate and ProductionDate <= EndDate 
		where LocationCode like @LocationCode
		and DATEPART(yyyy,a.ProductionDate)=@Year AND DATEPART(MM,a.ProductionDate)=@Month
		and UnitCode = @UnitCode 
		and MaterialCode = @MaterialCode
	)

	SELECT  
		ProductionDate,
		Week,
		LocationCode,
		UnitCode,
		BrandGroupCode,
		MaterialCode,
		ROUND(SUM(Ambil1+Ambil2+Ambil3),2) AS Take,
		pakai AS Production,
		TobFM AS TobFM,
		TobStem AS TobStem,
		TobSapon AS TobSapon,
		UncountableWaste AS Reject,
		Sisa AS Residu,
		CountableWaste AS Lost
	FROM ReportMaterialUsagePeriod	
	GROUP BY
		ProductionDate,
		Week,
		LocationCode,
		UnitCode,
		BrandGroupCode,
		MaterialCode,	
		pakai,
		TobFM,
		TobStem,
		TobSapon,
		UncountableWaste,
		Sisa,
		CountableWaste
)