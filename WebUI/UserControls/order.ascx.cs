using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using System.Collections.Generic;
using System.Net.Mail;
using System.Net;
using Orchestrator;
using Orchestrator.Facade;
using Orchestrator.WebUI;
using Orchestrator.Entities;
using System.Collections.Specialized;

using System.Data.SqlClient;
using Point = Orchestrator.Entities.Point;

using Telerik.Web.UI;
using System.Text;

using System.IO;
using System.Transactions;
using System.Drawing;

namespace Orchestrator.UserControls
{
    public partial class Order : System.Web.UI.UserControl
    {
        #region Nested Classes

        public class RedeliveryRow
        {
            public int JobId { get; set; }
            public string PointDescription { get; set; }
            public int PointId { get; set; }
            public DateTime NewBookedDateTime { get; set; }
            public string Reason { get; set; }
            public string RedeliveryIdAndOrderId { get; set; }
            public int RedeliveryId { get; set; }
            public int OrderId { get; set; }
            public int PartialDeliveryCompletedOrderId { get; set; }
            public int NewInstructionId { get; set; }
            public string CreateUser { get; set; }
            public DateTime CreateDateTime { get; set; }
        }

        public class RefusalRow
        {
            public int RefusalId { get; set; }
            public string ProductName { get; set; }
            public string ProductCode { get; set; }
            public string RefusalReceiptNumber { get; set; }
            public int QuantityOrdered { get; set; }
            public int QuantityRefused { get; set; }
            public string TimeFrame { get; set; }
            public string PackSize { get; set; }
            public string RefusalNotes { get; set; }
            public int? RedeliveryId { get; set; }
            public int OriginalOrderId { get; set; }
            public int? NewOrderId { get; set; }
            public int JobId { get; set; }
            public int InstructionId { get; set; }
            public string RedeliveryIdAndOrderId { get; set; }
            public string RefusalLocation { get; set; }
            public string RefusalType { get; set; }
        }

        #endregion

        #region Constants

        private const string _ogid_QS = "ogid";

        #endregion

        #region Private Fields

        bool _isUpdate = false, _deliveryPointUpdated = false, _collectionPointUpdated = false;
        private CultureInfo _currentCulture = null;
        Orchestrator.Entities.OrganisationReferenceCollection _clientReferences = null;
        Orchestrator.Entities.Order _order = null;
        private DataSet dsOrders = null;
        DataSet _dsBusinessType = null;
        private int? _exchangeRateID = -1;
        DataSet dsExtras = null;
        private List<DateTime> InvalidDates = null;
        bool _dlgAddUpdateRefusal_DialogCallBack_HasFired = false;

        #endregion

        #region Page Properties

        protected int OrderID
        {
            get
            {
                int _orderID = 0;
                if (this.ViewState["_orderID"] != null)
                    _orderID = (int)this.ViewState["_orderID"];

                if (_orderID == 0 && !string.IsNullOrEmpty(Request.QueryString["oID"]))
                    _orderID = int.Parse(Request.QueryString["oID"]);

                return _orderID;
            }
            set { this.ViewState["_orderID"] = value; }
        }

        protected int OrderInstructionID
        {
            get
            {
                //New orders will always be Collect and Deliver unless they
                //have been created by the Palletforce import in which case they may also be Collect or Deliver
                if (this.SavedOrder != null)
                    return this.SavedOrder.OrderInstructionID.Value;
                else
                    return 3; //Collect & Deliver
            }

        }

        protected Orchestrator.Entities.Order SavedOrder
        {
            get { return (Orchestrator.Entities.Order)this.ViewState["_order"]; }
            set { this.ViewState["_order"] = value; }
        }

        protected bool IsUpdate
        {
            get { return this.ViewState["_isUpdate"] == null ? false : (bool)this.ViewState["_isUpdate"]; }
            set
            {
                this.ViewState["_isUpdate"] = value;
                hidPageIsUpdate.Value = value.ToString();
            }
        }

        private bool IsClientUser
        {
            get { return (bool)this.ViewState["_isClientUser"]; }
            set { this.ViewState["_isClientUser"] = value; }
        }

        private bool DeliveryPointUpdated
        {
            get { return _deliveryPointUpdated; }
            set { _deliveryPointUpdated = value; }
        }

        private bool CollectionPointUpdated
        {
            get { return _collectionPointUpdated; }
            set { _collectionPointUpdated = value; }
        }

        private DataSet BusinessTypeDataSet
        {
            get
            {
                if (_dsBusinessType == null)
                {
                    Facade.IBusinessType facBusinessType = new Facade.BusinessType();
                    _dsBusinessType = facBusinessType.GetAll();

                    //Set the Primary Key on the DataSet to allow Find to be used
                    _dsBusinessType.Tables[0].PrimaryKey = new DataColumn[] { _dsBusinessType.Tables[0].Columns[0] };
                }

                return _dsBusinessType;
            }
        }

        protected CultureInfo CurrentCulture
        {
            get
            {
                if (_currentCulture == null)
                {
                    CultureInfo culture = null;
                    try
                    {
                        Facade.IOrganisation facOrg = new Orchestrator.Facade.Organisation();
                        culture = new CultureInfo(facOrg.GetCultureForOrganisation(int.Parse(this.cboClient.SelectedValue)));
                    }
                    catch { }

                    if (culture != null)
                        _currentCulture = culture;
                }

                return _currentCulture == null ? new CultureInfo(Orchestrator.Globals.Configuration.NativeCulture) : _currentCulture;
            }
            set { _currentCulture = value; }
        }

        protected int ExchangeRateForExtra
        {
            get { return _exchangeRateID == null ? -1 : (int)_exchangeRateID; }
        }

        protected int? ExchangeRateID
        {
            get { return _exchangeRateID; }
            set { _exchangeRateID = value; }
        }

        protected bool ConfirmZeroRate
        {
            get { return Orchestrator.Globals.Configuration.ConfirmZeroRate; }
        }

        public string SignatureImageBaseUrl
        {
            get { return WebUI.Utilities.GetSignatureImageBaseUri().AbsoluteUri; }
        }

        #endregion

        #region OrderGroup

        private Entities.OrderGroup _orderGroup = null;
        protected OrderGroup OrderGroup
        {
            get
            {
                if (_orderGroup == null)
                    LoadOrderGroup();
                return _orderGroup;

            }
            set { _orderGroup = value; }
        }

        /// <summary>
        /// Checks the querystring for a valid order group id and, if one
        /// is found, attempts to load the relevant order group.
        /// </summary>
        /// <returns>True if the order group was loaded, False otherwise.</returns>
        private bool LoadOrderGroup()
        {
            Facade.IOrderGroup facOrderGroup = new Facade.Order();
            if (this.SavedOrder.OrderGroupID > 0)
                this.OrderGroup = facOrderGroup.Get(this.SavedOrder.OrderGroupID);
            return OrderGroup != null;

        }

        /// <summary>
        /// Reloads the order group and causing the page to be rebound.
        /// </summary>
        private void RebindPage()
        {
            LoadOrderGroup();
            ConfigureDisplay();
        }

        /// <summary>
        /// Prepares the page for first time viewing.
        /// </summary>
        private void ConfigureDisplay()
        {
            CultureInfo culture = new CultureInfo(OrderGroup.LCID);

            // Populate the basic order group information.
            lblRate.Text = OrderGroup.ForeignRate.ToString("C", culture);
            lblRate.Visible = true;
            lnkEditRate.Visible = true;
            cboGroupedPlanning.ClearSelection();
            ListItem selected = cboGroupedPlanning.Items.FindByValue(OrderGroup.GroupedPlanning.ToString().ToLower());
            if (selected != null) selected.Selected = true;
            lblOrderCount.Text = OrderGroup.Orders.Count.ToString();

            // If the group is being invoiced, or has been invoiced do not allow the group to be changed.
            Facade.IOrder facOrder = new Facade.Order();
            bool IsBeingInvoiced = facOrder.IsOrderBeingInvoiced(OrderGroup.OrderIDs());
            lnkEditRate.Enabled = !IsBeingInvoiced;
            cboGroupedPlanning.Enabled = !IsBeingInvoiced;

            this.txtPalletForceNotes1.ReadOnly = !Orchestrator.Globals.Configuration.PalletForceSpecialInstructionsEnabled;

        }

        /// <summary>
        /// Display the infringments returned from the facade call.
        /// </summary>
        /// <param name="result">The result returned from the facade call containing the infringements.</param>
        private void DisplayInfringments(FacadeResult result)
        {
            // The update failed, display infringements.
            ruleInfringements.Infringements = result.Infringements;
            ruleInfringements.DisplayInfringments();
            ruleInfringements.Visible = true;
        }

        void cboGroupedPlanning_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (OrderGroup != null)
            {
                OrderGroup.GroupedPlanning = bool.Parse(cboGroupedPlanning.SelectedValue);

                Facade.IOrderGroup facOrderGroup = new Facade.Order();
                FacadeResult result = facOrderGroup.Update(OrderGroup, this.Page.User.Identity.Name);

                if (result.Success)
                {
                    RebindPage();
                }
                else
                {
                    DisplayInfringments(result);
                }
            }
        }

        #endregion

        #region OnInit/Page Load

        // This method has been overridden to fix an obscure event validation error
        // that occurs when the AddOrder.aspx page has been left open for about
        // 10 mins and is then used by the user. The error indicates that a control needs
        // to be registered for event validation.
        protected override void Render(HtmlTextWriter writer)
        {
            foreach (Control c in this.Controls)
                this.Page.ClientScript.RegisterForEventValidation(c.UniqueID);
            base.Render(writer);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.dlgOrderGroup.DialogCallBack += new EventHandler(dlgOrderGroup_DialogCallBack);
            this.dlgExtraWindow.DialogCallBack += new EventHandler(dlgExtraWindow_DialogCallBack);
            this.dlgDocumentWizard.DialogCallBack += new EventHandler(dlgDocumentWizard_DialogCallBack);
            this.dlgAddUpdateRefusal.DialogCallBack += new EventHandler(dlgAddUpdateRefusal_DialogCallBack);

            this.lnkUpdateConfidentialComments.Click += new EventHandler(lnkUpdateConfidentialComments_Click);
            this.lnkCreateGroup.Click += new EventHandler(lnkCreateGroup_Click);

            this.cboGroupedPlanning.SelectedIndexChanged += new EventHandler(cboGroupedPlanning_SelectedIndexChanged);
            this.cboClient.SelectedIndexChanged += new Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventHandler(cboClient_SelectedIndexChanged);

            this.RadGridForSubby.ItemDataBound += new Telerik.Web.UI.GridItemEventHandler(RadGridForSubby_ItemDataBound);
            this.RadGridForSubby.NeedDataSource += new GridNeedDataSourceEventHandler(RadGridForSubby_NeedDataSource);

            this.grdAttemptedDeliveries.NeedDataSource += new GridNeedDataSourceEventHandler(grdAttemptedDeliveries_NeedDataSource);
            this.grdExtras.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdExtras_NeedDataSource);
            this.grdExtras.ItemDataBound += new Telerik.Web.UI.GridItemEventHandler(grdExtras_ItemDataBound);
            this.grdNetworkExtras.NeedDataSource += new GridNeedDataSourceEventHandler(grdNetworkExtras_NeedDataSource);
            this.grdNetworkExtras.ItemDataBound += new Telerik.Web.UI.GridItemEventHandler(grdNetworkExtras_ItemDataBound);
            this.grdOrders.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdOrders_NeedDataSource);
            this.grdOrders.ItemDataBound += new GridItemEventHandler(grdOrders_ItemDataBound);
            this.grdOrders.DeleteCommand += new GridCommandEventHandler(grdOrders_DeleteCommand);
            this.grdRefusalsOriginalOrder.NeedDataSource += new GridNeedDataSourceEventHandler(grdRefusalsOriginalOrder_NeedDataSource);
            this.grdRefusalsOriginalOrder.ItemDataBound += new GridItemEventHandler(grdRefusalsOriginalOrder_ItemDataBound);
            this.grdRefusalsOriginalOrder.PreRender += new EventHandler(rptRefusalsOriginalOrder_PreRender);
            this.grdRefusalsNewOrder.NeedDataSource += new GridNeedDataSourceEventHandler(grdRefusalsNewOrder_NeedDataSource);
            this.grdRefusalsNewOrder.ItemDataBound += new GridItemEventHandler(grdRefusalsNewOrder_ItemDataBound);
            this.grdRefusalsNewOrder.PreRender += new EventHandler(grdRefusalsNewOrder_PreRender);

            this.pnlAttemptedDeliveries.PreRender += new EventHandler(pnlAttemptedDeliveries_PreRender);

            this.btnUnFlagForInvoicing.Click += new EventHandler(btnUnFlagForInvoicing_Click);
            this.btnMakeInvoiceable.Click += new EventHandler(btnMakeInvoiceable_Click);
            this.btnCreateDeliveryNote.Click += new EventHandler(btnCreateDeliveryNote_Click);
            this.btnCreateDeliveryNoteTop.Click += new EventHandler(btnCreateDeliveryNote_Click);
            this.btnPIL.Click += new EventHandler(btnPIL_Click);
            this.btnPILTop.Click += new EventHandler(btnPIL_Click);
            this.btnSubmit.Click += new EventHandler(btnSubmit_Click);
            this.btnSubmitTop.Click += new EventHandler(btnSubmit_Click);
            this.btnContinue.Click += new EventHandler(btnContinue_Click);
            this.btnAddGroupedOrder.Click += new EventHandler(btnAddGroupedOrder_Click);
            this.btnAddGroupedOrderTop.Click += new EventHandler(btnAddGroupedOrder_Click);
            this.btnAddAndReset.Click += new EventHandler(btnAddAndReset_Click);
            this.btnAddAndResetTop.Click += new EventHandler(btnAddAndReset_Click);
            this.btnPodLabel.Click += new EventHandler(btnPodLabel_Click);
            this.btnPodLabelTop.Click += new EventHandler(btnPodLabel_Click);
            this.btnCopy.Click += new EventHandler(btnCopy_Click);
            this.btnCopyTop.Click += new EventHandler(btnCopy_Click);
            this.btnCreateRun.Click += new EventHandler(btnCreateRun_Click);
            this.btnCreateRunTop.Click += new EventHandler(btnCreateRun_Click);

            this.repReferences.ItemDataBound += new RepeaterItemEventHandler(repReferences_ItemDataBound);

            this.cfvClient.ServerValidate += new ServerValidateEventHandler(cfvClient_ServerValidate);
            this.cfvDelivery.ServerValidate += new ServerValidateEventHandler(cfvDelivery_ServerValidate);

            this.btnSetAsInvoiced.Click += new EventHandler(btnSetAsInvoiced_Click);
            this.btnSetAsInvoicedTop.Click += new EventHandler(btnSetAsInvoiced_Click);
        }

        protected void btnSetAsInvoiced_Click(object sender, EventArgs e)
        {
            Facade.IOrder facOrder = new Facade.Order();
            facOrder.Update(this.OrderID, eOrderStatus.Invoiced, this.Page.User.Identity.Name);
            hidShowConfirmForOrderAfterDays.Value = Orchestrator.Globals.Configuration.ShowConfirmForOrderAfterDays.ToString();

            Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.AddEditOrder);
            RestictClientChoices();
            OrderID = 0;

            int orderID = -1;
            int.TryParse(Request.QueryString["oID"], out orderID);
            this.SavedOrder = null;

            // Refresh the order info.
            PopulateStaticControls();
            LoadOrder();
            ucCollectionPoint.ControlToFocusOnPostBack = dteCollectionFromDate.ClientID + "_t";
            ucDeliveryPoint.ControlToFocusOnPostBack = dteDeliveryByDate.ClientID + "_t";

            this.ApplyAttributesToBusinessTypes();

            litSetFocus.Controls.Clear();
            litSetFocus.Text = string.Empty;

            (this.Page as Orchestrator.Base.BasePage).ReturnValue = orderID.ToString();
        }

        void grdRefusalsNewOrder_PreRender(object sender, EventArgs e)
        {
            divRefusalsNewOrder.Visible = grdRefusalsNewOrder.Items.Count > 0;
        }

        void grdRefusalsNewOrder_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.DataItem != null)
            {
                RefusalRow refusal = null;
                string queryString = string.Empty;

                if (e.Item.DataItem.GetType() == typeof(RefusalRow))
                    refusal = (RefusalRow)e.Item.DataItem;

                if (refusal != null && (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem))
                {
                    queryString = string.Empty;
                    HtmlAnchor hypEditRefusal = (HtmlAnchor)e.Item.FindControl("hypGoodsId");
                    queryString = string.Format("RefusalId={0}&OrderId={1}&JobId={2}&RedeliveryId={3}", refusal.RefusalId, refusal.OriginalOrderId, refusal.JobId, refusal.RedeliveryId);
                    hypEditRefusal.HRef = string.Format("javascript:{0}", this.dlgAddUpdateRefusal.GetOpenDialogScript(queryString));

                    HtmlAnchor hypJobId = (HtmlAnchor)e.Item.FindControl("hypJobId");
                    hypJobId.HRef = "javascript:ViewJobDetails(" + refusal.JobId.ToString() + ");";

                    if (refusal.OriginalOrderId > -1)
                    {
                        HtmlAnchor hypOrderId = (HtmlAnchor)e.Item.FindControl("hypOrderId");
                        hypOrderId.HRef = "javascript:viewOrderProfile(" + refusal.OriginalOrderId.ToString() + ");";
                        Label lblOrderId = (Label)e.Item.FindControl("lblOrderId");
                        lblOrderId.Text = refusal.OriginalOrderId.ToString();
                    }
                }
            }
        }

        void grdRefusalsNewOrder_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            this.grdRefusalsNewOrder.DataSource = this.RefusalsForNewOrder();
        }

        void grdRefusalsOriginalOrder_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.DataItem != null)
            {
                RefusalRow refusal = null;
                string queryString = string.Empty;

                if (e.Item.DataItem.GetType() == typeof(RefusalRow))
                    refusal = (RefusalRow)e.Item.DataItem;

                if (refusal != null && (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem))
                {
                    queryString = string.Empty;
                    HtmlAnchor hypEditRefusal = (HtmlAnchor)e.Item.FindControl("hypGoodsId");
                    queryString = string.Format("RefusalId={0}&OrderId={1}&JobId={2}&RedeliveryId={3}", refusal.RefusalId, refusal.OriginalOrderId, refusal.JobId, refusal.RedeliveryId);
                    hypEditRefusal.HRef = string.Format("javascript:{0}", this.dlgAddUpdateRefusal.GetOpenDialogScript(queryString));

                    HtmlAnchor hypJobId = (HtmlAnchor)e.Item.FindControl("hypJobId");
                    hypJobId.HRef = "javascript:ViewJobDetails(" + refusal.JobId.ToString() + ");";

                    if (refusal.NewOrderId.HasValue && refusal.NewOrderId.Value > -1)
                    {
                        HtmlAnchor hypOrderId = (HtmlAnchor)e.Item.FindControl("hypOrderId");
                        hypOrderId.HRef = "javascript:viewOrderProfile(" + refusal.NewOrderId.ToString() + ");";
                        Label lblOrderId = (Label)e.Item.FindControl("lblOrderId");
                        lblOrderId.Text = refusal.NewOrderId.ToString();
                    }
                }
            }
        }

        void grdRefusalsOriginalOrder_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            this.grdRefusalsOriginalOrder.DataSource = this.RefusalsForOriginalOrder();
        }

        void rptRefusalsOriginalOrder_PreRender(object sender, EventArgs e)
        {
            divRefusalsOriginalOrder.Visible = grdRefusalsOriginalOrder.Items.Count > 0;
        }

        void grdAttemptedDeliveries_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            DataSet ds = null;

            if (this.OrderID > 0)
            {
                Orchestrator.Facade.IRedelivery facRedelivery = new Orchestrator.Facade.Redelivery();
                ds = facRedelivery.GetSummaryDataForOrderIDs(this.OrderID.ToString());
            }

            grdAttemptedDeliveries.DataSource = ds;
        }

        void pnlAttemptedDeliveries_PreRender(object sender, EventArgs e)
        {
            pnlAttemptedDeliveries.Visible = grdAttemptedDeliveries.Items.Count > 0;
        }

        void rptOversShorts_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                RefusalRow refusal = (RefusalRow)e.Item.DataItem;
                Button btnEditRefusal = (Button)e.Item.FindControl("btnEditRefusal");
                Button btnRemoveRefusal = (Button)e.Item.FindControl("btnRemoveRefusal");

                string queryString = string.Format("RefusalId={0}&OrderId={1}&JobId={2}", refusal.RefusalId, refusal.OriginalOrderId, refusal.JobId);
                btnEditRefusal.OnClientClick = string.Format("javascript:{0}", this.dlgAddUpdateRefusal.GetOpenDialogScript(queryString));

                btnRemoveRefusal.CommandArgument = string.Format("{0},{1}", refusal.RefusalId, refusal.InstructionId);
            }
        }

        protected void btnRemoveRefusal_Command(object sender, CommandEventArgs e)
        {
            if (e.CommandName == "RemoveRefusal")
            {
                if (e.CommandArgument != null)
                {
                    string[] vals = e.CommandArgument.ToString().Split(",".ToCharArray());
                    int refusalId = int.Parse(vals[0]);
                    //int instructionId = int.Parse(vals[1]);

                    Facade.GoodsRefusal facGR = new Orchestrator.Facade.GoodsRefusal();
                    facGR.Delete(refusalId, Page.User.Identity.Name);

                    //this.rptOversShorts.DataSource = OversShortDataSource();
                    //this.rptOversShorts.DataBind();
                }
            }
        }

        void rptAssociatedTurnAways_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                HtmlAnchor hypAssociatedOrderIds = (HtmlAnchor)e.Item.FindControl("hypAssociatedOrderIds");
                int associatedOrderId = (int)e.Item.DataItem;
                string queryString = string.Format("oid={0}", associatedOrderId.ToString());
                hypAssociatedOrderIds.HRef = string.Format("javascript:{0}", this.dlgOrder.GetOpenDialogScript(queryString));
                hypAssociatedOrderIds.InnerText = associatedOrderId.ToString();
            }
        }

        protected void dlgAddUpdateRefusal_DialogCallBack(object sender, EventArgs e)
        {
            this.grdRefusalsNewOrder.Rebind();
            this.grdRefusalsOriginalOrder.Rebind();
        }

        void dlgOrderGroup_DialogCallBack(object sender, EventArgs e)
        {
            hidShowConfirmForOrderAfterDays.Value = Orchestrator.Globals.Configuration.ShowConfirmForOrderAfterDays.ToString();

            Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.AddEditOrder);
            RestictClientChoices();
            OrderID = 0;

            int orderID = -1;
            int.TryParse(Request.QueryString["oID"], out orderID);

            if (this.SavedOrder != null)
            {
                // Refresh the order info.
                Facade.IOrder facOrder = new Orchestrator.Facade.Order();
                this.SavedOrder = facOrder.GetForOrderID(this.SavedOrder.OrderID);

                PopulateStaticControls();
                LoadOrder();
                
                ucCollectionPoint.ControlToFocusOnPostBack = dteCollectionFromDate.ClientID + "_t";
                ucDeliveryPoint.ControlToFocusOnPostBack = dteDeliveryByDate.ClientID + "_t";
            }

            this.ApplyAttributesToBusinessTypes();

            litSetFocus.Controls.Clear();
            litSetFocus.Text = string.Empty;

            (this.Page as Orchestrator.Base.BasePage).ReturnValue = orderID.ToString();
        }

        void dlgDocumentWizard_DialogCallBack(object sender, EventArgs e)
        {
            hidShowConfirmForOrderAfterDays.Value = Orchestrator.Globals.Configuration.ShowConfirmForOrderAfterDays.ToString();

            Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.AddEditOrder);
            RestictClientChoices();
            OrderID = 0;

            int orderID = -1;
            int.TryParse(Request.QueryString["oID"], out orderID);

            if (this.SavedOrder != null)
            {
                // Refresh the order info.
                PopulateStaticControls();
                LoadOrder();
                ucCollectionPoint.ControlToFocusOnPostBack = dteCollectionFromDate.ClientID + "_t";
                ucDeliveryPoint.ControlToFocusOnPostBack = dteDeliveryByDate.ClientID + "_t";
            }
            this.ApplyAttributesToBusinessTypes();

            litSetFocus.Controls.Clear();
            litSetFocus.Text = string.Empty;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            hidShowConfirmForOrderAfterDays.Value = Orchestrator.Globals.Configuration.ShowConfirmForOrderAfterDays.ToString();
            Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.AddEditOrder);
            RestictClientChoices();

            if (!IsPostBack)
            {
                this.OrderID = 0;
                int orderID = -1;
                int.TryParse(Request.QueryString["oID"], out orderID);
                if (orderID > 0)
                {
                    this.OrderID = orderID;
                    Facade.IOrder facOrder = new Orchestrator.Facade.Order();
                    this.SavedOrder = facOrder.GetForOrderID(this.OrderID);
                    this.IsUpdate = true;
                }

                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetAllowResponseInBrowserHistory(false);

                hidNetworkIdentityId.Value = Orchestrator.Globals.Configuration.PalletNetworkID.ToString();

                PopulateStaticControls();
                LoadOrder();
                ucCollectionPoint.ControlToFocusOnPostBack = dteCollectionFromDate.ClientID + "_t";
                ucDeliveryPoint.ControlToFocusOnPostBack = dteDeliveryByDate.ClientID + "_t";

                cboClient.Focus();

                OrderTabs.SelectedIndex = tabviewRevenue.Index;
                tabviewRevenue.Selected = true;

                if (this.SavedOrder != null)
                {
                    // Set the palletforce fields tab visible if the business type is IsPalletNetwork.
                    Facade.BusinessType facBusType = new Orchestrator.Facade.BusinessType();
                    DataSet dsBusinessTypes = facBusType.GetAll();
                    bool isNetwork = false;
                    foreach (DataRow row in dsBusinessTypes.Tables[0].Rows)
                    {
                        if (Convert.ToInt32(row["BusinessTypeID"]) == this.SavedOrder.BusinessTypeID)
                            if (Convert.ToBoolean(row["IsPalletNetwork"]) == true)
                            {
                                isNetwork = true;
                                break;
                            }
                    }

                    if (isNetwork || (Orchestrator.Globals.Configuration.PalletNetworkID == this.SavedOrder.CustomerIdentityID))
                    {
                        tabviewPalletForce.Selected = true;
                        tabviewPalletForce.Visible = true;
                        tabPalletForce.Visible = true;
                        tabviewPalletForce.Selected = true;
                        OrderTabs.SelectedIndex = tabviewPalletForce.Index;
                    }
                }

                /*
                 * Tom Farrow 24/03/2010
                 * 
                 * ---------------------------------------- 
                 * CHECK FOR THE EXISTENCE OF A TARIFF RATE
                 * ---------------------------------------- 
                 * This check is required in the scenario where tariff tables are created
                 * after orders are created. When orders are initally created without
                 * a tariff table present, they cannot be auto-rated and cannot be marked as rate
                 * overridden. If a tariff table is subsequently created with rates that would
                 * apply to those existing orders, those orders would be in an inconsistent state.
                 * 
                 * Thus, upon opening these orders a check needs to be made to see if a tariff-rate exists.
                 * If a rate exists and the order has a DIFFERENT rate, then the order should be
                 * marked as rate overridden. If a rate exists but it is the same as the orders 
                 * rate, then the order should be marked as auto-rated.
                             
                 */

                /* START OF CHECK */

                if (SavedOrder != null && SavedOrder.OrderGroupID < 1)
                {
                    EF.VigoOrder vigoOrder = EF.DataContext.Current.VigoOrderSet.Include("VigoOrderExtras.ExtraType").FirstOrDefault(v => v.OrderId == this.SavedOrder.OrderID);
                    CheckRateInformation(vigoOrder, true);
                }

                /* END OF CHECK */

            }
            else
            {
                if (this.SavedOrder != null && this.OrderID != this.SavedOrder.OrderID)
                {
                    // Refresh the order info.
                    PopulateStaticControls();
                    LoadOrder();
                    ucCollectionPoint.ControlToFocusOnPostBack = dteCollectionFromDate.ClientID + "_t";
                    ucDeliveryPoint.ControlToFocusOnPostBack = dteDeliveryByDate.ClientID + "_t";
                }
            }

            this.ApplyAttributesToBusinessTypes();

            litSetFocus.Controls.Clear();
            litSetFocus.Text = string.Empty;

            this.txtPalletForceNotes1.ReadOnly = !Orchestrator.Globals.Configuration.PalletForceSpecialInstructionsEnabled;
            this.btnSetAsInvoiced.Visible = this.btnSetAsInvoiced.Enabled = Orchestrator.Globals.Configuration.EnableSetAsInvoiced && IsUpdate;
            this.btnSetAsInvoicedTop.Visible = this.btnSetAsInvoiced.Enabled = Orchestrator.Globals.Configuration.EnableSetAsInvoiced && IsUpdate;

            if (this.txtPalletForceNotes1.ReadOnly == true)
                this.txtPalletForceNotes1.BackColor = System.Drawing.Color.LightGray;

        }

        #endregion

        #region Populate static controls

        private void PopulateStaticControls()
        {
            Label PageTitle = ((System.Web.UI.UserControl)(Page.Master)).FindControl("lblWizardTitle") as Label;

            if (PageTitle != null)
                PageTitle.Text = "Order Details";

            this.chkRequiresBookIn.Attributes.Add("onclick", "javascript:UpdateBookedIn(this);");
            this.chkBookedIn.Attributes.Add("onclick", "javascript:UpdateBookedIn(this);");

            this.lblLoadNumber.Text = Globals.Configuration.SystemLoadNumberText;
            this.lblDocketNumber.Text = Globals.Configuration.SystemDocketNumberText;

            tdAllocationLabel.Visible = tdAllocationField.Visible = WebUI.Utilities.IsAllocationEnabled();

            LoadBusinessTypes();
            LoadPalletTypes();
            LoadGoodsTypes();
            LoadServiceLevels();
            LoadExtraTypes();
            LoadInvalidDates();
            LoadDeliveryDates();
            LoadDeviationReasonCodes();
        }

        protected void LoadDeviationReasonCodes()
        {
            List<EF.DeviationReasonCode> deviationReasonCodes = EF.DataContext.Current.DeviationReasonCodes.ToList();

            cboDeviationReason.Items.Add(new ListItem(String.Empty, "-1"));

            foreach (EF.DeviationReasonCode code in deviationReasonCodes)
                cboDeviationReason.Items.Add(new ListItem(code.DeviationReason, code.DeviationReasonId.ToString(), true));

            cboDeviationReason.SelectedValue = "-1";
        }

        private void ClearScreen()
        {
            this.divDuplicateOrders.Visible = false;
            plcCancellation.Visible = false;
            //trExtras.Visible = false;
            trExtras_AddButton.Visible = false;
            trFinancial.Visible = false;
            tabFinancial.Visible = false;
            tabviewFinancial.Visible = false;
            tabAttemptedCollection.Visible = false;
            tabviewAttemptedCollection.Visible = false;
            phLegsForOrder.Visible = false;
            tabLegsForOrder.Visible = false;
            tabviewLegsForOrder.Visible = false;
            tabMwfDeliveryDetails.Visible = false;
            tabviewMwfDeliveryDetails.Visible = false;
            trOrderIdAndOrderStatus.Visible = false;
            phBookingForm.Visible = false;
            phOrderGroup.Visible = false;
            phOrderInfo.Visible = false;
            plcPOD.Visible = false;
            plcDriverCheckIn.Visible = false;
            plcOrderGroupDetail.Visible = false;
            lnkUpdateConfidentialComments.Enabled = false;
            lnkUpdateConfidentialComments.Visible = false;

            txtConfidentialComments.Height = 40;
            txtNotes.Height = 40;

            btnCreateDeliveryNote.Visible = false;
            btnCreateDeliveryNoteTop.Visible = false;
            btnCancelOrder.Visible = false;
            btnCancelOrderTop.Visible = false;
            btnPIL.Visible = false;
            btnPILTop.Visible = false;
            btnPodLabel.Visible = false;
            btnPodLabelTop.Visible = false;

            cboClient.SelectedValue = "";
            cboClient.Text = "";

            txtLoadNumber.Text = "";
            //txtDeliveryAnnotation.Text = "";
            txtDeliveryOrderNumber.Text = "";
            rntxtPallets.Text = "";
            rntPalletForceFullPallets.Text = "";
            rntPalletForceHalfPallets.Text = "";
            rntPalletForceOverPallets.Text = "";
            rntPalletForceQtrPallets.Text = "";
            rntxtCartons.Text = "";
            rntxtPalletSpaces.Text = "";
            rntOrderRate.Text = "";
            txtNotes.Text = "";
            txtConfidentialComments.Text = "";
            txtTrafficNotes.Text = "";

            txtRateTariffCard.Text = "";
            rntxtWeight.Text = "";
            ucCollectionPoint.Reset();
            ucDeliveryPoint.Reset();
            hidTariffRate.Value = string.Empty;

            this.SetDefaultCollectionBookingType();

            txtCollectionNotes.Text = "";

            dteCollectionFromDate.SelectedDate = DateTime.Today;
            dteCollectionFromTime.SelectedDate = DateTime.Today;
            dteCollectionByDate.SelectedDate = DateTime.Today;
            dteCollectionByTime.SelectedDate = DateTime.Today.Add(new TimeSpan(23, 59, 0));
            this.lblUpdateCollection.Text = String.Empty;

            this.SetDefaultDeliveryBookingType();

            // Clear any additional reference fields.
            foreach (RepeaterItem item in repReferences.Items)
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    TextBox ctl2 = (TextBox)item.FindControl("txtReferenceValue");
                    ctl2.Text = "";
                }

            txtDeliveryNotes.Text = "";
            this.lblUpdateDelivery.Text = String.Empty;

            this.chkBookedIn.Checked = false;
            dteTrunkDate.SelectedDate = null;

            txtTrackingNumber.Text = "";
            txtPalletForceNotes1.Text = "";
            txtPalletForceNotes2.Text = "";
            txtPalletForceNotes3.Text = "";
            txtPalletForceNotes4.Text = "";

            rntPalletForceFullPallets.Text = "";
            rntPalletForceQtrPallets.Text = "";
            rntPalletForceHalfPallets.Text = "";
            rntPalletForceOverPallets.Text = "";

            this.hidCheckSubmit.Value = "false";

            hidOrderGroupID.Value = string.Empty;
        }

        private void SetDefaultCollectionBookingType()
        {
            switch (Globals.Configuration.DefaultCollectionBookingType)
            {
                case 0:
                    {
                        rdCollectionBookingWindow.Checked = false;
                        rdCollectionIsAnytime.Checked = false;
                        rdCollectionTimedBooking.Checked = true;

                        hidCollectionTimingMethod.Value = "timed";
                        dteCollectionFromTime.Enabled = true;
                        break;
                    }
                case 1:
                    {
                        rdCollectionBookingWindow.Checked = true;
                        rdCollectionIsAnytime.Checked = false;
                        rdCollectionTimedBooking.Checked = false;

                        hidCollectionTimingMethod.Value = "window";
                        dteCollectionFromTime.Enabled = true;
                        break;
                    }
                case 2:
                    {
                        rdCollectionBookingWindow.Checked = false;
                        rdCollectionIsAnytime.Checked = true;
                        rdCollectionTimedBooking.Checked = false;

                        hidCollectionTimingMethod.Value = "anytime";
                        dteCollectionFromTime.Enabled = false;
                        break;
                    }
            }
        }

        private void SetDefaultDeliveryBookingType()
        {
            switch (Globals.Configuration.DefaultDeliveryBookingType)
            {
                case 0:
                    {
                        rdDeliveryBookingWindow.Checked = false;
                        rdDeliveryIsAnytime.Checked = false;
                        rdDeliveryTimedBooking.Checked = true;

                        hidDeliveryTimingMethod.Value = "timed";
                        dteDeliveryByTime.Enabled = true;
                        break;
                    }
                case 1:
                    {
                        rdDeliveryBookingWindow.Checked = true;
                        rdDeliveryIsAnytime.Checked = false;
                        rdDeliveryTimedBooking.Checked = false;

                        hidDeliveryTimingMethod.Value = "window";
                        dteDeliveryByTime.Enabled = true;
                        break;
                    }
                case 2:
                    {
                        rdDeliveryBookingWindow.Checked = false;
                        rdDeliveryIsAnytime.Checked = true;
                        rdDeliveryTimedBooking.Checked = false;

                        hidDeliveryTimingMethod.Value = "anytime";
                        dteDeliveryByTime.Enabled = false;
                        break;
                    }
            }
        }

        void dlgExtraWindow_DialogCallBack(object sender, EventArgs e)
        {
            this.grdExtras.Rebind();
        }

        #endregion

        #region Load

        private void LoadBusinessTypes()
        {

            if (this.BusinessTypeDataSet.Tables[0] == null)
                return;

            cboBusinessType.DataSource = this.BusinessTypeDataSet;
            cboBusinessType.DataBind();
            cboBusinessType.Items.Insert(0, "-- Please Select -- ");

            // If there is only one business type then select it by default.
            if (this.BusinessTypeDataSet.Tables[0].Rows.Count == 1)
            {
                cboBusinessType.SelectedIndex = 1;
            }

            this.ApplyAttributesToBusinessTypes();
        }

        private void ApplyAttributesToBusinessTypes()
        {
            if (this.BusinessTypeDataSet.Tables[0] == null)
                return;

            ListItem li = null;
            foreach (DataRow row in this.BusinessTypeDataSet.Tables[0].Rows)
            {
                li = cboBusinessType.Items.FindByValue(row["BusinessTypeID"].ToString());
                if (li != null)
                {
                    li.Attributes.Add("showcreatejob", row["ShowCreateJob"] == System.DBNull.Value ? false.ToString() : ((bool)row["ShowCreateJob"]).ToString().ToLower());
                    li.Attributes.Add("createjobchecked", row["CreateJobChecked"] == System.DBNull.Value ? false.ToString() : ((bool)row["CreateJobChecked"]).ToString().ToLower());
                    li.Attributes.Add("showpalletforcefields", row["IsPalletNetwork"] == System.DBNull.Value ? false.ToString() : ((bool)row["IsPalletNetwork"]).ToString().ToLower());
                    li.Attributes.Add("palletnetworkexportdepotcode", row.Field<string>("PalletNetworkExportDepotCode"));
                }
            }
        }

        private void LoadPalletTypes()
        {
            DataSet dsPalletTypes = null;
            DataView vwActivePalletTypes = null;
            bool filterDefaultForActive = false;

            // Clear the pallet types
            cboPalletType.Items.Clear();

            int clientId = 0;

            if (IsUpdate)
            {
                if (this.SavedOrder == null)
                {
                    Facade.IOrder facOrder = new Orchestrator.Facade.Order();
                    this.SavedOrder = facOrder.GetForOrderID(this.OrderID);
                }

                clientId = this.SavedOrder.CustomerIdentityID;
            }
            else
            {
                if (!string.IsNullOrEmpty(cboClient.SelectedValue))
                {
                    clientId = Convert.ToInt32(cboClient.SelectedValue);
                }
            }

            if (!(clientId == 0))
            {
                dsPalletTypes = Orchestrator.Facade.PalletType.GetAllPalletTypes(clientId);
                vwActivePalletTypes = dsPalletTypes.DefaultViewManager.CreateDataView(dsPalletTypes.Tables[0]);

                // Count the active pallet types.
                int countActiveRows = 0;
                foreach (DataRow row in vwActivePalletTypes.Table.Rows)
                {
                    if (Convert.ToInt32(row["IsActive"]) == 1)
                    {
                        countActiveRows++;
                    }
                }

                if (countActiveRows == 0)
                {
                    // No pallet types are specified for the client - return all pallet types
                    vwActivePalletTypes.RowFilter = "";
                }
                else
                {
                    // The client has pallet types - return all the client pallet types
                    if (IsUpdate)
                        vwActivePalletTypes.RowFilter = string.Format("IsActive = 1 OR PalletTypeID = {0}", this.SavedOrder.PalletTypeID);
                    else
                        vwActivePalletTypes.RowFilter = "IsActive = 1";

                    filterDefaultForActive = true;
                }
            }
            else
            {
                dsPalletTypes = Orchestrator.Facade.PalletType.GetAllPalletTypes(0);
                vwActivePalletTypes = dsPalletTypes.DefaultViewManager.CreateDataView(dsPalletTypes.Tables[0]);
            }

            cboPalletType.DataSource = vwActivePalletTypes;
            cboPalletType.DataTextField = "Description";
            cboPalletType.DataValueField = "PalletTypeID";
            cboPalletType.DataBind();

            // Set the client default pallet type to display (if there is one)
            int defaultPalletTypeIndex = 0;
            bool foundDefaultPalletType = false;
            foreach (DataRow row in vwActivePalletTypes.Table.Rows)
            {
                // Note there is an assumption that if "IsDefault" is true then so is "IsActive"
                if (Convert.ToInt32(row["IsDefault"]) == 1)
                {
                    cboPalletType.Items[defaultPalletTypeIndex].Selected = true;
                    foundDefaultPalletType = true;
                    break;
                }
                else
                {
                    if (filterDefaultForActive && Convert.ToInt32(row["IsActive"]) == 1)
                    {
                        defaultPalletTypeIndex++;
                        // If filterDefaultForActive == true but IsActive == 0 then do not increment index
                    }
                    else if (filterDefaultForActive == false)
                    {
                        defaultPalletTypeIndex++;
                    }
                }
            }

            //If we haven't found a default pallet type, look for the "Master Pallet Type default".
            if (!foundDefaultPalletType)
            {
                if (cboPalletType.Items.Count == 1)
                    cboPalletType.Items[0].Selected = true;
                else
                {
                    defaultPalletTypeIndex = 0;
                    //RadComboBoxItem li = cboPalletType.FindItemByText("white one way", true);
                    //if (li != null)
                    //    li.Selected = true;

                    foreach (DataRow row in vwActivePalletTypes.Table.Rows)
                    {
                        // Note there is an assumption that if "IsDefault" is true then so is "IsActive"
                        if (Convert.ToInt32(row["PalletTypeIsDefault"]) == 1)
                        {
                            cboPalletType.Items[defaultPalletTypeIndex].Selected = true;
                            foundDefaultPalletType = true;
                            break;
                        }
                        else
                        {
                            if (filterDefaultForActive && Convert.ToInt32(row["IsActive"]) == 1)
                            {
                                defaultPalletTypeIndex++;
                                // If filterDefaultForActive == true but IsActive == 0 then do not increment index
                            }
                            else if (filterDefaultForActive == false)
                            {
                                defaultPalletTypeIndex++;
                            }
                        }
                    }

                    if (foundDefaultPalletType)
                    {
                        cboPalletType.Items[defaultPalletTypeIndex].Selected = true;
                    }
                }
            }
        }

        private void LoadServiceLevels()
        {
            Orchestrator.Facade.IOrderServiceLevel facOrder = new Orchestrator.Facade.Order();
            DataSet dsServices = facOrder.GetAll();

            cboService.DataSource = dsServices;
            cboService.DataTextField = "Description";
            cboService.DataValueField = "OrderServiceLevelID";
            cboService.DataBind();

            lvServiceLevelDays.DataSource = dsServices;
            lvServiceLevelDays.DataBind();

            //Load the PalletForce Service Levels
            cboPalletForceService.DataSource = new DataView(dsServices.Tables[0], "ShortDescription<>''", "ShortDescription", DataViewRowState.CurrentRows);
            cboPalletForceService.DataTextField = "ShortDescription";
            cboPalletForceService.DataValueField = "OrderServiceLevelID";
            cboPalletForceService.DataBind();

            RadComboBoxItem li = null;
            RadComboBoxItem pfli = null;

            // Set the default Service Level
            if (Orchestrator.Globals.Configuration.DefaultOrderServiceLevel != string.Empty)
            {
                li = cboService.FindItemByText(Orchestrator.Globals.Configuration.DefaultOrderServiceLevel);
                if (li != null)
                    pfli = cboPalletForceService.FindItemByValue(li.Value);
            }
            else
            {
                li = cboService.FindItemByValue("1");
                pfli = cboPalletForceService.FindItemByValue("1");
            }

            if (li != null)
                li.Selected = true;

            if (pfli != null)
                pfli.Selected = true;
        }

        private void LoadExtraTypes()
        {

            Orchestrator.Facade.ExtraType facExtraType = new Orchestrator.Facade.ExtraType();
            List<Entities.ExtraType> extraTypes = facExtraType.GetForIsDisplayedOnAddUpdateOrder();
            cblPalletForceExtraTypes.DataSource = extraTypes.OrderBy(et => et.ShortDescription);
            cblPalletForceExtraTypes.DataTextField = "ShortDescription";
            cblPalletForceExtraTypes.DataValueField = "ExtraTypeId";
            cblPalletForceExtraTypes.DataBind();
        }

        private void OldLoadGoodsTypes()
        {
            DataSet dsGoodsTypes = Orchestrator.Facade.GoodsType.GetAllActiveGoodsTypes();
            cboGoodsType.DataSource = dsGoodsTypes;
            cboGoodsType.DataTextField = "Description";
            cboGoodsType.DataValueField = "GoodsTypeId";
            cboGoodsType.DataBind();

            cboGoodsType.Items[0].Selected = true;
        }

        private void LoadGoodsTypes()
        {
            DataSet dsGoodsType = null;
            DataView vwGoodsType = null;
            DataRowView selectRow = null;
            string savedOrderGoodsType = null;

            cboGoodsType.Items.Clear();
            this.cboGoodsType.ClearSelection();

            //Get the Goods Types for the client if selected
            //otherwise get all GoodsTypes
            //Ensure we use the Client ID from the SavedOrder if updating.
            int clientId = 0;
            if (IsUpdate)
            {
                if (this.SavedOrder == null)
                {
                    Facade.IOrder facOrder = new Orchestrator.Facade.Order();
                    this.SavedOrder = facOrder.GetForOrderID(this.OrderID);
                }

                clientId = this.SavedOrder.CustomerIdentityID;
            }
            else
                int.TryParse(cboClient.SelectedValue, out clientId);
            if (clientId != 0)
            {
                dsGoodsType = Facade.GoodsType.GetAllGoodsTypes(clientId);
                vwGoodsType = dsGoodsType.DefaultViewManager.CreateDataView(dsGoodsType.Tables[0]);

                vwGoodsType.RowFilter = "IsActive = 1";

                if (this.SavedOrder != null)
                {
                    // Add the Saved Order Goods Type just in case it has been removed for the Client or the orders has been created via Import/Integration with an incorrect type.
                    savedOrderGoodsType = this.SavedOrder.GoodsTypeID.ToString();
                    vwGoodsType.RowFilter = "IsActive = 1 OR GoodsTypeID = " + savedOrderGoodsType;
                }
                else
                    vwGoodsType.RowFilter = "IsActive = 1";

                //If no Goods Types are Active for the client reset the filter so that all are Active
                if (vwGoodsType.Count == 0)
                    vwGoodsType.RowFilter = "";

                //If there is a Default row select it, otherwise use the "Normal" GoodsType 
                foreach (DataRowView drv in vwGoodsType)
                    if ((bool)drv["IsDefault"])
                    {
                        selectRow = drv;
                        break;
                    }
            }
            else
            {
                dsGoodsType = Facade.GoodsType.GetAllActiveGoodsTypes();
                vwGoodsType = dsGoodsType.DefaultViewManager.CreateDataView(dsGoodsType.Tables[0]);
            }

            //If a default Goods Type could not be found select the Normal one
            if (selectRow == null)
            {
                foreach (DataRowView drv in vwGoodsType)
                    if ((string)drv["Description"] == "Normal")
                    {
                        selectRow = drv;
                        break;
                    }
            }

            //Bind the view to the Drop down list
            cboGoodsType.DataSource = vwGoodsType;
            cboGoodsType.DataTextField = "Description";
            cboGoodsType.DataValueField = "GoodsTypeId";
            cboGoodsType.DataBind();

            //Select the row chosen to be the default
            if (selectRow != null)
                cboGoodsType.SelectedValue = ((int)selectRow["GoodsTypeId"]).ToString();

        }

        private void LoadInvalidDates()
        {
            Facade.IOrganisation facOrg = new Facade.Organisation();

            // If the list of invalid dates has not been set then create it.
            if (InvalidDates == null)
            {
                InvalidDates = new List<DateTime>();

                DateTime startDate = DateTime.Today.AddMonths(-2);
                DateTime endDate = DateTime.Today.AddMonths(10);

                DataSet ds = facOrg.GetAllPublicHolidays(startDate, endDate);

                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                    foreach (DataRow dr in ds.Tables[0].Rows)
                        InvalidDates.Add((DateTime)dr["PublicHolidayDate"]);

                //List of weekend dates from 2 months prior to todays date, to 10 months into the future.
                while (startDate != endDate)
                {
                    if (startDate.DayOfWeek == DayOfWeek.Saturday || startDate.DayOfWeek == DayOfWeek.Sunday)
                        InvalidDates.Add(startDate);

                    startDate = startDate.AddDays(1);
                }
            }

            lvInvalidDates.DataSource = InvalidDates;
            lvInvalidDates.DataBind();
        }

        private void LoadDeliveryDates()
        {
            //// Set the Default dates
            dteCollectionFromDate.SelectedDate = DateTime.Today;
            dteCollectionByDate.SelectedDate = DateTime.Today;
            dteDeliveryByDate.SelectedDate = DateTime.Today.AddDays(1);
            dteDeliveryFromDate.SelectedDate = DateTime.Today.AddDays(1);

            // Find the selected service level 
            RadComboBoxItem li = cboService.SelectedItem;
            if (li != null)
            {
                //Check for weekend and public holiday dates, load if not loaded.
                if (InvalidDates != null)
                    LoadInvalidDates();

                //Find the Number of days for the specified service level.
                Orchestrator.Facade.IOrderServiceLevel facOrder = new Orchestrator.Facade.Order();
                DataSet dsServices = facOrder.GetAll();
                DataRow dr = dsServices.Tables[0].Rows.Cast<DataRow>().FirstOrDefault(cr => (int)cr["OrderServiceLevelID"] == int.Parse(li.Value));

                // If the number of days has been set, proceed.
                if (dr != null && dr["NoOfDays"] != DBNull.Value)
                {
                    int noOfDays = (int)dr["NoOfDays"];

                    // Get the latest collection date.
                    DateTime newDeliveryDate = dteCollectionByDate.SelectedDate.Value.AddDays(noOfDays);

                    // If the no of days added to the collection date is a weekend or public holiday, keep going until its neither.
                    while (InvalidDates.Exists(ivd => ivd == newDeliveryDate))
                    {
                        noOfDays++;
                        newDeliveryDate = dteCollectionByDate.SelectedDate.Value.AddDays(noOfDays);
                    }

                    // Set BOTH the delivery from and by date with the new delivery DATE.
                    dteDeliveryFromDate.SelectedDate = newDeliveryDate;
                    dteDeliveryByDate.SelectedDate = newDeliveryDate;
                }
            }
        }

        #endregion

        #region Load Order

        private void LoadOrder()
        {
            Orchestrator.Facade.IOrder facOrder = new Orchestrator.Facade.Order();
            int orderID = -1;
            int.TryParse(Request.QueryString["oID"], out orderID);

            bool orderIsInGroup = false;
            hidOrderGroupID.Value = string.Empty;

            if (orderID > 0)
            {
                this.OrderID = orderID;
                IsUpdate = true;

                DisplayOrder();

                this.btnSubmit.Text = "Update Order";
                this.btnSubmitTop.Text = "Update Order";
                this.hidCheckSubmit.Value = "false";
                btnAddAndReset.Visible = false;
                btnAddAndResetTop.Visible = false;
                var order = this.SavedOrder;
                btnCreateRun.Visible = !order.PlannedForCollection && !order.PlannedForDelivery;
                btnCreateRunTop.Visible = btnCreateRun.Visible;

                orderIsInGroup = order.OrderGroupID > 0;
                if (orderIsInGroup)
                    hidOrderGroupID.Value = order.OrderGroupID.ToString();
            }
            else
            {
                //Set the default values for a new Order
                ClearScreen();

                int orderGroupID = 0;
                if (!string.IsNullOrEmpty(Request.QueryString[_ogid_QS]) && int.TryParse(Request.QueryString[_ogid_QS], out orderGroupID))
                {
                    Facade.IOrderGroup facOrderGroup = new Facade.Order();
                    Entities.OrderGroup orderGroup = facOrderGroup.Get(orderGroupID);
                    orderIsInGroup = orderGroup != null;

                    if (orderIsInGroup)
                    {
                        hidOrderGroupID.Value = orderGroupID.ToString();
                        var firstOrder = orderGroup.Orders.FirstOrDefault();
                        int? allocatedToIdentityID = firstOrder == null ? null : firstOrder.AllocatedToIdentityID;
                        bool isManuallyAllocated = firstOrder == null ? false : firstOrder.IsManuallyAllocated;
                        LoadAllocation(true, allocatedToIdentityID, isManuallyAllocated, false);

                        if (orderGroup.Orders.Count > 0)
                        {
                            int customerIdentityID = orderGroup.Orders[0].CustomerIdentityID;
                            Facade.IOrganisation facOrganisation = new Facade.Organisation();
                            cboClient.Text = facOrganisation.GetNameForIdentityId(customerIdentityID);
                            cboClient.SelectedValue = customerIdentityID.ToString();
                            this.cboClient_SelectedIndexChanged(cboClient, null);

                            ListItem businessTypeItem = cboBusinessType.Items.FindByValue(orderGroup.Orders[orderGroup.Orders.Count - 1].BusinessTypeID.ToString());

                            if (businessTypeItem != null)
                            {
                                cboBusinessType.ClearSelection();
                                businessTypeItem.Selected = true;
                            }

                            Facade.IPoint facPoint = new Facade.Point();

                            this.txtLoadNumber.Text = orderGroup.Orders[0].CustomerOrderNumber;
                            this.rntxtPallets.Text = orderGroup.Orders[0].NoPallets.ToString();
                            this.rntxtPalletSpaces.Text = orderGroup.Orders[0].PalletSpaces.ToString();
                            this.rntxtCartons.Text = orderGroup.Orders[0].Cases.ToString();
                            this.rntxtWeight.Text = orderGroup.Orders[0].Weight.ToString();

                            this.txtTrafficNotes.Text = orderGroup.Orders[0].TrafficNotes;
                            this.txtCollectionNotes.Text = orderGroup.Orders[0].CollectionNotes;
                            this.txtConfidentialComments.Text = orderGroup.Orders[0].ConfidentialComments;

                            this.txtDeliveryNotes.Text = orderGroup.Orders[0].DeliveryNotes;
                            this.txtNotes.Text = orderGroup.Orders[0].Notes;

                            if (Globals.Configuration.GroupageGroupedOrdersSharedCollectionPoint)
                            {
                                // Set the default collection point of this new order to match the first
                                // order in the group.
                                Entities.Point firstOrdersCollectionPoint = facPoint.GetPointForPointId(orderGroup.Orders[0].CollectionPointID);
                                ucCollectionPoint.SelectedPoint = firstOrdersCollectionPoint;

                            }

                            dteCollectionFromDate.SelectedDate = orderGroup.Orders[0].CollectionDateTime;
                            dteCollectionByDate.SelectedDate = orderGroup.Orders[0].CollectionByDateTime;
                            dteCollectionFromTime.SelectedDate = orderGroup.Orders[0].CollectionDateTime;
                            dteCollectionByTime.SelectedDate = orderGroup.Orders[0].CollectionByDateTime;

                            if (orderGroup.Orders[0].CollectionIsAnytime)
                            {
                                // Anytime
                                rdCollectionIsAnytime.Checked = true;
                                rdCollectionTimedBooking.Checked = false;
                                rdCollectionBookingWindow.Checked = false;

                                dteCollectionFromTime.Enabled = false;
                                hidCollectionTimingMethod.Value = "anytime";
                            }
                            else if (orderGroup.Orders[0].CollectionDateTime == orderGroup.Orders[0].CollectionByDateTime)
                            {
                                // Timed booking
                                rdCollectionIsAnytime.Checked = false;
                                rdCollectionTimedBooking.Checked = true;
                                rdCollectionBookingWindow.Checked = false;

                                hidCollectionTimingMethod.Value = "timed";
                            }
                            else
                            {
                                // Booking windowq  
                                rdCollectionIsAnytime.Checked = false;
                                rdCollectionTimedBooking.Checked = false;
                                rdCollectionBookingWindow.Checked = true;

                                hidCollectionTimingMethod.Value = "window";
                            }

                            Entities.Point firstOrdersDeliveryPoint = facPoint.GetPointForPointId(orderGroup.Orders[0].DeliveryPointID);
                            ucDeliveryPoint.SelectedPoint = firstOrdersDeliveryPoint;

                            dteDeliveryFromDate.SelectedDate = orderGroup.Orders[0].DeliveryFromDateTime;
                            dteDeliveryByDate.SelectedDate = orderGroup.Orders[0].DeliveryDateTime;
                            dteDeliveryByTime.SelectedDate = orderGroup.Orders[0].DeliveryDateTime;
                            dteDeliveryFromTime.SelectedDate = orderGroup.Orders[0].DeliveryFromDateTime;

                            if (orderGroup.Orders[0].DeliveryIsAnytime)
                            {
                                // Anytime
                                rdDeliveryIsAnytime.Checked = true;
                                rdDeliveryTimedBooking.Checked = false;
                                rdDeliveryBookingWindow.Checked = false;

                                dteDeliveryFromTime.Enabled = false;
                                hidDeliveryTimingMethod.Value = "anytime";

                            }
                            else if (orderGroup.Orders[0].DeliveryFromDateTime == orderGroup.Orders[0].DeliveryDateTime)
                            {
                                // Timed booking
                                rdDeliveryIsAnytime.Checked = false;
                                rdDeliveryTimedBooking.Checked = true;
                                rdDeliveryBookingWindow.Checked = false;

                                hidDeliveryTimingMethod.Value = "timed";
                            }
                            else
                            {
                                // Booking window
                                rdDeliveryIsAnytime.Checked = false;
                                rdDeliveryTimedBooking.Checked = false;
                                rdDeliveryBookingWindow.Checked = true;

                                hidDeliveryTimingMethod.Value = "window";
                            }
                        }
                    }
                }
            }

            // If the order is in a group, the user can not change the client, or specify a rate.
            plcRate.Visible = !orderIsInGroup;
            cboClient.Enabled = !orderIsInGroup;
            trOrderTotal.Visible = !orderIsInGroup;

            // The user can only add a grouped order when the order is first created - or when the order is not in a group.
            btnAddGroupedOrder.Visible = !IsUpdate && !orderIsInGroup;
            btnAddGroupedOrderTop.Visible = !IsUpdate && !orderIsInGroup;

            if (Request.QueryString["returnId"] != null
                && Convert.ToBoolean(Request.QueryString["returnId"].ToString()) == true)
            {
                this.btnAddAndReset.Visible = false;
                this.btnAddAndResetTop.Visible = false;
                this.btnAddGroupedOrder.Visible = false;
                this.btnAddGroupedOrderTop.Visible = false;
                this.btnCreateRun.Visible = false;
                this.btnCreateRunTop.Visible = false;
                this.btnSubmit.Text = "Add Order and Close";
                this.btnSubmitTop.Text = "Add Order and Close";
                this.btnSubmit.Width = 175;
                this.btnSubmitTop.Width = 175;
            }
        }

        private void LoadAllocation(Entities.Order order)
        {
            bool isSubcontracted = order.JobSubContractID > 0;

            if (!isSubcontracted)
            {
                // Check whether the order is on a run which has been subcontracted as a whole
                Facade.IOrder facOrder = new Facade.Order();
                isSubcontracted = facOrder.IsOrderOnJobSubcontractedAsAWhole(order.OrderID);
            }

            LoadAllocation(order.OrderGroupID > 0, order.AllocatedToIdentityID, order.IsManuallyAllocated, isSubcontracted);
        }

        private void LoadAllocation(bool orderIsInGroup, int? allocatedToIdentityID, bool isManuallyAllocated, bool isSubcontracted)
        {
            if (WebUI.Utilities.IsAllocationEnabled())
            {
                // The user can't change allocation here if the order is in a group or has been subcontracted.
                string allocatedToName = string.Empty;
                if (allocatedToIdentityID.HasValue)
                {
                    var allocatedTo = EF.DataContext.Current.OrganisationSet.First(o => o.IdentityId == allocatedToIdentityID);
                    allocatedToName = allocatedTo.OrganisationName;
                }

                cboAllocatedTo.SelectedValue = allocatedToIdentityID.ToString();
                cboAllocatedTo.Text = allocatedToName;
                lblAllocatedTo.Text = string.IsNullOrEmpty(allocatedToName) ? "- none -" : allocatedToName;

                hidIsManuallyAllocated.Value = isManuallyAllocated.ToString();

                bool changeAllowed = !orderIsInGroup && !isSubcontracted;

                cboAllocatedTo.Visible = changeAllowed;
                lblAllocatedTo.Visible = !changeAllowed;
                lblAllocatedTo.ToolTip =
                    orderIsInGroup ?
                        "The allocation for a grouped order must be set on the order group details page" :
                        isSubcontracted ?
                            "To change the allocation of a subcontracted order please unsubcontract it and then reallocate or resubcontract" :
                            null;

                bool hasSubcontractorHistory = allocatedToIdentityID.HasValue;
                if (!hasSubcontractorHistory && this.OrderID > 0)
                    hasSubcontractorHistory = EF.DataContext.Current.OrderSubcontractorHistorySet.Any(osh => osh.OrderID == this.OrderID);

                imgAllocationHistory.Visible = hasSubcontractorHistory;
                imgAllocationHistory.Attributes["onclick"] = dlgAllocationHistory.GetOpenDialogScript(string.Format("oid={0}", this.OrderID));
            }
        }

        private List<RefusalRow> RefusalsForOriginalOrder()
        {
            List<RefusalRow> refusals = new List<RefusalRow>();
            bool anyRefusals = (from refusal in Orchestrator.EF.DataContext.Current.GoodsRefusalSet.Include("NewOrder")
                                where refusal.OriginalOrder.OrderId == this.OrderID
                                select refusal).Any();

            if (anyRefusals)
            {
                var ds = (from refusal in Orchestrator.EF.DataContext.Current.GoodsRefusalSet.Include("NewOrder").Include("Instruction")
                          .Include("GoodsRefusalLocation").Include("GoodsRefusalType").Include("OriginalOrder")
                          where refusal.OriginalOrder.OrderId == this.OrderID
                          select new
                          {
                              RefusalId = refusal.RefusalId,
                              ProductName = refusal.ProductName,
                              ProductCode = refusal.ProductCode,
                              QuantityRefused = refusal.QuantityRefused,
                              QuantityOrdered = refusal.QuantityOrdered,
                              PackSize = refusal.PackSize,
                              RefusalNotes = refusal.RefusalNotes,
                              RefusalReceiptNumber = refusal.RefusalReceiptNumber,
                              TimeFrame = refusal.TimeFrame,
                              OriginalOrderId = (refusal.OriginalOrder == null) ? -1 : refusal.OriginalOrder.OrderId,
                              NewOrderId = (refusal.NewOrder == null) ? -1 : refusal.NewOrder.OrderId,
                              RefusalLocation = refusal.GoodsRefusalLocation.Description,
                              RefusalType = refusal.GoodsRefusalType.Description,
                              JobId = refusal.Instruction.Job.JobId,
                              InstructionId = refusal.Instruction.InstructionId
                          }).ToList();

                var ds2 = from row in ds
                          select new RefusalRow
                          {
                              RefusalId = row.RefusalId,
                              ProductName = row.ProductName,
                              ProductCode = row.ProductCode,
                              QuantityRefused = row.QuantityRefused == null ? 0 : row.QuantityRefused.Value,
                              QuantityOrdered = row.QuantityOrdered == null ? 0 : row.QuantityOrdered.Value,
                              PackSize = row.PackSize,
                              RefusalNotes = row.RefusalNotes,
                              RefusalReceiptNumber = row.RefusalReceiptNumber,
                              TimeFrame = row.TimeFrame == null ? "" : row.TimeFrame.Value.ToString("dd/MM/yyyy"),
                              OriginalOrderId = row.OriginalOrderId,
                              NewOrderId = row.NewOrderId,
                              RefusalLocation = row.RefusalLocation,
                              RefusalType = row.RefusalType,
                              JobId = row.JobId,
                              InstructionId = row.InstructionId
                          };

                refusals = ds2.ToList<RefusalRow>();
            }
            return refusals;
        }

        private List<RefusalRow> RefusalsForNewOrder()
        {
            List<RefusalRow> refusals = new List<RefusalRow>();
            bool anyRefusals = (from refusal in Orchestrator.EF.DataContext.Current.GoodsRefusalSet.Include("NewOrder")
                                where refusal.NewOrder.OrderId == this.OrderID
                                select refusal).Any();

            if (anyRefusals)
            {

                var ds = (from refusal in Orchestrator.EF.DataContext.Current.GoodsRefusalSet.Include("NewOrder").Include("Instruction")
                          .Include("GoodsRefusalLocation").Include("GoodsRefusalType").Include("OriginalOrder")
                          where refusal.NewOrder.OrderId == this.OrderID
                          select new
                          {
                              RefusalId = refusal.RefusalId,
                              ProductName = refusal.ProductName,
                              ProductCode = refusal.ProductCode,
                              QuantityRefused = refusal.QuantityRefused,
                              QuantityOrdered = refusal.QuantityOrdered,
                              PackSize = refusal.PackSize,
                              RefusalNotes = refusal.RefusalNotes,
                              RefusalReceiptNumber = refusal.RefusalReceiptNumber,
                              TimeFrame = refusal.TimeFrame,
                              OriginalOrderId = refusal.OriginalOrder.OrderId,
                              NewOrderId = refusal.NewOrder.OrderId,
                              RefusalLocation = refusal.GoodsRefusalLocation.Description,
                              RefusalType = refusal.GoodsRefusalType.Description,
                              JobId = refusal.Instruction.Job.JobId,
                              InstructionId = refusal.Instruction.InstructionId
                          }).ToList();

                var ds2 = from row in ds
                          select new RefusalRow
                          {
                              RefusalId = row.RefusalId,
                              ProductName = row.ProductName,
                              ProductCode = row.ProductCode,
                              QuantityRefused = row.QuantityRefused == null ? 0 : row.QuantityRefused.Value,
                              QuantityOrdered = row.QuantityOrdered == null ? 0 : row.QuantityOrdered.Value,
                              PackSize = row.PackSize,
                              RefusalNotes = row.RefusalNotes,
                              RefusalReceiptNumber = row.RefusalReceiptNumber,
                              TimeFrame = row.TimeFrame == null ? "" : row.TimeFrame.Value.ToString("dd/MM/yyyy"),
                              OriginalOrderId = row.OriginalOrderId,
                              NewOrderId = row.NewOrderId,
                              RefusalLocation = row.RefusalLocation,
                              RefusalType = row.RefusalType,
                              JobId = row.JobId,
                              InstructionId = row.InstructionId
                          };

                refusals = ds2.ToList<RefusalRow>();
            }

            return refusals;
        }

        private void DisplayOrder()
        {
            Orchestrator.Entities.Point point = null;
            Orchestrator.Entities.Organisation org = null;
            Orchestrator.Entities.OrganisationDefault orgDefaults = null;

            Orchestrator.Facade.IOrganisation facOrg = new Orchestrator.Facade.Organisation();
            
            Orchestrator.Facade.IPOD facPOD = new Orchestrator.Facade.POD();

            Orchestrator.Facade.IPoint facPoint = new Orchestrator.Facade.Point();
            Orchestrator.Facade.IOrder facOrder = new Orchestrator.Facade.Order();

            if (this.SavedOrder == null)
                this.SavedOrder = facOrder.GetForOrderID(this.OrderID);

            Orchestrator.Entities.POD scannedPOD = facPOD.GetForOrderID(this.SavedOrder.OrderID);
            Orchestrator.Entities.Scan scannedBookingForm = null;

            CurrentCulture = new CultureInfo(this.SavedOrder.LCID);
            ExchangeRateID = this.SavedOrder.ExchangeRateID;

            phLegsForOrder.Visible = true;
            tabLegsForOrder.Visible = true;
            tabviewLegsForOrder.Visible = true;
            btnCreateDeliveryNote.Visible = true;
            btnCreateDeliveryNoteTop.Visible = true;
            btnCancelOrder.Visible = true;
            btnCancelOrderTop.Visible = true;
            btnPIL.Visible = true;
            btnPILTop.Visible = true;
            btnPodLabel.Visible = Globals.Configuration.PodLabelsEnabled;
            btnPodLabelTop.Visible = Globals.Configuration.PodLabelsEnabled;

            trFinancial.Visible = true;
            tabFinancial.Visible = true;
            tabviewFinancial.Visible = true;
            //trExtras.Visible = true;
            trExtras_AddButton.Visible = true;

            btnAddExtra.Attributes.Add("onclick", "javascript:" + dlgExtraWindow.GetOpenDialogScript("OID=" + OrderID + "&lcID=" + this.SavedOrder.LCID + "&erID=" + (ExchangeRateID.HasValue ? ExchangeRateID.Value.ToString() : "-1"))+ " return false;");

            txtConfidentialComments.Height = 110;
            txtNotes.Height = 110;
            this.tabviewRedeliveriesRefusals.Visible = true;
            //if (this.SavedOrder.FuelSurchargePercentage != null)
            this.rntFuelSurchargePercentage.Value = Convert.ToDouble(this.SavedOrder.FuelSurchargePercentage); //string.Format("{0}%", this.SavedOrder.FuelSurchargePercentage.Value);

            this.tabviewRedeliveriesRefusals.Visible = true;

            if (EF.DataContext.Current.AttemptedCollectionSet.Any(ac => ac.Order.OrderId == this.OrderID))
            {
                tabAttemptedCollection.Visible = tabviewAttemptedCollection.Visible = true;

                Facade.IAttemptedCollection facAC = new Facade.AttemptedCollection();
                rgAttemptedCollection.DataSource = facAC.GetDetailsForOrderId(this.OrderID);
                rgAttemptedCollection.DataBind();
            }

            #region Commented out
            //this.rptOversShorts.DataSource
            //this.rptOversShorts.DataSource = OversShortDataSource();
            //this.rptOversShorts.DataBind();

            //this.rptRefusalsNewOrder.DataSource = null;
            //this.rptRefusalsNewOrder.DataSource = this.RefusalsForNewOrder();
            //this.rptRefusalsNewOrder.DataBind();
            //this.rptRefusalsOriginalOrder.DataSource = null;
            //this.rptRefusalsOriginalOrder.DataSource = this.RefusalsForOriginalOrder();
            //this.rptRefusalsOriginalOrder.DataBind();
            #endregion

            // If this order is in a group, show a link to the group information, otherwise show the order's rate.
            if (this.SavedOrder.IsInGroup)
            {
                string openOrderGroupProfileJS = dlgOrderGroup.GetOpenDialogScript("OGID=" + this.SavedOrder.OrderGroupID.ToString()); //GetOpenOrderGroupJavascript(this.SavedOrder.OrderGroupID, false);
                this.lnkOrderGroup.NavigateUrl = "javascript:" + openOrderGroupProfileJS;

                this.lnkEditRate.NavigateUrl = "javascript:" + openOrderGroupProfileJS;
                Orchestrator.Facade.IOrderGroup facOrderGroup = new Orchestrator.Facade.Order();
                int orderCount = facOrderGroup.GetCountOfOrdersForGroup(this.SavedOrder.OrderGroupID);
                lnkOrderGroup.Text = string.Format("{0} orders contained within this order's group.", orderCount);
            }

            lnkOrderGroup.Visible = this.SavedOrder.IsInGroup;
            lnkCreateGroup.Visible = !this.SavedOrder.IsInGroup &&
                                     !(this.SavedOrder.IsReadyToInvoice || this.SavedOrder.OrderStatus == eOrderStatus.Cancelled ||
                                       this.SavedOrder.OrderStatus == eOrderStatus.Invoiced);

            bool orderBeingInvoiced = facOrder.IsOrderBeingInvoiced(this.SavedOrder.OrderID);

            if (this.SavedOrder.IsInGroup)
            {
                // Populate the basic order group information.
                lblRate.Text = OrderGroup.ForeignRate.ToString("C", this.CurrentCulture);
                lblRate.Visible = true;
                lnkEditRate.Visible = true;
                cboGroupedPlanning.ClearSelection();
                ListItem selected = cboGroupedPlanning.Items.FindByValue(OrderGroup.GroupedPlanning.ToString().ToLower());
                if (selected != null) selected.Selected = true;
                lblOrderCount.Text = OrderGroup.Orders.Count.ToString();

                lnkEditRate.Enabled = !orderBeingInvoiced;
                cboGroupedPlanning.Enabled = !orderBeingInvoiced;
            }

            if (this.SavedOrder.OrderStatus == eOrderStatus.Cancelled || this.SavedOrder.OrderStatus == eOrderStatus.Invoiced || orderBeingInvoiced)
            {
                btnCancelOrder.Visible = false;
                btnCancelOrderTop.Visible = false;
                grdOrders.Columns.FindByUniqueName("Remove").Visible = false;
            }
            else
            {
                if (this.SavedOrder.OrderStatus != eOrderStatus.Delivered)
                    grdOrders.Columns.FindByUniqueName("Remove").Visible = true;
                else
                    grdOrders.Columns.FindByUniqueName("Remove").Visible = false;
            }

            lblInvoiceSeperatley.Text = this.SavedOrder.InvoiceSeperatley == true ? "Yes" : "No";
            lblReadyToInvoice.Text = this.SavedOrder.IsReadyToInvoice ? "Yes" : "No";

            org = facOrg.GetForIdentityId(this.SavedOrder.CustomerIdentityID);
            orgDefaults = facOrg.GetInvoiceSettingForIdentityId(this.SavedOrder.CustomerIdentityID);
                                   
            btnMakeInvoiceable.Visible = this.SavedOrder.OrderStatus == eOrderStatus.Delivered && !this.SavedOrder.IsReadyToInvoice &&
                                         !this.SavedOrder.IsBeingInvoiced &&
                                         Orchestrator.WebUI.Security.Authorise.CanAccess(eSystemPortion.Invoicing);

            Orchestrator.Facade.IOrder facIOrder = new Orchestrator.Facade.Order();
            bool undeliveredOrders = facIOrder.IsAnyUndeliveredOrdersInGroup(this.SavedOrder.OrderID);

            if (btnMakeInvoiceable.Visible && (org.OnHold || orgDefaults.IsExcludedFromInvoicing || undeliveredOrders))
            {
                lblOnHoldReason.Text = "You cannot Flag this order as Ready to Invoice because:<br />"; 

                if (org.OnHold)
                {
                    string onHoldReasonText = "No reason has been provided";

                    if (!String.IsNullOrWhiteSpace(org.OnHoldReason))
                        onHoldReasonText = "The reason provided is: " + org.OnHoldReason;

                    lblOnHoldReason.Text += "<br />This Customer (" + org.OrganisationName + ") is On Hold. " + onHoldReasonText;
                }

                if (orgDefaults.IsExcludedFromInvoicing)
                    lblOnHoldReason.Text += "<br />This Customer (" + org.OrganisationName + ") is set to be Excluded From Invoicing";

                if (undeliveredOrders)
                    lblOnHoldReason.Text += "<br />There are Undelivered Orders in the same Group as this Order";

                btnMakeInvoiceable.Visible = false;
                lblOnHoldReason.Visible = true;
            }
            else
            {
                lblOnHoldReason.Visible = false;
                lblOnHoldReason.Text = "";
            }

            lblBeingInvoiced.Text = this.SavedOrder.IsBeingInvoiced ? "Yes" : "No";
            btnUnFlagForInvoicing.Visible = this.SavedOrder.IsReadyToInvoice && !this.SavedOrder.IsBeingInvoiced && this.SavedOrder.OrderStatus != eOrderStatus.Invoiced;
            
            // If both the unflag for invoicing, make invoicable buttons and the On-Hold Reason label are hidden, hide the button section.
            if (btnMakeInvoiceable.Visible == false && btnUnFlagForInvoicing.Visible == false && lblOnHoldReason.Visible == false )
                divInvoiceButtons.Visible = false;
            else
                divInvoiceButtons.Visible = true;

            if (this.SavedOrder.ImportedDate != null)
                lblImportedDate.Text = this.SavedOrder.ImportedDate.Value.ToString("dd/MM/yy HH:mm");

            if (this.SavedOrder.ExportedDate != null)
                lblExportedDate.Text = this.SavedOrder.ExportedDate.Value.ToString("dd/MM/yy HH:mm");


            if (this.SavedOrder.ClientInvoiceID <= 0)
            {
                lblBeingInvoiced.Text = "No";
                lblInvoiceNumber.Text = "None Assigned";
            }
            else
            {
                lblBeingInvoiced.Text = "Yes";
                lblInvoiceNumber.Text = this.SavedOrder.ClientInvoiceID.ToString();
                string PDFLink = this.SavedOrder.PDFLocation.ToString();
                lblInvoiceNumber.NavigateUrl = Orchestrator.Globals.Configuration.WebServer + PDFLink;
            }

            // this.SavedOrder.OrderGroupID > 0)
            plcOrderGroupDetail.Visible = (this.SavedOrder.OrderGroupID > 0);
            phOrderGroup.Visible = true;
            phOrderInfo.Visible = true;
            lblHasProblems.Text = this.SavedOrder.HasProblem ? "Yes" : "No";
            //lblReadyToInvoice.Text = this.SavedOrder.IsReadyToInvoice ? "Yes" : "No";

            if (this.SavedOrder.HasProblem)
                lblHasProblems.Attributes["Style"] = "color:Red";

            if (this.SavedOrder.CreateDateTime != DateTime.MinValue)
                lblCreated.Text = this.SavedOrder.CreatedBy + " on " + this.SavedOrder.CreateDateTime.ToString("dd/MM/yy HH:mm");
            if (this.SavedOrder.LastUpdatedDateTime != DateTime.MinValue)
                lblLastUpdated.Text = this.SavedOrder.LastUpdatedBy + " on " + this.SavedOrder.LastUpdatedDateTime.ToString("dd/MM/yy HH:mm");

            //If the order has a scanned Booking Form get it so that
            //a link to it can be displayed
            phBookingForm.Visible = true;
            plcPOD.Visible = true;
            if (this.SavedOrder.BookingFormScannedFormId != null)
            {
                Orchestrator.Facade.Form facBF = new Orchestrator.Facade.Form();
                scannedBookingForm = facBF.GetForScannedFormId(this.SavedOrder.BookingFormScannedFormId.Value);
            }

            if (scannedBookingForm != null)
            {
                hlBookingFormLink.Visible = true;
                hlBookingFormLink.NavigateUrl = scannedBookingForm.ScannedFormPDF ?? string.Empty;

                aScanBookingForm.InnerHtml = "| Re-Scan";
                aScanBookingForm.HRef = @"javascript:ReDoBookingForm(" + scannedBookingForm.ScannedFormId + "," + this.SavedOrder.OrderID.ToString() + ");";

            }
            else
            {
                hlBookingFormLink.Visible = false;
                aScanBookingForm.InnerHtml = "Scan";
                aScanBookingForm.HRef = @"javascript:NewBookingForm(" + this.SavedOrder.OrderID.ToString() + ");";
            }

            // Display the View/Rescan link, even if its a NoDocumentScanned Item.
            bool hasScannedForm = scannedPOD != null && !string.IsNullOrEmpty(scannedPOD.ScannedFormPDF);

            if (hasScannedForm)
            {
                plcDriverCheckIn.Visible = true;
                // Tom Farrow 05/02/08: Amended to use the pod id that is related to the order, not the job.
                hlPODLink.Visible = true;
                hlPODLink.NavigateUrl = scannedPOD.ScannedFormPDF.Trim();
                aScanPOD.InnerHtml = "| Re-Scan";
                int details = facOrder.GetDeliveryCollectDropIDForPODScanner(this.SavedOrder.OrderID);
                plcPOD.Visible = true;
                int JobId = facOrder.GetDeliveryJobIDForOrderID(this.SavedOrder.OrderID);
                aScanPOD.HRef = @"javascript:OpenPODWindowForEdit(" + scannedPOD.ScannedFormId + "," + JobId + "," + details + ");";            
            }
            else
            {
                if (this.SavedOrder.OrderStatus == eOrderStatus.Delivered || this.SavedOrder.OrderStatus == eOrderStatus.Invoiced)
                {
                    plcDriverCheckIn.Visible = true;
                    hlPODLink.Visible = false;
                    int details = facOrder.GetDeliveryCollectDropIDForPODScanner(this.SavedOrder.OrderID);
                    plcPOD.Visible = true;
                    int JobId = facOrder.GetDeliveryJobIDForOrderID(this.SavedOrder.OrderID);
                    aScanPOD.HRef = @"javascript:OpenPODWindow(" + JobId + "," + details + ")";
                }
                else
                {
                    plcPOD.Visible = false;
                }
            }

            var hasSignature = false;
            var hasMwfDeliveryDetails = false;

            if (OrderID > 0)
            {
                using (var uow = DIContainer.CreateUnitOfWork())
                {
                    // Display MWF signatures
                    var repo = DIContainer.CreateRepository<Repositories.IOrderRepository>(uow);
                    var signaturesForOrder = repo.GetMWFSignaturesForOrder(this.SavedOrder.OrderID);

                    lvSignatures.DataSource = signaturesForOrder;
                    lvSignatures.DataBind();

                    hasSignature = signaturesForOrder.Any();

                    // Display MWF delivery details
                    txtMwfDeliveryNotes.Text = string.Empty;
                    var deliverySignature = signaturesForOrder.FirstOrDefault(s => s.InstructionType == MWFInstructionTypeEnum.Deliver);

                    if (deliverySignature != null && !string.IsNullOrWhiteSpace(deliverySignature.Comment) && deliverySignature.Comment.ToUpper() != "SIGNED")
                        txtMwfDeliveryNotes.Text = deliverySignature.Comment;

                    var deliveryDetails = repo.GetMWFDeliveryDetails(this.SavedOrder.OrderID);
                    hasMwfDeliveryDetails = deliveryDetails != null;

                    if (hasMwfDeliveryDetails)
                    {
                        txtMwfQuantityDelivered.Text = string.Format("{0}", deliveryDetails.ConfirmedDeliverQuantity);
                        txtMwfArrivalDateTime.Text = deliveryDetails.OnSiteDateTime.HasValue ? deliveryDetails.OnSiteDateTime.Value.ToString("dd/MM/yy HH:mm") : string.Empty;
                        txtMwfDepartureDateTime.Text = deliveryDetails.CompleteDateTime.ToString("dd/MM/yy HH:mm");

                        var pallets = repo.GetMWFPallets(this.SavedOrder.OrderID).ToList();
                        rgMwfPallets.DataSource = pallets;
                    }

                    // Display MWF photos
                    trPhotos.Visible = false;

                    var pr = DIContainer.CreateRepository<Repositories.IPhotoRepository>(uow);

                    if (pr.GetForOrderId(this.SavedOrder.OrderID).Any())
                    {
                        trPhotos.Visible = true;
                        lnkPhotographs.NavigateUrl = String.Format("/mwf/SearchMwfPhotos.aspx?OrderId={0}", this.SavedOrder.OrderID);
                    }
                }
            }

            tabMwfDeliveryDetails.Visible = tabviewMwfDeliveryDetails.Visible = hasMwfDeliveryDetails;

            trScanLinks.Visible = phBookingForm.Visible || plcPOD.Visible || hasSignature;

            lblNominalCode.Text = GetNominalCodeDescription(this.SavedOrder);

            plcCancellation.Visible = this.SavedOrder.OrderStatus == eOrderStatus.Cancelled;
            if (this.SavedOrder.OrderStatus == eOrderStatus.Cancelled)
            {
                lblCancellationReason.Text = this.SavedOrder.CancellationReason;
                lblCancelledBy.Text = this.SavedOrder.CancelledBy;
                lblCancelledAt.Text = this.SavedOrder.CancelledAt.ToString("dd/MM/yy HH:mm");
            }

            // Set the Business Type
            if (this.SavedOrder.BusinessTypeID > 0)
            {
                cboBusinessType.ClearSelection();

                ListItem itemToRemove = null;
                foreach (ListItem item in cboBusinessType.Items)
                    if (item.Text.ToLower().Contains("please select"))
                        // Remove the "-- Please select --" option.
                        itemToRemove = item;

                if (itemToRemove != null)
                    cboBusinessType.Items.Remove(itemToRemove);

                cboBusinessType.Items.FindByValue(this.SavedOrder.BusinessTypeID.ToString()).Selected = true;
            }

            trOrderIdAndOrderStatus.Visible = true;
            this.lblOrderId.Text = this.SavedOrder.OrderID.ToString();
            this.lblOrderStatus.Text = this.SavedOrder.OrderStatus.ToString();

            // Set the Customer Identity
            
            cboClient.Text = org.OrganisationName;
            cboClient.SelectedValue = org.IdentityId.ToString();

            // Set the Load Number
            this.txtLoadNumber.Text = this.SavedOrder.CustomerOrderNumber;

            // Set the Client Order references
            Orchestrator.Facade.IOrganisationReference facOrganisationReference = new Orchestrator.Facade.Organisation();
            Orchestrator.Entities.OrganisationReferenceCollection _clientReferences = null;
            _clientReferences = facOrganisationReference.GetReferencesForOrganisationIdentityId(int.Parse(cboClient.SelectedValue), true);

            if (_clientReferences == null)
                _clientReferences = new Entities.OrganisationReferenceCollection();

            // Ensure each of the references in the client reference collection.
            foreach (Orchestrator.Entities.OrderReference reference in this.SavedOrder.OrderReferences)
                if (_clientReferences.FindByReferenceId(reference.OrganisationReference.OrganisationReferenceId) == null)
                    _clientReferences.Add(reference.OrganisationReference);

            if (_clientReferences != null)
            {
                repReferences.DataSource = _clientReferences;
                repReferences.DataBind();
            }

            //Set the Service Level
            cboService.ClearSelection();
            cboService.FindItemByValue(this.SavedOrder.OrderServiceLevelID.ToString()).Selected = true;

            // Set the pallet force service to the corresponding order service level. This will be over-ridden if existing information is present.
            cboPalletForceService.ClearSelection();
            RadComboBoxItem rcbi = cboPalletForceService.FindItemByValue(this.SavedOrder.OrderServiceLevelID.ToString());
            if (rcbi != null)
                rcbi.Selected = true;

            // Set If this is to be invoiced seperatley
            rblInvoiceSeperatley.ClearSelection();
            rblInvoiceSeperatley.Items.FindByValue(this.SavedOrder.InvoiceSeperatley.ToString().ToLower()).Selected = true;

            // Set the delivery Order Number
            this.txtDeliveryOrderNumber.Text = this.SavedOrder.DeliveryOrderNumber;

            /*
             * Basic rules for collection and delivery:
             * ----------------------------------------
             * If the from and by dates are exactly the same then the timing method is "Timed Booking".
             * If the from and by date times are midnight to 23:59 AND the from and by dates are the same day then the timing method is "Anytime" (note, this should be determined using the Anytime flag on the order entity).
             * If neither of the above apply, the timing method is booking window.
             * 
             */

            ///// COLLECTION DATES //////

            // Set the Collection Date and Is Anytime   

            dteCollectionFromDate.SelectedDate = this.SavedOrder.CollectionDateTime;
            dteCollectionByDate.SelectedDate = this.SavedOrder.CollectionByDateTime;
            dteCollectionFromTime.SelectedDate = this.SavedOrder.CollectionDateTime;
            dteCollectionByTime.SelectedDate = this.SavedOrder.CollectionByDateTime;

            if (this.SavedOrder.CollectionIsAnytime)
            {
                // Anytime
                rdCollectionIsAnytime.Checked = true;
                rdCollectionTimedBooking.Checked = false;
                rdCollectionBookingWindow.Checked = false;

                dteCollectionFromTime.Enabled = false;
                hidCollectionTimingMethod.Value = "anytime";
            }
            else if (this.SavedOrder.CollectionDateTime == this.SavedOrder.CollectionByDateTime)
            {
                // Timed booking
                rdCollectionIsAnytime.Checked = false;
                rdCollectionTimedBooking.Checked = true;
                rdCollectionBookingWindow.Checked = false;

                hidCollectionTimingMethod.Value = "timed";
            }
            else
            {
                // Booking window
                rdCollectionIsAnytime.Checked = false;
                rdCollectionTimedBooking.Checked = false;
                rdCollectionBookingWindow.Checked = true;

                hidCollectionTimingMethod.Value = "window";
            }

            if (this.SavedOrder.BookedInState == eBookedInState.NotRequired) // does not require booking in
            {
                this.chkRequiresBookIn.Checked = false;
                this.chkBookedIn.Checked = false;
            }
            else if (this.SavedOrder.BookedInState == eBookedInState.Required) // requires booking in but has not been booked in.
            {
                this.chkRequiresBookIn.Checked = true;
                this.chkBookedIn.Checked = false;
            }
            else if (this.SavedOrder.BookedInState == eBookedInState.BookedIn) // requires booking in and has been booked in.
            {
                this.chkRequiresBookIn.Checked = true;
                this.chkBookedIn.Checked = true;

                // Show the bookin details
                this.lblBookedInBy.Text = this.SavedOrder.BookedInBy + " at " + this.SavedOrder.BookedInDateTime;
                this.txtBookedInWith.Text = this.SavedOrder.BookedInWith;
                this.txtBookedInReferences.Text = this.SavedOrder.BookedInReferences;

            }

            ////////////////////////

            ////// DELIVERY DATES ////////

            dteDeliveryFromDate.SelectedDate = this.SavedOrder.DeliveryFromDateTime;
            dteDeliveryByDate.SelectedDate = this.SavedOrder.DeliveryDateTime;
            dteDeliveryByTime.SelectedDate = this.SavedOrder.DeliveryDateTime;
            dteDeliveryFromTime.SelectedDate = this.SavedOrder.DeliveryFromDateTime;

            if (this.SavedOrder.DeliveryIsAnytime)
            {
                // Anytime
                rdDeliveryIsAnytime.Checked = true;
                rdDeliveryTimedBooking.Checked = false;
                rdDeliveryBookingWindow.Checked = false;

                dteDeliveryFromTime.Enabled = false;
                hidDeliveryTimingMethod.Value = "anytime";

            }
            else if (this.SavedOrder.DeliveryFromDateTime == this.SavedOrder.DeliveryDateTime)
            {
                // Timed booking
                rdDeliveryIsAnytime.Checked = false;
                rdDeliveryTimedBooking.Checked = true;
                rdDeliveryBookingWindow.Checked = false;

                hidDeliveryTimingMethod.Value = "timed";
            }
            else
            {
                // Booking window
                rdDeliveryIsAnytime.Checked = false;
                rdDeliveryTimedBooking.Checked = false;
                rdDeliveryBookingWindow.Checked = true;

                hidDeliveryTimingMethod.Value = "window";
            }


           

            //txtDeliveryAnnotation.Text = this.SavedOrder.DeliveryAnnotation;

            ////////////////////////

            // Set the Collection Point
            point = facPoint.GetPointForPointId(this.SavedOrder.CollectionPointID);
            this.ucCollectionPoint.SelectedPoint = point;
            if (this.SavedOrder.PlannedForCollection)
            {
                if (this.SavedOrder.IsBeingInvoiced)
                    this.ucCollectionPoint.CanChangePoint = false;
            }
            else
                this.ucCollectionPoint.CanChangePoint = this.SavedOrder.CollectionPointID == this.SavedOrder.CollectionRunDeliveryPointID; // Can change the collection point if the order has not been collected.

            //Set the Delivery Point
            point = facPoint.GetPointForPointId(this.SavedOrder.DeliveryPointID);
            this.ucDeliveryPoint.SelectedPoint = point;
            this.ucDeliveryPoint.CanChangePoint = true; // Can change delivery point if the order has not been planned for delivery.

            //Set the pallet Type
            this.cboPalletType.FindItemByValue(this.SavedOrder.PalletTypeID.ToString()).Selected = true;

            //Set the cartons
            if (this.SavedOrder.Cases > 0)
                this.rntxtCartons.Value = this.SavedOrder.Cases;
            else
                this.rntxtCartons.Value = null;

            this.cboGoodsType.FindItemByValue(this.SavedOrder.GoodsTypeID.ToString()).Selected = true;

            //set the Rate
            //Set the cuture of the Order Rate to that of the order
            CultureInfo culture = new CultureInfo(this.SavedOrder.LCID);
            CultureInfo nativeCulture = new CultureInfo(Orchestrator.Globals.Configuration.NativeCulture);

            this.rntOrderRate.Culture = culture;
            if (org.Defaults.Count > 0 && org.Defaults[0].RateDecimalPlaces != 2)
                rntOrderRate.NumberFormat.DecimalDigits = org.Defaults[0].RateDecimalPlaces;

            this.rntOrderRate.Value = (double)this.SavedOrder.ForeignRate;

            //Set the exchange rate for client side calculations
            Facade.IExchangeRates facER = new Facade.ExchangeRates();
            hidExchangeRate.Value = facER.GetExchangeRate(this.SavedOrder.LCID, this.SavedOrder.CollectionDateTime).ToString();

            //Set a client side value to indicate whether native currenies
            //need to be diaplyed
            if (nativeCulture.LCID != culture.LCID)
                hidIsForeign.Value = "true";
            else
            {
                hidIsForeign.Value = "false";
            }

            this.txtRateTariffCard.Text = this.SavedOrder.TariffRateDescription;
            hidTariffRate.Value = this.SavedOrder.ForeignRate.ToString();

            hidIsAutorated.Value = this.SavedOrder.IsAutorated ? "true" : "false";
            hidTariffRateOverridden.Value = this.SavedOrder.IsTariffOverride ? "true" : "false";


            //se the Notes
            this.txtNotes.Text = this.SavedOrder.Notes;

            lnkUpdateConfidentialComments.Visible = false;
            lnkUpdateConfidentialComments.Enabled = false;

            if (orderBeingInvoiced)
            {
                btnSubmit.Enabled = false;
                btnSubmitTop.Enabled = false;
                lnkUpdateConfidentialComments.Visible = true;
                lnkUpdateConfidentialComments.Enabled = true;
                btnAddAndReset.Visible = false;
                btnAddAndResetTop.Visible = false;
                btnCreateRun.Visible = false;
                btnCreateRunTop.Visible = false;
            }

            if (this.SavedOrder.IsDelivered)
            {
                this.ucCollectionPoint.CanChangePoint = false;
            }

            //Set the Number of Pallets, Spaces and Weight
            this.rntxtPallets.Value = (double)this.SavedOrder.NoPallets;
            this.rntxtPalletSpaces.Value = (double)this.SavedOrder.PalletSpaces;
            if (this.SavedOrder.Weight > 0)
                this.rntxtWeight.Value = (double)this.SavedOrder.Weight;
            else
                this.rntxtWeight.Value = null;

            //Set the various Notes fields
            this.txtTrafficNotes.Text = this.SavedOrder.TrafficNotes;
            this.txtConfidentialComments.Text = this.SavedOrder.ConfidentialComments;

            this.txtCollectionNotes.Text = this.SavedOrder.CollectionNotes;
            this.txtDeliveryNotes.Text = this.SavedOrder.DeliveryNotes;

            //Get the BusinessType row for the selected BusinessType
            DataRow drBusinessType = this.BusinessTypeDataSet.Tables[0].Rows.Find(this.SavedOrder.BusinessTypeID);

            //Get the PalletForce fields if the BusinessType is PalletForce
            if ((bool)drBusinessType["IsPalletNetwork"] || this.SavedOrder.CustomerIdentityID == Orchestrator.Globals.Configuration.PalletNetworkID)
            {
                //Entities.VigoOrder vigoOrder = ((Facade.IVigoOrder)facOrder).GetForOrderId(this.SavedOrder.OrderID);
                EF.VigoOrder vigoOrder = EF.DataContext.Current.VigoOrderSet.Include("VigoOrderExtras.ExtraType").FirstOrDefault(v => v.OrderId == this.SavedOrder.OrderID);
                if (vigoOrder != null)
                {
                    if (vigoOrder.FullPallets > 0)
                        rntPalletForceFullPallets.Value = vigoOrder.FullPallets;
                    else
                        rntPalletForceFullPallets.Value = null;

                    if (vigoOrder.HalfPallets > 0)
                        rntPalletForceHalfPallets.Value = vigoOrder.HalfPallets;
                    else
                        rntPalletForceHalfPallets.Value = null;

                    if (vigoOrder.QtrPallets > 0)
                        rntPalletForceQtrPallets.Value = vigoOrder.QtrPallets;
                    else
                        rntPalletForceQtrPallets.Value = null;

                    if (vigoOrder.OverPallets > 0)
                        rntPalletForceOverPallets.Value = vigoOrder.OverPallets;
                    else
                        rntPalletForceOverPallets.Value = null;

                    txtTrackingNumber.Text = vigoOrder.TrackingNumber;
                    txtPalletForceNotes1.Text = vigoOrder.Note1;
                    txtPalletForceNotes2.Text = vigoOrder.Note2;
                    txtPalletForceNotes3.Text = vigoOrder.Note3;
                    txtPalletForceNotes4.Text = vigoOrder.Note4;

                    lblRequestingDepot.Text = vigoOrder.RequestingDepot;
                    lblCollectionDepot.Text = vigoOrder.CollectionDepot;
                    lblDeliveryDepot.Text = vigoOrder.DeliveryDepot;

                    //Set the Service level
                    cboPalletForceService.SelectedValue = vigoOrder.OrderServiceLevelReference.EntityKey.EntityKeyValues[0].Value.ToString();

                    //Tick the PalletForce Surcharge checkboxes for each VigoOrderExtra
                    foreach (var extra in vigoOrder.VigoOrderExtras)
                    {
                        ListItem item = cblPalletForceExtraTypes.Items.FindByValue(extra.ExtraType.ExtraTypeId.ToString());
                        if (item != null)
                            item.Selected = true;
                    }
                }
            }

            this.dteTrunkDate.SelectedDate = this.SavedOrder.TrunkDate;
            this.btnCopy.Visible = true;
            this.btnCopyTop.Visible = true;
            this.txtNumberofCopies.Visible = this.btnCopy.Visible;
            this.txtNumberofCopiesTop.Visible = this.btnCopy.Visible;
            this.trInvoiceSeperatley.Visible = true;

            this.chkOverrideFuelSurcharge.Checked = this.SavedOrder.FuelSurchargeOverridden;
            this.rntFuelSurchargePercentage.Enabled = this.SavedOrder.FuelSurchargeOverridden;
            this.hidManuallyEnteredRate.Value = "false"; // This hidden field only relates to the current session
            this.lblFuelSurcharge.Text = this.SavedOrder.FuelSurchargeForeignAmount.ToString("C", culture);

            this.hidCanCalculateFuelSurcharge.Value = this.SavedOrder.OrderStatus == eOrderStatus.Invoiced ? "false" : "true";

            Entities.OrganisationDefaultCollection defaults = facOrg.GetForIdentityId(this.SavedOrder.CustomerIdentityID).Defaults;
            if (defaults.Count == 1)
            {
                hidGlobalFuelSurchargeAppliesToExtras.Value = defaults[0].FuelSurchargeOnExtras ? "true" : "false";
                hidClientRateTariffDescription.Value = defaults[0].IncludeRateTariffCard;

                decimal fuelSurchargePerc = 0m;

                var standardFuelSurcharge = (from fs in ((Orchestrator.Base.BasePage)this.Page).DataContext.FuelSurchargeSet
                                             where fs.EffectiveDate < this.dteDeliveryByDate.SelectedDate.Value
                                             orderby fs.EffectiveDate descending
                                             select fs.SurchargeRate).FirstOrDefault();

                // Work out whether the client is set to use the standard fuel surcharge, override the standard, or adjust the standard
                switch (defaults[0].FuelSurchargeMode)
                {
                    case eOrganisationFuelSurchargeMode.Override:
                        fuelSurchargePerc = defaults[0].FuelSurchargePercentage.Value;
                        break;
                    case eOrganisationFuelSurchargeMode.Adjustment:
                        fuelSurchargePerc = standardFuelSurcharge + defaults[0].FuelSurchargeAdjustmentPercentage.Value;
                        break;
                    case eOrganisationFuelSurchargeMode.Standard:
                        fuelSurchargePerc = standardFuelSurcharge;
                        break;
                    default:
                        break;
                }

                // Set the restore value to whatever the client is set to use.
                this.hidFuelSurchargeRestoreValue.Value = fuelSurchargePerc.ToString(); //// = string.Format("{0:F2}%", fuelSurchargePerc);
            }

            // Calculate the totals for existing orders (as the javascript CalculateTotal should be disabled on load).

            // Calculate the fuel surcharge total
            decimal fsTotal = this.SavedOrder.FuelSurchargeForeignAmount + _extrasFuelSurchargeForeignAmountRunningTotal;

            if (this.SavedOrder.DeviationReasonId.HasValue)
                cboDeviationReason.SelectedValue = this.SavedOrder.DeviationReasonId.Value.ToString();

            // Calculate the grand total

            this.spnForeignTotal.InnerText = ""; // Rate + extras
            this.spnFuelSurchargeTotal.InnerText = ""; // rate fs + extras fs
            this.spnGrandTotal.InnerText = ""; // foreign total + fs total

            LoadAllocation(this.SavedOrder);
        }

        private decimal _extrasFuelSurchargeForeignAmountRunningTotal = 0m;
        private decimal _extraForeignAmountRunningTotal = 0m;

        #endregion

        #region GridEvents

        #region Commented out
        //void grdRedeliveriesRefusals_DetailTableDataBind(object source, GridDetailTableDataBindEventArgs e)
        //{
        //    GridDataItem dataItem = (GridDataItem)e.DetailTableView.ParentItem;
        //    if (e.DetailTableView.Name == "gtvRefusals")
        //    {
        //        string redeliveryIdAndOrderId = dataItem.GetDataKeyValue("RedeliveryIdAndOrderId").ToString();
        //        string[] keys = redeliveryIdAndOrderId.Split(":".ToCharArray());
        //        int redeliveryId = Convert.ToInt32(keys[0]);
        //        int orderId = Convert.ToInt32(keys[1]);

        //        var ds = (from refusal in Orchestrator.EF.DataContext.Current.GoodsRefusalSet
        //                  where refusal.Order.OrderId == orderId && refusal.Redelivery.RedeliveryId == redeliveryId
        //                  select new
        //                  {
        //                      RefusalId = refusal.RefusalId,
        //                      ProductName = refusal.ProductName,
        //                      ProductCode = refusal.ProductCode,
        //                      QuantityRefused = refusal.QuantityRefused,
        //                      QuantityOrdered = refusal.QuantityOrdered,
        //                      PackSize = refusal.PackSize,
        //                      RefusalNotes = refusal.RefusalNotes,
        //                      RefusalReceiptNumber = refusal.RefusalReceiptNumber,
        //                      TimeFrame = refusal.TimeFrame,
        //                      RedeliveryId = refusal.Redelivery.RedeliveryId,
        //                      OrderId = refusal.Order.OrderId,
        //                      JobId = refusal.Redelivery.JobId,
        //                      RefusalLocation = refusal.GoodsRefusalLocation.Description,
        //                      RefusalType = refusal.GoodsRefusalType.Description
        //                  }).ToList();

        //        var ds2 = from row in ds
        //                  select new RefusalRow
        //                  {
        //                      RefusalId = row.RefusalId,
        //                      ProductName = row.ProductName,
        //                      ProductCode = row.ProductCode,
        //                      QuantityRefused = row.QuantityRefused == null ? 0 : row.QuantityRefused.Value,
        //                      QuantityOrdered = row.QuantityOrdered == null ? 0 : row.QuantityOrdered.Value,
        //                      PackSize = row.PackSize,
        //                      RefusalNotes = row.RefusalNotes,
        //                      RefusalReceiptNumber = row.RefusalReceiptNumber,
        //                      TimeFrame = row.TimeFrame == null ? "" : row.TimeFrame.Value.ToString("dd/MM/yyyy"),
        //                      RedeliveryIdAndOrderId = row.RedeliveryId + ":" + row.OrderId,
        //                      OrderId = row.OrderId,
        //                      RedeliveryId = row.RedeliveryId,
        //                      JobId = row.JobId,
        //                      RefusalLocation = row.RefusalLocation,
        //                      RefusalType = row.RefusalType
        //                  };

        //        e.DetailTableView.DataSource = ds2;
        //    }
        //}

        //void grdRedeliveriesRefusals_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        //{
        //    var ds = (from redel in Orchestrator.EF.DataContext.Current.RedeliverySet
        //              from redelOrder in redel.RedeliveryOrders
        //              where redelOrder.OrderID == this.OrderID
        //              select new
        //              {
        //                  JobId = redel.JobId,
        //                  PointDescription = redel.Point.Description,
        //                  PointId = redel.Point.PointId,
        //                  NewBookedDateTime = redel.NewBookedDateTime,
        //                  Reason = redelOrder.RedeliveryReason.Description,
        //                  RedeliveryId = redel.RedeliveryId,
        //                  OrderId = redelOrder.OrderID,
        //                  PartialDeliveryCompletedOrderId = redelOrder.PartialDeliveryCompletedOrderId,
        //                  //NewInstructionId = redelOrder.NewInstruction.InstructionId,
        //                  CreateUser = redel.CreateUserId,
        //                  CreateDateTime = redel.CreateDate,
        //              }).ToList();


        //    var ds2 = from row in ds
        //              select new RedeliveryRow
        //              {
        //                  JobId = row.JobId,
        //                  NewBookedDateTime = row.NewBookedDateTime,
        //                  PointDescription = row.PointDescription,
        //                  PointId = row.PointId,
        //                  Reason = row.Reason,
        //                  RedeliveryIdAndOrderId = row.RedeliveryId.ToString() + ":" + row.OrderId.ToString(),
        //                  PartialDeliveryCompletedOrderId = row.PartialDeliveryCompletedOrderId == null ? 0 : row.PartialDeliveryCompletedOrderId.Value,
        //                  //NewInstructionId = row.NewInstructionId,
        //                  CreateUser = row.CreateUser,
        //                  CreateDateTime = row.CreateDateTime,
        //                  RedeliveryId = row.RedeliveryId,
        //                  OrderId = row.OrderId
        //              };

        //    this.grdRedeliveriesRefusals.DataSource = ds2;
        //}
        #endregion

        void RadGridForSubby_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is Telerik.Web.UI.GridDataItem)
            {
                DataRowView drv = e.Item.DataItem as DataRowView;
                if (drv != null)
                {
                    HtmlAnchor lnkViewInvoice = e.Item.FindControl("lnkViewInvoice") as HtmlAnchor;      //setup a HTML Anchor

                    if (lnkViewInvoice != null && drv["PDFLocation"] != DBNull.Value)
                        lnkViewInvoice.HRef = Orchestrator.Globals.Configuration.WebServer + (string)drv["PDFLocation"]; //selfbill

                    if (Orchestrator.Globals.Configuration.MultiCurrency)
                    {
                        Label lblForeignRate = e.Item.FindControl("lblForeignRate") as Label;

                        if (lblForeignRate != null && drv["ForeignRate"] != DBNull.Value)
                            lblForeignRate.Text = ((decimal)drv["ForeignRate"]).ToString("C", CurrentCulture);
                    }
                }
            }
        }

        void RadGridForSubby_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            Orchestrator.Facade.IOrder facOrder = new Orchestrator.Facade.Order();
            DataSet subbies = facOrder.GetSubcontractorDetailsFrom(OrderID);
            RadGridForSubby.DataSource = subbies;
        }

        void grdExtras_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {

            if (dsExtras == null)
            {
                Orchestrator.Facade.IOrderExtra facOrderExtra = new Orchestrator.Facade.Order();
                dsExtras = facOrderExtra.GetExtrasForOrderID(OrderID, true);
            }

            if (this.SavedOrder != null)
            {
                Orchestrator.Facade.IOrder facOrder = new Orchestrator.Facade.Order();
                IEnumerable<Repositories.DTOs.RateSurcharge> surcharges = null;

                //Get the surcharges swallowing the exception in case the Postcode in unknown
                try
                {
                    facOrder.GetRate(this.SavedOrder, true, out surcharges);
                }
                catch { }

                if (surcharges != null && surcharges.Count() > 0)
                {
                    foreach (DataRow drExtra in dsExtras.Tables[0].Rows)
                    {
                        if (drExtra["ExtraId"] == DBNull.Value)
                        {
                            int extraTypeId = (int)drExtra["ExtraTypeId"];

                            var surcharge = surcharges.SingleOrDefault(s => s.ExtraTypeID == extraTypeId);
                            if (surcharge != null)
                                drExtra["ForeignAmount"] = surcharge.ForeignRate;
                        }
                    }
                }
            }

            // get the client extras
            DataView dv = dsExtras.Tables[0].DefaultView;
            dv.RowFilter = "ExtraAppliesToID=1";
            grdExtras.DataSource = dv;
        }

        void grdNetworkExtras_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            if (dsExtras == null)
            {
                Orchestrator.Facade.IOrderExtra facOrder = new Orchestrator.Facade.Order();
                dsExtras = facOrder.GetExtrasForOrderID(OrderID, true);
            }

            // get the client extras
            DataView dv = dsExtras.Tables[0].DefaultView;
            dv.RowFilter = "ExtraAppliesToID=3";
            if (dv.Count == 0)
                pnlNetworkExtras.Visible = false;
            else
                grdNetworkExtras.DataSource = dv;
        }

        void grdExtras_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is Telerik.Web.UI.GridDataItem)
            {
                GridViewRow grv = e.Item.DataItem as GridViewRow;
                HtmlAnchor hypUpdateExtra = e.Item.FindControl("hypUpdateExtra") as HtmlAnchor;
                Telerik.Web.UI.RadNumericTextBox rntExtraForeignAmount = e.Item.FindControl("rntExtraForeignAmount") as Telerik.Web.UI.RadNumericTextBox;
                Label lblExtraFuelSurchargeAmount = e.Item.FindControl("lblExtraFuelSurchargeAmount") as Label;

                Telerik.Web.UI.GridDataItem dgi = e.Item as Telerik.Web.UI.GridDataItem;

                if ((int)((DataRowView)dgi.DataItem)["ExtraTypeId"] == (int)eExtraType.Custom)
                {
                    Telerik.Web.UI.GridDataItem gdi = e.Item as Telerik.Web.UI.GridDataItem;
                    e.Item.OwnerTableView.Columns[1].Visible = true;
                }

                if (rntExtraForeignAmount != null)
                {
                    rntExtraForeignAmount.Culture = rntOrderRate.Culture;
                    if (((DataRowView)dgi.DataItem)["ForeignAmount"] == DBNull.Value)
                        rntExtraForeignAmount.Text = string.Empty;
                    else
                    {
                        decimal ea = (decimal)((DataRowView)dgi.DataItem)["ForeignAmount"];
                        _extraForeignAmountRunningTotal += ea;
                        rntExtraForeignAmount.Value = Convert.ToDouble(ea);
                    }
                }

                if (((DataRowView)dgi.DataItem)["FuelSurchargeForeignAmount"] == DBNull.Value)
                    lblExtraFuelSurchargeAmount.Text = string.Empty;
                else
                {
                    decimal fsea = decimal.Parse((((DataRowView)dgi.DataItem)["FuelSurchargeForeignAmount"]).ToString());
                    _extrasFuelSurchargeForeignAmountRunningTotal += fsea;
                    lblExtraFuelSurchargeAmount.Text = fsea.ToString("C", this.CurrentCulture);
                }

                CheckBox chk = (CheckBox)e.Item.FindControl("chkSelectExtra");
                chk.Attributes.Add("onclick", string.Format("javascript:HandleSelection(this, {0});",
                             e.Item.ItemIndex));

                //Select the Extra Type rows that the Order already has Extras for
                if (((DataRowView)dgi.DataItem)["ExtraId"] != DBNull.Value)
                {
                    //TODO Do we still need a link to edit Extras - if so then only provide one where there is an ExtraId
                    string extraQueryString = string.Format("extraID={0}&OID={1}&lcID={2}&erID={3}",
                        (((DataRowView)dgi.DataItem)["ExtraID"]).ToString(),
                        OrderID,
                        this.SavedOrder.LCID,
                        this.SavedOrder.ExchangeRateID.HasValue ? this.SavedOrder.ExchangeRateID.Value.ToString() : "-1");

                    hypUpdateExtra.HRef = "javascript:" + dlgExtraWindow.GetOpenDialogScript(extraQueryString);

                    chk.Attributes.Add("ExtraId", (((DataRowView)dgi.DataItem)["ExtraID"]).ToString());

                    e.Item.Selected = true;
                    //e.Item.Style.Add("class", "SelectedRow_Orchestrator");     
                    e.Item.Style["class"] = "SelectedRow_Orchestrator";
                    chk.Checked = true;
                }
                else
                {
                    chk.Attributes.Add("ExtraId", "-1");
                }
            }
        }

        void grdNetworkExtras_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is Telerik.Web.UI.GridDataItem)
            {
                GridViewRow grv = e.Item.DataItem as GridViewRow;
                HtmlAnchor hypUpdateExtra = e.Item.FindControl("hypUpdateExtra") as HtmlAnchor;
                Label lblExtraForeignAmount = e.Item.FindControl("lblExtraForeignAmount") as Label;
                Telerik.Web.UI.GridDataItem dgi = e.Item as Telerik.Web.UI.GridDataItem;

                string extraQueryString = string.Format("extraID={0}&OID={1}&lcID={2}&erID={3}",
                    (((DataRowView)dgi.DataItem)["ExtraID"]).ToString(),
                    OrderID,
                    this.SavedOrder.LCID,
                    this.SavedOrder.ExchangeRateID.HasValue ? this.SavedOrder.ExchangeRateID.Value.ToString() : "-1");

                hypUpdateExtra.HRef = "javascript:" + dlgExtraWindow.GetOpenDialogScript(extraQueryString);

                if ((int)((DataRowView)dgi.DataItem)["ExtraTypeId"] == (int)eExtraType.Custom)
                {
                    Telerik.Web.UI.GridDataItem gdi = e.Item as Telerik.Web.UI.GridDataItem;
                    e.Item.OwnerTableView.Columns[1].Visible = true;
                }

                if (lblExtraForeignAmount != null && (((DataRowView)dgi.DataItem)["ForeignAmount"]) != DBNull.Value)
                    lblExtraForeignAmount.Text = ((decimal)((DataRowView)dgi.DataItem)["ForeignAmount"]).ToString("C", CurrentCulture);

            }
        }

        void grdOrders_DeleteCommand(object source, GridCommandEventArgs e)
        {
            int JobID = 0;
            JobID = int.Parse(e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["JobID"].ToString());
            Orchestrator.Facade.IJob facJob = new Orchestrator.Facade.Job();

            if (JobID > 0)
                facJob.RemoveOrder(JobID, OrderID, ((Orchestrator.Entities.CustomPrincipal)Page.User).UserName.ToString());
        }

        void grdOrders_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            Orchestrator.Facade.IOrder facOrder = new Orchestrator.Facade.Order();
            dsOrders = facOrder.GetLegsForOrder(OrderID);
            grdOrders.DataSource = dsOrders;

            // Can only cancel the order if it hasn't been attached to a job.
            if (btnCancelOrder.Visible)
            {
                btnCancelOrder.Visible = dsOrders.Tables[0].Rows.Count == 0;
                btnCancelOrderTop.Visible = btnCancelOrder.Visible;
            }
        }

        void grdOrders_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                bool canRemove =
                    Convert.ToBoolean(((System.Data.DataRowView)(e.Item.DataItem))["CanRemove"].ToString());

                if (!canRemove)
                {
                    GridDataItem gdi = e.Item as GridDataItem;
                    gdi["Remove"].Text = "";
                }

                //if (((int)((DataRowView)e.Item.DataItem)["InstructionStateID"]) == (int)eInstructionState.InProgress)
                //{
                    HtmlAnchor lnk = e.Item.FindControl("lnkCallIn") as HtmlAnchor;
                    lnk.Visible = true;
                    lnk.HRef = "javascript:CallInThis(" + ((DataRowView)e.Item.DataItem)["JobID"].ToString() + "," + ((DataRowView)e.Item.DataItem)["InstructionID"].ToString() + ");";
                //}
            }
        }

        #endregion

        #region OrderGroup

        /// <summary>
        /// Open the order group profile (preset to allow the user to add a new order).
        /// </summary>
        /// <param name="orderGroupID">The id of the order group.</param>
        /// <param name="addNewOrders">If true, when the window is opened the add orders window will be automatically opened.</param>
        private void OpenOrderGroup(int orderGroupID, bool addNewOrders)
        {
            if (orderGroupID > 0)
            {
                string openOrderGroupProfileJS =
                    string.Format("<script language=\"javascript\" type=\"text/javascript\">{0}</script>", GetOpenOrderGroupJavascript(orderGroupID, addNewOrders));
                Page.ClientScript.RegisterStartupScript(this.GetType(), "onload", openOrderGroupProfileJS);
            }
        }

        /// <summary>
        /// Retrieves the javascript to use to open the order group page.
        /// </summary>
        /// <param name="orderGroupID">The id of the order group.</param>
        /// <param name="addNewOrders">If true, when the window is opened the add orders window will be automatically opened.</param>
        /// <returns>The javascript that will cause the window to open.</returns>
        private string GetOpenOrderGroupJavascript(int orderGroupID, bool addNewOrders)
        {
            return dlgOrderGroup.GetOpenDialogScript(string.Format("OGID={0}", orderGroupID));

            //string orderGroupRelativeURL =
            //        string.Format("~/Groupage/OrderGroupProfile.aspx?ogid={0}{1}", orderGroupID, addNewOrders ? "&addOrder=true" : "");
            //string orderGroupURL = Page.ResolveClientUrl(orderGroupRelativeURL);
            //return string.Format("javascript:openResizableDialogWithScrollbars('{0}', 950, 650);", orderGroupURL);
        }

        #endregion

        #region Utilities

        private void SetFocus(string clientID)
        {
            if (!string.IsNullOrEmpty(clientID))
                litSetFocus.Text = "<script type=\"text/javascript\" language=\"javascript\">SetFocus('" + clientID + "');</script>";
            else
                litSetFocus.Text = string.Empty;
        }

        private string GetNominalCodeDescription(Entities.Order order)
        {
            string nominalCodeDescription = string.Empty;

            //Attempt to lookup the Nominal Code row based on the Nominal Code.
            //If it is a Nominal Code that has been constructed then a row will not be found.
            if (!string.IsNullOrEmpty(order.NominalCode))
            {
                Orchestrator.Facade.INominalCode facNominalCode = new Orchestrator.Facade.NominalCode();
                DataSet dsNominalCode = facNominalCode.GetForNominalCode(order.NominalCode);
                if (dsNominalCode != null && dsNominalCode.Tables.Count > 0 && dsNominalCode.Tables[0].Rows.Count == 1)
                    nominalCodeDescription = order.NominalCode + " (" + dsNominalCode.Tables[0].Rows[0]["Description"] + ")";
                else
                    nominalCodeDescription = order.NominalCode;
            }
            else
                nominalCodeDescription = "Not Set - This order's delivery has not been planned";

            return nominalCodeDescription;
        }

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

        #region Event Handlers

        #region Buttons & Links

        void btnPodLabel_Click(object sender, EventArgs e)
        {
            //Use the Pub-Sub service to Print the POD Labels
            Facade.PODLabelPrintingService podLabelPrintingService = new Facade.PODLabelPrintingService();
            bool isPrinted = podLabelPrintingService.PrintPODLabel(this.OrderID);

            string message;
            if (isPrinted)
                message = "Your POD label has been sent to the Printer";
            else
                message = "The POD Label Printing Service is not available. Please restart the service on your print server and try again.";

            Page.ClientScript.RegisterStartupScript(this.GetType(), "PrintPODLabel", "alert('" + message + "');", true);

            #region Commented out
            //NameValueCollection reportParams = new NameValueCollection();
            ////-------------------------------------------------------------------------------------	
            ////									Load Report Section 
            ////-------------------------------------------------------------------------------------	
            //reportParams.Add("OrderIdForSinglePodLabel", this.OrderID.ToString());

            //Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.PodLabel;
            //Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;

            // Show the user control
            //Page.ClientScript.RegisterStartupScript(this.GetType(), "onload", "<script language=\"javascript\">window.open('" + Page.ResolveClientUrl("../reports/reportviewer.aspx?wiz=true") + "');</script>");
            #endregion

        }

        void lnkUpdateConfidentialComments_Click(object sender, EventArgs e)
        {
            // update confidential comments field (this has a separate update button
            // because it should be possible to update it regardless of invoice status.
            Facade.IOrder facOrder = new Orchestrator.Facade.Order();
            bool result = facOrder.UpdateConfidentialComments(this.OrderID, this.txtConfidentialComments.Text, this.Page.User.Identity.Name);
            this.lblUpdatedConfidentialComments.Visible = true;

            if (result)
                lblUpdatedConfidentialComments.Text = string.Format("Saved at {0}", DateTime.Now.ToLongTimeString());
            else
                lblUpdatedConfidentialComments.Text = "Save failed";
        }

        void btnContinue_Click(object sender, EventArgs e)
        {
            eAddOrderOptions orderOptions = (eAddOrderOptions)Enum.Parse(typeof(eAddOrderOptions), this.divDuplicateOrders.Attributes["orderoptions"].ToString());

            switch (orderOptions)
            {
                case eAddOrderOptions.Grouped_Order:
                    this.AddGroupedOrder();
                    break;
                case eAddOrderOptions.Order:
                    this.SaveOrder(false);
                    break;
                case eAddOrderOptions.Order_And_Reset:
                    this.AddOrderAndReset();
                    break;
                default:
                    break;
            }

            this.divDuplicateOrders.Visible = false;
            this.hidCheckSubmit.Value = "false";
        }

        void btnSubmit_Click(object sender, EventArgs e)
        {
            if (!IsUpdate)
            {
                if (!CheckDuplicate(eAddOrderOptions.Order))
                {
                    this.SaveOrder(false);
                    this.divDuplicateOrders.Visible = false;
                }
            }
            else
            {
                Facade.IOrder facOrg = new Facade.Order();
                Entities.Order orderCheck = facOrg.GetForOrderID(this.OrderID);

                if (!(orderCheck.OrderStatus == eOrderStatus.Cancelled && this.SavedOrder.OrderStatus != eOrderStatus.Cancelled))
                {
                    this.SaveOrder(false);
                    this.divDuplicateOrders.Visible = false;
                }
                else
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "PrintPODLabel", "alert('Order NOT updated. This Order has been cancelled since this window was opened. Please close this window and reload the order to see the latest details.');", true);
            }

            this.hidCheckSubmit.Value = "false";
        }

        private bool CheckDuplicate(eAddOrderOptions orderOptions)
        {
            Facade.IOrder facOrder = new Facade.Order();
            Facade.IOrganisation facOrg = new Facade.Organisation();

            var customerIdentityID = int.Parse(cboClient.SelectedValue);
            var customerOrderNumber = txtLoadNumber.Text;
            var deliveryOrderNumber = txtDeliveryOrderNumber.Text;
            var businessTypeID = int.Parse(cboBusinessType.SelectedValue);
            var noPallets = (int)rntxtPallets.Value.Value;
            var weight = rntxtWeight.Value.HasValue ? (decimal)rntxtWeight.Value.Value : 0;

            var collectionPointID = ucCollectionPoint.PointID;
            var collectionDateTime = GetSelectedCollectionOrDeliveryDateTime(dteCollectionFromDate, dteCollectionFromTime);
            var collectionByDateTime = GetSelectedCollectionOrDeliveryDateTime(dteCollectionByDate, dteCollectionByTime);
            
            var deliveryPointID = ucDeliveryPoint.PointID;
            var deliveryFromDateTime = GetSelectedCollectionOrDeliveryDateTime(dteDeliveryFromDate, dteDeliveryFromTime);
            var deliveryDateTime = GetSelectedCollectionOrDeliveryDateTime(dteDeliveryByDate, dteDeliveryByTime);

            bool duplicateFound = false;

            var duplicates = facOrder.FindDuplicate(
                customerIdentityID,
                customerOrderNumber,
                deliveryOrderNumber,
                businessTypeID,
                deliveryPointID,
                deliveryFromDateTime,
                deliveryDateTime,
                collectionPointID,
                collectionDateTime,
                collectionByDateTime,
                noPallets,
                weight);

            if (duplicates.Any())
            {
                foreach (int id in duplicates)
                    this.duplicateOrderIds.Controls.Add(new HtmlAnchor() { HRef = "javascript:viewOrderProfile(" + id + ");", InnerHtml = id.ToString() + "<br/>" });

                string continueText;

                switch (orderOptions)
                {
                    case eAddOrderOptions.Grouped_Order:
                        continueText = "grouped order";
                        break;
                    case eAddOrderOptions.Order:
                        continueText = "order";
                        break;
                    case eAddOrderOptions.Order_And_Reset:
                        continueText = "order and reset";
                        break;
                    default:
                        continueText = "order";
                        break;
                }

                this.lblDuplicateContinue.Text = "Click \"Continue\" to add the new <b>" + continueText + "</b> anyway.";

                this.divDuplicateOrders.Attributes["orderoptions"] = orderOptions.ToString();
                this.divDuplicateOrders.Visible = true;

                duplicateFound = true;
            }

            return duplicateFound;
        }

        private void btnCreateRun_Click(object sender, EventArgs e)
        {
            SaveOrder(true);
        }

        private void SaveOrder(bool createRunAfterUpdate)
        {
            // Ensure that all points are created.
            Page.Validate("PointSavedValidation");
            bool pointsValid = Page.IsValid;

            Page.Validate("submit");
            bool orderDetailsValid = Page.IsValid;

            EF.VigoOrder vigoOrder = null;
            IEnumerable<Entities.Extra> extras = null;
            Orchestrator.Entities.Order order = PopulateOrder(out extras, out vigoOrder);

            if (orderDetailsValid && pointsValid && ucCollectionPoint.PointID > 0 && ucDeliveryPoint.PointID > 0)
            {

                Entities.FacadeResult retVal = new Entities.FacadeResult();
                Orchestrator.Facade.IOrder facOrder = new Orchestrator.Facade.Order();
                int orderGroupID = 0;
                if (!string.IsNullOrEmpty(Request.QueryString[_ogid_QS]))
                    int.TryParse(Request.QueryString[_ogid_QS], out orderGroupID);

                if (IsUpdate)
                {
                    Entities.FacadeResult res = UpdateOrder(order, vigoOrder, extras);
                    if (res.Success && createRunAfterUpdate)
                    {
                        btnCreateRun.Visible = !CreateFullLoadJob(order);
                        btnCreateRunTop.Visible = btnCreateRun.Visible;
                    }
                }
                else
                {
                    // Create the order.
                    CreateOrder(order, true, vigoOrder, extras);

                    if (OrderID > 0)
                    {
                        if (orderGroupID == 0)
                            ShowConfirmationWindow();

                        this.lblUpdateCollection.Text = String.Empty;
                        this.lblUpdateDelivery.Text = String.Empty;
                    }

                    if (orderGroupID == 0)
                    {
                        // reset 
                        this.rntxtPallets.Text = String.Empty;
                        rntPalletForceFullPallets.Text = String.Empty;
                        rntPalletForceHalfPallets.Text = String.Empty;
                        rntPalletForceOverPallets.Text = String.Empty;
                        rntPalletForceQtrPallets.Text = String.Empty;
                        this.rntxtPalletSpaces.Text = String.Empty;
                        this.rntxtWeight.Text = String.Empty;
                        this.rntxtCartons.Text = String.Empty;
                    }

                    if (Request.QueryString["returnId"] != null && Convert.ToBoolean(Request.QueryString["returnId"].ToString()))
                        (this.Page as Orchestrator.Base.BasePage).Close(OrderID.ToString());
                }
            }

            (this.Page as Orchestrator.Base.BasePage).ReturnValue = OrderID.ToString();

            this.ApplyAttributesToBusinessTypes();
            this.ReloadPalletNetworkDepots(order, vigoOrder);
        }

        private void ReloadPalletNetworkDepots(Entities.Order order, EF.VigoOrder vigoOrder)
        {
            // Pallet network depot fields can have been modified as part of the update process, so reload
            if (this.BusinessTypeDataSet.Tables[0] == null)
                return;

            // Get the BusinessType row for the selected BusinessType
            var drBusinessType = this.BusinessTypeDataSet.Tables[0].Rows.Find(order.BusinessTypeID);

            // Get the PalletForce fields if the BusinessType is PalletForce
            if (vigoOrder != null && ((bool)drBusinessType["IsPalletNetwork"] || order.CustomerIdentityID == Orchestrator.Globals.Configuration.PalletNetworkID))
            {
                lblRequestingDepot.Text = vigoOrder.RequestingDepot;
                lblCollectionDepot.Text = vigoOrder.CollectionDepot;
                lblDeliveryDepot.Text = vigoOrder.DeliveryDepot;
            }
        }

        void btnAddAndReset_Click(object sender, EventArgs e)
        {
            if (!CheckDuplicate(eAddOrderOptions.Order_And_Reset))
            {
                this.AddOrderAndReset();
                this.divDuplicateOrders.Visible = false;
            }

            this.hidCheckSubmit.Value = "false";
        }

        private void AddOrderAndReset()
        {
            // Ensure that all points are created.
            Page.Validate("PointSavedValidation");
            bool pointsValid = Page.IsValid;

            Page.Validate("submit");
            bool orderDetailsValid = Page.IsValid;

            if (orderDetailsValid && pointsValid && ucCollectionPoint.PointID > 0 && ucDeliveryPoint.PointID > 0)
            {
                //If the BusinessType is Palletforce IsVigoOrder will be set to true
                EF.VigoOrder vigoOrder = null;
                IEnumerable<Entities.Extra> extras = null;

                Orchestrator.Entities.Order order = PopulateOrder(out extras, out vigoOrder);

                if (IsUpdate)
                {
                    UpdateOrder(order, vigoOrder, extras);
                }
                else
                {
                    // Create the order.
                    CreateOrder(order, true, vigoOrder, extras);
                    if (OrderID > 0)
                    {
                        ShowConfirmationWindow();
                    }
                    // reset 
                    this.rntxtPallets.Text = String.Empty;
                    rntPalletForceFullPallets.Text = String.Empty;
                    rntPalletForceHalfPallets.Text = String.Empty;
                    rntPalletForceOverPallets.Text = String.Empty;
                    rntPalletForceQtrPallets.Text = String.Empty;
                    this.rntxtPalletSpaces.Text = String.Empty;
                    this.rntxtWeight.Text = String.Empty;
                    this.rntxtCartons.Text = String.Empty;                    

                    chkBookedIn.Checked = false;
                    lblBookedInBy.Text = string.Empty;
                    txtBookedInWith.Text = string.Empty;
                    txtBookedInReferences.Text = string.Empty;
                    chkRequiresBookIn.Checked = false;
                }

                //This page may be a dialog so tell the opener about the order it has just created or updated
                ((Orchestrator.Base.BasePage)this.Page).ReturnValue = OrderID.ToString();
                //Reload the combos and set the default values for a new Order
                PopulateStaticControls();
                ClearScreen();
            }
        }

        void btnCopy_Click(object sender, EventArgs e)
        {
            Facade.IOrder FL = new Facade.Order();
            int numberOfCopies = 1;
            if (!int.TryParse(txtNumberofCopies.Text, out numberOfCopies))
            {
                txtNumberofCopies.BackColor = System.Drawing.Color.Red;
                txtNumberofCopiesTop.BackColor = System.Drawing.Color.Red;
                return;
            }

            int orderID = 0;
            for (int i = 0; i < numberOfCopies; i++)
            {
                if (orderID == 0)
                    orderID = FL.CreateCopy(this.OrderID, Page.User.Identity.Name);
                else
                    FL.CreateCopy(this.OrderID, Page.User.Identity.Name);
            }

            Response.Redirect("/groupage/addorder.aspx?hm=1&oid=" + orderID);
        }

        private void ShowConfirmationWindow()
        {
            Telerik.Web.UI.RadWindowManager rwmOrder = this.Parent.FindControl("rwmOrder") as Telerik.Web.UI.RadWindowManager;
            Telerik.Web.UI.RadWindow wAddOrder = new Telerik.Web.UI.RadWindow();

            wAddOrder.VisibleOnPageLoad = true;
            wAddOrder.NavigateUrl = Globals.Configuration.WebServer + "/groupage/ordercreatedconf.aspx?OID=" + this.OrderID.ToString();
            wAddOrder.Title = "OrderCreated";
            wAddOrder.Visible = true;
            wAddOrder.Width = 650;
            wAddOrder.Height = 195;

            rwmOrder.Windows.Add(wAddOrder);
        }

        void btnAddGroupedOrder_Click(object sender, EventArgs e)
        {
            if (!CheckDuplicate(eAddOrderOptions.Grouped_Order))
            {
                this.AddGroupedOrder();
                this.divDuplicateOrders.Visible = false;
            }

            this.hidCheckSubmit.Value = "false";
        }

        private void AddGroupedOrder()
        {
            // Ensure that all points are created.
            Page.Validate("PointSavedValidation");
            bool pointsValid = Page.IsValid;

            Page.Validate("submit");
            bool orderDetailsValid = Page.IsValid;

            EF.VigoOrder vigoOrder = null;
            IEnumerable<Entities.Extra> surcharges = null;
            Orchestrator.Entities.Order order = PopulateOrder(out surcharges, out vigoOrder);

            if (orderDetailsValid && pointsValid && ucCollectionPoint.PointID > 0 && ucDeliveryPoint.PointID > 0)
            {
                // Create the order.
                CreateOrder(order, false, vigoOrder, surcharges);

                // Create a group for this order.
                Facade.IOrderGroup facOrderGroup = new Facade.Order();
                int orderGroupID =
                    facOrderGroup.Create(OrderID, Globals.Configuration.GroupageGroupedPlanning,
                                           string.Empty, string.Empty,
                                         ((Entities.CustomPrincipal)Page.User).UserName);

                // Open the order group profile (preset to allow the user to add a new order).
                if (orderGroupID > 0)
                {
                    string openOrderGroupProfileJS = dlgOrderGroup.GetOpenDialogScript(string.Format("OGID={0}&addOrder=true", orderGroupID));
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "onload", openOrderGroupProfileJS, true);
                }
            }

            this.ApplyAttributesToBusinessTypes();
        }

        void btnCreateDeliveryNote_Click(object sender, EventArgs e)
        {
            NameValueCollection reportParams = new NameValueCollection();
            DataSet dsDelivery = null;

            Orchestrator.Facade.IOrder facOrder = new Orchestrator.Facade.Order();

            if (OrderID != -1)
            {
                dsDelivery = facOrder.GetDeliveryNoteDataForOrderIDs(OrderID.ToString());

                //-------------------------------------------------------------------------------------	
                //									Load Report Section 
                //-------------------------------------------------------------------------------------	
                Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.DeliveryNote;
                Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;
                Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = dsDelivery;
                Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
                Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";

                // Show the user control
                Page.ClientScript.RegisterStartupScript(this.GetType(), "onload", "<script language=\"javascript\">window.open('" + Page.ResolveClientUrl("../reports/reportviewer.aspx?wiz=true") + "');</script>");
            }
        }

        void btnCreatePIL_Click(object sender, EventArgs e)
        {
            NameValueCollection reportParams = new NameValueCollection();
            DataSet dsPIL = null;

            Orchestrator.Facade.ICollectDropLoadOrder facLoadOrder = new Orchestrator.Facade.CollectDrop();

            if (OrderID != -1)
            {
                #region Pop-up Report
                dsPIL = facLoadOrder.GetPILData(OrderID.ToString());

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

        void btnUnFlagForInvoicing_Click(object sender, EventArgs e)
        {
            Orchestrator.Facade.IOrder facOrder = new Orchestrator.Facade.Order();
            if (OrderID > 0)
            {
                facOrder.UnflagForInvoicing(OrderID);
                this.ViewState["_order"] = null;
                DisplayOrder();
            }
        }

        void btnPIL_Click(object sender, EventArgs e)
        {
            NameValueCollection reportParams = new NameValueCollection();
            DataSet dsPIL = null;

            Orchestrator.Facade.ICollectDropLoadOrder facLoadOrder = new Orchestrator.Facade.CollectDrop();

            if (OrderID > 0)
            {
                #region Pop-up Report

                eReportType reportType = eReportType.PIL;
                dsPIL = facLoadOrder.GetPILData(OrderID.ToString());
                DataView dvPIL;

                if ((bool)dsPIL.Tables[0].Rows[0]["IsPalletNetwork"])
                {
                    reportType = Globals.Configuration.PalletNetworkLabelID; ;

                    //Need to duplicate the rows for the Pallteforce labels
                    dsPIL.Tables[0].Merge(dsPIL.Tables[0].Copy(), true);
                    dvPIL = new DataView(dsPIL.Tables[0], string.Empty, "OrderId, PalletCount", DataViewRowState.CurrentRows);
                }
                else
                {
                    dvPIL = new DataView(dsPIL.Tables[0]);
                }

                //-------------------------------------------------------------------------------------	
                //									Load Report Section 
                //-------------------------------------------------------------------------------------	
                Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = reportType;
                Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;
                Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = dvPIL;
                Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
                Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";

                // Show the user control
                Page.ClientScript.RegisterStartupScript(this.GetType(), "onload", "<script language=\"javascript\">window.open('" + Page.ResolveClientUrl("../reports/reportviewer.aspx?wiz=true") + "');</script>");
                #endregion
            }
        }

        void btnMakeInvoiceable_Click(object sender, EventArgs e)
        {
            Orchestrator.Facade.IOrder facOrder = new Orchestrator.Facade.Order();
            
            if (OrderID > 0)
            {
                bool result = facOrder.MakeInvoiceable(OrderID, ((Orchestrator.Entities.CustomPrincipal)Page.User).UserName);
                this.ViewState["_order"] = null;
                DisplayOrder();
            }
        }

        void lnkCreateGroup_Click(object sender, EventArgs e)
        {
            // Create a group for this order.
            Orchestrator.Facade.IOrderGroup facOrderGroup = new Orchestrator.Facade.Order();
            int orderGroupID =
                facOrderGroup.Create(OrderID, Orchestrator.Globals.Configuration.GroupageGroupedPlanning,
                                        string.Empty, string.Empty,
                                     ((Orchestrator.Entities.CustomPrincipal)Page.User).UserName);

            OpenOrderGroup(orderGroupID, false);
            DisplayOrder();
        }

        void lnkReset_Click(object sender, EventArgs e)
        {
            // Clear all of the fields.
            Response.Redirect(Request.Url.AbsoluteUri);
        }

        #endregion

        #region Drop Down Boxes

        void cboClient_SelectedIndexChanged(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if (((Telerik.Web.UI.RadComboBox)(o)).SelectedValue == "")
                return;

            Facade.IOrganisation facOrg = new Facade.Organisation();

            ConfigureClientReferences();

            //this.CurrentCulture = new CultureInfo(facOrg.GetCultureForOrganisation(int.Parse(((Telerik.Web.UI.RadComboBox)(o)).Value)));
            CultureInfo culture = null;

            int customerIdentityId = ((Telerik.Web.UI.RadComboBox)(o)).SelectedValue == "" ? 0 : Convert.ToInt32(((Telerik.Web.UI.RadComboBox)(o)).SelectedValue);

            CultureInfo nativeCulture = new CultureInfo(Orchestrator.Globals.Configuration.NativeCulture);
            if (customerIdentityId == 0)
                culture = nativeCulture;
            else
                culture = new CultureInfo(facOrg.GetCultureForOrganisation(int.Parse(((Telerik.Web.UI.RadComboBox)(o)).SelectedValue)));

            //If the culture has changed use the new culture for the rate and the Extras Amounts
            //and get the exchange rate for client side conversions
            if (culture.LCID != rntOrderRate.Culture.LCID)
            {
                rntOrderRate.Culture = culture;


                DateTime effectiveDate = DateTime.Now;
                if (dteCollectionFromDate.SelectedDate.HasValue)
                    effectiveDate = dteCollectionFromDate.SelectedDate.Value;
                Facade.IExchangeRates facER = new Facade.ExchangeRates();

                hidExchangeRate.Value = facER.GetExchangeRate(culture.LCID, effectiveDate).ToString();

                grdExtras.Rebind();
            }

            //Set a client side value to indicate whether native currenies
            //need to be diaplyed
            if (nativeCulture.LCID != culture.LCID)
                hidIsForeign.Value = "true";
            else
                hidIsForeign.Value = "false";

            // Populate pallet types and Goods Type combos
            LoadPalletTypes();
            LoadGoodsTypes();

            Entities.OrganisationDefaultCollection defaults = facOrg.GetForIdentityId(int.Parse(((Telerik.Web.UI.RadComboBox)(o)).SelectedValue)).Defaults;

            // Set the Default Goods Type for the client if we are not reloading an order
            if (this.OrderID == 0 && customerIdentityId != 0)
            {
                if (defaults.Count == 1)
                {
                    rntOrderRate.NumberFormat.DecimalDigits = defaults[0].RateDecimalPlaces;

                    hidClientRateTariffDescription.Value = defaults[0].IncludeRateTariffCard;

                    if (defaults[0].DefaultGoodsTypeId != 0)
                        cboGoodsType.FindItemByValue(defaults[0].DefaultGoodsTypeId.ToString()).Selected = true;

                    if (defaults[0].DefaultNumberOfPallets != null)
                    {
                        rntxtPallets.Text = defaults[0].DefaultNumberOfPallets.ToString();
                        rntxtPalletSpaces.Text = defaults[0].DefaultNumberOfPallets.ToString();
                    }
                    else
                    {
                        rntxtPallets.Text = string.Empty;
                        rntxtPalletSpaces.Text = string.Empty;
                    }

                    if (defaults[0].DefaultServiceLevelID > 0)
                    {
                        cboService.ClearSelection();
                        RadComboBoxItem item = cboService.Items.FindItemByValue(defaults[0].DefaultServiceLevelID.ToString());
                        if (item != null)
                            item.Selected = true;
                    }

                    if (defaults[0].DefaultBusinessTypeID > 0)
                    {
                        cboBusinessType.ClearSelection();
                        cboBusinessType.Items.FindByValue(defaults[0].DefaultBusinessTypeID.ToString()).Selected = true;
                    }
                    else
                    {
                        cboBusinessType.ClearSelection();
                        cboBusinessType.Items[0].Selected = true;
                    }

                    chkCreateJob.Style.Add("display", (cboBusinessType.SelectedItem.Attributes["showcreatejob"] == "true") ? String.Empty : "none");
                    chkCreateJob.Checked = (cboBusinessType.SelectedItem.Attributes["createjobchecked"] == "true");

                    if (defaults[0].OrganisationPalletTypes.Count > 0)
                    {
                        if (defaults[0].OrganisationPalletTypes.Count == 1)
                        {
                            cboPalletType.ClearSelection();
                            cboPalletType.Items.FindItemByValue(defaults[0].OrganisationPalletTypes[0].PalletTypeId.ToString()).Selected = true;
                        }
                        else
                        {
                            // Loop through and find the default pallet type for the organisation
                            if (defaults[0].OrganisationPalletTypes.FirstOrDefault(pt => pt.IsDefault == true) != null)
                            {
                                cboPalletType.ClearSelection();
                                cboPalletType.Items.FindItemByValue(defaults[0].OrganisationPalletTypes.FirstOrDefault(pt => pt.IsDefault == true).PalletTypeId.ToString()).Selected = true;
                            }
                        }
                    }

                }
                else
                {
                    hidClientRateTariffDescription.Value = "";
                }
            }
            else
            {
                hidClientRateTariffDescription.Value = defaults[0].IncludeRateTariffCard;

                cboGoodsType.ClearSelection();
                cboGoodsType.Items[0].Selected = true;
            }


            if (customerIdentityId != 0)
            {
                if (defaults.Count == 1)
                {
                    DateTime effectiveDeliveryDate = DateTime.Now;
                    if (dteDeliveryByDate.SelectedDate.HasValue)
                        effectiveDeliveryDate = dteDeliveryByDate.SelectedDate.Value;

                    Facade.Order facOrder = new Orchestrator.Facade.Order();
                    decimal fuelSurchargePerc = facOrder.GetFuelSurchargePercentageClientSettingsForIdentitId(customerIdentityId, effectiveDeliveryDate);

                    hidGlobalFuelSurchargeAppliesToExtras.Value = defaults[0].FuelSurchargeOnExtras ? "true" : "false";

                    if (chkOverrideFuelSurcharge.Checked)
                    {
                        this.rntFuelSurchargePercentage.Enabled = true;

                        // The client has changed and we're OVERRIDDING fuel surcharge
                        this.hidFuelSurchargeRestoreValue.Value = fuelSurchargePerc.ToString(); //// = string.Format("{0:F2}%", fuelSurchargePerc);
                    }
                    else
                    {
                        this.rntFuelSurchargePercentage.Enabled = false;

                        // We're NOT overridding fuel surcharge.
                        decimal fuelSurchargeAmount = 0m;
                        rntFuelSurchargePercentage.Value = Convert.ToDouble(fuelSurchargePerc); //// = string.Format("{0:F2}%", fuelSurchargePerc);
                        decimal rate = Convert.ToDecimal(rntOrderRate.Value);
                        decimal percAmount = (rate / 100.0m) * fuelSurchargePerc;

                        fuelSurchargeAmount = percAmount;
                        lblFuelSurcharge.Text = fuelSurchargeAmount.ToString("C", culture);
                        hidFuelSurchargeRestoreValue.Value = rntFuelSurchargePercentage.Value.Value.ToString();
                    }
                }

                if (Globals.Configuration.AlertForMissingClientDocuments)
                {
                    AlertForMissingClientDocuments(customerIdentityId);
                }
            }

            if (this.OrderID == 0 && customerIdentityId != 0)
                SetDefaultCollectionPoint();    // Always set the default collection point when adding an order and the client changes
            else if (ucCollectionPoint.SelectedPoint == null || ucCollectionPoint.SelectedPoint.PointId == 0)
                SetDefaultCollectionPoint();    // This is an existing order thus only set the default collection point if 

            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "service", "<script type='text/javascript'>updateDeliveryDateForServiceLevel(" + cboService.SelectedValue + ")</script>", false);
            ScriptManager.RegisterStartupScript(this.Page, this.GetType(), "pforce", "<script type='text/javascript'>setPalletForceService(" + cboService.SelectedValue + ")</script>", false);
        }

        private void AlertForMissingClientDocuments(int customerIdentityId)
        {
            //Get the OrganisationDocuments for this Org
            List<EF.OrganisationDocument> orgDocs = (
                from od in EF.DataContext.Current.OrganisationDocuments.Include("ScannedForm.FormType")
                where od.Organisation.IdentityId == customerIdentityId
                select od).ToList();

            string missingDocAlert = string.Empty;

            if (!orgDocs.Any(od => od.ScannedForm.FormType.FormTypeId == (int)eFormTypeId.ClientTnCs))
                missingDocAlert += "The Client T&Cs document has not been scanned.<br />";

            if (!orgDocs.Any(od => od.ScannedForm.FormType.FormTypeId == (int)eFormTypeId.CreditApplicationForm))
                missingDocAlert += "The Credit Application Form has not been scanned.<br />";

            lblMissingDocumentsAlert.Text = missingDocAlert;
        }

        void cboClient_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            cboClient.Items.Clear();

            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            // Stephen Newman 29/08/07
            // Do not return suspended clients.
            // If this is an update, it is possible the order was created for a suspended client in which case don't trap the user.
            bool returnSuspendedClients = !string.IsNullOrEmpty(Request.QueryString["oid"]);
            DataSet ds = facRefData.GetAllClientsFiltered("%" + e.Text, returnSuspendedClients);

            int itemsPerRequest = 15;
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

        void rcbPalletSpaces_ItemDataBound(object o, Telerik.Web.UI.RadComboBoxItemEventArgs e)
        {
            Telerik.Web.UI.RadComboBoxItem rcbi = e.Item as Telerik.Web.UI.RadComboBoxItem;

            using (rcbi)
            {
                DataRowView drv = rcbi.DataItem as DataRowView;


                rcbi.Attributes["PalletSpaceValue"] = ((decimal)drv["Value"]).ToString();

                switch (((decimal)drv["Value"]).ToString())
                {
                    case "0.25":
                        rcbi.Text = "Qtr";
                        break;
                    case "0.50":
                        rcbi.Text = "Half";
                        break;
                    default:
                        break;
                }
            }
        }

        #endregion

        #region Commented out
        //void rblInvolvement_PreRender(object sender, EventArgs e)
        //{
        //    RadioButtonList rbl = (RadioButtonList)sender;

        //    foreach (ListItem item in rbl.Items)
        //    {
        //        item.Attributes.Add("onClick", "javascript:ReRate();");
        //    }
        //}
        #endregion

        #endregion

        #region Entity Handling

        private Entities.Order PopulateOrder(out IEnumerable<Entities.Extra> extras, out EF.VigoOrder vigoOrder)
        {
            Facade.IOrder facOrder = new Facade.Order();

            var customerIdentityID = int.Parse(cboClient.SelectedValue);
            var businessTypeID = int.Parse(cboBusinessType.SelectedValue);

            var collectionPointID = ucCollectionPoint.PointID;
            var collectionDateTime = GetSelectedCollectionOrDeliveryDateTime(dteCollectionFromDate, dteCollectionFromTime);
            var collectionByDateTime = GetSelectedCollectionOrDeliveryDateTime(dteCollectionByDate, dteCollectionByTime);

            var deliveryPointID = ucDeliveryPoint.PointID;
            var deliveryFromDateTime = GetSelectedCollectionOrDeliveryDateTime(dteDeliveryFromDate, dteDeliveryFromTime);
            var deliveryDateTime = GetSelectedCollectionOrDeliveryDateTime(dteDeliveryByDate, dteDeliveryByTime);

            int deviationReasonID;
            int.TryParse(this.cboDeviationReason.SelectedValue, out deviationReasonID);

            var cp = Page.User as Entities.CustomPrincipal;
            var isClientUser = cp.IsInRole(((int)eUserRole.ClientUser).ToString());

            this.CollectionPointUpdated = this.SavedOrder == null || collectionPointID != this.SavedOrder.CollectionPointID;
            this.DeliveryPointUpdated = this.SavedOrder == null || deliveryPointID != this.SavedOrder.DeliveryPointID;

            var orderReferences = repReferences.Items.Cast<RepeaterItem>().ToDictionary(
                i => Convert.ToInt32(((HtmlInputHidden)i.FindControl("hidOrganisationReferenceId")).Value),
                i => ((TextBox)i.FindControl("txtReferenceValue")).Text);

            extras = GetSelectedExtras(collectionDateTime);

            #region Re-Retrieve fuel surcharge percentage in case it's changed since opening the Add order window

            Facade.IOrganisation facOrg = new Orchestrator.Facade.Organisation();
            Entities.OrganisationDefaultCollection defaults = facOrg.GetForIdentityId(customerIdentityID).Defaults;
            var culture = new CultureInfo(facOrg.GetCultureForOrganisation(customerIdentityID));

            if (defaults.Count == 1 && this.hidCanCalculateFuelSurcharge.Value.ToLower() == "true")
            {
                decimal fuelSurchargePerc = facOrder.GetFuelSurchargePercentageClientSettingsForIdentitId(customerIdentityID, deliveryDateTime.Date);

                hidGlobalFuelSurchargeAppliesToExtras.Value = defaults[0].FuelSurchargeOnExtras.ToString().ToLower();

                if (chkOverrideFuelSurcharge.Checked)
                {
                    this.rntFuelSurchargePercentage.Enabled = true;

                    // The client has changed and we're OVERRIDDING fuel surcharge
                    this.hidFuelSurchargeRestoreValue.Value = fuelSurchargePerc.ToString();
                }
                else
                {
                    this.rntFuelSurchargePercentage.Enabled = false;

                    // We're NOT overridding fuel surcharge.
                    decimal fuelSurchargeAmount = 0m;
                    rntFuelSurchargePercentage.Value = Convert.ToDouble(fuelSurchargePerc);
                    decimal orate = Convert.ToDecimal(rntOrderRate.Value);
                    decimal percAmount = (orate / 100.0m) * fuelSurchargePerc;

                    fuelSurchargeAmount = percAmount;
                    lblFuelSurcharge.Text = fuelSurchargeAmount.ToString("C", culture);
                    hidFuelSurchargeRestoreValue.Value = rntFuelSurchargePercentage.Value.Value.ToString();
                }
            }

            #endregion

            Orchestrator.Entities.Order order = null;
            vigoOrder = null;

            if (this.SavedOrder != null)
            {
                order = this.SavedOrder;
                vigoOrder = EF.DataContext.Current.VigoOrderSet.Include("VigoOrderExtras.ExtraType").FirstOrDefault(v => v.OrderId == order.OrderID);
            }
            else
            {
                order = new Orchestrator.Entities.Order
                {
                    OrderInstructionID = this.OrderInstructionID,
                    OrderType = eOrderType.Normal,
                };
            }

            if (vigoOrder == null)
            {
                vigoOrder = new EF.VigoOrder();
                EF.DataContext.Current.AddToVigoOrderSet(vigoOrder);
            }

            #region Vigo Order

            // Get the BusinessType row for the selected BusinessType
            DataRow drBusinessType = this.BusinessTypeDataSet.Tables[0].Rows.Find(businessTypeID);

            // Get the PalletForce fields if the BusinessType is PalletForce
            if (drBusinessType.Field<bool>("IsPalletNetwork"))
            {
                Facade.IVigoOrder facVigoOrder = new Facade.Order();

                facVigoOrder.PopulateVigoOrder(
                    vigoOrder,
                    this.SavedOrder == null,
                    (int?)rntPalletForceFullPallets.Value ?? 0,
                    (int?)rntPalletForceHalfPallets.Value ?? 0,
                    (int?)rntPalletForceQtrPallets.Value ?? 0,
                    (int?)rntPalletForceOverPallets.Value ?? 0,
                    int.Parse(cboPalletForceService.SelectedValue),
                    businessTypeID,
                    ucCollectionPoint.Depot,
                    ucDeliveryPoint.Depot,
                    ucDeliveryPoint.CountryCode,
                    deliveryFromDateTime,
                    deliveryDateTime,
                    txtPalletForceNotes1.Text,
                    txtPalletForceNotes2.Text,
                    txtPalletForceNotes3.Text,
                    txtPalletForceNotes4.Text);

                // Make a list of the selected PalletForce Extras
                var selectedExtraTypeIds = new List<int>();

                foreach (ListItem item in cblPalletForceExtraTypes.Items)
                {
                    var extraTypeId = int.Parse(item.Value);

                    if (item.Selected)
                        selectedExtraTypeIds.Add(extraTypeId);
                    else
                    {
                        var vigoOrderExtra = vigoOrder.VigoOrderExtras.FirstOrDefault(voe => voe.ExtraType.ExtraTypeId == extraTypeId);

                        if (vigoOrderExtra != null)
                            EF.DataContext.Current.DeleteObject(vigoOrderExtra);
                    }
                }

                // Remove the selected extras which are already present
                foreach (var extra in vigoOrder.VigoOrderExtras)
                    selectedExtraTypeIds.Remove(extra.ExtraType.ExtraTypeId);

                // Now add Extras for the selected extras remaining
                foreach (int extraTypeId in selectedExtraTypeIds)
                {
                    var vigoOrderExtra = new EF.VigoOrderExtra();
                    vigoOrderExtra.ExtraType = EF.DataContext.Current.ExtraTypeSet.First(et => et.ExtraTypeId == extraTypeId);
                    vigoOrder.VigoOrderExtras.Add(vigoOrderExtra);
                }
            }
            else
                vigoOrder = null;

            #endregion Vigo Order

            if (plcRate.Visible)
            {
                // Make sure the correct rate information is used.
                CheckRateInformation(vigoOrder, false);
            }

            facOrder.PopulateOrder(
                order,
                customerIdentityID,
                businessTypeID,
                txtLoadNumber.Text,
                txtDeliveryOrderNumber.Text,
                (int)rntxtPallets.Value.Value,
                rntxtWeight.Value.HasValue ? (decimal)rntxtWeight.Value.Value : 0,
                int.Parse(cboGoodsType.SelectedValue),
                txtNotes.Text,
                txtTrafficNotes.Text,
                txtConfidentialComments.Text,
                txtCollectionNotes.Text,
                txtDeliveryNotes.Text,
                rntxtCartons.Value.HasValue ? (int)rntxtCartons.Value.Value : 0,
                int.Parse(cboService.SelectedValue),
                (decimal)rntxtPalletSpaces.Value.Value,
                int.Parse(cboPalletType.SelectedValue),
                collectionPointID,
                collectionDateTime,
                collectionByDateTime,
                deliveryPointID,
                deliveryFromDateTime,
                deliveryDateTime,
                true,
                dteTrunkDate.SelectedDate,
                true,
                chkRequiresBookIn.Checked,
                chkBookedIn.Checked,
                txtBookedInWith.Text,
                txtBookedInReferences.Text,
                rntOrderRate.Value.HasValue ? (decimal)rntOrderRate.Value.Value : 0m,
                rntOrderRate.Culture.LCID,
                plcRate.Visible,
                hidTariffRateOverridden.Value.ToLower() == "true",
                hidManuallyEnteredRate.Value.ToLower() == "true",
                hidIsAutorated.Value.ToLower() == "true",
                txtRateTariffCard.Text,
                false,
                Convert.ToDecimal(rntFuelSurchargePercentage.Value),
                chkOverrideFuelSurcharge.Checked,
                bool.Parse(rblInvoiceSeperatley.SelectedValue),
                orderReferences,
                extras,
                isClientUser,
                Page.User.Identity.Name);

            // deviation reason code was not being saved.
            if (deviationReasonID > 0)
                order.DeviationReasonId = deviationReasonID;

            return order;
        }

        private static void FacadeBuildOrder(
            Entities.Order order,
            int customerIdentityID,
            int businessTypeID,
            string customerOrderNumber,
            string deliveryOrderNumber,
            int noPallets,
            decimal weight,
            int goodsTypeID,
            string notes,
            string trafficNotes,
            string confidentialComments,
            string collectionNotes,
            string deliveryNotes,
            int cases,
            int serviceLevelID,
            decimal palletSpaces,
            int palletTypeID,
            int collectionPointID,
            DateTime collectionDateTime,
            DateTime collectionByDateTime,
            int deliveryPointID,
            DateTime deliveryFromDateTime,
            DateTime deliveryDateTime,
            bool canSetTrunkDate,
            DateTime? trunkDate,
            bool canSetBookingIn,
            bool requiresBookingIn,
            bool isBookedIn,
            string bookedInWith,
            string bookedInReferences,
            decimal rate,
            int lcid,
            bool canSetTariffOverride,
            bool isTariffRateOverridden,
            bool isManuallyEnteredRate,
            bool isAutoRated,
            string rateTariffCard,
            bool useClientDefaultFuelSurcharge,
            decimal fuelSurchargePercentage,
            bool fuelSurchargeOverridden,
            bool invoiceSeparately,
            IDictionary<int, string> orderReferences,
            IEnumerable<Entities.Extra> extras,
            bool isClientUser,
            string userID)
        {
            order.CustomerIdentityID = customerIdentityID;
            order.BusinessTypeID = businessTypeID;
            order.CustomerOrderNumber = customerOrderNumber;
            order.DeliveryOrderNumber = deliveryOrderNumber;

            order.CollectionPointID = collectionPointID;
            order.CollectionDateTime = collectionDateTime;
            order.CollectionByDateTime = collectionByDateTime;

            order.DeliveryPointID = deliveryPointID;
            order.DeliveryFromDateTime = deliveryFromDateTime;
            order.DeliveryDateTime = deliveryDateTime;

            order.GoodsTypeID = goodsTypeID;
            order.Notes = notes;
            order.Cases = cases;

            if (order.OrderID == 0)
                order.OrderStatus = isClientUser ? eOrderStatus.Awaiting_Approval : eOrderStatus.Approved;

            // Add any custom references based on the client selected.
            int referenceID = 0;
            string referenceValue = string.Empty;

            foreach (var kvp in orderReferences)
            {
                referenceID = kvp.Key;
                referenceValue = kvp.Value;

                OrderReference orderRef = null;

                foreach (var existingOrderReference in order.OrderReferences)
                {
                    if (existingOrderReference.OrganisationReference.OrganisationReferenceId == referenceID)
                        orderRef = existingOrderReference;
                }

                if (orderRef == null)
                {
                    orderRef = new OrderReference { OrganisationReference = new OrganisationReference { OrganisationReferenceId = referenceID } };
                    order.OrderReferences.Add(orderRef);
                }

                orderRef.Reference = referenceValue;
            }

            order.OrderServiceLevelID = serviceLevelID;
            order.InvoiceSeperatley = invoiceSeparately;
            order.NoPallets = noPallets;
            order.PalletSpaces = palletSpaces;
            order.Weight = weight;
            order.PalletTypeID = palletTypeID;
            order.PalletSpaceID = 1; // Obsolete property

            order.TrafficNotes = trafficNotes;
            order.ConfidentialComments = confidentialComments;
            order.CollectionNotes = collectionNotes;
            order.DeliveryNotes = deliveryNotes;

            // Booking In
            if (canSetBookingIn)
            {
                if (requiresBookingIn && isBookedIn)
                {
                    if (order.BookedInState == eBookedInState.BookedIn)
                    {
                        if (order.BookedInReferences != bookedInReferences)
                            order.BookedInReferences = bookedInReferences;

                        if (order.BookedInWith != bookedInWith)
                            order.BookedInWith = bookedInWith;
                    }
                    else
                    {
                        order.BookedInState = eBookedInState.BookedIn;

                        if (string.IsNullOrWhiteSpace(order.BookedInBy))
                        {
                            order.BookedInBy = userID;
                            order.BookedInWith = bookedInWith;
                            order.BookedInReferences = bookedInReferences;
                            order.BookedInDateTime = order.BookedInDateTime.HasValue == false ? DateTime.Now : (DateTime?)null;
                        }
                    }
                }
                else if (requiresBookingIn)
                    order.BookedInState = eBookedInState.Required;
                else
                    order.BookedInState = eBookedInState.NotRequired;
            };

            if (canSetTrunkDate)
                order.TrunkDate = trunkDate;

            // Tariff Override
            if (canSetTariffOverride)
            {
                if (isTariffRateOverridden)
                {
                    order.IsTariffOverride = true;

                    if (isManuallyEnteredRate)
                    {
                        // The user has overridden the rate unless the rate has not changed since it was first overridden (in the case of an update).
                        order.TariffOverrideUserID = userID;
                        order.TariffOverrideDate = DateTime.Now;
                    }
                }
                else
                {
                    // If the order was previously overridden then it should now be marked as "NOT" overridden.
                    if (order.IsTariffOverride)
                    {
                        order.IsTariffOverride = false;
                        order.TariffOverrideDate = null;
                        order.TariffOverrideUserID = string.Empty;
                    }
                }
            }

            if (!isClientUser)
            {
                order.TariffRateDescription = rateTariffCard;
                order.IsAutorated = isAutoRated;
            }

            order.ForeignRate = rate;
            order.LCID = lcid;

            Facade.IExchangeRates facER = new Facade.ExchangeRates();

            if (order.LCID != new CultureInfo(Orchestrator.Globals.Configuration.NativeCulture).LCID)
            {
                order.ExchangeRateID = facER.GetCurrentExchangeRateID(Facade.Culture.GetCurrencySymbol(order.LCID), order.CollectionDateTime);
                order.Rate = facER.GetConvertedRate((int)order.ExchangeRateID, order.ForeignRate);

                if (extras != null)
                {
                    foreach (var extra in extras)
                    {
                        extra.ExtraAmount = facER.GetConvertedRate(order.ExchangeRateID.Value, extra.ForeignAmount);
                        extra.ExchangeRateID = order.ExchangeRateID.Value;
                        extra.LCID = order.LCID;
                    }
                }
            }
            else
            {
                order.Rate = decimal.Round(order.ForeignRate, 4, MidpointRounding.AwayFromZero);
                order.ExchangeRateID = null; // Otherwise, updating the customer from foreign to domestic will retain the foreign Exchange Rate ID
            }

            if (useClientDefaultFuelSurcharge)
            {
                if (order.OrderID == 0)
                {
                    // Set the fuel surcharge on the order according to the client's settings
                    Facade.Order facOrder = new Facade.Order();
                    facOrder.SetOrderFuelSurcharge(order);
                }
            }
            else
            {
                decimal fuelSurchargeForeignAmount = (fuelSurchargePercentage / 100.0m) * order.ForeignRate;
                order.FuelSurchargeForeignAmount = Math.Round(fuelSurchargeForeignAmount, 4, MidpointRounding.AwayFromZero);
                order.FuelSurchargePercentage = fuelSurchargePercentage;

                BusinessLogicLayer.IExchangeRates blER = new BusinessLogicLayer.ExchangeRates();
                BusinessLogicLayer.CurrencyConverter currencyConverter = blER.CreateCurrencyConverter(order.LCID, order.CollectionDateTime);
                // Convert rounded value so that UK amounts match
                order.FuelSurchargeAmount = currencyConverter.ConvertToLocal(order.FuelSurchargeForeignAmount);

                order.FuelSurchargeOverridden = fuelSurchargeOverridden;
            }
        }

        private static DateTime GetSelectedCollectionOrDeliveryDateTime(RadDatePicker datePicker, RadTimePicker timePicker)
        {
            return datePicker.SelectedDate.Value.Date.Add(
                new TimeSpan(
                    timePicker.SelectedDate.Value.Hour,
                    timePicker.SelectedDate.Value.Minute,
                    0)
            );
        }

        private IEnumerable<Extra> GetSelectedExtras(DateTime collectionDateTime)
        {
            var extras = new List<Extra>();

            CultureInfo nativeCulture = new CultureInfo(Orchestrator.Globals.Configuration.NativeCulture);

            BusinessLogicLayer.ExchangeRates blER = new Orchestrator.BusinessLogicLayer.ExchangeRates();
            BusinessLogicLayer.CurrencyConverter currencyConverter = blER.CreateCurrencyConverter(rntOrderRate.Culture.LCID, collectionDateTime);

            foreach (GridDataItem row in grdExtras.Items)
            {
                CheckBox chk = row.FindControl("chkSelectExtra") as CheckBox;
                if (chk.Checked)
                {
                    RadNumericTextBox rntExtraForeignAmount = row.FindControl("rntExtraForeignAmount") as RadNumericTextBox;
                    Label lblClientContact = row.FindControl("lblClientContact") as Label;
                    Label lblCustomDescription = row.FindControl("lblCustomDescription") as Label;
                    Label lblExtraState = row.FindControl("lblExtraState") as Label;

                    if (rntExtraForeignAmount != null && rntExtraForeignAmount.Value.HasValue)
                    {
                        int extraTypeId = (int)row.GetDataKeyValue("ExtraTypeId");
                        string clientContact = String.Empty;
                        string customDescription = String.Empty;
                        eExtraState extraState = eExtraState.Accepted;

                        if (lblClientContact != null)
                            clientContact = lblClientContact.Text.Trim();

                        if (lblCustomDescription != null)
                            customDescription = lblCustomDescription.Text.Trim();

                        // if there is no state specified make it accepted as this will be a surcharge.
                        if (lblExtraState != null)
                            if (String.IsNullOrEmpty(lblExtraState.Text))
                                extraState = eExtraState.Accepted;
                            else
                                extraState = (eExtraState)Enum.Parse(typeof(eExtraState),
                                    lblExtraState.Text.Trim().Replace(" ", ""));

                        int extraId = 0;
                        if (row.GetDataKeyValue("ExtraId") != DBNull.Value)
                            extraId = (int)row.GetDataKeyValue("ExtraId");

                        decimal foreignAmount = (decimal)rntExtraForeignAmount.Value.Value;
                        //TODO apply X rate
                        decimal amount = foreignAmount; // * this.Exc

                        decimal fuelSurchargePercentage = Convert.ToDecimal(rntFuelSurchargePercentage.Value);
                        decimal fuelSurchargeForeignAmount = (fuelSurchargePercentage / 100.0m) * foreignAmount;
                        fuelSurchargeForeignAmount = Math.Round(fuelSurchargeForeignAmount, 4, MidpointRounding.AwayFromZero);

                        Entities.Extra extra = new Entities.Extra();
                        extra.ExtraId = extraId;
                        extra.ExtraType = (eExtraType)extraTypeId;
                        extra.ForeignAmount = foreignAmount;
                        extra.ExtraAmount = amount;

                        var fsExtra = (from et in EF.DataContext.Current.ExtraTypeSet
                                       where et.ExtraTypeId == (int)extra.ExtraType && et.FuelSurchargeApplies == true
                                       select et).FirstOrDefault();

                        // Only apply fuel surcharges for the extra types that have fuel surcharge enabled.
                        if (fsExtra != null)
                        {
                            if (rntOrderRate.Culture.LCID != nativeCulture.LCID)
                                extra.FuelSurchargeAmount = currencyConverter.ConvertToLocal(fuelSurchargeForeignAmount);
                            else
                                extra.FuelSurchargeAmount = fuelSurchargeForeignAmount;

                            extra.FuelSurchargeForeignAmount = fuelSurchargeForeignAmount;
                        }

                        extra.JobId = 0;
                        extra.OrderID = 0;
                        extra.ExtraState = extraState;
                        extra.ClientContact = clientContact;
                        extra.CustomDescription = customDescription;
                        extras.Add(extra);
                    }
                }
            }

            return extras;
        }

        /// <summary>
        /// The user may have been too quick and submitted the page before the "get rate" method returned.
        /// This method tests for this (i.e. does the rate in the hidden field match the rate we get now),
        /// need to check for rate overrides.
        /// </summary>
        private void CheckRateInformation(EF.VigoOrder vigoOrder, bool calledOnPageLoad)
        {
            DateTime collectionDateTime = (DateTime)dteCollectionFromDate.SelectedDate;
            collectionDateTime = collectionDateTime.Subtract(collectionDateTime.TimeOfDay);
            if (dteCollectionFromTime.SelectedDate == null)
                collectionDateTime = collectionDateTime.Add(new TimeSpan(23, 59, 59));
            else
                collectionDateTime = collectionDateTime.Add(new TimeSpan(((DateTime)dteCollectionFromTime.SelectedDate).TimeOfDay.Hours, ((DateTime)dteCollectionFromTime.SelectedDate).TimeOfDay.Minutes, 0));

            DateTime deliveryDateTime = (DateTime)dteDeliveryByDate.SelectedDate;
            deliveryDateTime = deliveryDateTime.Subtract(deliveryDateTime.TimeOfDay);
            if (dteDeliveryByTime.SelectedDate == null)
                deliveryDateTime = deliveryDateTime.Add(new TimeSpan(23, 59, 59));
            else
                deliveryDateTime = deliveryDateTime.Add(new TimeSpan(((DateTime)dteDeliveryByTime.SelectedDate).TimeOfDay.Hours, ((DateTime)dteDeliveryByTime.SelectedDate).TimeOfDay.Minutes, 0));

            Facade.IOrder facOrder = new Facade.Order();
            IEnumerable<Repositories.DTOs.RateSurcharge> surcharges = null;
            Repositories.DTOs.RateInformation correctRateInformation = null;

            StringDictionary vals = new StringDictionary();
            vals.Add("cboClient", cboClient.SelectedValue);
            //vals.Add("rblInvolvement", rblInvolvement.SelectedValue);
            vals.Add("OrderInstructionID", this.OrderInstructionID.ToString());
            vals.Add("ucCollectionPoint", ucCollectionPoint.PointID.ToString());
            vals.Add("ucDeliveryPoint", ucDeliveryPoint.PointID.ToString());
            vals.Add("cboPalletType", cboPalletType.SelectedValue);
            vals.Add("rntxtPalletSpaces", rntxtPalletSpaces.Text);
            vals.Add("cboGoodsType", cboGoodsType.SelectedValue);
            vals.Add("cboService", cboService.SelectedValue);

            try
            {
                int weight = 0;
                if (rntxtWeight.Value.HasValue)
                    weight = (int)rntxtWeight.Value.Value;

                if (vigoOrder != null)
                {
                    correctRateInformation = facOrder.GetRate(int.Parse(cboClient.SelectedValue), int.Parse(cboBusinessType.SelectedValue), this.OrderInstructionID, ucCollectionPoint.PointID, ucDeliveryPoint.PointID, int.Parse(cboPalletType.SelectedValue), (decimal)rntxtPalletSpaces.Value.Value, weight, int.Parse(cboGoodsType.SelectedValue), int.Parse(cboService.SelectedValue), collectionDateTime, deliveryDateTime, true, out surcharges, vigoOrder.FullPallets, vigoOrder.HalfPallets, vigoOrder.QtrPallets, vigoOrder.OverPallets, false);
                }
                else
                {
                    correctRateInformation = facOrder.GetRate(int.Parse(cboClient.SelectedValue), int.Parse(cboBusinessType.SelectedValue), this.OrderInstructionID, ucCollectionPoint.PointID, ucDeliveryPoint.PointID, int.Parse(cboPalletType.SelectedValue), (decimal)rntxtPalletSpaces.Value.Value, weight, int.Parse(cboGoodsType.SelectedValue), int.Parse(cboService.SelectedValue), collectionDateTime, deliveryDateTime, true, out surcharges, false);
                }
            }
            catch (ApplicationException aex)
            {
                if (!aex.Message.StartsWith("Postcode"))
                    throw;
            }
            catch (FormatException)
            {
                try
                {
                    System.Net.Mail.MailMessage mailMessage = new System.Net.Mail.MailMessage();

                    mailMessage.To.Add("support@orchestrator.co.uk,t.lunken@p1tp.com");

                    string strMessageBody = string.Empty;

                    strMessageBody = string.Format("cboClient:{0}\nOrderInstructionId:{1}\nucCollectionPoint:{2}\nucDeliveryPoint:{3}\ncboPalletType:{4}\ntxtNoPallets:{5}\ncboGoodsType:{6}\ncboService:{7}",
                        vals["cboClient"],
                        //vals["rblInvolvement"],
                        this.OrderInstructionID,
                        vals["ucCollectionPoint"],
                        vals["ucDeliveryPoint"],
                        vals["cboPalletType"],
                        vals["rntxtPalletSpaces"],
                        vals["cboGoodsType"],
                        vals["cboService"]);

                    mailMessage.Subject = "String format exception";
                    mailMessage.Sender = new MailAddress("support@p1tp.com");
                    mailMessage.From = new MailAddress("support@p1tp.com");
                    mailMessage.Body = strMessageBody;
                    System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient();
                    smtp.Host = Globals.Configuration.MailServer;
                    smtp.Credentials = new NetworkCredential(Globals.Configuration.MailUsername,
                        Globals.Configuration.MailPassword);

                    smtp.Send(mailMessage);
                    mailMessage.Dispose();
                }
                catch
                {
                }
            }

            if (correctRateInformation != null)
            {
                // A rate was detected
                // Apply the new rate to the controls unless the user has overridden the rate.
                decimal userRate = decimal.Zero;
                decimal tariffRate = decimal.Zero;

                userRate = (decimal)rntOrderRate.Value.Value;
                decimal.TryParse(hidTariffRate.Value, out tariffRate);

                if (hidTariffRateOverridden.Value == "false" && hidIsAutorated.Value == "true" && calledOnPageLoad == false)
                {
                    // The rate was not overridden, update the specified rate.
                    rntOrderRate.Value = (double?)correctRateInformation.ForeignRate;
                    txtRateTariffCard.Text = correctRateInformation.TariffDescription;
                }
                else if (hidTariffRateOverridden.Value == "false" && hidIsAutorated.Value == "false" && SavedOrder != null)
                {
                    /* 
                     * We have a rate but the order is not auto-rated... the tariff table or rate value
                     * has been entered after the order was created. 
                     * 
                     * If the order rate is the same as the auto-rate, mark it as auto-rated,
                     * otherwise mark it as overridden.
                     * */

                    // Comparisons of NULL returns to be treated as equal to 0 rate to avoid incorrect Override flagging.

                    decimal? checkNullRate;

                    if (correctRateInformation.ForeignRate == null || correctRateInformation.ForeignRate.Equals(DBNull.Value))
                        checkNullRate = 0;
                    else
                        checkNullRate = correctRateInformation.ForeignRate;

                    if (rntOrderRate.Value == (double?)checkNullRate)
                    {
                        // Mark the order as auto-rated (no need to update the rate as it is already the same).
                        SavedOrder.IsAutorated = true;
                        hidIsAutorated.Value = "true";
                    }
                    else
                    {
                        // Mark the order auto-rated, but overridden... do not update the rate!
                        SavedOrder.IsAutorated = true;
                        SavedOrder.TariffOverrideDate = DateTime.Now;
                        SavedOrder.TariffOverrideUserID = Page.User.Identity.Name;
                        SavedOrder.IsTariffOverride = true;
                        hidIsAutorated.Value = "true";
                        hidTariffRateOverridden.Value = "true";
                        hidManuallyEnteredRate.Value = "false";
                    }
                }
                else
                {
                    // The rate was overridden, leave the user's rate alone.
                }

                // Update the tariff information so the correct data is picked up when creating the order.
                hidTariffRate.Value = correctRateInformation.ForeignRate.ToString();
            }
        }

        #endregion

        #region Client Reference Details

        private void ConfigureClientReferences()
        {
            if (_isUpdate)
            {
                // Populate the repeater control with the current reference fields
                List<Orchestrator.Entities.OrderReference> clientReferences = new List<Orchestrator.Entities.OrderReference>();
                foreach (Orchestrator.Entities.OrderReference reference in _order.OrderReferences)
                {
                    clientReferences.Add(reference);
                }

                repReferences.DataSource = clientReferences;
                repReferences.DataBind();
            }
            else
            {
                if (cboClient.SelectedValue != string.Empty)
                {
                    Orchestrator.Facade.IOrganisationReference facOrganisationReference = new Orchestrator.Facade.Organisation();
                    _clientReferences = facOrganisationReference.GetReferencesForOrganisationIdentityId(int.Parse(cboClient.SelectedValue), true);

                    if (_clientReferences != null)
                    {
                        // Populate the repeater control with the required reference fields
                        repReferences.DataSource = _clientReferences;
                        repReferences.DataBind();
                    }

                }
            }
        }

        private void repReferences_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            Orchestrator.Entities.OrganisationReferenceCollection organisationReferences = (Orchestrator.Entities.OrganisationReferenceCollection)repReferences.DataSource;

            HtmlInputHidden hidOrganisationReferenceId = (HtmlInputHidden)e.Item.FindControl("hidOrganisationReferenceId");
            PlaceHolder plcHolder = (PlaceHolder)e.Item.FindControl("plcHolder");

            int organisationReferenceId = Convert.ToInt32(hidOrganisationReferenceId.Value);

            if (!IsPostBack && this.SavedOrder != null && this.SavedOrder.OrderReferences != null)
            {
                // Make sure the value is in place
                TextBox txtReferenceValue = (TextBox)e.Item.FindControl("txtReferenceValue");
                Orchestrator.Entities.OrderReference currentValue = this.SavedOrder.GetForOrganisationReferenceID(organisationReferenceId);
                if (currentValue != null)
                    txtReferenceValue.Text = currentValue.Reference;
            }

            Orchestrator.Entities.OrganisationReference reference = organisationReferences.FindByReferenceId(organisationReferenceId);

            CustomValidator validatorControl = new CustomValidator();
            validatorControl.Enabled = false;
            validatorControl.ControlToValidate = "txtReferenceValue";
            validatorControl.Display = ValidatorDisplay.Dynamic;
            validatorControl.ErrorMessage = "Please supply a " + reference.Description + ".";
            validatorControl.Text = "<img src=\"../../images/error.png\"  Title=\"Please supply a " + reference.Description + ".\" />";
            validatorControl.EnableClientScript = false;

            switch (reference.DataType)
            {
                case eOrganisationReferenceDataType.Decimal:
                    validatorControl.Enabled = true;
                    validatorControl.ServerValidate += new ServerValidateEventHandler(validatorControl_ServerValidateDecimal);
                    break;
                case eOrganisationReferenceDataType.FreeText:
                    // No additional validation required.
                    break;
                case eOrganisationReferenceDataType.Integer:
                    validatorControl.Enabled = true;
                    validatorControl.ServerValidate += new ServerValidateEventHandler(validatorControl_ServerValidateInteger);
                    break;
            }

            if (reference.MandatoryOnOrder)
            {
                validatorControl.Enabled = true;
                validatorControl.ServerValidate += new ServerValidateEventHandler(validatorControl_ServerValidateTextNotEmpty);
            }

            plcHolder.Controls.Add(validatorControl);
        }

        protected bool IsReferencesValidatorEnabled(int organisationReferenceId)
        {
            Orchestrator.Entities.OrganisationReferenceCollection organisationReferences = (Orchestrator.Entities.OrganisationReferenceCollection)repReferences.DataSource;
            Orchestrator.Entities.OrganisationReference reference = organisationReferences.FindByReferenceId(organisationReferenceId);

            if (reference.DataType == eOrganisationReferenceDataType.Decimal || reference.DataType == eOrganisationReferenceDataType.Integer || reference.MandatoryOnOrder)
            {
                return true;
            }

            return false;
        }

        private void validatorControl_ServerValidateDecimal(object source, ServerValidateEventArgs args)
        {
            args.IsValid = P1TP.Components.Common.SimpleValidation.ValidateNumericValue(args.Value, 0, false);
        }

        private void validatorControl_ServerValidateInteger(object source, ServerValidateEventArgs args)
        {
            args.IsValid = P1TP.Components.Common.SimpleValidation.ValidateNumericValue(args.Value, 0, true);
        }

        private void validatorControl_ServerValidateTextNotEmpty(object source, ServerValidateEventArgs args)
        {
            args.IsValid = !args.Value.Trim().Equals(String.Empty);
        }

        #endregion

        #region Set Default Collection Point

        private void SetDefaultCollectionPoint()
        {
            try
            {
                int identityID = int.Parse(cboClient.SelectedValue);
                Facade.Organisation facOrganisation = new Orchestrator.Facade.Organisation();
                Entities.Organisation client = facOrganisation.GetForIdentityId(identityID);

                if (txtRateTariffCard.Text.ToLower() != "overridden")
                    txtRateTariffCard.Text = client.Defaults[0].IncludeRateTariffCard.ToString();

                if (client.Defaults[0].DefaultCollectionPointId > 0)
                {
                    Facade.IPoint facPoint = new Facade.Point();
                    Entities.Point defaultPoint = facPoint.GetPointForPointId(client.Defaults[0].DefaultCollectionPointId);
                    ucCollectionPoint.SelectedPoint = defaultPoint;
                }
                else
                {
                    ucCollectionPoint.SelectedPoint = null;
                }

                //ucCollectionPoint.PointOwnerDescription = client.OrganisationName;
                //ucCollectionPoint.PointOwnerCBO.Value = client.IdentityId.ToString();
                //ucCollectionPoint.PointCBO.Text = defaultPoint.Description;
                //ucCollectionPoint.PointCBO.Value = defaultPoint.PointId.ToString();
                //ucCollectionPoint.PostTownCBO.Text = defaultPoint.PostTown.TownName;
                //ucCollectionPoint.PostTownCBO.Value = defaultPoint.PostTown.TownId.ToString();
            }
            catch { }


        }

        #endregion

        #region Client User Order Entry Restrictions
        // if this is being entered by a client/customer then they can only select points that belong to them
        // they cannot change the customer drop down.
        // can they enter rates?

        private void RestictClientChoices()
        {
            Entities.CustomPrincipal cp = Page.User as Entities.CustomPrincipal;
            if (cp.IsInRole(((int)eUserRole.ClientUser).ToString()))
            {
                Facade.IUser facUser = new Facade.User();
                SqlDataReader reader = facUser.GetRelatedIdentity(((Entities.CustomPrincipal)Page.User).UserName);
                reader.Read();

                if ((eRelationshipType)reader["RelationshipTypeId"] == eRelationshipType.IsClient)
                {
                    plcBusinessType.Visible = false;
                    int RelatedIdentityID = int.Parse(reader["RelatedIdentityId"].ToString());

                    cboClient.SelectedValue = Convert.ToString(RelatedIdentityID);
                    Facade.IOrganisation facOrganisation = new Facade.Organisation();
                    Entities.Organisation organisation = facOrganisation.GetForIdentityId(RelatedIdentityID);
                    cboClient.Text = organisation.OrganisationName;
                    cboClient.Enabled = false;

                    this.ucCollectionPoint.ClientUserOrganisationIdentityID = RelatedIdentityID;
                    this.ucDeliveryPoint.ClientUserOrganisationIdentityID = RelatedIdentityID;
                    this.IsClientUser = true;
                }
            }
        }

        #endregion

        #region Validation

        /// <summary>
        /// Validate that the new rate entered for the group is a valid currency value, i.e. must be at least 0.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="args"></param>
        static void cfvRate_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            decimal newRate;

            if (decimal.TryParse(args.Value, NumberStyles.Currency, Thread.CurrentThread.CurrentCulture, out newRate))
                args.IsValid = newRate >= 0;
            else
                args.IsValid = false;
        }

        void cfvClient_ServerValidate(object source, ServerValidateEventArgs args)
        {
            int ignored = 0;
            args.IsValid = int.TryParse(cboClient.SelectedValue, out ignored);
        }

        void cfvDelivery_ServerValidate(object source, ServerValidateEventArgs args)
        {
            if (dteCollectionByTime.SelectedDate == null || dteDeliveryByTime.SelectedDate == null)
            {
                return;
            }

            if (hidCollectionTimingMethod.Value.ToLower() == "timed")
            {
                dteCollectionByDate.SelectedDate = dteCollectionFromDate.SelectedDate;
                dteCollectionByTime.SelectedDate = dteCollectionFromTime.SelectedDate;
            }
            else if (hidCollectionTimingMethod.Value.ToLower() == "anytime")
            {
                dteCollectionFromDate.SelectedDate = dteCollectionByDate.SelectedDate;
                dteCollectionFromTime.SelectedDate = dteCollectionByTime.SelectedDate.Value.Date; // should return midnight for the time portion of the date.

                // Ensure the collection by time is 23:59 (because the user has selected 'anytime')
                dteCollectionByTime.SelectedDate = dteCollectionByTime.SelectedDate.Value.Date.Add(new TimeSpan(23, 59, 0));
            }

            if (hidDeliveryTimingMethod.Value.ToLower() == "timed")
            {
                dteDeliveryFromDate.SelectedDate = dteDeliveryByDate.SelectedDate;
                dteDeliveryFromTime.SelectedDate = dteDeliveryByTime.SelectedDate;
            }
            else if (hidDeliveryTimingMethod.Value.ToLower() == "anytime")
            {
                dteDeliveryFromDate.SelectedDate = dteDeliveryByDate.SelectedDate;
                dteDeliveryFromTime.SelectedDate = dteDeliveryByTime.SelectedDate.Value.Date; // should return midnight for the time portion of the date.

                // Ensure the delivery by time is 23:59 (because the user has selected 'anytime')
                dteDeliveryByTime.SelectedDate = dteDeliveryByTime.SelectedDate.Value.Date.Add(new TimeSpan(23, 59, 0));
            }

            bool passed = true;
            string errorMessage = string.Empty;

            DateTime collectionFromDate = dteCollectionFromDate.SelectedDate.Value.Date;
            collectionFromDate = collectionFromDate.AddHours(dteCollectionFromTime.SelectedDate.Value.Hour);
            collectionFromDate = collectionFromDate.AddMinutes(dteCollectionFromTime.SelectedDate.Value.Minute);

            DateTime collectionByDate = dteCollectionByDate.SelectedDate.Value.Date;
            collectionByDate = collectionByDate.AddHours(dteCollectionByTime.SelectedDate.Value.Hour);
            collectionByDate = collectionByDate.AddMinutes(dteCollectionByTime.SelectedDate.Value.Minute);

            DateTime deliveryFromDate = dteDeliveryFromDate.SelectedDate.Value.Date;
            deliveryFromDate = deliveryFromDate.AddHours(dteDeliveryFromTime.SelectedDate.Value.Hour);
            deliveryFromDate = deliveryFromDate.AddMinutes(dteDeliveryFromTime.SelectedDate.Value.Minute);

            DateTime deliveryByDate = dteDeliveryByDate.SelectedDate.Value.Date;
            deliveryByDate = deliveryByDate.AddHours(dteDeliveryByTime.SelectedDate.Value.Hour);
            deliveryByDate = deliveryByDate.AddMinutes(dteDeliveryByTime.SelectedDate.Value.Minute);

            lblCollectionDeliveryExceptions.Text = "";

            if (hidCollectionTimingMethod.Value.ToLower() == "window")
            {
                // If booking window is selected for collections then "collection by" must occur 
                // after "collection from".
                if (!(collectionByDate > collectionFromDate))
                {
                    passed = false;
                    errorMessage = string.Format("{0}The collection by date must occur after the collection from date.", errorMessage);
                    lblCollectionDeliveryExceptions.Text = "<li>The collection by date must occur after the collection from date.";
                }

                // The collection booking window must be at least 5 mins.
                DateTime collectionFromPlus5Mins = collectionFromDate.AddMinutes(5);
                if (collectionByDate <= collectionFromPlus5Mins)
                {
                    passed = false;
                    errorMessage = string.Format("{0}\nThe collection booking window must be at least 5 minutes in size.", errorMessage);
                    lblCollectionDeliveryExceptions.Text = string.Format("{0}\n<li>The collection booking window must be at least 5 minutes in size.", lblCollectionDeliveryExceptions.Text);
                }
            }

            if (hidDeliveryTimingMethod.Value.ToLower() == "window")
            {
                // If booking window is selected for deliveries then "delivery by" must occur 
                // after "delivery from".
                if (!(deliveryByDate > deliveryFromDate))
                {
                    passed = false;
                    errorMessage = string.Format("{0}\nThe delivery by date must occur after the delivery from date.", errorMessage);
                    lblCollectionDeliveryExceptions.Text = string.Format("{0}\n<li>The delivery by date must occur after the delivery from date.", lblCollectionDeliveryExceptions.Text);
                }

                // The delivery booking window must be at least 5 mins.
                DateTime deliveryFromPlus5Mins = deliveryFromDate.AddMinutes(5);
                if (deliveryByDate <= deliveryFromPlus5Mins)
                {
                    passed = false;
                    errorMessage = string.Format("{0}\nThe delivery booking window must be at least 5 minutes in size.", errorMessage);
                    lblCollectionDeliveryExceptions.Text = string.Format("{0}\n<li>The delivery booking window must be at least 5 minutes in size.", lblCollectionDeliveryExceptions.Text);
                }
            }


            // If timed booking is selected for collections then "collection by" must equal
            // "collection from".
            if (hidCollectionTimingMethod.Value.ToLower() == "timed")
            {
                if (!(collectionByDate == collectionFromDate))
                {
                    passed = false;
                }
            }

            // If timed booking is selected for deliveries then "delivery by" must equal
            // "delivery from".
            if (hidDeliveryTimingMethod.Value.ToLower() == "timed")
            {
                if (!(deliveryByDate == deliveryFromDate))
                {
                    passed = false;
                }
            }

            // "Delivery from" must occur after "collection by" regardless of timing method (i.e. timed booking or booking window)

            if (hidDeliveryTimingMethod.Value.ToLower() == "anytime" && hidCollectionTimingMethod.Value.ToLower() == "anytime")
            {
                if (collectionFromDate.Date > deliveryFromDate.Date) /* Note: this comparison does not take the time portion into account as we're comparing anytime values. */
                {
                    passed = false;
                    errorMessage = string.Format("{0}\nThe delivery must occur after the collection date.", errorMessage);
                    lblCollectionDeliveryExceptions.Text = string.Format("{0}\n<li>The delivery must occur after the collection date.", lblCollectionDeliveryExceptions.Text);
                }
            }
            else
            {
                //Use collectionFrom rather than collectionTo so that booked windows can overlap
                // Allow the delivery to be anytime if the collection is timed
                if (collectionFromDate > deliveryFromDate && hidDeliveryTimingMethod.Value.ToLower() != "anytime") /* Note: this comparison takes account of the time */
                {
                    passed = false;
                    errorMessage = string.Format("{0}\nThe delivery must occur after the collection date.", errorMessage);
                    lblCollectionDeliveryExceptions.Text = string.Format("{0}\n<li>The delivery must occur after the collection date.", lblCollectionDeliveryExceptions.Text);
                }
            }


            // Warning: dates occur far in the future 

            // Has validation passed or not
            if (!passed)
            {
                imgCfvDeliveryWarning.Attributes.Add("title", errorMessage);
                args.IsValid = false;
            }
            else
            {
                args.IsValid = true;
            }
        }

        #endregion

        #region Create Order/Job

        private void CreateOrder(Entities.Order order, bool canCreateJob, EF.VigoOrder vigoOrder, IEnumerable<Entities.Extra> extras)
        {
            Facade.IOrder facOrder = new Facade.Order();
            FacadeResult retVal;
            int orderGroupID = 0;
            string userName = ((Orchestrator.Entities.CustomPrincipal)Page.User).UserName;

            if (!string.IsNullOrEmpty(Request.QueryString[_ogid_QS]) && int.TryParse(Request.QueryString[_ogid_QS], out orderGroupID))
            {
                retVal = facOrder.Create(order, orderGroupID, extras, userName);
                OrderID = retVal.ObjectId;

                if (!retVal.Success)
                {
                    ucInfringments.Infringements = retVal.Infringements;
                    ucInfringments.DisplayInfringments();
                }
            }
            else
            {
                OrderID = facOrder.Create(order, extras, userName);
            }

            if (OrderID > 0)
            {
                SaveAllocation(order, orderGroupID);

                //If the Business Type of the Order is PalletForce save the extra Vigo fields
                if (vigoOrder != null)
                {
                    if (vigoOrder.EntityState == EntityState.Added)
                    {
                        // Create the key that represents the order and assign it to the VigoOrder
                        EntityKey orderKey = new EntityKey("DataContext.OrderSet", "OrderId", OrderID);
                        vigoOrder.OrderId = OrderID;
                        vigoOrder.OrderReference.EntityKey = orderKey;
                    }

                    //Save the VigoOrder
                    EF.DataContext.Current.SaveChanges();
                }
            }

            Facade.IGPS facGps = new GPS();
            facGps.CreatePointToPointEstimates(order.CollectionPointID, order.DeliveryPointID, userName);

            if (plcBusinessType.Visible && chkCreateJob.Visible && chkCreateJob.Checked && (OrderID > 0) && canCreateJob)
                CreateFullLoadJob(order);
            else
                lblJobInformation.Text = "";

            lblInformation.Text = "The order Has been Added. <b>Order ID :: " + OrderID.ToString() + "</b>";

            if (!string.IsNullOrEmpty(Request.QueryString[_ogid_QS]))
            {
                btnAddGroupedOrder.Visible = false;
                btnAddGroupedOrderTop.Visible = false;
            }
        }

        private Entities.FacadeResult UpdateOrder(Entities.Order order, EF.VigoOrder vigoOrder, IEnumerable<Entities.Extra> extras)
        {
            Facade.IOrder facOrder = new Facade.Order();

            int? updatedRunID;
            RunUpdateStatus runUpdateStatus;
            var retVal = facOrder.UpdateOrderAndRun(order, vigoOrder, extras, this.CollectionPointUpdated, this.DeliveryPointUpdated, this.Page.User.Identity.Name, out updatedRunID, out runUpdateStatus);

            if (updatedRunID.HasValue)
            {
                if ((runUpdateStatus & RunUpdateStatus.SuccessfullyUpdatedRunCollectionTime) != RunUpdateStatus.NotSet)
                {
                    this.lblUpdateCollection.Text = string.Format(" - Collection date on Run {0} updated.", updatedRunID);
                    this.lblUpdateCollection.ForeColor = Color.Blue;
                }
                else if ((runUpdateStatus & RunUpdateStatus.FailedToUpdateRunCollectionTime) != RunUpdateStatus.NotSet)
                {
                    this.lblUpdateCollection.Text = string.Format(" - Collection date on Run {0} NOT updated.", updatedRunID);
                    this.lblUpdateCollection.ForeColor = Color.Red;
                }

                if ((runUpdateStatus & RunUpdateStatus.SuccessfullyUpdatedRunDeliveryTime) != RunUpdateStatus.NotSet)
                {
                    this.lblUpdateDelivery.Text = string.Format(" - Delivery date on Run {0} updated.", updatedRunID);
                    this.lblUpdateDelivery.ForeColor = Color.Green;
                }
                else if ((runUpdateStatus & RunUpdateStatus.FailedToUpdateRunDeliveryTime) != RunUpdateStatus.NotSet)
                {
                    this.lblUpdateDelivery.Text = string.Format(" - Delivery date on Run {0} NOT updated.", updatedRunID);
                    this.lblUpdateDelivery.ForeColor = Color.Red;
                }
                else if ((runUpdateStatus & RunUpdateStatus.SuccessfullyUpdatedRunDeliveryPoint) != RunUpdateStatus.NotSet)
                {
                    this.lblUpdateDelivery.Text = string.Format(" - Delivery point on Run {0} updated.", updatedRunID);
                    this.lblUpdateDelivery.ForeColor = Color.Green;
                }
                else if ((runUpdateStatus & RunUpdateStatus.FailedToUpdateRunDeliveryPoint) != RunUpdateStatus.NotSet)
                {
                    this.lblUpdateDelivery.Text = string.Format(" - Delivery point on Run {0} NOT updated.", updatedRunID);
                    this.lblUpdateDelivery.ForeColor = Color.Red;
                }
            }

            if (retVal.Success)
            {
                SaveAllocation(order);

                lblInformation.Text = "The order has been updated. <b>Order ID :: " + OrderID.ToString() + "</b>";
                lblInformation.ForeColor = System.Drawing.Color.Black;

                ucInfringments.Infringements = null;
                ucInfringments.DisplayInfringments();
            }
            else
            {
                if (order.PlannedForDelivery && updatedRunID.HasValue && DeliveryPointUpdated && retVal.Infringements != null)
                {
                    ucInfringments.Infringements = retVal.Infringements;
                    ucInfringments.DisplayInfringments();
                }
                else
                {
                    ucInfringments.Infringements = null;
                    ucInfringments.DisplayInfringments();
                }

                lblInformation.Text = "The order has not been updated. <b>Order ID :: " + OrderID.ToString() + "</b>";
                lblInformation.ForeColor = System.Drawing.Color.Red;
            }

            //Set selected pallet in UI
            this.cboPalletType.FindItemByValue(this.SavedOrder.PalletTypeID.ToString()).Selected = true;
            grdExtras.Rebind();

            return retVal;
        }

        //-----------------------------------------------------------------------------------------------------------

        private void SaveAllocation(Entities.Order order)
        {
            SaveAllocation(order, order.OrderGroupID);
        }

        private void SaveAllocation(Entities.Order order, int orderGroupID)
        {
            Facade.IOrder facOrder = new Facade.Order();

            int? allocatedToIdentityID = WebUI.Utilities.ParseNullable<int>(cboAllocatedTo.SelectedValue);
            bool isManuallyAllocated = allocatedToIdentityID.HasValue && (WebUI.Utilities.ParseNullable<bool>(hidIsManuallyAllocated.Value) ?? false);

            if (!isManuallyAllocated)
            {
                EF.Organisation consortiumMember;
                if (orderGroupID > 0)
                    consortiumMember = facOrder.GetConsortiumMemberToAllocate(orderGroupID, out isManuallyAllocated);
                else
                    consortiumMember = facOrder.GetConsortiumMemberToAllocate(order);

                allocatedToIdentityID = consortiumMember == null ? (int?)null : consortiumMember.IdentityId;
            }

            if (allocatedToIdentityID != order.AllocatedToIdentityID)
            {
                facOrder.UpdateAllocation(
                    order.OrderID,
                    allocatedToIdentityID,
                    isManuallyAllocated,
                    Page.User.Identity.Name);

                order.SetAllocatedToIdentityID(allocatedToIdentityID);
            }
        }

        private bool CreateFullLoadJob(Orchestrator.Entities.Order order)
        {
            #region Collection time settings
            DateTime earliestDelivery = DateTime.Now;
            Int32 earliestDropPoint = 0;

            #endregion

            #region Order Details
            Facade.IOrder facOrder = new Facade.Order();
            DataSet OrderData = facOrder.GetOrdersForList(OrderID.ToString(), true, true);
            #endregion

            #region Job Defaults
            Entities.Job job = new Orchestrator.Entities.Job();
            job.JobType = eJobType.Groupage;
            job.BusinessTypeID = order.BusinessTypeID;
            job.IdentityId = Globals.Configuration.IdentityId;
            job.LoadNumber = "GRP" + Environment.TickCount.ToString().Substring(0, 3);
            job.Charge = new Orchestrator.Entities.JobCharge();
            job.Charge.JobChargeAmount = 0;
            job.Charge.JobChargeType = eJobChargeType.FreeOfCharge;
            #endregion

            #region Instantiations

            Entities.InstructionCollection collections = new Orchestrator.Entities.InstructionCollection();
            Entities.Instruction iCollect = null;
            Entities.CollectDrop cd = null;

            Entities.InstructionCollection drops = new Orchestrator.Entities.InstructionCollection();
            Entities.Instruction iDrop = null;

            Facade.IPoint facPoint = new Facade.Point();
            Entities.Point point = null;

            #endregion

            #region Variables
            int collectSequence = 1;
            bool newcollection = false;

            if (job.Instructions == null)
                job.Instructions = new InstructionCollection();
            #endregion

            foreach (DataRow row in OrderData.Tables[0].Rows)
            {
                #region Collections

                newcollection = false;
                int pointID = (int)row["CollectionRunDeliveryPointID"];
                DateTime bookedDateTime = (DateTime)row["CollectionDateTime"];
                bool collectionIsAnyTime = (bool)row["CollectionRunDeliveryIsAnyTime"];

                // if this setting is true then we want to create a new instruction for the order.
                if (Globals.Configuration.OneDropAndLoadInstructionPerOrder)
                    iCollect = null;
                else
                    iCollect = collections.GetForInstructionTypeAndPointID(eInstructionType.Load, pointID, bookedDateTime, collectionIsAnyTime);

                if (iCollect == null)
                {
                    iCollect = new Orchestrator.Entities.Instruction();
                    iCollect.InstructionTypeId = (int)eInstructionType.Load;
                    iCollect.BookedDateTime = bookedDateTime;
                    if (collectionIsAnyTime)
                        iCollect.IsAnyTime = true;
                    point = facPoint.GetPointForPointId(pointID);
                    iCollect.PointID = pointID;
                    iCollect.Point = point;
                    iCollect.ClientsCustomerIdentityID = point.IdentityId;
                    iCollect.CollectDrops = new Orchestrator.Entities.CollectDropCollection();
                    iCollect.InstructionOrder = collectSequence; // TODO: Confirm if this is valid with Steve.
                    newcollection = true;
                    collectSequence++;
                }

                cd = new Orchestrator.Entities.CollectDrop();
                cd.PalletTypeID = (int)row["PalletTypeID"];
                cd.NoPallets = (int)row["NoPallets"];
                cd.NoCases = (int)row["Cases"];
                cd.GoodsTypeId = (int)row["GoodsTypeID"];
                cd.OrderID = (int)row["OrderID"];
                cd.OrderAction = eOrderAction.Default;
                cd.Weight = (decimal)row["Weight"];
                cd.ClientsCustomerReference = row["DeliveryOrderNumber"].ToString();
                cd.Docket = row["OrderID"].ToString();

                iCollect.CollectDrops.Add(cd);
                if (newcollection)
                    collections.Add(iCollect);

                #endregion

                #region Deliveries
                eOrderAction orderAction = eOrderAction.Default;
                int dropSequence = 1;
                bool newdelivery = false;
                newdelivery = false;
                int deliveryPointID = (int)row["DeliveryPointID"];
                bool deliveryIsAnyTime = (bool)row["DeliveryIsAnyTime"];
                DateTime deliveryDateTime = DateTime.MinValue;

                if (deliveryIsAnyTime)
                    deliveryDateTime = (DateTime)row["DeliveryDateTime"];
                else
                    deliveryDateTime = (DateTime)row["DeliveryFromDateTime"];

                // If the user has selected Default (i.e. Deliver) a drop instruction will be created, otherwise a trunk instruction will be
                // created.
                eInstructionType instructionType = eInstructionType.Drop;

                // if this setting is true then we want to create a new instruction for the order.
                if (Globals.Configuration.OneDropAndLoadInstructionPerOrder)
                    iDrop = null;
                else
                    iDrop = drops.GetForInstructionTypeAndPointID(instructionType, deliveryPointID, deliveryDateTime, deliveryIsAnyTime);

                if (iDrop == null)
                {
                    iDrop = new Orchestrator.Entities.Instruction();
                    iDrop.InstructionTypeId = (int)instructionType;
                    iDrop.BookedDateTime = deliveryDateTime;
                    if (deliveryIsAnyTime)
                        iDrop.IsAnyTime = true;
                    point = facPoint.GetPointForPointId(deliveryPointID);
                    iDrop.ClientsCustomerIdentityID = point.IdentityId;
                    iDrop.PointID = deliveryPointID;
                    iDrop.Point = point;

                    iDrop.CollectDrops = new Orchestrator.Entities.CollectDropCollection();
                    iDrop.InstructionOrder = dropSequence;
                    newdelivery = true;
                    dropSequence++;

                }

                cd = new Orchestrator.Entities.CollectDrop();
                cd.PalletTypeID = (int)row["PalletTypeID"];
                cd.NoPallets = (int)row["NoPallets"];
                cd.NoCases = (int)row["Cases"];
                cd.GoodsTypeId = (int)row["GoodsTypeID"];
                cd.OrderID = (int)row["OrderID"];
                cd.Weight = (decimal)row["Weight"];
                cd.ClientsCustomerReference = row["DeliveryOrderNumber"].ToString();
                cd.Docket = row["OrderID"].ToString();
                cd.OrderAction = orderAction;

                iDrop.CollectDrops.Add(cd);
                if (newdelivery)
                    drops.Insert(0, iDrop);
                //drops.Add(iDrop); Stephen Newman 23/04/07 Changed to insert the drop to the front of the list as the sort processed later seems to swap objects if equal.

                facOrder.UpdateForCollectionRun(cd.OrderID, iDrop.PointID, iDrop.BookedDateTime, iDrop.IsAnyTime, cd.OrderAction, Page.User.Identity.Name);

                // Get the earliest delivery
               // if (deliveryDateTime < earliestDelivery)
               // {
                    earliestDropPoint = deliveryPointID;
                    earliestDelivery = deliveryDateTime;
               // }



                #endregion
            }

            var userName = ((Entities.CustomPrincipal)Page.User).UserName;

            Facade.IJob facJob = new Facade.Job();
            //TODO: this next call potentially changes the order's collection times and therefore should really be within the create job transaction
            facJob.SetCalculatedCollectionTimes(collections, drops, userName);

            #region Add the Instructions to the job

            foreach (Entities.Instruction instruction in collections)
            {
                job.Instructions.Add(instruction);
            }

            drops.Sort(eInstructionSortType.DropDateTime);
            foreach (Entities.Instruction instruction in drops)
            {
                job.Instructions.Add(instruction);
            }

            #endregion

            job.JobState = eJobState.Booked;
            Facade.IJob facjob = new Facade.Job();
            Entities.FacadeResult res = facjob.Create(job, false, false, userName);

            bool retVal = false;

            if (res.Success)
            {
                var jobID = res.ObjectId;

                if (jobID > 0)
                {
                    lblJobInformation.Text = "</br><b>Run ID " + jobID + " </b>has been created.";
                    lblJobInformation.ForeColor = System.Drawing.Color.Black;
                    Page.ClientScript.RegisterStartupScript(this.GetType(), "onload", "<script language=\"javascript\">window.open('" + Page.ResolveClientUrl("../Job/job.aspx?jobId=" + jobID) + "'+ getCSID(), '_blank', 'top=0');</script>");
                    retVal = true;
                }
                else
                {
                    lblJobInformation.Text = "The Run has not been created, please use the Groupage - Deliveries screen to allocate this Order to a Run.";
                    lblJobInformation.ForeColor = System.Drawing.Color.Red;
                }
            }
            else
            {
                lblJobInformation.Text = "Could not create run due to the following infringements: " + string.Join(", ", res.Infringements.Select(i => i.Description));
                lblJobInformation.ForeColor = System.Drawing.Color.Red;
            }

            return retVal;
        }

  
        #endregion

    }

}
