using System.Web.Optimization;

namespace Orchestrator.WebUI
{
    public class BundleConfig
    {
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.UseCdn = true;

            var jqueryBundle = new ScriptBundle("~/bundles/jquery", "//ajax.googleapis.com/ajax/libs/jquery/2.1.3/jquery.min.js").Include("~/bower_components/jquery/dist/jquery.js");
            jqueryBundle.CdnFallbackExpression = "window.jQuery";
            bundles.Add(jqueryBundle);

            var angularBundle = new ScriptBundle("~/bundles/angular", "//ajax.googleapis.com/ajax/libs/angularjs/1.4.1/angular.min.js").Include("~/bower_components/angular/angular.js");
            angularBundle.CdnFallbackExpression = "window.angular";
            bundles.Add(angularBundle);

            bundles.Add(new ScriptBundle("~/bundles/corelibs")
              .Include("~/bower_components/underscore/underscore.js")
              .Include("~/bower_components/moment/moment.js")
            );

            bundles.Add(new ScriptBundle("~/bundles/proteo.shared.api")
                .Include("~/bower_components/angular-resource/angular-resource.js")
                .Include("~/bower_components/angular-cookie/angular-cookie.js")
                .Include("~/ng-shared/proteo-shared-api.js")
                .IncludeDirectory("~/ng-shared/api", "*.js", true)
            );

            bundles.Add(new ScriptBundle("~/bundles/proteo.shared.forms")
                .Include("~/bower_components/angular-bootstrap/ui-bootstrap.js")
                .Include("~/bower_components/angular-bootstrap/ui-bootstrap-tpls.js")
                .Include("~/bower_components/angular-sanitize/angular-sanitize.js")
                .Include("~/bower_components/ng-currency/src/ng-currency.js")
                .Include("~/bower_components/ngtoast/dist/ngToast.js")
                .Include("~/bower_components/angular-animate/angular-animate.js")
                .Include("~/ng-shared/proteo-shared-forms.js")
                .IncludeDirectory("~/ng-shared/forms", "*.js", true)
            );

            /***** IMPORTANT: whenever changes are made to the files that are included here make sure to make any appropriate corresponding change in WebUI.Tests/chutzpah.json ***/

            bundles.Add(new ScriptBundle("~/bundles/lib")
                .Include("~/bower_components/underscore/underscore.js")
                .Include("~/bower_components/angular-animate/angular-animate.js")
                .Include("~/bower_components/angular-bootstrap-switch/dist/angular-bootstrap-switch.js")
                .Include("~/bower_components/angular-messages/angular-messages.js")
                .Include("~/bower_components/angular-resource/angular-resource.js")
                .Include("~/bower_components/angular-sanitize/angular-sanitize.js")
                .Include("~/bower_components/angular-webstorage/angular-webstorage.js")
                .Include("~/bower_components/angular-cookie/angular-cookie.js")
                .Include("~/bower_components/angular-bootstrap/ui-bootstrap.js")
                .Include("~/bower_components/angular-bootstrap/ui-bootstrap-tpls.js")
                .Include("~/bower_components/angular-underscore-module/angular-underscore-module.js")
                .Include("~/bower_components/angular-ui-router/release/angular-ui-router.js")
                .Include("~/bower_components/angular-messages/angular-messages.js")
                .Include("~/bower_components/angular-loading-bar/build/loading-bar.js")
                .Include("~/bower_components/angular-fullscreen/src/angular-fullscreen.js")
                .Include("~/bower_components/angular-tree-control/angular-tree-control.js")
                .Include("~/bower_components/bootstrap-switch/dist/js/bootstrap-switch.js")
                .Include("~/bower_components/moment/moment.js")
                .Include("~/bower_components/moment-duration-format/lib/moment-duration-format.js")
                .Include("~/bower_components/moment-timezone/moment-timezone.js")
                .Include("~/bower_components/jquery-timepicker-jt/jquery.timepicker.js")
                .Include("~/bower_components/angular-jquery-timepicker/src/timepickerdirective.js")
                .Include("~/bower_components/ng-context-menu/dist/ng-context-menu.js")
                .Include("~/bower_components/ng-table/ng-table.js")
                .Include("~/bower_components/ng-resize/ngresize.js")
                .Include("~/bower_components/ladda/js/spin.js")
                .Include("~/bower_components/ladda/js/ladda.js")
                .Include("~/bower_components/angular-ladda/dist/angular-ladda.js")
                .Include("~/bower_components/tinycolor/tinycolor.js")
                .Include("~/bower_components/tinygradient/tinygradient.js")               
                .Include("~/bower_components/angular-datatables/dist/angular-datatables.js")
                .Include("~/bower_components/angular-datatables/dist/plugins/bootstrap/angular-datatables.bootstrap.js")
                .Include("~/bower_components/angular-bootstrap-checkbox/angular-bootstrap-checkbox.js")
                .Include("~/script/datatables/media/js/jquery.datatables.js")
                .Include("~/bower_components/angular-bootstrap-checkbox/angular-bootstrap-checkbox.js")
                .Include("~/script/dhtmlxscheduler.js")
                .Include("~/script/dhtmlxscheduler_limit.js")
                .Include("~/script/dhtmlxscheduler_timeline.js")
                .Include("~/script/dhtmlxscheduler_tooltip.js")
                .Include("~/script/dhtmlxscheduler_collision.js")
                .Include("~/script/dhtmlxscheduler_readonly.js")
                .Include("~/script/dhtmlxscheduler_editors.js")

            );

            bundles.Add(new ScriptBundle("~/bundles/app")
                .Include("~/ng/app.js")
                .IncludeDirectory("~/ng/controllers", "*.js", true)
                .IncludeDirectory("~/ng/directives", "*.js", true)
                .IncludeDirectory("~/ng/services", "*.js", true)
                .IncludeDirectory("~/ng/filters", "*.js", true)
                //Commented out cause isn't being used and breaks when deployed to box
                //.IncludeDirectory("~/ng/models", "*.js", true)
            );

            bundles.Add(new ScriptBundle("~/bundles/app-config-run")
                .Include(
                    "~/ng/app-config.js",
                    "~/ng/app-run.js")
            );

            // Used to fix relative paths in the CSS files during bundling/minification
            IItemTransform cssFixer = new CssRewriteUrlTransformFixed();

            bundles.Add(new StyleBundle("~/bundles/styles/lib")
                .Include("~/bower_components/bootstrap/dist/css/bootstrap.css", cssFixer)
                .Include("~/bower_components/bootstrap-switch/dist/css/bootstrap3/bootstrap-switch.css", cssFixer)
                .Include("~/Content/Css/dhtmlxscheduler_flat.css", cssFixer)
                .Include("~/bower_components/ladda/dist/ladda-themeless.min.css")
                .Include("~/bower_components/jquery-timepicker-jt/jquery.timepicker.css", cssFixer)
                .Include("~/bower_components/angular-loading-bar/build/loading-bar.css", cssFixer)
                .Include("~/bower_components/angular-tree-control/css/tree-control.css", cssFixer)
                .Include("~/bower_components/angular-tree-control/css/tree-control-attribute.css", cssFixer)
                .Include("~/bower_components/angular-datatables/dist/plugins/bootstrap/datatables.bootstrap.min.css", cssFixer)
            );

            bundles.Add(new StyleBundle("~/bundles/styles/proteo.shared.forms")
                .Include("~/bower_components/bootstrap/dist/css/bootstrap.css", cssFixer)
                .Include("~/bower_components/ngtoast/dist/ngToast.css", cssFixer)
                .Include("~/bower_components/ngtoast/dist/ngToast-animations.css", cssFixer)
            );

            bundles.Add(new LessBundle("~/bundles/styles/app")
                .Include("~/Content/Less/site.less", cssFixer)
            );

#if DEBUG
            // Set EnableOptimizations to false for debugging. For more information,
            // visit http://go.microsoft.com/fwlink/?LinkId=301862
            BundleTable.EnableOptimizations = false;
#endif
        }
    }
}