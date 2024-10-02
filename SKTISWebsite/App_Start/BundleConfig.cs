using System.Web;
using System.Web.Optimization;

namespace SKTISWebsite
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/moment").Include(
                        "~/Scripts/moment.js"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js",
                      "~/Scripts/vendors/bootstrap-*"));

            bundles.Add(new ScriptBundle("~/bundles/knockoutjs").Include(
                        "~/Scripts/knockout-3.3.0.js",
                        "~/Scripts/knockout.mapping-latest.js",
                        "~/Scripts/vendors/boostrap.tooltip.js",
                        "~/Scripts/vendors/bootstrap-datepicker.js",
                        "~/Scripts/vendors/bootstrap-timepicker.min.js",
                        "~/Scripts/common/app.CustomBinding.js",
                        "~/Scripts/knockout.validation.js",
                        "~/Scripts/knockout.validation.custom.js"));

            bundles.Add(new ScriptBundle("~/bundles/gridviewjs").Include(
                        "~/Scripts/common/app.GridViewModel.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquerynoty").Include(
                        "~/Scripts/noty/jquery.noty.js",
                        "~/Scripts/noty/layouts/top.js",
                        "~/Scripts/noty/themes/relax.js"
                        ));

            // Main Modules
            bundles.Add(new ScriptBundle("~/bundles/app").Include(
                        "~/Scripts/main.js",
                        "~/Scripts/scripts.js"
                        ));

            bundles.Add(new StyleBundle("~/bundles/css").Include(
                "~/Content/css/main.min.css",
                "~/Content/css/vendors/bootstrap-datetimepicker.css",
                "~/Content/css/vendors/bootstrap-select.css",
                "~/Content/css/vendors/bootstrap-timepicker.min.css",
                "~/Content/css/vendors/jquery-ui.css",
                "~/Content/css/vendors/animate.css",
                "~/Content/css/vendors/jquery.tree.css"
                )
                .Include("~/Content/css/vendors/font-awesome.css", new CssRewriteUrlTransform()
                ));

            bundles.Add(new ScriptBundle("~/bundles/async").Include(
                "~/Scripts/vendors/async.min.js"
            ));

            bundles.Add(new ScriptBundle("~/bundles/numeral").Include(
                "~/Scripts/numeral.min.js",
                "~/Scripts/languages.min.js"
                ));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
            //            "~/Scripts/jquery-ui-{version}.js"));

            //bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
            //            "~/Scripts/jquery.validate*"));

            //bundles.Add(new StyleBundle("~/Content/css/vendors/FontAwesome")
            //    .Include("~/Content/css/vendors/font-awesome.css", new CssRewriteUrlTransform()
            //    ));
        }
    }
}