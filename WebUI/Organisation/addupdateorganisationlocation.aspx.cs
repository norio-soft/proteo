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
using Telerik.Web.UI;

namespace Orchestrator.WebUI.Organisation
{
    /// <summary>
    /// Summary description for addupdateorganisationlocation.
    /// </summary>
    public partial class addupdateorganisationlocation : Orchestrator.Base.BasePage
    {
        #region Form Elements

        protected System.Web.UI.WebControls.RequiredFieldValidator rfvTelephone;

        #endregion

        #region Constants

        private const string C_LOCATION = "Location";
        private const string C_LOCATION_ID = "Location_Id";

        #endregion

        #region Page Variables

        protected int m_identityId = 0;
        private int m_locationId = 0;
        private bool m_isUpdate = false;
        private string m_organisationName;
        private Entities.OrganisationLocation m_location;
        private Entities.Individual _individual = null;
        private string accountCode = Orchestrator.Globals.Configuration.PostCodeAnywhereAccountCode;
        private string licenseKey = Orchestrator.Globals.Configuration.PostCodeAnywhereLicenceKey;
        #endregion

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.AddEditOrganisations, eSystemPortion.AddEditPoints, eSystemPortion.GeneralUsage);
            btnAdd.Enabled = Orchestrator.WebUI.Security.Authorise.CanAccess(eSystemPortion.AddEditOrganisations, eSystemPortion.AddEditPoints);

            m_organisationName = Convert.ToString(Request.QueryString["organisationName"]);
            m_identityId = Convert.ToInt32(Request.QueryString["identityId"]);
            m_locationId = Convert.ToInt32(Request.QueryString["organisationLocationId"]);

            SetWhereIAm();
            if (m_locationId > 0)
                m_isUpdate = true;

            if (!IsPostBack)
            {
                PopulateStaticControls();
                ConfigureReturnLink();

                if (m_isUpdate)
                {
                    LoadLocation();
                }
                else
                {
                    this.txtLocationName.Text = m_organisationName + " - ";
                }
            }
            else
            {
                // retrieve the location from the view state
                m_location = (Entities.OrganisationLocation)ViewState[C_LOCATION];
                _individual = (Entities.Individual)ViewState["VS_Individual"];
            }

            infringementDisplay.Visible = false;
        }
        private void SetWhereIAm()
        {
            string links = "<a href=\"../default.aspx \">Home</a> -> <a href=\"organisations.aspx \">List Of Clients</a>";
            links += "-> <a href=\"addupdateorganisation.aspx?identityId=" + m_identityId.ToString() + "\"> " + m_organisationName + "</a>";
            links += "-> Add/Update Location";
            lblWhereAmI.Text = links;
            lblWhereAmI.Visible = true;
        }

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

        protected void cfvTown_ServerValidate(object obj, ServerValidateEventArgs args)
        {
            args.IsValid = P1TP.Components.Common.SimpleValidation.ValidateNumericValue(cboClosestTown.SelectedValue, 1, true);
        }

        /// <summary>
        /// Configures a link that allows the user to return to the organisation they were manipulating.
        /// </summary>
        private void ConfigureReturnLink()
        {
            string link = "<a href=\"addupdateorganisation.aspx?identityId=" + m_identityId + "\">Return to " + Request.QueryString["organisationName"] + "</a><br>";
            lblReturnLink.Text = link;
            lblReturnLink.Visible = true;
        }

        /// <summary>
        /// Creates the location
        /// </summary>
        /// <returns>true if the create succeeded, false otherwise</returns>
        private bool CreateLocation()
        {
            Facade.IOrganisationLocation facOrganisationLocation = new Facade.Organisation();
            string userId = ((Entities.CustomPrincipal)Page.User).UserName;
            Entities.FacadeResult result = facOrganisationLocation.Create(m_location, _individual, userId);

            if (result.Success)
            {
                m_locationId = result.ObjectId;
                m_location.OrganisationLocationId = m_locationId;
            }
            else
            {
                infringementDisplay.Infringements = result.Infringements;
                infringementDisplay.DisplayInfringments();
            }

            return result.Success;
        }

        /// <summary>
        /// Retrieves the location identified by m_locationId from the organisation and populates the form controls.
        /// Only occurs in update mode.
        /// </summary>
        private void LoadLocation()
        {
            // retrieve the location and store it in viewstate
            Facade.IOrganisationLocation facOrganisationLocation = new Facade.Organisation();
            m_location = facOrganisationLocation.GetLocationForOrganisationLocationId(m_locationId);
            ViewState[C_LOCATION] = m_location;

            Facade.IIndividual facIndividual = new Facade.Individual();
            _individual = facIndividual.GetForOrganisationLocationID(m_location.OrganisationLocationId);

            if (_individual != null)
            {
                ViewState["VS_Individual"] = _individual;
                // populate the form controls
                cboTitle.ClearSelection();
                cboTitle.Items.FindByValue(_individual.Title.ToString()).Selected = true;
                txtFirstNames.Text = _individual.FirstNames;
                txtLastName.Text = _individual.LastName;
                if (_individual.Contacts.GetForContactType(eContactType.Email) != null)
                    txtEmailAddress.Text = _individual.Contacts.GetForContactType(eContactType.Email).ContactDetail;
            }

            // location information
            txtLocationName.Text = m_location.OrganisationLocationName;
            cboType.Items.FindByValue(((int)m_location.OrganisationLocationType).ToString()).Selected = true;
            txtTelephone.Text = m_location.TelephoneNumber;
            txtFax.Text = m_location.FaxNumber;

            // post town information
            cboClosestTown.Text = m_location.Point.PostTown.TownName;
            cboClosestTown.SelectedValue = m_location.Point.PostTown.TownId.ToString();

            // address information
            txtAddressLine1.Text = m_location.Point.Address.AddressLine1;
            txtAddressLine2.Text = m_location.Point.Address.AddressLine2;
            txtAddressLine3.Text = m_location.Point.Address.AddressLine3;
            txtPostTown.Text = m_location.Point.Address.PostTown;
            txtCounty.Text = m_location.Point.Address.County;
            txtPostCode.Text = m_location.Point.Address.PostCode;
            txtLongitude.Text = m_location.Point.Address.Longitude.ToString();
            txtLatitude.Text = m_location.Point.Address.Latitude.ToString();

            if (m_isUpdate)
            {
                cboTrafficArea.Items.FindByValue(m_location.Point.Address.TrafficArea.TrafficAreaId.ToString()).Selected = true;
                cboCountry.SelectedValue = m_location.Point.Address.CountryId.ToString();
            }

            btnAdd.Text = "Update";
        }

        private void PopulateCountryCombo()
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

        /// <summary>
        /// Populates the location object with the new information.
        /// </summary>
        private void PopulateLocation()
        {
            Entities.Point thisPoint;

            if (m_location == null)
            {
                // adding a new location, configure identity and a new address
                m_location = new Orchestrator.Entities.OrganisationLocation();
                m_location.IdentityId = m_identityId;
                thisPoint = new Entities.Point();
                m_location.Point = thisPoint;

            }
            else
            {
                thisPoint = m_location.Point;
            }

            if (_individual == null)
            {
                _individual = new Orchestrator.Entities.Individual();
                _individual.IndividualType = eIndividualType.Contact;
            }

            //Set the indiviudal details;
            _individual.FirstNames = txtFirstNames.Text;
            _individual.LastName = txtLastName.Text;
            _individual.Title = (eTitle)Enum.Parse(typeof(eTitle), cboTitle.SelectedValue);
            if (_individual.Contacts == null)
                _individual.Contacts = new Orchestrator.Entities.ContactCollection();

            _individual.Contacts.Add(new Orchestrator.Entities.Contact(eContactType.Email, txtEmailAddress.Text));
            _individual.Contacts.Add(new Orchestrator.Entities.Contact(eContactType.Telephone, txtTelephone.Text));
            _individual.Contacts.Add(new Orchestrator.Entities.Contact(eContactType.Fax, txtFax.Text));


            // Update the location based on it's settings.
            // location information
            m_location.OrganisationLocationName = txtLocationName.Text;
            m_location.OrganisationLocationType = (eOrganisationLocationType)Enum.Parse(typeof(eOrganisationLocationType), cboType.SelectedValue.Replace(" ", ""), true);
            m_location.TelephoneNumber = txtTelephone.Text;
            m_location.FaxNumber = txtFax.Text;

            // address information
            thisPoint.Description = m_organisationName + " - " + txtPostTown.Text;
            Facade.IPostTown facPostTown = new Facade.Point();
            thisPoint.PostTown = facPostTown.GetPostTownForTownId(Convert.ToInt32(cboClosestTown.SelectedValue));
            if (thisPoint.Address == null)
                thisPoint.Address = new Entities.Address();
            thisPoint.Address.AddressType = eAddressType.Correspondence;
            thisPoint.Address.AddressLine1 = txtAddressLine1.Text;
            thisPoint.Address.AddressLine2 = txtAddressLine2.Text;
            thisPoint.Address.AddressLine3 = txtAddressLine3.Text;
            thisPoint.Address.PostTown = txtPostTown.Text;
            thisPoint.Address.County = txtCounty.Text;
            thisPoint.Address.PostCode = txtPostCode.Text;
            thisPoint.Address.Longitude = Decimal.Parse(txtLongitude.Text);
            thisPoint.Address.Latitude = Decimal.Parse(txtLatitude.Text);
            if (thisPoint.Address.TrafficArea == null)
                thisPoint.Address.TrafficArea = new Entities.TrafficArea();

            thisPoint.Address.CountryDescription = cboCountry.SelectedItem.Text;
            thisPoint.Address.CountryId = Convert.ToInt32(cboCountry.SelectedValue);
            thisPoint.Address.TrafficArea.TrafficAreaId = Convert.ToInt32(cboTrafficArea.SelectedValue);
            thisPoint.IdentityId = m_identityId;
            thisPoint.Latitude = thisPoint.Address.Latitude;
            thisPoint.Longitude = thisPoint.Address.Longitude;
            thisPoint.OrganisationName = m_organisationName;

            Facade.IOrganisation facOrganisation = new Facade.Organisation();
            Orchestrator.Entities.Organisation organisation = facOrganisation.GetForIdentityId(thisPoint.IdentityId);

            // set the radius if the address was changed by addressLookup
            // if the org has a default, use it, if not, use the system default.
            if (!String.IsNullOrEmpty(this.hdnSetPointRadius.Value))
                if (organisation.Defaults[0].DefaultGeofenceRadius == null)
                    thisPoint.Radius = Globals.Configuration.GPSDefaultGeofenceRadius;
                else
                    thisPoint.Radius = organisation.Defaults[0].DefaultGeofenceRadius;
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
            }
        }

        private void PopulateStaticControls()
        {
            Facade.IReferenceData facRefData = new Facade.ReferenceData();
            cboType.DataSource = facRefData.GetAllOrganisationLocationTypes();
            cboType.DataBind();

            this.PopulateCountryCombo();

            cboTitle.DataSource = Enum.GetNames(typeof(eTitle));
            cboTitle.DataBind();

            hidOrganisationName.Value = Request.QueryString["organisationName"];

            PopulateTrafficAreaControl();

        }

        /// <summary>
        /// Updates the location
        /// </summary>
        /// <returns>true if the update succeeded, false otherwise</returns>
        private bool UpdateLocation()
        {
            Facade.IOrganisationLocation facOrganisationLocation = new Facade.Organisation();
            string userId = ((Entities.CustomPrincipal)Page.User).UserName;

            Entities.FacadeResult result = facOrganisationLocation.Update(m_location, _individual, userId);
            if (!result.Success)
            {
                infringementDisplay.Infringements = result.Infringements;
                infringementDisplay.DisplayInfringments();
            }

            m_location = facOrganisationLocation.GetLocationForOrganisationLocationId(m_location.OrganisationLocationId);
            cboTrafficArea.SelectedValue = m_location.Point.Address.TrafficArea.TrafficAreaId.ToString();

            return result.Success;
        }

        #region Event Handlers

        protected void btnAdd_Click(object sender, System.EventArgs e)
        {
            bool retVal = false;

            if (Page.IsValid)
            {
                PopulateLocation();

                if (m_location.OrganisationLocationId == 0)
                {
                    // create a new location
                    retVal = CreateLocation();
                }
                else
                {
                    // update an existing location
                    retVal = UpdateLocation();
                }

                if (m_isUpdate)
                {
                    lblConfirmation.Text = "The Location has " + (retVal ? "" : "not ") + "been updated successfully.";
                }
                else
                {
                    lblConfirmation.Text = "The Location has " + (retVal ? "" : "not ") + "been created successfully.";
                }

                lblConfirmation.Visible = true;
            }
        }

        #endregion

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);
            this.lnkLookUp.Click +=new EventHandler(lnkLookUp_Click);
            this.lstAddress.SelectedIndexChanged +=new EventHandler(lstAddress_SelectedIndexChanged);
            this.btnCancelList.Click += new EventHandler(btnCancelList_Click);
        }

        protected void btnCancelList_Click(object sender, EventArgs e)
        {
            this.pnlAddress.Visible = true;
            this.pnlAddressList.Visible = false;
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {

        }
        #endregion


        private DataSet FindAddressByPostCode(int attempt, string postcode)
        {

            PostCodeAnywhere.LookupUK lookUp = new PostCodeAnywhere.LookupUK();

            DataSet ds = null;

            try
            {
                ds = lookUp.ByPostcode_DataSet(postcode, accountCode, licenseKey, "");
                return ds;
            }
            catch (System.Web.Services.Protocols.SoapException ex)
            {
                if (attempt < 3)
                    return FindAddressByPostCode(attempt++, postcode);
                else
                    throw (ex);
            }
        }

        void lnkLookUp_Click(object sender, EventArgs e)
        {
            FindAddress();
        }

        private void FindAddress()
        {
            DataSet ds = null;

            if (txtPostCode.Text.Length > 0)
            {
                ds = FindAddressByPostCode(1, txtPostCode.Text);
            }
            else if (txtAddressLine1.Text.Length > 1 && txtPostTown.Text.Length > 1)
            {
                ds = FindAddressNoPostCodeByStreet("%" + txtAddressLine1.Text, "%" + txtPostTown.Text);
            }
            
            if (ds.Tables.Count > 0)
            {
                lstAddress.DataSource = ds;
                lstAddress.DataTextField = "description";
                lstAddress.DataValueField = "id";
                lstAddress.DataBind();

                pnlAddressList.Visible = true;
                pnlAddress.Visible = false;
            }
        }

        void lstAddress_SelectedIndexChanged(object sender, EventArgs e)
        {
            PostCodeAnywhere.LookupUK lookUp = new PostCodeAnywhere.LookupUK();
            PostCodeAnywhere.AddressResults address = null;

            address = lookUp.FetchAddress(lstAddress.SelectedValue, PostCodeAnywhere.enLanguage.enLanguageEnglish, PostCodeAnywhere.enContentType.enContentGeographicAddress, accountCode, licenseKey, "");

            DisplayAddress(address);
            pnlAddressList.Visible = false;
            pnlAddress.Visible = true;

        }

        private DataSet FindAddressNoPostCodeByStreet(string streetName, string townName)
        {
            DataSet results = new DataSet();

            PostCodeAnywhere.LookupUK lookUp = new PostCodeAnywhere.LookupUK();

            PostCodeAnywhere.InterimResults properties = lookUp.ByStreet(streetName, townName, false, accountCode, licenseKey, "");

            if (properties.Results.Length > 0)
            {
                results = lookUp.ByStreetKey_DataSet(properties.Results[0].Id, accountCode, licenseKey, "");
            }

            return results;
        }

        private DataSet FindAddressByCompanyAndTown(string companyName, string townName)
        {
            PostCodeAnywhere.LookupUK lookUp = new PostCodeAnywhere.LookupUK();

            DataSet ds = null;

            ds = lookUp.ByOrganisation_DataSet(companyName, townName, false, accountCode, licenseKey, "");

            return ds;
        }

        protected void DisplayAddress(PostCodeAnywhere.AddressResults address)
        {
            txtAddressLine1.Text = address.Results[0].Line1;
            txtAddressLine2.Text = address.Results[0].Line2;
            txtAddressLine3.Text = address.Results[0].Line3;
            txtPostTown.Text = address.Results[0].PostTown;
            txtPostCode.Text = address.Results[0].Postcode;
            txtCounty.Text = address.Results[0].County;
            txtLatitude.Text= address.Results[0].GeographicData.WGS84Latitude.ToString();
            txtLongitude.Text = address.Results[0].GeographicData.WGS84Longitude.ToString();

            txtLocationName.Text = m_organisationName + " - " + txtPostTown.Text;

            this.hdnSetPointRadius.Value = "true";

            if (cboClosestTown.SelectedValue == "")
            {
                Facade.IPostTown facPostTown = new Facade.Point();
                Entities.PostTown postTown = facPostTown.GetPostTownForTownName(txtPostTown.Text);

                if (postTown != null)
                {
                    RadComboBoxItem item = new RadComboBoxItem(txtPostTown.Text, postTown.TownId.ToString());
                    cboClosestTown.Items.Add(item);
                    item.Selected = true;
                }
            }
        }
    }
}
