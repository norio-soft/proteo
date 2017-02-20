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

    public partial class ProfitabilityReport : System.Web.UI.Page
    {

        private enum ReportInstanceEnum { ByDepot, ByDepotSingleVehicle, ByClient, ByClientByDeliveryPoint, ByClientByCollectionPoint };

        private class Filter
        {
            public ReportInstanceEnum ReportInstance { get; set; }
            public DateTime FromDate { get; set; }
            public DateTime ToDate { get; set; }
            public IEnumerable<int> DepotIDs { get; set; }
            public IEnumerable<string> DepotNames { get; set; }
            public int? VehicleID { get; set; }
            public string VehicleRegistration { get; set; }
            public IEnumerable<int> ClientIDs { get; set; }
            public IEnumerable<string> ClientNames { get; set; }
        }

        private static readonly IDictionary<ReportInstanceEnum, string> _reportTypeNames = new Dictionary<ReportInstanceEnum, string>
        {
            { ReportInstanceEnum.ByDepot, "Orchestrator.JR.Reports.rptProfitabilityByDepot, Orchestrator.JR" },
            { ReportInstanceEnum.ByDepotSingleVehicle, "Orchestrator.JR.Reports.rptProfitabilityByDepotSingleVehicle, Orchestrator.JR" },
            { ReportInstanceEnum.ByClient, "Orchestrator.JR.Reports.rptProfitabilityByClient, Orchestrator.JR" },
            { ReportInstanceEnum.ByClientByDeliveryPoint, "Orchestrator.JR.Reports.rptProfitabilityByClientByDeliveryPoint, Orchestrator.JR" },
            { ReportInstanceEnum.ByClientByCollectionPoint, "Orchestrator.JR.Reports.rptProfitabilityByClientByCollectionPoint, Orchestrator.JR" },
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
            this.cfvReportBy.ServerValidate += cfvReportBy_ServerValidate;
            this.btnReport.Click += btnGenerateReport_Click;
            this.btnGenerateReport.Click += btnGenerateReport_Click;
            this.btnGenerateCsv.Click += btnGenerateCsv_Click;
            this.btnGeneratePdf.Click += btnGeneratePdf_Click;
        }

        private void PopulateStaticControls()
        {
            var lastWeekStart = Entities.Utilities.GetWeekStartDate((DayOfWeek)this.WeekStartDay).AddDays(-7);
            this.dteStartDate.SelectedDate = lastWeekStart;
            this.dteEndDate.SelectedDate = lastWeekStart.AddDays(6);

            Facade.IOrganisationLocation facOrganisationLocation = new Facade.Organisation();
            this.lbAvailableDepots.DataSource = facOrganisationLocation.GetAllDepots(Orchestrator.Globals.Configuration.IdentityId);
            this.lbAvailableDepots.DataBind();

            var orgFacade = new Facade.Organisation();
            var orgs = orgFacade.GetAllForType((int)eOrganisationType.Client);
            this.lbAvailableClients.DataSource = orgs.Tables[0];
            this.lbAvailableClients.DataBind();
        }

        private Filter GetFilter()
        {
            var retVal = new Filter
            {
                FromDate = dteStartDate.SelectedDate.Value.Date,
                ToDate = dteEndDate.SelectedDate.Value.Date,
                DepotIDs = Enumerable.Empty<int>(),
                DepotNames = Enumerable.Empty<string>(),
                ClientIDs = Enumerable.Empty<int>(),
                ClientNames = Enumerable.Empty<string>(),
            };

            if (rbReportByDepot.Checked)
            {
                if (lbAvailableDepots.Items.Any() && lbSelectedDepots.Items.Any())
                {
                    retVal.DepotIDs = lbSelectedDepots.Items.Select(i => int.Parse(i.Value));
                    retVal.DepotNames = lbSelectedDepots.Items.Select(i => i.Text).OrderBy(s => s);
                }

                if (retVal.DepotIDs.Count() == 1)
                {
                    retVal.VehicleID = Utilities.ParseNullable<int>(cboVehicle.SelectedValue);
                    retVal.VehicleRegistration = retVal.VehicleID.HasValue ? cboVehicle.Text : null;
                }

                retVal.ReportInstance = retVal.VehicleID.HasValue ? ReportInstanceEnum.ByDepotSingleVehicle : ReportInstanceEnum.ByDepot;
            }
            else
            {
                if (lbAvailableClients.Items.Any() && lbSelectedClients.Items.Any())
                {
                    retVal.ClientIDs = lbSelectedClients.Items.Select(i => int.Parse(i.Value));
                    retVal.ClientNames = lbSelectedClients.Items.Select(i => i.Text).OrderBy(s => s);
                }

                if (retVal.ClientIDs.Count() != 1)
                    retVal.ReportInstance = ReportInstanceEnum.ByClient;
                else if (rbGroupByCollectionPoint.Checked)
                    retVal.ReportInstance = ReportInstanceEnum.ByClientByCollectionPoint;
                else
                    retVal.ReportInstance = ReportInstanceEnum.ByClientByDeliveryPoint;
            }

            return retVal;
        }

        private void btnGenerateReport_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                this.reportViewer.ReportSource = this.GenerateReport();
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

            if (new[] { ReportInstanceEnum.ByDepot, ReportInstanceEnum.ByDepotSingleVehicle }.Contains(filter.ReportInstance))
            {
                report.ReportParameters.ElementAt(2).Value = string.Join(",", filter.DepotIDs);
                report.ReportParameters.ElementAt(3).Value = filter.DepotNames.Any() ? string.Join(", ", filter.DepotNames) : "- all -";

                if (filter.ReportInstance == ReportInstanceEnum.ByDepotSingleVehicle)
                {
                    report.ReportParameters.ElementAt(4).Value = filter.VehicleID;
                    report.ReportParameters.ElementAt(5).Value = filter.VehicleRegistration;
                }
                else if (filter.ReportInstance == ReportInstanceEnum.ByDepot)
                {
                    report.ReportParameters.ElementAt(4).Value = filter.DepotIDs.Count() == 1; // isSingleDepot parameter
                }
            }
            else if (new[] { ReportInstanceEnum.ByClient, ReportInstanceEnum.ByClientByDeliveryPoint, ReportInstanceEnum.ByClientByCollectionPoint }.Contains(filter.ReportInstance))
            {
                report.ReportParameters.ElementAt(2).Value = string.Join(",", filter.ClientIDs);
                report.ReportParameters.ElementAt(3).Value = !filter.ClientNames.Any() ? "- all -" : (filter.ClientNames.Count() > 10 ? "- many -" : string.Join(", ", filter.ClientNames));
            }

            return new InstanceReportSource { ReportDocument = report };
        }

        private static IReportDocument GetReportDocument(string typeName)
        {
            var reportType = Type.GetType(typeName);
            return (IReportDocument)Activator.CreateInstance(reportType);
        }

        private void cfvStartDate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = dteStartDate.SelectedDate <= dteEndDate.SelectedDate;
        }

        private void cfvReportBy_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = rbReportByDepot.Checked || rbReportByClient.Checked;
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
                    case ReportInstanceEnum.ByDepot:
                        CsvExport.Export(
                            repo.GetReportDataByDepot(filter.FromDate, filter.ToDate, string.Join(",", filter.DepotIDs)),
                            fileName,
                            new CsvExport.PropertyMappings<Orchestrator.Repositories.DTOs.ProfitabilityReport.ByDepotRow>
                                {
                                    { "Depot", i => i.DepotName },
                                    { "Vehicle", i => i.VehicleRegistration },
                                    { "Revenue", i => i.Revenue.ToString("f2") },
                                    { "Cost", i => i.Cost.ToString("f2") },
                                    { "Profit", i => i.Profit.ToString("f2") },
                                    { "Margin", i => i.Margin.ToString("p2") },
                                    { "Single Delivery Count", i => i.SingleDeliveryCount.ToString() },
                                    { "Multiple Delivery Count", i => i.MultipleDeliveryCount.ToString() },
                                }
                            );

                        break;

                    case ReportInstanceEnum.ByDepotSingleVehicle:
                        CsvExport.Export(
                            repo.GetReportDataByDepotSingleVehicle(filter.FromDate, filter.ToDate, filter.VehicleID.Value),
                            fileName,
                            new CsvExport.PropertyMappings<Orchestrator.Repositories.DTOs.ProfitabilityReport.ByDepotSingleVehicleRow>
                                {
                                    { "Depot", i => i.DepotName },
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

                    case ReportInstanceEnum.ByClient:
                        CsvExport.Export(
                            repo.GetReportDataByClient(filter.FromDate, filter.ToDate, string.Join(",", filter.ClientIDs)),
                            fileName,
                            new CsvExport.PropertyMappings<Orchestrator.Repositories.DTOs.ProfitabilityReport.ByClientRow>
                                {
                                    { "Client", i => i.ClientName },
                                    { "Revenue", i => i.Revenue.ToString("f2") },
                                    { "Cost", i => i.Cost.ToString("f2") },
                                    { "Profit", i => i.Profit.ToString("f2") },
                                    { "Margin", i => i.Margin.ToString("p2") },
                                    { "Single Delivery Count", i => i.SingleDeliveryCount.ToString() },
                                    { "Multiple Delivery Count", i => i.MultipleDeliveryCount.ToString() },
                                }
                            );

                        break;

                    case ReportInstanceEnum.ByClientByDeliveryPoint:
                        CsvExport.Export(
                            repo.GetReportDataByClientByDeliveryPoint(filter.FromDate, filter.ToDate, filter.ClientIDs.First()),
                            fileName,
                            new CsvExport.PropertyMappings<Orchestrator.Repositories.DTOs.ProfitabilityReport.ByClientByDeliveryPointRow>
                                {
                                    { "Client", i => i.ClientName },
                                    { "Delivery Point", i => i.DeliveryPoint },
                                    { "Delivery Post Town", i => i.DeliveryPostTown },
                                    { "Revenue", i => i.Revenue.ToString("f2") },
                                    { "Cost", i => i.Cost.ToString("f2") },
                                    { "Profit", i => i.Profit.ToString("f2") },
                                    { "Margin", i => i.Margin.ToString("p2") },
                                    { "Single Delivery Count", i => i.SingleDeliveryCount.ToString() },
                                    { "Multiple Delivery Count", i => i.MultipleDeliveryCount.ToString() },
                                }
                            );

                        break;

                    case ReportInstanceEnum.ByClientByCollectionPoint:
                        CsvExport.Export(
                            repo.GetReportDataByClientByCollectionPoint(filter.FromDate, filter.ToDate, filter.ClientIDs.First()),
                            fileName,
                            new CsvExport.PropertyMappings<Orchestrator.Repositories.DTOs.ProfitabilityReport.ByClientByCollectionPointRow>
                                {
                                    { "Client", i => i.ClientName },
                                    { "Collection Point", i => i.CollectionPoint },
                                    { "Collection Post Town", i => i.CollectionPostTown },
                                    { "Delivery Point", i => i.DeliveryPoint },
                                    { "Delivery Post Town", i => i.DeliveryPostTown },
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