using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using P1TP.Components.Web.UI;
using P1TP.Components.Web.Validation;
using Orchestrator.WebUI.UserControls;
using Orchestrator.Globals;
using System.Globalization;
using System.Net.Mail;
using System.Net;
using System.Configuration;

namespace Orchestrator.WebUI.Invoicing
{
    /// <summary>
    /// Summary description for InvoiceAuditTrail.
    /// </summary>
    public partial class InvoiceAuditTrail : Orchestrator.Base.BasePage, IPostBackEventHandler
    {
        #region Constants

        //private const string C_SORT_DIRECTION = "C_SORT_DIRECTION";
        //private const string C_SORT_FIELD = "C_SORT_FIELD";

        #endregion

        #region Form Elements

        private Dictionary<int, CultureInfo> cultures = new Dictionary<int, CultureInfo>();

        #endregion

        #region Property Interfaces

        //public string SortDirection
        //{
        //    get
        //    {
        //        if (ViewState[C_SORT_FIELD] == null)
        //            return "ASC";
        //        else
        //            return (string)ViewState[C_SORT_FIELD];
        //    }
        //    set
        //    {
        //        ViewState[C_SORT_DIRECTION] = value;
        //    }
        //}

        //public string SortField
        //{
        //    get
        //    {
        //        if (ViewState[C_SORT_DIRECTION] == null)
        //            return "InvoiceDate";
        //        else
        //            return (string)ViewState[C_SORT_DIRECTION];
        //    }
        //    set
        //    {
        //        ViewState[C_SORT_FIELD] = value;
        //    }
        //}

        #endregion

        #region Page Load/Init

        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (Request.QueryString["rcbID"] != null) return;

             // Check to see if the user has requested to remove an order, or uncontract a leg
            string target = Request.Params["__EVENTTARGET"];

            if (!string.IsNullOrEmpty(target))
            {
                if (target.ToLower() == "btnpost")
                {
                    this.btnPost_Click(this.btnPost, new EventArgs());
                }
            }

            if (!IsPostBack)
            {
                // add the english culture to the list.
                this.cultures.Add(2057, new CultureInfo(2057));
                hidSelectedInvoices.Value = string.Empty;
                Utilities.ClearInvoiceSession();
                PopulateStaticControls();

                PopulateInvoices();                
            }


        }

        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            base.OnInit(e);

            dgInvoices.Sorting += new GridViewSortEventHandler(dgInvoices_Sorting);
        }

        

        private void InvoiceAuditTrail_Init(object sender, EventArgs e)
        {
            btnFilter.Click += new EventHandler(btnFilter_Click);

            this.cboClient.SelectedIndexChanged += new Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventHandler(cboClient_SelectedIndexChanged);
            this.cboClient.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);
            btnExportToCSV.Click += new EventHandler(btnExportToCSV_Click);
            this.btnEmailInvoices.Click += new EventHandler(btnEmailInvoices_Click);
            dgInvoices.RowDataBound += new GridViewRowEventHandler(dgInvoices_RowDataBound);
            this.btnPost.Click += new EventHandler(btnPost_Click);
            this.btnExportToCSVWithDetail.Click += new EventHandler(btnExportToCSVWithDetail_Click);
        }

        void btnExportToCSVWithDetail_Click(object sender, EventArgs e)
        {
            var selectedInvoiceIds = GetSelectedInvoiceIds();
            if (selectedInvoiceIds.Any())
            {
                Facade.IInvoice facInvoice = new Facade.Invoice();
                var invoiceIDs = string.Empty;
                foreach(int i in selectedInvoiceIds)
                    invoiceIDs += i.ToString() + ",";
                invoiceIDs = invoiceIDs.Substring(0, invoiceIDs.Length-1);

                DataSet ds = facInvoice.GetForExportWithDetail(invoiceIDs);

                Session["__ExportDS"] = ds.Tables[0];
                Server.Transfer("../Reports/csvexport.aspx?filename=InvoiceAudit.csv");
            }
        }

        protected void btnEmailInvoices_Click(object sender, EventArgs e)
        {
            foreach (GridViewRow row in dgInvoices.Rows)
                if (row.RowType == DataControlRowType.DataRow)
                {
                    HtmlInputCheckBox chkEmail = row.FindControl("chkEmail") as HtmlInputCheckBox;

                    if (chkEmail != null && chkEmail.Checked)
                    {
                        using (MailMessage mailMessage = new System.Net.Mail.MailMessage())
                        {
                            mailMessage.From = new MailAddress(Orchestrator.Globals.Configuration.MailFromAddress, Orchestrator.Globals.Configuration.MailFromName);

                            string[] recipients = chkEmail.Attributes["EmailAddresses"].ToString().Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                            foreach (string recipient in recipients)
                                mailMessage.To.Add(new MailAddress(recipient));

                            mailMessage.Attachments.Add(new Attachment(Server.MapPath(chkEmail.Attributes["PDFLocation"])));
                            mailMessage.Subject = Orchestrator.Globals.Configuration.InstallationCompanyName + " Invoice Number " + chkEmail.Attributes["InvoiceId"].ToString();
                            mailMessage.IsBodyHtml = false;

                            // We need to get/determine what text should be attached to the email for the invoice.
                            string body = string.Format("Please find attached Invoice “{0}” from {1}. If you require POD’s please access the self service website here {2}. Please contact Customer Services on {3}  for additional help,\n\n Regards", chkEmail.Attributes["InvoiceId"].ToString(), Orchestrator.Globals.Configuration.InstallationCompanyName, Orchestrator.Globals.Configuration.OrchestratorURL, Orchestrator.Globals.Configuration.InstallationCustomerServicesNumber);
                            mailMessage.Body = body;

                            SmtpClient smtp = new System.Net.Mail.SmtpClient(); 
                            smtp.Host = Globals.Configuration.MailServer;
                            smtp.Credentials = new NetworkCredential(Globals.Configuration.MailUsername, Globals.Configuration.MailPassword);

                            smtp.Send(mailMessage);
                        }
                    }
                }
        }

        void btnPost_Click(object sender, EventArgs e)
        {
            var selectedInvoiceIds = GetSelectedInvoiceIds();
            if (selectedInvoiceIds.Any())
                PostToAccountsSystem(selectedInvoiceIds);

            // Re-enable the Post To Accounts button and uncheck all invoices (to avoid possible duplicate posting etc).

            this.btnPost.Enabled = true;
            foreach (GridViewRow row in dgInvoices.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    HtmlInputCheckBox chkEmail = row.FindControl("chkInvoice") as HtmlInputCheckBox;
                    chkEmail.Checked = false;
                }
            }
        }

        void dgInvoices_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView drv = e.Row.DataItem as DataRowView;

                e.Row.Attributes.Add("onDblClick", Page.GetPostBackEventReference(this, e.Row.DataItemIndex.ToString()));

                int LCID = 2057;
                if (drv["LCID"] != DBNull.Value && Convert.ToInt32(drv["LCID"]) != -1)
                    LCID = Convert.ToInt32(drv["LCID"]);

                if (!this.cultures.ContainsKey(LCID))
                    this.cultures.Add(LCID, new CultureInfo(LCID));

                // add formatted currency values.
                Label netAmountLabel = (Label)e.Row.FindControl("netCurrencyLabel");
                netAmountLabel.Text = Convert.ToDecimal(drv["ForeignNetAmount"]).ToString("C", this.cultures[LCID]);

                Label VATAmountLabel = (Label)e.Row.FindControl("VATCurrencyLabel");
                VATAmountLabel.Text = Convert.ToDecimal(drv["ForeignVATAmount"]).ToString("C", this.cultures[LCID]);

                Label grossAmountLabel = (Label)e.Row.FindControl("grossCurrencyLabel");
                grossAmountLabel.Text = Convert.ToDecimal(drv["ForeignGrossAmount"]).ToString("C", this.cultures[LCID]);

                Label extraAmountLabel = (Label)e.Row.FindControl("extraCurrencyLabel");
                extraAmountLabel.Text = Convert.ToDecimal(drv["ForeignExtraAmount"]).ToString("C", this.cultures[LCID]);

                Label fuelAmountLabel = (Label)e.Row.FindControl("fuelCurrencyLabel");
                fuelAmountLabel.Text = Convert.ToDecimal(drv["ForeignFuelSurchargeAmount"]).ToString("C", this.cultures[LCID]);

                HtmlAnchor lnkInvoiceNo = e.Row.FindControl("lnkInvoiceNo") as HtmlAnchor;

                HtmlInputCheckBox chkEmail = e.Row.FindControl("chkEmail") as HtmlInputCheckBox;
                chkEmail.Attributes.Add("InvoiceId", drv["InvoiceId"].ToString());

                string invoiceTarget = String.Empty;

                int invoiceTypeID = 0;
                if (int.TryParse(drv["InvoiceTypeID"].ToString(), out invoiceTypeID))
                {
                    if (drv["PDFLocation"] != DBNull.Value && (invoiceTypeID == (int)eInvoiceType.ClientInvoicing || invoiceTypeID == (int)eInvoiceType.SubContractorSelfBill || invoiceTypeID == (int)eInvoiceType.SubContract || invoiceTypeID == (int)eInvoiceType.PFDepotCharge || invoiceTypeID == (int)eInvoiceType.PFHubCharge || invoiceTypeID == (int)eInvoiceType.PFSelfBillDeliveryPayment))
                        invoiceTarget = Globals.Configuration.WebServer + (string)drv["PDFLocation"];
                    else if (invoiceTypeID == (int)eInvoiceType.OneLiner)
                    {
                        invoiceTarget = "/invoicing/AddUpdateOneLinerInvoice.aspx?invoiceId=" + drv["InvoiceID"].ToString();

                        if (drv["PDFLocation"] == DBNull.Value)
                            chkEmail.Disabled = true;
                        else
                            chkEmail.Disabled = false;
                    }
                    else if (invoiceTypeID == (int)eInvoiceType.Normal || invoiceTypeID == (int)eInvoiceType.SelfBill)
                    {
                        invoiceTarget = "/invoicing/AddUpdateInvoice.aspx?invoiceId=" + drv["InvoiceID"].ToString();
                        chkEmail.Disabled = true; 
                    }
                    else if (invoiceTypeID == (int)eInvoiceType.Extra)
                    {
                            
                        invoiceTarget = "/invoicing/addupdateextrainvoice.aspx?invoiceId=" + drv["InvoiceID"].ToString();
                        chkEmail.Disabled = true; 
                    }
                    else if (invoiceTypeID == (int)eInvoiceType.SubContractorExtra)
                    {
                        invoiceTarget = "/invoicing/subcontractorsb/addupdateextrainvoice.aspx?invoiceId=" + drv["InvoiceID"].ToString();
                        chkEmail.Disabled = true; 
                    }
                }

                if (drv["InvoiceEmailAddress"] == null || drv["InvoiceEmailAddress"].ToString() == String.Empty )
                    chkEmail.Disabled = true;

                if (chkEmail.Disabled == false)
                {
                    chkEmail.Attributes.Add("EmailAddresses", drv["InvoiceEmailAddress"].ToString());
                    chkEmail.Attributes.Add("PDFLocation", (Convert.IsDBNull(drv["PDFLocation"])) ? "" : (string)drv["PDFLocation"]);
                }

                lnkInvoiceNo.HRef = invoiceTarget;

                // Enable/disable the selection checkboxes based on invoice type.
                HtmlInputCheckBox chkInvoice = (HtmlInputCheckBox)e.Row.FindControl("chkInvoice");

                var pdfLocation = 0;
                var returnValue = drv["PDFLocation"];

                if (returnValue != System.DBNull.Value)
                    pdfLocation = 1;

                chkInvoice.Attributes.Add("onclick", string.Format("updateInvoices(this,{0},{1},{2});", drv["InvoiceId"], Convert.ToDecimal(drv["ForeignNetAmount"]), pdfLocation));

                if (invoiceTypeID == (int)eInvoiceType.Normal ||
                    invoiceTypeID == (int)eInvoiceType.SelfBill ||
                    invoiceTypeID == (int)eInvoiceType.SubContract ||
                    invoiceTypeID == (int)eInvoiceType.ClientInvoicing ||
                    invoiceTypeID == (int)eInvoiceType.SubContractorSelfBill ||
                    invoiceTypeID == (int)eInvoiceType.OneLiner ||
                    invoiceTypeID == (int)eInvoiceType.Extra ||
                    invoiceTypeID == (int)eInvoiceType.PFDepotCharge ||
                    invoiceTypeID == (int)eInvoiceType.PFHubCharge ||
                    invoiceTypeID == (int)eInvoiceType.PFSelfBillDeliveryPayment)
                {
                    chkInvoice.Disabled = false;
                }
                else
                {
                    chkInvoice.Disabled = true;
                }
            }
        }

        public void RaisePostBackEvent(string eventArguments)
        {
            GridViewSelectEventArgs e = null;
            int selectedRowIndex = -1;

            if (!string.IsNullOrEmpty(eventArguments))
            {
                string[] args = eventArguments.Split('$');
                if (string.Compare(args[0], "DOUBLECLICK", true, System.Globalization.CultureInfo.InvariantCulture) == 0 && args.Length > 1)
                {
                    Int32.TryParse(args[1], out selectedRowIndex);
                    e = new GridViewSelectEventArgs(selectedRowIndex);
                    OnDblClick(e);
                }
            }
        }

        protected virtual void OnDblClick(EventArgs e)
        {

            DisplayInvoiceAuditReport(((GridViewSelectEventArgs)e).NewSelectedIndex);
        }

        void btnExportToCSV_Click(object sender, EventArgs e)
        {
            DataView dvData = new DataView(GetData().Tables[0]);
            
            var selectedInvoiceIds = GetSelectedInvoiceIds();
            if (selectedInvoiceIds.Any())
                dvData.RowFilter = string.Format("InvoiceId IN ({0})", string.Join(",", selectedInvoiceIds.Select(i => i.ToString()).ToArray()));

            Session["__ExportDS"] = dvData.ToTable();
            Server.Transfer("../Reports/csvexport.aspx?filename=InvoiceAudit.csv");
        }

        void cboClient_SelectedIndexChanged(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if (cboClient.SelectedItem != null)
            {
                PopulateInvoices();
            }
            else
            {
                dgInvoices.Visible = false;
                reportViewer.Visible = false;
            }
        }

        #endregion

        private void DisplayInvoiceAuditReport(int invoiceId)
        {
            // Cause the report to be displayed containing the job information for the specified invoice.
            Facade.IInvoice facInvoice = new Facade.Invoice();
            DataSet dsInvoiceAudit = facInvoice.GetInvoiceContents(invoiceId);

            // Configure the report settings collection
            NameValueCollection reportParams = new NameValueCollection();

            // Configure the Session variables used to pass data to the report
            Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.InvoiceContents;
            Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = dsInvoiceAudit;
            Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
            Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";

            // Setting the identity id of the report allows us to configure the fax and email boxes suitably.
            if (cboClient.SelectedValue != "")
                reportViewer.IdentityId = Convert.ToInt32(cboClient.SelectedValue);
            reportViewer.Visible = true;
        }

        private void PopulateStaticControls()
        {
            rblDateType.SelectedIndex = 0;

            cboInvoiceType.DataSource = Utilities.UnCamelCase(Enum.GetNames(typeof(eInvoiceType)));
            cboInvoiceType.DataBind();
            cboInvoiceType.Items.Insert(0, new ListItem("All Types", "0"));

            dteInvoiceStartDate.SelectedDate = DateTime.Now.Subtract(new TimeSpan(7, 0, 0, 0));
            dteInvoiceEndDate.SelectedDate = DateTime.Now;
        }

        private DataSet GetData()
        {
            #region Configure Parameters

            int clientId = 0;
            try
            {
                clientId = Convert.ToInt32(cboClient.SelectedValue);
            }
            catch { }

            int invoiceTypeId = 0;
            try
            {
                eInvoiceType invoiceType = (eInvoiceType)Enum.Parse(typeof(eInvoiceType), cboInvoiceType.SelectedValue.Replace(" ", ""));
                invoiceTypeId = (int)invoiceType;
            }
            catch { }

            DateTime startDate = dteInvoiceStartDate.SelectedDate.Value;
            startDate = startDate.Subtract(startDate.TimeOfDay);

            DateTime endDate = dteInvoiceEndDate.SelectedDate.Value;
            endDate = endDate.Subtract(endDate.TimeOfDay);
            endDate = endDate.Add(new TimeSpan(23, 59, 59));

            int dateType = int.Parse(rblDateType.SelectedValue);

            int? invoiceID = null;
            try
            {
                invoiceID = Convert.ToInt32(txtInvoiceId.Text);
            }
            catch { }

            bool showCancelled = chkShowCancelled.Checked;
            bool showPosted = chkShowPosted.Checked;

            #endregion

            Facade.IInvoice facInvoice = new Facade.Invoice();
            return facInvoice.GetInvoicesForAuditTrail(clientId, invoiceTypeId, dateType, startDate, endDate, showCancelled, showPosted, invoiceID);
        }

        private void PopulateInvoices(string sortString = null)
        {
            hidSelectedInvoices.Value = string.Empty;
            DataSet dsInvoices = GetData();
            
            if (!string.IsNullOrEmpty(sortString))
                dsInvoices.Tables[0].DefaultView.Sort = sortString;

            dgInvoices.DataSource = dsInvoices.Tables[0].DefaultView;
            dgInvoices.DataBind();
            int rowCount = dsInvoices.Tables[0].Rows.Count;
            if (rowCount > 0)
                dgInvoices.PageSize = rowCount;
            dgInvoices.Visible = true;

            reportViewer.Visible = false;
        }

        #region Web Form Designer generated code        

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Init += new EventHandler(InvoiceAuditTrail_Init);
        }
        #endregion

        void cboClient_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {

            cboClient.Items.Clear();

            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllInvoicedClientsFiltered(e.Text);

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
                rcItem.Text = dt.Rows[i]["OrganisationName"].ToString();
                rcItem.Value = dt.Rows[i]["IdentityId"].ToString();
                cboClient.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }
        }

        #region Event Handlers

        private void btnFilter_Click(object sender, EventArgs e)
        {
            PopulateInvoices();
        }

        void dgInvoices_SelectCommand(object sender, ComponentArt.Web.UI.GridItemEventArgs e)
        {
            if (e.Item[0].ToString() != "")
            {
                eInvoiceType invoiceType = (eInvoiceType)Enum.Parse(typeof(eInvoiceType), e.Item[3].ToString().Replace(" ", ""));

                reportViewer.Visible = false;

                if (invoiceType == eInvoiceType.Normal || invoiceType == eInvoiceType.SelfBill)
                {
                    // Display the selected invoice id in the report.
                    int invoiceId = Convert.ToInt32(e.Item[0]);
                    DisplayInvoiceAuditReport(invoiceId);
                }

            }
        }

        void dgInvoices_Sorting(object sender, GridViewSortEventArgs e)
        {
            string sort = e.SortExpression + " " + GetSortDirection(e.SortExpression);
            PopulateInvoices(sort);
        }

        private string GetSortDirection(string column)
        {
            string sortDirection = "ASC";
            string sortExpression = ViewState["SortExpression"] as string;

            if (sortExpression != null)
            {
                if (sortExpression == column)
                {
                    string lastDirection = ViewState["SortDirection"] as string;
                    if ((lastDirection != null) && (lastDirection == "ASC"))
                    {
                        sortDirection = "DESC";
                    }
                }
            }

            ViewState["SortDirection"] = sortDirection;
            ViewState["SortExpression"] = column;

            return sortDirection;
        }

        #endregion

        #region Posting to Accounts System
        private void PostToAccountsSystem(IEnumerable<int> selectedInvoiceIds)
        {
            Entities.Invoice _Invoice = null;
            Facade.IInvoice facInvoice = new Facade.Invoice();
            bool retVal = false;

            string invoicesPosted = string.Empty;
            string invoicesFailed = string.Empty;
            string invoicesAlreadyPosted = string.Empty;

            //For each selected invoice
            foreach (int invoiceId in selectedInvoiceIds)
            {

                _Invoice = PopulateInvoice(invoiceId);
                try
                {
                    // Check to make sure none of the selected invoices have been posted. 
                    // If any have been posted, prevent the posting from going ahead again.

                    // Check whether any of the selected invoices are "self bill". 
                    // If they are then do not allow them to be posted to accounts.
                    // At the time of writing this code TF and JS agreed that none of our existing clients
                    // actually posted self bill invoices anyway (although some had been posted in the past by mistake!)
                    // so the ability to post self bill invoices has not been made configurable. TF:10/09/2008
                    if (_Invoice.Posted || _Invoice.InvoiceType == eInvoiceType.SelfBill || _Invoice.ForCancellation == true )
                    {
                        retVal = false;
                        invoicesAlreadyPosted += invoiceId.ToString() + ", ";
                    }
                    else
                    {
                        _Invoice.Posted = true;
                        retVal = UpdateInvoice(_Invoice);
                    }
                }
                catch (Exception e)
                {
                    string err = GetErrorString(e);
                    lblError.Text = "An error occured when posting Invoice Id " + invoiceId.ToString() + ".<br/>" + err;
                    pnlError.Visible = true;
                    return;
                }

                if (retVal)
                {
                    invoicesPosted += invoiceId.ToString() + ", ";
                    btnPost.Enabled = false;
                }
                else
                {
                    invoicesFailed += invoiceId.ToString() + ", ";
                    lblError.Text = "The following Invoices failed to post: " + invoicesFailed + ".<br/>";
                    pnlError.Visible = true;
                }

                if (invoicesAlreadyPosted != string.Empty)
                {
                    lblError.Text = "The following Invoices were not posted because they are Self Bill, are Cancelled or have already been posted: " + invoicesAlreadyPosted + ".<br/>";
                    pnlError.Visible = true;
                }
            }

            PopulateInvoices();

        }


        private string GetErrorString(Exception ex)
        {

            string errorString = ex.Message;
            if (ex.InnerException != null)
                errorString += "<br/>" + GetErrorString(ex.InnerException);

            return errorString;
        }

        private Entities.Invoice PopulateInvoice(int invoiceId)
        {
            Entities.Invoice m_Invoice = null;

            Facade.IInvoice facInvoice = new Orchestrator.Facade.Invoice();
            m_Invoice = facInvoice.GetForInvoiceId(invoiceId);

            return m_Invoice;
        }

        private bool UpdateInvoice(Entities.Invoice invoice)
        {
            int clientID = 0;
            Facade.IInvoice facInvoice = new Facade.Invoice();
            Facade.IInvoiceOneLiner facInvoiceOneLiner = new Facade.Invoice();
            bool retVal = false;
            DataSet dsClientID;

            string userName = ((Entities.CustomPrincipal)Page.User).UserName;

            //GetClientForOneLinerInvoiceId should work for most invoice types, where as GetClientInvoiceId only works for groupage invoices
            if (invoice.InvoiceType == eInvoiceType.Normal)
                dsClientID = facInvoice.GetClientForInvoiceId(invoice.InvoiceId);
            else
                dsClientID = facInvoiceOneLiner.GetClientForOneLinerInvoiceId(invoice.InvoiceId);


            int.TryParse(dsClientID.Tables[0].Rows[0]["ClientId"].ToString(), out clientID);

            retVal = facInvoice.Update(invoice, clientID, userName);

            return retVal;
        }
        #endregion

        private IEnumerable<int> GetSelectedInvoiceIds()
        {
            return hidSelectedInvoices.Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries).Select(s => int.Parse(s));
        }

        protected void cfvInvoiceID_ServerValidate(object obj, ServerValidateEventArgs args)
        {
            args.IsValid = P1TP.Components.Common.SimpleValidation.ValidateNumericValue(args.Value, 0, true);
        }

    }
}
