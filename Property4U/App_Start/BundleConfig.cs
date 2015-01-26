using System.Web.Optimization;

namespace IdentitySample
{
    public class BundleConfig
    {
        // For more information on bundling, visit http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/bootstrap").Include(
                      "~/Scripts/bootstrap.js"
                      ));

            bundles.Add(new ScriptBundle("~/bundles/controldeskJS").Include(
                      "~/Scripts/respond.js",
                      "~/Scripts/jquery-ui.js",
                      "~/Scripts/jquery.metisMenu.js",
                      "~/Scripts/side-menu-search.js",
                      "~/Scripts/siminta.js",
                      "~/Scripts/raphael-2.1.0.min.js",
                      "~/Scripts/morris.js",
                      "~/Scripts/jquery.timeago.js"));

            bundles.Add(new ScriptBundle("~/bundles/frontendJS").Include(
                      "~/Scripts/jquery.scrollUp.min.js",
                      "~/Scripts/price-range.js",
                      "~/Scripts/lightGallery.js",
                      "~/Scripts/main.js",
                      "~/Scripts/WishlistCompareCookie.js",
                      "~/Scripts/Wishlist.js",
                      "~/Scripts/Comparison.js"));

            bundles.Add(new ScriptBundle("~/bundles/commonCookie").Include(
                      "~/Scripts/Array.IndexOf.js",
                      "~/Scripts/jquery_cookies.js"));

            bundles.Add(new StyleBundle("~/Content/bootstrapCSS").Include(
                      "~/Content/bootstrap.css"
                ));

            bundles.Add(new StyleBundle("~/Content/controldeskCSS").Include(
                      "~/Content/site.css",
                      "~/Content/jquery-ui.css",
                      "~/Content/blue-style.css",
                      "~/Content/blue-main-style.css",
                      "~/Content/pace-theme-big-counter.css",
                      "~/Content/morris-0.4.3.min.css"));

            bundles.Add(new StyleBundle("~/Content/frontendFont").Include(
                      "~/Content/fonts/Roboto.css",
                      "~/Content/fonts/OpenSans.css",
                      "~/Content/fonts/Abel.css"));

            bundles.Add(new StyleBundle("~/Content/frontendCSS").Include(
                      "~/Content/public/lightGallery.css",
                      "~/Content/public/price-range.css",
                      "~/Content/public/animate.css",
                      "~/Content/public/main.css",
                      "~/Content/public/responsive.css"));
            
            bundles.Add(new StyleBundle("~/Content/font-awesome").Include(
                     "~/Content/fontawesome/font-awesome.min.css", new CssRewriteUrlTransform()));

            bundles.Add(new StyleBundle("~/Content/commonCSS").Include(
                      "~/Content/social-buttons.css"
                ));

            //bundles.Add(new ScriptBundle("~/bundles/knockout").Include(
            //          "~/Scripts/knockout-{version}.js"));

            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            BundleTable.EnableOptimizations = true;
        }

    }
}
