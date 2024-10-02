using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System;
using System.IO;
using System.Web.Routing;
using AutoMapper;
using DocumentFormat.OpenXml.Drawing.Charts;
using HMS.SKTIS.BusinessObjects.DTOs;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.Core;
using SKTISWebsite.Models.News;


namespace SKTISWebsite.Controllers
{
    public class NewsController : BaseController
    {
        private IMasterDataBLL _bll;

        public NewsController(IMasterDataBLL bll)
        {
            _bll = bll;
            SetPage("MasterData/NewsInfo");
            PermissionIgnore("Details");
        }

        // GET: News
        public ActionResult Index(string page)
        {
            var newsInfos = _bll.GetNewsInfos();
            var model = Mapper.Map<List<NewsInfoCompositeViewModel>>(newsInfos);
            if (page == "create")
            {
                ViewBag.message = "News Info has been created";
            }
            else if (page == "edit")
            {
                ViewBag.message = "News Info has been updated";
            }
            else {
                ViewBag.message = "";
            }
            return View(model);
        }


        // GET: News/Details/5
        public ActionResult Details(int id)
        {
            var newsInfo = _bll.GetNewsInfoById(id);
            var model = Mapper.Map<NewsInfoDetailsViewModel>(newsInfo);
            return View(model);
        }

        // GET: News/Create
        public ActionResult Create()
        {
            //var level0 = _bll.GetLocationsByLevel("SKT", 0);
            //var model = new NewsInfoLevelViewModel();
            //model.DetailLevel0 = AutoMapper.Mapper.Map<List<Level0>>(level0);

            //foreach (var datalevel0 in model.DetailLevel0)
            //{
            //    datalevel0.Level1Detail = Mapper.Map<List<Level1>>(_bll.GetLocationsByLevel(datalevel0.LocationCode, 1));

            //    foreach (var datalevel1 in datalevel0.Level1Detail)
            //    {
            //        datalevel1.Level2Detail = Mapper.Map<List<Level2>>(_bll.GetLocationsByLevel(datalevel1.LocationCode, 1));

            //        foreach (var datalevel2 in datalevel1.Level2Detail)
            //        {
            //            datalevel2.Level3Detail = Mapper.Map<List<Level3>>(_bll.GetLocationsByLevel(datalevel2.LocationCode, 1));
            //        }

            //    }
                
            //}
            try
            {

            }
            catch (Exception ex)
            {
                return RedirectToAction("Index");
            }
            var model = new NewsInfoFormModel();
            return View(model);
        }

        // POST: News/Create
        [HttpPost]
        public ActionResult Create(NewsInfoFormModel model)
        {
            try
            {
                // TODO: Add insert logic here
                if(model.ImageUpload == null){
                    model.success = false;
                    model.message = "Image Can't be Empty";
                    return View(model);
                }
                var news = AutoMapper.Mapper.Map<NewsInfoDTO>(model.Details);
                var filenamecheck = model.ImageUpload.FileName;
                var name = "";
                if (filenamecheck.Contains("\\"))
                {
                    name = filenamecheck.Split('\\')[filenamecheck.Split('\\').Length - 1];
                }
                else
                {
                    name = model.ImageUpload.FileName;
                }

                name = name.Substring(0, name.Length - name.Split('.')[name.Split('.').Length - 1].Length);
                news.Image = SaveUploadedFile(model.ImageUpload, name);
                news.UpdateBy = GetUserName();
                news.CreatedBy = GetUserName();
                news.IsSlider = true;
                news.Active = true;
                _bll.InsertNewsInfo(news);
                return RedirectToAction("Index", new { page = "create"});
            }
            catch (Exception ex)
            {
                model.success = false;
                model.message = "Insert News Info Error.";
                return View(model);
            }
        }

        // GET: News/Edit/5
        public ActionResult Edit(int id)
        {
            var dbNewsInfo = _bll.GetNewsInfoById(id);
            var model = new NewsInfoFormModel();
            model.Details = Mapper.Map<NewsInfoViewModel>(dbNewsInfo);
            return View(model);
        }

        [HttpPost]
        public ActionResult Edit(NewsInfoFormModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Edit", model);
            }

            try
            {   
                var news = AutoMapper.Mapper.Map<NewsInfoDTO>(model.Details);
                if(model.ImageUpload != null){
                    string fullPath = Request.MapPath(Constants.UploadPathNews + news.Image);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                    var filenamecheck = model.ImageUpload.FileName;
                    var name = "";
                    if (filenamecheck.Contains("\\"))
                    {
                        name = filenamecheck.Split('\\')[filenamecheck.Split('\\').Length - 1];
                    }
                    else
                    {
                        name = model.ImageUpload.FileName;
                    }
                    name = name.Substring(0, name.Length - name.Split('.')[name.Split('.').Length - 1].Length);
                    news.Image = SaveUploadedFile(model.ImageUpload, name);
                }

                news.UpdateBy = GetUserName();
                _bll.UpdateNewsInfo(news);
                return RedirectToAction("Index", new { page = "edit" });
            }
            catch (Exception ex)
            {
                //ModelState.AddModelError("Error", ex.Message);
                //// here we need to return the error message
                //model.ErrorMessage = ex.Message;
                //model.Theme = new SelectList(_bll.GetAllMerchantThemes(), "ThemeID", "ThemeName", model.ThemeId);
                model.success = false;
                model.message = "Insert News Info Error.";
                return View(model);
            }
        }
        [HttpGet]
        public JsonResult Delete(int id)
        {
            var success = true;

            try
            {
                var news = AutoMapper.Mapper.Map<NewsInfoDTO>(_bll.GetNewsInfoById(id));
                var image = news.Image;
                _bll.DeleteNewsInfo(id);
                string fullPath = Request.MapPath(Constants.UploadPathNews + image);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
            }
            catch (Exception ex)
            {
                success = false;
            }

            return Json(new { success = success, message = "Delete News Info" + (!success ? " Error" : "") }, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult Activate(int id, bool activate)
        {
            var message = "Activate";
            var success = true;
            if(!activate){
                message = "Inactivate";
            }

            try {
                var news = AutoMapper.Mapper.Map<NewsInfoDTO>(_bll.GetNewsInfoById(id));
                var image = news.Image;
                news.Active = activate;
                news.UpdateBy = GetUserName();
                _bll.UpdateNewsInfo(news);
                if(!activate){
                    string fullPath = Request.MapPath(Constants.UploadPathNews + image);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                }
                
            }catch(Exception ex){
                success = false;
            }

            return Json(new { success = success, message = message + " News Info" +(!success?" Error":"")}, JsonRequestBehavior.AllowGet);
        }
        private string SaveUploadedFile(HttpPostedFileBase file, string Filename)
        {
            if (file == null || file.FileName == "")
                return "";

            string sFileName = "";

            ////initialize folders in case deleted by an test publish profile
            //if (!Directory.Exists(Server.MapPath(Constans.PoaSK)))
            //    Directory.CreateDirectory(Server.MapPath(Constans.PoaSK));

            sFileName = Constants.UploadPathNews + Path.GetFileName(Filename + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + "_" + Path.GetExtension(file.FileName));
            string path = Server.MapPath(sFileName);

            // file is uploaded
            file.SaveAs(path);

            return Path.GetFileName(Filename + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + "_" + Path.GetExtension(file.FileName));
        }
    }
}
