/****** Object:  StoredProcedure [dbo].[EMPJOBDATA_SCHEDULER_COPYTOUPDTMP_V2]    Script Date: 10/21/2016 2:47:27 PM ******/
IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[dbo].[EMPJOBDATA_SCHEDULER_COPYTOUPDTMP_V2]'))
	DROP PROCEDURE [dbo].[EMPJOBDATA_SCHEDULER_COPYTOUPDTMP_V2]
GO

/****** Object:  StoredProcedure [dbo].[EMPJOBDATA_SCHEDULER_COPYTOUPDTMP_V2]    Script Date: 10/21/2016 2:47:27 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[EMPJOBDATA_SCHEDULER_COPYTOUPDTMP_V2]
AS
BEGIN
DECLARE @CurrentHour INT
SET @CurrentHour = DATEPART(HOUR, GETDATE())
IF @CurrentHour >= 0 AND @CurrentHour <=3
BEGIN
	IF EXISTS
	(
			select EmployeeID, EmployeeNumber, EmployeeName, LocationCode, UnitCode, GroupCode, ProcessSettingsCode, Status 
			from MstPlantEmpJobsDataAcvTemp t where not exists(select * from MstPlantEmpJobsDataAcv where EmployeeID = t.employeeid)
			union all
			select a.EmployeeID, a.EmployeeNumber,  a.EmployeeName,  a.LocationCode,  a.UnitCode,  a.GroupCode,  a.ProcessSettingsCode,  a.Status 
			from MstPlantEmpJobsDataAcvtemp t inner join MstPlantEmpJobsDataAcv a on a.EmployeeID = t.employeeid
			where a.EmployeeNumber <> t.employeenumber
	)
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
		select EmployeeID, EmployeeNumber, EmployeeName, LocationCode, UnitCode, GroupCode, ProcessSettingsCode, Status 
		from MstPlantEmpJobsDataAcvTemp t where not exists(select * from MstPlantEmpJobsDataAcv where EmployeeID = t.employeeid)
		union all
		select a.EmployeeID, a.EmployeeNumber,  a.EmployeeName,  a.LocationCode,  a.UnitCode,  a.GroupCode,  a.ProcessSettingsCode,  a.Status 
		from MstPlantEmpJobsDataAcvtemp t inner join MstPlantEmpJobsDataAcv a on a.EmployeeID = t.employeeid
		where a.EmployeeNumber <> t.employeenumber
	
		OPEN cursor_EmpUpd
	
		FETCH NEXT FROM cursor_EmpUpd   
		INTO @empUpd_ID, @EmployeeNumber, @Name, @LocationCode, @UnitCode, @GroupCode, @ProcessSettingsCode, @Status
	
		WHILE @@FETCH_STATUS = 0  
		BEGIN  
			-- Get Effective Date
			SELECT @EffDate = MAX(CASE WHEN LAST_UPDATE_DATE >= LAST_UPD_NO_PEN THEN EFFDT
			WHEN LAST_UPD_NO_PEN > LAST_UPDATE_DATE THEN EFFDT_NO_PEN END) FROM MSTEMPPSFT WHERE EMPLOYEE_ID = @empUpd_ID

			-- Get Active Status
			SELECT TOP 1 
				@AcvSts = CASE WHEN ACS_STS = 'A' THEN 1 ELSE 0 END, 
				@JoinDate = JOIN_DT,
				@TitleID = TITLE_ID,
				@CCT = COST_CENTRE,
				@CCTDescription = COST_CENTRE_DESCR,
				@HCC = HEAD_OF_COST_CENTRE,
				@CompletedDate = CASE WHEN LAST_UPDATE_DATE >= LAST_UPD_NO_PEN THEN LAST_UPDATE_DATE
								 WHEN LAST_UPD_NO_PEN > LAST_UPDATE_DATE THEN LAST_UPD_NO_PEN END,
				@Action = ACTION,
				@Loc_id = LOCATION_ID 
			FROM MSTEMPPSFT WHERE EMPLOYEE_ID = @empUpd_ID AND CASE WHEN LAST_UPDATE_DATE >= LAST_UPD_NO_PEN THEN EFFDT
			WHEN LAST_UPD_NO_PEN > LAST_UPDATE_DATE THEN EFFDT_NO_PEN END = @EffDate

			IF EXISTS(SELECT * FROM MstPlantEmpUpdTmp WHERE EmployeeID = @empUpd_ID)
			BEGIN
				UPDATE MstPlantEmpUpdTmp
				SET 
					EmployeeNumber = @EmployeeNumber,
					Name = @Name,
					JoinDate = @JoinDate,
					TitleID = @TitleID,
					ProcessSettingsCode = @ProcessSettingsCode,
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
					FlagCompleted = 1,
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
						   ,1 as [FlagCompleted]
						   ,[Loc_id]
						   ,'System' as [CreatedBy]
						   ,GETDATE() as [CreatedDate]
						   ,GETDATE() as [UpdatedDate]
						   ,'System' as [UpdatedBy]
				FROM MstPlantEmpUpd WHERE EmployeeID = @empUpd_ID
			END
			---------------	
			FETCH NEXT FROM cursor_EmpUpd   
			INTO @empUpd_ID, @EmployeeNumber, @Name, @LocationCode, @UnitCode, @GroupCode, @ProcessSettingsCode, @Status
		END;
	
		CLOSE cursor_EmpUpd;  
		DEALLOCATE cursor_EmpUpd; 
	END
END
END
GO