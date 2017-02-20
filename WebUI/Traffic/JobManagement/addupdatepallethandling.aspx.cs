using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using System.IO;
using System.Text;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

using Telerik.Web.UI;

namespace Orchestrator.WebUI.Traffic.JobManagement
{
    public partial class addupdatepallethandling : Orchestrator.Base.BasePage
    {
        #region Private Variables

        private const string C_JOB_VS = "C_JOB_VS";
        private int m_jobId = 0;
        private Entities.Job m_job = null;              // The job as loaded when the page is first visited.

        private bool m_isUpdateable = true;
        private int m_emptyPalletCount = 0;
        IEnumerable<KeyValuePair<int, string>> currentResources = null;

        private int m_palletInstructionID = 0;          // Instruction Id of the Pallet Handling instruction on the job.
        protected int m_PalletIdentityId = 0;			// The id of the organisation we are going to send the pallets to.
        protected string m_PalletTown = String.Empty;	// The description of the town we are going to send the pallets to.
        protected int m_PalletTownId = 0;			    // The id of the town we are going to send the pallets to.
        protected int m_PalletPointId = 0;			    // The id of the point we are going to send the pallets to.

        #endregion

        #region Properties

        private const string vs_palletDeliveries = "vs_palletDeliveries";
        protected List<Entities.PalletDelivery> PalletDeliveries
        {
            get
            {
                if (ViewState[vs_palletDeliveries] == null)
                {
                    if (m_job.Instructions.Exists(ins => ins.InstructionTypeId == (int)eInstructionType.LeavePallets || ins.InstructionTypeId == (int)eInstructionType.DeHirePallets))
                    {
                        List<Entities.Instruction> palletHandlingInstructions = m_job.Instructions.FindAll(ins => ins.InstructionTypeId == (int)eInstructionType.LeavePallets || ins.InstructionTypeId == (int)eInstructionType.DeHirePallets).ToList();
                        List<Entities.PalletDelivery> lPDeliveries = new List<Entities.PalletDelivery>();

                        // Build up Pallet Deliveries.
                        foreach (Entities.Instruction ins in palletHandlingInstructions)
                            foreach (Entities.CollectDrop cd in ins.CollectDrops)
                            {
                                Entities.PalletDelivery currentPd = new Entities.PalletDelivery();

                                currentPd.PalletOrder = cd.Order;
                                currentPd.InstructionID = ins.InstructionID;
                                currentPd.CollectDropID = cd.CollectDropId;

                                currentPd.PalletAction = (eInstructionType)ins.InstructionTypeId;
                                currentPd.PalletActionDescription = Utilities.UnCamelCase(((eInstructionType)ins.InstructionTypeId).ToString());

                                currentPd.IsFixedUnit = false;
                                currentPd.PalletType = cd.PalletType;

                                if (ins.Trailer != null)
                                {
                                    currentPd.ResourceID = ins.Trailer.ResourceId;
                                    currentPd.Resource = ins.Trailer.TrailerRef;
                                }

                                currentPd.Destination = ins.Point.Description;

                                lPDeliveries.Add(currentPd);
                            }

                        ViewState[vs_palletDeliveries] = lPDeliveries;

                    }
                    else
                        ViewState[vs_palletDeliveries] = new List<Entities.PalletDelivery>();
                }

                return (List<Entities.PalletDelivery>)ViewState[vs_palletDeliveries];
            }
            set { ViewState[vs_palletDeliveries] = value; }
        }

        private const string vs_updatedatePalletDeliveries = "vs_updatedatePalletDeliveries";
        protected List<Entities.PalletDelivery> UpdatedPalletDeliveries
        {
            get
            {
                if (ViewState[vs_updatedatePalletDeliveries] == null)
                    ViewState[vs_updatedatePalletDeliveries] = new List<Entities.PalletDelivery>();

                return (List<Entities.PalletDelivery>)ViewState[vs_updatedatePalletDeliveries];
            }
            set { ViewState[vs_updatedatePalletDeliveries] = value; }
        }

        private const string vs_selectedPalletDelivery = "vs_selectedPalletDelivery";
        protected Entities.PalletDelivery SelectedPalletDelivery
        {
            get
            {
                if (ViewState[vs_selectedPalletDelivery] == null)
                    ViewState[vs_selectedPalletDelivery] = new Entities.PalletDelivery();

                return (Entities.PalletDelivery)ViewState[vs_selectedPalletDelivery];
            }
            set { ViewState[vs_selectedPalletDelivery] = value; }
        }

        #endregion

        #region Page Load

        protected void Page_Load(object sender, EventArgs e)
        {
            m_jobId = int.Parse(Request.QueryString["jobId"]);

            if (!IsPostBack)
                ConfigureDisplay();
            else
                m_job = (Entities.Job)ViewState[C_JOB_VS];

            idErrors.Visible = false;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            btnCancel.Click += new EventHandler(btnCancel_Click);
            btnGenerate.Click += new EventHandler(btnGenerate_Click);
            btnCancelUpdate.Click += new EventHandler(btnCancelUpdate_Click);
            btnUpdate.Click += new EventHandler(btnUpdate_Click);
            btnGenerateUpdate.Click += new EventHandler(btnGenerateUpdate_Click);

            lvEmptyPallets.ItemDataBound += new EventHandler<ListViewItemEventArgs>(lvEmptyPallets_ItemDataBound);
            lvExisingPalletHandling.ItemDataBound += new EventHandler<ListViewItemEventArgs>(lvExisingPalletHandling_ItemDataBound);

            rcbDeHireOrganisation.ItemsRequested += new RadComboBoxItemsRequestedEventHandler(rcbDeHireOrganisation_ItemsRequested);
        }

        #endregion

        #region Private Methods

        private void ConfigureDisplay()
        {
            // Bind the handle method
            rcbPalletHandlingAction.Items.Clear();
            rcbPalletHandlingAction.Items.Add(new RadComboBoxItem(Utilities.UnCamelCase(eInstructionType.LeavePallets.ToString()), ((int)eInstructionType.LeavePallets).ToString()));
            rcbPalletHandlingAction.Items.Add(new RadComboBoxItem(Utilities.UnCamelCase(eInstructionType.DeHirePallets.ToString()), ((int)eInstructionType.DeHirePallets).ToString()));
            rcbPalletHandlingAction.ClearSelection();

            // Setting currency culture
            CultureInfo systemCulture = new CultureInfo(Orchestrator.Globals.Configuration.NativeCulture);
            rntPalletDeliveryCharge.Culture = systemCulture;

            ClearPalletHandling();

            // Bind the job information.
            LoadJob();
            LoadEmptyPallets(null);

            if (m_emptyPalletCount == 0)
                palletButtonBar.Style.Add("display", "none");

            btnCancelUpdate.Style.Add("display", "none");
            LoadExistingPalletHandling(null);

            btnGenerateUpdate.Visible = !PalletDeliveries.Exists(lpds => lpds.IsDirty || lpds.PalletOrder.OrderID < 1);
        }

        private void ClearPalletHandling()
        {
            rcbPalletHandlingAction.SelectedIndex = 0;
            rcbDeHireOrganisation.ClearSelection();
            rcbDeHireOrganisation.Text = string.Empty;
            ucDeliveryPoint.Reset();

            //Delivery Date
            dteDeliveryFromDate.SelectedDate = null;
            dteDeliveryByDate.SelectedDate = null;
            dteDeliveryByTime.SelectedDate = null;
            dteDeliveryFromTime.SelectedDate = null;

            // Anytime
            rdDeliveryIsAnytime.Checked = true;
            rdDeliveryTimedBooking.Checked = false;
            rdDeliveryBookingWindow.Checked = false;

            dteDeliveryFromTime.Enabled = false;
            hidDeliveryTimingMethod.Value = "anytime";

            rntPalletDeliveryCharge.Value = null;
        }

        private void LoadJob()
        {
            Facade.IJob facJob = new Facade.Job();
            Facade.IInstruction facInstruction = new Facade.Instruction();

            m_job = facJob.GetJob(m_jobId, true, true);
            ViewState[C_JOB_VS] = m_job;
        }

        private Entities.Order GeneratePalletOrder(int palletTypeId, int noPallets, eInstructionType selectedPalletAction, ref string destination)
        {
            destination = string.Empty;

            int identityId = 0;
            bool deliveryIsAnytime = false;
            DateTime deliveryFromDateTime = new DateTime();
            DateTime deliveryDateTime = new DateTime();

            if (selectedPalletAction == eInstructionType.DeHirePallets)
                identityId = int.Parse(rcbDeHireOrganisation.SelectedValue);
            else
                identityId = Orchestrator.Globals.Configuration.IdentityId;

            deliveryFromDateTime = new DateTime(dteDeliveryFromDate.SelectedDate.Value.Year, dteDeliveryFromDate.SelectedDate.Value.Month, dteDeliveryFromDate.SelectedDate.Value.Day, dteDeliveryFromTime.SelectedDate.Value.Hour, dteDeliveryFromTime.SelectedDate.Value.Minute, 0);
            deliveryDateTime = new DateTime(dteDeliveryByDate.SelectedDate.Value.Year, dteDeliveryByDate.SelectedDate.Value.Month, dteDeliveryByDate.SelectedDate.Value.Day, dteDeliveryByTime.SelectedDate.Value.Hour, dteDeliveryByTime.SelectedDate.Value.Minute, 0);
            deliveryIsAnytime = rdDeliveryIsAnytime.Checked;

            Entities.Order palletOrder = new Entities.Order();

            // Setting default values for the Order.
            palletOrder.Cases = 0;
            palletOrder.Weight = 0;
            palletOrder.GoodsTypeID = 1; // This REALLY needs to be Pallets ( hint hint system definition ).
            palletOrder.PalletSpaceID = (int)ePalletSize.Normal;
            palletOrder.CustomerOrderNumber = "palletHandling";
            palletOrder.DeliveryOrderNumber = "palletHandling";
            palletOrder.OrderStatus = eOrderStatus.Approved;
            palletOrder.OrderType = eOrderType.PalletHandling;
            palletOrder.OrderInstructionID = 3; // Find out what this is?

            palletOrder.CustomerIdentityID = identityId;

            palletOrder.PalletTypeID = palletTypeId;
            palletOrder.NoPallets = noPallets;
            palletOrder.PalletSpaces = palletOrder.NoPallets;

            palletOrder.PlannedForCollection = palletOrder.PlannedForDelivery = true;
            palletOrder.CollectionPointID = palletOrder.DeliveryPointID = ucDeliveryPoint.SelectedPoint.PointId;
            destination = ucDeliveryPoint.SelectedPoint.Description;

            palletOrder.ForeignRate = palletOrder.Rate = rntPalletDeliveryCharge.Value.HasValue ? (decimal)rntPalletDeliveryCharge.Value : 0m;

            palletOrder.CollectionDateTime = palletOrder.DeliveryFromDateTime = deliveryFromDateTime;
            palletOrder.CollectionByDateTime = palletOrder.DeliveryDateTime = deliveryDateTime;
            palletOrder.CollectionIsAnytime = palletOrder.DeliveryIsAnytime = deliveryIsAnytime;

            palletOrder.FuelSurchargePercentage = 0m;
            palletOrder.FuelSurchargeAmount = palletOrder.FuelSurchargeForeignAmount = 0m;

            return palletOrder;
        }

        #region ListView Population

        private void LoadEmptyPallets(Entities.PalletDelivery selectedPalletDelivery)
        {
            Facade.IJob facJob = new Facade.Job();
            DataSet ds = facJob.GetUnhandledPalletsForJob(m_jobId, null);

            // If this is an update, then add the selected orders pallets to the unhandled pallets available - as this orders pallets will be updated.
            if (selectedPalletDelivery != null && selectedPalletDelivery.PalletOrder.OrderID > 0)
            {
                var existingRows = ds.Tables[0].Rows.Cast<DataRow>().AsEnumerable();
                DataRow dr = existingRows.FirstOrDefault(ldr => ldr.Field<int>("PalletTypeID") == selectedPalletDelivery.PalletTypeID && ldr.Field<int>("ResourceID") == selectedPalletDelivery.ResourceID);

                if (dr == null)
                {
                    #region Insert New Row

                    dr = ds.Tables[0].NewRow();
                    dr.SetField<string>("PalletType", selectedPalletDelivery.PalletType);
                    dr.SetField<int>("PalletTypeID", selectedPalletDelivery.PalletTypeID);

                    int currentPallets = selectedPalletDelivery.UpdatedNoPallets > 0 ? selectedPalletDelivery.UpdatedNoPallets : selectedPalletDelivery.NoOfPallets;

                    dr.SetField<int>("NoOfPallets", currentPallets);
                    dr.SetField<int>("TrailerPallets", selectedPalletDelivery.NoOfPallets);
                    dr.SetField<int>("HandledPallets", currentPallets);
                    dr.SetField<int>("CollectDropID", selectedPalletDelivery.CollectDropID);
                    dr.SetField<int>("ResourceID", selectedPalletDelivery.ResourceID);
                    dr.SetField<string>("Resource", selectedPalletDelivery.Resource);
                    dr.SetField<bool>("IsFixedUnit", selectedPalletDelivery.IsFixedUnit);
                    ds.Tables[0].Rows.Add(dr);

                    #endregion
                }
                else
                {
                    int currentPallets = dr.Field<int>("NoOfPallets");
                    currentPallets += selectedPalletDelivery.UpdatedNoPallets > 0 ? selectedPalletDelivery.NoOfPallets - selectedPalletDelivery.UpdatedNoPallets : 0;
                    dr.SetField<int>("NoOfPallets", currentPallets);
                }
            }

            // Check for any updated pallet handling instructions.
            foreach (Entities.PalletDelivery pd in PalletDeliveries.Where(cpd => cpd.IsDirty))//&& (selectedPalletDelivery == null || cpd.PalletOrder.OrderID != selectedPalletDelivery.PalletOrder.OrderID)))
            {
                var existingRows = ds.Tables[0].Rows.Cast<DataRow>().AsEnumerable();
                DataRow dr = existingRows.FirstOrDefault(ldr => ldr.Field<int>("PalletTypeID") == pd.PalletTypeID && ldr.Field<int>("ResourceID") == pd.ResourceID);

                if (dr == null)
                {
                    #region Insert New Row

                    dr = ds.Tables[0].NewRow();
                    dr.SetField<string>("PalletType", pd.PalletType);
                    dr.SetField<int>("PalletTypeID", pd.PalletTypeID);

                    int currentPallets = pd.UpdatedNoPallets > 0 ? pd.NoOfPallets - pd.UpdatedNoPallets : pd.NoOfPallets;

                    dr.SetField<int>("NoOfPallets", currentPallets);
                    dr.SetField<int>("TrailerPallets", currentPallets);
                    dr.SetField<int>("HandledPallets", currentPallets);
                    dr.SetField<int>("CollectDropID", pd.CollectDropID);
                    dr.SetField<int>("ResourceID", pd.ResourceID);
                    dr.SetField<string>("Resource", pd.Resource);
                    dr.SetField<bool>("IsFixedUnit", pd.IsFixedUnit);
                    ds.Tables[0].Rows.Add(dr);

                    #endregion
                }
                else
                {
                    int currentPallets = dr.Field<int>("NoOfPallets");
                    currentPallets += (pd.NoOfPallets - pd.UpdatedNoPallets); //pd.UpdatedNoPallets >= 0 ? pd.NoOfPallets - pd.UpdatedNoPallets : 0;
                    dr.SetField<int>("NoOfPallets", currentPallets);
                }
            }

            // Cast Datarows as Enumerated List
            var returndRows = ds.Tables[0].Rows.Cast<DataRow>().AsEnumerable();

            // If supplied with an existing pallet delivery, find this from the data rows provided.
            if (selectedPalletDelivery != null)
                returndRows = returndRows.Where(rr => rr.Field<int>("ResourceID") == selectedPalletDelivery.ResourceID && rr.Field<int>("PalletTypeID") == selectedPalletDelivery.PalletTypeID);

            // If there are still unhandled pallets and this is not a update.
            if (returndRows.Count() > 0 && selectedPalletDelivery == null)
            {
                var pendingPalletDeliveries = PalletDeliveries.Where(ppd => ppd.PalletOrder.OrderID < 1);

                if (pendingPalletDeliveries.Count() > 0)
                    foreach (DataRow dr in returndRows)
                    {
                        int resourceID = dr.Field<int>("ResourceID");
                        int palletTypeId = dr.Field<int>("PalletTypeID");

                        var foundPalletDeliveries = pendingPalletDeliveries.Where(ppd => ppd.ResourceID == resourceID && ppd.PalletTypeID == palletTypeId);

                        foreach (Entities.PalletDelivery pd in foundPalletDeliveries)
                            dr.SetField<int>("NoOfPallets", (dr.Field<int>("NoOfPallets") - pd.NoOfPallets));
                    }
            }

            // Get all the rows that have pallets remaining.
            var retValRows = returndRows.Where(rr => rr.Field<int>("NoOfPallets") > 0);

            // If Any exist, run the queries and bind them up.
            if (retValRows.Count() > 0)
            {
                // If is an update, only bind rows with the same pallet type as the one on the selected order.
                var resourceEmptyPallets = from row in retValRows
                                           group row by
                                           new { ResourceID = row["ResourceID"], PalletContainer = row["Resource"], IsFixedUnit = row["IsFixedUnit"] } into g
                                           orderby g.Key.ResourceID, g.Key.PalletContainer, g.Key.IsFixedUnit
                                           select new
                                           {
                                               Resources = g.Key,
                                               Items = selectedPalletDelivery == null ? g : g.Where(ite => ite.Field<int>("PalletTypeID") == selectedPalletDelivery.PalletTypeID)
                                           };

                currentResources = (from row in retValRows
                                    where (int)row["ResourceID"] != 0
                                    select new KeyValuePair<int, string>
                                    (
                                       (int)row["ResourceID"],
                                        (string)row["Resource"]
                                    )).Distinct<KeyValuePair<int, string>>();

                // If is an update, only bind the resource associated with this order.
                lvEmptyPallets.DataSource = selectedPalletDelivery == null ? resourceEmptyPallets : resourceEmptyPallets.Where(rep => (int)rep.Resources.ResourceID == selectedPalletDelivery.ResourceID);
            }

            lvEmptyPallets.DataBind();
        }

        private void LoadExistingPalletHandling(Entities.PalletDelivery selectedPalletDelivery)
        {
            List<Entities.PalletDelivery> lpds = new List<Entities.PalletDelivery>();

            if (PalletDeliveries.Count > 0 && selectedPalletDelivery != null && PalletDeliveries.Exists(pd => pd.PalletOrder.OrderID == selectedPalletDelivery.PalletOrder.OrderID))
                PalletDeliveries.Remove(selectedPalletDelivery);

            if (UpdatedPalletDeliveries.Count > 0)
            {
                List<int> uppd = new List<int>();
                foreach (Entities.PalletDelivery pd in UpdatedPalletDeliveries)
                    uppd.Add(pd.PalletOrder.OrderID);

                lpds.AddRange(PalletDeliveries.Where(lpd => !uppd.Contains(lpd.PalletOrder.OrderID)));
                lpds.AddRange(UpdatedPalletDeliveries.Where(lpd => selectedPalletDelivery == null || lpd.PalletOrder.OrderID != selectedPalletDelivery.PalletOrder.OrderID));
            }
            else
                lpds = PalletDeliveries;

            lvExisingPalletHandling.DataSource = lpds;
            lvExisingPalletHandling.DataBind();
        }

        private void Rebind(Entities.PalletDelivery selectedPalletDelivery)
        {
            LoadEmptyPallets(selectedPalletDelivery);
            LoadExistingPalletHandling(selectedPalletDelivery);
        }

        #endregion

        private void BindPalletHandling(Entities.PalletDelivery palletDelivery)
        {
            btnGenerate.Text = "Update";

            m_isUpdateable = palletDelivery.PalletOrder.OrderStatus == eOrderStatus.Approved;

            rcbPalletHandlingAction.FindItemByValue(((int)palletDelivery.PalletAction).ToString()).Selected = true;

            if (palletDelivery.PalletAction == eInstructionType.DeHirePallets)
            {
                // Set the Customer Identity
                Facade.IOrganisation facOrg = new Facade.Organisation();
                Entities.Organisation org = facOrg.GetForIdentityId(palletDelivery.PalletOrder.CustomerIdentityID);

                rcbDeHireOrganisation.Text = org.OrganisationName;
                rcbDeHireOrganisation.SelectedValue = org.IdentityId.ToString();

                deHireRow.Style.Remove("display");
            }

            Facade.IPoint facPoint = new Facade.Point();
            ucDeliveryPoint.SelectedPoint = facPoint.GetPointForPointId(palletDelivery.PalletOrder.DeliveryPointID);

            #region Delivery Date Time

            //Delivery Date
            dteDeliveryFromDate.SelectedDate = palletDelivery.PalletOrder.DeliveryFromDateTime;
            dteDeliveryByDate.SelectedDate = palletDelivery.PalletOrder.DeliveryDateTime;
            dteDeliveryByTime.SelectedDate = palletDelivery.PalletOrder.DeliveryDateTime;
            dteDeliveryFromTime.SelectedDate = palletDelivery.PalletOrder.DeliveryFromDateTime;

            if (palletDelivery.PalletOrder.DeliveryIsAnytime)
            {
                // Anytime
                rdDeliveryIsAnytime.Checked = true;
                rdDeliveryTimedBooking.Checked = false;
                rdDeliveryBookingWindow.Checked = false;

                dteDeliveryFromTime.Enabled = false;
                hidDeliveryTimingMethod.Value = "anytime";

            }
            else if (palletDelivery.PalletOrder.DeliveryFromDateTime == palletDelivery.PalletOrder.DeliveryDateTime)
            {
                // Timed booking
                rdDeliveryIsAnytime.Checked = false;
                rdDeliveryTimedBooking.Checked = true;
                rdDeliveryBookingWindow.Checked = false;

                hidDeliveryTimingMethod.Value = "timed";
            }
            else
            {
                // Booking window
                rdDeliveryIsAnytime.Checked = false;
                rdDeliveryTimedBooking.Checked = false;
                rdDeliveryBookingWindow.Checked = true;

                hidDeliveryTimingMethod.Value = "window";
            }

            #endregion

            rntPalletDeliveryCharge.Value = (double)palletDelivery.PalletOrder.ForeignRate;
            rntPalletDeliveryCharge.Culture = new CultureInfo(palletDelivery.PalletOrder.LCID);

            //Set Updateable flags.

            rcbPalletHandlingAction.Enabled = m_isUpdateable;
            rcbDeHireOrganisation.Enabled = m_isUpdateable;

            ucDeliveryPoint.CanUpdatePoint = m_isUpdateable;

            dteDeliveryFromDate.Enabled = m_isUpdateable;
            dteDeliveryByDate.Enabled = m_isUpdateable;
            dteDeliveryByTime.Enabled = m_isUpdateable;
            dteDeliveryFromTime.Enabled = m_isUpdateable;

            rdDeliveryIsAnytime.Disabled = !m_isUpdateable;
            rdDeliveryTimedBooking.Disabled = !m_isUpdateable;
            rdDeliveryBookingWindow.Disabled = !m_isUpdateable;

            rntPalletDeliveryCharge.Enabled = m_isUpdateable;
        }

        private void GetSelectedPallets()
        {
            Entities.Instruction palletHandlingInstruction = new Entities.Instruction();

            eInstructionType selectedPalletAction = (eInstructionType)int.Parse(rcbPalletHandlingAction.SelectedValue);

            foreach (ListViewDataItem lvdi in lvEmptyPallets.Items)
            {
                CheckBox chkResourceSelected = lvdi.FindControl("chkResourceSelected") as CheckBox;

                if (chkResourceSelected != null && chkResourceSelected.Checked)
                {
                    HiddenField hdnResource = lvdi.FindControl("hdnResource") as HiddenField;
                    HiddenField hidResourceID = lvdi.FindControl("hidResourceID") as HiddenField;
                    HiddenField hidIsFixedUnit = lvdi.FindControl("hidIsFixedUnit") as HiddenField;
                    ListView lvItems = lvdi.FindControl("lvItems") as ListView;

                    int resourceID = 0;
                    int.TryParse(hidResourceID.Value, out resourceID);

                    bool isFixedUnit = false;
                    int isFixedUnitValue = 0;
                    int.TryParse(hidIsFixedUnit.Value, out isFixedUnitValue);
                    isFixedUnit = isFixedUnitValue > 0;

                    foreach (ListViewDataItem ilvdi in lvItems.Items)
                    {
                        #region Generate Pallet Orders

                        RadNumericTextBox rntNoOfPallets = ilvdi.FindControl("rntNoOfPallets") as RadNumericTextBox;

                        if (rntNoOfPallets != null && rntNoOfPallets.Value.Value > 0)
                        {
                            HiddenField hdnPalletTypeID = ilvdi.FindControl("hdnPalletTypeID") as HiddenField;
                            HiddenField hdnPalletType = ilvdi.FindControl("hdnPalletType") as HiddenField;
                            HiddenField hdnIdentifier = ilvdi.FindControl("hdnIdentifier") as HiddenField;

                            string destination = string.Empty;
                            Entities.Order palletDelivery = null;
                            Entities.PalletDelivery pd = null;

                            if (hdnIdentifier != null && hdnIdentifier.Value != string.Empty)
                            {
                                #region Update Order
                                pd = SelectedPalletDelivery;

                                if (pd.PalletOrder.OrderID > 0)
                                {
                                    Entities.PalletDelivery updatedDelivery = null;

                                    if (UpdatedPalletDeliveries.Count > 0)
                                        updatedDelivery = UpdatedPalletDeliveries.Find(upd => upd.PalletOrder.OrderID == pd.PalletOrder.OrderID);

                                    if (updatedDelivery == null)
                                        updatedDelivery = new Entities.PalletDelivery();
                                    else
                                        UpdatedPalletDeliveries.Remove(updatedDelivery);

                                    if (updatedDelivery.PalletOrder == null || updatedDelivery.PalletOrder.OrderID < 1)
                                    {
                                        updatedDelivery.PalletOrder = GeneratePalletOrder(int.Parse(hdnPalletTypeID.Value), (int)rntNoOfPallets.Value.Value, selectedPalletAction, ref destination);
                                        updatedDelivery.PalletOrder.OrderID = pd.PalletOrder.OrderID;
                                    }
                                    else
                                        updatedDelivery.PalletOrder.NoPallets = (int)rntNoOfPallets.Value.Value;

                                    updatedDelivery.PalletAction = selectedPalletAction;
                                    updatedDelivery.PalletActionDescription = Utilities.UnCamelCase(selectedPalletAction.ToString());
                                    updatedDelivery.ResourceID = resourceID;
                                    updatedDelivery.IsFixedUnit = isFixedUnit;

                                    updatedDelivery.PalletType = hdnPalletType.Value;
                                    updatedDelivery.Resource = hdnResource.Value;
                                    updatedDelivery.Destination = destination;

                                    updatedDelivery.UpdatedNoPallets = updatedDelivery.PalletOrder.NoPallets;
                                    //pd.UpdatedNoPallets = updatedDelivery.PalletOrder.NoPallets;
                                    updatedDelivery.IsDirty = true;

                                    UpdatedPalletDeliveries.Add(updatedDelivery);
                                }

                                //pd.PalletOrder.NoPallets = (int)rntNoOfPallets.Value.Value;
                                pd.UpdatedNoPallets = (int)rntNoOfPallets.Value.Value;
                                pd.IsDirty = true;

                                #endregion
                            }
                            else
                            {
                                #region Generate Order

                                palletDelivery = GeneratePalletOrder(int.Parse(hdnPalletTypeID.Value), (int)rntNoOfPallets.Value.Value, selectedPalletAction, ref destination);

                                pd = new Entities.PalletDelivery();

                                pd.PalletOrder = palletDelivery;

                                pd.PalletAction = selectedPalletAction;
                                pd.PalletActionDescription = Utilities.UnCamelCase(selectedPalletAction.ToString());
                                pd.ResourceID = resourceID;
                                pd.IsFixedUnit = isFixedUnit;

                                pd.PalletType = hdnPalletType.Value;
                                pd.Resource = hdnResource.Value;
                                pd.Destination = destination;

                                #endregion
                            }

                            PalletDeliveries.Add(pd);
                        }

                        #endregion
                    }
                }
            }
        }

        private void LoadSelectedPallets(Entities.PalletDelivery selectedPalletDelivery, bool isUpdateable)
        {
            m_emptyPalletCount = 0;
            foreach (ListViewDataItem lvdi in lvEmptyPallets.Items)
            {
                ListView lvItems = lvdi.FindControl("lvItems") as ListView;
                Label lblPalletTotal = lvdi.FindControl("lblPalletTotal") as Label;
                CheckBox chkResourceSelected = lvdi.FindControl("chkResourceSelected") as CheckBox;

                if (chkResourceSelected != null)
                    chkResourceSelected.Checked = true;

                foreach (ListViewDataItem ilvdi in lvItems.Items)
                {
                    HiddenField hdnPalletTypeID = ilvdi.FindControl("hdnPalletTypeID") as HiddenField;
                    HiddenField hdnIdentifier = ilvdi.FindControl("hdnIdentifier") as HiddenField;
                    RadNumericTextBox rntNoOfPallets = ilvdi.FindControl("rntNoOfPallets") as RadNumericTextBox;

                    if (hdnIdentifier != null)
                        hdnIdentifier.Value = selectedPalletDelivery.Identifier;

                    int palletTypeID = int.Parse(hdnPalletTypeID.Value);

                    int hp = selectedPalletDelivery.UpdatedNoPallets > 0 ? selectedPalletDelivery.UpdatedNoPallets : selectedPalletDelivery.NoOfPallets;

                    if (hp > 0)
                    {
                        HtmlTableCell NoOfPalletsWrapper = ilvdi.FindControl("NoOfPalletsWrapper") as HtmlTableCell;
                        HtmlTableCell SelectedPalletsWrapper = ilvdi.FindControl("SelectedPalletsWrapper") as HtmlTableCell;
                        HiddenField hdnNoOfPallets = ilvdi.FindControl("hdnNoOfPallets") as HiddenField;
                        Label lblNoOfPallets = ilvdi.FindControl("lblNoOfPallets") as Label;

                        int hNoOfPallets = int.Parse(hdnNoOfPallets.Value);

                        rntNoOfPallets.MaxValue = hNoOfPallets;
                        rntNoOfPallets.Value = hp;
                        NoOfPalletsWrapper.Style.Add("background-color", "#FFADAD");
                        SelectedPalletsWrapper.Style.Add("background-color", "#FFADAD");
                    }

                    m_emptyPalletCount += (int)rntNoOfPallets.Value.Value;
                }

                if (lblPalletTotal != null)
                    lblPalletTotal.Text = m_emptyPalletCount.ToString();
            }
        }

        #endregion

        #region Event Handlers

        #region Button

        void btnCancel_Click(object sender, EventArgs e)
        {
            PalletDeliveries = null;
            this.Close();
        }

        void btnGenerate_Click(object sender, EventArgs e)
        {
            btnCancelUpdate.Style.Add("display", "none");
            palletButtonBar.Style.Add("display", "none");
            btnGenerateUpdate.Style.Remove("display");

            Entities.PalletDelivery currentPalletDelivery = null;

            if (btnGenerate.Text == "Update" && SelectedPalletDelivery.PalletOrder.OrderID > 0)
                currentPalletDelivery = SelectedPalletDelivery;

            GetSelectedPallets();
            //LoadEmptyPallets(currentPalletDelivery);
            //LoadExistingPalletHandling(null);
            Rebind(null);

            btnGenerate.Text = "Generate";
            ClearPalletHandling();

            btnGenerateUpdate.Visible = !PalletDeliveries.Exists(lpds => lpds.IsDirty || lpds.PalletOrder.OrderID < 1);
        }

        void btnCancelUpdate_Click(object sender, EventArgs e)
        {
            btnCancelUpdate.Style.Add("display", "none");
            palletButtonBar.Style.Add("display", "none");
            btnGenerateUpdate.Style.Remove("display");
            btnGenerate.Text = "Generate";

            PalletDeliveries.Add(SelectedPalletDelivery);

            Rebind(null);
            ClearPalletHandling();

            btnGenerateUpdate.Visible = !PalletDeliveries.Exists(lpds => lpds.IsDirty || lpds.PalletOrder.OrderID < 1);
        }

        void btnUpdate_Click(object sender, EventArgs e)
        {
            Entities.FacadeResult result = null;
            Entities.CustomPrincipal user = (Entities.CustomPrincipal)Page.User;
            Facade.IPalletReturn facPalletReturn = new Facade.Pallet();

            result = facPalletReturn.UpdatePalletHandlingInstructions(m_job, UpdatedPalletDeliveries, PalletDeliveries, user.IdentityId, user.UserName);

            if (result != null && result.Success)
            {
                this.ReturnValue = bool.TrueString;
                PalletDeliveries = null;
                UpdatedPalletDeliveries = null;
                this.Close();
            }
            else
            {
                idErrors.Infringements = result.Infringements;
                idErrors.DisplayInfringments();
            }
        }

        void btnGenerateUpdate_Click(object sender, EventArgs e)
        {
            eInstructionType selectedPalletAction = (eInstructionType)int.Parse(rcbPalletHandlingAction.SelectedValue);

            if (selectedPalletAction == eInstructionType.LeavePallets)
                rfvDeHireOrganisation.Enabled = false;

            if (Page.IsValid)
            {
                GetSelectedPallets();
                btnUpdate_Click(null, null);
            }
        }

        #region Protected Buttons

        protected void lbRemove_Click(object sender, EventArgs e)
        {
            LinkButton lbRemove = sender as LinkButton;

            if (lbRemove.CommandArgument.Length > 1)
            {
                Entities.PalletDelivery pd = PalletDeliveries.SingleOrDefault(lpd => lpd.Identifier == lbRemove.CommandArgument);

                if (pd != null)
                {
                    if (pd.PalletOrder.OrderID > 0)
                    {
                        pd.ToBeRemoved = true;
                        pd.IsDirty = true;
                    }
                    else
                        PalletDeliveries.Remove(pd);
                }
            }

            GetSelectedPallets();
            Rebind(null);
            ClearPalletHandling();

            btnGenerateUpdate.Visible = !PalletDeliveries.Exists(lpds => lpds.IsDirty || lpds.PalletOrder.OrderID < 1);
        }

        protected void lbSelect_Click(object sender, EventArgs e)
        {
            LinkButton lbUpdate = sender as LinkButton;

            if (lbUpdate.CommandArgument.Length > 1)
            {
                Entities.PalletDelivery pd = PalletDeliveries.SingleOrDefault(lpd => lpd.Identifier == lbUpdate.CommandArgument);

                if (pd != null)
                {
                    SelectedPalletDelivery = pd;
                    BindPalletHandling(pd);
                    Rebind(pd);
                    LoadSelectedPallets(pd, m_isUpdateable);

                    if (pd.PalletOrder.OrderID > 0)
                        btnGenerateUpdate.Style.Add("display", "none");

                    btnCancelUpdate.Style.Remove("display");
                    palletButtonBar.Style.Remove("display");
                }
            }
        }

        protected void lbUndo_Click(object sender, EventArgs e)
        {
            LinkButton lbUndo = sender as LinkButton;

            if (lbUndo.CommandArgument.Length > 1)
            {
                Entities.PalletDelivery pd = PalletDeliveries.SingleOrDefault(lpd => lpd.Identifier == lbUndo.CommandArgument);

                if (pd != null)
                {
                    Entities.PalletDelivery foundUpdatedPalletDelivery = UpdatedPalletDeliveries.FirstOrDefault(fupd => fupd.PalletOrder.OrderID == pd.PalletOrder.OrderID);

                    if (foundUpdatedPalletDelivery != null)
                        UpdatedPalletDeliveries.Remove(foundUpdatedPalletDelivery);

                    pd.UpdatedNoPallets = 0;
                    pd.IsDirty = false;
                    pd.ToBeRemoved = false;

                    Rebind(null);

                    btnGenerateUpdate.Visible = !PalletDeliveries.Exists(lpds => lpds.IsDirty || lpds.PalletOrder.OrderID < 1);
                }
            }
        }

        protected void btnPalletMove_Click(object sender, EventArgs e)
        {
            Button btnPalletMove = sender as Button;
            int listItemIndex = int.Parse(btnPalletMove.Attributes["ListItemIndex"]);
            int sourceResourceID = int.Parse(btnPalletMove.Attributes["ResourceID"]);
            int targetResourceID = -1;

            RadComboBox rcbAlternateResources = lvEmptyPallets.Items[listItemIndex].FindControl("rcbAlternateResources") as RadComboBox;
            ListView lvItems = lvEmptyPallets.Items[listItemIndex].FindControl("lvItems") as ListView;

            targetResourceID = int.Parse(rcbAlternateResources.SelectedValue);

            IList<KeyValuePair<int, int>> palletTypeCount = new List<KeyValuePair<int, int>>();

            foreach (ListViewDataItem lvdi in lvItems.Items)
            {
                HiddenField hdnPalletType = lvdi.FindControl("hdnPalletType") as HiddenField;
                RadNumericTextBox rntNoOfPallets = lvdi.FindControl("rntNoOfPallets") as RadNumericTextBox;

                int palletTypeID = int.Parse(hdnPalletType.Value);
                int palletCount = (int)rntNoOfPallets.Value;

                palletTypeCount.Add(new KeyValuePair<int, int>(palletTypeID, palletCount));
            }

            Facade.IPalletBalance facPalletBalance = new Facade.Pallet();
            if (facPalletBalance.MovePalletsFromSourcetoTargetResource(m_job.JobId, sourceResourceID, targetResourceID, palletTypeCount, ((Entities.CustomPrincipal)Page.User).UserName))
                ConfigureDisplay();
        }

        #endregion

        #endregion

        #region Combo Box

        void rcbDeHireOrganisation_ItemsRequested(object o, RadComboBoxItemsRequestedEventArgs e)
        {
            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllClientsFiltered(e.Text);
            DataTable dt = ds.Tables[0];
            DataTable boundResults = dt.Clone();

            int itemsPerRequest = 20;
            int itemOffset = e.NumberOfItems;
            int endOffset = itemOffset + itemsPerRequest;
            if (endOffset > ds.Tables[0].Rows.Count)
                endOffset = ds.Tables[0].Rows.Count;

            for (int i = itemOffset; i < endOffset; i++)
                boundResults.ImportRow(dt.Rows[i]);

            rcbDeHireOrganisation.Items.Clear();
            rcbDeHireOrganisation.DataSource = boundResults;
            rcbDeHireOrganisation.DataBind();

            if (dt.Rows.Count > 0)
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
        }

        #endregion

        #region ListView

        void lvEmptyPallets_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            m_emptyPalletCount = 0;
            ListView lvItems = e.Item.FindControl("lvItems") as ListView;
            Label lblPalletTotal = e.Item.FindControl("lblPalletTotal") as Label;
            Panel pnlUpdateResource = e.Item.FindControl("pnlUpdateResource") as Panel;
            Button btnPalletMove = e.Item.FindControl("btnPalletMove") as Button;
            HiddenField hidResourceID = e.Item.FindControl("hidResourceID") as HiddenField;
            HiddenField hidPalletTypeID = e.Item.FindControl("hidPalletTypeID") as HiddenField;
            RadComboBox rcbAlternateResources = e.Item.FindControl("rcbAlternateResources") as RadComboBox;

            int resourceID = int.Parse(hidResourceID.Value);
            var specificResources = currentResources.Where<KeyValuePair<int, string>>(cr => cr.Key != resourceID);

            ListViewDataItem clvdi = e.Item as ListViewDataItem;

            if (specificResources.Count<KeyValuePair<int, string>>() > 0)
            {
                pnlUpdateResource.Visible = true;

                btnPalletMove.Attributes.Add("ResourceID", resourceID.ToString());
                btnPalletMove.Attributes.Add("ListItemIndex", clvdi.DataItemIndex.ToString());
                btnPalletMove.Attributes.Add("onclick", string.Format("realPostBack(\"{0}\", \"\"); return false;", btnPalletMove.UniqueID));

                rcbAlternateResources.DataSource = specificResources;
                rcbAlternateResources.DataBind();
            }
            else
                pnlUpdateResource.Visible = false;

            foreach (ListViewDataItem lvdi in lvItems.Items)
            {
                RadNumericTextBox rntNoOfPallets = lvdi.FindControl("rntNoOfPallets") as RadNumericTextBox;

                if (rntNoOfPallets != null)
                    m_emptyPalletCount += (int)rntNoOfPallets.Value.Value;
            }

            lblPalletTotal.Text = m_emptyPalletCount.ToString();
        }

        void lvExisingPalletHandling_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                Entities.PalletDelivery pd = ((ListViewDataItem)e.Item).DataItem as Entities.PalletDelivery;

                LinkButton lbRemove = e.Item.FindControl("lbRemove") as LinkButton;
                LinkButton lbSelect = e.Item.FindControl("lbSelect") as LinkButton;
                LinkButton lbUndo = e.Item.FindControl("lbUndo") as LinkButton;

                //Only set to be visible before the order has been created.
                lbUndo.Visible = pd.IsDirty;
                lbRemove.Visible = !lbUndo.Visible;

                if (m_job.Instructions.Count == 2 && this.PalletDeliveries.Count == 1) // If this is a simple load / drop, you cannot have less than 2 instructions on a run for Groupage Runs.
                    lbRemove.Visible = false;

                if (PalletDeliveries.Where(lpd => lpd.PalletOrder.OrderID < 1).Count() != 0)
                    lbUndo.OnClientClick = string.Format("javascript:if(!pendingActionsWarning()) return false;");

                //lblConfirmation.Visible = lbRemove.Visible = lbUndo.Visible = pd.PalletOrder.OrderID < 1;
            }
        }

        #endregion

        #endregion
    }
}
