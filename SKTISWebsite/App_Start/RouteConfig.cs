using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace SKTISWebsite
{
    public class RouteConfig
    {
        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            var localizingSKT = new AutoLocalizingRoute("{language}/{controller}/{action}/{id}",
                                   new { controller = "Contract", action = "Index", id = UrlParameter.Optional },
               new { language = "^[a-z]{2}$" }
           );

            routes.Add("localizingSKT", localizingSKT);

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional, }
            );

            routes.MapRoute(
                "EquipmentFulfillment",                                                                             // Route name
                "{controller}/{action}/{param1}/{param2}/{param3}",                                                                  // URL with parameters
                new { controller = "EquipmentFulfillmentController", action = "Index", param1 = "", param2 = "", param3 = "" }    // Parameter defaults
            );

            routes.MapRoute(
                "MaintenanceEquipmentQualityInspection",                                                                             // Route name
                "{controller}/{action}/{param1}/{param2}/{param3}",                                                                  // URL with parameters
                new { controller = "MaintenanceEquipmentQualityInspectionController", action = "Index", param1 = "", param2 = "", param3 = "" }    // Parameter defaults
            );

            routes.MapRoute(
                "ExeReportDailyProduction",
                "{controller}/{action}/{param1}/{param2}/{param3}/{param4}",
                new { controller = "ExeReportDailyProductionController", action = "Index", param1 = "", param2 = "", param3 = "", param4 = "" }
            );

            routes.MapRoute(
                "ProductionCardApprovalDetail",                                                                             // Route name
                "{controller}/{action}/{param1}/{param2}/{param3}/{param4}",                                                                  // URL with parameters
                new { controller = "WagesProductionCardApprovalController", action = "Index", param1 = "", param2 = "", param3 = "", param4 = "" }    // Parameter defaults
            );

            routes.MapRoute(
                "TPOFeeExeActual",
                "{controller}/{action}/{param1}/{param2}/{param3}/{param4}",
                new { controller = "TPOFeeExeActualController", action = "Index", param1 = "", param2 = "", param3 = "", param4 = "" }
            );

            routes.MapRoute(
                "WagesEblekRelease",
                "{controller}/{action}/{param1}/{param2}/{param3}/{param4}/{param5}",
                new { controller = "WagesEblekReleaseController", action = "Index", param1 = "", param2 = "", param3 = "", param4 = "", param5 = "" }
            );

            routes.MapRoute(
                "WagesReportAbsents",
                "{controller}/{action}/{param1}/{param2}/{param3}/{param4}/{param5}",
                new { controller = "WagesReportAbsentsController", action = "WagesReportAbsentsGroup", param1 = "", param2 = "", param3 = "", param4 = "", param5 = "" }
            );

            routes.MapRoute(
               "PlanningPlantTPK",
               "{controller}/{action}/{param1}/{param2}/{param3}/{param4}/{param5}/{param6}/{param7}",
               new { controller = "PlanningPlantTPKController", action = "Index", param1 = "", param2 = "", param3 = "", param4 = "", param5 = "", param6 = "", param7 = "" }
            );

            routes.MapRoute(
                "ExeReportByProcess",
                "{controller}/{action}/{param1}/{param2}/{param3}/{param4}/{param5}/{param6}/{param7}",
                new { controller = "ExeReportByProcessController", action = "Index", param1 = "", param2 = "", param3 = "", param4 = "", param5 = "", param6 = "", param7 = "" }
            );

            routes.MapRoute(
                "EblekReleaseApproval",
                "{controller}/{action}/{param1}/{param2}/{param3}/{param4}/{param5}/{param6}/{param7}/{param8}",
                new { controller = "EblekReleaseApprovalController", action = "Index", param1 = "", param2 = "", param3 = "", param4 = "", param5 = "", param6 = "", param7 = "", param8 = "" }
            );

            routes.MapRoute(
                "ProductionCard",
                "{controller}/{action}/{param1}/{param2}/{param3}/{param4}/{param5}/{param6}/{param7}/{param8}",
                new { controller = "ProductionCardController", action = "Index", param1 = "", param2 = "", param3 = "", param4 = "", param5 = "", param6 = "", param7 = "", param8 = "" }
            );

            routes.MapRoute(
                "ExeTPOProductionEntry",
                "{controller}/{action}/{param1}/{param2}/{param3}/{param4}/{param5}/{param6}/{param7}/{param8}",
                new { controller = "ExeTPOProductionEntryController", action = "Index", param1 = "", param2 = "", param3 = "", param4 = "", param5 = "", param6 = "", param7 = "", param8 = "" }
            );

            routes.MapRoute(
                "ExePlantProductionEntry",
                "{controller}/{action}/{param1}/{param2}/{param3}/{param4}/{param5}/{param6}/{param7}/{param8}/{param9}/{param10}",
               new { controller = "ExePlantProductionEntryController", action = "Index", param1 = "", param2 = "", param3 = "", param4 = "", param5 = "", param6 = "", param7 = "", param8 = "", param9 = "", param10 = "" }
            );

            //routes.MapRoute(
            //    "TPOFeeExeActualDetail",                                                                                    // Route name
            //    "{controller}/{action}/{TPOFeeCode}/{pageFrom}",                                                            // URL with parameters
            //    new { controller = "TPOFeeExeActualDetailController", action = "Index", TPOFeeCode = "", pageFrom = "" }    // Parameter defaults
            //);
        }
        public class AutoLocalizingRoute : Route
        {
            public AutoLocalizingRoute(string url, object defaults, object constraints)
                : base(url, new RouteValueDictionary(defaults), new RouteValueDictionary(constraints), new MvcRouteHandler()) { }

            public override VirtualPathData GetVirtualPath(RequestContext requestContext, RouteValueDictionary values)
            {
                //only set the culture if it's not present in the values dictionary yet
                //this check ensures that we can link to a specific language when we need to (fe: when picking your language)
                if (!values.ContainsKey("language"))
                {
                    values["language"] = Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName;
                }

                return base.GetVirtualPath(requestContext, values);
            }
        }
    }
}
