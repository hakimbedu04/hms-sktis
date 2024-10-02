using System;
using System.Threading;
using System.Globalization;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using SKTISWebsite.Helper;
using SKTISWebsite.Models.TPOFeeExePlanDetail;
using SpreadsheetLight;
using Color = System.Drawing.Color;

namespace SKTISWebsite.Controllers
{
    public class TPOFeeExePlanDetailController : BaseController
    {
        private ITPOFeeBLL _tpoFeeBll;
        private IMasterDataBLL _masterDataBll;

        public TPOFeeExePlanDetailController(ITPOFeeBLL tpoFeeBll, IMasterDataBLL masterDataBll)
        {
            _tpoFeeBll = tpoFeeBll;
            _masterDataBll = masterDataBll;
            SetPage("TPOFee/Execution/TPOFeePlan");
        }

        [HttpPost]
        public ActionResult GenerateP1(string id)
        {
            MemoryStream ms = new MemoryStream();
            var tpoFeeHdrPlan = _tpoFeeBll.GetTpoFeeHdrPlan(id.Replace("_", "/"));
            var location = _masterDataBll.GetMstGenLocationsByParentCode(new GetMstGenLocationsByParentCodeInput
            {
                ParentLocationCode = tpoFeeHdrPlan.LocationCode
            }).FirstOrDefault();
            var regional = _masterDataBll.GetLocation(location == null ? string.Empty : location.ParentLocationCode);
            var mstGenBrandGroup = _masterDataBll.GetBrandGroupById(tpoFeeHdrPlan.BrandGroupCode);
            var mstGenWeek = _masterDataBll.GetWeekByYearAndWeek(tpoFeeHdrPlan.KPSYear, tpoFeeHdrPlan.KPSWeek);

            string fileName = "Upload Plan W" + tpoFeeHdrPlan.KPSWeek + " " + regional.LocationCode + " " + tpoFeeHdrPlan.LocationCode + ".txt";

            string path = Server.MapPath("~/Content/P1Template/" + fileName);

            if (System.IO.File.Exists(path))
            {
                System.IO.File.Delete(path);
            }

            if (!System.IO.File.Exists(path))
            {
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
                        "Batch number", "Business Place", "Section Code", "Amount in 2nd local currency", "Amount in 3rd local currency",
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
                        tpoFeeHdrPlan.TpoFeeCalculationPlans.Where(p => p.ProductionFeeType == EnumHelper.GetDescription(Enums.ProductionFeeType.ProductionCost)).Select(p => p.Calculate).FirstOrDefault() 
                        + tpoFeeHdrPlan.TpoFeeCalculationPlans.Where(p => p.ProductionFeeType == EnumHelper.GetDescription(Enums.ProductionFeeType.MaklonFeeTwoPercent)).Select(p => p.Calculate).FirstOrDefault() + "",
                        "",
                        "", "", "", "", "", "",
                        "", 
                        mstGenBrandGroup.SKTBrandCode + " - " + tpoFeeHdrPlan.LocationCode + " " +
                        mstGenWeek.StartDate.Value.Day + "." + mstGenWeek.StartDate.Value.Month + "." + mstGenWeek.StartDate.Value.Year.ToString().Substring(2, 2) + "-" +
                        mstGenWeek.EndDate.Value.Day + "." + mstGenWeek.EndDate.Value.Month + "." + mstGenWeek.EndDate.Value.Year.ToString().Substring(2, 2) + " (w." + tpoFeeHdrPlan.KPSWeek + ")"
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
                        tpoFeeHdrPlan.TpoFeeCalculationPlans.Where(p => p.ProductionFeeType == EnumHelper.GetDescription(Enums.ProductionFeeType.ProductionCost)).Select(p => p.Calculate).FirstOrDefault().ToString(),
                        "",
                        "", "V4", "", "", "", "",
                        "", 
                        mstGenBrandGroup.SKTBrandCode + " - " + tpoFeeHdrPlan.LocationCode + " " +
                        mstGenWeek.StartDate.Value.Day + "." + mstGenWeek.StartDate.Value.Month + "." + mstGenWeek.StartDate.Value.Year.ToString().Substring(2, 2) + "-" +
                        mstGenWeek.EndDate.Value.Day + "." + mstGenWeek.EndDate.Value.Month + "." + mstGenWeek.EndDate.Value.Year.ToString().Substring(2, 2) + " (w." + tpoFeeHdrPlan.KPSWeek + ")" + " Prod Fee"
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
                        tpoFeeHdrPlan.TpoFeeCalculationPlans.Where(p => p.ProductionFeeType == EnumHelper.GetDescription(Enums.ProductionFeeType.MaklonFeeTwoPercent)).Select(p => p.Calculate).FirstOrDefault().ToString(),
                        "",
                        "", "V4", "", "", "", "",
                        "", 
                        mstGenBrandGroup.SKTBrandCode + " - " + tpoFeeHdrPlan.LocationCode + " " +
                        mstGenWeek.StartDate.Value.Day + "." + mstGenWeek.StartDate.Value.Month + "." + mstGenWeek.StartDate.Value.Year.ToString().Substring(2, 2) + "-" +
                        mstGenWeek.EndDate.Value.Day + "." + mstGenWeek.EndDate.Value.Month + "." + mstGenWeek.EndDate.Value.Year.ToString().Substring(2, 2) + " (w." + tpoFeeHdrPlan.KPSWeek + ")" + " Mgmt Fee"
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

                    sw.WriteLine(write);

                }
            }

            //TempData["Message"] = "Upload P1 Template Successful";
            //return RedirectToAction("Index", "TPOFeeExePlanDetail", new { id = id });
            return File(path, "text/plain", fileName);
        }

        [HttpPost]
        public FileStreamResult GenerateExcel(string id, string reg, string regName)
        {
            var model = CreateInitTPOFeeExePlanDetailViewModel(id);

            var ms = new MemoryStream();

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }
            var templateFile = "";
            if (model.Calculations.Count() == 13)
            {
                templateFile = Enums.ExcelTemplate.TpoFeeExePlanDetail + ".xlsx";
            }
            else if (model.Calculations.Count() == 14)
            {
                templateFile = Enums.ExcelTemplate.TpoFeeExePlanDetail + "2.xlsx";
            }
            else
            {
                templateFile = Enums.ExcelTemplate.TpoFeeExePlanDetail + "3.xlsx";
            }
            
            var templateFileName = Server.MapPath(Constants.TPOFeeExcelTemplatesFolder + templateFile);
            if (System.IO.File.Exists(templateFileName))
            {
                System.IO.File.Copy(templateFileName, strFileName);
            }

            using (SLDocument slDoc = new SLDocument(strFileName))
            {
                //row values
                slDoc.SetCellValue(3, 2, reg);
                slDoc.SetCellValue(3, 3, regName);
                slDoc.SetCellValue(4, 2, model.Location);
                slDoc.SetCellValue(4, 3, model.LocationName);
                slDoc.SetCellValue(5, 2, model.CostCenter);
                slDoc.SetCellValue(6, 2, model.Brand);
                slDoc.SetCellValue(6, 4, model.StickPerBox.ToString());
                slDoc.SetCellValue(3, 6, model.KpsYear.ToString());
                slDoc.SetCellValue(4, 6, model.KpsWeek.ToString());
                slDoc.SetCellValue(6, 6, model.Paket.ToString());

                var iRow = 10;

                foreach (var item in model.TpoFeeProductionDailyPlanModels.Select((value, index) => new { Value = value, Index = index }))
                {
                    //get default style
                    var style = ExcelHelper.GetDefaultExcelStyle(slDoc);
                    if (item.Index % 2 == 0)
                    {
                        style.Fill.SetPattern(PatternValues.Solid, Color.LightGray, Color.LightGray);
                    }
                    slDoc.SetCellValue(iRow, 1, item.Value.Hari);
                    slDoc.SetCellValue(iRow, 2, item.Value.FeeDate.ToString());
                    slDoc.SetCellValue(iRow, 3, item.Value.OuputSticks.ToString());
                    slDoc.SetCellValue(iRow, 4, item.Value.OutputBox.ToString());
                    slDoc.SetCellValue(iRow, 5, item.Value.JKN.ToString());
                    slDoc.SetCellValue(iRow, 6, item.Value.JL1.ToString());
                    slDoc.SetCellValue(iRow, 7, item.Value.Jl2.ToString());
                    //set style
                    slDoc.SetCellStyle(iRow, 1, iRow, 7, style);
                    iRow++;
                }

                slDoc.SetCellValue(17, 3, model.TotalProductionStick.ToString());
                slDoc.SetCellValue(17, 4, model.TotalProductionBox.ToString());
                slDoc.SetCellValue(17, 5, model.TotalProductionJkn.ToString());
                slDoc.SetCellValue(17, 6, model.TotalProductionJl1.ToString());
                slDoc.SetCellValue(17, 7, model.TotalProductionJl2.ToString());

                slDoc.SetCellValue(18, 5, model.TotalDibayarJKN.ToString());
                slDoc.SetCellValue(18, 6, model.TotalDibayarJL1.ToString());
                slDoc.SetCellValue(18, 7, model.TotalDibayarJL2.ToString());

                var iRowC = 23;
                var ci = CultureInfo.CurrentCulture;
                ci.NumberFormat.CurrencySymbol = "";
                Thread.CurrentThread.CurrentCulture = ci;

                foreach (var calc in model.Calculations.Select((value, index) => new { ValueCalc = value, IndexCalc = index }))
                {
                    var style = ExcelHelper.GetDefaultExcelStyle(slDoc);
                    if (calc.IndexCalc % 2 == 0)
                    {
                        style.Fill.SetPattern(PatternValues.Solid, Color.LightGray, Color.LightGray);
                    }
                    
                    slDoc.SetCellValue(iRowC, 3, calc.ValueCalc.OutputProduction.HasValue == false ? "" : (calc.ValueCalc.OutputProduction.Value.ToString()));
                    slDoc.SetCellValue(iRowC, 4, calc.ValueCalc.OutputBiaya.HasValue == false ? "0.00" : (calc.ValueCalc.OutputBiaya.Value.ToString("C2")));
                    slDoc.SetCellValue(iRowC, 6, calc.ValueCalc.Calculate.HasValue == false ? "0.00" : (calc.ValueCalc.Calculate.Value.ToString("C2")));

                    //set style
                    slDoc.SetCellStyle(iRowC, 1, iRowC, 7, style);
                    iRowC++;
                }

                //slDoc.SetCellValue(21, 4, model.JknOutputProduksiYangDibayar.ToString());
                //slDoc.SetCellValue(21, 6, model.JknBiayaProduksiPerBox.ToString());
                //slDoc.SetCellValue(21, 8, model.JknTotalBiayaProduksiPerBox.ToString());

                //slDoc.SetCellValue(22, 4, model.Jl1OutputProduksiYangDibayar.ToString());
                //slDoc.SetCellValue(22, 6, model.Jl1BiayaProduksiPerBox.ToString());
                //slDoc.SetCellValue(22, 8, model.Jl1TotalBiayaProduksiPerBox.ToString());

                //slDoc.SetCellValue(23, 4, model.Jl2OutputProduksiYangDibayar.ToString());
                //slDoc.SetCellValue(23, 6, model.Jl2BiayaProduksiPerBox.ToString());
                //slDoc.SetCellValue(23, 8, model.Jl2TotalBiayaProduksiPerBox.ToString());

                //slDoc.SetCellValue(24, 4, model.Jl3OutputProduksiYangDibayar.ToString());
                //slDoc.SetCellValue(24, 6, model.Jl3BiayaProduksiPerBox.ToString());
                //slDoc.SetCellValue(24, 8, model.Jl3TotalBiayaProduksiPerBox.ToString());

                //slDoc.SetCellValue(25, 4, model.Jl4OutputProduksiYangDibayar.ToString());
                //slDoc.SetCellValue(25, 6, model.Jl4BiayaProduksiPerBox.ToString());
                //slDoc.SetCellValue(25, 8, model.Jl4TotalBiayaProduksiPerBox.ToString());

                //slDoc.SetCellValue(26, 4, model.BiayaProduksiOutputProduksiYangDibayar.ToString());
                //slDoc.SetCellValue(26, 6, model.BiayaProduksiBiayaProduksiPerBox.ToString());
                //slDoc.SetCellValue(26, 8, model.BiayaProduksiTotalBiayaProduksiPerBox.ToString());

                //slDoc.SetCellValue(27, 4, model.JasaManajemenOutputProduksiYangDibayar.ToString());
                //slDoc.SetCellValue(27, 6, model.JasaManajemenBiayaProduksiPerBox.ToString());
                //slDoc.SetCellValue(27, 8, model.JasaManajemenTotalBiayaProduksiPerBox.ToString());

                //slDoc.SetCellValue(28, 4, model.TotalBiayaProduksiDanJasaManagemenOutputProduksiYangDibayar.ToString());
                //slDoc.SetCellValue(28, 6, model.TotalBiayaProduksiDanJasaManagemenBiayaProduksiPerBox.ToString());
                //slDoc.SetCellValue(28, 8, model.TotalBiayaProduksiDanJasaManagemenTotalBiayaProduksiPerBox.ToString());

                //slDoc.SetCellValue(29, 4, model.JasaManajemenDuaPersenOutputProduksiYangDibayar.ToString());
                //slDoc.SetCellValue(29, 6, model.JasaManajemenDuaPersenBiayaProduksiPerBox.ToString());
                //slDoc.SetCellValue(29, 8, model.JasaManajemenDuaPersenTotalBiayaProduksiPerBox.ToString());

                //slDoc.SetCellValue(30, 4, model.TotalCostKeMpsOutputProduksiYangDibayar.ToString());
                //slDoc.SetCellValue(30, 6, model.TotalCostKeMpsBiayaProduksiPerBox.ToString());
                //slDoc.SetCellValue(30, 8, model.TotalCostKeMpsTotalBiayaProduksiPerBox.ToString());

                //slDoc.SetCellValue(31, 4, model.PembayaranOutputProduksiYangDibayar.ToString());
                //slDoc.SetCellValue(31, 6, model.PembayaranBiayaProduksiPerBox.ToString());
                //slDoc.SetCellValue(31, 8, model.PembayaranTotalBiayaProduksiPerBox.ToString());

                //slDoc.SetCellValue(32, 4, model.SisaYangHarusDiBayarOutputProduksiYangDibayar.ToString());
                //slDoc.SetCellValue(32, 6, model.SisaYangHarusDiBayarBiayaProduksiPerBox.ToString());
                //slDoc.SetCellValue(32, 8, model.SisaYangHarusDiBayarTotalBiayaProduksiPerBox.ToString());

                //slDoc.SetCellValue(33, 4, model.PpnBiayaProduksiSepuluhPersenOutputProduksiYangDibayar.ToString());
                //slDoc.SetCellValue(33, 6, model.PpnBiayaProduksiSepuluhPersenBiayaProduksiPerBox.ToString());
                //slDoc.SetCellValue(33, 8, model.PpnBiayaProduksiSepuluhPersenTotalBiayaProduksiPerBox.ToString());

                //slDoc.SetCellValue(34, 4, model.PpnJasaManajemenSepuluhPersenOutputProduksiYangDibayar.ToString());
                //slDoc.SetCellValue(34, 6, model.PpnJasaManajemenSepuluhPersenBiayaProduksiPerBox.ToString());
                //slDoc.SetCellValue(34, 8, model.PpnJasaManajemenSepuluhPersenTotalBiayaProduksiPerBox.ToString());

                //slDoc.SetCellValue(35, 4, model.TotalBayarOutputProduksiYangDibayar.ToString());
                //slDoc.SetCellValue(35, 6, model.TotalBayarBiayaProduksiPerBox.ToString());
                //slDoc.SetCellValue(35, 8, model.TotalBayarTotalBiayaProduksiPerBox.ToString());
                //if (model.Calculations.Count() == 13)
                //{
                //    slDoc.SetCellValue(38, 1, model.VendorName);
                //    slDoc.SetCellValue(39, 1, model.BankAccountNumber);
                //    slDoc.SetCellValue(40, 1, model.BankType);
                //    slDoc.SetCellValue(41, 1, model.BankBranch);

                //    slDoc.SetCellValue(44, 1, model.PreparedBy);
                //    slDoc.SetCellValue(44, 3, model.ApprovedBy);
                //    slDoc.SetCellValue(44, 5, model.AuthorizedBy);
                //}
                //else if (model.Calculations.Count() == 14)
                //{
                //    slDoc.SetCellValue(39, 1, model.VendorName);
                //    slDoc.SetCellValue(40, 1, model.BankAccountNumber);
                //    slDoc.SetCellValue(41, 1, model.BankType);
                //    slDoc.SetCellValue(42, 1, model.BankBranch);

                //    slDoc.SetCellValue(45, 1, model.PreparedBy);
                //    slDoc.SetCellValue(45, 3, model.ApprovedBy);
                //    slDoc.SetCellValue(45, 5, model.AuthorizedBy);
                //}
                //else
                //{
                //    slDoc.SetCellValue(40, 1, model.VendorName);
                //    slDoc.SetCellValue(41, 1, model.BankAccountNumber);
                //    slDoc.SetCellValue(42, 1, model.BankType);
                //    slDoc.SetCellValue(43, 1, model.BankBranch);

                //    slDoc.SetCellValue(46, 1, model.PreparedBy);
                //    slDoc.SetCellValue(46, 3, model.ApprovedBy);
                //    slDoc.SetCellValue(46, 5, model.AuthorizedBy);
                //}
                

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

        // GET: TPOFeeExePlanDetail
        public ActionResult Index(string id)
        {
            var model = CreateInitTPOFeeExePlanDetailViewModel(id);

            ViewBag.Message = TempData["Message"]; ;

            return View("Index", model);
        }

        private InitTPOFeeExePlanDetailViewModel CreateInitTPOFeeExePlanDetailViewModel(string id)
        {
            var tpoFeeHdrPlan = _tpoFeeBll.GetTpoFeeHdrPlan(id.Replace("_", "/"));
            var locationTpo = _masterDataBll.GetLocation(tpoFeeHdrPlan.LocationCode);
            //var location = _masterDataBll.GetMstGenLocationsByParentCode(new GetMstGenLocationsByParentCodeInput
            //{
            //    ParentLocationCode = tpoFeeHdrPlan.LocationCode
            //}).FirstOrDefault();

            //var regional = _masterDataBll.GetLocation(location == null ? string.Empty : location.ParentLocationCode);

            string locationCode = "";
            if (TempData["planRegional"] != null)
                locationCode = TempData["planRegional"].ToString();

            var regional = _masterDataBll.GetLocation(locationTpo.ParentLocationCode);

            var mstGenBrandGroup = _masterDataBll.GetBrandGroupById(tpoFeeHdrPlan.BrandGroupCode);
            var mstGenWeek = _masterDataBll.GetWeekByYearAndWeek(tpoFeeHdrPlan.KPSYear, tpoFeeHdrPlan.KPSWeek);
            var mstTpoPackage = _masterDataBll.GetTpoPackage(new GetMstTPOPackagesInput
            {
                LocationCode = tpoFeeHdrPlan.LocationCode,
                BrandGroupCode = tpoFeeHdrPlan.BrandGroupCode,
                StartDate = mstGenWeek.StartDate,
                EndDate = mstGenWeek.EndDate
            });

            var mstTpoInfo = _masterDataBll.GetMstTpoInfo(tpoFeeHdrPlan.LocationCode);

            var model = new InitTPOFeeExePlanDetailViewModel();

            model.TpoFeeCode = id;
            model.Regional = regional.LocationCode;
            model.RegionalName = regional.LocationName;
            model.Location = locationTpo.LocationCode;
            model.LocationName = locationTpo.LocationName;
            model.CostCenter = locationTpo.CostCenter;
            model.Brand = mstGenBrandGroup.SKTBrandCode;
            model.StickPerBox = mstGenBrandGroup.StickPerBox;
            model.KpsYear = tpoFeeHdrPlan.KPSYear;
            model.KpsWeek = tpoFeeHdrPlan.KPSWeek;
            model.Paket = mstTpoPackage == null ? null : mstTpoPackage.Package;
            model.TpoFeeProductionDailyPlanModels =
                Mapper.Map<List<TpoFeeProductionDailyPlanModel>>(tpoFeeHdrPlan.TpoFeeProductionDailyPlans);
            foreach (var item in model.TpoFeeProductionDailyPlanModels)
            {
                item.JKN = Math.Round(item.JKN.GetValueOrDefault(), 3);
                item.JL1 = Math.Round(item.JL1.GetValueOrDefault(), 3);
                item.Jl2 = Math.Round(item.Jl2.GetValueOrDefault(), 3);
            }
            model.TotalProductionStick = tpoFeeHdrPlan.TpoFeeProductionDailyPlans.Sum(p => p.OuputSticks);
            model.TotalProductionBox = tpoFeeHdrPlan.TpoFeeProductionDailyPlans.Sum(p => p.OutputBox);
            model.TotalProductionJkn = tpoFeeHdrPlan.TpoFeeProductionDailyPlans.Sum(p => p.JKN);
            model.TotalProductionJl1 = tpoFeeHdrPlan.TpoFeeProductionDailyPlans.Sum(p => p.JL1);
            model.TotalProductionJl2 = tpoFeeHdrPlan.TpoFeeProductionDailyPlans.Sum(p => p.Jl2);
            model.TotalProductionJl3 = tpoFeeHdrPlan.TpoFeeProductionDailyPlans.Sum(p => p.Jl3);
            model.TotalProductionJl4 = tpoFeeHdrPlan.TpoFeeProductionDailyPlans.Sum(p => p.Jl4);

            model.TotalDibayarJKN = tpoFeeHdrPlan.TotalProdJKN;
            model.TotalDibayarJL1 = tpoFeeHdrPlan.TotalProdJl1;
            model.TotalDibayarJL2 = tpoFeeHdrPlan.TotalProdJl2;
            model.TotalDibayarJL3 = tpoFeeHdrPlan.TotalProdJl3;
            model.TotalDibayarJL4 = tpoFeeHdrPlan.TotalProdJl4;

            model.VendorName = mstTpoInfo.VendorName;
            model.BankAccountNumber = mstTpoInfo.BankAccountNumber;
            model.BankType = mstTpoInfo.BankType;
            model.BankBranch = mstTpoInfo.BankBranch;
            model.PreparedBy = _tpoFeeBll.GetUserAdByRoleLocation(Enums.Role.SLT.ToString(), regional.LocationCode);
            model.ApprovedBy = _tpoFeeBll.GetUserAdByRoleLocation(Enums.Role.RM.ToString(), regional.LocationCode);
            model.AuthorizedBy = _tpoFeeBll.GetUserAdByRoleLocation(Enums.Role.SKTHD.ToString(), regional.LocationCode);

            model.Calculations = _tpoFeeBll.GetTpoFeeCalculationPlan(id.Replace("_", "/"));

            return model;
        }

        [HttpPost]
        public ActionResult BackToListParent(string regional, int kpsYear, int kpsWeek)
        {
            TempData["planRegional"] = regional;
            TempData["planYear"] = kpsYear;
            TempData["planWeek"] = kpsWeek;

            return RedirectToAction("Index", "TPOFeeExePlan");
        }
    }
}