/****** Object:  View [dbo].[EMSSourceDataBrandView]    Script Date: 18-May-16 11:56:50 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER view [dbo].[EMSSourceDataBrandView]
As
SELECT  B.IDMstWeek,A.LocationCode,C.ParentLocationCode,BrandCode,StartDate,EndDate 
FROM [dbo].[PlanWeeklyProductionPlanning] A inner join [dbo].[MstGenWeek] B 
on A.KPSYear = B.Year and A.KPSWeek = B.Week inner join [dbo].[MstGenLocation] As C on
A.LocationCode = C.LocationCode

GO


