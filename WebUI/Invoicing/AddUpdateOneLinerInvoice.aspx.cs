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
using P1TP.Components.Web.Validation;
using System.Globalization;
using System.IO;
using DataDynamics.ActiveReports.Export.Pdf;
using System.Configuration;
using System.Text;
using System.Net.Mail;
using System.Net;
using System.Collections.Generic;

namespace Orchestrator.WebUI.Invoicing
{
    /// <summary>
    /// Summary description for AddUpdateOneLinerInvoice.
    /// </summary>
    public partial class AddUpdateOneLinerInvoice : Orchestrator.Base.BasePage
    {
        #region Page Variables

        private bool m_isUpdate = false;
        private int m_InvoiceNo = 0;
        private Entities.Invoice m_Invoice;

        #endregion

        #region Form Elements


        #endregion

        #region Page Load/Init
        protected void Page_Load(object sender, System.EventArgs e)
        {
            Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.Invoicing);

            // Update Invoice
            m_InvoiceNo = Convert.ToInt32(Request.QueryString["InvoiceId"]);

            if (m_InvoiceNo > 0 || lblInvoiceNo.Text != "To Be Issued ... (This invoice has not yet been saved, add invoice to allocate Invoice No.)")
                m_isUpdate = true;

            if (!IsPostBack)
            {
                PopulateStaticControls();

                this.Title = "Add/Update One Liner Invoice";
                LoadVatTypes();

                Utilities.ClearInvoiceSession();

                if (m_isUpdate)
                    LoadInvoice();
            }
        }

        private void LoadVatTypes()
        {
            Orchestrator.Facade.Invoice facInvoice = new Orchestrator.Facade.Invoice();
            DataSet dsTaxRates = facInvoice.GetTaxRates(this.rdiInvoiceDate.SelectedDate.Value);

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

        protected void AddUpdateOneLinerInvoice_Init(object sender, EventArgs e)
        {
            this.btnAdd.Click += new EventHandler(btnAdd_Click);
            this.btnViewInvoice.Click += new EventHandler(btnViewInvoice_Click);
            this.btnSendToAccounts.Click += new EventHandler(btnSendToAccounts_Click);
            this.cboClient.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);
            this.cboClient.SelectedIndexChanged += new Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventHandler(cboClient_SelectedIndexChanged);
            this.btnEmailOneLiner.Click += new EventHandler(btnEmailOneLiner_Click);
        }

        protected void btnEmailOneLiner_Click(object sender, EventArgs e)
        {
            if (ViewState["invoice"] != null)
            {
                m_Invoice = (Entities.Invoice)ViewState["invoice"];

                using (MailMessage mailMessage = new System.Net.Mail.MailMessage())
                {
                    mailMessage.From = new MailAddress(Orchestrator.Globals.Configuration.MailFromAddress, Orchestrator.Globals.Configuration.MailFromName);

                    string emailAddress = this.cboEmail.Text;

                    if (this.cboEmail.SelectedValue != String.Empty)
                        emailAddress = this.cboEmail.SelectedValue;

                    mailMessage.To.Add(new MailAddress(emailAddress));

                    mailMessage.Attachments.Add(new Attachment(Server.MapPath(m_Invoice.PDFLocation)));
                    mailMessage.Subject = Orchestrator.Globals.Configuration.InstallationCompanyName + " Invoice Number " + m_Invoice.InvoiceId.ToString();
                    mailMessage.IsBodyHtml = false;

                    // We need to get/determine what text should be attached to the email for the invoice.
                    string body = string.Format("Please find attached Invoice “{0}” from {1}. If you require POD’s please access the self service website here {2}. Please contact Customer Services on {3}  for additional help,\n\n Regards", m_Invoice.InvoiceId.ToString(), Orchestrator.Globals.Configuration.InstallationCompanyName, Orchestrator.Globals.Configuration.OrchestratorURL, Orchestrator.Globals.Configuration.InstallationCustomerServicesNumber);
                    mailMessage.Body = body;

                    SmtpClient smtp = new System.Net.Mail.SmtpClient();
                    smtp.Host = Globals.Configuration.MailServer;
                    smtp.Credentials = new NetworkCredential(Globals.Configuration.MailUsername, Globals.Configuration.MailPassword);

                    smtp.Send(mailMessage);
                    lblEmailSent.Visible = true;
                }
            }
        }

        void cboClient_SelectedIndexChanged(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            //// Get the Account Codes(s) for the Customer
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
            rdiInvoiceDate.SelectedDate = DateTime.Today;

            Facade.INominalCode facNominalCode = new Facade.NominalCode();
            DataSet dsNominalCode = facNominalCode.GetAllActive();

            cboNominalCode.DataSource = dsNominalCode;
            cboNominalCode.DataTextField = "Description";
            cboNominalCode.DataValueField = "NominalCode";
            cboNominalCode.DataBind();
            cboNominalCode.Items.Insert(0, new Telerik.Web.UI.RadComboBoxItem());

            lblInvoiceType.Text = eInvoiceType.OneLiner.ToString();
            //-----------------------------------------------------------------------------
            //									Update Section
            //-----------------------------------------------------------------------------
            if (m_isUpdate)
            {
                lblInvoiceNo.Visible = true;
                lblInvoiceNo.Text = m_InvoiceNo.ToString();
                btnAdd.Text = "Update Invoice";
            }

            chkUseHeadedPaper.Checked = Orchestrator.Globals.Configuration.UseHeadedPaper;
        }
        #endregion

        #region Add/Update/Load/Populate Invoice
        ///	<summary> 
        /// Load Invoice
        ///	</summary>
        private void LoadInvoice()
        {
            if (ViewState["invoice"] == null)
            {
                Facade.IInvoice facInvoice = new Facade.Invoice();
                m_Invoice = facInvoice.GetForInvoiceId(m_InvoiceNo);

                ViewState["invoice"] = m_Invoice;
            }
            else
                m_Invoice = (Entities.Invoice)ViewState["invoice"];

            // Load the report with the relevant details
            if (m_Invoice != null)
            {
                lblInvoiceNo.Text = m_Invoice.InvoiceId.ToString();
                lblInvoiceNo.ForeColor = Color.Black;

                lblInvoiceType.Text = m_Invoice.InvoiceType.ToString();

                lblDateCreated.Text = m_Invoice.CreatedDate.ToShortDateString();
                lblDateCreated.ForeColor = Color.Black;

                rdiInvoiceDate.SelectedDate = m_Invoice.InvoiceDate;

                rtbTxtReason.Text = m_Invoice.InvoiceDetails;

                txtNETAmount.Text = m_Invoice.ClientInvoiceAmount.ToString("N2");

                rdiInvoiceDate.Enabled = !m_Invoice.Posted;

                lblAccountCode.Text = m_Invoice.AccountCode;

                if (cboNominalCode.FindItemByValue(m_Invoice.NominalCode) != null)
                    cboNominalCode.FindItemByValue(m_Invoice.NominalCode).Selected = true;
                cboNominalCode.Enabled = !m_Invoice.Posted;

                //cboVATType.SelectedValue = Convert.ToInt32(m_Invoice.VatRate).ToString();

                // Load Client
                Facade.IInvoiceOneLiner facInv = new Facade.Invoice();
                DataSet ds = facInv.GetClientForOneLinerInvoiceId(m_Invoice.InvoiceId);

                cboClient.SelectedValue = ds.Tables[0].Rows[0]["IdentityId"].ToString();
                cboClient.Text = ds.Tables[0].Rows[0]["OrganisationName"].ToString();
                cboClient.Enabled = false;

                Facade.IOrganisation facOrganisation = new Facade.Organisation();
                Entities.Organisation client = facOrganisation.GetForIdentityId(int.Parse(cboClient.SelectedValue));

                int LCID = Convert.ToInt32(ds.Tables[0].Rows[0]["LCID"]);
                CultureInfo culture = new CultureInfo(LCID);
                this.txtNETAmount.Culture = culture;

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

                if (m_isUpdate)
                {
                    if (m_Invoice.ForCancellation)
                    {
                        btnAdd.Visible = false;
                        btnSendToAccounts.Visible = false;
                        chkPostToExchequer.Visible = false;
                        chkDelete.Checked = true;
                    }
                    else
                    {
                        btnAdd.Visible = true;
                        btnSendToAccounts.Visible = true;
                        chkPostToExchequer.Visible = true;
                        chkDelete.Checked = false;
                    }

                    this.Title = "Update Invoice";
                    //Header1.subTitle = "Please make any changes neccessary.";
                    btnAdd.Text = "Update";
                }
                else
                {
                    chkPostToExchequer.Visible = true;
                }

                chkPostToExchequer.Visible = true;

                LoadReport();

                if (m_Invoice.Posted)
                {
                    btnAdd.Visible = false;
                    btnSendToAccounts.Visible = false;
                    chkPostToExchequer.Checked = true;
                    pnlInvoiceDeleted.Visible = false;
                    chkDelete.Visible = false;
                    chkPostToExchequer.Checked = true;
                    chkPostToExchequer.Visible = true;
                    txtNETAmount.Enabled = false;
                    rtbTxtReason.Enabled = false;
                    btnSendToAccounts.Visible = false;
                    btnViewInvoice.Visible = false;
                    rdiInvoiceDate.Enabled = false;
                    txtNETAmount.Enabled = false;
                    cboVATType.Enabled = false;
                }
                else
                {
                    btnAdd.Visible = true;
                    btnSendToAccounts.Visible = true;
                    chkPostToExchequer.Checked = false;
                    pnlInvoiceDeleted.Visible = true;
                    chkDelete.Visible = true;
                }
            }
            else if (cboClient.SelectedValue != null)
            {
                Facade.IOrganisation facOrganisation = new Facade.Organisation();
                Entities.Organisation client = facOrganisation.GetForIdentityId(int.Parse(cboClient.SelectedValue));

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
            }

            this.Title = "Update Invoice";
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
        /// Populate Invoice
        ///	</summary>
        private void populateInvoice()
        {
            if (ViewState["invoice"] == null)
                m_Invoice = new Entities.Invoice();
            else
                m_Invoice = (Entities.Invoice)ViewState["invoice"];

            if (lblInvoiceNo.Text != "To Be Issued ... (This invoice has not yet been saved, add invoice to allocate Invoice No.)")
                m_Invoice.InvoiceId = Convert.ToInt32(lblInvoiceNo.Text);

            m_Invoice.InvoiceType = eInvoiceType.OneLiner;

            m_Invoice.InvoiceDetails = rtbTxtReason.Text;

            m_Invoice.InvoiceDate = rdiInvoiceDate.SelectedDate.Value;

            Facade.IExchangeRates facER = new Facade.ExchangeRates();

            int exchangeRateId = -1;
            // the txtNetAmount textbox has had its culture set by the client.
            if (this.txtNETAmount.Culture.LCID != 2057)
                exchangeRateId = facER.GetCurrentExchangeRateID(Facade.Culture.GetCurrencySymbol(this.txtNETAmount.Culture.LCID), this.rdiInvoiceDate.SelectedDate.Value);

            m_Invoice.ClientInvoiceAmount = Decimal.Parse(txtNETAmount.Text, System.Globalization.NumberStyles.Currency);

            if (chkPostToExchequer.Checked)
                m_Invoice.Posted = true;
            else
                m_Invoice.Posted = false;

            int vatNo = 0;
            decimal vatRate = 0.00M;
            //Get VAT Rate and Vat Type
            Facade.IInvoice facInv = new Facade.Invoice();
            eVATType vatType = (eVATType)int.Parse(cboVATType.SelectedValue);
            facInv.GetVatRateForVatType(vatType, m_Invoice.InvoiceDate, out vatNo, out vatRate);

            m_Invoice.VatRate = vatRate;
            m_Invoice.VatNo = vatNo;

            m_Invoice.AccountCode = lblAccountCode.Text;
            m_Invoice.NominalCode = cboNominalCode.SelectedValue;

            // Deleted checked not required until the Invoice's are allowed to be deleted
            if (chkDelete.Checked)
                m_Invoice.ForCancellation = true;
            else
                m_Invoice.ForCancellation = false;
        }


        ///	<summary> 
        /// Update Invoice
        ///	</summary>
        private bool UpdateInvoice()
        {
            Facade.IInvoice facInvoice = new Facade.Invoice();
            Facade.IInvoiceOneLiner facInvoiceOneLiner = new Facade.Invoice();

            bool retVal = false;
            string userName = ((Entities.CustomPrincipal)Page.User).UserName;

            using (TransactionScope ts = new TransactionScope())
            {
                DataSet dsClientID;
                int clientID;

                //GetClientForOneLinerInvoiceId should work for most invoice types, where as GetClientInvoiceId only works for groupage invoices
                if (m_Invoice.InvoiceType == eInvoiceType.Normal)
                    dsClientID = facInvoice.GetClientForInvoiceId(m_Invoice.InvoiceId);
                else
                    dsClientID = facInvoiceOneLiner.GetClientForOneLinerInvoiceId(m_Invoice.InvoiceId);


                int.TryParse(dsClientID.Tables[0].Rows[0]["ClientId"].ToString(), out clientID);

                retVal = facInvoiceOneLiner.UpdateOneLiner(m_Invoice, this.txtNETAmount.Culture.LCID, clientID, userName);

                ts.Complete();

            }

            return retVal;
        }


        ///	<summary> 
        /// Add Invoice
        ///	</summary>
        private int AddInvoice()
        {
            int invoiceId = 0;
            Facade.IInvoiceOneLiner facInvoice = new Facade.Invoice();

            string userName = ((Entities.CustomPrincipal)Page.User).UserName;

            invoiceId = facInvoice.CreateOneLiner(m_Invoice, this.txtNETAmount.Culture.LCID, Convert.ToInt32(cboClient.SelectedValue), userName);

            if (invoiceId == 0)
            {
                lblConfirmation.Text = "There was an error adding the Invoice, please try again.";
                lblConfirmation.Visible = true;
                lblConfirmation.ForeColor = Color.Red;
            }
            else
            {
                // NO NEED: Alreayd done within the Data Access Layer.

                // Reference Invoice Id With Client
                //int clientId = Convert.ToInt32(cboClient.SelectedValue);
                //Facade.IInvoiceOneLiner facInv = new Facade.Invoice();
                //facInv.CreateInvoiceIdWithClientId(invoiceId, clientId);
                // Done in the Data Access Layer.
            }

            return invoiceId;
        }

        #endregion

        #region Methods & Functions
        /// <summary>
        /// Load Report Depending On Invoice Type
        /// </summary>
        private void LoadReport()
        {
            // Configure the Session variables used to pass data to the report
            NameValueCollection reportParams = new NameValueCollection();

            //-------------------------------------------------------------------------------------	
            //						Job/Collect-Drops/References/Demurrages Section
            //-------------------------------------------------------------------------------------	
            Facade.IInvoice facInv = new Facade.Invoice();

            DataSet dsInv = null;
            if (m_isUpdate)
            {
                dsInv = facInv.GetJobsForInvoiceId(Convert.ToInt32(lblInvoiceNo.Text));
                btnSendToAccounts.Visible = true;
                pnlInvoiceDeleted.Visible = true;
            }

            // Client Name & Id			
            reportParams.Add("Client", cboClient.Text);

            reportParams.Add("ClientId", cboClient.SelectedValue);

            // Details
            reportParams.Add("Reason", rtbTxtReason.Text);

            // Invoice Id
            if (lblInvoiceNo.Text != "To Be Issued ... (This invoice has not yet been saved, add invoice to allocate Invoice No.)")
                reportParams.Add("invoiceId", lblInvoiceNo.Text);
            else
                reportParams.Add("invoiceId", "0");

            reportParams.Add("NETAmount", decimal.Parse(txtNETAmount.Text, System.Globalization.NumberStyles.Currency).ToString());

            // Posted To Accounts
            if (chkPostToExchequer.Checked)
                reportParams.Add("Accounts", "true");

            int vatNo = 0;
            decimal vatRate = 0.00M;

            // Get VAT Rate for VAT Type
            eVATType vatType = (eVATType)int.Parse(cboVATType.SelectedValue);
            facInv.GetVatRateForVatType(vatType, this.rdiInvoiceDate.SelectedDate.Value, out vatNo, out vatRate);

            reportParams.Add("VATrate", vatRate.ToString());
            reportParams.Add("UseHeadedPaper", chkUseHeadedPaper.Checked.ToString());

            // Invoice Date 
            reportParams.Add("InvoiceDate", this.rdiInvoiceDate.SelectedDate.Value.ToShortDateString());

            //-------------------------------------------------------------------------------------	
            //									Load Report Section 
            //-------------------------------------------------------------------------------------	
            Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.InvoiceOneLiner;
            //			Session[Orchestrator.Globals.Constants.ReportDataSessionVariable] = dsInv;
            Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
            //			Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";
            Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;

            bool enforceDataExistence;
            var activeReport = Orchestrator.Reports.Utilities.GetActiveReport(eReportType.InvoiceOneLiner, "", out enforceDataExistence);
            activeReport.Run(false);

            if (m_Invoice == null)
                populateInvoice();

            // Generate the location this report's pdf will be output to.
            StringBuilder sbFilename = new StringBuilder(ConfigurationManager.AppSettings["GeneratedPDFRoot"]);
            sbFilename.Append("\\Invoices\\");
            sbFilename.Append(m_Invoice.InvoiceDate.Year.ToString());
            sbFilename.Append("\\");
            sbFilename.Append(m_Invoice.InvoiceDate.Month.ToString());
            sbFilename.Append("\\");
            sbFilename.Append(m_Invoice.InvoiceDate.Day.ToString());
            sbFilename.Append("\\One_Liner_Invoice_");
            sbFilename.Append(m_Invoice.InvoiceId);

            sbFilename.Append(".pdf");
            string filename = sbFilename.ToString();
            string oldPDFLocation = m_Invoice.PDFLocation;

            PdfExport pdf = new PdfExport();

            //If the invoice id is 0 then the invoice hasn't been created so don't save it
            if (m_Invoice.InvoiceId != 0)
            {
                Directory.CreateDirectory(Path.GetDirectoryName(filename));

                //Export the report to PDF.
                pdf.Export(activeReport.Document, filename);

                string userName = ((Entities.CustomPrincipal)Page.User).UserName;
                string pdfRoot = ConfigurationManager.AppSettings["GeneratedPDFRoot"];

                filename = filename.Remove(0, (pdfRoot != null ? pdfRoot.Length : 0));

                m_Invoice.PDFLocation = filename;

                facInv.UpdatePDFLocation(m_Invoice.InvoiceId, filename , userName);

                pdfViewer.Src = Globals.Configuration.WebServer + filename;

                lblEmail.Visible = true;
                btnEmailOneLiner.Visible = true;
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
                btnEmailOneLiner.Visible = false;
                cboEmail.Visible = false;
                rfvEmailAddress.Enabled = false;
                revEmailAddress.Enabled = false;
            }

            pdfViewer.Visible = true;

            // Show the add invoice button. 
            if (!chkPostToExchequer.Checked)
                btnAdd.Visible = true;

            // Deleted
            if (chkDelete.Checked)
                Response.Redirect("../default.aspx");
        }



        protected void btnAdd_Click(object sender, EventArgs e)
        {
            bool retVal = false;
            int invoiceId = 0;

            if (m_Invoice == null)
                populateInvoice();

            if (m_isUpdate)
                retVal = UpdateInvoice();
            else
                invoiceId = AddInvoice();

            if (m_isUpdate)
            {
                lblConfirmation.Text = "The Invoice has been updated successfully.";
                LoadReport();
            }
            else
            {
                lblConfirmation.Text = "The Invoice has been added successfully.";
                lblInvoiceNo.Text = invoiceId.ToString();
                lblInvoiceNo.ForeColor = Color.Black;
                lblDateCreated.Text = DateTime.Now.ToShortDateString();
                lblDateCreated.ForeColor = Color.Black;
                btnAdd.Text = "Update Invoice";
                m_InvoiceNo = invoiceId;
                LoadInvoice();
                m_isUpdate = true;
                LoadReport();
            }

            lblConfirmation.Visible = true;
            cboClient.Enabled = false;
            chkPostToExchequer.Visible = true;
        }

        protected void btnViewInvoice_Click(object sender, EventArgs e)
        {
            //this.Validate();

            //if (Page.IsValid)
            //{
            btnViewInvoice.Visible = true;
            LoadReport();
            //}
        }

        protected void btnSendToAccounts_Click(object sender, EventArgs e)
        {
            bool retVal = false;
            chkPostToExchequer.Checked = true;

            if (m_Invoice == null)
                populateInvoice();


            string applicationExceptionErrorMessage = string.Empty;

            LoadReport();

            try
            {
                m_Invoice.Posted = true;
                retVal = UpdateInvoice();
            }
            catch (ApplicationException aex)
            {
                if (aex.InnerException.Message.ToLower().Contains("client account") && aex.InnerException.Message.ToLower().Contains("does not exist"))
                    applicationExceptionErrorMessage = aex.InnerException.Message;
                else
                    throw;
            }

            if (retVal)
            {
                lblConfirmation.Text = "The Invoice has been posted to accounts.";
                chkPostToExchequer.Visible = true;
                chkPostToExchequer.Checked = true;

                // Disable all buttons now that the invoice has been posted
                btnAdd.Enabled = false;
                btnSendToAccounts.Enabled = false;
            }
            else
            {
                if (applicationExceptionErrorMessage != string.Empty)
                    lblConfirmation.Text = applicationExceptionErrorMessage;
                else
                    lblConfirmation.Text = "The Invoice has not been posted to accounts.";

                chkPostToExchequer.Checked = false;
            }

            LoadReport();

            lblConfirmation.Visible = true;
            cboClient.Enabled = false;
        }

        private void cboClient_SelectedItemChanged(object sender, EventArgs e)
        {
            bool onHold = false;

            Facade.IOrganisation facOrg = new Facade.Organisation();
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
            DataSet ds = facRefData.GetAllClientsFiltered("%" + e.Text, false);
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
            this.Init += new System.EventHandler(this.AddUpdateOneLinerInvoice_Init);
        }
        #endregion
    }
}
