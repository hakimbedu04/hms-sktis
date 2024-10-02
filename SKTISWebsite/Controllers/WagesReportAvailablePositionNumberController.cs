using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.IO;
using System.Reflection.Emit;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.BusinessObjects;
using HMS.SKTIS.BusinessObjects.DTOs.Execution;
using HMS.SKTIS.BusinessObjects.Inputs.PlantWages;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using Microsoft.Ajax.Utilities;
using SKTISWebsite.Code;
using SKTISWebsite.Models.WagesReportAvailablePositionNumber;
using SKTISWebsite.Models.LookupList;
using SKTISWebsite.Models.Common;
using SpreadsheetLight;
using Color = System.Drawing.Color;
using Enums = HMS.SKTIS.Core.Enums;


namespace SKTISWebsite.Controllers
{
    public class WagesReportAvailablePositionNumberController : BaseController
    {
        private IApplicationService _svc;
        private IMasterDataBLL _masterDataBLL;
        private IPlanningBLL _planningBll;
        private IPlantWagesExecutionBLL _plantWagesExecutionBll;

        public WagesReportAvailablePositionNumberController(IApplicationService applicationService, IMasterDataBLL masterDataBll, IPlanningBLL planningBll, IPlantWagesExecutionBLL plantWagesExecutionBll)
        {
            _svc = applicationService;
            _masterDataBLL = masterDataBll;
            _planningBll = planningBll;
            _plantWagesExecutionBll = plantWagesExecutionBll;
            SetPage("PlantWages/Execution/Report/AvailablePositionNumber");
        }

        public ActionResult Index()
        {
            var maintenanitems = _masterDataBLL.GetAllMaintenanceItems();
            var location = _masterDataBLL.GetLocationInfoParent(Enums.LocationCode.PLNT.ToString()) ?? new List<LocationInfoDTO>();
            var init = new InitWagesReportAvailablePositionNumberViewModel()
            {
                PLNTChildLocation = Mapper.Map<List<SelectListItem>>(location),
                LocationNameLookupList = _svc.GetLocationNamesLookupList(),
                DefaultDate = DateTime.Now.Date.Date 

            };

            return View("Index", init);
        }

        [HttpGet]
        public JsonResult GetUnits(string locationCode)
        {
            var process = _svc.GetPlantUnitCodeSelectListByLocationCode(locationCode);
            return Json(process, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetShifts(string locationCode)
        {
            var process = _svc.GetShiftByLocationCode(locationCode);
            return Json(process, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetProcess(string locationCode)
        {
            var process = _svc.GetProcessGroupSelectListByLocationCode(locationCode);
            return Json(process, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetEmpStatusNested(string locationCode)
        {
            var empStatus = _svc.GetLocStatusByParentLocationCode(locationCode);
            return Json(empStatus, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetGroupAvailabelPositionNumberByLocationUniShiftAndProcess(string locationCode, string unit, string process,string Status)
        {
            var model = _svc.GetGroupAvailabelPositionNumberByLocationUniShiftAndProcess(locationCode, unit, process, Status);
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetAvailablePositionNumberView(WagesReportAvailablePositionNumberInput Criteria)
        {
            var masterLists = _plantWagesExecutionBll.WagesReportAvailablePositionNumber(Criteria);
            var viewModel = Mapper.Map<List<ExeWagesReportAvailablePositionNumberViewModel>>(masterLists);
            var pageResult = new PageResult<ExeWagesReportAvailablePositionNumberViewModel>(viewModel, Criteria);
            return Json(pageResult);
        }

        [HttpPost]
        public FileStreamResult generateExcel(string locationCode,string unitCode,string processGroup, string statusEmp, string groupCode)
        {
            var input = new WagesReportAvailablePositionNumberInput
            {
                LocationCode = locationCode,
                UnitCode = unitCode,
                ProcessGroup = processGroup,
                Status = statusEmp,
                GroupCode = groupCode
            };

            var allLocations = _svc.GetLocationCodeCompat();
            string locationCompat = "";
            foreach (SelectListItem item in allLocations)
            {
                if (item.Value == locationCode)
                {
                    locationCompat = item.Text;
                }
            }

            input.SortExpression = "EmployeeNumber";
            input.SortOrder = "ASC";

            var exeWagesAvailablePositionNumber = _plantWagesExecutionBll.WagesReportAvailablePositionNumber(input);

            var exeWagesAvailablePositionNumberViewModel = Mapper.Map<List<ExeWagesReportAvailablePositionNumberViewModel>>(exeWagesAvailablePositionNumber);

            var ms = new MemoryStream();

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = Enums.PlantWagesExcelTemplate.WagesReportAvailablePositionNumber + ".xlsx";
            var templateFileName = Server.MapPath(Constants.PlantWagesExcelTemplatesFolder  + templateFile);
            if (System.IO.File.Exists(templateFileName))
            {
                System.IO.File.Copy(templateFileName, strFileName);
            }

            using (SLDocument slDoc = new SLDocument(strFileName))
            {
                var dscPlant = _masterDataBLL.GetLocation(input.LocationCode);
                //filter values
                slDoc.SetCellValue(3, 2, ": " + input.LocationCode + "-" + dscPlant.LocationName);
                slDoc.SetCellValue(4, 2, ": " + input.UnitCode);
                slDoc.SetCellValue(5, 2, ": " + input.ProcessGroup);
                slDoc.SetCellValue(6, 2, ": " + input.Status);
                slDoc.SetCellValue(7, 2, ": " + input.GroupCode);


                //row values
                var iRow = 11;

                foreach (var masterListGroup in exeWagesAvailablePositionNumberViewModel)
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
                        style.Fill.SetPattern(PatternValues.Solid, Color.LightGray, Color.LightGray);
                    }
                    
                    slDoc.SetCellValue(iRow, 1, masterListGroup.LocationCode);
                    slDoc.SetCellValue(iRow, 2, masterListGroup.UnitCode);
                    slDoc.SetCellValue(iRow, 5, masterListGroup.ProcessSettingsCode);
                    slDoc.SetCellValue(iRow, 8, masterListGroup.Status);
                    slDoc.SetCellValue(iRow, 10, masterListGroup.EmployeeNumber);                   
                    slDoc.SetCellStyle(iRow, 1, iRow, 12, style);
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
            var fileName = "Wages_Report_Available_Position_Number_" + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

	}
}