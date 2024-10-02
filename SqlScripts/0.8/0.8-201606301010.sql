-- =============================================
-- Author:		Dwi Yudha
-- Create date: 29-06-2016
-- Description:	Revision condition
-- Version:		1.0.10
-- Ticket:		http://tp.voxteneo.co.id/entity/8285
-- =============================================

ALTER FUNCTION [dbo].[GetTPOReportsProduction] 
(
	@LocationCode VARCHAR(10) = NULL,
	
	@YearFrom INT = NULL,
	@YearTo INT = NULL,
	@WeekFrom INT = NULL,
	@WeekTo INT = NULL,
	
	@Month INT = NULL,
	@Year INT = NULL,

	@DateFrom DATE = NULL,
	@DateTo DATE = NULL,

	@FilterType VARCHAR(10) =  NULL
)
RETURNS 
@TPOReportsProduction TABLE 
(	
	Regional				VARCHAR(10),
	LocationCode			VARCHAR(10),
	LocationAbbr			VARCHAR(10),
	LocationName			VARCHAR(100),

	UMK						DECIMAL,
	Brand					VARCHAR(25),
	Package					REAL,

	JKNProductionFee		DECIMAL,
	JL1ProductionFee		DECIMAL,
	JL2ProductionFee		DECIMAL,
	JL3ProductionFee		DECIMAL,
	JL4ProductionFee		DECIMAL,

	ManagementFee			DECIMAL,
	ProductivityIncentives	DECIMAL,

	Year					INT,

	StartDate				DATE,
	EndDate					DATE,

	WeekFrom				INT,
	WeekTo					INT,

	NoMemo					VARCHAR(50),

	JKNProductionVolume		DECIMAL,
	JL1ProductionVolume		DECIMAL,
	JL2ProductionVolume		DECIMAL,
	JL3ProductionVolume		DECIMAL,
	JL4ProductionVolume		DECIMAL
)
AS
BEGIN
	DECLARE @isProductionDaily INT = 0;

	-- FILTER TYPE SETUP ---------------------------------------------------------------------
	IF @FilterType = 'All'
	BEGIN
		SET @isProductionDaily = 1;
		
		SELECT TOP 1 @DateFrom = StartDate
		FROM MstGenWeek
		ORDER BY StartDate ASC

		SELECT TOP 1 @DateTo = EndDate
		FROM MstGenWeek
		ORDER BY EndDate DESC
	END

	IF @FilterType = 'Daily'
	BEGIN
		SET @isProductionDaily = 1;
	END

	IF @FilterType = 'YearMonth'
	BEGIN
		SET @isProductionDaily = 1;
		
		SET @DateFrom = DATEADD(mm, (@Year - 1900) * 12 + @Month - 1 , 0);
		SET @DateTo = DATEADD(mm, (@Year - 1900) * 12 + @Month, - 1);
	END

	IF @FilterType = 'YearWeek'
	BEGIN
		SET @isProductionDaily = 0;

		SELECT @DateFrom = MIN(StartDate), @DateTo = MAX(EndDate)
		FROM MstGenWeek
		WHERE (Year >= @YearFrom AND Week >= @WeekFrom) AND (Year <= @YearTo AND Week <= @WeekTo);
	END
	-- EOL FILTER TYPE SETUP -----------------------------------------------------------------

	DECLARE @minYear INT, @minWeek INT, @maxYear INT, @maxWeek INT;

	SELECT @minYear = Year, @minWeek = Week
	FROM MstGenWeek
	WHERE StartDate <= @DateFrom AND EndDate >= @DateFrom

	SELECT @maxYear = Year, @maxWeek = Week
	FROM MstGenWeek
	WHERE StartDate <= @DateTo AND EndDate >= @DateTo

	IF @minYear IS NULL OR @maxYear IS NULL
	BEGIN
		SET @minYear = 0;
		SET @minWeek = 0;
		SET @maxYear = 0;
		SET @maxWeek = 0;
	END

	-- MstTPOPackage temp table
	DECLARE @TPOPackage TABLE
	(
		TempId			INT,
		BrandGroupCode	VARCHAR(11),
		LocationCode	VARCHAR(10),
		Package			REAL,
		EffectiveDate	DATE,
		ExpiredDate		DATE,
		Year			INT,
		MemoRef			VARCHAR(50)
	)
	DECLARE @TempId INT;

	INSERT INTO @TPOPackage 
	SELECT	ROW_NUMBER() OVER(ORDER BY LocationCode) AS TempId,
			BrandGroupCode, 
			LocationCode, 
			Package, 
			EffectiveDate, 
			ExpiredDate, 
			CAST(DATEPART(yyyy, EffectiveDate) AS INT),
			MemoRef
	FROM MstTPOPackage
	WHERE LocationCode = @LocationCode
	  AND ((EffectiveDate >= @DateFrom AND ExpiredDate <= @DateTo)
	   OR (EffectiveDate >= @DateFrom AND EffectiveDate <= @DateTo)
	   OR (ExpiredDate >= @DateFrom AND ExpiredDate <= @DateTo))
	ORDER BY LocationCode, BrandGroupCode;

	DECLARE @listResultHeader TABLE
	(
		TPOFeeCode	VARCHAR(50),
		TotalJKN	DECIMAL,
		TotalJL1	DECIMAL,
		TotalJL2	DECIMAL,
		TotalJL3	DECIMAL,
		TotalJL4	DECIMAL
	)

	DECLARE @calculation TABLE
	(
		OrderFeeType	INT,
		Calculate		DECIMAL
	)

	DECLARE @productionDaily TABLE
	(
		FeeDate			DATE,
		JKNRp			DECIMAL,
		JL1Rp			DECIMAL,
		JL2Rp			DECIMAL,
		JL3Rp			DECIMAL,
		JL4Rp			DECIMAL,
		JKNBoxFinal		DECIMAL,
		JL1BoxFinal		DECIMAL,
		JL2BoxFinal		DECIMAL,
		JL3BoxFinal		DECIMAL,
		JL4BoxFinal		DECIMAL
	)

	-- Variable to insert
	DECLARE	@JKNProductionFee		DECIMAL,
			@JL1ProductionFee		DECIMAL,
			@JL2ProductionFee		DECIMAL,
			@JL3ProductionFee		DECIMAL,
			@JL4ProductionFee		DECIMAL,
			@JKNProductionVolume	DECIMAL,
			@JL1ProductionVolume	DECIMAL,
			@JL2ProductionVolume	DECIMAL,
			@JL3ProductionVolume	DECIMAL,
			@JL4ProductionVolume	DECIMAL,
			@Regional				VARCHAR(20),
			@LocationAbbr			VARCHAR(20),
			@LocationName			VARCHAR(50),
			@TPOLocationCode		VARCHAR(10),
			@UMK					DECIMAL,
			@Package				REAL,
			@Brand					VARCHAR(11),
			@ManagementFee			DECIMAL,
			@ProductivityIncentives	DECIMAL,
			@TPOYear				INT,
			@StartDate				DATE,
			@EndDate				DATE,
			@NoMemo					VARCHAR(50)


	-- LOOP by MstTPOPackage
	WHILE((SELECT COUNT(*) FROM @TPOPackage) > 0)
	BEGIN
		SET @TempId = (SELECT TOP 1 TempId From @TPOPackage);

		SELECT @Brand = BrandGroupCode, @Package = Package, @TPOLocationCode = LocationCode, @TPOYear = Year, @StartDate = EffectiveDate,
			@EndDate = ExpiredDate, @NoMemo = MemoRef
		FROM @TPOPackage 
		WHERE TempId = @TempId

		-- Location Data
		SELECT @Regional = ParentLocationCode, @LocationAbbr = ABBR, @LocationName = LocationName, @UMK = UMK
		FROM MstGenLocation
		WHERE LocationCode = @TPOLocationCode

		INSERT INTO @listResultHeader
		SELECT	TPOFeeCode,
				TotalJKN,
				TotalJL1,
				TotalJL2,
				TotalJL3,
				TotalJL4
		FROM TPOFeeHdr
		WHERE LocationCode = @LocationCode
		  AND CAST(CAST(KPSYear AS VARCHAR(4))+REPLACE(STR(KPSWeek, 2, 0), ' ', '0') AS INT) >= CAST(CAST(@minYear AS VARCHAR(4))+REPLACE(STR(@minWeek, 2, 0), ' ', '0') AS INT)
		  AND CAST(CAST(KPSYear AS VARCHAR(4))+REPLACE(STR(KPSWeek, 2, 0), ' ', '0') AS INT) <= CAST(CAST(@maxYear AS VARCHAR(4))+REPLACE(STR(@maxWeek, 2, 0), ' ', '0') AS INT)
		  AND BrandGroupCode = @Brand

		INSERT INTO @calculation
		SELECT OrderFeeType, Calculate
		FROM TPOFeeCalculation
		WHERE TPOFeeCode IN (SELECT TPOFeeCode FROM @listResultHeader)

		IF @isProductionDaily = 1
		BEGIN
			-- Daily data
			INSERT INTO @productionDaily
			SELECT	FeeDate,
					JKNRp,
					JL1Rp,
					JL2Rp,
					JL3Rp,
					JL4Rp,
					JKNBoxFinal,
					JL1BoxFinal,
					JL2BoxFinal,
					JL3BoxFinal,
					JL4BoxFinal
			FROM TPOFeeProductionDaily
			WHERE TPOFeeCode IN (SELECT TPOFeeCode FROM @listResultHeader)
			
			SELECT @JKNProductionFee = SUM(JKNRp) FROM @productionDaily WHERE FeeDate >= @DateFrom AND FeeDate <= @DateTo;
			SELECT @JL1ProductionFee = SUM(JL1Rp) FROM @productionDaily WHERE FeeDate >= @DateFrom AND FeeDate <= @DateTo;
			SELECT @JL2ProductionFee = SUM(JL2Rp) FROM @productionDaily WHERE FeeDate >= @DateFrom AND FeeDate <= @DateTo;
			SELECT @JL3ProductionFee = SUM(JL3Rp) FROM @productionDaily WHERE FeeDate >= @DateFrom AND FeeDate <= @DateTo;
			SELECT @JL4ProductionFee = SUM(JL4Rp) FROM @productionDaily WHERE FeeDate >= @DateFrom AND FeeDate <= @DateTo;
			SELECT @JKNProductionVolume = SUM(JKNBoxFinal) FROM @productionDaily WHERE FeeDate >= @DateFrom AND FeeDate <= @DateTo;
			SELECT @JL1ProductionVolume = SUM(JL1BoxFinal) FROM @productionDaily WHERE FeeDate >= @DateFrom AND FeeDate <= @DateTo;
			SELECT @JL2ProductionVolume = SUM(JL2BoxFinal) FROM @productionDaily WHERE FeeDate >= @DateFrom AND FeeDate <= @DateTo;
			SELECT @JL3ProductionVolume = SUM(JL3BoxFinal) FROM @productionDaily WHERE FeeDate >= @DateFrom AND FeeDate <= @DateTo;
			SELECT @JL4ProductionVolume = SUM(JL4BoxFinal) FROM @productionDaily WHERE FeeDate >= @DateFrom AND FeeDate <= @DateTo;

		END
		ELSE
		BEGIN
			-- Weekly data
			SELECT @JKNProductionFee = SUM(Calculate) FROM @calculation WHERE OrderFeeType = 1;
			SELECT @JL1ProductionFee = SUM(Calculate) FROM @calculation WHERE OrderFeeType = 2;
			SELECT @JL2ProductionFee = SUM(Calculate) FROM @calculation WHERE OrderFeeType = 3;
			SELECT @JL3ProductionFee = SUM(Calculate) FROM @calculation WHERE OrderFeeType = 4;
			SELECT @JL4ProductionFee = SUM(Calculate) FROM @calculation WHERE OrderFeeType = 5;
			SELECT @JKNProductionVolume = SUM(TotalJKN) FROM @listResultHeader;
			SELECT @JL1ProductionVolume = SUM(TotalJL1) FROM @listResultHeader;
			SELECT @JL2ProductionVolume = SUM(TotalJL2) FROM @listResultHeader;
			SELECT @JL3ProductionVolume = SUM(TotalJL3) FROM @listResultHeader;
			SELECT @JL4ProductionVolume = SUM(TotalJL4) FROM @listResultHeader;

		END
		
		SELECT @ManagementFee = SUM(Calculate) FROM @calculation WHERE OrderFeeType = 7; -- Jasa Manajemen
		SELECT @ProductivityIncentives = SUM(Calculate) FROM @calculation WHERE OrderFeeType = 16; -- Productivity Incentives

		INSERT INTO @TPOReportsProduction
		SELECT @Regional,
			   @TPOLocationCode,
			   @LocationAbbr,
			   @LocationName,
			   @UMK,
			   @Brand,
			   @Package,
			   @JKNProductionFee,
			   @JL1ProductionFee,
			   @JL2ProductionFee,
			   @JL3ProductionFee,
			   @JL4ProductionFee,
			   @ManagementFee,
			   @ProductivityIncentives,
			   @TPOYear,
			   @StartDate,
			   @EndDate,
			   @WeekFrom,
			   @WeekTo,
			   @NoMemo,
			   @JKNProductionVolume,
			   @JL1ProductionVolume,
			   @JL2ProductionVolume,
			   @JL3ProductionVolume,
			   @JL4ProductionVolume

		DELETE @TPOPackage WHERE  TempId = @TempId;
	END
	-- END OF WHILE
	
	RETURN 
END
