-- Description: Alter MaintenanceEquipmentFulfillmentView
-- Ticket: http://tp.voxteneo.com/entity/59548
-- Author: Yudha

ALTER VIEW [dbo].[MaintenanceEquipmentFulfillmentView]
AS
	----SELECT table1.LocationCode, 
	----	   table1.RequestDate, 
	----	   table1.RequestNumber, 
	----	   table1.CreatedBy, 
	----	   table1.FulFillmentDate, 
	----	   table1.ItemCode, 
	----	   table1.ItemDescription, 
	----	   TblInv.StackReady AS ReadyToUse, 
	----	   TblInv.StackOU      AS OnUse, 
	----	   TblInv.StackOR   AS OnRepair, 
	----	   table1.Qty                         AS RequestedQuantity, 
	----	   table1.ApprovedQty, 
	----	   table1.RequestToQty, 
	----	   table1.ApprovedQty - CASE 
	----							  WHEN table1.RequestToQty IS NULL THEN 0 
	----							  ELSE table1.RequestToQty 
	----							END           AS PurchaseQuantity, 
	----	   table1.PurchaseNumber, 
	----	   table1.UpdatedDate 
	----FROM   (SELECT dbo.MntcEquipmentFulfillment.LocationCode, 
	----			   dbo.MntcEquipmentFulfillment.RequestDate, 
	----			   dbo.MntcEquipmentFulfillment.RequestNumber, 
	----			   dbo.MntcEquipmentFulfillment.CreatedBy, 
	----			   dbo.MntcEquipmentFulfillment.FulFillmentDate, 
	----			   dbo.MntcEquipmentFulfillment.ItemCode, 
	----			   dbo.MstMntcItem.ItemDescription, 
	----			   dbo.MntcEquipmentRequest.Qty, 
	----			   ( dbo.MntcEquipmentFulfillment.RequestToQty 
	----				 + dbo.MntcEquipmentFulfillment.PurchaseQty ) AS ApprovedQty, 
	----			   dbo.MntcEquipmentFulfillment.RequestToQty, 
	----			   dbo.MntcEquipmentFulfillment.PurchaseNumber, 
	----			   dbo.MntcEquipmentFulfillment.UpdatedDate 
	----		FROM   dbo.MntcEquipmentFulfillment 
	----			   INNER JOIN dbo.MstMntcItem 
	----					   ON dbo.MntcEquipmentFulfillment.ItemCode = 
	----						  dbo.MstMntcItem.ItemCode 
	----			   INNER JOIN dbo.MntcEquipmentRequest 
	----					   ON dbo.MntcEquipmentFulfillment.RequestDate = 
	----						  dbo.MntcEquipmentRequest.RequestDate 
	----						  AND dbo.MntcEquipmentFulfillment.ItemCode = 
	----							  dbo.MntcEquipmentRequest.ItemCode 
	----						  AND dbo.MntcEquipmentFulfillment.LocationCode = 
	----							  dbo.MntcEquipmentRequest.LocationCode 
	----						  AND dbo.MntcEquipmentFulfillment.RequestNumber = 
	----							  dbo.MntcEquipmentRequest.RequestNumber) AS table1 
	----	   CROSS APPLY (SELECT TOP(1) InventoryDate, 
	----								  ItemCode, 
	----								  LocationCode, 
	----								  ItemType, 
	----								  ItemDescription, 
	----								  Sum(CASE 
	----										WHEN ItemStatus = 'READY TO USE' THEN 
	----										EndingStock 
	----										ELSE 0 
	----									  END) AS StackReady, 
	----								  Sum(CASE 
	----										WHEN ItemStatus = 'ON USED' THEN 
	----										EndingStock 
	----										ELSE 0 
	----									  END) AS StackOU, 
	----								  Sum(CASE 
	----										WHEN ItemStatus = 'ON REPAIR' THEN 
	----										EndingStock 
	----										ELSE 0 
	----									  END) AS StackOR 
	----					FROM   (SELECT a.InventoryDate, 
	----								   a.ItemStatus, 
	----								   a.ItemCode, 
	----								   a.LocationCode, 
	----								   b.ItemType, 
	----								   b.ItemDescription, 
	----								   a.BeginningStock, 
	----								   a.StockIn, 
	----								   a.StockOut, 
	----								   a.EndingStock 
	----							FROM   dbo.MntcInventory AS a 
	----								   INNER JOIN dbo.MstMntcItem AS b 
	----										   ON b.ItemCode = a.ItemCode 
	----							WHERE  ( NOT EXISTS 
	----									 (SELECT 1 AS Expr1 
	----									  FROM   dbo.MntcInventoryDeltaView 
	----									  WHERE  ( InventoryDate = 
	----											   a.InventoryDate ) 
	----											 AND ( LocationCode = 
	----												   a.LocationCode ) 
	----											 AND ( UnitCode = a.UnitCode ) 
	----											 AND ( ItemStatus = a.ItemStatus 
	----												 ) 
	----											 AND ( ItemCode = a.ItemCode )) 
	----								   ) 
	----							UNION ALL 
	----							SELECT a.InventoryDate, 
	----								   a.ItemStatus, 
	----								   a.ItemCode, 
	----								   a.LocationCode, 
	----								   c.ItemType, 
	----								   c.ItemDescription, 
	----								   a.BeginningStock + b.DBeginningStock AS 
	----								   BeginningStock, 
	----								   a.StockIn + b.DStockIn               AS 
	----								   StockIn, 
	----								   a.StockOut + b.DStockOut             AS 
	----								   StockOut, 
	----								   a.EndingStock + b.DEndingStock       AS 
	----								   EndingStock 
	----							FROM   dbo.MntcInventory AS a 
	----								   INNER JOIN dbo.MntcInventoryDeltaView AS b 
	----										   ON b.InventoryDate = a.InventoryDate 
	----											  AND b.ItemCode = a.ItemCode 
	----											  AND b.ItemStatus = a.ItemStatus 
	----											  AND b.LocationCode = 
	----												  a.LocationCode 
	----											  AND b.UnitCode = a.UnitCode 
	----								   INNER JOIN dbo.MstMntcItem AS c 
	----										   ON c.ItemCode = a.ItemCode 
	----							UNION ALL 
	----							SELECT table1.RequestDate, 
	----								   '', 
	----								   table1.ItemCode, 
	----								   table1.LocationCode, 
	----								   '', 
	----								   '', 
	----								   '', 
	----								   '', 
	----								   '', 
	----								   '') AS Inventory 
	----					WHERE  LocationCode = table1.LocationCode 
	----						   AND ItemCode = table1.ItemCode 
	----						   AND InventoryDate = table1.RequestDate 
	----					GROUP  BY InventoryDate, 
	----							  ItemCode, 
	----							  LocationCode, 
	----							  ItemType, 
	----							  ItemDescription 
	----					ORDER  BY ItemType DESC) AS TblInv;
	
--	Luki HMS 17/06/2016

	SELECT x.LocationCode
      ,x.RequestDate
      ,x.RequestNumber
      ,x.CreatedBy
      ,x.FulFillmentDate
      ,y.ItemCode
      ,y.ItemDescription
      ,y.ReadyToUse
      ,y.OnUse
      ,y.OnRepair
      ,x.RequestedQty RequestedQuantity
      ,x.ApprovedQty
      ,x.RequestToQty
      ,x.PurchaseQty PurchaseQuantity      
      ,x.PurchaseNumber
      ,x.UpdatedDate
 FROM ( 
 select a.LocationCode,a.RequestDate,a.RequestNumber,b.CreatedBy,a.FulFillmentDate,a.ItemCode,b.Qty RequestedQty,b.ApprovedQty,
 a.RequestToQty,a.PurchaseQty,a.PurchaseNumber,a.UpdatedDate
 from dbo.MntcEquipmentFulfillment a,
 dbo.MntcEquipmentRequest b
 where a.RequestNumber = b.RequestNumber
 and a.ItemCode = b.ItemCode
 UNION ALL
 Select LocationCode,RequestDate,RequestNumber,CreatedBy, NULL FulFillmentDate,ItemCode,Qty RequestedQty, 0 ApprovedQty,
 0 RequestToQty ,0 PurchaseQty, NULL PurchaseNumber,UpdatedDate
 from dbo.MntcEquipmentRequest a
 where not exists (select 1 from dbo.MntcEquipmentFulfillment 
 where RequestNumber = a.RequestNumber
 and ItemCode = a.ItemCode)) x,
 (SELECT [ItemCode]
      ,[ItemDescription]
      ,[LocationCode]
      ,sum(ReadyToUse) as ReadyToUse
      ,sum(Used) as OnUse
      ,sum(Repair) as OnRepair
  FROM dbo.MaintenanceEquipmentStockView
  where CONVERT(VARCHAR(10),InventoryDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
  group by [ItemCode]
      ,[ItemDescription]
      ,[LocationCode]) y
 where x.LocationCode = y.LocationCode
 and x.ItemCode = y.ItemCode;     


GO


