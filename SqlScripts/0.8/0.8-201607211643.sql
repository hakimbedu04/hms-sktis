-- =============================================
-- Description: Edit dividebyzero NULLIF ExeReportByStatusWeeklyView
-- Ticket: http://tp.voxteneo.co.id/entity/3038
-- Author: Azka - 2016/02/23
-- =============================================

-- =============================================
-- Description: Edit dividebyzero NULLIF, ISNULL, COALESCE if null then 0 ExeReportByStatusWeeklyView
-- Ticket: http://tp.voxteneo.co.id/entity/3038
-- Author: Azka - 2016/02/24
-- =============================================

-- =============================================
-- Description: Add Brand Group Code
-- Ticket: 
-- Author: Indra - 2016/02/24
-- =============================================

-- =============================================
-- Description: add absent_S on ActualAbsWorker		
-- Ticket: 
-- Author: azka - 2016/06/18
-- =============================================

-- =============================================
-- Description: Exluce SLP and SLS from calculation
-- Ticket: http://tp.voxteneo.co.id/entity/8456
-- Author: Abud - 2016/07/21
-- =============================================

ALTER VIEW [dbo].[ExeReportByStatusWeeklyView]
AS
SELECT        
	g.LocationCode, 
	g.UnitCode, 
	g.Shift, 
	g.BrandCode, 
	b.BrandGroupCode,
	g.ProcessGroup, 
	g.StatusEmp, 
	e.StatusIdentifier,
    ROUND(SUM(ISNULL(g.EmpIn, 0) - (ISNULL(g.Multi_CUTT, 0) + ISNULL(g.Multi_FWRP, 0) + ISNULL(g.Multi_GEN, 0) + ISNULL(g.Multi_PACK, 0) + ISNULL(g.Multi_ROLL, 0) + ISNULL(g.Multi_STAMP, 0) + iSNULL(g.Multi_SWRP, 0) + ISNULL(g.Multi_TPO, 0) + ISNULL(g.Multi_WRP, 0))), 2) AS ActualWorker, 
    ROUND(
	SUM(
	ISNULL(g.Absence_C, 0) 
	+ ISNULL(g.Absence_CH, 0) 
	+ ISNULL(g.Absence_CT, 0) 
	+ ISNULL(g.Absence_S, 0) 
	+ ISNULL(g.Absence_ETC, 0) 
	+ ISNULL(g.Absence_I, 0) 
	--+ ISNULL(g.Absence_SLP, 0) 
	--+ ISNULL(g.Absence_SLS, 0) 
	+ ISNULL(g.Absennce_A, 0)
	), 2) AS ActualAbsWorker, 
	ROUND(AVG(ISNULL(g.WorkHour, 0)), 2) AS ActualWorkHour, 
    ROUND(SUM(ISNULL(g.Production, 0)) ,2) AS PrdPerStk, 
	COALESCE(ROUND((SUM(ISNULL(g.Production, 0)) / NULLIF(AVG(ISNULL(g.WorkHour, 0)),0) / NULLIF(SUM(ISNULL(g.EmpIn, 0) - (ISNULL(g.Multi_CUTT, 0) + ISNULL(g.Multi_FWRP, 0) + ISNULL(g.Multi_GEN, 0) + ISNULL(g.Multi_PACK, 0) + ISNULL(g.Multi_ROLL, 0) + ISNULL(g.Multi_STAMP, 0) + ISNULL(g.Multi_SWRP, 0) + ISNULL(g.Multi_TPO, 0) + ISNULL(g.Multi_WRP, 0))), 0)), 2) ,0) AS StkPerHrPerPpl, 
	COALESCE(ROUND((SUM(ISNULL(g.Production, 0)) / NULLIF(AVG(ISNULL(g.WorkHour, 0)), 0)), 2), 0) AS StkPerHr, 
	COALESCE((ROUND(CONVERT(decimal, SUM(ISNULL(g.Production, 0))) / NULLIF((SELECT SUM(ISNULL(Production, 0)) AS Expr1 FROM dbo.ExeReportByGroupsWeekly WHERE (LocationCode = g.LocationCode) AND (UnitCode = g.UnitCode) AND (BrandCode = g.BrandCode) AND (KPSYear = g.KPSYear) AND (KPSWeek = g.KPSWeek)), 0), 3)) ,0) AS BalanceIndex, 
	g.KPSYear, 
    g.KPSWeek
FROM            
	dbo.ExeReportByGroups AS g INNER JOIN
    dbo.MstGenProcess AS p ON p.ProcessGroup = g.ProcessGroup INNER JOIN
    dbo.MstGenEmpStatus AS e ON e.StatusEmp = g.StatusEmp INNER JOIN
	dbo.MstGenBrand AS b ON g.BrandCode = b.BrandCode
GROUP BY 
	g.LocationCode, 
	g.UnitCode, 
	g.Shift, 
	g.BrandCode, 
	b.BrandGroupCode,
	g.ProcessGroup, 
	g.StatusEmp, 
	e.StatusIdentifier,
	g.KPSYear, 
	g.KPSWeek

GO


