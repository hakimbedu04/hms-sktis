/****** Object:  View [dbo].[ExeProductionEntryPrintView]    Script Date: 6/16/2016 12:47:30 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

-- =============================================
-- Description: Fix view add cast
-- Ticket: -
-- Author: Dwi Yudha
-- Editor: Indra Permana -> Change AbsentType into AbsentCodePayroll -> Change into AbsentCodeEblek
-- =============================================

-- =============================================
-- Description: I AM NOT CHANGE ANYTHING JUST RE-RUN THE SCRIPT :)
-- Ticket: http://tp.voxteneo.co.id/entity/7097, http://tp.voxteneo.co.id/entity/7096
-- Author: AZKA
-- Date: 2016/06/16
-- =============================================

-- Re Alter to fix Target & Actual datatype
-- Updated By: Robby

ALTER VIEW [dbo].[ExeProductionEntryPrintView]
AS
SELECT DISTINCT 
                         pev.KPSYear, pev.KPSWeek, pe.EmployeeID, pe.EmployeeNumber, mon.AbsentCodeEblek AS MonAbsentType, mon.AbsentRemark AS MonAbsentRemark, mon.ProdCapacity AS MonProdCapacity, 
                         mon.ProdTarget AS MonProdTarget, mon.ProdActual AS MonProdActual, tue.AbsentCodeEblek AS TueAbsentType, tue.AbsentRemark AS TueAbsentRemark, tue.ProdCapacity AS TueProdCapacity, 
                         tue.ProdTarget AS TueProdTarget, tue.ProdActual AS TueProdActual, wed.AbsentCodeEblek AS WedAbsentType, wed.AbsentRemark AS WedAbsentRemark, wed.ProdCapacity AS WedProdCapacity, 
                         wed.ProdTarget AS WedProdTarget, wed.ProdActual AS WedProdActual, thu.AbsentCodeEblek AS ThuAbsentType, thu.AbsentRemark AS ThuAbsentRemark, thu.ProdCapacity AS ThuProdCapacity, 
                         thu.ProdTarget AS ThuProdTarget, thu.ProdActual AS ThuProdActual, fri.AbsentCodeEblek AS FriAbsentType, fri.AbsentRemark AS FriAbsentRemark, fri.ProdCapacity AS FriProdCapacity, fri.ProdTarget AS FriProdTarget, 
                         fri.ProdActual AS FriProdActual, sat.AbsentCodeEblek AS SatAbsentType, sat.AbsentRemark AS SatAbsentRemark, sat.ProdCapacity AS SatProdCapacity, sat.ProdTarget AS SatProdTarget, 
                         sat.ProdActual AS SatProdActual, sun.AbsentCodeEblek AS SunAbsentType, sun.AbsentRemark AS SunAbsentRemark, sun.ProdCapacity AS SunProdCapacity, sun.ProdTarget AS SunProdTarget, 
                         sun.ProdActual AS SunProdActual, pev.LocationCode, pev.UnitCode, pev.Shift, pev.ProcessGroup, pev.GroupCode, pev.BrandCode
FROM            dbo.ExePlantProductionEntry AS pe INNER JOIN
                         dbo.ExePlantProductionEntryVerification AS pev ON pev.ProductionEntryCode = pe.ProductionEntryCode LEFT OUTER JOIN
                         dbo.ExePlantProductionEntry AS mon ON 
                         mon.ProductionEntryCode = 'EBL/' + pev.LocationCode + '/' + CAST(pev.Shift as varchar(1)) + '/' + pev.UnitCode + '/' + pev.GroupCode + '/' + pev.BrandCode + '/' + CAST(pev.KPSYear as varchar(4)) + '/' + CAST(pev.KPSWeek as varchar(2)) + '/1' AND 
                         mon.EmployeeID = pe.EmployeeID AND mon.EmployeeNumber = pe.EmployeeNumber LEFT OUTER JOIN
                         dbo.ExePlantProductionEntry AS tue ON 
                         tue.ProductionEntryCode = 'EBL/' + pev.LocationCode + '/' + CAST(pev.Shift as varchar(1)) + '/' + pev.UnitCode + '/' + pev.GroupCode + '/' + pev.BrandCode + '/' + CAST(pev.KPSYear as varchar(4)) + '/' + CAST(pev.KPSWeek as varchar(2)) + '/2' AND 
                         tue.EmployeeID = pe.EmployeeID AND tue.EmployeeNumber = pe.EmployeeNumber LEFT OUTER JOIN
                         dbo.ExePlantProductionEntry AS wed ON 
                         wed.ProductionEntryCode = 'EBL/' + pev.LocationCode + '/' + CAST(pev.Shift as varchar(1)) + '/' + pev.UnitCode + '/' + pev.GroupCode + '/' + pev.BrandCode + '/' + CAST(pev.KPSYear as varchar(4)) + '/' + CAST(pev.KPSWeek as varchar(2)) + '/3' AND 
                         wed.EmployeeID = pe.EmployeeID AND wed.EmployeeNumber = pe.EmployeeNumber LEFT OUTER JOIN
                         dbo.ExePlantProductionEntry AS thu ON 
                         thu.ProductionEntryCode = 'EBL/' + pev.LocationCode + '/' + CAST(pev.Shift as varchar(1)) + '/' + pev.UnitCode + '/' + pev.GroupCode + '/' + pev.BrandCode + '/' + CAST(pev.KPSYear as varchar(4)) + '/' + CAST(pev.KPSWeek as varchar(2)) + '/4' AND 
                         thu.EmployeeID = pe.EmployeeID AND thu.EmployeeNumber = pe.EmployeeNumber LEFT OUTER JOIN
                         dbo.ExePlantProductionEntry AS fri ON 
                         fri.ProductionEntryCode = 'EBL/' + pev.LocationCode + '/' + CAST(pev.Shift as varchar(1)) + '/' + pev.UnitCode + '/' + pev.GroupCode + '/' + pev.BrandCode + '/' + CAST(pev.KPSYear as varchar(4)) + '/' + CAST(pev.KPSWeek as varchar(2)) + '/5' AND 
                         fri.EmployeeID = pe.EmployeeID AND fri.EmployeeNumber = pe.EmployeeNumber LEFT OUTER JOIN
                         dbo.ExePlantProductionEntry AS sat ON 
                         sat.ProductionEntryCode = 'EBL/' + pev.LocationCode + '/' + CAST(pev.Shift as varchar(1)) + '/' + pev.UnitCode + '/' + pev.GroupCode + '/' + pev.BrandCode + '/' + CAST(pev.KPSYear as varchar(4)) + '/' + CAST(pev.KPSWeek as varchar(2)) + '/6' AND 
                         sat.EmployeeID = pe.EmployeeID AND sat.EmployeeNumber = pe.EmployeeNumber LEFT OUTER JOIN
                         dbo.ExePlantProductionEntry AS sun ON 
                         sun.ProductionEntryCode = 'EBL/' + pev.LocationCode + '/' + CAST(pev.Shift as varchar(1)) + '/' + pev.UnitCode + '/' + pev.GroupCode + '/' + pev.BrandCode + '/' + CAST(pev.KPSYear as varchar(4)) + '/' + CAST(pev.KPSWeek as varchar(2)) + '/7' AND 
                         sun.EmployeeID = pe.EmployeeID AND sun.EmployeeNumber = pe.EmployeeNumber
WHERE        (pev.ProductionEntryCode LIKE 'EBL/%')

GO


