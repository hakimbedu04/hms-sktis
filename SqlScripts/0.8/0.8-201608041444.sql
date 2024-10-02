/****** Object:  StoredProcedure [dbo].[EDIT_PLANT_PRODUCTION_ENTRY]    Script Date: 8/4/2016 12:08:24 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[INSERT_DEFAULT_TARGET_ACTUAL_ENTRY]
	@ProductionEntryCode	VARCHAR(64),
	@SaveType				VARCHAR(20)
AS
BEGIN
	DECLARE @ErrorMessage   NVARCHAR(1000);
	DECLARE @ErrorState	INT;
	DECLARE @ErrorSeverity  INT;

	BEGIN TRANSACTION 
	BEGIN TRY

	IF(@SaveType = 'target')
	BEGIN
		UPDATE ExePlantProductionEntry 
		SET ProdTarget = ProdCapacity
		WHERE ProductionEntryCode = @ProductionEntryCode
	END;
	
	IF(@SaveType = 'actual')
	BEGIN
		UPDATE ExePlantProductionEntry 
		SET ProdActual = ProdTarget
		WHERE ProductionEntryCode = @ProductionEntryCode
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
END