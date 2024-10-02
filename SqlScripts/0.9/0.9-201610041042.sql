IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[dbo].[RunMstPlantEmpJobsDataAcv2]'))
	DROP PROCEDURE [dbo].[RunMstPlantEmpJobsDataAcv2]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[RunMstPlantEmpJobsDataAcv2]
AS
BEGIN
DECLARE
		@CurrentHour INT

		SET @CurrentHour = DATEPART(HOUR, GETDATE())
	
		IF @CurrentHour >= 0 AND @CurrentHour <=3
		BEGIN
	MERGE MstPlantEmpJobsDataAcv AS target
	USING(
	SELECT
		A.EMPLOYEE_ID,
		A.EMPLOYEE_NO,
		A.NAME,
		A.JOIN_DT,
		A.TITLE_ID,
		 ISNULL(CASE WHEN A.TITLE_DESCR = 'Gunting' THEN 'CUTTING'
				  WHEN A.TITLE_DESCR = 'Giling' THEN 'ROLLING'
				  WHEN A.TITLE_DESCR = 'Pak' THEN 'PACKING'
				  WHEN A.TITLE_DESCR = 'Banderol' THEN 'STAMPING'
				  WHEN A.TITLE_DESCR = 'Wrapping' THEN 'WRAPPING'
				  WHEN A.TITLE_DESCR = 'General Worker Non Production' THEN 'GENERAL WORKER'
				  WHEN A.TITLE_DESCR = 'General Worker Production' THEN 'GENERAL WORKER'
				  WHEN A.TITLE_DESCR = 'General Worker Sorting Bundl.' THEN 'GENERAL WORKER'
				  WHEN A.TITLE_DESCR = 'Group Leader' THEN 'GROUP LEADER'
				  ELSE UPPER(A.TITLE_DESCR) END, 'NULL' 
	   ) AS TITLE_DESCR,
		A.STATUS,
		A.COST_CENTRE,
		ISNULL(
			A.COST_CENTRE_DESCR,
			'NULL'
		) AS COST_CENTRE_DESC,
		A.HEAD_OF_COST_CENTRE,
		ISNULL(
			A.LOCATION_ID,
			'NULL'
		) AS LOCATION_ID,
		A.KELOMPOK,
		A.UNIT_MMR,
		A.LOCATION_OPM,
		NULL AS REMARK,
		GETDATE() AS CreatedDate,
		'System' AS CreatedBy,
		GETDATE() AS UpdatedDate,
		'System' AS UpdatedBy,
		A.ACS_STS AS ACS_STATUS
	FROM
		MSTEMPPSFT AS A
	WHERE
		(
			A.COST_CENTRE IN(
				SELECT
					CostCenter
				FROM
					dbo.MstGenLocation
			) AND A.ACS_STS = 'A'
		)
) AS SOURCE ON
(
	target.EmployeeID = source.EMPLOYEE_ID
)
WHEN MATCHED THEN UPDATE
SET
	EmployeeNumber = source.EMPLOYEE_NO,
	EmployeeName = source.NAME,
	JoinDate = source.JOIN_DT,
	Title_id = source.TITLE_ID,
	ProcessSettingsCode = source.TITLE_DESCR,
	Status = source.STATUS,
	CCT = source.COST_CENTRE,
	CCTDescription = source.COST_CENTRE_DESC,
	HCC = source.HEAD_OF_COST_CENTRE,
	LocationCode = source.LOCATION_OPM,
	GroupCode = source.KELOMPOK,
	UnitCode = source.UNIT_MMR,
	Loc_id = source.LOCATION_ID,
	Remark = source.REMARK,
	UpdatedDate = source.UpdatedDate,
	UpdatedBy = source.UpdatedBy
	WHEN NOT MATCHED THEN INSERT
		(
			EmployeeID,
			EmployeeNumber,
			EmployeeName,
			JoinDate,
			Title_id,
			ProcessSettingsCode,
			Status,
			CCT,
			CCTDescription,
			HCC,
			LocationCode,
			GroupCode,
			UnitCode,
			Loc_id,
			Remark,
			CreatedDate,
			CreatedBy,
			UpdatedDate,
			UpdatedBy
		)
	VALUES(
		source.EMPLOYEE_ID,
		source.EMPLOYEE_NO,
		source.NAME,
		source.JOIN_DT,
		source.TITLE_ID,
		source.TITLE_DESCR,
		source.STATUS,
		source.COST_CENTRE,
		source.COST_CENTRE_DESC,
		source.HEAD_OF_COST_CENTRE,
		source.LOCATION_OPM,
		source.KELOMPOK,
		source.UNIT_MMR,
		source.REMARK,
		source.LOCATION_ID,
		source.CreatedDate,
		source.CreatedBy,
		source.UpdatedDate,
		source.UpdatedBy
	);
		END
-- remove acv I from MstPlantEmpJobsDataAcv
DEClARE @employeeID VARCHAR(64);
DECLARE @acvSts VARCHAR(5);
DECLARE cursor_psft CURSOR LOCAL FOR
SELECT EMPLOYEE_ID, ACS_STS
FROM MSTEMPPSFT

OPEN cursor_psft

FETCH NEXT FROM cursor_psft   
INTO @employeeID, @acvSts

WHILE @@FETCH_STATUS = 0  
BEGIN  
	
	IF(@acvSts = 'I')
	AND EXISTS(SELECT * FROM MstPlantEmpJobsDataAcv WHERE EmployeeID = @employeeID)
	BEGIN
		DELETE MstPlantEmpJobsDataAcv WHERE EmployeeID = @employeeID
	END

FETCH NEXT FROM cursor_psft   
INTO @employeeID, @acvSts
END

CLOSE cursor_psft;  
DEALLOCATE cursor_psft;  

END