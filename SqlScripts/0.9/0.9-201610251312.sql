SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Abud
-- Create date: 2016-06-06
-- Description:	Recalculate begining and endingStock for day forward
-- =============================================

-- =============================================
-- Author:		Azka
-- Create date: 2016-10-25
-- Description:	Add Round
-- =============================================
ALTER PROCEDURE [dbo].[RecalculateStockExeReportByProcess]
	@productionDate date,
	@locationCode varchar(20),
	@branchCode varchar(20)
AS
BEGIN

	;WITH targetTable AS (
		SELECT * from dbo.ExeReportByProcess
	)

	UPDATE
	targetTable
	SET 
		targetTable.BeginningStock = ISNULL(ROUND((targetTable.BeginningStock + source.Delta), 2), ROUND(targetTable.BeginningStock, 2)),
		targetTable.EndingStock = ISNULL(ROUND((targetTable.EndingStock + source.Delta), 2), ROUND(targetTable.EndingStock, 2))
	FROM(
		SELECT 
			base.LocationCode,
		   base.UnitCode,
		   base.BrandCode,
		   base.ProductionDate,
		   base.ProcessGroup,
		   base.Description,
		   base.UOM,
		   base.UOMOrder,
		   base.BeginningStock,
		   base.EndingStock,

	  (SELECT base.EndingStock - sub.BeginningStock
	   FROM dbo.ExeReportByProcess sub
	   WHERE sub.LocationCode = base.LocationCode
		 AND sub.UnitCode = base.UnitCode
		 AND sub.BrandCode = base.BrandCode
		 AND sub.ProcessGroup = base.ProcessGroup
		 AND sub.UOMOrder = base.UOMOrder
		 AND sub.ProductionDate = DATEADD(DAY,1,base.ProductionDate)) AS Delta
	FROM dbo.ExeReportByProcess base
	WHERE base.LocationCode = @locationCode
	  AND base.BrandCode = @branchCode
	  AND base.ProductionDate = @productionDate
	) AS source
	WHERE
	targetTable.LocationCode = source.LocationCode
	and targetTable.UnitCode = source.UnitCode
	and targetTable.BrandCode = source.BrandCode
	and targetTable.ProcessGroup = source.ProcessGroup
	and targetTable.UOMOrder = source.UOMOrder
	and targetTable.ProductionDate > @productionDate

END