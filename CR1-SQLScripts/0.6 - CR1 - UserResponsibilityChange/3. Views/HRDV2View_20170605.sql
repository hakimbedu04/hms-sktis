IF OBJECT_ID('HRDV2', 'V') IS NOT NULL
    DROP VIEW HRDV2;

/****** Object:  View [dbo].[ADSI_VNOTES]    Script Date: 6/5/2017 2:52:20 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[HRDV2] AS
SELECT 
	v.ID
	, v.NAME
	, a.FULL_NAME
	, a.EMAIL
	, v.TITLE_NAME
	, v.DEPARTMENT
	, v.LOCATION
	, v.SPV_ID_STRUCTURE
	, v.SPV_NAME
	, (SELECT EMAIL FROM [db_Intranet_HRDV2].[dbo].tbl_ADSI_User WHERE ID = 'ID' + v.SPV_ID_FUNCTIONAL) AS SPV_EMAIL
FROM [db_Intranet_HRDV2].[dbo].vNOTES_DATA_FLOW v
INNER JOIN [db_Intranet_HRDV2].[dbo].tbl_ADSI_User a on a.ID = 'ID' + v.ID


GO


