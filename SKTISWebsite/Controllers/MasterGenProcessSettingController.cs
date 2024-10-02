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
using SKTISWebsite.Models;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.MasterGenLocations;
using SKTISWebsite.Models.MasterGenProccessSetting;
using SKTISWebsite.Models.MasterGenProcess;
using SpreadsheetLight;
using Enums = HMS.SKTIS.Core.Enums;
using Microsoft.Ajax.Utilities;

namespace SKTISWebsite.Controllers
{
    public class MasterGenProcessSettingController : BaseController
    {
        private IMasterDataBLL _bll;
        private IApplicationService _svc;

        public MasterGenProcessSettingController(IApplicationService svc, IMasterDataBLL bll)
        {
            _svc = svc;
            _bll = bll;
            SetPage("MasterData/General/ProcessSetting");
        }

        // GET: Master Process Setting
        public ActionResult Index()
        {

            var masterGenProccessSetting = new InitMasterGenProccessSetting()
            {
                LocationCodeSelectList = _svc.GetLocationCodeSelectList(),
                BrandGroupCodeSelectList = _svc.GetBrandGroupCodeSelectList(),
                ProcessGroupSelectList = _svc.GetProcessGroupSelectList(),
                ProcessList = Mapper.Map<List<MasterGenProcessIdentifier>>(_bll.GetMasterProcesses(new GetMstGenProcessesInput())),
                LocationNameLookupList = _svc.GetLocationNamesLookupList()
            };
            return View("index", masterGenProccessSetting);
        }

        /// <summary>
        /// Get all master process setting data by filter
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns>list of master process setting</returns>
        [HttpPost]
        public JsonResult GetProcessSettings(GetMasterProcessSettingsInput criteria)
        {

            var masterProcess = _bll.GetMasterProcessSettings(criteria);
            var viewModel = Mapper.Map<List<MstGenProccessSettingViewModel>>(masterProcess);
            var pageResult = new PageResult<MstGenProccessSettingViewModel>(viewModel, criteria);
            return Json(pageResult);
        }

        /// <summary>
        /// Get all master process settings location data by filter
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetProcessSettingsLocations(GetMstGenProcessSettingLocationInput criteria)
        {
            var processSettingsLocations = _bll.GetMstGenProcessSettingLocations(criteria).DistinctBy(p => new { p.IDProcess, p.LocationCode }).OrderBy(p => p.IDProcess);
            var viewModel = Mapper.Map<List<MstGenProcessSettingLocationViewModel>>(processSettingsLocations);
            var pageResult = new PageResult<MstGenProcessSettingLocationViewModel>(viewModel, criteria);
            return Json(pageResult);
        }

        /// <summary>
        /// Get Process Setting Process ID
        /// </summary>
        /// <param name="increment">If true will add more from max existing value</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetIDProcessSelectList(bool increment = false)
        {
            var processIDList = _bll.GetProcessSettingIDProcess(increment);
            return Json(processIDList, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Save all new and updated general list item
        /// </summary>
        /// <param name="bulkData"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveAllProcessSettings(InsertUpdateData<MstGenProccessSettingViewModel> bulkData)
        {
            if (bulkData.New != null)
            {
                for (var i = 0; i < bulkData.New.Count; i++)
                {
                    //check row is null
                    if (bulkData.New[i] == null) continue;
                    var mstProcessSetting = Mapper.Map<MstGenProcessSettingDTO>(bulkData.New[i]);

                    //set createdby and updatedby
                    mstProcessSetting.CreatedBy = GetUserName();
                    mstProcessSetting.UpdatedBy = GetUserName();

                    try
                    {
                        var item = _bll.InsertProcessSetting(mstProcessSetting);
                        bulkData.New[i] = Mapper.Map<MstGenProccessSettingViewModel>(item);
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
                    var mstProcessSetting = Mapper.Map<MstGenProcessSettingDTO>(bulkData.Edit[i]);

                    //set updatedby
                    mstProcessSetting.UpdatedBy = GetUserName();

                    try
                    {
                        var item = _bll.UpdateProcessSetting(mstProcessSetting);
                        bulkData.Edit[i] = Mapper.Map<MstGenProccessSettingViewModel>(item);
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
                        bulkData.Edit[i].Message = EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception);
                        throw ex;
                    }
                }
            }

            return Json(bulkData);
        }

        [HttpPost]
        public ActionResult SaveAllProcessSettingLocations(InsertUpdateData<MstGenProccessSettingLocationViewModel> bulkData)
        {
            if (bulkData.New != null)
            {
                for (var i = 0; i < bulkData.New.Count; i++)
                {
                    //check row is null
                    if (bulkData.New[i] == null) continue;
                    var mstProcessSettingLocation = Mapper.Map<MstGenProcessSettingLocationDTO>(bulkData.New[i]);

                    //set createdby and updatedby
                    mstProcessSettingLocation.CreatedBy = GetUserName();
                    mstProcessSettingLocation.UpdatedBy = GetUserName();

                    try
                    {
                        var item = _bll.SaveMstGenProcessLocation(mstProcessSettingLocation, true);
                        bulkData.New[i] = Mapper.Map<MstGenProccessSettingLocationViewModel>(item);
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

            //Update data
            if (bulkData.Edit != null)
            {
                for (var i = 0; i < bulkData.Edit.Count; i++)
                {
                    //check row is null
                    if (bulkData.Edit[i] == null) continue;
                    var mstProcessSettingLocation = Mapper.Map<MstGenProcessSettingLocationDTO>(bulkData.Edit[i]);

                    // set created by since user can update the process setting only from update row (it will filtered at BLL)
                    mstProcessSettingLocation.CreatedBy = GetUserName();
                    //set updatedby
                    mstProcessSettingLocation.UpdatedBy = GetUserName();

                    try
                    {
                        var item = _bll.SaveMstGenProcessLocation(mstProcessSettingLocation, false);
                        bulkData.Edit[i] = Mapper.Map<MstGenProccessSettingLocationViewModel>(item);
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
        public ActionResult DeleteProsesLocation(MstGenProccessSettingLocationViewModel location)
        {
            var response = new ViewModelBase();
            var dbData = Mapper.Map<MstGenProcessSettingLocationDTO>(location);

            try
            {
                var item = _bll.DeleteProsesSettingLocId(dbData);
                location = Mapper.Map<MstGenProccessSettingLocationViewModel>(item);
                //response.ResponseType = Enums.ResponseType.Success.ToString();
                //response.Message = "Delete Data Succes";
            }
            catch (ExceptionBase ex)
            {
                response.ResponseType = Enums.ResponseType.Error.ToString();
                response.Message = ex.Message;
            }
            catch (Exception ex)
            {
                response.ResponseType = Enums.ResponseType.Error.ToString();
                response.Message = EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception); ;
            }
            return Json(location);
        }


        /// <summary>
        /// Generate excel by filter
        /// </summary>
        /// <param name="listGroup"></param>
        /// <returns></returns>
        [HttpPost]
        public FileStreamResult GenerateExcelProcess(string brandCode, string process)
        {
            var input = new GetMasterProcessSettingsInput() { BrandCode = brandCode, Process = process, SortExpression = "UpdatedDate", SortOrder = "DESC" };
            var masterProcessSettings = _bll.GetMasterProcessSettings(input);

            MemoryStream ms = new MemoryStream();

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = Enums.ExcelTemplate.MstGenProcessSettings + ".xlsx";
            var templateFileName = Server.MapPath(Constants.MasterDataExcelTemplatesFolder + templateFile);
            if (System.IO.File.Exists(templateFileName))
            {
                System.IO.File.Copy(templateFileName, strFileName);
            }

            using (SLDocument slDoc = new SLDocument(strFileName))
            {
                //filter values
              
                slDoc.SetCellValue(2, 2, brandCode == "" ? "All" : brandCode);
                slDoc.SetCellValue(3, 2, process == "" ? "All" : process);

                //row values
                var iRow = 6;

                foreach (var item in masterProcessSettings.Select((value, index) => new { Value = value, Index = index }))
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

                    slDoc.SetCellValue(iRow, 1, item.Value.IDProcess);
                    slDoc.SetCellValue(iRow, 2, item.Value.BrandGroupCode);
                    slDoc.SetCellValue(iRow, 3, item.Value.ProcessGroup);
                    //slDoc.SetCellValue(iRow, 3, item.Value.ProcessIdentifier);
                    slDoc.SetCellValue(iRow, 4, item.Value.StdStickPerHour.ToString());
                    slDoc.SetCellValue(iRow, 5, item.Value.MinStickPerHour.ToString());
                    slDoc.SetCellValue(iRow, 6, item.Value.UOMEblek.ToString());
                    slDoc.SetCellValue(iRow, 7, item.Value.Remark);
                    slDoc.SetCellValue(iRow, 8, item.Value.UpdatedBy);
                    slDoc.SetCellValue(iRow, 9, item.Value.UpdatedDate.ToString(Constants.DefaultDateFormat));
                    slDoc.SetCellStyle(iRow, 1, iRow, 9, style);
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
                slDoc.AutoFitColumn(1, 9);

                System.IO.File.Delete(strFileName);
                slDoc.SaveAs(ms);
            }
            // this is important. Otherwise you get an empty file
            // (because you'd be at EOF after the stream is written to, I think...).
            ms.Position = 0;
            var fileName = Enums.ExcelTemplate.MstGenProcessSettings + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        [HttpPost]
        public FileStreamResult GenerateExcelLocation(string locationCode, string brandGroupCode, int? idProcess)
        {
            var input = new GetMstGenProcessSettingLocationInput() { LocationCode = locationCode, BrandGroupCode = brandGroupCode, IDProcess = idProcess };
            var masterProcessSettingLocations = _bll.GetMstGenProcessSettingLocations(input).DistinctBy(p => new { p.IDProcess, p.LocationCode }).OrderBy(p => p.IDProcess);

            MemoryStream ms = new MemoryStream();

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = Enums.ExcelTemplate.MstGenProcessSettingLocation + ".xlsx";
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
                slDoc.SetCellValue(3, 2, brandGroupCode == "" ? "All" : brandGroupCode);
                slDoc.SetCellValue(4, 2, idProcess.ToString() == "" ? "All" : idProcess.ToString());

                //row values
                var iRow = 7;

                foreach (var item in masterProcessSettingLocations.Select((value, index) => new { Value = value, Index = index }))
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

                    slDoc.SetCellValue(iRow, 1, item.Value.IDProcess);
                    slDoc.SetCellValue(iRow, 2, item.Value.LocationCode);
                    slDoc.SetCellValue(iRow, 3, item.Value.LocationName);
                    //slDoc.SetCellValue(iRow, 4, item.Value.MaxWorker.ToString());
                    slDoc.SetCellValue(iRow, 4, "dd");
                    slDoc.SetCellStyle(iRow, 1, iRow, 4, style);
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
                slDoc.AutoFitColumn(1, 4);

                System.IO.File.Delete(strFileName);
                slDoc.SaveAs(ms);
            }
            // this is important. Otherwise you get an empty file
            // (because you'd be at EOF after the stream is written to, I think...).
            ms.Position = 0;
            var fileName = Enums.ExcelTemplate.MstGenProcessSettingLocation + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}
