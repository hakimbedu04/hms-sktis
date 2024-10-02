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
using SKTISWebsite.Models.MasterGenHoliday;
using SKTISWebsite.Models.MasterGenLocations;
using SpreadsheetLight;
using Enums = HMS.SKTIS.Core.Enums;

namespace SKTISWebsite.Controllers
{
    public class MasterGenHolidayController : BaseController
    {
        private IApplicationService _applicationService;
        private IMasterDataBLL _masterDataBLL;

        public MasterGenHolidayController(IApplicationService applicationService, IMasterDataBLL masterDataBll)
        {
            _applicationService = applicationService;
            _masterDataBLL = masterDataBll;
            SetPage("MasterData/General/Holiday");
        }
        //
        // GET: /MasterGenHoliday/
        public ActionResult Index()
        {
            var listMstYear = _applicationService.GetYearSelectList().OrderBy(x => x.Value).Select(x => x.Value).ToList();
            //var listYear = (Enumerable.Range(Convert.ToInt32(Enums.Years.Start), Convert.ToInt32(Enums.Years.End))).Select(fi => fi).ToList();
            var locationInfo = _masterDataBLL.GetLocationInfo(CurrentUser.Location[0].Code);
            var location = _masterDataBLL.GetAllLocationByLocationCode(CurrentUser.Location[0].Code, -1);
            var initHoliday = new InitHolidayModel
                              {
                                  LocationCode = Mapper.Map<List<SelectListItem>>(locationInfo),
                                  LocationDescs = Mapper.Map<List<MasterGenLocationDesc>>(location),
                                  //Years = listYear.Select(x => new SelectListItem
                                  //{
                                  //    Text = x.ToString(),
                                  //    Value = x.ToString(),
                                  //    Selected = (x.ToString() == DateTime.Now.Year.ToString())
                                  //}).ToList(),
                                  Years = listMstYear.Select(x => new SelectListItem{
                                      Text = x.ToString(),
                                      Value = x.ToString(),
                                      Selected = (x.ToString() == DateTime.Now.Year.ToString())
                                  }).ToList(),
                                  HolidayDate = DateTime.Now.Date,
                                  HolidayType = Mapper.Map<List<SelectListItem>>(_masterDataBLL.GetGeneralListHolidayType()),
                                  CurrentLocation = CurrentUser.Location[0].Code
                              };
            return View("Index", initHoliday);
        }

        /// <summary>
        /// get holiday whit parameter
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetMstGenHoliday(MstHolidayInput criteria)
        {
            var pageResult = new PageResult<MasterGenHolidayViewModel>();
            var TPOPackages = _masterDataBLL.GetMstHoliday(criteria);
            pageResult.TotalRecords = TPOPackages.Count;
            pageResult.TotalPages = (TPOPackages.Count / criteria.PageSize) + (TPOPackages.Count % criteria.PageSize != 0 ? 1 : 0);
            var result = TPOPackages.Skip((criteria.PageIndex - 1) * criteria.PageSize).Take(criteria.PageSize);
            pageResult.Results = Mapper.Map<List<MasterGenHolidayViewModel>>(result);
            return Json(pageResult);
        }

        /// <summary>
        /// POST : Save or Update Data
        /// </summary>
        /// <param name="bulkData"></param>
        /// <returns>Json</returns>
        [HttpPost]
        public JsonResult SaveHoliday(InsertUpdateData<MasterGenHolidayViewModel> bulkData)
        {
            if (bulkData.New != null)
            {
                for (int i = 0; i < bulkData.New.Count; i++)
                {
                    if (bulkData.New[i] == null) continue;

                    var holiday = Mapper.Map<MstHolidayDTO>(bulkData.New[i]);

                    holiday.CreatedBy = GetUserName();
                    holiday.UpdatedBy = GetUserName();
                    holiday.LocationCode = holiday.LocationCode.Trim();
                    try
                    {
                        var item = _masterDataBLL.InsertHoliday(holiday);
                        bulkData.New[i] = Mapper.Map<MasterGenHolidayViewModel>(item);
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
                        bulkData.New[i].Message = ex.Message;
                    }
                }
            }

            if (bulkData.Edit != null)
            {
                for (int i = 0; i < bulkData.Edit.Count; i++)
                {
                    if (bulkData.Edit[i] == null) continue;

                    var holiday = Mapper.Map<MstHolidayDTO>(bulkData.Edit[i]);

                    holiday.UpdatedBy = GetUserName();

                    try
                    {
                        var item = _masterDataBLL.UpdateHoliday(holiday);
                        bulkData.Edit[i] = Mapper.Map<MasterGenHolidayViewModel>(item);
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
                        bulkData.Edit[i].Message = ex.Message;
                    }
                }
            }

            return Json(bulkData);
        }

        /// <summary>
        /// Generate Excel
        /// </summary>
        /// <param name="LocationCode"></param>
        /// <param name="LocationName"></param>
        /// <param name="Year"></param>
        /// <param name="HolidayDate"></param>
        /// <returns></returns>
        [HttpPost]
        public FileStreamResult GenerateExcel(string LocationCode, string LocationName, int Year, DateTime? HolidayDate)
        {
            var input = new MstHolidayInput
            {
                LocationCode = LocationCode,
                Year = Year,
                HolidayDate = HolidayDate
            };
            var holidays = _masterDataBLL.GetMstHoliday(input);

            var ms = new MemoryStream();

            var strTempFileName = Path.GetTempFileName();
            var strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = Enums.ExcelTemplate.MstGenHoliday + ".xlsx";
            var templateFileName = Server.MapPath(Constants.MasterDataExcelTemplatesFolder + templateFile);
            if (System.IO.File.Exists(templateFileName))
            {
                System.IO.File.Copy(templateFileName, strFileName);
            }

            using (var slDoc = new SLDocument(strFileName))
            {
                //filter values
                slDoc.SetCellValue(2, 2, ": " + input.LocationCode + " - " + LocationName);
                slDoc.SetCellValue(3, 2, ": " + input.Year);
                slDoc.SetCellValue(4, 2, ": " + input.HolidayDate.ToString().Replace("0:00:00",""));

                //row values
                var iRow = 7;

                foreach (var holiday in holidays)
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

                    slDoc.SetCellValue(iRow, 1, holiday.LocationCode);
                    slDoc.SetCellValue(iRow, 2, holiday.HolidayDate.ToString(Constants.DefaultDateOnlyFormat));
                    slDoc.SetCellValue(iRow, 3, holiday.Description);
                    slDoc.SetCellValue(iRow, 4, holiday.HolidayType);
                    slDoc.SetCellValue(iRow, 5, holiday.StatusActive.ToString());
                    slDoc.SetCellValue(iRow, 6, holiday.Remark);
                    slDoc.SetCellValue(iRow, 7, holiday.UpdatedBy);
                    slDoc.SetCellValue(iRow, 8, holiday.UpdatedDate.ToString(Constants.DefaultDateFormat));
                    slDoc.SetCellStyle(iRow, 1, iRow, 8, style);
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
                slDoc.AutoFitColumn(2, 7);

                System.IO.File.Delete(strFileName);
                slDoc.SaveAs(ms);
            }
            // this is important. Otherwise you get an empty file
            // (because you'd be at EOF after the stream is written to, I think...).
            ms.Position = 0;
            var fileName = "MasterHoliday_" + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}