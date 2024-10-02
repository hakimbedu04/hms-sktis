/****** Object:  StoredProcedure [dbo].[EDIT_WORKER_ABSENTEEISM_FROM_ENTRY]    Script Date: 7/21/2016 10:07:12 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[EDIT_WORKER_ABSENTEEISM_FROM_ENTRY]
	@StartDateAbsent	DATETIME,
	@AbsentTypeDTO		VARCHAR(128),
	@AbsentTypeDB		VARCHAR(128),
	@SktAbsentCode		VARCHAR(11),
	@PayrollAbsentCode	VARCHAR(11),
	@EmployeeID			VARCHAR(64),
	@LocationCode		VARCHAR(8),
	@UnitCode			VARCHAR(4),
	@GroupCode			VARCHAR(4),
	@Shift				INT,
	@UpdatedDate		DATETIME,
	@UpdatedBy			VARCHAR(64)
AS
BEGIN
	DECLARE @ErrorMessage   NVARCHAR(1000);
	DECLARE @ErrorState	INT;
	DECLARE @ErrorSeverity  INT;

	BEGIN TRANSACTION 
	BEGIN TRY
		SET DATEFIRST 1

		-- Get Employee Number from MstPlantEmpJobsDataAll by Employee ID
		DECLARE @employeeNumber	VARCHAR(64);
		SELECT @employeeNumber = EmployeeNumber FROM MstPlantEmpJobsDataAll WHERE EmployeeID = @EmployeeID
		
		DECLARE @countCurrAbsenteeism INT;
		DECLARE @startDateCurrAbsenteeism DATETIME;
		DECLARE @endDateCurrAbsenteeism DATETIME;
		DECLARE @currAbsentType			VARCHAR(128);
		DECLARE @currSktAbsentCode		VARCHAR(11);
		DECLARE @currPayrollAbsentCode	VARCHAR(11);
		DECLARE @currLocationCode		VARCHAR(8);
		DECLARE @currUnitCode			VARCHAR(4);
		DECLARE @currGroupCode			VARCHAR(4);
		DECLARE @currShift				INT;

		DECLARE @countExistingAbsenteeism INT;
		DECLARE @existingStartDateAbsent DATETIME;

		SELECT @countCurrAbsenteeism = COUNT(*)
		FROM ExePlantWorkerAbsenteeism
		WHERE EmployeeID = @EmployeeID AND Shift = @Shift AND @StartDateAbsent BETWEEN StartDateAbsent AND EndDateAbsent;

		SELECT 
			@startDateCurrAbsenteeism = StartDateAbsent, @endDateCurrAbsenteeism = EndDateAbsent, @currAbsentType = AbsentType, @currSktAbsentCode = SktAbsentCode,
			@currPayrollAbsentCode = PayrollAbsentCode, @currLocationCode = LocationCode, @currUnitCode = UnitCode, @currGroupCode = GroupCode, @currShift = Shift
		FROM ExePlantWorkerAbsenteeism
		WHERE EmployeeID = @EmployeeID AND Shift = @Shift AND @StartDateAbsent BETWEEN StartDateAbsent AND EndDateAbsent;

		-- Assign Absent Type to null/empty
		IF(ISNULL(@AbsentTypeDTO, '') = '')
		BEGIN
			IF(@countCurrAbsenteeism > 0)
			BEGIN
				IF(@startDateCurrAbsenteeism = @StartDateAbsent)
				BEGIN
					IF(DATEDIFF(DAY, @startDateCurrAbsenteeism, @endDateCurrAbsenteeism) = 0)
					BEGIN
						DELETE FROM ExePlantWorkerAbsenteeism
						WHERE EmployeeID = @EmployeeID AND Shift = @Shift AND @StartDateAbsent BETWEEN StartDateAbsent AND EndDateAbsent;
					END;
					ELSE
					BEGIN
						-- Check data absenteeism before insert if end date less than start date
						IF([dbo].[CheckIsValidEndDateAndStartDate](DATEADD(DAY, 1, @startDateCurrAbsenteeism), @endDateCurrAbsenteeism) = 0)
						BEGIN 
							SET @ErrorMessage = 'End Date cannot be less than Start Date';  SET @ErrorState = 1; SET @ErrorSeverity = 16;
							RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState); 
						END;

						DELETE FROM ExePlantWorkerAbsenteeism
						WHERE EmployeeID = @EmployeeID AND Shift = @Shift AND @StartDateAbsent BETWEEN StartDateAbsent AND EndDateAbsent;

						INSERT INTO [dbo].[ExePlantWorkerAbsenteeism]
						   ([StartDateAbsent]
						   ,[EmployeeID]
						   ,[AbsentType]
						   ,[EndDateAbsent]
						   ,[SktAbsentCode]
						   ,[PayrollAbsentCode]
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
						VALUES
							  (DATEADD(DAY, 1, @startDateCurrAbsenteeism)
							  ,@EmployeeID
							  ,@currAbsentType
							  ,@endDateCurrAbsenteeism
							  ,@currSktAbsentCode
							  ,@currPayrollAbsentCode
							  ,@UpdatedDate
							  ,@UpdatedBy
							  ,@UpdatedDate
							  ,@UpdatedBy
							  ,@employeeNumber
							  ,@currLocationCode
							  ,@currUnitCode
							  ,@currGroupCode
							  ,CAST(GETDATE() as date)
							  ,@currShift)
					END;
				END;
				ELSE IF(@endDateCurrAbsenteeism = @StartDateAbsent)
				BEGIN
					IF(DATEDIFF(DAY, @startDateCurrAbsenteeism, @endDateCurrAbsenteeism) = 0)
					BEGIN
						DELETE FROM ExePlantWorkerAbsenteeism
						WHERE EmployeeID = @EmployeeID AND Shift = @Shift AND @StartDateAbsent BETWEEN StartDateAbsent AND EndDateAbsent;
					END;
					ELSE
					BEGIN
						-- Check data absenteeism before insert if end date less than start date
						IF([dbo].[CheckIsValidEndDateAndStartDate](@startDateCurrAbsenteeism, DATEADD(DAY, -1, @endDateCurrAbsenteeism)) = 0)
						BEGIN 
							SET @ErrorMessage = 'End Date cannot be less than Start Date';  SET @ErrorState = 1; SET @ErrorSeverity = 16;
							RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState); 
						END;

						DELETE FROM ExePlantWorkerAbsenteeism
						WHERE EmployeeID = @EmployeeID AND Shift = @Shift AND @StartDateAbsent BETWEEN StartDateAbsent AND EndDateAbsent;

						INSERT INTO [dbo].[ExePlantWorkerAbsenteeism]
						   ([StartDateAbsent]
						   ,[EmployeeID]
						   ,[AbsentType]
						   ,[EndDateAbsent]
						   ,[SktAbsentCode]
						   ,[PayrollAbsentCode]
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
						VALUES
							  (@startDateCurrAbsenteeism
							  ,@EmployeeID
							  ,@currAbsentType
							  ,DATEADD(DAY, -1, @endDateCurrAbsenteeism)
							  ,@currSktAbsentCode
							  ,@currPayrollAbsentCode
							  ,@UpdatedDate
							  ,@UpdatedBy
							  ,@UpdatedDate
							  ,@UpdatedBy
							  ,@employeeNumber
							  ,@currLocationCode
							  ,@currUnitCode
							  ,@currGroupCode
							  ,CAST(GETDATE() as date)
							  ,@currShift)
					END;		
				END;
				ELSE
				BEGIN
					DELETE FROM ExePlantWorkerAbsenteeism
					WHERE EmployeeID = @EmployeeID AND Shift = @Shift AND @StartDateAbsent BETWEEN StartDateAbsent AND EndDateAbsent;

					-- Check data absenteeism before insert if end date less than start date
					IF([dbo].[CheckIsValidEndDateAndStartDate](@startDateCurrAbsenteeism, DATEADD(DAY, -1, @StartDateAbsent)) = 0)
					BEGIN 
						SET @ErrorMessage = 'End Date cannot be less than Start Date';  SET @ErrorState = 1; SET @ErrorSeverity = 16;
						RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState); 
					END;

					INSERT INTO [dbo].[ExePlantWorkerAbsenteeism]
						   ([StartDateAbsent]
						   ,[EmployeeID]
						   ,[AbsentType]
						   ,[EndDateAbsent]
						   ,[SktAbsentCode]
						   ,[PayrollAbsentCode]
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
						VALUES
							  (@startDateCurrAbsenteeism
							   ,@EmployeeID
							  ,@currAbsentType
							  ,DATEADD(DAY, -1, @StartDateAbsent)
							  ,@currSktAbsentCode
							  ,@currPayrollAbsentCode
							  ,@UpdatedDate
							  ,@UpdatedBy
							  ,@UpdatedDate
							  ,@UpdatedBy
							  ,@employeeNumber
							  ,@currLocationCode
							  ,@currUnitCode
							  ,@currGroupCode
							  ,CAST(GETDATE() as date)
							  ,@currShift)

					-- Check data absenteeism before insert if end date less than start date
					IF([dbo].[CheckIsValidEndDateAndStartDate](DATEADD(DAY, 1, @StartDateAbsent), @endDateCurrAbsenteeism) = 0)
					BEGIN 
						SET @ErrorMessage = 'End Date cannot be less than Start Date';  SET @ErrorState = 1; SET @ErrorSeverity = 16;
						RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState); 
					END;

					INSERT INTO [dbo].[ExePlantWorkerAbsenteeism]
						   ([StartDateAbsent]
						   ,[EmployeeID]
						   ,[AbsentType]
						   ,[EndDateAbsent]
						   ,[SktAbsentCode]
						   ,[PayrollAbsentCode]
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
						VALUES
							  (DATEADD(DAY, 1, @StartDateAbsent)
							   ,@EmployeeID
							  ,@currAbsentType
							  ,@endDateCurrAbsenteeism
							  ,@currSktAbsentCode
							  ,@currPayrollAbsentCode
							  ,@UpdatedDate
							  ,@UpdatedBy
							  ,@UpdatedDate
							  ,@UpdatedBy
							  ,@employeeNumber
							  ,@currLocationCode
							  ,@currUnitCode
							  ,@currGroupCode
							  ,CAST(GETDATE() as date)
							  ,@currShift)
				END;
			END;
		END;
		ELSE
		BEGIN
			IF(@AbsentTypeDTO <> ISNULL(@AbsentTypeDB, ''))
			BEGIN
				IF(@countCurrAbsenteeism > 0)
				BEGIN
					IF(@startDateCurrAbsenteeism = @StartDateAbsent)
					BEGIN
						IF(DATEDIFF(DAY, @startDateCurrAbsenteeism, @endDateCurrAbsenteeism) = 0)
						BEGIN
							DELETE FROM ExePlantWorkerAbsenteeism
							WHERE EmployeeID = @EmployeeID AND Shift = @Shift AND @StartDateAbsent BETWEEN StartDateAbsent AND EndDateAbsent;
							
							 -- Check Previous day absenteeism with the same absent type
							DECLARE @previousDay DATETIME;
							DECLARE @countPreviousDayAbsenteeism INT;
							DECLARE @startDateAbsentPreviousAbsenteeism DATETIME;
							DECLARE @endDateAbsentPreviousAbsenteeism DATETIME;

							SET @previousDay = DATEADD(DAY, -1, CAST(@StartDateAbsent as date));
		
							SELECT @countPreviousDayAbsenteeism = COUNT(*) 
							FROM ExePlantWorkerAbsenteeism 
							WHERE EmployeeID = @EmployeeID AND Shift = @Shift AND EndDateAbsent = @previousDay AND AbsentType = @AbsentTypeDTO;

							SELECT @startDateAbsentPreviousAbsenteeism = @StartDateAbsent, @endDateAbsentPreviousAbsenteeism = EndDateAbsent
							FROM ExePlantWorkerAbsenteeism
							WHERE EmployeeID = @EmployeeID AND Shift = @Shift AND EndDateAbsent = @previousDay AND AbsentType = @AbsentTypeDTO;
		
							-- Check Next day absenteeism with the same absent type
							DECLARE @nextDay DATETIME;
							DECLARE @countNextDayAbsenteeism INT;
							DECLARE @startDateAbsentNextAbsenteeism DATETIME;
							DECLARE @endDateAbsentNextAbsenteeism DATETIME;

							SET @nextDay = DATEADD(DAY, 1, CAST(@StartDateAbsent as date));
		
							SELECT @countNextDayAbsenteeism = COUNT(*) 
							FROM ExePlantWorkerAbsenteeism 
							WHERE EmployeeID = @EmployeeID AND Shift = @Shift AND StartDateAbsent = @nextDay AND AbsentType = @AbsentTypeDTO;

							SELECT @startDateAbsentNextAbsenteeism = @StartDateAbsent, @endDateAbsentNextAbsenteeism = EndDateAbsent
							FROM ExePlantWorkerAbsenteeism
							WHERE EmployeeID = @EmployeeID AND Shift = @Shift AND StartDateAbsent = @nextDay AND AbsentType = @AbsentTypeDTO;

							IF([dbo].[CheckDateClosingPayrollOrHoliday](@StartDateAbsent, @LocationCode) = 0)
							BEGIN /** Checking if start date absent is no closing payroll or holiday **/
								IF([dbo].[CheckDateClosingPayrollOrHoliday](@previousDay, @LocationCode) = 1)
								BEGIN /** Checking if previous day is closing payroll or holiday **/
									IF(@countNextDayAbsenteeism > 0)
									BEGIN
										-- Check data absenteeism before insert if end date less than start date
										IF([dbo].[CheckIsValidEndDateAndStartDate](@StartDateAbsent, @endDateAbsentNextAbsenteeism) = 0)
										BEGIN 
											SET @ErrorMessage = 'End Date cannot be less than Start Date';  SET @ErrorState = 1; SET @ErrorSeverity = 16;
											RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState); 
										END;

										INSERT INTO [dbo].[ExePlantWorkerAbsenteeism]
										   ([StartDateAbsent]
										   ,[EmployeeID]
										   ,[AbsentType]
										   ,[EndDateAbsent]
										   ,[SktAbsentCode]
										   ,[PayrollAbsentCode]
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
										VALUES
										   (@StartDateAbsent
										   ,@EmployeeID
										   ,@AbsentTypeDTO
										   ,@endDateAbsentNextAbsenteeism
										   ,@SktAbsentCode
										   ,@PayrollAbsentCode
										   ,@UpdatedDate
										   ,@UpdatedBy
										   ,@UpdatedDate
										   ,@UpdatedBy
										   ,@employeeNumber
										   ,@LocationCode
										   ,@UnitCode
										   ,@GroupCode
										   ,CAST(GETDATE() as date)
										   ,@Shift)

										   DELETE FROM ExePlantWorkerAbsenteeism 
										   WHERE EmployeeID = @EmployeeID AND Shift = @Shift AND StartDateAbsent = @nextDay AND AbsentType = @AbsentTypeDTO;
									END;
									ELSE
									BEGIN
										SELECT @countExistingAbsenteeism = COUNT(*)
										FROM ExePlantWorkerAbsenteeism 
										WHERE StartDateAbsent = @StartDateAbsent AND EmployeeID = @EmployeeID AND Shift = @Shift;

										SELECT @StartDateAbsent = StartDateAbsent FROM ExePlantWorkerAbsenteeism
										WHERE StartDateAbsent = @StartDateAbsent AND EmployeeID = @EmployeeID AND Shift = @Shift;

										IF(@countExistingAbsenteeism > 0)
										BEGIN
											-- Check data absenteeism before insert if end date less than start date
											IF([dbo].[CheckIsValidEndDateAndStartDate](@existingStartDateAbsent, @StartDateAbsent) = 0)
											BEGIN 
												SET @ErrorMessage = 'End Date cannot be less than Start Date';  SET @ErrorState = 1; SET @ErrorSeverity = 16;
												RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState); 
											END;

											UPDATE ExePlantWorkerAbsenteeism
											SET 
												AbsentType			= @AbsentTypeDTO,
												EndDateAbsent		= @StartDateAbsent,
												SktAbsentCode		= @SktAbsentCode,
												PayrollAbsentCode	= @PayrollAbsentCode,
												CreatedDate			= @UpdatedDate,
												CreatedBy			= @UpdatedBy,
												UpdatedDate			= @UpdatedDate,
												UpdatedBy			= @UpdatedBy,
												LocationCode		= @LocationCode,
												UnitCode			= @UnitCode,
												GroupCode			= @GroupCode,
												TransactionDate		= CAST(GETDATE() as date),
												Shift				= @Shift,
												EmployeeNumber		= @employeeNumber
											WHERE EmployeeID = @EmployeeID AND Shift = @Shift AND @StartDateAbsent BETWEEN StartDateAbsent AND EndDateAbsent;
										END;
										ELSE
										BEGIN
											INSERT INTO [dbo].[ExePlantWorkerAbsenteeism]
											   ([StartDateAbsent]
											   ,[EmployeeID]
											   ,[AbsentType]
											   ,[EndDateAbsent]
											   ,[SktAbsentCode]
											   ,[PayrollAbsentCode]
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
											VALUES
											   (@StartDateAbsent
											   ,@EmployeeID
											   ,@AbsentTypeDTO
											   ,@StartDateAbsent
											   ,@SktAbsentCode
											   ,@PayrollAbsentCode
											   ,@UpdatedDate
											   ,@UpdatedBy
											   ,@UpdatedDate
											   ,@UpdatedBy
											   ,@employeeNumber
											   ,@LocationCode
											   ,@UnitCode
											   ,@GroupCode
											   ,CAST(GETDATE() as date)
											   ,@Shift)
										END;
									END;
								END;
								ELSE IF([dbo].[CheckDateClosingPayrollOrHoliday](@nextDay, @LocationCode) = 1)
								BEGIN
									IF(@countPreviousDayAbsenteeism > 0)
									BEGIN
										-- Check data absenteeism before insert if end date less than start date
										IF([dbo].[CheckIsValidEndDateAndStartDate](@startDateAbsentPreviousAbsenteeism, @StartDateAbsent) = 0)
										BEGIN 
											SET @ErrorMessage = 'End Date cannot be less than Start Date';  SET @ErrorState = 1; SET @ErrorSeverity = 16;
											RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState); 
										END;

										UPDATE ExePlantWorkerAbsenteeism
										SET 
												EndDateAbsent		= @StartDateAbsent,
												SktAbsentCode		= @SktAbsentCode,
												PayrollAbsentCode	= @PayrollAbsentCode,
												CreatedDate			= @UpdatedDate,
												CreatedBy			= @UpdatedBy,
												UpdatedDate			= @UpdatedDate,
												UpdatedBy			= @UpdatedBy,
												LocationCode		= @LocationCode,
												UnitCode			= @UnitCode,
												GroupCode			= @GroupCode,
												TransactionDate		= CAST(GETDATE() as date),
												Shift				= @Shift,
												EmployeeNumber		= @employeeNumber
										WHERE EmployeeID = @EmployeeID AND Shift = @Shift AND EndDateAbsent = @previousDay AND AbsentType = @AbsentTypeDTO;
									END;
									ELSE
									BEGIN
										SELECT @countExistingAbsenteeism = COUNT(*)
										FROM ExePlantWorkerAbsenteeism 
										WHERE StartDateAbsent = @StartDateAbsent AND EmployeeID = @EmployeeID AND Shift = @Shift;

										SELECT @StartDateAbsent = StartDateAbsent FROM ExePlantWorkerAbsenteeism
										WHERE StartDateAbsent = @StartDateAbsent AND EmployeeID = @EmployeeID AND Shift = @Shift;

										IF(@countExistingAbsenteeism > 0)
										BEGIN
											-- Check data absenteeism before insert if end date less than start date
											IF([dbo].[CheckIsValidEndDateAndStartDate](@existingStartDateAbsent, @StartDateAbsent) = 0)
											BEGIN 
												SET @ErrorMessage = 'End Date cannot be less than Start Date';  SET @ErrorState = 1; SET @ErrorSeverity = 16;
												RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState); 
											END;

											UPDATE ExePlantWorkerAbsenteeism
											SET 
												AbsentType			= @AbsentTypeDTO,
												EndDateAbsent		= @StartDateAbsent,
												SktAbsentCode		= @SktAbsentCode,
												PayrollAbsentCode	= @PayrollAbsentCode,
												CreatedDate			= @UpdatedDate,
												CreatedBy			= @UpdatedBy,
												UpdatedDate			= @UpdatedDate,
												UpdatedBy			= @UpdatedBy,
												LocationCode		= @LocationCode,
												UnitCode			= @UnitCode,
												GroupCode			= @GroupCode,
												TransactionDate		= CAST(GETDATE() as date),
												Shift				= @Shift,
												EmployeeNumber		= @employeeNumber
											WHERE EmployeeID = @EmployeeID AND Shift = @Shift AND @StartDateAbsent BETWEEN StartDateAbsent AND EndDateAbsent;
										END;
										ELSE
										BEGIN
											INSERT INTO [dbo].[ExePlantWorkerAbsenteeism]
											   ([StartDateAbsent]
											   ,[EmployeeID]
											   ,[AbsentType]
											   ,[EndDateAbsent]
											   ,[SktAbsentCode]
											   ,[PayrollAbsentCode]
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
											VALUES
											   (@StartDateAbsent
											   ,@EmployeeID
											   ,@AbsentTypeDTO
											   ,@StartDateAbsent
											   ,@SktAbsentCode
											   ,@PayrollAbsentCode
											   ,@UpdatedDate
											   ,@UpdatedBy
											   ,@UpdatedDate
											   ,@UpdatedBy
											   ,@employeeNumber
											   ,@LocationCode
											   ,@UnitCode
											   ,@GroupCode
											   ,CAST(GETDATE() as date)
											   ,@Shift)
										END;
									END;
								END;
								ELSE
								BEGIN
									IF(@countPreviousDayAbsenteeism > 0 AND @countNextDayAbsenteeism >0)
									BEGIN
										-- Check data absenteeism before insert if end date less than start date
										IF([dbo].[CheckIsValidEndDateAndStartDate](@startDateAbsentPreviousAbsenteeism, @endDateAbsentNextAbsenteeism) = 0)
										BEGIN 
											SET @ErrorMessage = 'End Date cannot be less than Start Date';  SET @ErrorState = 1; SET @ErrorSeverity = 16;
											RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState); 
										END;

										UPDATE ExePlantWorkerAbsenteeism
										SET 
											AbsentType			= @AbsentTypeDTO,
											EndDateAbsent		= @endDateAbsentNextAbsenteeism,
											SktAbsentCode		= @SktAbsentCode,
											PayrollAbsentCode	= @PayrollAbsentCode,
											CreatedDate			= @UpdatedDate,
											CreatedBy			= @UpdatedBy,
											UpdatedDate			= @UpdatedDate,
											UpdatedBy			= @UpdatedBy,
											LocationCode		= @LocationCode,
											UnitCode			= @UnitCode,
											GroupCode			= @GroupCode,
											TransactionDate		= CAST(GETDATE() as date),
											Shift				= @Shift,
											EmployeeNumber		= @employeeNumber
										WHERE EmployeeID = @EmployeeID AND Shift = @Shift AND EndDateAbsent = @previousDay AND AbsentType = @AbsentTypeDTO;

										DELETE FROM ExePlantWorkerAbsenteeism 
										WHERE EmployeeID = @EmployeeID AND Shift = @Shift AND StartDateAbsent = @nextDay AND AbsentType = @AbsentTypeDTO;
									END;
									ELSE IF(@countPreviousDayAbsenteeism > 0 AND @countNextDayAbsenteeism = 0)
									BEGIN
										-- Check data absenteeism before insert if end date less than start date
										IF([dbo].[CheckIsValidEndDateAndStartDate](@startDateAbsentPreviousAbsenteeism, @StartDateAbsent) = 0)
										BEGIN 
											SET @ErrorMessage = 'End Date cannot be less than Start Date';  SET @ErrorState = 1; SET @ErrorSeverity = 16;
											RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState); 
										END;

										UPDATE ExePlantWorkerAbsenteeism
										SET 
											AbsentType			= @AbsentTypeDTO,
											EndDateAbsent		= @StartDateAbsent,
											SktAbsentCode		= @SktAbsentCode,
											PayrollAbsentCode	= @PayrollAbsentCode,
											CreatedDate			= @UpdatedDate,
											CreatedBy			= @UpdatedBy,
											UpdatedDate			= @UpdatedDate,
											UpdatedBy			= @UpdatedBy,
											LocationCode		= @LocationCode,
											UnitCode			= @UnitCode,
											GroupCode			= @GroupCode,
											TransactionDate		= CAST(GETDATE() as date),
											Shift				= @Shift,
											EmployeeNumber		= @employeeNumber
										WHERE EmployeeID = @EmployeeID AND Shift = @Shift AND EndDateAbsent = @previousDay AND AbsentType = @AbsentTypeDTO;
									END;
									ELSE IF(@countPreviousDayAbsenteeism = 0 AND @countNextDayAbsenteeism > 0)
									BEGIN
										-- Check data absenteeism before insert if end date less than start date
										IF([dbo].[CheckIsValidEndDateAndStartDate](@StartDateAbsent, @endDateAbsentNextAbsenteeism) = 0)
										BEGIN 
											SET @ErrorMessage = 'End Date cannot be less than Start Date';  SET @ErrorState = 1; SET @ErrorSeverity = 16;
											RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState); 
										END;

										INSERT INTO [dbo].[ExePlantWorkerAbsenteeism]
											   ([StartDateAbsent]
											   ,[EmployeeID]
											   ,[AbsentType]
											   ,[EndDateAbsent]
											   ,[SktAbsentCode]
											   ,[PayrollAbsentCode]
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
											VALUES
											   (@StartDateAbsent
											   ,@EmployeeID
											   ,@AbsentTypeDTO
											   ,@endDateAbsentNextAbsenteeism
											   ,@SktAbsentCode
											   ,@PayrollAbsentCode
											   ,@UpdatedDate
											   ,@UpdatedBy
											   ,@UpdatedDate
											   ,@UpdatedBy
											   ,@employeeNumber
											   ,@LocationCode
											   ,@UnitCode
											   ,@GroupCode
											   ,CAST(GETDATE() as date)
											   ,@Shift)

										DELETE FROM ExePlantWorkerAbsenteeism 
										WHERE EmployeeID = @EmployeeID AND Shift = @Shift AND StartDateAbsent = @nextDay AND AbsentType = @AbsentTypeDTO;
									END;
									ELSE
									BEGIN
										SELECT @countExistingAbsenteeism = COUNT(*)
										FROM ExePlantWorkerAbsenteeism 
										WHERE StartDateAbsent = @StartDateAbsent AND EmployeeID = @EmployeeID AND Shift = @Shift;

										SELECT @StartDateAbsent = StartDateAbsent FROM ExePlantWorkerAbsenteeism
										WHERE StartDateAbsent = @StartDateAbsent AND EmployeeID = @EmployeeID AND Shift = @Shift;

										IF(@countExistingAbsenteeism > 0)
										BEGIN
											-- Check data absenteeism before insert if end date less than start date
											IF([dbo].[CheckIsValidEndDateAndStartDate](@existingStartDateAbsent, @StartDateAbsent) = 0)
											BEGIN 
												SET @ErrorMessage = 'End Date cannot be less than Start Date';  SET @ErrorState = 1; SET @ErrorSeverity = 16;
												RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState); 
											END;

											UPDATE ExePlantWorkerAbsenteeism
											SET 
												AbsentType			= @AbsentTypeDTO,
												EndDateAbsent		= @StartDateAbsent,
												SktAbsentCode		= @SktAbsentCode,
												PayrollAbsentCode	= @PayrollAbsentCode,
												CreatedDate			= @UpdatedDate,
												CreatedBy			= @UpdatedBy,
												UpdatedDate			= @UpdatedDate,
												UpdatedBy			= @UpdatedBy,
												LocationCode		= @LocationCode,
												UnitCode			= @UnitCode,
												GroupCode			= @GroupCode,
												TransactionDate		= CAST(GETDATE() as date),
												Shift				= @Shift,
												EmployeeNumber		= @employeeNumber
											WHERE EmployeeID = @EmployeeID AND Shift = @Shift AND @StartDateAbsent BETWEEN StartDateAbsent AND EndDateAbsent;
										END;
										ELSE
										BEGIN
											INSERT INTO [dbo].[ExePlantWorkerAbsenteeism]
											   ([StartDateAbsent]
											   ,[EmployeeID]
											   ,[AbsentType]
											   ,[EndDateAbsent]
											   ,[SktAbsentCode]
											   ,[PayrollAbsentCode]
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
											VALUES
											   (@StartDateAbsent
											   ,@EmployeeID
											   ,@AbsentTypeDTO
											   ,@StartDateAbsent
											   ,@SktAbsentCode
											   ,@PayrollAbsentCode
											   ,@UpdatedDate
											   ,@UpdatedBy
											   ,@UpdatedDate
											   ,@UpdatedBy
											   ,@employeeNumber
											   ,@LocationCode
											   ,@UnitCode
											   ,@GroupCode
											   ,CAST(GETDATE() as date)
											   ,@Shift)
										END;
									END;
								END;
							END;
							ELSE
							BEGIN
								IF(@countPreviousDayAbsenteeism > 0)
								BEGIN
									-- Check data absenteeism before insert if end date less than start date
									IF([dbo].[CheckIsValidEndDateAndStartDate](@endDateAbsentPreviousAbsenteeism, @StartDateAbsent) = 0)
									BEGIN 
										SET @ErrorMessage = 'End Date cannot be less than Start Date';  SET @ErrorState = 1; SET @ErrorSeverity = 16;
										RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState); 
									END;

									UPDATE ExePlantWorkerAbsenteeism
									SET 
										AbsentType			= @AbsentTypeDTO,
										EndDateAbsent		= @StartDateAbsent,
										SktAbsentCode		= @SktAbsentCode,
										PayrollAbsentCode	= @PayrollAbsentCode,
										CreatedDate			= @UpdatedDate,
										CreatedBy			= @UpdatedBy,
										UpdatedDate			= @UpdatedDate,
										UpdatedBy			= @UpdatedBy,
										LocationCode		= @LocationCode,
										UnitCode			= @UnitCode,
										GroupCode			= @GroupCode,
										TransactionDate		= CAST(GETDATE() as date),
										Shift				= @Shift,
										EmployeeNumber		= @employeeNumber
									WHERE EmployeeID = @EmployeeID AND Shift = @Shift AND EndDateAbsent = @previousDay AND AbsentType = @AbsentTypeDTO;
								END;
								ELSE
								BEGIN
									SELECT @countExistingAbsenteeism = COUNT(*)
									FROM ExePlantWorkerAbsenteeism 
									WHERE StartDateAbsent = @StartDateAbsent AND EmployeeID = @EmployeeID AND Shift = @Shift;

									SELECT @StartDateAbsent = StartDateAbsent FROM ExePlantWorkerAbsenteeism
									WHERE StartDateAbsent = @StartDateAbsent AND EmployeeID = @EmployeeID AND Shift = @Shift;

									IF(@countExistingAbsenteeism > 0)
									BEGIN
										-- Check data absenteeism before insert if end date less than start date
										IF([dbo].[CheckIsValidEndDateAndStartDate](@existingStartDateAbsent, @StartDateAbsent) = 0)
										BEGIN 
											SET @ErrorMessage = 'End Date cannot be less than Start Date';  SET @ErrorState = 1; SET @ErrorSeverity = 16;
											RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState); 
										END;

										UPDATE ExePlantWorkerAbsenteeism
										SET 
											AbsentType			= @AbsentTypeDTO,
											EndDateAbsent		= @StartDateAbsent,
											SktAbsentCode		= @SktAbsentCode,
											PayrollAbsentCode	= @PayrollAbsentCode,
											CreatedDate			= @UpdatedDate,
											CreatedBy			= @UpdatedBy,
											UpdatedDate			= @UpdatedDate,
											UpdatedBy			= @UpdatedBy,
											LocationCode		= @LocationCode,
											UnitCode			= @UnitCode,
											GroupCode			= @GroupCode,
											TransactionDate		= CAST(GETDATE() as date),
											Shift				= @Shift,
											EmployeeNumber		= @employeeNumber
										WHERE EmployeeID = @EmployeeID AND Shift = @Shift AND @StartDateAbsent BETWEEN StartDateAbsent AND EndDateAbsent;
									END;
									ELSE
									BEGIN
										INSERT INTO [dbo].[ExePlantWorkerAbsenteeism]
										   ([StartDateAbsent]
										   ,[EmployeeID]
										   ,[AbsentType]
										   ,[EndDateAbsent]
										   ,[SktAbsentCode]
										   ,[PayrollAbsentCode]
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
										VALUES
										   (@StartDateAbsent
										   ,@EmployeeID
										   ,@AbsentTypeDTO
										   ,@StartDateAbsent
										   ,@SktAbsentCode
										   ,@PayrollAbsentCode
										   ,@UpdatedDate
										   ,@UpdatedBy
										   ,@UpdatedDate
										   ,@UpdatedBy
										   ,@employeeNumber
										   ,@LocationCode
										   ,@UnitCode
										   ,@GroupCode
										   ,CAST(GETDATE() as date)
										   ,@Shift)
									END;
								END;
							END;
						END;
						ELSE
						BEGIN
						
							DELETE ExePlantWorkerAbsenteeism
							WHERE EmployeeID = @EmployeeID AND Shift = @Shift AND @StartDateAbsent BETWEEN StartDateAbsent AND EndDateAbsent;

							-- Check data absenteeism before insert if end date less than start date
							IF([dbo].[CheckIsValidEndDateAndStartDate](@startDateCurrAbsenteeism, @StartDateAbsent) = 0)
							BEGIN 
								SET @ErrorMessage = 'End Date cannot be less than Start Date';  SET @ErrorState = 1; SET @ErrorSeverity = 16;
								RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState); 
							END;
							
							INSERT INTO [dbo].[ExePlantWorkerAbsenteeism]
							   ([StartDateAbsent]
							   ,[EmployeeID]
							   ,[AbsentType]
							   ,[EndDateAbsent]
							   ,[SktAbsentCode]
							   ,[PayrollAbsentCode]
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
							VALUES
								  (DATEADD(DAY, 1, @startDateCurrAbsenteeism)
								  ,@EmployeeID
								  ,@currAbsentType
								  ,@endDateCurrAbsenteeism
								  ,@currSktAbsentCode
								  ,@currPayrollAbsentCode
								  ,@UpdatedDate
								  ,@UpdatedBy
								  ,@UpdatedDate
								  ,@UpdatedBy
								  ,@employeeNumber
								  ,@currLocationCode
								  ,@currUnitCode
								  ,@currGroupCode
								  ,CAST(GETDATE() as date)
								  ,@currShift)

							INSERT INTO [dbo].[ExePlantWorkerAbsenteeism]
								([StartDateAbsent]
								,[EmployeeID]
								,[AbsentType]
								,[EndDateAbsent]
								,[SktAbsentCode]
								,[PayrollAbsentCode]
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
							VALUES
							   (@StartDateAbsent
							   ,@EmployeeID
							   ,@AbsentTypeDTO
							   ,@StartDateAbsent
							   ,@SktAbsentCode
							   ,@PayrollAbsentCode
							   ,@UpdatedDate
							   ,@UpdatedBy
							   ,@UpdatedDate
							   ,@UpdatedBy
							   ,@employeeNumber
							   ,@LocationCode
							   ,@UnitCode
							   ,@GroupCode
							   ,CAST(GETDATE() as date)
							   ,@Shift)

						END;
					END;
					ELSE IF(@endDateCurrAbsenteeism = @StartDateAbsent)
					BEGIN
						IF(DATEDIFF(DAY, @startDateCurrAbsenteeism, @endDateCurrAbsenteeism) = 0)
						BEGIN

							DELETE ExePlantWorkerAbsenteeism
							WHERE EmployeeID = @EmployeeID AND Shift = @Shift AND @StartDateAbsent BETWEEN StartDateAbsent AND EndDateAbsent;
							
							INSERT INTO [dbo].[ExePlantWorkerAbsenteeism]
								([StartDateAbsent]
								,[EmployeeID]
								,[AbsentType]
								,[EndDateAbsent]
								,[SktAbsentCode]
								,[PayrollAbsentCode]
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
							VALUES
							   (@StartDateAbsent
							   ,@EmployeeID
							   ,@AbsentTypeDTO
							   ,@StartDateAbsent
							   ,@SktAbsentCode
							   ,@PayrollAbsentCode
							   ,@UpdatedDate
							   ,@UpdatedBy
							   ,@UpdatedDate
							   ,@UpdatedBy
							   ,@employeeNumber
							   ,@LocationCode
							   ,@UnitCode
							   ,@GroupCode
							   ,CAST(GETDATE() as date)
							   ,@Shift)
						END;
						ELSE
						BEGIN
							-- Check data absenteeism before insert if end date less than start date
							IF([dbo].[CheckIsValidEndDateAndStartDate](@startDateCurrAbsenteeism, DATEADD(DAY, -1, @endDateCurrAbsenteeism)) = 0)
							BEGIN 
								SET @ErrorMessage = 'End Date cannot be less than Start Date';  SET @ErrorState = 1; SET @ErrorSeverity = 16;
								RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState); 
							END;

							UPDATE ExePlantWorkerAbsenteeism
							SET 
								EndDateAbsent		= DATEADD(DAY, -1, @endDateCurrAbsenteeism)
							WHERE EmployeeID = @EmployeeID AND Shift = @Shift AND @StartDateAbsent BETWEEN StartDateAbsent AND EndDateAbsent;

							EXEC INSERT_WORKER_ABSENTEEISM_FROM_ENTRY @StartDateAbsent, @AbsentTypeDTO, @SktAbsentCode, @PayrollAbsentCode, @EmployeeID, @LocationCode, @UnitCode, @GroupCode, @Shift, @UpdatedDate, @UpdatedBy;
						END;
					END;
					ELSE
					BEGIN
						
						DELETE FROM ExePlantWorkerAbsenteeism
						WHERE EmployeeID = @EmployeeID AND Shift = @Shift AND @StartDateAbsent BETWEEN StartDateAbsent AND EndDateAbsent;

						-- Check data absenteeism before insert if end date less than start date
						IF([dbo].[CheckIsValidEndDateAndStartDate](@startDateCurrAbsenteeism, DATEADD(DAY, -1, @StartDateAbsent)) = 0)
						BEGIN 
							SET @ErrorMessage = 'End Date cannot be less than Start Date';  SET @ErrorState = 1; SET @ErrorSeverity = 16;
							RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState); 
						END;

						INSERT INTO [dbo].[ExePlantWorkerAbsenteeism]
							   ([StartDateAbsent]
							   ,[EmployeeID]
							   ,[AbsentType]
							   ,[EndDateAbsent]
							   ,[SktAbsentCode]
							   ,[PayrollAbsentCode]
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
						VALUES
							   (@startDateCurrAbsenteeism
							  ,@EmployeeID
							  ,@currAbsentType
							  ,DATEADD(DAY, -1, @StartDateAbsent)
							  ,@currSktAbsentCode
							  ,@currPayrollAbsentCode
							  ,@UpdatedDate 
							  ,@UpdatedBy
							  ,@UpdatedDate
							  ,@UpdatedBy
							  ,@employeeNumber
							  ,@LocationCode
							  ,@UnitCode
							  ,@GroupCode
							  ,CAST(GETDATE() as date)
							  ,@currShift)

						-- Check data absenteeism before insert if end date less than start date
						IF([dbo].[CheckIsValidEndDateAndStartDate](DATEADD(DAY, 1, @StartDateAbsent), @endDateCurrAbsenteeism) = 0)
						BEGIN 
							SET @ErrorMessage = 'End Date cannot be less than Start Date';  SET @ErrorState = 1; SET @ErrorSeverity = 16;
							RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState); 
						END;

						INSERT INTO [dbo].[ExePlantWorkerAbsenteeism]
							   ([StartDateAbsent]
							   ,[EmployeeID]
							   ,[AbsentType]
							   ,[EndDateAbsent]
							   ,[SktAbsentCode]
							   ,[PayrollAbsentCode]
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
						VALUES
							   (DATEADD(DAY, 1, @StartDateAbsent)
							  ,@EmployeeID
							  ,@currAbsentType
							  ,@endDateCurrAbsenteeism
							  ,@currSktAbsentCode
							  ,@PayrollAbsentCode
							  ,@UpdatedDate
							  ,@UpdatedBy
							  ,@UpdatedDate
							  ,@UpdatedBy
							  ,@employeeNumber
							  ,@currLocationCode
							  ,@currUnitCode
							  ,@currGroupCode
							  ,CAST(GETDATE() as date)
							  ,@currShift)

						  INSERT INTO [dbo].[ExePlantWorkerAbsenteeism]
							([StartDateAbsent]
							,[EmployeeID]
							,[AbsentType]
							,[EndDateAbsent]
							,[SktAbsentCode]
							,[PayrollAbsentCode]
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
						VALUES
						   (@StartDateAbsent
						   ,@EmployeeID
						   ,@AbsentTypeDTO
						   ,@StartDateAbsent
						   ,@SktAbsentCode
						   ,@PayrollAbsentCode
						   ,@UpdatedDate
						   ,@UpdatedBy
						   ,@UpdatedDate
						   ,@UpdatedBy
						   ,@employeeNumber
						   ,@LocationCode
						   ,@UnitCode
						   ,@GroupCode
						   ,CAST(GETDATE() as date)
						   ,@Shift)
					END;
				END;
			END;
			ELSE
			BEGIN
				EXEC INSERT_WORKER_ABSENTEEISM_FROM_ENTRY @StartDateAbsent, @AbsentTypeDTO, @SktAbsentCode, @PayrollAbsentCode, @EmployeeID, @LocationCode, @UnitCode, @GroupCode, @Shift, @UpdatedDate, @UpdatedBy;
			END;
		END;

		COMMIT TRANSACTION 
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION 
		SET @ErrorMessage  = ERROR_MESSAGE();
		SET	@ErrorState    = ERROR_STATE();
		SET @ErrorSeverity = ERROR_SEVERITY();

		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
		--SELECT @ERR_MSG = ERROR_MESSAGE(),
		--@ERR_STA = ERROR_STATE()
 
		--SET @ERR_MSG= 'Error occurred in store procedure: ' + @ERR_MSG;
 
		--THROW 50001, @ERR_MSG, @ERR_STA;
	END CATCH
END;