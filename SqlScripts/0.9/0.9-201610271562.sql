SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



ALTER PROCEDURE [dbo].[EMPJOBDATA_SCHEDULER_PRODUCTIONCARD]
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
				SELECT * FROM ProductionCard WHERE EmployeeID = @empUpd_ID AND ProductionDate >= @empUpd_EffDate
			)
			BEGIN
				DECLARE @tmpEffDate DATETIME;
				SET @tmpEffDate = @empUpd_EffDate;

				DECLARE @endDateCurrWeekSaturday DATETIME;
				SELECT @endDateCurrWeekSaturday = DATEADD(DAY, -1, EndDate) 
				FROM MstGenWeek WHERE CONVERT(DATE, GETDATE()) BETWEEN StartDate AND EndDate;

				WHILE(@tmpEffDate <= @endDateCurrWeekSaturday)
				BEGIN
					DECLARE @dayNumber INT;
					DECLARE @dayNumberSunday INT;
					SET @dayNumberSunday = 7;
					SET @dayNumber = DATEPART(DW, @tmpEffDate);
					IF NOT EXISTS(SELECT * FROM MstGenHoliday WHERE HolidayDate = @tmpEffDate AND LocationCode = @empUpd_LocationCode)
					BEGIN
						IF(@dayNumber <> @dayNumberSunday)
						BEGIN
							DECLARE @brandCode VARCHAR(64);
							DECLARE @shift INT;
							DECLARE @kpsWeek INT
							DECLARE @kpsYear INT;
							SELECT TOP 1 @kpsWeek = [Week], @kpsYear = [Year] FROM MstGenWeek WHERE @tmpEffDate BETWEEN StartDate AND EndDate

							SELECT TOP 1 @brandCode = BrandCode, @shift = Shift FROM ProductionCard 
							WHERE EmployeeID = @empUpd_ID AND ProductionDate = @tmpEffDate

							DECLARE @productionCardCode VARCHAR(64);
							SET @productionCardCode = 'EBL/' + @empUpd_LocationCode 
									+ '/' + CONVERT(varchar, @shift) 
									+ '/' + @empUpd_UnitCode 
									+ '/' + @empUpd_GroupCode 
									+ '/' + @brandCode 
									+ '/' + CONVERT(varchar,@kpsYear) 
									+ '/' + CONVERT(varchar,@kpsWeek)
									+ '/' + CONVERT(varchar,(select datepart(dw, @tmpEffDate)));

							UPDATE ProductionCard
							SET 
								LocationCode = @empUpd_LocationCode,
								UnitCode = @empUpd_UnitCode,
								ProcessGroup = @empUpd_ProcessSettingsCode,
								GroupCode = @empUpd_GroupCode,
								EmployeeNumber = @empUpd_Number,
								ProductionCardCode = @productionCardCode,
								UpdatedBy = 'SYSTEM',
								UpdatedDate = GETDATE()
							WHERE EmployeeID = @empUpd_ID AND ProductionDate = @tmpEffDate;
						END
					END
					SET @tmpEffDate = DATEADD(DAY, 1, @tmpEffDate);	  
				END
			END
		END
		ELSE IF(@empUpd_Status = '4')
		BEGIN
			DELETE ProductionCard
			WHERE EmployeeID = @empUpd_ID AND ProductionDate >= @empUpd_EffDate
		END
	END
	ELSE
	BEGIN
		DELETE ProductionCard
		WHERE EmployeeID = @empUpd_ID AND ProductionDate >= @empUpd_EffDate
	END
END

