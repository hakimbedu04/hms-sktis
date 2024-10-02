-- Description: TPOFeeExeActualView
-- Ticket: http://tp.voxteneo.co.id/entity/264
-- Author: Harizal

-- Description: Change Submited => Submitted
-- Ticket: http://tp.voxteneo.co.id/entity/3464
-- Author: Robby

-- Description: Add regional
-- Ticket: http://tp.voxteneo.co.id/entity/7082
-- Author: Yudha


ALTER VIEW [dbo].[TPOFeeExeActualView]
AS
     SELECT ROW_NUMBER() OVER(ORDER BY TPOFeeCode DESC) AS Row,
		  th.TPOFeeCode,
            mgl.LocationName,
            mgbg.SKTBrandCode,
                 CASE utl.IDFlow
                    WHEN 40 THEN 'OPEN'
                    WHEN 41 THEN 'DRAFT'
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
            utl.CreatedBy PIC,
            th.LocationCode,
            th.BrandGroupCode,
            th.KPSYear,
            th.KPSWeek,
            th.TPOFeeTempID,
            th.TaxtNoProd,
            th.TaxtNoMgmt,
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
     FROM dbo.TPOFeeHdr AS th
          INNER JOIN dbo.MstGenLocation AS mgl ON mgl.LocationCode = th.LocationCode
          INNER JOIN dbo.MstGenBrandGroup AS mgbg ON mgbg.BrandGroupCode = th.BrandGroupCode
          LEFT JOIN( 
                      SELECT MAX(TransactionDate) AS TransDate,
                             TransactionCode AS TransCode
                      FROM dbo.UtilTransactionLogs AS utl
                      GROUP BY TransactionCode ) AS TransactionLogGroup ON th.TPOFeeCode = TransactionLogGroup.TransCode
          LEFT JOIN dbo.UtilTransactionLogs AS utl ON TransactionLogGroup.TransDate = utl.TransactionDate
                                                   AND TransactionLogGroup.TransCode = utl.TransactionCode;

GO


