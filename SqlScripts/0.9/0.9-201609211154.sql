-- Description: ALTER VIEW
-- Ticket: http://tp.voxteneo.com/entity/58591
-- Author: Oka

-- Description: ALTER VIEW
-- Ticket: join dbo.MntcInventoryDeltaView dipindah ke dbo.TableDeltaView
-- Author: Wahyu

ALTER VIEW [dbo].[MaintenanceExecutionInventoryView]
AS
SELECT
  InventoryDate,
  ItemCode,
  LocationCode,
  ItemType,
  ItemDescription,
  SUM(CASE
    WHEN ItemStatus = 'IN TRANSIT' THEN BeginningStock
    ELSE 0
  END)
  AS StawIT,
  SUM(CASE
    WHEN ItemStatus = 'IN TRANSIT' THEN StockIn
    ELSE 0
  END) AS InIT,
  SUM(CASE
    WHEN ItemStatus = 'IN TRANSIT' THEN StockOut
    ELSE 0
  END) AS OutIT,
  SUM(CASE
    WHEN ItemStatus = 'IN TRANSIT' THEN EndingStock
    ELSE 0
  END) AS StackIT,
  SUM(CASE
    WHEN ItemStatus = 'QUALITY INSPECTION' THEN BeginningStock
    ELSE 0
  END) AS StawQI,
  SUM(CASE
    WHEN ItemStatus = 'QUALITY INSPECTION' THEN StockIn
    ELSE 0
  END) AS InQI,
  SUM(CASE
    WHEN ItemStatus = 'QUALITY INSPECTION' THEN StockOut
    ELSE 0
  END) AS OutQI,
  SUM(CASE
    WHEN ItemStatus = 'QUALITY INSPECTION' THEN EndingStock
    ELSE 0
  END) AS StackQI,
  SUM(CASE
    WHEN ItemStatus = 'READY TO USE' THEN BeginningStock
    ELSE 0
  END) AS StawReady,
  SUM(CASE
    WHEN ItemStatus = 'READY TO USE' THEN StockIn
    ELSE 0
  END) AS InReady,
  SUM(CASE
    WHEN ItemStatus = 'READY TO USE' THEN StockOut
    ELSE 0
  END) AS OutReady,
  SUM(CASE
    WHEN ItemStatus = 'READY TO USE' THEN EndingStock
    ELSE 0
  END) AS StackReady,
  SUM(CASE
    WHEN ItemStatus = 'ON USED' THEN BeginningStock
    ELSE 0
  END) AS StawOU,
  SUM(CASE
    WHEN ItemStatus = 'ON USED' THEN StockIn
    ELSE 0
  END) AS InOU,
  SUM(CASE
    WHEN ItemStatus = 'ON USED' THEN StockOut
    ELSE 0
  END)
  AS OutOU,
  SUM(CASE
    WHEN ItemStatus = 'ON USED' THEN EndingStock
    ELSE 0
  END) AS StackOU,
  SUM(CASE
    WHEN ItemStatus = 'ON REPAIR' THEN BeginningStock
    ELSE 0
  END) AS StawOR,
  SUM(CASE
    WHEN ItemStatus = 'ON REPAIR' THEN StockIn
    ELSE 0
  END) AS InOR,
  SUM(CASE
    WHEN ItemStatus = 'ON REPAIR' THEN StockOut
    ELSE 0
  END)
  AS OutOR,
  SUM(CASE
    WHEN ItemStatus = 'ON REPAIR' THEN EndingStock
    ELSE 0
  END) AS StackOR,
  SUM(CASE
    WHEN ItemStatus = 'BAD STOCK' THEN BeginningStock
    ELSE 0
  END) AS StawBS,
  SUM(CASE
    WHEN ItemStatus = 'BAD STOCK' THEN StockIn
    ELSE 0
  END) AS InBS,
  SUM(CASE
    WHEN ItemStatus = 'BAD STOCK' THEN StockOut
    ELSE 0
  END)
  AS OutBS,
  SUM(CASE
    WHEN ItemStatus = 'BAD STOCK' THEN EndingStock
    ELSE 0
  END) AS StackBS
FROM (SELECT
  a.InventoryDate,
  a.ItemStatus,
  a.ItemCode,
  a.LocationCode,
  b.ItemType,
  b.ItemDescription,
  a.BeginningStock,
  a.StockIn,
  a.StockOut,
  a.EndingStock
FROM dbo.MntcInventory AS a
INNER JOIN dbo.MstMntcItem AS b
  ON b.ItemCode = a.ItemCode
WHERE (NOT EXISTS (SELECT
  1 AS Expr1
--FROM dbo.MntcInventoryDeltaView
FROM dbo.TableDeltaView
WHERE (InventoryDate = a.InventoryDate)
AND (LocationCode = a.LocationCode)
AND (UnitCode = a.UnitCode)
AND (ItemStatus = a.ItemStatus)
AND (ItemCode = a.ItemCode))
)
UNION ALL
SELECT
  a.InventoryDate,
  a.ItemStatus,
  a.ItemCode,
  a.LocationCode,
  c.ItemType,
  c.ItemDescription,
  a.BeginningStock + b.DBeginningStock AS BeginningStock,
  a.StockIn + b.DStockIn AS StockIn,
  a.StockOut + b.DStockOut AS StockOut,
  a.EndingStock + b.DEndingStock AS EndingStock
FROM dbo.MntcInventory AS a
--INNER JOIN dbo.MntcInventoryDeltaView AS b
INNER JOIN dbo.TableDeltaView AS b
  ON b.InventoryDate = a.InventoryDate
  AND b.ItemCode = a.ItemCode
  AND b.ItemStatus = a.ItemStatus
  AND b.LocationCode = a.LocationCode
  AND b.UnitCode = a.UnitCode
INNER JOIN dbo.MstMntcItem AS c
  ON c.ItemCode = a.ItemCode) AS Inventory
GROUP BY InventoryDate,
         ItemCode,
         LocationCode,
         ItemType,
         ItemDescription

GO