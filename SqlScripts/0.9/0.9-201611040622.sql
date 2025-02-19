-- author : hakim
-- ticket : http://tp.voxteneo.co.id/entity/11020
-- description : create p1 template gl
-- date : 2016-11-04
/****** Object:  StoredProcedure [dbo].[GenerateP1TemplateGL]    Script Date: 10/21/2016 2:46:10 PM ******/
IF  EXISTS (SELECT * FROM sys.procedures WHERE object_id = OBJECT_ID(N'[dbo].[GenerateP1TemplateGL]'))
	DROP PROCEDURE [dbo].[GenerateP1TemplateGL]
GO

/****** Object:  StoredProcedure [dbo].[GenerateP1TemplateGL]    Script Date: 10/21/2016 2:46:10 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROC [dbo].[GenerateP1TemplateGL]
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
	
	-- declare local parameter snipping
	DECLARE @dateFrom DATETIME;
	DECLARE @dateTo DATETIME;
	DECLARE @Week INT;
	DECLARE @Year INT;
	DECLARE @Location VARCHAR(8);
	DECLARE @DocCurAmountProd Float;
	DECLARE @DocCurAmountMgmt Float;
	DECLARE @TempTable1 TABLE (TPOFeeCode VARCHAR(64), KPSWeek INT, KPSYear INT, DocCurAmount Float);
	DECLARE @TempTable2 TABLE (BrandGroupCode VARCHAR(20), LocationCode VARCHAR(8), DocCurAmount Numeric(20,0));

	SET @Year = @paramYear
	SET @Week = @ParamWeek
	SELECT @dateFrom = StartDate from MstGenWeek where Week = @Week and Year = @Year
	SET @dateTo = @ParamdateTo
	SET @Location = @ParamLocation

	IF(@Location = 'TPO')
	BEGIN
		-- begin : Calculate prod fee
		INSERT INTO @TempTable1
		SELECT X.TPOFeeCode,X.KPSWeek,X.KPSYear, SUM(X.DocCurAmount) as DocCurAmount
		FROM (
				SELECT TPOFeeCode,[FeeDate],[KPSYear],[KPSWeek],
				(JKNRp+JL1Rp+JL2Rp+JL3Rp+JL4Rp) as DocCurAmount
				FROM [TPOFeeProductionDailyPlan]
				WHERE FeeDate >= @dateFrom
				and FeeDate <= @dateTo
				and KPSWeek = @Week
				and KPSYear = @Year) X
		WHERE X.FeeDate >= @dateFrom
		and X.FeeDate <= @dateTo
		and X.KPSWeek = @Week
		and X.KPSYear = @Year
		GROUP BY X.TPOFeeCode,X.KPSWeek,X.KPSYear
		-- end : Calculate prod fee

		-- begin : Calculate Mgmt fee
		INSERT INTO @TempTable2
		SELECT Y.BrandGroupCode,Y.LocationCode,SUM(Y.DocCurAmount)
		FROM (
			SELECT EffectiveDate,ExpiredDate, BrandGroupCode,LocationCode,
			(ManagementFee+JKN+JL1+Jl2+Jl3+Jl4) as DocCurAmount
			FROM MstTPOFeeRate
			WHERE  @dateFrom BETWEEN EffectiveDate and ExpiredDate
			) Y
		INNER JOIN TPOFeeHdrPlan A
		ON A.LocationCode=Y.LocationCode
		WHERE a.KPSWeek = @Week
		and a.KPSYear = @Year
		and a.BrandGroupCode=Y.BrandGroupCode
		GROUP BY Y.LocationCode,Y.BrandGroupCode
		ORDER BY Y.LocationCode
		-- end : Calculate Mgmt fee
	END
	ELSE 
	BEGIN
		-- begin : Calculate prod fee
		INSERT INTO @TempTable1
		SELECT X.TPOFeeCode,X.KPSWeek,X.KPSYear, SUM(X.DocCurAmount) as DocCurAmount
		FROM (
				SELECT TDP.TPOFeeCode,TDP.[FeeDate],TDP.[KPSYear],TDP.[KPSWeek],MGL.LocationCode,MGL.ParentLocationCode,
				(JKNRp+JL1Rp+JL2Rp+JL3Rp+JL4Rp) as DocCurAmount
				FROM [TPOFeeProductionDailyPlan] TDP
				INNER JOIN TPOFeeHdrPlan FHP
				ON tdp.TPOFeeCode=FHP.TPOFeeCode
				INNER JOIN [MstGenLocation] MGL
				ON MGL.LocationCode=FHP.LocationCode
				WHERE TDP.FeeDate >= @dateFrom
				and TDP.FeeDate <= @dateTo
				and TDP.KPSWeek = @Week
				and TDP.KPSYear = @Year
				and MGL.ParentLocationCode = @Location) X
		WHERE X.FeeDate >= @dateFrom
		and X.FeeDate <= @dateTo
		and X.KPSWeek = @Week
		and X.KPSYear = @Year
		GROUP BY X.TPOFeeCode,X.KPSWeek,X.KPSYear
		-- end : Calculate prod fee

		-- begin : Calculate Mgmt fee
		INSERT INTO @TempTable2
		SELECT Y.BrandGroupCode,Y.LocationCode,SUM(Y.DocCurAmount)
		FROM (
			SELECT TFR.EffectiveDate,TFR.ExpiredDate, TFR.BrandGroupCode,TFR.LocationCode,
			(ManagementFee+JKN+JL1+Jl2+Jl3+Jl4) as DocCurAmount
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
		GROUP BY Y.LocationCode,Y.BrandGroupCode
		ORDER BY Y.LocationCode
		-- end : Calculate Mgmt fee
	END

	SELECT TOP 1 @DocCurAmountProd = SUM(DocCurAmount) FROM @TempTable1;
	SELECT TOP 1 @DocCurAmountMgmt = SUM(DocCurAmount) FROM @TempTable2;
	
	SELECT DISTINCT
	'' as LocationCode,
	'1' as Type,
	'3066' as Company,
	'IDR' as Currency,
	'' as ExchangeRate,
	'SC' as DocumentType,
	'' as TranslationDate,
	('AccrTPOFee'+ SUBSTRING(DATENAME(MONTH,dateadd(month, 0,@dateFrom)),1,3)+'(w.'+CONVERT(VARCHAR(2),@Week)+')'+CONVERT(VARCHAR(4),@Year)) as HeaderText,
	'3066322089' as Reference,
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
	@DocCurAmountProd,
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
	@DocCurAmountMgmt,
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
	b.DocCurAmount,
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
	a.DocCurAmount,
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