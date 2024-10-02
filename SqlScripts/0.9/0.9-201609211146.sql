-- =============================================
-- Author:		Wahyu
-- Create date: 2016-09-15
-- Description:	trigger to update tabelDeltaView
-- =============================================
/****** Object:  Trigger [trgAfterUpdateMntcEquipmentItemConvert]    Script Date: 09/21/2016 12:39:45 ******/
IF  EXISTS (SELECT * FROM sys.triggers WHERE object_id = OBJECT_ID(N'[dbo].[trgAfterUpdateMntcEquipmentItemConvert]'))
DROP TRIGGER [dbo].[trgAfterUpdateMntcEquipmentItemConvert]
GO

CREATE TRIGGER dbo.trgAfterUpdateMntcEquipmentItemConvert
   ON  dbo.MntcEquipmentItemConvert
   AFTER INSERT,DELETE,UPDATE
AS 
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for trigger here
    DECLARE @RC int
	EXECUTE @RC = [dbo].[CopyDeltaView] 

	SET NOCOUNT OFF;
END
GO