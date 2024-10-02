IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[dbo].[EMPJOBDATA_SCHEDULER_ABSENTEEISM_ENTRY_NOT_ACV]'))
	DROP PROCEDURE [dbo].[EMPJOBDATA_SCHEDULER_ABSENTEEISM_ENTRY_NOT_ACV]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[EMPJOBDATA_SCHEDULER_ABSENTEEISM_ENTRY_NOT_ACV]
	@empUpd_ID VARCHAR(64),
	@empUpd_Number VARCHAR(6),
	@empUpd_EffDate DATETIME,
	@empUpd_LocationCode VARCHAR(8),
	@empUpd_GroupCode VARCHAR(4),
	@empUpd_UnitCode VARCHAR(4),
	@empUpd_ProcessSettingsCode VARCHAR(16),
	@absentType VARCHAR(128),
	@tmpProductionDate DATETIME
AS
BEGIN
	IF EXISTS
	(
		SELECT * FROM ExePlantProductionEntry e
		inner join ExePlantProductionEntryVerification v on v.ProductionEntryCode = e.ProductionEntryCode
		WHERE e.EmployeeID = @empUpd_ID AND v.ProductionDate >= @empUpd_EffDate AND e.AbsentType = @absentType
	)
	BEGIN
		DELETE e
		FROM ExePlantProductionEntry e
		inner join ExePlantProductionEntryVerification v on v.ProductionEntryCode = e.ProductionEntryCode
		WHERE e.EmployeeID = @empUpd_ID AND v.ProductionDate >= @empUpd_EffDate AND e.AbsentType = @absentType
	END
END