using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Data;
using System.Globalization;
using System.Transactions;

namespace Orchestrator.WebUI.Groupage
{
    public partial class updateOrderDelivery : Orchestrator.Base.BasePage
    {
        //-----------------------------------------------------------------------------------------------------------

        protected void Page_Load(object sender, EventArgs e)
        {
            hidShowConfirmForOrderAfterDays.Value = Orchestrator.Globals.Configuration.ShowConfirmForOrderAfterDays.ToString();
            ((WizardMasterPage)this.Master).WizardTitle = "Update Order Delivery Point";
        }

        //-----------------------------------------------------------------------------------------------------------

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.grdOrders.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdOrders_NeedDataSource);
            this.grdOrders.ItemCreated += new Telerik.Web.UI.GridItemEventHandler(grdOrders_ItemCreated);
            this.grdOrders.ItemDataBound += new Telerik.Web.UI.GridItemEventHandler(grdOrders_ItemDataBound);

            this.btnRefresh.Click += new EventHandler(btnRefresh_Click);
            this.btnUpdateOrders.Click += new EventHandler(btnUpdateOrders_Click);
        }

        //-----------------------------------------------------------------------------------------------------------

        protected void btnUpdateOrders_Click(object sender, EventArgs e)
        {
            // Find out if we need to create a new instruction, if all the orders have been selected then we can 
            // change the instructions point and all the orders delivery points without adding a new instruction.

            // if some orders are left on the existing instruction then we need to remove the selected orders from 
            // the current instruction and create a new instruction based on the new delivery details. 
            // Then add the selected orders to the newly created instruction.

            // Ensure that all points are created.

            if (this.cboNewDeliveryPoint.PointID < 1)
                this.lblError.Text = "Please select a Point.";
            else
            {
                this.lblError.Text = String.Empty;
                Facade.IOrder facorder = new Facade.Order();
                Facade.IInstruction facInstruction = new Facade.Instruction();
                Entities.Instruction instruction = null;

                List<int> orderIds = new List<int>();
                int instructionId = -1;
                int runId = -1;

                foreach (GridDataItem item in grdOrders.Items)
                {
                    if (instructionId == -1)
                    {
                        Label lblInstruction = (Label)item.FindControl("lblInstructionId");
                        int.TryParse(lblInstruction.Text, out instructionId);
                    }
                    if (runId == -1)
                    {
                        HyperLink hypRun = (HyperLink)item.FindControl("hypRun");
                        int.TryParse(hypRun.Text, out runId);
                    }

                    CheckBox chkOrderId = (CheckBox)item.FindControl("chkSelectOrder");
                    int orderId;
                    int.TryParse(chkOrderId.Attributes["OrderID"].ToString(), out orderId);

                    if (chkOrderId.Checked)
                        orderIds.Add(int.Parse(chkOrderId.Attributes["OrderID"].ToString()));
                }

                instruction = facInstruction.GetInstruction(instructionId);

                if (instruction != null)
                {
                    Facade.IJob facJob = new Facade.Job();
                    Entities.Job run = facJob.GetJob(runId, true);

                    if (orderIds.Count == this.grdOrders.Items.Count)
                    {
                        ChangeExistingInstruction(instruction, run, orderIds);

                        List<Entities.Instruction> amendedInstructions = new List<Orchestrator.Entities.Instruction>();
                        amendedInstructions.Add(instruction);
                        // Commit the action.
                        Entities.FacadeResult retVal = null;
                        Entities.CustomPrincipal user = (Entities.CustomPrincipal)Page.User;

                        retVal = facJob.AmendInstructions(run, amendedInstructions, eLegTimeAlterationMode.Enforce_Booked_Times, user.Name);
                            
                    }
                    else
                    {
                        List<Entities.Instruction> amendedInstructions = new List<Orchestrator.Entities.Instruction>();

                        Facade.IOrder facOrder = new Facade.Order();
                        BusinessLogicLayer.ICollectDrop busCollectDrop = new BusinessLogicLayer.CollectDrop();

                        using (TransactionScope tran = new TransactionScope())
                        {
                            foreach (int ordId in orderIds)
                            {
                                Entities.Order currentOrder = facOrder.GetForOrderID(ordId);

                                Entities.CollectDrop collectDrop = instruction.CollectDrops.Find(i => i.OrderID == currentOrder.OrderID);
                                busCollectDrop.Delete(collectDrop, this.Page.User.Identity.Name);

                                this.AddNewDropInstruction(currentOrder, run, amendedInstructions);
                            }

                            // Commit the action.
                            Entities.FacadeResult retVal = null;
                            Entities.CustomPrincipal user = (Entities.CustomPrincipal)Page.User;

                            retVal = facJob.AmendInstructions(run, amendedInstructions, eLegTimeAlterationMode.Enforce_Booked_Times, user.Name);
                            if (retVal.Success)
                            {
                                tran.Complete();
                            }
                        }
                    }
                }

                this.grdOrders.DataSource = null;
                this.grdOrders.Rebind();
            }
        }

        //-----------------------------------------------------------------------------------------------------------

        private void AddNewDropInstruction(Entities.Order currentOrder, Entities.Job run, List<Entities.Instruction> amendedInstructions)
        {
            Facade.IPoint facPoint = new Facade.Point();
            
            // Load the job's current state.
            Facade.IJob facJob = new Facade.Job();

            #region Configure the end instruction.
            Entities.Instruction endInstruction = null;
            if (amendedInstructions.Count > 0)
                endInstruction = amendedInstructions[0];
            else
                endInstruction = run.Instructions.Find(i => i.InstructionTypeId == (int)eInstructionType.Drop && i.PointID == this.cboNewDeliveryPoint.PointID);
            
            // Can a matching instruction be found on the job after the start instruction?
            //endInstruction = job.Instructions.Find(i => i.InstructionOrder > startInstruction.InstructionOrder && i.InstructionTypeId == (int)untetheredLocations.EndInstructionType.Value && i.PointID == untetheredLocations.EndPointID.Value && !i.HasActual);
            if (endInstruction == null)
            {
                // A new instruction is required.
                endInstruction = new Orchestrator.Entities.Instruction();
                endInstruction.PointID = this.cboNewDeliveryPoint.PointID;
                endInstruction.Point = facPoint.GetPointForPointId(this.cboNewDeliveryPoint.PointID);
                endInstruction.InstructionTypeId = (int)eInstructionType.Drop;

                if (this.dteDeliveryByDate.SelectedDate.HasValue)
                {
                    endInstruction.BookedDateTime = this.dteDeliveryByDate.SelectedDate.Value;
                    endInstruction.PlannedArrivalDateTime = this.dteDeliveryByDate.SelectedDate.Value;
                    endInstruction.PlannedDepartureDateTime = this.dteDeliveryByDate.SelectedDate.Value;
                }
                else
                {
                    endInstruction.BookedDateTime = currentOrder.DeliveryDateTime;
                    endInstruction.PlannedArrivalDateTime = currentOrder.DeliveryDateTime;
                    endInstruction.PlannedDepartureDateTime = currentOrder.DeliveryDateTime;
                }

                if (this.rdDeliveryIsAnytime.Checked)
                    endInstruction.IsAnyTime = true;
                else
                    endInstruction.IsAnyTime = false;

                endInstruction.JobId = run.JobId;
                endInstruction.ClientsCustomerIdentityID = currentOrder.CustomerIdentityID;

                // Record that this instruction has been amended.
                amendedInstructions.Add(endInstruction);
            }
            else
                if(amendedInstructions.Count == 0)
                    amendedInstructions.Add(endInstruction);

            // Add the collect drop for this order to the instruction.
            Entities.CollectDrop endCollectDrop = new Entities.CollectDrop();
            endCollectDrop.Order = currentOrder;
            endCollectDrop.OrderID = currentOrder.OrderID;
            endCollectDrop.NoPallets = currentOrder.NoPallets;
            endCollectDrop.ClientsCustomerReference = currentOrder.CustomerOrderNumber;
            endCollectDrop.GoodsTypeId = currentOrder.GoodsTypeID;
            endCollectDrop.NoCases = currentOrder.Cases;
            endCollectDrop.Weight = currentOrder.Weight;
            endCollectDrop.Docket = currentOrder.OrderID.ToString();

            endCollectDrop.OrderAction = eOrderAction.Default;
            endInstruction.CollectDrops.Add(endCollectDrop);

            this.UpdateOrder(currentOrder);
            #endregion
        }

        //-----------------------------------------------------------------------------------------------------------

        private void ChangeExistingInstruction(Entities.Instruction instruction, Entities.Job run, List<int> orderIds)
        {
            Facade.IInstruction facInstruction = new Facade.Instruction();
            // no need to create a new instruction.
            Entities.FacadeResult retVal = new Entities.FacadeResult();
            Orchestrator.Facade.IOrder facOrder = new Orchestrator.Facade.Order();
            Facade.Job facJob = new Facade.Job();
            Entities.CollectDropCollection collectDrops = new Orchestrator.Entities.CollectDropCollection();
            // Update the Order and the relevant instructions
            foreach (int ordId in orderIds)
            {
                DataSet dsOrders = facOrder.GetLegsForOrder(ordId);
                // Get the last job id from the dataset
                int jobId = -1;
                if (dsOrders.Tables[0].Rows.Count > 0)
                    jobId = Convert.ToInt32(dsOrders.Tables[0].Rows[dsOrders.Tables[0].Rows.Count - 1]["JobID"].ToString());

                Entities.Order currentOrder = facOrder.GetForOrderID(ordId);
                if (instruction.CollectDrops.GetForOrderID(currentOrder.OrderID) != null)
                {
                    facInstruction.UpdatePointID(instruction.InstructionID, this.cboNewDeliveryPoint.PointID, this.Page.User.Identity.Name);
                    instruction.PointID = this.cboNewDeliveryPoint.PointID;

                    this.UpdateOrder(currentOrder);

                    instruction.BookedDateTime = currentOrder.DeliveryDateTime;
                    instruction.PlannedArrivalDateTime = currentOrder.DeliveryDateTime;
                    instruction.PlannedDepartureDateTime = currentOrder.DeliveryDateTime;
                }
            }
        }

        //-----------------------------------------------------------------------------------------------------------

        public void UpdateOrder(Entities.Order currentOrder)
        {
            Orchestrator.Facade.IOrder facOrder = new Orchestrator.Facade.Order();

            if (this.dteDeliveryByDate.SelectedDate.HasValue)
            {
                // Update Order
                // Delivery from
                if (rdDeliveryTimedBooking.Checked)
                {
                    currentOrder.DeliveryFromDateTime = dteDeliveryByDate.SelectedDate.Value.Date.Add(
                        new TimeSpan(
                            dteDeliveryByTime.SelectedDate.Value.Hour,
                            dteDeliveryByTime.SelectedDate.Value.Minute,
                            0)
                    );
                }
                else if (rdDeliveryBookingWindow.Checked)
                {
                    currentOrder.DeliveryFromDateTime = dteDeliveryFromDate.SelectedDate.Value.Date.Add(
                        new TimeSpan(
                            dteDeliveryFromTime.SelectedDate.Value.Hour,
                            dteDeliveryFromTime.SelectedDate.Value.Minute,
                            0)
                         );
                }
                else if (rdDeliveryIsAnytime.Checked)
                {
                    currentOrder.DeliveryFromDateTime = dteDeliveryByDate.SelectedDate.Value.Date;
                }

                // Delivery by
                currentOrder.DeliveryDateTime = dteDeliveryByDate.SelectedDate.Value.Date.Add(
                    new TimeSpan(
                        dteDeliveryByTime.SelectedDate.Value.Hour,
                        dteDeliveryByTime.SelectedDate.Value.Minute,
                        0)
                );
            }

            currentOrder.DeliveryPointID = this.cboNewDeliveryPoint.PointID;

            facOrder.Update(currentOrder, this.Page.User.Identity.Name);
        }

        //-----------------------------------------------------------------------------------------------------------

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            this.grdOrders.Rebind();
        }

        //-----------------------------------------------------------------------------------------------------------

        protected void grdOrders_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                DataRowView row = (DataRowView)e.Item.DataItem;
                string orderId = row["OrderID"].ToString();
                string runId = row["JobId"].ToString();

                if (String.IsNullOrEmpty(this.hypRunTitle.Text))
                {
                    this.hypRunTitle.Text = runId;
                    this.hypRunTitle.NavigateUrl = "javascript:ViewRun(" + runId + ");";
                }

                using (CheckBox chk = (CheckBox)e.Item.FindControl("chkSelectOrder"))
                {
                    chk.Attributes.Add("onClick", string.Format("javascript:HandleSelection(this, {0});",
                                 e.Item.ItemIndex));

                    chk.Attributes.Add("OrderId", orderId);
                }

                HyperLink hypUpdateOrder = e.Item.FindControl("hypUpdateOrder") as HyperLink;
                hypUpdateOrder.NavigateUrl = string.Format("javascript:ViewOrderProfile({0});", orderId);
                hypUpdateOrder.Text = orderId;

                HyperLink hypRun = e.Item.FindControl("hypRun") as HyperLink;
                hypRun.NavigateUrl = string.Format("javascript:ViewRun({0});", runId);
                hypRun.Text = runId;

                Label lblInstructionId = e.Item.FindControl("lblInstructionId") as Label;
                lblInstructionId.Text = row["InstructionId"].ToString(); 

                CultureInfo CurrentCulture = new CultureInfo(int.Parse(row["LCID"].ToString()));

                Label lblRate = e.Item.FindControl("lblRate") as Label;
                if (lblRate != null && row["ForeignRate"] != DBNull.Value)
                    lblRate.Text = ((decimal)row["ForeignRate"]).ToString("C", CurrentCulture);
            }
        }
        
        //-----------------------------------------------------------------------------------------------------------

        protected void grdOrders_ItemCreated(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            //throw new NotImplementedException();
        }

        //-----------------------------------------------------------------------------------------------------------

        protected void grdOrders_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            this.grdOrders.DataSource = this.GetData();
        }

        //-----------------------------------------------------------------------------------------------------------

        public DataSet GetData()
        {
            DataSet dsOrders = null;
            int orderId = -1;
            int instructionId = -1;
            Facade.IOrder facOrder = new Facade.Order();

            if (Request.QueryString["OrderId"] != null)
            {
                int.TryParse(Request.QueryString["OrderId"].ToString(), out orderId);
                dsOrders = facOrder.GetOrdersOnSameDropInstruction(orderId);
            }
            else
            {
                int.TryParse(Request.QueryString["InstructionId"].ToString(), out instructionId);
                dsOrders = facOrder.GetOrdersForInstructionID(instructionId);
            }

            if (dsOrders != null && dsOrders.Tables[0].Rows.Count > 0)
                lblTitleDelivery.Text = dsOrders.Tables[0].Rows[0]["DeliveryPointDescription"].ToString();
            else
                lblTitleDelivery.Text = String.Empty;

            return dsOrders;
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
    }
}
