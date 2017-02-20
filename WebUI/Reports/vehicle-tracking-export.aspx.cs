using Orchestrator.Extensions;
using System;
using System.Data;
using System.Web.UI;
using System.Web.UI.WebControls;


namespace Orchestrator.WebUI.Reports
{
    public partial class vehicle_tracking_export : Base.BasePage
    {

        Facade.IVehicle facVehicle;

        protected void Page_Load(object sender, EventArgs e)
        {
            Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage);
            facVehicle = new Facade.Resource();

            if (!IsPostBack)
            {
                dteStartDate.SelectedDate = DateTime.UtcNow.Subtract(new TimeSpan(1, 0, 0, 0));
                dteEndDate.SelectedDate = DateTime.Today;
                LoadVehicles();

            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            this.btnExportToCSV.Click += new EventHandler(btnExportToCSV_Click);
        }

        void LoadVehicles() {
            ddlVehicle.DataSource = facVehicle.GetAllVehicles();
            ddlVehicle.DataTextField = "RegNo";
            ddlVehicle.DataValueField = "ResourceId";
            ddlVehicle.DataBind();
            ddlVehicle.Items.Insert(0, new ListItem("--- [ Please Select ] ---", "-1"));
        }


        #region Event Handlers
        private void btnExportToCSV_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                var vehicleId = (int)ddlVehicle.SelectedValue.ToInt32(-1);
                var ds = facVehicle.GetGPSData(vehicleId, dteStartDate.SelectedDate.Value, dteEndDate.SelectedDate.Value);
                if (ds == null || ds.Tables[0].Rows.Count == 0)
                {
                    lblError.Text = "There is no data to show";
                    return;
                }

                Session["__ExportDS"] = ds.Tables[0];
                Server.Transfer("../Reports/csvexport.aspx?filename=vehicle-tracking-export.csv");
            }
        }

        #endregion


        #region Validation

        protected void cfvStartDate_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = false;

            if (dteStartDate.SelectedDate <= dteEndDate.SelectedDate)
                args.IsValid = true;
        }

        #endregion

    }
}