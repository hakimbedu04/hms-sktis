/****** Object:  UserDefinedFunction [dbo].[CheckIsValidEndDateAndStartDate]    Script Date: 7/21/2016 10:10:29 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE FUNCTION [dbo].[CheckIsValidEndDateAndStartDate]
(
	@StartDate	DATETIME,
	@EndDate	DATETIME
)
RETURNS BIT
AS
BEGIN
	IF(@EndDate < @StartDate) RETURN 0;

	RETURN 1;
END;