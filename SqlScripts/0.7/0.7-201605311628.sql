/****** Object:  View [dbo].[ExeTPOProductionEntryVerificationView]    Script Date: 5/31/2016 4:04:08 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description: add rounding totalactualvalue
-- Ticket: http://tp.voxteneo.co.id/entity/6823
-- Author: AZKA
-- Updated: 1.0 - 5/31/2016
-- =============================================

ALTER VIEW [dbo].[ExeTPOProductionEntryVerificationView]
AS

SELECT        et.ProductionEntryCode, etv.LocationCode, etv.BrandCode, etv.KPSYear, etv.KPSWeek, etv.ProductionDate, etv.ProcessGroup, SUM(et.Absent) AS Absent, etv.TotalTPKValue, ROUND(SUM(et.ActualProduction), 2) AS TotalActualValue, 
                         etv.VerifySystem, etv.VerifyManual
FROM            dbo.ExeTPOProduction AS et INNER JOIN
                         dbo.ExeTPOProductionEntryVerification AS etv ON etv.ProductionEntryCode = et.ProductionEntryCode
GROUP BY etv.LocationCode, etv.BrandCode, etv.KPSYear, etv.KPSWeek, etv.ProductionDate, etv.ProcessGroup, etv.TotalTPKValue, etv.VerifySystem, etv.VerifyManual, et.ProductionEntryCode

GO


