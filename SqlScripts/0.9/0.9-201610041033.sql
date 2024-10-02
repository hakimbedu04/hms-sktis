IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[dbo].[EMPJOBDATA_SCHEDULER_ASSIGNMENT_ENTRY_STS_4_ACV]'))
	DROP PROCEDURE [dbo].[EMPJOBDATA_SCHEDULER_ASSIGNMENT_ENTRY_STS_4_ACV]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[EMPJOBDATA_SCHEDULER_ASSIGNMENT_ENTRY_STS_4_ACV]
(
	@empUpd_ID VARCHAR(64),
	@tmpProductionDate DATETIME
)
AS
BEGIN
	SET DATEFIRST 1;
	
	DECLARE @absentType VARCHAR(128);
	SET @absentType = 'Multiskill Out';

	IF EXISTS
	(
		SELECT * FROM ExePlantProductionEntry e
		inner join ExePlantProductionEntryVerification v on v.ProductionEntryCode = e.ProductionEntryCode
		WHERE e.EmployeeID = @empUpd_ID AND v.ProductionDate = @tmpProductionDate AND AbsentType = @absentType
	)
	BEGIN
		DELETE e
		FROM ExePlantProductionEntry e
		inner join ExePlantProductionEntryVerification v on v.ProductionEntryCode = e.ProductionEntryCode
		WHERE e.EmployeeID = @empUpd_ID AND v.ProductionDate = @tmpProductionDate AND AbsentType = @absentType
	END
END;