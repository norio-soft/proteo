using System;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.Reports
{
    public partial class SubcontractorTurnover : System.Web.UI.Page
    {
        public bool IncludeTotalsPerClient
        {
            get { return chkIncludeTotalsPerClient.Checked; }
        }

        public bool IncludeTotalsPerMonth
        {
            get { return chkIncludeTotalsPerMonth.Checked; }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                // Default the dates to show the current year to date.
                dteEndDate.SelectedDate = DateTime.Now; // Current date.
                dteStartDate.SelectedDate = new DateTime(dteEndDate.SelectedDate.Value.Year, 1, 1); // Start of the current year.

                // Configure the min and max dates of the date controls.
                dteStartDate.MinDate = (DateTime)SqlDateTime.MinValue;
                dteStartDate.MaxDate = (DateTime)SqlDateTime.MaxValue;
                dteEndDate.MinDate = (DateTime)SqlDateTime.MinValue;
                dteEndDate.MaxDate = (DateTime)SqlDateTime.MaxValue;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            cfvEndAfterStartDate.ServerValidate += new ServerValidateEventHandler(cfvEndAfterStartDate_ServerValidate);
            grdClientTurnover.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdClientTurnover_NeedDataSource);
            grdClientTurnover.PreRender += new EventHandler(grdClientTurnover_PreRender);
            btnRefresh.Click += new EventHandler(btnRefresh_Click);
            btnExport.Click += new EventHandler(btnExport_Click);
        }

        private DataSet GetData()
        {
            DateTime startDate = dteStartDate.SelectedDate.Value;
            DateTime endDate = dteEndDate.SelectedDate.Value;

            Facade.IOrganisation facOrganisation = new Facade.Organisation();
            return facOrganisation.GetSubcontractorCostsPerMonthReport(startDate, endDate, IncludeTotalsPerClient, IncludeTotalsPerMonth);
        }

        /// <summary>
        /// Check that the user has supplied a start date that is less than or equal to the end date.
        /// </summary>
        void cfvEndAfterStartDate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = dteStartDate.SelectedDate.HasValue && dteEndDate.SelectedDate.HasValue && dteStartDate.SelectedDate.Value <= dteEndDate.SelectedDate.Value;
        }

        /// <summary>
        /// Rebind the grid.
        /// </summary>
        void btnRefresh_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                grdClientTurnover.Rebind();
            }
        }

        /// <summary>
        /// Return the report as a csv.
        /// </summary>
        void btnExport_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                // Clear anything that might have been written by the aspx page.
                Response.ClearContent();
                Response.ClearHeaders();

                // Add the appropriate headers
                Response.AddHeader("content-disposition", "attachment; filename=SubcontractorCostsPerMonth.csv");
                // Add the right content type
                Response.ContentType = "application/msexcel";

                TextWriter tw = Response.Output;

                DataSet ds = GetData();

                foreach (DataColumn column in ds.Tables[0].Columns)
                {
                    if (column.ColumnName != "IdentityID")
                    {
                        if (column.Ordinal > 0)
                            tw.Write(",");
                        tw.Write(column.ColumnName);
                    }
                }
                tw.WriteLine();

                foreach (DataRow row in ds.Tables[0].Rows)
                {
                    foreach (DataColumn column in ds.Tables[0].Columns)
                    {
                        if (column.Ordinal > 0)
                            tw.Write(",");

                        if (column.DataType == typeof(string))
                            tw.Write((string)row[column]);
                        else if (column.DataType == typeof(decimal))
                        {
                            if (row[column] == DBNull.Value)
                                tw.Write(default(int).ToString());
                            else
                                tw.Write((decimal)row[column]);
                        }
                    }
                    tw.WriteLine();
                }

                Response.Flush();
                Response.End();
            }
        }

        void grdClientTurnover_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            DataSet ds = GetData();

            // Ensure that data has been returned.
            if (ds != null)
            {
                // Reset the columns.
                grdClientTurnover.MasterTableView.Columns.Clear();

                GridBoundColumn customerColumn = new GridBoundColumn();
                customerColumn.HeaderText = "Subcontractor";
                customerColumn.DataField = "Customer";
                customerColumn.ItemStyle.HorizontalAlign = HorizontalAlign.Left;
                grdClientTurnover.MasterTableView.Columns.Add(customerColumn);

                for (int columnIndex = 2; columnIndex < ds.Tables[0].Columns.Count; columnIndex++)
                {
                    GridBoundColumn column = new GridBoundColumn();
                    column.HeaderText = ds.Tables[0].Columns[columnIndex].ColumnName;
                    column.DataField = ds.Tables[0].Columns[columnIndex].ColumnName;
                    column.DataFormatString = "{0:C}";
                    column.ItemStyle.HorizontalAlign = HorizontalAlign.Right;
                    grdClientTurnover.MasterTableView.Columns.Add(column);
                }

                if (IncludeTotalsPerClient)
                {
                    // Set the last column (the total column to show bold amounts).
                    GridBoundColumn totalColumn = grdClientTurnover.MasterTableView.Columns[grdClientTurnover.MasterTableView.Columns.Count - 1] as GridBoundColumn;
                    if (totalColumn != null)
                        totalColumn.ItemStyle.Font.Bold = true;
                }
            }

            grdClientTurnover.DataSource = ds;
            grdClientTurnover.Visible = ds != null; // Only show the grid if data is present.
        }

        void grdClientTurnover_PreRender(object sender, EventArgs e)
        {
            // Set the data in the last row to be bold.
            if (IncludeTotalsPerMonth && grdClientTurnover.MasterTableView.Items.Count > 0)
            {
                GridDataItem lastRow = grdClientTurnover.MasterTableView.Items[grdClientTurnover.MasterTableView.Items.Count - 1];
                for (int cellIndex = 1; cellIndex < lastRow.Cells.Count; cellIndex++)
                {
                    // Embolden the text.
                    lastRow.Cells[cellIndex].Style["font-weight"] = "bold";
                }
            }
        }
    }
}
