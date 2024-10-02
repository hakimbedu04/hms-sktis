-- =============================================
-- Description: CR-1 Upload TPO Daily to Temp Table Verification
-- Author: Azka
-- Ticket: http://tp.voxteneo.co.id/entity/18755
-- Update: 5/30/2017
-- =============================================

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

IF object_id('ExeTPOProductionEntryVerificationTemp', 'U') is null
BEGIN
	CREATE TABLE [dbo].[ExeTPOProductionEntryVerificationTemp](
		[ProductionEntryCode] [varchar](64) NOT NULL,
		[LocationCode] [varchar](8) NOT NULL,
		[ProcessGroup] [varchar](16) NOT NULL,
		[ProcessOrder] [int] NOT NULL,
		[BrandCode] [varchar](11) NOT NULL,
		[KPSYear] [int] NOT NULL,
		[KPSWeek] [int] NOT NULL,
		[ProductionDate] [datetime] NOT NULL,
		[WorkHour] [real] NOT NULL,
		[TotalTPKValue] [real] NULL,
		[TotalActualValue] [real] NULL,
		[VerifySystem] [bit] NULL,
		[VerifyManual] [bit] NULL,
		[Remark] [varchar](1) NULL,
		[CreatedDate] [datetime] NOT NULL,
		[CreatedBy] [varchar](64) NOT NULL,
		[UpdatedDate] [datetime] NOT NULL,
		[UpdatedBy] [varchar](64) NOT NULL,
		[Flag_Manual] [bit] NULL,
	 CONSTRAINT [PK_ExeTPOProductionEntryVerificationTemp] PRIMARY KEY CLUSTERED 
	(
		[ProductionEntryCode] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]


	SET ANSI_PADDING OFF

	ALTER TABLE [dbo].[ExeTPOProductionEntryVerificationTemp] ADD  DEFAULT ((0)) FOR [Flag_Manual]
END




