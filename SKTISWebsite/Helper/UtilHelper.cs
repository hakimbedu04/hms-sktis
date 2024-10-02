using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using HMS.SKTIS.BusinessObjects.Outputs;
using SKTISWebsite.Models.MasterGenBrandPackageMapping;
using SKTISWebsite.Models.MasterGenLocations;
using SKTISWebsite.Models.Account;

namespace SKTISWebsite.Helper
{
    public static class UtilHelper
    {
        public static bool AccessPage(string PageName)
        {
            var user = (UserSession)HttpContext.Current.Session["CurrentUser"];
            var res = user.Responsibility;
            if (res.Page == null)
                return false;
            var page = PageName.Split('/');
            if (page.Length == 1) {
                return res.Page.ContainsKey(PageName);                        
            }else{
                Dictionary<string, ResponsibilityPage> data = (Dictionary<string, ResponsibilityPage>) res.Page;

                for(var i = 0;i < (page.Length - 1);i++){
                    data = data[page[i]].Child;
                }
                return data.ContainsKey(page.Last());            
            }
            
        }
    }
}