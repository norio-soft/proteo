using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Collections.Specialized;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.POD
{
    public partial class outstandingPods : Orchestrator.Base.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                // Get filter params from the querystring, but only do this on initial load so we dont screw subsequent filtering
                // If querystring param not found then default values will be applied:
                int jobId = -1;
                int orderId = -1;
                DateTime startDate = DateTime.Today;
                DateTime endDate = DateTime.Today.AddDays(1);

                string qsJobId = getQueryStringValue("jobID");
                if (qsJobId != String.Empty)
                {
                    if (!int.TryParse(qsJobId, out jobId)) jobId = -1;
                }

                string qsOrderId = getQueryStringValue("orderID");
                if (qsOrderId != String.Empty)
                {
                    if (!int.TryParse(qsOrderId, out orderId)) orderId = -1;
                }

                string qsStartDate = getQueryStringValue("startDate");
                if (qsStartDate != String.Empty)
                {
                    if (!DateTime.TryParse(qsStartDate, out startDate)) startDate = DateTime.Today;
                }

                string qsEndDate = getQueryStringValue("endDate");
                if (qsEndDate != String.Empty)
                {
                    if (!DateTime.TryParse(qsEndDate, out endDate)) endDate = DateTime.Today.AddDays(1);
                }

                // Now apply any values we found into the form:
                dteStartDate.SelectedDate = startDate;
                dteEndDate.SelectedDate = endDate;

                // Terrible UI! :/ JobId and OrderId are mutually exclusive:
                if (jobId > -1)
                {
                    cblSearchFor.Items.FindByValue("JOBID").Selected = true;
                    txtSearchFor.Text = jobId.ToString();
                }
                else if (orderId > -1)
                {
                    cblSearchFor.Items.FindByValue("ORDERID").Selected = true;
                    txtSearchFor.Text = orderId.ToString();
                }

                if (jobId > -1 || orderId > -1)
                {   // If a job (run) or order id was in the QS then lets run the search for the user
                    DataSet dsOutStandingPODs = GetData();
                    gvOutstandingPOD.DataSource = dsOutStandingPODs;
                    if (dsOutStandingPODs != null)
                        GetOutstandingPODReportData(dsOutStandingPODs);
                }
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            // Attach events
            this.btnReport.Click += new EventHandler(btnReport_Click);
            this.gvOutstandingPOD.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(gvOutstandingPOD_NeedDataSource);
            this.dlgDocumentWizard.DialogCallBack += new EventHandler(dlgDocumentWizard_DialogCallBack);
            this.btnReset.Click += new EventHandler(btnReset_Click);

            // Default the search checkboxes as per settings:
            resetFilter();
        }

        protected void btnReset_Click(object sender, EventArgs e)
        {
            resetFilter();
            DataSet dsOutStandingPODs = GetData();
            gvOutstandingPOD.DataSource = dsOutStandingPODs;
            if (dsOutStandingPODs != null)
                GetOutstandingPODReportData(dsOutStandingPODs, true);
        }

        //-----------------------------------------------------------------------------------

        protected void dlgDocumentWizard_DialogCallBack(object sender, EventArgs e)
        {
            this.gvOutstandingPOD.Rebind();
        }

        protected void dlgRecordPODs_DialogCallBack(object sender, EventArgs e)
        {
            this.gvOutstandingPOD.Rebind();
        }

        //-----------------------------------------------------------------------------------

        protected void gvOutstandingPOD_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            if (this.IsPostBack)
            {
                DataSet dsOutStandingPODs = this.GetData();
                
                this.gvOutstandingPOD.DataSource = dsOutStandingPODs;

                if (dsOutStandingPODs != null)
                    this.GetOutstandingPODReportData(dsOutStandingPODs);
            }
        }

        

        //-----------------------------------------------------------------------------------

        private void btnReport_Click(object sender, EventArgs e)
        {
            btnReport.DisableServerSideValidation();

            if (Page.IsValid)
            {
                // Retrieve the report data and place it in the session variables
                this.gvOutstandingPOD.Rebind();
            }
            else
            {
                // Hide the report
                reportViewer.Visible = false;
                gvOutstandingPOD.Visible = false;
            }
        }

        //-----------------------------------------------------------------------------------

        /// <summary>
        /// Run the search using current fitler values.
        /// </summary>
        private DataSet GetData()
        {
            // Get filter values from form:
            string stringFilterValue = txtSearchFor.Text.Trim();
            string orderID = cblSearchFor.Items.FindByValue("ORDERID").Selected ? stringFilterValue : String.Empty;
            string loadNo = cblSearchFor.Items.FindByValue("LOADNO").Selected ? stringFilterValue : String.Empty;
            string docketNo = cblSearchFor.Items.FindByValue("DOCKETNO").Selected ? stringFilterValue : String.Empty;
            string runID = cblSearchFor.Items.FindByValue("JOBID").Selected ? stringFilterValue : String.Empty;

            int driverId = String.IsNullOrEmpty(cboDriver.SelectedValue) ? -1 : Int32.Parse(cboDriver.SelectedValue);

            DateTime selectedStartDate = dteStartDate.SelectedDate.Value;
            DateTime selectedEndDate = dteEndDate.SelectedDate.Value;
            DateTime startDateFilter = selectedStartDate.Subtract(selectedStartDate.TimeOfDay);                         // Set h:m:s to 00:00.00
            DateTime endDateFilter = selectedEndDate.Subtract(selectedEndDate.TimeOfDay).Add(new TimeSpan(23, 59, 59)); // Set h:m:s to 23:59.59

            bool includeCheckedInPODs = chkIncludeCheckedInPODs.Checked;
            bool includeResourced = chkIncludeResourced.Checked;

            // Originally, you could pass a param into this method to ignore any date filters; However, I have since fixed the querystring weirdness so this is no 
            // longer necessary though updating the SPROC was too much work so I just left it in place in case it becomes useful in future.
            bool ignoreDates = false;

            // Now run the search:
            Facade.IPOD facPOD = new Facade.POD();
            return facPOD.GetOutstandingForDriverOrSubby(driverId, startDateFilter, endDateFilter, orderID, loadNo, docketNo, runID, includeCheckedInPODs, ignoreDates, includeResourced);
        }

        //-----------------------------------------------------------------------------------

        private void GetOutstandingPODReportData(DataSet dsOutstandingPODs, bool invokedByReset = false)
        {
            if (dsOutstandingPODs.Tables[0].Rows.Count > 0)
            {
                // Configure the report settings collection
                NameValueCollection reportParams = new NameValueCollection();

                string driverId = "-1";
                string driverName = "All";

                if (!String.IsNullOrEmpty(this.cboDriver.SelectedValue))
                {
                    driverId = this.cboDriver.SelectedValue;
                    driverName = cboDriver.Text;
                }

                reportParams.Add("Driver", driverName);
                reportParams.Add("IdentityId", driverId);

                // Configure the Session variables used to pass data to the report
                Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.OutstandingPods;
                Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = dsOutstandingPODs;
                Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
                Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";
                Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;

                lblReportError.Visible = false;
                reportViewer.IdentityId = Globals.Configuration.IdentityId;
                // Show the user control
                reportViewer.Visible = true;

                gvOutstandingPOD.Visible = true;
            }
            else
            {
                lblReportError.Text = "No outstanding PODs found " + cboDriver.Text;
                reportViewer.Visible = false;
                lblReportError.Visible = true && !invokedByReset;
                gvOutstandingPOD.Visible = false;
            }
        }

        /// <summary>
        /// Reset all filter parameters back to initial settings
        /// </summary>
        private void resetFilter()
        {
            txtSearchFor.Text = String.Empty;
            cblSearchFor.Items.FindByValue("ORDERID").Selected = false;
            cblSearchFor.Items.FindByValue("LOADNO").Selected = true;
            cblSearchFor.Items.FindByValue("DOCKETNO").Selected = true;
            cblSearchFor.Items.FindByValue("JOBID").Selected = false;
            cboDriver.Text = String.Empty;
            cboDriver.SelectedValue = String.Empty;
            dteStartDate.SelectedDate = DateTime.Today;
            dteEndDate.SelectedDate = DateTime.Today.AddDays(1);
            chkIncludeCheckedInPODs.Checked = false;
            chkIncludeResourced.Checked = Globals.Configuration.OutstandingPODSearchIncludeResourcedDefault;
        }

        /// <summary>
        /// Gets the given named value from the querystring, if present, else String.Empty
        /// </summary>
        /// <param name="name">The querystring parameter name to retrieve</param>
        /// <returns>Value of named parameter, else String.Empty if not present</returns>
        private string getQueryStringValue(string name)
        {
            string result = String.Empty;
            if (Request.QueryString[name] != null)
            {
                result = Request.QueryString[name].ToString();
            }
            return result;
        }

    }

    //-----------------------------------------------------------------------------------

}
