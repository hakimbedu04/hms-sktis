/****** Object:  Table [dbo].[TemporaryTableMntcAll]    Script Date: 11/01/2016 11:13:24 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TemporaryTableMntcAll]') AND type in (N'U'))
DROP TABLE [dbo].[TemporaryTableMntcAll]
GO


CREATE TABLE [dbo].[TemporaryTableMntcAll](
	[InventoryDate] [datetime] NOT NULL,
	[ItemCode] [varchar](20) NOT NULL,
	[LocationCode] [varchar](8) NOT NULL,
	[UnitCode] [varchar](4) NOT NULL,
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

