/****** Object:  View [dbo].[ExeReportByStatusMonthlyView]    Script Date: 6/18/2016 8:09:52 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Description: Edit dividebyzero NULLIF ExeReportByStatusMonthlyView
-- Ticket: http://tp.voxteneo.co.id/entity/3038
-- Author: Azka
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

ALTER VIEW [dbo].[ExeReportByStatusMonthlyView]
as
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
    ROUND(SUM(ISNULL(g.Absence_C, 0) + ISNULL(g.Absence_CH, 0) + ISNULL(g.Absence_CT, 0) + ISNULL(g.Absence_S, 0) + ISNULL(g.Absence_ETC, 0) + ISNULL(g.Absence_I, 0) + ISNULL(g.Absence_SLP, 0) + ISNULL(g.Absence_SLS, 0) + ISNULL(g.Absennce_A, 0)) ,2) AS ActualAbsWorker, 
	ROUND(AVG(CONVERT(DECIMAL,ISNULL(g.WorkHour, 0))), 2) AS ActualWorkHour, 
    ROUND(SUM(ISNULL(g.Production, 0)), 2) AS PrdPerStk, 
	COALESCE(ROUND((SUM(ISNULL(g.Production, 0)) / NULLIF(AVG(ISNULL(g.WorkHour, 0)),0) / NULLIF(SUM(ISNULL(g.EmpIn, 0) - (ISNULL(g.Multi_CUTT, 0) + ISNULL(g.Multi_FWRP, 0) + ISNULL(g.Multi_GEN, 0) + ISNULL(g.Multi_PACK, 0) + ISNULL(g.Multi_ROLL, 0) + ISNULL(g.Multi_STAMP, 0) + ISNULL(g.Multi_SWRP, 0) + ISNULL(g.Multi_TPO, 0) + ISNULL(g.Multi_WRP, 0))), 0)), 2) ,0) AS StkPerHrPerPpl, 
	COALESCE(ROUND((SUM(ISNULL(g.Production, 0)) / NULLIF(AVG(ISNULL(g.WorkHour, 0)), 0)), 2), 0) AS StkPerHr,
	COALESCE((ROUND(CONVERT(decimal, SUM(ISNULL(g.Production, 0))) / NULLIF((SELECT  SUM(ISNULL(Production, 0)) AS Expr1 FROM dbo.ExeReportByGroupsMonthly WHERE (LocationCode = g.LocationCode) AND (UnitCode = g.UnitCode) AND (BrandCode = g.BrandCode) AND (Year = DATEPART(yyyy,g.ProductionDate)) AND (Month = DATEPART(MM,g.ProductionDate))), 0),  3)), 0) AS BalanceIndex, 
	DATEPART(yyyy,g.ProductionDate)  as Year, 
	DATEPART(MM,g.ProductionDate)  as Month
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
	DATEPART(yyyy,g.ProductionDate) , 
	DATEPART(MM,g.ProductionDate)

GO