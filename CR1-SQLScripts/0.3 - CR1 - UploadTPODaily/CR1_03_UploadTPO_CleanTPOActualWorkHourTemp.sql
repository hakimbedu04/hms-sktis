-- =============================================
-- Description: CR-1 Upload TPO Daily Clean Temp Table ActualWorkHour
-- Author: Azka
-- Ticket: http://tp.voxteneo.co.id/entity/18755
-- Update: 5/30/2017
-- =============================================

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('CLEAN_TPO_WORKHOUR_TEMP'))
DROP PROCEDURE [dbo].[CLEAN_TPO_WORKHOUR_TEMP]


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[CLEAN_TPO_WORKHOUR_TEMP]
	@locationCode VARCHAR(10),
	@brandCode		VARCHAR(20),
	@productionDate DATETIME,
	@processGroup VARCHAR(20),
	@statusEmp VARCHAR(29)
AS
BEGIN
	DELETE ExeTPOActualWorkHoursTemp WHERE LocationCode = @locationCode AND UnitCode = 'PROD' and BrandCode = @brandCode
	AND ProductionDate = @productionDate
END