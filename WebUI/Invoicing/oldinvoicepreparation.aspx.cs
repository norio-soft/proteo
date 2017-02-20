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

using ComponentArt.Web.UI;
using Orchestrator.Entities;
using Orchestrator.Globals;
using Orchestrator.WebUI.Security;

using P1TP.Components.Web.Validation;

namespace Orchestrator.WebUI.Invoicing
{
    /// <summary>
    /// Summary description for Invoice Preparation.
    /// </summary>
    public partial class InvoicePrepation : Orchestrator.Base.BasePage
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
        private const string C_EXPORTCSV_VS = "ExportCSV";
        #endregion

        #region Page Variables
        private int m_IdentityId = 0;
        private int m_resetDate = 0;
        private InvoicePreparation m_InvoicePreparation = null;
        private string jobIdCSV = String.Empty;
        private ArrayList m_selectedJobs = new ArrayList();
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

            // Reset Dates Marker 
            if (Request.QueryString["ResetDates"] != null)
                m_resetDate = Convert.ToInt32(Request.QueryString["ResetDates"]);

            if (cboClient.SelectedValue != "")
                m_IdentityId = Convert.ToInt32(cboClient.SelectedValue);

            // Update Invoice
            if (Request.QueryString["IdentityId"] != null)
            {
                btnSaveFilter.Visible = true;
                m_IdentityId = Convert.ToInt32(Request.QueryString["IdentityId"]);
            }
            else
                btnSaveFilter.Visible = false;

            if (Request.QueryString["SubIdentityId"] != null)
                m_IdentityId = Convert.ToInt32(Request.QueryString["SubIdentityId"]);

            if (!IsPostBack)
            {
                ClearFields();

                PopulateStaticControls();

                lblClient.Text = "Client";
                cboClient.Visible = true;

                if (m_IdentityId != 0)
                {
                    LoadGrid();
                }
            }
        }

        protected void JobsToInvoice_Init(object sender, EventArgs e)
        {
            this.btnExport.Click += new EventHandler(btnExport_Click);
            this.btnClear.Click += new System.EventHandler(btnClear_Click);
            this.btnClear2.Click += new System.EventHandler(btnClear_Click);

            this.btnFilter.Click += new System.EventHandler(btnFilter_Click);
            this.btnFilter2.Click += new System.EventHandler(btnFilter_Click);

            this.btnCreateInvoice.Click += new System.EventHandler(btnCreateInvoice_Click);
            this.btnCreateInvoice2.Click += new System.EventHandler(btnCreateInvoice_Click);

            this.chkMarkAll.CheckedChanged += new EventHandler(chkMarkAll_CheckedChanged);
            this.chkOnlyShowTicked.CheckedChanged += new EventHandler(chkOnlyShowTicked_CheckedChanged);

            this.btnSaveFilter.Click += new EventHandler(btnSaveFilter_Click);
            this.btnClearFilter.Click += new System.EventHandler(this.btnClearFilter_Click);
            this.btnLoadFilter.Click += new System.EventHandler(this.btnLoadFilter_Click);

            this.cboClient.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);
            this.cboClient.SelectedIndexChanged += new Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventHandler(cboClient_SelectedIndexChanged);

            this.dgJobs.ItemContentCreated += new ComponentArt.Web.UI.Grid.ItemContentCreatedEventHandler(dgJobs_ItemContentCreated);
            this.dgJobs.SortCommand += new ComponentArt.Web.UI.Grid.SortCommandEventHandler(dgJobs_SortCommand);
        }

        void btnExport_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                //  Get data set and then put in session
                DataView dvExport = new DataView();

                dvExport = (DataView)Session[C_EXPORTCSV_VS];

                Session["__ExportDS"] = dvExport.Table; 

                Server.Transfer("../Reports/csvexport.aspx?filename=InvoicePreparation.csv");
            }
        }
        #endregion
        
        #region Populate Static Controls
        private void PopulateStaticControls()
        {
            cboJobState.DataSource = Utilities.UnCamelCase(Enum.GetNames(typeof(eJobState)));
            cboJobState.DataBind();
            cboJobState.SelectedValue = Utilities.UnCamelCase(eJobState.ReadyToInvoice.ToString());

            if (m_IdentityId != 0)
            {
                //Get the Client name for id.
                Facade.IOrganisation facOrg = new Facade.Organisation();
                string name = facOrg.GetNameForIdentityId(m_IdentityId);

                cboClient.SelectedValue = m_IdentityId.ToString();
                cboClient.Text = name;

                chkMarkAll.Visible = true;
                btnSaveFilter.Visible = true;
            }
        }
        #endregion

        #region DBCombo's Server Methods and Initialisation
        void cboClient_SelectedIndexChanged(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if (cboClient.SelectedItem != null)
                m_IdentityId = int.Parse(cboClient.SelectedItem.Value);

            if (m_IdentityId != 0)
            {
                SortExpression = string.Empty;
                SortDirection = string.Empty;
                LoadGrid();
                dgJobs.Levels[0].Columns[1].Visible = true;
            }
        }

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
            pnlNormalJob.Visible = false;

            btnFilter.Visible = btnFilter2.Visible = true;
            btnCreateInvoice.Visible = btnCreateInvoice2.Visible = false;
            btnClear.Visible = btnClear2.Visible = false;

            // Reset the client fields
            cboClient.Text = string.Empty;
            cboClient.SelectedValue = string.Empty;

            lblJobState.Visible = false;
            cboJobState.Visible = false;

            // Filter Options Reset
            btnClearFilter.Visible = false;
            btnSaveFilter.Visible = false;
            lblSaveProgressNotification.Visible = false;

            dteEndDate.SelectedDate = DateTime.MinValue;
            dteStartDate.SelectedDate = DateTime.MinValue;

            // Clear Check Boxes
            chkMarkAll.Checked = chkOnlyShowTicked.Checked = false;

            // Clear hidden variables
            hidSelectedJobs.Value = String.Empty;
            hidJobCount.Value = "0";
            hidJobTotal.Value = "0"; 
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
            btnFilter2.DisableServerSideValidation();

            if (Page.IsValid)
            {
                lblSaveProgressNotification.Visible = false;
                btnSaveFilter.Visible = true;
                hidSelectedJobs.Value = String.Empty;
                hidJobCount.Value = "0";
                hidJobTotal.Value = "0"; 
                LoadGrid();
            }
        }

        protected void btnCreateInvoice_Click(object sender, EventArgs e)
        {
            btnCreateInvoice.DisableServerSideValidation();

            ArrayList arrJobId = null;

            arrJobId = GetJobsSelected();

            if (arrJobId.Count != 0)
            {
                //#15867 J.Steele 
                //Clear the Invoice Session variables before setting the specific ones
                Utilities.ClearInvoiceSession();

                Session["StartDate"] = dteStartDate.SelectedDate.Value.ToString();
                Session["EndDate"] = dteEndDate.SelectedDate.Value.ToString();
                Session["JobIds"] = arrJobId;

                Session["ClientId"] = Convert.ToInt32(cboClient.SelectedValue);
                Session["ClientName"] = cboClient.Text;
                Session["Type"] = eInvoiceFilterType.NormalInvoice;
                Server.Transfer("addupdateinvoice.aspx");
            }
        }

        private void btnSaveFilter_Click(object sender, EventArgs e)
        {
            SaveInvoicePreparationProgress();
        }

        private void btnLoadFilter_Click(object sender, System.EventArgs e)
        {
            LoadInvoicePreparationProgress();
            btnClearFilter.Visible = false;
        }

        private void btnClearFilter_Click(object sender, System.EventArgs e)
        {
            ClearFields();
            LoadGrid();
            lblSaveProgressNotification.Visible = true;
            lblSaveProgressNotification.ForeColor = Color.Blue;
            lblSaveProgressNotification.Text = "The progress of your last search has been unloaded.  You can turn this on using the 'Load Filter' button.";
            btnClearFilter.Visible = false;
        }
        #endregion

        #region Normal Jobs To Invoice
        private void chkMarkAll_CheckedChanged(object sender, EventArgs e)
        {
            ComponentArt.Web.UI.GridItem griditemMyRow;
            decimal jobTotal = 0.0M;
            int jobCount = 0;

            for (int i = 0; i <= dgJobs.Items.Count - 1; i++)
            {
                griditemMyRow = dgJobs.Items[i];
                string jobId = griditemMyRow["JobId"].ToString ();

                if (chkMarkAll.Checked)
                {
                    griditemMyRow["Include"] = true;
                    jobTotal += Decimal.Parse(griditemMyRow["ChargeAmount"].ToString());
                    jobCount++;

                    if (hidSelectedJobs.Value.Length != 0)
                        hidSelectedJobs.Value = hidSelectedJobs.Value = ",";

                    hidSelectedJobs.Value += jobId;
                }
                else
                {
                    griditemMyRow["Include"] = false;
                    hidSelectedJobs.Value = String.Empty;
                }
            }

            if (chkMarkAll.Checked)
            {
                lblDetails.Text = "You have selected " + jobCount + " job(s), and the total amount is " + jobTotal.ToString("C");
                hidJobTotal.Value = jobTotal.ToString();
                hidJobCount.Value = jobCount.ToString();
            }
            else
            {
                hidJobTotal.Value = hidJobCount.Value = "0";
                lblDetails.Text = "";
            }
        }

        #region Main Grid Events
        void dgJobs_ItemContentCreated(object sender, ComponentArt.Web.UI.GridItemContentCreatedEventArgs e)
        {
            int jobId = 0;

            jobId = Convert.ToInt32(e.Item["JobId"]);

            CheckBox chkIncludeJob1 = (CheckBox)e.Content.FindControl("chkIncludeJob");
            Repeater repDeliveryPoints = (Repeater)e.Content.FindControl("repDeliveryPoints");
            Repeater repReferences = (Repeater)e.Content.FindControl("repReferences");

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

        void dgJobs_SortCommand(object sender, ComponentArt.Web.UI.GridSortCommandEventArgs e)
        {
            m_selectedJobs = GetJobsSelected();

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
        #endregion

        protected ArrayList GetJobsSelected()
        {
            string working = hidSelectedJobs.Value;
            ArrayList retval = new ArrayList();

            if (working.Length > 0)
                working = working.Substring(0, working.Length - 1);

            string[] jobIds = working.Split(',');

            foreach (string jobId in jobIds)
            {
                if (jobId.ToString() != string.Empty)
                    retval.Add(jobId);
            }
            return retval;
        }

        private void LoadGrid()
        {
            int clientId = 0;
            DateTime startDate = DateTime.MinValue;
            DateTime endDate = DateTime.MinValue;
            eJobState jobState = new eJobState();

            // Client
            if (cboClient.Text != "")
                clientId = Convert.ToInt32(cboClient.SelectedValue);

            // Job State
            if (cboJobState.SelectedValue != string.Empty)
                jobState = (eJobState)Enum.Parse(typeof(eJobState), cboJobState.SelectedValue.Replace(" ", ""));

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
            if (dsInvoicing.Tables.Count > 0)
            {
                if (dsInvoicing.Tables[0].Rows.Count > 0)
                {
                    if (Convert.ToInt32(dsInvoicing.Tables[0].Rows[0]["OnHold"]) == 1)
                    {
                        dgJobs.Enabled = false;
                        lblOnHold.Visible = true;
                        lblOnHold.Text = cboClient.Text + "'s account has been put on hold, please go to <A HREF=../Organisation/addupdateorganisation.aspx?IdentityId=" + Convert.ToInt32(cboClient.SelectedValue) + ">" + cboClient.Text + "'s details to change.</A>";
                    }
                    else
                        lblOnHold.Visible = false;

                    dgJobs.Visible = true;
                }
                else
                {
                    lblOnHold.Visible = true;
                    lblOnHold.Text = "With the given parameters no jobs have been found.";
                    dgJobs.Visible = false;
                    pnlNormalJob.Visible = false;
                    lblClient.Text = "Client";
                    cboClient.Visible = true;
                }

                // Put in dummy checkbox column
                dsInvoicing.Tables[0].Columns.Add("Include", typeof(Boolean));

                // Sort By
                DataView dvInvoice = new DataView(dsInvoicing.Tables[0]);

                if (chkOnlyShowTicked.Checked)
                {
                    if (m_selectedJobs.Count != 0)
                        dvInvoice.RowFilter = "jobId IN (" + hidSelectedJobs.Value + ")";
                }
                // Load List 
                dgJobs.DataSource = dvInvoice;

                if (cboClient.SelectedValue == String.Empty)
                    dgJobs.GroupBy = "OrganisationName";
                else
                {
                    dgJobs.Levels[0].AllowGrouping = false;
                    dgJobs.Levels[0].Columns["OrganisationName"].Visible = false;
                }

                Session[C_EXPORTCSV_VS] = dvInvoice;

                dgJobs.DataBind();

                // Check All Now We Have Filtered Down
                if (dgJobs.Items.Count >= 1)
                {
                    btnExport.Visible = true;
                    ComponentArt.Web.UI.GridItem griditemMyRow;
                    decimal jobTotal = 0.0M;
                    int jobCount = 0;

                    for (int i = 0; i <= dgJobs.Items.Count - 1; i++)
                    {
                        griditemMyRow = dgJobs.Items[i];

                        if (chkOnlyShowTicked.Checked)
                        {
                            griditemMyRow["Include"] = true;
                            jobTotal += Decimal.Parse(griditemMyRow["ChargeAmount"].ToString());
                            jobCount++;
                        }
                        else
                        {
                            int jobId = 0;

                            jobId = Convert.ToInt32(griditemMyRow["JobId"]);

                            if (m_selectedJobs.Contains(jobId.ToString()))
                            {
                                griditemMyRow["Include"] = true;
                                jobTotal += Decimal.Parse(griditemMyRow["ChargeAmount"].ToString());
                                jobCount++;
                            }
                        }
                    }

                    if (jobCount != 0)
                    {
                        lblDetails.Text = "You have selected " + jobCount + " job(s), and the total amount is " + jobTotal.ToString("C");
                        hidJobTotal.Value = jobTotal.ToString();
                        hidJobCount.Value = jobCount.ToString();
                    }
                    else
                    {
                        hidJobTotal.Value = hidJobCount.Value = "0";
                        lblDetails.Text = "";
                    }
                }
            }
            else
            {
                btnExport.Visible = false;
                lblOnHold.Visible = true;
                lblOnHold.Text = "With the given parameters no jobs have been found.";
                dgJobs.Visible = false;
                pnlNormalJob.Visible = false;
                lblClient.Text = "Client";
                cboClient.Visible = true;
            }

            if (dgJobs.Items.Count >= 1)
            {
                if (cboClient.Text != "")
                {
                    chkMarkAll.Visible = true;

                    // If these jobs ready for invoice show Create Invoice Button (ONLY FOR READY-FOR-INVOICE)
                    if (jobState == eJobState.ReadyToInvoice)
                        btnCreateInvoice.Visible = btnCreateInvoice2.Visible = true;

                    dgJobs.Levels[0].Columns[1].Visible = true;
                    btnSaveFilter.Visible = true;
                }
                else
                {
                    //TODO: dgJobs.GroupBy[2].ToString();
                    dgJobs.Levels[0].Columns[1].Visible = false;
                }
            }

            lblJobCount.Text = "There are " + dgJobs.Items.Count.ToString() + " jobs ready to invoice.";

            pnlNormalJob.Visible = true;
            btnFilter.Visible = btnFilter2.Visible = true;
            btnClear.Visible = btnClear2.Visible = true;
        }

        protected void SaveInvoicePreparationProgress()
        {
            string userName = ((Entities.CustomPrincipal)Page.User).UserName;

            Facade.IInvoice facProgress = new Facade.Invoice();

            if (ViewState["invoicePreparation"] == null)
                m_InvoicePreparation = new Entities.InvoicePreparation();
            else
                m_InvoicePreparation = (Entities.InvoicePreparation)ViewState["invoicePreparation"];

            // Params
            if (cboClient.SelectedValue != "")
                m_InvoicePreparation.ClientId = Convert.ToInt32(cboClient.SelectedValue);

            if (dteStartDate.SelectedDate == DateTime.MinValue)
                m_InvoicePreparation.StartDate = DateTime.MinValue;
            else
                m_InvoicePreparation.StartDate = dteStartDate.SelectedDate.Value;

            if (dteEndDate.SelectedDate == DateTime.MinValue)
                m_InvoicePreparation.EndDate = DateTime.MinValue;
            else
                m_InvoicePreparation.EndDate = dteEndDate.SelectedDate.Value;
            
            m_selectedJobs = GetJobsSelected();
            m_InvoicePreparation.JobIdCSV = hidSelectedJobs.Value ;
            m_InvoicePreparation.JobState = ((eJobState)cboJobState.SelectedIndex + 1);
            m_InvoicePreparation.UserName = userName;

            bool saved = false;
            if (dteStartDate.SelectedDate == DateTime.MinValue || dteEndDate.SelectedDate == DateTime.MinValue)
                saved = facProgress.SaveInvoicePreparationProgress(m_InvoicePreparation);
            else
                saved = facProgress.SaveInvoicePreparationProgressWithDates(m_InvoicePreparation);

            if (saved)
            {
                lblSaveProgressNotification.Visible = true;
                lblSaveProgressNotification.ForeColor = Color.Blue;
                lblSaveProgressNotification.Text = "You have successfully saved the progress filter.";
                LoadGrid();
            }
            else
            {
                lblSaveProgressNotification.Visible = true;
                lblSaveProgressNotification.ForeColor = Color.Red;
                lblSaveProgressNotification.Text = "The unable to save the progress filter.";
            }
        }

        protected void LoadInvoicePreparationProgress()
        {
            Facade.IInvoice facProgress = new Facade.Invoice();

            string userName = ((Entities.CustomPrincipal)Page.User).UserName;

            ClearFields();

            m_InvoicePreparation = facProgress.LoadInvoicePreparationProgress(userName);

            // Load Params If Entity Is Populated
            if (m_InvoicePreparation != null)
            {
                lblSaveProgressNotification.Visible = true;
                lblSaveProgressNotification.ForeColor = Color.SeaGreen;
                lblSaveProgressNotification.Text = "The progress of your last search has been loaded.  You can turn this off using the 'Clear Filter' button.";

                cboClient.SelectedValue = m_InvoicePreparation.ClientId.ToString();
                m_IdentityId = Convert.ToInt32(m_InvoicePreparation.ClientId.ToString());
                Facade.IOrganisation facOrganisation = new Facade.Organisation();
                cboClient.Text = facOrganisation.GetForIdentityId(m_InvoicePreparation.ClientId).OrganisationName;

                dteStartDate.SelectedDate = m_InvoicePreparation.StartDate;
                dteEndDate.SelectedDate = m_InvoicePreparation.EndDate;

                // Load Array List To Grid After Loaded 
                jobIdCSV = m_InvoicePreparation.JobIdCSV; // ie: '234,234,233'
                
                hidSelectedJobs.Value = "," + jobIdCSV;
 
                if (jobIdCSV != null && jobIdCSV.Length > 0)
                {
                    m_selectedJobs.Clear();

                    string[] jobIds = jobIdCSV.Split(',');

                    foreach (string jobId in jobIds)
                        m_selectedJobs.Add(jobId);

                    LoadGrid();

                    btnSaveFilter.Visible = true;
                }
            }
            else
            {
                lblSaveProgressNotification.Visible = true;
                lblSaveProgressNotification.ForeColor = Color.Red;
                lblSaveProgressNotification.Text = "No filter progress found.";
                btnLoadFilter.Visible = false;
            }
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

        private void chkOnlyShowTicked_CheckedChanged(object sender, EventArgs e)
        {
            m_selectedJobs = GetJobsSelected();

            if (m_selectedJobs[0].ToString() != "")
            {
                btnFilter.DisableServerSideValidation();
                btnFilter2.DisableServerSideValidation();

                Page.Validate();

                if (Page.IsValid)
                    LoadGrid();
            }
            else
            {
                chkOnlyShowTicked.Checked = false; 
                //pnlNormalJob.Visible = false;
            }
        }

        protected void repReferences_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Entities.JobReference reference = (Entities.JobReference)e.Item.DataItem;
            }
        }

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
