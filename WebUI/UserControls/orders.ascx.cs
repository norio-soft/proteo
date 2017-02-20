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
using Orchestrator;
using Orchestrator.Globals;

using Telerik.Web.UI;

namespace Orchestrator.WebUI
{
    public partial class orders : System.Web.UI.UserControl
    {
        #region Control Fields
        private readonly string VS_SELECTION_DATE       = "_selectionDateTime";
        private readonly string VS_SELECTION_ISANYTIME  = "_selectionIsAnytime";
        private readonly string VS_SELECTED_ORDERS      = "_selectedOrders";
        private readonly string VS_PRESELECTED_POINT    = "_preSelectedPoint";
        private readonly string VS_PRESELECTED_ORDERS   = "_preSelectedOrders";
        private readonly string VS_EXCLUDED_ORDERS      = "_excludedOrders";
        private readonly string VS_BUSINESS_TYPE_ID     = "_businessTypeID";

        #endregion

        #region Control Properties

        /// <summary>
        /// Controls the business type ids of the orders to view
        /// </summary>
        public List<int> BusinessTypeIDs
        {
            set { this.ViewState[VS_BUSINESS_TYPE_ID] = value; }
            get
            {
                if (this.ViewState[VS_BUSINESS_TYPE_ID] == null)
                    return new List<int>();
                else
                    return (List<int>)this.ViewState[VS_BUSINESS_TYPE_ID];
            }
        }

        /// <summary>
        /// This is used to order the grid and highlight any orders with the same date... and time then date...
        /// </summary>
        public DateTime CollectionDate
        {
            set { this.ViewState[VS_SELECTION_DATE] = value; }
            get {
                if (this.ViewState[VS_SELECTION_DATE] == null) 
                    return DateTime.MaxValue;
                else
                    return (DateTime)this.ViewState[VS_SELECTION_DATE];
            }
        }

        public bool CollectionIsAnytime
        {
            set { this.ViewState[VS_SELECTION_ISANYTIME] = value; }
            get {
                if (this.ViewState[VS_SELECTION_ISANYTIME] == null)
                    return true;
                else
                    return (bool)this.ViewState[VS_SELECTION_ISANYTIME];
            }
        }

        public List<int> ExcludedOrderIDs
        {
            get { return (List<int>)this.ViewState[VS_EXCLUDED_ORDERS]; }
            set { this.ViewState[VS_EXCLUDED_ORDERS] = value; }
        }

        public List<int> PreSelectedOrderIDs
        {
            get { return (List<int>)this.ViewState[VS_PRESELECTED_ORDERS]; }
            set { this.ViewState[VS_PRESELECTED_ORDERS] = value; }
        }

        public List<int> OrderIDs
        {
            get
            {
                List<int> retVal = new List<int>();
                foreach (GridDataItem row in gvOrders2.SelectedItems)
                {
                    int orderID = int.Parse(row.OwnerTableView.DataKeyValues[row.ItemIndex]["OrderID"].ToString());
                    retVal.Add(orderID);
                }
                return retVal;

            }
        }

        public string OrderIDsAsString
        {
            get
            {
                string retVal = string.Empty;
                foreach (int i in OrderIDs)
                {
                    retVal += i.ToString() + ",";
                }
                if (retVal.EndsWith(",")) retVal = retVal.Substring(0, retVal.Length - 1);

                return retVal;
            }
        }

        public Entities.Point CollectionPoint
        {
            set
            {
                this.ViewState[VS_PRESELECTED_POINT] = value;
            }
            get { return (Entities.Point)this.ViewState[VS_PRESELECTED_POINT]; }
        }

       
        public bool ShowDateFilter
        {
            get { return pnlDateFilter.Visible; }
            set { pnlDateFilter.Visible = value; }
        }

        public bool ShowFilterOptions
        {
            set
            {
                pnlOptionsContent.Visible = value;
                pnlOptionsHeader.Visible = value;
            }
        }

        public bool ShowAddOrderButton
        {
            set { this.btnAddOrder.Visible = false; }
        }

        public bool ShowSelectOrdersButton
        {
            set { this.btnSelect.Visible = false; }
        }

        public bool ShowClearFiltersButton
        {
            set { this.btnClearFilters.Visible = false; }
        }

        public bool AllowFilteringByColumn
        {
            set { this.gvOrders2.MasterTableView.AllowFilteringByColumn = value; }
        }

        public bool UseStaticHeaders
        {
            set
            {
                this.gvOrders2.ClientSettings.Scrolling.UseStaticHeaders = value;
                this.gvOrders2.ClientSettings.Scrolling.AllowScroll = value;
            }
        }

        public int StaticHeight
        {
            set
            {
                this.gvOrders2.ClientSettings.Scrolling.ScrollHeight = value;
            }
        }

        public Unit Width
        {
            set
            {
                this.gvOrders2.Width = value;
                
            }
        }

        #endregion

        #region Page Load/Init
        protected void Page_Load(object sender, EventArgs e)
        {
            Security.Authorise.EnforceAuthorisation(eSystemPortion.AddEditOrder);



            if (!IsPostBack)
            {
                cblOrderStatus.DataSource = Enum.GetNames(typeof(eOrderStatus));
                cblOrderStatus.DataBind();
                CheckAll();

                SetDisplay();
            }

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.btnRefresh.Click += new EventHandler(btnRefresh_Click);
            this.btnClearFilters.Click +=new EventHandler(btnClearFilters_Click);
            this.btnSelect.Click += new EventHandler(btnSelect_Click);
            this.gvOrders2.NeedDataSource +=new GridNeedDataSourceEventHandler(gvOrders2_NeedDataSource);

            this.gvOrders2.ItemDataBound += new GridItemEventHandler(gvOrders2_ItemDataBound);
        }


        

        

        
        
        #endregion

        #region Grid Events
        void gvOrders2_ItemCreated(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is Telerik.Web.UI.GridDataItem)
            {
                Telerik.Web.UI.GridDataItem gridItem = e.Item as Telerik.Web.UI.GridDataItem;
                foreach (Telerik.Web.UI.GridColumn column in gvOrders2.Columns)
                {
                    if (column is Telerik.Web.UI.GridBoundColumn)
                    {
                        gridItem[column.UniqueName].ToolTip = gridItem[column.UniqueName].Text;

                    }
                }
            }
        }


        void gvOrders2_ItemDataBound(object sender, GridItemEventArgs e)
        {
            DateTime collectionDateTime = DateTime.MaxValue;
            if (e.Item is GridDataItem)
            {
                // This is a Patch until SP2 of the Telerik Grid
                CheckBox checkboxSelectColumn = ((e.Item as GridDataItem)["checkboxSelectColumn"]).Controls[1] as CheckBox;
                DataRowView drv = e.Item.DataItem as DataRowView;

                // If we have a list of points to pre-select then use this as well
                if (PreSelectedOrderIDs != null && PreSelectedOrderIDs.Contains((int)drv["OrderID"]))
                {
                    e.Item.Selected = true;
                    checkboxSelectColumn.Checked = true;
                }
                else if (ExcludedOrderIDs != null && ExcludedOrderIDs.Contains((int)drv["OrderID"]))
                {
                    checkboxSelectColumn.Enabled = false;
                }
            }
        }


        void gvOrders2_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            // Determine the parameters
            bool plannedForCollection = chkPlannedForCollection.Checked;
            bool plannedForDelivery = chkPlannedForDelivery.Checked;
            List<int> orderStatusIDs = new List<int>();
            foreach (ListItem li in cblOrderStatus.Items)
            {
                if (li.Selected)
                    orderStatusIDs.Add((int)((eOrderStatus)Enum.Parse(typeof(eOrderStatus), li.Value)));
            }
            List<int> lastPlannedActions = new List<int>();
            if (chkCrossDock.Checked) lastPlannedActions.Add((int)eOrderAction.Cross_Dock);
            if (chkTransShipped.Checked) lastPlannedActions.Add((int)eOrderAction.Trans_Ship);
            if (chkLeaveOnTrailer.Checked) lastPlannedActions.Add((int)eOrderAction.Leave_On_Trailer);


            Facade.IOrder facOrder = new Facade.Order();
            DataSet orderData = null;

            if (this.CollectionPoint != null || this.CollectionDate != DateTime.MaxValue)
            {
                if (this.CollectionPoint == null)
                    throw new ArgumentException("If you are proving a Collection Date you must also specify the collection point.");

                if (this.CollectionPoint != null && this.CollectionDate == DateTime.MaxValue)
                    throw new ArgumentException("If you are proving a Collection Point you must also specify the Collection Date and Time.");

                if (this.CollectionPoint != null)
                {
                    if (this.CollectionDate == DateTime.MaxValue)
                        throw new ArgumentException("If you are proving a Collection Date you must also specify the collection point.");

                    // Get the orders that we can add to this existing collection
                    orderData = facOrder.GetOrdersToAddToExistingCollection(this.CollectionDate, this.CollectionPoint.PointId, this.BusinessTypeIDs.Count == 1 ? this.BusinessTypeIDs[0] : 0);
                }
                else if (!this.ShowDateFilter)
                    orderData = facOrder.GetOrdersForStatus(orderStatusIDs, plannedForCollection, plannedForDelivery, lastPlannedActions, this.BusinessTypeIDs);
                else
                    orderData = facOrder.GetOrdersForStatusAndDateRange(orderStatusIDs, plannedForCollection, plannedForDelivery, dteStartDate.SelectedDate.Value, dteEndDate.SelectedDate.Value, lastPlannedActions, this.BusinessTypeIDs);

                if (ExcludedOrderIDs != null && ExcludedOrderIDs.Count > 0)
                    orderData.Tables[0].DefaultView.RowFilter = "OrderID NOT IN (" + Entities.Utilities.GetCSV(ExcludedOrderIDs) + ")";

                gvOrders2.Visible = true;
                gvOrders2.DataSource = orderData;
            }
            else
                gvOrders2.Visible = false;

            //lblOrderCount.Text = orderData.Tables[0].Rows.Count.ToString();

            //if (this.CollectionDate != DateTime.MaxValue)
            //{
            //    GridSortExpression expression = new GridSortExpression();
            //    expression.FieldName = "CollectionDateTime";
            //    expression.SortOrder = GridSortOrder.Ascending;
            //    this.gvOrders2.MasterTableView.SortExpressions.AddSortExpression(expression);
            //}

            //if (!IsPostBack && this.CollectionPoint != null)
            //{
            //    gvOrders2.MasterTableView.FilterExpression = "(CollectionPointDescription LIKE \'%" + this.CollecttionPoint.Description + "%\')";
            //    GridColumn col = gvOrders2.MasterTableView.GetColumnSafe("CollectionPointDescription");
            //    col.CurrentFilterFunction = GridKnownFunction.Contains;
            //    col.CurrentFilterValue = this.SelectedPoint.Description;
            //}
        }

    
        #endregion

        #region Private Methods

        private void SetDisplay()
        {
            if (CollectionPoint != null)
            {
                this.ShowFilterOptions = false;
                this.gvOrders2.AllowFilteringByColumn = false;
                this.gvOrders2.MasterTableView.AllowFilteringByColumn = false;
            }
        }
        private void CheckAll()
        {
            foreach (ListItem li in cblOrderStatus.Items)
            {
                li.Selected = true;
            }

            chkCrossDock.Checked = true;
            chkTransShipped.Checked = true;
        }

        #endregion

        #region Public Methods

        public void Rebind()
        {
            gvOrders2.Rebind();
        }

        #endregion

        #region Event Handlers

        #region Button Events
        void btnTest_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Trace.Write(gvOrders2.SelectedItems.Count);
        }

        void btnRefresh_Click(object sender, EventArgs e)
        {
            gvOrders2.Rebind();
        }
        
        void btnClearFilters_Click(object sender, EventArgs e)
        {
            foreach (GridColumn column in gvOrders2.MasterTableView.Columns)
            {
                column.CurrentFilterFunction = GridKnownFunction.NoFilter;
                column.CurrentFilterValue = String.Empty;
            }
            gvOrders2.MasterTableView.FilterExpression = String.Empty;
            gvOrders2.MasterTableView.Rebind();

        }

        void btnConfirmOrders_Click(object sender, EventArgs e)
        {
            // If the user can approve orders, automatically mark these orders as approved.
            bool autoApprove = Security.Authorise.CanAccess(eSystemPortion.ApproveOrder);
            eOrderStatus newOrderStatus = autoApprove ? eOrderStatus.Approved : eOrderStatus.Awaiting_Approval;

            List<int> orderIDs = new List<int>();
            foreach (GridItem row in gvOrders2.SelectedItems)
            {
                int orderID = int.Parse(row.OwnerTableView.DataKeyValues[row.ItemIndex]["OrderID"].ToString());
                orderIDs.Add(orderID);
            }
            if (orderIDs.Count > 0)
            {
                Facade.IOrder facOrder = new Facade.Order();
                facOrder.Update(orderIDs, newOrderStatus, ((Entities.CustomPrincipal)Page.User).UserName);
            }

        }

        void btnSelect_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #region Grid View Events



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

        #endregion
    }
}