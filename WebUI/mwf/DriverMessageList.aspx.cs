using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using Orchestrator.Repositories;
using Orchestrator.Models;

namespace Orchestrator.WebUI.mwf
{

    public partial class DriverMessageList : Orchestrator.Base.BasePage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var driverID = Utilities.ParseNullable<int>(Request.QueryString["dID"]);

                if (driverID.HasValue)
                    this.DriverPicker.SelectedValue = driverID.ToString();

                this.StartDate.SelectedDate = DateTime.Today.AddDays(-1);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.InstructionsGrid.NeedDataSource += InstructionsGrid_NeedDataSource;
            this.StartDateValid.ServerValidate += (s, ea) => ea.IsValid = !this.EndDate.SelectedDate.HasValue || (this.StartDate.SelectedDate <= this.EndDate.SelectedDate);
            this.ToolbarRefreshButton.Click += this.RefreshButton_Click;
            this.FilterOptionsRefreshButton.Click += this.RefreshButton_Click;

            this.LoadDrivers();
        }

        private void InstructionsGrid_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            if (this.StartDate.SelectedDate.HasValue)
                this.InstructionsGrid.DataSource = this.GetInstructionsGridData();
        }

        private void RefreshButton_Click(object sender, EventArgs e)
        {
            if (this.IsValid)
                this.InstructionsGrid.Rebind();
            else
                ScriptManager.RegisterStartupScript(this, this.GetType(), "ShowFilterOptions", "$(function() { filterOptionsDisplayToggle(true); });", true);
        }

        private void LoadDrivers()
        {
            var facDriver = new Facade.Resource();
            var drivers = facDriver.GetAllDrivers(false).Tables[0].AsEnumerable();

            var driverData = drivers.Select(d => new
            {
                Text = d.Field<string>("FullName"),
                Value = d.Field<int>("ResourceID"),
            });

            this.DriverPicker.DataSource = driverData;
            this.DriverPicker.DataBind();
        }

        private System.Collections.IEnumerable GetInstructionsGridData()
        {
            var selectedDriverID = Utilities.ParseNullable<int>(this.DriverPicker.SelectedValue);

            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<IMWF_InstructionRepository>(uow);
                var endDate = this.EndDate.SelectedDate.HasValue ? this.EndDate.SelectedDate.Value.Date.AddDays(1) : (DateTime?)null;
                var gridData = repo.GetForHENonOrderInstructionsGrid(selectedDriverID, this.StartDate.SelectedDate.Value.Date, endDate);
                return gridData;
            }
        }

    }

}