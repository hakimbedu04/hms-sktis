using System;
using System.Collections.Generic;
using System.Drawing;
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
using SKTISWebsite.Models.MasterGenBrandGroup;
using SKTISWebsite.Models.MasterGenBrandPackageMapping;
using SpreadsheetLight;
using Enums = HMS.SKTIS.Core.Enums;

namespace SKTISWebsite.Controllers
{
    public class MasterGenBrandPackageMappingController : BaseController
    {
        private IMasterDataBLL _masterDataBLL;

        public MasterGenBrandPackageMappingController(IMasterDataBLL masterDataBll)
        {
            _masterDataBLL = masterDataBll;
            SetPage("MasterData/General/BrandGroupPackageMapping");
        }


        //
        // GET: /MasterGenBrandPackageMapping/
        public ActionResult Index()
        {
            var model = new InitBrandPackageMapping
                        {
                            BrandGroupCodes = Mapper.Map<List<MasterGenBrandGroupViewModel>>(_masterDataBLL.GetActiveBrandGroups()),
                            BrandPackageMapping = Mapper.Map<List<BrandPkgMappingViewModel>>(_masterDataBLL.GetMstGenBrandPkgMappings())
                        };
            return View("Index", model);
        }

        [HttpPost]
        public ActionResult SavePackageMapping(List<BrandPkgMappingViewModel> packageMappings)
        {
            if (packageMappings == null) return Json(null);

            for (var i = 0; i < packageMappings.Count; i++)
            {
                var packageMappingDTO = Mapper.Map<MstGenBrandPkgMappingDTO>(packageMappings[i]);
                packageMappingDTO.UpdatedBy = GetUserName();
                try
                {
                    var item = _masterDataBLL.InsertUpdateMstGenBrandPkgMapping(packageMappingDTO);
                    packageMappings[i] = Mapper.Map<BrandPkgMappingViewModel>(item);
                    packageMappings[i].ResponseType = Enums.ResponseType.Success.ToString();
                }

                catch (ExceptionBase ex)
                {
                    packageMappings[i].ResponseType = Enums.ResponseType.Error.ToString();
                    packageMappings[i].Message = ex.Message;
                }
                catch (Exception ex)
                {
                    packageMappings[i].ResponseType = Enums.ResponseType.Error.ToString();
                    packageMappings[i].Message = EnumHelper.GetDescription(ExceptionCodes.BaseExceptions.unhandled_exception); ;
                }

            }

            return Json(packageMappings);
        }

        /// <summary>
        /// Generate excel by filter
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public FileStreamResult GenerateExcel()
        {

            var ms = new MemoryStream();

            string strTempFileName = Path.GetTempFileName();
            string strFileName = strTempFileName.Replace(".tmp", string.Empty) + ".xlsx";
            if (System.IO.File.Exists(strFileName))
            {
                System.IO.File.Delete(strFileName);
            }

            var templateFile = Enums.ExcelTemplate.MstBrandPackageMapping + ".xlsx";
            var templateFileName = Server.MapPath(Constants.MasterDataExcelTemplatesFolder + templateFile);
            if (System.IO.File.Exists(templateFileName))
            {
                System.IO.File.Copy(templateFileName, strFileName);
            }

            using (var slDoc = new SLDocument(strFileName))
            {
                var brandGroups = _masterDataBLL.GetActiveBrandGroups();
                var package = _masterDataBLL.GetMstGenBrandPkgMappings();

                var style = slDoc.CreateStyle();
                style.Fill.SetPattern(PatternValues.Solid, ColorTranslator.FromHtml("#1F4E78"), ColorTranslator.FromHtml("#1F4E78"));
                style.Font.FontName = "Calibri";
                style.Font.FontSize = 10;
                style.Font.FontColor = ColorTranslator.FromHtml("#FFFFFF");




                const int startColumn = 2;
                const int startRow = 5;

                slDoc.SetCellValue(startRow, startColumn, HMS.SKTIS.Application.Resources.Views.MasterGenBrandPackageMapping.MasterGenBrandPackageMapping.lblTitleBrand);
                slDoc.SetCellStyle(startRow, startColumn, style);

                var index = 1;

                foreach (var brandGroup in brandGroups)
                {

                    slDoc.SetCellStyle(startRow, startColumn + index, style);
                    slDoc.SetCellStyle(startRow + index, startColumn, style);
                    slDoc.SetCellValue(startRow, startColumn + index, brandGroup.BrandGroupCode);
                    slDoc.SetCellValue(startRow + index, startColumn, brandGroup.BrandGroupCode);

                    index++;
                }

                for (var i = 0; i < brandGroups.Count; i++)
                {
                    for (var j = 0; j < brandGroups.Count; j++)
                    {
                        if (i > j)
                        {
                            var val = package.FirstOrDefault(p => p.BrandGroupCodeSource.Trim() == brandGroups[i].BrandGroupCode.Trim() &&
                                                                  p.BrandGroupCodeDestination.Trim() == brandGroups[j].BrandGroupCode.Trim());
                            if (val != null) slDoc.SetCellValue(startRow + 1 + i, startColumn + 1 + j, val.MappingValue.ToString());

                            var Editstyle = slDoc.CreateStyle();
                            Editstyle.Fill.SetPattern(PatternValues.Solid, ColorTranslator.FromHtml("#B6DD78"), ColorTranslator.FromHtml("#B6DD78"));
                            Editstyle.Font.FontName = "Calibri";
                            Editstyle.Font.FontSize = 10;
                            Editstyle.Alignment.JustifyLastLine = true;
                            slDoc.SetCellStyle(startRow + 1 + i, startColumn + 1 + j, Editstyle);
                        }

                        if (brandGroups[i].BrandGroupCode == brandGroups[j].BrandGroupCode)
                        {
                            slDoc.SetCellValue(startRow + 1 + i, startColumn + 1 + j, 1);
                        }

                        if (i < j)
                        {
                            var val = package.FirstOrDefault(p => p.BrandGroupCodeSource.Trim() == brandGroups[i].BrandGroupCode.Trim() &&
                                                                  p.BrandGroupCodeDestination.Trim() == brandGroups[j].BrandGroupCode.Trim());
                            if (val != null) slDoc.SetCellValue(startRow + 1 + i, startColumn + 1 + j, val.MappingValue.ToString());
                        }
                    }
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
            var fileName = "MasterBrandPackageMapping_" + DateTime.Now.ToShortDateString() + ".xlsx";
            return File(ms, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
        }
    }
}