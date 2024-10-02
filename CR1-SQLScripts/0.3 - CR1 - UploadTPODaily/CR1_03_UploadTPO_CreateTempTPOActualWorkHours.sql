-- =============================================
-- Description: CR-1 Upload TPO Daily Create Temp Table Actual WorkHours
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

IF object_id('ExeTPOActualWorkHoursTemp', 'U') is null
BEGIN
	CREATE TABLE [dbo].[ExeTPOActualWorkHoursTemp](
		[LocationCode] [varchar](8) NOT NULL,
		[UnitCode] [varchar](4) NOT NULL,
		[BrandCode] [varchar](11) NOT NULL,
		[ProductionDate] [datetime] NOT NULL,
		[ProcessGroup] [varchar](16) NOT NULL,
		[ProcessOrder] [int] NOT NULL,
		[StatusEmp] [varchar](16) NOT NULL,
		[StatusIdentifier] [char](1) NOT NULL,
		[TimeIn] [time](7) NULL,
		[TimeOut] [time](7) NULL,
		[BreakTime] [time](7) NULL,
		[CreatedDate] [datetime] NOT NULL,
		[CreatedBy] [varchar](64) NOT NULL,
		[UpdatedDate] [datetime] NOT NULL,
		[UpdatedBy] [varchar](64) NOT NULL,
	 CONSTRAINT [PK_ExeTPOActualWorkHoursTemp] PRIMARY KEY CLUSTERED 
	(
		[LocationCode] ASC,
		[UnitCode] ASC,
		[BrandCode] ASC,
		[ProductionDate] ASC,
		[ProcessGroup] ASC,
		[StatusEmp] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY]

	ALTER TABLE [dbo].[ExeTPOActualWorkHoursTemp]  WITH NOCHECK ADD  CONSTRAINT [FK_ExeTPOActualWorkHours_MstGenBrandTemp] FOREIGN KEY([BrandCode])
	REFERENCES [dbo].[MstGenBrand] ([BrandCode])

	ALTER TABLE [dbo].[ExeTPOActualWorkHoursTemp] CHECK CONSTRAINT [FK_ExeTPOActualWorkHours_MstGenBrandTemp]

	ALTER TABLE [dbo].[ExeTPOActualWorkHoursTemp]  WITH NOCHECK ADD  CONSTRAINT [FK_ExeTPOActualWorkHours_MstGenEmpStatusTemp] FOREIGN KEY([StatusEmp])
	REFERENCES [dbo].[MstGenEmpStatus] ([StatusEmp])

	ALTER TABLE [dbo].[ExeTPOActualWorkHoursTemp] CHECK CONSTRAINT [FK_ExeTPOActualWorkHours_MstGenEmpStatusTemp]

	ALTER TABLE [dbo].[ExeTPOActualWorkHoursTemp]  WITH NOCHECK ADD  CONSTRAINT [FK_ExeTPOActualWorkHours_MstGenProcessTemp] FOREIGN KEY([ProcessGroup])
	REFERENCES [dbo].[MstGenProcess] ([ProcessGroup])

	ALTER TABLE [dbo].[ExeTPOActualWorkHoursTemp] CHECK CONSTRAINT [FK_ExeTPOActualWorkHours_MstGenProcessTemp]

	ALTER TABLE [dbo].[ExeTPOActualWorkHoursTemp]  WITH NOCHECK ADD  CONSTRAINT [FK_ExeTPOActualWorkHours_MstPlantUnitTemp] FOREIGN KEY([UnitCode], [LocationCode])
	REFERENCES [dbo].[MstPlantUnit] ([UnitCode], [LocationCode])

	ALTER TABLE [dbo].[ExeTPOActualWorkHoursTemp] CHECK CONSTRAINT [FK_ExeTPOActualWorkHours_MstPlantUnitTemp]

END




