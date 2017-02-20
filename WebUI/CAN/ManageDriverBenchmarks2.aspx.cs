using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.CAN
{
    public partial class ManageDriverBenchmarks2 : System.Web.UI.Page
    {
        private int CurrentBenchmarkID
        {
            get
            {
                int currentBenchmarkID = 0;
                if (ViewState["__currentBenchmarkID"] != null) currentBenchmarkID = (int)ViewState["__currentBenchmarkID"];
                return currentBenchmarkID;
            }
            set
            {
                ViewState["__currentBenchmarkID"] = value;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.rgBenchmarks.NeedDataSource += rgBenchmarks_NeedDataSource;
            this.rgBenchmarks.ItemCommand += rgBenchmarks_ItemCommand;
            this.btnAddNew.Click +=btnAddNew_Click;
            this.btnSaveChanges.Click +=btnSaveChanges_Click;
            this.btnCancel.Click += btnCancel_Click;
            this.btnCancelAssignments.Click += btnCancel_Click;
            this.btnSaveAssignments.Click += btnSaveAssignments_Click;
        }

        
        void btnCancel_Click(object sender, EventArgs e)
        {
            // Cheeky; clear everything down and invoke the first postback again. Seems valid to me...
            Response.Redirect(Request.RawUrl);
        }

        void rgBenchmarks_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            string command = e.CommandName.ToUpper();



            var argument = (int)(e.Item as GridDataItem).GetDataKeyValue("DriverGradingBenchmarkID");
            if (e.Item.ItemIndex > -1)
            {

                switch (command)
                {
                    case "EDITBENCHMARK":
                        // Load up the selected profile details then go to the add/edit page
                        editBenchmark(argument, () => gotoPanel(BenchmarkAdminPages.AddOrEditBenchmark));
                        break;

                    case "DELETEBENCHMARK":
                        // Delete the profile, then reload the profile list
                        deleteBenchmark(argument, () => gotoPanel(BenchmarkAdminPages.Listbenchmarks));
                        break;

                    case "ASSIGNDRIVERS":
                        // Load up the selected profile details then go to the add/edit page
                        assignBenchmark(argument, () => gotoPanel(BenchmarkAdminPages.AssignBenchmarktoDrivers));
                        break;
                }
            }
        }
        private enum BenchmarkAdminPages
        {
            Listbenchmarks = 0,
            AddOrEditBenchmark = 1, 
            AssignBenchmarktoDrivers= 2
        }

        private BenchmarkAdminPages activePanel;

        private void gotoPanel(BenchmarkAdminPages targetPanel)
        {
            activePanel = targetPanel;
            int activePanellIndex = (int)activePanel;
            switch (targetPanel)
            {
                case BenchmarkAdminPages.AssignBenchmarktoDrivers:
                    rmpBenchmarks.SelectedIndex = activePanellIndex;
                    break;

                case BenchmarkAdminPages.AddOrEditBenchmark:
                    deleteBenchmarkError.Visible = false;
                    rmpBenchmarks.SelectedIndex = activePanellIndex;
                    break;
                case BenchmarkAdminPages.Listbenchmarks:
                default:
                    divSuccess.Visible = false;
                    rmpBenchmarks.SelectedIndex = activePanellIndex;
                    rgBenchmarks.Rebind();
                    break;
            }
            deleteBenchmarkError.Visible = false;
        }
        private void editBenchmark(int benchmarkID, Action callback)
        {
            this.CurrentBenchmarkID = benchmarkID;
            LoadBenchmarks(benchmarkID);
            callback();
        }

        private void assignBenchmark(int benchmarkID, Action callback)
        {
            this.CurrentBenchmarkID = benchmarkID;
            LoadDrivers();
            callback();
        }

        public int GetDriverCount(EF.DriverGradingBenchmark benchamrk)
        {
            return benchamrk.tblDriverGradingBenchmarkDrivers.Count();
        }
        private void deleteBenchmark(int benchmarkID, Action callback)
        {
            var benchmark = EF.DataContext.Current.DriverGradingBenchmarks.Include("tblDriverGradingBenchmarkDrivers").First(b => b.DriverGradingBenchmarkID == benchmarkID);
            if (benchmark.tblDriverGradingBenchmarkDrivers.Count > 0 || benchmark.IsDefault)
            {
                deleteBenchmarkError.Visible = true;
            }
            else
            {
                EF.DataContext.Current.DriverGradingBenchmarks.DeleteObject(benchmark);
                EF.DataContext.Current.SaveChanges();
                callback();
            }
        }

        void rgBenchmarks_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            var data = EF.DataContext.Current.DriverGradingBenchmarks.Include("tblDriverGradingBenchmarkDrivers");
            rgBenchmarks.DataSource = data;

        }
     

        private void LoadBenchmarks(int driverGradingBenchmarkID)
        {
            var data = EF.DataContext.Current.DriverCANBenchmarks.Include("DriverGradingBenchmark").Include("CANBenchmarkType").Where(x => x.DriverGradingBenchmark.DriverGradingBenchmarkID == driverGradingBenchmarkID);
            txtTitle.Text = data.First().DriverGradingBenchmark.Title;
            txtDescription.Text = data.First().DriverGradingBenchmark.Description;

            foreach(var item in data)
            {
                if (item.CANBenchmarkType.CANBenchmarkTypeId == 1) // Baseline
                {
                    rnGradingBaseline.Value = (double)item.GradingPercentage;
                    rnHarshBrakingBaseline.Value = item.HarshBrakingCount;
                    rnMPGBaseline.Value = (double)item.MPG;
                    rnOverRevBaseline.Value = item.OverRevCount;
                    rnSpeedingBaseline.Value = item.SpeedingCount;
                    rnIdlingBaseline.Value = (double)item.IdlingPercentage;
                }
                else // Targets
                {
                    rnGradingTarget.Value = (double)item.GradingPercentage;
                    rnHarshBrakingTarget.Value = item.HarshBrakingCount;
                    rnMPGTarget.Value = (double)item.MPG;
                    rnOverRevTarget.Value = item.OverRevCount;
                    rnSpeedingTarget.Value = item.SpeedingCount;
                    rnIdlingTarget.Value = (double)item.IdlingPercentage;
                }
            }

            btnAddNew.Visible = true;
        }

        private void LoadDrivers()
        {
            List<DisplayDriver> driversInBenchmark = new List<DisplayDriver>();
            List<DisplayDriver> driversNotInBenchmark = new List<DisplayDriver>();
            int benchmarkID = this.CurrentBenchmarkID;

            if (benchmarkID > 0)
            {
                driversInBenchmark = (
                           from d in EF.DataContext.Current.DriverGradingBenchmarkDrivers.Include("Resource").Include("Resource.Driver.Individual")
                           where d.Resource.ResourceStatus.ResourceStatusId != 2 &&
                               (
                                   d.DriverGradingBenchmark.DriverGradingBenchmarkID == benchmarkID
                               )
                           orderby d.Resource.Driver.Individual.LastName, d.Resource.Driver.Individual.FirstNames
                           select new DisplayDriver
                           {
                               ResourceID = d.Resource.ResourceId,
                               Description = d.Resource.Driver.Individual.LastName + ", " + d.Resource.Driver.Individual.FirstNames,
                           }
                       ).ToList();
            }
            driversNotInBenchmark =  (from d in EF.DataContext.Current.Drivers.Include("Individual")
                                     where !EF.DataContext.Current.DriverGradingBenchmarkDrivers.Any(x=> x.Resource.ResourceId ==  d.ResourceId)
                                     && d.Resource.ResourceStatus.ResourceStatusId != 2
                                     orderby d.Resource.Driver.Individual.LastName, d.Resource.Driver.Individual.FirstNames
                       select new DisplayDriver
                       {
                           ResourceID = d.Resource.ResourceId,
                           Description = d.Resource.Driver.Individual.LastName + ", " + d.Resource.Driver.Individual.FirstNames,
                       }
                   ).ToList();

            lbDriversInBenchmark.DataSource = driversInBenchmark;
            lbDriversNotInBenchmark.DataSource = driversNotInBenchmark;
            lbDriversInBenchmark.DataBind();
            lbDriversNotInBenchmark.DataBind();

            lblDriversInBenchmark.Text = string.Format("Drivers in this Benchmark ({0})", driversInBenchmark.Count);
            lblDriversNotInBenchmark.Text = string.Format("Drivers NOT in any Benchmark ({0})", driversNotInBenchmark.Count);
        }
      
        private void ResetForm()
        {
            txtTitle.Text = "";
            txtDescription.Text = "";
            rnGradingBaseline.Value = (double?)null;
            rnGradingTarget.Value = (double?)null;
            rnHarshBrakingBaseline.Value = (double?)null;
            rnHarshBrakingTarget.Value = (double?)null;
            rnIdlingBaseline.Value = (double?)null;
            rnIdlingTarget.Value = (double?)null;
            rnMPGBaseline.Value = (double?)null;
            rnMPGTarget.Value = (double?)null;
            rnOverRevBaseline.Value = (double?)null;
            rnOverRevTarget.Value = (double?)null;
            rnSpeedingBaseline.Value = (double?)null;
            rnSpeedingTarget.Value = (double?)null;
            divSuccess.Visible = false;
            lbDriversInBenchmark.Items.Clear();
            lbDriversNotInBenchmark.Items.Clear();
            lblDriversInBenchmark.Text = "Drivers in this Benchmark";
            lblDriversNotInBenchmark.Text = "Drivers NOT in any Benchmark";
            txtTitle.Focus();
        }

        protected void btnAddNew_Click(object sender, EventArgs e)
        {
            addBenchmark(() => gotoPanel(BenchmarkAdminPages.AddOrEditBenchmark));
        }

        private void addBenchmark(Action callback)
        {
            
            btnSaveChanges.Text = "Save New Profile";
            btnCancel.Text = "Cancel";
            callback();
        }
        protected void btnSaveChanges_Click(object sender, EventArgs e)
        {
            EF.DriverGradingBenchmark drivergradingBenchmark = null;
            if (this.CurrentBenchmarkID == 0)
            {
                drivergradingBenchmark = new EF.DriverGradingBenchmark();
                EF.DataContext.Current.AddToDriverGradingBenchmarks(drivergradingBenchmark);

            }
            else
            {
                // creating a new benchmark
                int selectedDriverGradingBenchMarkId = this.CurrentBenchmarkID;
                drivergradingBenchmark = EF.DataContext.Current.DriverGradingBenchmarks.First(d => d.DriverGradingBenchmarkID == CurrentBenchmarkID);

            }
            drivergradingBenchmark.Title = txtTitle.Text;
            drivergradingBenchmark.Description = txtDescription.Text;
            drivergradingBenchmark.IsDefault = chkIsDefault.Checked;

            if (chkIsDefault.Checked)
            {
                // we need to set any other benchmark to not be default
                var data = EF.DataContext.Current.DriverGradingBenchmarks.Where(d => d.IsDefault == true);
                if (data.Count() > 0)
                    foreach (var b in data)
                        b.IsDefault = false;
            }

            // create new benchmarks and set the old ones to not enabled.
            EF.DriverCANBenchmark bmBaseline = new EF.DriverCANBenchmark();
            bmBaseline.CANBenchmarkType = EF.DataContext.Current.CANBenchmarkTypes.First(x => x.CANBenchmarkTypeId == 1);
            bmBaseline.IsEnabled = true;
            bmBaseline.SpeedingCount = (int)rnSpeedingBaseline.Value;
            bmBaseline.OverRevCount = (int)rnOverRevBaseline.Value;
            bmBaseline.HarshBrakingCount = (int)rnHarshBrakingBaseline.Value;
            bmBaseline.MPG = (decimal)rnMPGBaseline.Value;
            bmBaseline.IdlingPercentage = (decimal)rnIdlingBaseline.Value;
            bmBaseline.GradingPercentage = (decimal)rnGradingBaseline.Value;
            bmBaseline.CreateDate = DateTime.Now;
            bmBaseline.CreateUserId = Page.User.Identity.Name;
            drivergradingBenchmark.DriverCANBenchmarks.Add(bmBaseline);
            //EF.DataContext.Current.AddToDriverCANBenchmarks(bmBaseline);

            EF.DriverCANBenchmark bmTarget = new EF.DriverCANBenchmark();
            bmTarget.CANBenchmarkType = EF.DataContext.Current.CANBenchmarkTypes.First(x => x.CANBenchmarkTypeId == 2);
            bmTarget.IsEnabled = true;
            bmTarget.SpeedingCount = (int)rnSpeedingTarget.Value;
            bmTarget.OverRevCount = (int)rnOverRevTarget.Value;
            bmTarget.HarshBrakingCount = (int)rnHarshBrakingTarget.Value;
            bmTarget.IdlingPercentage = (decimal)rnIdlingTarget.Value;
            bmTarget.MPG = (decimal)rnMPGTarget.Value;
            bmTarget.GradingPercentage = (decimal)rnGradingTarget.Value;
            bmTarget.CreateDate = DateTime.Now;
            bmTarget.CreateUserId = Page.User.Identity.Name;
            drivergradingBenchmark.DriverCANBenchmarks.Add(bmTarget);

            EF.DataContext.Current.SaveChanges();

            ResetForm();

            this.CurrentBenchmarkID = drivergradingBenchmark.DriverGradingBenchmarkID;
            LoadDrivers();
            gotoPanel(BenchmarkAdminPages.AssignBenchmarktoDrivers);
            
        }

        void btnSaveAssignments_Click(object sender, EventArgs e)
        {
            // Set the current Benchmarks for this to IsEnabled = false
            EF.DriverGradingBenchmark drivergradingBenchmark = EF.DataContext.Current.DriverGradingBenchmarks.First(x => x.DriverGradingBenchmarkID == this.CurrentBenchmarkID);

          
            // Add the selected drivers to the benchmark
            foreach (var driver in EF.DataContext.Current.DriverGradingBenchmarkDrivers.Where(x => x.DriverGradingBenchmark.DriverGradingBenchmarkID == this.CurrentBenchmarkID))
            {
                EF.DataContext.Current.DriverGradingBenchmarkDrivers.DeleteObject(driver);
            }

            foreach (Telerik.Web.UI.RadListBoxItem item in lbDriversInBenchmark.Items)
            {
                int resourceID = int.Parse(item.Value);
                drivergradingBenchmark.tblDriverGradingBenchmarkDrivers.Add(new EF.DriverGradingBenchmarkDriver() { Resource = EF.DataContext.Current.Resources.First(r => r.ResourceId == resourceID), DriverGradingBenchmark = drivergradingBenchmark });
            }

            EF.DataContext.Current.SaveChanges();

            gotoPanel(BenchmarkAdminPages.Listbenchmarks);
        }


        class DisplayDriver
        {
            public int ResourceID { get; set; }
            public string Description { get; set; }
        }
    }
}