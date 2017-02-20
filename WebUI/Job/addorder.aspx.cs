using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using System.Data;
using System.Data.SqlClient;
using Telerik.Web.UI;
using System.Web.Script.Serialization;
using System.IO;
using Orchestrator;

namespace Orchestrator.WebUI.Job
{
    public partial class addorder :Orchestrator.Base.BasePage
    {
        protected enum eUntetheredOrderIntentionOption { Collect, Cross_Dock, Deliver };

        #region Constants
        private const string SESSION_KEY = "_editOrder";
        private const string DEFAULT_XDOCK_POINTID = "_Default_xdock_pointid";
        #endregion

        #region Page properties

        protected Entities.Order Order
        {
            get { return (Entities.Order)this.Session[SESSION_KEY]; }
            set { this.Session[SESSION_KEY] = value; }
        }

        protected int PreSelectedOrderId
        {
            get
            {
                int orderId = 0;
                int.TryParse(Request.QueryString["oid"], out orderId);
                return orderId;
            }
        }

        protected int InstructionID
        {
            get
            {
                int instructionID = -1;
                int.TryParse(Request.QueryString["iid"], out instructionID);
                return instructionID;
            }
        }

        protected int JobID
        {
            get
            {
                int jobID = -1;
                int.TryParse(Request.QueryString["jid"], out jobID);
                return jobID;
            }
        }

        protected eInstructionType InstructionType
        {
            get
            {
                int instructionTypeID = -1;
                int.TryParse(Request.QueryString["it"], out instructionTypeID);
                return (eInstructionType)instructionTypeID;
            }
        }

        protected int PointID
        {
            get
            {
                int pointID = -1;
                int.TryParse(Request.QueryString["pid"], out pointID);
                return pointID;
            }
        }

        protected int InstructionOrder
        {
            get
            {
                int instructionOrder = -1;
                int.TryParse(Request.QueryString["io"], out instructionOrder);
                return instructionOrder;
            }
        }

        private int DefaultCrossDockPointID
        {
            get
            {
                if (this.ViewState[DEFAULT_XDOCK_POINTID] != null)
                    return (int)this.ViewState[DEFAULT_XDOCK_POINTID];
                else
                {
                    Facade.IPoint facPoint = new Facade.Point();
                    Facade.IOrganisation facOrg = new Facade.Organisation();
                    DataSet dsDefaults = facOrg.GetDefaultsForIdentityId(Globals.Configuration.IdentityId);
                    if (dsDefaults.Tables[0].Rows.Count > 0)
                    {
                        if (dsDefaults.Tables[0].Rows[0]["CollectionRunDeliveryPointID"] != DBNull.Value)
                            this.ViewState[DEFAULT_XDOCK_POINTID] = (int)dsDefaults.Tables[0].Rows[0]["CollectionRunDeliveryPointID"];
                        else
                            this.ViewState[DEFAULT_XDOCK_POINTID] = -1;
                    }

                    return (int)this.ViewState[DEFAULT_XDOCK_POINTID];
                }
            }
        }

        public int SelectedOrderID
        {
            get
            {
                int orderID = 0;
                foreach (GridItem row in grdOrders.SelectedItems)
                {
                    orderID = int.Parse(row.OwnerTableView.DataKeyValues[row.ItemIndex]["OrderID"].ToString());
                }

                return orderID;
            }
        }

        /// <summary>
        /// Used to supply an indicator of the mode the add order functionality is operating in
        /// i.e. are we adding an order to a job where an instruction already exists to tether
        /// the new order to (i.e. collecting/trunking/delivering from/to an existing instruction
        /// on the job.  Or could this operation theoretically add two instructions to the job?
        /// </summary>
        public bool IsAddingToExistingInstruction
        {
            get
            {
                return InstructionID > 0;
            }
        }

        #endregion

        #region Page Load/Init

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.Order = null;
                // Show the find order screen
                mvAddOrder.SetActiveView(vwFindOrder);

                if(PreSelectedOrderId > 0)
                {
                    chkSearchByOrderID.Checked = true;
                    txtSearch.Text = PreSelectedOrderId.ToString();
                    LoadOrder(int.Parse(txtSearch.Text));
                }
            }
        }

        protected override void OnInit(EventArgs e)
        {
            ((WizardMasterPage)this.Master).WizardTitle = "Add Order to Job";
            base.OnInit(e);
            this.btnFindOrder.Click += new EventHandler(btnFindOrder_Click);
            this.rblCollectionInstructionDeliveryAction.SelectedIndexChanged += new EventHandler(rblCollectionInstructionDeliveryAction_SelectedIndexChanged);
            this.btnAddOrderToCollection.Click += new EventHandler(btnAddOrderToCollection_Click);
            this.btnAddOrderToDrop.Click += new EventHandler(btnAddOrderToDrop_Click);
            this.btnAddOrder.Click += new EventHandler(btnAddOrder_Click);
            this.btnAddOrderToTrunk.Click += new EventHandler(btnAddOrderToTrunk_Click);
            this.rblUntetheredOrderIntention.SelectedIndexChanged += new EventHandler(rblUntetheredOrderIntention_SelectedIndexChanged);
            this.btnAddUntetheredOrderToJob.Click += new EventHandler(btnAddUntetheredOrderToJob_Click);

            this.cboClient.ItemsRequested += new RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);
            this.cboResource.ItemsRequested += new RadComboBoxItemsRequestedEventHandler(cboResource_ItemsRequested);
            this.cboSubContractor.ItemsRequested += new RadComboBoxItemsRequestedEventHandler(cboSubContractor_ItemsRequested);

            this.grdOrders.NeedDataSource += new GridNeedDataSourceEventHandler(grdOrders_NeedDataSource);

            this.dlgCreateOrder.DialogCallBack += new EventHandler(dlgCreateOrder_DialogCallBack);
        }

        protected void dlgCreateOrder_DialogCallBack(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(this.dlgCreateOrder.ReturnValue))
            {
                txtSearch.Text = this.dlgCreateOrder.ReturnValue.ToString();
                this.LoadOrder(int.Parse(this.dlgCreateOrder.ReturnValue));
            }
        }

        #endregion

        #region Find Order Handling

        void btnFindOrder_Click(object sender, EventArgs e)
        {
            if (chkSearchByOrderID.Checked)
            {
                if (txtSearch.Text.Length == 0 || !Utilities.ValidateNumericValue(txtSearch.Text))
                {
                    lblOrderInformation.Text = "The Order ID that you entered is not valid.";
                }
                else
                    LoadOrder(int.Parse(txtSearch.Text));
            }
            else
            {
                // Removed code: Do not load the order here, rebind the grid based on filter settings.
                //if (this.SelectedOrderID > 0)
                //    LoadOrder(this.SelectedOrderID);
                //else
                this.grdOrders.Rebind();
            }
        }

        void btnAddOrder_Click(object sender, EventArgs e)
        {
            if (this.SelectedOrderID == 0)
            {
                lblWhereText.Text = "Please select an Order.";
            }
            else
            {
                LoadOrder(this.SelectedOrderID);
            }
        }

        private void LoadOrder(int orderID)
        {
            Facade.IOrder facorder = new Facade.Order();
            this.Order = facorder.GetForOrderID(orderID);
            Entities.InstructionCollection jobInstructions = null;

            #region Check for valid order id

            if (this.Order == null)
            {
                // No order can be found for the id.
                lblOrderInformation.Text = "No order can be found for this Order ID please check and re-key.";
                return;
            }
            else
            {
                Facade.IInstruction facInstruction = new Facade.Instruction();
                jobInstructions = facInstruction.GetForJobId(this.JobID, false);

                if (jobInstructions.Exists(i => i.CollectDrops.Exists(cd => cd.OrderID == this.Order.OrderID)))
                {
                    // Cannot add to this job as the order is already present.
                    lblError.Text = "You cannot add this order to this job as the order is already involved with this job.";
                    lblError.Visible = true;
                    return;
                }

                // ensure that the order is valid for any actions here.
                if (this.Order.OrderStatus == eOrderStatus.Cancelled || this.Order.OrderStatus == eOrderStatus.Awaiting_Approval || this.Order.OrderStatus == eOrderStatus.Invoiced |
                    this.Order.OrderStatus == eOrderStatus.Pending || this.Order.OrderStatus == eOrderStatus.Rejected)
                {
                    lblError.CssClass = "Error";
                    lblError.Text = "This order cannot be amended as it is not in the correct status.";

                    return;
                }
                Facade.IOrganisation facOrg = new Facade.Organisation();
                string client = facOrg.GetNameForIdentityId(this.Order.CustomerIdentityID);
                Facade.IPoint facPoint = new Facade.Point();


                lblOrderInformation.Text = string.Format("The Order [<a href='#' onclick='viewOrderProfile({3});'>{3}</a>] Selected is for <b>{0}</b>, picking up from {1} and delivering to {2}", client, facPoint.GetPointForPointId(this.Order.CollectionRunDeliveryPointID).Description, facPoint.GetPointForPointId(this.Order.DeliveryPointID).Description, this.Order.OrderID);
            }

            #endregion

            if (IsAddingToExistingInstruction)
            {
                switch (this.InstructionType)
                {
                    case eInstructionType.Load:
                        HandleCollectionInstruction();
                        break;
                    case eInstructionType.Drop:
                        HandleDropInstruction();
                        break;
                }
            }
            else
            {
                HandleUntetheredOrder(jobInstructions);
            }
        }

        private void FindOrder()
        {
            // Search for orders based on Date Range Status and text
            // Determine the parameters
            List<int> orderStatusIDs = new List<int>();
            orderStatusIDs.Add((int)eOrderStatus.Approved);
            orderStatusIDs.Add((int)eOrderStatus.Delivered);

            // Retrieve the client id, resource id, and sub-contractor identity id.
            int clientID = 0;
            int.TryParse(cboClient.SelectedValue, out clientID);
            int resourceID = 0;
            int.TryParse(cboResource.SelectedValue, out resourceID);
            int subContractorIdentityID = 0;
            int.TryParse(cboSubContractor.SelectedValue, out subContractorIdentityID);

            // Find the orders.
            Facade.IOrder facOrder = new Facade.Order();
            DataSet orderData = null;
            try
            {
                if (dteStartDate.SelectedDate == null) dteStartDate.SelectedDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 01);
                if (dteEndDate.SelectedDate == null || dteEndDate.SelectedDate < dteStartDate.SelectedDate) dteEndDate.SelectedDate = DateTime.Today;

                // depending on the Instruction type selected, we need to search for orders at different points
                int? collectionPointID = null;
                int? deliveryPointID = null;
                if (this.InstructionType == eInstructionType.Load)
                    collectionPointID = this.PointID;
                if (this.InstructionType == eInstructionType.Drop)
                    deliveryPointID = this.PointID;
                orderData =
                    facOrder.Search(orderStatusIDs, (DateTime)dteStartDate.SelectedDate, (DateTime)dteEndDate.SelectedDate, txtSearch.Text, true, true, clientID,
                                    resourceID, subContractorIdentityID, collectionPointID, deliveryPointID);
                if (orderData.Tables[0].Rows.Count == 1)
                {
                    grdOrders.Visible = false;
                    LoadOrder((int)orderData.Tables[0].Rows[0]["OrderID"]);

                }
                else
                {

                    //filter out orders already on this run
                    if (this.JobID > 0)
                    {
                        List<Entities.Order> _orders = facOrder.GetForJobID(this.JobID);
                        string _filter = string.Empty;
                        foreach (Entities.Order o in _orders)
                        {
                            if (_filter.Length > 0)
                                _filter += ",";
                            _filter += o.OrderID.ToString();
                        }

                        if (!string.IsNullOrEmpty(_filter))
                        {
                            DataView dv = orderData.Tables[0].DefaultView;
                            dv.RowFilter = "OrderID NOT IN (" + _filter + ")";
                            grdOrders.DataSource = dv;
                            grdOrders.Visible = true;
                            btnAddOrder.Visible = true;
                        }
                    }
                    else
                    {
                        btnAddOrder.Visible = true;
                        grdOrders.DataSource = orderData;
                        grdOrders.Visible = true;
                    }
                }
            }
            catch (SqlException exc)
            {
                if (exc.Message.StartsWith("Timeout expired."))
                {
                    // A timeout exception has been encountered, instead of throwing the error page, instruct the user to refine their search.
                    lblOrderInformation.Text = "Your query is not precise enough, please provide additional information or narrow the date/order state range.";

                    // Communicate the details of the exception to support.
                    string methodCall = "Facade.IOrder.Search('{0}', {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}); encountered by {9}";
                    Utilities.SendSupportEmailHelper(
                        string.Format(methodCall,
                                      Entities.Utilities.GetCSV(orderStatusIDs),
                                      dteStartDate.SelectedDate,
                                      dteEndDate.SelectedDate,
                                      txtSearch.Text,
                                     true,
                                     true,
                                      clientID,
                                      resourceID,
                                      subContractorIdentityID,
                                      ((Entities.CustomPrincipal)Page.User).UserName)
                        , exc);

                    orderData = null;
                }
                else
                    throw;
            }
        }

        #endregion

        #region Combobox Event Handlers

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
            RadComboBoxItem rcItem = null;
            for (int i = itemOffset; i < endOffset; i++)
            {
                rcItem = new RadComboBoxItem();
                rcItem.Text = dt.Rows[i]["OrganisationName"].ToString();
                rcItem.Value = dt.Rows[i]["IdentityID"].ToString();
                cboClient.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }
        }

        void cboResource_ItemsRequested(object o, RadComboBoxItemsRequestedEventArgs e)
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
            RadComboBoxItem rcItem = null;
            for (int i = itemOffset; i < endOffset; i++)
            {
                rcItem = new RadComboBoxItem();
                rcItem.Text = dt.Rows[i]["Description"].ToString();
                rcItem.Value = dt.Rows[i]["ResourceId"].ToString();
                cboResource.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }
        }

        void cboSubContractor_ItemsRequested(object o, RadComboBoxItemsRequestedEventArgs e)
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

        #region Collection Instruction Handling

        private void HandleCollectionInstruction()
        {
            Entities.Instruction instructionToAdd = new Orchestrator.Entities.Instruction();

            this.rblCollectionInstructionDeliveryAction.Items.Clear();


            // Has the Order been planned for delivery
            if (this.Order.PlannedForDelivery)
            {
                // Does CRDP = Collection Instruction Point Then XDock to DRCP
                if (this.PointID != this.Order.CollectionRunDeliveryPointID)
                {
                    // Cannot add to this Collection Point as the chain would be broken.
                    lblError.Text = "You cannot add this order to this collection as the order has been planned for delivery and to be collected from somewhere else.";
                    lblError.Visible = true;
                    return;
                }

                if (this.Order.CollectionRunDeliveryPointID == this.PointID)
                {
                    this.rblCollectionInstructionDeliveryAction.Items.Add(new ListItem("Cross Dock", ((int)eOrderAction.Cross_Dock).ToString()));
                }
            }
            else
            {
                // Determine which Actions Can be performed on the order at this point.

                // if NOT planned for delivery and CRDP = Collection Instruction Point Then we can Choose to Cross Dock or Deliver
                if (this.PointID == this.Order.CollectionRunDeliveryPointID)
                {
                    this.rblCollectionInstructionDeliveryAction.Items.Add(new ListItem("Cross Dock", ((int)eOrderAction.Cross_Dock).ToString()));
                    this.rblCollectionInstructionDeliveryAction.Items.Add(new ListItem("Deliver", ((int)eOrderAction.Default).ToString()));
                }
                else
                {
                    this.rblCollectionInstructionDeliveryAction.Items.Add(new ListItem("Deliver", ((int)eOrderAction.Default).ToString()));
                }
                // If XDock then we can pick any point on current job (with Instruction order > than Current Point) or Add New Point, default to default XDock Point
                // If not planned for delivery and the instruction pointid != CRDP then you can only add the delivery instruction
            }

            this.mvAddOrder.SetActiveView(vwCollectionInstruction);
        }

        void rblCollectionInstructionDeliveryAction_SelectedIndexChanged(object sender, EventArgs e)
        {
            rblCollectionInstructionDeliveryPoint.Items.Clear();

            Facade.IPoint facPoint = new Facade.Point();
            Entities.Point point = null;
            Facade.IInstruction facInstruction = new Facade.Instruction();
            Entities.InstructionCollection instructions = facInstruction.GetForJobId(this.JobID);
            if (int.Parse(rblCollectionInstructionDeliveryAction.SelectedValue) == (int)eOrderAction.Cross_Dock)
            {
                // We have chosen to cross dock from this collection.
                if (this.Order.PlannedForDelivery)
                {
                    // We can only cross dock this to the DRCP.
                    point = facPoint.GetPointForPointId(this.Order.DeliveryRunCollectionPointID);

                    // Check to See if the point already exists on this job
                    Entities.Instruction instructionToAmend = instructions.Find(i => i.PointID == this.Order.DeliveryRunCollectionPointID && i.InstructionOrder > this.InstructionOrder);
                    if (instructionToAmend != null)
                    {
                        // there is already an instruction on the job going to where we want to cross dock the order.
                        ListItem li = new ListItem("Cross Dock to the Existing Delivery - " + instructionToAmend.Point.Description, instructionToAmend.PointID.ToString());
                        li.Attributes.Add("InstructionID", instructionToAmend.InstructionID.ToString());
                        rblCollectionInstructionDeliveryPoint.Items.Add(li);
                    }
                    else
                    {
                        rblCollectionInstructionDeliveryPoint.Items.Add(new ListItem(point.Description, point.PointId.ToString()));
                    }
                }
                else
                {
                    // List the Points on the Job following this instruction.
                    foreach (Entities.Instruction instruction in instructions.FindAll(i => i.InstructionOrder > this.InstructionOrder))
                    {
                        if (rblCollectionInstructionDeliveryPoint.Items.FindByValue(instruction.PointID.ToString()) == null)
                        {
                            rblCollectionInstructionDeliveryPoint.Items.Add(new ListItem(instruction.Point.Description, instruction.PointID.ToString()));
                        }
                    }

                    // Add the orders current CRDP
                    point = facPoint.GetPointForPointId(this.DefaultCrossDockPointID);
                    if (point != null)
                        rblCollectionInstructionDeliveryPoint.Items.Add(new ListItem("The Default Cross Docking Location " + point.Description, point.PointId.ToString()));
                }
            }
            else
            {
                // Add the orders delivery point
                point = facPoint.GetPointForPointId(this.Order.DeliveryPointID);
                rblCollectionInstructionDeliveryPoint.Items.Add(new ListItem(" Deliver to " + point.Description, point.PointId.ToString()));
                rblCollectionInstructionDeliveryPoint.Items[0].Selected = true;
                btnAddOrderToCollection.Text = "Finish";
            }

            this.lblWhereText.Text = "Please choose where you want to " + (int.Parse(rblCollectionInstructionDeliveryAction.SelectedValue) == (int)eOrderAction.Cross_Dock ? "Cross Dock" : "Deliver") + " this order.";
            this.pnlWehere.Visible = true;
        }

        void btnAddOrderToCollection_Click(object sender, EventArgs e)
        {
            Facade.IInstruction facInstruction = new Facade.Instruction();
            Facade.IPoint facPoint = new Facade.Point();

            Entities.InstructionCollection instructions = facInstruction.GetForJobId(this.JobID);
            List<Entities.Instruction> instructionsToAmend = new List<Orchestrator.Entities.Instruction>();

            // Add this to the Load Instruction
            Entities.Instruction loadInstruction = instructions.GetForInstructionId(this.InstructionID);
            Entities.Instruction dropInstruction = null;
            Entities.CollectDrop cdLoad = CreateCollectDrop(this.Order);

            loadInstruction.CollectDrops.Add(cdLoad);
            instructionsToAmend.Add(loadInstruction);

            if (int.Parse(rblCollectionInstructionDeliveryAction.SelectedValue) == (int)eOrderAction.Cross_Dock)
            {
                if (instructions.Find(i => i.PointID == int.Parse(rblCollectionInstructionDeliveryPoint.SelectedValue) && i.InstructionTypeId == (int)eInstructionType.Trunk && i.CollectDrops.Exists(collectDrop => collectDrop.OrderID > 0) && i.InstructionOrder > loadInstruction.InstructionOrder && !i.HasActual) != null)
                {
                    // We are adding this to an existing instruction for cross docking
                    dropInstruction = instructions.Find(i => i.PointID == int.Parse(rblCollectionInstructionDeliveryPoint.SelectedValue) && i.InstructionTypeId == (int)eInstructionType.Trunk && i.CollectDrops.Exists(collectDrop => collectDrop.OrderID > 0) && i.InstructionOrder > loadInstruction.InstructionOrder && !i.HasActual);
                    var cdDrop = CreateCollectDrop(this.Order);
                    dropInstruction.CollectDrops.Add(cdDrop);
                }
                else
                {
                    // We have chosen to cross dock this to a new location not currently on this job, therefore add a new instruction
                    dropInstruction = new Orchestrator.Entities.Instruction();
                    dropInstruction.PointID = int.Parse(rblCollectionInstructionDeliveryPoint.SelectedValue);
                    dropInstruction.Point = facPoint.GetPointForPointId(int.Parse(rblCollectionInstructionDeliveryPoint.SelectedValue));
                    dropInstruction.InstructionTypeId = (int)eInstructionType.Trunk;
                    dropInstruction.BookedDateTime = this.Order.DeliveryDateTime;
                    dropInstruction.IsAnyTime = this.Order.DeliveryIsAnytime;
                    dropInstruction.JobId = this.JobID;

                    var cdDrop = CreateCollectDrop(this.Order);
                    cdDrop.OrderAction = eOrderAction.Cross_Dock;
                    dropInstruction.CollectDrops.Add(cdDrop);
                }

                instructionsToAmend.Add(dropInstruction);
            }
            else
            {
                // We are delivering this to a new location
                dropInstruction = new Orchestrator.Entities.Instruction();
                dropInstruction.PointID = int.Parse(rblCollectionInstructionDeliveryPoint.SelectedValue);
                dropInstruction.Point = facPoint.GetPointForPointId(int.Parse(rblCollectionInstructionDeliveryPoint.SelectedValue));
                dropInstruction.InstructionTypeId = (int)eInstructionType.Drop;
                dropInstruction.BookedDateTime = this.Order.DeliveryDateTime;
                dropInstruction.IsAnyTime = this.Order.DeliveryIsAnytime;
                dropInstruction.JobId = this.JobID;
                dropInstruction.ClientsCustomerIdentityID = this.Order.CustomerIdentityID;

                var cdDrop = CreateCollectDrop(this.Order);
                cdDrop.OrderAction = eOrderAction.Default;
                dropInstruction.CollectDrops.Add(cdDrop);

                instructionsToAmend.Add(dropInstruction);
            }

            Entities.FacadeResult retVal = new Orchestrator.Entities.FacadeResult();
            Entities.Job job = new Orchestrator.Entities.Job();
            Facade.IJob facJob = new Facade.Job();
            job = facJob.GetJob(this.JobID, true);
            Entities.CustomPrincipal user = Page.User as Entities.CustomPrincipal;
            retVal = facJob.AmendInstructions(job, instructionsToAmend, eLegTimeAlterationMode.Minimal, user.Name);
            if (retVal.Success)
            {
                job = facJob.GetJob(this.JobID, true);
                foreach (var exportJob in Orchestrator.Application.GetSpecificImplementations<Application.IExportJob>())
                    exportJob.AddOrder(job, this.Order, user.Name);

                this.ClientScript.RegisterStartupScript(this.Page.GetType(), "OnLoad", "window.opener.RefreshPage(); window.close();", true);
            }
        }

        private static Entities.CollectDrop CreateCollectDrop(Entities.Order order)
        {
            var cd = new Orchestrator.Entities.CollectDrop
            {
                Order = order,
                OrderID = order.OrderID,
                NoPallets = order.NoPallets,
                ClientsCustomerReference = order.CustomerOrderNumber,
                GoodsTypeId = order.GoodsTypeID,
                NoCases = order.Cases,
                Weight = order.Weight,
                Docket = order.OrderID.ToString(),
            };

            return cd;
        }

        #endregion

        #region Drop Instruction Handling

        private void HandleDropInstruction()
        {

            this.rblDropInstructionDeliveryAction.Items.Clear();
            this.rblDropInstructionDeliveryPoint.Items.Clear();

            Facade.IPoint facPoint = new Facade.Point();
            Entities.Point point = null;
            ListItem li = new ListItem();

            // Has the Order been planned for delivery
            if (this.Order.PlannedForDelivery)
            {
                // Cannot add to this Collection Point as the chain would be broken.
                lblDropInstructionInfo.CssClass = "Error";
                lblError.Text = "You cannot add this order to this drop as the order has already been planned to be delivered.";
                return;
            }
            else
            {

                // If the orders delivery point (DP) is not the same as this instructions point then do not allow
                if (this.Order.DeliveryPointID != this.PointID)
                {
                    lblDropInstructionInfo.CssClass = "Error";
                    point = facPoint.GetPointForPointId(this.Order.DeliveryPointID);
                    lblError.CssClass = "Error";
                    lblError.Text = string.Format("You cannot add this order for delivery here as this order is to be delivered to {0}.", point.Description);
                    return;
                }

                // there is only 1 option and that is to load this, but where you load from is now the question
                Facade.IInstruction facInstruction = new Facade.Instruction();
                Entities.InstructionCollection instructions = facInstruction.GetForJobId(this.JobID);

                // Add the existing collections for this job that happen before this delivery
                foreach (Entities.Instruction instruction in instructions.FindAll(i => i.InstructionOrder < this.InstructionOrder))
                {
                    if (rblDropInstructionDeliveryPoint.Items.FindByValue(instruction.PointID.ToString()) == null)
                    {
                        li = new ListItem(instruction.Point.Description, instruction.PointID.ToString());
                        li.Attributes.Add("InstructionID", instruction.InstructionID.ToString());
                        rblDropInstructionDeliveryPoint.Items.Add(li);
                    }
                }

                // Add the default XDock location for this haulier (if there is one)
                if (this.DefaultCrossDockPointID > 0)
                {
                    // Make sure that we have not listed this point already
                    if (rblDropInstructionDeliveryPoint.Items.FindByValue(this.DefaultCrossDockPointID.ToString()) == null)
                    {
                        point = facPoint.GetPointForPointId(this.DefaultCrossDockPointID);
                        li = new ListItem("Default Cross Dock location ", this.DefaultCrossDockPointID.ToString());
                        if (!this.Order.PlannedForCollection)
                        {
                            li.Selected = true;
                        }
                        rblDropInstructionDeliveryPoint.Items.Add(li);
                    }

                }

                // the order's Collection Run Delivery Point (CRDP)
                if (rblDropInstructionDeliveryPoint.Items.FindByValue(this.Order.CollectionRunDeliveryPointID.ToString()) == null)
                {
                    point = facPoint.GetPointForPointId(this.Order.CollectionRunDeliveryPointID);
                    li = new ListItem(point.Description, point.PointId.ToString());
                    if (this.Order.PlannedForCollection)
                        li.Selected = true;
                    rblDropInstructionDeliveryPoint.Items.Add(li);
                }



            }

            this.mvAddOrder.SetActiveView(vwDropInstruction);

            this.lblDropWhereText.Text = "Please choose where you want to Load  this order.";
            this.pnlLoadFrom.Visible = true;
        }

        void btnAddOrderToDrop_Click(object sender, EventArgs e)
        {
            Facade.IInstruction facInstruction = new Facade.Instruction();
            Facade.IJob facJob = new Facade.Job();
            Facade.IPoint facPoint = new Facade.Point();
            Entities.Instruction loadInstruction = new Orchestrator.Entities.Instruction();
            Entities.Instruction dropInstruction = new Orchestrator.Entities.Instruction();
            Entities.InstructionCollection instructions = facInstruction.GetForJobId(this.JobID);
            Entities.CollectDrop cd = new Orchestrator.Entities.CollectDrop();
            List<Entities.Instruction> instructionsToAmend = new List<Orchestrator.Entities.Instruction>();

            Entities.CollectDrop cdDrop = CreateCollectDrop(this.Order);
            dropInstruction = instructions.GetForInstructionId(this.InstructionID);
            dropInstruction.CollectDrops.Add(cdDrop);
            instructionsToAmend.Add(dropInstruction);

            // Are we adding this to an existing load on this job?
            // This is not the correct way of dealing with this as we should look at the particular instruction selected, there is a 
            // bug though that means that you cannot write out custom attributes to a Radio Button List
            if (instructions.Find(i => i.PointID == int.Parse(rblDropInstructionDeliveryPoint.SelectedValue) && i.InstructionTypeId == (int)eInstructionType.Load && i.InstructionOrder < dropInstruction.InstructionOrder && !i.HasActual) != null)
            {
                loadInstruction = instructions.Find(i => i.PointID == int.Parse(rblDropInstructionDeliveryPoint.SelectedValue) && i.InstructionTypeId == (int)eInstructionType.Load && i.InstructionOrder < dropInstruction.InstructionOrder && !i.HasActual);
                Entities.CollectDrop cdLoad = CreateCollectDrop(this.Order);
                loadInstruction.CollectDrops.Add(cdLoad);
            }
            else
            {
                // We need to add a new Load instruction
                loadInstruction = new Orchestrator.Entities.Instruction();
                loadInstruction.PointID = int.Parse(rblDropInstructionDeliveryPoint.SelectedValue);
                loadInstruction.Point = facPoint.GetPointForPointId(int.Parse(rblDropInstructionDeliveryPoint.SelectedValue));
                loadInstruction.InstructionTypeId = (int)eInstructionType.Load;
                loadInstruction.BookedDateTime = this.Order.DeliveryDateTime;
                loadInstruction.IsAnyTime = this.Order.DeliveryIsAnytime;
                loadInstruction.JobId = this.JobID;
                Entities.CollectDrop cdLoad = CreateCollectDrop(this.Order);
                loadInstruction.CollectDrops.Add(cdLoad);
            }

            instructionsToAmend.Add(loadInstruction);

            Entities.FacadeResult retVal = null;
            Entities.Job job = facJob.GetJob(this.JobID, true);
            Entities.CustomPrincipal user = (Entities.CustomPrincipal)Page.User;
            retVal = facJob.AmendInstructions(job, instructionsToAmend, eLegTimeAlterationMode.Minimal, user.Name);
            if (retVal.Success)
            {
                job = facJob.GetJob(this.JobID, true);
                foreach (var exportJob in Orchestrator.Application.GetSpecificImplementations<Application.IExportJob>())
                    exportJob.AddOrder(job, this.Order, user.Name);

                this.ClientScript.RegisterStartupScript(this.Page.GetType(), "OnLoad", "window.opener.RefreshPage(); window.close();", true);
            }

        }

        #endregion

        #region Trunk Instruction Handling

        private void HandleTrunkInstruction()
        {
            Facade.IInstruction facInstruction = new Facade.Instruction();
            Entities.InstructionCollection instructions = facInstruction.GetForJobId(this.JobID);
            Facade.IPoint facPoint = new Facade.Point();
            Entities.Point point = null;
            // does the job have a load instruction for the orders CRDP
            point = facPoint.GetPointForPointId(this.Order.CollectionRunDeliveryPointID);
            rblTrunkInstructionLoadPoint.Items.Add(new ListItem(point.Description, point.PointId.ToString()));

            this.mvAddOrder.SetActiveView(vwTrunkInstruction);
        }

        void btnAddOrderToTrunk_Click(object sender, EventArgs e)
        {
            Facade.IInstruction facInstruction = new Facade.Instruction();
            Facade.IJob facJob = new Facade.Job();
            Facade.IPoint facPoint = new Facade.Point();
            Entities.Instruction loadInstruction = new Orchestrator.Entities.Instruction();
            Entities.Instruction dropInstruction = new Orchestrator.Entities.Instruction();
            Entities.InstructionCollection instructions = facInstruction.GetForJobId(this.JobID);
            Entities.CollectDrop cd = new Orchestrator.Entities.CollectDrop();
            List<Entities.Instruction> instructionsToAmend = new List<Orchestrator.Entities.Instruction>();
            cd.Order = this.Order;
            cd.OrderID = this.Order.OrderID;
            cd.NoPallets = this.Order.NoPallets;
            cd.ClientsCustomerReference = this.Order.CustomerOrderNumber;
            cd.GoodsTypeId = this.Order.GoodsTypeID;
            cd.NoCases = this.Order.Cases;
            cd.Weight = this.Order.Weight;
            cd.Docket = this.Order.OrderID.ToString();

            dropInstruction = instructions.GetForInstructionId(this.InstructionID);
            dropInstruction.CollectDrops.Add(cd);
            instructionsToAmend.Add(dropInstruction);

            // Is ther a load instruction for this point already.
            if (instructions.Find(i => i.PointID == this.Order.CollectionRunDeliveryPointID && i.InstructionTypeId == (int)eInstructionType.Load && i.InstructionOrder < dropInstruction.InstructionOrder && !i.HasActual) != null)
            {
                // Add the collect drop to this existing load
                loadInstruction = instructions.Find(i => i.PointID == this.Order.CollectionRunDeliveryPointID && i.InstructionTypeId == (int)eInstructionType.Load && i.InstructionOrder < dropInstruction.InstructionOrder && !i.HasActual);
                loadInstruction.CollectDrops.Add(cd);
            }
            else
            {
                // Add a new load instruction for this.
                loadInstruction = new Orchestrator.Entities.Instruction();
                loadInstruction.InstructionTypeId = (int)eInstructionType.Load;
                loadInstruction.PointID = int.Parse(rblTrunkInstructionLoadPoint.SelectedValue);
                loadInstruction.Point = facPoint.GetPointForPointId(int.Parse(rblTrunkInstructionLoadPoint.SelectedValue));
                loadInstruction.BookedDateTime = this.Order.CollectionDateTime;
                loadInstruction.IsAnyTime = this.Order.CollectionIsAnytime;
                loadInstruction.JobId = this.JobID;
                loadInstruction.CollectDrops.Add(cd);
            }
            instructionsToAmend.Add(loadInstruction);

            Entities.FacadeResult retVal = null;
            Entities.Job job = facJob.GetJob(this.JobID, true);
            Entities.CustomPrincipal user = (Entities.CustomPrincipal)Page.User;
            retVal = facJob.AmendInstructions(job, instructionsToAmend, eLegTimeAlterationMode.Minimal, user.Name);
            if (retVal.Success)
            {
                job = facJob.GetJob(this.JobID, true);
                foreach (var exportJob in Orchestrator.Application.GetSpecificImplementations<Application.IExportJob>())
                    exportJob.AddOrder(job, this.Order, user.Name);

                this.ClientScript.RegisterStartupScript(this.Page.GetType(), "OnLoad", "window.opener.RefreshPage(); window.close();", true);
            }
        }

        #endregion

        #region Untethered Order Handling

        private void HandleUntetheredOrder(Entities.InstructionCollection jobInstructions)
        {
            // This collection stores each option followed by an "is available" indicator.
            List<KeyValuePair<eUntetheredOrderIntentionOption, bool>> options = new List<KeyValuePair<eUntetheredOrderIntentionOption, bool>>();
            eUntetheredOrderIntentionOption[] values = (eUntetheredOrderIntentionOption[])Enum.GetValues(typeof(eUntetheredOrderIntentionOption));
            // Disable any options that can not be used.
            //      If the order has been planned for delivery, you can not deliver it.
            //      If the order has been collected, you can not collect it.
            foreach (eUntetheredOrderIntentionOption value in values)
            {
                bool isValidOption = true;

                if (value == eUntetheredOrderIntentionOption.Deliver && this.Order.PlannedForDelivery)
                    isValidOption = false;
                else if (value == eUntetheredOrderIntentionOption.Collect && this.Order.CollectionPointID != this.Order.CollectionRunDeliveryPointID)
                    isValidOption = false;

                options.Add(new KeyValuePair<eUntetheredOrderIntentionOption, bool>(value, isValidOption));
            }

            // Determine the default action based on the instruction pattern on the job.
            //      If there is at least one delivery then assume the default is deliver.
            //      Otherwise default to collection.
            //      If the desired default option is not available, then cross-dock is the option to plumb for.
            eUntetheredOrderIntentionOption defaultOption = eUntetheredOrderIntentionOption.Cross_Dock;
            if (options.Find(o => o.Key == eUntetheredOrderIntentionOption.Collect).Value
                || options.Find(o => o.Key == eUntetheredOrderIntentionOption.Deliver).Value)
            {
                if (jobInstructions.Exists(i => i.InstructionTypeId == (int)eInstructionType.Drop)
                                    && options.Exists(o => o.Key == eUntetheredOrderIntentionOption.Deliver && o.Value))
                {
                    // At least one drop has been found, and we can deliver this order?
                    defaultOption = eUntetheredOrderIntentionOption.Deliver;
                }
                else if (jobInstructions.Exists(i => i.InstructionTypeId == (int)eInstructionType.Load
                                                    && i.CollectDrops.Exists(cd => cd.Order != null && cd.Order.CollectionPointID == i.PointID))
                                        && options.Exists(o => o.Key == eUntetheredOrderIntentionOption.Collect && o.Value))
                {
                    // At least one real collection has been found (i.e. collecting an order from it's original collection point).
                    defaultOption = eUntetheredOrderIntentionOption.Collect;
                }
            }

            // Present the options.
            rblUntetheredOrderIntention.ClearSelection();
            rblUntetheredOrderIntention.Items.Clear();
            foreach (KeyValuePair<eUntetheredOrderIntentionOption, bool> option in options)
            {
                ListItem item = new ListItem();
                string name = Enum.GetName(typeof(eUntetheredOrderIntentionOption), option.Key);
                item.Text = name.Replace("_", " ");
                item.Value = name;
                item.Enabled = option.Value;
                if (option.Key == defaultOption && item.Enabled)
                {
                    // Mark this item as selected.
                    item.Selected = true;
                    ProvideUntetheredOrderOption(option.Key);
                }
                rblUntetheredOrderIntention.Items.Add(item);
            }

            mvAddOrder.SetActiveView(vwUntetheredOrder);
        }

        private void ProvideUntetheredOrderOption(eUntetheredOrderIntentionOption intention)
        {
            Entities.UntetheredLocations untetheredLocations = null;

            // Configure Default Selections
            switch (intention)
            {
                case eUntetheredOrderIntentionOption.Collect:
                    untetheredLocations = GetLocationsForCollection();
                    break;
                case eUntetheredOrderIntentionOption.Cross_Dock:
                    untetheredLocations = GetLocationsForCrossDock();
                    break;
                case eUntetheredOrderIntentionOption.Deliver:
                    untetheredLocations = GetLocationsForDelivery();
                    break;
                default:
                    throw new ArgumentOutOfRangeException("intention", "The value supplied is not expected.");
            }

            // Prepare the UI for the user.
            ConfigureUntetheredStartPointDisplay(intention, untetheredLocations);
            ConfigureUntetheredEndPointDisplay(intention, untetheredLocations);

            // Store recovered values to be used when applying the add order.
            hidUntetheredControl.Value = untetheredLocations.ToString();
        }

        /// <summary>
        /// Configures the untethered ui options for the user to make the relevant choice
        /// for picking the goods up.
        /// </summary>
        /// <param name="intention">What the user intends to do with the order.</param>
        /// <param name="untetheredLocations">The default location choices.</param>
        private void ConfigureUntetheredStartPointDisplay(eUntetheredOrderIntentionOption intention, Entities.UntetheredLocations untetheredLocations)
        {
            ListItem li;
            Facade.IPoint facPoint = new Facade.Point();

            rblUntetheredOrderStartPoint.Items.Clear();

            // Hide point picker by default.
            pnlUntetheredOrderStartPointPicker.Style["display"] = "none";

            // Configure pick up options.
            switch (intention)
            {
                case eUntetheredOrderIntentionOption.Collect:
                case eUntetheredOrderIntentionOption.Cross_Dock:
                    // Pick up location is fixed and no change can be made.
                    li = new ListItem(facPoint.GetPointForPointId(untetheredLocations.StartPointID.Value).Description, untetheredLocations.StartPointID.Value.ToString());
                    li.Selected = true;
                    rblUntetheredOrderStartPoint.Items.Add(li);
                    break;
                case eUntetheredOrderIntentionOption.Deliver:
                    // Pickup is defaulted to the CRDP.
                    li = new ListItem(facPoint.GetPointForPointId(untetheredLocations.StartPointID.Value).Description, untetheredLocations.StartPointID.Value.ToString());
                    li.Selected = true;
                    rblUntetheredOrderStartPoint.Items.Add(li);

                    // Any other non-called in collection on the job is fair game.
                    Facade.IInstruction facInstruction = new Facade.Instruction();
                    Entities.InstructionCollection instructions = facInstruction.GetForJobId(JobID, false);
                    IEnumerable<Entities.Instruction> posibilities = instructions.FindAll(i => i.InstructionTypeId == (int)eInstructionType.Load && (i.InstructionActuals == null || i.InstructionActuals.Count == 0));

                    if (posibilities != null)
                        foreach (Entities.Instruction instruction in posibilities)
                        {
                            // Add points that don't already exist in the list.
                            if (rblUntetheredOrderStartPoint.Items.FindByValue(instruction.PointID.ToString()) == null)
                            {
                                li = new ListItem(instruction.Point.Description, instruction.Point.PointId.ToString());
                                rblUntetheredOrderStartPoint.Items.Add(li);
                            }
                        }

                    // The default cross dock location (if one exists).
                    Facade.IOrganisation facOrganisation = new Facade.Organisation();
                    Entities.Organisation owner = facOrganisation.GetForIdentityId(Globals.Configuration.IdentityId);

                    // Use the default cross dock location if one exists and it isn't already in the list of options.
                    if (owner != null
                        && owner.Defaults != null
                        && owner.Defaults.Count > 1
                        && owner.Defaults[0].GroupageCollectionRunDeliveryPoint > 0)
                    {
                        int defaultCrossDockLocation = owner.Defaults[0].GroupageCollectionRunDeliveryPoint;
                        if (rblUntetheredOrderStartPoint.Items.FindByValue(defaultCrossDockLocation.ToString()) == null)
                        {
                            li = new ListItem(facPoint.GetPointForPointId(defaultCrossDockLocation).Description, defaultCrossDockLocation.ToString());
                            rblUntetheredOrderStartPoint.Items.Add(li);
                        }
                    }

                    // Can select any other point.
                    li = new ListItem("Another Location", string.Empty);
                    rblUntetheredOrderStartPoint.Items.Add(li);
                    break;
            }

            // Add client side handling for when the start point is altered.
            var foundSelectedItem = false;
            foreach (ListItem item in rblUntetheredOrderStartPoint.Items)
            {
                item.Attributes["onclick"] = string.Format("javascript:SetCustomPointVisibility(this, '{0}')", pnlUntetheredOrderStartPointPicker.ClientID);
                if (item.Selected)
                    foundSelectedItem = true;
            }
            // Ensure at least one option is selected (if one exists)
            if (!foundSelectedItem && rblUntetheredOrderStartPoint.Items.Count > 0)
                rblUntetheredOrderStartPoint.Items[0].Selected = true;
            if (rblUntetheredOrderStartPoint.SelectedItem != null && rblUntetheredOrderStartPoint.SelectedValue == string.Empty)
                pnlUntetheredOrderStartPointPicker.Style["display"] = string.Empty;
        }

        /// <summary>
        /// Configures the untethered ui options for the user to make the relevant choice
        /// for handling the goods.
        /// </summary>
        /// <param name="intention">What the user intends to do with the order.</param>
        /// <param name="untetheredLocations">The default location choices.</param>
        private void ConfigureUntetheredEndPointDisplay(eUntetheredOrderIntentionOption intention, Entities.UntetheredLocations untetheredLocations)
        {
            ListItem li;
            Facade.IPoint facPoint = new Facade.Point();

            rblUntetheredOrderEndPoint.Items.Clear();

            // Hide point picker by default.
            pnlUntetheredOrderEndPointPicker.Style["display"] = "none";

            // Configure handling options.
            switch (intention)
            {
                case eUntetheredOrderIntentionOption.Collect:
                case eUntetheredOrderIntentionOption.Cross_Dock:
                    if (untetheredLocations.EndPointID.HasValue)
                    {
                        li = new ListItem(facPoint.GetPointForPointId(untetheredLocations.EndPointID.Value).Description, untetheredLocations.EndPointID.Value.ToString());
                        li.Selected = true;
                        rblUntetheredOrderEndPoint.Items.Add(li);
                    }

                    // Any other non-called in order handling trunk location on the job is an appropriate choice.
                    Facade.IInstruction facInstruction = new Facade.Instruction();
                    Entities.InstructionCollection instructions = facInstruction.GetForJobId(JobID, false);
                    IEnumerable<Entities.Instruction> possibilities = instructions.FindAll(i => i.InstructionTypeId == (int)eInstructionType.Trunk && i.CollectDrops != null && i.CollectDrops.Exists(cd => cd.Order != null) && (i.InstructionActuals == null || i.InstructionActuals.Count == 0));
                    if (possibilities != null)
                    {
                        foreach (Entities.Instruction instruction in possibilities)
                        {
                            // Add points that don't already exist in the list.
                            if (rblUntetheredOrderEndPoint.Items.FindByValue(instruction.PointID.ToString()) == null)
                            {
                                li = new ListItem(instruction.Point.Description, instruction.Point.PointId.ToString());
                                rblUntetheredOrderEndPoint.Items.Add(li);
                            }
                        }
                    }

                    // The default cross dock location (if one exists).
                    Facade.IOrganisation facOrganisation = new Facade.Organisation();
                    Entities.Organisation owner = facOrganisation.GetForIdentityId(Globals.Configuration.IdentityId);

                    // Use the default cross dock location if one exists and it isn't already in the list of options.
                    if (owner != null
                        && owner.Defaults != null
                        && owner.Defaults.Count > 1
                        && owner.Defaults[0].GroupageCollectionRunDeliveryPoint > 0)
                    {
                        int defaultCrossDockLocation = owner.Defaults[0].GroupageCollectionRunDeliveryPoint;
                        if (rblUntetheredOrderStartPoint.Items.FindByValue(defaultCrossDockLocation.ToString()) == null)
                        {
                            li = new ListItem(facPoint.GetPointForPointId(defaultCrossDockLocation).Description, defaultCrossDockLocation.ToString());
                            rblUntetheredOrderStartPoint.Items.Add(li);
                        }
                    }

                    // Can select any other point.
                    li = new ListItem("Another Location", string.Empty);
                    rblUntetheredOrderEndPoint.Items.Add(li);
                    break;
                case eUntetheredOrderIntentionOption.Deliver:
                    // Handling location is fixed and no change can be made.
                    li = new ListItem(facPoint.GetPointForPointId(untetheredLocations.EndPointID.Value).Description, untetheredLocations.EndPointID.Value.ToString());
                    li.Selected = true;
                    rblUntetheredOrderEndPoint.Items.Add(li);
                    break;
            }

            // Add client side handling for when the end point is altered.
            var foundSelectedItem = false;
            foreach (ListItem item in rblUntetheredOrderEndPoint.Items)
            {
                item.Attributes["onclick"] = string.Format("javascript:SetCustomPointVisibility(this, '{0}')", pnlUntetheredOrderEndPointPicker.ClientID);
                if (item.Selected)
                    foundSelectedItem = true;
            }
            // Ensure at least one option is selected (if one exists)
            if (!foundSelectedItem && rblUntetheredOrderEndPoint.Items.Count > 0)
                rblUntetheredOrderEndPoint.Items[0].Selected = true;
            if (rblUntetheredOrderEndPoint.SelectedItem != null && rblUntetheredOrderEndPoint.SelectedValue == string.Empty)
                pnlUntetheredOrderEndPointPicker.Style["display"] = string.Empty;
        }

        #region Location Recovery Logic

        /// <summary>
        /// User has indicated that they wish to delivery the new order to it's delivery location.
        /// Makes sense to default the collection location to
        /// the order's collection run delivery point.
        /// </summary>
        private Entities.UntetheredLocations GetLocationsForDelivery()
        {
            var untetheredLocations = new Entities.UntetheredLocations();

            // We're delivering to the right place, at the right time, to perform the correct action.
            untetheredLocations.EndPointID = this.Order.DeliveryPointID;
            untetheredLocations.EndDateTime = this.Order.DeliveryDateTime;
            untetheredLocations.EndIsAnyTime = this.Order.DeliveryIsAnytime;
            untetheredLocations.EndInstructionType = eInstructionType.Drop;

            // Only makes sense to default the collection location to
            // the order's collection run delivery point.
            untetheredLocations.StartPointID = this.Order.CollectionRunDeliveryPointID;
            untetheredLocations.StartDateTime = this.Order.CollectionRunDeliveryDateTime;
            untetheredLocations.StartIsAnyTime = this.Order.CollectionRunDeliveryIsAnytime;

            return untetheredLocations;
        }

        /// <summary>
        /// User has indicated that they wish to cross-dock the new order
        /// somewhere.  As we can only add cross-docks from the end of the
        /// collection run, it makes sense to default the collection point
        /// to that.
        /// If the order has been planned for delivery, default
        /// to the delivery run collection point, otherwise default to the
        /// haulier's default cross dock location.  If none is set, no default
        /// is possible.
        /// </summary>
        private Entities.UntetheredLocations GetLocationsForCrossDock()
        {
            var untetheredLocations = new Entities.UntetheredLocations();

            // We've got to bolt this on to the end of the collection run.
            untetheredLocations.StartPointID = this.Order.CollectionRunDeliveryPointID;
            untetheredLocations.StartDateTime = this.Order.CollectionRunDeliveryDateTime;
            untetheredLocations.StartIsAnyTime = this.Order.CollectionRunDeliveryIsAnytime;

            untetheredLocations.EndInstructionType = eInstructionType.Trunk;
            if (this.Order.PlannedForDelivery)
            {
                untetheredLocations.EndPointID = this.Order.DeliveryRunCollectionPointID;
                untetheredLocations.EndDateTime = this.Order.DeliveryRunCollectionDateTime;
                untetheredLocations.EndIsAnyTime = this.Order.DeliveryRunCollectionIsAnytime;
            }
            else
            {
                Facade.IOrganisation facOrganisation = new Facade.Organisation();
                Entities.Organisation owner = facOrganisation.GetForIdentityId(Globals.Configuration.IdentityId);

                // Use the default cross dock location if one exists.
                if (owner != null
                    && owner.Defaults != null
                    && owner.Defaults.Count > 1
                    && owner.Defaults[0].GroupageCollectionRunDeliveryPoint > 0)
                {
                    untetheredLocations.EndPointID = owner.Defaults[0].GroupageCollectionRunDeliveryPoint;
                }
                else
                {
                    // Not possible to supply an end point.
                    untetheredLocations.EndPointID = null;
                }

                // Unable to supply a decent default time.
                untetheredLocations.EndDateTime = null;
                untetheredLocations.EndIsAnyTime = null;
            }

            return untetheredLocations;
        }

        /// <summary>
        /// User has indicated that they wish to collect the new order
        /// from it's collection location.
        /// If the order is planned for delivery, default to the delivery
        /// run collection point, if not planned for delivery, default to
        /// deliverying the order.
        /// </summary>
        private Entities.UntetheredLocations GetLocationsForCollection()
        {
            var untetheredLocations = new Entities.UntetheredLocations();

            // We're collecting at the right place, and at the right time.
            untetheredLocations.StartPointID = this.Order.CollectionPointID;
            untetheredLocations.StartDateTime = this.Order.CollectionDateTime;
            untetheredLocations.StartIsAnyTime = this.Order.CollectionIsAnytime;

            // If the order is planned for delivery, default to the delivery
            // run collection point, if not planned for delivery, default to
            // deliverying the order.
            if (this.Order.PlannedForDelivery)
            {
                untetheredLocations.EndPointID = this.Order.DeliveryRunCollectionPointID;
                untetheredLocations.EndDateTime = this.Order.DeliveryRunCollectionDateTime;
                untetheredLocations.EndIsAnyTime = this.Order.DeliveryRunCollectionIsAnytime;
                untetheredLocations.EndInstructionType = eInstructionType.Trunk;
            }
            else
            {
                untetheredLocations.EndPointID = this.Order.DeliveryPointID;
                untetheredLocations.EndDateTime = this.Order.DeliveryDateTime;
                untetheredLocations.EndIsAnyTime = this.Order.DeliveryIsAnytime;
                untetheredLocations.EndInstructionType = eInstructionType.Drop;
            }

            return untetheredLocations;
        }

        #endregion

        #region Event Handlers

        void rblUntetheredOrderIntention_SelectedIndexChanged(object sender, EventArgs e)
        {
            eUntetheredOrderIntentionOption intention = (eUntetheredOrderIntentionOption)Enum.Parse(typeof(eUntetheredOrderIntentionOption), ((RadioButtonList)sender).SelectedValue, true);
            ProvideUntetheredOrderOption(intention);
        }

        void btnAddUntetheredOrderToJob_Click(object sender, EventArgs e)
        {
            // Recover the untethered locations.
            var untetheredLocations = new Entities.UntetheredLocations(hidUntetheredControl.Value);

            // Commit the start point.
            int startPointID = 0;
            if (!int.TryParse(rblUntetheredOrderStartPoint.SelectedValue, out startPointID))
            {
                if (ucUntetheredStartPoint.PointID < 1)
                {
                    lblError.Text = "You must specify the start point for this order.";
                    lblError.Visible = true;
                    return;
                }
                startPointID = ucUntetheredStartPoint.PointID;
            }
            untetheredLocations.StartPointID = startPointID;

            // Commit the end point.
            int endPointID = 0;
            if (!int.TryParse(rblUntetheredOrderEndPoint.SelectedValue, out endPointID))
            {
                if (ucUntetheredEndPoint.PointID < 1)
                {
                    lblError.Text = "You must specify the end point for this order.";
                    lblError.Visible = true;
                    return;
                }
                endPointID = ucUntetheredEndPoint.PointID;
            }
            untetheredLocations.EndPointID = endPointID;
            if (untetheredLocations.EndInstructionType == eInstructionType.Drop && this.Order.DeliveryPointID != untetheredLocations.EndPointID.Value)
            {
                // User has changed the end point, so they must want to cross-dock.
                untetheredLocations.EndInstructionType = eInstructionType.Trunk;
            }

            // Perform the required actions
            string userID = ((Entities.CustomPrincipal)Page.User).Name;
            Facade.IJob facJob = new Facade.Job();
            var res = facJob.AddUntetheredOrderToJob(this.JobID, this.Order, untetheredLocations, userID);

            if (res.Success)
            {
                this.ReturnValue = "refresh";
                this.Close();
            }
        }

        #endregion

        #endregion

        #region Grid View Events

        void grdOrders_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            if (Page.IsPostBack && e.RebindReason != GridRebindReason.InitialLoad)
                FindOrder();
            else
                grdOrders.DataSource = null;
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

    }
}
