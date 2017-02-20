using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Data;
using System.Globalization;
using System.Text;

using Telerik.Web.UI;
using System.Threading;

namespace Orchestrator.WebUI.Invoicing.groupage
{
    public partial class InvoiceBatchAndApproval : Orchestrator.Base.BasePage
    {
        private class BatchLabel
        {
            private string _originalBatchLabel;
            private string _newBatchLabel;
            private int _preInvoiceId;

            public BatchLabel(string originalBatchLabel, string newBatchLabel, int preInvoiceId)
            {
                _originalBatchLabel = originalBatchLabel;
                _newBatchLabel = newBatchLabel;
                _preInvoiceId = preInvoiceId;
            }

            public string OriginalBatchLabel
            {
                get { return _originalBatchLabel; }
                set { _originalBatchLabel = value; }
            }

            public string NewBatchLabel
            {
                get { return _newBatchLabel; }
                set { _newBatchLabel = value; }
            }

            public int PreInvoiceId
            {
                get { return _preInvoiceId; }
                set { _preInvoiceId = value; }
            }

            public bool ToUpdate
            {
                get { return !String.IsNullOrEmpty(_newBatchLabel) && _originalBatchLabel == _newBatchLabel; }
            }

            
        }

        #region Properties

        private const string COOKIE_LAST_INVOICE_DATE = "MultiBatch_InvoiceDate";

        private string userName;

        public bool IsFromDash
        {
            get { return ViewState["vw_isFromDash"] == null ? false : bool.Parse(ViewState["vw_isFromDash"].ToString()); }
            set { ViewState["vw_isFromDash"] = value; }
        }

        private bool IsUpdate()
        {
            bool retval = !string.IsNullOrEmpty(Request.QueryString["bID"]);
            return retval;
        }

        public int m_IdentityId
        {
            get { return ViewState["vw_IdentityId"] == null ? -1 : int.Parse(ViewState["vw_IdentityId"].ToString()); }
            set { ViewState["vw_IdentityId"] = value; }
        }

        public string RefreshPreInvoices { get; set; }

        public DataSet DsOrders
        {
            get 
                {
                    DataSet dsOrder = null;
                    if (ViewState["vw_DsOrders"] != null)
                        dsOrder = (DataSet)ViewState["vw_DsOrders"];

                    return dsOrder; 
                }
            set 
                { 
                    ViewState["vw_DsOrders"] = value; 
                }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["rcbID"]))
                return;

            Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.Invoicing);

            if (!IsPostBack)
                ConfigureDisplay();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            
            this.grdOrders.ItemDataBound += new GridItemEventHandler(grdOrders_ItemDataBound);
            this.grdOrders.NeedDataSource += new GridNeedDataSourceEventHandler(grdOrders_NeedDataSource);
            this.btnRefresh.Click += new EventHandler(btnRefresh_Click);
            this.btnCreateBatch.Click += new EventHandler(btnCreateBatch_Click);
            this.Button1.Click += new EventHandler(btnCreateBatch_Click);
            this.btnAutoBatch.Click += new EventHandler(btnAutoBatch_Click);
            this.cboClient.ItemsRequested += new RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);
            this.cboClient.SelectedIndexChanged += new RadComboBoxSelectedIndexChangedEventHandler(cboClient_SelectedIndexChanged);

            this.approvalProcess.ProcessActions += new EventHandler(approvalProcess_ProcessActions);
            this.approvalProcess.SelectedPreInvoiceChanged += new SelectedPreInvoiceChangedEventHandler(approvalProcess_SelectedPreInvoiceChanged);
            this.btnAlterBatch.Click += new EventHandler(btnAlterBatch_Click);
            this.btnUnflagOrder.Click += new EventHandler(btnUnflagOrder_Click);

        }

        void btnUnflagOrder_Click(object sender, EventArgs e)
        {
            Orchestrator.Facade.IOrder facOrder = new Orchestrator.Facade.Order();

            foreach (GridItem row in grdOrders.Items)
            {
                if (row.OwnerTableView.Name == grdOrders.MasterTableView.Name)
                {
                    HtmlInputCheckBox chkOrder = (HtmlInputCheckBox)row.FindControl("chkOrder");
                    int isFlaggedForInvoicing = (int)row.OwnerTableView.DataKeyValues[row.ItemIndex]["IsFlaggedForInvoicing"];

                    if (isFlaggedForInvoicing == 1 && chkOrder.Checked)
                    {
                        int orderID = (int)row.OwnerTableView.DataKeyValues[row.ItemIndex]["OrderID"];
                        facOrder.UnflagForInvoicing(orderID);
                    }

                }
            }

            RefreshData();

        }

        void btnAlterBatch_Click(object sender, EventArgs e)
        {
            AlterBatch();
        }

        void cboClient_SelectedIndexChanged(object obj, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            int identityId = 0;
            int.TryParse(e.Value, out identityId);

            if (identityId > 0)
            {
                var invoiceTypeId = (from o in EF.DataContext.Current.OrganisationDefaultsSet
                                     where o.IdentityId == identityId
                                     select o.InvoiceTypeId).FirstOrDefault();

                switch (invoiceTypeId)
                {
                    case 1: // Normal
                        this.rdoListInvoiceType.Items[0].Selected = true;
                        this.rdoListInvoiceType.Items[1].Selected = false;
                        break;
                    case 2: // Self Bill
                        this.rdoListInvoiceType.Items[0].Selected = false;
                        this.rdoListInvoiceType.Items[1].Selected = true;
                        break;
                    default:
                        break;
                }
            }
        }

        int ordercount = 3;
        decimal orderValue = 3000;
        DataTable CustomerBatchCount = new DataTable();
        Dictionary<string, string> CustomerOption = new Dictionary<string, string>();
        Dictionary<string, decimal> CustomerSetting = new Dictionary<string, decimal>();
        Facade.IOrganisation facOrg = new Facade.Organisation();

        private string GetBatchName(string customer,int identityID, decimal rate)
        {
            string batchID = string.Empty;
            // Add the customer setting and determine how they are to be grouped.
            if (!CustomerOption.ContainsKey(customer))
            {
                Entities.Organisation organisation = facOrg.GetForIdentityId(identityID);
                if (organisation.Defaults[0].InvoiceGroupingValue > 0)
                {
                    CustomerOption.Add(customer, organisation.Defaults[0].InvoiceGroupingType);
                    CustomerSetting.Add(customer, Convert.ToDecimal(organisation.Defaults[0].InvoiceGroupingValue));
                }
                else
                    return string.Empty;
            }

            if (CustomerOption[customer] == "ByCount")
                batchID = GetBatchNameForOrderCount(customer);
            else if (CustomerOption[customer] == "ByValue")
                batchID = GetBatchNameForOrderValue(customer, rate);

            // allows for the customer to not have a batching option and therefore do not auto batch

            return batchID;
        }

        private string GetBatchNameForOrderCount(string customer)
        {
            
            if (CustomerBatchCount.Columns.Count == 0)
            {
                // for each and every order here we need to batch these by the customer settings.
                CustomerBatchCount.Columns.Add(new DataColumn("Customer", typeof(string)));
                CustomerBatchCount.Columns.Add(new DataColumn("BatchName", typeof(string)));
                CustomerBatchCount.Columns.Add(new DataColumn("Total", typeof(int)));
                CustomerBatchCount.Columns.Add(new DataColumn("Active", typeof(bool)));
            }
            
            DataRow[] customers = CustomerBatchCount.Select("customer = '" + customer + "' AND Active = 1");
            
            string batchID = string.Empty;
            foreach (DataRow row in customers)
            {
                if ((int)row["Total"] <= CustomerSetting[customer])
                {
                    if ((int)row["Total"] == CustomerSetting[customer])
                    {
                        row["Active"] = false;
                        CustomerBatchCount.AcceptChanges();
                    }
                    else
                    {
                        row["Total"] = ((int)row["Total"]) + 1;
                        CustomerBatchCount.AcceptChanges();
                        batchID = row["BatchName"].ToString();
                    }
                }
            }

            DataRow[] fullBatches = CustomerBatchCount.Select("customer = '" + customer + "' and Active = 0");
            int fullBatchCount = fullBatches.Count();
            if (string.IsNullOrEmpty(batchID))
            {
                fullBatchCount = fullBatchCount == 0 ? 1 : fullBatchCount + 1;
                batchID = customer + " - " + fullBatchCount.ToString();
                CustomerBatchCount.Rows.Add(customer,batchID , 1, true);
                CustomerBatchCount.AcceptChanges();

            }

            return batchID;
        }

        private string GetBatchNameForOrderValue(string customer,decimal rate)
        {

            if (CustomerBatchCount.Columns.Count == 0)
            {
                // for each and every order here we need to batch these by the customer settings.
                CustomerBatchCount.Columns.Add(new DataColumn("Customer", typeof(string)));
                CustomerBatchCount.Columns.Add(new DataColumn("BatchName", typeof(string)));
                CustomerBatchCount.Columns.Add(new DataColumn("Total", typeof(decimal)));
                CustomerBatchCount.Columns.Add(new DataColumn("Active", typeof(bool)));


            }

            DataRow[] customers = CustomerBatchCount.Select("customer = '" + customer + "' AND Active = 1");

            string batchID = string.Empty;
            foreach (DataRow row in customers)
            {
                if ((decimal)row["Total"] <= CustomerSetting[customer])
                {
                    row["Total"] = ((decimal)row["Total"]) + rate;
                    CustomerBatchCount.AcceptChanges();

                    if ((decimal)row["Total"] >= CustomerSetting[customer])
                    {
                        row["Active"] = false;
                        CustomerBatchCount.AcceptChanges();
                    }

                    batchID = row["BatchName"].ToString();
                }

            }

            DataRow[] fullBatches = CustomerBatchCount.Select("customer = '" + customer + "' and Active = 0");
            int fullBatchCount = fullBatches.Count();
            if (string.IsNullOrEmpty(batchID))
            {
                fullBatchCount = fullBatchCount == 0 ? 1 : fullBatchCount + 1;
                batchID = customer + " - " + fullBatchCount.ToString();
                CustomerBatchCount.Rows.Add(customer, batchID, rate, true);
                CustomerBatchCount.AcceptChanges();

            }

            return batchID;
        }

        void btnAutoBatch_Click(object sender, EventArgs e)
        {
           

            // Batch by Order Count
            foreach (GridItem row in grdOrders.Items)
                if (row.OwnerTableView.Name == grdOrders.MasterTableView.Name)
                {
                    
                    string customerName = (string)row.OwnerTableView.DataKeyValues[row.ItemIndex]["CustomerOrganisationName"];
                    int identityID = (int)row.OwnerTableView.DataKeyValues[row.ItemIndex]["CustomerIdentityID"];
                    decimal rate = (decimal)row.OwnerTableView.DataKeyValues[row.ItemIndex]["ForeignRate"];
                    
                    TextBox txtBatchID = row.FindControl("txtBatchId") as TextBox;
                        
                    txtBatchID.Text = GetBatchName(customerName,identityID, rate);
                }

            // display the batch information.
            grdBatches.DataSource = CustomerBatchCount;
            grdBatches.DataBind();

            ClientScript.RegisterStartupScript(this.GetType(), "key", "showBatchPanel();", true);

        }

        #region Page Functions

        protected string GetDate(DateTime date, bool anytime)
        {
            string retVal = string.Empty;

            if (anytime)
                retVal = date.ToString("dd/MM/yy") + " AnyTime";
            else
                retVal = date.ToString("dd/MM/yy HH:mm");

            return retVal;
        }

        #endregion

        #region Private Functions

        private void ConfigureDisplay()
        {
            string MinDate = System.DateTime.Today.ToString();

            // Configure this to enure unique for the user
            this.hidUserName.Value = this.Page.User.Identity.Name;

            Facade.IPreInvoice facPreInvoice = new Facade.PreInvoice();
            DataSet dsLabels = facPreInvoice.GetAllPreInvoiceBatchLabels();
            DataView dv = dsLabels.Tables[0].DefaultView;
            dv.RowFilter = string.Format("CreateUserID = '{0}'" , this.Page.User.Identity.Name);
            this.hidBatchIndicator.Value = "0";

            if (dv.Count > 0)
            {
                foreach (DataRowView row in dv)
                {
                    string label = row["BatchLabel"].ToString();
                    if (label.Contains(this.Page.User.Identity.Name + "-"))
                    {
                        int numberPart = 0;
                        bool isNumber = int.TryParse(label.Substring(label.IndexOf("-") + 1), out numberPart);
                        if (isNumber)
                            if (numberPart > int.Parse(this.hidBatchIndicator.Value))
                                this.hidBatchIndicator.Value = (numberPart).ToString();
                    }
                }
                this.hidBatchIndicator.Value = (int.Parse(this.hidBatchIndicator.Value) + 1).ToString();
            }
            
            lvExistingBatches.DataSource = approvalProcess.BatchLabels;
            lvExistingBatches.DataBind();
            RefreshPreInvoices = "false";
            if (!string.IsNullOrEmpty(Request.QueryString["IdentityId"]))
            {
                int IdentityID = 0;
                int.TryParse(Request.QueryString["IdentityID"].ToString(), out IdentityID);
                m_IdentityId = IdentityID;
                IsFromDash = true;
            }

            if (!string.IsNullOrEmpty(Request.QueryString["MinDate"]))
            {
                MinDate = Request.QueryString["MinDate"];
                IsFromDash = true;
            }

            if (IsFromDash)
            {
                Facade.IOrganisation facOrg = new Facade.Organisation();
                string name = facOrg.GetNameForIdentityId(m_IdentityId);

                cboClient.SelectedValue = m_IdentityId.ToString();
                cboClient.Text = name;

                DateTime startDate = new DateTime();
                DateTime endDate = System.DateTime.Today;
                DateTime.TryParse(MinDate, out startDate);

                rdiStartDate.SelectedDate = startDate;
                rdiEndDate.SelectedDate = endDate;

                //Unselect NORM and select BOTH
                rdoListInvoiceType.Items[0].Selected = false;
                rdoListInvoiceType.Items[1].Selected = true;
            }

            LoadBusinessTypes();

            string delOrderNoText = Orchestrator.Globals.Configuration.SystemDocketNumberText;
            grdOrders.Columns.FindByUniqueName("DeliveryOrderNumber").HeaderText = String.IsNullOrEmpty(delOrderNoText) ? "Delivery Order No" : delOrderNoText;

            HttpCookie lastInvoiceDateCookie = Request.Cookies[COOKIE_LAST_INVOICE_DATE];
            if (lastInvoiceDateCookie != null)
            {
                rdiBatchInvoiceDate.SelectedDate = DateTime.Parse(lastInvoiceDateCookie.Value);
                rdiBatchInvoiceDateBottom.SelectedDate = DateTime.Parse(lastInvoiceDateCookie.Value);
            }
            else
            {
                rdiBatchInvoiceDate.SelectedDate = DateTime.Now.Date;
                rdiBatchInvoiceDateBottom.SelectedDate = DateTime.Now.Date;
            }
        }

        private void LoadBusinessTypes()
        {
            Facade.IBusinessType facBusinessType = new Facade.BusinessType();
            DataSet dsBusinessTypes = facBusinessType.GetAll();

            foreach (DataRow row in dsBusinessTypes.Tables[0].Rows)
            {
                ListItem li = new ListItem(row["Description"].ToString(), row["BusinessTypeID"].ToString());
                // Select all the business types by default.
                li.Selected = true;
                cblBusinessType.Items.Add(li);
                
            }
        }

        private void RefreshData()
        {
            if (!string.IsNullOrEmpty(Request.QueryString["rcbID"]))
                this.DsOrders = null;
            else
                this.DsOrders = GetReadyToInvoiceForDates();

            this.grdOrders.DataSource = null;
            this.grdOrders.DataSource = this.DsOrders;
            this.grdOrders.DataBind();

            this.approvalProcess.Rebind();

            if (this.DsOrders.Tables.Count > 0 && this.DsOrders.Tables[0].Rows.Count > 0)
                btnAutoBatch.Enabled = true;

            lvExistingBatches.DataSource = approvalProcess.BatchLabels;
            lvExistingBatches.DataBind();

            
        }

        private int GetPreInvoiceIdForBatch(string batchId)
        {
            int preInvoiceId = 0;

            foreach (GridItem row in grdOrders.Items)
                if (row.OwnerTableView.Name == grdOrders.MasterTableView.Name)
                {
                    TextBox txtBatchID = row.FindControl("txtBatchId") as TextBox;

                    if (txtBatchID != null && txtBatchID.Text == batchId)
                        preInvoiceId = int.Parse(txtBatchID.Attributes["PreInvoiceId"]);
                }

            return preInvoiceId;
        }

        private DataSet GetReadyToInvoiceForDates()
        {
            int clientID = 0;

            if (!IsFromDash)
                int.TryParse(cboClient.SelectedValue, out clientID);
            else
                clientID = m_IdentityId;

            // Set the Business Types
            List<int> BusinessTypes = new List<int>();
            foreach (ListItem li in cblBusinessType.Items)
                if (li.Selected)
                    BusinessTypes.Add(int.Parse(li.Value));

            if (BusinessTypes.Count == 0)
            {
                // none selected so assume to select all
                foreach (ListItem li in cblBusinessType.Items)
                    BusinessTypes.Add(int.Parse(li.Value));
            }

            DataSet ds = null;

            Orchestrator.Facade.IOrder facOrder = new Orchestrator.Facade.Order();
            ds = facOrder.GetDeliveredForInvoicing(rdiStartDate.SelectedDate, rdiEndDate.SelectedDate, 
                    (cboSearchAgainstDate.Items[0].Selected || cboSearchAgainstDate.Items[2].Selected),
                    (cboSearchAgainstDate.Items[1].Selected || cboSearchAgainstDate.Items[2].Selected), 
                    BusinessTypes, rdoListInvoiceType.Items[0].Selected, rdoListInvoiceType.Items[1].Selected, clientID);

            if (approvalProcess.SelectedPreInvoices.Count > 0)
            {
                DataSet dsOrdersOnPreInvoice = null;
                dsOrdersOnPreInvoice = facOrder.GetForPreInvoiceIds(approvalProcess.SelectedPreInvoices);

                ds.Tables[0].Merge(dsOrdersOnPreInvoice.Tables[0], true);
            }

            IsFromDash = false;

            return ds;
        }

        private List<int> GetOrderIdsForBatch(string batchId)
        {
            List<int> orderIds = new List<int>();

            foreach (GridItem row in grdOrders.Items)
                if (row.OwnerTableView.Name == grdOrders.MasterTableView.Name)
                {
                    TextBox txtBatchID = row.FindControl("txtBatchId") as TextBox;

                    if (txtBatchID != null && txtBatchID.Text == batchId)
                        orderIds.Add(int.Parse(row.OwnerTableView.DataKeyValues[row.ItemIndex]["OrderID"].ToString()));
                }

            return orderIds;
        }

        private List<int> GetOrderIdsForPreInvoice(string preInvoiceId)
        {
            List<int> orderIds = new List<int>();

            foreach (GridItem row in grdOrders.Items)
                if (row.OwnerTableView.Name == grdOrders.MasterTableView.Name)
                {
                    TextBox txtBatchID = row.FindControl("txtBatchId") as TextBox;

                    if (txtBatchID != null && txtBatchID.Attributes["PreInvoiceId"] == preInvoiceId)
                        orderIds.Add(int.Parse(row.OwnerTableView.DataKeyValues[row.ItemIndex]["OrderID"].ToString()));
                }

            return orderIds;
        }

        #endregion

        #region Events

        #region UserControl

        void approvalProcess_SelectedPreInvoiceChanged(object sender, SelectedPreInvoiceChangedEventArgs e)
        {
            foreach (GridItem row in grdOrders.Items)
                if (row.OwnerTableView.Name == grdOrders.MasterTableView.Name)
                {
                    TextBox txtBatchId = (TextBox)row.FindControl("txtBatchId");
                    if (txtBatchId.Attributes["PreInvoiceId"] == e.PreInvoiceID.ToString())
                        row.Selected = true;
                    else
                        row.Selected = false;
                }
        }

        void approvalProcess_ProcessActions(object sender, EventArgs e)
        {
            List<int> sent = new List<int>();
            sent.AddRange(approvalProcess.ApproveAndPost);
            sent.AddRange(approvalProcess.Approve);
            sent.AddRange(approvalProcess.Reject);

            this.approvalProcess.PreInvoiceIdsSentToWorkFlow.Clear();
            this.approvalProcess.PreInvoiceIdsSentToWorkFlow = sent;

            List<int> ToBeProcessed = new List<int>();

            ToBeProcessed.AddRange(approvalProcess.ApproveAndPost);
            ToBeProcessed.AddRange(approvalProcess.Approve);
            ToBeProcessed.AddRange(approvalProcess.Regenerate);

            int count = ToBeProcessed.Count + approvalProcess.Reject.Count;

            StringBuilder toBeProcessed = new StringBuilder();
            StringBuilder toReject = new StringBuilder();

            foreach (int i in ToBeProcessed)
                if (toBeProcessed.Length > 0)
                    toBeProcessed.Append("," + i.ToString());
                else
                    toBeProcessed.Append(i.ToString());

            foreach (int i in approvalProcess.Reject)
                if (toReject.Length > 0)
                    toReject.Append("," + i.ToString());
                else
                    toReject.Append(i.ToString());

            bool ProcessingComplete = false;
            int iteration = 0;

            while (!ProcessingComplete)
            {
                Facade.IWorkflowPreInvoice facPreInvoice = new Facade.PreInvoice();
                int foundCount = facPreInvoice.GetCountOfProcessedForInvoiceGenerationParamatersAndActionDateTime(toBeProcessed.ToString(), toReject.ToString(), approvalProcess.LastActionDate, ((Orchestrator.Entities.CustomPrincipal)Page.User).UserName);

                if (foundCount == count || iteration == 5) // Escape squence.
                    ProcessingComplete = true;
                else
                {
                    Thread.Sleep(2000);
                }

                iteration++;
            }

            if (approvalProcess.Approve.Count > 0 || approvalProcess.ApproveAndPost.Count > 0)
            {
                StringBuilder completed = new StringBuilder();

                foreach (int i in approvalProcess.ApproveAndPost)
                    if (completed.Length > 0)
                        completed.Append("," + i.ToString());
                    else
                        completed.Append(i.ToString());

                foreach (int i in approvalProcess.Approve)
                    if (completed.Length > 0)
                        completed.Append("," + i.ToString());
                    else
                        completed.Append(i.ToString());


                Facade.IInvoice facInvoice = new Facade.Invoice();
                List<int> InvoiceIds = facInvoice.GetInvoiceIdsForInvoiceGenerationParameters(completed.ToString());

                if (InvoiceIds != null && InvoiceIds.Count > 0)
                {
                    StringBuilder invoices = new StringBuilder();
                    foreach (int i in InvoiceIds)
                        if (invoices.Length > 0)
                            invoices.Append("," + i.ToString());
                        else
                            invoices.Append(i.ToString());

                    dlgOrder.Open(string.Format("i={0}", invoices.ToString()));
                }
            }

            RefreshData();
        }

        #endregion

        #region Grid

        void grdOrders_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            this.grdOrders.DataSource = null;
            this.grdOrders.DataSource = this.DsOrders;

            //this.approvalProcess.Rebind();

            if (this.DsOrders != null && this.DsOrders.Tables.Count > 0 && this.DsOrders.Tables[0].Rows.Count > 0)
                btnAutoBatch.Enabled = true;

            lvExistingBatches.DataSource = approvalProcess.BatchLabels;
            lvExistingBatches.DataBind();
        }

        void grdOrders_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridHeaderItem)
            {
                CheckBox chk = (CheckBox)e.Item.FindControl("chkSelectAll");
                if (chk != null)
                    chk.Attributes.Add("onclick", "javascript:HandleGridSelection();");
            }

            if (e.Item is GridDataItem)
            {
                DataRowView drv = (DataRowView)e.Item.DataItem;

                if (e.Item.OwnerTableView.Name == grdOrders.MasterTableView.Name)
                {
                    TextBox txtBatchId = (TextBox)e.Item.FindControl("txtBatchId");
                    Label lblReferences = (Label)e.Item.FindControl("lblReferences");
                    lblReferences.Text = (drv["OrderReferences"] == DBNull.Value) ? String.Empty : drv["OrderReferences"].ToString();
                    Label lblCharge = e.Item.FindControl("lblCharge") as Label;
                    HtmlInputCheckBox chkOrder = (HtmlInputCheckBox)e.Item.FindControl("chkOrder");
                    
                    txtBatchId.Attributes.Add("onchange",
                                                  string.Format(
                                                      "javascript:HandleGroupSelection(event, this, {0}, {1});",
                                                      drv["OrderGroupID"], drv["OrderId"]));

                    txtBatchId.Attributes.Add("PreInvoiceBatchId", drv["PreInvoiceBatchID"].ToString());
                    txtBatchId.Attributes.Add("PreInvoiceId", drv["PreInvoiceId"].ToString());
                    txtBatchId.Attributes.Add("OBatchLabel", drv["BatchLabel"].ToString());
                    txtBatchId.Attributes.Add("OCustomerIdentityId", drv["CustomerIdentityId"].ToString());
                    txtBatchId.Attributes.Add("IGPId", drv["InvoiceGenerationParametersID"].ToString());
                    txtBatchId.Attributes.Add("OrderGroupId", ((int)drv["OrderGroupId"]).ToString());

                    txtBatchId.Text = (drv["BatchLabel"] == DBNull.Value) ? String.Empty : drv["BatchLabel"].ToString();
                    txtBatchId.TabIndex = ((short)(e.Item.RowIndex - 1));

                    HtmlAnchor lnkOrderId = (HtmlAnchor)e.Item.FindControl("lnkOrderId");
                    lnkOrderId.InnerHtml = drv["OrderId"].ToString();
                    lnkOrderId.HRef = "javascript:viewOrderProfile(" + drv["OrderId"].ToString() + ");";

                    HtmlAnchor lnkJobId = (HtmlAnchor)e.Item.FindControl("lnkJobId");
                    string jobId = drv["JobId"].ToString();
                    if (jobId != String.Empty)
                    {
                        lnkJobId.InnerHtml = jobId;
                        lnkJobId.HRef = "javascript:openResizableDialogWithScrollbars('/job/job.aspx?jobId=" + jobId + "' + getCSID(),'1220','870');";
                    }

                    if (lblCharge != null)
                    {
                        CultureInfo culture = new CultureInfo((int)drv["LCID"]);
                        lblCharge.Text = ((decimal)drv["ForeignRate"]).ToString("C", culture);
                        lblCharge.Attributes.Add("OrderGroupId", ((int)drv["OrderGroupId"]).ToString());
                        lblCharge.Attributes.Add("Rate", ((decimal)drv["ForeignRate"]).ToString("N"));
                        chkOrder.Attributes.Add("Rate", ((decimal)drv["ForeignRate"]).ToString("N"));
                        chkOrder.Attributes.Add("OrderGroupId", ((int)drv["OrderGroupId"]).ToString());
                        chkOrder.Attributes.Add("Index", _indexCount.ToString());
                        _indexCount++;
                    }
                }
            }
        }
        private int _indexCount = 0;
        #endregion

        #region Combobox

        void cboClient_ItemsRequested(object o, RadComboBoxItemsRequestedEventArgs e)
        {
            cboClient.Items.Clear();
            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllClientsFiltered("%" + e.Text);
            DataTable dt = ds.Tables[0];
            DataTable boundResults = dt.Clone();

            int itemsPerRequest = 20;
            int itemOffset = e.NumberOfItems;
            int endOffset = itemOffset + itemsPerRequest;
            if (endOffset > dt.Rows.Count)
                endOffset = dt.Rows.Count;

            for (int i = itemOffset; i < endOffset; i++)
                boundResults.ImportRow(dt.Rows[i]);

            cboClient.DataSource = boundResults;
            cboClient.DataBind();

            if (dt.Rows.Count > 0)
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
        }

        #endregion

        #region Button

        void btnRefresh_Click(object sender, EventArgs e)
        {
            RefreshData();
        }


        /// <summary>
        /// this will allow people to enter an existing batch label against one or more order and have these added to that existing batch.
        /// </summary>
        Dictionary<string, DataSet> PreInvioceData = new Dictionary<string, DataSet>();
        List<Triplet> PreInvoiceAmenments = new List<Triplet>();
        
        
        private void AlterBatch()
        {
            Orchestrator.Facade.IOrder facOrder = new Orchestrator.Facade.Order();
            Orchestrator.Facade.IPreInvoice facPreInvoice = new Orchestrator.Facade.PreInvoice();

            // identify if there is a Pre Invoice with this label.
            // build up the list of changes, we must ber in mind order groups....
            foreach (GridItem row in grdOrders.Items)
            {
                if (row.OwnerTableView.Name == grdOrders.MasterTableView.Name)
                {
                    TextBox txtBatchID = row.FindControl("txtBatchId") as TextBox;
                    HyperLink lnkOrderId = row.FindControl("lnkOrderId") as HyperLink;
                    string batchLabel = txtBatchID.Text.Trim();

                    if (batchLabel.Length > 0)
                    {
                        // check to see if we have already loaded this batch
                        if (!PreInvioceData.ContainsKey(batchLabel))
                        {
                            // check to see if we have a batch with this label
                            DataSet ds = facPreInvoice.GetOrdersForBatchLabel(batchLabel);
                           
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                PreInvioceData.Add(batchLabel, ds);
                                // Modify this batch
                                if ((int)row.OwnerTableView.DataKeyValues[row.ItemIndex]["CustomerIdentityID"] == (int)ds.Tables[0].Rows[0]["CustomerIdentityID"])
                                {
                                    if (PreInvoiceAmenments.Count() > 0)
                                    {
                                        if (!PreInvoiceAmenments.Exists(x => x.First.ToString() == batchLabel))
                                        {
                                            PreInvoiceAmenments.Add(new Triplet(batchLabel, (int)ds.Tables[0].Rows[0]["PreInvoiceID"], new List<int>() { (int)row.OwnerTableView.DataKeyValues[row.ItemIndex]["OrderID"] }));
                                            foreach (DataRow o in ds.Tables[0].Rows)
                                                ((List<int>)PreInvoiceAmenments.First(x => x.First.ToString() == batchLabel).Third).Add((int)o["OrderID"]);
                                        }
                                        else
                                        {
                                            ((List<int>)PreInvoiceAmenments.First(x => x.First.ToString() == batchLabel).Third).Add((int)row.OwnerTableView.DataKeyValues[row.ItemIndex]["OrderID"]);
                                        }
                                    }
                                    else
                                    {
                                        PreInvoiceAmenments.Add(new Triplet(batchLabel, (int)ds.Tables[0].Rows[0]["PreInvoiceID"], new List<int>() { (int)row.OwnerTableView.DataKeyValues[row.ItemIndex]["OrderID"] }));
                                        foreach(DataRow o in ds.Tables[0].Rows )
                                            ((List<int>)PreInvoiceAmenments.First(x => x.First.ToString() == batchLabel).Third).Add((int)o["OrderID"]);

                                    }
                                }
                            }
                        }
                        else
                        {
                            // already loaded this batch no need to load again
                            if ((int)PreInvioceData[batchLabel].Tables[0].Rows[0]["CustomerIdentityID"] == (int)row.OwnerTableView.DataKeyValues[row.ItemIndex]["CustomerIdentityID"])
                            {
                                // we can add this to this batch as it is for the same customer
                                ((List<int>)PreInvoiceAmenments.First(x => x.First.ToString() == batchLabel).Third).Add((int)row.OwnerTableView.DataKeyValues[row.ItemIndex]["OrderID"]);
                            }
                        }
                    }
                }
            }

            if (PreInvoiceAmenments.Count > 0)
            {
                // Update the Pre Invoice 
                userName = Page.User.Identity.Name;
                List<int> preInvoiceIds = new List<int>();
                foreach (var pre in PreInvoiceAmenments)
                {
                    List<int> selectedOrderIDs = new List<int>();
                    facPreInvoice.RemovePreInvoiceOrders((int)pre.Second);
                    facPreInvoice.UpdatePreInvoiceOrders((int)pre.Second, (List<int>)pre.Third);
                    preInvoiceIds.Add((int)pre.Second);
                }

                try
                {
                    List<int> empty = new List<int>();
                    List<Orchestrator.Contracts.DataContracts.NotificationParty> emptyNotification = new List<Orchestrator.Contracts.DataContracts.NotificationParty>();
                    // Kick off the workflow.
                    GenerateInvoiceClient gic = new GenerateInvoiceClient("Orchestrator.InvoiceService");
                    gic.VerifyInvoices(empty.ToArray(), empty.ToArray(), preInvoiceIds.ToArray(), empty.ToArray(), emptyNotification.ToArray(), userName);
                }
                catch (System.ServiceModel.EndpointNotFoundException exc)
                {
                    // Not possible to send message to workflow host - send email to support.
                    Utilities.SendSupportEmailHelper("GenerateInvoiceClient.GenerateGroupageInvoiceAutoRun(int, Orchestrator.Entities.NotificationParty[], string)", exc);
                    this.lblError.Text = exc.Message;
                }
                DateTime lastActionDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);
                List<int> e = new List<int>();
                ProgressReporting(lastActionDate, e, preInvoiceIds, e);
                RefreshData();
                this.approvalProcess.Rebind();
            }

        }

        void btnCreateBatch_Click(object sender, EventArgs e)
        {
            //TODO: Find out if this should be the last invoice date and if so remove duplicate.
            DateTime lastActionDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, DateTime.Now.Hour, DateTime.Now.Minute, 0);

            DateTime invoiceDate = DateTime.Today;
            if(rdiBatchInvoiceDate.SelectedDate.HasValue)
                invoiceDate = rdiBatchInvoiceDate.SelectedDate.Value;

            HttpCookie lastInvoiceCookie = Request.Cookies[COOKIE_LAST_INVOICE_DATE];
            if (lastInvoiceCookie==null)
                lastInvoiceCookie = new HttpCookie(COOKIE_LAST_INVOICE_DATE);

            lastInvoiceCookie.Value=invoiceDate.ToShortDateString();
            Response.Cookies.Add(lastInvoiceCookie);

            List<BatchLabel> batchInfo = new List<BatchLabel>();
            //Dictionary<string, string> batchInfo = new Dictionary<string, string>();
            List<int> AllPreInvoiceIds = new List<int>();
            List<int> batchIds = new List<int>();
            List<int> preInvoiceIds = new List<int>();
            List<int> preInvoiceIdsToReject = new List<int>();
            List<int> iGPUpdate = new List<int>();
            List<int> iGPReject = new List<int>();

            Orchestrator.Facade.IPreInvoice facPreInvoice = new Orchestrator.Facade.PreInvoice();

            foreach (GridItem row in grdOrders.Items)
                if (row.OwnerTableView.Name == grdOrders.MasterTableView.Name)
                {
                    TextBox txtBatchID = row.FindControl("txtBatchId") as TextBox;
                    HyperLink lnkOrderId = row.FindControl("lnkOrderId") as HyperLink;
        
                    int isFlaggedForInvoicing = (int)row.OwnerTableView.DataKeyValues[row.ItemIndex]["IsFlaggedForInvoicing"];
                    HtmlInputCheckBox chkOrder = (HtmlInputCheckBox)row.FindControl("chkOrder");
                    
                    if (isFlaggedForInvoicing != 1 && chkOrder.Checked == true)
                    {
                        int orderID = (int)row.OwnerTableView.DataKeyValues[row.ItemIndex]["OrderID"];
                        ClientScript.RegisterStartupScript(GetType(), "Error", "alert('Order ID: " + orderID + " has not been Flagged as Ready to Invoice yet please review before proceeding');", true);
                        return;
                    }
                    if (txtBatchID != null && !String.IsNullOrEmpty(txtBatchID.Text))
                    {
                        BatchLabel bl = batchInfo.Find(lbl => lbl.NewBatchLabel == txtBatchID.Text.Trim());

                        if(bl == null || (bl.OriginalBatchLabel == string.Empty && bl.PreInvoiceId == 0))
                        {
                            int preInvoiceId = -1;
                            int.TryParse(txtBatchID.Attributes["PreInvoiceId"], out preInvoiceId);

                            string OBatchLabel = txtBatchID.Attributes["OBatchLabel"];

                            if (bl == null)
                                batchInfo.Add(new BatchLabel(OBatchLabel, txtBatchID.Text.Trim(), preInvoiceId));
                            else
                            {
                                bl.OriginalBatchLabel = OBatchLabel;
                                bl.PreInvoiceId = preInvoiceId;
                            }

                            if (!String.IsNullOrEmpty(txtBatchID.Attributes["IGPId"]))
                                iGPUpdate.Add(int.Parse(txtBatchID.Attributes["IGPId"]));
                        }
                    }

                    if (txtBatchID != null && String.IsNullOrEmpty(txtBatchID.Text) && !String.IsNullOrEmpty(txtBatchID.Attributes["PreInvoiceId"]))
                        if (!preInvoiceIdsToReject.Contains(int.Parse(txtBatchID.Attributes["PreInvoiceId"])))
                        {
                            preInvoiceIdsToReject.Add(int.Parse(txtBatchID.Attributes["PreInvoiceId"]));
                            iGPReject.Add(int.Parse(txtBatchID.Attributes["IGPId"]));
                        }

                    if (!String.IsNullOrEmpty(txtBatchID.Attributes["PreInvoiceId"]) && !AllPreInvoiceIds.Contains(int.Parse((txtBatchID.Attributes["PreInvoiceId"]))))
                        AllPreInvoiceIds.Add(int.Parse(txtBatchID.Attributes["PreInvoiceId"]));
                }

            // This is to reset all Pre-Invoices that are due to be updated - this will avoid any duplication issues with moving an order from 1 pre-invoice to another.
            foreach (int piID in AllPreInvoiceIds)
                facPreInvoice.RemovePreInvoiceOrders(piID);

            foreach(BatchLabel batch in batchInfo)
            {
                List<int> selectedOrderIDs = new List<int>();

                selectedOrderIDs = this.GetOrderIdsForBatch(batch.NewBatchLabel);

                if (selectedOrderIDs.Count > 0 && Page.IsValid)
                {
                    userName = Page.User.Identity.Name;

                    if(batch.PreInvoiceId > 0 && batch.ToUpdate)
                    {
                        facPreInvoice.UpdatePreInvoiceOrders(batch.PreInvoiceId, selectedOrderIDs);
                        preInvoiceIds.Add(batch.PreInvoiceId);
                        preInvoiceIdsToReject.Remove(batch.PreInvoiceId);
                    }
                    else
                    {
                        int batchID = facPreInvoice.CreateBatch(invoiceDate, selectedOrderIDs, userName);

                        try
                        {
                            if (batchID > 0)
                            {
                                batchIds.Add(batchID);
                                // Kick off the workflow.
                                GenerateInvoiceClient gic = new GenerateInvoiceClient("Orchestrator.InvoiceService");
                                gic.GenerateGroupageInvoiceAutoRun(batchID, batch.NewBatchLabel, new Orchestrator.Contracts.DataContracts.NotificationParty[] { }, String.Empty, String.Empty, userName);
                            }
                        }
                        catch (System.ServiceModel.EndpointNotFoundException exc)
                        {
                            // Not possible to send message to workflow host - send email to support.
                            Utilities.SendSupportEmailHelper("GenerateInvoiceClient.GenerateGroupageInvoiceAutoRun(int, Orchestrator.Entities.NotificationParty[], string)", exc);
                            this.lblError.Text = exc.Message;
                        }
                    }
                }
            }

            foreach (int preInvoiceId in preInvoiceIdsToReject)
            {
                List<int> orderIds = new List<int>();
                orderIds = this.GetOrderIdsForPreInvoice(preInvoiceId.ToString());
                facPreInvoice.UpdatePreInvoiceOrders(preInvoiceId, orderIds);
            }

            if (preInvoiceIds.Count > 0 || preInvoiceIdsToReject.Count > 0)
                try
                {
                    List<int> empty = new List<int>();
                    List<Orchestrator.Contracts.DataContracts.NotificationParty> emptyNotification = new List<Orchestrator.Contracts.DataContracts.NotificationParty>();
                    // Kick off the workflow.
                    GenerateInvoiceClient gic = new GenerateInvoiceClient("Orchestrator.InvoiceService");
                    gic.VerifyInvoices(empty.ToArray(), empty.ToArray(), preInvoiceIds.ToArray(), preInvoiceIdsToReject.ToArray(), emptyNotification.ToArray(), userName);
                }
                catch (System.ServiceModel.EndpointNotFoundException exc)
                {
                    // Not possible to send message to workflow host - send email to support.
                    Utilities.SendSupportEmailHelper("GenerateInvoiceClient.GenerateGroupageInvoiceAutoRun(int, Orchestrator.Entities.NotificationParty[], string)", exc);
                    this.lblError.Text = exc.Message;
                }

            // Insert Check here for Polling the Pre Invoice Batches to see if they've been removed / Invoice generation Parameters have been removed.
            // And Check the invoice generation parameters for the new Pre-Invoice generated.

            ProgressReporting(lastActionDate, batchIds, iGPUpdate, iGPReject);

            //dlgOrder.

            facPreInvoice.RemoveEmptyPreInvoices(AllPreInvoiceIds);
            RefreshData();
            this.approvalProcess.Rebind();

            // Incremement the batch count.
            int indicator = Convert.ToInt32(this.hidBatchIndicator.Value);
            this.hidBatchIndicator.Value = (++indicator).ToString();
        }

        private void ProgressReporting(DateTime lastActionDate, List<int> batchIds, List<int> iGPUpdate, List<int> iGPReject)
        {
            bool ProcessingComplete = false;
            int modifiedCount = iGPUpdate.Count + iGPReject.Count;

            StringBuilder toBeProcessed = new StringBuilder();
            StringBuilder toReject = new StringBuilder();
            StringBuilder toCreate = new StringBuilder();

            foreach (int i in iGPUpdate)
                if (toBeProcessed.Length > 0)
                    toBeProcessed.Append("," + i.ToString());
                else
                    toBeProcessed.Append(i.ToString());

            foreach (int i in iGPReject)
                if (toReject.Length > 0)
                    toReject.Append("," + i.ToString());
                else
                    toReject.Append(i.ToString());

            foreach (int i in batchIds)
                if (toCreate.Length > 0)
                    toCreate.Append("," + i.ToString());
                else
                    toCreate.Append(i.ToString());

            int iteration = 0;
            int foundModifiedCount = 0, foundCreatedCount = 0;
            while (!ProcessingComplete)
            {
                Facade.IWorkflowPreInvoice facWFPreInvoice = new Facade.PreInvoice();

                if (toBeProcessed.Length > 0 || toBeProcessed.Length > 0)
                    foundModifiedCount = facWFPreInvoice.GetCountOfProcessedForInvoiceGenerationParamatersAndActionDateTime(toBeProcessed.ToString(), toReject.ToString(), lastActionDate, userName);

                if (toCreate.Length > 0)
                    foundCreatedCount = facWFPreInvoice.GetCountOfProcessedPreInvoiceBatchIdsAndActionDateTime(toCreate.ToString(), lastActionDate, userName);

                // Both the Iteration value and Delay value need to be system settings.
                if ((foundModifiedCount == modifiedCount && foundCreatedCount == 0) || iteration >= 5)
                    ProcessingComplete = true;

                // Delay for 3 seconds then try again.
                DateTime dt;

                if (ProcessingComplete)
                    dt = DateTime.Now.AddSeconds(6); // Allow sufficent time for all Preinvoice to be ready when loading them into the usercontrol.
                else
                    dt = DateTime.Now.AddSeconds(3);

                DateTime newdate = DateTime.Now;
                do { newdate = DateTime.Now; }
                while (newdate < dt);

                iteration++;
            }

            RefreshPreInvoices = "true";
            
        }

        #endregion

        #endregion        
    }
}
