CREATE TABLE [dbo].[MntcInventoryAdjustment](
	[AdjustmentDate] [datetime] NOT NULL,
	[LocationCode] [varchar](8) NOT NULL,
	[UnitCode] [varchar](4) NOT NULL,
	[UnitCodeDestination] [varchar](4) NOT NULL,
	[ItemCode] [varchar](20) NOT NULL,
	[ItemStatusFrom] [varchar](32) NOT NULL,
	[ItemStatusTo] [varchar](32) NOT NULL,
	[AdjustmentValue] [int] NOT NULL,
	[AdjustmentType] [varchar](32) NULL,
	[Remark] [varchar](256) NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [varchar](64) NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedBy] [varchar](64) NULL,
 CONSTRAINT [PK_MntcInventoryAdjustment_1] PRIMARY KEY CLUSTERED 
(
	[AdjustmentDate] ASC,
	[LocationCode] ASC,
	[UnitCode] ASC,
	[UnitCodeDestination] ASC,
	[ItemCode] ASC,
	[ItemStatusFrom] ASC,
	[ItemStatusTo] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[MntcInventoryAdjustment]  WITH CHECK ADD  CONSTRAINT [FK_MntcInventoryAdjustment_REFERENCE_142_MSTPLANTUNIT] FOREIGN KEY([UnitCode], [LocationCode])
REFERENCES [dbo].[MstPlantUnit] ([UnitCode], [LocationCode])
GO

ALTER TABLE [dbo].[MntcInventoryAdjustment] CHECK CONSTRAINT [FK_MntcInventoryAdjustment_REFERENCE_142_MSTPLANTUNIT]
GO

ALTER TABLE [dbo].[MntcInventoryAdjustment]  WITH CHECK ADD  CONSTRAINT [FK_MntcInventoryAdjustment_REFERENCE_143_MSTMNTCITEM] FOREIGN KEY([ItemCode])
REFERENCES [dbo].[MstMntcItem] ([ItemCode])
GO

ALTER TABLE [dbo].[MntcInventoryAdjustment] CHECK CONSTRAINT [FK_MntcInventoryAdjustment_REFERENCE_143_MSTMNTCITEM]
GO


