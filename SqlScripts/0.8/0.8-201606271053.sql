/****** Object:  UserDefinedFunction [dbo].[GetReportProdStockProcessAllUnitView]    Script Date: 6/27/2016 10:46:47 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description: Get Data Report - Production and Stock Report by Process All Unit
-- Ticket: http://tp.voxteneo.co.id/entity/3044
-- Author: AZKA
-- Updated: 1.0 - 2016/05/12
-- =============================================

-- =============================================
-- Description: edit Production get from by process
-- Ticket: hot fix
-- Author: AZKA
-- Updated: 1.0 - 5/26/2016
-- =============================================

-- =============================================
-- Description: change production to keluar bersih
-- Ticket: http://tp.voxteneo.co.id/entity/6821
-- Author: AZKA
-- Updated: 4.0 - 6/14/2016
-- =============================================

-- =============================================
-- Description: change Planning for TPO
-- Ticket: http://tp.voxteneo.co.id/entity/7240
-- Author: AZKA
-- Updated: 5.0 - 2016/06/27
-- =============================================

ALTER FUNCTION [dbo].[GetReportProdStockProcessAllUnitView]
(
	@LocationCode as varchar(10),
	@DateFrom datetime,
	@DateTo datetime
)
RETURNS TABLE 
AS
RETURN
(
WITH BeginStock AS
(
	-- BeginningStock
	SELECT BrandGroupCode, BrandCode, LocationCode, UnitCode, [11] AS BeginStockInternalMove, [13] AS BeginStockExternalMove
	FROM
	(
		SELECT 
			gb.BrandGroupCode,
			pc.BrandCode,
			pc.LocationCode,
			pc.UnitCode,
			pc.UOMOrder,
			pc.BeginningStock
		FROM ExeReportByProcess pc
		INNER JOIN MstGenBrand gb on gb.BrandCode = pc.BrandCode
		WHERE pc.LocationCode IN (SELECT LocationCode FROM GetLastChildLocation(@LocationCode)) AND UOMOrder IN (11, 13) and ProductionDate = @DateFrom
	) byProcess
	PIVOT
	(
		SUM(BeginningStock)
		FOR UOMOrder IN ([11], [13])
	) AS piv
),
Production AS
(
	-- Production
	SELECT 
		gb.BrandGroupCode,
		bp.BrandCode,
		bp.LocationCode, 
		bp.UnitCode,
		ROUND((SUM(ISNULL(bp.KeluarBersih, 0))), 2) Production
	FROM ExeReportByProcess bp
	INNER JOIN MstGenBrand gb on gb.BrandCode = bp.BrandCode
	WHERE LocationCode IN (SELECT LocationCode FROM GetLastChildLocation(@LocationCode)) and (ProcessGroup = 'STAMPING' or ProcessGroup = 'WRAPPING') and ProductionDate >= @DateFrom AND ProductionDate <= @DateTo and UOMOrder = 7
	GROUP BY
		BrandGroupCode,
		bp.BrandCode,
		LocationCode, 
		UnitCode
),
Movement AS
(
	-- Movement
	SELECT BrandGroupCode, BrandCode, LocationCode, UnitCode, [11] AS PAP, [13] AS PAG
	FROM
	(
		SELECT 
			gb.BrandGroupCode,
			pc.BrandCode,
			pc.LocationCode,
			pc.UnitCode,
			pc.UOMOrder,
			pc.KeluarBersih
		FROM ExeReportByProcess pc
		INNER JOIN MstGenBrand gb on gb.BrandCode = pc.BrandCode
		WHERE pc.LocationCode IN (SELECT LocationCode FROM GetLastChildLocation(@LocationCode)) AND UOMOrder IN (11, 13) and ProductionDate >= @DateFrom AND ProductionDate <= @DateTo
	) byProcess
	PIVOT
	(
		SUM(KeluarBersih)
		FOR UOMOrder IN ([11], [13])
	) AS piv
),
EndingStock AS
(
	-- EndingStock
	SELECT BrandGroupCode, BrandCode, LocationCode, UnitCode, [11] AS EndingStockInternalMove, [13] AS EndingStockExternalMove
	FROM
	(
		SELECT 
			gb.BrandGroupCode,
			pc.BrandCode,
			pc.LocationCode,
			pc.UnitCode,
			pc.UOMOrder,
			pc.EndingStock
		FROM ExeReportByProcess pc
		INNER JOIN MstGenBrand gb on gb.BrandCode = pc.BrandCode
		WHERE pc.LocationCode IN (SELECT LocationCode FROM GetLastChildLocation(@LocationCode)) AND UOMOrder IN (11, 13) and ProductionDate = @DateTo
	) byProcess
	PIVOT
	(
		SUM(EndingStock)
		FOR UOMOrder IN ([11], [13])
	) AS piv
),
PlanningPlant AS
(
	-- Planning Plant
	select 
		BrandGroupCode, BrandCode, LocationCode, UnitCode,
		sum(grp.TPKValue) TPKValue
	from 
	(
		select distinct
			BrandGroupCode, BrandCode, LocationCode, UnitCode, ProductionDate,
			TPKValue
		from ExeReportByGroups 
		where LocationCode IN (SELECT LocationCode FROM GetLastChildLocation(@LocationCode)) and ProductionDate >= @DateFrom and ProductionDate <= @DateTo  and (ProcessGroup = 'STAMPING' or ProcessGroup = 'WRAPPING')
	) grp
	group by
		BrandGroupCode, BrandCode, LocationCode, UnitCode

),
PlanningTPO AS
(
select up.BrandGroupCode, up.BrandCode, up.LocationCode, sum(v.TargetValue * gbg.StickPerBox) as Planning from
(
	select gb.BrandGroupCode, box.BrandCode, box.LocationCode, box.KPSYear, box.KPSWeek, gw.StartDate as [TargetManual1], 
		DATEADD(dd, 1, gw.StartDate) as [TargetManual2],
		DATEADD(dd, 2, gw.StartDate) as [TargetManual3],
		DATEADD(dd, 3, gw.StartDate) as [TargetManual4],
		DATEADD(dd, 4, gw.StartDate) as [TargetManual5],
		DATEADD(dd, 5, gw.StartDate) as [TargetManual6],
		DATEADD(dd, 6, gw.StartDate) as [TargetManual7]
	from PlanTPOTargetProductionKelompokBox box
	inner join MstGenBrand gb on gb.BrandCode = box.BrandCode
	inner join MstGenWeek gw on gw.Year = box.KPSYear and gw.Week = box.KPSWeek
	where box.LocationCode IN (SELECT LocationCode FROM GetLastChildLocation(@LocationCode))
	) box
	unpivot
	(
		ProductionDate for [Day] IN ([TargetManual1], [TargetManual2], [TargetManual3], [TargetManual4], [TargetManual5], [TargetManual6], [TargetManual7])
	) as up
	INNER JOIN
	(
	
	select BrandGroupCode,BrandCode, LocationCode, TargetManual, TargetValue, KPSYear,KPSWeek from
	(
	select gb.BrandGroupCode, box.BrandCode, box.LocationCode, box.KPSYear, box.KPSWeek,
		box.TargetManual1,
		box.TargetManual2,
		box.TargetManual3,
		box.TargetManual4,
		box.TargetManual5,
		box.TargetManual6,
		box.TargetManual7
	from PlanTPOTargetProductionKelompokBox box
	inner join MstGenBrand gb on gb.BrandCode = box.BrandCode
	inner join MstGenWeek gw on gw.Year = box.KPSYear and gw.Week = box.KPSWeek
	where box.LocationCode IN (SELECT LocationCode FROM GetLastChildLocation(@LocationCode))
	) box
	unpivot
	(
		TargetValue for [TargetManual] IN (TargetManual1, TargetManual2, TargetManual3, TargetManual4, TargetManual5, TargetManual6, TargetManual7)
	) as up1
	) v on v.BrandCode = up.BrandCode and v.BrandGroupCode = up.BrandGroupCode and v.KPSWeek = up.KPSWeek and v.KPSYear = up.KPSYear and v.TargetManual = up.Day and v.LocationCode = up.LocationCode
	inner join MstGenBrandGroup gbg on gbg.BrandGroupCode = v.BrandGroupCode
	where ProductionDate between @DateFrom and @DateTo
group by
up.BrandGroupCode, up.BrandCode, up.LocationCode
)
SELECT 
	P.BrandGroupCode, 
	P.BrandCode,
	P.LocationCode,
	P.UnitCode,
	ISNULL(B.BeginStockInternalMove, 0) BeginStockInternalMove,
	ISNULL(B.BeginStockExternalMove, 0) BeginStockExternalMove,
	P.Production,
	P.Production - F.TPKValue as VarianceStick,
	COALESCE(((P.Production - F.TPKValue) / NULLIF(F.TPKValue, 0)) * 100, 0) as VariancePercent,
	CASE WHEN (SELECT dbo.GetParentLocationLastChild(@LocationCode)) = 'PLANT' THEN F.TPKValue ELSE T.Planning END as Planning,
	D.PAP,
	D.PAG,
	ISNULL(E.EndingStockInternalMove, 0) EndingStockInternalMove,
	ISNULL(E.EndingStockExternalMove, 0) EndingStockExternalMove
FROM Production P 
INNER JOIN Movement D ON D.BrandGroupCode = P.BrandGroupCode AND D.BrandCode = P.BrandCode AND D.LocationCode = P.LocationCode AND D.UnitCode = P.UnitCode
LEFT JOIN PlanningTPO T ON T.BrandCode = P.BrandCode AND T.BrandGroupCode = P.BrandGroupCode AND T.LocationCode = P.LocationCode 
LEFT JOIN PlanningPlant F ON F.BrandGroupCode = P.BrandGroupCode AND F.BrandCode = P.BrandCode AND F.LocationCode = P.LocationCode AND F.UnitCode = P.UnitCode
LEFT JOIN BeginStock B ON B.BrandGroupCode = P.BrandGroupCode AND B.BrandCode = P.BrandCode AND B.LocationCode = P.LocationCode AND B.UnitCode = P.UnitCode
LEFT JOIN EndingStock E ON E.BrandGroupCode = P.BrandGroupCode AND E.BrandCode = P.BrandCode AND E.LocationCode = P.LocationCode AND E.UnitCode = P.UnitCode

)
