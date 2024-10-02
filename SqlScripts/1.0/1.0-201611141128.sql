/****** Object:  View [dbo].[MntcFulfillmentView]    Script Date: 11/14/2016 12:00:02 ******/
IF  EXISTS (SELECT * FROM sys.views WHERE object_id = OBJECT_ID(N'[dbo].[MntcFulfillmentView]'))
DROP VIEW [dbo].[MntcFulfillmentView]
GO


-- Description: Create view MntcFulfillmentView, same like MaintenanceEquipmentFulfillmentView but get from MntcStockView
-- Ticket: http://tp.voxteneo.co.id/entity/10936
-- Author: Hakim
-- date	: 2016-11-01

-- Description: TPO cannot select from past
-- Ticket: http://tp.voxteneo.co.id/entity/10936
-- Author: Hakim
-- date	: 2016-11-01

CREATE VIEW [dbo].[MntcFulfillmentView]
AS
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
      ,sum (ReadyToUse) as ReadyToUse
      ,sum (Used) as OnUse
      ,sum(Repair) as OnRepair
  --FROM dbo.MntcStockView
  FROM TemporaryTableEquipmentStock
  --where CONVERT(VARCHAR(10),InventoryDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
  group by [ItemCode]
      ,[ItemDescription]
      ,[LocationCode]) y
 where  x.ItemCode = y.ItemCode;     


GO


