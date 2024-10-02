-- =============================================
-- Author:		Wahyu
-- Create date: 
-- Description:	Procedure for copying MntcInventoryDeltaView to TableDeltaView
-- =============================================

CREATE PROCEDURE [dbo].[CopyDeltaView] 
	-- Add the parameters for the stored procedure here
AS
BEGIN
	SET NOCOUNT ON;
	
	DECLARE @query2 AS varchar(MAX)
	SET @query2 = 'delete from dbo.TableDeltaView'; 
	EXEC (@query2)
	
    DECLARE @query AS varchar(MAX)
	SET @query = 'insert into dbo.TableDeltaView SELECT * FROM MntcInventoryDeltaView'; 
	EXEC (@query)
	
	SET nocount OFF; 
END
GO
