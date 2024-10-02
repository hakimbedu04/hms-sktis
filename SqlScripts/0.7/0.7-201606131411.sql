-- =============================================
-- Description: Join with MstPlantProductionGroup
-- Author: HAKIM
-- Updated: 6/13/2016
-- Ticket: http://tp.voxteneo.co.id/entity/7038
-- =============================================


ALTER VIEW [dbo].[ExePlantWorkerBalancingMulti]
AS
Select acv.EmployeeID,acv.EmployeeNumber,EmployeeName,B.GroupCode as SourceGroupcode,B.UnitCode as SourceUnitCode,B.LocationCode as SourceLocationCode,B.procfrom as SourceProcess,plnt.ProcessGroup as DestinationProcess,'' as UnitCodeDestination,'' as GroupCodeDestination ,cast('FALSE' as bit) as StatFrom from ( 
(select EmployeeID,Count(*) as Jumlah from PlanPlantIndividualCapacityWorkHours Group by EmployeeID Having Count(*) >1 ) as A 
inner join PlanPlantIndividualCapacityWorkHours plnt on A.EmployeeID = plnt.EmployeeID inner join MstPlantEmpJobsDataAcv acv on acv.EmployeeID = A.EmployeeID
inner join
(Select distinct EmployeeNumber as empnum,plntB.ProcessGroup as procfrom,plntB.GroupCode as groupcode,plntB.UnitCode as unitcode,plntB.LocationCode from 
(select EmployeeID from PlanPlantIndividualCapacityWorkHours Group by EmployeeID Having Count(*) >1 ) as C 
inner join PlanPlantIndividualCapacityWorkHours plntB on C.EmployeeID = plntB.EmployeeID 
inner join
MstPlantProductionGroup MPlntProcGr on plntB.ProcessGroup = MPlntProcGr.ProcessGroup and plntB.GroupCode = MPlntProcGr.GroupCode
inner join MstPlantEmpJobsDataAcv acv on acv.EmployeeID = C.EmployeeID where EmployeeNumber Like '%'+plntB.GroupCode+'%') as B on B.empnum = EmployeeNumber
) where B.procfrom NOT LIKE plnt.ProcessGroup 
Union ALL
Select *From (Select asgn.EmployeeID,asgn.EmployeeNumber,acv.EmployeeName,SourceGroupCode,SourceUnitCode,SourceLocationCode,SourceProcessGroup as SourceProcess,DestinationProcessGroup as DestinationProcess, DestinationUnitCode,DestinationGroupCode,cast('TRUE' as bit) as StatFrom From ExePlantWorkerAssignment asgn inner join MstPlantEmpJobsDataAcv acv on acv.EmployeeID = asgn.EmployeeID  where SourceProcessGroup NOT Like DestinationProcessGroup and CONVERT(VARCHAR(10),StartDate,110)  <= CONVERT(VARCHAR(10),GETDATE(),110) and CONVERT(VARCHAR(10),EndDate,110)  >=CONVERT(VARCHAR(10),GETDATE(),110) )as D




GO