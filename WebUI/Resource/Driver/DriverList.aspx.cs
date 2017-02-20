using System;
using System.Data;
using System.Drawing;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.Resource.Driver
{

    public partial class DriverList : Orchestrator.Base.BasePage
    {

        #region Constants

        private const string C_SORT_CRITERIA_VS = "C_SORT_CRITERIA_VS";
        private const string C_SORT_DIRECTION_VS = "C_SORT_DIRECTION_VS";
        private const string C_DRIVERDS_VS = "C_DRIVERDS_VS";

        #endregion

        #region Form Elements

        protected System.Web.UI.WebControls.Button btnAddVehicle;

        #endregion

        #region Page Elements

        private int searchType = 0;
        private bool m_canAlterAvailability = false; // An indication that the current user is allowed to alter the availability status of a driver.

        #endregion

        #region Property Interfaces

        private string SortCriteria
        {
            get { return (string)ViewState[C_SORT_CRITERIA_VS]; }
            set { ViewState[C_SORT_CRITERIA_VS] = value; }
        }

        private string SortDirection
        {
            get { return (string)ViewState[C_SORT_DIRECTION_VS]; }
            set { ViewState[C_SORT_DIRECTION_VS] = value; }
        }

        #endregion

        #region Page Load/Init/Error

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage);
            m_canAlterAvailability = Orchestrator.WebUI.Security.Authorise.CanAccess(eSystemPortion.AddEditResource, eSystemPortion.Plan);

            lblNote.Visible = false;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (Orchestrator.Globals.Configuration.FleetMetrikInstance)
            {
                this.grdDrivers.Columns.FindByUniqueName("ClientSelectColumn").Visible = false;
                this.grdDrivers.Columns.FindByUniqueName("Avail").Visible = false;
                this.grdDrivers.Columns.FindByUniqueName("HomePhone").Visible = false;
                this.grdDrivers.Columns.FindByUniqueName("PersonalMobile").Visible = false;
                this.grdDrivers.Columns.FindByUniqueName("DepotCode").Visible = false;
                this.grdDrivers.Columns.FindByUniqueName("CurrentLocation").Visible = false;
                this.grdDrivers.Columns.FindByUniqueName("CommunicationType").Visible = false;
                this.grdDrivers.Columns.FindByUniqueName("ShowFuture").Visible = false;
                this.grdDrivers.Columns.FindByUniqueName("OrganisationLocationName").Visible = false;
                this.grdDrivers.Columns.FindByUniqueName("Tacho").Visible = true;
            }
            
            this.dlgAddUpdateDriver.DialogCallBack += dlgAddUpdateDriver_DialogCallBack;
            this.grdDrivers.NeedDataSource += grdDrivers_NeedDataSource;
            this.grdDrivers.ItemDataBound += grdDrivers_ItemDataBound;
        }

        public void btnRefresh_Click(object sender, EventArgs e)
        {
            grdDrivers.Rebind();
        }

        void grdDrivers_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is GridCommandItem)
            {
                HtmlInputButton btnAddDriver = e.Item.FindControl("btnAddDriver") as HtmlInputButton;
                btnAddDriver.Attributes.Add("onclick", dlgAddUpdateDriver.GetOpenDialogScript());

                if(Orchestrator.Globals.Configuration.FleetMetrikInstance)
                {
                    var btnSendDriverMessage = e.Item.FindControl("btnSendDriverMessage") as HtmlInputButton;
                    btnSendDriverMessage.Visible = false;
                }
                
                
            }
            else if (e.Item is GridDataItem)
            {
                CheckBox clientSideSelectCheckBox = (e.Item as GridDataItem)["ClientSelectColumn"].Controls[0] as CheckBox;
                HtmlAnchor hypViewDriver = e.Item.FindControl("hypViewDriver") as HtmlAnchor;
                HtmlInputCheckBox chkAvailability = e.Item.FindControl("chkAvailability") as HtmlInputCheckBox;
                Label lblIsAgencyDriver = e.Item.FindControl("lblIsAgencyDriver") as Label;
                HtmlAnchor hypShowFuture = e.Item.FindControl("hypShowFuture") as HtmlAnchor;

                DataRowView drv = (DataRowView)e.Item.DataItem;

                clientSideSelectCheckBox.Enabled = !string.IsNullOrWhiteSpace(drv["PassCode"].ToString());

                string availability = string.Format("javascript:SetAvailability('{0}', this);", drv["ResourceId"]);
                chkAvailability.Attributes.Add("onclick", availability);
                chkAvailability.Checked = drv["ResourceStatusId"].ToString() == "1" ? true : false;

                hypViewDriver.InnerText = drv["FullName"].ToString();
                hypViewDriver.HRef = string.Format("javascript:{0}", dlgAddUpdateDriver.GetOpenDialogScript("identityId=" + drv["IdentityId"].ToString()));

                lblIsAgencyDriver.Text = ((bool)drv["IsAgencyDriver"]) ? "Yes" : "No";

                string date = DateTime.UtcNow.AddDays(-1).ToString("ddMMyyyy") + "0000";
                hypShowFuture.HRef = string.Format("javascript:ShowFuture('{0}','3' ,'{1}');", drv["ResourceId"], date);
                hypShowFuture.InnerText = "Resource Future";
            }

        }

        void grdDrivers_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            grdDrivers.DataSource = GetDriverData();
        }

        void dlgAddUpdateDriver_DialogCallBack(object sender, EventArgs e)
        {
            lblNote.Text = this.ReturnValue;
            lblNote.Visible = true;
            this.grdDrivers.Rebind();
        }

        #endregion

        #region Events & Methods

        private DataSet GetDriverData()
        {
            Facade.IDriver facResource = new Facade.Resource();
            DataSet dsDrivers = null;

            string lastName = txtFilterLastName.Text;
            string firstName = txtFilterFirstName.Text;

            dsDrivers = facResource.GetForNames(firstName, lastName);

            if (dsDrivers.Tables[0].Rows.Count == 0)
            {
                lblNote.Text = "No Drivers for this particular criteria.";
                lblNote.ForeColor = Color.Red;
                lblNote.Visible = true;
            }

            return dsDrivers;
        }

        #endregion

        private void dgDrivers_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            switch (e.CommandName.ToLower())
            {
                case "add":
                    bool success = false;

                    using (Facade.IUser facUser = new Facade.User())
                        success = facUser.AddResourceToList(Convert.ToInt32(e.Item.Cells[11].Text), ((Entities.CustomPrincipal)Page.User).IdentityId);

                    if (success)
                        lblNote.Text = "The resource has been added to your list.";
                    else
                        lblNote.Text = "The resource has not been added to your list.";
                    lblNote.Visible = true;
                    break;
            }
        }

        private void dgDrivers_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                // These attributes are required by the "remember where I am" yellow highlight functionality.
                e.Item.Attributes.Add("onClick", "javascript:HighlightRow('" + e.Item.ClientID + "');");
                e.Item.Attributes.Add("id", e.Item.ClientID);

                DataRowView drv = (DataRowView)e.Item.DataItem;

                CheckBox chkAvailability = (CheckBox)e.Item.FindControl("chkAvailability");
                chkAvailability.Attributes.Add("OnClick", "javascript:SetAvailability(" + ((int)drv["ResourceId"]).ToString() + ", '" + chkAvailability.ClientID + "')");

                eResourceStatus resourceStatus = (eResourceStatus)(int)drv["ResourceStatusId"];
                chkAvailability.Checked = resourceStatus == eResourceStatus.Active;
                chkAvailability.Enabled = m_canAlterAvailability;
            }



        }


    }

}
