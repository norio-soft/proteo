using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Orchestrator.Entities;
using Orchestrator.Globals;
using Orchestrator.WebUI.Controls;
using Orchestrator.Repositories;

using Telerik.Web.UI;

namespace Orchestrator.WebUI.Organisation
{
    /// <summary>
    /// Summary description for addupdateorganisation.
    /// </summary>
    public partial class addupdateorganisation : Orchestrator.Base.BasePage
    {
        protected string GetDescription
        {
            get
            {
                if (m_organisationType == eOrganisationType.Orchestrator)
                    return "Owner";
                else if (m_organisationType == eOrganisationType.SubContractor)
                    return "Sub-Contractor";
                else
                    return "Client";
            }
        }

        #region Constants & Enums

        private enum eReferenceMoveDirection { Down, Up };

        private const string C_ORGANISATION_VS = "Organisation";
        private const string C_ORGANISATION_CONTACTS_TABLE_VS = "OrganisationDataSet";
        private const string C_IDENTITY_ID_VS = "IdentityId";

        private const string C_ORGANISATION_EXPORT_MESSAGES_START_DATE = "ExportSettingsStartDate";
        private const string C_ORGANISATION_EXPORT_MESSAGES_END_DATE = "ExportSettingsEndDate";

        private const int C_MOVE_DOWN_CELL_ORDINAL = 5;
        private const int C_MOVE_UP_CELL_ORDINAL = 6;

        private const int C_MAX_LOG_COLUMNS_VS = 15;

        private const int C_TAB_DETAILS = 0;
        private const int C_TAB_REFERENCES = 1;
        private const int C_TAB_LOCATIONS = 2;
        private const int C_TAB_SETTINGS = 3;
        private const int C_TAB_LOG_SETTINGS = 4;
        private const int C_TAB_REPORT_SETTINGS = 5;
        private const int C_TAB_GROUPAGE = 6;
        private const int C_TAB_CLIENTS = 7;
        private const int C_TAB_CONTACTS = 8;
        private const int C_TAB_EXPORT_MESSAGES = 9;

        #endregion

        #region Page Variables
        private string accountCode = Orchestrator.Globals.Configuration.PostCodeAnywhereAccountCode;
        private string licenseKey = Orchestrator.Globals.Configuration.PostCodeAnywhereLicenceKey;
        private Entities.Organisation m_organisation;
        private static bool arrivalsBoardEnabledForHaulier = Orchestrator.Globals.Configuration.ArrivalsBoardEnabled;

        public DateTime? ExportMessagesStartDate
        {
            get
            {
                return ViewState[C_ORGANISATION_EXPORT_MESSAGES_START_DATE] as DateTime?;

            }
            set
            {
                ViewState[C_ORGANISATION_EXPORT_MESSAGES_START_DATE] = value;
            }
        }

        public DateTime? ExportMessagesEndDate
        {
            get
            {
                return ViewState[C_ORGANISATION_EXPORT_MESSAGES_END_DATE] as DateTime?;

            }
            set
            {
                ViewState[C_ORGANISATION_EXPORT_MESSAGES_END_DATE] = value;
            }
        }

        public DataSet OrganisationContactsDataSource
        {
            get
            {
                DataSet organisationContactsDataSource;
                object organisationContacts = ViewState[C_ORGANISATION_CONTACTS_TABLE_VS];

                if (organisationContacts != null)
                    organisationContactsDataSource = (DataSet)organisationContacts;
                else
                {
                    organisationContactsDataSource = this.PopulateOrganisationContacts();

                    ViewState[C_ORGANISATION_CONTACTS_TABLE_VS] = organisationContactsDataSource;
                }

                return organisationContactsDataSource;
            }
            set
            {
                ViewState[C_ORGANISATION_CONTACTS_TABLE_VS] = value;
            }
        }

        public IEnumerable<object> ExportMessagesDataSource { get; set; }

        protected string m_organisationName = String.Empty;
        protected int m_identityId = 0;
        protected int m_relatedIdentityId = 0;
        private eOrganisationType m_organisationType = eOrganisationType.Client;
        private bool m_isUpdate = false;
        private decimal? m_oldCreditLimit = null;


        protected string test;

        #endregion

        #region Tabbed Controls

        #region Client Details Tab

        protected TextBox tcOrganisationName
        {
            get { return ((TextBox)this.RadMultiPage1.FindControl("txtOrganisationName")); }
        }

        protected TextBox tcOrganisationDisplayName
        {
            get { return ((TextBox)this.RadMultiPage1.FindControl("txtOrganisationDisplayName")); }
        }

        protected TextBox tcMainTelephoneNumber
        {
            get { return ((TextBox)this.RadMultiPage1.FindControl("txtTelephone")); }
        }

        protected TextBox tcMainFaxNumber
        {
            get { return ((TextBox)this.RadMultiPage1.FindControl("txtFaxNumber")); }
        }

        protected TextBox tcAccountCode
        {
            get { return ((TextBox)this.RadMultiPage1.FindControl("txtAccountCode")); }
        }

        protected CheckBox tcIsSubContractor
        {
            get { return this.RadMultiPage1.FindControl("chkIsSubContractor") as CheckBox; }
        }

        protected CheckBox tcIsClient
        {
            get { return this.RadMultiPage1.FindControl("chkIsClient") as CheckBox; }
        }

        protected TextBox tcVATNumber
        {
            get { return this.RadMultiPage1.FindControl("txtVATNumber") as TextBox; }
        }

        protected Telerik.Web.UI.RadComboBox tcClosestTown
        {
            get { return ((Telerik.Web.UI.RadComboBox)this.RadMultiPage1.FindControl("cboClosestTown")); }
        }

        protected CustomValidator tcCustomValClosetTown
        {
            get { return ((CustomValidator)this.RadMultiPage1.FindControl("cfvTown")); }
        }

        protected TextBox tcAddressLine1
        {
            get { return ((TextBox)this.RadMultiPage1.FindControl("txtAddressLine1")); }
        }

        protected TextBox tcAddressLine2
        {
            get { return ((TextBox)this.RadMultiPage1.FindControl("txtAddressLine2")); }
        }

        protected TextBox tcAddressLine3
        {
            get { return ((TextBox)this.RadMultiPage1.FindControl("txtAddressLine3")); }
        }

        protected TextBox tcPostTown
        {
            get { return ((TextBox)this.RadMultiPage1.FindControl("txtPostTown")); }
        }

        protected TextBox tcCounty
        {
            get { return ((TextBox)this.RadMultiPage1.FindControl("txtCounty")); }
        }

        protected TextBox tcPostCode
        {
            get { return ((TextBox)this.RadMultiPage1.FindControl("txtPostCode")); }
        }

        protected HtmlInputHidden tcTrafficArea
        {
            get { return ((HtmlInputHidden)this.RadMultiPage1.FindControl("hidTrafficArea")); }
        }

        protected TextBox tcLongitude
        {
            get { return ((TextBox)this.RadMultiPage1.FindControl("txtLongitude")); }
        }

        protected TextBox tcLatitude
        {
            get { return ((TextBox)this.RadMultiPage1.FindControl("txtLatitude")); }
        }

        protected LinkButton tcLookUp
        {
            get { return ((LinkButton)this.RadMultiPage1.FindControl("lnkLookUp")); }
        }
        protected ListBox tcAddressList
        {
            get { return ((ListBox)this.RadMultiPage1.FindControl("lstAddress")); }
        }

        protected Panel tcAddressPanel
        {
            get { return ((Panel)this.RadMultiPage1.FindControl("pnlAddress")); }
        }

        protected Panel tcAddressListPanel
        {
            get { return ((Panel)this.RadMultiPage1.FindControl("pnlAddressList")); }
        }


        protected CheckBox tcClientSuspended
        {
            get { return ((CheckBox)this.RadMultiPage1.FindControl("chkSuspended")); }
        }

        protected TextBox tcClientSuspendedReason
        {
            get { return ((TextBox)this.RadMultiPage1.FindControl("txtSuspendedReason")); }
        }

        protected CheckBox tcClientOnHold
        {
            get { return ((CheckBox)this.RadMultiPage1.FindControl("chkOnHold")); }
        }

        protected TextBox tcClientOnHoldReason
        {
            get { return ((TextBox)this.RadMultiPage1.FindControl("txtOnHoldReason")); }
        }

        protected System.Web.UI.HtmlControls.HtmlAnchor tcFindAddressLink
        {
            get { return ((System.Web.UI.HtmlControls.HtmlAnchor)this.RadMultiPage1.FindControl("findAddressLink")); }
        }
        #endregion

        #region References Tab

        protected TextBox tcLoadNumberText
        {
            get { return (TextBox)this.RadMultiPage1.FindControl("txtLoadNumberText"); }
        }

        protected TextBox tcDocketNumberText
        {
            get { return (TextBox)this.RadMultiPage1.FindControl("txtDocketNumberText"); }
        }

        protected DataGrid tcReferences
        {
            get { return (DataGrid)this.RadMultiPage1.FindControl("dgReferences"); }
        }

        protected Button tcAddReference
        {
            get { return (Button)this.RadMultiPage1.FindControl("btnAddReference"); }
        }

        protected RadioButton tcUseAsDefaultLoadNo
        {
            get { return (RadioButton)this.RadMultiPage1.FindControl("rbUseAsDefaultLoadNo"); }
        }

        protected RadioButton tcUseAsDefaultDockeNo
        {
            get { return (RadioButton)this.RadMultiPage1.FindControl("rbUseAsDefaultDocketNo"); }
        }

        #endregion

        #region Locations Tab

        protected DataGrid tcLocations
        {
            get { return (DataGrid)this.RadMultiPage1.FindControl("dgLocations"); }
        }

        protected Telerik.Web.UI.RadGrid tcLocationsGrid
        {
            get { return (Telerik.Web.UI.RadGrid)this.RadMultiPage1.FindControl("grdLocations"); }
        }

        protected CheckBox tcShowDeletedReferences
        {
            get { return (CheckBox)this.RadMultiPage1.FindControl("chkShowDeletedReferences"); }
        }

        protected Button tcViewPoints
        {
            get { return (Button)this.RadMultiPage1.FindControl("btnViewPoints"); }
        }

        protected Button tcAddLocation
        {
            get { return (Button)this.RadMultiPage1.FindControl("btnAddLocation"); }
        }

        #endregion

        #region Settings Tab

        protected Telerik.Web.UI.RadComboBox tcPoint
        {
            get { return ((Telerik.Web.UI.RadComboBox)this.RadMultiPage1.FindControl("cboDefaultCollectionPoint")); }
        }

        protected DropDownList tcDefaultInvoiceType
        {
            get { return ((DropDownList)this.RadMultiPage1.FindControl("cboInvoiceType")); }
        }

        protected DropDownList tcDemurrageType
        {
            get { return ((DropDownList)this.RadMultiPage1.FindControl("cboDemurrageType")); }
        }

        protected TextBox tcRateTariffCard
        {
            get { return ((TextBox)this.RadMultiPage1.FindControl("txtRateTariffCard")); }
        }

        protected CheckBox tcIncludePODs
        {
            get { return ((CheckBox)this.RadMultiPage1.FindControl("chkIncludePODs")); }
        }

        protected CheckBox tcIncludeReferences
        {
            get { return ((CheckBox)this.RadMultiPage1.FindControl("chkIncludeReferences")); }
        }

        protected CheckBox tcIncludeInvoiceRunId
        {
            get { return ((CheckBox)this.RadMultiPage1.FindControl("chkIncludeInvoiceRunId")); }
        }

        protected CheckBox tcIsExcludedFromInvoicing
        {
            get { return ((CheckBox)this.RadMultiPage1.FindControl("chkIsExcludedFromInvoicing")); }
        }

        protected CheckBox tcIncludeDemurrage
        {
            get { return ((CheckBox)this.RadMultiPage1.FindControl("chkIncludeDemurrage")); }
        }

        protected RadioButton radioFuelSurchargeDisplayInclude
        {
            get { return ((RadioButton)this.RadMultiPage1.FindControl("rdFuelSurchargeDisplayInclude")); }
        }

        protected RadioButton radioFuelSurchargeHideInclude
        {
            get { return ((RadioButton)this.RadMultiPage1.FindControl("rdFuelSurchargeHideInclude")); }
        }

        protected RadioButton radioFuelSurchargeNeither
        {
            get { return ((RadioButton)this.RadMultiPage1.FindControl("rdFuelSurchargeNeither")); }
        }

        protected CheckBox tcIncludeWeights
        {
            get { return ((CheckBox)this.RadMultiPage1.FindControl("chkIncludeWeights")); }
        }

        protected CheckBox tcIncludePallets
        {
            get { return ((CheckBox)this.RadMultiPage1.FindControl("chkIncludePallets")); }
        }

        protected CheckBox tcIncludePalletType
        {
            get { return ((CheckBox)this.RadMultiPage1.FindControl("chkIncludePalletType")); }
        }

        protected CheckBox tcIncludeVatTotal
        {
            get { return ((CheckBox)this.RadMultiPage1.FindControl("chkIncludeVatTotal")); }
        }

        protected CheckBox tcIncludeJobDetails
        {
            get { return ((CheckBox)this.RadMultiPage1.FindControl("chkIncludeJobDetails")); }
        }

        protected CheckBox tcIncludeExtraDetails
        {
            get { return ((CheckBox)this.RadMultiPage1.FindControl("chkIncludeExtraDetails")); }
        }

        protected CheckBox tcIncludeSurcharges
        {
            get { return ((CheckBox)this.RadMultiPage1.FindControl("chkIncludeSurcharges")); }
        }

        protected CheckBox tcIncludeInstructionNotes
        {
            get { return ((CheckBox)this.RadMultiPage1.FindControl("chkIncludeInstructionNotes")); }
        }

        protected CheckBox tcIncludeJobExtras
        {
            get { return ((CheckBox)this.RadMultiPage1.FindControl("chkIncludeJobExtras")); }
        }

        protected CheckBox tcShowGoodsRefusal
        {
            get { return ((CheckBox)this.RadMultiPage1.FindControl("chkShowGoodsRefusal")); }
        }

        protected CheckBox tcIncludeServiceLevel
        {
            get { return ((CheckBox)this.RadMultiPage1.FindControl("chkIncludeServiceLevel")); }
        }

        protected DropDownList tcDefaultJobType
        {
            get { return ((DropDownList)this.RadMultiPage1.FindControl("cboJobType")); }
        }

        protected TextBox tcTippingThreshold
        {
            get { return ((TextBox)this.RadMultiPage1.FindControl("txtTippingThreshold")); }
        }

        protected TextBox tcDemurrageChargeRate
        {
            get { return ((TextBox)this.RadMultiPage1.FindControl("txtDemmurageChargeRate")); }
        }

        protected TextBox tcPalletNumberThreshold
        {
            get { return ((TextBox)this.RadMultiPage1.FindControl("txtPalletNumberThreshold")); }
        }

        protected HyperLink tcAlterPalletBalance
        {
            get { return ((HyperLink)this.RadMultiPage1.FindControl("hlAlterPalletBalance")); }
        }

        protected TextBox tcPalletPenaltyCharge
        {
            get { return ((TextBox)this.RadMultiPage1.FindControl("txtPalletPenaltyCharge")); }
        }

        protected TextBox tcCHEPNumber
        {
            get { return ((TextBox)this.RadMultiPage1.FindControl("txtCHEPNumber")); }
        }

        protected GridView tcPalletType
        {
            get { return ((GridView)this.RadMultiPage1.FindControl("gvPalletType")); }
        }

        protected ListView tcLinkedClients
        {
            get { return ((ListView)this.RadMultiPage1.FindControl("lvLinkedClients")); }
        }

        protected RadNumericTextBox radNumTxt_FuelSurchargeOverride
        {
            get { return ((RadNumericTextBox)this.RadMultiPage1.FindControl("rntFuelSurchargeOverride")); }
        }

        protected RadNumericTextBox radNumTxt_FuelSurchargeAdjustment
        {
            get { return ((RadNumericTextBox)this.RadMultiPage1.FindControl("rntFuelSurchargeAdjustment")); }
        }

        protected DropDownList tcFuelSurchargeBreakdownType
        {
            get { return ((DropDownList)this.RadMultiPage1.FindControl("cboFuelSurchargeBreakDownType")); }
        }

        protected RadioButton radioFuelSurchargeOverride
        {
            get { return ((RadioButton)this.RadMultiPage1.FindControl("rdFuelSurchargeOverride")); }
        }

        protected RadioButton radioFuelSurchargeAdjustment
        {
            get { return ((RadioButton)this.RadMultiPage1.FindControl("rdFuelSurchargeAdjustment")); }
        }

        protected RadioButton radioFuelSurchargeStandard
        {
            get { return ((RadioButton)this.RadMultiPage1.FindControl("rdFuelSurchargeStandard")); }
        }

        protected CheckBox tcFuelSurchargeOnExtras
        {
            get { return ((CheckBox)this.RadMultiPage1.FindControl("chkFuelSurchargeOnExtras")); }
        }

        protected GridView tcGoodsType
        {
            get { return ((GridView)this.RadMultiPage1.FindControl("gvGoodsType")); }
        }

        protected TextBox tcPaymentTerms
        {
            get { return ((TextBox)this.RadMultiPage1.FindControl("txtPaymentTerms")); }
        }

        protected CheckBox tcMustCaptureRates
        {
            get { return ((CheckBox)this.RadMultiPage1.FindControl("chkMustCaptureRates")); }
        }

        protected CheckBox tcGPSAutoCallIn
        {
            get { return this.RadMultiPage1.FindControl("chkGPSAutoCallIn") as CheckBox; }
        }

        protected CheckBox tcMustCaptureDebrief
        {
            get { return this.RadMultiPage1.FindControl("chkMustCaptureDebrief") as CheckBox; }
        }

        protected CheckBox tcMustCaptureCollectionDebrief
        {
            get { return this.RadMultiPage1.FindControl("chkMustCaptureCollectionDebrief") as CheckBox; }
        }

        protected TextBox tcEarlyMinutes
        {
            get { return this.RadMultiPage1.FindControl("txtEarlyMinutes") as TextBox; }
        }

        protected TextBox tcLateMinutes
        {
            get { return this.RadMultiPage1.FindControl("txtLateMinutes") as TextBox; }
        }

        protected DropDownList tcDefaultBusinessType
        {
            get { return this.RadMultiPage1.FindControl("cboDefaultBusinessType") as DropDownList; }
        }

        protected TextBox tcDefaultNoPallets
        {
            get { return this.RadMultiPage1.FindControl("txtDefaultNumberOfPallets") as TextBox; }
        }

        protected CheckBox tcAutoEmailInvoices
        {
            get { return this.RadMultiPage1.FindControl("chkAutoEmailInvoices") as CheckBox; }
        }

        protected CheckBox tcInvoiceAttachCSV
        {
            get { return this.RadMultiPage1.FindControl("chkInvoiceAttachCSV") as CheckBox; }
        }

        protected TextBox tcInvoiceEmailAddress
        {
            get { return this.RadMultiPage1.FindControl("txtInvoiceEmailAddress") as TextBox; }
        }

        protected CustomValidator tcCFVInvoiceEmailAddress
        {
            get { return this.RadMultiPage1.FindControl("cfvInvoiceEmailAddress") as CustomValidator; }
        }

        protected DropDownList tcInvoiceGroupType
        {
            get { return this.RadMultiPage1.FindControl("cboInvoiceGroupType") as DropDownList; }
        }

        protected RadNumericTextBox tcInvoiceGroupValue
        {
            get { return this.RadMultiPage1.FindControl("rntInvoiceGroupValue") as RadNumericTextBox; }
        }

        #endregion

        #region Log Settings Tab

        protected DropDownList tcLogFrequency
        {
            get { return ((DropDownList)this.RadMultiPage1.FindControl("cboLogFrequency")); }
        }

        protected Panel tcPanelDay
        {
            get { return ((Panel)this.RadMultiPage1.FindControl("pnlDay")); }
        }

        protected DropDownList tcDay
        {
            get { return ((DropDownList)this.RadMultiPage1.FindControl("cboDay")); }
        }

        protected Telerik.Web.UI.RadDateInput tcDeliverLogBy
        {
            get { return ((Telerik.Web.UI.RadDateInput)this.RadMultiPage1.FindControl("dteDeliverLogBy")); }
        }

        protected Telerik.Web.UI.RadDatePicker tcExportMessagesStartDate
        {
            get; set;
        }
        
        protected Telerik.Web.UI.RadDatePicker tcExportMessagesEndDate
        {
            get; set;
        }


        protected TextBox tcWarningsElapsed
        {
            get { return ((TextBox)this.RadMultiPage1.FindControl("txtWarningElapsedTime")); }
        }

        protected TextBox tcDefaultLogColumns
        {
            get { return ((TextBox)this.RadMultiPage1.FindControl("txtDefaultLog")); }
        }

        protected Button tcMoveColumnUp
        {
            get { return ((Button)this.RadMultiPage1.FindControl("btnMoveColumnUp")); }
        }

        protected Button tcMoveColumnDown
        {
            get { return ((Button)this.RadMultiPage1.FindControl("btnMoveColumnDown")); }
        }

        protected ListBox tcIncludedColumns
        {
            get { return ((ListBox)this.RadMultiPage1.FindControl("lbIncludedColumns")); }
        }

        protected ListBox tcExcludedColumns
        {
            get { return ((ListBox)this.RadMultiPage1.FindControl("lbExcludedColumns")); }
        }

        protected Button tcUnassignColumn
        {
            get { return ((Button)this.RadMultiPage1.FindControl("btnUnassignColumn")); }
        }

        protected Button tcAssignColumn
        {
            get { return ((Button)this.RadMultiPage1.FindControl("btnAssignColumn")); }
        }

        protected Label tcColumnError
        {
            get { return ((Label)this.RadMultiPage1.FindControl("lblColumnError")); }
        }
        #endregion

        #region Report Settings Tab

        protected DataGrid tcMPGeneralResourceInstructions
        {
            get { return (DataGrid)this.RadMultiPage1.FindControl("dgMPGeneralResourceInstructions"); }
        }

        protected DataGrid tcMPContactNumbers
        {
            get { return (DataGrid)this.RadMultiPage1.FindControl("dgMPContactNumbers"); }
        }

        #endregion

        #region Groupage Settings Tab

        protected Telerik.Web.UI.RadComboBox tccboGroupageDefaultCollectionPoint
        {
            get { return (Telerik.Web.UI.RadComboBox)this.RadMultiPage1.FindControl("cboPoint"); }
        }

        protected Telerik.Web.UI.RadComboBox tccboGroupageDefaultSort
        {
            get { return (Telerik.Web.UI.RadComboBox)this.RadMultiPage1.FindControl("cboDefaultSort"); }
        }

        protected Telerik.Web.UI.RadComboBox tccboDefaultAttemptedDeliveryReturnPoint
        {
            get { return (Telerik.Web.UI.RadComboBox)this.RadMultiPage1.FindControl("cboDefaultAttemptedDeliveryReturnPoint"); }
        }

        #endregion

        #region Clients Tab

        protected Telerik.Web.UI.RadGrid tcClientsGrid
        {
            get { return this.RadMultiPage1.FindControl("grdClients") as Telerik.Web.UI.RadGrid; }
        }


        #endregion

        #region Contacts Tab

        protected Telerik.Web.UI.RadGrid tcContactsGrid
        {
            get { return this.RadMultiPage1.FindControl("grdContacts") as Telerik.Web.UI.RadGrid; }
        }

        #endregion

        #region ExportMessages Tab

        protected Telerik.Web.UI.RadGrid tcExportMessagesGrid
        {
            get { return this.RadMultiPage1.FindControl("grdExportMessages") as Telerik.Web.UI.RadGrid; }
        }

        #endregion


        #endregion

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.AddEditOrganisations, eSystemPortion.GeneralUsage);
            btnAdd.Enabled = Orchestrator.WebUI.Security.Authorise.CanAccess(eSystemPortion.AddEditOrganisations);
            this.txtCreditLimit.ReadOnly = !Orchestrator.WebUI.Security.Authorise.CanAccess(eSystemPortion.AddEditCreditManagement);

            // Setup
            ResetPageDisplay();
            RetrieveQueryStringParameters();
            //SetWhereIAm();
            if (!IsPostBack)
            {
                this.CboCountryLoad();
                PopulateStaticControls();
                PopulateNominalCodes();

                if (m_isUpdate)
                    LoadOrganisation();

                ConfigureUI();
                ConfigureButtons();
            }
            else
            {
                if (m_isUpdate)
                {
                    // Retrieve the organisation details from the viewstate
                    m_organisation = (Entities.Organisation)ViewState[C_ORGANISATION_VS];

                    if (m_organisation == null)
                    {
                        LoadOrganisation();
                    }
                    m_organisationName = m_organisation.OrganisationName;
                    m_organisationType = m_organisation.OrganisationType;
                }

                PopulateStaticControls();
                //PopulateReferences();
                PopulateLocations();
                PopulateReportSettings();
                ConfigureUI();
            }

            infringementDisplay.Visible = true;
            tcIncludePalletType.Visible = Globals.Configuration.PalletTypeIncludedInInvoice;
            
        }

        private bool AddOrganisation(IDictionary<int, int> businessTypeTariffs)
        {
            int identityId = 0;
            bool success = false;
            Entities.FacadeResult retVal = new Entities.FacadeResult(0);

            Facade.IOrganisation facOrganisation = new Facade.Organisation();
            string userId = ((Entities.CustomPrincipal)Page.User).UserName;

            if (m_relatedIdentityId > 0)
                retVal = facOrganisation.Create(m_relatedIdentityId, m_organisation, businessTypeTariffs, userId);
            else
                retVal = facOrganisation.Create(m_organisation, businessTypeTariffs, userId);

            if (!retVal.Success)
            {
                infringementDisplay.Infringements = retVal.Infringements;
                infringementDisplay.DisplayInfringments();
                infringementDisplay.Visible = true;

                imgIcon.ImageUrl = "~/images/ico_critical.gif";

                lblConfirmation.Text = "There was an error adding the organisation, please try again.";
                lblConfirmation.Visible = true;
                pnlConfirmation.Visible = true;
                lblConfirmation.ForeColor = Color.Red;
                success = false;
            }
            else
            {
                if (retVal.Infringements.Count > 0)
                {
                    infringementDisplay.Infringements = retVal.Infringements;
                    infringementDisplay.DisplayInfringments();
                    infringementDisplay.Visible = true;

                    imgIcon.ImageUrl = "~/images/ico_critical.gif";

                    lblConfirmation.Text = "The Organisation has been successfully added. However, there was an error adding a matching organisation to your 3rd party integrated system.";
                    lblConfirmation.Visible = true;
                    pnlConfirmation.Visible = true;
                    lblConfirmation.ForeColor = Color.Red;
                }
                else
                {
                    lblConfirmation.Text = "The Organisation has been successfully added.";
                    infringementDisplay.Visible = false;
                }

                identityId = retVal.ObjectId;
                m_identityId = identityId;

                success = true;

                // Load the Account Code that has been issued from the accounts
                string accountCode = facOrganisation.GetAccountCodeForIdentityId(m_organisation.IdentityId);
                tcAccountCode.Text = accountCode;

                this.m_oldCreditLimit = null;
                if (m_organisation.CreditLimit != null)
                    try
                    {
                        Facade.IUser facUser = new Facade.User();
                        facUser.SendEmailToUsersInRole((int)eUserRole.CreditManagement,
                            "Credit Management Alert", this.GenerateCreditManagementEmail());
                    }
                    catch
                    {
                        // log that we couldn't send an email.
                    }
            }

            return success;
        }

        private void ConfigureButtons()
        {
            // Toggle the button display and attributes (certain buttons should only be enabled when in update mode).
            string confirmMessage = @"javascript:return(confirm('Any changes you have made will need to be saved before you continue.\nTo save your changes, click Cancel, the click the " + (m_isUpdate ? "Update" : "Add") + " button, you may then try again.'))";
            tcAddReference.Enabled = m_isUpdate;
            tcAddReference.Attributes.Add("onClick", confirmMessage);
            tcAddLocation.Enabled = m_isUpdate;
            tcViewPoints.Enabled = m_isUpdate;
            tcAddLocation.Attributes.Add("onClick", confirmMessage);
        }

        private void ConfigureUI()
        {
            // Hide pcv configuration panel.
            pcvConfiguration.Style.Add("display", "none");

            this.lblLoadNumber.Text = Globals.Configuration.SystemLoadNumberText;
            this.lblDocketNumber.Text = Globals.Configuration.SystemDocketNumberText;

            if (m_organisationType == eOrganisationType.Client)
            {
                // Hide tabs
                this.RadTabStrip1.Tabs[C_TAB_REPORT_SETTINGS].Visible = false;
                this.RadTabStrip1.Tabs[C_TAB_GROUPAGE].Visible = false;

                if (m_organisation != null && m_organisation.IsSubContractor)
                {
                    tcIsSubContractor.Checked = true;
                    tcIsSubContractor.Enabled = true;
                }

                btnPromotetoClient.Visible = false;
            }

            if (m_organisationType == eOrganisationType.SubContractor)
            {
                #region Switch off some validation controls

                //				((RequiredFieldValidator) UltraWebTab1.FindControl("rfvTippingThreshold")).Enabled = false;
                //				((CustomValidator) UltraWebTab1.FindControl("cfvTippingThreshold")).Enabled = false;
                //				((RequiredFieldValidator) UltraWebTab1.FindControl("rfvDemmurageChargeRate")).Enabled = false;
                //				((CustomValidator) UltraWebTab1.FindControl("cfvDemmurageChargeRate")).Enabled = false;
                //				((RequiredFieldValidator) UltraWebTab1.FindControl("rfvPalletNumberThreshold")).Enabled = false;
                //				((CustomValidator) UltraWebTab1.FindControl("cfvPalletNumberThreshold")).Enabled = false;
                //				((RequiredFieldValidator) UltraWebTab1.FindControl("rfvPalletPenaltyCharge")).Enabled = false;
                //				((CustomValidator) UltraWebTab1.FindControl("cfvPalletPenaltyCharge")).Enabled = false;
                //((RequiredFieldValidator) UltraWebTab1.FindControl("rfvFuelSurcharge")).Enabled = false;
                //((CustomValidator)RadMultiPage1.FindControl("cfvFuelSurcharge")).Enabled = false;
                ((RequiredFieldValidator)RadMultiPage1.FindControl("rfvDeliverLogBy")).Enabled = false;
                //((RequiredFieldValidator) RadMultiPage1.FindControl("rfvDeliverLogBy")).Enabled = false;
                ((RequiredFieldValidator)RadMultiPage1.FindControl("rfvWarningElapsedTime")).Enabled = false;
                ((CustomValidator)RadMultiPage1.FindControl("cfvWarningElapsedTime")).Enabled = false;

                #endregion

                // Hide various non-subcontractor elements
                this.RadTabStrip1.Tabs[C_TAB_REFERENCES].Visible = false;
                this.RadTabStrip1.Tabs[C_TAB_GROUPAGE].Visible = false;
                this.RadTabStrip1.Tabs[C_TAB_LOG_SETTINGS].Visible = false;
                this.RadTabStrip1.Tabs[C_TAB_REPORT_SETTINGS].Visible = false;
                //always show Theme settings tab
                //this.RadTabStrip1.Tabs[C_TAB_SETTINGS].Visible = false;
                this.RadTabStrip1.Tabs[C_TAB_LOCATIONS].Visible = false;

                // Alter some ui elements
                //Header.Title = Header.Title.Replace("Client", "Sub-Contractor");
                //Header.SubTitle = Header.SubTitle.Replace("Client", "Sub-Contractor");
                btnListClients.Text = btnListClients.Text.Replace("Clients", "Sub-Contractors");
                tcIsSubContractor.Enabled = false;

            }
            else if (m_identityId == Orchestrator.Globals.Configuration.IdentityId)
            {
                pcvConfiguration.Style.Add("display", "");
                tcShowDeletedReferences.Visible = m_isUpdate;
            }

            if (m_isUpdate)
                btnAdd.Text = "Update";

            // If new and clients should be suspended until documents scanned flag the client as suspended now
            if (!m_isUpdate && !IsPostBack && Globals.Configuration.ClientSuspendedPendingDocuments)
            {
                chkSuspended.Checked = true;
                chkSuspended.ToolTip = "New clients should be set as Suspended until their documents have been scanned";
            }
            else
                chkSuspended.ToolTip = string.Empty;

            this.tbwmVatNumber.TargetControlID = this.RadMultiPage1.FindControl("txtVATNumber").UniqueID;

            this.chkAutoEmailInvoices.Text = (Orchestrator.Globals.Configuration.SendInvoiceEmailOnPost) ? "Email Invoice when Posted to" : "Email Invoice when Approved to";

            if(Orchestrator.Globals.Configuration.FleetMetrikInstance)
            {
                this.RadTabStrip1.Tabs[C_TAB_REFERENCES].Visible = false;
                this.RadTabStrip1.Tabs[C_TAB_LOCATIONS].Visible = false;
                this.RadTabStrip1.Tabs[C_TAB_SETTINGS].Visible = false;
                this.RadTabStrip1.Tabs[C_TAB_REPORT_SETTINGS].Visible = false;
                this.RadTabStrip1.Tabs[C_TAB_GROUPAGE].Visible = false;
                this.RadTabStrip1.Tabs[C_TAB_CLIENTS].Visible = false;
                this.RadTabStrip1.Tabs[C_TAB_EXPORT_MESSAGES].Visible = false;

                btnListClients.Visible = false;
                tdFinancial.Visible = false;
                tdClientCulture.Visible = false;

            }

            this.divArrivalsBoard.Visible = arrivalsBoardEnabledForHaulier;

            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var organisationRepo = DIContainer.CreateRepository<IOrganisationRepository>(uow);
                tcLinkedClients.DataBind();

                if (m_identityId != 0)
                {
                    var thisOrg = organisationRepo.Find(m_identityId);
                    tcLinkedClients.DataSource = thisOrg.LinkedClients.OrderBy(o => o.OrganisationName);
                    tcLinkedClients.DataBind();
                }
            }
        }

        private void LoadOrganisation()
        {
            // Retrieve the organisation and place it in viewstate
            Facade.IOrganisation facOrganisation = new Facade.Organisation();

            if (m_identityId != 0)
            {
                m_organisation = facOrganisation.GetForIdentityId(m_identityId);
                m_organisationName = m_organisation.OrganisationName;
                m_organisationType = m_organisation.OrganisationType;

                lblOrganisationName.Text = m_organisationName;

                switch (m_organisationType)
                {
                    case eOrganisationType.ClientCustomer:
                        Response.Redirect("addupdateclientscustomer.aspx?identityId=" + m_identityId);
                        break;
                }

                ViewState[C_ORGANISATION_VS] = m_organisation;

                PopulateClientDetails();
                PopulateReferences();
                PopulateLocations();
                PopulateExportSettings();
                PopulateDefaults();
                PopulateClients();
                PopulateReportSettings();
                PopulateGroupageSettings();
                PopulateOrganisationContacts();
                PopulateAddOrderSettings();
                PopulateConsortiumSettings();

                SwitchUIToUpdate();
            }

        }

        private void PopulateAddOrderSettings()
        {
            if (m_organisation.Defaults != null && m_organisation.Defaults.Count > 0)
            {
                if (m_organisation.Defaults[0].DefaultBusinessTypeID > 0)
                    tcDefaultBusinessType.Items.FindByValue(m_organisation.Defaults[0].DefaultBusinessTypeID.ToString()).Selected = true;

                tcDefaultNoPallets.Text = m_organisation.Defaults[0].DefaultNumberOfPallets == null ? string.Empty : m_organisation.Defaults[0].DefaultNumberOfPallets.Value.ToString();
            }
        }

        private void PopulateConsortiumSettings()
        {
            int? allocationRuleSetID = null;
            bool excludeFromAutoSubcontracting = false;

            if (m_organisation.Defaults != null && m_organisation.Defaults.Count > 0)
            {
                var defaults = m_organisation.Defaults[0];
                allocationRuleSetID = defaults.AllocationRuleSetID;
                excludeFromAutoSubcontracting = defaults.ExcludeFromAutoSubcontracting;
            }

            RadComboBoxItem item = null;
            if (allocationRuleSetID.HasValue)
                item = cboAllocationRuleSet.FindItemByValue(allocationRuleSetID.Value.ToString());

            item = item ?? cboAllocationRuleSet.Items[0];
            item.Selected = true;

            chkExcludeFromAutoSubcontracting.Checked = excludeFromAutoSubcontracting;
        }

        private void PopulateNominalCodes()
        {
            Facade.INominalCode facNominalCode = new Facade.NominalCode();
            DataSet dsNominalCode = facNominalCode.GetAllActive();

            DataTable dt = dsNominalCode.Tables[0];
            DataRow dr = dt.NewRow();
            dr["NominalCodeID"] = "0";
            dr["Description"] = "Use Default";
            dt.Rows.InsertAt(dr, 0);

            this.cboNominalCode.DataSource = dt;
            this.cboNominalCode.DataTextField = "Description";
            this.cboNominalCode.DataValueField = "NominalCodeID";
            this.cboNominalCode.DataBind();

            this.cboNominalCode.Items[0].Selected = true;

        }

        private void PopulateOrganisation(out IDictionary<int, int> businessTypeTariffs)
        {
            businessTypeTariffs = null;

            if (m_isUpdate)
            {
                // Recover from viewstate
                m_organisation = (Entities.Organisation)ViewState[C_ORGANISATION_VS];
            }
            else
            {
                // Create a new organisation
                m_organisation = new Entities.Organisation();
            }

            //--------------------------------------------------------------------------
            // 1. Populate the Organisation
            //--------------------------------------------------------------------------
            #region Populate the Organisation
            this.m_oldCreditLimit = m_organisation.CreditLimit;

            m_organisation.OrganisationName = tcOrganisationName.Text;
            m_organisation.OrganisationDisplayName = tcOrganisationDisplayName.Text;
            m_organisation.OrganisationType = m_organisationType;
            m_organisation.AccountCode = tcAccountCode.Text;
            m_organisation.LoadNumberText = tcLoadNumberText.Text;
            m_organisation.DocketNumberText = tcDocketNumberText.Text;

            if (tcUseAsDefaultLoadNo.Checked)
                m_organisation.ReferenceDefault = "Load Number";
            else
                m_organisation.ReferenceDefault = "Docket Number";

            if (tcClientSuspended.Checked)
            {
                m_organisation.IdentityStatus = eIdentityStatus.Deleted;
                m_organisation.SuspendedReason = tcClientSuspendedReason.Text;
            }
            else
            {
                m_organisation.IdentityStatus = eIdentityStatus.Active;
                m_organisation.SuspendedReason = String.Empty;
            }

            if (tcClientOnHold.Checked)
            {
                m_organisation.OnHold = true;
                m_organisation.OnHoldReason = tcClientOnHoldReason.Text;
            }
            else
            {
                m_organisation.OnHoldReason = String.Empty;
                m_organisation.OnHold = false;
            }

            m_organisation.IsSubContractor = tcIsSubContractor.Checked;
            m_organisation.VATNumber = tcVATNumber.Text;
            m_organisation.LCID = int.Parse(rcbClientCulture.SelectedValue);

            if (this.txtCreditLimit.Value == null)
                m_organisation.CreditLimit = null;
            else
                m_organisation.CreditLimit = Convert.ToDecimal(this.txtCreditLimit.Value);

            if (cboNominalCode.SelectedValue == "0")
                m_organisation.NominalCodeId = null;
            else
                m_organisation.NominalCodeId = int.Parse(cboNominalCode.SelectedValue);

            // Note: cost centre was added primarily for JR exchequer integration. 
            // For JR, the Cost Centre is not passed to exchequer when updating the organisation,
            // it is only passed when posting invoices.
            m_organisation.CostCentre = this.txtCostCentre.Text;

            #endregion

            //--------------------------------------------------------------------------
            // 2. Populate the Individual
            //--------------------------------------------------------------------------
            #region Populate the IndividualContacts

            List<Entities.Individual> individualContacts = null;
            DataSet contacts = this.OrganisationContactsDataSource;

            if (!m_isUpdate)
                if (m_organisation.IndividualContacts == null || m_organisation.IndividualContacts.Count == 0)
                {
                    individualContacts = new List<Entities.Individual>();

                    foreach (DataRow row in contacts.Tables["organisationContacts"].Rows)
                    {
                        Entities.Individual individualContact = this.GetIndividual(0);

                        this.UpdateIndividual(ref individualContact, row);

                        individualContacts.Add(individualContact);
                    }

                    m_organisation.IndividualContacts = individualContacts;
                }

            #endregion

            //--------------------------------------------------------------------------
            // 3. Populate the Address
            //--------------------------------------------------------------------------
            #region Populate the Address

            Entities.OrganisationLocation headOffice = null;
            if (m_organisation.Locations != null)
                headOffice = m_organisation.Locations.GetHeadOffice();
            else
            {
                headOffice = new Entities.OrganisationLocation();
                headOffice.OrganisationLocationName = m_organisation.OrganisationName + " - " + tcPostTown.Text.Trim();
                headOffice.OrganisationLocationType = eOrganisationLocationType.HeadOffice;
                m_organisation.Locations = new Entities.OrganisationLocationCollection();
                m_organisation.Locations.Add(headOffice);
            }

            if (headOffice == null)
            {
                headOffice = new Entities.OrganisationLocation();
                m_organisation.Locations.Add(headOffice);
                headOffice.OrganisationLocationType = eOrganisationLocationType.HeadOffice;
                headOffice.OrganisationLocationName = "Head Office";
            }

            if (headOffice.Point == null)
            {
                headOffice.Point = new Entities.Point();
                headOffice.Point.Description = m_organisation.OrganisationName + " - " + tcPostTown.Text.Trim();
                headOffice.Point.Collect = true;
                headOffice.Point.Deliver = true;
                headOffice.Point.Latitude = decimal.Parse(tcLatitude.Text);
                headOffice.Point.Longitude = decimal.Parse(tcLongitude.Text);
            }

            Facade.IPostTown facPostTown = new Facade.Point();
            headOffice.Point.PostTown = facPostTown.GetPostTownForTownId(Convert.ToInt32(tcClosestTown.SelectedValue));

            if (headOffice.Point.Address == null)
            {
                headOffice.Point.Address = new Entities.Address();
                headOffice.Point.Address.AddressType = eAddressType.Correspondence;
            }

            headOffice.Point.Address.AddressLine1 = tcAddressLine1.Text;
            headOffice.Point.Address.AddressLine2 = tcAddressLine2.Text;
            headOffice.Point.Address.AddressLine3 = tcAddressLine3.Text;
            headOffice.Point.Address.PostTown = tcPostTown.Text;
            headOffice.Point.Address.County = tcCounty.Text;
            headOffice.Point.Address.PostCode = tcPostCode.Text;
            headOffice.Point.Address.Longitude = Decimal.Parse(tcLongitude.Text);
            headOffice.Point.Address.Latitude = Decimal.Parse(tcLatitude.Text);
            headOffice.Point.Address.CountryDescription = this.cboCountry.Text;
            headOffice.Point.Address.CountryId = Convert.ToInt32(this.cboCountry.SelectedValue);

            // set the radius of the point if the address was changed using the lookupAddress
            if (!String.IsNullOrEmpty(this.hdnSetPointRadius.Value))
                if (String.IsNullOrEmpty(this.txtDefaultRadius.Text))
                    headOffice.Point.Radius = Globals.Configuration.GPSDefaultGeofenceRadius;
                else
                    headOffice.Point.Radius = int.Parse(this.txtDefaultRadius.Text);

            if (headOffice.Point.Address.TrafficArea == null)
                headOffice.Point.Address.TrafficArea = new Orchestrator.Entities.TrafficArea();

            headOffice.Point.Address.TrafficArea.TrafficAreaId = Convert.ToInt32(tcTrafficArea.Value);

            headOffice.TelephoneNumber = tcMainTelephoneNumber.Text;
            headOffice.FaxNumber = tcMainFaxNumber.Text;

            #endregion

            if (m_organisationType != eOrganisationType.ClientCustomer)
            {
                //--------------------------------------------------------------------------
                // 4. Populate the Defaults
                //--------------------------------------------------------------------------
                #region Populate the Defaults

                Entities.OrganisationDefault organisationDefault = null;
                if (m_organisation.Defaults == null)
                {
                    m_organisation.Defaults = new Orchestrator.Entities.OrganisationDefaultCollection();
                }

                if (m_organisation.Defaults.Count == 1)
                {
                    organisationDefault = m_organisation.Defaults[0];
                }
                else
                {
                    organisationDefault = new Orchestrator.Entities.OrganisationDefault();
                    m_organisation.Defaults.Add(organisationDefault);
                }

                organisationDefault.InvoiceTypeId = (int)Enum.Parse(typeof(eInvoiceType), tcDefaultInvoiceType.SelectedValue.Replace(" ", ""));
                //new fields for invoice control


                if (m_isUpdate)
                {
                    organisationDefault.IncludeFuelSurcharge = rdFuelSurchargeDisplayInclude.Checked;
                    organisationDefault.IncludeFuelSurchargeInTotals = rdFuelSurchargeHideInclude.Checked || rdFuelSurchargeDisplayInclude.Checked;
                    organisationDefault.IncludeWeights = tcIncludeWeights.Checked;
                    organisationDefault.IncludePallets = tcIncludePallets.Checked;
                    organisationDefault.IncludePalletType = tcIncludePalletType.Checked;
                    organisationDefault.IncludeVatTotal = tcIncludeVatTotal.Checked;
                }
                else
                {
                    organisationDefault.IncludeFuelSurcharge = true;
                    organisationDefault.IncludeFuelSurchargeInTotals = true;
                    organisationDefault.IncludeWeights = true;
                    organisationDefault.IncludePallets = true;
                    organisationDefault.IncludeVatTotal = true;
                }



                organisationDefault.IncludeDemurrage = tcIncludeDemurrage.Checked;
                //added 21/11/07 S.Barriball
                organisationDefault.IncludeRateTariffCard = tcRateTariffCard.Text;

                organisationDefault.ExportReturnOrders = this.chkExportReturnOrders.Checked;
                organisationDefault.ImportReturnOrders = this.chkImportReturnOrder.Checked;
                organisationDefault.ConfirmationEmailAsBookingForm = this.chkAttachConfimationEmail.Checked;

                organisationDefault.IncludeReferences = tcIncludeReferences.Checked;
                organisationDefault.IncludeInvoiceRunId = tcIncludeInvoiceRunId.Checked;

                organisationDefault.IsExcludedFromInvoicing = tcIsExcludedFromInvoicing.Checked;

                organisationDefault.IncludePODs = false;

                if (m_isUpdate)
                    organisationDefault.IncludeJobDetails = tcIncludeJobDetails.Checked;
                else
                    organisationDefault.IncludeJobDetails = true;

                organisationDefault.IncludeExtraDetails = tcIncludeExtraDetails.Checked;
                organisationDefault.IncludeSurcharges = tcIncludeSurcharges.Checked;

                organisationDefault.IncludeInstructionNotes = tcIncludeInstructionNotes.Checked;

                organisationDefault.IncludeJobExtras = tcIncludeJobExtras.Checked;

                organisationDefault.ShowGoodsRefusedOnInvoice = tcShowGoodsRefusal.Checked;

                organisationDefault.JobTypeId = (int)Enum.Parse(typeof(eJobType), tcDefaultJobType.SelectedValue.Replace(" ", ""));

                //#12061 J.Steele Added Service Level
                organisationDefault.IncludeServiceLevel = tcIncludeServiceLevel.Checked;

                // Added 05/12/06 t.lunken
                organisationDefault.PaymentTerms = tcPaymentTerms.Text;

                // Added 06/12/06 Stephen Newman - Issue http://p1sps/sites/products/orchestrator/Lists/Issues/DispForm.aspx?ID=12
                organisationDefault.MustCaptureRate = tcMustCaptureRates.Checked;

                // Added 16/05/07 t.lunken 
                organisationDefault.MustCaptureDebrief = tcMustCaptureDebrief.Checked;

                // Added 26/10/08 t.lunken
                organisationDefault.CaptureCollectionDebrief = tcMustCaptureCollectionDebrief.Checked;

                // Added 21/05/07 T.Lunken
                organisationDefault.GroupageSearchString = tccboGroupageDefaultSort.Text;

                if (String.IsNullOrEmpty(this.txtDefaultRadius.Text))
                    organisationDefault.DefaultGeofenceRadius = null;
                else
                    organisationDefault.DefaultGeofenceRadius = int.Parse(this.txtDefaultRadius.Text);

                // Added 29/06/07 t.lunken
                // Changed 09/07/07 S.Newman
                int earlyMinutes = 0;
                organisationDefault.EarlyMinutes = 30;
                if (int.TryParse(tcEarlyMinutes.Text, out earlyMinutes))
                    organisationDefault.EarlyMinutes = earlyMinutes;

                int lateMinutes = 0;
                organisationDefault.LateMinutes = 30;
                if (int.TryParse(tcLateMinutes.Text, out lateMinutes))
                    organisationDefault.LateMinutes = lateMinutes;

                if (tcTippingThreshold.Text.Length > 0)
                    organisationDefault.TippingThreshold = Int32.Parse(tcTippingThreshold.Text);

                if (tcDemurrageChargeRate.Text.Length > 0)
                    organisationDefault.DemurrageChargeRate = Decimal.Parse(tcDemurrageChargeRate.Text);

                if (tcPalletNumberThreshold.Text.Length > 0)
                    organisationDefault.PalletThreshold = Int32.Parse(tcPalletNumberThreshold.Text);

                if (tcPalletPenaltyCharge.Text.Length > 0)
                    organisationDefault.PalletChargeRate = Decimal.Parse(tcPalletPenaltyCharge.Text);

                organisationDefault.CHEPNumber = tcCHEPNumber.Text;
                organisationDefault.GPSAutoCallIn = this.chkGPSAutoCallIn.Checked;

                if (tcPoint.SelectedValue != "")
                    organisationDefault.DefaultCollectionPointId = Convert.ToInt32(tcPoint.SelectedValue);
                else
                    organisationDefault.DefaultCollectionPointId = 0;

                if (radNumTxt_FuelSurchargeOverride.Value != null)
                    organisationDefault.FuelSurchargePercentage = Convert.ToDecimal(radNumTxt_FuelSurchargeOverride.Value);

                if (radNumTxt_FuelSurchargeAdjustment.Value != null)
                    organisationDefault.FuelSurchargeAdjustmentPercentage = Convert.ToDecimal(radNumTxt_FuelSurchargeAdjustment.Value);

                if (radioFuelSurchargeOverride.Checked == true)
                    organisationDefault.FuelSurchargeMode = eOrganisationFuelSurchargeMode.Override;
                else if (radioFuelSurchargeAdjustment.Checked == true)
                    organisationDefault.FuelSurchargeMode = eOrganisationFuelSurchargeMode.Adjustment;
                else if (radioFuelSurchargeStandard.Checked == true)
                    organisationDefault.FuelSurchargeMode = eOrganisationFuelSurchargeMode.Standard;

                organisationDefault.FuelSurchargeBreakdownTypeId = (eInvoiceDisplayMethod)Enum.Parse(typeof(eInvoiceDisplayMethod), tcFuelSurchargeBreakdownType.SelectedValue.Replace(" ", ""));

                //#12987 JBS
                organisationDefault.FuelSurchargeOnExtras = tcFuelSurchargeOnExtras.Checked;

                organisationDefault.DemurrageTypeId = (eInvoiceDisplayMethod)Enum.Parse(typeof(eInvoiceDisplayMethod), tcDemurrageType.SelectedValue.Replace(" ", ""));
                organisationDefault.LogFrequency = (eLogFrequency)Enum.Parse(typeof(eLogFrequency), tcLogFrequency.SelectedValue.Replace(" ", ""));
                organisationDefault.DayOfWeek = tcDay.SelectedValue.ToString();
                organisationDefault.LogDeliveryTime = tcDeliverLogBy.SelectedDate.Value;
                organisationDefault.LogWarningElapsedTime = Int32.Parse(tcWarningsElapsed.Text);

                string logColumns = String.Empty;

                for (int logCounter = 0; logCounter < tcIncludedColumns.Items.Count; logCounter++)
                {
                    if (logCounter == tcIncludedColumns.Items.Count - 1)
                        logColumns += tcIncludedColumns.Items[logCounter].Value;
                    else
                        logColumns += tcIncludedColumns.Items[logCounter].Value + ",";
                }

                tcDefaultLogColumns.Text = logColumns;

                organisationDefault.LogColumns = logColumns;

                //organisationDefault.LogColumns = tcDefaultLogColumns.Text;

                // Populate the pallet type information.
                organisationDefault.OrganisationPalletTypes = new List<Entities.OrganisationPalletType>();
                foreach (GridViewRow row in tcPalletType.Rows)
                {
                    CheckBox chkPalletTypeIsActive = (CheckBox)row.FindControl("chkPalletTypeIsActive");
                    RdoBtnGrouper cboPalletTypeIsDefault = (RdoBtnGrouper)row.FindControl("cboPalletTypeIsDefault");
                    HiddenField hidPalletTypeId = (HiddenField)row.FindControl("hidPalletTypeId");
                    if (chkPalletTypeIsActive.Checked || cboPalletTypeIsDefault.Checked)
                    {

                        CheckBox chkPalletTypeIsTracked = (CheckBox)row.FindControl("chkPalletTypeIsTracked");

                        organisationDefault.OrganisationPalletTypes.Add(
                            new Entities.OrganisationPalletType(
                                int.Parse(hidPalletTypeId.Value),
                                chkPalletTypeIsTracked.Checked,
                                cboPalletTypeIsDefault.Checked));

                        // If pallet tracking is enabled and the pallets are active, then Must Capture Debrief must be selected.
                        if (chkPalletTypeIsActive.Checked && chkPalletTypeIsTracked.Checked && !organisationDefault.MustCaptureDebrief)
                            organisationDefault.MustCaptureDebrief = true;

                    }
                    TextBox txtPalletBalance = (TextBox)row.FindControl("txtPalletBalance");
                    if (m_identityId > 0 && txtPalletBalance.Attributes["OriginalValue"] != txtPalletBalance.Text)
                    {
                        // Update the Pallet Balance for this type for this client.
                        //Facade.IPalletBalance facPalletBalance = new Facade.Pallet();
                        //facPalletBalance.UpdateClientPalletBalance(null, null, null, m_identityId, int.Parse(hidPalletTypeId.Value), int.Parse(txtPalletBalance.Text), Page.User.Identity.Name);
                    }
                }

                // Populate the goods type information.
                organisationDefault.ActiveGoodsTypeIds = new List<int>();
                foreach (GridViewRow row in tcGoodsType.Rows)
                {
                    CheckBox chkGoodsTypeIsActive = (CheckBox)row.FindControl("chkGoodsTypeIsActive");
                    RdoBtnGrouper cboGoodsTypeIsDefault = (RdoBtnGrouper)row.FindControl("cboGoodsTypeIsDefault");
                    if (chkGoodsTypeIsActive.Checked || cboGoodsTypeIsDefault.Checked)
                    {
                        HiddenField hidGoodsTypeId = (HiddenField)row.FindControl("hidGoodsTypeId");
                        organisationDefault.ActiveGoodsTypeIds.Add(int.Parse(hidGoodsTypeId.Value));

                        if (cboGoodsTypeIsDefault.Checked)
                            organisationDefault.DefaultGoodsTypeId = int.Parse(hidGoodsTypeId.Value);
                    }
                }

                organisationDefault.ActiveServiceLevelIds = new List<int>();
                foreach (GridViewRow row in gvServiceLevels.Rows)
                {
                    CheckBox chkServiceLevelEnabled = (CheckBox)row.FindControl("chkServiceLevelEnabled");
                    RdoBtnGrouper cboServiceLevelIsDefault = (RdoBtnGrouper)row.FindControl("cboServiceLevelIsDefault");
                    if (chkServiceLevelEnabled.Checked || cboServiceLevelIsDefault.Checked)
                    {
                        HiddenField hidServiceLevelID = (HiddenField)row.FindControl("hidServiceLevelID");
                        organisationDefault.ActiveServiceLevelIds.Add(int.Parse(hidServiceLevelID.Value));

                        if (cboServiceLevelIsDefault.Checked)
                            organisationDefault.DefaultServiceLevelID = int.Parse(hidServiceLevelID.Value);
                    }
                }

                organisationDefault.SelectedExtraTypesIds = new List<int>();
                foreach (GridViewRow row in gvExtraTypes.Rows)
                {
                    CheckBox chkExtraTypesEnabled = (CheckBox)row.FindControl("chkExtraTypesEnabled");
                    if (chkExtraTypesEnabled.Checked)
                    {
                        HiddenField hidExtraTypeID = (HiddenField)row.FindControl("hidExtraTypeID");
                        organisationDefault.SelectedExtraTypesIds.Add(int.Parse(hidExtraTypeID.Value));
                    }
                }

                if (!string.IsNullOrEmpty(tccboGroupageDefaultCollectionPoint.SelectedValue))
                {
                    string[] parts = tccboGroupageDefaultCollectionPoint.SelectedValue.Split(',');
                    organisationDefault.GroupageDeliveryRunCollectionPoint = int.Parse(parts[parts.Length - 1]);
                }
                else
                    organisationDefault.GroupageDeliveryRunCollectionPoint = 0;

                if (!string.IsNullOrEmpty(tccboDefaultAttemptedDeliveryReturnPoint.SelectedValue))
                {
                    string[] parts = tccboDefaultAttemptedDeliveryReturnPoint.SelectedValue.Split(',');
                    organisationDefault.DefaultRedeliveryReturnPointID = int.Parse(parts[parts.Length - 1]);
                }
                else
                    organisationDefault.DefaultRedeliveryReturnPointID = 0;

                // Set the default business type to the first one in the list if the user doesn't select one (as advised by GD 01/03/10).
                if (tcDefaultBusinessType.SelectedItem.Value.ToLower() == "--please select--")
                    organisationDefault.DefaultBusinessTypeID = 0;
                else
                    organisationDefault.DefaultBusinessTypeID = int.Parse(tcDefaultBusinessType.SelectedItem.Value);

                organisationDefault.DefaultNumberOfPallets = tcDefaultNoPallets.Text.Length > 0 ? int.Parse(tcDefaultNoPallets.Text) : (int?)null;

                if (tcAutoEmailInvoices.Checked && tcInvoiceEmailAddress.Text.Length > 0)
                {
                    organisationDefault.AutoEmailInvoices = tcAutoEmailInvoices.Checked;
                    organisationDefault.InvoiceEmailAddress = tcInvoiceEmailAddress.Text;
                }
                else
                {
                    organisationDefault.AutoEmailInvoices = false;
                    organisationDefault.InvoiceEmailAddress = string.Empty;
                }

                organisationDefault.InvoiceAttachCSV = tcInvoiceAttachCSV.Checked;

                if (tcInvoiceGroupType.SelectedValue != string.Empty)
                {
                    organisationDefault.InvoiceGroupingType = tcInvoiceGroupType.SelectedValue;
                    organisationDefault.InvoiceGroupingValue = tcInvoiceGroupValue.Value.HasValue ? tcInvoiceGroupValue.Value.Value : 0;
                }

                organisationDefault.AllocationRuleSetID = Utilities.ParseNullable<int>(cboAllocationRuleSet.SelectedValue);
                organisationDefault.ExcludeFromAutoSubcontracting = chkExcludeFromAutoSubcontracting.Checked;

                // Load the tariffs.
                organisationDefault.TariffID =
                    m_organisationType == eOrganisationType.Client && !rbTariffPerBusinessType.Checked ?
                        Utilities.ParseNullable<int>(cboTariff.SelectedValue) :
                        null;

                organisationDefault.PalletforceTrackingNoFromOrder = chkPalletforceTrackingNoFromOrder.Checked;

                if (m_organisationType == eOrganisationType.Client)
                {
                    if (rbTariffPerBusinessType.Checked)
                    {
                        var selectedTariffs =
                            from i in lvBusinessTypeTariffs.Items
                            let tariffID = Utilities.ParseNullable<int>(((RadComboBox)i.FindControl("cboBusinessTypeTariff")).SelectedValue)
                            where tariffID.HasValue
                            select new
                            {
                                BusinessTypeID = int.Parse(((HtmlInputHidden)i.FindControl("hidBusinessTypeID")).Value),
                                TariffID = tariffID.Value
                            };

                        businessTypeTariffs = selectedTariffs.ToDictionary(i => i.BusinessTypeID, i => i.TariffID);
                    }
                    else
                        businessTypeTariffs = new Dictionary<int, int>();
                }

                if (cboControlArea.SelectedItem.Value != "")
                    organisationDefault.DefaultControlAreaId = int.Parse(cboControlArea.SelectedItem.Value);
                else
                    organisationDefault.DefaultControlAreaId = null;

                organisationDefault.ArrivalsBoardEnabled = chkArrivalsBoardEnabled.Checked;
                #endregion

            }

            //--------------------------------------------------------------------------
            // 4. Populate the Defaults
            //--------------------------------------------------------------------------

            PopulateExportSettingsForUpdate();

        }

        private void PopulateReportSettings()
        {
            if (m_organisation != null)
            {
                Facade.IReportSetting facReportSetting = new Facade.ReportSetting();

                // Get the manifest report settings.
                tcMPGeneralResourceInstructions.DataSource = facReportSetting.GetReportSettings(eReportType.Manifest, m_organisation.IdentityId, (int)Orchestrator.Reports.rptManifest.eReportSettingPortion.GeneralInstructions);
                tcMPGeneralResourceInstructions.DataBind();
                tcMPContactNumbers.DataSource = facReportSetting.GetReportSettings(eReportType.Manifest, m_organisation.IdentityId, (int)Orchestrator.Reports.rptManifest.eReportSettingPortion.ContactNumbers);
                tcMPContactNumbers.DataBind();
            }
        }

        private void PopulateGroupageSettings()
        {
            if (m_organisation != null)
            {
                int pointID = 0;

                tccboGroupageDefaultSort.Items.Insert(0, new Telerik.Web.UI.RadComboBoxItem("..Select a Column to Sort by..", "0"));
                tccboGroupageDefaultSort.Items.Insert(1, new Telerik.Web.UI.RadComboBoxItem("Client", "1"));
                tccboGroupageDefaultSort.Items.Insert(2, new Telerik.Web.UI.RadComboBoxItem("Town", "2"));
                tccboGroupageDefaultSort.Items.Insert(2, new Telerik.Web.UI.RadComboBoxItem("Post Code", "3"));
                tccboGroupageDefaultSort.SelectedIndex = 0;

                if (m_organisation.OrganisationType == eOrganisationType.Orchestrator)
                {
                    if (m_organisation.Defaults[0].GroupageDeliveryRunCollectionPoint > 0)
                    {
                        pointID = m_organisation.Defaults[0].GroupageDeliveryRunCollectionPoint;

                        Facade.IPoint facPoint = new Facade.Point();
                        Entities.Point p = facPoint.GetPointForPointId(pointID);

                        tccboGroupageDefaultCollectionPoint.Text = p.Description;
                        tccboGroupageDefaultCollectionPoint.SelectedValue = p.PointId.ToString();
                    }

                    if (m_organisation.Defaults[0].GroupageSearchString != string.Empty)
                    {
                        tccboGroupageDefaultSort.ClearSelection();
                        tccboGroupageDefaultSort.FindItemByText(m_organisation.Defaults[0].GroupageSearchString).Selected = true;
                    }

                    if (m_organisation.Defaults[0].DefaultRedeliveryReturnPointID > 0)
                    {
                        pointID = m_organisation.Defaults[0].DefaultRedeliveryReturnPointID;

                        Facade.IPoint facPoint = new Facade.Point();
                        Entities.Point p = facPoint.GetPointForPointId(pointID);

                        tccboDefaultAttemptedDeliveryReturnPoint.Text = p.Description;
                        tccboDefaultAttemptedDeliveryReturnPoint.SelectedValue = p.PointId.ToString();
                    }
                }
            }
        }

        private void PopulateExportSettingsForUpdate()
        {
            if (m_organisation.ExportSettings== null)
            {
                m_organisation.ExportSettings = new OrganisationExportSettings();
            }

            m_organisation.ExportSettings.EnableSubContractExport = chkEnableSubContractExport.Checked;
            m_organisation.ExportSettings.FTPHost = txtFTPHostName.Text;
            m_organisation.ExportSettings.FTPPort = (txtFTPPortNumber.Value.HasValue) ? ((int?) txtFTPPortNumber.Value.Value) : ((int?) null);
            m_organisation.ExportSettings.FTPUsername = txtFTPUserName.Text;
            m_organisation.ExportSettings.FTPPassword = txtFTPPassword.Text;
            m_organisation.ExportSettings.FTPDirectory = txtFTPDirectory.Text;
            m_organisation.ExportSettings.FTPPassive = chkFTPPassive.Checked;

        }

        private IList<KeyValuePair<int, string>> GetPCVStatuses()
        {
            IList<KeyValuePair<int, string>> pcvStatuses = new List<KeyValuePair<int, string>>();

            if (pcvConfiguration.Style["display"] != "none")
            {
                pcvStatuses.Add(new KeyValuePair<int, string>(int.Parse(txtDeHired.Attributes["PCVRedemptionStatusID"].ToString()), txtDeHired.Text));
                pcvStatuses.Add(new KeyValuePair<int, string>(int.Parse(txtRequiresDeHire.Attributes["PCVRedemptionStatusID"].ToString()), txtRequiresDeHire.Text));
            }

            return pcvStatuses;
        }

        private void SetAddressOnFocus(int countryId)
        {
            int unitedKingdomCountryId = 1;
            if (countryId == unitedKingdomCountryId)
            {

                //tcAddressLine1.Attributes.Add("onfocus", "this.blur();" + btnAdd.ClientID + ".focus();");
                //tcAddressLine2.Attributes.Add("onfocus", "this.blur();" + btnAdd.ClientID + ".focus();");
                //tcAddressLine3.Attributes.Add("onfocus", "this.blur();" + btnAdd.ClientID + ".focus();");
                //tcPostTown.Attributes.Add("onfocus", "this.blur();" + btnAdd.ClientID + ".focus();");
                //tcCounty.Attributes.Add("onfocus", "this.blur();" + btnAdd.ClientID + ".focus();");
                //tcPostCode.Attributes.Add("onfocus", "this.blur();" + btnAdd.ClientID + ".focus();");
            }
            else
            {

                //addressLink.Attributes["style"] = "display:none;";
                //tcAddressLine1.Attributes.Remove("onfocus");
                //tcAddressLine2.Attributes.Remove("onfocus");
                //tcAddressLine3.Attributes.Remove("onfocus");
                //tcPostTown.Attributes.Remove("onfocus");
                //tcCounty.Attributes.Remove("onfocus");
                //tcPostCode.Attributes.Remove("onfocus");
            }
        }

        private void PopulateStaticControls()
        {
            if (!IsPostBack)
            {
                tcDefaultJobType.DataSource = Utilities.UnCamelCase(Enum.GetNames(typeof(eJobType)));
                tcDefaultJobType.DataBind();

                tcLogFrequency.DataSource = Utilities.UnCamelCase(Enum.GetNames(typeof(eLogFrequency)));
                tcLogFrequency.DataBind();

                tcFuelSurchargeBreakdownType.DataSource = Utilities.UnCamelCase(Enum.GetNames(typeof(eInvoiceDisplayMethod)));
                tcFuelSurchargeBreakdownType.DataBind();

                tcDemurrageType.DataSource = Utilities.UnCamelCase(Enum.GetNames(typeof(eInvoiceDisplayMethod)));
                tcDemurrageType.DataBind();

                // Load the array list with days and then populate the drop down
                ArrayList arrDay = new ArrayList(DateTimeFormatInfo.CurrentInfo.DayNames);
                tcDay.DataSource = arrDay;
                tcDay.DataBind();

                Facade.IReferenceData facReferenceData = new Facade.ReferenceData();
                tcIncludedColumns.DataValueField = "LogColumnId";
                tcIncludedColumns.DataTextField = "Description";
                tcIncludedColumns.DataSource = facReferenceData.GetLogColumns();
                tcIncludedColumns.DataBind();

                tcDeliverLogBy.SelectedDate = DateTime.Now;
                tcWarningsElapsed.Text = "900";

                //#12987 JBS
                //Set the Fuel Surcharge On Extras to the default value for this installation
                tcFuelSurchargeOnExtras.Checked = Orchestrator.Globals.Configuration.DefaultFuelSurchargeOnExtras;

                tcPalletType.DataSource = Facade.PalletType.GetAllPalletTypes(m_identityId);
                tcPalletType.RowDataBound += new GridViewRowEventHandler(tcPalletType_RowDataBound);
                tcPalletType.DataBind();

                tcGoodsType.DataSource = Facade.GoodsType.GetAllGoodsTypes(m_identityId);
                tcGoodsType.DataBind();

                // Organisation Currency.
                CultureInfo culture = new CultureInfo(Orchestrator.Globals.Configuration.NativeCulture);

                Facade.ICulture facCul = new Facade.Culture();
                DataSet Cultures = facCul.GetAllCultures();

                rcbClientCulture.Items.Clear();
                foreach (DataRow dr in Cultures.Tables[0].Rows)
                {
                    RadComboBoxItem rcbi = new RadComboBoxItem(dr["CultureName"].ToString(), dr["LCID"].ToString());
                    rcbClientCulture.Items.Add(rcbi);
                }

                if (m_organisation != null && m_organisation.LCID > 0)
                    rcbClientCulture.SelectedValue = m_organisation.LCID.ToString();
                else
                    rcbClientCulture.SelectedValue = culture.LCID.ToString(); // Set Default if none provided.

                pnlClientCulture.Visible = Globals.Configuration.MultiCurrency;

                Orchestrator.Facade.IOrderServiceLevel facOrder = new Orchestrator.Facade.Order();
                gvServiceLevels.DataSource = facOrder.GetAllForOrganisation(m_identityId);
                gvServiceLevels.DataBind();

                Orchestrator.Facade.ExtraType facExtraType = new Orchestrator.Facade.ExtraType();

                gvExtraTypes.DataSource = facExtraType.GetAllForOrganisation(m_identityId);
                gvExtraTypes.DataBind();

                Facade.IBusinessType facBusinessType = new Facade.BusinessType();
                tcDefaultBusinessType.DataSource = facBusinessType.GetAll();
                tcDefaultBusinessType.DataValueField = "BusinessTypeID";
                tcDefaultBusinessType.DataTextField = "Description";
                tcDefaultBusinessType.DataBind();

                tcDefaultBusinessType.Items.Insert(0, "--Please select--");

                btnSubcontractAllocatedOrders.Visible = pnlAllocation.Visible = Utilities.IsAllocationEnabled();

                cboAllocationRuleSet.DataSource = this.DataContext.AllocationRuleSetSet.Select(ars => new { ars.AllocationRuleSetID, ars.Description });
                cboAllocationRuleSet.DataBind();

                using (var uow = DIContainer.CreateUnitOfWork())
                {
                    var controlAreasRepo = DIContainer.CreateRepository<IControlAreaRepository>(uow);
                    var controlAreas = controlAreasRepo.GetAll();

                    foreach(var ca in controlAreas)
                    {
                        var rcItem = new Telerik.Web.UI.RadComboBoxItem();
                        rcItem.Text = ca.Description;
                        rcItem.Value = ca.ControlAreaId.ToString();
                        cboControlArea.Items.Add(rcItem);
                    }
                    cboControlArea.Items.Insert(0, new Telerik.Web.UI.RadComboBoxItem("--- [ Please Select ] ---"));
                }
            }
        }



        void tcPalletType_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                CheckBox chkPalletTypeIsActive = e.Row.FindControl("chkPalletTypeIsActive") as CheckBox;
                CheckBox chkPalletTypeIsTracked = e.Row.FindControl("chkPalletTypeIsTracked") as CheckBox;

                if (chkPalletTypeIsActive != null && chkPalletTypeIsTracked != null)
                {
                    chkPalletTypeIsActive.Attributes.Add("onclick", "javascript:phCheckPalletTypeConfiguration();");
                    chkPalletTypeIsTracked.Attributes.Add("onclick", "javascript:phCheckPalletTypeConfiguration();");
                }

                // set the javascript for managing the changes to the pallet balances
                TextBox txtPallets = e.Row.FindControl("txtPalletBalance") as TextBox;

                txtPallets.Attributes.Add("OriginalValue", ((DataRowView)e.Row.DataItem)["Balance"].ToString());
            }
        }

        private void ResetPageDisplay()
        {
            lblConfirmation.Visible = false;
            pnlConfirmation.Visible = false;
            // clear the error displays in the tabs
            foreach (RadTab tab in this.RadTabStrip1.Tabs)
            {
                if (tab.ImageUrl != String.Empty)
                    tab.ImageUrl = String.Empty;
            }
        }

        private void SetWhereIAm()
        {
            string links = "<a href=\"../default.aspx \">Home</a> -> <a href=\"organisations.aspx \">List Of Clients</a>";
            links += "-> Add/Update " + m_organisationName;
            //lblWhereAmI.Text = links;
            //lblWhereAmI.Visible = true;
        }

        private void SwitchUIToUpdate()
        {

            if (m_organisationType == eOrganisationType.SubContractor)
            {
                // change the back button to go back to subbies not clients
                btnCancelList.Text = "List Subcontractors";
            }
            //if (m_organisationType == eOrganisationType.SubContractor)
            //    Header.Title = "Update Sub-Contractor";
            //else if (m_identityId == Orchestrator.Globals.Configuration.IdentityId)
            //    Header.Title = "Update " + m_organisation.OrganisationName;
            //else
            //    Header.Title = "Update Client";

            //Header.subTitle = "Please make any changes neccessary.";
            //btnAdd.Text = "Update";
        }

        private void RetrieveQueryStringParameters()
        {
            // Get the identity id
            m_identityId = Convert.ToInt32(Request.QueryString["identityId"]);
            string yourCompany = Request.QueryString["yourCompany"];

            if (yourCompany != null && yourCompany == "true")
            {
                m_identityId = Orchestrator.Globals.Configuration.IdentityId;
                this.txtCreditLimit.Visible = false;
                this.lblCreditLimit.Visible = false;
                this.lblCostCentre.Visible = false;
                this.txtCostCentre.Visible = false;
                this.btnPromotetoClient.Visible = false;
            }

            if (m_identityId == 0)
            {
                // Look in the viewstate for the identity id - this will be set after an add (allows straight conversion into update mode)
                m_identityId = Convert.ToInt32(ViewState[C_IDENTITY_ID_VS]);
            }

            if (m_identityId > 0)
            {
                m_isUpdate = true;
            }

            // Get the related identity id
            m_relatedIdentityId = Convert.ToInt32(Request.QueryString["parentIdentityId"]);

            // Get the organisation type
            try
            {
                int organisationType = Convert.ToInt32(Request.QueryString["type"]);
                if (Enum.IsDefined(typeof(eOrganisationType), organisationType))
                    m_organisationType = (eOrganisationType)organisationType;
            }
            catch { }
        }

        private void UpdateErrorTabs()
        {
            // mark the tabs appropriately
            foreach (IValidator thisIValidator in Page.Validators)
            {
                if (!thisIValidator.IsValid)
                {
                    BaseValidator thisValidator = (BaseValidator)thisIValidator;

                    // find the RadPageView containing the validator.
                    RadPageView pageView;
                    if (thisValidator.Parent is RadPageView)
                        pageView = (RadPageView)thisValidator.Parent;
                    else if (thisValidator.Parent.Parent is RadPageView)
                        pageView = (RadPageView)thisValidator.Parent.Parent;
                    else
                        break;

                    foreach (RadTab tab in this.RadTabStrip1.Tabs)
                    {
                        if (tab.PageView.Equals(pageView))
                            MarkTabAsErrorContainer(tab);
                    }
                }
            }
        }

        private bool UpdateOrganisation(IDictionary<int, int> businessTypeTariffs)
        {
            Facade.IOrganisation facOrganisation = new Facade.Organisation();
            string userId = ((CustomPrincipal)Page.User).UserName;

            FacadeResult retVal = facOrganisation.Update(m_organisation, businessTypeTariffs, userId);

            if (!retVal.Success)
            {
                infringementDisplay.Infringements = retVal.Infringements;
                infringementDisplay.DisplayInfringments();
                infringementDisplay.Visible = true;
                lblConfirmation.Text = "There was an error updating the organisation, please try again.";
            }
            else
            {
                // Check special case: update success flag may be true but infringements may exist and will need to be displayed if 
                // attempted updates to 3rd party systems have not worked. Failure to update 3rd party systems does not necessarily mean 
                // that the client update in orchestrator should not go ahead.

                if (retVal.Infringements.Count > 0)
                {
                    infringementDisplay.Infringements = retVal.Infringements;
                    infringementDisplay.DisplayInfringments();
                    infringementDisplay.Visible = true;
                    lblConfirmation.Text = "The Organisation has been updated successfully. However, an error occurred whilst attempting to update a matching organisation in your 3rd party integrated system.";
                }
                else
                {
                    infringementDisplay.Visible = false;
                    lblConfirmation.Text = "The Organisation has been updated successfully.";
                }

                if (this.m_oldCreditLimit != m_organisation.CreditLimit)
                    try
                    {
                        //Update credit limit in Exchequer

                        Facade.IUser facUser = new Facade.User();
                        facUser.SendEmailToUsersInRole((int)eUserRole.CreditManagement,
                            "Credit Management Alert", this.GenerateCreditManagementEmail());
                    }
                    catch
                    {
                        // log that we couldn't send an email.
                    }

                //using (var uow = DIContainer.CreateUnitOfWork())
                //{
                //    var organisationRepo = DIContainer.CreateRepository<IOrganisationRepository>(uow);
                //    foreach()
                //}
            }

            return retVal.Success;
        }

        private string GenerateCreditManagementEmail()
        {
            StringBuilder emailBody = new StringBuilder();
            string oldCreditLimitString = String.Empty;

            if (this.m_oldCreditLimit == null)
                oldCreditLimitString = "Old Credit Limit: Not Set";
            else
                oldCreditLimitString = String.Format("Old Credit Limit: £{0}",
                    Math.Round((decimal)this.m_oldCreditLimit, 2));

            string creditLimitString = String.Empty;

            if (m_organisation.CreditLimit == null)
                creditLimitString = "New Credit Limit: Not Set";
            else
                creditLimitString = String.Format("New Credit Limit: £{0}",
                    Math.Round((decimal)m_organisation.CreditLimit, 2));

            string organisationLink = "";
            // Example link to an organisation.
            // http://richards.orchestrator.co.uk/organisation/addupdateorganisation.aspx?identityId=113

            organisationLink = string.Format("http://{0}{1}{2}", HttpContext.Current.Request.Url.Host, "/organisation/addupdateorganisation.aspx?identityId=", m_identityId.ToString());

            emailBody.AppendLine(String.Format("Client: {0}", m_organisation.OrganisationDisplayName));
            emailBody.AppendLine(oldCreditLimitString);
            emailBody.AppendLine(creditLimitString);
            emailBody.AppendLine(String.Format("Updated by: {0}", ((CustomPrincipal)this.Page.User).UserName));
            emailBody.AppendLine(String.Format("Updated at: {0}", DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss")));
            emailBody.AppendLine("");
            emailBody.AppendLine("The following link will take you to the organisation:");
            emailBody.AppendLine(organisationLink);

            return emailBody.ToString();
        }

        protected void dlgScan_DialogCallBack(object sender, EventArgs e)
        {
            //A scan wizard dialog has returned so we need to update the document links
            LoadStatusBox();

            if (chkSuspended.Checked && Globals.Configuration.ClientSuspendedPendingDocuments)
            {
                bool unsuspend = false;
                if (chkIsSubContractor.Checked)
                {
                    if (chkCreditApplicationForm.Checked && chkSubbyTnC.Checked)
                        unsuspend = true;
                }
                else
                {
                    if (chkCreditApplicationForm.Checked)
                        unsuspend = true;
                }

                if (unsuspend)
                {
                    chkSuspended.Checked = false;
                    m_organisation.IdentityStatus = eIdentityStatus.Active;

                    Facade.IOrganisation facOrganisation = new Facade.Organisation();
                    string userId = ((CustomPrincipal)Page.User).UserName;
                    FacadeResult retVal = facOrganisation.Update(m_organisation, userId);
                }
            }
        }

        protected void btnAdd_Click(object sender, EventArgs e)
        {
            bool success;

            tcCFVInvoiceEmailAddress.IsValid = !(tcAutoEmailInvoices.Checked && tcInvoiceEmailAddress.Text.Trim() == string.Empty);

            if (Page.IsValid)
            {
                Facade.IPCV facPCV = new Facade.PCV();
                IDictionary<int, int> businessTypeTariffs = null;
                PopulateOrganisation(out businessTypeTariffs);
                IList<KeyValuePair<int, string>> updatablePCVStatuses = GetPCVStatuses();

                try
                {
                    if (m_isUpdate)
                        success = UpdateOrganisation(businessTypeTariffs);
                    else
                        success = AddOrganisation(businessTypeTariffs);

                    if (success && updatablePCVStatuses.Count > 0)
                        facPCV.UpdatePCVRedemptionStatusDescriptions(updatablePCVStatuses, ((CustomPrincipal)Page.User).UserName);

                    if (success)
                        UpdateLinkedClients();
                }
                catch(Exception ex)
                {
                    Global.UnhandledException(ex);
                    success = false;
                    lblConfirmation.Text = string.Format("There was an error while saving your changes, please try again.");
                }
                finally
                {
                    ViewState[C_ORGANISATION_CONTACTS_TABLE_VS] = null;
                }

                if (m_isUpdate)
                {
                    if (success)
                    {
                        LoadOrganisation();
                        ConfigureUI();
                        this.ClientScript.RegisterStartupScript(this.GetType(), "CallBack", "__dialogCallBack(window, 'refresh');", true);
                    }
                }
                else
                {
                    if (success)
                    {
                        // Set viewstate objects
                        ViewState[C_IDENTITY_ID_VS] = m_organisation.IdentityId;
                        ViewState[C_ORGANISATION_VS] = m_organisation;

                        // Configure page for Update mode
                        SwitchUIToUpdate();

                        m_identityId = m_organisation.IdentityId;
                        LoadOrganisation();
                        ConfigureUI();
                    }
                }

                lblConfirmation.Visible = true;
                pnlConfirmation.Visible = true;
            }
            else
            {
                // Force the error icons to appear in the tabs
                UpdateErrorTabs();
            }
        }

        private void UpdateLinkedClients()
        {
            if (m_identityId == 0)
                return;

            using (var uow = DIContainer.CreateUnitOfWork())
            {
                var orgRepo = DIContainer.CreateRepository<IOrganisationRepository>(uow);
                var thisOrg = orgRepo.Find(m_identityId);

                if (thisOrg == null)
                    return;

                var clientIDsToAdd = LinkedClientsToAdd.Value.Split(',');

                foreach (var clientIDString in clientIDsToAdd)
                {
                    int clientID;

                    if (int.TryParse(clientIDString, out clientID) && clientID != m_identityId)
                    {
                        var client = thisOrg.LinkedClients.FirstOrDefault(o => o.IdentityID == clientID);

                        if (client == null)
                        {
                            client = orgRepo.Find(clientID);

                            if (client != null)
                            {
                                thisOrg.LinkedClients.Add(client);

                                // When adding a client as a linked client also add the same relationship in reverse
                                if (!client.LinkedClients.Any(lc => lc.IdentityID == thisOrg.IdentityID))
                                    client.LinkedClients.Add(thisOrg);
                            }
                        }
                    }
                }

                var clientIDsToRemove = LinkedClientsToRemove.Value.Split(',');

                foreach (var clientIDString in clientIDsToRemove)
                {
                    int clientID;

                    if (int.TryParse(clientIDString, out clientID))
                    {
                        var client = thisOrg.LinkedClients.FirstOrDefault(o => o.IdentityID == clientID);

                        if (client != null)
                            thisOrg.LinkedClients.Remove(client);
                    }

                }

                uow.SaveChanges();
            }
        }

        protected void btnListClients_Click(object sender, EventArgs e)
        {
            StringBuilder sb = new StringBuilder();

            if (m_organisationType == eOrganisationType.SubContractor)
                sb.Append("../Resource/SubContractor/subcontractors.aspx");
            else
                sb.Append("organisations.aspx");

            if (m_organisation != null)
                sb.Append(string.Format("?filter={0}", m_organisation.OrganisationName.Substring(0, 1)));

            Response.Redirect(sb.ToString());
        }

        protected void btnPromotetoClient_Click(object sender, EventArgs e)
        {
            using (Facade.IOrganisation facOrganisation = new Facade.Organisation())
            {
                m_organisation.IsSubContractor = true;

                if (UpdateOrganisation(null))
                    facOrganisation.UpdateOrganisationType(m_identityId, eOrganisationType.Client,
                                                       ((CustomPrincipal)Page.User).UserName);
                else
                    m_organisation.IsSubContractor = false;
            }

            lblConfirmation.Text = "The Sub-Contractor has " + (m_organisation.IsSubContractor ? "" : "not ") + "been promoted successfully.";
            lblConfirmation.Visible = true;
            pnlConfirmation.Visible = true;

            if (m_organisation.IsSubContractor)
            {
                tcIsSubContractor.Checked = true;
                btnPromotetoClient.Visible = false;
            }
        }

        #region Validation

        private void MarkTabAsErrorContainer(Telerik.Web.UI.RadTab tab)
        {
            if (tab.ImageUrl == String.Empty)
            {
                tab.ImageUrl = "../images/error.gif";
                tab.Text = " " + tab.Text;
            }
        }

        protected void ValidateTippingThreshold(object obj, ServerValidateEventArgs args)
        {
            args.IsValid = P1TP.Components.Common.SimpleValidation.ValidateNumericValue(args.Value, 0, true);
        }

        protected void ValidateDemurrageChargeRate(object obj, ServerValidateEventArgs args)
        {
            args.IsValid = P1TP.Components.Common.SimpleValidation.ValidateNumericValue(args.Value, 0);
        }

        protected void ValidateRateTariffCard(object sender, ServerValidateEventArgs args)
        {
            args.IsValid = false;
            CustomValidator validator = (CustomValidator)sender;
            TextBox tb = (TextBox)validator.Parent.FindControl(validator.ControlToValidate);
            if (tb != null)
            {
                bool isValid = (tb.Text.Length <= tb.MaxLength);
                args.IsValid = isValid;
            }
        }

        protected void ValidatePalletNumberThreshold(object obj, ServerValidateEventArgs args)
        {
            args.IsValid = P1TP.Components.Common.SimpleValidation.ValidateNumericValue(args.Value, 0, true);
        }

        protected void ValidatePalletPenaltyCharge(object obj, ServerValidateEventArgs args)
        {
            args.IsValid = P1TP.Components.Common.SimpleValidation.ValidateNumericValue(args.Value, 0);
        }

        protected void ValidateFuelSurcharge(object obj, ServerValidateEventArgs args)
        {
            //Allow negative Fuel Surcharges. Requested by Vicky Banks on 3/10/08 during meeting
            //with J.Steele to go through the Exchequer Integration
            args.IsValid = P1TP.Components.Common.SimpleValidation.ValidateNumericValue(args.Value, -100);
        }

        protected void ValidateLogWarningAfter(object obj, ServerValidateEventArgs args)
        {
            args.IsValid = P1TP.Components.Common.SimpleValidation.ValidateNumericValue(args.Value, 0, true);
        }

        protected void cfvTown_ServerValidate(object obj, ServerValidateEventArgs args)
        {
            args.IsValid = P1TP.Components.Common.SimpleValidation.ValidateNumericValue(tcClosestTown.SelectedValue, 1, true);
        }

        #endregion

        #region Client Details

        private void PopulateClientDetails()
        {
            CultureInfo culture = new CultureInfo(Orchestrator.Globals.Configuration.NativeCulture);

            tcOrganisationName.Text = m_organisation.OrganisationName;
            tcOrganisationDisplayName.Text = m_organisation.OrganisationDisplayName;
            tcAccountCode.Text = m_organisation.AccountCode;
            tcIsSubContractor.Checked = m_organisation.IsSubContractor;
            tcVATNumber.Text = m_organisation.VATNumber;

            if (m_organisation.NominalCodeId == null)
                this.cboNominalCode.SelectedValue = "0";
            else
                this.cboNominalCode.SelectedValue = m_organisation.NominalCodeId.ToString();

            Entities.OrganisationLocation headOffice = null;
            if (m_organisation.Locations != null && m_organisation.Locations.Count > 0)
                headOffice = m_organisation.Locations.GetHeadOffice();

            if (headOffice != null)
            {
                tcClosestTown.Text = headOffice.Point.PostTown.TownName;
                tcClosestTown.SelectedValue = headOffice.Point.PostTown.TownId.ToString();

                tcAddressLine1.Text = headOffice.Point.Address.AddressLine1;
                tcAddressLine2.Text = headOffice.Point.Address.AddressLine2;
                tcAddressLine3.Text = headOffice.Point.Address.AddressLine3;
                tcPostTown.Text = headOffice.Point.Address.PostTown;
                tcCounty.Text = headOffice.Point.Address.County;
                tcPostCode.Text = headOffice.Point.Address.PostCode;
                cboCountry.SelectedValue = headOffice.Point.Address.CountryId.ToString();
                this.SetAddressOnFocus(headOffice.Point.Address.CountryId);
                tcLongitude.Text = headOffice.Point.Address.Longitude.ToString();
                tcLatitude.Text = headOffice.Point.Address.Latitude.ToString();
                tcTrafficArea.Value = headOffice.Point.Address.TrafficArea.TrafficAreaId.ToString();
                tcMainTelephoneNumber.Text = headOffice.TelephoneNumber;
                tcMainFaxNumber.Text = headOffice.FaxNumber;
            }

            LoadStatusBox();

            // Organisation Currency.
            Facade.ICulture facCul = new Facade.Culture();
            DataSet Cultures = facCul.GetAllCultures();

            rcbClientCulture.Items.Clear();
            foreach (DataRow dr in Cultures.Tables[0].Rows)
            {
                RadComboBoxItem rcbi = new RadComboBoxItem(dr["CultureName"].ToString(), dr["LCID"].ToString());
                rcbClientCulture.Items.Add(rcbi);
            }

            if (m_organisation.LCID > 0)
                rcbClientCulture.SelectedValue = m_organisation.LCID.ToString();
            else
                rcbClientCulture.SelectedValue = culture.LCID.ToString(); // Set Default if none provided.

            pnlClientCulture.Visible = Globals.Configuration.MultiCurrency;

            if (m_organisation.OnHold == true)
            {
                tcClientOnHold.Checked = true;
                this.txtOnHoldReason.Text = m_organisation.OnHoldReason;
            }
            else
            {
                tcClientOnHold.Checked = false;
                this.txtOnHoldReason.Text = String.Empty;
            }

            if (m_organisation.CreditLimit == null)
                this.txtCreditLimit.Value = null;
            else
                this.txtCreditLimit.Value = Convert.ToDouble(m_organisation.CreditLimit);

            txtCostCentre.Text = m_organisation.CostCentre;

        }

        private void LoadStatusBox()
        {
            if (m_organisation.IdentityStatus == eIdentityStatus.Active)
            {
                tcClientSuspended.Checked = false;
            }
            else
            {
                tcClientSuspended.Checked = true;
                tcClientSuspendedReason.Text = m_organisation.SuspendedReason;
            }

            if (m_organisation.OnHold == true)
            {
                tcClientOnHoldReason.Text = m_organisation.OnHoldReason;
                tcClientOnHold.Checked = true;
            }
            else
            {
                tcClientOnHold.Checked = false;
            }

            //Get the OrganisationDocuments for this Org
            List<EF.OrganisationDocument> orgDocs = (
                from od in this.DataContext.OrganisationDocuments.Include("ScannedForm.FormType")
                where od.Organisation.IdentityId == m_organisation.IdentityId
                select od).ToList();


            //Set the Hyperlinks for each document type
            var creditApplicationForm = orgDocs.FirstOrDefault(od => od.ScannedForm.FormType.FormTypeId == (int)eFormTypeId.CreditApplicationForm);
            SetOrganisationDocumentLinks(eFormTypeId.CreditApplicationForm, creditApplicationForm, chkCreditApplicationForm, hlCreditApplicationForm, hlScanCreditApplicationForm);

            var clientTnC = orgDocs.FirstOrDefault(od => od.ScannedForm.FormType.FormTypeId == (int)eFormTypeId.ClientTnCs);
            SetOrganisationDocumentLinks(eFormTypeId.ClientTnCs, clientTnC, chkClientTnC, hlClientTnC, hlScanClientTnC);

            var subbyTnC = orgDocs.FirstOrDefault(od => od.ScannedForm.FormType.FormTypeId == (int)eFormTypeId.SubbyTnCs);
            SetOrganisationDocumentLinks(eFormTypeId.SubbyTnCs, subbyTnC, chkSubbyTnC, hlSubbyTnC, hlScanSubbyTnC);

        }

        private void SetOrganisationDocumentLinks(eFormTypeId formType, EF.OrganisationDocument organisationDocument, CheckBox chkPresent, HyperLink hlDocument, HyperLink hlScan)
        {
            if (organisationDocument != null)
            {
                chkPresent.Checked = true;

                if (organisationDocument.ScannedForm.IsUploaded.Value)
                {
                    hlDocument.Visible = true;
                    hlDocument.Text = "View";
                    hlDocument.NavigateUrl = organisationDocument.ScannedForm.ScannedFormPDF ?? string.Empty;

                    hlScan.Visible = true;
                    hlScan.Text = "| Re-Scan";
                    hlScan.NavigateUrl = "javascript:" + dlgScan.GetOpenDialogScript(string.Format("ScannedFormTypeId={0}&OrgDocId={1}&ScannedFormId={2}",
                        (int)formType, organisationDocument.OrganisationDocumentId, organisationDocument.ScannedForm.ScannedFormID));
                }
                else
                {
                    hlDocument.Visible = true;
                    hlDocument.NavigateUrl = string.Empty;
                    hlDocument.Text = "Awaiting Uploaded";

                    hlScan.Visible = false;
                }
            }
            else
            {
                chkPresent.Checked = false;
                hlDocument.Visible = false;

                hlScan.Visible = true;
                hlScan.Text = "Scan";
                hlScan.NavigateUrl = "javascript:" + dlgScan.GetOpenDialogScript(string.Format("ScannedFormTypeId={0}&OrgId={1}",
                    (int)formType, m_organisation.IdentityId));
            }
        }

        #endregion

        #region References

        protected void btnAddReference_Click(object sender, EventArgs e)
        {
            Response.Redirect("addupdateorganisationreference.aspx?identityId=" + m_identityId + "&organisationName=" + m_organisationName);
        }

        private Entities.OrganisationReference GetAffectedReference(DataGridItem gridItem)
        {
            // Retrieve the reference we are dealing with from the organisation's References collection.
            int referenceId;
            if (tcReferences.EditItemIndex == -1)
            {
                referenceId = Convert.ToInt32(((HtmlInputHidden)(gridItem.FindControl("hidReferenceIdForEdit"))).Value);
            }
            else
            {
                referenceId = Convert.ToInt32(((HtmlInputHidden)(gridItem.FindControl("hidReferenceIdForUpdate"))).Value);
            }
            Entities.OrganisationReference reference = m_organisation.References.FindByReferenceId(referenceId);
            return reference;
        }

        private DataSet GetReferenceData(bool activeOnly)
        {
            Facade.IOrganisation facOrganisation = new Facade.Organisation();
            return facOrganisation.GetReferencesForIdentityId(m_identityId, activeOnly);
        }

        private void MoveReference(int rowIndex, eReferenceMoveDirection direction)
        {
            bool success = false;

            DataGridItem movingItem = tcReferences.Items[rowIndex];
            Entities.OrganisationReference movingReference = GetAffectedReference(movingItem);

            // Move the reference
            int currentPosition = m_organisation.References.IndexOf(movingReference);
            m_organisation.References.RemoveAt(currentPosition);
            switch (direction)
            {
                case eReferenceMoveDirection.Down:
                    m_organisation.References.Insert(currentPosition + 1, movingReference);
                    break;
                case eReferenceMoveDirection.Up:
                    m_organisation.References.Insert(currentPosition - 1, movingReference);
                    break;
            }

            // Re-sync the sequence numbers
            for (int i = 0; i < m_organisation.References.Count; i++)
            {
                m_organisation.References[i].SequenceNumber = i + 1;
            }

            // Update the data
            Facade.IOrganisationReference facOrganisationReference = new Facade.Organisation();
            string userId = ((Entities.CustomPrincipal)Page.User).UserName;
            success = facOrganisationReference.Update(m_organisation.References, userId);

            if (success)
            {
                // Update the viewstate organisation
                ViewState[C_ORGANISATION_VS] = m_organisation;
            }

            // Rebind the datagrid
            PopulateReferences();
        }

        private void PopulateReferences()
        {
            if (!IsPostBack)
            {
                tcLoadNumberText.Text = m_organisation.LoadNumberText;
                tcDocketNumberText.Text = m_organisation.DocketNumberText;

                if (m_organisation.ReferenceDefault == "Load Number")
                    tcUseAsDefaultLoadNo.Checked = true;
                else
                    tcUseAsDefaultDockeNo.Checked = true;
            }

            // If "show deleted" checked, then activeOnly is false
            tcReferences.DataSource = GetReferenceData(!chkShowDeletedReferences.Checked);
            tcReferences.DataBind();
        }

        protected string[] ReferenceDataTypes
        {
            get { return Enum.GetNames(typeof(eOrganisationReferenceDataType)); }
        }

        protected string[] ReferenceStatus
        {
            get { return Enum.GetNames(typeof(eOrganisationReferenceStatus)); }
        }

        protected void dgReferences_CancelCommand(object source, DataGridCommandEventArgs e)
        {
            tcReferences.EditItemIndex = -1;
            PopulateReferences();
        }

        protected void dgReferences_EditCommand(object source, DataGridCommandEventArgs e)
        {
            tcReferences.EditItemIndex = e.Item.ItemIndex;
            PopulateReferences();
        }

        protected void dgReferences_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            switch (e.CommandName.ToLower())
            {
                case "movedown":
                    if (e.Item.ItemIndex < tcReferences.Items.Count)
                        MoveReference(e.Item.ItemIndex, eReferenceMoveDirection.Down);
                    break;
                case "moveup":
                    if (e.Item.ItemIndex > 0)
                        MoveReference(e.Item.ItemIndex, eReferenceMoveDirection.Up);
                    break;
            }
        }

        protected void dgReferences_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (tcReferences.EditItemIndex != -1)
            {
                // Disable the button columns
                e.Item.Cells[C_MOVE_DOWN_CELL_ORDINAL].Enabled = false;
                e.Item.Cells[C_MOVE_UP_CELL_ORDINAL].Enabled = false;

                // Disable the "view deleted" checkbox
                tcShowDeletedReferences.Enabled = false;
            }
            else
            {
                // Enable the "view deleted" checkbox
                tcShowDeletedReferences.Enabled = true;
            }
        }

        protected void dgReferences_UpdateCommand(object source, DataGridCommandEventArgs e)
        {
            DataGridItem updatingItem = tcReferences.Items[e.Item.ItemIndex];
            Entities.OrganisationReference updatingReference = GetAffectedReference(updatingItem);

            // Validate the reference description
            IValidator rfvReferenceDescription = (IValidator)updatingItem.FindControl("rfvReferenceDescription");
            rfvReferenceDescription.Validate();

            if (rfvReferenceDescription.IsValid)
            {
                // Update the reference
                updatingReference.Description = ((TextBox)updatingItem.FindControl("txtReferenceDescription")).Text;
                DropDownList cboReferenceDataType = (DropDownList)updatingItem.FindControl("cboReferenceDataType");
                DropDownList cboReferenceStatus = (DropDownList)updatingItem.FindControl("cboReferenceStatus");
                CheckBox chkCanDisplayOnInvoice = (CheckBox)updatingItem.FindControl("chkCanDisplayOnInvoice");
                CheckBox chkIsMandatoryOnOrder = (CheckBox)updatingItem.FindControl("chkIsMandatoryOnOrder");
                updatingReference.DataType = (eOrganisationReferenceDataType)Enum.Parse(typeof(eOrganisationReferenceDataType), cboReferenceDataType.SelectedValue, true);
                updatingReference.Status = (eOrganisationReferenceStatus)Enum.Parse(typeof(eOrganisationReferenceStatus), cboReferenceStatus.SelectedValue, true);
                updatingReference.DisplayOnInvoice = chkCanDisplayOnInvoice.Checked;
                updatingReference.MandatoryOnOrder = chkIsMandatoryOnOrder.Checked;

                // Update the data
                Facade.IOrganisationReference facOrganisationReference = new Facade.Organisation();
                string userId = ((Entities.CustomPrincipal)Page.User).UserName;
                bool success = facOrganisationReference.Update(updatingReference, userId);

                if (success)
                {
                    lblConfirmation.Text = "The Reference was updated successfully.";
                    lblConfirmation.Visible = true;
                    pnlConfirmation.Visible = true;
                    tcReferences.EditItemIndex = -1;

                    // Update the organisation in viewstate
                    ViewState[C_ORGANISATION_VS] = m_organisation;

                    // Rebind the datagrid
                    PopulateReferences();
                }
                else
                {
                    lblConfirmation.Text = "The Reference was not updated successfully.";
                    imgIcon.ImageUrl = "~/images/ico_critical.gif";
                    pnlConfirmation.Visible = true;
                    lblConfirmation.Visible = true;
                }
            }
            else
            {
                // Display an error marker
                MarkTabAsErrorContainer(this.RadTabStrip1.Tabs[C_TAB_REFERENCES]);
            }
        }

        protected void dgShowDeletedReferences_CheckedChanged(object sender, EventArgs e)
        {
            PopulateReferences();
        }

        #endregion

        #region Locations

        protected void btnViewPoints_Click(object sender, EventArgs e)
        {
            Response.Redirect("../Point/listpoints.aspx?identityId=" + m_identityId.ToString());
        }

        protected void btnAddLocation_Click(object sender, EventArgs e)
        {
            Response.Redirect("addupdateorganisationlocation.aspx?identityId=" + m_identityId + "&organisationName=" + m_organisationName.Replace("&", "%26"));
        }

        private DataSet GetLocationData()
        {
            Facade.IOrganisation facOrganisation = new Facade.Organisation();
            return facOrganisation.GetLocationsForIdentityId(m_identityId);
        }

        private string LocationSortCriteria
        {
            get { return (string)ViewState["LocationSortCriteria"]; }
            set { ViewState["LocationSortCriteria"] = value; }
        }

        private string LocationSortDirection
        {
            get { return (string)ViewState["LocationSortDirection"]; }
            set { ViewState["LocationSortDirection"] = value; }
        }

        private void PopulateLocations()
        {
            PopulateLocations(LocationSortCriteria + " " + LocationSortDirection);
        }

        private void PopulateLocations(string sort)
        {
            tcLocationsGrid.Rebind();
        }

        protected void dgLocations_Page(Object sender, DataGridPageChangedEventArgs e)
        {
            tcLocations.CurrentPageIndex = e.NewPageIndex;
            PopulateLocations();
        }

        protected void dgLocations_SortCommand(object source, System.Web.UI.WebControls.DataGridSortCommandEventArgs e)
        {
            DataSet dsLocations = GetLocationData();
            DataView dvLocations = new DataView(dsLocations.Tables[0]);

            // Configure the sort
            if (e.SortExpression == LocationSortCriteria)
            {
                // switch direction
                if (LocationSortDirection == "desc")
                    LocationSortDirection = "asc";
                else
                    LocationSortDirection = "desc";
            }
            else
            {
                // new sort
                LocationSortCriteria = e.SortExpression;
                LocationSortDirection = "desc";
            }

            dvLocations.Sort = LocationSortCriteria + " " + LocationSortDirection;
            tcLocations.DataSource = dvLocations;
            tcLocations.DataBind();
        }

        #endregion

        #region Export Settings

        private void PopulateExportSettings()
        {
            chkEnableSubContractExport.Checked = m_organisation.ExportSettings.EnableSubContractExport;
            txtFTPHostName.Text = m_organisation.ExportSettings.FTPHost;
            txtFTPPortNumber.Value = m_organisation.ExportSettings.FTPPort;
            txtFTPUserName.Text = m_organisation.ExportSettings.FTPUsername;
            txtFTPPassword.Text = m_organisation.ExportSettings.FTPPassword;
            txtFTPDirectory.Text = m_organisation.ExportSettings.FTPDirectory;
            chkFTPPassive.Checked = m_organisation.ExportSettings.FTPPassive;

            SetExportSettingsValidation();

        }

        protected void chkEnableSubContractExport_CheckedChanged(object sender, EventArgs e)
        {
            SetExportSettingsValidation();
        }

        /// <summary>
        /// Enables/Disables required field validation for ftp settings based on whether
        /// csv export is enabled or not;
        /// </summary>
        private void SetExportSettingsValidation()
        {
            rfvFTPHostname.Enabled = chkEnableSubContractExport.Checked;
            rfvFTPUserName.Enabled = chkEnableSubContractExport.Checked;
            rfvFTPPassword.Enabled = chkEnableSubContractExport.Checked;
        }

        #endregion

        #region Defaults

        private void PopulateDefaults()
        {
            if (m_organisation.Defaults != null & m_organisation.Defaults.Count > 0)
            {
                // The collection is always populated with one
                Orchestrator.Entities.OrganisationDefault currentDefault = m_organisation.Defaults[0];

                Facade.IPCV facPCV = new Facade.PCV();
                IList<KeyValuePair<int, string>> UpdatablePCVRedemptionStatuses = facPCV.GetUpdatablePCVStatuses();

                // Default collection point

                // Default collection point
                if (m_organisation.Defaults[0].DefaultCollectionPointId != 0)
                {
                    Facade.IPoint facPoint = new Facade.Point();
                    Entities.Point defaultCollectionPoint = facPoint.GetPointForPointId(Convert.ToInt32(m_organisation.Defaults[0].DefaultCollectionPointId));
                    tcPoint.SelectedValue = defaultCollectionPoint.PointId.ToString();
                    tcPoint.Text = defaultCollectionPoint.Description;
                }

                // Configure the invoice type id
                tcDefaultInvoiceType.SelectedValue = Utilities.UnCamelCase(((eInvoiceType)currentDefault.InvoiceTypeId).ToString());

                // Configure the invoice includes
                tcIncludePallets.Checked = currentDefault.IncludePallets;
                tcIncludePalletType.Checked = currentDefault.IncludePalletType;
                tcIncludeWeights.Checked = currentDefault.IncludeWeights;
                tcIncludeVatTotal.Checked = currentDefault.IncludeVatTotal;

                rdFuelSurchargeDisplayInclude.Checked = currentDefault.IncludeFuelSurcharge && currentDefault.IncludeFuelSurchargeInTotals;
                rdFuelSurchargeHideInclude.Checked = (!currentDefault.IncludeFuelSurcharge) && currentDefault.IncludeFuelSurchargeInTotals;
                rdFuelSurchargeNeither.Checked = (!currentDefault.IncludeFuelSurcharge) && (!currentDefault.IncludeFuelSurchargeInTotals);


                tcIncludeDemurrage.Checked = currentDefault.IncludeDemurrage;
                tcRateTariffCard.Text = currentDefault.IncludeRateTariffCard;
                tcDemurrageType.SelectedValue = Utilities.UnCamelCase(((eInvoiceDisplayMethod)currentDefault.DemurrageTypeId).ToString());

                // Configure the tariff settings
                bool showTariffs = m_organisationType == eOrganisationType.Client;
                pnlTariff.Visible = showTariffs;

                if (showTariffs)
                {
                    bool isPerBusinessType = false;

                    lvBusinessTypeTariffs.DataSource = this.DataContext.BusinessTypeSet.OrderBy(bt => bt.Description);
                    lvBusinessTypeTariffs.DataBind();

                    if (currentDefault.TariffID.HasValue)
                    {
                        var tariff = this.DataContext.TariffSet.First(t => t.TariffId == currentDefault.TariffID.Value);
                        cboTariff.Text = tariff.Description;
                        cboTariff.SelectedValue = tariff.TariffId.ToString();
                    }
                    else
                    {
                        var businessTypeTariffs =
                            this.DataContext.OrganisationBusinessTypeTariffSet
                            .Include("Tariff")
                            .Where(obtt => obtt.Organisation.IdentityId == m_identityId);

                        isPerBusinessType = businessTypeTariffs.Any();

                        foreach (var item in lvBusinessTypeTariffs.Items)
                        {
                            // Pre-select the currently active tariff.
                            int businessTypeID = (int)lvBusinessTypeTariffs.DataKeys[item.DataItemIndex].Value;
                            var businessTypeTariff = businessTypeTariffs.FirstOrDefault(obtt => obtt.BusinessType.BusinessTypeID == businessTypeID);

                            if (businessTypeTariff != null)
                            {
                                // Select the specified tariff.
                                var tariff = businessTypeTariff.Tariff;
                                var cboBusinessTypeTariff = (RadComboBox)item.FindControl("cboBusinessTypeTariff");
                                cboBusinessTypeTariff.Text = tariff.Description;
                                cboBusinessTypeTariff.SelectedValue = tariff.TariffId.ToString();
                            }
                        }
                    }

                    if (isPerBusinessType)
                        rbTariffPerBusinessType.Checked = true;
                    else
                        rbTariffPerClient.Checked = true;

                    pnlTariffPerClient.Style[HtmlTextWriterStyle.Display] = isPerBusinessType ? "none" : string.Empty;
                    pnlTariffPerBusinessType.Style[HtmlTextWriterStyle.Display] = isPerBusinessType ? string.Empty : "none";
                }

                //#12976 JBS
                //Enable tcDemurrageType if Extras are included
                tcDemurrageType.Visible = currentDefault.IncludeJobDetails;
                tcIncludeReferences.Checked = currentDefault.IncludeReferences;
                tcIncludeInvoiceRunId.Checked = currentDefault.IncludeInvoiceRunId;
                tcIsExcludedFromInvoicing.Checked = currentDefault.IsExcludedFromInvoicing;
                tcIncludeJobDetails.Checked = currentDefault.IncludeJobDetails;
                tcIncludeExtraDetails.Checked = tcDemurrageType.Visible = currentDefault.IncludeExtraDetails;
                tcIncludeSurcharges.Checked = currentDefault.IncludeSurcharges;
                tcIncludeInstructionNotes.Checked = currentDefault.IncludeInstructionNotes;
                tcIncludeJobExtras.Checked = currentDefault.IncludeJobExtras;
                tcShowGoodsRefusal.Checked = currentDefault.ShowGoodsRefusedOnInvoice;

                this.chkExportReturnOrders.Checked = currentDefault.ExportReturnOrders;
                this.chkImportReturnOrder.Checked = currentDefault.ImportReturnOrders;
                this.chkAttachConfimationEmail.Checked = currentDefault.ConfirmationEmailAsBookingForm;

                //#12061 J.Steele Configure Include Service Level
                tcIncludeServiceLevel.Checked = currentDefault.IncludeServiceLevel;

                // Configure the job type id
                tcDefaultJobType.SelectedValue = Utilities.UnCamelCase(((eJobType)currentDefault.JobTypeId).ToString());

                // Configure the demurrage settings
                tcTippingThreshold.Text = currentDefault.TippingThreshold.ToString();
                tcDemurrageChargeRate.Text = currentDefault.DemurrageChargeRate.ToString();

                // Configure the pallet settings
                tcPalletNumberThreshold.Text = currentDefault.PalletThreshold.ToString();
                tcPalletPenaltyCharge.Text = currentDefault.PalletChargeRate.ToString();
                tcCHEPNumber.Text = currentDefault.CHEPNumber;

                // Configure the fuel surcharge settings
                if (currentDefault.FuelSurchargePercentage != null)
                    radNumTxt_FuelSurchargeOverride.Value = new double?(Convert.ToDouble(currentDefault.FuelSurchargePercentage.Value));

                if (currentDefault.FuelSurchargeAdjustmentPercentage != null)
                    radNumTxt_FuelSurchargeAdjustment.Value = new double?(Convert.ToDouble(currentDefault.FuelSurchargeAdjustmentPercentage.Value));

                // Configure the fuel surcharge breakdown settings
                tcFuelSurchargeBreakdownType.SelectedValue = Utilities.UnCamelCase(((eInvoiceDisplayMethod)currentDefault.FuelSurchargeBreakdownTypeId).ToString());

                // Configure the fuelsurcharge settings based on mode
                switch (currentDefault.FuelSurchargeMode)
                {
                    case eOrganisationFuelSurchargeMode.Override:
                        this.radioFuelSurchargeStandard.Checked = false;
                        this.radioFuelSurchargeAdjustment.Checked = false;
                        this.radioFuelSurchargeOverride.Checked = true;
                        this.radNumTxt_FuelSurchargeOverride.Enabled = true;
                        this.radNumTxt_FuelSurchargeAdjustment.Enabled = false;

                        break;
                    case eOrganisationFuelSurchargeMode.Adjustment:
                        this.radioFuelSurchargeStandard.Checked = false;
                        this.radioFuelSurchargeAdjustment.Checked = true;
                        this.radioFuelSurchargeOverride.Checked = false;
                        this.radNumTxt_FuelSurchargeOverride.Enabled = false;
                        this.radNumTxt_FuelSurchargeAdjustment.Enabled = true;

                        break;
                    case eOrganisationFuelSurchargeMode.Standard:
                        this.radioFuelSurchargeStandard.Checked = true;
                        this.radioFuelSurchargeAdjustment.Checked = false;
                        this.radioFuelSurchargeOverride.Checked = false;
                        this.radNumTxt_FuelSurchargeOverride.Enabled = false;
                        this.radNumTxt_FuelSurchargeAdjustment.Enabled = false;

                        break;
                    default:
                        break;
                }

                //#12987 JBS
                // Configure the fuel surcharge on extras settings
                tcFuelSurchargeOnExtras.Checked = currentDefault.FuelSurchargeOnExtras;

                // Configure the Must Capture Rates property
                tcMustCaptureRates.Checked = currentDefault.MustCaptureRate;

                this.txtDefaultRadius.Text = (currentDefault.DefaultGeofenceRadius == null) ? String.Empty : currentDefault.DefaultGeofenceRadius.ToString();

                tcGPSAutoCallIn.Checked = currentDefault.GPSAutoCallIn;

                // Added 16/05/07 t.lunken
                // Configure if we are required to capture debrief information
                tcMustCaptureDebrief.Checked = currentDefault.MustCaptureDebrief;

                //Added 26/10/08 t.lunken
                tcMustCaptureCollectionDebrief.Checked = currentDefault.CaptureCollectionDebrief;

                // Configure the log frequency id
                tcLogFrequency.SelectedValue = Utilities.UnCamelCase(currentDefault.LogFrequency.ToString());

                this.txtPaymentTerms.Text = currentDefault.PaymentTerms;

                foreach (KeyValuePair<int, string> kvp in UpdatablePCVRedemptionStatuses)
                    switch (kvp.Key)
                    {
                        case (int)ePCVRedemptionStatus.PostedToBAndMForDeHire:
                            txtDeHired.Text = kvp.Value;
                            txtDeHired.Attributes.Add("PCVRedemptionStatusID", kvp.Key.ToString());
                            break;
                        case (int)ePCVRedemptionStatus.RequiresDeHire:
                            txtRequiresDeHire.Text = kvp.Value;
                            txtRequiresDeHire.Attributes.Add("PCVRedemptionStatusID", kvp.Key.ToString());
                            break;
                        default:
                            break;
                    }

                // Configure the Day Of Week depending of log frequency id
                if ((eLogFrequency)Enum.Parse(typeof(eLogFrequency), tcLogFrequency.SelectedValue.Replace(" ", "")) == eLogFrequency.Weekly)
                    tcPanelDay.Visible = true;
                else
                    tcPanelDay.Visible = false;

                tcDay.SelectedValue = currentDefault.DayOfWeek.ToString();

                // Configure the Deliver Log By setting
                tcDeliverLogBy.SelectedDate = currentDefault.LogDeliveryTime;

                // Configure the Warning Elapsed Time setting
                tcWarningsElapsed.Text = currentDefault.LogWarningElapsedTime.ToString();

                // Configure the Log Columns setting
                tcDefaultLogColumns.Text = currentDefault.LogColumns;

                if (!IsPostBack)
                {
                    Facade.IOrganisation facOrganisation = new Facade.Organisation();
                    tcIncludedColumns.DataTextField = "Description";
                    tcIncludedColumns.DataValueField = "LogColumnId";
                    tcIncludedColumns.DataSource = facOrganisation.GetIncludedLogColumnsForIdentityId(m_identityId);
                    tcIncludedColumns.DataBind();
                    tcExcludedColumns.DataTextField = "Description";
                    tcExcludedColumns.DataValueField = "LogColumnId";
                    tcExcludedColumns.DataSource = facOrganisation.GetExcludedLogColumnsForIdentityId(m_identityId);
                    tcExcludedColumns.DataBind();

                    tcPalletType.DataSource = Facade.PalletType.GetAllPalletTypes(m_identityId);
                    tcPalletType.DataBind();

                    tcGoodsType.DataSource = Facade.GoodsType.GetAllGoodsTypes(m_identityId);
                    tcGoodsType.DataBind();

                    
                }

                // 29-06-07 t.lunken Added Early/Late Times
                tcEarlyMinutes.Text = currentDefault.EarlyMinutes.ToString();
                tcLateMinutes.Text = currentDefault.LateMinutes.ToString();

                // 04/01/10 t.lunken
                tcAutoEmailInvoices.Checked = currentDefault.AutoEmailInvoices;
                tcInvoiceAttachCSV.Checked = currentDefault.InvoiceAttachCSV;



                if (currentDefault.InvoiceGroupingType != tcInvoiceGroupType.SelectedValue)
                {
                    tcInvoiceGroupType.ClearSelection();
                    tcInvoiceGroupType.Items.FindByValue(currentDefault.InvoiceGroupingType).Selected = true;
                }
                tcInvoiceGroupValue.Value = currentDefault.InvoiceGroupingValue;

                tcInvoiceEmailAddress.Text = currentDefault.InvoiceEmailAddress;

                chkPalletforceTrackingNoFromOrder.Checked = currentDefault.PalletforceTrackingNoFromOrder;

                if (currentDefault.DefaultControlAreaId != null)
                    cboControlArea.FindItemByValue(currentDefault.DefaultControlAreaId.ToString()).Selected = true;

                chkArrivalsBoardEnabled.Checked = currentDefault.ArrivalsBoardEnabled;

            }

           
        }

        #endregion

        #region Clients

        private string ClientSortCriteria
        {
            get { return (string)ViewState["ClientSortCriteria"]; }
            set { ViewState["ClientSortCriteria"] = value; }
        }

        private string ClientSortDirection
        {
            get { return (string)ViewState["ClientSortDirection"]; }
            set { ViewState["ClientSortDirection"] = value; }
        }

        private string IncompleteClientSortCriteria
        {
            get { return (string)ViewState["ClientSortCriteria"]; }
            set { ViewState["ClientSortCriteria"] = value; }
        }

        private string IncompleteClientSortDirection
        {
            get { return (string)ViewState["ClientSortDirection"]; }
            set { ViewState["ClientSortDirection"] = value; }
        }

        private DataSet GetClientData()
        {
            Facade.IOrganisation facOrganisation = new Facade.Organisation();
            return facOrganisation.GetClientsForIdentityId(m_identityId);
        }

        private void PopulateClients()
        {
            tcClientsGrid.Rebind();
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
            this.Init += new EventHandler(addupdateorganisation_Init);
        }

        #endregion

        private void addupdateorganisation_Init(object sender, EventArgs e)
        {
            //#129976 JBS
            //Enable cboDemurrageType when Job Extras are included
            //Note that DemurrageTypeId would be better named ExtrasDisplayMethodId
            this.tcIncludeExtraDetails.CheckedChanged += new EventHandler(tcIncludeExtraDetails_CheckedChanged);

            this.tcClosestTown.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(tcClosestTown_ItemsRequested);
            this.tcPoint.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(tcPoint_ItemsRequested);
            this.tcClientsGrid.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(tcClientsGrid_NeedDataSource);
            this.tcClientsGrid.ItemCommand += new Telerik.Web.UI.GridCommandEventHandler(tcClientsGrid_ItemCommand);

            this.tcLocationsGrid.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(tcLocationsGrid_NeedDataSource);

            //RJD:
            this.tcContactsGrid.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(tcContactsGrid_NeedDataSource);
            this.tcContactsGrid.UpdateCommand += new Telerik.Web.UI.GridCommandEventHandler(tcContactsGrid_UpdateCommand);
            this.tcContactsGrid.InsertCommand += new GridCommandEventHandler(tcContactsGrid_InsertCommand);
            this.tcContactsGrid.DeleteCommand += new GridCommandEventHandler(tcContactsGrid_DeleteCommand);
            this.tcContactsGrid.ItemCreated += new GridItemEventHandler(tcContactsGrid_ItemCreated);
            this.tcContactsGrid.PreRender += new EventHandler(tcContactsGrid_PreRender);

            this.tcExportMessagesGrid.NeedDataSource += tcExportMessagesGrid_NeedDataSource;
            this.tcExportMessagesGrid.ItemCreated +=tcExportMessagesGrid_ItemCreated;

            this.cboCountry.SelectedIndexChanged += new RadComboBoxSelectedIndexChangedEventHandler(cboCountry_SelectedIndexChanged);

            this.tcLookUp.Click += new EventHandler(lnkLookUp_Click);
            this.tcAddressList.SelectedIndexChanged += new EventHandler(lstAddress_SelectedIndexChanged);
            this.btnCancelList.Click += new EventHandler(btnCancelList_Click);
        }





        /*-------------------------------------------------------------------------------------------------------*/

        protected void btnCancelList_Click(object sender, EventArgs e)
        {
            this.pnlAddress.Visible = true;
            this.pnlAddressList.Visible = false;
        }

        /*-------------------------------------------------------------------------------------------------------*/

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

        /*-------------------------------------------------------------------------------------------------------*/

        private void cboCountry_SelectedIndexChanged(object o, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if (!String.IsNullOrEmpty(this.cboCountry.SelectedValue))
                this.SetAddressOnFocus(int.Parse(this.cboCountry.SelectedValue));

            if (Globals.Configuration.MultiCurrency && rcbClientCulture.Visible != false) //If MultiCurrency Is Enabled - set default culture for Country.
            {
                Facade.IReferenceData facRef = new Facade.ReferenceData();
                int defaultLCID = facRef.GetCultureForCountry(int.Parse(((Telerik.Web.UI.RadComboBox)(o)).SelectedValue));
                rcbClientCulture.SelectedValue = defaultLCID.ToString();
            }
        }

        /*-------------------------------------------------------------------------------------------------------*/

        private void tcContactsGrid_ItemCreated(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridEditableItem && e.Item.IsInEditMode)
            {
                GridEditableItem item = e.Item as GridEditableItem;
                this.AddValidator(item, "FirstName", " Must supply a first name.");
                this.AddValidator(item, "Lastname", " Must supply a last name.");
                this.AddValidator(item, "Email", " Valid email needed.", Constants.EMAIL_REGULAR_EXPRESSION.ToString());
                this.AddValidator(item, "Telephone", " Valid telephone number needed.", Constants.UK_TELEPHONE_NUMBER_REGULAR_EXPRESSION.ToString());
                this.AddValidator(item, "Fax", " Valid fax number needed.", Constants.UK_TELEPHONE_NUMBER_REGULAR_EXPRESSION.ToString());
            }
        }

        /*-------------------------------------------------------------------------------------------------------*/

        public void AddValidator(GridEditableItem item, string columnName, string errorMessage)
        {
            GridTextBoxColumnEditor editor = (GridTextBoxColumnEditor)item.EditManager.GetColumnEditor(columnName);
            TableCell cell = (TableCell)editor.TextBoxControl.Parent;

            RequiredFieldValidator validator = new RequiredFieldValidator();
            validator.ControlToValidate = editor.TextBoxControl.ID;
            validator.ErrorMessage = errorMessage;
            cell.Controls.Add(validator);
        }

        /*-------------------------------------------------------------------------------------------------------*/

        public void AddValidator(GridEditableItem item, string columnName, string errorMessage, string regularExpression)
        {
            GridTextBoxColumnEditor editor = (GridTextBoxColumnEditor)item.EditManager.GetColumnEditor(columnName);
            TableCell cell = (TableCell)editor.TextBoxControl.Parent;

            RegularExpressionValidator validator = new RegularExpressionValidator();
            validator.ControlToValidate = editor.TextBoxControl.ID;
            validator.ErrorMessage = errorMessage;
            validator.ValidationExpression = regularExpression;
            cell.Controls.Add(validator);
        }

        /*-------------------------------------------------------------------------------------------------------*/

        public void AddDropDownValidator(GridEditableItem item, string columnName, string errorMessage)
        {
            GridDropDownColumnEditor editor = (GridDropDownColumnEditor)item.EditManager.GetColumnEditor(columnName);
            TableCell cell = (TableCell)editor.ContainerControl.Controls[0].Parent;

            RequiredFieldValidator validator = new RequiredFieldValidator();
            validator.ControlToValidate = editor.ContainerControl.Controls[0].ID;
            validator.ErrorMessage = errorMessage;
            validator.InitialValue = "Not Set";
            cell.Controls.Add(validator);
        }

        /*-------------------------------------------------------------------------------------------------------*/

        public bool IsContactGridInEditMode()
        {
            bool inEditMode = false;

            foreach (GridDataItem item in tcContactsGrid.Items)
                if (item.EditFormItem.IsInEditMode)
                {
                    inEditMode = true;
                    break;
                }

            return inEditMode;
        }

        /*-------------------------------------------------------------------------------------------------------*/

        void tcContactsGrid_DeleteCommand(object source, GridCommandEventArgs e)
        {
            GridEditableItem editedItem = e.Item as GridEditableItem;

            int id = int.Parse(e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["IndividualId"].ToString());
            DataSet datasource = this.OrganisationContactsDataSource;

            List<DataRow> rowsToDelete = new List<DataRow>();

            //Extract rows to delete
            foreach (DataRow dataRow in datasource.Tables["organisationContacts"].Rows)
                if (Convert.ToInt32(dataRow["IndividualId"]) == id)
                    rowsToDelete.Add(dataRow);

            foreach (DataRow dataRow in rowsToDelete)
            {
                Entities.Individual individualToDelete = null;

                foreach (Entities.Individual individual in this.m_organisation.IndividualContacts)
                    if (individual.IdentityId == Convert.ToInt32(id))
                    {
                        individualToDelete = individual;
                        break;
                    }

                if (individualToDelete != null)
                {
                    string userId = ((Entities.CustomPrincipal)Page.User).UserName;

                    Facade.Organisation facOrganisation = new Facade.Organisation();

                    try
                    {
                        facOrganisation.DeleteContact(this.m_organisation.IdentityId, individualToDelete.IdentityId, userId);

                        datasource.Tables["organisationContacts"].Rows.Remove(dataRow);
                        this.m_organisation.IndividualContacts.Remove(individualToDelete);
                    }
                    catch
                    {
                        e.Canceled = true;
                    }
                }
            }

            datasource.Tables["organisationContacts"].AcceptChanges();
            this.OrganisationContactsDataSource = datasource;

            this.tcContactsGrid.DataSource = null;
        }

        /*-------------------------------------------------------------------------------------------------------*/

        void tcContactsGrid_PreRender(object sender, EventArgs e)
        {
            //Get the "GridCommandItem " using the GetItem method   
            GridCommandItem GridCommandItem = (GridCommandItem)this.tcContactsGrid.MasterTableView.GetItems(GridItemType.CommandItem)[0];

            Button button = GridCommandItem.FindControl("btnAddNewContact") as Button;

            if (button != null)
                if (m_isUpdate)
                {
                    button.Enabled = !this.IsContactGridInEditMode();
                }
                else
                    button.Enabled = false;
        }

        /*-------------------------------------------------------------------------------------------------------*/

        private void tcContactsGrid_InsertCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
        {
            // If this code doesn't fire when the insert contact button is clicked, check for validation issues
            // on the other tabs of the page.

            GridEditableItem editedItem = e.Item as GridEditableItem;
            GridEditManager editMan = editedItem.EditManager;
            int individualId = 0;
            DataSet organisationContacts = this.OrganisationContactsDataSource;

            DataRow newRow = organisationContacts.Tables["organisationContacts"].NewRow();

            newRow["IndividualId"] = individualId;

            //Update new values
            Hashtable newValues = new Hashtable();
            //The GridTableView will fill the values from all editable columns in the hash
            e.Item.OwnerTableView.ExtractValuesFromItem(newValues, editedItem);

            newRow.BeginEdit();
            try
            {
                foreach (DictionaryEntry entry in newValues)
                {
                    newRow[(string)entry.Key] = entry.Value;
                }
                newRow.EndEdit();
            }
            catch
            {
                newRow.CancelEdit();
                e.Canceled = true;
            }

            bool recordAlreadyExists = false;

            foreach (DataRow row in organisationContacts.Tables["organisationContacts"].Rows)
                if (row["IndividualId"] != System.DBNull.Value && (Convert.ToInt32(row["IndividualId"]) == individualId))
                {
                    recordAlreadyExists = true;
                    break;
                }

            if (!recordAlreadyExists)
            {
                organisationContacts.Tables["organisationContacts"].Rows.Add(newRow);
                organisationContacts.AcceptChanges();

                this.OrganisationContactsDataSource = organisationContacts;

                this.UpdateContacts(individualId);

                this.OrganisationContactsDataSource = null;

                this.tcContactsGrid.DataSource = null;
            }
        }

        /*-------------------------------------------------------------------------------------------------------*/

        void tcContactsGrid_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            // RJD:
            this.tcContactsGrid.DataSource = this.OrganisationContactsDataSource;
        }

        /*-------------------------------------------------------------------------------------------------------*/

        void tcContactsGrid_UpdateCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
        {
            DataSet organisationContacts = this.OrganisationContactsDataSource;
            int individualId = 0;

            GridEditableItem editedItem = e.Item as GridEditableItem;

            individualId = int.Parse(e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["IndividualId"].ToString());

            //Locate the changed row in the DataSource
            DataRow[] changedRows = organisationContacts.Tables[0].Select(string.Format("IndividualId = '{0}'", individualId.ToString()));

            if (changedRows.Length != 1)
            {
                e.Canceled = true;
                return;
            }

            //Update new values
            Hashtable newValues = new Hashtable();
            //The GridTableView will fill the values from all editable columns in the hash
            e.Item.OwnerTableView.ExtractValuesFromItem(newValues, editedItem);

            changedRows[0].BeginEdit();
            try
            {
                foreach (DictionaryEntry entry in newValues)
                {
                    changedRows[0][(string)entry.Key] = entry.Value;
                }
                changedRows[0].EndEdit();
            }
            catch
            {
                changedRows[0].CancelEdit();
                e.Canceled = true;
            }

            organisationContacts.AcceptChanges();

            this.OrganisationContactsDataSource = organisationContacts;

            this.UpdateContacts(individualId);

            this.OrganisationContactsDataSource = organisationContacts;

            this.tcContactsGrid.DataSource = null;
        }

        /*-------------------------------------------------------------------------------------------------------*/

        private void UpdateContacts(int individualIdentityId)
        {
            // RJD:
            if (this.m_organisation == null)
                return;

            DataSet organisationContacts = this.OrganisationContactsDataSource;

            Entities.Individual individual = this.GetIndividual(individualIdentityId);

            foreach (DataRow row in organisationContacts.Tables["organisationContacts"].Rows)
                if (Convert.ToInt32(row["IndividualId"]) == individualIdentityId)
                    this.UpdateIndividual(ref individual, row);

            if (m_isUpdate)
            {
                Entities.FacadeResult returnValue = new FacadeResult(0);
                string userId = ((Entities.CustomPrincipal)Page.User).UserName;

                Facade.Individual facIndividual = new Facade.Individual();

                if (individual.IdentityId == 0)
                {
                    returnValue = facIndividual.CreateIndividualAsContact(individual, this.m_organisation.IdentityId, userId);

                    if (!returnValue.Success)
                        this.ShowError(returnValue);
                    else
                    {
                        infringementDisplay.Infringements = null;
                        infringementDisplay.DisplayInfringments();
                        infringementDisplay.Visible = false;

                        individual.IdentityId = returnValue.ObjectId;

                        if (this.m_organisation.IndividualContacts == null)
                            this.m_organisation.IndividualContacts = new List<Individual>();

                        this.m_organisation.IndividualContacts.Add(individual);
                    }
                }
                else
                    if (facIndividual.Update(individual, userId))
                    {
                        infringementDisplay.Infringements = null;
                        infringementDisplay.DisplayInfringments();
                        infringementDisplay.Visible = false;
                    }
            }
        }

        /*-------------------------------------------------------------------------------------------------------*/


        private Entities.Individual GetIndividual(int individualIdentityId)
        {
            Entities.Individual individual = null;

            if (individualIdentityId == 0)
            {
                individual = new Entities.Individual();
                individual.IdentityId = 0;
                individual.IndividualType = eIndividualType.Contact;
                individual.IdentityStatus = eIdentityStatus.Active;
            }
            else
                individual = this.m_organisation.IndividualContacts.Find(delegate(Entities.Individual i)
                {
                    return i.IdentityId == individualIdentityId;
                });

            return individual;
        }

        /*-------------------------------------------------------------------------------------------------------*/

        private void ShowError(Entities.FacadeResult facadeResult)
        {
            infringementDisplay.Infringements = facadeResult.Infringements;
            infringementDisplay.DisplayInfringments();
            infringementDisplay.Visible = true;

            imgIcon.ImageUrl = "~/images/ico_critical.gif";
            lblConfirmation.Text = "There was an error adding the Contact, please try again.";
            lblConfirmation.Visible = true;
            pnlConfirmation.Visible = true;
            lblConfirmation.ForeColor = Color.Red;
        }

        /*-------------------------------------------------------------------------------------------------------*/

        private void UpdateIndividual(ref Entities.Individual individual, DataRow row)
        {
            // RJD: sort out title
            individual.Title = (eTitle)Enum.Parse(typeof(eTitle), row["Title"].ToString());
            individual.FirstNames = row["FirstName"].ToString().Trim();
            individual.LastName = row["LastName"].ToString().Trim();
            if (row["IndividualContactType"].ToString() == "Not Set")
                individual.IndividualContactType = 0;
            else
                individual.IndividualContactType = (eIndividualContactType)Enum.Parse(typeof(eIndividualContactType),
                    row["IndividualContactType"].ToString().Trim().Replace(" ", ""));

            individual.IndividualContactType = (eIndividualContactType)Enum.Parse(typeof(eIndividualContactType),
                row["IndividualContactType"].ToString().Trim().Replace(" ", ""));

            if (individual.Contacts == null)
                individual.Contacts = new ContactCollection();

            string[] contacts = Utilities.UnCamelCase(Enum.GetNames(typeof(eContactType)));

            foreach (string contact in contacts)
                this.PopulateContacts(individual.Contacts, row, contact.Replace(" ", ""));
        }

        /*-------------------------------------------------------------------------------------------------------*/

        private void PopulateContacts(Entities.ContactCollection contacts, DataRow row, string contactTypeString)
        {
            // RJD:
            eContactType contactTypeEnum = (eContactType)Enum.Parse(typeof(eContactType), contactTypeString);

            Entities.Contact tempContact = contacts.GetForContactType(contactTypeEnum);

            if (!String.IsNullOrEmpty(row[contactTypeString].ToString()))
                if (tempContact == null)
                {
                    tempContact = new Entities.Contact(0, contactTypeEnum,
                        row[contactTypeString].ToString());

                    contacts.Add(tempContact);
                }
                else
                    tempContact.ContactDetail = row[contactTypeString].ToString();
            else
                if (tempContact != null)
                    tempContact.ContactDetail = String.Empty;
        }

        /*-------------------------------------------------------------------------------------------------------*/

        public DataSet PopulateOrganisationContacts()
        {
            DataSet contactsDataSet = new DataSet("contactsDataSet");
            DataTable destinationTable = new DataTable("organisationContacts");

            string individualIdColumn = "IndividualId";
            string titleColumn = "Title";
            string firstNameColumn = "FirstName";
            string lastNameColumn = "LastName";
            string individualContactTypeColumn = "IndividualContactType";

            destinationTable.Columns.Add(individualIdColumn);
            destinationTable.Columns.Add(titleColumn);
            destinationTable.Columns[titleColumn].DefaultValue = 0;
            destinationTable.Columns.Add(firstNameColumn);
            destinationTable.Columns.Add(lastNameColumn);
            destinationTable.Columns.Add(individualContactTypeColumn);
            destinationTable.Columns.Add(eContactType.Email.ToString());
            destinationTable.Columns.Add(eContactType.Telephone.ToString());
            destinationTable.Columns.Add(eContactType.MobilePhone.ToString());
            destinationTable.Columns.Add(eContactType.PersonalMobile.ToString());
            destinationTable.Columns.Add(eContactType.Fax.ToString());

            if (m_organisation != null)
                if (m_organisation.IndividualContacts != null)
                    foreach (Entities.Individual individual in m_organisation.IndividualContacts)
                    {
                        DataRow newRow = destinationTable.NewRow();

                        newRow[individualIdColumn] = individual.IdentityId;
                        newRow[titleColumn] = individual.Title;
                        newRow[firstNameColumn] = individual.FirstNames;
                        newRow[lastNameColumn] = individual.LastName;
                        newRow[individualContactTypeColumn] = Utilities.UnCamelCase(individual.IndividualContactType.ToString());

                        foreach (Entities.Contact contact in individual.Contacts)
                            newRow[contact.ContactType.ToString()] = contact.ContactDetail.ToString();

                        destinationTable.Rows.Add(newRow);
                    }

            destinationTable.AcceptChanges();

            DataTable titlesDataTable = this.CreateTitleTable();

            DataTable individualContactTypesTable = this.CreateIndividualContactTypeTable();

            contactsDataSet.Tables.Add(destinationTable);
            contactsDataSet.Tables.Add(titlesDataTable);
            contactsDataSet.Tables.Add(individualContactTypesTable);

            return contactsDataSet;
        }

        /*-------------------------------------------------------------------------------------------------------*/

        private DataTable CreateTitleTable()
        {
            DataTable titles = new DataTable("Titles");
            string[] titleList = Utilities.UnCamelCase(Enum.GetNames(typeof(eTitle)));

            titles.Columns.Add(new DataColumn("Title", typeof(string)));

            foreach (string title in titleList)
                titles.Rows.Add(title);

            return titles;
        }

        /*-------------------------------------------------------------------------------------------------------*/

        private DataTable CreateIndividualContactTypeTable()
        {
            DataTable individualContactTypes = new DataTable("IndividualContactType");
            string[] individualContactTypeList = Utilities.UnCamelCase(Enum.GetNames(typeof(eIndividualContactType)));

            individualContactTypes.Columns.Add(new DataColumn("IndividualContactType", typeof(string)));

            foreach (string individualContactType in individualContactTypeList)
                individualContactTypes.Rows.Add(individualContactType);

            return individualContactTypes;
        }

        /*-------------------------------------------------------------------------------------------------------*/

        private void tcExportMessagesGrid_ItemCreated(object sender, GridItemEventArgs e)
        {
            // retrieve the export message start/end date pickers 
            // (since they live in a Telerik "CommandItemTemplate" they
            // are not avaiable from code-behind by default
            if (e.Item is Telerik.Web.UI.GridCommandItem)
            {
                Telerik.Web.UI.GridCommandItem item = e.Item as Telerik.Web.UI.GridCommandItem;
                tcExportMessagesStartDate = item.FindControl("dteExportMessagesStartDate") as RadDatePicker;
                tcExportMessagesEndDate = item.FindControl("dteExportMessagesEndDate") as RadDatePicker;

                tcExportMessagesStartDate.SelectedDate = ExportMessagesStartDate;
                tcExportMessagesEndDate.SelectedDate = ExportMessagesEndDate;

            }

        }

        private void tcExportMessagesGrid_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {

            PopulateExportMessages();
            this.tcExportMessagesGrid.DataSource = this.ExportMessagesDataSource;
        }

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            ExportMessagesStartDate = tcExportMessagesStartDate.SelectedDate;
            ExportMessagesEndDate = tcExportMessagesEndDate.SelectedDate;
            grdExportMessages.Rebind();

        }

        private void PopulateExportMessages()
        {

            if (this.m_organisation == null) return;

            using (var uow = DIContainer.CreateUnitOfWork())
            {

                int? lastNMessages = null;

                // if no start or end date specified just show the last 10 messages
                if (!ExportMessagesStartDate.HasValue && !ExportMessagesEndDate.HasValue)
                    lastNMessages = 10;

                var exportMessageRepo = DIContainer.CreateRepository<IExportMessageRepository>(uow);
                var exportMessages = exportMessageRepo.GetExportMessagesForSubContractor(this.m_organisation.IdentityId, lastNMessages, ExportMessagesStartDate, ExportMessagesEndDate).ToList();

                this.ExportMessagesDataSource = exportMessages.Select(em => new
                        {
                            MessageId = em.ExportMessage.ExportMessageID,
                            RunId = em.JobSubContract.JobID,
                            OrderId = em.ExportMessage.EntityID,
                            Filename = em.ExportMessage.FileName,
                            Sent = em.ExportMessage.CreateDate,
                            Status = Enum.Parse(typeof(eMessageState), em.ExportMessage.MessageStateID.ToString()).ToString()
                        }
                    );
            
            }

            exportMessagesShowingMessage.Visible = !ExportMessagesStartDate.HasValue && !ExportMessagesEndDate.HasValue;
        }

        /*-------------------------------------------------------------------------------------------------------*/

        void tcLocationsGrid_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            DataView dvLocations = new DataView(GetLocationData().Tables[0]);

            foreach (DataRowView rowView in dvLocations)
            {
                DataRow row = rowView.Row;
                row["organisationName"] = row["organisationName"].ToString().Replace("&", "%26");
            }

            tcLocationsGrid.DataSource = dvLocations;
        }

        void tcClientsGrid_ItemCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
        {
            if (e.CommandName.ToLower() == "removerelationship")
            {
                int identityID = (int)e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["IdentityId"];
                int relatedIdentityId = (int)e.Item.OwnerTableView.DataKeyValues[e.Item.ItemIndex]["RelatedIdentityId"];
                Facade.IIdentityRelationship facIdentity = new Facade.IdentityRelationship();
                facIdentity.DeleteRelationship(identityID, relatedIdentityId, Page.User.Identity.Name);
                m_identityId = relatedIdentityId;
                tcClientsGrid.Rebind();
            }
        }

        void tcClientsGrid_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            DataView dvClients = new DataView(GetClientData().Tables[0]);
            tcClientsGrid.DataSource = dvClients;
        }

        protected void gvExtraTypes_DataBound(object sender, EventArgs e)
        {
            var dsExtraTypes = (DataSet)gvExtraTypes.DataSource;

             foreach (GridViewRow row in gvExtraTypes.Rows)
            {
                DataRow dr = dsExtraTypes.Tables[0].Rows[row.RowIndex];
                CheckBox chkExtraTypesEnabled = (CheckBox)row.FindControl("chkExtraTypesEnabled");
                HiddenField hidExtraTypeID = (HiddenField)row.FindControl("hidExtraTypeID");

                if ((bool)dr["IsActive"] || !m_isUpdate)
                    chkExtraTypesEnabled.Checked = true;
                else
                    chkExtraTypesEnabled.Checked = false;

                hidExtraTypeID.Value = ((int)dr["ExtraTypeID"]).ToString();
            }
        }
        

        protected void tcServiceLevel_DataBound(object sender, EventArgs e)
        {
            DataSet dsServiceLevels = (DataSet)gvServiceLevels.DataSource;

            foreach (GridViewRow row in gvServiceLevels.Rows)
            {
                DataRow dr = dsServiceLevels.Tables[0].Rows[row.RowIndex];
                CheckBox chkServiceLevelEnabled = (CheckBox)row.FindControl("chkServiceLevelEnabled");
                HiddenField hidServiceLevelID = (HiddenField)row.FindControl("hidServiceLevelID");
                RdoBtnGrouper cboServiceLevelIsDefault = (RdoBtnGrouper)row.FindControl("cboServiceLevelIsDefault");

                if ((bool)dr["IsActive"] || !m_isUpdate)
                    chkServiceLevelEnabled.Checked = true;
                else
                    chkServiceLevelEnabled.Checked = false;

                hidServiceLevelID.Value = ((int)dr["OrderServiceLevelID"]).ToString();
                if ((bool)dr["IsDefault"] || (row.RowIndex == 0 && !m_isUpdate))
                    cboServiceLevelIsDefault.Checked = true;
                else
                    cboServiceLevelIsDefault.Checked = false;
            }
        }

        protected void tcGoodsType_DataBound(object sender, EventArgs e)
        {
            DataSet dsGoodsType = (DataSet)tcGoodsType.DataSource;

            foreach (GridViewRow row in tcGoodsType.Rows)
            {
                DataRow dr = dsGoodsType.Tables[0].Rows[row.RowIndex];
                CheckBox chkGoodsTypeIsActive = (CheckBox)row.FindControl("chkGoodsTypeIsActive");
                HiddenField hidGoodsTypeId = (HiddenField)row.FindControl("hidGoodsTypeId");
                RdoBtnGrouper cboGoodsTypeIsDefault = (RdoBtnGrouper)row.FindControl("cboGoodsTypeIsDefault");

                if ((bool)dr["IsActive"] || !m_isUpdate)
                    chkGoodsTypeIsActive.Checked = true;
                else
                    chkGoodsTypeIsActive.Checked = false;

                hidGoodsTypeId.Value = ((int)dr["GoodsTypeId"]).ToString();
                if ((bool)dr["IsDefault"] || (row.RowIndex == 0 && !m_isUpdate))
                    cboGoodsTypeIsDefault.Checked = true;
                else
                    cboGoodsTypeIsDefault.Checked = false;
            }
        }

        protected void tcPalletType_DataBound(object sender, EventArgs e)
        {
            DataSet dsPalletType = (DataSet)tcPalletType.DataSource;

            foreach (GridViewRow row in tcPalletType.Rows)
            {
                DataRow dr = dsPalletType.Tables[0].Rows[row.RowIndex];
                CheckBox chkPalletTypeIsActive = (CheckBox)row.FindControl("chkPalletTypeIsActive");
                HiddenField hidPalletTypeId = (HiddenField)row.FindControl("hidPalletTypeId");
                CheckBox chkPalletTypeIsTracked = (CheckBox)row.FindControl("chkPalletTypeIsTracked");
                RdoBtnGrouper cboPalletTypeIsDefault = (RdoBtnGrouper)row.FindControl("cboPalletTypeIsDefault");

                if (!m_isUpdate)
                {
                    chkPalletTypeIsActive.Checked = (bool)dr["ForNewClientsByDefault"];
                    chkPalletTypeIsTracked.Checked = (bool)dr["TrackPallets"];
                    cboPalletTypeIsDefault.Checked = (bool)dr["PalletTypeIsDefault"];
                }
                else
                {
                    cboPalletTypeIsDefault.Checked = (bool)dr["IsDefault"];

                    if ((bool)dr["IsActive"])
                        chkPalletTypeIsActive.Checked = true;
                    else
                        chkPalletTypeIsActive.Checked = false;
                    if ((bool)dr["TrackPallets"])
                        chkPalletTypeIsTracked.Checked = true;
                    else
                        chkPalletTypeIsTracked.Checked = false;
                }
                hidPalletTypeId.Value = ((int)dr["PalletTypeId"]).ToString();
            }
        }

        void tcPoint_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            tcPoint.Items.Clear();
            string searchText = e.Text;

            Orchestrator.Facade.IPoint facPoint = new Orchestrator.Facade.Point();
            DataSet ds = facPoint.GetAllFiltered(searchText);

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
                rcItem.Value = dt.Rows[i]["PointId"].ToString();
                tcPoint.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }
        }

        void tcCollectionPointTown_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            tcClosestTown.Items.Clear();

            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetTownForTownName(e.Text);

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
                tcClosestTown.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }
        }

        void tcClosestTown_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            int countryId = 1;

            if (ViewState["CountryId"] != null)
                countryId = Convert.ToInt32(ViewState["CountryId"]);

            Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            tcClosestTown.Items.Clear();

            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetTownForTownName(e.Text);

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
                tcClosestTown.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }
        }

        //#12976 JBS
        //Make the DemurrageType visible when Extras are included
        private void tcIncludeExtraDetails_CheckedChanged(object sender, EventArgs e)
        {
            tcDemurrageType.Visible = tcIncludeExtraDetails.Checked;
        }

        protected void tcMoveColumnUp_Click(object sender, EventArgs e)
        {
            int selectedIndex = tcIncludedColumns.SelectedIndex;

            if (selectedIndex > 0)
            {
                ListItem selectedListItem = tcIncludedColumns.SelectedItem;
                tcIncludedColumns.Items.RemoveAt(selectedIndex);
                tcIncludedColumns.Items.Insert(selectedIndex - 1, selectedListItem);
            }
        }

        protected void tcMoveColumnDown_Click(object sender, EventArgs e)
        {

            int selectedIndex = tcIncludedColumns.SelectedIndex;
            // Can't move down if last item in list
            if (selectedIndex != tcIncludedColumns.Items.Count - 1)
            {
                ListItem selectedListItem = tcIncludedColumns.SelectedItem;
                tcIncludedColumns.Items.RemoveAt(selectedIndex);
                tcIncludedColumns.Items.Insert(selectedIndex + 1, selectedListItem);
            }
        }

        protected void tcAssignColumn_Click(object sender, EventArgs e)
        {
            if (tcIncludedColumns.Items.Count == C_MAX_LOG_COLUMNS_VS)
            {
                tcColumnError.Visible = true;
                tcColumnError.Text = "Maximum columns exceeded";
            }
            else
            {
                tcColumnError.Visible = false;
                string listDescription;
                if (tcExcludedColumns.SelectedItem != null)
                {
                    listDescription = tcExcludedColumns.SelectedItem.Text;
                    if (tcIncludedColumns.Items.FindByText(listDescription) == null)
                    {
                        tcIncludedColumns.Items.Add(tcExcludedColumns.Items.FindByText(listDescription));
                        tcIncludedColumns.SelectedIndex = 0;
                        tcExcludedColumns.Items.Remove(tcExcludedColumns.Items.FindByText(listDescription));
                    }
                }
                else
                {
                    tcColumnError.Visible = true;
                    tcColumnError.Text = "No column selected";
                }
            }
        }

        protected void tcUnassignColumn_Click(object sender, EventArgs e)
        {
            tcColumnError.Visible = false;

            string listDescription;
            if (tcIncludedColumns.SelectedItem != null)
            {
                listDescription = tcIncludedColumns.SelectedItem.Text;
                if (tcExcludedColumns.Items.FindByText(listDescription) == null)
                {
                    tcExcludedColumns.Items.Add(tcIncludedColumns.Items.FindByText(listDescription));
                    tcExcludedColumns.SelectedIndex = 0;
                    tcIncludedColumns.Items.Remove(tcIncludedColumns.Items.FindByText(listDescription));
                }
            }
            else
            {
                tcColumnError.Visible = true;
                tcColumnError.Text = "No column selected.";
            }
        }

        protected void tcLogFrequency_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ((eLogFrequency)Enum.Parse(typeof(eLogFrequency), tcLogFrequency.SelectedValue.Replace(" ", "")) == eLogFrequency.Weekly)
                tcPanelDay.Visible = true;
            else
                tcPanelDay.Visible = false;

        }

        protected void dgMPGeneralResourceInstructions_PreRender(object sender, EventArgs e)
        {
            foreach (DataGridItem item in tcMPGeneralResourceInstructions.Items)
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    if (item.ItemIndex == 0)
                        item.Cells[2].Enabled = false;
                    else if (item.ItemIndex == tcMPGeneralResourceInstructions.Items.Count - 1)
                        item.Cells[1].Enabled = false;
                }
                else if (item.ItemType == ListItemType.EditItem)
                {
                    item.Cells[1].Enabled = false;
                    item.Cells[2].Enabled = false;
                    item.Cells[4].Enabled = false;
                }
        }

        protected void dgMPGeneralResourceInstructions_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            Facade.IReportSetting facReportSetting = new Facade.ReportSetting();
            HiddenField hidReportSetting = null;
            int reportSettingId = 0;

            switch (e.CommandName.ToLower())
            {
                case "up":
                    hidReportSetting = (HiddenField)e.Item.FindControl("hidReportSetting");
                    if (int.TryParse(hidReportSetting.Value, out reportSettingId))
                        facReportSetting.MoveUp(reportSettingId, ((Entities.CustomPrincipal)Page.User).UserName);
                    break;
                case "down":
                    hidReportSetting = (HiddenField)e.Item.FindControl("hidReportSetting");
                    if (int.TryParse(hidReportSetting.Value, out reportSettingId))
                        facReportSetting.MoveDown(reportSettingId, ((Entities.CustomPrincipal)Page.User).UserName);
                    break;
                case "edit":
                    tcMPGeneralResourceInstructions.EditItemIndex = e.Item.ItemIndex;
                    break;
                case "cancel":
                    tcMPGeneralResourceInstructions.EditItemIndex = -1;
                    break;
                case "update":
                    if (Page.IsValid)
                    {
                        TextBox txtInstruction = (TextBox)e.Item.FindControl("txtInstruction");
                        hidReportSetting = (HiddenField)e.Item.FindControl("hidReportSetting");
                        if (int.TryParse(hidReportSetting.Value, out reportSettingId))
                            facReportSetting.Update(reportSettingId, txtInstruction.Text, ((Entities.CustomPrincipal)Page.User).UserName);
                        tcMPGeneralResourceInstructions.EditItemIndex = -1;
                    }
                    break;
                case "insert":
                    if (Page.IsValid)
                    {
                        TextBox txtNewInstruction = (TextBox)e.Item.FindControl("txtNewInstruction");
                        facReportSetting.Create(eReportType.Manifest, m_organisation.IdentityId, (int)Orchestrator.Reports.rptManifest.eReportSettingPortion.GeneralInstructions, txtNewInstruction.Text, ((Entities.CustomPrincipal)Page.User).UserName);
                    }
                    break;
                case "delete":
                    hidReportSetting = (HiddenField)e.Item.FindControl("hidReportSetting");
                    if (int.TryParse(hidReportSetting.Value, out reportSettingId))
                        facReportSetting.Delete(reportSettingId, ((Entities.CustomPrincipal)Page.User).UserName);
                    break;
            }

            tcMPGeneralResourceInstructions.DataSource = facReportSetting.GetReportSettings(eReportType.Manifest, m_organisation.IdentityId, (int)Orchestrator.Reports.rptManifest.eReportSettingPortion.GeneralInstructions);
            tcMPGeneralResourceInstructions.DataBind();
        }

        protected void dgMPGeneralResourceInstructions_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.EditItem)
            {
                DataRowView drv = (DataRowView)e.Item.DataItem;

                TextBox txtInstruction = (TextBox)e.Item.FindControl("txtInstruction");
                txtInstruction.Text = (string)drv["Data"];
                HiddenField hidReportSetting = (HiddenField)e.Item.FindControl("hidReportSetting");
                hidReportSetting.Value = ((int)drv["ReportSettingId"]).ToString();
            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView drv = (DataRowView)e.Item.DataItem;

                HiddenField hidReportSetting = (HiddenField)e.Item.FindControl("hidReportSetting");
                hidReportSetting.Value = ((int)drv["ReportSettingId"]).ToString();
            }
        }

        protected void dgMPContactNumbers_PreRender(object sender, EventArgs e)
        {
            foreach (DataGridItem item in tcMPContactNumbers.Items)
                if (item.ItemType == ListItemType.Item || item.ItemType == ListItemType.AlternatingItem)
                {
                    if (item.ItemIndex == 0)
                        item.Cells[4].Enabled = false;
                    else if (item.ItemIndex == tcMPContactNumbers.Items.Count - 1)
                        item.Cells[3].Enabled = false;
                }
                else if (item.ItemType == ListItemType.EditItem)
                {
                    item.Cells[3].Enabled = false;
                    item.Cells[4].Enabled = false;
                    item.Cells[6].Enabled = false;
                }
        }

        protected void dgMPContactNumbers_ItemCommand(object source, DataGridCommandEventArgs e)
        {
            Facade.IReportSetting facReportSetting = new Facade.ReportSetting();
            HiddenField hidReportSetting = null;
            int reportSettingId = 0;

            switch (e.CommandName.ToLower())
            {
                case "up":
                    hidReportSetting = (HiddenField)e.Item.FindControl("hidReportSetting");
                    if (int.TryParse(hidReportSetting.Value, out reportSettingId))
                        facReportSetting.MoveUp(reportSettingId, ((Entities.CustomPrincipal)Page.User).UserName);
                    break;
                case "down":
                    hidReportSetting = (HiddenField)e.Item.FindControl("hidReportSetting");
                    if (int.TryParse(hidReportSetting.Value, out reportSettingId))
                        facReportSetting.MoveDown(reportSettingId, ((Entities.CustomPrincipal)Page.User).UserName);
                    break;
                case "edit":
                    tcMPContactNumbers.EditItemIndex = e.Item.ItemIndex;
                    break;
                case "cancel":
                    tcMPContactNumbers.EditItemIndex = -1;
                    break;
                case "update":
                    if (Page.IsValid)
                    {
                        TextBox txtContactNumberName = (TextBox)e.Item.FindControl("txtContactNumberName");
                        TextBox txtContactNumberQuickdial = (TextBox)e.Item.FindControl("txtContactNumberQuickdial");
                        TextBox txtContactNumber = (TextBox)e.Item.FindControl("txtContactNumber");
                        string data = txtContactNumberName.Text + "|" + txtContactNumberQuickdial.Text + "|" + txtContactNumber.Text;
                        hidReportSetting = (HiddenField)e.Item.FindControl("hidReportSetting");
                        if (int.TryParse(hidReportSetting.Value, out reportSettingId))
                            facReportSetting.Update(reportSettingId, data, ((Entities.CustomPrincipal)Page.User).UserName);
                        tcMPContactNumbers.EditItemIndex = -1;
                    }
                    break;
                case "insert":
                    if (Page.IsValid)
                    {
                        TextBox txtNewContactNumberName = (TextBox)e.Item.FindControl("txtNewContactNumberName");
                        TextBox txtNewContactNumberQuickdial = (TextBox)e.Item.FindControl("txtNewContactNumberQuickdial");
                        TextBox txtNewContactNumber = (TextBox)e.Item.FindControl("txtNewContactNumber");
                        string data = txtNewContactNumberName.Text + "|" + txtNewContactNumberQuickdial.Text + "|" + txtNewContactNumber.Text;
                        facReportSetting.Create(eReportType.Manifest, m_organisation.IdentityId, (int)Orchestrator.Reports.rptManifest.eReportSettingPortion.ContactNumbers, data, ((Entities.CustomPrincipal)Page.User).UserName);
                    }
                    break;
                case "delete":
                    hidReportSetting = (HiddenField)e.Item.FindControl("hidReportSetting");
                    if (int.TryParse(hidReportSetting.Value, out reportSettingId))
                        facReportSetting.Delete(reportSettingId, ((Entities.CustomPrincipal)Page.User).UserName);
                    break;
            }

            tcMPContactNumbers.DataSource = facReportSetting.GetReportSettings(eReportType.Manifest, m_organisation.IdentityId, (int)Orchestrator.Reports.rptManifest.eReportSettingPortion.ContactNumbers);
            tcMPContactNumbers.DataBind();
        }

        protected void dgMPContactNumbers_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.EditItem)
            {
                DataRowView drv = (DataRowView)e.Item.DataItem;
                string data = (string)drv["Data"];
                string[] dataParts = data.Split('|');

                TextBox txtContactNumberName = (TextBox)e.Item.FindControl("txtContactNumberName");
                txtContactNumberName.Text = dataParts[0];
                TextBox txtContactNumberQuickdial = (TextBox)e.Item.FindControl("txtContactNumberQuickdial");
                txtContactNumberQuickdial.Text = dataParts[1];
                TextBox txtContactNumber = (TextBox)e.Item.FindControl("txtContactNumber");
                txtContactNumber.Text = dataParts[2];

                HiddenField hidReportSetting = (HiddenField)e.Item.FindControl("hidReportSetting");
                hidReportSetting.Value = ((int)drv["ReportSettingId"]).ToString();
            }
            else if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                DataRowView drv = (DataRowView)e.Item.DataItem;

                HiddenField hidReportSetting = (HiddenField)e.Item.FindControl("hidReportSetting");
                hidReportSetting.Value = ((int)drv["ReportSettingId"]).ToString();
            }
        }

        #region Address Lookup Changes

        private DataSet FindAddressByPostCode(int attempt, string postcode)
        {
            PostCodeAnywhere.LookupUK lookUp = new PostCodeAnywhere.LookupUK();

            try
            {
                return lookUp.ByPostcode_DataSet(postcode, accountCode, licenseKey, "");
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

            if (tcPostCode.Text.Length > 0)
            {
                ds = FindAddressByPostCode(1, tcPostCode.Text);
            }
            else if (tcAddressLine1.Text.Length > 1 && tcPostTown.Text.Length > 1)
            {
                ds = FindAddressNoPostCodeByStreet("%" + tcAddressLine1.Text, "%" + tcPostTown.Text);
            }

            if (ds != null && ds.Tables.Count > 0)
            {
                tcAddressList.DataSource = ds;
                tcAddressList.DataTextField = "description";
                tcAddressList.DataValueField = "id";
                tcAddressList.DataBind();

                tcAddressListPanel.Visible = true;
                tcAddressPanel.Visible = false;
            }
        }

        void lstAddress_SelectedIndexChanged(object sender, EventArgs e)
        {
            PostCodeAnywhere.LookupUK lookUp = new PostCodeAnywhere.LookupUK();
            PostCodeAnywhere.AddressResults address = lookUp.FetchAddress(tcAddressList.SelectedValue, PostCodeAnywhere.enLanguage.enLanguageEnglish, PostCodeAnywhere.enContentType.enContentGeographicAddress, accountCode, licenseKey, "");

            DisplayAddress(address);
            tcAddressListPanel.Visible = false;
            tcAddressPanel.Visible = true;
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
            return lookUp.ByOrganisation_DataSet(companyName, townName, false, accountCode, licenseKey, "");
        }

        protected void DisplayAddress(PostCodeAnywhere.AddressResults address)
        {
            tcAddressLine1.Text = address.Results[0].Line1;
            tcAddressLine2.Text = address.Results[0].Line2;
            tcAddressLine3.Text = address.Results[0].Line3;
            tcPostTown.Text = address.Results[0].PostTown;
            tcPostCode.Text = address.Results[0].Postcode;
            tcCounty.Text = address.Results[0].County;
            tcLatitude.Text = address.Results[0].GeographicData.WGS84Latitude.ToString();
            tcLongitude.Text = address.Results[0].GeographicData.WGS84Longitude.ToString();

            if (tcClosestTown.SelectedValue == "")
            {
                Facade.IPostTown facPostTown = new Facade.Point();
                Entities.PostTown postTown = facPostTown.GetPostTownForTownName(tcPostTown.Text);

                if (postTown != null)
                {
                    RadComboBoxItem item = new RadComboBoxItem(tcPostTown.Text, postTown.TownId.ToString());
                    tcClosestTown.Items.Add(item);
                    item.Selected = true;
                }
            }
        }

        #endregion Address Lookup Changes
       
    }
}
