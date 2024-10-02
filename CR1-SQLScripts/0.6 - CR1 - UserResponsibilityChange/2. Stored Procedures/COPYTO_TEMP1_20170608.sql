SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[HRDV2_COPYTO_TEMP1] 
AS
BEGIN

DECLARE @dateNow DATETIME = GETDATE();

INSERT INTO [dbo].[HRDV2_SKTIS_Temp1]
           ([ID]
           ,[NAME]
           ,[FULL_NAME]
           ,[EMAIL]
           ,[TITLE_NAME]
           ,[DEPARTMENT]
           ,[LOCATION]
           ,[SPV_ID_STRUCTURE]
           ,[SPV_NAME]
           ,[SPV_EMAIL]
           ,[CreatedDate]
           ,[CreatedBy]
           ,[UpdatedDate]
           ,[UpdatedBy])
	SELECT 
		v.ID
		, v.NAME
		, v.FULL_NAME
		, v.EMAIL
		, v.TITLE_NAME
		, v.DEPARTMENT
		, v.LOCATION
		, v.SPV_ID_STRUCTURE
		, v.SPV_NAME
		, v.SPV_EMAIL
		, @dateNow
		, 'SYSTEM'
		, @dateNow
		, 'SYSTEM'
	FROM HRDV2 v
	INNER JOIN UtilUsersResponsibility u on u.UserAD = v.FULL_NAME
END;