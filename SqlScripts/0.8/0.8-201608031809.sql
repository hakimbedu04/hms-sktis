-- =============================================
-- Description: adding StatusEmp PRIMARY KEY 
-- Author: AZKA
-- Updated: 1.0 - 2016/08/03
-- =============================================

ALTER TABLE dbo.ExeReportByGroups
	DROP CONSTRAINT PK_ExeReportByGroups

ALTER TABLE dbo.ExeReportByGroups ADD CONSTRAINT
	PK_ExeReportByGroups PRIMARY KEY CLUSTERED 
	(
	LocationCode,
	GroupCode,
	BrandCode,
	StatusEmp,
	ProductionDate
	) WITH( STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]


ALTER TABLE dbo.ExeReportByGroups SET (LOCK_ESCALATION = TABLE)
