/****** Object:  View [dbo].[MntcInventoryAll]    Script Date: 5/24/2016 9:54:34 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- Description: Update MntcInventorAll ( adding unitcode)
-- Ticket: bug/8417 Point all the views related to Inventory to MtncinventoryViewAll
-- Author: Bagus

-- Description: rollback view to beginning
-- Author: azka

ALTER view [dbo].[MntcInventoryAll] as
SELECT a.InventoryDate,
       a.ItemStatus,
       a.ItemCode,
       a.LocationCode,
       b.ItemType,
       b.ItemDescription,
       BeginningStock,
       StockIn,
       StockOut,
       EndingStock,
	   a.UnitCode
FROM [MntcInventory] a
     INNER JOIN [MstMntcItem] b ON b.ItemCode = a.ItemCode
WHERE NOT EXISTS( 
                  SELECT 1
                  FROM [MntcInventoryDeltaView]
                  WHERE InventoryDate = a.InventoryDate
                    AND LocationCode = a.LocationCode
                    AND UnitCode = a.UnitCode
                    AND ItemStatus = a.ItemStatus
                    AND ItemCode = a.ItemCode )
UNION ALL
SELECT a.InventoryDate,
       a.ItemStatus,
       a.ItemCode,
       a.LocationCode,
       c.ItemType,
       c.ItemDescription,
       BeginningStock + DBeginningStock AS BeginningStock,
       StockIn + DStockIn AS StockIn,
       StockOut + DStockOut AS StockOut,
       EndingStock + DEndingStock AS EndingStock,
	   b.UnitCode
FROM [MntcInventory] a
     INNER JOIN [MntcInventoryDeltaView] b ON b.InventoryDate = a.InventoryDate
                                          AND b.ItemCode = a.ItemCode
                                          AND b.ItemStatus = a.ItemStatus
                                          AND b.LocationCode = a.LocationCode
                                          AND b.UnitCode = a.UnitCode
     INNER JOIN [MstMntcItem] c ON c.ItemCode = a.ItemCode



GO


