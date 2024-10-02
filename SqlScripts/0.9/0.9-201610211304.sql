-- =============================================
-- Ticket: http://tp.voxteneo.co.id/entity/10646
-- Author:	Azka
-- Create date: 21-10-2016
-- Description:	Create Temp Table for Employee ACV
-- =============================================

IF  EXISTS (SELECT * FROM sys.tables WHERE object_id = OBJECT_ID(N'[dbo].[MstPlantEmpJobsDataAcvTemp]'))
DROP TABLE [dbo].[MstPlantEmpJobsDataAcvTemp]
GO
/****** Object:  Table [dbo].[MstPlantEmpJobsDataAcvTemp]    Script Date: 10/21/2016 2:29:32 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[MstPlantEmpJobsDataAcvTemp](
	[EmployeeID] [varchar](64) NOT NULL,
	[EmployeeNumber] [varchar](6) NULL,
	[EmployeeName] [varchar](64) NULL,
	[JoinDate] [datetime] NULL,
	[Title_id] [varchar](12) NULL,
	[ProcessSettingsCode] [varchar](100) NULL,
	[Status] [varchar](64) NULL,
	[CCT] [varchar](64) NULL,
	[CCTDescription] [varchar](64) NULL,
	[HCC] [varchar](64) NULL,
	[LocationCode] [varchar](8) NULL,
	[GroupCode] [varchar](4) NULL,
	[UnitCode] [varchar](4) NULL,
	[Loc_id] [varchar](8) NULL,
	[Remark] [varchar](256) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](64) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](64) NOT NULL,
 CONSTRAINT [PK_MSTPLANTEMPJOBSDATAACVTEMP] PRIMARY KEY CLUSTERED 
(
	[EmployeeID] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO
