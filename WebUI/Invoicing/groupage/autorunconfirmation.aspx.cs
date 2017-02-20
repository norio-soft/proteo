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
using System.Collections.Specialized;
using System.Text;

using Telerik.Web.UI;

namespace Orchestrator.WebUI.Invoicing.Groupage
{
    public partial class autorunconfirmation : Orchestrator.Base.BasePage
    {
        #region Page Fields

        public int      _numberOfOrders = 0;
        public decimal  _valueOfOrders = 0;

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
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            
            this.grdOrders.ItemDataBound += new Telerik.Web.UI.GridItemEventHandler(grdOrders_ItemDataBound);
            grdOrders.PreRender += new EventHandler(grdOrders_PreRender);
            grdNotificationRecipients.NeedDataSource += new GridNeedDataSourceEventHandler(grdNotificationRecipients_NeedDataSource);
            
            this.btnChangeBatch.Click += new EventHandler(btnChangeBatch_Click);
            this.btnCreateBatch.Click += new EventHandler(btnCreateBatch_Click);
        }

        void grdNotificationRecipients_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            Facade.IUser facUser = new Facade.User();
            DataSet ds = facUser.GetAllUsersInRole(eUserRole.Invoicing);

            grdNotificationRecipients.DataSource = ds;
        }

        void grdOrders_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                GridDataItem gdi =  e.Item as GridDataItem;
                _numberOfOrders += int.Parse(gdi["OrderCount"].Text);
                _valueOfOrders += decimal.Parse(gdi["OrderValue"].Text, System.Globalization.NumberStyles.Any);
            }

            if(e.Item  is GridFooterItem)
            {
                GridFooterItem gfi = e.Item as GridFooterItem;
                gfi["OrganisationName"].Text = "Total";
                gfi["OrderCount"].Text = _numberOfOrders.ToString();
                gfi["OrderValue"].Text = _valueOfOrders.ToString("C");
            }
        }

        void grdOrders_PreRender(object sender, EventArgs e)
        {
            // Only allow the entry of the client invoice reference if there is one client being invoiced.
            trClientInvoiceReference.Visible = grdOrders.Items.Count == 1;
            trPOReference.Visible = grdOrders.Items.Count == 1;
        }

        #endregion

        #region Event Handlers

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
                        // Kick off the workflow.
                        GenerateInvoiceClient gic = new GenerateInvoiceClient("Orchestrator.InvoiceService");
                        gic.GenerateGroupageInvoiceAutoRun(BatchID,String.Empty, notificationParties.ToArray(), txtClientInvoiceReference.Text, txtPurchaseOrderReference.Text, ((Entities.CustomPrincipal)Page.User).UserName);

                        Server.Transfer("../../OfflineProcessInitiated.aspx");
                    }
                }
                catch (System.ServiceModel.EndpointNotFoundException exc)
                {
                    // Not possible to send message to workflow host - send email to support.
                    Utilities.SendSupportEmailHelper("GenerateInvoiceClient.GenerateGroupageInvoiceAutoRun(int, Orchestrator.Entities.NotificationParty[], string)", exc);
                    // Redirect user to an appropriate page.
                    Server.Transfer("../../OfflineProcessInitiationFailed.aspx");
                }
            }
        }

        #endregion

        #endregion

        #region Private Methods

        private void LoadBatchSummary()
        {
            Facade.IPreInvoice facPreInvoice = new Facade.PreInvoice();
            DataSet ds = facPreInvoice.GetSummaryForBatch(BatchID);
            this.grdOrders.DataSource = ds;

            lblInvoicedate.Text = ((DateTime)ds.Tables[0].Rows[0]["InvoiceDate"]).ToString("dd/MM/yy");
        }

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