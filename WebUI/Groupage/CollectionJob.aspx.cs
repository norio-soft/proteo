using System;
using System.Data;
using System.Web.UI.WebControls;

using System.Collections.Generic;
using Telerik.Web.UI;
using System.Xml.Serialization;

namespace Orchestrator.WebUI
{
    public partial class Groupage_CollectionJob : Orchestrator.Base.BasePage
    {

        #region Private fields


        private readonly string C_TRAFFIC_SHEET_XML = "TrafficSheetFilterXML";

        private const string C_OrderData_VS = "C_OrderData_VS";
        private const string C_OrdersString_VS = "C_OrdersString_VS";
        private const string C_DeliverToPointID = "DeliverToPointID";
        private const string C_DeliverToPoint   = "DeliverToPoint";
        private const string C_DeliverAtDateTime = "DeliverAtDateTime";
        private const string C_DeliverAtAnyTime = "DeliverAtAnyTime";
        private const string C_OrderActionID = "OrderActionID";
        private const string C_LoadOder = "LoadOrder";
        private const string C_BusinessTypeIDs = "BusinessTypeIDs";
        private const string C_TrafficAreaIDs = "TrafficAreaIDs";
        private const string C_GRID_NAME = "__collectionsGrid";
   
        string _owner = string.Empty;
        string _deliveryPoint = string.Empty;
        string _deliveryPointTown = string.Empty;
        bool _openJob = false;
        int _jobID = -1;
        private int _palletRunningTotal = 0;
        private decimal _weightRunningTotal = 0;

        #endregion

        #region Properties

        protected IEnumerable<int> BusinessTypeIDs
        {
            get { return (IEnumerable<int>)ViewState[C_BusinessTypeIDs]; }
            set { ViewState[C_BusinessTypeIDs] = value; }
        }

        protected IEnumerable<int> TrafficAreaIDs
        {
            get { return (IEnumerable<int>)ViewState[C_TrafficAreaIDs]; }
            set { ViewState[C_TrafficAreaIDs] = value; }
        }

        protected string OpenJob
        {
            get { return _openJob.ToString().ToLower(); }
        }

        protected int JobID
        {
            get { return _jobID; }
        }

        private DataSet OrderData
        {
            get { return (DataSet)ViewState[C_OrderData_VS]; }
            set
            {
                ViewState[C_OrderData_VS] = value;
                ConfigureBusinessTypeChoices(value);
            }
        }

        private string OrdersCSV
        {
            get { return this.ViewState[C_OrdersString_VS].ToString(); }
            set { this.ViewState[C_OrdersString_VS] = value; }
        }

        #endregion

        #region Page Load/Init

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack && Request.QueryString["rcbID"] == null)
            {
                this.BusinessTypeIDs = PreviousPage.BusinessTypeIDs;
                this.TrafficAreaIDs = PreviousPage.TrafficAreaIDs;

                OrdersCSV = PreviousPage.Orders;
                PrepareOrders();
                rblOrderAction.DataSource = GetFriendlyNames(typeof(eOrderAction));
                rblOrderAction.DataBind();
                rblOrderAction.Items[3].Selected = true;
                rblOrderAction.Items[0].Text = "Deliver";

            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.btnCreateJob.Click += new EventHandler(btnCreateJob_Click);
            this.btnChangePlannedDeliveryPoint.Click += new EventHandler(btnChangePlannedDeliveryPoint_Click);
            this.btnReOrder.Click += new EventHandler(btnReOrder_Click);

            this.grdOrders.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdOrders_NeedDataSource);
            this.grdOrders.ItemDataBound += new GridItemEventHandler(grdOrders_ItemDataBound);
            grdOrders.ItemCommand += new GridCommandEventHandler(grdOrders_ItemCommand);
            // this.grdOrders.DetailTableDataBind += new Telerik.Web.UI.GridDetailTableDataBindEventHandler(grdOrders_DetailTableDataBind);

            this.cvDeliveryPoint.ServerValidate +=new ServerValidateEventHandler(cvDeliveryPoint_ServerValidate);
            this.cvLoadOrder.ServerValidate += new ServerValidateEventHandler(cvLoadOrder_ServerValidate);
            this.cvCrossDockLocationError.ServerValidate += new ServerValidateEventHandler(cvCrossDockLocationError_ServerValidate);
        }

        void grdOrders_ItemCommand(object source, GridCommandEventArgs e)
        {
            switch (e.CommandName.ToLower())
            {
                case "remove":
                    int orderID = int.Parse(((GridDataItem)e.Item).GetDataKeyValue("OrderID").ToString());
                    List<int> ordersList = new List<int>();
                    string[] arrOrders = OrdersCSV.Split(',');
                    foreach (string oid in arrOrders)
                        if (int.Parse(oid) != orderID && !ordersList.Contains(int.Parse(oid)))
                            ordersList.Add(int.Parse(oid));
                    OrdersCSV = Entities.Utilities.GetCSV(ordersList);
                    if (OrdersCSV.Length == 0)
                        Response.Redirect("Collections.aspx");
                    else
                    {
                        PrepareOrders();
                        grdOrders.Rebind();
                    }
                    break;
            }
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Offer the user creating the job a choice of which business type the job should be.
        /// The default value is the business type which has the most pallets (based on the
        /// individual pallet count and business type of each order being added to the job).
        /// </summary>
        /// <param name="orders">The data set containing the order information.</param>
        private void ConfigureBusinessTypeChoices(DataSet orders)
        {
            // Clear the current business type options down.
            cboBusinessType.ClearSelection();
            cboBusinessType.Items.Clear();

            // Store a list of key value pairs that represents the total pallets involved for each business type.
            // In each item in the list the Key is the Business Type ID, and the Value is the pallet count for that
            // business type.
            List<KeyValuePair<int, int>> btCounts = new List<KeyValuePair<int, int>>();

            if (orders != null && orders.Tables.Count > 0 && orders.Tables[0] != null)
            {
                foreach (DataRow order in orders.Tables[0].Rows)
                {
                    int businessTypeID = (int)order["BusinessTypeID"];
                    int palletCount = (int)order["NoPallets"];

                    // The business type has already been encountered, increase the pallet count.
                    // As the value property is read only, we need to remove the item and then
                    // cause it to be recreated with the new total.
                    if (btCounts.Exists(btCount => btCount.Key == businessTypeID))
                    {
                        palletCount += btCounts.Find(btCount => btCount.Key == businessTypeID).Value;
                        btCounts.RemoveAll(btCount => btCount.Key == businessTypeID);
                    }

                    // Record the entry.
                    btCounts.Add(new KeyValuePair<int, int>(businessTypeID, palletCount));
                }

                // Sort the different key value pairs based on their value properties (i.e. their pallet count).
                // The result of the value compare is multiplied by -1 to effectively reverse the sort direction as:
                //      -1 * -1 =  1
                //       1 * -1 = -1
                //       0 * -1 =  0
                btCounts.Sort((x, y) => x.Value.CompareTo(y.Value) * -1);
            }

            // Create the dropdown items that match the business type information.
            Facade.IBusinessType facBusinessType = new Facade.BusinessType();
            foreach (KeyValuePair<int, int> btCount in btCounts)
            {
                ListItem li = new ListItem();
                li.Value = btCount.Key.ToString();
                Entities.BusinessType businessType = facBusinessType.GetForBusinessTypeID(btCount.Key);
                li.Text = string.Format("{0} ({1})", businessType.Description, btCount.Value);

                cboBusinessType.Items.Add(li);
            }
        }

        private string[] GetFriendlyNames(Type sender)
        {
            string[] strings= Enum.GetNames(sender);
            for (int i = 0; i < strings.Length; i ++)
            {
                strings[i] = strings[i].Replace("_", " ");
            }

            return strings;
        }

        #endregion

        #region Button Event Handlers

        void btnReOrder_Click(object sender, EventArgs e)
        {
            //validate that the order can be done.
            if (Page.IsValid)
            {
                //reorder the grid based on the values typed id;
                foreach (GridDataItem gdi in grdOrders.Items)
                {
                    TextBox txtLoadOrder = gdi.FindControl("txtLoadOrder") as TextBox;
                    if (txtLoadOrder != null)
                    {
                        int orderID = int.Parse(gdi.GetDataKeyValue("OrderID").ToString());
                        DataRow[] drows = OrderData.Tables[0].Select("OrderID = " + orderID.ToString());
                        if (drows.Length == 1)
                        {
                            drows[0]["LoadOrder"] = txtLoadOrder.Text;
                        }
                    }
                }

                this.OrderData.AcceptChanges();
                grdOrders.Rebind();
                hidIsOrderChange.Value = "false";
            }
        }

        void btnCreateJob_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                PopulateEntities();
                Session[C_GRID_NAME + Utilities.GetFilterFromCookie(this.CookieSessionID, Request).BusinessTypes[0].ToString()] = null;
            }
        }

        void btnChangePlannedDeliveryPoint_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid) return;

            if (grdOrders.SelectedItems.Count == 0)
            {
                // Alert the user that they have not selected any orders to update.
                lblNote.Text = "No orders were selected to update, please select some orders and try again.";
                pnlConfirmation.Visible = true;
            }
            else
            {
                bool isAnyTime = false;
                int pointId = 0;
                DateTime newDeliveryDateTime = dteDeliveryDate.SelectedDate.Value;
                newDeliveryDateTime = newDeliveryDateTime.Subtract(newDeliveryDateTime.TimeOfDay);
                if (dteDeliveryTime.SelectedDate.HasValue == false)
                {
                    isAnyTime = true;
                    newDeliveryDateTime = newDeliveryDateTime.Add(new TimeSpan(23, 59, 59));
                }
                else
                    newDeliveryDateTime = newDeliveryDateTime.Add(dteDeliveryTime.SelectedDate.Value.TimeOfDay);

                // The order action Default means Deliver when in a collection job so this is renamed.
                string orderActionSelected = rblOrderAction.SelectedValue.Replace(" ", "_");
                if (orderActionSelected == "Deliver") orderActionSelected = "Default";
                int orderActionID = (int) (eOrderAction) Enum.Parse(typeof (eOrderAction), orderActionSelected);

                bool changeMade = false;

                foreach (GridItem row in grdOrders.SelectedItems)
                {
                    {
                        int orderID = int.Parse(row.OwnerTableView.DataKeyValues[row.ItemIndex]["OrderID"].ToString());

                        DataRow[] drows = OrderData.Tables[0].Select("OrderID = " + orderID.ToString());
                        if (drows.Length == 1)
                        {
                            drows[0][C_DeliverToPointID] = ucDeliveryPoint.SelectedPoint.PointId;
                            drows[0][C_DeliverToPoint] = ucDeliveryPoint.SelectedPoint.Description;
                            drows[0][C_DeliverAtDateTime] = newDeliveryDateTime;
                            drows[0][C_DeliverAtAnyTime] = isAnyTime;
                            drows[0][C_OrderActionID] = orderActionID;
                            changeMade = true;
                        }
                    }
                }

                if (changeMade)
                {
                    OrderData.AcceptChanges();
                    grdOrders.Rebind();
                    hidIsUpdatePoint.Value = "false";
                }
                hidIsOrderChange.Value = "false";
            }
        }

        #endregion

        #region Grid View Event Handlers
        int i = 1;
        void grdOrders_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                DataRowView drv = (DataRowView) e.Item.DataItem;

                TextBox txt = e.Item.FindControl("txtLoadOrder") as TextBox;
                if (txt == null)
                    return;

                txt.Text = i.ToString();
                i++;

                _palletRunningTotal += (int) drv["NoPallets"];
                _weightRunningTotal += (decimal) drv["Weight"];
            }
            else if (e.Item is GridFooterItem)
            {
                Label lblPalletTotal = (Label) e.Item.FindControl("lblPalletTotal");
                Label lblWeightTotal = (Label) e.Item.FindControl("lblWeightTotal");

                if (lblPalletTotal != null && lblWeightTotal != null)
                {
                    lblPalletTotal.Text = _palletRunningTotal.ToString();
                    lblWeightTotal.Text = _weightRunningTotal.ToString("N2");
                }
            }
        }

        void grdOrders_DetailTableDataBind(object source, Telerik.Web.UI.GridDetailTableDataBindEventArgs e)
        {
            int orderID = int.Parse(e.DetailTableView.ParentItem.GetDataKeyValue("OrderID").ToString());
            DataSet ds = OrderData;
            DataView dv = ds.Tables[0].DefaultView;
            if (e.DetailTableView.DataMember == "OrderDetails")
            {
                dv.RowFilter = "OrderID = " + orderID.ToString();
                e.DetailTableView.DataSource = dv;
            }
        }

        void grdOrders_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            if (OrderData != null)
            {
                DataView dv = OrderData.Tables[0].DefaultView;
                dv.Sort = "LoadOrder ASC";
                grdOrders.DataSource = dv;
            }
           
           // grdOrders.MasterTableView.DetailTables[0].DataSource = OrderData.Tables[0];
        }

        private DataSet DeterminDefaultDeliveryPoint()
        {
            Facade.IPoint facPoint = new Facade.Point();
            return facPoint.GetDefaultDeliveryPointsForOrders(OrdersCSV);
        }

        protected string GetOrderAction(int orderActionID)
        {
            if (orderActionID == (int)eOrderAction.Default)
                return "Deliver";
            else
                return ((eOrderAction)orderActionID).ToString().Replace("_", " ");
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

        #region Entitiy Handling
        private void PrepareOrders()
        {
            Facade.IOrder facOrder = new Facade.Order();
            DataSet dsOrders = facOrder.GetOrdersForList(OrdersCSV, false, true);

            dsOrders.Tables[0].Columns.Add(C_DeliverToPointID, typeof(int));
            dsOrders.Tables[0].Columns.Add(C_DeliverToPoint, typeof(string));
            dsOrders.Tables[0].Columns.Add(C_DeliverAtDateTime, typeof(DateTime));
            dsOrders.Tables[0].Columns.Add(C_DeliverAtAnyTime, typeof(bool));
            dsOrders.Tables[0].Columns.Add(C_OrderActionID, typeof(int));
            dsOrders.Tables[0].Columns.Add(C_LoadOder, typeof(string));
            
            DataSet dsDefaultDeliveryPoints = DeterminDefaultDeliveryPoint();

            int loadOrder = 1;
            foreach (DataRow row in dsOrders.Tables[0].Rows)
            {
                int orderID = (int)row["OrderID"];
                DataRow[] drows = dsDefaultDeliveryPoints.Tables[0].Select("OrderID = " + orderID.ToString());
                if (drows.Length == 1)
                {
                    row[C_DeliverToPointID] = (int)drows[0]["DeliveryRunCollectionPointID"];
                    row[C_DeliverToPoint] = (string)drows[0]["Description"];
                    row[C_DeliverAtDateTime] = (DateTime)drows[0]["DeliveryRunCollectionDateTime"];
                    row[C_DeliverAtAnyTime] = (bool)drows[0]["DeliveryRunCollectionIsAnytime"];
                    row[C_OrderActionID] = (int)eOrderAction.Default;
                }

                if ((bool)row["PlannedForDelivery"])
                {
                    // The order is planned for delivery so the sensible default is to cross-dock to the delivery run collection point.
                    row[C_OrderActionID] = (int)eOrderAction.Cross_Dock;
                }
                else
                {
                    // The order has not been planned for delivery so the sensible default is to deliver to the delivery point.
                    row[C_OrderActionID] = (int)eOrderAction.Default;
                    row[C_DeliverToPointID] = (int)row["DeliveryPointID"];
                    row[C_DeliverToPoint] = (string)row["DeliveryPointDescription"];
                    row[C_DeliverAtDateTime] = (DateTime)row["DeliveryDateTime"];
                    row[C_DeliverAtAnyTime] = (bool)row["DeliveryIsAnyTime"];
                }

                row[C_LoadOder] = loadOrder.ToString();
                loadOrder++;
            }

            dsOrders.AcceptChanges();

            Facade.IOrganisation facOrganisation = new Facade.Organisation();
            Entities.Organisation owner = facOrganisation.GetForIdentityId(Globals.Configuration.IdentityId);

            Facade.IPoint facPoint = new Facade.Point();
            if (owner.Defaults[0].GroupageCollectionRunDeliveryPoint > 0)
            {
                ucDeliveryPoint.SelectedPoint =
                    facPoint.GetPointForPointId(owner.Defaults[0].GroupageCollectionRunDeliveryPoint);
            }
            else 
            {
                ucDeliveryPoint.SelectedPoint = facPoint.GetPointForPointId((int)dsOrders.Tables[0].Rows[0][C_DeliverToPointID]);
            }

            dteDeliveryDate.SelectedDate = (DateTime)dsOrders.Tables[0].Rows[0][C_DeliverAtDateTime];
            if (!(bool)dsOrders.Tables[0].Rows[0][C_DeliverAtAnyTime])
                dteDeliveryTime.SelectedDate = (DateTime)dsOrders.Tables[0].Rows[0][C_DeliverAtDateTime];

            OrderData = dsOrders;
        }

        private void PopulateEntities()
        {
            
            Entities.Job job = new Orchestrator.Entities.Job();
            job.Instructions = new Orchestrator.Entities.InstructionCollection();
            job.JobType = eJobType.Groupage;
            job.BusinessTypeID = int.Parse(cboBusinessType.SelectedValue);
            job.IdentityId = Globals.Configuration.IdentityId;
            job.LoadNumber = "GRP" + Environment.TickCount.ToString().Substring(0, 3);
            job.Charge = new Orchestrator.Entities.JobCharge();
            job.Charge.JobChargeAmount = 0;
            job.Charge.JobChargeType = eJobChargeType.FreeOfCharge;

            Entities.InstructionCollection collections = new Orchestrator.Entities.InstructionCollection();
            Entities.Instruction iCollect = null;
            Entities.CollectDrop cd = null;

            Entities.InstructionCollection drops = new Orchestrator.Entities.InstructionCollection();
            Entities.Instruction iDrop = null;

            Facade.IPoint facPoint = new Facade.Point();
            Facade.IOrder facOrder = new Facade.Order();
            Entities.Point point = null;

            int collectSequence = 1;
            bool newcollection = false;
            foreach (DataRow row in OrderData.Tables[0].Rows)
            {
                #region Collections
                newcollection = false;
                int pointID = (int)row["CollectionRunDeliveryPointID"];
                DateTime bookedDateTime = (DateTime)row["CollectionRunDeliveryDateTime"];
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
                    iCollect.ClientsCustomerIdentityID = point.IdentityId; //Steve is this correct
                    iCollect.CollectDrops = new Orchestrator.Entities.CollectDropCollection();
                    iCollect.InstructionOrder = collectSequence;
                    newcollection = true;
                    collectSequence++;
                }

                cd = new Orchestrator.Entities.CollectDrop();
                cd.NoPallets = (int)row["NoPallets"];
                cd.NoCases = (int)row["Cases"];
                cd.GoodsTypeId = (int)row["GoodsTypeID"];
                cd.OrderID = (int)row["OrderID"];
                cd.Weight = (decimal)row["Weight"];
                cd.ClientsCustomerReference = row["DeliveryOrderNumber"].ToString();
                cd.Docket = row["OrderID"].ToString();

                iCollect.CollectDrops.Add(cd);
                if (newcollection)
                    collections.Add(iCollect);
                
                #endregion

                #region Deliveries
                eOrderAction orderAction = (eOrderAction)(int)row[C_OrderActionID];
                int dropSequence = 1;
                bool newdelivery = false;
                newdelivery = false;
                int deliveryPointID = (int)row[C_DeliverToPointID];
                DateTime deliveryDateTime = (DateTime)row[C_DeliverAtDateTime];
                bool deliveryIsAnyTime = (bool)row[C_DeliverAtAnyTime];
                // If the user has selected Default (i.e. Deliver) a drop instruction will be created, otherwise a trunk instruction will be
                // created.
                eInstructionType instructionType = orderAction == eOrderAction.Default ? eInstructionType.Drop : eInstructionType.Trunk;

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
                    if ((bool)row[C_DeliverAtAnyTime])
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
                #endregion
            }

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
            _jobID = facjob.Create(job, Page.User.Identity.Name);
            _openJob = true;
        }

        #endregion

        #region Validator Events

        void cvDeliveryPoint_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = true;

            foreach (DataRow row in OrderData.Tables[0].Rows)
            {
                if (row["CollectionPointID"].ToString() == row[C_DeliverToPointID].ToString())
                {
                    args.IsValid = false;
                    break;
                }
            }
        }

        void cvLoadOrder_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = true;

            //validate that the load numbers are unique
            List<string> loadOrder = new List<string>();
            foreach (GridDataItem gdi in grdOrders.Items)
            {
                TextBox txt = gdi.FindControl("txtLoadOrder") as TextBox;
                if (txt == null)
                    args.IsValid = true; // This is a detail row.
                else
                {
                    if (loadOrder.Contains(txt.Text))
                    {
                        args.IsValid = false;
                        break;
                    }
                    loadOrder.Add(txt.Text);
                }
            }
        }

        void cvCrossDockLocationError_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = true;

            // The order action Default means Deliver when in a collection job so this is renamed.
            string orderActionSelected = rblOrderAction.SelectedValue.Replace(" ", "_");
            if (orderActionSelected == "Deliver") orderActionSelected = "Default";
            int orderActionID = (int)(eOrderAction)Enum.Parse(typeof(eOrderAction), orderActionSelected);

            foreach (GridItem row in grdOrders.SelectedItems)
            {
                {
                    int orderID = int.Parse(row.OwnerTableView.DataKeyValues[row.ItemIndex]["OrderID"].ToString());

                    DataRow[] drows = OrderData.Tables[0].Select("OrderID = " + orderID.ToString());
                    if (drows.Length == 1)
                    {
                        if (orderActionID == (int)eOrderAction.Cross_Dock)
                        {
                            if ((int)drows[0]["DeliveryPointID"] == ucDeliveryPoint.SelectedPoint.PointId)
                            {
                                // Add a rule infringement here.
                                args.IsValid = false;
                                break;
                            }
                        }
                    }
                }
            }
        }
        #endregion
    }
}