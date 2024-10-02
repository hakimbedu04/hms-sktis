/****** Object:  View [dbo].[ExePlantProductionEntryVerificationView]    Script Date: 6/16/2016 11:26:49 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

--Jejen Suhendar

--edited by: indra permana
--description: add condition on join transaction log

-- =============================================
-- Description: edit verify system
-- Ticket: http://tp.voxteneo.co.id/entity/3945
-- Author: Azka
-- Update: 05/04/2016
-- =============================================

-- =============================================
-- Description: I AM NOT CHANGE ANYTHING JUST RE-RUN THE SCRIPT :)
-- Ticket: http://tp.voxteneo.co.id/entity/7097, http://tp.voxteneo.co.id/entity/7096
-- Author: AZKA
-- Date: 2016/06/16
-- =============================================

ALTER VIEW [dbo].[ExePlantProductionEntryVerificationView]
AS
    SELECT eppev.ProductionEntryCode,
		 LocationCode,
		 UnitCode,
		 Shift,
		 ProcessGroup,
		 ProcessOrder,
		 GroupCode,
		 BrandCode,
		 KPSYear,
		 KPSWeek,
		 ProductionDate,
		 WorkHour,
		 TPKValue,
		 TotalTargetValue,
		 TotalActualValue,
		 TotalCapacityValue,
		 Remark,
		 eppev.CreatedDate,
		 eppev.CreatedBy,
		 eppev.UpdatedDate,
		 eppev.UpdatedBy,
		 Flag_Manual,
		 table1.A,
		 table1.I,
		 table1.C,
		 table1.CH,
		 table1.CT,
		 table1.SLS_SLP,
		 table1.ETC,
		 table1.Plant,
		 table1.Actual,
		 CASE WHEN eppev.VerifySystem = 1 THEN 1 ELSE 0 END AS VerifySystem,
		 eppev.VerifyManual 	  
		 FROM (
			 SELECT SUM(CASE WHEN eppe.AbsentCodeEblek = 'A' THEN 1 ELSE 0 END) AS A,
				   SUM(CASE WHEN eppe.AbsentCodeEblek = 'I' THEN 1 ELSE 0 END) AS I,
				   SUM(CASE WHEN eppe.AbsentCodeEblek = 'C' THEN 1 ELSE 0 END) AS C,
				   SUM(CASE WHEN eppe.AbsentCodeEblek = 'CH' THEN 1 ELSE 0 END) AS CH,
				   SUM(CASE WHEN eppe.AbsentCodeEblek = 'CT' THEN 1 ELSE 0 END) AS CT,
				   SUM(CASE WHEN eppe.AbsentCodeEblek IN ( 'SLS', 'SLP' ) THEN 1 ELSE 0 END) AS SLS_SLP,
				   (dbo.GetAbsent( eppe.ProductionEntryCode, NULL )) AS ETC ,
				   eppe.ProductionEntryCode,
				   SUM(eppe.ProdTarget) AS Plant,
				   SUM(eppe.ProdActual) AS Actual
			 FROM (SELECT DISTINCT p.* FROM dbo.ExePlantProductionEntry p
				 JOIN UtilTransactionLogs trans ON p.ProductionEntryCode = trans.TransactionCode 
				) AS eppe
			 GROUP BY eppe.ProductionEntryCode 
    ) AS table1
	INNER JOIN dbo.ExePlantProductionEntryVerification eppev ON table1.ProductionEntryCode = eppev.ProductionEntryCode
	--LEFT JOIN dbo.UtilTransactionLogs as UTL ON table1.ProductionEntryCode = UTL.TransactionCode AND UTL.IDFlow = 14 AND UTL.UpdatedDate = (SELECT MAX(UpdatedDate) FROM dbo.UtilTransactionLogs l WHERE l.TransactionCode = UTL.TransactionCode AND UTL.IDFlow = 14)

GO


