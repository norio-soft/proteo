using Orchestrator.Repositories;
using Orchestrator.Repositories.DTOs.RevenuePerVehicle;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Reporting;

namespace Orchestrator.WebUI.Report
{
    public partial class RevenuePerVehicleReport : Orchestrator.Base.BasePage
    {

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
            {
                var weekStart = Entities.Utilities.GetWeekStartDate((DayOfWeek)this.WeekStartDay);
                this.dteStartDate.SelectedDate = weekStart;
                this.dteEndDate.SelectedDate = DateTime.Today;
            }
            reportViewer.Visible = false;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.btnReport.Click += btnGenerateReport_Click;
            this.btnGenerateReport.Click += btnGenerateReport_Click;
            this.btnGenerateCsv.Click += btnGenerateCsv_Click;
            this.btnGeneratePdf.Click += btnGeneratePdf_Click;

            this.cboDepot.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboDepot_ItemsRequested);
            this.cboVehicle.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboVehicle_ItemsRequested);
        }

        protected void cboClient_SelectedIndexChanged(object sender, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Value)) cboDepot.Enabled = true;
            else cboDepot.Enabled = false;
        }

        protected void cboDepot_SelectedIndexChanged(object sender, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Value)) cboVehicle.Enabled = true;
            else cboVehicle.Enabled = false;
        }

        protected void cboVehicle_SelectedIndexChanged(object sender, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
        }

        private InstanceReportSource GenerateReport()
        {
            int depotId = 0;
            int.TryParse(cboDepot.SelectedValue, out depotId);
            int vehicleId = 0;
            int.TryParse(cboVehicle.SelectedValue, out vehicleId);

            RevenuePerVehicleReportData reportData;
            Facade.IJob facJob = new Facade.Job();
            if (depotId == 0 && vehicleId == 0)     //All Depots - All Vehicles
            {
                reportData = facJob.GetRevenuePerVehicleData(dteStartDate.SelectedDate.Value, dteEndDate.SelectedDate.Value);
            }
            else if (depotId !=0 && vehicleId == 0)     //Single Depot - All Vehicle
            {
                reportData = facJob.GetRevenuePerVehicleData(dteStartDate.SelectedDate.Value,
                    dteEndDate.SelectedDate.Value, depotId);
            }
            else        //Doesn't Matter - Single Vehicle
            {
                reportData = facJob.GetRevenuePerVehicleData(dteStartDate.SelectedDate.Value,
                    dteEndDate.SelectedDate.Value, depotId, vehicleId);
            }

            var typeName = "Orchestrator.Reports.rptRevenuePerVehicle, Orchestrator.Reports";
            var reportType = Type.GetType(typeName);
            var report = (Telerik.Reporting.Report)Activator.CreateInstance(reportType, reportData);

            foreach (IReportParameter reportParameter in report.ReportParameters)
            {
                switch (reportParameter.Name)
                {
                    case "fromDate": reportParameter.Value = dteStartDate.SelectedDate.Value.ToShortDateString(); break;
                    case "toDate": reportParameter.Value = dteEndDate.SelectedDate.Value.ToShortDateString(); break;
                    case "depot": reportParameter.Value = cboDepot.Text; break;
                    case "vehicle": reportParameter.Value = cboVehicle.Text; break;
                }
            }

            return new InstanceReportSource { ReportDocument = report };
        }

        private void btnGenerateCsv_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            int depotId = 0;
            int.TryParse(cboDepot.SelectedValue, out depotId);
            int vehicleId = 0;
            int.TryParse(cboVehicle.SelectedValue, out vehicleId);

            RevenuePerVehicleReportData reportData;
            Facade.IJob facJob = new Facade.Job();
            if (depotId == 0 && vehicleId == 0)     //All Depots - All Vehicles
            {
                reportData = facJob.GetRevenuePerVehicleData(dteStartDate.SelectedDate.Value,
                    dteEndDate.SelectedDate.Value);
            }
            else if (depotId != 0 && vehicleId == 0)     //Single Depot - All Vehicle
            {
                reportData = facJob.GetRevenuePerVehicleData(dteStartDate.SelectedDate.Value,
                    dteEndDate.SelectedDate.Value, depotId);
            }
            else        //Doesn't Matter - Single Vehicle
            {
                reportData = facJob.GetRevenuePerVehicleData(dteStartDate.SelectedDate.Value,
                    dteEndDate.SelectedDate.Value, depotId, vehicleId);
            }

            var vehicles = reportData.Depots.SelectMany(dptSummary => dptSummary.Vehicles);
            List<CsvExport.PropertyMapping<VehicleRevenue>> mappings = new List<CsvExport.PropertyMapping<VehicleRevenue>>();
            if (vehicleId == 0)
            {
            mappings.Add(new CsvExport.PropertyMapping<VehicleRevenue>("Depot", t => t.DepotName));
            mappings.Add(new CsvExport.PropertyMapping<VehicleRevenue>("Vehicle", t => t.VehicleName));
            }
            else
            {
                mappings.Add(new CsvExport.PropertyMapping<VehicleRevenue>("Vehicle", t => t.DepotName));
                mappings.Add(new CsvExport.PropertyMapping<VehicleRevenue>("Driver", t => t.VehicleName));
            }
            mappings.Add(new CsvExport.PropertyMapping<VehicleRevenue>("Revenue", t => t.Revenue_Formatted));
            mappings.Add(new CsvExport.PropertyMapping<VehicleRevenue>("Percent Of Total", t => t.PercentOfDepotTotal_Formatted));
            CsvExport.Export<VehicleRevenue>(vehicles, "RevenuePerVehicleReport.csv", mappings);
        }

        private string RoundSecondsToMinutes(int seconds)
        {
            return Math.Round((decimal)seconds / 60, 0, MidpointRounding.AwayFromZero).ToString("f0");
        }

        private void btnGeneratePdf_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var instanceReportSource = this.GenerateReport();
            Utilities.ExportReportAsPdf(instanceReportSource, Response, "RevenuePerVehicleReport.pdf");
        }

        private void cboDepot_ItemsRequested(object sender, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            cboDepot.Items.Clear();

            Facade.IOrganisationLocation facOrgLoc = new Facade.Organisation();
            DataSet ds = facOrgLoc.GetAllDepots(Orchestrator.Globals.Configuration.IdentityId);

            DataTable dt = ds.Tables[0];
            Telerik.Web.UI.RadComboBoxItem rcItem = null;
            rcItem = new Telerik.Web.UI.RadComboBoxItem();
            rcItem.Text = "All";
            rcItem.Value = "0";
            cboDepot.Items.Add(rcItem);

            List<Telerik.Web.UI.RadComboBoxItem> comboItems = new List<Telerik.Web.UI.RadComboBoxItem>();
            dt.Rows.Cast<DataRow>().ToList().ForEach(row => comboItems.Add(
                new Telerik.Web.UI.RadComboBoxItem
                {
                    Text = row["OrganisationLocationName"].ToString(),
                    Value = row["OrganisationLocationId"].ToString()
                }));
            comboItems.Sort((item1, item2) => item1.Text.CompareTo(item2.Text));
            cboDepot.Items.AddRange(comboItems);
        }

        private void cboVehicle_ItemsRequested(object sender, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            cboVehicle.Items.Clear();

            int depotId = 0;
            int.TryParse(cboDepot.SelectedValue, out depotId);
            Orchestrator.Facade.IVehicle facResource = new Orchestrator.Facade.Resource();

            DataSet ds = null;
            if (depotId == 0)
                ds = facResource.GetAllVehicles();
            else
                ds = facResource.GetVehicleForDepotId(depotId);

            DataTable dt = ds.Tables[0];
            Telerik.Web.UI.RadComboBoxItem rcItem = null;
            rcItem = new Telerik.Web.UI.RadComboBoxItem();
            rcItem.Text = "All";
            rcItem.Value = "0";
            cboVehicle.Items.Add(rcItem);

            List<int> excludedVehicles = null;
            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<Repositories.IVehicleExcludedFromRevenueReportRepository>(uow);
                excludedVehicles = repo.GetExcludedVehicleIds().ToList();
                if (excludedVehicles == null) excludedVehicles = new List<int>();
            }

            List<Telerik.Web.UI.RadComboBoxItem> comboItems =
                (from row in dt.Rows.Cast<DataRow>()
                 where !excludedVehicles.Contains(Convert.ToInt32(row["ResourceId"]))
                 select new Telerik.Web.UI.RadComboBoxItem
                 {
                     Value = row["ResourceId"].ToString(),
                     Text = row["RegNo"].ToString()
                 }).GroupBy(item => item.Value).Select(grp => grp.First()).ToList();
            comboItems.Sort((item1, item2) => item1.Text.CompareTo(item2.Text));
            cboVehicle.Items.AddRange(comboItems);
        }

        private void btnGenerateReport_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                reportViewer.ReportSource = this.GenerateReport();
                ScriptManager.RegisterStartupScript(this, this.GetType(),
                    "ShowFilterOptions", "$(function() { filterOptionsDisplayToggle(false); });", true);
                reportViewer.Visible = true;
            }
        }

        protected void cfvStartDate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = false;

            if (dteStartDate.SelectedDate <= dteEndDate.SelectedDate)
                args.IsValid = true;
        }

        protected void cfvEndDate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = false;

            if (dteEndDate.SelectedDate <= DateTime.Today)
                args.IsValid = true;
        }
    }
}