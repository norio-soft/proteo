using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

using Telerik.Web.UI;

namespace Orchestrator.WebUI.UserControls.Invoicing
{
    public partial class ApprovalProcess : System.Web.UI.UserControl
    {
        public event EventHandler ProcessActions;
        public event SelectedPreInvoiceChangedEventHandler SelectedPreInvoiceChanged;

        #region Control Fields

        private const string _VS_WorkflowInstanceID = "_VS_WorkflowInstanceID";
        private const string _VS_StayOpen = "_VS_StayOpen";
        private const string _VS_DS_PREINVOICES = "_VS_DS_PREINVOICES";

        #endregion

        #region Properties

        /// <summary>
        /// Access the workflow instance id being used during the approval process.
        /// </summary>
        private Guid WorkflowInstanceID
        {
            get
            {
                if (ViewState[_VS_WorkflowInstanceID] == null)
                    return Guid.Empty;
                else
                    return (Guid)ViewState[_VS_WorkflowInstanceID];
            }
            set { ViewState[_VS_WorkflowInstanceID] = value; }
        }

        private int StayOpen
        {
            get
            {
                if (ViewState[_VS_StayOpen] == null)
                    return -1;
                else
                    return (int)ViewState[_VS_StayOpen];
            }
            set { ViewState[_VS_StayOpen] = value; }
        }

        private DataSet dsTaxRates
        {
            get
            {
                DataSet dsTaxRates = null;
                if (ViewState["dsTaxRates"] != null)
                {
                    dsTaxRates = (DataSet)ViewState["dsTaxRates"];
                }

                return dsTaxRates;
            }
            set
            {
                ViewState["dsTaxRates"] = value;
            }
        }

        private DataSet DsPreInvoices
        {
            get 
            {
                DataSet dsPreInvoices = null;
                if (ViewState["_VS_DS_PREINVOICES"] != null)
                    dsPreInvoices = (DataSet)ViewState["_VS_DS_PREINVOICES"];

                return dsPreInvoices; 
            }
            set { ViewState[_VS_DS_PREINVOICES] = value; }
        }

        private string PreInvoiceTableName
        {
            get { return gvPreInvoices.MasterTableView.Name; }
        }

        private string PreInvoiceItemTableName
        {
            get { return gvPreInvoices.MasterTableView.DetailTables[0].Name; }
        }

        #region Exposed Properties For PreInvoice Actions

        private List<int> _existing = new List<int>();
        public List<int> Existing
        {
            get {

                int preInvoiceBatchId = -1;

                foreach (GridDataItem row in gvPreInvoices.Items)
                    if (row.OwnerTableView.Name == PreInvoiceTableName && (row.ItemType == GridItemType.Item || row.ItemType == GridItemType.AlternatingItem))
                    {
                        preInvoiceBatchId = int.Parse((gvPreInvoices.MasterTableView.DataKeyValues[row.ItemIndex]["PreInvoiceBatchID"]).ToString());

                        if(!_existing.Contains(preInvoiceBatchId))
                            _existing.Add(preInvoiceBatchId);
                    }

                return _existing; 
            }
        }

        public List<int> PreInvoiceIdsSentToWorkFlow
        {
            get
            {
                List<int> ids = new List<int>();
                if (ViewState["vw_PreInvoiceIdsSentToWorkFlow"] != null)
                    ids = (List<int>)ViewState["vw_PreInvoiceIdsSentToWorkFlow"];

                return ids;
            }
            set
            {
                ViewState["vw_PreInvoiceIdsSentToWorkFlow"] = value;
            }
        }

        private List<int> _selectedPreInvoices = new List<int>();
        public List<int> SelectedPreInvoices
        {
            get
            {
                int preInvoiceId = -1;

                foreach (GridDataItem row in gvPreInvoices.Items)
                    if (row.OwnerTableView.Name == PreInvoiceTableName && (row.ItemType == GridItemType.Item || row.ItemType == GridItemType.AlternatingItem))
                    {
                        CheckBox chkPreInvoice = row.FindControl("chkPreInvoice") as CheckBox;
                        if (chkPreInvoice != null)
                            if(chkPreInvoice.Checked)
                            {
                                preInvoiceId = int.Parse(chkPreInvoice.Attributes["PreInvoiceId"]);

                                if (!_selectedPreInvoices.Contains(preInvoiceId))
                                    _selectedPreInvoices.Add(preInvoiceId);
                            }
                    }

                return _selectedPreInvoices;
            }
        }

        private List<int> _reject = new List<int>();
        public List<int> Reject
        {
            get { return _reject; }
            set { _reject = value; }
        }

        private List<int> _regenerate = new List<int>();
        public List<int> Regenerate
        {
            get { return _regenerate; }
            set { _regenerate = value; }
        }

        private List<int> _approve = new List<int>();
        public List<int> Approve
        {
            get { return _approve; }
            set { _approve = value; }
        }

        private List<int> _approveAndPost = new List<int>();
        public List<int> ApproveAndPost
        {
            get { return _approveAndPost; }
            set { _approveAndPost = value; }
        }

        private List<int> _invoiceGenerationParameters = new List<int>();
        public List<int> InvoiceGenerationParameters
        {
            get { return _invoiceGenerationParameters; }
            set { _invoiceGenerationParameters = value; }
        }

        private DateTime? _lastActionDate = null;
        public DateTime? LastActionDate
        {
            get { return _lastActionDate; }
            set { _lastActionDate = value; }
        }

        private IEnumerable<DataRow> _batchLabels = null;
        public IEnumerable<DataRow> BatchLabels
        {
            get
            {
                if (_batchLabels == null)
                {
                    Facade.IWorkflowPreInvoice facPreInvoice = new Facade.PreInvoice();
                    DataSet ds = facPreInvoice.GetAllWithDetails();

                    if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    {
                        var queryPreInvoiceItems = ds.Tables[0].Rows.Cast<DataRow>().AsEnumerable();
                        var resultRows = from row in queryPreInvoiceItems
                                         select row;

                        _batchLabels = resultRows;
                    }
                }

                return _batchLabels;
            }
        }

        #endregion

        #endregion

        #region Page Methods

        protected void Page_Load(object sender, EventArgs e)
        {
            Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.Invoicing);

            string target = Request.Params["__EVENTTARGET"];
            string args = Request.Params["__EVENTARGUMENT"];

            cblCreatedBy.Attributes.Add("onclick", "javascript:selectCreatedBy(this);");

            if (!string.IsNullOrEmpty(target) && target.ToLower() == "preinvoiceselectedindexchanged" && !string.IsNullOrEmpty(args))
                PreInvoiceSelectedIndexChanged(args);

            if (!IsPostBack && !Page.IsCallback)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["wid"]))
                {
                    try
                    {
                        WorkflowInstanceID = new Guid(Request.QueryString["wid"]);
                    }
                    catch { }
                }
                this.RefreshPreInvoices();

            }
            
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            btnApply.Click += new EventHandler(btnApply_Click);
            btnRefresh.Click += new EventHandler(btnRefresh_Click);
            btnApplyTop.Click += new EventHandler(btnApply_Click);
            btnRefreshTop.Click += new EventHandler(btnRefresh_Click);

            rblFilterInvoices.SelectedIndexChanged += new EventHandler(rblFilterInvoices_SelectedIndexChanged);

            gvPreInvoices.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(gvPreInvoices_NeedDataSource);
            gvPreInvoices.ItemDataBound += new Telerik.Web.UI.GridItemEventHandler(gvPreInvoices_ItemDataBound);
            gvPreInvoices.DetailTableDataBind += new GridDetailTableDataBindEventHandler(gvPreInvoices_DetailTableDataBind);
            gvPreInvoices.ItemCommand += new GridCommandEventHandler(gvPreInvoices_ItemCommand);
            gvPreInvoices.PreRender += new EventHandler(gvPreInvoices_PreRender);

            ramApprovalProcess.AjaxRequest += new RadAjaxControl.AjaxRequestDelegate(ramApprovalProcess_AjaxRequest);
        }

        
        void gvPreInvoices_ItemCommand(object source, GridCommandEventArgs e)
        {
            if (e.CommandName == "Export")
            {
                List<int> orderIDs = new List<int>();
                int preInvoiceId = (int)e.Item.OwnerTableView.ParentItem.GetDataKeyValue("PreInvoiceID");
                if (preInvoiceId > 0)
                { 
                    foreach(GridDataItem item in e.Item.OwnerTableView.Items)
                    {
                        HtmlAnchor lnkOrder = item.FindControl("lnkOrderId") as HtmlAnchor;
                        if (lnkOrder != null)
                        {
                            int orderId = 0;
                            int.TryParse(lnkOrder.InnerHtml, out orderId);
                            orderIDs.Add(orderId);
                        }
                    }

                    Facade.IOrder facOrder = new Facade.Order();
                    DataSet dsOrdersToExport = facOrder.GetOrdersForPreInvoiceExport(orderIDs);

                    Facade.ExtraType facExtraTypes = new Orchestrator.Facade.ExtraType();
                    List<Entities.ExtraType> extraTypes = facExtraTypes.GetForIsEnabled(true);

                    foreach (Entities.ExtraType et in extraTypes)
                        dsOrdersToExport.Tables[0].Columns.Add("[" + et.Description + "]", typeof(decimal));

                    // Extra data
                    foreach (DataRow dr in dsOrdersToExport.Tables[0].Rows)
                    {
                        DataRow[] extraRows = dsOrdersToExport.Tables[1].Select("OrderID = " + dr["OrderID"]);
                        foreach (DataRow extra in extraRows)
                            if(dr.Table.Columns.Contains("[" + extra["ExtraType"] + "]"))
                                dr["[" + extra["ExtraType"] + "]"] = extra["ExtraAmount"]; 
                    }

                    Session["__ExportDS"] = dsOrdersToExport.Tables[0];
                    Response.Redirect("/reports/csvexport.aspx?filename=PreInvoiceExport.CSV");
                }
            }
        }

        #endregion

        #region Private Methods

        private void AddReaction(RadioButton radioButton, string controlID)
        {
            radioButton.Attributes.Remove("onClick");
            radioButton.Attributes.Add("onClick", "javascript:SelectAllItems('" + controlID + "');");
        }

        private void SaveBatchChanges()
        {
            string userName = ((Entities.CustomPrincipal)Page.User).UserName;

            // React to the user's options.
            foreach (GridDataItem row in gvPreInvoices.Items)
            {
                string tableName = row.OwnerTableView.Name;
                int preInvoiceId = -1;
                int.TryParse(row.GetDataKeyValue("PreInvoiceID").ToString(), out preInvoiceId);

                if (tableName == PreInvoiceTableName && (row.ItemType == GridItemType.Item || row.ItemType == GridItemType.AlternatingItem))
                {
                    HiddenField hidIsDirty = row.FindControl("hidIsDirty") as HiddenField;
                    bool isDirty = false;

                    if (bool.TryParse(hidIsDirty.Value, out isDirty) && isDirty)
                    {
                        #region Update PreInvoice
                        DateTime? invoiceDate = null;
                        string clientReference = string.Empty, purchaseOrderReference = string.Empty;
                        int taxRateID = -1;

                        RadDateInput rdiInvoiceDate = row.FindControl("rdiInvoiceDate") as RadDateInput;

                        RadTextBox rtClientReference = row.FindControl("rtClientReference") as RadTextBox;
                        RadTextBox rtPurchaseOrderReference = row.FindControl("rtPurchaseOrderReference") as RadTextBox;

                        DropDownList cboTaxRate = row.FindControl("cboTaxRate") as DropDownList;

                        if (rdiInvoiceDate.SelectedDate.HasValue)
                            invoiceDate = rdiInvoiceDate.SelectedDate.Value;

                        clientReference = rtClientReference.Text;
                        purchaseOrderReference = rtPurchaseOrderReference.Text;

                        int.TryParse(cboTaxRate.SelectedValue, out taxRateID);

                        Facade.IPreInvoice facPreInvoice = new Facade.PreInvoice();
                        facPreInvoice.UpdatePreinvoiceDetails(preInvoiceId, invoiceDate, clientReference, purchaseOrderReference, taxRateID, userName);
                        #endregion
                    }
                }
                else if (tableName == PreInvoiceItemTableName && (row.ItemType == GridItemType.Item || row.ItemType == GridItemType.AlternatingItem))
                {
                    HiddenField hidInvoiceItemIsDirty = row.FindControl("hidInvoiceItemIsDirty") as HiddenField;
                    bool isDirty = false;

                    HiddenField hidInvoiceItemPendingDelete = row.FindControl("hidInvoiceItemPendingDelete") as HiddenField;
                    bool isPendingDelete = false;

                    if (bool.TryParse(hidInvoiceItemPendingDelete.Value, out isPendingDelete) && isPendingDelete)
                    {
                        Facade.IPreInvoice facPreInvoice = new Facade.PreInvoice();

                        TextBox txtRate = row.FindControl("txtRate") as TextBox;
                        GridDataItem parentItem = row.OwnerTableView.ParentItem;

                        int itemID = int.Parse(txtRate.Attributes["ItemID"]);

                        // Update the rate (depends on item type - depends on invoice type)
                        int preInvoiceID = int.Parse(parentItem.GetDataKeyValue("PreInvoiceID").ToString());

                        facPreInvoice.RemovePreInvoiceOrder(preInvoiceID, itemID);
                    }

                    else if (bool.TryParse(hidInvoiceItemIsDirty.Value, out isDirty) && isDirty)
                    {
                        #region Pre Invoice Item

                        decimal rate = 0;
                        HiddenField hidOldRate = row.FindControl("hidOldRate") as HiddenField;
                        TextBox txtRate = row.FindControl("txtRate") as TextBox;
                        GridDataItem parentItem = row.OwnerTableView.ParentItem;

                        int lcid = int.Parse(parentItem.GetDataKeyValue("LCID").ToString());
                        CultureInfo preInvoiceCulture = new CultureInfo(lcid);

                        if (Decimal.TryParse(txtRate.Text, System.Globalization.NumberStyles.Currency, preInvoiceCulture, out rate))
                        {
                            //decimal oldRate = 0;
                            //oldRate = decimal.Parse(hidOldRate.Value);

                            // Only apply the rate change if the rate has altered.
                            //if (oldRate != rate)
                            if (hidOldRate.Value != txtRate.Text)
                            {
                                int itemID = int.Parse(txtRate.Attributes["ItemID"]);

                                // Update the rate (depends on item type - depends on invoice type)
                                int preInvoiceID = int.Parse(parentItem.GetDataKeyValue("PreInvoiceID").ToString());
                                eInvoiceType invoiceType = (eInvoiceType)int.Parse(parentItem.GetDataKeyValue("InvoiceTypeID").ToString());
                                CultureInfo culture = new CultureInfo(Orchestrator.Globals.Configuration.NativeCulture);

                                Facade.IPreInvoice facPreInvoice = new Facade.PreInvoice();
                                Facade.IExchangeRates facER = new Facade.ExchangeRates();

                                switch (invoiceType)
                                {
                                    case eInvoiceType.ClientInvoicing:
                                        #region Client Invoice

                                        Facade.IOrder facOrder = new Facade.Order();
                                        Entities.Order order = facOrder.GetForOrderID(itemID);
                                        order.ForeignRate = rate;

                                        if (order.LCID != culture.LCID)
                                        {
                                            BusinessLogicLayer.IExchangeRates blER = new BusinessLogicLayer.ExchangeRates();
                                            BusinessLogicLayer.CurrencyConverter currencyConverter = blER.CreateCurrencyConverter(order.LCID, order.CollectionDateTime);
                                            order.Rate = currencyConverter.ConvertToLocal(order.ForeignRate);
                                        }
                                        else
                                            order.Rate = decimal.Round(order.ForeignRate, 4, MidpointRounding.AwayFromZero);

                                        facPreInvoice.UpdateOrderRate(preInvoiceID, itemID, order.ForeignRate, order.Rate, userName);

                                        #endregion
                                        break;
                                    case eInvoiceType.SubContractorSelfBill:
                                    case eInvoiceType.SubContract:
                                        #region Subby Invoicing
                                        // Needs to be Amended For Currency : TL 12/05/08;
                                        Facade.IJobSubContractor facJSC = new Facade.Job();
                                        Entities.JobSubContractor jobSubContractor = facJSC.GetSubContractorForJobSubContractId(itemID);
                                        jobSubContractor.ForeignRate = rate;

                                        if (jobSubContractor.LCID != culture.LCID)
                                            jobSubContractor.Rate = facER.GetConvertedRate((int)jobSubContractor.ExchangeRateID, jobSubContractor.ForeignRate);
                                        else
                                            jobSubContractor.Rate = decimal.Round(jobSubContractor.ForeignRate, 4, MidpointRounding.AwayFromZero);

                                        facPreInvoice.UpdateJobSubContractRate(preInvoiceID, itemID, jobSubContractor.ForeignRate, jobSubContractor.Rate, jobSubContractor.ExchangeRateID, userName);
                                        #endregion
                                        break;
                                    default:
                                        throw new NotSupportedException("You can not alter item rates on invoices of type " + invoiceType.ToString());
                                }
                            }
                        }

                        #endregion
                    }
                }
            }
        }

        private void PreInvoiceSelectedIndexChanged(string argument)
        {
            int preInvoiceId = 0;
            int.TryParse(argument, out preInvoiceId);

            if(preInvoiceId > 0)
                if (SelectedPreInvoiceChanged != null)
                    SelectedPreInvoiceChanged(this, new SelectedPreInvoiceChangedEventArgs(preInvoiceId));
        }

        #endregion

        #region Public Methods

        public void Rebind()
        {
            this.RefreshPreInvoices();
        }

        #endregion

        #region Event Handlers

        #region Ajax Events

        void ramApprovalProcess_AjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            PreInvoiceSelectedIndexChanged(e.Argument);
        }

        #endregion

        #region Button Events

        void btnApply_Click(object sender, EventArgs e)
        {
            List<int> reject = new List<int>();
            List<int> regenerate = new List<int>();
            List<int> approve = new List<int>();
            List<int> approveAndPost = new List<int>();

            // Persist Row Details
            SaveBatchChanges();

            // React to the user's options.
            foreach (GridDataItem row in gvPreInvoices.Items)
            {
                #region Retrieve User Options

                if (row.OwnerTableView.Name == PreInvoiceTableName && (row.ItemType == GridItemType.Item || row.ItemType == GridItemType.AlternatingItem))
                {
                    HtmlInputRadioButton rdoReject = row.FindControl("rdoReject") as HtmlInputRadioButton;
                    HtmlInputRadioButton rdoApprove = row.FindControl("rdoApprove") as HtmlInputRadioButton;
                    HtmlInputRadioButton rdoApproveAndPost = row.FindControl("rdoApproveAndPost") as HtmlInputRadioButton;
                    HtmlInputCheckBox chkUseHeadedPaper = row.FindControl("chkUseHeadedPaper") as HtmlInputCheckBox;


                    HiddenField hidIsDirty = row.FindControl("hidIsDirty") as HiddenField;
                    bool isDirty = false;
                    bool.TryParse(hidIsDirty.Value, out isDirty);

                    if (!isDirty)
                    {
                        if (rdoReject.Checked)
                        {
                            reject.Add(int.Parse((gvPreInvoices.MasterTableView.DataKeyValues[row.ItemIndex]["PreInvoiceID"]).ToString()));
                            Reject.Add(int.Parse((gvPreInvoices.MasterTableView.DataKeyValues[row.ItemIndex]["InvoiceGenerationParametersID"]).ToString()));
                        }
                        else if (rdoApprove.Checked)
                        {
                            approve.Add(int.Parse((gvPreInvoices.MasterTableView.DataKeyValues[row.ItemIndex]["PreInvoiceID"]).ToString()));
                            Approve.Add(int.Parse((gvPreInvoices.MasterTableView.DataKeyValues[row.ItemIndex]["InvoiceGenerationParametersID"]).ToString()));
                        }
                        else if (rdoApproveAndPost.Checked)
                        {
                            approveAndPost.Add(int.Parse((gvPreInvoices.MasterTableView.DataKeyValues[row.ItemIndex]["PreInvoiceID"]).ToString()));
                            ApproveAndPost.Add(int.Parse((gvPreInvoices.MasterTableView.DataKeyValues[row.ItemIndex]["InvoiceGenerationParametersID"]).ToString()));
                        }
                        if (rdoApprove.Checked || rdoApproveAndPost.Checked)
                        {
                            if (chkUseHeadedPaper.Checked)
                            {
                                SetUseHeadedPaperParameter(int.Parse((gvPreInvoices.MasterTableView.DataKeyValues[row.ItemIndex]["PreInvoiceID"]).ToString()), chkUseHeadedPaper.Checked);
                            }
                        }
                    }
                    else
                    {
                        regenerate.Add(int.Parse((gvPreInvoices.MasterTableView.DataKeyValues[row.ItemIndex]["PreInvoiceID"]).ToString()));
                        Regenerate.Add(int.Parse((gvPreInvoices.MasterTableView.DataKeyValues[row.ItemIndex]["InvoiceGenerationParametersID"]).ToString()));
                    }
                }

                #endregion
            }

            try
            {
                // Kick off the workflow (if there is anything to do).
                if (approveAndPost.Count + approve.Count + regenerate.Count + reject.Count > 0)
                {
                    LastActionDate = DateTime.Now;

                    List<Contracts.DataContracts.NotificationParty> notificationParties = new List<Contracts.DataContracts.NotificationParty>();

                    GenerateInvoiceClient gic = new GenerateInvoiceClient("Orchestrator.InvoiceService");
                    gic.VerifyInvoices(approveAndPost.ToArray(), approve.ToArray(), regenerate.ToArray(), reject.ToArray(), notificationParties.ToArray(), ((Entities.CustomPrincipal)Page.User).UserName);

                    if (ProcessActions != null)
                        ProcessActions(sender, e);

                    //gvPreInvoices.Rebind();
                }
            }
            catch (System.ServiceModel.EndpointNotFoundException exc)
            {
                // Not possible to send message to workflow host - send email to support.
                //Utilities.SendSupportEmailHelper("GenerateInvoiceClient.VerifyInvoices(int[], int[], int[], int[], Orchestrator.Entities.NotificationParty[], string)", exc);
                // Redirect user to an appropriate page.
                //Server.Transfer("../../OfflineProcessInitiationFailed.aspx");
            }
        }

        void btnRefresh_Click(object sender, EventArgs e)
        {
            this.RefreshPreInvoices();
        }

        private void RefreshPreInvoices()
        {
            Facade.IWorkflowPreInvoice facPreInvoice = new Facade.PreInvoice();
            this.DsPreInvoices = facPreInvoice.GetAllWithDetails();

            ReloadCreatedByFilters();

            gvPreInvoices.Rebind();
            //Facade.IWorkflowPreInvoice facPreInvoice = new Facade.PreInvoice();
            //this.DsPreInvoices = facPreInvoice.GetAllWithDetails();
            //this.gvPreInvoices.DataSource = this.DsPreInvoices;
            //this.gvPreInvoices.DataBind();
        }

        private void ReloadCreatedByFilters()
        {
            foreach (DataRow item in DsPreInvoices.Tables[0].Rows)
            {
                var listItem = new ListItem(item.Field<string>(26));
                listItem.Selected = true;

                if (!cblCreatedBy.Items.Contains(listItem))
                    cblCreatedBy.Items.Add(listItem);
            }
        }

        #endregion

        #region Grid Events

        void gvPreInvoices_DetailTableDataBind(object source, GridDetailTableDataBindEventArgs e)
        {
            GridDataItem parentItem = e.DetailTableView.ParentItem as GridDataItem;
            if (parentItem.Edit)
                return;

            Facade.IPreInvoice facPreInvoice = new Facade.PreInvoice();
            e.DetailTableView.DataSource = facPreInvoice.GetItemsForPreInvoice(int.Parse(parentItem.GetDataKeyValue("PreInvoiceID").ToString()));
        }

        void gvPreInvoices_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            string tableName = e.Item.OwnerTableView.Name;

            if (tableName == PreInvoiceTableName)
            {
                #region Pre Invoice Table
                if (e.Item.ItemType == Telerik.Web.UI.GridItemType.Item || e.Item.ItemType == Telerik.Web.UI.GridItemType.AlternatingItem || e.Item.ItemType == GridItemType.EditItem)
                {
                    DataRowView drv = (DataRowView)e.Item.DataItem;

                    RadDateInput rdiInvoiceDate = e.Item.FindControl("rdiInvoiceDate") as RadDateInput;

                    CheckBox chkPreInvoice = e.Item.FindControl("chkPreInvoice") as CheckBox;
                    if (chkPreInvoice != null)
                        chkPreInvoice.Attributes.Add("PreInvoiceId", drv["PreInvoiceId"].ToString());

                    RadTextBox rtClientReference = e.Item.FindControl("rtClientReference") as RadTextBox;
                    RadTextBox rtPurchaseOrderReference = e.Item.FindControl("rtPurchaseOrderReference") as RadTextBox;

                    HtmlInputRadioButton rdoDoNothing = e.Item.FindControl("rdoDoNothing") as HtmlInputRadioButton;
                    HtmlInputRadioButton rdoReject = e.Item.FindControl("rdoReject") as HtmlInputRadioButton;
                    HtmlInputRadioButton rdoApprove = e.Item.FindControl("rdoApprove") as HtmlInputRadioButton;
                    HtmlInputRadioButton rdoApproveAndPost = e.Item.FindControl("rdoApproveAndPost") as HtmlInputRadioButton;
                    HtmlInputCheckBox chkUseHeadedPaper = e.Item.FindControl("chkUseHeadedPaper") as HtmlInputCheckBox;

                    HiddenField hidIsDirty = e.Item.FindControl("hidIsDirty") as HiddenField;

                    DropDownList cboTaxRate = e.Item.FindControl("cboTaxRate") as DropDownList;

                    Label lblPendingChanges = e.Item.FindControl("lblPendingChanges") as Label;
                    Label lblNetAmount = e.Item.FindControl("lblNetAmount") as Label;
                    Label lblExtraAmount = e.Item.FindControl("lblExtraAmount") as Label;
                    Label lblFuelSurchargeAmount = e.Item.FindControl("lblFuelSurchargeAmount") as Label;
                    Label lblTaxAmount = e.Item.FindControl("lblTaxAmount") as Label;
                    Label lblTotalAmount = e.Item.FindControl("lblTotalAmount") as Label;

                    rdoDoNothing.Name += "_" + e.Item.ItemIndex.ToString();
                    rdoReject.Name += "_" + e.Item.ItemIndex.ToString();
                    rdoApprove.Name += "_" + e.Item.ItemIndex.ToString();
                    rdoApproveAndPost.Name += "_" + e.Item.ItemIndex.ToString();

                    rdiInvoiceDate.SelectedDate = (DateTime)drv["InvoiceDate"];

                    hidIsDirty.Value = ((bool)drv["RequiresRegeneration"]).ToString();

                    if (this.dsTaxRates == null)
                    {
                        Orchestrator.Facade.Invoice facInvoice = new Orchestrator.Facade.Invoice();
                        DataSet taxRates = facInvoice.GetTaxRates((DateTime)drv["InvoiceDate"]);
                        this.dsTaxRates = taxRates;
                    }

                    cboTaxRate.DataTextField = "Description";
                    cboTaxRate.DataValueField = "VatNo";
                    cboTaxRate.DataSource = this.dsTaxRates;
                    cboTaxRate.DataBind();
                    cboTaxRate.ClearSelection();

                    int taxRateID = 0;
                    int.TryParse(drv["TaxRateID"].ToString(), out taxRateID);
                    if (cboTaxRate.Items.FindByValue(taxRateID.ToString()) == null)
                    {
                        ListItem newItem = new ListItem("New Standard", taxRateID.ToString());
                        cboTaxRate.Items.Add(newItem);
                    }

                    cboTaxRate.Items.FindByValue(taxRateID.ToString()).Selected = true;

                    if (drv["ClientInvoiceReference"] != DBNull.Value)
                        rtClientReference.Text = (string)drv["ClientInvoiceReference"];
                    else
                        rtClientReference.Text = string.Empty;

                    if (drv["PurchaseOrderReference"] != DBNull.Value)
                        rtPurchaseOrderReference.Text = (string)drv["PurchaseOrderReference"];
                    else
                        rtPurchaseOrderReference.Text = string.Empty;

                    int invoiceGenerationParametersID = int.Parse(drv["InvoiceGenerationParametersID"].ToString());
                    if ((bool)drv["RequiresRegeneration"])
                    {
                        lblPendingChanges.Text = "Pending Changes...";
                        lblPendingChanges.Style.Remove("display");

                        rdoDoNothing.Style.Add("display", "none");
                        rdoReject.Style.Add("display", "none");
                        rdoApprove.Style.Add("display", "none");
                        rdoApproveAndPost.Style.Add("display", "none");
                    }
                    else if (this.PreInvoiceIdsSentToWorkFlow.Exists(p => p == invoiceGenerationParametersID))
                    {
                        lblPendingChanges.Text = "Processing...";
                        lblPendingChanges.Style.Remove("display");

                        rdoDoNothing.Style.Add("display", "none");
                        rdoReject.Style.Add("display", "none");
                        rdoApprove.Style.Add("display", "none");
                        rdoApproveAndPost.Style.Add("display", "none");
                        this.PreInvoiceIdsSentToWorkFlow.Remove(invoiceGenerationParametersID);
                    }
                    else
                    {
                        lblPendingChanges.Text = string.Empty;
                        lblPendingChanges.Style.Add("display", "none");

                        rdoDoNothing.Style.Remove("display");
                        rdoReject.Style.Remove("display");
                        rdoApprove.Style.Remove("display");
                        rdoApproveAndPost.Style.Remove("display");
                    }

                    int lcID = (int)drv["LCID"];
                    CultureInfo culture = new CultureInfo(lcID);

                    lblNetAmount.Text = ((decimal)drv["ForeignNetAmount"]).ToString("C", culture);
                    lblExtraAmount.Text = ((decimal)drv["ForeignExtraAmount"]).ToString("C", culture);
                    lblFuelSurchargeAmount.Text = ((decimal)drv["ForeignFuelSurchargeAmount"]).ToString("C", culture);
                    lblTaxAmount.Text = ((decimal)drv["ForeignTaxAmount"]).ToString("C", culture);
                    lblTotalAmount.Text = ((decimal)drv["ForeignTotalAmount"]).ToString("C", culture);

                    chkUseHeadedPaper.Checked = Orchestrator.Globals.Configuration.UseHeadedPaper;
                }
                else if (e.Item.ItemType == Telerik.Web.UI.GridItemType.Header)
                {
                    RadioButton rdoDoNothing = e.Item.FindControl("rdoDoNothing") as RadioButton;
                    //RadioButton rdoReject = e.Item.FindControl("rdoReject") as RadioButton;
                    RadioButton rdoApprove = e.Item.FindControl("rdoApprove") as RadioButton;
                    RadioButton rdoApproveAndPost = e.Item.FindControl("rdoApproveAndPost") as RadioButton;

                    rdoDoNothing.GroupName += "_Header";
                    //rdoReject.GroupName += "_Header";
                    rdoApprove.GroupName += "_Header";
                    rdoApproveAndPost.GroupName += "_Header";

                    AddReaction(rdoDoNothing, "spnDoNothing");
                    //AddReaction(rdoReject, "spnReject");
                    AddReaction(rdoApprove, "spnApprove");
                    AddReaction(rdoApproveAndPost, "spnApproveAndPost");
                }
                #endregion
            }
            else if (tableName == PreInvoiceItemTableName)
            {
                #region Detail Table
                if (e.Item.ItemType == Telerik.Web.UI.GridItemType.Item || e.Item.ItemType == Telerik.Web.UI.GridItemType.AlternatingItem || e.Item.ItemType == GridItemType.EditItem)
                {
                    HiddenField hidOldRate = e.Item.FindControl("hidOldRate") as HiddenField;
                    TextBox txtRate = e.Item.FindControl("txtRate") as TextBox;
                    Label lblPalletSpaces = e.Item.FindControl("lblPalletSpaces") as Label;
                    HtmlAnchor lnkOrderId = e.Item.FindControl("lnkOrderId") as HtmlAnchor;
                    Label lblJobSubcontractId = e.Item.FindControl("lblJobSubcontractId") as Label;

                    DataRowView drv = e.Item.DataItem as DataRowView;

                    int itemID = (int)drv["ItemID"];
                    txtRate.Attributes.Add("ItemID", itemID.ToString());

                    if (Convert.ToBoolean(drv["IsOrder"]) == true)
                    {
                        lblJobSubcontractId.Visible = false;
                        lnkOrderId.Visible = true;
                        lnkOrderId.InnerHtml = itemID.ToString();
                        lnkOrderId.HRef = "javascript:viewOrder(" + itemID.ToString() + ");";
                    }
                    else
                    {
                        lblJobSubcontractId.Visible = true;
                        lnkOrderId.Visible = false;
                        lblJobSubcontractId.Text = itemID.ToString();
                    }

                    int lcID = (int)drv["LCID"];
                    CultureInfo culture = new CultureInfo(lcID);
                    int rateDecimalPlaces = (int)drv["RateDecimalPlaces"];
                    if (rateDecimalPlaces != 2)
                        culture.NumberFormat.CurrencyDecimalDigits = rateDecimalPlaces;

                    txtRate.Text = ((decimal)drv["ForeignRate"]).ToString("C", culture);

                    lblPalletSpaces.Text = ((decimal)drv["PalletSpaces"]).ToString("F2");
                    hidOldRate.Value = txtRate.Text;
                }
                #endregion
            }
        }

        void gvPreInvoices_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            gvPreInvoices.DataSource = null;

            DataSet dsCopy = this.DsPreInvoices.Copy();

            DataRow[] rowsToRemove = dsCopy.Tables[0].Select(string.Format("CreateUserId <> '{0}'", Page.User.Identity.Name));

            if (rblFilterInvoices.SelectedValue == "0")
            {
                chkSelectAllCreatedBy.Disabled = true;
                this.cblCreatedBy.Enabled = false;

                // remove the pre invoices that are not mine.
                foreach (var row in rowsToRemove)
                {
                    dsCopy.Tables[0].Rows.Remove(row);
                }

                dsCopy.Tables[0].AcceptChanges();
                gvPreInvoices.DataSource = dsCopy;
            }
            else
            {
                var checkedUsers = new List<ListItem>();
                foreach (ListItem item in this.cblCreatedBy.Items)
                {
                    if (item.Selected)
                        checkedUsers.Add(item);
                }


                var filteredOutRows = dsCopy.Tables[0].Rows.Cast<DataRow>().Where(dr => !checkedUsers.Any(u => u.Text == dr.Field<string>(26))).ToList();

                foreach (var row in filteredOutRows)
                {
                    dsCopy.Tables[0].Rows.Remove(row);
                }

                chkSelectAllCreatedBy.Disabled = false;
                this.cblCreatedBy.Enabled = true;
                // apply filtering if necessary
                gvPreInvoices.DataSource = dsCopy;
            }

            rblFilterInvoices.Items[0].Text = string.Format("Mine Only ({0})", DsPreInvoices.Tables[0].Rows.Count - rowsToRemove.Length);
            rblFilterInvoices.Items[1].Text = string.Format("All ({0})", DsPreInvoices.Tables[0].Rows.Count);
        }

        void gvPreInvoices_PreRender(object sender, EventArgs e)
        {
            if (StayOpen >= 0)
            {
                gvPreInvoices.Items[StayOpen].Expanded = true;
                StayOpen = -1;
            }
        }

        #endregion

        private void SetUseHeadedPaperParameter(int preInvoiceID, bool useHeadedPaper)
        {
            Facade.IPreInvoice facPreInvoice = new Facade.PreInvoice();
            facPreInvoice.UpdateUseHeadedPaper(preInvoiceID, useHeadedPaper, ((Entities.CustomPrincipal)Page.User).UserName);
        }

        void rblFilterInvoices_SelectedIndexChanged(object sender, EventArgs e)
        {
            gvPreInvoices.Rebind();
        }
        #endregion
    }
}
