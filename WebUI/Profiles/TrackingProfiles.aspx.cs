using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Orchestrator.Globals;
using System.Data;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.Profiles
{
    public partial class TrackingProfiles : System.Web.UI.Page
    {

        #region constants
        private DateTime BaseDate = new DateTime(2011, 05,15);
        #endregion

        #region Properties
        protected ProfilesService.TrackingProfile Profile
        {
            get
            {
                ProfilesService.TrackingProfile profile = new ProfilesService.TrackingProfile();
                if (this.ViewState["__profile"] != null)
                    profile = (ProfilesService.TrackingProfile)this.ViewState["__profile"];

                return profile;
            }

            set
            {
                this.ViewState["__profile"] = value;
            }
        }
        #endregion

        #region Page Load/Init
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["profileID"] != null)
                {
                    //Load the profile
                    ProfilesService.ProfilesClient client = new ProfilesService.ProfilesClient();
                    this.Profile = client.GetLocationProfile(new Guid(Request.QueryString["profileID"]));
                    ShowProfile();
                }
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GPSProfileManagement);

            this.rgProfiles.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(rgProfiles_NeedDataSource);
            this.rgProfiles.ItemCommand += new Telerik.Web.UI.GridCommandEventHandler(rgProfiles_ItemCommand);
            this.btnSave.Click += new EventHandler(btnSave_Click);
            this.btnCancel.Click += new EventHandler(btnCancel_Click);
            this.btnAdd.Click += new EventHandler(btnAdd_Click);
            this.btnSaveVehicles.Click += new EventHandler(btnSaveVehicles_Click);
            this.btnCancelVehicles.Click += new EventHandler(btnCancelVehicles_Click);
        }

                
        #endregion

        #region Private Methods
        private void ShowProfile()
        {
            this.txtTitle.Text = this.Profile.Title;
            this.rtpActiveFrequency.SelectedDate = BaseDate.AddSeconds((int)this.Profile.TimeFrequency);
            this.rtpIdleReportTime.SelectedDate = BaseDate.AddSeconds((int)this.Profile.IdlingFrequency);
            this.rtpSleepFrequency.SelectedDate = BaseDate.AddSeconds((int)this.Profile.SleepingFrequency);
            this.rtpSendFrequency.SelectedDate = BaseDate.AddSeconds((int)this.Profile.SendFrequency);
            this.rntDirectionAngle.Value = (double)this.Profile.DirectionAngle;
            this.rntDistance.Value = (double)this.Profile.DistanceFrequency;
            this.chkReportOnEvents.Checked = this.Profile.ReportOnEvents;
            this.chkReportOnCANEvents.Checked = this.Profile.ReportOnCANEvents;
            this.chkIsDefault.Checked = this.Profile.IsDefault;
        }

        private void UpdateProfile()
        {
            
            Profile.Title = this.txtTitle.Text;
            Profile.TimeFrequency = (decimal)(BaseDate.Add(this.rtpActiveFrequency.SelectedDate.Value.TimeOfDay) - BaseDate).TotalSeconds;
            Profile.IdlingFrequency = (decimal)(BaseDate.Add(this.rtpIdleReportTime.SelectedDate.Value.TimeOfDay) - BaseDate).TotalSeconds;
            Profile.SleepingFrequency = (decimal)(BaseDate.Add(this.rtpSleepFrequency.SelectedDate.Value.TimeOfDay) - BaseDate).TotalSeconds;
            Profile.SendFrequency = (decimal)(BaseDate.Add(this.rtpSendFrequency.SelectedDate.Value.TimeOfDay) - BaseDate).TotalSeconds;
            Profile.DirectionAngle = (decimal)this.rntDirectionAngle.Value;
            Profile.DistanceFrequency = (decimal)this.rntDistance.Value;
            Profile.ReportOnEvents = chkReportOnEvents.Checked;
            Profile.ReportOnCANEvents = chkReportOnCANEvents.Checked;
            this.Profile.IsDefault = chkIsDefault.Checked;
            
            ProfilesService.ProfilesClient client = new ProfilesService.ProfilesClient();
#if DEBUG
            bool updateResult = client.ManageLocationProfile(Profile, new Guid("BEA0F9A9-E630-4D04-851B-36BD1D97EEEB"));
#else
            bool updateResult = client.ManageLocationProfile(this.Profile, new Guid(Configuration.BlueSphereCustomerId));
#endif


            if (updateResult)
            {
                pnlInfo.Visible = true;
                if (this.Profile.ProfileID == Guid.Empty)
                    lblMessage.Text = "The Profile has been added.";
                else
                    lblMessage.Text = "Your changes have been saved.";
            }
        }

        private void ShowVehicles()
        {
            lstVehiclesInProfile.Items.Clear();
            lstVehicles.Items.Clear();
            // Get a list of the vehicles.
            Facade.IVehicle facVehicle = new Facade.Resource();
            DataSet dsVehicles = facVehicle.GetAllVehicles();

            foreach (DataRow row in dsVehicles.Tables[0].Rows)
                if (row["GPSUnitID"] != DBNull.Value && !String.IsNullOrEmpty(row["GPSUnitID"].ToString()) && this.Profile.GPSUnits.Exists(a => a == row["GPSUnitID"].ToString()))
                    lstVehiclesInProfile.Items.Add(new Telerik.Web.UI.RadListBoxItem() { Text = row["RegNo"].ToString(), Value = row["GPSUnitID"].ToString() });
                else
                    lstVehicles.Items.Add(new Telerik.Web.UI.RadListBoxItem() { Text = row["RegNo"].ToString(), Value = row["GPSUnitID"].ToString() });
        }
        #endregion

        #region Grid Event Handlers

        void rgProfiles_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            if (e.CommandName == "Edit")
            {
                e.Canceled = true;
                // go to the the edit display
                this.mpProfile.SelectedIndex = 1;
                
                // Load the Selected Profile
                ProfilesService.ProfilesClient client = new ProfilesService.ProfilesClient();
                this.Profile = client.GetLocationProfile(new Guid(e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["ProfileID"].ToString()));
                ShowProfile();
            }

            if (e.CommandName == "Vehicles")
            {
                e.Canceled = true;
                this.mpProfile.SelectedIndex = 2;
                ProfilesService.ProfilesClient client = new ProfilesService.ProfilesClient();
                this.Profile = client.GetLocationProfile(new Guid(e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["ProfileID"].ToString()));
                ShowVehicles();
            }
            
        }

        void rgProfiles_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            ProfilesService.ProfilesClient client = new ProfilesService.ProfilesClient();
#if DEBUG
            List<ProfilesService.TrackingProfile> profiles = client.GetLocationProfiles(new Guid("BEA0F9A9-E630-4D04-851B-36BD1D97EEEB"));
#else
            List<ProfilesService.TrackingProfile> profiles = client.GetLocationProfiles(new Guid(Configuration.BlueSphereCustomerId));
#endif

            rgProfiles.DataSource = profiles.Where(p=> p.IsDeleted == false);
        }

        #endregion

        #region Button Event Handlers
        void btnAdd_Click(object sender, EventArgs e)
        {
            this.mpProfile.SelectedIndex = 1;
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            this.mpProfile.SelectedIndex = 0;
        }

        void btnSave_Click(object sender, EventArgs e)
        {
            //Popualte and save the Location Tracking Profile
            if (Page.IsValid)
            {
                UpdateProfile();
            }

            this.mpProfile.SelectedIndex = 0;
            rgProfiles.Rebind();
        }

        void btnCancelVehicles_Click(object sender, EventArgs e)
        {
            // go back to the list - we don't need to rebind as we haven't changed anything
            this.mpProfile.SelectedIndex = 0;
        }

        void btnSaveVehicles_Click(object sender, EventArgs e)
        {
            // remove whatever is in the list.
            this.Profile.GPSUnits.Clear();

            // save the profile
            foreach (RadListBoxItem item in lstVehiclesInProfile.Items)
                this.Profile.GPSUnits.Add(item.Value);

            ProfilesService.ProfilesClient client = new ProfilesService.ProfilesClient();
            
#if DEBUG
            Guid customerID =  new Guid("BEA0F9A9-E630-4D04-851B-36BD1D97EEEB");
#else
            Guid customerID = new Guid(Configuration.BlueSphereCustomerId);
#endif
            client.AssignVehiclesToProfile(customerID, this.Profile.ProfileID, this.Profile.GPSUnits);
            this.mpProfile.SelectedIndex = 0;
            rgProfiles.Rebind();
        }


        #endregion
    }
}