using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Orchestrator.Repositories;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.mwf
{
    public partial class SearchMwfDriverLoginLogout : System.Web.UI.Page
    {
        /// <summary>
        /// Perform the search on postback
        /// </summary>
        protected void Page_Load(object sender, EventArgs e)
        {

            if (!IsPostBack)
            {
                PopulateDriverDropdown();
                PopulateVehicleDropdown();
            }
        }

        /// <summary>
        /// Call overridden OnInit method
        /// </summary>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.grdDriverLoginLogout.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdDriverLoginLogout_NeedDataSource);
            this.btnSearch.Click += new EventHandler(btnSearch_Click);
        }

        /// <summary>
        /// Query the database for Logons/Logouts based on the search parameters
        /// </summary>
        protected void grdDriverLoginLogout_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            if (IsPostBack)
            {
                using (var uow = DIContainer.CreateUnitOfWork())
                {
                    var repo = DIContainer.CreateRepository<Repositories.IMWF_DriverLogonLogoffRepository>(uow);

                    DateTime? startDate = dteStartDate.SelectedDate;

                    DateTime? endDate = dteEndDate.SelectedDate;

                    int? driverId = null;
                    if (cboDriver.SelectedIndex > 0)
                        driverId = Convert.ToInt32(cboDriver.SelectedValue);

                    int? vehicleId = null;
                    if (cboVehicle.SelectedIndex > 0)
                        vehicleId = Convert.ToInt32(cboVehicle.SelectedValue);

                    var driverLoginsLogouts = repo.GetForFindDriverLoginsLogouts(startDate, endDate, driverId, vehicleId);

                    grdDriverLoginLogout.DataSource = driverLoginsLogouts.ToList();
                }
            }
        }

        /// <summary>
        /// Perform the button clcik for the search button
        /// </summary>
        protected void btnSearch_Click(object sender, EventArgs e)
        {
            this.grdDriverLoginLogout.Rebind();
        }

        /// <summary>
        /// Populate the driver dropdown with all drivers (i.e. including deleted drivers)
        /// </summary>
        private void PopulateDriverDropdown()
        {
            Facade.IDriver driver = new Facade.Resource();

            cboDriver.Items.Clear();

            System.Data.DataSet dsDrivers = driver.GetAllDrivers(true);
            cboDriver.DataSource = dsDrivers;
            cboDriver.DataTextField = "FullName";
            cboDriver.DataValueField = "ResourceId";
            cboDriver.DataBind();

            cboDriver.Items.Insert(0, new RadComboBoxItem("- Select -", "-1"));
        }

        /// <summary>
        /// Populate the vehicle dropdown with all vehicles
        /// </summary>
        private void PopulateVehicleDropdown()
        {
            Facade.IVehicle vehicle = new Facade.Resource();

            cboVehicle.Items.Clear();

            System.Data.DataSet dsVehicles = vehicle.GetAllVehicles();
            cboVehicle.DataSource = dsVehicles;
            cboVehicle.DataTextField = "RegNo";
            cboVehicle.DataValueField = "ResourceId";
            cboVehicle.DataBind();

            cboVehicle.Items.Insert(0, new RadComboBoxItem("- Select -", "-1"));
        }

    }
}