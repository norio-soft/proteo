using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Orchestrator.Entities;
using Orchestrator.Globals;
using P1TP.Components.Web.Validation;
using Orchestrator.WebUI.Security;

namespace Orchestrator.WebUI.Invoicing
{
    /// <summary>
    /// Summary description for Multi Invoice Preparation.
    /// </summary>
    public partial class MultiInvoicePrepation : Orchestrator.Base.BasePage
    {
        #region Constants & Enums
        private const int C_HOURSPREVIOUS = 4;	// Past hours to show
        private const int C_HOURSFOLLOWING = 4;	// Following hours to show
        private const int C_HOURSTOTAL = 36;	// Total hours to show
        private const int C_HOURSMOVE = 6;		// Hours to move when going back / forward

        private const string C_HOURS_PREVIOUS_VS = "C_HOURS_PREVIOUS_VS";
        private const string C_HOURS_FOLLOWING_VS = "C_HOURS_FOLLOWING_VS";
        private const string C_SORT_EXPRESSION_VS = "C_SORT_EXPRESSION_VS";
        private const string C_SORT_DIRECTION_VS = "C_SORT_DIRECTION_VS";

        private static readonly TimeSpan C_TIMESPAN = new TimeSpan(0, 0, 30, 0, 0);
        private static readonly TimeSpan C_MOVEONREFRESH = C_TIMESPAN;

        private const string C_INVOICEPRINT_ARRAY_VS = "InvoicePrint";

        #endregion

        #region Page Variables

        private int m_IdentityId = 0;
        private string jobIdCSV = String.Empty;
        private NameValueCollection m_batches = new NameValueCollection();

        #endregion

        #region Property Interfaces

        private int HoursPrevious
        {
            get
            {
                if (ViewState[C_HOURS_PREVIOUS_VS] == null)
                {
                    HoursPrevious = 6;
                    return 6;
                }
                else
                    return (int)ViewState[C_HOURS_PREVIOUS_VS];
            }
            set
            {
                ViewState[C_HOURS_PREVIOUS_VS] = value;
            }
        }
        private int HoursFollowing
        {
            get
            {
                if (ViewState[C_HOURS_FOLLOWING_VS] == null)
                {
                    HoursFollowing = 3;
                    return 3;
                }
                else
                    return (int)ViewState[C_HOURS_FOLLOWING_VS];
            }
            set
            {
                ViewState[C_HOURS_FOLLOWING_VS] = value;
            }
        }
        private string SortExpression
        {
            get
            {
                if (ViewState[C_SORT_EXPRESSION_VS] == null)
                    return "CompleteDate";
                else
                    return (string)ViewState[C_SORT_EXPRESSION_VS];
            }
            set
            {
                ViewState[C_SORT_EXPRESSION_VS] = value;
            }
        }
        private string SortDirection
        {
            get
            {
                if (ViewState[C_SORT_DIRECTION_VS] == null)
                    return "ASC";
                else
                    return (string)ViewState[C_SORT_DIRECTION_VS];
            }
            set
            {
                ViewState[C_SORT_DIRECTION_VS] = value;
            }
        }

        #endregion

        #region Page/Load/Init/Error
        protected void Page_Load(object sender, System.EventArgs e)
        {
            Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.Invoicing);

            // Update Invoice
            if (Request.QueryString["IdentityId"] != null)
                m_IdentityId = Convert.ToInt32(Request.QueryString["IdentityId"]);

            if (!IsPostBack)
            {
                //CLEAR INVOICE SESSION VARIABLES

                ClearFields();

                PopulateStaticControls();

                lblClient.Text = "Client";
                cboClient.Visible = true;

                LoadBatchInvoices();

                if (m_IdentityId != 0)
                    LoadGrid();
            }

            pnlConfirmation.Visible = false;
        }

        protected void JobsToInvoice_Init(object sender, EventArgs e)
        {
            this.btnClear.Click += new System.EventHandler(btnClear_Click);
            this.btnClear1.Click += new System.EventHandler(btnClear_Click);

            this.btnFilter.Click += new System.EventHandler(btnFilter_Click);
            this.btnFilter1.Click += new EventHandler(btnFilter_Click);

            this.btnSaveBatch.Click += new EventHandler(btnSaveBatch_Click);
            this.btnSaveBatch1.Click += new EventHandler(btnSaveBatch_Click);

            this.cboClient.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);
            this.cboClient.SelectedIndexChanged += new Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventHandler(cboClient_SelectedIndexChanged);

            gvInvoiceBatch.RowDataBound += new GridViewRowEventHandler(gvInvoiceBatch_RowDataBound);
            gvInvoiceBatch.RowCommand += new GridViewCommandEventHandler(gvInvoiceBatch_RowCommand);

            MyClientSideAnchor.OnWindowClose += new Codesummit.WebModalAnchor.OnWindowCloseEventHandler(MyClientSideAnchor_OnWindowClose);

            this.dlJob.ItemDataBound += new DataListItemEventHandler(dlJob_ItemDataBound);

            this.rdoSortType.SelectedIndexChanged += new EventHandler(rdoSortType_SelectedIndexChanged);
        }

        void gvInvoiceBatch_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                CheckBox chkInclude = (CheckBox)e.Row.FindControl("chkInclude");
                int batchId = Convert.ToInt32(e.Row.Cells[0].Text);
                chkInclude.Attributes.Add("onClick", "javascript:GetCheckedBatches(" + batchId.ToString() + ", this);");

                if (hidBatchIdCSV.Value.Contains(batchId.ToString() + ","))
                    chkInclude.Checked = true;
            }
        }

        void gvInvoiceBatch_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            switch (e.CommandName.ToLower())
            {
                case "remove":
                    int batchId = int.Parse((gvInvoiceBatch.Rows[Convert.ToInt32(e.CommandArgument)]).Cells[0].Text);
                    RemoveBatch(batchId);
                    if (hidBatchIdCSV.Value.Contains(batchId.ToString() + ","))
                        hidBatchIdCSV.Value = hidBatchIdCSV.Value.Replace(batchId.ToString() + ",", "");

                    if (dlJob.Visible == true)
                        LoadGrid();
                    LoadBatchInvoices();

                    break;
            }
        }

        void MyClientSideAnchor_OnWindowClose(Codesummit.WebModalAnchor sender)
        {
            string outputData = sender.OutputData;

            if (outputData.Length > 0)
            {
                if (outputData.StartsWith("<createSingleInvoice"))
                {
                    // Retrieve the invoice id and display it on the page.
                    System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
                    doc.LoadXml(outputData);

                    string invoiceId = doc.DocumentElement.GetAttribute("invoiceId");

                    lblConfirmation.Text = "Your invoice has been created with Invoice No: " + invoiceId + ".";
                    pnlConfirmation.Visible = true;
                    lblConfirmation.ForeColor = Color.Blue;
                }
            }

            hidBatchIdCSV.Value = string.Empty;

            LoadBatchInvoices();

            if (cboClient.SelectedValue != string.Empty)
                LoadGrid();
            else
                ClearFields();
        }

        void dgInvoiceBatch_DeleteCommand(object sender, ComponentArt.Web.UI.GridItemEventArgs e)
        {
            RemoveBatch(Convert.ToInt32(e.Item[0].ToString()));

            if (dlJob.Visible == true)
                LoadGrid();

            LoadBatchInvoices();
        }

        void dgInvoiceBatch_UpdateCommand(object sender, ComponentArt.Web.UI.GridItemEventArgs e)
        {
            ViewBatch(Convert.ToInt32(e.Item[0].ToString()), 0, string.Empty);
        }

        void dgInvoiceBatch_NeedDataSource(object sender, EventArgs e)
        {
            LoadBatchInvoices();
        }

        void dgInvoiceBatch_NeedRebind(object sender, EventArgs e)
        {
            LoadBatchInvoices();
        }

        void cboClient_SelectedIndexChanged(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if (cboClient.SelectedItem != null)
                m_IdentityId = int.Parse(cboClient.SelectedValue);

            if (m_IdentityId != 0)
            {
                SortExpression = string.Empty;
                SortDirection = string.Empty;
                LoadGrid();
                LoadBatchInvoices();
            }
        }

        #endregion

        #region Populate Static Controls
        private void PopulateStaticControls()
        {
            // Invoice Sort Type
            rdoSortType.DataSource = Utilities.UnCamelCase(Enum.GetNames(typeof(eInvoiceSortType)));
            rdoSortType.DataBind();
            rdoSortType.Items[0].Selected = true;

            if (m_IdentityId != 0)
            {
                //Get the Client name for id.
                Facade.IOrganisation facOrg = new Facade.Organisation();
                string name = facOrg.GetNameForIdentityId(m_IdentityId);

                cboClient.SelectedValue = m_IdentityId.ToString();
                cboClient.Text = name;

                pnlFilter.Visible = true;
            }
        }
        #endregion

        #region DBCombo's Server Methods and Initialisation
        void cboClient_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            cboClient.Items.Clear();

            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllNormalClientsFiltered(e.Text);

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

        #region Methods & Event Handlers
        #region Other Events
        private void ClearFields()
        {
            pnlFilter.Visible = true;
            pnlNormalJob.Visible = false;

            btnFilter.Visible = true;
            btnFilter1.Visible = true;

            btnClear.Visible = false;
            btnClear1.Visible = false;

            // Reset the client fields
            cboClient.Visible = true;
            cboClient.Text = string.Empty;
            cboClient.SelectedValue = string.Empty;

            // Date Fields
            dteStartDate.SelectedDate = DateTime.MinValue;
            dteEndDate.SelectedDate = DateTime.MinValue;

            lblJobCount.Visible = false;
        }

        #endregion

        #region Button Events
        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            btnFilter.DisableServerSideValidation();

            if (Page.IsValid)
                LoadGrid();
        }

        private void btnSaveBatch_Click(object sender, EventArgs e)
        {
            bool success = GenerateBatch();

            if (success)
            {
                // Success message
                LoadGrid();
                LoadBatchInvoices();
                lblConfirmation.Text = "The batch invoice was saved successfully.";
                pnlConfirmation.Visible = true;
                lblConfirmation.ForeColor = Color.Blue;
            }
            else // Failure Message
            {
                lblConfirmation.Text = "The batch invoice was unsuccessfully saved. This could be beacuse there were multiple clients sleected for the same Batch Id.";
                pnlConfirmation.Visible = true;
                lblConfirmation.ForeColor = Color.Red;
            }

        }

        protected bool GenerateBatch()
        {
            bool success = true;

            NameValueCollection clients = new NameValueCollection();
            NameValueCollection batchNo = new NameValueCollection();

            foreach (DataListItem item in dlJob.Items)
            {
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    string batchId = ((TextBox)item.FindControl("txtBatchNo")).Text;
                    string jobId = ((LinkButton)item.FindControl("lnkJobId")).Text;
                    string identityId = ((HtmlInputHidden)item.FindControl("hidIdentityId")).Value;

                    if (batchId != String.Empty)
                    {
                        HtmlImage imgError = (HtmlImage)item.FindControl("imgError");
                        TextBox txtBatchNo = (TextBox)item.FindControl("txtBatchNo");

                        // Check whether this is a new batch and if 
                        // so make sure the job added is from the same client
                        if (m_batches.Get(batchId) == null)
                        {
                            m_batches.Add(batchId, jobId);
                            clients.Add(batchId, identityId);
                            batchNo.Add(batchId, batchId);
                            imgError.Visible = false;
                            txtBatchNo.BackColor = Color.White;
                            txtBatchNo.ToolTip = string.Empty;
                        }
                        else
                        {
                            if (identityId == clients.Get(batchId))
                            {
                                m_batches.Set(batchId, m_batches.Get(batchId) + "," + jobId);
                                imgError.Visible = false;
                                txtBatchNo.BackColor = Color.White;
                                txtBatchNo.ToolTip = string.Empty;
                            }
                            else
                            {
                                txtBatchNo.BackColor = Color.Red;
                                txtBatchNo.ToolTip = imgError.Alt;
                                imgError.Visible = true;
                                success = false;
                            }
                        }
                    }
                }
            }

            if (!success)
                return success;

            // Record batches that have been successful 
            Facade.IInvoiceBatches facInv = new Facade.Invoice();
            bool marker = false;
            string userId = ((Entities.CustomPrincipal)Page.User).UserName;

            for (int i = 0; i < m_batches.Count; i++)
            {
                int batchId = Convert.ToInt32(batchNo[i].ToString());
                string jobIds = m_batches[i].ToString();

                // Check whether these are OLD or NEW invoice batches either create or update
                bool exists = facInv.CheckBatchExists(batchId);

                if (exists)
                {
                    facInv.Update(batchId, jobIds, userId); // UPDATE If it has a Batch Id
                    marker = true;
                }
                else
                {
                    Entities.FacadeResult result = facInv.Create(batchId, jobIds, userId); // CREATE If it hasn't an Id 

                    if (result.Success)
                        marker = true;
                    else
                    {
                        infringementDisplay.Infringements = result.Infringements;
                        infringementDisplay.DisplayInfringments();

                        marker = false;
                    }
                }
            }

            return marker;
        }
        #endregion

        #region Normal Jobs To Invoice
        private void dlJob_ItemDataBound(object sender, DataListItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                // These attributes are required by the "remember where I am" yellow highlight functionality.
                e.Item.Attributes.Add("onClick", "javascript:HighlightRow('" + e.Item.ClientID + "');");
                e.Item.Attributes.Add("id", e.Item.ClientID);

                // References
                Facade.IJobReference facJobReference = new Facade.Job();

                LinkButton jobId = (LinkButton)e.Item.FindControl("lnkJobId");
                jobId.Attributes.Add("OnClick", "javascript:openDialogWithScrollbars('../traffic/JobManagement.aspx?wiz=true&jobId=" + jobId.Text + "'+ getCSID());");

                Repeater repReferences = (Repeater)e.Item.FindControl("repReferences");

                JobReferenceCollection jrc = facJobReference.GetJobReferences(Convert.ToInt32(jobId.Text));

                repReferences.DataSource = jrc;
                repReferences.DataBind();

                // Customers
                Facade.IJob facJobCustomer = new Facade.Job();

                Repeater repCustomers = (Repeater)e.Item.FindControl("repCustomers");

                DataSet ds = facJobCustomer.GetJobCustomers(Convert.ToInt32(jobId.Text));

                repCustomers.DataSource = ds;
                repCustomers.DataBind();

                // Account On Hold Handling
                HtmlInputHidden hidOnHold = (HtmlInputHidden)e.Item.FindControl("hidOnHold");
                TextBox txtBatch = (TextBox)e.Item.FindControl("txtBatchNo");

                if (hidOnHold.Value != "False")
                    txtBatch.Visible = false;
                else
                    txtBatch.Visible = true;
            }
        }

        private void LoadGrid()
        {
            int clientId = 0;
            DateTime startDate = DateTime.MinValue;
            DateTime endDate = DateTime.MinValue;
            eJobState jobState = new eJobState();

            Facade.IOrganisation facOrg = new Facade.Organisation();
            Entities.Organisation enOrg = new Entities.Organisation();

            // Client
            if (cboClient.Text != "")
            {
                clientId = Convert.ToInt32(cboClient.SelectedValue);
                pnlFilter.Visible = true;
            }

            if (cboClient.SelectedValue != "")
            {
                clientId = Convert.ToInt32(cboClient.SelectedValue);
                enOrg = facOrg.GetForIdentityId(Convert.ToInt32(cboClient.SelectedValue));
                cboClient.Text = enOrg.OrganisationName;
                pnlFilter.Visible = true;
            }
            //else
            //    pnlFilter.Visible = false;

            // Date Range
            if (dteStartDate.SelectedDate != DateTime.MinValue)
            {
                startDate = dteStartDate.SelectedDate.Value;
                startDate = startDate.Subtract(startDate.TimeOfDay);
            }

            if (dteEndDate.SelectedDate != DateTime.MinValue)
            {
                endDate = dteEndDate.SelectedDate.Value;
                endDate = endDate.Subtract(endDate.TimeOfDay);
                endDate = endDate.Add(new TimeSpan(23, 59, 59));
            }

            // Get Jobs to Invoice
            Facade.IInvoice facInvoice = new Facade.Invoice();
            DataSet dsInvoicing;

            bool posted = false;

            if (startDate != DateTime.MinValue || endDate != DateTime.MinValue)
                dsInvoicing = facInvoice.GetJobsToInvoiceWithParamsAndDate(clientId, jobState, posted, startDate, endDate);
            else
            {
                if (clientId == 0)
                    dsInvoicing = facInvoice.GetAllJobsToInvoice();
                else
                    dsInvoicing = facInvoice.GetJobsToInvoiceWithParams(clientId, jobState, posted);
            }

            // Check whether account is on hold
            if (dsInvoicing.Tables[0].Rows.Count > 0)
            {
                if (Convert.ToInt32(dsInvoicing.Tables[0].Rows[0]["OnHold"]) == 1)
                {
                    dlJob.Enabled = false;

                    if (string.IsNullOrEmpty(cboClient.Text))
                    {
                        lblOnHold.Visible = true;
                        lblOnHold.Text = "Client accounts are on hold, please update their on-hold status in order to raise invoices.</A>";
                    }
                    else
                    {
                        lblOnHold.Visible = true;
                        lblOnHold.Text = cboClient.Text + "'s account has been put on hold, please go to <A HREF=../Organisation/addupdateorganisation.aspx?IdentityId=" + Convert.ToInt32(cboClient.SelectedValue) + ">" + cboClient.Text + "'s details to change.</A>";
                    }
                }
                else
                    lblOnHold.Visible = false;

                dlJob.Visible = true;
            }
            else
            {
                lblOnHold.Visible = true;
                lblOnHold.Text = "With the given parameters no jobs have been found.";
                dlJob.Visible = false;
            }

            DataView dvInvoice = new DataView(dsInvoicing.Tables[0]);

            // Sort By
            foreach (ListItem sortField in rdoSortType.Items)
            {
                if (sortField.Selected)
                    dvInvoice.Sort = sortField.Text.Replace(" ", "");
            }

            // Load List 
            dlJob.DataSource = dvInvoice;

            dlJob.DataBind();

            lblJobCount.Text = "There are " + dlJob.Items.Count.ToString() + " jobs ready to invoice.";

            pnlNormalJob.Visible = true;
            btnFilter.Visible = true;
            btnFilter1.Visible = true;
            btnClear.Visible = true;
            btnClear1.Visible = true;
            pnlSort.Visible = true;
            lblJobCount.Visible = true;
        }


        protected void lnkJobId_Click(object sender, EventArgs e)
        {
            Response.Redirect("javascript:openDialogWithScrollbars('../traffic/JobManagement.aspx?wiz=true&jobId=" + ((LinkButton)sender).Text + "' + getCSID(),'600','400');");
        }

        #endregion

        #region Date Events
        private string GetTimeDefinitions()
        {
            StringBuilder sb = new StringBuilder();

            // Configure the display range
            sb.Append("&HoursPrevious=");
            sb.Append(HoursPrevious.ToString());
            sb.Append("&HoursFollowing=");
            sb.Append(HoursFollowing.ToString());

            DateTime startDate = Convert.ToDateTime(dteStartDate.Text);
            DateTime endDate = Convert.ToDateTime(dteEndDate.Text);

            // Configure the search range
            // Attach the date information the url being built up
            sb.Append("&StartDate=");
            sb.Append(startDate.ToString("dd/MM/yy"));
            sb.Append("&StartTime=");
            sb.Append(startDate.ToString("HH:mm"));
            sb.Append("&EndDate=");
            sb.Append(endDate.ToString("dd/MM/yy"));
            sb.Append("&EndTime=");
            sb.Append(endDate.ToString("HH:mm"));

            return sb.ToString();
        }

        #endregion

        #region Sort Events
        private void rdoSortType_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Reload Grid
            LoadGrid();
        }

        #endregion

        #region Batch Invoice Grid
        void dgJobs_SortCommand(object sender, ComponentArt.Web.UI.GridSortCommandEventArgs e)
        {
            if (e.SortExpression == SortExpression)
            {
                if (SortDirection == "DESC")
                    SortDirection = "ASC";
                else
                    SortDirection = "DESC";
            }
            else
            {
                SortExpression = e.SortExpression;
                SortDirection = "ASC";
            }

            LoadGrid();
        }

        void dgJobs_ItemContentCreated(object sender, ComponentArt.Web.UI.GridItemContentCreatedEventArgs e)
        {
            int jobId = 0;

            jobId = Convert.ToInt32(e.Item["JobId"]);

            TextBox txtBatchNo = (TextBox)e.Content.FindControl("txtBatchNo");
            Repeater repDeliveryPoints = (Repeater)e.Content.FindControl("repDeliveryPoints");
            Repeater repReferences = (Repeater)e.Content.FindControl("repReferences");

            if (txtBatchNo != null)
            {
                if (cboClient.SelectedValue != string.Empty)
                    txtBatchNo.Enabled = true;
                else
                    txtBatchNo.Enabled = false;
            }

            if (repReferences != null)
            {
                // References
                Facade.IJobReference facJobReference = new Facade.Job();
                repReferences.DataSource = facJobReference.GetJobReferences(jobId);
                repReferences.DataBind();
            }

            if (repDeliveryPoints != null)
            {
                // Customers
                Facade.IJob facJobCustomer = new Facade.Job();
                DataSet ds = facJobCustomer.GetJobCustomers(jobId);
                repDeliveryPoints.DataSource = ds;
                repDeliveryPoints.DataBind();
            }
        }

        private void ViewBatch(int batchId, int clientId, string jobIds)
        {
            ArrayList selectedJobs = new ArrayList();
            string[] job = jobIds.Split(',');

            for (int i = 0; i < job.Length; i++)
            {
                selectedJobs.Add(job[i].ToString());
            }

            //#15867 J.Steele 
            //Clear the Invoice Session variables before setting the specific ones
            Utilities.ClearInvoiceSession();
            Session["JobIds"] = selectedJobs;
            Session["ClientId"] = clientId;
            Session["BatchId"] = batchId;
            Server.Transfer("addupdateinvoice.aspx");
        }

        protected void repReferences_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Entities.JobReference reference = (Entities.JobReference)e.Item.DataItem;
            }
        }

        private void RemoveBatch(int batchId)
        {
            // Remove Batch From List and Reload The Invoice
            Facade.IInvoiceBatches facInv = new Facade.Invoice();

            string userId = ((Entities.CustomPrincipal)Page.User).UserName;

            bool success = facInv.Delete(batchId, userId);

            if (success)
            {
                LoadGrid();
                LoadBatchInvoices();
            }
        }

        #endregion

        #region Load Batch Invoices

        private void LoadBatchInvoices()
        {
            Facade.IInvoiceBatches facInv = new Orchestrator.Facade.Invoice();

            DataSet dsInvoice = facInv.GetAllBatches();

            // Put in dummy checkbox column
            dsInvoice.Tables[0].Columns.Add("Include", typeof(Boolean));

            //dgInvoiceBatch.DataSource = dsInvoice;
            gvInvoiceBatch.DataSource = dsInvoice;

            //dgInvoiceBatch.DataBind();
            gvInvoiceBatch.DataBind();

            if (dsInvoice.Tables[0].Rows.Count > 0)
            {
                //dgInvoiceBatch.Visible = true;
                gvInvoiceBatch.Visible = true;
                pnlInvoiceBatch.Visible = true;
                lblNote.Visible = false;
                btnAddUpdateMutliInvoice.Visible = true;
            }
            else
            {
                lblNote.Text = "No prepared batches";
                lblNote.ForeColor = Color.Blue;
                lblNote.Visible = true;
                //dgInvoiceBatch.Visible = false;
                pnlInvoiceBatch.Visible = true;
                btnAddUpdateMutliInvoice.Visible = false;
            }
        }
        #endregion
        #endregion

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Init += new System.EventHandler(this.JobsToInvoice_Init);
        }
        #endregion
    }
}
