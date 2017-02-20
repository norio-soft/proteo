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

namespace Orchestrator.WebUI.Groupage
{
    public partial class OrderGrid : Orchestrator.Base.BasePage
    {
        
        #region Page Load/Init

        protected void Page_Load(object sender, EventArgs e)
        {
            Security.Authorise.EnforceAuthorisation(eSystemPortion.AddEditOrder);
          

            if (!IsPostBack)
            {
                PopulateStaticControls();
               //CheckAll();
            }

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            
            btnConfirmOrders.Click += new EventHandler(btnConfirmOrders_Click);

            gvOrders2.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(gvOrders2_NeedDataSource);
            gvOrders2.ItemCreated += new Telerik.Web.UI.GridItemEventHandler(gvOrders2_ItemCreated);
            this.btnRefresh.Click += new EventHandler(btnRefresh_Click);
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

            List<int> businessTypeIDs = new List<int>();
            foreach (ListItem businessType in cboBusinessType.Items)
                if (businessType.Selected)
                    businessTypeIDs.Add(int.Parse(businessType.Value));
            
            Facade.IOrder facOrder = new Facade.Order();
            DataSet orderData = null;
            if (tdDateOptions.Visible)
                orderData = facOrder.GetOrdersForStatusAndDateRange(orderStatusIDs, plannedForCollection, plannedForDelivery, dteStartDate.SelectedDate.Value, dteEndDate.SelectedDate.Value, lastPlannedActions, businessTypeIDs);
            else
                orderData = facOrder.GetOrdersForStatus(orderStatusIDs, plannedForCollection, plannedForDelivery, lastPlannedActions, businessTypeIDs);
            gvOrders2.DataSource = orderData;

            lblOrderCount.Text = orderData.Tables[0].Rows.Count.ToString();
        }
        #endregion

        #region Private Methods

        private void CheckAll()
        {
            foreach (ListItem li in cblOrderStatus.Items)
                li.Selected = true;
            foreach (ListItem li in cboBusinessType.Items)
                li.Selected = true;

            chkCrossDock.Checked = true;
            chkTransShipped.Checked = true;
        }


        private void PopulateStaticControls()
        {
            cblOrderStatus.DataSource = Enum.GetNames(typeof(eOrderStatus));
            cblOrderStatus.DataBind();

            Facade.IBusinessType facBusinessType = new Facade.BusinessType();
            cboBusinessType.DataSource = facBusinessType.GetAll();
            cboBusinessType.DataBind();
        }


        #endregion

        #region Event Handlers

        #region Button Events
      
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

        void btnRefresh_Click(object sender, EventArgs e)
        {
            this.gvOrders2.Rebind();
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
