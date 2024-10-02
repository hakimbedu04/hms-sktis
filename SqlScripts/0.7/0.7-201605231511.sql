/****** Object:  UserDefinedFunction [dbo].[GetMaintenanceEquipmentStockView]    Script Date: 5/23/2016 1:46:02 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description: GetMaintenanceEquipmentStockView
-- Ticket: http://tp.voxteneo.co.id/entity/6629
-- Author: AZKA
-- Updated: 1.0 - 5/23/2016
-- =============================================


CREATE FUNCTION [dbo].[GetMaintenanceEquipmentStockView]
(
	@LocationCode as varchar(10),
	@UnitCode as varchar(4),
	@InventoryDate as DateTime
)
RETURNS TABLE 
AS
RETURN
(
SELECT 
	ISNULL(ROW_NUMBER() OVER (ORDER BY ItemCode DESC), - 1) AS RowID, 
	InventoryDate,
	ItemCode, 
	ItemDescription,
	LocationCode, 
	UnitCode,
	COALESCE([In Transit],0) InTransit, 
	COALESCE([QI],0) QI, 
	COALESCE([Ready To Use],0) ReadyToUse, 
	COALESCE([Bad Stock],0) BadStock, 
	(COALESCE([In Transit],0)+COALESCE([QI],0)+COALESCE([Ready To Use],0)+COALESCE([Bad Stock],0)) TotalStockMntc,
	COALESCE([On Used],0) Used,
	COALESCE([On Repair],0) Repair,
	(COALESCE([On Used],0)+COALESCE([On Repair],0)) TotalStockProd
FROM (
		select 
			mi.InventoryDate,
			mi.ItemCode,
			mmi.ItemDescription,
			mi.LocationCode,
			mi.ItemStatus,
			mi.EndingStock,
			mi.UnitCode
		from MntcInventoryAll mi
		INNER JOIN MstMntcItem mmi on mmi.ItemCode = mi.ItemCode
) as InventoryTable
PIVOT (MAX(EndingStock) FOR ItemStatus IN (
	[In Transit],
	[QI],
	[Ready To Use],
	[Bad Stock],
	[On Used],
	[On Repair]
))as PivotTable
		where LocationCode = @LocationCode and UnitCode = @UnitCode and InventoryDate = @InventoryDate
)
