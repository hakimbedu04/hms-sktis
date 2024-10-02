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
using Microsoft.Ajax.Utilities;
using SKTISWebsite.Code;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.Factories.Contract;
using SKTISWebsite.Models.MasterBrandGroupMaterial;
using SKTISWebsite.Models.MasterGenLocations;
using SpreadsheetLight;
using Enums = HMS.SKTIS.Core.Enums;


namespace SKTISWebsite.Controllers
{
    public class MasterGenBrandGroupMaterialController : BaseController
    {
        private IMasterDataBLL _masterDataBll;
        private AbstractSelectList _selectList;
        private IApplicationService _applicationService;

        public MasterGenBrandGroupMaterialController(IMasterDataBLL masterDataBll, AbstractSelectList selectList, IApplicationService applicationService)
        {
            _masterDataBll = masterDataBll;
            _selectList = selectList;
            _applicationService = applicationService;
            SetPage("MasterData/General/BrandGroupMaterial");
        }

        // GET: MasterGenBrandGroupMaterial
        public ActionResult Index()
        {
            string[] parameters =
            {
                Enums.MasterGeneralList.MtrlUOM.ToString()
            };

            _selectList._DataSource = _masterDataBll.GetGeneralLists(parameters);

            var model = new MasterBrandGroupMaterialIndexViewModel();
            //model.LocationCodeSelectList = _applicationService.GetLocationCodeSelectList();
            model.Uom = _selectList.CreateUom();
            //model.LocationDescs = Mapper.Map<List<MasterGenLocationDesc>>(_masterDataBll.GetMstLocationLists(new GetMstGenLocationInput()));

            return View("Index", model);
        }

        [HttpPost]
        public ActionResult Get(GetBrandGroupMaterialInput input)
        {
            var pageResult = new PageResult<MasterBrandGroupMaterialData>();
            var masterList = _masterDataBll.GetBrandGroupMaterial(input);
            pageResult.TotalRecords = masterList.Count;
            pageResult.TotalPages = (masterList.Count / input.PageSize) +
                                    (masterList.Count % input.PageSize != 0 ? 1 : 0);
            var result = masterList.Skip((input.PageIndex - 1) * input.PageSize).Take(input.PageSize);
            pageResult.Results = Mapper.Map<List<MasterBrandGroupMaterialData>>(result);

            return Json(pageResult, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public JsonResult Save(InsertUpdateData<MasterBrandGroupMaterialData> bulkData)
        {
            if (bulkData.New != null)
            {
                for (int i = 0; i < bulkData.New.Count; i++)
                {
                    if (bulkData.New[i] == null) continue;

                    var brandGroupMaterialDto = Mapper.Map<BrandGroupMaterialDTO>(bulkData.New[i]);

                    brandGroupMaterialDto.CreatedBy = GetUserName();
                    brandGroupMaterialDto.UpdatedBy = GetUserName();

                    try
                    {
                        var item = _masterDataBll.InsertBrandGroupMaterial(brandGroupMaterialDto);
                        bulkData.New[i] = Mapper.Map<MasterBrandGroupMaterialData>(item);
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

                    var brandGroupMaterialDto = Mapper.Map<BrandGroupMaterialDTO>(bulkData.Edit[i]);

                    brandGroupMaterialDto.UpdatedBy = GetUserName();

                    try
                    {
                        var item = _masterDataBll.UpdateBrandGroupMaterial(brandGroupMaterialDto);
                        bulkData.Edit[i] = Mapper.Map<MasterBrandGroupMaterialData>(item);
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


        [HttpPost]
        public FileStreamResult GenerateExcel(string BrandGroupCode)
        {
            var criteria = new GetBrandGroupMaterialInput()
            {
                BrandGroupCode = BrandGroupCode,
                SortExpression = "UpdatedDate", 
                SortOrder = "DESC"
                //,Location = Location
            };

            var listBrandGroupMaterial = _masterDataBll.GetBrandGroupMaterial(criteria);

            var ms = new MemoryStream();

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = Enums.ExcelTemplate.MstBrandGroupMaterial + ".xlsx";
            var templateFileName = Server.MapPath(Constants.MasterDataExcelTemplatesFolder + templateFile);
            if (System.IO.File.Exists(templateFileName))
            {
                System.IO.File.Copy(templateFileName, strFileName);
            }

            using (SLDocument slDoc = new SLDocument(strFileName))
            {
                //filter values
                //slDoc.SetCellValue(2, 2, criteria.Location);
                slDoc.SetCellValue(2, 2, criteria.BrandGroupCode == "" ? "All" : criteria.BrandGroupCode);

                //row values
                var iRow = 5;
                foreach (var item in listBrandGroupMaterial.Select((value, index) => new { Value = value, Index = index }))
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

                    slDoc.SetCellValue(iRow, 1, item.Value.BrandGroupCode);
                    slDoc.SetCellValue(iRow, 2, item.Value.MaterialCode);
                    slDoc.SetCellValue(iRow, 3, item.Value.MaterialName);
                    slDoc.SetCellValue(iRow, 4, item.Value.Description);
                    slDoc.SetCellValue(iRow, 5, item.Value.Uom);
                    slDoc.SetCellValue(iRow, 6, item.Value.StatusActive.HasValue ? item.Value.StatusActive.Value.ToString() : "False");
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
                slDoc.AutoFitColumn(2, 7);

                System.IO.File.Delete(strFileName);
                slDoc.SaveAs(ms);
            }
            // this is important. Otherwise you get an empty file
            // (because you'd be at EOF after the stream is written to, I think...).
            ms.Position = 0;
            var fileName = "MasterBrandGroup_" + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }

        public JsonResult GetBrandGroupByLocationCode(string locationCode)
        {
            //var data = _masterDataBll.GetMasterProcessSettingLocationDto(locationCode);
            var brandGroupCodes = _applicationService.GetAllBrandGroupCodeActive();
            return Json(brandGroupCodes, JsonRequestBehavior.AllowGet);
        }
    }
}