IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[dbo].[EMPJOBDATA_SCHEDULER_ABSENTEEISM_ACV]'))
	DROP PROCEDURE [dbo].[EMPJOBDATA_SCHEDULER_ABSENTEEISM_ACV]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[EMPJOBDATA_SCHEDULER_ABSENTEEISM_ACV]
(
	@empUpd_ID VARCHAR(64),
	@empUpd_Number VARCHAR(6),
	@empUpd_EffDate DATETIME,
	@empUpd_LocationCode VARCHAR(8),
	@empUpd_GroupCode VARCHAR(4),
	@empUpd_UnitCode VARCHAR(4),
	@empUpd_ProcessSettingsCode VARCHAR(16),
	@empUpd_Status VARCHAR(64)
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
		--1. START_DATE == END_DATE
		--2. START_DATE < END_DATE
		--2.1 START_DATE > EFFECTIVE_DATE [UPDATE LANGSUNG]
		--2.2 EFF_DATE BETWEEN START_DATE AND END_DATE
		IF(@startDate = @endDate)  -- IF LUAR 1 date
		BEGIN
			IF(@startDate >= @empUpd_EffDate)
			BEGIN
				UPDATE ExePlantWorkerAbsenteeism 
				SET LocationCode = @empUpd_LocationCode,
				EmployeeNumber = @empUpd_Number,
				UnitCode = @empUpd_UnitCode,
				GroupCode = @empUpd_GroupCode,
				UpdatedDate = GETDATE(),
				UpdatedBy = 'System'
				WHERE EmployeeID = @empUpd_ID AND StartDateAbsent = @startDate;
			END
		END
		ELSE IF(@startDate < @endDate) -- IF luar range
		BEGIN
			IF(@startDate > @empUpd_EffDate)
			BEGIN
				UPDATE ExePlantWorkerAbsenteeism 
				SET LocationCode = @empUpd_LocationCode,
				EmployeeNumber = @empUpd_Number,
				UnitCode = @empUpd_UnitCode,
				GroupCode = @empUpd_GroupCode,
				UpdatedDate = GETDATE(),
				UpdatedBy = 'System'
				WHERE EmployeeID = @empUpd_ID AND StartDateAbsent = @startDate;
			END
			ELSE IF(@empUpd_EffDate BETWEEN @startDate AND @endDate)
			BEGIN
				IF(@empUpd_EffDate = @startDate)
				BEGIN
					UPDATE ExePlantWorkerAbsenteeism 
					SET LocationCode = @empUpd_LocationCode,
					EmployeeNumber = @empUpd_Number,
					UnitCode = @empUpd_UnitCode,
					GroupCode = @empUpd_GroupCode,
					UpdatedDate = GETDATE(),
					UpdatedBy = 'System'
					WHERE EmployeeID = @empUpd_ID AND StartDateAbsent = @startDate;
				END
				ELSE
				BEGIN
					INSERT INTO [dbo].[ExePlantWorkerAbsenteeism]
						([StartDateAbsent]
						,[EmployeeID]
						,[AbsentType]
						,[EndDateAbsent]
						,[SktAbsentCode]
						,[PayrollAbsentCode]
						,[ePaf]
						,[Attachment]
						,[AttachmentPath]
						,[CreatedDate]
						,[CreatedBy]
						,[UpdatedDate]
						,[UpdatedBy]
						,[EmployeeNumber]
						,[LocationCode]
						,[UnitCode]
						,[GroupCode]
						,[TransactionDate]
						,[Shift])
					SELECT 
						@empUpd_EffDate as [StartDateAbsent]
						,[EmployeeID]
						,[AbsentType]
						,[EndDateAbsent]
						,[SktAbsentCode]
						,[PayrollAbsentCode]
						,[ePaf]
						,[Attachment]
						,[AttachmentPath]
						,[CreatedDate]
						,[CreatedBy]
						,GETDATE() as [UpdatedDate]
						,'System' as [UpdatedBy]
						,@empUpd_Number as [EmployeeNumber]
						,@empUpd_LocationCode as [LocationCode]
						,@empUpd_UnitCode as [UnitCode]
						,@empUpd_GroupCode as [GroupCode]
						,[TransactionDate]
						,[Shift]
					FROM ExePlantWorkerAbsenteeism
					WHERE EmployeeID = @empUpd_ID AND StartDateAbsent = @startDate;

					UPDATE ExePlantWorkerAbsenteeism
					SET
						EndDateAbsent = DATEADD(DAY, -1, @empUpd_EffDate),
						UpdatedDate = GETDATE(),
						UpdatedBy = 'System'
					WHERE EmployeeID = @empUpd_ID AND StartDateAbsent = @startDate;
				END
			END
		END

		-- Fixing Entry Absenteeism
		DECLARE @tmpEffDate DATETIME;
		DECLARE @tmpEndDate DATETIME;
		SET @tmpEffDate = @startDate;

		DECLARE @endDateCurrWeekSaturday DATETIME;
		SELECT @endDateCurrWeekSaturday = DATEADD(DAY, -1, EndDate) 
		FROM MstGenWeek WHERE CONVERT(DATE, GETDATE()) BETWEEN StartDate AND EndDate;

		IF(@endDate > @endDateCurrWeekSaturday)
		BEGIN
			SET @tmpEndDate = @endDateCurrWeekSaturday;
		END
		ELSE
		BEGIN
			SET @tmpEndDate = @endDate;
		END

		IF(@startDate < @empUpd_EffDate)
		BEGIN
			SET @tmpEffDate = @empUpd_EffDate
		END

		IF(@empUpd_Status = '5')
		BEGIN
			WHILE(@tmpEffDate <= @tmpEndDate)
			BEGIN
				DECLARE @dayNumber INT;
				DECLARE @dayNumberSunday INT;
				SET @dayNumberSunday = 7;
				SET @dayNumber = DATEPART(DW, @tmpEffDate);
				IF NOT EXISTS(SELECT * FROM MstGenHoliday WHERE HolidayDate = @tmpEffDate)
				BEGIN
					IF(@dayNumber <> @dayNumberSunday)
					BEGIN
						EXEC [EMPJOBDATA_SCHEDULER_ABSENTEEISM_ENTRY_ACV] 
						@empUpd_ID, @empUpd_Number, @empUpd_EffDate,  @empUpd_LocationCode,  
						@empUpd_GroupCode, @empUpd_UnitCode, @empUpd_ProcessSettingsCode, @absentType, @tmpEffDate
					END
				END
				SET @tmpEffDate = DATEADD(DAY, 1, @tmpEffDate);	  
			END													 
		END
		ELSE
		BEGIN
			WHILE(@tmpEffDate <= @tmpEndDate)
			BEGIN
				DELETE e
				FROM ExePlantProductionEntry e 
				inner join ExePlantProductionEntryVerification v on v.ProductionEntryCode = e.ProductionEntryCode
				WHERE e.EmployeeID = @empUpd_ID AND v.ProductionDate = @tmpEffDate AND e.AbsentType = @absentType

				SET @tmpEffDate = DATEADD(DAY, 1, @tmpEffDate);	  
			END		
		END
												 
		FETCH NEXT FROM cursor_absenteeism					
		INTO @startDate, @endDate, @absentType				
	END;													 
															 
	CLOSE cursor_absenteeism ;
	DEALLOCATE cursor_absenteeism;
END