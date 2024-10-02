using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SKTISWebsite.Models.GenerateP1TemplateGL
{
    public class GenerateP1TemplateGLViewModel
    {
        public string LocationCode { get; set; }
        public string Type { get; set; }
        public string Company { get; set; }
        public string Currency { get; set; }
        public string ExchangeRate { get; set; }
        public string DocumentType { get; set; }
        public string TranslationDate { get; set; }
        public string HeaderText { get; set; }
        public string Reference { get; set; }
        public string CCTransaction { get; set; }
        public string DocumentDate { get; set; }
        public string PostingDate { get; set; }
        public string AutomaticTex { get; set; }
        public string PostingKey { get; set; }
        public string Account { get; set; }
        public Nullable<double> DocCurAmount { get; set; }
        public string LocalCurAmount { get; set; }
        public string LocalCurrency { get; set; }
        public string TaxCode { get; set; }
        public string PONumber { get; set; }
        public string POItemNumber { get; set; }
        public string Quantity { get; set; }
        public string UOM { get; set; }
        public string Assignment { get; set; }
        public string Text { get; set; }
        public string SpecialGLIndicator { get; set; }
        public string RecoveryIndicator { get; set; }
        public string Customer { get; set; }
        public string BaselineDate { get; set; }
        public string ValueDate { get; set; }
        public string CostCenter { get; set; }
        public string WBS { get; set; }
        public string MaterialNumber { get; set; }
        public string BrandFamily { get; set; }
        public string PaymentTerms { get; set; }
        public string CashDiscount1 { get; set; }
        public string TradingPartner { get; set; }
        public string NewCompany { get; set; }
        public string IntercoAffiliate { get; set; }
        public string ProductionCenter { get; set; }
        public string PMIMarket { get; set; }
        public string ProductCategory { get; set; }
        public string ShipTo { get; set; }
        public string Label { get; set; }
        public string FinalMarket { get; set; }
        public string DocNumberEarMarkedFunds { get; set; }
        public string EarMarkedFunds { get; set; }
        public string TaxBasedAmount { get; set; }
        public string WithholdingTaxBaseAmount { get; set; }
        public string BatchNumber { get; set; }
        public string BusinessPlace { get; set; }
        public string SectionCode { get; set; }
        public string AmountIn2ndLocalCurrency { get; set; }
        public string AmountIn3ndLocalCurrency { get; set; }
        public string WTaxCode { get; set; }
    }
}