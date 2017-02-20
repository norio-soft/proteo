using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Web;
using System.Web.Caching;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using Telerik.Web.UI;
using System.Web.Services;
using System.Web.Script.Services;
using AjaxControlToolkit;

using Orchestrator.Globals;

namespace Orchestrator.WebUI.Traffic
{
    public partial class ResourceThis : Orchestrator.Base.BasePage
    {
        #region Properties
        protected bool PendingUpdates = false;
        protected Entities.Job _job = null;
        protected Entities.InstructionCollection _instructions = null;
        protected Entities.Instruction _instruction = null;
        protected List<Entities.JobSubContractor> _jobSubContractors = null;
        protected Entities.LegPlan _legPlan = null;
        protected bool _selectedLegSubbedOutPerLeg = false;
        protected bool _selectedLegSubbedOutWholeJob = false;
        protected bool _selectedLegSubbedOutPerOrder = false;

        protected string Driver, TrailerRef, RegNo = string.Empty;
        protected int InstructionId, DriverResourceId, TrailerResourceId, VehicleResourceId, firstUseInstructionId;
        //protected int JobId = 0;
        protected eJobType jobType = eJobType.Normal;

        protected int controlAreaId = 2;
        protected int[] ta;
        private int itemsPerRequest = 30;
        private int depotId = 0;
        private int _subbedLegCheckedCount = 0;

        private const string showLoadOrderTemplate = @"javascript:openResizableDialogWithScrollbars('LoadOrder.aspx?jobid={0}', '700', '258');";

        private DataSet controlAreas = null;
        private DataSet trafficAreas = null;

        private const string vs_updateOnClose = "vs_updateOnClose";
        protected bool UpdateOnClose
        {
            get { return ViewState[vs_updateOnClose] == null ? false : (bool)ViewState[vs_updateOnClose]; }
            set { ViewState[vs_updateOnClose] = value; }
        }

        protected int JobId { get; set; }
        protected int driverID { get; set; }
        protected int vehicleID { get; set; }
        protected int trailerID { get; set; }
        protected long startDate { get; set; }
        protected long endDate { get; set; }

        protected bool resourceIsAvailableCheck { get; set; }
        #endregion

        #region Cookie Stuff

        private readonly string C_TRAFFIC_SHEET_XML = "TrafficSheetFilterXML";
        protected Entities.TrafficSheetFilter _trafficSheetFilter = null;

        private void LoadCookieDefaults()
        {
            try
            {
                _trafficSheetFilter = Utilities.GetFilterFromCookie(this.CookieSessionID, Request);
            }
            catch
            {
                // invalid or old values stored in the cookie simply regenerate.
                _trafficSheetFilter = Utilities.GenerateCookie(this.CookieSessionID, this.Response, ((Entities.CustomPrincipal)Page.User).IdentityId);
            }

            // Of course, the GetFilterFromCookie method doesn't throw an exception...
            if (_trafficSheetFilter == null)
            {
                _trafficSheetFilter = Utilities.GenerateCookie(this.CookieSessionID, this.Response, ((Entities.CustomPrincipal)Page.User).IdentityId);
            }
            
            //Control Area and Depot
            controlAreaId = _trafficSheetFilter.ControlAreaId;
            depotId = _trafficSheetFilter.DepotId;

            //Traffic Areas
            ta = new int[_trafficSheetFilter.TrafficAreaIDs.Count];
            _trafficSheetFilter.TrafficAreaIDs.CopyTo(ta);
        }


        #endregion-

        #region Page / OnInit

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(Request.QueryString["rcbID"]) && !Page.IsCallback)
                return;

            resourceIsAvailableCheck = Orchestrator.Globals.Configuration.ResourceIsAvailableCheck;

            InstructionId = int.Parse(Request.QueryString["iID"]);
            JobId = int.Parse(Request.QueryString["jobId"]);
            DateTime sd = DateTime.Parse(Request.QueryString["LS"]);
            DateTime ed = DateTime.Parse(Request.QueryString["LE"]);
            long minDateTime = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).Ticks;
            startDate = (long)((sd.ToUniversalTime().Ticks - minDateTime) / 10000);
            endDate = (long)((ed.ToUniversalTime().Ticks - minDateTime) / 10000);

            int result;
            driverID = (int.TryParse(Request.QueryString["DR"], out result) ? result : -1);
            vehicleID = (int.TryParse(Request.QueryString["VR"], out result) ? result : -1);
            trailerID = (int.TryParse(Request.QueryString["TR"], out result) ? result : -1);

            int.TryParse(Request.QueryString["depotId"], out depotId);
            //// If possible, use the job from the cache if we have it to avoid a separate call to get the job type
            //_job = (Entities.Job)Cache.Get("JobEntityForJobId" + JobId);
            //if (_job == null)
            //{
            //    //Get the job if its not in the cache as it is now required later
                Facade.IJob facJob = new Facade.Job();
                _job = facJob.GetJob(JobId, true, true);
            //}

            jobType = _job.JobType;

            //Load the Traffic Areas, Control Area and Depot from the cookie or defaults
            LoadCookieDefaults();

            //Override these values if specific values are passed
            if (Request.QueryString["CA"] != null)
                controlAreaId = int.Parse(Request.QueryString["CA"]);

            if (Request.QueryString["TA"] != null)
            {
                List<int> trafficAreaIds = new List<int>();
                string[] taids = (Request.QueryString["TA"]).Split(',');
                foreach (string taid in taids)
                {
                    if (taid == "")
                    {
                        this.ClientScript.RegisterClientScriptBlock(this.GetType(), "TrafficAreaError", "alert('You must select some traffic areas in your traffic sheet filter to view this page.');", true);
                        this.Close();
                        return; 
                    }
                    else
                    {
                        trafficAreaIds.Add(Convert.ToInt32(taid));
                    }
                }

                ta = new int[trafficAreaIds.Count];
                trafficAreaIds.CopyTo(ta);
            }

            if (!IsPostBack)
            {
                // Remove any old legplan from the cache, just in case the user clicked the 'cross' in the top right hand bit of the window.
                Cache.Remove("LegPlanForJobId" + JobId + "UserID" + ((Entities.CustomPrincipal)Page.User).UserName);

                firstUseInstructionId = InstructionId;

                ShowResourceDrawingFrom();

                
                PreLoadSelectedResource();

                btnShowLoadOrder.OnClientClick = string.Format(showLoadOrderTemplate, JobId);

                this.DataBind();
                BindLegs();

                // Ensure that the number of selected subbed out legs is available to client side code.
                hidNumberOfSelectedLegsSubbedOut.Value = _subbedLegCheckedCount.ToString();

                ShowPCVS();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.btnCancel.Click += new EventHandler(btnCancel_Click);
            this.btnConfirm.Click += new EventHandler(btnConfirm_Click);
            this.btnRemoveResource.Click += new EventHandler(btnRemoveResource_Click);
            this.btnUpdateRows.Click += new EventHandler(btnUpdateRows_Click);
            this.btnUncommunicate.Click += new EventHandler(btnUncommunicate_Click);
            this.btnUpdateAndConfirm.Click += new EventHandler(btnUpdateAndConfirm_Click);

            this.cboDriver.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboDriver_ItemsRequested);
            this.cboTrailer.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboTrailer_ItemsRequested);
            this.cboVehicle.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboVehicle_ItemsRequested);
            this.cboThirdPartyTrailerOwner.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboThirdPartyTrailerOwner_ItemsRequested);

            this.grdRun.ItemDataBound += new GridItemEventHandler(grdRun_ItemDataBound);

            this.ramResourceThis.AjaxRequest += new RadAjaxControl.AjaxRequestDelegate(ramResourceThis_AjaxRequest);

            this.dlgGoodsRefused.DialogCallBack += new EventHandler(dlgGoodsRefused_DialogCallBack);
            this.dlgAlterPalletBalance.DialogCallBack += new EventHandler(dlgAlterPalletBalance_DialogCallBack);
        }

        void dlgAlterPalletBalance_DialogCallBack(object sender, EventArgs e)
        {
            if (this.dlgAlterPalletBalance.ReturnValue == "Refresh_Pallets")
            {
                int trailerResourceId = 0;
                int.TryParse(cboTrailer.SelectedValue, out trailerResourceId);

                if (trailerResourceId > 0)
                    GetTrailerContent(trailerResourceId);
                else
                    lvPalletBalances.DataSource = null;
            }
        }

        void dlgGoodsRefused_DialogCallBack(object sender, EventArgs e)
        {
            if (this.dlgGoodsRefused.ReturnValue == "Refresh_Redeliveries_And_Refusals")
            {
                int trailerResourceId = 0;
                int.TryParse(cboTrailer.SelectedValue, out trailerResourceId);

                if (trailerResourceId > 0)
                    GetTrailerContent(trailerResourceId);
                else
                    lvGoodsRefused.DataSource = null;
            }
        }

        #endregion

        #region Private Functions

        private void BindTrafficAreas(int controlAreaId, int trafficAreaId)
        {
            //BindTrafficAreas(controlAreaId);
            Facade.ITrafficArea facTrafficArea = new Facade.Traffic();
            trafficAreas = facTrafficArea.GetForControlAreaId(controlAreaId);
        }

        //private void BindTrafficAreas(int controlAreaId)
        //{
        //    //cboControlArea.ClearSelection();
        //    //cboControlArea.Items.FindByValue(controlAreaId.ToString()).Selected = true;

        //    //Facade.ITrafficArea facTrafficArea = new Facade.Traffic();
        //    //trafficAreas = facTrafficArea.GetForControlAreaId(controlAreaId);
        //    //cboTrafficArea.DataSource = trafficAreas;
        //    //cboTrafficArea.DataBind();
        //    //cboTrafficArea.ClearSelection();
        //}

        private DataTable BindComboBoxItems(DataTable dt, int numberOfItems, out int endOffset)
        {
           
                DataTable boundResults = dt.Clone();

                int itemOffset = numberOfItems;
                endOffset = itemOffset + itemsPerRequest;

                if (endOffset > dt.Rows.Count)
                    endOffset = dt.Rows.Count;
                for (int i = itemOffset; i < endOffset; i++)
                    boundResults.ImportRow(dt.Rows[i]);

                return boundResults;
        }

        private void ShowPCVS()
        {
            Facade.IJob facJob = new Facade.Job();
            Entities.Job _job = facJob.GetJob(JobId);
            if (_job.PCVs.Count > 0)
                divPCVS.Visible = true;
            else
                divPCVS.Visible = false;

            _job = null;
        }

        private void ShowResourceDrawingFrom()
        {
            // Recover or retrieve the control area.
            Entities.ControlArea controlArea = null;
            string controlAreaCacheName = "_controlArea" + controlAreaId.ToString();
            if (Cache[controlAreaCacheName] == null)
            {
                Facade.IControlArea facControlArea = new Facade.Traffic();
                controlArea = facControlArea.GetForControlAreaId(controlAreaId);
                Cache.Add(controlAreaCacheName, controlArea, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(1, 0, 0), System.Web.Caching.CacheItemPriority.Normal, null);
            }
            else
                controlArea = (Entities.ControlArea)Cache[controlAreaCacheName];

            string trafficAreaDescriptions = string.Empty;
            foreach (int taid in ta)
            {
                // Recover or retrieve the traffic area.
                Entities.TrafficArea trafficArea = null;
                Facade.ITrafficArea facTrafficArea = new Facade.Traffic();

                string trafficAreaCacheName = "_trafficArea" + taid.ToString();
                if (Cache[trafficAreaCacheName] == null)
                {
                    trafficArea = facTrafficArea.GetForTrafficAreaId(taid);
                    Cache.Add(trafficAreaCacheName, trafficArea, null, System.Web.Caching.Cache.NoAbsoluteExpiration, new TimeSpan(1, 0, 0), System.Web.Caching.CacheItemPriority.Normal, null);
                }
                else
                    trafficArea = (Entities.TrafficArea)Cache[trafficAreaCacheName];

                trafficAreaDescriptions += trafficArea.TrafficAreaName;
                if (trafficAreaDescriptions.Length > 0)
                    trafficAreaDescriptions += ", ";
            }

            trafficAreaDescriptions = trafficAreaDescriptions.Substring(0, trafficAreaDescriptions.Length - 2);

            if (depotId == 0)
                lblDrawingResourceFrom.Text = "You are currently drawing resource from " + controlArea.ControlAreaName + " (" + trafficAreaDescriptions + ")";
            else
            {
                Facade.IOrganisationLocation facOrganisationLocation = new Facade.Organisation();
                lblDrawingResourceFrom.Text = "You are currently drawing driver and vehicle resource from " + facOrganisationLocation.GetLocationForOrganisationLocationId(depotId).OrganisationLocationName + " along with all trailers";
            }
        }

        private void BindLegs()
        {
            //////////////////////////////////////////////
            // Set values required by leg binding here. //
            //////////////////////////////////////////////

            // Disable check boxes for legs that are subbed out using subby's own trailer.
            if (_job == null)
            {
                // Try and get the job from the cache
                _job = (Entities.Job)Cache.Get("JobEntityForJobId" + JobId);
            }

            if (_job == null)
            {
                // Job is not in the cache
                // Get the job subcontractors from the db 
                Facade.IJobSubContractor facJobSubContractor = new Facade.Job();
                _jobSubContractors = facJobSubContractor.GetSubContractorForJobId(JobId);

                // Get the instruction collection from the db
                _instructions = new Facade.Instruction().GetForJobId(JobId);
            }
            else
            {
                // use the job subcontractor collection from the cached job.
                _jobSubContractors = _job.SubContractors;

                if (_job.Instructions != null)
                {
                    // use the instruction collection from the cached job
                    _instructions = _job.Instructions;
                }
                else
                {
                    // otherwise get a fresh instruction collection
                    _instructions = new Facade.Instruction().GetForJobId(JobId);
                }
            }

            //Put some checks in to find why we are ghetting exceptions
            //(They can be removed once it has been fixed).
            if (_instructions == null)
                throw new ApplicationException("_instructions is null");

            _legPlan = (Entities.LegPlan)Cache.Get("LegPlanForJobId" + JobId + "UserID" + ((Entities.CustomPrincipal)Page.User).UserName);

            if (_legPlan == null)
            {
                #region Get Leg Plan

                _legPlan = new Facade.Instruction().GetLegPlan(_instructions, true);

                if (_legPlan == null)
                    throw new ApplicationException("_legPlan is null");

                UpdateLegPlanCache();

                #endregion
            }

            _instruction = _instructions.GetForInstructionId(InstructionId);
            if (_instruction == null)
                throw new ApplicationException("_instruction is null - InstructionId = " + InstructionId.ToString() + " InstructionsCount = " + _instructions.Count.ToString());

            // Determine if and how the selected leg has been subbed out
            if (_instruction.JobSubContractID > 0)
                _selectedLegSubbedOutPerLeg = true;

            if (_jobSubContractors != null && (_jobSubContractors.Count == 1 && _jobSubContractors[0].SubContractWholeJob))
                _selectedLegSubbedOutWholeJob = true;

            if (jobType == eJobType.Groupage && (_instruction.InstructionTypeId == (int)eInstructionType.Drop && _instruction.CollectDrops != null && (_instruction.CollectDrops.Count > 0 && _instruction.CollectDrops[0].Order.JobSubContractID > 0)))
            {
                // This will be set if the instruction is a drop and has a jobsubcontractid for its related order
                // We don't want to set this flag for instructions that aren't drops, because they cannot be subbed per order
                _selectedLegSubbedOutPerOrder = true;
            }

            ///////////////////
            // Bind the legs //
            ///////////////////

            Facade.IControlArea facControlArea = new Facade.Traffic();
            controlAreas = facControlArea.GetAll();
            //cboControlArea.DataSource = controlAreas;
            //cboControlArea.DataBind();

            

            grdRun.DataSource = _legPlan.Legs();
            grdRun.DataBind();
        }

        private List<Entities.FacadeResult> UpdateRun()
        {
            List<Entities.FacadeResult> results = new List<Entities.FacadeResult>();
            Facade.IInstruction facInstruction = new Facade.Instruction();
            object legPlanFromCache = null;
            DateTime lastUpdateDate = DateTime.Parse(Request.QueryString["LastUpdateDate"]);
            string cacheName = "LegPlanForJobId" + JobId + "UserID" + ((Entities.CustomPrincipal)Page.User).UserName;

            try
            {
                legPlanFromCache = Cache.Get(cacheName);

                if (legPlanFromCache != null)
                {
                    _legPlan = (Entities.LegPlan)legPlanFromCache;

                    results = facInstruction.PlanInstructions(_legPlan, JobId, lastUpdateDate, ((Entities.CustomPrincipal)Page.User).UserName);

                    foreach (Entities.LegView lv in _legPlan.FindAllLegs(fal => fal.GiveToControlArea > 0 || fal.GiveToTrafficArea > 0))
                    {
                        int controlAreaId = lv.Instruction.ControlAreaId;
                        int trafficAreaId = lv.Instruction.TrafficAreaId;

                        if (lv.GiveToControlArea > 0)
                            controlAreaId = lv.GiveToControlArea;

                        if (lv.GiveToTrafficArea > 0)
                            trafficAreaId = lv.GiveToTrafficArea;

                        Facade.IResource facResource = new Facade.Resource();
                        string reason = string.Format("Resource This Job Id:{0} Leg Id:{1}",
                            JobId.ToString(),
                            lv.InstructionID.ToString());

                        if (lv.Driver != null)
                            facResource.AssignToArea(controlAreaId, trafficAreaId, lv.Driver.ResourceId, reason, ((Entities.CustomPrincipal)Page.User).UserName);
                        if (lv.Vehicle != null)
                            facResource.AssignToArea(controlAreaId, trafficAreaId, lv.Vehicle.ResourceId, reason, ((Entities.CustomPrincipal)Page.User).UserName);
                        if (lv.Trailer != null)
                            facResource.AssignToArea(controlAreaId, trafficAreaId, lv.Trailer.ResourceId, reason, ((Entities.CustomPrincipal)Page.User).UserName);
                    }
                }
                else
                    throw new ApplicationException("_LegPlan returned null from the cache - " + cacheName);
            }
            catch (Exception ex)
            {
                throw new ApplicationException("_LegPlan null - " + cacheName, ex);
            }

            return results;
        }

        private Entities.Trailer UseThirdPartyTrailer()
        {
            Entities.Trailer trailer = null;

            int thirdPartyOrganisationIdentityID;
            if (int.TryParse(cboThirdPartyTrailerOwner.SelectedValue, out thirdPartyOrganisationIdentityID))
            {
                Facade.ITrailer facTrailer = new Facade.Resource();

                trailer = facTrailer.GetThirdPartyForTrailerRef(cboTrailer.Text, thirdPartyOrganisationIdentityID);
                if (trailer == null)
                {
                    Facade.IOrganisation facOrganisation = new Facade.Organisation();
                    Entities.Organisation owner = facOrganisation.GetForIdentityId(Orchestrator.Globals.Configuration.IdentityId);

                    // Create a new trailer.
                    // HACK : Force the home point to be the owner's head office.
                    // HACK : Populate using the first description, manufacturer, trailer type - there are not used or displayed so it shouldn't matter.
                    trailer = new Entities.Trailer();
                    trailer.HomePointId = owner.Locations.GetHeadOffice().Point.PointId;
                    trailer.LastPointId = trailer.HomePointId;
                    // This will start to be overwritten once the trailer is used in a call-in.
                    trailer.ThirdPartyOrganisationIdentityID = thirdPartyOrganisationIdentityID;
                    trailer.TrailerDescriptionId = (int)(facTrailer.GetAllTrailerDescriptions().Tables[0].Rows[0]["TrailerDescriptionID"]);
                    trailer.TrailerManufacturerId = (int)(facTrailer.GetAllTrailerManufacturers().Tables[0].Rows[0]["TrailerManufacturerID"]);
                    trailer.TrailerRef = cboTrailer.Text;
                    trailer.TrailerTypeId = (int)(facTrailer.GetAllTrailerTypes().Tables[0].Rows[0]["TrailerTypeID"]);

                    trailer.ResourceId = facTrailer.Create(trailer, ((Entities.CustomPrincipal)Page.User).UserName).ObjectId;
                }

                cboTrailer.SelectedValue = trailer.ResourceId.ToString();
            }

            return trailer;
        }

        private string GetResponse()
        {
            string retVal = "<ResourceThis/>";
            return retVal;
        }

        private void GetTrailerContent(int resourceID)
        {
            Facade.ITrailer facTrailer = new Facade.Resource();
            BusinessRules.ITrailer brTrailer = new BusinessRules.Resource();

            lvPalletBalances.DataSource = facTrailer.GetPalletCountForTrailer(resourceID, false);
            lvPalletBalances.DataBind();

            lvGoodsRefused.DataSource = facTrailer.GetGoodsStoredOnTrailer(resourceID);
            lvGoodsRefused.DataBind();

            Entities.FacadeResult retVal = brTrailer.ValidateAssignmentOfTrailers(resourceID, _job.JobId);

            if (!retVal.Success)
            {
                idErrors.Infringements = retVal.Infringements;
                idErrors.DisplayInfringments();
                idErrors.Visible = true;
                wrapperInfringementDisplay.Style.Add("Display", "");
            }
            else
            {
                Entities.Trailer trailer = facTrailer.GetForTrailerId(resourceID);

                if (trailer != null)
                    lblTrailerDescription.Text = trailer.TrailerType;
                else
                    lblTrailerDescription.Text = string.Empty;

                idErrors.Visible = false;
                wrapperInfringementDisplay.Style.Add("Display", "none");
                cboTrailer.Enabled = true;
            }
        }

        private void UpdateLegPlanCache()
        {
            if (Cache.Get("LegPlanForJobId" + JobId + "UserID" + ((Entities.CustomPrincipal)Page.User).UserName) != null)
            {
                Cache.Remove("LegPlanForJobId" + JobId + "UserID" + ((Entities.CustomPrincipal)Page.User).UserName);
            }

            Cache.Add("LegPlanForJobId" + JobId.ToString() + "UserID" + ((Entities.CustomPrincipal)Page.User).UserName,
                        _legPlan,
                        null,
                        Cache.NoAbsoluteExpiration,
                        TimeSpan.FromMinutes(60),
                        CacheItemPriority.Normal,
                        null);
        }

        private void ResetCaches()
        {
            Cache.Remove("LegPlanForJobId" + JobId + "UserID" + ((Entities.CustomPrincipal)Page.User).UserName);
            Cache.Remove("JobEntityForJobId" + JobId.ToString());

            _job = null;
            _instructions = null;
            _legPlan = null;
        }

        private void ClearComboBoxes()
        {
            cboDriver.ClearSelection();
            cboDriver.Items.Clear();
            cboDriver.Text = string.Empty;

            cboVehicle.ClearSelection();
            cboVehicle.Items.Clear();
            cboVehicle.Text = string.Empty;

            cboTrailer.ClearSelection();
            cboTrailer.Items.Clear();
            cboTrailer.Text = string.Empty;
            lblTrailerDescription.Text = string.Empty;

            cboThirdPartyTrailerOwner.ClearSelection();
        }

        #endregion

        #region Protected Functions

        protected string GetClientDataString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(string.Format("{0}{1}", controlAreaId.ToString(), ":"));

            for (int i = 0; i < ta.Length; i++)
            {
                if (i > 0)
                    sb.Append(",");

                sb.Append(ta[i].ToString());
            }

            sb.Append(string.Format("{0}{1}", ":", depotId.ToString()));
            return sb.ToString();
        }

        #endregion

        #region Events

        #region Ajax Manager

        void ramResourceThis_AjaxRequest(object sender, AjaxRequestEventArgs e)
        {
            string[] arguments = e.Argument.Split('|');

            if (arguments.Length > 1)
                switch (arguments[0])
                {
                    case "trailer":
                        int resourceID = 0;
                        int.TryParse(arguments[1], out resourceID);
                        GetTrailerContent(resourceID);
                        break;
                }
        }

        #endregion

        #region Buttons

        void btnConfirm_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                List<Entities.FacadeResult> results = UpdateRun();

                // Remove the job entity from the cache so that it is renewed.
                Cache.Remove("JobEntityForJobId" + JobId.ToString());

                if (results.Count == 0)
                {
                    UpdateOnClose = true;
                    btnCancel_Click(null, null);
                }
                else
                {
                    foreach (Entities.FacadeResult result in results)
                    {
                        idErrors.Infringements = result.Infringements;
                        idErrors.DisplayInfringments();
                        idErrors.Visible = true;
                        wrapperInfringementDisplay.Style.Add("Display", "");
                    }
                }
            }
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            // Remove the leg plan from the cache.
            Cache.Remove("LegPlanForJobId" + JobId + "UserID" + ((Entities.CustomPrincipal)Page.User).UserName);

            if (UpdateOnClose)
                this.ReturnValue = GetResponse();

            this.Close();
        }

        void btnRemoveResource_Click(object sender, EventArgs e)
        {
            //If removing resources there is nothing to commnuicate and the resources shouldn't be given to any other area.
            List<int> instructionIDs = new List<int>();
            Facade.IInstruction facInstruction = new Facade.Instruction();

            foreach (GridItem gi in grdRun.Items)
                if (gi.ItemType == GridItemType.Item || gi.ItemType == GridItemType.AlternatingItem)
                    if (((CheckBox)gi.FindControl("chkSelectLeg")).Checked)
                        instructionIDs.Add(Convert.ToInt32(((HtmlInputHidden)gi.FindControl("hidInstructionId")).Value));

            if (instructionIDs.Count == 0)
                instructionIDs.Add(InstructionId);

            int driverId = 0, trailerId = 0, vehicleId = 0;

            DateTime lastUpdateDate = DateTime.Parse(Request.QueryString["LastUpdateDate"]);
            Entities.FacadeResult res;

            res = facInstruction.PlanInstruction(instructionIDs, JobId, driverId, vehicleId, trailerId, lastUpdateDate, ((Entities.CustomPrincipal)Page.User).UserName);

            if (res.Success)
            {
                // Remove the leg plan from the cache.
                Cache.Remove("LegPlanForJobId" + JobId + "UserID" + ((Entities.CustomPrincipal)Page.User).UserName);

                this.ReturnValue = "refresh";
                this.Close();
            }
            else
            {
                idErrors.Infringements = res.Infringements;
                idErrors.DisplayInfringments();
                idErrors.Visible = true;
                wrapperInfringementDisplay.Style.Add("Display", "");
            }
        }

        void btnUpdateRows_Click(object sender, EventArgs e)
        {
            _legPlan = (Entities.LegPlan)Cache.Get("LegPlanForJobId" + JobId + "UserID" + ((Entities.CustomPrincipal)Page.User).UserName);
            if (_legPlan != null)
            {
                int driverResourceId = 0, vehicleResourceId = 0, trailerResourceId = 0, controlAreaId = 0, trafficAreaId = 0;

                int.TryParse(cboDriver.SelectedValue, out driverResourceId);
                int.TryParse(cboVehicle.SelectedValue, out vehicleResourceId);
                int.TryParse(cboTrailer.SelectedValue, out trailerResourceId);
                //int.TryParse(cboControlArea.SelectedValue, out controlAreaId);
                //int.TryParse(cboTrafficArea.SelectedValue, out trafficAreaId);

                List<int> instrucionIDs = new List<int>();
                List<int> commnuicatedInstructions = new List<int>();
                List<int> giveResourceInstructions = new List<int>();
                Dictionary<int, KeyValuePair<int, int>> instructionCATA = new Dictionary<int, KeyValuePair<int, int>>();

                foreach (GridItem gi in grdRun.Items)
                {
                    CheckBox chkSelectLeg = gi.FindControl("chkSelectLeg") as CheckBox;
                    CheckBox chkCommunicateLeg = gi.FindControl("chkCommunicateLeg") as CheckBox;
                    HtmlInputHidden hidInstructionId = gi.FindControl("hidInstructionId") as HtmlInputHidden;
                    DropDownList cboControlArea = gi.FindControl("cboControlArea") as DropDownList;
                    DropDownList cboTrafficArea = gi.FindControl("cboTrafficArea") as DropDownList;

                    int currentInstructionId = -1;

                    if (hidInstructionId != null)
                        currentInstructionId = int.Parse(hidInstructionId.Value);

                    //controlAreaId = int.Parse(cboControlArea.SelectedValue);
                    //trafficAreaId = int.Parse(cboTrafficArea.SelectedValue);
                    bool testCA = int.TryParse(cboControlArea.SelectedValue, out controlAreaId);
                    bool testTA = int.TryParse(cboTrafficArea.SelectedValue, out trafficAreaId);

                    if (!testCA || !testTA)
                    {
                        throw new ApplicationException("Unable to determine CA and/or TA.");
                    }
                        
                    if (chkSelectLeg != null && chkSelectLeg.Checked)
                    {
                        instrucionIDs.Add(currentInstructionId);
                        instructionCATA.Add(currentInstructionId, new KeyValuePair<int, int>(controlAreaId, trafficAreaId));
                    }

                    if (chkCommunicateLeg != null && chkCommunicateLeg.Checked)
                    {
                        commnuicatedInstructions.Add(currentInstructionId);
                        giveResourceInstructions.Add(currentInstructionId);
                     
                        // If the instruction, control area and traffic area don't already exist, add them in.
                        if(!instructionCATA.Any(cr => cr.Key == currentInstructionId))
                            instructionCATA.Add(currentInstructionId, new KeyValuePair<int, int>(controlAreaId, trafficAreaId));
                    }
                }

                if (instrucionIDs.Count > 0)
                {
                    #region Resourcing

                    Facade.IDriver facDriver = new Facade.Resource();
                    Facade.IVehicle facVehicle = new Facade.Resource();
                    Facade.ITrailer facTrailer = new Facade.Resource();

                    Entities.Driver selectedDriver = null;
                    Entities.Vehicle selectedVehicle = null;
                    Entities.Trailer selectedTrailer = null;

                    #region Driver Resource
                    switch (driverResourceId)
                    {
                        case -1: // Unassign.
                            selectedDriver = new Orchestrator.Entities.Driver();
                            break;
                        case 0: // Leave Unchanged.
                            selectedDriver = null;
                            break;
                        default: // Update with selected driver.
                            selectedDriver = facDriver.GetDriverForResourceId(driverResourceId);
                            break;
                    }
                    #endregion

                    #region Vehicle Resource
                    switch (vehicleResourceId)
                    {
                        case -1: // Unassign.
                            selectedVehicle = new Orchestrator.Entities.Vehicle();
                            break;
                        case 0: // Leave Unchanged.
                            selectedVehicle = null;
                            break;
                        default: // Update with selected driver.
                            selectedVehicle = facVehicle.GetForVehicleId(vehicleResourceId);
                            break;
                    }
                    #endregion

                    #region Trailer Resource
                    switch (trailerResourceId)
                    {
                        case -2: // Create or reuse the trailer specified.
                            selectedTrailer = UseThirdPartyTrailer();
                            break;
                        case -1: // Unassign.
                            selectedTrailer = new Orchestrator.Entities.Trailer();
                            break;
                        case 0: // Leave Unchanged.
                            selectedTrailer = null;
                            break;
                        default: // Update with selected driver.
                            selectedTrailer = facTrailer.GetForTrailerId(trailerResourceId);
                            break;
                    }
                    #endregion

                    if (selectedDriver != null || selectedVehicle != null || selectedTrailer != null || giveResourceInstructions.Count > 0)
                    {
                        _legPlan.AlterLegResource(instrucionIDs, giveResourceInstructions, selectedDriver, selectedVehicle, selectedTrailer, instructionCATA);
                        PendingUpdates = true;
                    }

                    #endregion
                }

                if (commnuicatedInstructions.Count > 0 || _legPlan.FindAllLegs(fal => fal.CommunicateLeg).Count > 0)
                {
                    _legPlan.CommunicateLegs(commnuicatedInstructions);
                    PendingUpdates = true;
                }

                UpdateLegPlanCache();
                BindLegs();
                ClearComboBoxes();
            }
        }

        void btnUncommunicate_Click(object sender, EventArgs e)
        {
            List<int> instrucionIDs = new List<int>();

            foreach (GridItem gi in grdRun.Items)
            {
                CheckBox chkSelectLeg = gi.FindControl("chkSelectLeg") as CheckBox;
                HtmlInputHidden hidInstructionId = gi.FindControl("hidInstructionId") as HtmlInputHidden;

                if (chkSelectLeg != null && chkSelectLeg.Checked)
                {
                    if (hidInstructionId != null)
                        instrucionIDs.Add(int.Parse(hidInstructionId.Value));
                }
            }

            if (instrucionIDs.Count > 0)
            {
                Facade.IDriverCommunication facDriverCommunication = new Facade.Resource();
                int jobID = _job.JobId;

                facDriverCommunication.RemoveDriverCommunication(jobID, instrucionIDs);
                _job = new Facade.Job().GetJob(JobId, true, true);

                ResetCaches();
                BindLegs();
                ClearComboBoxes();
                UpdateOnClose = true;
            }
        }

        private void btnUpdateAndConfirm_Click(object sender, EventArgs e)
        {
            try
            {
                btnUpdateRows_Click(sender, e);
            }
            catch (Exception)
            {
                return;
            }
            
            btnConfirm_Click(sender, e);
        }

        #endregion

        #region ComboBox Items

        private void PreLoadSelectedResource()
        {
            // on the Traffic sheet they can select the resource from the My Resource section (clicking on the radio button)
            // we need to make sure we pull in these resources and prefill the d v t combos.

            Driver = Request.QueryString["Driver"];
            int.TryParse(Request.QueryString["DR"], out DriverResourceId);

            RegNo = Request.QueryString["RegNo"];
            int.TryParse(Request.QueryString["VR"], out VehicleResourceId);

            TrailerRef = Request.QueryString["TrailerRef"];
            if (!string.IsNullOrEmpty(Request.QueryString["TR"]))
                int.TryParse(Request.QueryString["TR"], out TrailerResourceId);


            //if (this.DriverResourceId > 0)
            //{
            //    // pre select the driver
            //    cboDriver.SelectedValue = this.DriverResourceId.ToString();
                
            //    cboDriver.Text = this.Driver;
            //}
            //if (this.VehicleResourceId > 0)
            //{
            //    // pre select the vehicle
            //    cboVehicle.SelectedValue = this.VehicleResourceId.ToString();
            //    cboVehicle.Text = this.RegNo;
            //}
            //if (this.TrailerResourceId > 0)
            //{
            //    //pre selec the Trailer
            //    cboTrailer.SelectedValue = this.TrailerResourceId.ToString();
            //    cboTrailer.Text = this.TrailerRef;
            //}
        }

        void cboDriver_ItemsRequested(object sender, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            cboDriver.Items.Clear();

            Facade.IResource facResource = new Facade.Resource();
            DataSet ds = null;

            #region Get ControlArea and Traffic Area
            // As thgis is only set when there is no call back or post back we need to get the correct values

            string[] clientArgs = e.Context["FilterString"].ToString().Split(':');
            int controlAreaId = 0;
            controlAreaId = int.Parse(clientArgs[0]);

            string[] tas = clientArgs[1].Split(',');
            int[] trafficAreas = new int[tas.Length];

            for (int i = 0; i < tas.Length; i++)
                trafficAreas[i] = int.Parse(tas[i]);

            int.TryParse(clientArgs[2], out depotId);
            #endregion

            if (depotId > 0)
                ds = facResource.GetAllResourcesFiltered(e.Text, eResourceType.Driver, depotId, false);
            else
                ds = facResource.GetAllResourcesFiltered(e.Text, eResourceType.Driver, controlAreaId, trafficAreas, false);

            int endOffset = 0;
            DataTable boundResults = BindComboBoxItems(ds.Tables[0], e.NumberOfItems, out endOffset);

            cboDriver.DataSource = boundResults;
            cboDriver.DataBind();

            if (boundResults.Rows.Count > 0)
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), ds.Tables[0].Rows.Count);
        }

        void cboVehicle_ItemsRequested(object sender, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            cboVehicle.Items.Clear();
            Telerik.Web.UI.RadComboBoxItem rcItem = new Telerik.Web.UI.RadComboBoxItem();
            DataSet ds = null;
            string[] clientArgs = e.Context["FilterString"].ToString().Split(':');
            if (e.Context["FilterString"] != null)
            {
                if (clientArgs[0] == "true")
                {
                    int driverID = 0;
                    int.TryParse(clientArgs[1], out driverID);

                    // Get the Drivers usual vehicle
                    Facade.IDriver facDriver = new Facade.Resource();
                    Entities.Driver driver = facDriver.GetDriverForResourceId(driverID);
                    Entities.Vehicle vehicle = null;

                    if (driver != null)
                        vehicle = ((Facade.IVehicle)facDriver).GetForVehicleId(driver.AssignedVehicleId);

                    if (vehicle != null)
                    {
                        rcItem.Text = vehicle.RegNo;
                        rcItem.Value = vehicle.ResourceId.ToString();
                        rcItem.Selected = true;
                        ((Telerik.Web.UI.RadComboBox)sender).Items.Add(rcItem);
                        ((Telerik.Web.UI.RadComboBox)sender).Text = vehicle.RegNo;
                    }
                }
                else
                {
                    #region Get ControlArea and Traffic Area
                    // As thgis is only set when there is no call back or post back we need to get the correct values

                    int controlAreaId = 0;
                    controlAreaId = int.Parse(clientArgs[0]);

                    string[] tas = clientArgs[1].Split(',');
                    int[] trafficAreas = new int[tas.Length];

                    for (int i = 0; i < tas.Length; i++)
                        trafficAreas[i] = int.Parse(tas[i]);

                    int.TryParse(clientArgs[2], out depotId);
                    #endregion

                    Facade.IResource facResource = new Facade.Resource();
                    if (depotId > 0)
                        ds = facResource.GetAllResourcesFiltered(e.Text, eResourceType.Vehicle, depotId, false);
                    else
                        ds = facResource.GetAllResourcesFiltered(e.Text, eResourceType.Vehicle, controlAreaId, trafficAreas, false);
                }
            }
            else
            {
                Facade.IResource facResource = new Facade.Resource();
                ds = facResource.GetAllResourcesFiltered(e.Text, eResourceType.Vehicle, false);
            }

            if (ds != null)
            {
                int endOffset = 0;
                DataTable boundResults = BindComboBoxItems(ds.Tables[0], e.NumberOfItems, out endOffset);

                cboVehicle.DataSource = boundResults;
                cboVehicle.DataBind();

                if (boundResults.Rows.Count > 0)
                    e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), ds.Tables[0].Rows.Count);
            }
        }

        void cboTrailer_ItemsRequested(object sender, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            cboTrailer.Items.Clear();

            #region Get ControlArea and Traffic Area
            // As thgis is only set when there is no call back or post back we need to get the correct values

            string[] clientArgs = e.Context["FilterString"].ToString().Split(':');
            int controlAreaId = 0;
            controlAreaId = int.Parse(clientArgs[0]);

            string[] tas = clientArgs[1].Split(',');
            int[] trafficAreas = new int[tas.Length];

            for (int i = 0; i < tas.Length; i++)
                trafficAreas[i] = int.Parse(tas[i]);

            int.TryParse(clientArgs[2], out depotId);
            #endregion

            Facade.IResource facResource = new Facade.Resource();
            int endOffset = 0;
            DataSet ds = null;
            // Amended to pick up the home depot if this has been selected for filering.
            if (depotId > 0)
                ds = facResource.GetAllResourcesFiltered(e.Text, eResourceType.Trailer, depotId, false);
            else
                ds = facResource.GetAllResourcesFiltered(e.Text, eResourceType.Trailer, controlAreaId, trafficAreas, false);

            if (ds.Tables[0].Rows.Count == 0)
            {
                // Check if the third party trailer already exists, if so pre-populate the organisation.
                ds = facResource.GetAllResourcesFiltered(e.Text, eResourceType.Trailer, false, true);
            }

            DataTable boundResults = BindComboBoxItems(ds.Tables[0], e.NumberOfItems, out endOffset);

            cboTrailer.DataSource = boundResults;
            cboTrailer.DataBind();

            if (boundResults.Rows.Count > 0)
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), ds.Tables[0].Rows.Count);
        }

        void cboThirdPartyTrailerOwner_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            cboThirdPartyTrailerOwner.Items.Clear();
            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllOrganisationsFiltered(e.Text, false);
            DataTable dt = ds.Tables[0];
            DataTable boundResults = dt.Clone();

            int itemOffset = e.NumberOfItems;
            int endOffset = itemOffset + itemsPerRequest;
            if (endOffset > dt.Rows.Count)
                endOffset = dt.Rows.Count;

            for (int i = itemOffset; i < endOffset; i++)
                boundResults.ImportRow(dt.Rows[i]);

            cboThirdPartyTrailerOwner.DataSource = boundResults;
            cboThirdPartyTrailerOwner.DataBind();

            if (dt.Rows.Count > 0)
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
        }

        //protected void cboTrailer_SelectedIndexChanged(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        //{
        //    int resourceID = 0;
        //    int.TryParse(e.Value, out resourceID);

        //    GetTrailerContent(resourceID);
        //}

        #endregion

        #region Grid

        private bool _areasDataBound = false;
        void grdRun_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.ItemType == GridItemType.Item || e.Item.ItemType == GridItemType.AlternatingItem)
            {
                Entities.LegView lv = e.Item.DataItem as Entities.LegView;

                CheckBox chkSelectLeg = (CheckBox)e.Item.FindControl("chkSelectLeg");
                CheckBox chkCommunicateLeg = e.Item.FindControl("chkCommunicateLeg") as CheckBox;


                bool subbedOutLeg = false;

                // When selecting a subbed out leg, the driver and vehicle cannot be assigned, thus the driver and vehicle combos
                // should be enabled/disabled appropriately.
                if (lv.SubContractorForDisplay != null)
                {
                    // Ensure that selecting/unselecting this leg disables/enables the driver and vehicle combo as appropriate
                    chkSelectLeg.Attributes.Add("onclick", "javascript:subbedLegCheckedChange(this);");

                    // Identify that this is a subby leg.
                    subbedOutLeg = true;
                }

                if (lv.State == Entities.LegView.eLegState.Completed)
                    chkCommunicateLeg.Enabled = chkSelectLeg.Enabled = false;
                else
                {
                    chkCommunicateLeg.Enabled = chkSelectLeg.Enabled = true;

                    // This setting was added for nicholls as they didn't want all the legs selecting by default.
                    // We should default to true and only set to false if there is a false in the config.
                    if (firstUseInstructionId > 0)
                        if (Globals.Configuration.AllResourceThisLegsSelected)
                        {
                            // this is "normal" behaviour, before the config entry was added.
                            if (jobType == eJobType.Groupage || lv.InstructionID == InstructionId)
                                chkSelectLeg.Checked = true;
                        }
                        else
                        {
                            // all legs should not be selected by default so just select the leg for
                            // the instruction that was used to launch this page.
                            if (lv.InstructionID == InstructionId)
                                chkSelectLeg.Checked = true;
                        }

                    // Preserve the pending change.
                    chkCommunicateLeg.Checked = lv.CommunicateLeg;

                    // When loading the led view, if the driver resource has not been updated, show the current communicated status.
                    if ((lv.Driver != null && lv.Instruction.DriverCommunication != null && !lv.DriverIsDirty) || (lv.DriverIsDirty && lv.CommunicateLeg))
                        chkCommunicateLeg.Checked = true;

                    if (lv.CommunicateLeg)
                        e.Item.Cells[e.Item.OwnerTableView.GetColumn("Comm").OrderIndex].Style.Add("background-color", "#FFA");
                }

                //////////////////////////////////////////////////////////////////////////////////
                // By this point, unless the leg is completed it should be checked and enabled. //
                //////////////////////////////////////////////////////////////////////////////////

                if (_jobSubContractors != null && _jobSubContractors.Count > 0)
                // If jobsubContractors is null the if is short circuited thus the second test should not cause an error.
                {
                    // We have subby's in our collection, now check to see if any sub contracted legs are marked as subby using their own trailer.
                    // If we find any then we should uncheck the leg and disable the check box.

                    if (lv.SubContractorForDisplay != null)
                    {
                        // This leg uses a subby, so find it in our collection
                        Entities.JobSubContractor subby = _jobSubContractors.Find(
                            jsc => jsc.ContractorIdentityId == lv.SubContractorForDisplay.IdentityId);

                        // N.B: Do not check if the subby is null - it shouldn't be. If it is then let an exception propagate.
                        if (subby.UseSubContractorTrailer)
                            chkCommunicateLeg.Enabled = chkSelectLeg.Checked = chkSelectLeg.Enabled = false; // Set this legs checkbox to unchecked and disable it.
                    }
                }

                ////////////////////////////////////////////////////////////////////////////////////////////
                // By this point, unless the leg is completed or subbed out with subby using own trailer, //
                // it should be checked and enabled.                                                      //
                ////////////////////////////////////////////////////////////////////////////////////////////

                // If the selected leg is not subbed out then all legs that are not subbed out should be pre-selected.
                if (!_selectedLegSubbedOutWholeJob && !_selectedLegSubbedOutPerOrder && !_selectedLegSubbedOutPerLeg)
                {
                    // The selected leg is not subbed out, thus all legs that are not subbed out should be pre-selected
                    // for resourcing.
                    if (lv.SubContractorForDisplay != null)
                        chkSelectLeg.Checked = false; // There is a subby, so do not pre-select this leg.
                }
                else
                {
                    // The selected leg is subbed out in some 
                    // If the selected leg is subbed out per leg, only the selected leg should be pre-selected.
                    if (_selectedLegSubbedOutPerLeg && lv.Instruction.InstructionID != _instruction.InstructionID)
                        chkSelectLeg.Checked = false;

                    // If the selected leg is subbed out per order, all legs for that subby should be pre-selected
                    // thus uncheck any legs that aren't for the same subby
                    if (_selectedLegSubbedOutPerOrder && _jobSubContractors.Count != 1)
                    {
                        if (lv.SubContractorForDisplay == null)
                        {
                            // The instruction we're currently binding is not subbed out
                            chkSelectLeg.Checked = false;
                        }
                        else
                        {
                            Entities.JobSubContractor selectedInstructionSubContractor = null;
                            Entities.JobSubContractor currentInstructionSubContractor = null;

                            // Get the subby identityid for the current instruction (ie. the one we're currently binding.
                            if (_instruction.CollectDrops[0].Order.JobSubContractID > 0)
                                selectedInstructionSubContractor = _jobSubContractors.FirstOrDefault(jsc => jsc.JobSubContractID == _instruction.CollectDrops[0].Order.JobSubContractID);

                            // Get the subby identityid for the current instruction (ie. the one we're currently binding.
                            if (lv.Instruction.CollectDrops[0].Order.JobSubContractID > 0)
                                currentInstructionSubContractor = _jobSubContractors.FirstOrDefault(jsc => jsc.JobSubContractID == lv.Instruction.CollectDrops[0].Order.JobSubContractID);

                            if (currentInstructionSubContractor != null && (currentInstructionSubContractor.ContractorIdentityId != selectedInstructionSubContractor.ContractorIdentityId))
                                chkSelectLeg.Checked = false;
                        }
                    }
                }

                // Set the driver, vehicle and trailer fields on the repeater table
                // Driver
                string driverFullName = "";
                Label lblDriver = (Label)e.Item.FindControl("lblDriver");
                Label lblVehicle = (Label)e.Item.FindControl("lblVehicle");
                Label lblTrailer = (Label)e.Item.FindControl("lblTrailer");

                if (lv.SubContractorForDisplay != null && lv.Driver == null)
                {
                    // The driver is a subby. Pass the Organisation IdentityId.
                    driverFullName = lblDriver.Text = lv.SubContractorForDisplay.OrganisationDisplayName;
                    lblDriver.Attributes.Add("HasContent", "true");
                }
                else if (lv.SubContractorForDisplay == null && lv.Driver != null)
                {
                    // The driver is an inhouse resource. Pass Individual IdentityId.
                    // The driver is a subby. Pass the Organisation IdentityId.
                    driverFullName = lblDriver.Text = lv.Driver.Individual.FullName;
                    lblDriver.Attributes.Add("HasContent", "true");
                }
                else
                    lblDriver.Text = "&nbsp;";

                lblVehicle.Text = lv.Vehicle == null ? "&nbsp;" : lv.Vehicle.RegNo;         // Vehicle
                lblTrailer.Text = lv.Trailer == null ? "&nbsp;" : lv.Trailer.TrailerRef;    // Trailer

                if (lv.DriverIsDirty)
                {
                    e.Item.Cells[e.Item.OwnerTableView.GetColumn("Driver").OrderIndex].Style.Add("background-color", "#FFA");
                    PendingUpdates = true;
                }

                if (lv.VehicleIsDirty)
                {
                    e.Item.Cells[e.Item.OwnerTableView.GetColumn("Vehicle").OrderIndex].Style.Add("background-color", "#FFA");
                    PendingUpdates = true;
                }

                if (lv.TrailerIsDirty)
                {
                    e.Item.Cells[e.Item.OwnerTableView.GetColumn("Trailer").OrderIndex].Style.Add("background-color", "#FFA");
                    PendingUpdates = true;
                }

                //////////////////////////////////////////////////////////////////////////////////
                // To ensure that driver and vehicle combos are disabled if subbed out legs are pre-selected
                // we will keep a count and register a clientside script block to disable the combos as appropriate.
                if (subbedOutLeg && chkSelectLeg.Checked == true)
                {
                    _subbedLegCheckedCount++;
                }

                Label lblControlArea = e.Item.FindControl("lblControlArea") as Label;
                Label lblTrafficArea = e.Item.FindControl("lblTrafficArea") as Label;
                DropDownList cboControlArea = e.Item.FindControl("cboControlArea") as DropDownList;
                DropDownList cboTrafficArea = e.Item.FindControl("cboTrafficArea") as DropDownList;
                CascadingDropDown ccdControlArea = e.Item.FindControl("CascadingDropDown2") as CascadingDropDown;
                CascadingDropDown ccdTrafficArea = e.Item.FindControl("CascadingDropDown1") as CascadingDropDown;

                if (lv.GiveToControlArea != -1)
                {
                    var controlArea = from dr in controlAreas.Tables[0].Rows.Cast<DataRow>().AsEnumerable()
                                      where dr.Field<int>("ControlAreaId") == lv.GiveToControlArea
                                      select dr;


                    if (controlArea != null && controlArea.Count() > 0)
                    {
                        HtmlGenericControl spnCA = e.Item.FindControl("spnCA") as HtmlGenericControl;
                        e.Item.Cells[e.Item.OwnerTableView.GetColumn("CA").OrderIndex].Style.Add("background-color", "#FFA");
                        //lblControlArea.Text = controlArea.SingleOrDefault().Field<string>("Description");
                        ccdControlArea.SelectedValue = controlArea.SingleOrDefault().Field<Int32>("ControlAreaId").ToString();
                    }
                }
                else if (lv.ControlArea != null)
                {
                    ccdControlArea.SelectedValue = lv.EndLegPoint.Instruction.Point.Address.TrafficArea.ControlAreaId.ToString();
                }
                else
                    lblControlArea.Text = "&nbsp;";

                if (lv.GiveToControlArea != -1)
                {
                    if (trafficAreas == null || trafficAreas.Tables[0].Rows.Count == 0)
                        BindTrafficAreas(lv.GiveToControlArea, lv.GiveToTrafficArea);

                    var trafficArea = from dr in trafficAreas.Tables[0].Rows.Cast<DataRow>().AsEnumerable()
                                      where dr.Field<int>("TrafficAreaId") == lv.GiveToTrafficArea
                                      select dr;

                    if (trafficArea != null && trafficArea.Count() > 0)
                    {
                        HtmlGenericControl spnTA = e.Item.FindControl("spnTA") as HtmlGenericControl;
                        e.Item.Cells[e.Item.OwnerTableView.GetColumn("TA").OrderIndex].Style.Add("background-color", "#FFA");
                        ccdTrafficArea.ContextKey = trafficArea.SingleOrDefault().Field<Int32>("TrafficAreaId").ToString();
                        //lblTrafficArea.Text = trafficArea.SingleOrDefault().Field<string>("Description");
                    }
                }
                else if (lv.TrafficArea != null)
                {
                    ccdTrafficArea.ContextKey = lv.EndLegPoint.Instruction.Point.Address.TrafficArea.TrafficAreaId.ToString();

                }
                else
                {
                    ccdTrafficArea.ContextKey = "0";
                    lblTrafficArea.Text = "&nbsp;";
                }
            }
        }

        #endregion

        #endregion
    }
}
