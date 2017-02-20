using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Objects;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Services;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.Traffic
{
    public partial class changeBookedTimes : Orchestrator.Base.BasePage
    {
        List<int> SimpleInstructionIDs = new List<int>();
        private string vs_jobID = "vs_JobID";
        public int JobID
        {
            get { return ViewState[vs_jobID] == null ? -1 : (int)ViewState[vs_jobID]; }
            set { ViewState[vs_jobID] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                int jobId = int.Parse(Request.QueryString["jobId"]);
                JobID = jobId;
                PopulateInstructionTimes();
            }

            Page.ClientScript.RegisterStartupScript(this.GetType(), "Initialise Variables", "Sys.Application.add_load(function() { window.setTimeout(InitialiseOrchestratorPage, 300);});", true);
            if (SimpleInstructionIDs.Count > 0)
                hidSimpleInstructions.Value = SimpleInstructionIDs.ToCSVString();

            infrigementDisplay.Visible = false;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            btnCancel.Click += new EventHandler(btnCancel_Click);
            btnConfirm.Click += new EventHandler(btnConfirm_Click);
            repBookedDateTimes.ItemDataBound += new RepeaterItemEventHandler(repBookedDateTimes_ItemDataBound);

        }
        
        protected Dictionary<int[], DataSet> InstructionOrders
        {
            get
            {
                if (this.ViewState["InstructionOrders"] == null)
                {
                    this.ViewState["InstructionOrders"] = new Dictionary<int[], DataSet>();
                }
                return (Dictionary<int[], DataSet>)this.ViewState["InstructionOrders"];
            }
        }

        void repBookedDateTimes_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView drv = (DataRowView)e.Item.DataItem;
                bool hasActual = drv["InstructionActualId"] != DBNull.Value;

                RadDatePicker dteBookedDate = e.Item.FindControl("dteBookedDate") as RadDatePicker;
                RadTimePicker dteBookedTime = e.Item.FindControl("dteBookedTime") as RadTimePicker;

                RadDatePicker dteCollectionFromDate = e.Item.FindControl("dteCollectionFromDate") as RadDatePicker;
                RadTimePicker dteCollectionFromTime = e.Item.FindControl("dteCollectionFromTime") as RadTimePicker;
                RadDatePicker dteCollectionByDate = e.Item.FindControl("dteCollectionByDate") as RadDatePicker;
                RadTimePicker dteCollectionByTime = e.Item.FindControl("dteCollectionByTime") as RadTimePicker;

                Panel pnlTrunkInstruction = e.Item.FindControl("pnlTrunkInstruction") as Panel;
                Panel pnlOrderInstruction = e.Item.FindControl("pnlOrderInstruction") as Panel;
                RadGrid grdOrders = e.Item.FindControl("grdOrders") as RadGrid;

                HtmlInputRadioButton rdCollectionTimedBooking = e.Item.FindControl("rdCollectionTimedBooking") as HtmlInputRadioButton;
                HtmlInputRadioButton rdCollectionBookingWindow = e.Item.FindControl("rdCollectionBookingWindow") as HtmlInputRadioButton;
                HtmlInputRadioButton rdCollectionIsAnytime = e.Item.FindControl("rdCollectionIsAnytime") as HtmlInputRadioButton;
                HiddenField hidView = e.Item.FindControl("hidView") as HiddenField;

                DateTime bookedDateTime = (DateTime)drv["BookedDateTime"];
                bool isAnytime = (bool)drv["IsAnyTime"];

                dteBookedDate.SelectedDate = bookedDateTime;

                if (!isAnytime)
                    dteBookedTime.SelectedDate = bookedDateTime;

                // Can't alter the booked date time if an actual has been recorded.
                dteBookedDate.Enabled = !hasActual;
                dteBookedTime.Enabled = !hasActual;
                Panel pnlSimple = e.Item.FindControl("pnlSimple") as Panel;
                Panel pnlAdvanced = e.Item.FindControl("pnlAdvanced") as Panel;
                Panel pnlView = e.Item.FindControl("pnlSwitcher") as Panel;

                if ((int)drv["InstructionTypeId"] == 1 || (int)drv["InstructionTypeId"] == 2 || (int)drv["InstructionTypeId"] == 3
                    || (int)drv["InstructionTypeId"] == 4 || (int)drv["InstructionTypeId"] == 5)
                {

                    pnlOrderInstruction.Visible = true;
                    pnlTrunkInstruction.Visible = false;
                    Facade.IOrder facOrder = new Facade.Order();
                    DataSet ds = facOrder.GetForInstructionID((int)drv["InstructionId"]);
                    InstructionOrders.Add(new int[2] { (int)drv["InstructionId"], (int)drv["InstructionTypeId"] }, ds);
                    grdOrders.DataSource = ds;
                    grdOrders.ItemDataBound += new GridItemEventHandler(grdOrders_ItemDataBound);
                    grdOrders.DataBind();
                    if (hasActual)
                        grdOrders.Enabled = false;

                    Label lblInstructionDate = e.Item.FindControl("lblInstructionDateTime") as Label;
                    lblInstructionDate.Text = GetInstructionBookedDateTime((int)drv["InstructionId"], (int)drv["InstructionTypeId"]);
                    DateTime FromDate, ToDate;
                    switch ((int)drv["InstructionTypeID"])
                    {
                        case 1:
                        case 5:

                            GetLoadTimeForInstruction((int)drv["InstructionID"], out FromDate, out   ToDate);
                            dteCollectionFromDate.SelectedDate = FromDate;
                            dteCollectionFromTime.SelectedDate = FromDate;
                            dteCollectionByDate.SelectedDate = ToDate;
                            dteCollectionByTime.SelectedDate = ToDate;
                            if (FromDate == ToDate)
                            {
                                rdCollectionTimedBooking.Checked = true;
                                HtmlInputHidden hidCollectionFromTimeRestoreValue = e.Item.FindControl("hidCollectionFromTimeRestoreValue") as HtmlInputHidden;
                                hidCollectionFromTimeRestoreValue.Value = FromDate.ToString("HH:mm");

                                Page.ClientScript.RegisterStartupScript(this.GetType(), rdCollectionTimedBooking.ClientID, "Sys.Application.add_load(function() { collectionTimedBooking($('#" + rdCollectionTimedBooking.ClientID + "')[0]);});", true);
                            }
                            else if (FromDate.Date == ToDate.Date && FromDate.Hour == 00 && ToDate.Hour == 23 && ToDate.Minute == 59)
                            {
                                rdCollectionIsAnytime.Checked = true;
                                Page.ClientScript.RegisterStartupScript(this.GetType(), rdCollectionIsAnytime.ClientID, "Sys.Application.add_load(function() { collectionIsAnytime($('#" + rdCollectionIsAnytime.ClientID + "')[0]);});", true);
                            }
                            else
                            {
                                rdCollectionBookingWindow.Checked = true;
                                HtmlInputHidden hidCollectionByTimeRestoreValue = e.Item.FindControl("hidCollectionByTimeRestoreValue") as HtmlInputHidden;
                                hidCollectionByTimeRestoreValue.Value = ToDate.ToString("HH:mm");
                                HtmlInputHidden hidCollectionFromTimeRestoreValue = e.Item.FindControl("hidCollectionFromTimeRestoreValue") as HtmlInputHidden;
                                hidCollectionFromTimeRestoreValue.Value = FromDate.ToString("HH:mm");
                                Page.ClientScript.RegisterStartupScript(this.GetType(), rdCollectionBookingWindow.ClientID, "Sys.Application.add_load(function() { collectionBookingWindow($('#" + rdCollectionBookingWindow.ClientID + "')[0]);});", true);
                            }
                            break;
                        case 2:
                        case 3:
                        case 4:
                            GetDeliveryTimeForInstruction((int)drv["InstructionID"], out FromDate, out   ToDate);
                            dteCollectionFromDate.SelectedDate = FromDate;
                            dteCollectionFromTime.SelectedDate = FromDate;
                            dteCollectionByDate.SelectedDate = ToDate;
                            dteCollectionByTime.SelectedDate = ToDate;
                            if (FromDate == ToDate)
                            {
                                rdCollectionTimedBooking.Checked = true;
                                HtmlInputHidden hidCollectionFromTimeRestoreValue = e.Item.FindControl("hidCollectionFromTimeRestoreValue") as HtmlInputHidden;
                                hidCollectionFromTimeRestoreValue.Value = FromDate.ToString("HH:mm");
                                Page.ClientScript.RegisterStartupScript(this.GetType(), rdCollectionTimedBooking.ClientID, "Sys.Application.add_load(function() { collectionTimedBooking($('#" + rdCollectionTimedBooking.ClientID + "')[0]);});", true);
                            }
                            else if (FromDate.Date == ToDate.Date && FromDate.Hour == 00 && ToDate.Hour == 23 && ToDate.Minute == 59)
                            {
                                rdCollectionIsAnytime.Checked = true;
                                Page.ClientScript.RegisterStartupScript(this.GetType(), rdCollectionIsAnytime.ClientID, "Sys.Application.add_load(function() { collectionIsAnytime($('#" + rdCollectionIsAnytime.ClientID + "')[0]);});", true);
                            }
                            else
                            {
                                rdCollectionBookingWindow.Checked = true;
                                HtmlInputHidden hidCollectionByTimeRestoreValue = e.Item.FindControl("hidCollectionByTimeRestoreValue") as HtmlInputHidden;
                                hidCollectionByTimeRestoreValue.Value = ToDate.ToString("HH:mm");
                                HtmlInputHidden hidCollectionFromTimeRestoreValue = e.Item.FindControl("hidCollectionFromTimeRestoreValue") as HtmlInputHidden;
                                hidCollectionFromTimeRestoreValue.Value = FromDate.ToString("HH:mm");

                                Page.ClientScript.RegisterStartupScript(this.GetType(), rdCollectionBookingWindow.ClientID, "Sys.Application.add_load(function() { collectionBookingWindow($('#" + rdCollectionBookingWindow.ClientID + "')[0]);});", true);
                            }
                            break;
                    }
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        // offer to show the advanced view
                        hidView.Value = "advanced";

                        // if the orders have a mixture of time types we should show the advanced by default?
                        bool showSimple = true;

                        DateTime CollectDateTime = DateTime.MinValue, CollectByDateTime = DateTime.MinValue, DeliverFromDateTime = DateTime.MinValue, DeliverToDateTime = DateTime.MinValue;
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            if ((int)drv["InstructionTypeId"] == 1 || (int)drv["InstructionTypeId"] == 5)
                            {

                                // check the collection date times
                                if (CollectDateTime == DateTime.MinValue)
                                {
                                    CollectDateTime = (DateTime)row["CollectionDateTime"];
                                    CollectByDateTime = (DateTime)row["CollectionByDateTime"];
                                }
                                else
                                {
                                    // Check to see if these are the same
                                    if ((DateTime)row["CollectionDateTime"] == CollectDateTime && (DateTime)row["CollectionByDateTime"] == CollectByDateTime)
                                    {

                                    }
                                    else
                                    {
                                        showSimple = false;
                                        break;
                                    }
                                }
                            }

                            if ((int)drv["InstructionTypeId"] == 2 || (int)drv["InstructionTypeId"] == 3 || (int)drv["InstructionTypeId"] == 4)
                            {

                                // check the collection date times
                                if (DeliverFromDateTime == DateTime.MinValue)
                                {
                                    DeliverFromDateTime = (DateTime)row["DeliveryFromDateTime"];
                                    DeliverToDateTime = (DateTime)row["DeliveryDateTime"];
                                }
                                else
                                {
                                    // Check to see if these are the same
                                    if ((DateTime)row["DeliveryFromDateTime"] == DeliverFromDateTime && (DateTime)row["DeliveryDateTime"] == DeliverToDateTime)
                                    {

                                    }
                                    else
                                    {
                                        showSimple = false;
                                        break;
                                    }
                                }
                            }
                        }
                        if (showSimple)
                            SimpleInstructionIDs.Add((int)drv["InstructionId"]);
                    }
                }
                else
                {
                    pnlSimple.Visible = false;
                    pnlAdvanced.Visible = false;
                    pnlView.Visible = false;
                }
            }
        }

        protected string GetInstructionBookedDateTime(int instructionID, int InstructionTypeID)
        {
            string retVal = string.Empty;
            DateTime CollectFromDateTime, CollectByDateTime, DeliverFromDateTime, DeliverToDateTime;

            if (InstructionTypeID == (int)eInstructionType.Load || InstructionTypeID == (int)eInstructionType.PickupPallets)
                retVal = GetLoadTimeForInstruction(instructionID, out CollectFromDateTime, out CollectByDateTime);
            else if (InstructionTypeID == (int)eInstructionType.Drop || InstructionTypeID == (int)eInstructionType.DeHirePallets || InstructionTypeID == (int)eInstructionType.LeavePallets)
                retVal = GetDeliveryTimeForInstruction(instructionID, out DeliverFromDateTime, out DeliverToDateTime);

            return retVal;
        }
        
        private string GetLoadTimeForInstruction(int instructionID, out DateTime FromDateTime, out DateTime ToDateTime)
        {
            DateTime earliestLoadDateTime = DateTime.MaxValue;
            DateTime latestLoadDateTime = DateTime.MaxValue;

            DateTime CollectFromDateTime;
            DateTime CollectToDateTime;
            bool IsTimedBooking = false;
            IsTimedBooking = false;
            foreach (KeyValuePair<int[], DataSet> kvp in InstructionOrders)
            {
                if (kvp.Key[0] == instructionID)
                {
                    int rowCount = 0;
                    foreach (DataRow row in kvp.Value.Tables[0].Rows)
                    {
                        if (kvp.Key[1] == (int)eInstructionType.Load || kvp.Key[1] == (int)eInstructionType.PickupPallets)
                        {
                            CollectFromDateTime = (DateTime)row["CollectionDateTime"];
                            CollectToDateTime = (DateTime)row["CollectionByDateTime"];

                            if (rowCount == 0)
                            {
                                earliestLoadDateTime = CollectFromDateTime;
                                latestLoadDateTime = CollectToDateTime;
                                if (earliestLoadDateTime == latestLoadDateTime)
                                    IsTimedBooking = true;
                            }
                            else if (CollectFromDateTime == CollectToDateTime)
                            {
                                // Timed booking [check to ensure we encapsulate this]
                                if (CollectFromDateTime < earliestLoadDateTime)
                                    earliestLoadDateTime = CollectFromDateTime;
                                if (CollectToDateTime > latestLoadDateTime)
                                    latestLoadDateTime = CollectToDateTime;
                            }
                            else
                            {
                                if (CollectFromDateTime.Date == CollectToDateTime.Date && CollectFromDateTime.TimeOfDay.Hours == 0 && CollectToDateTime.TimeOfDay.Hours == 23)
                                {
                                    // Any time 
                                }
                                else
                                {
                                    if (!IsTimedBooking)
                                    {
                                        // Look for the most restrictive
                                        if (CollectFromDateTime < earliestLoadDateTime)
                                            earliestLoadDateTime = CollectFromDateTime;
                                        if (CollectToDateTime < latestLoadDateTime)
                                            latestLoadDateTime = CollectToDateTime;
                                    }
                                }
                            }
                        }
                        rowCount++;
                    }
                }
            }
            FromDateTime = earliestLoadDateTime;
            ToDateTime = latestLoadDateTime;
            string retVal = string.Empty;

            if (earliestLoadDateTime.Date == latestLoadDateTime.Date && earliestLoadDateTime.TimeOfDay == new TimeSpan(0, 0, 0) && latestLoadDateTime.TimeOfDay.Hours == 23 && latestLoadDateTime.Minute == 59)
            {
                // Any Time
                retVal = earliestLoadDateTime.ToString("dd/MM/yy") + " Anytime";
            }
            else if (earliestLoadDateTime.Date == latestLoadDateTime.Date && earliestLoadDateTime.TimeOfDay == latestLoadDateTime.TimeOfDay)
            {
                // time booking
                retVal = earliestLoadDateTime.ToString("dd/MM/yy HH:mm");
            }
            else
            {
                // Booking Window
                retVal = string.Format("{0} to {1}", earliestLoadDateTime.ToString("dd/MM HH:mm"), latestLoadDateTime.ToString("dd/MM HH:mm"));
            }
            return retVal;
        }

        private string GetDeliveryTimeForInstruction(int instructionID, out DateTime FromDateTime, out DateTime ToDateTime)
        {
            DateTime earliestDeliveryDateTime = DateTime.MaxValue;
            DateTime latestDeliveryDateTime = DateTime.MaxValue;

            DateTime DeliverFromDateTime;
            DateTime DeliverToDateTime;
            bool IsTimedBooking = false;
            foreach (KeyValuePair<int[], DataSet> kvp in InstructionOrders)
            {
                if (kvp.Key[0] == instructionID)
                {
                    int rowCount = 0;
                    foreach (DataRow row in kvp.Value.Tables[0].Rows)
                    {
                        if (kvp.Key[1] == (int)eInstructionType.Drop || kvp.Key[1] == (int)eInstructionType.DeHirePallets || kvp.Key[1] == (int)eInstructionType.LeavePallets)
                        {
                            DeliverFromDateTime = (DateTime)row["DeliveryFromDateTime"];
                            DeliverToDateTime = (DateTime)row["DeliveryDateTime"];

                            if (rowCount == 0)
                            {
                                earliestDeliveryDateTime = DeliverFromDateTime;
                                latestDeliveryDateTime = DeliverToDateTime;
                                if (DeliverFromDateTime == DeliverToDateTime)
                                    IsTimedBooking = true;
                            }
                            else if (DeliverFromDateTime == DeliverToDateTime)
                            {
                                // Timed booking [check to ensure we encapsulate this]
                                if (DeliverFromDateTime < earliestDeliveryDateTime)
                                    earliestDeliveryDateTime = DeliverFromDateTime;
                                if (DeliverToDateTime > latestDeliveryDateTime)
                                    latestDeliveryDateTime = DeliverToDateTime;
                            }
                            else
                            {
                                if (DeliverFromDateTime.Date == DeliverToDateTime.Date && DeliverFromDateTime.TimeOfDay.Hours == 0 && DeliverToDateTime.TimeOfDay.Hours == 23)
                                {
                                    // Any time 
                                }
                                else
                                {
                                    if (!IsTimedBooking) // if there is a timed booking on here dont worry about the window
                                    {
                                        // Look for the most restrictive
                                        if (DeliverFromDateTime < earliestDeliveryDateTime)
                                            earliestDeliveryDateTime = DeliverFromDateTime;
                                        if (DeliverToDateTime < latestDeliveryDateTime)
                                            latestDeliveryDateTime = DeliverToDateTime;
                                    }
                                }
                            }
                        }
                        rowCount++;
                    }
                }
            }
            FromDateTime = earliestDeliveryDateTime;
            ToDateTime = latestDeliveryDateTime;

            string retVal = string.Empty;

            if (earliestDeliveryDateTime.Date == latestDeliveryDateTime.Date && earliestDeliveryDateTime.TimeOfDay == new TimeSpan(0, 0, 0) && latestDeliveryDateTime.TimeOfDay.Hours == 23 && latestDeliveryDateTime.TimeOfDay.Minutes == 59)
            {
                // Any Time
                retVal = earliestDeliveryDateTime.ToString("dd/MM/yy") + " Anytime";
            }
            else if (earliestDeliveryDateTime.Date == latestDeliveryDateTime.Date && earliestDeliveryDateTime.TimeOfDay == latestDeliveryDateTime.TimeOfDay)
            {
                // time booking
                retVal = earliestDeliveryDateTime.ToString("dd/MM/yy HH:mm");
            }
            else
            {
                // Booking Window
                retVal = string.Format("{0} to {1}", earliestDeliveryDateTime.ToString("dd/MM HH:mm"), latestDeliveryDateTime.ToString("dd/MM HH:mm"));
            }
            return retVal;
        }

        private OrchDateTime GetEarliestTimeForInstruction(int instructionID)
        {

            DateTime earliestLoadDateTime = DateTime.MinValue;
            DateTime latestLoadDateTime = DateTime.MaxValue;
            foreach (KeyValuePair<int[], DataSet> kvp in InstructionOrders)
            {
                if (kvp.Key[0] == instructionID)
                {
                    foreach (DataRow row in kvp.Value.Tables[0].Rows)
                    {
                        if (kvp.Key[1] == (int)eInstructionType.Load || kvp.Key[1] == (int)eInstructionType.PickupPallets)
                        {
                            if ((DateTime)row["CollectionDateTime"] > earliestLoadDateTime)
                                earliestLoadDateTime = (DateTime)row["CollectionDateTime"];
                            if ((DateTime)row["CollectionByDateTime"] < latestLoadDateTime)
                                latestLoadDateTime = (DateTime)row["CollectionByDateTime"];
                        }
                        else if (kvp.Key[1] == (int)eInstructionType.Drop || kvp.Key[1] == (int)eInstructionType.LeavePallets || kvp.Key[1] == (int)eInstructionType.DeHirePallets)
                        {
                            if ((DateTime)row["DeliveryFromDateTime"] > earliestLoadDateTime)
                                earliestLoadDateTime = (DateTime)row["DeliveryFromDateTime"];
                            if ((DateTime)row["DeliveryDateTime"] < latestLoadDateTime)
                                latestLoadDateTime = (DateTime)row["DeliveryDateTime"];
                        }
                    }
                }
            }

            OrchDateTime retVal = new OrchDateTime();

            if (earliestLoadDateTime.Date == latestLoadDateTime.Date && earliestLoadDateTime.TimeOfDay == new TimeSpan(0, 0, 0) && latestLoadDateTime.TimeOfDay == new TimeSpan(23, 59, 59))
            {
                // Any Time
                retVal.DateTime = earliestLoadDateTime;
                retVal.IsAnyTime = true;
            }
            else if (earliestLoadDateTime.Date == latestLoadDateTime.Date && earliestLoadDateTime.TimeOfDay == latestLoadDateTime.TimeOfDay)
            {
                // time booking
                retVal.DateTime = earliestLoadDateTime;
            }
            else
            {
                // Booking Window
                retVal.DateTime = earliestLoadDateTime;
            }
            return retVal;
        }

        void grdOrders_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                DataRowView drv = (DataRowView)e.Item.DataItem;
                int instructionTypeID = (int)drv["InstructionTypeId"];
                TextBox txtCollectAt = e.Item.FindControl("txtCollectAtDate") as TextBox;
                TextBox txtCollectAtTime = e.Item.FindControl("txtCollectAtTime") as TextBox;
                TextBox txtDeliverAtFrom = e.Item.FindControl("txtDeliverAtFromDate") as TextBox;
                TextBox txtDeliverAtFromTime = e.Item.FindControl("txtDeliverAtFromTime") as TextBox;
                TextBox txtDeliverAtBy = e.Item.FindControl("txtDeliverByFromDate") as TextBox;
                TextBox txtDeliverAtByTime = e.Item.FindControl("txtDeliverByFromTime") as TextBox;
                TextBox txtCollectBy = e.Item.FindControl("txtCollectByDate") as TextBox;
                TextBox txtCollectByTime = e.Item.FindControl("txtCollectByTime") as TextBox;
                HiddenField hidOrderID = e.Item.FindControl("hidOrderChanged") as HiddenField;
                int orderID = (int)drv["OrderID"];
                e.Item.Attributes.Add("orderid", orderID.ToString());

                if (instructionTypeID == 1 || instructionTypeID == 5)
                {
                    //only show loading window
                    e.Item.OwnerTableView.Columns.FindByUniqueName("DeliverTo").Visible = false;
                    e.Item.OwnerTableView.Columns.FindByUniqueName("DeliverAt").Visible = false;

                    rimApproveOrder.GetSettingByBehaviorID("DateInputBehaviour").TargetControls.Add(new TargetInput(txtCollectAt.UniqueID, true));
                    rimApproveOrder.GetSettingByBehaviorID("DateInputBehaviour").TargetControls.Add(new TargetInput(txtCollectBy.UniqueID, true));
                    rimApproveOrder.GetSettingByBehaviorID("TimeInputBehaviour").TargetControls.Add(new TargetInput(txtCollectAtTime.UniqueID, true));
                    rimApproveOrder.GetSettingByBehaviorID("TimeInputBehaviour").TargetControls.Add(new TargetInput(txtCollectByTime.UniqueID, true));

                    txtCollectAt.Text = ((DateTime)drv["CollectionDateTime"]).ToString("dd/MM/yy");
                    txtCollectAtTime.Text = ((DateTime)drv["CollectionDateTime"]).ToString("HH:mm");
                    txtCollectBy.Text = ((DateTime)drv["CollectionByDateTime"]).ToString("dd/MM/yy");
                    txtCollectByTime.Text = ((DateTime)drv["CollectionByDateTime"]).ToString("HH:mm");

                    RadioButtonList rblCollectionTimeOptions = e.Item.FindControl("rblCollectionTimeOptions") as RadioButtonList;
                    if (txtCollectAt.Text == txtCollectBy.Text && txtCollectAtTime.Text == "00:00"
                        && txtCollectByTime.Text == "23:59")
                    {
                        // Any Time
                        rblCollectionTimeOptions.Items.FindByValue("2").Selected = true;
                        txtCollectBy.Style.Add("display", "none");
                        txtCollectByTime.Style.Add("display", "none");
                        txtCollectAtTime.Text = string.Empty;
                    }
                    else if (txtCollectAt.Text == txtCollectBy.Text && txtCollectAtTime.Text == txtCollectByTime.Text)
                    {
                        // time booking
                        rblCollectionTimeOptions.Items.FindByValue("1").Selected = true;
                        txtCollectBy.Style.Add("display", "none");
                        txtCollectByTime.Style.Add("display", "none");
                    }
                    else
                    {
                        // Booking Window
                        rblCollectionTimeOptions.Items.FindByValue("0").Selected = true;
                        txtCollectBy.Style.Add("display", "");
                        txtCollectByTime.Style.Add("display", "");
                    }
                }
                else if (instructionTypeID == 2 || instructionTypeID == 3 || instructionTypeID == 4)
                {
                    //only show delivery window
                    e.Item.OwnerTableView.Columns.FindByUniqueName("CollectFrom").Visible = false;
                    e.Item.OwnerTableView.Columns.FindByUniqueName("CollectAt").Visible = false;

                    rimApproveOrder.GetSettingByBehaviorID("DateInputBehaviour").TargetControls.Add(new TargetInput(txtDeliverAtFrom.UniqueID, true));
                    rimApproveOrder.GetSettingByBehaviorID("DateInputBehaviour").TargetControls.Add(new TargetInput(txtDeliverAtBy.UniqueID, true));
                    rimApproveOrder.GetSettingByBehaviorID("TimeInputBehaviour").TargetControls.Add(new TargetInput(txtDeliverAtByTime.UniqueID, true));
                    rimApproveOrder.GetSettingByBehaviorID("TimeInputBehaviour").TargetControls.Add(new TargetInput(txtDeliverAtFromTime.UniqueID, true));

                    txtDeliverAtFrom.Text = ((DateTime)drv["DeliveryFromDateTime"]).ToString("dd/MM/yy");
                    txtDeliverAtFromTime.Text = ((DateTime)drv["DeliveryFromDateTime"]).ToString("HH:mm");
                    txtDeliverAtBy.Text = ((DateTime)drv["DeliveryDateTime"]).ToString("dd/MM/yy");
                    txtDeliverAtByTime.Text = ((DateTime)drv["DeliveryDateTime"]).ToString("HH:mm");

                    RadioButtonList rblDeliveryTimeOptions = e.Item.FindControl("rblDeliveryTimeOptions") as RadioButtonList;
                    if (txtDeliverAtFrom.Text == txtDeliverAtBy.Text && txtDeliverAtFromTime.Text == "00:00"
                        && txtDeliverAtByTime.Text == "23:59")
                    {
                        // Any Time
                        rblDeliveryTimeOptions.Items.FindByValue("2").Selected = true;
                        txtDeliverAtBy.Style.Add("display", "none");
                        txtDeliverAtByTime.Style.Add("display", "none");
                        txtDeliverAtFromTime.Text = string.Empty;
                    }
                    else if (txtDeliverAtFrom.Text == txtDeliverAtBy.Text && txtDeliverAtFromTime.Text == txtDeliverAtByTime.Text)
                    {
                        // time booking
                        rblDeliveryTimeOptions.Items.FindByValue("1").Selected = true;
                        txtDeliverAtBy.Style.Add("display", "none");
                        txtDeliverAtByTime.Style.Add("display", "none");
                    }
                    else
                    {
                        // Booking Window
                        rblDeliveryTimeOptions.Items.FindByValue("0").Selected = true;
                        txtDeliverAtBy.Style.Add("display", "");
                        txtDeliverAtByTime.Style.Add("display", "");
                    }
                }
            }
        }

        private Entities.InstructionCollection PopulateInstructions()
        {
            Entities.InstructionCollection instructions = new Entities.InstructionCollection();

            if (JobID > 0)
            {
                Facade.Instruction facInstruction = new Orchestrator.Facade.Instruction();
                Entities.InstructionCollection originalInstructions = facInstruction.GetForJobId(JobID, false);

                foreach (RepeaterItem item in repBookedDateTimes.Items)
                    if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                    {

                        bool trunkOrAttemptedDeliveryCalledIn = false;

                        int instructionId = int.Parse(((HtmlInputHidden)item.FindControl("hidInstructionId")).Value);
                        int instructionTypeId = int.Parse(((HtmlInputHidden)item.FindControl("hidInstructionTypeId")).Value);

                        RadDateInput bookedDate = item.FindControl("dteBookedDate") as RadDateInput;
                        RadDateInput bookedTime = item.FindControl("dteBookedTime") as RadDateInput;

                        //Either retrieve the original instruction or create a shell instruction to hold the key pieces of information.
                        Entities.Instruction thisInstruction = originalInstructions.Exists(i => i.InstructionID == instructionId) ? originalInstructions.Find(i => i.InstructionID == instructionId) : new Entities.Instruction();
                        // Configure the new date time values.
                        if (instructionTypeId != (int)eInstructionType.Load && instructionTypeId != (int)eInstructionType.Drop && instructionTypeId != (int)eInstructionType.PickupPallets
                            && instructionTypeId != (int)eInstructionType.DeHirePallets && instructionTypeId != (int)eInstructionType.LeavePallets)
                        {

                            if (bookedDate != null && bookedTime != null) // These are NULL if the Trunk/Attempted Delivery is called-in.
                            {
                                DateTime bookedDateTime = bookedDate.SelectedDate.Value;
                                bookedDateTime = bookedDateTime.Subtract(bookedDateTime.TimeOfDay);
                                bool isAnytime = false;

                                if (!bookedTime.SelectedDate.HasValue)
                                {
                                    bookedDateTime = bookedDateTime.Add(new TimeSpan(23, 59, 59));
                                    isAnytime = true;
                                }
                                else
                                    bookedDateTime = bookedDateTime.Add(bookedTime.SelectedDate.Value.TimeOfDay);

                                // Update the instructions date and time.

                                thisInstruction.BookedDateTime = bookedDateTime;
                                thisInstruction.IsAnyTime = isAnytime;
                            }
                            else
                                trunkOrAttemptedDeliveryCalledIn = true;
                        }
                        else
                        {
                            // get tthe earliest time for the order on the instruction to ensure that the ordering does not get screwed
                            OrchDateTime orchDateTime = GetEarliestTimeForInstruction(instructionId);
                            thisInstruction.BookedDateTime = orchDateTime.DateTime;
                            thisInstruction.IsAnyTime = orchDateTime.IsAnyTime;
                        }

                        if(!trunkOrAttemptedDeliveryCalledIn)
                            instructions.Add(thisInstruction);
                    }
            }
            return instructions;
        }

        private void PopulateInstructionTimes()
        {
            if (JobID > 0)
            {
                // Retrieve the instruction times for this job.
                Facade.IInstruction facInstruction = new Facade.Instruction();
                var instructionsDataSet = facInstruction.GetInstructionsForAlteringBookedTimes(JobID);

                repBookedDateTimes.DataSource = instructionsDataSet;
                repBookedDateTimes.DataBind();

                if (Globals.Configuration.CollectionTimeDeliveryWindow)
                {
                    var instructions = instructionsDataSet.Tables[0].AsEnumerable();
                    var collects = instructions.Where(dr => dr.Field<int>("InstructionTypeID") == (int)eInstructionType.Load);

                    // Only set up collection time recalculation if the last collection point on the run has a delivery matrix
                    if (collects.Any())
                    {
                        var lastCollectDeliveryMatrixID = collects.Last().Field<int?>("PointDeliveryMatrixID");

                        if (lastCollectDeliveryMatrixID.HasValue && lastCollectDeliveryMatrixID > 0)
                        {
                            var firstDropFound = false;

                            for (var i = 0; i < instructions.Count(); i++)
                            {
                                var instruction = instructions.ElementAt(i);
                                var instructionRow = (HtmlTableRow)repBookedDateTimes.Items[i].FindControl("rowInstruction");
                                var cssClass = instructionRow.Attributes["class"];

                                cssClass += string.Format(" instructionRow_{0}", instruction.Field<int>("InstructionID"));

                                if (!firstDropFound && instruction.Field<int>("InstructionTypeID") == (int)eInstructionType.Drop)
                                {
                                    firstDropFound = true;
                                    cssClass += " firstDrop";
                                }

                                instructionRow.Attributes["class"] += cssClass.TrimStart();
                            }
                        }
                    }
                }
            }
        }

        #region Button Events

        void btnConfirm_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                SaveChages();
            }
        }

        private void SaveChages()
        {
            string ordersAsText = hidOrderIDs.Value;
            string[] orderIDs = ordersAsText.Split(',');
            List<string> orders = orderIDs.ToList();

            string instructionIDsAsText = hidInstructionIDs.Value;
            List<string> instructions = instructionIDsAsText.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            foreach (RepeaterItem ri in repBookedDateTimes.Items)
            {
                RadGrid grdOrders = ri.FindControl("grdOrders") as RadGrid;
                int instructionTypeId = int.Parse(((HtmlInputHidden)ri.FindControl("hidInstructionTypeId")).Value);
                int instructionID = int.Parse(((HtmlInputHidden)ri.FindControl("hidInstructionId")).Value);
                Panel pnlSimple = ri.FindControl("pnlSimple") as Panel;
                Panel pnlAdvanced = ri.FindControl("pnlAdvanced") as Panel;
                HiddenField hidView = ri.FindControl("hidView") as HiddenField;

                if (hidView.Value == "advanced")
                {
                    foreach (Telerik.Web.UI.GridItem item in grdOrders.Items)
                    {
                        if (item is GridDataItem)
                        {
                            if (orders.Contains(item.Attributes["orderid"]))
                            {
                                int orderID = int.Parse(item.Attributes["orderid"]);
                                // get the information required for the update;
                                if (instructionTypeId == 1 || instructionTypeId == 5)
                                {
                                    if (item.OwnerTableView.Columns.FindByUniqueName("CollectAt").Visible)
                                    {
                                        DateTime collectFromDate = DateTime.Parse(((TextBox)item.FindControl("txtCollectAtDate")).Text);
                                        DateTime? collectFromTime = null;
                                        if (((TextBox)item.FindControl("txtCollectAtTime")).Text != "")
                                            collectFromTime = DateTime.Parse(((TextBox)item.FindControl("txtCollectAtTime")).Text);

                                        DateTime collectByDate = DateTime.Parse(((TextBox)item.FindControl("txtCollectByDate")).Text);
                                        DateTime? collectByTime = null;
                                        if (((TextBox)item.FindControl("txtCollectByTime")).Text != "")
                                            collectByTime = DateTime.Parse(((TextBox)item.FindControl("txtCollectByTime")).Text);

                                        RadioButtonList rblCollectionDateType = item.FindControl("rblCollectionTimeOptions") as RadioButtonList;

                                        if (int.Parse(rblCollectionDateType.SelectedValue) == 2)
                                        {
                                            collectByDate = collectFromDate;
                                            collectFromTime = new DateTime(collectByTime.Value.Year, collectByTime.Value.Month, collectByTime.Value.Day, 0, 0, 0);
                                            collectByTime = new DateTime(collectByTime.Value.Year, collectByTime.Value.Month, collectByTime.Value.Day, 23, 59, 0);
                                        }
                                        UpdateOrderCollection(orderID, collectFromDate, collectFromTime, collectByDate, collectByTime, int.Parse(rblCollectionDateType.SelectedValue), Page.User.Identity.Name);
                                    }
                                }

                                if (instructionTypeId == 2 || instructionTypeId == 3 || instructionTypeId == 4)
                                {
                                    DateTime deliverAtFromDate = DateTime.Parse(((TextBox)item.FindControl("txtDeliverAtFromDate")).Text);
                                    DateTime? deliverAtFromTime = null;
                                    if (((TextBox)item.FindControl("txtDeliverAtFromTime")).Text != "")
                                        deliverAtFromTime = DateTime.Parse(((TextBox)item.FindControl("txtDeliverAtFromTime")).Text);

                                    DateTime deliverByDate = DateTime.Parse(((TextBox)item.FindControl("txtDeliverByFromDate")).Text);
                                    DateTime? deliverByTime = null;
                                    if (((TextBox)item.FindControl("txtDeliverByFromTime")).Text != "")
                                        deliverByTime = DateTime.Parse(((TextBox)item.FindControl("txtDeliverByFromTime")).Text);

                                    RadioButtonList rblDeliveryDateType = item.FindControl("rblDeliveryTimeOptions") as RadioButtonList;

                                    if (int.Parse(rblDeliveryDateType.SelectedValue) == 2)
                                    {
                                        deliverByDate = deliverAtFromDate;
                                        deliverAtFromTime = new DateTime(deliverByTime.Value.Year, deliverByTime.Value.Month, deliverByTime.Value.Day, 0, 0, 0);
                                        deliverByTime = new DateTime(deliverByTime.Value.Year, deliverByTime.Value.Month, deliverByTime.Value.Day, 23, 59, 0);
                                    }
                                    UpdateOrderDelivery(orderID, deliverAtFromDate, deliverAtFromTime, deliverByDate, deliverByTime, int.Parse(rblDeliveryDateType.SelectedValue), Page.User.Identity.Name);
                                }
                            }
                        }
                    }
                }
                else
                {
                    // ensure that there has been a change made to the dates before saving to prevent accidental changes
                    // use the same date for all the orders on the instruction.
                    Facade.IOrder facOrder = new Facade.Order();
                    DataSet dsOrders = facOrder.GetForInstructionID(instructionID);
                    foreach (DataRow row in dsOrders.Tables[0].Rows)
                    {
                        DateTime collectFromDate = ((RadDatePicker)ri.FindControl("dteCollectionFromDate")).SelectedDate.Value;
                        DateTime collectFromTime = ((RadTimePicker)ri.FindControl("dteCollectionFromTime")).SelectedDate.Value;
                        DateTime collectByDate = ((RadDatePicker)ri.FindControl("dteCollectionByDate")).SelectedDate.Value;
                        DateTime collectByTime = ((RadTimePicker)ri.FindControl("dteCollectionByTime")).SelectedDate.Value;
                        HtmlInputRadioButton rdCollectionTimedBooking = (HtmlInputRadioButton)ri.FindControl("rdCollectionTimedBooking");
                        HtmlInputRadioButton rdCollectionBookingWindow = (HtmlInputRadioButton)ri.FindControl("rdCollectionBookingWindow");
                        HtmlInputRadioButton rdCollectionIsAnytime = (HtmlInputRadioButton)ri.FindControl("rdCollectionIsAnytime");

                        if (rdCollectionIsAnytime.Checked)
                        {
                            collectByDate = collectFromDate;
                            collectFromTime = new DateTime(collectByTime.Year, collectByTime.Month, collectByTime.Day, 0, 0, 0);
                            collectByTime = new DateTime(collectByTime.Year, collectByTime.Month, collectByTime.Day, 23, 59, 0);
                        }
                        int collectionTimeType = 1;
                        if (rdCollectionBookingWindow.Checked)
                            collectionTimeType = 0;
                        if (rdCollectionIsAnytime.Checked)
                            collectionTimeType = 2;
                        if (rdCollectionTimedBooking.Checked)
                            collectionTimeType = 1;

                        if (instructionTypeId == 1 || instructionTypeId == 5)
                        {
                            UpdateOrderCollection((int)row["OrderID"], collectFromDate, collectFromTime, collectByDate, collectByTime, collectionTimeType, Page.User.Identity.Name);
                            foreach (KeyValuePair<int[], DataSet> kvp in InstructionOrders)
                                if (kvp.Key[0] == instructionID)
                                    foreach (DataRow dr in kvp.Value.Tables[0].Rows)
                                        if ((int)dr["OrderId"] == (int)row["OrderID"])
                                        {
                                            dr["CollectionDateTime"] = collectFromDate.Date.Add(collectFromTime.TimeOfDay);
                                            dr["CollectionByDateTime"] = collectByDate.Date.Add(collectByTime.TimeOfDay);
                                        }
                        }

                        if (instructionTypeId == 2 || instructionTypeId == 3 || instructionTypeId == 4)
                        {
                            UpdateOrderDelivery((int)row["OrderID"], collectFromDate, collectFromTime, collectByDate, collectByTime, collectionTimeType, Page.User.Identity.Name);
                            foreach (KeyValuePair<int[], DataSet> kvp in InstructionOrders)
                                if (kvp.Key[0] == instructionID)
                                    foreach (DataRow dr in kvp.Value.Tables[0].Rows)
                                        if ((int)dr["OrderId"] == (int)row["OrderID"])
                                        {
                                            dr["DeliveryFromDateTime"] = collectFromDate.Date.Add(collectFromTime.TimeOfDay);
                                            dr["DeliveryDateTime"] = collectByDate.Date.Add(collectByTime.TimeOfDay);
                                        }
                        }
                    }
                }
            }

            int jobId = int.Parse(Request.QueryString["jobId"]);

            Facade.IJob facJob = new Facade.Job();
            Entities.InstructionCollection instructionCollection = PopulateInstructions();

            if (instructionCollection.Count > 0)
            {
                DateTime lastUpdateDate = DateTime.Parse(Request.QueryString["LastUpdateDate"]);
                Entities.FacadeResult result = facJob.UpdateBookedTimes(jobId, instructionCollection, chkAlterLegsToReflectNewTimes.Checked, lastUpdateDate, ((Entities.CustomPrincipal)Page.User).UserName);


                if (result.Success)
                {
                    // Successful update return and refresh the grid.
                    this.ReturnValue = "refresh";
                    this.Close();
                    
                }
                else
                {
                    //// Display infringments to user.
                    this.ReturnValue = "refresh";
                    this.Close();
                }
            }
            else
            {
                // nothing to update
                this.Close();
            }
        }

        #region Methods for storing changes

        public List<string> UpdateOrder(int orderID, DateTime collectFromDate, DateTime? collectFromTime, DateTime collectByDate, DateTime? collectByTime, int collectionTimeType,
                                                    DateTime deliverFromDate, DateTime? deliverFromTime, DateTime deliverToDate, DateTime? deliverToTime, int deliveryTimeType, string userID)
        {
            List<string> result = new List<string>();

            result.Add(true.ToString());

            Orchestrator.EF.DataContext data = EF.DataContext.Current;
            Orchestrator.EF.Order order = data.OrderSet.First(o => o.OrderId == orderID);

            //determine the date(s) to use

            order.CollectionDateTime = collectFromDate.Add(collectFromTime.HasValue ? new TimeSpan(collectFromTime.Value.Hour, collectFromTime.Value.Minute, 0) : new TimeSpan(23, 59, 0));
            if (collectionTimeType == 1)
                order.CollectionByDateTime = order.CollectionDateTime;
            else
                order.CollectionByDateTime = collectByDate.Add(collectByTime.HasValue ? new TimeSpan(collectByTime.Value.Hour, collectByTime.Value.Minute, 0) : new TimeSpan(23, 59, 0));

            order.DeliveryFromDateTime = deliverFromDate.Add(deliverFromTime.HasValue ? new TimeSpan(deliverFromTime.Value.Hour, deliverFromTime.Value.Minute, 0) : new TimeSpan(23, 59, 0));
            if (deliveryTimeType == 1)
                order.DeliveryDateTime = order.DeliveryFromDateTime.Value;
            else
                order.DeliveryDateTime = deliverToDate.Add(deliverToTime.HasValue ? new TimeSpan(deliverToTime.Value.Hour, deliverToTime.Value.Minute, 0) : new TimeSpan(23, 59, 0));

            order.LastUpdateDate = DateTime.Now;
            order.LastUpdateUserID = userID;

            Facade.IExchangeRates facER = new Facade.ExchangeRates();
            CultureInfo nativeCulture = new CultureInfo(Orchestrator.Globals.Configuration.NativeCulture);

            if (order.LCID != nativeCulture.LCID)
            {
                order.ExchangeRateID = facER.GetCurrentExchangeRateID(Facade.Culture.GetCurrencySymbol(order.LCID.Value), order.CollectionDateTime);
                order.Rate = facER.GetConvertedRate(order.ExchangeRateID.Value, order.ForeignRate.Value);
            }
            else
                order.Rate = decimal.Round(order.ForeignRate.Value, 4, MidpointRounding.AwayFromZero);
            try
            {
                data.SaveChanges();
            }
            catch
            {
                data.Refresh(RefreshMode.ClientWins, order);
            }
            result.Add(orderID.ToString());

            return result;
        }

        public List<string> UpdateOrderCollection(int orderID, DateTime collectFromDate, DateTime? collectFromTime, DateTime collectByDate, DateTime? collectByTime, int collectionTimeType,
                                                   string userID)
        {
            List<string> result = new List<string>();

            result.Add(true.ToString());

            Orchestrator.EF.DataContext data = EF.DataContext.Current;
            Orchestrator.EF.Order order = data.OrderSet.First(o => o.OrderId == orderID);

            //determine the date(s) to use
            if (collectionTimeType == 0)
            {
                // Window
                collectFromDate = collectFromDate.Subtract(collectFromDate.TimeOfDay).Add(collectFromTime.Value.TimeOfDay);
                collectByDate = collectByDate.Subtract(collectByDate.TimeOfDay).Add(collectByTime.Value.TimeOfDay);
            }
            if (collectionTimeType == 1)
            {
                // Timed
                collectFromDate = collectFromDate.Subtract(collectFromDate.TimeOfDay);
                collectFromDate = collectFromDate.Add(collectFromTime.Value.TimeOfDay);
                collectByDate = collectFromDate;
            }
            if (collectionTimeType == 2)
            {
                // Anytime
                collectFromDate = collectFromDate.Subtract(collectFromDate.TimeOfDay);
                collectByDate = collectFromDate;
                collectByDate = collectByDate.Add(new TimeSpan(23, 59, 59));
            }

            order.CollectionDateTime = collectFromDate;
            order.CollectionByDateTime = collectByDate;

            // Set the CollectionIsAnytime flag appropriately (we are not using the Data Access Layer which would do this automatically).
            order.CollectionIsAnytime = collectionTimeType == 2;

            order.LastUpdateDate = DateTime.Now;
            order.LastUpdateUserID = userID;

            try
            {
                data.SaveChanges();
            }
            catch
            {
                data.Refresh(RefreshMode.ClientWins, order);
            }

            result.Add(orderID.ToString());

            return result;
        }

        public List<string> UpdateOrderDelivery(int orderID, DateTime deliverFromDate, DateTime? deliverFromTime, DateTime deliverToDate, DateTime? deliverToTime, int deliveryTimeType, string userID)
        {
            List<string> result = new List<string>();

            result.Add(true.ToString());

            Orchestrator.EF.DataContext data = EF.DataContext.Current;
            Orchestrator.EF.Order order = data.OrderSet.First(o => o.OrderId == orderID);

            //determine the date(s) to use
            if (deliveryTimeType == 0)
            {
                // Window
                deliverFromDate = deliverFromDate.Subtract(deliverFromDate.TimeOfDay).Add(deliverFromTime.Value.TimeOfDay);
                deliverToDate = deliverToDate.Subtract(deliverToDate.TimeOfDay).Add(deliverToTime.Value.TimeOfDay);

            }
            if (deliveryTimeType == 1)
            {
                // Timed
                deliverFromDate = deliverFromDate.Subtract(deliverFromDate.TimeOfDay);
                deliverFromDate = deliverFromDate.Add(deliverFromTime.Value.TimeOfDay);
                deliverToDate = deliverFromDate;
            }
            if (deliveryTimeType == 2)
            {
                // Anytime

                deliverToTime = DateTime.Today.Add(new TimeSpan(23, 59, 0));
                deliverFromDate = deliverFromDate.Subtract(deliverFromDate.TimeOfDay);
                deliverToDate = deliverFromDate.Add(deliverToTime.Value.TimeOfDay);
            }

            order.DeliveryFromDateTime = deliverFromDate;
            order.DeliveryDateTime = deliverToDate;

            // Set the DeliveryIsAnyTime flag appropriately (we are not using the Data Access Layer which would do this automatically).
            order.DeliveryIsAnytime = deliveryTimeType == 2;
            order.LastUpdateDate = DateTime.Now;
            order.LastUpdateUserID = userID;

            try
            {
                data.SaveChanges();
            }
            catch
            {
                data.Refresh(RefreshMode.ClientWins, order);
            }

            result.Add(orderID.ToString());

            return result;
        }

        #endregion Methods for storing changes

        void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Date Validation
        void cfvDelivery_ServerValidate(object source, ServerValidateEventArgs args)
        {
        }
        #endregion

        #region Page Methods

        [WebMethod]
        public static Facade.DeliveryWindow.CalculatedCollectionTimes RecalculateCollectionTimes(int jobID, DateTime earliestDropDateTime, int earliestDropPointID)
        {
            try
            {
                var facDeliveryWindow = new Facade.DeliveryWindow();
                return facDeliveryWindow.CalculateCollectionTime(jobID, earliestDropDateTime, earliestDropPointID);
            }
            catch (Exception ex)
            {
                WebUI.Global.UnhandledException(ex);
                throw;
            }
        }

        #endregion Page Methods
    }

    public struct OrchDateTime
    {
        public DateTime DateTime;
        public bool IsAnyTime;
    }
}
