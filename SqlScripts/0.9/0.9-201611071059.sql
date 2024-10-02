IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[dbo].[GenerateP1TemplateAP]'))
	DROP PROCEDURE [dbo].[GenerateP1TemplateAP]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROC [dbo].[GenerateP1TemplateAP]
(
		@ParamLocation VARCHAR(8),
		@ParamWeek     INT,
		@ParamYear	   INT
)
AS
BEGIN
	--SET @ParamLocation = 'REG2';
	--SET @ParamWeek     = 39;
	--SET @ParamYear     = 2016;

	DECLARE @Location	VARCHAR(8),
			@Week		INT,
			@Year		INT,
			@DD			VARCHAR(2),
			@MM			VARCHAR(2),
			@YY			VARCHAR(4),
			@StartDate	DATETIME,
			@ItemStartDate	DATETIME,
			@ItemEndDate	DATETIME,
			@DDCurr		VARCHAR(2),
			@MMCurr		VARCHAR(2),
			@YYCurr		VARCHAR(4),
			@ItemText   VARCHAR(64)

	DECLARE @TempTable1 TABLE(	
			BiayaProduksi float ,
			PPNBiayaProduksi10Prcnt float ,
			JasaManajemen float ,
			PPNJasaManajemen10Prcnt float ,
			PajakJasaManajemenSebesar2Prcnt float ,
			TPOFeeCode VARCHAR(64), 
			LocationCode VARCHAR(8), 
			KPSWeek INT, 
			KPSYear INT, 
			TaxtNoProd VARCHAR(32), 
			OrderFeeType INT, 
			SKTBrandCode VARCHAR(32),
			VendorName VARCHAR(128),
			FlowStatus VARCHAR(12));

	SET @Location = @ParamLocation;
	SET @Week     = @ParamWeek;
	SET @Year     = @ParamYear;

	SELECT @StartDate = DATEADD(DD,5,(select StartDate from MstGenWeek where Week = @Week and Year = @Year))

	SELECT @ItemStartDate = StartDate from MstGenWeek where Week = @Week and Year = @Year;
	SELECT @ItemEndDate = EndDate from MstGenWeek where Week = @Week and Year = @Year;

	SELECT @DD = CONVERT(VARCHAR(2), @StartDate, 103)
	SELECT @MM = CONVERT(VARCHAR(2), @StartDate, 101)
	SELECT @YY = SUBSTRING( CONVERT(varchar(4),(SELECT DATEPART( YY, @StartDate))),3,2)

	SELECT @DDCurr = CONVERT(VARCHAR(2), GETDATE(), 103)
	SELECT @MMCurr = CONVERT(VARCHAR(2), GETDATE(), 101)
	SELECT @YYCurr = SUBSTRING( CONVERT(varchar(4),(SELECT DATEPART( YY, GETDATE()))),3,2)

	IF(@Location != 'TPO')
	BEGIN
		INSERT INTO @TempTable1
		SELECT Cal1.Calculate as BiayaProduksi,
		Cal2.Calculate as PPNBiayaProduksi10Prcnt,
		Cal3.Calculate as JasaManajemen,
		Cal4.Calculate as PPNJasaManajemen10Prcnt,
		Cal5.Calculate as PajakJasaManajemenSebesar2Prcnt,
		TFH.TPOFeeCode, TFH.LocationCode, TFH.KPSWeek, TFH.KPSYear, TFH.TaxtNoProd, TFC.OrderFeeType, GBG.SKTBrandCode,MTI.VendorName,
		CASE UTL.IDFlow
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
		END AS FlowStatus
		FROM 
			(SELECT a.TPOFeeCode, a.Calculate, a.OrderFeeType,b.LocationCode FROM [TPOFeeCalculation] a
			INNER JOIN [TPOFeeHdr] b
			ON a.TPOFeeCode=b.TPOFeeCode
			WHERE OrderFeeType = 6
			AND a.KPSWeek = @Week
			AND a.KPSYear = @Year) Cal1
		INNER JOIN 
			(SELECT a.TPOFeeCode, a.Calculate, a.OrderFeeType,b.LocationCode FROM [TPOFeeCalculation] a
			INNER JOIN [TPOFeeHdr] b
			ON a.TPOFeeCode=b.TPOFeeCode
			WHERE OrderFeeType = 13
			AND a.KPSWeek = @Week
			AND a.KPSYear = @Year) Cal2
		ON Cal1.TPOFeeCode=Cal2.TPOFeeCode

		INNER JOIN 
			(SELECT a.TPOFeeCode, a.Calculate, a.OrderFeeType,b.LocationCode FROM [TPOFeeCalculation] a
			INNER JOIN [TPOFeeHdr] b
			ON a.TPOFeeCode=b.TPOFeeCode
			WHERE OrderFeeType = 7
			AND a.KPSWeek = @Week
			AND a.KPSYear = @Year) Cal3
		ON Cal2.TPOFeeCode=Cal3.TPOFeeCode

		INNER JOIN 
			(SELECT a.TPOFeeCode, a.Calculate, a.OrderFeeType,b.LocationCode FROM [TPOFeeCalculation] a
			INNER JOIN [TPOFeeHdr] b
			ON a.TPOFeeCode=b.TPOFeeCode
			WHERE OrderFeeType = 14
			AND a.KPSWeek = @Week
			AND a.KPSYear = @Year) Cal4
		ON Cal3.TPOFeeCode=Cal4.TPOFeeCode

		INNER JOIN 
			(SELECT a.TPOFeeCode, a.Calculate, a.OrderFeeType,b.LocationCode FROM [TPOFeeCalculation] a
			INNER JOIN [TPOFeeHdr] b
			ON a.TPOFeeCode=b.TPOFeeCode
			WHERE OrderFeeType = 9
			AND a.KPSWeek = @Week
			AND a.KPSYear = @Year) Cal5
		ON Cal4.TPOFeeCode=Cal5.TPOFeeCode
		INNER JOIN [TPOFeeCalculation] TFC
		ON Cal5.TPOFeeCode = tfc.TPOFeeCode
		AND Cal5.OrderFeeType = tfc.OrderFeeType
		INNER JOIN [TPOFeeHdr] TFH
		ON TFC.TPOFeeCode=TFH.TPOFeeCode
		INNER JOIN( SELECT MAX(TransactionDate) AS TransDate, TransactionCode AS TransCode
			FROM dbo.UtilTransactionLogs AS utl
			GROUP BY TransactionCode ) AS log ON TFC.TPOFeeCode = log.TransCode
		LEFT JOIN dbo.UtilTransactionLogs AS utl ON log.TransDate = utl.TransactionDate
			AND log.TransCode = utl.TransactionCode
		INNER JOIN [MstGenLocation] MGL
		ON MGL.LocationCode=TFH.LocationCode
		INNER JOIN [MstGenBrandGroup] GBG
		ON TFH.[BrandGroupCode]=GBG.[BrandGroupCode]
		INNER JOIN [MstTPOInfo] MTI
		ON MTI.LocationCode = TFH.LocationCode
		WHERE TFH.KPSWeek = @Week
		AND TFH.KPSYear = @Year
		AND MGL.ParentLocationCode = @Location;
	END
	ELSE
	BEGIN
		INSERT INTO @TempTable1
		SELECT Cal1.Calculate as BiayaProduksi,
		Cal2.Calculate as PPNBiayaProduksi10Prcnt,
		Cal3.Calculate as JasaManajemen,
		Cal4.Calculate as PPNJasaManajemen10Prcnt,
		Cal5.Calculate as PajakJasaManajemenSebesar2Prcnt,
		TFH.TPOFeeCode, TFH.LocationCode, TFH.KPSWeek, TFH.KPSYear, TFH.TaxtNoProd, TFC.OrderFeeType, GBG.SKTBrandCode,MTI.VendorName,
		CASE UTL.IDFlow
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
		END AS FlowStatus
		FROM 
			(SELECT a.TPOFeeCode, a.Calculate, a.OrderFeeType,b.LocationCode FROM [TPOFeeCalculation] a
			INNER JOIN [TPOFeeHdr] b
			ON a.TPOFeeCode=b.TPOFeeCode
			WHERE OrderFeeType = 6
			AND a.KPSWeek = @Week
			AND a.KPSYear = @Year) Cal1
		INNER JOIN 
			(SELECT a.TPOFeeCode, a.Calculate, a.OrderFeeType,b.LocationCode FROM [TPOFeeCalculation] a
			INNER JOIN [TPOFeeHdr] b
			ON a.TPOFeeCode=b.TPOFeeCode
			WHERE OrderFeeType = 13
			AND a.KPSWeek = @Week
			AND a.KPSYear = @Year) Cal2
		ON Cal1.TPOFeeCode=Cal2.TPOFeeCode

		INNER JOIN 
			(SELECT a.TPOFeeCode, a.Calculate, a.OrderFeeType,b.LocationCode FROM [TPOFeeCalculation] a
			INNER JOIN [TPOFeeHdr] b
			ON a.TPOFeeCode=b.TPOFeeCode
			WHERE OrderFeeType = 7
			AND a.KPSWeek = @Week
			AND a.KPSYear = @Year) Cal3
		ON Cal2.TPOFeeCode=Cal3.TPOFeeCode

		INNER JOIN 
			(SELECT a.TPOFeeCode, a.Calculate, a.OrderFeeType,b.LocationCode FROM [TPOFeeCalculation] a
			INNER JOIN [TPOFeeHdr] b
			ON a.TPOFeeCode=b.TPOFeeCode
			WHERE OrderFeeType = 14
			AND a.KPSWeek = @Week
			AND a.KPSYear = @Year) Cal4
		ON Cal3.TPOFeeCode=Cal4.TPOFeeCode

		INNER JOIN 
			(SELECT a.TPOFeeCode, a.Calculate, a.OrderFeeType,b.LocationCode FROM [TPOFeeCalculation] a
			INNER JOIN [TPOFeeHdr] b
			ON a.TPOFeeCode=b.TPOFeeCode
			WHERE OrderFeeType = 9
			AND a.KPSWeek = @Week
			AND a.KPSYear = @Year) Cal5
		ON Cal4.TPOFeeCode=Cal5.TPOFeeCode

		INNER JOIN [TPOFeeCalculation] TFC
		ON Cal5.TPOFeeCode = tfc.TPOFeeCode
		AND Cal5.OrderFeeType = tfc.OrderFeeType
		INNER JOIN [TPOFeeHdr] TFH
		ON TFC.TPOFeeCode=TFH.TPOFeeCode
		INNER JOIN( SELECT MAX(TransactionDate) AS TransDate, TransactionCode AS TransCode
			FROM dbo.UtilTransactionLogs AS utl
			GROUP BY TransactionCode ) AS log ON TFC.TPOFeeCode = log.TransCode
		LEFT JOIN dbo.UtilTransactionLogs AS utl ON log.TransDate = utl.TransactionDate
			AND log.TransCode = utl.TransactionCode
		INNER JOIN [MstGenLocation] MGL
		ON MGL.LocationCode=TFH.LocationCode
		INNER JOIN [MstGenBrandGroup] GBG
		ON TFH.[BrandGroupCode]=GBG.[BrandGroupCode]
		INNER JOIN [MstTPOInfo] MTI
		ON MTI.LocationCode = TFH.LocationCode
		WHERE TFH.KPSWeek = @Week
		AND TFH.KPSYear = @Year;
	END

	--SELECT * FROM @TempTable1

	SELECT DISTINCT
	'1' as no,
	TPOFeeCode,
	FlowStatus,
	LocationCode, 
	'1' AS counter,
	'3066' AS Company,
	'IDR' AS CurrencyKey,
	'KD' AS DocumentType,
	@DD + '.' + @MM + '.' + @YY AS TaxDate,
	ISNULL(TaxtNoProd,'') AS DocumentHeaderText,
	'' AS Reference,
	@DDCurr + '.' + @MMCurr + '.' + @YYCurr AS DocumentDate,
	'' AS SCBIndocator,
	'' AS PaymentReference,
	'' AS HouseBank,
	'0' AS InstructionKey1,
	'0' AS InstructionKey2,
	'0' AS InstructionKey3,
	'0' AS InstructionKey4,
	'' AS PartnerBankType,
	'' AS ReferenceText1,
	'' AS PostingKey,
	'' AS Account,
	'0' AS AmountInDocumentCurrency,
	'IDR' AS LocalCurrency,
	'' AS TaxCode,
	'0' AS Quantity,
	'' AS BaseUnitOfMeasure,
	'' AS Assignment,
	'' AS ItemText,
	'' AS LongText,
	'' AS CostCenter,
	'' AS WBSElement,
	'' AS MaterialNumber,
	'' AS PaymentTerms,
	'' AS TradingPartner,
	'' AS [Order],
	'' AS ShipToParty,
	'' AS CustomerCOPA,
	'' AS BrandFamilyCOPA,
	'' AS IntercoAffiliateCOPA,
	'' AS WBSCOPA,
	'' AS ProductCOPA,
	'' AS ProductionCenterCOPA,
	'' AS PMIMarketCOPA,
	'' AS ProductCategoryCOPA,
	'' AS LabelCOPA,
	'' AS TradeChannelCOPA,
	'' AS ShipToPartyCOPA,
	'' AS FinalMarketCOPA,
	'' AS CreditControlArea,
	'' AS IndicatorNegativePosting,
	'' AS FiscalYear,
	'' AS PurchasingDocNum,
	'0' AS ItemNumOfPO,
	'' AS ISRAccount,
	'' AS ISRReference,
	'' AS Plant,
	'' AS Identifier
	FROM @TempTable1
	--WHERE LocationCode = 'IDAD'

	UNION ALL

	SELECT DISTINCT
	'2' as no,
	TPOFeeCode,
	FlowStatus,
	LocationCode, 
	'2' AS counter,
	'' AS Company,
	'IDR' AS CurrencyKey,
	'' AS DocumentType,
	@DD + '.' + @MM + '.' + @YY AS TaxDate,
	'' AS DocumentHeaderText,
	'' AS Reference,
	@DDCurr + '.' + @MMCurr + '.' + @YYCurr AS DocumentDate,
	'' AS SCBIndocator,
	'' AS PaymentReference,
	'' AS HouseBank,
	'0' AS InstructionKey1,
	'0' AS InstructionKey2,
	'0' AS InstructionKey3,
	'0' AS InstructionKey4,
	'' AS PartnerBankType,
	'' AS ReferenceText1,
	'31' AS PostingKey,
	'81003668' AS Account,
	 ((BiayaProduksi+PPNBiayaProduksi10Prcnt) + (JasaManajemen+PPNJasaManajemen10Prcnt)) - PajakJasaManajemenSebesar2Prcnt
	 AS AmountInDocumentCurrency,
	'IDR' AS LocalCurrency,
	'V4' AS TaxCode,
	'0' AS Quantity,
	'' AS BaseUnitOfMeasure,
	@DDCurr+'-'+SUBSTRING(DATENAME(MONTH,dateadd(month, 0,GETDATE())),1,3)+'-'+@YYCurr AS Assignment,
	(
		SKTBrandCode +'-'+ LocationCode+ 
		CONVERT(VARCHAR(2), @ItemStartDate, 103) +'.'+CONVERT(VARCHAR(2), @ItemStartDate, 101)+'.'+SUBSTRING( CONVERT(varchar(4),(SELECT DATEPART( YY, @ItemStartDate))),3,2) +'-'+
		CONVERT(VARCHAR(2), @ItemEndDate, 103) +'.'+CONVERT(VARCHAR(2), @ItemEndDate, 101)+'.'+SUBSTRING( CONVERT(varchar(4),(SELECT DATEPART( YY, @ItemEndDate))),3,2) +
		'(w.'+CONVERT(VARCHAR(2),@Week)+')'
	) AS ItemText,
	'' AS LongText,
	'' AS CostCenter,
	'' AS WBSElement,
	'' AS MaterialNumber,
	'CR07' AS PaymentTerms,
	'' AS TradingPartner,
	'' AS [Order],
	'' AS ShipToParty,
	'' AS CustomerCOPA,
	'' AS BrandFamilyCOPA,
	'' AS IntercoAffiliateCOPA,
	'' AS WBSCOPA,
	'' AS ProductCOPA,
	'' AS ProductionCenterCOPA,
	'' AS PMIMarketCOPA,
	'' AS ProductCategoryCOPA,
	'' AS LabelCOPA,
	'' AS TradeChannelCOPA,
	'' AS ShipToPartyCOPA,
	'' AS FinalMarketCOPA,
	'' AS CreditControlArea,
	'' AS IndicatorNegativePosting,
	'' AS FiscalYear,
	'' AS PurchasingDocNum,
	'0' AS ItemNumOfPO,
	'' AS ISRAccount,
	'' AS ISRReference,
	'' AS Plant,
	'' AS Identifier
	FROM @TempTable1
	--WHERE LocationCode = 'IDAD'

	UNION ALL

	SELECT DISTINCT
	'3' as no,
	TPOFeeCode,
	FlowStatus,
	LocationCode, 
	'2' AS counter,
	'' AS Company,
	'IDR' AS CurrencyKey,
	'' AS DocumentType,
	@DD + '.' + @MM + '.' + @YY AS TaxDate,
	'' AS DocumentHeaderText,
	'' AS Reference,
	@DDCurr + '.' + @MMCurr + '.' + @YYCurr AS DocumentDate,
	'' AS SCBIndocator,
	'' AS PaymentReference,
	'' AS HouseBank,
	'0' AS InstructionKey1,
	'0' AS InstructionKey2,
	'0' AS InstructionKey3,
	'0' AS InstructionKey4,
	'' AS PartnerBankType,
	'' AS ReferenceText1,
	'40' AS PostingKey,
	'82182000' AS Account,
	(BiayaProduksi + PPNBiayaProduksi10Prcnt) AS AmountInDocumentCurrency,
	'IDR' AS LocalCurrency,
	'V4' AS TaxCode,
	'0' AS Quantity,
	'' AS BaseUnitOfMeasure,
	VendorName AS Assignment,
	(
		SKTBrandCode +'-'+ LocationCode+ 
		CONVERT(VARCHAR(2), @ItemStartDate, 103) +'.'+CONVERT(VARCHAR(2), @ItemStartDate, 101)+'.'+SUBSTRING( CONVERT(varchar(4),(SELECT DATEPART( YY, @ItemStartDate))),3,2) +'-'+
		CONVERT(VARCHAR(2), @ItemEndDate, 103) +'.'+CONVERT(VARCHAR(2), @ItemEndDate, 101)+'.'+SUBSTRING( CONVERT(varchar(4),(SELECT DATEPART( YY, @ItemEndDate))),3,2) +
		'(w.'+CONVERT(VARCHAR(2),@Week)+')ProdCost'
	) AS ItemText,
	'' AS LongText,
	'3066150VR3' AS CostCenter,
	'' AS WBSElement,
	'' AS MaterialNumber,
	'' AS PaymentTerms,
	'' AS TradingPartner,
	'' AS [Order],
	'' AS ShipToParty,
	'' AS CustomerCOPA,
	'' AS BrandFamilyCOPA,
	'' AS IntercoAffiliateCOPA,
	'' AS WBSCOPA,
	'' AS ProductCOPA,
	'' AS ProductionCenterCOPA,
	'' AS PMIMarketCOPA,
	'' AS ProductCategoryCOPA,
	'' AS LabelCOPA,
	'' AS TradeChannelCOPA,
	'' AS ShipToPartyCOPA,
	'' AS FinalMarketCOPA,
	'' AS CreditControlArea,
	'' AS IndicatorNegativePosting,
	'' AS FiscalYear,
	'' AS PurchasingDocNum,
	'0' AS ItemNumOfPO,
	'' AS ISRAccount,
	'' AS ISRReference,
	'' AS Plant,
	'' AS Identifier
	FROM @TempTable1
	--WHERE LocationCode = 'IDAD'

	UNION ALL

	SELECT DISTINCT
	'4' as no,
	TPOFeeCode,
	FlowStatus,
	LocationCode, 
	'2' AS counter,
	'' AS Company,
	'IDR' AS CurrencyKey,
	'' AS DocumentType,
	@DD + '.' + @MM + '.' + @YY AS TaxDate,
	'' AS DocumentHeaderText,
	'' AS Reference,
	@DDCurr + '.' + @MMCurr + '.' + @YYCurr AS DocumentDate,
	'' AS SCBIndocator,
	'' AS PaymentReference,
	'' AS HouseBank,
	'0' AS InstructionKey1,
	'0' AS InstructionKey2,
	'0' AS InstructionKey3,
	'0' AS InstructionKey4,
	'' AS PartnerBankType,
	'' AS ReferenceText1,
	'40' AS PostingKey,
	'82182010' AS Account,
	(JasaManajemen+PPNJasaManajemen10Prcnt) AS AmountInDocumentCurrency,
	'IDR' AS LocalCurrency,
	'V4' AS TaxCode,
	'0' AS Quantity,
	'' AS BaseUnitOfMeasure,
	VendorName AS Assignment,
	(
		SKTBrandCode +'-'+ LocationCode+ 
		CONVERT(VARCHAR(2), @ItemStartDate, 103) +'.'+CONVERT(VARCHAR(2), @ItemStartDate, 101)+'.'+SUBSTRING( CONVERT(varchar(4),(SELECT DATEPART( YY, @ItemStartDate))),3,2) +'-'+
		CONVERT(VARCHAR(2), @ItemEndDate, 103) +'.'+CONVERT(VARCHAR(2), @ItemEndDate, 101)+'.'+SUBSTRING( CONVERT(varchar(4),(SELECT DATEPART( YY, @ItemEndDate))),3,2) +
		'(w.'+CONVERT(VARCHAR(2),@Week)+')MgmtFee'
	) AS ItemText,
	'' AS LongText,
	'3066150VR3' AS CostCenter,
	'' AS WBSElement,
	'' AS MaterialNumber,
	'' AS PaymentTerms,
	'' AS TradingPartner,
	'' AS [Order],
	'' AS ShipToParty,
	'' AS CustomerCOPA,
	'' AS BrandFamilyCOPA,
	'' AS IntercoAffiliateCOPA,
	'' AS WBSCOPA,
	'' AS ProductCOPA,
	'' AS ProductionCenterCOPA,
	'' AS PMIMarketCOPA,
	'' AS ProductCategoryCOPA,
	'' AS LabelCOPA,
	'' AS TradeChannelCOPA,
	'' AS ShipToPartyCOPA,
	'' AS FinalMarketCOPA,
	'' AS CreditControlArea,
	'' AS IndicatorNegativePosting,
	'' AS FiscalYear,
	'' AS PurchasingDocNum,
	'0' AS ItemNumOfPO,
	'' AS ISRAccount,
	'' AS ISRReference,
	'' AS Plant,
	'' AS Identifier
	FROM @TempTable1
	--WHERE LocationCode = 'IDAD'

	UNION ALL

	SELECT DISTINCT
	'5' as no,
	TPOFeeCode,
	FlowStatus,
	LocationCode, 
	'2' AS counter,
	'' AS Company,
	'IDR' AS CurrencyKey,
	'' AS DocumentType,
	@DD + '.' + @MM + '.' + @YY AS TaxDate,
	'' AS DocumentHeaderText,
	'' AS Reference,
	@DDCurr + '.' + @MMCurr + '.' + @YYCurr AS DocumentDate,
	'' AS SCBIndocator,
	'' AS PaymentReference,
	'' AS HouseBank,
	'0' AS InstructionKey1,
	'0' AS InstructionKey2,
	'0' AS InstructionKey3,
	'0' AS InstructionKey4,
	'' AS PartnerBankType,
	'' AS ReferenceText1,
	'50' AS PostingKey,
	'22071000' AS Account,
	PajakJasaManajemenSebesar2Prcnt AS AmountInDocumentCurrency,
	'IDR' AS LocalCurrency,
	'' AS TaxCode,
	'0' AS Quantity,
	'' AS BaseUnitOfMeasure,
	'' AS Assignment,
	'' AS ItemText,
	'' AS LongText,
	'' AS CostCenter,
	'' AS WBSElement,
	'' AS MaterialNumber,
	'' AS PaymentTerms,
	'' AS TradingPartner,
	'' AS [Order],
	'' AS ShipToParty,
	'' AS CustomerCOPA,
	'' AS BrandFamilyCOPA,
	'' AS IntercoAffiliateCOPA,
	'' AS WBSCOPA,
	'' AS ProductCOPA,
	'' AS ProductionCenterCOPA,
	'' AS PMIMarketCOPA,
	'' AS ProductCategoryCOPA,
	'' AS LabelCOPA,
	'' AS TradeChannelCOPA,
	'' AS ShipToPartyCOPA,
	'' AS FinalMarketCOPA,
	'' AS CreditControlArea,
	'' AS IndicatorNegativePosting,
	'' AS FiscalYear,
	'' AS PurchasingDocNum,
	'0' AS ItemNumOfPO,
	'' AS ISRAccount,
	'' AS ISRReference,
	'' AS Plant,
	'' AS Identifier
	FROM @TempTable1
	--WHERE LocationCode = 'IDAD'
	ORDER BY TPOFeeCode, no,counter ASC

END