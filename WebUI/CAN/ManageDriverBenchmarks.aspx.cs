using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Orchestrator.WebUI.CAN
{
    public partial class ManageDriverBenchmarks : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
                LoadOrgUnits();

        }

        private void LoadOrgUnits()
        {
            cboOrgunit.DataSource = EF.DataContext.Current.OrgUnits.OrderBy(c=>c.Name);
            cboOrgunit.DataBind();

            LoadBenchmarks(int.Parse(cboOrgunit.Items[0].Value));
        }


        private void LoadBenchmarks(int orgUnitID)
        {
            var data = EF.DataContext.Current.CANBenchmarks.Include("CANBenchmarkType").Where(c => c.IsEnabled == true && c.OrgUnitId == orgUnitID);
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

        }

        protected void cboOrgunit_SelectedIndexChanged(object sender, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            LoadBenchmarks(int.Parse(cboOrgunit.SelectedValue));
        }

        protected void btnSaveChanges_Click(object sender, EventArgs e)
        {

            int selectedOrgUnitId = int.Parse(cboOrgunit.SelectedValue);
            // Set the current Benchmarks for this Orgunit to IsEnabled = false
            var existingBM = EF.DataContext.Current.CANBenchmarks.Where(a => a.OrgUnitId == selectedOrgUnitId && a.IsEnabled == true);
            foreach (var bm in existingBM)
                bm.IsEnabled = false;

            // create new benchmarks and set the old ones to not enabled.
            EF.CANBenchmark bmBaseline = new EF.CANBenchmark();
            bmBaseline.OrgUnitId = int.Parse(cboOrgunit.SelectedValue);
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
            EF.DataContext.Current.AddToCANBenchmarks(bmBaseline);

            EF.CANBenchmark bmTarget = new EF.CANBenchmark();
            bmTarget.OrgUnitId = int.Parse(cboOrgunit.SelectedValue);
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
            EF.DataContext.Current.AddToCANBenchmarks(bmTarget);
            EF.DataContext.Current.SaveChanges();

            divSuccess.Visible = true;
        }

       
    }
}