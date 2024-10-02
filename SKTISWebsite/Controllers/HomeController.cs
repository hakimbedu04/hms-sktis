using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using AutoMapper;
using DocumentFormat.OpenXml.Spreadsheet;
using HMS.SKTIS.Contracts;
using SKTISWebsite.Models.Home;
using HMS.SKTIS.BusinessObjects.Inputs;
using SKTISWebsite.Models.News;
using System;
using HMS.SKTIS.BusinessObjects.Outputs;

namespace SKTISWebsite.Controllers
{
    public class HomeController : BaseController
    {
        private IMasterDataBLL _bll;
        private IUtilitiesBLL _util;

        public HomeController(IMasterDataBLL bll,IUtilitiesBLL util)
        {
            _bll = bll;
            _util = util;
            SetPage("Homepage");
        }

        //
        // GET: /Home/
        public ActionResult Index(InitHomeViewModel init)
        {
            //var masterProcessModel = _masterData.GetAllMasterProcess()
            //          .Select(AutoMapper.Mapper.Map<MasterProcessModel>).ToList();
            //return View("Index", masterProcessModel);
            var session = (UserSession)System.Web.HttpContext.Current.Session["CurrentUser"];
            var unAuthorizedUser = session.Responsibility.Location;
            if (unAuthorizedUser == null) return View("Index", init);
            var count = unAuthorizedUser.Count();
            if (count == 0)
            {
                Session.Clear();
                //return RedirectToAction("UserNotAuthorized", "Error");
                init.IsRuleEmpty = true;
                return View("index", init);
            }

            return View("index", init);
        }

        [HttpPost]
        public ActionResult SettingResponsibility(int IDResponsibility)
        {
            var status = _util.GetValidStatusResponsibility(IDResponsibility, CurrentUser.Username);
            if (status)
            {
                setResponsibility(IDResponsibility);
                return RedirectToAction("index");
            }
            else
            {
                Session.Clear();
                var init = new InitHomeViewModel
                {
                    IsExpired = true
                };
                return RedirectToAction("index", init);
            }
        }

        [HttpGet]
        public ActionResult SettingResponsibilityManual(int id)
        {
            var status = _util.GetValidStatusResponsibility(id, CurrentUser.Username);
            if (status)
            {
                setResponsibility(id);
                return RedirectToAction("index");
            }
            else
            {
                Session.Clear();
                var init = new InitHomeViewModel
                {
                    IsExpired = true
                };
                return RedirectToAction("index", init);
            }
        }

        public ActionResult HomepageSlider()
        {
            var sliderNewsInfos = _bll.GetSliderNewsInfo(CurrentUserLocation.Code);
            var model = Mapper.Map<List<SliderNewsInfoViewModel>>(sliderNewsInfos);
            return PartialView("HomepageSlider", model);
        }

        public ActionResult HomepageNewsInfo()
        {
            var NewsInfos = _bll.GetNewsInfosHome();
            var model = Mapper.Map<List<NewsInfoCompositeViewModel>>(NewsInfos);
            return PartialView("HomepageNewsInfo", model);
        }

        public ActionResult GetHolidayInformation()
        {
            var TPOPackages = _bll.GetHolidayInformation(CurrentUserLocation.Code);            
            return Json(TPOPackages, JsonRequestBehavior.AllowGet);
        }
        
        
 	    public ActionResult HomepageInformatips()
        {
            var InformatipsNewsInfos = _bll.GetInformatipsNewsInfo(CurrentUserLocation.Code);
            var model = Mapper.Map<InformatipsNewsInfoViewModel>(InformatipsNewsInfos);
            return PartialView("HomepageInformatips", model);
        }
        //
        // GET: /Home/Details/5
        //public ActionResult Details(int id)
        //{
        //    return View();
        //}

        ////
        //// GET: /Home/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        ////
        //// POST: /Home/Create
        //[HttpPost]
        //public ActionResult Create(FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add insert logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        ////
        //// GET: /Home/Edit/5
        //public ActionResult Edit(int id)
        //{
        //    return View();
        //}

        ////
        //// POST: /Home/Edit/5
        //[HttpPost]
        //public ActionResult Edit(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add update logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}

        ////
        //// GET: /Home/Delete/5
        //public ActionResult Delete(int id)
        //{
        //    return View();
        //}

        ////
        //// POST: /Home/Delete/5
        //[HttpPost]
        //public ActionResult Delete(int id, FormCollection collection)
        //{
        //    try
        //    {
        //        // TODO: Add delete logic here

        //        return RedirectToAction("Index");
        //    }
        //    catch
        //    {
        //        return View();
        //    }
        //}
    }
}
