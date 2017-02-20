using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Telerik.Web.UI;
using System.Collections;

namespace Orchestrator.WebUI.administration
{
    public partial class manufacturers : System.Web.UI.Page
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
            this.grdManufacturers.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdManufacturers_NeedDataSource);

        }

        protected void grdManufacturers_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            DataSet ds = Facade.Manufacturer.GetAllManufacturers();

            DataView dv = ds.Tables[0].DefaultView;

            if (m_isClient)
            {
                grdManufacturers.Columns[1].Visible = true;
            }
            else
            {
                grdManufacturers.Columns[1].Visible = false;
            }

            grdManufacturers.DataSource = dv;
        }

        protected void grdManufacturers_UpdateCommand(object sender, GridCommandEventArgs e)
        {
            GridEditableItem item = e.Item as GridEditableItem;
            var manufacturerId = (int)item.GetDataKeyValue("VehicleManufacturerId");
            string oldManufacturerDesc = item.GetDataKeyValue("Description").ToString();
            string manufacturerDesc = (item.FindControl("ManufacturerDescEdit") as RadTextBox).Text;

            if (string.Compare(manufacturerDesc, oldManufacturerDesc, false) != 0)
            {
                Facade.Manufacturer.Update(manufacturerId, manufacturerDesc);
                
            }            

            item.Edit = false;
        }

        protected void grdManufacturers_InsertCommand(object sender, GridCommandEventArgs e)
        {
            GridEditableItem item = e.Item as GridEditableItem;

            string manufacturerDescNew = (item.FindControl("ManufacturerDescNew") as RadTextBox).Text;

            if (!string.IsNullOrEmpty(manufacturerDescNew))
            {
                Facade.Manufacturer.Create(manufacturerDescNew);
            }

            if (item.IsInEditMode)
                item.Edit = false;
        }
    }
}