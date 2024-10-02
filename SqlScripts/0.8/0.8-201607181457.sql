/****** Object:  StoredProcedure [dbo].[INSERT_WORKER_ASSIGNMENT]    Script Date: 7/18/2016 12:04:21 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[DELETE_WORKER_ABSENTEEISM]
	@StartDateAbsent	DATETIME,
	@EmployeeID			VARCHAR(64),
	@AbsentType			VARCHAR(128),
	@EndDateAbsent		DATETIME,
	@SktAbsentCode		VARCHAR(11),
	@PayrollAbsentCode	VARCHAR(11),
	@ePaf				VARCHAR(64),
	@Attachment			VARCHAR(64),
	@AttachmentPath		VARCHAR(64),
	@CreatedDate		DATETIME,
	@CreatedBy			VARCHAR(64),
	@UpdatedDate		DATETIME,
	@UpdatedBy			VARCHAR(64),
	@EmployeeNumber		VARCHAR(64),
	@LocationCode		VARCHAR(8),
	@UnitCode			VARCHAR(4),
	@GroupCode			VARCHAR(4),
	@TransactionDate	DATETIME,
	@Shift				INT
AS
BEGIN
	DECLARE @ERR_MSG AS NVARCHAR(4000)  ,@ERR_STA AS SMALLINT 

	BEGIN TRANSACTION 
	BEGIN TRY
		SET DATEFIRST 1

		-- Get value from master absent type
		SELECT @AbsentType = AbsentType, @SKTAbsentCode = SktAbsentCode, @PayrollAbsentCode = PayrollAbsentCode  
		FROM MstPlantAbsentType WHERE AbsentType = @AbsentType;

		DECLARE @CurrentDate DATETIME;
		DECLARE @Week INT;
		DECLARE @Year INT;

		DECLARE @ProductionEntryCode VARCHAR(50); -- Production Entry Code Source
		
		-- Set CurrentDate = start date absenteeism
		SET @CurrentDate = @StartDateAbsent;

		-- Looping from CurrentDate (start data absenteeism) until end date absenteeism
		WHILE (@CurrentDate <= @EndDateAbsent)
		BEGIN
			-- Get year and week from master gen week by @CurrentDate
			SELECT @Week = [Week], @Year = [Year] 
			FROM MstGenWeek 
			WHERE @CurrentDate BETWEEN StartDate AND EndDate;

			-- Get production entry code from join to verification
			SELECT TOP 1 @ProductionEntryCode = ev.ProductionEntryCode 
			FROM ExePlantProductionEntryVerification ev 
			INNER JOIN ExePlantProductionEntry en on en.ProductionEntryCode = ev.ProductionEntryCode
			WHERE 
				ev.LocationCode = @LocationCode AND
				ev.UnitCode = @UnitCode AND
				ev.GroupCode = @GroupCode AND
				ev.Shift = @Shift AND
				ev.KPSYear = @Year AND
				ev.KPSWeek = @Week AND
				ev.ProductionDate = @CurrentDate AND
				en.EmployeeID = @EmployeeID

			-- Update production entry
			UPDATE ExePlantProductionEntry 
			SET 
				AbsentType = NULL,
				AbsentCodeEblek = NULL,
				AbsentCodePayroll = NULL,
				StartDateAbsent = NULL,
				ProdTarget = ProdCapacity,
				ProdActual = null,
				UpdatedDate = @UpdatedDate,
				UpdatedBy = @UpdatedBy,
				IsFromAbsenteeism = 0
			WHERE ProductionEntryCode = @ProductionEntryCode AND EmployeeID = @EmployeeID;

			SET @CurrentDate = DATEADD(DAY, 1, @CurrentDate); /*increment current date*/

		END;

		DELETE FROM ExePlantWorkerAbsenteeism WHERE EmployeeID = @EmployeeID and Shift = @Shift and StartDateAbsent = @StartDateAbsent;
		
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


