/****** Object:  StoredProcedure [dbo].[ExtendedExeReportByProcess]    Script Date: 12/21/2016 2:28:37 PM ******/
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

-- =============================================
-- Description: add rounding on beginning stock, production, endingstock, 
-- Author: azka
-- Update: 14/10/2016
-- ticket: http://tp.voxteneo.co.id/entity/10449
-- =============================================

-- =============================================
-- Description: add param unitcode and fix query where (adding unitcode)
-- Author: azka
-- Update: 01/11/2016
-- ticket: http://tp.voxteneo.co.id/entity/10794
-- =============================================

ALTER PROCEDURE [dbo].[ExtendedExeReportByProcess]
	-- Add the parameters for the stored procedure here
	@LocationCode varchar(8),
	@BrandCode varchar(16),
	@CreatedBy varchar(50),
	@UpdatedBy varchar(50),
	@ProductionDate date,
	@ProcessGroup varchar(20),
	@UnitCode varchar(10)
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
		ROUND((max(e.Production) / max(mgbg.StickPerPack)), 2) as Production,
		--max(e.KeluarBersih) as KeluarBersih,
		ROUND((max(e.KeluarBersih) / max(mgbg.StickPerPack)), 2) as KeluarBersih,
		max(e.RejectSample) as RejectSample,
		ROUND(ISNULL((select TOP 1 EndingStock from ExeReportByProcess sub
			where 
				sub.ProductionDate = DATEADD(day, -1, @ProductionDate) 
				and sub.LocationCode = @LocationCode 
				and sub.BrandCode = @BrandCode 
				and sub.ProcessGroup = @ProcessGroup
				and sub.UnitCode = MAX(e.UnitCode)
				and sub.UOMOrder = 6
				--and KPSYear = SOURCE.KPSYear
				--and KPSWeek = SOURCE.KPSWeek
		),0), 2) as BeginingStock,
		ROUND((max(e.EndingStock)/ max(mgbg.StickPerPack)), 2) as EndingStock,
		max(e.CreatedDate) as CreatedDate,
		max(e.CreatedBy) as CreatedBy,
		max(e.UpdatedDate) as UpdatedDate,
		max(e.UpdatedBy) as UpdatedBy
	FROM dbo.ExeReportByProcess e
	JOIN dbo.MstGenBrand AS mgb ON mgb.BrandCode = e.BrandCode
	JOIN dbo.MstGenBrandGroup AS mgbg ON mgbg.BrandGroupCode = mgb.BrandGroupCode
	WHERE e.ProcessGroup = @ProcessGroup AND e.LocationCode = @LocationCode AND e.UnitCode = @UnitCode AND e.BrandCode = @BrandCode AND e.ProductionDate = @ProductionDate
	GROUP BY e.LocationCode,e.UnitCode,e.BrandCode,e.ProductionDate,e.ProcessGroup
	
	RETURN;
	END
--Luki Edited Start
	ELSE 
	BEGIN
	IF (@ProcessGroup = 'WRAPPING')
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
		ROUND((max(e.Production) / max(mgbg.StickPerPack)), 2) as Production,
		--max(e.KeluarBersih) as KeluarBersih,
		ROUND((max(e.KeluarBersih)/max(mgbg.StickPerPack)), 2) as KeluarBersih,
		0 as RejectSample,
		ROUND(ISNULL((select TOP 1 EndingStock from ExeReportByProcess sub
			where 
				sub.ProductionDate = DATEADD(day, -1, @ProductionDate) 
				and sub.LocationCode = @LocationCode 
				and sub.BrandCode = @BrandCode 
				and sub.ProcessGroup = @ProcessGroup
				and sub.UnitCode = MAX(e.UnitCode)
				and sub.UOMOrder = 6
				--and KPSYear = SOURCE.KPSYear
				--and KPSWeek = SOURCE.KPSWeek
		),0), 2) as BeginingStock,
		ROUND((max(e.EndingStock)/ max(mgbg.StickPerPack)), 2) as EndingStock,
		max(e.CreatedDate) as CreatedDate,
		max(e.CreatedBy) as CreatedBy,
		max(e.UpdatedDate) as UpdatedDate,
		max(e.UpdatedBy) as UpdatedBy
	FROM dbo.ExeReportByProcess e
	JOIN dbo.MstGenBrand AS mgb ON mgb.BrandCode = e.BrandCode
	JOIN dbo.MstGenBrandGroup AS mgbg ON mgbg.BrandGroupCode = mgb.BrandGroupCode
	WHERE e.ProcessGroup = @ProcessGroup AND e.LocationCode = @LocationCode AND e.UnitCode = @UnitCode AND e.BrandCode = @BrandCode AND e.ProductionDate = @ProductionDate
	GROUP BY e.LocationCode,e.UnitCode,e.BrandCode,e.ProductionDate,e.ProcessGroup
	END
--Luki Edited End
	-- INSERT A NEW ROW STAMPING 2

	DECLARE @rejectSampleUOM7 INT;
	DECLARE @keluarBersihUOM7 FLOAT;
	select TOP 1 @rejectSampleUOM7 = RejectSample, @keluarBersihUOM7 = KeluarBersih from ExeReportByProcess e 
	WHERE e.ProcessGroup = @ProcessGroup AND e.LocationCode = @LocationCode   AND e.UnitCode = @UnitCode AND e.BrandCode = @BrandCode AND e.ProductionDate =  @ProductionDate
	and UOMOrder = 7

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
		'StampPack' as Description,
		('Pack') as Uom,
		(8) AS UOMOrder,
		ROUND((max(e.Production) / max(mgbg.StickPerPack)), 2) as Production,
		--max(e.KeluarBersih) as KeluarBersih,
		ROUND((max(e.Production) / max(mgbg.StickPerPack)) - (ISNULL(@rejectSampleUOM7, 0) / mgbg.StickPerPack), 2) as KeluarBersih,
		ROUND((ISNULL(@rejectSampleUOM7, 0) / mgbg.StickPerPack), 2) as RejectSample,
		ROUND(ISNULL((select TOP 1 EndingStock from ExeReportByProcess xy
			where 
				xy.ProductionDate = DATEADD(day, -1, @ProductionDate) 
				and xy.LocationCode = @LocationCode 
				and xy.BrandCode = @BrandCode 
				and xy.ProcessGroup = @ProcessGroup
				and xy.UnitCode = MAX(e.UnitCode)
				and xy.UOMOrder = 8
				--and KPSYear = SOURCE.KPSYear
				--and KPSWeek = SOURCE.KPSWeek
		),0), 2) as BeginingStock,
		(max(e.Production) / max(mgbg.StickPerPack)) - (max(e.Production) / max(mgbg.StickPerPack)) - (@rejectSampleUOM7 / mgbg.StickPerPack) + (@rejectSampleUOM7 / mgbg.StickPerPack) as EndingStock,
		max(e.CreatedDate) as CreatedDate,
		max(e.CreatedBy) as CreatedBy,
		max(e.UpdatedDate) as UpdatedDate,
		max(e.UpdatedBy) as UpdatedBy
	FROM dbo.ExeReportByProcess AS e
	JOIN dbo.MstGenBrand AS mgb ON mgb.BrandCode = e.BrandCode
	JOIN dbo.MstGenBrandGroup AS mgbg ON mgbg.BrandGroupCode = mgb.BrandGroupCode
	WHERE e.ProcessGroup = @ProcessGroup AND e.LocationCode = @LocationCode AND e.UnitCode = @UnitCode AND e.BrandCode = @BrandCode AND e.ProductionDate = @ProductionDate
	GROUP BY e.LocationCode,e.UnitCode,e.BrandCode,e.ProductionDate,e.ProcessGroup,mgbg.StickPerPack

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
		ROUND(ISNULL(@keluarBersihUOM7, 0), 2) as Production,
		ROUND(max(ISNULL(CASE WHEN @ProcessGroup = 'WRAPPING' THEN @keluarBersihUOM7 ELSE KeluarBersih END, 0)), 2) as KeluarBersih,
		0 as RejectSample,
		ROUND(ISNULL((select TOP 1 EndingStock from ExeReportByProcess xy
			where 
				xy.ProductionDate = DATEADD(day, -1, @ProductionDate) 
				and xy.LocationCode = @LocationCode 
				and xy.BrandCode = @BrandCode 
				and xy.ProcessGroup = @ProcessGroup
				and xy.UnitCode = MAX(e.UnitCode)
				and xy.UOMOrder = 9
				--and KPSYear = SOURCE.KPSYear
				--and KPSWeek = SOURCE.KPSWeek
		),0), 2) as BeginingStock,
		CASE WHEN @ProcessGroup = 'WRAPPING' THEN 0 ELSE ROUND(ISNULL(((select TOP 1 EndingStock from ExeReportByProcess xy
			where 
				xy.ProductionDate = DATEADD(day, -1, @ProductionDate) 
				and xy.LocationCode = @LocationCode 
				and xy.BrandCode = @BrandCode 
				and xy.ProcessGroup = @ProcessGroup
				and xy.UnitCode = MAX(e.UnitCode)
				and xy.UOMOrder = 9
				--and KPSYear = SOURCE.KPSYear
				--and KPSWeek = SOURCE.KPSWeek
		) + max(Production) - max(KeluarBersih) - max(RejectSample)),0), 2) END as EndingStock,
		max(CreatedDate) as CreatedDate,
		max(CreatedBy) as CreatedBy,
		max(UpdatedDate) as UpdatedDate,
		max(UpdatedBy) as UpdatedBy
	FROM dbo.ExeReportByProcess e WHERE e.ProcessGroup = @ProcessGroup AND e.LocationCode = @LocationCode AND e.UnitCode = @UnitCode AND e.BrandCode = @BrandCode AND e.ProductionDate = @ProductionDate
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
		ROUND(ISNULL(@keluarBersihUOM7, 0) / max(mgbg.StickPerSlof), 2) as Production,
		--max(e.KeluarBersih) as KeluarBersih,
		ROUND(ISNULL(@keluarBersihUOM7, 0) /max(mgbg.StickPerSlof), 2) as KeluarBersih,
		0 as RejectSample,
		ROUND(ISNULL((select TOP 1 EndingStock from ExeReportByProcess xy
			where 
				xy.ProductionDate = DATEADD(day, -1, @ProductionDate) 
				and xy.LocationCode = @LocationCode 
				and xy.BrandCode = @BrandCode 
				and xy.ProcessGroup = @ProcessGroup
				and xy.UnitCode = MAX(e.UnitCode)
				and xy.UOMOrder = 10
				--and KPSYear = SOURCE.KPSYear
				--and KPSWeek = SOURCE.KPSWeek
		),0), 2) as BeginingStock,		
		@keluarBersihUOM7 / max(mgbg.StickPerSlof) - @keluarBersihUOM7/max(mgbg.StickPerSlof) as EndingStock,
		max(e.CreatedDate) as CreatedDate,
		max(e.CreatedBy) as CreatedBy,
		max(e.UpdatedDate) as UpdatedDate,
		max(e.UpdatedBy) as UpdatedBy
	FROM dbo.ExeReportByProcess e
	JOIN dbo.MstGenBrand AS mgb ON mgb.BrandCode = e.BrandCode
	JOIN dbo.MstGenBrandGroup AS mgbg ON mgbg.BrandGroupCode = mgb.BrandGroupCode
	WHERE e.ProcessGroup = @ProcessGroup AND e.LocationCode = @LocationCode AND e.UnitCode = @UnitCode AND e.BrandCode = @BrandCode AND ProductionDate = @ProductionDate
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
		ISNULL(@keluarBersihUOM7, 0) as Production,
		ISNULL(@keluarBersihUOM7, 0) as KeluarBersih,
		0 as RejectSample,
		ROUND(ISNULL((select TOP 1 EndingStock from ExeReportByProcess xy
			where 
				xy.ProductionDate = DATEADD(day, -1, @ProductionDate) 
				and xy.LocationCode = @LocationCode 
				and xy.BrandCode = @BrandCode 
				and xy.ProcessGroup = @ProcessGroup
				and xy.UnitCode = MAX(e.UnitCode)
				and xy.UOMOrder = 11
				--and KPSYear = SOURCE.KPSYear
				--and KPSWeek = SOURCE.KPSWeek
		),0), 2) as BeginingStock,
		CASE WHEN @ProcessGroup = 'WRAPPING' THEN 0 ELSE ROUND(ISNULL(((select TOP 1 EndingStock from ExeReportByProcess xy
			where 
				xy.ProductionDate = DATEADD(day, -1, @ProductionDate) 
				and xy.LocationCode = @LocationCode 
				and xy.BrandCode = @BrandCode 
				and xy.ProcessGroup = @ProcessGroup
				and xy.UnitCode = MAX(e.UnitCode)
				and xy.UOMOrder = 11
				--and KPSYear = SOURCE.KPSYear
				--and KPSWeek = SOURCE.KPSWeek
		) + max(Production) - max(KeluarBersih) - max(RejectSample)),0), 2) END as EndingStock,
		max(CreatedDate) as CreatedDate,
		max(CreatedBy) as CreatedBy,
		max(UpdatedDate) as UpdatedDate,
		max(UpdatedBy) as UpdatedBy
	FROM dbo.ExeReportByProcess e WHERE e.ProcessGroup = @ProcessGroup AND e.LocationCode = @LocationCode AND e.UnitCode = @UnitCode AND e.BrandCode = @BrandCode AND e.ProductionDate = @ProductionDate
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
		ROUND((ISNULL(@keluarBersihUOM7, 0) / max(mgbg.StickPerBox)), 2) as Production,
		--max(e.KeluarBersih) as KeluarBersih,
		ROUND((ISNULL(@keluarBersihUOM7, 0) /max(mgbg.StickPerBox)), 2) as KeluarBersih,
		0 as RejectSample,
		ROUND(ISNULL((select TOP 1 EndingStock from ExeReportByProcess xy
			where 
				xy.ProductionDate = DATEADD(day, -1, @ProductionDate) 
				and xy.LocationCode = @LocationCode 
				and xy.BrandCode = @BrandCode 
				and xy.ProcessGroup = @ProcessGroup
				and xy.UnitCode = MAX(e.UnitCode)
				and xy.UOMOrder = 12
				--and KPSYear = SOURCE.KPSYear
				--and KPSWeek = SOURCE.KPSWeek
		),0), 2) as BeginingStock,
		(@keluarBersihUOM7 / max(mgbg.StickPerBox)) - (@keluarBersihUOM7/max(mgbg.StickPerBox)) as EndingStock,
		max(e.CreatedDate) as CreatedDate,
		max(e.CreatedBy) as CreatedBy,
		max(e.UpdatedDate) as UpdatedDate,
		max(e.UpdatedBy) as UpdatedBy
	FROM dbo.ExeReportByProcess AS e 
	JOIN dbo.MstGenBrand AS mgb ON mgb.BrandCode = e.BrandCode
	JOIN dbo.MstGenBrandGroup AS mgbg ON mgbg.BrandGroupCode = mgb.BrandGroupCode
	WHERE e.ProcessGroup = @ProcessGroup AND e.LocationCode = @LocationCode AND e.UnitCode = @UnitCode AND e.BrandCode = @BrandCode AND e.ProductionDate = @ProductionDate
	GROUP BY e.LocationCode,e.UnitCode,e.BrandCode,e.ProductionDate,e.ProcessGroup

	-- INSERT A NEW ROW STAMPING 6
	DECLARE @extMoveAdj INT;
	SET @extMoveAdj =isnull((Select AdjustmentValue FROM ProductAdjustment
	where AdjustmentType like '%External%'
	AND [LocationCode] = @LocationCode
	AND [UnitCode] = @UnitCode
	AND [BrandCode] = @BrandCode
	AND [ProductionDate] = @ProductionDate),0)

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
		ISNULL(@extMoveAdj, 0) as KeluarBersih,
		0 as RejectSample,
		ROUND(ISNULL((select TOP 1 EndingStock from ExeReportByProcess xy
			where 
				xy.ProductionDate = DATEADD(day, -1, @ProductionDate) 
				and xy.LocationCode = @LocationCode 
				and xy.BrandCode = @BrandCode 
				and xy.ProcessGroup = @ProcessGroup
				and xy.UnitCode = MAX(e.UnitCode)
				and xy.UOMOrder = 13
				--and KPSYear = SOURCE.KPSYear
				--and KPSWeek = SOURCE.KPSWeek
		),0), 2) as BeginingStock,
		ROUND((
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
		sub.ProcessGroup = @ProcessGroup AND sub.LocationCode = @LocationCode AND sub.UnitCode = @UnitCode AND sub.BrandCode = @BrandCode AND sub.ProductionDate = @ProductionDate AND sub.UOMOrder = 11
		and sub.UnitCode = UnitCode
		)
		-
		@extMoveAdj
		---
		--max(KeluarBersih)
		), 2)
		as EndingStock,
		max(CreatedDate) as CreatedDate,
		max(CreatedBy) as CreatedBy,
		max(UpdatedDate) as UpdatedDate,
		max(UpdatedBy) as UpdatedBy
	FROM dbo.ExeReportByProcess e WHERE e.ProcessGroup = @ProcessGroup AND e.LocationCode = @LocationCode AND e.UnitCode = @UnitCode AND e.BrandCode = @BrandCode AND e.ProductionDate = @ProductionDate
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
		ROUND(ISNULL(@extMoveAdj, 0) / max(mgbg.StickPerBox), 2) as KeluarBersih,
		0 as RejectSample,
		ROund(ISNULL((select TOP 1 EndingStock from ExeReportByProcess xy
			where 
				xy.ProductionDate = DATEADD(day, -1, @ProductionDate) 
				and xy.LocationCode = @LocationCode 
				and xy.BrandCode = @BrandCode 
				and xy.ProcessGroup = @ProcessGroup
				and xy.UnitCode = MAX(e.UnitCode)
				and xy.UOMOrder = 14
				--and KPSYear = SOURCE.KPSYear
				--and KPSWeek = SOURCE.KPSWeek
		),0), 2) as BeginingStock,
		ROUND((
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
		sub.ProcessGroup = @ProcessGroup AND sub.LocationCode = @LocationCode AND sub.UnitCode = @UnitCode AND sub.BrandCode = @BrandCode AND sub.ProductionDate = @ProductionDate AND sub.UOMOrder = 12
		and sub.UnitCode = UnitCode
		)
		-
		@extMoveAdj / max(mgbg.StickPerBox)
		---
		--max(KeluarBersih)
		), 2) as EndingStock,
		max(e.CreatedDate) as CreatedDate,
		max(e.CreatedBy) as CreatedBy,
		max(e.UpdatedDate) as UpdatedDate,
		max(e.UpdatedBy) as UpdatedBy
	FROM dbo.ExeReportByProcess AS e 
	JOIN dbo.MstGenBrand AS mgb ON mgb.BrandCode = e.BrandCode
	JOIN dbo.MstGenBrandGroup AS mgbg ON mgbg.BrandGroupCode = mgb.BrandGroupCode
	WHERE e.ProcessGroup = @ProcessGroup AND e.LocationCode = @LocationCode AND e.UnitCode = @UnitCode AND e.BrandCode = @BrandCode AND e.ProductionDate = @ProductionDate
	GROUP BY e.LocationCode,e.UnitCode,e.BrandCode,e.ProductionDate,e.ProcessGroup
	END

--END