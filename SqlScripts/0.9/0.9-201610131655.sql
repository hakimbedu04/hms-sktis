IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[dbo].[GENERATE_PRODUCTION_CARD]'))
	DROP PROCEDURE [dbo].[GENERATE_PRODUCTION_CARD]
GO
/****** Object:  StoredProcedure [dbo].[GENERATE_PRODUCTION_CARD]    Script Date: 10/13/2016 3:50:43 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GENERATE_PRODUCTION_CARD]
(
	@LocationCode		VARCHAR(8),
	@UnitCode			VARCHAR(4),
	@Shift				INT,
	@BrandCode			VARCHAR(20),
	@kpsYear			INT,
	@kpsWeek			INT,
	@productionDate		DATETIME,
	@groups				VARCHAR(MAX),
	@userName			VARCHAR(20)
)
AS
BEGIN

BEGIN TRANSACTION trans
BEGIN TRY

	SET DATEFIRST 1;

	DECLARE @splitGroups VARCHAR(4);
	
	-- Set Day of Week
	DECLARE @day INT; SET @day = datepart(dw, @productionDate);

	-- Set brandgroupcode
	DECLARE @brandGroupCode VARCHAR(20); SELECT @brandGroupCode = BrandGroupCode FROM MstGenBrand WHERE BrandCode = @BrandCode;

	-- Set Jkn Hour
	DECLARE @JKNhour INT; SELECT TOP 1 @JKNhour = JknHour FROM MstGenStandardHours WHERE DayType = 'Non-Holiday' AND [Day] = @day  

	-- Loop group code
	DECLARE cursor_groups CURSOR LOCAL FOR SELECT splitdata from dbo.fnSplitString(@groups,'-')

	DECLARE @DestLocationCode	VARCHAR(8);
	DECLARE @DestUnitCode		VARCHAR(4);
	DECLARE @DestShift			INT;
	DECLARE @DestBrandCode		VARCHAR(20);
	DECLARE @DestProcess		VARCHAR(20);
	DECLARE @GroupDummy			VARCHAR(4);
	DECLARE @DestProdActual		REAL;
	DECLARE @SrcProdActual		REAL;
	DECLARE @SrcLocationCode	VARCHAR(8);
	DECLARE @SrcUnitCode		VARCHAR(4);
	DECLARE @SrcShift			INT;
	DECLARE @SrcBrandCode		VARCHAR(20);
	DECLARE @SrcProcess			VARCHAR(20);
	DECLARE @SrcGroupCode		VARCHAR(4);

	OPEN cursor_groups
	FETCH NEXT FROM cursor_groups   
	INTO @splitGroups

	WHILE @@FETCH_STATUS = 0  
	BEGIN  
		-- Set production entry code
		DECLARE @ProductionCode VARCHAR(200);
		SET @ProductionCode = 'EBL/' + @LocationCode 
         + '/' + CONVERT(varchar,@Shift) 
         + '/' + @UnitCode 
         + '/' + @splitGroups 
         + '/' + @BrandCode 
         + '/' + CONVERT(varchar,@kpsYear) 
         + '/' + CONVERT(varchar,@kpsWeek)
         + '/' + CONVERT(varchar,@day);

		 -- Set Production card code
		DECLARE @ProductionCardCode VARCHAR(200);
		SET @ProductionCardCode = 'WPC/' + @LocationCode 
         + '/' + CONVERT(varchar,@Shift) 
         + '/' + @UnitCode 
         + '/' + @splitGroups 
         + '/' + @BrandCode 
         + '/' + CONVERT(varchar,@kpsYear) 
         + '/' + CONVERT(varchar,@kpsWeek)
         + '/' + CONVERT(varchar,@day);

		DECLARE @employeeID VARCHAR(64);
		DECLARE @absentType VARCHAR(128);
		DECLARE @prodActual REAL;
		DECLARE @prodTarget REAL;
		DECLARE @sktAbsentCodeEblek VARCHAR(128);
		DECLARE @processGroup VARCHAR(16);
		DECLARE @employeeNumber VARCHAR(20);
		DECLARE @workHour INT;
		DECLARE @payrollAbsent VARCHAR(128);

		-- Loop through production entry
		DECLARE cursor_entry CURSOR LOCAL FOR
		SELECT e.EmployeeID, e.EmployeeNumber, v.WorkHour, ISNULL(e.ProdActual, 0), e.ProdTarget, ISNULL(e.AbsentType, ''), ISNULL(e.AbsentCodeEblek, ''), ISNULL(v.ProcessGroup, ''), ISNULL(e.AbsentCodePayroll, '')
		FROM ExePlantProductionEntry e
		inner join ExePlantProductionEntryVerification v on v.ProductionEntryCode = e.ProductionEntryCode 
		WHERE v.ProductionEntryCode = @ProductionCode

		OPEN cursor_entry
		FETCH NEXT FROM cursor_entry   
		INTO @employeeID, @employeeNumber, @workHour, @prodActual, @prodTarget, @absentType, @sktAbsentCodeEblek, @processGroup, @payrollAbsent

		WHILE @@FETCH_STATUS = 0  
		BEGIN  
			-- Declare for Paid Other and Production
			DECLARE @paidOther REAL;
			DECLARE @production REAL;

			-- Set Production = Prod Actual Entry
			SET @production = ISNULL(@prodActual, 0);
			SET @paidOther = 0;

			DECLARE @stdStickPerHour INT;
			DECLARE @uomEblek INT;
			SELECT TOP 1 @stdStickPerHour = StdStickPerHour, @uomEblek = UOMEblek FROM ProcessSettingsAndLocationView 
			WHERE BrandGroupCode = @brandGroupCode AND ProcessGroup = @processGroup

			-- Check group code source
			IF(SUBSTRING(@splitGroups, 2, 1) <> '5')
			BEGIN
				IF(ISNULL(@absentType, '') <> '')
				BEGIN
					-- Set calculation absent
					DECLARE @calculation VARCHAR(128);
					SELECT @calculation = Calculation FROM MstPlantAbsentType WHERE AbsentType = @absentType;

					IF(ISNULL(@calculation, '') = 'T')
					BEGIN
						SET @paidOther = ISNULL(@prodTarget, 0) - ISNULL(@prodActual, 0);
					END
					IF(ISNULL(@sktAbsentCodeEblek, '') <> 'MO')
					BEGIN
						-- Set paid other for different types of calculation
						IF(ISNULL(@calculation, '') = '40/6')
						BEGIN
							IF(ISNULL(@uomEblek,0) = 0)
							BEGIN
								SET @paidOther = 0 - @prodActual;
							END
							ELSE
							BEGIN
								SET @paidOther = ((40/6) * (@stdStickPerHour / @uomEblek)) - @prodActual;
							END
						END
						ELSE IF(ISNULL(@calculation, '') = 'JKN')
						BEGIN
							SET @paidOther = (@JKNhour * (@stdStickPerHour / @uomEblek)) -  @prodActual;
						END
					END
					IF(ISNULL(@sktAbsentCodeEblek, '') = 'MO')
					BEGIN
						-- Check Assignment
						IF EXISTS
						(
							SELECT * FROM ExePlantWorkerAssignment WHERE EmployeeID = @employeeID AND SourceLocationCode = @LocationCode
							AND SourceUnitCode = @UnitCode AND @productionDate BETWEEN StartDate AND EndDate AND SourceBrandCode = @BrandCode
							AND SourceShift = @Shift AND SourceProcessGroup = @processGroup and SourceGroupCode = @splitGroups
						)
						BEGIN
							SET @GroupDummy = SUBSTRING(@splitGroups, 1, 1) + '5' + SUBSTRING(@splitGroups, 3, 2);

							SELECT @DestLocationCode = DestinationLocationCode, @DestUnitCode = DestinationUnitCode, @DestShift = DestinationShift,
							@DestBrandCode = DestinationBrandCode, @DestProcess = DestinationProcessGroup
							FROM ExePlantWorkerAssignment WHERE EmployeeID = @employeeID AND SourceLocationCode = @LocationCode
							AND SourceUnitCode = @UnitCode AND @productionDate BETWEEN StartDate AND EndDate AND SourceBrandCode = @BrandCode
							AND SourceShift = @Shift and SourceGroupCode = @splitGroups

							SELECT @DestProdActual =  ISNULL(ProdActual, 0) FROM ExePlantProductionEntry e inner join ExePlantProductionEntryVerification v on v.ProductionEntryCode = e.ProductionEntryCode
							WHERE v.LocationCode = @DestLocationCode AND v.UnitCode = @DestUnitCode AND v.BrandCode = @DestBrandCode AND v.ProcessGroup = @DestProcess
							AND v.Shift = @DestShift AND v.GroupCode = @GroupDummy AND e.EmployeeID = @employeeID

							IF(ISNULL(@processGroup, '') = ISNULL(@DestProcess, ''))
							BEGIN
								SET @production = ISNULL(@prodActual, 0) + ISNULL(@DestProdActual, 0);
								SET @paidOther = 0;
							END
							ELSE
							BEGIN
								SET @paidOther = ISNULL(@DestProdActual, 0) - ISNULL(@prodActual, 0);
							END
						END
					END
				END
			END
			ELSE IF(SUBSTRING(@splitGroups, 2, 1) = '5')
			BEGIN
				SET @GroupDummy = SUBSTRING(@splitGroups, 1, 1) + '5' + SUBSTRING(@splitGroups, 3, 2);
				-- Check Assignment Destination
				IF EXISTS
				(
					SELECT * FROM ExePlantWorkerAssignment WHERE EmployeeID = @employeeID AND DestinationLocationCode = @LocationCode
					AND DestinationUnitCode = @UnitCode AND @productionDate BETWEEN StartDate AND EndDate AND DestinationBrandCode = @BrandCode
					AND DestinationShift = @Shift AND DestinationProcessGroup = @processGroup AND DestinationGroupCodeDummy = @GroupDummy
				)
				BEGIN
					SELECT @DestLocationCode = DestinationLocationCode, @DestUnitCode = DestinationUnitCode, @DestShift = DestinationShift,
					@DestBrandCode = DestinationBrandCode, @DestProcess = DestinationProcessGroup, @SrcLocationCode = SourceLocationCode,
					@SrcUnitCode = SourceUnitCode, @SrcShift = SourceShift, @SrcBrandCode = SourceBrandCode, @SrcGroupCode = SourceGroupCode,
					@SrcProcess = SourceProcessGroup
					FROM ExePlantWorkerAssignment WHERE EmployeeID = @employeeID AND DestinationLocationCode = @LocationCode
					AND DestinationUnitCode = @UnitCode AND @productionDate BETWEEN StartDate AND EndDate AND DestinationBrandCode = @BrandCode
					AND DestinationShift = @Shift AND DestinationProcessGroup = @processGroup AND DestinationGroupCodeDummy = @GroupDummy

					SELECT @DestProdActual = ISNULL(ProdActual, 0) FROM ExePlantProductionEntry e inner join ExePlantProductionEntryVerification v on v.ProductionEntryCode = e.ProductionEntryCode
					WHERE v.LocationCode = @DestLocationCode AND v.UnitCode = @DestUnitCode AND v.BrandCode = @DestBrandCode AND v.ProcessGroup = @DestProcess
					AND v.Shift = @DestShift AND v.GroupCode = @GroupDummy AND e.EmployeeID = @employeeID

					SELECT @SrcProdActual = ISNULL(ProdActual, 0) FROM ExePlantProductionEntry e inner join ExePlantProductionEntryVerification v on v.ProductionEntryCode = e.ProductionEntryCode
					WHERE v.LocationCode = @SrcLocationCode AND v.UnitCode = @SrcUnitCode AND v.BrandCode = @SrcBrandCode AND v.ProcessGroup = @SrcProcess
					AND v.Shift = @SrcShift AND v.GroupCode = @SrcGroupCode AND e.EmployeeID = @employeeID

					IF(ISNULL(@absentType, '') <> '')
					BEGIN
						IF(ISNULL(@absentType, '') = 'Pulang Pagi (Sakit)' OR ISNULL(@absentType, '') = 'Pulang Pagi (Mendadak) > 4 jam' OR ISNULL(@absentType, '') = 'Pulang Pagi (Ijin)')
						BEGIN
							DECLARE @srcBrandgroupCode VARCHAR(64);
							DECLARE @destBrandgroupCode VARCHAR(64);
							SELECT @srcBrandgroupCode = BrandGroupCode FROM MstGenBrand where BrandCode = @SrcBrandCode;
							SELECT @destBrandgroupCode = BrandGroupCode FROM MstGenBrand where BrandCode = @DestBrandCode;

							DECLARE @srcUomEblek INT;
							DECLARE @srcStdStickHour INT;
							DECLARE @destUomEblek INT;
							DECLARE @destStdStickHour INT;

							SELECT TOP 1 @srcUomEblek = ISNULL(UOMEblek, 0), @srcStdStickHour = ISNULL(StdStickPerHour, 0) FROM ProcessSettingsAndLocationView WHERE BrandGroupCode = @srcBrandgroupCode AND LocationCode = @SrcLocationCode AND ProcessGroup = @SrcProcess
							SELECT TOP 1 @destUomEblek = ISNULL(UOMEblek, 0), @destStdStickHour = ISNULL(StdStickPerHour, 0) FROM ProcessSettingsAndLocationView WHERE BrandGroupCode = @srcBrandgroupCode AND LocationCode = @SrcLocationCode AND ProcessGroup = @SrcProcess

							DECLARE @workHourSrc REAL;
							DECLARE @workHourDst REAL;
							SET @workHourSrc = CASE WHEN @srcUomEblek = 0 THEN 0 ELSE @prodActual / (@srcStdStickHour / @srcUomEblek) END;
							SET @workHourDst = CASE WHEN @destUomEblek = 0 THEN 0 ELSE @prodActual / (@destStdStickHour / @destUomEblek) END;

							IF(@workHourSrc + @workHourDst > 4)
							BEGIN
								SET @paidOther = @workHourDst * (@srcStdStickHour / @srcUomEblek);
							END
							ELSE
							BEGIN
								SET @paidOther = (@JKNhour * (@stdStickPerHour / @uomEblek)) -  @prodActual;
							END
							-- Update Production Card Source
							IF(@SrcProcess = @DestProcess)
							BEGIN
								UPDATE ProductionCard 
									SET UpahLain = @paidOther
								WHERE EmployeeID = @employeeID AND LocationCode = @SrcLocationCode AND UnitCode = @SrcUnitCode
								AND Shift = @SrcShift AND BrandCode = @SrcBrandCode AND ProcessGroup = @SrcProcess AND GroupCode = @SrcGroupCode
								AND RevisionType = 0 AND ProductionDate = @productionDate
							END
						END
					END
					-- Update Production Card Source
					IF(@SrcProcess = @DestProcess)
					BEGIN
						UPDATE ProductionCard 
						SET
						Production = ISNULL(@SrcProdActual, 0) + ISNULL(@DestProdActual, 0)
						WHERE EmployeeID = @employeeID AND LocationCode = @SrcLocationCode AND UnitCode = @SrcUnitCode
						AND Shift = @SrcShift AND BrandCode = @SrcBrandCode AND ProcessGroup = @SrcProcess AND GroupCode = @SrcGroupCode
						AND RevisionType = 0 AND ProductionDate = @productionDate
					END
				END

				BREAK;
			END
			IF NOT EXISTS
			(
				SELECT * FROM ProductionCard WHERE EmployeeID = @employeeID AND RevisionType = 0 AND LocationCode = @LocationCode
				AND UnitCode = @UnitCode AND BrandCode = @BrandCode AND ProcessGroup = @processGroup AND GroupCode = @splitGroups
				AND ProductionDate = @productionDate
			)
			BEGIN
				-- Insert Production Card Revision 0
				INSERT INTO [dbo].[ProductionCard]
					   ([RevisionType]
					   ,[LocationCode]
					   ,[UnitCode]
					   ,[Shift]
					   ,[BrandCode]
					   ,[BrandGroupCode]
					   ,[ProcessGroup]
					   ,[GroupCode]
					   ,[EmployeeID]
					   ,[EmployeeNumber]
					   ,[Production]
					   ,[ProductionDate]
					   ,[WorkHours]
					   ,[Absent]
					   ,[UpahLain]
					   ,[EblekAbsentType]
					   ,[Remark]
					   ,[Comments]
					   ,[CreatedDate]
					   ,[CreatedBy]
					   ,[UpdatedDate]
					   ,[UpdatedBy]
					   ,[ProductionCardCode])
				 VALUES
					   (0
					   ,@LocationCode
					   ,@UnitCode
					   ,@Shift
					   ,@BrandCode
					   ,@brandGroupCode
					   ,@processGroup
					   ,@splitGroups
					   ,@employeeID
					   ,@employeeNumber
					   ,@production
					   ,@productionDate
					   ,@workHour
					   ,@payrollAbsent
					   ,@paidOther
					   ,@sktAbsentCodeEblek
					   ,NULL
					   ,NULL
					   ,GETDATE()
					   ,@userName
					   ,GETDATE()
					   ,@userName
					   ,@ProductionCardCode)
			END
			ELSE
			BEGIN
				DECLARE @startDateClosingPay DATETIME, 
						@endDateClosingPay	 DATETIME;

				SELECT @endDateClosingPay	= MAX(ClosingDate) FROM MstClosingPayroll WHERE ClosingDate < CONVERT(DATE, GETDATE());
				SELECT @startDateClosingPay = DATEADD(DAY, 1, MAX(ClosingDate)) FROM MstClosingPayroll WHERE ClosingDate < @endDateClosingPay;
				
				DECLARE @tmpClosingDate TABLE (closingDate DATETIME);
				
				DECLARE @a INT = 0;
				WHILE @a < 7
				BEGIN
					INSERT INTO @tmpClosingDate (ClosingDate) VALUES (DATEADD(DAY, @a, @startDateClosingPay));
					SET @a = @a + 1
				END

				IF EXISTS (SELECT * FROM @tmpClosingDate WHERE closingDate = @productionDate)
				BEGIN
					DECLARE @currRevType INT;
					SELECT @currRevType = MAX(RevisionType) FROM ProductionCard 
					WHERE LocationCode = @LocationCode AND UnitCode = @UnitCode AND BrandCode = @BrandCode and ProcessGroup = @processGroup
					AND GroupCode = @splitGroups AND EmployeeID = @employeeID AND ProductionDate = @productionDate

					DECLARE @oldProduction REAL, @oldPaidOther REAL;
					SELECT @oldProduction = ISNULL(Production, 0), @oldPaidOther = ISNULL(UpahLain, 0) FROM ProductionCard 
					WHERE LocationCode = @LocationCode AND UnitCode = @UnitCode AND BrandCode = @BrandCode and ProcessGroup = @processGroup
					AND GroupCode = @splitGroups AND EmployeeID = @employeeID AND ProductionDate = @productionDate AND RevisionType = 0

					IF(ISNULL(@production, 0) - ISNULL(@oldProduction, 0) > 0)
					BEGIN
						-- Insert Production Card Revision > 0
						INSERT INTO [dbo].[ProductionCard]
							   ([RevisionType]
							   ,[LocationCode]
							   ,[UnitCode]
							   ,[Shift]
							   ,[BrandCode]
							   ,[BrandGroupCode]
							   ,[ProcessGroup]
							   ,[GroupCode]
							   ,[EmployeeID]
							   ,[EmployeeNumber]
							   ,[Production]
							   ,[ProductionDate]
							   ,[WorkHours]
							   ,[Absent]
							   ,[UpahLain]
							   ,[EblekAbsentType]
							   ,[Remark]
							   ,[Comments]
							   ,[CreatedDate]
							   ,[CreatedBy]
							   ,[UpdatedDate]
							   ,[UpdatedBy]
							   ,[ProductionCardCode])
						 VALUES
							   (@currRevType + 1
							   ,@LocationCode
							   ,@UnitCode
							   ,@Shift
							   ,@BrandCode
							   ,@brandGroupCode
							   ,@processGroup
							   ,@splitGroups
							   ,@employeeID
							   ,@employeeNumber
							   ,@production - @oldProduction
							   ,@productionDate
							   ,@workHour
							   ,@payrollAbsent
							   ,@paidOther - @oldPaidOther
							   ,@sktAbsentCodeEblek
							   ,NULL
							   ,NULL
							   ,GETDATE()
							   ,@userName
							   ,GETDATE()
							   ,@userName
							   ,@ProductionCardCode)
					END
				END
				ELSE
				BEGIN
					UPDATE ProductionCard
					SET [Absent] = @payrollAbsent, EblekAbsentType = @absentType, Production = @production, UpahLain = @paidOther
					WHERE EmployeeID = @employeeID AND RevisionType = 0 AND LocationCode = @LocationCode
					AND UnitCode = @UnitCode AND BrandCode = @BrandCode AND ProcessGroup = @processGroup AND GroupCode = @splitGroups
					AND ProductionDate = @productionDate 
				END
			END

			FETCH NEXT FROM cursor_entry INTO @employeeID, @employeeNumber, @workHour, @prodActual, @prodTarget, @absentType, @sktAbsentCodeEblek, @processGroup, @payrollAbsent
		END

		CLOSE cursor_entry; DEALLOCATE cursor_entry;

		FETCH NEXT FROM cursor_groups INTO @splitGroups
	END

	CLOSE cursor_groups; DEALLOCATE cursor_groups;  

COMMIT TRANSACTION trans
END TRY
BEGIN CATCH
	ROLLBACK TRANSACTION trans
	IF (@@TRANCOUNT > 0) ROLLBACK TRANSACTION
	DECLARE @ErrorMessage   NVARCHAR(1000) = ERROR_MESSAGE(),
			@ErrorState     INT = ERROR_STATE(),
			@ErrorSeverity  INT = ERROR_SEVERITY();

	RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
END CATCH
END