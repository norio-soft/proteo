using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections.Specialized;
using System.Data;

namespace Orchestrator.WebUI.Reports
{
    public partial class RouteDeviation : Orchestrator.Base.BasePage
    {

        //---------------------------------------------------------------------------------------

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        //---------------------------------------------------------------------------------------

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.btnRefresh.Click += new EventHandler(btnRefresh_Click);
            this.btnExport.Click += new EventHandler(btnExport_Click);
            this.VehiclesRadioButton.CheckedChanged += new EventHandler(RadioButton_CheckedChanged);
            this.DriversRadioButton.CheckedChanged += new EventHandler(RadioButton_CheckedChanged);
            this.cboDriver.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboDriver_ItemsRequested);
            this.cboVehicle.ItemsRequested += new Telerik.Web.UI.RadComboBoxItemsRequestedEventHandler(cboVehicle_ItemsRequested);
        }

        //---------------------------------------------------------------------------------------

        protected void cboVehicle_ItemsRequested(object sender, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            cboVehicle.Items.Clear();

            Telerik.Web.UI.RadComboBoxItem rcItem = new Telerik.Web.UI.RadComboBoxItem();
            DataSet ds = null;

            Facade.IResource facResource = new Facade.Resource();
            ds = facResource.GetAllResourcesFiltered(e.Text, eResourceType.Vehicle, false);

            if (ds != null)
            {
                int endOffset = 0;
                DataTable boundResults = BindComboBoxItems(ds.Tables[0], e.NumberOfItems, out endOffset);

                cboVehicle.DataSource = boundResults;
                cboVehicle.DataBind();

                if (boundResults.Rows.Count > 0)
                    e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), boundResults.Rows.Count.ToString());
            }
        }

        //---------------------------------------------------------------------------------------

        protected void cboDriver_ItemsRequested(object sender, Telerik.Web.UI.RadComboBoxItemsRequestedEventArgs e)
        {
            Page.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            cboDriver.Items.Clear();

            Facade.IResource facResource = new Facade.Resource();
            DataSet ds = null;

            ds = facResource.GetAllResourcesFiltered(e.Text, eResourceType.Driver, false);

            int endOffset = 0;
            DataTable boundResults = BindComboBoxItems(ds.Tables[0], e.NumberOfItems, out endOffset);

            cboDriver.DataSource = boundResults;
            cboDriver.DataBind();

            if (boundResults.Rows.Count > 0)
                e.Message = string.Format("Items <b>1</b>-<b>{0}</b> out of <b>{1}</b>", endOffset.ToString(), boundResults.Rows.Count.ToString());
        }

        //---------------------------------------------------------------------------------------

        private DataTable BindComboBoxItems(DataTable dt, int numberOfItems, out int endOffset)
        {
            DataTable boundResults = dt.Clone();

            int itemOffset = numberOfItems;
            endOffset = itemOffset + 15;

            if (endOffset > dt.Rows.Count)
                endOffset = dt.Rows.Count;

            for (int i = itemOffset; i < endOffset; i++)
                boundResults.ImportRow(dt.Rows[i]);

            return boundResults;
        }

        //---------------------------------------------------------------------------------------

        protected void RadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (this.VehiclesRadioButton.Checked)
            {
                this.cboVehicle.Visible = true;
                this.cboDriver.Visible = false;
            }
            else if (this.DriversRadioButton.Checked)
            {
                this.cboDriver.Visible = true;
                this.cboVehicle.Visible = false;
            }
        }

        //---------------------------------------------------------------------------------------

        protected void btnExport_Click(object sender, EventArgs e)
        {
            int resourceId = 0;
            bool driverOrVehicle = false;
            if (this.VehiclesRadioButton.Checked)
            {
                int.TryParse(this.cboVehicle.SelectedValue, out resourceId);
                driverOrVehicle = true;
            }
            else if (this.DriversRadioButton.Checked)
            {
                int.TryParse(this.cboDriver.SelectedValue, out resourceId);
                driverOrVehicle = false;
            }
            
            Facade.IInstruction facInstruction = new Orchestrator.Facade.Instruction();

            DataSet dsRouteDeviation = facInstruction.GetRouteDeviationReport(resourceId, driverOrVehicle, dteStartDate.SelectedDate.Value, dteEndDate.SelectedDate.Value,
                Convert.ToInt32(this.txtDeviationPerc.Value.Value), Convert.ToInt32(this.txtEstimatedDistance.Value.Value));

            Session["__ExportDS"] = dsRouteDeviation.Tables[0];
            Response.Redirect("../reports/csvexport.aspx?filename=RouteDeviationByDateRange" + resourceId.ToString() + ".CSV");
        }

        //---------------------------------------------------------------------------------------

        protected void btnRefresh_Click(object sender, EventArgs e)
        {
            int resourceId = 0;
            bool driverOrVehicle = false;
            if (this.VehiclesRadioButton.Checked)
            {
                int.TryParse(this.cboVehicle.SelectedValue, out resourceId);
                driverOrVehicle = true;
            }
            else if (this.DriversRadioButton.Checked)
            {
                int.TryParse(this.cboDriver.SelectedValue, out resourceId);
                driverOrVehicle = false;
            }

            Facade.IInstruction facInstruction = new Orchestrator.Facade.Instruction();

            DataSet dsRouteDeviation = facInstruction.GetRouteDeviationReport(resourceId, driverOrVehicle, dteStartDate.SelectedDate.Value, 
                dteEndDate.SelectedDate.Value, Convert.ToInt32(this.txtDeviationPerc.Value.Value),Convert.ToInt32(this.txtEstimatedDistance.Value.Value));

            NameValueCollection reportParams = new NameValueCollection();

            reportParams.Add("StartDate", dteStartDate.SelectedDate.Value.ToString("dd/MM/yy"));
            reportParams.Add("EndDate", dteEndDate.SelectedDate.Value.ToString("dd/MM/yy"));

            // Configure the Session variables used to pass data to the report
            Session[Orchestrator.Globals.Constants.ReportTypeSessionVariable] = eReportType.RouteDeviation;
            Session[Orchestrator.Globals.Constants.ReportDataSessionTableVariable] = dsRouteDeviation;
            Session[Orchestrator.Globals.Constants.ReportDataSessionSortVariable] = String.Empty;
            Session[Orchestrator.Globals.Constants.ReportDataMemberSessionVariable] = "Table";
            Session[Orchestrator.Globals.Constants.ReportParamsSessionVariable] = reportParams;

            // Show the user control
            reportViewer.Visible = true;
        }

        //---------------------------------------------------------------------------------------
    }
}