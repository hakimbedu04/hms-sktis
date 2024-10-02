IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[dbo].[EMPJOBDATA_SCHEDULER_ICAPACITY]'))
	DROP PROCEDURE [dbo].[EMPJOBDATA_SCHEDULER_ICAPACITY]
GO
/****** Object:  StoredProcedure [dbo].[EMPJOBDATA_SCHEDULER_V1]    Script Date: 10/13/2016 10:53:59 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[EMPJOBDATA_SCHEDULER_ICAPACITY]
AS
BEGIN

BEGIN TRANSACTION trans1
BEGIN TRY

	SET DATEFIRST 1;

	DECLARE @empUpd_ID VARCHAR(64);
	DECLARE @empUpd_LocationCode VARCHAR(8);
	DECLARE @empUpd_GroupCode VARCHAR(4);
	DECLARE @empUpd_UnitCode VARCHAR(4);
	DECLARE @empUpd_ProcessSettingsCode VARCHAR(16);
	DECLARE @empUpd_AcvSts INT;

	-- Cursor Loop MstPlantEmpUpd
	DECLARE cursor_EmpUpd CURSOR LOCAL FOR
	SELECT EmployeeID, LocationCode, UnitCode, GroupCode, ProcessSettingsCode, AcvSts
	FROM MstPlantEmpUpd WHERE Status = 5

	OPEN cursor_EmpUpd

	FETCH NEXT FROM cursor_EmpUpd   
	INTO @empUpd_ID, @empUpd_LocationCode, @empUpd_UnitCode, @empUpd_GroupCode, @empUpd_ProcessSettingsCode, @empUpd_AcvSts

	WHILE @@FETCH_STATUS = 0  
	BEGIN  
		
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
		INTO @empUpd_ID, @empUpd_LocationCode, @empUpd_UnitCode, @empUpd_GroupCode, @empUpd_ProcessSettingsCode, @empUpd_AcvSts
	END;

	CLOSE cursor_EmpUpd;  
	DEALLOCATE cursor_EmpUpd; 

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