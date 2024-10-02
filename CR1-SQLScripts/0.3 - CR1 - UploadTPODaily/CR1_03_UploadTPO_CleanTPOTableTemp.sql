-- =============================================
-- Description: CR-1 Upload TPO Daily Clean Temp Table
-- Author: Azka
-- Ticket: http://tp.voxteneo.co.id/entity/18755
-- Update: 5/30/2017
-- =============================================

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('CLEAN_TPO_ENTRY_TEMP'))
DROP PROCEDURE [dbo].[CLEAN_TPO_ENTRY_TEMP]


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CLEAN_TPO_ENTRY_TEMP]
	@listProductionEntryCode VARCHAR(MAX) -- list string production entry split by ';'
AS
BEGIN
	DECLARE @splitProdEntryCode VARCHAR(50);

	DECLARE cursor_splitProductionEntryCode CURSOR LOCAL FOR
	SELECT splitdata FROM [dbo].[fnSplitString](@listProductionEntryCode, ';')

	OPEN cursor_splitProductionEntryCode

	FETCH NEXT FROM cursor_splitProductionEntryCode   
	INTO @splitProdEntryCode

	WHILE @@FETCH_STATUS = 0  
	BEGIN

		DELETE ExeTPOProductionTemp WHERE ProductionEntryCode = @splitProdEntryCode
		DELETE ExeTPOProductionEntryVerificationTemp WHERE ProductionEntryCode = @splitProdEntryCode

		FETCH NEXT FROM cursor_splitProductionEntryCode   
		INTO @splitProdEntryCode
	END

	CLOSE cursor_splitProductionEntryCode;  
	DEALLOCATE cursor_splitProductionEntryCode;  
END