/****** Object:  Table [dbo].[ExePlantProductionEntry]    Script Date: 05/10/2015 11:12:17 ******/
DROP TABLE [dbo].[ExeProductionEntryRelease]
GO
DROP TABLE [dbo].[ExePlantProductionEntryVerification]
GO
DROP TABLE [dbo].[ExePlantProductionEntry]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ExePlantProductionEntry](
	[ProductionEntryDate] [datetime] NOT NULL,
	[GroupCode] [varchar](4) NOT NULL,
	[UnitCode] [varchar](4) NOT NULL,
	[LocationCode] [varchar](8) NOT NULL,
	[ProcessGroup] [varchar](16) NOT NULL,
	[BrandCode] [varchar](11) NOT NULL,
	[StartDateAbsent] [datetime] NULL,
	[AbsentType] [varchar](128) NULL,
	[EmployeeID] [varchar](64) NOT NULL,
	[EmployeeNumber] [varchar](11) NULL,
	[ProductionEntryCode] [varchar](64) NULL,
	[ProdCapacity] [int] NULL,
	[ProdTarget] [int] NULL,
	[ProdActual] [int] NULL,
	[AbsentRemark] [varchar](8) NULL,
	[AbsentCodeEblek] [varchar](8) NULL,
	[AbsentCodePayroll] [varchar](8) NULL,
	[ProductionDate] [datetime] NULL,
	[CurrentApproval] [varchar](32) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_EXEPLANTPRODUCTIONENTRY] PRIMARY KEY CLUSTERED 
(
	[ProductionEntryDate] ASC,
	[GroupCode] ASC,
	[UnitCode] ASC,
	[LocationCode] ASC,
	[ProcessGroup] ASC,
	[BrandCode] ASC,
	[EmployeeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ExePlantProductionEntryVerification]    Script Date: 05/10/2015 11:12:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ExePlantProductionEntryVerification](
	[ProductionEntryDate] [datetime] NOT NULL,
	[GroupCode] [varchar](4) NOT NULL,
	[UnitCode] [varchar](4) NOT NULL,
	[LocationCode] [varchar](8) NOT NULL,
	[ProcessGroup] [varchar](16) NOT NULL,
	[BrandCode] [varchar](11) NOT NULL,
	[KPSYear] [int] NULL,
	[KPSWeek] [int] NULL,
	[TPKValue] [int] NULL,
	[TotalTargetValue] [int] NULL,
	[TotalActualValue] [int] NULL,
	[TotalCapacityValue] [int] NULL,
	[VerifySystem] [bit] NULL,
	[VerifyManual] [bit] NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
	[Remark] [varchar](256) NULL,
 CONSTRAINT [PK_EXEPLANTPRODUCTIONENTRYVERIFICATION] PRIMARY KEY NONCLUSTERED 
(
	[ProductionEntryDate] ASC,
	[GroupCode] ASC,
	[UnitCode] ASC,
	[LocationCode] ASC,
	[ProcessGroup] ASC,
	[BrandCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[ExeProductionEntryRelease]    Script Date: 05/10/2015 11:12:17 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[ExeProductionEntryRelease](
	[ProductionEntryDate] [datetime] NOT NULL,
	[GroupCode] [varchar](4) NOT NULL,
	[UnitCode] [varchar](4) NOT NULL,
	[LocationCode] [varchar](8) NOT NULL,
	[ProcessGroup] [varchar](16) NOT NULL,
	[BrandCode] [varchar](11) NOT NULL,
	[Remark] [varchar](256) NULL,
	[EblekDate] [datetime] NULL,
	[IsLocked] [bit] NULL,
	[CreatedDate] [datetime] NULL,
	[CreatedBy] [varchar](64) NULL,
	[UpdatedDate] [datetime] NULL,
	[UpdatedBy] [varchar](64) NULL,
 CONSTRAINT [PK_EXEPRODUCTIONENTRYRELEASE] PRIMARY KEY CLUSTERED 
(
	[ProductionEntryDate] ASC,
	[GroupCode] ASC,
	[UnitCode] ASC,
	[LocationCode] ASC,
	[ProcessGroup] ASC,
	[BrandCode] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
ALTER TABLE [dbo].[ExePlantProductionEntry]  WITH CHECK ADD  CONSTRAINT [FK_EXEPLANTPRODUCTIONENTRY_REFERENCE_121_EXEPLANTPRODUCTIONENTRYV] FOREIGN KEY([ProductionEntryDate], [GroupCode], [UnitCode], [LocationCode], [ProcessGroup], [BrandCode])
REFERENCES [dbo].[ExePlantProductionEntryVerification] ([ProductionEntryDate], [GroupCode], [UnitCode], [LocationCode], [ProcessGroup], [BrandCode])
GO
ALTER TABLE [dbo].[ExePlantProductionEntry] CHECK CONSTRAINT [FK_EXEPLANTPRODUCTIONENTRY_REFERENCE_121_EXEPLANTPRODUCTIONENTRYV]
GO
ALTER TABLE [dbo].[ExePlantProductionEntry]  WITH CHECK ADD  CONSTRAINT [FK_EXEPLANTPRODUCTIONENTRY_RELATIONSHIP_117_EXEPLANTWORKERABSENTEEIS] FOREIGN KEY([StartDateAbsent], [EmployeeID], [AbsentType])
REFERENCES [dbo].[ExePlantWorkerAbsenteeism] ([StartDateAbsent], [EmployeeID], [AbsentType])
GO
ALTER TABLE [dbo].[ExePlantProductionEntry] CHECK CONSTRAINT [FK_EXEPLANTPRODUCTIONENTRY_RELATIONSHIP_117_EXEPLANTWORKERABSENTEEIS]
GO
