using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using SKTISWebsite.Code;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.MstPlantEmployeeJobsData;
using SpreadsheetLight;
using Enums = HMS.SKTIS.Core.Enums;
using SKTISWebsite.Models.LookupList;

namespace SKTISWebsite.Controllers
{
    public class MasterPlantEmployeeJobDataController : BaseController
    {
        private IMasterDataBLL _bll;
        private IApplicationService _svc;

        public MasterPlantEmployeeJobDataController(IMasterDataBLL bll, IApplicationService svc)
        {
            _bll = bll;
            _svc = svc;
            SetPage("MasterData/Plant/EmployeeJobData");
        }

        // GET: MasterEmployeeJobData
        public ActionResult Index()
        {
            var InitMstPlantEmployeeJobsData = new InitMstPlantEmployeeJobsData
            {
                LocationNameLookupList = _svc.GetLocationNamesLookupList()
            };

            return View("Index", InitMstPlantEmployeeJobsData);
        }

        /// <summary>
        /// Get all list of locationcode
        /// </summary>
        /// <returns>list of LocationCode</returns>
        [HttpGet]
        public JsonResult GetLocationCodeSelectList()
        {
            var model = _bll.GetPlantLocationCodes();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Get all master employee jobs data active by filter
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns>list of employee jobs data active</returns>
        [HttpPost]
        public JsonResult GetEmployeeJobDatas(GetMstEmployeeJobsDataActivesInput criteria)
        {
            var pageResult = new PageResult<MstEmployeeJobsDataViewModel>();
            var masterLists = _bll.GetMstEmployeeJobsDataActives(criteria);
            pageResult.TotalRecords = masterLists.Count;
            pageResult.TotalPages = (masterLists.Count / criteria.PageSize) + (masterLists.Count % criteria.PageSize != 0 ? 1 : 0);
            var result = masterLists.Skip((criteria.PageIndex - 1) * criteria.PageSize).Take(criteria.PageSize);
            pageResult.Results = Mapper.Map<List<MstEmployeeJobsDataViewModel>>(result);
            return Json(pageResult);
        }

        /// <summary>
        /// Generate excel by filter
        /// </summary>
        /// <param name="locationCode"></param>
        /// <returns></returns>
        [HttpPost]
        public FileStreamResult GenerateExcel(string locationCode)
        {
            var input = new GetMstEmployeeJobsDataActivesInput() { LocationCode = locationCode };
            var datas = _bll.GetMstEmployeeJobsDataActives(input);

            MemoryStream ms = new MemoryStream();

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = Enums.ExcelTemplate.MstEmployeeJobsData + ".xlsx";
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

                foreach (var data in datas.Select((value, index) => new { Value = value, Index = index }))
                {
                    SLStyle style = slDoc.CreateStyle();
                    style.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Font.FontName = "Calibri";
                    style.Font.FontSize = 10;

                    if (data.Index % 2 == 0)
                    {
                        style.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);
                    }

                    slDoc.SetCellValue(iRow, 1, data.Value.EmployeeID);
                    slDoc.SetCellValue(iRow, 2, data.Value.EmployeeNumber);
                    slDoc.SetCellValue(iRow, 3, data.Value.EmployeeName);
                    slDoc.SetCellValue(iRow, 4, data.Value.JoinDate.HasValue ? data.Value.JoinDate.Value.ToString(Constants.DefaultDateFormat) : string.Empty);
                    slDoc.SetCellValue(iRow, 5, data.Value.Title_id);
                    slDoc.SetCellValue(iRow, 6, data.Value.ProcessSettingsCode);
                    slDoc.SetCellValue(iRow, 7, data.Value.Status);
                    slDoc.SetCellValue(iRow, 8, data.Value.CCT);
                    slDoc.SetCellValue(iRow, 9, data.Value.CCTDescription);
                    slDoc.SetCellValue(iRow, 10, data.Value.HCC);
                    slDoc.SetCellValue(iRow, 11, data.Value.GroupCode);
                    slDoc.SetCellValue(iRow, 12, data.Value.Loc_id);
                    slDoc.SetCellValue(iRow, 13, data.Value.LocationCode);
                    slDoc.SetCellValue(iRow, 14, data.Value.UnitCode);
                    slDoc.SetCellValue(iRow, 15, data.Value.Remark);
                    slDoc.SetCellValue(iRow, 16, data.Value.UpdatedBy);
                    slDoc.SetCellValue(iRow, 17, data.Value.UpdatedDate.ToString(Constants.DefaultDateFormat));
                    slDoc.SetCellStyle(iRow, 1, iRow, 17, style);
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
                slDoc.AutoFitColumn(2, 18);

                System.IO.File.Delete(strFileName);
                slDoc.SaveAs(ms);
            }
            // this is important. Otherwise you get an empty file
            // (because you'd be at EOF after the stream is written to, I think...).
            ms.Position = 0;
            var fileName = "MasterEmployeeJobsData_" + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        public IApplicationService svc { get; set; }
    }
}