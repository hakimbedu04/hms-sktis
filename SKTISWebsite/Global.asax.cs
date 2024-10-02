using System;
using System.Globalization;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using HMS.SKTIS.BLL;
using HMS.SKTIS.BLL.TPOFeeBLL;
using HMS.SKTIS.Contracts;
using HMS.SKTIS.DAL;
using SimpleInjector;
using SimpleInjector.Integration.Web;
using SimpleInjector.Integration.Web.Mvc;
using SKTISWebsite.Code;
using SKTISWebsite.Models.Factories.Concreate;
using SKTISWebsite.Models.Factories.Contract;
using HMS.SKTIS.BLL.UtilitiesBLL;
using HMS.SKTIS.BLL.ExecutionBLL;
using HMS.SKTIS.BLL.PlantWagesBLL;
using System.Web.Configuration;

namespace SKTISWebsite
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);


            SKTISWebsiteMapper.Initialize();
            BLLMapper.Initialize();

            var webLifestyle = new WebRequestLifestyle();

            var container = new Container();
            container.Register<IUnitOfWork, SqlUnitOfWork>(webLifestyle);
            container.Register<IApplicationService, ApplicationService>();
            container.Register<IMasterDataBLL, MasterDataBLL>();
            container.Register<IPlanningBLL, PlanningBLL>();
            container.Register<IMaintenanceBLL, MaintenanceBLL>();
            container.Register<ISelectListBLL, SelectListBLL>();
            container.Register<IUserBLL, UserBLL>();
            container.Register<IFormsAuthentication, FormsAuthenticationService>();
            container.Register<AbstractSelectList, ConcreteSelectList>();
            container.Register<IUploadService, UploadService>();
            container.Register<ISSISPackageService, SSISService>();
            container.Register<IGeneralBLL, GeneralBLL>();
            container.Register<IUtilitiesBLL, UtilitiesBLL>();
            container.Register<IExecutionPlantBLL, ExecutionPlantBLL>();
            container.Register<IExecutionTPOBLL, ExecutionTPOBLL>();
            container.Register<IExecutionOtherBLL, ExecutionOtherBLL>();
            container.Register<IExeReportBLL, ExeReportBLL>();
            container.Register<IPlantWagesExecutionBLL, PlantWagesExecutionBLL>();
            container.Register<ITPOFeeBLL, TPOFeeBLL>();
            container.Register<ITPOFeeExeGLAccruedBLL, TPOFeeExeGLAccruedBLL>();
            container.Register<IExeReportProdStockProcessBLL, ExeReportProdStockProcessBLL>();
            container.Register<IVTLogger, VTLogger>();
            container.Register<IExeReportByStatusBLL, ExeReportByStatusBLL>();
            container.Register<IEmailApprovalsBLL, EmailApprovalsBLL>();
            container.Register<IReportByStatusService, ReportByStatusService>();
            container.Verify();
            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
        }

        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            var currentContext = new HttpContextWrapper(HttpContext.Current);
            RouteData routeData = RouteTable.Routes.GetRouteData(currentContext);

            // no route data for static files
            if (routeData == null)
                return;

            object routeCulture = routeData != null ? routeData.Values["language"] : null;
            HttpCookie languageCookie = HttpContext.Current.Request.Cookies["lang"];
            //List<string> userLanguages = GetWebsiteLanguages();

            // in following order,
            // get language from route
            // get language from cookie
            // get default language for current merchant
            string language =
                (routeCulture != null
                    ? routeCulture.ToString()
                    : (languageCookie != null
                        ? languageCookie.Value : "id"));
            var globalization = WebConfigurationManager.GetSection("system.web/globalization") as GlobalizationSection;
            var cultureInfo = CultureInfo.GetCultureInfoByIetfLanguageTag(globalization.Culture);

            //var cultureInfo = new CultureInfo(language);
            routeData.Values["language"] = language;

            Thread.CurrentThread.CurrentUICulture = cultureInfo;
            Thread.CurrentThread.CurrentCulture = CultureInfo.CreateSpecificCulture(cultureInfo.Name);
        }
    }
}
