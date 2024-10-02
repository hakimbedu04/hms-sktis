using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml.Spreadsheet;
using HMS.SKTIS.BusinessObjects.Inputs.TPOFee;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using SKTISWebsite.Code;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.TPOFeeExeAPOpen;
using SpreadsheetLight;
using HMS.SKTIS.BusinessObjects.DTOs.TPOFee;
using SKTISWebsite.Models.TPOFeeAPOpen;
using SKTISWebsite.Models.TPOGenerateP1TemplateView;
using SKTISWebsite.Models.GenerateP1TemplateAP;

namespace SKTISWebsite.Controllers
{
    public class TPOFeeExeAPOpenController : BaseController
    {
        private IApplicationService _svc;
        private IMasterDataBLL _masterDataBll;
        private ITPOFeeBLL _tpoFeeBll;
        private IUtilitiesBLL _utilitiesBll;
        private IGeneralBLL _generalBll;

        public TPOFeeExeAPOpenController(
            IApplicationService applicationService,
            IMasterDataBLL masterDataBll,
            ITPOFeeBLL tpoFeeBll,
            IUtilitiesBLL utilitiesBll,
            IGeneralBLL generalBll
        )
        {
            _svc = applicationService;
            _masterDataBll = masterDataBll;
            _tpoFeeBll = tpoFeeBll;
            _utilitiesBll = utilitiesBll;
            _generalBll = generalBll;
            SetPage("TPOFee/Execution/TPOFeeAP");
        }

        // GET: TPOFeeExeActual
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
            var init = new InitTPOFeeExeAPOpenViewModel()
            {
                YearSelectList = _svc.GetGenWeekYears(),
                DefaultYear = yearOld,
                DefaultWeek = weekOld,
                DefaultRegional = regionalOld,
                Regional = _svc.GetTPOLocationCodeSelectListCompat(),
                //roleP1Template = _utilitiesBll.RoleButtonChecker(param1.Replace("_", "/"), CurrentUser.Responsibility.Role, Enums.PageName.TPOFeeAP.ToString(), Enums.ButtonName.P1Template.ToString())
            };

          
            

            return View("Index", init);
        }

        [HttpGet]
        public JsonResult GetWeekByYear(int year)
        {
            var kpsWeek = _masterDataBll.GetWeekByYear(year);
            return Json(kpsWeek, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //public JsonResult GenerateP1(List<TPOFeeExeAPOpenViewModel> bulkData)
        //{
        //    if (bulkData.Any())
        //    {
        //        //SKTISWebsite.Models.TPOFeeExeGLAccrued.TPOFeeExeGLAccruedViewListModel;
        //        var location = bulkData.Select(c => c.LocationCode).FirstOrDefault();
        //        var regional = _masterDataBll.GetLocation(location).ParentLocationCode;
        //        var kpsWeek = bulkData.Select(c => c.KPSWeek).FirstOrDefault();
        //        var kpsYear = bulkData.Select(c => c.KPSYear).FirstOrDefault();

        //        string fileName = "Upload Fee Open W" + kpsWeek + " " + regional + ".txt";

        //        string path = Server.MapPath("/Content/P1Template/" + fileName);

        //        if (System.IO.File.Exists(path))
        //        {
        //            System.IO.File.Delete(path);
        //        }

        //        if (!System.IO.File.Exists(path))
        //        {
        //            var dto = _tpoFeeBll.GetCompletedTpoFeeAPOpen(new GetTPOFeeAPOpenInput()
        //            {
        //                Regional = regional,
        //                Year = kpsYear,
        //                Week = kpsWeek
        //            });
        //            var viewModel = Mapper.Map<List<TPOFeeAPOpenViewModel>>(dto);
        //            for (int i = 0; i < viewModel.Count; i++)
        //            {
        //                var mstGenWeek = _masterDataBll.GetWeekByYearAndWeek(kpsYear, kpsWeek);
        //                var text = viewModel[i].SKTBrandCode +
        //                            viewModel[i].LocationCode
        //                            + mstGenWeek.StartDate.Value.Day + "." +
        //                            mstGenWeek.StartDate.Value.ToString("MM") + "." +
        //                            mstGenWeek.StartDate.Value.Year.ToString().Substring(2, 2) + "-"
        //                            + mstGenWeek.EndDate.Value.Day + "." +
        //                            mstGenWeek.EndDate.Value.ToString("MM") + "." +
        //                            mstGenWeek.EndDate.Value.Year.ToString().Substring(2, 2) + "(w." + kpsWeek + ")";

        //                using (StreamWriter sw = System.IO.File.AppendText(path))
        //                {
        //                    string[] title =
        //                {
        //                    "Type", "Company", "Currency", "Exchange rate", "Document type", "Translation date"
        //                    , "Header text", "Reference", "CC transaction", "Document date", "Posting date",
        //                    "Automatic tax", "Posting key", "Account", "Doc cur amount", "Local cur amount",
        //                    "Local currency", "Tax code", "PO Number", "PO Item Number", "Quantity", "UOM",
        //                    "Assignment", "Text", "Special GL Indicator", "Recovery Indicator", "Customer",
        //                    "Baseline date", "Value date", "Cost center", "WBS", "Material number",
        //                    "Brand family",
        //                    "Payment terms", "Cash discount 1", "Trading partner", "New company",
        //                    "Interco affiliate",
        //                    "Production center", "PMI Market", "Product category", "Ship-to", "Label",
        //                    "Final market",
        //                    "DocNumber-Earmarked Funds", "Earmarked Funds", "Tax Base Amount",
        //                    "Withholding Tax Base Amount",
        //                    "Batch number", "Business Place", "Section Code", "Amount in 2nd local currency",
        //                    "Amount in 3rd local currency",
        //                    "W. Tax Code"
        //                };

        //                    string[] content1 =
        //                {
        //                    "1", "3066", "IDR", "", "SC", ""
        //                    ,
        //                    viewModel[i].LocationCode + " w." + kpsWeek + " " +
        //                    kpsYear,
        //                    "SRGLC150319", "",
        //                    DateTime.Now.Day + "." + DateTime.Now.Month + "." +
        //                    DateTime.Now.Year.ToString().Substring(2, 2),
        //                    DateTime.Now.Day + "." + DateTime.Now.Month + "." +
        //                    DateTime.Now.Year.ToString().Substring(2, 2),
        //                    "", "", "", "", "",
        //                    "", "", "", "", "", "",
        //                    "", "", "", "", "",
        //                    "", "", "", "", "", "",
        //                    "", "", "", "", "",
        //                    "", "", "", "", "", "",
        //                    "", "", "", "",
        //                    "", "", "", "", "",
        //                    ""
        //                };

        //                    string[] content2 =
        //                {
        //                    "2", "", "", "", "", ""
        //                    , "",
        //                    "", "",
        //                    "",
        //                    "",
        //                    "", "50", "22561000", "SKIP",
        //                    (viewModel[i].BiayaProduksi + viewModel[i].JasaManajemen2Percent).ToString(),
        //                    "Jumlah Biaya Produksi + Pajak Jasa Maklon sebesar 2 %", "", "V0", "", "", "",
        //                    "", text, "", "", "",
        //                    "", "", "", "", "", "",
        //                    "", "", "", "", "",
        //                    "", "", "", "", "", "",
        //                    "", "", "", "",
        //                    "", "", "", "", "",
        //                    ""
        //                };

        //                    string[] content3 =
        //                {
        //                    "2", "", "", "", "", ""
        //                    , "",
        //                    "", "",
        //                    "",
        //                    "",
        //                    "", "40", "82182000", "SKIP",
        //                    viewModel[i].BiayaProduksi.ToString(),
        //                    "Biaya Produksi", "", "", "", "", "",
        //                    "", "", "", text + " Prod Fee", "", "",
        //                    "", "", "", "", "3066150VR1", "",
        //                    "", "", "", "", "",
        //                    "", "", "", "", "", "",
        //                    "", "", "", "",
        //                    "", "", "", "", "",
        //                    ""
        //                };

        //                    string[] content4 =
        //                {
        //                    "2", "", "", "", "", ""
        //                    , "",
        //                    "", "",
        //                    "",
        //                    "",
        //                    "", "40", "82182010", "SKIP",
        //                    viewModel[i].JasaManajemen2Percent.ToString(),
        //                    "Pajak Jasa Maklon sebesar 2 %", "", "", "", "", "",
        //                    "", "", "", text + "  Mgmt Fee", "", "",
        //                    "", "", "", "", "3066150VR1", "",
        //                    "", "", "", "", "",
        //                    "", "", "", "", "", "",
        //                    "", "", "", "",
        //                    "", "", "", "", "",
        //                    ""
        //                };

        //                    string write = string.Empty;

        //                    foreach (var data in title)
        //                    {
        //                        write += data + "\t";
        //                    }

        //                    sw.WriteLine(write);

        //                    write = string.Empty;

        //                    foreach (var data in content1)
        //                    {
        //                        write += data + "\t";
        //                    }

        //                    sw.WriteLine(write);

        //                    write = string.Empty;

        //                    foreach (var data in content2)
        //                    {
        //                        write += data + "\t";
        //                    }

        //                    sw.WriteLine(write);

        //                    write = string.Empty;

        //                    foreach (var data in content3)
        //                    {
        //                        write += data + "\t";
        //                    }

        //                    sw.WriteLine(write);

        //                    write = string.Empty;

        //                    foreach (var data in content4)
        //                    {
        //                        write += data + "\t";
        //                    }

        //                    sw.WriteLine(write + System.Environment.NewLine);

        //                }

        //                _generalBll.ExeTransactionLog(new TransactionLogInput()
        //                {
        //                    page = Enums.PageName.TPOFeeAP.ToString(),
        //                    ActionButton = Enums.ButtonName.P1Template.ToString(),
        //                    UserName = GetUserName(),
        //                    TransactionCode = viewModel[i].TPOFeeCode,
        //                    ActionTime = DateTime.Now.AddSeconds(i),
        //                    TransactionDate = DateTime.Now.AddSeconds(i)
        //                });
        //            }
        //        }
        //    }
        //    else
        //    {
        //        bulkData = null;
        //    }

        //    return Json(bulkData);
        //}
        //[HttpPost]
        //public ActionResult GenerateP1(GetTPOFeeAPOpenInput criteria)
        //{
        //    var sb = new StringBuilder();

        //    string fileName = "Upload Fee Open W" + criteria.Week + " " + criteria.Regional + ".txt";

        //    //string path = Server.MapPath("/Content/P1Template/" + fileName);

        //    //if (System.IO.File.Exists(path))
        //    //{
        //    //    System.IO.File.Delete(path);
        //    //}

        //    //if (!System.IO.File.Exists(path))
        //    //{
        //        var dto = _tpoFeeBll.GetCompletedTpoFeeAPOpen(criteria);
        //        var viewModel = Mapper.Map<List<TPOFeeAPOpenViewModel>>(dto);
        //        for (int i = 0; i < viewModel.Count; i++)
        //        {
        //            var mstGenWeek = _masterDataBll.GetWeekByYearAndWeek(criteria.Year, criteria.Week);
        //            var text = viewModel[i].SKTBrandCode +
        //                        viewModel[i].LocationCode
        //                        + mstGenWeek.StartDate.Value.Day + "." +
        //                        mstGenWeek.StartDate.Value.ToString("MM") + "." +
        //                        mstGenWeek.StartDate.Value.Year.ToString().Substring(2, 2) + "-"
        //                        + mstGenWeek.EndDate.Value.Day + "." +
        //                        mstGenWeek.EndDate.Value.ToString("MM") + "." +
        //                        mstGenWeek.EndDate.Value.Year.ToString().Substring(2, 2) + "(w." + criteria.Week + ")";

        //            //using (StreamWriter sw = System.IO.File.AppendText(path))
        //            //{
        //                string[] title =
        //                {
        //                    "Type", "Company", "Currency", "Exchange rate", "Document type", "Translation date"
        //                    , "Header text", "Reference", "CC transaction", "Document date", "Posting date",
        //                    "Automatic tax", "Posting key", "Account", "Doc cur amount", "Local cur amount",
        //                    "Local currency", "Tax code", "PO Number", "PO Item Number", "Quantity", "UOM",
        //                    "Assignment", "Text", "Special GL Indicator", "Recovery Indicator", "Customer",
        //                    "Baseline date", "Value date", "Cost center", "WBS", "Material number",
        //                    "Brand family",
        //                    "Payment terms", "Cash discount 1", "Trading partner", "New company",
        //                    "Interco affiliate",
        //                    "Production center", "PMI Market", "Product category", "Ship-to", "Label",
        //                    "Final market",
        //                    "DocNumber-Earmarked Funds", "Earmarked Funds", "Tax Base Amount",
        //                    "Withholding Tax Base Amount",
        //                    "Batch number", "Business Place", "Section Code", "Amount in 2nd local currency",
        //                    "Amount in 3rd local currency",
        //                    "W. Tax Code"
        //                };

        //                string[] content1 =
        //                {
        //                    "1", "3066", "IDR", "", "SC", ""
        //                    ,
        //                    viewModel[i].LocationCode + " w." + criteria.Week + " " +
        //                    criteria.Year,
        //                    "SRGLC150319", "",
        //                    DateTime.Now.Day + "." + DateTime.Now.Month + "." +
        //                    DateTime.Now.Year.ToString().Substring(2, 2),
        //                    DateTime.Now.Day + "." + DateTime.Now.Month + "." +
        //                    DateTime.Now.Year.ToString().Substring(2, 2),
        //                    "", "", "", "", "",
        //                    "", "", "", "", "", "",
        //                    "", "", "", "", "",
        //                    "", "", "", "", "", "",
        //                    "", "", "", "", "",
        //                    "", "", "", "", "", "",
        //                    "", "", "", "",
        //                    "", "", "", "", "",
        //                    ""
        //                };

        //                string[] content2 =
        //                {
        //                    "2", "", "", "", "", ""
        //                    , "",
        //                    "", "",
        //                    "",
        //                    "",
        //                    "", "50", "22561000", "SKIP",
        //                    (viewModel[i].BiayaProduksi + viewModel[i].JasaManajemen2Percent).ToString(),
        //                    "Jumlah Biaya Produksi + Pajak Jasa Maklon sebesar 2 %", "", "V0", "", "", "",
        //                    "", text, "", "", "",
        //                    "", "", "", "", "", "",
        //                    "", "", "", "", "",
        //                    "", "", "", "", "", "",
        //                    "", "", "", "",
        //                    "", "", "", "", "",
        //                    ""
        //                };

        //                string[] content3 =
        //                {
        //                    "2", "", "", "", "", ""
        //                    , "",
        //                    "", "",
        //                    "",
        //                    "",
        //                    "", "40", "82182000", "SKIP",
        //                    viewModel[i].BiayaProduksi.ToString(),
        //                    "Biaya Produksi", "", "", "", "", "",
        //                    "", "", "", text + " Prod Fee", "", "",
        //                    "", "", "", "", "3066150VR1", "",
        //                    "", "", "", "", "",
        //                    "", "", "", "", "", "",
        //                    "", "", "", "",
        //                    "", "", "", "", "",
        //                    ""
        //                };

        //                string[] content4 =
        //                {
        //                    "2", "", "", "", "", ""
        //                    , "",
        //                    "", "",
        //                    "",
        //                    "",
        //                    "", "40", "82182010", "SKIP",
        //                    viewModel[i].JasaManajemen2Percent.ToString(),
        //                    "Pajak Jasa Maklon sebesar 2 %", "", "", "", "", "",
        //                    "", "", "", text + "  Mgmt Fee", "", "",
        //                    "", "", "", "", "3066150VR1", "",
        //                    "", "", "", "", "",
        //                    "", "", "", "", "", "",
        //                    "", "", "", "",
        //                    "", "", "", "", "",
        //                    ""
        //                };

        //                string write = string.Empty;

        //                foreach (var data in title)
        //                {
        //                    write += data + "\t";
        //                }

        //                sb.Append(write + Environment.NewLine);

        //                write = string.Empty;

        //                foreach (var data in content1)
        //                {
        //                    write += data + "\t";
        //                }

        //                sb.Append(write + Environment.NewLine);

        //                write = string.Empty;

        //                foreach (var data in content2)
        //                {
        //                    write += data + "\t";
        //                }

        //                sb.Append(write + Environment.NewLine);

        //                write = string.Empty;

        //                foreach (var data in content3)
        //                {
        //                    write += data + "\t";
        //                }

        //                sb.Append(write + Environment.NewLine);

        //                write = string.Empty;

        //                foreach (var data in content4)
        //                {
        //                    write += data + "\t";
        //                }

        //                sb.Append(write + Environment.NewLine + Environment.NewLine);

        //            //}

        //            _generalBll.ExeTransactionLog(new TransactionLogInput()
        //            {
        //                page = Enums.PageName.TPOFeeAP.ToString(),
        //                ActionButton = Enums.ButtonName.P1Template.ToString(),
        //                UserName = GetUserName(),
        //                TransactionCode = viewModel[i].TPOFeeCode,
        //                ActionTime = DateTime.Now.AddSeconds(i),
        //                TransactionDate = DateTime.Now.AddSeconds(i),
        //                IDRole = CurrentUser.Responsibility.Role
        //            });
        //        }
        //    //}

        //        return File(Encoding.ASCII.GetBytes(sb.ToString()), "text/plain", fileName);

        //    //return File(path, "text/plain", fileName);
        //}

        [HttpPost]
        //public ActionResult GenerateP1(GetTPOFeeAPOpenInput criteria)
        //{
        //    var sb = new StringBuilder();

        //    string fileName = "Upload Fee Open W" + criteria.Week + " " + criteria.Regional + ".txt";

        //    //var dto = _tpoFeeBll.GetP1Template(criteria);
        //    var dto = _tpoFeeBll.GetP1TemplateAP(criteria);
        //    var viewModel = Mapper.Map<List<GenerateP1TemplateAPViewModel>>(dto);
            
        //    string[] title =
        //                {
        //                    "counter", "Company Code", "Currency Key", "Document Type", "Tax Date",	"Document Header Text",	"Reference", "Document Date",
        //                    "SCB Indicator", "Payment Reference", "House Bank",	"Instruction Key 1", "Instruction Key 2", "Instruction Key 3", "Instruction Key 4",
        //                    "Partner Bank Type", "Reference Text 1", "Posting Key",	"Account", "Amount in Document Currency", "Local Currency", "Tax code",
        //                    "Quantity", "Base Unit of Measure",	"Assignment", "Item Text", "Long Text",	"Cost Center",	"WBS Element", "Material Number",
        //                    "Payment terms", "Trading partner", "Order", "Ship-to party", "Customer CO-PA", "Brand family CO-PA", "Interco affiliate CO-PA",
        //                    "WBS CO-PA", "Product CO-PA", "Production center CO-PA", "PMI Market CO-PA",	"Product category CO-PA",	"Label CO-PA",	"Trade Channel CO-PA",
        //                    "Ship-to party COPA",	"Final Market COPA",	"Credit Control area",	"Indicator: Negative posting",	"Fiscal Year",	"Purchasing Document Number",
        //                    "Item Number of PO",	"ISR Account",	"ISR Reference",	"Plant",	"Identifier"
        //                };
        //    string write = string.Empty;

        //    foreach (var data in title)
        //    {
        //        write += data + "\t";
        //    }
        //    sb.Append(write + Environment.NewLine);

        //    for (int i = 0; i < viewModel.Count; i++)
        //    {
        //        if(viewModel[i].FlowStatus == "AUTHORIZED" || viewModel[i].FlowStatus == "COMPLETED" || viewModel[i].FlowStatus == "END")
        //        {
        //            string amountInDocumentCurrency = "";
        //                    if (viewModel[i].AmountInDocumentCurrency.GetValueOrDefault() == null || viewModel[i].AmountInDocumentCurrency.GetValueOrDefault() == 0)
        //                    {
        //                        amountInDocumentCurrency = amountInDocumentCurrency;
        //                    }
        //                    else
        //                    {
        //                        amountInDocumentCurrency = Convert.ToString(viewModel[i].AmountInDocumentCurrency.Value);
        //                    }
        //            string[] content1 =
        //                    {
        //                        viewModel[i].counter,
        //                        viewModel[i].Company, viewModel[i].CurrencyKey, viewModel[i].DocumentType, viewModel[i].TaxDate, viewModel[i].DocumentHeaderText,
        //                        viewModel[i].Reference , viewModel[i].DocumentDate , viewModel[i].SCBIndocator , viewModel[i].PaymentReference , viewModel[i].HouseBank , viewModel[i].InstructionKey1 , 
        //                        viewModel[i].InstructionKey2 , viewModel[i].InstructionKey3 , viewModel[i].InstructionKey4 , viewModel[i].PartnerBankType , viewModel[i].ReferenceText1 , 
        //                        viewModel[i].PostingKey , viewModel[i].Account , amountInDocumentCurrency , viewModel[i].LocalCurrency , viewModel[i].TaxCode , viewModel[i].Quantity , 
        //                        viewModel[i].BaseUnitOfMeasure , viewModel[i].Assignment , viewModel[i].ItemText , viewModel[i].LongText , viewModel[i].CostCenter , viewModel[i].WBSElement , 
        //                        viewModel[i].MaterialNumber , viewModel[i].PaymentTerms , viewModel[i].TradingPartner , viewModel[i].Order , viewModel[i].ShipToParty , viewModel[i].BrandFamilyCOPA , 
        //                        viewModel[i].IntercoAffiliateCOPA , viewModel[i].WBSCOPA , viewModel[i].ProductCOPA , viewModel[i].ProductionCenterCOPA , viewModel[i].PMIMarketCOPA , viewModel[i].ProductCategoryCOPA , 
        //                        viewModel[i].LabelCOPA , viewModel[i].TradeChannelCOPA , viewModel[i].ShipToPartyCOPA , viewModel[i].FinalMarketCOPA , viewModel[i].CreditControlArea , viewModel[i].IndicatorNegativePosting , 
        //                        viewModel[i].FiscalYear , viewModel[i].PurchasingDocNum , viewModel[i].ItemNumOfPO , viewModel[i].ISRAccount , viewModel[i].ISRReference , viewModel[i].Plant , viewModel[i].Identifier
        //                    };

        //            write = string.Empty;

        //            foreach (var data in content1)
        //            {
        //                write += data + "\t";
        //            }

        //            sb.Append(write + Environment.NewLine);

        //            if (viewModel[i].no == "5")
        //            {
        //                sb.Append(Environment.NewLine);
        //            }

        //            //_generalBll.ExeTransactionLog(new TransactionLogInput()
        //            //{
        //            //    page = Enums.PageName.TPOFeeAP.ToString(),
        //            //    ActionButton = Enums.ButtonName.P1Template.ToString(),
        //            //    UserName = GetUserName(),
        //            //    TransactionCode = viewModel[i].TPOFeeCode,
        //            //    ActionTime = DateTime.Now.AddSeconds(i),
        //            //    TransactionDate = DateTime.Now.AddSeconds(i),
        //            //    IDRole = CurrentUser.Responsibility.Role
        //            //});
        //        }
                
        //    }
        //    return File(Encoding.ASCII.GetBytes(sb.ToString()), "text/plain", fileName);
        //}

        public ActionResult GenerateP1(GetTPOFeeAPOpenInput criteria)
        {
            //var sb = new StringBuilder();

            string fileName = "Upload Fee Open W" + criteria.Week + " " + criteria.Regional + ".txt";

            var generateData = new GenerateP1TemplateAP();
            var sb = generateData.getP1(criteria.Regional, criteria.Week, criteria.Year);
            
            return File(Encoding.ASCII.GetBytes(sb.ToString()), "text/plain", fileName);
        }
        

        [HttpPost]
        public JsonResult GetTPOFeeExeAPOpen(GetTPOFeeExeAPOpenInput criteria)
        {
            var result = _tpoFeeBll.GetTpoFeeExeAPOpens(criteria);

            /*foreach (TPOFeeExeAPOpenViewDTO fee in result)
            {
                var translog = _utilitiesBll.GetTransactionLogsByTransactionCode(fee.TPOFeeCode);
                var idFlow = translog.Max(m => m.IDFlow).ToString();

                if (idFlow != null)
                {
                    var role = _utilitiesBll.GetRoleByIDFlow(Int32.Parse(idFlow));
                    if (Int32.Parse(idFlow) == 40)
                    {
                         role = _utilitiesBll.GetRoleByIDFlow(35);
                    }
                    else {
                         role = _utilitiesBll.GetRoleByIDFlow(Int32.Parse(idFlow));
                    }
                    
                    fee.PIC = role != null ? role.RolesName : "";
                }
            }*/
            foreach (TPOFeeExeAPOpenViewDTO fee in result)
            {
                fee.PIC = _utilitiesBll.GetRoleByRoleCode(fee.PIC).RolesName;
            }
            var viewModel = Mapper.Map<List<TPOFeeExeAPOpenViewModel>>(result);
            var pageResult = new PageResult<TPOFeeExeAPOpenViewModel>(viewModel, criteria);

            TempData["Year"] = criteria.Year;
            TempData["week"] = criteria.Week;
           
            

            return Json(pageResult);
        }

        [HttpPost]
        public FileStreamResult GenerateExcel(GetTPOFeeExeAPOpenInput criteria)
        {
            criteria.SortExpression = "LocationCode";
            criteria.SortOrder = "ASC";

            //var tpoFeeExeActuals = _tpoFeeBll.GetTpoFeeExeActuals(criteria);
            var tpoFeeExeActuals = _tpoFeeBll.GetTpoFeeExeAPOpens(criteria);




            /*foreach (TPOFeeExeAPOpenViewDTO fee in tpoFeeExeActuals)
            {
                var translog = _utilitiesBll.GetTransactionLogsByTransactionCode(fee.TPOFeeCode);
                var idFlow = translog.Max(m => m.IDFlow).ToString();

                if (idFlow != null)
                {
                    var role = _utilitiesBll.GetRoleByIDFlow(Int32.Parse(idFlow));

                    fee.PIC = role != null ? role.RolesName + " - " + fee.PIC : fee.PIC;
                }
            }*/

            foreach (TPOFeeExeAPOpenViewDTO fee in tpoFeeExeActuals)
            {
                fee.PIC = _utilitiesBll.GetRoleByRoleCode(fee.PIC).RolesName;
            }

            MemoryStream ms = new MemoryStream();

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = Enums.ExcelTemplate.TPOFeeExeActual + ".xlsx";
            var templateFileName = Server.MapPath(Constants.TPOFeeExcelTemplatesFolder + templateFile);
            if (System.IO.File.Exists(templateFileName))
            {
                System.IO.File.Copy(templateFileName, strFileName);
            }


            var allLocations = _svc.GetLocationCodeCompat();
            string locationCompat = "";
            foreach (SelectListItem item in allLocations)
            {
                if (item.Value == criteria.Regional)
                {
                    locationCompat = item.Text;
                }
            }


            using (SLDocument slDoc = new SLDocument(strFileName))
            {
                //filter values
                slDoc.SetCellValue(3, 2, locationCompat);
                slDoc.SetCellValue(3, 5, criteria.Year);
                slDoc.SetCellValue(4, 5, criteria.Week);
                slDoc.SetCellValue(1, 1, "TPO Fee AP Open");

                //row values
                var iRow = 7;

                foreach (var tpoFeeExeActualViewDto in tpoFeeExeActuals)
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

                    slDoc.SetCellValue(iRow, 1, tpoFeeExeActualViewDto.LocationCode);
                    slDoc.SetCellValue(iRow, 2, tpoFeeExeActualViewDto.LocationName);
                    slDoc.SetCellValue(iRow, 3, tpoFeeExeActualViewDto.SKTBrandCode);
                    slDoc.SetCellValue(iRow, 4, tpoFeeExeActualViewDto.Status);
                    slDoc.SetCellValue(iRow, 5, tpoFeeExeActualViewDto.PIC);
                    slDoc.SetCellStyle(iRow, 1, iRow, 5, style);
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
                slDoc.AutoFitColumn(1, 6);

                System.IO.File.Delete(strFileName);
                slDoc.SaveAs(ms);
            }
            // this is important. Otherwise you get an empty file
            // (because you'd be at EOF after the stream is written to, I think...).
            ms.Position = 0;
            var fileName = "TPOFee_TPOFeeAPOpen_" + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}