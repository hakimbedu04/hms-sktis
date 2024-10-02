-- Description: Weekly for report
-- Ticket: http://tp.voxteneo.co.id/entity/3524
-- Author: Dwi Yudha

ALTER VIEW [dbo].[TPOFeeReportsProductionWeeklyView]
AS

	
	select distinct
	loc.ParentLocationCode as 'Regional'
		, hdr.LocationCode as 'Location'
		, loc.ABBR as 'LocationCode'
		, loc.LocationName
		, loc.UMK 
		, hdr.BrandGroupCode
		, ISNULL(pac.Package, 0) as 'Package'
		, COALESCE(hdr.TotalProdJKN * mtr.JKN, 0) AS JKN
		, COALESCE(hdr.TotalProdJl1 * mtr.JL1, 0) AS JL1
		, COALESCE(hdr.TotalProdJl2 * mtr.JL2, 0) AS JL2
		, COALESCE(hdr.TotalProdJl3 * mtr.JL3, 0) AS JL3
		, COALESCE(hdr.TotalProdJl4 * mtr.JL4, 0) AS JL4
		, CAST(COALESCE(mtr.ManagementFee, 0) * hdr.TotalProdBox as float) AS JasaManajemen
		, COALESCE(mtr.ProductivityIncentives, 0) AS ProductivityIncentives
		, hdr.KPSYear AS Year
		, hdr.KPSWeek AS Week
		, pac.EffectiveDate AS StartDate
		, pac.ExpiredDate AS EndDate
		, pac.MemoRef
		, COALESCE(hdr.TotalProdJKN, 0) AS TotalProdJKN
		, COALESCE(hdr.TotalProdJl1, 0) AS TotalProdJl1
		, COALESCE(hdr.TotalProdJl2, 0) AS TotalProdJl2
		, COALESCE(hdr.TotalProdJl3, 0) AS TotalProdJl3
		, COALESCE(hdr.TotalProdJl4, 0) AS TotalProdJl4
	FROM TPOFeeHdr hdr
	INNER JOIN MstGenLocation loc ON loc.LocationCode = hdr.LocationCode
	INNER JOIN MstTPOPackage pac ON hdr.LocationCode = pac.LocationCode
	AND hdr.BrandGroupCode = pac.BrandGroupCode
	INNER JOIN dbo.MstTPOFeeRate mtr ON mtr.LocationCode = hdr.LocationCode
	AND mtr.BrandGroupCode = hdr.BrandGroupCode 
	AND ((SELECT StartDate FROM MstGenWeek WHERE Year = hdr.KPSYear and Week = hdr.KPSWeek) BETWEEN pac.EffectiveDate AND pac.ExpiredDate
	OR (SELECT enddate FROM MstGenWeek WHERE Year = hdr.KPSYear and Week = hdr.KPSWeek) BETWEEN pac.EffectiveDate AND pac.ExpiredDate)
	AND ((SELECT StartDate FROM MstGenWeek WHERE Year = hdr.KPSYear and Week = hdr.KPSWeek) BETWEEN mtr.EffectiveDate AND mtr.ExpiredDate
	OR (SELECT enddate FROM MstGenWeek WHERE Year = hdr.KPSYear and Week = hdr.KPSWeek) BETWEEN mtr.EffectiveDate AND mtr.ExpiredDate)



	--select distinct
	--loc.ParentLocationCode as 'Regional'
	--	, hdr.LocationCode as 'Location'
	--	, loc.ABBR as 'LocationCode'
	--	, loc.LocationName
	--	, loc.UMK 
	--	, hdr.BrandGroupCode
	--	, ISNULL(pac.Package, 0) as 'Package'
	--	, COALESCE(hdr.TotalProdJKN * mtr.JKN, 0) AS JKN
	--	, COALESCE(hdr.TotalProdJl1 * mtr.JL1, 0) AS JL1
	--	, COALESCE(hdr.TotalProdJl2 * mtr.JL2, 0) AS JL2
	--	, COALESCE(hdr.TotalProdJl3 * mtr.JL3, 0) AS JL3
	--	, COALESCE(hdr.TotalProdJl4 * mtr.JL4, 0) AS JL4
	--	, CAST(COALESCE(mtr.ManagementFee, 0) * hdr.TotalProdBox as float) AS JasaManajemen
	--	, COALESCE(mtr.ProductivityIncentives, 0) AS ProductivityIncentives
	--	, hdr.KPSYear AS Year
	--	, hdr.KPSWeek AS Week
	--	, pac.EffectiveDate AS StartDate
	--	, pac.ExpiredDate AS EndDate
	--	, pac.MemoRef
	--	, COALESCE(hdr.TotalProdJKN, 0) AS TotalProdJKN
	--	, COALESCE(hdr.TotalProdJl1, 0) AS TotalProdJl1
	--	, COALESCE(hdr.TotalProdJl2, 0) AS TotalProdJl2
	--	, COALESCE(hdr.TotalProdJl3, 0) AS TotalProdJl3
	--	, COALESCE(hdr.TotalProdJl4, 0) AS TotalProdJl4
	--FROM TPOFeeHdr hdr
	--INNER JOIN MstGenLocation loc ON loc.LocationCode = hdr.LocationCode
	--INNER JOIN MstTPOPackage pac ON hdr.LocationCode = pac.LocationCode
	--AND hdr.BrandGroupCode = pac.BrandGroupCode
	--INNER JOIN dbo.MstTPOFeeRate mtr ON mtr.LocationCode = hdr.LocationCode
	--AND mtr.BrandGroupCode = hdr.BrandGroupCode 
	--AND ((SELECT StartDate FROM MstGenWeek WHERE Year = hdr.KPSYear and Week = hdr.KPSWeek) BETWEEN pac.EffectiveDate AND pac.ExpiredDate
	--OR (SELECT enddate FROM MstGenWeek WHERE Year = hdr.KPSYear and Week = hdr.KPSWeek) BETWEEN pac.EffectiveDate AND pac.ExpiredDate)
	--AND ((SELECT StartDate FROM MstGenWeek WHERE Year = hdr.KPSYear and Week = hdr.KPSWeek) BETWEEN mtr.EffectiveDate AND mtr.ExpiredDate
	--OR (SELECT enddate FROM MstGenWeek WHERE Year = hdr.KPSYear and Week = hdr.KPSWeek) BETWEEN mtr.EffectiveDate AND mtr.ExpiredDate)

--	SELECT distinct
--	loc.ParentLocationCode as 'Regional'
--	, hdr.LocationCode as 'Location'
--	, loc.ABBR as 'LocationCode'
--	, loc.LocationName
--	, loc.UMK 
--	, hdr.BrandGroupCode
--	, ISNULL(pac.Package, 0) as 'Package'
--	, COALESCE(hdr.TotalProdJKN * mtr.JKN, 0) AS JKN
--	, COALESCE(hdr.TotalProdJl1 * mtr.JL1, 0) AS JL1
--	, COALESCE(hdr.TotalProdJl2 * mtr.JL2, 0) AS JL2
--	, COALESCE(hdr.TotalProdJl3 * mtr.JL3, 0) AS JL3
--	, COALESCE(hdr.TotalProdJl4 * mtr.JL4, 0) AS JL4
--	, CAST(COALESCE(mtr.ManagementFee, 0) * hdr.TotalProdBox as float) AS JasaManajemen
--	, COALESCE(mtr.ProductivityIncentives, 0) AS ProductivityIncentives
--	, hdr.KPSYear AS Year
--	, hdr.KPSWeek AS Week
--	, pac.EffectiveDate AS StartDate
--	, pac.ExpiredDate AS EndDate
--	, pac.MemoRef
--	, COALESCE(hdr.TotalProdJKN, 0) AS TotalProdJKN
--	, COALESCE(hdr.TotalProdJl1, 0) AS TotalProdJl1
--	, COALESCE(hdr.TotalProdJl2, 0) AS TotalProdJl2
--	, COALESCE(hdr.TotalProdJl3, 0) AS TotalProdJl3
--	, COALESCE(hdr.TotalProdJl4, 0) AS TotalProdJl4
--FROM TPOFeeHdr hdr
--INNER JOIN MstGenLocation loc on loc.LocationCode = hdr.LocationCode
--INNER JOIN MstTPOPackage pac ON pac.LocationCode = hdr.LocationCode
--		AND ((SELECT StartDate FROM MstGenWeek WHERE Year = hdr.KPSYear and Week = hdr.KPSWeek) BETWEEN pac.EffectiveDate AND pac.ExpiredDate
--OR (SELECT enddate FROM MstGenWeek WHERE Year = hdr.KPSYear and Week = hdr.KPSWeek) BETWEEN pac.EffectiveDate AND pac.ExpiredDate)
--INNER JOIN dbo.MstTPOFeeRate mtr ON mtr.LocationCode = hdr.LocationCode
--		AND mtr.BrandGroupCode = hdr.BrandGroupCode 
--		AND ((SELECT StartDate FROM MstGenWeek WHERE Year = hdr.KPSYear and Week = hdr.KPSWeek) BETWEEN mtr.EffectiveDate AND mtr.ExpiredDate
--OR (SELECT enddate FROM MstGenWeek WHERE Year = hdr.KPSYear and Week = hdr.KPSWeek) BETWEEN mtr.EffectiveDate AND mtr.ExpiredDate)



	--SELECT l.ParentLocationCode AS Regional, 
	--	hdr.LocationCode AS Location,
	--	l.ABBR AS LocationCode,
	--	l.LocationName,
	--	l.UMK,
	--	hdr.BrandGroupCode,
	--	p.Package,
	--	COALESCE(table1.JKN, 0) AS JKN,
	--	COALESCE(table1.JL1, 0) AS JL1,
	--	COALESCE(table1.JL2, 0) AS JL2,
	--	COALESCE(table1.JL3, 0) AS JL3,
	--	COALESCE(table1.JL4, 0) AS JL4,
	--	COALESCE(table1.JasaManajemen, 0) AS JasaManajemen,
	--	COALESCE(mtr.ProductivityIncentives, 0) AS ProductivityIncentives,
	--	hdr.KPSYear AS Year,
	--	hdr.KPSWeek AS Week,
	--	p.EffectiveDate AS StartDate,
	--	p.ExpiredDate AS EndDate,
	--	p.MemoRef,
	--	COALESCE(hdr.TotalProdJKN, 0) AS TotalProdJKN,
	--	COALESCE(hdr.TotalProdJl1, 0) AS TotalProdJl1,
	--	COALESCE(hdr.TotalProdJl2, 0) AS TotalProdJl2,
	--	COALESCE(hdr.TotalProdJl3, 0) AS TotalProdJl3,
	--	COALESCE(hdr.TotalProdJl4, 0) AS TotalProdJl4
	--FROM TPOFeeHdr hdr
	--JOIN MstGenLocation l ON hdr.LocationCode = l.LocationCode
	--LEFT JOIN MstTPOPackage p ON DATEPART(YEAR, p.EffectiveDate) <= hdr.KPSYear 
	--	AND DATEPART(WEEK, p.EffectiveDate) <= hdr.KPSWeek
	--	AND DATEPART(YEAR, p.ExpiredDate) >= hdr.KPSYear
	--	AND DATEPART(WEEK, p.ExpiredDate) >= hdr.KPSWeek
	--LEFT JOIN dbo.MstTPOFeeRate mtr ON mtr.LocationCode = hdr.LocationCode
	--	AND mtr.BrandGroupCode = hdr.BrandGroupCode 
	--	AND DATEPART(YEAR, mtr.EffectiveDate) <= hdr.KPSYear 
	--	AND DATEPART(WEEK, mtr.EffectiveDate) <= hdr.KPSWeek
	--	AND DATEPART(YEAR, mtr.ExpiredDate) >= hdr.KPSYear
	--	AND DATEPART(WEEK, mtr.ExpiredDate) >= hdr.KPSWeek
	--JOIN
	--( SELECT p.TPOFeeCode,
	--			   [JKN],
	--			   [JL1],
	--			   [JL2],
	--			   [JL3],
	--			   [JL4],
	--			   [Biaya Produksi] BiayaProduksi,
	--			   [Jasa Manajemen] JasaManajemen,
	--			   [Total Biaya Produksi & Jasa Manajemen] TotalBiayaProduksiJasaManajemen,
	--			   [Pajak Jasa Manajemen Sebesar 2%] PajakJasaManajemenSebesar2,
	--			   [Total Biaya Yang Harus Dibayarkan Ke MPS] TotalBiayaYangHarusDibayarkanKeMPS,
	--			   [Pembayaran] Pembayaran,
	--			   [Sisa yang harus dibayar] SisaYangHarusDibayar,
	--			   [PPN Biaya Produksi 10%] PPNBiayaProduksi,
	--			   [PPN Jasa Manajemen 10%] PPNJasaManajemen,
	--			   [Total Bayar] TotalBayar
	--		FROM (  SELECT p.TPOFeeCode,
	--					p.Calculate,
	--					p.ProductionFeeType
	--			  FROM dbo.TPOFeeCalculation AS p ) 
	--			  AS j PIVOT( SUM(Calculate) FOR ProductionFeeType IN( 
	--							[JKN],
	--							[JL1],
	--							[JL2],
	--							[JL3],
	--							[JL4],
	--							[Biaya Produksi],
	--							[Jasa Manajemen],
	--							[Total Biaya Produksi & Jasa Manajemen],
	--							[Pajak Jasa Manajemen Sebesar 2%],
	--							[Total Biaya Yang Harus Dibayarkan Ke MPS],
	--							[Pembayaran],
	--							[Sisa yang harus dibayar],
	--							[PPN Biaya Produksi 10%],
	--							[PPN Jasa Manajemen 10%],
	--							[Total Bayar] )) AS p ) AS table1
	--ON table1.TPOFeeCode = hdr.TPOFeeCode