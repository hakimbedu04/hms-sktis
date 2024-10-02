/****** Object:  StoredProcedure [dbo].[DELETE_WORKER_ASSIGNMENT]    Script Date: 7/21/2016 10:08:36 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

ALTER PROCEDURE [dbo].[DELETE_WORKER_ASSIGNMENT]
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
	DECLARE @ERR_MSG AS NVARCHAR(4000), @ERR_STA AS SMALLINT 

	BEGIN TRANSACTION 
	BEGIN TRY
		SET DATEFIRST 1

		--SELECT 
		--	@SourceLocationCode			= SourceLocationCode
		--	,@SourceUnitCode			= SourceUnitCode			
		--	,@SourceShift				= SourceShift				
		--	,@SourceProcessGroup		= SourceProcessGroup		
		--	,@SourceGroupCode			= SourceGroupCode			
		--	,@SourceBrandCode			= SourceBrandCode			
		--	,@DestinationLocationCode	= DestinationLocationCode	
		--	,@DestinationUnitCode		= DestinationUnitCode		
		--	,@DestinationShift			= DestinationShift			
		--	,@DestinationProcessGroup	= DestinationProcessGroup	
		--	,@DestinationGroupCode		= DestinationGroupCode		
		--	,@DestinationGroupCodeDummy	= DestinationGroupCodeDummy	
		--	,@DestinationBrandCode		= DestinationBrandCode				
		--	,@EmployeeNumber			= EmployeeNumber			
		--	,@EndDate					= EndDate						
		--FROM ExePlantWorkerAssignment 
		--WHERE EmployeeID = @EmployeeID AND StartDate = @StartDate;

		DECLARE @CurrentDate DATETIME;
		DECLARE @Week INT;
		DECLARE @Year INT;

		DECLARE @ProductionEntryCodeSource VARCHAR(50); -- Production Entry Code Source
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

			-- Create Production Entry Code Destination Dummy
			SET @ProductionEntryCodeDestinationDummy = 'EBL' + '/' + @DestinationLocationCode 
															 + '/' + CAST(@DestinationShift as VARCHAR(1))   
															 + '/' + @DestinationUnitCode 
															 + '/' + @DestinationGroupCodeDummy 
															 + '/' + @DestinationBrandCode 
															 + '/' + CAST(@Year as VARCHAR(4))    
															 + '/' + CAST(@Week as VARCHAR(2))   
															 + '/' + CONVERT(varchar,(select datepart(dw, @CurrentDate)));

			
			DELETE FROM ExePlantProductionEntry WHERE ProductionEntryCode = @ProductionEntryCodeDestinationDummy AND EmployeeID = @EmployeeID;

			-- Check existing production entry verification dummy group
			DECLARE @countEntryVerificationDummy INT;
			SELECT @countEntryVerificationDummy = COUNT(*) 
			FROM ExePlantProductionEntryVerification ev INNER JOIN ExePlantProductionEntry en ON en.ProductionEntryCode = ev.ProductionEntryCode
			WHERE ev.ProductionEntryCode = @ProductionEntryCodeDestinationDummy;

			IF(@countEntryVerificationDummy = 0)
			BEGIN /** delete existing entry verification dummy **/
				DELETE ExePlantProductionEntryVerification WHERE ProductionEntryCode = @ProductionEntryCodeDestinationDummy;
			END;

			-- Update source production entry
			UPDATE ExePlantProductionEntry 
				SET 
					AbsentType = NULL, 
					AbsentCodeEblek = NULL, 
					AbsentCodePayroll = NULL,
					AbsentRemark = NULL,
					StartDateAbsent = NULL,
					UpdatedDate = GETDATE(),
					UpdatedBy = @UpdatedBy,
					IsFromAbsenteeism = 0
				WHERE ProductionEntryCode = @ProductionEntryCodeSource and EmployeeID = @EmployeeID AND (AbsentType = 'Multiskill Out' OR AbsentType = 'Tugas Luar');

			-- Delete existing assignment
			DELETE FROM ExePlantWorkerAssignment WHERE EmployeeID = @EmployeeID AND StartDate = @StartDate;

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