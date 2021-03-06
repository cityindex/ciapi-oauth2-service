﻿using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using BundleTransformer.Core.Orderers;
using BundleTransformer.Core.Transformers;

namespace BootstrapSupport
{
    public class BootstrapBundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            var cssTransformer = new CssTransformer();
            var jsTransformer = new JsTransformer();
            var nullOrderer = new NullOrderer();

            bundles.Add(new ScriptBundle("~/content/js").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/jquery-migrate-{version}.js",
                "~/Content/vendor/bootstrap/js/bootstrap.js",
                "~/Scripts/jquery.validate.js",
                "~/scripts/jquery.validate.unobtrusive.js",
                "~/Scripts/jquery.validate.unobtrusive-custom-for-bootstrap.js"
                ));

            bundles.Add(new ScriptBundle("~/scripts/modernizr").Include(
                "~/Scripts/modernizr-{version}.js"
                ));

            bundles.Add(new StyleBundle("~/content/css").Include(
                "~/Content/vendor/bootstrap/css/bootstrap.css",
                "~/Content/vendor/bootstrap/css/bootstrap-mvc-validation.css"
                ));

            var lessBundle = new Bundle("~/content/less").Include("~/Content/less/ciauth.less");
            lessBundle.Transforms.Add(cssTransformer);
            lessBundle.Orderer = nullOrderer;

            bundles.Add(lessBundle);

      //      BundleTable.EnableOptimizations = true;
        }
    }
}