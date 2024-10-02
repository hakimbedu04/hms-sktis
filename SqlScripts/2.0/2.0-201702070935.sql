IF EXISTS (SELECT * FROM sys.objects WHERE  object_id = OBJECT_ID(N'[dbo].[GetExeReportDailyProductionAchievement]') AND type IN ( N'FN', N'IF', N'TF', N'FS', N'FT' ))
  DROP FUNCTION [dbo].[GetExeReportDailyProductionAchievement]

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description: Get Data Daily Production Achievement
-- Ticket: http://tp.voxteneo.co.id/entity/3045
-- Author: AZKA
-- Updated: 1.0 - 4/27/2016
-- =============================================

-- =============================================
-- Description: Edit join msttpopackage expired date
-- Ticket: http://tp.voxteneo.co.id/entity/3045
-- Author: AZKA
-- Updated: 2.0 - 4/28/2016
-- =============================================

-- =============================================
-- Description: Edit All
-- Ticket: http://tp.voxteneo.co.id/entity/3045
-- Author: AZKA
-- Updated: 3.0 - 5/03/2016
-- =============================================

-- =============================================
-- Description: plant planning edit * stickperbox
-- Ticket: http://tp.voxteneo.co.id/entity/3045
-- Author: AZKA
-- Updated: 3.0 - 5/04/2016
-- =============================================

-- =============================================
-- Description: Value WH Eq from Total Actual Not Planning
-- Ticket: http://tp.voxteneo.co.id/entity/6451
-- Author: Yudha
-- Updated: 3.1 - 5/12/2016
-- =============================================

-- =============================================
-- Description: add column ParentLocationCode
-- Ticket: http://tp.voxteneo.co.id/entity/9988
-- Author: Hakim
-- Updated: 3.2 - 9/09/2016
-- =============================================

-- =============================================
-- Description: change planning to WPP
-- Ticket: http://tp.voxteneo.co.id/entity/11198
-- Author: Azka
-- Updated: 3.3 - 14/11/2016
-- =============================================

CREATE FUNCTION [dbo].[GetExeReportDailyProductionAchievement]
(
	@LocationCode as varchar(10),
	@KpsWeek int,
	@KpsYear int
)
RETURNS TABLE 
AS
RETURN
(
	with ProductionDaily as
		(
			select 
				SKTBrandCode, 
				BrandCode, 
				ABBR, 
				ParentLocationCode, 
				Package,
				StdStickPerHour,
				LocationCode,
				KPSWeek,
				KPSYear,
				COALESCE([Monday], 0) as Monday, 
				COALESCE([Tuesday], 0) as Tuesday, 
				COALESCE([Wednesday], 0) as Wednesday, 
				COALESCE([Thursday], 0) as Thursday, 
				COALESCE([Friday], 0) as Friday, 
				COALESCE([Saturday], 0) as Saturday, 
				COALESCE([Sunday], 0) as Sunday,
				(COALESCE([Monday], 0)+COALESCE([Tuesday], 0)+COALESCE([Wednesday], 0)+COALESCE([Thursday], 0)+COALESCE([Friday], 0)+COALESCE([Saturday], 0)+COALESCE([Sunday], 0)) as Total
			from 
			(
				select 
					bygrp.SKTBrandCode,
					bygrp.BrandCode,
					bygrp.ABBR,
					bygrp.ParentLocationCode,
					bygrp.NameOfDay,
					bygrp.Production,
					Package,
					StdStickPerHour,
					LocationCode,
					KPSWeek,
					KPSYear
				from
				(
					select 
						gbg.SKTBrandCode,
						procc.BrandCode,
						loc.ABBR,
						loc.ParentLocationCode,
						procc.LocationCode,
						procc.ProductionDate,
						Datename(weekday, procc.ProductionDate) as [NameOfDay],
						sum(procc.KeluarBersih) as Production,
						Coalesce(tpopkt.Package, 0) as Package,
						procsetview.StdStickPerHour,
						procc.KPSWeek,
						procc.KPSYear
					from ExeReportByProcess procc
					inner join MstGenBrand bg on bg.BrandCode = procc.BrandCode
					inner join MstGenBrandGroup gbg on gbg.BrandGroupCode = bg.BrandGroupCode
					inner join MstGenLocation loc on loc.LocationCode = procc.LocationCode
					left join MstTPOPackage tpopkt on tpopkt.LocationCode = procc.LocationCode and tpopkt.BrandGroupCode = bg.BrandGroupCode and procc.ProductionDate between tpopkt.EffectiveDate and tpopkt.ExpiredDate
					left join ProcessSettingsAndLocationView procsetview on procc.LocationCode = procsetview.LocationCode and bg.BrandGroupCode = procsetview.BrandGroupCode and procsetview.ProcessGroup = 'ROLLING' 
					where procc.LocationCode IN (select LocationCode from GetLastChildLocation(@LocationCode)) and procc.KPSWeek = @KpsWeek and procc.KPSYear = @KpsYear and (procc.ProcessGroup = 'STAMPING' or procc.ProcessGroup = 'WRAPPING') and procc.UOMOrder = 7
					group by
						gbg.SKTBrandCode,
						procc.BrandCode,
						procc.LocationCode,
						procc.ProductionDate,
						procc.BrandCode,
						loc.ABBR,
						loc.ParentLocationCode,
						Package,
						procsetview.StdStickPerHour,
						procc.KPSWeek,
						procc.KPSYear
				) bygrp


			)acv
			PIVOT
			(
				SUM(Production)
				FOR NameOfDay IN ([Monday], [Tuesday], [Wednesday], [Thursday], [Friday], [Saturday], [Sunday])
			) piv
		),
		PlanningDaily as
		(
					select 
						SKTBrandCode,
						BrandCode,
						ABBR,
						ParentLocationCode,
						LocationCode,
						sum(TPKValue) as TPKValue,
						WorkerCount
					from
					(
						---- Planning TPO
						select 
						gbg.SKTBrandCode,
						tpk.BrandCode,
						loc.ABBR,
						loc.ParentLocationCode,
						tpk.LocationCode,
						sum(tpk.TotalTargetManual * procsetview.StickPerBox) as TPKValue,
						0 as [WorkerCount],
						tpk.KPSWeek,
						tpk.KPSYear
						from PlanTPOTargetProductionKelompokBox tpk
						inner join MstGenBrand gb on gb.BrandCode = tpk.BrandCode
						inner join MstGenBrandGroup gbg on gbg.BrandGroupCode = gb.BrandGroupCode 
						inner join MstGenLocation loc on loc.LocationCode = tpk.LocationCode
						left join ProcessSettingsAndLocationView procsetview on tpk.LocationCode = procsetview.LocationCode and gbg.BrandGroupCode = procsetview.BrandGroupCode and procsetview.ProcessGroup = 'STAMPING' 
						where tpk.KPSWeek = @KpsWeek and tpk.KPSYear = @KpsYear and tpk.LocationCode IN (select LocationCode from GetLastChildLocation(@LocationCode))
						group by
						gbg.SKTBrandCode,
						tpk.BrandCode,
						loc.ABBR,
						loc.ParentLocationCode,
						tpk.LocationCode,
						tpk.KPSWeek,
						tpk.KPSYear

						union all
						---- Planning Plant
						select 
						gbg.SKTBrandCode,
						tpu.BrandCode,
						loc.ABBR,
						loc.ParentLocationCode,
						tpu.LocationCode,
						sum(totalTargetmanual * procsetview.StickPerBox) as TPKValue,
						sum(tpu.WorkerRegister) as WorkerCount,
						tpu.KPSWeek,
						tpu.KPSYear
						from PlanTargetProductionUnit tpu
						inner join MstGenBrand gb on gb.BrandCode = tpu.BrandCode
						inner join MstGenBrandGroup gbg on gbg.BrandGroupCode = gb.BrandGroupCode 
						inner join MstGenLocation loc on loc.LocationCode = tpu.LocationCode
						left join ProcessSettingsAndLocationView procsetview on tpu.LocationCode = procsetview.LocationCode and gbg.BrandGroupCode = procsetview.BrandGroupCode and procsetview.ProcessGroup = 'STAMPING' 
						where tpu.KPSWeek = @KpsWeek and tpu.KPSYear = @KpsYear and tpu.LocationCode IN (select LocationCode from GetLastChildLocation(@LocationCode))
						group by
						gbg.SKTBrandCode,
						tpu.BrandCode,
						loc.ABBR,
						loc.ParentLocationCode,
						tpu.LocationCode,
						tpu.KPSWeek,
						tpu.KPSYear
					) a
					group by
					SKTBrandCode,
						BrandCode,
						ABBR,
						ParentLocationCode,
						LocationCode,
						WorkerCount
		)
		SELECT 
			B.LocationCode,
			B.SKTBrandCode,
			B.BrandCode,
			B.ABBR,
			B.ParentLocationCode,
			B.Monday, B.Tuesday, B.Wednesday, B.Thursday, B.Friday, B.Saturday, B.Sunday,
			B.Monday + B.Tuesday + B.Wednesday + B.Thursday + B.Friday + B.Saturday + B.Sunday as Total,
			CAST((wpp.Value1 * 1000000) as FLOAT) as Planning, --P.TPKValue as Planning,
			--(B.Total - P.TPKValue) as VarianceStick,
			((B.Monday + B.Tuesday + B.Wednesday + B.Thursday + B.Friday + B.Saturday + B.Sunday)-(CAST((wpp.Value1 * 1000000) as FLOAT))) as VarianceStick,
			CAST(COALESCE(((B.Total - P.TPKValue) / NULLIF(P.TPKValue, 0)), 0)  * 100 AS Decimal(10,2)) as VariancePercent,
			CAST(COALESCE((B.Total / NULLIF(P.TPKValue, 0)), 0) * 100 AS Decimal(10,2)) as ReliabilityPercent,
			B.Package,
			CASE WHEN (select dbo.GetParentLocationLastChild(B.LocationCode)) = 'PLANT' THEN CAST(COALESCE((B.Monday + B.Tuesday + B.Wednesday + B.Thursday + B.Friday + B.Saturday + B.Sunday) / NULLIF((B.StdStickPerHour * P.WorkerCount), 0), 0) AS Decimal(10,2))
			ELSE CAST(COALESCE((B.Monday + B.Tuesday + B.Wednesday + B.Thursday + B.Friday + B.Saturday + B.Sunday) / NULLIF((B.Package * 360 * B.StdStickPerHour), 0), 0) AS Decimal(10,2)) END as TWHEqv
		FROM ProductionDaily B
		INNER JOIN PlanningDaily P on P.SKTBrandCode = B.SKTBrandCode and P.BrandCode = B.BrandCode and P.LocationCode = B.LocationCode and P.ABBR = B.ABBR and P.ParentLocationCode = B.ParentLocationCode
		INNER JOIN PlanWeeklyProductionPlanning wpp on wpp.LocationCode = B.LocationCode AND wpp.BrandCode = B.BrandCode AND wpp.KPSYear = @KpsYear AND wpp.KPSWeek = @KpsWeek
)
