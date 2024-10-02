using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.BusinessObjects.Inputs.TPOFee;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using SKTISWebsite.Code;
using SKTISWebsite.Helper;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.TPOFeeExePlan;
using SpreadsheetLight;
using Color = System.Drawing.Color;
using System.Globalization;

namespace SKTISWebsite.Controllers
{
    public class TPOFeeExePlanController : BaseController
    {
        private IApplicationService _applicationService;
        private IMasterDataBLL _masterDataBll;
        private ITPOFeeBLL _tpoFeeBll;
      
        public TPOFeeExePlanController(IApplicationService applicationService, IMasterDataBLL masterDataBll, ITPOFeeBLL tpoFeeBll)
        {
            _applicationService = applicationService;
            _masterDataBll = masterDataBll;
            _tpoFeeBll = tpoFeeBll;
            SetPage("TPOFee/Execution/TPOFeePlan");
        }

        // GET: TPOFeeExePlan
        public ActionResult Index()
        {

            var model = new InitTPOFeeExePlanViewModel();
            model.Regional = _applicationService.GetTPOLocationCodeSelectListCompat();
            model.KpsYear = _applicationService.GetYearSelectList(DateTime.Now.Year);
            model.KpsWeek = _applicationService.GetWeekSelectList(_masterDataBll.GetWeekByDate(DateTime.Now).Week);
            //model.DefaultRegional = _applicationService.GetTPOLocationCodeSelectList().Select(p => p.Value).FirstOrDefault();
            //model.DefaultKpsYear = DateTime.Now.Year;
            //model.DefaultKpsWeek = _masterDataBll.GetWeekByDate(DateTime.Now).Week;

            string regional;
            if (TempData["planRegional"] != null)
            {
                regional = TempData["planRegional"].ToString();
            }
            else
            {
                regional = _applicationService.GetTPOLocationCodeSelectList().Select(p => p.Value).FirstOrDefault();
            }

            int kpsYear = DateTime.Now.Year;
            if (TempData["planYear"] != null)
            {
                kpsYear = GenericHelper.IsNumeric(TempData["planYear"]) ? (int)TempData["planYear"] : DateTime.Now.Year;
            }

            int kpsWeek;
            if (TempData["planWeek"] != null)
            {
                kpsWeek = GenericHelper.IsNumeric(TempData["planWeek"]) ? (int)TempData["planWeek"] : DateTime.Now.Year;
            }
            else
            {
                var result = _masterDataBll.GetWeekByDate(DateTime.Now).Week;

                kpsWeek = result.HasValue ? result.Value : 1;
            }

            model.DefaultRegional = regional;
            model.DefaultKpsYear = kpsYear;
            model.DefaultKpsWeek = kpsWeek;

            return View("Index", model);
        }

        public JsonResult GetTpoFeePlan(GetTPOFeeHdrPlanInput criteria)
        {
            var pageResult = new PageResult<TpoFeeExePlanViewModel>();
            var tpoFeeExePlan = _tpoFeeBll.GetTpoFeePlanView(criteria);
            pageResult.TotalRecords = tpoFeeExePlan.Count;
            pageResult.TotalPages = (tpoFeeExePlan.Count / criteria.PageSize) + (tpoFeeExePlan.Count % criteria.PageSize != 0 ? 1 : 0);
            var result = tpoFeeExePlan.Skip((criteria.PageIndex - 1) * criteria.PageSize).Take(criteria.PageSize);
            pageResult.Results = Mapper.Map<List<TpoFeeExePlanViewModel>>(result);


            TempData["planRegional"] = criteria.ParentLocationCode;

            return Json(pageResult);



            //var data = _tpoFeeBll.GetTpoFeePlanView(input);

            //var result = Mapper.Map<List<TpoFeeExePlanViewModel>>(data);
            //return Json(result);
        }

        //[HttpPost]
        //public JsonResult GenerateP1(List<TpoFeeExePlanViewModel> bulkData)
        //{

        //    List<string> regionalList = new List<string>();

        //    foreach (var data in bulkData)
        //    {
        //        if (data.Checkbox != false)
        //        {
        //            var tpoFeeHdrPlanList = _tpoFeeBll.GetTpoFeeHdrPlan(data.TpoFeeCode);
        //            var locationObject = _masterDataBll.GetMstGenLocationsByParentCode(new GetMstGenLocationsByParentCodeInput
        //            {
        //                ParentLocationCode = tpoFeeHdrPlanList.LocationCode
        //            }).FirstOrDefault();
        //            var regionalObject = _masterDataBll.GetLocation(locationObject == null ? string.Empty : locationObject.ParentLocationCode);

        //            var check = regionalList.Where(p => p.Contains(regionalObject.LocationCode)).FirstOrDefault();

        //            if (check == null)
        //            {
        //                regionalList.Add(regionalObject.LocationCode);
        //            }
        //        }
        //    }

        //    if (bulkData.Where(p => p.Checkbox == true).Count() != 0)
        //    {
        //        string regionalCode = "REG";

        //        foreach (var data in regionalList)
        //        {
        //            regionalCode += data.Substring(3, 1);
        //        }


        //        var tpoFeeHdrPlanObject = _tpoFeeBll.GetTpoFeeHdrPlan(bulkData[0].TpoFeeCode);
        //        string fileName = "Upload Plan W" + tpoFeeHdrPlanObject.KPSWeek + " " + regionalCode + ".txt";

        //        string path = Server.MapPath("~/Content/P1Template/" + fileName);

        //        if (System.IO.File.Exists(path))
        //        {
        //            System.IO.File.Delete(path);
        //        }

        //        if (!System.IO.File.Exists(path))
        //        {


        //            for (int i = 0; i < bulkData.Count; i++)
        //            {

        //                var tpoFeeHdrPlan = _tpoFeeBll.GetTpoFeeHdrPlan(bulkData[i].TpoFeeCode);
        //                var mstGenBrandGroup = _masterDataBll.GetBrandGroupById(tpoFeeHdrPlan.BrandGroupCode);
        //                var mstGenWeek = _masterDataBll.GetWeekByYearAndWeek(tpoFeeHdrPlan.KPSYear, tpoFeeHdrPlan.KPSWeek);

        //                // Create a file to write to.
        //                using (StreamWriter sw = System.IO.File.CreateText(path))
        //                {
        //                    string[] title =
        //            {
        //                "Type", "Company", "Currency", "Exchange rate", "Document type", "Translation date"
        //                , "Header text", "Reference", "CC transaction", "Document date", "Posting date",
        //                "Automatic tax", "Posting key", "Account", "Doc cur amount", "Local cur amount",
        //                "Local currency", "Tax code", "PO Number", "PO Item Number", "Quantity", "UOM",
        //                "Assignment", "Text", "Special GL Indicator", "Recovery Indicator", "Customer",
        //                "Baseline date", "Value date", "Cost center", "WBS", "Material number", "Brand family",
        //                "Payment terms", "Cash discount 1", "Trading partner", "New company", "Interco affiliate",
        //                "Production center", "PMI Market", "Product category", "Ship-to", "Label", "Final market",
        //                "DocNumber-Earmarked Funds", "Earmarked Funds", "Tax Base Amount", "Withholding Tax Base Amount",
        //                "Batch number", "Business Place", "Section Code", "Amount in 2nd local currency",
        //                "Amount in 3rd local currency",
        //                "W. Tax Code"
        //            };

        //                    string[] content1 =
        //            {
        //                "1", "3066", "IDR", "", "KR", ""
        //                , tpoFeeHdrPlan.LocationCode + " w." + tpoFeeHdrPlan.KPSWeek + " " + tpoFeeHdrPlan.KPSYear,
        //                mstGenBrandGroup.SKTBrandCode, "",
        //                DateTime.Now.Day + "." + DateTime.Now.Month + "." + DateTime.Now.Year.ToString().Substring(2, 2),
        //                DateTime.Now.Day + "." + DateTime.Now.Month + "." + DateTime.Now.Year.ToString().Substring(2, 2),
        //                "X", "", "", "", "",
        //                "", "", "", "", "", "",
        //                "", "", "", "", "",
        //                "", "", "", "", "", "",
        //                "", "", "", "", "",
        //                "", "", "", "", "", "",
        //                "", "", "", "",
        //                "", "", "", "", "",
        //                ""
        //            };

        //                    string[] content2 =
        //            {
        //                "2", "", "", "", "", ""
        //                , "",
        //                "", "",
        //                "",
        //                "",
        //                "", "31", "",
        //                tpoFeeHdrPlan.TpoFeeCalculationPlans.Where(
        //                    p =>
        //                        p.ProductionFeeType == EnumHelper.GetDescription(Enums.ProductionFeeType.ProductionCost))
        //                    .Select(p => p.Calculate)
        //                    .FirstOrDefault()
        //                +
        //                tpoFeeHdrPlan.TpoFeeCalculationPlans.Where(
        //                    p =>
        //                        p.ProductionFeeType ==
        //                        EnumHelper.GetDescription(Enums.ProductionFeeType.MaklonFeeTwoPercent))
        //                    .Select(p => p.Calculate)
        //                    .FirstOrDefault() + "",
        //                "",
        //                "", "", "", "", "", "",
        //                "",
        //                mstGenBrandGroup.SKTBrandCode + " - " + tpoFeeHdrPlan.LocationCode + " " +
        //                mstGenWeek.StartDate.Value.Day + "." + mstGenWeek.StartDate.Value.Month + "." +
        //                mstGenWeek.StartDate.Value.Year.ToString().Substring(2, 2) + "-" +
        //                mstGenWeek.EndDate.Value.Day + "." + mstGenWeek.EndDate.Value.Month + "." +
        //                mstGenWeek.EndDate.Value.Year.ToString().Substring(2, 2) + " (w." + tpoFeeHdrPlan.KPSWeek + ")"
        //                , "", "", "",
        //                "", "", "", "", "", "",
        //                "", "", "", "", "",
        //                "", "", "", "", "", "",
        //                "", "", "", "",
        //                "", "", "", "", "",
        //                ""
        //            };

        //                    string[] content3 =
        //            {
        //                "2", "", "", "", "", ""
        //                , "",
        //                "", "",
        //                "",
        //                "",
        //                "", "40", "",
        //                tpoFeeHdrPlan.TpoFeeCalculationPlans.Where(
        //                    p =>
        //                        p.ProductionFeeType == EnumHelper.GetDescription(Enums.ProductionFeeType.ProductionCost))
        //                    .Select(p => p.Calculate)
        //                    .FirstOrDefault()
        //                    .ToString(),
        //                "",
        //                "", "V4", "", "", "", "",
        //                "",
        //                mstGenBrandGroup.SKTBrandCode + " - " + tpoFeeHdrPlan.LocationCode + " " +
        //                mstGenWeek.StartDate.Value.Day + "." + mstGenWeek.StartDate.Value.Month + "." +
        //                mstGenWeek.StartDate.Value.Year.ToString().Substring(2, 2) + "-" +
        //                mstGenWeek.EndDate.Value.Day + "." + mstGenWeek.EndDate.Value.Month + "." +
        //                mstGenWeek.EndDate.Value.Year.ToString().Substring(2, 2) + " (w." + tpoFeeHdrPlan.KPSWeek + ")" +
        //                " Prod Fee"
        //                , "", "", "",
        //                "", "", "3066150VR1", "", "", "",
        //                "", "", "", "", "",
        //                "", "", "", "", "", "",
        //                "", "", "", "",
        //                "", "", "", "", "",
        //                ""
        //            };

        //                    string[] content4 =
        //            {
        //                "2", "", "", "", "", ""
        //                , "",
        //                "", "",
        //                "",
        //                "",
        //                "", "40", "",
        //                tpoFeeHdrPlan.TpoFeeCalculationPlans.Where(
        //                    p =>
        //                        p.ProductionFeeType ==
        //                        EnumHelper.GetDescription(Enums.ProductionFeeType.MaklonFeeTwoPercent))
        //                    .Select(p => p.Calculate)
        //                    .FirstOrDefault()
        //                    .ToString(),
        //                "",
        //                "", "V4", "", "", "", "",
        //                "",
        //                mstGenBrandGroup.SKTBrandCode + " - " + tpoFeeHdrPlan.LocationCode + " " +
        //                mstGenWeek.StartDate.Value.Day + "." + mstGenWeek.StartDate.Value.Month + "." +
        //                mstGenWeek.StartDate.Value.Year.ToString().Substring(2, 2) + "-" +
        //                mstGenWeek.EndDate.Value.Day + "." + mstGenWeek.EndDate.Value.Month + "." +
        //                mstGenWeek.EndDate.Value.Year.ToString().Substring(2, 2) + " (w." + tpoFeeHdrPlan.KPSWeek + ")" +
        //                " Mgmt Fee"
        //                , "", "", "",
        //                "", "", "3066150VR1", "", "", "",
        //                "", "", "", "", "",
        //                "", "", "", "", "", "",
        //                "", "", "", "",
        //                "", "", "", "", "",
        //                ""
        //            };

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
        //            }
        //        }
        //    }
        //    return Json(bulkData);
        //}

        [HttpPost]
        public ActionResult GenerateP1(P1Template<TpoFeeExePlanViewModel> bulkData)
        {
            var counter = bulkData.Data.Count(p => p.Checkbox);
            if (bulkData.Data.Any() && counter > 0)
            {
                MemoryStream ms = new MemoryStream();
                List<string> regionalList = new List<string>();

                foreach (var data in bulkData.Data)
                {
                    if (data.Checkbox != false)
                    {
                        var tpoFeeHdrPlanList = _tpoFeeBll.GetTpoFeeHdrPlan(data.TpoFeeCode);
                        var locationObject = _masterDataBll.GetMstGenLocationsByParentCode(new GetMstGenLocationsByParentCodeInput
                        {
                            ParentLocationCode = tpoFeeHdrPlanList.LocationCode
                        }).FirstOrDefault();
                        var regionalObject = _masterDataBll.GetLocation(locationObject == null ? string.Empty : locationObject.ParentLocationCode);

                        var check = regionalList.Where(p => p.Contains(regionalObject.LocationCode)).FirstOrDefault();

                        if (check == null)
                        {
                            regionalList.Add(regionalObject.LocationCode);
                        }
                    }
                }

                if (bulkData.Data.Where(p => p.Checkbox == true).Count() != 0)
                {
                    string regionalCode = "REG";

                    foreach (var data in regionalList)
                    {
                        regionalCode += data.Substring(3, 1);
                    }


                    var tpoFeeHdrPlanObject = _tpoFeeBll.GetTpoFeeHdrPlan(bulkData.Data[0].TpoFeeCode);
                    string fileName = "Upload Plan W" + tpoFeeHdrPlanObject.KPSWeek + " " + regionalCode + ".txt";

                    string path = Server.MapPath("~/Content/P1Template/" + fileName);

                    if (System.IO.File.Exists(path))
                    {
                        System.IO.File.Delete(path);
                    }

                    if (!System.IO.File.Exists(path))
                    {


                        for (int i = 0; i < bulkData.Data.Count; i++)
                        {

                            var tpoFeeHdrPlan = _tpoFeeBll.GetTpoFeeHdrPlan(bulkData.Data[i].TpoFeeCode);
                            var mstGenBrandGroup = _masterDataBll.GetBrandGroupById(tpoFeeHdrPlan.BrandGroupCode);
                            var mstGenWeek = _masterDataBll.GetWeekByYearAndWeek(tpoFeeHdrPlan.KPSYear, tpoFeeHdrPlan.KPSWeek);

                            // Create a file to write to.
                            using (StreamWriter sw = System.IO.File.CreateText(path))
                            {
                                string[] title =
                    {
                        "Type", "Company", "Currency", "Exchange rate", "Document type", "Translation date"
                        , "Header text", "Reference", "CC transaction", "Document date", "Posting date",
                        "Automatic tax", "Posting key", "Account", "Doc cur amount", "Local cur amount",
                        "Local currency", "Tax code", "PO Number", "PO Item Number", "Quantity", "UOM",
                        "Assignment", "Text", "Special GL Indicator", "Recovery Indicator", "Customer",
                        "Baseline date", "Value date", "Cost center", "WBS", "Material number", "Brand family",
                        "Payment terms", "Cash discount 1", "Trading partner", "New company", "Interco affiliate",
                        "Production center", "PMI Market", "Product category", "Ship-to", "Label", "Final market",
                        "DocNumber-Earmarked Funds", "Earmarked Funds", "Tax Base Amount", "Withholding Tax Base Amount",
                        "Batch number", "Business Place", "Section Code", "Amount in 2nd local currency",
                        "Amount in 3rd local currency",
                        "W. Tax Code"
                    };

                                string[] content1 =
                    {
                        "1", "3066", "IDR", "", "KR", ""
                        , tpoFeeHdrPlan.LocationCode + " w." + tpoFeeHdrPlan.KPSWeek + " " + tpoFeeHdrPlan.KPSYear,
                        mstGenBrandGroup.SKTBrandCode, "",
                        DateTime.Now.Day + "." + DateTime.Now.Month + "." + DateTime.Now.Year.ToString().Substring(2, 2),
                        DateTime.Now.Day + "." + DateTime.Now.Month + "." + DateTime.Now.Year.ToString().Substring(2, 2),
                        "X", "", "", "", "",
                        "", "", "", "", "", "",
                        "", "", "", "", "",
                        "", "", "", "", "", "",
                        "", "", "", "", "",
                        "", "", "", "", "", "",
                        "", "", "", "",
                        "", "", "", "", "",
                        ""
                    };

                                string[] content2 =
                    {
                        "2", "", "", "", "", ""
                        , "",
                        "", "",
                        "",
                        "",
                        "", "31", "",
                        tpoFeeHdrPlan.TpoFeeCalculationPlans.Where(
                            p =>
                                p.ProductionFeeType == EnumHelper.GetDescription(Enums.ProductionFeeType.ProductionCost))
                            .Select(p => p.Calculate)
                            .FirstOrDefault()
                        +
                        tpoFeeHdrPlan.TpoFeeCalculationPlans.Where(
                            p =>
                                p.ProductionFeeType ==
                                EnumHelper.GetDescription(Enums.ProductionFeeType.MaklonFeeTwoPercent))
                            .Select(p => p.Calculate)
                            .FirstOrDefault() + "",
                        "",
                        "", "", "", "", "", "",
                        "",
                        mstGenBrandGroup.SKTBrandCode + " - " + tpoFeeHdrPlan.LocationCode + " " +
                        mstGenWeek.StartDate.Value.Day + "." + mstGenWeek.StartDate.Value.Month + "." +
                        mstGenWeek.StartDate.Value.Year.ToString().Substring(2, 2) + "-" +
                        mstGenWeek.EndDate.Value.Day + "." + mstGenWeek.EndDate.Value.Month + "." +
                        mstGenWeek.EndDate.Value.Year.ToString().Substring(2, 2) + " (w." + tpoFeeHdrPlan.KPSWeek + ")"
                        , "", "", "",
                        "", "", "", "", "", "",
                        "", "", "", "", "",
                        "", "", "", "", "", "",
                        "", "", "", "",
                        "", "", "", "", "",
                        ""
                    };

                                string[] content3 =
                    {
                        "2", "", "", "", "", ""
                        , "",
                        "", "",
                        "",
                        "",
                        "", "40", "",
                        tpoFeeHdrPlan.TpoFeeCalculationPlans.Where(
                            p =>
                                p.ProductionFeeType == EnumHelper.GetDescription(Enums.ProductionFeeType.ProductionCost))
                            .Select(p => p.Calculate)
                            .FirstOrDefault()
                            .ToString(),
                        "",
                        "", "V4", "", "", "", "",
                        "",
                        mstGenBrandGroup.SKTBrandCode + " - " + tpoFeeHdrPlan.LocationCode + " " +
                        mstGenWeek.StartDate.Value.Day + "." + mstGenWeek.StartDate.Value.Month + "." +
                        mstGenWeek.StartDate.Value.Year.ToString().Substring(2, 2) + "-" +
                        mstGenWeek.EndDate.Value.Day + "." + mstGenWeek.EndDate.Value.Month + "." +
                        mstGenWeek.EndDate.Value.Year.ToString().Substring(2, 2) + " (w." + tpoFeeHdrPlan.KPSWeek + ")" +
                        " Prod Fee"
                        , "", "", "",
                        "", "", "3066150VR1", "", "", "",
                        "", "", "", "", "",
                        "", "", "", "", "", "",
                        "", "", "", "",
                        "", "", "", "", "",
                        ""
                    };

                                string[] content4 =
                    {
                        "2", "", "", "", "", ""
                        , "",
                        "", "",
                        "",
                        "",
                        "", "40", "",
                        tpoFeeHdrPlan.TpoFeeCalculationPlans.Where(
                            p =>
                                p.ProductionFeeType ==
                                EnumHelper.GetDescription(Enums.ProductionFeeType.MaklonFeeTwoPercent))
                            .Select(p => p.Calculate)
                            .FirstOrDefault()
                            .ToString(),
                        "",
                        "", "V4", "", "", "", "",
                        "",
                        mstGenBrandGroup.SKTBrandCode + " - " + tpoFeeHdrPlan.LocationCode + " " +
                        mstGenWeek.StartDate.Value.Day + "." + mstGenWeek.StartDate.Value.Month + "." +
                        mstGenWeek.StartDate.Value.Year.ToString().Substring(2, 2) + "-" +
                        mstGenWeek.EndDate.Value.Day + "." + mstGenWeek.EndDate.Value.Month + "." +
                        mstGenWeek.EndDate.Value.Year.ToString().Substring(2, 2) + " (w." + tpoFeeHdrPlan.KPSWeek + ")" +
                        " Mgmt Fee"
                        , "", "", "",
                        "", "", "3066150VR1", "", "", "",
                        "", "", "", "", "",
                        "", "", "", "", "", "",
                        "", "", "", "",
                        "", "", "", "", "",
                        ""
                    };

                                string write = string.Empty;

                                foreach (var data in title)
                                {
                                    write += data + "\t";
                                }

                                sw.WriteLine(write);

                                write = string.Empty;

                                foreach (var data in content1)
                                {
                                    write += data + "\t";
                                }

                                sw.WriteLine(write);

                                write = string.Empty;

                                foreach (var data in content2)
                                {
                                    write += data + "\t";
                                }

                                sw.WriteLine(write);

                                write = string.Empty;

                                foreach (var data in content3)
                                {
                                    write += data + "\t";
                                }

                                sw.WriteLine(write);

                                write = string.Empty;

                                foreach (var data in content4)
                                {
                                    write += data + "\t";
                                }

                                sw.WriteLine(write + System.Environment.NewLine);

                            }
                        }
                    }
                    return File(path, "text/plain", fileName);
                }
            }
            return Content("No data");
        }

        [HttpPost]
        public FileStreamResult GenerateExcel(GetTPOFeeHdrPlanInput input)
        {

            var tpoFeePlans = _tpoFeeBll.GetTpoFeePlanView(input);

            var ms = new MemoryStream();

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var allLocations = _applicationService.GetLocationCodeCompat();
            string locationCompat = "";
            foreach (SelectListItem item in allLocations)
            {
                if (item.Value == input.ParentLocationCode)
                {
                    locationCompat = item.Text;
                }
            }

            var templateFile = Enums.ExcelTemplate.TpoFeeExePlan + ".xlsx";
            var templateFileName = Server.MapPath(Constants.TPOFeeExcelTemplatesFolder + templateFile);
            if (System.IO.File.Exists(templateFileName))
            {
                System.IO.File.Copy(templateFileName, strFileName);
            }

            using (SLDocument slDoc = new SLDocument(strFileName))
            {
                //row values
                slDoc.SetCellValue(3, 2, locationCompat);
                slDoc.SetCellValue(3, 5, input.KpsYear.ToString());
                slDoc.SetCellValue(4, 5, input.KpsWeek.ToString());

                var iRow = 8;

                foreach (var item in tpoFeePlans.Select((value, index) => new { Value = value, Index = index }))
                {
                    //get default style
                    var style = ExcelHelper.GetDefaultExcelStyle(slDoc);
                    if (item.Index % 2 == 0)
                    {
                        style.Fill.SetPattern(PatternValues.Solid, Color.LightGray, Color.LightGray);
                    }
                    
                    slDoc.SetCellValue(iRow, 1, item.Value.LocationCode);
                    slDoc.SetCellValue(iRow, 2, item.Value.LocationName);
                    slDoc.SetCellValue(iRow, 3, item.Value.SktBrandCode);
                    slDoc.SetCellValue(iRow, 4, string.Empty);
                    slDoc.SetCellValue(iRow, 5, String.Format(CultureInfo.CurrentCulture, "{0:n0}", item.Value.JknBox));
                    slDoc.SetCellValue(iRow, 6, String.Format(CultureInfo.CurrentCulture, "{0:n0}", item.Value.Jl1Box));
                    slDoc.SetCellValue(iRow, 7, String.Format(CultureInfo.CurrentCulture, "{0:n0}", item.Value.Jl2Box));
                    slDoc.SetCellValue(iRow, 8, String.Format(CultureInfo.CurrentCulture, "{0:n0}", item.Value.Jl3Box));
                    slDoc.SetCellValue(iRow, 9, String.Format(CultureInfo.CurrentCulture, "{0:n0}", item.Value.Jl4Box));
                    slDoc.SetCellValue(iRow, 10, String.Format(CultureInfo.CurrentCulture, "{0:n0}", item.Value.ProductionCost));
                    slDoc.SetCellValue(iRow, 11, String.Format(CultureInfo.CurrentCulture, "{0:n0}", item.Value.MaklonFee));
                    slDoc.SetCellValue(iRow, 12, String.Format(CultureInfo.CurrentCulture, "{0:n0}", item.Value.ProductivityIncentives));
                    slDoc.SetCellValue(iRow, 13, String.Format(CultureInfo.CurrentCulture, "{0:n0}", item.Value.MaklonFeeTwoPercent));
                    slDoc.SetCellValue(iRow, 14, String.Format(CultureInfo.CurrentCulture, "{0:n0}", item.Value.ProductivityIncentivesTwoPercent));
                    slDoc.SetCellValue(iRow, 15, String.Format(CultureInfo.CurrentCulture, "{0:n0}", item.Value.ProductionCostTenPercent));
                    slDoc.SetCellValue(iRow, 16, String.Format(CultureInfo.CurrentCulture, "{0:n0}", item.Value.MaklonFeeTenPercent));
                    slDoc.SetCellValue(iRow, 17, String.Format(CultureInfo.CurrentCulture, "{0:n0}", item.Value.ProductivityIncentivesTenPercent));
                    slDoc.SetCellValue(iRow, 18, String.Format(CultureInfo.CurrentCulture, "{0:n0}", item.Value.TotalCost));

                    //set style
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
                //slDoc.AutoFitColumn(2, 7);

                System.IO.File.Delete(strFileName);
                slDoc.SaveAs(ms);
            }
            // this is important. Otherwise you get an empty file
            // (because you'd be at EOF after the stream is written to, I think...).
            ms.Position = 0;
            var fileName = "TpoFeeExePlan_" + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}