IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[dbo].[EMPJOBDATA_SCHEDULER_ASSIGNMENT_NOT_ACV]'))
	DROP PROCEDURE [dbo].[EMPJOBDATA_SCHEDULER_ASSIGNMENT_NOT_ACV]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[EMPJOBDATA_SCHEDULER_ASSIGNMENT_NOT_ACV]
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
	DECLARE @dummyGroup VARCHAR(4);

	DECLARE cursor_assignment CURSOR FOR
	SELECT StartDate, EndDate, DestinationGroupCodeDummy FROM ExePlantWorkerAssignment
	WHERE EmployeeID = @empUpd_ID AND (@empUpd_EffDate < StartDate OR @empUpd_EffDate BETWEEN StartDate AND EndDate) 

	OPEN cursor_assignment
	
	FETCH NEXT FROM cursor_assignment
	INTO @startDate, @endDate, @dummyGroup

	WHILE @@FETCH_STATUS = 0
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
		ELSE IF(@startDate > @empUpd_EffDate)
		BEGIN
			DELETE ExePlantWorkerAssignment
			WHERE EmployeeID = @empUpd_ID AND StartDate = @startDate;
		END

		-- Fixing Entry Absenteeism
		DECLARE @tmpEffDate DATETIME;
		SET @tmpEffDate = @empUpd_EffDate;

		WHILE(@tmpEffDate <= @endDate)
		BEGIN
			EXEC [EMPJOBDATA_SCHEDULER_ASSIGNMENT_ENTRY_NOT_ACV] 
			@empUpd_ID, @empUpd_Number, @empUpd_EffDate,  @empUpd_LocationCode,  
			@empUpd_GroupCode, @empUpd_UnitCode, @empUpd_ProcessSettingsCode, @tmpEffDate, @dummyGroup
		
			SET @tmpEffDate = DATEADD(DAY, 1, @tmpEffDate);	  
		END													 
												 
		FETCH NEXT FROM cursor_assignment					
		INTO @startDate, @endDate, @dummyGroup				
	END;													 
															 
	CLOSE cursor_assignment ;
	DEALLOCATE cursor_assignment;
END
