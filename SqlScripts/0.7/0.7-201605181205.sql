
/****** Object:  View [dbo].[ExeReportMaterialUsageView]    Script Date: 18-May-16 12:04:48 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO




-- Description: Create [dbo].[ExeReportMaterialUsageView]
-- Ticket: TIM HM SAMPOERNA
-- Author: TAUFIK HIDAYAT

ALTER VIEW [dbo].[ExeReportMaterialUsageView]
AS
 
 
 SELECT LocationCode,UnitCode,MaterialCode,ProductionDate,Year,Week,Month,
	   ROUND(Sum(Ambil1 + Ambil2 + Ambil3),2) as Take,
	   ROUND(Sum(pakai),2) as Production,
	   ROUND(Sum(TobFM ),2) as TobFM,
	   ROUND(Sum(TobSapon ),2) as TobSapon,
	   ROUND(Sum(TobStem ),2) as TobStem,
	   ROUND(Sum(CountableWaste),2) as Reject,
	   ROUND(Sum(Sisa),2) as Residu,
	   ROUND(Sum(UncountableWaste),2) as Lost
  FROM [dbo].[ExeMaterialUsage] inner join [dbo].[MstGenWeek]  
  on ProductionDate >= StartDate and ProductionDate <= EndDate
  Group by LocationCode,UnitCode,MaterialCode,ProductionDate,Year,Week,Month

GO


