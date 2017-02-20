using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using System.Collections.Generic;
using Telerik.Web.UI;
using System.Xml.Serialization;

namespace Orchestrator.WebUI
{
    public partial class Groupage_DeliveryJob : Orchestrator.Base.BasePage
    {
        private const string C_OrderData_VS = "C_OrderData_VS";
        private const string C_OrdersString_VS = "C_OrdersString_VS";
        private const string C_BusinessTypeIDs = "BusinessTypeIDs";
        private const string C_GRID_NAME = "__deliveriesGrid";

        #region Private Fields

        string _owner = string.Empty;
        string _collectionPoint = string.Empty;
        string _collectionPointTown = string.Empty;
        bool _openJob = false;
        int _jobID = 0;
        private int _palletRunningTotal = 0;
        private decimal _weightRunningTotal = 0;
        private decimal _rateRunningTotal = 0;

        #endregion

        #region Page properties
        protected IEnumerable<int> BusinessTypeIDs
        {
            get { return (IEnumerable<int>)ViewState[C_BusinessTypeIDs]; }
            set { ViewState[C_BusinessTypeIDs] = value; }
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
            if (PreviousPage != null && PreviousPage.IsCrossPagePostBack)
            {
                OrdersCSV = PreviousPage.Orders;
            }

            if (!IsPostBack && Request.QueryString["rcbID"] == null)
            {
                this.BusinessTypeIDs = PreviousPage.BusinessTypeIDs;
                PrepareOrders();
            }

            if (ucCollectionPoint.IsPostBack && ucCollectionPoint.PointID > 0)
                hidIsUpdatePoint.Value = "true";
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.btnCreateJob.Click += new EventHandler(btnCreateJob_Click);
            this.btnChangePlannedColletionPoint.Click += new EventHandler(btnChangePlannedColletionPoint_Click);
            this.grdOrders.NeedDataSource += new GridNeedDataSourceEventHandler(grdOrders_NeedDataSource);

            this.grdOrders.DetailTableDataBind += new GridDetailTableDataBindEventHandler(grdOrders_DetailTableDataBind);
            this.grdOrders.ItemDataBound += new GridItemEventHandler(grdOrders_ItemDataBound);
            grdOrders.ItemCommand += new GridCommandEventHandler(grdOrders_ItemCommand);

            this.btnResetCollectionPoint.Click += new EventHandler(btnResetCollectionPoint_Click);

            // Restores the column ordering filters and sorting for the user
            LoadGridSettings();
        }
        #endregion


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
                        Response.Redirect("Deliveries.aspx?" + this.CookieSessionID);
                    else
                        PrepareOrders();
                    break;
            }
        }

        int i = 1;
        int _runningOrder = -1;
        void grdOrders_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                DataRowView drv = (DataRowView)e.Item.DataItem;
                HtmlInputHidden hidJobOrder = e.Item.FindControl("hidJobOrder") as HtmlInputHidden;
                hidJobOrder.Value = (++_runningOrder).ToString();
                
                _palletRunningTotal += (int)drv["NoPallets"];
                _weightRunningTotal += (decimal)drv["Weight"];
                _rateRunningTotal += (decimal)drv["Rate"];

                string deliveryAddress = string.Empty;
                if (drv["AddressLine1"] != DBNull.Value && !string.IsNullOrEmpty(drv["AddressLine1"].ToString()))
                    deliveryAddress = drv["AddressLine1"].ToString();
                if (drv["AddressLine2"] != DBNull.Value && !string.IsNullOrEmpty(drv["AddressLine2"].ToString()))
                    deliveryAddress += "<br/>" + drv["AddressLine2"].ToString();

                if (drv["AddressLine3"] != DBNull.Value && !string.IsNullOrEmpty(drv["AddressLine3"].ToString()))
                    deliveryAddress += "<br/>" + drv["AddressLine3"].ToString();

                if (drv["PostTown"] != DBNull.Value && !string.IsNullOrEmpty(drv["PostTown"].ToString()))
                    deliveryAddress += "<br/>" + drv["PostTown"].ToString();
                if (drv["County"] != DBNull.Value && !string.IsNullOrEmpty(drv["County"].ToString()))
                    deliveryAddress += "<br/>" + drv["County"].ToString();
                if (drv["PostCode"] != DBNull.Value && !string.IsNullOrEmpty(drv["PostCode"].ToString()))
                    deliveryAddress += "<br/>" + drv["PostCode"].ToString();

                Literal litAddress = e.Item.FindControl("litAddress") as Literal;
                litAddress.Text = deliveryAddress;

                Literal litReferences = e.Item.FindControl("litReferences") as Literal;
                litReferences.Text = drv["CustomerOrderNumber"] + ", " + drv["DeliveryOrderNumber"];
            }
            else if (e.Item is GridFooterItem)
            {
                Label lblPalletTotal = (Label)e.Item.FindControl("lblPalletTotal");
                Label lblWeightTotal = (Label)e.Item.FindControl("lblWeightTotal");
                Label lblRateTotal = (Label)e.Item.FindControl("lblRateTotal");

                if (lblPalletTotal != null && lblWeightTotal != null && lblRateTotal != null)
                {
                    lblPalletTotal.Text = _palletRunningTotal.ToString();
                    lblWeightTotal.Text = _weightRunningTotal.ToString("N2") + " kg";
                    lblRateTotal.Text = _rateRunningTotal.ToString("C");
                }
            }
        }

        void grdOrders_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            if (OrderData != null)
            {
                DataView dv = OrderData.Tables[0].DefaultView;
                //dv.Sort = "LoadOrder ASC";
                grdOrders.DataSource = dv;
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

        #region Button Event Handlers

        

        void setDeliveryOrder()
        {
            //validate that the order can be done.
            if (Page.IsValid)
            {
                //reorder the drid based on the values typed id;
                int delOrder = 0;
                foreach (GridDataItem gdi in grdOrders.Items)
                {
                    HtmlInputHidden hidJobOrder = gdi.FindControl("hidJobOrder") as HtmlInputHidden;

                    int orderID = int.Parse(gdi.GetDataKeyValue("OrderID").ToString());
                    DataRow[] drows = OrderData.Tables[0].Select("OrderID = " + orderID.ToString());
                    if (drows.Length == 1)
                    {
                        drows[0]["DeliveryOrder"] = int.Parse(hidJobOrder.Value);
                        delOrder++;    
                    }

                }

                this.OrderData.AcceptChanges();
            }
        }


        void btnCreateJob_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                setDeliveryOrder();
                PopulateEntities();
                Session[C_GRID_NAME] = null;
            }
        }

        void btnResetCollectionPoint_Click(object sender, EventArgs e)
        {
            Facade.IOrder facOrder = new Facade.Order();
            DataSet dsOrders = facOrder.GetOrdersForList(OrdersCSV, true, false);
            dsOrders.Tables[0].Columns.Add("CollectFromPointID", typeof(int));
            dsOrders.Tables[0].Columns.Add("CollectFromPoint", typeof(string));
            dsOrders.Tables[0].Columns.Add("CollectAtDateTime", typeof(DateTime));
            dsOrders.Tables[0].Columns.Add("CollectAtAnyTime", typeof(bool));
            dsOrders.Tables[0].Columns.Add("LoadOrder", typeof(string));
            dsOrders.Tables[0].Columns.Add("DeliveryOrder", typeof(int));

            foreach (DataRow row in dsOrders.Tables[0].Rows)
            {
                row["CollectFromPointID"] = (int)row["DeliveryRunCollectionPointID"];
                row["CollectFromPoint"] = row["DeliveryRunCollectionPointDescription"].ToString();
                row["CollectAtDateTime"] = (DateTime)row["DeliveryRunCollectionDateTime"];
                row["CollectAtAnyTime"] = (bool)row["DeliveryRunCollectionIsAnyTime"];

            }

            dsOrders.AcceptChanges();
            OrderData = dsOrders;
            grdOrders.Rebind();
        }

        void btnChangePlannedColletionPoint_Click(object sender, EventArgs e)
        {
            if (grdOrders.SelectedItems.Count == 0)
            {
                // Alert the user that they have not selected any orders to update.
                lblNote.Text = "No orders were selected to update, please select some orders and try again.";
                pnlConfirmation.Visible = true;
            }
            else
            {
                bool isAnyTime = false;
                int pointId = ucCollectionPoint.SelectedPoint.PointId;
                DateTime newCollectionDateTime = dteCollectionDate.SelectedDate.Value;
                newCollectionDateTime = newCollectionDateTime.Subtract(newCollectionDateTime.TimeOfDay);
                if (dteCollectionTime.SelectedDate.HasValue == false)
                {
                    isAnyTime = true;
                    newCollectionDateTime = newCollectionDateTime.Add(new TimeSpan(23, 59, 59));
                }
                else
                    newCollectionDateTime = newCollectionDateTime.Add(dteCollectionTime.SelectedDate.Value.TimeOfDay);

                bool changeMade = false;
                foreach (GridItem row in grdOrders.SelectedItems)
                {
                    int orderID = int.Parse(row.OwnerTableView.DataKeyValues[row.ItemIndex]["OrderID"].ToString());
                    DataRow[] drows = OrderData.Tables[0].Select("OrderID = " + orderID.ToString());
                    if (drows.Length == 1)
                    {
                        drows[0]["CollectFromPointID"] = pointId;
                        drows[0]["CollectFromPoint"] = ucCollectionPoint.SelectedPoint.Description;
                        drows[0]["CollectAtDateTime"] = newCollectionDateTime;
                        drows[0]["CollectAtAnyTime"] = isAnyTime;
                        changeMade = true;
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

        #region Server Validation

        

        #endregion
        
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

        private void PrepareOrders()
        {
            if (!IsPostBack)
                OrdersCSV = PreviousPage.Orders;

            Facade.IOrder facOrder = new Facade.Order();
            DataSet dsOrders = facOrder.GetOrdersForList(OrdersCSV, true, false);

            Facade.IOrganisation facOrg = new Facade.Organisation();
            Entities.Organisation org = facOrg.GetForIdentityId(Globals.Configuration.IdentityId);
            Facade.IPoint facPoint = new Facade.Point();
            Entities.Point deliveryRunCollectionPoint = null;
            if (org.Defaults[0].GroupageDeliveryRunCollectionPoint > 0)
                deliveryRunCollectionPoint = facPoint.GetPointForPointId(org.Defaults[0].GroupageDeliveryRunCollectionPoint);

            dsOrders.Tables[0].Columns.Add("CollectFromPointID", typeof(int));
            dsOrders.Tables[0].Columns.Add("CollectFromPoint", typeof(string));
            dsOrders.Tables[0].Columns.Add("CollectAtDateTime", typeof(DateTime));
            dsOrders.Tables[0].Columns.Add("CollectAtAnyTime", typeof(bool));
            dsOrders.Tables[0].Columns.Add("LoadOrder", typeof(string));
            dsOrders.Tables[0].Columns.Add("DeliveryOrder", typeof(int));

            int loadOrder = 1;
            foreach (DataRow row in dsOrders.Tables[0].Rows)
            {
                row["CollectFromPointID"] = (int)row["DeliveryRunCollectionPointID"];
                row["CollectFromPoint"] = row["DeliveryRunCollectionPointDescription"].ToString();
                row["CollectAtDateTime"] = (DateTime)row["DeliveryRunCollectionDateTime"];
                row["CollectAtAnyTime"] = (bool)row["DeliveryRunCollectionIsAnyTime"];
                if (loadOrder == dsOrders.Tables[0].Rows.Count)
                    row["LoadOrder"] = "L";
                else
                    row["LoadOrder"] = loadOrder.ToString();
                loadOrder++;

                // Set the default collection point to that specified by the organisation settings 
                if (row["LastPlannedOrderActionID"] == DBNull.Value)
                {
                    if (deliveryRunCollectionPoint != null)
                    {
                        row["CollectFromPointID"] = deliveryRunCollectionPoint.PointId;
                        row["CollectFromPoint"] = deliveryRunCollectionPoint.Description;
                    }
                }

            }

            dsOrders.AcceptChanges();
            OrderData = dsOrders;
            grdOrders.Rebind();
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

            bool newcollection = false;
            foreach (DataRow row in OrderData.Tables[0].Rows)
            {
                #region Collections
                newcollection = false;
                int pointID = (int)row["CollectFromPointID"];
                DateTime bookedDateTime = (DateTime)row["CollectAtDateTime"];
                bool collectionIsAnytime = (bool)row["CollectAtAnyTime"];

                // if this setting is true then we want to create a new instruction for the order.
                if (Globals.Configuration.OneDropAndLoadInstructionPerOrder)
                    iCollect = null;
                else
                    iCollect = collections.GetForInstructionTypeAndPointID(eInstructionType.Load, pointID, bookedDateTime, collectionIsAnytime);

                if (iCollect == null)
                {
                    iCollect = new Orchestrator.Entities.Instruction();
                    iCollect.InstructionTypeId = (int)eInstructionType.Load;
                    iCollect.BookedDateTime = bookedDateTime;
                    if ((bool)row["CollectAtAnytime"])
                        iCollect.IsAnyTime = true;
                    point = facPoint.GetPointForPointId(pointID);
                    iCollect.PointID = pointID;
                    iCollect.Point = point;
                    iCollect.ClientsCustomerIdentityID = point.IdentityId; //Steve is this correct
                    iCollect.CollectDrops = new Orchestrator.Entities.CollectDropCollection();
                    newcollection = true;
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
            }

            // Add the Drops in the Order specified
            DataView dvDrops = OrderData.Tables[0].DefaultView;
            dvDrops.Sort = "DeliveryOrder ASC";
            DataTable dtDrops = dvDrops.ToTable();
            foreach (DataRow row in dtDrops.Rows)
            {
                #region Deliveries
                bool newdelivery = false;
                newdelivery = false;
                int deliveryPointID = (int)row["DeliveryPointID"];
                DateTime deliveryDateTime = (DateTime)row["DeliveryDateTime"];
                bool deliveryIsAnyTime = (bool)row["DeliveryIsAnyTime"];

                // if this setting is true then we want to create a new instruction for the order.
                if (Globals.Configuration.OneDropAndLoadInstructionPerOrder)
                    iDrop = null;
                else
                    iDrop = drops.GetForInstructionTypeAndPointID(eInstructionType.Drop, deliveryPointID, deliveryDateTime, deliveryIsAnyTime);

                if (iDrop == null)
                {
                    iDrop = new Orchestrator.Entities.Instruction();
                    iDrop.InstructionTypeId = (int)eInstructionType.Drop;
                    iDrop.BookedDateTime = deliveryDateTime;
                    if ((bool)row["DeliveryIsAnytime"])
                        iDrop.IsAnyTime = true;
                    point = facPoint.GetPointForPointId(deliveryPointID);
                    iDrop.ClientsCustomerIdentityID = point.IdentityId;
                    iDrop.PointID = deliveryPointID;
                    iDrop.Point = point;

                    iDrop.CollectDrops = new Orchestrator.Entities.CollectDropCollection();
                    newdelivery = true;
                }

                cd = new Orchestrator.Entities.CollectDrop();
                cd.NoPallets = (int)row["NoPallets"];
                cd.NoCases = (int)row["Cases"];
                cd.GoodsTypeId = (int)row["GoodsTypeID"];
                cd.OrderID = (int)row["OrderID"];
                cd.Weight = (decimal)row["Weight"];
                cd.ClientsCustomerReference = row["DeliveryOrderNumber"].ToString();
                cd.Docket = row["OrderID"].ToString();

                iDrop.CollectDrops.Add(cd);
                if (newdelivery)
                    drops.Add(iDrop); //Stephen Newman 23/04/07 Changed to insert the drop to the front of the list as the sort processed later seems to swap objects if equal.

                facOrder.UpdateForDeliveryRun(cd.OrderID, iCollect.PointID, iCollect.BookedDateTime, iCollect.IsAnyTime, Page.User.Identity.Name);

                #endregion
            }




            #region Add the Instructions to the job

            if (job.Instructions == null)
                job.Instructions = new Entities.InstructionCollection();

            foreach (Entities.Instruction instruction in collections)
            {
                job.Instructions.Add(instruction);
            }

            // removed by t.lunken 23/08/07 as this is now manually done.
            //drops.Sort(eInstructionSortType.DropDateTime); 
            foreach (Entities.Instruction instruction in drops)
            {
                job.Instructions.Add(instruction);
            }
            #endregion

            Facade.IJob facjob = new Facade.Job();
            job.JobState = eJobState.Booked;
            int jobID = facjob.Create(job, Page.User.Identity.Name);
            _jobID = jobID;
            _openJob = true;
        }

        protected void odsOrders_Selecting(object sender, ObjectDataSourceSelectingEventArgs e)
        {
            e.InputParameters.Add("orders", OrdersCSV);
        }

        protected void btnSaveGridSettings_Click(object sender, EventArgs e)
        {
            Utilities.SaveGridSettings(this.grdOrders, eGrid.DeliveryJob, Page.User.Identity.Name);
        }

        private void LoadGridSettings()
        {
            Utilities.LoadSettings(this.grdOrders, eGrid.DeliveryJob, Page.User.Identity.Name);
        }
    }
}
