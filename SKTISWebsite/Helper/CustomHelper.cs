using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using SKTISWebsite.Models.MasterGenBrandPackageMapping;
using SKTISWebsite.Models.MasterGenLocations;
using SKTISWebsite.Models.Account;
using HMS.SKTIS.BusinessObjects.Outputs;

namespace SKTISWebsite.Helper
{
    public static class CustomHelper
    {
        public static string PageTitle { get; set; }

        /// <summary>
        /// set current language url
        /// </summary>
        /// <param name="urlHelper"></param>
        /// <param name="lang">language</param>
        /// <returns>url string</returns>
        public static string LanguageUrl(this UrlHelper urlHelper, string lang)
        {
            var rd = urlHelper.RequestContext.RouteData;
            var request = urlHelper.RequestContext.HttpContext.Request;
            var values = new RouteValueDictionary(rd.Values);
            foreach (string key in request.QueryString.Keys)
            {
                values[key] = request.QueryString[key];
            }
            values["language"] = lang;
            return urlHelper.RouteUrl(values);
        }

        /// <summary>
        /// Create url language link
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="url"></param>
        /// <param name="lang"></param>
        /// <returns></returns>
        public static HtmlString LanguageLink(this HtmlHelper helper, string url, string lang)
        {
            var style = lang.ToLower() == Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToLower() ? "class=\"active-lang\"" : "";
            var link = string.Format("<a href=\"{0}\" {2}>{1}</a>", url, lang, style);
            return new HtmlString(link);
        }

        /// <summary>
        /// Get current language
        /// </summary>
        /// <param name="helper"></param>
        /// <returns></returns>
        public static HtmlString CurrentLanguage(this HtmlHelper helper)
        {
            return new HtmlString(Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToUpper());
        }


        /// <summary>
        /// Set Page Title 
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="pageTitle">Title</param>
        public static void SetPageTitle(this HtmlHelper helper, string pageTitle)
        {
            //HttpContext.Current.Request.RequestContext.RouteData.Values["Title"] = titlePage;
            PageTitle = pageTitle;
        }

        public static string GetPageTitle(this HtmlHelper helper)
        {
            //var route = HttpContext.Current.Request.RequestContext.RouteData.Values["Title"];

            if (PageTitle != null)
                return "| " + PageTitle;

            return "";
        }
        /// <summary>
        /// Create tag 'td' with attribute data-column and data-row
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="sourceIndex"></param>
        /// <param name="destIndex"></param>
        /// <param name="sourceBrand"></param>
        /// <param name="destBarnd"></param>
        /// <param name="brandPackage"></param>
        /// <returns></returns>
        public static HtmlString CreatePackageMappingTable(this HtmlHelper helper, int sourceIndex, int destIndex, string sourceBrand, string destBarnd, List<BrandPkgMappingViewModel> brandPackage, string inputclass)
        {
            var classname = "";
            var datatype = "";
            var value = "";


            //for editable
            if (sourceIndex > destIndex)
            {
                classname = "class=\"current\"";

                if (sourceIndex != 0 && destIndex != 0)
                {
                    datatype = "editable";
                    var val = brandPackage.FirstOrDefault(p => p.BrandGroupCodeSource.Trim() == sourceBrand.Trim() && p.BrandGroupCodeDestination.Trim() == destBarnd.Trim());
                    value = val != null ? string.Format("<input type=\"number\" step=\"any\" data-sktis-validation=\"decimal-dot\" class=\"{1}\" value=\"{0}\"/>", val.MappingValue, inputclass)
                                        : string.Format("<input type=\"number\" step=\"any\" data-sktis-validation=\"decimal-dot\" class=\"{0}\"/>", inputclass);
                }
            }

            //value always 1
            if (sourceIndex == destIndex)
                value = "1";

            //non editable
            if (sourceIndex < destIndex)
            {
                if (sourceIndex != 0 && destIndex != 0)
                {
                    datatype = "non-editable";
                    var val = brandPackage.FirstOrDefault(p => p.BrandGroupCodeSource.Trim() == sourceBrand.Trim() && p.BrandGroupCodeDestination.Trim() == destBarnd.Trim());
                    if (val != null)
                        value = val.MappingValue.ToString();
                }
            }

            var link = string.Format("<td data-column=\"{0}\" data-row=\"{1}\" data-type=\"{2}\" {3}>{4}</td>", destBarnd, sourceBrand, datatype, classname, value);
            return new HtmlString(link);
        }

        public static HtmlString CreatePackageMappingTableUpdatedBy(this HtmlHelper helper, string sourceBrand, string destBarnd, List<BrandPkgMappingViewModel> brandPackage, string inputclass)
        {
            var classname = "";
            var datatype = "";
            var updatedBy = "";
            var updatedDate = "";

            datatype = "non-editable";
            var val = brandPackage.FirstOrDefault(p => p.BrandGroupCodeSource.Trim() == sourceBrand.Trim() && p.BrandGroupCodeDestination.Trim() == destBarnd.Trim());
            if (val != null)
            {
                updatedBy = val.UpdatedBy.ToString();
                updatedDate = val.UpdatedDate.ToString();
            }
            else
            {
                updatedBy = "System";
                updatedDate = "01/01/1970 00:00:00";
            }

            var link = string.Format("<td data-column=\"{0}\" data-row=\"{1}\" data-type=\"{2}\" {3}>{4}</td>", destBarnd, sourceBrand, datatype, classname, updatedBy);
            link += string.Format("<td data-column=\"{0}\" data-row=\"{1}\" data-type=\"{2}\" {3}>{4}</td>", destBarnd, sourceBrand, datatype, classname, updatedDate);
            return new HtmlString(link);
        }

        /// <summary>
        /// get base url
        /// </summary>
        /// <param name="helper"></param>
        /// <param name="url"></param>
        /// <returns></returns>
        public static IHtmlString BaseUrl(this HtmlHelper helper, string url = null)
        {
            var path = HttpContext.Current.Request.ApplicationPath ?? "";
            return new HtmlString("'" + HttpContext.Current.Request.Url.Scheme + "://" + HttpContext.Current.Request.Url.Authority +
                   path.TrimEnd('/') + "/" + url + "'");
        }


        public static string GetAllChildIds(string parentCode, List<MasterGenLocationDesc> locations, string liClass, string checkboxAttr)
        {
            var parents = locations.FirstOrDefault(x => x.LocationCode == parentCode);
            if (parents == null)
                return "";

            var childs = locations.Where(l => l.ParentLocationCode == parentCode).ToList();

            var li = childs.Count != 0 ? "class=\"" + liClass + "\"" : "";

            var htmlTree = new StringBuilder();
            htmlTree.Append("<li " + li + ">");

            htmlTree.Append("<div class=\"tree-contain\">");
            htmlTree.Append("<input type=\"checkbox\" " + checkboxAttr + " class=\"tree-checkbox\" data-location=\"" + parents.LocationCode + "\">");
            htmlTree.Append(string.Format("<span>{0} - {1}</span>", parents.LocationCode, parents.LocationName));
            htmlTree.Append("</div>");

            if (childs.Count != 0)
            {
                htmlTree.Append("<ul>");
                foreach (var child in childs)
                {
                    htmlTree.Append(GetAllChildIds(child.LocationCode, locations, liClass, checkboxAttr));
                }
                htmlTree.Append("</ul>");
            }

            htmlTree.Append("</li>");
            return htmlTree.ToString();
        }

        public static HtmlString CreateLocationTree(this HtmlHelper helper, string currentLocation, List<MasterGenLocationDesc> locations, string liClass, string checkboxAttr)
        {
            return new HtmlString(GetAllChildIds(currentLocation, locations, liClass, checkboxAttr));
        }

        public static string Name(this HtmlHelper htmlHelper)
        {
            var user = (UserSession)HttpContext.Current.Session["CurrentUser"];
            //return user != null ? user.EmployeeName : "";
            return user != null ? user.Name: "";
        }

        public static string Username(this HtmlHelper htmlHelper)
        {
            var user = (UserSession)HttpContext.Current.Session["CurrentUser"];
            //return user != null ? user.EmployeeName : "";
            return user != null ? user.Username: "";
        }
    }
}