using System;
using System.Data;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using System.Collections.Generic;
using System.Collections.Specialized;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.Traffic
{
    public partial class OrderBasedManifest : Orchestrator.Base.BasePage
    {
        private List<int> _ordersList;
        public List<int> OrdersList
        {
            get
            {
                if (_ordersList == null)
                    _ordersList = new List<int>();

                _ordersList.Clear();
                int orderID = 0;
                foreach (GridItem row in grdManifest.Items)
                {
                    CheckBox chk = row.FindControl("chkOrderID") as CheckBox;

                    if (chk != null && chk.Checked)
                    {
                        orderID = int.Parse(row.OwnerTableView.DataKeyValues[row.ItemIndex]["OrderId"].ToString());

                        if (!_ordersList.Contains(orderID))
                        {
                            _ordersList.Add(orderID);
                        }
                    }
                }

                return _ordersList;
            }
        }

        private List<int> _addOrdersList;
        public List<int> AddOrdersList
        {
            get
            {
                if (_addOrdersList == null)
                    _addOrdersList = new List<int>();

                _addOrdersList.Clear();
                int orderID = 0;
                foreach (GridItem row in grdManifestAddOrders.Items)
                {
                    CheckBox chk = row.FindControl("chkOrderID") as CheckBox;

                    if (chk != null && chk.Checked)
                    {
                        orderID = int.Parse(row.OwnerTableView.DataKeyValues[row.ItemIndex]["OrderId"].ToString());
                        if (!_addOrdersList.Contains(orderID))
                        {
                            _addOrdersList.Add(orderID);
                        }
                    }
                }

                return _addOrdersList;
            }
        }

        private Entities.OrderManifest OrderManifest
        {
            get
            {
                if (this.ViewState["OrderManifest"] == null)
                    return null;
                else
                    return (Entities.OrderManifest)this.ViewState["OrderManifest"];
            }
            set
            {
                this.ViewState["OrderManifest"] = value;
            }
        }

        // Signifies whether the user is creating a new manifest or viewing/modifying an
        // existing order manifest.
        private int OrderManifestId
        {
            get
            {
                if (this.ViewState["OrderManifestId"] == null)
                    return 0;
                else
                    return (int)this.ViewState["OrderManifestId"];
            }
            set
            {
                this.ViewState["OrderManifestId"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            txtExtraRowCount.Type = NumericType.Number;

            if (!Page.IsPostBack)
            {
                OrderManifestId = Convert.ToInt32(Request.QueryString["omID"]);

                // The default start date is the start of tomorrow.
                DateTime defaultStartDateTime = DateTime.Now;
                defaultStartDateTime = defaultStartDateTime.Subtract(defaultStartDateTime.TimeOfDay);
                defaultStartDateTime = defaultStartDateTime.AddDays(1);

                dteStartDateTime.SelectedDate = defaultStartDateTime;

                if (OrderManifestId == 0)
                {

                    pnlOrdersCurrentlyOnManifest.Visible = false;

                    dteOrderManifestDate.SelectedDate = defaultStartDateTime;

                    // The user is looking to create a new order manifest
                    this.OrderManifest = null;

                    // Hide the resouce and subcontractor combos
                    trResourceSubcontractorComboSection.Visible = true;

                    grdManifest.Visible = false;
                }
                else
                {
                    pnlOrdersCurrentlyOnManifest.Visible = true;

                    // Hide the resouce and subcontractor combos
                    trResourceSubcontractorComboSection.Visible = false;

                    grdManifestAddOrders.Visible = false;

                    pnlSaveAndDisplay.Visible = true;

                    // The user wants to view/modify an existing order manifest
                    // Retrieve the Order Manifest
                    Facade.OrderManifest facOrderManifest = new Orchestrator.Facade.OrderManifest();
                    this.OrderManifest = facOrderManifest.GetForOrderManifestId(OrderManifestId);

                    lblOrderManifestNumber.Text = this.OrderManifest.OrderManifestId.ToString();
                    this.lblResourceOrSubcontractorNameLabel.Text = this.OrderManifest.ResourceId > 0 ? "Resource:" : "Subcontractor:";
                    this.lblResourceOrSubcontractorName.Text = this.OrderManifest.ResourceId > 0 ? this.OrderManifest.ResourceName : this.OrderManifest.SubContractorName;

                    // Users should not be allowed to edit the resource and subby drop downs.
                    this.cboResource.Enabled = false;
                    this.cboSubContractor.Enabled = false;

                    if (this.OrderManifest.ResourceId > 0)
                    {
                        this.cboResource.SelectedValue = this.OrderManifest.ResourceId.ToString();
                        this.cboResource.Text = this.OrderManifest.ResourceName;
                    }
                    else
                    {
                        this.cboSubContractor.SelectedValue = this.OrderManifest.SubContractorIdentityId.ToString();
                        this.cboSubContractor.Text = this.OrderManifest.SubContractorName;
                    }

                    this.dteOrderManifestDate.SelectedDate = new DateTime?(this.OrderManifest.ManifestDate);
                    this.txtOrderManifestName.Text = this.OrderManifest.Description;

                    grdManifest.Visible = true;
                    grdManifest.DataSource = new Facade.OrderManifest().GetOrderManifestForDisplay(this.OrderManifest.OrderManifestId);
                    grdManifest.DataBind();

                    // Do not display the saved manifest until the user explicitly clicks the "Save & Display" button.
                    //DisplaySavedManifest();
                }
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            cboResource.ItemsRequested += new RadComboBoxItemsRequestedEventHandler(cboResource_ItemsRequested);
            cboSubContractor.ItemsRequested += new RadComboBoxItemsRequestedEventHandler(cboSubContractor_ItemsRequested);
            grdManifest.ItemDataBound += new GridItemEventHandler(grdManifest_ItemDataBound);
            grdManifestAddOrders.ItemDataBound += new GridItemEventHandler(grdManifestAddOrders_ItemDataBound);
            grdManifestAddOrders.NeedDataSource += new GridNeedDataSourceEventHandler(grdManifestAddOrders_NeedDataSource);
            //btnReset.Click += new EventHandler(btnReset_Click);
            btnGetOrders.Click += new EventHandler(btnGetOrders_Click);
            btnDisplayManifest.Click += new EventHandler(btnDisplayManifest_Click);
            btnProducePILs.Click += new EventHandler(btnProducePILs_Click);
        }

        void grdManifestAddOrders_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            GetOrders();
        }

        

     

        private void SaveManifest(bool addOrders)
        {
            Facade.OrderManifest facOrderManifest = new Orchestrator.Facade.OrderManifest();

            if (OrderManifestId > 0)
            {
                List<int> orderIdsOfOrdersToRemoveFromJobs = new List<int>();

                // Identify those for removal
                if (chkRemoveFromJob.Checked)
                {
                    foreach (Entities.OrderManifestOrder existingOrderManifestOrder in this.OrderManifest.OrderManifestOrders)
                    {
                        if (!(this.OrdersList.Find(o => o == existingOrderManifestOrder.OrderId) > 0))
                        {
                            // Order Manifest order does not exist in the selected list.
                            if (existingOrderManifestOrder.Removed == false)
                            {
                                existingOrderManifestOrder.Removed = true;
                                orderIdsOfOrdersToRemoveFromJobs.Add(existingOrderManifestOrder.OrderId);
                            }
                        }
                    }
                }

                if (addOrders)
                {
                    // Identify those for insertion
                    foreach (int orderId in this.AddOrdersList)
                    {
                        Entities.OrderManifestOrder existingOrderManifestOrder = this.OrderManifest.OrderManifestOrders.Find(o => o.OrderId == orderId);
                        if (existingOrderManifestOrder != null)
                        {
                            if (existingOrderManifestOrder.Removed)
                            {
                                // This order has previously been in the manifest, then removed, and now the user
                                // wants to add it in once again.
                                existingOrderManifestOrder.Removed = false;
                            }
                        }
                        else
                        {
                            // The order has never been saved on this manifest, add it...
                            // Insert the order manifest order into the collection
                            Entities.OrderManifestOrder omo = new Orchestrator.Entities.OrderManifestOrder();
                            omo.OrderManifestId = this.OrderManifest.OrderManifestId;
                            omo.OrderId = orderId;
                            this.OrderManifest.OrderManifestOrders.Add(omo);
                        }
                    }
                }

                this.OrderManifest.Description = txtOrderManifestName.Text;
                this.OrderManifest.ManifestDate = dteOrderManifestDate.SelectedDate.Value;

                // No point in updating the driverresourceid or the subcontractoridentityid because once
                // a manifest has been created it should not be possible to change them.

                facOrderManifest.UpdateOrderManifest(this.OrderManifest, orderIdsOfOrdersToRemoveFromJobs, this.Page.User.Identity.Name);
                this.OrderManifest = new Facade.OrderManifest().GetForOrderManifestId(OrderManifestId);
            }
            else
            {
                if (this.AddOrdersList.Count != 0)
                {
                    // This is a completely new manifest.
                    Entities.OrderManifest newOrderManifest = new Orchestrator.Entities.OrderManifest();
                    newOrderManifest.Description = txtOrderManifestName.Text;
                    newOrderManifest.ManifestDate = dteOrderManifestDate.SelectedDate.Value;

                    if (cboResource.SelectedValue != "")
                    {
                        newOrderManifest.ResourceId = Convert.ToInt32(cboResource.SelectedValue);
                        newOrderManifest.ResourceName = cboResource.Text;
                    }

                    if (cboSubContractor.SelectedValue != "")
                    {
                        newOrderManifest.SubContractorIdentityId = Convert.ToInt32(cboSubContractor.SelectedValue);
                        newOrderManifest.SubContractorName = cboSubContractor.Text;
                    }

                    List<Entities.OrderManifestOrder> orderManifestOrders = new List<Orchestrator.Entities.OrderManifestOrder>();
                    foreach (int orderId in this.AddOrdersList)
                    {
                        Entities.OrderManifestOrder omo = new Orchestrator.Entities.OrderManifestOrder();
                        omo.OrderId = orderId;
                        orderManifestOrders.Add(omo);
                    }

                    newOrderManifest.OrderManifestOrders = orderManifestOrders;

                    OrderManifestId = facOrderManifest.CreateOrderManifest(newOrderManifest, Page.User.Identity.Name);
                    this.OrderManifest = new Facade.OrderManifest().GetForOrderManifestId(OrderManifestId);
                }
                else
                {
                    this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "NoRows", "alert('There are no orders in your manifest. Please add some orders and try again.');", true);
                }
            }
        }

        void grdManifest_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                DataRowView drv = (DataRowView)e.Item.DataItem;
                HiddenField hidOrderId = (HiddenField)e.Item.FindControl("hidOrderId");
                Label lblCustomer = (Label)e.Item.FindControl("lblCustomer");
                Label lblStartFrom = (Label)e.Item.FindControl("lblStartFrom");
                Label lblStartAt = (Label)e.Item.FindControl("lblStartAt");
                CheckBox chkOrderID = (CheckBox)e.Item.FindControl("chkOrderID");
                HtmlAnchor hypOrderId = (HtmlAnchor)e.Item.FindControl("hypOrderId");

                int orderId = int.Parse(drv["OrderId"].ToString());
                hidOrderId.Value = orderId.ToString();

                hypOrderId.HRef = "javascript:viewOrderProfile(" + orderId.ToString() + ");";

                HtmlAnchor hypJobId = (HtmlAnchor)e.Item.FindControl("hypJobId");
                int JobId = 0;
                int.TryParse(drv["JobId"].ToString(), out JobId);

                if (JobId > 0)
                {
                    hypJobId.InnerText = JobId.ToString();
                    hypJobId.HRef = "javascript:ViewJobDetails(" + JobId.ToString() + ");";
                }
                else
                {
                    hypJobId.InnerText = string.Empty;
                    hypJobId.HRef = string.Empty;
                }

                lblCustomer.Text = drv["Customer"].ToString();
                lblStartFrom.Text = drv["StartFrom"].ToString();
                lblStartAt.Text = drv["StartAtDisplay"].ToString();

                chkOrderID.Attributes.Add("onclick", "javascript:ChangeList(event,this);");

                if (drv["Removed"].ToString().ToLower() == "true")
                {
                    hypOrderId.InnerText = orderId.ToString() + " (Removed)";
                    chkOrderID.Checked = false;
                    e.Item.Enabled = false;
                    e.Item.CssClass = "GridRow_Office2007";
                }
                else
                {
                    hypOrderId.InnerText = orderId.ToString();
                    if (string.IsNullOrEmpty(drv["JobId"].ToString()))
                    {
                        // Order is no longer on a job -- assume that it should be removed at the next save.
                        chkOrderID.Checked = false;
                    }
                    else
                    {
                        chkOrderID.Checked = true;
                        e.Item.CssClass = "SelectedRow_Office2007";
                    }
                }
            }
            else if (e.Item.ItemType == GridItemType.Header)
            {
                //HtmlInputCheckBox chkAll = (HtmlInputCheckBox)e.Item.FindControl("chkAll");
                //chkAll.Checked = true;
            }
        }

        void grdManifestAddOrders_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                DataRowView drv = (DataRowView)e.Item.DataItem;
                HiddenField hidOrderId = (HiddenField)e.Item.FindControl("hidOrderId");
                Label lblCustomer = (Label)e.Item.FindControl("lblCustomer");
                Label lblStartFrom = (Label)e.Item.FindControl("lblStartFrom");
                Label lblStartAt = (Label)e.Item.FindControl("lblStartAt");
                CheckBox chkOrderID = (CheckBox)e.Item.FindControl("chkOrderID");
                HtmlAnchor hypOrderId = (HtmlAnchor)e.Item.FindControl("hypOrderId");
                
                int orderId = int.Parse(drv["OrderId"].ToString());
                hidOrderId.Value = orderId.ToString();
                hypOrderId.InnerText = orderId.ToString();
                hypOrderId.HRef = "javascript:viewOrderProfile(" + orderId.ToString() + ");";

                HtmlAnchor hypJobId = (HtmlAnchor)e.Item.FindControl("hypJobId");
                int JobId = 0;
                int.TryParse(drv["JobId"].ToString(), out JobId);

                if (JobId > 0)
                {
                    hypJobId.InnerText = JobId.ToString();
                    hypJobId.HRef = "javascript:ViewJobDetails(" + JobId.ToString() + ");";
                }
                else
                {
                    hypJobId.InnerText = string.Empty;
                    hypJobId.HRef = string.Empty;
                }

                lblCustomer.Text = drv["Customer"].ToString();
                lblStartFrom.Text = drv["StartFrom"].ToString();
                lblStartAt.Text = drv["StartAtDisplay"].ToString();

                chkOrderID.Checked = true;
            }
            else if (e.Item.ItemType == GridItemType.Header)
            {
                //HtmlInputCheckBox chkAll = (HtmlInputCheckBox)e.Item.FindControl("chkAll");
                //chkAll.Checked = true;
            }
        }

        private DateTime GetStartDateTime()
        {
            DateTime startDateTime = dteStartDateTime.SelectedDate.Value;
            return startDateTime;
        }

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

        void btnProducePILs_Click(object sender, EventArgs e)
        {
            NameValueCollection reportParams = new NameValueCollection();
            DataSet dsPIL = null;

            Orchestrator.Facade.ICollectDropLoadOrder facLoadOrder = new Orchestrator.Facade.CollectDrop();

            if (this.OrdersList.Count > 0)
            {
                #region Pop-up Report


                dsPIL = facLoadOrder.GetPILData(Entities.Utilities.GetCSV(this.OrdersList));

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

        void btnReset_Click(object sender, EventArgs e)
        {
            cboResource.Text = string.Empty;
            cboResource.SelectedValue = string.Empty;

            cboSubContractor.Text = string.Empty;
            cboSubContractor.SelectedValue = string.Empty;

            // pnlMatchedOrders.Visible = false;
            reportViewer.Visible = false;
        }

        void btnGetOrders_Click(object sender, EventArgs e)
        {
            grdManifestAddOrders.Rebind();
        }

        private void GetOrders()
        {
            int resourceId = 0;
            int subContractorId = 0;

            //pnlMatchedOrders.Visible = false;
            reportViewer.Visible = false;
            DataSet dsManifestData = null;
            Facade.IManifest facManifest = new Facade.Manifest();

            DateTime startDateTime = GetStartDateTime();

            lblOrderCount.Text = string.Empty;
            if (int.TryParse(cboResource.SelectedValue, out resourceId))
            {
                lblOrderCount.Text = "Searching for matches for resource.  ";
                dsManifestData = facManifest.GetOrdersBasedManifestForResource(resourceId, startDateTime, chkIncludePlannedWork.Checked);
            }
            else if (int.TryParse(cboSubContractor.SelectedValue, out subContractorId))
            {
                lblOrderCount.Text = "Searching for matches for sub-contractor.  ";
                dsManifestData = facManifest.GetOrdersBasedManifestForSubContractor(subContractorId, startDateTime);
            }

            if (dsManifestData != null)
            {
                if (this.OrderManifest != null)
                {
                    // Remove those orders that are already in the manifest (and not marked as removed).
                    for (int i = dsManifestData.Tables[0].Rows.Count; i > 0; i--)
                    {
                        DataRow row = dsManifestData.Tables[0].Rows[i - 1];
                        int orderId = Convert.ToInt32(row["OrderId"].ToString());

                        foreach (Entities.OrderManifestOrder omo in this.OrderManifest.OrderManifestOrders)
                        {
                            if (omo.OrderId == orderId && omo.Removed == false)
                            {
                                // This order is already on the manifest, remove it from the
                                // list of orders to add...
                                dsManifestData.Tables[0].Rows.Remove(row);
                                break;
                            }
                        }
                    }
                }
                grdManifestAddOrders.DataSource = dsManifestData;
                //grdManifestAddOrders.DataBind();
                grdManifestAddOrders.Visible = true;

                lblOrderCount.Text += dsManifestData.Tables[0].Rows.Count + " Orders matched for inclusion on manifest.";

                pnlSaveAndDisplay.Visible = true;
            }
        }

        void DisplaySavedManifest()
        {
            if (this.OrderManifest != null)
            {
                pnlOrdersCurrentlyOnManifest.Visible = true;

                // Hide the resouce and subcontractor combos
                trResourceSubcontractorComboSection.Visible = false;

                grdManifestAddOrders.Visible = false;

                pnlSaveAndDisplay.Visible = true;

                lblOrderManifestNumber.Text = this.OrderManifest.OrderManifestId.ToString();
                this.lblResourceOrSubcontractorNameLabel.Text = this.OrderManifest.ResourceId > 0 ? "Resource:" : "Subcontractor:";
                this.lblResourceOrSubcontractorName.Text = this.OrderManifest.ResourceId > 0 ? this.OrderManifest.ResourceName : this.OrderManifest.SubContractorName;

                // Users should not be allowed to edit the resource and subby drop downs.
                this.cboResource.Enabled = false;
                this.cboSubContractor.Enabled = false;

                this.dteOrderManifestDate.SelectedDate = new DateTime?(this.OrderManifest.ManifestDate);
                this.txtOrderManifestName.Text = this.OrderManifest.Description;

                grdManifest.Visible = true;
                pnlOrdersCurrentlyOnManifest.Visible = true;

                DataSet dsManifestData = null;

                // If the order manifest is null, then there is no saved manifest yet (because the user is creating a new one).
                if (this.OrderManifest != null)
                {
                    dsManifestData = new Facade.OrderManifest().GetOrderManifestForDisplay(this.OrderManifest.OrderManifestId);
                    grdManifest.DataSource = dsManifestData;
                    grdManifest.DataBind();
                }

                if (dsManifestData != null && dsManifestData.Tables[0].Rows.Count > 0)
                {
                    if (this.OrderManifest.ResourceId == 0)
                        this.reportViewer.IdentityId = this.OrderManifest.SubContractorIdentityId;

                    for (int i = dsManifestData.Tables[0].Rows.Count; i > 0; i--)
                    {
                        DataRow row = dsManifestData.Tables[0].Rows[i - 1];

                        if (row["Removed"].ToString().ToLower() == "true")
                        {
                            dsManifestData.Tables[0].Rows.Remove(row);
                        }
                    }

                    if (dsManifestData.Tables[0].Rows.Count > 0)
                    {
                        for (int extraRows = 0; extraRows < Convert.ToInt32(string.IsNullOrEmpty(txtExtraRowCount.Text) ? "0" : txtExtraRowCount.Text); extraRows++)
                        {
                            DataRow newRow = dsManifestData.Tables[0].NewRow();
                            dsManifestData.Tables[0].Rows.Add(newRow);
                        }

                        dsManifestData.AcceptChanges();

                        string resourceLabel = string.Empty;
                        resourceLabel = this.OrderManifest.ResourceId > 0 ? this.OrderManifest.ResourceName : this.OrderManifest.SubContractorName;

                        // Configure the report settings collection
                        NameValueCollection reportParams = new NameValueCollection();
                        string vehicle = dsManifestData.Tables[0].Rows[0]["RegNo"] != DBNull.Value ? dsManifestData.Tables[0].Rows[0]["RegNo"].ToString() : "";
                        reportParams.Add("Vehicle", vehicle);
                        reportParams.Add("ResourceLabel", resourceLabel);
                        reportParams.Add("ManifestDate", this.OrderManifest.ManifestDate.ToString("dd/MM/yy"));
                        reportParams.Add("ShowFullAddress", chkIncludeFullAddresses.Checked.ToString());
                        reportParams.Add("ShowCollectionDetails", chkShowCollectionDetails.Checked.ToString());

                        // Configure the Session variables used to pass data to the report
                        Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.OrdersBasedmanifest;
                        Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = dsManifestData;
                        Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
                        Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";
                        Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;

                        reportViewer.Visible = true;
                    }
                    else
                    {
                        this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "NoRows", "alert('There are no orders in your manifest. Please add some orders and try again.');", true);
                    }
                }
            }
            else
            {
                this.Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "NoRows", "alert('There are no orders in your manifest. Please add some orders and try again.');", true);
            }
        }

        void btnDisplayManifest_Click(object sender, EventArgs e)
        {
            bool addOrders = true;

            if (this.grdManifestAddOrders.Visible == false)
                // The add orders grid has not been visible to the user, thus do not add 
                // the orders that are selected in the hidden grid.
                addOrders = false;

            this.grdManifestAddOrders.Visible = false;
            lblOrderCount.Text = string.Empty;

            if (Page.IsPostBack)
                SaveManifest(addOrders);

            DisplaySavedManifest();
        }
        #endregion

        #endregion

    }
}
