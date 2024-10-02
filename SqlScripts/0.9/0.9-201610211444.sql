/****** Object:  StoredProcedure [dbo].[EMPJOBDATA_SCHEDULER_UPDATE_MSTPLANTPRODUCTIONGROUP_V2]    Script Date: 10/21/2016 2:44:11 PM ******/
IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[dbo].[EMPJOBDATA_SCHEDULER_UPDATE_MSTPLANTPRODUCTIONGROUP_V2]'))
	DROP PROCEDURE [dbo].[EMPJOBDATA_SCHEDULER_UPDATE_MSTPLANTPRODUCTIONGROUP_V2]
GO
/****** Object:  StoredProcedure [dbo].[EMPJOBDATA_SCHEDULER_UPDATE_MSTPLANTPRODUCTIONGROUP_V2]    Script Date: 10/21/2016 2:44:11 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



CREATE PROCEDURE [dbo].[EMPJOBDATA_SCHEDULER_UPDATE_MSTPLANTPRODUCTIONGROUP_V2]
AS
BEGIN

DECLARE @CurrentHour INT
SET @CurrentHour = DATEPART(HOUR, GETDATE())
IF @CurrentHour >= 0 AND @CurrentHour <=3
BEGIN
	DECLARE @groupCode VARCHAR(4);
	DECLARE @unitCode VARCHAR(4);
	DECLARE @processGroup VARCHAR(16);
	DECLARE @locationCode VARCHAR(8);
	
	-- Cursor Loop MstPlantProductionGroup
	DECLARE cursor_PlantProductionGroup CURSOR FOR
	SELECT LocationCode, UnitCode, ProcessGroup, GroupCode FROM MstPlantProductionGroup
	
	OPEN cursor_PlantProductionGroup
	
	FETCH NEXT FROM cursor_PlantProductionGroup   
	INTO @locationCode, @unitCode, @processGroup, @groupCode
	
	WHILE @@FETCH_STATUS = 0  
	BEGIN  
		DECLARE @workerCount INT;
		SELECT @workerCount = COUNT(EmployeeID) FROM MstPlantEmpJobsDataAcv WHERE LocationCode = @locationCode AND UnitCode = @unitCode AND ProcessSettingsCode = @processGroup
		AND GroupCode = @groupCode
	
		IF NOT EXISTS
		(
			SELECT * FROM MstPlantEmpJobsDataAcv 
			WHERE LocationCode = @locationCode AND UnitCode = @unitCode AND GroupCode = @groupCode AND ProcessSettingsCode = @processGroup
		)
		BEGIN
			UPDATE MstPlantProductionGroup 
			SET StatusActive = 0
			WHERE LocationCode = @locationCode AND UnitCode = @unitCode AND ProcessGroup = @processGroup AND GroupCode = @groupCode
		END
	
		UPDATE MstPlantProductionGroup 
		SET WorkerCount = @workerCount
		WHERE LocationCode = @locationCode AND UnitCode = @unitCode AND ProcessGroup = @processGroup AND GroupCode = @groupCode
		---------------	
		FETCH NEXT FROM cursor_PlantProductionGroup   
		INTO @locationCode, @unitCode, @processGroup, @groupCode
	END;
	
	CLOSE cursor_PlantProductionGroup;  
	DEALLOCATE cursor_PlantProductionGroup;  
	
	INSERT INTO [dbo].[MstPlantProductionGroup]
	           ([GroupCode]
	           ,[UnitCode]
	           ,[LocationCode]
	           ,[ProcessGroup]
	           ,[InspectionLeader]
	           ,[Leader1]
	           ,[Leader2]
	           ,[NextGroupCode]
	           ,[Mst_UnitCode]
	           ,[Mst_LocationCode]
	           ,[Mst_ProcessGroup]
	           ,[AvailableNumbers]
	           ,[WorkerCount]
	           ,[StatusActive]
	           ,[Remark]
	           ,[CreatedDate]
	           ,[CreatedBy]
	           ,[UpdatedDate]
	           ,[UpdatedBy])
			SELECT 
				[GroupCode]
	           ,[UnitCode]
	           ,[LocationCode]
	           ,ProcessSettingsCode as [ProcessGroup]
	           ,NULL as [InspectionLeader]
	           ,NULL as [Leader1]
	           ,NULL as [Leader2]
	           ,NULL as [NextGroupCode]
	           ,UnitCode as [Mst_UnitCode]
	           ,LocationCode as [Mst_LocationCode]
	           ,ProcessSettingsCode as [Mst_ProcessGroup]
	           ,0 as [AvailableNumbers]
	           ,COUNT(EmployeeID) as [WorkerCount]
	           ,1 as [StatusActive]
	           ,NULL as [Remark]
	           ,GETDATE() as [CreatedDate]
	           ,'SYSTEM' as [CreatedBy]
	           ,GETDATE() as [UpdatedDate]
	           ,'SYSTEM' as [UpdatedBy]
	FROM MstPlantEmpJobsDataAcv a
	WHERE NOT EXISTS 
	(SELECT LocationCode, UnitCode, ProcessGroup, GroupCode 
	FROM MstPlantProductionGroup g 
	WHERE g.LocationCode = a.LocationCode and g.UnitCode = a.UnitCode and g.ProcessGroup = a.ProcessSettingsCode and g.GroupCode = a.GroupCode
	) AND a.Status = 5
	group by LocationCode, UnitCode, ProcessSettingsCode, GroupCode
	
	END
END

GO