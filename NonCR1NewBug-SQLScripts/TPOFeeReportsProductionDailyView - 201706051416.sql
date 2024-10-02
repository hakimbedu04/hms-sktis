-- =============================================================================
-- Description: change join to msttpofeepackage 
-- Ticket: http://tp.voxteneo.co.id/entity/3685
-- Author: hakim
-- UpdatedDate: 5/6/2017
-- version : 2.0
-- =============================================================================


	ALTER view [dbo].[TPOFeeReportsProductionDailyView]
	as
	-- comment according to version 2.0 (unused)
	/*SELECT
		hdr.TPOFeeCode,
		d.FeeDate,
		l.ParentLocationCode AS Regional, 
		hdr.LocationCode AS LocationCode,
		l.ABBR AS LocationAbbr,
		l.LocationName,
		l.UMK,
		hdr.BrandGroupCode as Brand,
		COALESCE(p.Package,0) as Package,
		COALESCE(d.JKNRP, 0) AS JKNProductionFee,
		COALESCE(d.JL1RP, 0) AS JL1ProductionFee,
		COALESCE(d.JL2RP, 0) AS JL2ProductionFee,
		COALESCE(d.JL3RP, 0) AS JL3ProductionFee,
		COALESCE(d.JL4RP, 0) AS JL4ProductionFee,
		COALESCE(mtr.managementfee*(d.jknboxfinal+d.jl1boxfinal+d.jl2boxfinal+d.jl3boxfinal+d.jl4boxfinal), 0) AS ManagementFee,
		COALESCE(mtr.ProductivityIncentives, 0) AS ProductivityIncentives,
		hdr.KPSYear AS Year,
		hdr.KPSWeek AS Week,
		p.EffectiveDate AS StartDate,
		p.ExpiredDate AS EndDate,
		p.MemoRef as NoMemo,
		COALESCE(d.jknboxfinal, 0) AS JKNProductionVolume,
		COALESCE(d.jl1boxfinal, 0) AS JL1ProductionVolume,
		COALESCE(d.jl2boxfinal, 0) AS JL2ProductionVolume,
		COALESCE(d.jl3boxfinal, 0) AS JL3ProductionVolume,
		COALESCE(d.jl4boxfinal, 0) AS JL4ProductionVolume
	FROM TPOFeeHdr hdr
	JOIN MstGenLocation l ON hdr.LocationCode = l.LocationCode
	INNER JOIN TPOFeeProductionDaily d ON hdr.TPOFeeCode = d.TPOFeeCode 
	LEFT JOIN MstTPOPackage p ON 
		--DATEPART(YEAR, p.EffectiveDate) <= hdr.KPSYear 
		--AND DATEPART(WEEK, p.EffectiveDate) <= hdr.KPSWeek
		--AND DATEPART(YEAR, p.ExpiredDate) >= hdr.KPSYear
		--AND DATEPART(WEEK, p.ExpiredDate) >= hdr.KPSWeek
		--AND p.BrandGroupCode=hdr.BrandGroupCode 
		CAST(CAST(DATEPART(YEAR, p.EffectiveDate) as varchar)+RIGHT('00'+CAST(DATEPART(WEEK, p.EffectiveDate) as varchar),2) AS INT)<=CAST(CAST(hdr.KPSYear AS VARCHAR)+RIGHT('00'+CAST(hdr.KPSWeek AS VARCHAR),2) AS int)
		AND CAST(CAST(DATEPART(YEAR, p.ExpiredDate) as varchar)+RIGHT('00'+CAST(DATEPART(WEEK, p.ExpiredDate) as varchar),2) AS INT)>=CAST(CAST(hdr.KPSYear AS VARCHAR)+RIGHT('00'+CAST(hdr.KPSWeek AS VARCHAR),2) AS int)
		AND p.BrandGroupCode=hdr.BrandGroupCode 
		AND p.locationcode=hdr.LocationCode
	LEFT JOIN dbo.MstTPOFeeRate mtr ON mtr.LocationCode = hdr.LocationCode
		AND mtr.BrandGroupCode = hdr.BrandGroupCode 
		AND CAST(CAST(DATEPART(YEAR, mtr.EffectiveDate) as varchar)+RIGHT('00'+CAST(DATEPART(WEEK, mtr.EffectiveDate) as varchar),2) AS INT)<=CAST(CAST(hdr.KPSYear AS VARCHAR)+RIGHT('00'+CAST(hdr.KPSWeek AS VARCHAR),2) AS int)
		AND CAST(CAST(DATEPART(YEAR, mtr.ExpiredDate) as varchar)+RIGHT('00'+CAST(DATEPART(WEEK, mtr.ExpiredDate) as varchar),2) AS INT)>=CAST(CAST(hdr.KPSYear AS VARCHAR)+RIGHT('00'+CAST(hdr.KPSWeek AS VARCHAR),2) AS int)
		--AND DATEPART(YEAR, mtr.EffectiveDate) <= hdr.KPSYear 
		--AND DATEPART(WEEK, mtr.EffectiveDate) <= hdr.KPSWeek
		--AND DATEPART(YEAR, mtr.ExpiredDate) >= hdr.KPSYear
		--AND DATEPART(WEEK, mtr.ExpiredDate) >= hdr.KPSWeek
	WHERE d.jknrp<>0 or d.jl2rp<>0
	UNION ALL
	SELECT
		hdr.TPOFeeCode,
		d.FeeDate,
		l.ParentLocationCode AS Regional, 
		hdr.LocationCode AS Location,
		l.ABBR AS LocationCode,
		l.LocationName,
		l.UMK,
		hdr.BrandGroupCode,
		COALESCE(p.Package,0),
		--mtr.managementfee,
		COALESCE(d.JKNRP, 0) AS JKN,
		COALESCE(d.JL1RP, 0) AS JL1,
		COALESCE(d.JL2RP, 0) AS JL2,
		COALESCE(d.JL3RP, 0) AS JL3,
		COALESCE(d.JL4RP, 0) AS JL4,
		COALESCE(mtr.managementfee*(d.jkn+d.jl1+d.jl2+d.jl3+d.jl4), 0) AS JasaManajemen,
		COALESCE(mtr.ProductivityIncentives, 0) AS ProductivityIncentives,
		hdr.KPSYear AS Year,
		hdr.KPSWeek AS Week,
		p.EffectiveDate AS StartDate,
		p.ExpiredDate AS EndDate,
		p.MemoRef,
		COALESCE(d.jkn, 0) AS JKNBox,
		COALESCE(d.jl1, 0) AS Jl1Box,
		COALESCE(d.jl2, 0) AS Jl2Box,
		COALESCE(d.jl3, 0) AS Jl3Box,
		COALESCE(d.jl4, 0) AS Jl4Box
	FROM TPOFeeHdrPlan hdr
	JOIN MstGenLocation l ON hdr.LocationCode = l.LocationCode
	INNER JOIN TPOFeeProductionDailyPlan d ON hdr.TPOFeeCode = d.TPOFeeCode 
	LEFT JOIN MstTPOPackage p ON 
		--DATEPART(YEAR, p.EffectiveDate) <= hdr.KPSYear 
		--AND DATEPART(WEEK, p.EffectiveDate) <= hdr.KPSWeek
		--AND DATEPART(YEAR, p.ExpiredDate) >= hdr.KPSYear
		--AND DATEPART(WEEK, p.ExpiredDate) >= hdr.KPSWeek
		CAST(CAST(DATEPART(YEAR, p.EffectiveDate) as varchar)+RIGHT('00'+CAST(DATEPART(WEEK, p.EffectiveDate) as varchar),2) AS INT)<=CAST(CAST(hdr.KPSYear AS VARCHAR)+RIGHT('00'+CAST(hdr.KPSWeek AS VARCHAR),2) AS int)
		AND CAST(CAST(DATEPART(YEAR, p.ExpiredDate) as varchar)+RIGHT('00'+CAST(DATEPART(WEEK, p.ExpiredDate) as varchar),2) AS INT)>=CAST(CAST(hdr.KPSYear AS VARCHAR)+RIGHT('00'+CAST(hdr.KPSWeek AS VARCHAR),2) AS int)
		AND p.BrandGroupCode=hdr.BrandGroupCode 
		AND p.locationcode=hdr.LocationCode 
	LEFT JOIN dbo.MstTPOFeeRate mtr ON mtr.LocationCode = hdr.LocationCode
		AND mtr.BrandGroupCode = hdr.BrandGroupCode
		AND CAST(CAST(DATEPART(YEAR, mtr.EffectiveDate) as varchar)+RIGHT('00'+CAST(DATEPART(WEEK, mtr.EffectiveDate) as varchar),2) AS INT)<=CAST(CAST(hdr.KPSYear AS VARCHAR)+RIGHT('00'+CAST(hdr.KPSWeek AS VARCHAR),2) AS int)
		AND CAST(CAST(DATEPART(YEAR, mtr.ExpiredDate) as varchar)+RIGHT('00'+CAST(DATEPART(WEEK, mtr.ExpiredDate) as varchar),2) AS INT)>=CAST(CAST(hdr.KPSYear AS VARCHAR)+RIGHT('00'+CAST(hdr.KPSWeek AS VARCHAR),2) AS int)
		
		--AND DATEPART(YEAR, mtr.EffectiveDate) <= hdr.KPSYear 
		--AND DATEPART(WEEK, mtr.EffectiveDate) <= hdr.KPSWeek
		--AND DATEPART(YEAR, mtr.ExpiredDate) >= hdr.KPSYear
		--AND DATEPART(WEEK, mtr.ExpiredDate) >= hdr.KPSWeek
	LEFT JOIN TPOFeeProductionDaily e ON d.TPOFeeCode=e.TPOFeeCode AND d.FeeDate=e.FeeDate WHERE (e.jknrp=0 and e.JL2Rp=0) or (e.jknrp is null and e.JL2Rp is null)
	*/

	-- use this LineOfCode according to version 2.0
	SELECT
		hdr.TPOFeeCode,
		d.FeeDate,
		l.ParentLocationCode AS Regional, 
		hdr.LocationCode AS LocationCode,
		l.ABBR AS LocationAbbr,
		l.LocationName,
		COALESCE(l.UMK, 0) AS UMK,
		hdr.BrandGroupCode as Brand,
		COALESCE(p.Package,0) as Package,
		COALESCE(d.JKNRP, 0) AS JKNProductionFee,
		COALESCE(d.JL1RP, 0) AS JL1ProductionFee,
		COALESCE(d.JL2RP, 0) AS JL2ProductionFee,
		COALESCE(d.JL3RP, 0) AS JL3ProductionFee,
		COALESCE(d.JL4RP, 0) AS JL4ProductionFee,
		COALESCE(mtr.managementfee*(d.jknboxfinal+d.jl1boxfinal+d.jl2boxfinal+d.jl3boxfinal+d.jl4boxfinal), 0) AS ManagementFee,
		COALESCE(mtr.ProductivityIncentives, 0) AS ProductivityIncentives,
		hdr.KPSYear AS Year,
		hdr.KPSWeek AS Week,
		p.EffectiveDate AS StartDate,
		p.ExpiredDate AS EndDate,
		p.MemoRef as NoMemo,
		COALESCE(d.jknboxfinal, 0) AS JKNProductionVolume,
		COALESCE(d.jl1boxfinal, 0) AS JL1ProductionVolume,
		COALESCE(d.jl2boxfinal, 0) AS JL2ProductionVolume,
		COALESCE(d.jl3boxfinal, 0) AS JL3ProductionVolume,
		COALESCE(d.jl4boxfinal, 0) AS JL4ProductionVolume
	FROM TPOFeeHdr hdr
	JOIN MstGenLocation l ON hdr.LocationCode = l.LocationCode
	INNER JOIN TPOFeeProductionDaily d ON hdr.TPOFeeCode = d.TPOFeeCode 
	LEFT JOIN MstTPOPackage p ON p.BrandGroupCode=hdr.BrandGroupCode 
		AND p.locationcode=hdr.LocationCode
	INNER JOIN dbo.MstTPOFeeRate mtr ON mtr.LocationCode = hdr.LocationCode
		AND mtr.BrandGroupCode = hdr.BrandGroupCode 
		AND d.FeeDate >= mtr.EffectiveDate and d.FeeDate <= mtr.ExpiredDate
		AND d.FeeDate >= p.EffectiveDate and d.FeeDate <= p.ExpiredDate
	WHERE d.jknrp<>0 or d.jl2rp<>0
	UNION ALL
	SELECT
		hdr.TPOFeeCode,
		d.FeeDate,
		l.ParentLocationCode AS Regional, 
		hdr.LocationCode AS Location,
		l.ABBR AS LocationCode,
		l.LocationName,
		COALESCE(l.UMK, 0) AS UMK,
		hdr.BrandGroupCode,
		COALESCE(p.Package,0),
		--mtr.managementfee,
		COALESCE(d.JKNRP, 0) AS JKN,
		COALESCE(d.JL1RP, 0) AS JL1,
		COALESCE(d.JL2RP, 0) AS JL2,
		COALESCE(d.JL3RP, 0) AS JL3,
		COALESCE(d.JL4RP, 0) AS JL4,
		COALESCE(mtr.managementfee*(d.jkn+d.jl1+d.jl2+d.jl3+d.jl4), 0) AS JasaManajemen,
		COALESCE(mtr.ProductivityIncentives, 0) AS ProductivityIncentives,
		hdr.KPSYear AS Year,
		hdr.KPSWeek AS Week,
		p.EffectiveDate AS StartDate,
		p.ExpiredDate AS EndDate,
		p.MemoRef,
		COALESCE(d.jkn, 0) AS JKNBox,
		COALESCE(d.jl1, 0) AS Jl1Box,
		COALESCE(d.jl2, 0) AS Jl2Box,
		COALESCE(d.jl3, 0) AS Jl3Box,
		COALESCE(d.jl4, 0) AS Jl4Box
	FROM TPOFeeHdrPlan hdr
	JOIN MstGenLocation l ON hdr.LocationCode = l.LocationCode
	INNER JOIN TPOFeeProductionDailyPlan d ON hdr.TPOFeeCode = d.TPOFeeCode 
	LEFT JOIN MstTPOPackage p ON p.BrandGroupCode=hdr.BrandGroupCode 
		AND p.locationcode=hdr.LocationCode 
	INNER JOIN dbo.MstTPOFeeRate mtr ON mtr.LocationCode = hdr.LocationCode
		AND mtr.BrandGroupCode = hdr.BrandGroupCode
		AND d.FeeDate >= mtr.EffectiveDate and d.FeeDate <= mtr.ExpiredDate
		and d.FeeDate >= p.EffectiveDate and d.FeeDate <= p.ExpiredDate
	LEFT JOIN TPOFeeProductionDaily e ON d.TPOFeeCode=e.TPOFeeCode AND d.FeeDate=e.FeeDate WHERE (e.jknrp=0 and e.JL2Rp=0) or (e.jknrp is null and e.JL2Rp is null)
