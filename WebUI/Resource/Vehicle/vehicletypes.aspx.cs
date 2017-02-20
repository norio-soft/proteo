using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Orchestrator.WebUI.Resource.Vehicle
{
    public partial class vehicletypes : Orchestrator.Base.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.btnAdd.Click += new EventHandler(btnAdd_Click);
            this.btnCancel.Click += new EventHandler(btnCancel_Click);
            this.btnUpdate.Click += new EventHandler(btnUpdate_Click);
            this.grdVehicleTypes.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdVehicleTypes_NeedDataSource);
            this.grdVehicleTypes.ItemCommand += new Telerik.Web.UI.GridCommandEventHandler(grdVehicleTypes_ItemCommand);
            hidVehicleTypeID.Value = "0";
        }

        void grdVehicleTypes_ItemCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
        {
            if (e.CommandName.ToLower() == "select")
            {
                // Load the Vehicle Type
                this.hidVehicleTypeID.Value = grdVehicleTypes.MasterTableView.DataKeyValues[e.Item.ItemIndex]["VehicleTypeID"].ToString();
                this.txtDescription.Text = grdVehicleTypes.MasterTableView.DataKeyValues[e.Item.ItemIndex]["Description"].ToString();
                this.lblCreated.Text = string.Format("Created By {0} at {1}", grdVehicleTypes.MasterTableView.DataKeyValues[e.Item.ItemIndex]["CreateUserID"].ToString(), ((DateTime)grdVehicleTypes.MasterTableView.DataKeyValues[e.Item.ItemIndex]["CreateDate"]).ToString("dd/MM/yy"));
                this.lblUpdated.Text = string.Format("LastUpdated Created By {0} at {1}", grdVehicleTypes.MasterTableView.DataKeyValues[e.Item.ItemIndex]["LastUpdateUserID"].ToString(), ((DateTime)grdVehicleTypes.MasterTableView.DataKeyValues[e.Item.ItemIndex]["LastUpdateDate"]).ToString("dd/MM/yy"));

                this.fsVehicleType.Visible = true;
                this.fsVehicleTypes.Visible = false;
            }
        }

        void grdVehicleTypes_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            Facade.IVehicle facVehicle = new Facade.Resource();
            this.grdVehicleTypes.DataSource = facVehicle.GetAllVehicleTypes();
        }

        void btnUpdate_Click(object sender, EventArgs e)
        {
            Facade.IVehicle facVehicle = new Facade.Resource();
            int vehcileTypeID = 0;
            int.TryParse(hidVehicleTypeID.Value, out vehcileTypeID);
            int retVal = facVehicle.UpdateVehicleType(vehcileTypeID, txtDescription.Text, Page.User.Identity.Name);
            if (retVal > 0)
            {
                fsVehicleType.Visible = false;
                fsVehicleTypes.Visible = true;
                grdVehicleTypes.Rebind();
            }
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            this.fsVehicleType.Visible = false;
            this.fsVehicleTypes.Visible = true;
        }

        void btnAdd_Click(object sender, EventArgs e)
        {
            hidVehicleTypeID.Value = "0";
            this.txtDescription.Text = String.Empty;
            this.lblCreated.Text = String.Empty;
            this.lblUpdated.Text = String.Empty;
            this.fsVehicleType.Visible = true;
            this.fsVehicleTypes.Visible = false;
        }
    }
}
