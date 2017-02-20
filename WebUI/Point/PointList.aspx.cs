using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.Point
{
    public partial class PointList : Orchestrator.Base.BasePage
    {

        //-------------------------------------------------------------------------

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        //----------------------------------------------------------------------------

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.rgPoints.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(rgPoints_NeedDataSource);
            this.rgPoints.ItemDataBound += new Telerik.Web.UI.GridItemEventHandler(rgPoints_ItemDataBound);
            this.btnRefreshTop.Click += new EventHandler(btnRefresh_Click);
            this.cboClient.SelectedIndexChanged += new RadComboBoxSelectedIndexChangedEventHandler(cboClient_SelectedIndexChanged);
            this.cboClosestTown.SelectedIndexChanged += new RadComboBoxSelectedIndexChangedEventHandler(cboClosestTown_SelectedIndexChanged);
            this.dlgAddUpdatePointGeofence.DialogCallBack += new EventHandler(DialogCallBack_RebindGrid);
            this.dlgAddUpdatePoint.DialogCallBack += new EventHandler(DialogCallBack_RebindGrid);
            this.dlgAddUpdateOrganisation.DialogCallBack += new EventHandler(DialogCallBack_RebindGrid);
            this.rblGeocodedOrNot.SelectedIndexChanged += new EventHandler(rblGeocodedOrNot_SelectedIndexChanged);

            if(Orchestrator.Globals.Configuration.FleetMetrikInstance)
            {
                this.rgPoints.Columns.FindByUniqueName("Point").HeaderText = "Location";
                this.rgPoints.Columns.FindByUniqueName("PointCode").HeaderText = "Location Code";
            }
           
        }

        //----------------------------------------------------------------------------

        protected void rblGeocodedOrNot_SelectedIndexChanged(object sender, EventArgs e)
        {
            this.rgPoints.Rebind();
        }

        //----------------------------------------------------------------------------

        protected void DialogCallBack_RebindGrid(object sender, EventArgs e)
        {
            this.rgPoints.Rebind();
        }

        //----------------------------------------------------------------------------

        protected void cboClosestTown_SelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            this.rgPoints.Rebind();
        }

        //----------------------------------------------------------------------------

        protected void cboClient_SelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            this.rgPoints.Rebind();
        }

        //----------------------------------------------------------------------------

        protected void rgPoints_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                DataRowView dataRow = (DataRowView)e.Item.DataItem;

                CheckBox chkCustomGeofence = (CheckBox)e.Item.FindControl("chkCustomGeofence");
                if (dataRow["GeofencePointId"] != DBNull.Value
                    && (dataRow["Radius"] == DBNull.Value || Convert.ToInt32(dataRow["Radius"]) == 0))
                {
                    chkCustomGeofence.Checked = true;
                }
                else
                    chkCustomGeofence.Checked = false;
            }
        }

        //----------------------------------------------------------------------------

        public void btnRefresh_Click(object sender, EventArgs e)
        {
            this.rgPoints.Rebind();
        }

        //----------------------------------------------------------------------------

        public void rgPoints_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            int identityId = 0;
            if (!String.IsNullOrEmpty(this.cboClient.Text))
                identityId = int.Parse(this.cboClient.SelectedValue);

            int townId = 0;
            if (!String.IsNullOrEmpty(this.cboClosestTown.SelectedValue))
                townId = int.Parse(this.cboClosestTown.SelectedValue);

            Facade.IPoint facPoint = new Facade.Point();
            DataSet dsPoints = new DataSet();
            dsPoints.Tables.Add("table1");

            if (identityId >= 0 || townId >= 0 || !String.IsNullOrEmpty(this.txtPointDescription.Text)
                || !String.IsNullOrEmpty(this.txtPostCode.Text) || !String.IsNullOrEmpty(this.txtPointCode.Text))
            {
                dsPoints = facPoint.GetAllForOrganisationAndTownFilteredWithGeo(identityId, townId, 
                    this.txtPointDescription.Text, this.txtPostCode.Text,
                    (this.rblGeocodedOrNot.SelectedValue ==String.Empty) ? 0 : Convert.ToInt32(this.rblGeocodedOrNot.SelectedValue),
                    this.txtPointCode.Text);
            }

            this.rgPoints.DataSource = dsPoints;
        }

        //-------------------------------------------------------------------------
    }
}
