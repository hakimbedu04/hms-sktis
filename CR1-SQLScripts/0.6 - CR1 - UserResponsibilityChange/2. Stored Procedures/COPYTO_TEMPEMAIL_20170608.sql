SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[HRDV2_COPYTO_TEMPEMAIL] 
AS
BEGIN
	DECLARE @name varchar(50)
	DECLARE @email varchar(100)
	DECLARE @spvName varchar(50)
	DECLARE @spvEmail varchar(100)
	DECLARE @fullName varchar(100)
	DECLARE @dateNow datetime = GETDATE();

	DECLARE cursor_changeUser CURSOR LOCAL FOR
	SELECT v1.NAME
		, v1.EMAIL
		, v1.SPV_NAME
		, v1.SPV_EMAIL
		, v1.FULL_NAME
	FROM HRDV2_SKTIS_1 v1 
	INNER JOIN HRDV2_SKTIS_2 v2 on v1.ID = v2.ID AND v1.TITLE_NAME <> v2.TITLE_NAME

	OPEN cursor_changeUser

	FETCH NEXT FROM cursor_changeUser   
	INTO @name, @email, @spvName, @spvEmail, @fullName

	WHILE @@FETCH_STATUS = 0  
	BEGIN
		DECLARE @recipientEmails VARCHAR(250) = @email + ';' + @spvEmail;
		DECLARE @bodyMail VARCHAR(500);

		INSERT INTO [dbo].[HRDV2_TEMP_MAIL]
           ([NAME]
           ,[EMAIL]
           ,[FULL_NAME]
           ,[SPV_NAME]
           ,[SPV_EMAIL]
           ,[SUBJECT]
           ,[BODY_MAIL]
           ,[CreatedDate]
           ,[CreatedBy]
           ,[UpdatedDate]
           ,[UpdatedBy]
		   ,[IDResponsibility])
		SELECT 
				 @name
			   , @email
			   , @fullName
			   , @spvName
			   , @spvEmail
			   , '[SKTIS-Responsibility] User have different title name'
			   , @bodyMail
			   , @dateNow
			   , 'SYSTEM'
			   , @dateNow
			   , 'SYSTEM'
			   , IDResponsibility
		FROM UtilUsersResponsibility WHERE UserAD = @fullName AND ExpiredDate > Convert(DATE, GETDATE())

		FETCH NEXT FROM cursor_changeUser   
		INTO @name, @email, @spvName, @spvEmail, @fullName
	END

	CLOSE cursor_changeUser;  
	DEALLOCATE cursor_changeUser;  
END;

