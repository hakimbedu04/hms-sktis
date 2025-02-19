/****** Object:  UserDefinedFunction [dbo].[GetExeReportMaterialUsageWeekly]    Script Date: 18/05/2016 11:58:37 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER FUNCTION [dbo].[GetExeReportMaterialUsageWeekly]
(	
	@LocationCode VARCHAR(10),
	@UnitCode VARCHAR(10),
	@YearFromWeek int,
	@YearToWeek int,
	@WeekFrom int,
	@WeekTo int
)
RETURNS TABLE 
AS
RETURN 
(
	WITH ReportMaterialUsage AS(		
		select a.LocationCode as LocationCode,a.UnitCode as UnitCode,a.BrandGroupCode as BrandGroupCode,b.MaterialCode as MaterialCode,b.MaterialName as MaterialName, Ambil1,Ambil2, Ambil3,pakai,Sisa,TobFM,TobStem,TobSapon,UncountableWaste,CountableWaste
		from
			(select a.LocationCode,a.UnitCode,a.BrandGroupCode,a.MaterialCode,sum(a.Ambil1) Ambil1, sum(a.Ambil2) Ambil2,sum(a.Ambil3) Ambil3,sum(a.pakai) pakai,sum(a.TobFM) TobFM,sum(a.TobStem) TobStem,sum(a.TobSapon) TobSapon,sum(a.UncountableWaste) UncountableWaste,sum(a.Sisa) Sisa,sum(a.CountableWaste) CountableWaste
			From [dbo].[ExeMaterialUsage] a inner join [dbo].[MstGenWeek] b on ProductionDate >= StartDate and ProductionDate <= EndDate 
			where LocationCode like @LocationCode 
			and Year between @YearFromWeek and @YearToWeek and Week between @WeekFrom and @WeekTo
			and UnitCode = @UnitCode 
			group by a.MaterialCode,a.LocationCode,a.UnitCode,a.BrandGroupCode) a,[dbo].[MstGenMaterial] b
		where a.MaterialCode=b.MaterialCode and a.BrandGroupCode=b.BrandGroupCode 
	)

	SELECT  
		LocationCode,
		UnitCode,
		BrandGroupCode,
		MaterialCode,
		MaterialName,
		ROUND(SUM(Ambil1+Ambil2+Ambil3),2) AS Take,
		ROUND(SUM(pakai),2) AS Production,
		ROUND(SUM(TobFM),2) AS TobFM,
		ROUND(SUM(TobStem),2) AS TobStem,
		ROUND(SUM(TobSapon),2) AS TobSapon,
		ROUND(SUM(UncountableWaste),2) AS Reject,
		ROUND(SUM(Sisa),2) AS Residu,
		ROUND(SUM(CountableWaste),2) AS Lost
	FROM ReportMaterialUsage
	GROUP BY
		LocationCode,
		UnitCode,
		BrandGroupCode,
		MaterialCode,
		MaterialName
)
