
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetMaintenanceEquipmentStockFunction]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[GetMaintenanceEquipmentStockFunction]
GO

-- =============================================
-- Description: GetMaintenanceEquipmentStockFunction
-- Author: WAHYU
-- Updated: 1.0 - 10/17/2016
-- =============================================


CREATE FUNCTION [dbo].[GetMaintenanceEquipmentStockFunction]
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
		--from MntcInventoryAll mi
		from
		( -- codingan dari [MaintenanceExecutionInventoryProcedure]tanpa tabel temporary
		
		
		SELECT
		  InventoryDate,
		  LocationCode,
		  UnitCode,
		  ItemStatus,
		  ItemCode,
		  ItemType,
		  ItemDescription,
		  SUM(BeginningStock) BeginningStock,
		  SUM(StockIn) StockIn,
		  SUM(StockOut) StockOut,
		  SUM(EndingStock) EndingStock
		  
		  FROM (
			SELECT
				a.InventoryDate,
				a.ItemStatus,
				a.ItemCode,
				a.LocationCode,
				a.UnitCode,
				c.ItemType,
				c.ItemDescription,
				CASE WHEN b.DBeginningStock IS NULL THEN a.BeginningStock ELSE a.BeginningStock + b.DBeginningStock END     
				BeginningStock,
				--a.BeginningStock + b.DBeginningStock AS BeginningStock,
				CASE WHEN b.DStockIn IS NULL THEN a.StockIn ELSE a.StockIn + b.DStockIn END     
				StockIn,
				--a.StockIn + b.DStockIn AS StockIn,
				CASE WHEN b.DStockOut IS NULL THEN a.StockOut ELSE a.StockOut + b.DStockOut END     
				StockOut,
				--a.StockOut + b.DStockOut AS StockOut,
				CASE WHEN b.DEndingStock IS NULL THEN a.EndingStock ELSE a.EndingStock + b.DEndingStock END     
				EndingStock
				--a.EndingStock + b.DEndingStock AS EndingStock
			FROM (
				SELECT
					a.InventoryDate,
					a.ItemStatus,
					a.ItemCode,
					a.LocationCode,
					a.BeginningStock,
					a.StockIn,
					a.StockOut,
					a.EndingStock,
					a.UnitCode
				FROM dbo.MntcInventory a
				WHERE InventoryDate = @InventoryDate
				AND LocationCode = @LocationCode
				--AND ItemStatus = 'BAD STOCK'
			) AS a
			LEFT JOIN (select * from GetDeltaViewFunction(@LocationCode,@InventoryDate)) as b
			
			--dbo.MntcInventoryDeltaView AS b
			ON b.InventoryDate = a.InventoryDate
				AND b.ItemCode = a.ItemCode
				AND b.ItemStatus = a.ItemStatus
				AND b.LocationCode = a.LocationCode
				AND b.UnitCode = a.UnitCode
			INNER JOIN dbo.MstMntcItem AS c
				ON c.ItemCode = a.ItemCode
		) as c
		GROUP BY InventoryDate,
				 ItemCode,
				 LocationCode,
				 ItemType,
				 ItemDescription,
				 ItemStatus,
				 UnitCode

				
		-- END of codingan dari view [MaintenanceExecutionInventoryProcedure]
		)mi
		INNER JOIN MstMntcItem mmi on mmi.ItemCode = mi.ItemCode
		where mi.LocationCode = @LocationCode 
		AND mi.InventoryDate = @InventoryDate 
		AND mi.UnitCode LIKE + '%'+@UnitCode+'%'
) as InventoryTable
PIVOT (MAX(EndingStock) FOR ItemStatus IN (
	[In Transit],
	[QI],
	[Ready To Use],
	[Bad Stock],
	[On Used],
	[On Repair]
))as PivotTable


)

GO


