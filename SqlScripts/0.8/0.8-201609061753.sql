/****** Object:  StoredProcedure [dbo].[INSERT_WORKER_ASSIGNMENT_BYSYSTEM]    Script Date: 9/6/2016 4:43:25 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[INSERT_WORKER_ASSIGNMENT_BYSYSTEM]
(@years INT, @weeks INT, @locationCode VARCHAR(4), @unitCode VARCHAR(4))
AS
BEGIN
	declare @startdatex datetime
	declare @enddatex datetime
	select @startdatex = StartDate,@enddatex = EndDate from MstGenWeek where Year = @years and Week = @weeks
	SET DATEFIRST 1
	DECLARE @Employee VARCHAR(50)

	DECLARE @temp TABLE (TransactionDate	datetime,
		SourceLocationCode	varchar(8),
		SourceUnitCode	varchar(4),
		SourceShift	int,
		SourceProcessGroup	varchar(16),
		SourceGroupCode	varchar(4),
		SourceBrandCode	varchar(11),
		DestinationLocationCode	varchar(8),
		DestinationUnitCode	varchar(4),
		DestinationShift	int,
		DestinationProcessGroup	varchar(16),
		DestinationGroupCode	varchar(4),
		DestinationGroupCodeDummy	varchar(6),
		DestinationBrandCode	varchar(11),
		EmployeeID	varchar(64),
		EmployeeNumber	varchar(6),
		StartDate	datetime,
		EndDate	datetime,
		CreatedDate	datetime,
		CreatedBy	varchar(64),
		UpdatedDate	datetime,
		UpdatedBy	varchar(64)
	)

	INSERT INTO @temp
	SELECT * FROM (
		SELECT * FROM ExePlantWorkerAssignment a 
		WHERE (a.StartDate <= @startdatex AND a.EndDate >= @enddatex) AND SourceLocationCode = @locationCode and SourceUnitCode = @unitCode
		UNION
		SELECT * FROM ExePlantWorkerAssignment a 
		WHERE (a.StartDate >= @startdatex AND a.EndDate <= @enddatex) AND SourceLocationCode = @locationCode and SourceUnitCode = @unitCode
		UNION
		SELECT * FROM ExePlantWorkerAssignment a 
		WHERE (a.EndDate >= @startdatex AND a.StartDate <= @enddatex) AND SourceLocationCode = @locationCode and SourceUnitCode = @unitCode
		UNION
		SELECT * FROM ExePlantWorkerAssignment a 
		WHERE (a.EndDate >= @startdatex AND a.StartDate >= @enddatex) AND SourceLocationCode = @locationCode and SourceUnitCode = @unitCode
	) AS final

	DECLARE @SourceLocationCode VARCHAR(8)
	DECLARE @SourceUnitCode VARCHAR(4)
	DECLARE @SourceShift INT
	DECLARE @SourceProcessGroup VARCHAR(16)
	DECLARE @SourceGroupCode VARCHAR(4)
	DECLARE @SourceBrandCode VARCHAR(11)
	DECLARE @DestinationLocationCode VARCHAR(8)
	DECLARE @DestinationUnitCode VARCHAR(4)
	DECLARE @DestinationShift INT
	DECLARE @DestinationProcessGroup VARCHAR(16)
	DECLARE @DestinationGroupCode VARCHAR(4)
	DECLARE @DestinationGroupCodeDummy VARCHAR(6)
	DECLARE @DestinationBrandCode VARCHAR(11)
	DECLARE @EmployeeID VARCHAR(64)
	DECLARE @EmployeeNumber VARCHAR(6)
	DECLARE @StartDate DATETIME
	DECLARE @EndDate DATETIME
	DECLARE @CreatedBy VARCHAR(64)
	DECLARE @UpdatedBy VARCHAR(64)

	SET @CreatedBy = 'SYSTEM';
	SET @UpdatedBy = 'SYSTEM';

	DECLARE MY_CURSOR CURSOR LOCAL STATIC READ_ONLY FORWARD_ONLY FOR 
	SELECT EmployeeID, SourceLocationCode, SourceUnitCode, SourceShift, SourceProcessGroup, SourceGroupCode, SourceBrandCode, DestinationLocationCode, DestinationUnitCode, DestinationShift, DestinationProcessGroup, DestinationGroupCode, DestinationGroupCodeDummy, DestinationBrandCode, EmployeeID, EmployeeNumber, StartDate, EndDate 
	FROM @temp
	OPEN MY_CURSOR
	FETCH NEXT FROM MY_CURSOR INTO @Employee, @SourceLocationCode, @SourceUnitCode, @SourceShift, @SourceProcessGroup, @SourceGroupCode, @SourceBrandCode, @DestinationLocationCode, @DestinationUnitCode, @DestinationShift, @DestinationProcessGroup, @DestinationGroupCode, @DestinationGroupCodeDummy, @DestinationBrandCode, @EmployeeID, @EmployeeNumber, @StartDate, @EndDate
	WHILE @@FETCH_STATUS = 0 BEGIN 

		--SELECT @SourceLocationCode = SourceLocationCode, @SourceUnitCode = SourceUnitCode, @SourceShift = SourceShift, @SourceProcessGroup = SourceProcessGroup, @SourceGroupCode = SourceGroupCode, @SourceBrandCode = SourceBrandCode, @DestinationLocationCode = DestinationLocationCode, @DestinationUnitCode = DestinationUnitCode, @DestinationShift = DestinationShift, @DestinationProcessGroup = DestinationProcessGroup, @DestinationGroupCode = DestinationGroupCode, @DestinationGroupCodeDummy = DestinationGroupCodeDummy, @DestinationBrandCode = DestinationBrandCode, @EmployeeID = EmployeeID, @EmployeeNumber = EmployeeNumber, @StartDate = StartDate, @EndDate = EndDate, @CreatedBy = 'SYSTEM', @UpdatedBy = 'SYSTEM'
		--FROM @temp WHERE EmployeeID = @Employee
	
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
		
			-- Check startdate / enddate assignment
			IF(@StartDate <= @startdatex)
			BEGIN
				SET @StartDate = @startdatex;
			END
			
			IF(@EndDate >= @enddatex)
			BEGIN
				SET @EndDate = @enddatex;
			END

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
						StartDateAbsent = @CurrentDate,
						ProdActual = 0,
						ProdTarget = 0
					WHERE ProductionEntryCode = @ProductionEntryCodeSource and EmployeeID = @EmployeeID;
				END;
				ELSE
				BEGIN /** Assignment to PLANT Destination Location **/

					-- Condition if verification destination is exists then create dummy verification and entry
					IF EXISTS
					(
						SELECT * FROM ExePlantProductionEntryVerification WHERE ProductionEntryCode = @ProductionEntryCodeDestination
					)
					BEGIN
						-- update source production entry multiskill out
						UPDATE ExePlantProductionEntry 
						SET 
							AbsentType = @AbsentType, 
							AbsentCodeEblek = @SKTAbsentCode, 
							AbsentCodePayroll = @PayrollAbsentCode,
							AbsentRemark = @RemarkAbsent,
							StartDateAbsent = @CurrentDate
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
				END;

				SET @CurrentDate = DATEADD(DAY, 1, @CurrentDate);
				END;

	FETCH NEXT FROM MY_CURSOR INTO @Employee, @SourceLocationCode, @SourceUnitCode, @SourceShift, @SourceProcessGroup, @SourceGroupCode, @SourceBrandCode, @DestinationLocationCode, @DestinationUnitCode, @DestinationShift, @DestinationProcessGroup, @DestinationGroupCode, @DestinationGroupCodeDummy, @DestinationBrandCode, @EmployeeID, @EmployeeNumber, @StartDate, @EndDate
	END
	CLOSE MY_CURSOR
	DEALLOCATE MY_CURSOR
END