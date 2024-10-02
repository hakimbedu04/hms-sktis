IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GenerateP1TemplateGL1]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GenerateP1TemplateGL1]
GO


CREATE PROC [dbo].[GenerateP1TemplateGL1]
(
		-- begin : parameter function
	@ParamdateTo DATETIME,
	@ParamWeek INT,
	@paramYear INT,
	@ParamLocation VARCHAR(8)
	-- end : parameter function
)
AS
BEGIN
	-- author : hakim
	-- ticket : http://tp.voxteneo.co.id/entity/11020
	-- description : create p1 template GL using ado.net connection
	-- date : 2016-11-09

	-- author : hakim
	-- ticket : http://tp.voxteneo.co.id/entity/11020
	-- description : change logic get value DocCurAmount
	-- date : 2016-11-17

	-- author : hakim
	-- ticket : http://tp.voxteneo.co.id/entity/11276
	-- description : update calculation logic for biayaproduksi and jasamaklon (recalculate, doesn't use Rp)
	-- date : 2016-11-30
	
	-- declare local parameter snipping
	DECLARE @dateFrom DATETIME;
	DECLARE @dateTo DATETIME;
	DECLARE @Week INT;
	DECLARE @Year INT;
	DECLARE @Location VARCHAR(8);
	DECLARE @DocCurAmountProd Float;
	DECLARE @DocCurAmountMgmt Float;
	DECLARE @TempTable1 TABLE (TPOFeeCode VARCHAR(64), KPSWeek INT, KPSYear INT, LocationCode VARCHAR(4), BrandGroupCode VARCHAR(20),BiayaProduksi FLOAT, JasaMaklon FLOAT, ParentLocationCode VARCHAR(4));
	--DECLARE @TempTable2 TABLE (BrandGroupCode VARCHAR(20), LocationCode VARCHAR(8), DocCurAmount Numeric(20,0));

	SET @Year = @paramYear
	SET @Week = @ParamWeek
	SELECT @dateFrom = StartDate from MstGenWeek where Week = @Week and Year = @Year
	SET @dateTo = @ParamdateTo
	SET @Location = @ParamLocation

	--select * from MstTPOFeeRate


	--IF(@Location = 'TPO')
	--BEGIN
	--	INSERT INTO @TempTable1
	--	SELECT TDP.TPOFeeCode,TDP.[KPSYear],TDP.[KPSWeek],MGL.LocationCode,TFR.BrandGroupCode
	--		, (
	--			(CASE WHEN ISNULL(TFR.JKN,0) != 0 THEN (SUM(ISNULL(TDP.JKNRp,0)) / ISNULL(TFR.JKN,0)) ELSE 0 END * TFR.JKN) +
	--			(CASE WHEN ISNULL(TFR.Jl1,0) != 0 THEN (SUM(ISNULL(TDP.JL1Rp,0)) / ISNULL(TFR.JL1,0)) ELSE 0 END * TFR.JL1) +
	--			(CASE WHEN ISNULL(TFR.Jl2,0) != 0 THEN (SUM(ISNULL(TDP.JL2Rp,0)) / ISNULL(TFR.Jl2,0)) ELSE 0 END * TFR.Jl2) +
	--			(CASE WHEN ISNULL(TFR.Jl3,0) != 0 THEN (SUM(ISNULL(TDP.JL3Rp,0)) / ISNULL(TFR.Jl3,0)) ELSE 0 END * TFR.Jl3) +
	--			(CASE WHEN ISNULL(TFR.Jl4,0) != 0 THEN (SUM(ISNULL(TDP.JL4Rp,0)) / ISNULL(TFR.Jl4,0)) ELSE 0 END * TFR.Jl4)
	--			) as BiayaProduksi
	--		, (
	--			ISNULL(TFR.ManagementFee,0) * 
	--			(CASE WHEN ISNULL(TFR.JKN,0) != 0 THEN (SUM(ISNULL(TDP.JKNRp,0)) / ISNULL(TFR.JKN,0)) ELSE 0 END +
	--				CASE WHEN ISNULL(TFR.Jl1,0) != 0 THEN (SUM(ISNULL(TDP.JL1Rp,0)) / ISNULL(TFR.JL1,0)) ELSE 0 END +
	--				CASE WHEN ISNULL(TFR.Jl2,0) != 0 THEN (SUM(ISNULL(TDP.JL2Rp,0)) / ISNULL(TFR.Jl2,0)) ELSE 0 END +
	--				CASE WHEN ISNULL(TFR.Jl3,0) != 0 THEN (SUM(ISNULL(TDP.JL3Rp,0)) / ISNULL(TFR.Jl3,0)) ELSE 0 END +
	--				CASE WHEN ISNULL(TFR.Jl4,0) != 0 THEN (SUM(ISNULL(TDP.JL4Rp,0)) / ISNULL(TFR.Jl4,0)) ELSE 0 END
	--			)
	--			) as JasaMaklon
	--			, MGL.ParentLocationCode
	--	FROM [TPOFeeProductionDailyPlan] TDP
	--	INNER JOIN TPOFeeHdrPlan FHP
	--	ON tdp.TPOFeeCode=FHP.TPOFeeCode
	--	INNER JOIN [MstGenLocation] MGL
	--	ON MGL.LocationCode=FHP.LocationCode
	--	INNER JOIN MstTPOFeeRate TFR
	--	ON TFR.LocationCode = FHP.LocationCode
	--	WHERE TFR.BrandGroupCode = FHP.BrandGroupCode
	--	and TDP.FeeDate >= @dateFrom
	--	and TDP.FeeDate <= @dateTo
	--	and TDP.KPSWeek = @Week
	--	and TDP.KPSYear = @Year
	--	and TFR.EffectiveDate <= @dateFrom
	--	and TFR.ExpiredDate >= @dateTo
	--	GROUP BY TDP.TPOFeeCode,TDP.[KPSYear],TDP.[KPSWeek],MGL.LocationCode,TFR.BrandGroupCode
	--	,TFR.JKN, TFR.JL1, TFR.Jl2, TFR.Jl3, TFR.Jl4, TFR.ManagementFee,MGL.ParentLocationCode
	--	ORDER BY MGL.ParentLocationCode, MGL.LocationCode
	--END
	--ELSE 
	--BEGIN
	--	INSERT INTO @TempTable1
	--	SELECT TDP.TPOFeeCode,TDP.[KPSYear],TDP.[KPSWeek],MGL.LocationCode,TFR.BrandGroupCode
	--		, (
	--			(CASE WHEN ISNULL(TFR.JKN,0) != 0 THEN (SUM(ISNULL(TDP.JKNRp,0)) / ISNULL(TFR.JKN,0)) ELSE 0 END * TFR.JKN) +
	--			(CASE WHEN ISNULL(TFR.Jl1,0) != 0 THEN (SUM(ISNULL(TDP.JL1Rp,0)) / ISNULL(TFR.JL1,0)) ELSE 0 END * TFR.JL1) +
	--			(CASE WHEN ISNULL(TFR.Jl2,0) != 0 THEN (SUM(ISNULL(TDP.JL2Rp,0)) / ISNULL(TFR.Jl2,0)) ELSE 0 END * TFR.Jl2) +
	--			(CASE WHEN ISNULL(TFR.Jl3,0) != 0 THEN (SUM(ISNULL(TDP.JL3Rp,0)) / ISNULL(TFR.Jl3,0)) ELSE 0 END * TFR.Jl3) +
	--			(CASE WHEN ISNULL(TFR.Jl4,0) != 0 THEN (SUM(ISNULL(TDP.JL4Rp,0)) / ISNULL(TFR.Jl4,0)) ELSE 0 END * TFR.Jl4)
	--		  ) as BiayaProduksi
	--		, (
	--			ISNULL(TFR.ManagementFee,0) * 
	--			(CASE WHEN ISNULL(TFR.JKN,0) != 0 THEN (SUM(ISNULL(TDP.JKNRp,0)) / ISNULL(TFR.JKN,0)) ELSE 0 END +
	--			 CASE WHEN ISNULL(TFR.Jl1,0) != 0 THEN (SUM(ISNULL(TDP.JL1Rp,0)) / ISNULL(TFR.JL1,0)) ELSE 0 END +
	--			 CASE WHEN ISNULL(TFR.Jl2,0) != 0 THEN (SUM(ISNULL(TDP.JL2Rp,0)) / ISNULL(TFR.Jl2,0)) ELSE 0 END +
	--			 CASE WHEN ISNULL(TFR.Jl3,0) != 0 THEN (SUM(ISNULL(TDP.JL3Rp,0)) / ISNULL(TFR.Jl3,0)) ELSE 0 END +
	--			 CASE WHEN ISNULL(TFR.Jl4,0) != 0 THEN (SUM(ISNULL(TDP.JL4Rp,0)) / ISNULL(TFR.Jl4,0)) ELSE 0 END
	--			)
	--		  ) as JasaMaklon
	--		  , MGL.ParentLocationCode
	--	FROM [TPOFeeProductionDailyPlan] TDP
	--	INNER JOIN TPOFeeHdrPlan FHP
	--	ON tdp.TPOFeeCode=FHP.TPOFeeCode
	--	INNER JOIN [MstGenLocation] MGL
	--	ON MGL.LocationCode=FHP.LocationCode
	--	INNER JOIN MstTPOFeeRate TFR
	--	ON TFR.LocationCode = FHP.LocationCode
	--	WHERE TFR.BrandGroupCode = FHP.BrandGroupCode
	--	and TDP.FeeDate >= @dateFrom
	--	and TDP.FeeDate <= @dateTo
	--	and TDP.KPSWeek = @Week
	--	and TDP.KPSYear = @Year
	--	and TFR.EffectiveDate <= @dateFrom
	--	and TFR.ExpiredDate >= @dateTo
	--	and MGL.ParentLocationCode = @Location
	--	GROUP BY TDP.TPOFeeCode,TDP.[KPSYear],TDP.[KPSWeek],MGL.LocationCode,TFR.BrandGroupCode
	--	,TFR.JKN, TFR.JL1, TFR.Jl2, TFR.Jl3, TFR.Jl4, TFR.ManagementFee, MGL.ParentLocationCode
	--	ORDER BY MGL.LocationCode
	--END

	IF(@Location = 'TPO')
	BEGIN
		INSERT INTO @TempTable1
		SELECT TDP.TPOFeeCode,TDP.[KPSYear],TDP.[KPSWeek],MGL.LocationCode,TFR.BrandGroupCode
		, (
		((
			ROUND(CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN 
		
			CASE WHEN 
			( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) ) <= 
			( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JKNJam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
			THEN ( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) )
			ELSE ( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JKNJam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
			END

		END,0)
		) * TFR.JKN) + 
		(
		(
		ROUND(CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN 
		
			(
				CASE WHEN 
				( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (
																		CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN 
																			CASE WHEN 
																			( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) ) <= 
																			( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JKNJam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
																			THEN ( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) )
																			ELSE ( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JKNJam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
																			END
																		END
																	 ) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) ) > 0
				THEN
				(
					CASE WHEN 
					( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (
																		CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN 
																			CASE WHEN 
																			( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) ) <= 
																			( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JKNJam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
																			THEN ( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) )
																			ELSE ( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JKNJam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
																			END
																		END
																	 ) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) ) <=
																	--paket * empPackage * stdPerHour * sumJl1Jam) / stickPerBox
																	( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JL1Jam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )

					THEN
						( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (
																		CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN 
																			CASE WHEN 
																			( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) ) <= 
																			( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JKNJam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
																			THEN ( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) )
																			ELSE ( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JKNJam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
																			END
																		END
																	 ) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) )
					ELSE
					( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JL1Jam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
					END
			
				)
				ELSE 0 
				END			
		
			)
		
		END,0)
	
		) * TFR.JL1) +
		(
		(
		ROUND(CASE WHEN 
		( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (
															  ROUND(CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN 
																  CASE WHEN 
																  ( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) ) <= 
																  ( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JKNJam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
																  THEN ( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) )
																  ELSE ( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JKNJam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
																  END
															  END,0)
															  ) 
															  - (
																ROUND(CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN 
																(
																	CASE WHEN 
																	( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (
																															CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN 
																																CASE WHEN 
																																( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) ) <= 
																																( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JKNJam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
																																THEN ( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) )
																																ELSE ( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JKNJam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
																																END
																															END
																															) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) ) > 0
																	THEN
																	(
																		CASE WHEN 
																		( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (
																															CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN 
																																CASE WHEN 
																																( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) ) <= 
																																( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JKNJam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
																																THEN ( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) )
																																ELSE ( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JKNJam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
																																END
																															END
																															) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) ) <=
																														--paket * empPackage * stdPerHour * sumJl1Jam) / stickPerBox
																														( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JL1Jam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )

																		THEN
																			( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (
																															CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN 
																																CASE WHEN 
																																( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) ) <= 
																																( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JKNJam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
																																THEN ( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) )
																																ELSE ( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JKNJam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
																																END
																															END
																															) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) )
																		ELSE
																		( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JL1Jam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
																		END
			
																	)
																	ELSE 0 
																	END
		
																)
		
															END,0)
														  
														  
															  )
															  - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) ) > 0
		 THEN 
		 ( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (
																ROUND(CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN 
																	CASE WHEN 
																	( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) ) <= 
																	( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JKNJam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
																	THEN ( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) )
																	ELSE ( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JKNJam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
																	END
																END,0)
																) 
																- (
																ROUND(CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN 
																(
																	CASE WHEN 
																	( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (
																															CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN 
																																CASE WHEN 
																																( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) ) <= 
																																( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JKNJam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
																																THEN ( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) )
																																ELSE ( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JKNJam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
																																END
																															END
																															) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) ) > 0
																	THEN
																	(
																		CASE WHEN 
																		( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (
																															CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN 
																																CASE WHEN 
																																( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) ) <= 
																																( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JKNJam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
																																THEN ( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) )
																																ELSE ( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JKNJam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
																																END
																															END
																															) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) ) <=
																														--paket * empPackage * stdPerHour * sumJl1Jam) / stickPerBox
																														( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JL1Jam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )

																		THEN
																			( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (
																															CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN 
																																CASE WHEN 
																																( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) ) <= 
																																( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JKNJam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
																																THEN ( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) )
																																ELSE ( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JKNJam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
																																END
																															END
																															) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) )
																		ELSE
																		( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JL1Jam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
																		END
			
																	)
																	ELSE 0 
																	END			
		
																)
		
															END,0)
														  
														  
																)
																- (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) )
		 ELSE 0
		 END,0)
		) * TFR.Jl2)

		) as BiayaProduksi
		,( SUM(TDP.OutputBox) * TFR.ManagementFee) as JasaMaklon
		,MGL.ParentLocationCode
	FROM [TPOFeeProductionDailyPlan] TDP
	INNER JOIN TPOFeeHdrPlan FHP
	ON tdp.TPOFeeCode=FHP.TPOFeeCode
	INNER JOIN [MstGenLocation] MGL
	ON MGL.LocationCode=FHP.LocationCode
	INNER JOIN MstTPOFeeRate TFR
	ON TFR.LocationCode = FHP.LocationCode
	INNER JOIN MstTPOPackage MTP
	ON MGL.LocationCode = MTP.LocationCode
	INNER JOIN MstGenBrandGroup MGBG
	ON TFR.BrandGroupCode = MGBG.BrandGroupCode
	INNER JOIN ProcessSettingsAndLocationView PSLV
	ON FHP.LocationCode = PSLV.LocationCode
	WHERE TFR.BrandGroupCode = FHP.BrandGroupCode
	and FHP.BrandGroupCode = pslv.BrandGroupCode
	and PSLV.ProcessGroup = 'ROLLING'
	and FHP.BrandGroupCode = MTP.BrandGroupCode
	and TDP.FeeDate >= @dateFrom
	and TDP.FeeDate <= @dateTo
	and TDP.KPSWeek = @Week
	and TDP.KPSYear = @Year
	and TFR.EffectiveDate <= @dateFrom
	and TFR.ExpiredDate >= @dateTo

	and MTP.EffectiveDate <= @dateFrom
	and MTP.ExpiredDate >= @dateTo
	GROUP BY TDP.TPOFeeCode,TDP.[KPSYear],TDP.[KPSWeek],MGL.LocationCode,TFR.BrandGroupCode
	,TFR.JKN, TFR.JL1, TFR.Jl2, TFR.Jl3, TFR.Jl4, TFR.ManagementFee, MGL.ParentLocationCode,MTP.Package,MGBG.EmpPackage,MGBG.StickPerBox, PSLV.StdStickPerHour
	ORDER BY MGL.ParentLocationCode, MGL.LocationCode
	END
	ELSE 
	BEGIN
		INSERT INTO @TempTable1
		SELECT TDP.TPOFeeCode,TDP.[KPSYear],TDP.[KPSWeek],MGL.LocationCode,TFR.BrandGroupCode
		, (
		((
			ROUND(CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN 
		
			CASE WHEN 
			( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) ) <= 
			( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JKNJam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
			THEN ( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) )
			ELSE ( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JKNJam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
			END

		END,0)
		) * TFR.JKN) + 
		(
		(
		ROUND(CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN 
		
			(
				CASE WHEN 
				( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (
																		CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN 
																			CASE WHEN 
																			( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) ) <= 
																			( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JKNJam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
																			THEN ( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) )
																			ELSE ( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JKNJam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
																			END
																		END
																	 ) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) ) > 0
				THEN
				(
					CASE WHEN 
					( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (
																		CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN 
																			CASE WHEN 
																			( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) ) <= 
																			( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JKNJam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
																			THEN ( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) )
																			ELSE ( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JKNJam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
																			END
																		END
																	 ) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) ) <=
																	--paket * empPackage * stdPerHour * sumJl1Jam) / stickPerBox
																	( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JL1Jam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )

					THEN
						( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (
																		CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN 
																			CASE WHEN 
																			( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) ) <= 
																			( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JKNJam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
																			THEN ( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) )
																			ELSE ( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JKNJam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
																			END
																		END
																	 ) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) )
					ELSE
					( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JL1Jam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
					END
			
				)
				ELSE 0 
				END			
		
			)
		
		END,0)
	
		) * TFR.JL1) +
		(
		(
		ROUND(CASE WHEN 
		( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (
															  ROUND(CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN 
																  CASE WHEN 
																  ( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) ) <= 
																  ( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JKNJam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
																  THEN ( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) )
																  ELSE ( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JKNJam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
																  END
															  END,0)
															  ) 
															  - (
																ROUND(CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN 
																(
																	CASE WHEN 
																	( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (
																															CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN 
																																CASE WHEN 
																																( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) ) <= 
																																( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JKNJam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
																																THEN ( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) )
																																ELSE ( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JKNJam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
																																END
																															END
																															) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) ) > 0
																	THEN
																	(
																		CASE WHEN 
																		( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (
																															CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN 
																																CASE WHEN 
																																( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) ) <= 
																																( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JKNJam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
																																THEN ( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) )
																																ELSE ( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JKNJam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
																																END
																															END
																															) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) ) <=
																														--paket * empPackage * stdPerHour * sumJl1Jam) / stickPerBox
																														( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JL1Jam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )

																		THEN
																			( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (
																															CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN 
																																CASE WHEN 
																																( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) ) <= 
																																( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JKNJam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
																																THEN ( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) )
																																ELSE ( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JKNJam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
																																END
																															END
																															) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) )
																		ELSE
																		( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JL1Jam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
																		END
			
																	)
																	ELSE 0 
																	END
		
																)
		
															END,0)
														  
														  
															  )
															  - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) ) > 0
		 THEN 
		 ( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (
																ROUND(CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN 
																	CASE WHEN 
																	( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) ) <= 
																	( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JKNJam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
																	THEN ( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) )
																	ELSE ( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JKNJam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
																	END
																END,0)
																) 
																- (
																ROUND(CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN 
																(
																	CASE WHEN 
																	( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (
																															CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN 
																																CASE WHEN 
																																( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) ) <= 
																																( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JKNJam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
																																THEN ( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) )
																																ELSE ( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JKNJam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
																																END
																															END
																															) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) ) > 0
																	THEN
																	(
																		CASE WHEN 
																		( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (
																															CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN 
																																CASE WHEN 
																																( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) ) <= 
																																( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JKNJam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
																																THEN ( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) )
																																ELSE ( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JKNJam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
																																END
																															END
																															) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) ) <=
																														--paket * empPackage * stdPerHour * sumJl1Jam) / stickPerBox
																														( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JL1Jam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )

																		THEN
																			( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (
																															CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN 
																																CASE WHEN 
																																( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) ) <= 
																																( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JKNJam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
																																THEN ( (SUM(TDP.JKN)) + (SUM(TDP.JL1)) + (SUM(TDP.Jl2)) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) )
																																ELSE ( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JKNJam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
																																END
																															END
																															) - (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) )
																		ELSE
																		( ( (CASE WHEN ISNULL(MTP.Package,0) != 0 THEN ISNULL(MTP.Package,0) ELSE 0 END) * (MGBG.EmpPackage) * (CASE WHEN ISNULL(PSLV.StdStickPerHour,0) != 0 THEN PSLV.StdStickPerHour ELSE 0 END) * (SUM(TDP.JL1Jam)) ) / (CASE WHEN ISNULL(MGBG.StickPerBox,0) != 0 THEN MGBG.StickPerBox ELSE 0 END) )
																		END
			
																	)
																	ELSE 0 
																	END			
		
																)
		
															END,0)
														  
														  
																)
																- (SUM(TDP.Jl3)) - (SUM(TDP.Jl4)) )
		 ELSE 0
		 END,0)
		) * TFR.Jl2)

		) as BiayaProduksi
		,( SUM(TDP.OutputBox) * TFR.ManagementFee) as JasaMaklon
		,MGL.ParentLocationCode
	FROM [TPOFeeProductionDailyPlan] TDP
	INNER JOIN TPOFeeHdrPlan FHP
	ON tdp.TPOFeeCode=FHP.TPOFeeCode
	INNER JOIN [MstGenLocation] MGL
	ON MGL.LocationCode=FHP.LocationCode
	INNER JOIN MstTPOFeeRate TFR
	ON TFR.LocationCode = FHP.LocationCode
	INNER JOIN MstTPOPackage MTP
	ON MGL.LocationCode = MTP.LocationCode
	INNER JOIN MstGenBrandGroup MGBG
	ON TFR.BrandGroupCode = MGBG.BrandGroupCode
	INNER JOIN ProcessSettingsAndLocationView PSLV
	ON FHP.LocationCode = PSLV.LocationCode
	WHERE TFR.BrandGroupCode = FHP.BrandGroupCode
	and FHP.BrandGroupCode = pslv.BrandGroupCode
	and PSLV.ProcessGroup = 'ROLLING'
	and FHP.BrandGroupCode = MTP.BrandGroupCode
	and TDP.FeeDate >= @dateFrom
	and TDP.FeeDate <= @dateTo
	and TDP.KPSWeek = @Week
	and TDP.KPSYear = @Year
	and TFR.EffectiveDate <= @dateFrom
	and TFR.ExpiredDate >= @dateTo

	and MTP.EffectiveDate <= @dateFrom
	and MTP.ExpiredDate >= @dateTo
	and MGL.ParentLocationCode = @Location
	GROUP BY TDP.TPOFeeCode,TDP.[KPSYear],TDP.[KPSWeek],MGL.LocationCode,TFR.BrandGroupCode
	,TFR.JKN, TFR.JL1, TFR.Jl2, TFR.Jl3, TFR.Jl4, TFR.ManagementFee, MGL.ParentLocationCode,MTP.Package,MGBG.EmpPackage,MGBG.StickPerBox, PSLV.StdStickPerHour
	ORDER BY MGL.LocationCode
	END
	
	SELECT DISTINCT
	'LocationCode' as LocationCode,
	'Type' as Type,
	'Company' as Company,
	'Currency' as Currency,
	'Exchange Rate' as ExchangeRate,
	'Document Type' as DocumentType,
	'Translation Date' as TranslationDate,
	'Header Text' as HeaderText,
	'Reference' as Reference,
	'CC Transaction' as CCTransaction,
	'Document Date' as DocumentDate,
	'Posting Date' as PostingDate,
	'Automatic Tax' as AutomaticTax,
	'Posting Key' as PostingKey,
	'Account' as Account,
	'Doc Cur Amount' as DocCurAmount,
	'Local Cur Amount' as LocalCurAmount,
	'Local Currency' as LocalCurrency,
	'Tax Code' as TaxCode,
	'PO Number' as PONumber,
	'PO Item Number' as POItemNumber,
	'Quantity' as Quantity,
	'UOM' as UOM,
	'Assignment' as Assignment,
	'Text' as Text,
	'Special GL Indicator' as SpecialGLIndicator,
	'Recovery Indicator' as RecoveryIndicator,
	'Customer' as Customer,
	'Baseline Date' as BaselineDate,
	'Value Date' as ValueDate,
	'Cost Center' as CostCenter,
	'WBS' as WBS,
	'Material Number' as MaterialNumber,
	'Brand Family' as BrandFamily,
	'Payment Terms' as PaymentTerms,
	'Cash Discount 1' as CashDiscount1,
	'Trading Partner' as TradingPartner,
	'New Company' as NewCompany,
	'Interco Affiliate' as IntercoAffiliate,
	'Production Center' as ProductionCenter,
	'PMI Market' as PMIMarket,
	'Product Category' as ProductCategory,
	'Ship-To' as ShipTo,
	'Label' as Label,
	'Final Market' as FinalMarket,
	'DocNumber-EarMarked Funds' as DocNumberEarMarkedFunds,
	'EarMarked Funds' as EarMarkedFunds,
	'Tax Based Amount' as TaxBasedAmount,
	'Withholding Tax Base Amount' as WithholdingTaxBaseAmount,
	'Batch Number' as BatchNumber,
	'Business Place' as BusinessPlace,
	'Section Code' as SectionCode,
	'Amount in 2nd Local Currency' as AmountIn2ndLocalCurrency,
	'Amount in 3rd Local Currency' as AmountIn3ndLocalCurrency,
	'W.Tax Code' as WTaxCode

	UNION ALL

	SELECT DISTINCT
	'' as LocationCode,
	'1' as Type,
	'3066' as Company,
	'IDR' as Currency,
	'' as ExchangeRate,
	'SC' as DocumentType,
	'' as TranslationDate,
	('AccrTPOFee'+ SUBSTRING(DATENAME(MONTH,dateadd(month, 0,@dateFrom)),1,3)+'(w.'+CONVERT(VARCHAR(2),@Week)+')'+CONVERT(VARCHAR(4),@Year)) as HeaderText,
	'3066' + CONVERT(varchar(4),(SELECT DATEPART( YY, @dateTo))) + CONVERT(VARCHAR(2), @dateTo, 101) as Reference,
	'' as CCTransaction,
	CONVERT(VARCHAR(2),DATEPART(DD,@dateFrom)) +'.'+CONVERT(VARCHAR(2), @dateFrom, 101)+'.'+SUBSTRING( CONVERT(varchar(4),(SELECT DATEPART( YY, @dateFrom))),3,2) as DocumentDate,
	CONVERT(VARCHAR(2),DATEPART(DD,@dateFrom)) +'.'+CONVERT(VARCHAR(2), @dateFrom, 101)+'.'+SUBSTRING( CONVERT(varchar(4),(SELECT DATEPART( YY, @dateFrom))),3,2) as PostingDate,
	'' as AutomaticTex,
	'' as PostingKey,
	'' as Account,
	'' as DocCurAmount,
	'' as LocalCurAmount,
	'' as LocalCurrency,
	'' as TaxCode,
	'' as PONumber,
	'' as POItemNumber,
	'' as Quantity,
	'' as UOM,
	'' as Assignment,
	'' as Text,
	'' as SpecialGLIndicator,
	'' as RecoveryIndicator,
	'' as Customer,
	'' as BaselineDate,
	'' as ValueDate,
	'' as CostCenter,
	'' as WBS,
	'' as MaterialNumber,
	'' as BrandFamily,
	'' as PaymentTerms,
	'' as CashDiscount1,
	'' as TradingPartner,
	'' as NewCompany,
	'' as IntercoAffiliate,
	'' as ProductionCenter,
	'' as PMIMarket,
	'' as ProductCategory,
	'' as ShipTo,
	'' as Label,
	'' as FinalMarket,
	'' as DocNumberEarMarkedFunds,
	'' as EarMarkedFunds,
	'' as TaxBasedAmount,
	'' as WithholdingTaxBaseAmount,
	'' as BatchNumber,
	'' as BusinessPlace,
	'' as SectionCode,
	'' as AmountIn2ndLocalCurrency,
	'' as AmountIn3ndLocalCurrency,
	'' as WTaxCode

	UNION ALL

	SELECT DISTINCT
	'' as LocationCode,
	'2' as Type,
	'' as Company,
	'' as Currency,
	'' as ExchangeRate,
	'' as DocumentType,
	'' as TranslationDate,
	'' as HeaderText,
	'' as Reference,
	'' as CCTransaction,
	'' as DocumentDate,
	'' as PostingDate,
	'' as AutomaticTex,
	'50' as PostingKey,
	'22561000' as Account,
	LTRIM(str(SUM(BiayaProduksi),25,0)) as DocCurAmount,
	'' as LocalCurAmount,
	'' as LocalCurrency,
	'V0' as TaxCode,
	'' as PONumber,
	'' as POItemNumber,
	'' as Quantity,
	'' as UOM,
	'' as Assignment,
	('AccrTPOFee'+ SUBSTRING(DATENAME(MONTH,dateadd(month, 0,@dateFrom)),1,3)+'(w.'+CONVERT(VARCHAR(2),@Week)+')'+CONVERT(VARCHAR(4),@Year)+'-Prod') as Text,
	'' as SpecialGLIndicator,
	'' as RecoveryIndicator,
	'' as Customer,
	'' as BaselineDate,
	'' as ValueDate,
	'' as CostCenter,
	'' as WBS,
	'' as MaterialNumber,
	'' as BrandFamily,
	'' as PaymentTerms,
	'' as CashDiscount1,
	'' as TradingPartner,
	'' as NewCompany,
	'' as IntercoAffiliate,
	'' as ProductionCenter,
	'' as PMIMarket,
	'' as ProductCategory,
	'' as ShipTo,
	'' as Label,
	'' as FinalMarket,
	'' as DocNumberEarMarkedFunds,
	'' as EarMarkedFunds,
	'' as TaxBasedAmount,
	'' as WithholdingTaxBaseAmount,
	'' as BatchNumber,
	'' as BusinessPlace,
	'' as SectionCode,
	'' as AmountIn2ndLocalCurrency,
	'' as AmountIn3ndLocalCurrency,
	'' as WTaxCode
	FROM @TempTable1

	UNION ALL

	SELECT DISTINCT
	'' as LocationCode,
	'2' as Type,
	'' as Company,
	'' as Currency,
	'' as ExchangeRate,
	'' as DocumentType,
	'' as TranslationDate,
	'' as HeaderText,
	'' as Reference,
	'' as CCTransaction,
	'' as DocumentDate,
	'' as PostingDate,
	'' as AutomaticTex,
	'50' as PostingKey,
	'22561000' as Account,
	LTRIM(str(SUM(JasaMaklon),25,0)) as DocCurAmount,
	'' as LocalCurAmount,
	'' as LocalCurrency,
	'V0' as TaxCode,
	'' as PONumber,
	'' as POItemNumber,
	'' as Quantity,
	'' as UOM,
	'' as Assignment,
	('AccrTPOFee'+ SUBSTRING(DATENAME(MONTH,dateadd(month, 0,@dateFrom)),1,3)+'(w.'+CONVERT(VARCHAR(2),@Week)+')'+CONVERT(VARCHAR(4),@Year)+'-Maklon') as Text,
	'' as SpecialGLIndicator,
	'' as RecoveryIndicator,
	'' as Customer,
	'' as BaselineDate,
	'' as ValueDate,
	'' as CostCenter,
	'' as WBS,
	'' as MaterialNumber,
	'' as BrandFamily,
	'' as PaymentTerms,
	'' as CashDiscount1,
	'' as TradingPartner,
	'' as NewCompany,
	'' as IntercoAffiliate,
	'' as ProductionCenter,
	'' as PMIMarket,
	'' as ProductCategory,
	'' as ShipTo,
	'' as Label,
	'' as FinalMarket,
	'' as DocNumberEarMarkedFunds,
	'' as EarMarkedFunds,
	'' as TaxBasedAmount,
	'' as WithholdingTaxBaseAmount,
	'' as BatchNumber,
	'' as BusinessPlace,
	'' as SectionCode,
	'' as AmountIn2ndLocalCurrency,
	'' as AmountIn3ndLocalCurrency,
	'' as WTaxCode
	FROM @TempTable1

	UNION ALL

	SELECT
	a.LocationCode,
	'2' as Type,
	'' as Company,
	'' as Currency,
	'' as ExchangeRate,
	'' as DocumentType,
	'' as TranslationDate,
	'' as HeaderText,
	'' as Reference,
	'' as CCTransaction,
	'' as DocumentDate,
	'' as PostingDate,
	'' as AutomaticTex,
	'40' as PostingKey,
	'82182000' as Account,
	LTRIM(str(b.BiayaProduksi,25,0)) AS DocCurAmount,
	'' as LocalCurAmount,
	'' as LocalCurrency,
	'V0' as TaxCode,
	'' as PONumber,
	'' as POItemNumber,
	'' as Quantity,
	'' as UOM,
	'' as Assignment,
	(c.SKTBrandCode +' - '+ a.LocationCode +' '+ 
	CONVERT(VARCHAR(2),DATEPART(DD,@dateFrom)) +'.'+CONVERT(VARCHAR(2), @dateFrom, 101)+'.'+SUBSTRING( CONVERT(varchar(4),(SELECT DATEPART( YY, @dateFrom))),3,2) +'-'+
	CONVERT(VARCHAR(2),DATEPART(DD,@dateTo)) +'.'+CONVERT(VARCHAR(2), @dateTo, 101)+'.'+SUBSTRING( CONVERT(varchar(4),(SELECT DATEPART( YY, @dateTo))),3,2) + ' ' +
	'(w.'+CONVERT(VARCHAR(2),@Week)+') Prod Fee') as Text,
	'' as SpecialGLIndicator,
	'' as RecoveryIndicator,
	'' as Customer,
	'' as BaselineDate,
	'' as ValueDate,
	d.CostCenter,
	'' as WBS,
	'' as MaterialNumber,
	'' as BrandFamily,
	'' as PaymentTerms,
	'' as CashDiscount1,
	'' as TradingPartner,
	'' as NewCompany,
	'' as IntercoAffiliate,
	'' as ProductionCenter,
	'' as PMIMarket,
	'' as ProductCategory,
	'' as ShipTo,
	'' as Label,
	'' as FinalMarket,
	'' as DocNumberEarMarkedFunds,
	'' as EarMarkedFunds,
	'' as TaxBasedAmount,
	'' as WithholdingTaxBaseAmount,
	'' as BatchNumber,
	'' as BusinessPlace,
	'' as SectionCode,
	'' as AmountIn2ndLocalCurrency,
	'' as AmountIn3ndLocalCurrency,
	'' as WTaxCode
	FROM TPOFeeHdrPlan a 
	INNER JOIN @TempTable1 b
	ON a.TPOFeeCode = b.TPOFeeCode
	INNER JOIN [MstGenBrandGroup] c
	ON a.[BrandGroupCode]=c.[BrandGroupCode]
	INNER JOIN [MstGenLocation] d
	ON a.LocationCode=d.LocationCode
	WHERE a.KPSWeek = @Week 
	AND a.KPSYear = @Year

	UNION ALL

	SELECT
	a.LocationCode,
	'2' as Type,
	'' as Company,
	'' as Currency,
	'' as ExchangeRate,
	'' as DocumentType,
	'' as TranslationDate,
	'' as HeaderText,
	'' as Reference,
	'' as CCTransaction,
	'' as DocumentDate,
	'' as PostingDate,
	'' as AutomaticTex,
	'40' as PostingKey,
	'82182010' as Account,
	LTRIM(str(a.JasaMaklon,25,0)) as DocCurAmount,
	'' as LocalCurAmount,
	'' as LocalCurrency,
	'V0' as TaxCode,
	'' as PONumber,
	'' as POItemNumber,
	'' as Quantity,
	'' as UOM,
	'' as Assignment,
	(c.SKTBrandCode +' - '+ a.LocationCode +' '+ 
	CONVERT(VARCHAR(2),DATEPART(DD,@dateFrom)) +'.'+CONVERT(VARCHAR(2), @dateFrom, 101)+'.'+SUBSTRING( CONVERT(varchar(4),(SELECT DATEPART( YY, @dateFrom))),3,2) +'-'+
	CONVERT(VARCHAR(2),DATEPART(DD,@dateTo)) +'.'+CONVERT(VARCHAR(2), @dateTo, 101)+'.'+SUBSTRING( CONVERT(varchar(4),(SELECT DATEPART( YY, @dateTo))),3,2) + ' ' +
	'(w.'+CONVERT(VARCHAR(2),@Week)+') Mgmt Fee') as Text,
	'' as SpecialGLIndicator,
	'' as RecoveryIndicator,
	'' as Customer,
	'' as BaselineDate,
	'' as ValueDate,
	d.CostCenter,
	'' as WBS,
	'' as MaterialNumber,
	'' as BrandFamily,
	'' as PaymentTerms,
	'' as CashDiscount1,
	'' as TradingPartner,
	'' as NewCompany,
	'' as IntercoAffiliate,
	'' as ProductionCenter,
	'' as PMIMarket,
	'' as ProductCategory,
	'' as ShipTo,
	'' as Label,
	'' as FinalMarket,
	'' as DocNumberEarMarkedFunds,
	'' as EarMarkedFunds,
	'' as TaxBasedAmount,
	'' as WithholdingTaxBaseAmount,
	'' as BatchNumber,
	'' as BusinessPlace,
	'' as SectionCode,
	'' as AmountIn2ndLocalCurrency,
	'' as AmountIn3ndLocalCurrency,
	'' as WTaxCode
	FROM @TempTable1 a
	INNER JOIN [MstGenBrandGroup] c
	ON a.[BrandGroupCode]=c.[BrandGroupCode]
	INNER JOIN [MstGenLocation] d
	ON a.LocationCode=d.LocationCode;

END
