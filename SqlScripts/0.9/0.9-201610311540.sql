/**Wahyu : Repair Parameter sniffing on [MaintenanceExecutionInventoryProcedure]***/

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MaintenanceExecutionInventoryProcedure]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[MaintenanceExecutionInventoryProcedure]
GO


CREATE PROCEDURE [dbo].[MaintenanceExecutionInventoryProcedure]
(	
	@InventoryDate			DATETIME,
	@LocationCode			VARCHAR(50),
	@QParam               VARCHAR(100),
	@UserAD              VARCHAR(50)
)
AS
BEGIN

DECLARE @InvDate DATETIME
    SET @InvDate = @InventoryDate
DECLARE @LocCode varchar(50)
    SET @LocCode = @LocationCode
DECLARE @QPrm varchar(100)
    SET @QPrm = @QParam
DECLARE @UsrAD varchar(50)
    SET @UsrAD = @UserAD
    
--delete from table temporary
DELETE FROM TemporaryTableViewInventory WHERE QParam LIKE + '%'+@UsrAD+'%'

--insert to table temporary
INSERT INTO TemporaryTableViewInventory

------Siapkan tabel mntcInventory di memori
----DECLARE @VirtualMntcInventory TABLE
----(
----      [InventoryDate] [datetime] NOT NULL,
----	  [ItemStatus] [varchar](32) NOT NULL,
----	  [ItemCode] [varchar](20) NOT NULL,
----	  [LocationCode] [varchar](8) NOT NULL,
----	  [BeginningStock] [int] NULL,
----	  [StockIn] [int] NULL,
----	  [StockOut] [int] NULL,
----	  [EndingStock] [int] NULL,
----	  [UnitCode] [varchar](4) NOT NULL
---- )
----insert into @VirtualMntcInventory
----SELECT * FROM MntcInventory WHERE LocationCode = @LocationCode and InventoryDate >= @InventoryDate


------Siapkan delta view di memori
----DECLARE @VirtualDeltaView TABLE
----(
----      [InventoryDate] [datetime] NOT NULL,
----	  [LocationCode] [varchar](8) NOT NULL,
----	  [UnitCode] [varchar](4) NOT NULL,
----	  [ItemStatus] [varchar](32) NOT NULL,
----  	  [ItemCode] [varchar](20) NOT NULL,
----	  [DBeginningStock] [int] NULL,
----	  [DStockIn] [int] NULL,
----	  [DStockOut] [int] NULL,
----	  [DEndingStock] [int] NULL
---- )
----insert into @VirtualDeltaView
----SELECT * FROM MntcInventoryDeltaView WHERE LocationCode = @LocationCode and InventoryDate >= @InventoryDate


SELECT
  InventoryDate,
  ItemCode,
  LocationCode,
  ItemType,
  ItemDescription,
  ItemStatus,
  SUM(BeginningStock) BeginningStock,
  SUM(StockIn) StockIn,
  SUM(StockOut) StockOut,
  SUM(EndingStock) EndingStock,
  QParam = @QPrm
  
  FROM (
	SELECT
		a.InventoryDate,
		a.ItemStatus,
		a.ItemCode,
		a.LocationCode,
		c.ItemType,
		c.ItemDescription,
		CASE WHEN b.DBeginningStock IS NULL THEN a.BeginningStock ELSE a.BeginningStock + b.DBeginningStock END     
        BeginningStock,
		--a.BeginningStock + b.DBeginningStock AS BeginningStock,
		CASE WHEN b.DStockIn IS NULL THEN a.StockIn ELSE a.StockIn + b.DStockIn END     
        StockIn,
		--a.StockIn + b.DStockIn AS StockIn,
		CASE WHEN b.DStockOut IS NULL THEN a.StockOut ELSE a.StockOut + b.DStockOut END     
        StockOut,
		--a.StockOut + b.DStockOut AS StockOut,
		CASE WHEN b.DEndingStock IS NULL THEN a.EndingStock ELSE a.EndingStock + b.DEndingStock END     
        EndingStock
		--a.EndingStock + b.DEndingStock AS EndingStock
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
		WHERE InventoryDate = @InvDate
		AND LocationCode = @LocCode
		--AND ItemStatus = 'BAD STOCK'
	) AS a
	LEFT JOIN (select * from GetDeltaViewFunction(@LocCode,@InvDate)) as b
	
	--dbo.MntcInventoryDeltaView AS b
	ON b.InventoryDate = a.InventoryDate
		AND b.ItemCode = a.ItemCode
		AND b.ItemStatus = a.ItemStatus
		AND b.LocationCode = a.LocationCode
		AND b.UnitCode = a.UnitCode
	INNER JOIN dbo.MstMntcItem AS c
		ON c.ItemCode = a.ItemCode
) as c
GROUP BY InventoryDate,
         ItemCode,
         LocationCode,
         ItemType,
         ItemDescription,
         ItemStatus
  
  
  --SUM(CASE
  --  WHEN ItemStatus = 'IN TRANSIT' THEN BeginningStock
  --  ELSE 0
  --END)
  --AS StawIT,
  --SUM(CASE
  --  WHEN ItemStatus = 'IN TRANSIT' THEN StockIn
  --  ELSE 0
  --END) AS InIT,
  --SUM(CASE
  --  WHEN ItemStatus = 'IN TRANSIT' THEN StockOut
  --  ELSE 0
  --END) AS OutIT,
  --SUM(CASE
  --  WHEN ItemStatus = 'IN TRANSIT' THEN EndingStock
  --  ELSE 0
  --END) AS StackIT,
  --SUM(CASE
  --  WHEN ItemStatus = 'QUALITY INSPECTION' THEN BeginningStock
  --  ELSE 0
  --END) AS StawQI,
  --SUM(CASE
  --  WHEN ItemStatus = 'QUALITY INSPECTION' THEN StockIn
  --  ELSE 0
  --END) AS InQI,
  --SUM(CASE
  --  WHEN ItemStatus = 'QUALITY INSPECTION' THEN StockOut
  --  ELSE 0
  --END) AS OutQI,
  --SUM(CASE
  --  WHEN ItemStatus = 'QUALITY INSPECTION' THEN EndingStock
  --  ELSE 0
  --END) AS StackQI,
  --SUM(CASE
  --  WHEN ItemStatus = 'READY TO USE' THEN BeginningStock
  --  ELSE 0
  --END) AS StawReady,
  --SUM(CASE
  --  WHEN ItemStatus = 'READY TO USE' THEN StockIn
  --  ELSE 0
  --END) AS InReady,
  --SUM(CASE
  --  WHEN ItemStatus = 'READY TO USE' THEN StockOut
  --  ELSE 0
  --END) AS OutReady,
  --SUM(CASE
  --  WHEN ItemStatus = 'READY TO USE' THEN EndingStock
  --  ELSE 0
  --END) AS StackReady,
  --SUM(CASE
  --  WHEN ItemStatus = 'ON USED' THEN BeginningStock
  --  ELSE 0
  --END) AS StawOU,
  --SUM(CASE
  --  WHEN ItemStatus = 'ON USED' THEN StockIn
  --  ELSE 0
  --END) AS InOU,
  --SUM(CASE
  --  WHEN ItemStatus = 'ON USED' THEN StockOut
  --  ELSE 0
  --END)
  --AS OutOU,
  --SUM(CASE
  --  WHEN ItemStatus = 'ON USED' THEN EndingStock
  --  ELSE 0
  --END) AS StackOU,
  --SUM(CASE
  --  WHEN ItemStatus = 'ON REPAIR' THEN BeginningStock
  --  ELSE 0
  --END) AS StawOR,
  --SUM(CASE
  --  WHEN ItemStatus = 'ON REPAIR' THEN StockIn
  --  ELSE 0
  --END) AS InOR,
  --SUM(CASE
  --  WHEN ItemStatus = 'ON REPAIR' THEN StockOut
  --  ELSE 0
  --END)
  --AS OutOR,
  --SUM(CASE
  --  WHEN ItemStatus = 'ON REPAIR' THEN EndingStock
  --  ELSE 0
  --END) AS StackOR,
  --SUM(CASE
  --  WHEN ItemStatus = 'BAD STOCK' THEN BeginningStock
  --  ELSE 0
  --END) AS StawBS,
  --SUM(CASE
  --  WHEN ItemStatus = 'BAD STOCK' THEN StockIn
  --  ELSE 0
  --END) AS InBS,
  --SUM(CASE
  --  WHEN ItemStatus = 'BAD STOCK' THEN StockOut
  --  ELSE 0
  --END)
  --AS OutBS,
  --SUM(CASE
  --  WHEN ItemStatus = 'BAD STOCK' THEN EndingStock
  --  ELSE 0
  --END) AS StackBS
--into TemporaryTableViewInventory
----FROM (SELECT
----  a.InventoryDate,
----  a.ItemStatus,
----  a.ItemCode,
----  a.LocationCode,
----  b.ItemType,
----  b.ItemDescription,
----  a.BeginningStock,
----  a.StockIn,
----  a.StockOut,
----  a.EndingStock
----FROM dbo.MntcInventory AS a
------FROM @VirtualMntcInventory AS a
----INNER JOIN dbo.MstMntcItem AS b
----  ON b.ItemCode = a.ItemCode
----WHERE (NOT EXISTS (SELECT
----  1 AS Expr1
----FROM dbo.MntcInventoryDeltaView
------FROM dbo.TableDeltaView
------FROM @VirtualDeltaView
----WHERE (InventoryDate = a.InventoryDate)
----AND (LocationCode = a.LocationCode)
----AND (UnitCode = a.UnitCode)
----AND (ItemStatus = a.ItemStatus)
----AND (ItemCode = a.ItemCode))
----)
----UNION ALL
----SELECT
----  a.InventoryDate,
----  a.ItemStatus,
----  a.ItemCode,
----  a.LocationCode,
----  c.ItemType,
----  c.ItemDescription,
----  a.BeginningStock + b.DBeginningStock AS BeginningStock,
----  a.StockIn + b.DStockIn AS StockIn,
----  a.StockOut + b.DStockOut AS StockOut,
----  a.EndingStock + b.DEndingStock AS EndingStock
------FROM @VirtualMntcInventory AS a
----FROM dbo.MntcInventory AS a
----INNER JOIN dbo.MntcInventoryDeltaView AS b
------INNER JOIN dbo.TableDeltaView AS b
------INNER JOIN @VirtualDeltaView AS b
----  ON b.InventoryDate = a.InventoryDate
----  AND b.ItemCode = a.ItemCode
----  AND b.ItemStatus = a.ItemStatus
----  AND b.LocationCode = a.LocationCode
----  AND b.UnitCode = a.UnitCode
----INNER JOIN dbo.MstMntcItem AS c
----  ON c.ItemCode = a.ItemCode) AS Inventory
----GROUP BY InventoryDate,
----         ItemCode,
----         LocationCode,
----         ItemType,
----         ItemDescription,
----         ItemStatus



END;
GO


