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
using SKTISWebsite.Models.MasterGenBrandPackageItem;
using SKTISWebsite.Models.MasterMntcItemLocation;
using SpreadsheetLight;
using Enums = HMS.SKTIS.Core.Enums;

namespace SKTISWebsite.Controllers
{
    public class MasterGenBrandPackageItemController : BaseController
    {
        private IMasterDataBLL _masterDataBLL;

        public MasterGenBrandPackageItemController(IMasterDataBLL masterDataBLL)
        {
            _masterDataBLL = masterDataBLL;
            SetPage("MasterData/Maintenance/PackageEquipments");
        }


        // GET: /MasterPackageEquipment/
        public ActionResult Index()
        {
            var maintenanitems = _masterDataBLL.GetAllMaintenanceItems();
            var initPackageEquipment = new InitGenBrandPackageItem
                                       {
                                           Item = Mapper.Map<List<SelectListItem>>(maintenanitems),
                                           ItemDescription = Mapper.Map<List<MstMntcItemDescription>>(maintenanitems),
                                           BrandGroup = Mapper.Map<List<BrandCodeSelectItem>>(_masterDataBLL.GetActiveBrandGroups()),
                                       };
            return View("Index", initPackageEquipment);
        }

        /// <summary>
        /// Get all master package equipment
        /// </summary>
        /// <param name="criteria">parameter</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult GetMstPackageEquipments(MstGenBrandPackageItemInput criteria)
        {
            var pageResult = new PageResult<MasterGenBrandPackageItemViewModel>();
            var packageEquipment = _masterDataBLL.GetMstGenBrandPackageItem(criteria);

            Func<MstGenBrandPackageItemDTO, Object> orderByFunc = null;

            switch (criteria.SortExpression)
            {
                case "BrandGroupCode":
                    orderByFunc = i => i.BrandGroupCode;
                    break;
                case "ItemCode":
                    orderByFunc = i => i.ItemCode;
                    break;
                case "ItemDescription":
                    orderByFunc = i => i.ItemDescription;
                    break;
                case "Qty":
                    orderByFunc = i => i.Qty;
                    break;
                default:
                    orderByFunc = i => i.UpdatedDate;
                    break;
            }

            if (criteria.SortOrder == "DESC")
                packageEquipment = packageEquipment.OrderByDescending(orderByFunc).ToList();
            else
                packageEquipment = packageEquipment.OrderBy(orderByFunc).ToList();

            pageResult.TotalRecords = packageEquipment.Count;
            pageResult.TotalPages = (packageEquipment.Count / criteria.PageSize) + (packageEquipment.Count % criteria.PageSize != 0 ? 1 : 0);
            var result = packageEquipment.Skip((criteria.PageIndex - 1) * criteria.PageSize).Take(criteria.PageSize);
            pageResult.Results = Mapper.Map<List<MasterGenBrandPackageItemViewModel>>(result);
            return Json(pageResult);
        }

        [HttpPost]
        public ActionResult SavePackageEquipment(InsertUpdateData<MasterGenBrandPackageItemViewModel> bulkData)
        {
            if (bulkData.New != null)
                for (var i = 0; i < bulkData.New.Count; i++)
                {
                    //check row is null
                    if (bulkData.New[i] == null) continue;
                    var PackageEquipment = Mapper.Map<MstGenBrandPackageItemDTO>(bulkData.New[i]);

                    // set createdby and updatedby
                    PackageEquipment.CreatedBy = GetUserName();
                    PackageEquipment.UpdatedBy = GetUserName();

                    try
                    {
                        var item = _masterDataBLL.InsertBrandPackageItem(PackageEquipment);
                        bulkData.New[i] = Mapper.Map<MasterGenBrandPackageItemViewModel>(item);
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
                        bulkData.New[i].Message = EnumHelper.GetDescription(ExceptionCodes.BLLExceptions.KeyExist); ;
                    }
                }

            // Update data
            if (bulkData.Edit != null)
                for (var i = 0; i < bulkData.Edit.Count; i++)
                {
                    //check row is null
                    if (bulkData.Edit[i] == null) continue;

                    var PackageEquipment = Mapper.Map<MstGenBrandPackageItemDTO>(bulkData.Edit[i]);
                    PackageEquipment.UpdatedBy = GetUserName();
                    try
                    {
                        var item = _masterDataBLL.UpdateBrandPackageItem(PackageEquipment);

                        bulkData.Edit[i] = Mapper.Map<MasterGenBrandPackageItemViewModel>(item);
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
            return Json(bulkData);
        }

        [HttpPost]
        public FileStreamResult GenerateExcel(string brandGroupCode)
        {
            var input = new MstGenBrandPackageItemInput() { BrandGroupCode = brandGroupCode };
            var masterPackageEquipment = _masterDataBLL.GetMstGenBrandPackageItem(input);

            var ms = new MemoryStream();

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = Enums.ExcelTemplate.MstPackageEquipment + ".xlsx";
            var templateFileName = Server.MapPath(Constants.MasterDataExcelTemplatesFolder + templateFile);
            if (System.IO.File.Exists(templateFileName))
            {
                System.IO.File.Copy(templateFileName, strFileName);
            }

            using (SLDocument slDoc = new SLDocument(strFileName))
            {
                //filter values
                slDoc.SetCellValue(4, 2, brandGroupCode == "" ? "All" : brandGroupCode);
                //row values
                var iRow = 7;

                foreach (var masterItem in masterPackageEquipment)
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

                    slDoc.SetCellValue(iRow, 1, masterItem.BrandGroupCode);
                    slDoc.SetCellValue(iRow, 2, masterItem.ItemCode);
                    slDoc.SetCellValue(iRow, 3, masterItem.ItemDescription);
                    slDoc.SetCellValue(iRow, 4, masterItem.Qty.ToString());
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
            var fileName = "MasterPackageEquipment_" + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}