namespace Orchestrator.WebUI.UserControls
{
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

    public partial class InstructionOrders : System.Web.UI.UserControl
    {
        #region Control Fields

        private readonly int C_CollectFromColumnIndex = 2;
        private readonly int C_CollectAtColumnIndex = 3;

        private readonly string VS_INSTRUCTION = "_instruction";
        private readonly string VS_SHOWCOLLECTIONCOLUMNS = "_showCollectionColumns";

        #endregion

        #region Property Interface

        public global::Orchestrator.Entities.Instruction Instruction
        {
            get { return (Orchestrator.Entities.Instruction)ViewState[VS_INSTRUCTION]; }
            set { ViewState[VS_INSTRUCTION] = value; }
        }

        public bool ShowCollectionColumns
        {
            get { return ViewState[VS_SHOWCOLLECTIONCOLUMNS] != null ? (bool)ViewState[VS_SHOWCOLLECTIONCOLUMNS] : true; }
            set { ViewState[VS_SHOWCOLLECTIONCOLUMNS] = value; }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(Orchestrator.eSystemPortion.GeneralUsage);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            gvOrders.ItemCreated += new Telerik.Web.UI.GridItemEventHandler(gvOrders_ItemCreated);
            gvOrders.ItemDataBound += new GridItemEventHandler(gvOrders_ItemDataBound);
            gvOrders.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(gvOrders_NeedDataSource);
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

        public List<int> GetForSelectionStatus(bool selected)
        {
            List<int> retVal = new List<int>();
            foreach (GridDataItem row in gvOrders.Items)
            {
                if (row.Selected == selected)
                {
                    int orderID = int.Parse(row.OwnerTableView.DataKeyValues[row.ItemIndex]["OrderID"].ToString());
                    retVal.Add(orderID);
                }
            }
            return retVal;
        }

        #region Event Handlers

        #region Grid Events

        void gvOrders_ItemCreated(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is Telerik.Web.UI.GridDataItem)
            {
                Telerik.Web.UI.GridDataItem gridItem = e.Item as Telerik.Web.UI.GridDataItem;
                foreach (Telerik.Web.UI.GridColumn column in gvOrders.Columns)
                    if (column is Telerik.Web.UI.GridBoundColumn)
                        gridItem[column.UniqueName].ToolTip = gridItem[column.UniqueName].Text;
            }
        }

        void gvOrders_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                DataRowView drv = e.Item.DataItem as DataRowView;
                e.Item.Selected = true;
                // This is a Patch until SP2 of the Telerik Grid
                CheckBox checkboxSelectColumn = ((e.Item as GridDataItem)["checkboxSelectColumn"]).Controls[1] as CheckBox;
                checkboxSelectColumn.Checked = true;
                checkboxSelectColumn.Enabled = true; // Default to selected and deselectable.
                // The user can not alter the selection state of this item if this is a delivery instruction,
                // or the collection run delivery point does not equal this jobs terminating instruction point.
                // This condition indicates that a further job has been created to take that goods on.
                Facade.IRedelivery facRedelivery = new Facade.Redelivery();
                if (Instruction.InstructionTypeId == (int)Orchestrator.eInstructionType.Drop)
                    checkboxSelectColumn.Enabled = false;
                else if (facRedelivery.GetForJobIDAndOrderID(Instruction.JobId, (int)drv["OrderID"]).Count > 0)
                    checkboxSelectColumn.Enabled = false;
                else
                {
                    if ((int)drv["CollectionRunDeliveryPointID"] != (int)drv["DeliveryPointID"]
                        && (string)drv["OrderAction"] != Orchestrator.eOrderAction.Default.ToString())
                        checkboxSelectColumn.Enabled = false;
                }
            }
            else if (e.Item.ItemType == GridItemType.Header)
            {
                // Don't allow select/deselect all for this grid.
                (((e.Item as GridHeaderItem)["checkboxSelectColumn"]).Controls[1] as CheckBox).Visible = false;
            }
        }

        void gvOrders_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            if (Instruction != null && Instruction.InstructionID > 0)
            {
                Orchestrator.Facade.IOrder facOrder = new Orchestrator.Facade.Order();
                gvOrders.DataSource = facOrder.GetOrdersForInstructionID(Instruction.InstructionID);

                gvOrders.Columns[C_CollectFromColumnIndex].Visible = ShowCollectionColumns;
                gvOrders.Columns[C_CollectAtColumnIndex].Visible = ShowCollectionColumns;
            }
        }

        #endregion

        #endregion
    }
}