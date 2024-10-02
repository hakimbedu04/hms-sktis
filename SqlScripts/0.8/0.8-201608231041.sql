/****** Object:  StoredProcedure [dbo].[ExtendedExeReportByProcess]    Script Date: 8/22/2016 4:39:22 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Opun Buds
-- Create date: 18-01-2016
-- Description:	For Packing process group
-- Edited by: Indra Permana
-- Description: Add brandcode, process, productiondate for parameter
-- =============================================

-- =============================================
-- Description: edit external move production, keluar bersih, sample 0
-- Ticket: http://tp.voxteneo.co.id/entity/6444
-- Author: Azka
-- Update: 11/05/2016
-- =============================================

-- =============================================
-- Description: edit external move EndingStock
-- Author: Bagus
-- Update: 12/05/2016
-- =============================================

-- =============================================
-- Description: fix external move EndingStock
-- Author: abud
-- Update: 25/05/2016
-- =============================================

-- =============================================
-- Description: Adding WRAPPING condition to param @ProcessGroup from BaseExeReportByProcess
-- Author: azka
-- Update: 23/08/2016
-- =============================================

ALTER PROCEDURE [dbo].[ExtendedExeReportByProcess]
	-- Add the parameters for the stored procedure here
	@LocationCode varchar(8),
	@BrandCode varchar(16),
	@CreatedBy varchar(50),
	@UpdatedBy varchar(50),
	@ProductionDate date,
	@ProcessGroup varchar(20)
AS
BEGIN
	IF(@ProcessGroup = 'PACKING')
	BEGIN
	-- INSERT A NEW ROW PACKING
	INSERT INTO dbo.ExeReportByProcess
	(
		LocationCode,
		UnitCode,
		BrandCode,
		KPSYear,
		KPSWeek,
		ProductionDate,
		ProcessGroup,
		ProcessOrder,
		Shift,
		Description,
		UOM,
		UOMOrder,
		Production,
		KeluarBersih,
		RejectSample,
		BeginningStock,
		EndingStock,
		CreatedDate,
		CreatedBy,
		UpdatedDate,
		UpdatedBy
	) SELECT
		max(e.LocationCode) as LocationCode,
		max(e.UnitCode) as UnitCode,
		max(e.BrandCode) as BrandCode,
		max(e.KPSYear) as KPSYear,
		max(e.KPSWeek) as KPSWeek,
		max(e.ProductionDate) as ProductionDate,
		max(e.ProcessGroup) as ProcessGroup,
		max(e.ProcessOrder) as ProcessOrder,
		max(e.Shift) as Shift,
		max(e.Description) as Description,
		('Pack') as Uom,
		(6) AS UOMOrder,
		(max(e.Production) / max(psalv.StickPerPack)) as Production,
		--max(e.KeluarBersih) as KeluarBersih,
		(max(e.KeluarBersih)/max(psalv.StickPerPack)) as KeluarBersih,
		max(e.RejectSample) as RejectSample,
		ISNULL((select TOP 1 EndingStock from ExeReportByProcess sub
			where 
				sub.ProductionDate = DATEADD(day, -1, @ProductionDate) 
				and sub.LocationCode = @LocationCode 
				and sub.BrandCode = @BrandCode 
				and sub.ProcessGroup = @ProcessGroup
				and sub.UnitCode = MAX(e.UnitCode)
				and sub.UOMOrder = 6
				--and KPSYear = SOURCE.KPSYear
				--and KPSWeek = SOURCE.KPSWeek
		),0) as BeginingStock,
		(max(e.EndingStock)/ max(psalv.StickPerPack)) as EndingStock,
		max(e.CreatedDate) as CreatedDate,
		max(e.CreatedBy) as CreatedBy,
		max(e.UpdatedDate) as UpdatedDate,
		max(e.UpdatedBy) as UpdatedBy
	FROM dbo.ExeReportByProcess e
	JOIN dbo.MstGenBrand AS mgb ON mgb.BrandCode = e.BrandCode
	JOIN dbo.ProcessSettingsAndLocationView AS psalv ON psalv.LocationCode = e.LocationCode AND psalv.BrandGroupCode = mgb.BrandGroupCode AND psalv.ProcessGroup = e.ProcessGroup
	WHERE e.ProcessGroup = 'PACKING' AND e.LocationCode = @LocationCode AND e.BrandCode = @BrandCode AND e.ProductionDate = @ProductionDate
	GROUP BY e.LocationCode,e.UnitCode,e.BrandCode,e.ProductionDate,e.ProcessGroup
	END
	ELSE
	BEGIN
	-- INSERT A NEW ROW STAMPING 2
	INSERT INTO dbo.ExeReportByProcess
	(
		LocationCode,
		UnitCode,
		BrandCode,
		KPSYear,
		KPSWeek,
		ProductionDate,
		ProcessGroup,
		ProcessOrder,
		Shift,
		Description,
		UOM,
		UOMOrder,
		Production,
		KeluarBersih,
		RejectSample,
		BeginningStock,
		EndingStock,
		CreatedDate,
		CreatedBy,
		UpdatedDate,
		UpdatedBy
	) SELECT
		max(e.LocationCode) as LocationCode,
		max(e.UnitCode) as UnitCode,
		max(e.BrandCode) as BrandCode,
		max(e.KPSYear) as KPSYear,
		max(e.KPSWeek) as KPSWeek,
		max(e.ProductionDate) as ProductionDate,
		max(e.ProcessGroup) as ProcessGroup,
		max(e.ProcessOrder) as ProcessOrder,
		max(e.Shift) as Shift,
		max(e.Description) as Description,
		('Pack') as Uom,
		(8) AS UOMOrder,
		(max(e.Production) / max(psalv.StickPerPack)) as Production,
		--max(e.KeluarBersih) as KeluarBersih,
		(max(e.Production)/max(psalv.StickPerPack)) as KeluarBersih,
		max(e.RejectSample) as RejectSample,
		ISNULL((select TOP 1 EndingStock from ExeReportByProcess xy
			where 
				xy.ProductionDate = DATEADD(day, -1, @ProductionDate) 
				and xy.LocationCode = @LocationCode 
				and xy.BrandCode = @BrandCode 
				and xy.ProcessGroup = @ProcessGroup
				and xy.UnitCode = MAX(e.UnitCode)
				and xy.UOMOrder = 8
				--and KPSYear = SOURCE.KPSYear
				--and KPSWeek = SOURCE.KPSWeek
		),0) as BeginingStock,
		ISNULL((
		((select TOP 1 EndingStock from ExeReportByProcess xy
			where 
				xy.ProductionDate = DATEADD(day, -1, @ProductionDate) 
				and xy.LocationCode = @LocationCode 
				and xy.BrandCode = @BrandCode 
				and xy.ProcessGroup = @ProcessGroup
				and xy.UnitCode = MAX(e.UnitCode)
				and xy.UOMOrder = 8
				--and KPSYear = SOURCE.KPSYear
				--and KPSWeek = SOURCE.KPSWeek
		) +
		(max(e.Production) / max(psalv.StickPerPack))
		) - (max(e.Production)/max(psalv.StickPerPack)) - max(RejectSample)
		),0) as EndingStock,
		max(e.CreatedDate) as CreatedDate,
		max(e.CreatedBy) as CreatedBy,
		max(e.UpdatedDate) as UpdatedDate,
		max(e.UpdatedBy) as UpdatedBy
	FROM dbo.ExeReportByProcess AS e
	JOIN dbo.MstGenBrand AS mgb ON mgb.BrandCode = e.BrandCode
	JOIN dbo.ProcessSettingsAndLocationView AS psalv ON psalv.LocationCode = e.LocationCode AND psalv.BrandGroupCode = mgb.BrandGroupCode AND psalv.ProcessGroup = e.ProcessGroup
	WHERE e.ProcessGroup = @ProcessGroup AND e.LocationCode = @LocationCode AND e.BrandCode = @BrandCode AND e.ProductionDate = @ProductionDate
	GROUP BY e.LocationCode,e.UnitCode,e.BrandCode,e.ProductionDate,e.ProcessGroup

	-- INSERT A NEW ROW STAMPING 2
	INSERT INTO dbo.ExeReportByProcess
	(
		LocationCode,
		UnitCode,
		BrandCode,
		KPSYear,
		KPSWeek,
		ProductionDate,
		ProcessGroup,
		ProcessOrder,
		Shift,
		Description,
		UOM,
		UOMOrder,
		Production,
		KeluarBersih,
		RejectSample,
		BeginningStock,
		EndingStock,
		CreatedDate,
		CreatedBy,
		UpdatedDate,
		UpdatedBy
	) SELECT
		max(LocationCode) as LocationCode,
		max(UnitCode) as UnitCode,
		max(BrandCode) as BrandCode,
		max(KPSYear) as KPSYear,
		max(KPSWeek) as KPSWeek,
		max(ProductionDate) as ProductionDate,
		max(ProcessGroup) as ProcessGroup,
		max(ProcessOrder) as ProcessOrder,
		max(Shift) as Shift,
		('DisplayCarton') as Description,
		max(UOM) as Uom,
		(9) AS UOMOrder,
		max(Production) as Production,
		max(KeluarBersih) as KeluarBersih,
		max(RejectSample) as RejectSample,
		ISNULL((select TOP 1 EndingStock from ExeReportByProcess xy
			where 
				xy.ProductionDate = DATEADD(day, -1, @ProductionDate) 
				and xy.LocationCode = @LocationCode 
				and xy.BrandCode = @BrandCode 
				and xy.ProcessGroup = @ProcessGroup
				and xy.UnitCode = MAX(e.UnitCode)
				and xy.UOMOrder = 9
				--and KPSYear = SOURCE.KPSYear
				--and KPSWeek = SOURCE.KPSWeek
		),0) as BeginingStock,
		ISNULL(((select TOP 1 EndingStock from ExeReportByProcess xy
			where 
				xy.ProductionDate = DATEADD(day, -1, @ProductionDate) 
				and xy.LocationCode = @LocationCode 
				and xy.BrandCode = @BrandCode 
				and xy.ProcessGroup = @ProcessGroup
				and xy.UnitCode = MAX(e.UnitCode)
				and xy.UOMOrder = 9
				--and KPSYear = SOURCE.KPSYear
				--and KPSWeek = SOURCE.KPSWeek
		) + max(Production) - max(KeluarBersih) - max(RejectSample)),0) as EndingStock,
		max(CreatedDate) as CreatedDate,
		max(CreatedBy) as CreatedBy,
		max(UpdatedDate) as UpdatedDate,
		max(UpdatedBy) as UpdatedBy
	FROM dbo.ExeReportByProcess e WHERE e.ProcessGroup = @ProcessGroup AND e.LocationCode = @LocationCode AND e.BrandCode = @BrandCode AND e.ProductionDate = @ProductionDate
	GROUP BY e.LocationCode,e.UnitCode,e.BrandCode,e.ProductionDate,e.ProcessGroup

	-- INSERT A NEW ROW STAMPING 3
	INSERT INTO dbo.ExeReportByProcess
	(
		LocationCode,
		UnitCode,
		BrandCode,
		KPSYear,
		KPSWeek,
		ProductionDate,
		ProcessGroup,
		ProcessOrder,
		Shift,
		Description,
		UOM,
		UOMOrder,
		Production,
		KeluarBersih,
		RejectSample,
		BeginningStock,
		EndingStock,
		CreatedDate,
		CreatedBy,
		UpdatedDate,
		UpdatedBy
	) SELECT
		max(e.LocationCode) as LocationCode,
		max(e.UnitCode) as UnitCode,
		max(e.BrandCode) as BrandCode,
		max(e.KPSYear) as KPSYear,
		max(e.KPSWeek) as KPSWeek,
		max(e.ProductionDate) as ProductionDate,
		max(e.ProcessGroup) as ProcessGroup,
		max(e.ProcessOrder) as ProcessOrder,
		max(e.Shift) as Shift,
		('DisplayCarton') as Description,
		('Slof') as Uom,
		(10) AS UOMOrder,
		(max(e.Production) / max(psalv.StickPerSlof)) as Production,
		--max(e.KeluarBersih) as KeluarBersih,
		(max(e.Production)/max(psalv.StickPerSlof)) as KeluarBersih,
		max(e.RejectSample) as RejectSample,
		ISNULL((select TOP 1 EndingStock from ExeReportByProcess xy
			where 
				xy.ProductionDate = DATEADD(day, -1, @ProductionDate) 
				and xy.LocationCode = @LocationCode 
				and xy.BrandCode = @BrandCode 
				and xy.ProcessGroup = @ProcessGroup
				and xy.UnitCode = MAX(e.UnitCode)
				and xy.UOMOrder = 10
				--and KPSYear = SOURCE.KPSYear
				--and KPSWeek = SOURCE.KPSWeek
		),0) as BeginingStock,		
		ISNULL((
		(select TOP 1 EndingStock from ExeReportByProcess xy
			where 
				xy.ProductionDate = DATEADD(day, -1, @ProductionDate) 
				and xy.LocationCode = @LocationCode 
				and xy.BrandCode = @BrandCode 
				and xy.ProcessGroup = @ProcessGroup
				and xy.UnitCode = MAX(e.UnitCode)
				and xy.UOMOrder = 10
				--and KPSYear = SOURCE.KPSYear
				--and KPSWeek = SOURCE.KPSWeek
		) + (max(e.Production) / max(psalv.StickPerSlof)) - (max(e.Production)/max(psalv.StickPerSlof)) - max(RejectSample)
		),0) as EndingStock,
		max(e.CreatedDate) as CreatedDate,
		max(e.CreatedBy) as CreatedBy,
		max(e.UpdatedDate) as UpdatedDate,
		max(e.UpdatedBy) as UpdatedBy
	FROM dbo.ExeReportByProcess e
	JOIN dbo.MstGenBrand AS mgb ON mgb.BrandCode = e.BrandCode
	JOIN dbo.ProcessSettingsAndLocationView AS psalv ON psalv.LocationCode = e.LocationCode AND psalv.BrandGroupCode = mgb.BrandGroupCode AND psalv.ProcessGroup = e.ProcessGroup
	WHERE e.ProcessGroup = @ProcessGroup AND e.LocationCode = @LocationCode AND e.BrandCode = @BrandCode AND ProductionDate = @ProductionDate
	GROUP BY e.LocationCode,e.UnitCode,e.BrandCode,e.ProductionDate,e.ProcessGroup

	-- INSERT A NEW ROW STAMPING 4
	INSERT INTO dbo.ExeReportByProcess
	(
		LocationCode,
		UnitCode,
		BrandCode,
		KPSYear,
		KPSWeek,
		ProductionDate,
		ProcessGroup,
		ProcessOrder,
		Shift,
		Description,
		UOM,
		UOMOrder,
		Production,
		KeluarBersih,
		RejectSample,
		BeginningStock,
		EndingStock,
		CreatedDate,
		CreatedBy,
		UpdatedDate,
		UpdatedBy
	) SELECT
		max(LocationCode) as LocationCode,
		max(UnitCode) as UnitCode,
		max(BrandCode) as BrandCode,
		max(KPSYear) as KPSYear,
		max(KPSWeek) as KPSWeek,
		max(ProductionDate) as ProductionDate,
		max(ProcessGroup) as ProcessGroup,
		max(ProcessOrder) as ProcessOrder,
		max(Shift) as Shift,
		('InternalMove') as Description,
		('Stick') as Uom,
		(11) AS UOMOrder,
		max(Production) as Production,
		max(KeluarBersih) as KeluarBersih,
		max(RejectSample) as RejectSample,
		ISNULL((select TOP 1 EndingStock from ExeReportByProcess xy
			where 
				xy.ProductionDate = DATEADD(day, -1, @ProductionDate) 
				and xy.LocationCode = @LocationCode 
				and xy.BrandCode = @BrandCode 
				and xy.ProcessGroup = @ProcessGroup
				and xy.UnitCode = MAX(e.UnitCode)
				and xy.UOMOrder = 11
				--and KPSYear = SOURCE.KPSYear
				--and KPSWeek = SOURCE.KPSWeek
		),0) as BeginingStock,
		ISNULL(((select TOP 1 EndingStock from ExeReportByProcess xy
			where 
				xy.ProductionDate = DATEADD(day, -1, @ProductionDate) 
				and xy.LocationCode = @LocationCode 
				and xy.BrandCode = @BrandCode 
				and xy.ProcessGroup = @ProcessGroup
				and xy.UnitCode = MAX(e.UnitCode)
				and xy.UOMOrder = 11
				--and KPSYear = SOURCE.KPSYear
				--and KPSWeek = SOURCE.KPSWeek
		) + max(Production) - max(KeluarBersih) - max(RejectSample)),0) as EndingStock,
		max(CreatedDate) as CreatedDate,
		max(CreatedBy) as CreatedBy,
		max(UpdatedDate) as UpdatedDate,
		max(UpdatedBy) as UpdatedBy
	FROM dbo.ExeReportByProcess e WHERE e.ProcessGroup = @ProcessGroup AND e.LocationCode = @LocationCode AND e.BrandCode = @BrandCode AND e.ProductionDate = @ProductionDate
	GROUP BY e.LocationCode,e.UnitCode,e.BrandCode,e.ProductionDate,e.ProcessGroup

	-- INSERT A NEW ROW STAMPING 5
	INSERT INTO dbo.ExeReportByProcess
	(
		LocationCode,
		UnitCode,
		BrandCode,
		KPSYear,
		KPSWeek,
		ProductionDate,
		ProcessGroup,
		ProcessOrder,
		Shift,
		Description,
		UOM,
		UOMOrder,
		Production,
		KeluarBersih,
		RejectSample,
		BeginningStock,
		EndingStock,
		CreatedDate,
		CreatedBy,
		UpdatedDate,
		UpdatedBy
	) SELECT
		max(e.LocationCode) as LocationCode,
		max(e.UnitCode) as UnitCode,
		max(e.BrandCode) as BrandCode,
		max(e.KPSYear) as KPSYear,
		max(e.KPSWeek) as KPSWeek,
		max(e.ProductionDate) as ProductionDate,
		max(e.ProcessGroup) as ProcessGroup,
		max(e.ProcessOrder) as ProcessOrder,
		max(e.Shift) as Shift,
		('InternalMove') as Description,
		('Box') as Uom,
		(12) AS UOMOrder,
		(max(e.Production) / max(psalv.StickPerBox)) as Production,
		--max(e.KeluarBersih) as KeluarBersih,
		(max(e.Production)/max(psalv.StickPerBox)) as KeluarBersih,
		max(e.RejectSample) as RejectSample,
		ISNULL((select TOP 1 EndingStock from ExeReportByProcess xy
			where 
				xy.ProductionDate = DATEADD(day, -1, @ProductionDate) 
				and xy.LocationCode = @LocationCode 
				and xy.BrandCode = @BrandCode 
				and xy.ProcessGroup = @ProcessGroup
				and xy.UnitCode = MAX(e.UnitCode)
				and xy.UOMOrder = 12
				--and KPSYear = SOURCE.KPSYear
				--and KPSWeek = SOURCE.KPSWeek
		),0) as BeginingStock,
		ISNULL(((select TOP 1 EndingStock from ExeReportByProcess xy
			where 
				xy.ProductionDate = DATEADD(day, -1, @ProductionDate) 
				and xy.LocationCode = @LocationCode 
				and xy.BrandCode = @BrandCode 
				and xy.ProcessGroup = @ProcessGroup
				and xy.UnitCode = MAX(e.UnitCode)
				and xy.UOMOrder = 12
				--and KPSYear = SOURCE.KPSYear
				--and KPSWeek = SOURCE.KPSWeek
		) + (max(e.Production) / max(psalv.StickPerBox)) - (max(e.Production)/max(psalv.StickPerBox)) - max(RejectSample)),0) as EndingStock,
		max(e.CreatedDate) as CreatedDate,
		max(e.CreatedBy) as CreatedBy,
		max(e.UpdatedDate) as UpdatedDate,
		max(e.UpdatedBy) as UpdatedBy
	FROM dbo.ExeReportByProcess AS e 
	JOIN dbo.MstGenBrand AS mgb ON mgb.BrandCode = e.BrandCode
	JOIN dbo.ProcessSettingsAndLocationView AS psalv ON psalv.LocationCode = e.LocationCode AND psalv.BrandGroupCode = mgb.BrandGroupCode AND psalv.ProcessGroup = e.ProcessGroup
	WHERE e.ProcessGroup = @ProcessGroup AND e.LocationCode = @LocationCode AND e.BrandCode = @BrandCode AND e.ProductionDate = @ProductionDate
	GROUP BY e.LocationCode,e.UnitCode,e.BrandCode,e.ProductionDate,e.ProcessGroup

	-- INSERT A NEW ROW STAMPING 6
	INSERT INTO dbo.ExeReportByProcess
	(
		LocationCode,
		UnitCode,
		BrandCode,
		KPSYear,
		KPSWeek,
		ProductionDate,
		ProcessGroup,
		ProcessOrder,
		Shift,
		Description,
		UOM,
		UOMOrder,
		Production,
		KeluarBersih,
		RejectSample,
		BeginningStock,
		EndingStock,
		CreatedDate,
		CreatedBy,
		UpdatedDate,
		UpdatedBy
	) SELECT
		max(LocationCode) as LocationCode,
		max(UnitCode) as UnitCode,
		max(BrandCode) as BrandCode,
		max(KPSYear) as KPSYear,
		max(KPSWeek) as KPSWeek,
		max(ProductionDate) as ProductionDate,
		max(ProcessGroup) as ProcessGroup,
		max(ProcessOrder) as ProcessOrder,
		max(Shift) as Shift,
		('ExternalMove')as Description,
		max(UOM) as Uom,
		(13) AS UOMOrder,
		0 as Production,
		0 as KeluarBersih,
		0 as RejectSample,
		ISNULL((select TOP 1 EndingStock from ExeReportByProcess xy
			where 
				xy.ProductionDate = DATEADD(day, -1, @ProductionDate) 
				and xy.LocationCode = @LocationCode 
				and xy.BrandCode = @BrandCode 
				and xy.ProcessGroup = @ProcessGroup
				and xy.UnitCode = MAX(e.UnitCode)
				and xy.UOMOrder = 13
				--and KPSYear = SOURCE.KPSYear
				--and KPSWeek = SOURCE.KPSWeek
		),0) as BeginingStock,
		(
		ISNULL((select TOP 1 EndingStock from ExeReportByProcess xy
			where 
				xy.ProductionDate = DATEADD(day, -1, @ProductionDate) 
				and xy.LocationCode = @LocationCode 
				and xy.BrandCode = @BrandCode 
				and xy.ProcessGroup = @ProcessGroup
				and xy.UnitCode = MAX(e.UnitCode)
				and xy.UOMOrder = 13
				--and KPSYear = SOURCE.KPSYear
				--and KPSWeek = SOURCE.KPSWeek
		),0)
		+
		(SELECT TOP 1 sub.KeluarBersih
		FROM dbo.ExeReportByProcess  AS sub 
		WHERE 
		sub.ProcessGroup = @ProcessGroup AND sub.LocationCode = @LocationCode AND sub.BrandCode = @BrandCode AND sub.ProductionDate = @ProductionDate AND sub.UOMOrder = 11
		and sub.UnitCode = UnitCode
		)
		-
		max(RejectSample)
		---
		--max(KeluarBersih)
		) 
		as EndingStock,
		max(CreatedDate) as CreatedDate,
		max(CreatedBy) as CreatedBy,
		max(UpdatedDate) as UpdatedDate,
		max(UpdatedBy) as UpdatedBy
	FROM dbo.ExeReportByProcess e WHERE e.ProcessGroup = @ProcessGroup AND e.LocationCode = @LocationCode AND e.BrandCode = @BrandCode AND e.ProductionDate = @ProductionDate
	GROUP BY e.LocationCode,e.UnitCode,e.BrandCode,e.ProductionDate,e.ProcessGroup

	-- INSERT A NEW ROW STAMPING 7
	INSERT INTO dbo.ExeReportByProcess
	(
		LocationCode,
		UnitCode,
		BrandCode,
		KPSYear,
		KPSWeek,
		ProductionDate,
		ProcessGroup,
		ProcessOrder,
		Shift,
		Description,
		UOM,
		UOMOrder,
		Production,
		KeluarBersih,
		RejectSample,
		BeginningStock,
		EndingStock,
		CreatedDate,
		CreatedBy,
		UpdatedDate,
		UpdatedBy
	) SELECT
		max(e.LocationCode) as LocationCode,
		max(e.UnitCode) as UnitCode,
		max(e.BrandCode) as BrandCode,
		max(e.KPSYear) as KPSYear,
		max(e.KPSWeek) as KPSWeek,
		max(e.ProductionDate) as ProductionDate,
		max(e.ProcessGroup) as ProcessGroup,
		max(e.ProcessOrder) as ProcessOrder,
		max(e.Shift) as Shift,
		('ExternalMove') as Description,
		('Box') as Uom,
		(14) AS UOMOrder,
		0 as Production,
		--max(e.KeluarBersih) as KeluarBersih,
		0 as KeluarBersih,
		0 as RejectSample,
		ISNULL((select TOP 1 EndingStock from ExeReportByProcess xy
			where 
				xy.ProductionDate = DATEADD(day, -1, @ProductionDate) 
				and xy.LocationCode = @LocationCode 
				and xy.BrandCode = @BrandCode 
				and xy.ProcessGroup = @ProcessGroup
				and xy.UnitCode = MAX(e.UnitCode)
				and xy.UOMOrder = 14
				--and KPSYear = SOURCE.KPSYear
				--and KPSWeek = SOURCE.KPSWeek
		),0) as BeginingStock,
		(
		ISNULL((select TOP 1 EndingStock from ExeReportByProcess xy
			where 
				xy.ProductionDate = DATEADD(day, -1, @ProductionDate) 
				and xy.LocationCode = @LocationCode 
				and xy.BrandCode = @BrandCode 
				and xy.ProcessGroup = @ProcessGroup
				and xy.UnitCode = MAX(e.UnitCode)
				and xy.UOMOrder = 14
				--and KPSYear = SOURCE.KPSYear
				--and KPSWeek = SOURCE.KPSWeek
		),0)
		+
		(
		SELECT TOP 1 (sub.KeluarBersih) as KeluarBersih
		FROM dbo.ExeReportByProcess  AS sub 
		WHERE 
		sub.ProcessGroup = @ProcessGroup AND sub.LocationCode = @LocationCode AND sub.BrandCode = @BrandCode AND sub.ProductionDate = @ProductionDate AND sub.UOMOrder = 12
		and sub.UnitCode = UnitCode
		)
		-
		MAX(RejectSample)
		---
		--max(KeluarBersih)
		) as EndingStock,
		max(e.CreatedDate) as CreatedDate,
		max(e.CreatedBy) as CreatedBy,
		max(e.UpdatedDate) as UpdatedDate,
		max(e.UpdatedBy) as UpdatedBy
	FROM dbo.ExeReportByProcess AS e 
	JOIN dbo.MstGenBrand AS mgb ON mgb.BrandCode = e.BrandCode
	JOIN dbo.ProcessSettingsAndLocationView AS psalv ON psalv.LocationCode = e.LocationCode AND psalv.BrandGroupCode = mgb.BrandGroupCode AND psalv.ProcessGroup = e.ProcessGroup
	WHERE e.ProcessGroup = @ProcessGroup AND e.LocationCode = @LocationCode AND e.BrandCode = @BrandCode AND e.ProductionDate = @ProductionDate
	GROUP BY e.LocationCode,e.UnitCode,e.BrandCode,e.ProductionDate,e.ProcessGroup
	END

END