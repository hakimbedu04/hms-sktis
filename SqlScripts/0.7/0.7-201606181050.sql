-- =============================================
-- Author: Azka
-- Create date: 2016/06/18
-- Ticket: http://tp.voxteneo.co.id/entity/7057
-- Description:	adding Flag_Manual
-- =============================================

ALTER TABLE [dbo].[ExeTPOProductionEntryVerification]
	ADD Flag_Manual BIT NULL DEFAULT(0)