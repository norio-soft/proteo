using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Reporting;
using System.Text;

namespace Orchestrator.WebUI.Reports
{
    public partial class RunOverviewReport : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulateStaticControls();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.btnReport.Click += new EventHandler(btnReport_Click);
        }

        private void PopulateStaticControls()
        {
            #region // Populate Control Areas
            Facade.IControlArea facControlArea = new Facade.Traffic();
            #endregion

            #region // Populate Traffic Areas
            Facade.ITrafficArea facTrafficArea = (Facade.ITrafficArea)facControlArea;
            cblTrafficAreas.DataSource = facTrafficArea.GetAll();
            cblTrafficAreas.DataBind();

            if (cblTrafficAreas.Items.Count > 8)
            {
                divTrafficAreas.Attributes.Add("class", "overflowHandler");
                divTrafficAreas.Attributes.Add("style", "height:100px;");
            }

            for (int i = 0; i < cblTrafficAreas.Items.Count; i++)
            {
                cblTrafficAreas.Items[i].Selected = true;
            }

            #endregion

            #region // Cause the job states to be displayed
            cblJobStates.DataSource = Utilities.GetEnumPairs<eJobState>();
            cblJobStates.DataBind();
            #endregion
        }

        private void btnReport_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
                generateReport();
        }

        private void generateReport()
        {
            var trafficAreaIDs = string.Join(",", cblTrafficAreas.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value));
            var jobStateIDs = string.Join(",", cblJobStates.Items.Cast<ListItem>().Where(i => i.Selected).Select(i => i.Value));

            var typeName = "Orchestrator.Reports.rptRunOverview, Orchestrator.Reports";
            var reportType = Type.GetType(typeName);
            var report = (IReportDocument)Activator.CreateInstance(reportType);

            report.ReportParameters.ElementAt(0).Value = dteStartDate.SelectedDate.Value.Date;
            report.ReportParameters.ElementAt(1).Value = dteEndDate.SelectedDate.Value.Date.AddDays(1);
            report.ReportParameters.ElementAt(2).Value = trafficAreaIDs;
            report.ReportParameters.ElementAt(3).Value = jobStateIDs;

            var reportSource = new InstanceReportSource { ReportDocument = report };
            this.rptvRunOverview.ReportSource = reportSource;
        }

        #region Validation
        protected void cfvStartDate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = false;

            if (dteStartDate.SelectedDate <= dteEndDate.SelectedDate)
                args.IsValid = true;
        }
        #endregion
    }

    
}