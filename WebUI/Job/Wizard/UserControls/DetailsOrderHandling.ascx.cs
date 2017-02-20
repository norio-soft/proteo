using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.Caching;

using Orchestrator;

namespace Orchestrator.WebUI.Job.Wizard.UserControls
{
    public delegate void AfterMoveMergeCommand();

    public partial class DetailsOrderHandling : System.Web.UI.UserControl
    {
        protected class CollectDropWithCollectDropActual
        {
            public Entities.CollectDrop CollectDrop { get; set; }
            public Entities.CollectDropActual CollectDropActual { get; set; }
        }

        protected class CollectDropsWithActuals
        {
            public Entities.CollectDrop CollectDrop { get; set; }
            public eInstructionType InstructionType { get; set; }
            public int NumberOfPallets { get; set; }
            public string DehiringReceipt { get; set; }
        }

        public event AfterMoveMergeCommand OnAfterMoveMergeCommand;
        public event EventHandler Refresh;

        private int m_jobId;
        private Entities.Job m_job;
        private bool m_canEdit = false;
        private int m_cumulativePalletCount = 0;
        private List<int> _orderedInstructionIdList;
        private Dictionary<int, Entities.Instruction> _instructionTrunk;
        private Dictionary<int, Entities.Instruction> _instructionPallet;
        string addOrderurl = string.Empty;
        string convertInstructionURL = String.Empty;

        public Dictionary<int, Entities.Instruction> InstructionTrunk
        {
            get { return _instructionTrunk; }
            set { _instructionTrunk = value; }
        }

        public Entities.Job Job
        {
            get
            {
                if (m_job != null)
                {
                    return m_job;
                }
                else
                {
                    return GetJobEntityFromCache();
                }
            }
            set { m_job = value; }
        }

        public DetailsOrderHandling() { }

        protected void Page_Init(object sender, EventArgs e)
        {
            addOrderurl = dlgAddOrder.ClientID + "_Open('iid={0}&io={1}&pid={2}&it={3}&jid={4}')";
            convertInstructionURL = dlgConvertInstruction.ClientID + "_Open('iid={0}')";
            repOH.ItemCommand += new RepeaterCommandEventHandler(repOrderHandling_ItemCommand);
            repOH.ItemDataBound += new RepeaterItemEventHandler(repOrderHandling_ItemDataBound);
            this.dlgOrder.DialogCallBack += new EventHandler(dlgOrder_DialogCallBack);
            this.dlgConvertInstruction.DialogCallBack += new EventHandler(dlgConvertInstruction_DialogCallBack);
        }

        void dlgConvertInstruction_DialogCallBack(object sender, EventArgs e)
        {
            this.Refresh(sender, e);
        }

        void dlgOrder_DialogCallBack(object sender, EventArgs e)
        {
            this.Refresh(this.dlgOrder, new EventArgs());
        }

        protected void Page_Load(object sender, System.EventArgs e)
        {
            m_jobId = Convert.ToInt32(Request.QueryString["jobId"]);
            Job = GetJobEntityFromCache();
            linkJobStyles.Attributes.Add("href", Orchestrator.Globals.Configuration.WebServer + "/style/JobDetail.css");
            m_canEdit = Orchestrator.WebUI.Security.Authorise.CanAccess(eSystemPortion.AddEditJob);
            PopulateJobControls();

            // Check to see if the user has requested to remove an order
            string target = Request.Params["__EVENTTARGET"];
            string args = Request.Params["__EVENTARGUMENT"];

            int OrderID = 0;

            if (!string.IsNullOrEmpty(args))
            {
                if (args.ToLower() != "refresh")
                    OrderID = Convert.ToInt32(args);

                if (target.ToLower() == "removeorder")
                {
                    if (OrderID > 0)
                    {
                        this.RemoveOrder(OrderID);
                    }
                }
            }
        }

        protected void RemoveOrder(int orderId)
        {
            if (orderId > 0)
            {
                if (m_jobId > 0)
                {
                    Orchestrator.Facade.IJob facJob = new Orchestrator.Facade.Job();
                    Entities.FacadeResult result = facJob.RemoveOrder(m_jobId, orderId, ((Orchestrator.Entities.CustomPrincipal)Page.User).UserName.ToString());
                    if (result.Success == true)
                    {
                        // have amended to do a full refresh otherwise the legs on the Job screen do not get refreshed when you remove an order.
                        this.Page.ClientScript.RegisterStartupScript(this.Page.GetType(), "OnLoad", "window.RefreshPage(); ", true);

                        //this.RefreshJobEntityCache();
                        //PopulateJobControls();
                    }
                }
            }
        }


        #region Display Of Istruction Booked Times
        
        protected string GetInstructionBookedDateTime( int instructionID, int InstructionTypeID )
        {
            string retVal = string.Empty;

            if( InstructionTypeID == (int)eInstructionType.Load )
                retVal = GetLoadTimeForInstruction(instructionID);
            else if( InstructionTypeID == (int)eInstructionType.Drop )
                retVal = GetDeliveryTimeForInstruction(instructionID);

            return retVal;
        }

        private string GetLoadTimeForInstruction( int instructionID )
        {
            DateTime earliestLoadDateTime = DateTime.MaxValue;
            DateTime latestLoadDateTime = DateTime.MaxValue;

            DateTime CollectFromDateTime;
            DateTime CollectToDateTime;
            bool IsTimedBooking = false;

            var CollectDrops = this.Job.Instructions.GetForInstructionId( instructionID ).CollectDrops;

            foreach( Entities.CollectDrop cd in CollectDrops )
            {

                int rowCount = 0;

                CollectFromDateTime = cd.Order.CollectionDateTime;
                CollectToDateTime = cd.Order.CollectionByDateTime;

                if( rowCount == 0 )
                {
                    earliestLoadDateTime = CollectFromDateTime;
                    latestLoadDateTime = CollectToDateTime;
                    if( earliestLoadDateTime == latestLoadDateTime )
                        IsTimedBooking = true;
                }
                else if( CollectFromDateTime == CollectToDateTime )
                {
                    // Timed booking [check to ensure we encapsulate this]
                    if( CollectFromDateTime < earliestLoadDateTime )
                        earliestLoadDateTime = CollectFromDateTime;
                    if( CollectToDateTime > latestLoadDateTime )
                        latestLoadDateTime = CollectToDateTime;
                }
                else
                {
                    if( CollectFromDateTime.Date == CollectToDateTime.Date && CollectFromDateTime.TimeOfDay.Hours == 0 && CollectToDateTime.TimeOfDay.Hours == 23 )
                    {
                        // Any time 
                    }
                    else
                    {
                        if( !IsTimedBooking )
                        {
                            // Look for the most restrictive
                            if( CollectFromDateTime < earliestLoadDateTime )
                                earliestLoadDateTime = CollectFromDateTime;
                            if( CollectToDateTime < latestLoadDateTime )
                                latestLoadDateTime = CollectToDateTime;
                        }
                    }
                }

                rowCount++;
            }

            string retVal = string.Empty;

            if( earliestLoadDateTime.Date == latestLoadDateTime.Date && earliestLoadDateTime.TimeOfDay == new TimeSpan( 0, 0, 0 ) && latestLoadDateTime.TimeOfDay.Hours == 23 && latestLoadDateTime.Minute == 59 )
            {
                // Any Time
                retVal = earliestLoadDateTime.ToString( "dd/MM/yy" ) + " Anytime";
            }
            else if( earliestLoadDateTime.Date == latestLoadDateTime.Date && earliestLoadDateTime.TimeOfDay == latestLoadDateTime.TimeOfDay )
            {
                // time booking
                retVal = earliestLoadDateTime.ToString( "dd/MM/yy HH:mm" );
            }
            else
            {
                // Booking Window
                retVal = string.Format( "{0} to {1}", earliestLoadDateTime.ToString( "dd/MM HH:mm" ), latestLoadDateTime.ToString( "dd/MM HH:mm" ) );
            }
            return retVal;
        }

        private string GetDeliveryTimeForInstruction( int instructionID )
        {
            DateTime earliestDeliveryDateTime = DateTime.MaxValue;
            DateTime latestDeliveryDateTime = DateTime.MaxValue;

            DateTime DeliverFromDateTime;
            DateTime DeliverToDateTime;
            bool IsTimedBooking = false;

            var CollectDrops = this.Job.Instructions.GetForInstructionId( instructionID ).CollectDrops;

            foreach( Entities.CollectDrop cd in CollectDrops )
            {

                int rowCount = 0;

                DeliverFromDateTime = cd.Order.DeliveryFromDateTime;
                DeliverToDateTime = cd.Order.DeliveryDateTime;

                if( rowCount == 0 )
                {
                    earliestDeliveryDateTime = DeliverFromDateTime;
                    latestDeliveryDateTime = DeliverToDateTime;
                    if( DeliverFromDateTime == DeliverToDateTime )
                        IsTimedBooking = true;
                }
                else if( DeliverFromDateTime == DeliverToDateTime )
                {
                    // Timed booking [check to ensure we encapsulate this]
                    if( DeliverFromDateTime < earliestDeliveryDateTime )
                        earliestDeliveryDateTime = DeliverFromDateTime;
                    if( DeliverToDateTime > latestDeliveryDateTime )
                        latestDeliveryDateTime = DeliverToDateTime;
                }
                else
                {
                    if( DeliverFromDateTime.Date == DeliverToDateTime.Date && DeliverFromDateTime.TimeOfDay.Hours == 0 && DeliverToDateTime.TimeOfDay.Hours == 23 )
                    {
                        // Any time 
                    }
                    else
                    {
                        if( !IsTimedBooking ) // if there is a timed booking on here dont worry about the window
                        {
                            // Look for the most restrictive
                            if( DeliverFromDateTime < earliestDeliveryDateTime )
                                earliestDeliveryDateTime = DeliverFromDateTime;
                            if( DeliverToDateTime < latestDeliveryDateTime )
                                latestDeliveryDateTime = DeliverToDateTime;
                        }
                    }
                }
                rowCount++;
            }

            string retVal = string.Empty;

            if( earliestDeliveryDateTime.Date == latestDeliveryDateTime.Date && earliestDeliveryDateTime.TimeOfDay == new TimeSpan( 0, 0, 0 ) && latestDeliveryDateTime.TimeOfDay.Hours == 23 && latestDeliveryDateTime.TimeOfDay.Minutes == 59 )
            {
                // Any Time
                retVal = earliestDeliveryDateTime.ToString( "dd/MM/yy" ) + " Anytime";
            }
            else if( earliestDeliveryDateTime.Date == latestDeliveryDateTime.Date && earliestDeliveryDateTime.TimeOfDay == latestDeliveryDateTime.TimeOfDay )
            {
                // time booking
                retVal = earliestDeliveryDateTime.ToString( "dd/MM/yy HH:mm" );
            }
            else
            {
                // Booking Window
                retVal = string.Format( "{0} to {1}", earliestDeliveryDateTime.ToString( "dd/MM HH:mm" ), latestDeliveryDateTime.ToString( "dd/MM HH:mm" ) );
            }
            return retVal;
        }

        #endregion

        /// <summary>
        /// Populates the various parts of the pages with the relevant job data.
        /// </summary>
        private void PopulateJobControls()
        {
            Entities.InstructionCollection collections = new Entities.InstructionCollection();
            Entities.InstructionCollection deliveries = new Entities.InstructionCollection();
            Entities.InstructionCollection orderHandlingInstructions = new Entities.InstructionCollection();

            if (_orderedInstructionIdList == null)
                _orderedInstructionIdList = new List<int>();

            if (_instructionTrunk == null)
                _instructionTrunk = new Dictionary<int, Entities.Instruction>();

            if (_instructionPallet == null)
                _instructionPallet = new Dictionary<int, Entities.Instruction>();

            _orderedInstructionIdList.Clear();
            _instructionTrunk.Clear();
            _instructionPallet.Clear();

            int previousInstructionId = 0;

            // Populate from the legs collection
            foreach (Entities.Instruction instruction in Job.Instructions)
            {
                switch ((eInstructionType)instruction.InstructionTypeId)
                {
                    case eInstructionType.Load:
                    case eInstructionType.Drop:
                    case eInstructionType.AttemptedDelivery:
                        orderHandlingInstructions.Add(instruction);
                        _orderedInstructionIdList.Add(instruction.InstructionID);
                        break;
                    case eInstructionType.Trunk:
                        if (Job.JobType == eJobType.Groupage && instruction.CollectDrops.Count > 0 && instruction.CollectDrops[0].OrderID > 0)
                            orderHandlingInstructions.Add(instruction);
                        else
                        {
                            // This dictionary collection is used to identify leg trunk, not cross docks, tranship or leave on trailer.
                            orderHandlingInstructions.Add(instruction);
                            _instructionTrunk.Add(previousInstructionId, instruction);
                        }
                        break;
                    case eInstructionType.PickupPallets:
                    case eInstructionType.LeavePallets:
                    case eInstructionType.DeHirePallets:
                        orderHandlingInstructions.Add(instruction);
                        _instructionPallet.Add(instruction.InstructionID, instruction);
                        break;
                }

                previousInstructionId = instruction.InstructionID;
            }

            repOH.DataSource = orderHandlingInstructions;
            repOH.DataBind();
        }

        #region Order Handling Event Handlers

        protected void repOrderHandling_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string userId = ((Entities.CustomPrincipal)Page.User).UserName;
            int plannerId = ((Entities.CustomPrincipal)Page.User).IdentityId;
            int selectedInstructionId;
            int previousInstructionId;
            int followingInstructionId;
            Facade.IJob facJob = new Facade.Job();

            switch (e.CommandName.ToLower())
            {
                case "down":
                    selectedInstructionId = Int32.Parse(((HtmlInputHidden)e.Item.FindControl("hidInstructionId")).Value);
                    followingInstructionId = FindRelativeInstructionId(selectedInstructionId, FindPreviousOrFollowingInstructionId.Following);
                    facJob.UpdateSwitchActions(Job, followingInstructionId, selectedInstructionId, plannerId, userId);

                    this.RefreshJobEntityCache();
                    PopulateJobControls();

                    break;

                case "up":
                    selectedInstructionId = Int32.Parse(((HtmlInputHidden)e.Item.FindControl("hidInstructionId")).Value);
                    previousInstructionId = FindRelativeInstructionId(selectedInstructionId, FindPreviousOrFollowingInstructionId.Previous);
                    facJob.UpdateSwitchActions(Job, selectedInstructionId, previousInstructionId, plannerId, userId);

                    this.RefreshJobEntityCache();
                    PopulateJobControls();

                    break;

                case "merge":
                    selectedInstructionId = Int32.Parse(((HtmlInputHidden)e.Item.FindControl("hidInstructionId")).Value);
                    followingInstructionId = FindRelativeInstructionId(selectedInstructionId, FindPreviousOrFollowingInstructionId.Following);
                    facJob.MergeInstructions(
                        Job.Instructions.GetForInstructionId(selectedInstructionId),
                        Job.Instructions.GetForInstructionId(followingInstructionId),
                        userId);

                    this.RefreshJobEntityCache();
                    PopulateJobControls();

                    break;
            }

            // Let subscribers know that the move or merge has compeleted.
            OnAfterMoveMergeCommand();

        }

        private Entities.Instruction GetInstruction(int instructionID)
        {
            var instruction = Job.Instructions.GetForInstructionId(instructionID);
            return instruction;
        }

        bool isDehireInstruction = false;
        protected void repOrderHandling_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            // Reset collect drop running totals
            weightRunningTotal = weightActualRunningTotal = palletSpacesRunningTotal = 0;
            orderCount = 0;
            hasActuals = false;
            multipleWeightCodes = false;
            palletsRunningTotal = palletsActualRunningTotal = casesRunningTotal = casesActualRunningTotal = 0;
            ////

            int itemIndex;

            Facade.IInstructionActual facInstructionActual = new Facade.Instruction();

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem || e.Item.ItemType == ListItemType.SelectedItem)
            {
                bool allowUp = false;
                bool allowDown = false;
                bool allowMerge = false;
                bool hasActual = false;
                bool allowRemove = false;

                Entities.Instruction instruction = (Entities.Instruction)e.Item.DataItem;

                Repeater collectDropRepeater = (Repeater)e.Item.FindControl("repOCD");
                switch ((eInstructionType)instruction.InstructionTypeId)
                {
                    case eInstructionType.Trunk:
                    case eInstructionType.LeavePallets:
                    case eInstructionType.DeHirePallets:
                        collectDropRepeater.DataSource = null;
                        collectDropRepeater.Visible = false;
                        break;

                    default:
                        collectDropRepeater.DataSource = instruction.CollectDrops;
                        collectDropRepeater.Visible = true;
                        break;
                }

             
                // If Instruction Type is not Pallet Handling or Trunk
                if (collectDropRepeater.DataSource != null)
                {
                    #region Order Handling

                    if (m_canEdit)
                    {
                        #region Can Edit Order Details

                        itemIndex = e.Item.ItemIndex;
                        int instructionId = instruction.InstructionID;
                        eInstructionType instructionType = (eInstructionType)instruction.InstructionTypeId;

                        Entities.InstructionActual actual = facInstructionActual.GetEntityForInstructionId(instructionId);
                        hasActual = (actual != null);

                        // Instructions can be moved if the instruction and the move target have no actuals and the previous instruction is not
                        // loading goods this instruction is delivering.
                        if (itemIndex > 0 && !hasActual && instructionType != eInstructionType.AttemptedDelivery)
                        {
                            int previousInstructionId = FindRelativeInstructionId(instructionId, FindPreviousOrFollowingInstructionId.Previous);
                            if (previousInstructionId > 0)
                            {
                                //Entities.Instruction previousInstruction = (facInstructionActual as Facade.IInstruction).GetInstruction(previousInstructionId);
                                Entities.Instruction previousInstruction = GetInstruction(previousInstructionId);

                                eInstructionType previousInstructionType = (eInstructionType)previousInstruction.InstructionTypeId;

                                bool previousHasActual = facInstructionActual.GetEntityForInstructionId(previousInstructionId) != null;

                                if (!previousHasActual && previousInstructionType != eInstructionType.AttemptedDelivery)
                                {
                                    if (previousInstructionType != eInstructionType.Load)
                                        allowUp = true;
                                    else
                                    {
                                        bool found = false;

                                        foreach (Entities.CollectDrop previousCD in previousInstruction.CollectDrops)
                                            if (instruction.CollectDrops.GetForOrderID(previousCD.OrderID) != null)
                                                found = true;

                                        allowUp = !found;
                                    }
                                }
                            }
                        }

                        if (!hasActual && instructionType != eInstructionType.AttemptedDelivery) //itemIndex < _orderedInstructionIdList.Count - 1 && 
                        {
                            int followingInstructionId = FindRelativeInstructionId(instructionId, FindPreviousOrFollowingInstructionId.Following);

                            Entities.Instruction followingInstruction = null;

                            //followingInstruction = (facInstructionActual as Facade.IInstruction).GetInstruction(followingInstructionId);
                            followingInstruction = GetInstruction(followingInstructionId);

                            if (followingInstruction != null)
                            {
                                eInstructionType followingInstructionType = (eInstructionType)followingInstruction.InstructionTypeId;

                                if (followingInstructionType != eInstructionType.AttemptedDelivery)
                                {
                                    // Instructions can be moved down if the following instruction is not delivering goods this instruction is loading.
                                    if (followingInstructionType != eInstructionType.Drop && followingInstructionType != eInstructionType.Trunk)
                                        allowDown = true;
                                    else
                                    {
                                        bool found = false;
                                        foreach (Entities.CollectDrop followingCD in followingInstruction.CollectDrops)
                                            if (instruction.CollectDrops.GetForOrderID(followingCD.OrderID) != null)
                                                found = true;

                                        allowDown = !found;
                                    }

                                    // Instructions can be merged if the following instruction is of the same type and this instruction does not have an actual and both instructions occur at the same point.
                                    allowMerge = instructionType == followingInstructionType && instruction.PointID == followingInstruction.PointID;
                                }
                            }
                        }

                        // Instructions can be edited if it is a collection and not all it's delivery dockets have been called in.
                        //Facade.IOrder facOrder = new Facade.Order();
                        //DataSet dsOrderHandling = facOrder.GetOrdersForInstructionID(instructionId);
                        //allowAlter = instructionType == eInstructionType.Load && dsOrderHandling.Tables[0].Select("DeliveryCalledIn = 0").Length > 0;
                        #endregion
                    }

                    // The user can only convert the instruction type if it is a drop
                    // instruction that is handling at least one order and no call-in
                    // has been recorded.
                    var btnConvertDrop = (Button)e.Item.FindControl("btnConvertDrop");
                    btnConvertDrop.Enabled = btnConvertDrop.Visible = !hasActual && instruction.InstructionTypeId == (int)eInstructionType.Drop && instruction.CollectDrops.Exists(cd => cd.OrderID > 0);
                    btnConvertDrop.OnClientClick = String.Format(convertInstructionURL, instruction.InstructionID);

                    ImageButton imgBtnUp = (ImageButton)e.Item.FindControl("imgBtnUp");
                    imgBtnUp.Enabled = allowUp;
                    imgBtnUp.Visible = allowUp;

                    if (allowUp)
                        imgBtnUp.Attributes.Add("onclick", "javascript: hidePageShowLoading();");

                    ImageButton imgBtnDown = (ImageButton)e.Item.FindControl("imgBtnDown");
                    imgBtnDown.Enabled = allowDown;
                    imgBtnDown.Visible = allowDown;

                    if (allowDown)
                        imgBtnDown.Attributes.Add("onclick", "javascript: hidePageShowLoading();");

                    ImageButton imgBtnMerge = (ImageButton)e.Item.FindControl("imgBtnMerge");
                    imgBtnMerge.Enabled = allowMerge;
                    imgBtnMerge.Visible = allowMerge;

                    if (allowMerge)
                        imgBtnMerge.Attributes.Add("onclick", "javascript: hidePageShowLoading();");

                    HtmlImage imgLegsPlanning = (HtmlImage)e.Item.FindControl("imgLegsPlanning");
                    imgLegsPlanning.Src = "~/images/expand.jpg";

                    List<ListItem> instructionActions = new List<ListItem>();
                    //Entities.Instruction instruction = (Entities.Instruction)e.Item.DataItem;

                    if (instruction.InstructionTypeId == (int)eInstructionType.Load)
                        m_cumulativePalletCount += instruction.TotalPallets;

                    Label lblInstructionType = e.Item.FindControl("lblInstructionType") as Label;
                    Label lblBookedDateTime = (Label)e.Item.FindControl("lblBookedDateTime");
                    Label lblArrivalDateTime = (Label)e.Item.FindControl("lblArrivalDateTime");
                    Label lblDepartureDateTime = (Label)e.Item.FindControl("lblDepartureDateTime");
                    Label lblCalledInDateTime = (Label)e.Item.FindControl("lblCalledInDateTime");
                    Label lblCallIn = (Label)e.Item.FindControl("lblCallIn");
                    Label lblPalletsOn = e.Item.FindControl("lblPalletsOn") as Label;
                    HtmlImage imgInstructionType = (HtmlImage)e.Item.FindControl("imgInstructionType");

                    // Get the planned time.
                    Label lblPlannedDateTime = (Label)e.Item.FindControl("lblPlannedDateTime");
                    lblPlannedDateTime.Text = instruction.PlannedArrivalDateTime.ToString("dd/MM/yy ddd, HH:mm");

                    // Display the booked date and time
                    if( instruction.InstructionTypeId == 1 || instruction.InstructionTypeId == 2 )
                        lblBookedDateTime.Text = GetInstructionBookedDateTime(instruction.InstructionID, instruction.InstructionTypeId);
                    else
                    {
                        if( instruction.IsAnyTime )
                            lblBookedDateTime.Text = instruction.BookedDateTime.ToString("dd/MM/yy ddd, ") + " AnyTime";
                        else
                            lblBookedDateTime.Text = instruction.BookedDateTime.ToString("dd/MM/yy ddd, HH:mm");
                    }

                    using (var uow = DIContainer.CreateUnitOfWork())
                    {
                        var pr = DIContainer.CreateRepository<Repositories.IPhotoRepository>(uow);
                        if (pr.GetForInstructionId(instruction.InstructionID).Any())
                        {
                            HtmlTableRow tblrowPhotos = (HtmlTableRow)e.Item.FindControl("tblrowPhotos");
                            tblrowPhotos.Visible = true;

                            HyperLink lnkPhotographs = (HyperLink)e.Item.FindControl("lnkPhotographs");
                            lnkPhotographs.NavigateUrl = String.Format("/mwf/SearchMwfPhotos.aspx?InstructionId={0}", instruction.InstructionID);
                        }
                    }

                    if (instruction.InstructionActuals != null)
                    {
                        if (instruction.InstructionTypeId != (int)eInstructionType.Load)
                            foreach (Entities.CollectDropActual cda in instruction.InstructionActuals[0].CollectDropActuals)
                                m_cumulativePalletCount -= (cda.NumberOfPallets - cda.NumberOfPalletsReturned);

                        // Set actual values
                        lblArrivalDateTime.Text = instruction.InstructionActuals[0].ArrivalDateTime.ToString("dd/MM/yy ddd, HH:mm");
                        lblDepartureDateTime.Text = instruction.InstructionActuals[0].LeaveDateTime.ToString("dd/MM/yy ddd, HH:mm");
                        lblCallIn.Text = instruction.InstructionActuals[0].CreateDate.ToString("dd/MM/yy ddd, HH:mm") + " " + instruction.InstructionActuals[0].CreateUser;

                        // If the Order has not been invoiced we cannot remove the call In.
                        Facade.IJob facJob = new Facade.Job();
                        Facade.IGoodsRefusal facGoodsRefusal = new Facade.GoodsRefusal();

                        bool beingInvoiced = facJob.IsBeingInvoiced(m_jobId) || facJob.OrdersBeingInvoicedForInstruction(instruction.InstructionID);
                        DataSet goodsRefusal = facGoodsRefusal.GetRefusalsForInstructionId(instruction.InstructionID);
                        int goodsRefusalCount = goodsRefusal.Tables[0].Rows.Count;

                        Button btn = e.Item.FindControl("btnRecordCallIn") as Button;
                        btn.CssClass = "buttonClassCallIn-Remove";
                        btn.Text = "";
                        btn.ToolTip = "Remove Call In";
                        if (!beingInvoiced)
                        {
                            btn.OnClientClick = "RemoveCallIn(" + instruction.JobId.ToString() + "," + instruction.InstructionID.ToString() + "," + goodsRefusalCount.ToString() +");return false;";

                            if (instruction.Redelivery == null && instruction.InstructionTypeId == (int)eInstructionType.Drop && (instruction.InstructionActuals != null && instruction.InstructionActuals.Count > 0))
                            {
                                Button btnRedeliver = e.Item.FindControl("btnRedeliver") as Button;
                                btnRedeliver.OnClientClick = "ReDeliver(" + instruction.JobId.ToString() + "," + instruction.InstructionID.ToString() + ");return false;";
                                btnRedeliver.Visible = true;
                            }
                        }
                        else
                        {
                            btn.OnClientClick = "AlreadyInvoiced("+instruction.JobId.ToString()+"," + instruction.InstructionID.ToString() + ")";
                        }


                        // Show the rows for actuals.
                        HtmlTableRow tblrowArrival = (HtmlTableRow)e.Item.FindControl("tblrowArrival");
                        HtmlTableRow tblrowDeparture = (HtmlTableRow)e.Item.FindControl("tblrowDeparture");
                        HtmlTableRow tblrowCallin = (HtmlTableRow)e.Item.FindControl("tblrowCallin");
                        tblrowArrival.Visible = tblrowDeparture.Visible = tblrowCallin.Visible = true;

                        // Add the call-in item to the instruction action combo.
                        instructionActions.Add(new ListItem("View Call-in", "window.location=webserver + '/Traffic/JobManagement/DriverCallIn/CallIn.aspx?wiz=true&jobId=" + instruction.JobId.ToString() + "&instructionId=" + instruction.InstructionID.ToString() + "' + getCSID();"));

                        // Find the extra id
                        string demurrageText = "Demurrage";
                        Entities.ExtraCollection extras = Job.Extras;
                        int extraId = 0;
                        if (extras != null && extras.Count > 0)
                            foreach (Entities.Extra extra in extras)
                                if (extra.InstructionId == instruction.InstructionID)
                                {
                                    extraId = extra.ExtraId;
                                    demurrageText = "Demurrage*";

                                    HtmlImage imgInstructionInfo = (HtmlImage)e.Item.FindControl("imgInstructionInfo");
                                    imgInstructionInfo.Src = Orchestrator.Globals.Configuration.WebServer + "/images/ico_info_small.gif";
                                    imgInstructionInfo.Alt = "Demurrage exists";
                                    imgInstructionInfo.Visible = true;

                                    break;
                                }

                        instructionActions.Add(new ListItem(demurrageText, "showDemurrageWindow(" + instruction.JobId + "," + instruction.InstructionID.ToString() + "," + extraId.ToString() + ");"));

                        var lvSignatures = (ListView)e.Item.FindControl("lvSignatures");

                        using (var uow = DIContainer.CreateUnitOfWork())
                        {
                            var repo = DIContainer.CreateRepository<Repositories.IInstructionRepository>(uow);
                            var signaturesForInstruction = repo.GetMWFSignaturesForInstruction(instruction.InstructionID);

                            lvSignatures.DataSource = signaturesForInstruction;
                            lvSignatures.DataBind();
                        }
                    }
                    else
                    {
                        if (instruction.InstructionTypeId != (int)eInstructionType.Load)
                            m_cumulativePalletCount -= instruction.TotalPallets;

                        // If the Order has not been invoiced we cannot remove the call In.
                        if (instruction.InstructionState == eInstructionState.Planned || instruction.InstructionState == eInstructionState.InProgress || instruction.InstructionState == eInstructionState.Completed)
                        {
                            Button btn = e.Item.FindControl("btnRecordCallIn") as Button;
                            btn.CssClass = "buttonClassCallIn-Add";
                            btn.ToolTip = "Add Call In";
                            btn.Text = "";
                            btn.OnClientClick = "location.href = '/Traffic/JobManagement/DriverCallIn/CallIn.aspx?jobid=" + instruction.JobId + "&instructionid=" + instruction.InstructionID + "'+ getCSID(); return false;";

                            if (instruction.Redelivery == null && instruction.InstructionTypeId == (int)eInstructionType.Drop && (instruction.InstructionActuals != null && instruction.InstructionActuals.Count > 0))
                            {
                                Button btnRedeliver = e.Item.FindControl("btnRedeliver") as Button;
                                btnRedeliver.OnClientClick = "ReDeliver(" + instruction.JobId.ToString() + "," + instruction.InstructionID.ToString() + ");return false;";
                                btnRedeliver.Visible = true;
                            }
                        }
                        else
                        {
                            ((Button)e.Item.FindControl("btnRecordCallIn")).Visible = false;
                        }
                    }

                    if (Globals.Configuration.IsMwfCustomer)
                    {
                        using (var uow = DIContainer.CreateUnitOfWork())
                        {
                            var repo = DIContainer.CreateRepository<Repositories.IMWF_InstructionRepository>(uow);
                            var mwfInstruction = repo.GetForHaulierEnterpriseInstruction(instruction.InstructionID);

                            if (mwfInstruction != null)
                            {
                                var statusDescription = Models.MWF_Instruction.GetStatusDescription(mwfInstruction.Status);

                                ((HtmlTableRow)e.Item.FindControl("tblrowMwfStatus")).Visible = true;

                                if (!string.IsNullOrWhiteSpace(mwfInstruction.DeviceIdentifier))
                                    ((Label)e.Item.FindControl("lblMwfStatus")).Text = statusDescription + " (" + mwfInstruction.DeviceIdentifier + ")";
                                else
                                    ((Label)e.Item.FindControl("lblMwfStatus")).Text = statusDescription;     
                                
                            }
                        }
                    }

                    Button btnAddOrder = null;
                    // Display the action being performed.
                    switch ((eInstructionType)instruction.InstructionTypeId)
                    {
                        case eInstructionType.Load:
                            lblInstructionType.Text = "Load";
                            imgInstructionType.Src = @"~/images/loadfinal.png";
                            imgInstructionType.Alt = "Load";
                            btnAddOrder = e.Item.FindControl("btnAddOrder") as Button;
                            btnAddOrder.Visible = true;
                            btnAddOrder.OnClientClick = string.Format(addOrderurl, instruction.InstructionID, instruction.InstructionOrder, instruction.PointID, instruction.InstructionTypeId, instruction.JobId);
                            instructionActions.Add(new ListItem("Load Release Form", "window.location=webserver + '/Traffic/JobManagement/LoadReleaseNotification.aspx?jobId=" + instruction.JobId.ToString() + "&instructionId=" + instruction.InstructionID.ToString() + "'"));
                            break;

                        case eInstructionType.Drop:
                            lblInstructionType.Text = "Drop";
                            imgInstructionType.Alt = "Drop";
                            btnAddOrder = e.Item.FindControl("btnAddOrder") as Button;
                            btnAddOrder.Visible = true;
                            btnAddOrder.OnClientClick = string.Format(addOrderurl, instruction.InstructionID, instruction.InstructionOrder, instruction.PointID, instruction.InstructionTypeId, instruction.JobId);
                            imgInstructionType.Src = instruction.Redelivery == null ? @"~/images/dropfinal.png" : @"~/images/dropturnedaway.png";
                            break;

                        case eInstructionType.Trunk:
                            lblInstructionType.Text = "Trunk";
                            imgInstructionType.Src = @"~/images/warehouselayers.png";
                            imgInstructionType.Alt = "Trunk";
                            break;

                        case eInstructionType.AttemptedDelivery:
                            lblInstructionType.Text = "Attempted Delivery";
                            imgInstructionType.Src = @"~/images/dropturnedaway.png";
                            imgInstructionType.Alt = "Attempted Delivery";
                            e.Item.FindControl("repOCD").Visible = false;
                            break;

                        default:
                            break;
                    }

                    // Display details about redelivery / attempted delivery data
                    Repeater repTurnedAwayOCD = (Repeater)e.Item.FindControl("repTurnedAwayOCD");
                    if (instruction.Redelivery != null)
                    {
                        repTurnedAwayOCD.Visible = true;
                        repTurnedAwayOCD.DataSource = instruction.Redelivery.Orders;
                        repTurnedAwayOCD.DataBind();
                    }
                    else
                        repTurnedAwayOCD.Visible = false;

                    // Display the combo list and "go" button for instruction actions, if any exist.
                    if (instructionActions.Count > 0)
                    {
                        // Create the instruction action combo
                        HtmlSelect select = new HtmlSelect();
                        select.ID = "ActionSelector" + instruction.InstructionID.ToString();
                        select.Style.Add("Width", "120px");

                        // Add items to the combo
                        foreach (ListItem item in instructionActions)
                            select.Items.Add(item);

                        // Add the instruction action combo to the form
                        PlaceHolder ph = (PlaceHolder)e.Item.FindControl("phInstructionAction");
                        ph.Controls.Add(select);

                        // Wire up the go button so that it executes the users chosen instruction action
                        HtmlInputButton btnInstructionActionGo = (HtmlInputButton)e.Item.FindControl("btnInstructionActionGo");
                        btnInstructionActionGo.Attributes.Add("onclick", "javascript:ActionSelector('" + select.ClientID + "');");
                    }

                    // Display the cumulative number of pallets on the trailer after this instruction is performed.
                    lblPalletsOn.Text = m_cumulativePalletCount.ToString();

                    // Hide the action drop down if there are no options for the user to select.
                    Panel pnlInstructionActual = (Panel)e.Item.FindControl("pnlInstructionActual");
                    if (instructionActions.Count < 1)
                        pnlInstructionActual.Visible = false;
                    else
                        pnlInstructionActual.Visible = true;

                    #endregion
                }

                HtmlTableRow rowCollectDrops = (HtmlTableRow)e.Item.FindControl("tblRowCollectDrops");
                HtmlTableRow rowPalletHandling = (HtmlTableRow)e.Item.FindControl("rowPalletHandling");
                HtmlTableRow rowTrunk = (HtmlTableRow)e.Item.FindControl("rowTrunk");
                HtmlTableRow rowTrunkSeparator = (HtmlTableRow)e.Item.FindControl("rowTrunkSeparator");
                HtmlTableRow rowTrunkSpacer = (HtmlTableRow)e.Item.FindControl("rowTrunkSpacer");

                // Show/hide trunk row (do not show for cross docks, transships or leave on trailer.
                //if (_instructionTrunk.ContainsKey(instruction.InstructionID))
                if (instruction.InstructionTypeId == (int)eInstructionType.Trunk)
                {
                    #region Trunk Instruction

                    Label lblInstructionAction = e.Item.FindControl("lblInstructionAction") as Label;

                    Entities.InstructionActual actual = facInstructionActual.GetEntityForInstructionId(instruction.InstructionID);
                    hasActual = (actual != null);

                    rowCollectDrops.Visible = false;
                    rowPalletHandling.Visible = false;
                    rowTrunk.Visible = true;
                    rowTrunkSeparator.Visible = true;
                    rowTrunkSpacer.Visible = true;

                    // Get the trunk instruction
                    Entities.Instruction trunkInstruction = instruction; // _instructionTrunk[instruction.InstructionID];
                    HtmlTableRow trunkPalletsOn = e.Item.FindControl("trunkPalletsOn") as HtmlTableRow;
                    Button btnTipSheet = e.Item.FindControl("btnTipSheet") as Button;
                    HtmlImage imgRemoveTrunk = (HtmlImage)e.Item.FindControl("imgRemoveTrunk");
                    HtmlImage imgUpdateTrunkPoint = e.Item.FindControl("imgUpdateTrunkPoint") as HtmlImage;
                    HtmlImage Img1 = e.Item.FindControl("Img1") as HtmlImage;

                    if (trunkInstruction.CollectDrops.Exists(cd => cd.OrderID > 0))
                    {
                        Repeater repTrunkOrders = e.Item.FindControl("repTrunkOrders") as Repeater;
                        Button btnAddOrder = e.Item.FindControl("btnTrunkAddOrder") as Button;
                        Label lblTrunkPalletsOn = e.Item.FindControl("lblTrunkPalletsOn") as Label;

                        repTrunkOrders.DataSource = trunkInstruction.CollectDrops.FindAll(cd => cd.Order != null).ToList();
                        repTrunkOrders.ItemDataBound += new RepeaterItemEventHandler(repTrunkOrders_ItemDataBound);
                        repTrunkOrders.DataBind();

                        btnAddOrder.Visible = true;
                        btnAddOrder.OnClientClick = string.Format(addOrderurl, instruction.InstructionID, instruction.InstructionOrder, instruction.PointID, instruction.InstructionTypeId, instruction.JobId);

                        // Display the cumulative number of pallets on the trailer after this instruction is performed.
                        m_cumulativePalletCount -= instruction.TotalPallets;
                        lblTrunkPalletsOn.Text = m_cumulativePalletCount.ToString();

                        if (trunkPalletsOn != null)
                            trunkPalletsOn.Visible = true;

                        btnTipSheet.CommandArgument = instruction.InstructionID.ToString();
                        btnTipSheet.Visible = true;

                        imgUpdateTrunkPoint.Visible = false;
                        imgRemoveTrunk.Visible = false;

                        Img1.Src = "/images/warehouselayers.png";

                        if (trunkInstruction.CollectDrops.Exists(cd => cd.OrderAction == eOrderAction.Cross_Dock))
                            Img1.Alt = lblInstructionAction.Text = "Cross Dock";
                        else
                            Img1.Alt = lblInstructionAction.Text = "Trans Ship";

                        // The user can only convert the instruction type if it is a trunk
                        // instruction that is handling at least one order and no call-in
                        // has been recorded.
                        var btnConvertTrunk = (Button)e.Item.FindControl("btnConvertTrunk");
                        btnConvertTrunk.Enabled = btnConvertTrunk.Visible = !hasActual;
                        btnConvertTrunk.OnClientClick = String.Format(convertInstructionURL, instruction.InstructionID);
                    }
                    else
                    {
                        if (trunkPalletsOn != null)
                            trunkPalletsOn.Visible = false;

                        btnTipSheet.Visible = false;

                        lblInstructionAction.Text = "Trunk";

                        string instructionStateID = ((int)trunkInstruction.InstructionState).ToString();
                        string cInstructionID = trunkInstruction.InstructionID.ToString();
                        string cDriver = trunkInstruction.Driver != null ? trunkInstruction.Driver.Individual.FullName : string.Empty;
                        string cVehicle = trunkInstruction.Vehicle != null ? trunkInstruction.Vehicle.RegNo : string.Empty;
                        string cLastUpdateDate = this.Job.LastUpdateDate.ToString();

                        imgUpdateTrunkPoint.Visible = true;
                        imgUpdateTrunkPoint.Attributes.Add("onclick", "javascript:if( " + instructionStateID + " != 4 ) { openUpdateTrunkWindow('" + cInstructionID + "','" + cDriver + "','" + cVehicle + "','" + cLastUpdateDate + "'); } else { alert('This leg has already been completed and cannot be removed.'); };");
                        imgRemoveTrunk.Attributes.Add("onclick", "javascript:if (" + ((int)trunkInstruction.InstructionState).ToString() + " != 4) {openRemoveTrunkWindow(" + trunkInstruction.JobId.ToString() + " ," + trunkInstruction.InstructionID.ToString() + ", '" + this.Job.LastUpdateDate.ToString() + "');} else {alert('This leg has already been completed and cannot be removed.')};");

                        Img1.Src = "/images/trunk.gif";
                        Img1.Alt = "Trunk";
                    }

                    Facade.IJob facJob = new Facade.Job();
                    Facade.IGoodsRefusal facGoodsRefusal = new Facade.GoodsRefusal();

                    bool beingInvoiced = facJob.IsBeingInvoiced(m_jobId) || facJob.OrdersBeingInvoicedForInstruction(instruction.InstructionID);
                    DataSet goodsRefusal = facGoodsRefusal.GetRefusalsForInstructionId(instruction.InstructionID);
                    int goodsRefusalCount = goodsRefusal.Tables[0].Rows.Count;

                    Button btn = e.Item.FindControl("btnTrunkCallIn") as Button;

                    if (instruction.HasActual)
                    {
                        if (!beingInvoiced)
                        {
                            btn.CssClass = "buttonClassCallIn-Remove";
                            btn.Text = "";
                            btn.ToolTip = "Remove Call In";
                            btn.OnClientClick = "RemoveCallIn(" + instruction.JobId.ToString() + "," + instruction.InstructionID.ToString() + "," + goodsRefusalCount.ToString() + ");return false;";
                        }
                        else
                            btn.Visible = false;

                    }
                    else
                    {
                        if (!beingInvoiced)
                        {
                            if (instruction.InstructionState == eInstructionState.Planned || instruction.InstructionState == eInstructionState.InProgress || instruction.InstructionState == eInstructionState.Completed)
                            {
                                btn.CssClass = "buttonClassCallIn-Add";
                                btn.Text = "";
                                btn.ToolTip = "Add Call In";
                                btn.OnClientClick = "location.href = '/Traffic/JobManagement/DriverCallIn/CallIn.aspx?jobid=" + instruction.JobId + "&instructionid=" + instruction.InstructionID + "'+ getCSID(); return false;";
                            }
                            else
                                btn.Visible = false;
                        }
                    }

                    Label lblTrunkPointOrganisation = (Label)e.Item.FindControl("lblTrunkPointOrganisation");
                    lblTrunkPointOrganisation.Text = trunkInstruction.Point.OrganisationName;

                    Label lblTrunkPointDescription = (Label)e.Item.FindControl("lblTrunkPointDescription");
                    lblTrunkPointDescription.Text = trunkInstruction.Point.Description;

                    // Display the trunk booked date and time
                    Label lblTrunkBookedDateTime = (Label)e.Item.FindControl("lblTrunkBookedDateTime");
                    if (trunkInstruction.IsAnyTime)
                        lblTrunkBookedDateTime.Text = trunkInstruction.BookedDateTime.ToString("dd/MM/yy ddd, ") + " AnyTime";
                    else
                        lblTrunkBookedDateTime.Text = trunkInstruction.BookedDateTime.ToString("dd/MM/yy ddd, HH:mm");

                    // Enable the trunk mouse over point address popup
                    HtmlGenericControl spnTrunkPoint = (HtmlGenericControl)e.Item.FindControl("spnTrunkPoint");
                    spnTrunkPoint.Attributes.Add("onmouseover", "javascript:ShowPointToolTip(this," + trunkInstruction.Point.PointId.ToString() + ");");

                    // Display the trunk departure time.
                    Label lblTrunkDepartureDateTime = (Label)e.Item.FindControl("lblTrunkDepartureDateTime");
                    lblTrunkDepartureDateTime.Text = trunkInstruction.PlannedDepartureDateTime.ToString("dd/MM/yy ddd, HH:mm");

                    // Display the trunk planned date and time and also specify the RemoveTrunk button.
                    Label lblTrunkPlannedDateTime = (Label)e.Item.FindControl("lblTrunkPlannedDateTime");
                    lblTrunkPlannedDateTime.Text = trunkInstruction.PlannedArrivalDateTime.ToString("dd/MM/yy ddd, HH:mm");

                    if (Globals.Configuration.IsMwfCustomer)
                    {
                        using (var uow = DIContainer.CreateUnitOfWork())
                        {
                            var repo = DIContainer.CreateRepository<Repositories.IMWF_InstructionRepository>(uow);
                            var mwfTrunkToInstruction = repo.GetForHaulierEnterpriseInstruction(instruction.InstructionID, MWFInstructionTypeEnum.TrunkTo);
                            var mwfProceedFromInstruction = repo.GetProceedFromInstructionForHEOriginInstruction(instruction.InstructionID);

                            if (mwfTrunkToInstruction != null)
                            {
                                var trunkToStatusDescription = Models.MWF_Instruction.GetStatusDescription(mwfTrunkToInstruction.Status);

                                ((HtmlTableRow)e.Item.FindControl("trTrunkToMwfStatus")).Visible = true;
                                ((Label)e.Item.FindControl("lblTrunkToMwfStatus")).Text = trunkToStatusDescription;
                            }

                            if (mwfProceedFromInstruction != null)
                            {
                                var proceedFromStatusDescription = Models.MWF_Instruction.GetStatusDescription(mwfProceedFromInstruction.Status);

                                ((HtmlTableRow)e.Item.FindControl("trTrunkProceedFromMwfStatus")).Visible = true;
                                ((Label)e.Item.FindControl("lblTrunkProceedFromMwfStatus")).Text = proceedFromStatusDescription;
                            }
                        }
                    }

                    #endregion
                }

                if ((eInstructionType)instruction.InstructionTypeId == eInstructionType.DeHirePallets || (eInstructionType)instruction.InstructionTypeId == eInstructionType.LeavePallets || (eInstructionType)instruction.InstructionTypeId == eInstructionType.PickupPallets)
                {
                    #region Pallet Handling

                    List<ListItem> instructionActions = new List<ListItem>();

                    HtmlTableCell phPalletsON = e.Item.FindControl("phPalletsON") as HtmlTableCell;
                    HtmlTableCell phPalletsOFF = e.Item.FindControl("phPalletsOFF") as HtmlTableCell;

                    Label lblphOn = e.Item.FindControl("lblphOn") as Label;

                    Label lblphBookedDateTime = (Label)e.Item.FindControl("lblphBookedDateTime");
                    Label lblphPlannedDateTime = e.Item.FindControl("lblphPlannedDateTime") as Label;
                    Label lblphArrivalDateTime = (Label)e.Item.FindControl("lblphArrivalDateTime");
                    Label lblphDepartureDatetime = (Label)e.Item.FindControl("lblphDepartureDatetime");
                    Label lblphCallIn = (Label)e.Item.FindControl("lblphCallIn");

                    HtmlImage imgphLegsPlanning = (HtmlImage)e.Item.FindControl("imgphLegsPlanning");
                    imgphLegsPlanning.Src = "~/images/expand.jpg";

                    if (instruction.InstructionTypeId == (int)eInstructionType.PickupPallets)
                        m_cumulativePalletCount += instruction.TotalPallets;
                    else
                        m_cumulativePalletCount -= instruction.TotalPallets;

                    // Get the planned time.
                    lblphPlannedDateTime.Text = instruction.PlannedArrivalDateTime.ToString("dd/MM/yy ddd, HH:mm");

                    // Display the booked date and time
                    if (instruction.IsAnyTime)
                        lblphBookedDateTime.Text = instruction.BookedDateTime.ToString("dd/MM/yy ddd, ") + " AnyTime";
                    else
                        lblphBookedDateTime.Text = instruction.BookedDateTime.ToString("dd/MM/yy ddd, HH:mm");

                    if (instruction.InstructionActuals != null)
                    {
                        // Set actual values
                        lblphArrivalDateTime.Text = instruction.InstructionActuals[0].ArrivalDateTime.ToString("dd/MM/yy ddd, HH:mm");
                        lblphDepartureDatetime.Text = instruction.InstructionActuals[0].LeaveDateTime.ToString("dd/MM/yy ddd, HH:mm");
                        lblphCallIn.Text = instruction.InstructionActuals[0].CreateDate.ToString("dd/MM/yy ddd, HH:mm") + " " + instruction.InstructionActuals[0].CreateUser;

                        // If the Order has been invoiced we cannot remove the call In.
                        Facade.IJob facJob = new Facade.Job();
                        Facade.IGoodsRefusal facGoodsRefusal = new Facade.GoodsRefusal();

                        bool beingInvoiced = facJob.IsBeingInvoiced(m_jobId) || facJob.OrdersBeingInvoicedForInstruction(instruction.InstructionID);
                        DataSet goodsRefusal = facGoodsRefusal.GetRefusalsForInstructionId(instruction.InstructionID);

                        int goodsRefusalCount = goodsRefusal.Tables[0].Rows.Count;

                        if (!beingInvoiced)
                        {
                            Button btn = e.Item.FindControl("btnPalletCallIn") as Button;
                            btn.CssClass = "buttonClassCallIn-Remove";
                            btn.Text = "";
                            btn.ToolTip = "Remove Call In";
                            btn.OnClientClick = "RemoveCallIn(" + instruction.JobId.ToString() + "," + instruction.InstructionID.ToString() + "," + goodsRefusalCount.ToString() + ");return false;";
                            
                        }

                        // Show the rows for actuals.
                        HtmlTableRow phArrivalDateTime = (HtmlTableRow)e.Item.FindControl("phArrivalDateTime");
                        HtmlTableRow phDepartureDateTime = (HtmlTableRow)e.Item.FindControl("phDepartureDateTime");
                        HtmlTableRow phCallIn = (HtmlTableRow)e.Item.FindControl("phCallIn");
                        phArrivalDateTime.Visible = phDepartureDateTime.Visible = phCallIn.Visible = true;

                        // Add the call-in item to the instruction action combo.
                        instructionActions.Add(new ListItem("View Call-in", "window.location=webserver + '/Traffic/JobManagement/DriverCallIn/CallIn.aspx?wiz=true&jobId=" + instruction.JobId.ToString() + "&instructionId=" + instruction.InstructionID.ToString() + "'+ getCSID();"));

                        // Find the extra id
                        string demurrageText = "Demurrage";
                        Entities.ExtraCollection extras = Job.Extras;
                        int extraId = 0;
                        if (extras != null && extras.Count > 0)
                            foreach (Entities.Extra extra in extras)
                                if (extra.InstructionId == instruction.InstructionID)
                                {
                                    extraId = extra.ExtraId;
                                    demurrageText = "Demurrage*";

                                    HtmlImage imgInstructionInfo = (HtmlImage)e.Item.FindControl("imgInstructionInfo");
                                    imgInstructionInfo.Src = Orchestrator.Globals.Configuration.WebServer + "/images/ico_info_small.gif";
                                    imgInstructionInfo.Alt = "Demurrage exists";
                                    imgInstructionInfo.Visible = true;

                                    break;
                                }

                        instructionActions.Add(new ListItem(demurrageText, "showDemurrageWindow(" + instruction.JobId + "," + instruction.InstructionID.ToString() + "," + extraId.ToString() + ");"));
                    }
                    else
                    {
                        // If the Order has not been invoiced we cannot remove the call In.
                        if (instruction.InstructionState == eInstructionState.Planned || instruction.InstructionState == eInstructionState.InProgress || instruction.InstructionState == eInstructionState.Completed)
                        {
                            Button btn = e.Item.FindControl("btnPalletCallIn") as Button;
                            btn.CssClass = "buttonClassCallIn-Add";
                            btn.ToolTip = "Add Call In";
                            btn.Text = "";
                            btn.OnClientClick = "location.href = '/Traffic/JobManagement/DriverCallIn/CallIn.aspx?jobid=" + instruction.JobId + "&instructionid=" + instruction.InstructionID + "'+ getCSID(); return false;";
                        }
                        else
                        {
                            ((Button)e.Item.FindControl("btnPalletCallIn")).Visible = false;
                        }
                    }

                    Button btnPalletHandling = e.Item.FindControl("btnPalletHandling") as Button;
                    if (instruction.Trailer != null)
                    {
                        btnPalletHandling.OnClientClick = "openPalletHandlingWindow(" + instruction.JobId.ToString() + ");return false;";
                    }
                    else
                    {
                        btnPalletHandling.OnClientClick = "alert('You must assign a trailer in order to handle pallets');return false;";
                    }
                    

                    // Display the combo list and "go" button for instruction actions, if any exist.
                    if (instructionActions.Count > 0)
                    {
                        // Create the instruction action combo
                        HtmlSelect select = new HtmlSelect();
                        select.ID = "ActionSelector" + instruction.InstructionID.ToString() + string.Format("_{0}", new Random(instruction.InstructionID).Next());
                        select.Style.Add("Width", "120px");

                        // Add items to the combo
                        foreach (ListItem item in instructionActions)
                            select.Items.Add(item);

                        // Add the instruction action combo to the form
                        PlaceHolder phPalletHandling = (PlaceHolder)e.Item.FindControl("phPalletHandlingInstructionAction");
                        phPalletHandling.Controls.Add(select);

                        // Wire up the go button so that it executes the users chosen instruction action
                        HtmlInputButton btnphInstructionActionGo = (HtmlInputButton)e.Item.FindControl("btnphInstructionActionGo");
                        btnphInstructionActionGo.Attributes.Add("onclick", "javascript:ActionSelector('" + select.ClientID + "');");
                    }

                    // Hide the action drop down if there are no options for the user to select.
                    Panel pnlphInstructionActual = (Panel)e.Item.FindControl("pnlphInstructionActual");
                    if (instructionActions.Count < 1)
                        pnlphInstructionActual.Visible = false;
                    else
                        pnlphInstructionActual.Visible = true;

                    if ((eInstructionType)instruction.InstructionTypeId == eInstructionType.DeHirePallets)
                        isDehireInstruction = true;

                    HtmlImage imgPalletHandling = (HtmlImage)e.Item.FindControl("imgPalletHandling");
                    Label lblPalletHandling = e.Item.FindControl("lblPalletHandling") as Label;

                    switch ((eInstructionType)instruction.InstructionTypeId)
                    {
                        case eInstructionType.DeHirePallets:
                            imgPalletHandling.Src = @"~/images/icon-pallet-dehire.png";
                            lblPalletHandling.Text = "De Hire Pallets";
                            break;
                        case eInstructionType.PickupPallets:
                            imgPalletHandling.Src = @"~/images/icon-pallet-load.png";
                            lblPalletHandling.Text = "Pick Up Pallets";
                            break;

                        default:
                            imgPalletHandling.Src = @"~/images/icon-pallet-leave.png";
                            lblPalletHandling.Text = "Leave Pallets";
                            break;
                    }

                    rowCollectDrops.Visible = false;
                    rowPalletHandling.Visible = true;
                    rowTrunk.Visible = false;
                    rowTrunkSeparator.Visible = false;
                    rowTrunkSpacer.Visible = false;

                    // Get the Pallet instruction
                    IEnumerable<Entities.InstructionActual> iaCollection = instruction.InstructionActuals == null ? Enumerable.Empty<Entities.InstructionActual>() : instruction.InstructionActuals.Cast<Entities.InstructionActual>();
                    var collectDropsWithActuals =
                        from cd in instruction.CollectDrops.Cast<Entities.CollectDrop>()
                        let cda =
                                (from ia in iaCollection
                                 from cda in ia.CollectDropActuals.Cast<Entities.CollectDropActual>()
                                 where cda.CollectDropId == cd.CollectDropId
                                 select cda).SingleOrDefault()
                        select new CollectDropsWithActuals
                        {
                            CollectDrop = cd,
                            InstructionType = (eInstructionType)instruction.InstructionTypeId,
                            NumberOfPallets = cda != null ? cda.NumberOfPallets : -1,
                            DehiringReceipt = (cda != null && cda.DehiringReceipt != null) ? cda.DehiringReceipt.ReceiptNumber : "",
                        };

                    Repeater repPalletHandling = e.Item.FindControl("repPalletHandling") as Repeater;
                    repPalletHandling.DataSource = collectDropsWithActuals;
                    repPalletHandling.ItemDataBound += new RepeaterItemEventHandler(repPalletHandling_ItemDataBound);
                    repPalletHandling.DataBind();

                    lblphOn.Text = m_cumulativePalletCount.ToString();

                    #endregion
                }

                // reset Is Dehire Flag
                isDehireInstruction = false;
            }
        }

        void repTrunkOrders_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Entities.CollectDrop cd = (Entities.CollectDrop)e.Item.DataItem;

                HtmlAnchor hypOrderId = (HtmlAnchor)e.Item.FindControl("hypOrderId");
                hypOrderId.HRef = string.Format("javascript:{0}", this.dlgOrder.GetOpenDialogScript(string.Format("oid={0}", cd.Order.OrderID)));

                HtmlAnchor hypLoadNumber = (HtmlAnchor)e.Item.FindControl("hypLoadNumber");
                hypLoadNumber.HRef = string.Format("javascript:{0}", this.dlgOrder.GetOpenDialogScript(string.Format("oid={0}", cd.Order.OrderID)));

                HtmlAnchor hypDocketNumber = (HtmlAnchor)e.Item.FindControl("hypDocketNumber");
                hypDocketNumber.HRef = string.Format("javascript:{0}", this.dlgOrder.GetOpenDialogScript(string.Format("oid={0}", cd.Order.OrderID)));
            }
        }

        decimal weightRunningTotal = 0, weightActualRunningTotal = 0, palletSpacesRunningTotal = 0;
        int orderCount = 0;
        string weightCode;
        bool multipleWeightCodes = false, hasActuals = false;
        int palletsRunningTotal = 0, palletsActualRunningTotal = 0, casesRunningTotal = 0, casesActualRunningTotal = 0;

        void repPalletHandling_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                #region Header

                Label lblRate = e.Item.FindControl("lblRate") as Label;
                HtmlTableCell trforeignRate = e.Item.FindControl("trforeignRate") as HtmlTableCell;

                if (lblRate != null && trforeignRate != null)
                    lblRate.Visible = trforeignRate.Visible = Globals.Configuration.MultiCurrency;

                if (isDehireInstruction)
                {
                    HtmlTableCell thDehireReceipt = e.Item.FindControl("thDehireReceipt") as HtmlTableCell;
                    thDehireReceipt.Visible = true;
                }

                orderCount = palletsActualRunningTotal = palletsRunningTotal = 0;

                #endregion
            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                CollectDropsWithActuals collectDropWithActuals = (CollectDropsWithActuals)e.Item.DataItem;
                Label lblOrderAction = (Label)e.Item.FindControl("lblOrderAction");

                orderCount++;
                palletsRunningTotal += collectDropWithActuals.CollectDrop.NoPallets;

                #region Pallet Details

                if (collectDropWithActuals.InstructionType == eInstructionType.PickupPallets)
                    lblOrderAction.Text = "Collect";
                else if (collectDropWithActuals.InstructionType == eInstructionType.LeavePallets || collectDropWithActuals.InstructionType == eInstructionType.DeHirePallets)
                    lblOrderAction.Text = "Deliver";

                if (isDehireInstruction)
                {
                    HtmlTableCell tdDehireReceipt = e.Item.FindControl("tdDehireReceipt") as HtmlTableCell;
                    tdDehireReceipt.Visible = true;
                }

                if (collectDropWithActuals.NumberOfPallets >= 0)
                {
                    Label lblPalletsActual = e.Item.FindControl("lblPalletsActual") as Label;

                    palletsActualRunningTotal += collectDropWithActuals.NumberOfPallets;
                    lblPalletsActual.Text = string.Format(" ({0})", collectDropWithActuals.NumberOfPallets.ToString());

                    if (collectDropWithActuals.DehiringReceipt.Length > 0)
                    {
                        Label lblDehireReceipt = e.Item.FindControl("lblDehireReceipt") as Label;
                        lblDehireReceipt.Text = collectDropWithActuals.DehiringReceipt;
                    }
                }

                #endregion

                #region Pallet Order Details

                Entities.Order order = collectDropWithActuals.CollectDrop.Order;
                HtmlImage imgRemoveOrder = (HtmlImage)e.Item.FindControl("imgRemoveOrder");
                HtmlImage imgOrderCollectionDeliveryNotes = (HtmlImage)e.Item.FindControl("imgOrderCollectionDeliveryNotes");
                System.Web.UI.WebControls.Image imgHasExtras = (System.Web.UI.WebControls.Image)e.Item.FindControl("imgHasExtra");

                // Check to see if the order is being invoiced
                Orchestrator.Facade.IOrder facOrder = new Orchestrator.Facade.Order();
                bool orderBeingInvoiced = facOrder.IsOrderBeingInvoiced(order.OrderID);

                // Show invoice link as appropriate. The invoice link becomes part of the order status text.
                int invoiceId = 0;
                if (order.OrderStatus == eOrderStatus.Invoiced)
                {
                    invoiceId = facOrder.ClientInvoiceID(order.OrderID);
                }

                PlaceHolder phStatus = (PlaceHolder)e.Item.FindControl("phOrderStatus");
                if (invoiceId > 0)
                {
                    string strInvoiceUrl = Orchestrator.Globals.Configuration.WebServer + order.PDFLocation;
                    HyperLink hypStatus = new HyperLink();
                    hypStatus.Target = "_blank";
                    hypStatus.NavigateUrl = strInvoiceUrl;
                    hypStatus.Text = string.Format("{0}({1})", order.OrderStatus.ToString(), invoiceId.ToString());
                    hypStatus.ToolTip = string.Concat("Click to view invoice ", invoiceId.ToString());
                    phStatus.Controls.Add(hypStatus);
                }
                else
                {
                    // If there is no invoice, just show the order status text.
                    Label lblStatus = new Label();
                    lblStatus.Text = order.OrderStatus.ToString();
                    phStatus.Controls.Add(lblStatus);
                }

                // Specify the remove order button as appropriate
                if (order.OrderStatus == eOrderStatus.Invoiced)
                {
                    imgRemoveOrder.Attributes.Add("onclick", "javascript:alert('You cannot remove this order because it has been invoiced.');");
                    imgRemoveOrder.Src = "~/images/itxt_xButton_greyedout.GIF";
                    imgRemoveOrder.Alt = "You cannot remove this order because it has been invoiced.";
                }
                else if (order.OrderStatus == eOrderStatus.Delivered && Job.Instructions != null && Job.Instructions.Exists(ins => ins.InstructionTypeId == (int)eInstructionType.Drop && ins.CollectDrops != null && ins.CollectDrops.Exists(cd => cd.OrderID == order.OrderID)))
                {
                    // That the order has been delivered and this is the delivery job, otherwise you should be able to remove the order from collection run.
                    imgRemoveOrder.Attributes.Add("onclick", "javascript:alert('You cannot remove this order because it has been delivered on this run.');");
                    imgRemoveOrder.Src = "~/images/itxt_xButton_greyedout.GIF";
                    imgRemoveOrder.Alt = "You cannot remove this order because it has been delivered.";
                }
                else if (order.OrderStatus == eOrderStatus.Cancelled)
                {
                    imgRemoveOrder.Attributes.Add("onclick", "javascript:alert('You cannot remove this order because it has been cancelled.');");
                    imgRemoveOrder.Src = "~/images/itxt_xButton_greyedout.GIF";
                    imgRemoveOrder.Alt = "You cannot remove this order because it has been cancelled.";
                }
                else if (orderBeingInvoiced)
                {
                    imgRemoveOrder.Attributes.Add("onclick", "javascript:alert('You cannot remove this order because it is being invoiced.');");
                    imgRemoveOrder.Src = "~/images/itxt_xButton_greyedout.GIF";
                    imgRemoveOrder.Alt = "You cannot remove this order because it is being invoiced.";
                }
                else
                {
                    imgRemoveOrder.Attributes.Add("onclick", "javascript:alert('You cannot currently remove pallet handling orders.');");
                    imgRemoveOrder.Src = "~/images/itxt_xButton_greyedout.GIF";
                    imgRemoveOrder.Alt = "You cannot currently remove pallet handling orders.";
                }
                //else
                //{
                //    Entities.Instruction loadInstruction = null;
                //    Entities.CollectDrop loadCollectDrop = null;
                //    BusinessRules.IJob burJob = new BusinessRules.Job();
                //    Entities.FacadeResult retVal = burJob.ValidateRemoveOrder(Job, order, out loadInstruction, out loadCollectDrop);
                //    if (retVal.Success)
                //    {
                //        imgRemoveOrder.Attributes.Add("onclick", "javascript:removeOrder(" + order.OrderID.ToString() + ");");
                //        imgRemoveOrder.Alt = "Remove order " + order.OrderID.ToString();
                //    }
                //    else
                //    {
                //        StringBuilder sb = new StringBuilder();
                //        sb.Append("");

                //        foreach (Entities.BusinessRuleInfringement bri in retVal.Infringements)
                //        {
                //            sb.Append(bri.Description);
                //        }

                //        imgRemoveOrder.Attributes.Add("onclick", "javascript:alert('You cannot remove order " + order.OrderID + " for the following reasons: " + sb.ToString() + "');");
                //        imgRemoveOrder.Src = "~/images/itxt_xButton_greyedout.GIF";
                //        imgRemoveOrder.Alt = "You cannot remove this order for the following reasons: " + sb.ToString();
                //    }
                //}

                #endregion

                #region Rate Details

                HtmlTableCell tdforeignRate = e.Item.FindControl("tdforeignRate") as HtmlTableCell;
                Label lblGBPRate = e.Item.FindControl("lblGBPRate") as Label;
                CultureInfo culture = null;

                if (order.LCID > 0) // Default to GBP
                    culture = new CultureInfo(order.LCID);
                else
                    culture = new CultureInfo(Orchestrator.Globals.Configuration.NativeCulture);

                if (lblGBPRate != null)
                    lblGBPRate.Text = order.Rate.ToString("C");

                if (tdforeignRate != null)
                {
                    tdforeignRate.Visible = Globals.Configuration.MultiCurrency;

                    if (tdforeignRate.Visible)
                    {
                        Label lblRate = e.Item.FindControl("lblRate") as Label;

                        if (lblRate != null)
                            lblRate.Text = order.ForeignRate.ToString("C", culture);
                    }
                }

                #endregion
            }
            else if (e.Item.ItemType == ListItemType.Footer)
            {
                #region Footer

                if (isDehireInstruction)
                {
                    HtmlTableCell ftDehireReciept = e.Item.FindControl("ftDehireReciept") as HtmlTableCell;
                    ftDehireReciept.Visible = true;
                }

                Label lblOrderCount = (Label)e.Item.FindControl("lblOrderCount");
                lblOrderCount.Text = "Orders: " + orderCount.ToString();

                Label lblPalletsTotal = e.Item.FindControl("lblPalletsTotal") as Label;
                lblPalletsTotal.Text = string.Format("{0} ({1})", palletsRunningTotal, palletsActualRunningTotal);

                HtmlTableCell frforeignRate = e.Item.FindControl("frforeignRate") as HtmlTableCell;

                if (frforeignRate != null)
                    frforeignRate.Visible = Globals.Configuration.MultiCurrency;

                orderCount = palletsActualRunningTotal = palletsRunningTotal = 0;

                #endregion
            }
        }

        protected void repOrderCollectDrops_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            // Specify the order handling totals.
            if (e.Item.ItemType == ListItemType.Footer)
            {
                #region Footer

                if (!multipleWeightCodes)
                {
                    Label lblWeightTotal = (Label)e.Item.FindControl("lblWeightTotal");
                    lblWeightTotal.Text = weightRunningTotal.ToString("F0");

                    if (hasActuals)
                    {
                        Label lblWeightActualTotal = (Label)e.Item.FindControl("lblWeightActualTotal");
                        lblWeightActualTotal.Text = "(" + weightActualRunningTotal.ToString("F0") + ")";
                    }
                }

                Label lblNoCasesTotal = (Label)e.Item.FindControl("lblNoCasesTotal");
                if (lblNoCasesTotal != null)
                    lblNoCasesTotal.Text = casesRunningTotal.ToString();

                if (hasActuals)
                {
                    Label lblNoCasesActualTotal = (Label)e.Item.FindControl("lblNoCasesActualTotal");
                    lblNoCasesActualTotal.Text = "(" + casesActualRunningTotal.ToString() + ")";
                }

                Label lblPalletsTotal = (Label)e.Item.FindControl("lblPalletsTotal");
                lblPalletsTotal.Text = palletsRunningTotal.ToString();

                if (hasActuals)
                {
                    Label lblPalletsActualTotal = (Label)e.Item.FindControl("lblPalletsActualTotal");
                    lblPalletsActualTotal.Text = "(" + palletsActualRunningTotal.ToString() + ")";
                }

                Label lblPalletSpacesTotal = (Label)e.Item.FindControl("lblPalletSpacesTotal");
                if (lblPalletSpacesTotal != null)
                    lblPalletSpacesTotal.Text = palletSpacesRunningTotal.ToString("F2");

                Label lblOrderCount = (Label)e.Item.FindControl("lblOrderCount");
                lblOrderCount.Text = "Orders: " + orderCount.ToString();

                HtmlTableCell frforeignRate = e.Item.FindControl("frforeignRate") as HtmlTableCell;

                if (frforeignRate != null)
                    frforeignRate.Visible = Globals.Configuration.MultiCurrency;

                orderCount = palletsRunningTotal = palletsActualRunningTotal = casesActualRunningTotal = casesRunningTotal = 0;
                palletSpacesRunningTotal = weightActualRunningTotal = weightRunningTotal = 0m;

                #endregion
            }

            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                #region Item

                Entities.CollectDrop collectDrop = (Entities.CollectDrop)e.Item.DataItem;

                orderCount++;

                weightRunningTotal += collectDrop.Weight;
                palletsRunningTotal += collectDrop.NoPallets;
                palletSpacesRunningTotal += collectDrop.Order.PalletSpaces;
                casesRunningTotal += collectDrop.NoCases;

                Label lblOrderAction = (Label)e.Item.FindControl("lblOrderAction");

                // Set the weight and pallet actuals
                Entities.Instruction relatedInstruction = null;
                foreach (Entities.Instruction instruction in Job.Instructions)
                {
                    relatedInstruction = instruction;
                    bool foundInstruction = false;

                    foreach (Entities.CollectDrop cd in instruction.CollectDrops)
                    {
                        if (cd.CollectDropId == collectDrop.CollectDropId)
                        {
                            foundInstruction = true;
                            break;
                        }
                        else
                            continue;
                    }

                    if (foundInstruction)
                        break;
                }

                if (relatedInstruction != null)
                {
                    eInstructionType instructionType = (eInstructionType)relatedInstruction.InstructionTypeId;

                    if (instructionType == eInstructionType.Load)
                        lblOrderAction.Text = "Collect";
                    else if (instructionType == eInstructionType.Drop)
                        lblOrderAction.Text = "Deliver";
                    else if (instructionType == eInstructionType.Trunk)
                        lblOrderAction.Text = collectDrop.OrderAction.ToString().Replace("_", " ");

                    Label lblWeightActual = (Label)e.Item.FindControl("lblWeightActual");
                    Label lblPalletsActual = (Label)e.Item.FindControl("lblPalletsActual");
                    Label lblNoCasesActual = (Label)e.Item.FindControl("lblNoCasesActual");

                    if (relatedInstruction.InstructionActuals != null && relatedInstruction.InstructionActuals.Count == 1)
                    {
                        foreach (Entities.CollectDropActual cda in relatedInstruction.InstructionActuals[0].CollectDropActuals)
                        {
                            if (cda.CollectDropId == collectDrop.CollectDropId)
                            // We have found the collect drop actual.
                            {
                                hasActuals = true;

                                lblWeightActual.Text = "(" + cda.Weight.ToString("F0") + ")";
                                lblWeightActual.Font.Bold = false;

                                weightActualRunningTotal += cda.Weight;

                                lblPalletsActual.Text = "(" + cda.NumberOfPallets.ToString() + ")";
                                lblPalletsActual.Font.Bold = false;

                                palletsActualRunningTotal += cda.NumberOfPallets;

                                lblNoCasesActual.Text = "(" + cda.NumberOfCases.ToString() + ")";
                                lblNoCasesActual.Font.Bold = false;

                                casesActualRunningTotal += cda.NumberOfCases;
                                break;
                            }
                        }
                    }
                }

                Entities.Order order = collectDrop.Order;
                HtmlImage imgRemoveOrder = (HtmlImage)e.Item.FindControl("imgRemoveOrder");
                HtmlImage imgOrderCollectionDeliveryNotes = (HtmlImage)e.Item.FindControl("imgOrderCollectionDeliveryNotes");
                System.Web.UI.WebControls.Image imgHasExtras = (System.Web.UI.WebControls.Image)e.Item.FindControl("imgHasExtra");

                // Check to see if the order is being invoiced
                Orchestrator.Facade.IOrder facOrder = new Orchestrator.Facade.Order();
                bool orderBeingInvoiced = facOrder.IsOrderBeingInvoiced(order.OrderID);

                // Show invoice link as appropriate. The invoice link becomes part of the order status text.
                int invoiceId = 0;
                if (order.OrderStatus == eOrderStatus.Invoiced)
                {
                    invoiceId = facOrder.ClientInvoiceID(order.OrderID);
                }

                PlaceHolder phStatus = (PlaceHolder)e.Item.FindControl("phOrderStatus");
                if (invoiceId > 0)
                {
                    string strInvoiceUrl = Orchestrator.Globals.Configuration.WebServer + order.PDFLocation;
                    HyperLink hypStatus = new HyperLink();
                    hypStatus.Target = "_blank";
                    hypStatus.NavigateUrl = strInvoiceUrl;
                    hypStatus.Text = string.Format("{0}({1})", order.OrderStatus.ToString(), invoiceId.ToString());
                    hypStatus.ToolTip = string.Concat("Click to view invoice ", invoiceId.ToString());
                    phStatus.Controls.Add(hypStatus);
                }
                else
                {
                    // If there is no invoice, just show the order status text.
                    Label lblStatus = new Label();
                    lblStatus.Text = order.OrderStatus.ToString();
                    phStatus.Controls.Add(lblStatus);
                }

                // Specify the remove order button as appropriate
                if (order.OrderStatus == eOrderStatus.Invoiced)
                {
                    imgRemoveOrder.Attributes.Add("onclick", "javascript:alert('You cannot remove this order because it has been invoiced.');");
                    imgRemoveOrder.Src = "~/images/itxt_xButton_greyedout.GIF";
                    imgRemoveOrder.Alt = "You cannot remove this order because it has been invoiced.";
                }
                else if (order.OrderStatus == eOrderStatus.Delivered && Job.Instructions != null && Job.Instructions.Exists(ins => ins.InstructionTypeId == (int)eInstructionType.Drop && ins.CollectDrops != null && ins.CollectDrops.Exists(cd => cd.OrderID == order.OrderID)))
                {
                    // That the order has been delivered and this is the delivery job, otherwise you should be able to remove the order from collection run.
                    imgRemoveOrder.Attributes.Add("onclick", "javascript:alert('You cannot remove this order because it has been delivered on this run.');");
                    imgRemoveOrder.Src = "~/images/itxt_xButton_greyedout.GIF";
                    imgRemoveOrder.Alt = "You cannot remove this order because it has been delivered.";
                }
                else if (order.OrderStatus == eOrderStatus.Cancelled)
                {
                    imgRemoveOrder.Attributes.Add("onclick", "javascript:alert('You cannot remove this order because it has been cancelled.');");
                    imgRemoveOrder.Src = "~/images/itxt_xButton_greyedout.GIF";
                    imgRemoveOrder.Alt = "You cannot remove this order because it has been cancelled.";
                }
                else if (orderBeingInvoiced)
                {
                    imgRemoveOrder.Attributes.Add("onclick", "javascript:alert('You cannot remove this order because it is being invoiced.');");
                    imgRemoveOrder.Src = "~/images/itxt_xButton_greyedout.GIF";
                    imgRemoveOrder.Alt = "You cannot remove this order because it is being invoiced.";
                }
                else
                {
                    Entities.Instruction loadInstruction = null;
                    Entities.CollectDrop loadCollectDrop = null;
                    BusinessRules.IJob burJob = new BusinessRules.Job();
                    Entities.FacadeResult retVal = burJob.ValidateRemoveOrder(Job, order, out loadInstruction, out loadCollectDrop);
                    if (retVal.Success)
                    {
                        imgRemoveOrder.Attributes.Add("onclick", "javascript:removeOrder(" + order.OrderID.ToString() + ");");
                        imgRemoveOrder.Alt = "Remove order " + order.OrderID.ToString();
                    }
                    else
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("");

                        foreach (Entities.BusinessRuleInfringement bri in retVal.Infringements)
                        {
                            sb.Append(bri.Description);
                        }

                        imgRemoveOrder.Attributes.Add("onclick", "javascript:alert('You cannot remove order " + order.OrderID + " for the following reasons: " + sb.ToString() + "');");
                        imgRemoveOrder.Src = "~/images/itxt_xButton_greyedout.GIF";
                        imgRemoveOrder.Alt = "You cannot remove this order for the following reasons: " + sb.ToString();
                    }
                }

                if (imgOrderCollectionDeliveryNotes != null)
                {
                    // if it is the orders 1st collection or orders delivery destination - show the collection / delivery note.
                    if (((order.CollectionPointID == relatedInstruction.PointID && !string.IsNullOrEmpty(order.CollectionNotes)) || (order.DeliveryPointID == relatedInstruction.PointID && !string.IsNullOrEmpty(order.DeliveryNotes))) && collectDrop.OrderAction == eOrderAction.Default)
                    {
                        bool isCollection = order.CollectionPointID == relatedInstruction.PointID ? true : false;
                        //string orderNotes = string.Format("Collection Notes<br/>{0}<p>Delivery Notes<br/>{1}</p>", order.CollectionNotes, order.DeliveryNotes);
                        //imgOrderCollectionDeliveryNotes.Attributes.Add("title", orderNotes);

                        string tooltip = "$('{0}').qtip({style: {name: 'dark'}content: {url:'/groupage/getOrderCollectionDeliveryNotes.aspx',data: {orderID:{0}, isCollection:{1},mthod: 'get'}});";




                        imgOrderCollectionDeliveryNotes.Attributes.Add("orderid", order.OrderID.ToString());
                        imgOrderCollectionDeliveryNotes.Attributes.Add("iscollection", isCollection.ToString());
                        imgOrderCollectionDeliveryNotes.Attributes.Add("rel", "/groupage/getOrderCollectionDeliveryNotes.aspx");

//                        imgOrderCollectionDeliveryNotes.Attributes.Add("onmouseover", "javascript:ShowOrderCollectionDeliveryNotes(this," + order.OrderID.ToString() + ",'" + isCollection.ToString() + "');");
  //                      imgOrderCollectionDeliveryNotes.Attributes.Add("onmouseout", "javascript:closeToolTip();");
    //                    imgOrderCollectionDeliveryNotes.Attributes.Add("class", "orchestratorLink");
                    }
                    else
                        imgOrderCollectionDeliveryNotes.Visible = false;
                }

                imgHasExtras.Visible = order.HasExtraAttached;

                HtmlTableCell tdforeignRate = e.Item.FindControl("tdforeignRate") as HtmlTableCell;
                Label lblGBPRate = e.Item.FindControl("lblGBPRate") as Label;
                CultureInfo culture = null;

                if (order.LCID > 0) // Default to GBP
                    culture = new CultureInfo(order.LCID);
                else
                    culture = new CultureInfo(Orchestrator.Globals.Configuration.NativeCulture);

                if (lblGBPRate != null)
                    lblGBPRate.Text = order.Rate.ToString("C");

                if (tdforeignRate != null)
                {
                    tdforeignRate.Visible = Globals.Configuration.MultiCurrency;

                    if (tdforeignRate.Visible)
                    {
                        Label lblRate = e.Item.FindControl("lblRate") as Label;

                        if (lblRate != null)
                            lblRate.Text = order.ForeignRate.ToString("C", culture);
                    }
                }

                HtmlAnchor hypOrderId = (HtmlAnchor)e.Item.FindControl("hypOrderId");
                hypOrderId.HRef = string.Format("javascript:{0}", this.dlgOrder.GetOpenDialogScript(string.Format("oid={0}", order.OrderID)));

                HtmlAnchor hypLoadNumber = (HtmlAnchor)e.Item.FindControl("hypLoadNumber");
                hypLoadNumber.HRef = string.Format("javascript:{0}", this.dlgOrder.GetOpenDialogScript(string.Format("oid={0}", order.OrderID)));

                HtmlAnchor hypDocketNumber = (HtmlAnchor)e.Item.FindControl("hypDocketNumber");
                hypDocketNumber.HRef = string.Format("javascript:{0}", this.dlgOrder.GetOpenDialogScript(string.Format("oid={0}", order.OrderID)));

                #endregion
            }

            if (e.Item.ItemType == ListItemType.Header)
            {
                #region Header

                Label lblRate = e.Item.FindControl("lblRate") as Label;
                HtmlTableCell trforeignRate = e.Item.FindControl("trforeignRate") as HtmlTableCell;

                if (lblRate != null && trforeignRate != null)
                    lblRate.Visible = trforeignRate.Visible = Globals.Configuration.MultiCurrency;

                #endregion
            }
        }

        protected void repTurnedAwayOrderCollectDrops_ItemCommand(object sender, RepeaterCommandEventArgs e)
        {
            switch (e.CommandName.ToLower())
            {
                case "removeattemptlater":
                    HtmlInputHidden hidInstructionId = (HtmlInputHidden)((RepeaterItem)e.Item.Parent.Parent.Parent.Parent).FindControl("hidInstructionId");
                    int instructionID = 0;
                    if (int.TryParse(hidInstructionId.Value, out instructionID))
                    {
                        Entities.Instruction instruction = m_job.Instructions.Find(
                            delegate(Entities.Instruction ins)
                            {
                                return ins.InstructionID == instructionID;
                            });

                        if (instruction != null)
                        {
                            Facade.IRedelivery facRedelivery = new Facade.Redelivery();
                            Entities.FacadeResult result = facRedelivery.RemoveOrderBasedRedelivery(m_job, instruction, ((Entities.CustomPrincipal)Page.User).UserName);

                            this.RefreshJobEntityCache();
                            PopulateJobControls();

                            // Let subscribers know that the move or merge has compeleted.
                            OnAfterMoveMergeCommand();
                        }
                    }
                    break;
            }
        }

        protected void repTurnedAwayOrderCollectDrops_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            // Specify the order handling totals.
            if (e.Item.ItemType == ListItemType.Footer)
            {
                #region Footer
                if (!multipleWeightCodes)
                {
                    Label lblWeightTotal = (Label)e.Item.FindControl("lblWeightTotal");
                    lblWeightTotal.Text = weightRunningTotal.ToString("F0");

                    if (hasActuals)
                    {
                        Label lblWeightActualTotal = (Label)e.Item.FindControl("lblWeightActualTotal");
                        lblWeightActualTotal.Text = "(" + weightActualRunningTotal.ToString("F0") + ")";
                    }
                }

                Label lblNoCasesTotal = (Label)e.Item.FindControl("lblNoCasesTotal");
                if (lblNoCasesTotal != null)
                    lblNoCasesTotal.Text = casesRunningTotal.ToString();

                if (hasActuals)
                {
                    Label lblNoCasesActualTotal = (Label)e.Item.FindControl("lblNoCasesActualTotal");
                    lblNoCasesActualTotal.Text = "(" + casesActualRunningTotal.ToString() + ")";
                }

                Label lblPalletsTotal = (Label)e.Item.FindControl("lblPalletsTotal");
                lblPalletsTotal.Text = palletsRunningTotal.ToString();

                if (hasActuals)
                {
                    Label lblPalletsActualTotal = (Label)e.Item.FindControl("lblPalletsActualTotal");
                    lblPalletsActualTotal.Text = "(" + palletsActualRunningTotal.ToString() + ")";
                }

                Label lblPalletSpacesTotal = (Label)e.Item.FindControl("lblPalletSpacesTotal");
                if (lblPalletSpacesTotal != null)
                    lblPalletSpacesTotal.Text = palletSpacesRunningTotal.ToString("F2");

                Label lblOrderCount = (Label)e.Item.FindControl("lblOrderCount");
                lblOrderCount.Text = "Orders: " + orderCount.ToString();

                orderCount = palletsRunningTotal = palletsActualRunningTotal = casesActualRunningTotal = casesRunningTotal = 0;
                palletSpacesRunningTotal = weightActualRunningTotal = weightRunningTotal = 0m;

                #endregion
            }
            else if (e.Item.ItemType == ListItemType.Header)
            {
                #region Header
                Entities.Instruction instruction = (Entities.Instruction)((RepeaterItem)e.Item.Parent.Parent.Parent.Parent).DataItem;

                ImageButton imgBtnRemoveAttemptLater = (ImageButton)e.Item.FindControl("imgBtnRemoveAttemptLater");
                imgBtnRemoveAttemptLater.Visible = false;

                if (instruction.Redelivery != null)
                {
                    BusinessRules.IJob burJob = new BusinessRules.Job();
                    Entities.FacadeResult validationResult = burJob.ValidateRemoveOrderBasedRedelivery(m_job, instruction);
                    imgBtnRemoveAttemptLater.Visible = true;
                    //imgBtnRemoveAttemptLater.Enabled = validationResult.Infringements.Count == 0;

                    if (validationResult.Infringements.Count > 0)
                    {
                        StringBuilder sb = new StringBuilder();
                        sb.Append("");

                        foreach (Entities.BusinessRuleInfringement validationInfringement in validationResult.Infringements)
                        {
                            sb.Append(@"\n  ");
                            sb.Append(validationInfringement.Description);
                        }

                        imgBtnRemoveAttemptLater.Attributes.Add("onclick", "javascript:alert('You cannot remove this refusal for the following reasons: " + sb.ToString() + "');return false;");
                        imgBtnRemoveAttemptLater.ToolTip = "You cannot remove this refusal for the following reasons: " + sb.ToString().Replace(@"\n  ", "");
                        imgBtnRemoveAttemptLater.ImageUrl = "~/images/itxt_xButton_greyedout.GIF";
                    }
                    else
                    {
                        imgBtnRemoveAttemptLater.Attributes.Add("onclick", "javascript:return confirm('Are you sure you want to remove this refusal?');");
                    }
                }
                #endregion
            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                #region Item
                Entities.RedeliveryOrder redeliveryOrder = (Entities.RedeliveryOrder)e.Item.DataItem;

                orderCount++;

                weightRunningTotal += redeliveryOrder.Order.Weight;
                casesRunningTotal += redeliveryOrder.Order.Cases;
                palletsRunningTotal += redeliveryOrder.Order.NoPallets;
                palletSpacesRunningTotal += redeliveryOrder.Order.PalletSpaces;

                Entities.Order order = redeliveryOrder.Order;

                // Check to see if the order is being invoiced
                Orchestrator.Facade.IOrder facOrder = new Orchestrator.Facade.Order();
                bool orderBeingInvoiced = facOrder.IsOrderBeingInvoiced(order.OrderID);

                // Show invoice link as appropriate. The invoice link becomes part of the order status text.
                int invoiceId = 0;
                if (order.OrderStatus == eOrderStatus.Invoiced)
                {
                    invoiceId = facOrder.ClientInvoiceID(order.OrderID);
                }

                PlaceHolder phStatus = (PlaceHolder)e.Item.FindControl("phOrderStatus");
                if (invoiceId > 0)
                {
                    string strInvoiceUrl = Orchestrator.Globals.Configuration.WebServer + order.PDFLocation;
                    HyperLink hypStatus = new HyperLink();
                    hypStatus.Target = "_blank";
                    hypStatus.NavigateUrl = strInvoiceUrl;
                    hypStatus.Text = string.Format("{0}({1})", order.OrderStatus.ToString(), invoiceId.ToString());
                    hypStatus.ToolTip = string.Concat("Click to view invoice ", invoiceId.ToString());
                    phStatus.Controls.Add(hypStatus);
                }
                else
                {
                    // If there is no invoice, just show the order status text.
                    Label lblStatus = new Label();
                    lblStatus.Text = order.OrderStatus.ToString();
                    phStatus.Controls.Add(lblStatus);
                }

                HtmlAnchor hypOrderId = (HtmlAnchor)e.Item.FindControl("hypOrderId");
                hypOrderId.HRef = string.Format("javascript:{0}", this.dlgOrder.GetOpenDialogScript(string.Format("oid={0}", order.OrderID)));

                HtmlAnchor hypLoadNumber = (HtmlAnchor)e.Item.FindControl("hypLoadNumber");
                hypLoadNumber.HRef = string.Format("javascript:{0}", this.dlgOrder.GetOpenDialogScript(string.Format("oid={0}", order.OrderID)));

                HtmlAnchor hypDocketNumber = (HtmlAnchor)e.Item.FindControl("hypDocketNumber");
                hypDocketNumber.HRef = string.Format("javascript:{0}", this.dlgOrder.GetOpenDialogScript(string.Format("oid={0}", order.OrderID)));

                #endregion
            }
        }

        protected void btnTipSheet_Click(object sender, EventArgs e)
        {
            NameValueCollection reportParams = new NameValueCollection();
            Facade.IJob facJob = new Facade.Job();
            Button btnTipSheet = sender as Button;

            if (btnTipSheet != null)
            {
                DataSet ds = facJob.GetTipSheet(btnTipSheet.CommandArgument, Orchestrator.Globals.Configuration.PalletNetworkID);

                ////-------------------------------------------------------------------------------------	
                ////									Load Report Section 
                ////-------------------------------------------------------------------------------------	
                Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.TippingSheet;
                Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;
                Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = ds;
                Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
                Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";

                // Show the user control
                Page.ClientScript.RegisterStartupScript(this.GetType(), "onload", "<script language=\"javascript\">window.open('" + Page.ResolveClientUrl("../reports/reportviewer.aspx?wiz=true") + "');</script>");
            }
        }

        private void AddJobEntityToCache(Entities.Job job)
        {
            if (Cache.Get("JobEntityForJobId" + m_jobId) != null)
            {
                Cache.Remove("JobEntityForJobId" + m_jobId);
            }

            Cache.Add("JobEntityForJobId" + m_jobId.ToString(),
                        job,
                        null,
                        Cache.NoAbsoluteExpiration,
                        TimeSpan.FromMinutes(5),
                        CacheItemPriority.Normal,
                        null);
        }

        private Entities.Job GetJobEntityFromCache()
        {
            Entities.Job job = (Entities.Job)Cache.Get("JobEntityForJobId" + m_jobId);

            if (job == null)
            {
                Facade.IJob facJob = new Facade.Job();
                Facade.IPCV facPCV = new Facade.PCV();
                Facade.IJobSubContractor facJobSubContractor = new Facade.Job();

                job = facJob.GetJob(m_jobId, true);
                job.Charge = ((Facade.IJobCharge)facJob).GetForJobId(m_jobId);
                job.Extras = facJob.GetExtras(m_jobId, true);
                job.PCVs = facPCV.GetForJobId(m_jobId);
                job.References = ((Facade.IJobReference)facJob).GetJobReferences(m_jobId);
                job.SubContractors = facJobSubContractor.GetSubContractorForJobId(m_jobId);

                AddJobEntityToCache(job);

                m_job = job;
            }

            return job;
        }

        private enum FindPreviousOrFollowingInstructionId : int
        {
            Previous = 0,
            Following = 1
        }

        /// <summary>
        /// Return the previous or following InstructionId or -1 if it cannot be found.
        /// </summary>
        /// <param name="instructionId"></param>
        /// <returns></returns>
        private int FindRelativeInstructionId(int rootInstructionId, FindPreviousOrFollowingInstructionId find)
        {
            if (_orderedInstructionIdList != null)
            {
                int index = 0;

                switch (find)
                {
                    case FindPreviousOrFollowingInstructionId.Previous:
                        index = _orderedInstructionIdList.IndexOf(rootInstructionId) - 1;
                        if (index >= 0 && index <= _orderedInstructionIdList.Count - 1)
                        {
                            return _orderedInstructionIdList[index];
                        }

                        break;

                    case FindPreviousOrFollowingInstructionId.Following:
                        index = _orderedInstructionIdList.IndexOf(rootInstructionId) + 1;
                        if (index >= 0 && index <= _orderedInstructionIdList.Count - 1)
                        {
                            return _orderedInstructionIdList[index];
                        }

                        break;

                    default:
                        break;
                }
            }

            return -1;
        }

        private Entities.Job RefreshJobEntityCache()
        {
            Entities.Job job = null;

            Facade.IJob facJob = new Facade.Job();
            Facade.IInstruction facInstruction = new Facade.Instruction();
            Facade.IPCV facPCV = new Facade.PCV();
            Facade.IJobSubContractor facJobSubContractor = new Facade.Job();

            job = facJob.GetJob(m_jobId);
            job.Charge = ((Facade.IJobCharge)facJob).GetForJobId(m_jobId);
            job.Extras = facJob.GetExtras(m_jobId, true);
            job.Instructions = facInstruction.GetForJobId(m_jobId);
            job.PCVs = facPCV.GetForJobId(m_jobId);
            job.References = ((Facade.IJobReference)facJob).GetJobReferences(m_jobId);
            job.SubContractors = facJobSubContractor.GetSubContractorForJobId(m_jobId);

            if (job != null)
            {
                AddJobEntityToCache(job);
                this.Job = job;
            }
            else
            {
                // System in inconsistent state, throw invalid operation ex.
                throw new InvalidOperationException();
            }

            return job;
        }

        #endregion

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);

        }

        /// <summary>
        ///		Required method for Designer support - do not modify
        ///		the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {

        }
        #endregion
    }
}
