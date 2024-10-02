using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using SKTISWebsite.Code;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.Factories.Contract;
using SKTISWebsite.Models.MasterGenLocations;
using SpreadsheetLight;
using Enums = HMS.SKTIS.Core.Enums;

namespace SKTISWebsite.Controllers
{
    public class MasterGenLocationController : BaseController
    {
        private AbstractSelectList _abstractSelectList;
        private IMasterDataBLL _masterDataBll;
        private IApplicationService _svc;

        public MasterGenLocationController(AbstractSelectList abstractSelectList, IMasterDataBLL masterDataMasterDataBll, IApplicationService applicationService)
        {
            _masterDataBll = masterDataMasterDataBll;
            _abstractSelectList = abstractSelectList;
            _svc = applicationService;
            SetPage("MasterData/General/Location");
        }

        // GET: MasterLocation
        public ActionResult Index()
        {
            var locationsDefault = _svc.GetPlantAndRegionalLocationLookupList();
            var locations = _masterDataBll.GetLocationCodes();
            var locationCodes = locations.Select(location => new SelectListItem()
            {
                Value = location,
                Text = location
            }).ToList();

            var initMasterLocationItem = new InitMstLocationItem
            {
                LocationLookupList = locationsDefault,
                ItemShift = Mapper.Map<List<SelectListItem>>(_masterDataBll.GetGeneralListShift()),
                ItemLocationCodes = locationCodes,
                ItemTPORanks = Mapper.Map<List<SelectListItem>>(_masterDataBll.GetGeneralListTPORank()),
                LocationDescs = Mapper.Map<List<MasterGenLocationDesc>>(_masterDataBll.GetMstLocationLists(new GetMstGenLocationInput()))
            };
            return View("Index", initMasterLocationItem);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetMasterLocations(GetMstGenLocationsByParentCodeInput criteria)
        {
            var pageResult = new PageResult<MstGenLocationViewModel>();
            var masterLocations = _masterDataBll.GetMstGenLocationsByParentCode(criteria);

            pageResult.TotalRecords = masterLocations.Count;
            pageResult.TotalPages = (masterLocations.Count / criteria.PageSize) + (masterLocations.Count % criteria.PageSize != 0 ? 1 : 0);
            var result = masterLocations.Skip((criteria.PageIndex - 1) * criteria.PageSize).Take(criteria.PageSize);
            pageResult.Results = Mapper.Map<List<MstGenLocationViewModel>>(result);
            return Json(pageResult);
        }

        /// <summary>
        /// Get location 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetListLocation()
        {
            var input = new GetAllLocationsInput();
            var model = _masterDataBll.GetAllLocations(input);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetListLocationTPOInfo()
        {
            var model = _masterDataBll.GetAllLocationCodeInfo();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        ///// <summary>
        ///// Get Shift
        ///// </summary>
        //[HttpGet]
        //public JsonResult GetListShift()
        //{
        //    string[] parameter = { EnumHelper.GetDescription(Enums.MasterGeneralList.Shift) };
        //    _abstractSelectList._DataSource = _masterDataBll.GetGeneralLists(parameter);
        //    return Json(_abstractSelectList.CreateShift(), JsonRequestBehavior.AllowGet);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bulkData"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveLocations(InsertUpdateData<MstGenLocationViewModel> bulkData)
        {
            if (bulkData.New != null)
                for (int i = 0; i < bulkData.New.Count; i++)
                {
                    //check row is null
                    if (bulkData.New[i] == null) continue;
                    var location = Mapper.Map<MstGenLocationDTO>(bulkData.New[i]);

                    // set createdby and updatedby
                    location.CreatedBy = GetUserName();
                    location.UpdatedBy = GetUserName();

                    try
                    {
                        var item = _masterDataBll.InsertLocation(location);
                        bulkData.New[i] = Mapper.Map<MstGenLocationViewModel>(item);
                        bulkData.New[i].ResponseType = Enums.ResponseType.Success.ToString();
                    }
                    catch (BLLException ex)
                    {
                        bulkData.New[i].ResponseType = Enums.ResponseType.Error.ToString();
                        bulkData.New[i].Message = ex.Message;
                    }
                }
            // Update data
            if (bulkData.Edit != null)
                for (var i = 0; i < bulkData.Edit.Count; i++)
                {
                    //check row is null
                    if (bulkData.Edit[i] == null) continue;

                    var location = Mapper.Map<MstGenLocationDTO>(bulkData.Edit[i]);
                    location.UpdatedBy = GetUserName();
                    try
                    {
                        var item = _masterDataBll.UpdateLocation(location);

                        bulkData.Edit[i] = Mapper.Map<MstGenLocationViewModel>(item);
                        bulkData.Edit[i].ResponseType = Enums.ResponseType.Success.ToString();
                    }
                    catch (BLLException ex)
                    {
                        bulkData.Edit[i].ResponseType = Enums.ResponseType.Error.ToString();
                        bulkData.Edit[i].Message = ex.Message;
                    }
                }
            return Json(bulkData);
        }

        /// <summary>
        /// Generate excel by filter
        /// </summary>
        /// <param name="locationCode"></param>
        /// <returns></returns>
        [HttpPost]
        public FileStreamResult GenerateExcel(string locationCodeLocation)
        {
            var input = new GetMstGenLocationsByParentCodeInput() { ParentLocationCode = locationCodeLocation };
            var masterLocations = _masterDataBll.GetMstGenLocationsByParentCode(input);

            var ms = new MemoryStream();

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = Enums.ExcelTemplate.MstLocation + ".xlsx";
            var templateFileName = Server.MapPath(Constants.MasterDataExcelTemplatesFolder + templateFile);
            if (System.IO.File.Exists(templateFileName))
            {
                System.IO.File.Copy(templateFileName, strFileName);
            }

            using (SLDocument slDoc = new SLDocument(strFileName))
            {
                //filter values
                slDoc.SetCellValue(2, 2, locationCodeLocation);

                //row values
                var iRow = 5;

                foreach (var item in masterLocations.Select((value, index) => new { Value = value, Index = index }))
                {
                    SLStyle style = slDoc.CreateStyle();
                    style.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Font.FontName = "Calibri";
                    style.Font.FontSize = 10;

                    if (item.Index % 2 == 0)
                    {
                        style.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);
                    }

                    slDoc.SetCellValue(iRow, 1, item.Value.LocationCode);
                    slDoc.SetCellValue(iRow, 2, item.Value.LocationName);
                    slDoc.SetCellValue(iRow, 3, item.Value.CostCenter);
                    slDoc.SetCellValue(iRow, 4, item.Value.ABBR);
                    slDoc.SetCellValue(iRow, 5, item.Value.Shift);
                    slDoc.SetCellValue(iRow, 6, item.Value.ParentLocationCode);
                    slDoc.SetCellValue(iRow, 7, item.Value.UMK);
                    slDoc.SetCellValue(iRow, 8, item.Value.KPPBC);
                    slDoc.SetCellValue(iRow, 9, item.Value.Address);
                    slDoc.SetCellValue(iRow, 10, item.Value.City);
                    slDoc.SetCellValue(iRow, 11, item.Value.Region);
                    slDoc.SetCellValue(iRow, 12, item.Value.Phone);
                    slDoc.SetCellValue(iRow, 13, item.Value.StatusActive);
                    slDoc.SetCellValue(iRow, 14, item.Value.Remark);
                    slDoc.SetCellValue(iRow, 15, item.Value.UpdatedBy);
                    slDoc.SetCellValue(iRow, 16, item.Value.UpdatedDate.ToString(Constants.DefaultDateFormat));
                    slDoc.SetCellStyle(iRow, 1, iRow, 16, style);

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
                slDoc.AutoFitColumn(1, 16);

                System.IO.File.Delete(strFileName);
                slDoc.SaveAs(ms);
            }
            // this is important. Otherwise you get an empty file
            // (because you'd be at EOF after the stream is written to, I think...).
            ms.Position = 0;
            var fileName = Enums.ExcelTemplate.MstLocation + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}