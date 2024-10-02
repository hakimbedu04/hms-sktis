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
using HMS.SKTIS.Utils;
using SKTISWebsite.Code;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.MstUnit;
using SpreadsheetLight;
using Enums = HMS.SKTIS.Core.Enums;
using SKTISWebsite.Models.MstPlantUnit;

namespace SKTISWebsite.Controllers
{
    public class MasterPlantUnitController : BaseController
    {
        private IMasterDataBLL _bll;
        private IApplicationService _svc;

        public MasterPlantUnitController(IMasterDataBLL bll, IApplicationService svc)
        {
            _bll = bll;
            _svc = svc;
            SetPage("MasterData/Plant/Unit");
        }

        // GET: MasterUnit
        public ActionResult Index()
        {
            var InitMstPlantUnit = new InitMstPlantUnit
            {
                LocationNameLookupList = _svc.GetLocationNamesLookupList()
            };
            return View("Index", InitMstPlantUnit);
        }

        [HttpPost]
        public JsonResult GetUnits(GetMstPlantUnitsInput criteria)
        {
            var pageResult = new PageResult<MstPlantUnitViewModel>();
            var masterUnits = _bll.GetMstPlantUnits(criteria);
            pageResult.TotalRecords = masterUnits.Count;
            pageResult.TotalPages = (masterUnits.Count / criteria.PageSize) + (masterUnits.Count % criteria.PageSize != 0 ? 1 : 0);
            var result = masterUnits.Skip((criteria.PageIndex - 1) * criteria.PageSize).Take(criteria.PageSize);
            pageResult.Results = Mapper.Map<List<MstPlantUnitViewModel>>(result);
            return Json(pageResult);
        }

        /// <summary>
        /// Get all list of locationcode
        /// </summary>
        /// <returns>list of LocationCode</returns>
        [HttpGet]
        public JsonResult GetLocationCodeSelectList()
        {
            var model = _svc.GetDescendantLocationByLocationCode(Enums.LocationCode.PLNT.ToString(), 2);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Save all new and updated general list item
        /// </summary>
        /// <param name="bulkData"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveAllUnits(InsertUpdateData<MstPlantUnitViewModel> bulkData)
        {
            if (bulkData.New != null)
            {
                for (var i = 0; i < bulkData.New.Count; i++)
                {
                    //check row is null
                    if (bulkData.New[i] == null) continue;
                    var mstUnit = Mapper.Map<MstPlantUnitDTO>(bulkData.New[i]);

                    //set createdby and updatedby
                    mstUnit.CreatedBy = GetUserName();
                    mstUnit.UpdatedBy = GetUserName();

                    try
                    {
                        var item = _bll.InsertMstPlantUnit(mstUnit);
                        bulkData.New[i] = Mapper.Map<MstPlantUnitViewModel>(item);
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
            {
                for (var i = 0; i < bulkData.Edit.Count; i++)
                {
                    //check row is null
                    if (bulkData.Edit[i] == null) continue;
                    var mstUnit = Mapper.Map<MstPlantUnitDTO>(bulkData.Edit[i]);

                    //set updatedby
                    mstUnit.UpdatedBy = GetUserName();

                    try
                    {
                        var item = _bll.UpdateMstPlantUnit(mstUnit);
                        bulkData.Edit[i] = Mapper.Map<MstPlantUnitViewModel>(item);
                        bulkData.Edit[i].ResponseType = Enums.ResponseType.Success.ToString();
                    }
                    catch (ExceptionBase ex)
                    {
                        bulkData.Edit[i].ResponseType = Enums.ResponseType.Error.ToString();
                        bulkData.Edit[i].Message = ex.Message;
                    }
                    catch (Exception ex)
                    {
                        bulkData.Edit[i].ResponseType = Enums.ResponseType.Error.ToString();
                        bulkData.Edit[i].Message = EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception); ;
                    }
                }
            }

            return Json(bulkData);
        }


        [HttpPost]
        public FileStreamResult GenerateExcel(string locationCode)
        {
            var input = new GetMstPlantUnitsInput() { LocationCode = locationCode };
            var masterUnits = _bll.GetMstPlantUnits(input);

            MemoryStream ms = new MemoryStream();

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = Enums.ExcelTemplate.MstPlantUnit + ".xlsx";
            var templateFileName = Server.MapPath(Constants.MasterDataExcelTemplatesFolder + templateFile);
            if (System.IO.File.Exists(templateFileName))
            {
                System.IO.File.Copy(templateFileName, strFileName);
            }

            var allLocations = _svc.GetLocationCodeCompat();
            string locationCompat = "";
            foreach (SelectListItem item in allLocations)
            {
                if (item.Value == locationCode)
                {
                    locationCompat = item.Text;
                }
            }

            using (SLDocument slDoc = new SLDocument(strFileName))
            {
                //filter values
                slDoc.SetCellValue(2, 2, locationCompat);

                //row values
                var iRow = 5;

                foreach (var item in masterUnits.Select((value, index) => new { Value = value, Index = index }))
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
                    slDoc.SetCellValue(iRow, 2, item.Value.UnitCode);
                    slDoc.SetCellValue(iRow, 3, item.Value.UnitName);
                    slDoc.SetCellValue(iRow, 4, item.Value.StatusActive.ToString());
                    slDoc.SetCellValue(iRow, 5, item.Value.Remark);
                    slDoc.SetCellValue(iRow, 6, item.Value.UpdatedBy);
                    slDoc.SetCellValue(iRow, 7, item.Value.UpdatedDate.ToString(Constants.DefaultDateFormat));
                    slDoc.SetCellStyle(iRow, 1, iRow, 7, style);

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
                slDoc.AutoFitColumn(1, 7);

                System.IO.File.Delete(strFileName);
                slDoc.SaveAs(ms);
            }
            // this is important. Otherwise you get an empty file
            // (because you'd be at EOF after the stream is written to, I think...).
            ms.Position = 0;
            var fileName = Enums.ExcelTemplate.MstPlantUnit + "_" + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}