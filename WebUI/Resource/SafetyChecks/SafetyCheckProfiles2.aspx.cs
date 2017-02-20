using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.UI.Design.MobileControls;
using System.Web.UI.WebControls;
using MobileWorkerFlow.MWFServicesCommunication.MWFServices;
using Orchestrator.Entities;
using Orchestrator.Globals;
using Orchestrator.WebUI.Organisation;
using Telerik.Web.UI;
using System.Collections;

namespace Orchestrator.WebUI.Resource.SafetyChecks
{

    /// <summary>
   /// This page uses a direct connection to the BlueSphere Database due to repeated performance and corruption issues
   /// with the previous method
   /// 
   /// Please note that if you have any issues with this on a new installation please ensure that the Firewall (RapidSwitch) has been 
   /// updated to allow port 1433 from the IP Address of the new server.
    /// </summary>

    public partial class Profiles2 : System.Web.UI.Page
    {

        #region constants
        private const string __safetycheckprofilefaultlist = "__safetycheckprofilefaultlist";
        private const string __BSCustomer = "__BSCustomer";
        #endregion
        private SafetyCheckProfileAdminPages activePanel
        {
            get
            {
                SafetyCheckProfileAdminPages panel = SafetyCheckProfileAdminPages.ListSafetyCheckProfiles;
                if (ViewState["__scpadmin_activePanel"] != null) panel = (SafetyCheckProfileAdminPages)ViewState["__scpadmin_activePanel"];
                return panel;
            }
            set
            {
                ViewState["__scpadmin_activePanel"] = value;
            }
        }
        private enum SafetyCheckProfileAdminPages
        {
             ListSafetyCheckProfiles = 0
            ,AddOrEditSafetyCheckProfile = 1
            ,AssignProfileToVehicles = 2
        }

        private Fleetwood.BlueSphere.BusinessLogic.SafetyCheckProfileList ProfileList
        {
            get
            {
                return BSCustomer.SafetyCheckProfileList;
            }
        
        }

        private Guid CurrentProfileID
        {
            get
            {
                Guid retVal = Guid.Empty;
                if (this.ViewState["__currentProfileID"] != null)
                    retVal = (Guid)this.ViewState["__currentProfileID"];

                return retVal;
            }
            set
            {
                this.ViewState["__currentProfileID"] = value;
            }
        }
        private Fleetwood.BlueSphere.BusinessLogic.SafetyCheckProfile CurrentProfile
        {
            get
            {
                Fleetwood.BlueSphere.BusinessLogic.SafetyCheckProfile currentProfile = null;
                if (Session["__CurrentProfile"] != null )
                {
                    currentProfile = (Fleetwood.BlueSphere.BusinessLogic.SafetyCheckProfile)Session["__CurrentProfile"]; 
                }
                return currentProfile;

            }
            set
            {
                Session["__CurrentProfile"] = value;
                this.CurrentProfileID = value.ID;
            }
        }
        private Fleetwood.BlueSphere.BusinessLogic.FaultTypeList CurrentProfileFaultList
        {
            get
            {

               
                Fleetwood.BlueSphere.BusinessLogic.FaultTypeList profileFaults = null;
                if (Session[__safetycheckprofilefaultlist] != null) profileFaults = (Fleetwood.BlueSphere.BusinessLogic.FaultTypeList)Session[__safetycheckprofilefaultlist];
                return profileFaults;
            }
            set
            {
                Session[__safetycheckprofilefaultlist] = value;
            }
        }

        private Fleetwood.BlueSphere.BusinessLogic.Customer BSCustomer
        {
            get
            {
                if (Session[__BSCustomer] == null)
                {
                    Guid customerId = new Guid(Configuration.BlueSphereCustomerId);
                    Session[__BSCustomer] = new Fleetwood.BlueSphere.BusinessLogic.Customer(customerId);
                    
                }
                return Session[__BSCustomer] as Fleetwood.BlueSphere.BusinessLogic.Customer;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // reset session variables
                Session[__BSCustomer] = null;
                Session[__safetycheckprofilefaultlist] = null;

                gotoPanel(SafetyCheckProfileAdminPages.ListSafetyCheckProfiles);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage, eSystemPortion.AddEditResource);

            // ListSafetyCheckProfiles events
            rgProfiles.NeedDataSource += rgProfiles_NeedDataSource;
            rgProfiles.ItemCommand += rgProfiles_ItemCommand;
            btnAddNew.Click += btnAddNewProfile_Click;

            // AddOrEditSafetyCheckProfile events
            btnAddNewFault.Click += btnAddNewFault_Click;
            btnCancel.Click += btnCancel_Click; // BEWARE: Same cancel method is used below for panel assignment cancel button
            btnSaveChanges.Click += btnSaveChanges_Click;
            repeaterFaultList.ItemCommand += repeaterFaultList_ItemCommand;
            repeaterFaultList.ItemDataBound += repeaterFaultList_ItemDataBound;

            // AssignProfileToVehicles
            btnCancelAssignments.Click += btnCancel_Click; // BEWARE: Same cancel method is used above for add/edit profile panel cancel button
            btnSaveAssignments.Click += btnSaveAssignments_Click;

            chkShowAllVehicles.CheckedChanged += chkShowAllVehicles_CheckedChanged;
            // TODO: Attach more events ?

            if (!IsPostBack)
            {   // On initial load make sure objects reflect a new profile
                //CurrentProfile = new SafetyCheckProfile();
                //CurrentProfileFaultList = new List<SafetyCheckProfileFault>();
             
            }
        }

        

        /// <summary>
        /// On cancel click; this assumes client side validated user wants to abandon changes
        /// </summary>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            // Cheeky; clear everything down and invoke the first postback again. Seems valid to me...
            Response.Redirect(Request.RawUrl);
        }

        private void gotoPanel(SafetyCheckProfileAdminPages targetPanel)
        {

           
            activePanel = targetPanel;
            int activePanellIndex = (int)activePanel;
            switch (targetPanel)
            {
                case SafetyCheckProfileAdminPages.AssignProfileToVehicles:
                    rmpSafetyProfiles.SelectedIndex = activePanellIndex;
                    populateAssignProfileFields();
                    break;

                case SafetyCheckProfileAdminPages.AddOrEditSafetyCheckProfile:
                    deleteProfileError.Visible = false;
                    rmpSafetyProfiles.SelectedIndex = activePanellIndex;
                    populateProfileFields();
                    break;
                case SafetyCheckProfileAdminPages.ListSafetyCheckProfiles:
                default:
                    saveProfileError.Visible = false;
                    rmpSafetyProfiles.SelectedIndex = activePanellIndex;
                    rgProfiles.Rebind();
                    break;
            }
        }

        #region == Safety Check Profile List ==

        private void rgProfiles_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            Fleetwood.BlueSphere.BusinessLogic.SafetyCheckProfileList Profiles = this.BSCustomer.SafetyCheckProfileList;
                
            rgProfiles.DataSource = Profiles; ;
        }

        private void rgProfiles_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            string command = e.CommandName.ToUpper();
            if (e.Item.ItemIndex > -1)
            {
                Fleetwood.BlueSphere.BusinessLogic.SafetyCheckProfile  selectedRow = ProfileList[e.Item.ItemIndex];
                switch (command)
                {
                    case "EDITPROFILE":
                        // Load up the selected profile details then go to the add/edit page
                        editProfile(selectedRow, () => gotoPanel(SafetyCheckProfileAdminPages.AddOrEditSafetyCheckProfile));
                        break;

                    case "DELETEPROFILE":
                        // Delete the profile, then reload the profile list
                        deleteProfile(selectedRow, () => gotoPanel(SafetyCheckProfileAdminPages.ListSafetyCheckProfiles));
                        break;

                    case "ASSIGNPROFILE":
                        // Load up the selected profile details then go to the add/edit page
                        assignProfile(selectedRow, () => gotoPanel(SafetyCheckProfileAdminPages.AssignProfileToVehicles));
                        break;
                }
            }
        }

        private void addProfile(Action callback)
        {
            CurrentProfile = new Fleetwood.BlueSphere.BusinessLogic.SafetyCheckProfile();
            CurrentProfile.CustomerID = BSCustomer.ID;
            CurrentProfileFaultList = new Fleetwood.BlueSphere.BusinessLogic.FaultTypeList(false);
            btnSaveChanges.Text = "Save New Profile";
            btnCancel.Text = "Cancel";
            callback();
        }

        private void editProfile(Fleetwood.BlueSphere.BusinessLogic.SafetyCheckProfile targetProfile, Action callback)
        {
            CurrentProfile = targetProfile;
            CurrentProfileFaultList = CurrentProfile.FaultTypeList;
            btnSaveChanges.Text = "Save Changes";
            btnCancel.Text = "Cancel Changes";
            callback();
        }

        private void deleteProfile(Fleetwood.BlueSphere.BusinessLogic.SafetyCheckProfile targetProfile, Action callback)
        {
            CurrentProfile = targetProfile;
            CurrentProfile.IsDeleted = true;
            CurrentProfile.Update();
            callback();
        }

        private void assignProfile(Fleetwood.BlueSphere.BusinessLogic.SafetyCheckProfile targetProfile, Action callback)
        {
            this.CurrentProfile = targetProfile;
            lblAssigningProfileTitle.InnerText = targetProfile.Title;
            callback();
        }

        private void btnAddNewProfile_Click(object sender, EventArgs e)
        {
            // Create a new profile (and fault list) then go to the add/edit page
            addProfile(() => gotoPanel(SafetyCheckProfileAdminPages.AddOrEditSafetyCheckProfile));
        }

        #endregion

        #region == Add / Edit Safety Check Profile ==
        /// <summary>
        /// Populates all form fields according ot the current profile and current profile fault list
        /// </summary>
        private void populateProfileFields()
        {
            if (CurrentProfile != null)
            {
                txtProfileName.Text = CurrentProfile.Title;
                chkOdometer.Checked = CurrentProfile.Odometer;
                chkSignature.Checked = CurrentProfile.Signature;
                chkAtLogon.Checked = CurrentProfile.AtLogon;
                chkAtLogoff.Checked = CurrentProfile.AtLogoff;
                // How annoying; label for this field means we have to negate the DB Field :@
                chkVOSACompliant.Checked = !CurrentProfile.IsVOSACompliant;
                populateProfileFaultList();
            }
        }

        /// <summary>
        /// Rebinds the fault list table to the fault list for the current profile
        /// </summary>
        private void populateProfileFaultList()
        {
            // Resort the profile list here so we pick up any reordering the user may have performed.
            CurrentProfileFaultList.Sort("OrderNumber");
            repeaterFaultList.DataSource = CurrentProfileFaultList;
            repeaterFaultList.DataBind();
        }

        /// <summary>
        /// Ensures the CurrentProfile reflects the data entered into the fields on screen
        /// </summary>
        private void updateProfileDataSource()
        {
            
            CurrentProfile.Title = txtProfileName.Text.Trim();
            CurrentProfile.Odometer = chkOdometer.Checked;
            CurrentProfile.Signature = chkSignature.Checked;
            CurrentProfile.AtLogon = chkAtLogon.Checked;
            CurrentProfile.AtLogoff = chkAtLogoff.Checked;
            // How annoying; label for this field means we have to negate the value before it hits the DB :@
            CurrentProfile.IsVOSACompliant = !chkVOSACompliant.Checked;
        }

        /// <summary>
        /// Ensures the CurrentProfileFaultList reflects the data entered into the fault list table on screen
        /// </summary>
        private void updateFaultListDataSource()
        {
            for (int i = 0; i < CurrentProfileFaultList.Count; i++) 
            {
                Fleetwood.BlueSphere.BusinessLogic.FaultType thisFault = CurrentProfileFaultList[i];
                foreach (RepeaterItem item in repeaterFaultList.Items)
                {
                    if (item.ItemIndex == i)
                    {
                        TextBox txtFaultTitle = (TextBox)item.FindControl("txtFaultTitle");
                        thisFault.Title = txtFaultTitle.Text;
                        TextBox txtFaultCategory = (TextBox)item.FindControl("txtFaultCategory");
                        thisFault.FaultCategory = txtFaultCategory.Text;
                        CheckBox chkIsDiscretionary = (CheckBox)item.FindControl("chkIsDiscretionary");
                        thisFault.IsDiscretionaryQuestion = chkIsDiscretionary.Checked;
                        CheckBox chkHighlight = (CheckBox)item.FindControl("chkHighlight");
                        thisFault.Highlight = chkHighlight.Checked;
                        // We've updated thisFault now - break out of repeater loop and move onto next fault, there can categorically be no more matches
                        break;
                    }
                }
            }
        }

        private void repeaterFaultList_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem)
                return;

            ((RequiredFieldValidator)e.Item.FindControl("faultTitleVal")).ControlToValidate = ((TextBox)e.Item.FindControl("txtFaultTitle")).ID;
            // Commented out on purpose until they realise they want it back((RequiredFieldValidator)e.Item.FindControl("faultCatVal")).ControlToValidate = ((TextBox)e.Item.FindControl("txtFaultCategory")).ID;


            // Hide the up/down buttons on the appropriate rows
            if (e.Item.ItemIndex == 0) e.Item.FindControl("imgBtnUp").Visible = false;
            if (e.Item.ItemIndex == (CurrentProfileFaultList.Count - 1)) e.Item.FindControl("imgBtnDown").Visible = false;
        }

        private void repeaterFaultList_ItemCommand(object source, RepeaterCommandEventArgs e)
        {
            string command = e.CommandName.ToUpper();
            if (command == "DELETEFAULT")
            {
                int selectedIndex = e.Item.ItemIndex;
                CurrentProfileFaultList.RemoveAt(selectedIndex);
            }
            else
            {
                int selectedIndex = e.Item.ItemIndex;
                int newIndex = selectedIndex;
                switch (command)
                {
                    case "MOVEDOWN":
                        newIndex++;
                        break;
                    case "MOVEUP":
                        newIndex--;
                        break;
                }
                if (selectedIndex != newIndex && newIndex > -1 && newIndex < CurrentProfileFaultList.Count)
                {   // Only continue if the indexes look right/are within bounds
                    // First ensure that we persist any text changes user made to the fault list
                    updateFaultListDataSource();
                    // Just swap the ordernumber values from objects at selectedIndex and newIndex and rebind the grid...
                    int oldOrderNumber = CurrentProfileFaultList[selectedIndex].OrderNumber;
                    CurrentProfileFaultList[selectedIndex].OrderNumber = CurrentProfileFaultList[newIndex].OrderNumber;
                    CurrentProfileFaultList[newIndex].OrderNumber = oldOrderNumber;
                }
            }
            populateProfileFaultList();
        }

        private const int FAULT_ORDER_NUMBER_INCREMENT = 10;
        private void btnAddNewFault_Click(object sender, EventArgs e)
        {
            // First ensure that we persist any text changes user made to the fault list
            updateFaultListDataSource();
            // Work out what the next order value will be (remember there may not be any (for a new profile), in which case use the increment to start us off)
            int newOrderValue = FAULT_ORDER_NUMBER_INCREMENT;
            if (CurrentProfile != null && CurrentProfileFaultList.Count > 0) newOrderValue = CurrentProfileFaultList.Count + FAULT_ORDER_NUMBER_INCREMENT;
            Fleetwood.BlueSphere.BusinessLogic.FaultType newFault = new Fleetwood.BlueSphere.BusinessLogic.FaultType();
            newFault.OrderNumber = newOrderValue;
            CurrentProfileFaultList.Add(newFault);
            populateProfileFaultList();
        }

        /// <summary>
        /// Validates that we have the minimum # of faults for a profile (ie. at least 1)
        /// </summary>
        protected void atLeastOneFault_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            e.IsValid = (CurrentProfileFaultList.Count > 0);
            //atLeastOneFaultValidator.Visible = !e.IsValid;
        }

        /// <summary>
        /// On save click - beware some funky logic included here if user is trying to UPDATE an existing profile...
        /// </summary>
        private void btnSaveChanges_Click(object sender, EventArgs e)
        {
            updateProfileDataSource();
            updateFaultListDataSource();

            Page.Validate("MinimumProfileDetails");
            if (Page.IsValid)
            {
                try
                {
                    // get the currently Assigned vehicles (if any)
                    Fleetwood.BlueSphere.BusinessLogic.VehicleList assignedVehicles = CurrentProfile.VehicleList;
                    
                    //when profiels are updated this actually creates a new one
                    Guid newProfileID = CurrentProfile.Update();
                    CurrentProfile = new Fleetwood.BlueSphere.BusinessLogic.SafetyCheckProfile(newProfileID);
                    foreach (Fleetwood.BlueSphere.BusinessLogic.FaultType item in CurrentProfileFaultList)
                    {
                        if (item.SafetyCheckProfileID != new Guid("00000000-0000-0000-0000-000000000000"))
                        {
                            Fleetwood.BlueSphere.BusinessLogic.FaultType clonedFault = item.Clone();
                            clonedFault.SafetyCheckProfileID = CurrentProfile.ID;
                            clonedFault.IsDeleted = false;
                            clonedFault.Update();
                        }
                        else
                        {
                            item.SafetyCheckProfileID = CurrentProfile.ID;
                            item.Update();
                        }
                   }

                    // Now we need to re-ssign any vehicles that had the old Id to the new one.
                    ArrayList assignments = new ArrayList();
                    foreach (Fleetwood.BlueSphere.BusinessLogic.Vehicle v in assignedVehicles)
                        assignments.Add(v.ID);

                    CurrentProfile.AssignVehicles(assignments);

                    saveProfileError.Visible = false;
                    Response.Redirect(Request.RawUrl);
                }
                catch (Exception ex)
                {
                    saveProfileError.Visible = true;
                    saveProfileError.InnerText = ex.Message;
                }
            }
                
        }
        #endregion

        #region == Assign Safety Profile En-mass ==


        void chkShowAllVehicles_CheckedChanged(object sender, EventArgs e)
        {
            populateAssignProfileFields();
        }

        private void populateAssignProfileFields()
        {
            Dictionary<string, string> assignedVehicleList = new Dictionary<string, string>();
            Fleetwood.BlueSphere.BusinessLogic.VehicleList assignedVehicleListWithUnavailable = CurrentProfile.VehicleList;

            foreach (Fleetwood.BlueSphere.BusinessLogic.Vehicle v in assignedVehicleListWithUnavailable)
            {
                if (v.IsRetired || v.IsDeleted)
                    continue;

                assignedVehicleList.Add(v.DisplayRegistration, v.ID.ToString());
            }

            refreshRadListBoxContents(
                 assignedVehiclesListBox
                , false
                , assignedVehicleList
            );

            Dictionary<string, string> unassignedVehicleList = new Dictionary<string, string>();

            foreach (Fleetwood.BlueSphere.BusinessLogic.Vehicle v in BSCustomer.VehicleList)
            {
                if (v.IsRetired || v.IsDeleted || v.SafetyCheckProfileID == CurrentProfileID)
                    continue;

                if (v.SafetyCheckProfileID == Guid.Empty)
                    unassignedVehicleList.Add(v.DisplayRegistration, v.ID.ToString());
                else
                {
                    if (chkShowAllVehicles.Checked)
                        unassignedVehicleList.Add(v.DisplayRegistration + " (*)", v.ID.ToString());
                }
            }

            refreshRadListBoxContents(
                 unassignedVehiclesListBox
                , false
                , unassignedVehicleList
            );

            lblAssignedCount.Text = assignedVehicleList.Count.ToString();
            lblUnassignedCount.Text = unassignedVehicleList.Count.ToString();
        }
        
        private void refreshRadListBoxContents(RadListBox targetRadListBox, bool persistSelections, Dictionary<string, string> sourceVehicles)
        {
            IList<RadListBoxItem> selectedItems = null;
            if (persistSelections)
            {
                selectedItems = targetRadListBox.SelectedItems;
            }
            targetRadListBox.Items.Clear();
            foreach (KeyValuePair<string, string> vehicle in sourceVehicles)
            {
                targetRadListBox.Items.Add(new RadListBoxItem(vehicle.Key, vehicle.Value));
            }
            if (persistSelections)
            {
                foreach (RadListBoxItem oldItem in selectedItems)
                {
                    if(targetRadListBox.Items.Contains(oldItem)) targetRadListBox.SelectedItems.Add(oldItem);
                }
            }
        }

        private void btnSaveAssignments_Click(object sender, EventArgs e)
        {
            assignProfileError.Visible = false;
            try
            {
                ArrayList reassignmentList = new ArrayList();
                foreach (RadListBoxItem item in assignedVehiclesListBox.Items)
                {
                    reassignmentList.Add(new Guid(item.Value));
                }

                CurrentProfile.AssignVehicles(reassignmentList);

                foreach (Fleetwood.BlueSphere.BusinessLogic.Vehicle v in BSCustomer.VehicleList)
                {
                    if (v.SafetyCheckProfileID == CurrentProfileID)
                        if (!reassignmentList.Contains(v.ID))
                        {
                            v.SafetyCheckProfileID = Guid.Empty;
                            v.Update();
                        }
                }
                Response.Redirect(Request.RawUrl);
            }

            catch (Exception ex)
            {
                assignProfileError.InnerText = ex.Message;
                assignProfileError.Visible = true;
            }

        }

        #endregion

    }
}