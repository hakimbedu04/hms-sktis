-- =============================================
-- Description: membuat sp antrian sp copydeltaview
-- Ticket: http://tp.voxteneo.co.id/entity/10370
-- Author: Hakim
-- =============================================
IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[dbo].[SchedulerQueueCopyDeltaView]'))
	DROP PROCEDURE [dbo].[SchedulerQueueCopyDeltaView]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[SchedulerQueueCopyDeltaView] 
	-- Add the parameters for the stored procedure here
AS
BEGIN
	DECLARE @TempDate DATETIME
	DECLARE @STATUS BIT
	DECLARE @COUNTER INT

	SET @TempDate = (SELECT TOP 1 CreatedDate FROM QueueCopyDeltaView ORDER BY CreatedDate DESC)
	SET @STATUS = (SELECT TOP 1 [status] FROM QueueStatusCopyDeltaView)
	SET @COUNTER = (SELECT COUNT(ID) FROM QueueCopyDeltaView);

	IF(@COUNTER > 0)
	BEGIN
		IF(@STATUS = 0)
		BEGIN
			DELETE QueueCopyDeltaView WHERE CreatedDate <= @TempDate;
			UPDATE QueueStatusCopyDeltaView SET [status] = 1;
			EXEC CopyDeltaView;
			UPDATE QueueStatusCopyDeltaView SET [status] = 0;
		END
	END
END
