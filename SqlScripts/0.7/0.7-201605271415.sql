-- Description: rollback view to beginning
-- Author: azka
-- Description: Equipment to sparepart fix change SUM to MAX
-- Author: abud
-- Author: Robby
-- Description: Inventory Adjustment fix duplicate stockOut
-- Author: Indra
-- Description: Mntc Equipment Repair fix calculation http://tp.voxteneo.co.id/entity/6666

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
         SELECT b.InventoryDate,
                b.LocationCode,
                b.UnitCode,
                b.ItemStatus,
                b.ItemCode,
                0 DeltaStaw,
                0 DeltaStockIn,
                SUM(QtyReceiving) AS DeltaStockOut,
                -SUM(QtyReceiving) AS DeltaStak
         FROM [MntcEquipmentQualityInspection] a,
              [MntcInventory] b
         WHERE a.TransactionDate = b.InventoryDate
           AND a.LocationCode = b.LocationCode
           AND b.UnitCode = 'MTNC'
           AND b.ItemStatus = 'IN TRANSIT'
           AND a.ItemCode = b.ItemCode
           AND b.BeginningStock > 0
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
                0 DeltaStockIn,
                0 DeltaStockOut,
                -SUM(QtyReceiving) AS DeltaStak
         FROM [MntcEquipmentQualityInspection] a,
              [MntcInventory] b
         WHERE a.TransactionDate < b.InventoryDate
           AND a.LocationCode = b.LocationCode
           AND b.UnitCode = 'MTNC'
           AND b.ItemStatus = 'IN TRANSIT'
           AND a.ItemCode = b.ItemCode
           AND b.BeginningStock > 0
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
           AND a.UnitCode = b.UnitCode
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
           AND a.UnitCode = b.UnitCode
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
                SUM(QtyTakenByUnit) AS DeltaStockIn,
                0 AS DeltaStockOut,
                SUM(QtyTakenByUnit) AS DeltaStak
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
                0 AS DeltaStockOut,
                SUM(QtyTakenByUnit) AS DeltaStak
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
         --SELECT b.InventoryDate,
         --       b.LocationCode,
         --       b.UnitCode,
         --       b.ItemStatus,
         --       b.ItemCode,
         --       0 DeltaStaw,
         --       0 DeltaStockIn,
         --       SUM(AdjustmentValue) AS DeltaStockOut,
         --       -SUM(AdjustmentValue) AS DeltaStak
         --FROM [MntcInventoryAdjustment] a,
         --     [MntcInventory] b
         --WHERE a.AdjustmentDate = b.InventoryDate
         --  AND a.LocationCode = b.LocationCode
         --  AND a.UnitCode = b.UnitCode
         --  AND a.ItemStatusFrom = b.ItemStatus
         --  AND a.ItemCode = b.ItemCode
         --  AND CONVERT(VARCHAR(10),a.UpdatedDate,110)=CONVERT(VARCHAR(10),GETDATE(),110)
         --GROUP BY b.InventoryDate,
         --         b.LocationCode,
         --         b.UnitCode,
         --         b.ItemStatus,
         --         b.ItemCode
         --UNION ALL
         SELECT b.InventoryDate,
                b.LocationCode,
                b.UnitCode,
                b.ItemStatus,
                b.ItemCode,
                SUM(AdjustmentValue) AS DeltaStaw,
                0 DeltaStockIn,
                0 DeltaStockOut,
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
         SELECT b.InventoryDate,
                b.LocationCode,
                b.UnitCode,
                b.ItemStatus,
                b.ItemCode,
                0 DeltaStaw,
                0 AS DeltaStockIn,
                -SUM(AdjustmentValue) DeltaStockOut,
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
                SUM(AdjustmentValue) AS DeltaStaw,
                0 DeltaStockIn,
                0 DeltaStockOut,
                SUM(AdjustmentValue) AS DeltaStak
         FROM [MntcInventoryAdjustment] a,
              [MntcInventory] b
         WHERE a.AdjustmentDate < b.InventoryDate
           AND a.LocationCode = b.LocationCode
           AND a.UnitCode = b.UnitCode
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
                -SUM(QtyUsage) AS DeltaStaw,
                0 DeltaStockIn,
                0 DeltaStockOut,
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
                  b.ItemCode ) AS x
     GROUP BY InventoryDate,
              LocationCode,
              UnitCode,
              ItemStatus,
              ItemCode;

GO


