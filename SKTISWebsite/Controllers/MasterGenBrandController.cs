using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
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
using SKTISWebsite.Models.MasterBrand;
using SpreadsheetLight;
using Enums = HMS.SKTIS.Core.Enums;

namespace SKTISWebsite.Controllers
{
    public class MasterGenBrandController : BaseController
    {
        private IMasterDataBLL _masterDataBll;
        private AbstractSelectList _selectList;

        public MasterGenBrandController(IMasterDataBLL masterDataBll, AbstractSelectList selectList)
        {
            _masterDataBll = masterDataBll;
            _selectList = selectList;
            SetPage("MasterData/General/Brand");
        }

        // GET: MasterBrand
        public ActionResult Index()
        {
            var model = new IndexMasterBrandViewModel();
            model.BrandGroupCode = _selectList.CreateBrandGroupCode();
            return View("Index", model);
        }

        /// <summary>
        /// Get all master general list data by criteria
        /// </summary>
        /// <param name="criteria"></param>
        /// <returns>list of master general list</returns>
        [HttpPost]
        public JsonResult Get(GetBrandInput criteria)
        {
            var pageResult = new PageResult<GetMasterBrandViewModel>();
            var masterLists = _masterDataBll.GetBrands(criteria);
            pageResult.TotalRecords = masterLists.Count;
            pageResult.TotalPages = (masterLists.Count / criteria.PageSize) +
                                    (masterLists.Count % criteria.PageSize != 0 ? 1 : 0);
            var result = masterLists.Skip((criteria.PageIndex - 1) * criteria.PageSize).Take(criteria.PageSize);
            pageResult.Results = Mapper.Map<List<GetMasterBrandViewModel>>(result);

            return Json(pageResult, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public JsonResult GetAllBrandCode()
        {
            var result = _masterDataBll.GetBrands(new GetBrandInput());
            return Json(result.Select(m => m.BrandCode), JsonRequestBehavior.AllowGet);
        }
        /// <summary>
        /// Save & Update data brand
        /// </summary>
        /// <param name="bulkData"></param>
        [HttpPost]
        public ActionResult Save(InsertUpdateData<GetMasterBrandViewModel> bulkData)
        {
            if (bulkData.New != null)
            {
                for (int i = 0; i < bulkData.New.Count; i++)
                {
                    if (bulkData.New[i] == null) continue;

                    var brandItem = Mapper.Map<BrandDTO>(bulkData.New[i]);

                    brandItem.CreatedBy = GetUserName();
                    brandItem.UpdatedBy = GetUserName();


                    try
                    {
                        _masterDataBll.InsertBrand(brandItem);
                        bulkData.New[i] = Mapper.Map<GetMasterBrandViewModel>(brandItem);
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

            if (bulkData.Edit != null)
            {
                for (int i = 0; i < bulkData.Edit.Count; i++)
                {
                    if (bulkData.Edit[i] == null) continue;

                    var brandItem = Mapper.Map<BrandDTO>(bulkData.Edit[i]);

                    brandItem.UpdatedBy = GetUserName();
                    try
                    {
                        var item = _masterDataBll.UpdateBrand(brandItem);
                        bulkData.Edit[i] = Mapper.Map<GetMasterBrandViewModel>(item);
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
        /// 
        /// </summary>
        /// <param name="BrandGroupCode"></param>
        /// <param name="BrandCode"></param>
        /// <param name="EffectiveDate"></param>
        /// <param name="ExpiredDate"></param>
        /// <returns></returns>
        [HttpPost]
        public FileStreamResult GenerateExcel(string BrandGroupCode, string BrandCode, DateTime? EffectiveDate, DateTime? ExpiredDate)
        {
            var criteria = new GetBrandInput
            {
                BrandGroupCode = BrandGroupCode,
                BrandCode = BrandCode,
                EffectiveDate = EffectiveDate,
                ExpiredDate = ExpiredDate
            };
            var listBrand = _masterDataBll.GetBrands(criteria);

            var ms = new MemoryStream();

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = Enums.ExcelTemplate.MstBrand + ".xlsx";
            var templateFileName = Server.MapPath(Constants.MasterDataExcelTemplatesFolder + templateFile);
            if (System.IO.File.Exists(templateFileName))
            {
                System.IO.File.Copy(templateFileName, strFileName);
            }

            using (SLDocument slDoc = new SLDocument(strFileName))
            {
                //filter values
                slDoc.SetCellValue(2, 2, string.IsNullOrEmpty(criteria.BrandGroupCode) ? Enums.All.All.ToString() : criteria.BrandGroupCode);
                slDoc.SetCellValue(3, 2, string.IsNullOrEmpty(criteria.BrandCode) ? Enums.All.All.ToString() : criteria.BrandCode);
                slDoc.SetCellValue(4, 2, EffectiveDate.ToString());
                slDoc.SetCellValue(5, 2, ExpiredDate.ToString());

                //row values
                var iRow = 8;

                foreach (var masterItem in listBrand.Select((value, index) => new { value = value, index = index }))
                {
                    SLStyle style = slDoc.CreateStyle();
                    style.Border.LeftBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.TopBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.RightBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Border.BottomBorder.BorderStyle = BorderStyleValues.Thin;
                    style.Font.FontName = "Calibri";
                    style.Font.FontSize = 10;

                    if (masterItem.index % 2 == 0)
                    {
                        style.Fill.SetPattern(PatternValues.Solid, System.Drawing.Color.LightGray, System.Drawing.Color.LightGray);
                    }

                    slDoc.SetCellValue(iRow, 1, masterItem.value.BrandGroupCode);
                    slDoc.SetCellValue(iRow, 2, masterItem.value.BrandCode);
                    slDoc.SetCellValue(iRow, 3, masterItem.value.Description);
                    slDoc.SetCellValue(iRow, 4, masterItem.value.EffectiveDate.ToString());
                    slDoc.SetCellValue(iRow, 5, masterItem.value.ExpiredDate.ToString());
                    slDoc.SetCellValue(iRow, 6, masterItem.value.Remark);
                    slDoc.SetCellValue(iRow, 7, masterItem.value.UpdatedBy);
                    slDoc.SetCellValue(iRow, 8, masterItem.value.UpdatedDate.ToString(Constants.DefaultDateFormat));
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
            var fileName = "MasterBrand_" + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

    }
}
