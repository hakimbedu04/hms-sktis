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
		max(e.BeginningStock) as BeginingStock,
		max(e.EndingStock) as EndingStock,
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
		max(e.BeginningStock) as BeginingStock,
		max(e.EndingStock) as EndingStock,
		max(e.CreatedDate) as CreatedDate,
		max(e.CreatedBy) as CreatedBy,
		max(e.UpdatedDate) as UpdatedDate,
		max(e.UpdatedBy) as UpdatedBy
	FROM dbo.ExeReportByProcess AS e
	JOIN dbo.MstGenBrand AS mgb ON mgb.BrandCode = e.BrandCode
	JOIN dbo.ProcessSettingsAndLocationView AS psalv ON psalv.LocationCode = e.LocationCode AND psalv.BrandGroupCode = mgb.BrandGroupCode AND psalv.ProcessGroup = e.ProcessGroup
	WHERE e.ProcessGroup = 'STAMPING' AND e.LocationCode = @LocationCode AND e.BrandCode = @BrandCode AND e.ProductionDate = @ProductionDate
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
		max(BeginningStock) as BeginingStock,
		max(EndingStock) as EndingStock,
		max(CreatedDate) as CreatedDate,
		max(CreatedBy) as CreatedBy,
		max(UpdatedDate) as UpdatedDate,
		max(UpdatedBy) as UpdatedBy
	FROM dbo.ExeReportByProcess WHERE ProcessGroup = 'STAMPING' AND LocationCode = @LocationCode AND BrandCode = @BrandCode AND ProductionDate = @ProductionDate
	GROUP BY LocationCode,UnitCode,BrandCode,ProductionDate,ProcessGroup

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
		max(e.BeginningStock) as BeginingStock,
		max(e.EndingStock) as EndingStock,
		max(e.CreatedDate) as CreatedDate,
		max(e.CreatedBy) as CreatedBy,
		max(e.UpdatedDate) as UpdatedDate,
		max(e.UpdatedBy) as UpdatedBy
	FROM dbo.ExeReportByProcess e
	JOIN dbo.MstGenBrand AS mgb ON mgb.BrandCode = e.BrandCode
	JOIN dbo.ProcessSettingsAndLocationView AS psalv ON psalv.LocationCode = e.LocationCode AND psalv.BrandGroupCode = mgb.BrandGroupCode AND psalv.ProcessGroup = e.ProcessGroup
	WHERE e.ProcessGroup = 'STAMPING' AND e.LocationCode = @LocationCode AND e.BrandCode = @BrandCode AND ProductionDate = @ProductionDate
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
		max(BeginningStock) as BeginingStock,
		max(EndingStock) as EndingStock,
		max(CreatedDate) as CreatedDate,
		max(CreatedBy) as CreatedBy,
		max(UpdatedDate) as UpdatedDate,
		max(UpdatedBy) as UpdatedBy
	FROM dbo.ExeReportByProcess WHERE ProcessGroup = 'STAMPING' AND LocationCode = @LocationCode AND BrandCode = @BrandCode AND ProductionDate = @ProductionDate
	GROUP BY LocationCode,UnitCode,BrandCode,ProductionDate,ProcessGroup

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
		max(e.BeginningStock) as BeginingStock,
		max(e.EndingStock) as EndingStock,
		max(e.CreatedDate) as CreatedDate,
		max(e.CreatedBy) as CreatedBy,
		max(e.UpdatedDate) as UpdatedDate,
		max(e.UpdatedBy) as UpdatedBy
	FROM dbo.ExeReportByProcess AS e 
	JOIN dbo.MstGenBrand AS mgb ON mgb.BrandCode = e.BrandCode
	JOIN dbo.ProcessSettingsAndLocationView AS psalv ON psalv.LocationCode = e.LocationCode AND psalv.BrandGroupCode = mgb.BrandGroupCode AND psalv.ProcessGroup = e.ProcessGroup
	WHERE e.ProcessGroup = 'STAMPING' AND e.LocationCode = @LocationCode AND e.BrandCode = @BrandCode AND e.ProductionDate = @ProductionDate
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
		max(BeginningStock) as BeginingStock,
		(
		SELECT TOP 1 (sub.BeginningStock + sub.KeluarBersih) as EndingStock
		FROM dbo.ExeReportByProcess  AS sub 
		WHERE 
		sub.ProcessGroup = 'STAMPING' AND sub.LocationCode = @LocationCode AND sub.BrandCode = @BrandCode AND sub.ProductionDate = @ProductionDate AND sub.UOMOrder = 11
		and sub.UnitCode = UnitCode
		) as EndingStock,
		max(CreatedDate) as CreatedDate,
		max(CreatedBy) as CreatedBy,
		max(UpdatedDate) as UpdatedDate,
		max(UpdatedBy) as UpdatedBy
	FROM dbo.ExeReportByProcess WHERE ProcessGroup = 'STAMPING' AND LocationCode = @LocationCode AND BrandCode = @BrandCode AND ProductionDate = @ProductionDate
	GROUP BY LocationCode,UnitCode,BrandCode,ProductionDate,ProcessGroup

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
		max(e.BeginningStock) as BeginingStock,
		(
		SELECT TOP 1 (sub.BeginningStock + sub.KeluarBersih) as EndingStock
		FROM dbo.ExeReportByProcess  AS sub 
		WHERE 
		sub.ProcessGroup = 'STAMPING' AND sub.LocationCode = @LocationCode AND sub.BrandCode = @BrandCode AND sub.ProductionDate = @ProductionDate AND sub.UOMOrder = 12
		and sub.UnitCode = UnitCode
		) as EndingStock,
		max(e.CreatedDate) as CreatedDate,
		max(e.CreatedBy) as CreatedBy,
		max(e.UpdatedDate) as UpdatedDate,
		max(e.UpdatedBy) as UpdatedBy
	FROM dbo.ExeReportByProcess AS e 
	JOIN dbo.MstGenBrand AS mgb ON mgb.BrandCode = e.BrandCode
	JOIN dbo.ProcessSettingsAndLocationView AS psalv ON psalv.LocationCode = e.LocationCode AND psalv.BrandGroupCode = mgb.BrandGroupCode AND psalv.ProcessGroup = e.ProcessGroup
	WHERE e.ProcessGroup = 'STAMPING' AND e.LocationCode = @LocationCode AND e.BrandCode = @BrandCode AND e.ProductionDate = @ProductionDate
	GROUP BY e.LocationCode,e.UnitCode,e.BrandCode,e.ProductionDate,e.ProcessGroup
	END

END



GO


