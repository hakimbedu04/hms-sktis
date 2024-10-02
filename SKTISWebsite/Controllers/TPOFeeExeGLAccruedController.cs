using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.BusinessObjects.Inputs.TPOFee;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using SKTISWebsite.Code;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.TPOFeeExeGLAccrued;
using SKTISWebsite.Models.GenerateP1TemplateGL;
using System.IO;
using SpreadsheetLight;


namespace SKTISWebsite.Controllers
{
    public class TPOFeeExeGLAccruedController : BaseController
    {
        private readonly IApplicationService    _appService;
        private readonly IMasterDataBLL         _masterDataBll;
        private readonly ITPOFeeExeGLAccruedBLL _tpoFeeExeGlAccruedBll;

        public TPOFeeExeGLAccruedController
        (
            IApplicationService     appService,
            IMasterDataBLL          masterDataBll,
            ITPOFeeExeGLAccruedBLL  tpoFeeExeGlAccruedBll
        )
        {
            _appService             = appService;
            _masterDataBll          = masterDataBll;
            _tpoFeeExeGlAccruedBll  = tpoFeeExeGlAccruedBll;
            SetPage("TPOFee/Execution/TPOFeeGL");
        }

        // GET: TPOFeeExeGLAccrued
        public ActionResult Index()
        {
            int yearOld = DateTime.Now.Year;
            if (TempData["year1"] != null)
            {
                yearOld = (int)TempData["year1"];

            }
            int? weekOld = _masterDataBll.GetGeneralWeekWeekByDate(DateTime.Now);
            if (TempData["week1"] != null)
            {
                weekOld = (int)TempData["week1"];

            }
            string regionalOld = "TPO";
            if (TempData["regional1"] != null)
            {
                regionalOld = (string)TempData["regional1"];

            }

            var initModel = new InitTPOFeeExeGLAccruedViewModel()
            {
                YearSelectList      = _appService.GetGenWeekYears(),
                DefaultYear         = yearOld,
                DefaultWeek         = weekOld,
                DefaultRegional     = regionalOld,
                Regional            = _appService.GetTPOLocationCodeSelectListCompat()
            };
            return View("Index", initModel);
        }

        [HttpPost]
        public JsonResult GetTPOFeeExeGLAccruedViewList(GetTPOFeeExeGLAccruedInput criteria)
        {
            TempData["Year"] = criteria.KpsYear;
            TempData["week"] = criteria.KpsWeek;
            TempData["regional"] = criteria.Regional;

            var result = _tpoFeeExeGlAccruedBll.GetTPOFeeExeGLAccruedViewList(criteria);
            var viewModel = Mapper.Map<List<TPOFeeExeGLAccruedViewListModel>>(result);
            
            var pageResult = new PageResult<TPOFeeExeGLAccruedViewListModel>(viewModel, criteria);
            return Json(pageResult);
        }

        [HttpGet]
        public JsonResult GetWeeks(int year)
        {
            var kpsWeek = _masterDataBll.GetWeekByYear(year);
            return Json(kpsWeek, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetClosingDates(int year, int week)
        {
            var listClosingDates = _masterDataBll.GetClosingDatePayrollByYear(year, week);
            return Json(listClosingDates, JsonRequestBehavior.AllowGet);
        }
         
        //[HttpPost]
        //public JsonResult GenerateP1(List<TPOFeeExeGLAccruedViewListModel> bulkData)
        //{
        //    if (bulkData.Any())
        //    {
        //        if (bulkData.Count(p => p.Checkbox) != 0)
        //        {
        //            var regional = bulkData.Select(c => c.Regional).FirstOrDefault();
        //            var kpsWeek = bulkData.Select(c => c.KpsWeek).FirstOrDefault();
        //            var kpsYear = bulkData.Select(c => c.KpsYear).FirstOrDefault();

        //            string fileName = "Upload GL W" + kpsWeek + " " + regional + ".txt";

        //            string path = Server.MapPath("~/Content/P1Template/" + fileName);

        //            if (System.IO.File.Exists(path))
        //            {
        //                System.IO.File.Delete(path);
        //            }

        //            var counter = bulkData.Count(p => p.Checkbox);



        //            if (!System.IO.File.Exists(path))
        //            {
        //                for (int i = 0; i < bulkData.Count; i++)
        //                {
        //                    if (bulkData[i].Checkbox == true)
        //                    {
        //                        var mstGenWeek = _masterDataBll.GetWeekByYearAndWeek(kpsYear, kpsWeek);
        //                        var text = bulkData[i].Brand.Substring(0, 3) + "-" + bulkData[i].Brand.Substring(3, 2) +
        //                                   bulkData[i].Location
        //                                   + mstGenWeek.StartDate.Value.Day + "." +
        //                                   mstGenWeek.StartDate.Value.ToString("MM") + "." +
        //                                   mstGenWeek.StartDate.Value.Year.ToString().Substring(2, 2) + "-"
        //                                   + mstGenWeek.EndDate.Value.Day + "." +
        //                                   mstGenWeek.EndDate.Value.ToString("MM") + "." +
        //                                   mstGenWeek.EndDate.Value.Year.ToString().Substring(2, 2) + "(w." + kpsWeek + ")";

        //                        using (StreamWriter sw = System.IO.File.AppendText(path))
        //                        {
        //                            string[] title =
        //                        {
        //                            "Type", "Company", "Currency", "Exchange rate", "Document type", "Translation date"
        //                            , "Header text", "Reference", "CC transaction", "Document date", "Posting date",
        //                            "Automatic tax", "Posting key", "Account", "Doc cur amount", "Local cur amount",
        //                            "Local currency", "Tax code", "PO Number", "PO Item Number", "Quantity", "UOM",
        //                            "Assignment", "Text", "Special GL Indicator", "Recovery Indicator", "Customer",
        //                            "Baseline date", "Value date", "Cost center", "WBS", "Material number",
        //                            "Brand family",
        //                            "Payment terms", "Cash discount 1", "Trading partner", "New company",
        //                            "Interco affiliate",
        //                            "Production center", "PMI Market", "Product category", "Ship-to", "Label",
        //                            "Final market",
        //                            "DocNumber-Earmarked Funds", "Earmarked Funds", "Tax Base Amount",
        //                            "Withholding Tax Base Amount",
        //                            "Batch number", "Business Place", "Section Code", "Amount in 2nd local currency",
        //                            "Amount in 3rd local currency",
        //                            "W. Tax Code"
        //                        };

        //                            string[] content1 =
        //                        {
        //                            "1", "3066", "IDR", "", "SC", ""
        //                            ,
        //                            bulkData[i].Location + " w." + kpsWeek + " " +
        //                            kpsYear,
        //                            "SRGLC150319", "",
        //                            DateTime.Now.Day + "." + DateTime.Now.Month + "." +
        //                            DateTime.Now.Year.ToString().Substring(2, 2),
        //                            DateTime.Now.Day + "." + DateTime.Now.Month + "." +
        //                            DateTime.Now.Year.ToString().Substring(2, 2),
        //                            "", "", "", "", "",
        //                            "", "", "", "", "", "",
        //                            "", "", "", "", "",
        //                            "", "", "", "", "", "",
        //                            "", "", "", "", "",
        //                            "", "", "", "", "", "",
        //                            "", "", "", "",
        //                            "", "", "", "", "",
        //                            ""
        //                        };

        //                            string[] content2 =
        //                        {
        //                            "2", "", "", "", "", ""
        //                            , "",
        //                            "", "",
        //                            "",
        //                            "",
        //                            "", "50", "22561000", "SKIP",
        //                            (bulkData[i].BiayaProduksi + bulkData[i].JasaManagemen).ToString(),
        //                            "Jumlah Biaya Produksi + Pajak Jasa Maklon sebesar 2 %", "", "V0", "", "", "",
        //                            "", text, "", "", "",
        //                            "", "", "", "", "", "",
        //                            "", "", "", "", "",
        //                            "", "", "", "", "", "",
        //                            "", "", "", "",
        //                            "", "", "", "", "",
        //                            ""
        //                        };

        //                            string[] content3 =
        //                        {
        //                            "2", "", "", "", "", ""
        //                            , "",
        //                            "", "",
        //                            "",
        //                            "",
        //                            "", "40", "82182000", "SKIP",
        //                            bulkData[i].BiayaProduksi.ToString(),
        //                            "Biaya Produksi", "", "", "", "", "",
        //                            "", "", "", text + " Prod Fee", "", "",
        //                            "", "", "", "", "3066150VR1", "",
        //                            "", "", "", "", "",
        //                            "", "", "", "", "", "",
        //                            "", "", "", "",
        //                            "", "", "", "", "",
        //                            ""
        //                        };

        //                            string[] content4 =
        //                        {
        //                            "2", "", "", "", "", ""
        //                            , "",
        //                            "", "",
        //                            "",
        //                            "",
        //                            "", "40", "82182010", "SKIP",
        //                            bulkData[i].JasaManagemen.ToString(),
        //                            "Pajak Jasa Maklon sebesar 2 %", "", "", "", "", "",
        //                            "", "", "", text + "  Mgmt Fee", "", "",
        //                            "", "", "", "", "3066150VR1", "",
        //                            "", "", "", "", "",
        //                            "", "", "", "", "", "",
        //                            "", "", "", "",
        //                            "", "", "", "", "",
        //                            ""
        //                        };

        //                            string write = string.Empty;

        //                            foreach (var data in title)
        //                            {
        //                                write += data + "\t";
        //                            }

        //                            sw.WriteLine(write);

        //                            write = string.Empty;

        //                            foreach (var data in content1)
        //                            {
        //                                write += data + "\t";
        //                            }

        //                            sw.WriteLine(write);

        //                            write = string.Empty;

        //                            foreach (var data in content2)
        //                            {
        //                                write += data + "\t";
        //                            }

        //                            sw.WriteLine(write);

        //                            write = string.Empty;

        //                            foreach (var data in content3)
        //                            {
        //                                write += data + "\t";
        //                            }

        //                            sw.WriteLine(write);

        //                            write = string.Empty;

        //                            foreach (var data in content4)
        //                            {
        //                                write += data + "\t";
        //                            }

        //                            sw.WriteLine(write + System.Environment.NewLine);

        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            bulkData = null;
        //        }
        //    }
        //    else
        //    {
        //        bulkData = null;
        //    }
        //    return Json(bulkData);
        //}

        //[HttpPost]
        //public ActionResult GenerateP1T(P1Template<TPOFeeExeGLAccruedViewListModel> bulkData)
        //{
        //    var sb = new StringBuilder();

        //    var counter = bulkData.Data.Count(p => p.Checkbox);
        //    if (bulkData.Data.Any() && counter > 0)
        //    {
        //        // MemoryStream ms = new MemoryStream();
        //        var regional = bulkData.Data.Select(c => c.Regional).FirstOrDefault();
        //        var kpsWeek = bulkData.Data.Select(c => c.KpsWeek).FirstOrDefault();
        //        var kpsYear = bulkData.Data.Select(c => c.KpsYear).FirstOrDefault();

        //        var fileName = "Upload GL W" + kpsWeek + " " + regional + ".txt";
                
        //        for (int i = 0; i < bulkData.Data.Count; i++)
        //        {
        //            if (bulkData.Data[i].Checkbox == true)
        //            {
        //                var mstGenWeek = _masterDataBll.GetWeekByYearAndWeek(kpsYear, kpsWeek);
        //                var text = bulkData.Data[i].Brand.Substring(0, 3) + "-" +
        //                           bulkData.Data[i].Brand.Substring(3, 2) +
        //                           bulkData.Data[i].Location
        //                           + mstGenWeek.StartDate.Value.Day + "." +
        //                           mstGenWeek.StartDate.Value.ToString("MM") + "." +
        //                           mstGenWeek.StartDate.Value.Year.ToString().Substring(2, 2) + "-"
        //                           + mstGenWeek.EndDate.Value.Day + "." +
        //                           mstGenWeek.EndDate.Value.ToString("MM") + "." +
        //                           mstGenWeek.EndDate.Value.Year.ToString().Substring(2, 2) + "(w." + kpsWeek + ")";

        //                //using (StreamWriter sw = System.IO.File.AppendText(path))
        //                //{
        //                string[] title =
        //                    {
        //                        "Type", "Company", "Currency", "Exchange rate", "Document type", "Translation date"
        //                        , "Header text", "Reference", "CC transaction", "Document date", "Posting date",
        //                        "Automatic tax", "Posting key", "Account", "Doc cur amount", "Local cur amount",
        //                        "Local currency", "Tax code", "PO Number", "PO Item Number", "Quantity", "UOM",
        //                        "Assignment", "Text", "Special GL Indicator", "Recovery Indicator", "Customer",
        //                        "Baseline date", "Value date", "Cost center", "WBS", "Material number",
        //                        "Brand family",
        //                        "Payment terms", "Cash discount 1", "Trading partner", "New company",
        //                        "Interco affiliate",
        //                        "Production center", "PMI Market", "Product category", "Ship-to", "Label",
        //                        "Final market",
        //                        "DocNumber-Earmarked Funds", "Earmarked Funds", "Tax Base Amount",
        //                        "Withholding Tax Base Amount",
        //                        "Batch number", "Business Place", "Section Code", "Amount in 2nd local currency",
        //                        "Amount in 3rd local currency",
        //                        "W. Tax Code"
        //                    };

        //                string[] content1 =
        //                    {
        //                        "1", "3066", "IDR", "", "SC", ""
        //                        ,
        //                        bulkData.Data[i].Location + " w." + kpsWeek + " " +
        //                        kpsYear,
        //                        "SRGLC150319", "",
        //                        DateTime.Now.Day + "." + DateTime.Now.Month + "." +
        //                        DateTime.Now.Year.ToString().Substring(2, 2),
        //                        DateTime.Now.Day + "." + DateTime.Now.Month + "." +
        //                        DateTime.Now.Year.ToString().Substring(2, 2),
        //                        "", "", "", "", "",
        //                        "", "", "", "", "", "",
        //                        "", "", "", "", "",
        //                        "", "", "", "", "", "",
        //                        "", "", "", "", "",
        //                        "", "", "", "", "", "",
        //                        "", "", "", "",
        //                        "", "", "", "", "",
        //                        ""
        //                    };

        //                string[] content2 =
        //                    {
        //                        "2", "", "", "", "", ""
        //                        , "",
        //                        "", "",
        //                        "",
        //                        "",
        //                        "", "50", "22561000", "SKIP",
        //                        (bulkData.Data[i].BiayaProduksi + bulkData.Data[i].JasaManagemen).ToString(),
        //                        "Jumlah Biaya Produksi + Pajak Jasa Maklon sebesar 2 %", "", "V0", "", "", "",
        //                        "", text, "", "", "",
        //                        "", "", "", "", "", "",
        //                        "", "", "", "", "",
        //                        "", "", "", "", "", "",
        //                        "", "", "", "",
        //                        "", "", "", "", "",
        //                        ""
        //                    };

        //                string[] content3 =
        //                    {
        //                        "2", "", "", "", "", ""
        //                        , "",
        //                        "", "",
        //                        "",
        //                        "",
        //                        "", "40", "82182000", "SKIP",
        //                        bulkData.Data[i].BiayaProduksi.ToString(),
        //                        "Biaya Produksi", "", "", "", "", "",
        //                        "", "", "", text + " Prod Fee", "", "",
        //                        "", "", "", "", "3066150VR1", "",
        //                        "", "", "", "", "",
        //                        "", "", "", "", "", "",
        //                        "", "", "", "",
        //                        "", "", "", "", "",
        //                        ""
        //                    };

        //                string[] content4 =
        //                    {
        //                        "2", "", "", "", "", ""
        //                        , "",
        //                        "", "",
        //                        "",
        //                        "",
        //                        "", "40", "82182010", "SKIP",
        //                        bulkData.Data[i].JasaManagemen.ToString(),
        //                        "Pajak Jasa Maklon sebesar 2 %", "", "", "", "", "",
        //                        "", "", "", text + "  Mgmt Fee", "", "",
        //                        "", "", "", "", "3066150VR1", "",
        //                        "", "", "", "", "",
        //                        "", "", "", "", "", "",
        //                        "", "", "", "",
        //                        "", "", "", "", "",
        //                        ""
        //                    };

        //                string write = string.Empty;

        //                foreach (var data in title)
        //                {
        //                    write += data + "\t";
        //                }

        //                //sw.WriteLine(write);
        //                sb.Append(write + Environment.NewLine);
        //                write = string.Empty;

        //                foreach (var data in content1)
        //                {
        //                    write += data + "\t";
        //                }

        //                //sw.WriteLine(write);
        //                sb.Append(write + Environment.NewLine);
                        

        //                sb.Append(write + Environment.NewLine + Environment.NewLine);
        //            }
        //        }

        //        return File(Encoding.ASCII.GetBytes(sb.ToString()), "text/plain", fileName);
        //    }

        //    // Please change this.
        //    return Content("No data");
        //}
        //hakim
        [HttpPost]
        public ActionResult GenerateP1(GetTPOFeeExeGLAccruedInput criteria)
        {
            string fileName = "Upload GL W" + criteria.KpsWeek + " " + criteria.Location + ".txt";

            var generateData = new GenerateP1TemplateAP();
            var sb = generateData.getP1GL(criteria.ClosingDate, criteria.KpsWeek, criteria.KpsYear, criteria.Location);

            return File(Encoding.ASCII.GetBytes(sb.ToString()), "text/plain", fileName);
            //var sb = new StringBuilder();

            //var fileName = "Upload GL W" + criteria.KpsWeek + " " + criteria.Location + ".txt";

            //var dto = _tpoFeeExeGlAccruedBll.GetP1TemplateGL(criteria);
            //var viewModel = Mapper.Map<List<GenerateP1TemplateGLViewModel>>(dto);
            //string[] title =
            //            {
            //                "Type", "Company", "Currency", "Exchange rate", "Document type", "Translation date",
            //                "Header text", "Reference", "CC transaction", "Document date", "Posting date",
            //                "Automatic tax", "Posting key", "Account", "Doc cur amount", "Local cur amount",
            //                "Local currency", "Tax code", "PO Number", "PO Item Number", "Quantity", "UOM",
            //                "Assignment", "Text", "Special GL Indicator", "Recovery Indicator", "Customer",
            //                "Baseline date", "Value date", "Cost center", "WBS", "Material number","Brand family",
            //                "Payment terms", "Cash discount 1", "Trading partner", "New company","Interco affiliate",
            //                "Production center", "PMI Market", "Product category", "Ship-to", "Label","Final market",
            //                "DocNumber-Earmarked Funds", "Earmarked Funds", "Tax Base Amount","Withholding Tax Base Amount",
            //                "Batch number", "Business Place", "Section Code", "Amount in 2nd local currency","Amount in 3rd local currency",
            //                "W. Tax Code"
            //            };
            //string write = string.Empty;

            //foreach (var data in title)
            //{
            //    write += data + "\t";
            //}
            //sb.Append(write + Environment.NewLine);

            //for (int i = 0; i < viewModel.Count; i++)
            //{
            //    string doccuramount = "";
            //    if (viewModel[i].DocCurAmount.GetValueOrDefault() == null || viewModel[i].DocCurAmount.GetValueOrDefault() == 0)
            //    {
            //        doccuramount = doccuramount;
            //    }
            //    else
            //    {
            //        doccuramount = Convert.ToString(viewModel[i].DocCurAmount.Value);
            //    }
            //    string[] content1 =
            //            {
            //                viewModel[i].Type, viewModel[i].Company, viewModel[i].Currency, viewModel[i].ExchangeRate, viewModel[i].DocumentType, viewModel[i].TranslationDate,
            //                viewModel[i].HeaderText,viewModel[i].Reference,viewModel[i].CCTransaction,viewModel[i].DocumentDate,viewModel[i].PostingDate,
            //                viewModel[i].AutomaticTex,viewModel[i].PostingKey,viewModel[i].Account,doccuramount,viewModel[i].LocalCurAmount,
            //                viewModel[i].LocalCurrency,viewModel[i].TaxCode,viewModel[i].PONumber,viewModel[i].POItemNumber,viewModel[i].Quantity,viewModel[i].UOM,
            //                viewModel[i].Assignment,viewModel[i].Text,viewModel[i].SpecialGLIndicator,viewModel[i].RecoveryIndicator,viewModel[i].Customer,
            //                viewModel[i].BaselineDate,viewModel[i].ValueDate,viewModel[i].CostCenter,viewModel[i].WBS,viewModel[i].MaterialNumber,viewModel[i].BrandFamily,
            //                viewModel[i].PaymentTerms,  viewModel[i].CashDiscount1 ,  viewModel[i].TradingPartner ,  viewModel[i].NewCompany ,viewModel[i].IntercoAffiliate ,
            //                viewModel[i].ProductionCenter, viewModel[i].PMIMarket ,  viewModel[i].ProductCategory ,  viewModel[i].ShipTo ,  viewModel[i].Label ,viewModel[i].FinalMarket ,
            //                viewModel[i].DocNumberEarMarkedFunds ,  viewModel[i].EarMarkedFunds ,  viewModel[i].TaxBasedAmount ,viewModel[i].WithholdingTaxBaseAmount ,
            //                viewModel[i].BatchNumber ,  viewModel[i].BusinessPlace ,  viewModel[i].SectionCode ,  viewModel[i].AmountIn2ndLocalCurrency ,viewModel[i].AmountIn3ndLocalCurrency ,viewModel[i].WTaxCode
            //            };

            //    write = string.Empty;

            //    foreach (var data in content1)
            //    {
            //        write += data + "\t";
            //    }

            //    sb.Append(write + Environment.NewLine);
            //}
            //return File(Encoding.ASCII.GetBytes(sb.ToString()), "text/plain", fileName);
        }

        //[HttpPost]
        //public ActionResult GenerateP1T(P1Template<TPOFeeExeGLAccruedViewListModel> bulkData)
        //{
        //    var sb = new StringBuilder();

        //    var counter = bulkData.Data.Count(p => p.Checkbox);
        //    if (bulkData.Data.Any() && counter > 0)
        //    {
        //       // MemoryStream ms = new MemoryStream();
        //        var regional = bulkData.Data.Select(c => c.Regional).FirstOrDefault();
        //        var kpsWeek = bulkData.Data.Select(c => c.KpsWeek).FirstOrDefault();
        //        var kpsYear = bulkData.Data.Select(c => c.KpsYear).FirstOrDefault();

        //        var fileName = "Upload GL W" + kpsWeek + " " + regional + ".txt";
        //        //string path = Server.MapPath("~/Content/P1Template/" + fileName);

        //        //if (System.IO.File.Exists(path))
        //        //{
        //        //    System.IO.File.Delete(path);
        //        //}
        //        //============================
            
        //        //if (!System.IO.File.Exists(path))
        //        //{
        //            for (int i = 0; i < bulkData.Data.Count; i++)
        //            {
        //                if (bulkData.Data[i].Checkbox == true)
        //                {
        //                    var mstGenWeek = _masterDataBll.GetWeekByYearAndWeek(kpsYear, kpsWeek);
        //                    var text = bulkData.Data[i].Brand.Substring(0, 3) + "-" +
        //                               bulkData.Data[i].Brand.Substring(3, 2) +
        //                               bulkData.Data[i].Location
        //                               + mstGenWeek.StartDate.Value.Day + "." +
        //                               mstGenWeek.StartDate.Value.ToString("MM") + "." +
        //                               mstGenWeek.StartDate.Value.Year.ToString().Substring(2, 2) + "-"
        //                               + mstGenWeek.EndDate.Value.Day + "." +
        //                               mstGenWeek.EndDate.Value.ToString("MM") + "." +
        //                               mstGenWeek.EndDate.Value.Year.ToString().Substring(2, 2) + "(w." + kpsWeek + ")";

        //                    //using (StreamWriter sw = System.IO.File.AppendText(path))
        //                    //{
        //                    string[] title =
        //                    {
        //                        "Type", "Company", "Currency", "Exchange rate", "Document type", "Translation date"
        //                        , "Header text", "Reference", "CC transaction", "Document date", "Posting date",
        //                        "Automatic tax", "Posting key", "Account", "Doc cur amount", "Local cur amount",
        //                        "Local currency", "Tax code", "PO Number", "PO Item Number", "Quantity", "UOM",
        //                        "Assignment", "Text", "Special GL Indicator", "Recovery Indicator", "Customer",
        //                        "Baseline date", "Value date", "Cost center", "WBS", "Material number",
        //                        "Brand family",
        //                        "Payment terms", "Cash discount 1", "Trading partner", "New company",
        //                        "Interco affiliate",
        //                        "Production center", "PMI Market", "Product category", "Ship-to", "Label",
        //                        "Final market",
        //                        "DocNumber-Earmarked Funds", "Earmarked Funds", "Tax Base Amount",
        //                        "Withholding Tax Base Amount",
        //                        "Batch number", "Business Place", "Section Code", "Amount in 2nd local currency",
        //                        "Amount in 3rd local currency",
        //                        "W. Tax Code"
        //                    };

        //                    string[] content1 =
        //                    {
        //                        "1", "3066", "IDR", "", "SC", ""
        //                        ,
        //                        bulkData.Data[i].Location + " w." + kpsWeek + " " +
        //                        kpsYear,
        //                        "SRGLC150319", "",
        //                        DateTime.Now.Day + "." + DateTime.Now.Month + "." +
        //                        DateTime.Now.Year.ToString().Substring(2, 2),
        //                        DateTime.Now.Day + "." + DateTime.Now.Month + "." +
        //                        DateTime.Now.Year.ToString().Substring(2, 2),
        //                        "", "", "", "", "",
        //                        "", "", "", "", "", "",
        //                        "", "", "", "", "",
        //                        "", "", "", "", "", "",
        //                        "", "", "", "", "",
        //                        "", "", "", "", "", "",
        //                        "", "", "", "",
        //                        "", "", "", "", "",
        //                        ""
        //                    };

        //                    string[] content2 =
        //                    {
        //                        "2", "", "", "", "", ""
        //                        , "",
        //                        "", "",
        //                        "",
        //                        "",
        //                        "", "50", "22561000", "SKIP",
        //                        (bulkData.Data[i].BiayaProduksi + bulkData.Data[i].JasaManagemen).ToString(),
        //                        "Jumlah Biaya Produksi + Pajak Jasa Maklon sebesar 2 %", "", "V0", "", "", "",
        //                        "", text, "", "", "",
        //                        "", "", "", "", "", "",
        //                        "", "", "", "", "",
        //                        "", "", "", "", "", "",
        //                        "", "", "", "",
        //                        "", "", "", "", "",
        //                        ""
        //                    };

        //                    string[] content3 =
        //                    {
        //                        "2", "", "", "", "", ""
        //                        , "",
        //                        "", "",
        //                        "",
        //                        "",
        //                        "", "40", "82182000", "SKIP",
        //                        bulkData.Data[i].BiayaProduksi.ToString(),
        //                        "Biaya Produksi", "", "", "", "", "",
        //                        "", "", "", text + " Prod Fee", "", "",
        //                        "", "", "", "", "3066150VR1", "",
        //                        "", "", "", "", "",
        //                        "", "", "", "", "", "",
        //                        "", "", "", "",
        //                        "", "", "", "", "",
        //                        ""
        //                    };

        //                    string[] content4 =
        //                    {
        //                        "2", "", "", "", "", ""
        //                        , "",
        //                        "", "",
        //                        "",
        //                        "",
        //                        "", "40", "82182010", "SKIP",
        //                        bulkData.Data[i].JasaManagemen.ToString(),
        //                        "Pajak Jasa Maklon sebesar 2 %", "", "", "", "", "",
        //                        "", "", "", text + "  Mgmt Fee", "", "",
        //                        "", "", "", "", "3066150VR1", "",
        //                        "", "", "", "", "",
        //                        "", "", "", "", "", "",
        //                        "", "", "", "",
        //                        "", "", "", "", "",
        //                        ""
        //                    };

        //                    string write = string.Empty;

        //                    foreach (var data in title)
        //                    {
        //                        write += data + "\t";
        //                    }

        //                    //sw.WriteLine(write);
        //                    sb.Append(write + Environment.NewLine);
        //                    write = string.Empty;

        //                    foreach (var data in content1)
        //                    {
        //                        write += data + "\t";
        //                    }

        //                    //sw.WriteLine(write);
        //                    sb.Append(write + Environment.NewLine);
        //                    write = string.Empty;

        //                    foreach (var data in content2)
        //                    {
        //                        write += data + "\t";
        //                    }

        //                    //sw.WriteLine(write);
        //                    sb.Append(write + Environment.NewLine);
        //                    write = string.Empty;

        //                    foreach (var data in content3)
        //                    {
        //                        write += data + "\t";
        //                    }

        //                    //sw.WriteLine(write);
        //                    sb.Append(write + Environment.NewLine);
        //                    write = string.Empty;

        //                    foreach (var data in content4)
        //                    {
        //                        write += data + "\t";
        //                    }

        //                    sb.Append(write + Environment.NewLine + Environment.NewLine);
        //                    //sw.WriteLine(write + System.Environment.NewLine);
        //                    //sw.Flush();
        //                    //}
        //                }
        //            }

        //        //}
        //        return File(Encoding.ASCII.GetBytes(sb.ToString()), "text/plain", fileName);
        //        //return File(path, "text/plain", fileName);
        //    }

        //    // Please change this.
        //    return Content("No data");
        //}

        //[HttpPost]
        //public FileStreamResult GenerateP1(List<TPOFeeExeGLAccruedViewListModel> bulkData)
        //{
        //    MemoryStream ms = new MemoryStream();
        //    var regional = bulkData.Select(c => c.Regional).FirstOrDefault();
        //    var kpsWeek = bulkData.Select(c => c.KpsWeek).FirstOrDefault();
        //    var kpsYear = bulkData.Select(c => c.KpsYear).FirstOrDefault();

        //    //string fileName = "Upload GL W" + kpsWeek + " " + regional + ".txt";
            
        //    string path = Server.MapPath("~/Content/P1Template/" + fileName);

        //    if (bulkData.Any())
        //    {
        //        if (bulkData.Count(p => p.Checkbox) != 0)
        //        {
                    

        //            if (System.IO.File.Exists(path))
        //            {
        //                System.IO.File.Delete(path);
        //            }

        //            var counter = bulkData.Count(p => p.Checkbox);

        //            if (!System.IO.File.Exists(path))
        //            {
        //                for (int i = 0; i < bulkData.Count; i++)
        //                {
        //                    if (bulkData[i].Checkbox == true)
        //                    {
        //                        var mstGenWeek = _masterDataBll.GetWeekByYearAndWeek(kpsYear, kpsWeek);
        //                        var text = bulkData[i].Brand.Substring(0, 3) + "-" + bulkData[i].Brand.Substring(3, 2) +
        //                                   bulkData[i].Location
        //                                   + mstGenWeek.StartDate.Value.Day + "." +
        //                                   mstGenWeek.StartDate.Value.ToString("MM") + "." +
        //                                   mstGenWeek.StartDate.Value.Year.ToString().Substring(2, 2) + "-"
        //                                   + mstGenWeek.EndDate.Value.Day + "." +
        //                                   mstGenWeek.EndDate.Value.ToString("MM") + "." +
        //                                   mstGenWeek.EndDate.Value.Year.ToString().Substring(2, 2) + "(w." + kpsWeek + ")";

        //                        using (StreamWriter sw = System.IO.File.AppendText(path))
        //                        {
        //                            string[] title =
        //                        {
        //                            "Type", "Company", "Currency", "Exchange rate", "Document type", "Translation date"
        //                            , "Header text", "Reference", "CC transaction", "Document date", "Posting date",
        //                            "Automatic tax", "Posting key", "Account", "Doc cur amount", "Local cur amount",
        //                            "Local currency", "Tax code", "PO Number", "PO Item Number", "Quantity", "UOM",
        //                            "Assignment", "Text", "Special GL Indicator", "Recovery Indicator", "Customer",
        //                            "Baseline date", "Value date", "Cost center", "WBS", "Material number",
        //                            "Brand family",
        //                            "Payment terms", "Cash discount 1", "Trading partner", "New company",
        //                            "Interco affiliate",
        //                            "Production center", "PMI Market", "Product category", "Ship-to", "Label",
        //                            "Final market",
        //                            "DocNumber-Earmarked Funds", "Earmarked Funds", "Tax Base Amount",
        //                            "Withholding Tax Base Amount",
        //                            "Batch number", "Business Place", "Section Code", "Amount in 2nd local currency",
        //                            "Amount in 3rd local currency",
        //                            "W. Tax Code"
        //                        };

        //                            string[] content1 =
        //                        {
        //                            "1", "3066", "IDR", "", "SC", ""
        //                            ,
        //                            bulkData[i].Location + " w." + kpsWeek + " " +
        //                            kpsYear,
        //                            "SRGLC150319", "",
        //                            DateTime.Now.Day + "." + DateTime.Now.Month + "." +
        //                            DateTime.Now.Year.ToString().Substring(2, 2),
        //                            DateTime.Now.Day + "." + DateTime.Now.Month + "." +
        //                            DateTime.Now.Year.ToString().Substring(2, 2),
        //                            "", "", "", "", "",
        //                            "", "", "", "", "", "",
        //                            "", "", "", "", "",
        //                            "", "", "", "", "", "",
        //                            "", "", "", "", "",
        //                            "", "", "", "", "", "",
        //                            "", "", "", "",
        //                            "", "", "", "", "",
        //                            ""
        //                        };

        //                            string[] content2 =
        //                        {
        //                            "2", "", "", "", "", ""
        //                            , "",
        //                            "", "",
        //                            "",
        //                            "",
        //                            "", "50", "22561000", "SKIP",
        //                            (bulkData[i].BiayaProduksi + bulkData[i].JasaManagemen).ToString(),
        //                            "Jumlah Biaya Produksi + Pajak Jasa Maklon sebesar 2 %", "", "V0", "", "", "",
        //                            "", text, "", "", "",
        //                            "", "", "", "", "", "",
        //                            "", "", "", "", "",
        //                            "", "", "", "", "", "",
        //                            "", "", "", "",
        //                            "", "", "", "", "",
        //                            ""
        //                        };

        //                            string[] content3 =
        //                        {
        //                            "2", "", "", "", "", ""
        //                            , "",
        //                            "", "",
        //                            "",
        //                            "",
        //                            "", "40", "82182000", "SKIP",
        //                            bulkData[i].BiayaProduksi.ToString(),
        //                            "Biaya Produksi", "", "", "", "", "",
        //                            "", "", "", text + " Prod Fee", "", "",
        //                            "", "", "", "", "3066150VR1", "",
        //                            "", "", "", "", "",
        //                            "", "", "", "", "", "",
        //                            "", "", "", "",
        //                            "", "", "", "", "",
        //                            ""
        //                        };

        //                            string[] content4 =
        //                        {
        //                            "2", "", "", "", "", ""
        //                            , "",
        //                            "", "",
        //                            "",
        //                            "",
        //                            "", "40", "82182010", "SKIP",
        //                            bulkData[i].JasaManagemen.ToString(),
        //                            "Pajak Jasa Maklon sebesar 2 %", "", "", "", "", "",
        //                            "", "", "", text + "  Mgmt Fee", "", "",
        //                            "", "", "", "", "3066150VR1", "",
        //                            "", "", "", "", "",
        //                            "", "", "", "", "", "",
        //                            "", "", "", "",
        //                            "", "", "", "", "",
        //                            ""
        //                        };

        //                            string write = string.Empty;

        //                            foreach (var data in title)
        //                            {
        //                                write += data + "\t";
        //                            }

        //                            sw.WriteLine(write);

        //                            write = string.Empty;

        //                            foreach (var data in content1)
        //                            {
        //                                write += data + "\t";
        //                            }

        //                            sw.WriteLine(write);

        //                            write = string.Empty;

        //                            foreach (var data in content2)
        //                            {
        //                                write += data + "\t";
        //                            }

        //                            sw.WriteLine(write);

        //                            write = string.Empty;

        //                            foreach (var data in content3)
        //                            {
        //                                write += data + "\t";
        //                            }

        //                            sw.WriteLine(write);

        //                            write = string.Empty;

        //                            foreach (var data in content4)
        //                            {
        //                                write += data + "\t";
        //                            }

        //                            sw.WriteLine(write + System.Environment.NewLine);

        //                        }
        //                    }
        //                }
        //            }
        //        }
        //        else
        //        {
        //            bulkData = null;
        //        }
        //    }
        //    else
        //    {
        //        bulkData = null;
        //    }
        //    //return Json(bulkData);
        //    ms.Position = 0;
        //    return File(ms, ".txt", fileName);
        //}

        [HttpPost]
        public FileStreamResult GenerateExcel(GetTPOFeeExeGLAccruedInput input)
        {
            var result = _tpoFeeExeGlAccruedBll.GetTPOFeeExeGLAccruedViewList(input);

            var ms = new MemoryStream();

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = HMS.SKTIS.Core.Enums.ExcelTemplate.TPOFeeExeGLAccrued + ".xlsx";
            var templateFileName = Server.MapPath(Constants.TPOFeeExcelTemplatesFolder + templateFile);
            if (System.IO.File.Exists(templateFileName))
            {
                System.IO.File.Copy(templateFileName, strFileName);
            }

            var allLocations = _appService.GetLocationCodeCompat();
            string locationCompat = "";
            foreach (SelectListItem item in allLocations)
            {
                if (item.Value == input.Regional)
                {
                    locationCompat = item.Text;
                }
            }


            using (SLDocument slDoc = new SLDocument(strFileName))
            {

                //filter values
                slDoc.SetCellValue(3, 2, ": " + locationCompat);
                slDoc.SetCellValue(3, 9, ": " + input.KpsYear);
                slDoc.SetCellValue(4, 9, ": " + input.KpsWeek);
                slDoc.SetCellValue(5, 9, ": " + input.ClosingDate.Date.ToString(Constants.DefaultDateOnlyFormat));

                //row values
                var iRow = 9;

                foreach (var item in result)
                {
                    SLStyle style = slDoc.CreateStyle();
                    style.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Font.FontName = "Calibri";
                    style.Font.FontSize = 10;

                    if (iRow % 2 == 0)
                    {
                        style.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);
                    }

                    slDoc.SetCellValue(iRow, 1, item.Location);
                    slDoc.SetCellValue(iRow, 2, item.LocationName);
                    slDoc.SetCellValue(iRow, 3, item.Brand);
                    slDoc.SetCellValue(iRow, 4, item.Note);
                    slDoc.SetCellValue(iRow, 5, item.JknBox);
                    slDoc.SetCellValue(iRow, 6, item.Jl1Box);
                    slDoc.SetCellValue(iRow, 7, item.Jl2Box);
                    slDoc.SetCellValue(iRow, 8, item.Jl3Box);
                    slDoc.SetCellValue(iRow, 9, item.Jl4Box);
                    slDoc.SetCellValue(iRow, 10, item.BiayaProduksi);
                    slDoc.SetCellValue(iRow, 11, item.JasaManagemen);
                    slDoc.SetCellStyle(iRow, 1, iRow, 11, style);
                    iRow++;
                }

                var slSheetProtection = new SLSheetProtection
                {
                    AllowInsertRows = false,
                    AllowInsertColumns = false,
                    AllowDeleteRows = false,
                    AllowDeleteColumns = false,
                    AllowFormatCells = true,
                    AllowFormatColumns = true,
                    AllowFormatRows = true,
                    AllowAutoFilter = true,
                    AllowSort = true
                };


                slDoc.ProtectWorksheet(slSheetProtection);
                slDoc.AutoFitColumn(1, 12);

                System.IO.File.Delete(strFileName);
                slDoc.SaveAs(ms);
            }
            // this is important. Otherwise you get an empty file
            // (because you'd be at EOF after the stream is written to, I think...).
            ms.Position = 0;
            var fileName = "TPOFee_ TPOFeeGLList _" + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}