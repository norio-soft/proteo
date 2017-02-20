using System;
using System.Linq;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Collections.Generic;
using Telerik.Web.UI;
using System.Web;

namespace Orchestrator.WebUI.Organisation
{
    public partial class WorkForClientNew : System.Web.UI.Page
    {

        #region Initialization

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitializeControls();
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            grdClientRevenue.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdClientRevenue_NeedDataSource);
            grdClientRevenue.PreRender += new EventHandler(grdClientRevenue_PreRender);
            btnRefresh.Click += new EventHandler(btnRefresh_Click);
            btnExport.Click += new EventHandler(btnExport_Click);
        }

        #endregion

        #region private properties

        private DateTime StartDate
        {
            get
            {
                DateTime endDate;

                switch (DateRange)
                {
                    case ClientRevenueDateRange.Year:
                        endDate = DateTime.Today.AddYears(-1);
                        break;
                    case ClientRevenueDateRange.Month:
                        endDate = DateTime.Today.AddDays(-31);
                        break;
                    case ClientRevenueDateRange.Week:
                        endDate = DateTime.Today.AddDays(-7);
                        break;
                    case ClientRevenueDateRange.Day:
                        endDate = DateTime.Today.AddDays(-1);
                        break;
                    default:
                        endDate = DateTime.Today.AddYears(-1);
                        break;
                }

                return endDate;
            }
        }

        private DateTime EndDate
        {
            get
            {
                return DateTime.Today.AddMilliseconds(-1);
            }
        }

        private ClientRevenueDateRange DateRange
        {
            get
            {
                return (ClientRevenueDateRange)Enum.Parse(typeof(ClientRevenueDateRange), lstDateRange.SelectedValue);
            }
        }

        private string HeadingText
        {
            get
            {
                return string.Format("Use this screen to retrieve a report showing revenue per client over the specified date range. (Currently showing {0})", this.RangeText);
            }
        }

        private string RangeText
        {
            get
            {
                string rangeText;

                switch (DateRange)
                {
                    case ClientRevenueDateRange.Year:
                    case ClientRevenueDateRange.Month:
                    case ClientRevenueDateRange.Week:
                        rangeText =string.Format("{0} to {1}", StartDate.ToString("d"), EndDate.ToString("d"));
                        break;
                    case ClientRevenueDateRange.Day:
                        rangeText = EndDate.ToString("d");
                        break;
                    default:
                        rangeText = string.Format("{0} to {1}", StartDate.ToString("d"), EndDate.ToString("d"));
                        break;
                }

                return rangeText;
            }
        }

        #endregion

        #region Private Methods

        private void InitializeControls()
        {

            var enumValues = (ClientRevenueDateRange[]) Enum.GetValues(typeof(ClientRevenueDateRange));
            var listItems = enumValues.Select(e => new DropDownListItem(e.GetAttribute<DescriptionAttribute>().Description, e.ToString()));
            foreach (var item in listItems)
	        {
                lstDateRange.Items.Add(item);
	        } 

            lstDateRange.SelectedValue = ClientRevenueDateRange.Week.ToString();

            cboClient.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboClient_ItemsRequested);

        }

        private DataSet GetData()
        {
            Facade.IOrganisation facOrganisation = new Facade.Organisation();
            Func<DateTime, string> dateFormatter;
            DataSet dataSet;

            int clientId = (string.IsNullOrEmpty(cboClient.SelectedValue)) ? -1 : int.Parse(cboClient.SelectedValue);

            switch (DateRange)
            {
                case ClientRevenueDateRange.Year:
                    dateFormatter = (dateTime) => string.Format("{0} {1}", dateTime.ToString("MMM"), dateTime.Year);
                    dataSet = facOrganisation.GetClientWorkPerMonthReport(this.StartDate, this.EndDate, true, true, dateFormatter, clientId);
                    break;
                case ClientRevenueDateRange.Month:
                    dateFormatter = (dateTime) => dateTime.ToString("M");
                    dataSet = facOrganisation.GetClientWorkPerDayReport(this.StartDate, this.EndDate, true, true, dateFormatter, clientId);
                    break;
                case ClientRevenueDateRange.Week:
                    dateFormatter = (dateTime) => dateTime.DayOfWeek.ToString();
                    dataSet = facOrganisation.GetClientWorkPerDayReport(this.StartDate, this.EndDate, true, true, dateFormatter, clientId);
                    break;
                case ClientRevenueDateRange.Day:
                    dateFormatter = (dateTime) => "Yesterday";
                    dataSet = facOrganisation.GetClientWorkPerDayReport(this.StartDate, this.EndDate, true, true, dateFormatter, clientId);
                    var columnToRemove = dataSet.Tables[0].Columns.Cast<DataColumn>().Single(c => c.ColumnName == "Yesterday");
                    dataSet.Tables[0].Columns.Remove(columnToRemove);
                    break;
                default:
                    dateFormatter = (dateTime) => string.Format("{0} {1}", dateTime.ToString("MMM"), dateTime.Year);
                    dataSet = facOrganisation.GetClientWorkPerMonthReport(this.StartDate, this.EndDate, true, true, dateFormatter, clientId);
                    break;
            }


            return dataSet;

        }

        #endregion

        #region Event Handlers

        /// <summary>
        /// Rebind the grid.
        /// </summary>
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                grdClientRevenue.Rebind();
            }
        }

        /// <summary>
        /// Return the report as a csv.
        /// </summary>
        private void btnExport_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                // Clear anything that might have been written by the aspx page.
                Response.ClearContent();
                Response.ClearHeaders();

                // Add the appropriate headers
                string filename = string.Format("Client Revenue {0}.csv", RangeText).Replace('/', '-');
                Response.AddHeader("content-disposition", string.Format("attachment; filename={0}", filename));
                // Add the right content type
                Response.ContentType = "application/msexcel";

                DataSet ds = GetData();
                var columnsToIgnore = new List<string>() { "IdentityID" };
                string csv = ds.Tables[0].ToCSV(columnsToIgnore);

                Response.Write(csv);

                Response.Flush();
                Response.End();
            }
        }

        private void grdClientRevenue_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            DataSet ds = GetData();

            // Ensure that data has been returned.
            if (ds != null)
            {
                // if we only have the totals row (i.e. no client revenue data was returned) then remove
                // totals row and let the grid display its "No revenue data for the specified date range." message
                if (ds.Tables[0].Rows.Count == 1)
                {
                    ds.Tables[0].Rows.Remove(ds.Tables[0].Rows[0]);
                }

                // Reset the columns.
                grdClientRevenue.MasterTableView.Columns.Clear();

                GridHyperLinkColumn customerColumn = new GridHyperLinkColumn();
                customerColumn.HeaderText = "Customer";
                customerColumn.DataTextField = "Customer";
                customerColumn.DataNavigateUrlFormatString = string.Format("workForClient.aspx?mode=client&csid=FS21eE&customerID={0}&startDate={1}&endDate={2}",
                                                                            "{0}",
                                                                            HttpUtility.UrlEncode(StartDate.ToString("s", System.Globalization.CultureInfo.InvariantCulture), System.Text.Encoding.UTF8),
                                                                            HttpUtility.UrlEncode(EndDate.ToString("s", System.Globalization.CultureInfo.InvariantCulture), System.Text.Encoding.UTF8));
                customerColumn.DataNavigateUrlFields = new string[] {"IdentityID"};
                customerColumn.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                grdClientRevenue.MasterTableView.Columns.Add(customerColumn);

                for (int columnIndex = 2; columnIndex < ds.Tables[0].Columns.Count; columnIndex++)
                {
                    GridBoundColumn column = new GridBoundColumn();
                    column.HeaderText = ds.Tables[0].Columns[columnIndex].ColumnName;
                    column.DataField = ds.Tables[0].Columns[columnIndex].ColumnName;
                    column.DataFormatString = "{0:C}";
                    column.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                    grdClientRevenue.MasterTableView.Columns.Add(column);
                }

                // Set the last column (the total column to show bold amounts).
                GridBoundColumn totalColumn = grdClientRevenue.MasterTableView.Columns[grdClientRevenue.MasterTableView.Columns.Count - 1] as GridBoundColumn;
                if (totalColumn != null)
                    totalColumn.ItemStyle.Font.Bold = true;

            }

            grdClientRevenue.DataSource = ds;
            grdClientRevenue.Visible = ds != null; // Only show the grid if data is present.

            heading.InnerText = this.HeadingText;
        }

        private void grdClientRevenue_PreRender(object sender, EventArgs e)
        {
            // Set the data in the last row to be bold.
            if (grdClientRevenue.MasterTableView.Items.Count > 0)
            {
                GridDataItem lastRow = grdClientRevenue.MasterTableView.Items[grdClientRevenue.MasterTableView.Items.Count - 1];
                for (int cellIndex = 1; cellIndex < lastRow.Cells.Count; cellIndex++)
                {
                    // Embolden the text.
                    lastRow.Cells[cellIndex].Style["font-weight"] = "bold";
                }
            }

        }

        void cboClient_ItemsRequested(object o, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            cboClient.Items.Clear();

            Orchestrator.Facade.IReferenceData facRefData = new Orchestrator.Facade.ReferenceData();
            DataSet ds = facRefData.GetAllOrganisationsFiltered(e.Text, eOrganisationType.Client, false);

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
    }

    #endregion


    public enum ClientRevenueDateRange
    {

        [Description("Day (Yesterday)")]
        Day,
        [Description("Week (Last 7 days)")]
        Week,
        [Description("Month (Last 31 days)")]
        Month,
        [Description("Year (Last 12 months)")]
        Year
    }

}
