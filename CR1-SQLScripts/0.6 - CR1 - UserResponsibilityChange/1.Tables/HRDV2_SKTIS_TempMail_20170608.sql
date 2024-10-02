SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

SET ANSI_PADDING ON
GO

CREATE TABLE [dbo].[HRDV2_TEMP_MAIL](
	[NAME] [varchar](50) NULL,
	[EMAIL] [varchar](100) NULL,
	[FULL_NAME] [varchar](200) NULL,
	[SPV_NAME] [varchar](50) NULL,
	[SPV_EMAIL] [varchar](100) NULL,
	[SUBJECT] [varchar](100) NULL,
	[BODY_MAIL] [varchar](max) NULL,
	[CreatedDate] [datetime] NOT NULL,
	[CreatedBy] [varchar](50) NOT NULL,
	[UpdatedDate] [datetime] NOT NULL,
	[UpdatedBy] [varchar](50) NOT NULL,
	[IDResponsibility] [INT] NULL
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO


