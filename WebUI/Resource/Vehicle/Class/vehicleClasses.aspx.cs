using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
using System.Data;

namespace Orchestrator.WebUI.Resource.VehicleClass
{
    public partial class vehicleClasses : System.Web.UI.Page
    {

        protected bool m_isClient = false;

        protected DataTable GridData
        {
            get { return (DataTable)this.ViewState["_gridData"]; }
            set { this.ViewState["_gridData"] = value; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.AddEditUser);

            if (Request.QueryString["IsClient"] != null)
                m_isClient = true;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.grdVehicleClasses.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdVehicleClasses_NeedDataSource);

        }

        protected void grdVehicleClasses_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            DataSet ds = Facade.VehicleClass.GetAllVehicleClasses();

            DataView dv = ds.Tables[0].DefaultView;

            if (m_isClient)
            {
                grdVehicleClasses.Columns[1].Visible = true;
            }
            else
            {
                grdVehicleClasses.Columns[1].Visible = false;
            }

            grdVehicleClasses.DataSource = dv;
        }

        protected void grdVehicleClasses_UpdateCommand(object sender, GridCommandEventArgs e)
        {
            
            GridEditableItem item = e.Item as GridEditableItem;
            var manufacturerId = (int)item.GetDataKeyValue("VehicleClassId");

            string oldDesc = item.GetDataKeyValue("ClassDescription").ToString();
            string newDesc = (item.FindControl("VehicleClassDescEdit") as RadTextBox).Text;

            if (string.Compare(newDesc, oldDesc, false) != 0)
            {
                Facade.VehicleClass.UpdateVehicleClass(manufacturerId, newDesc);
            }

            item.Edit = false;            
        }

        protected void grdVehicleClasses_InsertCommand(object sender, GridCommandEventArgs e)
        {            
            GridEditableItem item = e.Item as GridEditableItem;

            string VehicleClassDescNew = (item.FindControl("VehicleClassDescNew") as RadTextBox).Text;

            if (!string.IsNullOrEmpty(VehicleClassDescNew))
            {
                Facade.VehicleClass.CreateVehicleClass(VehicleClassDescNew);
            }

            if (item.IsInEditMode)
                item.Edit = false;
        }



    }
}