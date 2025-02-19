USE [SKT_DEV]
GO
/****** Object:  UserDefinedFunction [dbo].[GetRealStock]    Script Date: 05/23/2016 15:03:25 ******/
/**
-- =============================================
-- Author:		HAKIM
-- Updated date: 2016/05/23
-- Description:	alter UserDefinedFunction [dbo].[GetRealStock]
-- reference : http://tp.voxteneo.co.id/entity/6630
-- =============================================
*/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
/* GET Ending Stock By Location COde and Item Code */
/* Date always using current date From Application */
ALTER FUNCTION [dbo].[GetRealStock]
(	
	@locationcode VARCHAR(20),
	@itemcode VARCHAR(20),
	@date DATE
)
RETURNS TABLE 
AS
RETURN 
(
	
	--SELECT SUM(COALESCE(EndingStock,0)) as EndingStock FROM MntcInventory
	--WHERE
	--	ItemCode = 'BDK-006' AND
	--	ItemStatus IN ('Ready To Use','On Used') AND
	--	InventoryDate = CAST('2016-05-23' as DATE) AND
	--	LocationCode = 'idba'
	--GROUP BY InventoryDate, ItemCode,LocationCode
	
	SELECT StackReady as EndingStock FROM [SKT_DEV].[dbo].[MaintenanceExecutionInventoryView]
	WHERE
	ItemCode = @itemcode AND
	InventoryDate = CAST(@date as DATE) AND
	LocationCode = @locationcode
	
)
