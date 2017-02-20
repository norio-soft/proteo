using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Xml.Linq;

using System.Collections.Generic;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.Invoicing
{
    public partial class MatchOrder : Orchestrator.Base.BasePage
    {
        #region Properties

        private string consignmentNo = string.Empty;
        private CultureInfo OrchestratorCulture = new CultureInfo(Orchestrator.Globals.Configuration.NativeCulture);
        private IEnumerable<DataRow> JSCOrderData = null;

        private static string vs_pfInvoiceItemID = "vs_pfInvoiceItemID";
        public int PFInvoiceItemID
        {
            set { ViewState[vs_pfInvoiceItemID] = value; }
            get { return ViewState[vs_pfInvoiceItemID] == null ? -1 : (int)ViewState[vs_pfInvoiceItemID]; }
        }

        private static string vs_pfInvoice = "vs_pfInvoice";
        public Entities.PalletForceImportedInvoice PFInvoice
        {
            get { return ViewState[vs_pfInvoice] == null ? null : (Entities.PalletForceImportedInvoice)ViewState[vs_pfInvoice]; }
            set { ViewState[vs_pfInvoice] = value; }
        }

        private static string vs_preInvoiceID = "vs_preInvoiceID";
        public int PreInvoiceID
        {
            set { ViewState[vs_preInvoiceID] = value; }
            get { return ViewState[vs_preInvoiceID] == null ? -1 : (int)ViewState[vs_preInvoiceID]; }
        }

        private static string vs_importedInvoiceItemId = "vs_importedInvoiceItemId";
        public int ImportedInvoiceItemId
        {
            set { ViewState[vs_importedInvoiceItemId] = value; }
            get { return ViewState[vs_importedInvoiceItemId] == null ? -1 : (int)ViewState[vs_importedInvoiceItemId]; }
        }

        private static string vs_loadNo = "vs_loadNo";
        public string LoadNo
        {
            set { ViewState[vs_loadNo] = value; }
            get { return ViewState[vs_loadNo] == null ? String.Empty : ViewState[vs_loadNo].ToString(); }
        }

        private static string vs_itemIDs = "vs_itemIDs";
        public List<int> ItemIDs
        {
            get { return ViewState[vs_itemIDs] == null ? null : (List<int>)ViewState[vs_itemIDs]; }
            set { ViewState[vs_itemIDs] = value; }
        }

        #endregion

        #region Exposed Methods

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

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["piID"] != null && !string.IsNullOrEmpty(Request.QueryString["piID"].ToString()))
                    PreInvoiceID = int.Parse(Request.QueryString["piID"].ToString());

                if (Request.QueryString["pfID"] != null && !string.IsNullOrEmpty(Request.QueryString["pfID"].ToString()))
                    PFInvoiceItemID = int.Parse(Request.QueryString["pfID"].ToString());

                if (Request.QueryString["cNo"] != null && !string.IsNullOrEmpty(Request.QueryString["cNo"].ToString()))
                    consignmentNo = Request.QueryString["cNo"].ToString();

                if (Request.QueryString["importedInvoiceItemID"] != null && !string.IsNullOrEmpty(Request.QueryString["importedInvoiceItemID"].ToString()))
                    ImportedInvoiceItemId = Convert.ToInt32(Request.QueryString["importedInvoiceItemID"]);

                if (Request.QueryString["loadNo"] != null && !string.IsNullOrEmpty(Request.QueryString["loadNo"].ToString()))
                    LoadNo = Request.QueryString["loadNo"].ToString();

                ConfigureDisplay();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            btnClose.Click += new EventHandler(btnClose_Click);
            btnFindOrder.Click += new EventHandler(btnFindOrder_Click);
            btnMatchOrder.Click += new EventHandler(btnMatchOrder_Click);

            grdOrders.ItemDataBound += new Telerik.Web.UI.GridItemEventHandler(grdOrders_ItemDataBound);
            this.cboSubContractor.ItemsRequested += new RadComboBoxItemsRequestedEventHandler(cboSubContractor_ItemsRequested);
            this.cboClient.ItemsRequested += new RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);
            this.cboResource.ItemsRequested += new RadComboBoxItemsRequestedEventHandler(cboResource_ItemsRequested);
        }

        void cboClient_ItemsRequested(object o, RadComboBoxItemsRequestedEventArgs e)
        {
            cboClient.Items.Clear();

            Facade.IReferenceData facReferenceData = new Facade.ReferenceData();
            DataSet ds = facReferenceData.GetAllClientsFiltered(e.Text);

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
                rcItem.Value = dt.Rows[i]["IdentityID"].ToString();
                cboClient.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }
        }

        void cboResource_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            cboResource.Items.Clear();

            Facade.IResource facResource = new Facade.Resource();
            DataSet ds = facResource.GetAllResourcesFiltered(e.Text, false, true);

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

        #region Private Functions

        private void ConfigureDisplay()
        {
            string headerText = "Rate";

            if (this.PreInvoiceID > 0)
            {
                Facade.IPalletForceImportPreInvoice facPreInvoice = new Facade.PreInvoice();
                PFInvoice = facPreInvoice.GetImportedInvoiceForPreInvoiceID(PreInvoiceID, true);
                ItemIDs = PFInvoice.GetOrdersOnInvoice();

                switch (PFInvoice.InvoiceType)
                {
                    case eInvoiceType.PFDepotCharge:
                        headerText = "Sub Contract Rate";
                        break;
                    case eInvoiceType.PFHubCharge:
                        headerText = "Hub Charge";
                        break;
                    case eInvoiceType.PFSelfBillDeliveryPayment:
                        headerText = "Order Rate";
                        break;
                }

                if (consignmentNo != String.Empty)
                {
                    lblRefCaption.Text = "Consignment No";
                    lblRef.Text = txtSearch.Text = consignmentNo;
                }
            }
            else
                if (LoadNo != String.Empty)
                {
                    lblRefCaption.Text = "Load No";
                    lblRef.Text = txtSearch.Text = LoadNo;
                }

            grdOrders.Columns.FindByUniqueName("CurrentRate").HeaderText = headerText;

            dteStartDate.SelectedDate = new DateTime(DateTime.Today.Year, DateTime.Today.AddMonths(-1).Month, DateTime.Today.Day);
            dteEndDate.SelectedDate = DateTime.Today;

            Facade.IBusinessType facBT = new Facade.BusinessType();
            cblBusinessType.DataSource = facBT.GetAll();
            cblBusinessType.DataBind();

            foreach (ListItem li in cblBusinessType.Items)
                li.Selected = true;
        }

        private void FindOrder()
        {
            // Search for orders based on Date Range Status and text
            // Determine the parameters
            List<int> orderStatusIDs = new List<int>();
            orderStatusIDs.Add((int)eOrderStatus.Approved);
            orderStatusIDs.Add((int)eOrderStatus.Delivered);
            bool searchCustomFields = true;

            if (PFInvoice != null && PFInvoice.InvoiceType == eInvoiceType.PFDepotCharge)
                orderStatusIDs.Add((int)eOrderStatus.Invoiced);

            List<int> businessTypeIDs = new List<int>();
            foreach (ListItem li in cblBusinessType.Items)
                if (li.Selected)
                    businessTypeIDs.Add(int.Parse(li.Value));

            // Retrieve the client id, resource id, and sub-contractor identity id.
            int clientID = 0, resourceID = 0, subContractorIdentityID = 0;
            int.TryParse(cboClient.SelectedValue, out clientID);
            int.TryParse(cboResource.SelectedValue, out resourceID);
            int.TryParse(cboSubContractor.SelectedValue, out subContractorIdentityID);

            // Find the orders.
            if (dteStartDate.SelectedDate == null) dteStartDate.SelectedDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 01);
            if (dteEndDate.SelectedDate == null || dteEndDate.SelectedDate < dteStartDate.SelectedDate) dteEndDate.SelectedDate = DateTime.Today;

            if (chkSearchByOrderID.Checked)
                searchCustomFields = false;

            Facade.IOrder facOrder = new Facade.Order();
            Facade.IJobSubContractor facJSC = new Facade.Job();
            DataSet orderData = facOrder.Search(orderStatusIDs, (DateTime)dteStartDate.SelectedDate, (DateTime)dteEndDate.SelectedDate, txtSearch.Text, true, true, true, true, clientID, resourceID, subContractorIdentityID, businessTypeIDs, 0, 0, 0, searchCustomFields);
            
            var queryResultsSet = orderData.Tables[0].AsEnumerable();
            grdOrders.DataSource = queryResultsSet;
            grdOrders.DataBind();
        }

        #endregion

        #region Events

        #region Buttons

        void btnClose_Click(object sender, EventArgs e)
        { 
            this.ReturnValue = string.Empty;
            this.Close();
        }

        void btnFindOrder_Click(object sender, EventArgs e)
        {
            FindOrder();
        }

        void btnMatchOrder_Click(object sender, EventArgs e)
        {
            int itemID = -1;
            int.TryParse(hdnItemID.Value, out itemID);

            if (this.ImportedInvoiceItemId == -1)
            {
                if (itemID > 0)
                {
                    Entities.PalletForceImportedInvoiceItem pfInvoiceItem = PFInvoice.PalletForceImportedInvoiceItems.SingleOrDefault(pfiii => pfiii.PalletForceImportedItemID == PFInvoiceItemID);

                    if (pfInvoiceItem != null)
                        pfInvoiceItem.OrderID = itemID;
                }

                Facade.IPalletForceImportPreInvoice facPFIPreInvoice = new Facade.PreInvoice();
                bool retVal = facPFIPreInvoice.UpdatePalletForcePreInvoiceItem(PFInvoice, PFInvoiceItemID, PreInvoiceID, ((Entities.CustomPrincipal)Page.User).UserName);

                if (retVal)
                {
                    this.ReturnValue = bool.TrueString;
                    this.Close();
                }
            }
            else
                if (itemID > 0)
                {
                    // if this was called from the invoicingImport then we just want to return the importedInvoiceItemId and the ORder selected.
                    this.ReturnValue = ImportedInvoiceItemId.ToString() + "," + itemID.ToString();
                    this.Close();
                }
        }

        #endregion

        #region Grid

        void grdOrders_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                const string itemID = "ItemID";
                string columnName = string.Empty, rateColumn = string.Empty;
                DataRow dr = e.Item.DataItem as DataRow;

                int lcid = OrchestratorCulture.LCID;
                int orderID = dr.Field<int>("OrderID");
                decimal? rate = -1m;

                Label lblRate = e.Item.FindControl("lblRate") as Label;
                HtmlInputCheckBox chkSelect = e.Item.FindControl("chkSelect") as HtmlInputCheckBox;

                if (PFInvoice != null)
                {
                    switch (PFInvoice.InvoiceType)
                    {
                        #region Column Settings
                        case eInvoiceType.PFDepotCharge:
                            columnName = "SubContractLCID";
                            rateColumn = "SubContractRate";
                            break;
                        case eInvoiceType.PFHubCharge:
                            columnName = "lcid";
                            rateColumn = "HubCharge";
                            break;
                        case eInvoiceType.PFSelfBillDeliveryPayment:
                            columnName = "lcid";
                            rateColumn = "ForeignRate";
                            break;
                        default:
                            columnName = "lcid";
                            rateColumn = "ForeignRate";
                            break;
                        #endregion
                    }
                }
                else
                {
                    columnName = "lcid";
                    rateColumn = "ForeignRate";
                }

                if (chkSelect != null)
                {
                    chkSelect.Disabled = true;

                    if (ImportedInvoiceItemId == -1)
                    {
                        if (!ItemIDs.Exists(li => li == dr.Field<int?>("OrderID").Value)
                                &&
                                (
                                    (PFInvoice.InvoiceType != eInvoiceType.PFDepotCharge && dr.Field<int>("OrderStatusID") == (int)eOrderStatus.Delivered)
                                    ||
                                    (dr.Field<int?>("JobSubContractID").HasValue && !PFInvoice.GetJobSubContractOnInvoice().Exists(jsc => jsc == dr.Field<int?>("JobSubContractID").Value))
                                )
                            )
                        {
                            chkSelect.Attributes.Add(itemID, dr.Field<int?>("OrderID").Value.ToString());
                            chkSelect.Disabled = false;
                        }
                    }
                    else
                    {
                        // Called From invoicingImport
                        if (!dr.Field<int?>("ImportedInvoiceItemID").HasValue)                                                 
                        {
                            chkSelect.Attributes.Add(itemID, dr.Field<int?>("OrderID").Value.ToString());
                            chkSelect.Disabled = false;
                        }
                    }                    
                }

                if (!string.IsNullOrEmpty(dr.Field<int?>(columnName).ToString()))
                    int.TryParse(dr.Field<int>(columnName).ToString(), out lcid);

                rate = dr.Field<decimal?>(rateColumn);

                if (rate.HasValue && rate >= 0 && lblRate != null)
                    lblRate.Text = rate.Value.ToString("C", new CultureInfo(lcid));
            }
        }

        #endregion

        #endregion
    }
}