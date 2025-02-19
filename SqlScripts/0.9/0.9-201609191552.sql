create FUNCTION [dbo].[GetInventoryView]
(	
	@InventoryDate			VARCHAR(50),
	@LocationCode			VARCHAR(50),
	@ItemType				VARCHAR(50)
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
FROM (
	SELECT
		aa.InventoryDate,
		aa.ItemStatus,
		aa.ItemCode,
		aa.LocationCode,
		bb.ItemType,
		bb.ItemDescription,
		aa.BeginningStock,
		aa.StockIn,
		aa.StockOut,
		aa.EndingStock
	FROM (
		SELECT
			a.InventoryDate,
			a.ItemStatus,
			a.ItemCode,
			a.LocationCode,
			a.BeginningStock,
			a.StockIn,
			a.StockOut,
			a.EndingStock,
			a.UnitCode
		FROM dbo.MntcInventory a
		WHERE InventoryDate = @InventoryDate
		AND LocationCode = @LocationCode
		--AND ItemStatus = 'BAD STOCK'
		) AS aa
		INNER JOIN dbo.MstMntcItem bb ON bb.ItemCode = aa.ItemCode AND bb.ItemType = @ItemType
		WHERE (
			NOT EXISTS (SELECT 1 AS Expr1
			FROM dbo.MntcInventoryDeltaView
			WHERE (InventoryDate = aa.InventoryDate)
			AND (LocationCode = aa.LocationCode)
			AND (UnitCode = aa.UnitCode)
			AND (ItemStatus = aa.ItemStatus)
			AND (ItemCode = aa.ItemCode)
		)
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
	FROM (
		SELECT
			a.InventoryDate,
			a.ItemStatus,
			a.ItemCode,
			a.LocationCode,
			a.BeginningStock,
			a.StockIn,
			a.StockOut,
			a.EndingStock,
			a.UnitCode
		FROM dbo.MntcInventory a
		WHERE InventoryDate = @InventoryDate
		AND LocationCode = @LocationCode
		--AND ItemStatus = 'BAD STOCK'
	) AS a
	INNER JOIN dbo.MntcInventoryDeltaView AS b
	ON b.InventoryDate = a.InventoryDate
		AND b.ItemCode = a.ItemCode
		AND b.ItemStatus = a.ItemStatus
		AND b.LocationCode = a.LocationCode
		AND b.UnitCode = a.UnitCode
	INNER JOIN dbo.MstMntcItem AS c
		ON c.ItemCode = a.ItemCode AND c.ItemType = @ItemType
) as c
GROUP BY InventoryDate,
         ItemCode,
         LocationCode,
         ItemType,
         ItemDescription
)