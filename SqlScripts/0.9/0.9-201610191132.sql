-- =============================================
-- Description: add sp to copy ending stock prev day to curr day beginnning stock
-- Author: azka
-- Update: 19/10/2016
-- ticket: http://tp.voxteneo.co.id/entity/10449
-- =============================================
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[COPY_END_BEGIN_STOCK_BYPROCESS]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[COPY_END_BEGIN_STOCK_BYPROCESS]
GO

CREATE PROC COPY_END_BEGIN_STOCK_BYPROCESS
(
	@paramLocationCode	VARCHAR(8),
	@paramUnitCode		VARCHAR(4),
	@paramBrandCode		VARCHAR(11),
	@paramProdDate		DATETIME

)
AS
BEGIN
	DECLARE @prevDate DATETIME;
	
	SET @prevDate = DATEADD(DAY, -1, @paramProdDate);

	UPDATE a
	SET 
		a.BeginningStock = b.EndingStock,
		a.EndingStock = b.EndingStock
	FROM ExeReportByProcess a 
	INNER JOIN ExeReportByProcess b on b.LocationCode = a.LocationCode and a.UnitCode = b.UnitCode
	and a.BrandCode = b.BrandCode and a.ProcessOrder = b.ProcessOrder
	and a.UOMOrder = b.UOMOrder
	WHERE a.ProductionDate = @paramProdDate and a.LocationCode = @paramLocationCode
	and a.UnitCode = @paramUnitCode and a.BrandCode = @paramBrandCode and b.ProductionDate = @prevDate

END