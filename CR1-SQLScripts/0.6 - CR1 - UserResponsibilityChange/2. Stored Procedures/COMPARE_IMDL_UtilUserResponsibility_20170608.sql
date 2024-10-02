SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[HRDV2_COMPARE_IMDL_UtilUserResponsibility]	
AS
BEGIN

	DECLARE @userAD varchar(20)
	DECLARE @idRespons int;
	DECLARE @startDate DATE;
	DECLARE @endDate DATE;

	DECLARE cursor_imdl CURSOR LOCAL FAST_FORWARD FOR
	SELECT 
		'PMI\' + t.MSACCT 
		, u.IDResponsibility
		, convert(datetime, t.STRTDAT, 112)
		, convert(datetime, t.ENDDAT, 112)
	FROM IMDLTemp t
	INNER JOIN UtilResponsibility u on u.ResponsibilityName = SUBSTRING(t.BROLE_DESC, CHARINDEX('SKTIS', t.BROLE_DESC) + 6, LEN(t.BROLE_DESC))

	OPEN cursor_imdl

	FETCH NEXT FROM cursor_imdl   
	INTO @userAD, @idRespons, @startDate, @endDate

	WHILE @@FETCH_STATUS = 0  
	BEGIN  
		DECLARE @dateNow DATETIME = GETDATE();

		IF EXISTS (SELECT * FROM UtilUsersResponsibility WHERE UserAD = @userAD AND IDResponsibility = @idRespons)
		BEGIN
			UPDATE UtilUsersResponsibility
			SET EffectiveDate = @startDate, ExpiredDate = @endDate, UpdatedBy = 'SYSTEM', UpdatedDate = @dateNow
			WHERE UserAD = @userAD AND IDResponsibility = @idRespons
		END
		ELSE
		BEGIN

			INSERT INTO [dbo].[UtilUsersResponsibility]
			   ([IDResponsibility]
			   ,[UserAD]
			   ,[EffectiveDate]
			   ,[ExpiredDate]
			   ,[CreatedDate]
			   ,[CreatedBy]
			   ,[UpdatedDate]
			   ,[UpdatedBy])
			VALUES
			   (@idRespons
			   ,@userAD
			   ,@startDate
			   ,@endDate
			   ,@dateNow
			   ,'SYSTEM'
			   ,@dateNow
			   ,'SYSTEM')
		END

		UPDATE ur
			SET 
				ur.ExpiredDate = CONVERT(date, getdate())
				, ur.UpdatedBy = 'SYSTEM'
				, ur.UpdatedDate = @dateNow
		FROM UtilUsersResponsibility ur
		WHERE NOT EXISTS
		(
			SELECT * FROM IMDLTemp t
			INNER JOIN UtilResponsibility u on u.ResponsibilityName = SUBSTRING(t.BROLE_DESC, CHARINDEX('SKTIS', t.BROLE_DESC) + 6, LEN(t.BROLE_DESC))
			WHERE ('PMI\' + t.MSACCT) = ur.UserAD AND u.IDResponsibility = ur.IDResponsibility
		) AND UserAD = @userAD

		FETCH NEXT FROM cursor_imdl   
		INTO @userAD, @idRespons, @startDate, @endDate
	END

	CLOSE cursor_imdl;  
	DEALLOCATE cursor_imdl;  
END