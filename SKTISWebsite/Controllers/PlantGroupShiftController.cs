using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using AutoMapper;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.Contracts;
using SKTISWebsite.Code;
using SKTISWebsite.Models.PlantGroupShift;
using HMS.SKTIS.Core;
using SKTISWebsite.Models.Common;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.Utils;
using System.IO;
using SpreadsheetLight;
using DocumentFormat.OpenXml.Spreadsheet;

namespace SKTISWebsite.Controllers
{
    public class PlantGroupShiftController : BaseController
    {
        private IApplicationService _svc;
        private IMasterDataBLL _masterDataBll;
        private IPlanningBLL _planningBll;
        private IVTLogger _vtlogger;

        public PlantGroupShiftController(IApplicationService svc, IVTLogger vtlogger, IMasterDataBLL masterDataBll, IPlanningBLL planningBll)
        {
            _svc = svc;
            _masterDataBll = masterDataBll;
            _planningBll = planningBll;
            _vtlogger = vtlogger;
            SetPage("Productionplanning/Plant/GroupShift");
        }

        // GET: PlantGroupShift
        public ActionResult Index()
        {
            var model = new InitPlantGroupShift();
            model.Location = _svc.GetLocationCodeSelectListByShift(HMS.SKTIS.Core.Enums.Shift.Shift2);            
            model.ListPlantGroupShift1 = new List<PlanPlantGroupShiftViewModel>();
            model.ListPlantGroupShift2 = new List<PlanPlantGroupShiftViewModel>();
            model.YearSelectList = _svc.GetGenWeekYears();
            return View("Index", model);
        }

        [HttpGet]
        public JsonResult GetPlantLocationCode()
        {
            var locCodes = _svc.GetLocationCodeSelectListByShift(HMS.SKTIS.Core.Enums.Shift.Shift2);
            return Json(locCodes, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetUnitCode(string locationCode)
        {
            var listUnitCodes = _masterDataBll.GetAllUnits(new GetAllUnitsInput { LocationCode = locationCode }).Where(t => t.UnitCode != "MTNC").OrderBy(t =>t.CreatedDate).ToList();

            return Json(listUnitCodes, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWeekByYear(int year)
        {
            return Json(_masterDataBll.GetWeekByYear(year), JsonRequestBehavior.AllowGet);
        }
        
        public PartialViewResult GetPlanPlantGroupShifts(GetPlanPlantGroupShiftInput input)
        {
            try
            {
                var model = new Models.PlantGroupShift.InitPlantGroupShift();
                var planPlantGroupShifts = _planningBll.GetPlanPlantGroupShifts(input);
                model.ListPlantGroupShift1 =
                    Mapper.Map<List<PlanPlantGroupShiftViewModel>>(planPlantGroupShifts.Where(p => p.Shift == 1));
                model.ListPlantGroupShift2 =
                    Mapper.Map<List<PlanPlantGroupShiftViewModel>>(planPlantGroupShifts.Where(p => p.Shift == 2));
                if (planPlantGroupShifts.Any())
                {
                    model.StartDate = planPlantGroupShifts.FirstOrDefault().StartDate;
                    model.EndDate = planPlantGroupShifts.FirstOrDefault().EndDate;
                }

                return PartialView("_PlantGroupShiftPartial", model);
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { input }, "View - GetPlanPlantGroupShifts");
                return null;
            }
        }

        /// <summary>
        /// Save Plant Group Shift
        /// </summary>
        /// <param name="bulkData"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SavePlantGroupShift(List<PlanPlantGroupShiftViewModel> bulkData)
        {
            try
            {
                #region saving data

                if (bulkData.Count > 0)
                {
                    for (var i = 0; i < bulkData.Count; i++)
                    {
                        var groupShift = Mapper.Map<PlanPlantGroupShiftDTO>(bulkData[i]);
                        try
                        {
                            var item = _planningBll.SavePlanPlantGroupShift(groupShift, GetUserName());
                            bulkData[i] = Mapper.Map<PlanPlantGroupShiftViewModel>(item);
                            bulkData[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Success.ToString();
                        }
                        catch (ExceptionBase ex)
                        {
                            bulkData[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                            bulkData[i].Message = ex.Message;
                        }
                        catch (Exception ex)
                        {
                            bulkData[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                            bulkData[i].Message =
                                EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception);
                            ;
                        }
                    }
                }
                return Json(bulkData);

                #endregion;
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { }, "Save - SavePlantGroupShift");
                return Json(bulkData);
            }
        }
        
        /// <summary>
        /// Generate excel by filter
        /// </summary>
        /// <param name="listGroup"></param>
        /// <returns></returns>
        [HttpPost]
        public FileStreamResult GenerateExcel(string locationCode, string unitCode, DateTime? startDate, DateTime? endDate)
        {
            try
            {
                #region generate Excel

                var input = new GetPlanPlantGroupShiftInput
                {
                    LocationCode = locationCode,
                    UnitCode = unitCode,
                    StartDate = startDate
                };
                var groupShiftlist = _planningBll.GetGroupShift(input);

                MemoryStream ms = new MemoryStream();

                string strTempFileName = Path.GetTempFileName();
                string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
                if (System.IO.File.Exists(strFileName))
                {
                    System.IO.File.Delete(strFileName);
                }
                var locationInfo = _masterDataBll.GetLocation(locationCode);
                var templateFile = HMS.SKTIS.Core.Enums.PlanningExcelTemplate.PlantGroupShift + ".xlsx";
                var templateFileName = Server.MapPath(Constants.PlanningExcelTemplatesFolder + templateFile);
                if (System.IO.File.Exists(templateFileName))
                {
                    System.IO.File.Copy(templateFileName, strFileName);
                }

                using (SLDocument slDoc = new SLDocument(strFileName))
                {
                    //filter values
                    slDoc.SetCellValue(4, 2, locationCode);
                    slDoc.SetCellValue(4, 3, locationInfo.LocationName);
                    slDoc.SetCellValue(5, 2, unitCode);
                    slDoc.SetCellValue(6, 2, startDate.Value.ToString(Constants.DefaultDateOnlyFormat));
                    slDoc.SetCellValue(7, 2, endDate.Value.ToString(Constants.DefaultDateOnlyFormat));

                    //row values
                    var iRow = 9;

                    foreach (var groupShift in groupShiftlist)
                    {
                        SLStyle style = slDoc.CreateStyle();
                        style.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                        style.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                        style.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                        style.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                        style.Font.FontName = "Calibri";
                        style.Font.FontSize = 10;

                        if (iRow%2 == 0)
                        {
                            style.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray,
                                System.Drawing.Color.LightGray);
                        }

                        slDoc.SetCellValue(iRow, 1, groupShift.GroupCode);
                        slDoc.SetCellValue(iRow, 2, groupShift.StartDate.Value.ToString(Constants.DefaultDateOnlyFormat));
                        slDoc.SetCellValue(iRow, 3, groupShift.EndDate.Value.ToString(Constants.DefaultDateOnlyFormat));
                        slDoc.SetCellValue(iRow, 4, (groupShift.Shift == 1) ? "Yes" : "No");
                        slDoc.SetCellValue(iRow, 5, (groupShift.Shift == 2) ? "Yes" : "No");
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
                    slDoc.AutoFitColumn(1, 5);

                    System.IO.File.Delete(strFileName);
                    slDoc.SaveAs(ms);
                }
                // this is important. Otherwise you get an empty file
                // (because you'd be at EOF after the stream is written to, I think...).
                ms.Position = 0;
                var fileName = "PlantGroupShift_" + DateTime.Now.ToShortDateString() + ".xlsx";
                return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);

                #endregion
            }
            catch (Exception ex)
            {
                _vtlogger.Err(ex, new List<object> { }, "Generate Excel on PlantGroupShift");
                return null;
            }
        }
    }
}