/****** Object:  View [dbo].[MstPlantProductionGroupView]    Script Date: 7/13/2016 11:39:50 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


ALTER VIEW [dbo].[MstPlantProductionGroupView]
AS
     SELECT ISNULL(ROW_NUMBER() OVER(ORDER BY MstPlantProductionGroup.CreatedDate DESC), -1) AS RowID,
            EmpJobsData.GroupCode,
            EmpJobsData.LocationCode,
            EmpJobsData.UnitCode,
            EmpJobsData.ProcessSettingsCode,
            EmpJobsData.WorkerCount,
            dbo.MstPlantProductionGroup.Leader1,
            dbo.MstPlantProductionGroup.Leader2,
            dbo.MstPlantProductionGroup.InspectionLeader,
            dbo.MstPlantProductionGroup.NextGroupCode,
            CAST(CASE
                     WHEN dbo.MstPlantProductionGroup.StatusActive IS NULL
                     THEN 1
                     ELSE dbo.MstPlantProductionGroup.StatusActive
                 END AS BIT) AS StatusActive,
            dbo.MstPlantProductionGroup.Remark,
            dbo.MstPlantProductionGroup.CreatedDate,
            dbo.MstPlantProductionGroup.CreatedBy,
            dbo.MstPlantProductionGroup.UpdatedDate,
            dbo.MstPlantProductionGroup.UpdatedBy,
            InspectionLeader.EmployeeName AS LeaderInspectionName,
            Leader1.EmployeeName AS Leader1Name,
            Leader2.EmployeeName AS Leader2Name
     FROM( 
           SELECT GroupCode,
                  LocationCode,
                  UnitCode,
                  ProcessSettingsCode,
                  COUNT(EmployeeID) AS WorkerCount
           FROM dbo.MstPlantEmpJobsDataAcv
           WHERE dbo.MstPlantEmpJobsDataAcv.Status = '5'
           GROUP BY GroupCode,
                    LocationCode,
                    UnitCode,
                    ProcessSettingsCode ) AS EmpJobsData
         LEFT OUTER JOIN dbo.MstPlantProductionGroup ON EmpJobsData.GroupCode = dbo.MstPlantProductionGroup.GroupCode
											 AND EmpJobsData.LocationCode = MstPlantProductionGroup.LocationCode
         LEFT JOIN MstPlantEmpJobsDataAll InspectionLeader ON MstPlantProductionGroup.InspectionLeader = InspectionLeader.EmployeeID
         LEFT JOIN MstPlantEmpJobsDataAll Leader1 ON MstPlantProductionGroup.Leader1 = Leader1.EmployeeID
         LEFT JOIN MstPlantEmpJobsDataAll Leader2 ON MstPlantProductionGroup.Leader2 = Leader2.EmployeeID;

GO


