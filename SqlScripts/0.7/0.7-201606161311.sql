/****** Object:  View [dbo].[PlanPlantIndividualCapacityByReferenceView]    Script Date: 6/16/2016 12:56:56 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



-- Description: Fix view PlanPlantIndividualCapacityByReferenceView
-- Ticket: --
-- Author: Oka
-- Modified: Indra Permana
-- Add ISNULL
-- Modified: I Made Ardi Siskayana
-- Add ISNULL absentType and prodActual ( > 0 & IS NOT NULL)

-- =============================================
-- Description: I AM NOT CHANGE ANYTHING JUST RE-RUN THE SCRIPT :)
-- Ticket: http://tp.voxteneo.co.id/entity/7097, http://tp.voxteneo.co.id/entity/7096
-- Author: AZKA
-- Date: 2016/06/16
-- =============================================

ALTER VIEW [dbo].[PlanPlantIndividualCapacityByReferenceView]

AS

SELECT        
		dbo.ExePlantProductionEntryVerification.ProductionDate, 
		dbo.ExePlantProductionEntryVerification.GroupCode, 
		dbo.ExePlantProductionEntryVerification.UnitCode, 
		dbo.ExePlantProductionEntryVerification.LocationCode, 
		dbo.ExePlantProductionEntryVerification.ProcessGroup, 
		dbo.ExePlantProductionEntryVerification.BrandCode, 
		dbo.ExePlantProductionEntryVerification.WorkHour,
		ISNULL(MIN(ProdActual) OVER (PARTITION BY dbo.ExePlantProductionEntry.EmployeeID), 0) AS MinimumValue, 
		ISNULL(MAX(ProdActual) OVER (PARTITION BY dbo.ExePlantProductionEntry.EmployeeID), 0) AS MaximumValue, 
		ISNULL(AVG(ProdActual) OVER (PARTITION BY dbo.ExePlantProductionEntry.EmployeeID), 0) AS AverageValue,
		ISNULL((((SELECT        MAX(ProdActual)
			FROM            (SELECT        TOP
			50
			PERCENT
			ProdActual
			FROM            dbo.ExePlantProductionEntryVerification
			ORDER
			BY
			ProdActual)
			AS
			BottomHalf)
			+
			(SELECT        MIN(ProdActual)
			FROM            (SELECT        TOP 50 PERCENT ProdActual
			FROM            dbo.ExePlantProductionEntryVerification
			ORDER BY ProdActual DESC) AS TopHalf)) / 2 ),0) AS MedianValue, 
		ISNULL(ProdActual, 0) AS LatestValue, 
		dbo.MstPlantEmpJobsDataAcv.EmployeeID AS EmployeeID, 
		dbo.MstPlantEmpJobsDataAcv.EmployeeNumber AS EmployeeNumber, 
		dbo.PlanPlantIndividualCapacityWorkHours.HoursCapacity3, 
		dbo.PlanPlantIndividualCapacityWorkHours.HoursCapacity5, 
		dbo.PlanPlantIndividualCapacityWorkHours.HoursCapacity6, 
		dbo.PlanPlantIndividualCapacityWorkHours.HoursCapacity7, 
		dbo.PlanPlantIndividualCapacityWorkHours.HoursCapacity8, 
		dbo.PlanPlantIndividualCapacityWorkHours.HoursCapacity9, 
		dbo.PlanPlantIndividualCapacityWorkHours.HoursCapacity10
	FROM            
		dbo.ExePlantProductionEntryVerification INNER JOIN
		dbo.ExePlantProductionEntry ON dbo.ExePlantProductionEntryVerification.ProductionEntryCode = dbo.ExePlantProductionEntry.ProductionEntryCode and dbo.ExePlantProductionEntry.AbsentType IS NULL and (dbo.ExePlantProductionEntry.ProdActual > 0 and dbo.ExePlantProductionEntry.ProdActual IS NOT NULL) INNER JOIN
		dbo.MstPlantEmpJobsDataAcv ON dbo.MstPlantEmpJobsDataAcv.EmployeeID = dbo.ExePlantProductionEntry.EmployeeID INNER JOIN
		dbo.PlanPlantIndividualCapacityWorkHours ON dbo.PlanPlantIndividualCapacityWorkHours.EmployeeID = dbo.ExePlantProductionEntry.EmployeeID AND 
		dbo.PlanPlantIndividualCapacityWorkHours.GroupCode = dbo.ExePlantProductionEntryVerification.GroupCode AND 
		dbo.PlanPlantIndividualCapacityWorkHours.UnitCode = dbo.ExePlantProductionEntryVerification.UnitCode AND 
		dbo.PlanPlantIndividualCapacityWorkHours.LocationCode = dbo.ExePlantProductionEntryVerification.LocationCode AND 
		dbo.PlanPlantIndividualCapacityWorkHours.ProcessGroup = dbo.ExePlantProductionEntryVerification.ProcessGroup;


GO


