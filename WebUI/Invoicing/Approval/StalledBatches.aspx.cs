using System;
using System.Data;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.Invoicing.Approval
{
    public partial class StalledBatches : Orchestrator.Base.BasePage
    {
        private const string _preInvoiceBatchID = "PreInvoiceBatchID";

        #region Page Load/Init

        protected void Page_Load(object sender, EventArgs e)
        {
            Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.Invoicing);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            gvBatches.ItemCommand += new Telerik.Web.UI.GridCommandEventHandler(gvBatches_ItemCommand);
            gvBatches.ItemDataBound += new Telerik.Web.UI.GridItemEventHandler(gvBatches_ItemDataBound);
            gvBatches.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(gvBatches_NeedDataSource);
            btnRefresh.Click += new EventHandler(btnRefresh_Click);
        }

        #endregion

        #region Event Handlers

        #region Grid Event Handlers

        void gvBatches_ItemCommand(object source, Telerik.Web.UI.GridCommandEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                int preInvoiceBatchID = int.Parse(gvBatches.MasterTableView.DataKeyValues[e.Item.ItemIndex][_preInvoiceBatchID].ToString());

                switch (e.CommandName.ToLower())
                {
                    case "continuegroup":
                        HiddenField hidInvoiceType = (HiddenField) e.Item.FindControl("hidInvoiceType");
                        eInvoiceType invoiceType = (eInvoiceType)int.Parse(hidInvoiceType.Value);
                        if (invoiceType == eInvoiceType.ClientInvoicing)
                        {
                            Response.Redirect(string.Format("../groupage/autorunconfirmation.aspx?bid={0}", preInvoiceBatchID));
                        }
                        else if (invoiceType == eInvoiceType.SubContractorSelfBill)
                        {
                            Response.Redirect(string.Format("../subcontractorsb/autorunconfirmation.aspx?bid={0}", preInvoiceBatchID));
                        }
                        break;
                    case "removegroup":
                        Facade.IPreInvoice facPreInvoice = new Facade.PreInvoice();
                        facPreInvoice.DeleteBatch(preInvoiceBatchID);
                        gvBatches.Rebind();
                        break;
                }
            }
        }

        void gvBatches_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is GridDataItem)
            {
                DataRowView drv = (DataRowView) e.Item.DataItem;
                int preInvoiceBatchID = int.Parse(gvBatches.MasterTableView.DataKeyValues[e.Item.ItemIndex][_preInvoiceBatchID].ToString());
                DataRow[] ordersContained =
                    drv.Row.Table.DataSet.Tables[1].Select(string.Format("PreInvoiceBatchID = {0}", preInvoiceBatchID));
                DataRow[] jobSubContractorsContained =
                    drv.Row.Table.DataSet.Tables[2].Select(string.Format("PreInvoiceBatchID = {0}", preInvoiceBatchID));

                Label lblInvoiceType = (Label) e.Item.FindControl("lblInvoiceType");
                HiddenField hidInvoiceType = (HiddenField)e.Item.FindControl("hidInvoiceType");
                Label lblItemCount = (Label) e.Item.FindControl("lblItemCount");

                if (ordersContained.Length > 0)
                {
                    lblInvoiceType.Text = "Groupage";
                    hidInvoiceType.Value = ((int)eInvoiceType.ClientInvoicing).ToString();
                    lblItemCount.Text = ordersContained.Length.ToString();
                }
                else if (jobSubContractorsContained.Length > 0)
                {
                    lblInvoiceType.Text = "Sub-Contractor";
                    hidInvoiceType.Value = ((int)eInvoiceType.SubContractorSelfBill).ToString();
                    lblItemCount.Text = jobSubContractorsContained.Length.ToString();
                }
            }
        }

        void gvBatches_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            Facade.IPreInvoice facPreInvoice = new Facade.PreInvoice();
            gvBatches.DataSource = facPreInvoice.GetStalledBatches();
        }

        #endregion

        #region Button Events

        void btnRefresh_Click(object sender, EventArgs e)
        {
            gvBatches.Rebind();
        }

        #endregion

        #endregion
    }
}