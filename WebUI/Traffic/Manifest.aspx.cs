using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace Orchestrator.WebUI.Traffic
{
    public partial class Manifest : Orchestrator.Base.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                SetDefaultStartDateTime();
                this.txtExtraRowCount.Text = Globals.Configuration.DefaultResourceManifestNoOfBlankLines.ToString();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            cboResource.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboResource_ItemsRequested);
            cboSubContractor.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboSubContractor_ItemsRequested);
            gvManifest.RowDataBound += new GridViewRowEventHandler(gvManifest_RowDataBound);
            btnReset.Click += new EventHandler(btnReset_Click);
            btnGetJobs.Click += new EventHandler(btnGetJobs_Click);
            btnDisplayManifest.Click += new EventHandler(btnDisplayManifest_Click);
            btnProducePILs.Click += new EventHandler(btnProducePILs_Click);
        }

        #region Date Methods

        private void SetDefaultStartDateTime()
        {
            // The default start date is the start of today.
            DateTime defaultStartDateTime = DateTime.Now;
            defaultStartDateTime = defaultStartDateTime.Subtract(defaultStartDateTime.TimeOfDay);

            dteStartDate.SelectedDate = defaultStartDateTime;
            dteStartTime.SelectedDate = defaultStartDateTime;
        }

        private DateTime GetStartDateTime()
        {
            DateTime startDateTime = dteStartDate.SelectedDate.Value;
            startDateTime = startDateTime.Subtract(startDateTime.TimeOfDay);
            startDateTime = startDateTime.Add(dteStartTime.SelectedDate.Value.TimeOfDay);

            return startDateTime;
        }

        #endregion

        #region Event Handlers

        #region Combobox Event Handlers

        void cboResource_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            cboResource.Items.Clear();

            Facade.IResource facResource = new Facade.Resource();
            DataSet ds = facResource.GetAllResourcesFiltered(e.Text, false, false);

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
                rcItem.Text = dt.Rows[i]["Description"].ToString();
                rcItem.Value = dt.Rows[i]["ResourceId"].ToString();
                cboResource.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }
        }

        void cboSubContractor_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            cboSubContractor.Items.Clear();

            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllSubContractorsFiltered(e.Text, false);

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
                cboSubContractor.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }
        }

        #endregion

        #region Button Event Handlers

        void btnGetJobs_Click(object sender, EventArgs e)
        {
            btnGetJobs.DisableServerSideValidation();

            int resourceId = 0;
            int subContractorId = 0;

            pnlMatchedJobs.Visible = false;
            reportViewer.Visible = false;
            DataSet dsManifestData = null;
            Facade.IManifest facManifest = new Facade.Manifest();

            DateTime startDateTime = GetStartDateTime();

            lblLegCount.Text = string.Empty;
            if (int.TryParse(cboResource.SelectedValue, out resourceId))
            {
                lblLegCount.Text = "Searching for matches for resource.  ";
                dsManifestData = facManifest.GetManifestForResource(resourceId, startDateTime, chkIncludePlannedWork.Checked);
            }
            else if (int.TryParse(cboSubContractor.SelectedValue, out subContractorId))
            {
                lblLegCount.Text = "Searching for matches for sub-contractor.  ";
                dsManifestData = facManifest.GetManifestForSubContractor(subContractorId, startDateTime);
            }

            if (dsManifestData != null)
            {
                pnlMatchedJobs.Visible = true;
                gvManifest.DataSource = dsManifestData;
                gvManifest.DataBind();

                lblLegCount.Text += dsManifestData.Tables[0].Rows.Count + " legs matched for inclusion on manifest.";
                btnDisplayManifest_Click(this, new EventArgs());
            }
        }

        void btnDisplayManifest_Click(object sender, EventArgs e)
        {
            btnDisplayManifest.DisableServerSideValidation();

            int resourceId = 0;
            int subContractorId = 0;
            string resourceLabel = string.Empty;
            reportViewer.Visible = false;
            DataSet dsManifestData = null;
            Facade.IManifest facManifest = new Facade.Manifest();

            DateTime startDateTime = GetStartDateTime();

            List<int> jobIds = new List<int>();
            foreach (GridViewRow row in gvManifest.Rows)
                if (row.RowType == DataControlRowType.DataRow)
                {
                    CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");
                    if (chkSelect.Checked)
                    {
                        HiddenField hidJobId = (HiddenField)row.FindControl("hidJobId");
                        jobIds.Add(int.Parse(hidJobId.Value));
                    }
                }

            if (int.TryParse(cboResource.SelectedValue, out resourceId))
            {
                dsManifestData = facManifest.GetManifestForResource(resourceId, startDateTime, chkIncludePlannedWork.Checked, jobIds);
                resourceLabel = cboResource.Text;
            }
            else if (int.TryParse(cboSubContractor.SelectedValue, out subContractorId))
            {
                dsManifestData = facManifest.GetManifestForSubContractor(subContractorId, startDateTime, jobIds);
                resourceLabel = cboSubContractor.Text;
            }

            int extraRowCount = 5;
            int.TryParse(txtExtraRowCount.Text, out extraRowCount);

            if (dsManifestData != null && dsManifestData.Tables[0].Rows.Count > 0)
            {
                if (!string.IsNullOrEmpty(cboSubContractor.SelectedValue))
                    this.reportViewer.IdentityId = int.Parse(cboSubContractor.SelectedValue);

                for (int extraRows = 0; extraRows < extraRowCount; extraRows++)
                {
                    DataRow newRow = dsManifestData.Tables[0].NewRow();
                    dsManifestData.Tables[0].Rows.Add(newRow);
                }
                dsManifestData.AcceptChanges();

                // Configure the report settings collection
                NameValueCollection reportParams = new NameValueCollection();
                reportParams.Add("ResourceLabel", resourceLabel);
                reportParams.Add("StartDateTime", startDateTime.ToString("dd/MM/yy HH:mm"));
                reportParams.Add("ShowFullAddress", chkIncludeFullAddresses.Checked.ToString());
                reportParams.Add("SpecialInstructions", txtSpecialInstructions.Text.ToString());

                // Configure the Session variables used to pass data to the report
                Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.Manifest;
                Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = dsManifestData;
                Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
                Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";
                Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;

                reportViewer.Visible = true;
            }
        }

        void btnReset_Click(object sender, EventArgs e)
        {
            cboResource.Text = string.Empty;
            cboResource.SelectedValue = string.Empty;

            cboSubContractor.Text = string.Empty;
            cboSubContractor.SelectedValue = string.Empty;
            txtSpecialInstructions.Text = string.Empty;
            SetDefaultStartDateTime();

            pnlMatchedJobs.Visible = false;
            reportViewer.Visible = false;
        }

        void btnProducePILs_Click(object sender, EventArgs e)
        {
            Facade.IOrder facOrder = new Facade.Order();

            NameValueCollection reportParams = new NameValueCollection();
            DataSet dsPIL = null;
            List<int> orderIDs = new List<int>();
            List<int> jobIDs = new List<int>();
            foreach (GridViewRow row in gvManifest.Rows)
                if (row.RowType == DataControlRowType.DataRow)
                {
                    CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");
                    if (chkSelect.Checked)
                    {
                        HiddenField hidJobId = (HiddenField)row.FindControl("hidJobId");
                        int jobID = int.Parse(hidJobId.Value);
                        // Check if this is a groupage job or not.
                        List<Entities.Order> orders = facOrder.GetForJobID(jobID);

                        //GetPILData no longer accepts JobIds
                        //if (orders.Count == 0)
                        //{
                        //    if (!jobIDs.Contains(jobID))
                        //        jobIDs.Add(jobID);
                        //}
                        //else
                        //{
                        foreach (Entities.Order order in orders)
                            if (!orderIDs.Contains(order.OrderID))
                                orderIDs.Add(order.OrderID);
                        //}
                    }
                }

            Orchestrator.Facade.ICollectDropLoadOrder facLoadOrder = new Orchestrator.Facade.CollectDrop();

            if (orderIDs.Count > 0 || jobIDs.Count > 0)
            {
                #region Pop-up Report

                //dsPIL = facLoadOrder.GetPILData(Entities.Utilities.GetCSV(orderIDs), Entities.Utilities.GetCSV(jobIDs));
                dsPIL = facLoadOrder.GetPILData(Entities.Utilities.GetCSV(orderIDs));


                //-------------------------------------------------------------------------------------	
                //									Load Report Section 
                //-------------------------------------------------------------------------------------	
                Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.PIL;
                Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;
                Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = dsPIL;
                Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
                Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";

                // Show the user control
                Page.ClientScript.RegisterStartupScript(this.GetType(), "onload", "<script language=\"javascript\">window.open('" + Page.ResolveClientUrl("../reports/reportviewer.aspx?wiz=true") + "');</script>");

                #endregion
            }
        }

        #endregion

        #region GridView Event Handlers

        void gvManifest_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView drv = (DataRowView)e.Row.DataItem;
                HiddenField hidJobId = (HiddenField)e.Row.FindControl("hidJobId");
                //Label lblJobId = (Label)e.Row.FindControl("lblJobId");
                Label lblCustomer = (Label)e.Row.FindControl("lblCustomer");
                Label lblStartFrom = (Label)e.Row.FindControl("lblStartFrom");
                Label lblStartAt = (Label)e.Row.FindControl("lblStartAt");
                CheckBox chkSelect = (CheckBox)e.Row.FindControl("chkSelect");
                HtmlAnchor lnkItem = (HtmlAnchor)e.Row.FindControl("lnkItem");

                int jobId = int.Parse(drv["JobId"].ToString());
                hidJobId.Value = jobId.ToString();
                //lblJobId.Text = jobId.ToString();
                lblCustomer.Text = drv["Customer"].ToString();
                lblStartFrom.Text = drv["StartFrom"].ToString();
                lblStartAt.Text = drv["StartAtDisplay"].ToString();
                if ((bool)drv["IsOrder"])
                {
                    lnkItem.Title = "Update Order";
                    lnkItem.HRef = "javascript:viewOrderProfile(" + jobId.ToString() + ");";
                }
                else
                {
                    lnkItem.Title = "View Job Details";
                    lnkItem.HRef = "javascript:viewJobDetails(" + jobId.ToString() + ");";
                }

                if (e.Row.RowIndex > 0)
                {
                    HiddenField hidPreviousRowJobId = (HiddenField)gvManifest.Rows[e.Row.RowIndex - 1].FindControl("hidJobId");
                    int previousRowJobId = int.Parse(hidPreviousRowJobId.Value);
                    if (jobId == previousRowJobId)
                    {
                        // This is the next leg in a job so hide the checkbox and blank out the relevant data.
                        chkSelect.Visible = false;
                        //lblJobId.Text = string.Empty;
                        lblCustomer.Text = string.Empty;
                        lblStartFrom.Text = string.Empty;
                        lblStartAt.Text = string.Empty;
                    }
                    else
                        chkSelect.Checked = true;
                }
                else
                    chkSelect.Checked = true;
            }
            else if (e.Row.RowType == DataControlRowType.Header)
            {
                HtmlInputCheckBox chkAll = (HtmlInputCheckBox)e.Row.FindControl("chkAll");
                chkAll.Checked = true;
            }
        }

        #endregion

        #endregion
    }
}