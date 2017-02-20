using Orchestrator.Repositories.DTOs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.administration.VehicleRevenueReportExclusions
{
    public partial class VehicleRevenueReportExclusions : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            grdVehicleRevenueReportExclusions.NeedDataSource += GrdVehicleRevenueReportExclusions_NeedDataSource;
            grdVehicleRevenueReportExclusions.ItemCommand += GrdVehicleRevenueReportExclusions_ItemCommand;
            this.cboVehicle.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboVehicle_ItemsRequested);
        }

        private void GrdVehicleRevenueReportExclusions_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            switch (e.CommandName.ToLower())
            {
                case "remove":
                    int vehicleResourceId = int.Parse(((GridDataItem)e.Item).GetDataKeyValue("VehicleResourceId").ToString());
                    using (var uow = DIContainer.CreateUnitOfWork())
                    {
                        var repo = DIContainer.CreateRepository<Repositories.IVehicleExcludedFromRevenueReportRepository>(uow);
                        repo.RemoveVehicle(vehicleResourceId);
                    }
                    this.grdVehicleRevenueReportExclusions.Rebind();
                    break;
            }
        }

        protected void addNewExclusion_Click(object sender, EventArgs e)
        {
            int vehicleId = 0;
            int.TryParse(cboVehicle.SelectedValue, out vehicleId);

            if (vehicleId == 0)
            {
                //ERROR
            }
            else
            {
                using (var uow = DIContainer.CreateUnitOfWork())
                {
                    var repo = DIContainer.CreateRepository<Repositories.IVehicleExcludedFromRevenueReportRepository>(uow);
                    repo.AddVehicle(vehicleId);
                    this.grdVehicleRevenueReportExclusions.Rebind();
                    cboVehicle.SelectedIndex = -1;
                    cboVehicle.Text = string.Empty;
                }
            }
        }

        private void GrdVehicleRevenueReportExclusions_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<Repositories.IVehicleExcludedFromRevenueReportRepository>(uow);
                grdVehicleRevenueReportExclusions.DataSource = repo.GetExcludedVehicles().ToList();
            }
        }

        protected void cboDepot_SelectedIndexChanged(object sender, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if (!string.IsNullOrEmpty(e.Value)) cboVehicle.Enabled = true;
            else cboVehicle.Enabled = false;
        }

        protected void cboVehicle_SelectedIndexChanged(object sender, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
        }

        private void cboVehicle_ItemsRequested(object sender, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            cboVehicle.Items.Clear();

            Orchestrator.Facade.IVehicle facResource = new Orchestrator.Facade.Resource();
            DataSet ds = facResource.GetAllVehicles();
            List<int> excludedVehicles = null;
            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<Repositories.IVehicleExcludedFromRevenueReportRepository>(uow);
                excludedVehicles = repo.GetExcludedVehicleIds().ToList();
                if (excludedVehicles == null) excludedVehicles = new List<int>();
            }

            List<Telerik.Web.UI.RadComboBoxItem> comboItems =
                (from row in ds.Tables[0].Rows.Cast<DataRow>()
                 where !excludedVehicles.Contains(Convert.ToInt32(row["ResourceId"]))
                 select new Telerik.Web.UI.RadComboBoxItem
            {
                     Value = row["ResourceId"].ToString(),
                     Text = row["RegNo"].ToString()
                 }).GroupBy(item => item.Value).Select(grp => grp.First()).ToList();

            comboItems.Sort((item1, item2) => item1.Text.CompareTo(item2.Text));
            cboVehicle.Items.AddRange(comboItems);
        }        
    }
}