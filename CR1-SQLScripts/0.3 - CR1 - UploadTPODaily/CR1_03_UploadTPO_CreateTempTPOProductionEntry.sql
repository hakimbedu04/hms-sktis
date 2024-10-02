-- =============================================
-- Description: CR-1 Upload TPO Daily to Temp Table TPO Production
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

IF object_id('ExeTPOProductionTemp', 'U') is null
BEGIN
	CREATE TABLE [dbo].[ExeTPOProductionTemp](
		[ProductionEntryCode] [varchar](64) NOT NULL,
		[StatusEmp] [varchar](64) NOT NULL,
		[StatusIdentifier] [int] NOT NULL,
		[ProductionGroup] [varchar](20) NOT NULL,
		[WorkerCount] [int] NULL,
		[Absent] [int] NULL,
		[ActualProduction] [real] NULL,
		[CreatedDate] [datetime] NOT NULL,
		[CreatedBy] [varchar](64) NOT NULL,
		[UpdatedDate] [datetime] NOT NULL,
		[UpdatedBy] [varchar](64) NOT NULL,
	 CONSTRAINT [PK_ExeTPOProductionTemp] PRIMARY KEY CLUSTERED 
	(
		[ProductionEntryCode] ASC,
		[StatusEmp] ASC,
		[ProductionGroup] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	SET ANSI_PADDING OFF

	ALTER TABLE [dbo].[ExeTPOProductionTemp]  WITH NOCHECK ADD  CONSTRAINT [FK_ExeTPOProduction_ExeTPOProductionEntryVerificationTemp] FOREIGN KEY([ProductionEntryCode])
	REFERENCES [dbo].[ExeTPOProductionEntryVerificationTemp] ([ProductionEntryCode])

	ALTER TABLE [dbo].[ExeTPOProductionTemp] NOCHECK CONSTRAINT [FK_ExeTPOProduction_ExeTPOProductionEntryVerificationTemp]
END




