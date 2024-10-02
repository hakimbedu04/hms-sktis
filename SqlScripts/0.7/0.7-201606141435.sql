-- Description: rollback view to beginning
-- Author: azka
-- Description: Equipment to sparepart fix change SUM to MAX
-- Author: abud
-- Author: Robby
-- Description: Inventory Adjustment fix duplicate stockOut
-- Author: Indra
-- Description: Mntc Equipment Repair fix calculation http://tp.voxteneo.co.id/entity/6666
-- Author: Hakim
-- Description: in transit out = 0 http://tp.voxteneo.co.id/entity/6594
-- Edited on 2016-09-13
-- Edited by Indra Permana
-- Description: stockout in MntcRepair, in transit from mntcQualityInspection

-- Edited on 2016-09-14
-- Edited by Indra Permana
-- Description: in transit from mntcQualityInspection, 
-- bad stock from mntcrepairitemusage, status from dan status to from mntc inventory adjustment


ALTER VIEW [dbo].[MntcInventoryDeltaView]
AS
     SELECT InventoryDate,
            LocationCode,
            UnitCode,
            ItemStatus,
            ItemCode,
            SUM(DeltaStaw) AS DBeginningStock,
            SUM(DeltaStockIn) AS DStockIn,
            SUM(DeltaStockOut) AS DStockOut,
            SUM(DeltaStak) AS DEndingStock
     FROM(
         SELECT b.InventoryDate,
                b.LocationCode,
                b.UnitCode,
                b.ItemStatus,
                b.ItemCode,
                0 DeltaStaw,
                0 DeltaStockIn,
                --SUM(SourceStock) AS DeltaStockOut,
                ---SUM(SourceStock) AS DeltaStak
			 MAX(a.SourceQty) AS DeltaStockOut,
                -MAX(a.SourceQty) AS DeltaStak
         FROM [MntcEquipmentItemConvert] a,
              [MntcInventory] b
         WHERE a.TransactionDate = b.InventoryDate
           AND a.LocationCode = b.LocationCode
           AND b.UnitCode = 'MTNC'
           AND a.SourceStatus = b.ItemStatus
           AND a.ItemCodeSource = b.ItemCode
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
         SELECT b.InventoryDate,
                b.LocationCode,
                b.UnitCode,
                b.ItemStatus,
                b.ItemCode,
                -MAX(a.SourceQty) AS DeltaStaw,
                0 DeltaStockIn,
                0 DeltaStockOut,
                -MAX(a.SourceQty) AS DeltaStak
         FROM [MntcEquipmentItemConvert] a,
              [MntcInventory] b
         WHERE a.TransactionDate < b.InventoryDate
           AND a.LocationCode = b.LocationCode
           AND b.UnitCode = 'MTNC'
           AND a.SourceStatus = b.ItemStatus
           AND a.ItemCodeSource = b.ItemCode
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
         SELECT b.InventoryDate,
                b.LocationCode,
                b.UnitCode,
                b.ItemStatus,
                b.ItemCode,
                0 DeltaStaw,
                MAX(QtyGood) AS DeltaStockIn,
                0 DeltaStockOut,
                MAX(a.QtyGood) AS DeltaStak
         FROM [MntcEquipmentItemConvert] a,
              [MntcInventory] b
         WHERE a.TransactionDate = b.InventoryDate
           AND a.LocationCode = b.LocationCode
           AND b.UnitCode = 'MTNC'
           AND b.ItemStatus = 'READY TO USE'
           AND a.ItemCodeDestination = b.ItemCode
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
         SELECT b.InventoryDate,
                b.LocationCode,
                b.UnitCode,
                b.ItemStatus,
                b.ItemCode,
                +MAX(QtyGood) AS DeltaStaw,
                0 DeltaStockIn,
                0 DeltaStockOut,
                +MAX(a.QtyGood) AS DeltaStak
         FROM [MntcEquipmentItemConvert] a,
              [MntcInventory] b
         WHERE a.TransactionDate < b.InventoryDate
           AND a.LocationCode = b.LocationCode
           AND b.UnitCode = 'MTNC'
           AND b.ItemStatus = 'READY TO USE'
           AND a.ItemCodeDestination = b.ItemCode
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
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
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
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
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
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
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
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
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
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
           AND (( a.LocationCodeSource LIKE 'REG%'
              AND b.UnitCode = 'WHSE'
              AND b.ItemStatus = 'IN TRANSIT'
                )
             OR ( a.LocationCodeSource LIKE 'ID%'
              AND b.UnitCode = 'MTNC'
              AND b.ItemStatus = 'READY TO USE'
                )
             OR ( a.LocationCodeSource LIKE 'ID%'
              AND b.UnitCode <> 'MTNC'
              AND b.ItemStatus = 'ON USED'
                )
               )
           AND a.ItemCode = b.ItemCode
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
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
           AND (( a.LocationCodeSource LIKE 'REG%'
              AND b.UnitCode = 'WHSE'
              AND b.ItemStatus = 'IN TRANSIT'
                )
             OR ( a.LocationCodeSource LIKE 'ID%'
              AND b.UnitCode = 'MTNC'
              AND b.ItemStatus = 'READY TO USE'
                )
             OR ( a.LocationCodeSource LIKE 'ID%'
              AND b.UnitCode <> 'MTNC'
              AND b.ItemStatus = 'ON USED'
                )
               )
           AND a.ItemCode = b.ItemCode
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
		 -- Mntc Quality Inspection 
		 -- In Transit
         SELECT b.InventoryDate,
                b.LocationCode,
                b.UnitCode,
                b.ItemStatus,
                b.ItemCode,
                0 DeltaStaw,
                SUM(QTYTransit) as DeltaStockIn,
                0 AS DeltaStockOut,
                SUM(QTYTransit) AS DeltaStak
         FROM [MntcEquipmentQualityInspection] a,
              [MntcInventory] b
         WHERE a.TransactionDate = b.InventoryDate
           AND a.LocationCode = b.LocationCode
           AND b.UnitCode = 'MTNC'
           AND b.ItemStatus = 'IN TRANSIT'
           AND a.ItemCode = b.ItemCode
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
         SELECT b.InventoryDate,
                b.LocationCode,
                b.UnitCode,
                b.ItemStatus,
                b.ItemCode,
                -SUM(QtyReceiving) AS DeltaStaw,
                 SUM(QTYTransit) as DeltaStockIn,
                0 DeltaStockOut,
                -SUM(QtyReceiving) AS DeltaStak
         FROM [MntcEquipmentQualityInspection] a,
              [MntcInventory] b
         WHERE a.TransactionDate < b.InventoryDate
           AND a.LocationCode = b.LocationCode
           AND b.UnitCode = 'MTNC'
           AND b.ItemStatus = 'IN TRANSIT'
           AND a.ItemCode = b.ItemCode
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
		 -- Mntc Quality Inspection 
		 -- Quality Inspection
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
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
         SELECT b.InventoryDate,
                b.LocationCode,
                b.UnitCode,
                b.ItemStatus,
                b.ItemCode,
                SUM(QtyReceiving - ( QtyPass + QtyReject + QtyReturn )) DeltaStaw,
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
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
		 -- Mntc Quality Inspection 
		 -- Ready To Use
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
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
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
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
		 -- Mntc Quality Inspection 
		 -- Bad Stock
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
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
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
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
		 -- Mntc Repair 
		 -- On Repair
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
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
         SELECT b.InventoryDate,
                b.LocationCode,
                b.UnitCode,
                b.ItemStatus,
                b.ItemCode,
                0 AS DeltaStaw,
                SUM(QtyRepairRequest) AS DeltaStockIn,
                SUM(QtyCompletion + QtyBadStock) AS DeltaStockOut,
                SUM(QtyRepairRequest - ( QtyCompletion + QtyBadStock )) AS DeltaStak
         FROM [MntcEquipmentRepair] a,
              [MntcInventory] b
         WHERE a.TransactionDate < b.InventoryDate
           AND a.LocationCode = b.LocationCode
           AND a.UnitCode = b.UnitCode
           AND b.ItemStatus = 'ON REPAIR'
           AND a.ItemCode = b.ItemCode
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
		 -- Mntc Repair 
		 -- Ready To Use
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
           AND b.UnitCode = 'MTNC'
           AND b.ItemStatus = 'READY TO USE'
           AND a.ItemCode = b.ItemCode
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
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
         WHERE a.TransactionDate < b.InventoryDate
           AND a.LocationCode = b.LocationCode
           AND b.UnitCode = 'MTNC'
           AND b.ItemStatus = 'READY TO USE'
           AND a.ItemCode = b.ItemCode
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
		 -- Mntc Repair 
		 -- On Used
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
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
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
         WHERE a.TransactionDate < b.InventoryDate
           AND a.LocationCode = b.LocationCode
           AND a.UnitCode = b.UnitCode
           AND b.ItemStatus = 'ON USED'
           AND a.ItemCode = b.ItemCode
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
		 -- Mntc Repair 
		 -- Bad Stock
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
           AND b.UnitCode = 'MTNC'
           AND b.ItemStatus = 'BAD STOCK'
           AND a.ItemCode = b.ItemCode
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
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
         WHERE a.TransactionDate < b.InventoryDate
           AND a.LocationCode = b.LocationCode
           AND b.UnitCode = 'MTNC'
           AND b.ItemStatus = 'BAD STOCK'
           AND a.ItemCode = b.ItemCode
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
		 -- Mntc Inventory Adjusment
		 -- Item Status From
         SELECT b.InventoryDate,
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
           AND a.ItemCode = b.ItemCode
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
         SELECT b.InventoryDate,
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
         WHERE a.AdjustmentDate < b.InventoryDate
           AND a.LocationCode = b.LocationCode
           AND a.UnitCode = b.UnitCode
           AND a.ItemStatusFrom = b.ItemStatus
           AND a.ItemCode = b.ItemCode
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
		 -- Mntc Inventory Adjusment
		 -- Item Status To
         SELECT b.InventoryDate,
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
           AND a.ItemCode = b.ItemCode
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
         SELECT b.InventoryDate,
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
         WHERE a.AdjustmentDate < b.InventoryDate
           AND a.LocationCode = b.LocationCode
           AND a.UnitCodeDestination = b.UnitCode
           AND a.ItemStatusTo = b.ItemStatus
           AND a.ItemCode = b.ItemCode
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
		 -- Mntc Repair Item Usage
		 -- Ready to Use
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
           AND a.UnitCode = b.UnitCode
           AND b.ItemStatus = 'READY TO USE'
           AND a.ItemCodeDestination = b.ItemCode
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
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
         WHERE a.TransactionDate < b.InventoryDate
           AND a.LocationCode = b.LocationCode
           AND a.UnitCode = b.UnitCode
           AND b.ItemStatus = 'READY TO USE'
           AND a.ItemCodeDestination = b.ItemCode
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode 
		 UNION ALL
		 -- Mntc Repair Item Usage
		 -- Bad Stock
		 SELECT b.InventoryDate,
                b.LocationCode,
                b.UnitCode,
                b.ItemStatus,
                b.ItemCode,
                0 DeltaStaw,
                SUM(QtyUsage) DeltaStockIn,
                0 AS DeltaStockOut,
                SUM(QtyUsage) AS DeltaStak
         FROM [MntcRepairItemUsage] a,
              [MntcInventory] b
         WHERE a.TransactionDate = b.InventoryDate
           AND a.LocationCode = b.LocationCode
           AND a.UnitCode = b.UnitCode
           AND b.ItemStatus = 'BAD STOCK'
           AND a.ItemCodeDestination = b.ItemCode
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode
         UNION ALL
         SELECT b.InventoryDate,
                b.LocationCode,
                b.UnitCode,
                b.ItemStatus,
                b.ItemCode,
                0 DeltaStaw,
                SUM(QtyUsage) DeltaStockIn,
                0 AS DeltaStockOut,
                SUM(QtyUsage) AS DeltaStak
         FROM [MntcRepairItemUsage] a,
              [MntcInventory] b
         WHERE a.TransactionDate < b.InventoryDate
           AND a.LocationCode = b.LocationCode
           AND a.UnitCode = b.UnitCode
           AND b.ItemStatus = 'BAD STOCK'
           AND a.ItemCodeDestination = b.ItemCode
           AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         GROUP BY b.InventoryDate,
                  b.LocationCode,
                  b.UnitCode,
                  b.ItemStatus,
                  b.ItemCode ) AS x
     GROUP BY InventoryDate,
              LocationCode,
              UnitCode,
              ItemStatus,
              ItemCode;

GO


