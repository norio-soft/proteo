using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Text;

using Orchestrator;
using Orchestrator.Entities;
using Orchestrator.WebUI;
using Telerik.Web.UI;
using System.Linq;
using System.Drawing;
using Orchestrator.Repositories;

namespace Orchestrator.WebUI.Controls
{
    public partial class Point : System.Web.UI.UserControl
    {
        public event SelectedPointChangedEventHandler SelectedPointChanged;
        public event EventHandler NewPointSelected;

        private string accountCode = Orchestrator.Globals.Configuration.PostCodeAnywhereAccountCode;
        private string licenseKey = Orchestrator.Globals.Configuration.PostCodeAnywhereLicenceKey;
        private const string VS_ControlToFocusOnPostBack = "_controlToFocusOnPostBack";
        private const string VS_CLIENTUSER_ORGANISATION_IDENTITYID = "__vsClientUserOrganisationIdentityID";
        private const string VS_REQUIRE_ADDRESS_LINE1 = "__vsRequireAddressline1";
        private const string VS_SHOW_FULL_ADDRESS = "__vsShowFullAddress";

        private string add1 = string.Empty;
        private int _clientIdentityId = 0;
        private bool editMode = false;

        #region Properties

        public string ComboClientID
        {
            get { return this.cboPoint.ClientID; }
        }

        public Unit Width { get; set; }
        private int _pointID = -1;
        public int PointID
        {
            get
            {
                if (string.IsNullOrEmpty(cboPoint.SelectedValue))
                {
                    if (_pointID > 0)
                        return _pointID;
                    else
                        return -1;
                }

                string[] s = cboPoint.SelectedValue.Split(",".ToCharArray());
                return Convert.ToInt32(s[1]);
            }
            set 
            {
                _pointID = value;
            }

        }

        public int TownID
        {
            // get { return string.IsNullOrEmpty(cboTown.SelectedValue) == true ? -1 : int.Parse(cboTown.SelectedValue); }
            get { return -1; }
        }

        public int ClientUserOrganisationIdentityID
        {
            get { return this.Session[VS_CLIENTUSER_ORGANISATION_IDENTITYID] != null ? (int)this.Session[VS_CLIENTUSER_ORGANISATION_IDENTITYID] : -1; }
            set { this.Session[VS_CLIENTUSER_ORGANISATION_IDENTITYID] = value; }
        }

        public Entities.Point SelectedPoint
        {
            get
            {
                if (PointID > 0)
                {
                    Orchestrator.Facade.Point facPoint = new Orchestrator.Facade.Point();
                    Orchestrator.Entities.Point point = null;
                    point = facPoint.GetPointForPointId(PointID);
                    return point;
                }
                else
                    return null;
            }
            set
            {
                if (value != null)
                {
                    this.PointID = value.PointId;
                }
               UpdateControls(value);
            
            }
        }

        public ePointType PointType
        {
            get
            {
                if (this.ViewState[this.ClientID + "_pointType"] != null)
                    return (Orchestrator.ePointType)this.ViewState[this.ClientID + "_pointType"];
                else
                    return ePointType.Deliver;
            }
            set
            {
                this.ViewState[this.ClientID + "_pointType"] = value;
            }
        }

        public int NewPointOwnerIdentityID
        {
            set
            {
                this.cboNewPointOwner.SelectedValue = value.ToString();
            }
        }

        public string NewPointOwnerDescription
        {
            set
            {
                this.cboNewPointOwner.Text = value;
                this.txtDescription.Text = value + " - ";
            }
        }

        public bool CanCreateNewPoint
        {
            set
            {
                lnkNewPoint.Visible = value;
            }
        }

        public bool CanChangePoint
        {
            set
            {
                cboPoint.Enabled = value;
                if (value == false)
                    lnkClearSelectedPoint.Visible = false;
            }
        }

        public bool IsDepotVisible
        {
            get
            {
                return divDepot.Visible;
            }

            set
            {
                divDepot.Visible = value;
            }
        }

        public string Depot
        {
            get { return (string)this.ViewState["Depot"]; }
            private set { this.ViewState["Depot"] = value; }
        }

        public string CountryCode
        {
            get { return (string)this.ViewState["CountryCode"]; }
            private set { this.ViewState["CountryCode"] = value; }
        }
        

        public string ControlToFocusOnPostBack
        {
            get { return ViewState[VS_ControlToFocusOnPostBack] == null ? string.Empty : (string)ViewState[VS_ControlToFocusOnPostBack]; }
            set { ViewState[VS_ControlToFocusOnPostBack] = value; }
        }

        public bool EditMode
        {
            get
            {
                return editMode;
            }
            set
            {
                if (value)
                    this.PointEditMode();
                else
                    this.Reset();

                editMode = value;
            }
        }

        public bool ShowInEditMode { get; set; }

        public bool CanClearPoint
        {
            set
            {
                lnkClearSelectedPoint.Visible = value;
            }
        }

        public bool CanUpdatePoint
        {
            set
            {
                lnkAlterPoint.Visible = value;

                if (Globals.Configuration.GPSRealtime)
                    lnkPointGeography.Visible = value;
            }
        }
        public bool ShowFullAddress
        {
            get
            {
                return ViewState[VS_SHOW_FULL_ADDRESS] == null ? true : (bool)ViewState[VS_SHOW_FULL_ADDRESS];
            }
            set
            {
                this.ViewState[VS_SHOW_FULL_ADDRESS] = value;
            }
        }

        public bool RequireAddressLine1
        {
            get { return ViewState[VS_REQUIRE_ADDRESS_LINE1] == null ? true : (bool)ViewState[VS_REQUIRE_ADDRESS_LINE1]; }
            set { ViewState[VS_REQUIRE_ADDRESS_LINE1] = value; }
        }

        public bool ShowHelp
        {
            set
            {
                if (value)
                    pnlShowHelp.Visible = true;
            }
        }

        public string ValidationGroup
        {
            set
            {
                rfvPoint.ValidationGroup = value;
            }
        }
        #endregion

        #region Page Load/Init

        private bool _pointSelectionRequired = true;
        public bool PointSelectionRequired { get { return _pointSelectionRequired; } set { _pointSelectionRequired = value; } }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //this.CboCountryLoad();
               

                rfvPoint.Enabled = PointSelectionRequired;

                Orchestrator.Facade.IPoint facPoint = new Orchestrator.Facade.Point();
                Orchestrator.Entities.Point selectedPoint = facPoint.GetPointForPointId(this.PointID);

                if (this.Width.Value != 0)
                {
                    this.cboPoint.Width = this.Width;
                    this.cboClosestTown.Width = this.Width;
                }

                //Reset the Depot
                this.Depot = string.Empty;
                this.CountryCode = string.Empty;

                if (selectedPoint != null)
                {
                    cboPoint.SelectedValue = selectedPoint.IdentityId.ToString() + "," + selectedPoint.PointId.ToString();

                    if (ShowFullAddress)
                    {
                        string fullAddress = selectedPoint.Address.ToString();
                        fullAddress = fullAddress.Replace("\n", "");
                        fullAddress = fullAddress.Replace("\r", "<br>");
                        lblFullAddress.Text = fullAddress;
                        pnlFullAddress.Visible = true;
                    }

                    if (!String.IsNullOrEmpty(selectedPoint.Address.PostCode))
                    {
                        //Change to only show the first depot for a postcode (as there should be only one anyway)
                        //Get the Depot Codes for this Point's PostCode
                        //string depots = string.Empty;

                        //foreach (string depotCode in
                        //    EF.Depot.GetForPostCode(selectedPoint.Address.PostCode).Select(d => d.Code))
                        //{
                        //    depots += depotCode + ", ";
                        //}
                        //if (depots.Length > 0)
                        //{
                        //    depots = depots.Remove(depots.Length - 2);
                        //    lblDepot.Text = string.Format("Depot {0}", depots);
                        //}
                        //else
                        //    lblDepot.Text = "";

                        //We have a postcode so try to lookup a depot
                        var depot = EF.Depot.GetForPostCode(selectedPoint.Address.PostCode).FirstOrDefault();

                        if (depot != null)
                        {
                            this.Depot = depot.Code;
                            this.CountryCode = depot.CountryCode ?? string.Empty;
                        }


                    }

                    if (this.IsDepotVisible)
                        lblDepot.Text = string.Format("Depot {0}", this.Depot);

                    this.lnkPointGeography.OnClientClick = "javascript:" + this.ClientID + "_UpdateGeography(" + selectedPoint.PointId + "); return false;";
                }
                if (this.EditMode)
                    this.PointEditMode();
            }
            else
                this.divDuplicateAddress.Visible = false;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.cboPoint.SelectedIndexChanged += new Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventHandler(cboPoint_SelectedIndexChanged);
            this.cboPoint.OnClientDropDownClosed = this.ClientID + "_cboPoint_ClientDropDownClosed";
            this.cboNewPointOwner.OnClientItemsRequesting = this.ClientID + "_stopRequesting";

            this.cboNewPointOwner.OnClientSelectedIndexChanged = this.ClientID + "_newPointOwner_SelectedIndexChanged";
            this.cboClosestTown.OnClientItemsRequesting = this.ClientID + "_cboClosestTown_Requesting";
            this.cboClosestTown.OnClientSelectedIndexChanged = this.ClientID + "_cboClosestTown_SelectedIndexChanged";
            this.cboClosestTown.OnClientBlur = this.ClientID + "_cboClosestTown_Blur";
            this.cboCountry.OnClientSelectedIndexChanged = this.cboCountry.ClientID + "_SeleectedIndexChange";
            this.btnGoBack.OnClientClick = this.ClientID + "_hideDuplicateAddressWindow";
            this.cfvClosestTown.ServerValidate += new ServerValidateEventHandler(cfvClosestTown_ServerValidate);

            this.lnkAlterPoint.Click += new EventHandler(lnkAlterPoint_Click);
            this.lnkNewPoint.Click += new EventHandler(lnkNewPoint_Click);
            this.lnkClearSelectedPoint.Click += new EventHandler(lnkClearSelectedPoint_Click);
            this.btnCancel.Click += new EventHandler(btnCancel_Click);
            this.btnCreatePoint.Click += new EventHandler(btnCreatePoint_Click);
            this.btnAlterPoint.Click += new EventHandler(btnAlterPoint_Click);
            this.lnkLookUp.Click += new EventHandler(lnkLookUp_Click);
            this.btnContinue.Click += new EventHandler(btnContinue_Click);

            this.cboCountry.SelectedIndexChanged += new RadComboBoxSelectedIndexChangedEventHandler(cboCountry_SelectedIndexChanged);
            this.lstAddress.SelectedIndexChanged += new EventHandler(lstAddress_SelectedIndexChanged);
            this.cfvDescription.ServerValidate += new ServerValidateEventHandler(cfvDescription_ServerValidate);
            this.lvDuplicateAddress.ItemCommand += new EventHandler<ListViewCommandEventArgs>(lvDuplicateAddress_ItemCommand);
            this.lvDuplicateAddress.SelectedIndexChanging += new EventHandler<ListViewSelectEventArgs>(lvDuplicateAddress_SelectedIndexChanging);

            this.btnCancelList.Click += new EventHandler(btnCancelList_Click);

        }

        void lvDuplicateAddress_SelectedIndexChanging(object sender, ListViewSelectEventArgs e)
        {
            
        }

        void lvDuplicateAddress_ItemCommand(object sender, ListViewCommandEventArgs e)
        {
            int pointId = Convert.ToInt32(e.CommandArgument);
            Facade.IPoint facPoint = new Facade.Point();
            Entities.Point p = facPoint.GetPointForPointId(pointId);
            if (p != null)
            {
                if (p.PointStateId == ePointState.Deleted)
                {
                    string userId = ((Orchestrator.Entities.CustomPrincipal)Page.User).UserName;
                    p.PointStateId = ePointState.Approved;
                    Entities.FacadeResult result = facPoint.Update(p, userId);
                }

                this.SelectedPoint = p;
                cboPoint.Text = p.Description;
                cboPoint.SelectedValue = p.IdentityId.ToString() + "," + p.PointId.ToString();

                pnlNewPoint.Visible = false;
                pnlPoint.Visible = true;
                this.divDuplicateAddress.Visible = false;
                inpCreateNewPointSelected.Value = string.Empty;
            }
        }

        void btnContinue_Click(object sender, EventArgs e)
        {
            this.CreatePoint();
            this.divDuplicateAddress.Visible = false;
        }

        //private void CboCountryLoad()
        //{
        //    this.cboCountry.Items.Clear();

        //    Orchestrator.Facade.IReferenceData facRef = new Orchestrator.Facade.ReferenceData();
        //    DataSet countries = facRef.GetAllCountries();

        //    foreach (DataRow row in countries.Tables[0].Rows)
        //    {
        //        Telerik.Web.UI.RadComboBoxItem rcItem = new Telerik.Web.UI.RadComboBoxItem();
        //        rcItem.Text = row["CountryDescription"].ToString();
        //        rcItem.Value = row["CountryId"].ToString();
        //        this.cboCountry.Items.Add(rcItem);
        //    }

        //    if (!Page.IsPostBack)
        //    {
        //        if (this.cboCountry.Items.Count > 0)
        //            this.cboCountry.SelectedIndex = 0;
        //    }
        //}

        void cboCountry_SelectedIndexChanged(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if (!String.IsNullOrEmpty(this.cboCountry.SelectedValue))
            {
                if (this.cboCountry.SelectedValue != "1")
                    this.lnkLookUp.Visible = false;
                else
                    this.lnkLookUp.Visible = true;
            }
        }

        void btnCancelList_Click(object sender, EventArgs e)
        {
            pnlAddressList.Visible = false;
            pnlNewPoint.Visible = true;
        }

        void cfvDescription_ServerValidate(object source, ServerValidateEventArgs args)
        {
            txtDescription.Text = txtDescription.Text.Trim();
            if (txtDescription.Text == cboNewPointOwner.Text + " - " || txtDescription.Text.StartsWith(" "))
                args.IsValid = false;
            else
                args.IsValid = true;
        }

        void cfvClosestTown_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = cboClosestTown.SelectedValue != string.Empty;
        }

        void lstAddress_SelectedIndexChanged(object sender, EventArgs e)
        {
            PostCodeAnywhere.LookupUK lookUp = new PostCodeAnywhere.LookupUK();
            PostCodeAnywhere.AddressResults address = null;

            address = lookUp.FetchAddress(lstAddress.SelectedValue, PostCodeAnywhere.enLanguage.enLanguageEnglish, PostCodeAnywhere.enContentType.enContentGeographicAddress, accountCode, licenseKey, "");

            DisplayAddress(address);
            pnlAddressList.Visible = false;
            pnlNewPoint.Visible = true;

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
            else
            {
                ds = FindAddressByCompanyAndTown("%" + cboNewPointOwner.Text, "%" + txtPostTown.Text);
            }

            if (ds.Tables.Count > 0)
            {
                lstAddress.DataSource = ds;
                lstAddress.DataTextField = "description";
                lstAddress.DataValueField = "id";
                lstAddress.DataBind();

                pnlAddressList.Visible = true;
                pnlNewPoint.Visible = false;
            }
        }

        protected void DisplayAddress(PostCodeAnywhere.AddressResults address)
        {
            add1 = txtAddressLine1.Text = address.Results[0].Line1;

            txtAddressLine2.Text = address.Results[0].Line2;
            txtAddressLine3.Text = address.Results[0].Line3;
            txtPostTown.Text = address.Results[0].PostTown;
            txtPostCode.Text = address.Results[0].Postcode;
            txtCounty.Text = address.Results[0].County;
            hidLat.Value = address.Results[0].GeographicData.WGS84Latitude.ToString();
            hidLon.Value = address.Results[0].GeographicData.WGS84Longitude.ToString();

            int countryID = Int32.Parse(cboCountry.SelectedValue);

            this.hdnSetPointRadius.Value = "true";

            if (cboClosestTown.SelectedValue == "")
            {
                cboClosestTown.Text = txtPostTown.Text;
                Facade.IPostTown facPostTown = new Facade.Point();
                Entities.PostTown postTown = facPostTown.GetPostTownForTownName(txtPostTown.Text, countryID);
                if (postTown != null)
                {
                    cboClosestTown.SelectedValue = postTown.TownId.ToString();
                    txtDescription.Text = string.Concat(cboNewPointOwner.Text, " - ", cboClosestTown.Text);
                }

                cboClosestTown.Focus();
            }
        }

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

        #endregion

        #region Button Event Handlers

        void btnAlterPoint_Click(object sender, EventArgs e)
        {
            Page.Validate("AlterPoint");

            if (Page.IsValid)
            {
                int ownerID = 0;

                #region Validate that the Point Name is Unique for this Organisation
                bool foundPointName = false;
                lblError.Visible = false;
                Orchestrator.Facade.IPoint facPoint = new Orchestrator.Facade.Point();
                Entities.Point point = facPoint.GetPointForPointId(PointID);

                ownerID = new Facade.Organisation().GetForName(cboNewPointOwner.Text).IdentityId;

                DataSet pointNames = facPoint.GetAllForOrganisation(ownerID, ePointType.Any, txtDescription.Text);
                foreach (DataRow row in pointNames.Tables[0].Rows)
                {
                    if (((string)row["Description"]) == txtDescription.Text && point.PointId != (int)row["PointId"])
                        foundPointName = true;
                }
                #endregion

                if (foundPointName)
                {
                    lblError.Text = "The Description must be unique for this organisation.";
                    lblError.ForeColor = Color.Red;
                    lblError.Visible = true;
                    pnlPoint.Visible = false;
                    pnlNewPoint.Visible = true;
                    pnlFullAddress.Visible = false;
                }
                else
                {
                    Orchestrator.Facade.IPostTown facPostTown = new Orchestrator.Facade.Point();
                    Orchestrator.Entities.PostTown town = facPostTown.GetPostTownForTownId(int.Parse(cboClosestTown.SelectedValue));

                    // Set the point owner and description
                    point.OrganisationName = cboPoint.Text;
                    point.IdentityId = ownerID;
                    point.Description = txtDescription.Text;
                    point.PointCode = this.txtPointCode.Text;

                    // Get the point type
                    switch (this.PointType)
                    {
                        case ePointType.Collect:
                            point.Collect = true;
                            break;
                        case ePointType.Deliver:
                            point.Deliver = true;
                            break;
                        case ePointType.Any:
                            point.Collect = true;
                            point.Deliver = true;
                            break;
                    }

                    // set the address
                    Orchestrator.Entities.Address address = new Orchestrator.Entities.Address();
                    address.AddressLine1 = txtAddressLine1.Text;
                    address.AddressLine2 = txtAddressLine2.Text;
                    address.AddressLine3 = txtAddressLine3.Text;
                    address.AddressType = eAddressType.Point;
                    address.County = txtCounty.Text;
                    address.CountryDescription = this.cboCountry.Text;
                    address.CountryId = Convert.ToInt32(this.cboCountry.SelectedValue);
                    address.IdentityId = ownerID;
                    decimal latitude = 0;
                    if (decimal.TryParse(hidLat.Value, out latitude))
                        address.Latitude = latitude;
                    decimal longitude = 0;
                    if (decimal.TryParse(hidLon.Value, out longitude))
                        address.Longitude = longitude;
                    address.PostCode = txtPostCode.Text.ToUpper();
                    address.PostTown = txtPostTown.Text;
                    if (address.TrafficArea == null)
                        address.TrafficArea = new Orchestrator.Entities.TrafficArea();

                    Facade.IOrganisation facOrganisation = new Facade.Organisation();
                    Orchestrator.Entities.Organisation organisation = facOrganisation.GetForIdentityId(point.IdentityId);

                    // set the radius if the address was changed by addressLookup
                    // if the org has a default, use it, if not, use the system default.
                    if (!String.IsNullOrEmpty(this.hdnSetPointRadius.Value))
                        if (organisation.Defaults != null)
                            if (organisation.Defaults.Count > 0 && organisation.Defaults[0].DefaultGeofenceRadius.HasValue)
                                point.Radius = organisation.Defaults[0].DefaultGeofenceRadius;
                            else
                                point.Radius = Globals.Configuration.GPSDefaultGeofenceRadius;
                        else
                            point.Radius = Globals.Configuration.GPSDefaultGeofenceRadius;

                    // Get the Traffic Area for this Point
                    address.TrafficArea.TrafficAreaId = Convert.ToInt32(cboTrafficArea.SelectedValue);

                    point.Address = address;
                    point.Longitude = address.Longitude;
                    point.Latitude = address.Latitude;
                    point.PostTown = town;

                    point.PointNotes = txtPointNotes.Text;

                    if (ClientUserOrganisationIdentityID > 0)
                        point.PointStateId = ePointState.Unapproved;
                    else
                        point.PointStateId = ePointState.Approved;

                    point.PhoneNumber = txtPhoneNumber.Text;

                    string userId = ((Orchestrator.Entities.CustomPrincipal)Page.User).UserName;

                    // Create the new point
                    Entities.FacadeResult result = facPoint.Update(point, userId);

                    if (result.Success)
                    {
                        // get the Point with all parts populated.
                        this.SelectedPoint = point;
                        cboPoint.Text = point.Description;
                        cboPoint.SelectedValue = point.IdentityId.ToString() + "," + point.PointId.ToString();

                        pnlNewPoint.Visible = false;
                        pnlPoint.Visible = true;
                        inpCreateNewPointSelected.Value = string.Empty;
                    }
                    else
                    {
                        for (int i = 0; i < result.Infringements.Count; i++)
                            lblError.Text += result.Infringements[i].Description + Environment.NewLine;

                        lblError.ForeColor = Color.Red;
                        lblError.Visible = true;
                        pnlPoint.Visible = false;
                        pnlNewPoint.Visible = true;
                        pnlFullAddress.Visible = false;
                    }
                }
            }
        }

        protected void CreatePoint()
        {
            if (Page.IsValid)
            {
                string userID = ((Orchestrator.Entities.CustomPrincipal)Page.User).UserName;

                var ownerIdentityID = string.IsNullOrWhiteSpace(cboNewPointOwner.SelectedValue) ? (int?)null : int.Parse(cboNewPointOwner.SelectedValue);
                var closestTownID = int.Parse(cboClosestTown.SelectedValue);
                var countryID = int.Parse(cboCountry.SelectedValue);
                var trafficAreaID = int.Parse(cboTrafficArea.SelectedValue);

                decimal latitude = 0;
                decimal longitude = 0;
                decimal.TryParse(hidLat.Value, out latitude);
                decimal.TryParse(hidLon.Value, out longitude);

                Facade.IPoint facPoint = new Facade.Point();

                var result = facPoint.Create(
                    ownerIdentityID,
                    cboNewPointOwner.Text,
                    txtDescription.Text,
                    closestTownID,
                    txtAddressLine1.Text,
                    txtAddressLine2.Text,
                    txtAddressLine3.Text,
                    txtPostTown.Text,
                    txtCounty.Text,
                    txtPostCode.Text.ToUpper(),
                    countryID,
                    latitude,
                    longitude,
                    this.PointType,
                    txtPointCode.Text,
                    txtPointNotes.Text,
                    txtPhoneNumber.Text,
                    trafficAreaID,
                    this.ClientUserOrganisationIdentityID,
                    userID);

                if (!result.Success)
                {
                    if (result.Infringements.Any(bri => bri.Key == typeof(Orchestrator.BusinessRules.eBRPoint).FullName + "." + BusinessRules.eBRPoint.PointNameAlreadyExistsForClient.ToString()))
                    {
                        lblError.Text = "The Description must be unique for this organisation.";
                        lblError.ForeColor = Color.Red;
                        lblError.Visible = true;
                        pnlPoint.Visible = false;
                        pnlNewPoint.Visible = true;
                        pnlFullAddress.Visible = false;
                    }
                }
                else if (result.ObjectId > 0)
                {
                    // get the Point with all parts populated.
                    var newPoint = facPoint.GetPointForPointId(result.ObjectId);
                    this.SelectedPoint = newPoint;
                    cboPoint.Text = newPoint.Description;
                    cboPoint.SelectedValue = newPoint.IdentityId.ToString() + "," + newPoint.PointId.ToString();

                    if (SelectedPointChanged != null)
                        SelectedPointChanged(this, new SelectedPointChangedEventArgs(this.SelectedPoint));

                    this.lnkPointGeography.OnClientClick = "javascript:" + this.ClientID + "_UpdateGeography(" + this.SelectedPoint.PointId + "); return false;";

                    pnlNewPoint.Visible = false;
                    pnlPoint.Visible = true;
                    inpCreateNewPointSelected.Value = string.Empty;

                    txtDescription.Text = newPoint.Description;
                }
            }
        }

        void btnCreatePoint_Click(object sender, EventArgs e)
        {
            if (GetPossibleDuplicates().Count == 0)
            {
                this.CreatePoint();
                this.divDuplicateAddress.Visible = false;
            }
        }

        private void PopulateTrafficAreaControl()
        {
            using (Facade.ITrafficArea facTrafficArea = new Facade.Traffic())
            {
                cboTrafficArea.Items.Clear();

                cboTrafficArea.DataValueField = "TrafficAreaId";
                cboTrafficArea.DataTextField = "Description";
                cboTrafficArea.DataSource = facTrafficArea.GetAll();
                cboTrafficArea.DataBind();

                cboTrafficArea.Items.Insert(0, new RadComboBoxItem("Derive from Postcode", "0"));
            }
        }

        public void cboTrafficAreaValidator_ServerValidate(object source, System.Web.UI.WebControls.ServerValidateEventArgs args)
        {
            // User must either select a traffic area or enter a postcode from which one can be derived.
            if (args.Value == 0.ToString() && !(txtPostCode.Text.Length > 0))
            {
                args.IsValid = false;
            }
            else
            {
                args.IsValid = true;
            }
        }

        void btnCancel_Click(object sender, EventArgs e)
        {
            pnlNewPoint.Visible = false;
            pnlPoint.Visible = true;

            //if (cboPoint.SelectedValue == "")
            //{
            //      TF: Commented out 2 lines below as cannot see the point of updating the point value
            //      with the new point details when the creation of the new point has been cancelled.
            //    cboPoint.SelectedValue = cboNewPointOwner.SelectedValue;
            //    cboPoint.Text = cboNewPointOwner.Text;
            //}

            if (!string.IsNullOrEmpty(lblFullAddress.Text) && lblFullAddress.Text.ToUpper() != "FULL ADDRESS")
            {
                lblFullAddress.Text = string.Empty;
                pnlFullAddress.Visible = false;

            }

            inpCreateNewPointSelected.Value = string.Empty;
        }

        void lnkNewPoint_Click(object sender, EventArgs e)
        {
            Reset();
            PointEditMode();

            if (NewPointSelected != null)
                NewPointSelected(this, e);

            // attach the postcode handling client scripts
            
        }

        private void PointEditMode()
        {
            if (PointID > 0 || SelectedPoint != null)
            {
                ScriptManager.GetCurrent(Page).SetFocus(cboCountry);

                btnCreatePoint.Visible = false;
                btnAlterPoint.Visible = true;
                cboNewPointOwner.Enabled = false;
                lblError.Text = String.Empty;
                lblError.Visible = false;

                // Clear Previous Values 
                cboClosestTown.ClearSelection();
                cboCountry.ClearSelection();
                txtAddressLine1.Text = "";
                txtAddressLine2.Text = "";
                txtAddressLine3.Text = "";
                txtPostTown.Text = "";
                txtPostCode.Text = "";
                txtCounty.Text = "";
                txtPointNotes.Text = "";
                hidLat.Value = "";
                hidLon.Value = "";
                this.txtPointCode.Text = String.Empty;
                this.hdnSetPointRadius.Value = String.Empty;

                // Get the point and populate the fields.
                Facade.Point facPoint = new Orchestrator.Facade.Point();
                Entities.Point point = null;
                if (SelectedPoint == null)
                    point = SelectedPoint;
                else
                    point = facPoint.GetPointForPointId(this.PointID);

                EditPointLoadCountry();

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
                txtAddressLine1.Text = point.Address.AddressLine1;
                txtAddressLine2.Text = point.Address.AddressLine2;
                txtAddressLine3.Text = point.Address.AddressLine3;
                txtPostTown.Text = point.PostTown.TownName;
                txtPostCode.Text = point.Address.PostCode;
                txtCounty.Text = point.Address.County;
                txtPointNotes.Text = point.PointNotes;
                this.txtPointCode.Text = point.PointCode;
                hidLat.Value = point.Latitude.ToString();
                hidLon.Value = point.Longitude.ToString();
                this.txtPhoneNumber.Text = point.PhoneNumber;

                cboCountry.Text = point.Address.CountryDescription;
                cboCountry.SelectedValue = point.Address.CountryId.ToString();

                // Set closest town combo
                cboClosestTown.Text = point.PostTown.TownName;
                cboClosestTown.SelectedValue = point.PostTown.TownId.ToString();

                // Set traffic area
                PopulateTrafficAreaControl();
                cboTrafficArea.Items.FindItemByValue(point.Address.TrafficArea.TrafficAreaId.ToString()).Selected = true;

                // Get/set the organisation 
                cboNewPointOwner.Text = point.OrganisationName;
                
                pnlPoint.Visible = false;
                pnlNewPoint.Visible = true;
                this.rfvAddressLine1.Enabled = this.RequireAddressLine1;
                lblFullAddress.Text = string.Empty;
                pnlFullAddress.Visible = false;
            }
            else
            {
                try
                {
                    ScriptManager.GetCurrent(Page).SetFocus(cboNewPointOwner);
                }
                catch { }

                // this is becuase we do not want to show the cancel button from the add point page.
                if (Request.QueryString["allowclose"] != null && Convert.ToBoolean( Request.QueryString["allowclose"].ToString()) == false)
                    btnCancel.Visible = false;


                btnCreatePoint.Visible = true;
                btnAlterPoint.Visible = false;
                cboNewPointOwner.Enabled = true;
                lblError.Text = String.Empty;
                lblError.Visible = false;

                // Clear Previous Values 
                //cboClosestTown.ClearSelection();
                if (string.IsNullOrEmpty(cboNewPointOwner.SelectedValue))
                {
                    cboNewPointOwner.SelectedValue = "";
                    cboNewPointOwner.Text = "";
                }
                else
                { 
                    // this s a new point for a client so do not allow the organisation to be changed.
                    cboNewPointOwner.Enabled = false;
                }
                
                txtCreatedBy.Text = ((CustomPrincipal)Page.User).Name;
                txtCreationDate.Text = DateTime.Now.ToLongDateString();
                txtLastModifiedBy.Text = string.Empty;
                txtLastModificationDate.Text = string.Empty;

                cboClosestTown.SelectedValue = "";
                cboClosestTown.Text = "";

                txtAddressLine1.Text = "";
                txtAddressLine2.Text = "";
                txtAddressLine3.Text = "";
                txtPostTown.Text = "";
                txtPostCode.Text = "";
                txtCounty.Text = "";
                hidLat.Value = "";
                hidLon.Value = "";

                txtPointNotes.Text = "";
                txtDescription.Text = " - ";
                this.txtPointCode.Text = "";
                this.txtPhoneNumber.Text = "";
                pnlPoint.Visible = false;
                pnlNewPoint.Visible = true;
                this.rfvAddressLine1.Enabled = this.RequireAddressLine1;
                lblFullAddress.Text = string.Empty;
                pnlFullAddress.Visible = false;
                PopulateTrafficAreaControl();
                EditPointLoadCountry();

                cboCountry.SelectedIndex = 0;
            }

            inpCreateNewPointSelected.Value = bool.TrueString;
        }

        private void EditPointLoadCountry()
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

        void lnkAlterPoint_Click(object sender, EventArgs e)
        {
            PointEditMode();
        }

        void lnkPointGeography_Click(object sender, EventArgs e)
        {
            PointEditMode();
        }

        void lnkClearSelectedPoint_Click(object sender, EventArgs e)
        {
            cboPoint.ClearSelection();
            cboPoint.SelectedIndex = 0;
            cboPoint.Text = "";
            this.SelectedPoint = null;
            lblFullAddress.Text = String.Empty;
            pnlFullAddress.Visible = false;
            this.lnkPointGeography.OnClientClick = String.Empty;
            this.hdnSetPointRadius.Value = String.Empty;
            this.txtPointCode.Text = String.Empty;
        }

        #endregion

        #region Combo Events

        public void cboPoint_SelectedIndexChanged(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            Orchestrator.Entities.Point selectedPoint = null;

            if (cboPoint.SelectedValue != String.Empty)
            {
                Orchestrator.Facade.IPoint facPoint = new Orchestrator.Facade.Point();
                selectedPoint = facPoint.GetPointForPointId(this.PointID);
            }

            UpdateControls(selectedPoint);
       }

        private void UpdateControls(Entities.Point point)
        {
            //Reset the Depot
            this.Depot = string.Empty;
            this.CountryCode = string.Empty;
            lblDepot.Text = string.Empty;

            if (point == null)
            {
                cboPoint.Text = string.Empty;
                cboPoint.SelectedValue = string.Empty;
                lblFullAddress.Text = string.Empty;
                pnlFullAddress.Visible = false;
                this.lnkPointGeography.OnClientClick = String.Empty;
            }
            else
            {
                cboPoint.Text = point.Description;
                cboPoint.SelectedValue = point.IdentityId.ToString() + "," + point.PointId.ToString();

                if (ShowFullAddress)
                {
                    string fullAddress = point.Address.ToString();
                    fullAddress = fullAddress.Replace("\n", "");
                    fullAddress = fullAddress.Replace("\r", "<br>");
                    lblFullAddress.Text = fullAddress;
                    pnlFullAddress.Visible = true;
                }
                if (point.Address != null)
                {
                    if (!String.IsNullOrEmpty(point.Address.PostCode))
                    {
                        //We have a postcode so try to lookup a depot
                        var depot = EF.Depot.GetForPostCode(point.Address.PostCode).FirstOrDefault();

                        if (depot != null)
                            this.Depot = depot.Code;
                    }
                }
                if (this.IsDepotVisible)
                    lblDepot.Text = string.Format("Depot {0}", this.Depot);

                this.lnkPointGeography.OnClientClick = "javascript:" + this.ClientID + "_UpdateGeography(" + point.PointId + "); return false;";

                if (SelectedPointChanged != null)
                    SelectedPointChanged(this, new SelectedPointChangedEventArgs(point));
            }
        }

        #endregion

        #region Public Methods
        public void Reset()
        {
            cboNewPointOwner.SelectedValue = string.Empty;
            cboNewPointOwner.Enabled = true;
            cboNewPointOwner.Text = string.Empty;

            cboPoint.SelectedValue = String.Empty;
            cboPoint.Text = String.Empty;
            cboClosestTown.Text = String.Empty;
            cboClosestTown.SelectedValue = String.Empty;
            cboCountry.Text = String.Empty;
            cboCountry.SelectedValue = String.Empty;
            hidLat.Value = String.Empty;
            hidLon.Value = String.Empty;
            lblFullAddress.Text = String.Empty;
            pnlFullAddress.Visible = false;
            this.lnkPointGeography.OnClientClick = String.Empty;
            this.hdnSetPointRadius.Value = String.Empty;
            this.txtPointCode.Text = String.Empty;
            this.divDuplicateAddress.Visible = false;
            this.lvDuplicateAddress.Items.Clear();

        }

        public PointCollection GetPossibleDuplicates()
        {
            BusinessLogicLayer.IPoint busPoint = new BusinessLogicLayer.Point();
            PointCollection pointCollection = busPoint.GetPossibleDuplicates(this.txtDescription.Text, this.cboNewPointOwner.Text, this.txtAddressLine1.Text, this.txtPostTown.Text, this.txtPostCode.Text);

            if (pointCollection.Count > 0)
            {
                this.lvDuplicateAddress.DataSource = pointCollection;
                this.lvDuplicateAddress.DataBind();
                this.divDuplicateAddress.Visible = true;
            }

            return pointCollection;
        }
        #endregion

        protected void cfvCreatePoint_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = true;

            if (btnCreatePoint.Visible == true)
            {
                // The create point process is not complete
                args.IsValid = false;
            }
        }

        protected void cfvAlterPoint_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = true;

            if (btnAlterPoint.Visible == true)
            {
                // The alter point process is not complete
                args.IsValid = false;
            }
        }
    }
}
