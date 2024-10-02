/****** Object:  Table [dbo].[TableDeltaView]    Script Date: 09/15/2016 11:58:37 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TableDeltaView]') AND type in (N'U'))
DROP TABLE [dbo].[TableDeltaView]
GO

/****** Object:  Table [dbo].[TableDeltaView]    Script Date: 09/15/2016 11:58:37 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[TableDeltaView](
	[InventoryDate] [datetime],
	[LocationCode] [varchar](10),
	[UnitCode] [varchar](10),
	[ItemStatus] [varchar](40),
	[ItemCode] [varchar](10),
	[DBeginningStock] [int] NULL,
	[DStockIn] [int] NULL,
	[DStockOut] [int] NULL,
	[DEndingStock] [int] NULL
CONSTRAINT [PK_TableDeltaView] PRIMARY KEY NONCLUSTERED 
(
	[InventoryDate] ASC,
	[LocationCode] ASC,
	[UnitCode] ASC,
	[ItemStatus] ASC,
	[ItemCode] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


