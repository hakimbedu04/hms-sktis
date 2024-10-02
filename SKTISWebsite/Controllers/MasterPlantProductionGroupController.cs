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
using SKTISWebsite.Models.MasterPlantProductionGroup;
using SpreadsheetLight;
using Enums = HMS.SKTIS.Core.Enums;

namespace SKTISWebsite.Controllers
{
    public class MasterPlantProductionGroupController : BaseController
    {
        private IMasterDataBLL _bll;
        private IApplicationService _svc;

        /// <summary>
        /// Initializes a new instance of the <see cref="MasterPlantProductionGroupController"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        /// <param name="bll">The BLL.</param>
        /// <param name="svc">The SVC.</param>
        public MasterPlantProductionGroupController(IMasterDataBLL bll, IApplicationService svc)
        {
            _bll = bll;
            _svc = svc;
            SetPage("MasterData/Plant/ProductionGroup");
        }

        /// <summary>
        /// Indexes this instance.
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            var model = new InitMstPlantProductionGroup()
            {
                LocationCodeSelectList = _svc.GetPlantChildLocationCode(),
                //LocationNameLookupList = _svc.GetLocationNamesLookupList(),
                LocationNameLookupList = _svc.GetLastLocationChildList(Enums.LocationCode.PLNT.ToString()),
                LeaderLookupList = _svc.GetAllMandorLookupList(),
                GroupCodeSelectList = _svc.GetGroupCodeSelectList()
            };

            return View(model);

        }

        /// <summary>
        /// Gets the MST plant production group.
        /// </summary>
        /// <param name="criteria">The criteria.</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetMstPlantProductionGroup(GetMstPlantProductionGroupsInput criteria)
        {
            var plantProductionGroups = _bll.GetMstPlantProductionGroups(criteria);
            var viewModel = Mapper.Map<List<MstPlantProductionGroupViewModel>>(plantProductionGroups);
            var pageResult = new PageResult<MstPlantProductionGroupViewModel>(viewModel, criteria);
            return Json(pageResult);
        }


        /// <summary>
        /// Gets the unit code select list.
        /// </summary>
        /// <param name="locationCode">The location code.</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetUnitCodeSelectListByLocationCode(string locationCode)
        {
            var model = _svc.GetPlantUnitCodeSelectListByLocationCode(locationCode);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets the process group select list by location code.
        /// </summary>
        /// <param name="locationCode">The location code.</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetProcessGroupSelectListByLocationCode(string locationCode)
        {
            var model = _svc.GetProcessGroupSelectListByLocationCode(locationCode);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult SaveAllPlantProductionGroup(InsertUpdateData<MstPlantProductionGroupViewModel> bulkData)
        {
            var locationCode = bulkData.Parameters.ContainsKey("LocationCode") ? bulkData.Parameters["LocationCode"] : "";
            var unitCode = bulkData.Parameters.ContainsKey("UnitCode") ? bulkData.Parameters["UnitCode"] : "";
            var processSettingsCode = bulkData.Parameters.ContainsKey("ProcessSettingsCode") ? bulkData.Parameters["ProcessSettingsCode"] : "";

            // save data
            if (bulkData.Edit != null)
            {
                for (var i = 0; i < bulkData.Edit.Count; i++)
                {
                    //check row is null
                    if (bulkData.Edit[i] == null) continue;

                    var plantProductionGroup = Mapper.Map<MstPlantProductionGroupDTO>(bulkData.Edit[i]);
                    plantProductionGroup.LeaderInspection = bulkData.Edit[i].InspectionLeader;
                    plantProductionGroup.LocationCode = locationCode;
                    plantProductionGroup.UnitCode = unitCode;
                    plantProductionGroup.ProcessGroup = processSettingsCode;
                    // line below commented because it will comes from System (SSIS)
                    // plantProductionGroup.CreatedBy = GetUserName();
                    plantProductionGroup.UpdatedBy = GetUserName();
                    try
                    {
                        var item = _bll.SaveMstPlantProductionGroup(plantProductionGroup);

                        bulkData.Edit[i] = Mapper.Map<MstPlantProductionGroupViewModel>(item);
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
                        bulkData.Edit[i].Message =
                            EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception);
                        ;
                    }
                }
            }

            return Json(bulkData);
        }

        /// <summary>
        /// Generates the excel.
        /// </summary>
        /// <param name="locationCode">The location code.</param>
        /// <param name="unit">The unit.</param>
        /// <param name="processSettingsCode">The process.</param>
        /// <returns></returns>
        [HttpPost]
        public FileStreamResult GenerateExcel(string locationCode, string unitCode, string processSettingsCode)
        {
            var input = new GetMstPlantProductionGroupsInput
            {
                LocationCode = locationCode,
                UnitCode = unitCode,
                ProcessSettingsCode = processSettingsCode
            };
            var mstPlantProductionGroups = _bll.GetMstPlantProductionGroups(input);

            var ms = new MemoryStream();

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
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

            var templateFile = Enums.ExcelTemplate.MstPlantProductionGroup + ".xlsx";
            var templateFileName = Server.MapPath(Constants.MasterDataExcelTemplatesFolder + templateFile);
            if (System.IO.File.Exists(templateFileName))
            {
                System.IO.File.Copy(templateFileName, strFileName);
            }

            using (var slDoc = new SLDocument(strFileName))
            {
                slDoc.SetCellValue(2, 2, locationCompat);
                slDoc.SetCellValue(3, 2, unitCode == "" ? "All" : unitCode);
                slDoc.SetCellValue(4, 2, processSettingsCode == "" ? "All" : processSettingsCode);


                //row values
                var iRow = 8;

                foreach (var item in mstPlantProductionGroups.Select((value, index) => new { Value = value, Index = index }))
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
                    slDoc.SetCellValue(iRow, 1, item.Value.GroupCode);
                    slDoc.SetCellValue(iRow, 2, item.Value.Leader1Name);
                    slDoc.SetCellValue(iRow, 3, item.Value.Leader2Name);
                    slDoc.SetCellValue(iRow, 4, item.Value.LeaderInspectionName);
                    slDoc.SetCellValue(iRow, 5, item.Value.WorkerCount.ToString());
                    slDoc.SetCellValue(iRow, 6, item.Value.NextGroupCode);
                    slDoc.SetCellValue(iRow, 7, item.Value.StatusActive.ToString());
                    slDoc.SetCellValue(iRow, 8, item.Value.Remark);
                    slDoc.SetCellValue(iRow, 9, item.Value.UpdatedBy);
                    slDoc.SetCellValue(iRow, 10,
                        item.Value.UpdatedDate.HasValue
                            ? item.Value.UpdatedDate.Value.ToString(Constants.DefaultDateFormat)
                            : string.Empty);
                    slDoc.SetCellStyle(iRow, 1, iRow, 10, style);

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
                //slDoc.AutoFitColumn(1, 10);

                System.IO.File.Delete(strFileName);
                slDoc.SaveAs(ms);
            }
            // this is important. Otherwise you get an empty file
            // (because you'd be at EOF after the stream is written to, I think...).
            ms.Position = 0;
            var fileName = "MasterPlantProductionGroup_" + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}