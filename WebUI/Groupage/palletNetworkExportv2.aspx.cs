using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using Telerik.Web.UI;
using System.Globalization;

namespace Orchestrator.WebUI.Groupage
{
    public partial class palletNetworkExportv2 : Orchestrator.Base.BasePage
    {
        //-----------------------------------------------------------------------------------------------------------

        int orderCount = 0;
        double totalWeight = 0;
        double totalPalletCount = 0;
        double totalPalletSpaces = 0;
        double QtrPalletsTotal = 0;
        double HalfPalletsTotal = 0;
        double FullPalletsTotal = 0;
        double OverPalletsTotal = 0;
        double OrderRateTotal = 0;
        double SubContractCostTotal = 0;
        double HubChargeTotal = 0;

        //-----------------------------------------------------------------------------------------------------------

        protected void Page_Load(object sender, EventArgs e)
        {
            
        }
        
        //-----------------------------------------------------------------------------------------------------------
        
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            
            this.grdOrders.NeedDataSource +=new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdOrders_NeedDataSource);
            this.grdOrders.ItemCreated += new Telerik.Web.UI.GridItemEventHandler(grdOrders_ItemCreated);
            this.grdOrders.ItemDataBound += new GridItemEventHandler(grdOrders_ItemDataBound);
            this.grdOrders.UpdateCommand += new GridCommandEventHandler(grdOrders_UpdateCommand);
            this.btnSearch.Click += new EventHandler(btnSearch_Click);
            this.btnExport.Click += new EventHandler(btnExport_Click);
            this.rboFilterOption.SelectedIndexChanged += (o, eV) => { this.grdOrders.Rebind(); };

            this.dteTrunkDate.SelectedDate = DateTime.Today;
        }

        //-----------------------------------------------------------------------------------------------------------

      

        //-----------------------------------------------------------------------------------------------------------

        protected void grdOrders_UpdateCommand(object source, GridCommandEventArgs e)
        {
            GridEditableItem editedItem = e.Item as GridEditableItem;
            int orderId = 0;

            if (!String.IsNullOrEmpty(editedItem.GetDataKeyValue("OrderId").ToString()))
                orderId = Convert.ToInt32(editedItem.GetDataKeyValue("OrderId").ToString());

            if (orderId > 0)
            {
                RadNumericTextBox txtNoPallets = (RadNumericTextBox)e.Item.FindControl("txtNoPallets");
                RadNumericTextBox txtPalletSpaces = (RadNumericTextBox)e.Item.FindControl("txtPalletSpaces");
                RadNumericTextBox txtWeight = (RadNumericTextBox)e.Item.FindControl("txtWeight");
                RadNumericTextBox txtQtrPallets = (RadNumericTextBox)e.Item.FindControl("txtQtrPallets");
                RadNumericTextBox txtHalfPallets = (RadNumericTextBox)e.Item.FindControl("txtHalfPallets");
                RadNumericTextBox txtFullPallets = (RadNumericTextBox)e.Item.FindControl("txtFullPallets");
                RadNumericTextBox txtOverPallets = (RadNumericTextBox)e.Item.FindControl("txtOverPallets");

                Orchestrator.EF.DataContext context = new Orchestrator.EF.DataContext();
                Orchestrator.EF.VigoOrder vigoOrder = context.VigoOrderSet.FirstOrDefault(vo => vo.OrderId == orderId);

                vigoOrder.QtrPallets = Convert.ToInt32(txtQtrPallets.Value);
                vigoOrder.HalfPallets = Convert.ToInt32(txtHalfPallets.Value);
                vigoOrder.FullPallets = Convert.ToInt32(txtFullPallets.Value);
                vigoOrder.OverPallets = Convert.ToInt32(txtOverPallets.Value);

                vigoOrder.LastUpdateDate = DateTime.Now;
                vigoOrder.LastUpdateUserId = Page.User.Identity.Name;

                context.SaveChanges(true);

                //Orchestrator.EF.RateInformation rateInformation = null;

                Facade.IOrder facOrder = new Facade.Order();
                Orchestrator.Entities.Order order = facOrder.GetForOrderID(orderId);

                order.NoPallets = Convert.ToInt32(txtNoPallets.Value);
                order.PalletSpaces = Convert.ToDecimal(txtPalletSpaces.Value);
                order.Weight = Convert.ToDecimal(txtWeight.Value);

                order.LastUpdatedDateTime = DateTime.Now;
                order.LastUpdatedBy = Page.User.Identity.Name;

               
                facOrder.Update(order, Page.User.Identity.Name);

                // We need to make sure the subcontractor (Palletforce) costs are re-rated
                Facade.IJobSubContractor jobSubContractor = new Facade.Job();
                if (order != null && order.JobSubContractID > 0)
                {
                    Entities.JobSubContractor js = jobSubContractor.GetSubContractorForJobSubContractId(order.JobSubContractID);
                    jobSubContractor.UpdateSubContractorCostsForOrders(new List<int>() { order.OrderID }, js, this.Page.User.Identity.Name);
                }


                CheckBoxList cblSurcharge = e.Item.FindControl("cblSurcharges") as CheckBoxList;
                List<EF.ExtraType> extraTypes = context.ExtraTypeSet.Select(et => et).ToList();

                List<int> selectedExtraTypeIds = new List<int>();
                foreach (ListItem item in cblSurcharge.Items)
                    if (item.Selected) selectedExtraTypeIds.Add(int.Parse(item.Value));

                //Remove the selected extras which are already present
                foreach (var extra in vigoOrder.VigoOrderExtras)
                    selectedExtraTypeIds.Remove(extra.ExtraType.ExtraTypeId);

                //Now add Extras for the selected extras remaining
                foreach (int extraTypeId in selectedExtraTypeIds)
                {
                    var vigoOrderExtra = new EF.VigoOrderExtra();
                    vigoOrderExtra.ExtraType = extraTypes.First(et => et.ExtraTypeId == extraTypeId); //new EntityKey("DataContext.ExtraTypeSet", "ExtraTypeId", extraTypeId);
                    vigoOrder.VigoOrderExtras.Add(vigoOrderExtra);
                }


                context.SaveChanges();

                //this.grdOrders.DataSource = null;
                //this.grdOrders.Rebind();
            }
        }

        //-----------------------------------------------------------------------------------------------------------

        protected void grdOrders_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                DataRowView row = (DataRowView)e.Item.DataItem;
                int orderId = int.Parse(row["OrderID"].ToString());
                string runId = row["JobId"].ToString();

                using (CheckBox chk = (CheckBox)e.Item.FindControl("chkSelectOrder"))
                {
                    chk.Attributes.Add("onClick",string.Format("javascript:HandleSelection(this, {0});",
                                 e.Item.ItemIndex));

                    chk.Attributes.Add("OrderId", orderId.ToString());
                }

                orderCount++;
                totalPalletCount += (int)row["NoPallets"];
                totalPalletSpaces += Convert.ToDouble(row["PalletSpaces"]);
                totalWeight += Convert.ToDouble(row["Weight"]);
                QtrPalletsTotal += Convert.ToDouble(row["QtrPallets"]);
                HalfPalletsTotal += Convert.ToDouble(row["HalfPallets"]);
                FullPalletsTotal += Convert.ToDouble(row["FullPallets"]);
                OverPalletsTotal += Convert.ToDouble(row["OverPallets"]);
                OrderRateTotal += (row["ForeignRate"] != DBNull.Value) ? Convert.ToDouble(row["ForeignRate"]) : 0.00;
                SubContractCostTotal += (row["SubContractRate"] != DBNull.Value) ? Convert.ToDouble(row["SubContractRate"]) : 0.00;
                HubChargeTotal += (row["SubContractRate"] != DBNull.Value) ? Convert.ToDouble(row["HubCharge"]) : 0.00;

                // Delivery At column
                Label lblDeliverAt = (Label)e.Item.FindControl("lblDeliverAt");
                DateTime delFrom = Convert.ToDateTime(row["DeliveryFromDateTime"].ToString());
                DateTime delBy = Convert.ToDateTime(row["DeliveryDateTime"].ToString());

                if (lblDeliverAt != null)
                {
                    if (delFrom == delBy)
                    {
                        // Timed booking... only show a single date.
                        lblDeliverAt.Text = GetDate(Convert.ToDateTime(row["DeliveryDateTime"].ToString()), false);
                    }
                    else
                    {
                        // If the times span from mignight to 23:59 on the same day then 
                        // it's an 'anytime' window.
                        if (delFrom.Date == delBy.Date && delFrom.Hour == 0 && delFrom.Minute == 0 && delBy.Hour == 23 && delBy.Minute == 59)
                        {
                            // It's anytime
                            lblDeliverAt.Text = GetDate(Convert.ToDateTime(row["DeliveryDateTime"].ToString()), true);
                        }
                        else
                        {
                            // It's a booking window
                            lblDeliverAt.Text = GetDate(Convert.ToDateTime(row["DeliveryFromDateTime"].ToString()), false) + " to " + GetDate(Convert.ToDateTime(row["DeliveryDateTime"].ToString()), false);
                        }
                    }
                }
  
                //if (row["IsTariffOverride"] != DBNull.Value && Convert.ToBoolean(row["IsTariffOverride"]))
                //    e.Item.Cells[grdOrders.Columns.FindByUniqueName("Rate").OrderIndex].BackColor = System.Drawing.Color.Yellow;

                if (row["MessageStateId"] != DBNull.Value)
                {
                    eMessageState messageState = (eMessageState)row["MessageStateId"];
                    switch (messageState)
                    {
                        case eMessageState.Unprocessed:
                            e.Item.BackColor = System.Drawing.Color.Pink;
                            break;

                        case eMessageState.Processed:
                        case eMessageState.NotProcessed:
                            e.Item.BackColor = System.Drawing.Color.Violet;
                            break;

                        case eMessageState.Error:
                            e.Item.BackColor = System.Drawing.Color.Red;
                            break;
                    }
                }

                CheckBoxList cblsurcharges = e.Item.FindControl("cblSurcharges") as CheckBoxList;

                EF.VigoOrder vigoOrder = EF.DataContext.Current.VigoOrderSet.Include("VigoOrderExtras.ExtraType").FirstOrDefault(v => v.OrderId == orderId);

                Orchestrator.Facade.ExtraType facExtraType = new Orchestrator.Facade.ExtraType();
                List<Entities.ExtraType> extraTypes = facExtraType.GetForIsDisplayedOnAddUpdateOrder();
                cblsurcharges.DataSource = extraTypes.OrderBy(et => et.ShortDescription);
                cblsurcharges.DataTextField = "ShortDescription";
                cblsurcharges.DataValueField = "ExtraTypeId";
                cblsurcharges.DataBind();

                //Tick the PalletForce Surcharge checkboxes for each VigoOrderExtra
                foreach (var extra in vigoOrder.VigoOrderExtras)
                {
                    ListItem item = cblsurcharges.Items.FindByValue(extra.ExtraType.ExtraTypeId.ToString());
                    if (item != null)
                        item.Selected = true;
                }
            }
            else if (e.Item is GridEditableItem)
            {
                DataRowView row = (DataRowView)e.Item.DataItem;

                RadNumericTextBox txtRate = e.Item.FindControl("txtRate") as RadNumericTextBox;
                if (txtRate != null)
                {
                    CultureInfo culture = new CultureInfo(int.Parse(row["LCID"].ToString()));
                    txtRate.Culture = culture;

                    //if (row["TariffRateId"] == null)
                    //{
                    //    txtRate.BackColor = System.Drawing.Color.Yellow;
                    //}
                }
            }
        }

        //-----------------------------------------------------------------------------------------------------------

        protected void btnExport_Click(object sender, EventArgs e)
        {
            Facade.ExportOrder facExportOrder = new Facade.ExportOrder();
            List<int> orderIds = new List<int>();
            List<int> exportedOrderIds = new List<int>();
            bool noPalletValues = false;

            // Update the exported rows by changing their colours
            foreach (GridDataItem item in grdOrders.Items)
            {
                CheckBox chkOrderId = (CheckBox)item.FindControl("chkSelectOrder");
                int orderId;
                int.TryParse(chkOrderId.Attributes["OrderID"].ToString(), out orderId);

                if (chkOrderId.Checked)
                {
                    EF.Order order = EF.DataContext.Current.OrderSet.Include("VigoOrder").FirstOrDefault(o => o.OrderId == orderId);
                    noPalletValues = (order.VigoOrder.QtrPallets == 0
                                        && order.VigoOrder.HalfPallets == 0
                                        && order.VigoOrder.FullPallets == 0
                                        && order.VigoOrder.OverPallets == 0)
                                     || order.Weight == 0;

                    if (noPalletValues)
                        item.BackColor = System.Drawing.Color.LightBlue;

                    orderIds.Add(orderId);
                }
            }

            if (!noPalletValues)
            {
                exportedOrderIds = facExportOrder.Create(orderIds, this.Page.User.Identity.Name);

                this.grdOrders.DataSource = null;
                this.grdOrders.Rebind();
            }
        }

        //-----------------------------------------------------------------------------------------------------------

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            this.grdOrders.DataSource = null;
            this.grdOrders.Rebind();
        }

        //-----------------------------------------------------------------------------------------------------------

        protected string GetDate(DateTime date, bool anytime)
        {
            string retVal = string.Empty;

            if (anytime)
                retVal = date.ToString("dd/MM/yy") + " AnyTime";
            else
                retVal = date.ToString("dd/MM/yy HH:mm");

            return retVal;
        }

        //-----------------------------------------------------------------------------------------------------------

        protected void grdOrders_ItemCreated(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is GridFooterItem)
            {
                ((Label)e.Item.FindControl("lblPalletsTotal")).Text = String.Format("NoPallets: {0}", totalPalletCount.ToString());
                ((Label)e.Item.FindControl("lblSpacesTotal")).Text = String.Format("Spaces: {0}", totalPalletSpaces.ToString("F2"));
                ((Label)e.Item.FindControl("lblWeightTotal")).Text = String.Format("Kgs: {0}", totalWeight.ToString());
                ((Label)e.Item.FindControl("lblQtrPalletsTotal")).Text = String.Format("1/4: {0}",QtrPalletsTotal.ToString());
                ((Label)e.Item.FindControl("lblHalfPalletsTotal")).Text = String.Format("1/2: {0}",HalfPalletsTotal.ToString());
                ((Label)e.Item.FindControl("lblFullPalletsTotal")).Text = String.Format("Full: {0}",FullPalletsTotal.ToString());
                ((Label)e.Item.FindControl("lblOverPalletsTotal")).Text = String.Format("O/S: {0}",OverPalletsTotal.ToString());

            }
        }

        //-----------------------------------------------------------------------------------------------------------

        protected void grdOrders_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            // Search for orders based on Date Range Status and text
            // Determine the parameters
            List<int> orderStatusIDs = new List<int>();
            orderStatusIDs.Add((int)eOrderStatus.Approved);
            orderStatusIDs.Add((int)eOrderStatus.Delivered);
            orderStatusIDs.Add((int)eOrderStatus.Invoiced);

            // Set the Business Types
            List<int> BusinessTypes = new List<int>();
            Facade.IBusinessType facBusinessType = new Facade.BusinessType();
            DataSet dsBusinessTypes = facBusinessType.GetAll();

            foreach (DataRow row in dsBusinessTypes.Tables[0].Rows)
                BusinessTypes.Add(int.Parse(row["BusinessTypeID"].ToString()));
            // Retrieve the client id, resource id, and sub-contractor identity id.
            int clientID = 0;
            int resourceID = 0;
            int subContractorIdentityID = Globals.Configuration.PalletNetworkID;

            int collectionPointId = 0;
            int deliveryPointId = 0;

            int goodsType = 0;
        
            // Find the orders.
            Facade.IOrder facOrder = new Facade.Order();
            DataSet orderData = null;
            try
            {
                orderData =
                    facOrder.Search(orderStatusIDs, this.dteTrunkDate.SelectedDate, this.dteTrunkDate.SelectedDate.Value.AddDays(1), String.Empty,false,false,true,false,
                                    clientID, resourceID, subContractorIdentityID, BusinessTypes, collectionPointId, deliveryPointId, goodsType);
            }
            catch (SqlException exc)
            {
                throw;
            }

            orderCount = 0;
            totalPalletCount = 0;
            totalPalletSpaces = 0;
            totalWeight = 0;
            QtrPalletsTotal = 0;
            HalfPalletsTotal = 0;
            FullPalletsTotal = 0;
            OverPalletsTotal = 0;
            OrderRateTotal = 0;
            SubContractCostTotal = 0;
            HubChargeTotal = 0;

            if (rboFilterOption.SelectedValue == "1")
            {
                grdOrders.DataSource = orderData;
            }
            else if (rboFilterOption.SelectedValue == "2")
            {
                DataView dv = orderData.Tables[0].DefaultView;
                dv.RowFilter = "MessageStateId IS NOT NULL";
                grdOrders.DataSource = dv;
            }
            else
            {
                DataView dv = orderData.Tables[0].DefaultView;
                dv.RowFilter = "MessageStateId IS NULL";
                grdOrders.DataSource = dv;
                
                
            }
        }

        //-----------------------------------------------------------------------------------------------------------

    }

    //-----------------------------------------------------------------------------------------------------------

}
