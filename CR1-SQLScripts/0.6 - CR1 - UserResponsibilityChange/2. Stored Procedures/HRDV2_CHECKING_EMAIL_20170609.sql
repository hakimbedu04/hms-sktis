CREATE PROCEDURE [dbo].[HRDV2_CHECKING_EMAIL] 
AS
BEGIN
	
	DECLARE @userAD VARCHAR(20);
	DECLARE @email VARCHAR(200);
	DECLARE @spvMail VARCHAR(200);
	DECLARE @subject VARCHAR(100);
	DECLARE @bodyMail VARCHAR(MAX);

	DECLARE cursorUserAD CURSOR LOCAL FAST_FORWARD FOR
	SELECT DISTINCT FULL_NAME, EMAIL, SPV_EMAIL, SUBJECT, BODY_MAIL FROM HRDV2_TEMP_MAIL

	OPEN cursorUserAD

	FETCH NEXT FROM cursorUserAD   
	INTO @userAD, @email, @spvMail, @subject, @bodyMail

	WHILE @@FETCH_STATUS = 0  
	BEGIN  
		IF EXISTS (
			SELECT FULL_NAME, IDResponsibility FROM HRDV2_TEMP_MAIL WHERE FULL_NAME = @userAD
			EXCEPT
			SELECT UserAD, IDResponsibility FROM UtilUsersResponsibility WHERE UserAD = @userAD AND ExpiredDate > CONVERT(DATE, GETDATE())
		) 
		BEGIN
			DELETE FROM HRDV2_TEMP_MAIL WHERE FULL_NAME = @userAD
		END
		ELSE
		BEGIN
			DECLARE	@ToEmailAddress VARCHAR(250);

			SET @ToEmailAddress = @email +';' + @spvMail

			/**** SENT EMAIL IN HMS ****/
			--EXEC [msdb].[dbo].[sp_send_dbmail] 
			--	@profile_name='SKTIS_Mail',
			--	@body_format='HTML',
			--	@recipients = @ToEmailAddress,
			--	@subject= @subject,
			--	@body = @bodyMail

		END

		FETCH NEXT FROM cursorUserAD   
		INTO @userAD, @email, @spvMail, @subject, @bodyMail
	END

	CLOSE cursorUserAD;  
	DEALLOCATE cursorUserAD;  

END