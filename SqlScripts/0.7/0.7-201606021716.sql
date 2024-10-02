-- Ticket: http://tp.voxteneo.co.id/entity/6876
-- Author: Indra
-- Date: 2016/06/02

CREATE VIEW [dbo].[PRD_PROD_CARD]
AS
SELECT 
	ProductionCardCode as PROD_CARD_NO,
	BrandCode as OPM_BRAND_CODE,
	BrandGroupCode as GL_CLASS,
	EmployeeID as EMP_ID,
	ProductionDate as PROD_DATE,
	CAST(Production as decimal(18,2)) as PROD_CURRENT,
	CAST(UpahLain as decimal(18,2)) as PAY_OTHER,
	Absent as ABSENT_TYPE,
	CreatedBy as CREATED_BY,
	CreatedDate as CREATED_DATE,
	UpdatedBy as LAST_UPDATED_BY,
	UpdatedDate as LAST_UPDATED_DATE,
	EmployeeNumber as EMP_NO,
	EblekAbsentType as EBLEK_ABS_TYPE,
	RevisionType as REV_TYPE,
	Remark as REMARKS,
	WorkHours as WORK_HOURS,
	Shift as SHIFT
FROM dbo.ProductionCard 



