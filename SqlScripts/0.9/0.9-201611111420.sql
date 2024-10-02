/****** Object:  Table [dbo].[TemporaryTableEquipmentStock]    Script Date: 11/11/2016 12:29:47 ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[TemporaryTableEquipmentStock]') AND type in (N'U'))
DROP TABLE [dbo].[TemporaryTableEquipmentStock]
GO


CREATE TABLE [dbo].[TemporaryTableEquipmentStock](
	[RowID] [bigint] NOT NULL,
	[InventoryDate] [datetime] NOT NULL,
	[ItemCode] [varchar](20) NOT NULL,
	[ItemDescription] [varchar](256) NULL,
	[LocationCode] [varchar](8) NOT NULL,
	[UnitCode] [varchar](4) NOT NULL,
	[InTransit] [int] NULL,
	[QI] [int] NULL,
	[ReadyToUse] [int] NULL,
	[BadStock] [int] NULL,
	[TotalStockMntc] [int] NULL,
	[Used] [int] NULL,
	[Repair] [int] NULL,
	[TotalStockProd] [int] NULL,
	[QParam] [varchar](100) NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


