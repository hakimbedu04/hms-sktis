IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[dbo].[EMPJOBDATA_SCHEDULER_ABSENTEEISM_NOT_ACV]'))
	DROP PROCEDURE [dbo].[EMPJOBDATA_SCHEDULER_ABSENTEEISM_NOT_ACV]
GO
/****** Object:  StoredProcedure [dbo].[EMPJOBDATA_SCHEDULER_ABSENTEEISM_NOT_ACV]    Script Date: 9/28/2016 5:02:20 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[EMPJOBDATA_SCHEDULER_ABSENTEEISM_NOT_ACV]
(
	@empUpd_ID VARCHAR(64),
	@empUpd_Number VARCHAR(6),
	@empUpd_EffDate DATETIME,
	@empUpd_LocationCode VARCHAR(8),
	@empUpd_GroupCode VARCHAR(4),
	@empUpd_UnitCode VARCHAR(4),
	@empUpd_ProcessSettingsCode VARCHAR(16)
)
AS
BEGIN
	SET DATEFIRST 1;

	DECLARE @startDate DATETIME;
	DECLARE @endDate DATETIME;
	DECLARE @absentType VARCHAR(128);

	DECLARE cursor_absenteeism CURSOR FOR
	SELECT StartDateAbsent, EndDateAbsent, AbsentType FROM ExePlantWorkerAbsenteeism 
	WHERE EmployeeID = @empUpd_ID AND (@empUpd_EffDate < StartDateAbsent OR @empUpd_EffDate BETWEEN StartDateAbsent AND EndDateAbsent) 

	OPEN cursor_absenteeism
	
	FETCH NEXT FROM cursor_absenteeism
	INTO @startDate, @endDate, @absentType

	WHILE @@FETCH_STATUS = 0
	BEGIN
		IF(@startDate = @endDate)  -- IF LUAR 1 date
		BEGIN
			IF(@startDate >= @empUpd_EffDate)
			BEGIN
				DELETE ExePlantWorkerAbsenteeism
				WHERE EmployeeID = @empUpd_ID AND StartDateAbsent = @startDate;
			END
		END
		ELSE IF(@startDate < @endDate)
		BEGIN
			IF(@startDate > @empUpd_EffDate)
			BEGIN
				DELETE ExePlantWorkerAbsenteeism
				WHERE EmployeeID = @empUpd_ID AND StartDateAbsent = @startDate;
			END
			ELSE IF(@empUpd_EffDate BETWEEN @startDate AND @endDate)
			BEGIN
				IF(@empUpd_EffDate = @startDate)
				BEGIN
					DELETE ExePlantWorkerAbsenteeism
					WHERE EmployeeID = @empUpd_ID AND StartDateAbsent = @startDate;
				END
				ELSE
				BEGIN
					UPDATE ExePlantWorkerAbsenteeism
					SET
						EndDateAbsent = DATEADD(DAY, -1, @empUpd_EffDate),
						UpdatedDate = GETDATE(),
						UpdatedBy = 'System'
					WHERE EmployeeID = @empUpd_ID AND StartDateAbsent = @startDate;
				END
			END
		END
		ELSE IF(@startDate > @empUpd_EffDate)
		BEGIN
			DELETE ExePlantWorkerAbsenteeism
			WHERE EmployeeID = @empUpd_ID AND StartDateAbsent = @startDate;
		END

		-- Fixing Entry Absenteeism
		DECLARE @tmpEffDate DATETIME;
		SET @tmpEffDate = @empUpd_EffDate;

		WHILE(@tmpEffDate <= @endDate)
		BEGIN
			EXEC [EMPJOBDATA_SCHEDULER_ABSENTEEISM_ENTRY_NOT_ACV]
			@empUpd_ID, @empUpd_Number, @empUpd_EffDate,  @empUpd_LocationCode,  
			@empUpd_GroupCode, @empUpd_UnitCode, @empUpd_ProcessSettingsCode, @absentType, @tmpEffDate
		
			SET @tmpEffDate = DATEADD(DAY, 1, @tmpEffDate);	  
		END													 
												 
		FETCH NEXT FROM cursor_absenteeism					
		INTO @startDate, @endDate, @absentType				
	END;													 
															 
	CLOSE cursor_absenteeism ;
	DEALLOCATE cursor_absenteeism;
END