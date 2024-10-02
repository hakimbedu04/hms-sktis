using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using HMS.SKTIS.Utils;
using SKTISWebsite.Models.Common;
using SKTISWebsite.Models.TPOFeeSettingCalculation;
using AutoMapper;

namespace SKTISWebsite.Controllers
{
    public class TPOFeeSettingCalculationController : BaseController
    {
        private IMasterDataBLL _masterDataBll;

        public TPOFeeSettingCalculationController(IMasterDataBLL masterDataBll)
        {
            _masterDataBll = masterDataBll;
            SetPage("MasterData/TPO/TPOFeeSetting");
        }

        // GET: TPOFeeSettingCalculation
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult GetTpoFeeSettingCalculations()
        {
            var masterLists = _masterDataBll.GetTpoFeeSettingCalculations();
            var viewModel = Mapper.Map<List<TPOFeeSettingCalculationViewModel>>(masterLists);
            var pageResult = new PageResult<TPOFeeSettingCalculationViewModel>(viewModel);
            return Json(pageResult);
        }

        [HttpPost]
        public ActionResult SaveAllListGroups(InsertUpdateData<TPOFeeSettingCalculationViewModel> bulkData)
        {
            // Update data
            if (bulkData.Edit != null)
            {
                for (var i = 0; i < bulkData.Edit.Count; i++)
                {
                    //check row is null
                    if (bulkData.Edit[i] == null) continue;
                    var mstListGroup = Mapper.Map<TPOFeeSettingCalculationDTO>(bulkData.Edit[i]);

                    mstListGroup.CreatedBy = GetUserName();
                    mstListGroup.UpdatedBy = GetUserName();

                    try
                    {
                        var item = _masterDataBll.UpdateTPOFeeSettingCalculation(mstListGroup);
                        bulkData.Edit[i] = Mapper.Map<TPOFeeSettingCalculationViewModel>(item);
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
    }
}