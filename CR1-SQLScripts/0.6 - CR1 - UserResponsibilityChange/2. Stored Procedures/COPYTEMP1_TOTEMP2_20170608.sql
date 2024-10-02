SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[HRDV2_COPYTEMP1_TOTEMP2] 
AS
BEGIN
	INSERT INTO [dbo].[HRDV2_SKTIS_Temp2]
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
	SELECT [ID]
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
		  ,[UpdatedBy]
  FROM [SKTIS].[dbo].[HRDV2_SKTIS_Temp1]
END;