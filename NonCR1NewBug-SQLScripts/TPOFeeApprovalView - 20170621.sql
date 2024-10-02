/****** Object:  View [dbo].[TPOFeeApprovalView]    Script Date: 6/21/2017 11:23:00 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO



-- Description: Flow need changes
-- Ticket: http://tp.voxteneo.co.id/entity/8618
-- Author: Dwi Yudha

-- change jasa manajement 2 percent

ALTER VIEW [dbo].[TPOFeeApprovalView]
AS
    SELECT mgl.LocationCode,
		mgl.LocationName,
		SKTBrandCode,
		'' Note,       
		table2.JKN,
		table2.JL1,
		table2.JL2,
		table2.JL3,
		table2.JL4,
		table1.BiayaProduksi,
		table1.JasaManajemen,
		COALESCE(mtr.ProductivityIncentives, 0) AS ProductivityIncentives,
		table1.PajakJasaManajemenSebesar2 AS JasaManajemen2Percent,
		(COALESCE(mtr.ProductivityIncentives, 0) * 2) / 100 ProductivityIncentives2Percent,
		table1.PPNBiayaProduksi AS BiayaProduksi10Percent,
		table1.PPNJasaManajemen AS JasaMakloon10Percent,
		(COALESCE(mtr.ProductivityIncentives, 0) * 10) / 100 ProductivityIncentives10Percent,
		table1.TotalBayar,
		th.KPSYear,
		th.KPSWeek,
		utl.IDFlow,
		table1.TPOFeeCode,
		CASE utl.IDFlow
			WHEN 40 THEN 'OPEN'
			WHEN 43 THEN 'REVISED'
			WHEN 47 THEN 'REVISED'
			WHEN 41 THEN 'DRAFT'
			WHEN 42 THEN 'SUBMITTED'
			WHEN 44 THEN 'APPROVED'
			WHEN 45 THEN 'AUTHORIZED'
			WHEN 46 THEN 'COMPLETED'
			WHEN 48 THEN 'END'
			WHEN 51 THEN 'END'
			ELSE 'OPEN'
		END AS Status,
		th.TaxtNoMgmt,
		th.TaxtNoProd,
		uf.DestinationRole, 
		func3.FunctionName AS DestinationFunctionForm, 
		CASE utl.IDFlow
		WHEN 40 THEN 'TAAPRD'
		ELSE roles2.RolesCode 
		END AS DestinationRolesCodes,
		utl.CreatedBy PIC,
        th.BrandGroupCode,
        th.TPOFeeTempID,
        th.CurrentApproval,
        th.CreatedDate,
        th.CreatedBy,
        th.UpdatedDate,
        th.UpdatedBy,
        th.Status AS Expr1,
        th.PengirimanL1,
        th.PengirimanL2,
        th.PengirimanL3,
        th.PengirimanL4,
        th.StickPerBox,
        th.TPOPackageValue,
        th.StickPerPackage,
        th.TotalProdStick,
        th.TotalProdBox,
        th.TotalProdJKN,
        th.TotalProdJl1,
        th.TotalProdJl2,
        th.TotalProdJl3,
        th.TotalProdJl4,
		mgl.ParentLocationCode
		FROM (SELECT p.TPOFeeCode,
			[Biaya Produksi] BiayaProduksi,
			[Jasa Manajemen] JasaManajemen,
			[Total Biaya Produksi & Jasa Manajemen] TotalBiayaProduksiJasaManajemen,
			[Pajak Jasa Manajemen Sebesar 2%] PajakJasaManajemenSebesar2,
			[Total Biaya Yang Harus Dibayarkan Ke MPS] TotalBiayaYangHarusDibayarkanKeMPS,
			[Pembayaran] Pembayaran,
			[Sisa yang harus dibayar] SisaYangHarusDibayar,
			[PPN Biaya Produksi 10%] PPNBiayaProduksi,
			[PPN Jasa Manajemen 10%] PPNJasaManajemen,
			[Total Bayar] TotalBayar
			FROM (  SELECT p.TPOFeeCode,
				p.Calculate,
				p.ProductionFeeType
				FROM dbo.TPOFeeCalculation AS p ) AS j PIVOT( SUM(Calculate) FOR ProductionFeeType IN( [Biaya Produksi],
					[Jasa Manajemen],
					[Total Biaya Produksi & Jasa Manajemen],
					[Pajak Jasa Manajemen Sebesar 2%],
					[Total Biaya Yang Harus Dibayarkan Ke MPS],
					[Pembayaran],
					[Sisa yang harus dibayar],
					[PPN Biaya Produksi 10%],
					[PPN Jasa Manajemen 10%],
					[Total Bayar] )) AS p ) AS table1
		INNER JOIN ( SELECT p.TPOFeeCode,
			[JKN],
			[JL1],
			[JL2],
			[JL3],
			[JL4]
			FROM (  SELECT p.TPOFeeCode,
				p.OutputProduction,
				p.ProductionFeeType
				FROM dbo.TPOFeeCalculation AS p ) AS j PIVOT( SUM(OutputProduction) FOR ProductionFeeType IN( [JKN],
					[JL1],
					[JL2],
					[JL3],
					[JL4] )) AS p ) AS table2 ON table2.TPOFeeCode = table1.TPOFeeCode
		INNER JOIN( SELECT MAX(TransactionDate) AS TransDate, TransactionCode AS TransCode
			FROM dbo.UtilTransactionLogs AS utl
			GROUP BY TransactionCode ) AS log ON table1.TPOFeeCode = log.TransCode
		LEFT JOIN dbo.UtilTransactionLogs AS utl ON log.TransDate = utl.TransactionDate
			AND log.TransCode = utl.TransactionCode
		INNER JOIN dbo.UtilFlows uf ON uf.IDFlow = utl.IDFlow
		INNER JOIN dbo.UtilFunctions AS func3 ON func3.IDFunction = uf.DestinationForm 
		INNER JOIN dbo.UtilRoles AS roles2 ON roles2.IDRole = uf.DestinationRole
		INNER JOIN dbo.TPOFeeHdr th ON th.TPOFeeCode = table1.TPOFeeCode
		INNER JOIN dbo.MstGenLocation AS mgl ON mgl.LocationCode = th.LocationCode
		INNER JOIN dbo.MstGenBrandGroup AS mgbg ON mgbg.BrandGroupCode = th.BrandGroupCode
		LEFT JOIN dbo.MstTPOFeeRate mtr ON mtr.LocationCode = th.LocationCode
			AND mtr.BrandGroupCode = th.BrandGroupCode AND mtr.EffectiveDate <= th.CreatedDate AND mtr.ExpiredDate >= th.CreatedDate;




