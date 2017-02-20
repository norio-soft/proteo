using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Orchestrator.Globals;
using System.Linq;
using Orchestrator.Exceptions;
using Orchestrator.Repositories;
using Orchestrator.Models;
using Orchestrator.WebUI.Code.components;
using Telerik.Web.UI;


namespace Orchestrator.WebUI.resource.vehicle
{
    /// <summary>
    /// Summary description for addupdatevehicle.
    /// </summary>
    public partial class addupdatevehicle : Orchestrator.Base.BasePage
    {
        #region Constants

        private const string C_RESOURCE_ID = "ResourceId";

        #endregion

        #region Page Variables

        protected bool m_isUpdate = false;
        public int m_resourceId = 0;
        private long m_long = System.Int64.MaxValue;
        private Entities.Vehicle vehicle;
        protected string vehicleRegistrationNo = String.Empty;
        //private	eResourceType						m_resourceType;
        //private	DateTime							m_startDate;

        protected int m_organisationId = 0;
        protected string m_startTown = String.Empty;
        protected int m_startTownId = 0;
        protected int m_pointId = 0;

        private bool? _enableVehicleNominalCodes = null;
        private List<TelematicsSolution> m_telematicsSolutions;
        private List<TreeDataNode> m_orgUnitTreeNodes;

        public bool EnableVehicleNominalCodes
        {
            get
            {
                if (!_enableVehicleNominalCodes.HasValue)
                {
                    _enableVehicleNominalCodes = bool.Parse(System.Configuration.ConfigurationManager.AppSettings["EnableVehicleNominalCodes"].ToString());
                }

                return (bool)_enableVehicleNominalCodes;
            }
        }

        #endregion

        #region Form Elements


        protected System.Web.UI.WebControls.RequiredFieldValidator rfvHomePoint;


        #endregion

        #region Page Load/Init/Error

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage, eSystemPortion.AddEditResource);
            btnAdd.Enabled = Orchestrator.WebUI.Security.Authorise.CanAccess(eSystemPortion.AddEditResource);
            ProfitabilityReportExclusionsRow.Visible = false; ;

            if (!EnableVehicleNominalCodes)
                fsFinancialDetails.Visible = false;

            if (Request.QueryString["rcbID"] == null)
            {
                // Attempt from QueryString
                m_resourceId = GetResourceId();

                //Get the OrgUnits
                m_orgUnitTreeNodes = (from ou in EF.DataContext.Current.OrgUnits
                              orderby ou.Name
                              select new TreeDataNode
                              {
                                  OrgUnitId = ou.OrgUnitId,
                                  ParentOrgUnitId = ou.ParentOrgUnitId,
                                  Text = ou.Name
                              }).ToList();

                if (m_resourceId > 0)
                {
                    m_isUpdate = true;
                }

                if (!IsPostBack)
                {
                    PopulateStaticControls();
                }

                btnAddVehicleKey.Enabled = m_isUpdate;
                if (m_isUpdate)
                {
                    if (!IsPostBack)
                        LoadVehicle();

                    // Enable the add vehicle key button only when updating
                    string confirmMessage = @"javascript:return(confirm('Any changes you have made will need to be saved before you continue.\nTo save your changes, click Cancel, the click the " + (m_isUpdate ? "Update" : "Add") + " button, you may then try again.'))";
                    btnAddVehicleKey.Attributes.Add("onClick", confirmMessage);
                }

                if (!IsPostBack) BindResourceGroupings();

                if (Orchestrator.Globals.Configuration.ProfitabilityReportDisplayVehicleExclusionsLink)
                    ProfitabilityReportExclusionsRow.Visible = true;
            }

            this.thirdPartyIntegrationIDRangeValidator.MaximumValue = Int64.MaxValue.ToString();


        }

        private void addupdatevehicle_Init(object sender, EventArgs e)
        {
            ((WizardMasterPage)this.Master).WizardTitle = "Vehicle Details";
            this.cboManufacturer.SelectedIndexChanged += new System.EventHandler(this.cboManufacturer_SelectedIndexChanged);
            this.btnAddVehicleKey.Click += new System.EventHandler(this.btnAddVehicleKey_Click);
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            this.cboPoint.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboPoint_ItemsRequested);
            this.cboOrganisation.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboOrganisation_ItemsRequested);
            this.btnCancel.Click += new EventHandler(btnCancel_Click);

        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            mwhelper.CausePostBack = false;
            mwhelper.CloseForm = true;
        }

        #endregion

        #region DBCombo's Server Methods and Initialisation

        void cboOrganisation_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {

            cboOrganisation.Items.Clear();

            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllOrganisationsFiltered(e.Text);

            int itemsPerRequest = 20;
            int itemOffset = e.NumberOfItems;
            int endOffset = itemOffset + itemsPerRequest;
            if (endOffset > ds.Tables[0].Rows.Count)
                endOffset = ds.Tables[0].Rows.Count;

            DataTable dt = ds.Tables[0];
            Telerik.Web.UI.RadComboBoxItem rcItem = null;
            for (int i = itemOffset; i < endOffset; i++)
            {
                rcItem = new Telerik.Web.UI.RadComboBoxItem();
                rcItem.Text = dt.Rows[i]["OrganisationName"].ToString();
                rcItem.Value = dt.Rows[i]["IdentityId"].ToString();
                cboOrganisation.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }
        }

        void cboPoint_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            cboPoint.Items.Clear();
            int identityId = 0;
            string searchText = "";
            if (e.Context["FilterString"] != null && e.Context["FilterString"].ToString() != "")
            {
                string[] values = e.Context["FilterString"].ToString().Split(';');
                try { identityId = int.Parse(values[0]); }
                catch { }
                if (values.Length > 1 && values[1] != "false" && !string.IsNullOrEmpty(values[1]))
                {
                    searchText = values[1];
                }
                else if (!string.IsNullOrEmpty(e.Text))
                    searchText = e.Text;
            }
            else
                searchText = e.Context["FilterString"].ToString();

            Orchestrator.Facade.IPoint facPoint = new Orchestrator.Facade.Point();
            DataSet ds = facPoint.GetAllForOrganisation(identityId, ePointType.Any, 0, searchText);
            System.Diagnostics.Debug.Write(e.Value + " " + e.Text);

            int itemsPerRequest = 20;
            int itemOffset = e.NumberOfItems;
            int endOffset = itemOffset + itemsPerRequest;
            if (endOffset > ds.Tables[0].Rows.Count)
                endOffset = ds.Tables[0].Rows.Count;

            DataTable dt = ds.Tables[0];
            Telerik.Web.UI.RadComboBoxItem rcItem = null;
            for (int i = itemOffset; i < endOffset; i++)
            {
                rcItem = new Telerik.Web.UI.RadComboBoxItem();
                rcItem.Text = dt.Rows[i]["Description"].ToString();
                rcItem.Value = dt.Rows[i]["PointId"].ToString();
                cboPoint.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }
        }
        #endregion

        #region Populate and Display Controls/Elements
        ///	<summary> 
        /// Populate Static Controls 
        ///	</summary>
        private void PopulateStaticControls()
        {
            if(Orchestrator.Globals.Configuration.FleetMetrikInstance)
                OnlyEnableFleetMetrikFields();

            if (!Orchestrator.Globals.Configuration.ShowVehicleDepot)
            {
                vehicleDepot.Style.Add("display", "none");
                cboTrafficArea.Visible = false;
                rfvDepot.Enabled = false;
            }
                

            // Load the Classes Dropdown
            Facade.IVehicle facResource = new Facade.Resource();
            DataSet dsVehicleClasses = facResource.GetAllVehicleClasses();
            cboClass.DataSource = dsVehicleClasses;
            cboClass.DataTextField = "Description";
            cboClass.DataValueField = "VehicleClassId";
            cboClass.DataBind();
            cboClass.Items.Insert(0, new ListItem("--- [ Please Select ] ---", ""));

            // load the vehicle Types
            cboVehicleType.DataSource = facResource.GetAllVehicleTypes();
            cboVehicleType.Items.Insert(0, new ListItem("--- [ Please Select ] ---", ""));
            cboVehicleType.DataBind();

            // Can this system support fixed units?
            chkIsFixedUnit.Enabled = Configuration.InstallationSupportsFixedUnits;

            // Load the Manufacturers Dropdown
            Facade.IVehicle facVehicleMan = new Facade.Resource();
            DataSet dsVehicleManufacturers = facVehicleMan.GetAllVehicleManufacturers();
            cboManufacturer.DataSource = dsVehicleManufacturers;
            cboManufacturer.DataTextField = "Description";
            cboManufacturer.DataValueField = "VehicleManufacturerId";
            cboManufacturer.DataBind();
            cboManufacturer.Items.Insert(0, new ListItem("--- [ Please Select ] ---", ""));

            cboVehicleType.DataSource = facResource.GetAllVehicleTypes();
            cboVehicleType.DataBind();
            cboVehicleType.Items.Insert(0, new ListItem("--- [ Please Select ] ---", "0"));

            // Load the Model Dropdown with relevant fields
            cboManufacturer_SelectedIndexChanged(cboManufacturer, EventArgs.Empty);

            Facade.IOrganisationLocation facOrganiastionLocation = new Facade.Organisation();
            cboDepot.DataSource = facOrganiastionLocation.GetAllDepots(Configuration.IdentityId);
            cboDepot.DataValueField = "OrganisationLocationId";
            cboDepot.DataTextField = "OrganisationLocationName";
            cboDepot.DataBind();
            cboDepot.Items.Insert(0, new ListItem("--- [ Please Select ] ---", ""));

            Facade.IControlArea facControlArea = new Facade.Traffic();
            cboControlArea.DataSource = facControlArea.GetAll();
            cboControlArea.DataTextField = "Description";
            cboControlArea.DataValueField = "ControlAreaId";
            cboControlArea.DataBind();
            cboControlArea.Items.Insert(0, new ListItem("--- [ Please Select ] ---", ""));

            Facade.ITrafficArea facTrafficArea = (Facade.ITrafficArea)facControlArea;
            cboTrafficArea.DataSource = facTrafficArea.GetAll();
            cboTrafficArea.DataTextField = "Description";
            cboTrafficArea.DataValueField = "TrafficAreaId";
            cboTrafficArea.DataBind();
            cboTrafficArea.Items.Insert(0, new ListItem("--- [ Please Select ] ---", ""));

            // Get the nominal codes
            Facade.INominalCode facNominalCode = new Orchestrator.Facade.NominalCode();
            DataSet dsNominalCodes = facNominalCode.GetAllActive();
            cboNominalCode.Items.Insert(0, new ListItem("--- [ Please Select ] ---", "0"));

            foreach (DataRow row in dsNominalCodes.Tables[0].Rows)
            {
                ListItem item = new ListItem();
                item.Value = row["NominalCodeId"].ToString();
                item.Text = row["NominalCode"].ToString() + " - " + row["Description"].ToString();
                cboNominalCode.Items.Add(item);
            }


            InitialiseTelematicsSolution();

        }

        private void OnlyEnableFleetMetrikFields()
        {
            fsFinancialDetails.Visible = false;
            fsKeyDetails.Visible = false;
            fsLastKnownLocation.Visible = false;
            fsBelongsTo.Visible = false;
            audit.Visible = false;
            trIsFixedUnit.Visible = false;
            trThirdPartyIntergration.Visible = false;
        }

        private void BindResourceGroupings()
        {

            RadTreeNodeBinding binding = new RadTreeNodeBinding();
            binding.CheckedField = "Checked";
            binding.Checkable = true;

            //Bind the data to the tree
            trvResourceGrouping.DataBindings.Add(binding);
            trvResourceGrouping.DataSource = m_orgUnitTreeNodes;
            trvResourceGrouping.DataBind();
            trvResourceGrouping.ExpandAllNodes();
        }

        private void trvResourceGrouping_NodeDataBound(object sender, RadTreeNodeEventArgs e)
        {
            TreeDataNode dataNode = (TreeDataNode)e.Node.DataItem;
            e.Node.Attributes["OrgUnitId"] = dataNode.OrgUnitId.ToString();
        }

        private void InitialiseTelematicsSolution()
        {

            m_telematicsSolutions = Configuration.TelematicsSolutions.Where(x => x.IsActive).ToList();

            if (m_telematicsSolutions.Any())
            {
                // Only one active telematics solution, use that as the default solution
                if (m_telematicsSolutions.Count == 1)
                {
                    ViewState["defaultTelematicsSolution"] = m_telematicsSolutions[0].TelematicsSolutionID;
                }
                else
                // Multiple active solutions, show the the user the telematics dropdown
                {
                    //make the drop down telematics solution dropdown visible
                    telematicsOption.Visible = true;

                    cboTelematicsSolution.DataSource = m_telematicsSolutions;
                    cboTelematicsSolution.DataBind();
                    cboTelematicsSolution.Items.Insert(0, new ListItem("--- [ Please Select ] ---", ""));

                }
            }
            else
                ViewState["defaultTelematicsSolution"] = null;

        }

        #endregion

        #region Add/Update/Populate/Load Vehicle

        ///	<summary> 
        /// Add Vehicle
        ///	</summary>
        private Entities.FacadeResult AddVehicle()
        {
            Facade.IVehicle facResource = new Facade.Resource();

            string userName = ((Entities.CustomPrincipal)Page.User).UserName;

            Entities.FacadeResult retval;

            try
            {
                retval = facResource.Create(vehicle, userName);
            }
            catch (BusinessFacadeException ex)
            {
                if (ex.InnerException is BusinessFacadeException && 
                    ex.InnerException.InnerException is BusinessLayerException &&
                    ex.InnerException.InnerException.InnerException is ThirdPartyTelematicsException)
                {

                    return new Entities.FacadeResult(false, "There was an error updating the vehicle on a third party telematics system. It may not be usable, please contact support.");
                }
                else
                    throw;
            }

            if (retval.Success)
            {
                // Store the new id in the viewstate
                m_resourceId = retval.ObjectId;
                vehicle.ResourceId = m_resourceId;
                ViewState[C_RESOURCE_ID] = m_resourceId;

                if (!string.IsNullOrEmpty(Configuration.BlueSphereCustomerId))
                {
                    try
                    {
                        var message = MobileWorkerFlow.MWFServicesCommunication.Client.AddNewVehicleToBlueSphere(vehicle, retval.ObjectId.ToString());

                        if (!string.IsNullOrEmpty(message))
                        {
                            ViewState["IsUpdate"] = m_isUpdate = true;
                            LoadVehicle();
                            return new Entities.FacadeResult(false, "There was an error setting up the Vehicle for MWF, " + message);
                        }
                    }
                    catch
                    {
                        // MM (2nd April 2012): Not ideal but solution for now, if fail to get through to bluesphere then show error to user
                        ViewState["IsUpdate"] = m_isUpdate = true;
                        LoadVehicle();
                        retval = new Entities.FacadeResult(false, "There was an error adding the vehicle and they will not be usable, please contact support.");
                    }
                }

            }
            else
            {
                return new Entities.FacadeResult(false);
            }

            return retval;
        }

        ///	<summary> 
        /// Update Vehicle
        ///	</summary>
        private Entities.FacadeResult UpdateVehicle()
        {
            Facade.IVehicle facResource = new Facade.Resource();
            string userName = ((Entities.CustomPrincipal)Page.User).UserName;

            if (chkDelete.Checked)
                vehicle.GPSUnitID = "";

            Entities.FacadeResult retVal;
            try
            {
                retVal = facResource.Update(vehicle, userName);
            }
            catch (BusinessFacadeException ex)
            {
                if (ex.InnerException is BusinessFacadeException && 
                    ex.InnerException.InnerException is BusinessLayerException &&
                    ex.InnerException.InnerException.InnerException is ThirdPartyTelematicsException)
                {

                    return new Entities.FacadeResult(false, "There was an error updating the vehicle on a third party telematics system. It may not be usable, please contact support.");
                }
                else
                    throw;
            }

            if (retVal.Success && !string.IsNullOrEmpty(Configuration.BlueSphereCustomerId))
            {
                try
                {
                    var message = MobileWorkerFlow.MWFServicesCommunication.Client.UpdateVehicleInBlueSphere(vehicle, vehicle.ResourceId.ToString());

                    if (!string.IsNullOrEmpty(message))
                    {

                        LoadVehicle();
                        return new Entities.FacadeResult(false, "There was an error updating the the vehicle for MWF, " + message);
                    }
                }
                catch
                {
                    // MM (2nd April 2012): Not ideal but solution for now, if fail to get through to bluesphere then show error to user
                    return new Entities.FacadeResult(false, "There was an error updating the vehicle and they will not be usable, please contact support.");
                }
            }
            return retVal;
        }

        ///	<summary> 
        /// Load Vehicle
        ///	</summary>
        private void LoadVehicle()
        {
            if (ViewState["vehicle"] == null)
            {
                Facade.IVehicle facVehicle = new Facade.Resource();
                vehicle = facVehicle.GetForVehicleId(m_resourceId);
                ViewState["vehicle"] = vehicle;
            }
            else
                vehicle = (Entities.Vehicle)ViewState["vehicle"];

            if (vehicle != null)
            {
                txtRegistrationNo.Text = vehicle.RegNo;
                vehicleRegistrationNo = vehicle.RegNo;
                txtChassisNo.Text = vehicle.ChassisNo;
                txtTelephoneNumber.Text = vehicle.CabPhoneNumber;
                cboClass.SelectedIndex = -1;

                dteMOTExpiry.SelectedDate = vehicle.MOTExpiry;
                dteServiceDate.SelectedDate = vehicle.VehicleServiceDueDate;

                cboManufacturer.ClearSelection();

                cboClass.Items.FindByValue(vehicle.VehicleClassId.ToString()).Selected = true;
                cboManufacturer.Items.FindByValue(vehicle.VehicleManufacturerId.ToString()).Selected = true;

                chkIsFixedUnit.Checked = vehicle.IsFixedUnit;
                pnlTrailerDetails.Visible = !vehicle.IsFixedUnit;

                if (pnlTrailerDetails.Visible && vehicle.TrailerResourceID.HasValue)
                    lblTrailer.Text = vehicle.TrailerResource;

                cboVehicleType.ClearSelection();
                if (vehicle.VehicleTypeID > 0)
                    cboVehicleType.Items.FindByValue(vehicle.VehicleTypeID.ToString()).Selected = true;

                if (telematicsOption.Visible)
                {
                    cboTelematicsSolution.ClearSelection();
                    if (vehicle.TelematicsSolution.HasValue)
                        cboTelematicsSolution.Items.FindByText(vehicle.TelematicsSolution.ToString()).Selected = true;
                }

                // Need to load model after manufacturer has loaded and then loaded 
                // model dropdown with the relevant results
                cboManufacturer_SelectedIndexChanged(cboManufacturer, EventArgs.Empty);
                cboModel.Items.FindByValue(vehicle.VehicleModelId.ToString()).Selected = true;

                // Need to load model after manufacturer has loaded and then loaded 
                // model dropdown with the relevant results
                cboManufacturer_SelectedIndexChanged(cboManufacturer, EventArgs.Empty);
                cboModel.Items.FindByValue(vehicle.VehicleModelId.ToString()).Selected = true;

                Facade.IPoint facPoint = new Facade.Point();
                Entities.Point point = facPoint.GetPointForPointId(vehicle.HomePointId);

                cboOrganisation.Text = point.OrganisationName;
                cboOrganisation.SelectedValue = point.IdentityId.ToString();
                m_organisationId = point.IdentityId;

                m_startTown = point.PostTown.TownName;
                m_startTownId = point.PostTown.TownId;

                cboPoint.Text = point.Description;
                cboPoint.SelectedValue = point.PointId.ToString();
                m_pointId = point.PointId;

                // Set the nominal code
                if (vehicle.NominalCodeId > 0)
                    cboNominalCode.Items.FindByValue(vehicle.NominalCodeId.ToString()).Selected = true;

                PopulateKeys();

                if (vehicle.ResourceStatus == eResourceStatus.Deleted)
                    chkDelete.Checked = true;

                Entities.ControlArea ca = null;
                Entities.TrafficArea ta = null;

                using (Facade.IResource facResource = new Facade.Resource())
                    facResource.GetControllerForResourceId(vehicle.ResourceId, ref ca, ref ta);

                cboDepot.ClearSelection();
                if (vehicle.DepotId > 0)
                    cboDepot.Items.FindByValue(vehicle.DepotId.ToString()).Selected = true;

                if (ca != null && ta != null)
                {
                    cboControlArea.ClearSelection();
                    cboControlArea.Items.FindByValue(ca.ControlAreaId.ToString()).Selected = true;
                    cboTrafficArea.ClearSelection();
                    cboTrafficArea.Items.FindByValue(ta.TrafficAreaId.ToString()).Selected = true;
                }

                cboDedicatedToClient.SelectedValue = vehicle.DedicatedToClientIdentityID.ToString();

                if (vehicle.DedicatedToClientIdentityID.HasValue)
                {
                    using (var uow = DIContainer.CreateUnitOfWork())
                    {
                        var repo = DIContainer.CreateRepository<IOrganisationRepository>(uow);
                        var client = repo.Find(vehicle.DedicatedToClientIdentityID.Value);
                        cboDedicatedToClient.Text = client.OrganisationName;
                    }
                }
                else
                    cboDedicatedToClient.Text = "- none -";

                chkDelete.Visible = true;
                pnlVehicleDeleted.Visible = true;

                txtGPSUnitID.Text = vehicle.GPSUnitID;
                txtGPSUnitID.Text = txtGPSUnitID.Text.ToUpper();

                txtThirdPartyIntegrationID.Text = (vehicle.ThirdPartyIntegrationID.HasValue) ? vehicle.ThirdPartyIntegrationID.ToString() : string.Empty;

                //Find all tree nodes that correspond to the org units
                var treeNodesToCheck = m_orgUnitTreeNodes.Where(x => vehicle.OrgUnitIDs.Contains(x.OrgUnitId.Value));
                foreach (var treeNode in treeNodesToCheck)
                {
                    treeNode.Checked = true;
                }

            }

            btnAdd.Text = "Update";
        }

        ///	<summary> 
        /// Populate Vehicle
        ///	</summary>
        private void populateVehicle()
        {
            if (ViewState["vehicle"] == null)
            {
                vehicle = new Entities.Vehicle();
                if (m_resourceId > 0)
                    vehicle.ResourceId = m_resourceId;
                if(Orchestrator.Globals.Configuration.FleetMetrikInstance)
                {
                    // set the owner organisation, home point and last known location
                    Facade.IOrganisation facOrganisation = new Facade.Organisation();
                    Entities.Organisation ownerOrg = facOrganisation.GetForIdentityId(Configuration.IdentityId);
                    vehicle.HomePointId = ownerOrg.Locations.GetHeadOffice().Point.PointId;
                    vehicle.DepotId = ownerOrg.Locations.GetHeadOffice().OrganisationLocationId;
                }
            }
            else
                vehicle = (Entities.Vehicle)ViewState["vehicle"];

            vehicle.ResourceType = eResourceType.Vehicle;
            vehicle.VehicleClassId = Convert.ToInt32(cboClass.Items[cboClass.SelectedIndex].Value);
            vehicle.VehicleManufacturerId = Convert.ToInt32(cboManufacturer.Items[cboManufacturer.SelectedIndex].Value);
            vehicle.VehicleModelId = Convert.ToInt32(cboModel.Items[cboModel.SelectedIndex].Value);
            vehicle.RegNo = txtRegistrationNo.Text;
            vehicle.ChassisNo = txtChassisNo.Text;
            vehicle.MOTExpiry = dteMOTExpiry.SelectedDate.Value;
            vehicle.VehicleServiceDueDate = dteServiceDate.SelectedDate.Value;
            vehicle.CabPhoneNumber = txtTelephoneNumber.Text;
            if(!Orchestrator.Globals.Configuration.FleetMetrikInstance)
            {
                if(Orchestrator.Globals.Configuration.ShowVehicleDepot)
                    vehicle.DepotId = Convert.ToInt32(cboDepot.SelectedValue);
                else
                {
                    Facade.IOrganisation facOrganisation = new Facade.Organisation();
                    Entities.Organisation ownerOrg = facOrganisation.GetForIdentityId(Configuration.IdentityId);
                    vehicle.DepotId = ownerOrg.Locations.GetHeadOffice().OrganisationLocationId;
                }

                vehicle.HomePointId = Convert.ToInt32(cboPoint.SelectedValue);
            }
                
            vehicle.IsFixedUnit = chkIsFixedUnit.Checked;
            vehicle.IsTTInstalled = true;
            vehicle.VehicleTypeID = Convert.ToInt32(cboVehicleType.Items[cboVehicleType.SelectedIndex].Value);
            vehicle.NominalCodeId = Convert.ToInt32(cboNominalCode.Items[cboNominalCode.SelectedIndex].Value);

            // if the telematics option is visible use the selected solution (validation enforces a non-null selection)
            if (telematicsOption.Visible)
            {
                vehicle.TelematicsSolution = (eTelematicsSolution?)Enum.Parse(typeof(eTelematicsSolution), cboTelematicsSolution.SelectedValue);
            }
            else
            {
                // if telematics option is not visible and we're creating, use the default
                if (!m_isUpdate && ViewState["defaultTelematicsSolution"] != null)
                    vehicle.TelematicsSolution = (eTelematicsSolution)ViewState["defaultTelematicsSolution"];
                //otherwise leave the telematics solution as it is

            }


            if (chkDelete.Checked)
                vehicle.ResourceStatus = eResourceStatus.Deleted;
            else
                vehicle.ResourceStatus = eResourceStatus.Active;

            
            vehicle.GPSUnitID = txtGPSUnitID.Text.ToUpper();
            vehicle.DedicatedToClientIdentityID = Utilities.ParseNullable<int>(cboDedicatedToClient.SelectedValue);
            vehicle.ThirdPartyIntegrationID = (string.IsNullOrEmpty(txtThirdPartyIntegrationID.Text)) ? (long?)null : (long?)long.Parse(txtThirdPartyIntegrationID.Text);

            vehicle.OrgUnitIDs = trvResourceGrouping.CheckedNodes.Select(x => Int32.Parse(x.Attributes["OrgUnitID"])).ToList();

        }

        private int GetResourceId()
        {
            int retVal = Convert.ToInt32(Request.QueryString["resourceId"]);
            if (retVal > 0)
            {
                // Store in ViewState
                ViewState[C_RESOURCE_ID] = retVal;
            }
            else
            {
                // Attempt from ViewState
                retVal = Convert.ToInt32(ViewState[C_RESOURCE_ID]);
            }

            return retVal;
        }

        private void cboManufacturer_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            // Load the Models Dropdown with reference to Manufacturers
            if (cboManufacturer.SelectedIndex != 0)
            {
                Facade.IVehicle facVehicleMod = new Facade.Resource();
                DataSet dsVehicleModels = facVehicleMod.GetVehicleModelForManufacturerId(Convert.ToInt32(cboManufacturer.SelectedValue));
                cboModel.DataSource = dsVehicleModels;
                cboModel.DataTextField = "Description";
                cboModel.DataValueField = "VehicleModelId";
                cboModel.DataBind();
                if (dsVehicleModels.Tables[0].Rows.Count != 0)
                {
                    cboModel.Visible = true;
                }
            }
            else
                cboModel.Visible = false;
        }





        ///	<summary> 
        /// Reset Fields
        ///	</summary>
        private void ResetFields()
        {
            txtChassisNo.Text = string.Empty;
            txtRegistrationNo.Text = string.Empty;
            txtTelephoneNumber.Text = string.Empty;
            cboClass.SelectedIndex = -1;
            cboManufacturer.ClearSelection();
            cboManufacturer.ClearSelection();
            cboOrganisation.Text = cboOrganisation.SelectedValue = String.Empty;
            cboPoint.Text = cboPoint.SelectedValue = String.Empty;
            cboVehicleType.ClearSelection();
            cboTelematicsSolution.ClearSelection();
            cboClass.Items.FindByValue("0").Selected = true;
            cboManufacturer.Items.FindByValue("0").Selected = true;
            cboModel.Items.FindByValue("0").Selected = true;
            cboNominalCode.Items.FindByValue("0").Selected = true;
            txtGPSUnitID.Text = string.Empty;
            txtThirdPartyIntegrationID.Text = string.Empty;

            m_organisationId = 0;
            m_startTown = String.Empty;
            m_startTownId = 0;
            m_pointId = 0;
        }

        ///	<summary> 
        /// Button Add
        ///	</summary>
        private void btnAdd_Click(object sender, System.EventArgs e)
        {
            if (Page.IsValid)
            {
                Entities.FacadeResult retVal;

                populateVehicle();

                if (m_isUpdate)
                    retVal = UpdateVehicle();
                else
                    retVal = AddVehicle();

                if (m_isUpdate)
                {
                    if (retVal.Success)
                    {
                        infringementDisplay.Visible = false;
                        lblConfirmation.Text = "The vehicle has been updated successfully.";
                        // refresh the page we came from if it is expecting a call back
                        this.ClientScript.RegisterStartupScript(this.GetType(), "CallBack", "window.opener.location.reload()", true);

                        if (cboControlArea.SelectedValue != String.Empty && cboTrafficArea.SelectedValue != String.Empty)
                        {
                            using (Facade.IResource facResource = new Facade.Resource())
                                facResource.AssignToArea(Convert.ToInt32(cboControlArea.SelectedValue), Convert.ToInt32(cboTrafficArea.SelectedValue), vehicle.ResourceId, "Vehicle has been updated", ((Entities.CustomPrincipal)Page.User).UserName);
                        }

                        audit.Reload();

                        mwhelper.OutputData = lblConfirmation.Text;
                        mwhelper.CloseForm = true;
                    }
                    else
                    {
                        // Display the infringements
                        infringementDisplay.Visible = true;
                        infringementDisplay.Infringements = retVal.Infringements;
                        infringementDisplay.DisplayInfringments();

                        lblConfirmation.Visible = true;
                        lblConfirmation.ForeColor = Color.Red;

                        if (!string.IsNullOrEmpty(retVal.ErrorMessage))
                            lblConfirmation.Text =  retVal.ErrorMessage;
                        else
                            lblConfirmation.Text = "The vehicle has not been updated successfully.";
                    }
                }
                else
                {
                    if (retVal.Success)
                    {
                        infringementDisplay.Visible = false;

                        lblConfirmation.Text = "The vehicle has been added successfully.";
                        // refresh the page we came from if it is expecting a call back
                        this.ClientScript.RegisterStartupScript(this.GetType(), "CallBack", "window.opener.location.reload();", true);

                        if (cboControlArea.SelectedValue != String.Empty && cboTrafficArea.SelectedValue != String.Empty)
                        {
                            using (Facade.IResource facResource = new Facade.Resource())
                                facResource.AssignToArea(Convert.ToInt32(cboControlArea.SelectedValue), Convert.ToInt32(cboTrafficArea.SelectedValue), vehicle.ResourceId, "Vehicle has been updated", ((Entities.CustomPrincipal)Page.User).UserName);
                        }

                        // Switch to update mode
                        ViewState[C_RESOURCE_ID] = vehicle.ResourceId;
                        m_isUpdate = true;

                        string confirmMessage = @"javascript:return(confirm('Any changes you have made will need to be saved before you continue.\nTo save your changes, click Cancel, the click the " + (m_isUpdate ? "Update" : "Add") + " button, you may then try again.'))";
                        btnAddVehicleKey.Enabled = m_isUpdate;
                        btnAddVehicleKey.Attributes.Add("onClick", confirmMessage);
                        btnAdd.Text = "Update";

                        mwhelper.OutputData = lblConfirmation.Text;
                        mwhelper.CloseForm = true;
                    }
                    else
                    {

                        // Display the infringements
                        infringementDisplay.Visible = true;
                        infringementDisplay.Infringements = retVal.Infringements;
                        infringementDisplay.DisplayInfringments();

                        lblConfirmation.Visible = true;
                        lblConfirmation.ForeColor = Color.Red;

                        if (!string.IsNullOrEmpty(retVal.ErrorMessage))
                            lblConfirmation.Text = retVal.ErrorMessage;
                        else
                            lblConfirmation.Text = "The vehicle has not been updated successfully.";
                    }
                }

                lblConfirmation.Visible = true;
            }
        }


        #endregion

        #region Add/Update/Populate/Load Vehicle Keys

        private void PopulateKeys()
        {
            dgdKeys.DataSource = GetKeyData();

            if (((DataSet)dgdKeys.DataSource).Tables[0].Rows.Count > 0)
            {
                dgdKeys.Visible = true;
                dgdKeys.DataBind();
            }
            else
            {
                dgdKeys.Visible = false;
            }
        }


        private DataSet GetKeyData()
        {
            Facade.IVehicleKey facVehicleKey = new Facade.Resource();
            return facVehicleKey.GetForVehicleId(m_resourceId);
        }


        private void btnAddVehicleKey_Click(object sender, EventArgs e)
        {
            Response.Redirect("addupdatevehiclekey.aspx?resourceId=" + m_resourceId + "&vehicleRegistrationNo=" + vehicleRegistrationNo);
        }

        #region Vehicle Key Sorting and Paging

        protected string VehicleKeySortCriteria
        {
            get { return (string)ViewState["VehicleKey_SortCriteria"]; }
            set { ViewState["VehicleKey_SortCriteria"] = value; }
        }

        protected string VehicleKeySortDirection
        {
            get { return (string)ViewState["VehicleKey_SortDirection"]; }
            set { ViewState["VehicleKey_SortDirection"] = value; }
        }

        protected void dgdKeys_Page(Object sender, DataGridPageChangedEventArgs e)
        {
            dgdKeys.DataSource = GetKeyData();
            dgdKeys.CurrentPageIndex = e.NewPageIndex;
            dgdKeys.DataBind();
        }

        protected void dgdKeys_SortCommand(object source, System.Web.UI.WebControls.DataGridSortCommandEventArgs e)
        {
            DataSet dsKeys = GetKeyData();
            DataView dv = new DataView(dsKeys.Tables[0]);

            if (this.VehicleKeySortCriteria == e.SortExpression)
                if (this.VehicleKeySortDirection == "desc")
                    this.VehicleKeySortDirection = "asc";
                else
                    this.VehicleKeySortDirection = "desc";

            this.VehicleKeySortCriteria = e.SortExpression;

            dv.Sort = e.SortExpression + ' ' + this.VehicleKeySortDirection;
            dgdKeys.DataSource = dv;
            dgdKeys.DataBind();
        }

        #endregion
        #endregion

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
            trvResourceGrouping.NodeDataBound += new RadTreeViewEventHandler(trvResourceGrouping_NodeDataBound);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Init += new EventHandler(addupdatevehicle_Init);

        }
        #endregion

    }
}
