SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

IF EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID('HRDV2_CheckUSER'))
DROP PROCEDURE [dbo].[HRDV2_CheckUSER]

/******************** 
	Check user in not exists in HRD V2 (ADSI_VNOTES) then set expired date = today 
*********************/

CREATE PROCEDURE [dbo].[HRDV2_CheckUSER]
AS
BEGIN

	UPDATE UtilUsersResponsibility
	SET ExpiredDate = DATEADD(dd, 0, DATEDIFF(dd, 0, GETDATE()))
	WHERE UserAD IN  
	(
		SELECT DISTINCT adTemp.UserAD FROM MstADTemp adTemp WHERE NOT EXISTS (SELECT * FROM HRDV2 WHERE FULL_NAME = adTemp.UserAD)
	) 
	
END