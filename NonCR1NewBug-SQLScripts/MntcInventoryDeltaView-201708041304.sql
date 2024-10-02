USE [SKTIS]
GO

/****** Object:  View [dbo].[MntcInventoryDeltaView]    Script Date: 04-08-17 1:02:31 PM ******/
DROP VIEW [dbo].[MntcInventoryDeltaView]
GO

/****** Object:  View [dbo].[MntcInventoryDeltaView]    Script Date: 04-08-17 1:02:31 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO






CREATE VIEW [dbo].[MntcInventoryDeltaView]
AS
     SELECT InventoryDate,
            LocationCode,
            UnitCode,
            ItemStatus,
            ItemCode,
            SUM(isnull(DeltaStaw,0)) AS DBeginningStock,
            SUM(isnull(DeltaStockIn,0)) AS DStockIn,
            SUM(isnull(DeltaStockOut,0)) AS DStockOut,
            SUM(isnull(DeltaStak,0)) AS DEndingStock
     FROM(
--Luki: Item Convert (transaction date) Status: Source=Destination 
         SELECT b.InventoryDate,
                b.LocationCode,
                b.UnitCode,
                b.ItemStatus,
                b.ItemCode,
                0 DeltaStaw,
                0 DeltaStockIn,
                --SUM(SourceStock) AS DeltaStockOut,
                ---SUM(SourceStock) AS DeltaStak
				SUM(a.SourceQty) AS DeltaStockOut,
                -SUM(a.SourceQty) AS DeltaStak
         FROM (SELECT [TransactionDate]
      ,[LocationCode]
      ,[ItemCodeSource]
      ,[SourceStatus]
      ,[UpdatedDate]
      ,max([SourceQty]) as SourceQty
  FROM [SKTIS].[dbo].[MntcEquipmentItemConvert]
  group by TransactionDate,LocationCode,ItemCodeSource,SourceStatus,UpdatedDate) a,
              [MntcInventory] b
         WHERE a.TransactionDate = b.InventoryDate
           AND a.LocationCode = b.LocationCode
           AND b.UnitCode = 'MTNC'
           AND a.SourceStatus = b.ItemStatus
           AND a.ItemCodeSource = b.ItemCode
           AND a.SourceQty > 0
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
--Luki: Item Convert (Update next date) Status: Source=Destination
         SELECT b.InventoryDate,
                b.LocationCode,
                b.UnitCode,
                b.ItemStatus,
                b.ItemCode,
                -SUM(a.SourceQty) AS DeltaStaw,
                0 DeltaStockIn,
                0 DeltaStockOut,
                -SUM(a.SourceQty) AS DeltaStak
         FROM (SELECT [TransactionDate]
      ,[LocationCode]
      ,[ItemCodeSource]
      ,[SourceStatus]
      ,[UpdatedDate]
      ,max([SourceQty]) as SourceQty
  FROM [SKTIS].[dbo].[MntcEquipmentItemConvert]
  group by TransactionDate,LocationCode,ItemCodeSource,SourceStatus,UpdatedDate) a,
              [MntcInventory] b
         WHERE a.TransactionDate < b.InventoryDate
           AND a.LocationCode = b.LocationCode
           AND b.UnitCode = 'MTNC'
           AND a.SourceStatus = b.ItemStatus
           AND a.ItemCodeSource = b.ItemCode
           AND a.SourceQty > 0
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
--Luki: Item Convert (transaction date) Status Destination 'READY TO USE'
         SELECT b.InventoryDate,
                b.LocationCode,
                b.UnitCode,
                b.ItemStatus,
                b.ItemCode,
                0 DeltaStaw,
                SUM(a.QtyGood) AS DeltaStockIn,
                0 DeltaStockOut,
                SUM(a.QtyGood) AS DeltaStak
         FROM [MntcEquipmentItemConvert] a,
              [MntcInventory] b
         WHERE a.TransactionDate = b.InventoryDate
           AND a.LocationCode = b.LocationCode
           AND b.UnitCode = 'MTNC'
           AND b.ItemStatus = 'READY TO USE'
           AND a.ItemCodeDestination = b.ItemCode
           AND a.QtyGood > 0
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
--Luki: Item Convert (update next date) Status Destination 'READY TO USE'
         SELECT b.InventoryDate,
                b.LocationCode,
                b.UnitCode,
                b.ItemStatus,
                b.ItemCode,
                +SUM(QtyGood) AS DeltaStaw,
                0 DeltaStockIn,
                0 DeltaStockOut,
                +SUM(a.QtyGood) AS DeltaStak
         FROM [MntcEquipmentItemConvert] a,
              [MntcInventory] b
         WHERE a.TransactionDate < b.InventoryDate
           AND a.LocationCode = b.LocationCode
           AND b.UnitCode = 'MTNC'
           AND b.ItemStatus = 'READY TO USE'
           AND a.ItemCodeDestination = b.ItemCode
           AND a.QtyGood > 0
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
--Luki: Item Convert (transaction date) Status Destination 'BAD STOCK'
         SELECT b.InventoryDate,
                b.LocationCode,
                b.UnitCode,
                b.ItemStatus,
                b.ItemCode,
                0 DeltaStaw,
                +SUM(a.QtyDisposal) AS  DeltaStockIn,
                0 DeltaStockOut,
                +SUM(a.QtyDisposal) AS DeltaStak
         FROM [MntcEquipmentItemConvert] a,
              [MntcInventory] b
         WHERE a.TransactionDate = b.InventoryDate
           AND a.LocationCode = b.LocationCode
           AND b.UnitCode = 'MTNC'
           AND b.ItemStatus = 'BAD STOCK'
           AND a.ConversionType = 1
           AND a.ItemCodeDestination = b.ItemCode
           AND a.QtyDisposal > 0
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
--Luki: Item Convert (update next date) Status Destination 'BAD STOCK'
         SELECT b.InventoryDate,
                b.LocationCode,
                b.UnitCode,
                b.ItemStatus,
                b.ItemCode,
                +SUM(a.QtyDisposal) AS DeltaStaw,
                0 DeltaStockIn,
                0 DeltaStockOut,
                +SUM(a.QtyDisposal) AS DeltaStak
         FROM [MntcEquipmentItemConvert] a,
              [MntcInventory] b
         WHERE a.TransactionDate < b.InventoryDate
           AND a.LocationCode = b.LocationCode
           AND b.UnitCode = 'MTNC'
           AND b.ItemStatus = 'BAD STOCK'
           AND a.ConversionType = 1
           AND a.ItemCodeDestination = b.ItemCode
           AND a.QtyDisposal > 0
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
--Luki: Item Disposal (transaction date) Status 'BAD STOCK'
         SELECT b.InventoryDate,
                b.LocationCode,
                b.UnitCode,
                b.ItemStatus,
                b.ItemCode,
                0 DeltaStaw,
                0 DeltaStockIn,
                +SUM(a.QtyDisposal) AS DeltaStockOut,
                -SUM(a.QtyDisposal) AS DeltaStak
         FROM [MntcEquipmentItemDisposal] a,
              [MntcInventory] b
         WHERE a.TransactionDate = b.InventoryDate
           AND a.LocationCode = b.LocationCode
           AND b.UnitCode = 'MTNC'
           AND b.ItemStatus = 'BAD STOCK'
           AND a.ItemCode = b.ItemCode
           AND a.QtyDisposal > 0
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
--Luki: Item Disposal (update next date) Status Source 'BAD STOCK'
         SELECT b.InventoryDate,
                b.LocationCode,
                b.UnitCode,
                b.ItemStatus,
                b.ItemCode,
                -SUM(a.QtyDisposal) AS DeltaStaw,
                0 DeltaStockIn,
                0 DeltaStockOut,
                -SUM(a.QtyDisposal) AS DeltaStak
         FROM [MntcEquipmentItemDisposal] a,
              [MntcInventory] b
         WHERE a.TransactionDate < b.InventoryDate
           AND a.LocationCode = b.LocationCode
           AND b.UnitCode = 'MTNC'
           AND b.ItemStatus = 'BAD STOCK'
           AND a.ItemCode = b.ItemCode
           AND a.QtyDisposal > 0
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL

-----------------------------------------------
--Luki: Item Movement (transaction date) source
         SELECT b.InventoryDate,
                b.LocationCode,
                b.UnitCode,
                b.ItemStatus,
                b.ItemCode,
                0 DeltaStak,
                0 DeltaStockIn,
                SUM(QtyTransfer) AS DeltaStockOut,
                -SUM(QtyTransfer) AS DeltaStaw
         FROM [MntcEquipmentMovement] a,
              [MntcInventory] b
         WHERE a.TransferDate = b.InventoryDate
           AND a.LocationCodeSource = b.LocationCode
           AND (( a.LocationCodeSource LIKE 'REG%'
              AND b.UnitCode = 'WHSE'
              AND b.ItemStatus = 'IN TRANSIT'
                )
             OR ( a.LocationCodeSource LIKE 'ID%'
              AND b.UnitCode = 'MTNC'
              AND b.ItemStatus = 'READY TO USE'
                )
               )
           AND a.ItemCode = b.ItemCode
           AND a.QtyTransfer > 0
           AND CONVERT(VARCHAR(10),a.CreatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
           -- update from  UpdateDate to CreatedDate avoid stock taken twice
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
--Luki: Item Movement (update next date) source
         SELECT b.InventoryDate,
                b.LocationCode,
                b.UnitCode,
                b.ItemStatus,
                b.ItemCode,
                -SUM(QtyTransfer) AS DeltaStaw,
                0 DeltaStockIn,
                0 DeltaStockOut,
                -SUM(QtyTransfer) AS DeltaStaw
         FROM [MntcEquipmentMovement] a,
              [MntcInventory] b
         WHERE a.TransferDate < b.InventoryDate
           AND a.LocationCodeSource = b.LocationCode
           AND (( a.LocationCodeSource LIKE 'REG%'
              AND b.UnitCode = 'WHSE'
              AND b.ItemStatus = 'IN TRANSIT'
                )
             OR ( a.LocationCodeSource LIKE 'ID%'
              AND b.UnitCode = 'MTNC'
              AND b.ItemStatus = 'READY TO USE'
                )
               )
           AND a.ItemCode = b.ItemCode
           AND a.QtyTransfer > 0
           AND CONVERT(VARCHAR(10),a.CreatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
           -- update from  UpdateDate to CreatedDate avoid stock taken twice 
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
--Luki: Item Movement (transaction date) destination
         SELECT b.InventoryDate,
                b.LocationCode,
                b.UnitCode,
                b.ItemStatus,
                b.ItemCode,
                0 DeltaStaw,
                SUM(QtyReceive) AS DeltaStockIn,
                0 DeltaStockOut,
                SUM(QtyReceive) AS DeltaStak
         FROM [MntcEquipmentMovement] a,
              [MntcInventory] b
         WHERE a.ReceiveDate = b.InventoryDate
           AND a.LocationCodeDestination = b.LocationCode
           AND a.UnitCodeDestination = b.UnitCode
           AND (( a.LocationCodeDestination LIKE 'REG%'
              AND b.UnitCode = 'WHSE'
              AND b.ItemStatus = 'IN TRANSIT'
                )
             OR ( a.LocationCodeDestination LIKE 'ID%'
              AND b.UnitCode = 'MTNC'
              AND b.ItemStatus = 'READY TO USE'
                )
             OR ( a.LocationCodeDestination LIKE 'ID%'
              AND b.UnitCode <> 'MTNC'
              AND b.ItemStatus = 'ON USED'
                )
               )
           AND a.ItemCode = b.ItemCode
           AND a.QtyReceive > 0
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
--Luki: Item Movement (update next date) destination
         SELECT b.InventoryDate,
                b.LocationCode,
                b.UnitCode,
                b.ItemStatus,
                b.ItemCode,
                SUM(QtyReceive) AS DeltaStaw,
                0 DeltaStockIn,
                0 DeltaStockOut,
                SUM(QtyReceive) AS DeltaStak
         FROM [MntcEquipmentMovement] a,
              [MntcInventory] b
         WHERE a.ReceiveDate < b.InventoryDate
           AND a.LocationCodeDestination = b.LocationCode
           AND a.UnitCodeDestination = b.UnitCode
           AND (( a.LocationCodeDestination LIKE 'REG%'
              AND b.UnitCode = 'WHSE'
              AND b.ItemStatus = 'IN TRANSIT'
                )
             OR ( a.LocationCodeDestination LIKE 'ID%'
              AND b.UnitCode = 'MTNC'
              AND b.ItemStatus = 'READY TO USE'
                )
             OR ( a.LocationCodeDestination LIKE 'ID%'
              AND b.UnitCode <> 'MTNC'
              AND b.ItemStatus = 'ON USED'
                )
               )
           AND a.ItemCode = b.ItemCode
           AND a.QtyReceive > 0
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL

-----------------------------------------------
--Luki: Quality Inspection (transaction date) 'IN TRANSIT'
         SELECT b.InventoryDate,
                b.LocationCode,
                b.UnitCode,
                b.ItemStatus,
                b.ItemCode,
                0 DeltaStaw,
                SUM(QTYTransit) as DeltaStockIn,
                0 DeltaStockOut,
                SUM(QTYTransit) AS DeltaStak
         FROM [MntcEquipmentQualityInspection] a,
              [MntcInventory] b
         WHERE a.TransactionDate = b.InventoryDate
           AND a.LocationCode = b.LocationCode
           AND b.UnitCode = 'MTNC'
           AND b.ItemStatus = 'IN TRANSIT'
           AND a.ItemCode = b.ItemCode
           --AND b.BeginningStock > 0
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
           AND a.QTYTransit > 0
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
--Luki: Quality Inspection (update next date) 'IN TRANSIT'
         SELECT b.InventoryDate,
                b.LocationCode,
                b.UnitCode,
                b.ItemStatus,
                b.ItemCode,
                SUM(QTYTransit) AS DeltaStaw,
                0 DeltaStockIn,
                0 DeltaStockOut,
                SUM(QtyReceiving)AS DeltaStak
         FROM [MntcEquipmentQualityInspection] a,
              [MntcInventory] b
         WHERE a.TransactionDate < b.InventoryDate
           AND a.LocationCode = b.LocationCode
           AND b.UnitCode = 'MTNC'
           AND b.ItemStatus = 'IN TRANSIT'
           AND a.ItemCode = b.ItemCode
           --AND b.BeginningStock > 0
           AND a.QTYTransit > 0
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
--Luki: Quality Inspection (transaction date) 'QUALITY INSPECTION'
         SELECT b.InventoryDate,
                b.LocationCode,
                b.UnitCode,
                b.ItemStatus,
                b.ItemCode,
                0 DeltaStaw,
                SUM(QtyReceiving) AS DeltaStockIn,
                SUM(QtyPass + QtyReject + QtyReturn) AS DeltaStockOut,
                SUM(QtyReceiving - ( QtyPass + QtyReject + QtyReturn )) AS DeltaStak
         FROM [MntcEquipmentQualityInspection] a,
              [MntcInventory] b
         WHERE a.TransactionDate = b.InventoryDate
           AND a.LocationCode = b.LocationCode
           AND b.UnitCode = 'MTNC'
           AND b.ItemStatus = 'QUALITY INSPECTION'
           AND a.ItemCode = b.ItemCode
           AND (a.QtyReceiving > 0 or QtyPass > 0 or QtyReject > 0 or QtyReturn > 0)
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
--Luki: Quality Inspection (update next date) 'QUALITY INSPECTION'
         SELECT b.InventoryDate,
                b.LocationCode,
                b.UnitCode,
                b.ItemStatus,
                b.ItemCode,
                SUM(QtyReceiving - ( QtyPass + QtyReject + QtyReturn )) AS DeltaStaw,
                0 DeltaStockIn,
                0 DeltaStockOut,
                SUM(QtyReceiving - ( QtyPass + QtyReject + QtyReturn )) AS DeltaStak
         FROM [MntcEquipmentQualityInspection] a,
              [MntcInventory] b
         WHERE a.TransactionDate < b.InventoryDate
           AND a.LocationCode = b.LocationCode
           AND b.UnitCode = 'MTNC'
           AND b.ItemStatus = 'QUALITY INSPECTION'
           AND a.ItemCode = b.ItemCode
           AND (a.QtyReceiving > 0 or QtyPass > 0 or QtyReject > 0 or QtyReturn > 0)
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
--Luki: Quality Inspection (transaction date) 'READY TO USE'
         SELECT b.InventoryDate,
                b.LocationCode,
                b.UnitCode,
                b.ItemStatus,
                b.ItemCode,
                0 DeltaStaw,
                SUM(QtyPass) AS DeltaStockIn,
                0 DeltaStockOut,
                SUM(QtyPass) AS DeltaStak
         FROM [MntcEquipmentQualityInspection] a,
              [MntcInventory] b
         WHERE a.TransactionDate = b.InventoryDate
           AND a.LocationCode = b.LocationCode
           AND b.UnitCode = 'MTNC'
           AND b.ItemStatus = 'READY TO USE'
           AND a.ItemCode = b.ItemCode
           AND a.QtyPass > 0
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
--Luki: Quality Inspection (update next date) 'READY TO USE'
         SELECT b.InventoryDate,
                b.LocationCode,
                b.UnitCode,
                b.ItemStatus,
                b.ItemCode,
                SUM(QtyPass) AS DeltaStaw,
                0 DeltaStockIn,
                0 DeltaStockOut,
                SUM(QtyPass) AS DeltaStak
         FROM [MntcEquipmentQualityInspection] a,
              [MntcInventory] b
         WHERE a.TransactionDate < b.InventoryDate
           AND a.LocationCode = b.LocationCode
           AND b.UnitCode = 'MTNC'
           AND b.ItemStatus = 'READY TO USE'
           AND a.ItemCode = b.ItemCode
           AND a.QtyPass > 0
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
--Luki: Quality Inspection (transaction date) 'BAD STOCK'
         SELECT b.InventoryDate,
                b.LocationCode,
                b.UnitCode,
                b.ItemStatus,
                b.ItemCode,
                0 DeltaStaw,
                SUM(QtyReject) AS DeltaStockIn,
                0 DeltaStockOut,
                SUM(QtyReject) AS DeltaStak
         FROM [MntcEquipmentQualityInspection] a,
              [MntcInventory] b
         WHERE a.TransactionDate = b.InventoryDate
           AND a.LocationCode = b.LocationCode
           AND b.UnitCode = 'MTNC'
           AND b.ItemStatus = 'BAD STOCK'
           AND a.ItemCode = b.ItemCode
           AND a.QtyReject > 0
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
--Luki: Quality Inspection (update next date) 'BAD STOCK'
         SELECT b.InventoryDate,
                b.LocationCode,
                b.UnitCode,
                b.ItemStatus,
                b.ItemCode,
                SUM(QtyReject) AS DeltaStaw,
                0 DeltaStockIn,
                0 DeltaStockOut,
                SUM(QtyReject) AS DeltaStak
         FROM [MntcEquipmentQualityInspection] a,
              [MntcInventory] b
         WHERE a.TransactionDate < b.InventoryDate
           AND a.LocationCode = b.LocationCode
           AND b.UnitCode = 'MTNC'
           AND b.ItemStatus = 'BAD STOCK'
           AND a.ItemCode = b.ItemCode
           AND a.QtyReject > 0
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
--Luki: Repair (transaction date) 'ON REPAIR'
         SELECT b.InventoryDate,
                b.LocationCode,
                b.UnitCode,
                b.ItemStatus,
                b.ItemCode,
                0 DeltaStaw,
                SUM(QtyRepairRequest) AS DeltaStockIn,
                SUM(QtyCompletion + QtyBadStock) AS DeltaStockOut,
                SUM(QtyRepairRequest - ( QtyCompletion + QtyBadStock )) AS DeltaStak
         FROM [MntcEquipmentRepair] a,
              [MntcInventory] b
         WHERE a.TransactionDate = b.InventoryDate
           AND a.LocationCode = b.LocationCode
           AND a.UnitCode = b.UnitCode
           AND b.ItemStatus = 'ON REPAIR'
           AND a.ItemCode = b.ItemCode
           AND (a.QtyRepairRequest > 0 or a.QtyCompletion > 0 or a.QtyBadStock > 0)
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
--Luki: Repair (update next date) 'ON REPAIR'
         SELECT b.InventoryDate,
                b.LocationCode,
                b.UnitCode,
                b.ItemStatus,
                b.ItemCode,
                SUM(QtyRepairRequest - ( QtyCompletion + QtyBadStock )) AS DeltaStaw,
                0 DeltaStockIn,
                0 DeltaStockOut,
                SUM(QtyRepairRequest - ( QtyCompletion + QtyBadStock )) AS DeltaStak
         FROM [MntcEquipmentRepair] a,
              [MntcInventory] b
         WHERE a.TransactionDate < b.InventoryDate
           AND a.LocationCode = b.LocationCode
           AND a.UnitCode = b.UnitCode
           AND b.ItemStatus = 'ON REPAIR'
           AND a.ItemCode = b.ItemCode
           AND (a.QtyRepairRequest > 0 or a.QtyCompletion > 0 or a.QtyBadStock > 0)
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
--Luki: Repair (transaction date) 'READY TO USE'
         SELECT b.InventoryDate,
                b.LocationCode,
                b.UnitCode,
                b.ItemStatus,
                b.ItemCode,
                0 DeltaStaw,
                SUM(QtyCompletion) AS DeltaStockIn,
                SUM(QtyTakenByUnit) AS DeltaStockOut,
                SUM(QtyCompletion - QtyTakenByUnit) AS DeltaStak
         FROM [MntcEquipmentRepair] a,
              [MntcInventory] b
         WHERE a.TransactionDate = b.InventoryDate
           AND a.LocationCode = b.LocationCode
           --AND a.UnitCode = b.UnitCode
           AND b.UnitCode = 'MTNC'
           AND b.ItemStatus = 'READY TO USE'
           AND a.ItemCode = b.ItemCode
           AND (a.QtyCompletion > 0 or a.QtyTakenByUnit > 0)
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
--Luki: Repair (update next date) 'READY TO USE'
         SELECT b.InventoryDate,
                b.LocationCode,
                b.UnitCode,
                b.ItemStatus,
                b.ItemCode,
                SUM(QtyCompletion - QtyTakenByUnit) AS DeltaStaw,
                0 DeltaStockIn,
                0 DeltaStockOut,
                SUM(QtyCompletion - QtyTakenByUnit) AS DeltaStak
         FROM [MntcEquipmentRepair] a,
              [MntcInventory] b
         WHERE a.TransactionDate < b.InventoryDate
           AND a.LocationCode = b.LocationCode
           --AND a.UnitCode = b.UnitCode
           AND b.UnitCode = 'MTNC'
           AND b.ItemStatus = 'READY TO USE'
           AND a.ItemCode = b.ItemCode
           AND (a.QtyCompletion > 0 or a.QtyTakenByUnit > 0)
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
--Luki: Repair (transaction date) 'ON USED'
         SELECT b.InventoryDate,
                b.LocationCode,
                b.UnitCode,
                b.ItemStatus,
                b.ItemCode,
                0 DeltaStaw,
                SUM(QtyTakenByUnit) AS DeltaStockIn,
                SUM(QtyRepairRequest) AS DeltaStockOut,
                SUM(QtyTakenByUnit - QtyRepairRequest) AS DeltaStak
         FROM [MntcEquipmentRepair] a,
              [MntcInventory] b
         WHERE a.TransactionDate = b.InventoryDate
           AND a.LocationCode = b.LocationCode
           AND a.UnitCode = b.UnitCode
           AND b.ItemStatus = 'ON USED'
           AND a.ItemCode = b.ItemCode
           AND (a.QtyTakenByUnit > 0 or a.QtyRepairRequest > 0)
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
--Luki: Repair (update next date) 'ON USED'
         SELECT b.InventoryDate,
                b.LocationCode,
                b.UnitCode,
                b.ItemStatus,
                b.ItemCode,
                SUM(QtyTakenByUnit - QtyRepairRequest)as DeltaStaw,
                0 DeltaStockIn,
                0 DeltaStockOut,
                SUM(QtyTakenByUnit - QtyRepairRequest) AS DeltaStak
         FROM [MntcEquipmentRepair] a,
              [MntcInventory] b
         WHERE a.TransactionDate < b.InventoryDate
           AND a.LocationCode = b.LocationCode
           AND a.UnitCode = b.UnitCode
           AND b.ItemStatus = 'ON USED'
           AND a.ItemCode = b.ItemCode
           AND (a.QtyTakenByUnit > 0 or a.QtyRepairRequest > 0)
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
--Luki: Repair (transaction date) 'BAD STOCK'
         SELECT b.InventoryDate,
                b.LocationCode,
                b.UnitCode,
                b.ItemStatus,
                b.ItemCode,
                0 DeltaStaw,
                SUM(QtyBadStock) AS DeltaStockIn,
                0 DeltaStockOut,
                SUM(QtyBadStock) AS DeltaStak
         FROM [MntcEquipmentRepair] a,
              [MntcInventory] b
         WHERE a.TransactionDate = b.InventoryDate
           AND a.LocationCode = b.LocationCode
           --AND a.UnitCode = 'MNTC'
           AND b.UnitCode = 'MTNC'
           AND b.ItemStatus = 'BAD STOCK'
           AND a.ItemCode = b.ItemCode
           AND a.QtyBadStock > 0
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
--Luki: Repair (update next date) 'BAD STOCK'
         SELECT b.InventoryDate,
                b.LocationCode,
                b.UnitCode,
                b.ItemStatus,
                b.ItemCode,
                SUM(QtyBadStock) AS DeltaStaw,
                0 DeltaStockIn,
                0 DeltaStockOut,
                SUM(QtyBadStock) AS DeltaStak
         FROM [MntcEquipmentRepair] a,
              [MntcInventory] b
         WHERE a.TransactionDate < b.InventoryDate
           AND a.LocationCode = b.LocationCode
           --AND a.UnitCode = 'MNTC'
           AND b.UnitCode = 'MTNC'
           AND b.ItemStatus = 'BAD STOCK'
           AND a.ItemCode = b.ItemCode
           AND a.QtyBadStock > 0
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
------Luki: Adjustment (transaction date) source
--         SELECT b.InventoryDate,
--                b.LocationCode,
--                b.UnitCode,
--                b.ItemStatus,
--                b.ItemCode,
--                0 DeltaStaw,
--                0 DeltaStockIn,
--                SUM(AdjustmentValue) AS DeltaStockOut,
--                -SUM(AdjustmentValue) AS DeltaStak
--         FROM [MntcInventoryAdjustment] a,
--              [MntcInventory] b
--         WHERE a.AdjustmentDate = b.InventoryDate
--           AND a.LocationCode = b.LocationCode
--           AND a.UnitCode = b.UnitCode
--           AND a.ItemStatusFrom = b.ItemStatus
--           AND a.ItemStatusFrom <> a.ItemStatusTo
--           AND a.ItemCode = b.ItemCode
--           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
--         GROUP BY b.InventoryDate,
--                  b.LocationCode,
--                  b.UnitCode,
--                  b.ItemStatus,
--                  b.ItemCode
--         UNION ALL
----Luki: Adjustment (update next date) source
--         SELECT b.InventoryDate,
--                b.LocationCode,
--                b.UnitCode,
--                b.ItemStatus,
--                b.ItemCode,
--                -SUM(AdjustmentValue) AS DeltaStaw,
--                0 DeltaStockIn,
--                0 DeltaStockOut,
--                -SUM(AdjustmentValue) AS DeltaStak
--         FROM [MntcInventoryAdjustment] a,
--              [MntcInventory] b
--         WHERE a.AdjustmentDate < b.InventoryDate
--           AND a.LocationCode = b.LocationCode
--           AND a.UnitCode = b.UnitCode
--           AND a.ItemStatusFrom = b.ItemStatus
--           AND a.ItemStatusFrom <> a.ItemStatusTo
--           AND a.ItemCode = b.ItemCode
--           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
--         GROUP BY b.InventoryDate,
--                  b.LocationCode,
--                  b.UnitCode,
--                  b.ItemStatus,
--                  b.ItemCode
--         UNION ALL
----Luki: Adjustment (transaction date) destination (add: 01/07/2016)
--         SELECT b.InventoryDate,
--                b.LocationCode,
--                b.UnitCode,
--                b.ItemStatus,
--                b.ItemCode,
--                0 DeltaStaw,
--                SUM(AdjustmentValue) AS DeltaStockIn,
--                0 DeltaStockOut,
--                SUM(AdjustmentValue) AS DeltaStak
--         FROM [MntcInventoryAdjustment] a,
--              [MntcInventory] b
--         WHERE a.AdjustmentDate = b.InventoryDate
--           AND a.LocationCode = b.LocationCode
--           AND a.UnitCodeDestination = b.UnitCode
--           AND a.ItemStatusTo = b.ItemStatus
--           AND a.ItemStatusFrom <> a.ItemStatusTo
--           AND a.ItemCode = b.ItemCode
--           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
--         GROUP BY b.InventoryDate,
--                  b.LocationCode,
--                  b.UnitCode,
--                  b.ItemStatus,
--                  b.ItemCode
--         UNION ALL
----Luki: Adjustment (update next date) destination (add: 01/07/2016)
--         SELECT b.InventoryDate,
--                b.LocationCode,
--                b.UnitCode,
--                b.ItemStatus,
--                b.ItemCode,
--                SUM(AdjustmentValue) AS DeltaStaw,
--                0 DeltaStockIn,
--                0 DeltaStockOut,
--                SUM(AdjustmentValue) AS DeltaStak
--         FROM [MntcInventoryAdjustment] a,
--              [MntcInventory] b
--         WHERE a.AdjustmentDate < b.InventoryDate
--           AND a.LocationCode = b.LocationCode
--           AND a.UnitCodeDestination = b.UnitCode
--           AND a.ItemStatusTo = b.ItemStatus
--           AND a.ItemStatusFrom <> a.ItemStatusTo
--           AND a.ItemCode = b.ItemCode
--           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
--         GROUP BY b.InventoryDate,
--                  b.LocationCode,
--                  b.UnitCode,
--                  b.ItemStatus,
--                  b.ItemCode
--         UNION ALL
----Luki: Adjustment (transaction date) status: (from = destination)
--         SELECT b.InventoryDate,
--                b.LocationCode,
--                b.UnitCode,
--                b.ItemStatus,
--                b.ItemCode,
--                0 DeltaStaw,
--                SUM(AdjustmentValue) * ISNULL(NULLIF(SIGN(SUM(AdjustmentValue)), -1),0) AS DeltaStockIn,
--                SUM(AdjustmentValue) * ISNULL(NULLIF(SIGN(SUM(AdjustmentValue)), 1),0) AS DeltaStockOut,
--                SUM(AdjustmentValue) AS DeltaStak
--         FROM [MntcInventoryAdjustment] a,
--              [MntcInventory] b
--         WHERE a.AdjustmentDate = b.InventoryDate
--           AND a.LocationCode = b.LocationCode
--           AND a.UnitCode = b.UnitCode
--           AND a.ItemStatusTo = b.ItemStatus
--           AND a.ItemCode = b.ItemCode
--           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
--         GROUP BY b.InventoryDate,
--                  b.LocationCode,
--                  b.UnitCode,
--                  b.ItemStatus,
--                  b.ItemCode
--         UNION ALL
----Luki: Adjustment (transaction date) status: (from = destination)
--         SELECT b.InventoryDate,
--                b.LocationCode,
--                b.UnitCode,
--                b.ItemStatus,
--                b.ItemCode,
--                SUM(AdjustmentValue) AS DeltaStaw,
--                0 DeltaStockIn,
--                0 DeltaStockOut,
--                SUM(AdjustmentValue) AS DeltaStak
--         FROM [MntcInventoryAdjustment] a,
--              [MntcInventory] b
--         WHERE a.AdjustmentDate < b.InventoryDate
--           AND a.LocationCode = b.LocationCode
--           AND a.UnitCode = b.UnitCode
--           AND a.ItemStatusTo = b.ItemStatus
--           AND a.ItemCode = b.ItemCode
--           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
--         GROUP BY b.InventoryDate,
--                  b.LocationCode,
--                  b.UnitCode,
--                  b.ItemStatus,
--                  b.ItemCode
--         UNION ALL
         ---- wahyu 27-10-2016 -> query change instruction fom Mr.Luki


--Luki: Adjustment (transaction date) source

SELECT
  b.InventoryDate,
  b.LocationCode,
  b.UnitCode,
  b.ItemStatus,
  b.ItemCode,
  0 DeltaStaw,
  0 DeltaStockIn,
  SUM(AdjustmentValue) AS DeltaStockOut,
  -SUM(AdjustmentValue) AS DeltaStak

FROM [MntcInventoryAdjustment] a,
     [MntcInventory] b

WHERE a.AdjustmentDate = b.InventoryDate

AND a.LocationCode = b.LocationCode
AND a.UnitCode = b.UnitCode
AND a.ItemStatusFrom = b.ItemStatus
AND a.ItemStatusFrom <> a.ItemStatusTo
AND a.ItemCode = b.ItemCode
AND CONVERT(varchar(10), a.UpdatedDate, 110) = CONVERT(varchar(10), GETDATE(), 110)


GROUP BY b.InventoryDate,
         b.LocationCode,
         b.UnitCode,
         b.ItemStatus,
         b.ItemCode

UNION ALL

--Luki: Adjustment (update next date) source

SELECT
  b.InventoryDate,
  b.LocationCode,
  b.UnitCode,
  b.ItemStatus,
  b.ItemCode,
  -SUM(AdjustmentValue) AS DeltaStaw,
  0 DeltaStockIn,
  0 DeltaStockOut,
  -SUM(AdjustmentValue) AS DeltaStak

FROM [MntcInventoryAdjustment] a,
     [MntcInventory] b

WHERE a.AdjustmentDate < b.InventoryDate
AND a.LocationCode = b.LocationCode
AND a.UnitCode = b.UnitCode
AND a.ItemStatusFrom = b.ItemStatus
AND a.ItemStatusFrom <> a.ItemStatusTo
AND a.ItemCode = b.ItemCode

AND CONVERT(varchar(10), a.UpdatedDate, 110) = CONVERT(varchar(10), GETDATE(), 110)

GROUP BY b.InventoryDate,
         b.LocationCode,
         b.UnitCode,
         b.ItemStatus,
         b.ItemCode

UNION ALL

--Luki: Adjustment (transaction date) destination (add: 01/⁠07/⁠2016)

SELECT
  b.InventoryDate,
  b.LocationCode,
  b.UnitCode,
  b.ItemStatus,
  b.ItemCode,
  0 DeltaStaw,
  SUM(AdjustmentValue) AS DeltaStockIn,
  0 DeltaStockOut,
  SUM(AdjustmentValue) AS DeltaStak

FROM [MntcInventoryAdjustment] a,
     [MntcInventory] b

WHERE a.AdjustmentDate = b.InventoryDate

AND a.LocationCode = b.LocationCode
AND a.UnitCodeDestination = b.UnitCode
AND a.ItemStatusTo = b.ItemStatus
AND a.ItemStatusFrom <> a.ItemStatusTo
AND a.ItemCode = b.ItemCode
AND CONVERT(varchar(10), a.UpdatedDate, 110) = CONVERT(varchar(10), GETDATE(), 110)

GROUP BY b.InventoryDate,
         b.LocationCode,
         b.UnitCode,
         b.ItemStatus,
         b.ItemCode

UNION ALL

--Luki: Adjustment (update next date) destination (add: 01/⁠07/⁠2016)

SELECT
  b.InventoryDate,
  b.LocationCode,
  b.UnitCode,
  b.ItemStatus,
  b.ItemCode,
  SUM(AdjustmentValue) AS DeltaStaw,
  0 DeltaStockIn,
  0 DeltaStockOut,
  SUM(AdjustmentValue) AS DeltaStak

FROM [MntcInventoryAdjustment] a,
     [MntcInventory] b

WHERE a.AdjustmentDate < b.InventoryDate

AND a.LocationCode = b.LocationCode
AND a.UnitCodeDestination = b.UnitCode
AND a.ItemStatusTo = b.ItemStatus
AND a.ItemStatusFrom <> a.ItemStatusTo
AND a.ItemCode = b.ItemCode
AND CONVERT(varchar(10), a.UpdatedDate, 110) = CONVERT(varchar(10), GETDATE(), 110)

GROUP BY b.InventoryDate,
         b.LocationCode,
         b.UnitCode,
         b.ItemStatus,
         b.ItemCode
UNION ALL

--Luki: Adjustment (transaction date) status: (from = destination)

SELECT
  b.InventoryDate,
  b.LocationCode,
  b.UnitCode,
  b.ItemStatus,
  b.ItemCode,
  0 DeltaStaw,
  SUM(AdjustmentValue) * ISNULL(NULLIF(SIGN(SUM(AdjustmentValue)), -1), 0)
  AS DeltaStockIn,
  SUM(AdjustmentValue) * ISNULL(NULLIF(SIGN(SUM(AdjustmentValue)), 1), 0)
  AS DeltaStockOut,
  SUM(AdjustmentValue) AS DeltaStak
FROM [MntcInventoryAdjustment] a,
     [MntcInventory] b

WHERE a.AdjustmentDate = b.InventoryDate
AND a.LocationCode = b.LocationCode
AND a.UnitCode = b.UnitCode
AND a.ItemStatusTo = a.ItemStatusFrom
AND a.ItemStatusTo = b.ItemStatus
AND a.ItemCode = b.ItemCode
AND CONVERT(varchar(10), a.UpdatedDate, 110) = CONVERT(varchar(10), GETDATE(), 110)

GROUP BY b.InventoryDate,
         b.LocationCode,
         b.UnitCode,
         b.ItemStatus,
         b.ItemCode

UNION ALL

--Luki: Adjustment (transaction date) status: (from = destination)

SELECT
  b.InventoryDate,
  b.LocationCode,
  b.UnitCode,
  b.ItemStatus,
  b.ItemCode,
  SUM(AdjustmentValue) AS DeltaStaw,
  0 DeltaStockIn,
  0 DeltaStockOut,
  SUM(AdjustmentValue) AS DeltaStak
FROM [MntcInventoryAdjustment] a,
     [MntcInventory] b
WHERE a.AdjustmentDate < b.InventoryDate
AND a.LocationCode = b.LocationCode
AND a.UnitCode = b.UnitCode
AND a.ItemStatusTo = a.ItemStatusFrom
AND a.ItemStatusTo = b.ItemStatus
AND a.ItemCode = b.ItemCode
AND CONVERT(varchar(10), a.UpdatedDate, 110) = CONVERT(varchar(10), GETDATE(), 110)

GROUP BY b.InventoryDate,
         b.LocationCode,
         b.UnitCode,
         b.ItemStatus,
         b.ItemCode
 
 UNION ALL
         
--Luki: ItemUsage (transaction date) 'READY TO USE'
         SELECT b.InventoryDate,
                b.LocationCode,
                b.UnitCode,
                b.ItemStatus,
                b.ItemCode,
                0 DeltaStaw,
                0 DeltaStockIn,
                SUM(QtyUsage) AS DeltaStockOut,
                -SUM(QtyUsage) AS DeltaStak
         FROM [MntcRepairItemUsage] a,
              [MntcInventory] b
         WHERE a.TransactionDate = b.InventoryDate
           AND a.LocationCode = b.LocationCode
           --AND a.UnitCode = b.UnitCode
           AND b.UnitCode = 'MTNC'
           AND b.ItemStatus = 'READY TO USE'
           AND a.ItemCodeDestination = b.ItemCode
           AND a.QtyUsage > 0
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
--Luki: ItemUsage (Update next date) 'READY TO USE'
         SELECT b.InventoryDate,
                b.LocationCode,
                b.UnitCode,
                b.ItemStatus,
                b.ItemCode,
                -SUM(QtyUsage) AS DeltaStaw,
                0 DeltaStockIn,
                0 DeltaStockOut,
                -SUM(QtyUsage) AS DeltaStak
         FROM [MntcRepairItemUsage] a,
              [MntcInventory] b
         WHERE a.TransactionDate < b.InventoryDate
           AND a.LocationCode = b.LocationCode
           --AND a.UnitCode = b.UnitCode
           AND b.UnitCode = 'MTNC'
           AND b.ItemStatus = 'READY TO USE'
           AND a.ItemCodeDestination = b.ItemCode
           AND a.QtyUsage > 0
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
--Luki: ItemUsage (transaction date) 'BAD STOCK'
         SELECT b.InventoryDate,
                b.LocationCode,
                b.UnitCode,
                b.ItemStatus,
                b.ItemCode,
                0 DeltaStaw,
                SUM(QtyUsage) AS DeltaStockIn,
                0 DeltaStockOut,
                SUM(QtyUsage) AS DeltaStak
         FROM [MntcRepairItemUsage] a,
              [MntcInventory] b
         WHERE a.TransactionDate = b.InventoryDate
           AND a.LocationCode = b.LocationCode
           --AND a.UnitCode = b.UnitCode
           AND b.UnitCode = 'MTNC'
           AND b.ItemStatus = 'BAD STOCK'
           AND a.ItemCodeDestination = b.ItemCode
           AND a.QtyUsage > 0
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
--Luki: ItemUsage (Update next date) 'BAD STOCK'
         SELECT b.InventoryDate,
                b.LocationCode,
                b.UnitCode,
                b.ItemStatus,
                b.ItemCode,
                SUM(QtyUsage) AS DeltaStaw,
                0 DeltaStockIn,
                0 DeltaStockOut,
                SUM(QtyUsage) AS DeltaStak
         FROM [MntcRepairItemUsage] a,
              [MntcInventory] b
         WHERE a.TransactionDate < b.InventoryDate
           AND a.LocationCode = b.LocationCode
           --AND a.UnitCode = b.UnitCode
           AND b.UnitCode = 'MTNC'
           AND b.ItemStatus = 'BAD STOCK'
           AND a.ItemCodeDestination = b.ItemCode
           AND a.QtyUsage > 0
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode ) AS x
     WHERE not (x.DeltaStaw = 0 and x.DeltaStockIn = 0 and x.DeltaStockOut = 0 and x.DeltaStak = 0)             
     GROUP BY InventoryDate,
              LocationCode,
              UnitCode,
              ItemStatus,
              ItemCode;









GO


