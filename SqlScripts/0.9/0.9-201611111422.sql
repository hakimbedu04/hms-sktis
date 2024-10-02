-- =============================================
-- Author:		Wahyu
-- Create date: 
-- Description:	Procedure for copying data to [TemporaryTableEquipmentStock]
-- =============================================

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MntcEquipmentStockProcedure]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[MntcEquipmentStockProcedure]
GO

CREATE PROCEDURE [dbo].[MntcEquipmentStockProcedure]
(	
	@LocationCode           VARCHAR(10),
	@UnitCode               VARCHAR(10),
	@InventoryDate          DateTime,
	@QParam                 VARCHAR(100),
	@UserAD                 VARCHAR(50)
)
AS
BEGIN

DECLARE @Unit varchar(50)
    SET @Unit = @UnitCode
DECLARE @InvDate DATETIME
    SET @InvDate = @InventoryDate
DECLARE @LocCode varchar(50)
    SET @LocCode = @LocationCode
DECLARE @QPrm varchar(100)
    SET @QPrm = @QParam
DECLARE @UsrAD varchar(50)
    SET @UsrAD = @UserAD
    
--delete from table temporary
DELETE FROM TemporaryTableEquipmentStock WHERE QParam LIKE + '%'+@UsrAD+'%'

--insert to table temporary
INSERT INTO TemporaryTableEquipmentStock

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
	(COALESCE([On Used],0)+COALESCE([On Repair],0)) TotalStockProd,
	QParam = @QPrm
	
	--INTO TemporaryTableEquipmentStock
	
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
		from TemporaryTableViewInventory mi
		--( -- codingan dari [MaintenanceExecutionInventoryProcedure]tanpa tabel temporary
		
		
		--SELECT
		--  InventoryDate,
		--  LocationCode,
		--  UnitCode,
		--  ItemStatus,
		--  ItemCode,
		--  ItemType,
		--  ItemDescription,
		--  SUM(BeginningStock) BeginningStock,
		--  SUM(StockIn) StockIn,
		--  SUM(StockOut) StockOut,
		--  SUM(EndingStock) EndingStock
		  
		--  FROM (
		--	SELECT
		--		a.InventoryDate,
		--		a.ItemStatus,
		--		a.ItemCode,
		--		a.LocationCode,
		--		a.UnitCode,
		--		c.ItemType,
		--		c.ItemDescription,
		--		CASE WHEN b.DBeginningStock IS NULL THEN a.BeginningStock ELSE a.BeginningStock + b.DBeginningStock END     
		--		BeginningStock,
		--		--a.BeginningStock + b.DBeginningStock AS BeginningStock,
		--		CASE WHEN b.DStockIn IS NULL THEN a.StockIn ELSE a.StockIn + b.DStockIn END     
		--		StockIn,
		--		--a.StockIn + b.DStockIn AS StockIn,
		--		CASE WHEN b.DStockOut IS NULL THEN a.StockOut ELSE a.StockOut + b.DStockOut END     
		--		StockOut,
		--		--a.StockOut + b.DStockOut AS StockOut,
		--		CASE WHEN b.DEndingStock IS NULL THEN a.EndingStock ELSE a.EndingStock + b.DEndingStock END     
		--		EndingStock
		--		--a.EndingStock + b.DEndingStock AS EndingStock
		--	FROM (
		--		SELECT
		--			a.InventoryDate,
		--			a.ItemStatus,
		--			a.ItemCode,
		--			a.LocationCode,
		--			a.BeginningStock,
		--			a.StockIn,
		--			a.StockOut,
		--			a.EndingStock,
		--			a.UnitCode
		--		FROM dbo.MntcInventory a
		--		WHERE InventoryDate = @InvDate
		--		AND LocationCode = @LocCode
		--		--AND ItemStatus = 'BAD STOCK'
		--	) AS a
		--	LEFT JOIN (select * from GetDeltaViewFunction(@LocCode,@InvDate)) as b
			
		--	--dbo.MntcInventoryDeltaView AS b
		--	ON b.InventoryDate = a.InventoryDate
		--		AND b.ItemCode = a.ItemCode
		--		AND b.ItemStatus = a.ItemStatus
		--		AND b.LocationCode = a.LocationCode
		--		AND b.UnitCode = a.UnitCode
		--	INNER JOIN dbo.MstMntcItem AS c
		--		ON c.ItemCode = a.ItemCode
		--) as c
		--GROUP BY InventoryDate,
		--		 ItemCode,
		--		 LocationCode,
		--		 ItemType,
		--		 ItemDescription,
		--		 ItemStatus,
		--		 UnitCode

				
		-- END of codingan dari view [MaintenanceExecutionInventoryProcedure]
		--)mi
		INNER JOIN MstMntcItem mmi on mmi.ItemCode = mi.ItemCode
		where mi.LocationCode = @LocCode 
		AND mi.InventoryDate = @InvDate 
		AND mi.UnitCode LIKE + '%'+@Unit+'%'
) as InventoryTable
PIVOT (MAX(EndingStock) FOR ItemStatus IN (
	[In Transit],
	[QI],
	[Ready To Use],
	[Bad Stock],
	[On Used],
	[On Repair]
))as PivotTable




END;