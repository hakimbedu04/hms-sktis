IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[dbo].[EMPJOBDATA_SCHEDULER_ENTRY_EMPTY_NOT_ACV]'))
	DROP PROCEDURE [dbo].[EMPJOBDATA_SCHEDULER_ENTRY_EMPTY_NOT_ACV]
GO
/****** Object:  StoredProcedure [dbo].[EMPJOBDATA_SCHEDULER_ENTRY_EMPTY_NOT_ACV]    Script Date: 9/28/2016 4:56:09 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[EMPJOBDATA_SCHEDULER_ENTRY_EMPTY_NOT_ACV]
(
	@empUpd_ID VARCHAR(64),
	@empUpd_EffDate DATETIME
)
AS
BEGIN
	IF EXISTS
	(
		SELECT * FROM ExePlantProductionEntry e
		inner join ExePlantProductionEntryVerification v on v.ProductionEntryCode = e.ProductionEntryCode
		WHERE e.EmployeeID = @empUpd_ID AND v.ProductionDate >= @empUpd_EffDate AND e.AbsentType IS NULL
	)
	BEGIN
		DELETE e
		FROM ExePlantProductionEntry e
		inner join ExePlantProductionEntryVerification v on v.ProductionEntryCode = e.ProductionEntryCode
		WHERE e.EmployeeID = @empUpd_ID AND v.ProductionDate >= @empUpd_EffDate AND e.AbsentType IS NULL
	END
END