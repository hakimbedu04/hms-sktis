-- =============================================
-- Description: CR-1 Upload TPO Daily Copy Workhour temp To Actual WorkHour
-- Author: Azka
-- Ticket: http://tp.voxteneo.co.id/entity/18755
-- Update: 5/31/2017
-- =============================================

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('UPLOAD_TPODAILY_COPY_ACTUALWORKHOURTEMP'))
DROP PROCEDURE [dbo].[UPLOAD_TPODAILY_COPY_ACTUALWORKHOURTEMP]


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[UPLOAD_TPODAILY_COPY_ACTUALWORKHOURTEMP]
	@locationCode VARCHAR(8),
	@brandCode VARCHAR(11),
	@productionDate DATETIME,
	@processGroup VARCHAR(16),
	@statusEmp VARCHAR(16)
AS
BEGIN

	IF EXISTS (SELECT LocationCode, UnitCode, BrandCode, ProductionDate, ProcessGroup, StatusEmp FROM ExeTPOActualWorkHours
				WHERE UnitCode = 'PROD' AND LocationCode = @locationCode AND BrandCode = @brandCode AND ProductionDate = @productionDate
				AND ProcessGroup = @processGroup AND StatusEmp = @statusEmp)
	BEGIN
		UPDATE t
		SET 
			t.TimeIn = e.TimeIn,
			t.TimeOut = e.TimeOut,
			t.BreakTime = e.BreakTime,
			t.UpdatedBy = e.UpdatedBy,
			t.UpdatedDate = e.UpdatedDate
		FROM ExeTPOActualWorkHours as t INNER JOIN
					ExeTPOActualWorkHoursTemp e on e.LocationCode = t.LocationCode AND e.UnitCode = t.UnitCode
					AND e.BrandCode = t.BrandCode AND e.ProductionDate = t.ProductionDate AND e.ProcessGroup = t.ProcessGroup
					AND e.StatusEmp = t.StatusEmp
		WHERE e.UnitCode = 'PROD' AND e.LocationCode = @locationCode AND e.BrandCode = @brandCode AND e.ProductionDate = @productionDate
				AND e.ProcessGroup = @processGroup AND e.StatusEmp = @statusEmp
	END
	ELSE
	BEGIN
		INSERT INTO [dbo].[ExeTPOActualWorkHours]
					   ([LocationCode]
					   ,[UnitCode]
					   ,[BrandCode]
					   ,[ProductionDate]
					   ,[ProcessGroup]
					   ,[ProcessOrder]
					   ,[StatusEmp]
					   ,[StatusIdentifier]
					   ,[TimeIn]
					   ,[TimeOut]
					   ,[BreakTime]
					   ,[CreatedDate]
					   ,[CreatedBy]
					   ,[UpdatedDate]
					   ,[UpdatedBy])
		SELECT 
					[LocationCode]
					   ,[UnitCode]
					   ,[BrandCode]
					   ,[ProductionDate]
					   ,[ProcessGroup]
					   ,[ProcessOrder]
					   ,[StatusEmp]
					   ,[StatusIdentifier]
					   ,[TimeIn]
					   ,[TimeOut]
					   ,[BreakTime]
					   ,[CreatedDate]
					   ,[CreatedBy]
					   ,[UpdatedDate]
					   ,[UpdatedBy]
		FROM ExeTPOActualWorkHoursTemp e
		WHERE e.UnitCode = 'PROD' AND e.LocationCode = @locationCode AND e.BrandCode = @brandCode AND e.ProductionDate = @productionDate
				AND e.ProcessGroup = @processGroup AND e.StatusEmp = @statusEmp
	END
	
	DELETE ExeTPOActualWorkHoursTemp
		WHERE UnitCode = 'PROD' AND LocationCode = @locationCode AND BrandCode = @brandCode AND ProductionDate = @productionDate
				AND ProcessGroup = @processGroup AND StatusEmp = @statusEmp
END