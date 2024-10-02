using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using SKTISWebsite.Code;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.MasterGenLocations;
using SKTISWebsite.Models.MstTPOFeeRate;
using SKTISWebsite.Models.MstTPOPackage;
using SpreadsheetLight;
using Enums = HMS.SKTIS.Core.Enums;

namespace SKTISWebsite.Controllers
{
    public class MasterTpoRateController : BaseController
    {
        private IMasterDataBLL _masterDataBLL;
        private IApplicationService _applicationService;

        public MasterTpoRateController(IMasterDataBLL masterDataBll, IApplicationService applicationService)
        {
            _masterDataBLL = masterDataBll;
            _applicationService = applicationService;
            SetPage("MasterData/TPO/TPOFeeRate");
        }

        // GET: MasterTpoRate
        public ActionResult Index()
        {
            var listYear = (Enumerable.Range(Convert.ToInt32(Enums.Years.Start), Convert.ToInt32(Enums.Years.End))).Select(fi => fi).ToList();

            var initTpoPackage = new InitMstTPOPackageViewModel()
            {
                Locations = Mapper.Map<List<SelectListItem>>(_masterDataBLL.GetMasterTPOInfos(new GetMasterTPOInfoInput())),
                LocationDescs = Mapper.Map<List<MasterGenLocationDesc>>(_masterDataBLL.GetMstLocationLists(new GetMstGenLocationInput())),
                Years = _applicationService.GetGenWeekYears().ToList(),
                UploadPath = new Uri(Request.Url, Url.Content("~/Upload/MasterTpoRate/")).AbsoluteUri
            };
            return View("Index", initTpoPackage);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetMstTPOFeeRate(MstTPOFeeRateInput criteria)
        {
            var TPOFeeRate = _masterDataBLL.GetTPOFeeRate(criteria);
            var viewModel = Mapper.Map<List<MstTPOFeeRateViewModel>>(TPOFeeRate);
            var pageResult = new PageResult<MstTPOFeeRateViewModel>(viewModel, criteria);
            return Json(pageResult);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bulkData"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveTPOFeeRate(InsertUpdateData<MstTPOFeeRateViewModel> bulkData)
        {
            var locationCode = bulkData.Parameters != null ? bulkData.Parameters["LocationCode"] : "";
            var brandGrupCode = bulkData.Parameters != null ? bulkData.Parameters["BrandGroupCode"] : "";
            if (bulkData.New != null)
            {
                for (int i = 0; i < bulkData.New.Count; i++)
                {
                    if (bulkData.New[i] == null) continue;
                    var feeRate = Mapper.Map<MstTPOFeeRateDTO>(bulkData.New[i]);

                    //set ceatedby and updatedby
                    feeRate.CreatedBy = GetUserName();
                    feeRate.UpdatedBy = GetUserName();

                    // set location code and BrandGroupCode
                    feeRate.LocationCode = locationCode;
                    feeRate.BrandGroupCode = brandGrupCode;

                    if (feeRate.EffectiveDate == DateTime.MinValue)
                        feeRate.EffectiveDate = DateTime.Now.Date;
                    if (feeRate.ExpiredDate == DateTime.MinValue)
                        feeRate.ExpiredDate = DateTime.Now.Date;

                    try
                    {
                        var item = _masterDataBLL.InsertTPOFeeRate(feeRate);
                        bulkData.New[i] = Mapper.Map<MstTPOFeeRateViewModel>(item);
                        bulkData.New[i].ResponseType = Enums.ResponseType.Success.ToString();
                    }
                    catch (ExceptionBase ex)
                    {
                        bulkData.New[i].ResponseType = Enums.ResponseType.Error.ToString();
                        bulkData.New[i].Message = ex.Message;
                    }
                    catch (Exception ex)
                    {
                        bulkData.New[i].ResponseType = Enums.ResponseType.Error.ToString();
                        bulkData.New[i].Message = EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception); ;
                    }
                }
            }
            // Update data
            if (bulkData.Edit != null)
                for (var i = 0; i < bulkData.Edit.Count; i++)
                {
                    //check row is null
                    if (bulkData.Edit[i] == null) continue;

                    var tpoFeeRateDto = Mapper.Map<MstTPOFeeRateDTO>(bulkData.Edit[i]);
                    tpoFeeRateDto.UpdatedBy = GetUserName();
                    try
                    {
                        var item = _masterDataBLL.UpdateTPOFeeRate(tpoFeeRateDto);

                        bulkData.Edit[i] = Mapper.Map<MstTPOFeeRateViewModel>(item);
                        bulkData.Edit[i].ResponseType = Enums.ResponseType.Success.ToString();
                    }
                    catch (ExceptionBase ex)
                    {
                        bulkData.Edit[i].ResponseType = Enums.ResponseType.Error.ToString();
                        bulkData.Edit[i].Message = ex.Message;
                    }
                    catch (Exception ex)
                    {
                        bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                        bulkData.Edit[i].Message = EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception); ;
                    }
                }

            return Json(bulkData);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="locationCode"></param>
        /// <param name="brandGroupCode"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        [HttpPost]
        public FileStreamResult GenerateExcel(string locationCode,string brandGroupCode, string year)
        {
            var input = new MstTPOFeeRateInput
            {
                LocationCode = locationCode,
                BrandGroupCode = brandGroupCode,
                Year = year
            };

            var tpoFeeRate = _masterDataBLL.GetTPOFeeRate(input);
            var ms = new MemoryStream();
            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = Enums.ExcelTemplate.MstTPOFeeRate + ".xlsx";
            var templateFileName = Server.MapPath(Constants.MasterDataExcelTemplatesFolder + templateFile);
            if (System.IO.File.Exists(templateFileName))
            {
                System.IO.File.Copy(templateFileName, strFileName);
            }

            using (SLDocument slDoc = new SLDocument(strFileName))
            {
                //filter values
                var locationName = _masterDataBLL.GetMstLocationById(locationCode);

                if (locationName != null)
                    slDoc.SetCellValue(2, 2, locationCode+" - "+locationName.LocationName);
                if(brandGroupCode !=null)
                    slDoc.SetCellValue(3, 2, brandGroupCode);
                if (year != null)
                    slDoc.SetCellValue(4, 2, year == "" ? "All" : year);

                //row values
                var iRow = 7;

                foreach (var rate in tpoFeeRate.Select((value, index) => new { Value = value, Index = index }))
                {

                    SLStyle style = slDoc.CreateStyle();
                    style.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Font.FontName = "Calibri";
                    style.Font.FontSize = 10;

                    if (rate.Index % 2 == 0)
                    {
                        style.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);
                    }

                    slDoc.SetCellValue(iRow, 1, rate.Value.EffectiveDate.ToString(Constants.DefaultDateOnlyFormat));
                    slDoc.SetCellValue(iRow, 2, rate.Value.ExpiredDate.ToString(Constants.DefaultDateOnlyFormat));
                    slDoc.SetCellValue(iRow, 3, rate.Value.JKN);
                    slDoc.SetCellValue(iRow, 4, rate.Value.Jl1);
                    slDoc.SetCellValue(iRow, 5, rate.Value.Jl2);
                    slDoc.SetCellValue(iRow, 6, rate.Value.Jl3);
                    slDoc.SetCellValue(iRow, 7, rate.Value.Jl4);
                    slDoc.SetCellValue(iRow, 8, rate.Value.ManagementFee);
                    slDoc.SetCellValue(iRow, 9, rate.Value.ProductivityIncentives);
                    slDoc.SetCellValue(iRow, 10, rate.Value.MemoRef);
                    slDoc.SetCellValue(iRow, 11, rate.Value.MemoPath);
                    slDoc.SetCellValue(iRow, 12, rate.Value.Remark);
                    slDoc.SetCellValue(iRow, 13, rate.Value.UpdatedBy);
                    slDoc.SetCellValue(iRow, 14, rate.Value.UpdatedDate.ToString(Constants.DefaultDateFormat));
                    slDoc.SetCellStyle(iRow, 1, iRow, 14, style);

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
                slDoc.AutoFitColumn(1, 14);

                System.IO.File.Delete(strFileName);
                slDoc.SaveAs(ms);
            }
            // this is important. Otherwise you get an empty file
            // (because you'd be at EOF after the stream is written to, I think...).
            ms.Position = 0;
            var fileName = "MasterTPORate_" + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        public JsonResult GetBrandGroupByLocationCode(string locationCode)
        {
            //var data = _masterDataBll.GetMasterProcessSettingLocationDto(locationCode);
            var brandGroupCodes = _applicationService.GetBrandGroupCodeSelectListByParentLocationCode(locationCode);
            return Json(brandGroupCodes, JsonRequestBehavior.AllowGet);
        }
    }
}