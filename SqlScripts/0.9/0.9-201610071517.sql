-- =============================================
-- Description: membuat tabel antrian sp copydeltaview
-- Ticket: http://tp.voxteneo.co.id/entity/10370
-- Author: Hakim
-- =============================================

CREATE TABLE [dbo].[QueueCopyDeltaView](
 [ID] [INT] NULL,
 [CreatedDate] [datetime] NULL,
)

CREATE TABLE [dbo].[QueueStatusCopyDeltaView](
 [status] [bit] NULL,
 [UpdatedDate] [datetime] NULL,
)

INSERT INTO [dbo].[QueueStatusCopyDeltaView] VALUES (0,GETDATE());