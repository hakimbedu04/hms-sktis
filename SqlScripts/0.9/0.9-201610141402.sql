/****** Object:  Table [dbo].[TemporaryTableViewInventory]    Script Date: 10/14/2016 12:50:27 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TemporaryTableViewInventory]') AND type in (N'U'))
DROP TABLE [dbo].[TemporaryTableViewInventory]
GO

CREATE TABLE [dbo].[TemporaryTableViewInventory](
	[InventoryDate] [datetime] NOT NULL,
	[ItemCode] [varchar](20) NOT NULL,
	[LocationCode] [varchar](8) NOT NULL,
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


