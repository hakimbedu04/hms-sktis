IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[dbo].[EMPJOBDATA_PopulateUpd]'))
	DROP PROCEDURE [dbo].[EMPJOBDATA_PopulateUpd]
GO
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE PROCEDURE [dbo].[EMPJOBDATA_PopulateUpd]
AS
BEGIN
	DECLARE @currDateTime DATETIME; 
	SET @currDateTime = GETDATE();
	
	DECLARE @currLastUpdatedUpd DATETIME;
	SELECT @currLastUpdatedUpd = ISNULL(DATEADD(S, MAX(RowVersion), '1970-01-01'), '1970-01-01') FROM MstPlantEmpUpdTmp
	
	INSERT INTO MstPlantEmpUpd
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
           ,[CreatedDate]
           ,[CreatedBy]
           ,[UpdatedDate]
           ,[UpdatedBy])
	SELECT
	  A.EMPLOYEE_ID as EmployeeID,
	  A.EMPLOYEE_NO as EmployeeNumber,
	  A.NAME as Name,
	  A.JOIN_DT as JoinDate,
	  A.TITLE_ID as TitleID,
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
	  ) AS ProcessSettingsCode,
	  A.STATUS as Status,
	  A.COST_CENTRE as CCT,
	  ISNULL(A.COST_CENTRE_DESCR, 'NULL') as CCTDescription,
	  A.HEAD_OF_COST_CENTRE AS HCC,
	  A.LOCATION_OPM as LocationCode,
	  A.KELOMPOK as GroupCode,
	  A.UNIT_MMR as UnitCode,
	  CASE WHEN A.LAST_UPDATE_DATE >= A.LAST_UPD_NO_PEN THEN A.LAST_UPDATE_DATE
	       WHEN A.LAST_UPD_NO_PEN > A.LAST_UPDATE_DATE THEN A.LAST_UPD_NO_PEN END 
	  as CompletedDate,
	  CASE WHEN A.ACS_STS = 'A' THEN 1 ELSE 0 END as AcvSts,
	  CASE WHEN A.LAST_UPDATE_DATE >= A.LAST_UPD_NO_PEN THEN A.EFFDT
		   WHEN A.LAST_UPD_NO_PEN > A.LAST_UPDATE_DATE THEN A.EFFDT_NO_PEN END 
	  as EffDate,
	  A.ACTION as Action,
	  DATEDIFF(second,{d '1970-01-01'}, @currDateTime) as RowVersion,
	  1 as FlagCompleted,
	  A.LOCATION_ID as Loc_id,
	  @currDateTime as CreatedDate,
	  'System' as CreatedBy,
	  @currDateTime as UpdatedDate,
	  'System' as UpdatedBy
	FROM MSTEMPPSFT AS A, (SELECT EMPLOYEE_ID, MAX(LAST_UPD_NO_PEN) as LAST_UPD FROM MSTEMPPSFT GROUP BY EMPLOYEE_ID) B
	WHERE (A.COST_CENTRE IN (SELECT
	  CostCenter
	FROM dbo.MstGenLocation)
	)
	AND A.EMPLOYEE_ID = B.EMPLOYEE_ID
	AND A.LAST_UPD_NO_PEN = B.LAST_UPD
	AND ((A.LAST_UPDATE_DATE >= @currLastUpdatedUpd AND A.LAST_UPDATE_DATE >= A.EFFDT) 
			OR
		 (A.LAST_UPD_NO_PEN >= @currLastUpdatedUpd AND A.LAST_UPD_NO_PEN >= A.EFFDT_NO_PEN)) 
END;