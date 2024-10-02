CREATE TABLE [dbo].[VT_SchemaChangeLog](
	[SchemaId] [int] IDENTITY(1,1) NOT NULL,
	[MajorReleaseNumber] [varchar](10) NULL,
	[MinorReleaseNumber] [varchar](10) NULL,
	[ScriptName] [varchar](50) NOT NULL,
	[ScriptDateApplied] [datetime] NULL,
 CONSTRAINT [pk_VT_SchemaChangeLog] PRIMARY KEY CLUSTERED 
(
	[SchemaId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

SET ANSI_PADDING OFF
GO

ALTER TABLE [dbo].[VT_SchemaChangeLog] ADD  DEFAULT (getdate()) FOR [ScriptDateApplied]
GO


