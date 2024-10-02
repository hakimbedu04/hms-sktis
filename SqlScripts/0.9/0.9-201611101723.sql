IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[dbo].[GenerateP1TemplateGL1]'))
	DROP PROCEDURE [dbo].[GenerateP1TemplateGL1]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROC [dbo].[GenerateP1TemplateGL1]
(
	-- begin : parameter function
	@ParamdateTo DATETIME,
	@ParamWeek INT,
	@paramYear INT,
	@ParamLocation VARCHAR(8)
	-- end : parameter function
)
AS
BEGIN
	-- author : hakim
	-- ticket : http://tp.voxteneo.co.id/entity/11020
	-- description : create p1 template GL using ado.net connection
	-- date : 2016-11-10

	-- author : hakim
	-- ticket : http://tp.voxteneo.co.id/entity/11020
	-- description : TPO order by regional and locationcode, reg order by locationcode
	-- date : 2016-11-10
	
	-- declare local parameter snipping
	DECLARE @dateFrom DATETIME;
	DECLARE @dateTo DATETIME;
	DECLARE @Week INT;
	DECLARE @Year INT;
	DECLARE @Location VARCHAR(8);
	DECLARE @DocCurAmountProd Float;
	DECLARE @DocCurAmountMgmt Float;
	DECLARE @TempTable1 TABLE (TPOFeeCode VARCHAR(64), KPSWeek INT, KPSYear INT, DocCurAmount Float, ParentLocationCode VARCHAR(8),LocationCode VARCHAR(8));
	DECLARE @TempTable2 TABLE (BrandGroupCode VARCHAR(20), LocationCode VARCHAR(8), DocCurAmount Numeric(20,0), ParentLocationCode VARCHAR(8));

	SET @Year = @paramYear
	SET @Week = @ParamWeek
	SELECT @dateFrom = StartDate from MstGenWeek where Week = @Week and Year = @Year
	SET @dateTo = @ParamdateTo
	SET @Location = @ParamLocation

	IF(@Location = 'TPO')
	BEGIN
		-- begin : Calculate prod fee
		INSERT INTO @TempTable1
		SELECT X.TPOFeeCode,X.KPSWeek,X.KPSYear, SUM(X.DocCurAmount) as DocCurAmount,X.ParentLocationCode,X.LocationCode
		FROM (
				SELECT TDP.TPOFeeCode,TDP.[FeeDate],TDP.[KPSYear],TDP.[KPSWeek],
				(JKNRp+JL1Rp+JL2Rp+JL3Rp+JL4Rp) as DocCurAmount, MGL.ParentLocationCode,FHP.LocationCode
				FROM [TPOFeeProductionDailyPlan] TDP
				INNER JOIN TPOFeeHdrPlan FHP
				ON tdp.TPOFeeCode=FHP.TPOFeeCode
				INNER JOIN [MstGenLocation] MGL
				ON MGL.LocationCode=FHP.LocationCode
				WHERE TDP.FeeDate >= @dateFrom
				and TDP.FeeDate <= @dateTo
				and TDP.KPSWeek = @Week
				and TDP.KPSYear = @Year) X
		WHERE X.FeeDate >= @dateFrom
		and X.FeeDate <= @dateTo
		and X.KPSWeek = @Week
		and X.KPSYear = @Year
		GROUP BY X.TPOFeeCode,X.KPSWeek,X.KPSYear,X.ParentLocationCode,X.LocationCode
		ORDER BY X.ParentLocationCode,X.LocationCode
		-- end : Calculate prod fee

		-- begin : Calculate Mgmt fee
		INSERT INTO @TempTable2
		SELECT Y.BrandGroupCode,Y.LocationCode,SUM(Y.DocCurAmount),Y.ParentLocationCode
		FROM (
			SELECT TFR.EffectiveDate,TFR.ExpiredDate, TFR.BrandGroupCode,TFR.LocationCode,
			(ManagementFee+JKN+JL1+Jl2+Jl3+Jl4) as DocCurAmount, MGL.ParentLocationCode
			FROM MstTPOFeeRate TFR
			INNER JOIN [MstGenLocation] MGL
			ON MGL.LocationCode=TFR.LocationCode
			WHERE  @dateFrom BETWEEN TFR.EffectiveDate and TFR.ExpiredDate
			) Y
		INNER JOIN TPOFeeHdrPlan A
		ON A.LocationCode=Y.LocationCode
		WHERE a.KPSWeek = @Week
		and a.KPSYear = @Year
		and a.BrandGroupCode=Y.BrandGroupCode
		GROUP BY Y.LocationCode,Y.BrandGroupCode,Y.ParentLocationCode
		ORDER BY Y.ParentLocationCode ,Y.LocationCode
		-- end : Calculate Mgmt fee
	END
	ELSE 
	BEGIN
		-- begin : Calculate prod fee
		INSERT INTO @TempTable1
		SELECT Q.TPOFeeCode,Q.KPSWeek,Q.KPSYear, SUM(Q.DocCurAmount) as DocCurAmount,Q.ParentLocationCode,Q.LocationCode
		FROM (
				SELECT TDP.TPOFeeCode,TDP.[FeeDate],TDP.[KPSYear],TDP.[KPSWeek],
				(JKNRp+JL1Rp+JL2Rp+JL3Rp+JL4Rp) as DocCurAmount,MGL.ParentLocationCode, FHP.LocationCode
				FROM [TPOFeeProductionDailyPlan] TDP
				INNER JOIN TPOFeeHdrPlan FHP
				ON tdp.TPOFeeCode=FHP.TPOFeeCode
				INNER JOIN [MstGenLocation] MGL
				ON MGL.LocationCode=FHP.LocationCode
				WHERE TDP.FeeDate >= @dateFrom
				and TDP.FeeDate <= @dateTo
				and TDP.KPSWeek = @Week
				and TDP.KPSYear = @Year
				and MGL.ParentLocationCode = @Location) Q
		WHERE Q.FeeDate >= @dateFrom
		and Q.FeeDate <= @dateTo
		and Q.KPSWeek = @Week
		and Q.KPSYear = @Year
		GROUP BY Q.TPOFeeCode,Q.KPSWeek,Q.KPSYear,Q.ParentLocationCode,Q.LocationCode
		ORDER BY Q.LocationCode
		-- end : Calculate prod fee

		-- begin : Calculate Mgmt fee
		INSERT INTO @TempTable2
		SELECT Y.BrandGroupCode,Y.LocationCode,SUM(Y.DocCurAmount),Y.ParentLocationCode
		FROM (
			SELECT TFR.EffectiveDate,TFR.ExpiredDate, TFR.BrandGroupCode,TFR.LocationCode,
			(ManagementFee+JKN+JL1+Jl2+Jl3+Jl4) as DocCurAmount,MGL.ParentLocationCode
			FROM MstTPOFeeRate TFR
			INNER JOIN [MstGenLocation] MGL
			ON MGL.LocationCode=TFR.LocationCode
			WHERE  @dateFrom BETWEEN TFR.EffectiveDate and TFR.ExpiredDate
			AND MGL.ParentLocationCode = @Location
			) Y
		INNER JOIN TPOFeeHdrPlan A
		ON A.LocationCode=Y.LocationCode
		WHERE a.KPSWeek = @Week
		and a.KPSYear = @Year
		and a.BrandGroupCode=Y.BrandGroupCode
		GROUP BY Y.LocationCode,Y.BrandGroupCode,Y.ParentLocationCode
		ORDER BY Y.LocationCode
		-- end : Calculate Mgmt fee
	END

	SELECT TOP 1 @DocCurAmountProd = SUM(DocCurAmount) FROM @TempTable1;
	SELECT TOP 1 @DocCurAmountMgmt = SUM(DocCurAmount) FROM @TempTable2;
	
	SELECT DISTINCT
	'ParentLocationCode' AS ParentLocationCode,
	'LocationCode' as LocationCode,
	'Type' as Type,
	'Company' as Company,
	'Currency' as Currency,
	'Exchange Rate' as ExchangeRate,
	'Document Type' as DocumentType,
	'Translation Date' as TranslationDate,
	'Header Text' as HeaderText,
	'Reference' as Reference,
	'CC Transaction' as CCTransaction,
	'Document Date' as DocumentDate,
	'Posting Date' as PostingDate,
	'Automatic Tax' as AutomaticTax,
	'Posting Key' as PostingKey,
	'Account' as Account,
	'Doc Cur Amount' as DocCurAmount,
	'Local Cur Amount' as LocalCurAmount,
	'Local Currency' as LocalCurrency,
	'Tax Code' as TaxCode,
	'PO Number' as PONumber,
	'PO Item Number' as POItemNumber,
	'Quantity' as Quantity,
	'UOM' as UOM,
	'Assignment' as Assignment,
	'Text' as Text,
	'Special GL Indicator' as SpecialGLIndicator,
	'Recovery Indicator' as RecoveryIndicator,
	'Customer' as Customer,
	'Baseline Date' as BaselineDate,
	'Value Date' as ValueDate,
	'Cost Center' as CostCenter,
	'WBS' as WBS,
	'Material Number' as MaterialNumber,
	'Brand Family' as BrandFamily,
	'Payment Terms' as PaymentTerms,
	'Cash Discount 1' as CashDiscount1,
	'Trading Partner' as TradingPartner,
	'New Company' as NewCompany,
	'Interco Affiliate' as IntercoAffiliate,
	'Production Center' as ProductionCenter,
	'PMI Market' as PMIMarket,
	'Product Category' as ProductCategory,
	'Ship-To' as ShipTo,
	'Label' as Label,
	'Final Market' as FinalMarket,
	'DocNumber-EarMarked Funds' as DocNumberEarMarkedFunds,
	'EarMarked Funds' as EarMarkedFunds,
	'Tax Based Amount' as TaxBasedAmount,
	'Withholding Tax Base Amount' as WithholdingTaxBaseAmount,
	'Batch Number' as BatchNumber,
	'Business Place' as BusinessPlace,
	'Section Code' as SectionCode,
	'Amount in 2nd Local Currency' as AmountIn2ndLocalCurrency,
	'Amount in 3rd Local Currency' as AmountIn3ndLocalCurrency,
	'W.Tax Code' as WTaxCode

	UNION ALL

	SELECT DISTINCT
	'' AS ParentLocationCode,
	'' as LocationCode,
	'1' as Type,
	'3066' as Company,
	'IDR' as Currency,
	'' as ExchangeRate,
	'SC' as DocumentType,
	'' as TranslationDate,
	('AccrTPOFee'+ SUBSTRING(DATENAME(MONTH,dateadd(month, 0,@dateFrom)),1,3)+'(w.'+CONVERT(VARCHAR(2),@Week)+')'+CONVERT(VARCHAR(4),@Year)) as HeaderText,
	'3066' + CONVERT(varchar(4),(SELECT DATEPART( YY, @dateTo))) + CONVERT(VARCHAR(2), @dateTo, 101) as Reference,
	'' as CCTransaction,
	CONVERT(VARCHAR(2),DATEPART(DD,@dateFrom)) +'.'+CONVERT(VARCHAR(2), @dateFrom, 101)+'.'+SUBSTRING( CONVERT(varchar(4),(SELECT DATEPART( YY, @dateFrom))),3,2) as DocumentDate,
	CONVERT(VARCHAR(2),DATEPART(DD,@dateFrom)) +'.'+CONVERT(VARCHAR(2), @dateFrom, 101)+'.'+SUBSTRING( CONVERT(varchar(4),(SELECT DATEPART( YY, @dateFrom))),3,2) as PostingDate,
	'' as AutomaticTex,
	'' as PostingKey,
	'' as Account,
	'' as DocCurAmount,
	'' as LocalCurAmount,
	'' as LocalCurrency,
	'' as TaxCode,
	'' as PONumber,
	'' as POItemNumber,
	'' as Quantity,
	'' as UOM,
	'' as Assignment,
	'' as Text,
	'' as SpecialGLIndicator,
	'' as RecoveryIndicator,
	'' as Customer,
	'' as BaselineDate,
	'' as ValueDate,
	'' as CostCenter,
	'' as WBS,
	'' as MaterialNumber,
	'' as BrandFamily,
	'' as PaymentTerms,
	'' as CashDiscount1,
	'' as TradingPartner,
	'' as NewCompany,
	'' as IntercoAffiliate,
	'' as ProductionCenter,
	'' as PMIMarket,
	'' as ProductCategory,
	'' as ShipTo,
	'' as Label,
	'' as FinalMarket,
	'' as DocNumberEarMarkedFunds,
	'' as EarMarkedFunds,
	'' as TaxBasedAmount,
	'' as WithholdingTaxBaseAmount,
	'' as BatchNumber,
	'' as BusinessPlace,
	'' as SectionCode,
	'' as AmountIn2ndLocalCurrency,
	'' as AmountIn3ndLocalCurrency,
	'' as WTaxCode

	UNION ALL

	SELECT DISTINCT
	'ParentLocationCode' AS ParentLocationCode,
	'' as LocationCode,
	'2' as Type,
	'' as Company,
	'' as Currency,
	'' as ExchangeRate,
	'' as DocumentType,
	'' as TranslationDate,
	'' as HeaderText,
	'' as Reference,
	'' as CCTransaction,
	'' as DocumentDate,
	'' as PostingDate,
	'' as AutomaticTex,
	'50' as PostingKey,
	'22561000' as Account,
	LTRIM(str(@DocCurAmountProd,25,0)) as DocCurAmount,
	'' as LocalCurAmount,
	'' as LocalCurrency,
	'V0' as TaxCode,
	'' as PONumber,
	'' as POItemNumber,
	'' as Quantity,
	'' as UOM,
	'' as Assignment,
	('AccrTPOFee'+ SUBSTRING(DATENAME(MONTH,dateadd(month, 0,@dateFrom)),1,3)+'(w.'+CONVERT(VARCHAR(2),@Week)+')'+CONVERT(VARCHAR(4),@Year)+'-Prod') as Text,
	'' as SpecialGLIndicator,
	'' as RecoveryIndicator,
	'' as Customer,
	'' as BaselineDate,
	'' as ValueDate,
	'' as CostCenter,
	'' as WBS,
	'' as MaterialNumber,
	'' as BrandFamily,
	'' as PaymentTerms,
	'' as CashDiscount1,
	'' as TradingPartner,
	'' as NewCompany,
	'' as IntercoAffiliate,
	'' as ProductionCenter,
	'' as PMIMarket,
	'' as ProductCategory,
	'' as ShipTo,
	'' as Label,
	'' as FinalMarket,
	'' as DocNumberEarMarkedFunds,
	'' as EarMarkedFunds,
	'' as TaxBasedAmount,
	'' as WithholdingTaxBaseAmount,
	'' as BatchNumber,
	'' as BusinessPlace,
	'' as SectionCode,
	'' as AmountIn2ndLocalCurrency,
	'' as AmountIn3ndLocalCurrency,
	'' as WTaxCode
	FROM @TempTable1

	UNION ALL

	SELECT DISTINCT
	'ParentLocationCode' AS ParentLocationCode,
	'' as LocationCode,
	'2' as Type,
	'' as Company,
	'' as Currency,
	'' as ExchangeRate,
	'' as DocumentType,
	'' as TranslationDate,
	'' as HeaderText,
	'' as Reference,
	'' as CCTransaction,
	'' as DocumentDate,
	'' as PostingDate,
	'' as AutomaticTex,
	'50' as PostingKey,
	'22561000' as Account,
	LTRIM(str(@DocCurAmountMgmt,25,0)) as DocCurAmount,
	'' as LocalCurAmount,
	'' as LocalCurrency,
	'V0' as TaxCode,
	'' as PONumber,
	'' as POItemNumber,
	'' as Quantity,
	'' as UOM,
	'' as Assignment,
	('AccrTPOFee'+ SUBSTRING(DATENAME(MONTH,dateadd(month, 0,@dateFrom)),1,3)+'(w.'+CONVERT(VARCHAR(2),@Week)+')'+CONVERT(VARCHAR(4),@Year)+'-Maklon') as Text,
	'' as SpecialGLIndicator,
	'' as RecoveryIndicator,
	'' as Customer,
	'' as BaselineDate,
	'' as ValueDate,
	'' as CostCenter,
	'' as WBS,
	'' as MaterialNumber,
	'' as BrandFamily,
	'' as PaymentTerms,
	'' as CashDiscount1,
	'' as TradingPartner,
	'' as NewCompany,
	'' as IntercoAffiliate,
	'' as ProductionCenter,
	'' as PMIMarket,
	'' as ProductCategory,
	'' as ShipTo,
	'' as Label,
	'' as FinalMarket,
	'' as DocNumberEarMarkedFunds,
	'' as EarMarkedFunds,
	'' as TaxBasedAmount,
	'' as WithholdingTaxBaseAmount,
	'' as BatchNumber,
	'' as BusinessPlace,
	'' as SectionCode,
	'' as AmountIn2ndLocalCurrency,
	'' as AmountIn3ndLocalCurrency,
	'' as WTaxCode
	FROM @TempTable2

	UNION ALL

	SELECT
	b.ParentLocationCode,
	a.LocationCode,
	'2' as Type,
	'' as Company,
	'' as Currency,
	'' as ExchangeRate,
	'' as DocumentType,
	'' as TranslationDate,
	'' as HeaderText,
	'' as Reference,
	'' as CCTransaction,
	'' as DocumentDate,
	'' as PostingDate,
	'' as AutomaticTex,
	'40' as PostingKey,
	'82182000' as Account,
	LTRIM(str(b.DocCurAmount,25,0)) AS DocCurAmount,
	'' as LocalCurAmount,
	'' as LocalCurrency,
	'V0' as TaxCode,
	'' as PONumber,
	'' as POItemNumber,
	'' as Quantity,
	'' as UOM,
	'' as Assignment,
	(c.SKTBrandCode +' - '+ a.LocationCode +' '+ 
	CONVERT(VARCHAR(2),DATEPART(DD,@dateFrom)) +'.'+CONVERT(VARCHAR(2), @dateFrom, 101)+'.'+SUBSTRING( CONVERT(varchar(4),(SELECT DATEPART( YY, @dateFrom))),3,2) +'-'+
	CONVERT(VARCHAR(2),DATEPART(DD,@dateTo)) +'.'+CONVERT(VARCHAR(2), @dateTo, 101)+'.'+SUBSTRING( CONVERT(varchar(4),(SELECT DATEPART( YY, @dateTo))),3,2) + ' ' +
	'(w.'+CONVERT(VARCHAR(2),@Week)+') Prod Fee') as Text,
	'' as SpecialGLIndicator,
	'' as RecoveryIndicator,
	'' as Customer,
	'' as BaselineDate,
	'' as ValueDate,
	d.CostCenter,
	'' as WBS,
	'' as MaterialNumber,
	'' as BrandFamily,
	'' as PaymentTerms,
	'' as CashDiscount1,
	'' as TradingPartner,
	'' as NewCompany,
	'' as IntercoAffiliate,
	'' as ProductionCenter,
	'' as PMIMarket,
	'' as ProductCategory,
	'' as ShipTo,
	'' as Label,
	'' as FinalMarket,
	'' as DocNumberEarMarkedFunds,
	'' as EarMarkedFunds,
	'' as TaxBasedAmount,
	'' as WithholdingTaxBaseAmount,
	'' as BatchNumber,
	'' as BusinessPlace,
	'' as SectionCode,
	'' as AmountIn2ndLocalCurrency,
	'' as AmountIn3ndLocalCurrency,
	'' as WTaxCode
	FROM TPOFeeHdrPlan a 
	INNER JOIN @TempTable1 b
	ON a.TPOFeeCode = b.TPOFeeCode
	INNER JOIN [MstGenBrandGroup] c
	ON a.[BrandGroupCode]=c.[BrandGroupCode]
	INNER JOIN [MstGenLocation] d
	ON a.LocationCode=d.LocationCode
	WHERE a.KPSWeek = @Week 
	AND a.KPSYear = @Year

	UNION ALL

	SELECT
	a.ParentLocationCode,
	a.LocationCode,
	'2' as Type,
	'' as Company,
	'' as Currency,
	'' as ExchangeRate,
	'' as DocumentType,
	'' as TranslationDate,
	'' as HeaderText,
	'' as Reference,
	'' as CCTransaction,
	'' as DocumentDate,
	'' as PostingDate,
	'' as AutomaticTex,
	'40' as PostingKey,
	'82182010' as Account,
	LTRIM(str(a.DocCurAmount,25,0)) as DocCurAmount,
	'' as LocalCurAmount,
	'' as LocalCurrency,
	'V0' as TaxCode,
	'' as PONumber,
	'' as POItemNumber,
	'' as Quantity,
	'' as UOM,
	'' as Assignment,
	(c.SKTBrandCode +' - '+ a.LocationCode +' '+ 
	CONVERT(VARCHAR(2),DATEPART(DD,@dateFrom)) +'.'+CONVERT(VARCHAR(2), @dateFrom, 101)+'.'+SUBSTRING( CONVERT(varchar(4),(SELECT DATEPART( YY, @dateFrom))),3,2) +'-'+
	CONVERT(VARCHAR(2),DATEPART(DD,@dateTo)) +'.'+CONVERT(VARCHAR(2), @dateTo, 101)+'.'+SUBSTRING( CONVERT(varchar(4),(SELECT DATEPART( YY, @dateTo))),3,2) + ' ' +
	'(w.'+CONVERT(VARCHAR(2),@Week)+') Mgmt Fee') as Text,
	'' as SpecialGLIndicator,
	'' as RecoveryIndicator,
	'' as Customer,
	'' as BaselineDate,
	'' as ValueDate,
	d.CostCenter,
	'' as WBS,
	'' as MaterialNumber,
	'' as BrandFamily,
	'' as PaymentTerms,
	'' as CashDiscount1,
	'' as TradingPartner,
	'' as NewCompany,
	'' as IntercoAffiliate,
	'' as ProductionCenter,
	'' as PMIMarket,
	'' as ProductCategory,
	'' as ShipTo,
	'' as Label,
	'' as FinalMarket,
	'' as DocNumberEarMarkedFunds,
	'' as EarMarkedFunds,
	'' as TaxBasedAmount,
	'' as WithholdingTaxBaseAmount,
	'' as BatchNumber,
	'' as BusinessPlace,
	'' as SectionCode,
	'' as AmountIn2ndLocalCurrency,
	'' as AmountIn3ndLocalCurrency,
	'' as WTaxCode
	FROM @TempTable2 a
	INNER JOIN [MstGenBrandGroup] c
	ON a.[BrandGroupCode]=c.[BrandGroupCode]
	INNER JOIN [MstGenLocation] d
	ON a.LocationCode=d.LocationCode;

END
