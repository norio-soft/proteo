using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections.Generic;
using Orchestrator.Globals;

using Telerik.Web.UI;
using Orchestrator.Repositories;
using Orchestrator.Entities;

namespace Orchestrator.WebUI.Point
{
    /// <summary>
    /// Summary description for addupdatepoint.
    /// </summary>
    public partial class addupdatepoint : Orchestrator.Base.BasePage
    {
        #region Page Variables

        private bool m_isUpdate = false;

        private int m_pointId = 0;
        private int m_identityId = 0;
        private string m_organisationname = String.Empty;

        private Entities.Point point;
        private DataSet dsClients;
        public int PointID
        {
            get
            {
                return m_pointId;
            }
        }


        #endregion

        #region Form Elements

        #endregion

        #region Constructor
        public int IdentityId
        {
            get { return m_identityId; }
        }
        #endregion

        #region Page Load/Init/Error

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GPSProfileManagement);
            

            m_pointId = Convert.ToInt32(Request.QueryString["pointId"]);
            m_identityId = Convert.ToInt32(Request.QueryString["identityId"]);
            m_organisationname = Request.QueryString["organisationName"];

            if (m_pointId == 0)
                m_pointId = Convert.ToInt32(ViewState["pointId"]);
            if (m_pointId > 0)
            {
                m_isUpdate = true;

                // Remove the Event handler for the combo box
                cboClient.Enabled = false;
                btnRemove.Visible = true;
              
            }

            if (!IsPostBack)
            {
                PopulateStaticControls();

                this.CboCountryLoad();

                if (m_isUpdate)
                {
                    LoadPoint();
                }
                else
                {
                    if (m_identityId > 0)
                    {
                        cboClient.SelectedValue = m_identityId.ToString();
                        cboClient.Text = m_organisationname;
                        txtDescription.Text = m_organisationname + "-";
                    }

                    txtCreatedBy.Text = ((CustomPrincipal)Page.User).Name;
                    txtCreationDate.Text = DateTime.Now.ToLongDateString();
                    txtLastModifiedBy.Text = string.Empty;
                    txtLastModificationDate.Text = string.Empty;

                    Page.ClientScript.RegisterStartupScript(this.GetType(), "onLoad", "<script language=\"javascript\">document.all['" + txtDescription.ClientID + "'].focus(); </script>");
                }

                PopulateTrafficAreaControl();
                cboTown.Focus();
            }

            infringementDisplay.Visible = false;
        }

        #endregion

        #region Populate Static Controls

        private void PopulateStaticControls()
        {
            if (Orchestrator.Globals.Configuration.FleetMetrikInstance)
                OnlyEnableFleetMetrikFields();

            Facade.IReferenceData facRefData = new Facade.ReferenceData();
            dsClients = facRefData.GetAllClients();

            

            int unitedKingdomCountryId = 1;
            this.SetAddressOnFocus(unitedKingdomCountryId);

            PopulateDeliveryMatrix();
        }

        private void OnlyEnableFleetMetrikFields()
        {
            trDelPerMatrix.Visible = false;
            trPointCode.Visible = false;
            pAddressText.Visible = false;
            trTrafficArea.Visible = false;
            fsRemoveIntegration.Visible = false;
            btnList.Visible = false;
        }

        /// <summary>
        /// Populate Delivery Matrix
        /// </summary>
        private void PopulateDeliveryMatrix()
        {
            Facade.DeliveryWindow facDeliveryWindowSetup = new Orchestrator.Facade.DeliveryWindow();
            DataSet ds = facDeliveryWindowSetup.GetMatrix();

            cboDeliveryPeriod.DataSource = ds.Tables[0];
            cboDeliveryPeriod.DataTextField = "ZoneDescDropDown";
            cboDeliveryPeriod.DataValueField = "DeliveryWindowMatrixID";
            cboDeliveryPeriod.DataBind();
        }

        private void SetAddressOnFocus(int countryId)
        {
            int unitedKingdomCountryId = 1;

            if (countryId == unitedKingdomCountryId)
            {
                //this.txtAddressLine1.Attributes.Add("onfocus", "this.blur();" + txtLongitude.ClientID + ".focus();");
                //this.txtAddressLine2.Attributes.Add("onfocus", "this.blur();" + txtLongitude.ClientID + ".focus();");
                //this.txtAddressLine3.Attributes.Add("onfocus", "this.blur();" + txtLongitude.ClientID + ".focus();");
                //this.txtPostTown.Attributes.Add("onfocus", "this.blur();" + txtLongitude.ClientID + ".focus();");
                //this.txtCounty.Attributes.Add("onfocus", "this.blur();" + txtLongitude.ClientID + ".focus();");
                //this.txtPostCode.Attributes.Add("onfocus", "this.blur();" + txtLongitude.ClientID + ".focus();");
            }
            else
            {
                //this.txtAddressLine1.Attributes.Remove("onfocus");
                //this.txtAddressLine2.Attributes.Remove("onfocus");
                //this.txtAddressLine3.Attributes.Remove("onfocus");
                //this.txtPostTown.Attributes.Remove("onfocus");
                //this.txtCounty.Attributes.Remove("onfocus");
                //this.txtPostCode.Attributes.Remove("onfocus");
            }
        }

        private void PopulateTrafficAreaControl()
        {
            using (Facade.ITrafficArea facTrafficArea = new Facade.Traffic())
            {
                cboTrafficArea.DataValueField = "TrafficAreaId";
                cboTrafficArea.DataTextField = "Description";

                cboTrafficArea.DataSource = facTrafficArea.GetAll();
                cboTrafficArea.DataBind();
                cboTrafficArea.Items.Insert(0, new ListItem("Derive from postcode", "0"));

                if (m_isUpdate)
                {
                    cboTrafficArea.Items.FindByValue(point.Address.TrafficArea.TrafficAreaId.ToString()).Selected = true;
                }
            }
        }
        #endregion
        
        #region Point Load/Populate/Add/Update
        private void LoadPoint()
        {
            if (ViewState["point"] == null)
            {
                Facade.IPoint facPoint = new Facade.Point();
                point = facPoint.GetPointForPointId(m_pointId);
                ViewState["point"] = point;
            }
            else
                point = (Entities.Point)ViewState["point"];

            if (point != null)
            {
                using (var uow = DIContainer.CreateUnitOfWork())
                {
                    var userRepo = DIContainer.CreateRepository<IUserRepository>(uow);
                    var user = userRepo.FindByName(point.CreatorUserId);
                    if (user != null)
                        txtCreatedBy.Text = user.FullName;
                    else txtCreatedBy.Text = point.CreatorUserId;

                    if (!string.IsNullOrEmpty(point.LastUpdaterUserId))
                    {
                        user = userRepo.FindByName(point.LastUpdaterUserId);
                        if (user != null)
                            txtLastModifiedBy.Text = user.FullName;
                        else txtLastModifiedBy.Text = point.LastUpdaterUserId;
                    }
                }
                txtCreationDate.Text = point.CreationDate.ToLongDateString();
                if (point.LastUpdateDate != null) txtLastModificationDate.Text = point.LastUpdateDate.Value.ToLongDateString();

                txtDescription.Text = point.Description;

                RadComboBoxItem rcbItem = new RadComboBoxItem(point.OrganisationName, point.IdentityId.ToString());
                cboClient.Items.Add(rcbItem);
                cboClient.SelectedValue = point.IdentityId.ToString();

                cboTown.SelectedValue = point.PostTown.TownId.ToString();
                cboTown.Text = point.PostTown.TownName;
                txtLongitude.Text = point.Longitude.ToString();
                txtLatitude.Text = point.Latitude.ToString();
                this.txtPointCode.Text = point.PointCode;

                // Added for existing Points whose Addresses have Latitude and Longitude values,
                // but their containing Points do not.
                if (txtLongitude.Text == "0" && txtLatitude.Text == "0" && point.Address != null)
                {
                    txtLongitude.Text = point.Address.Longitude.ToString();
                    txtLatitude.Text = point.Address.Latitude.ToString();
                }

                if (point.IdentityId > 0)
                {
                    if (point.Address != null)
                    {
                        txtAddressLine1.Text = point.Address.AddressLine1;
                        txtAddressLine2.Text = point.Address.AddressLine2;
                        txtAddressLine3.Text = point.Address.AddressLine3;
                        txtPostTown.Text = point.Address.PostTown;
                        txtCounty.Text = point.Address.County;
                        txtPostCode.Text = point.Address.PostCode;
                        this.cboCountry.SelectedValue = point.Address.CountryId.ToString();
                        this.cboTrafficArea.SelectedValue = point.Address.TrafficArea.TrafficAreaId.ToString();
                    }
                }


                txtPointNotes.Text = point.PointNotes;

                hidTrafficArea.Value = point.Address.TrafficArea.TrafficAreaId.ToString();

                // get the delivery point for saving
                this.cboDeliveryPeriod.SelectedValue = point.DeliveryMatrix.ToString();
            }

            btnAdd.Text = "Update";
        }

        private void PopulatePoint()
        {
            if (ViewState["point"] == null)
            {
                point = new Entities.Point();
                point.Address = new Entities.Address();
            }
            else
                point = (Entities.Point)ViewState["point"];

            point.Description = txtDescription.Text;
            point.PointCode = this.txtPointCode.Text;

            //Always make a Point both Collection and Delivery
            point.Collect = true;
            point.Deliver = true;

            point.IdentityId = int.Parse(cboClient.SelectedValue);

            // Get the town object for the town
            point.PostTown = new Entities.PostTown();
            Facade.IPostTown facPostTown = new Facade.Point();
            point.PostTown = facPostTown.GetPostTownForTownId(Convert.ToInt32(cboTown.SelectedValue));

            point.Address.AddressLine1 = txtAddressLine1.Text;
            point.Address.AddressLine2 = txtAddressLine2.Text;
            point.Address.AddressLine3 = txtAddressLine3.Text;
            point.Address.AddressType = eAddressType.Point;
            point.Address.PostTown = txtPostTown.Text;
            point.Address.County = txtCounty.Text;
            point.Address.PostCode = txtPostCode.Text;
            point.Address.CountryDescription = this.cboCountry.Text;
            point.Address.CountryId = Convert.ToInt32(this.cboCountry.SelectedValue);

            if (txtLongitude.Text.Length > 0 && txtLatitude.Text.Length > 0)
            {
                point.Longitude = point.Address.Longitude = Decimal.Parse(txtLongitude.Text);
                point.Latitude = point.Address.Latitude = Decimal.Parse(txtLatitude.Text);
            }

            Facade.IOrganisation facOrganisation = new Facade.Organisation();
            Orchestrator.Entities.Organisation organisation = facOrganisation.GetForIdentityId(point.IdentityId);

            // set the radius if the address was changed by addressLookup
            // if the org has a default, use it, if not, use the system default.
            if (!String.IsNullOrEmpty(this.hdnSetPointRadius.Value))
                if (organisation.Defaults.Count == 0 || organisation.Defaults[0].DefaultGeofenceRadius == null)
                    point.Radius = Globals.Configuration.GPSDefaultGeofenceRadius;
                else
                    point.Radius = organisation.Defaults[0].DefaultGeofenceRadius;

            if (point.Address.TrafficArea == null)
                point.Address.TrafficArea = new Entities.TrafficArea();

            if (m_isUpdate)
                point.Address.TrafficArea.TrafficAreaId = Convert.ToInt32(cboTrafficArea.SelectedValue);
            else
                point.Address.TrafficArea.TrafficAreaId = 0; //Convert.ToInt32(hidTrafficArea.Value);

            point.PointNotes = txtPointNotes.Text;

            // get the delivery point for saving
            point.DeliveryMatrix = Convert.ToInt32(this.cboDeliveryPeriod.SelectedValue);
        }

        private bool UpdatePoint()
        {
            Facade.IPoint facPoint = new Facade.Point();
            string userName = ((Entities.CustomPrincipal)Page.User).UserName;

            // Ensure the latest traffic area id is reflected in the update.
            point.Address.TrafficArea.TrafficAreaId = Convert.ToInt32(cboTrafficArea.SelectedValue);

            Entities.FacadeResult retVal = facPoint.Update(point, userName);
            if (retVal.Success)
            {
                lblConfirmation.Text = "The point has been updated.";
                lblConfirmation.Visible = true;

                point = facPoint.GetPointForPointId(point.PointId);
                cboTrafficArea.SelectedValue = point.Address.TrafficArea.TrafficAreaId.ToString();
            }
            else
            {
                infringementDisplay.Infringements = retVal.Infringements;
                infringementDisplay.DisplayInfringments();
            }

            return retVal.Success;
        }

        private void CboCountryLoad()
        {
            this.cboCountry.Items.Clear();

            Orchestrator.Facade.IReferenceData facRef = new Orchestrator.Facade.ReferenceData();
            DataSet countries = facRef.GetAllCountries();

            foreach (DataRow row in countries.Tables[0].Rows)
            {
                Telerik.Web.UI.RadComboBoxItem rcItem = new Telerik.Web.UI.RadComboBoxItem();
                rcItem.Text = row["CountryDescription"].ToString();
                rcItem.Value = row["CountryId"].ToString();
                this.cboCountry.Items.Add(rcItem);
            }
        }

        private bool AddPoint()
        {
            Facade.IPoint facPoint = new Facade.Point();
            string userName = ((Entities.CustomPrincipal)Page.User).UserName;

            Entities.FacadeResult retVal = facPoint.Create(point, userName);

            if (retVal.Success)
            {
                lblConfirmation.Text = "The point was added.";
                lblConfirmation.Visible = true;

                ViewState["pointId"] = retVal.ObjectId;

                point = facPoint.GetPointForPointId(retVal.ObjectId);

                ViewState["point"] = point;

                cboClient.Enabled = false;

                PopulateTrafficAreaControl();

                cboTrafficArea.Visible = true;
                lblTrafficArea.Visible = true;
                cboTrafficArea.Items.FindByValue(point.Address.TrafficArea.TrafficAreaId.ToString()).Selected = true;

                btnAdd.Text = "Update";
                m_isUpdate = true;

                PopulateTrafficAreaControl();
            }
            else
            {
                infringementDisplay.Infringements = retVal.Infringements;
                infringementDisplay.DisplayInfringments();
            }

            return retVal.Success;
        }

        #endregion

        #region Event Handlers & Methods

        public void cboTrafficAreaValidator_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            // If postcode is blank then a traffic area must be selected.
            // If traffic area is blank then a postcode must be specified.
            if (string.IsNullOrEmpty(txtPostCode.Text) && cboTrafficArea.SelectedValue == 0.ToString())
            {
                args.IsValid = false;
            }
            else
            {
                args.IsValid = true;
            }
        }

        protected void btnRemove_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                bool retVal = false;

                if (point == null)
                    PopulatePoint();

                point.PointStateId = ePointState.Deleted;

                UpdatePoint();

                this.ClientScript.RegisterStartupScript(this.GetType(), "CallBack",
                                                        "__dialogCallBack(window, 'refresh');", true);
            }
        }

        protected void btnAdd_Click(object sender, System.EventArgs e)
        {
            if (Page.IsValid)
            {
                bool retVal = false;

                if (point == null)
                    PopulatePoint();

                if (m_isUpdate)
                    retVal = UpdatePoint();
                else
                    retVal = AddPoint();

                this.ClientScript.RegisterStartupScript(this.GetType(), "CallBack", "__dialogCallBack(window, 'refresh');", true);
            }
        }

        private List<int> GetIntegrationsForRemoval()
        {
            List<int> integrationPoints = new List<int>();

            foreach (GridDataItem i in RemoveIntegrationGrid.MasterTableView.Items)
            {
                if (i.OwnerTableView.Name == RemoveIntegrationGrid.MasterTableView.Name)
                {
                    CheckBox chk = (CheckBox)i["CheckboxSelectColumn"].Controls[0];
                    if (chk.Checked == true)
                    {
                        integrationPoints.Add(Convert.ToInt32(GetDataItemValue(i, "IntegrationPointID")));
                    }
                }
            }

            return integrationPoints;
        }

        #endregion

        #region DBCombo's Server Methods and Initialisation

        public void RemoveIntegrationGrid_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            Facade.IPoint facPoint = new Facade.Point();
            RemoveIntegrationGrid.DataSource = facPoint.GetIntegratedPointsForPointID(Convert.ToInt32(Request.QueryString["pointId"]));
        }



        void cboClient_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            cboClient.Items.Clear();
            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllClientsFiltered("%" + e.Text, true);
            DataTable dt = ds.Tables[0];
            DataTable boundResults = dt.Clone();

            int itemsPerRequest = 15;
            int itemOffset = e.NumberOfItems;
            int endOffset = itemOffset + itemsPerRequest;
            if (endOffset > dt.Rows.Count)
                endOffset = dt.Rows.Count;

            for (int i = itemOffset; i < endOffset; i++)
                boundResults.ImportRow(dt.Rows[i]);

            cboClient.DataSource = boundResults;
            cboClient.DataBind();

            if (dt.Rows.Count > 0)
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
        }

        void cboTown_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            cboTown.Items.Clear();

            int countryId = 1; //UK

            if (e.Context["FilterString"] != null && e.Context["FilterString"].ToString().ToLower().IndexOf("countryid=") > -1)
            {
                string countryIdString = e.Context["FilterString"].ToString().ToLower().Replace("countryid=", "");
                countryIdString = countryIdString.Substring(0, countryIdString.IndexOf(';'));
                countryId = Convert.ToInt32(countryIdString);
            }

            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetTownForTownNameAndCountry(e.Text, countryId);

            int itemsPerRequest = 20;
            int itemOffset = e.NumberOfItems;
            int endOffset = itemOffset + itemsPerRequest;
            if (endOffset > ds.Tables[0].Rows.Count)
                endOffset = ds.Tables[0].Rows.Count;

            DataTable dt = ds.Tables[0];
            Telerik.Web.UI.RadComboBoxItem rcItem = null;
            for (int i = itemOffset; i < endOffset; i++)
            {
                rcItem = new Telerik.Web.UI.RadComboBoxItem();
                rcItem.Text = dt.Rows[i]["Description"].ToString();
                rcItem.Value = dt.Rows[i]["TownId"].ToString();
                cboTown.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }
            else
                this.cboTown.Text = String.Empty;
        }

        void cboCountry_SelectedIndexChanged(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            //if (!String.IsNullOrEmpty(this.cboCountry.SelectedValue))
            //{
            //    if (this.cboCountry.SelectedValue != "1")
            //        this.findAddressLink.Visible = false;
            //    else
            //        this.findAddressLink.Visible = true;

            //    if (!String.IsNullOrEmpty(this.cboTown.Text))
            //    {
            //        Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs args = new Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs();
            //        args.Context["FilterString"] = String.Format("countryid={0};", this.cboCountry.SelectedValue);
            //        args.Text = this.cboTown.Text;

            //        this.cboTown_ItemsRequested(null, args);
            //    }

            //    this.SetAddressOnFocus(Convert.ToInt32(this.cboCountry.SelectedValue));
            //}
        }

        public void RemoveIntegrationGrid_ItemCommand(object source, GridCommandEventArgs e)
        {
            switch (e.CommandName.ToLower())
            {
                case "remove":
                    int pointID = Convert.ToInt32(Request.QueryString["pointId"]);
                    try
                    {
                        List<int> integrationPoints = new List<int>();
                        integrationPoints = GetIntegrationsForRemoval();

                        if (integrationPoints.Count > 0)
                        {
                            GridDataItem dataItem = this.RemoveIntegrationGrid.SelectedItems[0] as GridDataItem;
                            if (dataItem != null)
                            {
                                int integrationPointID = Convert.ToInt32(GetDataItemValue(dataItem, "IntegrationPointID"));
                                Orchestrator.Facade.IPoint facPoint = new Orchestrator.Facade.Point();
                                bool success = false;
                                success = facPoint.RemoveIntegrationFromPoint(integrationPointID);

                                if (success == true)
                                {
                                    Response.Redirect(Request.RawUrl);
                                }
                                else
                                {
                                    ClientScript.RegisterStartupScript(GetType(), "Error", "alert('Unable to remove Integration, please try again');", true);
                                }
                            }
                        }
                        else
                        {
                            ClientScript.RegisterStartupScript(GetType(), "Error", "alert('Please select the Row you would like to delete and the click the cross');", true);
                        }

                    }
                    catch
                    {
                        ClientScript.RegisterStartupScript(GetType(), "Error", "alert('Unable to remove Integration, please try again');", true);
                    }


                    break;
            }
        }
        #endregion

        protected override void OnInit(EventArgs e)
        {
            this.cboTown.OnClientItemsRequesting = this.cboTown.ClientID + "_pointRequesting";
            this.cboClient.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);
            this.cboTown.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboTown_ItemsRequested);
            this.cboCountry.SelectedIndexChanged += new Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventHandler(cboCountry_SelectedIndexChanged);
            this.RemoveIntegrationGrid.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(RemoveIntegrationGrid_NeedDataSource);
            RemoveIntegrationGrid.ItemCommand += new GridCommandEventHandler(RemoveIntegrationGrid_ItemCommand);
            base.OnInit(e);
        }

        


        private string GetDataItemValue(GridDataItem dataItem, string columnName)
        {
            string retVal = dataItem[columnName].Text.Trim();
            if (retVal == "&nbsp;")
                retVal = string.Empty;
            return retVal;
        }
  

        #region Web Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        //private void InitializeComponent()
        //{ }

        #endregion
    }
}