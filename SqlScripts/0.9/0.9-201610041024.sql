IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[dbo].[EMPJOBDATA_SCHEDULER_V1]'))
	DROP PROCEDURE [dbo].[EMPJOBDATA_SCHEDULER_V1]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[EMPJOBDATA_SCHEDULER_V1]
AS
BEGIN

BEGIN TRANSACTION trans1
BEGIN TRY

	SET DATEFIRST 1;

	DECLARE @empUpd_ID VARCHAR(64);
	DECLARE @empUpd_Number VARCHAR(6);
	DECLARE @empUpd_Name VARCHAR(64);
	DECLARE @empUpd_EffDate DATETIME;
	DECLARE @empUpd_LocationCode VARCHAR(8);
	DECLARE @empUpd_GroupCode VARCHAR(4);
	DECLARE @empUpd_UnitCode VARCHAR(4);
	DECLARE @empUpd_ProcessSettingsCode VARCHAR(16);
	DECLARE @empUpd_AcvSts INT;
	DECLARE @empUpd_Status VARCHAR(64);

	-- Cursor Loop MstPlantEmpUpd
	DECLARE cursor_EmpUpd CURSOR LOCAL FOR
	SELECT EmployeeID, EmployeeNumber, Name, EffDate, LocationCode, UnitCode, GroupCode, ProcessSettingsCode, AcvSts, Status
	FROM MstPlantEmpUpd

	OPEN cursor_EmpUpd

	FETCH NEXT FROM cursor_EmpUpd   
	INTO @empUpd_ID, @empUpd_Number, @empUpd_Name, @empUpd_EffDate, @empUpd_LocationCode, 
	@empUpd_UnitCode, @empUpd_GroupCode, @empUpd_ProcessSettingsCode, @empUpd_AcvSts, @empUpd_Status

	WHILE @@FETCH_STATUS = 0  
	BEGIN  
		SET @empUpd_EffDate = CONVERT(DATE, @empUpd_EffDate);

		DECLARE @oldLocationCode VARCHAR(8);
		DECLARE @oldUnitCode VARCHAR(4);
		DECLARE @oldGroupCode VARCHAR(4);
		DECLARE @oldProcess VARCHAR(16);
		DEClARE @oldEmpNumber VARCHAR(6);
		DECLARE @oldStatus VARCHAR(64);
		
		DECLARE @acStatus BIT;
		SET @acStatus = 1;

		SELECT @oldLocationCode = LocationCode, @oldUnitCode = UnitCode, @oldGroupCode = GroupCode, @oldProcess = ProcessSettingsCode,
		@oldEmpNumber = EmployeeNumber, @oldStatus = Status
		FROM MstPlantEmpJobsDataAcv  WHERE EmployeeID = @empUpd_ID 

		IF NOT EXISTS
		(
			SELECT * FROM MstPlantEmpJobsDataAcv WHERE EmployeeID = @empUpd_ID
		)
		OR
		(
			@oldLocationCode <> @empUpd_LocationCode OR @oldUnitCode <> @empUpd_UnitCode OR @oldGroupCode <> @empUpd_GroupCode OR @oldProcess <> @empUpd_ProcessSettingsCode
			OR @oldEmpNumber <> @empUpd_Number OR @oldStatus <> @empUpd_Status OR @acStatus <> @empUpd_AcvSts
		)
		BEGIN
			IF(@empUpd_AcvSts = 1)
			BEGIN
				-- Update Absenteeism and Entry Absenteeism
				IF EXISTS 
				(
					SELECT * FROM ExePlantWorkerAbsenteeism WHERE EmployeeID = @empUpd_ID AND 
					(@empUpd_EffDate < StartDateAbsent OR @empUpd_EffDate BETWEEN StartDateAbsent AND EndDateAbsent)
				)
				BEGIN
					EXEC [EMPJOBDATA_SCHEDULER_ABSENTEEISM_ACV] @empUpd_ID, @empUpd_Number, @empUpd_EffDate, @empUpd_LocationCode, 
					@empUpd_GroupCode, @empUpd_UnitCode, @empUpd_ProcessSettingsCode, @empUpd_Status
				END
				-- Update Assignment and Entry Assignment
				IF EXISTS
				(
					SELECT * FROM ExePlantWorkerAssignment WHERE EmployeeID = @empUpd_ID AND
					(@empUpd_EffDate < StartDate OR @empUpd_EffDate BETWEEN StartDate AND EndDate)
				)
				BEGIN
					EXEC [EMPJOBDATA_SCHEDULER_ASSIGNMENT_ACV] @empUpd_ID, @empUpd_Number, @empUpd_EffDate, @empUpd_LocationCode, 
					@empUpd_GroupCode, @empUpd_UnitCode, @empUpd_ProcessSettingsCode, @empUpd_Status
				END
				-- Update Entry
				--IF EXISTS
				--(
				--	SELECT * FROM ExePlantProductionEntry e 
				--	inner join ExePlantProductionEntryVerification v on v.ProductionEntryCode = e.ProductionEntryCode
				--	WHERE e.EmployeeID = @empUpd_ID AND v.ProductionDate >= @empUpd_EffDate AND e.AbsentType IS NULL
				--)
				--BEGIN
					EXEC [EMPJOBDATA_SCHEDULER_ENTRY_EMPTY_ACV] @empUpd_ID, @empUpd_Number, @empUpd_EffDate, @empUpd_LocationCode, 
					@empUpd_GroupCode, @empUpd_UnitCode, @empUpd_ProcessSettingsCode, @empUpd_Status
				--END
			END
			ELSE IF(@empUpd_AcvSts = 0)
			BEGIN
				-- Update Absenteeism and Entry Absenteeism
				IF EXISTS 
				(
					SELECT * FROM ExePlantWorkerAbsenteeism WHERE EmployeeID = @empUpd_ID AND 
					(@empUpd_EffDate < StartDateAbsent OR @empUpd_EffDate BETWEEN StartDateAbsent AND EndDateAbsent)
				)
				BEGIN
					EXEC [EMPJOBDATA_SCHEDULER_ABSENTEEISM_NOT_ACV] @empUpd_ID, @empUpd_Number, @empUpd_EffDate, @empUpd_LocationCode, 
					@empUpd_GroupCode, @empUpd_UnitCode, @empUpd_ProcessSettingsCode
				END
				-- Update Assignment and Entry Assignment
				IF EXISTS
				(
					SELECT * FROM ExePlantWorkerAssignment WHERE EmployeeID = @empUpd_ID AND
					(@empUpd_EffDate < StartDate OR @empUpd_EffDate BETWEEN StartDate AND EndDate)
				)
				BEGIN
					EXEC EMPJOBDATA_SCHEDULER_ASSIGNMENT_NOT_ACV @empUpd_ID, @empUpd_Number, @empUpd_EffDate, @empUpd_LocationCode, 
					 @empUpd_GroupCode, @empUpd_UnitCode, @empUpd_ProcessSettingsCode
				END
				-- Update Entry
				IF EXISTS
				(
					SELECT * FROM ExePlantProductionEntry e 
					inner join ExePlantProductionEntryVerification v on v.ProductionEntryCode = e.ProductionEntryCode
					WHERE e.EmployeeID = @empUpd_ID AND v.ProductionDate >= @empUpd_EffDate AND e.AbsentType IS NULL
				)
				BEGIN
					EXEC [EMPJOBDATA_SCHEDULER_ENTRY_EMPTY_NOT_ACV] @empUpd_ID, @empUpd_EffDate
				END
			END

			-- Update Production Card
			EXEC [EMPJOBDATA_SCHEDULER_PRODUCTIONCARD] @empUpd_ID, @empUpd_Number, @empUpd_EffDate, @empUpd_LocationCode, 
			 @empUpd_GroupCode, @empUpd_UnitCode, @empUpd_ProcessSettingsCode, @empUpd_Status, @empUpd_AcvSts
		END

		---------------	
		FETCH NEXT FROM cursor_EmpUpd   
		INTO @empUpd_ID, @empUpd_Number, @empUpd_Name, @empUpd_EffDate, @empUpd_LocationCode, 
		@empUpd_UnitCode, @empUpd_GroupCode, @empUpd_ProcessSettingsCode, @empUpd_AcvSts, @empUpd_Status
	END;

	CLOSE cursor_EmpUpd;  
	DEALLOCATE cursor_EmpUpd;  

	-- EXEC [EMPJOBDATA_SCHEDULER_UPDATE_ACV];
	-- EXEC [EMPJOBDATA_SCHEDULER_UPDATE_ALL];
	
	-- EXEC [EMPJOBDATA_SCHEDULER_UPDATE_MSTPLANTPRODUCTIONGROUP];

	-- EXEC [RunMstPlantEmpJobsDataAcv2]
	-- EXEC [RunMstPlantEmpJobsDataAll2]

	-- EXEC [EMPJOBDATA_SCHEDULER_UPDATE_MSTPLANTPRODUCTIONGROUP];

	-- EXEC [EMPJOBDATA_SCHEDULER_COPYTOUPDTMP]

COMMIT TRANSACTION trans1
END TRY
BEGIN CATCH
	ROLLBACK TRANSACTION trans1
	IF (@@TRANCOUNT > 0) ROLLBACK TRANSACTION
	DECLARE @ErrorMessage   NVARCHAR(1000) = ERROR_MESSAGE(),
			@ErrorState     INT = ERROR_STATE(),
			@ErrorSeverity  INT = ERROR_SEVERITY();

	RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
END CATCH

END