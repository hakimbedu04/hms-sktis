/****** Object:  UserDefinedFunction [dbo].[GetAbsensi]    Script Date: 6/1/2016 11:52:20 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- ==================================================================
-- Description: Get Count Absensi for tuning plant entry verification
-- Ticket: http://tp.voxteneo.co.id/entity/6533
-- Author: AZKA
-- Updated: 1.0 - 6/1/2016
-- =================================================================

CREATE FUNCTION [dbo].[GetAbsensi]
(	
	@ProductionEntryCode VARCHAR(50),
	@AbsentCode          VARCHAR(50)
)
RETURNS @Absensi TABLE (CountAbsent INT)
AS
BEGIN
		INSERT INTO @Absensi
			SELECT dbo.GetAbsent(@ProductionEntryCode, @AbsentCode)
	RETURN;
END;