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
    public partial class driverinstructionlist : Orchestrator.Base.BasePage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var driverID = Utilities.ParseNullable<int>(Request.QueryString["dID"]);

                if (driverID.HasValue)
                    this.DriverPicker.SelectedValue = driverID.ToString();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.InstructionsGrid.NeedDataSource += (s, ea) => this.InstructionsGrid.DataSource = this.GetInstructionsGridData();
            this.InstructionsGrid.ItemDataBound += this.InstructionsGrid_ItemDataBound;
            this.DriverPicker.SelectedIndexChanged += (s, ea) => this.InstructionsGrid.Rebind();

            this.LoadDrivers();
        }

        private void InstructionsGrid_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                var item = (GridDataItem)e.Item;
                int communicationStatusID = (int)item.GetDataKeyValue("CommunicationStatusID");

                if (communicationStatusID != (int)MWFCommunicationStatusEnum.ReceivedOnDevice)
                    item["CommunicationStatus"].ForeColor = System.Drawing.ColorTranslator.FromHtml("#EF4337");
            }
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

            this.ReassignDriverPicker.DataSource = driverData;
            this.ReassignDriverPicker.DataBind();
        }

        private System.Collections.IEnumerable GetInstructionsGridData()
        {
            Facade.IPoint facPoint = new Facade.Point();
            Facade.Resource facDriver = new Facade.Resource();

            var selectedDriverId = Utilities.ParseNullable<int>(this.DriverPicker.SelectedValue);

            if (!selectedDriverId.HasValue)
                return new string[] { };

            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<IMWF_InstructionRepository>(uow);
                var gridData = repo.GetForHEDriverInstructionsGrid(selectedDriverId.Value);
                return gridData;
            }
        }

    }
}