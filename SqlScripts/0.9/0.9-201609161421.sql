/****** Object:  StoredProcedure [dbo].[CHECK_AND_FIX_ASSIGNMENT_DEST]    Script Date: 9/16/2016 4:08:42 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[CHECK_AND_FIX_ASSIGNMENT_DEST]
(@years INT, @weeks INT,  @locationCode VARCHAR(4), @unitCode VARCHAR(4))
AS
BEGIN
BEGIN TRANSACTION 
BEGIN TRY

DECLARE	   @SourceLocationCode			VARCHAR(8);
DECLARE	   @SourceUnitCode				VARCHAR(4);
DECLARE	   @SourceShift					INT;
DECLARE	   @SourceGroupCode				VARCHAR(4);
DECLARE	   @SourceBrandCode				VARCHAR(11);
DECLARE	   @DestinationLocationCode		VARCHAR(50);
DECLARE    @DestinationUnitCode         VARCHAR(50);
DECLARE    @DestinationBrandCode        VARCHAR(50);
DECLARE	   @DestinationGroupCodeDummy	VARCHAR(50);
DECLARE    @DestinationShift			INT;
DECLARE    @Year						INT;
DECLARE    @Week						INT;
DECLARE    @StartDate					DATETIME;
DECLARE	   @EndDate						DATETIME;
DEClARE	   @CurrDate					DATETIME;
DECLARE    @ProductionEntryCodeDestinationDummy VARCHAR(50);
DECLARE	   @ProductionEntryCodeSource			VARCHAR(50);
DECLARE	   @ProductionEntryCodeDestination		VARCHAR(50);
DECLARE	   @EmployeeID							VARCHAR(64);
DECLARE    @SourceProcessGroup					VARCHAR(16);
DECLARE    @DestinationProcessGroup				VARCHAR(16);
DECLARE    @statusEmp							VARCHAR(50);
DECLARE    @DestinationGroupCode				VARCHAR(4);
DECLARE	   @EmployeeNumber						VARCHAR(6);

DECLARE @MyCursor CURSOR;
BEGIN
	DECLARE @startdatex DATETIME;
	DECLARE @enddatex DATETIME;

	SELECT @startdatex = StartDate, @enddatex = EndDate
	FROM MstGenWeek 
	WHERE [Year] = @years and [Week] = @weeks

	SET @MyCursor = CURSOR FOR
	SELECT 
		DestinationLocationCode, 
		DestinationUnitCode, 
		DestinationShift, 
		DestinationBrandCode, 
		DestinationGroupCodeDummy, 
		StartDate,
		EndDate,
		EmployeeID,
		SourceLocationCode,
		SourceGroupCode,
		SourceShift,
		SourceUnitCode,
		SourceBrandCode,
		DestinationProcessGroup,
		SourceProcessGroup,
		DestinationGroupCode,
		EmployeeNumber
	FROM 
	(
		SELECT * FROM ExePlantWorkerAssignment a 
		WHERE (a.StartDate <= @startdatex AND a.EndDate >= @enddatex) 
		UNION
		SELECT * FROM ExePlantWorkerAssignment a 
		WHERE (a.StartDate >= @startdatex AND a.EndDate <= @enddatex) 
		UNION
		SELECT * FROM ExePlantWorkerAssignment a 
		WHERE (a.EndDate >= @startdatex AND a.StartDate <= @enddatex) 
		UNION
		SELECT * FROM ExePlantWorkerAssignment a 
		WHERE (a.EndDate >= @startdatex AND a.StartDate >= @enddatex)
	) 
	as assignment
	WHERE (SourceLocationCode = @locationCode and SourceUnitCode = @unitCode) OR (DestinationLocationCode = @locationCode AND DestinationUnitCode = @unitCode)

	OPEN @MyCursor
	FETCH NEXT FROM @MyCursor
	INTO @DestinationLocationCode, @DestinationUnitCode, @DestinationShift, @DestinationBrandCode, @DestinationGroupCodeDummy, @StartDate, @EndDate, @EmployeeID,
	@SourceLocationCode, @SourceGroupCode, @SourceShift, @SourceUnitCode, @SourceBrandCode, @DestinationProcessGroup, @SourceProcessGroup, @DestinationGroupCode,
	@EmployeeNumber

	WHILE @@FETCH_STATUS = 0
	BEGIN
		
		-- Check startdate / enddate assignment
		IF(@StartDate <= @startdatex)
		BEGIN
			SET @StartDate = @startdatex;
		END
		
		IF(@EndDate >= @enddatex)
		BEGIN
			SET @EndDate = @enddatex;
		END

		SET @CurrDate = @StartDate;
		WHILE (@CurrDate <= @EndDate)
		BEGIN
			SET DATEFIRST 1
			DECLARE @AbsentType			VARCHAR(128);
			DECLARE @SKTAbsentCode		VARCHAR(128);
			DECLARE @PayrollAbsentCode	VARCHAR(128);
		
			-- Get value multiskill out from master absent type
			SELECT @AbsentType = AbsentType, @SKTAbsentCode = SktAbsentCode, @PayrollAbsentCode = PayrollAbsentCode  
			FROM MstPlantAbsentType WHERE AbsentType = 'Multiskill Out';

			-- Create Production Entry Code Source
			SET @ProductionEntryCodeSource = 'EBL' + '/' + @SourceLocationCode 
												   + '/' + CAST(@SourceShift as VARCHAR(1)) 
												   + '/' + @SourceUnitCode 
												   + '/' + @SourceGroupCode 
												   + '/' + @SourceBrandCode 
												   + '/' + CAST(@years as VARCHAR(4))    
												   + '/' + CAST(@weeks as VARCHAR(2)) 
												   + '/' + CONVERT(varchar,(select datepart(dw, @CurrDate)));
			
			DECLARE @currAbsent VARCHAR(128);
			SELECT @currAbsent = AbsentType FROM ExePlantProductionEntry
			WHERE ProductionEntryCode = @ProductionEntryCodeSource and EmployeeID = @EmployeeID;

			IF(ISNULL(@currAbsent, '') <> 'Multiskill Out')
			BEGIN
				-- Update source production entry multiskill out
				UPDATE ExePlantProductionEntry 
				SET 
					AbsentType = @AbsentType, 
					AbsentCodeEblek = @SKTAbsentCode, 
					AbsentCodePayroll = @PayrollAbsentCode,
					StartDateAbsent = @CurrDate
				WHERE ProductionEntryCode = @ProductionEntryCodeSource and EmployeeID = @EmployeeID;
			END

			-- Create Production Entry Code Destination
			SET @ProductionEntryCodeDestination = 'EBL' + '/' + @DestinationLocationCode 
												+ '/' + CAST(@DestinationShift as VARCHAR(1))  
												+ '/' + @DestinationUnitCode 
												+ '/' + @DestinationGroupCode 
												+ '/' + @DestinationBrandCode 
												+ '/' + CAST(@years as VARCHAR(4))    
											    + '/' + CAST(@weeks as VARCHAR(2)) 
												+ '/' + CONVERT(varchar,(select datepart(dw, @CurrDate)));

			-- Create Production Entry Code Destination Dummy
			SET @ProductionEntryCodeDestinationDummy = 'EBL' + '/' + @DestinationLocationCode 
													 + '/' + CAST(@DestinationShift as VARCHAR(1))   
													 + '/' + @DestinationUnitCode 
													 + '/' + @DestinationGroupCodeDummy 
													 + '/' + @DestinationBrandCode 
													 + '/' + CAST(@years as VARCHAR(4))    
													 + '/' + CAST(@weeks as VARCHAR(2))   
													 + '/' + CONVERT(varchar,(select datepart(dw, @CurrDate)));

			IF EXISTS
			(
				SELECT * FROM ExePlantProductionEntryVerification where ProductionEntryCode = @ProductionEntryCodeDestination
			)
			BEGIN
				IF NOT EXISTS 
				(
					SELECT * FROM ExePlantProductionEntryVerification where ProductionEntryCode = @ProductionEntryCodeDestinationDummy
				)
				BEGIN /** create entry verification dummy **/
					-- Get Process order from destination process
					DECLARE @processOrder INT;
					SELECT @processOrder = ProcessOrder  FROM MstGenProcess 
					WHERE ProcessGroup = @DestinationProcessGroup;

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
							   ,@years as KPSYear
							   ,@weeks as KPSWeek
							   ,@CurrDate as ProductionDate
							   ,WorkHour
							   ,0
							   ,0
							   ,0
							   ,TotalCapacityValue
							   ,VerifySystem
							   ,VerifyManual
							   ,Remark
							   ,GETDATE() as CreatedDate
							   ,'SYSTEM' as CreatedBy
							   ,GETDATE() as UpdatedDate
							   ,'SYSTEM' as UpdatedBy
							   ,Flag_Manual
							FROM ExePlantProductionEntryVerification WHERE ProductionEntryCode = @ProductionEntryCodeDestination
					END;
					ELSE IF(@SourceProcessGroup = 'DAILY' or @SourceProcessGroup = 'GROUP LEADER' or @SourceProcessGroup = 'GENERAL WORKER')
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
								,@years
								,@weeks
								,@CurrDate
								,0
								,0
								,0
								,0
								,0
								,0
								,0
								,NULL
								,GETDATE()
								,'SYSTEM'
								,GETDATE()
								,'SYSTEM'
								,0)
					END;
				END;
				ELSE
				BEGIN
					IF NOT EXISTS
					(
						SELECT * FROM ExePlantProductionEntry e
						WHERE e.EmployeeID = @EmployeeID and e.ProductionEntryCode = @ProductionEntryCodeDestinationDummy
					)
					BEGIN
						DECLARE @prodCap DECIMAL(18,3);
						IF(LOWER(@SourceProcessGroup) = LOWER(@DestinationProcessGroup))
						BEGIN /** assignment to same process**/
							SET @statusEmp = 'Resmi';
						
							-- Get value from source production entry
							SELECT @prodCap = ProdCapacity
							FROM ExePlantProductionEntry 
							WHERE ProductionEntryCode = @ProductionEntryCodeSource AND EmployeeID = @EmployeeID;
						END;
						ELSE
						BEGIN /** assignment to different process**/	
							SET @statusEmp = 'Multiskill';
						
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

						DECLARE @statusIdentifier	CHAR(1); 
						-- Set statusIdentifier by status employee
						SELECT @statusIdentifier = StatusIdentifier 
						FROM MstGenEmpStatus 
						WHERE StatusEmp = @statusEmp;

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
							   ,@prodCap
							   ,NULL
							   ,NULL
							   ,NULL
							   ,NULL
							   ,GETDATE()
							   ,'SYSTEM'
							   ,GETDATE()
							   ,'SYSTEM'
							   ,0)

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
					END;
				END;
			END;
			
			SET @CurrDate = DATEADD(DAY, 1, @CurrDate); /*increment current date*/
		END;

		FETCH NEXT FROM @MyCursor
		INTO @DestinationLocationCode, @DestinationUnitCode, @DestinationShift, @DestinationBrandCode, @DestinationGroupCodeDummy, @StartDate, @EndDate, @EmployeeID,
		@SourceLocationCode, @SourceGroupCode, @SourceShift, @SourceUnitCode, @SourceBrandCode, @DestinationProcessGroup, @SourceProcessGroup, @DestinationGroupCode,
		@EmployeeNumber
  
    END; 

    CLOSE @MyCursor ;
    DEALLOCATE @MyCursor;
END;
	COMMIT TRANSACTION 
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION 
		DECLARE @ErrorMessage   NVARCHAR(1000) = ERROR_MESSAGE(),
				@ErrorState     INT = ERROR_STATE(),
				@ErrorSeverity  INT = ERROR_SEVERITY();

		RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
	END CATCH
END
