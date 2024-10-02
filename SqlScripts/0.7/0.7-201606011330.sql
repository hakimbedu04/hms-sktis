/****** Object:  UserDefinedFunction [dbo].[GetWorkHour]    Script Date: 6/1/2016 11:53:26 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =========================================================================
-- Description: Get Calculation WorkHour for tuning plant entry verification
-- Ticket: http://tp.voxteneo.co.id/entity/6533
-- Author: AZKA
-- Updated: 1.0 - 6/1/2016
-- =========================================================================

CREATE FUNCTION [dbo].[GetWorkHour]
(	
	@LocationCode VARCHAR(8),
	@UnitCode	VARCHAR(4),
	@GroupCode VARCHAR(4),
	@BrandCode VARCHAR(11),
	@Shift INT,
	@ProsesGroup VARCHAR(16),
	@ProductionDate DATE,
	@KPSYear INT,
	@KPSWeek INT,
	@DayOfWeek INT
)
RETURNS @WorkHourTemp TABLE (WorkHour DECIMAL(10,2))
AS
BEGIN
		INSERT INTO @WorkHourTemp
			SELECT dbo.CalculateWorkHours(@LocationCode, @UnitCode, @GroupCode, @BrandCode, @Shift, @ProsesGroup, @ProductionDate, @KPSYear, @KPSWeek, @DayOfWeek)
	RETURN;
END;
