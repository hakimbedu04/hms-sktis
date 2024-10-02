using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.BusinessObjects.Inputs.TPOFee;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using SKTISWebsite.Code;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.TPOFeeAPClose;
using SKTISWebsite.Models.GenerateP1TemplateAP;
using SpreadsheetLight;
using HMS.SKTIS.BusinessObjects.Inputs;

namespace SKTISWebsite.Controllers
{
    public class TPOFeeAPCloseController : BaseController
    {
        private IApplicationService _svc;
        private IMasterDataBLL _masterDataBll;
        private ITPOFeeBLL _tpoFeeBll;
        private IGeneralBLL _generalBll;

        public TPOFeeAPCloseController(IApplicationService applicationService, IMasterDataBLL masterDataBll, ITPOFeeBLL tpoFeeBll, IGeneralBLL generalBll)
        {
            _svc = applicationService;
            _masterDataBll = masterDataBll;
            _tpoFeeBll = tpoFeeBll;
            _generalBll = generalBll;
            SetPage("TPOFee/Execution/TPOFeeAPClose");
        }

        // GET: TPOFeeAPClose
        public ActionResult Index()
        {
            var init = new InitTPOFeeAPCloseViewModel()
            {
                YearSelectList = _svc.GetGenWeekYears(),
                DefaultWeek = _masterDataBll.GetGeneralWeekWeekByDate(DateTime.Now),
                Regional = _svc.GetTPOLocationCodeSelectListCompat()
            };
            return View("Index", init);
        }

        [HttpGet]
        public JsonResult GetWeekByYear(int year)
        {
            var kpsWeek = _masterDataBll.GetWeekByYear(year);
            return Json(kpsWeek, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult GetTPOFeeAPClose(GetTPOFeeAPCloseInput criteria)
        {
            var result = _tpoFeeBll.GetTpoFeeAPClose(criteria);
            var viewModel = Mapper.Map<List<TPOFeeAPCloseViewModel>>(result);
            var pageResult = new PageResult<TPOFeeAPCloseViewModel>(viewModel, criteria);
            return Json(pageResult);
        }

        [HttpPost]
        public FileStreamResult GenerateExcel(GetTPOFeeAPCloseInput criteria)
        {
            var tpoFeeApOpenDtos = _tpoFeeBll.GetTpoFeeAPClose(criteria);

            MemoryStream ms = new MemoryStream();

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = Enums.ExcelTemplate.TPOFeeAPClose + ".xlsx";
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
                slDoc.SetCellValue(3, 16, criteria.Year);
                slDoc.SetCellValue(4, 16, criteria.Week);

                //row values
                var iRow = 8;

                foreach (var tpoApp in tpoFeeApOpenDtos)
                {
                    SLStyle style = slDoc.CreateStyle();
                    style.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Font.FontName = "Calibri";
                    style.Font.FontSize = 10;

                    //if (tpoApp.Status == "APPROVED")
                    //{
                    //    style.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.DarkSalmon, System.Drawing.Color.DarkSalmon);
                    //}

                    
                    slDoc.SetCellValue(iRow, 1, tpoApp.LocationCode);
                    slDoc.SetCellValue(iRow, 2, tpoApp.LocationName);
                    slDoc.SetCellValue(iRow, 3, tpoApp.SKTBrandCode);
                    slDoc.SetCellValue(iRow, 4, tpoApp.Note);
                    slDoc.SetCellValue(iRow, 5, tpoApp.JKN.ToString());
                    slDoc.SetCellValue(iRow, 6, tpoApp.JL1.ToString());
                    slDoc.SetCellValue(iRow, 7, tpoApp.JL2.ToString());
                    slDoc.SetCellValue(iRow, 8, tpoApp.JL3.ToString());
                    slDoc.SetCellValue(iRow, 9, tpoApp.JL4.ToString());
                    slDoc.SetCellValue(iRow, 10, tpoApp.BiayaProduksi.ToString());
                    slDoc.SetCellValue(iRow, 11, tpoApp.JasaManajemen.ToString());
                    slDoc.SetCellValue(iRow, 12, tpoApp.ProductivityIncentives.ToString());
                    slDoc.SetCellValue(iRow, 13, tpoApp.JasaManajemen2Percent.ToString());
                    slDoc.SetCellValue(iRow, 14, tpoApp.ProductivityIncentives2Percent.ToString());
                    slDoc.SetCellValue(iRow, 15, tpoApp.BiayaProduksi10Percent.ToString());
                    slDoc.SetCellValue(iRow, 16, tpoApp.JasaMakloon10Percent.ToString());
                    slDoc.SetCellValue(iRow, 17, tpoApp.ProductivityIncentives10Percent.ToString());
                    slDoc.SetCellValue(iRow, 18, tpoApp.TotalBayar.ToString());
                    slDoc.SetCellStyle(iRow, 1, iRow, 18, style);
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
                //slDoc.AutoFitColumn(1, 6);

                System.IO.File.Delete(strFileName);
                slDoc.SaveAs(ms);
            }
            // this is important. Otherwise you get an empty file
            // (because you'd be at EOF after the stream is written to, I think...).
            ms.Position = 0;
            var fileName = "TPOFee_TPOFeeApClose_" + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpPost]
        public ActionResult GenerateP1(P1Template<TPOFeeAPCloseViewModel> bulkData)
        {
            var counter = bulkData.Data.Count(p => p.Check);
            if (bulkData.Data.Any() && counter > 0)
            {
                var location = bulkData.Data.Select(c => c.LocationCode).FirstOrDefault();
                string regional = _masterDataBll.GetLocation(location).ParentLocationCode;
                int kpsWeek = bulkData.Data.Select(c => c.KPSWeek).FirstOrDefault();
                int kpsYear = bulkData.Data.Select(c => c.KPSYear).FirstOrDefault();

                var fileName = "Upload Fee Close W" + kpsWeek + " " + regional + ".txt";

                var generateData = new GenerateP1TemplateAP();
                var sb = generateData.getP1Close(bulkData, regional,kpsWeek,kpsYear);
                return File(Encoding.ASCII.GetBytes(sb.ToString()), "text/plain", fileName);
            }
            return Content("No data");

            //var sb = new StringBuilder();

            //var counter = bulkData.Data.Count(p => p.Check);
            //if (bulkData.Data.Any() && counter > 0)
            //{
            //    var location = bulkData.Data.Select(c => c.LocationCode).FirstOrDefault();
            //    var regional = _masterDataBll.GetLocation(location).ParentLocationCode;
            //    var kpsWeek = bulkData.Data.Select(c => c.KPSWeek).FirstOrDefault();
            //    var kpsYear = bulkData.Data.Select(c => c.KPSYear).FirstOrDefault();

            //    string fileName = "Upload Fee Close W" + kpsWeek + " " + regional + ".txt";

            //    var criteria = new GetTPOFeeAPOpenInput()
            //    {
            //        Regional = regional,
            //        Week = kpsWeek,
            //        Year = kpsYear
            //    };

            //    var dto = _tpoFeeBll.GetP1TemplateAP(criteria);
            //    var viewModel = Mapper.Map<List<GenerateP1TemplateAPViewModel>>(dto);
            //    string[] title =
            //            {
            //                "counter", "Company Code", "Currency Key", "Document Type", "Tax Date",	"Document Header Text",	"Reference", "Document Date",
            //                "SCB Indicator", "Payment Reference", "House Bank",	"Instruction Key 1", "Instruction Key 2", "Instruction Key 3", "Instruction Key 4",
            //                "Partner Bank Type", "Reference Text 1", "Posting Key",	"Account", "Amount in Document Currency", "Local Currency", "Tax code",
            //                "Quantity", "Base Unit of Measure",	"Assignment", "Item Text", "Long Text",	"Cost Center",	"WBS Element", "Material Number",
            //                "Payment terms", "Trading partner", "Order", "Ship-to party", "Customer CO-PA", "Brand family CO-PA", "Interco affiliate CO-PA",
            //                "WBS CO-PA", "Product CO-PA", "Production center CO-PA", "PMI Market CO-PA",	"Product category CO-PA",	"Label CO-PA",	"Trade Channel CO-PA",
            //                "Ship-to party COPA",	"Final Market COPA",	"Credit Control area",	"Indicator: Negative posting",	"Fiscal Year",	"Purchasing Document Number",
            //                "Item Number of PO",	"ISR Account",	"ISR Reference",	"Plant",	"Identifier"
            //            };
            //    //string write = string.Empty;

            //    //foreach (var data in title)
            //    //{
            //    //    write += data + "\t";
            //    //}
            //    //sb.Append(write + Environment.NewLine);
            //    var counterTitle = 0;
            //    for (int i = 0; i < viewModel.Count; i++)
            //    {
            //        for (int j = 0; j < bulkData.Data.Count; j++)
            //        {
            //            if (viewModel[i].LocationCode == bulkData.Data[j].LocationCode)
            //            {
            //                if (bulkData.Data[j].Check == true)
            //                {
            //                    string amountInDocumentCurrency = "";
            //                    if (viewModel[i].AmountInDocumentCurrency.GetValueOrDefault() == null || viewModel[i].AmountInDocumentCurrency.GetValueOrDefault() == 0)
            //                    {
            //                        amountInDocumentCurrency = amountInDocumentCurrency;
            //                    }
            //                    else
            //                    {
            //                        amountInDocumentCurrency = Convert.ToString(viewModel[i].AmountInDocumentCurrency.Value);
            //                    }

            //                    string[] content1 =
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
            //                    string write = string.Empty;
            //                    if(counterTitle == 0){
            //                        foreach (var data in title)
            //                        {
            //                            write += data + "\t";
            //                        }

            //                        sb.Append(write + Environment.NewLine);
            //                        counterTitle = counterTitle + 1;
            //                    }

            //                    write = string.Empty;

            //                    foreach (var data in content1)
            //                    {
            //                        write += data + "\t";
            //                    }

            //                    sb.Append(write + Environment.NewLine);

            //                    if (viewModel[i].no == "5")
            //                    {
            //                        sb.Append(Environment.NewLine);
            //                    }

            //                    // commented related to this ticket http://tp.voxteneo.co.id/entity/11063
            //                    //_generalBll.ExeTransactionLog(new TransactionLogInput()
            //                    //{
            //                    //    page = Enums.PageName.TPOFeeAP.ToString(),
            //                    //    ActionButton = Enums.ButtonName.P1Template.ToString(),
            //                    //    UserName = GetUserName(),
            //                    //    TransactionCode = bulkData.Data[i].TPOFeeCode,
            //                    //    ActionTime = DateTime.Now.AddSeconds(i),
            //                    //    TransactionDate = DateTime.Now.AddSeconds(i),
            //                    //    IDRole = CurrentUser.Responsibility.Role
            //                    //});
            //                    // commented related to this ticket http://tp.voxteneo.co.id/entity/11063
            //                }
            //            }
            //        }
            //    }
            //    //}
            //    //}
            //    return File(Encoding.ASCII.GetBytes(sb.ToString()), "text/plain", fileName);
            //    //return File(path, "text/plain", fileName);
            //}

            //return Content("No data");
        }
    }
}