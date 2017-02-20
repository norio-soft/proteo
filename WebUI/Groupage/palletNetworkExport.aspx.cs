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
    public partial class palletNetworkExport : Orchestrator.Base.BasePage
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

            this.dteTrunkDate.SelectedDate = DateTime.Today;
        }

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

                //// Rate has not changed so auto rate it
                //if (order.ForeignRate == Convert.ToDecimal(txtRate.Value.Value))
                //{
                //    IEnumerable<Orchestrator.EF.RateSurcharge> rateSurcharge = new List<Orchestrator.EF.RateSurcharge>();
                //    rateInformation = facOrder.GetRate(order, true, out rateSurcharge);

                //    if (rateInformation != null)
                //    {
                //        order.TariffRateID = rateInformation.TariffRateId;
                //        order.ForeignRate = rateInformation.ForeignRate;
                //        order.Rate = rateInformation.Rate;
                //    }
                //    else
                //    {
                //        Facade.IExchangeRates facER = new Facade.ExchangeRates();
                //        CultureInfo nativeCulture = new CultureInfo(Orchestrator.Globals.Configuration.NativeCulture);

                //        order.ForeignRate = Convert.ToDecimal(txtRate.Value);

                //        if (order.LCID != nativeCulture.LCID)
                //        {
                //            order.ExchangeRateID = facER.GetCurrentExchangeRateID(Facade.Culture.GetCurrencySymbol(order.LCID), order.CollectionDateTime);
                //            order.Rate = facER.GetConvertedRate((int)order.ExchangeRateID, order.ForeignRate);
                //        }
                //        else
                //            order.Rate = order.ForeignRate;

                //        order.IsTariffOverride = true;
                //        order.TariffRateID = null;
                //        order.TariffOverrideUserID = Page.User.Identity.Name;
                //        order.TariffOverrideDate = DateTime.Now;
                //    }
                //}
                //else
                //{
                //    Facade.IExchangeRates facER = new Facade.ExchangeRates();
                //    CultureInfo nativeCulture = new CultureInfo(Orchestrator.Globals.Configuration.NativeCulture);

                //    order.ForeignRate = Convert.ToDecimal(txtRate.Value);

                //    if (order.LCID != nativeCulture.LCID)
                //    {
                //        order.ExchangeRateID = facER.GetCurrentExchangeRateID(Facade.Culture.GetCurrencySymbol(order.LCID), order.CollectionDateTime);
                //        order.Rate = facER.GetConvertedRate((int)order.ExchangeRateID, order.ForeignRate);
                //    }
                //    else
                //        order.Rate = order.ForeignRate;

                //    order.IsTariffOverride = true;
                //    order.TariffRateID = null;
                //    order.TariffOverrideUserID = Page.User.Identity.Name;
                //    order.TariffOverrideDate = DateTime.Now;
                //}

                facOrder.Update(order, Page.User.Identity.Name);

                // We need to make sure the subcontractor (Palletforce) costs are re-rated
                Facade.IJobSubContractor jobSubContractor = new Facade.Job();
                if (order != null && order.JobSubContractID > 0)
                {
                    Entities.JobSubContractor js = jobSubContractor.GetSubContractorForJobSubContractId(order.JobSubContractID);
                    jobSubContractor.UpdateSubContractorCostsForOrders(new List<int>() { order.OrderID }, js, this.Page.User.Identity.Name);
                }

                this.grdOrders.DataSource = null;
                this.grdOrders.Rebind();
            }
        }

        //-----------------------------------------------------------------------------------------------------------

        protected void grdOrders_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                DataRowView row = (DataRowView)e.Item.DataItem;
                string orderId = row["OrderID"].ToString();
                string runId = row["JobId"].ToString();

                using (CheckBox chk = (CheckBox)e.Item.FindControl("chkSelectOrder"))
                {
                    chk.Attributes.Add("onClick",string.Format("javascript:HandleSelection(this, {0});",
                                 e.Item.ItemIndex));

                    chk.Attributes.Add("OrderId", orderId);
                }
    
                HyperLink hypUpdateOrder = e.Item.FindControl("hypUpdateOrder") as HyperLink;
                hypUpdateOrder.NavigateUrl = string.Format("javascript:ViewOrderProfile({0});", orderId);
                hypUpdateOrder.Text = orderId;

                HyperLink hypRun = e.Item.FindControl("hypRun") as HyperLink;
                hypRun.NavigateUrl = string.Format("javascript:ViewRun({0});", runId);
                hypRun.Text = runId;

                CultureInfo CurrentCulture = new CultureInfo(int.Parse(row["LCID"].ToString()));

                Label lblRate = e.Item.FindControl("lblRate") as Label;
                if (lblRate != null && row["ForeignRate"] != DBNull.Value)
                    lblRate.Text = ((decimal)row["ForeignRate"]).ToString("C", CurrentCulture);

                Label lblSubContractRate = e.Item.FindControl("lblSubContractCost") as Label;
                Label lblHubCharge = e.Item.FindControl("lblHubCharge") as Label;

                if (row["SubContractLCID"] != DBNull.Value)
                {
                    CultureInfo subContractCulture = new CultureInfo(int.Parse(row["SubContractLCID"].ToString()));

                    if (lblSubContractRate != null && row["SubContractRate"] != DBNull.Value)
                        lblSubContractRate.Text = ((decimal)row["SubContractRate"]).ToString("C", subContractCulture);

                    if (lblHubCharge != null && row["HubCharge"] != DBNull.Value)
                        lblHubCharge.Text = ((decimal)row["HubCharge"]).ToString("C", subContractCulture);
                }
                else
                {
                    lblSubContractRate.Text = decimal.Zero.ToString("C", CurrentCulture);
                    lblHubCharge.Text = decimal.Zero.ToString("C", CurrentCulture);
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

            // Update the exported rows by changing their colours
            foreach (GridDataItem item in grdOrders.Items)
            {
                CheckBox chkOrderId = (CheckBox)item.FindControl("chkSelectOrder");
                int orderId;
                int.TryParse(chkOrderId.Attributes["OrderID"].ToString(), out orderId);

                if(chkOrderId.Checked)
                    orderIds.Add(orderId);
            }

            exportedOrderIds = facExportOrder.Create(orderIds, this.Page.User.Identity.Name);

            this.grdOrders.DataSource = null;
            this.grdOrders.Rebind();
        }

        //-----------------------------------------------------------------------------------------------------------

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            this.grdOrders.DataSource = null;
            this.grdOrders.Rebind();
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

                ((Label)e.Item.FindControl("lblRateTotal")).Text = String.Format("Rate: {0}", OrderRateTotal.ToString("F2"));
                ((Label)e.Item.FindControl("lblSubContractCostTotal")).Text = String.Format("Sub Contract Cost: {0}", SubContractCostTotal.ToString("F2"));
                ((Label)e.Item.FindControl("lblHubChargeTotal")).Text = String.Format("Hub Charge: {0}", HubChargeTotal.ToString("F2"));
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

            // sort out the rate to display, when none of the orders have a rate and only the order group has a rate.
            // we only want to display the rate for one order in the group.
            List<int> orderGroupIds = new List<int>();

            if (orderData != null)
            {

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

                foreach (DataRow row in orderData.Tables[0].Rows)
                {
                    if (row["OrderGroupId"] != System.DBNull.Value && Convert.ToDouble(row["ForeignRate"].ToString()) > 0.00)
                    {
                        if (orderGroupIds.Contains(int.Parse(row["OrderGroupId"].ToString())))
                            row["ForeignRate"] = 0.00;
                        else
                        {
                            row["ForeignRate"] = row["GroupRate"];
                            orderGroupIds.Add(int.Parse(row["OrderGroupId"].ToString()));
                        }
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
                }
            }

            grdOrders.DataSource = orderData;
        }

        //-----------------------------------------------------------------------------------------------------------

    }

    //-----------------------------------------------------------------------------------------------------------

}
