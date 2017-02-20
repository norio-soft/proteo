using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

using System.Collections.Generic;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.Invoicing.SubContractorSB
{
    public partial class autorunconfirmation : Orchestrator.Base.BasePage
    {
        #region Page Fields

        public int _numberOfSubContracts = 0;
        public decimal _valueOfSubContracts = 0;
        public Dictionary<int, string> SubbyInvoiceNumbers = new Dictionary<int, string>();

        protected int BatchID
        {
            get { return this.ViewState["_batchID"] != null ? (int)this.ViewState["_batchID"] : -1; }
            set { this.ViewState["_batchID"] = value; }
        }
        #endregion

        #region Page Load/Init

        protected void Page_Load(object sender, EventArgs e)
        {
            Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.Invoicing);

            if (!IsPostBack)
            {
                int _batchID = -1;
                int.TryParse(Request.QueryString["bID"], out _batchID);
                if (_batchID > 0)
                {
                    BatchID = _batchID;
                    LoadBatchSummary();
                }
            }
            if (Request.QueryString["InvoiceNumbers"] != null)
            {
                string invoiceNumbers = Request.QueryString["InvoiceNumbers"];
                string[] splitBySubcontractId = invoiceNumbers.Split(new char[] { ',' });

                int idColumn = 0;
                int invoiceNoColumn = 1;

                foreach (string row in splitBySubcontractId)
                    if (!String.IsNullOrEmpty(row))
                    {
                        string[] splitIdAndInvoiceRef = row.Split(new char[] { ':' });
                        if (splitIdAndInvoiceRef.Length == 2)
                            SubbyInvoiceNumbers.Add(int.Parse(splitIdAndInvoiceRef[idColumn]),
                                splitIdAndInvoiceRef[invoiceNoColumn]);
                    }

                if (this.SubbyInvoiceNumbers.Count > 0)
                {
                    this.chkSelfBill.Checked = false;
                    this.pnlSubbySelfBill.Visible = false;
                }
            }
            else
                this.pnlSubbySelfBill.Visible = true;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            grdSubbies.ItemDataBound += new Telerik.Web.UI.GridItemEventHandler(grdSubbies_ItemDataBound);
            grdNotificationRecipients.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdNotificationRecipients_NeedDataSource);
            cfvNotificationPartiesMustBeSpecified.ServerValidate += new ServerValidateEventHandler(cfvNotificationPartiesMustBeSpecified_ServerValidate);
            chkSelfBill.CheckedChanged += new EventHandler(chkSelfBill_CheckedChanged);

            btnChangeBatch.Click += new EventHandler(btnChangeBatch_Click);
            btnCreateBatch.Click += new EventHandler(btnCreateBatch_Click);
        }

        #endregion

        private void LoadBatchSummary()
        {
            Facade.IPreInvoice facPreInvoice = new Facade.PreInvoice();
            DataSet ds = facPreInvoice.GetSummaryForBatchForSubbySelfBill(BatchID);
            grdSubbies.DataSource = ds;

            lblInvoicedate.Text = facPreInvoice.GetBatchInvoiceDate(BatchID).ToString("dd/MM/yyyy");
        }

        #region Event Handlers

        #region CheckBox Events

        void chkSelfBill_CheckedChanged(object sender, EventArgs e)
        {
            if (!chkSelfBill.Checked && grdSubbies.Items.Count == 1)
                pnlSubbyInvoiceNo.Visible = true;
            else
                pnlSubbyInvoiceNo.Visible = false;                
        }

        #endregion

        #region Button Events

        void btnChangeBatch_Click(object sender, EventArgs e)
        {
            Response.Redirect("autorunstart.aspx?bID=" + this.Request.QueryString["bID"]);
        }

        /// <summary>
        /// Call the Service To Create the Batch.
        /// </summary>
        void btnCreateBatch_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                List<Contracts.DataContracts.NotificationParty> notificationParties = new List<Contracts.DataContracts.NotificationParty>();

                foreach (GridItem item in grdNotificationRecipients.SelectedItems)
                {
                    Contracts.DataContracts.NotificationParty np = new Contracts.DataContracts.NotificationParty();
                    np.FullName = grdNotificationRecipients.MasterTableView.DataKeyValues[item.ItemIndex]["FullName"].ToString();
                    np.UserName = grdNotificationRecipients.MasterTableView.DataKeyValues[item.ItemIndex]["UserName"].ToString();
                    np.EmailAddress = grdNotificationRecipients.MasterTableView.DataKeyValues[item.ItemIndex]["EmailAddress"].ToString();

                    notificationParties.Add(np);
                }

                try
                {
                    if (BatchID > 0)
                    {
                        if (this.SubbyInvoiceNumbers.Count == 0)
                            this.SubbyInvoiceNumbers.Add(-1, txtSubbyInvoiceNo.Text);
                        // Kick off the workflow.
                        GenerateInvoiceClient gic = new GenerateInvoiceClient("Orchestrator.InvoiceService");
                        gic.GenerateSubContractorInvoice(BatchID, notificationParties.ToArray(), chkSelfBill.Checked, this.SubbyInvoiceNumbers, ((Entities.CustomPrincipal)Page.User).UserName);

                        Server.Transfer("../../OfflineProcessInitiated.aspx");
                    }
                }
                catch (System.ServiceModel.EndpointNotFoundException exc)
                {
                    // Not possible to send message to workflow host - send email to support.
                    Utilities.SendSupportEmailHelper("GenerateInvoiceClient.GenerateSubContractorSelfBillInvoice(int, Orchestrator.Entities.NotificationParty[], string)", exc);
                    // Redirect user to an appropriate page.
                    Server.Transfer("../../OfflineProcessInitiationFailed.aspx");
                }
            }
        }

        #endregion

        #region Grid Events

        void grdNotificationRecipients_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            Facade.IUser facUser = new Facade.User();
            DataSet ds = facUser.GetAllUsersInRole(eUserRole.Invoicing);

            grdNotificationRecipients.DataSource = ds;
        }

        void grdSubbies_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem gdi = e.Item as GridDataItem;
                _numberOfSubContracts += int.Parse(gdi["subbyCount"].Text);
                _valueOfSubContracts += decimal.Parse(gdi["subbyValue"].Text, System.Globalization.NumberStyles.Any);
            }

            if (e.Item is GridFooterItem)
            {
                GridFooterItem gfi = e.Item as GridFooterItem;
                gfi["OrganisationName"].Text = "Total";
                gfi["subbyCount"].Text = _numberOfSubContracts.ToString();
                gfi["subbyValue"].Text = _valueOfSubContracts.ToString("C");
            }
        }

        #endregion

        #region Validation

        void cfvNotificationPartiesMustBeSpecified_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = grdNotificationRecipients.SelectedItems.Count > 0;
        }

        #endregion

        #endregion

        protected override PageStatePersister PageStatePersister
        {
            get
            {
                return new HiddenFieldPageStatePersister(this);
            }
        }
    }
}