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
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.Factories.Contract;
using SKTISWebsite.Models.MasterTOPInfo;
using SpreadsheetLight;
using Enums = HMS.SKTIS.Core.Enums;
using SKTISWebsite.Models.MasterGenLocations;

namespace SKTISWebsite.Controllers
{
    public class MasterTPOInfoController : BaseController
    {
        private AbstractSelectList _abstractSelectList;
        private IMasterDataBLL _masterDataBll;

        public MasterTPOInfoController(AbstractSelectList abstractSelectList, IMasterDataBLL masterDataMasterDataBll)
        {
            _masterDataBll = masterDataMasterDataBll;
            _abstractSelectList = abstractSelectList;
            SetPage("MasterData/General/Location");
        }

        // GET: MasterTpo
        public ActionResult Index()
        {
            var locations = _masterDataBll.GetLocationCodes();
            var locationCodes = locations.Select(location => new SelectListItem()
            {
                Value = location,
                Text = location
            }).ToList();

            var initMasterTpoItems = new InitMasterTPOInfo
            {
                ItemLocationCodes = locationCodes,
                ItemTPORanks = Mapper.Map<List<SelectListItem>>(_masterDataBll.GetGeneralListTPORank()),
                LocationDescs = Mapper.Map<List<MasterGenLocationDesc>>(_masterDataBll.GetMstLocationLists(new GetMstGenLocationInput()))
            };
            return View("Index", initMasterTpoItems);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tpo"></param>
        /// <returns></returns>
        public ActionResult GetMasterTpo(GetMasterTPOInfoInput tpo)
        {
            var pageResult = new PageResult<MasterTPOInfoViewModels>();
            var masterTpo = _masterDataBll.GetMstTPOInfos(tpo);

            pageResult.TotalRecords = masterTpo.Count;
            pageResult.TotalPages = (masterTpo.Count / tpo.PageSize) + (masterTpo.Count % tpo.PageSize != 0 ? 1 : 0);
            var result = masterTpo.Skip((tpo.PageIndex - 1)*tpo.PageSize).Take(tpo.PageSize);
            pageResult.Results = Mapper.Map<List<MasterTPOInfoViewModels>>(result);
            return Json(pageResult);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bulkData"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveTpos(InsertUpdateData<MasterTPOInfoViewModels> bulkData)
        {
            if (bulkData.New != null)
                for (int i = 0; i < bulkData.New.Count; i++)
                {
                    //check row is null
                    if (bulkData.New[i] == null) continue;
                    var tpo = Mapper.Map<MstTPOInfoDTO>(bulkData.New[i]);

                    // set createdby and updatedby
                    tpo.CreatedBy = GetUserName();
                    tpo.UpdatedBy = GetUserName();

                    try
                    {
                        var item = _masterDataBll.InsertMasterTPOInfo(tpo);
                        bulkData.New[i] = Mapper.Map<MasterTPOInfoViewModels>(item);
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

                    var tpo = Mapper.Map<MstTPOInfoDTO>(bulkData.Edit[i]);
                    tpo.UpdatedBy = GetUserName();
                    try
                    {
                        var item = _masterDataBll.UpdateMasterTPOInfo(tpo);

                        bulkData.Edit[i] = Mapper.Map<MasterTPOInfoViewModels>(item);
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
        /// <returns></returns>
        [HttpPost]
        public FileStreamResult GenerateExcel(string locationCode)
        {
            var mstTpoLists = _masterDataBll.GetMstTPOInfos(new GetMasterTPOInfoInput { LocationCode = locationCode });
            var mstLoc = _masterDataBll.GetMstLocationById(locationCode);
            var loc = "";
            if (mstLoc != null)
                loc = mstLoc.LocationName;
            var ms = new MemoryStream();

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = Enums.ExcelTemplate.MstTPO + ".xlsx";
            var templateFileName = Server.MapPath(Constants.MasterDataExcelTemplatesFolder + templateFile);
            if (System.IO.File.Exists(templateFileName))
            {
                System.IO.File.Copy(templateFileName, strFileName);
            }

            using (var slDoc = new SLDocument(strFileName))
            {
                // add style
                //SLStyle style = slDoc.CreateStyle();
                //style.Border.BottomBorder.BorderStyle = BorderStyleValues.Hair;
                //style.Border.LeftBorder.BorderStyle = BorderStyleValues.Medium;
                //style.Border.RightBorder.BorderStyle = BorderStyleValues.SlantDashDot;
                //style.Border.TopBorder.BorderStyle = BorderStyleValues.Thick;
                //slDoc.SetCellStyle(2,2, style);
                slDoc.SetCellValue(2, 2, locationCode + " - " + loc);

                //row values
                var iRow = 5;

                foreach (var tpo in mstTpoLists)
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

                    slDoc.SetCellValue(iRow, 1, tpo.LocationCode);
                    slDoc.SetCellValue(iRow, 2, tpo.LocationName);
                    slDoc.SetCellValue(iRow, 3, tpo.TPORank);
                    slDoc.SetCellValue(iRow, 4, tpo.VendorNumber);
                    slDoc.SetCellValue(iRow, 5, tpo.VendorName);
                    slDoc.SetCellValue(iRow, 6, tpo.BankType);
                    slDoc.SetCellValue(iRow, 7, tpo.BankAccountNumber);
                    slDoc.SetCellValue(iRow, 8, tpo.BankAccountName);
                    slDoc.SetCellValue(iRow, 9, tpo.BankBranch);
                    slDoc.SetCellValue(iRow, 10, tpo.Owner);
                    slDoc.SetCellValue(iRow, 11, tpo.Established.ToString(Constants.DefaultDateFormat));
                    slDoc.SetCellValue(iRow, 12, tpo.UpdatedBy);
                    slDoc.SetCellValue(iRow, 13, tpo.UpdatedDate.ToString(Constants.DefaultDateFormat));
                    slDoc.SetCellStyle(iRow, 1, iRow, 13, style);
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
                slDoc.AutoFitColumn(1, 13);

                System.IO.File.Delete(strFileName);
                slDoc.SaveAs(ms);
            }
            // this is important. Otherwise you get an empty file
            // (because you'd be at EOF after the stream is written to, I think...).
            ms.Position = 0;
            var fileName = "MasterProcess_" + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}