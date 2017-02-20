using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Drawing;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using System.Transactions;

using Orchestrator.Globals;
using Orchestrator.WebUI.UserControls;


using P1TP.Components.Web.Validation;
using System.Threading;

namespace Orchestrator.WebUI.Invoicing
{
	/// <summary>
	/// Summary description for AddUpdateExtraInvoice.
	/// </summary>
	public partial class AddUpdateExtraInvoice : Orchestrator.Base.BasePage
	{
		#region Page Properties

		protected string SortDir
		{
			get {return (string)ViewState["sortDir"];}
			set { ViewState["sortDir"] = value;}
		}

		///	<summary> 
		///	Sort Criteria
		///	</summary
		protected string SortCriteria
		{
			get { return (string)ViewState["sortCriteria"];}
			set {ViewState["sortCriteria"] = value;}
		}

        private const string vs_dsTaxRates = "vs_dsTaxRates";
        protected DataSet dsTaxRates
        {
            get { return ViewState[vs_dsTaxRates] == null ? null : (DataSet)ViewState[vs_dsTaxRates]; }
            set { ViewState[vs_dsTaxRates] = value; }
        }

		#endregion

		#region Constants

		private const string CONFIRMATION_ADD_MESSAGE = "The extras invoice has been added successfully.";
		private const string CONFIRMATION_UPDATE_MESSAGE = "The extras invoice has been updated successfully.";
		private const string CONFIRMATION_ERROR_MESSAGE = "There was an error updating the extras invoice.";

		#endregion

		#region DataGrid Columns Enum

		private enum eAddExtrasDataGridColumns { ExtraId, JobId, ExtraType, ExtraState, ClientContact, ForeignAmount, Edit, AddToInvoice };//Edit, AddToInvoice };
		private enum eIncludedExtrasDataGridColumns { ExtraId, JobId, ExtraType, ExtraState, ClientContact, ForeignAmount,  RemoveFromInvoice};

		#endregion

		#region Private Fields

		#region Retrieved from Session state
		
		private int							m_identityId;
		private string						m_extraIdCSV;

		#endregion

		private Entities.ExtraCollection	m_extraCollection;
		private Entities.InvoiceExtra		m_invoiceExtra;

		private decimal						m_invoiceAmount;
		private int							m_invoiceId;

		private bool						m_isUpdate;
		
		#endregion

		#region Page Load

		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.Invoicing);

            if (Request.QueryString["InvoiceId"] != null || lblInvoiceNo.Text != Orchestrator.Globals.Constants.UNALLOCATED_INVOICE_ID)
				m_isUpdate = true;

            if (!IsPostBack)
            {
                ViewState["ExtraIdCSV"] = m_extraIdCSV = Convert.ToString(Session["ExtraIdCSV"]);
                ViewState["IdentityId"] = m_identityId = Convert.ToInt32(Session["IdentityId"]);

                if (m_isUpdate)
                {
                    ViewState["InvoiceId"] = m_invoiceId = Convert.ToInt32(Request.QueryString["InvoiceId"]);
                    LoadInvoiceExtra();
                    ViewInvoice();
                    btnChangeExtras.Visible = false;
                }
                else
                {
                    lblInvoiceNo.Text = Orchestrator.Globals.Constants.UNALLOCATED_INVOICE_ID;

                    ResetSessionVariables();

                    if (m_extraIdCSV == "" && m_identityId == 0)
                        Response.Redirect("InvoiceExtraPreparation.aspx");
                }

                ConfigurePageHeader(m_isUpdate);
                PopulateStaticControls();
            }
            else
            {
                m_identityId = int.Parse(ViewState["IdentityId"].ToString());
            }
		}

		#endregion

		#region Private Methods

		private void ConfigurePageHeader(bool isUpdate)
		{
			if (isUpdate)
			{
				Title.Text = "Update an extra invoice.";
                SubTitle.Text = "Update an extra invoice below.";

			}
			else
			{
                Title.Text = "Add an extra invoice.";
                SubTitle.Text = "Add an extra invoice below.";
			}
				
		}

		#region Load Invoice Extra

		private void LoadInvoiceExtra()
		{
			if (ViewState["InvoiceExtra"] == null)
			{
				Facade.IInvoiceExtra facInvoiceExtra = new Facade.Invoice();
				m_invoiceExtra = facInvoiceExtra.GetInvoiceExtraForInvoiceId(Convert.ToInt32(ViewState["InvoiceId"]));
				ViewState["InvoiceExtra"] = m_invoiceExtra;
			}
			else
			{
				m_invoiceExtra = (Entities.InvoiceExtra) ViewState["InvoiceExtra"];
			}

			lblClient.Text = GetInvoiceClient();
            rdiInvoiceDate.SelectedDate = m_invoiceExtra.InvoiceDate;
            rdiInvoiceDate.Enabled = !m_invoiceExtra.Posted;
			lblInvoiceNo.Text = m_invoiceExtra.InvoiceId.ToString();
			lblInvoiceNo.ForeColor = Color.Black;
			txtInvoiceDetails.Text = m_invoiceExtra.InvoiceDetails;
			lblDateCreated.Text = m_invoiceExtra.CreatedDate.ToString("dd/MM/yy");
			lblDateCreated.ForeColor = Color.Black;

            

            cboNominalCode.Enabled = !m_invoiceExtra.Posted;

			if (m_isUpdate)
			{
				foreach (Entities.Extra extra in m_invoiceExtra.Extras)
				{
					if (!(m_extraIdCSV == string.Empty))
					{
						m_extraIdCSV += ",";
					}
					m_extraIdCSV += extra.ExtraId;
				}
			}

			if (m_invoiceExtra.OverrideTotalAmountNet != 0)
			{
				chkOverrideAmount.Checked = true;
				pnlOverrideAmounts.Visible = true;

				txtReasonForOverride.Text = m_invoiceExtra.OverrideReason;
				txtOverrideAmountGross.Text = m_invoiceExtra.OverrideTotalAmountGross.ToString();
				txtOverrideAmountNet.Text = m_invoiceExtra.OverrideTotalAmountNet.ToString();
				txtOverrideAmountVAT.Text = m_invoiceExtra.OverrideTotalAmountVAT.ToString();
			}

			if (m_isUpdate)
			{
				if (m_invoiceExtra.ForCancellation)
				{
					btnAddUpdateInvoice.Visible = false;
					btnSendToAccounts.Visible = false;
					chkPostToExchequer.Visible = false;
					chkDelete.Checked = true;
				}
				else
				{
					btnAddUpdateInvoice.Visible = true;
					btnSendToAccounts.Visible = true;
					chkPostToExchequer.Visible = true;
					chkDelete.Checked = false;						
				}
			}
			else
				chkPostToExchequer.Visible = true;
			
			btnAddUpdateInvoice.Visible = true;
			btnAddUpdateInvoice.Text = "Update";

			if (m_invoiceExtra.Posted)
			{
				btnAddUpdateInvoice.Visible = false;
				btnSendToAccounts.Visible = false; 
				chkPostToExchequer.Checked = true;
				pnlInvoiceDeleted.Visible = false;
				chkDelete.Visible = false;
				txtAmountGross.Enabled = false;
				txtAmountNet.Enabled = false;
				txtInvoiceDetails.Enabled = false;
				txtInvoiceDetails.Enabled = false;
				chkIncludeExtraDetail.Enabled = false;
				chkOverrideAmount.Enabled = false;
				
				pnlOverrideAmounts.Enabled = false;				
                chkIncludeInvoiceDetail.Enabled = false; 
				
				btnViewInvoice.Visible = false;
			}
			else
			{
				btnAddUpdateInvoice.Visible = true;
				btnSendToAccounts.Visible = true; 
				chkPostToExchequer.Checked = false;
				pnlInvoiceDeleted.Visible = true; 
				chkDelete.Visible = true;
			}

			ViewState["ExtraIdCSV"] = m_extraIdCSV;
		}

        private void SetCurrencyTextboxCulture(int LCID)
        {
            CultureInfo culture = null;

            if (LCID == -1)
                culture = Thread.CurrentThread.CurrentCulture;
            else
                culture = new CultureInfo(LCID);

            this.txtAmountGross.Culture = culture;
            this.txtAmountNet.Culture = culture;
            this.txtOverrideAmountGross.Culture = culture;
            this.txtOverrideAmountNet.Culture = culture;
            this.txtOverrideAmountVAT.Culture = culture;
        }

		#endregion

		#region Populate Invoice Extra

		private void PopulateInvoiceExtra()
		{
			Facade.IInvoiceExtra facInvoiceExtra = new Facade.Invoice();

			if (m_invoiceExtra == null)
			{
				m_invoiceExtra = new Entities.InvoiceExtra();
			}

            if (lblInvoiceNo.Text != Orchestrator.Globals.Constants.UNALLOCATED_INVOICE_ID)
			{
				m_invoiceExtra.InvoiceId = int.Parse(lblInvoiceNo.Text);
				m_invoiceExtra.CreatedDate = Convert.ToDateTime(lblDateCreated.Text);
			}
			else
				m_invoiceExtra.CreatedDate = DateTime.Now;

            // Set the account code.
            Entities.Organisation orgClient = null;
            Facade.Organisation facOrg = new Orchestrator.Facade.Organisation();
            orgClient = facOrg.GetForIdentityId(m_identityId);
            m_invoiceExtra.AccountCode = orgClient.AccountCode;
            //

			m_invoiceExtra.InvoiceDate = this.rdiInvoiceDate.SelectedDate.Value;
			m_invoiceExtra.InvoiceType = eInvoiceType.Extra;
			m_invoiceExtra.ClientInvoiceAmount = Decimal.Parse(txtAmountNet.Text, NumberStyles.Currency);
			m_invoiceExtra.InvoiceDetails = txtInvoiceDetails.Text;
			m_invoiceExtra.CreatedDate = DateTime.Now;
            m_invoiceExtra.NominalCode = cboNominalCode.SelectedValue;

			if (chkOverrideAmount.Checked)
			{
				m_invoiceExtra.OverrideReason = txtReasonForOverride.Text;
				m_invoiceExtra.OverrideTotalAmountGross = Decimal.Parse(txtOverrideAmountGross.Text, NumberStyles.Currency);
				m_invoiceExtra.OverrideTotalAmountNet = Decimal.Parse(txtOverrideAmountNet.Text, NumberStyles.Currency);
				m_invoiceExtra.OverrideTotalAmountVAT = Decimal.Parse(txtOverrideAmountVAT.Text, NumberStyles.Currency);
			}
			else
			{
				m_invoiceExtra.OverrideReason = string.Empty;
				m_invoiceExtra.OverrideTotalAmountGross = 0;
				m_invoiceExtra.OverrideTotalAmountNet = 0;
				m_invoiceExtra.OverrideTotalAmountVAT = 0;
			}

			m_invoiceExtra.Extras = facInvoiceExtra.GetExtraCollectionForExtraIds(Convert.ToString(ViewState["ExtraIdCSV"]));

            int vatNo = 0, vatType;
            decimal vatRate = 0.00M;

            vatType = int.Parse(cboVATType.SelectedValue);

			// Vat Rate and Vat Type
			Facade.IInvoice facInv = new Facade.Invoice();
            facInv.GetVatRateForVatType((eVATType)vatType, m_invoiceExtra.InvoiceDate, out vatNo, out vatRate);
			m_invoiceExtra.VatRate = vatRate;
            m_invoiceExtra.VatNo = vatNo;

			if (chkPostToExchequer.Checked)
				m_invoiceExtra.Posted = true;  
			else
				m_invoiceExtra.Posted = false;

			if (chkDelete.Checked)
				m_invoiceExtra.ForCancellation = true;
			else
				m_invoiceExtra.ForCancellation = false;
		}

        private void UpdateInvoiceExtra()
        {
            m_invoiceExtra.InvoiceDate = this.rdiInvoiceDate.SelectedDate.Value;
            m_invoiceExtra.NominalCode = cboNominalCode.SelectedValue;

            int vatNo = 0, vatType;
            decimal vatRate = 0.00M;

            vatType = int.Parse(cboVATType.SelectedValue);

            // Vat Rate and Vat Type
            Facade.IInvoice facInv = new Facade.Invoice();
            facInv.GetVatRateForVatType((eVATType)vatType, m_invoiceExtra.InvoiceDate, out vatNo, out vatRate);
            m_invoiceExtra.VatRate = vatRate;
            m_invoiceExtra.VatNo = vatNo;

            if (chkDelete.Checked)
                m_invoiceExtra.ForCancellation = true;
            else
                m_invoiceExtra.ForCancellation = false;
        }

		#endregion
	
		private void ResetSessionVariables()
		{
			Session["ExtraIdCSV"] = null;
			Session["IdentityId"] = null;
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

            this.btnAddUpdateInvoice.Click += new EventHandler(btnAddUpdateInvoice_Click);
            this.btnChangeExtras.Click += new EventHandler(btnChangeExtras_Click);
            this.btnViewInvoice.Click += new EventHandler(btnViewInvoice_Click);
            this.btnSendToAccounts.Click += new EventHandler(btnSendToAccounts_Click);

            this.chkOverrideAmount.CheckedChanged += new EventHandler(chkOverrideAmount_CheckedChanged);

            this.cboVATType.SelectedIndexChanged += new EventHandler(cboVATType_SelectedIndexChanged);
		}
		
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    

		}
		#endregion

		#region Populate Static Controls

        private void LoadVatTypes()
        {
            Orchestrator.Facade.Invoice facInvoice = new Orchestrator.Facade.Invoice();
            dsTaxRates = facInvoice.GetTaxRates(this.rdiInvoiceDate.SelectedDate.Value);

            cboVATType.DataTextField = "Description";
            cboVATType.DataValueField = "VatTypeId";
            cboVATType.DataSource = dsTaxRates;
            cboVATType.DataBind();

            //select the Standard VAT Type
            int standard;
            if (m_invoiceExtra == null)
                standard = (int)eVATType.Standard;
            else
                standard = facInvoice.GetVatTypeForVatNo(m_invoiceExtra.VatNo);

            foreach (ListItem li in cboVATType.Items)
                if (li.Value == standard.ToString())
                {
                    li.Selected = true;
                    break;
                }
            
        }

		private void PopulateStaticControls()
		{
		    if (!m_isUpdate || rdiInvoiceDate.SelectedDate == null)
		        rdiInvoiceDate.SelectedDate = DateTime.Today;
            
            chkIncludeInvoiceDetail.Checked = Globals.Configuration.ShowInvoiceDetailsOnInvoiceExtraByDefault;

			decimal invoiceAmountNet = GetInvoiceAmount();
			txtAmountNet.Text = invoiceAmountNet.ToString("N2");

            LoadVatTypes();

            // Vat Rate and Vat Type
            int vatNo, vatType;
            decimal vatRate = 0m;
            vatType = int.Parse(cboVATType.SelectedValue);

            Facade.IInvoice facInv = new Facade.Invoice();
            facInv.GetVatRateForVatType((eVATType)vatType, this.rdiInvoiceDate.SelectedDate.Value, out vatNo, out vatRate);

            //This calculates the tax rate i.e 17.5% -> 17.5 / 100 = 0.175, 
            //                                          0.175 + 1 = 1.175
            decimal invoiceAmountGross = invoiceAmountNet * ((vatRate / 100) + 1);
			txtAmountGross.Text = invoiceAmountGross.ToString("N2");

            Facade.INominalCode facNominalCode = new Facade.NominalCode();
            DataSet dsNominalCode = facNominalCode.GetAllActive();
            DataTable dt = dsNominalCode.Tables[0];
            DataRow dr = dt.NewRow();
            dr["NominalCode"] = "";
            dr["Description"] = "Please Select a Nominal Code";
            dt.Rows.InsertAt(dr, 0);

            cboNominalCode.DataSource = dt;
            cboNominalCode.DataTextField = "Description";
            cboNominalCode.DataValueField = "NominalCode";
            cboNominalCode.DataBind();
            cboNominalCode.Items[0].Selected = true;

            if (m_invoiceExtra != null && cboNominalCode.FindItemByValue(m_invoiceExtra.NominalCode) != null)
                cboNominalCode.FindItemByValue(m_invoiceExtra.NominalCode).Selected = true;

			lblClient.Text = GetInvoiceClient();
		}

		private string GetInvoiceClient()
		{
			if (m_isUpdate)
			{
				Facade.IInvoiceExtra facInvoiceExtra = new Facade.Invoice();
				DataSet dsClient = facInvoiceExtra.GetClientForInvoiceExtraId(Convert.ToInt32(ViewState["InvoiceId"]));
				
				m_identityId = Convert.ToInt32(dsClient.Tables[0].Rows[0]["IdentityId"]);
				ViewState["IdentityId"] = m_identityId;

				return dsClient.Tables[0].Rows[0]["OrganisationName"].ToString();
			}
			else
			{
				Facade.IOrganisation facOrganisation = new Facade.Organisation();
				return facOrganisation.GetNameForIdentityId(m_identityId);
			}
		}

		private decimal GetInvoiceAmount()
		{
			Facade.IInvoiceExtra facInvoiceExtra = new Facade.Invoice();
			
			DataSet dsExtras = facInvoiceExtra.GetExtraDataSetForExtraIds(Convert.ToString(ViewState["ExtraIdCSV"]));

            m_invoiceAmount = 0;
			for (int i = 0; i < dsExtras.Tables[0].Rows.Count; i++)
				m_invoiceAmount += Convert.ToDecimal(dsExtras.Tables[0].Rows[i]["ForeignAmount"]);


            if (dsExtras.Tables[0].Rows.Count > 0)
            { 
                if(dsExtras.Tables[0].Rows[0]["LCID"] == DBNull.Value || Convert.ToInt32(dsExtras.Tables[0].Rows[0]["LCID"]) == -1 )
                    this.SetCurrencyTextboxCulture(-1);
                else
                    this.SetCurrencyTextboxCulture(Convert.ToInt32(dsExtras.Tables[0].Rows[0]["LCID"]));
            }
                
			return m_invoiceAmount;
		}

		#endregion

		#region ActiveReport

		private void ViewInvoice()
		{
			Facade.IInvoiceExtra facInvoiceExtra = new Facade.Invoice();
			
			DataSet dsExtras = facInvoiceExtra.GetExtraDataSetForExtraIds(Convert.ToString(ViewState["ExtraIdCSV"]));

			decimal totalAmount = 0.0M;

			for (int i = 0; i < dsExtras.Tables[0].Rows.Count; i++)
			{
				totalAmount += Convert.ToDecimal(dsExtras.Tables[0].Rows[i]["ForeignAmount"]);
			}

            reportViewer.IdentityId = m_identityId;
			NameValueCollection reportParams = new NameValueCollection();

            if (lblInvoiceNo.Text != Orchestrator.Globals.Constants.UNALLOCATED_INVOICE_ID)
				reportParams.Add("InvoiceNo", lblInvoiceNo.Text);
			else
				reportParams.Add("InvoiceNo", "0");

			if (chkIncludeExtraDetail.Checked)
				reportParams.Add("ExtraDetail", "Include");

			if (chkIncludeInvoiceDetail.Checked)
				reportParams.Add("InvoiceDetails", txtInvoiceDetails.Text);

			// Posted To Accounts
			if (chkPostToExchequer.Checked)
				reportParams.Add("Accounts", "true");

            int VatNo, vatType;
            decimal VatRate = 0m;

            if (m_invoiceExtra == null)
            {
                vatType = int.Parse(cboVATType.SelectedValue);
                Facade.IInvoice facInv = new Facade.Invoice();
                facInv.GetVatRateForVatType((eVATType)vatType, this.rdiInvoiceDate.SelectedDate.Value, out VatNo, out VatRate);
            }
            else
            {
                VatNo = m_invoiceExtra.VatNo;
                VatRate = m_invoiceExtra.VatRate;
            }

            reportParams.Add("InvoiceExtraVatNo", VatNo.ToString());
            reportParams.Add("InvoiceExtraVatRate", VatRate.ToString());
			reportParams.Add("TotalAmountNET", Convert.ToString(totalAmount));
			
			if (pnlOverrideAmounts.Visible)
			{
				reportParams.Add("OverrideNET", Convert.ToString(Decimal.Parse(txtOverrideAmountNet.Text,
												NumberStyles.Currency)));
				reportParams.Add("OverrideVAT", Convert.ToString(Decimal.Parse(txtOverrideAmountVAT.Text,
												NumberStyles.Currency)));
				reportParams.Add("OverrideGross", Convert.ToString(Decimal.Parse(txtOverrideAmountGross.Text,
												  NumberStyles.Currency)));
			}

			reportParams.Add("IdentityId", Convert.ToString(ViewState["IdentityId"]));
			reportParams.Add("OrganisationName", lblClient.Text);

			// Invoice Date
			reportParams.Add("InvoiceDate", this.rdiInvoiceDate.SelectedDate.Value.ToShortDateString ());

            Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.InvoiceExtra;
            Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = dsExtras;
            Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
            Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";
            Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;
							
			reportViewer.Visible = true;

			if (!chkPostToExchequer.Checked)
                btnAddUpdateInvoice.Visible = true;


			// Deleted
			if (chkDelete.Checked)
				Response.Redirect("../default.aspx");
		}

		#endregion

		#region Event Handlers

		#region Button Event Handlers

        protected void btnChangeExtras_Click(object sender, EventArgs e)
        {
            btnChangeExtras.DisableServerSideValidation();
            m_extraIdCSV = Convert.ToString(ViewState["ExtraIdCSV"]);
            m_identityId = Convert.ToInt32(ViewState["IdentityId"]);
            Session["_selectedExtras"] = m_extraIdCSV;
            Session["_selectedExtraIdentityId"] = m_identityId;
         
           Response.Redirect("invoiceextrapreparation.aspx?edit=true");
        }

        protected void btnViewInvoice_Click(object sender, EventArgs e)
		{
            if (Page.IsValid)
                ViewInvoice();
		}

        protected void cvNominalCode_Validate(object sender, ServerValidateEventArgs e)
        {

            if (e.Value == "Please Select a Nominal Code" && txtAmountNet.Value > 0)
                e.IsValid = false;
            else
                e.IsValid = true;

        }

		protected void btnAddUpdateInvoice_Click(object sender, EventArgs e)
		{
            if (Page.IsValid)
            {

                bool addUpdateSuccess = false;

                if (m_invoiceExtra == null)
                    PopulateInvoiceExtra();

                Facade.IInvoiceExtra facInvoiceExtra = new Facade.Invoice();

                if (m_isUpdate)
                {
                    #region Update
                    UpdateInvoiceExtra();

                    using (TransactionScope ts = new TransactionScope())
                    {

                        if (facInvoiceExtra.UpdateInvoiceExtra(m_invoiceExtra, this.txtAmountNet.Culture.LCID, ((Entities.CustomPrincipal)Page.User).UserName))
                        {
                            lblConfirmation.Text = CONFIRMATION_UPDATE_MESSAGE;
                            addUpdateSuccess = true;
                            rdiInvoiceDate.Enabled = !m_invoiceExtra.Posted;
                        }
                        else
                            lblConfirmation.Text = CONFIRMATION_ERROR_MESSAGE;

                        ts.Complete();
                    }
                    #endregion
                }
                else
                {
                    #region Create
                    m_extraIdCSV = (string)ViewState["ExtraIdCSV"];

                    Entities.FacadeResult result = facInvoiceExtra.CreateInvoiceExtra(m_invoiceExtra, this.txtAmountNet.Culture.LCID, Convert.ToInt32(ViewState["IdentityId"]), ((Entities.CustomPrincipal)Page.User).UserName, m_extraIdCSV);

                    if (result.Success)
                    {
                        lblConfirmation.Text = CONFIRMATION_ADD_MESSAGE;
                        btnAddUpdateInvoice.Text = "Update invoice";

                        addUpdateSuccess = true;
                        m_isUpdate = true;

                        ViewState["InvoiceId"] = result.ObjectId; // InvoiceId;
                        ViewState["InvoiceExtra"] = null;

                        //PopulateStaticControls();
                        LoadInvoiceExtra();
                    }
                    else
                    {
                        infringementDisplay.Infringements = result.Infringements;
                        infringementDisplay.DisplayInfringments();

                        lblConfirmation.Text = CONFIRMATION_ERROR_MESSAGE;
                    }
                    #endregion
                }

                lblConfirmation.Visible = true;

                if (addUpdateSuccess)
                    ViewInvoice();
            }
		}

		#endregion

		#region CheckBox Event Handlers

		private void chkOverrideAmount_CheckedChanged(object sender, EventArgs e)
		{
			pnlOverrideAmounts.Visible = chkOverrideAmount.Checked;
			
			if (pnlOverrideAmounts.Visible)
			{
				txtOverrideAmountNet.Text = txtAmountNet.Text;

				decimal vatAmount = Decimal.Parse(txtAmountNet.Text, NumberStyles.Currency)
					* (17.50M / 100M);

				txtOverrideAmountVAT.Text = vatAmount.ToString();

				decimal totalAmountGross = Decimal.Parse(txtAmountNet.Text, NumberStyles.Currency)
					+ vatAmount;

				txtOverrideAmountGross.Text = totalAmountGross.ToString();
			}
		}

		protected void cboSelectExtraState_SelectedIndexChanged(object sender, EventArgs e)
		{
			eExtraState state = (eExtraState) Enum.Parse(typeof(eExtraState), ((DropDownList) sender).SelectedValue);

			DataGridItem containingItem = ((DataGridItem) ((DropDownList) sender).Parent.Parent);

			RequiredFieldValidator rfvClientContact = (RequiredFieldValidator) containingItem.FindControl("rfvClientContact");

			rfvClientContact.Enabled = (state == eExtraState.Accepted || state == eExtraState.Refused);
		}

		#endregion

        #region Drop Down List

        void cboVATType_SelectedIndexChanged(object sender, EventArgs e)
        {
            int vatNo, vatType;
            decimal vatRate = 0m, invoiceAmountNet = 0m, invoiceAmountGross = 0m;
            Facade.IInvoice facInv = new Facade.Invoice();

            // Get current Net amount.
            invoiceAmountNet = txtAmountNet.Value.HasValue ? (decimal)txtAmountNet.Value.Value : 0m;

            vatType = int.Parse(cboVATType.SelectedValue);

            // Get the VAT rate for the newly selected VAT Type.
            facInv.GetVatRateForVatType((eVATType)vatType, this.rdiInvoiceDate.SelectedDate.Value, out vatNo, out vatRate);

            // Update the Gross Amount.
            invoiceAmountGross = invoiceAmountNet * ((vatRate / 100) + 1);
            txtAmountGross.Text = invoiceAmountGross.ToString("N2");
        }

        #endregion

        #endregion

        protected void btnSendToAccounts_Click(object sender, System.EventArgs e)
		{
			bool retVal = false;
			chkPostToExchequer.Checked = true;

			if (m_invoiceExtra ==null)
				PopulateInvoiceExtra();

			Facade.IInvoiceExtra   facInvoiceExtra = new Facade.Invoice();

            lblConfirmation.Text = string.Empty;

			if (m_isUpdate)
			{
                string applicationExceptionErrorMessage = string.Empty;
                bool postSuccess = false;

                try 
	            {
                    postSuccess = facInvoiceExtra.UpdateInvoiceExtra(m_invoiceExtra, this.txtAmountNet.Culture.LCID, ((Entities.CustomPrincipal)Page.User).UserName);    	
	            }   
	            catch (ApplicationException aex)
	            {
                    if (aex.InnerException.Message.ToLower().Contains("client account") && aex.InnerException.Message.ToLower().Contains("does not exist"))
                        applicationExceptionErrorMessage = aex.InnerException.Message;
                    else
                        throw;
	            }

                if (postSuccess)
                {
                    lblConfirmation.Text = CONFIRMATION_UPDATE_MESSAGE;
                    retVal = true;
                }
                else
                {
                    if (applicationExceptionErrorMessage != string.Empty)
                        lblConfirmation.Text = applicationExceptionErrorMessage;
                    else
                        lblConfirmation.Text = CONFIRMATION_ERROR_MESSAGE;
                }
			}

			if (retVal) 
			{
                if (lblConfirmation.Text == string.Empty)
                    lblConfirmation.Text = "The Invoice has been posted to accounts.";
				
				chkPostToExchequer.Visible = true;
				chkPostToExchequer.Checked = true;
			
				// Disable all buttons now that the invoice has been posted
				btnAddUpdateInvoice.Enabled = false;
				btnSendToAccounts.Enabled = false;
			}
			else
			{
                if (lblConfirmation.Text == string.Empty)
                    lblConfirmation.Text = "The Invoice has not been posted to accounts.";
				
				chkPostToExchequer.Checked = false;
			}
			
			LoadInvoiceExtra(); 
			ViewInvoice ();

            lblConfirmation.Visible = true;
		}

	}
}
