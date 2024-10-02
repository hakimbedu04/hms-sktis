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
	
		-- Cursor Loop MstPlantEmpUpd
		DECLARE cursor_EmpUpd CURSOR LOCAL FOR
		SELECT EmployeeID
		FROM MstPlantEmpUpd
	
		OPEN cursor_EmpUpd
	
		FETCH NEXT FROM cursor_EmpUpd   
		INTO @empUpd_ID
	
		WHILE @@FETCH_STATUS = 0  
		BEGIN  
			IF EXISTS(SELECT * FROM MstPlantEmpUpdTmp WHERE EmployeeID = @empUpd_ID)
			BEGIN
				UPDATE MstPlantEmpUpdTmp
				SET RowVersion = DATEDIFF(second,{d '1970-01-01'}, GETDATE())
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
			INTO @empUpd_ID
		END;
	
		CLOSE cursor_EmpUpd;  
		DEALLOCATE cursor_EmpUpd; 
	END
END