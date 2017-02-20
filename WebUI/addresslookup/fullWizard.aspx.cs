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

namespace Orchestrator.WebUI.addresslookup
{
    /// <summary>
    /// Summary description for fullWizard.
    /// </summary>
    public partial class fullWizard : Orchestrator.Base.BasePage
    {
        protected override PageStatePersister PageStatePersister
        {
            get
            {
                return new HiddenFieldPageStatePersister(this);
            }
        }

        #region Form Elements

        protected TextBox txtBuildingNoName;
        protected RegularExpressionValidator revPostCode;

        #endregion

        #region Page Varibles

        private enum ePanel { Start, FindAddress, FindAddressNoPostCode, Results, Manual, Address };
        string accountCode = Globals.Configuration.PostCodeAnywhereAccountCode;
        string licenseKey = Globals.Configuration.PostCodeAnywhereLicenceKey;

        #region Manual Address Specification Controls


        #endregion

        #region Address Control Variables

        //protected string AddressLine1 = string.Empty;
        //protected string AddressLine2 = string.Empty;
        //protected string AddressLine3 = string.Empty;
        //protected string AddressLine4 = string.Empty;
        //protected string AddressLine5 = string.Empty;
        //protected string PostTown = string.Empty;
        //protected string County = string.Empty;
        //protected string PostCode = string.Empty;
        //protected string Longitude = string.Empty;
        //protected string Latitude = string.Empty;
        //protected string TrafficArea = string.Empty;

        protected string AddressLine1VAL = string.Empty;
        protected string AddressLine2VAL = string.Empty;
        protected string AddressLine3VAL = string.Empty;
        protected string AddressLine4VAL = string.Empty;
        protected string AddressLine5VAL = string.Empty;
        protected string PostTownVAL = string.Empty;
        protected string CountyVAL = string.Empty;
        protected string PostCodeVAL = string.Empty;
        protected string LongitudeVAL = string.Empty;
        protected string LatitudeVAL = string.Empty;
        protected string TrafficAreaVAL = string.Empty;
        protected string SetPointRadiusVAL = "true";

        #endregion

        #endregion

        #region Page Load/Init/Error

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.AddEditJob, eSystemPortion.TakeCallIn, eSystemPortion.AddEditPoints, eSystemPortion.AddEditOrganisations);

            if (!IsPostBack)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["PostCode"]))
                {
                    txtPostCodeOnly.Text = Request.QueryString["PostCode"];
                    FindAddressPostCode();
                    PopulateStaticControls();
                }
                else
                {
                    setPanel(ePanel.Start);
                    PopulateStaticControls();
                }
            }

            //AddressLine1 = Request.QueryString["AddressLine1"];
            //AddressLine2 = Request.QueryString["AddressLine2"];
            //AddressLine3 = Request.QueryString["AddressLine3"];
            //AddressLine4 = Request.QueryString["AddressLine4"];
            //AddressLine5 = Request.QueryString["AddressLine5"];
            //PostTown = Request.QueryString["PostTown"];
            //County = Request.QueryString["County"];
            //PostCode = Request.QueryString["PostCode"];
            //Longitude = Request.QueryString["longitude"];
            //Latitude = Request.QueryString["latitude"];
            //TrafficArea = Request.QueryString["TrafficArea"];

            btnNext.Attributes.Add("onClick", "javascript:HidePage();");
            btnSpecify.Visible = false;
        }

        protected void fullWizard_Init(object sender, EventArgs e)
        {
            this.rbOption.SelectedIndexChanged += new System.EventHandler(this.rbOption_SelectedIndexChanged);
            this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            this.btnSpecify.Click += new EventHandler(btnSpecify_Click);
        }

        #endregion

        #region Methods & Events

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

        protected void PopulateStaticControls()
        {
            Facade.IReferenceData facRef = new Facade.ReferenceData();
            DataSet ds = facRef.GetAllTrafficAreas();
            cboTrafficArea.DataTextField = "Description";
            cboTrafficArea.DataValueField = "TrafficAreaId";
            cboTrafficArea.DataSource = ds;
            cboTrafficArea.DataBind();
            cboTrafficArea.Items.Insert(0, new ListItem("Derive from postcode", "0"));
        }

        #region Button Events

        private void btnNext_Click(object sender, System.EventArgs e)
        {
            ePanel panelId = (ePanel)this.ViewState["panelId"];

            if (panelId != ePanel.Start)
                rbOption.SelectedIndex = -1;

            btnSpecify.Visible = !rfvAddress.IsValid;

            switch (panelId)
            {
                case ePanel.Start:
                    if (rbOption.SelectedValue != "")
                    {
                        setPanel((ePanel)int.Parse(rbOption.SelectedValue));
                        if ((ePanel)int.Parse(rbOption.SelectedValue) == ePanel.FindAddressNoPostCode)
                        {
                            if (Request.QueryString["forResource"] != null)
                            {
                                spanCompanyName.Visible = false;
                            }
                            if (Request.QueryString["searchCompany"] != null && Request.QueryString["searchCompany"].Length > 0)
                                txtCompanyName.Text = Request.QueryString["searchCompany"];
                            if (Request.QueryString["searchTown"].Length > 0)
                                txtTownName.Text = Request.QueryString["searchTown"];
                            if (Request.QueryString["searchStreet"] != null && Request.QueryString["searchStreet"].Length > 0)
                                txtStreetName.Text = Request.QueryString["streetName"];
                        }
                    }
                    break;
                case ePanel.FindAddressNoPostCode:
                    FindAddressNoPostCode();
                    break;
                case ePanel.FindAddress:
                    FindAddressPostCode();
                    break;
                case ePanel.Manual:
                    if (Page.IsValid)
                        DisplayManualAddress();
                    break;
                case ePanel.Results:
                    if (Page.IsValid)
                        DisplayAddress();
                    break;
            }
        }

        private void btnBack_Click(object sender, System.EventArgs e)
        {
            ePanel panelId = (ePanel)this.ViewState["panelId"];

            switch (panelId)
            {
                case ePanel.Start:
                    break;
                case ePanel.FindAddressNoPostCode:
                    setPanel(ePanel.Start);
                    break;
                case ePanel.Results:
                    setPanel(ePanel.Start);
                    break;
                case ePanel.FindAddress:
                    setPanel(ePanel.Start);
                    break;
                case ePanel.Address:
                    setPanel(ePanel.Start);
                    break;
                case ePanel.Manual:
                    setPanel(ePanel.Start);
                    break;
            }
        }

        private void btnSpecify_Click(object sender, EventArgs e)
        {
            setPanel(ePanel.Manual);
        }

        #endregion

        private void setPanel(ePanel PanelId)
        {
            switch (PanelId)
            {
                #region Start
                case ePanel.Start:
                    pnlStart.Visible = true;
                    pnlAddressNoPostCode.Visible = false;
                    pnlPostCodeOnly.Visible = false;
                    pnlSearchResults.Visible = false;
                    btnFinish.Visible = false;
                    pnlAddress.Visible = false;
                    pnlManual.Visible = false;
                    btnBack.Enabled = false;
                    btnNext.Visible = true;
                    break;
                #endregion

                #region Find Address No Post Code
                case ePanel.FindAddressNoPostCode:
                    pnlStart.Visible = false;
                    pnlAddressNoPostCode.Visible = true;
                    pnlAddress.Visible = false;
                    pnlPostCodeOnly.Visible = false;
                    pnlSearchResults.Visible = false;
                    pnlManual.Visible = false;
                    btnFinish.Visible = false;
                    btnBack.Enabled = true;
                    break;
                #endregion

                #region Find Address
                case ePanel.FindAddress:
                    pnlStart.Visible = false;
                    pnlAddressNoPostCode.Visible = false;
                    pnlAddress.Visible = false;
                    pnlPostCodeOnly.Visible = true;
                    pnlSearchResults.Visible = false;
                    pnlManual.Visible = false;
                    btnFinish.Visible = false;
                    btnBack.Enabled = true;

                    break;
                #endregion

                #region Results
                case ePanel.Results:
                    pnlStart.Visible = false;
                    pnlAddressNoPostCode.Visible = false;
                    pnlAddress.Visible = false;
                    pnlPostCodeOnly.Visible = false;
                    pnlSearchResults.Visible = true;
                    pnlManual.Visible = false;
                    btnFinish.Visible = false;
                    btnBack.Enabled = true;
                    break;
                #endregion

                #region Manual
                case ePanel.Manual:
                    pnlStart.Visible = false;
                    pnlAddressNoPostCode.Visible = false;
                    pnlAddress.Visible = false;
                    pnlPostCodeOnly.Visible = false;
                    pnlSearchResults.Visible = false;
                    pnlManual.Visible = true;
                    btnFinish.Visible = false;
                    btnBack.Enabled = true;
                    break;
                #endregion

                #region Address
                case ePanel.Address:
                    pnlStart.Visible = false;
                    pnlAddressNoPostCode.Visible = false;
                    pnlAddress.Visible = true;
                    pnlPostCodeOnly.Visible = false;
                    pnlSearchResults.Visible = false;
                    btnNext.Visible = false;
                    pnlManual.Visible = false;
                    btnBack.Enabled = true;
                    btnFinish.Visible = true;
                    break;
                #endregion
            }

            this.ViewState["panelId"] = PanelId;
        }

        private DataSet FindAddressNoPostCode(string companyName, string townName)
        {
            PostCodeAnywhere.LookupUK lookUp = new PostCodeAnywhere.LookupUK();

            DataSet ds = null;

            ds = lookUp.ByOrganisation_DataSet(companyName, townName, false, accountCode, licenseKey, "");

            return ds;
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

        private void SpecifyAddress()
        {
        }

        private void FindAddressNoPostCode()
        {
            DataSet ds;

            if (txtCompanyName.Text != "")
            {
                ds = FindAddressNoPostCode("%" + txtCompanyName.Text, "%" + txtTownName.Text);
            }
            else
            {
                ds = FindAddressNoPostCodeByStreet("%" + txtStreetName.Text, "%" + txtTownName.Text);
            }

            if (ds.Tables.Count > 0)
            {
                lstAddress.DataSource = ds.Tables[0].DefaultView;
                lstAddress.DataTextField = "description";
                lstAddress.DataValueField = "id";
                lstAddress.DataBind();

                btnSpecify.Visible = ds.Tables[0].Rows.Count == 0;
            }

            setPanel(ePanel.Results);
        }

        private DataSet FindAddressPostCode(int attempt, string postcode)
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
                    return FindAddressPostCode(attempt++, postcode);
                else
                    throw (ex);
            }
        }

        private void FindAddressPostCode()
        {
            DataSet ds = FindAddressPostCode(0, txtPostCodeOnly.Text);

            lstAddress.DataSource = ds.Tables[0].DefaultView;
            lstAddress.DataTextField = "description";
            lstAddress.DataValueField = "id";
            lstAddress.DataBind();
            setPanel(ePanel.Results);
        }

        private void rbOption_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            setPanel((ePanel)int.Parse(rbOption.SelectedValue));
            if ((ePanel)int.Parse(rbOption.SelectedValue) == ePanel.FindAddressNoPostCode)
            {
                if (Request.QueryString["forResource"] != null)
                {
                    spanCompanyName.Visible = false;
                }
                if (Request.QueryString["searchCompany"] != null && Request.QueryString["searchCompany"].Length > 0)
                    txtCompanyName.Text = Request.QueryString["searchCompany"];
                if (Request.QueryString["searchTown"].Length > 0)
                    txtTownName.Text = Request.QueryString["searchTown"];
                if (Request.QueryString["searchStreet"] != null && Request.QueryString["searchStreet"].Length > 0)
                    txtStreetName.Text = Request.QueryString["streetName"];
            }
        }

        private void DisplayAddress()
        {
            PostCodeAnywhere.LookupUK lookUp = new PostCodeAnywhere.LookupUK();
            PostCodeAnywhere.AddressResults address = null;

            address = lookUp.FetchAddress(lstAddress.SelectedValue, PostCodeAnywhere.enLanguage.enLanguageEnglish, PostCodeAnywhere.enContentType.enContentGeographicAddress, accountCode, licenseKey, "");

            string addressText = string.Empty;

            addressText = address.Results[0].OrganisationName;
            addressText = AddAddressLine(address.Results[0].Line1, addressText);
            addressText = AddAddressLine(address.Results[0].Line2, addressText);
            addressText = AddAddressLine(address.Results[0].Line3, addressText);
            addressText = AddAddressLine(address.Results[0].Line4, addressText);
            addressText = AddAddressLine(address.Results[0].Line5, addressText);
            addressText = AddAddressLine(address.Results[0].PostTown, addressText);
            addressText = AddAddressLine(address.Results[0].County, addressText);
            addressText = AddAddressLine(address.Results[0].Postcode, addressText);
            SetAddressValues(address.Results[0]);
            lblAddress.Text = addressText;

            // Display the points that share this points long/lat.
            Facade.IPoint facPoint = new Facade.Point();
            DataSet dsMatchingPoints = facPoint.GetAllForLongLat(address.Results[0].GeographicData.WGS84Longitude, address.Results[0].GeographicData.WGS84Latitude);
            if (dsMatchingPoints.Tables[0].Rows.Count > 0)
            {
                repExistingAddresses.DataSource = dsMatchingPoints;
                repExistingAddresses.DataBind();
                pnlExistingAddresses.Visible = true;
                pnlHideFinishButton.Visible = true;
            }
            else
            {
                pnlExistingAddresses.Visible = false;
                pnlHideFinishButton.Visible = false;
            }

            setPanel(ePanel.Address);

            lblGridReference.Text = address.Results[0].GeographicData.WGS84Latitude.ToString() + ", " + address.Results[0].GeographicData.WGS84Longitude.ToString();
        }

        private void DisplayManualAddress()
        {
            string addressText = String.Empty;
            addressText = AddAddressLine(txtAddressLine1.Text, addressText);
            addressText = AddAddressLine(txtAddressLine2.Text, addressText);
            addressText = AddAddressLine(txtAddressLine3.Text, addressText);
            addressText = AddAddressLine(txtTown.Text, addressText);
            addressText = AddAddressLine(txtCounty.Text, addressText);
            addressText = AddAddressLine(txtPostCode.Text, addressText);

            lblAddress.Text = addressText;

            AddressLine1VAL = txtAddressLine1.Text;
            AddressLine2VAL = txtAddressLine2.Text;
            AddressLine3VAL = txtAddressLine3.Text;
            AddressLine4VAL = String.Empty;
            AddressLine5VAL = String.Empty;
            PostTownVAL = txtTown.Text;
            CountyVAL = txtCounty.Text;
            PostCodeVAL = txtPostCode.Text;
            TrafficAreaVAL = cboTrafficArea.SelectedValue;
            LongitudeVAL = "0";
            LatitudeVAL = "0";
            
            setPanel(ePanel.Address);
            lblGridReference.Text = "0, 0";
        }

        private void SetAddressValues(PostCodeAnywhere.Address address)
        {
            AddressLine1VAL = address.Line1;
            AddressLine2VAL = address.Line2;
            AddressLine3VAL = address.Line3;
            AddressLine4VAL = address.Line4;
            AddressLine5VAL = address.Line5;
            PostTownVAL = address.PostTown;
            CountyVAL = address.County;
            PostCodeVAL = address.Postcode;
            TrafficAreaVAL = cboTrafficArea.SelectedValue;
            LongitudeVAL = address.GeographicData.WGS84Longitude.ToString();
            LatitudeVAL = address.GeographicData.WGS84Latitude.ToString();
        }

        private string AddAddressLine(string addressLine, string address)
        {
            if (addressLine.Length > 0)
            {
                address += "<br>" + addressLine;
            }
            return address;
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
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Init += new System.EventHandler(this.fullWizard_Init);

        }
        #endregion
    }
}
