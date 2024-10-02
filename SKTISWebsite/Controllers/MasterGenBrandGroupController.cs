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
using SKTISWebsite.Models.MasterGenBrandGroup;
using SpreadsheetLight;
using Enums = HMS.SKTIS.Core.Enums;

namespace SKTISWebsite.Controllers
{
    public class MasterGenBrandGroupController : BaseController
    {
        private IMasterDataBLL _masterDataBll;
        private AbstractSelectList _abstractSelectList;

        public MasterGenBrandGroupController(IMasterDataBLL masterDataBll, AbstractSelectList abstractSelectList)
        {
            _masterDataBll = masterDataBll;
            _abstractSelectList = abstractSelectList;
            SetPage("MasterData/General/BrandGroup");
        }

        // GET: MasterBrandGroup
        public ActionResult Index()
        {
            string[] parameter =
            {
                Enums.MasterGeneralList.BrandFamily.ToString(),
                Enums.MasterGeneralList.Pack.ToString(),
                Enums.MasterGeneralList.Class.ToString()
            };
            _abstractSelectList._DataSource = _masterDataBll.GetGeneralLists(parameter);

            var model = new IndexMasterBrandGroupViewModel();
            model.BrandFamily = _abstractSelectList.CreateBrandFamily();
            model.PackType = _abstractSelectList.CreatePack();
            model.ClassType = _abstractSelectList.CreateClass();

            return View("Index", model);
        }

        /// <summary>
        /// POST : Get data Master Brand Group
        /// </summary>
        /// <param name="input">GetBrandGroupInput Object</param>
        /// <returns>Json</returns>
        [HttpPost]
        public JsonResult Get(GetMstGenBrandGroupInput input)
        {
            var pageResult = new PageResult<MasterGenBrandGroupViewModel>();
            var masterList = _masterDataBll.GetBrandGroups(input);
            pageResult.TotalRecords = masterList.Count;
            pageResult.TotalPages = (masterList.Count / input.PageSize) +
                                    (masterList.Count % input.PageSize != 0 ? 1 : 0);
            var result = masterList.Skip((input.PageIndex - 1) * input.PageSize).Take(input.PageSize);
            pageResult.Results = Mapper.Map<List<MasterGenBrandGroupViewModel>>(result);

            return Json(pageResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// POST : Save or Update Data
        /// </summary>
        /// <param name="bulkData"></param>
        /// <returns>Json</returns>
        [HttpPost]
        public JsonResult Save(InsertUpdateData<MasterGenBrandGroupViewModel> bulkData)
        {
            if (bulkData.New != null)
            {
                for (int i = 0; i < bulkData.New.Count; i++)
                {
                    if (bulkData.New[i] == null) continue;

                    var brandGroupItem = Mapper.Map<MstGenBrandGroupDTO>(bulkData.New[i]);

                    brandGroupItem.CreatedBy = GetUserName();
                    brandGroupItem.UpdatedBy = GetUserName();

                    try
                    {
                        var item = _masterDataBll.InsertBrandGroup(brandGroupItem);
                        bulkData.New[i] = Mapper.Map<MasterGenBrandGroupViewModel>(item);
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

                    var brandGroupItem = Mapper.Map<MstGenBrandGroupDTO>(bulkData.Edit[i]);

                    brandGroupItem.UpdatedBy = GetUserName();

                    try
                    {
                        var item = _masterDataBll.UpdateBrandGroup(brandGroupItem);
                        bulkData.Edit[i] = Mapper.Map<MasterGenBrandGroupViewModel>(item);
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
        /// POST : Generate Excel Master Brand Group
        /// </summary>
        /// <param name="BrandFamily">string Brand Family</param>
        /// <param name="BrandGroupCode">string BrandGroupCode</param>
        /// <param name="Pack">string Pack</param>
        /// <param name="Class">string Class</param>
        /// <returns>FileStreamResult</returns>
        [HttpPost]
        public FileStreamResult GenerateExcel(string BrandFamily, string BrandGroupCode, string Pack, string Class)
        {
            var criteria = new GetMstGenBrandGroupInput
            {
                BrandFamily = BrandFamily,
                BrandGroupCode = BrandGroupCode,
                PackType = Pack,
                ClassType = Class
            };

            var listBrandGroup = _masterDataBll.GetBrandGroups(criteria).OrderByDescending(c => c.UpdatedDate);

            var ms = new MemoryStream();

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = Enums.ExcelTemplate.MstBrandGroup + ".xlsx";
            var templateFileName = Server.MapPath(Constants.MasterDataExcelTemplatesFolder + templateFile);
            if (System.IO.File.Exists(templateFileName))
            {
                System.IO.File.Copy(templateFileName, strFileName);
            }

            using (SLDocument slDoc = new SLDocument(strFileName))
            {
                //filter values
                slDoc.SetCellValue(4, 2, String.IsNullOrEmpty(criteria.BrandFamily) ? "All" : criteria.BrandFamily);
                slDoc.SetCellValue(5, 2, String.IsNullOrEmpty(criteria.BrandGroupCode) ? "All" : criteria.BrandGroupCode);
                slDoc.SetCellValue(6, 2, String.IsNullOrEmpty(criteria.PackType) ? "All" : criteria.PackType);
                slDoc.SetCellValue(7, 2, String.IsNullOrEmpty(criteria.ClassType) ? "All" : criteria.ClassType);

                //row values
                var iRow = 10;

                foreach (var masterItem in listBrandGroup)
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

                    slDoc.SetCellValue(iRow, 1, masterItem.BrandFamily);
                    slDoc.SetCellValue(iRow, 2, masterItem.PackType);
                    slDoc.SetCellValue(iRow, 3, masterItem.ClassType);
                    slDoc.SetCellValue(iRow, 4, masterItem.StickPerPack.ToString());
                    slDoc.SetCellValue(iRow, 5, masterItem.PackPerSlof.ToString());
                    slDoc.SetCellValue(iRow, 6, masterItem.SlofPerBal.ToString());
                    slDoc.SetCellValue(iRow, 7, masterItem.BalPerBox.ToString());
                    slDoc.SetCellValue(iRow, 8, masterItem.BrandGroupCode);
                    slDoc.SetCellValue(iRow, 9, masterItem.SKTBrandCode);
                    slDoc.SetCellValue(iRow, 10, masterItem.Description);
                    slDoc.SetCellValue(iRow, 11, masterItem.CigarreteWeight.ToString());
                    slDoc.SetCellValue(iRow, 12, masterItem.EmpPackage.ToString());
                    slDoc.SetCellValue(iRow, 13, masterItem.StickPerSlof.ToString());
                    slDoc.SetCellValue(iRow, 14, masterItem.StickPerBal.ToString());
                    slDoc.SetCellValue(iRow, 15, masterItem.StickPerBox.ToString());
                    slDoc.SetCellValue(iRow, 16, masterItem.StatusActive.ToString());
                    slDoc.SetCellValue(iRow, 17, masterItem.Remark);
                    slDoc.SetCellValue(iRow, 18, masterItem.UpdatedBy);
                    slDoc.SetCellValue(iRow, 19, masterItem.UpdatedDate.ToString(Constants.DefaultDateFormat));
                    slDoc.SetCellStyle(iRow, 1, iRow, 19, style);
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
                slDoc.AutoFitColumn(1, 18);

                System.IO.File.Delete(strFileName);
                slDoc.SaveAs(ms);
            }
            // this is important. Otherwise you get an empty file
            // (because you'd be at EOF after the stream is written to, I think...).
            ms.Position = 0;
            var fileName = "MasterBrandGroup_" + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        /// <summary>
        /// Gets Brand Group Code By Brand Family
        /// </summary>
        /// <param name="BrandFamily">Brand Family Name</param>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetBrandGroupByFamily(string BrandFamily)
        {
            var input = new GetMstGenBrandGroupInput()
            {
                BrandFamily = BrandFamily
            };
            var model = _masterDataBll.GetBrandGroups(input);
            return Json(model, JsonRequestBehavior.AllowGet);
        }
    }
}