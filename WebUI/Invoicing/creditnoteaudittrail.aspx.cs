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

using P1TP.Components.Web.UI;
using P1TP.Components.Web.Validation;
using Orchestrator.WebUI.UserControls;
using Orchestrator.Globals;
using System.Collections.Generic;
using System.Globalization;

namespace Orchestrator.WebUI.Invoicing
{
	/// <summary>
	/// Summary description for InvoiceAuditTrail.
	/// </summary>
	public partial class CreditNoteAuditTrail : Orchestrator.Base.BasePage, IPostBackEventHandler  
	{
		#region Constants

		private const string C_SORT_DIRECTION = "C_SORT_DIRECTION";
		private const string C_SORT_FIELD = "C_SORT_FIELD";

		#endregion

		#region Form Elements

        private Dictionary<int, CultureInfo> cultures = new Dictionary<int, CultureInfo>();

		#endregion

		#region Property Interfaces

		public string SortDirection
		{
			get
			{
				if (ViewState[C_SORT_FIELD] == null)
					return "ASC";
				else
					return (string) ViewState[C_SORT_FIELD];
			}
			set
			{
				ViewState[C_SORT_DIRECTION] = value;
			}
		}

		public string SortField
		{
			get
			{
				if (ViewState[C_SORT_DIRECTION] == null)
					return "CreditNoteDate";
				else
					return (string) ViewState[C_SORT_DIRECTION];
			}
			set
			{
				ViewState[C_SORT_FIELD] = value;
			}
		}

		#endregion

        //#region Web Form Designer generated code
        //override protected void OnInit(EventArgs e)
        //{
        //    //
        //    // CODEGEN: This call is required by the ASP.NET Web Form Designer.
        //    //
        //    InitializeComponent();
        //    base.OnInit(e);
        //}

        ///// <summary>
        ///// Required method for Designer support - do not modify
        ///// the contents of this method with the code editor.
        ///// </summary>
        //private void InitializeComponent()
        //{
        //    this.Init += new EventHandler(Page_Init);
        //}
        //#endregion

        #region Page Load/Init


		protected void Page_Load(object sender, System.EventArgs e)
		{
            if (Request.QueryString["rcbID"] != null) return;

            // add the english culture to the list.
            this.cultures.Add(2057, new CultureInfo(2057));

            if (!IsPostBack)
            {
                hidSelectedCreditNotes.Value = string.Empty;
                Utilities.ClearInvoiceSession();
                PopulateStaticControls();
            }
		}

		private void Page_Init(object sender, EventArgs e)
		{
            //Do button event hookups in control properties
			//btnFilter.Click += new EventHandler(btnFilter_Click);
            //btnExportToCSV.Click += new EventHandler(btnExportToCSV_Click);
            //this.btnPost.Click += new EventHandler(btnPost_Click);

            this.cboClient.SelectedIndexChanged += new Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventHandler(cboClient_SelectedIndexChanged);
            this.cboClient.ItemsRequested  +=new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);
            
            dgCreditNotes.RowDataBound += new GridViewRowEventHandler(dgCreditNotes_RowDataBound);
		}

        protected void btnPost_Click(object sender, EventArgs e)
        {
            if (hidSelectedCreditNotes.Value.Length > 0)
                PostToAccountsSystem();
        }


        void dgCreditNotes_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                DataRowView drv = e.Row.DataItem as DataRowView;

                e.Row.Attributes.Add("onDblClick", Page.ClientScript.GetPostBackEventReference(this, e.Row.DataItemIndex.ToString()));

                int LCID = 2057;
                if (drv["LCID"] != DBNull.Value && Convert.ToInt32(drv["LCID"]) != -1)
                    LCID = Convert.ToInt32(drv["LCID"]);

                if (!this.cultures.ContainsKey(LCID))
                    this.cultures.Add(LCID, new CultureInfo(LCID));

                //TODO: Update to use foreign amounts
                // add formatted currency values based on culture.
                Label netAmountLabel = (Label)e.Row.FindControl("netCurrencyLabel");
                netAmountLabel.Text = Convert.ToDecimal(drv["ForeignNetAmount"]).ToString("C", this.cultures[LCID]);
                //netAmountLabel.Text = Convert.ToDecimal(drv["NetAmount"]).ToString("C", this.cultures[LCID]);

                Label VATAmountLabel = (Label)e.Row.FindControl("VATCurrencyLabel");
                VATAmountLabel.Text = Convert.ToDecimal(drv["ForeignVATAmount"]).ToString("C", this.cultures[LCID]);
                //VATAmountLabel.Text = Convert.ToDecimal(drv["VATAmount"]).ToString("C", this.cultures[LCID]);

                Label grossAmountLabel = (Label)e.Row.FindControl("grossCurrencyLabel");
                grossAmountLabel.Text = Convert.ToDecimal(drv["ForeignGrossAmount"]).ToString("C", this.cultures[LCID]);
                //grossAmountLabel.Text = Convert.ToDecimal(drv["GrossAmount"]).ToString("C", this.cultures[LCID]);

                HtmlAnchor lnkViewInvoice = e.Row.FindControl("lnkViewInvoice") as HtmlAnchor;
                HtmlAnchor lnkEditInvoice = e.Row.FindControl("lnkEditInvoice") as HtmlAnchor;

                string creditNoteTarget;

                if (drv["PDFLocation"] != DBNull.Value)
                    creditNoteTarget = Globals.Configuration.WebServer + (string)drv["PDFLocation"];
                else
                    creditNoteTarget = "/invoicing/addupdateonelinercredit.aspx?creditNoteId=" + drv["CreditNoteId"].ToString();
                
                lnkViewInvoice.HRef = creditNoteTarget;

                lnkEditInvoice.HRef = "/invoicing/addupdateonelinercredit.aspx?creditNoteId=" + drv["CreditNoteId"].ToString();
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

            DisplayCreditNoteAuditReport(((GridViewSelectEventArgs)e).NewSelectedIndex);
        }
        
        protected void btnExportToCSV_Click(object sender, EventArgs e)
        {
            DataView dvData = GetData();
            Session["__ExportDS"] = dvData.Table;
            //TODO This looks like a unique name for the files is NOT generated therefore
            //it will get overwritten and possibly viewed by the wrong user
            Server.Transfer("../Reports/csvexport.aspx?filename=CreditNoteAudit.csv");
        }

        void cboClient_SelectedIndexChanged(object o, Telerik.Web.UI.RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if (cboClient.SelectedItem != null)
            {
                PopulateCreditNotes();
            }
            else
            {
                dgCreditNotes.Visible = false;
                reportViewer.Visible = false;
            }
        }

		#endregion

		private void DisplayCreditNoteAuditReport(int invoiceId)
		{
			// Cause the report to be displayed containing the job information for the specified invoice.
            //Facade.CreditNote facCreditNote = new Facade.CreditNote();
            //DataSet dsInvoiceAudit = facCreditNote.GetInvoiceContents(invoiceId);

            //// Configure the report settings collection
            //NameValueCollection reportParams = new NameValueCollection();

            //// Configure the Session variables used to pass data to the report
            //Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.InvoiceContents;
            //Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = dsInvoiceAudit;
            //Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
            //Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";

            //// Setting the identity id of the report allows us to configure the fax and email boxes suitably.
            //if (cboClient.SelectedValue != "")
            //    reportViewer.IdentityId = Convert.ToInt32(cboClient.SelectedValue);
            //reportViewer.Visible = true;
		}
        
		private void PopulateStaticControls()
		{
            //cboInvoiceType.DataSource = Utilities.UnCamelCase(Enum.GetNames(typeof(eInvoiceType)));
            //cboInvoiceType.DataBind();
            //cboInvoiceType.Items.Insert(0, new ListItem("All Types", "0"));

            rblDateType.SelectedIndex = 0;

            dteStartDate.SelectedDate = DateTime.Now.Date.AddDays(-1);
			dteEndDate.SelectedDate = DateTime.Now;

		}

        private DataView GetData()
        {
            int clientId = 0;
            int.TryParse(cboClient.SelectedValue, out clientId);
            //try
            //{
            //    clientId = Convert.ToInt32(cboClient.SelectedValue);
            //}
            //catch { }

            //int invoiceTypeId = 0;
            //try
            //{
            //    eInvoiceType invoiceType = (eInvoiceType)Enum.Parse(typeof(eInvoiceType), cboInvoiceType.SelectedValue.Replace(" ", ""));
            //    invoiceTypeId = (int)invoiceType;
            //}
            //catch { }

            DateTime startDate = dteStartDate.SelectedDate.Value;
            startDate = startDate.Subtract(startDate.TimeOfDay);

            //Add 1 day to the end date entered so that invoices before
            //that datetime are returned.
            DateTime endDate = dteEndDate.SelectedDate.Value;
            endDate = endDate.Subtract(endDate.TimeOfDay);
            endDate = endDate.AddDays(1);

            int dateType = int.Parse(rblDateType.SelectedValue);

            Facade.CreditNote facCreditNote = new Facade.CreditNote();
            DataSet dsCreditNotes = facCreditNote.GetCreditNotesForAuditTrail(clientId, dateType, startDate, endDate);

            DataView dvCreditNotes = new DataView(dsCreditNotes.Tables[0]);
            // Stephen Newman 15/05/2006
            // Stop applying default sort i.e. Invoice Date
            //dvInvoices.Sort = (SortField + " " + SortDirection).Trim();

            return dvCreditNotes;
        }

		private void PopulateCreditNotes()
		{
            hidSelectedCreditNotes.Value = string.Empty;
            DataView dvCreditNotes = GetData();
            dgCreditNotes.DataSource = dvCreditNotes;
			dgCreditNotes.DataBind();
            if (dvCreditNotes.Table.Rows.Count > 0)
                dgCreditNotes.PageSize = dvCreditNotes.Table.Rows.Count;
            dgCreditNotes.Visible = true;

			reportViewer.Visible = false;
		}


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

        protected void btnFilter_Click(object sender, EventArgs e)
		{
			PopulateCreditNotes();
		}
       
        void dgCreditNotes_SelectCommand(object sender, ComponentArt.Web.UI.GridItemEventArgs e)
        {
            if (e.Item[0].ToString() != "")
            {
                reportViewer.Visible = false;

                // Display the selected Credit Note in the report.
                int creditNoteId = Convert.ToInt32(e.Item[0]);
                DisplayCreditNoteAuditReport(creditNoteId);
            }
        }

		#endregion

        #region Posting to Accounts System
        private void PostToAccountsSystem()
        {
            Entities.CreditNote creditNote = null;
            Facade.CreditNote facCreditNote = new Facade.CreditNote();

            if (hidSelectedCreditNotes.Value.EndsWith(",")) hidSelectedCreditNotes.Value = hidSelectedCreditNotes.Value.Substring(0, hidSelectedCreditNotes.Value.Length - 1);

            foreach (string creditNoteId in hidSelectedCreditNotes.Value.Split(','))
            {

                creditNote = PopulateCreditNote(Int32.Parse(creditNoteId));
                try
                {
                    UpdateCreditNote(creditNote);
                }
                catch (Exception e)
                {
                    string err = GetErrorString(e);
                    lblError.Text = "There was a problem posting this Credit Note Id " + creditNoteId.ToString() + " this could be because it has already been posted to your account's system." + "<br/>" + err;
                    pnlError.Visible = true;
                    hidSelectedCreditNotes.Value = "";
                    return;
                }

            }

            PopulateCreditNotes();
        }


        private string GetErrorString(Exception ex)
        {

            string errorString = ex.Message;
            if (ex.InnerException != null)
                errorString += "<br/>" + GetErrorString(ex.InnerException);

            return errorString;
        }
        private Entities.CreditNote PopulateCreditNote(int creditNoteId)
        {
            Entities.CreditNote creditNote = null;

            Facade.CreditNote facCreditNote = new Orchestrator.Facade.CreditNote();
            creditNote = facCreditNote.GetForCreditNoteId(creditNoteId);
            
            creditNote.Posted = true;

            return creditNote;
        }

        private void UpdateCreditNote(Entities.CreditNote creditNote)
        {
            Facade.CreditNote facCreditNote = new Facade.CreditNote();
            string userName = ((Entities.CustomPrincipal)Page.User).UserName;
            facCreditNote.UpdateOneLiner(creditNote, userName);
        }
        #endregion
    }
}
