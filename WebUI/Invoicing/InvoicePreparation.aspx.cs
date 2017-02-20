using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Globalization;

using System.Drawing;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Orchestrator.Entities;
using Orchestrator.Globals;
using Orchestrator.WebUI.Security;
using P1TP.Components.Web.Validation;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.Invoicing
{
    /// <summary>
    /// Summary description for Invoice Preparation.
    /// </summary>
    public partial class InvoicePreparationNew : Orchestrator.Base.BasePage
    {
        protected override PageStatePersister PageStatePersister
        {
            get
            {
                return new HiddenFieldPageStatePersister(this);
            }
        }

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

        private enum eDataGridColumns { ExtraId, JobId, ExtraType, ExtraState, ClientContact, ExtraAmount, IncludeInInvoice };
        #endregion

        #region Page Variables
        private int m_IdentityId = 0;
        private ArrayList m_selectedJobs = new ArrayList();
        private int m_resetDate = 0;
        private InvoicePreparation m_InvoicePreparation = null;
        private string jobIdCSV = String.Empty;
        private int m_selectedJobCount = 0;
        private decimal m_selectedJobValues = 0;

        private decimal jobTotal = 0.0M;
        private int jobCount = 0;
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

        #region Form Elements
        protected HtmlGenericControl fsExtras;
        protected System.Web.UI.WebControls.CheckBox chkMarkAll;
        protected System.Web.UI.WebControls.RequiredFieldValidator rfvJobState;
        protected System.Web.UI.WebControls.Label lblSelfBillNote;
        #endregion

        #region Page/Load/Init/Error
        protected void Page_Load(object sender, System.EventArgs e)
        {
            Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.Invoicing);

            if (Request.QueryString["rcbID"] != null) return;


            // Reset Dates Marker 
            if (Request.QueryString["ResetDates"] != null)
                m_resetDate = Convert.ToInt32(Request.QueryString["ResetDates"]);

            // Update Invoice
            if (cboClient.SelectedValue != "")
                m_IdentityId = Convert.ToInt32(cboClient.SelectedValue);

            if (m_IdentityId == 0 && Request.QueryString["IdentityId"] != null)
                m_IdentityId = Convert.ToInt32(Request.QueryString["IdentityId"]);

            if (m_IdentityId != 0)
                btnSaveFilter.Visible = true;
            else
                btnSaveFilter.Visible = false;

            if (!IsPostBack)
            {
                //ClearFields();
                Utilities.ClearInvoiceSession();
                //CLEAR INVOICE SESSION VARIABLES

                PopulateStaticControls();
                lblClient.Text = "Client";


                if (m_IdentityId != 0)
                {
                    performSelfBillSearch();
                    // PerformExtraSearch();
                }

            }
        }

        protected void JobsToInvoice_Init(object sender, EventArgs e)
        {
            this.btnExport.Click += new EventHandler(btnExport_Click);
            this.btnFilterExtras.Click += new EventHandler(btnFilterExtras_Click);

            this.btnClear.Click += new System.EventHandler(btnClear_Click);
            this.btnClear2.Click += new System.EventHandler(btnClear_Click);

            this.btnFilter.Click += new System.EventHandler(btnFilter_Click);
            this.btnFilter2.Click += new System.EventHandler(btnFilter_Click);

            this.btnCreateInvoice.Click += new System.EventHandler(btnCreateInvoice_Click);
            this.btnCreateInvoice2.Click += new System.EventHandler(btnCreateInvoice_Click);

            this.btnClearFilter.Click += new System.EventHandler(this.btnClearFilter_Click);
            this.btnLoadFilter.Click += new System.EventHandler(this.btnLoadFilter_Click);
            this.btnSaveFilter.Click += new EventHandler(btnSaveFilter_Click);

            //this.chkSelfMarkAll.CheckedChanged += new EventHandler(chkSelfMarkAll_CheckedChanged);
            this.chkOnlyShowTicked.CheckedChanged += new EventHandler(chkOnlyShowTicked_CheckedChanged);

            this.cboClient.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);

            extrasRadGrid.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(extrasRadGrid_NeedDataSource);
            extrasRadGrid.ItemDataBound += new Telerik.Web.UI.GridItemEventHandler(extrasRadGrid_ItemDataBound);
            extrasRadGrid.UpdateCommand += new GridCommandEventHandler(extrasRadGrid_UpdateCommand);
            extrasRadGrid.ItemCreated += new GridItemEventHandler(extrasRadGrid_ItemCreated);

            dvJobs.DataBinding += new EventHandler(dvJobs_DataBinding);
            dvJobs.RowDataBound += new GridViewRowEventHandler(dvJobs_RowDataBound);
            dvJobs.Sorting += new GridViewSortEventHandler(dvJobs_Sorting);
            dvJobs.RowCreated += new GridViewRowEventHandler(dvJobs_RowCreated);

        }

        private void extrasRadGrid_UpdateCommand(object source, GridCommandEventArgs e)
        {
            GridEditableItem editedItem = e.Item as GridEditableItem;
            int extraId = 0;

            if (!String.IsNullOrEmpty(editedItem["ExtraId"].Text))
                extraId = Convert.ToInt32(editedItem["ExtraId"].Text);

            if (extraId > 0)
            {
                DropDownList cboExtraState = (DropDownList)e.Item.FindControl("cboExtraState");
                HtmlInputControl txtClientContact = (HtmlInputControl)e.Item.FindControl("txtClientContact");
                RadNumericTextBox txtAmount = (RadNumericTextBox)e.Item.FindControl("txtAmount");

                Facade.Job facJob = new Facade.Job();
                Entities.Extra extra = facJob.GetExtraForExtraId(extraId);

                extra.ExtraState = (eExtraState)Enum.Parse(typeof(eExtraState), cboExtraState.SelectedItem.Text);
                extra.ClientContact = txtClientContact.Value;
                extra.ExtraAmount = Convert.ToDecimal(txtAmount.Value);

                facJob.UpdateExtra(extra, ((CustomPrincipal)this.Page.User).UserName);

                extrasRadGrid.Rebind();
            }
        }

        private void extrasRadGrid_ItemCreated(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridEditFormItem && e.Item.IsInEditMode)
            {
                if (!e.Item.OwnerTableView.IsItemInserted)
                {
                    if (e.Item.DataItem != null && e.Item.DataItem.GetType() != typeof(GridInsertionObject) && e.Item.IsInEditMode && e.Item.DataItem.GetType() != typeof(GridEditFormInsertItem))
                    {
                        GridEditableItem item = e.Item as GridEditableItem;
                        GridTemplateColumnEditor editor = (GridTemplateColumnEditor)item.EditManager.GetColumnEditor("ExtraStateCol");
                        DropDownList cboExtraState = (DropDownList)editor.ContainerControl.FindControl("cboExtraState");

                        this.PopulateExtraStates(cboExtraState, null);

                        DataRowView drv = (DataRowView)e.Item.DataItem;
                        cboExtraState.Items.FindByText(drv["ExtraState"].ToString()).Selected = true;
                    }
                }
            }
        }

        private void extrasRadGrid_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                DataRowView drv = (DataRowView)e.Item.DataItem;
                Label extraAmount = (Label)e.Item.FindControl("lblExtraAmount");

                extraAmount.Text = ((decimal)drv["ExtraAmount"]).ToString("C");
            }
        }

        public void cboExtraState_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            if (args.Value == 0.ToString())
            {
                args.IsValid = false;
            }
            else
            {
                args.IsValid = true;
            }
        }

        public void cboExtraType_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            if (args.Value == 0.ToString())
            {
                args.IsValid = false;
            }
            else
            {
                args.IsValid = true;
            }
        }

        protected void PopulateExtraStates(object sender, EventArgs e)
        {
            DropDownList cboExtraState = sender as DropDownList;

            if (cboExtraState != null && cboExtraState.Items.Count == 0)
            {
                string[] extraStates = Enum.GetNames(typeof(eExtraState));

                for (int index = 0; index < extraStates.Length; index++)
                    cboExtraState.Items.Add(new ListItem(extraStates[index], index.ToString()));

                cboExtraState.Items.Remove(cboExtraState.Items.FindByText(eExtraState.Invoiced.ToString()));
            }
        }

        protected void PopulateExtraTypes(object sender, EventArgs e)
        {
            DropDownList cboExtraState = sender as DropDownList;

            // Static controls relating to invoice extras:
            Facade.ExtraType facExtraType = new Orchestrator.Facade.ExtraType();
            bool? getActiveExtraTypes = true;
            cboExtraType.DataSource = facExtraType.GetForIsEnabled(getActiveExtraTypes);
            cboExtraType.DataValueField = "ExtraTypeId";
            cboExtraType.DataTextField = "Description";
            cboExtraType.DataBind();
        }

        void extrasRadGrid_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            this.PerformExtraSearch();
        }

        void dvJobs_RowCreated(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
                AddGlyph(dvJobs, e.Row);
        }

        //protected string sortExpression = string.Empty;

        void dvJobs_Sorting(object sender, GridViewSortEventArgs e)
        {
            SortExpression = e.SortExpression;
            if (e.SortDirection.ToString() == "Ascending")
            {
                if (SortDirection == e.SortDirection.ToString())
                    SortDirection = "Descending";
                else
                    SortDirection = e.SortDirection.ToString();
            }
            else
            {
                SortDirection = e.SortDirection.ToString();
            }

            jobIdCSV = hidSelectedJobs.Value;
            performSelfBillSearch();
        }

        void dvJobs_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                // These attributes are required by the "remember where I am" yellow highlight functionality.
                e.Row.Attributes.Add("onClick", "javascript:HighlightRow('" + e.Row.ClientID + "');");
                e.Row.Attributes.Add("id", e.Row.ClientID);

                string param = string.Empty;

                e.Row.Attributes.Add("onmousedown", "showMenu()");
                DataRowView drv = ((DataRowView)e.Row.DataItem);

                string jobId = drv["JobId"].ToString();
                string amount = drv["ChargeAmount"].ToString();

                ((CheckBox)e.Row.FindControl("chkSelect")).Attributes.Add("onclick", "selectItem(" + jobId + "," + amount + ",this);");

                if (jobIdCSV.IndexOf(drv["JobId"].ToString()) > -1)
                {
                    ((CheckBox)e.Row.FindControl("chkSelect")).Checked = true;
                    jobCount++;
                    jobTotal += decimal.Parse(amount);
                }
            }
        }

        void dvJobs_DataBinding(object sender, EventArgs e)
        {
            //Literal l = (Literal)sender;
            //DataGridItem container = (DataGridItem)l.NamingContainer;


        }



        void btnExport_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                DataTable exportCSV = (DataTable)Session[C_EXPORTCSV_VS];
                if (!exportCSV.Columns.Contains("Ticked"))
                    exportCSV.Columns.Add("Ticked", typeof(bool));
                foreach (GridViewRow row in dvJobs.Rows)
                    if (row.RowType == DataControlRowType.DataRow)
                    {
                        CheckBox chkSelect = (CheckBox)row.FindControl("chkSelect");
                        HtmlInputHidden hidJobId = (HtmlInputHidden)row.FindControl("hidJobId");

                        DataRow[] jobRows = exportCSV.Select("JobID = " + hidJobId.Value);
                        foreach (DataRow jobRow in jobRows)
                            jobRow["Ticked"] = chkSelect.Checked;
                    }

                //  Get data set and then put in session
                DataView dvExport = new DataView();
                dvExport = new DataView(exportCSV);

                Session["__ExportDS"] = (DataTable)Session[C_EXPORTCSV_VS];

                Server.Transfer("../Reports/csvexport.aspx?filename=InvoicePreparation.csv");
            }
        }

        #endregion

        #region Object DataSource Methods
        public DataSet GetJobsToInvoice()
        {
            m_selectedJobValues = 0;
            m_selectedJobCount = 0;

            DateTime startDate = DateTime.MinValue;
            DateTime endDate = DateTime.MinValue;

            StringBuilder jobStates = new StringBuilder();

            foreach (string name in Enum.GetNames(typeof(eJobState)))
            {
                if (name != eJobState.Booked.ToString() && name != eJobState.Planned.ToString() && name != eJobState.InProgress.ToString()
                    && name != eJobState.Cancelled.ToString())
                {
                    if (jobStates.Length > 0)
                        jobStates.Append(",");

                    jobStates.Append(((int)(eJobState)Enum.Parse(typeof(eJobState), name)).ToString());
                }
            }

            string jobStateCSV = jobStates.ToString();

            //cboJobState.Visible = m_IdentityId > 0;
            //lblJobState.Visible = m_IdentityId > 0;
            //pnlExtraFilter.Visible = m_IdentityId > 0;


            // Job State
            if (cboJobState.SelectedValue != string.Empty)
                jobStateCSV = ((int)(eJobState)Enum.Parse(typeof(eJobState), eJobState.ReadyToInvoice.ToString())).ToString();

            //    jobStateCSV = eJobState.ReadyToInvoice.ToString();//Enum.Parse(typeof(eJobState), cboJobState.SelectedValue.Replace(" ", ""))).ToString();

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
                dsInvoicing = facInvoice.GetJobsToInvoiceWithParamsAndDate(m_IdentityId, eJobState.ReadyToInvoice, posted, startDate, endDate);
            else
            {
                if (m_IdentityId == 0)
                    dsInvoicing = facInvoice.GetAllJobsToInvoice();
                else
                    dsInvoicing = facInvoice.GetJobsToInvoiceWithParams(m_IdentityId, eJobState.ReadyToInvoice, posted);
            }
            //if (startDate != DateTime.MinValue && endDate != DateTime.MinValue)
            //    dsInvoicing = facInvoice.GetSelfBillJobswithParamsAndDate(m_IdentityId, jobStateCSV, posted, startDate, endDate);
            //else
            //    dsInvoicing = facInvoice.GetSelfBillJobswithParams(m_IdentityId, jobStateCSV, posted);

            // Check whether account is on hold
            if (dsInvoicing.Tables[0].Rows.Count > 0)
            {
                if (Convert.ToInt32(dsInvoicing.Tables[0].Rows[0]["OnHold"]) == 1)
                {
                    dvJobs.Enabled = false;
                    lblSelfOnHold.Visible = true;
                    lblSelfOnHold.Text = cboClient.Text + "'s account has been put on hold, please go to <A HREF=../Organisation/addupdateorganisation.aspx?IdentityId=" + Convert.ToInt32(cboClient.SelectedValue) + ">" + cboClient.Text + "'s details to change.</A>";
                }
                else
                    lblSelfOnHold.Visible = false;

                //dgSelfBillJobs.Visible = true;
            }
            else
            {
                lblSelfOnHold.Visible = true;
                lblSelfOnHold.Text = "With the given parameters no jobs have been found.";
                dvJobs.Visible = false;
                pnlExtras.Visible = false;
                chkOnlyShowTicked.Visible = false;
            }

            // Put in dummy checkbox column
            dsInvoicing.Tables[0].Columns.Add("Include", typeof(Boolean));

            // Bind jobs to datagrid 
            DataView invoicableJobs = new DataView(dsInvoicing.Tables[0]);
            // invoicableJobs.Sort = (SortExpression + " " + SortDirection).Trim();

            if (chkOnlyShowTicked.Checked)
                invoicableJobs.RowFilter = "jobId IN (" + hidSelectedJobs.Value + ")";



            //if (cboClient.SelectedValue == String.Empty)
            //    dgSelfBillJobs.GroupBy = "OrganisationName";
            //else
            //{
            //    dgSelfBillJobs.Levels[0].Columns["OrganisationName"].Visible = false;
            //    dgSelfBillJobs.Levels[0].AllowGrouping = false;
            //}

            Session[C_EXPORTCSV_VS] = dsInvoicing.Tables[0];
            if (SortExpression != "")
            {
                // invoicableJobs.Sort = SortExpression;
                invoicableJobs.Sort = SortExpression + " " + (SortDirection == "Ascending" ? "ASC" : "DESC");
            }
            else
            {
                invoicableJobs.Sort = "DocketNumbers Asc";
            }

            return dsInvoicing;

        }
        #endregion

        private void AddGlyph(GridView grid, GridViewRow item)
        {
            Label glyph = new Label();
            glyph.EnableTheming = false;
            glyph.Font.Name = "webdings";
            glyph.Font.Size = FontUnit.XSmall;
            glyph.Text = (grid.SortDirection == System.Web.UI.WebControls.SortDirection.Ascending ? " 5" : " 6");

            // Find the column sorted by
            for (int i = 0; i < grid.Columns.Count; i++)
            {
                string colExpr = grid.Columns[i].SortExpression;
                if (colExpr != "" && colExpr == grid.SortExpression)
                {
                    item.Cells[i].Controls.Add(glyph);
                }
            }
        }

        #region Populate Static Controls
        private void PopulateStaticControls()
        {
            cboJobState.DataSource = Utilities.UnCamelCase(Enum.GetNames(typeof(eJobState)));
            cboJobState.DataBind();
            //cboJobState.Items.Insert(0, "");

            lblJobState.Visible = false;
            cboJobState.Visible = false;

            if (m_IdentityId != 0)
            {
                //Get the Client name for id.
                Facade.IOrganisation facOrg = new Facade.Organisation();
                string name = facOrg.GetNameForIdentityId(m_IdentityId);

                cboClient.SelectedValue = m_IdentityId.ToString();
                cboClient.Text = name;
            }

            // Static controls relating to invoice extras:
            Facade.ExtraType facExtraType = new Orchestrator.Facade.ExtraType();
            bool? getActiveExtraTypes = true;
            cboExtraType.DataSource = facExtraType.GetForIsEnabled(getActiveExtraTypes); //Enum.GetNames(typeof(eExtraType));
            cboExtraType.DataValueField = "ExtraTypeId";
            cboExtraType.DataTextField = "Description";
            cboExtraType.DataBind();
            cboExtraType.Items.Insert(0, "");

            cboSelectExtraState.DataSource = Enum.GetNames(typeof(eExtraState));
            cboSelectExtraState.DataBind();
            cboSelectExtraState.Items.Insert(0, "");
            cboSelectExtraState.Items.RemoveAt((int)eExtraState.Invoiced - 1);
            cboSelectExtraState.Items.FindByValue(eExtraState.Accepted.ToString()).Selected = true;
        }
        #endregion

        #region Combo's Server Methods and Initialisation
        void cboClient_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            cboClient.Items.Clear();

            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllNormalClientsFiltered(e.Text); // Change to Normal Clients

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

            btnFilter.Visible = btnFilter2.Visible = true;
            btnCreateInvoice.Visible = btnCreateInvoice2.Visible = false;
            btnClear.Visible = btnClear2.Visible = false;

            // Reset the client fields
            cboClient.Text = string.Empty;
            cboClient.SelectedValue = string.Empty;

            // Date Fields
            dteStartDate.SelectedDate = DateTime.MinValue;
            dteEndDate.SelectedDate = DateTime.MinValue;

            // Filter Options 
            btnSaveFilter.Visible = false;
            lblSaveProgressNotification.Visible = false;
            btnClearFilter.Visible = false;

            // Clear Check Boxes
            //chkSelfMarkAll.Checked = false;
            chkOnlyShowTicked.Checked = false;

            // Clear hidden variables
            hidSelectedJobs.Value = String.Empty;
            hidJobCount.Value = String.Empty;
            hidJobTotal.Value = String.Empty;
            hidSelectedExtras.Value = string.Empty;
        }

        private ArrayList GetExtrasSelected()
        {
            ArrayList extraIds = new ArrayList();

            //loop through grid and add all the ExtraIds of Extras that are checked to the Extras list.
            foreach (GridItem selectedExtras in this.extrasRadGrid.SelectedItems)
            {
                string extraId = selectedExtras.OwnerTableView.DataKeyValues[selectedExtras.ItemIndex]["ExtraId"].ToString();
                extraIds.Add(extraId);
            }

            return extraIds;
        }

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


        protected void cboSelectExtraState_SelectedIndexChanged(object sender, EventArgs e)
        {
            eExtraState state = (eExtraState)Enum.Parse(typeof(eExtraState), ((DropDownList)sender).SelectedValue);

            DataGridItem containingItem = ((DataGridItem)((DropDownList)sender).Parent.Parent);

            RequiredFieldValidator rfvClientContact = (RequiredFieldValidator)containingItem.FindControl("rfvClientContact");

            rfvClientContact.Enabled = (state == eExtraState.Accepted || state == eExtraState.Refused);
        }

        private void chkOnlyShowTicked_CheckedChanged(object sender, EventArgs e)
        {
            m_selectedJobs = GetJobsSelected();

            if (m_selectedJobs[0].ToString() != "")
            {
                btnFilter.DisableServerSideValidation();
                btnFilter2.DisableServerSideValidation();

                Page.Validate();

                if (Page.IsValid)
                    performSelfBillSearch();
            }
            else
            {
                chkOnlyShowTicked.Checked = false;
                //pnlNormalJob.Visible = false;
            }
        }
        #endregion

        #region Button Events
        protected void btnClear_Click(object sender, EventArgs e)
        {
            ClearFields();
        }

        protected void btnFilter_Click(object sender, EventArgs e)
        {
            m_selectedJobs = GetJobsSelected();

            btnFilter.DisableServerSideValidation();
            btnFilter2.DisableServerSideValidation();

            if (Page.IsValid)
            {
                lblSaveProgressNotification.Visible = false;
                btnSaveFilter.Visible = true;
                hidSelectedJobs.Value = String.Empty;
                hidJobCount.Value = "0";
                hidJobTotal.Value = "0";
                performSelfBillSearch();
                PerformExtraSearch();
                extrasRadGrid.DataBind();
            }
        }

        protected void btnCreateInvoice_Click(object sender, EventArgs e)
        {
            btnCreateInvoice.DisableServerSideValidation();
            btnCreateInvoice2.DisableServerSideValidation();

            ArrayList arrJobId = null;
            ArrayList arrExtraId = null;

            arrJobId = GetJobsSelected();
            arrExtraId = GetExtrasSelected();

            if (arrJobId.Count != 0)
            {
                pnlMessage.Visible = false;

                //#15867 J.Steele 
                //Clear the Invoice Session variables before setting the specific ones
                Utilities.ClearInvoiceSession();

                Session["StartDate"] = dteStartDate.SelectedDate.Value.ToString();
                Session["EndDate"] = dteEndDate.SelectedDate.Value.ToString();
                Session["JobIds"] = arrJobId;

                Session["ClientId"] = Convert.ToInt32(cboClient.SelectedValue);
                Session["ClientName"] = cboClient.Text;
                Session["Type"] = eInvoiceFilterType.SelfBillInvoice;

                if (arrExtraId.Count != 0)
                    Session["ExtraIds"] = arrExtraId;
                else
                    Session["ExtraIds"] = null;

                Server.Transfer("addupdateinvoice.aspx");
            }
            else
            {
                pnlMessage.Visible = true;
                lblMessage.Text = "You must select at least 1 job to invoice.";
            }
        }

        private void btnFilterExtras_Click(object sender, EventArgs e)
        {
            PerformExtraSearch();
        }

        private void btnClearFilter_Click(object sender, System.EventArgs e)
        {
            performSelfBillSearch();
            lblSaveProgressNotification.Visible = true;
            lblSaveProgressNotification.ForeColor = Color.Blue;
            lblSaveProgressNotification.Text = "The progress of your last search has been unloaded.  You can turn this on using the 'Load Filter' button.";
            btnClearFilter.Visible = false;
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
        #endregion

        #region Self Bill Jobs To Invoice
        private void performSelfBillSearch()
        {
            m_selectedJobValues = 0;
            m_selectedJobCount = 0;

            DateTime startDate = DateTime.MinValue;
            DateTime endDate = DateTime.MinValue;

            StringBuilder jobStates = new StringBuilder();

            foreach (string name in Enum.GetNames(typeof(eJobState)))
            {
                if (name != eJobState.Booked.ToString() && name != eJobState.Planned.ToString() && name != eJobState.InProgress.ToString()
                    && name != eJobState.Cancelled.ToString())
                {
                    if (jobStates.Length > 0)
                        jobStates.Append(",");

                    jobStates.Append(((int)(eJobState)Enum.Parse(typeof(eJobState), name)).ToString());
                }
            }

            string jobStateCSV = jobStates.ToString();

            //cboJobState.Visible = m_IdentityId > 0;
            //lblJobState.Visible = m_IdentityId > 0;
            pnlExtraFilter.Visible = m_IdentityId > 0;


            // Job State
            if (cboJobState.SelectedValue != string.Empty)
                jobStateCSV = ((int)(eJobState)Enum.Parse(typeof(eJobState), eJobState.ReadyToInvoice.ToString())).ToString();

            //    jobStateCSV = eJobState.ReadyToInvoice.ToString();//Enum.Parse(typeof(eJobState), cboJobState.SelectedValue.Replace(" ", ""))).ToString();

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
                dsInvoicing = facInvoice.GetJobsToInvoiceWithParamsAndDate(m_IdentityId, eJobState.ReadyToInvoice, posted, startDate, endDate);
            else
            {
                if (m_IdentityId == 0)
                    dsInvoicing = facInvoice.GetAllJobsToInvoice();
                else
                    dsInvoicing = facInvoice.GetJobsToInvoiceWithParams(m_IdentityId, eJobState.ReadyToInvoice, posted);
            }
            //if (startDate != DateTime.MinValue && endDate != DateTime.MinValue)
            //    dsInvoicing = facInvoice.GetSelfBillJobswithParamsAndDate(m_IdentityId, jobStateCSV, posted, startDate, endDate);
            //else
            //    dsInvoicing = facInvoice.GetSelfBillJobswithParams(m_IdentityId, jobStateCSV, posted);

            // Check whether account is on hold
            if (dsInvoicing.Tables[0].Rows.Count > 0)
            {
                if (Convert.ToInt32(dsInvoicing.Tables[0].Rows[0]["OnHold"]) == 1)
                {
                    dvJobs.Enabled = false;
                    lblSelfOnHold.Visible = true;
                    lblSelfOnHold.Text = cboClient.Text + "'s account has been put on hold, please go to <A HREF=../Organisation/addupdateorganisation.aspx?IdentityId=" + Convert.ToInt32(cboClient.SelectedValue) + ">" + cboClient.Text + "'s details to change.</A>";
                }
                else
                    lblSelfOnHold.Visible = false;
            }
            else
            {
                lblSelfOnHold.Visible = true;
                lblSelfOnHold.Text = "With the given parameters no jobs have been found.";
                pnlExtras.Visible = false;
                chkOnlyShowTicked.Visible = false;
            }

            // Bind jobs to datagrid 
            DataView invoicableJobs = new DataView(dsInvoicing.Tables[0]);
            Session[C_EXPORTCSV_VS] = dsInvoicing.Tables[0];

            if (chkOnlyShowTicked.Checked)
                invoicableJobs.RowFilter = "jobId IN (" + hidSelectedJobs.Value + ")";

            if (SortExpression != "")
            {
                // invoicableJobs.Sort = SortExpression;
                invoicableJobs.Sort = SortExpression + " " + (SortDirection == "Ascending" ? "ASC" : "DESC");
            }
            else
            {
                invoicableJobs.Sort = "DocketNumbers Asc";
            }
            dvJobs.DataSource = invoicableJobs;
            dvJobs.DataBind();

            if (invoicableJobs.Table.Rows.Count > 0)
            {
                btnExport.Visible = true;
                //chkSelfMarkAll.Visible = true;
                btnSaveFilter.Visible = true;
                btnCreateInvoice.Visible = btnCreateInvoice2.Visible = true;

                if (cboClient.SelectedValue != String.Empty)
                {
                    dvJobs.Columns[0].Visible = true;
                    dvJobs.Columns[1].Visible = false;
                }
                else
                {
                    dvJobs.Columns[0].Visible = false;
                    dvJobs.Columns[1].Visible = true;
                }

                for (int i = 0; i <= dvJobs.Rows.Count - 1; i++)
                {
                    GridViewRow griditemMyRow = dvJobs.Rows[i];
                    CheckBox chkInclude = (CheckBox)griditemMyRow.FindControl("chkSelect");
                    HtmlInputHidden hidJobId = (HtmlInputHidden)griditemMyRow.FindControl("hidJobId");

                    if (chkOnlyShowTicked.Checked)
                    {
                        chkInclude.Checked = true;
                        jobTotal += Decimal.Parse(griditemMyRow.Cells[5].Text.Replace("£", ""));
                        jobCount++;
                    }
                    else
                    {
                        int jobId = 0;

                        jobId = Convert.ToInt32(hidJobId.Value);

                        if (m_selectedJobs.Contains(jobId.ToString()))
                        {
                            chkInclude.Checked = true;
                            //jobTotal += Decimal.Parse(griditemMyRow.Cells[5].Text.Replace("£", ""));
                            //jobCount++;
                        }
                    }
                }

                chkOnlyShowTicked.Visible = true;
            }
            else
            {
                btnExport.Visible = false;
                btnSaveFilter.Visible = true;
                chkOnlyShowTicked.Visible = false;
            }

            lblDetails.Text = "You have selected " + jobCount + " job(s), and the total amount is " + jobTotal.ToString("C");
            hidJobTotal.Value = jobTotal.ToString();
            hidJobCount.Value = jobCount.ToString();

            btnFilter.Visible = btnFilter2.Visible = true;

            btnClear.Visible = btnClear2.Visible = true;
            pnlExtras.Visible = cboClient.SelectedValue != "";

            lblJobCount.Text = "There are " + dvJobs.Rows.Count.ToString() + " jobs ready to invoice.";
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

        #region Extras DataGrid Event Handlers (OnCancelCommand/OnEditCommand/OnUpdateCommand)

        #endregion

        #region Perform Extra Search

        private void PerformExtraSearch()
        {
            if (cboClient.SelectedValue != "")
            {
                // Test for populating the combo box
                DataTable tblExtras = new DataTable("Extras");
                DataTable tblExtraStates = new DataTable("ExtraStates");

                string extraStateCSV = Convert.ToString((int)eExtraState.Accepted) + "," +
                    Convert.ToString((int)eExtraState.AwaitingResponse) + "," +
                    Convert.ToString((int)eExtraState.Refused);

                // Will always be retrieving extras with parameters, since filtering by client:
                Facade.IInvoiceExtra facInvoiceExtra = new Facade.Invoice();

                DataSet dsExtras = facInvoiceExtra.GetExtrasWithParams(
                    Convert.ToInt32(cboClient.SelectedValue),
                    txtExtraJobId.Text == "" ? 0 : int.Parse(txtExtraJobId.Text),
                    cboExtraType.SelectedIndex != 0 ? cboExtraType.SelectedIndex : 0,
                    Convert.ToString(cboSelectExtraState.SelectedIndex) != "-1" ? Convert.ToString(((int)(eExtraState)Enum.Parse(typeof(eExtraState), cboSelectExtraState.SelectedValue))) : extraStateCSV,
                    dteExtraDateFrom.Text == "" ? Convert.ToDateTime(SqlDateTime.MinValue.ToString()) : dteExtraDateFrom.SelectedDate.Value,
                    dteExtraDateFrom.Text == "" ? Convert.ToDateTime(SqlDateTime.MinValue.ToString()).Add(new TimeSpan(23, 59, 00)) : dteExtraDateTo.SelectedDate.Value,
                    false);

                // Get a table containing the extra states. 
                tblExtraStates = new DataTable("ExtraStates");
                tblExtraStates.Columns.Add(new DataColumn("ExtraStateId", typeof(int)));
                tblExtraStates.Columns.Add(new DataColumn("ExtraState", typeof(string)));

                string[] extraStates = Utilities.UnCamelCase(Enum.GetNames(typeof(eExtraState)));
                foreach (string item in extraStates)
                {
                    if (item.ToString() != "Invoiced")
                    {
                        DataRow state = tblExtraStates.NewRow();
                        state[0] = (int)Enum.Parse(typeof(eExtraState), item.Replace(" ", ""));
                        state[1] = item.ToString();
                        tblExtraStates.Rows.Add(state);
                    }
                }

                // Add the extra states table to the dataset and set the name of the first table to make the relationship easier to understand.
                dsExtras.Tables[0].TableName = "Extras";
                dsExtras.Tables.Add(tblExtraStates);

                // Configure the relationship.
                dsExtras.Relations.Add(dsExtras.Tables["ExtraStates"].Columns["ExtraStateId"], dsExtras.Tables["Extras"].Columns["ExtraStateId"]);

                extrasRadGrid.DataSource = dsExtras;
                
            }
        }
        #endregion

        #region Main Datagrid Event Handlers
        private void dgSelfBillJobs_ItemContentCreated(object sender, ComponentArt.Web.UI.GridItemContentCreatedEventArgs e)
        {
            int jobId = 0;

            jobId = Convert.ToInt32(e.Item["JobId"]);

            //  CheckBox chkIncludeJob = (CheckBox)e.Content.FindControl("chkIncludeJob");
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



        protected void repReferences_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Entities.JobReference reference = (Entities.JobReference)e.Item.DataItem;
            }
        }
        #endregion

        #region Filter Functions
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
            m_InvoicePreparation.JobIdCSV = hidSelectedJobs.Value;
            jobIdCSV = hidSelectedJobs.Value;
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
                performSelfBillSearch();
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
            btnLoadFilter.DisableServerSideValidation();

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

                cboJobState.SelectedIndex = (int)m_InvoicePreparation.JobState - 1;

                dteStartDate.SelectedDate = m_InvoicePreparation.StartDate;
                dteEndDate.SelectedDate = m_InvoicePreparation.EndDate;

                // Load Array List To Grid After Loaded 
                jobIdCSV = m_InvoicePreparation.JobIdCSV; // ie: '234,234,233'

                hidSelectedJobs.Value = jobIdCSV + ",";

                if (jobIdCSV != null && jobIdCSV.Length > 0)
                {
                    m_selectedJobs.Clear();

                    //jobIdCSV = jobIdCSV.Substring(0, jobIdCSV.Length - 1);

                    string[] jobIds = jobIdCSV.Split(',');

                    foreach (string jobId in jobIds)
                        m_selectedJobs.Add(jobId);

                    performSelfBillSearch();

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
    }
}
