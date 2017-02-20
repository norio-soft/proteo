using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Data.SqlClient;
using Telerik.Web.UI;


namespace Orchestrator.WebUI
{
    public partial class Performance : Orchestrator.Base.BasePage
    {

        //---------------------------------------------------------------------------------------------------//

        protected void Page_Load(object sender, EventArgs e)
        {
            this.cboUsers.ItemsRequested += new RadComboBoxItemsRequestedEventHandler(cboUsers_ItemsRequested);
            this.cboPage.ItemsRequested += new RadComboBoxItemsRequestedEventHandler(cboPage_ItemsRequested);
            this.cboSinglePageUser.ItemsRequested += new RadComboBoxItemsRequestedEventHandler(cboSinglePageUser_ItemsRequested);

            this.PopulatePageCombo();
            this.PopulateUsers();
        }

        //---------------------------------------------------------------------------------------------------//

        void cboSinglePageUser_ItemsRequested(object o, RadComboBoxItemsRequestedEventArgs e)
        {
            this.PopulateUsers();
        }

        //---------------------------------------------------------------------------------------------------//

        void cboPage_ItemsRequested(object o, RadComboBoxItemsRequestedEventArgs e)
        {
            this.PopulatePageCombo();
        }

        //---------------------------------------------------------------------------------------------------//

        void cboUsers_ItemsRequested(object o, RadComboBoxItemsRequestedEventArgs e)
        {
            this.PopulateUsers();
        }

        //---------------------------------------------------------------------------------------------------//

        protected void btnAllPagesRefresh_Click(object sender, EventArgs e)
        {
            DataSet graph = this.GetAllPageDataTable(false);

            this.radAllPagesChart.ChartSeriesCollection.Clear();
            this.radAllPagesChart.Title1.Text = String.Format("Performance of all pages" + Environment.NewLine + "between {0} and {1}"
                , this.dteAllPagesStartDate.Date.ToString("dd/MM/yyyy"), this.dteAllPagesEndDate.Date.ToString("dd/MM/yyyy"));

            ChartSeries chartSeries = this.radAllPagesChart.CreateSeries("Page Processing Time", System.Drawing.Color.Orange, ChartSeriesType.Bar);
            chartSeries.LabelAppearance.RotationAngle = 90;
            chartSeries.LabelAppearance.TextFont = new System.Drawing.Font("Arial", 12, System.Drawing.FontStyle.Bold);
            this.radAllPagesChart.XAxis.AxisStyle = ChartYAxisStyle.Extended;

            this.radAllPagesChart.YAxis.AxisStyle = ChartYAxisStyle.Extended;
            this.radAllPagesChart.YAxis.Label.Text = "Average processing time in seconds";

            foreach (DataRow row in graph.Tables[0].Rows)
            {
                string[] parts = row["VirtualPathString"].ToString().Split(new char[] { '/' });
                chartSeries.AddItem(Convert.ToDouble(row["AverageProcessingTime"]), parts[parts.Length - 1], System.Drawing.Color.Orange);
            }

            this.radAllPagesChart.Visible = true;
        }

        //---------------------------------------------------------------------------------------------------//

        protected void btnSinglePageRefresh_Click(object sender, EventArgs e)
        {
            DataSet graph = this.GetSinglePageDataTable(false);

            this.radSinglePageChart.ChartSeriesCollection.Clear();

            this.radSinglePageChart.Title1.Text = String.Format("Performance of {0}" + Environment.NewLine + "between {1} and {2}", this.cboPage.Text.Trim(),
                this.dteSinglePageStartDate.Date.ToString("dd/MM/yyyy"), this.dteSinglePageEndDate.Date.ToString("dd/MM/yyyy"));

            ChartSeries chartSeries = this.radSinglePageChart.CreateSeries("Page Processing Time", System.Drawing.Color.Orange, ChartSeriesType.Line);
            chartSeries.LabelAppearance.Visible = false;
            chartSeries.LineWidth = 3;

            this.radSinglePageChart.XAxis.ShowLabels = true;
            this.radSinglePageChart.XAxis.AutoScale = false;
            this.radSinglePageChart.XAxis.ShowMarks = false;
            this.radSinglePageChart.XAxis.AxisStyle = ChartYAxisStyle.Normal;
            this.radSinglePageChart.XAxis.Label.Text = "";
            this.radSinglePageChart.XAxis.LabelRotationAngle = 40;
            this.radSinglePageChart.XAxis.Items.Clear();

            this.radSinglePageChart.YAxis.AxisStyle = ChartYAxisStyle.Extended;
            this.radSinglePageChart.YAxis.Label.Text = "Average processing time in seconds";

            foreach (DataRow row in graph.Tables[0].Rows)
            {
                chartSeries.AddItem(Convert.ToDouble(row["AverageProcessingTime"]), row["Date"].ToString(), System.Drawing.Color.Orange);
                ChartAxisItem item = new ChartAxisItem(row["Date"].ToString(), System.Drawing.Color.Black);
                item.Visible = true;
                this.radSinglePageChart.XAxis.Items.AddItem(item);
            }

            this.radSinglePageChart.Visible = true;
        }

        //---------------------------------------------------------------------------------------------------//

        protected void btnExportAll_Click(object sender, EventArgs e)
        {
            DataSet graphData = this.GetAllPageDataTable(true);

            Session["__ExportDS"] = graphData.Tables[0];
            Response.Redirect("/reports/csvexport.aspx?filename=AllPagesForDateRange.csv");
        }

        //---------------------------------------------------------------------------------------------------//

        protected void btnExportSingle_Click(object sender, EventArgs e)
        {
            DataSet graphData = this.GetSinglePageDataTable(true);

            Session["__ExportDS"] = graphData.Tables[0];
            Response.Redirect("/reports/csvexport.aspx?filename=SinglePageForDateRange.csv");
        }

        //---------------------------------------------------------------------------------------------------//

        private DataSet GetAllPageDataTable(bool isExport)
        {
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings[2].ConnectionString);
            SqlCommand command = new SqlCommand((isExport) ? "spAllPagePerformanceExport" : "spAllPagePerformanceForDateRange", connection);
            command.Parameters.Add(new SqlParameter("@FromDate", SqlDbType.DateTime, 8));
            command.Parameters.Add(new SqlParameter("@ToDate", SqlDbType.DateTime, 8));

            if (this.cboUsers.Text != "All Users")
            {
                command.Parameters.Add(new SqlParameter("@User", SqlDbType.VarChar, 60));
                command.Parameters["@User"].Value = this.cboUsers.Text;
            }

            command.Parameters["@FromDate"].Value = this.dteAllPagesStartDate.Date;
            command.Parameters["@ToDate"].Value = this.dteAllPagesEndDate.Date;

            DataSet graph = new DataSet();
            command.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);

            dataAdapter.Fill(graph);

            return graph;
        }

        //---------------------------------------------------------------------------------------------------//

        private DataSet GetSinglePageDataTable(bool isExport)
        {
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings[2].ConnectionString);

            SqlCommand command = new SqlCommand((isExport) ? "spSinglePagePerformanceExport" : "spSinglePagePerformanceForDateRange", connection);
            command.Parameters.Add(new SqlParameter("@FromDate", SqlDbType.DateTime, 8));
            command.Parameters.Add(new SqlParameter("@ToDate", SqlDbType.DateTime, 8));

            if (this.cboSinglePageUser.Text != "All Users")
            {
                command.Parameters.Add(new SqlParameter("@User", SqlDbType.VarChar, 60));
                command.Parameters["@User"].Value = this.cboSinglePageUser.Text;
            }

            command.Parameters.Add(new SqlParameter("@Page", SqlDbType.VarChar, 1000));
            command.Parameters["@FromDate"].Value = this.dteSinglePageStartDate.Date;
            command.Parameters["@ToDate"].Value = this.dteSinglePageEndDate.Date;
            command.Parameters["@Page"].Value = this.cboPage.Text;

            DataSet graph = new DataSet();
            command.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);

            dataAdapter.Fill(graph);
            return graph;
        }

        //---------------------------------------------------------------------------------------------------//

        private void PopulatePageCombo()
        {
            SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings[2].ConnectionString);
            SqlCommand command = new SqlCommand("spGetPageNames", connection);

            DataTable pages = new DataTable();
            command.CommandType = CommandType.StoredProcedure;
            SqlDataAdapter dataAdapter = new SqlDataAdapter(command);

            dataAdapter.Fill(pages);

            this.cboPage.Items.Clear();

            foreach (DataRow row in pages.Rows)
                this.cboPage.Items.Add(new RadComboBoxItem(row[0].ToString()));
        }

        //---------------------------------------------------------------------------------------------------//

        private void PopulateUsers()
        {
            Facade.User user = new Orchestrator.Facade.User();
            DataSet users = user.GetAllUsers();

            this.cboUsers.Items.Clear();
            this.cboSinglePageUser.Items.Clear();

            this.cboUsers.Items.Add(new RadComboBoxItem("All Users"));
            this.cboSinglePageUser.Items.Add(new RadComboBoxItem("All Users"));

            foreach (DataRow row in users.Tables[0].Rows)
            {
                this.cboUsers.Items.Add(new RadComboBoxItem(row["UserName"].ToString()));
                this.cboSinglePageUser.Items.Add(new RadComboBoxItem(row["UserName"].ToString()));
            }
        }

        //---------------------------------------------------------------------------------------------------//
    }
}
