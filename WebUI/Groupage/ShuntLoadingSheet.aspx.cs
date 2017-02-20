using System;
using System.Collections.Generic;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Orchestrator.EF;
using Orchestrator.Entities;
using Telerik.Web.UI;
using System.Web.UI.HtmlControls;

namespace Orchestrator.WebUI.Groupage
{
    public partial class ShuntLoadingSheet : Orchestrator.Base.BasePage
    {

        private int orderGroupId = 0;
        private Color rowBackColour = System.Drawing.Color.LightGray;
        private bool isClientUser = true;
        protected int groups = 0;
        
        //------------------------------------------------------------------------------

        protected void Page_Load(object sender, EventArgs e)
        {
            if (this.User.IsInRole(((int)eUserRole.KnaufLoadingSheetInClientPortal).ToString())
                && this.User.IsInRole(((int)eUserRole.ClientUser).ToString()))
                isClientUser = true;
            else
                isClientUser = false;

            groups = 0;
            this.lblTotalGroups.Text = String.Format("Total Groups : {0}", groups);

            if (!this.IsPostBack)
            { 
                this.dteLoadingDate.SelectedDate = DateTime.Today;
                this.grdShuntLoading.Rebind();
            }
        }

        //------------------------------------------------------------------------------

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.grdShuntLoading.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdShuntLoading_NeedDataSource);
            this.grdShuntLoading.ItemDataBound += new Telerik.Web.UI.GridItemEventHandler(grdShuntLoading_ItemDataBound);
            this.btnRefresh.Click += new EventHandler(btnRefresh_Click);
            this.btnBottomRefresh.Click += new EventHandler(btnRefresh_Click);
            this.btnUpdate.Click += new EventHandler(btnUpdate_Click);
            this.btnBottomUpdate.Click += new EventHandler(btnUpdate_Click);
            this.rdoLoaded.SelectedIndexChanged += new EventHandler(rdoLoaded_SelectedIndexChanged);
        }

        //------------------------------------------------------------------------------

        protected void rdoLoaded_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.grdShuntLoading.Rebind();
        }

        //------------------------------------------------------------------------------

        protected void btnUpdate_Click(object sender, EventArgs e)
        {
            foreach(GridDataItem item in this.grdShuntLoading.Items)
            {
                if (item is Telerik.Web.UI.GridDataItem)
                {
                    #region Get inputs

                    HiddenField hidTrailerIsDirty = item.FindControl("hidTrailerIsDirty") as HiddenField;
                    HiddenField hidRowDirty = item.FindControl("hidRowDirty") as HiddenField;

                    TextBox txtPlannedtrailerRef = item.FindControl("txtPlannedtrailerRef") as TextBox;
                    TextBox txtActualtrailerRef = item.FindControl("txtActualtrailerRef") as TextBox;
                    TextBox txtDropOrder = item.FindControl("txtDropOrder") as TextBox;

                    CheckBox chkIsLiveLoader = item.FindControl("chkIsLiveLoader") as CheckBox;

                    TextBox txtPriority = item.FindControl("txtPriority") as TextBox;
                    HtmlAnchor ancOrderId = item.FindControl("ancOrderId") as HtmlAnchor;

                    TextBox txtLoadingNow = item.FindControl("txtLoadingNow") as TextBox;

                    #endregion

                    int orderId = 0;
                    bool rowDirty = false, trailerDirty = false;

                    int.TryParse(ancOrderId.InnerText, out orderId);
                    bool.TryParse(hidRowDirty.Value, out rowDirty);
                    bool.TryParse(hidTrailerIsDirty.Value, out trailerDirty);

                    // rowDirty is as a least one field except the Despatched trailer havs been changed.
                    // trailerDirty is the Despatched Trailer only.
                    if (rowDirty || trailerDirty)
                    {
                        EF.ShuntLoading shuntLoadingRow =
                        (from shuntLoading in Orchestrator.EF.DataContext.Current.ShuntLoadingSet.Include("Order")
                         where shuntLoading.OrderId == orderId
                         select shuntLoading).FirstOrDefault();

                        if (shuntLoadingRow == null) // Create new row
                        {
                            shuntLoadingRow = new ShuntLoading();
                            Orchestrator.EF.DataContext.Current.AddToShuntLoadingSet(shuntLoadingRow);
                            shuntLoadingRow.OrderId = orderId;
                            shuntLoadingRow.OrderReference.EntityKey = Orchestrator.EF.DataContext.CreateKey("OrderSet", "OrderId", orderId);
                            if (shuntLoadingRow.LoadingNow == null)
                                shuntLoadingRow.LoadingNow = string.Empty;
                        }

                        if (trailerDirty) // Only overwrite the trailer ref if the user has EXPLICITLY changed the trailer ref.
                            shuntLoadingRow.ActualTrailerRef = txtActualtrailerRef.Text;
                        
                        if (rowDirty)
                        {
                            shuntLoadingRow.PlannedTrailerRef = txtPlannedtrailerRef.Text;
                            shuntLoadingRow.SortOrder = (String.IsNullOrEmpty(txtPriority.Text)) ? new int?() : int.Parse(txtPriority.Text);
                            shuntLoadingRow.DropOrder = (String.IsNullOrEmpty(txtDropOrder.Text)) ? new int?() : int.Parse(txtDropOrder.Text);
                            shuntLoadingRow.IsLiveLoader = chkIsLiveLoader.Checked;
                            shuntLoadingRow.LoadingNow = txtLoadingNow.Text;
                        }
                    }
                }
            }

            Orchestrator.EF.DataContext.Current.SaveChanges();
            grdShuntLoading.Rebind();
        }

        //------------------------------------------------------------------------------

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            this.grdShuntLoading.Rebind();
        }

        //------------------------------------------------------------------------------

        protected void grdShuntLoading_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is Telerik.Web.UI.GridDataItem)
            {
                EF.Order order = e.Item.DataItem as EF.Order;

                if (order.OrderGroup == null)
                    if (rowBackColour != System.Drawing.Color.LightGray)
                    {
                        rowBackColour = System.Drawing.Color.LightGray;
                        e.Item.BackColor = rowBackColour;
                    }
                    else 
                    {
                        e.Item.BackColor = Color.White;
                        rowBackColour = e.Item.BackColor;
                    }
                else
                    if (order.OrderGroup.OrderGroupID != orderGroupId)
                        if (rowBackColour != System.Drawing.Color.LightGray)
                        {
                            rowBackColour = System.Drawing.Color.LightGray;
                            e.Item.BackColor = rowBackColour;
                        }
                        else
                        {
                            e.Item.BackColor = Color.White;
                            rowBackColour = e.Item.BackColor;
                        }
                    else
                    {
                        e.Item.BackColor = rowBackColour;
                    }

                if (order.OrderGroup == null || order.OrderGroup.OrderGroupID != orderGroupId)
                {
                    groups++;
                }

                orderGroupId = (order.OrderGroup == null) ? 0 : order.OrderGroup.OrderGroupID;

                HtmlAnchor orderId = e.Item.FindControl("ancOrderId") as HtmlAnchor;
                orderId.InnerHtml = order.OrderId.ToString();

                if (!isClientUser)
                    orderId.HRef = String.Format("javascript:viewOrderProfile({0})",order.OrderId);

                Label lblDeliveryTime = e.Item.FindControl("lblDeliveryTime") as Label;

                if (order.DeliveryIsAnytime)
                    lblDeliveryTime.Text = String.Empty;
                else if(order.DeliveryDateTime != order.DeliveryFromDateTime)
                    lblDeliveryTime.Text = order.DeliveryFromDateTime.Value.ToShortTimeString() + " - " +  order.DeliveryDateTime.ToShortTimeString();
                else
                    lblDeliveryTime.Text = order.DeliveryDateTime.ToShortTimeString();

                Label lblReleaseNumber = e.Item.FindControl("lblReleaseNumber") as Label;
                lblReleaseNumber.Text = order.CustomerOrderNumber;

                Label lblDeliveryTown = e.Item.FindControl("lblDeliveryPoint") as Label;
                lblDeliveryTown.Text = order.DeliveryPoint.Description + " - " + order.DeliveryPoint.Address.PostCode.Trim() ;

                Label lblWeight = e.Item.FindControl("lblWeight") as Label;
                lblWeight.Text = Convert.ToInt32(order.Weight).ToString();

                CheckBox chkIsLiveLoader = e.Item.FindControl("chkIsLiveLoader") as CheckBox;
                if (isClientUser)
                    chkIsLiveLoader.Enabled = false;

                chkIsLiveLoader.InputAttributes.Add("OrderGroupId", orderGroupId.ToString());
                chkIsLiveLoader.Checked = (order.ShuntLoadingOrder == null) ? false : order.ShuntLoadingOrder.IsLiveLoader;

                TextBox txtPlannedTrailerRef = e.Item.FindControl("txtPlannedTrailerRef") as TextBox;
                if (isClientUser)
                    txtPlannedTrailerRef.ReadOnly = true;

                txtPlannedTrailerRef.Text = (order.ShuntLoadingOrder == null) ? String.Empty : order.ShuntLoadingOrder.PlannedTrailerRef ;

                TextBox txtActualTrailerRef = e.Item.FindControl("txtActualTrailerRef") as TextBox;
                if (isClientUser)
                    txtActualTrailerRef.ReadOnly = true;

                txtActualTrailerRef.Text = (order.ShuntLoadingOrder == null) ? String.Empty : order.ShuntLoadingOrder.ActualTrailerRef;

                TextBox txtLoadingNow = e.Item.FindControl("txtLoadingNow") as TextBox;
                if (!isClientUser)
                    txtLoadingNow.ReadOnly = true;

                txtLoadingNow.Text = (order.ShuntLoadingOrder == null) ? String.Empty : order.ShuntLoadingOrder.LoadingNow;

                TextBox txtPriority = e.Item.FindControl("txtPriority") as TextBox;
                if (isClientUser)
                    txtPriority.ReadOnly = true;

                txtPriority.Text = (order.ShuntLoadingOrder == null) ? String.Empty : order.ShuntLoadingOrder.SortOrder.ToString();
                txtPriority.Attributes.Add("onchange", "javascript:HandleGroupedOrders(this);");
                txtPriority.Attributes.Add("OrderGroupId", (order.OrderGroup != null) ? order.OrderGroup.OrderGroupID.ToString() : String.Empty);

                TextBox txtDropOrder = e.Item.FindControl("txtDropOrder") as TextBox;
                if (isClientUser)
                    txtDropOrder.ReadOnly = true;
                txtDropOrder.Text = (order.ShuntLoadingOrder == null) ? String.Empty : order.ShuntLoadingOrder.DropOrder.ToString();

                this.lblTotalGroups.Text = String.Format("Total Groups : {0}", groups);
            }
        }

        //------------------------------------------------------------------------------

        protected void grdShuntLoading_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            /*
            This is a Knauf - Firmin Specific Report. It is only for Order of Knauf Drywall Ltd,  Knauf Drywall (Trunks)
            It has been deemed that the ids for such cn be hard coded. See below.

            */
            int KnaufDrywallLtd = 695;
            int KnaufDrywallTrunks = 705;


            DateTime loadingDate = this.dteLoadingDate.SelectedDate.Value;
            DateTime todate = loadingDate.AddDays(1);
            List<EF.Order> orderlist = new List<Orchestrator.EF.Order>();
            IOrderedEnumerable<EF.Order> orderlistOrdered;
            int orderStatusCancelled = (int)eOrderStatus.Cancelled;
            int orderStatusAwaitingApproval = (int)eOrderStatus.Awaiting_Approval;
            int orderStatusRejected = (int)eOrderStatus.Rejected;

            // not loaded
            if (this.rdoLoaded.SelectedValue == "0")
            {
                orderlist = (from order in
                                 Orchestrator.EF.DataContext.Current.OrderSet.Include("ShuntLoadingOrder").Include("DeliveryPoint").Include("DeliveryPoint.Address").Include("OrderGroup")
                                 where order.CollectionDateTime >= loadingDate && order.CollectionDateTime < todate
                                 && (order.ShuntLoadingOrder == null || (order.ShuntLoadingOrder.ActualTrailerRef == null
                                 || order.ShuntLoadingOrder.ActualTrailerRef == String.Empty)) && (order.OrderStatusId != orderStatusCancelled && order.OrderStatusId != orderStatusAwaitingApproval && order.OrderStatusId != orderStatusRejected)
                                 && (order.Organisation.IdentityId == KnaufDrywallLtd || order.Organisation.IdentityId == KnaufDrywallTrunks)
                                 orderby (order.OrderGroup == null) ? 0 : order.OrderGroup.OrderGroupID, order.ShuntLoadingOrder.SortOrder.HasValue ? order.ShuntLoadingOrder.SortOrder.Value : 50
                                 select order).ToList();

                this.SortOrderGroupPriority(orderlist);

                orderlistOrdered = orderlist.OrderBy(o => (o.ShuntLoadingOrder == null) ? 50 : ((o.ShuntLoadingOrder.SortOrder.HasValue) ? o.ShuntLoadingOrder.SortOrder.Value : 50))
                    .ThenBy(o => (o.OrderGroup == null) ? 0 : o.OrderGroup.OrderGroupID)
                    .ThenBy(o => (o.ShuntLoadingOrder == null) ? 999: o.ShuntLoadingOrder.DropOrder.HasValue ? o.ShuntLoadingOrder.DropOrder : 999).ThenBy(o => o.OrderId);

                this.grdShuntLoading.Columns.FindByUniqueName("actualTrailerRef").Visible = false;
                this.grdShuntLoading.Columns.FindByUniqueName("priority").Visible = true;

                this.lblTotalOrders.Text = String.Format("Total Orders : {0}", orderlist.Count);
            }
            else if (this.rdoLoaded.SelectedValue == "4")
            {
                orderlist = (from order in
                                 Orchestrator.EF.DataContext.Current.OrderSet.Include("ShuntLoadingOrder").Include("DeliveryPoint").Include("DeliveryPoint.Address").Include("OrderGroup")
                             where order.CollectionDateTime >= loadingDate && order.CollectionDateTime < todate
                             && (order.ShuntLoadingOrder == null || (order.ShuntLoadingOrder.ActualTrailerRef == null
                             || order.ShuntLoadingOrder.ActualTrailerRef == String.Empty)) && (order.OrderStatusId != orderStatusCancelled && order.OrderStatusId != orderStatusAwaitingApproval && order.OrderStatusId != orderStatusRejected)
                             && (order.Organisation.IdentityId == KnaufDrywallLtd || order.Organisation.IdentityId == KnaufDrywallTrunks)
                             && (order.ShuntLoadingOrder == null || order.ShuntLoadingOrder.IsLiveLoader == false)
                             orderby (order.OrderGroup == null) ? 0 : order.OrderGroup.OrderGroupID, order.ShuntLoadingOrder.SortOrder.HasValue ? order.ShuntLoadingOrder.SortOrder.Value : 50
                             select order).ToList();

                this.SortOrderGroupPriority(orderlist);

                orderlistOrdered = orderlist.OrderBy(o => (o.ShuntLoadingOrder == null) ? 50 : ((o.ShuntLoadingOrder.SortOrder.HasValue) ? o.ShuntLoadingOrder.SortOrder.Value : 50))
                    .ThenBy(o => (o.OrderGroup == null) ? 0 : o.OrderGroup.OrderGroupID)
                    .ThenBy(o => (o.ShuntLoadingOrder == null) ? 999 : o.ShuntLoadingOrder.DropOrder.HasValue ? o.ShuntLoadingOrder.DropOrder : 999).ThenBy(o => o.OrderId);

                this.grdShuntLoading.Columns.FindByUniqueName("actualTrailerRef").Visible = false;
                this.grdShuntLoading.Columns.FindByUniqueName("priority").Visible = true;

                this.lblTotalOrders.Text = String.Format("Total Orders : {0}", orderlist.Count);
            }
            else if (this.rdoLoaded.SelectedValue == "1") // loaded
            {
                orderlist = (from order in
                                 Orchestrator.EF.DataContext.Current.OrderSet.Include("ShuntLoadingOrder").Include("DeliveryPoint").Include("DeliveryPoint.Address").Include("OrderGroup")
                                 where order.CollectionDateTime >= loadingDate && order.CollectionDateTime < todate
                                 && (order.ShuntLoadingOrder != null && !String.IsNullOrEmpty(order.ShuntLoadingOrder.ActualTrailerRef))
                                 && (order.OrderStatusId != orderStatusCancelled && order.OrderStatusId != orderStatusAwaitingApproval && order.OrderStatusId != orderStatusRejected)
                                 && (order.Organisation.IdentityId == KnaufDrywallLtd || order.Organisation.IdentityId == KnaufDrywallTrunks)
                                 orderby (order.OrderGroup == null) ? 0 : order.OrderGroup.OrderGroupID, order.ShuntLoadingOrder.SortOrder.HasValue ? order.ShuntLoadingOrder.SortOrder.Value : 50
                                 select order).ToList();

                this.SortOrderGroupPriority(orderlist);

                orderlistOrdered = orderlist.OrderBy(o => (o.ShuntLoadingOrder == null) ? 50 : ((o.ShuntLoadingOrder.SortOrder.HasValue) ? o.ShuntLoadingOrder.SortOrder.Value : 50))
                    .ThenBy(o => (o.OrderGroup == null) ? 0 : o.OrderGroup.OrderGroupID)
                    .ThenBy(o => (o.ShuntLoadingOrder == null) ? 999 : o.ShuntLoadingOrder.DropOrder.HasValue ? o.ShuntLoadingOrder.DropOrder : 999).ThenBy(o => o.OrderId);

                this.grdShuntLoading.Columns.FindByUniqueName("actualTrailerRef").Visible = true;
                this.grdShuntLoading.Columns.FindByUniqueName("priority").Visible = true;

                this.lblTotalOrders.Text = String.Format("Total Orders : {0}", orderlist.Count);
            }
            else //both
            {
                orderlist = (from order in
                                 Orchestrator.EF.DataContext.Current.OrderSet.Include("ShuntLoadingOrder").Include("DeliveryPoint").Include("DeliveryPoint.Address").Include("OrderGroup")
                                 where order.CollectionDateTime >= loadingDate && order.CollectionDateTime < todate
                                 && (order.OrderStatusId != orderStatusCancelled && order.OrderStatusId != orderStatusAwaitingApproval && order.OrderStatusId != orderStatusRejected)
                                 && (order.Organisation.IdentityId == KnaufDrywallLtd || order.Organisation.IdentityId == KnaufDrywallTrunks)
                                 orderby order.OrderGroup.OrderGroupID,order.OrderId
                                 select order).ToList();

                this.SortOrderGroupPriority(orderlist);

                orderlistOrdered = orderlist.OrderBy(o => (o.OrderGroup == null) ? 0 : o.OrderGroup.OrderGroupID).ThenBy(o => o.OrderId);

                this.grdShuntLoading.Columns.FindByUniqueName("actualTrailerRef").Visible = true;
                this.grdShuntLoading.Columns.FindByUniqueName("priority").Visible = false;

                this.lblTotalOrders.Text = String.Format("Total Orders : {0}", orderlist.Count);
            }

            grdShuntLoading.DataSource = orderlistOrdered;
        }

        //------------------------------------------------------------------------------

        private void SortOrderGroupPriority(List<EF.Order> orderlist)
        {
            int priority = 50;
            int orderGroupId = 0;
            foreach (EF.Order ord in orderlist)
            {
                if (ord.OrderGroup != null)
                    if (orderGroupId == 0 || orderGroupId != ord.OrderGroup.OrderGroupID)
                    {
                        orderGroupId = ord.OrderGroup.OrderGroupID;
                        priority = (ord.ShuntLoadingOrder == null) ? 50 : ((ord.ShuntLoadingOrder.SortOrder.HasValue) ? ord.ShuntLoadingOrder.SortOrder.Value : 50);

                        if (ord.ShuntLoadingOrder == null)
                        {
                            ord.ShuntLoadingOrder = new ShuntLoading();
                            Orchestrator.EF.DataContext.Current.AddToShuntLoadingSet(ord.ShuntLoadingOrder);
                            ord.ShuntLoadingOrder.OrderId = ord.OrderId;
                            ord.ShuntLoadingOrder.OrderReference.EntityKey = Orchestrator.EF.DataContext.CreateKey("OrderSet", "OrderId", ord.OrderId);
                            ord.ShuntLoadingOrder.PlannedTrailerRef = String.Empty;
                            ord.ShuntLoadingOrder.ActualTrailerRef = String.Empty;
                            ord.ShuntLoadingOrder.SortOrder = priority;
                            ord.ShuntLoadingOrder.DropOrder = new int?();
                        }
                    }
                    else if (ord.OrderGroup.OrderGroupID == orderGroupId)
                    {
                        if (ord.ShuntLoadingOrder != null )
                            ord.ShuntLoadingOrder.SortOrder = priority;
                        else //create
                        {
                            ord.ShuntLoadingOrder = new ShuntLoading();
                            Orchestrator.EF.DataContext.Current.AddToShuntLoadingSet(ord.ShuntLoadingOrder);
                            ord.ShuntLoadingOrder.OrderId = ord.OrderId;
                            ord.ShuntLoadingOrder.OrderReference.EntityKey = Orchestrator.EF.DataContext.CreateKey("OrderSet", "OrderId", ord.OrderId);
                            ord.ShuntLoadingOrder.PlannedTrailerRef = String.Empty;
                            ord.ShuntLoadingOrder.ActualTrailerRef = String.Empty;
                            ord.ShuntLoadingOrder.SortOrder = priority;
                            ord.ShuntLoadingOrder.DropOrder = new int?();
                        }
                    }
                    else
                    {
                        orderGroupId = 0;
                        priority = 50;
                    }
            }
        }

        //------------------------------------------------------------------------------

    }
}
