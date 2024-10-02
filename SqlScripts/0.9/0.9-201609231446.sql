/****** Object:  StoredProcedure [dbo].[INSERT_WORKER_ABSENTEEISM_FROM_ENTRY]    Script Date: 9/23/2016 2:44:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[INSERT_WORKER_ABSENTEEISM_FROM_ENTRY]
	@StartDateAbsent	DATETIME,
	@AbsentType			VARCHAR(128),
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

	BEGIN TRANSACTION trans2
	BEGIN TRY
		SET DATEFIRST 1

		-- Get Employee Number from MstPlantEmpJobsDataAll by Employee ID
		DECLARE @employeeNumber	VARCHAR(64);
		SELECT @employeeNumber = EmployeeNumber FROM MstPlantEmpJobsDataAll WHERE EmployeeID = @EmployeeID
		
		-- Check Previous day absenteeism with the same absent type
		DECLARE @previousDay DATETIME;
		DECLARE @countPreviousDayAbsenteeism INT;
		DECLARE @startDateAbsentPreviousAbsenteeism DATETIME;
		DECLARE @endDateAbsentPreviousAbsenteeism DATETIME;

		SET @previousDay = DATEADD(DAY, -1, CAST(@StartDateAbsent as date));
		
		SELECT @countPreviousDayAbsenteeism = COUNT(*) 
		FROM ExePlantWorkerAbsenteeism 
		WHERE EmployeeID = @EmployeeID AND Shift = @Shift AND EndDateAbsent = @previousDay AND AbsentType = @AbsentType;

		SELECT @startDateAbsentPreviousAbsenteeism = @StartDateAbsent, @endDateAbsentPreviousAbsenteeism = EndDateAbsent
		FROM ExePlantWorkerAbsenteeism
		WHERE EmployeeID = @EmployeeID AND Shift = @Shift AND EndDateAbsent = @previousDay AND AbsentType = @AbsentType;
		
		-- Check Next day absenteeism with the same absent type
		DECLARE @nextDay DATETIME;
		DECLARE @countNextDayAbsenteeism INT;
		DECLARE @startDateAbsentNextAbsenteeism DATETIME;
		DECLARE @endDateAbsentNextAbsenteeism DATETIME;

		SET @nextDay = DATEADD(DAY, 1, CAST(@StartDateAbsent as date));
		
		SELECT @countNextDayAbsenteeism = COUNT(*) 
		FROM ExePlantWorkerAbsenteeism 
		WHERE EmployeeID = @EmployeeID AND Shift = @Shift AND StartDateAbsent = @nextDay AND AbsentType = @AbsentType;

		SELECT @startDateAbsentNextAbsenteeism = @StartDateAbsent, @endDateAbsentNextAbsenteeism = EndDateAbsent
		FROM ExePlantWorkerAbsenteeism
		WHERE EmployeeID = @EmployeeID AND Shift = @Shift AND StartDateAbsent = @nextDay AND AbsentType = @AbsentType;

		IF([dbo].[CheckDateClosingPayrollOrHoliday](@StartDateAbsent, @LocationCode) = 0)
		BEGIN /** Checking if start date absent is no closing payroll or holiday **/
			IF([dbo].[CheckDateClosingPayrollOrHoliday](@previousDay, @LocationCode) = 1)
			BEGIN /** Checking if previous day is closing payroll or holiday **/
				IF (@countNextDayAbsenteeism > 0)
				BEGIN
					BEGIN -- insert into worker absenteeism
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
						   ,@AbsentType
						   ,@endDateAbsentNextAbsenteeism
						   ,@SktAbsentCode
						   ,@PayrollAbsentCode
						   ,GETDATE()
						   ,@UpdatedBy
						   ,GETDATE()
						   ,@UpdatedBy
						   ,@employeeNumber
						   ,@LocationCode
						   ,@UnitCode
						   ,@GroupCode
						   ,CAST(GETDATE() as date)
						   ,@Shift)

						   DELETE FROM ExePlantWorkerAbsenteeism 
						   WHERE EmployeeID = @EmployeeID AND Shift = @Shift AND StartDateAbsent = @nextDay AND AbsentType = @AbsentType;
					END;
				END;
				ELSE
				BEGIN
					DECLARE @countExistingAbsenteeism INT;
					DECLARE @existingStartDateAbsent DATETIME;
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
							AbsentType			= @AbsentType,
							EndDateAbsent		= @StartDateAbsent,
							SktAbsentCode		= @SktAbsentCode,
							PayrollAbsentCode	= @PayrollAbsentCode,
							CreatedDate			= GETDATE(),
							CreatedBy			= @UpdatedBy,
							UpdatedDate			= GETDATE(),
							UpdatedBy			= @UpdatedBy,
							LocationCode		= @LocationCode,
							UnitCode			= @UnitCode,
							GroupCode			= @GroupCode,
							TransactionDate		= CAST(GETDATE() as date),
							Shift				= @Shift,
							EmployeeNumber		= @employeeNumber
						WHERE StartDateAbsent = @StartDateAbsent AND EmployeeID = @EmployeeID AND Shift = @Shift;
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
						   ,@AbsentType
						   ,@StartDateAbsent
						   ,@SktAbsentCode
						   ,@PayrollAbsentCode
						   ,GETDATE()
						   ,@UpdatedBy
						   ,GETDATE()
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
				IF(@countPreviousDayAbsenteeism > 0 AND @countNextDayAbsenteeism > 0)
				BEGIN
					-- Check data absenteeism before insert if end date less than start date
					IF([dbo].[CheckIsValidEndDateAndStartDate](@startDateAbsentPreviousAbsenteeism, @endDateAbsentNextAbsenteeism) = 0)
					BEGIN 
						SET @ErrorMessage = 'End Date cannot be less than Start Date';  SET @ErrorState = 1; SET @ErrorSeverity = 16;
						RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState); 
					END;

					UPDATE ExePlantWorkerAbsenteeism 
					SET 
						EndDateAbsent = @endDateAbsentNextAbsenteeism,
						CreatedDate = GETDATE(),
						CreatedBy = @UpdatedBy,
						UpdatedDate = GETDATE(),
						UpdatedBy = @UpdatedBy,
						LocationCode = @LocationCode,
						UnitCode = @UnitCode,
						GroupCode = @GroupCode,
						TransactionDate = CAST(GETDATE() as date)
					WHERE EmployeeID = @EmployeeID AND Shift = @Shift AND EndDateAbsent = @previousDay AND AbsentType = @AbsentType;

					DELETE FROM ExePlantWorkerAbsenteeism WHERE EmployeeID = @EmployeeID AND Shift = @Shift AND StartDateAbsent = @nextDay AND AbsentType = @AbsentType;
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
						EndDateAbsent = @StartDateAbsent,
						CreatedDate = GETDATE(),
						CreatedBy = @UpdatedBy,
						UpdatedDate = GETDATE(),
						UpdatedBy = @UpdatedBy,
						LocationCode = @LocationCode,
						UnitCode = @UnitCode,
						GroupCode = @GroupCode,
						TransactionDate = CAST(GETDATE() as date)
					WHERE EmployeeID = @EmployeeID AND Shift = @Shift AND EndDateAbsent = @previousDay AND AbsentType = @AbsentType;
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
						   ,@AbsentType
						   ,@endDateAbsentNextAbsenteeism
						   ,@SktAbsentCode
						   ,@PayrollAbsentCode
						   ,GETDATE()
						   ,@UpdatedBy
						   ,GETDATE()
						   ,@UpdatedBy
						   ,@employeeNumber
						   ,@LocationCode
						   ,@UnitCode
						   ,@GroupCode
						   ,CAST(GETDATE() as date)
						   ,@Shift)

					DELETE FROM ExePlantWorkerAbsenteeism WHERE EmployeeID = @EmployeeID AND Shift = @Shift AND StartDateAbsent = @nextDay AND AbsentType = @AbsentType;
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
							AbsentType			= @AbsentType,
							EndDateAbsent		= @StartDateAbsent,
							SktAbsentCode		= @SktAbsentCode,
							PayrollAbsentCode	= @PayrollAbsentCode,
							CreatedDate			= GETDATE(),
							CreatedBy			= @UpdatedBy,
							UpdatedDate			= GETDATE(),
							UpdatedBy			= @UpdatedBy,
							LocationCode		= @LocationCode,
							UnitCode			= @UnitCode,
							GroupCode			= @GroupCode,
							TransactionDate		= CAST(GETDATE() as date),
							Shift				= @Shift,
							EmployeeNumber		= @employeeNumber
						WHERE StartDateAbsent = @StartDateAbsent AND EmployeeID = @EmployeeID AND Shift = @Shift;
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
						   ,@AbsentType
						   ,@StartDateAbsent
						   ,@SktAbsentCode
						   ,@PayrollAbsentCode
						   ,GETDATE()
						   ,@UpdatedBy
						   ,GETDATE()
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
				IF([dbo].[CheckIsValidEndDateAndStartDate](@startDateAbsentPreviousAbsenteeism, @StartDateAbsent) = 0)
				BEGIN 
					SET @ErrorMessage = 'End Date cannot be less than Start Date';  SET @ErrorState = 1; SET @ErrorSeverity = 16;
					RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState); 
				END;

				UPDATE ExePlantWorkerAbsenteeism 
				SET 
						EndDateAbsent = @StartDateAbsent,
						CreatedDate = GETDATE(),
						CreatedBy = @UpdatedBy,
						UpdatedDate = GETDATE(),
						UpdatedBy = @UpdatedBy,
						LocationCode = @LocationCode,
						UnitCode = @UnitCode,
						GroupCode = @GroupCode,
						TransactionDate = CAST(GETDATE() as date)	
				WHERE EmployeeID = @EmployeeID AND Shift = @Shift AND EndDateAbsent = @previousDay AND AbsentType = @AbsentType;
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
						AbsentType			= @AbsentType,
						EndDateAbsent		= @StartDateAbsent,
						SktAbsentCode		= @SktAbsentCode,
						PayrollAbsentCode	= @PayrollAbsentCode,
						CreatedDate			= GETDATE(),
						CreatedBy			= @UpdatedBy,
						UpdatedDate			= GETDATE(),
						UpdatedBy			= @UpdatedBy,
						LocationCode		= @LocationCode,
						UnitCode			= @UnitCode,
						GroupCode			= @GroupCode,
						TransactionDate		= CAST(GETDATE() as date),
						Shift				= @Shift,
						EmployeeNumber		= @employeeNumber
					WHERE StartDateAbsent = @StartDateAbsent AND EmployeeID = @EmployeeID AND Shift = @Shift;
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
					   ,@AbsentType
					   ,@StartDateAbsent
					   ,@SktAbsentCode
					   ,@PayrollAbsentCode
					   ,GETDATE()
					   ,@UpdatedBy
					   ,GETDATE()
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

		COMMIT TRANSACTION trans2
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION trans2
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