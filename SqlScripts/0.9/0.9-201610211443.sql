/****** Object:  StoredProcedure [dbo].[EMPJOBDATA_SCHEDULER_V2]    Script Date: 10/21/2016 2:42:54 PM ******/
IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[dbo].[EMPJOBDATA_SCHEDULER_V2]'))
	DROP PROCEDURE [dbo].[EMPJOBDATA_SCHEDULER_V2]
GO

/****** Object:  StoredProcedure [dbo].[EMPJOBDATA_SCHEDULER_V2]    Script Date: 10/21/2016 2:42:54 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[EMPJOBDATA_SCHEDULER_V2]
AS
BEGIN

BEGIN TRANSACTION trans1
BEGIN TRY
	DECLARE @CurrentHour INT
	SET @CurrentHour = DATEPART(HOUR, GETDATE())
	IF @CurrentHour >= 0 AND @CurrentHour <=3
	BEGIN
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
	
		-- Cursor Loop tmp ACV
		DECLARE cursor_tmpACV CURSOR LOCAL FOR
		select EmployeeID, EmployeeNumber, EmployeeName, LocationCode, UnitCode, GroupCode, ProcessSettingsCode, Status 
		from MstPlantEmpJobsDataAcvTemp t where not exists(select * from MstPlantEmpJobsDataAcv where EmployeeID = t.employeeid)
		union all
		select a.EmployeeID, a.EmployeeNumber,  a.EmployeeName,  a.LocationCode,  a.UnitCode,  a.GroupCode,  a.ProcessSettingsCode,  a.Status 
		from MstPlantEmpJobsDataAcvtemp t inner join MstPlantEmpJobsDataAcv a on a.EmployeeID = t.employeeid
		where a.EmployeeNumber <> t.employeenumber
	
		OPEN cursor_tmpACV
	
		FETCH NEXT FROM cursor_tmpACV   
		INTO @empUpd_ID, @empUpd_Number, @empUpd_Name, @empUpd_LocationCode, 
		@empUpd_UnitCode, @empUpd_GroupCode, @empUpd_ProcessSettingsCode, @empUpd_Status
	
		WHILE @@FETCH_STATUS = 0  
		BEGIN
		
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
			FROM MstPlantEmpJobsDataAcvTemp  WHERE EmployeeID = @empUpd_ID 
	
			-- Get Effective Date
			SELECT @empUpd_EffDate = MAX(CASE WHEN LAST_UPDATE_DATE >= LAST_UPD_NO_PEN THEN EFFDT
			WHEN LAST_UPD_NO_PEN > LAST_UPDATE_DATE THEN EFFDT_NO_PEN END) FROM MSTEMPPSFT WHERE EMPLOYEE_ID = @empUpd_ID
	
			-- Get Active Status
			SELECT TOP 1 @empUpd_AcvSts = CASE WHEN ACS_STS = 'A' THEN 1 ELSE 0 END 
			FROM MSTEMPPSFT WHERE EMPLOYEE_ID = @empUpd_ID AND CASE WHEN LAST_UPDATE_DATE >= LAST_UPD_NO_PEN THEN EFFDT
			WHEN LAST_UPD_NO_PEN > LAST_UPDATE_DATE THEN EFFDT_NO_PEN END = @empUpd_EffDate
	
			IF(@empUpd_AcvSts = 1)
			BEGIN
				-- Update Absenteeism and Entry Absenteeism
				IF EXISTS 
				(
					SELECT * FROM ExePlantWorkerAbsenteeism WHERE EmployeeID = @empUpd_ID AND 
					(@empUpd_EffDate < StartDateAbsent OR @empUpd_EffDate BETWEEN StartDateAbsent AND EndDateAbsent)
				)
				BEGIN
					EXEC [EMPJOBDATA_SCHEDULER_ABSENTEEISM_ACV_V2] @empUpd_ID, @empUpd_Number, @empUpd_EffDate, @empUpd_LocationCode, 
					@empUpd_GroupCode, @empUpd_UnitCode, @empUpd_ProcessSettingsCode, @empUpd_Status
				END
				-- Update Assignment and Entry Assignment
				IF EXISTS
				(
					SELECT * FROM ExePlantWorkerAssignment WHERE EmployeeID = @empUpd_ID AND
					(@empUpd_EffDate < StartDate OR @empUpd_EffDate BETWEEN StartDate AND EndDate)
				)
				BEGIN
					EXEC [EMPJOBDATA_SCHEDULER_ASSIGNMENT_ACV_V2] @empUpd_ID, @empUpd_Number, @empUpd_EffDate, @empUpd_LocationCode, 
					@empUpd_GroupCode, @empUpd_UnitCode, @empUpd_ProcessSettingsCode, @empUpd_Status
				END
			
				EXEC [EMPJOBDATA_SCHEDULER_ENTRY_EMPTY_ACV_V2] @empUpd_ID, @empUpd_Number, @empUpd_EffDate, @empUpd_LocationCode, 
				@empUpd_GroupCode, @empUpd_UnitCode, @empUpd_ProcessSettingsCode, @empUpd_Status
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
	
			FETCH NEXT FROM cursor_tmpACV   
			INTO @empUpd_ID, @empUpd_Number, @empUpd_Name, @empUpd_LocationCode, 
			@empUpd_UnitCode, @empUpd_GroupCode, @empUpd_ProcessSettingsCode, @empUpd_Status
		END  
	
		CLOSE cursor_tmpACV;  
		DEALLOCATE cursor_tmpACV;  
END

COMMIT TRANSACTION trans1
END TRY
BEGIN CATCH
	ROLLBACK TRANSACTION trans1
	DECLARE @ErrorMessage   NVARCHAR(1000) = ERROR_MESSAGE(),
			@ErrorState     INT = ERROR_STATE(),
			@ErrorSeverity  INT = ERROR_SEVERITY();

	RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
END CATCH

END
GO