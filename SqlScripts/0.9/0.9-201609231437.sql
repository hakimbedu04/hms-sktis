/****** Object:  StoredProcedure [dbo].[EDIT_PLANT_PRODUCTION_ENTRY]    Script Date: 9/23/2016 2:37:03 PM ******/
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

-- =============================================
-- Description: adding update production verification
-- Author: AZKA
-- Updated: 2.0 - 2016/08/03
-- =============================================

-- =============================================
-- Description: Remove IsFromAbsenteeism = 0
-- Author: WAHYU
-- Updated: 3.0 - 2016/08/16
-- =============================================


-- =============================================
-- Description: adding check condition if absent from absenteeism
-- Author: AZKA
-- Updated: 4.0 - 2016/08/23
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
	@ProdTarget				REAL,
	@ProdActual				REAL,
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

	BEGIN TRANSACTION A
	BEGIN TRY
		DECLARE @absentTypeCurrDB VARCHAR(128);
		DECLARE @prodTargetCurrDB REAL;
		DECLARE @prodActualCurrDB REAL;
		DECLARE @IsActiveInAbsent BIT;
		DECLARE @totalActualRecalculate REAL;
		DECLARE @totalTargetRecalculate REAL;

		SELECT @absentTypeCurrDB = AbsentType, @prodTargetCurrDB = ProdTarget, @prodActualCurrDB = ProdActual, @ProdCapacity = ProdCapacity, @IsActiveInAbsent = IsFromAbsenteeism
		FROM ExePlantProductionEntry 
		WHERE ProductionEntryCode = @ProductionEntryCode AND EmployeeID = @EmployeeID

		IF(ISNULL(@AbsentType, '') <> ISNULL(@absentTypeCurrDB, ''))
		BEGIN
			IF(ISNULL(@AbsentType, '') = '')
			BEGIN
				-- assign absent type to null
				IF(ISNULL(@absentTypeCurrDB, '') <> '' AND ISNULL(@IsActiveInAbsent, 0) <> 1)
				BEGIN
					EXEC EDIT_WORKER_ABSENTEEISM_FROM_ENTRY @StartDateAbsent, @AbsentType, @absentTypeCurrDB, @AbsentCodeEblek, @AbsentCodePayroll, @EmployeeID, @LocationCode,
					@UnitCode, @GroupCode, @Shift, @UpdatedDate, @UpdatedBy

					UPDATE ExePlantProductionEntry 
					SET
						AbsentType = NULL, 
						StartDateAbsent = NULL,
						AbsentCodeEblek = NULL,
						AbsentCodePayroll = NULL,
						AbsentRemark = NULL,
						IsFromAbsenteeism = 0
					WHERE ProductionEntryCode = @ProductionEntryCode AND EmployeeID = @EmployeeID
				END
			END
			ELSE 
			BEGIN
				IF(ISNULL(@AbsentType, '') <> '' AND ISNULL(@absentTypeCurrDB, '') <> 'Multiskill Out' AND ISNULL(@IsActiveInAbsent, 0) <> 1)
				BEGIN
					SELECT @AbsentCodeEblek = SktAbsentCode, @AbsentCodePayroll = PayrollAbsentCode, @AbsentRemark = Remark
					FROM MstPlantAbsentType WHERE AbsentType = @AbsentType

					IF(ISNULL(@absentTypeCurrDB, '') <> '')
					BEGIN
						EXEC EDIT_WORKER_ABSENTEEISM_FROM_ENTRY @StartDateAbsent, @AbsentType, @absentTypeCurrDB, @AbsentCodeEblek, @AbsentCodePayroll, @EmployeeID, @LocationCode,
						@UnitCode, @GroupCode, @Shift, @UpdatedDate, @UpdatedBy
					END
					ELSE
					BEGIN
						EXEC INSERT_WORKER_ABSENTEEISM_FROM_ENTRY @StartDateAbsent, @AbsentType, @AbsentCodeEblek, @AbsentCodePayroll, @EmployeeID, @LocationCode,
						@UnitCode, @GroupCode, @Shift, @UpdatedDate, @UpdatedBy
					END

					UPDATE ExePlantProductionEntry 
					SET

						AbsentType = @AbsentType, 
						StartDateAbsent = @StartDateAbsent,
						AbsentCodeEblek = @AbsentCodeEblek,
						AbsentCodePayroll = @AbsentCodePayroll,
						AbsentRemark = @AbsentRemark,
						IsFromAbsenteeism = 0
					WHERE ProductionEntryCode = @ProductionEntryCode AND EmployeeID = @EmployeeID
				END
			END
		END

		IF(@SaveType = 'target')
		BEGIN
			IF(@ProdTarget IS NOT NULL)
			BEGIN
				UPDATE ExePlantProductionEntry 
				SET
					ProdTarget = @ProdTarget, 
					ProdActual  = @ProdActual
				WHERE ProductionEntryCode = @ProductionEntryCode AND EmployeeID = @EmployeeID
			END
		END
		ELSE IF(@SaveType = 'actual')
		BEGIN
		IF(@ProdActual IS NOT NULL)
			BEGIN
				UPDATE ExePlantProductionEntry 
				SET 
					ProdActual  = @ProdActual
				WHERE ProductionEntryCode = @ProductionEntryCode AND EmployeeID = @EmployeeID
			END;
		END

		SELECT 
			@totalActualRecalculate = SUM(ProdActual), 
			@totalTargetRecalculate = SUM(ProdTarget) 
		FROM ExePlantProductionEntry 
		WHERE ProductionEntryCode = @ProductionEntryCode;

		UPDATE ExePlantProductionEntryVerification
		SET
			TotalActualValue = @totalActualRecalculate,
			TotalTargetValue = @totalTargetRecalculate
		WHERE ProductionEntryCode = @ProductionEntryCode;

		COMMIT TRANSACTION A
	END TRY
	BEGIN CATCH
		ROLLBACK TRANSACTION A
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