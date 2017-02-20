using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Collections.Specialized;

namespace Orchestrator.WebUI.Report
{
    public partial class jobpricing : Orchestrator.Base.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage);

            if (!IsPostBack)
            {
                dteStartDate.SelectedDate = DateTime.UtcNow.Subtract(new TimeSpan(1, 0, 0, 0));
                dteEndDate.SelectedDate = DateTime.Today;
                
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.btnGenerate.Click += new EventHandler(btnGenerate_Click);
            this.cboClient.ItemsRequested +=new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);
            this.btnExportToCSV.Click +=new EventHandler(btnExportToCSV_Click);
        }

        #region ActiveReport
        private void LoadReport()
        {
            DateTime today = DateTime.UtcNow;
            today = today.Subtract(today.TimeOfDay);

            int identityId = 0;
            int.TryParse(cboClient.SelectedValue, out identityId);

            Facade.IOrganisation facOrganisation = new Facade.Organisation();
            Facade.IJob facJob = new Facade.Job();

            DataSet dsReport = facJob.GetJobPricingData(identityId, dteStartDate.SelectedDate.Value, dteEndDate.SelectedDate.Value);

            if (dsReport.Tables[1].Rows.Count == 0)
            {
                lblError.Text = "No completed delivery and returns for client " + cboClient.Text + " for period " + dteStartDate.SelectedDate.Value.ToString("dd/MM/yy") + " to " + dteEndDate.SelectedDate.Value.ToString("dd/MM/yy");
                lblError.Visible = true;
                reportViewer.Visible = false;
            }

            else
            {
                lblError.Visible = false;
                
                NameValueCollection reportParams = new NameValueCollection();

                reportParams.Add("IdentityID", cboClient.SelectedValue);
                reportParams.Add("StartDate", dteStartDate.SelectedDate.Value.ToString("dd/MM/yy"));
                reportParams.Add("EndDate", dteEndDate.SelectedDate.Value.ToString("dd/MM/yy"));

                // Configure the Session variables used to pass data to the report
                Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.JobPricing;
                Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = dsReport;
                Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
                Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table1";
                Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;

                if (cboClient.SelectedValue != "")
                    reportViewer.IdentityId = int.Parse(cboClient.SelectedValue);
                // Show the user control
                reportViewer.Visible = true;
            }
        }
        #endregion

        #region DBCombo's Server Methods and Initialisation

        void cboClient_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {

            cboClient.Items.Clear();

            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllClientsFiltered(e.Text);

            int itemsPerRequest = 20;
            int itemOffset = e.NumberOfItems;
            int endOffset = itemOffset + itemsPerRequest;
            if (endOffset > ds.Tables[0].Rows.Count)
                endOffset = ds.Tables[0].Rows.Count;

            DataTable dt = ds.Tables[0];
            Telerik.Web.UI.RadComboBoxItem rcItem = null;
            for (int i = itemOffset; i < endOffset; i++)
            {
                rcItem = new Telerik.Web.UI.RadComboBoxItem();
                rcItem.Text = dt.Rows[i]["OrganisationName"].ToString();
                rcItem.Value = dt.Rows[i]["IdentityId"].ToString();
                cboClient.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }
        }
        #endregion

        #region Event Handlers
        private void btnExportToCSV_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                int identityId = 0;
                int.TryParse(cboClient.SelectedValue, out identityId);

                DateTime start = dteStartDate.SelectedDate.Value;
                start = start.Subtract(start.TimeOfDay);
                DateTime end = dteEndDate.SelectedDate.Value;
                end = end.Subtract(end.TimeOfDay).Add(new TimeSpan(23, 59, 59));
                Facade.IJob facJob = new Facade.Job();
                
                DataSet dsReport = facJob.GetJobPricingData(identityId, dteStartDate.SelectedDate.Value, dteEndDate.SelectedDate.Value);
                Session["__ExportDS"] = dsReport.Tables[1];
                Server.Transfer("../Reports/csvexport.aspx?filename=runpricing.csv");
            }
        }
        private void btnGenerate_Click(object sender, System.EventArgs e)
        {
            if (Page.IsValid)
                LoadReport();
        }

        #endregion

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