using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Reporting;
using System.Text;
using System.Globalization;
using Orchestrator.Repositories;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.Reports.JR
{

    public partial class ProfitabilityByPlannerReport : System.Web.UI.Page
    {

        private enum ReportInstanceEnum { ByPlanner, BySingleDriver };

        private class Filter
        {
            public ReportInstanceEnum ReportInstance { get; set; }
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }
            public IEnumerable<int> PlannerIDs { get; set; }
            public IEnumerable<string> PlannerNames { get; set; }
            public int? PlannerIdentityID { get; set; }
            public int? DriverIdentityID { get; set; }
        }

        private static readonly IDictionary<ReportInstanceEnum, string> _reportTypeNames = new Dictionary<ReportInstanceEnum, string>
        {
            { ReportInstanceEnum.ByPlanner, "Orchestrator.JR.Reports.rptProfitabilityByPlanner, Orchestrator.JR" },
            { ReportInstanceEnum.BySingleDriver, "Orchestrator.JR.Reports.rptProfitabilityByPlannerSingleDriver, Orchestrator.JR" },
        };

        protected int WeekStartDay
        {
            get
            {
                var retVal = Globals.Configuration.ReportingFirstDayOfWeek;

                if (!retVal.HasValue)
                {
                    var culture = new CultureInfo(Globals.Configuration.NativeCulture);
                    retVal = culture.DateTimeFormat.FirstDayOfWeek;
                }

                return (int)retVal.Value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                PopulateStaticControls();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.cfvStartDate.ServerValidate += cfvStartDate_ServerValidate;
            this.btnReport.Click += btnGenerateReport_Click;
            this.btnGenerateReport.Click += btnGenerateReport_Click;
            this.btnGenerateCsv.Click += btnGenerateCsv_Click;
            this.btnGeneratePdf.Click += btnGeneratePdf_Click;
        }

        private void PopulateStaticControls()
        {
            var lastWeekStart = Entities.Utilities.GetWeekStartDate((DayOfWeek)this.WeekStartDay).AddDays(-14);
            this.dteStartDate.SelectedDate = lastWeekStart;
            this.dteEndDate.SelectedDate = lastWeekStart.AddDays(6);

            Facade.IUser facUser = new Facade.User();
            
            this.lbAvailablePlanners.DataSource = facUser.GetAllUsersInRole(eUserRole.Planner);
            this.lbAvailablePlanners.DataBind();

        }

        private Filter GetFilter()
        {
            var retVal = new Filter
            {
                FromDate = dteStartDate.SelectedDate.Value.Date,
                ToDate = dteEndDate.SelectedDate.Value.Date,
                PlannerNames = Enumerable.Empty<string>(),

            };

                if (lbSelectedPlanners.Items.Any())
                {
                    retVal.PlannerIDs = lbSelectedPlanners.Items.Select(i => int.Parse(i.Value));
                    retVal.PlannerNames = lbSelectedPlanners.Items.Select(i => i.Text).OrderBy(s => s);
                }

                if (lbSelectedPlanners.Items.Count == 1)
            {
                retVal.DriverIdentityID = int.Parse(cboDriver.SelectedValue);
            }

                retVal.ReportInstance = retVal.DriverIdentityID.HasValue ? ReportInstanceEnum.BySingleDriver : ReportInstanceEnum.ByPlanner;
            
            return retVal;
        }

        private void btnGenerateReport_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                this.reportViewer.ReportSource = this.GenerateReport();

                // turns off the filter display after generating the report.
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowFilterOptions", "$(function() { filterOptionsDisplayToggle(false); });", true);
            }
        }

        private InstanceReportSource GenerateReport()
        {
            IReportDocument report = null;
            var filter = this.GetFilter();
            var reportTypeName = _reportTypeNames[filter.ReportInstance];
            report = GetReportDocument(reportTypeName);

            report.ReportParameters.ElementAt(0).Value = filter.FromDate;
            report.ReportParameters.ElementAt(1).Value = filter.ToDate;

            if (new[] { ReportInstanceEnum.ByPlanner, ReportInstanceEnum.BySingleDriver}.Contains(filter.ReportInstance))
            {
                report.ReportParameters.ElementAt(2).Value = string.Join(",", filter.PlannerIDs);
                if (lbAvailablePlanners.Items.Count == 0)
                {
                    // all planners
                    report.ReportParameters.ElementAt(3).Value = " - all -";
                }
                else
                {
                    report.ReportParameters.ElementAt(3).Value = filter.PlannerNames.Any() ? string.Join(", ", filter.PlannerNames) : "- all -";
                }

                if (filter.ReportInstance == ReportInstanceEnum.BySingleDriver)
                {
                    report.ReportParameters.ElementAt(2).Value = filter.DriverIdentityID;
                }
                else if (filter.ReportInstance == ReportInstanceEnum.ByPlanner)
                {
                    report.ReportParameters.ElementAt(4).Value = filter.PlannerIDs.Count() == 1; // isSingleDepot parameter
                }
            }

            return new InstanceReportSource { ReportDocument = report };
        }

        private static IReportDocument GetReportDocument(string typeName)
        {
            var reportType = Type.GetType(typeName);
            var  rpt = Activator.CreateInstance(reportType);
            return (IReportDocument)rpt;
        }

        private void cfvStartDate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = dteStartDate.SelectedDate <= dteEndDate.SelectedDate;
        }


        private void btnGenerateCsv_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<IProfitReportRunRepository>(uow);
                var filter = this.GetFilter();
                var fileName = "Profitability.csv";

                switch (filter.ReportInstance)
                {
                    case ReportInstanceEnum.ByPlanner:
                        CsvExport.Export(
                            repo.GetReportDataByPlanner(filter.FromDate, filter.ToDate, string.Join(",", filter.PlannerIDs)),
                            fileName,
                            new CsvExport.PropertyMappings<Orchestrator.Repositories.DTOs.ProfitabilityReport.ByPlannerRow>
                                {
                                    { "Planner", i => i.PlannerName },
                                    { "Driver", i => i.VehicleRegistration },
                                    { "PayrollNo", i => i.PayrollNo },
                                    { "Revenue", i => i.Revenue.ToString("f2") },
                                    { "Cost", i => i.Cost.ToString("f2") },
                                    { "Profit", i => i.Profit.ToString("f2") },
                                    { "Margin", i => i.Margin.ToString("p2") },
                                    { "Single Delivery Count", i => i.SingleDeliveryCount.ToString() },
                                    { "Multiple Delivery Count", i => i.MultipleDeliveryCount.ToString() },
                                }
                            );

                        break;

                    case ReportInstanceEnum.BySingleDriver:
                        CsvExport.Export(
                            repo.GetReportDataByPlannerSingleDriver(filter.FromDate, filter.ToDate, filter.DriverIdentityID.Value),
                            fileName,
                            new CsvExport.PropertyMappings<Orchestrator.Repositories.DTOs.ProfitabilityReport.ByPlannerSinglePlannerRow>
                                {
                                    { "Planner", i => i.PlannerName},
                                    { "Vehicle", i => i.VehicleRegistration },
                                    { "Driver", i => i.DriverName },
                                    { "Revenue", i => i.Revenue.ToString("f2") },
                                    { "Cost", i => i.Cost.ToString("f2") },
                                    { "Profit", i => i.Profit.ToString("f2") },
                                    { "Margin", i => i.Margin.ToString("p2") },
                                    { "Single Delivery Count", i => i.SingleDeliveryCount.ToString() },
                                    { "Multiple Delivery Count", i => i.MultipleDeliveryCount.ToString() },
                                }
                            );

                        break;

                }
            }
        }

        private void btnGeneratePdf_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var instanceReportSource = this.GenerateReport();
            Utilities.ExportReportAsPdf(instanceReportSource, Response, "Profitability.pdf");
        }

    }

}