using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;

namespace Orchestrator.WebUI.Organisation
{
    public partial class WorkForDriver : Orchestrator.Base.BasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                this.PopulateJobStatusList();

                GetData();

                foreach (ListItem li in cblJobStatus.Items)
                {
                    li.Selected = true;
                }
            }
            else
                BindJavaScript();

            this.Form.DefaultButton = btnGetData.UniqueID;
        }

        private const string c_AllWorkForDriver_VS = "vs_AllWorkForDriver";

        protected override PageStatePersister PageStatePersister
        {
            get
            {
                return new HiddenFieldPageStatePersister(this);
            }
        }

        public DataSet AllWorkForDriver
        {
            get
            {
                if (ViewState[c_AllWorkForDriver_VS] == null)
                {
                    var selectedDrivers = hidSelectedDriversValues.Value;
                    Facade.IOrganisation facOrg = new Facade.Organisation();
                    if ((selectedDrivers.Length > 0 || cbSelectedAllDrivers.Checked) && dteDateFrom.SelectedDate.HasValue && dteDateTo.SelectedDate.HasValue)
                    {
                        string csvJobStatus = GetCsvJobStatus();

                        if (cbSelectedAllDrivers.Checked)
                            selectedDrivers = String.Empty;

                        AllWorkForDriver = facOrg.GetAllWorkForDrivers(selectedDrivers, dteDateFrom.SelectedDate.Value, dteDateTo.SelectedDate.Value, csvJobStatus);
                    }
                    else
                        ViewState[c_AllWorkForDriver_VS] = null;

                }

                return (DataSet)ViewState[c_AllWorkForDriver_VS];
            }
            set { ViewState[c_AllWorkForDriver_VS] = value; }
        }



        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            this.repDrivers.ItemDataBound += new RepeaterItemEventHandler(repDrivers_ItemDataBound);

            this.cboDriver.ItemsRequested += cboDriver_ItemsRequested;

            //this.cblJobStatus.PreRender += cblJobStatus_PreRender;
            this.cblJobStatus.DataBound += cblJobStatus_DataBound;

            this.grdSummary.NeedDataSource += grdSummary_NeedDataSource;
            this.grdSummary.ItemDataBound += grdSummary_ItemDataBound;

            this.btnGetData.Click += btnGetData_Click;
            btnExportToCSV.Click += new EventHandler(btnExportToCSV_Click);
        }

        private void btnExportToCSV_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                var selectedDrivers = hidSelectedDriversValues.Value;
                if (dteDateFrom.SelectedDate.HasValue && dteDateTo.SelectedDate.HasValue && (selectedDrivers.Length > 0 || cbSelectedAllDrivers.Checked))
                {

                    if (cbSelectedAllDrivers.Checked)
                        selectedDrivers = String.Empty;

                    string csvJobStatus = GetCsvJobStatus();

                    Facade.IOrganisation facOrg = new Facade.Organisation();
                    AllWorkForDriver = facOrg.GetAllWorkForDrivers(selectedDrivers, dteDateFrom.SelectedDate.Value, dteDateTo.SelectedDate.Value, csvJobStatus);

                    DataTable original = AllWorkForDriver.Tables[0];
                    DataTable newTable = new DataTable();

                    newTable.Columns.Add("Driver Name");
                    newTable.Columns.Add("Order ID");
                    newTable.Columns.Add("Run ID");
                    newTable.Columns.Add("Customer Order Number");
                    newTable.Columns.Add("Delivery Order Number");
                    newTable.Columns.Add("Client");
                    newTable.Columns.Add("Rate");
                    newTable.Columns.Add("Extras");
                    newTable.Columns.Add("Collection Point");
                    newTable.Columns.Add("Delivery Point");
                    newTable.Columns.Add("Post Town");
                    newTable.Columns.Add("Delivery Date");
                    newTable.Columns.Add("No Pallets");
                    newTable.Columns.Add("Pallet Spaces");
                    newTable.Columns.Add("Weight");
                    newTable.Columns.Add("Trailer");
                    newTable.Columns.Add("Has POD");


                    for (int row = 0; row <= original.Rows.Count - 1; row++)
                    {
                        DataRow newRow = newTable.NewRow();

                        newRow[0] = original.Rows[row][10];
                        newRow[1] = original.Rows[row][2];
                        newRow[2] = original.Rows[row][0];
                        newRow[3] = original.Rows[row][5];
                        newRow[4] = original.Rows[row][6];
                        newRow[5] = original.Rows[row][7];
                        newRow[6] = original.Rows[row][8];
                        newRow[7] = original.Rows[row][12] + " extras worth " + original.Rows[row][11];
                        newRow[8] = original.Rows[row][14];
                        newRow[9] = original.Rows[row][16];
                        newRow[10] = original.Rows[row][17];
                        newRow[11] = original.Rows[row][19];
                        newRow[12] = original.Rows[row][21];
                        newRow[13] = original.Rows[row][22];
                        newRow[14] = original.Rows[row][23];
                        newRow[15] = original.Rows[row][27];
                        newRow[16] = original.Rows[row][24];
                        newTable.Rows.Add(newRow);
                    }

                    Session["__ExportDS"] = newTable;

                    Server.Transfer("../Reports/csvexport.aspx?filename=AllWorkForDriverExport.csv");
                }
            }
        }

        private void PopulateJobStatusList()
        {
            cblJobStatus.DataSource = Enum.GetNames(typeof(eJobState));
            cblJobStatus.DataBind();

            BindJavaScript();
        }

        private void BindJavaScript()
        { 
            if (cblJobStatus.Items.FindByText("All") == null)
            {
                cblJobStatus.DataSource = Enum.GetNames(typeof(eJobState));
                cblJobStatus.DataBind();
            }

            cblJobStatus.Items.FindByText("All").Attributes.Add("onclick", "javascript:CheckSelectAll(this)");

            foreach (ListItem item in cblJobStatus.Items)
            {
                if (item.Text != "All")
                    item.Attributes.Add("onclick", "javascript:CheckSelectBox(this)");

                item.Text = Regex.Replace(item.Text, "([a-z])([A-Z])", "$1 $2");
            }
        }


        #region Grid Events

        void grdSummary_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridFooterItem)
            {

                Label lblTotalWeight = (Label)e.Item.FindControl("lblTotalWeight");
                Label lblExtrasBreakdown = (Label)e.Item.FindControl("lblExtrasBreakdown");
                Label lblTotalOfFuelSurchargeAmount = (Label)e.Item.FindControl("lblTotalOfFuelSurchargeAmount");
                Label lblTotalCountOfOrders = (Label)e.Item.FindControl("lblTotalCountOfOrders");
                Label lblTotalCountOfDeliveryRuns = (Label)e.Item.FindControl("lblTotalCountOfDeliveryRuns");
                Label lblTotalRate = (Label)e.Item.FindControl("lblTotalRate");


                int totalRuns = 0;
                int totalOrders = 0;
                decimal totalRate = 0;
                decimal totalFuelSurcharge = 0;
                int extraCount = 0;
                decimal extraAmount = 0;
                decimal totalWeight = 0;


                foreach (DataRowView row in AllWorkForDriver.Tables[2].DefaultView)
                {
                    totalRuns += (int)row["CountOfRuns"];
                    totalOrders += (int)row["CountOfOrders"];
                    totalRate += (decimal)row["TotalRate"];
                    extraCount += (int)row["NumberOfExtras"];
                    extraAmount += (decimal)row["TotalValueOfExtras"];
                    totalFuelSurcharge += (decimal)row["TotalFuelSurcharge"];
                    totalWeight += (decimal)row["totalWeight"];
                }

                lblTotalRate.Text = totalRate.ToString("C");
                lblExtrasBreakdown.Text = string.Format("{0} extras worth {1}", extraCount, extraAmount.ToString("C"));
                lblTotalOfFuelSurchargeAmount.Text = totalFuelSurcharge.ToString("C");
                lblTotalCountOfOrders.Text = totalOrders.ToString();
                lblTotalCountOfDeliveryRuns.Text = totalRuns.ToString();
                lblTotalWeight.Text = totalWeight.ToString("F0");
            }
        }

        void grdSummary_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            if (AllWorkForDriver != null)
            {
                if (AllWorkForDriver.Tables[2].DefaultView.Count > 0)
                {
                    this.grdSummary.DataSource = AllWorkForDriver.Tables[2];
                    tblSummary.Style["display"] = "";
                }
                else
                    tblSummary.Style["display"] = "none";
            }
        }

        void repDrivers_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {

                Label lblTitle = (Label)e.Item.FindControl("lblTitle");
                HiddenField hidDriverID = (HiddenField)e.Item.FindControl("hidDriverID");


                DataRow dr = (DataRow)(e.Item.DataItem);

                hidDriverID.Value = dr["ResourceId"].ToString();
                lblTitle.Text = dr["Fullname"].ToString();
            }
        }

        protected void grd_NeedDataSource(object source, GridNeedDataSourceEventArgs e)
        {
            RadGrid grdDrivers = (RadGrid)source;
            HiddenField hidDriverID = (HiddenField)grdDrivers.Parent.FindControl("hidDriverID");

            RadGrid grid = (RadGrid)source;

            if (hidDriverID != null && AllWorkForDriver != null)
            {
                string driverID = string.Empty;

                if (hidDriverID.Value == string.Empty)
                {
                    driverID = ((System.Data.DataRow)((RepeaterItem)grdDrivers.Parent).DataItem)["ResourceId"].ToString();
                    hidDriverID.Value = driverID;
                }
                else
                    driverID = hidDriverID.Value;

                AllWorkForDriver.Tables[0].DefaultView.RowFilter = "DriverID = " + driverID;

                grdDrivers.DataSource = AllWorkForDriver.Tables[0].DefaultView;
            }
        }

        void btnGetData_Click(object sender, EventArgs e)
        {
            GetData();
        }

        protected void grd_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (AllWorkForDriver != null)
            {
                if (e.Item is GridDataItem)
                {
                    DataRowView drv = (DataRowView)(e.Item.DataItem);
                    HyperLink lnkPOD = (HyperLink)e.Item.FindControl("lnkPOD");
                    bool hasPod = (bool)drv["HasPOD"];

                    if (hasPod)
                    {
                        lnkPOD.ForeColor = System.Drawing.Color.Blue;
                        lnkPOD.NavigateUrl = drv.Row["ScannedFormPDF"].ToString().Trim();
                        lnkPOD.Text = "Yes";
                    }
                    else
                    {
                        var orderStatus = (eOrderStatus)drv["OrderStatusID"];

                        if (orderStatus == eOrderStatus.Delivered || orderStatus == eOrderStatus.Invoiced)
                        {
                            int orderID = (int)drv["OrderID"];
                            lnkPOD.Text = "No";
                            lnkPOD.ForeColor = System.Drawing.Color.Blue;
                            lnkPOD.NavigateUrl = @"javascript:OpenPODWindow(" + orderID + ")";
                        }
                        else
                        {
                            lnkPOD.Text = "N/A";
                            lnkPOD.ToolTip = "Not Delivered";
                            lnkPOD.Style.Add("text-decoration", "none");
                        }
                    }

                    e.Item.Style["background-color"] = Orchestrator.WebUI.Utilities.GetJobStateColourForHTML((eJobState)drv["JobStateID"]);

                }
                else if (e.Item is GridFooterItem)
                {
                    HiddenField hidDriverID = (HiddenField)e.Item.Parent.DataItemContainer.FindControl("hidDriverID");
                    AllWorkForDriver.Tables[1].DefaultView.RowFilter = "DriverID = " + hidDriverID.Value;

                    Label lblTotalRate = (Label)e.Item.FindControl("lblTotalRate");
                    Label lblTotalExtras = (Label)e.Item.FindControl("lblTotalExtras");

                    decimal totalRate = 0;
                    int extraCount = 0;
                    decimal extraAmount = 0;

                    foreach (DataRowView row in AllWorkForDriver.Tables[1].DefaultView)
                    {
                        totalRate += (decimal)row["TotalRate"];
                        extraCount += (int)row["CountExtras"];
                        extraAmount += (decimal)row["TotalExtras"];

                    }

                    lblTotalRate.Text = totalRate.ToString("C");
                    lblTotalExtras.Text = string.Format("{0} extras worth {1}", extraCount, extraAmount.ToString("C"));
                }
            }
        }

        void cboDriver_ItemsRequested(object sender, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            LoadDrivers(e);
        }

        void cblJobStatus_DataBound(object sender, EventArgs e)
        {
            var allItem = new ListItem("All", "-1");
            allItem.Selected = true;

            cblJobStatus.Items.Add(allItem);
        }

        #endregion Grid Events

        #region Private Methods

        private string GetCsvJobStatus()
        {
            var jobStatusItems = cblJobStatus.Items.Cast<ListItem>();
            var allJobStatusesSelected = jobStatusItems.Any(i => i.Selected && i.Text == "All");

            if (!allJobStatusesSelected && jobStatusItems.Any(o => o.Selected))
                jobStatusItems = jobStatusItems.Where(o => o.Selected);

            string csvJobStatus = string.Join(", ", jobStatusItems.Select(o => (int)((eJobState)Enum.Parse(typeof(eJobState), o.Value))));
            return csvJobStatus;
        }

        private void GetData()
        {
            var selectedDrivers = hidSelectedDriversValues.Value;
            if (dteDateFrom.SelectedDate.HasValue && dteDateTo.SelectedDate.HasValue && (selectedDrivers.Length > 0 || cbSelectedAllDrivers.Checked))
            {

                if (cbSelectedAllDrivers.Checked)
                    selectedDrivers = String.Empty;

                string csvJobStatus = GetCsvJobStatus();

                Facade.IOrganisation facOrg = new Facade.Organisation();
                AllWorkForDriver = facOrg.GetAllWorkForDrivers(selectedDrivers, dteDateFrom.SelectedDate.Value, dteDateTo.SelectedDate.Value, csvJobStatus);
                BindRepeater();
            }
        }

        private void LoadDrivers(Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            cboDriver.Items.Clear();

            Facade.IReferenceData facDriver = new Facade.ReferenceData();

            DataSet ds = facDriver.GetAllDriversFiltered(e.Text, false);

            cboDriver.DataSource = ds;
            cboDriver.DataBind();
        }

        private void BindRepeater()
        {
            Facade.IBusinessType facBusinessType = new Facade.BusinessType();
            Facade.IOrganisation facOrganisation = new Facade.Organisation();

            var selectedDrivers = hidSelectedDriversValues.Value;
            var driverList = selectedDrivers.Split(',').ToList();

            Facade.IDriver facDriver = new Facade.Resource();

            DataSet ds = facDriver.GetAllDrivers(false);

            var  drivers = from driver in ds.Tables[0].AsEnumerable()
                              select driver;

            if (!cbSelectedAllDrivers.Checked)
            {
                drivers = from driver in ds.Tables[0].AsEnumerable()
                          where driverList.Contains(driver.Field<int>("ResourceId").ToString())
                          select driver;
            }

            repDrivers.DataSource = drivers;
            repDrivers.DataBind();

            grdSummary.Rebind();

            AllWorkForDriver = null;

        }

        #endregion Private Methods

    }
}