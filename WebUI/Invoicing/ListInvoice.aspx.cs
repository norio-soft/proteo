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
using Orchestrator.WebUI.Security;

namespace Orchestrator.WebUI.Invoicing
{
	/// <summary>
	/// Summary description for List Invoice.
	/// </summary>
	public partial class ListInvoice: Orchestrator.Base.BasePage
	{	
		#region Page Variables
		private const string C_InvoiceDATA_VS = "InvoiceData";
		private DataSet dsInvoice; 
		private string sortDirection = "asc";
		#endregion

        #region Form Elements
        #endregion

        #region Page Load/Init
        protected void Page_Load(object sender, System.EventArgs e)
        {
            Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.Invoicing);
            lblNote.Text = "";

            if (!IsPostBack)
            {
                if (Request.QueryString["rcbID"] != null)
                    pnlFilter.Visible = true;

                Utilities.ClearInvoiceSession();

                PopulateStaticControls();
            }
            else
            {
                dsInvoice = (DataSet)ViewState[C_InvoiceDATA_VS];
            }

            string target = Request.Params["__EVENTTARGET"];

            if (!string.IsNullOrEmpty(target) && target.ToLower().Contains("txtinvoiceid"))
            {
                // Fire the click event
                this.btnSearch_Click(this.btnSearch, new EventArgs());
            }
        }

        protected void ListInvoice_Init(object sender, EventArgs e)
        {
            this.rdoPosted.SelectedIndexChanged += new EventHandler(rdoPosted_SelectedIndexChanged);
            this.rdoFilterOptions.SelectedIndexChanged += new System.EventHandler(this.rdoFilterOptions_SelectedIndexChanged);

            this.cboJobState.SelectedIndexChanged += new EventHandler(cboJobState_SelectedIndexChanged);
            this.cboClient.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);
            this.cboSubContractor.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboSubContractor_ItemsRequested);
            this.cboClient.SelectedIndexChanged += new Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventHandler(cboClient_SelectedIndexChanged);
            this.cboSubContractor.SelectedIndexChanged += new Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventHandler(cboSubContractor_SelectedIndexChanged);

            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);


            gvInvoices.RowDataBound += new GridViewRowEventHandler(gvInvoices_RowDataBound);
            gvSubContratorInvoice.RowDataBound += new GridViewRowEventHandler(gvSubContratorInvoice_RowDataBound);

            cfvInvoiceId.ServerValidate += new ServerValidateEventHandler(cfvInvoiceId_ServerValidate);
        }

        void cfvInvoiceId_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = Utilities.ValidateNumericValue(args.Value);
        }

        void gvSubContratorInvoice_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                foreach (TableCell cell in e.Row.Cells)
                    cell.CssClass = "DataCell";

                e.Row.Attributes.Add("onclick", "HighlightRow('" + e.Row.ClientID + "');");
            }
        }

        void gvInvoices_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                foreach (TableCell cell in e.Row.Cells)
                    cell.CssClass = "DataCell";

                e.Row.Attributes.Add("onclick", "HighlightRow('" + e.Row.ClientID + "');");
            }
        }




        void dgSelfBillInvoice_NeedDataSource(object sender, EventArgs e)
        {
            switch ((eInvoiceFilterType)Enum.Parse(typeof(eInvoiceFilterType), rdoFilterOptions.SelectedValue.Replace(" ", "")))
            {
                case eInvoiceFilterType.NormalInvoice:
                    performSearch();
                    break;
                case eInvoiceFilterType.OneLinerInvoice:
                    performSearch();
                    break;
                case eInvoiceFilterType.InvoiceId:
                    LocateInvoice();
                    break;
                case eInvoiceFilterType.SelfBillInvoice:
                    performSelfBillSearch();
                    break;
                case eInvoiceFilterType.Extra:
                    performSelfBillSearch();
                    break;
                case eInvoiceFilterType.SubContractorInvoice:
                    performSubContractorSearch();
                    break;
            }
        }

        void dgInvoice_NeedDataSource(object sender, EventArgs e)
        {
            switch ((eInvoiceFilterType)Enum.Parse(typeof(eInvoiceFilterType), rdoFilterOptions.SelectedValue.Replace(" ", "")))
            {
                case eInvoiceFilterType.NormalInvoice:
                    performSearch();
                    break;
                case eInvoiceFilterType.OneLinerInvoice:
                    performSearch();
                    break;
                case eInvoiceFilterType.InvoiceId:
                    LocateInvoice();
                    break;
                case eInvoiceFilterType.SelfBillInvoice:
                    performSelfBillSearch();
                    break;
                case eInvoiceFilterType.Extra:
                    performSelfBillSearch();
                    break;
                case eInvoiceFilterType.SubContractorInvoice:
                    performSubContractorSearch();
                    break;
            }
        }



        #endregion

        #region Populate Static Controls
        ///	<summary> 
        ///	Populate Static Controls
        ///	</summary>
        private void PopulateStaticControls()
        {
            rdoFilterOptions.DataSource = Utilities.UnCamelCase(Enum.GetNames(typeof(eInvoiceFilterType)));
            rdoFilterOptions.DataBind();
            rdoFilterOptions.Items[0].Selected = true;

            cboJobState.DataSource = Enum.GetNames(typeof(eJobState));
            cboJobState.DataBind();
            cboJobState.SelectedValue = eJobState.Invoiced.ToString();

            rdoPosted.DataSource = Enum.GetNames(typeof(eWithOrWithout));
            rdoPosted.DataBind();
            rdoPosted.Items[2].Selected = true;

        }

        #region DBCombo's Server Methods and Initialisation


        void cboSubContractor_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {

            cboSubContractor.Items.Clear();

            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllSubContractorsFiltered(e.Text);

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
                cboSubContractor.Items.Add(rcItem);
            }

            if (dt.Rows.Count > 0)
            {
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), dt.Rows.Count.ToString());
            }
        }
        void cboClient_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {

            cboClient.Items.Clear();

            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllClientsFiltered(e.Text);

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



        #endregion

        #endregion

        #region Methods & Events
        ///	<summary> 
        ///	Button Search Click
        ///	</summary>
        private void btnSearch_Click(object sender, System.EventArgs e)
        {
            Page.Validate();

            if (Page.IsValid)
            {
                switch ((eInvoiceFilterType)Enum.Parse(typeof(eInvoiceFilterType), rdoFilterOptions.SelectedValue.Replace(" ", "")))
                {
                    case eInvoiceFilterType.NormalInvoice:
                        performSearch();
                        break;
                    case eInvoiceFilterType.OneLinerInvoice:
                        performSearch();
                        break;
                    case eInvoiceFilterType.InvoiceId:
                        LocateInvoice();
                        break;
                    case eInvoiceFilterType.SelfBillInvoice:
                        performSelfBillSearch();
                        break;
                    case eInvoiceFilterType.Extra:
                        performSelfBillSearch();
                        break;
                    case eInvoiceFilterType.SubContractorInvoice:
                        performSubContractorSearch();
                        break;
                }
            }
        }

        private void LocateInvoice()
        {
            int invoiceId = 0;
            invoiceId = Convert.ToInt32(txtInvoiceId.Text);

            Facade.IInvoice facInvoice = new Facade.Invoice();

            DataSet ds = facInvoice.GetInvoiceForInvoiceId(invoiceId);

            if (ds.Tables[0].Rows.Count != 0)
            {
                if (Convert.ToBoolean(ds.Tables[0].Rows[0]["ForCancellation"]) != true)
                {
                    // Populate ClientId Session Variable
                    int clientId = 0;
                    clientId = Convert.ToInt32(ds.Tables[0].Rows[0]["IdentityId"]);

                    //#15867 J.Steele 
                    //Clear the Invoice Session variables before setting the specific ones
                    Utilities.ClearInvoiceSession();
                    Session["ClientId"] = clientId;

                    // Find which invoice page to load
                    switch ((eInvoiceType)Enum.Parse(typeof(eInvoiceType), ds.Tables[0].Rows[0]["InvoiceType"].ToString()))
                    {
                        case eInvoiceType.Normal:
                            Response.Redirect("addupdateInvoice.aspx?InvoiceId=" + invoiceId, false);
                            break;

                        case eInvoiceType.SelfBill:
                            Response.Redirect("addupdateInvoice.aspx?invoiceId=" + invoiceId, false);
                            break;

                        case eInvoiceType.OneLiner:
                            Response.Redirect("addupdateonelinerinvoice.aspx?InvoiceId=" + invoiceId, false);
                            break;

                        case eInvoiceType.SelfBillRemainder:
                            Response.Redirect("addupdateselfbillinvoice.aspx?invoiceId=" + invoiceId, false);
                            break;

                        case eInvoiceType.Extra:
                            Response.Redirect("addupdateextrainvoice.aspx?invoiceId=" + invoiceId, false);
                            break;

                        case eInvoiceType.ClientInvoicing:
                            // Register a script that spawns a new window displaying the groupage invoice pdf.
                            string pdfLocation = Globals.Configuration.WebServer + ds.Tables[0].Rows[0]["PDFLocation"];
                            string openInvoicePdfScript = string.Format("openResizableDialogWithScrollbars('{0}', 700, 600);", pdfLocation.Replace(@"\", @"/"));
                            this.ClientScript.RegisterClientScriptBlock(this.GetType(), "OpenInvoicePdfByInvoiceId", openInvoicePdfScript, true);
                            break;
                    }
                }
                else
                {
                    lblNote.Visible = true;
                    lblNote.Text = "The Invoice Id " + txtInvoiceId.Text + " has been deleted!";
                    lblNote.ForeColor = Color.Red;
                }
            }
            else
            {
                lblNote.Visible = true;
                lblNote.Text = "The Invoice Id " + txtInvoiceId.Text + " has not been found!";
                lblNote.ForeColor = Color.Red;
            }
        }

        private void performSearch()
        {
            int searchInvoiceType;

            switch ((eInvoiceFilterType)Enum.Parse(typeof(eInvoiceFilterType), rdoFilterOptions.SelectedValue.Replace(" ", "")))
            {
                case eInvoiceFilterType.NormalInvoice:
                    searchInvoiceType = (int)eInvoiceType.Normal;
                    break;
                case eInvoiceFilterType.OneLinerInvoice:
                    searchInvoiceType = (int)eInvoiceType.OneLiner;
                    break;
                default:
                    searchInvoiceType = (int)eInvoiceType.Normal;
                    break;
            }

            Facade.IInvoice facInvoice = new Facade.Invoice();

            int posted = 0;

            posted = rdoPosted.SelectedIndex;
            switch ((eWithOrWithout)Enum.Parse(typeof(eWithOrWithout), rdoPosted.SelectedValue))
            {
                case eWithOrWithout.All:
                    posted = 2;
                    break;
                case eWithOrWithout.With:
                    posted = 1;
                    break;
                case eWithOrWithout.Without:
                    posted = 0;
                    break;
            }

            int clientId = cboClient.SelectedValue == "" ? 0 : Convert.ToInt32(cboClient.SelectedValue);

            DateTime startDate = dteStartDate.SelectedDate.Value;
            startDate = startDate.Subtract(startDate.TimeOfDay);

            DateTime endDate = Convert.ToDateTime(dteEndDate.SelectedDate);
            endDate = endDate.Subtract(endDate.TimeOfDay);
            endDate = endDate.Add(new TimeSpan(23, 59, 59));

            // Job State
            eJobState jobState = new eJobState();

            if (cboJobState.SelectedValue != string.Empty)
                jobState = (eJobState)Enum.Parse(typeof(eJobState), cboJobState.SelectedValue);

            if (startDate != DateTime.MinValue && endDate != DateTime.MinValue)
            {
                dsInvoice = facInvoice.GetwithParamsAndDate(clientId, string.Empty, searchInvoiceType == (int)eInvoiceType.Normal ? jobState : 0, posted, startDate, endDate, (eInvoiceType)searchInvoiceType);
            }
            else
            {
                if (clientId == 0)
                    dsInvoice = facInvoice.GetAll(posted, (eInvoiceType)searchInvoiceType);
                else
                    dsInvoice = facInvoice.GetwithParams(clientId, string.Empty,
                        searchInvoiceType == (int)eInvoiceType.Normal ? jobState : 0,
                        posted, (eInvoiceType)searchInvoiceType);
            }

            gvInvoices.DataSource = dsInvoice;

            ViewState[C_InvoiceDATA_VS] = dsInvoice;

            try
            {
                gvInvoices.DataBind();
            }
            catch (Exception)
            {
                gvInvoices.DataBind();
            }

            if (gvInvoices.Rows.Count == 0)
            {
                pnlNormalInvoice.Visible = false;
                gvInvoices.Visible = false;
                lblNote.Text = "There are no invoices for the given criteria.";
                lblNote.ForeColor = Color.Red;
                lblNote.Visible = true;
            }
            else
            {
                pnlNormalInvoice.Visible = true;
                gvInvoices.Visible = true;
            }
        }

        private void performSelfBillSearch()
        {
            Facade.IInvoice facInvoice = new Facade.Invoice();

            eInvoiceType searchInvoiceType = eInvoiceType.SelfBill;

            switch ((eInvoiceFilterType)Enum.Parse(typeof(eInvoiceFilterType), rdoFilterOptions.SelectedValue.Replace(" ", "")))
            {
                case eInvoiceFilterType.SelfBillInvoice:
                    searchInvoiceType = eInvoiceType.SelfBill;
                    break;
                case eInvoiceFilterType.Extra:
                    searchInvoiceType = eInvoiceType.Extra;
                    break;
                default:
                    searchInvoiceType = eInvoiceType.Normal;
                    break;
            }
            int posted = 0;

            switch ((eWithOrWithout)Enum.Parse(typeof(eWithOrWithout), rdoPosted.SelectedValue))
            {
                case eWithOrWithout.All:
                    posted = 2;
                    break;
                case eWithOrWithout.With:
                    posted = 1;
                    break;
                case eWithOrWithout.Without:
                    posted = 0;
                    break;
            }

            int clientId = cboClient.SelectedValue == "" ? 0 : Convert.ToInt32(cboClient.SelectedValue);

            string clientInvoiceNumber = string.Empty;

            if (txtClientInvoiceNumber.Text != string.Empty)
                clientInvoiceNumber = txtClientInvoiceNumber.Text;

            DateTime startDate = dteStartDate.SelectedDate.Value;
            startDate = startDate.Subtract(startDate.TimeOfDay);

            DateTime endDate = dteEndDate.SelectedDate.Value;
            endDate = endDate.Subtract(endDate.TimeOfDay);
            endDate = endDate.Add(new TimeSpan(23, 59, 59));

            // Job State
            eJobState jobState = new eJobState();
            if (cboJobState.SelectedValue != string.Empty)
                jobState = (eJobState)Enum.Parse(typeof(eJobState), cboJobState.SelectedValue);

            if (startDate != DateTime.MinValue && endDate != DateTime.MinValue)
            {
                dsInvoice = facInvoice.GetwithParamsAndDate(clientId, clientInvoiceNumber, jobState, posted, startDate, endDate, searchInvoiceType);
            }
            else
            {
                if (clientId == 0)
                    dsInvoice = facInvoice.GetAll(posted, searchInvoiceType);
                else
                    dsInvoice = facInvoice.GetwithParams(clientId, clientInvoiceNumber, jobState, posted, searchInvoiceType);
            }

            gvInvoices.Columns[7].Visible = true;
            gvInvoices.DataSource = dsInvoice;

            ViewState[C_InvoiceDATA_VS] = dsInvoice;

            gvInvoices.DataBind();

            if (gvInvoices.Rows.Count == 0)
            {
                pnlNormalInvoice.Visible = false;
                lblNote.Text = "There are no invoices for the given criteria.";
                lblNote.ForeColor = Color.Red;
                lblNote.Visible = true;
            }
            {
                pnlNormalInvoice.Visible = true;
                gvInvoices.Visible = true;
            }
        }

        private void performSubContractorSearch()
        {
            Facade.IInvoiceSubContrator facInvoice = new Facade.Invoice();

            int posted = 0;

            switch ((eWithOrWithout)Enum.Parse(typeof(eWithOrWithout), rdoPosted.SelectedValue))
            {
                case eWithOrWithout.All:
                    posted = 2;
                    break;
                case eWithOrWithout.With:
                    posted = 1;
                    break;
                case eWithOrWithout.Without:
                    posted = 0;
                    break;
            }

            int clientId = cboSubContractor.SelectedValue == "" ? 0 : Convert.ToInt32(cboSubContractor.SelectedValue);

            string clientInvoiceNumber = String.Empty;

            if (txtClientInvoiceNumber.Text != string.Empty)
                clientInvoiceNumber = txtClientInvoiceNumber.Text;

            DateTime startDate = DateTime.MinValue;
            DateTime endDate = DateTime.MinValue;

            if (dteStartDate.SelectedDate.HasValue)
            {
                startDate = dteStartDate.SelectedDate.Value;
                startDate = startDate.Subtract(startDate.TimeOfDay);
            }

            if (dteEndDate.SelectedDate.HasValue)
            {
                endDate = dteEndDate.SelectedDate.Value;
                endDate = endDate.Subtract(endDate.TimeOfDay);
                endDate = endDate.Add(new TimeSpan(23, 59, 59));
            }

            // Job State
            eJobState jobState = new eJobState();
            if (cboJobState.SelectedValue != string.Empty)
                jobState = (eJobState)Enum.Parse(typeof(eJobState), cboJobState.SelectedValue);

            if (startDate != DateTime.MinValue && endDate != DateTime.MinValue)
            {
                dsInvoice = facInvoice.GetSubContractorswithParamsAndDate(clientId, clientInvoiceNumber, jobState, posted, startDate, endDate, eInvoiceType.SubContract);
            }
            else
            {
                if (clientId == 0)
                    dsInvoice = facInvoice.GetSubContractors(posted, eInvoiceType.SubContract);
                else
                    dsInvoice = facInvoice.GetSubContractorswithParams(clientId, clientInvoiceNumber, jobState, posted, eInvoiceType.SubContract);
            }

            gvSubContratorInvoice.DataSource = dsInvoice;

            ViewState[C_InvoiceDATA_VS] = dsInvoice;

            gvSubContratorInvoice.DataBind();


            if (gvSubContratorInvoice.Rows.Count == 0)
            {
                pnlSubContractorInvoice.Visible = false;
                lblNote.Text = "There are no invoices for the given criteria.";
                lblNote.ForeColor = Color.Red;
                lblNote.Visible = true;
            }
            else
            {
                pnlSubContractorInvoice.Visible = true;
            }

            lblClientInvoiceNumber.Visible = txtClientInvoiceNumber.Visible = true;
        }

        private void rdoFilterOptions_SelectedIndexChanged(object sender, System.EventArgs e)
        {
            #region Clear Fields & Hide Panels (General)
            HideGridPanels();
            lblNote.Visible = false;
            txtInvoiceId.Text = string.Empty;
            cboClient.Text = string.Empty;
            cboSubContractor.Text = string.Empty;
            rdoPosted.SelectedIndex = 2;
            #endregion

            switch ((eInvoiceFilterType)Enum.Parse(typeof(eInvoiceFilterType), rdoFilterOptions.SelectedValue.Replace(" ", "")))
            {
                case eInvoiceFilterType.NormalInvoice:
                    //performSearch();

                    #region Clear Fields & Hide Panels (Normal Invoice Specific)
                    pnlFilter.Visible = true;

                    lblSubContractor.Visible = false;
                    cboSubContractor.Visible = false;

                    lblJobState.Visible = false;
                    cboJobState.Visible = false;

                    lblClient.Visible = true;
                    cboClient.Visible = true;

                    rfvInvoiceId.Visible = false;
                    pnlInvoiceId.Visible = false;

                    pnlPosted.Visible = true;
                    txtClientInvoiceNumber.Visible = false;
                    lblClientInvoiceNumber.Visible = false;
                    #endregion
                    break;
                case eInvoiceFilterType.OneLinerInvoice:
                    // TODO: ((HyperLinkColumn)dgInvoice.Levels[0].Columns[0].DataCellClientTemplateId[0]).DataNavigateUrlFormatString = "addupdateonelinerinvoice.aspx?InvoiceId={0}";
                    //   performSearch();

                    #region Clear Fields & Hide Panels (OneLiner Invoice Specific)
                    pnlFilter.Visible = true;

                    lblSubContractor.Visible = false;
                    cboSubContractor.Visible = false;

                    lblJobState.Visible = false;
                    cboJobState.Visible = false;

                    lblClient.Visible = true;
                    cboClient.Visible = true;

                    rfvInvoiceId.Visible = false;
                    pnlInvoiceId.Visible = false;

                    pnlPosted.Visible = true;
                    txtClientInvoiceNumber.Visible = false;
                    lblClientInvoiceNumber.Visible = false;
                    #endregion

                    break;
                case eInvoiceFilterType.SelfBillInvoice:
                    //performSelfBillSearch();

                    #region Clear Fields & Hide Panels	(Self Bill Invoice Specific)
                    pnlFilter.Visible = true;

                    lblSubContractor.Visible = false;
                    cboSubContractor.Visible = false;

                    lblJobState.Visible = false;
                    cboJobState.Visible = false;

                    lblClient.Visible = true;
                    cboClient.Visible = true;

                    rfvInvoiceId.Visible = false;
                    pnlInvoiceId.Visible = false;

                    pnlPosted.Visible = true;
                    txtClientInvoiceNumber.Visible = true;
                    lblClientInvoiceNumber.Visible = true;
                    #endregion
                    break;

                case eInvoiceFilterType.Extra:
                    //performSelfBillSearch();

                    #region Clear Fields & Hide Panels	(Extra Invoice Specific)
                    pnlFilter.Visible = true;

                    lblSubContractor.Visible = false;
                    cboSubContractor.Visible = false;

                    lblJobState.Visible = false;
                    cboJobState.Visible = false;

                    lblClient.Visible = true;
                    cboClient.Visible = true;

                    rfvInvoiceId.Visible = false;
                    pnlInvoiceId.Visible = false;

                    pnlPosted.Visible = true;
                    txtClientInvoiceNumber.Visible = false;
                    lblClientInvoiceNumber.Visible = false;
                    #endregion
                    break;
                case eInvoiceFilterType.SubContractorInvoice:
                    //	performSubContractorSearch();

                    #region Clear Fields & Hide Panels (Sub Contractor Invoice Specific)
                    pnlFilter.Visible = true;

                    lblSubContractor.Visible = true;
                    cboSubContractor.Visible = true;

                    lblJobState.Visible = true;
                    cboJobState.Visible = true;

                    lblClient.Visible = false;
                    cboClient.Visible = false;

                    lblJobState.Visible = false;
                    cboJobState.Visible = false;

                    rfvInvoiceId.Visible = false;
                    pnlInvoiceId.Visible = false;

                    pnlPosted.Visible = false;
                    txtClientInvoiceNumber.Visible = true;
                    lblClientInvoiceNumber.Visible = true;
                    #endregion
                    break;
                case eInvoiceFilterType.InvoiceId:

                    #region Clear Fields & Hide Panels (Invoice Id Specific)
                    pnlFilter.Visible = false;

                    rfvInvoiceId.Visible = true;
                    pnlInvoiceId.Visible = true;

                    pnlPosted.Visible = false;
                    txtClientInvoiceNumber.Visible = false;
                    lblClientInvoiceNumber.Visible = false;
                    #endregion
                    break;
            }
        }
        private void HideGridPanels()
        {
            pnlNormalInvoice.Visible = true;
            pnlSubContractorInvoice.Visible = true;
            gvInvoices.Visible = false;
            lblNote.Visible = false;
            txtClientInvoiceNumber.Visible = false;
            lblClientInvoiceNumber.Visible = false;
        }

        private void rdoPosted_SelectedIndexChanged(object sender, EventArgs e)
        {
            HideGridPanels();
            txtClientInvoiceNumber.Text = string.Empty;
        }

        void cboSubContractor_SelectedIndexChanged(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            HideGridPanels();
        }

        void cboClient_SelectedIndexChanged(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            HideGridPanels();
        }


        private void cboJobState_SelectedIndexChanged(object sender, EventArgs e)
        {
            HideGridPanels();
        }
        #endregion

        #region Normal Invoice Grid

        void dgInvoice_ItemContentCreated(object sender, ComponentArt.Web.UI.GridItemContentCreatedEventArgs e)
        {
            HtmlAnchor link = (HtmlAnchor)e.Content.FindControl("lnkInvoiceId");

            switch ((eInvoiceFilterType)Enum.Parse(typeof(eInvoiceFilterType), rdoFilterOptions.SelectedValue.Replace(" ", "")))
            {
                case eInvoiceFilterType.NormalInvoice:
                    link.HRef = "addupdateInvoice.aspx?InvoiceId=" + e.Item["InvoiceId"];
                    link.InnerText = e.Item["InvoiceId"].ToString();
                    break;
                case eInvoiceFilterType.OneLinerInvoice:
                    link.HRef = "addupdateonelinerinvoice.aspx?InvoiceId=" + e.Item["InvoiceId"];
                    link.InnerText = e.Item["InvoiceId"].ToString();
                    break;
            }
        }

        #endregion

        #region Self Bill Invoice Grid

        void dgSelfBillInvoice_ItemContentCreated(object sender, ComponentArt.Web.UI.GridItemContentCreatedEventArgs e)
        {
            HtmlAnchor link = (HtmlAnchor)e.Content.FindControl("lnkSelfInvoiceId");

            switch ((eInvoiceFilterType)Enum.Parse(typeof(eInvoiceFilterType), rdoFilterOptions.SelectedValue.Replace(" ", "")))
            {
                case eInvoiceFilterType.SelfBillInvoice:
                    link.HRef = "addupdateInvoice.aspx?InvoiceId=" + e.Item["InvoiceId"];
                    link.InnerText = e.Item["InvoiceId"].ToString();
                    break;
                case eInvoiceFilterType.Extra:
                    link.HRef = "addupdateextrainvoice.aspx?InvoiceId=" + e.Item["InvoiceId"];
                    link.InnerText = e.Item["InvoiceId"].ToString();
                    break;
            }
        }

        #endregion

        #region Sub Contractor Invoice Grid

        #endregion

        #region Validation

        protected void ValidateInvoiceId(object obj, ServerValidateEventArgs args)
        {
            args.IsValid = P1TP.Components.Common.SimpleValidation.ValidateNumericValue(args.Value, 0, true);
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
            this.Init += new System.EventHandler(this.ListInvoice_Init);

        }


        protected string GetInvoiceLinkURL(eInvoiceFilterType invoiceFilterType)
        {
            string retVal = string.Empty;
            switch (invoiceFilterType)
            {
                case eInvoiceFilterType.NormalInvoice:
                    retVal = "addupdateinvoice";
                    break;
                case eInvoiceFilterType.OneLinerInvoice:
                    retVal = "addupdateonelinerinvoice";
                    break;
                case eInvoiceFilterType.Extra:
                    retVal = "addupdateextrainvoice";
                    break;
                case eInvoiceFilterType.SelfBillInvoice:
                    retVal = "addupdateInvoice";
                    break;
                case eInvoiceFilterType.SubContractorInvoice:
                    retVal = "";
                    break;
                default:
                    retVal = "addupdateinvoice.aspx";
                    break;
            }

            return retVal + ".aspx?InvoiceId=";
        }
        #endregion
    }
}

