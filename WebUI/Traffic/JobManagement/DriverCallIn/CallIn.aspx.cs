using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.SessionState; 
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using System.Text;
using System.Collections.Generic;
using Telerik.Web.UI;
using System.Threading;
using System.Threading.Tasks;

namespace Orchestrator.WebUI.Traffic.JobManagement.DriverCallIn
{
    /// <summary>
    /// Summary description for CallIn.
    /// </summary>
    public partial class CallIn : Orchestrator.Base.BasePage
    {
        protected class CollectDropWithCollectDropActual
        {
            public Entities.CollectDrop CollectDrop { get; set; }
            public Entities.CollectDropActual CollectDropActual { get; set; }
        }

        private const string C_JOB_VS = "C_JOB_VS";
        private const string C_PCV_VS = "C_PCV_VS";
        private const string C_POINT_ID_VS = "C_POINT_ID_VS";
        private const string C_SUBBY_INSTRUCTION_IDS = "C_SUBBY_INSTRUCTION_IDS";

        #region Form Elements

        protected Panel pnlResources;

        #endregion

        #region Page Variables

        #region Private

        private int m_instructionId = 0;		        // The id of the instruction that is being called-in.	
        private int m_palletsDespatched = 0;
        private int m_palletsReceived = 0;
        private int m_palletsReturned = 0;
        bool trackingPalletType = true;
        bool captureDebriefs = true;
        IEnumerable<DataRow> organisationPalletTypes = null;

        private Entities.Organisation m_organisation = null;    // The client of the job.
        private bool m_canEdit = false;

        private Dictionary<int, List<Entities.CollectDrop>> Customers = new Dictionary<int, List<Entities.CollectDrop>>();

        #endregion

        #region Protected

        protected override PageStatePersister PageStatePersister
        {
            get { return new HiddenFieldPageStatePersister(this.Page); }
        }

        protected Entities.Job m_job = null;
        protected List<Entities.PCV> _PCVS = new List<Orchestrator.Entities.PCV>();

        protected int m_jobId = 0;	                    // The id of the job we are currently manipulating.
        protected int m_pointId = 0;	                // The id of the point visited in the job we are currently manipulating.

        protected int m_PalletIdentityId = 0;			// The id of the organisation we are going to send the pallets to.
        protected string m_PalletTown = String.Empty;	// The description of the town we are going to send the pallets to.
        protected int m_PalletTownId = 0;			    // The id of the town we are going to send the pallets to.
        protected int m_PalletPointId = 0;			    // The id of the point we are going to send the pallets to.

        private const string vs_usersSubCon = "vs_userParentOrg";
        protected int UserParentIdentityId
        {
            get
            {
                int retVal = -1;
                if (ViewState[vs_usersSubCon] == null)
                {
                    Facade.IUser facUser = new Facade.User();
                    DataSet dsOrg = facUser.GetOrganisationForUser(this.Page.User.Identity.Name);

                    if (dsOrg.Tables[0].Rows.Count > 0)
                        ViewState[vs_usersSubCon] = dsOrg.Tables[0].Rows[0]["RelatedIdentityId"].ToString();
                }

                retVal = int.Parse(ViewState[vs_usersSubCon].ToString());
                return retVal;
            }
        }

        private const string vs_anyPalletTypesTracked = "vs_anyPalletTypesTracked";
        protected bool AnyPalletTypesTracked
        {
            get { return ViewState[vs_anyPalletTypesTracked] == null ? false : (bool)ViewState[vs_anyPalletTypesTracked]; }
            set { ViewState[vs_anyPalletTypesTracked] = value; }
        }

        protected List<int> SubbyInstructionIds
        {
            get { return ViewState[C_SUBBY_INSTRUCTION_IDS] == null ? new List<int>() : (List<int>)ViewState[C_SUBBY_INSTRUCTION_IDS]; }
            set { ViewState[C_SUBBY_INSTRUCTION_IDS] = value; }
        }

        private const string vs_anyClientRequiresDebrief = "vs_anyClientRequiresDebrief";
        protected bool AnyClientRequiresDebrief
        {
            get { return ViewState[vs_anyClientRequiresDebrief] == null ? false : (bool)ViewState[vs_anyClientRequiresDebrief]; }
            set { ViewState[vs_anyClientRequiresDebrief] = value; }
        }

        private const string vs_nextInstructionId = "vs_nextInstructionId";
        protected int NextInstructionId
        {
            get { return ViewState[vs_nextInstructionId] == null ? -1 : (int)ViewState[vs_nextInstructionId]; }
            set { ViewState[vs_nextInstructionId] = value; }
        }

        private const string vs_previousInstructionId = "vs_previousInstructionId";
        protected int PreviousInstructionId
        {
            get { return ViewState[vs_previousInstructionId] == null ? -1 : (int)ViewState[vs_previousInstructionId]; }
            set { ViewState[vs_previousInstructionId] = value; }
        }

        #endregion

        #region public

        private const string vs_updatePCVs = "vs_updatePCVs";
        public List<Entities.PCV> UpdatedPCVs
        {
            get { return ViewState[vs_updatePCVs] == null ? null : (List<Entities.PCV>)ViewState[vs_updatePCVs]; }
            set { ViewState[vs_updatePCVs] = value; }
        }

        #endregion

        #endregion

        private bool ShowMarkForRedeliveryCheckbox(int instructionId)
        {
            var instructionInformation = (from i in EF.DataContext.Current.InstructionSet
                                          where i.InstructionId == m_instructionId
                                          select new
                                          {
                                              Redelivery = i.Redelivery.FirstOrDefault(),
                                              i.InstructionType.InstructionTypeId
                                          }).FirstOrDefault();

           bool retVal = false;

           if (instructionInformation != null && ((eInstructionType)instructionInformation.InstructionTypeId) == eInstructionType.Drop && instructionInformation.Redelivery == null)
               retVal = true;

           return retVal;
        }

        private bool ShowMarkForAttemptedCollection(int instructionId)
        {
            Facade.IInstruction facInstruction = new Facade.Instruction();
            Entities.Instruction instruction = facInstruction.GetInstruction(m_instructionId, false);

            bool retVal = false;

            if (instruction != null // Exists
            &&
            ((eInstructionType)instruction.InstructionTypeId) == eInstructionType.Load // Type Load
            &&
            !instruction.CollectDrops.Exists(cd => cd.Order.CollectionPointID != cd.Order.DeliveryRunCollectionPointID)) // That these orders have not been cross-docked.
                retVal = true;

            return retVal;
        }

        #region Page Load/Init

        protected void Page_Load(object sender, System.EventArgs e)
        {
            trInformation.Visible = false;

            Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage, eSystemPortion.TakeCallIn);
            m_canEdit = Orchestrator.WebUI.Security.Authorise.CanAccess(eSystemPortion.TakeCallIn);
            btnRemoveActual.Enabled = m_canEdit;
            btnStoreActual.Enabled = m_canEdit;
            btnHandlePallets.Enabled = m_canEdit;

            m_jobId = Convert.ToInt32(Request.QueryString["jobId"]);
            m_instructionId = Convert.ToInt32(Request.QueryString["instructionId"]);

            //J.Steele 17/06/08
            //We now need to bind on postback as well because the ResourceThis dialog
            //causes a page reload by posting back
            if (!IsPostBack)
            {
                #region Initialise JS alerts on button press

                btnHandlePallets.OnClientClick = "openPalletHandlingWindow(" + m_jobId.ToString() + ");return false;";

                #endregion

                // Bind the job information.
                BindJob();

                // Load the appropriate instruction.
                BindInstruction();
            }

            if (this.IsPostBack)
            {
                if (m_instructionId == 0)
                    m_instructionId = Convert.ToInt32(hidInstructionId.Value);
                
                m_job = (Entities.Job)ViewState[C_JOB_VS];
                m_pointId = (int)ViewState[C_POINT_ID_VS];

                tabStrip1.Populate();
            }
        }

        override protected void OnInit(EventArgs e)
        {
            base.OnInit(e);

            btnStoreDriverContact.Click += new EventHandler(btnStoreDriverContact_Click);
            btnRemoveActual.Click += new EventHandler(btnRemoveActual_Click);
            btnStoreActual.Click += new EventHandler(btnStoreActual_Click);
            btnUpdateAttachedPCVs.Click += new EventHandler(btnUpdateAttachedPCVs_Click);
            btnConfirmRefusalType.Click += new EventHandler(btnConfirmRefusalType_Click);

            dlgResource.DialogCallBack += new EventHandler(dlgResource_DialogCallBack);
            dlgDriverClockIn.DialogCallBack += new EventHandler(dlgDriverClockIn_DialogCallBack);

            lvAttachedPCVs.ItemDataBound += new EventHandler<ListViewItemEventArgs>(lvAttachedPCVs_ItemDataBound);
            lvPickUpPallets.ItemDataBound += new EventHandler<ListViewItemEventArgs>(lvPalletHandling_ItemDataBound);
            lvDeHirePallets.ItemDataBound += new EventHandler<ListViewItemEventArgs>(lvDeHirePalletHandling_ItemDataBound);
            lvLeavePallets.ItemDataBound += new EventHandler<ListViewItemEventArgs>(lvPalletHandling_ItemDataBound);

            repReturns.ItemDataBound += new RepeaterItemEventHandler(repReturns_ItemDataBound);
            repClient.ItemDataBound += new RepeaterItemEventHandler(repClient_ItemDataBound);

            ramCallin.AjaxRequest += new RadAjaxControl.AjaxRequestDelegate(ramCallin_AjaxRequest);
        }

        #endregion

        #region Data Manipulation Methods

        /// <summary>
        /// Creates the driver call-in record against the current instruction.
        /// </summary>
        /// <param name="instructionActual">The InstructionActual object.</param>
        /// <param name="userId">The user recording the call-in.</param>
        /// <returns>Success indicator.</returns>
        private Entities.FacadeResult CreateInstructionActual(Entities.InstructionActual instructionActual, string userId)
        {
            Entities.FacadeResult result = null;

            // This has been amended to handle multiple PCV abased On Pallet Type
            PopulatePCV();

            using (Facade.IInstructionActual facInstructionActual = new Facade.Instruction())
            {
                DateTime startedAt = DateTime.UtcNow;
                result = facInstructionActual.Create(m_job, instructionActual, _PCVS, userId, true);
                DateTime endedAt = DateTime.UtcNow;
                DisplayElapsedTime("Create Call In", startedAt, endedAt, userId);
                instructionActual.InstructionActualId = result.ObjectId;
            }

            return result;
        }

        /// <summary>
        /// Updates the instruction actual record.
        /// </summary>
        /// <param name="instructionActual">The InstructionActual object.</param>
        /// <param name="userId">The user updating the call-in.</param>
        /// <returns>Success indicator.</returns>
        private Entities.FacadeResult UpdateInstructionActual(Entities.InstructionActual instructionActual, string userId)
        {
            Entities.FacadeResult result = null;

            // This has been amended to handle multiple PCVs based on Orders on the drop
            PopulatePCV();

            using (Facade.IInstructionActual facInstructionActual = new Facade.Instruction())
            {
                DateTime startedAt = DateTime.UtcNow;
                result = facInstructionActual.Update(m_job, instructionActual, _PCVS, userId);
                DateTime endedAt = DateTime.UtcNow;
                DisplayElapsedTime("Update Call In", startedAt, endedAt, userId);
            }

            return result;
        }

        #endregion

        #region Entity Population Methods

        /// <summary>
        /// Creates a new point based on the separate parts of the point passed as arguements.
        /// </summary>
        /// <param name="organisationId">The identity id of the organisation this point is registered to.</param>
        /// <param name="organisationName">The name of the organisation this point is registered to.</param>
        /// <param name="closestTownId">The id of the town id that is closest to this point.</param>
        /// <param name="description">The description that should be given to this point.</param>
        /// <param name="addressLine1">The first line of the address.</param>
        /// <param name="addressLine2">The second line of the address.</param>
        /// <param name="addressLine3">The third line of the address.</param>
        /// <param name="postTown">The town.</param>
        /// <param name="county">The county the point is within.</param>
        /// <param name="postCode">The post code.</param>
        /// <param name="longitude">The longitude attached to this point.</param>
        /// <param name="latitude">The latitude attached to this point.</param>
        /// <param name="trafficAreaId">The traffic area for this point.</param>
        /// <param name="userId">The id of the user creating this point.</param>
        /// <returns>The id of the new point created, or 0 if there were infringments encountered.</returns>
        private int CreateNewPoint(int organisationId, string organisationName, int closestTownId, string description, string addressLine1, string addressLine2, string addressLine3, string postTown, string county, string postCode, decimal longitude, decimal latitude, int trafficAreaId, string userId)
        {
            Entities.FacadeResult retVal = null;

            Entities.Point point = new Entities.Point();
            point.Address = new Entities.Address();
            point.Address.AddressLine1 = addressLine1;
            point.Address.AddressLine2 = addressLine2;
            point.Address.AddressLine3 = addressLine3;
            point.Address.PostTown = postTown;
            point.Address.County = county;
            point.Address.PostCode = postCode;
            point.Address.Longitude = longitude;
            point.Address.Latitude = latitude;
            point.Address.TrafficArea = new Entities.TrafficArea();
            point.Address.TrafficArea.TrafficAreaId = trafficAreaId;
            point.Address.AddressType = eAddressType.Point;

            point.Description = description;
            point.IdentityId = organisationId;
            point.Latitude = latitude;
            point.Longitude = longitude;
            point.OrganisationName = organisationName;
            Facade.IPostTown facPostTown = new Facade.Point();
            point.PostTown = facPostTown.GetPostTownForTownId(closestTownId);

            Facade.IPoint facPoint = new Facade.Point();
            retVal = facPoint.Create(point, userId);

            if (retVal.Success)
                return retVal.ObjectId;
            else
            {
                infringementDisplay.Infringements = retVal.Infringements;
                infringementDisplay.DisplayInfringments();

                return 0;
            }
        }

        /// <summary>
        /// Retrieve the job the user is working on and place it in ViewState.
        /// </summary>
        private void LoadJob()
        {
            using (Facade.IJob facJob = new Facade.Job())
            {
                m_job = facJob.GetJob(m_jobId);

                if (m_job.JobState == eJobState.Cancelled)
                    Response.Redirect("../../../Job/job.aspx?wiz=true&jobId=" + m_job.JobId.ToString() + "&csid=" + this.CookieSessionID);
            }

            ViewState[C_JOB_VS] = m_job;
        }

        /// <summary>
        /// Populates the instruction actual object from the call-in information supplied.
        /// </summary>
        /// <returns>The instructionActual object.</returns>
        private Entities.InstructionActual PopulateInstructionActual(string userId)
        {
            Facade.IInstruction facInstruction = new Facade.Instruction();
            m_instructionId = Convert.ToInt32(hidInstructionId.Value);
            Entities.InstructionActual instructionActual = ((Facade.IInstructionActual)facInstruction).GetEntityForInstructionId(m_instructionId);
            if (instructionActual == null)
            {
                instructionActual = new Entities.InstructionActual();
                instructionActual.InstructionId = m_instructionId;
                instructionActual.CollectDropActuals = new Entities.CollectDropActualCollection();
            }

            // Populate the collect drop information.
            eInstructionType instructionType = (eInstructionType)Enum.Parse(typeof(eInstructionType), hidInstructionType.Value);

            // Populate the main data about a call-in.
            instructionActual.Discrepancies = txtDiscrepancies.Text;
            instructionActual.DriversNotes = txtDriverNotes.Text;

            DateTime? deptDateTime = null, arriveDateTime = null;

            arriveDateTime = new DateTime(dteArrivalDate.SelectedDate.Value.Year, dteArrivalDate.SelectedDate.Value.Month, dteArrivalDate.SelectedDate.Value.Day, dteArrivalTime.SelectedDate.Value.Hour, dteArrivalTime.SelectedDate.Value.Minute, 0);

            // If the instruction type is trunk and the departue date and time has not been set, use the arrival datetime.
            if (instructionType == eInstructionType.Trunk && trGPSDeptTime.Style["display"] == "none" && trDeptTime.Style["display"] == "none")
                deptDateTime = arriveDateTime;
            else
                deptDateTime = new DateTime(dteDepartureDate.SelectedDate.Value.Year, dteDepartureDate.SelectedDate.Value.Month, dteDepartureDate.SelectedDate.Value.Day, dteDepartureTime.SelectedDate.Value.Hour, dteDepartureTime.SelectedDate.Value.Minute, 0);

            instructionActual.ArrivalDateTime = arriveDateTime.Value;
            instructionActual.LeaveDateTime = instructionActual.CollectDropDateTime = deptDateTime.Value;

            #region Instruction Type Specific Loadings

            switch (instructionType)
            {
                case eInstructionType.Load:
                    #region Load
                    // The collect drop dockets are specified by the drop recording process - we just need to create dummy actuals (we only do this for the initial create).
                    if (instructionActual.InstructionActualId == 0)
                    {
                        Entities.Instruction instruction = facInstruction.GetInstruction(m_instructionId);

                        foreach (Entities.CollectDrop loadDocket in instruction.CollectDrops)
                        {
                            Entities.CollectDropActual loadDocketDummyActual = new Entities.CollectDropActual();

                            // GRD:14/11/08 : I have amended this to populate with whatever is on the load docket 
                            //                so that the drop will have something sensible on it.
                            loadDocketDummyActual.CollectDropId = loadDocket.CollectDropId;
                            loadDocketDummyActual.NumberOfCases = loadDocket.NoCases;
                            loadDocketDummyActual.NumberOfPallets = loadDocket.NoPallets;
                            loadDocketDummyActual.Weight = loadDocket.Weight;

                            instructionActual.CollectDropActuals.Add(loadDocketDummyActual);
                        }
                    }
                    break;
                    #endregion
                case eInstructionType.Trunk:
                case eInstructionType.AttemptedDelivery:
                    #region Trunk / Attempted Delivery
                    // The collect drop dockets are specified by the drop recording process - we just need to create dummy actuals (we only do this for the initial create).
                    if (instructionActual.InstructionActualId == 0)
                    {
                        Entities.Instruction instruction = facInstruction.GetInstruction(m_instructionId);

                        foreach (Entities.CollectDrop loadDocket in instruction.CollectDrops)
                        {
                            Entities.CollectDropActual loadDocketDummyActual = new Entities.CollectDropActual();

                            loadDocketDummyActual.CollectDropId = loadDocket.CollectDropId;
                            loadDocketDummyActual.NumberOfCases = 0;
                            loadDocketDummyActual.NumberOfPallets = 0;
                            loadDocketDummyActual.Weight = 0;

                            instructionActual.CollectDropActuals.Add(loadDocketDummyActual);
                        }
                    }
                    break;
                    #endregion
                case eInstructionType.Drop:
                    if (m_job.JobType == eJobType.Normal || m_job.JobType == eJobType.Groupage)
                    {
                        #region Normal / Groupage
                        // Record the actuals for both the collection and the load portions.
                        foreach (RepeaterItem riClient in repClient.Items)
                        {
                            Repeater repPalletType = riClient.FindControl("repPalletType") as Repeater;

                            foreach (RepeaterItem riPalletType in repPalletType.Items)
                            {
                                Repeater repCollectDrop = riPalletType.FindControl("repCollectDrop") as Repeater;
                                foreach (RepeaterItem item in repCollectDrop.Items)
                                {
                                    if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                                    {
                                        HtmlInputHidden hidCollectDropId = (HtmlInputHidden)item.FindControl("hidCollectDropId");
                                        HtmlInputHidden hidCollectDropActualId = (HtmlInputHidden)item.FindControl("hidCollectDropActualId");

                                        TextBox txtQuantityDespatched = (TextBox)item.FindControl("txtQuantityDespatched");
                                        TextBox txtQuantityDelivered = (TextBox)item.FindControl("txtQuantityDelivered");
                                        TextBox txtShortageReference = (TextBox)item.FindControl("txtQuantityShortageReference");
                                        TextBox txtPalletsDespatched = (TextBox)item.FindControl("txtPalletsDespatched");
                                        TextBox txtPalletsDelivered = (TextBox)item.FindControl("txtPalletsDelivered");
                                        TextBox txtPalletsReturned = (TextBox)item.FindControl("txtPalletsReturned");
                                        TextBox txtWeightDespatched = (TextBox)item.FindControl("txtWeightDespatched");
                                        TextBox txtWeightDelivered = (TextBox)item.FindControl("txtWeightDelivered");

                                        RadComboBox rcbPalletType = item.FindControl("rcbPalletType") as RadComboBox;

                                        m_palletsDespatched += Convert.ToInt32(txtPalletsDespatched.Text);
                                        m_palletsReceived += Convert.ToInt32(txtPalletsDelivered.Text);
                                        m_palletsReturned += Convert.ToInt32(txtPalletsReturned.Text);

                                        int collectDropId = Convert.ToInt32(hidCollectDropId.Value);
                                        int collectDropActualId = Convert.ToInt32(hidCollectDropActualId.Value);

                                        Entities.CollectDropActual dropActual = null;

                                        if (collectDropActualId == 0)
                                        {
                                            // This is a new actual record
                                            dropActual = new Entities.CollectDropActual();
                                            dropActual.InstructionActualId = instructionActual.InstructionActualId;

                                            instructionActual.CollectDropActuals.Add(dropActual);
                                        }
                                        else
                                        {
                                            // This is an update to an existing actual record
                                            foreach (Entities.CollectDropActual existingDropActual in instructionActual.CollectDropActuals)
                                                if (existingDropActual.CollectDropActualId == collectDropActualId)
                                                    dropActual = existingDropActual;
                                        }

                                        // Configure the drop information.
                                        dropActual.CollectDropId = collectDropId;
                                        dropActual.NumberOfCases = Convert.ToInt32(txtQuantityDelivered.Text);
                                        dropActual.NumberOfPallets = Convert.ToInt32(txtPalletsDelivered.Text);
                                        dropActual.NumberOfPalletsReturned = Convert.ToInt32(txtPalletsReturned.Text);
                                        dropActual.Weight = Convert.ToDecimal(txtWeightDelivered.Text);
                                        dropActual.ShortageReference = txtShortageReference.Text;

                                        // Configure the collection actual also.
                                        Facade.ICollectDrop facCollectDrop = new Facade.CollectDrop();
                                        Entities.CollectDrop drop = facCollectDrop.GetForCollectDropId(collectDropId);
                                        Entities.CollectDrop load = facCollectDrop.GetForOrder(drop.OrderID, m_jobId, eInstructionType.Load);
                                        Facade.ICollectDropActual facCollectDropActual = new Facade.CollectDrop();
                                        Entities.CollectDropActual loadActual = facCollectDropActual.GetForCollectDropId(load.CollectDropId);

                                        //GRD: Part of Out of Sequence Call In Changes
                                        if (loadActual != null)
                                        {
                                            loadActual.NumberOfCases = Convert.ToInt32(txtQuantityDespatched.Text);
                                            loadActual.NumberOfPallets = Convert.ToInt32(txtPalletsDespatched.Text);
                                            loadActual.Weight = Convert.ToDecimal(txtWeightDespatched.Text);

                                            // Update the collection actual.
                                            facCollectDropActual.Update(loadActual, m_job.JobId, userId);
                                        }

                                        if (rcbPalletType != null)
                                        {
                                            // If selected pallet type differ's from the orders original pallettype, update.
                                            if (!string.IsNullOrEmpty(rcbPalletType.SelectedValue))
                                            {
                                                int selectedPalletType = int.Parse(rcbPalletType.SelectedValue);
                                                if (load.PalletTypeID != selectedPalletType)
                                                {
                                                    Facade.IOrder facOrder = new Facade.Order();
                                                    facOrder.UpdatePalletType(load.OrderID, selectedPalletType, load.Order.CustomerIdentityID, m_jobId, m_instructionId, load, loadActual, drop, userId);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        #endregion
                    }
                    else if (m_job.JobType == eJobType.Return)
                    {
                        #region Return
                        // Update the job's receipt number.
                        using (Facade.IJob facJob = new Facade.Job())
                        {
                            facJob.UpdateReturnReceiptNumber(m_job.JobId, txtReturnReceiptNumber.Text, userId);
                            LoadJob();
                        }

                        // Record the goods return information.
                        foreach (RepeaterItem item in repReturns.Items)
                        {
                            if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                            {
                                HtmlInputHidden hidCollectDropId = (HtmlInputHidden)item.FindControl("hidCollectDropId");
                                HtmlInputHidden hidCollectDropActualId = (HtmlInputHidden)item.FindControl("hidCollectDropActualId");

                                int collectDropId = Convert.ToInt32(hidCollectDropId.Value);
                                int collectDropActualId = Convert.ToInt32(hidCollectDropActualId.Value);

                                Entities.CollectDropActual returnActual = null;

                                if (collectDropActualId == 0)
                                {
                                    // This is a new actual record
                                    returnActual = new Entities.CollectDropActual();
                                    returnActual.InstructionActualId = instructionActual.InstructionActualId;
                                    using (Facade.IGoodsRefusal facGoodsRefusal = new Facade.GoodsRefusal())
                                        returnActual.GoodsRefusal = facGoodsRefusal.GetForRefusalId(Convert.ToInt32(((HtmlInputHidden)item.FindControl("hidGoodsRefusalId")).Value));

                                    instructionActual.CollectDropActuals.Add(returnActual);
                                }
                                else
                                {
                                    // This is an update to an existing actual record
                                    foreach (Entities.CollectDropActual existingDropActual in instructionActual.CollectDropActuals)
                                        if (existingDropActual.CollectDropActualId == collectDropActualId)
                                            returnActual = existingDropActual;
                                }

                                // Configure the drop information.
                                returnActual.CollectDropId = collectDropId;
                                returnActual.GoodsRefusal.RefusalReceiptNumber = ((TextBox)item.FindControl("txtReturnReceiptNumber")).Text;
                            }
                        }
                        #endregion
                    }
                    break;
                case eInstructionType.LeavePallets:
                    #region Leave Pallets
                    // Record the pallets being left.
                    if (instructionActual.CollectDropActuals == null)
                        instructionActual.CollectDropActuals = new Entities.CollectDropActualCollection();

                    foreach (ListViewDataItem lvdi in lvLeavePallets.Items)
                    {
                        HiddenField hidCollectDropActual = lvdi.FindControl("hidCollectDropActual") as HiddenField;
                        HiddenField hidCollectDropID = lvdi.FindControl("hidCollectDropID") as HiddenField;
                        HiddenField hidPalletTypeID = lvdi.FindControl("hidPalletTypeID") as HiddenField;
                        RadNumericTextBox rntActualPallets = lvdi.FindControl("rntActualPallets") as RadNumericTextBox;

                        int collectDropActualID = int.Parse(hidCollectDropActual.Value);
                        int collectDropID = int.Parse(hidCollectDropID.Value);
                        int palletTypeID = int.Parse(hidPalletTypeID.Value);

                        Entities.CollectDropActual leavePalletsActual = null;

                        if (collectDropActualID > 0)
                            leavePalletsActual = instructionActual.CollectDropActuals.Cast<Entities.CollectDropActual>().SingleOrDefault(cda => cda.CollectDropActualId == collectDropActualID);

                        if (leavePalletsActual == null)
                        {
                            leavePalletsActual = new Entities.CollectDropActual();
                            leavePalletsActual.CollectDropId = collectDropID;

                            instructionActual.CollectDropActuals.Add(leavePalletsActual);
                        }

                        leavePalletsActual.NumberOfPallets = (int)rntActualPallets.Value;
                    }
                    #endregion
                    break;
                case eInstructionType.DeHirePallets:
                    #region De-Hire Pallets
                    // Record the dehire receipt.
                    if (instructionActual.CollectDropActuals == null)
                        instructionActual.CollectDropActuals = new Entities.CollectDropActualCollection();

                    DateTime dataIssued = dteDeHireDate.SelectedDate.Value;

                    foreach (ListViewDataItem lvdi in lvDeHirePallets.Items)
                    {
                        HiddenField hidCollectDropActual = lvdi.FindControl("hidCollectDropActual") as HiddenField;
                        HiddenField hidCollectDropID = lvdi.FindControl("hidCollectDropID") as HiddenField;
                        HiddenField hidOrderID = lvdi.FindControl("hidOrderID") as HiddenField;
                        HiddenField hidPalletTypeID = lvdi.FindControl("hidPalletTypeID") as HiddenField;

                        RadNumericTextBox rntActualPallets = lvdi.FindControl("rntActualPallets") as RadNumericTextBox;

                        TextBox txtDeHireReceipt = lvdi.FindControl("txtDeHireReceipt") as TextBox;

                        DropDownList cboDeHireReceiptType = lvdi.FindControl("cboDeHireReceiptType") as DropDownList;

                        int collectDropActualID = int.Parse(hidCollectDropActual.Value);
                        int collectDropID = int.Parse(hidCollectDropID.Value);
                        int orderID = int.Parse(hidOrderID.Value);
                        int palletTypeID = int.Parse(hidPalletTypeID.Value);

                        Entities.CollectDropActual dehirePalletActual = null;

                        if (collectDropActualID > 0)
                            dehirePalletActual = instructionActual.CollectDropActuals.Cast<Entities.CollectDropActual>().SingleOrDefault(cda => cda.CollectDropActualId == collectDropActualID);

                        if (dehirePalletActual == null)
                        {
                            dehirePalletActual = new Entities.CollectDropActual();
                            dehirePalletActual.CollectDropId = collectDropID;
                            dehirePalletActual.DehiringReceipt = new Entities.DehiringReceipt();
                            instructionActual.CollectDropActuals.Add(dehirePalletActual);
                        }

                        dehirePalletActual.DehiringReceipt.OrderID = orderID;
                        dehirePalletActual.DehiringReceipt.JobID = m_jobId;
                        dehirePalletActual.DehiringReceipt.InstructionID = m_instructionId;
                        dehirePalletActual.DehiringReceipt.NumberOfPallets = dehirePalletActual.NumberOfPallets = (int)rntActualPallets.Value;
                        dehirePalletActual.DehiringReceipt.DateIssued = dataIssued;
                        dehirePalletActual.DehiringReceipt.ReceiptNumber = txtDeHireReceipt.Text;
                        dehirePalletActual.DehiringReceipt.ReceiptType = (eDehiringReceiptType)Enum.Parse(typeof(eDehiringReceiptType), cboDeHireReceiptType.SelectedValue.Replace(" ", ""));
                    }
                    break;
                    #endregion
                case eInstructionType.PickupPallets:
                    #region Pickup Pallets
                    // Record the pick up pallets actual.
                    if (instructionActual.CollectDropActuals == null)
                        instructionActual.CollectDropActuals = new Entities.CollectDropActualCollection();

                    foreach (ListViewDataItem lvdi in lvPickUpPallets.Items)
                    {
                        HiddenField hidCollectDropActual = lvdi.FindControl("hidCollectDropActual") as HiddenField;
                        HiddenField hidCollectDropID = lvdi.FindControl("hidCollectDropID") as HiddenField;
                        HiddenField hidPalletTypeID = lvdi.FindControl("hidPalletTypeID") as HiddenField;
                        RadNumericTextBox rntActualPallets = lvdi.FindControl("rntActualPallets") as RadNumericTextBox;

                        int collectDropActualID = int.Parse(hidCollectDropActual.Value);
                        int collectDropID = int.Parse(hidCollectDropID.Value);
                        int palletTypeID = int.Parse(hidPalletTypeID.Value);

                        Entities.CollectDropActual pickupPalletActual = null;

                        if (collectDropActualID > 0)
                            pickupPalletActual = instructionActual.CollectDropActuals.Cast<Entities.CollectDropActual>().SingleOrDefault(cda => cda.CollectDropActualId == collectDropActualID);

                        if (pickupPalletActual == null)
                        {
                            pickupPalletActual = new Entities.CollectDropActual();
                            pickupPalletActual.CollectDropId = collectDropID;
                            instructionActual.CollectDropActuals.Add(pickupPalletActual);
                        }

                        pickupPalletActual.NumberOfPallets = (int)rntActualPallets.Value;
                    }

                    break;
                    #endregion
                case eInstructionType.LeaveGoods:
                    #region Leave Goods
                    // Record the goods return information.
                    foreach (RepeaterItem item in repReturns.Items)
                    {
                        if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                        {
                            HtmlInputHidden hidCollectDropId = (HtmlInputHidden)item.FindControl("hidCollectDropId");
                            HtmlInputHidden hidCollectDropActualId = (HtmlInputHidden)item.FindControl("hidCollectDropActualId");

                            int collectDropId = Convert.ToInt32(hidCollectDropId.Value);
                            int collectDropActualId = Convert.ToInt32(hidCollectDropActualId.Value);

                            Entities.CollectDropActual returnActual = null;

                            if (collectDropActualId == 0)
                            {
                                // This is a new actual record
                                returnActual = new Entities.CollectDropActual();
                                returnActual.InstructionActualId = instructionActual.InstructionActualId;
                                using (Facade.IGoodsRefusal facGoodsRefusal = new Facade.GoodsRefusal())
                                    returnActual.GoodsRefusal = facGoodsRefusal.GetForRefusalId(Convert.ToInt32(((HtmlInputHidden)item.FindControl("hidGoodsRefusalId")).Value));

                                instructionActual.CollectDropActuals.Add(returnActual);
                            }
                            else
                            {
                                // This is an update to an existing actual record
                                foreach (Entities.CollectDropActual existingDropActual in instructionActual.CollectDropActuals)
                                    if (existingDropActual.CollectDropActualId == collectDropActualId)
                                        returnActual = existingDropActual;
                            }

                            // Configure the drop information.
                            returnActual.CollectDropId = collectDropId;
                        }
                    }
                    break;
                    #endregion
            }

            #endregion

            return instructionActual;
        }

        private void PopulatePCV()
        {
            // We need to populate/Create 1 or more PCVs
            Facade.IPCV facPCV = new Facade.PCV();
            foreach (Entities.PCV pcv in facPCV.GetForPointId(m_jobId, m_pointId, false))
                if (pcv.JobOfIssue == m_jobId && !_PCVS.Exists(epcv => epcv.PCVId == pcv.PCVId))
                    _PCVS.Add(pcv);

            foreach (RepeaterItem riClient in repClient.Items)
            {
                Repeater repPalletType = riClient.FindControl("repPalletType") as Repeater;
                foreach (RepeaterItem riPalletType in repPalletType.Items)
                {
                    List<int> l_collectDropIDs = new List<int>();
                    int l_palletsDespatched = 0, l_palletsDelivered = 0, l_palletsReturned = 0;

                    Repeater repCollectDrop = riPalletType.FindControl("repCollectDrop") as Repeater;
                    TextBox txtPCVVoucherCode = riPalletType.FindControl("txtPCVVoucherCode") as TextBox;
                    CheckBox chkNoPCVIssued = riPalletType.FindControl("chkNoPCVIssued") as CheckBox;
                    RadComboBox rcbReasonForIssue = riPalletType.FindControl("rcbReasonForIssue") as RadComboBox;
                    HiddenField hidPalletType = riPalletType.FindControl("hidpalletType") as HiddenField;
                    HiddenField hidClientID = riPalletType.FindControl("hidClientID") as HiddenField;

                    //int palletsToRecover = Convert.ToInt32(hidPalletsToRecover.Value);

                    if (txtPCVVoucherCode.Text.Length > 0 || chkNoPCVIssued.Checked)
                    {
                        Entities.PCV lPCV = null;
                        int clientID = int.Parse(hidClientID.Value);
                        int palletTypeID = int.Parse(hidPalletType.Value);

                        // Check to see if there is a PCV at this point for this job for this pallet type
                        if (_PCVS.Exists(p => p.PalletTypeID == palletTypeID && p.ClientID == clientID))
                            lPCV = _PCVS.Find(PCV => (PCV.PalletTypeID == palletTypeID) && (PCV.ClientID == clientID));

                        if (lPCV == null)
                            lPCV = new Orchestrator.Entities.PCV();

                        foreach (RepeaterItem ri in repCollectDrop.Items)
                        {
                            int pds = 0, pdl = 0, pr = 0;
                            Entities.CollectDrop cd = ri.DataItem as Entities.CollectDrop;

                            HtmlInputHidden hidCollectDropId = ri.FindControl("hidCollectDropId") as HtmlInputHidden;
                            TextBox txtPalletsDespatched = ri.FindControl("txtPalletsDespatched") as TextBox;
                            TextBox txtPalletsDelivered = ri.FindControl("txtPalletsDelivered") as TextBox;
                            TextBox txtPalletsReturned = ri.FindControl("txtPalletsReturned") as TextBox;

                            pds = int.Parse(txtPalletsDespatched.Text);
                            pdl = int.Parse(txtPalletsDelivered.Text);
                            pr = int.Parse(txtPalletsReturned.Text);

                            if (pr <= pds && pds > 0 && (pdl > pr))
                            {
                                l_palletsDespatched += pds;
                                l_palletsDelivered += pdl;
                                l_palletsReturned += pr;
                                l_collectDropIDs.Add(int.Parse(hidCollectDropId.Value));
                            }
                        }

                        lPCV.PCVIssued = !chkNoPCVIssued.Checked; // If checked, no PCV Issued.
                        lPCV.JobOfIssue = m_jobId;
                        lPCV.DeliverPointId = m_pointId;
                        lPCV.PCVRedemptionStatusId = ePCVRedemptionStatus.ToBeRedeemed;

                        if (!chkNoPCVIssued.Checked)
                            lPCV.PCVReasonForIssuingId = (ePCVReasonForIssuing)int.Parse(rcbReasonForIssue.SelectedValue);
                        else
                            lPCV.PCVReasonForIssuingId = ePCVReasonForIssuing.NoPalletsAvailable;

                        lPCV.PCVStatusId = ePCVStatus.Outstanding;
                        lPCV.RequiresScan = true;
                        lPCV.NoOfPalletsReceived = l_palletsDelivered;
                        lPCV.NoOfPalletsReturned = l_palletsReturned;

                        int l_voucherNo;

                        if (int.TryParse(txtPCVVoucherCode.Text, out l_voucherNo))
                            lPCV.VoucherNo = l_voucherNo;
                        else
                            lPCV.VoucherNo = 1;

                        lPCV.NoOfSignings = 1;
                        lPCV.DateOfIssue = dteDepartureDate.SelectedDate.Value;
                        lPCV.DateOfIssue = lPCV.DateOfIssue.Subtract(lPCV.DateOfIssue.TimeOfDay);
                        lPCV.DateOfIssue = lPCV.DateOfIssue.Add(dteDepartureTime.SelectedDate.Value.TimeOfDay);

                        Facade.ITrafficArea facTrafficArea = new Facade.Traffic();
                        lPCV.HeldLocation = facTrafficArea.GetForTrafficAreaId(m_job.StartingTrafficAreaId).ControlAreaId;
                        lPCV.InstructionID = int.Parse(hidInstructionId.Value);
                        lPCV.PalletTypeID = palletTypeID;
                        lPCV.ClientID = clientID;
                        lPCV.CollectDropIDs = l_collectDropIDs;

                        // If updating an existing pcv (removal), empty the text field of the voucher code.
                        if (lPCV.NoOfPalletsOutstanding == 0)
                            txtPCVVoucherCode.Text = string.Empty;

                        //Only add to the list if not updating an existing PCV.
                        if (lPCV.PCVId < 1)
                            _PCVS.Add(lPCV);
                    }
                }
            }
        }

        #endregion

        #region Page Population Methods

        /// <summary>
        /// Retrieves the appropriate instruction and binds it's information to the user interface.
        /// </summary>
        private void BindInstruction()
        {
            Entities.Instruction instruction = null;
            btnStoreDriverContact.Visible = false;

            // Get the instruction to work with.
            Facade.IInstruction facInstruction = new Facade.Instruction();
            
            if (m_instructionId == 0)
            {
                // Get the job's next instruction.
                instruction = facInstruction.GetNextInstruction(m_jobId);
            }
            else
            {
                // Get the specific instruction.
                instruction = facInstruction.GetInstruction(m_instructionId);
            }
      
            if (instruction != null)
            {
                NextInstructionId = facInstruction.GetFollowingInstructionId(m_jobId, instruction.InstructionID);
                PreviousInstructionId = facInstruction.GetPreviousInstructionId(instruction.InstructionID);

                #region Bind Next Call-in button.

                string nextCallInURL = string.Format("/Job/Job.aspx?wiz=true&jobId={0}&csid={1}", m_jobId, this.CookieSessionID);
                string previousCallInURL = string.Format("Alert('This is the first call-in on this Run');");

                if(NextInstructionId > 0)
                    nextCallInURL = string.Format("CallIn.aspx?jobId={0}&wiz=true&instructionId={1}&csid={2}", m_jobId, NextInstructionId, this.CookieSessionID);

                if(PreviousInstructionId > 0)
                    previousCallInURL = string.Format("CallIn.aspx?jobId={0}&wiz=true&instructionId={1}&csid={2}", m_jobId, PreviousInstructionId, this.CookieSessionID);

                btnNext.OnClientClick = string.Format("if(!btn_MoveNext('{0}')) return false;", nextCallInURL);
                btnAddMoveNext.OnClientClick = string.Format("if(!btn_AddMoveNext('{0}')) return false;", nextCallInURL);
                btnPrevious.OnClientClick = string.Format("if(!btn_MovePrevious('{0}')) return false;", previousCallInURL);

                #endregion

                #region Action on Instruction

                if (((Entities.CustomPrincipal)this.Page.User).IsInRole(((int)eUserRole.SubConPortal).ToString()))
                {
                    this.SubbyInstructionIds = facInstruction.GetInstructionIDsForSubContractor(m_jobId, this.UserParentIdentityId, false);

                    if (!this.SubbyInstructionIds.Contains(instruction.InstructionID))
                    {
                        this.lblNotYourInstruction.Text = "You are not resourced on the specified instruction.";
                        this.Table2.Visible = false;
                    }

                    Entities.Instruction followingInstruction = facInstruction.GetFollowingInstruction(m_jobId, instruction.InstructionID);

                    if (followingInstruction == null)
                    {
                        btnNext.Visible = false;
                        btnAddMoveNext.Visible = false;
                    }
                }

                if (instruction.InstructionOrder == 0)
                    this.btnPrevious.Visible = false;

                btnStoreDriverContact.Visible = true;
                m_instructionId = instruction.InstructionID;
                m_pointId = instruction.PointID;
                ViewState[C_POINT_ID_VS] = instruction.PointID;
                eInstructionType instructionType = (eInstructionType)instruction.InstructionTypeId;
                hidInstructionType.Value = Enum.GetName(typeof(eInstructionType), instructionType);
                DateTime defaultDate = DateTime.MinValue;
                btnHandlePallets.Visible = false;

                // Don't allow pallets to be handled once the job is invoiced.
                SetButtonStates(m_instructionId);

                // Check that the leg this instruction is attached to is of the correct state.
                if (instruction.InstructionState == eInstructionState.Booked || instruction.InstructionState == eInstructionState.Planned)
                {
                    BindLastInstruction();
                    return;
                }

                defaultDate = instruction.PlannedArrivalDateTime;
                DateTime defaultArrivalDate = instruction.PlannedArrivalDateTime;
                DateTime defaultDepartureDate = instruction.PlannedDepartureDateTime;

                lblPlannedTime.Text = instruction.PlannedArrivalDateTime.ToString("dd/MM/yy HH:mm");

                // J.Steele 12/06/08
                // This may be used to implement Nicholls Unknown trailer changes
                // so that it will be convenenient for them to change the trailer if it is Unknown
                string resourceThisLink = string.Format(string.Format(@"javascript:openResourceWindow({0},'{1}','{2}','{3}','{4}','{5}','{6}',{7},'{8:dd/MM/yyyy  HH:mm:ss}')",
                                            instruction.InstructionID.ToString(), // instructionId
                                            instruction.Driver == null ? "" : instruction.Driver.Individual.FullName, // Fullname
                                            instruction.Driver == null ? "" : instruction.Driver.ResourceId.ToString(), //DriverResourceId
                                            instruction.Vehicle == null ? "" : instruction.Vehicle.RegNo,
                                            instruction.Vehicle == null ? "" : instruction.Vehicle.ResourceId.ToString(),
                                            instruction.Trailer == null ? "" : instruction.Trailer.TrailerRef,
                                            instruction.Trailer == null ? "" : instruction.Trailer.ResourceId.ToString(),
                                            m_job.JobId,
                                            m_job.LastUpdateDate
                                            ));

                hlChangeResources.NavigateUrl = resourceThisLink;

                if (
                    //Whole Job subbed
                    (m_job.SubContractors.Count > 0 && m_job.SubContractors[0].SubContractWholeJob)

                    //This instruction subbed 
                    || instruction.JobSubContractID > 0

                    //Order subbed and this instruction is the drop for the Order
                    || (
                        m_job.JobType == eJobType.Groupage
                        && instruction.InstructionTypeId == (int)eInstructionType.Drop
                        && instruction.CollectDrops != null
                        && instruction.CollectDrops.Count > 0
                        && instruction.CollectDrops[0].Order != null
                        && instruction.CollectDrops[0].Order.JobSubContractID > 0) // Subbed for the delivery of this order
                    )
                {


                    Facade.IJobSubContractor facSub = new Facade.Job();
                    Entities.JobSubContractor subby = null;
                    if (instruction.JobSubContractID > 0)
                        subby = facSub.GetSubContractorForJobSubContractId(instruction.JobSubContractID);
                    else if (m_job.JobType == eJobType.Groupage
                        && instruction.InstructionTypeId == (int)eInstructionType.Drop
                        && instruction.CollectDrops != null
                        && instruction.CollectDrops.Count > 0
                        && instruction.CollectDrops[0].Order != null
                        && instruction.CollectDrops[0].Order.JobSubContractID > 0)
                    {
                        subby = facSub.GetSubContractorForJobSubContractId(instruction.CollectDrops[0].Order.JobSubContractID);
                    }
                    else
                        subby = facSub.GetSubContractorForJobId(m_jobId)[0];

                    using (Facade.IOrganisation facOrganisation = new Facade.Organisation())
                    {
                        lblDriver.Text = facOrganisation.GetForIdentityId(subby.ContractorIdentityId).OrganisationName;
                        lblDriver.Attributes.Add("onmouseover", "javascript:showHelpTipUrl(event, '~/resource/driver/drivercontactpopup.aspx?identityId=organisation:" + subby.ContractorIdentityId.ToString() + "');");
                    }

                    if (subby.UseSubContractorTrailer)
                        txtTrailer.Text = "Using sub-contractors trailer.";
                    else if (instruction.Trailer != null)
                        txtTrailer.Text = instruction.Trailer.TrailerRef;
                    else
                        txtTrailer.Text = string.Empty;
                    //Display an "Unknown" trailer as red
                    if (txtTrailer.Text.Equals("unknown", StringComparison.OrdinalIgnoreCase))
                        txtTrailer.ForeColor = Color.Red;
                    else
                        txtTrailer.ForeColor = Color.Black;


                    lblVehicle.Visible = false;
                    lblVehicleText.Visible = false;
                    trTakeTrailerOn.Visible = false;
                    imgClockIn.Visible = false;
                }
                else
                {
                    lblDriver.Text = instruction.Driver.Individual.FullName;
                    lblDriver.Attributes.Add("onmouseover", "javascript:showHelpTipUrl(event, '~/resource/driver/drivercontactpopup.aspx?identityId=individual:" + instruction.Driver.Individual.IdentityId.ToString() + "');");
                    lblDriver.Attributes.Add("onmouseout", "javascript:hideHelpTip(this);");

                    txtTrailer.Text = instruction.Trailer != null ? instruction.Trailer.TrailerRef : "N/A";
                    //Display an "Unknown" trailer as red
                    if (txtTrailer.Text.Equals("unknown", StringComparison.OrdinalIgnoreCase))
                        txtTrailer.ForeColor = Color.Red;
                    else
                        txtTrailer.ForeColor = Color.Black;

                    lblVehicle.Text = instruction.Vehicle.RegNo;

                    //Has the Driver got a start time for today?
                    hidDriverResourceID.Value = instruction.Driver.ResourceId.ToString();
                    imgClockIn.Attributes.Add("onclick", "ClockIn(" + instruction.Driver.ResourceId + ")");
                    DataAccess.IDriver dacDriver = new DataAccess.Driver();
                    DataSet dsStartTimes = dacDriver.GetStartTimesForDateAndDriver(instruction.Driver.ResourceId, DateTime.Today);
                    if (dsStartTimes.Tables[0].Rows.Count == 0)
                    {

                    }
                    else
                    {
                        lblStartTime.Text = "<a href=\"javascript:ClockIn(" + instruction.Driver.ResourceId + ");\">" + ((DateTime)dsStartTimes.Tables[0].Rows[0][1]).ToString("dd/MM HH:mm") + "</a>";
                        imgClockIn.Visible = false;
                    }

                    // If this is the last instruction on the job provide the option to "move" the trailer to the driver's next location.
                    trTakeTrailerOn.Visible = false;
                    if (instruction.Trailer != null)
                    {
                        hidTrailerResourceId.Value = instruction.Trailer.ResourceId.ToString();

                        bool isLastInstructionOnJob = facInstruction.GetFollowingInstruction(m_jobId, instruction.InstructionID) == null;
                        if (isLastInstructionOnJob)
                        {
                            Facade.IJourney facJourney = new Facade.Journey();
                            DataSet nextLocation = facJourney.GetNextLocationForTrailer(m_jobId);
                            if (nextLocation != null && nextLocation.Tables.Count == 1)
                            {
                                trTakeTrailerOn.Visible = true;
                                chkMoveTrailer.Text = "Driver will take trailer on to " + nextLocation.Tables[0].Rows[0]["Description"].ToString() + ".";
                                hidTakeTrailerToPointId.Value = ((int)nextLocation.Tables[0].Rows[0]["PointId"]).ToString();
                            }
                        }
                    }
                }

                // Populate the instruction summary.
                switch (instructionType)
                {
                    case eInstructionType.Load:
                        lblInstructionDescription.Text = "Collect Goods from " + instruction.Point.Description + ", " + instruction.Point.PostTown.TownName + " by " + lblPlannedTime.Text + ".";
                        break;
                    case eInstructionType.Trunk:
                        lblInstructionDescription.Text = "Trunk at " + instruction.Point.Description + ", " + instruction.Point.PostTown.TownName + " by " + lblPlannedTime.Text + ".";
                        break;
                    case eInstructionType.Drop:
                        lblInstructionDescription.Text = "Deliver Goods to " + instruction.Point.Description + ", " + instruction.Point.PostTown.TownName + " by " + lblPlannedTime.Text + ".";
                        break;
                    case eInstructionType.LeavePallets:
                        lblInstructionDescription.Text = "Leave Pallets at " + instruction.Point.Description + ", " + instruction.Point.PostTown.TownName + " by " + lblPlannedTime.Text + ".";
                        break;
                    case eInstructionType.DeHirePallets:
                        lblInstructionDescription.Text = "De-Hire Pallets at " + instruction.Point.Description + ", " + instruction.Point.PostTown.TownName + " for " + instruction.ClientsCustomer + " by " + lblPlannedTime.Text + ".";
                        break;
                    case eInstructionType.PickupPallets:
                        lblInstructionDescription.Text = "Pickup Pallets at " + instruction.Point.Description + ", " + instruction.Point.PostTown.TownName + " by " + lblPlannedTime.Text + ".";
                        break;
                    case eInstructionType.LeaveGoods:
                        lblInstructionDescription.Text = "Leave Goods at " + instruction.Point.Description + ", " + instruction.Point.PostTown.TownName + " by " + lblPlannedTime.Text + ".";
                        break;
                    case eInstructionType.AttemptedDelivery:
                        lblInstructionDescription.Text = "Delivery Attempted at " + instruction.Point.Description + ", " + instruction.Point.PostTown.TownName + " by " + lblPlannedTime.Text + ".";
                        break;
                }

                lblInstructionDescription.Attributes.Add("onmouseover", "javascript:showHelpTipUrl(event, '~/point/getPointAddressHtml.aspx?pointId=" + instruction.Point.PointId.ToString() + "');");
                lblInstructionDescription.Attributes.Add("onmouseout", "javascript:hideHelpTip(this);");

                // Removed re task #22449
                //lblOrganisation.Attributes.Add("onmouseover", "javascript:showHelpTipUrl(event, '~/organisation/getContactDetails.aspx?identityId=" + m_job.IdentityId.ToString() + "');");
                //lblOrganisation.Attributes.Add("onmouseout", "javascript:hideHelpTip(this);");

                if (instructionType == eInstructionType.Drop)
                {
                    // Bind the pcv span
                    using (Facade.IPCV facPCV = new Facade.PCV())
                    {
                        foreach (Entities.PCV lpcv in facPCV.GetForPointId(m_jobId, m_pointId, false))
                            if (lpcv.JobOfIssue == m_jobId && !_PCVS.Exists(epcv => epcv.PCVId == lpcv.PCVId))
                                _PCVS.Add(lpcv);

                        if (UpdatedPCVs == null || UpdatedPCVs.Count == 0)
                            UpdatedPCVs = facPCV.GetForInstructionIDAndDeliveryPointID(instruction.InstructionID, m_pointId);

                        if (UpdatedPCVs.Count > 0)
                        {
                            lvAttachedPCVs.DataSource = UpdatedPCVs;
                            lvAttachedPCVs.DataBind();

                            rcbPCVAction.DataSource = facPCV.GetAllPCVRemptionDetailStatuses();
                            rcbPCVAction.DataBind();

                            pnlAttachedPCVDetails.Visible = true;
                        }
                        else
                            pnlAttachedPCVDetails.Visible = false;
                    }
                }

                if (instruction.IsAnyTime)
                    lblBookedDate.Text = instruction.BookedDateTime.ToString("dd/MM/yy") + " Any Time.";
                else
                    lblBookedDate.Text = instruction.BookedDateTime.ToString("dd/MM/yy HH:mm");

                pnlDeliveryCallIn.Visible = instructionType == eInstructionType.Drop;
                pnlLeavePalletsCallIn.Visible = instructionType == eInstructionType.LeavePallets;
                pnlDehirePalletsCallIn.Visible = instructionType == eInstructionType.DeHirePallets;
                pnlPickupPallets.Visible = instructionType == eInstructionType.PickupPallets;
                pnlReturnGoodsCallIn.Visible = instructionType == eInstructionType.Drop && m_job.JobType == eJobType.Return;
                pnlTrunk.Visible = m_job.JobType == eJobType.Groupage && instructionType == eInstructionType.Trunk && instruction.CollectDrops.GetForOrderID(0) == null;

                switch (instructionType)
                {
                    case eInstructionType.Load:
                        BindInstructionForLoad(instruction);
                        break;
                    case eInstructionType.Trunk:
                    case eInstructionType.AttemptedDelivery:
                        BindInstructionForTrunk(instruction);
                        break;
                    case eInstructionType.Drop:
                        if (m_job.JobType == eJobType.Normal || m_job.JobType == eJobType.Groupage)
                            BindInstructionForDrop(instruction);
                        else if (m_job.JobType == eJobType.Return)
                            BindInstructionForReturnGoods(instruction);
                        break;
                    case eInstructionType.LeavePallets:
                        AnyPalletTypesTracked = true;
                        BindInstructionForLeavePallets(instruction);
                        break;
                    case eInstructionType.DeHirePallets:
                        AnyPalletTypesTracked = true;
                        dteDeHireDate.SelectedDate = defaultDate;
                        BindInstructionForDeHirePallets(instruction);
                        break;
                    case eInstructionType.PickupPallets:
                        AnyPalletTypesTracked = true;
                        BindInstructionForPickupPallets(instruction);
                        break;
                    case eInstructionType.LeaveGoods:
                        BindInstructionForLeaveGoods(instruction);
                        break;
                }

                if (instruction.InstructionActuals == null || instruction.InstructionActuals.Count == 0)
                    BindInstructionProgress(instruction.PlannedArrivalDateTime, instruction.PlannedDepartureDateTime);

                #endregion

                #region Empty Pallet Count

                int? resourceID = instruction.Trailer != null ? instruction.Trailer.ResourceId : instruction.Vehicle != null && instruction.Vehicle.IsFixedUnit ? instruction.Vehicle.ResourceId : (int?)null;
                hidResourceID.Value = resourceID == null ? "-1" : ((int)resourceID).ToString();
                GetEmptyPalletCount();

                #endregion
            }
            else
            {
                // All the instructions have been recorded.
                BindLastInstruction();
            }

            if (((Entities.CustomPrincipal)this.Page.User).IsInRole(((int)eUserRole.SubConPortal).ToString()))
            {
                // hide things that sub cons should not see.
                this.buttonBar.Visible = false;
                this.pnlResourceDetails.Visible = false;
                this.btnStoreDriverContact.Visible = false;
                this.btnRedeliver.Visible = false;
                this.btnHandlePallets.Visible = false;

                this.trGPSDeptTime.Visible = false;
                this.trGPSArrival.Visible = false;

            }
        }

        private bool _beingInvoiced = false;

        private void SetButtonStates(int instructionId)
        {
            Facade.IJob facJob = new Facade.Job();
            _beingInvoiced = facJob.OrdersBeingInvoicedForInstruction(instructionId);
            bool subbyNotBeingInvoiced = (facJob as Facade.IJobSubContractor).IsRateEditableForJob(m_jobId);
            btnStoreActual.Enabled = m_canEdit;
            btnRemoveActual.Enabled = m_canEdit && !_beingInvoiced && subbyNotBeingInvoiced;
            btnHandlePallets.Visible = btnHandlePallets.Enabled = m_canEdit && !_beingInvoiced && AnyPalletTypesTracked && AnyClientRequiresDebrief;
            btnAddMoveNext.Enabled = m_canEdit;
        }

        /// <summary>
        /// Retrieves the progress information recorded against an instruction and populates the relevant controls.
        /// </summary>
        private void BindInstructionProgress(DateTime defaultArrivalDate, DateTime defaultDepartureDate)
        {
            // Load the contact information from the table.
            Facade.IInstructionProgress facInstructionProgress = new Facade.InstructionProgress();
            Entities.InstructionProgress progress = facInstructionProgress.Get(m_instructionId);

            if (progress != null)
            {
                if (progress.IncomingPenetration != null)
                {
                    lblGPSArrivalTime.Text = progress.IncomingPenetration.Value.ToString("dd/MM/yy HH:mm");
                    if (!dteArrivalDate.SelectedDate.HasValue)
                        dteArrivalDate.SelectedDate = dteArrivalTime.SelectedDate = progress.IncomingPenetration.Value;
                }
                if (progress.OutgoingPenetration != null)
                {
                    this.lblGPSDepartureTime.Text = progress.OutgoingPenetration.Value.ToString("dd/MM/yy HH:mm");
                    if (!dteDepartureDate.SelectedDate.HasValue)
                        dteDepartureDate.SelectedDate = dteDepartureTime.SelectedDate = progress.OutgoingPenetration.Value;
                }

                if (!dteArrivalDate.SelectedDate.HasValue)
                    dteArrivalDate.SelectedDate = dteArrivalTime.SelectedDate = defaultArrivalDate;

                if (!dteDepartureDate.SelectedDate.HasValue)
                    dteDepartureDate.SelectedDate = dteDepartureTime.SelectedDate = defaultDepartureDate;

                txtDriverNotes.Text = progress.DriversNotes;
                txtDiscrepancies.Text = progress.Discrepancies;
            }
            else
            {
                dteArrivalDate.SelectedDate = dteArrivalTime.SelectedDate = defaultArrivalDate;
                dteDepartureDate.SelectedDate = dteDepartureTime.SelectedDate = defaultDepartureDate;
            }
        }

        /// <summary>
        /// Binds the common components of the instruction actual.
        /// </summary>
        /// <param name="instructionActual">The recorded call-in information.</param>
        private void BindInstructionActual(Entities.InstructionActual instructionActual)
        {

            Facade.IJob facJob = new Facade.Job();
            _beingInvoiced = facJob.IsBeingInvoiced(m_jobId) || facJob.OrdersBeingInvoicedForInstruction(instructionActual.InstructionId);
            bool subbyNotBeingInvoiced = (facJob as Facade.IJobSubContractor).IsRateEditableForJob(m_jobId);

            btnStoreDriverContact.Visible = false;
            btnAddMoveNext.Visible = false;

            if (instructionActual.InstructionActualId > 0)
            {
                btnStoreActual.Text = "Update Call-in";
                this.btnRedeliver.Visible = false;
                if (((Entities.CustomPrincipal)this.Page.User).IsInRole(((int)eUserRole.SubConPortal).ToString()))
                {
                    this.pnlCreateUser.Visible = true;
                    this.lblCreatedBy.Text = instructionActual.CreateUser;
                    this.lblCreatedDate.Text = instructionActual.CreateDate.ToString("dd/MM/yy HH:mm");
                    if (!String.IsNullOrEmpty(instructionActual.LastUpdateUser))
                    {
                        this.lblUpdatedBy.Text = instructionActual.LastUpdateUser;
                        this.lblUpdatedDate.Text = instructionActual.LastUpdateDate.ToString("dd/MM/yy HH:mm");
                    }
                }
                else
                    this.pnlCreateUser.Visible = false;
            }
            else
                this.btnRedeliver.Visible = ShowMarkForRedeliveryCheckbox(m_instructionId) || ShowMarkForAttemptedCollection(m_instructionId);

            hidInstructionActualId.Value = instructionActual.InstructionActualId.ToString();
            dteArrivalDate.SelectedDate = instructionActual.ArrivalDateTime;
            dteArrivalTime.SelectedDate = instructionActual.ArrivalDateTime;

            dteDepartureDate.SelectedDate = instructionActual.LeaveDateTime;
            dteDepartureTime.SelectedDate = instructionActual.LeaveDateTime;

            txtDriverNotes.Text = instructionActual.DriversNotes;
            txtDiscrepancies.Text = instructionActual.Discrepancies;

            if (!_beingInvoiced && subbyNotBeingInvoiced)
                btnRemoveActual.Visible = true;
            else
                btnRemoveActual.Visible = false;

            //SetButtonStates(instructionActual.InstructionId);
        }

        /// <summary>
        /// Binds the instruction (and instruction actual information if available) for the passed instruction, if the instruction type is Load.
        /// </summary>
        /// <param name="instruction"></param>
        private void BindInstructionForLoad(Entities.Instruction instruction)
        {
            hidInstructionId.Value = instruction.InstructionID.ToString();

            if (instruction.InstructionActuals == null || instruction.InstructionActuals.Count == 0)
            {
                // This instruction has yet to be recorded
                hidInstructionActualId.Value = "0";
                btnStoreActual.Text = "Add Call-in";

                this.btnRedeliver.Visible = ShowMarkForRedeliveryCheckbox(m_instructionId) || ShowMarkForAttemptedCollection(m_instructionId);

                btnRemoveActual.Visible = false;
                if (!lblBookedDate.Text.Contains("Any Time"))
                {
                    dteArrivalDate.SelectedDate = DateTime.Parse(lblBookedDate.Text);
                    dteArrivalTime.SelectedDate = dteArrivalDate.SelectedDate;
                    dteDepartureDate.SelectedDate = dteArrivalDate.SelectedDate;
                    dteDepartureTime.SelectedDate = dteDepartureDate.SelectedDate;
                }
            }
            else
            {
                // This instruction has been recorded.
                BindInstructionActual(instruction.InstructionActuals[0]);
            }

        }

        /// <summary>
        /// Binds the instruction (and instruction actual information if available) for the passed instruction, if the instruction type is Trunk.
        /// </summary>
        /// <param name="instruction"></param>
        private void BindInstructionForTrunk(Entities.Instruction instruction)
        {
            hidInstructionId.Value = instruction.InstructionID.ToString();

            if (instruction.InstructionActuals == null || instruction.InstructionActuals.Count == 0)
            {
                // This instruction has yet to be recorded
                hidInstructionActualId.Value = "0";
                btnStoreActual.Text = "Add Call-in";
                btnRemoveActual.Visible = false;
                if (!lblBookedDate.Text.Contains("Any Time"))
                {
                    dteArrivalDate.SelectedDate = DateTime.Parse(lblBookedDate.Text);
                    dteArrivalTime.SelectedDate = dteArrivalDate.SelectedDate;
                    dteDepartureDate.SelectedDate = dteArrivalDate.SelectedDate;
                    dteDepartureTime.SelectedDate = dteDepartureDate.SelectedDate;
                }
            }
            else
            {
                // This instruction has been recorded.
                BindInstructionActual(instruction.InstructionActuals[0]);
            }

            // if its actually a trunk, hide the departure datetime controls.
            if (instruction.InstructionTypeId == (int)eInstructionType.Trunk && instruction.CollectDrops.Count == 0)
            {
                // Disable the departure datetime validation.
                rfvArrivalDate.Enabled = rfvArrivalTime.Enabled = false;
                //cfvDepartureDate.Enabled = 

                // Hide the departure datetime controls.
                trGPSDeptTime.Style.Add("display", "none");
                trDeptTime.Style.Add("display", "none");
            }

            // Bind the orders
            List<Entities.CollectDrop> involvedOrders = new List<Entities.CollectDrop>();
            foreach (Entities.CollectDrop collectDrop in instruction.CollectDrops)
                if (collectDrop.Order != null)
                    involvedOrders.Add(collectDrop);
            if (involvedOrders.Count > 0)
            {
                repOrderHandling.DataSource = involvedOrders;
                repOrderHandling.DataBind();
            }
            else
                repOrderHandling.Visible = false;
        }

        /// <summary>
        /// Binds the instruction (and instruction actual information if available) for the passed instruction, if the instruction type is Drop.
        /// </summary>
        /// <param name="instruction"></param>
        private void BindInstructionForDrop(Entities.Instruction instruction)
        {
            hidInstructionId.Value = instruction.InstructionID.ToString();
            bool hasActual = false;

            if (instruction.InstructionActuals == null || instruction.InstructionActuals.Count == 0)
            {
                // This instruction has yet to be recorded
                hidInstructionActualId.Value = "0";
                btnStoreActual.Text = "Add Call-in";
                btnRemoveActual.Visible = false;
                btnHandlePallets.Visible = false;

                ClearInstructionActuals();
            }
            else
            {
                hasActual = true;

                // This instruction has been recorded.
                BindInstructionActual(instruction.InstructionActuals[0]);
            }

            //Generate the Clients for the Instruction
            if (m_job.JobType == eJobType.Groupage)
            {
                foreach (Entities.CollectDrop CD in instruction.CollectDrops)
                {
                    if (CD.OrderID > 0)
                    {
                        if (!Customers.ContainsKey(CD.Order.CustomerIdentityID))
                        {
                            // add this to the customer dictionary
                            List<Entities.CollectDrop> cds = new List<Orchestrator.Entities.CollectDrop>();
                            cds.Add(CD);

                            Customers.Add(CD.Order.CustomerIdentityID, cds);
                        }
                        else
                        {
                            // add the order to the existing dictionary
                            List<Entities.CollectDrop> cds = Customers[CD.Order.CustomerIdentityID];
                            cds.Add(CD);
                        }
                    }
                }

            }
            else if (m_job.JobType == eJobType.Normal)
            {
                List<Entities.CollectDrop> cds = new List<Orchestrator.Entities.CollectDrop>();
                foreach (Entities.CollectDrop cd in instruction.CollectDrops)
                    cds.Add(cd);

                Customers.Add(m_job.IdentityId, cds);
            }
            repClient.DataSource = Customers;
            repClient.DataBind();

            //If instruction has Actual, display Handle Pallets btn.
            btnHandlePallets.Visible = hasActual;
        }

        /// <summary>
        /// Binds the instruction (and instruction actual information if available) for the passed instruction, if the instruction type is LeaveGoods.
        /// </summary>
        /// <param name="instruction"></param>
        private void BindInstructionForLeaveGoods(Entities.Instruction instruction)
        {
            hidInstructionId.Value = instruction.InstructionID.ToString();

            if (instruction.InstructionActuals == null || instruction.InstructionActuals.Count == 0)
            {
                // This instruction has yet to be recorded
                hidInstructionActualId.Value = "0";
                btnStoreActual.Text = "Add Call-in";
                btnRemoveActual.Visible = false;

                ClearInstructionActuals();
            }
            else
            {
                // This instruction has been recorded.
                BindInstructionActual(instruction.InstructionActuals[0]);
            }


            repReturns.DataSource = instruction.CollectDrops;
            repReturns.DataBind();
        }

        /// <summary>
        /// Binds the instruction (and instruction actual information if available) for the passed instruction, if the instruction type is LeavePallets.
        /// </summary>
        /// <param name="instruction"></param>
        private void BindInstructionForLeavePallets(Entities.Instruction instruction)
        {
            hidInstructionId.Value = instruction.InstructionID.ToString();

            IEnumerable<Entities.InstructionActual> iaCollection = instruction.InstructionActuals == null ? Enumerable.Empty<Entities.InstructionActual>() : instruction.InstructionActuals.Cast<Entities.InstructionActual>();

            var collectDropsWithActuals =
                from cd in instruction.CollectDrops.Cast<Entities.CollectDrop>()
                select new CollectDropWithCollectDropActual()
                {
                    CollectDrop = cd,
                    CollectDropActual =
                        (from ia in iaCollection
                         from cda in ia.CollectDropActuals.Cast<Entities.CollectDropActual>()
                         where cda.CollectDropId == cd.CollectDropId
                         select cda).SingleOrDefault()
                };

            lvLeavePallets.DataSource = collectDropsWithActuals;
            lvLeavePallets.DataBind();

            if (instruction.InstructionActuals == null || instruction.InstructionActuals.Count == 0)
            {
                // This instruction has yet to be recorded
                hidInstructionActualId.Value = "0";
                btnStoreActual.Text = "Add Call-in";
                btnRemoveActual.Visible = false;

                ClearInstructionActuals();
            }
            else
            {
                // This instruction has been recorded.
                BindInstructionActual(instruction.InstructionActuals[0]);
                btnHandlePallets.Visible = btnHandlePallets.Enabled && m_canEdit;
            }
        }

        /// <summary>
        /// Binds the instruction (and instruction actual information if available) for the passed instruction, if the instruction type is DeHirePallets.
        /// </summary>
        /// <param name="instruction"></param>
        private void BindInstructionForDeHirePallets(Entities.Instruction instruction)
        {
            hidInstructionId.Value = instruction.InstructionID.ToString();

            IEnumerable<Entities.InstructionActual> iaCollection = instruction.InstructionActuals == null ? Enumerable.Empty<Entities.InstructionActual>() : instruction.InstructionActuals.Cast<Entities.InstructionActual>();

            var collectDropsWithActuals =
                from cd in instruction.CollectDrops.Cast<Entities.CollectDrop>()
                select new CollectDropWithCollectDropActual()
                {
                    CollectDrop = cd,
                    CollectDropActual =
                        (from ia in iaCollection
                         from cda in ia.CollectDropActuals.Cast<Entities.CollectDropActual>()
                         where cda.CollectDropId == cd.CollectDropId
                         select cda).SingleOrDefault()
                };

            lvDeHirePallets.DataSource = collectDropsWithActuals;
            lvDeHirePallets.DataBind();

            if (instruction.InstructionActuals == null || instruction.InstructionActuals.Count == 0)
            {
                // This instruction has yet to be recorded
                hidInstructionActualId.Value = "0";
                btnStoreActual.Text = "Add Call-in";
                btnRemoveActual.Visible = false;

                ClearInstructionActuals();
            }
            else
            {
                // This instruction has been recorded.
                BindInstructionActual(instruction.InstructionActuals[0]);

                if (instruction.InstructionActuals[0].CollectDropActuals[0].DehiringReceipt != null)
                    dteDeHireDate.SelectedDate = instruction.InstructionActuals[0].CollectDropActuals[0].DehiringReceipt.DateIssued;

                btnHandlePallets.Visible = btnHandlePallets.Enabled && m_canEdit;
            }
        }

        /// <summary>
        /// Binds the instruction (and instruction actual information if available) for the passed instruction, if the instruction type is PickupPallets.
        /// </summary>
        /// <param name="instruction"></param>
        private void BindInstructionForPickupPallets(Entities.Instruction instruction)
        {
            hidInstructionId.Value = instruction.InstructionID.ToString();

            IEnumerable<Entities.InstructionActual> iaCollection = instruction.InstructionActuals == null ? Enumerable.Empty<Entities.InstructionActual>() : instruction.InstructionActuals.Cast<Entities.InstructionActual>();

            var collectDropsWithActuals =
                from cd in instruction.CollectDrops.Cast<Entities.CollectDrop>()
                select new CollectDropWithCollectDropActual()
                {
                    CollectDrop = cd,
                    CollectDropActual =
                        (from ia in iaCollection
                         from cda in ia.CollectDropActuals.Cast<Entities.CollectDropActual>()
                         where cda.CollectDropId == cd.CollectDropId
                         select cda).SingleOrDefault()
                };

            lvPickUpPallets.DataSource = collectDropsWithActuals;
            lvPickUpPallets.DataBind();

            if (instruction.InstructionActuals == null || instruction.InstructionActuals.Count == 0)
            {
                // This instruction has yet to be recorded
                hidInstructionActualId.Value = "0";
                btnStoreActual.Text = "Add Call-in";
                btnRemoveActual.Visible = false;

                ClearInstructionActuals();
            }
            else
            {
                // This instruction has been recorded.
                BindInstructionActual(instruction.InstructionActuals[0]);
                btnHandlePallets.Visible = btnHandlePallets.Enabled && m_canEdit;
            }
        }

        /// <summary>
        /// Binds the instruction (and instruction actual information if available) for the passed instruction, if the instruction is to Return Goods.
        /// </summary>
        /// <param name="instruction"></param>
        private void BindInstructionForReturnGoods(Entities.Instruction instruction)
        {
            hidInstructionId.Value = instruction.InstructionID.ToString();

            if (instruction.InstructionActuals == null || instruction.InstructionActuals.Count == 0)
            {
                // This instruction has yet to be recorded
                hidInstructionActualId.Value = "0";
                btnStoreActual.Text = "Add Call-in";
                btnRemoveActual.Visible = false;

                ClearInstructionActuals();
            }
            else
            {
                // This instruction has been recorded.
                BindInstructionActual(instruction.InstructionActuals[0]);
                txtReturnReceiptNumber.Text = m_job.ReturnReceiptNumber;
            }

            txtReturnReceiptNumber.Text = m_job.ReturnReceiptNumber;

            repReturns.DataSource = instruction.CollectDrops;
            repReturns.DataBind();
        }

        /// <summary>
        /// Displays the job information on the page.
        /// </summary>
        private void BindJob()
        {
            using (Facade.IJob facJob = new Facade.Job())
            {
                LoadJob();

                using (Facade.IOrganisation facOrganisation = new Facade.Organisation())
                    m_organisation = facOrganisation.GetForIdentityId(m_job.IdentityId);

                //Removed - re Task number #22449
                //lblLoadNumber.Text = m_job.LoadNumber;
                //lblLoadNumberText.Text = m_organisation.LoadNumberText;
                //lblOrganisation.Text = m_organisation.OrganisationName;
            }
        }

        /// <summary>
        /// Configures the page to display the "no more instructions" page.
        /// </summary>
        private void BindLastInstruction()
        {
            if (!((Entities.CustomPrincipal)this.Page.User).IsInRole(((int)eUserRole.SubConPortal).ToString()))
            {
                Response.Redirect("~/Traffic/JobManagement/DriverCallIn/tabProgress.aspx?wiz=true&jobId=" + m_jobId.ToString() + "&instructionId=" + m_instructionId.ToString() + "&csid=" + this.CookieSessionID);
            }

        }

        /// <summary>
        /// Resets the instruction actuals information.
        /// </summary>
        private void ClearInstructionActuals()
        {
            dteArrivalDate.SelectedDate = null;
            dteArrivalTime.SelectedDate = null;
            dteDepartureDate.SelectedDate = null;
            dteDepartureTime.SelectedDate = null;
            txtDiscrepancies.Text = String.Empty;
            txtDriverNotes.Text = String.Empty;

            hidInstructionActualId.Value = "0";
        }

        private void GetEmptyPalletCount()
        {
            int? resourceID = int.Parse(hidResourceID.Value);

            if (resourceID == -1)
                resourceID = null;

            Facade.Job facJob = new Facade.Job();
            DataSet ds = facJob.GetEmptyPalletCountForJobAndResource(m_jobId, resourceID);

            var queryPallets = ds.Tables[0].Rows.Cast<DataRow>();
            List<DataRow> emptyPallet = new List<DataRow>();

            if (queryPallets.Any())
                emptyPallet = queryPallets.ToList();

            if (emptyPallet.Count > 0 && (int)emptyPallet[0]["JobPalletCount"] > 0)
            {
                lblTotalEmptyPalletCount.Text = emptyPallet[0]["JobPalletCount"].ToString();
                lblResourceEmptyPalletCount.Text = emptyPallet[0]["ResourcePalletCount"].ToString();
                lblResourceDescription.Text = " (" + emptyPallet[0]["Resource"].ToString() + ")";
                pnlEmptyPalletCount.Visible = true;
            }
            else
                pnlEmptyPalletCount.Visible = false;
        }

        private void DisplayElapsedTime(string operation, DateTime startedAt, DateTime endedAt, string userId)
        {
            TimeSpan elapsed = endedAt.Subtract(startedAt);
            lblElapsedTime.Text = "Process Time: " + elapsed.TotalMilliseconds.ToString() + "ms.";
            Facade.ProcessingTime.Create(m_jobId, operation, m_instructionId, (decimal)elapsed.TotalMilliseconds, userId);
        }

        protected string RetrieveReturnReceiptTextBoxClientIDs()
        {
            StringBuilder sb = new StringBuilder();

            if (pnlReturnGoodsCallIn.Visible)
            {
                foreach (RepeaterItem item in repReturns.Items)
                {
                    if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                    {
                        if (sb.Length > 0)
                            sb.Append(",");

                        sb.Append(item.FindControl("txtReturnReceiptNumber").ClientID);
                    }
                }
            }

            return sb.ToString();
        }

        #endregion

        #region Private Methods

        private void RefreshIFrame()
        {
            ScriptManager.RegisterStartupScript(this, this.GetType(), "refreshIframe", string.Format("refreshIframe({0});", m_jobId.ToString()), true);
        }

        private void SendCallinAlertEmail()
        {
            Facade.User facUser = new Facade.User();
            Entities.User loggedOnUser = facUser.GetUserByUserName(Context.User.Identity.Name);

            Dictionary<string, string> recipients = new Dictionary<string, string>();

            if (loggedOnUser != null)
                recipients.Add(loggedOnUser.Email, loggedOnUser.FullName);
            else
                recipients.Add("support@orchestrator.co.uk", "support");

            string Subject = string.Format("Add / Update Call-in {0}", m_jobId);
            string Body = string.Format("Orchestrator has encounter an issue when trying to add a Call-in for Run {0}.\n\nPlease go here, to retry.", m_jobId);

            Utilities.SendEmail(recipients, Subject, Body, null);
        }

        private bool StoreActualWorkAndDisplayInfringements(string userId)
        {
            var instructionActual = PopulateInstructionActual(userId);

            var result = StoreActualWork(instructionActual, userId);

            if (!result.Success)
            {
                infringementDisplay.Infringements = result.Infringements;
                infringementDisplay.DisplayInfringments();
            }

            return result.Success;
        }

        private Entities.FacadeResult StoreActualWork(Entities.InstructionActual instructionActual, string userId)
        {
            var result = new Entities.FacadeResult(true);

            try
            {
                #region First Attempt Add/Update Callin

                if (instructionActual.InstructionActualId == 0)
                {
                    result = CreateInstructionActual(instructionActual, userId);
                    lblMessage.Text = "The call-in has been recorded.";
                }
                else
                {
                    result = UpdateInstructionActual(instructionActual, userId);
                    lblMessage.Text = "The call-in has been updated.";
                }

                #endregion
            }
            catch (Exception ex)
            {
                if (ex is SqlException && Utilities.MapSqlException(ex as SqlException))
                {
                    try
                    {
                        #region Second Attempt Add/Update Callin

                        if (instructionActual.InstructionActualId == 0)
                        {
                            result = CreateInstructionActual(instructionActual, userId);
                            lblMessage.Text = "The call-in has been recorded.";
                        }
                        else
                        {
                            result = UpdateInstructionActual(instructionActual, userId);
                            lblMessage.Text = "The call-in has been updated.";
                        }

                        #endregion
                    }
                    catch (Exception innerEx)
                    {
                        if (innerEx is SqlException && Utilities.MapSqlException(ex as SqlException))
                        {
                            #region Email User to make them aware than the call-in process has failed.

                            SendCallinAlertEmail();

                            #endregion
                        }

                        Global.UnhandledException(innerEx);
                    }
                }
                else
                {
                    Global.UnhandledException(ex);
                }
            }
            finally
            {
                if (result == null)
                {
                    Entities.BusinessRuleInfringementCollection bric = new Entities.BusinessRuleInfringementCollection();
                    bric.Add(new Entities.BusinessRuleInfringement("Add / Update Callin", "There was an error processing the call-in. You will recieve a notification shortly"));
                    result = new Entities.FacadeResult(false);
                    result.Infringements = bric;
                }
            }

            if (result.Success)
            {
                this.ReturnValue = result.Success.ToString();

                if (trTakeTrailerOn.Visible && chkMoveTrailer.Checked)
                {
                    #region Update Resource Location

                    int trailerResourceId = int.Parse(hidTrailerResourceId.Value);
                    int takeToPointId = int.Parse(hidTakeTrailerToPointId.Value);

                    Entities.Instruction activeInstruction = null;
                    int pointId = 0;

                    if (m_job.Instructions == null)
                        using (Facade.IInstruction facInstructions = new Facade.Instruction())
                            m_job.Instructions = facInstructions.GetForJobId(m_job.JobId);

                    foreach (Entities.Instruction instruction in m_job.Instructions)
                        if (instruction.InstructionID == instructionActual.InstructionId)
                        {
                            activeInstruction = instruction;
                            pointId = instruction.PointID;
                            break;
                        }

                    int instructionID = 0, driverId = 0, vehicleId = 0;
                    if (activeInstruction != null)
                    {
                        instructionID = activeInstruction.InstructionID;
                        if (activeInstruction.Driver != null)
                            driverId = activeInstruction.Driver.ResourceId;
                        if (activeInstruction.Vehicle != null)
                            vehicleId = activeInstruction.Vehicle.ResourceId;
                    }

                    Facade.IResource facResource = new Facade.Resource();
                    facResource.UpdateResourceLocation(trailerResourceId, takeToPointId, instructionActual.InstructionId, m_jobId, instructionActual.LeaveDateTime, driverId, vehicleId, userId);

                    #endregion
                }

                //Store any actions set for any attached PCVs.
                if (this.UpdatedPCVs != null && this.UpdatedPCVs.Count > 0)
                    btnUpdateAttachedPCVs_Click(null, null);
            }

            return result;
        }

        private void DiplayPCVValidationFailure()
        {
            // if the validation has failed for the PCV voucher code we nee to make sure we are shing the panels
            if (hiddenPanelArray.Value.Length > 0)
            {
                string[] clientIDs = hiddenPanelArray.Value.Split(',');

                string script = "$(\"#{0}\").css('display', '');";
                string output = "<script type=\"text/javascript\">$(document)ready(function(){";
                foreach (string client in clientIDs)
                {
                    if (client.Length > 1)
                        output += string.Format(script, client);
                }
                output += "});</script>";
                lblInjectScript.Text = output;
            }
        }

        #endregion

        #region Ajax Request

        private void ramCallin_AjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            var userId = ((Entities.CustomPrincipal)Page.User).UserName;
            var instructionActual = PopulateInstructionActual(userId);

            Task.Run(() =>
                {
                    try
                    {
                        StoreActualWork(instructionActual, userId);
                    }
                    catch (Exception ex)
                    {
                        Global.UnhandledException(ex);
                    }
                }
            );
        }

        #endregion

        #region Button Events

        //
        // Add Clean Call-in is a javascript function that calls an Ajax Request on ramCallin_AjaxRequest. This then fires the call-in method asynchronously.
        //

        private void btnConfirmRefusalType_Click(object sender, EventArgs e)
        {
            if (ShowMarkForAttemptedCollection(m_instructionId))
                Response.Redirect(string.Format("../OrderBasedAttemptedCollection.aspx?jId={0}&iId={1}&csid=", m_jobId, m_instructionId, this.CookieSessionID));

            var success = StoreActualWorkAndDisplayInfringements(((Entities.CustomPrincipal)Page.User).UserName);

            if (success)
            {
                string url = string.Empty;
                string refusalType = hidRefusalType.Value;

                switch (refusalType)
                {
                    case "Full":
                        url= string.Format("../orderbasedredelivery.aspx?wiz=true&jobID={0}&instructionID={1}",
                            m_jobId,
                            hidInstructionId.Value) + "&csid=" + this.CookieSessionID;
                        break;

                    case "Partial":
                        url = string.Format("Refusal.aspx?wiz=true&jobID={0}&instructionId={1}&t=1",
                            m_jobId,
                            hidInstructionId.Value) + "&csid=" + this.CookieSessionID;
                        break;
                }

                // Send the user to the mark for redelivery page.
                Response.Redirect(url);
            }
            else
                DiplayPCVValidationFailure();
        }

        private void btnStoreActual_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                this.StoreActualWorkAndDisplayInfringements(((Entities.CustomPrincipal)Page.User).UserName);

                trInformation.Visible = true;
                BindJob();
                BindInstruction();
                RefreshIFrame();
            }
            else
                DiplayPCVValidationFailure();

            LoadingPanel1.Visible = false;
        }

        private void btnRemoveActual_Click(object sender, EventArgs e)
        {
            Facade.IInstructionActual facInstructionActual = new Facade.Instruction();
            Facade.IGoodsRefusal facGoodsRefusal = new Facade.GoodsRefusal();
            DateTime startedAt = DateTime.UtcNow;

            DataSet ds = facGoodsRefusal.GetRefusalsForInstructionId(m_instructionId);
            List<int> rows = new List<int>();

            if (ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow item in ds.Tables[0].Rows)
                {
                    facGoodsRefusal.Delete((int)item["RefusalID"], ((Entities.CustomPrincipal)Page.User).UserName);
                }
            }

            Entities.FacadeResult result = facInstructionActual.Delete(m_job, m_instructionId, m_pointId, Convert.ToInt32(hidInstructionActualId.Value), ((Entities.CustomPrincipal)Page.User).UserName);
            DateTime endedAt = DateTime.UtcNow;

            if (result.Success)
                Response.Redirect(string.Format("/traffic/JobManagement/DriverCallIn/CallIn.aspx?wiz=true&jobId={0}&csid={1}", m_jobId.ToString(), this.CookieSessionID));
            else
            {
                infringementDisplay.Infringements = result.Infringements;
                infringementDisplay.DisplayInfringments();
                LoadingPanel1.Visible = false;
            }
        }
        
        private void btnStoreDriverContact_Click(object sender, EventArgs e)
        {
            Entities.InstructionProgress progress = new Entities.InstructionProgress(m_instructionId);
            progress.Discrepancies = txtDiscrepancies.Text;
            progress.DriversNotes = txtDriverNotes.Text;

            Facade.IInstructionProgress facInstructionProgress = new Facade.InstructionProgress();
            facInstructionProgress.Record(progress, ((Entities.CustomPrincipal)Page.User).UserName);
        }

        private void btnUpdateAttachedPCVs_Click(object sender, EventArgs e)
        {
            int pcvRedemptionStatus = int.Parse(rcbPCVAction.SelectedItem.Value);
            string clientContact = txtClientContact.Text;
            HiddenField hidCheckForChanges = pnlAttachedPCVDetails.FindControl("hidCheckForChanges") as HiddenField;
            List<Entities.PCV> selectedPCVs = new List<Entities.PCV>();
            Facade.IPCV facPCV = new Facade.PCV();

            foreach (ListViewItem lvi in lvAttachedPCVs.Items)
            {
                CheckBox chkSelectedPCV = lvi.FindControl("chkSelectedPCV") as CheckBox;

                if (chkSelectedPCV.Checked)
                {
                    HiddenField hidPCVID = lvi.FindControl("hidPCVID") as HiddenField;
                    int pcvID = int.Parse(hidPCVID.Value);

                    Entities.PCV upcv = this.UpdatedPCVs.First(lpcv => lpcv.PCVId == pcvID);
                    this.UpdatedPCVs.Remove(upcv);

                    upcv.PCVRedemptionDetailStatus = (ePCVRedemptionDetailStatus)pcvRedemptionStatus;
                    upcv.ClientContact = clientContact;

                    if (pcvRedemptionStatus == (int)ePCVRedemptionDetailStatus.Refused_PCV_Signed && (upcv.NoOfSignings + 1 <= 3))
                        upcv.NoOfSignings = upcv.NoOfSignings + 1;

                    selectedPCVs.Add(upcv);
                    this.UpdatedPCVs.Add(upcv);

                    if (hidCheckForChanges != null)
                        hidCheckForChanges.Value = bool.TrueString;
                }
            }

            if (selectedPCVs.Count > 0)
            {
                int trailerResourceId = int.Parse(hidTrailerResourceId.Value);
                facPCV.UpdateAttachedPCVs(m_job, m_pointId, trailerResourceId, m_instructionId, selectedPCVs, ((Entities.CustomPrincipal)Page.User).UserName);
            }

            CheckBox chkHeaderSelect = lvAttachedPCVs.FindControl("chkHeaderSelect") as CheckBox;
            if (chkHeaderSelect != null && chkHeaderSelect.Checked)
                chkHeaderSelect.Checked = false;

            lvAttachedPCVs.DataSource = this.UpdatedPCVs;
            lvAttachedPCVs.DataBind();

            GetEmptyPalletCount();
        }

        protected void btnResetPCV_Click(object sender, EventArgs e)
        {
            LinkButton lbResetPCV = sender as LinkButton;
            int pcvID = int.Parse(lbResetPCV.CommandArgument);
            int trailerResourceId = int.Parse(hidTrailerResourceId.Value);

            Facade.IPCV facPCV = new Facade.PCV();
            Entities.FacadeResult facRes = facPCV.ResetRedeemedPCV(pcvID, m_jobId, m_job.JobType, m_instructionId, m_pointId, trailerResourceId, ((Entities.CustomPrincipal)Page.User).UserName);

            if (facRes.Success)
            {
                this.UpdatedPCVs = null;
                BindInstruction();
            }
            else
            {
                infringementDisplay.Infringements = facRes.Infringements;
                infringementDisplay.DisplayInfringments();
            }
        }

        #endregion

        #region Dialog Events

        void dlgResource_DialogCallBack(object sender, EventArgs e)
        {
            // Bind the job information.
            BindJob();

            // Load the appropriate instruction.
            BindInstruction();
        }

        void dlgDriverClockIn_DialogCallBack(object sender, EventArgs e)
        {
            Dialog cDialog = sender as Dialog;

            int driverResourceID;
            int.TryParse(hidDriverResourceID.Value, out driverResourceID);

            lblStartTime.Text = string.Format("<a href=\"javascript:ClockIn({0});\">{1}</a>", driverResourceID.ToString(), cDialog.ReturnValue);
            imgClockIn.Visible = false;
            imgClockIn.Style.Add("display", "none");
        }

        #endregion

        #region ListView Events

        void lvAttachedPCVs_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                Entities.PCV cpcv = ((ListViewDataItem)e.Item).DataItem.CastByExample<Entities.PCV>(new Entities.PCV());
                Label lblNoOfSignings = e.Item.FindControl("lblNoOfSignings") as Label;
                CheckBox chkSelectedPCV = e.Item.FindControl("chkSelectedPCV") as CheckBox;
                LinkButton lbResetPCV = e.Item.FindControl("lbResetPCV") as LinkButton;
                HiddenField hidPCVID = e.Item.FindControl("hidPCVID") as HiddenField;
                HtmlTableRow rpcv = e.Item.FindControl("rpcv") as HtmlTableRow;

                hidPCVID.Value = cpcv.PCVId.ToString();

                if (cpcv.PCVRedemptionDetailStatus == ePCVRedemptionDetailStatus.Refused_PCV_Signed)
                    lblNoOfSignings.Text = cpcv.NoOfSignings.ToString();
                else
                    lblNoOfSignings.Text = cpcv.NoOfSignings.ToString();

                switch (cpcv.PCVRedemptionDetailStatus)
                {
                    case ePCVRedemptionDetailStatus.Redeemed:
                        rpcv.Style.Add("background-color", "#A7F1A7");
                        chkSelectedPCV.Enabled = false;
                        lbResetPCV.Visible = true;
                        break;

                    case ePCVRedemptionDetailStatus.Refused_PCV_Signed:
                        rpcv.Style.Add("background-color", "#AEBFDB");
                        break;

                    case ePCVRedemptionDetailStatus.Refused_PCV_Unsigned:
                        rpcv.Style.Add("background-color", "#FFB6B3");
                        break;

                    default:
                        rpcv.Style.Remove("background-color");
                        break;
                }
            }
        }

        void lvPalletHandling_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                RadNumericTextBox rntActualPallets = e.Item.FindControl("rntActualPallets") as RadNumericTextBox;

                ListViewDataItem lvdi = e.Item as ListViewDataItem;
                CollectDropWithCollectDropActual currentItem = lvdi.DataItem as CollectDropWithCollectDropActual;

                if (currentItem.CollectDropActual != null)
                    rntActualPallets.Value = currentItem.CollectDropActual.NumberOfPallets;
            }
        }

        void lvDeHirePalletHandling_ItemDataBound(object sender, ListViewItemEventArgs e)
        {
            if (e.Item.ItemType == ListViewItemType.DataItem)
            {
                Label lblDeHirePalletsFor = e.Item.FindControl("lblDeHirePalletsFor") as Label;
                DropDownList cboDeHireReceiptType = e.Item.FindControl("cboDeHireReceiptType") as DropDownList;
                ListViewDataItem lvdi = e.Item as ListViewDataItem;
                CollectDropWithCollectDropActual currentItem = lvdi.DataItem as CollectDropWithCollectDropActual;

                cboDeHireReceiptType.DataSource = Utilities.UnCamelCase(Enum.GetNames(typeof(eDehiringReceiptType)));
                cboDeHireReceiptType.DataBind();

                using (Facade.IOrganisation facOrganisation = new Facade.Organisation())
                    lblDeHirePalletsFor.Text = facOrganisation.GetForIdentityId(currentItem.CollectDrop.Order.CustomerIdentityID).OrganisationName;

                if (currentItem.CollectDropActual != null)
                {
                    RadNumericTextBox rntActualPallets = e.Item.FindControl("rntActualPallets") as RadNumericTextBox;
                    rntActualPallets.Value = currentItem.CollectDropActual.NumberOfPallets;

                    TextBox txtDeHireReceipt = e.Item.FindControl("txtDeHireReceipt") as TextBox;
                    txtDeHireReceipt.Text = currentItem.CollectDropActual.DehiringReceipt.ReceiptNumber;

                    cboDeHireReceiptType.ClearSelection();
                    cboDeHireReceiptType.Items.FindByValue(Utilities.UnCamelCase(Enum.GetName(typeof(eDehiringReceiptType), currentItem.CollectDropActual.DehiringReceipt.ReceiptType))).Selected = true;
                }
            }
        }

        #endregion

        #region Repeater Events

        void repClient_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                KeyValuePair<int, List<Entities.CollectDrop>> de = (KeyValuePair<int, List<Entities.CollectDrop>>)e.Item.DataItem;
                // populate the Client Name
                List<Entities.CollectDrop> cds = de.Value;
                Label lblClient = e.Item.FindControl("lblClient") as Label;

                if (m_job.JobType == eJobType.Groupage)
                {

                    lblClient.Text = cds[0].Order.CustomerName;
                    DataSet ds = Facade.PalletType.GetAllPalletTypes(cds[0].Order.CustomerIdentityID);
                    organisationPalletTypes = ds.Tables[0].Rows.Cast<DataRow>().AsEnumerable().Where(dr => dr.Field<bool>("IsActive") == true);
                }
                else
                    lblClient.Visible = false;

                // Bind the Pallet Types for This Set of Orders
                Dictionary<int, List<Entities.CollectDrop>> CustomerPalletTypes = new Dictionary<int, List<Orchestrator.Entities.CollectDrop>>();
                List<Entities.CollectDrop> palletTypesCD = new List<Orchestrator.Entities.CollectDrop>();
                if (m_job.JobType == eJobType.Groupage)
                {
                    foreach (Entities.CollectDrop cd in de.Value)
                    {
                        if (!CustomerPalletTypes.ContainsKey(cd.Order.PalletTypeID))
                        {
                            palletTypesCD = new List<Orchestrator.Entities.CollectDrop>();
                            palletTypesCD.Add(cd);
                            CustomerPalletTypes.Add(cd.Order.PalletTypeID, palletTypesCD);
                        }
                        else
                        {
                            palletTypesCD = CustomerPalletTypes[cd.Order.PalletTypeID];
                            palletTypesCD.Add(cd);
                        }
                    }
                }
                else
                {
                    // As this is for a non Groupage Job (Richards only) we need to specify a pallet type
                    foreach (Entities.CollectDrop cd in de.Value)
                        palletTypesCD.Add(cd);

                    CustomerPalletTypes.Add(1, palletTypesCD);
                }

                Repeater repPaletTypes = e.Item.FindControl("repPalletType") as Repeater;
                repPaletTypes.ItemDataBound += new RepeaterItemEventHandler(repPaletTypes_ItemDataBound);
                repPaletTypes.DataSource = CustomerPalletTypes;
                repPaletTypes.DataBind();
            }
        }

        protected void repOrderHandling_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Entities.CollectDrop docket = (Entities.CollectDrop)e.Item.DataItem;
                Label lblWeightCode = e.Item.FindControl("lblWeightCode") as Label;

                lblWeightCode.Text = Facade.WeightType.GetForWeightTypeId(docket.Order.WeightTypeID).ShortCode;

                // If there are any goods refused on this order allow them to be "taken off the trailer"
                // TF 15/12/09: Commented out as goods are automatically taken off trailer at cross dock/drop.
                //if (docket.Order.HasRefusal)
                //{
                //    CheckBox chkTakeOffTrailer = e.Item.FindControl("chkTakeOffTrailer") as CheckBox;
                //    chkTakeOffTrailer.Visible = true;
                //}
            }
        }

        void repReturns_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Entities.CollectDrop returnDocket = (Entities.CollectDrop)e.Item.DataItem;

                // Get the actual for this docket
                Facade.ICollectDropActual facCollectDropActual = new Facade.CollectDrop();
                Entities.CollectDropActual returnActualDocket = facCollectDropActual.GetForCollectDropId(returnDocket.CollectDropId);

                if (returnActualDocket != null)
                {
                    ((HtmlInputHidden)e.Item.FindControl("hidCollectDropActualId")).Value = returnActualDocket.CollectDropActualId.ToString();
                    int goodsRefusalId = Convert.ToInt32(((HtmlInputHidden)e.Item.FindControl("hidGoodsRefusalId")).Value);
                    using (Facade.IGoodsRefusal facGoodsRefusal = new Facade.GoodsRefusal())
                    {
                        Entities.GoodsRefusal goods = facGoodsRefusal.GetForRefusalId(goodsRefusalId);
                        ((TextBox)e.Item.FindControl("txtReturnReceiptNumber")).Text = goods.RefusalReceiptNumber;
                    }
                }

                // Configure the no form val list for store goods
                RequiredFieldValidator rfvReturnReceiptNumber = (RequiredFieldValidator)e.Item.FindControl("rfvReturnReceiptNumber");
            }
        }

        CustomValidator cfvPCVVoucherCode = null;
        int collectDropCount = 0, currentCollectDropCounter = 0;
        void repPaletTypes_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataAccess.Organisation dacOrg = new DataAccess.Organisation();

                // Get the data item and convert to the KVP
                KeyValuePair<int, List<Entities.CollectDrop>> kvp = (KeyValuePair<int, List<Entities.CollectDrop>>)e.Item.DataItem;
                Panel pnlPalletTypePCV = e.Item.FindControl("pnlPalletTypePCV") as Panel;

                RadComboBox rcbReasonForIssue = e.Item.FindControl("rcbReasonForIssue") as RadComboBox;
                rcbReasonForIssue.DataSource = Utilities.GetEnumPairs<ePCVReasonForIssuing>();
                rcbReasonForIssue.DataTextField = "Value";
                rcbReasonForIssue.DataValueField = "Key";
                rcbReasonForIssue.DataBind();

                if (m_job.JobType == eJobType.Groupage)
                {
                    #region Groupage
                    // Determine if we are tracking pallets of this type for this client.
                    // If we are not there is no need to capture anything about them. Just default the values
                    HiddenField hidTrackingpalletType = e.Item.FindControl("hidTrackingpalletType") as HiddenField;
                    HiddenField hidCaptureDebriefForClient = e.Item.FindControl("hidCaptureDebriefForClient") as HiddenField;
                    HiddenField hidCollectDropCount = e.Item.FindControl("hidCollectDropCount") as HiddenField;

                    trackingPalletType = DataAccess.PalletType.TrackingPalletTypeForClient(kvp.Value[0].Order.CustomerIdentityID, kvp.Value[0].Order.PalletTypeID);
                    captureDebriefs = dacOrg.CaptureDeliveryDebriefsForClient(kvp.Value[0].Order.CustomerIdentityID);

                    hidTrackingpalletType.Value = trackingPalletType.ToString();
                    hidCaptureDebriefForClient.Value = captureDebriefs.ToString();
                    hidCollectDropCount.Value = kvp.Value.Count.ToString();

                    collectDropCount = kvp.Value.Count;
                    currentCollectDropCounter = 0;

                    if (trackingPalletType)
                        AnyPalletTypesTracked = trackingPalletType;

                    if (captureDebriefs)
                        AnyClientRequiresDebrief = captureDebriefs;

                    btnHandlePallets.Visible = btnHandlePallets.Enabled = m_canEdit && !_beingInvoiced && AnyPalletTypesTracked && AnyClientRequiresDebrief;

                    Label lblPalletType = e.Item.FindControl("lblPalletType") as Label;
                    lblPalletType.Text = kvp.Value[0].Order.PalletType;

                    // Bind the Pallet type name
                    Label lblPalletTypeFooter = e.Item.FindControl("lblPalletTypeFooter") as Label;

                    HiddenField hidPalletType = e.Item.FindControl("hidPalletType") as HiddenField;
                    HiddenField hidClientID = e.Item.FindControl("hidClientID") as HiddenField;

                    TextBox txtPCVVoucherCode = e.Item.FindControl("txtPCVVoucherCode") as TextBox;
                    TextBox txtPCVPallets = e.Item.FindControl("txtPCVPallets") as TextBox;

                    CheckBox chkNoPCVIssued = e.Item.FindControl("chkNoPCVIssued") as CheckBox;

                    RequiredFieldValidator rfvPCVPallets = e.Item.FindControl("rfvPCVPallets") as RequiredFieldValidator;

                    CustomValidator cfvPCVPallets = e.Item.FindControl("cfvPCVPallets") as CustomValidator;
                    cfvPCVVoucherCode = e.Item.FindControl("cfvPCVVoucherCode") as CustomValidator;

                    lblPalletTypeFooter.Text = kvp.Value[0].Order.PalletType;
                    hidPalletType.Value = kvp.Value[0].Order.PalletTypeID.ToString();
                    hidClientID.Value = kvp.Value[0].Order.CustomerIdentityID.ToString();

                    chkNoPCVIssued.Attributes.Add("onclick", string.Format("javascript:NoPCVIssuedForDeliveryPoint({0}, {1}, '{2}', {3}, {4}, this);", txtPCVPallets.ClientID, txtPCVVoucherCode.ClientID, rcbReasonForIssue.ClientID, rfvPCVPallets.ClientID, cfvPCVPallets.ClientID));

                    Page.ClientScript.RegisterExpandoAttribute(cfvPCVPallets.ClientID, "txtPCVPallets", txtPCVPallets.ClientID, false);
                    Page.ClientScript.RegisterExpandoAttribute(cfvPCVPallets.ClientID, "hidCollectDropCount", hidCollectDropCount.ClientID, false);

                    if (trackingPalletType && captureDebriefs)
                    {
                        if (_PCVS.Count > 0)
                            foreach (Entities.PCV ipcv in _PCVS)
                                if ((ipcv.InstructionID == int.Parse(this.hidInstructionId.Value)) && (ipcv.ClientID == kvp.Value[0].Order.CustomerIdentityID) && (ipcv.PalletTypeID == kvp.Value[0].Order.PalletTypeID))
                                {
                                    txtPCVVoucherCode.Text = ipcv.VoucherNo.ToString();
                                    txtPCVPallets.Text = ipcv.NoOfPalletsOutstanding.ToString();
                                    chkNoPCVIssued.Checked = !ipcv.PCVIssued;
                                    rcbReasonForIssue.SelectedValue = ((int)ipcv.PCVReasonForIssuingId).ToString();

                                    if (ipcv.NoOfPalletsOutstanding > 0)
                                        pnlPalletTypePCV.Attributes.Add("visible", "true");

                                    if (chkNoPCVIssued.Checked)
                                    {
                                        rcbReasonForIssue.Enabled = txtPCVVoucherCode.Enabled = txtPCVPallets.Enabled = !chkNoPCVIssued.Checked;
                                        txtPCVPallets.Style.Add("background-color", "#B8B8B8");
                                        txtPCVVoucherCode.Style.Add("background-color", "#B8B8B8");
                                    }
                                }
                    }
                    else
                        rfvPCVPallets.Enabled = cfvPCVPallets.Enabled = cfvPCVVoucherCode.Enabled = false;

                    #endregion
                }
                else
                {
                    #region Normal

                    // we always track as they have no choice at the momment and cannot specify pallet type
                    HiddenField hidPalletType = e.Item.FindControl("hidPalletType") as HiddenField;
                    HiddenField hidInstructionID = e.Item.FindControl("hidInstructionID") as HiddenField;
                    HiddenField hidClientID = e.Item.FindControl("hidClientID") as HiddenField;
                    CheckBox chkNoPCVIssued = e.Item.FindControl("chkNoPCVIssued") as CheckBox;
                    TextBox txtPCVVoucherCode = e.Item.FindControl("txtPCVVoucherCode") as TextBox;
                    TextBox txtPCVPallets = e.Item.FindControl("txtPCVPallets") as TextBox;
                    hidClientID.Value = m_job.IdentityId.ToString();
                    hidPalletType.Value = "1";

                    if (_PCVS.Count > 0)
                        foreach (Entities.PCV ipcv in _PCVS)
                            if ((ipcv.InstructionID == int.Parse(this.hidInstructionId.Value)) && (ipcv.ClientID == kvp.Value[0].Order.CustomerIdentityID) && (ipcv.PalletTypeID == kvp.Value[0].Order.PalletTypeID))
                            {
                                txtPCVVoucherCode.Text = ipcv.VoucherNo.ToString();
                                txtPCVPallets.Text = ipcv.NoOfPalletsOutstanding.ToString();
                                chkNoPCVIssued.Checked = !ipcv.PCVIssued;
                                rcbReasonForIssue.SelectedValue = ((int)ipcv.PCVReasonForIssuingId).ToString();
                                pnlPalletTypePCV.Attributes.Add("visible", "true");

                                if (chkNoPCVIssued.Checked)
                                {
                                    txtPCVVoucherCode.Enabled = txtPCVPallets.Enabled = !chkNoPCVIssued.Checked;
                                    txtPCVPallets.Style.Add("background-color", "#B8B8B8");
                                    txtPCVVoucherCode.Style.Add("background-color", "#B8B8B8");
                                }
                            }

                    #endregion
                }

                // Bind up the Child Repeater which is the collect drops
                Repeater repCollectDrop = e.Item.FindControl("repCollectDrop") as Repeater;
                repCollectDrop.ItemDataBound += new RepeaterItemEventHandler(repCollectDrop_ItemDataBound);
                repCollectDrop.DataSource = kvp.Value;
                repCollectDrop.DataBind();
            }
        }

        void repCollectDrop_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Entities.CollectDrop drop = (Entities.CollectDrop)e.Item.DataItem;

                if (m_job.JobType == eJobType.Normal)
                {
                    (e.Item.FindControl("lblDocketNumberText") as Label).Text = m_organisation.DocketNumberText;
                    (e.Item.FindControl("lblDocketDisplay") as Label).Text = drop.Docket;
                    (e.Item.FindControl("lblCustomerReferenceDisplay") as Label).Text = "Client's Customer Ref";
                    (e.Item.FindControl("lblCustomerReference") as Label).Text = drop.ClientsCustomerReference;
                }
                else
                {
                    (e.Item.FindControl("lblDocketNumberText") as Label).Text = "Customer Order No";
                    (e.Item.FindControl("lblDocketDisplay") as Label).Text = drop.Order.CustomerOrderNumber;
                    (e.Item.FindControl("lblCustomerReferenceDisplay") as Label).Text = "Delivery Order No";
                    (e.Item.FindControl("lblCustomerReference") as Label).Text = drop.Order.DeliveryOrderNumber;

                    Facade.IOrganisation facOrg = new Facade.Organisation();
                    string customerName = facOrg.GetNameForIdentityId(drop.Order.CustomerIdentityID);
                }

                // Get the load docket for this drop.
                Facade.ICollectDrop facCollectDrop = new Facade.CollectDrop();
                Entities.CollectDrop load = facCollectDrop.GetForOrder(drop.OrderID, m_jobId, eInstructionType.Load);
                // Get the actuals for the load and drop.
                Facade.ICollectDropActual facCollectDropActual = new Facade.CollectDrop();
                Entities.CollectDropActual loadActual = facCollectDropActual.GetForCollectDropId(load.CollectDropId);
                Entities.CollectDropActual dropActual = facCollectDropActual.GetForCollectDropId(drop.CollectDropId);

                // Controls that are populated by the load actual.
                TextBox txtQuantityDespatched = (TextBox)e.Item.FindControl("txtQuantityDespatched");
                TextBox txtPalletsDespatched = (TextBox)e.Item.FindControl("txtPalletsDespatched");
                TextBox txtWeightDespatched = (TextBox)e.Item.FindControl("txtWeightDespatched");
                TextBox txtPalletsDelivered = (TextBox)e.Item.FindControl("txtPalletsDelivered");
                Label lblPalletsReturned = (Label)e.Item.FindControl("lblPalletsReturned");
                HtmlTable tblPalletReturned = e.Item.FindControl("tblPalletReturned") as HtmlTable;

                if (loadActual == null)
                {
                    txtQuantityDespatched.Text = load.NoCases.ToString();
                    txtPalletsDespatched.Text = load.NoPallets.ToString();
                    txtWeightDespatched.Text = load.Weight.ToString("F0");
                    txtPalletsDelivered.Text = load.NoPallets.ToString();
                }
                else
                {
                    txtQuantityDespatched.Text = loadActual.NumberOfCases.ToString();
                    txtPalletsDespatched.Text = loadActual.NumberOfPallets.ToString();
                    txtWeightDespatched.Text = loadActual.Weight.ToString("F0");
                    txtPalletsDelivered.Text = loadActual.NumberOfPallets.ToString();
                }

                // Controls that are populated by the drop actual.
                
                HiddenField hidPalletsPrevious = e.Item.FindControl("hidPalletsPrevious") as HiddenField;

                HtmlInputHidden hidCollectDropActualId = (HtmlInputHidden)e.Item.FindControl("hidCollectDropActualId");
                
                Label lblCustomerReference = (Label)e.Item.FindControl("lblCustomerReference");
                Label lblPCVRequired = (Label)e.Item.Parent.Parent.FindControl("lblPCVRequired");
                
                TextBox txtQuantityDelivered = (TextBox)e.Item.FindControl("txtQuantityDelivered");
                TextBox txtShortageReference = (TextBox)e.Item.FindControl("txtQuantityShortageReference");
                TextBox txtPalletsReturned = (TextBox)e.Item.FindControl("txtPalletsReturned");
                TextBox txtWeightDelivered = (TextBox)e.Item.FindControl("txtWeightDelivered");
                TextBox txtPCVPallets = (TextBox)e.Item.Parent.Parent.FindControl("txtPCVPallets");

                Panel pnlPalletTypePCV = (Panel)e.Item.Parent.Parent.FindControl("pnlPalletTypePCV");

                RadComboBox rcbPalletType = e.Item.FindControl("rcbPalletType") as RadComboBox;

                CustomValidator cfvPCVPallets = e.Item.Parent.Parent.FindControl("cfvPCVPallets") as CustomValidator;

                if (dropActual == null)
                {
                    txtQuantityDelivered.Text = load.NoCases.ToString();
                    txtWeightDelivered.Text = load.Weight.ToString("F0");

                    if (rcbPalletType != null)
                    {
                        rcbPalletType.ClearSelection();

                        foreach (DataRow dr in organisationPalletTypes)
                        {
                            // Must capture debrief still has to be enabled for pallet tracking.
                            bool isTracked = !AnyClientRequiresDebrief ? false : AnyClientRequiresDebrief && dr.Field<bool>("TrackPallets");

                            RadComboBoxItem rcbi = new RadComboBoxItem(dr.Field<string>("Description"), dr.Field<int>("PalletTypeID").ToString());
                            rcbi.Attributes.Add("IsTracked", isTracked.ToString());
                            rcbPalletType.Items.Add(rcbi);
                        }

                        if (rcbPalletType.Items.Count > 0 && rcbPalletType.FindItemByValue(load.PalletTypeID.ToString()) != null)
                            rcbPalletType.FindItemByValue(load.PalletTypeID.ToString()).Selected = true;
                    }
                }
                else
                {
                    if (rcbPalletType != null)
                        rcbPalletType.Style.Add("display", "none");

                    // Show the PCV for the Collect Drop
                    hidCollectDropActualId.Value = dropActual.CollectDropActualId.ToString();
                    txtQuantityDelivered.Text = dropActual.NumberOfCases.ToString();
                    txtShortageReference.Text = dropActual.ShortageReference;
                    txtPalletsDelivered.Text = dropActual.NumberOfPallets.ToString();
                    txtPalletsReturned.Text = dropActual.NumberOfPalletsReturned.ToString();
                    hidPalletsPrevious.Value = (dropActual.NumberOfPallets - dropActual.NumberOfPalletsReturned).ToString();
                    txtWeightDelivered.Text = dropActual.Weight.ToString("F0");
                }

                txtPalletsReturned.Attributes.Add("onblur", "javascript:PromptForPCV('" + txtPalletsDelivered.ClientID + "', '" + txtPalletsReturned.ClientID + "', " + "'" + txtPCVPallets.ClientID + "', '" + pnlPalletTypePCV.ClientID + "', '" + hidPalletsPrevious.ClientID + "', '" + lblTotalEmptyPalletCount.ClientID + "', '" + cfvPCVVoucherCode.ClientID + "', '" + cfvPCVPallets.ClientID + "');");

                Page.ClientScript.RegisterExpandoAttribute(cfvPCVPallets.ClientID, string.Format("txtPalletsDelivered{0}", currentCollectDropCounter), txtPalletsDelivered.ClientID, false);
                Page.ClientScript.RegisterExpandoAttribute(cfvPCVPallets.ClientID, string.Format("txtPalletsReturned{0}", currentCollectDropCounter), txtPalletsReturned.ClientID, false);

                currentCollectDropCounter++;

                if (trackingPalletType && captureDebriefs)
                {
                    tblPalletReturned.Style.Add("display", "");
                    pnlPalletTypePCV.Attributes.Add("visible", "true");

                    // Configure the "prompt for pcv" functionality
                    if (dropActual != null) 
                    {
                        if (dropActual.NumberOfPalletsReturned < dropActual.NumberOfPallets)
                            lblPCVRequired.Visible = false;
                        else
                            pnlPalletTypePCV.Attributes.Remove("visible");
                    }
                }
                else
                    tblPalletReturned.Style.Add("display", "none");

            }
        }

        void repPCV_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            // Determine if we are tracking the pallets for the client and bind up only those necessary
            Facade.IOrganisation facOrg = new Facade.Organisation();
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                Label lblPalletType = e.Item.FindControl("lblPalletType") as Label;
                TextBox txtPallets = e.Item.FindControl("txtPallets") as TextBox;
                HiddenField hidPCVInstructionID = e.Item.FindControl("hidPCVInstructionID") as HiddenField;
                HiddenField hidPCVOrderID = e.Item.FindControl("hidPCVOrderID") as HiddenField;
                HiddenField hidPCVCollectDropID = e.Item.FindControl("hidPCVCollectDropID") as HiddenField;

                Entities.CollectDrop cd = e.Item.DataItem as Entities.CollectDrop;
                string customerName = facOrg.GetNameForIdentityId(cd.Order.CustomerIdentityID);
                lblPalletType.Text = customerName + "( " + cd.Order.PalletType + " )";
                lblPalletType.Attributes.Add("PalletTypeID", cd.Order.PalletTypeID.ToString());
                hidPCVCollectDropID.Value = cd.CollectDropId.ToString();
                hidPCVInstructionID.Value = cd.InstructionID.ToString();
                hidPCVOrderID.Value = cd.OrderID.ToString();
            }
        }

        #endregion

        #region Validation Events

        private void cfvDepartureDate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = false;

            // The departure date must be after the arrival date.
            if (rfvArrivalDate.IsValid && rfvArrivalTime.IsValid && rfvDepartureTime.IsValid)
            {
                DateTime arrivalDateTime = dteArrivalDate.SelectedDate.Value;
                arrivalDateTime = arrivalDateTime.Subtract(arrivalDateTime.TimeOfDay);
                arrivalDateTime = arrivalDateTime.Add(dteArrivalTime.SelectedDate.Value.TimeOfDay);

                DateTime departureDateTime = dteDepartureDate.SelectedDate.Value;
                departureDateTime = departureDateTime.Subtract(departureDateTime.TimeOfDay);
                departureDateTime = departureDateTime.Add(dteDepartureTime.SelectedDate.Value.TimeOfDay);

                if (arrivalDateTime <= departureDateTime)
                    args.IsValid = true;
            }
        }

        #endregion
    }
}
