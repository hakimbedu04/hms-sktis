USE [SKT_DEV]
GO

/****** Object:  View [dbo].[MaintenanceEquipmentFulfillmentDetailView]    Script Date: 05/19/2016 13:16:50 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- Description: Update to MntcAllView
-- Ticket: bug/8417 Point all the views related to Inventory to MtncinventoryViewAll
-- Author: Bagus
ALTER VIEW [dbo].[MaintenanceEquipmentFulfillmentDetailView]
AS
     --SELECT mi.ItemCode,
     --       mi.LocationCode,
     --       mi.EndingStock,
     --       mrtl.QtyFromLocation AS Quantity,
     --       mi.ItemStatus,
     --       mi.InventoryDate
     --FROM dbo.MntcInventory AS mi
     --     LEFT OUTER JOIN dbo.MntcRequestToLocation AS mrtl ON mrtl.LocationCode = mi.LocationCode;

     SELECT mmil.ItemCode,
            mmil.LocationCode,
            mi.EndingStock,
            mrtl.QtyFromLocation AS Quantity,
            mi.ItemStatus,
            mi.InventoryDate,
            mi.ItemDescription
     FROM dbo.MstMntcItemLocation mmil
          LEFT OUTER JOIN dbo.MntcInventoryAll mi ON mi.ItemCode = mmil.ItemCode
                                              AND mi.LocationCode = mmil.LocationCode
          LEFT OUTER JOIN dbo.MntcRequestToLocation AS mrtl ON mrtl.LocationCode = mmil.LocationCode;

GO


