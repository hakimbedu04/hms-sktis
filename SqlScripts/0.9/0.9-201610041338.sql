IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[dbo].[EMPJOBDATA_SCHEDULER_COPYTOUPDTMP]'))
	DROP PROCEDURE [dbo].[EMPJOBDATA_SCHEDULER_COPYTOUPDTMP]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[EMPJOBDATA_SCHEDULER_COPYTOUPDTMP]
AS
BEGIN

	IF EXISTS(select * from MstPlantEmpUpd)
	BEGIN
		DECLARE @empUpd_ID VARCHAR(64);
		DECLARE @EmployeeNumber VARCHAR(6);
		DECLARE @Name VARCHAR(64);
		DECLARE @JoinDate DATETIME;
		DECLARE @TitleID VARCHAR(12);
		DECLARE @ProcessSettingsCode VARCHAR(16);
		DECLARE @Status VARCHAR(64);
		DECLARE @CCT VARCHAR(64);
		DECLARE @CCTDescription VARCHAR(64);
		DECLARE @HCC VARCHAR(64);
		DECLARE @LocationCode VARCHAR(8);
		DECLARE @GroupCode VARCHAR(4);
		DECLARE @UnitCode VARCHAR(4);
		DECLARE @CompletedDate DATETIME;
		DECLARE @AcvSts BIT;
		DECLARE @EffDate DATETIME;
		DECLARE @Action VARCHAR(64);
		DECLARE @FlagCompleted INT;
		DECLARE @Loc_id VARCHAR(8);
	
		-- Cursor Loop MstPlantEmpUpd
		DECLARE cursor_EmpUpd CURSOR LOCAL FOR
		SELECT [EmployeeID], [EmployeeNumber], [Name], [JoinDate], [TitleID], [ProcessSettingsCode],[Status],[CCT],[CCTDescription],[HCC]
		,[LocationCode],[GroupCode],[UnitCode],[CompletedDate],[AcvSts],[EffDate],[Action],[FlagCompleted],[Loc_id]
		FROM MstPlantEmpUpd
	
		OPEN cursor_EmpUpd
	
		FETCH NEXT FROM cursor_EmpUpd   
		INTO @empUpd_ID, @EmployeeNumber, @Name, @JoinDate, @TitleID, @ProcessSettingsCode, @Status, @CCT, @CCTDescription, @HCC,
		@LocationCode, @GroupCode, @UnitCode, @CompletedDate, @AcvSts, @EffDate, @Action, @FlagCompleted, @Loc_id
	
		WHILE @@FETCH_STATUS = 0  
		BEGIN  
			IF EXISTS(SELECT * FROM MstPlantEmpUpdTmp WHERE EmployeeID = @empUpd_ID)
			BEGIN
				UPDATE MstPlantEmpUpdTmp
				SET 
					EmployeeNumber = @EmployeeNumber,
					Name = @Name,
					JoinDate = @JoinDate,
					TitleID = @ProcessSettingsCode,
					Status = @Status,
					CCT = @CCT,
					CCTDescription = @CCTDescription,
					HCC = @HCC,
					LocationCode = @LocationCode,
					GroupCode = @GroupCode,
					UnitCode = @UnitCode,
					CompletedDate = @CompletedDate,
					AcvSts = @AcvSts,
					EffDate = @EffDate,
					Action = @Action,
					FlagCompleted = @FlagCompleted,
					Loc_id = @Loc_id,
					RowVersion = DATEDIFF(second,{d '1970-01-01'}, GETDATE()),
					UpdatedDate = GETDATE(),
					UpdatedBy = 'System'
				WHERE EmployeeID = @empUpd_ID	
			END
			ELSE
			BEGIN
				INSERT INTO [dbo].[MstPlantEmpUpdTmp]
						   ([EmployeeID]
						   ,[EmployeeNumber]
						   ,[Name]
						   ,[JoinDate]
						   ,[TitleID]
						   ,[ProcessSettingsCode]
						   ,[Status]
						   ,[CCT]
						   ,[CCTDescription]
						   ,[HCC]
						   ,[LocationCode]
						   ,[GroupCode]
						   ,[UnitCode]
						   ,[CompletedDate]
						   ,[AcvSts]
						   ,[EffDate]
						   ,[Action]
						   ,[RowVersion]
						   ,[FlagCompleted]
						   ,[Loc_id]
						   ,[CreatedBy]
						   ,[CreatedDate]
						   ,[UpdatedDate]
						   ,[UpdatedBy])
				SELECT 
							[EmployeeID]
						   ,[EmployeeNumber]
						   ,[Name]
						   ,[JoinDate]
						   ,[TitleID]
						   ,[ProcessSettingsCode]
						   ,[Status]
						   ,[CCT]
						   ,[CCTDescription]
						   ,[HCC]
						   ,[LocationCode]
						   ,[GroupCode]
						   ,[UnitCode]
						   ,[CompletedDate]
						   ,[AcvSts]
						   ,[EffDate]
						   ,[Action]
						   ,DATEDIFF(second,{d '1970-01-01'}, GETDATE()) as [RowVersion]
						   ,[FlagCompleted]
						   ,[Loc_id]
						   ,'System' as [CreatedBy]
						   ,GETDATE() as [CreatedDate]
						   ,GETDATE() as [UpdatedDate]
						   ,'System' as [UpdatedBy]
				FROM MstPlantEmpUpd WHERE EmployeeID = @empUpd_ID
			END
			---------------	
			FETCH NEXT FROM cursor_EmpUpd   
			INTO @empUpd_ID, @EmployeeNumber, @Name, @JoinDate, @TitleID, @ProcessSettingsCode, @Status, @CCT, @CCTDescription, @HCC,
			@LocationCode, @GroupCode, @UnitCode, @CompletedDate, @AcvSts, @EffDate, @Action, @FlagCompleted, @Loc_id
		END;
	
		CLOSE cursor_EmpUpd;  
		DEALLOCATE cursor_EmpUpd; 
	END
END