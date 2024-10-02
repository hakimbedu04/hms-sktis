IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[dbo].[GenerateP1TemplateAP1]'))
	DROP PROCEDURE [dbo].[GenerateP1TemplateAP1]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROC [dbo].[GenerateP1TemplateAP1]
(
		@ParamLocation VARCHAR(8),
		@ParamWeek     INT,
		@ParamYear	   INT,
		@ParamPage		   VARCHAR(8)
)
AS
BEGIN
	-- author : hakim
	-- ticket : http://tp.voxteneo.co.id/entity/11020
	-- description : create p1 template ap
	-- date : 2016-11-09
	
	-- author : hakim
	-- ticket : http://tp.voxteneo.co.id/entity/11020
	-- description : add @ParamPage as parameter
	-- date : 2016-11-10

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
			FlowStatus VARCHAR(12),
			BrandGroupCode VARCHAR(20), 
			CreatedDate DATETIME);

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

	IF(@ParamPage = 'APOPEN')
	BEGIN
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
			END AS FlowStatus, TFH.BrandGroupCode, TFH.CreatedDate
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
			ON Cal5.TPOFeeCode = TFC.TPOFeeCode
			AND Cal5.OrderFeeType = TFC.OrderFeeType
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
			AND MGL.ParentLocationCode = @Location
			AND utl.IDFlow in (45,46); -- authorized, completed
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
			END AS FlowStatus, TFH.BrandGroupCode, TFH.CreatedDate
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
			ON Cal5.TPOFeeCode = TFC.TPOFeeCode
			AND Cal5.OrderFeeType = TFC.OrderFeeType
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
			AND utl.IDFlow in (45,46); -- authorized, completed
		END
	END
	ELSE
	BEGIN
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
			END AS FlowStatus, TFH.BrandGroupCode, TFH.CreatedDate
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
			ON Cal5.TPOFeeCode = TFC.TPOFeeCode
			AND Cal5.OrderFeeType = TFC.OrderFeeType
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
			AND MGL.ParentLocationCode = @Location
			AND utl.IDFlow in (46,48,51); -- completed, end
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
			END AS FlowStatus, TFH.BrandGroupCode, TFH.CreatedDate
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
			ON Cal5.TPOFeeCode = TFC.TPOFeeCode
			AND Cal5.OrderFeeType = TFC.OrderFeeType
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
			AND utl.IDFlow in (46,48,51); --completed / end
		END
	END

	--SELECT * FROM @TempTable1
SELECT DISTINCT
	'Location Code' as LocationCode, 
	'0' as no,
	'ATPO Fee Code' as TPOFeeCode,
	'Flow Status' as FlowStatus,
	'counter' AS counter,
	'Company' AS Company,
	'Currency Key' AS CurrencyKey,
	'Document Type' AS DocumentType,
	'Tax Date' AS TaxDate,
	'Document Header Text' AS DocumentHeaderText,
	'Reference' AS Reference,
	'Document Date' AS DocumentDate,
	'SCB Indocator' AS SCBIndocator,
	'Payment Reference' AS PaymentReference,
	'House Bank' AS HouseBank,
	'Instruction Key 1' AS InstructionKey1,
	'Instruction Key 2' AS InstructionKey2,
	'Instruction Key 3' AS InstructionKey3,
	'Instruction Key 4' AS InstructionKey4,
	'Partner Bank Type' AS PartnerBankType,
	'Reference Text 1' AS ReferenceText1,
	'Posting Key' AS PostingKey,
	'Account' AS Account,
	'Amount In Document Currency' AS AmountInDocumentCurrency,
	'Local Currency' AS LocalCurrency,
	'Tax Code' AS TaxCode,
	'Quantity' AS Quantity,
	'Base Unit Of Measure' AS BaseUnitOfMeasure,
	'Assignment' AS Assignment,
	'Item Text' AS ItemText,
	'Long Text' AS LongText,
	'Cost Center' AS CostCenter,
	'WBS Element' AS WBSElement,
	'Material Number' AS MaterialNumber,
	'Payment Terms' AS PaymentTerms,
	'Trading Partner' AS TradingPartner,
	'Order' AS [Order],
	'Ship To Party' AS ShipToParty,
	'Customer CO-PA' AS CustomerCOPA,
	'Brand Family CO-PA' AS BrandFamilyCOPA,
	'Interco Affiliate CO-PA' AS IntercoAffiliateCOPA,
	'WBS CO-PA' AS WBSCOPA,
	'Product CO-PA' AS ProductCOPA,
	'Production Center CO-PA' AS ProductionCenterCOPA,
	'PMI Market CO-PA' AS PMIMarketCOPA,
	'Product Category CO-PA' AS ProductCategoryCOPA,
	'Label CO-PA' AS LabelCOPA,
	'Trade Channel CO-PA' AS TradeChannelCOPA,
	'Ship-To Party COPA' AS ShipToPartyCOPA,
	'Final Market COPA' AS FinalMarketCOPA,
	'Credit Control Area' AS CreditControlArea,
	'Indicator Negative Posting' AS IndicatorNegativePosting,
	'Fiscal Year' AS FiscalYear,
	'Purchasing Document Number' AS PurchasingDocNum,
	'Item Number Of PO' AS ItemNumOfPO,
	'ISR Account' AS ISRAccount,
	'ISR Reference' AS ISRReference,
	'Plant' AS Plant,
	'Identifier' AS Identifier	

	UNION ALL

	SELECT DISTINCT
	LocationCode, 
	'1' as no,
	TPOFeeCode,
	FlowStatus,
	
	'1' AS counter,
	'3066' AS Company,
	'IDR' AS CurrencyKey,
	'KD' AS DocumentType,
	@DD + '.' + @MM + '.' + @YY AS TaxDate,
	ISNULL(TaxtNoProd,'') AS DocumentHeaderText,
	LocationCode + SUBSTRING(BrandGroupCode,1,5)+SUBSTRING(BrandGroupCode,9,2)+ltrim(str(CAST(CreatedDate as int))) AS Reference,
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
	LocationCode, 
	'2' as no,
	TPOFeeCode,
	FlowStatus,
	
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
	 --CONVERT(VARCHAR(128),((BiayaProduksi+PPNBiayaProduksi10Prcnt) + (JasaManajemen+PPNJasaManajemen10Prcnt)) - PajakJasaManajemenSebesar2Prcnt)
	 --AS AmountInDocumentCurrency,
	 LTRIM(str(((BiayaProduksi+PPNBiayaProduksi10Prcnt) + (JasaManajemen+PPNJasaManajemen10Prcnt)) - PajakJasaManajemenSebesar2Prcnt,25,0))
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
	LocationCode, 
	'3' as no,
	TPOFeeCode,
	FlowStatus,
	
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
	--CONVERT(VARCHAR(128),(BiayaProduksi + PPNBiayaProduksi10Prcnt)) AS AmountInDocumentCurrency,
	LTRIM(str((BiayaProduksi + PPNBiayaProduksi10Prcnt),25,0)) AS AmountInDocumentCurrency,
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
	LocationCode, 
	'4' as no,
	TPOFeeCode,
	FlowStatus,
	
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
	--CONVERT(VARCHAR(128),(JasaManajemen+PPNJasaManajemen10Prcnt)) AS AmountInDocumentCurrency,
	LTRIM(str((JasaManajemen+PPNJasaManajemen10Prcnt),25,0)) AS AmountInDocumentCurrency,
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
	LocationCode, 
	'5' as no,
	TPOFeeCode,
	FlowStatus,
	
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
	--CONVERT(VARCHAR(128),PajakJasaManajemenSebesar2Prcnt) AS AmountInDocumentCurrency,
	LTRIM(str(PajakJasaManajemenSebesar2Prcnt,25,0)) AS AmountInDocumentCurrency,
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
