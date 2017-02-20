using System;
using System.Collections.Generic;
using System.Data;
//using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using System.Globalization;
using System.Transactions;
using System.Web.UI.HtmlControls;

using Telerik.Web.UI;

namespace Orchestrator.WebUI.Traffic.JobManagement
{
    /// <summary>
    /// Allows the specification of a redelivery event, i.e. the delivery has been attempted but the driver has been
    /// turned away.  The intended functionality is explained as in Task 12051, please see additional functional requirements.
    /// </summary>
    public partial class OrderBasedRedelivery : Orchestrator.Base.BasePage
    {
        /// <summary>
        /// This page should use hidden field viewstate persistance as this page "spawns" other pages and there
        /// are elements in viewstate which are crucial to the page working correctly.
        /// </summary>
        protected override System.Web.UI.PageStatePersister PageStatePersister
        {
            get
            {
                return new System.Web.UI.HiddenFieldPageStatePersister(this);
            }
        }

        // Enum used to control the actions made.
        protected enum eOrderRedeliveryAction { Delivered, Refused };
        // Enum used to control how the situation should be resolved.
        protected enum eOrderResolutionMethod { Redeliver, DeliverToNewLocation, AttemptLater, NotApplicable, DontKnow };

        // Fields used to extract information from the querystring and viewstate in a consistent manner
        private const string _job_VSK = "job";
        private const string _jobID_QSK = "jobID";
        private const string _instructionID_QSK = "instructionID";
        private const string _redeliveryData_VSK = "redeliveryData";

        #region Page Templates

        private const string privateNoteTemplate = @"PointID Update : Previous PointID - {0} : Point description : {1}";

        #endregion

        #region Properties

        /// <summary>
        /// Returns the Instruction that the redelivery will apply to.
        /// </summary>
        private Entities.Instruction _instruction = null;
        public Entities.Instruction Instruction
        {
            get
            {
                if (_instruction == null)
                {
                    if (Job != null && InstructionID.HasValue)
                    {
                        foreach (Entities.Instruction instruction in Job.Instructions)
                            if (instruction.InstructionID == InstructionID)
                            {
                                _instruction = instruction;
                                return _instruction;
                            }
                    }
                }
                else
                    return _instruction;

                return null;
            }
        }

        /// <summary>
        /// Returns the InstructionID the redelivery is being recorded against.
        /// </summary>
        public int? InstructionID
        {
            get
            {
                int instructionID;
                if (!string.IsNullOrEmpty(Request.QueryString[_instructionID_QSK]) && int.TryParse(Request.QueryString[_instructionID_QSK], out instructionID))
                    return instructionID;

                return null;
            }
        }

        /// <summary>
        /// Returns the Job being used.
        /// </summary>
        /// <remarks>This object is stored in ViewState once loaded.</remarks>
        private Entities.Job _job = null;
        public Entities.Job Job
        {
            get
            {
                if (_job == null)
                {
                    if (ViewState[_job_VSK] != null && (ViewState[_job_VSK] is Entities.Job))
                    {
                        _job = (Entities.Job)ViewState[_job_VSK];
                    }
                    else if (JobID.HasValue)
                    {
                        Facade.Job facJob = new Facade.Job();
                        _job = facJob.GetJob((int)JobID);
                        if (_job != null)
                        {
                            Facade.IInstruction facInstruction = new Facade.Instruction();
                            _job.Instructions = facInstruction.GetForJobId(_job.JobId);
                        }
                        ViewState[_job_VSK] = _job;
                    }
                }

                return _job;
            }
        }

        /// <summary>
        /// Returns the JobID the redelivery is being recorded against.
        /// </summary>
        public int? JobID
        {
            get
            {
                int jobID;
                if (!string.IsNullOrEmpty(Request.QueryString[_jobID_QSK]) && int.TryParse(Request.QueryString[_jobID_QSK], out jobID))
                    return jobID;

                return null;
            }
        }

        public DataSet RedeliveryData
        {
            get
            {
                if (ViewState[_redeliveryData_VSK] == null || !(ViewState[_redeliveryData_VSK] is DataSet))
                    PrepareRedeliveryData();

                return (DataSet)ViewState[_redeliveryData_VSK];
            }
            set { ViewState[_redeliveryData_VSK] = value; }
        }

        #endregion

        #region Page Load/Init

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                PopulateStaticControls();
                ConfigurePage();
            }
            else
            {
                // Configure validators based on the pages control values
                this.rfvClientContact.Enabled = (chkCharging.Checked && this.cboExtraState.Text != "Awaiting Response");
                this.rfvCustomReason.Enabled = (rdOrderAction_Refused.Checked && this.cboRedeliveryReason.Text == "Other");

                // Pull out the point validators
                RequiredFieldValidator crossDockPointValidator = FindValidator<RequiredFieldValidator>("ucCrossDockPoint");
                RequiredFieldValidator newDeliveryPointValidator = FindValidator<RequiredFieldValidator>("ucNewDeliveryPoint");

                if (crossDockPointValidator != null)
                    crossDockPointValidator.Enabled = chkCrossDockGoods.Checked;

                if (newDeliveryPointValidator != null)
                    newDeliveryPointValidator.Enabled = chkDeliverGoodsElsewhere.Checked;
            }
        }

        private T FindValidator<T>(string uniqueName) where T : System.Web.UI.Control, System.Web.UI.IValidator
        {
            T result = default(T);

            foreach (System.Web.UI.Control c in this.Page.Validators)
            {
                if (c.GetType() == typeof(T))
                {
                    if (c.UniqueID.Contains(uniqueName))
                    {
                        result = (T)c;
                        break;
                    }
                }
            }

            return result;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            cfvUpdateOrders.ServerValidate += new ServerValidateEventHandler(cfvUpdateOrders_ServerValidate);

            grdOrders.ItemDataBound += new GridItemEventHandler(grdOrders_ItemDataBound);
            grdOrders.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdOrders_NeedDataSource);

            btnJobDetails.Click += new EventHandler(btnJobDetails_Click);
            btnUpdateSelectedOrders.Click += new EventHandler(btnUpdateSelectedOrders_Click);
            btnSave.Click += new EventHandler(btnSave_Click);
            btnCancel.Click += new EventHandler(btnCancel_Click);

            dlgOrder.DialogCallBack += new EventHandler(dlgOrder_DialogCallBack);
        }

        void dlgOrder_DialogCallBack(object sender, EventArgs e)
        {
            //_instruction = new Facade.Instruction().GetInstruction(InstructionID.Value);
        }

        #endregion

        #region Private Methods

        #region Object Manipulation Methods

        /// <summary>
        /// Locate or create an instruction to perform the redelivery action.
        /// </summary>
        /// <param name="pointID">The point for the instruction.</param>
        /// <param name="instructionType">The action to perform.</param>
        /// <param name="minInstructionOrder">The earliest order the instruction can appear,</param>
        /// <param name="job">The job the instruction will be in.</param>
        /// <param name="alteredInstructions">Altered instructions (new and edited).</param>
        /// <returns>The instruction that meets the desired criteria.</returns>
        private Entities.Instruction LocateInstruction(int pointID, eInstructionType instructionType, int minInstructionOrder, Entities.Job job, List<Entities.Instruction> alteredInstructions)
        {
            Entities.Instruction instruction = null;

            // Find a matching instruction in the job.
            instruction = job.Instructions.Find(
                delegate(Entities.Instruction ins)
                {
                    return ins.PointID == pointID
                            && ins.InstructionTypeId == (int)instructionType
                            && ins.InstructionOrder >= minInstructionOrder
                            && ins.InstructionState < eInstructionState.Completed;
                });

            if (instruction != null)
            {
                // Add the instruction to the altered instruction collection (of it doesn't already exist!).
                if (!alteredInstructions.Exists(
                    delegate(Entities.Instruction ins)
                    {
                        return ins.InstructionID == instruction.InstructionID;
                    }))
                {
                    alteredInstructions.Add(instruction);
                }
            }
            else
            {
                // Find a matching instruction in the altered instruction collection.
                instruction = alteredInstructions.Find(
                    delegate(Entities.Instruction ins)
                    {
                        return ins.PointID == pointID
                                && ins.InstructionTypeId == (int)instructionType
                                && ins.InstructionOrder >= minInstructionOrder
                                && ins.InstructionState < eInstructionState.Completed;
                    });

                if (instruction == null)
                {
                    Facade.IPoint facPoint = new Facade.Point();

                    // A new instruction needs to be configured to manage the action.
                    instruction = new Entities.Instruction();
                    instruction.InstructionTypeId = (int)instructionType;

                    instruction.Point = facPoint.GetPointForPointId(pointID);
                    instruction.PointID = instruction.Point.PointId;

                    if (instructionType == eInstructionType.Drop)
                        instruction.ClientsCustomerIdentityID = instruction.Point.IdentityId;

                    // This instruction needs to happen at the end of the job.
                    if (alteredInstructions.Count > 1)
                    {
                        instruction.BookedDateTime = alteredInstructions[alteredInstructions.Count - 1].BookedDateTime.AddMinutes(15);
                        instruction.InstructionOrder = alteredInstructions[alteredInstructions.Count - 1].InstructionOrder + 1;
                    }
                    else
                    {
                        instruction.BookedDateTime = job.Instructions[job.Instructions.Count - 1].BookedDateTime.AddMinutes(15);
                        instruction.InstructionOrder = job.Instructions[job.Instructions.Count - 1].InstructionOrder + 1;
                    }
                    instruction.IsAnyTime = false;

                    alteredInstructions.Add(instruction);
                }
            }

            return instruction;
        }

        /// <summary>
        /// Takes the instruction's orders and constructs the redelivery data used to control the redelivery
        /// option available to the user.
        /// </summary>
        private void PrepareRedeliveryData()
        {
            DataSet redeliveryData = new DataSet();

            DataTable dt = new DataTable();
            redeliveryData.Tables.Add(dt);

            #region Table Definition

            dt.Columns.Add(new DataColumn("OrderID", typeof(int)));
            dt.Columns.Add(new DataColumn("Client", typeof(string)));
            dt.Columns.Add(new DataColumn("CustomerOrderNumber", typeof(string)));
            dt.Columns.Add(new DataColumn("Weight", typeof(string)));
            dt.Columns.Add(new DataColumn("Pallets", typeof(int)));
            dt.Columns.Add(new DataColumn("LCID", typeof(int)));

            dt.Columns.Add(new DataColumn("OrderAction", typeof(eOrderRedeliveryAction)));
            dt.Columns.Add(new DataColumn("OrderActionLabel", typeof(string)));

            dt.Columns.Add(new DataColumn("RedeliveryReason", typeof(int)));
            dt.Columns.Add(new DataColumn("RedeliveryReasonLabel", typeof(string)));
            dt.Columns.Add(new DataColumn("RedeliveryCustomReason", typeof(string)));
            dt.Columns.Add(new DataColumn("RedeliveryReasonDisplay", typeof(string)));
            dt.Columns.Add(new DataColumn("RedeliveryFromDateTime", typeof(DateTime)));
            dt.Columns.Add(new DataColumn("RedeliveryByDateTime", typeof(DateTime)));
            dt.Columns.Add(new DataColumn("RedeliveryIsAnyTime", typeof(bool)));

            dt.Columns.Add(new DataColumn("ResolutionMethod", typeof(eOrderResolutionMethod)));
            dt.Columns.Add(new DataColumn("ResolutionMethodLabel", typeof(string)));
            dt.Columns.Add(new DataColumn("CrossDockPointID", typeof(int)));
            dt.Columns.Add(new DataColumn("CrossDockPointName", typeof(string)));

            dt.Columns.Add(new DataColumn("RaiseExtra", typeof(bool)));
            dt.Columns.Add(new DataColumn("RaiseExtraLabel", typeof(string)));
            dt.Columns.Add(new DataColumn("ExtraType", typeof(eExtraType)));
            dt.Columns.Add(new DataColumn("ExtraTypeLabel", typeof(string)));
            dt.Columns.Add(new DataColumn("ExtraAmount", typeof(decimal)));
            dt.Columns.Add(new DataColumn("ExtraState", typeof(eExtraState)));
            dt.Columns.Add(new DataColumn("ExtraCustomReason", typeof(string)));
            dt.Columns.Add(new DataColumn("ExtraStateLabel", typeof(string)));
            dt.Columns.Add(new DataColumn("ExtraClientContact", typeof(string)));
            dt.Columns.Add(new DataColumn("DeliveryPointID", typeof(int)));
            dt.Columns.Add(new DataColumn("DeliveryPointName", typeof(string)));
            dt.Columns.Add(new DataColumn("CreateOnwardRun", typeof(bool)));

            #endregion

            Facade.IReferenceData facRefData = new Facade.ReferenceData();
            DataSet dsWeightTypes = facRefData.GetAllWeightTypes();

            Facade.IOrganisation facOrganisation = new Facade.Organisation();
            Dictionary<int, string> clientNames = new Dictionary<int, string>();

            Entities.Organisation client = facOrganisation.GetForIdentityId(Globals.Configuration.IdentityId);
            Facade.IPoint facPoint = new Facade.Point();
            Entities.Point returnPoint = null;

            if (client.Defaults[0].DefaultRedeliveryReturnPointID > 0)
                returnPoint = facPoint.GetPointForPointId(client.Defaults[0].DefaultRedeliveryReturnPointID);

            EF.RedeliveryReason defaultRedelivery = EF.DataContext.Current.RedeliveryReasonSet.Where(rr => rr.RedeliveryReasonId == Orchestrator.Globals.Configuration.RedeliveryReasonId).FirstOrDefault();

            foreach (Entities.CollectDrop cd in Instruction.CollectDrops)
            {
                DataRow dr = dt.NewRow();

                dr["OrderID"] = cd.Order.OrderID;
                if (!clientNames.ContainsKey(cd.Order.CustomerIdentityID))
                    clientNames.Add(cd.Order.CustomerIdentityID, facOrganisation.GetNameForIdentityId(cd.Order.CustomerIdentityID));
                dr["Client"] = clientNames[cd.Order.CustomerIdentityID];
                dr["CustomerOrderNumber"] = cd.Order.CustomerOrderNumber;
                dr["Weight"] = string.Format("{0} {1}",
                    cd.Order.Weight.ToString("N2"),
                    dsWeightTypes.Tables[0].Select("WeightTypeID = " + cd.Order.WeightTypeID)[0]["ShortCode"]);
                dr["Pallets"] = cd.Order.NoPallets;
                dr["LCID"] = cd.Order.LCID;

                dr["OrderAction"] = eOrderRedeliveryAction.Delivered;

                dr["CrossDockPointID"] = 0;
                dr["CrossDockPointName"] = string.Empty;

                dr["OrderActionLabel"] = Utilities.UnCamelCase(Enum.GetName(typeof(eOrderRedeliveryAction), (eOrderRedeliveryAction)dr["OrderAction"]));

                dr["RaiseExtra"] = false;
                dr["RaiseExtraLabel"] = (bool)dr["RaiseExtra"] ? "Yes" : "No";

                dr["RedeliveryReason"] = defaultRedelivery.RedeliveryReasonId;
                dr["RedeliveryReasonLabel"] = defaultRedelivery.Description;

                dr["RedeliveryFromDateTime"] = cd.Order.DeliveryFromDateTime;
                dr["RedeliveryByDateTime"] = cd.Order.DeliveryDateTime;
                dr["RedeliveryIsAnyTime"] = cd.Order.DeliveryIsAnytime;

                dr["ResolutionMethod"] = eOrderResolutionMethod.NotApplicable;
                dr["ResolutionMethodLabel"] = Utilities.UnCamelCase(Enum.GetName(typeof(eOrderResolutionMethod), (eOrderResolutionMethod)dr["ResolutionMethod"]));

                //if (returnPoint != null)
                //{
                //    dr["CrossDockPointID"] = returnPoint.PointId;
                //    dr["CrossDockPointName"] = returnPoint.Description;
                //}
                //else
                //{
                //    // Return the goods to where their were collected from.
                //    Entities.Instruction collectionInstruction = this.Job.Instructions.Find(
                //        delegate(Entities.Instruction ins)
                //        {
                //            return ins.InstructionTypeId == (int)eInstructionType.Load
                //                   && ins.CollectDrops.GetForOrderID(cd.Order.OrderID) != null;
                //        });

                //    dr["CrossDockPointID"] = collectionInstruction.Point.PointId;
                //    dr["CrossDockPointName"] = collectionInstruction.Point.Description;
                //}

                dr["DeliveryPointName"] = Instruction.Point.Description;
                dr["DeliveryPointID"] = Instruction.Point.PointId;
                dr["CreateOnwardRun"] = false;

                dt.Rows.Add(dr);
            }

            RedeliveryData = redeliveryData;
        }

        private eOrderRedeliveryAction SelectedOrderAction()
        {
            eOrderRedeliveryAction result = eOrderRedeliveryAction.Delivered;

            if (rdOrderAction_Refused.Checked)
                result = eOrderRedeliveryAction.Refused;

            return result;
        }

        private eOrderResolutionMethod SelectedResolutionMethod()
        {
            eOrderResolutionMethod result = eOrderResolutionMethod.NotApplicable;
            if (rdResolutionMethod_AttemptLater.Checked)
                result = eOrderResolutionMethod.AttemptLater;
            else if (rdResolutionMethod_DontKnow.Checked)
                result = eOrderResolutionMethod.DontKnow;
            else if (rdResolutionMethod_Redeliver.Checked)
                result = eOrderResolutionMethod.Redeliver;

            return result;
        }

        /// <summary>
        /// Updates the records of the user intentions for each order.
        /// </summary>
        private void UpdateRedeliveryRows(DataRow[] redeliveryRows)
        {
            // Load the intentions.
            eOrderRedeliveryAction redeliveryAction = SelectedOrderAction();

            Facade.IOrganisation facOrganisation = new Facade.Organisation();
            Entities.Organisation client = facOrganisation.GetForIdentityId(Globals.Configuration.IdentityId);
            Facade.IPoint facPoint = new Facade.Point();
            Entities.Point returnPoint = null;
            if (client.Defaults[0].DefaultRedeliveryReturnPointID > 0)
                returnPoint = facPoint.GetPointForPointId(client.Defaults[0].DefaultRedeliveryReturnPointID);

            EF.RedeliveryReason defaultRedelivery = EF.DataContext.Current.RedeliveryReasonSet.Where(rr => rr.RedeliveryReasonId == Orchestrator.Globals.Configuration.RedeliveryReasonId).FirstOrDefault();

            foreach (DataRow redeliveryRow in redeliveryRows)
            {
                // Reset the default behaviour.
                redeliveryRow["OrderAction"] = eOrderRedeliveryAction.Refused;

                redeliveryRow["RedeliveryReason"] = defaultRedelivery.RedeliveryReasonId;
                redeliveryRow["RedeliveryCustomReason"] = string.Empty;
                redeliveryRow["RedeliveryReasonDisplay"] = defaultRedelivery.Description;

                redeliveryRow["ResolutionMethod"] = eOrderResolutionMethod.Redeliver;
                redeliveryRow["CreateOnwardRun"] = false;

                if (returnPoint != null)
                {
                    redeliveryRow["CrossDockPointID"] = returnPoint.PointId;
                    redeliveryRow["CrossDockPointName"] = returnPoint.Description;
                }
                else
                {
                    // Return the goods to where their were collected from.
                    Entities.Instruction collectionInstruction = this.Job.Instructions.Find(
                        delegate(Entities.Instruction ins)
                        {
                            return ins.InstructionTypeId == (int)eInstructionType.Load
                                   && ins.CollectDrops.GetForOrderID((int)redeliveryRow["OrderID"]) != null;
                        });

                    redeliveryRow["CrossDockPointID"] = collectionInstruction.Point.PointId;
                    redeliveryRow["CrossDockPointName"] = collectionInstruction.Point.Description;
                }

                redeliveryRow["RaiseExtra"] = false;
                redeliveryRow["ExtraType"] = eExtraType.TurnedAway;
                redeliveryRow["ExtraAmount"] = default(decimal);
                redeliveryRow["ExtraState"] = eExtraState.AwaitingResponse;

                Facade.IInstruction facIns = new Facade.Instruction();
                Entities.Instruction currentIns = facIns.GetInstruction(this.InstructionID.Value);
                Entities.Point attemptedDelPoint = facPoint.GetPointForPointId(currentIns.PointID);

                redeliveryRow["ExtraCustomReason"] = txtExtraCustomReason.Text = string.Format("Attempted delivery to {0}", attemptedDelPoint.Description);
                redeliveryRow["ExtraClientContact"] = default(string);

                // Update the order redelivery action.
                redeliveryRow["OrderAction"] = redeliveryAction;

                if (redeliveryAction == eOrderRedeliveryAction.Refused)
                {
                    // The order was turned away.
                    int redeliveryReason = int.Parse(cboRedeliveryReason.SelectedValue);
                    string redeliveryReasonText = cboRedeliveryReason.SelectedItem.Text;
                    
                    redeliveryRow["RedeliveryReason"] = redeliveryReason;
                    redeliveryRow["RedeliveryCustomReason"] = txtCustomReason.Text;
                    redeliveryRow["RedeliveryReasonDisplay"] = txtCustomReason.Text.Trim().Length > 0 ? string.Format("{0}, {1}", redeliveryReasonText, txtCustomReason.Text) : redeliveryReasonText;

                    eOrderResolutionMethod resolutionMethod = eOrderResolutionMethod.NotApplicable;
                    resolutionMethod = SelectedResolutionMethod();

                    redeliveryRow["ResolutionMethod"] = resolutionMethod;

                    if (resolutionMethod == eOrderResolutionMethod.Redeliver || resolutionMethod == eOrderResolutionMethod.AttemptLater)
                    {
                        DateTime deliveryFromDateTime = dteDeliveryFromDate.SelectedDate.Value;
                        deliveryFromDateTime = deliveryFromDateTime.Subtract(deliveryFromDateTime.TimeOfDay);

                        if (dteDeliveryFromTime.SelectedDate.HasValue)
                            deliveryFromDateTime = deliveryFromDateTime.Add(new TimeSpan(dteDeliveryFromTime.SelectedDate.Value.TimeOfDay.Hours, dteDeliveryFromTime.SelectedDate.Value.TimeOfDay.Minutes, 0));
                        else
                            deliveryFromDateTime = deliveryFromDateTime.Add(new TimeSpan(0, 0, 0));

                        DateTime deliveryByDateTime = dteDeliveryByDate.SelectedDate.Value;
                        deliveryByDateTime = deliveryByDateTime.Subtract(deliveryByDateTime.TimeOfDay);

                        if (dteDeliveryByTime.SelectedDate.HasValue)
                            deliveryByDateTime = deliveryByDateTime.Add(new TimeSpan(dteDeliveryByTime.SelectedDate.Value.TimeOfDay.Hours, dteDeliveryByTime.SelectedDate.Value.TimeOfDay.Minutes, 0));
                        else
                            deliveryByDateTime = deliveryByDateTime.Add(new TimeSpan(23, 59, 59));

                        if (rdDeliveryIsAnytime.Checked)
                        {
                            deliveryFromDateTime = deliveryByDateTime.Date;
                        }
                        redeliveryRow["RedeliveryFromDateTime"] = deliveryFromDateTime;
                        redeliveryRow["RedeliveryByDateTime"] = deliveryByDateTime;

                        //This is lifed from the Order Entites Propterty - to determine the "Anytime" Flag.
                        redeliveryRow["RedeliveryIsAnyTime"] = ((deliveryFromDateTime.Day == deliveryByDateTime.Day)
                                                                && (deliveryFromDateTime.Month == deliveryByDateTime.Month)
                                                                && (deliveryFromDateTime.Year == deliveryByDateTime.Year)
                                                                && (deliveryFromDateTime.Hour == 0)
                                                                && (deliveryFromDateTime.Minute == 0)
                                                                && (deliveryByDateTime.Hour == 23)
                                                                && (deliveryByDateTime.Minute == 59)) ? true : false;
                    }

                    if (resolutionMethod == eOrderResolutionMethod.AttemptLater)
                    {
                        redeliveryRow["CrossDockPointID"] = 0;
                        redeliveryRow["CrossDockPointName"] = string.Empty;

                        int orderId = (int)redeliveryRow["OrderID"];
                        EF.Point delPoint = (from o in EF.DataContext.Current.OrderSet
                                             where o.OrderId == orderId
                                             select o.DeliveryPoint).FirstOrDefault();

                        redeliveryRow["DeliveryPointID"] = delPoint.PointId;
                        redeliveryRow["DeliveryPointName"] = delPoint.Description;
                    }

                    if (resolutionMethod == eOrderResolutionMethod.Redeliver)
                    {
                        if (chkCrossDockGoods.Checked)
                        {
                            redeliveryRow["CrossDockPointID"] = ucCrossDockPoint.SelectedPoint.PointId;
                            redeliveryRow["CrossDockPointName"] = ucCrossDockPoint.SelectedPoint.Description;
                            redeliveryRow["CreateOnwardRun"] = chkCreateOnwardRun.Checked ? true : false;
                        }
                        else
                        {
                            redeliveryRow["CrossDockPointID"] = 0;
                            redeliveryRow["CrossDockPointName"] = string.Empty;
                        }

                        if (chkDeliverGoodsElsewhere.Checked)
                        {
                            redeliveryRow["DeliveryPointName"] = ucNewDeliveryPoint.SelectedPoint.Description;
                            redeliveryRow["DeliveryPointID"] = ucNewDeliveryPoint.SelectedPoint.PointId;
                        }
                        else
                        {
                            redeliveryRow["DeliveryPointName"] = string.Empty;
                            redeliveryRow["DeliveryPointID"] = 0;
                        }
                    }

                    redeliveryRow["RaiseExtra"] = chkCharging.Checked;

                    if (chkCharging.Checked)
                    {
                        // Record the information pertaining to the extra charge.
                        redeliveryRow["ExtraType"] = Utilities.SelectedEnumValue<eExtraType>(cboExtraType.SelectedValue);
                        if (txtExtraAmount.Value.HasValue)
                            redeliveryRow["ExtraAmount"] = (decimal)((double)txtExtraAmount.Value.Value);
                        eExtraState extraStatus = Utilities.SelectedEnumValue<eExtraState>(cboExtraState.SelectedValue);
                        redeliveryRow["ExtraState"] = extraStatus;
                        if (extraStatus == eExtraState.Accepted)
                            redeliveryRow["ExtraClientContact"] = txtClientContact.Text;
                    }
                }
                else if (redeliveryAction == eOrderRedeliveryAction.Delivered)
                {
                    redeliveryRow["OrderAction"] = eOrderRedeliveryAction.Delivered;
                    redeliveryRow["ResolutionMethod"] = eOrderResolutionMethod.NotApplicable;
                    redeliveryRow["CrossDockPointID"] = 0;
                    redeliveryRow["CrossDockPointName"] = string.Empty;
                    redeliveryRow["DeliveryPointID"] = 0;
                    redeliveryRow["DeliveryPointName"] = string.Empty;
                }

                // Update labels.
                redeliveryRow["OrderActionLabel"] = Utilities.UnCamelCase(Enum.GetName(typeof(eOrderRedeliveryAction), redeliveryAction));
                redeliveryRow["RedeliveryReasonLabel"] = defaultRedelivery.Description;
                redeliveryRow["ResolutionMethodLabel"] = Utilities.UnCamelCase(Enum.GetName(typeof(eOrderResolutionMethod), (eOrderResolutionMethod)redeliveryRow["ResolutionMethod"]));
                redeliveryRow["RaiseExtraLabel"] = (bool)redeliveryRow["RaiseExtra"] ? "Yes" : "No";

                Facade.ExtraType facExtraType = new Orchestrator.Facade.ExtraType();
                Entities.ExtraType et = facExtraType.GetForExtraTypeID(Convert.ToInt32(redeliveryRow["ExtraType"]));
                redeliveryRow["ExtraTypeLabel"] = et.Description;

                redeliveryRow["ExtraStateLabel"] = Utilities.UnCamelCase(Enum.GetName(typeof(eExtraState), (eExtraState)redeliveryRow["ExtraState"]));
            }

            // Cause a rebind.
            grdOrders.Rebind();
        }

        #endregion

        #region Page Configuration Methods

        /// <summary>
        /// Load the particulars of this job onto the page.
        /// </summary>
        private void ConfigurePage()
        {

            if (this.Job != null
                && this.Job.JobType == eJobType.Groupage
                && Instruction != null
                && Instruction.InstructionTypeId == (int)eInstructionType.Drop
                )
            {
                #region Good Parameters

                Facade.IInstructionProgress facInstructionProgress = new Facade.InstructionProgress();

                // A valid instruction has been supplied.
                mv.SetActiveView(vwConfigureRedelivery);

                // Default the arrival and departure date and time based on GPS/actual/planned times.
                DateTime arrivalDateTime = DateTime.MinValue;
                DateTime departureDateTime = DateTime.MinValue;

                // Use the planned datetime.
                arrivalDateTime = Instruction.PlannedArrivalDateTime;
                departureDateTime = Instruction.PlannedArrivalDateTime;

                // Use the actual (call-in) times.
                if (Instruction.InstructionActuals != null && Instruction.InstructionActuals.Count > 0)
                {
                    arrivalDateTime = Instruction.InstructionActuals[0].ArrivalDateTime;
                    departureDateTime = Instruction.InstructionActuals[0].LeaveDateTime;
                }

                // Use the progress information (GPS to be).
                Entities.InstructionProgress progress = facInstructionProgress.Get((int)InstructionID);

                if (progress != null)
                {
                    if (progress.IncomingPenetration != null)
                        arrivalDateTime = (DateTime)progress.IncomingPenetration;
                    if (progress.OutgoingPenetration != null)
                        departureDateTime = (DateTime)progress.OutgoingPenetration;
                }

                //txtArrivalDateTime.SelectedDate = arrivalDateTime;
                //txtDepartureDateTime.SelectedDate = departureDateTime;

                dteDeliveryFromDate.SelectedDate = departureDateTime;
                dteDeliveryFromTime.SelectedDate = departureDateTime.Add(new TimeSpan(8, 0, 0));
                dteDeliveryByDate.SelectedDate = departureDateTime;
                dteDeliveryByTime.SelectedDate = departureDateTime.Add(new TimeSpan(17, 0, 0));

                ucNewDeliveryPoint.SelectedPoint = Instruction.Point;
                #endregion
            }
            else
            {
                #region Bad Parameters

                // Either incomplete information has been supplied, or what has been supplied is not valid for this operation.
                mv.SetActiveView(vwIncorrectDataSupplied);

                // The user can't do anything if no jobid has been supplied.
                btnJobDetails.Visible = JobID.HasValue;

                #endregion
            }
        }

        /// <summary>
        /// Configure the page for default behaviour and bind the various controls with the valid options.
        /// </summary>
        private void PopulateStaticControls()
        {
            DateTime minDate = DateTime.Now.AddYears(-1);
            DateTime maxDate = DateTime.Now.AddYears(1);

            // Configure Min and Max date ranges for the arrival/departure controls.
            //txtArrivalDateTime.MinDate = minDate;
            //txtArrivalDateTime.MaxDate = maxDate;
            //txtDepartureDateTime.MinDate = minDate;
            //txtDepartureDateTime.MaxDate = maxDate;

            if (this.Instruction.InstructionActuals[0] != null)
            {
                this.txtArrivalDateTime.SelectedDate = this.Instruction.InstructionActuals[0].ArrivalDateTime;
                this.txtDepartureDateTime.SelectedDate = this.Instruction.InstructionActuals[0].LeaveDateTime;
            }

            dteDeliveryFromDate.SelectedDate = maxDate;
            dteDeliveryFromTime.SelectedDate = maxDate.Add(new TimeSpan(8, 0, 0));
            dteDeliveryByDate.SelectedDate = maxDate;
            dteDeliveryByTime.SelectedDate = maxDate.Add(new TimeSpan(17, 0, 0));

            // Bind the options for redelivery reasons, delivery options only.
            cboRedeliveryReason.DataSource = EF.DataContext.Current.RedeliveryReasonSet.Where(rr => rr.IsEnabled == true && rr.IsDelivery == true).OrderBy(rr => rr.Description);
            cboRedeliveryReason.DataBind();

            // Bind the resolution options (remove any invalid options).
            List<string> resolutionMethods = new List<string>(Utilities.UnCamelCase(Enum.GetNames(typeof(eOrderResolutionMethod))));
            resolutionMethods.Remove(Utilities.UnCamelCase(Enum.GetName(typeof(eOrderResolutionMethod), eOrderResolutionMethod.NotApplicable)));

            // Load the default return to location point.
            Facade.IPoint facPoint = new Facade.Point();
            Facade.IOrganisation facOrganisation = new Facade.Organisation();
            Entities.Organisation organisation = facOrganisation.GetForIdentityId(Orchestrator.Globals.Configuration.IdentityId);
            if (organisation.Defaults[0].DefaultRedeliveryReturnPointID > 0)
            {
                ucCrossDockPoint.SelectedPoint = facPoint.GetPointForPointId(organisation.Defaults[0].DefaultRedeliveryReturnPointID);
            }

            // Bind the extra types (remove any invalid options).
            Facade.ExtraType facExtraType = new Facade.ExtraType();
            bool? getActiveExtraTypes = true;

            List<Entities.ExtraType> extraTypes = facExtraType.GetForIsEnabled(getActiveExtraTypes);
            extraTypes.RemoveAll(o => (eExtraType)(o.ExtraTypeId) == eExtraType.Custom
                || (eExtraType)(o.ExtraTypeId) == eExtraType.Demurrage
                || (eExtraType)(o.ExtraTypeId) == eExtraType.DemurrageExtra);

            cboExtraType.DataSource = extraTypes;
            cboExtraType.DataValueField = "ExtraTypeId";
            cboExtraType.DataTextField = "Description";
            cboExtraType.DataBind();

            ListItem redeliver =
                cboExtraType.Items.FindByValue(
                    Utilities.UnCamelCase(Enum.GetName(typeof(eExtraType), eExtraType.TurnedAway)));
            if (redeliver != null)
                redeliver.Selected = true;

            // Bind the extra states (remove any invalid options).
            List<string> extraStates = new List<string>(Utilities.UnCamelCase(Enum.GetNames(typeof(eExtraState))));
            extraStates.Remove(Utilities.UnCamelCase(Enum.GetName(typeof(eExtraState), eExtraState.Invoiced)));
            cboExtraState.DataSource = extraStates;
            cboExtraState.DataBind();

            Facade.IInstruction facIns = new Facade.Instruction();
            //Facade.IPoint facPoint = new Facade.Point();

            Entities.Instruction currentIns = facIns.GetInstruction(this.InstructionID.Value);
            Entities.Point attemptedDelPoint = facPoint.GetPointForPointId(currentIns.PointID);

            txtExtraCustomReason.Text = string.Format("Attempted delivery to {0}", attemptedDelPoint.Description);
        }

        #endregion

        #endregion

        #region Event Handlers

        #region Button Event Handlers

        private void btnJobDetails_Click(object sender, EventArgs e)
        {
            // Send the user back to the job details page.
            Response.Redirect(string.Format("../../Job/Job.aspx?jobID={0}", JobID) + "&csid=" + this.CookieSessionID);
        }

        void btnUpdateSelectedOrders_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                StringBuilder orderIDs = new StringBuilder();

                foreach (GridItem gridItem in grdOrders.SelectedItems)
                {
                    if (orderIDs.Length > 0)
                        orderIDs.Append(",");

                    orderIDs.Append(grdOrders.MasterTableView.DataKeyValues[gridItem.ItemIndex]["OrderID"].ToString());
                }

                DataRow[] redeliveryRows = null;
                if (orderIDs.Length > 0)
                {
                    string filter = string.Format("OrderID IN ({0})", orderIDs.ToString());
                    redeliveryRows = RedeliveryData.Tables[0].Select(filter);
                }
                else
                {
                    redeliveryRows = RedeliveryData.Tables[0].Select();
                }

                UpdateRedeliveryRows(redeliveryRows);

                btnSave.Enabled = true;
            }
        }

        void btnUpdateAllOrders_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                DataRow[] redeliveryRows = RedeliveryData.Tables[0].Select();
                UpdateRedeliveryRows(redeliveryRows);
            }
        }

        void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                List<Entities.Instruction> alteredInstructions = new List<Entities.Instruction>();
                alteredInstructions.Add(this.Instruction);

                // Populate the redelivery object.
                Entities.Redelivery redelivery = new Entities.Redelivery();
                redelivery.JobId = this.Job.JobId;
                redelivery.PointId = this.Instruction.PointID;
                redelivery.RedeliveryReason = int.Parse(cboRedeliveryReason.SelectedValue);
                redelivery.CustomReason = string.Empty;
                redelivery.ArrivalDateTime = txtArrivalDateTime.SelectedDate.Value;
                redelivery.DepartureDateTime = txtDepartureDateTime.SelectedDate.Value;
                redelivery.OriginalInstructionID = this.Instruction.InstructionID;
                redelivery.OriginalBookedDateTime = this.Instruction.BookedDateTime;
                redelivery.OriginalIsAnyTime = this.Instruction.IsAnyTime;
                redelivery.Orders = new List<Entities.RedeliveryOrder>();
                redelivery.Extras = new Entities.ExtraCollection();
                this.Instruction.Redelivery = redelivery;


                #region // Create an Instruction Actual for the Redelivery as we might as well call this in now.
                Entities.InstructionActual instructionActual = null;
                Facade.IInstructionActual facIA = new Facade.Instruction();
                instructionActual = facIA.GetEntityForInstructionId((int)InstructionID);
                
                // instruction actual should never be null
                //if (instructionActual == null)
                //{
                //    instructionActual = new Orchestrator.Entities.InstructionActual();
                //    instructionActual.InstructionId = (int)InstructionID;
                //    instructionActual.CollectDropActuals = new Orchestrator.Entities.CollectDropActualCollection();
                //}

                instructionActual.Discrepancies = "";
                instructionActual.DriversNotes = "";

                instructionActual.ArrivalDateTime = txtArrivalDateTime.SelectedDate.Value;
                instructionActual.CollectDropDateTime = txtArrivalDateTime.SelectedDate.Value;
                instructionActual.LeaveDateTime = txtDepartureDateTime.SelectedDate.Value;

                //instructionActual.CollectDropActuals = new Orchestrator.Entities.CollectDropActualCollection();

                //Entities.CollectDropActual cda = null;
                //foreach (Entities.CollectDrop cd in this.Instruction.CollectDrops)
                //{
                //    cda = new Orchestrator.Entities.CollectDropActual();
                //    cda.NumberOfCases = cd.NoCases;
                //    cda.NumberOfPallets = cd.NoPallets;
                //    cda.NumberOfPalletsReturned = cd.NoPallets;
                //    cda.CollectDropId = cd.CollectDropId;
                //    cda.Weight = cd.Weight;
                //    instructionActual.CollectDropActuals.Add(cda);
                //}

                #endregion

                List<Entities.GoodsRefusal> unsavedGoodsRefused = new List<Orchestrator.Entities.GoodsRefusal>();

                // Attach the orders that were turned away and any extras required.
                Facade.IOrder facOrder = new Facade.Order();
                foreach (DataRow row in RedeliveryData.Tables[0].Rows)
                {
                    int orderId = (int)row["OrderID"];
                    eOrderRedeliveryAction orderRedeliveryAction = (eOrderRedeliveryAction)row["OrderAction"];
                    eOrderResolutionMethod orderResolutionMethod = (eOrderResolutionMethod)row["ResolutionMethod"];

                    if (orderRedeliveryAction == eOrderRedeliveryAction.Refused && orderResolutionMethod == eOrderResolutionMethod.DontKnow)
                    {
                        // Create a goods refused record and do nothing else.
                        Entities.GoodsRefusal gr = new Orchestrator.Entities.GoodsRefusal();
                        gr.DateDelivered = this.txtArrivalDateTime.SelectedDate.Value;
                        gr.Docket = orderId.ToString();
                        gr.InstructionId = InstructionID.Value;
                        gr.JobId = Instruction.JobId;

                        EF.Order order = (from o in EF.DataContext.Current.OrderSet.Include("Organisation")
                                          where o.OrderId == orderId
                                          select o).FirstOrDefault();

                        var cdActual = (from i in EF.DataContext.Current.InstructionSet
                                        where i.InstructionId == Instruction.InstructionID
                                        select i.InstructionActuals.FirstOrDefault().CollectDropActual.FirstOrDefault()).FirstOrDefault();

                        gr.IdentityId = order.Organisation.IdentityId;
                        gr.ProductCode = "Unknown - All goods";
                        gr.ProductName = "Unknown - All goods";
                        gr.QuantityOrdered = order.Cases;
                        gr.QuantityRefused = cdActual.NoCases.Value;
                        gr.NoPallets = order.NoPallets;
                        gr.NoPalletSpaces = order.PalletSpaces;
                        gr.RefusalType = (int)eGoodsRefusedType.Other;
                        gr.RefusalStatus = eGoodsRefusedStatus.Outstanding;
                        gr.RefusalLocation = eGoodsRefusedLocation.OnTrailer;
                        gr.OriginalOrderId = orderId;
                        gr.TimeFrame = DateTime.Now.AddDays(Orchestrator.Globals.Configuration.RefusalNoOfDaysToReturnGoods);
                        gr.RefusedFromPointId = this.Instruction.PointID;
                        gr.RefusalNotes = string.Empty;
                        gr.PackSize = string.Empty;

                        unsavedGoodsRefused.Add(gr);
                        // Do not set the store point or return point for the "dont know" scenario.
                        //gr.StorePointId = 0; // (int)row["CrossDockPointID"];
                        //gr.ReturnPointId = 0; // (int)row["DeliveryPointID"]; 

                        continue;
                    }

                    int crossDockPointId = (int)row["CrossDockPointID"];
                    int deliveryPointId = (int)row["DeliveryPointID"];
                    int orderDeliveryPointId = (from o in EF.DataContext.Current.OrderSet
                                                where o.OrderId == orderId
                                                select o.DeliveryPoint.PointId).FirstOrDefault();

                    // If we're cross docking, always create a goods refused so that the goods
                    // can be planned onwards from the refusal report screen and so that the 
                    // refusal filters on the collections/deliveries screen also pick up the order.
                    if (
                        orderRedeliveryAction == eOrderRedeliveryAction.Refused
                        &&
                        crossDockPointId > 0
                        )
                    {
                        Entities.GoodsRefusal gr = new Orchestrator.Entities.GoodsRefusal();
                        gr.DateDelivered = this.txtArrivalDateTime.SelectedDate.Value;
                        gr.Docket = orderId.ToString();
                        gr.InstructionId = InstructionID.Value;
                        gr.JobId = Instruction.JobId;

                        EF.Order order = (from o in EF.DataContext.Current.OrderSet.Include("Organisation")
                                          where o.OrderId == orderId
                                          select o).FirstOrDefault();

                        var cdActual = (from i in EF.DataContext.Current.InstructionSet
                                        where i.InstructionId == Instruction.InstructionID
                                        select i.InstructionActuals.FirstOrDefault().CollectDropActual.FirstOrDefault()).FirstOrDefault();

                        gr.IdentityId = order.Organisation.IdentityId;
                        gr.ProductCode = "Unknown - All goods";
                        gr.ProductName = "Unknown - All goods";
                        gr.QuantityOrdered = order.Cases;
                        gr.QuantityRefused = cdActual.NoCases.Value;
                        gr.NoPallets = order.NoPallets;
                        gr.NoPalletSpaces = order.PalletSpaces;
                        gr.RefusalType = (int)eGoodsRefusedType.Other;
                        gr.RefusalStatus = eGoodsRefusedStatus.Outstanding;
                        gr.RefusalLocation = eGoodsRefusedLocation.OnTrailer;
                        gr.OriginalOrderId = orderId;
                        gr.NewOrderId = orderId;
                        gr.TimeFrame = DateTime.Now.AddDays(Orchestrator.Globals.Configuration.RefusalNoOfDaysToReturnGoods); 
                        gr.RefusedFromPointId = this.Instruction.PointID;
                        gr.RefusalNotes = string.Empty;
                        gr.PackSize = string.Empty;

                        unsavedGoodsRefused.Add(gr);

                        gr.StorePointId = crossDockPointId;
                        if (deliveryPointId > 0)
                            gr.ReturnPointId = deliveryPointId;
                        else
                            gr.ReturnPointId = orderDeliveryPointId;
                    }

                    Entities.RedeliveryOrder redeliveryOrder = new Entities.RedeliveryOrder();

                    #region //eOrderRedeliveryAction.Refused
                    if (orderRedeliveryAction == eOrderRedeliveryAction.Refused)
                    {
                        // Record that this order was turned away.
                        redeliveryOrder.OrderID = (int)row["OrderID"];
                        redeliveryOrder.RedeliveryReason = (int)row["RedeliveryReason"];
                        redeliveryOrder.CustomReason = string.Empty;

                        if(row["RedeliveryCustomReason"] != DBNull.Value)
                            redeliveryOrder.CustomReason = (string)row["RedeliveryCustomReason"];

                        // Build the instruction to resolve the turn away.
                        Entities.Instruction instruction = null;

                        if (crossDockPointId > 0)
                            redeliveryOrder.CreateOnwardRun = (bool)row["CreateOnwardRun"];

                        //----------------------------------------------
                        // 5 scenarios:
                        //----------------------------------------------
                        // 1 - Store 
                        // 2 - Store and deliver to new location
                        // 3 - Store and deliver to original location
                        // 4 - Deliver to new location
                        // 5 - Deliver to original location
                        //----------------------------------------------
                        switch (orderResolutionMethod)
                        {
                            case eOrderResolutionMethod.AttemptLater:
                                // 5 - Deliver to original location
                                instruction = LocateInstruction(this.Instruction.PointID, eInstructionType.Drop, this.Instruction.InstructionOrder + 1, this.Job, alteredInstructions);
                                redeliveryOrder.NewDeliveryFromDateTime = (DateTime)row["RedeliveryFromDateTime"];
                                redeliveryOrder.NewDeliveryByDateTime = (DateTime)row["RedeliveryByDateTime"];
                                redeliveryOrder.NewDeliveryIsAnyTime = (bool)row["RedeliveryIsAnyTime"];

                                instruction.PlannedArrivalDateTime = redeliveryOrder.NewDeliveryByDateTime;
                                instruction.BookedDateTime = redeliveryOrder.NewDeliveryByDateTime;
                                break;

                            case eOrderResolutionMethod.Redeliver:
                                // 4 - Deliver to new location
                                if (crossDockPointId == 0 && deliveryPointId > 0)
                                {
                                    instruction = LocateInstruction(deliveryPointId, eInstructionType.Drop, this.Instruction.InstructionOrder + 1, this.Job, alteredInstructions);
                                    redeliveryOrder.IsDeliverToLocation = true;
                                    redeliveryOrder.NewDeliveryFromDateTime = (DateTime)row["RedeliveryFromDateTime"];
                                    redeliveryOrder.NewDeliveryByDateTime = (DateTime)row["RedeliveryByDateTime"];
                                    redeliveryOrder.NewDeliveryIsAnyTime = (bool)row["RedeliveryIsAnyTime"];

                                    instruction.PlannedArrivalDateTime = redeliveryOrder.NewDeliveryByDateTime;
                                    instruction.BookedDateTime = redeliveryOrder.NewDeliveryByDateTime;
                                    break;
                                }

                                // 2 - Store and deliver to new location
                                // 3 - Store and deliver to original location
                                if (crossDockPointId > 0 && deliveryPointId > 0)
                                {
                                    instruction = LocateInstruction(crossDockPointId, eInstructionType.Trunk, this.Instruction.InstructionOrder + 1, this.Job, alteredInstructions);
                                    redeliveryOrder.IsReturnToLocation = true;
                                    redeliveryOrder.NewDeliveryFromDateTime = (DateTime)row["RedeliveryFromDateTime"];
                                    redeliveryOrder.NewDeliveryByDateTime = (DateTime)row["RedeliveryByDateTime"];
                                    redeliveryOrder.NewDeliveryIsAnyTime = (bool)row["RedeliveryIsAnyTime"];

                                    instruction.PlannedArrivalDateTime = redeliveryOrder.NewDeliveryByDateTime;
                                    instruction.BookedDateTime = redeliveryOrder.NewDeliveryByDateTime;

                                    if (deliveryPointId != instruction.PointID)
                                    {
                                        // we are going to be delivering the order to a new location
                                        redeliveryOrder.DeliveryPointID = deliveryPointId;
                                    }
                                    break;
                                }

                                // 1 - Store 
                                if (crossDockPointId > 0 && deliveryPointId == 0)
                                {
                                    instruction = LocateInstruction(crossDockPointId, eInstructionType.Trunk, this.Instruction.InstructionOrder + 1, this.Job, alteredInstructions);
                                    redeliveryOrder.IsReturnToLocation = true;
                                    redeliveryOrder.NewDeliveryFromDateTime = (DateTime)row["RedeliveryFromDateTime"];
                                    redeliveryOrder.NewDeliveryByDateTime = (DateTime)row["RedeliveryByDateTime"];
                                    redeliveryOrder.NewDeliveryIsAnyTime = (bool)row["RedeliveryIsAnyTime"];

                                    instruction.PlannedArrivalDateTime = redeliveryOrder.NewDeliveryByDateTime;
                                    instruction.BookedDateTime = redeliveryOrder.NewDeliveryByDateTime;
                                    break;
                                }
                                break;
                            default: break;
                        }

                        //Add Order to list of Orders Turned Away.
                        redelivery.Orders.Add(redeliveryOrder);

                        // Add the order to this instruction.
                        Entities.CollectDrop cd = new Entities.CollectDrop();
                        Entities.Order order = facOrder.GetForOrderID(redeliveryOrder.OrderID);
                        cd.NoPallets = order.NoPallets;
                        cd.NoCases = order.Cases;
                        cd.GoodsTypeId = order.GoodsTypeID;
                        cd.OrderID = order.OrderID;
                        cd.Weight = order.Weight;
                        cd.ClientsCustomerReference = order.DeliveryOrderNumber;
                        cd.Docket = order.OrderID.ToString();
                        cd.OrderAction = instruction.InstructionTypeId == (int)eInstructionType.Drop ? eOrderAction.Default : eOrderAction.Cross_Dock;
                        cd.Order = order;
                        instruction.CollectDrops.Add(cd);

                        // Configure the rest of the redelivery order entry.
                        //redeliveryOrder.OriginalDeliveryDateTime = order.DeliveryDateTime;
                        redeliveryOrder.OriginalDeliveryFromDateTime = order.DeliveryFromDateTime;
                        redeliveryOrder.OriginalDeliveryByDateTime = order.DeliveryDateTime;
                        redeliveryOrder.OriginalDeliveryIsAnyTime = order.DeliveryIsAnytime;
                        redeliveryOrder.Order = order;
                        redeliveryOrder.NewInstruction = instruction;

                        if (redeliveryOrder.IsDeliverToLocation)
                        {
                            //Update order private notes with old PointID / Description of delivery point. Then update order with new delivery pointID.
                            Facade.IPoint facPoint = new Facade.Point();
                            Entities.Point deliveryPoint = facPoint.GetPointForPointId(order.DeliveryPointID);

                            redeliveryOrder.NewConfidentialComments = string.Format("{0} {1}", redeliveryOrder.Order.ConfidentialComments, string.Format(privateNoteTemplate, order.DeliveryPointID, deliveryPoint.Description));
                            redeliveryOrder.DeliveryPointID = redeliveryOrder.NewInstruction.PointID;
                        }

                        // Remove the order from the instruction.
                        for (int cdIndex = 0; cdIndex < this.Instruction.CollectDrops.Count; cdIndex++)
                            if (this.Instruction.CollectDrops[cdIndex].OrderID == redeliveryOrder.OrderID)
                            {
                                this.Instruction.CollectDrops.RemoveAt(cdIndex);
                                break;
                            }

                    }
                    #endregion

                    bool raiseExtra = (bool)row["RaiseExtra"];

                    #region //raiseExtra
                    if (raiseExtra)
                    {
                        // Build an extra to cover the redelivery.
                        Entities.Extra extra = new Entities.Extra();
                        extra.OrderID = (int)row["OrderID"];
                        extra.ExtraType = (eExtraType)row["ExtraType"];
                        extra.ForeignAmount = (decimal)row["ExtraAmount"];
                        extra.ExchangeRateID = redeliveryOrder.Order.ExchangeRateID;
                        extra.ExtraState = (eExtraState)row["ExtraState"];
                        extra.CustomDescription = (string)row["ExtraCustomReason"];

                        if (row["ExtraClientContact"] != DBNull.Value && ((string)row["ExtraClientContact"]).Length > 0)
                            extra.ClientContact = (string)row["ExtraClientContact"];

                        if (extra.ExchangeRateID != null && Globals.Configuration.MultiCurrency)
                        {
                            Facade.IExchangeRates facER = new Facade.ExchangeRates();
                            extra.ExtraAmount = facER.GetConvertedRate((int)extra.ExchangeRateID, extra.ForeignAmount);
                        }
                        else
                            extra.ExtraAmount = extra.ForeignAmount;

                        redelivery.Extras.Add(extra);
                    }
                    #endregion
                }

                if (redelivery.Orders.Count > 0)
                {
                    Facade.IRedelivery facRedelivery = new Facade.Redelivery();
                    Entities.FacadeResult result = null;

                    try
                    {
                        redelivery.UnsavedGoodsRefused = unsavedGoodsRefused;
                        result = facRedelivery.Create(redelivery, alteredInstructions, this.Job, instructionActual, Page.User.Identity.Name); ;
                    }
                    catch (Exception ex)
                    {
                        result.Success = false;
                    }

                    if (result.Success)
                        Response.Redirect(string.Format("/job/job.aspx?jobID={0}", JobID) + "&csid=" + this.CookieSessionID);
                    else
                    {
                        idErrors.Infringements = result.Infringements;
                        idErrors.DisplayInfringments();
                        idErrors.Visible = true;
                    }
                }
                else
                {
                    if (unsavedGoodsRefused.Count > 0)
                    {
                        Facade.GoodsRefusal facGoodsRefused = new Orchestrator.Facade.GoodsRefusal();

                        using (TransactionScope ts = new TransactionScope())
                        {
                            bool result = false;
                            foreach (Entities.GoodsRefusal gr in unsavedGoodsRefused)
                            {
                                result = facGoodsRefused.Create(gr, eJobType.Groupage, InstructionID.Value, Page.User.Identity.Name);
                                if (!result)
                                {
                                    break;
                                }
                            }

                            if (result)
                            {
                                ts.Complete();
                                Response.Redirect(string.Format("/job/job.aspx?jobID={0}", JobID) + "&csid=" + this.CookieSessionID);
                            }
                            else
                            {
                                feedbackInstructions.Message = "The goods refused record could not be created.";
                                feedbackInstructions.Visible = true;
                            }
                        }
                    }
                }
            }
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            if (Orchestrator.Globals.Configuration.UseJobManagementLink)
                Response.Redirect(string.Format("/traffic/jobmanagement.aspx?jobID={0}", JobID) + "&csid=" + this.CookieSessionID);
            else
                Response.Redirect(string.Format("/job/job.aspx?jobID={0}", JobID) + "&csid=" + this.CookieSessionID);
        }

        #endregion

        #region Grid Event Handlers

        void grdOrders_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                Label lblReturnToLocationDescription = (Label)e.Item.FindControl("lblReturnToLocationDescription");
                Label lblDeliverToDescription = (Label)e.Item.FindControl("lblDeliverToDescription");
                Label lblRedeliveryReason = (Label)e.Item.FindControl("lblRedeliveryReason");
                Label lblExtraRate = (Label)e.Item.FindControl("lblExtraRate");
                DataRowView drv = (DataRowView)e.Item.DataItem;

                HtmlAnchor hypOrderId = (HtmlAnchor)e.Item.FindControl("hypOrderId");
                string queryString = string.Format("oid={0}", drv["OrderID"]);
                hypOrderId.HRef = string.Format("javascript:{0}", this.dlgOrder.GetOpenDialogScript(queryString));

                lblReturnToLocationDescription.Visible = false;
                lblRedeliveryReason.Visible = false;
                lblExtraRate.Visible = false;

                eOrderResolutionMethod orderResolutionMethod = (eOrderResolutionMethod)drv["ResolutionMethod"];
                if (orderResolutionMethod == eOrderResolutionMethod.Redeliver || orderResolutionMethod == eOrderResolutionMethod.DeliverToNewLocation)
                {
                    lblReturnToLocationDescription.Text = string.Format(" - ({0})", (string)drv["CrossDockPointName"]);
                    lblReturnToLocationDescription.Attributes.Add("onMouseOver", "javascript:ShowPointToolTip(this," + (int)drv["CrossDockPointID"] + ");");
                    lblReturnToLocationDescription.Attributes.Add("onmouseout", "javascript:closeToolTip();");
                    lblReturnToLocationDescription.Visible = true;

                    lblDeliverToDescription.Text = (string)drv["DeliveryPointName"];
                    lblDeliverToDescription.Visible = true;
                }

                eOrderRedeliveryAction orderRedeliveryAction = (eOrderRedeliveryAction)drv["OrderAction"];
                if (orderRedeliveryAction == eOrderRedeliveryAction.Refused)
                {
                    lblRedeliveryReason.Text = (string)drv["RedeliveryReasonDisplay"];
                    lblRedeliveryReason.Visible = true;
                }

                bool raiseExtra = (bool)drv["RaiseExtra"];
                if (raiseExtra)
                {
                    int lcID = (int)drv["LCID"];
                    CultureInfo culture = new CultureInfo(lcID);

                    lblExtraRate.Text = string.Format(" - ({0})", ((decimal)drv["ExtraAmount"]).ToString("C", culture));
                    lblExtraRate.Visible = true;
                }
            }
        }

        void grdOrders_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            grdOrders.DataSource = RedeliveryData;
        }

        #endregion

        #region Validation Event Handlers

        //void cvArrivalDepartureDateTime_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        //{
        //    // The arrival date time must be before the departure date time.
        //    args.IsValid = txtArrivalDateTime.SelectedDate <= txtDepartureDateTime.SelectedDate;
        //}

        void cfvUpdateOrders_ServerValidate(object source, ServerValidateEventArgs args)
        {
            StringBuilder lcIDs = new StringBuilder();
            CustomValidator cfvUpdateOrders = source as CustomValidator;

            if (grdOrders.SelectedItems.Count == 0)
            {
                foreach (GridItem rgi in grdOrders.MasterTableView.Items)
                    rgi.Selected = true;
            }

            if (grdOrders.SelectedItems.Count > 0)
            {
                if (chkCharging.Checked) //Only validate if an extra is to be added.
                {
                    foreach (GridItem gridItem in grdOrders.SelectedItems)
                        if (!lcIDs.ToString().Contains(grdOrders.MasterTableView.DataKeyValues[gridItem.ItemIndex]["LCID"].ToString()))
                        {
                            if (lcIDs.Length > 0)
                                lcIDs.Append(",");

                            lcIDs.Append(grdOrders.MasterTableView.DataKeyValues[gridItem.ItemIndex]["LCID"].ToString());
                        }

                    string[] cultures = lcIDs.ToString().Split(',');
                    args.IsValid = cultures.Length < 2; //If more than one culture is present - don't process.

                    if (cfvUpdateOrders != null)
                        cfvUpdateOrders.Text = "Orders with different currency types have been selected.";
                }
            }
            else
            {
                args.IsValid = false;

                if (cfvUpdateOrders != null)
                    cfvUpdateOrders.Text = "Please select at least 1 order to update.";
            }
        }

        #endregion

        #endregion
    }
}
