
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MaintenanceExecutionInventoryFunction]') AND type in (N'FN', N'IF', N'TF', N'FS', N'FT'))
DROP FUNCTION [dbo].[MaintenanceExecutionInventoryFunction]
GO

-- =============================================
-- [MaintenanceExecutionInventoryFunction]
-- Description: function for accessing data on TemporaryTableViewInventory
-- Author: WAHYU
-- Updated: 1.0 - 10/14/2016
-- =============================================


CREATE FUNCTION [dbo].[MaintenanceExecutionInventoryFunction]
(
	@InventoryDate  DATETIME,
	@LocationCode   VARCHAR(16),
	@ItemType       VARCHAR(16),
	@UserAD         VARCHAR(50)
)
RETURNS TABLE 
AS
RETURN
(
SELECT 
	InventoryDate,
    ItemCode,
    LocationCode,
    ItemType,
    ItemDescription,
	SUM(BeginningStock) BeginningStock,
	SUM(StockIn) StockIn,
	SUM(StockOut) StockOut,
	SUM(EndingStock) EndingStock,
	--pivotting
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
FROM	TemporaryTableViewInventory WITH (NOLOCK)
         
WHERE InventoryDate = @InventoryDate 
AND LocationCode = @LocationCode
AND ItemType = @ItemType 
AND QParam LIKE + '%'+@UserAD+'%'

GROUP BY InventoryDate,
		 ItemCode,
         LocationCode,
         ItemType,
         ItemDescription
) 


GO


