-- =============================================
-- Author:		Opun Buds
-- Create date: 14-04-2016
-- Description:	Store Procedure For insert or update to ExeReportByProcess
-- Edited by: Indra Permana
-- Description: Add brandcode, process, kpsyear, kpsweek, productiondate for parameter
-- Edited by: Ardi Siskayana
-- Description: condition beginningStock from endingstock
-- Edited by: Dwi Yudha
-- Description: extended SP condition
-- =============================================

-- =============================================
-- Description: edit rounding on SUM(TotalActualValue)
-- Author: Azka
-- Update: 11/05/2016
-- =============================================

ALTER PROCEDURE [dbo].[BaseExeReportByProcess]
	@LocationCode varchar(8),
	@BrandCode varchar(16),
	@ProcessGroup varchar(16),
	@KPSYear varchar(4),
	@KPSWeek varchar(2), 
	@CreatedBy varchar(50),
	@UpdatedBy varchar(50),
	@ProductionDate date
AS
BEGIN TRY
	
SELECT 
*,
((BeginningStock + Production)-KeluarBersih-RejectSample) AS EndingStock,
GETDATE() as CreateDate,
@CreatedBy as CreatedBy,
GETDATE() as UpdatedDate,
@UpdatedBy as UpdatedBy
INTO #SOURCE
FROM
	(SELECT 
		SOURCE.LocationCode,
		SOURCE.UnitCode,
		SOURCE.BrandCode,
		SOURCE.KPSYear,
		SOURCE.KPSWeek,
		SOURCE.ProductionDate,
		SOURCE.ProcessGroup,
		SOURCE.ProcessOrder,
		SOURCE.Shift,
		(
		(select psalv.UOMEblek
		from dbo.MstGenBrand as mgb 
		join dbo.ProcessSettingsAndLocationView AS psalv ON psalv.LocationCode = SOURCE.LocationCode AND psalv.BrandGroupCode = mgb.BrandGroupCode AND psalv.ProcessGroup = SOURCE.ProcessGroup
		where mgb.BrandCode = SOURCE.BrandCode) * SOURCE.Production
		) AS Production,
		(CASE
			WHEN SOURCE.ProcessGroup = 'ROLLING' THEN 'UnCutCigarette' 
			WHEN SOURCE.ProcessGroup = 'CUTTING' THEN 'CutCigarette' 
			WHEN SOURCE.ProcessGroup = 'FOILROLL' THEN 'Alufoil' 
			WHEN SOURCE.ProcessGroup = 'STICKWRAPPING' THEN 'WrappedCigarette' 
			WHEN SOURCE.ProcessGroup = 'PACKING' THEN 'UnStampPack' 
			WHEN SOURCE.ProcessGroup = 'STAMPING' THEN 'StampPack' 
			ELSE 'something else man, still undefined' 
		END)AS Description,
		(CASE 
			WHEN SOURCE.ProcessGroup = 'ROLLING' THEN 'Stick' 
			WHEN SOURCE.ProcessGroup = 'CUTTING' THEN 'Stick' 
			WHEN SOURCE.ProcessGroup = 'FOILROLL' THEN 'Stick' 
			WHEN SOURCE.ProcessGroup = 'STICKWRAPPING' THEN 'Stick' 
			WHEN SOURCE.ProcessGroup = 'PACKING' THEN 'Stick' 
			WHEN SOURCE.ProcessGroup = 'STAMPING' THEN 'Stick' 
			ELSE 'something else man, still undefined' 
		END)AS UOM,
		(CASE 
			WHEN SOURCE.ProcessGroup = 'ROLLING' THEN 1 
			WHEN SOURCE.ProcessGroup = 'CUTTING' THEN 2 
			WHEN SOURCE.ProcessGroup = 'FOILROLL' THEN 3 
			WHEN SOURCE.ProcessGroup = 'STICKWRAPPING' THEN 4 
			WHEN SOURCE.ProcessGroup = 'PACKING' THEN 5 
			WHEN SOURCE.ProcessGroup = 'STAMPING' THEN 7 
			ELSE 12 
		END)AS UOMOrder,
		ISNULL((
				CASE
					--WHEN SOURCE.ProcessGroup = 'ROLLING' 
						--THEN (0) 
					--WHEN SOURCE.ProcessGroup = 'STICKWRAPPING' 
						--THEN (0)
					--WHEN SOURCE.ProcessGroup = 'CUTTING' OR SOURCE.ProcessGroup = 'FOILROLL'
						--THEN dbo.GetKeluarBersihExeReportByProcess(
							--	SOURCE.ProcessGroup,SOURCE.LocationCode,SOURCE.UnitCode,
								--SOURCE.BrandCode,SOURCE.KPSYear,SOURCE.KPSWeek,SOURCE.ProductionDate,SOURCE.ProcessOrder)
					--WHEN SOURCE.ProcessGroup = 'PACKING'
						--THEN dbo.GetKeluarBersihExeReportByProcess(
							--	SOURCE.ProcessGroup,SOURCE.LocationCode,SOURCE.UnitCode,
								--SOURCE.BrandCode,SOURCE.KPSYear,SOURCE.KPSWeek,SOURCE.ProductionDate,SOURCE.ProcessOrder) 
					WHEN SOURCE.ProcessGroup = 'ROLLING' 
						THEN dbo.GetKeluarBersihExeReportByProcess(
								'CUTTING',SOURCE.LocationCode,SOURCE.UnitCode,
								SOURCE.BrandCode,SOURCE.KPSYear,SOURCE.KPSWeek,SOURCE.ProductionDate,SOURCE.ProcessOrder)
					WHEN SOURCE.ProcessGroup = 'CUTTING' OR SOURCE.ProcessGroup = 'FOILROLL' or SOURCE.ProcessGroup = 'STICKWRAPPING' and SOURCE.LocationCode !='ID26'
						THEN dbo.GetKeluarBersihExeReportByProcess(
								'PACKING',SOURCE.LocationCode,SOURCE.UnitCode,
								SOURCE.BrandCode,SOURCE.KPSYear,SOURCE.KPSWeek,SOURCE.ProductionDate,SOURCE.ProcessOrder)
					WHEN SOURCE.ProcessGroup = 'CUTTING' OR SOURCE.ProcessGroup = 'FOILROLL' or SOURCE.ProcessGroup = 'STICKWRAPPING' and SOURCE.LocationCode ='ID26'
						THEN dbo.GetKeluarBersihExeReportByProcess(
								'WRAPPING',SOURCE.LocationCode,SOURCE.UnitCode,
								SOURCE.BrandCode,SOURCE.KPSYear,SOURCE.KPSWeek,SOURCE.ProductionDate,SOURCE.ProcessOrder)
					WHEN SOURCE.ProcessGroup = 'PACKING'
						THEN dbo.GetKeluarBersihExeReportByProcess(
								SOURCE.ProcessGroup,SOURCE.LocationCode,SOURCE.UnitCode,
								SOURCE.BrandCode,SOURCE.KPSYear,SOURCE.KPSWeek,SOURCE.ProductionDate,SOURCE.ProcessOrder)
					WHEN SOURCE.ProcessGroup = 'STAMPING' OR SOURCE.ProcessGroup = 'WRAPPING'
						THEN (select psalv.UOMEblek
								from dbo.MstGenBrand as mgb 
								join dbo.ProcessSettingsAndLocationView AS psalv ON psalv.LocationCode = SOURCE.LocationCode AND psalv.BrandGroupCode = mgb.BrandGroupCode AND psalv.ProcessGroup = SOURCE.ProcessGroup
								where mgb.BrandCode = SOURCE.BrandCode) * SOURCE.Production
					ELSE 0 
				END),0) AS KeluarBersih,
				(CASE 
					WHEN SOURCE.ProcessGroup = 'PACKING' THEN 0 
					ELSE 0 
				END) AS RejectSample,
				(CASE 
					--WHEN (select TOP 1 EndingStock from ExeReportByProcess where ProductionDate = DATEADD(day, -1, getdate()) and LocationCode = @LocationCode and BrandCode = @BrandCode and ProcessGroup = @ProcessGroup) =  NULL
					--THEN 0 
					--ELSE (select TOP 1 EndingStock from ExeReportByProcess where ProductionDate = DATEADD(day, -1, getdate()) and LocationCode = @LocationCode and BrandCode = @BrandCode and ProcessGroup = @ProcessGroup)
					WHEN (select TOP 1 EndingStock from ExeReportByProcess 
						where 
							ProductionDate = DATEADD(day, -1, @ProductionDate)
							and LocationCode = @LocationCode 
							and BrandCode = @BrandCode 
							and ProcessGroup = @ProcessGroup
							and UnitCode = SOURCE.UnitCode
							--and KPSYear = SOURCE.KPSYear
							--and KPSWeek = SOURCE.KPSWeek
						) =  0 
					THEN 0 
					ELSE (select TOP 1 EndingStock from ExeReportByProcess 
						where 
							ProductionDate = DATEADD(day, -1, @ProductionDate) 
							and LocationCode = @LocationCode 
							and BrandCode = @BrandCode 
							and ProcessGroup = @ProcessGroup
							and UnitCode = SOURCE.UnitCode
							--and KPSYear = SOURCE.KPSYear
							--and KPSWeek = SOURCE.KPSWeek
							)
				END)AS BeginningStock
	FROM
	(SELECT 
		MAX(a.LocationCode) AS LocationCode,
		MAX(x.UnitCode) AS UnitCode,
		MAX(a.BrandCode) AS BrandCode,
		MAX(a.KPSYear) AS KPSYear,
		MAX(a.KPSWeek) AS KPSWeek,
		MAX(a.ProductionDate) AS ProductionDate,
		MAX(a.ProcessGroup) AS ProcessGroup,
		MAX(po.ProcessOrder) AS ProcessOrder,
		MAX(l.Shift) AS Shift,
		ROUND((SUM(ISNULL(a.TotalActualValue,0))), 2) AS Production
	FROM dbo.ExeTPOProductionEntryVerificationView a
	JOIN dbo.MstPlantUnit x ON a.LocationCode = x.LocationCode AND x.UnitCode = 'PROD'
	JOIN dbo.MstGenProcess po ON po.ProcessGroup = a.ProcessGroup AND po.StatusActive = 1
	JOIN dbo.MstGenLocation l ON l.LocationCode = a.LocationCode AND l.StatusActive = 1
	GROUP BY a.LocationCode,x.UnitCode,a.BrandCode,a.ProductionDate,po.ProcessOrder
	UNION ALL 
	SELECT max(b.LocationCode) AS LocationCode,max(b.UnitCode) AS UnitCode,
		max(b.BrandCode) AS BrandCode,max(b.KPSYear) AS KPSYear,
		max(b.KPSWeek) AS KPSWeek,max(b.ProductionDate) AS ProductionDate,
		max(b.ProcessGroup) AS ProcessGroup,
		max(b.ProcessOrder) AS ProcessOrder,
		max(b.Shift) AS Shift, ROUND(SUM(ISNULL(b.TotalActualValue,0)), 2) AS Production
	FROM dbo.ExePlantProductionEntryVerificationView b
	GROUP BY b.LocationCode,b.UnitCode,b.BrandCode,b.ProductionDate,b.ProcessOrder) AS SOURCE
	--JOIN dbo.MstGenBrand AS mgb ON mgb.BrandCode = SOURCE.BrandCode
	--JOIN dbo.ProcessSettingsAndLocationView AS psalv ON psalv.LocationCode = SOURCE.LocationCode AND psalv.BrandGroupCode = mgb.BrandGroupCode AND psalv.ProcessGroup = SOURCE.ProcessGroup
) AS CH9

	IF EXISTS (SELECT 1 FROM dbo.ExeReportByProcess WHERE LocationCode = @LocationCode AND BrandCode = @BrandCode AND ProcessGroup = @ProcessGroup AND KPSYear = @KPSYear AND KPSWeek = @KPSWeek AND ProductionDate = @ProductionDate)
		BEGIN
			UPDATE dbo.ExeReportByProcess
			SET 
				Production = 
				(
				CASE
				WHEN dbo.ExeReportByProcess.UOMOrder = 6
					THEN (#SOURCE.Production / psalv.StickPerPack)
				ELSE #SOURCE.Production
				END
				), 
				KeluarBersih = (
				CASE
				WHEN dbo.ExeReportByProcess.UOMOrder = 6
					THEN (#SOURCE.KeluarBersih / psalv.StickPerPack)
				ELSE #SOURCE.KeluarBersih
				END
				),
				EndingStock = (
				CASE
				WHEN dbo.ExeReportByProcess.UOMOrder = 6
				THEN ((dbo.ExeReportByProcess.BeginningStock + dbo.ExeReportByProcess.Production)-dbo.ExeReportByProcess.KeluarBersih-dbo.ExeReportByProcess.RejectSample)
				ELSE ISNULL(#SOURCE.EndingStock,0)
				END
				),
				UpdatedDate = GETDATE()
			FROM #SOURCE
			JOIN dbo.ExeReportByProcess 
				ON 
					dbo.ExeReportByProcess.LocationCode = #SOURCE.LocationCode
					AND dbo.ExeReportByProcess.UnitCode = #SOURCE.UnitCode 
					AND dbo.ExeReportByProcess.BrandCode = #SOURCE.BrandCode
					AND dbo.ExeReportByProcess.KPSYear = #SOURCE.KPSYear
					AND dbo.ExeReportByProcess.KPSWeek = #SOURCE.KPSWeek
					AND dbo.ExeReportByProcess.ProductionDate = #SOURCE.ProductionDate
					AND dbo.ExeReportByProcess.ProcessGroup = #SOURCE.ProcessGroup
					AND dbo.ExeReportByProcess.ProcessOrder = #SOURCE.ProcessOrder
					AND dbo.ExeReportByProcess.UOMOrder = #SOURCE.UOMOrder
			JOIN dbo.MstGenBrand AS mgb ON mgb.BrandCode = dbo.ExeReportByProcess.BrandCode
			JOIN dbo.ProcessSettingsAndLocationView AS psalv ON psalv.LocationCode = dbo.ExeReportByProcess.LocationCode AND psalv.BrandGroupCode = mgb.BrandGroupCode AND psalv.ProcessGroup = dbo.ExeReportByProcess.ProcessGroup
			WHERE #SOURCE.LocationCode = @LocationCode AND #SOURCE.BrandCode = @BrandCode AND #SOURCE.ProcessGroup = @ProcessGroup and #SOURCE.ProductionDate = @ProductionDate
		END
	ELSE
		BEGIN
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
				#SOURCE.LocationCode,
				#SOURCE.UnitCode,
				#SOURCE.BrandCode,
				#SOURCE.KPSYear,
				#SOURCE.KPSWeek,
				#SOURCE.ProductionDate,
				#SOURCE.ProcessGroup,
				#SOURCE.ProcessOrder,
				#SOURCE.Shift,
				#SOURCE.Description,
				#SOURCE.UOM,
				#SOURCE.UOMOrder,
				ISNULL(#SOURCE.Production,0) AS Production,
				ISNULL(#SOURCE.KeluarBersih,0) AS KeluarBersih,
				#SOURCE.RejectSample,
				ISNULL(#SOURCE.BeginningStock,0) AS BeginningStock,
				ISNULL(#SOURCE.EndingStock,0) AS EndingStock,
				GETDATE(),
				@CreatedBy,
				GETDATE(),
				@UpdatedBy
			FROM
			  #SOURCE 
			WHERE #SOURCE.LocationCode = @LocationCode AND #SOURCE.BrandCode = @BrandCode AND #SOURCE.ProcessGroup = @ProcessGroup AND #SOURCE.ProductionDate = @ProductionDate

			if(@ProcessGroup = 'STAMPING' OR @ProcessGroup = 'PACKING')
			BEGIN
				EXEC [dbo].ExtendedExeReportByProcess @LocationCode,@BrandCode,@CreatedBy,@UpdatedBy,@ProductionDate,@ProcessGroup;
			END
		END

END TRY
BEGIN CATCH
	SELECT
        ERROR_NUMBER() AS ErrorNumber
        ,ERROR_SEVERITY() AS ErrorSeverity
        ,ERROR_STATE() AS ErrorState
        ,ERROR_PROCEDURE() AS ErrorProcedure
        ,ERROR_LINE() AS ErrorLine
        ,ERROR_MESSAGE() AS ErrorMessage;
END CATCH

GO


