IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[dbo].[EMPJOBDATA_SCHEDULER_UPDATE_ACV]'))
	DROP PROCEDURE [dbo].[EMPJOBDATA_SCHEDULER_UPDATE_ACV]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[EMPJOBDATA_SCHEDULER_UPDATE_ACV]
AS
BEGIN
	DECLARE @employeeID VARCHAR(64);
	DECLARE @employeeNumber VARCHAR(11);
	DECLARE @employeeName VARCHAR(64);
	DECLARE @joinDate DATETIME;
	DECLARE @titleID VARCHAR(12);
	DECLARE @processSettingsCode VARCHAR(16);
	DECLARE @status VARCHAR(64);
	DECLARE @cct VARCHAR(64);
	DECLARE @cctDesc VARCHAR(64);
	DECLARE @hcc VARCHAR(64);
	DECLARE @locationCode VARCHAR(8);
	DECLARE @groupCode VARCHAR(4);
	DECLARE @unitCode VARCHAR(4);
	DECLARE @loc_id VARCHAR(8);
	DECLARE @acvSts BIT;
	
	DECLARE @cursor1 CURSOR;
	
	SET @cursor1 = CURSOR FOR
	SELECT DISTINCT EmployeeID, EmployeeNumber, Name, JoinDate, TitleID, ProcessSettingsCode, Status, CCT, CCTDescription,
	HCC, LocationCode, GroupCode, UnitCode, Loc_id, AcvSts
	FROM MstPlantEmpUpd
	
	OPEN @cursor1
		FETCH NEXT FROM @cursor1
		INTO @employeeID, @employeeNumber, @employeeName, @joinDate, @titleID, @processSettingsCode, @status,
		@cct, @cctDesc, @hcc, @locationCode, @groupCode, @unitCode, @loc_id, @acvSts
		
		WHILE @@FETCH_STATUS = 0
		BEGIN
			IF(@acvSts = 1)
			BEGIN
				IF EXISTS
				(
					SELECT * FROM MstPlantEmpJobsDataAcv WHERE EmployeeID = @employeeID
				)
				BEGIN
					UPDATE MstPlantEmpJobsDataAcv
					SET
						EmployeeNumber = @employeeNumber,
						EmployeeName = @employeeName,
						JoinDate = @joinDate,
						Title_id = @titleID,
						ProcessSettingsCode = @processSettingsCode,
						Status = @status,
						CCT = @cct,
						CCTDescription = @cctDesc,
						HCC = @hcc,
						LocationCode = @locationCode,
						GroupCode = @groupCode,
						UnitCode = @unitCode,
						Loc_id = @loc_id,
						UpdatedDate = GETDATE(),
						UpdatedBy = 'System'
					WHERE EmployeeID = @employeeID
				END
				ELSE
				BEGIN
					INSERT INTO [dbo].[MstPlantEmpJobsDataAcv]
					   ([EmployeeID]
					   ,[EmployeeNumber]
					   ,[EmployeeName]
					   ,[JoinDate]
					   ,[Title_id]
					   ,[ProcessSettingsCode]
					   ,[Status]
					   ,[CCT]
					   ,[CCTDescription]
					   ,[HCC]
					   ,[LocationCode]
					   ,[GroupCode]
					   ,[UnitCode]
					   ,[Loc_id]
					   ,[Remark]
					   ,[CreatedDate]
					   ,[CreatedBy]
					   ,[UpdatedDate]
					   ,[UpdatedBy])
					SELECT [EmployeeID]
						  ,[EmployeeNumber]
						  ,[Name] as [EmployeeName]
						  ,[JoinDate]
						  ,[TitleID] as [Title_id]
						  ,[ProcessSettingsCode]
						  ,[Status]
						  ,[CCT]
						  ,[CCTDescription]
						  ,[HCC]
						  ,[LocationCode]
						  ,[GroupCode]
						  ,[UnitCode]
						  ,[Loc_id]
						  ,NULL as Remark
						  ,GETDATE() as [CreatedDate]
						  ,'System' as [CreatedBy]
						  ,GETDATE() as [UpdatedDate]
						  ,'System' as [UpdatedBy]
					  FROM [dbo].[MstPlantEmpUpd]
					  WHERE EmployeeID = @employeeID
				END;
			END
			ELSE 
			BEGIN
				DELETE MstPlantEmpJobsDataAcv WHERE EmployeeID = @employeeID;
			END

			FETCH NEXT FROM @cursor1
			INTO @employeeID, @employeeNumber, @employeeName, @joinDate, @titleID, @processSettingsCode, @status,
			@cct, @cctDesc, @hcc, @locationCode, @groupCode, @unitCode, @loc_id, @acvSts
		END; 
	
	    CLOSE @cursor1 ;
	    DEALLOCATE @cursor1;
END