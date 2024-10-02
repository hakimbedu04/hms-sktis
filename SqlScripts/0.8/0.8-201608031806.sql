/****** Object:  StoredProcedure [dbo].[INSERT_WORKER_ASSIGNMENT]    Script Date: 8/2/2016 2:50:33 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[INSERT_WORKER_ASSIGNMENT]
	@SourceLocationCode			VARCHAR(8),
	@SourceUnitCode				VARCHAR(4),
	@SourceShift				INT,
	@SourceProcessGroup			VARCHAR(16),
	@SourceGroupCode			VARCHAR(4),
	@SourceBrandCode			VARCHAR(11),
	@DestinationLocationCode	VARCHAR(8),
	@DestinationUnitCode		VARCHAR(4),
	@DestinationShift			INT,
	@DestinationProcessGroup	VARCHAR(16),
	@DestinationGroupCode		VARCHAR(4),
	@DestinationGroupCodeDummy	VARCHAR(6),
	@DestinationBrandCode		VARCHAR(11),
	@EmployeeID					VARCHAR(64),
	@EmployeeNumber				VARCHAR(6),
	@StartDate					DATETIME,
	@EndDate					DATETIME,
	@CreatedBy					VARCHAR(64),
	@UpdatedBy					VARCHAR(64)
AS
BEGIN
	DECLARE @ERR_MSG AS NVARCHAR(4000)  ,@ERR_STA AS SMALLINT 

	BEGIN TRANSACTION 
	BEGIN TRY
		SET DATEFIRST 1

		BEGIN -- insert into worker assignment
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
		 VALUES
			   (CONVERT(date, GETDATE())  
			   ,@SourceLocationCode
			   ,@SourceUnitCode
			   ,@SourceShift
			   ,@SourceProcessGroup
			   ,@SourceGroupCode
			   ,@SourceBrandCode
			   ,@DestinationLocationCode
			   ,@DestinationUnitCode
			   ,@DestinationShift
			   ,@DestinationProcessGroup
			   ,@DestinationGroupCode
			   ,@DestinationGroupCodeDummy
			   ,@DestinationBrandCode
			   ,@EmployeeID
			   ,@EmployeeNumber
			   ,@StartDate
			   ,@EndDate
			   ,GETDATE()
			   ,@CreatedBy
			   ,GETDATE()
			   ,@UpdatedBy)
		END;
		
		DECLARE @AbsentType			VARCHAR(128);
		DECLARE @RemarkAbsent		VARCHAR(256);
		DECLARE @SKTAbsentCode		VARCHAR(128);
		DECLARE @PayrollAbsentCode	VARCHAR(128);
		
		-- Get value multiskill out from master absent type
		SELECT @AbsentType = AbsentType, @RemarkAbsent = Remark, @SKTAbsentCode = SktAbsentCode, @PayrollAbsentCode = PayrollAbsentCode  
		FROM MstPlantAbsentType WHERE AbsentType = 'Multiskill Out';

		DECLARE @CurrentDate DATETIME;
		DECLARE @Week INT;
		DECLARE @Year INT;

		DECLARE @ProductionEntryCodeSource VARCHAR(50); -- Production Entry Code Source
		DECLARE @ProductionEntryCodeDestination VARCHAR(50); -- Production Entry Code Destination
		DECLARE @ProductionEntryCodeDestinationDummy VARCHAR(50); -- Production Entry Code Destination Dummy
		
		-- Set CurrentDate = start date assignment
		SET @CurrentDate = @StartDate;

		-- Looping from CurrentDate (start data assignment) until end date assignment
		WHILE (@CurrentDate <= @EndDate)
		BEGIN
			-- Get year and week from master gen week by @CurrentDate
			SELECT @Week = [Week], @Year = [Year] 
			FROM MstGenWeek 
			WHERE @CurrentDate BETWEEN StartDate AND EndDate;

			-- Create Production Entry Code Source
			SET @ProductionEntryCodeSource = 'EBL' + '/' + @SourceLocationCode 
												   + '/' + CAST(@SourceShift as VARCHAR(1)) 
												   + '/' + @SourceUnitCode 
												   + '/' + @SourceGroupCode 
												   + '/' + @SourceBrandCode 
												   + '/' + CAST(@Year as VARCHAR(4))    
												   + '/' + CAST(@Week as VARCHAR(2)) 
												   + '/' + CONVERT(varchar,(select datepart(dw, @CurrentDate)));

			-- Create Production Entry Code Destination
			SET @ProductionEntryCodeDestination = 'EBL' + '/' + @DestinationLocationCode 
														+ '/' + CAST(@DestinationShift as VARCHAR(1))  
														+ '/' + @DestinationUnitCode 
														+ '/' + @DestinationGroupCode 
														+ '/' + @DestinationBrandCode 
														+ '/' + CAST(@Year as VARCHAR(4))    
													    + '/' + CAST(@Week as VARCHAR(2)) 
														+ '/' + CONVERT(varchar,(select datepart(dw, @CurrentDate)));
			
			-- Create Production Entry Code Destination Dummy
			SET @ProductionEntryCodeDestinationDummy = 'EBL' + '/' + @DestinationLocationCode 
															 + '/' + CAST(@DestinationShift as VARCHAR(1))   
															 + '/' + @DestinationUnitCode 
															 + '/' + @DestinationGroupCodeDummy 
															 + '/' + @DestinationBrandCode 
															 + '/' + CAST(@Year as VARCHAR(4))    
															 + '/' + CAST(@Week as VARCHAR(2))   
															 + '/' + CONVERT(varchar,(select datepart(dw, @CurrentDate)));
			
			IF((SELECT dbo.GetParentLocationLastChild(@DestinationLocationCode)) = 'TPO')
			BEGIN /** Assignment to TPO Destination Location **/
				
				-- Get value tugas luar from master absent type
				SELECT @AbsentType = AbsentType, @RemarkAbsent = Remark, @SKTAbsentCode = SktAbsentCode, @PayrollAbsentCode = PayrollAbsentCode  
				FROM MstPlantAbsentType
				WHERE AbsentType = 'Tugas Luar';
				
				-- update source production entry Tugas Luar
				UPDATE ExePlantProductionEntry 
				SET 
					AbsentType = @AbsentType, 
					AbsentCodeEblek = @SKTAbsentCode, 
					AbsentCodePayroll = @PayrollAbsentCode, 
					AbsentRemark = @RemarkAbsent,
					ProdActual = 0,
					ProdTarget = 0
				WHERE ProductionEntryCode = @ProductionEntryCodeSource and EmployeeID = @EmployeeID;
			END;
			ELSE
			BEGIN /** Assignment to PLANT Destination Location **/
				
				-- update source production entry multiskill out
				UPDATE ExePlantProductionEntry 
				SET 
					AbsentType = @AbsentType, 
					AbsentCodeEblek = @SKTAbsentCode, 
					AbsentCodePayroll = @PayrollAbsentCode,
					AbsentRemark = @RemarkAbsent
				WHERE ProductionEntryCode = @ProductionEntryCodeSource and EmployeeID = @EmployeeID;

				-- Get Process order from destination process
				DECLARE @processOrder INT;
				SELECT @processOrder = ProcessOrder  FROM MstGenProcess 
				WHERE ProcessGroup = @DestinationProcessGroup;
				
				-- Check existing production entry verification dummy group
				DECLARE @countEntryVerificationDummy INT;
				SELECT @countEntryVerificationDummy = COUNT(*) 
				FROM ExePlantProductionEntryVerification 
				WHERE ProductionEntryCode = @ProductionEntryCodeDestinationDummy;

				IF(@countEntryVerificationDummy = 0)
				BEGIN /** create entry verification dummy **/
					IF(@SourceProcessGroup <> 'DAILY')
					BEGIN /** process group source is not DAILY **/
						INSERT INTO [dbo].[ExePlantProductionEntryVerification]
							   ([ProductionEntryCode]
							   ,[LocationCode]
							   ,[UnitCode]
							   ,[Shift]
							   ,[ProcessGroup]
							   ,[ProcessOrder]
							   ,[GroupCode]
							   ,[BrandCode]
							   ,[KPSYear]
							   ,[KPSWeek]
							   ,[ProductionDate]
							   ,[WorkHour]
							   ,[TPKValue]
							   ,[TotalTargetValue]
							   ,[TotalActualValue]
							   ,[TotalCapacityValue]
							   ,[VerifySystem]
							   ,[VerifyManual]
							   ,[Remark]
							   ,[CreatedDate]
							   ,[CreatedBy]
							   ,[UpdatedDate]
							   ,[UpdatedBy]
							   ,[Flag_Manual])
							SELECT
								@ProductionEntryCodeDestinationDummy as ProductionEntryCode
							   ,@DestinationLocationCode as LocationCode
							   ,@DestinationUnitCode as UnitCode
							   ,@DestinationShift as Shift
							   ,@DestinationProcessGroup as ProcessGroup
							   ,@processOrder as ProcessOrder
							   ,@DestinationGroupCodeDummy as GroupCode
							   ,@DestinationBrandCode as BrandCode
							   ,@Year as KPSYear
							   ,@Week as KPSWeek
							   ,@CurrentDate as ProductionDate
							   ,WorkHour
							   ,0
							   ,0
							   ,0
							   ,TotalCapacityValue
							   ,VerifySystem
							   ,VerifyManual
							   ,Remark
							   ,GETDATE() as CreatedDate
							   ,@CreatedBy as CreatedBy
							   ,GETDATE() as UpdatedDate
							   ,@UpdatedBy as UpdatedBy
							   ,Flag_Manual
							FROM ExePlantProductionEntryVerification WHERE ProductionEntryCode = @ProductionEntryCodeDestination
					END;
					ELSE IF(@SourceProcessGroup = 'DAILY')
					BEGIN /** process group source = DAILY **/
						INSERT INTO [dbo].[ExePlantProductionEntryVerification]
								([ProductionEntryCode]
								,[LocationCode]
								,[UnitCode]
								,[Shift]
								,[ProcessGroup]
								,[ProcessOrder]
								,[GroupCode]
								,[BrandCode]
								,[KPSYear]
								,[KPSWeek]
								,[ProductionDate]
								,[WorkHour]
								,[TPKValue]
								,[TotalTargetValue]
								,[TotalActualValue]
								,[TotalCapacityValue]
								,[VerifySystem]
								,[VerifyManual]
								,[Remark]
								,[CreatedDate]
								,[CreatedBy]
								,[UpdatedDate]
								,[UpdatedBy]
								,[Flag_Manual])
						VALUES
								(@ProductionEntryCodeDestinationDummy
								,@DestinationLocationCode
								,@DestinationUnitCode
								,@DestinationShift
								,@DestinationProcessGroup
								,@processOrder
								,@DestinationGroupCodeDummy
								,@DestinationBrandCode
								,@Year
								,@Week
								,@CurrentDate
								,0
								,0
								,0
								,0
								,0
								,0
								,0
								,NULL
								,GETDATE()
								,@CreatedBy
								,GETDATE()
								,@UpdatedBy
								,0)
					END;
				END;

				DECLARE @statusEmp			VARCHAR(50);
				DECLARE @statusIdentifier	CHAR(1); 
				DECLARE @prodCap			DECIMAL(18,3);
				DEClARE @prodActual			REAL;
				DECLARE @prodTarget			REAL;

				IF(LOWER(@SourceProcessGroup) = LOWER(@DestinationProcessGroup))
				BEGIN /** assignment to same process**/
					SET @statusEmp = 'Resmi';
					SET @prodActual = NULL;
					SET @prodTarget = NULL;
					
					-- Get value from source production entry
					SELECT @prodCap = ProdCapacity
					FROM ExePlantProductionEntry 
					WHERE ProductionEntryCode = @ProductionEntryCodeSource AND EmployeeID = @EmployeeID;
				END;
				ELSE
				BEGIN /** assignment to different process**/	
					SET @statusEmp = 'Multiskill';
					SET @prodActual = NULL;
					SET @prodTarget = NULL;
					
					DECLARE @brandGroupCode VARCHAR(20);
					DECLARE @workHour INT;
					
					-- Get workhour from destination entry verification
					SELECT @workHour = WorkHour 
					FROM ExePlantProductionEntryVerification 
					WHERE ProductionEntryCode = @ProductionEntryCodeDestination;
					
					-- Get brand group code by brandcode
					SELECT @brandGroupCode = BrandGroupCode 
					FROM MstgenBrand 
					WHERE BrandCode = @DestinationBrandCode;

					-- Set Prod Capacity value by HoursCapacity from PlantIndividualWorkHours
					SELECT @prodCap = CASE WHEN @workHour = 3 THEN HoursCapacity3 
										   WHEN @workHour = 5 THEN HoursCapacity5 
										   WHEN @workHour = 6 THEN HoursCapacity6
										   WHEN @workHour = 7 THEN HoursCapacity7
										   WHEN @workHour = 8 THEN HoursCapacity8
										   WHEN @workHour = 9 THEN HoursCapacity9
										   WHEN @workHour = 10 THEN HoursCapacity10 END 
					FROM PlanPlantIndividualCapacityWorkHours 
					WHERE BrandGroupCode = @brandGroupCode AND GroupCode = @DestinationGroupCode AND 
						  UnitCode = @DestinationUnitCode AND LocationCode = @DestinationLocationCode AND ProcessGroup = @DestinationProcessGroup

				END;

				-- Set statusIdentifier by status employee
				SELECT @statusIdentifier = StatusIdentifier 
				FROM MstGenEmpStatus 
				WHERE StatusEmp = @statusEmp;

				-- Check existing production entry dummy
				DECLARE @countEntryDummy INT;
				SELECT @countEntryDummy = COUNT(*) FROM ExePlantProductionEntry WHERE ProductionEntryCode = @ProductionEntryCodeDestinationDummy AND EmployeeID = @EmployeeID;
				IF(@countEntryDummy = 0)
				BEGIN /** Insert production entry dummy **/
					INSERT INTO [dbo].[ExePlantProductionEntry]
					   ([ProductionEntryCode]
					   ,[EmployeeID]
					   ,[EmployeeNumber]
					   ,[StatusEmp]
					   ,[StatusIdentifier]
					   ,[StartDateAbsent]
					   ,[AbsentType]
					   ,[ProdCapacity]
					   ,[ProdTarget]
					   ,[ProdActual]
					   ,[AbsentRemark]
					   ,[AbsentCodeEblek]
					   ,[AbsentCodePayroll]
					   ,[CreatedDate]
					   ,[CreatedBy]
					   ,[UpdatedDate]
					   ,[UpdatedBy]
					   ,[IsFromAbsenteeism])
					VALUES
					   (@ProductionEntryCodeDestinationDummy
					   ,@EmployeeID
					   ,@EmployeeNumber
					   ,@statusEmp
					   ,@statusIdentifier
					   ,NULL
					   ,NULL
					   ,@prodCap
					   ,@prodTarget
					   ,@prodActual
					   ,NULL
					   ,NULL
					   ,NULL
					   ,GETDATE()
					   ,@CreatedBy
					   ,GETDATE()
					   ,@UpdatedBy
					   ,0)
				END;
				ELSE
				BEGIN /** update production entry dummy **/
					UPDATE [dbo].[ExePlantProductionEntry]
					SET 
					  [StartDateAbsent] = NULL
					  ,[AbsentType] = NULL
					  ,[ProdCapacity] = @prodCap
					  ,[ProdTarget] = @prodTarget
					  ,[ProdActual] = @prodActual
					  ,[AbsentRemark] = NULL
					  ,[AbsentCodeEblek] = NULL
					  ,[AbsentCodePayroll] = NULL
					  ,[UpdatedDate] = GETDATE()
					  ,[UpdatedBy] = @UpdatedBy
					WHERE ProductionEntryCode = @ProductionEntryCodeDestinationDummy AND EmployeeID = @EmployeeID;
				END;

				DECLARE @totalActualDummy REAL;
				DECLARE @totalTargetDummy REAL;

				SELECT 
					@totalActualDummy = SUM(ProdActual), 
					@totalTargetDummy = SUM(ProdTarget) 
				FROM ExePlantProductionEntry 
				WHERE ProductionEntryCode = @ProductionEntryCodeDestinationDummy;

				UPDATE ExePlantProductionEntryVerification
				SET
					TotalActualValue = @totalActualDummy,
					TotalTargetValue = @totalTargetDummy
				WHERE ProductionEntryCode = @ProductionEntryCodeDestinationDummy;

				DELETE FROM WorkerAssignmentRemoval WHERE ProductionEntryCode = @ProductionEntryCodeDestinationDummy AND EmployeeID = @EmployeeID AND StartDate = @StartDate AND @EndDate = @EndDate;
			END;

			SET @CurrentDate = DATEADD(DAY, 1, @CurrentDate); /*increment current date*/

		END;
		
		COMMIT TRANSACTION 
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION 
		DECLARE @ErrorMessage   NVARCHAR(1000) = ERROR_MESSAGE(),
				@ErrorState     INT = ERROR_STATE(),
				@ErrorSeverity  INT = ERROR_SEVERITY();

		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
		--SELECT @ERR_MSG = ERROR_MESSAGE(),
		--@ERR_STA = ERROR_STATE()
 
		--SET @ERR_MSG= 'Error occurred in store procedure: ' + @ERR_MSG;
 
		--THROW 50001, @ERR_MSG, @ERR_STA;
	END CATCH
END;