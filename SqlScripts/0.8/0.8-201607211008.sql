/****** Object:  UserDefinedFunction [dbo].[CheckDateClosingPayrollOrHoliday]    Script Date: 7/21/2016 10:09:50 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[CheckDateClosingPayrollOrHoliday]
(
	@Date			DATETIME,
	@LocationCode	VARCHAR(8)
)
RETURNS BIT
AS
BEGIN
	
	 -- Checking Closing Payroll
     DECLARE @countCosingPayroll INT;
	 SELECT @countCosingPayroll = COUNT(*) FROM MstClosingPayroll WHERE ClosingDate = @Date;
	 IF(@countCosingPayroll > 0) RETURN 1;

	 -- Checking Holiday
	 DECLARE @countHoliday INT;
	 SELECT @countHoliday = COUNT(*) FROM MstGenHoliday WHERE LocationCode = @LocationCode AND HolidayDate = @Date;
	 IF(@countHoliday > 0) RETURN 1;

	 -- Checking if sunday
	 IF((SELECT DATENAME(weekday, @Date)) = 'Sunday') RETURN 1;

	RETURN 0;
END;