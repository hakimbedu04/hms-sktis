SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


ALTER PROCEDURE [dbo].[EMPJOBDATA_SCHEDULER_ASSIGNMENT_ACV]
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
	DECLARE @destGroupCode VARCHAR(4);
	DECLARE @srcLocation VARCHAR(64);
	DECLARE @srcUnit VARCHAR(64);
	DECLARE @srcBrandCode VARCHAR(11);
	DECLARE @srcProcess VARCHAR(20);
	DECLARE @srcShift INT;
	DECLARE @srcGroupCode VARCHAR(20);
	DECLARE @dummyGroup VARCHAR(4);

	DECLARE cursor_assignment CURSOR FOR
	SELECT SourceLocationCode, SourceBrandCode, SourceGroupCode, SourceProcessGroup, SourceUnitCode, SourceShift,
			StartDate, EndDate, DestinationGroupCode, DestinationGroupCodeDummy
	FROM ExePlantWorkerAssignment 
	WHERE EmployeeID = @empUpd_ID AND (@empUpd_EffDate < StartDate OR @empUpd_EffDate BETWEEN StartDate AND EndDate) 

	OPEN cursor_assignment

	FETCH NEXT FROM cursor_assignment
	INTO @srcLocation, @srcBrandCode, @srcGroupCode, @srcProcess, @srcUnit, @srcShift, @startDate, @endDate, @destGroupCode, @dummyGroup

	WHILE @@FETCH_STATUS = 0
	BEGIN
		-- Fixing Assignment
		-- GroupUpd = Dest Group
		IF(@destGroupCode = @empUpd_GroupCode)
		BEGIN
			IF(@startDate = @endDate)  -- IF LUAR 1 date
			BEGIN
				IF(@startDate >= @empUpd_EffDate)
				BEGIN
					DELETE ExePlantWorkerAssignment
					WHERE EmployeeID = @empUpd_ID AND StartDate = @startDate;
				END
			END
			ELSE IF(@startDate < @endDate)
			BEGIN
				IF(@startDate > @empUpd_EffDate)
				BEGIN
					DELETE ExePlantWorkerAssignment
					WHERE EmployeeID = @empUpd_ID AND StartDate = @startDate;
				END
				ELSE IF(@empUpd_EffDate BETWEEN @startDate AND @endDate)
				BEGIN
					IF(@empUpd_EffDate = @startDate)
					BEGIN
						DELETE ExePlantWorkerAssignment
						WHERE EmployeeID = @empUpd_ID AND StartDate = @startDate;
					END
					ELSE
					BEGIN
						UPDATE ExePlantWorkerAssignment
						SET
							EndDate = DATEADD(DAY, -1, @empUpd_EffDate),
							UpdatedDate = GETDATE(),
							UpdatedBy = 'System'
						WHERE EmployeeID = @empUpd_ID AND StartDate = @startDate;
					END
				END
			END
		END;
		ELSE
		BEGIN
			IF(@startDate = @endDate)  -- IF LUAR 1 date
			BEGIN
				IF(@startDate >= @empUpd_EffDate)
				BEGIN
					UPDATE ExePlantWorkerAssignment 
					SET SourceLocationCode = @empUpd_LocationCode,
					SourceUnitCode = @empUpd_UnitCode,
					SourceGroupCode = @empUpd_GroupCode,
					SourceProcessGroup = @empUpd_ProcessSettingsCode,
					UpdatedDate = GETDATE(),
					UpdatedBy = 'System'
					WHERE EmployeeID = @empUpd_ID AND StartDate = @startDate;
				END
			END
			ELSE IF(@startDate < @endDate)
			BEGIN
				IF(@startDate > @empUpd_EffDate)
				BEGIN
					UPDATE ExePlantWorkerAssignment 
					SET SourceLocationCode = @empUpd_LocationCode,
					SourceUnitCode = @empUpd_UnitCode,
					SourceGroupCode = @empUpd_GroupCode,
					SourceProcessGroup = @empUpd_ProcessSettingsCode,
					UpdatedDate = GETDATE(),
					UpdatedBy = 'System'
					WHERE EmployeeID = @empUpd_ID AND StartDate = @startDate;
				END
				ELSE IF(@empUpd_EffDate BETWEEN @startDate AND @endDate)
				BEGIN
					IF(@empUpd_EffDate = @startDate)
					BEGIN
						UPDATE ExePlantWorkerAssignment 
						SET SourceLocationCode = @empUpd_LocationCode,
						SourceUnitCode = @empUpd_UnitCode,
						SourceGroupCode = @empUpd_GroupCode,
						SourceProcessGroup = @empUpd_ProcessSettingsCode,
						UpdatedDate = GETDATE(),
						UpdatedBy = 'System'
						WHERE EmployeeID = @empUpd_ID AND StartDate = @startDate;
					END
					ELSE
					BEGIN
						UPDATE ExePlantWorkerAssignment
						SET
							EndDate = DATEADD(DAY, -1, @empUpd_EffDate),
							UpdatedDate = GETDATE(),
							UpdatedBy = 'System'
						WHERE EmployeeID = @empUpd_ID AND StartDate = @startDate;
			
						INSERT INTO [dbo].[ExePlantWorkerAssignment]
							([TransactionDate]
							,[SourceLocationCode]
							,[SourceUnitCode]
							,[SourceShift]
							,[SourceProcessGroup]
							,[SourceGroupCode]
							,[SourceBrandCode]
							,[DestinationLocationCode]
							,[DestinationUnitCode]
							,[DestinationShift]
							,[DestinationProcessGroup]
							,[DestinationGroupCode]
							,[DestinationGroupCodeDummy]
							,[DestinationBrandCode]
							,[EmployeeID]
							,[EmployeeNumber]
							,[StartDate]
							,[EndDate]
							,[CreatedDate]
							,[CreatedBy]
							,[UpdatedDate]
							,[UpdatedBy])
						SELECT 
							[TransactionDate]
							,@empUpd_LocationCode as [SourceLocationCode]
							,@empUpd_UnitCode as  [SourceUnitCode]
							,[SourceShift]
							,@empUpd_ProcessSettingsCode as [SourceProcessGroup]
							,@empUpd_GroupCode as [SourceGroupCode]
							,[SourceBrandCode]
							,[DestinationLocationCode]
							,[DestinationUnitCode]
							,[DestinationShift]
							,[DestinationProcessGroup]
							,[DestinationGroupCode]
							,[DestinationGroupCodeDummy]
							,[DestinationBrandCode]
							,[EmployeeID]
							,[EmployeeNumber]
							,@empUpd_EffDate as [StartDate]
							,[EndDate]
							,[CreatedDate]
							,[CreatedBy]
							,[UpdatedDate]
							,[UpdatedBy]
						FROM ExePlantWorkerAssignment			
						WHERE EmployeeID = @empUpd_ID AND StartDate = @startDate;
					END
				END
			END
		END

		-- Fixing Entry Assignment
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

		WHILE(@tmpEffDate <= @tmpEndDate)
		BEGIN
			DECLARE @dayNumber INT;
			DECLARE @dayNumberSunday INT;
			SET @dayNumberSunday = 7;
			SET @dayNumber = DATEPART(DW, @tmpEffDate);
			IF NOT EXISTS(SELECT * FROM MstGenHoliday WHERE HolidayDate = @tmpEffDate AND LocationCode = @empUpd_LocationCode)
			BEGIN
				IF(@dayNumber <> @dayNumberSunday)
				BEGIN
					-- GroupUpd = Dest Group
					IF(@destGroupCode = @empUpd_GroupCode)
					BEGIN
						EXEC [EMPJOBDATA_SCHEDULER_ASSIGNMENT_ENTRY_SAME_GROUP_ACV] @empUpd_ID, @empUpd_Number, @empUpd_EffDate, @empUpd_LocationCode,
							@empUpd_GroupCode, @empUpd_UnitCode, @empUpd_ProcessSettingsCode, @tmpEffDate, @dummyGroup
					END
					ELSE
					BEGIN
						IF(@empUpd_Status  = '5')
						BEGIN
							EXEC [EMPJOBDATA_SCHEDULER_ASSIGNMENT_ENTRY_ACV] @empUpd_ID, @empUpd_Number, @empUpd_EffDate, @empUpd_LocationCode, @empUpd_GroupCode,
								@empUpd_UnitCode, @empUpd_ProcessSettingsCode, @tmpEffDate
						END
						ELSE
						BEGIN -- status = 4
							EXEC [EMPJOBDATA_SCHEDULER_ASSIGNMENT_ENTRY_STS_4_ACV] @empUpd_ID, @tmpEffDate
						END
					END  
				END
			END

			SET @tmpEffDate = DATEADD(DAY, 1, @tmpEffDate);
		END		
		FETCH NEXT FROM cursor_assignment
		INTO @srcLocation, @srcBrandCode, @srcGroupCode, @srcProcess, @srcUnit, @srcShift, @startDate, @endDate, @destGroupCode, @dummyGroup
	END;
	CLOSE cursor_assignment ;
	DEALLOCATE cursor_assignment;
END
