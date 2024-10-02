-- =============================================
-- Author:		abud
-- Create date: 2016-06-02
-- Description:	Report by process switching brand
-- =============================================
ALTER PROCEDURE [dbo].[SwitchingBrandExeReportByProcess]
	@locationCode varchar(10),
	@brandGroupCode varchar(20),
	@productionDate date
AS
BEGIN
	DECLARE @brandCode AS TABLE (max_brand VARCHAR(20), min_brand VARCHAR(20));
	DECLARE @status int;
	DECLARE @updated int;

	-- Get MAX and MIN Brand
	INSERT INTO @brandCode(max_brand, min_brand)
	SELECT MAX(p.BrandCode) AS max_brand,
			MIN(p.BrandCode) AS min_brand
	FROM dbo.ExeTPOProductionEntryVerificationView p
	JOIN dbo.MstGenBrand b ON b.BrandCode = p.BrandCode
	WHERE p.LocationCode = @locationCode
		AND b.BrandGroupCode = @brandGroupCode
		AND p.ProductionDate = @productionDate

	SET @status =  (CASE
		WHEN 
			(SELECT COUNT(*) FROM dbo.ExeReportByProcess WHERE LocationCode = @locationCode AND BrandCode = (SELECT min_brand FROM @brandCode) AND ProductionDate = @productionDate) > 0
		THEN 0
		ELSE -1
		END
	);

	SET @updated = (SELECT (CASE WHEN (max_brand = min_brand) THEN 0 ELSE 1 END)  FROM @brandCode);

	-- Update present date
	IF @updated = 1
	BEGIN
		UPDATE dbo.ExeReportByProcess 
		SET BeginningStock = brandMin.EndingStock
		FROM (
			SELECT src.EndingStock, src.ProcessGroup, src.UOMOrder
			FROM dbo.ExeReportByProcess AS src
			WHERE 
			src.LocationCode = @locationCode 
			AND src.ProductionDate = DATEADD(DAY,@status,@productionDate)
			AND src.BrandCode = (SELECT min_brand FROM @brandCode)
		) AS brandMin
		WHERE 
		dbo.ExeReportByProcess.LocationCode = @locationCode 
		AND dbo.ExeReportByProcess.ProductionDate = @productionDate
		AND dbo.ExeReportByProcess.BrandCode = (SELECT max_brand FROM @brandCode)
		AND dbo.ExeReportByProcess.ProcessGroup = brandMin.ProcessGroup
		AND dbo.ExeReportByProcess.UOMOrder = brandMin.UOMOrder
	END
END

GO


