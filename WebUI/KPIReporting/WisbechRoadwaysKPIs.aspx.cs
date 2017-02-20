using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Text;
using Orchestrator.SystemFramework;

namespace Orchestrator.WebUI.KPIReporting
{
    public partial class WisbechRoadwaysKPIs : System.Web.UI.Page
    {

        #region Page Event Handlers

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            cvBusinessTypes.ServerValidate += new ServerValidateEventHandler(cvBusinessTypes_ServerValidate);
            btnProductReturns.Click += new EventHandler(btnProductReturns_Click);
            btnTonnesToCustomer.Click += new EventHandler(btnTonnesToCustomer_Click);
            btnDeliveryPerformance.Click += new EventHandler(btnDeliveryPerformance_Click);
            btnOrderRefusals.Click += new EventHandler(btnOrderRefusals_Click);
            btnOutstandingPods.Click += new EventHandler(btnOutstandingPods_Click);
            btnFactoryClearance.Click += new EventHandler(btnFactoryClearance_Click);
            btnCollectionPerformance.Click += new EventHandler(btnCollectionPerformance_Click);
            btnCapacityUsage.Click += new EventHandler(btnCapacityUsage_Click);
        }

        #endregion Page Event Handlers

        #region Private Methods

        private string PercentageFormatter(DataRow dr, string col)
        {
            return dr.Field<decimal>(col).ToString("p2");
        }

        private string DateFormatter(DataRow dr, string col)
        {
            return dr.Field<DateTime>(col).ToShortDateString();
        }

        #endregion Private Methods

        #region Control Event Handlers

        private void cvBusinessTypes_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = chkTTCBusinessType.SelectedBusinessTypeIDs.Any();
        }

        private void btnProductReturns_Click(object sender, EventArgs e)
        {
            DateTime startDate = dtePRStartDate.SelectedDate.Value.Date;
            DateTime endDate = dtePREndDate.SelectedDate.Value.Date.Add(new TimeSpan(23, 59, 59));

            Facade.IKPI facKPI = new Orchestrator.Facade.KPI();
            DataSet dsSource = facKPI.ProductReturns(startDate, endDate);

            CsvExport.Export(
                dsSource.Tables[0],
                "ProductReturns.csv",
                new CsvExport.ColumnMappings
                {
                    { "OriginalCollectionPoint", "Despatch Point" },
                    { "OriginalDeliveryOrderNumber", "Docket Number" },
                    { "ReturnDeliveryDateTime", "Return Date", this.DateFormatter },
                    { "RefusalType", "Refusal Type" },
                    { "RefusalReceiptNumber", "Refusal Receipt Number" },
                    { "RefusalNotes", "Refusal Notes" },
                    { "ProductCode", "Product Code" },
                    { "ProductName", "Product Name" },
                    { "PackSize", "Pack Size" },
                    { "QuantityRefused", "Quantity Refused" },
                    { "ReturnDeliveryPoint", "Return To" },
                    { "ReturnOrderRate", "ReturnOrderRate" },
                });
        }

        private void btnTonnesToCustomer_Click(object sender, EventArgs e)
        {
            DateTime startDate = dteTTCStartDate.SelectedDate.Value.Date;
            DateTime endDate = dteTTCEndDate.SelectedDate.Value.Date.Add(new TimeSpan(23, 59, 59));

            IEnumerable<int> businessTypeIDs = chkTTCBusinessType.AllBusinessTypesSelected ? null : chkTTCBusinessType.SelectedBusinessTypeIDs;

            Facade.IKPI facKPI = new Facade.KPI();
            DataSet dsSource = facKPI.TonnesToCustomer(businessTypeIDs, startDate, endDate);

            CsvExport.Export(
                dsSource.Tables[0],
                "TonnesToCustomer.csv",
                new CsvExport.ColumnMappings
                {
                    { "Customer", "Customer" },
                    { "TotalTonnes", "Total Tonnes" },
                    { "Loads", "Loads" },
                    { "Drops", "Drops" },
                    { "AverageLoadSize", "Average Load Size" },
                    { "AverageDropSize", "Average Drop Size" },
                    { "RouteRate", "Route Rate" },
                    { "AdditionalCollsCost", "Additional Collections Cost" },
                    { "AdditionalDropsCost", "Additional Drops Cost" },
                    { "FuelSurcharge", "Fuel Surcharge" },
                    { "Demurrage", "Demurrage" },
                    { "BankHoliday", "Bank Holiday" },
                    { "TotalCost", "Total Cost" },
                    { "TotalCostPerTonne", "Total Cost Per Tonne" },
                });
        }

        private void btnDeliveryPerformance_Click(object sender, EventArgs e)
        {
            DateTime startDate = dteDPStartDate.SelectedDate.Value.Date;
            DateTime endDate = dteDPEndDate.SelectedDate.Value.Date.Add(new TimeSpan(23, 59, 59));

            Facade.IKPI facKPI = new Facade.KPI();
            DataSet dsSource = facKPI.CollectDropPerformance(startDate, endDate, eInstructionType.Drop);

            CsvExport.Export(
                dsSource.Tables[0],
                "DeliveryPerformance.csv",
                new CsvExport.ColumnMappings
                {
                    { "MemberName", "Member" },
                    { "TargetOnTimeProportion", "Target On-Time", this.PercentageFormatter },
                    { "OnTimeProportion", "Actual On-Time", this.PercentageFormatter },
                });
        }

        private void btnOrderRefusals_Click(object sender, EventArgs e)
        {
            DateTime startDate = dteORStartDate.SelectedDate.Value.Date;
            DateTime endDate = dteOREndDate.SelectedDate.Value.Date.Add(new TimeSpan(23, 59, 59));

            Facade.IKPI facKPI = new Facade.KPI();
            DataSet dsSource = facKPI.OrderRefusals(startDate, endDate);

            CsvExport.Export(
                dsSource.Tables[0],
                "OrderRefusals.csv",
                new CsvExport.ColumnMappings
                {
                    { "Total", "Total" },
                    { "Haulier", "Haulier" },
                    { "Premier", "Premier" },
                    { "Customer", "Customer" },
                    { "Other", "Other" },
                });
        }

        private void btnOutstandingPods_Click(object sender, EventArgs e)
        {
            DateTime startDate = dtePODStartDate.SelectedDate.Value.Date;
            DateTime endDate = dtePODEndDate.SelectedDate.Value.Date.Add(new TimeSpan(23, 59, 59));
            bool isDetailReport = rblPODReportType.SelectedIndex == 1;

            Facade.IKPI facKPI = new Facade.KPI();

            if (isDetailReport)
            {
                DataSet dsSource = facKPI.OutstandingPODDetail(startDate, endDate);

                CsvExport.Export(
                    dsSource.Tables[0],
                    "OutstandingPODDetail.csv",
                    new CsvExport.ColumnMappings
                    {
                        { "Client", "Client" },
                        { "Member", "Member" },
                        { "OrderID", "Order ID" },
                        { "BusinessType", "Business Type" },
                        { "DeliveryOrderNumber", "Docket Number" },
                        { "CustomerOrderNumber", "Load Number" },
                        { "DateTimeDelivered", "Date Delivered", this.DateFormatter },
                        { "DeliveredInDateRange", "In Date Range?", (dr, col) => dr.Field<bool>(col) ? "Yes" : "No" },
                    });
            }
            else
            {
                DataSet dsSource = facKPI.OutstandingPODSummary(startDate, endDate);

                CsvExport.Export(
                    dsSource.Tables[0],
                    "OutstandingPODSummary.csv",
                    new CsvExport.ColumnMappings
                    {
                        { "Client", "Client" },
                        { "Member", "Member" },
                        { "BusinessType", "Business Type" },
                        { "CountDeliveredInDateRange", "In Date Range" },
                        { "CountDeliveredPriorToDateRange", "Prior To Date Range" },
                    });
            }
        }

        private void btnFactoryClearance_Click(object sender, EventArgs e)
        {
            DateTime startDate = dteFCStartDate.SelectedDate.Value.Date;
            DateTime endDate = dteFCEndDate.SelectedDate.Value.Date.Add(new TimeSpan(23, 59, 59));

            Facade.IKPI facKPI = new Orchestrator.Facade.KPI();
            DataSet dsSource = facKPI.FactoryClearance(startDate, endDate);

            CsvExport.Export(
                dsSource.Tables[0],
                "FactoryClearance.csv",
                new CsvExport.ColumnMappings
                {
                    { "MemberName", "Member" },
                    { "CollectionPoint", "Collection Point" },
                    { "LoadCount", "Load Count" },
                });
        }

        private void btnCollectionPerformance_Click(object sender, EventArgs e)
        {
            DateTime startDate = dteCPStartDate.SelectedDate.Value.Date;
            DateTime endDate = dteCPEndDate.SelectedDate.Value.Date.Add(new TimeSpan(23, 59, 59));

            Facade.IKPI facKPI = new Facade.KPI();
            DataSet dsSource = facKPI.CollectDropPerformance(startDate, endDate, eInstructionType.Load);

            CsvExport.Export(
                dsSource.Tables[0],
                "CollectionPerformance.csv",
                new CsvExport.ColumnMappings
                {
                    { "MemberName", "Member" },
                    { "TargetOnTimeProportion", "Target On-Time", this.PercentageFormatter },
                    { "OnTimeProportion", "Actual On-Time", this.PercentageFormatter },
                });
        }

        private void btnCapacityUsage_Click(object sender, EventArgs e)
        {
            DateTime startDate = dteCUStartDate.SelectedDate.Value.Date;
            DateTime endDate = dteCUEndDate.SelectedDate.Value.Date.Add(new TimeSpan(23, 59, 59));

            Facade.IKPI facKPI = new Facade.KPI();
            DataSet dsSource = facKPI.CapacityUsage(startDate, endDate);

            CsvExport.Export(
                dsSource.Tables[0],
                "CapacityUsage.csv",
                new CsvExport.ColumnMappings
                {
                    { "OrganisationName", "Client" },
                    { "NumberOfLoads", "Number of Loads" },
                    { "CapacityUsage", "Weight Percentage of Capacity", this.PercentageFormatter },
                });
        }

        #endregion Control Event Handlers

    }
}
