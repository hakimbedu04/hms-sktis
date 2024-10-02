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
using SKTISWebsite.Models.MasterGenList;
using SpreadsheetLight;
using Enums = HMS.SKTIS.Core.Enums;

namespace SKTISWebsite.Controllers
{
    public class MasterGenListController : BaseController
    {
        private IMasterDataBLL _bll;

        public MasterGenListController(IMasterDataBLL bll)
        {
            _bll = bll;
            SetPage("MasterData/General/ListGroup");
        }

        // GET: MasterList
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Get all master general list data by filter
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns>list of master general list</returns>
        [HttpPost]
        public JsonResult GetListGroups(GetMstGenListsInput criteria)
        {
            var masterLists = _bll.GetMstGeneralLists(criteria);
            var viewModel = Mapper.Map<List<MasterGenListViewModel>>(masterLists);
            var pageResult = new PageResult<MasterGenListViewModel>(viewModel, criteria);
            return Json(pageResult);
        }

        /// <summary>
        /// Get all list of listgroup
        /// </summary>
        /// <returns>list of listgroup</returns>
        [HttpGet]
        public JsonResult GetListGroupSelectList()
        {
            var model = _bll.GetListGroupsEnum();
            return Json(model, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Save all new and updated general list item
        /// </summary>
        /// <param name="bulkData"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult SaveAllListGroups(InsertUpdateData<MasterGenListViewModel> bulkData)
        {
            if (bulkData.New != null)
            {
                for (var i = 0; i < bulkData.New.Count; i++)
                {
                    //check row is null
                    if (bulkData.New[i] == null) continue;
                    var mstListGroup = Mapper.Map<MstGeneralListDTO>(bulkData.New[i]);

                    //set createdby and updatedby
                    mstListGroup.CreatedBy = GetUserName();
                    mstListGroup.UpdatedBy = GetUserName();

                    try
                    {
                        var item = _bll.InsertMstGeneralList(mstListGroup);
                        bulkData.New[i] = Mapper.Map<MasterGenListViewModel>(item);
                        bulkData.New[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Success.ToString();
                    }
                    catch (ExceptionBase ex)
                    {
                        bulkData.New[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                        bulkData.New[i].Message = ex.Message;
                    }
                    catch (Exception ex)
                    {
                        bulkData.New[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
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
                    var mstListGroup = Mapper.Map<MstGeneralListDTO>(bulkData.Edit[i]);

                    //set updatedby
                    mstListGroup.UpdatedBy = GetUserName();

                    try
                    {
                        var item = _bll.UpdateMstGeneralList(mstListGroup);
                        bulkData.Edit[i] = Mapper.Map<MasterGenListViewModel>(item);
                        bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Success.ToString();
                    }
                    catch (ExceptionBase ex)
                    {
                        bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                        bulkData.Edit[i].Message = ex.Message;
                    }
                    catch (Exception ex)
                    {
                        bulkData.Edit[i].ResponseType = HMS.SKTIS.Core.Enums.ResponseType.Error.ToString();
                        bulkData.Edit[i].Message = EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception); ;
                    }
                }
            }

            return Json(bulkData);
        }

        /// <summary>
        /// Generate excel by filter
        /// </summary>
        /// <param name="listGroup"></param>
        /// <returns></returns>
        [HttpPost]
        public FileStreamResult GenerateExcel(string listGroup)
        {
            var input = new GetMstGenListsInput { ListGroup = listGroup, SortExpression = "UpdatedDate", SortOrder = "DESC" };
            var masterListGroups = _bll.GetMstGeneralLists(input);

            MemoryStream ms = new MemoryStream();

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = Enums.ExcelTemplate.MstGeneralList + ".xlsx";
            var templateFileName = Server.MapPath(Constants.MasterDataExcelTemplatesFolder + templateFile);
            if (System.IO.File.Exists(templateFileName))
            {
                System.IO.File.Copy(templateFileName, strFileName);
            }

            using (SLDocument slDoc = new SLDocument(strFileName))
            {
                //filter values
            
                slDoc.SetCellValue(4, 2, listGroup == "" ? "All" : listGroup);

                //row values
                var iRow = 7;

                foreach (var masterListGroup in masterListGroups)
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

                    slDoc.SetCellValue(iRow, 1, masterListGroup.ListGroup);
                    slDoc.SetCellValue(iRow, 2, masterListGroup.ListDetail);
                    slDoc.SetCellValue(iRow, 3, masterListGroup.StatusActive.ToString());
                    slDoc.SetCellValue(iRow, 4, masterListGroup.Remark);
                    slDoc.SetCellValue(iRow, 5, masterListGroup.UpdatedBy);
                    slDoc.SetCellValue(iRow, 6, masterListGroup.UpdatedDate.ToString(Constants.DefaultDateFormat));
                    slDoc.SetCellStyle(iRow, 1, iRow, 6, style);
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
                slDoc.AutoFitColumn(1, 6);

                System.IO.File.Delete(strFileName);
                slDoc.SaveAs(ms);
            }
            // this is important. Otherwise you get an empty file
            // (because you'd be at EOF after the stream is written to, I think...).
            ms.Position = 0;
            var fileName = "MasterGeneralList_" + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}
