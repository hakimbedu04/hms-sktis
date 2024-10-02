/****** Object:  View [dbo].[MntcInventoryAll]    Script Date: 5/23/2016 3:48:02 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- Description: Update MntcInventorAll ( adding unitcode)
-- Ticket: bug/8417 Point all the views related to Inventory to MtncinventoryViewAll
-- Author: Bagus

-- =============================================
-- Description: edit on join MntcInventory
-- Ticket: http://tp.voxteneo.co.id/entity/6629
-- Author: AZKA
-- Updated: 1.0 - 5/23/2016
-- =============================================

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
       b.ItemStatus,
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
     JOIN [MntcInventoryDeltaView] b ON b.InventoryDate = a.InventoryDate
                                          AND b.ItemCode = a.ItemCode
                                          AND b.LocationCode = a.LocationCode
                                          AND b.UnitCode = a.UnitCode
     INNER JOIN [MstMntcItem] c ON c.ItemCode = a.ItemCode



GO


