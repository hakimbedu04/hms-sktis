-- Description: Create view MntcFulfillmentView, same like MaintenanceEquipmentStockView but join from TemporaryTableMntcAll and must be exec sp MntcInventoryAllProcedure before call this view
-- Ticket: http://tp.voxteneo.co.id/entity/10936
-- Author: Hakim
-- date	: 2016-11-01

CREATE VIEW [dbo].[MntcStockView]
AS
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
		from TemporaryTableMntcAll mi
		INNER JOIN MstMntcItem mmi on mmi.ItemCode = mi.ItemCode
) as InventoryTable
PIVOT (MAX(EndingStock) FOR ItemStatus IN (
	[In Transit],
	[QI],
	[Ready To Use],
	[Bad Stock],
	[On Used],
	[On Repair]
))as PivotTable;
GO