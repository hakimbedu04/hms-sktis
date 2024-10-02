/****** Object:  UserDefinedFunction [dbo].[GetReportProdStockProcessView]    Script Date: 5/25/2016 1:41:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description: Get Data Report - Production and Stock Report by Process
-- Ticket: http://tp.voxteneo.co.id/entity/3044
-- Author: AZKA
-- Updated: 1.0 - 2016/05/12
-- =============================================

-- =============================================
-- Description: edit Production get from by process
-- Ticket: hot fix
-- Author: AZKA
-- Updated: 1.0 - 5/25/2016
-- =============================================

ALTER FUNCTION [dbo].[GetReportProdStockProcessView]
(
	@LocationCode as varchar(10),
	@UnitCode as varchar(10),
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
		WHERE pc.LocationCode IN (SELECT LocationCode FROM GetLastChildLocation(@LocationCode)) and UnitCode = @UnitCode AND UOMOrder IN (11, 13) and ProductionDate = @DateFrom
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
		ROUND((SUM(ISNULL(bp.Production,0))), 2) Production
	FROM ExeReportByProcess bp
	INNER JOIN MstGenBrand gb on gb.BrandCode = bp.BrandCode
	WHERE LocationCode IN (SELECT LocationCode FROM GetLastChildLocation(@LocationCode)) and UnitCode = @UnitCode and (ProcessGroup = 'STAMPING' or ProcessGroup = 'WRAPPING') and ProductionDate >= @DateFrom AND ProductionDate <= @DateTo and UOMOrder = 7
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
		WHERE pc.LocationCode IN (SELECT LocationCode FROM GetLastChildLocation(@LocationCode)) and UnitCode = @UnitCode AND UOMOrder IN (11, 13) and ProductionDate >= @DateFrom AND ProductionDate <= @DateTo
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
		WHERE pc.LocationCode IN (SELECT LocationCode FROM GetLastChildLocation(@LocationCode)) and UnitCode = @UnitCode AND UOMOrder IN (11, 13) and ProductionDate = @DateTo
	) byProcess
	PIVOT
	(
		SUM(EndingStock)
		FOR UOMOrder IN ([11], [13])
	) AS piv
),
Planning AS
(
	-- Planning
	select 
		BrandGroupCode, BrandCode, LocationCode, UnitCode,
		sum(grp.TPKValue) TPKValue
	from 
	(
		select distinct
			BrandGroupCode, BrandCode, LocationCode, UnitCode, ProductionDate,
			TPKValue
		from ExeReportByGroups 
		where LocationCode IN (SELECT LocationCode FROM GetLastChildLocation(@LocationCode)) and UnitCode = @UnitCode and ProductionDate >= @DateFrom and ProductionDate <= @DateTo and (ProcessGroup = 'STAMPING' or ProcessGroup = 'WRAPPING')
	) grp
	group by
		BrandGroupCode, BrandCode, LocationCode, UnitCode

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
	F.TPKValue as Planning,
	D.PAP,
	D.PAG,
	ISNULL(E.EndingStockInternalMove, 0) EndingStockInternalMove,
	ISNULL(E.EndingStockExternalMove, 0) EndingStockExternalMove
FROM Production P 
INNER JOIN Movement D ON D.BrandGroupCode = P.BrandGroupCode AND D.BrandCode = P.BrandCode AND D.LocationCode = P.LocationCode AND D.UnitCode = P.UnitCode
INNER JOIN Planning F ON F.BrandGroupCode = P.BrandGroupCode AND F.BrandCode = P.BrandCode AND F.LocationCode = P.LocationCode AND F.UnitCode = P.UnitCode
LEFT JOIN BeginStock B ON B.BrandGroupCode = P.BrandGroupCode AND B.BrandCode = P.BrandCode AND B.LocationCode = P.LocationCode AND B.UnitCode = P.UnitCode
LEFT JOIN EndingStock E ON E.BrandGroupCode = P.BrandGroupCode AND E.BrandCode = P.BrandCode AND E.LocationCode = P.LocationCode AND E.UnitCode = P.UnitCode
)