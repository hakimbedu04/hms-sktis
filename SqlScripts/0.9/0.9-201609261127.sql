/****** Object:  Trigger [trgAfterUpdateMntcInventory]    Script Date: 09/26/2016 11:26:59 ******/
IF  EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[trgAfterUpdateMntcInventory]'))
DROP TRIGGER [dbo].[trgAfterUpdateMntcInventory]
GO


