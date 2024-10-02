IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[dbo].[EMPJOBDATA_SCHEDULER_PRODUCTIONCARD]'))
	DROP PROCEDURE [dbo].[EMPJOBDATA_SCHEDULER_PRODUCTIONCARD]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[EMPJOBDATA_SCHEDULER_PRODUCTIONCARD]
(
	@empUpd_ID VARCHAR(64),
	@empUpd_Number VARCHAR(6),
	@empUpd_EffDate DATETIME,
	@empUpd_LocationCode VARCHAR(8),
	@empUpd_GroupCode VARCHAR(4),
	@empUpd_UnitCode VARCHAR(4),
	@empUpd_ProcessSettingsCode VARCHAR(16),
	@empUpd_Status VARCHAR(64),
	@empUpd_Acv BIT
)
AS
BEGIN
	IF(@empUpd_Acv = 1)
	BEGIN
		IF(@empUpd_Status = '5')
		BEGIN
			IF EXISTS 
			(
				SELECT * FROM ProductionCard WHERE EmployeeID = @empUpd_ID AND LocationCode = @empUpd_LocationCode AND UnitCode = @empUpd_UnitCode
				AND GroupCode = @empUpd_GroupCode AND ProcessGroup = @empUpd_ProcessSettingsCode AND ProductionDate >= @empUpd_EffDate
			)
			BEGIN
				UPDATE ProductionCard
				SET 
					LocationCode = @empUpd_LocationCode,
					UnitCode = @empUpd_UnitCode,
					ProcessGroup = @empUpd_ProcessSettingsCode,
					GroupCode = @empUpd_GroupCode,
					EmployeeNumber = @empUpd_Number
				WHERE EmployeeID = @empUpd_ID AND LocationCode = @empUpd_LocationCode AND UnitCode = @empUpd_UnitCode
				AND GroupCode = @empUpd_GroupCode AND ProcessGroup = @empUpd_ProcessSettingsCode AND ProductionDate >= @empUpd_EffDate
			END
		END
		ELSE IF(@empUpd_Status = '4')
		BEGIN
			DELETE ProductionCard
			WHERE EmployeeID = @empUpd_ID AND LocationCode = @empUpd_LocationCode AND UnitCode = @empUpd_UnitCode
			AND GroupCode = @empUpd_GroupCode AND ProcessGroup = @empUpd_ProcessSettingsCode AND ProductionDate >= @empUpd_EffDate
		END
	END
	ELSE
	BEGIN
		DELETE ProductionCard
		WHERE EmployeeID = @empUpd_ID AND LocationCode = @empUpd_LocationCode AND UnitCode = @empUpd_UnitCode
		AND GroupCode = @empUpd_GroupCode AND ProcessGroup = @empUpd_ProcessSettingsCode AND ProductionDate >= @empUpd_EffDate
	END
END