-- =============================================
-- Author:		Wahyu
-- Create date: 
-- Description:	Procedure for copying data to [TemporaryTableViewInventory]
-- =============================================

IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[MntcInventoryAllProcedure]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[MntcInventoryAllProcedure]
GO

CREATE PROCEDURE [dbo].[MntcInventoryAllProcedure]
(	
	@InventoryDate			DATETIME,
	@LocationCode			VARCHAR(50),
	@QParam                 VARCHAR(100),
	@UserAD                 VARCHAR(50)
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
DELETE FROM TemporaryTableMntcAll WHERE QParam LIKE + '%'+@UserAD+'%'

--insert to table temporary
INSERT INTO TemporaryTableMntcAll

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
  UnitCode,
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
		a.UnitCode,
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
			a.UnitCode,
			a.BeginningStock,
			a.StockIn,
			a.StockOut,
			a.EndingStock
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
         UnitCode,
         ItemDescription,
         ItemStatus
  
  

END;