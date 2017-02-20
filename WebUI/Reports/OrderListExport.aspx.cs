using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;

namespace Orchestrator.WebUI.Reports
{

    public partial class OrderListExport : System.Web.UI.Page
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                var customerIdentityID = Utilities.ParseNullable<int>(Request.QueryString["ciid"]);

                if (customerIdentityID.HasValue)
                {
                    Facade.IOrganisation facOrganisation = new Facade.Organisation();
                    this.clientCombo.Text = facOrganisation.GetNameForIdentityId(customerIdentityID.Value);
                    this.clientCombo.SelectedValue = customerIdentityID.Value.ToString();
                }

                this.startDate.SelectedDate = this.endDate.SelectedDate = DateTime.Today.AddDays(-1);
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            exportButton.Click += new EventHandler(exportButton_Click);
        }

        private void exportButton_Click(object sender, EventArgs e)
        {
            if (!Page.IsValid)
                return;

            var customerIdentityID = Utilities.ParseNullable<int>(clientCombo.SelectedValue);
            var fromDate = this.startDate.SelectedDate.Value.Date;
            var toDate = this.endDate.SelectedDate.Value.Date;
            var businessTypeIDs = this.businessTypeCheckList.AllBusinessTypesSelected ? null : this.businessTypeCheckList.SelectedBusinessTypeIDs;
            var subcontractorIdentityID = Utilities.ParseNullable<int>(this.cboSubcontractor.SelectedValue);
            var isInvoiced = this.rblInvoiced.SelectedIndex == 1 ? false : this.rblInvoiced.SelectedIndex == 2 ? true : (bool?)null;
            var hasSubcontractorInvoice = this.rblSubcontractorInvoiced.SelectedIndex == 1 ? false : this.rblInvoiced.SelectedIndex == 2 ? true : (bool?)null;

            Facade.IOrder facOrder = new Facade.Order();
            var dtSource = facOrder.GetOrderListForExport(customerIdentityID, fromDate, toDate, businessTypeIDs, subcontractorIdentityID, isInvoiced, hasSubcontractorInvoice).Tables[0];

            if (dtSource.Rows.Count == 0)
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "NoData", "$(function() { alert('No matching orders were found'); });", true);
                return;
            }

            Func<DataRow, string, string> dateFormatter = (dr, col) => dr.Field<DateTime>(col).ToShortDateString();
            Func<DataRow, string, string> nullableCurrencyFormatter = (dr, col) => string.Format("{0:n2}", dr.Field<decimal?>(col));
            Func<DataRow, string, string> nullableIntFormatter = (dr, col) => string.Format("{0}", dr.Field<int?>(col));

            CsvExport.Export(
                dtSource,
                "Orders.csv",
                new CsvExport.ColumnMappings
                {
                    { "OrderID", "Order ID" },
                    { "CustomerName", "Customer" },
                    { "DeliveryOrderNumber", "Docket number" },
                    { "CustomerOrderNumber", "Load number" },
                    { "JobID", "Run ID", nullableIntFormatter },
                    { "BusinessType", "Business Type" },
                    { "CollectionPoint", "Collection point" },
                    { "CollectionPostcode", "Collection postcode" },
                    { "DeliveryPoint", "Delivery point" },
                    { "DeliveryPostcode", "Delivery postcode" },
                    { "PalletSpaces", "Pallet spaces" },
                    { "Weight", "Weight" },
                    { "SubcontractorRate", "Subcontractor cost", nullableCurrencyFormatter },
                    { "ForeignRateIncludingFuelSurcharge", "Total revenue" },
                    { "ExtrasForeignTotal", "Extras" },
                    { "FuelSurchargeForeignAmount", "Fuel surcharge" },
                    { "ForeignRate", "Revenue" },
                    { "CollectionDateTime", "Collection date", dateFormatter },
                    { "DeliveryDateTime", "Delivery date", dateFormatter },
                    { "SubcontractorName", "Subcontractor" },
                    { "OrderStatus", "Order status" },
                    { "HasSubcontractorInvoice", "Has subcontractor invoice?", (dr, col) => dr.Field<bool>(col) ? "Yes" : "No" },
                });
        }
    }

}