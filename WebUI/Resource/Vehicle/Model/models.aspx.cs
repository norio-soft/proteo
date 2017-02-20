using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using Telerik.Web.UI;
using System.Collections;
using System.Configuration;
using System.IO;

namespace Orchestrator.WebUI.administration
{
    public partial class models : System.Web.UI.Page
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
            this.grdModels.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdModels_NeedDataSource);            
        }        

        protected void grdModels_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            DataSet ds = Facade.Model.GetAllModels();
            DataView dv = ds.Tables[0].DefaultView;

            if (m_isClient)
            {
                grdModels.Columns[1].Visible = true;
                grdModels.Columns[2].Visible = true;
            }
            else
            {
                grdModels.Columns[1].Visible = false;
                grdModels.Columns[2].Visible = false;
            }

            grdModels.DataSource = dv;
        }


        protected void grdModels_OnItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item.IsInEditMode)
            {
                GridEditableItem item = (GridEditableItem)e.Item;
                if (!(e.Item is IGridInsertItem))
                {
                    RadComboBox combo = (RadComboBox)item.FindControl("ManufacturerEdit");
                    
                    if (combo != null)
                    {
                        combo.Items.Clear();

                        RadComboBoxItem preselectedItem = new RadComboBoxItem();
                        preselectedItem.Text = item.GetDataKeyValue("ManufacturerDescription").ToString();
                        preselectedItem.Value = item.GetDataKeyValue("VehicleManufacturerId").ToString();
                        combo.Items.Insert(0, preselectedItem);

                        DataSet ds = Facade.Manufacturer.GetAllManufacturers();
                        
                        if (ds != null && ds.Tables.Count > 0)
                        {
                            foreach( DataRow row in ds.Tables[0].Rows )
                            {
                                RadComboBoxItem newItem = new RadComboBoxItem{ Value = row["VehicleManufacturerId"].ToString(), Text=row["Description"].ToString()};

                                if (newItem.Text == item.GetDataKeyValue("ManufacturerDescription").ToString())
                                    newItem.Selected = true;

                                combo.Items.Add(newItem);
                            }                            
                        }
                        else
                        {
                            combo.SelectedIndex = 0;
                        }
                    }
                }
            }
        }


        protected void ManufacturerDropDown_ItemsRequested(object sender, RadComboBoxItemsRequestedEventArgs e)
        {
            DataSet ds = Facade.Manufacturer.GetAllManufacturers();
            DataView dv = ds.Tables[0].DefaultView;

            RadComboBox comboBox = sender as RadComboBox;
            comboBox.Items.Clear();

            comboBox.DataSource = dv;
            comboBox.DataTextField="Description";
            comboBox.DataValueField = "VehicleManufacturerId";            
            comboBox.DataBind();
        }


        protected void grdModels_UpdateCommand(object sender, GridCommandEventArgs e)
        {
            GridEditableItem item = e.Item as GridEditableItem;
            int ModelId = (int)item.GetDataKeyValue("VehicleModelId");
            string ModelDesc = (item.FindControl("ModelDescriptionEdit") as RadTextBox).Text;
            int ManufacturerId = int.Parse((item.FindControl("ManufacturerEdit") as RadComboBox).SelectedValue);

            Facade.Model.Update(ModelId, ModelDesc, ManufacturerId);

            item.Edit = false;
        }

        protected void grdModels_InsertCommand(object sender, GridCommandEventArgs e)
        {
            GridEditableItem item = e.Item as GridEditableItem;
            int ManufacturerId = int.Parse((item.FindControl("ManufacturerNew") as RadComboBox).SelectedValue);
            string ModelDescription = (item.FindControl("ModelDescriptionNew") as RadTextBox).Text;

            if (!string.IsNullOrEmpty(ModelDescription))
            {
                Facade.Model.Create(ManufacturerId, ModelDescription);
            }

            if (item.IsInEditMode)
                item.Edit = false;
        }
    }
}