IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[dbo].[EMPJOBDATA_SCHEDULER_ASSIGNMENT_ENTRY_NOT_ACV]'))
	DROP PROCEDURE [dbo].[EMPJOBDATA_SCHEDULER_ASSIGNMENT_ENTRY_NOT_ACV]
GO
/****** Object:  StoredProcedure [dbo].[EMPJOBDATA_SCHEDULER_ASSIGNMENT_ENTRY_NOT_ACV]    Script Date: 9/28/2016 5:00:12 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[EMPJOBDATA_SCHEDULER_ASSIGNMENT_ENTRY_NOT_ACV]
	@empUpd_ID VARCHAR(64),
	@empUpd_Number VARCHAR(6),
	@empUpd_EffDate DATETIME,
	@empUpd_LocationCode VARCHAR(8),
	@empUpd_GroupCode VARCHAR(4),
	@empUpd_UnitCode VARCHAR(4),
	@empUpd_ProcessSettingsCode VARCHAR(16),
	@tmpProductionDate DATETIME,
	@dummyGroup VARCHAR(4)
AS
BEGIN
	IF EXISTS
	(
		SELECT * FROM ExePlantProductionEntry e
		inner join ExePlantProductionEntryVerification v on v.ProductionEntryCode = e.ProductionEntryCode
		WHERE e.EmployeeID = @empUpd_ID AND v.ProductionDate = @tmpProductionDate AND e.AbsentType = 'Multiskill Out'
	)
	BEGIN
		DELETE e
		FROM ExePlantProductionEntry e
		inner join ExePlantProductionEntryVerification v on v.ProductionEntryCode = e.ProductionEntryCode
		WHERE e.EmployeeID = @empUpd_ID AND v.ProductionDate = @tmpProductionDate AND e.AbsentType = 'Multiskill Out'
	END
	--- dummy group
	IF EXISTS
	(
		SELECT * FROM ExePlantProductionEntry e
		inner join ExePlantProductionEntryVerification v on v.ProductionEntryCode = e.ProductionEntryCode
		WHERE e.EmployeeID = @empUpd_ID AND v.ProductionDate = @tmpProductionDate AND v.GroupCode = @dummyGroup
	)
	BEGIN
		DELETE e FROM ExePlantProductionEntry e
		inner join ExePlantProductionEntryVerification v on v.ProductionEntryCode = e.ProductionEntryCode
		WHERE e.EmployeeID = @empUpd_ID AND v.ProductionDate = @tmpProductionDate AND v.GroupCode = @dummyGroup
	END

	-- delete verification dummy if there is no entry dummy left
	DECLARE @countEnt INT;
	DECLARE @prodEntryCodeDummy VARCHAR(64);

	SELECT @countEnt = COUNT(*) FROM ExePlantProductionEntry e
	inner join ExePlantProductionEntryVerification v on v.ProductionEntryCode = e.ProductionEntryCode
	WHERE v.ProductionDate = @tmpProductionDate AND v.GroupCode = @dummyGroup

	IF(@countEnt = 0)
	BEGIN
		SELECT TOP 1 @prodEntryCodeDummy = v.ProductionEntryCode 
		FROM ExePlantProductionEntryVerification v
		WHERE v.ProductionDate = @tmpProductionDate AND v.GroupCode = @dummyGroup

		IF(@countEnt = 0)
		BEGIN
			DELETE ExePlantProductionEntryVerification
			WHERE ProductionEntryCode = @prodEntryCodeDummy;
		END
	END

END