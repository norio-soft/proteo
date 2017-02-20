using System;
using System.Collections;
using System.Configuration;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

using Telerik.Web.UI;

namespace Orchestrator.WebUI.resource.vehicle
{
    /// <summary>
    /// Summary description for VehicleList.
    /// </summary>
    public partial class VehicleList : Orchestrator.Base.BasePage
    {
        #region Form Elements

        protected System.Web.UI.WebControls.Button cmdAddUser;
        CheckBox chkIncludeDeleted = null;

        #endregion

        #region Page Elements

        private int searchType = 0;
        private bool? _enableVehicleNominalCodes = null;

        public bool EnableVehicleNominalCodes
        {
            get
            {
                if (!_enableVehicleNominalCodes.HasValue)
                {
                    _enableVehicleNominalCodes = bool.Parse(ConfigurationManager.AppSettings["EnableVehicleNominalCodes"].ToString());
                }

                return (bool)_enableVehicleNominalCodes;
            }
        }

        #endregion

        #region Page Init/Load/Error

        protected void Page_Load(object sender, System.EventArgs e)
        {
            Orchestrator.WebUI.Security.Authorise.EnforceAuthorisation(eSystemPortion.GeneralUsage);

            if (Request.QueryString["searchType"] != null)
                searchType = Convert.ToInt32(Request.QueryString["searchType"]);

            if (!IsPostBack)
            {
                if (Request.QueryString["showAvailable"] != null)
                {
                    rdiStartDate.SelectedDate = DateTime.Today;
                    rdiEndDate.SelectedDate = DateTime.Today;
                    fsDateFilter.Visible = true;
                }
                else
                    PopulateVehicles();
            }

            lblNote.Visible = false;
        }

        protected void chkIncludeDeleted_CheckedChanged(object sender, EventArgs e)
        {
            CheckBox cb = sender as CheckBox;
            ViewState["chkIncludeDeletedValue"] = cb.Checked;
            grdVehicles.Rebind();
        }

        protected void chkIncludeDeleted_PreRender(object sender, EventArgs e)
        {
            if (IsPostBack && ViewState["chkIncludeDeletedValue"] != null)
            {
                CheckBox cb = sender as CheckBox;
                cb.Checked = (bool)ViewState["chkIncludeDeletedValue"];
            }
        }

        protected void VehicleList_Init(object sender, EventArgs e)
        {
            this.grdVehicles.NeedDataSource += new Telerik.Web.UI.GridNeedDataSourceEventHandler(grdVehicles_NeedDataSource);
            this.grdVehicles.DataBound += new EventHandler(grdVehicles_DataBound);
            //this.btnFilter.Click += new EventHandler(btnFilter_Click);
            this.dlgAddUpdateVehicle.DialogCallBack += new EventHandler(dlgAddUpdateVehicle_DialogCallBack);
            this.grdVehicles.ItemDataBound += new Telerik.Web.UI.GridItemEventHandler(grdVehicles_ItemDataBound);
        }

        void grdVehicles_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            if (e.Item is GridCommandItem)
            {
                HtmlInputButton btnAddVehicle = e.Item.FindControl("btnAddVehicle") as HtmlInputButton;
                btnAddVehicle.Attributes.Add("onclick", dlgAddUpdateVehicle.GetOpenDialogScript());
            }
            if (e.Item is GridDataItem)
            {
                HtmlAnchor hypAddUpdateVehicle = e.Item.FindControl("hypAddUpdateVehicle") as HtmlAnchor;
                DataRowView drv = (DataRowView)e.Item.DataItem;

                hypAddUpdateVehicle.InnerText = drv["RegNo"].ToString();
                hypAddUpdateVehicle.HRef = string.Format("javascript:{0}", dlgAddUpdateVehicle.GetOpenDialogScript("resourceid=" + drv["ResourceId"].ToString()));
            }
        }

        void dlgAddUpdateVehicle_DialogCallBack(object sender, EventArgs e)
        {
            lblNote.Text = this.ReturnValue;
            lblNote.Visible = true;
            this.grdVehicles.Rebind();
        }

        void grdVehicles_DataBound(object sender, EventArgs e)
        {
            // Hide the nominal code column based on configuration settings.
            if (!EnableVehicleNominalCodes)
            {
                foreach (Telerik.Web.UI.GridColumn col in grdVehicles.Columns)
                {
                    if (col.UniqueName == "NominalCode")
                    {
                        col.Display = false;
                        return;
                    }
                }
            }
        }

        void grdVehicles_NeedDataSource(object source, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            GridItem cmdItem = null;
            CheckBox chkIncludeDeleted = null;
            if(grdVehicles.Items.Count > 0)
            {
                cmdItem = grdVehicles.MasterTableView.GetItems(GridItemType.CommandItem)[0];
                chkIncludeDeleted = cmdItem.FindControl("chkIncludeDeleted") as CheckBox;
            }
            
            DataSet dsVehicles = GetVehicleData(false);
            if (dsVehicles != null)
            {
                grdVehicles.DataSource = dsVehicles;
                if (grdVehicles.Items.Count != 0)
                {
                    grdVehicles.Visible = true;
                }

                if (chkIncludeDeleted != null && chkIncludeDeleted.Checked)
                    this.grdVehicles.DataSource = GetVehicleData(true);
                else
                    this.grdVehicles.DataSource = GetVehicleData(false);

            }
        }

        //void MyClientSideAnchor_OnWindowClose(Codesummit.WebModalAnchor sender)
        //{
        //    lblNote.Text = sender.OutputData;
        //    lblNote.Visible = true;
        //    PopulateVehicles();
        //}

        void btnFilter_Click(object sender, EventArgs e)
        {
            //this.Header1.title = "Available Vehicles";
            PopulateVehicles();
        }

        #endregion

        #region Vehicle Data Grid

        ///	<summary> 
        ///	Data Set Get Vehicle Data
        ///	</summary
        private DataSet GetVehicleData(bool inlcudeDeleted)
        {
            Facade.IVehicle facResource = new Facade.Resource();
            DataSet dsVehicles = null;
            Facade.IResource facTrailer = new Facade.Resource();

            if (Request.QueryString["showAvailable"] != null)
            {
                DateTime startDate = rdiStartDate.SelectedDate.Value;
                startDate = startDate.Subtract(startDate.TimeOfDay);

                DateTime endDate = rdiEndDate.SelectedDate.Value;
                endDate = endDate.Subtract(endDate.TimeOfDay).Add(new TimeSpan(23, 59, 59));

                dsVehicles = facTrailer.GetAvailableForDateRange(eResourceType.Vehicle, startDate, endDate);

                if (Orchestrator.Globals.Configuration.FleetMetrikInstance)
                    dsVehicles.Tables[0].Columns.Add("CurrentLocation", typeof(string));
            }
            else if (Orchestrator.Globals.Configuration.FleetMetrikInstance)
            {
                dsVehicles = facResource.GetAllWithGPS(inlcudeDeleted);
                dsVehicles.Tables[0].Columns.Add("CurrentLocation", typeof(string));
            }
            else
            {
                dsVehicles = facResource.GetAllVehicles();
                if (Orchestrator.Globals.Configuration.FleetMetrikInstance)
                    dsVehicles.Tables[0].Columns.Add("CurrentLocation", typeof(string));
            }


            return dsVehicles;
        }

        ///	<summary> 
        ///	Populate Vehicles
        ///	</summary>
        private void PopulateVehicles()
        {
            DataSet dsVehicles = null;

            if (chkIncludeDeleted != null && chkIncludeDeleted.Checked)
                dsVehicles = GetVehicleData(true);
            else
                dsVehicles = GetVehicleData(false);


            if (dsVehicles != null)
            {
                grdVehicles.DataSource = dsVehicles;
                grdVehicles.DataBind();
                if (grdVehicles.Items.Count != 0)
                    grdVehicles.Visible = true;
            }
        }

        #endregion

        #region Methods & Events

        ///	<summary> 
        ///	Buttom Add Vehicle Click
        ///	</summary
        private void btnAddVehicle_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("addupdatevehicle.aspx");
        }

        #endregion

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();

            base.OnInit(e);

            if(Orchestrator.Globals.Configuration.FleetMetrikInstance)
            {
                this.grdVehicles.Columns.FindByUniqueName("NominalCode").Visible = false;
                this.grdVehicles.Columns.FindByUniqueName("RegularDriver").Visible = false;
                this.grdVehicles.Columns.FindByUniqueName("IsFixedUnit").Visible = false;
                this.grdVehicles.Columns.FindByUniqueName("ThirdPartyIntegrationID").Visible = false;
                this.grdVehicles.Columns.FindByUniqueName("MOTExpiry").Visible = false;
                this.grdVehicles.Columns.FindByUniqueName("TelephoneNumber").Visible = false;
                this.grdVehicles.Columns.FindByUniqueName("DepotCode").Visible = false;
                this.grdVehicles.Columns.FindByUniqueName("ResourceID").Visible = false;
               ((GridTemplateColumn)this.grdVehicles.Columns.FindByUniqueName("IsTTInstalled")).Visible = false;
                ((GridTemplateColumn)this.grdVehicles.Columns.FindByUniqueName("IsTTInstalled")).ItemTemplate = null;

                this.grdVehicles.Columns.FindByUniqueName("GPSLastUpdate").Visible = true;
                this.grdVehicles.Columns.FindByUniqueName("GPSLastUpdateType").Visible = true;
                this.grdVehicles.Columns.FindByUniqueName("GPSStatus").Visible = true;

                this.grdVehicles.Columns.FindByUniqueName("RegNo").ItemStyle.Width = 170;
                this.grdVehicles.Columns.FindByUniqueName("VehicleManufacturer").ItemStyle.Width = 170;
                this.grdVehicles.Columns.FindByUniqueName("GPSUnitID").ItemStyle.Width = 170;
                this.grdVehicles.Columns.FindByUniqueName("CurrentLocation").ItemStyle.Width = 400;
            }
            else
            {
                ((GridTemplateColumn)this.grdVehicles.Columns.FindByUniqueName("GPSStatus")).ItemTemplate = null;
            }
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.Init += new System.EventHandler(this.VehicleList_Init);

        }
        #endregion

    }
}
