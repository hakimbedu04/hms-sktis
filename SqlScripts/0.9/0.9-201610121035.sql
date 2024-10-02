-- =============================================
-- Description: update primary key table QueueCopyDeltaView
-- Ticket: http://tp.voxteneo.co.id/entity/10370
-- Author: Hakim
-- =============================================
/****** Object:  Table [dbo].[QueueCopyDeltaView]    Script Date: 12-Oct-16 10:34:00 AM ******/
DROP TABLE [dbo].[QueueCopyDeltaView]
GO

/****** Object:  Table [dbo].[QueueCopyDeltaView]    Script Date: 12-Oct-16 10:34:00 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[QueueCopyDeltaView](
	[ID] [int] NULL,
	[CreatedDate] [datetime] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[CreatedDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


