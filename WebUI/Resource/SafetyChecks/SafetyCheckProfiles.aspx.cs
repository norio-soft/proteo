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

namespace Orchestrator.WebUI.Resource.SafetyChecks
{

    /// <summary>
    /// Explanation of logic on this page; it's rather non-standard as a result of how Safety Profiles MUST work
    /// TODO: Clean up these notes before final commit to GIT
    /// The page has three possible actions the user can invoke, which are detailed below.
    /// Initially, the page will load on the active profile list. From here, users can:
    /// 1: Delete a profile
    /// 2: Add a profile
    /// 3: Edit a profile
    /// </summary>

    public partial class Profiles : System.Web.UI.Page
    {
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

        private List<SafetyCheckProfile> ProfileList
        {
            get
            {
                List<SafetyCheckProfile> currentProfiles = null;
                if (ViewState["__safetycheckprofilelist"] != null) currentProfiles = (List<SafetyCheckProfile>)ViewState["__safetycheckprofilelist"];
                return currentProfiles;
            }
            set
            {
                ViewState["__safetycheckprofilelist"] = value;
            }
        }
        private SafetyCheckProfile CurrentProfile
        {
            get
            {
                SafetyCheckProfile currentProfile = null;
                if (ViewState["__safetycheckprofile"] != null) currentProfile = (SafetyCheckProfile)ViewState["__safetycheckprofile"];
                return currentProfile;
            }
            set
            {
                ViewState["__safetycheckprofile"] = value;
            }
        }
        private List<SafetyCheckProfileFault> CurrentProfileFaultList
        {
            get
            {
                List<SafetyCheckProfileFault> profileFaults = null;
                if (ViewState["__safetycheckprofilefaultlist"] != null) profileFaults = (List<SafetyCheckProfileFault>)ViewState["__safetycheckprofilefaultlist"];
                return profileFaults;
            }
            set
            {
                ViewState["__safetycheckprofilefaultlist"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
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

            // TODO: Attach more events ?

            if (!IsPostBack)
            {   // On initial load make sure objects reflect a new profile
                CurrentProfile = new SafetyCheckProfile();
                CurrentProfileFaultList = new List<SafetyCheckProfileFault>();
                AssigningProfile = null;
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
                    populateAssignProfileFields(false);
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
            Guid customerId = new Guid(Configuration.BlueSphereCustomerId);
            ProfileList = (List<SafetyCheckProfile>)MobileWorkerFlow.MWFServicesCommunication.Client.GetActiveSafetyCheckProfileList(customerId);
            rgProfiles.DataSource = ProfileList;
        }

        private void rgProfiles_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            string command = e.CommandName.ToUpper();
            if (e.Item.ItemIndex > -1)
            {
                SafetyCheckProfile selectedRow = ProfileList[e.Item.ItemIndex];
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
            CurrentProfile = new SafetyCheckProfile();
            CurrentProfileFaultList = new List<SafetyCheckProfileFault>();
            btnSaveChanges.Text = "Save New Profile";
            btnCancel.Text = "Cancel";
            callback();
        }

        private void editProfile(SafetyCheckProfile targetProfile, Action callback)
        {
            CurrentProfile = targetProfile;
            CurrentProfileFaultList = MobileWorkerFlow.MWFServicesCommunication.Client.GetSafetyCheckProfileFaultList(new Guid(Configuration.BlueSphereCustomerId), CurrentProfile.ProfileId);
            btnSaveChanges.Text = "Save Changes";
            btnCancel.Text = "Cancel Changes";
            callback();
        }

        private void deleteProfile(SafetyCheckProfile targetProfile, Action callback)
        {

            bool profileWasDeleted = MobileWorkerFlow.MWFServicesCommunication.Client.DeleteSafetyCheckProfile(new Guid(Configuration.BlueSphereCustomerId), targetProfile.ProfileId);
            deleteProfileError.Visible = !profileWasDeleted;
            callback();
        }

        private void assignProfile(SafetyCheckProfile targetProfile, Action callback)
        {
            AssigningProfile = targetProfile;
            lblAssigningProfileTitle.InnerText = AssigningProfile.ProfileTitle;
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
            txtProfileName.Text = CurrentProfile.ProfileTitle;
            chkOdometer.Checked = CurrentProfile.Odometer;
            chkSignature.Checked = CurrentProfile.Signature;
            chkAtLogon.Checked = CurrentProfile.AtLogon;
            chkAtLogoff.Checked = CurrentProfile.AtLogoff;
            // How annoying; label for this field means we have to negate the DB Field :@
            chkVOSACompliant.Checked = !CurrentProfile.IsVOSACompliant;
            populateProfileFaultList();
        }

        /// <summary>
        /// Rebinds the fault list table to the fault list for the current profile
        /// </summary>
        private void populateProfileFaultList()
        {
            // Resort the profile list here so we pick up any reordering the user may have performed.
            CurrentProfileFaultList.Sort(delegate(SafetyCheckProfileFault f1, SafetyCheckProfileFault f2) { return f1.OrderNumber.CompareTo(f2.OrderNumber); });
            repeaterFaultList.DataSource = CurrentProfileFaultList;
            repeaterFaultList.DataBind();
        }

        /// <summary>
        /// Ensures the CurrentProfile reflects the data entered into the fields on screen
        /// </summary>
        private void updateProfileDataSource()
        {
            CurrentProfile.ProfileTitle = txtProfileName.Text.Trim();
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
                SafetyCheckProfileFault thisFault = CurrentProfileFaultList[i];
                foreach (RepeaterItem item in repeaterFaultList.Items)
                {
                    if (item.ItemIndex == i)
                    {
                        TextBox txtFaultTitle = (TextBox)item.FindControl("txtFaultTitle");
                        thisFault.FaultTitle = txtFaultTitle.Text;
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
            if (CurrentProfileFaultList.Any()) newOrderValue = CurrentProfileFaultList.Max(r => r.OrderNumber) + FAULT_ORDER_NUMBER_INCREMENT;
            SafetyCheckProfileFault newFault = new SafetyCheckProfileFault();
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
                bool savingBrandNewProfile = CurrentProfile.ProfileId.Equals(Guid.Empty);
                MWFDataResourceReturnResult saveResult = MobileWorkerFlow.MWFServicesCommunication.Client.UnknownResult;
                Guid customerId = new Guid(Configuration.BlueSphereCustomerId);
                if (savingBrandNewProfile)
                {
                    // 1. For a new profile, just insert everything
                    saveResult = MobileWorkerFlow.MWFServicesCommunication.Client.AddSafetyCheckProfileAndFaults(
                         customerId
                        ,CurrentProfile
                        ,CurrentProfileFaultList
                    );
                }
                else
                {
                    // 1. Clones the existing profile to a new one (NOT inlcuding the fault list) and reassigns any active vehicles to the newly cloned profile
                    // If the profile has been pulled from the search results it will have the string " (Active)" appended to the end of the profile title - don't write this to the new profile.
                    CurrentProfile.ProfileTitle = CurrentProfile.ProfileTitle.Replace(" (Active)", string.Empty);
                    saveResult = MobileWorkerFlow.MWFServicesCommunication.Client.UpdateSafetyCheckProfile(customerId, CurrentProfile);
                    if (saveResult.Success)
                    {
                        // 2. Clone success - now (soft) delete the source profile and associted fault list
                        bool oldProfileDeleted = MobileWorkerFlow.MWFServicesCommunication.Client.DeleteSafetyCheckProfile(customerId, CurrentProfile.ProfileId);
                        // 3. Finally, insert the fault list into the newly cloned profile
                        saveResult = MobileWorkerFlow.MWFServicesCommunication.Client.AddFaultTypeListToProfile(
                             customerId
                            ,CurrentProfileFaultList
                            ,saveResult.ResourceGuid
                        );
                    }
                }
                
                if (!saveResult.Success)
                {   // Something went wrong:
#if DEBUG
                    // Some more useful details if youre debugging
                    saveProfileErrorMessage.InnerText = saveResult.ErrorMessage + "\r\nEXCEPTION MESSAGE: " + saveResult.Exception.Message + "\r\nSTACKTRACE: " + saveResult.Exception.StackTrace;
#else
                    // End users will get a more generic error so we don't expose any of the codebase
                    saveProfileErrorMessage.InnerText = "An error was encountered while saving the profile, please try again later. If the problem persists, please contact the support desk.";
#endif
                }
                saveProfileError.Visible = !saveResult.Success;

                if (saveResult.Success)
                {   // If everything worked, invoke a GET so if user F5s we dont double submit
                    Response.Redirect(Request.RawUrl);
                }
            }
        }
        #endregion

        #region == Assign Safety Profile En-mass ==

        private SafetyCheckProfile AssigningProfile
        {
            get
            {
                SafetyCheckProfile assigningProfile = null;
                if (ViewState["__assigningsafetycheckprofile"] != null) assigningProfile = (SafetyCheckProfile)ViewState["__assigningsafetycheckprofile"];
                return assigningProfile;
            }
            set
            {
                ViewState["__assigningsafetycheckprofile"] = value;
            }
        }
        private Dictionary<string, Guid> AssignedVehicles
        {
            get
            {
                Dictionary<string, Guid> assignedList = null;
                if (ViewState["__assignedvehicleslist"] != null) assignedList = (Dictionary<string, Guid>)ViewState["__assignedvehicleslist"];
                return assignedList;
            }
            set
            {
                ViewState["__assignedvehicleslist"] = value;
            }
        }
        private Dictionary<string, Guid> UnassignedVehicles
        {
            get
            {
                Dictionary<string, Guid> unassignedList = null;
                if (ViewState["__unassignedvehicleslist"] != null) unassignedList = (Dictionary<string, Guid>)ViewState["__unassignedvehicleslist"];
                return unassignedList;
            }
            set
            {
                ViewState["__unassignedvehicleslist"] = value;
            }
        }

        private void populateAssignProfileFields(bool persistSelections)
        {
            refreshAssignedVehiclesList(persistSelections);
            refreshUnassignedVehiclesList(persistSelections);
        }

        private void refreshAssignedVehiclesList(bool persistSelections)
        {
            if (AssignedVehicles == null)
            {
                AssignedVehicles = MobileWorkerFlow.MWFServicesCommunication.Client.GetVehiclesAssignedToProfile(new Guid(Configuration.BlueSphereCustomerId), AssigningProfile.ProfileId, true, true, false);
            }
            refreshRadListBoxContents(
                 assignedVehiclesListBox
                ,persistSelections
                ,AssignedVehicles
            );
        }
        private void refreshUnassignedVehiclesList(bool persistSelections)
        {
            if (UnassignedVehicles == null)
            {
                UnassignedVehicles = MobileWorkerFlow.MWFServicesCommunication.Client.GetVehiclesAssignedToProfile(new Guid(Configuration.BlueSphereCustomerId), Guid.Empty, true, true, false);
            }
            refreshRadListBoxContents(
                 unassignedVehiclesListBox
                ,persistSelections
                ,UnassignedVehicles
            );
        }
        private void refreshRadListBoxContents(RadListBox targetRadListBox, bool persistSelections, Dictionary<string, Guid> sourceVehicles)
        {
            IList<RadListBoxItem> selectedItems = null;
            if (persistSelections)
            {
                selectedItems = targetRadListBox.SelectedItems;
            }
            targetRadListBox.Items.Clear();
            foreach (KeyValuePair<string, Guid> kvp in sourceVehicles)
            {
                targetRadListBox.Items.Add(new RadListBoxItem(kvp.Key, kvp.Value.ToString()));
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
            List<Guid> reassignmentList = new List<Guid>();
            foreach (RadListBoxItem item in assignedVehiclesListBox.Items)
            {
                reassignmentList.Add(new Guid(item.Value));
            }

            Guid customerId = new Guid(Configuration.BlueSphereCustomerId);
            bool reassignedResult = MobileWorkerFlow.MWFServicesCommunication.Client.SetVehiclesSafetyCheckProfileAssignment(customerId, AssigningProfile.ProfileId, reassignmentList);

            assignProfileError.Visible = !reassignedResult;
            if (!reassignedResult)
            {
                gotoPanel(SafetyCheckProfileAdminPages.AssignProfileToVehicles);
            } 
            else 
            {
                Response.Redirect(Request.RawUrl);
            }
        }

        #endregion

    }
}