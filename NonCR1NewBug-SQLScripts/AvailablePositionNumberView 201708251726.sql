USE [SKTIS]
GO
/****** Object:  StoredProcedure [dbo].[AvailablePositionNumberView]    Script Date: 25-Aug-17 3:46:03 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
--author : tim HMS

--author : hakim
-- date : 2017-08-25
-- desc : add parameter unitcode & add max employee filter

ALTER Procedure [dbo].[AvailablePositionNumberView]
(
	@LocationCode varchar(10),
	@GroupCode varchar(5),	
	@UnitCode Varchar(4)
)
as
Begin
  --SELECT nilai3.LocationCode,nilai3.EmployeeNumber,nilai3.GroupCode,
  --(SELECT StatusEmp  FROM dbo.MstGenEmpStatus  where StatusIdentifier =  SUBSTRING(GroupCode,2,1) ) as Status,nilai3.UnitCode,nilai3.ProcessSettingsCode   FROM(SELECT distinct  (SELECT  EmployeeNumber  
  --FROM [dbo].[MstPlantEmpJobsDataAcv]  
  --where LocationCode = @LocationCode and GroupCode = @GroupCode and EmployeeNumber =  A.EmployeeNumber  ) EmployeeNumber
  --FROM [dbo].[MstPlantEmpJobsDataAll] As A
  --where LocationCode = @LocationCode and GroupCode = @GroupCode)  nilai1   Right Join
  --(SELECT * FROM(SELECT distinct locationcode,(EmployeeNumber) EmployeeNumber,GroupCode, UnitCode,ProcessSettingsCode,Status 
  --FROM [MstPlantEmpJobsDataAll]
  --where LocationCode = @LocationCode and GroupCode = @GroupCode) as nilai2) nilai3 on nilai1.EmployeeNumber = nilai3.EmployeeNumber
  --where nilai1.EmployeeNumber is null	
	--declare @LocationCode  varchar(10)
	--declare @GroupCode varchar(5)
	--declare @UnitCode Varchar(4)

	--SET @LocationCode = 'id22'
	--SET @GroupCode = '1145'
	--SET @UnitCode = '2024'

  SELECT nilai3.LocationCode, nilai3.EmployeeNumber, nilai3.GroupCode,
  (SELECT StatusEmp  FROM dbo.MstGenEmpStatus  WHERE StatusIdentifier =  SUBSTRING(GroupCode,2,1) ) as Status,
  nilai3.UnitCode, nilai3.ProcessSettingsCode FROM (
														SELECT distinct (
																			SELECT  EmployeeNumber  
																			FROM [dbo].[MstPlantEmpJobsDataAcv]  
																			WHERE LocationCode = @LocationCode 
																			and GroupCode = @GroupCode 
																			and EmployeeNumber =  A.EmployeeNumber  
																			and UnitCode = @UnitCode 
																		) EmployeeNumber
														FROM [dbo].[MstPlantEmpJobsDataAcv] As A
														where LocationCode = @LocationCode and GroupCode = @GroupCode AND UnitCode = @UnitCode
													) nilai1
  RIGHT JOIN
  (
	SELECT * FROM (
					SELECT distinct locationcode,(EmployeeNumber) EmployeeNumber,GroupCode, UnitCode,ProcessSettingsCode,Status 
					FROM [MstPlantEmpJobsDataAll]
					WHERE LocationCode = @LocationCode and GroupCode = @GroupCode AND UnitCode = @UnitCode
				  ) as nilai2
   ) nilai3 
   ON nilai1.EmployeeNumber = nilai3.EmployeeNumber
   WHERE nilai1.EmployeeNumber is null AND 
   nilai3.EmployeeNumber < (
								SELECT MAX(EmployeeNumber) FROM MstPlantEmpJobsDataAcv
								WHERE LocationCode = @LocationCode 
								AND GroupCode = @GroupCode
								AND UnitCode = @UnitCode
							)
 End
