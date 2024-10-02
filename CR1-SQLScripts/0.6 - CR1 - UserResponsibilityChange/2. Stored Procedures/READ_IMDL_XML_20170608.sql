SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[HRDV2_READ_IMDL_XML]
(
	@XMLFileName VARCHAR(MAX)
)	
AS
BEGIN
	SET NOCOUNT ON

	DECLARE @XMLwithOpenXML TABLE
	(
		Id INT IDENTITY PRIMARY KEY,
		XMLData XML,
		LoadedDateTime DATETIME
	)

	DECLARE @sqlStmnt VARCHAR(MAX);

	DECLARE @pathXML VARCHAR(MAX);

	SET @pathXML = 'C:\Users\Azka\Desktop\' + @XMLFileName;

	SET @sqlStmnt = 'SELECT CONVERT(XML, BulkColumn) AS BulkColumn, GETDATE() 
						FROM OPENROWSET(BULK ''' + @pathXML + ''', SINGLE_BLOB) AS x';

	INSERT INTO @XMLwithOpenXML(XMLData, LoadedDateTime)
	EXEC (@sqlStmnt)

	DECLARE @XML AS XML, @hDoc AS INT, @SQL NVARCHAR (MAX)    

	SELECT @XML = XMLData FROM @XMLwithOpenXML -- The row to process    

	EXEC sp_xml_preparedocument @hDoc OUTPUT, @XML, '<root xmlns:a="urn:pmintl.com:pi:imdl"/>'

	DELETE FROM [dbo].[IMDLTemp]

	INSERT INTO [dbo].[IMDLTemp]
           ([BROLE]
           ,[BROLE_DESC]
           ,[STRTDAT]
           ,[ENDDAT]
           ,[AUOBJID]
           ,[AUOBJ]
           ,[ACCT]
           ,[ACCTSTA]
           ,[MSACCT]
           ,[NACHN_EN]
           ,[VORNA_EN]
           ,[SUPERV]
           ,[WKEMAIL])

	SELECT 
		BROLE
		, BROLE_DESC
		, STRTDAT 
		, ENDDAT 
		, AUOBJID 
		, AUOBJ 
		, ACCT 
		, ACCTSTA 
		, MSACCT 
		, NACHN_EN
		, VORNA_EN
		, SUPERV 
		, WKEMAIL 
	FROM OPENXML(@hDoc, '//a:MT_EMSIntegration/row')
	WITH
	(
		BROLE varchar(100) 'BROLE'
		, BROLE_DESC varchar(200) 'BROLE_DESC'
		, STRTDAT varchar(100) 'STRTDAT'
		, ENDDAT varchar(100) 'ENDDAT'
		, AUOBJID varchar(100) 'AUOBJID'
		, AUOBJ varchar(200) 'AUOBJ'
		, ACCT varchar(100) 'ACCT'
		, ACCTSTA varchar(50) 'ACCTSTA'
		, MSACCT varchar(100) 'MSACCT'
		, NACHN_EN varchar(100) 'NACHN_EN'
		, VORNA_EN varchar(100) 'VORNA_EN'
		, SUPERV varchar(100) 'SUPERV'
		, WKEMAIL varchar(200) 'WKEMAIL'
	)

	EXEC sp_xml_removedocument @hDoc
END