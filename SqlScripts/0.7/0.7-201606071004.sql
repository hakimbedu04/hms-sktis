/****** Object:  UserDefinedFunction [dbo].[GetProcessFromProdCard]    Script Date: 6/7/2016 9:38:05 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Opun Buds
-- Create date: 2016-04-22
-- Description:	Function buat 
-- =============================================

-- =============================================
-- Author: Azka
-- Create date: 6/7/2016
-- Ticket : http://tp.voxteneo.co.id/entity/6903
-- Description:	Change from production card to plant Entry Verification
-- =============================================

ALTER FUNCTION [dbo].[GetProcessFromProdCard] 
(	
	@StartDate DATE = NULL,
	@EndDate DATE = NULL,
	@LocationCode varchar(20)= NULL,
	@UnitCode varchar(20) = NULL
)
RETURNS @ProcessGroup TABLE (id int,ProcessGroup varchar(100))
AS
BEGIN
	INSERT INTO @ProcessGroup
	select distinct v.ProcessOrder, v.ProcessGroup from dbo.ExePlantProductionEntryVerification v
	inner join dbo.UtilTransactionLogs u on u.TransactionCode = v.ProductionEntryCode
	WHERE v.LocationCode = ISNULL(@LocationCode, v.LocationCode)
	and v.UnitCode = ISNULL(@UnitCode, v.UnitCode)
	and v.ProductionDate >= @StartDate
	and v.ProductionDate <= @EndDate
	and u.IDFlow = 17
	order by v.ProcessOrder
	RETURN
END
