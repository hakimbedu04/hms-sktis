using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using HMS.SKTIS.BusinessObjects.DTOs.PlantWages;
using HMS.SKTIS.BusinessObjects.Inputs;
using HMS.SKTIS.BusinessObjects.Inputs.PlantWages;
using HMS.SKTIS.BusinessObjects.Outputs;
using HMS.SKTIS.Core;
using Microsoft.Ajax.Utilities;
using SKTISWebsite.Code;
using System.Web.Mvc;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.EblekReleaseApproval;
using System;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Utils;
using System.IO;
using SpreadsheetLight;
using DocumentFormat.OpenXml.Spreadsheet;
using Enums = HMS.SKTIS.Core.Enums;
using HMS.SKTIS.BusinessObjects.DTOs;

namespace SKTISWebsite.Controllers
{
    public class EblekReleaseApprovalController : BaseController
    {
        private readonly IApplicationService        _appService;
        private readonly IMasterDataBLL             _masterDataBll;
        private readonly IPlantWagesExecutionBLL    _plantWagesBll;
        private readonly IUtilitiesBLL _utilitiesBLL;
        private readonly IVTLogger _vtLogger;

        public EblekReleaseApprovalController
        (
            IApplicationService     appService,
            IMasterDataBLL          masterDataBll,
            IPlantWagesExecutionBLL plantWages,
            IUtilitiesBLL           utilitiesBLL,
            IVTLogger vt
        )
        {
            _appService     = appService;
            _masterDataBll  = masterDataBll;
            _plantWagesBll  = plantWages;
            _utilitiesBLL = utilitiesBLL;
            _vtLogger = vt;
            SetPage("PlantWages/Execution/EblekReleaseApproval");
        }

        public ActionResult Index(string param1, string param2, int? param3,string param4, string param5, int? param6)
        {
            if (param6.HasValue) setResponsibility(param6.Value);

            DateTime outputDateTimeValue;
            DateTime.TryParseExact(param5, "dd-MM-yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out outputDateTimeValue);
            var weekFromMasterData = _masterDataBll.GetWeekByDate(outputDateTimeValue);
            var week = weekFromMasterData == null ? 0 : weekFromMasterData.Week == null ? 0 : weekFromMasterData.Week.Value;
            var date = outputDateTimeValue.Date.ToShortDateString();
            var year = outputDateTimeValue.Year;
            var model = new InitEblekRelaseApproval()
            {
                LocationCodeSelectList          = _appService.GetPlantChildLocationCode(),
                YearSelectList                  = _appService.GetGenWeekYears(),
                DefaultYear                     = DateTime.Now.Year,
                DefaultWeek                     = _masterDataBll.GetGeneralWeekWeekByDate(DateTime.Now),
                TodayDate                       = DateTime.Now.ToShortDateString(),
                PlntChildLocationLookupList     = _appService.GetChildLocationsLookupList(HMS.SKTIS.Core.Enums.LocationCode.PLNT.ToString()),
                //Date                            = _appService.DatePlantWagesSelectList(),
                Param1LocationCode = param1,
                Param2UnitCode = param2,
                Param3Shift = param3,
                Param4Process = param4,
                Param5Date = param5,
                dateFromUrl = date,
                weekFromUrl = week,
                yearFromUrl = year
            };
            return View(model);
        }

        public JsonResult RoleButton(InsertUpdateData<EblekReleaseApprovalViewModel> bulkData)
        {
            var appState = new List<bool>();
            var rtnState = new List<bool>();
            if (bulkData.Edit != null)
            {
                for (int i = 0; i < bulkData.Edit.Count; i++)
                {
                    if (bulkData.Edit[i] == null || bulkData.Edit[i].Flag == false) continue;

                    var data = bulkData.Edit[i];
                    //generate transactioncode
                    var content = new ButtonState()
                    {
                        Approve = Convert.ToBoolean(_utilitiesBLL.RoleButtonChecker(data.ProductionEntryCode, CurrentUser.Responsibility.Role, HMS.SKTIS.Core.Enums.PageName.EblekReleaseApproval.ToString(), HMS.SKTIS.Core.Enums.ButtonName.Approve.ToString())),
                        Return = Convert.ToBoolean(_utilitiesBLL.RoleButtonChecker(data.ProductionEntryCode, CurrentUser.Responsibility.Role, HMS.SKTIS.Core.Enums.PageName.EblekReleaseApproval.ToString(), HMS.SKTIS.Core.Enums.ButtonName.Approve.ToString()))
                    };
                    appState.Add(content.Approve);
                    rtnState.Add(content.Return);

                }
            }

            var init = new ButtonState()
            {
                Approve = !appState.Contains(false),
                Return =  !rtnState.Contains(false)
            };
            return Json(init, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetEblekReleaseApproval(GetEblekReleaseApprovalInput criteria)
        {
            var eblekReleaseApproval = _plantWagesBll.GetEblekReleaseApproval(criteria);
            var viewModel = Mapper.Map<List<EblekReleaseApprovalViewModel>>(eblekReleaseApproval);
            var pageResult = new PageResult<EblekReleaseApprovalViewModel>(viewModel, criteria);
            return Json(pageResult);
        }

        [HttpPost]
        public ActionResult ApproveAllEblekReleaseApproval(InsertUpdateData<EblekReleaseApprovalViewModel> bulkData)
        {
            var locationCode = bulkData.Parameters != null ? bulkData.Parameters["LocationCode"] : "";
            var unitCode = bulkData.Parameters != null ? bulkData.Parameters["UnitCode"] : "";
            var shift = bulkData.Parameters != null ? bulkData.Parameters["Shift"] : "";

            //edit data
            if (bulkData.Edit != null)
            {
                for (var i = 0; i < bulkData.Edit.Count; i++)
                {
                    //check row is null
                    if (bulkData.Edit[i] == null) continue;

                    //if (!bulkData.Edit.Select(c => c.Flag).Contains(true))
                    if (bulkData.Edit[i].Flag == false)
                    {
                        bulkData.Edit[i] = null;
                        continue;
                    }

                    var eblekReleaseApproval = Mapper.Map<EblekReleaseApprovalDTO>(bulkData.Edit[i]);

                    //set updatedby
                    eblekReleaseApproval.UpdatedBy = GetUserName();
                    var item = new EblekReleaseApprovalDTO();
                    try
                    {
                        item = _plantWagesBll.ApproveEblekReleaseApproval(eblekReleaseApproval, CurrentUser);
                        bulkData.Edit[i] = Mapper.Map<EblekReleaseApprovalViewModel>(item);
                        bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Success.ToString();
                        var week = _masterDataBll.GetWeekByDate(item.EblekDate);

                        _plantWagesBll.SendEmailApprove(new GetUserAndEmailInput
                        {
                            Process = item.ProcessGroup,
                            BrandCode = item.BrandCode,
                            LocationCode = locationCode,
                            UnitCode = unitCode,
                            Shift = item.Shift,
                            Date = item.EblekDate,
                            FunctionName = HMS.SKTIS.Core.Enums.PageName.EblekReleaseApproval.ToString(),
                            ButtonName = HMS.SKTIS.Core.Enums.ButtonName.Approve.ToString(),
                            UserName = GetUserName(),
                            EmailSubject = HMS.SKTIS.Core.Enums.PageName.EblekReleaseApproval.ToString() + " - " + HMS.SKTIS.Core.Enums.ButtonName.Approve.ToString(),
                            Remark = eblekReleaseApproval.Remark,
                            KpsYear = week.Year,
                            KpsWeek = week.Week
                        });
                    }
                    catch (ExceptionBase ex)
                    {
                        _vtLogger.Err(ex, new List<object> { eblekReleaseApproval });
                        bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                        bulkData.Edit[i].Message = ex.Message;
                    }
                    catch (Exception ex)
                    {
                        _vtLogger.Err(ex, new List<object> { eblekReleaseApproval });
                        bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                        bulkData.Edit[i].Message =
                            EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception);
                        ;
                    }
                }
            }

            return Json(bulkData);
        }

        [HttpPost]
        public ActionResult ReturnAllEblekReleaseApproval(InsertUpdateData<EblekReleaseApprovalViewModel> bulkData)
        {
            var locationCode= bulkData.Parameters != null ? bulkData.Parameters["LocationCode"] : "";
            var unitCode = bulkData.Parameters != null ? bulkData.Parameters["UnitCode"] : "";
            var shift = bulkData.Parameters != null ? bulkData.Parameters["Shift"] : "";

            //edit data
            if (bulkData.Edit != null)
            {
                for (var i = 0; i < bulkData.Edit.Count; i++)
                {
                    //check row is null
                    if (bulkData.Edit[i] == null) continue;

                    if (!bulkData.Edit.Select(c => c.Flag).Contains(true))
                    {
                        bulkData.Edit[i] = null;
                        continue;
                    }

                    var eblekReleaseApproval = Mapper.Map<EblekReleaseApprovalDTO>(bulkData.Edit[i]);

                    //set updatedby
                    eblekReleaseApproval.UpdatedBy = GetUserName();

                    try
                    {
                        var item = _plantWagesBll.ReturnEblekReleaseApproval(eblekReleaseApproval, CurrentUser);
                        bulkData.Edit[i] = Mapper.Map<EblekReleaseApprovalViewModel>(item);
                        bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Success.ToString();

                        _plantWagesBll.SendEmail(new GetUserAndEmailInput {
                            LocationCode = locationCode,
                            UnitCode = unitCode,
                            Shift = Convert.ToInt32(shift),
                            Date = item.EblekDate,
                            FunctionName = HMS.SKTIS.Core.Enums.PageName.EblekReleaseApproval.ToString(),
                            ButtonName = HMS.SKTIS.Core. Enums.ButtonName.Return.ToString(),
                            UserName = GetUserName(),
                            EmailSubject = HMS.SKTIS.Core.Enums.PageName.EblekReleaseApproval.ToString() + " - " + HMS.SKTIS.Core. Enums.ButtonName.Return.ToString(),
                            Remark = eblekReleaseApproval.Remark
                        });
                    }
                    catch (ExceptionBase ex)
                    {
                        _vtLogger.Err(ex, new List<object> { eblekReleaseApproval });
                        bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                        bulkData.Edit[i].Message = ex.Message;
                    }
                    catch (Exception ex)
                    {
                        _vtLogger.Err(ex, new List<object> { eblekReleaseApproval });
                        bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                        bulkData.Edit[i].Message =
                            EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception);
                        ;
                    }
                }
            }

            return Json(bulkData);
        }

        [HttpPost]
        public FileStreamResult GenerateExcelEblekReleaseApproval(GetEblekReleaseApprovalInput input)
        {
            var inputEblekReleaseApproval = new GetEblekReleaseApprovalInput
            {
                LocationCode = input.LocationCode,
                UnitCode = input.UnitCode,
                Date = input.Date,
                Shift = input.Shift,
                Process = input.Process,
                KpsWeek = input.KpsWeek,
                KpsYear = input.KpsYear
            };

            var allLocations = _appService.GetLocationCodeCompat();
            string locationCompat = "";
            foreach (SelectListItem item in allLocations)
            {
                if (item.Value == input.LocationCode)
                {
                    locationCompat = item.Text;
                }
            }


            var eblekReleaseApproval = _plantWagesBll.GetEblekReleaseApproval(inputEblekReleaseApproval);

            var ms = new MemoryStream();

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = HMS.SKTIS.Core.Enums.PlantWagesExcelTemplate.PlantWagesEblekReleaseApproval + ".xlsx";
            var templateFileName = Server.MapPath(Constants.PlantWagesExcelTemplatesFolder + templateFile);
            if (System.IO.File.Exists(templateFileName))
            {
                System.IO.File.Copy(templateFileName, strFileName);
            }

            using (SLDocument slDoc = new SLDocument(strFileName))
            {

                //filter values
                slDoc.SetCellValue(3, 2, ": " + locationCompat);
                slDoc.SetCellValue(4, 2, ": " + input.UnitCode);
                slDoc.SetCellValue(5, 2, ": " + input.Shift);
                slDoc.SetCellValue(6, 2, ": " + input.Process);
                slDoc.SetCellValue(3, 4, ": " + input.KpsYear);
                slDoc.SetCellValue(4, 4, ": " + input.KpsWeek);
                slDoc.SetCellValue(5, 4, ": " + input.Date.ToString(Constants.DefaultDateOnlyFormat));

                //row values
                var iRow = 9;

                foreach (var masterListGroup in eblekReleaseApproval)
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
                        style.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);
                    }

                    slDoc.SetCellValue(iRow, 1, masterListGroup.EblekDate.ToString(Constants.DefaultDateOnlyFormat));
                    slDoc.SetCellValue(iRow, 2, masterListGroup.BrandCode);
                    slDoc.SetCellValue(iRow, 3, masterListGroup.GroupCode);
                    slDoc.SetCellValue(iRow, 4, masterListGroup.Remark);
                    //slDoc.SetCellStyle(iRow, 1, iRow, 12, style);
                    slDoc.AutoFitColumn(4);
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
                //slDoc.AutoFitColumn(1, 12);

                System.IO.File.Delete(strFileName);
                slDoc.SaveAs(ms);
            }
            // this is important. Otherwise you get an empty file
            // (because you'd be at EOF after the stream is written to, I think...).
            ms.Position = 0;
            var fileName = "PlantWages_EblekReleaseApproval_" + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpGet]
        public JsonResult GetUnitCode(string locationCode)
        {
            var input = new GetAllUnitsInput() { LocationCode = locationCode };
            var units = _masterDataBll.GetAllUnits(input).Where(c => c.UnitCode != HMS.SKTIS.Core.Enums.UnitCodeDefault.MTNC.ToString());
            return Json(new SelectList(units, "UnitCode", "UnitCode"), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetShift(string locationCode)
        {
            var shift = _appService.GetShiftByLocationCode(locationCode);
            return Json(shift, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetProcessGroup(string locationCode)
        {
            //var result = _appService.GetProcessGroupSelectListByLocationCode(locationCode);
            var input = new GetAllProcessSettingsInput() { LocationCode = locationCode };
            var processSettings = _masterDataBll.GetMasterProcessSettingByLocationCode(locationCode);
            var processSettingsDistinctByProcessGroup = processSettings
                                                        .Where(c => c.ProcessGroup != EnumHelper.GetDescription(HMS.SKTIS.Core.Enums.Process.Daily))
                                                        .DistinctBy(x => x.ProcessGroup);
            return Json(new SelectList(processSettingsDistinctByProcessGroup, "ProcessGroup", "ProcessGroup"), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetWeekByYear(int year)
        {
            var kpsWeek = _masterDataBll.GetWeekByYear(year);
            return Json(kpsWeek, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetDateByYearWeek(int year, int week)
        {
            var date = _appService.GetSelectListDateByYearAndWeek(year, week);
            return Json(date, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetDateByCurrentYearWeek()
        {
            var mstGenWeek = _masterDataBll.GetWeekByDate(DateTime.Now);
            var week = mstGenWeek == null ? 0 : mstGenWeek.Week == null ? 0 : mstGenWeek.Week.Value;
            var date = _masterDataBll.GetDateByWeek(DateTime.Now.Year, week).Select(c => c.Date.ToShortDateString());
            return Json(date, JsonRequestBehavior.AllowGet);
        }

    }
}