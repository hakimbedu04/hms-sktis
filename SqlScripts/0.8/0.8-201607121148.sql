-- Description: Coalesce ProductivityIncentives & change JKN~JL4 Value
-- Ticket: http://tp.voxteneo.co.id/entity/8336
-- Author: Dwi Yudha


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
		 (table1.JasaManajemen * 2) /100 JasaManajemen2Percent,
		 (COALESCE(mtr.ProductivityIncentives, 0) * 2) / 100 ProductivityIncentives2Percent,
		 (table1.BiayaProduksi * 10) / 100 AS BiayaProduksi10Percent,
		  (table1.JasaManajemen * 10) / 100 AS JasaMakloon10Percent,
		 (COALESCE(mtr.ProductivityIncentives, 0) * 10) / 100 ProductivityIncentives10Percent,
		 table1.TotalBayar,
		 th.KPSYear,
		 th.KPSWeek,		 
		 utl.IDFlow,
		 table1.TPOFeeCode,
		 CASE IDFlow
		    WHEN 40 THEN 'OPEN'
		    WHEN 43 THEN 'REVISED'
		    WHEN 47 THEN 'RETURNED'
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
		 th.TaxtNoProd
    FROM ( SELECT p.TPOFeeCode,
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
	   INNER JOIN dbo.TPOFeeHdr th ON th.TPOFeeCode = table1.TPOFeeCode
	   INNER JOIN dbo.MstGenLocation AS mgl ON mgl.LocationCode = th.LocationCode
	   INNER JOIN dbo.MstGenBrandGroup AS mgbg ON mgbg.BrandGroupCode = th.BrandGroupCode
	   LEFT JOIN dbo.MstTPOFeeRate mtr ON mtr.LocationCode = th.LocationCode
								AND mtr.BrandGroupCode = th.BrandGroupCode AND mtr.EffectiveDate <= th.CreatedDate AND mtr.ExpiredDate >= th.CreatedDate;

GO

