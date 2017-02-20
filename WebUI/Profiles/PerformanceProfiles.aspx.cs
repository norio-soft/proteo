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
    public partial class PerformanceProfile : System.Web.UI.Page
    {
        #region Page Load/Init

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage, eSystemPortion.AddEditJob, eSystemPortion.Plan, eSystemPortion.AddEditPoints);
            btnAddNew.Enabled = Orchestrator.WebUI.Security.Authorise.CanAccess(eSystemPortion.AddEditJob, eSystemPortion.Plan, eSystemPortion.AddEditPoints);

            this.rgProfiles.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(rgProfiles_NeedDataSource);
            this.rgProfiles.ItemCommand += new Telerik.Web.UI.GridCommandEventHandler(rgProfiles_ItemCommand);
            this.btnCancelVehicles.Click += new EventHandler(btnCancelVehicles_Click);
            this.btnSaveVehicles.Click += new EventHandler(btnSaveVehicles_Click);
            this.btnSave.Click += new EventHandler(btnSave_Click);
            this.btnCancel.Click += new EventHandler(btnCancel_Click);
            this.btnAddNew.Click += new EventHandler(btnAddNew_Click);
            this.btnCopy.Click += new EventHandler(btnCopy_Click);
        }

        #endregion

        #region Properties

        ProfilesService.ProfilesClient _serviceClient = null;
        private ProfilesService.ProfilesClient ServiceClient
        {
            get
            {
                if (_serviceClient == null)
                    _serviceClient = new ProfilesService.ProfilesClient();

                return _serviceClient;
            }
        }

        ProfilesService.PerformanceProfile Profile
        {
            get
            {
                ProfilesService.PerformanceProfile _performanceProfile = null;
                if (this.ViewState["__profile"] != null)
                    _performanceProfile = (ProfilesService.PerformanceProfile)this.ViewState["__profile"];

                return _performanceProfile;
                    
            }
            set{
                this.ViewState["__profile"] = value;
            }
        }

        #endregion

        #region Grid Events
        void rgProfiles_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
#if DEBUG
            List<ProfilesService.PerformanceProfile> profiles = this.ServiceClient.GetPerformanceProfiles(new Guid("BEA0F9A9-E630-4D04-851B-36BD1D97EEEB"));
#else
            List<ProfilesService.PerformanceProfile> profiles = this.ServiceClient.GetPerformanceProfiles(new Guid(Configuration.BlueSphereCustomerId));
#endif
            this.rgProfiles.DataSource = profiles;
        }

        void rgProfiles_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            if (e.CommandName == "Edit")
            {
                e.Canceled = true;
                this.rmpProfiles.SelectedIndex = 1;
                this.Profile = this.ServiceClient.GetPerformanceProfile(new Guid(e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["ProfileID"].ToString()));
                ShowProfile();
            }

            if (e.CommandName == "Vehicles")
            {
               e.Canceled = true;
               this.Profile = this.ServiceClient.GetPerformanceProfile(new Guid(e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["ProfileID"].ToString()));
                this.rmpProfiles.SelectedIndex = 2;
                ShowVehicles();
            }
        }
        #endregion

        #region Edit Profile

         void btnAddNew_Click(object sender, EventArgs e)
        {
            ClearDownTabs();

            this.Profile = new ProfilesService.PerformanceProfile();
            this.rmpProfiles.SelectedIndex = 1;
            rmpPerformance.SelectedIndex = 0;

            rgProfiles.Rebind();
        }

        private void ShowProfile()
        {
            this.txtTitle.Text = this.Profile.Title;
            this.txtMake.Text = this.Profile.Make;
            this.txtModel.Text = this.Profile.Model;
            this.txtFreeStoppingTime.Value = (double)this.Profile.FreeStoppingTime;
            this.txtFreeIdlingTime.Value = (double)this.Profile.FreeIdlingTime;
            this.txtDecelerationCutOff.Value = (double)this.Profile.DecelerationCutOff;
            this.txtDecelerationWarning.Value = (double)this.Profile.DecelerationWarning;
            this.txtEconomyBandEnd.Value = (double)this.Profile.EconomyBandEnd;
            this.txtEconomyBandStart.Value = (double)this.Profile.EconomyBandStart;
            this.txtFreeSpeedingTime.Value = (double)this.Profile.FreeSpeedingTime;
            this.txtOverRevLimit.Value = (double)this.Profile.OverRevLimit;
            this.txtOverRevWarning.Value = (double)this.Profile.OverRevWarning;
            this.txtSpeedingLimit.Value = (double)this.Profile.SpeedingLimit;
            this.txtPurpose.Text = this.Profile.Purpose;
            this.chkIsDefault.Checked = this.Profile.IsDefault;

            #region Rev Bands
            txtRevBand1.Value = (double)this.Profile.RevBands[0];
            txtRevBand2.Value = (double)this.Profile.RevBands[1];
            txtRevBand3.Value = (double)this.Profile.RevBands[2];
            txtRevBand4.Value = (double)this.Profile.RevBands[3];
            txtRevBand5.Value = (double)this.Profile.RevBands[4];
            txtRevBand6.Value = (double)this.Profile.RevBands[5];
            txtRevBand7.Value = (double)this.Profile.RevBands[6];
            txtRevBand8.Value = (double)this.Profile.RevBands[7];
            txtRevBand9.Value = (double)this.Profile.RevBands[8];
            txtRevBand10.Value = (double)this.Profile.RevBands[9];
            #endregion

            #region Speed Bands
            txtSpeedBand1.Value = (double)this.Profile.SpeedBands[0];
            txtSpeedBand2.Value = (double)this.Profile.SpeedBands[1];
            txtSpeedBand3.Value = (double)this.Profile.SpeedBands[2];
            txtSpeedBand4.Value = (double)this.Profile.SpeedBands[3];
            txtSpeedBand5.Value = (double)this.Profile.SpeedBands[4];
            txtSpeedBand6.Value = (double)this.Profile.SpeedBands[5];
            txtSpeedBand7.Value = (double)this.Profile.SpeedBands[6];
            txtSpeedBand8.Value = (double)this.Profile.SpeedBands[7];
            txtSpeedBand9.Value = (double)this.Profile.SpeedBands[8];
            txtSpeedBand10.Value = (double)this.Profile.SpeedBands[9];
            #endregion

            rmpPerformance.SelectedIndex = 0;
        }
       
        void btnCancel_Click(object sender, EventArgs e)
        {
            this.rmpProfiles.SelectedIndex = 0;
        }

        void btnSave_Click(object sender, EventArgs e)
        {
            UpdateProfile();
            this.rmpProfiles.SelectedIndex = 0;
            this.rgProfiles.Rebind();
        }

        private void UpdateProfile()
        {
            this.Profile.Title = txtTitle.Text;
            this.Profile.Make = txtMake.Text;
            this.Profile.Model = txtModel.Text;
            this.Profile.Purpose = txtPurpose.Text;
            this.Profile.FreeStoppingTime = (decimal)this.txtFreeStoppingTime.Value;
            this.Profile.FreeSpeedingTime = (decimal)this.txtFreeSpeedingTime.Value;
            this.Profile.FreeIdlingTime = (decimal)this.txtFreeIdlingTime.Value;
            this.Profile.DecelerationCutOff = (decimal)this.txtDecelerationCutOff.Value;
            this.Profile.DecelerationWarning= (decimal)this.txtDecelerationWarning.Value;
            this.Profile.EconomyBandStart= (decimal)this.txtEconomyBandStart.Value;
            this.Profile.EconomyBandEnd= (decimal)this.txtEconomyBandEnd.Value;
            this.Profile.OverRevWarning= (decimal)this.txtOverRevWarning.Value;
            this.Profile.OverRevLimit= (decimal)this.txtOverRevLimit.Value;
            this.Profile.SpeedingLimit= (decimal)this.txtSpeedingLimit.Value;
            this.Profile.IsDefault = this.chkIsDefault.Checked;

            List<int> RevBands = new List<int>();
            RevBands.Add((int)txtRevBand1.Value);
            RevBands.Add((int)txtRevBand2.Value);
            RevBands.Add((int)txtRevBand3.Value);
            RevBands.Add((int)txtRevBand4.Value);
            RevBands.Add((int)txtRevBand5.Value);
            RevBands.Add((int)txtRevBand6.Value);
            RevBands.Add((int)txtRevBand7.Value);
            RevBands.Add((int)txtRevBand8.Value);
            RevBands.Add((int)txtRevBand9.Value);
            RevBands.Add((int)txtRevBand10.Value);
            this.Profile.RevBands = RevBands;

            List<int> SpeedBands = new List<int>();
            SpeedBands.Add((int)txtSpeedBand1.Value);
            SpeedBands.Add((int)txtSpeedBand2.Value);
            SpeedBands.Add((int)txtSpeedBand3.Value);
            SpeedBands.Add((int)txtSpeedBand4.Value);
            SpeedBands.Add((int)txtSpeedBand5.Value);
            SpeedBands.Add((int)txtSpeedBand6.Value);
            SpeedBands.Add((int)txtSpeedBand7.Value);
            SpeedBands.Add((int)txtSpeedBand8.Value);
            SpeedBands.Add((int)txtSpeedBand9.Value);
            SpeedBands.Add((int)txtSpeedBand10.Value);
            this.Profile.SpeedBands = SpeedBands;

#if DEBUG
            Guid customerID = new Guid("BEA0F9A9-E630-4D04-851B-36BD1D97EEEB");
#else
            Guid customerID = new Guid(Configuration.BlueSphereCustomerId);
#endif
            this.ServiceClient.ManagePerformanceProfile(this.Profile, customerID);
            
        }
        #endregion

        private void ClearDownTabs()
        {
            #region Performance

            txtTitle.Text = string.Empty;
            txtPurpose.Text = string.Empty;
            txtMake.Text = string.Empty;
            txtModel.Text = string.Empty;

            chkIsDefault.Checked = false;

            txtFreeStoppingTime.Text = string.Empty;
            txtFreeIdlingTime.Text = string.Empty;
            txtEconomyBandStart.Text = string.Empty;
            txtEconomyBandEnd.Text = string.Empty;

            txtFreeSpeedingTime.Text = string.Empty;
            txtSpeedingLimit.Text = string.Empty;
            txtOverRevWarning.Text = string.Empty;
            txtOverRevLimit.Text = string.Empty;

            txtDecelerationCutOff.Text = string.Empty;
            txtDecelerationWarning.Text = string.Empty;

            #endregion

            #region Rev Bands

            txtRevBand1.Text = string.Empty;
            txtRevBand2.Text = string.Empty;
            txtRevBand3.Text = string.Empty;
            txtRevBand4.Text = string.Empty;
            txtRevBand5.Text = string.Empty;
            txtRevBand6.Text = string.Empty;
            txtRevBand7.Text = string.Empty;
            txtRevBand8.Text = string.Empty;
            txtRevBand9.Text = string.Empty;
            txtRevBand10.Text = string.Empty;

            #endregion

            #region Speed Bands

            txtSpeedBand1.Text = string.Empty;
            txtSpeedBand2.Text = string.Empty;
            txtSpeedBand3.Text = string.Empty;
            txtSpeedBand4.Text = string.Empty;
            txtSpeedBand5.Text = string.Empty;
            txtSpeedBand6.Text = string.Empty;
            txtSpeedBand7.Text = string.Empty;
            txtSpeedBand8.Text = string.Empty;
            txtSpeedBand9.Text = string.Empty;
            txtSpeedBand10.Text = string.Empty;

            #endregion
        }

        private void ShowVehicles()
        {
            lstVehiclesInProfile.Items.Clear();
            lstVehicles.Items.Clear();

            // Get a list of the vehicles.
            Facade.IVehicle facVehicle = new Facade.Resource();
            DataSet dsVehicles = facVehicle.GetAllVehicles();

            foreach (DataRow row in dsVehicles.Tables[0].Rows)
            {
                if (string.IsNullOrEmpty(row["GPSUnitID"].ToString()))
                    continue;

                if (this.Profile.GPSUnits.Count > 0 &&  this.Profile.GPSUnits.FirstOrDefault(a => a == row["GPSUnitID"].ToString()) != null)
                    lstVehiclesInProfile.Items.Add(new Telerik.Web.UI.RadListBoxItem() { Text = row["RegNo"].ToString(), Value = row["GPSUnitID"].ToString() });
                else
                    lstVehicles.Items.Add(new Telerik.Web.UI.RadListBoxItem() { Text = row["RegNo"].ToString(), Value = row["GPSUnitID"].ToString() });
            }
        }

        void btnSaveVehicles_Click(object sender, EventArgs e)
        {
            // remove whatever is in the list.
            this.Profile.GPSUnits.Clear();

            // save the profile
            foreach (RadListBoxItem item in lstVehiclesInProfile.Items)
                if (!string.IsNullOrEmpty(item.Value))
                    this.Profile.GPSUnits.Add(item.Value);

            ProfilesService.ProfilesClient client = new ProfilesService.ProfilesClient();

#if DEBUG
            Guid customerID = new Guid("BEA0F9A9-E630-4D04-851B-36BD1D97EEEB");
#else
            Guid customerID = new Guid(Configuration.BlueSphereCustomerId);
#endif
            client.AssignVehiclesToPerformanceProfile(customerID, this.Profile.ProfileID, this.Profile.GPSUnits);
            this.rmpProfiles.SelectedIndex = 0;
            this.rgProfiles.Rebind();
        }

        void btnCancelVehicles_Click(object sender, EventArgs e)
        {
            this.rmpProfiles.SelectedIndex = 0;
        }

        void btnCopy_Click(object sender, EventArgs e)
        {
            #if DEBUG
            Guid customerID = new Guid("BEA0F9A9-E630-4D04-851B-36BD1D97EEEB");
#else
            Guid customerID = new Guid(Configuration.BlueSphereCustomerId);
#endif

            this.Profile.Title += " - Copy";
            this.Profile.ProfileID = Guid.Empty;
            this.ServiceClient.ManagePerformanceProfile(this.Profile, customerID);
            this.rmpProfiles.SelectedIndex = 0;
            this.rgProfiles.Rebind();
        }
    }
}