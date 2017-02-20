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

namespace Orchestrator.WebUI.Job.Wizard.UserControls
{

    public partial class OrderHandling : System.Web.UI.UserControl, WebUI.IDefaultButton
    {
        #region Control Variables

        private readonly string VS_OrderHandling = "_orderHandling";

        #endregion

        #region Page Variables

        private int m_jobId;
        private Entities.Job m_job;
        private bool m_isAmendment = false;
        private bool m_isUpdate = false;

        private Entities.Instruction m_instruction = null;
        private int m_instructionIndex;
        private List<int> m_additions = new List<int>();
        private List<int> m_removals = new List<int>();

        private List<Entities.Instruction> m_collections = new List<Entities.Instruction>();
        private List<Entities.Instruction> m_deliveries = new List<Entities.Instruction>();
        List<int> m_changedInstructions = new List<int>();

        #endregion

        #region Property Interface

        private DataSet OrderHandlingData
        {
            get { return (DataSet)ViewState[VS_OrderHandling]; }
            set { ViewState[VS_OrderHandling] = value; }
        }

        protected Entities.Instruction CurrentInstruction
        {
            get { return m_instruction; }
            set { m_instruction = value; }
        }

        #endregion

        protected void Page_Load(object sender, EventArgs e)
        {
            m_jobId = Convert.ToInt32(Request.QueryString["jobId"]);

            if (m_jobId > 0)
                m_isUpdate = true;

            plcLegTimeAlteringMode.Visible = m_isUpdate;

            // Retrieve the job from the session variable
            m_job = (Entities.Job)Session[wizard.C_JOB];

            if (Session[wizard.C_INSTRUCTION_INDEX] != null)
            {
                m_instructionIndex = (int)Session[wizard.C_INSTRUCTION_INDEX];

                if (!m_isUpdate && m_instructionIndex != m_job.Instructions.Count)
                    m_isAmendment = true;
            }
            if ((Entities.Instruction)Session[wizard.C_INSTRUCTION] != null)
                m_instruction = (Entities.Instruction)Session[wizard.C_INSTRUCTION];

            if (Session[wizard.C_ADDED_ORDERS] as List<int> != null)
                m_additions = Session[wizard.C_ADDED_ORDERS] as List<int>;
            if (Session[wizard.C_REMOVED_ORDERS] as List<int> != null)
                m_removals = Session[wizard.C_REMOVED_ORDERS] as List<int>;

            if (!IsPostBack)
            {
                // Configure cancel button confirmation alert.
                btnCancel.Attributes.Add("onClick", wizard.C_CONFIRM_MESSAGE);

                PopulateStaticControls();

                // Construct the data.
                ConfigureOrderHandlingData();
                gvOrders.DataSource = OrderHandlingData;
                gvOrders.DataBind();

                // Configure the booked date and time for this new instruction.
                if (!m_isAmendment)
                    SetBookedDateTimeLabel(m_instruction.BookedDateTime, m_instruction.IsAnyTime);
            }
        }

        private void PopulateStaticControls()
        {
            // Order Actions.
            string[] actions = Enum.GetNames(typeof(eOrderAction));
            for (int actionIndex = 0; actionIndex < actions.Length; actionIndex++)
                actions[actionIndex] = actions[actionIndex].Replace("_", " ");
            rdoOrderAction.DataSource = actions;
            rdoOrderAction.DataBind();
            rdoOrderAction.Items[0].Selected = true;
            rdoOrderAction.Items[0].Text = "Deliver";

            // Leg Time Altering Mode.
            string[] modes = Enum.GetNames(typeof(eLegTimeAlterationMode));
            for (int modeIndex = 0; modeIndex < modes.Length; modeIndex++)
                modes[modeIndex] = modes[modeIndex].Replace("_", " ");
            rdoLegTimeAlteringMode.DataSource = modes;
            rdoLegTimeAlteringMode.DataBind();
            rdoLegTimeAlteringMode.Items[0].Selected = true;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            lnkUseEarliestTime.Click += new EventHandler(lnkUseEarliestTime_Click);
            lnkUseCollection.Click += new EventHandler(lnkUseCollection_Click);

            btnUpdateOrderHandling.Click += new EventHandler(btnUpdateOrderHandling_Click);

            repValidationMessages.ItemDataBound += new RepeaterItemEventHandler(repValidationMessages_ItemDataBound);

            gvOrders.ItemCommand += new GridCommandEventHandler(gvOrders_ItemCommand);
            gvOrders.ItemCreated += new GridItemEventHandler(gvOrders_ItemCreated);
            gvOrders.ItemDataBound += new GridItemEventHandler(gvOrders_ItemDataBound);
            gvOrders.NeedDataSource += new GridNeedDataSourceEventHandler(gvOrders_NeedDataSource);

            btnBack.Click += new EventHandler(btnBack_Click);
            btnNext.Click += new EventHandler(btnNext_Click);
            btnCancel.Click += new EventHandler(btnCancel_Click);
        }

        #region Protected Methods

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

        #region Private Methods

        /// <summary>
        /// Adds new entries into the changed instructions list if they do not already exist and apply to instructions existing in the
        /// database.
        /// </summary>
        /// <param name="instructionID">The instruction id to add.</param>
        private void AddToChangedInstructions(int instructionID)
        {
            if (instructionID > 0 && m_changedInstructions.IndexOf(instructionID) == -1)
                m_changedInstructions.Add(instructionID);
        }

        /// <summary>
        /// Configures the order handling dataset to hold the relevant information and flags to faciliate the specification of how
        /// each individual order will be handled.
        /// </summary>
        private void ConfigureOrderHandlingData()
        {
            // Get the original dataset for this instruction.
            Facade.IOrder facOrder = new Facade.Order();
            DataSet data = facOrder.GetOrdersForInstructionID(m_instruction.InstructionID);

            // Add the additional control columns.
            data.Tables[0].Columns.Add(new DataColumn("IsDirty", typeof(bool)));

            foreach (DataRow row in data.Tables[0].Rows)
            {
                row["IsDirty"] = false;

                // Mark the row as dirty if the instruction time has changed.
                if ((DateTime)row["CollectionDateTime"] != m_instruction.BookedDateTime || (bool)row["CollectionIsAnyTime"] != m_instruction.IsAnyTime)
                    row["IsDirty"] = true;

                // Alter Default to Deliver.
                row["OrderAction"] = ((string)row["OrderAction"]).Replace("_", " ").Replace("Default", "Deliver");

                // Ensure that the collection date and time is correct.
                row["CollectionDateTime"] = m_instruction.BookedDateTime;
                row["CollectionIsAnyTime"] = m_instruction.IsAnyTime;

                // Mark any items set for removal as being dirty, anything else is not dirty.
                if (m_removals.IndexOf((int)row["OrderID"]) > -1)
                    row["IsDirty"] = true;
            }

            // Add the additional orders being collected.
            Facade.IOrganisation facOrganisation = new Facade.Organisation();
            Facade.IPoint facPoint = new Facade.Point();
            foreach (int orderID in m_additions)
            {
                // Retrieve the order from the database.
                Entities.Order order = facOrder.GetForOrderID(orderID);

                // Configure the sensible defaults.
                eOrderAction orderAction;
                Entities.Point deliveryPoint;
                DateTime deliveryDateTime;
                bool deliveryIsAnyTime;
                if (order.PlannedForDelivery)
                {
                    // The order is planned for delivery so the sensible default is to cross-dock to the delivery run collection point.
                    orderAction = eOrderAction.Cross_Dock;
                    deliveryPoint = facPoint.GetPointForPointId(order.DeliveryRunCollectionPointID);
                    deliveryDateTime = order.DeliveryRunCollectionDateTime;
                    deliveryIsAnyTime = order.DeliveryRunCollectionIsAnytime;
                }
                else
                {
                    // The order has not been planned for delivery so the sensible default is to deliver to the delivery point.
                    orderAction = eOrderAction.Default;
                    deliveryPoint = facPoint.GetPointForPointId(order.DeliveryPointID);
                    deliveryDateTime = order.DeliveryDateTime;
                    deliveryIsAnyTime = order.DeliveryIsAnytime;
                }

                // Construct the new row (with sensible defaults).
                DataRow newRow = data.Tables[0].NewRow();
                newRow["OrderID"] = orderID;
                newRow["CustomerOrganisationName"] = facOrganisation.GetForIdentityId(order.CustomerIdentityID).OrganisationName;
                newRow["CustomerOrderNumber"] = order.CustomerOrderNumber;
                newRow["CollectionPointID"] = m_instruction.Point.PointId;
                newRow["CollectionPointDescription"] = m_instruction.Point.Description;
                newRow["CollectionDateTime"] = m_instruction.BookedDateTime;
                newRow["CollectionIsAnyTime"] = m_instruction.IsAnyTime;
                newRow["OrderAction"] = orderAction.ToString().Replace("_", " ").Replace("Default", "Deliver");
                newRow["DeliveryPointID"] = deliveryPoint.PointId;
                newRow["DeliveryPointDescription"] = deliveryPoint.Description;
                newRow["DeliveryDateTime"] = deliveryDateTime;
                newRow["DeliveryIsAnyTime"] = deliveryIsAnyTime;
                newRow["DeliveryCalledIn"] = false;
                newRow["DeliveryOrderNumber"] = order.DeliveryOrderNumber;
                newRow["GoodsTypeDescription"] = Facade.GoodsType.GetForGoodsTypeId(order.GoodsTypeID).Description;
                newRow["PalletTypeDescription"] = Facade.PalletType.GetForPalletTypeId(order.PalletTypeID).Description;
                newRow["NoPallets"] = order.NoPallets;
                newRow["WeightShortCode"] = Facade.WeightType.GetForWeightTypeId(order.WeightTypeID).ShortCode;
                newRow["Weight"] = order.Weight;
                newRow["Cases"] = order.Cases;
                newRow["Notes"] = order.Notes;
                newRow["PlannedForDelivery"] = order.PlannedForDelivery;
                newRow["OrderStatusID"] = (int)order.OrderStatus;
                newRow["IsDelivered"] = order.IsDelivered;
                newRow["CollectionRunDeliveryPointID"] = deliveryPoint.PointId;
                newRow["CollectionRunDeliveryDatetime"] = deliveryDateTime;
                newRow["CollectionRunDeliveryIsAnytime"] = deliveryIsAnyTime;
                if (order.PlannedForDelivery)
                {
                    newRow["DeliveryRunCollectionPointID"] = order.DeliveryRunCollectionPointID;
                    newRow["DeliveryRunCollectionDateTime"] = order.DeliveryRunCollectionDateTime;
                    newRow["DeliveryRunCollectionIsAnytime"] = order.DeliveryRunCollectionIsAnytime;
                }
                else
                {
                    newRow["DeliveryRunCollectionPointID"] = deliveryPoint.PointId;
                    newRow["DeliveryRunCollectionDateTime"] = deliveryDateTime;
                    newRow["DeliveryRunCollectionIsAnytime"] = deliveryIsAnyTime;
                }
                newRow["IsDirty"] = true; // This is a new item so must be worked upon.
                
                // Add the new row to the dataset.
                data.Tables[0].Rows.Add(newRow);
            }

            // Store the constructed dataset.
            data.AcceptChanges();
            OrderHandlingData = data;
        }

        /// <summary>
        /// Finds the instruction in the list of instructions that is providing the specified operation at the specified point, date and time.
        /// </summary>
        /// <param name="instructions">The instructions to interrogate.</param>
        /// <param name="pointID">The point the operation must be taking place on.</param>
        /// <param name="bookedDateTime">The date and time the operation must be taking place at.</param>
        /// <param name="isAnyTime">Whether the operation can take time at anytime.</param>
        /// <param name="instructionType">The operation being performed.</param>
        /// <returns>The matching instruction, or null if no match is found.</returns>
        private Entities.Instruction FindInstructionForLocationAndOperation(ref List<Entities.Instruction> instructions, int pointID, DateTime bookedDateTime, bool isAnyTime, eInstructionType instructionType)
        {
            Entities.Instruction foundInstruction = instructions.Find(
                delegate(Entities.Instruction instruction)
                {
                    return
                        instruction.PointID == pointID &&
                        instruction.BookedDateTime.Subtract(new TimeSpan(0, 0, 0, instruction.BookedDateTime.Second, instruction.BookedDateTime.Millisecond)) == bookedDateTime.Subtract(new TimeSpan(0, 0, 0, bookedDateTime.Second, bookedDateTime.Millisecond)) &&
                        instruction.IsAnyTime == isAnyTime &&
                        (eInstructionType)instruction.InstructionTypeId == instructionType &&
                        (
                            instruction.InstructionActuals == null ||
                            instruction.InstructionActuals.Count == 0
                        );
                });

            return foundInstruction;
        }

        /// <summary>
        /// Locates the instruction in the list of instructions that deals with the specified order.
        /// </summary>
        /// <param name="instructions">The list of instructions to interrogate.</param>
        /// <param name="orderID">The order id to search for.</param>
        /// <returns>The instruction found, or null if no match is found.</returns>
        private Entities.Instruction FindInstructionForOrderID(ref List<Entities.Instruction> instructions, int orderID)
        {
            Entities.Instruction foundInstruction = instructions.Find(
                delegate(Entities.Instruction instruction)
                {
                    return instruction.CollectDrops.GetForOrderID(orderID) != null;
                });
            return foundInstruction;
        }

        /// <summary>
        /// Generates a new collect drop from the supplied information.
        /// </summary>
        /// <param name="order">The order to generate a collect drop for.</param>
        /// <param name="orderAction">The order action performed at the location.</param>
        /// <param name="collectDropSummaryID">The id of the collect drop summary this collect drop will be added to.</param>
        /// <returns>A collect drop ready for insertion into a collect drop summary.</returns>
        private Entities.CollectDrop GenerateCollectDropForOrder(Entities.Order order, eOrderAction orderAction, int instructionID)
        {
            Entities.CollectDrop cd = new Entities.CollectDrop();
            cd.InstructionID = instructionID;
            cd.Docket = order.OrderID.ToString();
            cd.OrderID = order.OrderID;
            cd.Weight = order.Weight;
            cd.NoCases = 0;
            cd.NoPallets = order.NoPallets;
            cd.GoodsTypeId = order.GoodsTypeID;
            cd.OrderAction = orderAction;

            return cd;
        }
        
        /// <summary>
        /// Interrogates the job and places all load instructions into m_collections and all order handling trunks and all drops into m_deliveries.
        /// </summary>
        private void GenerateCollectionsAndDeliveries()
        {
            if (m_isUpdate)
            {
                // Populate from the legs collection
                foreach (Entities.Instruction instruction in m_job.Instructions)
                    if (instruction.InstructionID == m_instruction.InstructionID)
                        m_collections.Add(m_instruction);
                    else
                        switch ((eInstructionType)instruction.InstructionTypeId)
                        {
                            case eInstructionType.Load:
                                m_collections.Add(instruction);
                                break;
                            case eInstructionType.Drop:
                                m_deliveries.Add(instruction);
                                break;
                            case eInstructionType.Trunk:
                                m_deliveries.Add(instruction);
                                break;
                        }
            }
            else
            {
                // Populate from the instructions collection
                foreach (Entities.Instruction instruction in m_job.Instructions)
                    if (instruction.InstructionID == m_instruction.InstructionID)
                        m_collections.Add(m_instruction);
                    else
                        switch ((eInstructionType)instruction.InstructionTypeId)
                        {
                            case eInstructionType.Load:
                                m_collections.Add(instruction);
                                break;
                            case eInstructionType.Drop:
                                m_deliveries.Add(instruction);
                                break;
                            case eInstructionType.Trunk:
                                m_deliveries.Add(instruction);
                                break;
                        }
            }
        }

        /// <summary>
        /// Generates an instruction to be added to the job.
        /// </summary>
        /// <param name="pointID">The ID of the point the instuction takes place at.</param>
        /// <param name="bookedDateTime">The date and time the instruction should take place at.</param>
        /// <param name="isAnyTime">A flag indicating if the booked date and time should be considered to be anytime.</param>
        /// <param name="instructionType">The action being performed at the location.</param>
        /// <returns>The generated instruction.</returns>
        private Entities.Instruction GenerateInstruction(int pointID, DateTime bookedDateTime, bool isAnyTime, eInstructionType instructionType)
        {
            Facade.IPoint facPoint = new Facade.Point();
            Entities.Point point = facPoint.GetPointForPointId(pointID);
            Entities.Instruction instruction = new Entities.Instruction();

            instruction.InstructionTypeId = (int)instructionType;
            instruction.JobId = m_jobId;
            instruction.BookedDateTime = bookedDateTime;
            instruction.IsAnyTime = isAnyTime;
            instruction.PointID = point.PointId;
            instruction.Point = point;
            instruction.CollectDrops = new Entities.CollectDropCollection();
            if (instructionType == eInstructionType.Drop || instructionType == eInstructionType.Trunk)
                instruction.ClientsCustomerIdentityID = point.IdentityId;

            return instruction;
        }

        /// <summary>
        /// Sends the user to the specified step in the job manipulation process.  Passing this job's job id on the querystring if the
        /// job already exists in the database.
        /// </summary>
        /// <param name="step">The string representation of the step to jump to, this are defined in the wizard.aspx page.</param>
        private void GoToStep(string step)
        {
            string url = "wizard.aspx?step=" + step;

            if (m_isUpdate)
                url += "&jobId=" + m_jobId.ToString();

            Response.Redirect(url);
        }

        /// <summary>
        /// Removes the docket associated with the given order id from the instruction.
        /// </summary>
        /// <param name="orderID">The order id to search for.</param>
        /// <param name="instruction">The instruction to search.</param>
        private void RemoveOrderFromInstruction(int orderID, Entities.Instruction instruction)
        {
            if (instruction != null)
            {
                for (int cdIndex = 0; cdIndex < instruction.CollectDrops.Count; cdIndex++)
                    if (instruction.CollectDrops[cdIndex].OrderID == orderID)
                    {
                        instruction.CollectDrops.RemoveAt(cdIndex);
                        cdIndex--;
                        AddToChangedInstructions(instruction.InstructionID);

                        if (instruction.InstructionTypeId == (int)eInstructionType.Load)
                            RemoveOrderFromInstruction(orderID, m_deliveries.Find(delegate(Entities.Instruction deliveryInstruction) { return deliveryInstruction.CollectDrops.GetForOrderID(orderID) != null; }));
                    }
            }
        }
        
        /// <summary>
        /// Removes any order in the job that has been flagged for removal.
        /// </summary>
        private void RemoveOrdersFlaggedForRemoval()
        {
            foreach (int orderID in m_removals)
                RemoveOrderFromInstruction(orderID, m_instruction);
        }

        /// <summary>
        /// Updates the booked date time that will be used when actually creating the new instruction (sticks it in a label for later use).
        /// </summary>
        private void SetBookedDateTimeLabel(DateTime bookedDateTime, bool isAnyTime)
        {
            bookedDateTime = bookedDateTime.Subtract(new TimeSpan(0, 0, 0, bookedDateTime.Second, bookedDateTime.Millisecond));

            lblBookedDateTime.Text = bookedDateTime.ToString("dd/MM/yy") + " " + (isAnyTime ? "AnyTime" : bookedDateTime.ToString("HH:mm"));

            // Locate existing instruction in the job that is collecting from the same point with the same date and time.
            GenerateCollectionsAndDeliveries();
            Entities.Instruction matchedInstruction = m_collections.Find(
                delegate(Entities.Instruction collection)
                {
                    return
                        collection.InstructionID > 0 &&
                        (collection.InstructionActuals == null || collection.InstructionActuals.Count == 0) &&
                        collection.Point.PointId == m_instruction.Point.PointId &&
                        collection.BookedDateTime.Subtract(new TimeSpan(0, 0, 0, collection.BookedDateTime.Second)) == bookedDateTime &&
                        collection.IsAnyTime == isAnyTime;
                }
            );

            plcUseInstruction.Visible = matchedInstruction != null;
            if (matchedInstruction != null)
                hidUseInstructionID.Value = matchedInstruction.InstructionID.ToString();
        }

        /// <summary>
        /// Checks the order actions being applied to ensure that they make sense, returns the result, and if not valid binds information
        /// to a repeater on the page.
        /// </summary>
        /// <returns></returns>
        private bool ValidateOrderHandling()
        {
            List<OrderHandlingValidationMessage> messages = new List<OrderHandlingValidationMessage>();

            eOrderAction orderAction = (eOrderAction)Enum.Parse(typeof(eOrderAction), rdoOrderAction.SelectedValue.Replace(" ", "_"));
            GridItemCollection items = gvOrders.SelectedItems;

            Facade.IOrder facOrder = new Facade.Order();
            Facade.IPoint facPoint = new Facade.Point();
            Facade.IInstruction facInstruction = new Facade.Instruction();

            DateTime arrivalDateTime = dteBookedDate.SelectedDate.Value;
            arrivalDateTime = arrivalDateTime.Subtract(arrivalDateTime.TimeOfDay);
            if (dteBookedTime.SelectedDate.HasValue == false)
                arrivalDateTime = arrivalDateTime.Add(dteBookedTime.SelectedDate.Value.TimeOfDay);

            // 1. Ensure the user has selected at least one order.
            if (items.Count == 0)
                messages.Add(new OrderHandlingValidationMessage(OrderHandlingValidationMessage.eMessageType.Error, "You must select at least one order to apply this change to."));

            foreach (GridDataItem item in items)
            {
                int orderID = int.Parse(item.OwnerTableView.DataKeyValues[item.ItemIndex]["OrderID"].ToString());
                DataRow row = OrderHandlingData.Tables[0].Select("OrderID = " + orderID.ToString())[0];
                Entities.Order order = facOrder.GetForOrderID(orderID);

                if (orderAction == eOrderAction.Default)
                {
                    Entities.Point deliveryPoint = facPoint.GetPointForPointId(order.DeliveryPointID);

                    // 2. Attempting to change the delivery point.
                    if (ucPoint.SelectedPoint.PointId != deliveryPoint.PointId)
                        messages.Add(new OrderHandlingValidationMessage(OrderHandlingValidationMessage.eMessageType.Error, string.Format("You can not deliver order {0} to {1} as it has been configured to be delivered to {2}.", order.CustomerOrderNumber, ucPoint.SelectedPoint.Description, deliveryPoint.Description)));

                    // 3. Attempting to deliver an order which has already been specified for delivery on another job.
                    Entities.Instruction deliveryInstruction = facInstruction.GetDeliveryInstructionForOrder(orderID);
                    if (deliveryInstruction != null && deliveryInstruction.JobId != m_instruction.JobId)
                        messages.Add(new OrderHandlingValidationMessage(OrderHandlingValidationMessage.eMessageType.Error, string.Format("Order {0} has already been specified for delivery on a separate job.", order.CustomerOrderNumber)));
                }

                // 4. Warn if the user is trying to get an order to a location after it should be delivered.
                bool willArriveLate = order.DeliveryDateTime < arrivalDateTime;
                if (willArriveLate)
                    messages.Add(new OrderHandlingValidationMessage(OrderHandlingValidationMessage.eMessageType.Warning, string.Format("Order {0} will arrive at the location after it's delivery time {1}.", order.CustomerOrderNumber, GetDate(order.DeliveryDateTime, order.DeliveryIsAnytime))));
                
                // 5. Check the user isn't sending the goods back to their collection point.
                if (ucPoint.SelectedPoint.PointId == m_instruction.PointID)
                    messages.Add(new OrderHandlingValidationMessage(OrderHandlingValidationMessage.eMessageType.Error, string.Format("You can not choose to send order {0} back to it's collection point.", order.CustomerOrderNumber)));
            }

            // Bind messages to the repeater.
            repValidationMessages.DataSource = messages;
            repValidationMessages.DataBind();
            repValidationMessages.Visible = messages.Count > 0;

            return messages.Find(
                delegate(OrderHandlingValidationMessage message)
                {
                    return message.Type == OrderHandlingValidationMessage.eMessageType.Error;
                }) == null;
        }

        #endregion

        #region Event Handlers

        #region Grid Events

        void gvOrders_ItemCommand(object source, GridCommandEventArgs e)
        {
            Facade.IOrder facOrder = new Facade.Order();
            Facade.IPoint facPoint = new Facade.Point();
            int orderID = int.Parse(e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["OrderID"].ToString());

            switch (e.CommandName.ToLower())
            {
                case "populatepoint":
                    DataRow row = OrderHandlingData.Tables[0].Select("OrderID = " + orderID.ToString())[0];
                    int pointID = (int)row["DeliveryPointID"];
                    ucPoint.SelectedPoint = facPoint.GetPointForPointId(pointID);
                    dteBookedDate.SelectedDate = (DateTime)row["DeliveryDateTime"];
                    if ((bool)row["DeliveryIsAnyTime"])
                        dteBookedTime.SelectedDate = (DateTime?) null;
                    else
                        dteBookedTime.SelectedDate = dteBookedDate.SelectedDate.Value;
                    rdoOrderAction.ClearSelection();
                    string orderAction = (string)row["OrderAction"];
                    orderAction = orderAction.Replace("Deliver", "Default");
                    rdoOrderAction.Items.FindByValue(orderAction).Selected = true;
                    break;
                case "populatedeliverypoint":
                    Entities.Order order = facOrder.GetForOrderID(orderID);
                    ucPoint.SelectedPoint = facPoint.GetPointForPointId(order.DeliveryPointID);
                    dteBookedDate.SelectedDate = order.DeliveryDateTime;
                    if (order.DeliveryIsAnytime)
                        dteBookedTime.SelectedDate = (DateTime?)null;
                    else
                        dteBookedTime.SelectedDate = order.DeliveryDateTime;
                    rdoOrderAction.ClearSelection();
                    rdoOrderAction.Items.FindByValue("Default").Selected = true;
                    break;
            }
        }

        void gvOrders_ItemCreated(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem gridItem = e.Item as GridDataItem;
                foreach (GridColumn column in gvOrders.Columns)
                    if (column is GridBoundColumn)
                        gridItem[column.UniqueName].ToolTip = gridItem[column.UniqueName].Text;
            }
        }

        void gvOrders_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                DataRowView drv = e.Item.DataItem as DataRowView;

                // This is a Patch until SP2 of the Telerik Grid
                GridDataItem row = e.Item as GridDataItem;
                CheckBox checkboxSelectColumn = (row["checkboxSelectColumn"]).Controls[1] as CheckBox;

                int orderID = int.Parse(row.OwnerTableView.DataKeyValues[row.ItemIndex]["OrderID"].ToString());

                // Can not copy through the delivery point and datetime if the order is planned for delivery.
                Facade.IOrder facOrder = new Facade.Order();
                Facade.IPoint facPoint = new Facade.Point();
                Entities.Order order = facOrder.GetForOrderID(orderID);
                LinkButton lnkFinalDeliveryPoint = e.Item.FindControl("lnkFinalDeliveryPoint") as LinkButton;
                Label lblDeliveryDateTime = e.Item.FindControl("lblDeliveryDateTime") as Label;
                lnkFinalDeliveryPoint.CommandArgument = order.DeliveryPointID.ToString();
                lnkFinalDeliveryPoint.Text = facPoint.GetPointForPointId(order.DeliveryPointID).Description;
                lnkFinalDeliveryPoint.Enabled = !order.PlannedForDelivery;
                lblDeliveryDateTime.Text = order.DeliveryDateTime.ToString("dd/MM/yy") + " " + (order.DeliveryIsAnytime ? "AnyTime" : order.DeliveryDateTime.ToString("HH:mm"));
                HtmlGenericControl spnFinalDeliveryPoint = e.Item.FindControl("spnFinalDeliveryPoint") as HtmlGenericControl;
                spnFinalDeliveryPoint.Attributes.Add("onMouseOver", string.Format("ShowPointToolTip(this, {0});", order.DeliveryPointID.ToString()));

                // Can not select an order flagged for removal.
                if (m_removals.IndexOf(orderID) > -1 || (bool)drv["DeliveryCalledIn"])
                    checkboxSelectColumn.Enabled = false;
                else
                    checkboxSelectColumn.Enabled = true;
            }
            else if (e.Item.ItemType == GridItemType.Header)
            {
                // Don't allow select/deselect all for this grid.
                (((e.Item as GridHeaderItem)["checkboxSelectColumn"]).Controls[1] as CheckBox).Visible = false;
            }
        }

        void gvOrders_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            gvOrders.DataSource = OrderHandlingData;
        }

        #endregion

        #region Link Button Events

        void lnkUseEarliestTime_Click(object sender, EventArgs e)
        {
            if (m_instruction.InstructionID < 1)
            {
                DateTime earliest = m_instruction.BookedDateTime;
                bool isAnyTime = m_instruction.IsAnyTime;

                // Get the earliest order booked date time.
                Facade.IOrder facOrder = new Facade.Order();
                foreach (DataRow row in OrderHandlingData.Tables[0].Rows)
                {
                    int orderID = (int)row["OrderID"];
                    Entities.Order order = facOrder.GetForOrderID(orderID);
                    if (order.CollectionRunDeliveryDateTime < earliest)
                    {
                        earliest = order.CollectionRunDeliveryDateTime;
                        isAnyTime = order.CollectionRunDeliveryIsAnytime;
                    }
                }

                SetBookedDateTimeLabel(earliest, isAnyTime);
                m_instruction.BookedDateTime = earliest;
                m_instruction.IsAnyTime = isAnyTime;
                plcSubstitute.Visible = false;
            }
        }
        
        void lnkUseCollection_Click(object sender, EventArgs e)
        {
            int instructionID = 0;
            if (int.TryParse(hidUseInstructionID.Value, out instructionID) && instructionID > 0)
            {
                GenerateCollectionsAndDeliveries();

                Session[wizard.C_REMOVED_ORDERS] = new List<int>();
                Session[wizard.C_INSTRUCTION_INDEX] = null;
                Session[wizard.C_INSTRUCTION] = m_collections.Find(delegate(Entities.Instruction collection) { return collection.InstructionID == instructionID; });

                GoToStep("SGOH");
            }
        }

        #endregion

        #region Repeater Events

        void repValidationMessages_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                OrderHandlingValidationMessage message = e.Item.DataItem as OrderHandlingValidationMessage;

                Image imgIcon = e.Item.FindControl("imgIcon") as Image;
                Literal litText = e.Item.FindControl("litText") as Literal;

                switch (message.Type)
                {
                    case OrderHandlingValidationMessage.eMessageType.Error:
                        imgIcon.ImageUrl = "~/images/ico_critical_small.gif";
                        break;
                    case OrderHandlingValidationMessage.eMessageType.Warning:
                        imgIcon.ImageUrl = "~/images/ico_warning_small.gif";
                        break;
                    case OrderHandlingValidationMessage.eMessageType.Information:
                        imgIcon.ImageUrl = "~/images/ico_info_small.gif";
                        break;
                }
                imgIcon.AlternateText = message.Type.ToString();

                litText.Text = message.Text;
            }
        }

        #endregion

        #region Button Events

        void btnUpdateOrderHandling_Click(object sender, EventArgs e)
        {
            if (Page.IsValid && ValidateOrderHandling())
            {
                // Extract information about the new decisions made.
                eOrderAction orderAction = (eOrderAction)Enum.Parse(typeof(eOrderAction), rdoOrderAction.SelectedValue.Replace(" ", "_"));
                bool arrivalIsAnyTime = false;
                DateTime arrivalDateTime = dteBookedDate.SelectedDate.Value;
                arrivalDateTime = arrivalDateTime.Subtract(arrivalDateTime.TimeOfDay);
                if (dteBookedTime.SelectedDate.HasValue == false)
                    arrivalDateTime = arrivalDateTime.Add(dteBookedTime.SelectedDate.Value.TimeOfDay);
                else
                {
                    arrivalDateTime = arrivalDateTime.Add(new TimeSpan(23, 59, 59));
                    arrivalIsAnyTime = true;
                }
                Entities.Point destinationPoint = ucPoint.SelectedPoint;

                // Update the grid and then rebind.
                GridItemCollection items = gvOrders.SelectedItems;
                foreach (GridDataItem item in items)
                {
                    string orderID = item.OwnerTableView.DataKeyValues[item.ItemIndex]["OrderID"].ToString();
                    DataRow row = OrderHandlingData.Tables[0].Select("OrderID = " + orderID)[0];

                    row["OrderAction"] = orderAction.ToString().Replace("_", " ").Replace("Default", "Deliver");
                    row["DeliveryPointID"] = destinationPoint.PointId;
                    row["DeliveryPointDescription"] = destinationPoint.Description;
                    row["DeliveryDateTime"] = arrivalDateTime;
                    row["DeliveryIsAnyTime"] = arrivalIsAnyTime;

                    row["IsDirty"] = true;
                }

                OrderHandlingData.AcceptChanges();
                gvOrders.Rebind();
            }
        }

        void btnBack_Click(object sender, EventArgs e)
        {
            // If this collection has been called in return to the job details page as the collection information can not be changed.
            if (m_instruction.InstructionActuals == null || m_instruction.InstructionActuals.Count == 0)
                GoToStep("SGO");
            else
            {
                Session[wizard.C_ADDED_ORDERS] = new List<int>();
                Session[wizard.C_REMOVED_ORDERS] = new List<int>();

                GoToStep("JD");
            }
        }

        void btnNext_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("btnNext_Click Start: " + DateTime.Now.ToString("dd/MM/yy HH:mm:ss:fffffff"));
            Facade.IOrder facOrder = new Facade.Order();
            List<int> involvedOrderIDs = new List<int>();

            // By default mark the current instruction as changed (as long as it is not called in).
            if (m_instruction.InstructionActuals == null || m_instruction.InstructionActuals.Count == 0)
                AddToChangedInstructions(m_instruction.InstructionID);

            // Control variables.
            Entities.CollectDrop drop = null;
            Entities.Instruction dropper = null;
            Entities.Order order = null;
            eOrderAction orderAction = eOrderAction.Default;

            // Populate Instruction Collects to aid interogation
            GenerateCollectionsAndDeliveries();

            // Remove any collect drops that have been flagged for removal.
            RemoveOrdersFlaggedForRemoval();

            foreach (DataRow row in OrderHandlingData.Tables[0].Rows)
            {
                int orderID = (int)row["OrderID"];
                involvedOrderIDs.Add(orderID);

                // Only work on added or existing orders which are not being removed and orders which have been marked as dirty.
                if (m_removals.IndexOf(orderID) == -1 && (bool)row["IsDirty"])
                {
                    // Initialise control variables.
                    drop = null;
                    dropper = null;
                    order = facOrder.GetForOrderID(orderID);
                    orderAction = (eOrderAction)Enum.Parse(typeof(eOrderAction), ((string)row["OrderAction"]).Replace("Deliver", "Default").Replace(" ", "_"));

                    // Add new orders to the collection instruction.
                    if (m_additions.IndexOf(orderID) > -1)
                    {
                        m_instruction.CollectDrops.Add(GenerateCollectDropForOrder(order, eOrderAction.Default, m_instruction.InstructionID));
                        AddToChangedInstructions(m_instruction.InstructionID);
                    }

                    // Extract Delivery Information from the row.
                    int deliveryPointID = (int)row["DeliveryPointID"];
                    DateTime deliveryDateTime = (DateTime)row["DeliveryDateTime"];
                    bool deliveryIsAnyTime = (bool)row["DeliveryIsAnyTime"];
                    eInstructionType deliveryInstructionType = orderAction == eOrderAction.Default ? eInstructionType.Drop : eInstructionType.Trunk;

                    // Locate the delivery instruction on this job for this delivery information.
                    dropper = FindInstructionForLocationAndOperation(ref m_deliveries, deliveryPointID, deliveryDateTime, deliveryIsAnyTime, deliveryInstructionType);

                    // Remove this item from it's current drop instruction.
                    RemoveOrderFromInstruction(orderID, m_deliveries.Find(delegate(Entities.Instruction currentDeliveryInstruction) { return currentDeliveryInstruction.CollectDrops.GetForOrderID(orderID) != null; }));

                    if (dropper == null)
                    {
                        // A new drop instruction is required to handle the drop of this order.
                        dropper = GenerateInstruction(deliveryPointID, deliveryDateTime, deliveryIsAnyTime, deliveryInstructionType);
                        dropper.CollectDrops.Add(GenerateCollectDropForOrder(order, orderAction, dropper.InstructionID));

                        // If a drop already exists on the job to handle this order, remove this order from the instruction.
                        Entities.Instruction originalDrop = FindInstructionForOrderID(ref m_deliveries, orderID);
                        if (originalDrop != null)
                            RemoveOrderFromInstruction(orderID, originalDrop);

                        // Add the new dropping instruction to the delivery instructions.
                        m_deliveries.Add(dropper);
                    }
                    else
                    {
                        // Create/Update the order action.
                        drop = dropper.CollectDrops.GetForOrderID(orderID);

                        if (drop != null)
                            drop.OrderAction = orderAction;
                        else
                        {
                            if (dropper.InstructionTypeId == (int)eInstructionType.Trunk && dropper.CollectDrops.Count == 1 && dropper.CollectDrops[0].OrderID == 0)
                            {
                                // This is a normal trunk point that we are now going to hijack to use as a order handling trunk.
                                dropper.CollectDrops[0].OrderID = order.OrderID;
                                dropper.CollectDrops[0].OrderAction = orderAction;
                                dropper.CollectDrops[0].Docket = order.OrderID.ToString();
                                dropper.CollectDrops[0].Weight = order.Weight;
                                dropper.CollectDrops[0].NoCases = 0;
                                dropper.CollectDrops[0].NoPallets = order.NoPallets;
                                dropper.CollectDrops[0].GoodsTypeId = order.GoodsTypeID;
                                dropper.ClientsCustomerIdentityID = dropper.Point.IdentityId;
                            }
                            else
                                dropper.CollectDrops.Add(GenerateCollectDropForOrder(order, orderAction, dropper.InstructionID));
                        }

                        // Mark this instruction as changed.
                        AddToChangedInstructions(dropper.InstructionID);
                    }
                }
            }

            Entities.CustomPrincipal user = Page.User as Entities.CustomPrincipal;

            bool success = false;
            if (m_isUpdate)
            {
                // If this is a new instruction, add it to the collections list.
                if (m_instruction.InstructionID == 0)
                    m_collections.Add(m_instruction);

                // Update this job's instruction.
                Facade.IJob facJob = new Facade.Job();
                System.Diagnostics.Debug.WriteLine("btnNext_Click Make Call: " + DateTime.Now.ToString("dd/MM/yy HH:mm:ss:fffffff"));
                List<Entities.Instruction> amendedInstructions = new List<Entities.Instruction>();
                foreach (Entities.Instruction instruction in m_collections)
                    if (instruction.InstructionID == 0 || m_changedInstructions.Exists(delegate(int changedInstructionID) { return changedInstructionID == instruction.InstructionID; }))
                        amendedInstructions.Add(instruction);
                foreach (Entities.Instruction instruction in m_deliveries)
                    if (instruction.InstructionID == 0 || m_changedInstructions.Exists(delegate(int changedInstructionID) { return changedInstructionID == instruction.InstructionID; }))
                        amendedInstructions.Add(instruction);

                // Obtain the leg alteration mode to apply.
                eLegTimeAlterationMode legAlterationMode = (eLegTimeAlterationMode)Enum.Parse(typeof(eLegTimeAlterationMode), rdoLegTimeAlteringMode.SelectedValue.Replace(" ", "_"));

                Entities.FacadeResult result = facJob.AmendInstructions(m_job, amendedInstructions, legAlterationMode, user.UserName);
                success = result.Success;

                if (success)
                {
                    m_job = facJob.GetJob(m_jobId, true);
                    m_job.Charge = ((Facade.IJobCharge)facJob).GetForJobId(m_jobId);
                    m_job.References = ((Facade.IJobReference)facJob).GetJobReferences(m_jobId);
                }
                else
                {
                    // Display problems with the amend.
                    idErrors.Infringements = result.Infringements;
                    idErrors.DisplayInfringments();
                    idErrors.Visible = true;
                }
            }
            else
            {
                // Add instructions to the instruction collection.
                if (m_isAmendment)
                {
                }
                else
                {
                }
            }

            if (success)
            {
                // Configure Session Variables
                Session[wizard.C_ADDED_ORDERS] = null;
                Session[wizard.C_REMOVED_ORDERS] = null;
                Session[wizard.C_INSTRUCTION_INDEX] = null;
                Session[wizard.C_INSTRUCTION] = null;
                Session[wizard.C_JOB] = m_job;

                GoToStep("JD");
            }
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            GoToStep("CANCEL");
        }

        #endregion

        #endregion

        #region IDefaultButton

        public System.Web.UI.Control DefaultButton
        {
            get { return btnUpdateOrderHandling; }
        }

        #endregion

        /// <summary>
        /// Inner class used to faciliate the construction and display of validation messages.
        /// </summary>
        private class OrderHandlingValidationMessage
        {
            public enum eMessageType { Information, Warning, Error };

            private eMessageType _type = eMessageType.Information;
            private string _text = string.Empty;

            public eMessageType Type
            {
                get { return _type; }
                set { _type = value; }
            }

            public string Text
            {
                get { return _text; }
                set { _text = value; }
            }

            public OrderHandlingValidationMessage(eMessageType type, string text)
            {
                _type = type;
                _text = text;
            }
        }
    }
}