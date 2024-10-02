/****** Object:  StoredProcedure [dbo].[EMPJOBDATA_SCHEDULER_ICAPACITY_V2]    Script Date: 10/21/2016 2:46:10 PM ******/
IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[dbo].[EMPJOBDATA_SCHEDULER_ICAPACITY_V2]'))
	DROP PROCEDURE [dbo].[EMPJOBDATA_SCHEDULER_ICAPACITY_V2]
GO

/****** Object:  StoredProcedure [dbo].[EMPJOBDATA_SCHEDULER_ICAPACITY_V2]    Script Date: 10/21/2016 2:46:10 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[EMPJOBDATA_SCHEDULER_ICAPACITY_V2]
AS
BEGIN

BEGIN TRANSACTION trans1
BEGIN TRY
DECLARE @CurrentHour INT
SET @CurrentHour = DATEPART(HOUR, GETDATE())
IF @CurrentHour >= 0 AND @CurrentHour <=3
BEGIN
	SET DATEFIRST 1;

	DECLARE @empUpd_ID VARCHAR(64);
	DECLARE @empUpd_LocationCode VARCHAR(8);
	DECLARE @empUpd_GroupCode VARCHAR(4);
	DECLARE @empUpd_UnitCode VARCHAR(4);
	DECLARE @empUpd_ProcessSettingsCode VARCHAR(16);
	DECLARE @empUpd_AcvSts INT;

	-- Cursor Loop MstPlantEmpUpd
	DECLARE cursor_EmpUpd CURSOR LOCAL FOR
	select EmployeeID, LocationCode, UnitCode, GroupCode, ProcessSettingsCode 
	from MstPlantEmpJobsDataAcvTemp t where not exists(select * from MstPlantEmpJobsDataAcv where EmployeeID = t.employeeid) AND t.Status = '5'
	union all
	select a.EmployeeID, a.LocationCode,  a.UnitCode,  a.GroupCode,  a.ProcessSettingsCode
	from MstPlantEmpJobsDataAcvtemp t inner join MstPlantEmpJobsDataAcv a on a.EmployeeID = t.employeeid
	where a.EmployeeNumber <> t.employeenumber AND a.Status = '5'
	
	OPEN cursor_EmpUpd

	FETCH NEXT FROM cursor_EmpUpd   
	INTO @empUpd_ID, @empUpd_LocationCode, @empUpd_UnitCode, @empUpd_GroupCode, @empUpd_ProcessSettingsCode

	WHILE @@FETCH_STATUS = 0  
	BEGIN  
		-- Get Effective Date
		DECLARE @empUpd_EffDate DATETIME;
		SELECT @empUpd_EffDate = MAX(CASE WHEN LAST_UPDATE_DATE >= LAST_UPD_NO_PEN THEN EFFDT
		WHEN LAST_UPD_NO_PEN > LAST_UPDATE_DATE THEN EFFDT_NO_PEN END) FROM MSTEMPPSFT WHERE EMPLOYEE_ID = @empUpd_ID
		
		SELECT TOP 1 @empUpd_AcvSts = CASE WHEN ACS_STS = 'A' THEN 1 ELSE 0 END FROM MSTEMPPSFT WHERE EMPLOYEE_ID = @empUpd_ID
		AND CASE WHEN LAST_UPDATE_DATE >= LAST_UPD_NO_PEN THEN EFFDT
			WHEN LAST_UPD_NO_PEN > LAST_UPDATE_DATE THEN EFFDT_NO_PEN END = @empUpd_EffDate

		IF EXISTS 
		(
			SELECT * FROM [PlanPlantIndividualCapacityWorkHours] WHERE EmployeeID = @empUpd_ID AND GroupCode = @empUpd_GroupCode
			AND UnitCode = @empUpd_UnitCode AND LocationCode = @empUpd_LocationCode AND ProcessGroup = @empUpd_ProcessSettingsCode
		)
		BEGIN
			UPDATE [PlanPlantIndividualCapacityWorkHours]
			SET
				GroupCode = @empUpd_GroupCode,
				UnitCode = @empUpd_UnitCode,
				LocationCode = @empUpd_LocationCode,
				StatusActive = @empUpd_AcvSts
			WHERE EmployeeID = @empUpd_ID AND GroupCode = @empUpd_GroupCode
			AND UnitCode = @empUpd_UnitCode AND LocationCode = @empUpd_LocationCode AND ProcessGroup = @empUpd_ProcessSettingsCode
		END
		ELSE
		BEGIN
			DECLARE @kpsYear INT;
			DECLARE @kpsWeek INT;

			SELECT TOP 1 @kpsYear = [Year], @kpsWeek = [Week] FROM MstGenWeek WHERE CONVERT(DATE, GETDATE()) BETWEEN StartDate AND EndDate;

			DECLARE @tmpBrand TABLE (BrandCode VARCHAR(11));
			INSERT INTO @tmpBrand
			SELECT DISTINCT BrandCode FROM [dbo].[PlanPlantTargetProductionKelompok] WHERE LocationCode = @empUpd_LocationCode AND UnitCode = @empUpd_UnitCode
			AND KPSYear = @kpsYear AND KPSWeek = @kpsWeek

			DECLARE @brandCode VARCHAR(11);

			DECLARE cursorTmpBrand CURSOR LOCAL FOR
			SELECT BrandCode
			FROM @tmpBrand

			OPEN cursorTmpBrand

			FETCH NEXT FROM cursorTmpBrand   
			INTO @brandCode

			WHILE @@FETCH_STATUS = 0  
			BEGIN
				DECLARE @brandGroupCode VARCHAR(20);
				SELECT @brandGroupCode = BrandGroupCode FROM MstGenBrand WHERE BrandCode = @brandCode

				IF NOT EXISTS
				(
					SELECT * FROM [PlanPlantIndividualCapacityWorkHours] WHERE EmployeeID = @empUpd_ID AND GroupCode = @empUpd_GroupCode
					AND UnitCode = @empUpd_UnitCode AND LocationCode = @empUpd_LocationCode AND ProcessGroup = @empUpd_ProcessSettingsCode
					AND BrandGroupCode = @brandGroupCode
				)
				BEGIN
					DECLARE @hoursCap3 DECIMAL(18,3),
							@hoursCap5 DECIMAL(18,3),
							@hoursCap6 DECIMAL(18,3),
							@hoursCap7 DECIMAL(18,3),
							@hoursCap8 DECIMAL(18,3),
							@hoursCap9 DECIMAL(18,3),
							@hoursCap10 DECIMAL(18,3)

					SELECT 
						@hoursCap3 = CASE WHEN UOMEblek = 0 THEN 0 ELSE MinStickPerHour * 3 / UomEblek END,
						@hoursCap5 = CASE WHEN UOMEblek = 0 THEN 0 ELSE MinStickPerHour * 5 / UomEblek END,
						@hoursCap6 = CASE WHEN UOMEblek = 0 THEN 0 ELSE MinStickPerHour * 6 / UomEblek END,
						@hoursCap7 = CASE WHEN UOMEblek = 0 THEN 0 ELSE MinStickPerHour * 7 / UomEblek END,
						@hoursCap8 = CASE WHEN UOMEblek = 0 THEN 0 ELSE MinStickPerHour * 8 / UomEblek END,
						@hoursCap9 = CASE WHEN UOMEblek = 0 THEN 0 ELSE MinStickPerHour * 9 / UomEblek END,
						@hoursCap10 = CASE WHEN UOMEblek = 0 THEN 0 ELSE MinStickPerHour * 10 / UomEblek END
					FROM ProcessSettingsAndLocationView WHERE BrandGroupCode = @brandGroupCode AND ProcessGroup = @empUpd_ProcessSettingsCode
					AND LocationCode = @empUpd_LocationCode

					INSERT INTO [dbo].[PlanPlantIndividualCapacityWorkHours]
						   ([BrandGroupCode]
						   ,[EmployeeID]
						   ,[GroupCode]
						   ,[UnitCode]
						   ,[LocationCode]
						   ,[ProcessGroup]
						   ,[HoursCapacity3]
						   ,[HoursCapacity5]
						   ,[HoursCapacity6]
						   ,[HoursCapacity7]
						   ,[HoursCapacity8]
						   ,[HoursCapacity9]
						   ,[HoursCapacity10]
						   ,[StatusActive]
						   ,[CreatedDate]
						   ,[CreatedBy]
						   ,[UpdatedDate]
						   ,[UpdatedBy])
					 VALUES
						   (@brandGroupCode
						   ,@empUpd_ID
						   ,@empUpd_GroupCode
						   ,@empUpd_UnitCode
						   ,@empUpd_LocationCode
						   ,@empUpd_ProcessSettingsCode
						   ,@hoursCap3
						   ,@hoursCap5
						   ,@hoursCap6
						   ,@hoursCap7
						   ,@hoursCap8
						   ,@hoursCap9
						   ,@hoursCap10
						   ,@empUpd_AcvSts
						   ,GETDATE()
						   ,'SYSTEM'
						   ,GETDATE()
						   ,'SYSTEM')
				END

				FETCH NEXT FROM cursorTmpBrand   
				INTO @brandCode
			END

			CLOSE cursorTmpBrand;  
			DEALLOCATE cursorTmpBrand; 
		END

		---------------	
		FETCH NEXT FROM cursor_EmpUpd   
		INTO @empUpd_ID, @empUpd_LocationCode, @empUpd_UnitCode, @empUpd_GroupCode, @empUpd_ProcessSettingsCode
	END;

	CLOSE cursor_EmpUpd;  
	DEALLOCATE cursor_EmpUpd; 
END
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
GO