using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.UI.WebControls;
using Orchestrator.Entities;
using Orchestrator.Facade;
using Orchestrator.Globals;
using System.Collections.Generic;
using Orchestrator.WebUI.Code.components;
using Orchestrator.Exceptions;
using Orchestrator.Models;
using Orchestrator.Repositories;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.Resource.Driver
{
	/// <summary>
	/// Summary description for AddUpdateDriver.
	/// </summary>
	public partial class AddUpdateDriver : Base.BasePage
	{
		
		#region Page Variables 
		
		private bool _isUpdate;
		private int _identityId;
		private Entities.Driver _driver;
		private Entities.Point _point = null;

		protected int m_organisationId = 0;
		protected string m_startTown = String.Empty;
		protected int m_startTownId = 0;
		protected int m_pointId = 0;
        private List<TelematicsSolution> m_telematicsSolutions;
        private List<TreeDataNode> m_orgUnitTreeNodes;
		
		#endregion 

		#region Page Load/Init

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage, eSystemPortion.AddEditResource);
			btnAdd.Enabled = Security.Authorise.CanAccess(eSystemPortion.AddEditResource);

            if (Request.QueryString["rcbID"] == null)
            {
                _identityId = Convert.ToInt32(Request.QueryString["identityId"]);

                //Get the OrgUnits
                m_orgUnitTreeNodes = (from ou in EF.DataContext.Current.OrgUnits
                                      orderby ou.Name
                                      select new TreeDataNode
                                      {
                                          OrgUnitId = ou.OrgUnitId,
                                          ParentOrgUnitId = ou.ParentOrgUnitId,
                                          Text = ou.Name
                                      }).ToList();

                if (_identityId > 0 || ViewState["IsUpdate"] != null)
                    _isUpdate = true;

                if (!IsPostBack)
                {
                    PopulateStaticControls();
                    if (_isUpdate)
                        LoadDriver();

                    BindResourceGroupings();
                }
            }
		}

		protected void AddUpdateDriver_Init(object sender, EventArgs e)
		{
			btnAdd.Click += new System.EventHandler(btnAdd_Click);
			cboOrganisation.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);
            cboTown.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboTown_ItemsRequested);
            cboPoint.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboPoint_ItemsRequested);

            cfvPoint.ServerValidate += new ServerValidateEventHandler(cfvPoint_ServerValidate);
		}

        private void BindDriverCommunicationTypes()
        {
            var names = new List<string>();
            names.AddRange(Enum.GetNames(typeof(eDriverCommunicationType)));
            names.Insert(0, "None");
            rblDefaultCommunicationType.DataSource = names;
            rblDefaultCommunicationType.DataBind();
            rblDefaultCommunicationType.SelectedIndex = 0;
        }

        void cfvPoint_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = P1TP.Components.Common.SimpleValidation.ValidateNumericValue(cboPoint.SelectedValue, 1, true);
        }

        #endregion 

		#region Populate Controls
		
        ///	<summary> 
		/// Populate Static Controls
		///	</summary>
		private void PopulateStaticControls()
		{
            if(Orchestrator.Globals.Configuration.FleetMetrikInstance)
                OnlyEnableFleetMetrikFields();

            cboTitle.DataSource = Utilities.UnCamelCase(Enum.GetNames(typeof(eTitle)));
			cboTitle.DataBind();

            IDriver facDriver = new Facade.Resource();
            cboDriverType.DataSource = facDriver.GetAllDriverTypes();
            cboDriverType.DataTextField = "Description";
            cboDriverType.DataValueField = "DriverTypeID";
			cboDriverType.DataBind();
			cboDriverType.Items.Insert(0, new ListItem("--- [ Please Select ] ---", ""));

            // Configure the attributes for the address target textboxes.
            txtAddressLine1.Attributes.Add("onfocus", "this.blur();" + txtPostCode.ClientID + ".focus();");
            txtAddressLine2.Attributes.Add("onfocus", "this.blur();" + txtPostCode.ClientID + ".focus();");
            txtAddressLine3.Attributes.Add("onfocus", "this.blur();" + txtPostCode.ClientID + ".focus();");
            txtPostTown.Attributes.Add("onfocus", "this.blur();" + txtPostCode.ClientID + ".focus();");
            txtCounty.Attributes.Add("onfocus", "this.blur();" + txtPostCode.ClientID + ".focus();");

			IVehicle facVehicle = new Facade.Resource();
			var dsVehicles = facVehicle.GetAllVehicles();
			cboVehicle.DataSource = dsVehicles;
			cboVehicle.DataTextField = "RegNo";
			cboVehicle.DataValueField = "ResourceId";
			cboVehicle.DataBind();
			cboVehicle.Items.Insert(0, new ListItem("--- [ Please Select ] ---", "-1"));

			IOrganisationLocation facOrganiastionLocation = new Facade.Organisation();
			cboDepot.DataSource = facOrganiastionLocation.GetAllDepots(Configuration.IdentityId);
			cboDepot.DataValueField = "OrganisationLocationId";
			cboDepot.DataTextField = "OrganisationLocationName";
			cboDepot.DataBind();
			cboDepot.Items.Insert(0, new Telerik.Web.UI.RadComboBoxItem("--- [ Please Select ] ---"));

			IControlArea facControlArea = new Facade.Traffic();

			cboControlArea.DataSource = facControlArea.GetAll();
			cboControlArea.DataTextField = "Description";
			cboControlArea.DataValueField = "ControlAreaId";
			cboControlArea.DataBind();
            cboControlArea.Items.Insert(0, new Telerik.Web.UI.RadComboBoxItem("--- [ Please Select ] ---"));

			var facTrafficArea = (ITrafficArea) facControlArea;
			cboTrafficArea.DataSource = facTrafficArea.GetAll();
			cboTrafficArea.DataTextField = "Description";
			cboTrafficArea.DataValueField = "TrafficAreaId";
			cboTrafficArea.DataBind();
            cboTrafficArea.Items.Insert(0, new Telerik.Web.UI.RadComboBoxItem("--- [ Please Select ] ---"));

            this.dteDOB.MinDate = new DateTime(1900, 1, 1);

            IUser facUser = new Facade.User();

            cboDriverPlanner.DataSource = facUser.GetAllUsersInRole(eUserRole.Planner);
            cboDriverPlanner.DataValueField = "IdentityID";
            cboDriverPlanner.DataTextField = "FullName";
            cboDriverPlanner.DataBind();
            cboDriverPlanner.Items.Insert(0, new Telerik.Web.UI.RadComboBoxItem("--- [ Please Select ] ---"));

            //Load Agencies
            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var agenciesRepo = DIContainer.CreateRepository<IAgencyRepository>(uow);
                var agencies = agenciesRepo.GetAgencies();
                foreach(var ag in agencies)
                {
                    var rcItem = new Telerik.Web.UI.RadComboBoxItem();
                    rcItem.Text = ag.Name;
                    rcItem.Value = ag.AgencyId.ToString();
                    cboAgency.Items.Add(rcItem);
                }
            }

            InitialiseTelematicsSolution();

            BindDriverCommunicationTypes();
		}

        private void OnlyEnableFleetMetrikFields()
        {
            trPasscode.Visible = false;
            trDriverType.Visible = false;
            trDOB.Visible = false;
            trStartDate.Visible = false;
            trHomePhone.Visible = false;
            trPersonalMobile.Visible = false;
            trPayrollNumber.Visible = false;
            trPrefCommunication.Visible = false;
            fsAddress.Visible = false;
            fsResourceGrouping.Visible = false;
            fsLKL.Visible = false;
            fsBelongsTo.Visible = false;
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

        #endregion 

		#region Events & Methods

		///	<summary> 
		/// Button Add Click
		///	</summary>
		private void btnAdd_Click(object sender, System.EventArgs e)
		{
			if (Page.IsValid)
			{
				bool retVal = false;

				if (_driver==null)
					populateDriver();

				if (_isUpdate)
				{
					retVal = UpdateDriver();

					if (retVal)
					{
						lblConfirmation.Text = "The Driver has been updated successfully";

                        if (cboControlArea.SelectedValue != String.Empty && cboTrafficArea.SelectedValue != String.Empty)
						{
							using (Facade.IResource facResource = new Facade.Resource())
                                facResource.AssignToArea(Convert.ToInt32(cboControlArea.SelectedValue), Convert.ToInt32(cboTrafficArea.SelectedValue), _driver.ResourceId, "Driver has been updated", ((Entities.CustomPrincipal)Page.User).UserName);
						}

                        this.ReturnValue = lblConfirmation.Text;
                    }
				}
				else
				{
					retVal = AddDriver();

					if (retVal)
					{
						lblConfirmation.Text = "The Driver has been added successfully.";

                        if (cboControlArea.SelectedValue != String.Empty && cboTrafficArea.SelectedValue != String.Empty)
						{
							using (Facade.IResource facResource = new Facade.Resource())
                                facResource.AssignToArea(Convert.ToInt32(cboControlArea.SelectedValue), Convert.ToInt32(cboTrafficArea.SelectedValue), _driver.ResourceId, "Driver has been updated", ((Entities.CustomPrincipal)Page.User).UserName);
						}

                        this.ReturnValue = lblConfirmation.Text;
                    }
				}

				lblConfirmation.Visible = true;

				if (retVal)
				{
                    this.ReturnValue = "success";
                    this.Close();
				}
			}
		}

        #endregion 

		#region Add/Update Point
		private bool UpdatePoint()
		{
			Facade.IPoint facPoint= new Facade.Point();
			string userName = ((Entities.CustomPrincipal)Page.User).UserName;
			
			Entities.FacadeResult retVal = facPoint.Update(_point, userName);

			return retVal.Success;

		}
		private bool AddPoint()
		{
			if (_point != null)
			{
				Facade.IPoint facPoint= new Facade.Point();
				string userName = ((Entities.CustomPrincipal)Page.User).UserName;

				Entities.FacadeResult retVal = facPoint.Create(_point, userName);

				return retVal.Success;
			}

			return false;
		}
		#endregion 

		#region Add/Update/Populate/Load Driver
		///	<summary> 
		/// Add Driver
		///	</summary>
		private bool AddDriver()
		{
			int resourceId = 0;
			Facade.IDriver facResource = new Facade.Resource();
			string userName = ((Entities.CustomPrincipal)Page.User).UserName;

            Entities.FacadeResult retVal;

            try
            {
                retVal = facResource.Create(_driver, userName, DateTime.Now);
            }
            catch (BusinessFacadeException ex)
            {
                if (ex.InnerException is BusinessLayerException && ex.InnerException.InnerException is ThirdPartyTelematicsException)
                {
                    lblConfirmation.Text = "There was an error updating the vehicle on a third party telematics system. It may not be usable, please contact support.";
                    lblConfirmation.Visible = true;
                    lblConfirmation.ForeColor = Color.Red;
                    return false;
                }
                else
                    throw;
            }


		    if (retVal.Success)
		    {
                resourceId = retVal.ObjectId;
                _driver.ResourceId = resourceId;

                // populate the driver title with the resouce id, this will be overwritten when gets added to bluesphere
                _driver.DriverTitle = _driver.ResourceId.ToString();

                ViewState["driver"] = _driver;

                // Only try to add to bluesphere if has a passcode and a BlueSphereCustomerId.
                if (!string.IsNullOrEmpty(Configuration.BlueSphereCustomerId) && !string.IsNullOrEmpty(_driver.Passcode))
		        {
		            try
		            {
                       string message = MobileWorkerFlow.MWFServicesCommunication.Client.AddNewDriverToBlueSphere(ref _driver);

		                if (!string.IsNullOrEmpty(message))
		                {
		                    lblConfirmation.Text = "There was an error setting up the driver for MWF, " + message;
                            lblConfirmation.Visible = true;
                            lblConfirmation.ForeColor = Color.Red;
		                    ViewState["IsUpdate"] = _isUpdate = true;

                            // If not set up for mwf then remove the passcode to avoid trying to communicate to them.
                            ResetDriverPasscode();

                            LoadDriver();
		                    return false;
		                }

                        // Update the driver in viewstate, probably won't be used as the window is closed when added but done this just in case.
                        ViewState["driver"] = _driver;
		            }
		            catch
		            {
		                // MM (8th March 2012): Not ideal but solution for now, if fail to get through to bluesphere then show error to user
		                    lblConfirmation.Text = "There was an error adding the driver and they will not be usable, please contact support.";

		                lblConfirmation.Visible = true;
		                lblConfirmation.ForeColor = Color.Red;
                        ViewState["IsUpdate"] = _isUpdate = true;

                        // if not set up for mwf then remove the passcode to avoid trying to communicate to them.
                        ResetDriverPasscode();

                        LoadDriver();
		                return false;
		            }
		        }
		    }
            else
            {
                lblConfirmation.Text = "There was an error adding the driver, please try again. ";

                foreach (Entities.BusinessRuleInfringement item in retVal.Infringements)
                {
                    if (!string.IsNullOrEmpty(item.Description))
                        lblConfirmation.Text += string.Format("<br/>{0}", item.Description.ToString());
                }
            }

            if (resourceId == 0)
            {
                if (string.IsNullOrEmpty(lblConfirmation.Text))
                    lblConfirmation.Text += "There was an error adding the driver.";

                lblConfirmation.Visible = true;
                lblConfirmation.ForeColor = Color.Red;
                return false;
            }
            else
            {
                _isUpdate = true;
                return true;
            }
		}


		///	<summary> 
		/// Update Driver
		///	</summary>
		private bool UpdateDriver()
		{
			Facade.IDriver facResource = new Facade.Resource();
			string userName = ((Entities.CustomPrincipal)Page.User).UserName;

            Entities.FacadeResult retVal;

            try
            {
                retVal = facResource.Update(_driver, userName, DateTime.Now);
            }
            catch (BusinessFacadeException ex)
            {
                if (ex.InnerException is BusinessLayerException && ex.InnerException.InnerException is ThirdPartyTelematicsException)
                {
                    lblConfirmation.Text = "There was an error updating the driver on a third party telematics system. They may not be usable, please contact support.";
                    lblConfirmation.Visible = true;
                    lblConfirmation.ForeColor = Color.Red;
                    return false;
                }
                else 
                    throw;
            }

            if (retVal.Success)
            {
                // Only try to update the driver if they have a passcode and a BlueSphereCustomerId.
                if (!string.IsNullOrEmpty(Configuration.BlueSphereCustomerId) && !string.IsNullOrEmpty(_driver.Passcode))
                {
                    try
                    {
                        var message = MobileWorkerFlow.MWFServicesCommunication.Client.UpdateDriverInBlueSphere(ref _driver);

                        if (!string.IsNullOrEmpty(message))
                        {
                            lblConfirmation.Text = "There was an error updating the the driver for MWF, " + message;
                            lblConfirmation.Visible = true;
                            lblConfirmation.ForeColor = Color.Red;
                            
                            // If not set up for mwf then remove the passcode to avoid trying to communicate to them.
                            ResetDriverPasscode();

                            LoadDriver();
                            return false;
                        }

                        // Update the driver in viewstate, probably won't be used as the window is closed when updated but done this just in case.
                        ViewState["driver"] = _driver;
                    }
                    catch (Exception ex)
                    {
                        // MM (8th March 2012): Not ideal but solution for now, if fail to get through to bluesphere then show error to user
                            lblConfirmation.Text =
                            "There was an error updating the driver and they may not be usable, please contact support.";

                        lblConfirmation.Visible = true;
                        lblConfirmation.ForeColor = Color.Red;
                        
                        // If not set up for mwf then remove the passcode to avoid trying to communicate to them.
                        ResetDriverPasscode();
                        LoadDriver();
                        return false;
                    }
                }

                return true;
            }
            else
            {
                lblConfirmation.Text = "There was an error updating the driver, please try again. ";

                foreach (Entities.BusinessRuleInfringement item in retVal.Infringements)
                {
                    if (!string.IsNullOrEmpty(item.Description))
                        lblConfirmation.Text += string.Format("<br/>{0}", item.Description.ToString());
                }

                lblConfirmation.Visible = true;
                lblConfirmation.ForeColor = Color.Red;

                return false;
            }            
		}

	    private void ResetDriverPasscode()
	    {
            var userName = ((Entities.CustomPrincipal)Page.User).UserName;
	        IDriver facResource= new Facade.Resource();
            _driver.Passcode = null;
	        facResource.Update(_driver, userName);
	        ViewState["driver"] = _driver;
	    }

        ///	<summary> 
		/// Load Driver 
		///	</summary>
		private void LoadDriver()
		{		
			if (ViewState["driver"]==null)
			{
				IDriver facDriver = new Facade.Resource();
				_driver = facDriver.GetDriverForIdentityId(_identityId);

				ViewState["driver"] = _driver;
			}
			else
				_driver = (Entities.Driver)ViewState["driver"];

			if (_driver != null)
			{
                hypAddNewVehicle.Visible = true;
				cboTitle.SelectedValue = Utilities.UnCamelCase(_driver.Individual.Title.ToString());
				txtFirstNames.Text = _driver.Individual.FirstNames;
				txtLastName.Text = _driver.Individual.LastName;
			    txtPasscode.Text = _driver.Passcode;

                dteDOB.SelectedDate = _driver.Individual.DOB;

				if (_driver.Individual.Contacts != null && _driver.Individual.Contacts.Count > 0)
				{
					Entities.Contact telephone = _driver.Individual.Contacts.GetForContactType(eContactType.Telephone);
					Entities.Contact mobile = _driver.Individual.Contacts.GetForContactType(eContactType.MobilePhone);
					Entities.Contact personalMobile = _driver.Individual.Contacts.GetForContactType(eContactType.PersonalMobile);

					if (telephone != null)
						txtTelephone.Text = telephone.ContactDetail;
					if (mobile != null)
						txtMobilePhone.Text = mobile.ContactDetail;
					if (personalMobile != null)
						txtPersonalMobile.Text = personalMobile.ContactDetail;
				}

				if (_driver.Individual.Address != null)
				{
					txtAddressLine1.Text = _driver.Individual.Address.AddressLine1;
					txtAddressLine2.Text = _driver.Individual.Address.AddressLine2;
					txtAddressLine3.Text = _driver.Individual.Address.AddressLine3;

					txtPostTown.Text = _driver.Individual.Address.PostTown;
					txtCounty.Text = _driver.Individual.Address.County;
					txtPostCode.Text = _driver.Individual.Address.PostCode;

					if (_driver.Individual.Address.TrafficArea != null)
						hidTrafficArea.Value = _driver.Individual.Address.TrafficArea.TrafficAreaId.ToString();
				}

				foreach(Entities.Contact contact in _driver.Individual.Contacts)
				{
					if (contact.ContactType == eContactType.Telephone)
						txtTelephone.Text = contact.ContactDetail;
					if (contact.ContactType == eContactType.MobilePhone)
						txtMobilePhone.Text = contact.ContactDetail;
					if (contact.ContactType == eContactType.PersonalMobile)
						txtPersonalMobile.Text = contact.ContactDetail;
				}

                Facade.IPoint facPoint = new Facade.Point();
				Entities.Point point = facPoint.GetPointForPointId(_driver.HomePointId);

                if (point != null && point.PointId > 0)
                {
                    cboOrganisation.Text = point.OrganisationName;
                    cboOrganisation.SelectedValue = point.IdentityId.ToString();
                    m_organisationId = point.IdentityId;

                    m_startTown = point.PostTown.TownName;
                    m_startTownId = point.PostTown.TownId;

                    cboPoint.Text = point.Description;
                    cboPoint.SelectedValue = point.PointId.ToString();
                    m_pointId = point.PointId;
                }

				cboDriverType.Items.FindByValue(_driver.DriverType.DriverTypeID.ToString()).Selected = true;
				
				cboVehicle.ClearSelection();
				if (_driver.AssignedVehicleId != 0)
				{
					ListItem vehicle = cboVehicle.Items.FindByValue(_driver.AssignedVehicleId.ToString());
					if (vehicle != null)
						vehicle.Selected = true;
				}

                txtDigitalTachoCardId.Text = _driver.DigitalTachoCardId;
                chkAgencyDriver.Checked = _driver.IsAgencyDriver;

				if (_driver.Individual.IdentityStatus == eIdentityStatus.Deleted)
					chkDelete.Checked = true;

				if (_driver.Point != null)
				{
					txtLatitude.Text = _driver.Point.Latitude.ToString();
					txtLongitude.Text = _driver.Point.Longitude.ToString();
					cboTown.SelectedValue = _driver.Point.PostTown.TownId.ToString();
					cboTown.Text = _driver.Point.PostTown.TownName;
				}

				Entities.ControlArea ca = null;
				Entities.TrafficArea ta = null;

				using (Facade.IResource facResource = new Facade.Resource())
					facResource.GetControllerForResourceId(_driver.ResourceId, ref ca, ref ta);

				cboDepot.ClearSelection();
				if (_driver.DepotId > 0)
					cboDepot.FindItemByValue(_driver.DepotId.ToString()).Selected = true;

				if (ca != null && ta != null)
				{
					cboControlArea.ClearSelection();
					cboControlArea.FindItemByValue(ca.ControlAreaId.ToString()).Selected = true;
					cboTrafficArea.ClearSelection();
					cboTrafficArea.FindItemByValue(ta.TrafficAreaId.ToString()).Selected = true;
				}

				chkDelete.Visible = true;
				pnlDriverDeleted.Visible = true;

                if (_driver.DefaultCommunicationTypeID == 0)
                    rblDefaultCommunicationType.Items.FindByText("None").Selected = true;
                else
                    rblDefaultCommunicationType.Items.FindByText(((eDriverCommunicationType)_driver.DefaultCommunicationTypeID).ToString()).Selected = true;
                txtPayrollNo.Text = _driver.PayrollNo;
                dteSD.SelectedDate = _driver.StartDate;

                if (telematicsOption.Visible)
                {
                    cboTelematicsSolution.ClearSelection();
                    if (_driver.TelematicsSolution.HasValue)
                        cboTelematicsSolution.Items.FindByText(_driver.TelematicsSolution.ToString()).Selected = true;
                }

                //Find all tree nodes that correspond to the org units
                var treeNodesToCheck = m_orgUnitTreeNodes.Where(x => _driver.OrgUnitIDs.Contains(x.OrgUnitId.Value));
                foreach (var treeNode in treeNodesToCheck)
                {
                    treeNode.Checked = true;
                }

                if (_driver.PlannerIdentityID != null)
			    {
			        cboDriverPlanner.ClearSelection();
			        cboDriverPlanner.FindItemByValue(_driver.PlannerIdentityID.ToString()).Selected = true;
			    }

                if(_driver.AgencyId != null)
                {
                    cboAgency.ClearSelection();
                    cboAgency.FindItemByValue(_driver.AgencyId.ToString()).Selected = true;
                }
			}

			btnAdd.Text = "Update";
		}

        ///	<summary> 
   		/// Populate Driver
		///	</summary>
		private void populateDriver()
		{
			if (ViewState["driver"] ==null)
				_driver = new Entities.Driver();
			else
				_driver = (Entities.Driver)ViewState["driver"];

			_driver.ResourceType = eResourceType.Driver;
			if (_driver.Individual == null)
				_driver.Individual = new Entities.Individual();

			_driver.Individual.Title = (eTitle)Enum.Parse(typeof(eTitle), cboTitle.SelectedValue.Replace(" ", ""));
			_driver.Individual.FirstNames = txtFirstNames.Text;
			_driver.Individual.LastName = txtLastName.Text;
            if (dteDOB.SelectedDate != null)
                _driver.Individual.DOB = dteDOB.SelectedDate.Value;
            else
                _driver.Individual.DOB = DateTime.MinValue;
			_driver.Individual.IndividualType = eIndividualType.Driver;
            _driver.Passcode = txtPasscode.Text;

			if (_driver.Individual.Address == null)
			{
				_driver.Individual.Addresses = new Entities.AddressCollection();
				_driver.Individual.Addresses.Add(new Entities.Address());
			}

			_driver.Individual.Address.AddressLine1 = txtAddressLine1.Text;
			_driver.Individual.Address.AddressLine2 = txtAddressLine2.Text;
			_driver.Individual.Address.AddressLine3 = txtAddressLine3.Text;
			_driver.Individual.Address.PostTown = txtPostTown.Text;
			_driver.Individual.Address.County = txtCounty.Text;
			_driver.Individual.Address.PostCode = txtPostCode.Text;

			if (_driver.Individual.Address.TrafficArea == null)
			{
				_driver.Individual.Address.TrafficArea = new Orchestrator.Entities.TrafficArea();
			}
            if(hidTrafficArea.Value != "")
			    _driver.Individual.Address.TrafficArea.TrafficAreaId = Convert.ToInt32(hidTrafficArea.Value);

			if (_driver.Individual.Contacts == null)
			{
				_driver.Individual.Contacts = new Entities.ContactCollection();
			}

			Entities.Contact contact = _driver.Individual.Contacts.GetForContactType(eContactType.Telephone);
			if (null == contact)
			{
				contact = new Entities.Contact();
				contact.ContactType = eContactType.Telephone;
				_driver.Individual.Contacts.Add(contact);
			}
			
			contact.ContactDetail = txtTelephone.Text;
			
			contact = _driver.Individual.Contacts.GetForContactType(eContactType.MobilePhone);
			if(null == contact)
			{
				contact = new Entities.Contact();
				contact.ContactType = eContactType.MobilePhone;
				_driver.Individual.Contacts.Add(contact);
			}

			contact.ContactDetail = txtMobilePhone.Text;

			contact = _driver.Individual.Contacts.GetForContactType(eContactType.PersonalMobile);
			
			if (contact == null)
			{
				contact = new Entities.Contact();
				contact.ContactType = eContactType.PersonalMobile;
				_driver.Individual.Contacts.Add(contact);
			}
			
			contact.ContactDetail = txtPersonalMobile.Text;

            // Retrieve the organisation and place it in viewstate
            Facade.IOrganisation facOrganisation = new Facade.Organisation();
            Entities.Organisation m_organisation = facOrganisation.GetForIdentityId(Orchestrator.Globals.Configuration.IdentityId);
            Entities.OrganisationLocation currentOrganisationLocation = null;
            Entities.Point currentPoint = null;
            Entities.Address currentAddress = null;
            if (m_organisation != null && m_organisation.Locations.Count > 0)
            {
                currentOrganisationLocation = m_organisation.Locations.GetHeadOffice();
                currentPoint = currentOrganisationLocation.Point;
                currentAddress = currentPoint.Address;
            }

            if (cboPoint.SelectedValue != "")
                _driver.HomePointId = Convert.ToInt32(cboPoint.SelectedValue);
            else
                _driver.HomePointId = currentPoint.PointId;
			if (Convert.ToInt32(cboVehicle.SelectedValue) > 0)
				_driver.AssignedVehicleId = Convert.ToInt32(cboVehicle.SelectedValue);
			else
				_driver.AssignedVehicleId = 0;

			if (chkDelete.Checked)
			{
				_driver.Individual.IdentityStatus = eIdentityStatus.Deleted;
				_driver.ResourceStatus = eResourceStatus.Deleted;
			}
			else
			{
				_driver.Individual.IdentityStatus = eIdentityStatus.Active;
				_driver.ResourceStatus = eResourceStatus.Active;
			}

            if (_driver.Point == null && cboTown.SelectedValue != "")
			{
				_driver.Point = new Entities.Point();
			}

            if (cboDepot.SelectedValue != "")
                _driver.DepotId = Convert.ToInt32(cboDepot.SelectedValue);
            else
                _driver.DepotId = currentOrganisationLocation.OrganisationLocationId;
			
			if (_driver.Point != null)
			{
                decimal dLatitude = 0;
                decimal.TryParse(txtLatitude.Text, out dLatitude);
                decimal dLongtitude = 0;
                decimal.TryParse(txtLongitude.Text, out dLongtitude);

                _driver.Individual.Address.Latitude = dLatitude;
                _driver.Individual.Address.Longitude = dLongtitude;
				_driver.Point.PostTown = new Entities.PostTown();
				Facade.IPostTown facPostTown = new Facade.Point();
				_driver.Point.IdentityId = 0;
                _driver.Point.PostTown = facPostTown.GetPostTownForTownId(Convert.ToInt32(cboTown.SelectedValue));
				_driver.Point.Address = _driver.Individual.Address;                
                _driver.Point.Latitude = dLatitude;                
                _driver.Point.Longitude = dLongtitude;
				_driver.Point.Description = _driver.Individual.FirstNames + " " + _driver.Individual.LastName + " - Home";
                // We must try to set the radius of the point so that a new geofence
                // can be generated as we have just changed the point location through the address lookup.
                // not sure that this is neccessary for drivers though
                if (!String.IsNullOrEmpty(this.hdnSetPointRadius.Value))
                    _driver.Point.Radius = Globals.Configuration.GPSDefaultGeofenceRadius;
			}

            if (cboDriverType.SelectedValue != "")
                _driver.DriverType.DriverTypeID = int.Parse(cboDriverType.SelectedValue);
            else
                _driver.DriverType.DriverTypeID = 1;

            _driver.DigitalTachoCardId = txtDigitalTachoCardId.Text;
			_driver.IsAgencyDriver = chkAgencyDriver.Checked;
            if (dteSD.SelectedDate != null)
            {
                _driver.StartDate = dteSD.SelectedDate.Value;
            }
            else
            {
                _driver.StartDate = null;
            }
            _driver.PayrollNo = txtPayrollNo.Text; 

            string selectedCommunicationType = rblDefaultCommunicationType.SelectedValue;
            if (selectedCommunicationType == "None")
                _driver.DefaultCommunicationTypeID = 0;
            else
            {
                _driver.DefaultCommunicationTypeID = (int)Enum.Parse(typeof(eDriverCommunicationType), rblDefaultCommunicationType.SelectedValue);
            }

            _driver.OrgUnitIDs = trvResourceGrouping.CheckedNodes.Select(x => Int32.Parse(x.Attributes["OrgUnitID"])).ToList();

            // if the telematics option is visible use the selected solution (validation enforces a non-null selection)
            if (telematicsOption.Visible)
            {
                _driver.TelematicsSolution = (eTelematicsSolution?)Enum.Parse(typeof(eTelematicsSolution), cboTelematicsSolution.SelectedValue);
            }
            else
            {
                // if telematics option is not visible and we're creating, use the default
                if (!_isUpdate && ViewState["defaultTelematicsSolution"] != null)
                    _driver.TelematicsSolution = (eTelematicsSolution)ViewState["defaultTelematicsSolution"];
                //otherwise leave the telematics solution as it is
            }

            if (!string.IsNullOrEmpty(cboDriverPlanner.SelectedValue))
            {
                _driver.PlannerIdentityID = Convert.ToInt32(cboDriverPlanner.SelectedValue);
            }
            else
            {
                _driver.PlannerIdentityID = null;   //PROT-6452 - un-allocating a driver from its planner
            }

            if (chkAgencyDriver.Checked)
            {
                if (cboAgency.SelectedIndex == -1)
                {
                    var agency = new Agency()
                    {
                        Name = cboAgency.Text
                    };

                    using (var uow = DIContainer.CreateUnitOfWork())
                    {
                        var agenciesRepo = DIContainer.CreateRepository<IAgencyRepository>(uow);
                        agenciesRepo.Add(agency);
                        uow.SaveChanges();
                    }
                    _driver.AgencyId = agency.AgencyId;
                }
                else
                {
                    _driver.AgencyId = int.Parse(cboAgency.SelectedItem.Value);
                }
            }
            else
                _driver.AgencyId = null;
            

			ViewState["driver"] = _driver;
		}

        private void trvResourceGrouping_NodeDataBound(object sender, RadTreeNodeEventArgs e)
        {
            TreeDataNode dataNode = (TreeDataNode)e.Node.DataItem;
            e.Node.Attributes["OrgUnitId"] = dataNode.OrgUnitId.ToString();
        }
		#endregion

		#region Adds/Update/Populate/Load Availability

		///	<summary> 
		/// Populate Driver Availability
		///	</summary>
		private void populateAvailability()
		{
			 // TODO: Populate the drivers availability

		}

		#endregion

		#region DBCombo's Server Methods and Initialisation

		void cboClient_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
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
            if (e.Context["FilterString"] != null && e.Context["FilterString"].ToString()  != "")
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

        void cboTown_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            cboTown.Items.Clear();

            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetTownForTownName(e.Text);

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
                rcItem.Value = dt.Rows[i]["TownId"].ToString();
                cboTown.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }
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
            trvResourceGrouping.NodeDataBound += new RadTreeViewEventHandler(trvResourceGrouping_NodeDataBound);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Init += new System.EventHandler(this.AddUpdateDriver_Init);

		}
		#endregion
	}
}
