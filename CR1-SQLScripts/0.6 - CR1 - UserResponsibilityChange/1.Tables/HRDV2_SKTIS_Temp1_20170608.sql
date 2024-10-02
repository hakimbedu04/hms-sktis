SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[HRDV2_SKTIS_Temp1](
	[ID] [varchar](10) NULL,
	[NAME] [varchar](50) NULL,
	[FULL_NAME] [varchar](100) NULL,
	[EMAIL] [varchar](100) NULL,
	[TITLE_NAME] [varchar](50) NULL,
	[DEPARTMENT] [varchar](50) NULL,
	[LOCATION] [varchar](50) NULL,
	[SPV_ID_STRUCTURE] [varchar](10) NULL,
	[SPV_NAME] [varchar](50) NULL,
	[SPV_EMAIL] [varchar](100) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](50) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](50) NOT NULL
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


