using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections.Specialized;

namespace Orchestrator.WebUI.Resource.Driver
{
    public partial class driverRevenue : Orchestrator.Base.BasePage
    {

        //----------------------------------------------------------------------------------------------

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        //----------------------------------------------------------------------------------------------

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.btnRefresh.Click += new EventHandler(btnRefresh_Click);
            this.dteStartDate.SelectedDate = DateTime.Today;
            this.dteEndDate.SelectedDate = DateTime.Today.AddDays(7);
        }

        //----------------------------------------------------------------------------------------------

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            Facade.IDriver facDriver = new Facade.Resource();
            DataSet dsDriverRevenue = facDriver.GetDriverRevenue(Convert.ToInt32(cboDriver.SelectedValue),
                dteStartDate.SelectedDate.Value.Date, dteEndDate.SelectedDate.Value.Date);

            NameValueCollection reportParams = new NameValueCollection();

            reportParams.Add("ResourceId", cboDriver.SelectedValue);
            reportParams.Add("StartDate", dteStartDate.SelectedDate.Value.Date.ToString("dd/MM/yyyy"));
            reportParams.Add("EndDate", dteEndDate.SelectedDate.Value.Date.ToString("dd/MM/yyyy"));

            // Configure the Session variables used to pass data to the report
            Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.DriverRevenue;
            Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = dsDriverRevenue;
            Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
            Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";
            Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;

            // Show the user control
            reportViewer.Visible = true;
        }

        //----------------------------------------------------------------------------------------------
    }
}
