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
using Orchestrator.Facade;
using System.Data;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.Reports
{

    public partial class DriverTimeReport : System.Web.UI.Page
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
                this.PopulateStaticControls();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.btnReport.Click += btnGenerateReport_Click;
            this.btnGenerateReport.Click += btnGenerateReport_Click;
            this.btnGenerateCsv.Click += btnGenerateCsv_Click;
            this.btnGeneratePdf.Click += btnGeneratePdf_Click;
        }

        private void PopulateStaticControls()
        {
            Facade.IDriver facResource = new Facade.Resource();
            DataSet dsDrivers = facResource.GetAllDrivers(false);
            cboDriver.DataSource = dsDrivers;
            cboDriver.DataTextField = "FullName";
            cboDriver.DataValueField = "ResourceId";
            cboDriver.DataBind();
            cboDriver.Items.Insert(0, new RadComboBoxItem("- all -", "-1"));
            
            var weekStart = Entities.Utilities.GetWeekStartDate((DayOfWeek)this.WeekStartDay);
            this.dteStartDate.SelectedDate = weekStart;
            this.dteEndDate.SelectedDate = weekStart.AddDays(6);
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
            var typeName = "Orchestrator.Reports.rptDriverTime, Orchestrator.Reports";
            var reportType = Type.GetType(typeName);
            var report = (IReportDocument)Activator.CreateInstance(reportType);

            report.ReportParameters.ElementAt(0).Value = dteStartDate.SelectedDate.Value.Date;
            report.ReportParameters.ElementAt(1).Value = dteEndDate.SelectedDate.Value.Date;
            report.ReportParameters.ElementAt(2).Value = cboDriver.SelectedValue;

            if (!rbDefault.Checked && txtHoursGreaterThan.Value.HasValue)
            {
                var seconds = (int)txtHoursGreaterThan.Value * 3600;

                if (rbDrive.Checked)
                {
                    Telerik.Reporting.Filter driveFilter = new Telerik.Reporting.Filter();
                    driveFilter.Expression = "=Sum(Fields.DriveDuration)";
                    driveFilter.Operator = Telerik.Reporting.FilterOperator.GreaterThan;
                    driveFilter.Value = "=" + seconds.ToString();
                    report.Reports.ElementAt(0).Groups.ElementAt(1).Filters.Add(driveFilter);
                }

                if (rbSpreadOver.Checked)
                {
                    Telerik.Reporting.Filter spreadOverFilter = new Telerik.Reporting.Filter();
                    spreadOverFilter.Expression = "=Sum(Fields.AvailabilityDuration) + Sum(Fields.WorkDuration) + Sum(Fields.DriveDuration)";
                    spreadOverFilter.Operator = Telerik.Reporting.FilterOperator.GreaterThan;
                    spreadOverFilter.Value = "=" + seconds.ToString();
                    report.Reports.ElementAt(0).Groups.ElementAt(1).Filters.Add(spreadOverFilter);
                }
            }
            
            return new InstanceReportSource { ReportDocument = report };
        }

        protected void cfvStartDate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = false;

            if (dteStartDate.SelectedDate <= dteEndDate.SelectedDate)
                args.IsValid = true;
        }

        private void btnGenerateCsv_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<IDriverWorkingStateRepository>(uow);
                DateTime? mostRecentDateStamp;
                var data = repo.GetDurations(dteStartDate.SelectedDate.Value.Date, dteEndDate.SelectedDate.Value.Date, null, out mostRecentDateStamp);

                CsvExport.Export(
                    data,
                    "DriverTime.csv",
                    new CsvExport.PropertyMappings<Orchestrator.Repositories.DTOs.DriverDayDurations>
                    {
                        { "Depot", i => i.DepotName },
                        { "Driver", i => i.DriverName },
                        { "Day", i => i.Day.ToShortDateString() },
                        { "Break", i => RoundSecondsToMinutes(i.RestDuration) },
                        { "POA", i => RoundSecondsToMinutes(i.AvailabilityDuration)  },
                        { "Other Work", i => RoundSecondsToMinutes(i.WorkDuration)  },
                        { "Drive", i => RoundSecondsToMinutes(i.DriveDuration)  },
                    });
            }
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
            Utilities.ExportReportAsPdf(instanceReportSource, Response, "DriverTime.pdf");
        }

    }

}