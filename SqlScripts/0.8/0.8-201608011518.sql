/****** Object:  StoredProcedure [dbo].[EDIT_PLANT_PRODUCTION_ENTRY]    Script Date: 8/1/2016 2:26:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description: fixing insert target and actual entry
-- Ticket: http://tp.voxteneo.co.id/entity/8555
-- Author: AZKA
-- Updated: 1.0 - 2016/08/01
-- =============================================

ALTER PROCEDURE [dbo].[EDIT_PLANT_PRODUCTION_ENTRY]
	@ProductionEntryCode	VARCHAR(64),
	@EmployeeID				VARCHAR(64),
	@EmployeeNumber			VARCHAR(11),
	@StatusEmp				VARCHAR(64),
	@StatusIdentifier		INT,
	@StartDateAbsent		DATETIME,
	@AbsentType				VARCHAR(128),
	@ProdCapacity			DECIMAL(18, 3),
	@ProdTarget				REAL = NULL,
	@ProdActual				REAL = NULL,
	@AbsentRemark			VARCHAR(256),
	@AbsentCodeEblek		VARCHAR(128),
	@AbsentCodePayroll		VARCHAR(128),
	@UpdatedDate			DATETIME,
	@UpdatedBy				VARCHAR(64),
	@IsFromAbsenteeism		BIT,
	@SaveType				VARCHAR(20),
	@LocationCode			VARCHAR(8),
	@UnitCode				VARCHAR(4),
	@GroupCode				VARCHAR(4),
	@Shift					INT
AS
BEGIN
	DECLARE @ErrorMessage   NVARCHAR(1000);
	DECLARE @ErrorState	INT;
	DECLARE @ErrorSeverity  INT;

	BEGIN TRANSACTION 
	BEGIN TRY
		DECLARE @absentTypeCurrDB VARCHAR(128);
		DECLARE @prodTargetCurrDB REAL;
		DECLARE @prodActualCurrDB REAL;

		SELECT @AbsentCodeEblek = SktAbsentCode, @AbsentCodePayroll = PayrollAbsentCode, @AbsentRemark = Remark
		FROM MstPlantAbsentType WHERE AbsentType = @AbsentType

		SELECT @absentTypeCurrDB = AbsentType, @prodTargetCurrDB = ProdTarget, @prodActualCurrDB = ProdActual, @ProdCapacity = ProdCapacity
		FROM ExePlantProductionEntry 
		WHERE ProductionEntryCode = @ProductionEntryCode AND EmployeeID = @EmployeeID

		IF(ISNULL(@AbsentType, '') <> ISNULL(@absentTypeCurrDB, ''))
		BEGIN
			IF(ISNULL(@absentTypeCurrDB,'') = '')
			BEGIN
				IF(@AbsentType <> ISNULL(@absentTypeCurrDB, ''))
				BEGIN
					EXEC INSERT_WORKER_ABSENTEEISM_FROM_ENTRY @StartDateAbsent, @AbsentType, @AbsentCodeEblek, @AbsentCodePayroll, @EmployeeID, @LocationCode,
					@UnitCode, @GroupCode, @Shift, @UpdatedDate, @UpdatedBy
				END;
			END;
			ELSE
			BEGIN
				EXEC EDIT_WORKER_ABSENTEEISM_FROM_ENTRY @StartDateAbsent, @AbsentType, @absentTypeCurrDB, @AbsentCodeEblek, @AbsentCodePayroll, @EmployeeID, @LocationCode,
				@UnitCode, @GroupCode, @Shift, @UpdatedDate, @UpdatedBy
			END;
		END;

		IF(@SaveType = 'target')
		BEGIN
			IF(@ProdTarget IS NULL)
			BEGIN
				IF(@ProdCapacity IS NULL)
				BEGIN
					SET @ProdTarget = 0;
				END
				ELSE
				BEGIN
					SET @ProdTarget = @ProdCapacity;
				END
			END
			UPDATE ExePlantProductionEntry 
			SET
				ProdTarget = @ProdTarget, 
				ProdActual  = @ProdActual, 
				AbsentType = @AbsentType, 
				StartDateAbsent = @StartDateAbsent,
				AbsentCodeEblek = @AbsentCodeEblek,
				AbsentCodePayroll = @AbsentCodePayroll,
				AbsentRemark = @AbsentRemark,
				IsFromAbsenteeism = 0
			WHERE ProductionEntryCode = @ProductionEntryCode AND EmployeeID = @EmployeeID
		END
		ELSE IF(@SaveType = 'actual')
		BEGIN
		IF(@ProdActual IS NULL)
			BEGIN
				SET @ProdActual = @prodTargetCurrDB;
			END;
			UPDATE ExePlantProductionEntry 
			SET 
				ProdActual  = @ProdActual, 
				AbsentType = @AbsentType, 
				StartDateAbsent = @StartDateAbsent,
				AbsentCodeEblek = @AbsentCodeEblek,
				AbsentCodePayroll = @AbsentCodePayroll,
				AbsentRemark = @AbsentRemark,
				IsFromAbsenteeism = 0
			WHERE ProductionEntryCode = @ProductionEntryCode AND EmployeeID = @EmployeeID
		END

		IF(ISNULL(@AbsentType, '') = '')
		BEGIN
			SET @StartDateAbsent = NULL;
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