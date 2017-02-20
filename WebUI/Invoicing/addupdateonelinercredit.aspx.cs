using System;
using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using System.Transactions;


using Orchestrator.WebUI.UserControls;
using Orchestrator.Globals;
using Orchestrator.WebUI.Security;
using Orchestrator.Entities;
using P1TP.Components.Web.Validation;
using System.Globalization;
using System.Text;
using System.Configuration;
using DataDynamics.ActiveReports.Export.Pdf;
using System.IO;
using System.Collections.Generic;
using System.Net.Mail;
using System.Net;

namespace Orchestrator.WebUI.Invoicing
{
	/// <summary>
	/// Summary description for AddUpdateOneLinerCreditNote.
	/// </summary>
	public partial class AddUpdateOneLinerCreditNote : Orchestrator.Base.BasePage
	{
		#region Page Variables
		
		private bool				m_isUpdate = false;
		private int					m_CreditNoteNo = 0;
		private Entities.CreditNote	m_CreditNote;

		#endregion 

		#region Form Elements
		

		#endregion 

		#region Page Load/Init
		protected void Page_Load(object sender, System.EventArgs e)
		{
			Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.Invoicing);
  
			// Update CreditNote
			m_CreditNoteNo = Convert.ToInt32(Request.QueryString["CreditNoteId"]);  
			
			if (m_CreditNoteNo > 0 || lblCreditNoteNo.Text != "To Be Issued ... (This Credit Note has not yet been saved, add Credit Note to allocate Credit Note No.)" )
				m_isUpdate = true;	

			if (!IsPostBack)
			{
                PopulateStaticControls();
                
                this.Title = "Add/Update One Liner Credit";
                LoadVatTypes();

                Utilities.ClearInvoiceSession();
				
                if (m_isUpdate)
                    LoadCreditNote();
			}
		}

        private void LoadVatTypes()
        {
            Orchestrator.Facade.Invoice facInvoice = new Orchestrator.Facade.Invoice();
            DataSet dsTaxRates = facInvoice.GetTaxRates(this.rdiCreditNoteDate.SelectedDate.Value);

            cboVATType.DataTextField = "Description";
            cboVATType.DataValueField = "VATTypeId";
            cboVATType.DataSource = dsTaxRates;
            cboVATType.DataBind();

            //select the Standard VAT Type
            string standard = ((int)eVATType.Standard).ToString();
            foreach (ListItem li in cboVATType.Items)
                if (li.Value == standard)
                {
                    li.Selected = true;
                    break;
                }
        }   	
		
		protected void AddUpdateOneLinerCreditNote_Init(object sender, EventArgs e)
		{
			this.btnAdd.Click +=new EventHandler(btnAdd_Click);
			this.btnViewCreditNote.Click +=new EventHandler(btnViewCreditNote_Click);	 
			this.btnSendToAccounts.Click +=new EventHandler(btnSendToAccounts_Click);
			// this.btnSendToAccounts.Click += new System.EventHandler(this.btnSendToAccounts_Click);
            this.cboClient.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);
            this.cboClient.SelectedIndexChanged += new Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventHandler(cboClient_SelectedIndexChanged);
            this.btnEmailCreditNote.Click += new EventHandler(btnEmailCreditNote_Click);
        }



        void cboClient_SelectedIndexChanged(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
               // Get the Account Codes(s) for the Customer
                Facade.IOrganisation facOrg = new Facade.Organisation();
                Entities.Organisation org = facOrg.GetForIdentityId(int.Parse(cboClient.SelectedValue));

                lblAccountCode.Text = org.AccountCode;

                CultureInfo culture = new CultureInfo(org.LCID);
                this.txtNETAmount.Culture = culture;
           
        }

		#endregion 
		
		#region Populate Static Controls
		///	<summary> 
		/// Populate Static Controls
		///	</summary>
		private void PopulateStaticControls()
		{
            rdiCreditNoteDate.SelectedDate = DateTime.Now;

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

			
			//lblCreditNoteType.Text = eCreditNoteType.OneLiner.ToString();

			//-----------------------------------------------------------------------------
			//									Update Section
			//-----------------------------------------------------------------------------
			if (m_isUpdate) 
			{
				lblCreditNoteNo.Visible = true;
				lblCreditNoteNo.Text = m_CreditNoteNo.ToString();
				btnAdd.Text = "Update Credit Note";
			}
		}
		#endregion 

		#region Add/Update/Load/Populate CreditNote
		///	<summary> 
		/// Load CreditNote
		///	</summary>
		private void LoadCreditNote()
		{
			if (ViewState["CreditNote"]==null)
			{
				Facade.CreditNote facCreditNote = new Facade.CreditNote();
				m_CreditNote = facCreditNote.GetForCreditNoteId(m_CreditNoteNo);
				
				ViewState["CreditNote"] = m_CreditNote;
			}
			else
				m_CreditNote = (Entities.CreditNote)ViewState["CreditNote"];

			// Load the report with the relevant details
			if (m_CreditNote != null)
			{
				lblCreditNoteNo.Text = m_CreditNote.CreditNoteId.ToString();  
				lblCreditNoteNo.ForeColor = Color.Black;
				
				//lblCreditNoteType.Text = m_CreditNote.CreditNoteType.ToString();
				
				lblDateCreated.Text = m_CreditNote.CreateDate.ToShortDateString(); 
				lblDateCreated.ForeColor = Color.Black;
                
                lblAccountCode.Text = m_CreditNote.AccountCode;

                rdiCreditNoteDate.SelectedDate = m_CreditNote.CreditNoteDate;
                rdiCreditNoteDate.Enabled = !m_CreditNote.Posted;
				
                rtbTxtReason.Text = m_CreditNote.CreditNoteDetails; 
				txtNETAmount.Text = m_CreditNote.ForeignNetAmount.ToString("N2");

                if (cboNominalCode.FindItemByValue(m_CreditNote.NominalCode) != null)
                    cboNominalCode.FindItemByValue(m_CreditNote.NominalCode).Selected = true;

                cboNominalCode.Enabled = !m_CreditNote.Posted;

                //cboVATType.SelectedValue = Convert.ToInt32(m_CreditNote.VatNo).ToString();

                // Load Client
                //TODO Is this necessary?
                Facade.IOrganisation facOrganisation = new Facade.Organisation();
                Entities.Organisation client = facOrganisation.GetForIdentityId(m_CreditNote.ClientId);

                cboClient.SelectedValue = m_CreditNote.ClientId.ToString();
                cboClient.Text = client.OrganisationName;
                cboClient.Enabled = false;

                if (m_isUpdate)
                {
                    // TODO: the culture will have to come from the invoice when it is available.
                    CultureInfo culture = new CultureInfo(m_CreditNote.LCID);
                    this.txtNETAmount.Culture = culture;

                    btnAdd.Visible = true;
                    btnSendToAccounts.Visible = true;
                    chkPostToExchequer.Visible = true;
                    chkDelete.Checked = false;

                    this.Title = "Update Credit Note";
                    btnAdd.Text = "Update";
                }
                else
                {
                    // take the culture from the client if we are adding
                    CultureInfo culture = new CultureInfo(client.LCID);
                    this.txtNETAmount.Culture = culture;

                    chkPostToExchequer.Visible = true;
                }
                                                        
				chkPostToExchequer.Visible = true;
			 
				LoadReport();

                if (client != null && (client.IndividualContacts != null && client.IndividualContacts.Count > 0))
                {
                    DataSet emailDataSet = this.GetContactDataSet(client.IndividualContacts, eContactType.Email);

                    cboEmail.DataSource = emailDataSet;
                    cboEmail.DataMember = "contactTable";
                    cboEmail.DataValueField = "ContactDetail";
                    cboEmail.DataTextField = "ContactDisplay";
                    cboEmail.DataBind();

                    if (this.cboEmail.Items.Count > 0)
                        this.txtEmail.Text = this.cboEmail.Items[0].Value;
                }

                if (m_CreditNote.Posted)
				{
					btnAdd.Visible = false;
					btnSendToAccounts.Visible = false; 
					chkPostToExchequer.Checked = true;
					pnlCreditNoteDeleted.Visible = false;
					chkDelete.Visible = false;
					chkPostToExchequer.Checked = true;
					chkPostToExchequer.Visible = true;
					txtNETAmount.Enabled = false;
					rtbTxtReason.Enabled = false;
					btnSendToAccounts.Visible = false;
					btnViewCreditNote.Visible = false; 
					rdiCreditNoteDate.Enabled = false;
					rtbTxtReason.Enabled = false;
					txtNETAmount.Enabled = false;
                    cboVATType.Enabled = false;
				}
				else
				{
					btnAdd.Visible = true;
					btnSendToAccounts.Visible = true; 
					chkPostToExchequer.Checked = false;
                    //A delete function may be rquired at some point
					//pnlCreditNoteDeleted.Visible = true; 
					//chkDelete.Visible = true;
				}
			}
			
			this.Title = "Update Credit Note";
			//Header1.subTitle = "Please make any changes neccessary.";
			btnAdd.Text = "Update";
		}

        private DataSet GetContactDataSet(List<Entities.Individual> individualContacts, eContactType contactType)
        {
            DataSet contactsDataSet = new DataSet();
            DataTable contactTable = new DataTable("contactTable");

            string contactNameColumn = "ContactName";
            string contactDetailColumn = "ContactDetail";
            string ContactDisplayColumn = "ContactDisplay";

            contactTable.Columns.Add(contactNameColumn);
            contactTable.Columns.Add(contactDetailColumn);
            contactTable.Columns.Add(ContactDisplayColumn);

            contactsDataSet.Tables.Add(contactTable);

            foreach (Entities.Individual individual in individualContacts)
                if (individual.Contacts != null)
                {
                    Entities.Contact contact = individual.Contacts.GetForContactType(contactType);
                    if (contact != null)
                    {
                        DataRow row = contactsDataSet.Tables[0].NewRow();

                        row[contactNameColumn] = String.Format("{0} {1} {2}",
                            individual.Title, individual.FirstNames, individual.LastName);

                        row[contactDetailColumn] = contact.ContactDetail;

                        row[ContactDisplayColumn] = String.Format("{0} - {1}", row[contactNameColumn], row[contactDetailColumn]);

                        contactsDataSet.Tables[0].Rows.Add(row);
                    }
                }

            return contactsDataSet;
        }

        ///	<summary> 
        /// Populate CreditNote
        ///	</summary>
        private void populateCreditNote()
		{
			if (ViewState["CreditNote"] == null)
				m_CreditNote = new Entities.CreditNote();
			else
				m_CreditNote = (Entities.CreditNote)ViewState["CreditNote"];
			
			if (lblCreditNoteNo.Text != "To Be Issued ... (This Credit Note has not yet been saved, add Credit Note to allocate Credit Note No.)")
				m_CreditNote.CreditNoteId = Convert.ToInt32(lblCreditNoteNo.Text);
			
			//m_CreditNote.CreditNoteType = eCreditNoteType.OneLiner;
            m_CreditNote.ClientId = Convert.ToInt32(cboClient.SelectedValue);

			m_CreditNote.CreditNoteDetails = rtbTxtReason.Text;


			if (chkPostToExchequer.Checked)
				m_CreditNote.Posted = true;  
			else
				m_CreditNote.Posted = false;

            int vatNo = 0;
            decimal vatRate = 0.00M;

			//Get VAT Rate for VAT Type
			Facade.IInvoice  facInv = new Facade.Invoice();
            eVATType vatType = (eVATType)int.Parse(cboVATType.SelectedValue);
            facInv.GetVatRateForVatType(vatType, this.rdiCreditNoteDate.SelectedDate.Value, out vatNo, out vatRate);

            m_CreditNote.VatNo = vatNo;
            m_CreditNote.VatRate = vatRate;

            Facade.IExchangeRates facER = new Facade.ExchangeRates();

            if (this.txtNETAmount.Culture.LCID != 2057)
            {
                m_CreditNote.LCID = this.txtNETAmount.Culture.LCID;

                m_CreditNote.ExchangeRateID =
                    facER.GetCurrentExchangeRateID(Facade.Culture.GetCurrencySymbol(m_CreditNote.LCID),
                    this.rdiCreditNoteDate.SelectedDate.Value);
            }
            else
            {
                m_CreditNote.ExchangeRateID = null;
                m_CreditNote.LCID = 2057; //English
            }

            //Calculate VATamount and GrossAmount
            //#16479 Round all amounts to 2 decimal places
            m_CreditNote.ForeignNetAmount = Math.Round(Decimal.Parse(txtNETAmount.Text, System.Globalization.NumberStyles.Currency), 2);
            m_CreditNote.ForeignVATAmount = Math.Round((m_CreditNote.ForeignNetAmount * m_CreditNote.VatRate) / 100, 2);
            m_CreditNote.ForeignGrossAmount = Math.Round(m_CreditNote.ForeignNetAmount + m_CreditNote.ForeignVATAmount, 2);

            if (m_CreditNote.ExchangeRateID != null)
            {
                m_CreditNote.NetAmount = facER.GetConvertedRate((int)m_CreditNote.ExchangeRateID,
                    m_CreditNote.ForeignNetAmount);
                m_CreditNote.VATamount = facER.GetConvertedRate((int)m_CreditNote.ExchangeRateID,
                    m_CreditNote.ForeignVATAmount);
                m_CreditNote.GrossAmount = facER.GetConvertedRate((int)m_CreditNote.ExchangeRateID,
                    m_CreditNote.ForeignGrossAmount);
            }
            else
            {
                m_CreditNote.NetAmount = m_CreditNote.ForeignNetAmount;
                m_CreditNote.VATamount = m_CreditNote.ForeignVATAmount;
                m_CreditNote.GrossAmount = m_CreditNote.ForeignGrossAmount;
            }

            m_CreditNote.AccountCode = lblAccountCode.Text;
            m_CreditNote.NominalCode = cboNominalCode.SelectedValue;

            m_CreditNote.CreditNoteDate = rdiCreditNoteDate.SelectedDate.Value;
		}

		
		///	<summary> 
		/// Update CreditNote
		///	</summary>
		private void UpdateCreditNote()
		{
			Facade.CreditNote facCreditNote = new Facade.CreditNote();

			string userName = ((Entities.CustomPrincipal)Page.User).UserName;

			facCreditNote.UpdateOneLiner(m_CreditNote, userName);
		}
	
	
		///	<summary> 
		/// Add CreditNote
		///	</summary>
		private int AddCreditNote()
		{
			int CreditNoteId = 0;
			Facade.CreditNote facCreditNote = new Facade.CreditNote();

			string userName = ((Entities.CustomPrincipal)Page.User).UserName;
			
			CreditNoteId = facCreditNote.CreateOneLiner(m_CreditNote, userName);
            
			if (CreditNoteId == 0)
			{
				lblConfirmation.Text = "There was an error adding the Credit Note, please try again.";
				lblConfirmation.Visible = true;
				lblConfirmation.ForeColor = Color.Red;
			}
			else
			{
				// NO NEED: Alreayd done within the Data Access Layer.

				// Reference CreditNote Id With Client
				//int clientId = Convert.ToInt32(cboClient.SelectedValue);
				//Facade.ICreditNoteOneLiner facInv = new Facade.CreditNote();
				//facInv.CreateCreditNoteIdWithClientId(CreditNoteId, clientId);
				// Done in the Data Access Layer.
			}
	
			return CreditNoteId;
		}

		#endregion
		
		#region Methods & Functions
		/// <summary>
		/// Load Report Depending On CreditNote Type
		/// </summary>
		private void LoadReport()
		{

			// Configure the Session variables used to pass data to the report
			NameValueCollection reportParams = new NameValueCollection();

			//-------------------------------------------------------------------------------------	
			//						Job/Collect-Drops/References/Demurrages Section
			//-------------------------------------------------------------------------------------	
			//Facade.CreditNote facCreditNote = new Facade.CreditNote();
	
			if (m_isUpdate)
			{
				btnSendToAccounts.Visible = true;
                //A delete function may be rquired at some point
				//pnlCreditNoteDeleted.Visible = true; 
			}
		
			// Client Name & Id			
			reportParams.Add("Client", cboClient.Text);
			
			reportParams.Add("ClientId", cboClient.SelectedValue);

			// Details
			reportParams.Add("Reason", rtbTxtReason.Text);
		
			// CreditNote Id
			if (lblCreditNoteNo.Text != "To Be Issued ... (This Credit Note has not yet been saved, add Credit Note to allocate Credit Note No.)")
				reportParams.Add("CreditNoteId", lblCreditNoteNo.Text);
			else
				reportParams.Add("CreditNoteId", "0");

			reportParams.Add("NetAmount", decimal.Parse(txtNETAmount.Text, System.Globalization.NumberStyles.Currency).ToString());
			 
			// Posted To Accounts
			if (chkPostToExchequer.Checked)
				reportParams.Add("Accounts", "true");

            int vatNo = 0;
            decimal vatRate = 0.00M;

            //Get VAT Rate for VAT Type
            Facade.IInvoice facInv = new Facade.Invoice();
            eVATType vatType = (eVATType)int.Parse(cboVATType.SelectedValue);
            facInv.GetVatRateForVatType(vatType, rdiCreditNoteDate.SelectedDate.Value, out vatNo, out vatRate);

			reportParams.Add("VATrate", vatRate.ToString());

			// CreditNote Date 
			reportParams.Add("CreditNoteDate", rdiCreditNoteDate.SelectedDate.Value.ToShortDateString ());

			//-------------------------------------------------------------------------------------	
			//									Load Report Section 
			//-------------------------------------------------------------------------------------	
            Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.CreditNoteOneLiner;
//			Session[Orchestrator.Globals.Constants.ReportDataSessionVariable] = dsInv;
//			Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";
            Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;

            bool enforceDataExistence;
            var activeReport = Orchestrator.Reports.Utilities.GetActiveReport(eReportType.CreditNoteOneLiner, "", out enforceDataExistence);
            activeReport.Run(false);

            if (m_CreditNote == null)
                populateCreditNote();

            // Generate the location this report's pdf will be output to.
            StringBuilder sbFilename = new StringBuilder(ConfigurationManager.AppSettings["GeneratedPDFRoot"]);
            sbFilename.Append("\\CreditNotes\\");
            sbFilename.Append(m_CreditNote.CreditNoteDate.Year.ToString());
            sbFilename.Append("\\");
            sbFilename.Append(m_CreditNote.CreditNoteDate.Month.ToString());
            sbFilename.Append("\\");
            sbFilename.Append(m_CreditNote.CreditNoteDate.Day.ToString());
            sbFilename.Append("\\Credit_Note_");
            sbFilename.Append(m_CreditNote.CreditNoteId);

            sbFilename.Append(".pdf");
            string filename = sbFilename.ToString();
            string oldPDFLocation = m_CreditNote.PDFLocation;

            PdfExport pdf = new PdfExport();
            Facade.CreditNote facCreditNote = new Facade.CreditNote();

            //If the invoice id is 0 then the invoice hasn't been created so don't save it
            if (m_CreditNote.CreditNoteId != 0)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filename));

                //Export the report to PDF.
                pdf.Export(activeReport.Document, filename);

                string pdfRoot = ConfigurationManager.AppSettings["GeneratedPDFRoot"];
                filename = filename.Remove(0, (pdfRoot != null ? pdfRoot.Length : 0));

                m_CreditNote.PDFLocation = filename;

                string userName = ((Entities.CustomPrincipal)Page.User).UserName;

                facCreditNote.UpdatePDFLocation(m_CreditNote.CreditNoteId, filename, userName);

                pdfViewer.Src = Globals.Configuration.WebServer + filename;

                lblEmail.Visible = true;
                btnEmailCreditNote.Visible = true;
                cboEmail.Visible = true;
                rfvEmailAddress.Enabled = true;
                revEmailAddress.Enabled = true;
            }
            else
            {
                //Instead of saving the pdf just output the memory stream
                var ms = new System.IO.MemoryStream();
                pdf.Export(activeReport.Document, ms);

                pdfViewer.Src = "data:application/pdf;base64," + Convert.ToBase64String(ms.ToArray());

                lblEmail.Visible = false;
                btnEmailCreditNote.Visible = false;
                cboEmail.Visible = false;
                rfvEmailAddress.Enabled = false;
                revEmailAddress.Enabled = false;
            }

            pdfViewer.Visible = true;
			
			// Show the add CreditNote button. 
			if (!chkPostToExchequer.Checked)
				btnAdd.Visible = true;

			// Deleted
			if (chkDelete.Checked)
				Response.Redirect("../default.aspx");
		}

		protected void btnAdd_Click(object sender, EventArgs e)
		{
			int CreditNoteId = 0;

			if (m_CreditNote==null)
				populateCreditNote();

			if (m_isUpdate) 
				UpdateCreditNote();
			else
				CreditNoteId = AddCreditNote();

			if (m_isUpdate) 
			{
				lblConfirmation.Text = "The Credit Note has been updated successfully.";
				LoadReport();
			}
			else
			{
				lblConfirmation.Text = "The Credit Note has been added successfully.";
				lblCreditNoteNo.Text = CreditNoteId.ToString();
				lblCreditNoteNo.ForeColor = Color.Black;
				lblDateCreated.Text = DateTime.Now.ToShortDateString();
				lblDateCreated.ForeColor = Color.Black;
				btnAdd.Text = "Update Credit Note";
                m_CreditNoteNo = CreditNoteId;
				LoadCreditNote();
				m_isUpdate = true;
				LoadReport();
			}
			
			lblConfirmation.Visible=true;
			cboClient.Enabled = false;
			chkPostToExchequer.Visible = true;
		}

		protected void btnViewCreditNote_Click(object sender, EventArgs e)
		{


            this.Validate();
            
			if (Page.IsValid)
			{
				btnViewCreditNote.Visible = true;
				LoadReport();	
			}
		}

        protected void btnEmailCreditNote_Click(object sender, EventArgs e)
        {
            if (ViewState["CreditNote"] != null)
            {
                m_CreditNote = (Entities.CreditNote)ViewState["CreditNote"];

                using (MailMessage mailMessage = new System.Net.Mail.MailMessage())
                {
                    mailMessage.From = new MailAddress(Orchestrator.Globals.Configuration.MailFromAddress, Orchestrator.Globals.Configuration.MailFromName);

                    string emailAddress = this.cboEmail.Text;

                    if (this.cboEmail.SelectedValue != String.Empty)
                        emailAddress = this.cboEmail.SelectedValue;

                    mailMessage.To.Add(new MailAddress(emailAddress));

                    mailMessage.Attachments.Add(new Attachment(Server.MapPath(m_CreditNote.PDFLocation)));
                    mailMessage.Subject = Orchestrator.Globals.Configuration.InstallationCompanyName + " Credit Note Number " + m_CreditNote.CreditNoteId.ToString();
                    mailMessage.IsBodyHtml = false;

                    // We need to get/determine what text should be attached to the email for the invoice.
                    string body = string.Format("Please find attached Credit Note “{0}” from {1}. If you require POD’s please access the self service website here {2}. Please contact Customer Services on {3}  for additional help,\n\n Regards", m_CreditNote.CreditNoteId.ToString(), Orchestrator.Globals.Configuration.InstallationCompanyName, Orchestrator.Globals.Configuration.OrchestratorURL, Orchestrator.Globals.Configuration.InstallationCustomerServicesNumber);
                    mailMessage.Body = body;

                    SmtpClient smtp = new System.Net.Mail.SmtpClient();
                    smtp.Host = Globals.Configuration.MailServer;
                    smtp.Credentials = new NetworkCredential(Globals.Configuration.MailUsername, Globals.Configuration.MailPassword);

                    smtp.Send(mailMessage);
                    lblEmailSent.Visible = true;
                }
            }
        }


        protected void btnSendToAccounts_Click(object sender, EventArgs e)
		{		  
            //Set The flag to indicate that it should be posted
            chkPostToExchequer.Checked = true;

			if (m_CreditNote==null)
				populateCreditNote();

            LoadReport();

            try
            {
                m_CreditNote.Posted = true;
                UpdateCreditNote();

                lblConfirmation.Text = "The Credit Note has been posted to accounts.";
                chkPostToExchequer.Visible = true;
                chkPostToExchequer.Checked = true;

                btnAdd.Enabled = false;
                btnSendToAccounts.Enabled = false;
            }
            catch (Exception ex)
            {
                lblConfirmation.Text = "The Credit Note has not been posted to accounts.";
                chkPostToExchequer.Checked = false;
            }

			LoadReport();
			
			lblConfirmation.Visible=true;
			cboClient.Enabled = false;
		}

		private void cboClient_SelectedItemChanged(object sender, EventArgs e)
		{	
			bool onHold = false;
			
			Facade.IOrganisation facOrg = new Facade.Organisation ();
			onHold = facOrg.GetOrganisationAccountStatusForIdentityId(Convert.ToInt32(cboClient.SelectedValue)); // OnHold

			if (onHold)
			{
				btnAdd.Enabled = false;
				lblConfirmation.Visible = true;
				lblConfirmation.Text = cboClient.Text + "'s account has been put on hold, please go to <A HREF=../Organisation/addupdateorganisation.aspx?IdentityId=" + Convert.ToInt32(cboClient.SelectedValue) + ">" + cboClient.Text + "'s details to change.</A>";			
			}
			else
			{
				btnAdd.Enabled = true;
				lblConfirmation.Visible = false;
			}
		}
		#endregion 

		#region DBCombo's Server Methods and Initialisation

        void cboClient_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            cboClient.Items.Clear();
            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllClientsFiltered("%" + e.Text,false);
            DataTable dt = ds.Tables[0];
            DataTable boundResults = dt.Clone();

            int itemsPerRequest = 20;
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
			this.Init += new System.EventHandler(this.AddUpdateOneLinerCreditNote_Init);
		}
		#endregion
	}
}
