using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;

using Orchestrator.Entities;
using Telerik.Web.UI;
using System.Web.UI.HtmlControls;

namespace Orchestrator.WebUI.Groupage
{
    public partial class OrderGroupProfile : Orchestrator.Base.BasePage
    {
        #region Fields

        // Query-string elements
        private const string _addorder_QS = "addOrder";
        private const string _orderGroupID_QS = "ogid";

        // Viewstate elements
        private const string _orderGroupID_VS = "ogid";

        // Javascript templates
        private const string _openWindow_JS = "openWindow('{0}', '{1}', {2}, {3});";

        private Entities.OrderGroup _orderGroup = null;
        protected bool _isBeingInvoiced = true;

        private int _decimalPlaces = 2;

        #endregion

        #region Properties

        protected bool IsBeingInvoiced
        {
            get { return _isBeingInvoiced; }
            set { _isBeingInvoiced = value; }
        }

        protected OrderGroup OrderGroup
        {
            get { return _orderGroup; }
            set { _orderGroup = value; }
        }

        private int OrderGroupID
        {
            get
            {
                if (OrderGroup == null)
                {
                    if (ViewState[_orderGroupID_QS] == null)
                    {
                        return 0;
                    }
                    else
                    {
                        return (int)ViewState[_orderGroupID_QS];
                    }
                }
                else
                {
                    return OrderGroup.OrderGroupID;
                }
            }
            set { ViewState[_orderGroupID_VS] = value; }
        }

        #endregion

        #region Page Load/Init

        protected void Page_Load(object sender, EventArgs e)
        {
            if (LoadOrderGroup())
            {
                //ConfigureDisplay();

                if (!IsPostBack)
                {
                    ConfigureDisplay();

                    // Detect presence of auto-add order and add the javascript to pop the add order window up (if presence detected).
                    if ((!string.IsNullOrEmpty(Request.QueryString[_addorder_QS]) && Request.QueryString[_addorder_QS].ToLower() == "true"))
                        LaunchAddOrderWindow();
                }
                else
                {
                    Facade.IOrder facOrder = new Facade.Order();
                    this.IsBeingInvoiced = facOrder.IsOrderBeingInvoiced(OrderGroup.OrderIDs());
                }
            }
            else
            {
                // Invalid order group specified.
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            dlgOrder.DialogCallBack += new EventHandler(dlgOrder_DialogCallBack);
            dlgAssociateOrder.DialogCallBack += new EventHandler(dlgAssociateOrder_DialogCallBack);

            cboGroupedPlanning.SelectedIndexChanged += new EventHandler(cboGroupedPlanning_SelectedIndexChanged);
            cboAllocatedTo.SelectedIndexChanged += new RadComboBoxSelectedIndexChangedEventHandler(cboAllocatedTo_SelectedIndexChanged);

            grdOrders.ItemCommand += new Telerik.Web.UI.GridCommandEventHandler(grdOrders_ItemCommand);
            grdOrders.ItemDataBound += new GridItemEventHandler(grdOrders_ItemDataBound);
            grdOrders.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdOrders_NeedDataSource);

            btnCreateJob.Click += new EventHandler(btnCreateJob_Click);
            btnProducePILs.Click += new EventHandler(btnProducePILs_Click);
            btnRemoveAll.Click += new EventHandler(btnRemoveAll_Click);
            btnUpdateOrders.Click += new EventHandler(btnUpdateOrders_Click);

            GridColumn customerOrderNumber = this.grdOrders.MasterTableView.Columns.FindByUniqueName("CustomerOrderNumber");
            if (customerOrderNumber != null)
                customerOrderNumber.HeaderText = Globals.Configuration.SystemLoadNumberText;

            GridColumn deliveryrOrderNumber = this.grdOrders.MasterTableView.Columns.FindByUniqueName("DeliveryOrderNumber");
            if (deliveryrOrderNumber != null)
                deliveryrOrderNumber.HeaderText = Globals.Configuration.SystemDocketNumberText;
        }

        private void CheckRateInformation(Entities.Order order, ref bool orderInGroupOverridden, ref bool updateOrder)
        {
            Facade.IOrder facOrder = new Facade.Order();
            IEnumerable<Repositories.DTOs.RateSurcharge> surcharges = null;
            Repositories.DTOs.RateInformation correctRateInformation = null;

            try
            {
                Entities.VigoOrder vigoOrder = null;
                Facade.IVigoOrder facVigoOrder = new Facade.Order();
                vigoOrder = facVigoOrder.GetForOrderId(order.OrderID);

                if (vigoOrder != null)
                    correctRateInformation = facOrder.GetRate(order, false, false, out surcharges, vigoOrder);
                else
                    correctRateInformation = facOrder.GetRate(order, false, false, out surcharges);
            }
            catch (ApplicationException aex)
            {
                if (!aex.Message.StartsWith("Postcode"))
                    throw;
            }

            if (correctRateInformation != null)
            {
                // A rate was detected
                if (order.IsAutorated == false)
                {
                    updateOrder = true;

                    if (order.ForeignRate == correctRateInformation.ForeignRate)
                    {
                        order.IsAutorated = true;
                    }
                    else
                    {
                        orderInGroupOverridden = true;
                        order.IsAutorated = true;
                        order.TariffOverrideDate = DateTime.Now;
                        order.TariffOverrideUserID = this.Page.User.Identity.Name;
                        order.IsTariffOverride = true;
                    }
                }
            }
        }

        void btnUpdateOrders_Click(object sender, EventArgs e)
        {
            Facade.IOrder facOrder = new Facade.Order();

            decimal rateTotal = 0m;

            bool orderInGroupOverridden = false;
            bool updateOrder = false;
            foreach (Entities.Order order in this.OrderGroup.Orders)
            {
                updateOrder = false;

                /*
                * Tom Farrow 24/03/2010
                * 
                * ---------------------------------------- 
                * CHECK FOR THE EXISTENCE OF A TARIFF RATE
                * ---------------------------------------- 
                * This check is required in the scenario where tariff tables are created
                * after orders are created. When orders are initally created without
                * a tariff table present, they cannot be auto-rated and cannot be marked as rate
                * overridden. If a tariff table is subsequently created with rates that would
                * apply to those existing orders, those orders would be in an inconsistent state.
                * 
                * Thus, upon opening these orders a check needs to be made to see if a tariff-rate exists.
                * If a rate exists and the order has a DIFFERENT rate, then the order should be
                * marked as rate overridden. If a rate exists but it is the same as the orders 
                * rate, then the order should be marked as auto-rated.
                                         
                */

                /* START OF CHECK */

                CheckRateInformation(order, ref orderInGroupOverridden, ref updateOrder);

                /* END OF CHECK */

                GridDataItem gdi = this.grdOrders.MasterTableView.FindItemByKeyValue("OrderID", order.OrderID);
                RadNumericTextBox rntForeignRate = gdi.FindControl("rntForeignRate") as RadNumericTextBox;
                if (Convert.ToDecimal(rntForeignRate.Value) != order.ForeignRate)
                {
                    // Mark as overridden... update the rate on the order.
                    decimal newRate = Convert.ToDecimal(rntForeignRate.Value);
                    order.ForeignRate = newRate;
                    if (!order.IsTariffOverride)
                    {
                        order.IsTariffOverride = true;
                        order.TariffOverrideDate = DateTime.Now;
                        order.TariffOverrideUserID = User.Identity.Name;
                        this.txtRateTariffCard.Text = "Overridden";
                    }

                    // Recalculate the GBP amounts
                    BusinessLogicLayer.IExchangeRates blER = new BusinessLogicLayer.ExchangeRates();
                    BusinessLogicLayer.CurrencyConverter currencyConverter = blER.CreateCurrencyConverter(order.LCID, order.CollectionDateTime);
                    order.Rate = currencyConverter.ConvertToLocal(order.ForeignRate);
                    rateTotal += newRate;

                    updateOrder = true;
                }
                else
                {
                    rateTotal += order.ForeignRate;
                }

                if (!String.IsNullOrEmpty(this.txtLoadNumber.Text))
                {
                    updateOrder = true;
                    order.CustomerOrderNumber = this.txtLoadNumber.Text;
                }

                // Update the order as the CheckRateInformation method may have amended the order
                if (updateOrder)
                    facOrder.Update(order, User.Identity.Name);

            }

            if (orderInGroupOverridden)
            {
                this.txtRateTariffCard.Text = this.OrderGroup.TariffRateDescription = "Overridden";
                Facade.IOrderGroup facOrderGroup = new Facade.Order();
                facOrderGroup.Update(this.OrderGroup, this.Page.User.Identity.Name);
            }

            RebindPage();
            //CultureInfo culture = new CultureInfo(this.OrderGroup.LCID);
            //if (_decimalPlaces != 2)
            //    culture.NumberFormat.CurrencyDecimalDigits = _decimalPlaces;
            //lblRate.Text = rateTotal.ToString("C", culture);

            this.ReturnValue = "refresh";
        }

        void dlgOrder_DialogCallBack(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(this.dlgOrder.ReturnValue))
                if (this.OrderGroup.GroupedPlanning)
                {
                    IList<string> jobIds = new List<string>();
                    Facade.IOrder facOrder = new Facade.Order();
                    foreach (Entities.Order order in OrderGroup.Orders)
                    {
                        DataSet dsLegsForOrder = facOrder.GetLegsForOrder(order.OrderID);
                        if (dsLegsForOrder.Tables[0].Rows.Count > 0)
                            foreach (DataRow row in dsLegsForOrder.Tables[0].Rows)
                                if (!jobIds.Contains(row["JobId"].ToString()))
                                    jobIds.Add(row["JobId"].ToString());
                    }

                    if (jobIds.Count == 1)
                    {
                        int parsedOrderId = int.Parse(this.dlgOrder.ReturnValue);
                        int parsedJobId = int.Parse(jobIds[0]);

                        // Check to see if the order is already on the run.
                        // If it is then don't add it again.
                        var orderOnRun = (
                            (
                                from i in EF.DataContext.Current.InstructionSet
                                from cd in i.CollectDrops
                                where cd.Order.OrderId == parsedOrderId
                                    && i.Job.JobId == parsedJobId
                                select i.InstructionId
                            )
                            .Count() > 0);

                        if (!orderOnRun)
                            this.AddOrderToRun(parsedOrderId, parsedJobId);
                    }
                }

            RebindPage();

        }

        void dlgAssociateOrder_DialogCallBack(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(this.dlgAssociateOrder.ReturnValue))
                if (this.OrderGroup.GroupedPlanning)
                {
                    IList<string> jobIds = new List<string>();
                    Facade.IOrder facOrder = new Facade.Order();
                    foreach (Entities.Order order in OrderGroup.Orders)
                    {
                        DataSet dsLegsForOrder = facOrder.GetLegsForOrder(order.OrderID);
                        if (dsLegsForOrder.Tables[0].Rows.Count > 0)
                            foreach (DataRow row in dsLegsForOrder.Tables[0].Rows)
                                if (!jobIds.Contains(row["JobId"].ToString()))
                                    jobIds.Add(row["JobId"].ToString());
                    }

                    if (jobIds.Count == 1)
                    {
                        string[] orderIds = this.dlgAssociateOrder.ReturnValue.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string orderId in orderIds)
                        {
                            if (!String.IsNullOrEmpty(orderId))
                            {
                                int parsedOrderId = int.Parse(orderId);
                                int parsedJobId = int.Parse(jobIds[0]);

                                // Check to see if the order is already on the run.
                                // If it is then don't add it again.
                                var orderOnRun = (
                                    (
                                        from i in EF.DataContext.Current.InstructionSet
                                        from cd in i.CollectDrops
                                        where cd.Order.OrderId == parsedOrderId
                                            && i.Job.JobId == parsedJobId
                                        select i.InstructionId
                                    )
                                    .Count() > 0);


                                // Check to see if the order is already on the run.
                                // If it is then don't add it again.
                                var orderAlreadyOnRun = (
                                    (
                                        from i in EF.DataContext.Current.InstructionSet
                                        from cd in i.CollectDrops
                                        where cd.Order.OrderId == parsedOrderId
                                            && i.InstructionType.InstructionTypeId == (int)eInstructionType.Drop
                                        select i.InstructionId
                                    )
                                    .Count() > 0);

                                if (!orderOnRun)
                                    this.AddOrderToRun(parsedOrderId, parsedJobId);
                            }
                        }
                    }
                }

            RebindPage();
        }

        private void AddOrderToRun(int orderId, int runId)
        {
            CustomPrincipal customPrincipal = (Entities.CustomPrincipal)Page.User;
            string userName = customPrincipal.UserName;
            Facade.IJob facJob = new Facade.Job();
            Entities.Job job = new Orchestrator.Entities.Job();
            job = facJob.GetJob(runId, true, true);

            Entities.InstructionCollection collections = new Orchestrator.Entities.InstructionCollection();
            Entities.Instruction collectionInstruction = null;
            Entities.CollectDrop cd;

            Entities.InstructionCollection deliveries = new Orchestrator.Entities.InstructionCollection();
            Entities.Instruction deliveryInstruction = null;

            Facade.IPoint facPoint = new Facade.Point();
            Facade.IOrder facOrder = new Facade.Order();
            Entities.Point point;
            Entities.Order order = facOrder.GetForOrderID(orderId);

            int pointID = order.CollectionPointID;
            DateTime bookedDateTime = order.CollectionDateTime;
            bool collectionIsAnyTime = order.CollectionIsAnytime;

            // if this setting is true then we want to create a new instruction for the order.
            if (Globals.Configuration.OneDropAndLoadInstructionPerOrder)
                collectionInstruction = null;
            else
                collectionInstruction = job.Instructions.GetForInstructionTypeAndPointID(eInstructionType.Load, pointID, bookedDateTime, collectionIsAnyTime);

            if (collectionInstruction == null)
            {
                collectionInstruction = new Orchestrator.Entities.Instruction();
                collectionInstruction.InstructionTypeId = (int)eInstructionType.Load;
                collectionInstruction.BookedDateTime = bookedDateTime;
                if (collectionIsAnyTime)
                {
                    collectionInstruction.IsAnyTime = true;
                }
                point = facPoint.GetPointForPointId(pointID);
                collectionInstruction.PointID = pointID;
                collectionInstruction.Point = point;
                collectionInstruction.ClientsCustomerIdentityID = point.IdentityId;
                collectionInstruction.CollectDrops = new Orchestrator.Entities.CollectDropCollection();
                collectionInstruction.InstructionOrder = 0;
            }

            cd = new Orchestrator.Entities.CollectDrop();
            cd.PalletTypeID = order.PalletTypeID;
            cd.NoPallets = order.NoPallets;
            cd.NoCases = order.Cases;
            cd.GoodsTypeId = order.GoodsTypeID;
            cd.OrderID = order.OrderID;
            cd.OrderAction = eOrderAction.Default;
            cd.Weight = order.Weight;
            cd.ClientsCustomerReference = order.DeliveryOrderNumber;
            cd.Docket = order.OrderID.ToString();

            collectionInstruction.CollectDrops.Add(cd);
            collections.Add(collectionInstruction);

            eOrderAction orderAction = eOrderAction.Default;
            int deliveryPointID = order.DeliveryPointID;
            DateTime deliveryDateTime = order.DeliveryDateTime;
            bool deliveryIsAnyTime = order.DeliveryIsAnytime;

            // if this setting is true then we want to create a new instruction for the order.
            if (Globals.Configuration.OneDropAndLoadInstructionPerOrder)
                deliveryInstruction = null;
            else
                deliveryInstruction = job.Instructions.GetForInstructionTypeAndPointID(eInstructionType.Drop, deliveryPointID, deliveryDateTime, deliveryIsAnyTime);

            if (deliveryInstruction == null)
            {
                deliveryInstruction = new Orchestrator.Entities.Instruction();
                deliveryInstruction.InstructionTypeId = (int)eInstructionType.Drop;
                deliveryInstruction.BookedDateTime = deliveryDateTime;
                if (deliveryIsAnyTime)
                    deliveryInstruction.IsAnyTime = true;
                point = facPoint.GetPointForPointId(deliveryPointID);
                deliveryInstruction.ClientsCustomerIdentityID = point.IdentityId;
                deliveryInstruction.PointID = deliveryPointID;
                deliveryInstruction.Point = point;

                deliveryInstruction.CollectDrops = new Orchestrator.Entities.CollectDropCollection();
                deliveryInstruction.InstructionOrder = 0;
            }

            cd = new Orchestrator.Entities.CollectDrop();
            cd.PalletTypeID = order.PalletTypeID;
            cd.NoPallets = order.NoPallets;
            cd.NoCases = order.Cases;
            cd.GoodsTypeId = order.GoodsTypeID;
            cd.OrderID = order.OrderID;
            cd.Weight = order.Weight;
            cd.ClientsCustomerReference = order.DeliveryOrderNumber;
            cd.Docket = order.OrderID.ToString();
            cd.OrderAction = orderAction;

            deliveryInstruction.CollectDrops.Add(cd);
            deliveries.Add(deliveryInstruction);

            facOrder.UpdateForCollectionRun(cd.OrderID, deliveryInstruction.PointID, deliveryInstruction.BookedDateTime, deliveryInstruction.IsAnyTime, cd.OrderAction, userName);

            List<Instruction> instructions = new List<Instruction>();
            Entities.CollectDropCollection orderedCollectDrops = new CollectDropCollection();

            for (int i = 0; i < collections.Count; i++)
            {
                orderedCollectDrops = new CollectDropCollection();
                int pos = 0;
                foreach (Entities.CollectDrop orderedCD in collections[i].CollectDrops)
                {
                    if (orderedCollectDrops.Count == 0)
                        orderedCollectDrops.Add(orderedCD);
                    else
                    {
                        for (int x = 0; x < orderedCollectDrops.Count; x++)
                        {
                            if (orderedCD.OrderID > orderedCollectDrops[x].OrderID)
                                pos = x + 1;
                        }
                        orderedCollectDrops.Insert(pos, orderedCD);
                    }
                }
                collections[i].CollectDrops = orderedCollectDrops;
                instructions.Add(collections[i]);
            }

            Entities.InstructionCollection orderedInstructions = new InstructionCollection();
            for (int i = 0; i < deliveries.Count; i++)
            {
                int pos = 0;
                if (orderedInstructions.Count == 0)
                    orderedInstructions.Add(deliveries[i]);
                else
                {
                    for (int y = 0; y < orderedInstructions.Count; y++)
                    {
                        if (deliveries[i].CollectDrops[0].OrderID > orderedInstructions[y].CollectDrops[0].OrderID)
                            pos = y + 1;
                    }
                    orderedInstructions.Insert(pos, deliveries[i]);
                }
            }

            foreach (Entities.Instruction ins in orderedInstructions)
                instructions.Add(ins);

            facJob.AmendInstructions(job, instructions, eLegTimeAlterationMode.Minimal, userName);
        }

        protected override PageStatePersister PageStatePersister
        {
            get { return new HiddenFieldPageStatePersister(this); }
        }

        #endregion

        /// <summary>
        /// Prepares the page for first time viewing.
        /// </summary>
        private void ConfigureDisplay()
        {
            // Populate master page information
            //this.Master.WizardTitle = "Order Group Details";

            CultureInfo culture = new CultureInfo(OrderGroup.LCID);

            // Populate the basic order group information.

            lblRate.Visible = true;
            cboGroupedPlanning.ClearSelection();
            ListItem selected = cboGroupedPlanning.Items.FindByValue(OrderGroup.GroupedPlanning.ToString().ToLower());
            if (selected != null) selected.Selected = true;
            lblOrderCount.Text = OrderGroup.Orders.Count.ToString();
            btnProducePILs.Enabled = OrderGroup.Orders.Count > 0;

            _decimalPlaces = GetDecimalPlaces(OrderGroup);
            if (_decimalPlaces != 2)
                culture.NumberFormat.CurrencyDecimalDigits = _decimalPlaces;

            lblRate.Text = OrderGroup.ForeignRate.ToString("C", culture);


            // Allow a job to be created for this order group, unless any of the orders have been attached to a job.
            Facade.ICollectDrop facCollectDrop = new Facade.CollectDrop();
            Entities.CollectDropCollection collectDrops = facCollectDrop.GetForOrderID(OrderGroup.OrderIDs());
            btnCreateJob.Enabled = collectDrops.Count == 0 && OrderGroup.Orders.Count > 0;

            // If the group is being invoiced, or has been invoiced do not allow the group to be changed.
            Facade.IOrder facOrder = new Facade.Order();
            this.IsBeingInvoiced = facOrder.IsOrderBeingInvoiced(OrderGroup.OrderIDs());

            cboGroupedPlanning.Enabled = !IsBeingInvoiced;
            btnRemoveAll.Visible = !IsBeingInvoiced;
            grdOrders.Columns[grdOrders.Columns.Count - 1].Display = !IsBeingInvoiced;

            // Rebind the grid.
            grdOrders.Rebind();

            this.btnUpdateOrders.Enabled = !IsBeingInvoiced;

            // Set the rate tariff card.
            this.txtRateTariffCard.Text = this.OrderGroup.TariffRateDescription;

            // Configure the allocation field
            bool isAllocationEnabled = WebUI.Utilities.IsAllocationEnabled();
            trAllocation.Visible = isAllocationEnabled;

            this.lblLoadNoText.Text = String.Format("Apply {0} to group.",Globals.Configuration.SystemLoadNumberText);

            if (isAllocationEnabled)
            {
                // An assumption is being made here that, for all orders in a group, the business logic will have
                // ensured that all are allocated (and subcontracted where that has happened) to the same subcontractor.
                int? allocatedToIdentityID = this.OrderGroup.Orders.Any() ? this.OrderGroup.Orders.First().AllocatedToIdentityID : null;

                string allocatedToName = string.Empty;
                if (allocatedToIdentityID.HasValue)
                {
                    var allocatedTo = EF.DataContext.Current.OrganisationSet.First(o => o.IdentityId == allocatedToIdentityID);
                    allocatedToName = allocatedTo.OrganisationName;
                }

                cboAllocatedTo.SelectedValue = allocatedToIdentityID.ToString();
                cboAllocatedTo.Text = allocatedToName;
                lblAllocatedTo.Text = string.IsNullOrEmpty(allocatedToName) ? "- none -" : allocatedToName;

                // The user can't change allocation here if there are no orders in the group or if any order been subcontracted.
                bool changeAllowed = this.OrderGroup.Orders.Any() && !this.OrderGroup.Orders.Any(o => o.JobSubContractID > 0);

                if (changeAllowed)
                {
                    // Check that none of the orders are on a run which has been subcontracted as a whole
                    changeAllowed = !this.OrderGroup.Orders.Any(o => facOrder.IsOrderOnJobSubcontractedAsAWhole(o.OrderID));
                }

                cboAllocatedTo.Visible = changeAllowed;
                lblAllocatedTo.Visible = !changeAllowed;
            }
        }

        private int GetDecimalPlaces(OrderGroup orderGroup)
        {
            int decimalPlaces = 2;

            if (orderGroup != null && orderGroup.Orders.Count > 0)
            {
                Facade.IOrganisation facOrg = new Facade.Organisation();
                Entities.OrganisationDefaultCollection defaults = facOrg.GetForIdentityId(orderGroup.Orders[0].CustomerIdentityID).Defaults;
                if (defaults != null && defaults.Count > 0)
                    decimalPlaces = defaults[0].RateDecimalPlaces;
            }

            return decimalPlaces;
        }


        /// <summary>
        /// Display the infringments returned from the facade call.
        /// </summary>
        /// <param name="result">The result returned from the facade call containing the infringements.</param>
        private void DisplayInfringments(FacadeResult result)
        {
            // The update failed, display infringements.
            ruleInfringements.Infringements = result.Infringements;
            ruleInfringements.DisplayInfringments();
            ruleInfringements.Visible = true;
        }

        /// <summary>
        /// Formats the datetime to take into account the presence of isAnytime flag.
        /// </summary>
        /// <param name="date">The date format.</param>
        /// <param name="isAnytime">The anytime flag.</param>
        /// <returns>The date in a format suitable for display.</returns>
        protected static string GetDate(DateTime date, bool isAnytime)
        {
            string retVal;

            if (isAnytime)
                retVal = date.ToString("dd/MM/yy") + " AnyTime";
            else
                retVal = date.ToString("dd/MM/yy HH:mm");

            return retVal;
        }

        /// <summary>
        /// Obtains the URL to use when the page needs to be refreshed i.e. when orders have been added/associated.
        /// </summary>
        /// <returns>The URL.</returns>
        protected string GetRefreshPageUrl()
        {
            string refreshPageRelativeURL =
                string.Format("~/Groupage/OrderGroupProfile.aspx?ogid={0}", OrderGroup != null ? OrderGroup.OrderGroupID : 0);
            string refreshPageURL = Page.ResolveClientUrl(refreshPageRelativeURL);
            return string.Format("'{0}'", refreshPageURL);
        }

        /// <summary>
        /// Launches a window that will allow the user to add new orders into the system that
        /// will automatically be associated with this order group.
        /// </summary>
        private void LaunchAddOrderWindow()
        {
            ClientScript.RegisterStartupScript(GetType(), "onload", "<script language=\"javascript\" type=\"text/javascript\">setTimeout(\"addNewOrder();\", 2000);</script>");
        }

        /// <summary>
        /// Checks the querystring for a valid order group id and, if one
        /// is found, attempts to load the relevant order group.
        /// </summary>
        /// <returns>True if the order group was loaded, False otherwise.</returns>
        private bool LoadOrderGroup()
        {
            int orderGroupID;
            if (int.TryParse(Request.QueryString[_orderGroupID_QS], out orderGroupID))
            {
                // Set the viewstate element.
                this.OrderGroupID = orderGroupID;
            }
            else
            {
                // Attempt to read from the viewstate element.
                orderGroupID = OrderGroupID;
            }

            if (orderGroupID > 0)
            {
                Facade.IOrderGroup facOrderGroup = new Facade.Order();
                OrderGroup = facOrderGroup.Get(orderGroupID);

                _decimalPlaces = GetDecimalPlaces(OrderGroup);
            }

            return OrderGroup != null;
        }

        /// <summary>
        /// Creates a job to manage the collection and delivery of the orders.
        /// </summary>
        private void CreateJob()
        {
            string userName = ((Entities.CustomPrincipal)Page.User).UserName;

            // Ensure the orders are all of the correct business type.
            Facade.IOrderGroup facOrderGroup = new Facade.Order();
            var businessTypeID = facOrderGroup.DetermineBusinessType(OrderGroup.OrderGroupID, false, userName);

            Facade.IJob facJob = new Facade.Job();
            Entities.FacadeResult res = facJob.CreateJobForOrderGroup(businessTypeID, this.OrderGroup, userName);

            if (res.Success)
            {
                int jobID = res.ObjectId;

                string jobDetailsRelativeURL = string.Format("~/Job/Job.aspx?wiz=true&jobID={0}", jobID);
                string jobDetailsURL = Page.ResolveClientUrl(jobDetailsRelativeURL);
                string jobDetailsJS = string.Format("openResizableDialogWithScrollbars('{0}' + getCSID(), 1220, 930);", jobDetailsURL);
                ScriptManager.RegisterStartupScript(this, this.GetType(), "viewJob", jobDetailsJS, true);
            }
            else
            {
                this.DisplayInfringments(res);
            }

            // Update the UI of the page.
            RebindPage();
        }

        /// <summary>
        /// Reloads the order group and causing the page to be rebound.
        /// </summary>
        private void RebindPage()
        {
            LoadOrderGroup();
            ConfigureDisplay();
        }

        #region Event Handlers

        #region Button Event Handlers

        void btnCreateJob_Click(object sender, EventArgs e)
        {
            if (OrderGroup != null && OrderGroup.Orders.Count > 0)
            {
                // Create a job to manage the collection and delivery of these orders based on their booked times.
                CreateJob();
            }

            RebindPage();
        }

        void btnProducePILs_Click(object sender, EventArgs e)
        {
            if (OrderGroup != null && OrderGroup.Orders.Count > 0)
            {
                NameValueCollection reportParams = new NameValueCollection();
                DataSet dsPIL = null;

                Orchestrator.Facade.ICollectDropLoadOrder facLoadOrder = new Orchestrator.Facade.CollectDrop();

                #region Pop-up Report

                dsPIL = facLoadOrder.GetPILData(OrderGroup.OrderIDs());

                //-------------------------------------------------------------------------------------	
                //									Load Report Section 
                //-------------------------------------------------------------------------------------	
                Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.PIL;
                Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;
                Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = dsPIL;
                Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
                Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";

                // Show the user control
                Page.ClientScript.RegisterStartupScript(this.GetType(), "onload",
                                                        "<script language=\"javascript\">window.open('" +
                                                        Page.ResolveClientUrl("../reports/reportviewer.aspx?wiz=true") +
                                                        "');</script>");

                #endregion
            }

            RebindPage();
        }

        void btnRemoveAll_Click(object sender, EventArgs e)
        {
            if (OrderGroup != null && OrderGroup.Orders.Count > 0)
            {
                List<int> orderIDs = new List<int>();
                foreach (Entities.Order order in OrderGroup.Orders)
                {
                    if (!orderIDs.Contains(order.OrderID))
                        orderIDs.Add(order.OrderID);
                }

                Facade.IOrderGroup facOrderGroup = new Facade.Order();
                FacadeResult result =
                    facOrderGroup.RemoveOrdersFromGroup(OrderGroup.OrderGroupID, orderIDs,
                                                        ((CustomPrincipal)User).UserName);
                if (result.Success)
                {
                    RebindPage();
                }
                else
                {
                    DisplayInfringments(result);
                }
            }

        }

        #endregion

        #region Grid Event Handlers

        void grdOrders_ItemCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
        {
            switch (e.CommandName.ToLower())
            {
                case "removeorder":
                    int orderID = int.Parse(((GridDataItem)e.Item).GetDataKeyValue("OrderID").ToString());
                    List<int> orderIDs = new List<int>(1);
                    orderIDs.Add(orderID);
                    Facade.IOrderGroup facOrderGroup = new Facade.Order();
                    CustomPrincipal principal = (CustomPrincipal)User;
                    FacadeResult result =
                        facOrderGroup.RemoveOrdersFromGroup(OrderGroup.OrderGroupID, orderIDs, principal.UserName);
                    if (result.Success)
                    {
                        RebindPage();
                    }
                    else
                    {
                        DisplayInfringments(result);
                    }
                    break;
            }
        }

        void grdOrders_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                if (e.Item.OwnerTableView.Name == grdOrders.MasterTableView.Name)
                {
                    DataRowView drv = (DataRowView)e.Item.DataItem;
                    eOrderStatus orderStatus = (eOrderStatus)drv["OrderStatusID"];

                    e.Item.Style["background-color"] = Orchestrator.WebUI.Utilities.GetOrderStateColourForHTML(orderStatus, false);

                    Telerik.Web.UI.RadNumericTextBox rntForiegnRate = e.Item.FindControl("rntForeignRate") as Telerik.Web.UI.RadNumericTextBox;
                    rntForiegnRate.Value = double.Parse(drv["ForeignRate"].ToString());

                    // Set the rate culture
                    CultureInfo culture = new CultureInfo(this.OrderGroup.LCID);
                    if (_decimalPlaces != 2)
                        culture.NumberFormat.CurrencyDecimalDigits = _decimalPlaces;

                    rntForiegnRate.Culture = culture;

                    HtmlAnchor hypRun = e.Item.FindControl("hypRun") as HtmlAnchor;
                    if (hypRun != null)
                        if (drv["JobId"] != DBNull.Value)
                        {
                            hypRun.HRef = "javascript:ViewJob(" + drv["JobId"].ToString() + ");";
                            hypRun.InnerHtml = drv["JobId"].ToString();
                        }
                }
            }
        }

        void grdOrders_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            if (OrderGroup != null)
            {
                Facade.IOrder facOrder = new Facade.Order();
                grdOrders.DataSource = facOrder.GetOrdersForList(OrderGroup.OrderIDs(), true, false);
            }
            else
            {
                grdOrders.DataSource = null;
            }
        }

        #endregion

        #region Radio Button List Event Handlers

        void cboGroupedPlanning_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (OrderGroup != null)
            {
                OrderGroup.GroupedPlanning = bool.Parse(cboGroupedPlanning.SelectedValue);

                Facade.IOrderGroup facOrderGroup = new Facade.Order();
                CustomPrincipal principal = (CustomPrincipal)User;
                FacadeResult result = facOrderGroup.Update(OrderGroup, principal.UserName);

                if (result.Success)
                {
                    RebindPage();
                }
                else
                {
                    DisplayInfringments(result);
                }
            }
        }

        #endregion

		#region ComboBox Event Handlers

        private void cboAllocatedTo_SelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if (OrderGroup != null)
            {
                int? allocatedToIdentityID = Utilities.ParseNullable<int>(e.Value);
                Facade.IOrderGroup facOrderGroup = new Facade.Order();
                facOrderGroup.UpdateAllocation(OrderGroupID, allocatedToIdentityID, true, Page.User.Identity.Name);
                RebindPage();
            }
        }

 		#endregion ComboBox Event Handlers

        #endregion
    }
}
