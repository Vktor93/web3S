using System.Web;
using System.Web.Optimization;
using WebGrupo3S.Helpers;

namespace WebGrupo3S
{
    public class BundleConfig
    {
        // Para obtener más información sobre Bundles, visite http://go.microsoft.com/fwlink/?LinkId=301862
        public static void RegisterBundles(BundleCollection bundles)
        {
            RegisterHome(bundles);

            RegisterUsuarios(bundles);

            RegisterShared(bundles);

            RegisterCalendar(bundles);

            RegisterMailbox(bundles);

            RegisterLayout(bundles);

            RegisterTables(bundles);

            bundles.Add(new StyleBundle("~/Content/css").Include(
            "~/Content/bootstrap/css",
            "~/Content/themes/base/all.css",
            "~/Content/site.css"));

            bundles.Add(new ScriptBundle("~/bundles/modalform").Include(
            "~/Scripts/modalform.js"));

            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
             "~/Scripts/jquery-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryui").Include(
             "~/Scripts/jquery-ui-{version}.js"));

            bundles.Add(new ScriptBundle("~/bundles/jqueryval").Include(
                        "~/Scripts/jquery.validate*"));

            // Use the development version of Modernizr to develop with and learn from. Then, when you're
            // ready for production, use the build tool at http://modernizr.com to pick only the tests you need.
            bundles.Add(new ScriptBundle("~/bundles/modernizr").Include(
                        "~/Scripts/modernizr-*"));

            bundles.Add(new ScriptBundle("~/bundles/respond").Include(
                        "~/Scripts/respond.js"));

            bundles.Add(new ScriptBundle("~/bundles/table").Include(
                    "~/Scripts/Tables/jquery.dataTables.min.js",
                    "~/Scripts/Tables/dataTables.buttons.min.js",
                    "~/Scripts/Tables/buttons.html5.min.js",
                    "~/Scripts/Tables/buttons.colVis.min.js",
                    "~/Scripts/Tables/jszip.min.js",
                    "~/Scripts/Tables/pdfmake.min.js",
                    "~/Scripts/Tables/vfs_fonts.js"                    
                ));

            bundles.Add(new StyleBundle("~/Content/table").Include(
                //"~/Content/Tables/jquery.dataTables.min.css",
                //"~/Content/Tables/jquery.dataTables_themeroller.css",
                "~/Content/Tables/buttons.dataTables.min.css"
                //"~/Content/Tables/responsive.dataTables.min.css"
                //"~/Content/Tables/dataTables.bootstrap.min.css"

            ));

        }

        private static void RegisterTables(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/Scripts/Tables/Simple/menu").Include(
                "~/Scripts/Tables/Simple-menu.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Tables/Data").Include(
                "~/Scripts/Tables/Data.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Tables/Data/menu").Include(
                "~/Scripts/Tables/Data-menu.js"));
        }


        private static void RegisterHome(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/Scripts/Home/DashboardV1").Include(
                "~/Scripts/Home/DashboardV1.js"));
            bundles.Add(new ScriptBundle("~/Scripts/Home/DashboardV1/menu").Include(
                "~/Scripts/Home/DashboardV1-menu.js"));
        }

        private static void RegisterShared(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/Scripts/Shared/_Layout").Include(
                "~/Scripts/Shared/_Layout.js"));
        }

        private static void RegisterUsuarios(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/Scripts/Usuarios/Usuario").Include(
                "~/Scripts/Usuarios/Usuario.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Usuarios/Usuario/menu").Include(
                "~/Scripts/Usuarios/Usuario-menu.js"));
        }

        private static void RegisterMailbox(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/Scripts/Mailbox/Inbox").Include(
                "~/Scripts/Mailbox/Inbox.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Mailbox/Inbox/menu").Include(
                "~/Scripts/Mailbox/Inbox-menu.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Mailbox/Compose").Include(
                "~/Scripts/Mailbox/Compose.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Mailbox/Compose/menu").Include(
               "~/Scripts/Mailbox/Compose-menu.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Mailbox/Read/menu").Include(
                "~/Scripts/Mailbox/Read-menu.js"));
        }

        private static void RegisterCalendar(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/Scripts/Calendar").Include(
                "~/Scripts/Calendar/Calendar.js"));

            bundles.Add(new ScriptBundle("~/Scripts/Calendar/menu").Include(
                "~/Scripts/Calendar/Calendar-menu.js"));
        }


        private static void RegisterLayout(BundleCollection bundles)
        {
            // bootstrap
            bundles.Add(new ScriptBundle("~/Admin/bootstrap/js").Include(
                "~/Admin/bootstrap/js/bootstrap.min.js"));

            // dist
            bundles.Add(new ScriptBundle("~/Admin/dist/js").Include(
                "~/Admin/dist/js/app.js"));

            bundles.Add(new StyleBundle("~/Admin/bootstrap-min/css").Include(
                "~/Admin/bootstrap/css/bootstrap.min.css"));

            bundles.Add(new StyleBundle("~/Admin/bootstrap/css").Include("~/Admin/bootstrap/css/bootstrap.css"));

            bundles.Add(new StyleBundle("~/Admin/dist/css").Include(
                "~/Admin/dist/css/admin-lte.min.css"));

            bundles.Add(new StyleBundle("~/Admin/dist/css/skins").Include(
                "~/Admin/dist/css/skins/_all-skins.min.css"));

            // documentation
            bundles.Add(new ScriptBundle("~/Admin/documentation/js").Include(
                "~/Admin/documentation/js/docs.js"));

            bundles.Add(new StyleBundle("~/Admin/documentation/css").Include(
                "~/Admin/documentation/css/style.css"));

            // plugins | bootstrap-slider
            bundles.Add(new ScriptBundle("~/Admin/plugins/bootstrap-slider/js").Include(
                                        "~/Admin/plugins/bootstrap-slider/js/bootstrap-slider.js"));

            bundles.Add(new StyleBundle("~/Admin/plugins/bootstrap-slider/css").Include(
                                        "~/Admin/plugins/bootstrap-slider/css/slider.css"));

            // plugins | bootstrap-wysihtml5
            bundles.Add(new ScriptBundle("~/Admin/plugins/bootstrap-wysihtml5/js").Include(
                                         "~/Admin/plugins/bootstrap-wysihtml5/js/bootstrap3-wysihtml5.all.min.js"));

            bundles.Add(new StyleBundle("~/Admin/plugins/bootstrap-wysihtml5/css").Include(
                                        "~/Admin/plugins/bootstrap-wysihtml5/css/bootstrap3-wysihtml5.min.css"));

            // plugins | ckeditor
            bundles.Add(new ScriptBundle("~/Admin/plugins/ckeditor/js").Include(
                                         "~/Admin/plugins/ckeditor/js/ckeditor.js"));

            // plugins | colorpicker
            bundles.Add(new ScriptBundle("~/Admin/plugins/colorpicker/js").Include(
                                         "~/Admin/plugins/colorpicker/js/bootstrap-colorpicker.min.js"));

            bundles.Add(new StyleBundle("~/Admin/plugins/colorpicker/css").Include(
                                        "~/Admin/plugins/colorpicker/css/bootstrap-colorpicker.css"));

            // plugins | datatables
            bundles.Add(new ScriptBundle("~/Admin/plugins/datatables/js").Include(
                                         "~/Admin/plugins/datatables/js/jquery.dataTables.min.js",
                                         "~/Admin/plugins/datatables/js/dataTables.bootstrap.min.js"));

            bundles.Add(new StyleBundle("~/Admin/plugins/datatables/css").Include(
                                        "~/Admin/plugins/datatables/css/dataTables.bootstrap.css"));

            // plugins | datepicker
            bundles.Add(new ScriptBundle("~/Admin/plugins/datepicker/js").Include(
                                         "~/Admin/plugins/datepicker/js/bootstrap-datepicker.js",
                                         "~/Admin/plugins/datepicker/js/locales/bootstrap-datepicker*"));

            bundles.Add(new StyleBundle("~/Admin/plugins/datepicker/css").Include(
                                        "~/Admin/plugins/datepicker/css/datepicker3.css"));

            // plugins | daterangepicker
            bundles.Add(new ScriptBundle("~/Admin/plugins/daterangepicker/js").Include(
                                         "~/Admin/plugins/daterangepicker/js/moment.min.js",
                                         "~/Admin/plugins/daterangepicker/js/daterangepicker.js"));

            bundles.Add(new StyleBundle("~/Admin/plugins/daterangepicker/css").Include(
                                        "~/Admin/plugins/daterangepicker/css/daterangepicker-bs3.css"));

            // plugins | fastclick
            bundles.Add(new ScriptBundle("~/Admin/plugins/fastclick/js").Include(
                                         "~/Admin/plugins/fastclick/js/fastclick.min.js"));

            // plugins | flot
            bundles.Add(new ScriptBundle("~/Admin/plugins/flot/js").Include(
                                         "~/Admin/plugins/flot/js/jquery.flot.min.js",
                                         "~/Admin/plugins/flot/js/jquery.flot.resize.min.js",
                                         "~/Admin/plugins/flot/js/jquery.flot.pie.min.js",
                                         "~/Admin/plugins/flot/js/jquery.flot.categories.min.js"));

            // plugins | font-awesome
            bundles.Add(new StyleBundle("~/Admin/plugins/font-awesome/css").Include(
                                        "~/Admin/plugins/font-awesome/css/font-awesome.min.css"));

            // plugins | fullcalendar
            bundles.Add(new ScriptBundle("~/Admin/plugins/fullcalendar/js").Include(
                                         "~/Admin/plugins/fullcalendar/js/fullcalendar.min.js"));

            bundles.Add(new StyleBundle("~/Admin/plugins/fullcalendar/css").Include(
                                        "~/Admin/plugins/fullcalendar/css/fullcalendar.min.css"));

            bundles.Add(new StyleBundle("~/Admin/plugins/fullcalendar/css/print").Include(
                                        "~/Admin/plugins/fullcalendar/css/print/fullcalendar.print.css"));

            // plugins | icheck
            bundles.Add(new ScriptBundle("~/Admin/plugins/icheck/js").Include(
                                         "~/Admin/plugins/icheck/js/icheck.min.js"));

            bundles.Add(new StyleBundle("~/Admin/plugins/icheck/css").Include(
                                        "~/Admin/plugins/icheck/css/all.css"));

            bundles.Add(new StyleBundle("~/Admin/plugins/icheck/css/flat").Include(
                                        "~/Admin/plugins/icheck/css/flat/flat.css"));

            bundles.Add(new StyleBundle("~/Admin/plugins/icheck/css/sqare/blue").Include(
                                        "~/Admin/plugins/icheck/css/sqare/blue.css"));

            // plugins | input-mask
            bundles.Add(new ScriptBundle("~/Admin/plugins/input-mask/js").Include(
                                         "~/Admin/plugins/input-mask/js/jquery.inputmask.js",
                                         "~/Admin/plugins/input-mask/js/jquery.inputmask.date.extensions.js",
                                         "~/Admin/plugins/input-mask/js/jquery.inputmask.extensions.js"));

            // plugins | ionicons
            bundles.Add(new StyleBundle("~/Admin/plugins/ionicons/css").Include(
                                        "~/Admin/plugins/ionicons/css/ionicons.min.css"));

            // plugins | ionslider
            bundles.Add(new ScriptBundle("~/Admin/plugins/ionslider/js").Include(
                                         "~/Admin/plugins/ionslider/js/ion.rangeSlider.min.js"));

            bundles.Add(new StyleBundle("~/Admin/plugins/ionslider/css").Include(
                                        "~/Admin/plugins/ionslider/css/ion.rangeSlider.css",
                                        "~/Admin/plugins/ionslider/css/ion.rangeSlider.skinNice.css"));

            // plugins | jquery
            bundles.Add(new ScriptBundle("~/Admin/plugins/jquery/js").Include(
                                         "~/Admin/plugins/jquery/js/jQuery-2.1.4.min.js"));

            // plugins | jquery-validate
            bundles.Add(new ScriptBundle("~/Admin/plugins/jquery-validate/js").Include(
                                         "~/Admin/plugins/jquery-validate/js/jquery.validate*"));

            // plugins | jquery-ui
            bundles.Add(new ScriptBundle("~/Admin/plugins/jquery-ui/js").Include(
                                         "~/Admin/plugins/jquery-ui/js/jquery-ui.min.js"));

            // plugins | jvectormap
            bundles.Add(new ScriptBundle("~/Admin/plugins/jvectormap/js").Include(
                                         "~/Admin/plugins/jvectormap/js/jquery-jvectormap-1.2.2.min.js",
                                         "~/Admin/plugins/jvectormap/js/jquery-jvectormap-world-mill-en.js"));

            bundles.Add(new StyleBundle("~/Admin/plugins/jvectormap/css").Include(
                                        "~/Admin/plugins/jvectormap/css/jquery-jvectormap-1.2.2.css"));

            // plugins | knob
            bundles.Add(new ScriptBundle("~/Admin/plugins/knob/js").Include(
                                         "~/Admin/plugins/knob/js/jquery.knob.js"));

            // plugins | morris
            bundles.Add(new StyleBundle("~/Admin/plugins/morris/css").Include(
                                        "~/Admin/plugins/morris/css/morris.css"));

            // plugins | momentjs
            bundles.Add(new ScriptBundle("~/Admin/plugins/momentjs/js").Include(
                                         "~/Admin/plugins/momentjs/js/moment.min.js"));

            // plugins | pace
            bundles.Add(new ScriptBundle("~/Admin/plugins/pace/js").Include(
                                         "~/Admin/plugins/pace/js/pace.min.js"));

            bundles.Add(new StyleBundle("~/Admin/plugins/pace/css").Include(
                                        "~/Admin/plugins/pace/css/pace.min.css"));

            // plugins | slimscroll
            bundles.Add(new ScriptBundle("~/Admin/plugins/slimscroll/js").Include(
                                         "~/Admin/plugins/slimscroll/js/jquery.slimscroll.min.js"));

            // plugins | sparkline
            bundles.Add(new ScriptBundle("~/Admin/plugins/sparkline/js").Include(
                                         "~/Admin/plugins/sparkline/js/jquery.sparkline.min.js"));

            // plugins | timepicker
            bundles.Add(new ScriptBundle("~/Admin/plugins/timepicker/js").Include(
                                         "~/Admin/plugins/timepicker/js/bootstrap-timepicker.min.js"));

            bundles.Add(new StyleBundle("~/Admin/plugins/timepicker/css").Include(
                                        "~/Admin/plugins/timepicker/css/bootstrap-timepicker.min.css"));

            // plugins | raphael
            bundles.Add(new ScriptBundle("~/Admin/plugins/raphael/js").Include(
                                         "~/Admin/plugins/raphael/js/raphael-min.js"));

            // plugins | select2
            bundles.Add(new ScriptBundle("~/Admin/plugins/select2/js").Include(
                                         "~/Admin/plugins/select2/js/select2.full.min.js"));

            bundles.Add(new StyleBundle("~/Admin/plugins/select2/css").Include(
                                        "~/Admin/plugins/select2/css/select2.min.css"));

            // plugins | morris
            bundles.Add(new ScriptBundle("~/Admin/plugins/morris/js").Include(
                                         "~/Admin/plugins/morris/js/morris.min.js"));

        }
    }
}
