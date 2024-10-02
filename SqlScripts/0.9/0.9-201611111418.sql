--Wahyu , based on Mr Luki's emailed  SQL
--Repair Temporary Table View Inventory
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TemporaryTableViewInventory]') AND type in (N'U'))
DROP TABLE [dbo].[TemporaryTableViewInventory]
GO


CREATE TABLE [dbo].[TemporaryTableViewInventory](
	[InventoryDate] [datetime] NOT NULL,
	[ItemCode] [varchar](20) NOT NULL,
	[LocationCode] [varchar](8) NOT NULL, --Luki
	[UnitCode] [varchar](8) NOT NULL,
	[ItemType] [varchar](16) NULL,
	[ItemDescription] [varchar](256) NULL,
	[ItemStatus] [varchar](32) NOT NULL,
	[BeginningStock] [int] NULL,
	[StockIn] [int] NULL,
	[StockOut] [int] NULL,
	[EndingStock] [int] NULL,
	[QParam] [varchar](100) NULL
) ON [PRIMARY]

GO

--Repair SP MaintenanceExecutionInventoryProcedure
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

SELECT
  InventoryDate,
  ItemCode,
  LocationCode,
  UnitCode, --Luki
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
		a.UnitCode, --Luki
		c.ItemType,
		c.ItemDescription,
		--Luki start
		a.BeginningStock + isnull(b.DBeginningStock,0) BeginningStock,
		a.StockIn + isnull(b.DStockIn,0) StockIn,
		a.StockOut + isnull(b.DStockOut,0) StockOut,
		a.EndingStock + isnull(b.DEndingStock,0) EndingStock
		--Luki end
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
         UnitCode,
         ItemType,
         ItemDescription,
         ItemStatus

END;

GO

------------------------------------------------



