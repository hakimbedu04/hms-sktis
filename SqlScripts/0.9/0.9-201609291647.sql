SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- Description: Worker Absenteeism View (need employee and absent type detail)
-- Ticket: http://tp.voxteneo.com/entity/56270
-- Author: Whisnu

-- =============================================================================
-- Description: Adding COALESCE sktabsentcode, payrollabsentcode
-- Ticket: http://tp.voxteneo.co.id/entity/3685
-- Author: Whisnu
-- UpdatedDate: 4/6/2016
-- =============================================================================

ALTER VIEW [dbo].[ExePlantWorkerAbsenteeismView]
AS
SELECT      wa.EmployeeNumber, 
			(SELECT TOP 1 EmployeeName FROM MstPlantEmpJobsDataAcv WHERE EmployeeID = wa.EmployeeID) as EmployeeName, 
			(SELECT TOP 1 ProcessSettingsCode FROM MstPlantEmpJobsDataAcv WHERE EmployeeID = wa.EmployeeID) as ProcessSettingsCode, 
			wa.GroupCode, 
			COALESCE(abs.SktAbsentCode, '') AS SktAbsentCode, 
			COALESCE(abs.PayrollAbsentCode, '') AS PayrollAbsentCode, 
			wa.StartDateAbsent, 
			wa.EmployeeID, 
			wa.AbsentType, 
			wa.EndDateAbsent, 
            wa.ePaf, 
			wa.Attachment, 
			wa.AttachmentPath, 
			wa.CreatedDate, 
			wa.CreatedBy, 
			wa.UpdatedDate, 
			wa.UpdatedBy, 
			wa.LocationCode, 
			wa.UnitCode, 
			wa.TransactionDate, 
			wa.Shift
FROM    dbo.ExePlantWorkerAbsenteeism AS wa INNER JOIN
        dbo.MstPlantAbsentType AS abs ON abs.AbsentType = wa.AbsentType

GO