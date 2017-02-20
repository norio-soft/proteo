using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Humanizer;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.Tariff
{

    public partial class AddZoneMap : Orchestrator.Base.BasePage
    {

        #region Event Handlers
        
        protected void Page_Init(object sender, EventArgs e)
        {
            btnSave.Click += btnSave_Click;
            btnCancel.Click += btnCancel_Click;
            this.Title = "Orchestrator - Add Zone Map";
       }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!this.IsPostBack)
            {
                LoadZoneTypes();
                LoadCopyFromZoneMaps();

                txtZoneMapDescription.Focus();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            //If the Copy option is selelected get the ZoneMapID of the selected
            //zone map and use it to copy the zone map
            Models.ZoneMap zoneMap;

            //We can't use SelectedIndex to check if a selection has been made because
            //when a combo is AJAX populated Items, SelectedIndex and SelectedItem are not updated
            if (optCopyZoneMap.Checked && cboCopyFromZoneMap.SelectedIndex > -1)
                zoneMap = CreateZoneMap(int.Parse(cboCopyFromZoneMap.SelectedValue));
            else
                zoneMap = CreateZoneMap();

            //Redirect and pass new ZoneMapID so that it is selected
            this.Response.Redirect("~/Tariff/EditZoneMap.aspx?ZoneMapID=" + zoneMap.ZoneMapID.ToString());
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            this.Response.Redirect("~/Tariff/ZoneMapList.aspx");
        }

        #endregion

        #region Data Loading

        private void LoadZoneTypes()
        {
            var zoneTypes = Enum.GetValues(typeof(eZoneType)).Cast<eZoneType>().Select(zt => new { ZoneTypeID = (int)zt, Description = zt.Humanize() });

            cboZoneType.DataValueField = "ZoneTypeID";
            cboZoneType.DataTextField = "Description";
            cboZoneType.DataSource = zoneTypes.ToList();
            cboZoneType.DataBind();
        }

        private void LoadCopyFromZoneMaps()
        {
            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<Repositories.IZoneMapRepository>(uow);
                var zoneMaps = repo.GetAll().OrderBy(zm => zm.Description).Select(zm => new { zm.ZoneMapID, zm.Description });

                cboCopyFromZoneMap.Items.Clear();
                cboCopyFromZoneMap.DataValueField = "ZoneMapID";
                cboCopyFromZoneMap.DataTextField = "Description";
                cboCopyFromZoneMap.DataSource = zoneMaps.ToList();
                cboCopyFromZoneMap.DataBind();
            }
        }

        #endregion

        #region Data Saving

        private Models.ZoneMap CreateZoneMap(int? copyFromZoneMapID = null)
        {
            //Create a new zone map and assign entered values
            var zoneType = (eZoneType)int.Parse(cboZoneType.SelectedValue);

            var zoneMap = new Models.ZoneMap
            {
                ZoneType = zoneType,
                Description = txtZoneMapDescription.Text,
                IsEnabled = true,
            };

            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var repo = DIContainer.CreateRepository<Repositories.IZoneMapRepository>(uow);
                repo.Add(zoneMap);

                //Save changes to database
                uow.SaveChanges();

                if (copyFromZoneMapID.HasValue)
                    repo.ZoneMapCopyZones(copyFromZoneMapID.Value, zoneMap.ZoneMapID);
            }

            //and return the new zone map with its assigned ZoneMapID
            return zoneMap;
        }

        #endregion
    
    }

}
