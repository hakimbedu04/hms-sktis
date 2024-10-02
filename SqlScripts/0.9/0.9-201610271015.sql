-- Author : Hakim
-- ticket : http://tp.voxteneo.co.id/entity/10764
-- date	  : 2016-10-27
-- description: generate p1 template in view
CREATE VIEW [dbo].[TPOGenerateP1TemplateView]
AS
SELECT 
	'1' as RowOrder,
	LocationCode as LocationCode,
	KPSWeek as KPSWeek,
	KPSYear as KPSYear,
	ParentLocationCode as ParentLocationCode,
	Status as Status,
	'1' as Type,
	'3066' as Company,
	'IDR' as Currency,
	'' as ExchangeRate,
	'SC' as DocumentType,
	'' as TranslationDate,
	(SELECT CONVERT(VARCHAR(4),[LocationCode]) + ' w.' + CONVERT(VARCHAR(4),[KPSWeek]) + CONVERT(VARCHAR(4),[KPSYear])) as HeaderText,
	'SRGLC150319' as Reference,
	'' as CCTransaction,
	 (SELECT CONVERT(VARCHAR(2),DATEPART( DD, GETDATE()))) + '.' + (SELECT CONVERT(VARCHAR(2),DATEPART( MM, GETDATE()))) + '.' + SUBSTRING( convert(varchar(4),(SELECT DATEPART( YY, GETDATE()))),3,2) as DocumentDate,
	 (SELECT CONVERT(VARCHAR(2),DATEPART( DD, GETDATE()))) + '.' + (SELECT CONVERT(VARCHAR(2),DATEPART( MM, GETDATE()))) + '.' + SUBSTRING( convert(varchar(4),(SELECT DATEPART( YY, GETDATE()))),3,2) as PostingDate,
	'' as AutomaticTax,
	'' as PostingKey ,  
	'' as Account ,  
	'' as DocCurAmount ,  
	'' as LocalCurAmount ,
	'' as LocalCurrency ,  
	'' as TaxCode ,  
	'' as PONumber ,  
	'' as POItemNumber ,  
	'' as Quantity ,  
	'' as UOM ,
	'' as Assignment ,  
	'' as Text ,  
	'' as SpecialGLIndicator ,  
	'' as RecoveryIndicator ,  
	'' as Customer ,
	'' as BaselineDate ,  
	'' as ValueDate ,  
	'' as CostCenter ,  
	'' as WBS ,  
	'' as MaterialNumber ,
	'' as BrandFamily ,
	'' as PaymentTerms ,  
	'' as CashDiscount1 ,  
	'' as TradingPartner ,  
	'' as NewCompany ,
	'' as IntercoAffiliate ,
	'' as ProductionCenter,  
	'' as PMIMarket ,  
	'' as ProductCategory ,  
	'' as ShipTo ,  
	'' as Label ,
	'' as FinalMarket ,
	'' as DocNumberEarmarkedFunds ,  
	'' as EarmarkedFunds ,  
	'' as TaxBaseAmount ,
	'' as WithholdingTaxBaseAmount ,
	'' as BatchNumber ,  
	'' as BusinessPlace ,  
	'' as SectionCode ,  
	'' as AmountIn2ndLocalCurrency ,
	'' as AmountIn3rdLocalCurrency ,
	'' as WTaxCode

FROM [dbo].[TPOFeeApprovalView]
--where LocationCode = 'IDAA'
--AND KPSWeek = 40
--and KPSYear = 2016

UNION

SELECT 
	'2' as RowOrder,
	LocationCode as LocationCode,
	KPSWeek as KPSWeek,
	KPSYear as KPSYear,
	ParentLocationCode as ParentLocationCode,
	Status as Status,
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
	'' as AutomaticTax,
	'50' as PostingKey ,  
	'22561000' as Account ,  
	'SKIP' as DocCurAmount ,  
	(BiayaProduksi + JasaManajemen2Percent) as LocalCurAmount ,
	'Jumlah Biaya Produksi + Pajak Jasa Maklon sebesar 2 %' as LocalCurrency , 
	 
	'' as TaxCode ,
	'V0' as PONumber ,  
	'' as POItemNumber ,  
	'' as Quantity ,  
	'' as UOM ,
	'' as Assignment ,  
	(SELECT [SKTBrandCode] +''+ [LocationCode] +''+
	    CONVERT(VARCHAR(4),DATEPART(dd, (SELECT StartDate FROM MstGenWeek WHERE [Week] = (SELECT KPSWeek ) and [YEAR] = (SELECT KPSYear ))))+'.'+

		CONVERT(VARCHAR(4),DATEPART(mm, (SELECT StartDate FROM MstGenWeek WHERE [Week] = (SELECT KPSWeek ) and [YEAR] = (SELECT KPSYear ))))+'.'+

		SUBSTRING(CONVERT(VARCHAR(4), DATEPART(yy, (SELECT StartDate FROM MstGenWeek WHERE [Week] = (SELECT KPSWeek ) and [YEAR] = (SELECT KPSYear )))),3,2)	
		+'-'+
		CONVERT(VARCHAR(4),DATEPART(dd, (SELECT EndDate FROM MstGenWeek WHERE [Week] = (SELECT KPSWeek ) and [YEAR] = (SELECT KPSYear ))))+'.'+

		CONVERT(VARCHAR(4),DATEPART(mm, (SELECT EndDate FROM MstGenWeek WHERE [Week] = (SELECT KPSWeek ) and [YEAR] = (SELECT KPSYear ))))+'.'+

		SUBSTRING(CONVERT(VARCHAR(4), DATEPART(yy, (SELECT EndDate FROM MstGenWeek WHERE [Week] = (SELECT KPSWeek ) and [YEAR] = (SELECT KPSYear )))),3,2)
				
		+'(w.'+CONVERT(VARCHAR(4),KPSWeek)+')')
		  as Text , 
	'' as SpecialGLIndicator ,  
	'' as RecoveryIndicator ,  
	'' as Customer ,
	'' as BaselineDate ,  
	'' as ValueDate ,  
	'' as CostCenter ,  
	'' as WBS ,  
	'' as MaterialNumber ,
	'' as BrandFamily ,
	'' as PaymentTerms ,  
	'' as CashDiscount1 ,  
	'' as TradingPartner ,  
	'' as NewCompany ,
	'' as IntercoAffiliate ,
	'' as ProductionCenter,  
	'' as PMIMarket ,  
	'' as ProductCategory ,  
	'' as ShipTo ,  
	'' as Label ,
	'' as FinalMarket ,
	'' as DocNumberEarmarkedFunds ,  
	'' as EarmarkedFunds ,  
	'' as TaxBaseAmount ,
	'' as WithholdingTaxBaseAmount ,
	'' as BatchNumber ,  
	'' as BusinessPlace ,  
	'' as SectionCode ,  
	'' as AmountIn2ndLocalCurrency ,
	'' as AmountIn3rdLocalCurrency ,
	'' as WTaxCode
FROM [dbo].[TPOFeeApprovalView]
--where LocationCode = 'IDAA'
--AND KPSWeek = 40
--and KPSYear = 2016

UNION

SELECT 
	'3' as RowOrder,
	LocationCode as LocationCode,
	KPSWeek as KPSWeek,
	KPSYear as KPSYear,
	ParentLocationCode as ParentLocationCode,
	Status as Status,
	'2' as Type,
	'' as Company,
	'' as Currency,
	'' as ExchangeRate,
	'' as DocumentType,
	'' as TranslationDate,
	'' as HeaderText,
	'' as Reference,
	'' as CCTransaction,
	''  as DocumentDate,
	'' as PostingDate,
	'' as AutomaticTax,
	'40' as PostingKey ,  
	'82182000' as Account ,  
	'SKIP' as DocCurAmount ,
	CONVERT(VARCHAR(50),BiayaProduksi) as LocalCurAmount ,
	'Biaya Produksi' as LocalCurrency , 
	'' as TaxCode ,  
	'' as PONumber ,  
	'' as POItemNumber ,  
	'' as Quantity ,  
	'' as UOM ,
	'' as Assignment ,  
	'' as Text ,  
	'' as SpecialGLIndicator ,
	(SELECT [SKTBrandCode] +''+ [LocationCode] +''+
	     CONVERT(VARCHAR(4),DATEPART(dd, (SELECT StartDate FROM MstGenWeek WHERE [Week] = (SELECT KPSWeek ) and [YEAR] = (SELECT KPSYear ))))+'.'+

		CONVERT(VARCHAR(4),DATEPART(mm, (SELECT StartDate FROM MstGenWeek WHERE [Week] = (SELECT KPSWeek ) and [YEAR] = (SELECT KPSYear ))))+'.'+

		SUBSTRING(CONVERT(VARCHAR(4), DATEPART(yy, (SELECT StartDate FROM MstGenWeek WHERE [Week] = (SELECT KPSWeek ) and [YEAR] = (SELECT KPSYear )))),3,2)
				
		 +'-'+
		 CONVERT(VARCHAR(4),DATEPART(dd, (SELECT EndDate FROM MstGenWeek WHERE [Week] = (SELECT KPSWeek ) and [YEAR] = (SELECT KPSYear ))))+'.'+

		CONVERT(VARCHAR(4),DATEPART(mm, (SELECT EndDate FROM MstGenWeek WHERE [Week] = (SELECT KPSWeek ) and [YEAR] = (SELECT KPSYear ))))+'.'+

		SUBSTRING(CONVERT(VARCHAR(4), DATEPART(yy, (SELECT EndDate FROM MstGenWeek WHERE [Week] = (SELECT KPSWeek ) and [YEAR] = (SELECT KPSYear )))),3,2)
				
		 +'(w.'+CONVERT(VARCHAR(4),KPSWeek)+')'+ ' Prod Fee')
		  as RecoveryIndicator ,
	'' as Customer ,
	'' as BaselineDate ,
	'' as ValueDate ,
	'' as CostCenter ,
	'' as WBS ,
	'' as MaterialNumber ,
	'3066150VR1' as BrandFamily ,
	'' as PaymentTerms ,  
	'' as CashDiscount1 ,  
	'' as TradingPartner ,  
	'' as NewCompany ,
	'' as IntercoAffiliate ,
	'' as ProductionCenter,  
	'' as PMIMarket ,  
	'' as ProductCategory ,  
	'' as ShipTo ,  
	'' as Label ,
	'' as FinalMarket ,
	'' as DocNumberEarmarkedFunds ,  
	'' as EarmarkedFunds ,  
	'' as TaxBaseAmount ,
	'' as WithholdingTaxBaseAmount ,
	'' as BatchNumber ,  
	'' as BusinessPlace ,  
	'' as SectionCode ,  
	'' as AmountIn2ndLocalCurrency ,
	'' as AmountIn3rdLocalCurrency ,
	'' as WTaxCode
FROM [dbo].[TPOFeeApprovalView]
--where LocationCode = 'IDAA'
--AND KPSWeek = 40
--and KPSYear = 2016

UNION

SELECT 
	'4' as RowOrder,
	LocationCode as LocationCode,
	KPSWeek as KPSWeek,
	KPSYear as KPSYear,
	ParentLocationCode as ParentLocationCode,
	Status as Status,
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
	'' as AutomaticTax,
	'40' as PostingKey ,  
	'82182000' as Account ,  
	'SKIP' as DocCurAmount ,  
	CONVERT(VARCHAR(50),JasaManajemen2Percent) as LocalCurAmount ,
	'Pajak Jasa Maklon Sebesar 2 %' as LocalCurrency , 
	'' as TaxCode ,  
	'' as PONumber ,  
	'' as POItemNumber ,  
	'' as Quantity ,  
	'' as UOM ,
	'' as Assignment ,  
	'' as Text ,  
	'' as SpecialGLIndicator ,  
	(SELECT [SKTBrandCode] +''+ [LocationCode] +''+
	    CONVERT(VARCHAR(4),DATEPART(dd, (SELECT StartDate FROM MstGenWeek WHERE [Week] = (SELECT KPSWeek ) and [YEAR] = (SELECT KPSYear ))))+'.'+

		CONVERT(VARCHAR(4),DATEPART(mm, (SELECT StartDate FROM MstGenWeek WHERE [Week] = (SELECT KPSWeek ) and [YEAR] = (SELECT KPSYear ))))+'.'+

		SUBSTRING(CONVERT(VARCHAR(4), DATEPART(yy, (SELECT StartDate FROM MstGenWeek WHERE [Week] = (SELECT KPSWeek ) and [YEAR] = (SELECT KPSYear )))),3,2)
		+'-'+
		CONVERT(VARCHAR(4),DATEPART(dd, (SELECT EndDate FROM MstGenWeek WHERE [Week] = (SELECT KPSWeek ) and [YEAR] = (SELECT KPSYear ))))+'.'+

		CONVERT(VARCHAR(4),DATEPART(mm, (SELECT EndDate FROM MstGenWeek WHERE [Week] = (SELECT KPSWeek ) and [YEAR] = (SELECT KPSYear ))))+'.'+

		SUBSTRING(CONVERT(VARCHAR(4), DATEPART(yy, (SELECT EndDate FROM MstGenWeek WHERE [Week] = (SELECT KPSWeek ) and [YEAR] = (SELECT KPSYear )))),3,2)
				
		+'(w.'+CONVERT(VARCHAR(4),KPSWeek)+')'+ ' Mgmt Fee')
		 as RecoveryIndicator ,  
	'' as Customer ,
	'' as BaselineDate ,  
	'' as ValueDate ,  
	'' as CostCenter ,  
	'' as WBS ,  
	'' as MaterialNumber ,
	'3066150VR1' as BrandFamily ,
	'' as PaymentTerms ,  
	'' as CashDiscount1 ,  
	'' as TradingPartner ,  
	'' as NewCompany ,
	'' as IntercoAffiliate ,
	'' as ProductionCenter,  
	'' as PMIMarket ,  
	'' as ProductCategory ,  
	'' as ShipTo ,  
	'' as Label ,
	'' as FinalMarket ,
	'' as DocNumberEarmarkedFunds ,  
	'' as EarmarkedFunds ,  
	'' as TaxBaseAmount ,
	'' as WithholdingTaxBaseAmount ,
	'' as BatchNumber ,  
	'' as BusinessPlace ,  
	'' as SectionCode ,  
	'' as AmountIn2ndLocalCurrency ,
	'' as AmountIn3rdLocalCurrency ,
	'' as WTaxCode
FROM [dbo].[TPOFeeApprovalView]
--where LocationCode = 'IDAA'
--AND KPSWeek = 40
--and KPSYear = 2016


GO